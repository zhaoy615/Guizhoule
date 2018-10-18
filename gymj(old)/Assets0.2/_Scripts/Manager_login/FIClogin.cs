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
    //GameObject warnPanelForWorld;
    //public Text Txt_Loading;
    public Image errTipBGImage;
    //copy form old 
    public static ShareSDK myShareSdk;
    private string objname;
    private Toggle agreementTge;
    //copy end
    void Start()
    {
        Input.multiTouchEnabled = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        myShareSdk = Camera.main.transform.GetComponent<ShareSDK>();
        myShareSdk.authHandler = AuthResultHandler;
        myShareSdk.showUserHandler = GetUserInfoResultHandler;
        myShareSdk.shareHandler = FICStartGame.ShareResultHandler;
        GameInfo.cs.serverType = ServerType.ListServer;
        loginButton = transform.Find("/Canvas/loginBtn").gameObject.GetComponent<Button>();
        agreementTge = transform.Find("/Canvas/agreementTge").gameObject.GetComponent<Toggle>();
        //// GameInfo.Xintiao()
        //warnPanelForWorld = Resources.Load<GameObject>("Game_GYMJ/Prefabs/warningPanelForWorld");
        //GameObject go= GameObject.Instantiate(warnPanelForWorld);
        //DontDestroyOnLoad(go);

    }
    public void OnPointerClick()
    {
        if (!agreementTge.isOn)
        {
            FICWaringPanel._instance.Show("请阅读并同意用户协议！");
            return;
        }
            GPSManager.instance.StartCrt();
            GetTestUserInfo();
            loginButton.enabled = false;
        
       // myShareSdk.Authorize(PlatformType.WeChat);
       //myShareSdk.GetUserInfo(PlatformType.WeChat);
		OutLog.log("点击登陆 !");
        //Txt_Loading.text = "点击登录";
    }
    private void GetTestUserInfo()
    {

        GameInfo.Sex = 0;// UnityEngine.Random.Range(1,3);
        GameInfo.province = "贵州";
        GameInfo.city = "贵阳";
        GameInfo.unionid = GameInfo.OpenID;
        GameInfo.NickName ="测试用户" + GameInfo.OpenID.Substring(0,3);
        Debug.Log("登录");
         GameInfo.HeadImg = GameInfo.Sex == 1 ? headimg[UnityEngine.Random.Range(5, headimg.Length)] : headimg[UnityEngine.Random.Range(0, 5)];
        Debug.Log(GameInfo.Latitude);
        GameInfo.cs.SentUserLoginMessage(GameInfo.OpenID, GameInfo.NickName, GameInfo.Sex.ToString(), GameInfo.province, GameInfo.city, GameInfo.HeadImg, GameInfo.unionid, GameInfo.Latitude);
        OutLog.log("测试用户登录，openid："+GameInfo.OpenID);
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
                    GameInfo.province = data["province"].ToString();
                    GameInfo.city = data["city"].ToString();
                    GameInfo.unionid = data["unionid"].ToString();
                    //OutLog.PrintLog(MiniJSON.jsonEncode(data));
                    // Txt_Loading.text = "获取用户信息成功";
                    OutLog.log("微信获取openid：" + GameInfo.OpenID);
                    GameInfo.cs.SentUserLoginMessage(GameInfo.OpenID, GameInfo.NickName, GameInfo.Sex.ToString(), GameInfo.province, GameInfo.city, GameInfo.HeadImg, GameInfo.unionid, GameInfo.Latitude);

                }
                catch (Exception ex)
                {
                    loginButton.enabled = true;
                    // Txt_Loading.text = ex.ToString();
                    OutLog.log("获取用户信息出错" + ex.ToString());
                }
            }

        }
        else if (state == ResponseState.Fail)
        {
            loginButton.enabled = true;
            // Txt_Loading.text = "授权失败";
            //OutLog.PrintLog("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
        }
        else if (state == ResponseState.Cancel)
        {
            loginButton.enabled = true;
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
                GameInfo.province = data["province"].ToString();
                GameInfo.city = data["city"].ToString();
                GameInfo.unionid = data["unionid"].ToString();
                //OutLog.PrintLog(MiniJSON.jsonEncode(data));
                // Txt_Loading.text = "获取用户信息成功";
                OutLog.log("微信获取openid："+GameInfo.OpenID);
                GameInfo.cs.SentUserLoginMessage(GameInfo.OpenID, GameInfo.NickName, GameInfo.Sex.ToString(), GameInfo.province, GameInfo.city, GameInfo.HeadImg, GameInfo.unionid, GameInfo.Latitude);

            }
            catch (Exception ex)
            {
               // Txt_Loading.text = ex.ToString();
                OutLog.log("获取用户信息出错"+ex.ToString());
                loginButton.enabled = true;
                //Err.text = ex.StackTrace + ex.Message;
            }
        }
        else if (state == ResponseState.Cancel)
        {
            //Txt_Loading.text = "获取用户信息取消";
            OutLog.log("获取用户信息取消");
            loginButton.enabled = true;
        }
        else if (state == ResponseState.Fail)
        {
            // Txt_Loading.text = "获取用户信息失败";
            OutLog.log("获取用户信息失败");
            myShareSdk.Authorize(PlatformType.WeChat);
            loginButton.enabled = true;
        }
    }
    //copy end
    public void Update()
    {
        if (GameInfo.returnlogin != null)
        {
            //Txt_Loading.text = GameInfo.returnlogin.ToString();
            if (GameInfo.returnlogin.loginstat == 1)
            {
                isClosed = true;
                GameInfo.userID = GameInfo.returnlogin.UserID;
                GameInfo.userRoomCard = GameInfo.returnlogin.UserRoomCard;
                //Txt_Loading.text = "Scene_Hall";
                //SceneManager.LoadScene("Scene_Hall");
                //GameInfo.listIp = GameInfo.ip;
                //GameInfo.listPort = GameInfo.port;
                GameInfo.sceneID = "Scene_Hall";
                SceneManager.LoadScene("LoadingHall");
                //StartCoroutine("loadScene", "Scene_Hall");
                //Img_Loading.SetActive(true);
            }
            //判断异步对象并且异步对象没有加载完毕，显示进度  
            //if (asyncOperation != null && !asyncOperation.isDone)
            //{
            //    //GUILayout.Label("progress:" + (float)asyncOperation.progress * 100 + "%");
            //    Txt_Loading.text = ("progress:" + (float)asyncOperation.progress * 100 + "%");
            //}

            else if (GameInfo.returnlogin.loginstat == 2)
            {
                LoginError();
                FICWaringPanel._instance.Show("登录失败!");
                loginButton.enabled = true;
            }
            GameInfo.returnlogin = null;

            ///////////////////////////////////////////////////////////
            //if (GameInfo.returnAddServer != null )
            //{
            //    GameInfo.status = GameInfo.returnServerIP.Status;
            //    if (GameInfo.status == 2)
            //    {
            //        GameInfo.room_id = int.Parse(GameInfo.returnServerIP.RoomID);
            //    }
                
            //    GameInfo.returnServerIP = null;
            //    SendAddServer sendAddServer = SendAddServer.CreateBuilder()
            //    .SetOpenid(GameInfo.OpenID)
            //    .SetUnionid(GameInfo.unionid)
            //    .Build();
            //    byte[] body = sendAddServer.ToByteArray();
            //    byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1011, body.Length, 0, body);
            //    GameInfo.cs.Send(data);
            //}
            ///////////////////////////////////////////////////////////
        }
        else if(GameInfo.returnServerIP != null)
        {
            if (GameInfo.returnServerIP.Status == 1)
            {
                GameInfo.cs.Closed();
                GameInfo.listIp = GameInfo.returnServerIP.ip;
                GameInfo.listPort = GameInfo.returnServerIP.port;
                GameInfo.cs.SentUserLoginMessage(GameInfo.OpenID, GameInfo.NickName, GameInfo.Sex.ToString(), GameInfo.province, GameInfo.city, GameInfo.HeadImg, GameInfo.unionid, GameInfo.Latitude);
                GameInfo.returnServerIP = null;
            }
            else
            {
                GameInfo.sceneID = "Scene_Hall";
                SceneManager.LoadScene("LoadingHall");
            }
            
            //if (GameInfo.returnAddServer.Status == 1)
            //{
            //    SendConnData sendAddServer = SendConnData.CreateBuilder()
            //    .SetOpenid(GameInfo.OpenID)
            //    .SetRoomID(GameInfo.room_id)
            //    .Build();
            //    byte[] body = sendAddServer.ToByteArray();
            //    byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1011, body.Length, 0, body);
            //    GameInfo.cs.Send(data);
            //}
            

        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            FICWaringPanel._instance.ShowQuit();
            FICWaringPanel._instance.WarnningMethods = delegate { Application.Quit(); };
        }
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
