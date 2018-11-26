using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MJBLL.common;
using cn.sharesdk.unity3d;
using DNL;

public class Manager_Hall : MonoBehaviour
{

    public Text userTip;
    public Text nickName;
    public Text ID;
    public Text RoomCardNum;
    public GameObject reconTip;
    public Image errTipBGImage;
    public Image headImage;
    public Image longOrFang;
    string headimg = GameInfo.HeadImg;
    public bool isClosed = false;

    //2017.8.1連接場景 
    public GameObject NoticeInfo;      //公告面板
    public GameObject CreateRoom_pel; //创建房间面板
    public GameObject Rule_Pel;   // 规则面板
    public GameObject notice_Pel; //通知面板
    public GameObject mask_bg;//背景遮罩
    public GameObject Recon_Pel;//断线重连
    Coroutine AnnouncementCourse; //公告进程
    public Sprite[] ShareTt;//微信分享按钮贴图
    private Manager_HallAudio managerAudio; ///申明脚本Manager_Audio
    private Manager_Hall music;
    private AudioSource _ArrayAudioSources;
    private Button shareButtonAtHall;
    // Use this for initialization
    void Start()
    {
      //房卡图片
        longOrFang = transform.Find("/Game_UI/One_UI/gold").GetComponent<Image>();
		//根据当前的登录类型（龙宝or房卡(?微信)），改变计费单位的图片
		ChangeLongOrFang();
		//设置场景标签为主界面
        GameInfo.sceneWhich = SceneID.Hall;
		//分享按钮
        shareButtonAtHall = transform.Find("/Game_UI/One_UI/ShareOne_Btn").GetComponent<Button>();
		shareButtonAtHall.onClick.AddListener(OnShareButtonAtHallClick);
		//
        GameInfo.isYuanQue = 0;
		//获取音乐播放器
        _ArrayAudioSources = GameObject.Find("Audio Source").GetComponent<AudioSource>();
		//给所有按键绑定音乐播放方法
        allBtnVoiceObj();
		//禁止休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//如果服务器类型是游戏，则变为列表服务器
        if (GameInfo.cs.serverType == ServerType.GameServer)
        {
            GameInfo.cs.Closed();
            GameInfo.cs.serverType = ServerType.ListServer;
        }
		//5秒后开始发送心跳包
        Invoke("Timer", 5.0f);

        userTip.text = null;
        nickName.text = GameInfo.NickName;
        ID.text = "ID:" + GameInfo.userID;
        SendGetRoomCard();

        RoomCardNum.text = GameInfo.userRoomCard + "";
		//加载网络头像，heading是Url,headImage是报错时的预备图片
//2018.3.21为了测试而注释------------------------------------------------------
        LoadImage.Instance.LoadPicture(headimg, headImage);
		//
        StartCoroutine(Show_NoticeInfo());
		//是否已经开始游戏
        GameInfo.isAllreadyStart = false;
		//重连提示
        Recon_Pel = transform.Find("/Game_UI/Two_UI/Recon_Pel").gameObject;
        //==========================脚本单例==========================//

        music = gameObject.GetComponent<Manager_Hall>();
        managerAudio = GameObject.Find("Main Camera").GetComponent<Manager_HallAudio>();
		//关闭登录的按钮
//2018.3.21为了测试而注释------------------------------------------------------
        FICWaringPanel._instance.Hide();
    }
	void ChangeLongOrFang()
	{
		switch (GameInfo.loginType)
		{
		case LoginType.none:
			break;
		case LoginType.longbao:
			longOrFang.sprite = Resources.Load<Sprite>("Hall/Texture/主页面/功能按钮/龙宝");
			break;
		case LoginType.fangka:
			longOrFang.sprite = Resources.Load<Sprite>("Hall/Texture/主页面/功能按钮/房卡");
			break;
		default:
			break;
		}
	}

