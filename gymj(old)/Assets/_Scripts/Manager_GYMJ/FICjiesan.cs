using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MJBLL.common;
using UnityEngine.SceneManagement;
using System;
using DNL;
using System.Text;

public class FICjiesan : MonoBehaviour
{

    /*解散房间功能分析，以及协议作用

      1.房主创建房间后，可以选择解散，牌局未开始则，直接解散
      2.其他人加入房间后，可以选择退出，牌局未开始，则直接退出
      3.牌局开始之后，房主以及所有人都是解散房间

      */

    //===================类的调用===================
    private FICMJPlaying mjPlaying;
    //================设置按钮点击==========================
    private Button settingButton;
    //================PopUp_UI父物体==========================
    private Transform PopUpUITrans;
    //=============申请解散或者退出按钮====================
    public Button JieSanOrQuitButton;
    private Text JieSanOrQuitText;
    //=============申请解散房间，请投票的panel====================
    private Transform JieSanPanelTrans;
    private Text JieSanTittle;

    private Transform firstPeoTrans;
    private Image firstHeadImage;
    private Image firstStateImage;
    private Text firstNameText;

    private Transform secondPeoTrans;
    private Image secondHeadImage;
    private Image secondStateImage;
    private Text secondNameText;

    private Transform thirdPeoTrans;
    private Image thirdHeadImage;
    private Image thirdStateImage;
    private Text thirdNameText;

    private Button agreeButton;
    private Button refuseButton;
    private Text agreeTimeText;
    //===================图片定义==================
    private Sprite agreeSprit;
    private Sprite refuseSprit;
    //=============警告界面，是否真的解散房间====================
    private Transform warningJieSanTrans;
    private Button yesJieSanButton;
    private Button noJieSanButton;
    //=============申请解散或者退出按钮====================
    private Transform fanduiPanelTrans;
    private Text fanduiNamesText;
    private string fanduiNamesStr;
    //============定义中间或者公共变量===========
    private string imageURL = null;
    private int voteCount = 1;
    private int refuseCont = 0;
    private int jsFw = 0;
    private Manager_Audio managerAudio; ///
    private Manager_Game managerGame;
    private Mannager_Time managerTime;

