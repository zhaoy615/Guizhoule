using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MJBLL.common;
using System.Threading;
using UnityEngine.UI;
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Assets.Script_me;
using cn.sharesdk.unity3d;

using DNL;
using System.Text;

public class FICStartGame : MonoBehaviour
{
    public Color darkColor = new Color(0.25f, 0.25f, 0.25f);
    public Color resetColor = new Color(0.6f, 0.6f, 0.6f, 1);
    public Sprite[] HuaseArray = new Sprite[30];// 2d鸡牌统计以及结算界面的时候显示
    private Sprite[] HuTypeArray = new Sprite[15];//胡牌牌型的图片，在结算面板显示,
    public Sprite[] QueTypeArray = new Sprite[4];//0,不放东西1,筒2,条3,万
    public GameObject[] deskSkillGOArray = new GameObject[10];//桌面特效数组,不知道大小
    public GameObject[] huSkillGOArray = new GameObject[6];//不同胡特效数组
    private Sprite[] huTypeSpritArray = new Sprite[6];//胡牌类型的数组

    private GameObject fanji2dGO;//2d翻鸡牌，结算界面之前显示的
    //=================脚本单例==================//
    private Show2dJiPai jipai2d;//获取显示鸡牌这个脚本
    public FICpaipaipai managerPai;
    public FICMaskPai maskPai;
    public Manager_PengGang managerPengGang;
    public Manager_Game managerGame;
    private Mannager_Time manager_Time;
    private Manager_Game music;
    private Manager_Audio ManagerAudio;
    public FICMyCards myCards;
    private FICMJPlaying mjPlaying;
    public FICPaiDui paidui;
    private Manager_Audio managerAudio;
    private Manager_Audio _ManagerAudio;
    public Reconnection recon;
    //private  Image Shoupai_S;
    //private float start_x = 0;
    //private float MJ_w = 0;
    //private float MJ_w_o = 55;
    private GameObject MJ_S_Floder;//s方位手牌父物体
    private GameObject MJ_E_Floder;//e方位手牌父物体
    private GameObject MJ_W_Floder;//w方位手牌父物体
    private GameObject MJ_N_Floder;//n方位手牌父物体
    private Text paiCountText;//还剩下多少张牌
    private int paiCount = 1;//只用来最后荒牌检测是否为零，所有初值给了1
    public GameObject jiesuanPanel;//结算面板
    //private GameObject dajiesuanPanel;//大结算面板
    //private GameObject Img_Hu;
    private Text roomRuleText;//房间规则text
    private float playerTime = 10f;//玩家倒计时10s
    private bool isFanJi = false;//是否翻鸡了
    public bool isBaoTing = false;//是否报听了
    public GameObject fanJiPaiGO;//3d的翻鸡牌
    private GameObject conPlayerGO;//结算界面每个人的信息预制体
    private GameObject conPlayerFinalGO;//大结算界面每个人的信息预制体
    private GameObject jipaiItemGO;//结算界面每个人鸡牌（牌面，分数，类型）的预制体
    private GameObject douItemGO;//结算界面每个人豆（类型，分值）的预制体
    private GameObject player1JieSuanGO;//第一个人结算信息位置确定
    private GameObject player2JieSuanGO;//第二个人结算信息位置确定
    private Transform player1JieSuanTrans;//第一个人结算信息位置确定
    private Transform player2JieSuanTrans;//第二个人结算信息位置确定
    private Vector3 playerOffset;//两个人结算信息之间的偏移量
    private Vector3 conPlayerPos;//生成每个人结算信息的位置确定（初始位置+偏移量）
    private Vector3 conFinalPlayerPos;//生成大结算每个人结算信息的位置确定（初始位置+偏移量）
    private GameObject jiesuanPanelContinueButtonGO;//结算界面，继续游戏按钮
    private GameObject jiesuanPanelShowFinalButtonGO;//结算界面，显示最终大结算按钮
    public GameObject QueYiMen; //三个缺一门按钮的父物体
    public GameObject dingquezhongGO;//定缺中三个字
    private GameObject dingquehouGO;
    public Image eastImage;//每个方位生成一张定缺后的文字，缩小移动到目标位置
    public Image westImage;//每个方位生成一张定缺后的文字，缩小移动到目标位置
    public Image northImage;//每个方位生成一张定缺后的文字，缩小移动到目标位置
    private Vector3 eastImageVec;
    private Vector3 westImageVec;
    private Vector3 northImageVec;
    //private Image southImage;
    public Vector3 eastQuePos;//每个方位生成一张定缺后的文字，缩小移动到的（目标位置）
    public Vector3 westQuePos;//每个方位生成一张定缺后的文字，缩小移动到的（目标位置）
    public Vector3 northQuePos;//每个方位生成一张定缺后的文字，缩小移动到的（目标位置）
    //private Vector3 southQuePos;
    //冲锋、乌骨鸡动画效果
    public GameObject AniJi_S;//桌面特效表现位置
    public GameObject AniJi_E;//桌面特效表现位置
    public GameObject AniJi_W;//桌面特效表现位置
    public GameObject AniJi_N;//桌面特效表现位置
    private Text roomid;//房间id
    public Image fanjipai;//翻鸡动画图片
    private Text fanjiText;//结算界面翻鸡两个字
    public int fanjipaihs;//结算界面翻鸡牌的花色
    private GameObject root;//为了避免找不到物体，首先找了个根物体
    private GameObject game_Prefabs;//为了避免找不到物体，首先找到了灯物体,现替换成game_prefabs
    public Transform eJiGridTrans;//头顶的各种标志的父物体
    public Transform wJiGridTrans;//头顶的各种标志的父物体
    public Transform sJiGridTrans;//头顶的各种标志的父物体
    public Transform nJiGridTrans;//头顶的各种标志的父物体
    private GameObject chongfengjiGO;//头顶的冲锋鸡标志
    private GameObject wugujiGO;//头顶的乌骨鸡标志
    private GameObject zerenjiGO;//头顶的责任鸡标志
    private GameObject zerenwuguGO;//头顶的责任乌骨标志
    private GameObject zhuangGO;//头顶的庄标志
    public GameObject tingGO;//头顶的听标志
    private Transform jiTx;//桌面鸡特效
    private Transform pghgGridTrans;//碰杠胡过按钮的父物体
    public GameObject pengButtonGO;//碰按钮
    public GameObject gangButtonGO;//杠按钮
    public GameObject guoButtonGO;//过按钮
    public GameObject huButtonGO;//胡按钮
    public GameObject tingButtonGO;//听按钮
    public Texture[] touziqiTexture;//骰子器貼圖
    public Transform deskSkillTran;
    private static string objname;
    private int reconState;
    private bool isChu;
    public bool isRecon;
    public float reconTime;
    private float delayQuick;
    public GamePlayback gamePlayback;
    public bool isAutoShow = true;
    public bool isFaPaiEnd = false;
    public bool isGameServer = true;
    //==================翻鸡之前显示是否叫牌，或者胡牌类型===========
    public Transform _TransIsJiaoAndPao;
    public Transform _NorthIsJiaoAndPao;
    public Transform _EastIsJiaoAndPao;
    public Transform _SouthIsJiaoAndPao;
    public Transform _WestIsJiaoAndPao;
    public Vector3 _NorthIsJiaoAndPaoVt;
    public Vector3 _EastIsJiaoAndPaoVt;
    public Vector3 _SouthIsJiaoAndPaoVt;
    public Vector3 _WestIsJiaoAndPaoVt;
    private Sprite[] _ArrayIsPaoAndIsJiao = new Sprite[15];


    private void Awake()
    {
        manager_Time = gameObject.GetComponent<Mannager_Time>();
    }
    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        //=====================脚本实例========================//
        jipai2d = transform.Find("/Game_UI/PopUp_UI/panel_ji").GetComponent<Show2dJiPai>();
        managerPengGang = transform.Find("/Main Camera").GetComponent<Manager_PengGang>();
        managerGame = transform.Find("/Main Camera").GetComponent<Manager_Game>();
        managerPai = GameObject.Find("Main Camera").GetComponent<FICpaipaipai>();
        paidui = gameObject.GetComponent<FICPaiDui>();
        maskPai = gameObject.GetComponent<FICMaskPai>();
        myCards = gameObject.GetComponent<FICMyCards>();
        mjPlaying = gameObject.GetComponent<FICMJPlaying>();
        recon = gameObject.GetComponent<Reconnection>();
        gamePlayback = gameObject.GetComponent<GamePlayback>();

        delayQuick = 0;
        reconState = 0;
        isChu = false;
        isRecon = false;
        reconTime = 0;
        managerAudio = GameObject.Find("Main Camera").GetComponent<Manager_Audio>();
        roomRuleText = transform.Find("/Game_UI/Interaction_UI/desktop_UI/roomRuleText").GetComponent<Text>();
        fanji2dGO = transform.Find("/Game_UI/PopUp_UI/panel_ji/panel_jis/image_fanji2d").gameObject;//结算之前，各家鸡牌中间的翻鸡
        deskSkillTran = transform.Find("/Game_UI/Interaction_UI/Ji_TX");//所有桌面特效表现的父物体
        AniJi_S = deskSkillTran.Find("AniJi_S").gameObject;
        AniJi_E = deskSkillTran.Find("AniJi_E").gameObject;
        AniJi_W = deskSkillTran.Find("AniJi_W").gameObject;
        AniJi_N = deskSkillTran.Find("AniJi_N").gameObject;
        //=====================头像各种标志的父物体===================
        eJiGridTrans = transform.Find("/Game_UI/Fixed_UI/Heads/Head_east/grid_ji");
        wJiGridTrans = transform.Find("/Game_UI/Fixed_UI/Heads/Head_west/grid_ji");
        sJiGridTrans = transform.Find("/Game_UI/Fixed_UI/Heads/Head_south/grid_ji");
        nJiGridTrans = transform.Find("/Game_UI/Fixed_UI/Heads/Head_north/grid_ji");
        //======================头像各种标志=============================
        chongfengjiGO = Resources.Load("Game_GYMJ/ItemsOnHead/item_ji_chong") as GameObject;
        wugujiGO = Resources.Load("Game_GYMJ/ItemsOnHead/item_ji_wu") as GameObject;
        zerenjiGO = Resources.Load("Game_GYMJ/ItemsOnHead/item_ji_ze") as GameObject;
        zerenwuguGO = Resources.Load("Game_GYMJ/ItemsOnHead/item_ji_zewu") as GameObject;
        zhuangGO = Resources.Load("Game_GYMJ/ItemsOnHead/item_ji_zhuang") as GameObject;
        tingGO = Resources.Load("Game_GYMJ/ItemsOnHead/item_ji_ting") as GameObject;
        //==========================碰杠胡过按钮==================================
        pghgGridTrans = transform.Find("/Game_UI/Interaction_UI/can_buttons_pghg/grid_pghg");
        pengButtonGO = pghgGridTrans.transform.Find("item_button_peng").gameObject;
        gangButtonGO = pghgGridTrans.transform.Find("item_button_gang").gameObject;
        huButtonGO = pghgGridTrans.transform.Find("item_button_hu").gameObject;
        //guoButtonGO = Resources.Load("item_button_guo") as GameObject;
        guoButtonGO = pghgGridTrans.transform.Find("item_button_guo").gameObject;
        tingButtonGO = pghgGridTrans.transform.Find("item_button_ting").gameObject;

