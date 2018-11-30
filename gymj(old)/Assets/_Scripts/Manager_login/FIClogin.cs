using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using cn.sharesdk.unity3d;
using MJBLL.common;
using DNL;
using System.IO;
using System.Text;
using System.Xml;

public class FIClogin : MonoBehaviour
{
    //虚拟的登录跳转
    // public string openid = "1";
    //异步对象  
    AsyncOperation asyncOperation;

    string nickname = "";
    string sex = "2";
    string province = "guizhou";
    string city = "guiyang";
    public string[] headimg;
    string unionid = "1";
    bool isClosed = false;
    
    private  Text Err_Tip;

    GameObject Img_Loading;
    Button loginButton;

    Button longBaoLoginButton;

    Transform loginUserPanelTrans;
    InputField inputUserName;
    InputField inputPasword;
    Button btnZhanghao;
    Button closeButton;

    //GameObject warnPanelForWorld;
    //public Text Txt_Loading;
    public Image errTipBGImage;
    //copy form old 
    public static ShareSDK myShareSdk;
    private string objname;
    private Toggle agreementTge;
    //copy end
    void Start()
    {//========ceshi
        //GameInfo.certificate = "H4sIAAAAAAAEAA1QyQ0AQQhqyVv36dl/STuJDyMIaBYEkIl+Zj46KeEUc1yByoB7bw57XSpoJgF+CSAP2IOhSh5LnLjTpKqzpA7eLjYo+QxBvxS7LKtHQHCaALRkjAg9n9t4/WV+35OdV8tIGIXqCuTdPFoi80mI9XaA9SmqHGSijF4ruaA+MLdkl2VF4ksyjho3eSmJ40hMAZ6Z2YCpd33xwkmcQ9CQMETV0+vvu/lqjQe4ByRdqd+davR2YNtbWMhvxKG+mgsRjFzzNkcbIYpVtlVa1BaKKIYUBVGDj68EifIZJupIz/f+hi/hl+DleEsrUT/oJ33coAEAAA==";
        //GameInfo.province = "贵州";
       /// GameInfo.city = "贵阳";
        //=============
		//设置场景标签为login
        GameInfo.sceneWhich = SceneID.Login;
		//关闭多点触控
        Input.multiTouchEnabled = false;
		//设置手机屏幕永不休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//mobSDK4连
        myShareSdk = Camera.main.transform.GetComponent<ShareSDK>();
        //myShareSdk.authHandler = AuthResultHandler;
        myShareSdk.showUserHandler = GetUserInfoResultHandler;
        myShareSdk.shareHandler = FICStartGame.ShareResultHandler;
		//服务器类型为列表服务器
        GameInfo.cs.serverType = ServerType.ListServer;
		//微信登录按钮,由OnPointerClick()负责
        loginButton = transform.Find("loginBtn").gameObject.GetComponent<Button>();
		//龙宝登录按钮
        longBaoLoginButton = transform.Find("loginUserBtn").gameObject.GetComponent<Button>();
		//龙宝登录按钮-账号密码输入界面
        loginUserPanelTrans = transform.Find("loginPWB");
		//账号密码输入界面-账号
        inputUserName = loginUserPanelTrans.Find("InputUsername").GetComponent<InputField>();
		//账号密码输入界面-密码
        inputPasword = loginUserPanelTrans.Find("InputPassword").GetComponent<InputField>();
		//账号密码输入界面-登录按钮
        btnZhanghao = loginUserPanelTrans.Find("loginBtn").GetComponent<Button>();
		//账号密码输入界面-关闭按钮
        closeButton = loginUserPanelTrans.Find("closeBtn").GetComponent<Button>();
		//登录协议确认选框
        agreementTge = transform.Find("/Canvas/agreementTge").gameObject.GetComponent<Toggle>();

        longBaoLoginButton.onClick.AddListener(LongbaoDenglu);
        btnZhanghao.onClick.AddListener(ZhanghaoDenglu);
        closeButton.onClick.AddListener(CloseZhanghaoPanel);
        //// GameInfo.Xintiao()
        //warnPanelForWorld = Resources.Load<GameObject>("Game_GYMJ/Prefabs/warningPanelForWorld");
        //GameObject go= GameObject.Instantiate(warnPanelForWorld);
        //DontDestroyOnLoad(go);

      

    }
	//copy end
	public void Update()
	{
		//如果不是没有返回登录信息
		if (GameInfo.returnlogin != null)
		{
			switch (GameInfo.returnlogin.loginstat)
			{
			case 1:
				isClosed = true;
				GameInfo.userID = GameInfo.returnlogin.UserID;
				GameInfo.userRoomCard = GameInfo.returnlogin.UserRoomCard;
				if (!string.IsNullOrEmpty(GameInfo.returnlogin.headimg))
				{
					GameInfo.HeadImg = GameInfo.returnlogin.headimg;
				}
				if (!string.IsNullOrEmpty(GameInfo.returnlogin.unionid))
				{
					GameInfo.unionid = GameInfo.returnlogin.unionid;
					//?
					GameInfo.OpenID = GameInfo.returnlogin.unionid;
				}
				if (!string.IsNullOrEmpty( GameInfo.returnlogin.Certificate))
				{
					GameInfo.certificate = GameInfo.returnlogin.Certificate;
					WirteConfigFile();
				}
				if (!string.IsNullOrEmpty(GameInfo.returnlogin.UserName))
				{
					GameInfo.NickName = GameInfo.returnlogin.UserName;
				}
				GameInfo.sceneID = "Scene_Hall";
				SceneManager.LoadScene("LoadingHall");
				break;
				//登录失败,弹出微信登录和龙宝登录
			case 2:
				LoginError();
				FICWaringPanel._instance.Show("登录失败!");
				GameInfo.certificate = "";
				EnableLoginButtons();
				break;
			case 3:
				//显示账号密码输入界面，隐藏微信登录按钮和龙宝登录按钮
			case 4:
				ShowZhanghaoPanel();
				HideLoginButtons();
				break;
				//账号密码错误，显示账号密码输入界面
			case 5:
				FICWaringPanel._instance.Show("账号密码错误!");
				ShowZhanghaoPanel();
				break;
			}
			GameInfo.returnlogin = null;
		}
		//监听手机返回键
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			FICWaringPanel._instance.ShowQuit();
			FICWaringPanel._instance.WarnningMethods = delegate { Application.Quit(); };
		}
	}
		
	//龙宝登录按钮监听方法
    void LongbaoDenglu()
    {
		
        if (!agreementTge.isOn)
        {
            FICWaringPanel._instance.Show("请阅读并同意用户协议！");
            return;
        }
		//定位
        GPSManager.instance.StartCrt();
		//如果证书不为空
        if (!string.IsNullOrEmpty(GameInfo.certificate))
        {
            SendLoginPW sendLoginPW = new SendLoginPW();
            
            sendLoginPW.province = GameInfo.province;
            sendLoginPW.city = GameInfo.city;
            sendLoginPW.Latitude = GameInfo.Latitude;
            sendLoginPW.Certificate = GameInfo.certificate;
            byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendLoginPW);
            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1003, body.Length, 0, body);
            GameInfo.cs.Send(data);
            DisableLoginButtons();
            GameInfo.loginType = LoginType.longbao;
        }
        else
        {
            HideLoginButtons();
            ShowZhanghaoPanel();
        }
    }
	//隐藏微信登录按钮和龙宝登录按钮
	void HideLoginButtons()
	{
		loginButton.gameObject.SetActive(false);
		longBaoLoginButton.gameObject.SetActive(false);
	}
	//显示账号密码输入界面
	void ShowZhanghaoPanel()
	{
		loginUserPanelTrans.gameObject.SetActive(true);
	}
	//账号密码输入界面-登录按钮监听方法
	void ZhanghaoDenglu()
	{
		if (!agreementTge.isOn)
		{
			FICWaringPanel._instance.Show("请阅读并同意用户协议！");
			return;
		}
		//定位
		GPSManager.instance.StartCrt();
		//SendLoginPW来自于谷歌插件
		SendLoginPW sendLoginPW = new SendLoginPW();
		sendLoginPW.UserAccount = inputUserName.text;
		sendLoginPW.pwd = inputPasword.text;
		//省份
		sendLoginPW.province =  GameInfo.province;
		//城市
		sendLoginPW.city = GameInfo.city;
		//维度
		sendLoginPW.Latitude = GameInfo.Latitude;

		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendLoginPW);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1003, body.Length, 0, body);
		GameInfo.cs.Send(data);
		HideZhanghaoPanel();

		GameInfo.loginType = LoginType.longbao;
	}
	//账号密码输入界面-关闭按钮监听方法
	void CloseZhanghaoPanel()
	{
		HideZhanghaoPanel();
		ShowLoginButtons();
	}
	//隐藏账号密码输入界面
	void HideZhanghaoPanel()
	{
		loginUserPanelTrans.gameObject.SetActive(false);
	}
	//显示微信登录按钮和龙宝登录按钮
	void ShowLoginButtons()
	{
		loginButton.gameObject.SetActive(true);
		//longBaoLoginButton.gameObject.SetActive(true);
	}
	//设置微信登录按钮和龙宝登录按钮的按钮起效
    void EnableLoginButtons()
    {
        loginButton.enabled = true;
        //longBaoLoginButton.enabled = true;
        ShowLoginButtons();
    }
	//设置微信登录按钮和龙宝登录按钮的按钮失效
    void DisableLoginButtons()
    {
        loginButton.enabled = false;
        longBaoLoginButton.enabled = false;
    }
    public Text Txt_Loading;
	//房卡登录模式（？微信登录模式）
    public void OnPointerClick()
    {
        if (!agreementTge.isOn)
        {
            FICWaringPanel._instance.Show("请阅读并同意用户协议！");
            return;
        }
        GPSManager.instance.StartCrt();

        myShareSdk.GetUserInfo(PlatformType.WeChat);
        GetTestUserInfo();
        //DisableLoginButtons();

        GameInfo.loginType = LoginType.fangka;
       
        OutLog.log("点击登陆 !");
        Txt_Loading.text = "点击登录";
    }
    //一个暂时的测试方法
    private void GetTestUserInfo()
    {
        Debug.Log("ERITOR 测试");
//#if UNITY_EDITOR
        GameInfo.province = "贵州";
        GameInfo.city = "贵阳";
        Debug.Log(GameInfo.Latitude);
        //GameInfo.Latitude = "0,0";
        GameInfo.Latitude = UnityEngine.Random.Range(1f, 100f).ToString() + " , " + UnityEngine.Random.Range(1f, 100f).ToString();
        Debug.Log("lit : " + GameInfo.Latitude); 
//#endif
        GameInfo.Sex = 0;// UnityEngine.Random.Range(1,3);
        
        //？
        GameInfo.unionid = GameInfo.OpenID;
        GameInfo.NickName = "测试用户" + GameInfo.OpenID.Substring(0, 3);
        GameInfo.HeadImg = GameInfo.Sex == 1 ? headimg[UnityEngine.Random.Range(5, headimg.Length)] : headimg[UnityEngine.Random.Range(0, 5)];
        
        GameInfo.cs.SentUserLoginMessage(GameInfo.OpenID, GameInfo.NickName, GameInfo.Sex.ToString(), GameInfo.province, GameInfo.city, GameInfo.HeadImg, GameInfo.unionid, GameInfo.Latitude);
       
        //微信
        //GameInfo.Sex = int.Parse(data["sex"].ToString());
        //GameInfo.province = data["province"].ToString();
        //GameInfo.city = data["city"].ToString();
        //GameInfo.unionid = data["unionid"].ToString();

        //GameInfo.NickName = "测试用户";
        //GameInfo.OpenID = "abcdefghighklmn789654123";
        //GameInfo.HeadImg = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1491977284065&di=2df0abbda86b7c08d617ccebc795222a&imgtype=0&src=http%3A%2F%2Fimg01.appcms.cc%2F20140729%2Ft01a%2Ft01a1b9594e2775c715.png";
        //GameInfo.cs.SentUserLoginMessage(GameInfo.OpenID, GameInfo.NickName, GameInfo.Sex.ToString(), GameInfo.province, GameInfo.city, GameInfo.HeadImg, GameInfo.unionid);
    }

    void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable data)
    {
        //Txt_Loading.text = "获取授权回调";
        if (state == ResponseState.Success)
        {
            //OutLog.PrintLog(MiniJSON.jsonEncode(result));
            //授权成功的话，获取用户信息  
            // Txt_Loading.text = "授权成功";
            //myShareSdk.GetUserInfo(PlatformType.WeChat);
            OutLog.log("获取授权，" + state + "::" + MiniJSON.jsonEncode(data));
            if (state == ResponseState.Success)
            {
                try
                {
                    GameInfo.NickName = data["nickname"].ToString();
                    GameInfo.OpenID = data["openid"].ToString();
                    GameInfo.HeadImg = data["headimgurl"].ToString();
                    GameInfo.Sex = int.Parse(data["sex"].ToString());
                    //GameInfo.province = data["province"].ToString();
                    //GameInfo.city = data["city"].ToString();
                    GameInfo.unionid = data["unionid"].ToString();
                    //OutLog.PrintLog(MiniJSON.jsonEncode(data));
                    // Txt_Loading.text = "获取用户信息成功";
                    OutLog.log("微信获取openid：" + GameInfo.OpenID);
                    GameInfo.cs.SentUserLoginMessage(GameInfo.OpenID, GameInfo.NickName, GameInfo.Sex.ToString(), GameInfo.province, GameInfo.city, GameInfo.HeadImg, GameInfo.unionid, GameInfo.Latitude);

                }
                catch (Exception ex)
                {
                    EnableLoginButtons();
                    // Txt_Loading.text = ex.ToString();
                    OutLog.log("获取用户信息出错" + ex.ToString());
                }
            }

        }
        else if (state == ResponseState.Fail)
        {
            EnableLoginButtons();
            // Txt_Loading.text = "授权失败";
            //OutLog.PrintLog("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
        }
        else if (state == ResponseState.Cancel)
        {
            EnableLoginButtons();
            //  Txt_Loading.text = "授权取消";
            //OutLog.PrintLog("cancel !");
        }
    }
    private void GetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable data)
    {
        //Txt_Loading.text = "获取用户信息回调";
		OutLog.log("获取用户消息，"+state + "::" + MiniJSON.jsonEncode(data));
        if (state == ResponseState.Success)
        {
            try
            {
                GameInfo.NickName = data["nickname"].ToString();
                GameInfo.OpenID = data["openid"].ToString();
                GameInfo.HeadImg = data["headimgurl"].ToString();
                GameInfo.Sex = int.Parse(data["sex"].ToString());
                //GameInfo.province = data["province"].ToString();
                //GameInfo.city = data["city"].ToString();
                GameInfo.unionid = data["unionid"].ToString();
                //OutLog.PrintLog(MiniJSON.jsonEncode(data));
                // Txt_Loading.text = "获取用户信息成功";
                OutLog.log("微信获取openid："+GameInfo.OpenID);
                //Txt_Loading.text += "微信获取openid：" + GameInfo.OpenID;
                GameInfo.Latitude = UnityEngine.Random.Range(1f, 1000f).ToString() + " , " + UnityEngine.Random.Range(1f, 1000f).ToString();
                GameInfo.cs.SentUserLoginMessage(GameInfo.OpenID, GameInfo.NickName, GameInfo.Sex.ToString(), GameInfo.province, GameInfo.city, GameInfo.HeadImg, GameInfo.unionid, GameInfo.Latitude);

            }
            catch (Exception ex)
            {
               // Txt_Loading.text = ex.ToString();
                OutLog.log("获取用户信息出错"+ex.ToString());
                EnableLoginButtons();
                //Err.text = ex.StackTrace + ex.Message;
            }
        }
        else if (state == ResponseState.Cancel)
        {
            //Txt_Loading.text = "获取用户信息取消";
            OutLog.log("获取用户信息取消");
            EnableLoginButtons();
        }
        else if (state == ResponseState.Fail)
        {
            // Txt_Loading.text = "获取用户信息失败";
            OutLog.log("获取用户信息失败");
            myShareSdk.Authorize(PlatformType.WeChat);
            EnableLoginButtons();
        }
    }
	//编写证书，以方便下次直接登录
    void WirteConfigFile()
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.persistentDataPath + "/Config.xml");
        XmlNode node = xml.SelectSingleNode("data/Certificate");
        node.InnerText = GameInfo.certificate;
        xml.Save(Application.persistentDataPath + "/Config.xml");
    }
    //IEnumerator loadScene(string sceneName)
    //{
    //    yield return asyncOperation = Application.LoadLevelAsync(sceneName);
    //    print("load Complete!");
    //}

    void LoginError()
    {
        //Err_Tip.gameObject.SetActive(true);
        errTipBGImage.gameObject.SetActive(true);
        //Err_Tip.DOFade(0, 1f).SetLoops(3);
        //Err_Tip.text = "登录失败!";
        Invoke("Close_ErrTip", 3);
    }

    public void Close_ErrTip()
    {
        //Err_Tip.text = null;
        //Err_Tip.GetComponent<Text>().DOFade(1, 0);
        //Err_Tip.gameObject.SetActive(false);
        errTipBGImage.gameObject.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        GameInfo.cs.Closed();
        if(GameInfo.cs.myThread != null)
        GameInfo.cs.myThread.Abort();
    }
}