	//mobSDK相关的分享
	private void OnShareButtonAtHallClick()
	{
		//Application.CaptureScreenshot("Screenshot.png");
		StartCoroutine(JiePingTime(0.5f));

	}
	//text	title	url	imageUrl/imagePath	shareType	musicUrl	filePath

	private IEnumerator JiePingTime(float time)
	{
		yield return new WaitForSeconds(0.5f);


		ShareContent content = new ShareContent();
		content.SetText("等你乐麻将，贵阳本土的麻将");//没显示
		//content.SetImagePath(imagePath);//显示了
		content.SetImageUrl("http://qy-imageserver.oss-cn-shenzhen.aliyuncs.com/20171219115836.jpg");
		content.SetTitle("等你乐麻将\n等你乐麻将，贵阳本土的麻将");//显示了
		content.SetUrl("http://download.gzqyrj.com/download/html/majhong.html");//显示了
		//content.SetShareType(ContentType.Image);
		content.SetShareType(ContentType.Webpage);
		// FIClogin.myShareSdk.ShowPlatformList(null, content, 100, 100);

		//FIClogin.myShareSdk.ShowShareContentEditor(PlatformType.WeChat, content);
		FIClogin.myShareSdk.ShowShareContentEditor(PlatformType.WeChatMoments, content);
	}