        touziqiTexture = new Texture[5];
        touziqiTexture[0] = Resources.Load("Game_GYMJ/Texture/Game_Table/saiziqi_A") as Texture;//骰子器默认效果
        touziqiTexture[1] = Resources.Load("Game_GYMJ/Texture/Game_Table/saiziqi_B") as Texture;//骰子器闪烁效果
        touziqiTexture[2] = Resources.Load("Game_GYMJ/Texture/Game_Table/saiziqi_z_A") as Texture;//骰子器庄家默认效果
        touziqiTexture[3] = Resources.Load("Game_GYMJ/Texture/Game_Table/saiziqi_z_B") as Texture;//骰子器庄家闪烁效果
        touziqiTexture[4] = Resources.Load("Game_GYMJ/Texture/Game_Table/saiziqi_c") as Texture;//骰子器警告效果
        paiCountText = transform.Find("/Game_UI/Fixed_UI/paiNum/Text").GetComponent<Text>();
        Invoke("Timer", 5.0f);
        GameObject.Find("Main Camera").GetComponent<Manager_Game>().userTip.text = null;
        root = GameObject.Find("/Game_UI");
        game_Prefabs = GameObject.Find("/Game_Prefabs");
        fanJiPaiGO = game_Prefabs.transform.Find("fanjipai").gameObject;
        jiesuanPanel = root.transform.Find("PopUp_UI/panel_settlement").gameObject;
        //dajiesuanPanel = root.transform.Find("PopUp_UI/panel_finalsettlement").gameObject;//场景添加，按照小结算的模式，复制修改
        conPlayerGO = Resources.Load("Game_GYMJ/Prefabs/con_player") as GameObject;
        conPlayerFinalGO = Resources.Load("Game_GYMJ/Prefabs/con_playercount") as GameObject;
        jipaiItemGO = Resources.Load("Game_GYMJ/Prefabs/item_jipai2") as GameObject;
        douItemGO = Resources.Load("Game_GYMJ/Prefabs/item_dou") as GameObject;
        dingquezhongGO = root.transform.Find("Interaction_UI/Texts_que").gameObject;
        dingquehouGO = root.transform.Find("Fixed_UI/Image_que").gameObject;//定缺后显示大图，缩小成小图，的父物体，可以用deskskill位置代替
        QueYiMen = transform.Find("/Game_UI/Interaction_UI/can_que").gameObject;
        //获取播放器对象
        music = (GetComponent("Manager_Game") as Manager_Game);
        //=======================从小结算方法复制过来的初始定义，===========
        player1JieSuanGO = jiesuanPanel.transform.Find("con_player1").gameObject;
        player2JieSuanGO = jiesuanPanel.transform.Find("con_player2").gameObject;
        jiesuanPanelContinueButtonGO = jiesuanPanel.transform.Find("littlesettlement/btn_continue").gameObject;
        jiesuanPanelShowFinalButtonGO = jiesuanPanel.transform.Find("littlesettlement/btn_showfinal").gameObject;
        player1JieSuanTrans = player1JieSuanGO.transform;
        player2JieSuanTrans = player2JieSuanGO.transform;
        playerOffset = player2JieSuanTrans.position - player1JieSuanTrans.position;
        conPlayerPos = player1JieSuanTrans.position;
        conFinalPlayerPos = player1JieSuanTrans.position;
        roomid = jiesuanPanel.transform.Find("littlesettlement/text_room").GetComponent<Text>();
        fanjipai = jiesuanPanel.transform.Find("littlesettlement/text_fanji/image_fanji").GetComponent<Image>();
        fanjiText = jiesuanPanel.transform.Find("littlesettlement/text_fanji").GetComponent<Text>();
        jiTx = transform.Find("/Game_UI/Interaction_UI/desktop_UI/JiTX");
        //======================
        eastImage = transform.Find("/Game_UI/Fixed_UI/Image_que/eastImage").GetComponent<Image>();
        westImage = transform.Find("/Game_UI/Fixed_UI/Image_que/westImage").GetComponent<Image>();
        northImage = transform.Find("/Game_UI/Fixed_UI/Image_que/northImage").GetComponent<Image>();
        eastQuePos = transform.Find("/Game_UI/Fixed_UI/Image_que/eastImage1").transform.position;
        westQuePos = transform.Find("/Game_UI/Fixed_UI/Image_que/westImage1").transform.position;
        northQuePos = transform.Find("/Game_UI/Fixed_UI/Image_que/northImage1").transform.position;
        eastImageVec = eastImage.transform.position;
        westImageVec = westImage.transform.position;
        northImageVec = northImage.transform.position;
        //==================翻鸡之前显示是否叫牌，或者胡牌类型===========
        _TransIsJiaoAndPao = transform.Find("/Game_UI/PopUp_UI/panelIsJiaoAndIsPao");
        _NorthIsJiaoAndPao = _TransIsJiaoAndPao.Find("north");
        _NorthIsJiaoAndPaoVt = _NorthIsJiaoAndPao.position;
        _EastIsJiaoAndPao = _TransIsJiaoAndPao.Find("east");
        _EastIsJiaoAndPaoVt = _EastIsJiaoAndPao.position;
        _SouthIsJiaoAndPao = _TransIsJiaoAndPao.Find("south");
        _SouthIsJiaoAndPaoVt = _SouthIsJiaoAndPao.position;
        _WestIsJiaoAndPao = _TransIsJiaoAndPao.Find("west");
        _WestIsJiaoAndPaoVt = _WestIsJiaoAndPao.position;
        InitArrays();
        FICWaringPanel._instance.Hide();
    }
    /// <summary>
    /// 初始化之前定义的各种数组
    /// </summary>
    private void InitArrays()
    {

        //private GameObject[] huSkillGOArray = new GameObject[6];//不同胡特效数组

        for (int i = 1; i < 30; i++)
        {
            if (i % 10 != 0)
            {
                HuaseArray[i] = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_MJ2D/" + i);
            }
        }
        for (int i = 1; i < 15; i++)
        {
            HuTypeArray[i] = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_paixing/" + i);
        }
        for (int i = 1; i < 6; i++)
        {
            huTypeSpritArray[i] = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_huType/" + i);
        }
        for (int i = 1; i < 15; i++)
        {
            _ArrayIsPaoAndIsJiao[i] = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_IsJiao/" + i);
        }

        QueTypeArray[1] = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_UI/game_icon_tong");
        QueTypeArray[2] = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_UI/game_icon_tiao");
        QueTypeArray[3] = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_UI/game_icon_W");

        deskSkillGOArray[0] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_pengnew");
        deskSkillGOArray[1] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_gangnew");
        deskSkillGOArray[2] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_tingnew");
        deskSkillGOArray[3] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_hunew");
        deskSkillGOArray[4] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_zimonew");
        deskSkillGOArray[5] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_cfjnew");
        deskSkillGOArray[6] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_cfwgnew");
        deskSkillGOArray[7] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_zrwgnew");
        deskSkillGOArray[8] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_zrj");
        deskSkillGOArray[9] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/tianting");
        //1:自摸；2:胡牌；3:抢杠；4:热炮;5:杠上开花)
        huSkillGOArray[0] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_hunew");
        huSkillGOArray[1] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_zimonew");
        huSkillGOArray[2] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_hunew");
        huSkillGOArray[3] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_qgnew");
        huSkillGOArray[4] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_rpnew");
        huSkillGOArray[5] = Resources.Load<GameObject>("Game_GYMJ/Prefabs/texiao/item_gskhnew");


    }


    public int xintiao = 0;
    /// <summary>
    /// 心跳
    /// </summary>
    void Timer()
    {
        if (GameInfo.UserHearbeat >= 2)
        {
            if (!GameInfo.isAllreadyStart)
            {
                FICWaringPanel._instance.Show("连接失败，请重新登录游戏！");
                FICWaringPanel._instance.WarnningMethods = delegate { if (!GameInfo.isAllreadyStart) { GameInfo.cs.Closed(); SceneManager.LoadScene("_login"); } };
            }
        }
        else
        {
            GPSManager.instance.StartCrt();
            GameInfo.Xintiao();
        }
        Invoke("Timer", 2.0f);
    }
    public void SetRecon()
    {
        if (!isRecon && (Time.realtimeSinceStartup - reconTime) > 5 && GameInfo.isAllreadyStart)
        {
            reconTime = Time.realtimeSinceStartup;
            recon.ReconServer();
        }
    }
    public void SendRecon()
    {
        if (GameInfo.returnAddServer != null && GameInfo.returnAddServer.status == 1 && isRecon)
        {
            //SendConnData sendConnData = SendConnData.CreateBuilder()
            //        .SetOpenid(GameInfo.OpenID)
            //        .SetRoomID(GameInfo.room_id)
            //        .Build();
            //byte[] body = sendConnData.ToByteArray();
            SendConnData sendConnData = new SendConnData();
            sendConnData.openid = GameInfo.OpenID;
            sendConnData.RoomID = GameInfo.room_id;
            byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendConnData);
            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5019, body.Length, 0, body);
            GameInfo.cs.Send(data);
            GameInfo.returnAddServer = null;
        }
    }
    /// <summary>
    /// 请求开始游戏
    /// </summary>
    private void StartGame()
    {
        //SendStart ss = SendStart.CreateBuilder().SetOpenid(GameInfo.OpenID).SetRoomid(GameInfo.room_id).Build();
        //byte[] body = ss.ToByteArray();
        SendStart ss = new SendStart();
        ss.openid = GameInfo.OpenID;
        ss.roomid = GameInfo.room_id;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(ss);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 2007, body.Length, 0, body);

        GameInfo.cs.Send(data);

    }

    /// <summary>
    /// 一局结束之后，发送请求继续游戏
    /// </summary>
    private void RequireContinueGame()
    {
        //SendGetGame ss = SendGetGame.CreateBuilder().SetFw(GameInfo.FW).SetRoomID(GameInfo.room_id).Build();
        //byte[] body = ss.ToByteArray();
        SendGetGame ss = new SendGetGame();
        ss.fw = GameInfo.FW;
        ss.room_ID = GameInfo.room_id;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(ss);

        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5017, body.Length, 0, body);

        GameInfo.cs.Send(data);
    }
    /// <summary>
    /// 游戏物理逻辑处理
    /// </summary>
    void Update()
    {
        //for (int i = 0; i < myCards.shouPaiGOList.Count; i++)
        //{
        //    if (myCards.shouPaiGOList[i] == null)
        //        myCards.shouPaiGOList.RemoveAt(i);
        //}
        #region Android适配
        //移动设备适配
        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    MJ_w = Screen.height / 19.0f;
        //}
        ////PC端适配
        //else
        //{
        //    MJ_w = Screen.width / 15f;
        //}

        // MJ_w = Screen.height / 51.0f;
        //MJ_w_o = GameObject.Find("Shoupai_S").GetComponent<RectTransform>().rect.width / 15f;
        #endregion
        GameReturnConnData();
        GameReturnRoomMsg();
        GameReturnZhuang();

        ReturnQuickVoice();
        if (isFaPaiEnd)
        {
            GameReturnAll();
            GameReturnHyUser();
            ReturnTTOrTH();
            GameRetrunAddRoom();
            GameReturnMsg();
            GameReturnMP();
            GameReturnPeng();
            GameReturnGang();
            GameReturnMsgList();
            GameReturnFJ();
            GameReturnBTMsg();
            GameReturnTT();
            GameReturnTP();
            GameReturnQYM();
            GameReturnHByType();
            GameReturnJS();
            ShowLastCount();
            // GameReturnStartGame();
            //GameReturnZR();
            GameReturnHTypeAtEnd();
            ChangeButtonAtEnd();
            SendRecon();
        }
        ShowFinalJieSuanPanelAbnormal();
        OnGameOperationProcess();

    }
    /// <summary>
    /// 服务端返回杠集合
    /// </summary>
    void GameReturnGang()
    {
        if (GameInfo.returnGang != null)
        {
            //添加新的动态显示按钮，留着以上作为对比
            guoButtonGO.SetActive(true);
            gangButtonGO.SetActive(true);
            //GameInfo.returnGang.GangList.Count>1
            GameInfo.cunGang = GameInfo.returnGang;

            GameInfo.returnGang = null;
        }

    }


    public bool isNotConntction = true;
    /// <summary>
    /// 返回庄信息
    /// </summary>
    void GameReturnZhuang()
    {
        if (GameInfo.returnZhuang != null)
        {
            Resources.UnloadUnusedAssets();//清除没有使用的资源，试试看能不能解决卡顿
            GameInfo.isAllreadyStart = true;
            GameInfo.isStartGame = true;
            GameInfo.zhuang = GameInfo.returnZhuang.zhuang;
            SeZiQi(GameInfo.nowFW);
            global::FW zhuangFW = GameInfo.GetFW(GameInfo.returnZhuang.zhuang);
            switch (zhuangFW)
            {
                case global::FW.East:
                    GameObject gojie = GameObject.Instantiate(zhuangGO, eJiGridTrans) as GameObject;
                    if (GameInfo.isLZhuang) gojie.transform.Find("count").GetComponent<Text>().text = "X" + GameInfo.returnZhuang.zhuangCount.ToString();
                    gojie.transform.SetSiblingIndex(0);
                    gojie.transform.localPosition = Vector3.zero;
                    gojie.transform.localScale = Vector3.one;
                    break;
                case global::FW.West:
                    GameObject gojiw = GameObject.Instantiate(zhuangGO, wJiGridTrans) as GameObject;
                    if (GameInfo.isLZhuang) gojiw.transform.Find("count").GetComponent<Text>().text = "X" + GameInfo.returnZhuang.zhuangCount.ToString();
                    gojiw.transform.SetSiblingIndex(0);
                    gojiw.transform.localPosition = Vector3.zero;
                    gojiw.transform.localScale = Vector3.one;
                    break;
                case global::FW.North:
                    GameObject gojin = GameObject.Instantiate(zhuangGO, nJiGridTrans) as GameObject;
                    if (GameInfo.isLZhuang) gojin.transform.Find("count").GetComponent<Text>().text = "X" + GameInfo.returnZhuang.zhuangCount.ToString();
                    gojin.transform.SetSiblingIndex(0);
                    gojin.transform.localPosition = Vector3.zero;
                    gojin.transform.localScale = Vector3.one;
                    break;
                case global::FW.South:
                    GameObject gojis = GameObject.Instantiate(zhuangGO, sJiGridTrans) as GameObject;
                    if (GameInfo.isLZhuang) gojis.transform.Find("count").GetComponent<Text>().text = "X" + GameInfo.returnZhuang.zhuangCount.ToString();
                    gojis.transform.SetSiblingIndex(0);
                    gojis.transform.localPosition = Vector3.zero;
                    gojis.transform.localScale = Vector3.one;
                    break;
            }

            managerGame.Room.gameObject.SetActive(false);
            managerGame.Room_Id.gameObject.SetActive(false);
            if (isNotConntction)
            {
                paidui.KaiShiPaiJu();
            }
            else
            {
                if (!string.IsNullOrEmpty(GameInfo.returnZhuang.seizi))
                {
                    string[] str = GameInfo.returnZhuang.seizi.Split(',');
                    int.TryParse(str[0], out paidui.dicNum1);
                    int.TryParse(str[1], out paidui.dicNum2);
                }
            }
        }
        GameInfo.returnZhuang = null;

    }







    /// <summary>
    /// 责任鸡，生成头像的,在碰杠方法中 invoke调用的时
    /// </summary>
    private void GameReturnZR()
    {
        if (GameInfo.returnZR != null)
        {
            // voiceGame();
            //GameInfo.returnZR.gtype;// = 1;(1责任鸡；2责任无骨)
            //GameInfo.returnZR.zrfw; //= 2;责任方位
            //GameInfo.returnZR.dzrfw; //= 3;碰杠人的方位

            global::FW zrfw = GameInfo.GetFW(GameInfo.returnZR.zrfw);
            if (GameInfo.returnZR.gtype == 1)
            {
                switch (zrfw)
                {
                    case global::FW.East:
                        GameObject gojie = GameObject.Instantiate(zerenjiGO, eJiGridTrans) as GameObject;
                        gojie.transform.localPosition = Vector3.zero;
                        gojie.transform.localScale = Vector3.one;
                        //GameObject.Instantiate(deskSkillGOArray[8], AniJi_E.transform.position, Quaternion.identity, deskSkillTran);

                        break;
                    case global::FW.West:
                        GameObject gojiw = GameObject.Instantiate(zerenjiGO, wJiGridTrans) as GameObject;
                        gojiw.transform.localPosition = Vector3.zero;
                        gojiw.transform.localScale = Vector3.one;
                        //GameObject.Instantiate(deskSkillGOArray[8], AniJi_W.transform.position, Quaternion.identity, deskSkillTran);
                        break;
                    case global::FW.North:
                        GameObject gojin = GameObject.Instantiate(zerenjiGO, nJiGridTrans) as GameObject;
                        gojin.transform.localPosition = Vector3.zero;
                        gojin.transform.localScale = Vector3.one;
                        //GameObject.Instantiate(deskSkillGOArray[8], AniJi_N.transform.position, Quaternion.identity, deskSkillTran);
                        break;
                    case global::FW.South:
                        GameObject gojis = GameObject.Instantiate(zerenjiGO, sJiGridTrans) as GameObject;
                        gojis.transform.localPosition = Vector3.zero;
                        gojis.transform.localScale = Vector3.one;
                        // GameObject.Instantiate(deskSkillGOArray[8], AniJi_S.transform.position, Quaternion.identity, deskSkillTran);
                        break;
                }

            }
            else
            {
                switch (zrfw)
                {
                    case global::FW.East:
                        GameObject gojie = GameObject.Instantiate(zerenwuguGO, eJiGridTrans) as GameObject;
                        gojie.transform.localPosition = Vector3.zero;
                        gojie.transform.localScale = Vector3.one;
                        ///路遥170704添加
                        break;
                    case global::FW.West:
                        GameObject gojiw = GameObject.Instantiate(zerenwuguGO, wJiGridTrans) as GameObject;
                        gojiw.transform.localPosition = Vector3.zero;
                        gojiw.transform.localScale = Vector3.one;
                        //GameObject.Instantiate(deskSkillGOArray[7], AniJi_W.transform.position, Quaternion.identity, deskSkillTran);
                        break;
                    case global::FW.North:
                        GameObject gojin = GameObject.Instantiate(zerenwuguGO, nJiGridTrans) as GameObject;
                        gojin.transform.localPosition = Vector3.zero;
                        gojin.transform.localScale = Vector3.one;
                        // GameObject.Instantiate(deskSkillGOArray[7], AniJi_N.transform.position, Quaternion.identity, deskSkillTran);
                        break;
                    case global::FW.South:
                        GameObject gojis = GameObject.Instantiate(zerenwuguGO, sJiGridTrans) as GameObject;
                        gojis.transform.localPosition = Vector3.zero;
                        gojis.transform.localScale = Vector3.one;
                        // GameObject.Instantiate(deskSkillGOArray[7], AniJi_S.transform.position, Quaternion.identity, deskSkillTran);
                        break;
                }
            }
        }
        GameInfo.returnZR = null;
    }
    int tingCount = 0;
    /// <summary>
    /// 返回胡牌
    /// </summary>
    private void GameReturnHByType()
    {
        if (GameInfo.returnHByType != null)
        {
            // hu.SetActive(true);
            //guo.SetActive(true);
            // GameInfo.pengInfo = GameInfo.returnAll;
            GameInfo.huPaiInfo = GameInfo.returnHByType;

            //添加新的动态显示按钮，留着以上作为对比
            guoButtonGO.SetActive(true);
            huButtonGO.SetActive(true);

            if (GameInfo.isTH && tingCount == 0)
            {
                tingCount++;
                tingButtonGO.SetActive(true);
            }

            GameInfo.returnHByType = null;
        }
    }

    ///
    //message ReturnHType
    //{
    //    required int32 FWZ=1;//胡的方位
    //    required int32 type=2;//胡的方式(1:自摸；2:胡牌；3:抢杠；4:热炮;5:杠上开花)
    //    optional int32 FWB=3;//放炮的方位
    //    required MaJiang MJ=4;//胡的牌
    //}
    /// <summary>
    /// 返回胡牌
    /// </summary>
    private void GameReturnHTypeAtEnd()
    {
        ///路遥170704添加
        if (GameInfo.returnHType != null)
        {
            ClearTeXxiao();
            voiceGame();
            GameInfo.gameVoice = GameInfo.returnHType.FWZ;
            GameInfo.cunHType = GameInfo.returnHType;
            GameInfo.cunHtypeType = GameInfo.cunHType.type;


            if (GameInfo.returnHType.DXInfo.Count > 0)
            {
                foreach (var item in GameInfo.returnHType.DXInfo)
                {
                    ShowHuPaiBeforeJI(item.DXFW, item.DXType, GameInfo.returnHType.MJ, GameInfo.returnHType.FWB);
                }
            }
            else
            {
                if (GameInfo.returnHType.type == 1 || GameInfo.returnHType.type == 5)
                {
                    ShowHuPaiBeforeJI(GameInfo.returnHType.FWZ, GameInfo.returnHType.type, GameInfo.returnHType.MJ, GameInfo.returnHType.FWB);
                }
                else
                {
                    ShowHuPaiBeforeJI(GameInfo.returnHType.FWZ, 0, GameInfo.returnHType.MJ, GameInfo.returnHType.FWB);


                }
            }

            //封牌，胡牌之后不许打
            foreach (var item in myCards.shouPaiGOList)
            {
                item.GetComponent<MJ_Event>().isCanOut = false;
            }
        }
        GameInfo.returnHType = null;
    }
    /// <summary>
    /// 在胡牌方位播放胡牌方式的声音，（什么方位显示胡的牌？）
    ///  //message ReturnHType
    //{
    //    required int32 FWZ=1;//胡的方位
    //    required int32 type=2;//胡的方式(1:自摸；2:胡牌；3:抢杠；4:热炮;5:杠上开花)
    //    optional int32 FWB=3;//放炮的方位
    //    required MaJiang MJ=4;//胡的牌
    //}
    /// </summary>
    /// <param name="hFW"></param>
    /// <param name="hType"></param>
    /// <param name="hMJ"></param>
    /// <param name="fpFW"></param>
    void ShowHuPaiBeforeJI(int hFW, int hType, MaJiang hMJ, int fpFW = 0)
    {
        if (hFW != fpFW)
        {
            FW fw = GameInfo.GetFW(hFW);
            switch (fw)
            {
                case FW.East:
                    GameObject.Instantiate(huSkillGOArray[hType], AniJi_E.transform.position, Quaternion.identity, deskSkillTran);
                    break;
                case FW.West:
                    GameObject.Instantiate(huSkillGOArray[hType], AniJi_W.transform.position, Quaternion.identity, deskSkillTran);
                    break;
                case FW.North:
                    GameObject.Instantiate(huSkillGOArray[hType], AniJi_N.transform.position, Quaternion.identity, deskSkillTran);
                    break;
                case FW.South:
                    GameObject.Instantiate(huSkillGOArray[hType], AniJi_S.transform.position, Quaternion.identity, deskSkillTran);
                    break;
            }

        }


    }

    /// <summary>
    /// 服务器下发定缺消息
    /// </summary>
    private void GameReturnQYM()
    {
        if (GameInfo.returnAYM != null)
        {
            foreach (var item in GameInfo.returnAYM.QP)
            {
                //将之前，定缺中。。。几个字变成，相对应缺的门，向头像上方缩小移动，固定
                //item.fw
                //item.Type;1,筒，2，条，3，万
                //dingquezhongGO.gameObject.SetActive(false);

                dingquezhongGO.transform.Find("east_queconfiging").gameObject.SetActive(false);
                dingquezhongGO.transform.Find("west_queconfiging").gameObject.SetActive(false);
                dingquezhongGO.transform.Find("north_queconfiging").gameObject.SetActive(false);
                //dingquehouGO.gameObject.SetActive(true);


                //southImage = transform.Find("/Cans_UI/Image_que/southImage").GetComponent<Image>();

                //southQuePos = transform.Find("/Cans_UI/Image_que/southImage1").transform.position;
                switch (GameInfo.GetFW(item.fw))
                {//(动画移动需要一个位置，位置确定后再添加)
                    case global::FW.East:
                        eastImage.gameObject.SetActive(true);
                        eastImage.overrideSprite = QueTypeArray[item.type];
                        eastImage.transform.DOMove(eastQuePos, 1f);
                        eastImage.SetNativeSize();
                        eastImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
                        break;
                    case global::FW.West:
                        westImage.gameObject.SetActive(true);
                        westImage.overrideSprite = QueTypeArray[item.type];
                        westImage.transform.DOMove(westQuePos, 1f);
                        westImage.SetNativeSize();
                        westImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
                        break;
                    case global::FW.North:
                        northImage.gameObject.SetActive(true);
                        northImage.overrideSprite = QueTypeArray[item.type];
                        northImage.transform.DOMove(northQuePos, 1f);
                        northImage.SetNativeSize();
                        northImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
                        break;
                    case global::FW.South:
                        maskPai.OnQue((GameInfo.QueType)item.type);
                        //southImage.gameObject.SetActive(true);
                        //southImage.overrideSprite = QueTypeArray[item.Type];
                        //southImage.transform.DOMove(southQuePos, 1f);
                        //southImage.transform.DOScale(new Vector3(0.3f, 0.3f, 1), 1);
                        break;
                    default:
                        break;
                }

            }

            GameInfo.returnAYM = null;
            GameInfo.IsDingQue = false;
            if (GameInfo.IsDingQueGang)
            {
                //gangs.SetActive(true);
                //guo.SetActive(true);
                gangButtonGO.SetActive(true);
                guoButtonGO.SetActive(true);
                GameInfo.IsDingQueGang = false;
            }

        }
    }
    /// <summary>
    /// 返回房间信息
    /// </summary>
    private void GameReturnRoomMsg()
    {
        if (GameInfo.returnRoomMsg != null)
        {
            if (GameInfo.returnRoomMsg.is_wgj != 1)//房间未配置乌骨鸡
                GameInfo.cfwgj = true;//不再会有冲锋乌骨鸡
            GameInfo.room_peo = GameInfo.returnRoomMsg.room_peo;
            if (GameInfo.FW == 1) GameInfo.roomInfo.roomPeo = GameInfo.room_peo;
            GameInfo.gameCount = GameInfo.returnRoomMsg.count;//房间局数
            if (GameInfo.FW == 1) GameInfo.roomInfo.juSu = GameInfo.gameCount;
            GameInfo.IsSetRoomInfo = true;
            // GameInfo.returnRoomMsg.IsLianzhuang 
            gameObject.GetComponent<FICMJPlaying>().ShowCards(GameInfo.FW);
            ShowRoomRuleOnDesk();
            PlayerPrefs.SetString("roominfo", JsonUtility.ToJson(GameInfo.roomInfo));
            GameInfo.returnRoomMsg = null;
        }
    }

    private void GameRetrunAddRoom()
    {
        if (GameInfo.returnAddRoom != null)
        {
            if (GameInfo.returnAddRoom.state == 10000)
            {
                //   isClosed = true;
                //加入房间成功
                // FICCreatRoom.MJplayers.Add(GameInfo.dir, GameInfo.returnAddRoom.UserinfoList[GameInfo.returnAddRoom.UserinfoCount - 1]);
                foreach (var item in GameInfo.returnAddRoom.userinfo)
                {
                    if (item.openid.Equals(GameInfo.OpenID))
                        GameInfo.FW = item.user_FW;
                    if (GameInfo.MJplayers.ContainsKey(item.user_FW))
                        GameInfo.MJplayers[item.user_FW] = item;
                    else
                        GameInfo.MJplayers.Add(item.user_FW, item);

                    GameInfo.MJplayersWhoQuit[item.openid] = item.user_FW;
                }
                //GameInfo.sceneID = "Scene_Game";
                // SceneManager.LoadScene("LoadingHall");
                //SceneManager.LoadScene("Scene_Game");
                GameInfo.returnAddRoom = null;
            }
            else if (GameInfo.returnAddRoom.state == 10001)
            {
                GameInfo.returnAddRoom = null;
                FICWaringPanel._instance.Show("房间不存在!");
                Invoke("Close_Warn", 3);
            }
            else if (GameInfo.returnAddRoom.state == 10002)
            {
                GameInfo.returnAddRoom = null;
                FICWaringPanel._instance.Show("房间已满!");
                //房间人数已满
                Invoke("Close_Warn", 3);
            }
        }
    }
    /// <summary>
    /// 显示房间玩法规则
    /// </summary>
    void ShowRoomRuleOnDesk()
    {
        if (GameInfo.returnRoomMsg != null)
        {
            roomRuleText.text = "";
            string roomRule = "";
            GameInfo.roomRlue = null;
            if (GameInfo.returnRoomMsg.QuickCard == 1)
            {
                roomRule += "  十秒出牌";
                if (GameInfo.FW == 1)
                    GameInfo.roomInfo.is_smcp = 1;
                // manager_Time.countDownText.gameObject.SetActive(true);
                manager_Time.isShimiao = true;
            }
            if (GameInfo.returnRoomMsg.Is_yuanque == 1) { roomRule += "  原缺"; GameInfo.isYuanQue = 1; if (GameInfo.FW == 1) GameInfo.roomInfo.is_yuanque = 1; }
            if (GameInfo.returnRoomMsg.is_benji == 1) { roomRule += "  本鸡"; if (GameInfo.FW == 1) GameInfo.roomInfo.is_benji = 1; }
            if (GameInfo.returnRoomMsg.is_shangxiaji == 1) { roomRule += "  摇摆鸡"; if (GameInfo.FW == 1) GameInfo.roomInfo.is_shangxiaji = 1; }
            if (GameInfo.returnRoomMsg.is_xinqiji == 1) { roomRule += "  星期鸡"; if (GameInfo.FW == 1) GameInfo.roomInfo.is_xingqiji = 1; }
            if (GameInfo.returnRoomMsg.is_wgj == 1) { roomRule += "  乌骨鸡"; if (GameInfo.FW == 1) GameInfo.roomInfo.is_wgj = 1; }


            if (GameInfo.returnRoomMsg.is_lianzhuang == 1)
            {
                roomRule += "  连庄";
                GameInfo.isLZhuang = true;
                if (GameInfo.FW == 1) GameInfo.roomInfo.is_yikousan = 3;
            }
            else
            {
                switch (GameInfo.returnRoomMsg.is_yikousan)
                {
                    case 0:
                        roomRule += "  1扣2";
                        GameInfo.isLZhuang = false;
                        if (GameInfo.FW == 1) GameInfo.roomInfo.is_yikousan = 0;
                        break;
                    case 1:
                        roomRule += "  通三";
                        GameInfo.isLZhuang = false;
                        if (GameInfo.FW == 1) GameInfo.roomInfo.is_yikousan = 1;
                        break;
                }
            }
           
            GameInfo.roomRlue = roomRule;
            ShowRoomRule();
        }
    }

    private void ShowRoomRule()
    {
        roomRuleText.text = GameInfo.roomRlue;
        managerGame.juNum.text = GameInfo.gameNum + "/" + GameInfo.gameCount + "局";
    }

    /// <summary>
    /// 返回重发信息
    /// </summary>
    private void GameReturnMsgList()
    {
        if (GameInfo.returnMsgList != null)
        {
            foreach (var item in GameInfo.returnMsgList.msg)
            {
                // var data = item.ToByteArray();
                var data = item;
                int number = IntToByte.bytesToInt(data, 0);//消息号
                int length = IntToByte.bytesToInt(data, 4);//消息长度
                int resnumber = IntToByte.bytesToInt(data, 8);//返回消息号
                byte[] body = new byte[length];
                Array.Copy(data, 12, body, 0, length);
                MethodsByNew(number, body);
            }
            GameInfo.returnMsgList = null;
        }
    }

    /// <summary>
    /// 返回活跃用户     
    /// </summary>
    private void GameReturnHyUser()
    {
        if (GameInfo.returnHyUser != null)
        {

            GameInfo.nowFW = GameInfo.returnHyUser.fw;//活跃用户方位
            GameInfo.HYFw = GameInfo.returnHyUser.fw;
            managerGame.ShowRay(GameInfo.returnHyUser.fw);


            if (GameInfo.FW == GameInfo.returnHyUser.fw && GameInfo.recon)//如果是轮到自己操作 并且是断线重连
            {
                //var returnAll = ReturnAll.CreateBuilder().SetFw(GameInfo.returnHyUser.Fw);
                var returnAll = new ReturnAll();
                returnAll.fw = GameInfo.returnHyUser.fw;
                foreach (var item in GameInfo.returnHyUser.cz.Split('|'))
                {
                    GameInfo.gameVoice = GameInfo.returnHyUser.fw; ///
                    switch (item)
                    {
                        case "3001"://出牌  
                            isChu = true;
                            GameInfo.returnHyUser = null;
                            return;
                        case "30081"://碰牌
                            //returnAll.SetPeng(1).SetMj(GameInfo.returnHyUser.Mj);
                            returnAll.peng = 1;
                            returnAll.mj = GameInfo.returnHyUser.mj;
                            break;
                        case "30082"://杠牌
                                     // returnAll.SetGang(1).SetMj(GameInfo.returnHyUser.Mj);
                            returnAll.gang = 1;
                            returnAll.mj = GameInfo.returnHyUser.mj;
                            break;
                        case "30083"://摸牌
                                     // returnAll.SetMo(1).SetMj(GameInfo.returnHyUser.Mj);
                            returnAll.mo = 1;
                            returnAll.mj = GameInfo.returnHyUser.mj;
                            break;
                        case "30084"://胡牌
                            //returnAll.SetHu(1).SetMj(GameInfo.returnHyUser.Mj);
                            returnAll.hu = 1;
                            returnAll.mj = GameInfo.returnHyUser.mj;
                            break;
                    }
                }
                try
                {
                    // GameInfo.returnAll = returnAll.Build();
                    GameInfo.returnAll = returnAll;
                }
                catch
                {
                }
            }
            else
            {
                foreach (var item in GameInfo.returnHyUser.cz.Split('|'))
                {
                    GameInfo.gameVoice = GameInfo.returnHyUser.fw; ///
                    //当其他用户可以出牌，摸牌，碰牌 杠牌，胡牌的时候，需要在作出操作以后才能使台面的光标闪烁、动画效果和倒计时
                    switch (item)
                    {
                        case "3001"://出牌               
                            SeZiQi(GameInfo.returnHyUser.fw);

                            break;

                        case "30081"://碰牌
                            if (GameInfo.FW == GameInfo.returnHyUser.fw)
                            {
                                SeZiQi(GameInfo.returnHyUser.fw);
                            }
                            else
                            { }
                            break;

                        case "30082"://杠牌
                            if (GameInfo.FW == GameInfo.returnHyUser.fw)
                            {
                                SeZiQi(GameInfo.returnHyUser.fw);
                            }
                            else
                            { }
                            break;

                        case "30083"://摸牌
                            print("加1");
                            SeZiQi(GameInfo.returnHyUser.fw);
                            managerPai.MoPai(GameInfo.GetFW(GameInfo.returnHyUser.fw));
                            if (GameInfo.returnHyUser.fw != GameInfo.FW)
                            {
                                paidui.PaiDuiReduceOneForMo();
                            }
                            break;

                        case "30084"://胡牌
                            if (GameInfo.FW == GameInfo.returnHyUser.fw)
                            {
                                SeZiQi(GameInfo.returnHyUser.fw);
                            }
                            else
                            { }
                            break;
                    }
                }
            }
            GameInfo.returnHyUser = null;
        }
    }
    /// <summary>
    /// 色子器
    /// </summary>
    /// <param name="fw">当前活跃用户</param>
    public void SeZiQi(int fw)
    {//色子器，需要动场景，现全部注销掉，（将四个色子器合成一个，旋转父物体）
        int zhuang = GameInfo.Rfw(GameInfo.zhuang);//
        ////限制出牌时间
        ////GameObject.Find("Main Camera").GetComponent<Mannager_Time>().seziqi_Time();
        //// var nowFw = GameInfo.GetFW(GameInfo.FW);
        var rfw = GameInfo.Rfw(GameInfo.FW);
        int xintiao = 0;

        switch (rfw)
        {
            case 1://如果我的位置是 东， 那么需要旋转色子器
                GameObject.Find("/Game_Prefabs/TABLE/touziqi").transform.localRotation = Quaternion.Euler(0, 0, 0);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink").transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2://如果我的位置是 南， 那么需要旋转色子器
                GameObject.Find("/Game_Prefabs/TABLE/touziqi").transform.localRotation = Quaternion.Euler(0, 90, 0);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink").transform.localRotation = Quaternion.Euler(0, 90, 0);
                break;
            case 3: //如果我的位置是 西， 那么需要旋转色子器
                GameObject.Find("/Game_Prefabs/TABLE/touziqi").transform.localRotation = Quaternion.Euler(0, 180, 0);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink").transform.localRotation = Quaternion.Euler(0, 180, 0);
                break;
            case 4: //如果我的位置是 北， 那么需要旋转色子器
                GameObject.Find("/Game_Prefabs/TABLE/touziqi").transform.localRotation = Quaternion.Euler(0, 270, 0);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink").transform.localRotation = Quaternion.Euler(0, 270, 0);
                break;
        }

        ////当前活跃用户需要闪烁的位置
        switch (GameInfo.Rfw(fw))
        {
            case 2://南
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_E").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 1 ? 2 : 0];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_W").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 3 ? 2 : 0];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_N").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 4 ? 2 : 0];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_S").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 2 ? 3 : 1];

                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_S").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 2 ? 2 : 0];

                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_E").gameObject.SetActive(false);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_W").gameObject.SetActive(false);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_N").gameObject.SetActive(false);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_S").gameObject.SetActive(true);
                break;
            case 1://东
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_E").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 1 ? 3 : 1];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_W").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 3 ? 2 : 0];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_N").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 4 ? 2 : 0];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_S").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 2 ? 2 : 0];

                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_E").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 1 ? 2 : 0];

                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_E").gameObject.SetActive(true);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_W").gameObject.SetActive(false);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_N").gameObject.SetActive(false);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_S").gameObject.SetActive(false);
                break;
            case 4://北
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_E").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 1 ? 2 : 0];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_W").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 3 ? 2 : 0];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_N").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 4 ? 3 : 1];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_S").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 2 ? 2 : 0];

                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_N").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 4 ? 2 : 0];

                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_E").gameObject.SetActive(false);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_W").gameObject.SetActive(false);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_N").gameObject.SetActive(true);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_S").gameObject.SetActive(false);
                break;
            case 3://西
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_E").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 1 ? 2 : 0];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_W").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 3 ? 3 : 1];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_N").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 4 ? 2 : 0];
                GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_S").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 2 ? 2 : 0];

                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_W").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 3 ? 2 : 0];

                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_E").gameObject.SetActive(false);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_W").gameObject.SetActive(true);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_N").gameObject.SetActive(false);
                GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_S").gameObject.SetActive(false);
                break;
        }
        manager_Time.ResetSZQDown();
    }
    private void StartReSetSeZiQi(int fw, int chufw)
    {
        #region 旧代码


        ////限制出牌时间
        ////GameObject.Find("Main Camera").GetComponent<Mannager_Time>().seziqi_Time();
        //// var nowFw = GameInfo.GetFW(GameInfo.FW);
        //var rfw = GameInfo.Rfw(GameInfo.FW);
        //switch (rfw)
        //{
        //    case 1://如果我的位置是 东， 那么需要旋转色子器
        //        GameObject.Find("Table/touziqi").transform.rotation = Quaternion.Euler(0, 0, 0);
        //        break;
        //    case 2://如果我的位置是 南， 那么需要旋转色子器
        //        GameObject.Find("Table/touziqi").transform.rotation = Quaternion.Euler(0, 90, 0);
        //        break;
        //    case 3: //如果我的位置是 西， 那么需要旋转色子器
        //        GameObject.Find("Table/touziqi").transform.rotation = Quaternion.Euler(0, 180, 0);
        //        break;
        //    case 4: //如果我的位置是 北， 那么需要旋转色子器
        //        GameObject.Find("Table/touziqi").transform.rotation = Quaternion.Euler(0, 270, 0);
        //        break;
        //}
        ////当前活跃用户需要闪烁的位置
        //switch (GameInfo.Rfw(chufw))
        //{
        //    case 1://东
        //        GameObject.Find("Table/touziqi/E_turn").SetActive(true);
        //        GameObject.Find("Table/touziqi/S_turn").SetActive(false);
        //        GameObject.Find("Table/touziqi/W_turn").SetActive(false);
        //        GameObject.Find("Table/touziqi/N_turn").SetActive(false);
        //        break;
        //    case 2://南
        //        GameObject.Find("Table/touziqi/S_turn").SetActive(true);
        //        GameObject.Find("Table/touziqi/E_turn").SetActive(false);
        //        GameObject.Find("Table/touziqi/W_turn").SetActive(false);
        //        GameObject.Find("Table/touziqi/N_turn").SetActive(false);
        //        break;
        //    case 3://西
        //        GameObject.Find("Table/touziqi/W_turn").SetActive(true);
        //        GameObject.Find("Table/touziqi/E_turn").SetActive(false);
        //        GameObject.Find("Table/touziqi/S_turn").SetActive(false);
        //        GameObject.Find("Table/touziqi/N_turn").SetActive(false);
        //        break;
        //    case 4://北
        //        GameObject.Find("Table/touziqi/N_turn").SetActive(true);
        //        GameObject.Find("Table/touziqi/E_turn").SetActive(false);
        //        GameObject.Find("Table/touziqi/S_turn").SetActive(false);
        //        GameObject.Find("Table/touziqi/W_turn").SetActive(false);
        //        break;
        //}
        #endregion
    }
    private void OnGameOperationProcess()
    {
        if (GameInfo.gameOperationProcess != null)
        {
            isFaPaiEnd = true;
            isFanJi = true;
            managerPai.nShouPaiQua = Quaternion.Euler(90, 180, 180);
            managerPai.eShouPaiQua = Quaternion.Euler(90, 90, 0);
            managerPai.wShouPaiQua = Quaternion.Euler(90, 90, 180);
            GameInfo.isPlayback = true;
            managerGame.Room.gameObject.SetActive(false);
            managerGame.Room_Id.gameObject.SetActive(false);
            manager_Time.countDownText.gameObject.SetActive(false);
            roomid.gameObject.SetActive(false);
            transform.Find("/Game_UI/PopUp_UI/panel_settlement/finalsettlement/btn_share").gameObject.SetActive(false);
            transform.Find("/Game_UI/PopUp_UI/panel_settlement/littlesettlement/btn_details").gameObject.SetActive(false);
            transform.Find("/Game_UI/Fixed_UI/But_SheZhi").gameObject.SetActive(false);
            transform.Find("/Game_UI/Fixed_UI/But_shuaxin").gameObject.SetActive(false);
            transform.Find("/Game_UI/Fixed_UI/Voice").gameObject.SetActive(false);
            transform.Find("/Game_UI/Fixed_UI/Voice_Dd").gameObject.SetActive(false);
            transform.Find("/Game_UI/Fixed_UI/But_Feedback").gameObject.SetActive(false);
            transform.Find("/Game_UI/Fixed_UI/warn").gameObject.SetActive(false);
            // transform.Find("/Game_UI/Fixed_UI/A").gameObject.SetActive(false);
            // GameInfo.zhuang = GameInfo.gameOperationProcess.GetGameOperationInfo(0).OperationFW;
            GameInfo.zhuang = GameInfo.gameOperationProcess.gameOperationInfo[0].OperationFW;
            //GameInfo.returnRoomMsg = GameInfo.gameOperationProcess.Message;
            GameInfo.returnRoomMsg = GameInfo.gameOperationProcess.message;
            managerGame.juNum.gameObject.SetActive(false);
            //GameInfo.returnUserInfo = GameInfo.gameOperationProcess.UserInfo;
            GameInfo.returnUserInfo = GameInfo.gameOperationProcess.userInfo;
            StartCoroutine(gamePlayback.OperationPlayback(GameInfo.gameOperationProcess.gameOperationInfo, GameInfo.gameOperationProcess.JieSuanInfo));
            GameInfo.gameOperationProcess = null;
        }
    }


    /// <summary>
    /// 断线重新进入房间
    /// </summary>
    private void GameReturnConnData()
    {
        if (GameInfo.returnConnData != null)
        {
            bool isCanBT = true;
            GameInfo.returnUserInfo = GameInfo.returnConnData.userInfo;
            if (GameInfo.FW == 0)
                return;
            if (reconState == 0)
            {
                isFaPaiEnd = true;
                GameInfo.isAllreadyStart = true;
                if (recon == null)
                {
                    recon = gameObject.GetComponent<Reconnection>();
                }
                recon.OnClearObjList();
                reconState++;
            }
            if (reconState == 1)
            {
                GameInfo.room_id = GameInfo.returnConnData.roomID;
                GameInfo.gameNum = GameInfo.returnConnData.GameNumber;
                GameObject.Find("Main Camera").GetComponent<Manager_Game>().Room_Id.text = GameInfo.room_id.ToString();
                GameObject.Find("Main Camera").GetComponent<Manager_Game>().Room_Id.gameObject.SetActive(false);
                GameInfo.returnRoomMsg = GameInfo.returnConnData.message;
                if (GameInfo.returnConnData.JiXuYouXi == 1) { GameInfo.returnConnData = null; return; }
                isNotConntction = false;
                GameInfo.returnZhuang = GameInfo.returnConnData.zhuang;
                //SeZiQi(GameInfo.returnConnData.Zhuang.Zhuang);


                reconState++;
            }

            if (reconState == 2 && GameInfo.returnZhuang == null)
            {
                ShowRoomRule();
                //其他玩家创建手牌
                GameInfo.integrals.Clear();
                foreach (var item in GameInfo.returnConnData.shoupai)
                {
                    if (item.FW != GameInfo.FW)
                    {
                        managerPai.CeretShouPai(GameInfo.GetFW(item.FW), item.Pcount);
                    }
                    else
                    {
                        ShengChengPai(item.mj, GameInfo.returnConnData.hyUser.fw, GameInfo.returnConnData.hyUser.cz);
                    }
                    //显示缺一门
                    if (GameInfo.room_peo != 4 && GameInfo.isYuanQue == 0)
                    {
                        recon.OnShowQYM(item.FW, item.QYM);
                    }
                    if (item.Tianting != 0 && item.FW == GameInfo.FW)
                    {
                        recon.ReconTingPaiInfo(GameInfo.GetFW(item.FW), item.Tianting, "tianting");
                        myCards.ReconnectTingMask();
                    }
                    else if (item.Tianting != 0)
                    {
                        recon.ReconTingPaiInfo(GameInfo.GetFW(item.FW), "tianting", item.Tianting);
                        isCanBT = false;
                    }

                    if (item.Baoting != 0 && item.FW == GameInfo.FW && isCanBT)
                    {
                        recon.ReconTingPaiInfo(GameInfo.GetFW(item.FW), item.Baoting, "baoting");
                        myCards.ReconnectTingMask();
                    }
                    else if(item.Baoting != 0)
                    {
                        recon.ReconTingPaiInfo(GameInfo.GetFW(item.FW), "baoting", item.Baoting);
                    }
                    recon.ShowScore(item.FW, item.Scare);
                }
                //重连碰牌集合
                foreach (var item in GameInfo.returnConnData.peng)
                {
                    MaJiang mj = null;
                    for (int i = 0; i < item.mj.Count; i++)
                    {
                        if (mj == null || mj.PaiHS != item.mj[i].PaiHS)
                        {
                            if (item.FW != GameInfo.FW)
                            {
                                if (GameInfo.room_peo != 2)
                                {
                                    if ((GameInfo.FW + 1 > GameInfo.room_peo ? 1 : GameInfo.FW + 1) == item.FW)
                                    {
                                        //右边用户
                                        mj = item.mj[i];
                                        managerPai.PengGangPai(FW.East, FW.East, mj.PaiHS, PPTYPE.Peng, false);
                                    }
                                    if ((GameInfo.FW == 1 ? GameInfo.room_peo : GameInfo.FW - 1) == item.FW)
                                    {
                                        //左边用户
                                        mj = item.mj[i];
                                        managerPai.PengGangPai(FW.West, FW.West, mj.PaiHS, PPTYPE.Peng, false);
                                    }

                                    if (GameInfo.room_peo == 4 && ((GameInfo.FW + 2) > GameInfo.room_peo ? (GameInfo.FW + 2) - 4 : (GameInfo.FW + 2)) == item.FW)
                                    {
                                        //上方用户
                                        mj = item.mj[i];
                                        managerPai.PengGangPai(FW.North, FW.North, mj.PaiHS, PPTYPE.Peng, false);
                                    }
                                }
                                else
                                {
                                    mj = item.mj[i];
                                    managerPai.PengGangPai(FW.North, FW.North, mj.PaiHS, PPTYPE.Peng, false);
                                }
                            }
                            else
                            {
                                mj = item.mj[i];
                                GameObject qipai = GameObject.Find("Main Camera");
                                myCards.PengPaiEntir(mj.PaiHS, false);
                            }
                        }
                    }
                }
                //重连杠牌集合
                int gangCount = 0;
                foreach (var item in GameInfo.returnConnData.gang)
                {
                    if (!string.IsNullOrEmpty(item.gang))
                    {
                        gangCount++;
                        switch (GameInfo.GetFW(item.FW))
                        {
                            case FW.East:
                                var gang = item.gang.Split(',');
                                for (int i = 0; i < gang.Length - 1; i++)
                                {
                                    managerPai.PengGangPai(FW.East, GameInfo.GetFW(int.Parse(gang[i].Split('|')[1])), int.Parse(gang[i].Split('|')[0]), PPTYPE.Gang, false, gang[i].Split('|')[2]);
                                }
                                break;
                            case FW.West:
                                gang = item.gang.Split(',');
                                for (int i = 0; i < gang.Length - 1; i++)
                                {
                                    managerPai.PengGangPai(FW.West, GameInfo.GetFW(int.Parse(gang[i].Split('|')[1])), int.Parse(gang[i].Split('|')[0]), PPTYPE.Gang, false, gang[i].Split('|')[2]);
                                }
                                break;
                            case FW.North:
                                gang = item.gang.Split(',');
                                for (int i = 0; i < gang.Length - 1; i++)
                                {
                                    managerPai.PengGangPai(FW.North, GameInfo.GetFW(int.Parse(gang[i].Split('|')[1])), int.Parse(gang[i].Split('|')[0]), PPTYPE.Gang, false, gang[i].Split('|')[2]);
                                }
                                break;
                            case FW.South:
                                gang = item.gang.Split(',');
                                for (int i = 0; i < gang.Length - 1; i++)
                                {
                                    myCards.GangREConnect(int.Parse(gang[i].Split('|')[0]), gang[i].Split('|')[2], GameInfo.GetFW(int.Parse(gang[i].Split('|')[1])));
                                }
                                break;
                        }
                    }
                }
                //重连出牌集合
                foreach (var item in GameInfo.returnConnData.chu)
                {
                    foreach (var mjitem in item.mj)
                    {
                        managerPai.ChuPai(GameInfo.GetFW(item.FW), mjitem.PaiHS, mjitem.PaiID, true);
                    }
                    if (item.FW == GameInfo.FW && item.mj.Count == 0) GameInfo.CardsNumber++;
                }
                //显示冲锋或责任鸡
                foreach (var item in GameInfo.returnConnData.jp)
                {
                    if (item.CFJ == 1)
                    {
                        GameObject go = GameObject.Instantiate(chongfengjiGO, recon.GetJiPaiTrans(item.FW)) as GameObject;
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = Vector3.zero;
                    }
                    else if (item.CFJ == 2)
                    {
                        GameObject go = GameObject.Instantiate(zerenjiGO, recon.GetJiPaiTrans(item.FW)) as GameObject;
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = Vector3.zero;
                    }
                    if (item.WGJ == 1)
                    {
                        GameObject go = GameObject.Instantiate(wugujiGO, recon.GetJiPaiTrans(item.FW)) as GameObject;
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = Vector3.zero;
                    }
                    else if (item.WGJ == 2)
                    {
                        GameObject go = GameObject.Instantiate(zerenwuguGO, recon.GetJiPaiTrans(item.FW)) as GameObject;
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = Vector3.zero;
                    }
                }
                //重连牌堆
                if (GameInfo.isYuanQue == 1) gangCount += 36;
                paidui.PaiDuiReconnect(108 - GameInfo.returnConnData.PaiCount - gangCount, gangCount);
                paiCountText.text = GameInfo.returnConnData.PaiCount.ToString() + "张";
                paiCount = GameInfo.returnConnData.PaiCount;
                ReturnPaiCount count = new ReturnPaiCount();
                count.PaiCount = GameInfo.returnConnData.PaiCount;
                GameInfo.returnPaiCount = count;
                GameInfo.returnHyUser = GameInfo.returnConnData.hyUser;

                reconState++;
                if (GameInfo.returnConnData.LastChumj != null)
                {
                    recon.OnMoveDiamond(managerPai.qipaiEList, GameInfo.returnConnData.LastChumj.PaiHS, GameInfo.returnConnData.LastChumj.PaiID);
                    recon.OnMoveDiamond(managerPai.qipaiWList, GameInfo.returnConnData.LastChumj.PaiHS, GameInfo.returnConnData.LastChumj.PaiID);
                    recon.OnMoveDiamond(managerPai.qipaiNList, GameInfo.returnConnData.LastChumj.PaiHS, GameInfo.returnConnData.LastChumj.PaiID);
                    recon.OnMoveDiamond(myCards.qiPaiGOList, GameInfo.returnConnData.LastChumj.PaiHS, GameInfo.returnConnData.LastChumj.PaiID);

                }


            }



            //执行最后操作
            if (reconState == 3)
            {
                if (GameInfo.returnConnData.tp != null)
                {
                    GameInfo.returnTP = GameInfo.returnConnData.tp;
                }
                foreach (var item in GameInfo.returnConnData.SendData)
                {
                   
                    int number = IntToByte.bytesToInt(item, 0);//消息号
                    if (number == 15013) continue;
                        int length = IntToByte.bytesToInt(item, 4);//消息长度
                    int resnumber = IntToByte.bytesToInt(item, 8);//返回消息号
                    byte[] body = new byte[length];
                    Array.Copy(item, 12, body, 0, length);
                    GameInfo.cs.MethodsByNew(number, body);

                }
                maskPai.OnQueYiMenButtonClick(GameInfo.queType, true);

                if (GameInfo.returnConnData.LastMomj != null)
                {
                    GameObject go = null;
                    foreach (var item in myCards.shouPaiGOList)
                    {
                        MJ_SP tempMj = item.GetComponent<MJ_SP>();
                        if (tempMj.HS == GameInfo.returnConnData.LastMomj.PaiHS && tempMj.ID == GameInfo.returnConnData.LastMomj.PaiID && GameInfo.returnConnData.hyUser.fw == GameInfo.FW)
                        {
                            go = item;
                            break;
                        }
                    }
                    if (go != null)
                    {
                        myCards.shouPaiGOList.Remove(go);
                        Destroy(go);//多次摸牌重叠问题
                        maskPai.OnQueYiMenButtonClick(GameInfo.queType, true);
                        myCards.MoPai(GameInfo.returnConnData.LastMomj.PaiHS, GameInfo.returnConnData.LastMomj.PaiID);

                        if (isChu && (GameInfo.isTT || GameInfo.isRealTing)) AutoChuPai();
                    }
                }

                reconState = 0;
                isRecon = false;
                GameInfo.returnConnData = null;
            }

        }
    }
    /// <summary>
    /// 碰杠胡后消息
    /// </summary>
    private void GameReturnPeng()
    {
        if (GameInfo.returnPeng != null)
        {
            voiceGame();
            if (GameInfo.returnPeng.state == 2 && GameInfo.returnPeng.fw == GameInfo.FW)
            {
                paidui.PaiDuiReduceOneForGang();
                isMoNotGamg = false;
            }

            if (GameInfo.returnPeng.state != 4)
                GameInfo.nowFW = GameInfo.returnPeng.fw;
            ///倒计时
            if (GameInfo.returnPeng.state == 3)
            {
                // 接收到胡牌信息
                //Img_Hu.SetActive(true);
                huButtonGO.SetActive(true);
                guoButtonGO.SetActive(true);
            }
            //如果不是自己碰的牌
            else if (GameInfo.returnPeng.state != 4 && GameInfo.nowFW != GameInfo.FW)
            {
                if (GameInfo.returnPeng.state == 1)
                {

                    //GameObject.Find("Main Camera").GetComponent<Click_PengGang>().Peng_E(GameInfo.returnPeng);
                    managerPai.PengGangPai(GameInfo.GetFW(GameInfo.returnPeng.fw), GameInfo.GetFW(GameInfo.returnPeng.Pfw), GameInfo.returnPeng.mj.PaiHS, PPTYPE.Peng, true);
                    ShowDeskSkillsOnDirection(GameInfo.GetFW(GameInfo.returnPeng.fw), PPTYPE.Peng);

                }

                else
                {
                    managerPai.PengGangPai(GameInfo.GetFW(GameInfo.returnPeng.fw), GameInfo.GetFW(GameInfo.returnPeng.Pfw), GameInfo.returnPeng.mj.PaiHS, PPTYPE.Gang, true, GameInfo.returnPeng.Gtype);
                    ShowDeskSkillsOnDirection(GameInfo.GetFW(GameInfo.returnPeng.fw), PPTYPE.Gang);

                }

            }
            GameInfo.returnPeng = null;
            SeZiQi(GameInfo.nowFW);
        }
    }
    /// <summary>
    /// 根据传过来的方位和方式显示，碰杠特效
    /// </summary>
    /// <param name="fw"></param>
    /// <param name="pptype"></param>
    public void ShowDeskSkillsOnDirection(FW fw, PPTYPE pptype)
    {
        if (pptype == PPTYPE.Gang && fw != GameInfo.GetFW(GameInfo.FW))
        {
            paidui.PaiDuiReduceOneForGang();
        }
        UnityEngine.Object GO;
        switch (fw)
        {
            case FW.East:
                GO = pptype == PPTYPE.Gang ? GameObject.Instantiate(deskSkillGOArray[1], AniJi_E.transform.position, Quaternion.identity, deskSkillTran) : GameObject.Instantiate(deskSkillGOArray[0], AniJi_E.transform.position, Quaternion.identity, deskSkillTran);

                break;
            case FW.West:
                GO = pptype == PPTYPE.Gang ? GameObject.Instantiate(deskSkillGOArray[1], AniJi_W.transform.position, Quaternion.identity, deskSkillTran) : GameObject.Instantiate(deskSkillGOArray[0], AniJi_W.transform.position, Quaternion.identity, deskSkillTran);
                break;
            case FW.North:
                GO = pptype == PPTYPE.Gang ? GameObject.Instantiate(deskSkillGOArray[1], AniJi_N.transform.position, Quaternion.identity, deskSkillTran) : GameObject.Instantiate(deskSkillGOArray[0], AniJi_N.transform.position, Quaternion.identity, deskSkillTran);
                break;
            case FW.South:
                GO = pptype == PPTYPE.Gang ? GameObject.Instantiate(deskSkillGOArray[1], AniJi_S.transform.position, Quaternion.identity, deskSkillTran) : GameObject.Instantiate(deskSkillGOArray[0], AniJi_S.transform.position, Quaternion.identity, deskSkillTran);
                break;
        }
        Invoke("GameReturnZR", 1.5f);
    }


    public bool isMoNotGamg = true;
    /// <summary>
    /// 处理摸牌，1,生成摸牌，2，判定是是否缺一门，需要遮罩，3.判定是否听牌，调用自动出牌方法
    /// </summary>
    private void GameReturnMP()
    {
        if (GameInfo.returnMP != null)
        {
            if (isMoNotGamg)
            {
                paidui.PaiDuiReduceOneForMo();
            }
            isMoNotGamg = true;
            GameInfo.nowFW = GameInfo.FW;
            myCards.MoPai(GameInfo.returnMP.mj.PaiHS, GameInfo.returnMP.mj.PaiID);

            if (GameInfo.isRealTing || GameInfo.isTT)
            {

                Invoke("AutoChuPai", 1);
            }
            else
            {
                JudgeMoPaiNeedMask();
            }
            GameInfo.returnMP = null;
        }
    }
    /// <summary>
    /// 判断摸的这张牌是否需要遮罩
    /// </summary>
    private void JudgeMoPaiNeedMask()
    {
        switch (GameInfo.queType)
        {
            case GameInfo.QueType.Tong:
                if (GameInfo.returnMP.mj.PaiHS > 10 && maskPai.isHaveTong)
                {
                    myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1].GetComponent<MeshRenderer>().material.color = darkColor;
                    maskPai.maskList.Add(myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1]);
                }
                break;
            case GameInfo.QueType.Tiao:
                if ((GameInfo.returnMP.mj.PaiHS < 10 || GameInfo.returnMP.mj.PaiHS > 20) && maskPai.isHaveTiao)
                {
                    myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1].GetComponent<MeshRenderer>().material.color = darkColor;
                    maskPai.maskList.Add(myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1]);
                }
                break;
            case GameInfo.QueType.Wan:
                if (GameInfo.returnMP.mj.PaiHS < 20 && maskPai.isHaveWan)
                {
                    myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1].GetComponent<MeshRenderer>().material.color = darkColor;
                    maskPai.maskList.Add(myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1]);
                }
                break;
        }
    }


    /// <summary>
    /// 自动出牌
    /// </summary>
    private void AutoChuPai()
    {

        /**/
        if (!huButtonGO.activeInHierarchy
       && !guoButtonGO.activeInHierarchy
       && !pengButtonGO.activeInHierarchy
       && !gangButtonGO.activeInHierarchy
       && !tingButtonGO.activeInHierarchy)
        {
            managerPai.ChuPai(myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1].GetComponent<MJ_Event>());
            foreach (var item in myCards.shouPaiGOList)
            {
                item.GetComponent<MJ_Event>().isCanOut = false;
            }
        }

    }

    /// <summary>
    /// 游戏播放音效
    /// </summary>
    public void voiceGame()
    {
        //==============================牌花色、鸡音效===================================//
        PlayJI();

        //===========================碰、杠牌音效==========================//
        PlayPengGang();


        //=====================责任鸡，责任乌骨鸡音效=========================//
        //if (GameInfo.returnZR != null)
        //{
        //    GameInfo.gameVoice = GameInfo.returnZR.zrfw;
        //    if (GameInfo.returnZR.gtype.Equals(1) && GameInfo.returnPeng == null)
        //    {
        //        managerAudio.AudioClick("ZRJ");
        //    }

        //    else if (GameInfo.returnZR.gtype.Equals(2) && GameInfo.returnPeng == null)
        //    {
        //        managerAudio.AudioClick("ZRWGJ");
        //    }
        //}

        //==================================胡牌音效========================================//
        PlayHu();

        //======================翻鸡音效==========================//
        if (GameInfo.returnFJ != null)
        {
            //GameInfo.gameVoice = GameInfo.returnHType.FWZ;
            //FICAudioPlay._instance.PlaySound(GameInfo.returnHType.FWZ, "Fanji");

            //OutLog.log("播放杠声音，现在的类型是：fanji");
        }

        //======================放炮音效=========================//
        if (GameInfo.returnHType != null && GameInfo.returnHType.FWB != 0)
        {
            FICAudioPlay._instance.PlaySound(GameInfo.returnHType.FWZ, "Fangpao");

            OutLog.log("播放杠声音，现在的类型是：fangpao");


        }
    }

    #region voiceGameTyptJudge
    private void PlayJI()
    {

        if (GameInfo.returnMsg != null)
        {
            if ((GameInfo.returnMsg.msg.Equals("CFJ") || GameInfo.returnMsg.msg.Equals("CFWG")) && GameInfo.returnMsg.mj.PaiHS == 0) return;
            OutLog.log("播放牌声音,牌的花色是：" + GameInfo.returnMsg.mj.PaiHS);
            FICAudioPlay._instance.PlaySound(GameInfo.returnMsg.FW, "", GameInfo.returnMsg.mj.PaiHS);
        }
        ////==============================牌花色、鸡音效===================================//
        //if (GameInfo.returnMsg != null && !GameInfo.returnMsg.msg.Equals("CFJ") && !GameInfo.returnMsg.msg.Equals("CFWG"))
        //{
        //    //managerAudio.AudioPai(GameInfo.returnMsg.mj.PaiHS);
        //    FICAudioPlay._instance.PlaySound(GameInfo.returnMsg.FW, "", GameInfo.returnMsg.mj.PaiHS);
        //}
        //else if (GameInfo.returnMsg != null && GameInfo.returnMsg.msg.Equals("CFJ"))
        //{
        //    FICAudioPlay._instance.PlaySound(GameInfo.returnMsg.FW, "CFJ");
        //}

        //else if (GameInfo.returnMsg != null && GameInfo.returnMsg.msg.Equals("CFWG"))
        //{
        //    FICAudioPlay._instance.PlaySound(GameInfo.returnMsg.FW, "CFWG");
        //}

    }

    private void PlayPengGang()
    {
        //===========================碰、杠牌音效==========================//
        if (GameInfo.returnPeng != null)
        {
            GameInfo.gameVoice = GameInfo.returnPeng.fw;
            if (GameInfo.returnPeng.state == 1)
            {
                OutLog.log("播放杠声音，现在的类型是：peng");

                FICAudioPlay._instance.PlaySound(GameInfo.returnPeng.fw, "Peng");
            }
            else if (GameInfo.returnPeng.Gtype.Equals("Z"))
            {
                OutLog.log("播放杠声音，现在的类型是：zhuanwandou");
                FICAudioPlay._instance.PlaySound(GameInfo.returnPeng.fw, "Zhuanwandou");
            }
            else if (GameInfo.returnPeng.Gtype.Equals("A"))
            {
                OutLog.log("播放杠声音，现在的类型是：andou");
                FICAudioPlay._instance.PlaySound(GameInfo.returnPeng.fw, "Andou");
            }
            else if (GameInfo.returnPeng.Gtype.Equals("M"))
            {
                OutLog.log("播放杠声音，现在的类型是：mingdou");
                FICAudioPlay._instance.PlaySound(GameInfo.returnPeng.fw, "Mingdou");
            }
            else if (GameInfo.returnPeng.Gtype.Equals("H"))
            {
                OutLog.log("播放杠声音，现在的类型是：gang");
                FICAudioPlay._instance.PlaySound(GameInfo.returnPeng.fw, "Gang");
            }
        }
    }

    private void PlayHu()
    {
        //==================================胡牌音效========================================//
        if (GameInfo.returnHType != null)
        {
            if (GameInfo.returnHType.FWB != 0)
            {
                return;

            }
            GameInfo.gameVoice = GameInfo.returnHType.FWZ;
            if (GameInfo.returnHType.type.Equals(1))
            {
                FICAudioPlay._instance.PlaySound(GameInfo.returnHType.FWZ, "Zimo");

                OutLog.log("播放杠声音，现在的类型是：zimo");
            }

            else if (GameInfo.returnHType.type.Equals(2))
            {
                FICAudioPlay._instance.PlaySound(GameInfo.returnHType.FWZ, "Hu");

                OutLog.log("播放杠声音，现在的类型是：hu");
            }

        }

    }
    #endregion
    /// <summary>

    /// 处理其他用户打牌信息
    /// </summary>
    private void GameReturnMsg()
    {
        //处理其他用户打牌信息
        if (GameInfo.returnMsg != null)
        {
            GameInfo.gameVoice = GameInfo.returnMsg.FW;
            voiceGame();
            GameInfo.nowFW = GameInfo.returnMsg.FW;
            // GameInfo.returnMsg.Msg;显示牌信息功能未完成
            if (GameInfo.FW == GameInfo.returnMsg.FW)
            {
                if (GameInfo.cloneMjs != null && GameInfo.cloneMjs.Count > 0)
                {
                    foreach (var item in GameInfo.cloneMjs)
                    {
                        Destroy(item);
                    }
                }
                //if (!GameInfo.cfj && chuPai.GetComponent<MJ_SP>().HS == 11)
                if (GameInfo.returnMsg.msg.Equals("CFJ"))
                {
                    //S冲锋鸡
                    GameInfo.cfj = true;
                    GameObject gojis = GameObject.Instantiate(chongfengjiGO, sJiGridTrans) as GameObject;
                    gojis.transform.localPosition = Vector3.zero;
                    gojis.transform.localScale = Vector3.one;
                    GameObject.Instantiate(deskSkillGOArray[5], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                    Instantiate(Resources.Load("Game_GYMJ/Prefabs/texiao/JiTX"), jiTx.position, Quaternion.identity, jiTx);
                }

                // if (!GameInfo.cfwgj && chuPai.GetComponent<MJ_SP>().HS == 8)
                if (GameInfo.returnMsg.msg.Equals("CFWG"))
                {
                    //S冲锋乌骨鸡
                    GameInfo.cfwgj = true;
                    GameObject gojis = GameObject.Instantiate(wugujiGO, sJiGridTrans) as GameObject;
                    gojis.transform.localPosition = Vector3.zero;
                    gojis.transform.localScale = Vector3.one;

                    GameObject.Instantiate(deskSkillGOArray[6], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                }
                //////////================2017-06-29 修改收到服务器信息后才将牌打出==================
                if (GameInfo.returnMsg.mj.PaiID != 0)
                {
                    //GameInfo.nowFW = GameInfo.nowFW == GameInfo.room_peo ? 1 : GameInfo.nowFW + 1;
                    var mjInfo = myCards.shouPaiGOList.Find(w => w.GetComponent<MJ_SP>().ID == GameInfo.returnMsg.mj.PaiID);
                    //managerPai.RemoveMJ(mjInfo.GetComponent<MJ_Event>());
                    if (mjInfo != null)
                    {
                        pengButtonGO.SetActive(false);
                        gangButtonGO.SetActive(false);
                        guoButtonGO.SetActive(false);
                        huButtonGO.SetActive(false);
                        tingButtonGO.SetActive(false);
                        myCards.DestroyThisShouPai(mjInfo.gameObject);
                        managerPai.ChuPai(global::FW.South, mjInfo.GetComponent<MJ_SP>().HS, mjInfo.GetComponent<MJ_SP>().ID);

                    }
                }
                //////////==================================
            }



            if (GameInfo.room_peo != 2)
            {
                if ((GameInfo.FW + 1 > GameInfo.room_peo ? 1 : GameInfo.FW + 1) == GameInfo.returnMsg.FW)
                {
                    if (GameInfo.returnMsg.msg.Equals("CFJ"))
                    {
                        //E冲锋鸡
                        GameInfo.cfj = true;
                        GameObject gojie = GameObject.Instantiate(chongfengjiGO, eJiGridTrans) as GameObject;
                        gojie.transform.localPosition = Vector3.zero;
                        gojie.transform.localScale = Vector3.one;
                        GameObject.Instantiate(deskSkillGOArray[5], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                        Instantiate(Resources.Load("Game_GYMJ/Prefabs/texiao/JiTX"), jiTx.position, Quaternion.identity, jiTx);
                    }

                    if (GameInfo.returnMsg.msg.Equals("CFWG"))
                    {
                        //E乌骨鸡
                        GameInfo.cfwgj = true;
                        GameObject gojie = GameObject.Instantiate(wugujiGO, eJiGridTrans) as GameObject;
                        gojie.transform.localPosition = Vector3.zero;
                        gojie.transform.localScale = Vector3.one;
                        GameObject.Instantiate(deskSkillGOArray[6], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                    }

                    //右边用户
                    //GameObject qipai = GameObject.Find("Main Camera");
                    //qipai.GetComponent<Manager_Qipai>().AddPai_E(GameInfo.returnMsg.Mj.PaiHS, GameInfo.returnMsg.Mj.PaiID, GameObject.Find("Qipai_E").transform);
                    //GameObject game = GameObject.Find("ShouPai_E");
                    ///Destroy(game.gameObject.transform.GetChild(game.gameObject.transform.childCount - 1).gameObject);
                    if (GameInfo.returnMsg.mj.PaiID != 0)
                        managerPai.ChuPai(global::FW.East, GameInfo.returnMsg.mj.PaiHS, GameInfo.returnMsg.mj.PaiID);
                }
                if ((GameInfo.FW == 1 ? GameInfo.room_peo : GameInfo.FW - 1) == GameInfo.returnMsg.FW)
                {
                    if (GameInfo.returnMsg.msg.Equals("CFJ"))
                    {
                        //W冲锋鸡
                        GameInfo.cfj = true;
                        GameObject gojiw = GameObject.Instantiate(chongfengjiGO, wJiGridTrans) as GameObject;
                        gojiw.transform.localPosition = Vector3.zero;
                        gojiw.transform.localScale = Vector3.one;
                        GameObject.Instantiate(deskSkillGOArray[5], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                        Instantiate(Resources.Load("Game_GYMJ/Prefabs/texiao/JiTX"), jiTx.position, Quaternion.identity, jiTx);
                    }

                    if (GameInfo.returnMsg.msg.Equals("CFWG"))
                    {
                        //W冲锋乌骨鸡
                        GameInfo.cfwgj = true;
                        GameObject gojiw = GameObject.Instantiate(wugujiGO, wJiGridTrans) as GameObject;
                        gojiw.transform.localPosition = Vector3.zero;
                        gojiw.transform.localScale = Vector3.one;
                        GameObject.Instantiate(deskSkillGOArray[6], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                    }
                    //左边用户
                    //GameObject qipai = GameObject.Find("Main Camera");
                    //qipai.GetComponent<Manager_Qipai>().AddPai_W(GameInfo.returnMsg.Mj.PaiHS, GameInfo.returnMsg.Mj.PaiID, GameObject.Find("Qipai_W").transform);
                    //GameObject game = GameObject.Find("ShouPai_W");
                    // Destroy(game.gameObject.transform.GetChild(game.gameObject.transform.childCount - 1));
                    //Destroy(game.gameObject.transform.GetChild(game.gameObject.transform.childCount - 1).gameObject);
                    if (GameInfo.returnMsg.mj.PaiID != 0)
                        managerPai.ChuPai(global::FW.West, GameInfo.returnMsg.mj.PaiHS, GameInfo.returnMsg.mj.PaiID);
                }
                if (GameInfo.room_peo == 4)
                {
                    if (((GameInfo.FW + 2) > GameInfo.room_peo ? (GameInfo.FW + 2) - 4 : (GameInfo.FW + 2)) == GameInfo.returnMsg.FW)
                    {
                        if (GameInfo.returnMsg.msg.Equals("CFJ"))
                        {
                            //N冲锋鸡
                            GameInfo.cfj = true;
                            GameObject gojin = GameObject.Instantiate(chongfengjiGO, nJiGridTrans) as GameObject;
                            gojin.transform.localPosition = Vector3.zero;
                            gojin.transform.localScale = Vector3.one;
                            GameObject.Instantiate(deskSkillGOArray[5], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                            Instantiate(Resources.Load("Game_GYMJ/Prefabs/texiao/JiTX"), jiTx.position, Quaternion.identity, jiTx);
                        }

                        if (GameInfo.returnMsg.msg.Equals("CFWG"))
                        {
                            //N乌骨鸡
                            GameInfo.cfwgj = true;
                            GameObject gojin = GameObject.Instantiate(wugujiGO, nJiGridTrans) as GameObject;
                            gojin.transform.localPosition = Vector3.zero;
                            gojin.transform.localScale = Vector3.one;
                            GameObject.Instantiate(deskSkillGOArray[6], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                        }
                        //上方用户
                        //GameObject qipai = GameObject.Find("Main Camera");
                        //qipai.GetComponent<Manager_Qipai>().AddPai_N(GameInfo.returnMsg.Mj.PaiHS, GameInfo.returnMsg.Mj.PaiID, GameObject.Find("Qipai_N").transform);
                        //GameObject game = GameObject.Find("ShouPai_N");
                        // Destroy(game.gameObject.transform.GetChild(game.gameObject.transform.childCount - 1).gameObject);
                        if (GameInfo.returnMsg.mj.PaiID != 0)
                            managerPai.ChuPai(global::FW.North, GameInfo.returnMsg.mj.PaiHS, GameInfo.returnMsg.mj.PaiID);
                    }
                }
            }
            else//二人牌桌只有对家
            {
                if (GameInfo.returnMsg.FW != GameInfo.FW)
                {
                    if (GameInfo.returnMsg.msg.Equals("CFJ"))
                    {
                        //N冲锋鸡
                        GameInfo.cfj = true;
                        GameObject gojin = GameObject.Instantiate(chongfengjiGO, nJiGridTrans) as GameObject;
                        gojin.transform.localPosition = Vector3.zero;
                        gojin.transform.localScale = Vector3.one;
                        GameObject.Instantiate(deskSkillGOArray[5], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                        Instantiate(Resources.Load("Game_GYMJ/Prefabs/texiao/JiTX"), jiTx.position, Quaternion.identity, jiTx);
                    }

                    if (GameInfo.returnMsg.msg.Equals("CFWG"))
                    {
                        //N乌骨鸡
                        GameInfo.cfwgj = true;
                        GameObject gojin = GameObject.Instantiate(wugujiGO, nJiGridTrans) as GameObject;
                        gojin.transform.localPosition = Vector3.zero;
                        gojin.transform.localScale = Vector3.one;

                        GameObject.Instantiate(deskSkillGOArray[6], deskSkillTran.position, Quaternion.identity, deskSkillTran);
                    }
                    if (GameInfo.returnMsg.mj.PaiID != 0)
                        managerPai.ChuPai(global::FW.North, GameInfo.returnMsg.mj.PaiHS, GameInfo.returnMsg.mj.PaiID);
                }
                ///

            }

            GameInfo.returnMsg = null;
        }
    }

    /// <summary>
    /// 处理自己牌的状态
    /// </summary>
    private void GameReturnAll()
    {
        if (myCards.textureArray != null)
        {
            ///处理自己牌的状态
            if (GameInfo.returnAll != null)
            {
                Debug.Log(GameInfo.returnAll.fw + "," + GameInfo.returnAll.ToString());
                GameInfo.nowFW = GameInfo.returnAll.fw;
                SeZiQi(GameInfo.nowFW);
                //碰
                if (GameInfo.returnAll.peng != 0 && GameInfo.returnAll.peng == 1)
                {//haspeng
                    //pengs.SetActive(true);
                    //guo.SetActive(true);
                    GameInfo.pengInfo = GameInfo.returnAll;

                    //添加新的动态显示按钮，留着以上作为对比
                    guoButtonGO.SetActive(true);
                    pengButtonGO.SetActive(true);

                }
                //杠
                if (GameInfo.returnAll.gang != 0 && GameInfo.returnAll.gang == 1)
                {
                    if (GameInfo.IsDingQue)
                    {
                        GameInfo.IsDingQueGang = true;
                    }
                    else
                    {
                        //gangs.SetActive(true);
                        //guo.SetActive(true);

                        //添加新的动态显示按钮，留着以上作为对比
                        guoButtonGO.SetActive(true);
                        gangButtonGO.SetActive(true);

                    }
                    GameInfo.pengInfo = GameInfo.returnAll;
                }
                //和 胡牌消息改成其他协议发送
                //if (GameInfo.returnAll.HasHu && GameInfo.returnAll.Hu == 1)
                //{
                //    hu.SetActive(true);
                //    guo.SetActive(true);
                //    GameInfo.pengInfo = GameInfo.returnAll;
                //}
                //摸
                if (GameInfo.returnAll.mo != 0 && GameInfo.returnAll.mo == 1)
                {
                    //var mp = SendMP.CreateBuilder().SetOpenid(GameInfo.OpenID).SetRoomid(GameInfo.room_id).SetMType(GameInfo.returnAll.HasMType ? GameInfo.returnAll.MType : 0).Build();
                    var mp = new SendMP();
                    mp.openid = GameInfo.OpenID;
                    mp.roomid = GameInfo.room_id;
                    mp.mType = GameInfo.returnAll.mType;

                    // byte[] body = mp.ToByteArray();
                    byte[] body = ProtobufUtility.GetByteFromProtoBuf(mp);
                    byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 3002, body.Length, 0, body);

                    GameInfo.cs.Send(data);
                    GameInfo.CardsNumber++;
                }
                GameInfo.returnAll = null;
            }
        }
    }

    /// <summary>
    /// //开始游戏
    /// </summary>
    public void GameReturnStartGame()
    {
        //开始游戏
        if (GameInfo.returnStartGame != null)
        {
            if (myCards.shouPaiGOList.Count != 0)
            {
                return;
            }
            GameInfo.isClosed = false;
            GameInfo.isAllreadyStart = true;
            try
            {
                myCards.CreatCardsAtStart(GameInfo.returnStartGame.mj);
                FillUserPai(GameInfo.room_peo);
                //GameInfo.PaiXu(myCards.shouPaiGOList, MJ_S_Floder);
                myCards.SortAllShouPai();
                if (myCards.shouPaiGOList.Count == 14)
                {
                    myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1].transform.position += myCards.shouPaiOffsetX;

                }
            }
            catch (Exception ex)
            {
                OutLog.log("创建手牌，并排序，异常！！！！！！！！！！！" + ex.ToString());
            }
            //获取当前出牌的用户
            GameInfo.nowFW = GameInfo.returnStartGame.chuuser;
            if (GameInfo.nowFW == GameInfo.FW)
            {
                GameInfo.CardsNumber++;
            }

            if (!GameInfo.IsDingQueClick)
            {
                if (GameInfo.room_peo < 4 && GameInfo.isYuanQue == 0)
                {
                    QueYiMen.SetActive(true);
                    GameInfo.IsDingQueClick = true;
                    GameInfo.IsDingQue = true;
                    //在其他用户前 提示 定缺中····，直接所有人面前显示定缺中，方便快捷，等下同时生成
                    dingquezhongGO.gameObject.SetActive(true);
                }
                ///20170627路遥添加
                else
                {
                    dingquezhongGO.gameObject.SetActive(false);
                }
            }
            else
            {

            }

            StartReSetSeZiQi(GameInfo.FW, GameInfo.returnStartGame.chuuser);
            GameInfo.returnStartGame = null;
            ShowRoomRule();
        }
    }
    /// <summary>
    /// 打完色子之后，开始游戏
    /// </summary>
    public void GameReturnStartGameNew()
    {
        //4张4张的生成个人手牌，
        //4张3张的生成其他玩家手牌
        //手牌生成完成之后，倒牌，排序，立牌，显示定缺
        if (GameInfo.returnStartGame != null)
        {
            GameInfo.isClosed = false;
            GameInfo.isAllreadyStart = true;
            GameInfo.gameNum = GameInfo.returnStartGame.gamenumber;
            try
            {
                myCards.CreatCatdsAtStartByFour(GameInfo.returnStartGame.mj);
                FillUserPai(GameInfo.room_peo);
                //GameInfo.PaiXu(myCards.shouPaiGOList, MJ_S_Floder);
            }
            catch (Exception ex)
            {
                OutLog.log("生成所有人的手牌，异常！！！！！！！！！！！！" + ex.ToString());
            }

        }

    }

    public void GameReturnStartGameNewSide()
    {
        //获取当前出牌的用户
        GameInfo.nowFW = GameInfo.returnStartGame.chuuser;
        if (GameInfo.nowFW == GameInfo.FW)
        {
            GameInfo.CardsNumber++;
        }

        if (!GameInfo.IsDingQueClick)
        {
            if (GameInfo.room_peo < 4 && GameInfo.isYuanQue == 0)
            {
                QueYiMen.SetActive(true);
                GameInfo.IsDingQueClick = true;
                GameInfo.IsDingQue = true;
                //在其他用户前 提示 定缺中····，直接所有人面前显示定缺中，方便快捷，等下同时生成
                dingquezhongGO.gameObject.SetActive(true);
            }
            ///20170627路遥添加
            else
            {
                dingquezhongGO.gameObject.SetActive(false);
            }
        }
        else
        {

        }
        foreach (var item in myCards.shouPaiGOList)
        {
            item.GetComponent<MJ_Event>().isCanOut = true;
        }
        StartReSetSeZiQi(GameInfo.FW, GameInfo.returnStartGame.chuuser);
        GameInfo.returnStartGame = null;
        ShowRoomRule();
        manager_Time.isFawanpai = true;
        isFaPaiEnd = true;
        manager_Time.ShowTenDaojiShi();
    }
    /// <summary>
    /// 返回是否天听和天胡
    /// </summary>
    private void ReturnTTOrTH()
    {
        if (GameInfo.returnTTOrTH != null)
        {
            switch (GameInfo.returnTTOrTH.state)
            {
                //天听    
                case 213:
                    tingButtonGO.SetActive(true);
                    guoButtonGO.SetActive(true);
                    GameInfo.isTT = true;
                    break;
                // 天和
                case 214:
                    //    huButtonGO.SetActive(true);
                    //    guoButtonGO.SetActive(true);  
                    GameInfo.isTH = true;
                    break;
            }
            GameInfo.returnTTOrTH = null;
        }
    }
    /// <summary>
    /// 根据牌桌类型生成其他玩家手牌
    /// </summary>
    /// <param name="type"></param>
    private void FillUserPai(int type, int lenth = 14)
    {

        if (type != 2)
        {
            //右边用户
            managerPai.CeretShouPai(global::FW.East, (GameInfo.FW + 1 > type ? 1 : GameInfo.FW + 1) == GameInfo.returnStartGame.chuuser ? 14 : 13);


            //左边用户
            managerPai.CeretShouPai(global::FW.West, (GameInfo.FW == 1 ? type : GameInfo.FW - 1) == GameInfo.returnStartGame.chuuser ? 14 : 13);
            root.transform.Find("Interaction_UI/Texts_que/east_queconfiging").gameObject.SetActive(true);
            root.transform.Find("Interaction_UI/Texts_que/west_queconfiging").gameObject.SetActive(true);
            if (type == 4)//三人桌没有对家
            {
                managerPai.CeretShouPai(global::FW.North, ((GameInfo.FW + 2) > type ? (GameInfo.FW + 2) - 4 : (GameInfo.FW + 2)) == GameInfo.returnStartGame.chuuser ? 14 : 13);

                root.transform.Find("Interaction_UI/Texts_que/north_queconfiging").gameObject.SetActive(true);
            }
        }
        else//二人桌
        {

            root.transform.Find("Interaction_UI/Texts_que/north_queconfiging").gameObject.SetActive(true);
            //   managerPai.CeretShouPai(global::FW.North, ((GameInfo.FW + 1) > type ? (GameInfo.FW + 1) - 4 : (GameInfo.FW + 2)) == GameInfo.returnStartGame.Chuuser ? 14 : 13);

            managerPai.CeretShouPai(global::FW.North, GameInfo.returnStartGame.chuuser == GameInfo.FW ? 13 : 14);

        }
    }
    /// <summary>
    /// 生成正面玩家手牌
    /// </summary>
    /// <param name="mjList">麻将数量</param>
    private void ShengChengPai(IList<MaJiang> mjList, int fw, string cz)
    {
        if (mjList.Count < 13)
        {
            if (fw != GameInfo.FW || cz == "30083")
            {
                myCards.firstShouPaiTrans.position = myCards.newFirstShouPaiTrans + (13 - mjList.Count) * myCards.shouPaiOffsetX;
            }
            else
            {
                myCards.firstShouPaiTrans.position = myCards.newFirstShouPaiTrans + (14 - mjList.Count) * myCards.shouPaiOffsetX;
            }
        }
        myCards.CreatCardsAtStart(mjList);
    }
    /// <summary>
    /// 过牌
    /// </summary>
    public void Guo()
    {
        tingButtonGO.SetActive(false);
        pengButtonGO.SetActive(false);
        gangButtonGO.SetActive(false);
        guoButtonGO.SetActive(false);
        huButtonGO.SetActive(false);


        if (GameInfo.pengInfo != null || GameInfo.huPaiInfo != null || GameInfo.cunGang != null || GameInfo.isTT != false || GameInfo.isTH != false)
        {
            //var sp = SendPeng.CreateBuilder().SetTypes(GameInfo.isTH ? 5 : (GameInfo.huPaiInfo != null ? 3 : (GameInfo.pengInfo != null ? 1 : GameInfo.cunGang != null ? 2 : 4)))
            //.SetState(2)
            //.SetOpenid(GameInfo.OpenID)
            //.SetRoomid(GameInfo.room_id);
            //if (GameInfo.isTT)
            //    sp.SetFW(GameInfo.FW);
            //if (GameInfo.pengInfo != null)
            //    sp.SetFW(GameInfo.pengInfo.Fw);//服务端需要的是 打出可以碰的牌的方位，不是当前方位 GameInfo.pengInfo.Fw : (GameInfo.huPaiInfo.HasFWB ? GameInfo.huPaiInfo.FWB : 0)
            //if (GameInfo.huPaiInfo != null)
            //    sp.SetFW(GameInfo.huPaiInfo.HasFWB ? GameInfo.huPaiInfo.FWB : GameInfo.huPaiInfo.FWZ);
            //if (GameInfo.cunGang != null)
            //    sp.SetFW(GameInfo.cunGang.GangList[0].Fw);
            //if (GameInfo.pengInfo != null || GameInfo.huPaiInfo != null || GameInfo.cunGang != null)
            //    sp.SetMj(GameInfo.pengInfo != null ? GameInfo.pengInfo.Mj : (GameInfo.huPaiInfo != null ? GameInfo.huPaiInfo.MJ : GameInfo.cunGang.GangList[0].Mj));
            //if (GameInfo.cunGang != null)
            //{
            //    GameObject qipai = GameObject.Find("Main Camera");
            //    string gangType = qipai.GetComponent<FICpaipaipai>().GetGangType(GameInfo.cunGang.GangList[0].Type);
            //    sp.SetGtype(gangType);
            //}
            var sp = new SendPeng();
            sp.types = GameInfo.isTH ? 5 : (GameInfo.huPaiInfo != null ? 3 : (GameInfo.pengInfo != null ? 1 : GameInfo.cunGang != null ? 2 : 4));
            sp.state = 2;
            sp.openid = GameInfo.OpenID;
            sp.roomid = GameInfo.room_id;
            if (GameInfo.isTT)
                sp.FW = GameInfo.FW;
            if (GameInfo.pengInfo != null)
                sp.FW = GameInfo.pengInfo.fw;
            if (GameInfo.huPaiInfo != null)
                sp.FW = (GameInfo.huPaiInfo.FWB != 0 ? GameInfo.huPaiInfo.FWB : GameInfo.huPaiInfo.FWZ);
            if (GameInfo.cunGang != null)
                sp.FW = (GameInfo.cunGang.gang[0].fw);
            if (GameInfo.pengInfo != null || GameInfo.huPaiInfo != null || GameInfo.cunGang != null)
                sp.mj = (GameInfo.pengInfo != null ? GameInfo.pengInfo.mj : (GameInfo.huPaiInfo != null ? GameInfo.huPaiInfo.MJ : GameInfo.cunGang.gang[0].mj));
            if (GameInfo.cunGang != null)
            {
                GameObject qipai = GameObject.Find("Main Camera");
                string gangType = qipai.GetComponent<FICpaipaipai>().GetGangType(GameInfo.cunGang.gang[0].Type);
                sp.Gtype = (gangType);
            }



            //byte[] body = sp.Build().ToByteArray();
            byte[] body = ProtobufUtility.GetByteFromProtoBuf(sp);
            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 4001, body.Length, 0, body);
            GameInfo.cs.Send(data);
            GameInfo.pengInfo = null;
            GameInfo.huPaiInfo = null;
            GameInfo.returnHByType = null;
            GameInfo.cunGang = null;
            GameInfo.isTH = false;
            if (!GameInfo.TT_bl)
            {
                GameInfo.isTT = false;
            }
        }
        GameInfo.returnTP = null;

    }
    /// <summary>
    /// 胡牌
    /// </summary>
    public void Hu()
    {
        pengButtonGO.SetActive(false);
        gangButtonGO.SetActive(false);
        guoButtonGO.SetActive(false);
        huButtonGO.SetActive(false);
        if (GameInfo.huPaiInfo.FWZ == GameInfo.FW)
        {
            GameObject go = Instantiate(myCards.shouPaiGO);
            //如果是别人放炮或者抢杠，那张牌归自己，算鸡用
            if (GameInfo.huPaiInfo.type == 2 || GameInfo.huPaiInfo.type == 3 || GameInfo.huPaiInfo.type == 4)
            {
                go.layer = 0;
                go.GetComponent<MJ_SP>().HS = GameInfo.huPaiInfo.MJ.PaiHS;
                go.GetComponent<MJ_SP>().ID = GameInfo.huPaiInfo.MJ.PaiID;
                myCards.shouPaiGOList.Add(go);
            }
            //如果别人放炮，那张牌从别人弃牌区移除，算鸡用
            if (GameInfo.huPaiInfo.type == 2 || GameInfo.huPaiInfo.type == 3)
            {
                switch (GameInfo.GetFW(GameInfo.huPaiInfo.FWB))
                {
                    case FW.East:

                        go.transform.position = managerPai.qipaiEList[managerPai.qipaiEList.Count - 1].transform.position;
                        go.transform.rotation = managerPai.qipaiEList[managerPai.qipaiEList.Count - 1].transform.rotation;
                        go.GetComponent<Renderer>().material.mainTexture = myCards.textureArray[go.GetComponent<MJ_SP>().HS];
                        Destroy(managerPai.qipaiEList[managerPai.qipaiEList.Count - 1]);
                        managerPai.qipaiEList.RemoveAt(managerPai.qipaiEList.Count - 1);
                        break;
                    case FW.North:
                        Instantiate(go);
                        go.transform.position = managerPai.qipaiNList[managerPai.qipaiNList.Count - 1].transform.position;
                        go.transform.rotation = managerPai.qipaiNList[managerPai.qipaiNList.Count - 1].transform.rotation;
                        go.GetComponent<Renderer>().material.mainTexture = myCards.textureArray[go.GetComponent<MJ_SP>().HS];
                        Destroy(managerPai.qipaiNList[managerPai.qipaiNList.Count - 1]);
                        managerPai.qipaiNList.RemoveAt(managerPai.qipaiNList.Count - 1);
                        break;
                    case FW.West:
                        Instantiate(go);
                        go.transform.position = managerPai.qipaiWList[managerPai.qipaiWList.Count - 1].transform.position;
                        go.transform.rotation = managerPai.qipaiWList[managerPai.qipaiWList.Count - 1].transform.rotation;
                        go.GetComponent<Renderer>().material.mainTexture = myCards.textureArray[go.GetComponent<MJ_SP>().HS];
                        Destroy(managerPai.qipaiWList[managerPai.qipaiWList.Count - 1]);
                        managerPai.qipaiWList.RemoveAt(managerPai.qipaiWList.Count - 1);
                        break;
                }
                managerPai.ResetDiamondPosition(managerPai.emptyGO.transform.position);
            }
        }


        if (GameInfo.huPaiInfo.FWB > 0)
        {
            //     var sp = SendHu.CreateBuilder().SetFWZ(GameInfo.huPaiInfo.FWZ)
            //.SetFWB(GameInfo.huPaiInfo.FWB)
            //.SetType(GameInfo.huPaiInfo.Type)
            // .SetMJ(GameInfo.huPaiInfo.MJ)
            // .SetRoomid(GameInfo.room_id).SetOpenid(GameInfo.OpenID)
            // .Build();
            //     byte[] body = sp.ToByteArray();

            var sp = new SendHu();
            sp.FWZ = GameInfo.huPaiInfo.FWZ;
            sp.FWB = GameInfo.huPaiInfo.FWB;
            sp.type = GameInfo.huPaiInfo.type;
            sp.MJ = GameInfo.huPaiInfo.MJ;
            sp.roomid = GameInfo.room_id;
            sp.openid = GameInfo.OpenID;
            byte[] body = ProtobufUtility.GetByteFromProtoBuf(sp);


            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5016, body.Length, 0, body);
            GameInfo.cs.Send(data);
            GameInfo.huPaiInfo = null;
        }
        else
        {
            //  var sp = SendHu.CreateBuilder().SetFWZ(GameInfo.huPaiInfo.FWZ)
            //.SetType(GameInfo.huPaiInfo.Type)
            // .SetMJ(GameInfo.huPaiInfo.MJ)
            // .SetRoomid(GameInfo.room_id).SetOpenid(GameInfo.OpenID)
            // .SetRoomid(GameInfo.room_id)
            // .SetOpenid(GameInfo.OpenID)
            // .SetRoomid(GameInfo.room_id)
            // .Build();
            //  byte[] body = sp.ToByteArray();

            var sp = new SendHu();
            sp.FWZ = GameInfo.huPaiInfo.FWZ;
            sp.type = GameInfo.huPaiInfo.type;
            sp.MJ = GameInfo.huPaiInfo.MJ;
            sp.roomid = GameInfo.room_id;
            sp.openid = GameInfo.OpenID;

            byte[] body = ProtobufUtility.GetByteFromProtoBuf(sp);

            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5016, body.Length, 0, body);
            GameInfo.cs.Send(data);
            GameInfo.huPaiInfo = null;
        }


    }

    /// <summary>
    /// 根据消息号分发给不同的方法
    /// </summary>
    /// <param name="news"></param>
    private void MethodsByNew(int news, byte[] body)
    {
        //var data = item.ToByteArray();

        switch (news)
        {
            case 12004://服务器返回加入信息
                //GameInfo.returnAddRoom = ReturnAddRoom.ParseFrom(body);
                GameInfo.returnAddRoom = ProtobufUtility.DeserializeProtobuf<ReturnAddRoom>(body);
                Debug.Log("R12004:" + GameInfo.returnAddRoom.ToString());
                break;
            case 12005://服务器主动推送加入玩家信息
                GameInfo.returnUserInfo = ProtobufUtility.DeserializeProtobuf<ReturnUserInfo>(body);
                Debug.Log("R12005:" + GameInfo.returnUserInfo.ToString());
                break;
            case 12006://服务器返回主动推送游戏开始信息
                GameInfo.returnStartGame = ProtobufUtility.DeserializeProtobuf<ReturnStartGame>(body);
                Debug.Log("R12006:" + GameInfo.returnStartGame.ToString());
                GameReturnStartGame();
                break;
            case 12008://服务器返回开始游戏
                GameInfo.returnStart = ProtobufUtility.DeserializeProtobuf<ReturnStart>(body);
                Debug.Log("R12008:" + GameInfo.returnStart.ToString());
                break;
            case 13009://返回出牌信息，出牌方，出牌的花色
                GameInfo.returnMsg = ProtobufUtility.DeserializeProtobuf<ReturnMsg>(body);
                Debug.Log("R13009:" + GameInfo.returnMsg.ToString());
                GameReturnMsg();
                break;
            case 13008://返回碰杠胡摸 四个信息和 方位
                GameInfo.returnAll = ProtobufUtility.DeserializeProtobuf<ReturnAll>(body);
                Debug.Log("R13008:" + GameInfo.returnAll.ToString());
                GameReturnAll();
                break;
            case CreateHead.CSXYNUM + 3003://返回摸牌
                GameInfo.returnMP = ProtobufUtility.DeserializeProtobuf<ReturnMP>(body);
                Debug.Log("R" + CreateHead.CSXYNUM + 3003 + GameInfo.returnMP.ToString());
                GameReturnMP();
                break;
            case CreateHead.CSXYNUM + 4002://返回下发碰杠胡消息
                GameInfo.returnPeng = ProtobufUtility.DeserializeProtobuf<ReturnPeng>(body);
                Debug.Log("R" + CreateHead.CSXYNUM + 4002 + GameInfo.returnPeng.ToString());
                GameReturnPeng();
                break;
            case CreateHead.CSXYNUM + 6001://返回是否断线重连
                GameInfo.returnRecon = ProtobufUtility.DeserializeProtobuf<ReturnRecon>(body);
                Debug.Log("R" + CreateHead.CSXYNUM + 6001 + GameInfo.returnRecon.ToString());
                break;
            case CreateHead.CSXYNUM + 7001://返回断线前消息
                GameInfo.returnConnData = ProtobufUtility.DeserializeProtobuf<ReturnConnData>(body);
                Debug.Log("R" + CreateHead.CSXYNUM + 7001 + GameInfo.returnConnData.ToString());
                //GameReturnConnData();
                break;
            case CreateHead.CSXYNUM + 7004://返回断线前消息
                GameInfo.returnHyUser = ProtobufUtility.DeserializeProtobuf<ReturnHyUser>(body);
                Debug.Log("R" + CreateHead.CSXYNUM + 7004 + GameInfo.returnHyUser.ToString());
                GameReturnHyUser();
                break;
            case CreateHead.CSXYNUM + 7002://返回重新发牌信息
                GameInfo.returnMsgList = ProtobufUtility.DeserializeProtobuf<ReturnMsgList>(body);
                Debug.Log("R" + CreateHead.CSXYNUM + 7002 + GameInfo.returnMsgList.ToString());
                GameReturnMsgList();
                break;

            case CreateHead.CSXYNUM + 7005:///返回翻鸡牌
                GameInfo.returnMsgList = ProtobufUtility.DeserializeProtobuf<ReturnMsgList>(body);
                Debug.Log(CreateHead.CSXYNUM + 7005 + GameInfo.returnMsgList.ToString());
                GameReturnMsgList();
                break;

            case CreateHead.CSXYNUM + 5002://返回听牌方位
                GameInfo.returnBTMSG = ProtobufUtility.DeserializeProtobuf<ReturnBTMsg>(body);
                Debug.Log(CreateHead.CSXYNUM + 5002 + GameInfo.returnBTMSG.ToString());
                GameReturnBTMsg();
                break;
            case CreateHead.CSXYNUM + 2009://返回打出叫牌的集合
                GameInfo.returnTP = ProtobufUtility.DeserializeProtobuf<ReturnTP>(body);
                Debug.Log(CreateHead.CSXYNUM + 2009 + GameInfo.returnMsgList.ToString());
                GameReturnTP();
                break;
            case CreateHead.CSXYNUM + 7009://返回结算信息(全体)
                GameInfo.returnJS = ProtobufUtility.DeserializeProtobuf<ReturnJS>(body);
                Debug.Log(CreateHead.CSXYNUM + 7009 + GameInfo.returnJS.ToString());
                GameReturnJS();
                break;
            case CreateHead.CSXYNUM + 5004://返回请求解散房间信息(所有人接收，客户端自主判断是否显示)
                GameInfo.returnJSMsg = ProtobufUtility.DeserializeProtobuf<ReturnJSMsg>(body);
                Debug.Log(CreateHead.CSXYNUM + 5004 + GameInfo.returnJSMsg.ToString());
                break;
            case CreateHead.CSXYNUM + 5006://返回请求解散房间信息(所有人接收，客户端自主判断是否显示)
                GameInfo.returnJSByOnew = ProtobufUtility.DeserializeProtobuf<ReturnJSByOnew>(body);
                Debug.Log(CreateHead.CSXYNUM + 5006 + GameInfo.returnJSByOnew.ToString());
                break;
            case CreateHead.CSXYNUM + 5007://返回单个用户同意解散房间信息
                GameInfo.returnALLIdea = ProtobufUtility.DeserializeProtobuf<ReturnAllIdea>(body);
                Debug.Log(CreateHead.CSXYNUM + 5007 + GameInfo.returnALLIdea.ToString());
                break;
            case CreateHead.CSXYNUM + 7003://返回单个用户同意解散房间信息
                GameInfo.returnRoomMsg = ProtobufUtility.DeserializeProtobuf<ReturnRoomMsg>(body);
                Debug.Log(CreateHead.CSXYNUM + 5007 + GameInfo.returnRoomMsg.ToString());
                GameReturnRoomMsg();
                break;
            case CreateHead.CSXYNUM + 7008://玩家手牌集合
                GameInfo.returnUserSPai = ProtobufUtility.DeserializeProtobuf<ReturnUserSPai>(body);
                Debug.Log(CreateHead.CSXYNUM + 7008 + GameInfo.returnUserSPai.ToString());
                break;
            case CreateHead.CSXYNUM + 5012://服务器下发定缺信息
                GameInfo.returnAYM = ProtobufUtility.DeserializeProtobuf<ReturnAYM>(body);
                Debug.Log(CreateHead.CSXYNUM + 5012 + GameInfo.returnAYM.ToString());
                break;
            case CreateHead.CSXYNUM + 5015://返回胡牌（替换原3008的胡
                GameInfo.returnHByType = ProtobufUtility.DeserializeProtobuf<ReturnHByType>(body);
                Debug.Log(CreateHead.CSXYNUM + 5015 + GameInfo.returnHByType.ToString());
                GameReturnHByType();
                break;
            case CreateHead.CSXYNUM + 7006://返回剩余牌堆数目
                GameInfo.returnPaiCount = ProtobufUtility.DeserializeProtobuf<ReturnPaiCount>(body);
                Debug.Log(CreateHead.CSXYNUM + 7006 + GameInfo.returnPaiCount.ToString());
                ShowLastCount();
                break;
            ///170704路遥添加
            case CreateHead.CSXYNUM + 5021:
                var returnHType = ProtobufUtility.DeserializeProtobuf<ReturnHType>(body);
                GameInfo.returnHType = returnHType;
                Debug.Log(CreateHead.CSXYNUM + 5021 + returnHType.ToString());
                OutLog.log("15021:startgame:返回胡牌");
                break;
            case CreateHead.CSXYNUM + 5018:
                var returnZR = ProtobufUtility.DeserializeProtobuf<ReturnZR>(body);
                GameInfo.returnZR = returnZR;
                Debug.Log(CreateHead.CSXYNUM + 5018 + returnZR.ToString());
                OutLog.log("15018:startgame:返回责任鸡");
                break;
        }
    }
    /// <summary>
    /// 显示还剩下多少张牌
    /// </summary>
    void ShowLastCount()
    {
        if (GameInfo.returnPaiCount != null)
        {
            paiCountText.transform.parent.gameObject.SetActive(true);
            paiCountText.text = GameInfo.returnPaiCount.PaiCount.ToString() + "张";
            paiCount = GameInfo.returnPaiCount.PaiCount;
        }
        GameInfo.returnPaiCount = null;

    }

    /// <summary>
    /// 发送听牌通知
    /// 只有天听用这个，报听会有手牌集合选择，出牌时候才发送
    /// 
    /// </summary>
    /// <param name="openid"></param>
    /// <param name="type">报听和天听（1：报听，2：天听）</param>
    /// <param name="roomID">房间ID</param>
    public void GameSendBT(string openid, int type, int roomID)
    {
        //MaJiang majiang = MaJiang.CreateBuilder().SetPaiHS(0).SetPaiID(0).Build();
        //SendBT BT = SendBT.CreateBuilder()
        //    .SetOpenid(openid)
        //    .SetType(type)
        //    .SetRoomID(roomID).SetMj(majiang)
        //    .Build();
        //byte[] body = BT.ToByteArray();

        MaJiang majiang = new MaJiang();
        majiang.PaiHS = 0;
        majiang.PaiID = 0;
        SendBT BT = new SendBT();
        BT.openid = openid;
        BT.type = type;
        BT.roomID = roomID;
        BT.mj = majiang;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(BT);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5001, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }

    int sTingNum = 0;
    int eTingNum = 0;
    int wTingNum = 0;
    int nTingNum = 0;

    /// <summary>
    /// 返回普通报听听牌方位
    /// </summary>
    private void GameReturnBTMsg()
    {
        if (GameInfo.returnBTMSG != null)
        {
            ///路遥170704添加
            if (GameInfo.FW == GameInfo.returnBTMSG.fw)
            {
                if (sTingNum == 0)
                {
                    GameObject go = GameObject.Instantiate(deskSkillGOArray[2], deskSkillTran.localPosition, Quaternion.identity, deskSkillTran) as GameObject;
                    go.transform.localPosition = deskSkillTran.localPosition;
                    StartCoroutine(go.GetComponent<TingMove>().Move(AniJi_S.transform.localPosition));
                    //GameObject gojis = GameObject.Instantiate(tingGO, sJiGridTrans) as GameObject;
                    //gojis.transform.localPosition = Vector3.zero;
                    //gojis.transform.localScale = Vector3.one;
                    sTingNum++;

                }



            }

            if (GameInfo.room_peo != 2)
            {
                if ((GameInfo.FW + 1 > GameInfo.room_peo ? 1 : GameInfo.FW + 1) == GameInfo.returnBTMSG.fw)
                {
                    if (eTingNum == 0)
                    {
                        GameObject tf = GameObject.Instantiate(deskSkillGOArray[2], deskSkillTran.localPosition, Quaternion.identity, deskSkillTran) as GameObject;
                        tf.transform.localPosition = deskSkillTran.localPosition;
                        StartCoroutine(tf.GetComponent<TingMove>().Move(AniJi_E.transform.localPosition));
                        //GameObject gojie = GameObject.Instantiate(tingGO, eJiGridTrans) as GameObject;
                        //gojie.transform.localPosition = Vector3.zero;
                        //gojie.transform.localScale = Vector3.one;
                        eTingNum++;
                    }

                }

                if ((GameInfo.FW == 1 ? GameInfo.room_peo : GameInfo.FW - 1) == GameInfo.returnBTMSG.fw)
                {
                    if (wTingNum == 0)
                    {
                        GameObject tf = GameObject.Instantiate(deskSkillGOArray[2], deskSkillTran.localPosition, Quaternion.identity, deskSkillTran) as GameObject;
                        tf.transform.localPosition = deskSkillTran.localPosition;
                        StartCoroutine(tf.GetComponent<TingMove>().Move(AniJi_W.transform.localPosition));
                        //GameObject gojiw = GameObject.Instantiate(tingGO, wJiGridTrans) as GameObject;
                        //gojiw.transform.localPosition = Vector3.zero;
                        //gojiw.transform.localScale = Vector3.one;
                        wTingNum++;
                    }

                }
                if (GameInfo.room_peo == 4)//三人桌没有对家
                {
                    if (((GameInfo.FW + 2) > GameInfo.room_peo ? (GameInfo.FW + 2) - 4 : (GameInfo.FW + 2)) == GameInfo.returnBTMSG.fw)
                    {
                        if (nTingNum == 0)
                        {
                            GameObject tf = GameObject.Instantiate(deskSkillGOArray[2], deskSkillTran.localPosition, Quaternion.identity, deskSkillTran) as GameObject;
                            tf.transform.localPosition = deskSkillTran.localPosition;
                            StartCoroutine(tf.GetComponent<TingMove>().Move(AniJi_N.transform.localPosition));
                            //GameObject gojin = GameObject.Instantiate(tingGO, nJiGridTrans) as GameObject;
                            //gojin.transform.localPosition = Vector3.zero;
                            //gojin.transform.localScale = Vector3.one;
                            nTingNum++;

                        }

                    }
                }
            }
            else//二人桌
            {
                if (GameInfo.FW != GameInfo.returnBTMSG.fw)
                {
                    if (nTingNum == 0)
                    {
                        GameObject tf = GameObject.Instantiate(deskSkillGOArray[2], deskSkillTran.localPosition, Quaternion.identity, deskSkillTran) as GameObject;
                        tf.transform.localPosition = deskSkillTran.localPosition;
                        StartCoroutine(tf.GetComponent<TingMove>().Move(AniJi_N.transform.localPosition));
                        //GameObject gojin = GameObject.Instantiate(tingGO, nJiGridTrans) as GameObject;
                        //gojin.transform.localPosition = Vector3.zero;
                        //gojin.transform.localScale = Vector3.one;
                        nTingNum++;

                    }

                }
            }
        }
        GameInfo.returnBTMSG = null;
    }
    /// <summary>
    /// 返回天听方位
    /// </summary>
    private void GameReturnTT()
    {
        if (GameInfo.returnTT != null)
        {

            FW fw = GameInfo.GetFW(GameInfo.returnTT.fw);
            switch (fw)
            {
                case FW.East:
                    GameObject tf = GameObject.Instantiate(deskSkillGOArray[9], deskSkillTran.localPosition, Quaternion.identity, deskSkillTran) as GameObject;
                    tf.transform.localPosition = deskSkillTran.localPosition;
                    StartCoroutine(tf.GetComponent<TingMove>().Move(AniJi_E.transform.localPosition));
                    //GameObject gojie = GameObject.Instantiate(tingGO, eJiGridTrans) as GameObject;
                    //gojie.transform.localPosition = Vector3.zero;
                    //gojie.transform.localScale = Vector3.one;
                    break;
                case FW.West:
                    tf = GameObject.Instantiate(deskSkillGOArray[9], deskSkillTran.localPosition, Quaternion.identity, deskSkillTran) as GameObject;
                    tf.transform.localPosition = deskSkillTran.localPosition;
                    StartCoroutine(tf.GetComponent<TingMove>().Move(AniJi_W.transform.localPosition));
                    //GameObject gojiw = GameObject.Instantiate(tingGO, wJiGridTrans) as GameObject;
                    //gojiw.transform.localPosition = Vector3.zero;
                    //gojiw.transform.localScale = Vector3.one;
                    break;
                case FW.North:
                    tf = GameObject.Instantiate(deskSkillGOArray[9], deskSkillTran.localPosition, Quaternion.identity, deskSkillTran) as GameObject;
                    tf.transform.localPosition = deskSkillTran.localPosition;
                    StartCoroutine(tf.GetComponent<TingMove>().Move(AniJi_N.transform.localPosition));
                    //GameObject gojin = GameObject.Instantiate(tingGO, nJiGridTrans) as GameObject;
                    //gojin.transform.localPosition = Vector3.zero;
                    //gojin.transform.localScale = Vector3.one;
                    break;
                case FW.South:
                    tf = GameObject.Instantiate(deskSkillGOArray[9], deskSkillTran.localPosition, Quaternion.identity, deskSkillTran) as GameObject;
                    tf.transform.localPosition = deskSkillTran.localPosition;
                    StartCoroutine(tf.GetComponent<TingMove>().Move(AniJi_S.transform.localPosition));
                    //GameObject gojis = GameObject.Instantiate(tingGO, sJiGridTrans) as GameObject;
                    //gojis.transform.localPosition = Vector3.zero;
                    //gojis.transform.localScale = Vector3.one;
                    break;
            }
            GameInfo.returnTT = null;
        }

    }
    /// <summary>
    /// 返回打出叫牌的集合
    /// </summary>
    private void GameReturnTP()
    {
        //如果听牌集合中存在牌，并且此时并没有打出一张牌 则可以选择报听或者天听。
        if (GameInfo.returnTP != null
            && GameInfo.returnTP.mj.Count > 0
            && GameInfo.CardsNumber == 1
            && !isBaoTing
            && GameInfo.HYFw == GameInfo.FW
            //&& GameObject.Find("Main Camera").GetComponent<FICMaskPai>().maskList.Count == 0
            )//在缺一门的情况下手牌会被遮罩， 所以判断如果没有遮罩 那么就可以报听
        {
            //ting.SetActive(true);
            //guo.SetActive(true);


            //这里就报听了？？？   
            //添加新的动态显示按钮，留着以上作为对比
            if (!GameInfo.isTT && !isBaoTing)
            {
                guoButtonGO.SetActive(true);
                tingButtonGO.SetActive(true);
            }
            isBaoTing = true;
            // TingPaiMask();
        }
    }

    /// <summary>
    /// 听牌之后，给非打出听牌集合中的牌加上遮罩
    /// </summary>
    public void TingPaiMask()
    {
        if (GameInfo.isTH)
        {//有天胡选择了天听，以后过牌不会是过天胡
            GameInfo.isHhouTing = true;
            GameInfo.isTH = false;
        }
        else
        {
            List<int> indexList = new List<int>();
            bool isNeedMask = true;
            //遍历手牌和返回听牌合集，找到手牌中不加遮罩的牌的下标(太难反选，换成找应该加遮罩的牌的下标)
            for (int i = 0; i < myCards.shouPaiGOList.Count; i++)
            {
                for (int j = 0; j < GameInfo.returnTP.mj.Count; j++)
                {
                    if (myCards.shouPaiGOList[i].GetComponent<MJ_SP>().HS == GameInfo.returnTP.mj[j].PaiHS)
                    {
                        isNeedMask = false;
                    }
                }
                if (isNeedMask)
                {
                    indexList.Add(i);
                }
                isNeedMask = true;

            }

            for (int i = 0; i < maskPai.maskList.Count; i++)
            {
                maskPai.maskList[i].GetComponent<MeshRenderer>().material.color = resetColor;
                maskPai.maskList[i].GetComponent<MJ_Event>().isCanOut = true;
            }
            maskPai.maskList.Clear();
            for (int i = 0; i < indexList.Count; i++)
            {
                myCards.shouPaiGOList[indexList[i]].GetComponent<MeshRenderer>().material.color = darkColor;

                myCards.shouPaiGOList[indexList[i]].GetComponent<MJ_Event>().isCanOut = false;

                maskPai.maskList.Add(myCards.shouPaiGOList[indexList[i]].gameObject);
            }
        }



    }

    public bool isShowFanji = true;
    /// <summary>
    /// 返回翻鸡牌
    /// </summary>
    private void GameReturnFJ()
    {

        if (GameInfo.returnFJ != null)
        {

            int zhuang = GameInfo.Rfw(GameInfo.zhuang);//
            manager_Time.countDownText.gameObject.SetActive(false);
            manager_Time.szqTime = 100f;

            int hs = GameInfo.returnFJ.HS;
            //===================翻鸡后活跃用户不闪烁=====================//
            GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_E").gameObject.SetActive(false);
            GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_W").gameObject.SetActive(false);
            GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_N").gameObject.SetActive(false);
            GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_S").gameObject.SetActive(false);

            GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_E").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 1 ? 0 : 0];
            GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_W").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 3 ? 0 : 0];
            GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_N").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 4 ? 0 : 0];
            GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_S").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 2 ? 0 : 0];

            if (hs == -1)
            {
                isShowFanji = false;
                jipai2d.fanJiHS = hs;
            }
            else
            {

                fanjipaihs = hs;
                //显示鸡牌 
                jipai2d.fanJiHS = hs;
                Invoke("ShowZhuoJi", 1f);

            }

            GameInfo.returnFJ = null;
        }
    }
    /// <summary>
    /// 这是显示捉鸡两个字的图片
    /// </summary>
    void ShowZhuoJi()
    {
        FICAudioPlay._instance.PlaySound(GameInfo.gameVoice, "Fanji");
        OutLog.log("播放杠声音，现在的类型是：fanji");
        jipai2d.zhuoJiGO.SetActive(true);
        Invoke("Close3DFanJi", 1f);
    }
    /// <summary>
    /// 把3d翻鸡显示出来，并关掉捉鸡2个字
    /// </summary>
    void Close3DFanJi()
    {
        fanJiPaiGO.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[fanjipaihs];
        fanJiPaiGO.SetActive(true);
        isFanJi = true;//显示完翻鸡牌后 才能有后续显示出现、
        jipai2d.zhuoJiGO.SetActive(false);
        fanJiPaiGO.GetComponent<Animation>().Play();
    }

    bool isHuangPai = false;
    bool isNotHu = true;
    /// <summary>
    /// 返回结算信息(全体)
    /// 把所有人牌放倒，并在台面上显示每个人的鸡牌。3秒后弹出结算窗口。
    /// </summary>
    private void GameReturnJS()
    {
        if (GameInfo.returnJS != null && !isFanJi)
        {
            //是否有人胡牌了
            foreach (var item in GameInfo.returnJS.js)
            {
                //if (item.HasIsPao)
                //{
                //    isNotHu = false;
                //}
                if (item.is_pao != 0)
                {
                    isNotHu = false;
                }
            }
            //如果最后一张，没有人胡牌就走荒牌
            //if (paiCount == 0 && !isHuangPai && isNotHu)
            if (!isHuangPai && isNotHu)
            {
                jipai2d.huangpaiGO.SetActive(true);

                foreach (var item in myCards.shouPaiGOList)
                {
                    item.GetComponent<MJ_Event>().isCanOut = false;
                }
                isHuangPai = true;
                isShowFanji = false;
                ReceiveJieSuan();

                GameInfo.cunJS = GameInfo.returnJS;
                GameInfo.returnJS = null;
            }
            //如果最后一张有人胡牌
            // if (paiCount == 0 && !isNotHu)
            if (!isShowFanji && !isNotHu)
            {
                isShowFanji = false;
                ReceiveJieSuan();

                GameInfo.cunJS = GameInfo.returnJS;
                GameInfo.returnJS = null;
            }

        }

        if (GameInfo.returnJS != null && isFanJi)
        {
            
            ReceiveJieSuan();

            GameInfo.cunJS = GameInfo.returnJS;
            GameInfo.returnJS = null;
        }
    }
    /// <summary>
    /// 荒牌和正常结算都会调用这个方法
    /// 调用show3dpai，给其他家手牌赋值
    /// </summary>
    void ReceiveJieSuan()
    {
        isFanJi = false;
        isAutoShow = true;
        // GameInfo.returnUserSPai.UspList.First(w => w.Fw == item.Userinfo.UserFW);
        foreach (var item in GameInfo.returnJS.js)
        {
            var uspInfo = GameInfo.returnUserSPai.usp.First(w => w.fw == item.userinfo.user_FW);
            if (item.userinfo.user_FW == GameInfo.FW)
            {
                if (item.is_pao == 0)
                {
                    if (item.is_jiao == 0)
                    {

                        ShowIsPaoAndIsJiao(FW.South, 13);
                    }
                    else
                    {

                        ShowIsPaoAndIsJiao(FW.South, 12);
                    }

                }
                else
                {
                    if (GameInfo.cunHtypeType == 1 && item.is_pao == 5)
                    {

                        ShowIsPaoAndIsJiao(FW.South, 14);
                    }
                    else
                    {

                        ShowIsPaoAndIsJiao(FW.South, item.is_pao);
                    }

                }
                SaveNormalJiByCountForSouth(item);
                if (item.is_jiao == 0)
                {
                    jipai2d.sIsBao = true;
                }
            }
            else
            {
                if (GameInfo.room_peo != 2)
                {
                    if ((GameInfo.FW + 1 > GameInfo.room_peo ? 1 : GameInfo.FW + 1) == item.userinfo.user_FW)
                    {
                        Show3DPai(uspInfo.mj, FW.East);
                        SaveNormalJiByCountForEast(item);

                        if (item.is_jiao == 0)
                        {
                            jipai2d.eIsBao = true;
                        }

                        if (item.is_pao == 0)
                        {
                            if (item.is_jiao == 0)
                            {

                                ShowIsPaoAndIsJiao(FW.East, 13);
                            }
                            else
                            {

                                ShowIsPaoAndIsJiao(FW.East, 12);
                            }

                        }
                        else
                        {
                            if (GameInfo.cunHtypeType == 1 && item.is_pao == 5)
                            {

                                ShowIsPaoAndIsJiao(FW.East, 14);
                            }
                            else
                            {

                                ShowIsPaoAndIsJiao(FW.East, item.is_pao);
                            }

                        }
                    }


                    if ((GameInfo.FW == 1 ? GameInfo.room_peo : GameInfo.FW - 1) == item.userinfo.user_FW)
                    {
                        //左边用户
                        //需补充 

                        Show3DPai(uspInfo.mj, FW.West);
                        SaveNormalJiByCountForWest(item);
                        if (item.is_jiao == 0)
                        {
                            jipai2d.wIsBao = true;
                        }

                        if (item.is_pao == 0)
                        {
                            if (item.is_jiao == 0)
                            {

                                ShowIsPaoAndIsJiao(FW.West, 13);
                            }
                            else
                            {

                                ShowIsPaoAndIsJiao(FW.West, 12);
                            }

                        }
                        else
                        {
                            if (GameInfo.cunHtypeType == 1 && item.is_pao == 5)
                            {

                                ShowIsPaoAndIsJiao(FW.West, 14);
                            }
                            else
                            {

                                ShowIsPaoAndIsJiao(FW.West, item.is_pao);
                            }

                        }
                    }
                    if (GameInfo.room_peo == 4)//三人桌没有对家
                    {
                        if (((GameInfo.FW + 2) > GameInfo.room_peo ? (GameInfo.FW + 2) - 4 : (GameInfo.FW + 2)) == item.userinfo.user_FW)
                        {
                            //上方用户
                            //需补充 

                            Show3DPai(uspInfo.mj, FW.North);

                            SaveNormalJiByCountForNorth(item);
                            if (item.is_jiao == 0)
                            {
                                jipai2d.nIsBao = true;
                            }
                            if (item.is_pao == 0)
                            {
                                if (item.is_jiao == 0)
                                {

                                    ShowIsPaoAndIsJiao(FW.North, 13);
                                }
                                else
                                {

                                    ShowIsPaoAndIsJiao(FW.North, 12);
                                }

                            }
                            else
                            {
                                if (GameInfo.cunHtypeType == 1 && item.is_pao == 5)
                                {

                                    ShowIsPaoAndIsJiao(FW.North, 14);
                                }
                                else
                                {

                                    ShowIsPaoAndIsJiao(FW.North, item.is_pao);
                                }

                            }
                        }
                    }
                }
                else//二人桌
                {
                    if (GameInfo.FW != item.userinfo.user_FW)
                    {
                        //需补充 
                        Show3DPai(uspInfo.mj, FW.North);
                        SaveNormalJiByCountForNorth(item);
                        if (item.is_jiao == 0)
                        {
                            jipai2d.nIsBao = true;
                        }
                        if (item.is_pao == 0)
                        {
                            if (item.is_jiao == 0)
                            {

                                ShowIsPaoAndIsJiao(FW.North, 13);
                            }
                            else
                            {

                                ShowIsPaoAndIsJiao(FW.North, 12);
                            }

                        }
                        else
                        {
                            if (GameInfo.cunHtypeType == 1 && item.is_pao == 5)
                            {

                                ShowIsPaoAndIsJiao(FW.North, 14);
                            }
                            else
                            {

                                ShowIsPaoAndIsJiao(FW.North, item.is_pao);
                            }

                        }
                    }

                }

            }
        }
        try
        {

            paidui.ClearPaiDui();
            managerPai.HuPai();

        }
        catch (Exception e)
        {
            OutLog.log("清理牌堆，显示所有人的手牌error" + e);
        }
        if (!isHuangPai)
        {
            Invoke("FanJi", 1f);
            fanji2dGO.SetActive(true);
        }

        Invoke("ShowJieSuanPanel", 6);

    }
    public void ShowIsPaoAndIsJiao(FW fw, int num)
    {
        switch (fw)
        {
            case FW.East:
                _EastIsJiaoAndPao.GetComponent<Image>().sprite = _ArrayIsPaoAndIsJiao[num];
                _EastIsJiaoAndPao.GetComponent<Image>().SetNativeSize();
                _EastIsJiaoAndPao.localPosition += Vector3.up * 40;
                _EastIsJiaoAndPao.gameObject.SetActive(true);
                break;
            case FW.West:
                _WestIsJiaoAndPao.GetComponent<Image>().sprite = _ArrayIsPaoAndIsJiao[num];
                _WestIsJiaoAndPao.GetComponent<Image>().SetNativeSize();
                _WestIsJiaoAndPao.localPosition += Vector3.up * 40;
                _WestIsJiaoAndPao.gameObject.SetActive(true);
                break;
            case FW.North:
                _NorthIsJiaoAndPao.GetComponent<Image>().sprite = _ArrayIsPaoAndIsJiao[num];
                _NorthIsJiaoAndPao.GetComponent<Image>().SetNativeSize();
                _NorthIsJiaoAndPao.localPosition += Vector3.up * 40;
                _NorthIsJiaoAndPao.gameObject.SetActive(true);
                break;
            case FW.South:
                _SouthIsJiaoAndPao.GetComponent<Image>().sprite = _ArrayIsPaoAndIsJiao[num];
                _SouthIsJiaoAndPao.GetComponent<Image>().SetNativeSize();
                _SouthIsJiaoAndPao.localPosition += Vector3.up * 80;
                _SouthIsJiaoAndPao.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 将普通鸡根据数目存储到,特殊鸡存储到字典里，给自己的
    /// </summary>
    void SaveNormalJiByCountForSouth(UserJS item)
    {
        foreach (var jipais in item.jp)
        {
            //           冲锋鸡            冲锋乌骨           责任乌骨           责任幺鸡
            if (jipais.type == 2 || jipais.type == 4 || jipais.type == 8 || jipais.type == 9)
            {
                jipai2d.jisDic[jipais.type] = 1;
            }
            else
            {
                int CuCount = myCards.qiPaiGOList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int ShouCount = myCards.shouPaiGOList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int PengGangCount = myCards.pengPaiGOList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int AllCount = CuCount + ShouCount + PengGangCount;
                if (jipais.PaiHS == 11 && jipai2d.jisDic.ContainsKey(2))
                {
                    AllCount--;
                }
                if (jipais.PaiHS == 8 && jipai2d.jisDic.ContainsKey(4))
                {
                    AllCount--;
                }
                for (int i = 0; i < AllCount; i++)
                {
                    jipai2d.jisList.Add(jipais.PaiHS);
                }
            }
        }
    }
    /// <summary>
    /// 将普通鸡根据数目存储到,特殊鸡存储到字典里，给对面的
    /// </summary>
    void SaveNormalJiByCountForNorth(UserJS item)
    {

        foreach (var jipais in item.jp)
        {
            //           冲锋鸡            冲锋乌骨           责任乌骨           责任幺鸡
            if (jipais.type == 2 || jipais.type == 4 || jipais.type == 8 || jipais.type == 9)
            {
                jipai2d.jinDic[jipais.type] = 1;
            }
            else
            {
                int CuCount = managerPai.qipaiNList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int ShouCount = managerPai.shoupaiNList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int PengGangCount = managerPai.pengpaiNList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int AllCount = CuCount + ShouCount + PengGangCount;
                if (jipais.PaiHS == 11 && jipai2d.jinDic.ContainsKey(2))
                {
                    AllCount--;
                }
                if (jipais.PaiHS == 8 && jipai2d.jinDic.ContainsKey(4))
                {
                    AllCount--;
                }
                for (int i = 0; i < AllCount; i++)
                {
                    jipai2d.jinList.Add(jipais.PaiHS);
                }
            }
        }
    }
    /// <summary>
    /// 将普通鸡根据数目存储到,特殊鸡存储到字典里，给左边的
    /// </summary>
    void SaveNormalJiByCountForWest(UserJS item)
    {

        foreach (var jipais in item.jp)
        {
            //           冲锋鸡            冲锋乌骨           责任乌骨           责任幺鸡
            if (jipais.type == 2 || jipais.type == 4 || jipais.type == 8 || jipais.type == 9)
            {
                jipai2d.jiwDic[jipais.type] = 1;
            }
            else
            {
                int CuCount = managerPai.qipaiWList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int ShouCount = managerPai.shoupaiWList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int PengGangCount = managerPai.pengpaiWList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int AllCount = CuCount + ShouCount + PengGangCount;
                if (jipais.PaiHS == 11 && jipai2d.jiwDic.ContainsKey(2))
                {
                    AllCount--;
                }
                if (jipais.PaiHS == 8 && jipai2d.jiwDic.ContainsKey(4))
                {
                    AllCount--;
                }
                for (int i = 0; i < AllCount; i++)
                {
                    jipai2d.jiwList.Add(jipais.PaiHS);
                }
            }
        }
    }
    /// <summary>
    /// 将普通鸡根据数目存储到,特殊鸡存储到字典里，给右边的
    /// </summary>
    void SaveNormalJiByCountForEast(UserJS item)
    {
        foreach (var jipais in item.jp)
        {
            //           冲锋鸡            冲锋乌骨           责任乌骨           责任幺鸡
            if (jipais.type == 2 || jipais.type == 4 || jipais.type == 8 || jipais.type == 9)
            {
                jipai2d.jieDic[jipais.type] = 1;
            }
            else
            {
                int CuCount = managerPai.qipaiEList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int ShouCount = managerPai.shoupaiEList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int PengGangCount = managerPai.pengpaiEList.FindAll(u => u.GetComponent<MJ_SP>().HS == jipais.PaiHS).Count;
                int AllCount = CuCount + ShouCount + PengGangCount;
                if (jipais.PaiHS == 11 && jipai2d.jieDic.ContainsKey(2))
                {
                    AllCount--;
                }
                if (jipais.PaiHS == 8 && jipai2d.jieDic.ContainsKey(4))
                {
                    AllCount--;
                }
                for (int i = 0; i < AllCount; i++)
                {
                    jipai2d.jieList.Add(jipais.PaiHS);
                }
            }
        }
    }

    void FanJi()
    {
        jipai2d.ShowFanJi();
    }

    /// <summary>
    /// 一局结束的时候，显示所有玩家的手牌
    /// </summary>
    private void Show3DPai(IList<MaJiang> spList, FW fw)
    {
        List<MaJiang> spMJList = new List<MaJiang>();
        for (int i = 0; i < spList.Count; i++)
        {
            spMJList.Add(spList[i]);
        }


        switch (fw)
        {
            case FW.East:
                int numbere = managerPai.shoupaiEList.Count - spList.Count;
                if (numbere > 0)
                {
                    for (int i = 0; i < numbere; i++)
                    {
                        Destroy(managerPai.shoupaiEList[managerPai.shoupaiEList.Count - 1]);
                    }

                }
                break;
            case FW.West:
                int numbern = managerPai.shoupaiWList.Count - spList.Count;
                if (numbern > 0)
                {
                    for (int i = 0; i < numbern; i++)
                    {
                        Destroy(managerPai.shoupaiWList[managerPai.shoupaiWList.Count - 1]);
                    }

                }
                break;
            case FW.North:
                int numberw = managerPai.shoupaiNList.Count - spList.Count;
                if (numberw > 0)
                {
                    for (int i = 0; i < numberw; i++)
                    {
                        Destroy(managerPai.shoupaiNList[managerPai.shoupaiNList.Count - 1]);
                    }

                }
                break;
        }
        if (GameInfo.cunHType != null)
        {
            switch (fw)
            {
                case FW.East:
                    if (GameInfo.GetFW(GameInfo.cunHType.FWZ) == FW.East || DXHuFW(FW.East))
                    {
                        MaJiang mj = spMJList.First(w => w.PaiID == GameInfo.cunHType.MJ.PaiID);
                        int index = spMJList.IndexOf(spMJList.First(w => w.PaiID == GameInfo.cunHType.MJ.PaiID));

                        spMJList.RemoveAt(index);
                        spMJList.Add(mj);

                    }
                    break;
                case FW.West:
                    if (GameInfo.GetFW(GameInfo.cunHType.FWZ) == FW.West || DXHuFW(FW.West))
                    {
                        MaJiang mj = spMJList.First(w => w.PaiID == GameInfo.cunHType.MJ.PaiID);
                        int index = spMJList.IndexOf(spMJList.First(w => w.PaiID == GameInfo.cunHType.MJ.PaiID));

                        spMJList.RemoveAt(index);
                        spMJList.Add(mj);

                    }
                    break;
                case FW.North:
                    if (GameInfo.GetFW(GameInfo.cunHType.FWZ) == FW.North || DXHuFW(FW.North))
                    {
                        MaJiang mj = spMJList.First(w => w.PaiID == GameInfo.cunHType.MJ.PaiID);
                        int index = spMJList.IndexOf(spMJList.First(w => w.PaiID == GameInfo.cunHType.MJ.PaiID));

                        spMJList.RemoveAt(index);
                        spMJList.Add(mj);

                    }
                    break;
            }
        }

        for (int i = 0; i < spMJList.Count; i++)
        {
            switch (fw)
            {
                case FW.East:
                    if (i >= managerPai.shoupaiEList.Count)
                    { managerPai.MoPai(FW.East); }
                    managerPai.shoupaiEList[i].transform.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[spMJList[i].PaiHS];
                    managerPai.shoupaiEList[i].GetComponent<MJ_SP>().HS = spMJList[i].PaiHS;
                    managerPai.shoupaiEList[i].GetComponent<MJ_SP>().ID = spMJList[i].PaiID;
                    break;
                case FW.West:

                    if (i >= managerPai.shoupaiWList.Count)
                    { managerPai.MoPai(FW.West); }
                    managerPai.shoupaiWList[i].transform.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[spMJList[i].PaiHS];
                    managerPai.shoupaiWList[i].GetComponent<MJ_SP>().HS = spMJList[i].PaiHS;
                    managerPai.shoupaiWList[i].GetComponent<MJ_SP>().ID = spMJList[i].PaiID;
                    break;
                case FW.North:

                    if (i >= managerPai.shoupaiNList.Count)
                    { managerPai.MoPai(FW.North); }
                    managerPai.shoupaiNList[i].transform.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[spMJList[i].PaiHS];
                    managerPai.shoupaiNList[i].GetComponent<MJ_SP>().HS = spMJList[i].PaiHS;
                    managerPai.shoupaiNList[i].GetComponent<MJ_SP>().ID = spMJList[i].PaiID;
                    break;
            }
        }
        spMJList.Clear();
    }
    bool DXHuFW(FW fw)
    {
        bool isHave = false;
        for (int i = 0; i < GameInfo.cunHType.DXInfo.Count; i++)
        {
            if (GameInfo.GetFW(GameInfo.cunHType.DXInfo[i].DXFW) == fw)
            {
                isHave = true;
            }
        }
        return isHave;
    }
    void HideWeiJiaoPai()
    {
        _NorthIsJiaoAndPao.gameObject.SetActive(false);
        _EastIsJiaoAndPao.gameObject.SetActive(false);
        _SouthIsJiaoAndPao.gameObject.SetActive(false);
        _WestIsJiaoAndPao.gameObject.SetActive(false);
    }
    /// <summary>
    /// 结算界面显示
    /// </summary>
    public void ShowJieSuanPanel()
    {
        HideWeiJiaoPai();
        if (isAutoShow)
        {
            jiesuanPanel.SetActive(true);
        }
        if (GameInfo.cunJS != null)
        {
            //GameInfo.gameNum++;
            jiesuanPanel.transform.Find("littlesettlement").gameObject.SetActive(true);

            roomid.text = "房间ID:" + GameInfo.room_id.ToString();
            if (isShowFanji)
            {
                fanjipai.sprite = GameObject.Find("Main Camera").GetComponent<FICStartGame>().HuaseArray[fanjipaihs];

            }
            else
            {
                fanjiText.gameObject.SetActive(false);

            }


            if (!isDaJieSuan)
            {
                jiesuanPanelShowFinalButtonGO.SetActive(false);
            }
            else
            {
                jiesuanPanelContinueButtonGO.SetActive(false);
                jiesuanPanelShowFinalButtonGO.SetActive(true);

            }
            foreach (var item in GameInfo.cunJS.js)
            {
                ShowOneJieSuan(item);
            }
            foreach (var score in GameInfo.integrals)
            {
                managerGame.ShowIntegral(score.Key, score.Value);
            }
        }
        //GameReturnJS();
        isFaPaiEnd = false;
        GameInfo.cunJS = null;
        GameInfo.cunHType = null;
    }
    public void IsButtonShow()
    {
        isAutoShow = false;
    }

    /// <summary>
    /// 显示结算界面中，一个玩家的各种信息,需将一个玩家结算的预制体的背景色改为蓝色
    /// </summary>
    /// <param name="item"></param>
    private void ShowOneJieSuan(UserJS item)
    {
        int fw = item.userinfo.user_FW;//方位(wei)
        string nickeName = item.userinfo.nickname;//昵称,did
        string headimg = item.userinfo.headimg;//头像(wei)
        var jpList = item.jp;//鸡牌的集合， 需要循环取出
                             // PaiHS花色，did
                             //Count 对应分值，did
                             //type (1:幺鸡,2：冲锋鸡,3：乌骨鸡,4:冲锋乌骨,5:星期鸡,did
                             //6：本机,7：普通鸡,8:责任乌骨,9:责任幺鸡)
        var douList = item.dc;//豆的集合（杠牌后得分叫做豆） 需要循环取出 type 豆类型(1:明豆，2：暗豆)，did
                              //Count 对应分值did
        var fs = item.FS;//分数did
        var isJiao = item.is_jiao;//是否叫牌（1：叫牌，0:没叫牌）did
        var ispao = 0;
        ispao = item.is_pao;//方式(1:自摸,2：放炮,3:清一色,4:小七对，5：龙七对，6:大对子,7:接泡,8:杠上开花，9:清大对,10:清龙骑,11:清七对,)did
        var dyfs = item.dy_fs;//胡牌对应分数did

        if (GameInfo.integrals.ContainsKey(fw))
        {
            GameInfo.integrals[fw] += fs;
        }
        else
        {
            GameInfo.integrals.Add(fw, fs);
        }
        GameObject playerGo = GameObject.Instantiate(conPlayerGO, conPlayerPos, Quaternion.identity, jiesuanPanel.transform.Find("littlesettlement")) as GameObject;
        Image playerBGImage = playerGo.transform.Find("image_board_yellow").GetComponent<Image>();
        Image head = playerGo.transform.Find("image_bg_head/head").GetComponent<Image>();
        //head.sprite=
        //加载网络图片，已做
        LoadImage.Instance.LoadPicture(headimg, head);
        //GameObject fangpaoGO = playerGo.transform.Find("image_bg_head/fangpao").gameObject;
        GameObject huTypeGO = playerGo.transform.Find("image_board_yellow/image_htype").gameObject;
        Transform jipaiGrid = playerGo.transform.Find("panel_jipai/Viewport/panel_grid");
        playerGo.transform.Find("text_name").GetComponent<Text>().text = nickeName;
        for (int i = 0; i < jpList.Count; i++)
        {

            if (jpList[i].PaiHS > 0 && jpList[i].type != 11)
            {
                GameObject jiGO = GameObject.Instantiate(jipaiItemGO, jipaiGrid) as GameObject;
                //Vector3 jigopos= jiGO.GetComponent<RectTransform>().position;
                //jiGO.GetComponent<RectTransform>().position = new Vector3(jigopos.x, jigopos.y, 0);
                jiGO.transform.localScale = Vector3.one * 2;
                jiGO.transform.localPosition = Vector3.zero;
                if (jpList[i].PaiHS < 99)
                {
                    jiGO.transform.Find("sprite_ji_hs").GetComponent<Image>().sprite = HuaseArray[jpList[i].PaiHS];
                }
                if (jpList[i].PaiHS == 100)
                {
                    jiGO.transform.Find("text_ji_type").GetComponent<Text>().text = "天听";
                }
                else if (jpList[i].PaiHS == 101)
                {

                    jiGO.transform.Find("text_ji_type").GetComponent<Text>().text = "报听";
                }
                else if (jpList[i].PaiHS == 102)
                {

                    jiGO.transform.Find("text_ji_type").GetComponent<Text>().text = "天胡";
                }
                else
                {
                    jiGO.transform.Find("text_ji_type").GetComponent<Text>().text = JiPaiType(jpList[i].type);

                }
                if (jpList[i].Count > 0)
                {
                    jiGO.transform.Find("text_ji_score").GetComponent<Text>().text = "+" + jpList[i].Count;
                }
                else
                {
                    jiGO.transform.Find("text_ji_score").GetComponent<Text>().text = "" + jpList[i].Count;
                }
            }
            else if (jpList[i].PaiHS > 0)
            {
                GameObject jiGO = GameObject.Instantiate(jipaiItemGO, jipaiGrid) as GameObject;
                //Vector3 jigopos= jiGO.GetComponent<RectTransform>().position;
                //jiGO.GetComponent<RectTransform>().position = new Vector3(jigopos.x, jigopos.y, 0);
                jiGO.transform.localScale = Vector3.one * 2;
                jiGO.transform.localPosition = Vector3.zero;
                jiGO.transform.Find("text_ji_type").GetComponent<Text>().text = JiaoPaiType(jpList[i].PaiHS);
                if (jpList[i].Count > 0)
                {
                    jiGO.transform.Find("text_ji_score").GetComponent<Text>().text = "+" + jpList[i].Count;
                }
                else
                {
                    jiGO.transform.Find("text_ji_score").GetComponent<Text>().text = "" + jpList[i].Count;
                }

            }
        }

        for (int i = 0; i < douList.Count; i++)
        {
            if (douList[i].type > 0)
            {
                GameObject douGO = GameObject.Instantiate(douItemGO, jipaiGrid) as GameObject;
                douGO.transform.localScale = Vector3.one * 2;
                douGO.transform.localPosition = Vector3.zero;
                douGO.transform.Find("text_ji_type").GetComponent<Text>().text = DouType(douList[i].type);
                if (douList[i].Count > 0)
                {
                    douGO.transform.Find("text_ji_score").GetComponent<Text>().text = "+" + douList[i].Count;
                }
                else
                {
                    douGO.transform.Find("text_ji_score").GetComponent<Text>().text = "" + douList[i].Count;
                }
            }
        }
        playerGo.transform.Find("text_score").GetComponent<Text>().text = fs > 0 ? "+" + fs : fs.ToString();

        Text huType = playerGo.transform.Find("image_board_yellow/image_type").GetComponent<Text>();
        Text huScore = playerGo.transform.Find("image_board_yellow/image_type/text").GetComponent<Text>();
        if (ispao == 0)
        {
            if (isJiao == 1)
            {
                huType.text = "叫牌";
                playerBGImage.sprite = Resources.Load("Game_GYMJ/Texture/Game_UI/settlement_board_blue", typeof(Sprite)) as Sprite;
            }
            else
            {
                huType.text = "伪教排";
                playerBGImage.sprite = Resources.Load("Game_GYMJ/Texture/Game_UI/settlement_board_gray", typeof(Sprite)) as Sprite;
            }
        }
        else

        {
            if ((GameInfo.cunHtypeType == 1 || GameInfo.cunHtypeType == 3) && ispao == 5)
            {

                huType.text = "胡";
            }
            else
            {
                switch (ispao)
                {
                    case 1:
                        huType.text = "小七对";
                        break;
                    case 2:
                        huType.text = "单吊清一色";
                        break;
                    case 3:
                        huType.text = "大对子";
                        break;
                    case 4:
                        huType.text = "龙七对";
                        break;
                    case 5:
                        huType.text = "平胡";
                        break;
                    case 6:
                        huType.text = "清七对";
                        break;
                    case 7:
                        huType.text = "双清";
                        break;
                    case 8:
                        huType.text = "清大对";
                        break;
                    case 9:
                        huType.text = "清龙七";
                        break;
                    case 10:
                        huType.text = "清一色";
                        break;
                    case 11:
                        huType.text = "放炮";
                        break;
                    case 12:
                        huType.text = "叫牌";
                        break;
                    case 13:
                        huType.text = "伪教排";
                        break;
                    case 14:
                        huType.text = "胡";
                        break;

                }
            }
            if (ispao != 11)
            {
                playerBGImage.sprite = Resources.Load("Game_GYMJ/Texture/Game_UI/settlement_board_yellow", typeof(Sprite)) as Sprite;
            }
            else if (isJiao == 1)
            {
                playerBGImage.sprite = Resources.Load("Game_GYMJ/Texture/Game_UI/settlement_board_blue", typeof(Sprite)) as Sprite;
            }
            else
            {
                playerBGImage.sprite = Resources.Load("Game_GYMJ/Texture/Game_UI/settlement_board_gray", typeof(Sprite)) as Sprite;
            }

        }
        huType.SetNativeSize();
        huScore.text = dyfs == 0 ? "" : (dyfs > 0 ? "+" + dyfs : "" + dyfs);
        conPlayerPos += playerOffset;

        HuTypeOnHead(fw, huTypeGO);
        if (GameInfo.cunHType != null && GameInfo.cunHType.DXInfo.Count > 0)
        {
            for (int i = 0; i < GameInfo.cunHType.DXInfo.Count; i++)
            {
                if (fw == GameInfo.cunHType.DXInfo[i].DXFW)
                {
                    HuTypeOnHeadDUO(GameInfo.cunHType.DXInfo[i].DXFW, huTypeGO);
                }

            }
        }


        //GameInfo.cunHType = null;
    }

    /// <summary>
    /// 胡牌类型，在头顶上显示，如果需要可以更改位置
    /// </summary>
    void HuTypeOnHead(int fw, GameObject go)
    {
        if (GameInfo.cunHType != null && fw == GameInfo.cunHType.FWZ)
        {
            go.GetComponent<Image>().sprite = huTypeSpritArray[GameInfo.cunHType.type];
            go.GetComponent<Image>().SetNativeSize();
            go.SetActive(true);

        }
    }
    void HuTypeOnHeadDUO(int fw, GameObject go)
    {
        if (GameInfo.cunHType != null)
        {
            go.GetComponent<Image>().sprite = huTypeSpritArray[GameInfo.cunHType.type];
            go.GetComponent<Image>().SetNativeSize();
            go.SetActive(true);

        }
    }




    private string DouType(int type)
    {

        switch (type)
        {
            case 1:
                return "明豆";
            case 2:
                return "暗豆";
            case 3:
                return "转弯豆";
            default:
                return type.ToString();

        }

    }
    /// <summary>
    /// 加载网络图片，
    /// </summary>
    /// <param name="url"></param>
    /// <param name="headImage"></param>
    /// <returns></returns>
    //IEnumerator LoadHeadImage(string url, Image headImage)
    //{
    //    WWW www = new WWW(url);
    //    yield return www;
    //    if (www != null && string.IsNullOrEmpty(www.error))
    //    {
    //        Texture2D texture = www.texture;
    //        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    //        try
    //        {
    //            headImage.GetComponent<Image>().sprite = sprite;
    //        }
    //        catch (Exception)
    //        {
    //        }
    //    }
    //}
    /// <summary>
    /// 将int类型的type，转换成可以显示的type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string JiPaiType(int type)
    {
        //(1:幺鸡,2：冲锋鸡,3：乌骨鸡,4:冲锋乌骨,5:星期鸡,
        //6：本机,7：普通鸡,8:责任乌骨,9:责任幺鸡)
        string jiType = "";
        switch (type)
        {
            case 1:
                jiType = "幺鸡";
                break;
            case 2:
                jiType = "冲锋鸡";
                break;
            case 3:
                jiType = "乌骨鸡";
                break;
            case 4:
                jiType = "冲锋乌骨";
                break;
            case 5:
                jiType = "星期鸡";
                break;
            case 6:
                jiType = "本鸡";
                break;
            case 7:
                jiType = "普通鸡";
                break;
            case 8:
                jiType = "责任乌骨";
                break;
            case 9:
                jiType = "责任幺鸡";
                break;
            case 10:
                jiType = "连庄";
                break;
        }
        return jiType;
    }
    /// <summary>
    /// （七对:1；单调大队:2；大队:3；龙七对:4；小牌:5；清七对:6；清单调大队:7；清大对:8清龙七:9;清一色:10;）
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string JiaoPaiType(int type)
    {
        string strtype = ";";

        switch (type)
        {
            case 1:
                strtype = "小七对";
                break;
            case 2:
                strtype = "单吊清";
                break;
            case 3:
                strtype = "大对子";
                break;
            case 4:
                strtype = "龙七对";
                break;
            case 5:
                strtype = "下叫";
                break;
            case 6:
                strtype = "清七对";
                break;
            case 7:
                strtype = "双清";
                break;
            case 8:
                strtype = "清大对";
                break;
            case 9:
                strtype = "清龙七";
                break;
            case 10:
                strtype = "清一色";
                break;
        }
        return strtype;
    }
    bool isDaJieSuan = false;
    void ChangeButtonAtEnd()
    {
        if (GameInfo.returnDJS != null)
        {
            //GameInfo.cs.Closed();
            //GameInfo.cs.serverType = ServerType.ListServer;
            isDaJieSuan = true;
            GameInfo.isAllreadyStart = false;
            isGameServer = false;
            foreach (var item in myCards.shouPaiGOList)
            {
                item.GetComponent<MJ_Event>().isCanOut = false;
            }
        }
    }

    /// <summary>
    /// 显示最终大结算界面，界面需做，将每个人的userajs类型写死，只更改之后的数字,
    /// </summary>
    void ShowFinalJieSuanPanel()
    {
        if (GameInfo.returnDJS != null)
        {
            //GameInfo.isAllreadyStart = false;
            //if (GameInfo.returnDJS.State == 2)
            //{
            //    dajiesuanPanel.SetActive(true);
            //    foreach (var item in GameInfo.returnDJS.UserjsList)
            //    {
            //        ShowFinalOneJieSuan(item);
            //    }
            //}
            jiesuanPanel.transform.Find("littlesettlement").gameObject.SetActive(false);
            jiesuanPanel.transform.Find("finalsettlement").gameObject.SetActive(true);
            foreach (var item in GameInfo.returnDJS.userjs)
            {
                ShowFinalOneJieSuan(item);
            }
        }
        GameInfo.integrals.Clear();
        GameInfo.returnDJS = null;
    }

    /// <summary>
    /// 自动显示解散房间返回的大结算
    /// </summary>
    void ShowFinalJieSuanPanelAbnormal()
    {
        if (GameInfo.returnDJS != null)
        {
            if (GameInfo.returnDJS.state == 2)
            {
                jiesuanPanel.SetActive(true);
                jiesuanPanel.transform.Find("littlesettlement").gameObject.SetActive(false);
                jiesuanPanel.transform.Find("finalsettlement").gameObject.SetActive(true);
                foreach (var item in GameInfo.returnDJS.userjs)
                {
                    ShowFinalOneJieSuan(item);
                }
                GameInfo.integrals.Clear();
                GameInfo.returnDJS = null;
            }
        }

    }
    void ShowFinalOneJieSuan(UserAJS item)
    {
        //message UserAJS{
        //    required Userinfo user = 1;//用户数据
        //    required int32 zimo = 2;//自摸次数
        //    required int32 dianpao = 3;//点炮次数
        //    required int32 andou = 4;//暗斗次数
        //    required int32 Mdou = 5;//明豆次数
        //    required int32 Zdou = 6;//转弯豆次数
        //    required int32 Scare = 7;//分数
        //}
        string nickeName = item.user.nickname;//昵称,
        string headimg = item.user.headimg;//头像

        GameObject playerGo = GameObject.Instantiate(conPlayerFinalGO, conFinalPlayerPos, Quaternion.identity, jiesuanPanel.transform.Find("finalsettlement")) as GameObject;
        playerGo.transform.Find("text_name").GetComponent<Text>().text = nickeName;
        Image head = playerGo.transform.Find("image_bg_head/head").GetComponent<Image>();

        LoadImage.Instance.LoadPicture(headimg, head);

        playerGo.transform.Find("panel_finalcount/text_zi").GetComponent<Text>().text = item.zimo.ToString();
        playerGo.transform.Find("panel_finalcount/text_dian").GetComponent<Text>().text = item.dianpao.ToString();
        playerGo.transform.Find("panel_finalcount/text_an").GetComponent<Text>().text = item.andou.ToString();
        playerGo.transform.Find("panel_finalcount/text_ming").GetComponent<Text>().text = item.Mdou.ToString();
        playerGo.transform.Find("panel_finalcount/text_zhuan").GetComponent<Text>().text = item.Zdou.ToString();
        if (item.Scare > 0)
        {
            playerGo.transform.Find("panel_finalcount/text_Score").GetComponent<Text>().text = "+" + item.Scare.ToString();
        }
        else
        {
            playerGo.transform.Find("panel_finalcount/text_Score").GetComponent<Text>().text = item.Scare.ToString();
        }
        conFinalPlayerPos += playerOffset;

    }
    /// <summary>
    /// 继续游戏按钮被点击
    /// </summary>
    public void OnContinueButtonClick()
    {
       // if(manager_Time.isShimiao) manager_Time.countDownText.gameObject.SetActive(true);//继续游戏时显示计时器
        //ClearAllListsAndChanges();
        GameInfo.ClearAllListsAndChanges();
        GameInfo.isClosed = true;
        //SceneManager.LoadScene("Game_GYMJ");
        recon.OnClearObjList();
        ClearAllListsAndChanges();//本脚本的清空
        managerPai.ClearAllListsAndChanges();//paipaipai的清空
        myCards.ClearAll();//mycard的清空
        RequireContinueGame();
        //================还原骰子器默认状态======================//
        int zhuang = GameInfo.Rfw(GameInfo.zhuang);//
        GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_E").gameObject.SetActive(false);
        GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_W").gameObject.SetActive(false);
        GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_N").gameObject.SetActive(false);
        GameObject.Find("/Game_Prefabs/TABLE/touziqi_Wink/touziqi_S").gameObject.SetActive(false);

        GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_E").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 1 ? 0 : 0];
        GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_W").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 3 ? 0 : 0];
        GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_N").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 4 ? 0 : 0];
        GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_S").GetComponent<Renderer>().material.mainTexture = touziqiTexture[zhuang == 2 ? 0 : 0];

        ////=================保存游戏中音量=======================//
        //managerAudio._ConMusic.value = PlayerPrefs.GetFloat("musicVoice");
        //managerAudio._ConSound.value = PlayerPrefs.GetFloat("soundVoice");

        ////=================还原语言=============================//
        //managerGame.Mandarin.isOn = Convert.ToBoolean(PlayerPrefs.GetString("Pop"));
        //managerGame.Local.isOn = Convert.ToBoolean(PlayerPrefs.GetString("Local"));

    }
    /// <summary>
    /// 清空本脚本所涉及的所有集合，更改
    /// </summary>
    private void ClearAllListsAndChanges()
    {
        ClearJiOnHead();
        ClearQue();
        ClearShouPaiList();
        ClearPaiDui();
        ClearTeXxiao();
        ClearJiPanel();
        ClearJieSuan();
        ClearVar();
        manager_Time.ResetSZQDown();
        manager_Time.ResetShimiao();
    }
    #region 清空各种表现的方法集合


    /// <summary>
    /// 清空头顶每个人的鸡牌集合，包括庄，听
    /// </summary>
    private void ClearJiOnHead()
    {
        foreach (Transform item in eJiGridTrans)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in wJiGridTrans)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in nJiGridTrans)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in sJiGridTrans)
        {
            Destroy(item.gameObject);
        }
    }
    /// <summary>
    /// 清空定缺表现
    /// </summary>
    private void ClearQue()
    {
        eastImage.SetNativeSize();
        eastImage.transform.position = eastImageVec;
        eastImage.gameObject.SetActive(false);
        westImage.SetNativeSize();
        westImage.transform.position = westImageVec;
        westImage.gameObject.SetActive(false);
        northImage.SetNativeSize();
        northImage.transform.position = northImageVec;
        northImage.gameObject.SetActive(false);
        maskPai.southImage.SetNativeSize();
        maskPai.southImage.transform.position = maskPai.southImageVec;
        maskPai.southImage.gameObject.SetActive(false);
    }
    /// <summary>
    /// 清除手牌集合，删除所有人的手牌GO，并清空所有人手牌集合
    /// </summary>
    private void ClearShouPaiList()
    {
        foreach (GameObject item in myCards.shouPaiGOList)
        {
            Destroy(item);
        }
        foreach (var item in managerPai.shoupaiEList)
        {
            Destroy(item);
        }
        foreach (var item in managerPai.shoupaiWList)
        {
            Destroy(item);
        }
        foreach (var item in managerPai.shoupaiNList)
        {
            Destroy(item);
        }
        myCards.shouPaiGOList.Clear();
        managerPai.shoupaiEList.Clear();
        managerPai.shoupaiWList.Clear();
        managerPai.shoupaiNList.Clear();

    }
    /// <summary>
    /// 清空牌堆集合，删除所有牌堆，并清空所有牌堆
    /// </summary>
    private void ClearPaiDui()
    {
        foreach (var item in paidui._ArrayUpPais)
        {
            Destroy(item);
        }
        foreach (var item in paidui._ArrayDownPais)
        {
            Destroy(item);
        }
        Array.Clear(paidui._ArrayDownPais, 0, 53);
        Array.Clear(paidui._ArrayUpPais, 0, 53);
        paidui.isSkip = true;
        paidui.isSkipGang = true;
    }
    /// <summary>
    /// 清空桌面特效
    /// </summary>
    private void ClearTeXxiao()
    {
        foreach (Transform item in deskSkillTran)
        {
            if (item.name != "AniJi_E" && item.name != "AniJi_N" && item.name != "AniJi_S" && item.name != "AniJi_W")
            {
                Destroy(item.gameObject);
            }
        }
        foreach (Transform item in AniJi_E.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in AniJi_N.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in AniJi_W.transform)
        {
            Destroy(item.gameObject);
        }
        //foreach (Transform item in AniJi_S.transform)
        //{
        //    Destroy(item.gameObject);
        //}

    }
    /// <summary>
    /// 清空显示各家鸡牌界面
    /// </summary>
    public void ClearJiPanel()
    {
        jipai2d.ClearAll();
        jipai2d.CloseFanJiPanel();
        jipai2d.huangpaiGO.SetActive(false);
        fanji2dGO.SetActive(false);
        _EastIsJiaoAndPao.position = _EastIsJiaoAndPaoVt;
        _WestIsJiaoAndPao.position = _WestIsJiaoAndPaoVt;
        _NorthIsJiaoAndPao.position = _NorthIsJiaoAndPaoVt;
        _SouthIsJiaoAndPao.position = _SouthIsJiaoAndPaoVt;
    }
    /// <summary>
    /// 清空结算界面
    /// </summary>
    public void ClearJieSuan()
    {
        foreach (Transform item in jiesuanPanel.transform.Find("littlesettlement"))
        {
            if (item.name == "con_player(Clone)")
            {

                Destroy(item.gameObject);
            }
        }

        conPlayerPos = player1JieSuanTrans.position;
        fanjiText.gameObject.SetActive(true);
        jiesuanPanel.SetActive(false);
    }
    /// <summary>
    /// 清空本脚本的各种变量
    /// </summary>
    private void ClearVar()
    {
        paiCount = 1;
        playerTime = 10f;
        isFanJi = false;
        isBaoTing = false;
        delayQuick = 0;
        reconState = 0;
        isChu = false;
        isRecon = false;
        reconTime = 0;
        xintiao = 0;
        isNotConntction = true;
        tingCount = 0;
        isMoNotGamg = true;
        paiCountText.text = "";

        sTingNum = 0;
        eTingNum = 0;
        wTingNum = 0;
        nTingNum = 0;
        isShowFanji = true;
        isHuangPai = false;
        isNotHu = true;
        isFaPaiEnd = false;
        GameInfo.HYFw = 0;
        maskPai.maskList.Clear();

        managerPai.ResetDiamondPosition(managerPai.emptyGO.transform.position);
    }
    #endregion

    /// <summary>
    /// 小结算界面的结束游戏按钮被点击
    /// </summary>
    public void OnShowFinalButtonClick()
    {
        ShowFinalJieSuanPanel();
    }
    /// <summary>
    /// 分享战绩按钮被点击
    /// </summary>
    public void OnShareRecordButtonClick()
    {
        Application.CaptureScreenshot("Screenshot.png");
        StartCoroutine(JiePingTime(0.5f));

    }

    private IEnumerator JiePingTime(float time)
    {
        yield return new WaitForSeconds(0.5f);
        string imagePath = Application.persistentDataPath;
        imagePath = imagePath + "/Screenshot.png";

        ShareContent content = new ShareContent();
        content.SetText("setText,大家好，这是贵阳麻将");
        content.SetImagePath(imagePath);
        content.SetTitle("setTitle,贵阳麻将呦");
        content.SetComment("setComment,这是贵阳麻将呦");
        content.SetShareType(ContentType.Image);
        //content.SetShareType(ContentType.Webpage);
        // FIClogin.myShareSdk.ShowPlatformList(null, content, 100, 100);
        FIClogin.myShareSdk.ShowShareContentEditor(PlatformType.WeChat, content);
    }

    /// <summary>
    /// 返回大厅按钮被点击
    /// </summary>
    public void OnBackToHallButtonClick()
    {
        // GameInfo.isYuanQue = 0;
        GameInfo.roomRlue = null;
        GameInfo.ClearAllListsAndChanges();
        manager_Time.isShimiao = false;
        GameInfo.gameNum = 1;
        GameInfo.FW = 0;
        GameInfo.IsSetRoomInfo = false;
        GameInfo.isClosed = true;
        SceneManager.LoadScene("Scene_Hall");

        //=================还原游戏中音量=======================//
        managerAudio._ConMusic.value = PlayerPrefs.GetFloat("musicVoice");
        managerAudio._ConSound.value = PlayerPrefs.GetFloat("soundVoice");

        //=================还原语言=============================//
        managerGame.Mandarin.isOn = Convert.ToBoolean(PlayerPrefs.GetString("Pop"));
        managerGame.Local.isOn = Convert.ToBoolean(PlayerPrefs.GetString("Local"));


    }
    /// <summary>
    /// 分享回调函数
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    public static void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {   //成功  
        if (state == ResponseState.Success)
        {
            //OutLog.PrintLog("authorize success !");
            //OutLog.PrintLog(MiniJSON.jsonEncode(result));
            print("share successfully - share resule :");
            print(MiniJSON.jsonEncode(result));
            objname = "分析成功：" + MiniJSON.jsonEncode(result);

        }
        //失败  
        else if (state == ResponseState.Fail)
        {
            OutLog.PrintLog("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
        }
        //关闭  
        else if (state == ResponseState.Cancel)
        {
            OutLog.PrintLog("cancel !");
        }
    }



    /// <summary>
    /// 关闭线程
    /// </summary>
    private void OnApplicationQuit()
    {
        GameInfo.cs.Closed();
        GameInfo.cs.myThread.Abort();
    }

    /// <summary>
    ///判断服务器下发的快捷语音，并播放
    ///
    /// </summary>
    void ReturnQuickVoice()
    {
        //================================== 快捷语音文字提醒 ==========================================//
        var quickVoiceW = transform.Find("/Game_UI/Fixed_UI/Heads/Head_west/Text");
        var quickVoiceN = transform.Find("/Game_UI/Fixed_UI/Heads/Head_north/Text");
        var quickVoiceS = transform.Find("/Game_UI/Fixed_UI/Heads/Head_south/Text");
        var quickVoiceE = transform.Find("/Game_UI/Fixed_UI/Heads/Head_east/Text");

        if (GameInfo.returnVoice != null)// && Time.realtimeSinceStartup - delayQuick > 5)
        //transform.Find("/Game_UI/PopUp_UI/Voice").gameObject.SetActive(false);
        {

            if (GameInfo.returnVoice.VoiceNumber >= 1 && GameInfo.returnVoice.VoiceNumber <= 3)
            {
                String str = GameInfo.returnVoice.FW + "," + GameInfo.returnVoice.VoiceNumber + "," + GameInfo.returnVoice.FWT;
                managerGame.ReturnProp(str);
            }

            switch (GameInfo.GetFW(GameInfo.returnVoice.FW))
            {
                case FW.East:
                    //if (Time.realtimeSinceStartup - delayQuick > 5)
                   // {
                   //     delayQuick = Time.realtimeSinceStartup;
                        //managerAudio.quickVoice(GameInfo.returnVoice.VoiceNumber);
                        FICAudioPlay._instance.PlayShorVoice(GameInfo.MJplayers[GameInfo.returnVoice.FW].sex, GameInfo.returnVoice.VoiceNumber);

                  //  }
                    break;
                case FW.West:
                  //  if (Time.realtimeSinceStartup - delayQuick > 5)
                  //  {
                  //      delayQuick = Time.realtimeSinceStartup;
                        //managerAudio.quickVoice(GameInfo.returnVoice.VoiceNumber); 
                        FICAudioPlay._instance.PlayShorVoice(GameInfo.MJplayers[GameInfo.returnVoice.FW].sex, GameInfo.returnVoice.VoiceNumber);

                  //  }
                    break;
                case FW.North:
                   // if (Time.realtimeSinceStartup - delayQuick > 5)
                  //  {
                  //      delayQuick = Time.realtimeSinceStartup;
                        //managerAudio.quickVoice(GameInfo.returnVoice.VoiceNumber);
                        FICAudioPlay._instance.PlayShorVoice(GameInfo.MJplayers[GameInfo.returnVoice.FW].sex, GameInfo.returnVoice.VoiceNumber);

                  //  }
                    break;
                case FW.South:
                   // if (Time.realtimeSinceStartup - delayQuick > 5)
                   // {
                   //     delayQuick = Time.realtimeSinceStartup;
                        //managerAudio.quickVoice(GameInfo.returnVoice.VoiceNumber);
                        FICAudioPlay._instance.PlayShorVoice(GameInfo.MJplayers[GameInfo.returnVoice.FW].sex, GameInfo.returnVoice.VoiceNumber);

                  //  }
                    break;
                default:
                    break;
            }
            //managerAudio.quickVoice(GameInfo.returnVoice.VoiceNumber);
            GameInfo.returnVoice = null;
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!GameInfo.isAllreadyStart) return;
        SendConnectionStatus sendConnectionStatus = new SendConnectionStatus();
        sendConnectionStatus.openid = GameInfo.OpenID;
        sendConnectionStatus.ConnectionStatus = focus ? 1 : 0;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendConnectionStatus);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 8005, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }
    //private void OnApplicationPause(bool pause)
    //{
    //    SendConnectionStatus sendConnectionStatus = SendConnectionStatus.CreateBuilder()
    //           .SetOpenid(GameInfo.OpenID)
    //           .SetConnectionStatus(pause ? 1 : 0)
    //               .Build();
    //    byte[] body = sendConnectionStatus.ToByteArray();
    //    byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 8005, body.Length, 0, body);
    //    GameInfo.cs.Send(data);
    //}
}


