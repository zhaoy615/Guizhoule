using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Desktop_UI : Singleton<Desktop_UI> {
    //==========================================================================================
    public Transform bottomCards_Pel;
    private Image cardOne;
    private Image cardTwo;
    private Image cardThree;
    //==========================================================================================
    public Transform No_Ad_Pel;
    private Dictionary<int,Text> outCardsCount;  // 大王   小王    2   A   K   Q   J   10   9   8   7   6   5   4   3
    //==========================================================================================
    public Transform Heads_Pel;
    private Transform Head_west;
    private Image bottom_west;
    private Text userName_west;
    private Text integral_west;
    private Image landlord_west;
    private Transform Head_south;
    private Image bottom_south;
    private Text userName_south;
    private Text integral_south;
    private Image landlord_south;
    private Transform Head_east;
    private Image bottom_east;
    private Text userName_east;
    private Text integral_east;
    private Image landlord_east;
    //==========================================================================================
    public Transform PaiJuInfo_Pel;
    private Text time;
    private Text mulriple;
    private Text signal;
    private Text juNum;
    //==========================================================================================
    public Transform GameOperationBtns;
    private Transform GameOperation_south;
    private Button[] OperationBtns_south;//0:不出 1:提示 2:出牌 3:一分 4:两分 5:三分 6:不叫(分) 7:叫地主 8:不叫 9:抢地主 10:不抢 11:加倍 12:不加倍
    private Transform countDown_south;
    private Transform GameOperation_west;
    private Transform countDown_west;
    private Transform GameOperation_east;
    private Transform countDown_east;
    //==========================================================================================
    private Transform WestPot;
    private Transform EastPot;
    private Transform SouthPot;
    //==========================================================================================
    private Sprite[] bottomCardsList = new Sprite[54];

    private new void Awake()
    {
        base.Awake();
        InitUI();
    }
	void Update () {
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                SetPaiJuInfo(3, "NO");
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                SetPaiJuInfo(3, "3G/4G");
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                SetPaiJuInfo(3, "WIFI");
                break;
        }
    }
    void InitUI()
    {
        cardOne = bottomCards_Pel.Find("cardOne").GetComponent<Image>();
        cardTwo = bottomCards_Pel.Find("cardTwo").GetComponent<Image>();
        cardThree = bottomCards_Pel.Find("cardThree").GetComponent<Image>();

        outCardsCount = new Dictionary<int, Text>();
        for (int i = 1; i <= 15; i++)
        {
            outCardsCount.Add(i, No_Ad_Pel.Find("count").GetChild(i - 1).GetComponent<Text>());
        }

        Head_west = Heads_Pel.Find("Head_west");
        bottom_west = Head_west.Find("bottom").GetComponent<Image>();
        userName_west = Head_west.Find("name").GetComponent<Text>();
        integral_west = Head_west.Find("integral").GetComponent<Text>();
        landlord_west = Head_west.Find("landlord").GetComponent<Image>();
        Head_south = Heads_Pel.Find("Head_south");
        bottom_south = Head_south.Find("bottom").GetComponent<Image>();
        userName_south = Head_south.Find("name").GetComponent<Text>();
        integral_south = Head_south.Find("integral").GetComponent<Text>();
        landlord_south = Head_south.Find("landlord").GetComponent<Image>();
        Head_east = Heads_Pel.Find("Head_east");
        bottom_east = Head_east.Find("bottom").GetComponent<Image>();
        userName_east = Head_east.Find("name").GetComponent<Text>();
        integral_east = Head_east.Find("integral").GetComponent<Text>();
        landlord_east = Head_east.Find("landlord").GetComponent<Image>();

        time = PaiJuInfo_Pel.Find("time").GetComponent<Text>();
        mulriple = PaiJuInfo_Pel.Find("mulriple").GetComponent<Text>();
        signal = PaiJuInfo_Pel.Find("signal").GetComponent<Text>();
        juNum = PaiJuInfo_Pel.Find("juNum").GetComponent<Text>();

        GameOperation_south = GameOperationBtns.Find("GameOperation_south");
        OperationBtns_south = new Button[13];
        for (int i = 0; i < OperationBtns_south.Length; i++)
        {
            OperationBtns_south[i] = GameOperation_south.GetChild(i).GetComponent<Button>();
        }
        AddSouthButtonEvent();
        countDown_south = GameOperation_south.Find("countDown");

        GameOperation_west = GameOperationBtns.Find("GameOperation_west");
        countDown_west = GameOperation_west.Find("countDown");

        GameOperation_east = GameOperationBtns.Find("GameOperation_east");
        countDown_east = GameOperation_east.Find("countDown");

        WestPot = transform.Find("TXPot/WestPot");
        EastPot = transform.Find("TXPot/EastPot");
        SouthPot = transform.Find("TXPot/SouthPot");

        bottomCardsList = Resources.LoadAll<Sprite>("Game_DDZ/Textures/CardsFaceSmall");
    }
    /// <summary>
    /// 设置地主玩家图标
    /// </summary>
    /// <param name="fw">方位</param>
    public void SetLandlordIcon(int fw)
    {
        switch (fw)
        {
            case 1:
                landlord_west.gameObject.SetActive(true);
                break;
            case 2:
                landlord_south.gameObject.SetActive(true);
                break;
            case 3:
                landlord_east.gameObject.SetActive(true);
                break;
        }
    }
    /// <summary>
    /// 设置底牌
    /// </summary>
    /// <param name="indexOne">第一张牌索引</param>
    /// <param name="indexTwo">第二张牌索引</param>
    /// <param name="indexThree">第三张牌索引</param>
    public void SetBottomCards(int indexOne,int indexTwo, int indexThree)
    {
        cardOne.sprite = bottomCardsList[indexOne];
        cardTwo.sprite = bottomCardsList[indexTwo];
        cardThree.sprite = bottomCardsList[indexThree];
    }
    /// <summary>
    /// 设置记牌器数值
    /// </summary>
    /// <param name="name">牌的名字</param>
    public void SetNoAdNum(int name)
    {
        int index = 0;
        if (name / 13 < 4)
        {
            index = name % 13 == 0? 13: name % 13;
        }
        else
        {
            index = name % 13 == 1 ? 13 : 14;
        }
        int tempNum = 0;
        int.TryParse(outCardsCount[index].text, out tempNum);
        if (tempNum - 1 != 0)
        {
            outCardsCount[index].color = new Color(212/255f,73/255f,6/255f,1);
        }
        else
        {
            outCardsCount[index].color = new Color(170/255f, 160/255f, 160/255f, 1);
        }
        outCardsCount[index].text = (--tempNum).ToString();
    }
    /// <summary>
    /// 设置头像图片
    /// </summary>
    /// <param name="fw">方位</param>
    /// <param name="Path">图片地址</param>
    public void SetHeads(int fw,string Path)
    {
        LoadImage.Instance.LoadPicture(Path, bottom_west);
        LoadImage.Instance.LoadPicture(Path, bottom_south);
        LoadImage.Instance.LoadPicture(Path, bottom_east);
    }
    /// <summary>
    /// 设置用户名字
    /// </summary>
    /// <param name="fw">方位</param>
    /// <param name="name">名字</param>
    public void SetUserName(int fw, string name)
    {
        userName_west.text = name;
        userName_south.text = name;
        userName_east.text = name;
    }
    /// <summary>
    /// 设置用户积分
    /// </summary>
    /// <param name="fw">方位</param>
    /// <param name="score">分数</param>
    public void SetIntegral(string fw, string score)
    {
        integral_west.text = score;
        integral_south.text = score;
        integral_east.text = score;
    }
    /// <summary>
    /// 设置牌局信息
    /// </summary>
    /// <param name="type">1(时间),2(倍数),3(信号),4(局数)</param>
    /// <param name="value">设置的值</param>
    public void SetPaiJuInfo(int type, string value)
    {
        switch (type)
        {
            case 1:
                time.text = value;
                break;
            case 2:
                mulriple.text = value;
                break;
            case 3:
                signal.text = value;
                break;
            case 4:
                juNum.text = value;
                break;
        }
    }
    /// <summary>
    /// 显示South操作按钮
    /// </summary>
    /// <param name="time">操作时间</param>
    /// <param name="index">按钮索引0:不出 1:提示 2:出牌 3:一分 4:两分 5:三分 6:不叫(分) 7:叫地主 8:不叫(地主) 9:抢地主 10:不抢 11:加倍 12:不加倍</param>
    /// <param name="act">按钮事件</param>
    public void AddSouthButtonEvent()
    {
        OperationBtns_south[0].onClick.AddListener(delegate { HideSouthGameOperation(); DDZSendMessage.Instance.SendOutCard("");});
        OperationBtns_south[1].onClick.AddListener(delegate { MainLogic.Instance.HintPlayCards(); });
        OperationBtns_south[2].onClick.AddListener(delegate { DDZSendMessage.Instance.SendOutCard("103,104,105");/*HideSouthGameOperation();*/ });
        OperationBtns_south[3].onClick.AddListener(delegate { HideSouthGameOperation(); DDZSendMessage.Instance.SendCallLandlord(2, 1); });
        OperationBtns_south[4].onClick.AddListener(delegate { HideSouthGameOperation(); DDZSendMessage.Instance.SendCallLandlord(2, 2); });
        OperationBtns_south[5].onClick.AddListener(delegate { HideSouthGameOperation(); DDZSendMessage.Instance.SendCallLandlord(2, 3); });
        OperationBtns_south[6].onClick.AddListener(delegate { HideSouthGameOperation(); DDZSendMessage.Instance.SendCallLandlord(2, 0); });
        OperationBtns_south[7].onClick.AddListener(delegate { HideSouthGameOperation(); DDZSendMessage.Instance.SendCallLandlord(1, 1); });
        OperationBtns_south[8].onClick.AddListener(delegate { HideSouthGameOperation(); DDZSendMessage.Instance.SendCallLandlord(1, 0); });
        OperationBtns_south[9].onClick.AddListener(delegate { HideSouthGameOperation(); DDZSendMessage.Instance.SendCallLandlord(1, 1); });
        OperationBtns_south[10].onClick.AddListener(delegate { HideSouthGameOperation(); DDZSendMessage.Instance.SendCallLandlord(1, 0); });
        OperationBtns_south[11].onClick.AddListener(delegate { HideSouthGameOperation(); MainLogic.Instance.AddMultiple(); });
        OperationBtns_south[12].onClick.AddListener(delegate { HideSouthGameOperation(); MainLogic.Instance.NotAddMultiple(); });

    }
    public void ShowSouthGameOperation(int time, int[] index)
    {
        for (int i = 0; i < index.Length; i++)
        {
            OperationBtns_south[index[i]].gameObject.SetActive(true);
        }
        StartCoroutine(CountDown(1, countDown_south, time));
    }
    public void HideSouthGameOperation()
    {
        for (int i = 0; i < OperationBtns_south.Length; i++)
        {
            OperationBtns_south[i].gameObject.SetActive(false);
        }
        countDown_south.gameObject.SetActive(false);
    }
    /// <summary>
    /// 设置West操作按钮
    /// </summary>
    /// <param name="time">操作时间</param>
    public void ShowWestGameOperation(int time)
    {
        StartCoroutine(CountDown(3,countDown_west,time));
    }
    public void HideWestGameOperation()
    {
        countDown_west.gameObject.SetActive(false);
        countDown_west.Find("warn").gameObject.SetActive(true);
    }
    /// <summary>
    /// 设置East操作按钮
    /// </summary>
    /// <param name="time">操作时间</param>
    public void ShowEastGameOperation(int time)
    {
        StartCoroutine(CountDown(2,countDown_east, time));
    }
    public void HideEastGameOperation()
    {
        countDown_east.gameObject.SetActive(false);
        countDown_east.Find("warn").gameObject.SetActive(true);
    }
    /// <summary>
    /// 计时器
    /// </summary>
    /// <param name="text">显示文本</param>
    /// <returns></returns>
    IEnumerator CountDown(int fw,Transform tf,int Num)
    {
        tf.gameObject.SetActive(true);
        float time = Num + 1; ;
        Text text = tf.Find("Text").GetComponent<Text>();
        while (time > 0)
        {
            text.text = ((int)time).ToString();
            time -= Time.deltaTime;
            if (text.text.Equals("10") && tf.Find("warn")!=null) tf.Find("warn").gameObject.SetActive(true);
             yield return 0;
        }
        switch (fw)
        {
            case 1:
                HideSouthGameOperation();
                break;
            case 2:
                HideEastGameOperation();
                break;
            case 3:
                HideWestGameOperation();
                break;
        }
    }
    /// <summary>
    /// Bomb_TX(炸弹)、CunTian_TX(春天)、FeiJi_TX(飞机)、LianDui_TX(连队)、SanDaiEr_TX(三带二)、SanDaiYi_TX(三带一)、SiDaiEr_TX(四带二)、ShunZi_TX(顺子)
    /// </summary>
    /// <param name="type">特效类型</param>
    /// <param name="fw">特效方位，可不输入显示位置为中心</param>
    public void ShowTx(string type,int fw)
    {
        GameObject go = ObjectPool.Instance.Spawn(type);
        switch (fw)
        {
            case 1:
                go.transform.SetParent(SouthPot);
                go.transform.localScale = Vector3.one;
                go.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
                break;
            case 2:
                go.transform.SetParent(EastPot);
                go.transform.localScale = Vector3.one;
                go.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
                break;
            case 3:
                go.transform.SetParent(WestPot);
                go.transform.localScale = Vector3.one;
                go.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
                break;
            default:
                go.transform.SetParent(SouthPot.parent);
                go.transform.localScale = Vector3.one;
                go.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
                break;
        }
    }
}