	/// <summary>
	/// 大厅所有按钮对象音效
	/// </summary>
	public void allBtnVoiceObj()
	{
		//===========================================大厅其他按钮================================================//
		transform.Find("/Game_UI/One_UI/CreateRoom_Btn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); }); ///创建房间音效
		transform.Find("/Game_UI/One_UI/JoinRoom_Btn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///加入房间音效
		transform.Find("/Game_UI/One_UI/feedback_Btn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });///公告音效
		transform.Find("/Game_UI/One_UI/News_Btn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///通知音效
		transform.Find("/Game_UI/One_UI/Rule_Btn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); }); ///创规音效
		transform.Find("/Game_UI/One_UI/setting_Btn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///设置音效
		transform.Find("/Game_UI/One_UI/ShareOne_Btn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); }); ///微信分享
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/Game_Type/Create_Btn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); }); ///创建房间里的创建房间按钮
		//===========================================关闭按钮=============================================//
		transform.Find("/Game_UI/Two_UI/News_Pel/close_Btn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///通知音效关闭按钮
		transform.Find("/Game_UI/PopUp_UI/SheZhi/But_Close").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///设置音效关闭按钮
		//===========================================创建房间选择按钮=============================================//
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/inningNum/8").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); });///8局
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/inningNum/16").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); });   ///16局
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/peopleNum/2").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); }); ///2人
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/peopleNum/3").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); });   ///3人
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/peopleNum/4").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); });///4人
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/JI_Type/BenJI").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); });   ///本鸡
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/JI_Type/ShangXiaJI").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); }); ///上下鸡
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/JI_Type/XingQiJI").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); });   ///星期鸡
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/JI_Type/WuGuJI").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); });///乌骨鸡
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/playType/LianZhuang").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); });   ///连庄
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/playType/1Kou2").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); }); ///1扣2
		//transform.Find("/Game_UI/Two_UI/CreateRoom_pel/playType/1Kou3").GetComponent<Toggle>().onValueChanged.AddListener(delegate { btnVoice("btn1"); });   ///通三

		//=============================================加入房间选择按钮=============================================//
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num1").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });///1
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num2").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///2
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num3").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); }); ///3
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num4").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///4
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num5").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });///5
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num6").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///6
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num7").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); }); ///7
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num8").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///8
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num9").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });///9
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_num0").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///0
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_clear").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); }); ///加入房间清空
		transform.Find("/Game_UI/Two_UI/Join_Pel/BG/btn/btn_backspace").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); });   ///家入房间删除
	}

	void Timer()
	{
		//心跳包,未完成
		int xintiao = GameInfo.Xintiao();
		//if (xintiao >= 2)
		//{
		//    //提示用户掉线
		//   // userTip.text = "服务器断开连接！\n等待网络恢复...";
		//    FICWaringPanel._instance.Show("服务器断开连接！\n等待网络恢复...");
		//   // FICWaringPanel._instance.WarnningMethods = delegate { transform.Find("/Game_UI/Two_UI/CreateRoom_pel/Game_Type/Create_Btn").gameObject.GetComponent<Button>().enabled = true; };
		//    errTipBGImage.gameObject.SetActive(true);
		//}
		//if (xintiao == 0)
		//{
		//    userTip.text = null;
		//    errTipBGImage.gameObject.SetActive(false);
		//}
		//发送用户信息
		SendGetRoomCard();
		//每隔2秒发送一次心跳包
		Invoke("Timer", 2.0f);
	}

	void SendGetRoomCard()
	{ 
		SendGetRoomCard sendGetRoomCard = new SendGetRoomCard();
		sendGetRoomCard.UserID = GameInfo.userID;
		sendGetRoomCard.openid = GameInfo.OpenID;
		sendGetRoomCard.unionid = GameInfo.unionid;
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGetRoomCard);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1022, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}
	//循环顯示公告欄信息
	IEnumerator Show_NoticeInfo()
	{
		Debug.Log("顯示公告");
		//真的不卡吗？很怀疑
		while (true)
		{
			
			if (GameInfo.announcementInfo != null)
			{
				for (int i = 0; i < GameInfo.announcementInfo.Count; i++)
				{
					NoticeInfo.GetComponentInChildren<Text>().text = GameInfo.announcementInfo[i].Title + ":" + GameInfo.announcementInfo[i].Content + "\n";
					if (i == GameInfo.announcementInfo.Count - 1)
					{
						i = -1;
					}
					yield return new WaitForSeconds(10);
				}
			}
			yield return new WaitForSeconds(1);

		}
	}

    // Update is called once per frame
    void Update()
    {
		//如果房卡不为0,将房卡数量和服务器的同步
        if (GameInfo.returnGetRoomCard != null)
        {
            GameInfo.userRoomCard = GameInfo.returnGetRoomCard.UserRoomCard;
            RoomCardNum.text = GameInfo.userRoomCard + "";
            GameInfo.returnGetRoomCard = null;
        }
        //从服务器获取公告信息
        if (GameInfo.returnAnnouncement != null)
        {
            GameInfo.announcementInfo = GameInfo.returnAnnouncement.announcement;
            GameInfo.returnAnnouncement = null;
        }
        if (GameInfo.returnServerIP != null)
        {
            GameInfo.ip = GameInfo.returnServerIP.ip;
            GameInfo.port = GameInfo.returnServerIP.port;
			//状态 1为正常跳转， 2为有未结束的游戏，需要重新连接
            GameInfo.status = GameInfo.returnServerIP.Status;

           
            GameInfo.cs.Closed();
            SendAddServer sendAddServer = new SendAddServer();//..CreateBuilder()
            sendAddServer.openid = GameInfo.OpenID;
            sendAddServer.unionid = GameInfo.unionid;
            
            Debug.Log(GameInfo.OpenID);

            byte[] body = ProtobufUtility.GetByteFromProtoBuf( sendAddServer);
            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1011, body.Length, 0, body);
            GameInfo.cs.serverType = ServerType.GameServer;

          
            GameInfo.cs.Send(data);

           
			//重连提示
            if (GameInfo.status == 2)
            {
                GameInfo.room_id = int.Parse(GameInfo.returnServerIP.RoomID);
                Recon_Pel.SetActive(true);
				//应该重连
                GameInfo.recon = true;
                isClosed = true;
                print("断线重连");
                //提示用户有断线重联的情况， 跳转场景
                reconTip.SetActive(true);
				//3秒后跳转至游戏场景
                Invoke("ReEnterRoom", 3);

            }
            GameInfo.returnServerIP = null;
        }
		//获取服务器状态
        if (GameInfo.returnAddServer != null)
        {
            GameInfo.addStatus = GameInfo.returnAddServer.status;
            GameInfo.returnAddServer = null;
        }
		//监听退出键
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            FICWaringPanel._instance.ShowQuit();
            FICWaringPanel._instance.WarnningMethods = delegate { Application.Quit(); };
        }
        if (GameInfo.returnGameOperation != null) ShowOperationStatus();
    }
	//跳转至游戏场景
	public void ReEnterRoom()
	{
		isClosed = true;
		reconTip.SetActive(false);
		SceneManager.LoadScene("Game_GYMJ");
	}
	//根据服务器返回的信息，显示提示信息
    void ShowOperationStatus()
    {
        switch (GameInfo.returnGameOperation.status)
        {
            case 1:
                break;
            case -1:
                FICWaringPanel._instance.Show("房卡不足");
                GameInfo.cs.Closed();
                GameInfo.cs.serverType = ServerType.ListServer;
                break;
            case -2:
                FICWaringPanel._instance.Show("不是圈子用户");
                GameInfo.cs.Closed();
                GameInfo.cs.serverType = ServerType.ListServer;
                break;
            case -3:
                FICWaringPanel._instance.Show("圈主房卡不足");
                GameInfo.cs.Closed();
                GameInfo.cs.serverType = ServerType.ListServer;
                break;
        }
        GameInfo.returnGameOperation = null;
    }

    //退出登录,设置里控制
    public void GameQuit()
    {
        GameInfo.unionid = "";
       // GameInfo.OpenID = SystemInfo.deviceUniqueIdentifier;
        GameInfo.cs.Closed();
        GameInfo.certificate = "";
        SceneManager.LoadScene("_login");
    }
		
    public string RoomInfo(ReturnRoomMsg msg)
    {
        string roomRule = "";

        if (msg.Is_yuanque == 1) roomRule += "原缺 ";
        if (msg.is_benji == 1) roomRule += "本鸡 ";
        if (msg.is_shangxiaji == 1) roomRule += "摇摆鸡 ";
        if (msg.is_xinqiji == 1) roomRule += "星期鸡 ";
        if (msg.is_wgj == 1) roomRule += "乌骨鸡 ";
     

        if (msg.is_lianzhuang == 1) roomRule += "连庄 ";
        else if (msg.is_yikousan == 0) roomRule += "1扣2 ";
        else roomRule += "通三 ";

        if (msg.QuickCard == 1) roomRule += "十秒出牌 ";
        return roomRule;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    public void HallMusicPlay(string str)
    {
        _ArrayAudioSources.clip = (AudioClip)Resources.Load("AudioSource/Sound/" + str, typeof(AudioClip));//调用Resources方法加载AudioClip资源
        _ArrayAudioSources.loop = false;              //开始设置AudioSource不循环不放
        _ArrayAudioSources.Play();
        _ArrayAudioSources.volume = PlayerPrefs.GetFloat("soundVoice");///给音效的拖动按钮赋值
    }

    /// <summary>
    /// 游戏按钮音效
    /// </summary>
    /// <param name="type"></param>
    public void btnVoice(string type)
    {  
        music.Play("btn1");

    }

	/// <summary>
	/// 控制播放游戏所有的音源
	/// </summary>
	/// <param name="str"></param>
	public void Play(string str)
	{
		_ArrayAudioSources.clip = (AudioClip)Resources.Load("AudioSource/Music/" + str, typeof(AudioClip));
		_ArrayAudioSources.loop = false;
		_ArrayAudioSources.Play();
		_ArrayAudioSources.volume = PlayerPrefs.GetFloat("soundVoice");///给音效的拖动按钮赋值
	}


	//???
    public bool SearchObjName(string name, Transform tf)
    {
        foreach (Transform item in tf)
        {
            if (item.name == name)
            {
                return true;
            }
        }
        return false;
    }

   

    private void OnApplicationQuit()
    {
//2018.3.21为了测试而注释------------------------------------------------------		
        GameInfo.cs.Closed();
        GameInfo.cs.myThread.Abort();
    }
}
