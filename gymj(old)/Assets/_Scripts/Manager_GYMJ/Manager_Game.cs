using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MJBLL.common;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using DNL;
using System.Text;
using cn.sharesdk.unity3d;

public class Manager_Game : MonoBehaviour
{
    private FICMJPlaying mjPlaying;
    private FICStartGame ficStartGame;//单例FICStartGame脚本
    private FICjiesan ficjiesan;
    private GameObject Img_setNum;//游戏场景左上角弹出框
    private bool a = true;

    private GameObject SQ_jiesan;//同意解散弹出框
    public Text Room_Id;//房间号
    private Button shareRoomIdButton;
    public Text Room;//string 房间
    public Text time;
    public Text signal;
    public Text juNum;
    public Text userTip;

    private GameObject state_E;//东家同意解散房间状态
    private GameObject state_S;//南家同意解散房间状态
    private GameObject state_W;//西家同意解散房间状态
    private GameObject state_N;//北面同意解散房间状态

    private GameObject GameUIGO;
    private GameObject warningPl;//玩家距离提示框
    private GameObject warn; //提醒图标
    private Text warningPlInfo;
    //用户信息面板
    public GameObject userInfoPel_E;
    public GameObject userInfoPel_W;
    public GameObject userInfoPel_S;
    public GameObject userInfoPel_N;
    //头像按钮
    private Button Head_Mask_E;
    private Button Head_Mask_W;
    private Button Head_Mask_S;
    private Button Head_Mask_N;

    //玩家得分
    private Text integral_E;
    private Text integral_W;
    private Text integral_S;
    private Text integral_N;
    //连接状态
    private GameObject ConnectionStatus_E;
    private GameObject ConnectionStatus_W;
    private GameObject ConnectionStatus_S;
    private GameObject ConnectionStatus_N;

    //光
    private GameObject ray_E;
    private GameObject ray_W;
    private GameObject ray_S;
    private GameObject ray_N;

    private GameObject SafetyDetection;

    private int usrNum = 0;//有几个玩家需要显示距离
    private Manager_Audio managerAudio;
    private Button refresh;

    //ActionParam action = new ActionParam();
    private ArrayList _userInfo;//

    public AudioSource Sound;//声明一个AudioSource,控制游戏所有的AudioClip
    public AudioSource[] _ArrayAudioSources;
    //普通话
    public Toggle Mandarin;
    //方言
    public Toggle Local;
    public Timer delayed;
    public Timer delayed_1;
    bool warnTrigger = false;

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        //========================================还原语言选择============================================//
        Mandarin = transform.Find("/Game_UI/PopUp_UI/SheZhi/Language/Mandarin").GetComponent<Toggle>();
        Local = transform.Find("/Game_UI/PopUp_UI/SheZhi/Language/Local").GetComponent<Toggle>();
        try
        {
            Mandarin.isOn = Convert.ToBoolean(PlayerPrefs.GetString("Pop"));
            Local.isOn = Convert.ToBoolean(PlayerPrefs.GetString("Local"));
        }
        catch
        {
        }
        //GameInfo.fangyan  = Convert.ToBoolean(PlayerPrefs.GetString("pt"));
        //GameInfo.fangyan = Convert.ToBoolean(PlayerPrefs.GetString("fangyan"));

        _ArrayAudioSources = transform.Find("/Audio Source").gameObject.GetComponents<AudioSource>();

        if (GameInfo.recon)
        {
            gameObject.GetComponent<Reconnection>().clearReturn();
            SendRecon();
        }

        managerAudio = GameObject.Find("Main Camera").GetComponent<Manager_Audio>();//单例Manager_Audio脚本
        GameUIGO = transform.Find("/Game_UI").gameObject;
        mjPlaying = gameObject.GetComponent<FICMJPlaying>();
        ficjiesan = gameObject.GetComponent<FICjiesan>();
        //shezhi.SetActive(false);
        //进入游戏全屏


        Img_setNum = transform.Find("/Game_UI/PopUp_UI/SheZhi").gameObject;
        SQ_jiesan = transform.Find("/Game_UI/PopUp_UI/SQ_jiesan").gameObject;

        Room = transform.Find("/Game_UI/Interaction_UI/desktop_UI/Text").GetComponent<Text>();
        Room_Id = transform.Find("/Game_UI/Interaction_UI/desktop_UI/Room_Id").GetComponent<Text>();
        shareRoomIdButton = Room_Id.transform.Find("btn_shareroom").GetComponent<Button>();
        shareRoomIdButton.onClick.AddListener(ShareRoomId);