    private void Start()
    {
        //===================脚本单例===============================//
        managerGame = gameObject.GetComponent<Manager_Game>();
        managerAudio = gameObject.GetComponent<Manager_Audio>();
        managerTime = gameObject.GetComponent<Mannager_Time>();
        //===================类的调用===================
        mjPlaying = gameObject.GetComponent<FICMJPlaying>();
           //================设置按钮点击==========================
           settingButton = transform.Find("/Game_UI/Fixed_UI/But_SheZhi").GetComponent<Button>();
        //================PopUp_UI父物体==========================
        PopUpUITrans = transform.Find("/Game_UI/PopUp_UI");
        JieSanOrQuitButton = PopUpUITrans.Find("SheZhi/Quit").GetComponent<Button>();
        JieSanOrQuitText = JieSanOrQuitButton.transform.Find("Text").GetComponent<Text>();

        //=============申请解散房间，请投票的panel====================
        JieSanPanelTrans = PopUpUITrans.Find("SQ_jiesan");
        JieSanTittle = JieSanPanelTrans.Find("Text").GetComponent<Text>();

        firstPeoTrans = JieSanPanelTrans.Find("firstPeo");
        firstHeadImage = firstPeoTrans.Find("head_2").GetComponent<Image>();
        firstStateImage = firstPeoTrans.Find("state_2").GetComponent<Image>();
        firstNameText = firstPeoTrans.Find("nickname_2").GetComponent<Text>();

        secondPeoTrans= JieSanPanelTrans.Find("secondPeo");
        secondHeadImage = secondPeoTrans.Find("head_3").GetComponent<Image>();
        secondStateImage = secondPeoTrans.Find("state_3").GetComponent<Image>();
        secondNameText = secondPeoTrans.Find("nickname_3").GetComponent<Text>();

        thirdPeoTrans= JieSanPanelTrans.Find("thirdPeo");
        thirdHeadImage = thirdPeoTrans.Find("head_4").GetComponent<Image>();
        thirdStateImage = thirdPeoTrans.Find("state_4").GetComponent<Image>();
        thirdNameText = thirdPeoTrans.Find("nickname_4").GetComponent<Text>();

        agreeButton = JieSanPanelTrans.Find("Agree").GetComponent<Button>();
        //agreeTimeText = agreeButton.transform.Find("time_60").GetComponent<Text>();
        refuseButton = JieSanPanelTrans.Find("Refuse").GetComponent<Button>();


        //=============警告界面，是否真的解散房间====================
        warningJieSanTrans = PopUpUITrans.Find("DD_jiesan");
        yesJieSanButton = warningJieSanTrans.Find("Yes_Btn").GetComponent<Button>();
        noJieSanButton = warningJieSanTrans.Find("No_Btn").GetComponent<Button>();

        //=============申请解散或者退出按钮====================
        fanduiPanelTrans = PopUpUITrans.Find("jiesan_SB");
        fanduiNamesText = fanduiPanelTrans.Find("Text").GetComponent<Text>();
        fanduiNamesStr = null;
        //==============图片赋值=====================
        agreeSprit = Resources.Load("Game_GYMJ/Texture/Game_UI/popup_head_yes",typeof(Sprite)) as Sprite;
        refuseSprit = Resources.Load("Game_GYMJ/Texture/Game_UI/popup_head_no", typeof(Sprite)) as Sprite;
        //==================按钮事件监听添加===============
        settingButton.onClick.AddListener(OnSettingButtonClick);
        JieSanOrQuitButton.onClick.AddListener(OnJieSanOrQuitButtonClick);

        //yesJieSanButton.onClick.AddListener(WarningYesButtonClick);
        //noJieSanButton.onClick.AddListener(WarningNoButtonClick);

        agreeButton.onClick.AddListener(SendAgreeMessage);
        refuseButton.onClick.AddListener(SendRefuseMessage);
        
    }


    private void Update()
    {
        ReturnJSMsg();
        ReturnJSByOnew();
        ReturnALLIdea();
        ReturnRemoveUser();
        ReturnDFailure();
    }
    private void ReturnDFailure()
    {
        if (GameInfo.returnDisbandedFailure != null && GameInfo.returnDisbandedFailure.status == 1)
        {
            FICWaringPanel._instance.Show("申请解散时间间隔不足5秒");
            GameInfo.returnDisbandedFailure = null;
        }
    }
    /// <summary>
    /// 设置按钮点击之后，面板已控制设置界面显示，这里只是判断，是应该显示退出还是显示解散
    /// 功能：
    /// 1.如果是房主，则显示解散房间
    /// 2，如果不是房主，牌局未开始之前显示退出房间，牌局开始之后，显示解散房间
    /// </summary>
    private void OnSettingButtonClick()
    {
        if (GameInfo.FW != 1 && !GameInfo.isAllreadyStart)
        {
            JieSanOrQuitText.text = "退出房间";
        }
        else
        {
            JieSanOrQuitText.text = "解散房间";
        }
    }
    /// <summary>
    /// 解散或者退出按钮点击
    /// 如果是解散房间，弹出警告界面，如果点击是，则发送请求解散房间
    /// 如果是退出放假，则发送请求退出房间
    /// </summary>
    private void OnJieSanOrQuitButtonClick()
    {
        Debug.Log("界山犯贱");
        if (JieSanOrQuitText.text.Equals("解散房间"))
        {
            JieSanOrQuitButton.transform.parent.gameObject.SetActive(false);
            WarningYesButtonClick();
            //=================保存游戏中音量=======================//
            managerAudio._ConMusic.value = PlayerPrefs.GetFloat("musicVoice");
            managerAudio._ConSound.value = PlayerPrefs.GetFloat("soundVoice");

            //=================还原语言=============================//
            managerGame.Mandarin.isOn = Convert.ToBoolean(PlayerPrefs.GetString("Pop"));
            managerGame.Local.isOn = Convert.ToBoolean(PlayerPrefs.GetString("Local"));
            Debug.Log("界山犯贱1");
        }
        else
        {
            SendQuitMessage();
            Debug.Log("界山犯贱2");
        }
    }
    /// <summary>
    /// 警告面板，是按钮点击，发送请求解散房间消息
    /// </summary>
    private void WarningYesButtonClick()
    {
        //点击警告面板的是按钮，发送解散房间消息
        SendJieSanMessage();
        warningJieSanTrans.gameObject.SetActive(false);
        GameInfo.jieSanOpenid = GameInfo.OpenID;
    }
    /// <summary>
    /// 警告面板，否按钮点击，直接关闭弹出框
    /// </summary>
    private void WarningNoButtonClick()
    {
        SendAgreeMessage();
        warningJieSanTrans.gameObject.SetActive(false);
    }