        time = transform.Find("/Game_UI/Fixed_UI/Time/Text").GetComponent<Text>();
        signal = transform.Find("/Game_UI/Fixed_UI/signal/Text").GetComponent<Text>();
        juNum = transform.Find("/Game_UI/Fixed_UI/juNum/Text").GetComponent<Text>();
        userTip = transform.Find("/Game_UI/Fixed_UI/userTip_text").GetComponent<Text>();
        refresh = transform.Find("/Game_UI/Fixed_UI/But_shuaxin").GetComponent<Button>();
        refresh.onClick.AddListener(delegate { ficStartGame.SetRecon(); });

        warningPl = GameUIGO.transform.Find("PopUp_UI/warningP").gameObject;
        warn = GameUIGO.transform.Find("Fixed_UI/warn").gameObject;
        warningPlInfo = warningPl.transform.Find("info").GetComponent<Text>();
        warn.GetComponent<Button>().onClick.AddListener(delegate { ShowSafetyDetection(); });

        Head_Mask_E = GameUIGO.transform.Find("Fixed_UI/Heads/Head_east/Head_Mask").GetComponent<Button>();
        Head_Mask_W = GameUIGO.transform.Find("Fixed_UI/Heads/Head_west/Head_Mask").GetComponent<Button>();
        Head_Mask_S = GameUIGO.transform.Find("Fixed_UI/Heads/Head_south/Head_Mask").GetComponent<Button>();
        Head_Mask_N = GameUIGO.transform.Find("Fixed_UI/Heads/Head_north/Head_Mask").GetComponent<Button>();
        userInfoPel_E = GameUIGO.transform.Find("PopUp_UI/userInfoPel_E").gameObject;
        userInfoPel_W = GameUIGO.transform.Find("PopUp_UI/userInfoPel_W").gameObject;
        userInfoPel_S = GameUIGO.transform.Find("PopUp_UI/userInfoPel_S").gameObject;
        userInfoPel_N = GameUIGO.transform.Find("PopUp_UI/userInfoPel_N").gameObject;
        ConnectionStatus_E = GameUIGO.transform.Find("Fixed_UI/Heads/Head_east/ConnectionStatus").gameObject;
        ConnectionStatus_W = GameUIGO.transform.Find("Fixed_UI/Heads/Head_west/ConnectionStatus").gameObject;
        ConnectionStatus_S = GameUIGO.transform.Find("Fixed_UI/Heads/Head_south/ConnectionStatus").gameObject;
        ConnectionStatus_N = GameUIGO.transform.Find("Fixed_UI/Heads/Head_north/ConnectionStatus").gameObject;
        ray_E = GameUIGO.transform.Find("Fixed_UI/Heads/Head_east/ray").gameObject;
        ray_W = GameUIGO.transform.Find("Fixed_UI/Heads/Head_west/ray").gameObject;
        ray_S = GameUIGO.transform.Find("Fixed_UI/Heads/Head_south/ray").gameObject;
        ray_N = GameUIGO.transform.Find("Fixed_UI/Heads/Head_north/ray").gameObject;


        integral_E = Head_Mask_E.transform.parent.Find("integral").GetComponent<Text>();
        integral_W = Head_Mask_W.transform.parent.Find("integral").GetComponent<Text>();
        integral_S = Head_Mask_S.transform.parent.Find("integral").GetComponent<Text>();
        integral_N = Head_Mask_N.transform.parent.Find("integral").GetComponent<Text>();

        SafetyDetection = transform.Find("/Game_UI/PopUp_UI/SafetyDetection").gameObject;

        Room_Id.text = GameInfo.room_id.ToString();

        if (GameInfo.gameNum >= 2)
        {
            Room.gameObject.SetActive(false);
            Room_Id.gameObject.SetActive(false);
        }
        //Room_Id.text +="--"+ Screen.height;
        //Room_Id.text += "/" + Screen.width;
        ficStartGame = gameObject.GetComponent<FICStartGame>();


        warningPl.transform.Find("yesBtn").GetComponent<Button>().onClick.AddListener(delegate
        {
            OnYesButtonClick();
        });
        warningPl.transform.Find("exitBtn").GetComponent<Button>().onClick.AddListener(delegate
        {
            ExitGYMJGame();
        });
        //usrFD = new Dictionary<int, string>();