    /// <summary>
    /// 发送请求解散房间
    /// </summary>
    public void SendJieSanMessage()
    {
        //SendJS sendjs = SendJS.CreateBuilder()
        //     .SetOpenid(GameInfo.OpenID)
        //     .SetRoomid(GameInfo.room_id)
        //     .SetUnionid(GameInfo.unionid)
        //     .Build();
        //byte[] body = sendjs.ToByteArray();

        Debug.Log("界山房间发送");
        SendJS sendjs = new SendJS();
        sendjs.openid = GameInfo.OpenID;
        sendjs.roomid = GameInfo.room_id;
        sendjs.unionid = GameInfo.unionid;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendjs);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5003, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }
    /// <summary>
    ///发送请求退出房间 
    /// </summary>
    public void SendQuitMessage()
    {

        //SendRemove quitRequire = SendRemove.CreateBuilder()
        // .SetOpenid(GameInfo.OpenID)
        // .SetRoomID(GameInfo.room_id)
        // .SetUnionid(GameInfo.unionid)
        // .Build();
        //byte[] body = quitRequire.ToByteArray();
        Debug.Log("点击尖山房间");
        SendRemove quitRequire = new SendRemove();
        quitRequire.openid = GameInfo.OpenID;
        quitRequire.RoomID = GameInfo.room_id;
        quitRequire.unionid = GameInfo.unionid;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(quitRequire);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5008, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }



    /// <summary>
    /// 发送同意解散房间
    /// </summary>
    public void SendAgreeMessage()
    {
        agreeButton.gameObject.SetActive(false);
        refuseButton.gameObject.SetActive(false);
        //SendJSIdea jsidea = SendJSIdea.CreateBuilder()
        // .SetOpenid(GameInfo.OpenID)
        // .SetRoomid(GameInfo.room_id)
        // .SetState(1)
        // .Build();
        //byte[] body = jsidea.ToByteArray();
        SendJSIdea jsidea = new SendJSIdea();
        jsidea.openid = GameInfo.OpenID;
        jsidea.roomid = GameInfo.room_id;
        jsidea.state = 1;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(jsidea);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5005, body.Length, 0, body);
        GameInfo.cs.Send(data);

    }
    /// <summary>
    /// 发送拒绝解散房间
    /// </summary>
    private void SendRefuseMessage()
    {
        agreeButton.gameObject.SetActive(false);
        refuseButton.gameObject.SetActive(false);
        //  SendJSIdea jsidea = SendJSIdea.CreateBuilder()
        //.SetOpenid(GameInfo.OpenID)
        //.SetRoomid(GameInfo.room_id)
        //.SetState(0)
        //.Build();
        //  byte[] body = jsidea.ToByteArray();

        SendJSIdea jsidea = new SendJSIdea();
        jsidea.openid = GameInfo.OpenID;
        jsidea.roomid = GameInfo.room_id;
        jsidea.state = 0;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(jsidea);

        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5005, body.Length, 0, body);
        GameInfo.cs.Send(data);

    }
    /// <summary>
    /// 返回请求解散房间信息
    /// </summary>
    private void ReturnJSMsg()
    {
        if (GameInfo.returnJSMsg!=null)
        {
            managerTime.ResetJSDown();
            JieSanPanelTrans.gameObject.SetActive(true);
            string jsName = null;
            jsName = GameInfo.returnJSMsg.NickName;
            if (jsName.Length>5)
            {
                jsName = jsName.Substring(0, 5)+"...";
            }
            JieSanTittle.text = "玩家"+jsName+"发起解散房间投票请作出投票选择";

            if (GameInfo.jieSanOpenid != null)
            {
                InitJieSanPanelOnMyClick();
                agreeButton.gameObject.SetActive(false);
                refuseButton.gameObject.SetActive(false);
            }
            else
            {
                InitJieSanPanelForOtherClick();
            }
            jsFw = GameInfo.returnJSMsg.fw;
            GameInfo.returnJSMsg = null;
        }
    }
    /// <summary>
    /// 初始化解散panel，个人头像和昵称,
    /// </summary>
    private void InitJieSanPanelForOtherClick()
    {
        switch (GameInfo.room_peo)
        {
            case 2:
                firstHeadImage.sprite = mjPlaying.backHeadImage.sprite;
                firstNameText.text = mjPlaying.MeNickNameText.text;
                firstStateImage.gameObject.SetActive(false);
                secondPeoTrans.gameObject.SetActive(false);
                thirdPeoTrans.gameObject.SetActive(false);
                break;
            case 3:

                firstHeadImage.sprite = mjPlaying.backHeadImage.sprite;
                firstNameText.text = mjPlaying.MeNickNameText.text;
                firstStateImage.gameObject.SetActive(false);

                if( GameInfo.GetFW(GameInfo.returnJSMsg.fw) != FW.West)
                {
                    secondHeadImage.sprite = mjPlaying.leftHeadImage.sprite;
                    secondNameText.text = mjPlaying.leftNickNameText.text;
                }
                else
                {
                    secondHeadImage.sprite = mjPlaying.rightHeadImage.sprite;
                    secondNameText.text = mjPlaying.rightNickNameText.text;
                }
                secondStateImage.gameObject.SetActive(false);

                thirdPeoTrans.gameObject.SetActive(false);
                break;
            case 4:
                firstHeadImage.sprite = mjPlaying.backHeadImage.sprite;
                firstNameText.text = mjPlaying.MeNickNameText.text;
                firstStateImage.gameObject.SetActive(false);
                switch (GameInfo.GetFW(GameInfo.returnJSMsg.fw))
                {
                    case FW.East:
                        secondHeadImage.sprite = mjPlaying.frontHeadImage.sprite;
                        secondNameText.text = mjPlaying.frontNickNameText.text;
                        secondStateImage.gameObject.SetActive(false);

                        thirdHeadImage.sprite = mjPlaying.leftHeadImage.sprite;
                        thirdNameText.text = mjPlaying.leftNickNameText.text;
                        thirdStateImage.gameObject.SetActive(false);
                        break;
                    case FW.West:
                        secondHeadImage.sprite = mjPlaying.frontHeadImage.sprite;
                        secondNameText.text = mjPlaying.frontNickNameText.text;
                        secondStateImage.gameObject.SetActive(false);

                        thirdHeadImage.sprite = mjPlaying.rightHeadImage.sprite;
                        thirdNameText.text = mjPlaying.rightNickNameText.text;
                        thirdStateImage.gameObject.SetActive(false);
                        break;
                    case FW.North:
                        secondHeadImage.sprite = mjPlaying.rightHeadImage.sprite;
                        secondNameText.text = mjPlaying.rightNickNameText.text;
                        secondStateImage.gameObject.SetActive(false);

                        thirdHeadImage.sprite = mjPlaying.leftHeadImage.sprite;
                        thirdNameText.text = mjPlaying.leftNickNameText.text;
                        thirdStateImage.gameObject.SetActive(false);
                        break;
                }
                break;
        }
        refuseButton.gameObject.SetActive(true);
        agreeButton.gameObject.SetActive(true);
    }

    private void InitJieSanPanelOnMyClick()
    {
        switch (GameInfo.room_peo)
        {
            case 2:
                firstHeadImage.sprite = mjPlaying.frontHeadImage.sprite;
                firstNameText.text = mjPlaying.frontNickNameText.text;
                firstStateImage.gameObject.SetActive(false);
                secondPeoTrans.gameObject.SetActive(false);
                thirdPeoTrans.gameObject.SetActive(false);
                break;
            case 3:

                firstHeadImage.sprite = mjPlaying.rightHeadImage.sprite;
                firstNameText.text = mjPlaying.rightNickNameText.text;
                firstStateImage.gameObject.SetActive(false);

                secondHeadImage.sprite = mjPlaying.leftHeadImage.sprite;
                secondNameText.text = mjPlaying.leftNickNameText.text;
                secondStateImage.gameObject.SetActive(false);

                thirdPeoTrans.gameObject.SetActive(false);
                break;
            case 4:
                firstHeadImage.sprite = mjPlaying.rightHeadImage.sprite;
                firstNameText.text = mjPlaying.rightNickNameText.text;
                firstStateImage.gameObject.SetActive(false);

                secondHeadImage.sprite = mjPlaying.frontHeadImage.sprite;
                secondNameText.text = mjPlaying.frontNickNameText.text;
                secondStateImage.gameObject.SetActive(false);

                thirdHeadImage.sprite = mjPlaying.leftHeadImage.sprite;
                thirdNameText.text = mjPlaying.leftNickNameText.text;
                thirdStateImage.gameObject.SetActive(false);
                break;
        }
    }
    /// <summary>
    /// 返回单个用户同意信息
    /// </summary>
    private void ReturnJSByOnew()
    {
        if (GameInfo.returnJSByOnew!=null&&GameInfo.jieSanOpenid!=null)
        {
            voteCount++;
            Debug.Log(GameInfo.returnJSByOnew.fw + "|" + GameInfo.returnJSByOnew.NickName + "|" + GameInfo.returnJSByOnew.state);
            switch (GameInfo.room_peo)
            {
                case 2:
                    if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["front"])
                    {
                        firstStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            firstStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            firstStateImage.sprite = refuseSprit;
                            fanduiNamesStr += firstNameText.text;
                            refuseCont++;
                        }
                    }
                    break;
                case 3:
                    if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["right"])
                    {
                        firstStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            firstStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            firstStateImage.sprite = refuseSprit;
                            fanduiNamesStr += firstNameText.text + ";";
                            refuseCont++;
                        }
                    }
                    else if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["left"])
                    {
                        secondStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            secondStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            secondStateImage.sprite = refuseSprit;
                            fanduiNamesStr += secondNameText.text ;
                            refuseCont++;
                        }
                    }
                    break;
                case 4:
                    if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["right"])
                    {
                        firstStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            firstStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            firstStateImage.sprite = refuseSprit;
                            fanduiNamesStr += firstNameText.text + ";";
                            refuseCont++;
                        }
                    }
                    else if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["front"])
                    {
                        secondStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            secondStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            secondStateImage.sprite = refuseSprit;
                            fanduiNamesStr += secondNameText.text + ";";
                            refuseCont++;
                        }
                    }
                    else if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["left"])
                    {
                        thirdStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            thirdStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            thirdStateImage.sprite = refuseSprit;
                            fanduiNamesStr += thirdNameText.text ;
                            refuseCont++;
                        }
                    }
                    break;

            }

            GameInfo.returnJSByOnew = null;

            if (voteCount == GameInfo.room_peo)
            {
                if (refuseCont > 0)
                {
                    HideVotePanelAndShowRefusePanel();
                }
            }


        }
        if (GameInfo.returnJSByOnew != null && GameInfo.jieSanOpenid == null)
        {
            voteCount++;
            Debug.Log(GameInfo.returnJSByOnew.fw + "|" + GameInfo.returnJSByOnew.NickName + "|" + GameInfo.returnJSByOnew.state);
            switch (GameInfo.room_peo)
            {
                case 2:
                    if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["back"])
                    {
                        firstStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            firstStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            firstStateImage.sprite = refuseSprit;
                            fanduiNamesStr += firstNameText.text;
                            refuseCont++;
                        }
                    }
                    break;
                case 3:
                    if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["back"])
                    {
                        firstStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            firstStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            firstStateImage.sprite = refuseSprit;
                            fanduiNamesStr += firstNameText.text + ";";
                            refuseCont++;
                        }
                    }
                    else if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["left"]||GameInfo.returnJSByOnew.fw==GameInfo.MJVoteDic["right"])
                    {
                        secondStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            secondStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            secondStateImage.sprite = refuseSprit;
                            fanduiNamesStr += secondNameText.text;
                            refuseCont++;
                        }
                    }
                    break;
                case 4:
                    if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["back"])
                    {
                        firstStateImage.gameObject.SetActive(true);
                        if (GameInfo.returnJSByOnew.state == 1)
                        {
                            firstStateImage.sprite = agreeSprit;
                        }
                        else
                        {
                            firstStateImage.sprite = refuseSprit;
                            fanduiNamesStr += firstNameText.text + ";";
                            refuseCont++;
                        }
                    }
                    else 
                    {
                        switch (GameInfo.GetFW(jsFw))
                        {
                            case FW.East:
                                if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["front"])
                                {
                                    secondStateImage.gameObject.SetActive(true);
                                    if (GameInfo.returnJSByOnew.state == 1)
                                    {
                                        secondStateImage.sprite = agreeSprit;
                                    }
                                    else
                                    {
                                        secondStateImage.sprite = refuseSprit;
                                        fanduiNamesStr += secondNameText.text + ";";
                                        refuseCont++;
                                    }

                                }
                                else if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["left"])
                                {
                                    thirdStateImage.gameObject.SetActive(true);
                                    if (GameInfo.returnJSByOnew.state == 1)
                                    {
                                        thirdStateImage.sprite = agreeSprit;
                                    }
                                    else
                                    {
                                        thirdStateImage.sprite = refuseSprit;
                                        fanduiNamesStr += thirdNameText.text;
                                        refuseCont++;
                                    }
                                }
                                break;
                            case FW.West:
                                if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["front"])
                                {
                                    secondStateImage.gameObject.SetActive(true);
                                    if (GameInfo.returnJSByOnew.state == 1)
                                    {
                                        secondStateImage.sprite = agreeSprit;
                                    }
                                    else
                                    {
                                        secondStateImage.sprite = refuseSprit;
                                        fanduiNamesStr += secondNameText.text + ";";
                                        refuseCont++;
                                    }

                                }
                                else if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["right"])
                                {
                                    thirdStateImage.gameObject.SetActive(true);
                                    if (GameInfo.returnJSByOnew.state == 1)
                                    {
                                        thirdStateImage.sprite = agreeSprit;
                                    }
                                    else
                                    {
                                        thirdStateImage.sprite = refuseSprit;
                                        fanduiNamesStr += thirdNameText.text ;
                                        refuseCont++;
                                    }
                                }
                                break;
                            case FW.North:
                                if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["right"])
                                {
                                    secondStateImage.gameObject.SetActive(true);
                                    if (GameInfo.returnJSByOnew.state == 1)
                                    {
                                        secondStateImage.sprite = agreeSprit;
                                    }
                                    else
                                    {
                                        secondStateImage.sprite = refuseSprit;
                                        fanduiNamesStr += secondNameText.text + ";";
                                        refuseCont++;
                                    }

                                }
                                else if (GameInfo.returnJSByOnew.fw == GameInfo.MJVoteDic["left"])
                                {
                                    thirdStateImage.gameObject.SetActive(true);
                                    if (GameInfo.returnJSByOnew.state == 1)
                                    {
                                        thirdStateImage.sprite = agreeSprit;
                                    }
                                    else
                                    {
                                        thirdStateImage.sprite = refuseSprit;
                                        fanduiNamesStr += thirdNameText.text ;
                                        refuseCont++;
                                    }
                                }
                                break;
                      
                        }
                        
                       
                    }
                    break;

            }
            GameInfo.returnJSByOnew = null;

            if (voteCount == GameInfo.room_peo)
            {
                if (refuseCont > 0)
                {
                    HideVotePanelAndShowRefusePanel();
                }
            }
        }
      

    }
    /// <summary>
    /// 当投票人数等于房间人数的时候，投票环节有人拒绝，那么，隐藏投票界面，显示继续游戏界面
    /// </summary>
    private void HideVotePanelAndShowRefusePanel()
    {
        JieSanPanelTrans.gameObject.SetActive(false);

        fanduiPanelTrans.gameObject.SetActive(true);

        fanduiNamesText.text = "玩家" + fanduiNamesStr + "反对\n解散房间失败\r\n请继续游戏";
        GameInfo.jieSanOpenid =null;
        fanduiNamesStr = null;
        refuseCont = 0;
        voteCount = 1;
    }
    /// <summary>
    /// 返回集体消息是否解散房间
    /// state=1,则同意解散房间，跳转到大厅界面，要不要等待时间？
    /// state=2，则弹出继续游戏弹框
    /// </summary>
    private void ReturnALLIdea()
    {
        if (GameInfo.returnALLIdea!=null)
        {
            if (GameInfo.returnALLIdea.state==1)
            {
                if (!GameInfo.isAllreadyStart && !GameInfo.recon && !GameInfo.isStartGame)
                {
                    GameInfo.roomRlue = null;
                    GameInfo.FW = 0;
                    GameInfo.gameNum = 1;
                    GameInfo.IsSetRoomInfo = false;
                    SceneManager.LoadScene("Scene_Hall");
                    GameInfo.ClearAllListsAndChanges();

                }
                //GameInfo.cs.Closed();
                //GameInfo.cs.serverType = ServerType.ListServer;
                JieSanPanelTrans.gameObject.SetActive(false);
            }
            else
            {
                fanduiPanelTrans.gameObject.SetActive(true);
                
                voteCount = 1;
            }
            GameInfo.returnALLIdea = null;
        }
    }

    /// <summary>
    /// 返回退出玩家
    /// 现判断返回的状态，如果0，已经开始游戏，需要提醒？
    /// 如果1：退出成功，则需判断openid是自己还是别人
    ///                     如果是自己，则直接跳转，清空玩家信息
    ///                     如果是别人，直接清空玩家信息，将显示信息清除
    /// </summary>
    private void ReturnRemoveUser()
    {
        if (GameInfo.returnRemoveUser!=null)
        {
           if(GameInfo.returnRemoveUser.status==0)
            {//返回已经开始游戏了，需要提醒？

            }
            else
            {
                //退出成功，需判断是不是自己
                if (GameInfo.returnRemoveUser.openid==GameInfo.OpenID)
                {
                    GameInfo.MJplayers.Clear();
                    GameInfo.ClearAllListsAndChanges();
                    SceneManager.LoadScene("Scene_Hall");
                }
                else
                {
                    GameInfo.MJplayers.Remove(GameInfo.MJplayersWhoQuit[GameInfo.returnRemoveUser.openid]);
                    FW fw = GameInfo.GetFW(GameInfo.MJplayersWhoQuit[GameInfo.returnRemoveUser.openid]);

                    switch (fw)
                    {
                        case FW.East:
                            mjPlaying.rightHeadFGO.SetActive(false);
                            break;
                        case FW.West:

                            mjPlaying.leftHeadFGO.SetActive(false);
                            break;
                        case FW.North:

                            mjPlaying.frontHeadFGO.SetActive(false);
                            break;
                    }
                }

            }

            GameInfo.returnRemoveUser = null;
        }
    }

}