        allbtnVoice();
    }
    private void ShareRoomId()
    {
        StringBuilder textContent = new StringBuilder();

        if (GameInfo.GroupID != 0)
        {
            textContent.Append("朋友圈:【" + GameInfo.GroupID + "】");
        }
        textContent.Append("房间号:【" + Room_Id.text + "】");
        switch (GameInfo.room_peo)
        {
            case 2:
                textContent.Append("二丁拐  ");
                break;
            case 3:
                textContent.Append("三丁拐  ");
                break;
            case 4:
                textContent.Append("四人局  ");
                break;
        }
        textContent.Append(GameInfo.gameCount + "局房  ");
        textContent.Append(GameInfo.roomRlue);

        ShareContent content = new ShareContent();
        content.SetText(textContent.ToString());//显示了
        if (string.IsNullOrEmpty(GameInfo.certificate))
        {
            content.SetTitle("【等你乐麻将】【房卡】");//显示了
        }
        else
        {
            content.SetTitle("【等你乐麻将】*【龙宝】*");//显示了
        }

        content.SetImageUrl("http://qy-imageserver.oss-cn-shenzhen.aliyuncs.com/20171219115836.jpg");//没显示
        content.SetUrl("http://download.gzqyrj.com/download/html/majhong.html");
        content.SetShareType(ContentType.Webpage);
        //content.SetShareType(ContentType.Webpage);
        // FIClogin.myShareSdk.ShowPlatformList(null, content, 100, 100);
        FIClogin.myShareSdk.ShowShareContentEditor(PlatformType.WeChat, content);
    }
    /// <summary>
    /// 控制播放游戏所有的音源
    /// </summary>
    /// <param name="str"></param>
    public void Play(string str)
    {
        for (int i = 0; i < 8; i++)
        {
            if (!_ArrayAudioSources[i].isPlaying)
            {
                _ArrayAudioSources[i].clip = (AudioClip)Resources.Load("AudioSource/Sound/" + str, typeof(AudioClip));
                _ArrayAudioSources[i].loop = false;
                //Isdelay = Time.realtimeSinceStartup;
                _ArrayAudioSources[i].Play();
                _ArrayAudioSources[i].volume = PlayerPrefs.GetFloat("soundVoice");///给音效的拖动按钮赋值
                return;
            }

        }
        //Sound.clip = (AudioClip)Resources.Load("AudioSource/Sound/" + str, typeof(AudioClip));//调用Resources方法加载AudioClip资源
        //Sound.loop = false;              //开始设置AudioSource不循环不放
        //Sound.Play();
    }


    /// <summary>
    /// 还原语言选择
    /// </summary>
    public void swicthLangauage()
    {
        if (Mandarin.isOn == true)
        {
            GameInfo.fangyan = false;
            PlayerPrefs.SetString("Pop", Mandarin.isOn.ToString());
            //PlayerPrefs.SetString("pt", GameInfo.fangyan.ToString());
        }
        else if (Mandarin.isOn == false)
        {
            GameInfo.fangyan = true;
            PlayerPrefs.SetString("Pop", Mandarin.isOn.ToString());
            //PlayerPrefs.SetString("fangyan", GameInfo.fangyan.ToString());
        }

        if (Local.isOn == true)
        {
            GameInfo.fangyan = true;
            PlayerPrefs.SetString("Local", Local.isOn.ToString());
            // PlayerPrefs.SetString("fangyan", GameInfo.fangyan.ToString());
        }
        else if (Local.isOn == false)
        {
            GameInfo.fangyan = false;
            PlayerPrefs.SetString("Local", Local.isOn.ToString());
            //PlayerPrefs.SetString("pt", GameInfo.fangyan.ToString());
        }
    }


    /// <summary>
    /// 游戏场景物理逻辑处理s
    /// </summary>
    void Update()
    {
        swicthLangauage();

        if (DateTime.Now.Minute > 9) { time.text = DateTime.Now.Hour + ":" + DateTime.Now.Minute; }
        else { time.text = DateTime.Now.Hour + ":0" + DateTime.Now.Minute; }
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                signal.text = "NO";
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                signal.text = "3G/4G";
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                signal.text = "WIFI";
                break;
        }
        if (GameInfo.returnDis != null)
        {
            ShowDistance(GameInfo.returnDis.FW == GameInfo.FW ? GameInfo.returnDis.FWT : GameInfo.returnDis.FW, GameInfo.returnDis.dis);
            GameInfo.returnDis = null;
        }
        //收到警告信息，打開提醒窗口
        if (GameInfo.returnIPSame != null || GameInfo.returnIsJ != null || GameInfo.returnCloseGPS != null)
        {

            warningPl.SetActive(true);

            warn.GetComponent<Animator>().SetInteger("warnInt", 1);

            string str = "";
            if (GameInfo.returnIsJ != null) str += "距离过近";
            if (GameInfo.returnIPSame != null) str += "IP相同";
            if (GameInfo.returnCloseGPS != null) str += "未开启GPS";
            warningPlInfo.text = "房间内有玩家" + str + "\n为防止您利益受到损害\n请确认是否继续游戏";
            GameInfo.returnIsJ = null;
            GameInfo.returnIPSame = null;
            GameInfo.returnCloseGPS = null;
        }
        if (GameInfo.returnIsJ5Seconds != null || GameInfo.returnIPSame5Seconds != null || GameInfo.returnCloseGPS5Seconds != null)
        {
            warn.GetComponent<Animator>().SetInteger("warnInt", 1);
            if (GameInfo.returnIsJ5Seconds != null) ShowDisNearUser(GameInfo.returnIsJ5Seconds.isj);
            if (GameInfo.returnIPSame5Seconds != null)
            {
                for (int i = 1; i < 5; i++)
                {
                    if (GameInfo.returnIPSame5Seconds.FW.Contains(i)) ShowIPNearUser(i, true);
                    else ShowIPNearUser(i, false);
                }
            }
            GameInfo.returnIsJ5Seconds = null;
            GameInfo.returnIPSame5Seconds = null;
            if (GameInfo.returnCloseGPS5Seconds != null)
            {
                foreach (var item in GameInfo.returnCloseGPS5Seconds.FW)
                {
                    GameInfo.fwGpsList.Add(item);
                }
            }
            else
            {
                GameInfo.fwGpsList.Clear();
            }
            GameInfo.returnCloseGPS5Seconds = null;
            if (delayed == null) delayed = new Timer(HideWarn, null, 10000, 0);
            delayed.Change(10000, 0);

            if (delayed_1 == null) delayed_1 = new Timer(ChangeStatus, null, 10000, 0);
            delayed_1.Change(10000, 0);
        }
        if (GameInfo.isHideWarn)
        {
            warn.GetComponent<Animator>().SetInteger("warnInt", 2);
            for (int i = 1; i < 5; i++)
            {
                ShowIPNearUser(i, false);
            }
            GameInfo.isHideWarn = false;
        }
        ShowConnectionStatus();

        if (GameInfo.returnManaged != null)
        {
            ShowManaged(GameInfo.returnManaged.state);
            GameInfo.returnManaged = null;
        }

        //检测是否按下返回键，提示退出游戏
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            FICWaringPanel._instance.ShowQuit();
            FICWaringPanel._instance.WarnningMethods = delegate { Application.Quit(); };
        }
        if (warnTrigger) DelayedHide();
    }
    /// <summary>
    /// 隐藏警告提示
    /// </summary>
    /// <param name="state"></param>
    public void HideWarn(object state)
    {
        GameInfo.isHideWarn = true;
    }
    /// <summary>
    /// 显示用户积分
    /// </summary>
    /// <param name="fw">用户方位</param>
    /// <param name="integral">积分数</param>
    public void ShowIntegral(int fw, int integral)
    {
        switch (GameInfo.GetFW(fw))
        {
            case FW.East:
                integral_E.text = "积分:" + integral;
                break;
            case FW.West:
                integral_W.text = "积分:" + integral;
                break;
            case FW.North:
                integral_N.text = "积分:" + integral;
                break;
            case FW.South:
                integral_S.text = "积分:" + integral;
                break;
        }
    }
    public void InvokeAgreeJSroom()
    {
        GameInfo.ClearAllListsAndChanges();
        SceneManager.LoadScene("Scene_Hall");
    }
    /// <summary>
    /// 显示托管
    /// </summary>
    public void ShowManaged(int status)
    {
        ficStartGame.managerPengGang.HideMJSCanGang();
        foreach (Transform item in transform.Find("/Game_UI/Interaction_UI/can_buttons_pghg/grid_pghg"))
        {
            item.gameObject.SetActive(false);
        }
        for (int i = 0; i < ficStartGame.myCards.shouPaiGOList.Count; i++)
        {
            ficStartGame.myCards.shouPaiGOList[i].GetComponent<MeshRenderer>().material.color = ficStartGame.resetColor;
            ficStartGame.myCards.shouPaiGOList[i].GetComponent<MJ_Event>().isCanGang = false;
        }
    }
    /// <summary>
    /// 发送请求托管
    /// </summary>
    public void OnSendManaged()
    {
        //SendManaged sendManaged = SendManaged.CreateBuilder()
        //    .SetState(1)
        //    .SetOpenid(GameInfo.OpenID)
        //    .Build();
        //byte[] body = sendManaged.ToByteArray();

        SendManaged sendManaged = new SendManaged();
        sendManaged.state = 1;
        sendManaged.openid = GameInfo.OpenID;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendManaged);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 2012, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }


    /// <summary>
    /// 听牌按钮调用的方法
    /// 1.如果是天听，直接发送听牌按钮
    /// 2.如果是报听，会根据返回叫牌集合遮住手牌
    /// </summary>
    public void Ting()
    {
        GameInfo.TT_bl = true;
        if (GameInfo.isTT)
            GameObject.Find("Main Camera").GetComponent<FICStartGame>().GameSendBT(GameInfo.OpenID, 2, GameInfo.room_id);
        else
        {
            //报听遮盖
            GameInfo.IsTingPai = true;
            ficStartGame.TingPaiMask();
        }
    }

    ///// <summary>
    ///// 拒绝解散房间
    ///// </summary>
    //public void RefuseJSroom()
    //{

    //}
    public void ShowSafetyDetection()
    {
        for (int i = 1; i <= GameInfo.MJplayers.Count; i++)
        {
            SafetyDetection.transform.Find("User" + i).gameObject.SetActive(true);
            SafetyDetection.transform.Find("User" + i + "/name").GetComponent<Text>().text = GameInfo.MJplayers[i].nickname;
            SafetyDetection.transform.Find("User" + i + "/IP").GetComponent<Text>().text = GameInfo.MJplayers[i].UserIP;
            SafetyDetection.transform.Find("UserHead" + (int)GameInfo.GetFW(i) + "/name").GetComponent<Text>().text = GameInfo.MJplayers[i].nickname;
            SafetyDetection.transform.Find("UserHead" + (int)GameInfo.GetFW(i)).gameObject.SetActive(true);

            switch (GameInfo.GetFW(i))
            {
                case FW.East:
                    SafetyDetection.transform.Find("UserHead" + (int)GameInfo.GetFW(i) + "/head").GetComponent<Image>().sprite = mjPlaying.rightHeadImage.sprite;
                    break;
                case FW.West:
                    SafetyDetection.transform.Find("UserHead" + (int)GameInfo.GetFW(i) + "/head").GetComponent<Image>().sprite = mjPlaying.leftHeadImage.sprite;
                    break;
                case FW.North:
                    SafetyDetection.transform.Find("UserHead" + (int)GameInfo.GetFW(i) + "/head").GetComponent<Image>().sprite = mjPlaying.frontHeadImage.sprite;
                    break;
                case FW.South:
                    SafetyDetection.transform.Find("UserHead" + (int)GameInfo.GetFW(i) + "/head").GetComponent<Image>().sprite = mjPlaying.backHeadImage.sprite;
                    break;
            }
        }
        for (int i = 1; i <= GameInfo.room_peo; i++)
        {
            if (!GameInfo.MJplayers.ContainsKey(i))
            {
                SafetyDetection.transform.Find("User" + (i)).gameObject.SetActive(false);
                SafetyDetection.transform.Find("UserHead" + (int)GameInfo.GetFW(i)).gameObject.SetActive(false);
            }
            SafetyDetection.transform.Find("UserHead" + (int)GameInfo.GetFW(i) + "/error").gameObject.SetActive(false);
        }
        foreach (var item in GameInfo.fwGpsList)
        {
            SafetyDetection.transform.Find("UserHead" + (int)GameInfo.GetFW(item) + "/error").gameObject.SetActive(true);
        }
    }
    public void ShowDisNearUser(List<ReturnIsJ> isj)
    {
        if (GameInfo.room_peo == 0 | GameInfo.FW == 0) return;
        string str = "";
        foreach (var item in isj)
        {
            switch (GameInfo.GetFW(item.FWO))
            {
                case FW.East:
                    if (GameInfo.GetFW(item.FWW) == FW.North) str = "EN";
                    if (GameInfo.GetFW(item.FWW) == FW.South) str = "ES";
                    break;
                case FW.West:
                    if (GameInfo.GetFW(item.FWW) == FW.North) str = "WN";
                    if (GameInfo.GetFW(item.FWW) == FW.East) str = "WE";
                    break;
                case FW.South:
                    if (GameInfo.GetFW(item.FWW) == FW.North) str = "SN";
                    if (GameInfo.GetFW(item.FWW) == FW.West) str = "SW";
                    break;
            }
            if (str == "") continue;
            SafetyDetection.transform.Find(str).gameObject.SetActive(true);
        }
    }


    void ChangeStatus(object status)
    {
        warnTrigger = true;
    }
    void DelayedHide()
    {
        SafetyDetection.transform.Find("EN").gameObject.SetActive(false);
        SafetyDetection.transform.Find("ES").gameObject.SetActive(false);
        SafetyDetection.transform.Find("WN").gameObject.SetActive(false);
        SafetyDetection.transform.Find("WE").gameObject.SetActive(false);
        SafetyDetection.transform.Find("SN").gameObject.SetActive(false);
        SafetyDetection.transform.Find("SW").gameObject.SetActive(false);
        warnTrigger = false;
    }
    public void ShowIPNearUser(int fw, bool isNear)
    {
        if (isNear)
        {
            SafetyDetection.transform.Find("User" + fw).GetComponent<Image>().sprite = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_UI/red");
        }
        else
        {
            SafetyDetection.transform.Find("User" + fw).GetComponent<Image>().sprite = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_UI/green");
        }
    }
    #region 按钮响应事件
    /// <summary>
    /// 游戏界面右上角设置按钮弹出
    /// </summary>
    public void But_setNum()
    {
        if (a)
        {
            Img_setNum.SetActive(true);

            //transform.Find("/Cans_UI/Images/Time&Wifi").gameObject.SetActive(false);
            a = false;
        }

        else
        {
            a = true;
            Img_setNum.SetActive(false);
            // transform.Find("/Cans_UI/Images/Time&Wifi").gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// 退出游戏
    /// </summary>
    public void Quit_game()
    {
        Application.Quit();
    }

    public void ShowUserInfo(long id, string ip, int fw)
    {
        switch (GameInfo.GetFW(fw))
        {
            case FW.East:
                mjPlaying.rightIDText.text = id + "";
                mjPlaying.rightIPText.text = ip;
                break;
            case FW.West:
                mjPlaying.leftIDText.text = id + "";
                mjPlaying.leftIPText.text = ip;
                break;
            case FW.North:
                mjPlaying.frontIDText.text = id + "";
                mjPlaying.frontIPText.text = ip;
                break;
            case FW.South:
                mjPlaying.backIDText.text = id + "";
                mjPlaying.backIPText.text = ip;
                break;
        }
    }
    #endregion 
    /// <summary>
    /// 顯示玩家距離
    /// </summary>
    public void ShowDistance(int fw, string str)
    {
        float dis;
        float.TryParse(str, out dis);
        string unit = "m";
        if (dis > 1000)
        {
            dis = dis / 1000;

            unit = "Km";
        }
        switch (GameInfo.GetFW(fw))
        {
            case FW.East:
                mjPlaying.rightDistanceText.text = string.Format("{0:0.0}", dis) + unit;
                break;
            case FW.West:
                mjPlaying.leftDistanceText.text = string.Format("{0:0.0}", dis) + unit;
                break;
            case FW.North:
                mjPlaying.frontDistanceText.text = string.Format("{0:0.0}", dis) + unit;
                break;
        }
    }


    /// <summary>
    /// warningPanel显示以后，点击是，则发送请求开始游戏。点击否，就发送推出消息
    /// </summary>
    void OnNoButtonClick()
    {///用户请求退出房间 +5008
        //    message SendRemove{
        //        required string openid = 1;//用户openid
        //        required int32 RoomID = 2;//房间号
        //    }
        //    ///服务端返回退出玩家 +5009(未接受到则为客服端判断错误)
        //    message ReturnRemoveUser{
        //        required string openid = 1;//退出的用户ID

        //    }

        //var mp = SendRemove.CreateBuilder().SetOpenid(GameInfo.OpenID).SetRoomID(GameInfo.room_id).Build();
        //byte[] body = mp.ToByteArray();

        SendRemove mp = new SendRemove();
        mp.openid = GameInfo.OpenID;
        mp.RoomID = GameInfo.room_id;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(mp);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5008, body.Length, 0, body);

        GameInfo.cs.Send(data);
    }
    /// <summary>
    /// 判断是否接受到服务器发来的推出房间消息
    /// </summary>
    void ExitGYMJGame()
    {

        if (GameInfo.FW != 1)
        {
            ficjiesan.SendQuitMessage();
        }
        else
        {
            ficjiesan.SendJieSanMessage();
        }
        GameInfo.gameCount = 1;
        GameInfo.FW = 0;
        GameInfo.IsSetRoomInfo = false;
    }

    /// <summary>
    /// 当别人距离太近，点击 是，则正常开始游戏,点击是，发送7093，确认开始
    /// </summary>
    void OnYesButtonClick()
    {

        //mjPlaying.isCanStart = true;
        warningPl.SetActive(false);

        //客户端发送请求，确认开始					+7093
        //        message SendConfirmationStarts
        //{
        //            required string openid = 1;//用户OPENID
        //            required int32 RoomID = 2;//房间号
        //        }
        //var mp = SendConfirmationStarts.CreateBuilder().SetOpenid(GameInfo.OpenID).SetRoomID(GameInfo.room_id).Build();
        //byte[] body = mp.ToByteArray();
        SendConfirmationStarts mp = new SendConfirmationStarts();
        mp.openid = GameInfo.OpenID;
        mp.RoomID = GameInfo.room_id;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(mp);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 7094, body.Length, 0, body);

        GameInfo.cs.Send(data);

    }

    /// <summary>
    /// 游戏按钮音效
    /// </summary>
    /// <param name="type"></param>
    public void musiClick(string type)
    {
        FICAudioPlay._instance.ButtonPlay();
    }
    /// <summary>
    /// 发送断线重连
    /// </summary>
    void SendRecon()
    {
        //SendConnData sendConnData = SendConnData.CreateBuilder()
        //    .SetOpenid(GameInfo.OpenID)
        //    .SetRoomID(GameInfo.room_id)
        //    .Build();
        //byte[] body = sendConnData.ToByteArray();

        SendConnData sendConnData = new SendConnData();
        sendConnData.openid = GameInfo.OpenID;
        sendConnData.RoomID = GameInfo.room_id;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendConnData);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5019, body.Length, 0, body);
        GameInfo.cs.serverType = ServerType.GameServer;
        Debug.Log(GameInfo.ip + "|" + GameInfo.port);
        // GameInfo.cs.Closed();
        GameInfo.cs.Send(data);
        GameInfo.status = 0;
    }
    /// <summary>
    /// 显示连接状态 ConnectionStatus   1在线，2离线
    /// </summary>
    void ShowConnectionStatus()
    {
        if (GameInfo.returnConnectionStatus != null)
        {
            foreach (var item in GameInfo.returnConnectionStatus.ConnectionStatus)
            {
                switch (GameInfo.GetFW(item.FW))
                {
                    case FW.East:
                        if (item.ConnectionStatus == 1) { ConnectionStatus_E.SetActive(false); }
                        else { ConnectionStatus_E.SetActive(true); }
                        break;
                    case FW.West:
                        if (item.ConnectionStatus == 1) { ConnectionStatus_W.SetActive(false); }
                        else { ConnectionStatus_W.SetActive(true); }
                        break;
                    case FW.North:
                        if (item.ConnectionStatus == 1) { ConnectionStatus_N.SetActive(false); }
                        else { ConnectionStatus_N.SetActive(true); }
                        break;
                    case FW.South:
                        if (item.ConnectionStatus == 1) { ConnectionStatus_S.SetActive(false); }
                        else { ConnectionStatus_S.SetActive(true); }
                        break;
                }
            }

            GameInfo.returnConnectionStatus = null;
        }
    }

    /// <summary>
    /// 游戏场景所有按钮音效对象
    /// </summary>
    public void allbtnVoice()
    {
        //==============================================游戏场景其他按钮音效===============================================//
        transform.Find("/Game_UI/Fixed_UI/But_SheZhi").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btn1"); }); ///   右上角设置按钮音效
        transform.Find("/Game_UI/Fixed_UI/Voice_Dd").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btn1"); }); ///     快捷语音音效
        transform.Find("/Game_UI/Fixed_UI/But_shuaxin").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btn1"); }); ///刷新按钮

        transform.Find("/Game_UI/PopUp_UI/panel_settlement/littlesettlement/btn_details").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btn2"); }); ///结算界面查看详情
        transform.Find("/Game_UI/PopUp_UI/panel_settlement/littlesettlement/btn_continue").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btn2"); });//小结算界面接续游戏
        transform.Find("/Game_UI/PopUp_UI/panel_ji/panel_jis/btn_showjiesuanpanel").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btn2"); }); ///翻鸡界面返回结算按钮

        transform.Find("/Game_UI/PopUp_UI/panel_settlement/finalsettlement/btn_share").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btn2"); });//大结算分享按钮
        transform.Find("/Game_UI/PopUp_UI/panel_settlement/finalsettlement/btn_continue").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btn2"); }); ///大结算返回大厅按钮

        //==============================================游戏场景头像点击音效=========================================//
        Head_Mask_E.onClick.AddListener(delegate
        {
            userInfoPel_E.SetActive(true);
            musiClick("btn2");
        });
        Head_Mask_W.onClick.AddListener(delegate
        {
            musiClick("btn2");
            userInfoPel_W.SetActive(true);
        });
        Head_Mask_S.onClick.AddListener(delegate
        {
            userInfoPel_S.SetActive(true);
            musiClick("btn2");
        });
        Head_Mask_N.onClick.AddListener(delegate
        {
            userInfoPel_N.SetActive(true);
            musiClick("btn2");
        });

        //========================================游戏场景关闭按钮================================================//
        transform.Find("/Game_UI/PopUp_UI/SheZhi/But_Close").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btnClose"); }); /// 设置弹出框关闭按钮音效
        transform.Find("/Game_UI/PopUp_UI/SheZhi/But_Close").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btnClose"); }); /// 快捷语音弹出框关闭按钮音效
        transform.Find("/Game_UI/PopUp_UI/Voice/close_Btn").GetComponent<Button>().onClick.AddListener(delegate { musiClick("btnClose"); }); /// 退出登录音效

    }
    /// <summary>
    /// 显示头像背后闪光
    /// </summary>
    /// <param name="fw">显示方位</param>
    public void ShowRay(int fw)
    {
        ray_E.SetActive(false);
        ray_W.SetActive(false);
        ray_S.SetActive(false);
        ray_N.SetActive(false);
        switch (GameInfo.GetFW(fw))
        {
            case FW.East:
                ray_E.SetActive(true);
                break;
            case FW.West:
                ray_W.SetActive(true);
                break;
            case FW.North:
                ray_N.SetActive(true);
                break;
            case FW.South:
                ray_S.SetActive(true);
                break;
        }
    }
    /// <summary>
    /// 发送礼物表情消息
    /// </summary>
    /// <param name="str">礼物字符串标注("int(语音号),int(发送方位)")</param>
    public void SendProp(string str)
    {
        string[] strs = str.Split(',');
        //GameObject  go =  Instantiate(Resources.Load<GameObject>("Game_GYMJ/Prefabs/prop/" + strs[0]), GameUIGO.transform.Find("Fixed_UI/Heads")) as GameObject;
        // go.GetComponent<RectTransform>().localPosition = GetHeadPos((int)(GameInfo.GetFW(GameInfo.FW)));
        //go.GetComponent<RectTransform>().localScale = Vector3.one;
        //go.GetComponent<Image>().SetNativeSize();
        // go.GetComponent<RectTransform>().DOLocalMove(GetHeadPos(int.Parse(strs[1])), 2);
        //StartCoroutine(PropMove(go));

        //SendVoice sendGameOperation = SendVoice.CreateBuilder()
        //   .SetOpenid(GameInfo.OpenID)
        //   .SetRoomID(GameInfo.room_id)
        //   .SetVoiceNumber(int.Parse(strs[0]))
        //   .SetFWT(GetTrueFw(int.Parse(strs[1])))
        //   .Build();
        //byte[] body = sendGameOperation.ToByteArray();

        SendVoice sendGameOperation = new SendVoice();
        sendGameOperation.openid = GameInfo.OpenID;
        sendGameOperation.RoomID = GameInfo.room_id;
        sendGameOperation.VoiceNumber = int.Parse(strs[0]);
        sendGameOperation.FWT = GetTrueFw(int.Parse(strs[1]));
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGameOperation);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 8002, body.Length, 0, body);
        Debug.Log(data);
        GameInfo.cs.Send(data);
    }
    /// <summary>
    /// 获取目标方位的真实方位
    /// </summary>
    /// <param name="fw">目标方位</param>
    /// <returns></returns>
    int GetTrueFw(int fw)
    {
        for (int i = 1; i < 5; i++)
        {
            if ((int)GameInfo.GetFW(i) == fw)
            {
                return i;
            }
        }
        return 0;
    }
    /// <summary>
    /// 返回消息实现礼物效果
    /// </summary>
    /// <param name="str">礼物字符串标注("int(语音号),int(发送方位)")</param>
    public void ReturnProp(string str)
    {
        string[] strs = str.Split(',');
        GameObject go = Instantiate(Resources.Load<GameObject>("Game_GYMJ/Prefabs/prop/" + strs[1]), GameUIGO.transform.Find("Fixed_UI/Heads")) as GameObject;
        go.GetComponent<RectTransform>().localScale = Vector3.one;
        if (strs[1].Equals("3") && GetHeadPos((int)GameInfo.GetFW(int.Parse(strs[0]))).x < GetHeadPos((int)GameInfo.GetFW(int.Parse(strs[2]))).x) go.transform.localScale = new Vector3(-1, 1, 1);
        go.GetComponent<RectTransform>().localPosition = GetHeadPos((int)GameInfo.GetFW(int.Parse(strs[0])));
        if (go.GetComponent<AudioSource>() != null)
        {
            for (int i = 0; i < go.GetComponents<AudioSource>().Length; i++)
            {
                go.GetComponents<AudioSource>()[i].volume = managerAudio._ConSound.value;
            }
        }
        go.GetComponent<Image>().SetNativeSize();
        Tweener tw = go.GetComponent<RectTransform>().DOLocalMove(GetHeadPos((int)GameInfo.GetFW(int.Parse(strs[2]))), 0.5f);
        tw.OnComplete(() =>
        {
            go.GetComponent<Animator>().SetTrigger("Trigger");
            if (go.GetComponent<AudioSource>() != null) go.GetComponent<AudioSource>().Play();
            StartCoroutine(PropMove(go));
        });
    }
    /// <summary>
    /// 礼物移动效果
    /// </summary>
    /// <param name="go">礼物对象</param>
    /// <returns></returns>
    IEnumerator PropMove(GameObject go)
    {
        yield return new WaitForSeconds(3);
        Destroy(go);
    }
    /// <summary>
    /// 获取头像位置信息
    /// </summary>
    /// <param name="fw">方位信息</param>
    /// <returns></returns>
    public Vector2 GetHeadPos(int fw)
    {
        switch (fw)
        {
            case 1:
                return GameUIGO.transform.Find("Fixed_UI/Heads/Head_east").GetComponent<RectTransform>().localPosition;
            case 2:
                return GameUIGO.transform.Find("Fixed_UI/Heads/Head_west").GetComponent<RectTransform>().localPosition;
            case 3:
                return GameUIGO.transform.Find("Fixed_UI/Heads/Head_north").GetComponent<RectTransform>().localPosition;
            case 4:
                return GameUIGO.transform.Find("Fixed_UI/Heads/Head_south").GetComponent<RectTransform>().localPosition;
        }
        return transform.position;
    }
}
