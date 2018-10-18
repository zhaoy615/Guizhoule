using UnityEngine;
using System.Collections;
using DG.Tweening;
public struct ConfirmMessages
{
    public Quaternion quater;
    public Transform trans;
    public Vector3 offset;
}


public class FICPaiDui : MonoBehaviour
{
    //public static FICPaiDui _instance;
    private FICStartGame _ScriptStartGame;
    //==============牌堆总父物体，以及各个方位牌堆父物体========================
    private Transform _TransPaiDuiParent;
    private Transform _TransPaiDuiNorthParent;
    private Transform _TransPaiDuiEastParent;
    private Transform _TransPaiDuiSouthParent;
    private Transform _TransPaiDuiWestParent;


    //=============对面方位牌堆赋值=========================
    private Transform _TransNorthUp;
    private Transform _TransNorthDown;
    private Transform _TransNorthDownSide;
    private Vector3 _VecNorthOffset;


    //==============右边方位牌堆赋值========================
    private Transform _TransEastUp;
    private Transform _TransEastDown;
    private Transform _TransEastDownSide;
    private Vector3 _VecEastOffset;


    //===============自己方位牌堆赋值=======================
    private Transform _TransSouthUp;
    private Transform _TransSouthDown;
    private Transform _TransSouthDownSide;
    private Vector3 _VecSouthOffset;


    //================左边方位牌堆赋值======================
    private Transform _TransWestUp;
    private Transform _TransWestDown;
    private Transform _TransWestDownSide;
    private Vector3 _VecWestOffset;


    //==============通用:牌堆上下偏移量========================
    private Vector3 _VeceUpOffset;

    //==============通用:生成牌的预制体========================
    private GameObject _GOCube;

    //==============通用:桌面牌堆盖板========================
    private Transform _TransDeskCap;
    //==============通用:上下牌数组========================
    public GameObject[] _ArrayUpPais;
    public GameObject[] _ArrayDownPais;

    //==============通用:生成牌堆顺序位置信息========================
    private Vector3 _FirstPosition;
    private Vector3 _SecondPosition;
    private Vector3 _ThirdPosition;
    private Vector3 _FourthPosition;

    //==============通用:生成牌堆顺序位置信息========================
    private Quaternion _QuaNorth;
    private Quaternion _QuaEast;
    private Quaternion _QuaSouth;
    private Quaternion _QuaWest;

    //==============通用:3个方法合成一个，返回结构体========================
    private ConfirmMessages message;
    private int numIndexWhole;
    private int numIndexGang;
    public bool isSkip;//为了避免for循环删除物体，摸，不跳转删除下一排
    public bool isSkipGang;//为了避免for循环删除物体，杠，不跳转删除下一排

    //==============桌面盖子下降动画==========
    private  Tweener twe;
    public int dicNum1;
    public int dicNum2;

    //private void Awake()
    //{
    //    _instance = this;
    //}
    private void Start()
    {
        _ScriptStartGame = gameObject.GetComponent<FICStartGame>();
        //==============牌堆总父物体，以及各个方位牌堆父物体========================
        _TransPaiDuiParent = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/PaiDuiContainer");
        _TransPaiDuiNorthParent = _TransPaiDuiParent.Find("NorthContainer");
        _TransPaiDuiEastParent = _TransPaiDuiParent.Find("EastContainer");
        _TransPaiDuiSouthParent = _TransPaiDuiParent.Find("SouthContainer");
        _TransPaiDuiWestParent = _TransPaiDuiParent.Find("WestContainer");

        //=============对面方位牌堆赋值=========================
        _TransNorthUp = _TransPaiDuiNorthParent.Find("northup");
        _TransNorthDown = _TransPaiDuiNorthParent.Find("northdown");
        _TransNorthDownSide = _TransPaiDuiNorthParent.Find("northdown2");
        _VecNorthOffset = _TransNorthDownSide.position - _TransNorthDown.position;

        //==============右边方位牌堆赋值========================
        _TransEastUp = _TransPaiDuiEastParent.Find("eastup");
        _TransEastDown = _TransPaiDuiEastParent.Find("eastdown");
        _TransEastDownSide = _TransPaiDuiEastParent.Find("eastdown2");
        _VecEastOffset = _TransEastDownSide.position - _TransEastDown.position;


        //===============自己方位牌堆赋值=======================
        _TransSouthUp = _TransPaiDuiSouthParent.Find("southup");
        _TransSouthDown = _TransPaiDuiSouthParent.Find("southdown");
        _TransSouthDownSide = _TransPaiDuiSouthParent.Find("southdown2");
        _VecSouthOffset = _TransSouthDownSide.position - _TransSouthDown.position;


        //================左边方位牌堆赋值======================
        _TransWestUp = _TransPaiDuiWestParent.Find("westup");
        _TransWestDown = _TransPaiDuiWestParent.Find("westdown");
        _TransWestDownSide = _TransPaiDuiWestParent.Find("westdown2");
        _VecWestOffset = _TransWestDownSide.position - _TransWestDown.position;


        //==============通用:牌堆上下偏移量========================
        _VeceUpOffset = _TransWestUp.position - _TransWestDown.position;

        //==============通用:生成牌的预制体========================
        //_GOCube = Resources.Load<GameObject>("Game_GYMJ/Prefabs/emptyGO");
        _GOCube = GameObject.Find("emptyGO").gameObject;

        //==============通用:桌面牌堆盖板========================
        _TransDeskCap = transform.Find("/Game_Prefabs/TABLE/盖板");
        //==============通用:上下牌数组========================
        _ArrayUpPais = new GameObject[54];
        _ArrayDownPais = new GameObject[54];

        ////==============通用:生成牌堆顺序位置信息========================
        //_QuaNorth = Quaternion.Euler(0, 0, 0);
        //_QuaEast = Quaternion.identity;
        //_QuaSouth = Quaternion.Euler(0, 90, 90);
        //_QuaWest = Quaternion.Euler(90, 90, 90);

        //==============通用:3个方法合成一个，返回结构体========================
        message = new ConfirmMessages();
        isSkip = true;
        isSkipGang = true;
    }


    public void KaiShiPaiJu()
    {
        DownDeskCap();
        string[] str = GameInfo.returnZhuang.seizi.Split(',');
        dicNum1 = int.Parse(str[0]);
        dicNum2 = int.Parse(str[1]);
        DaShaiZi(dicNum1,dicNum2 );//色子器的值右庄信息传递

        //6.手牌排序
        // 7.定缺显示
    }


    /// <summary>
    /// 摸牌时候，牌堆减少一张
    /// </summary>
    public void PaiDuiReduceOneForMo()
    {
        ReducePaiByOne(numIndexWhole);
    }
    /// <summary>
    /// 杠牌时候，牌堆从最后减少一张
    /// </summary>
    public void PaiDuiReduceOneForGang()
    {
        ReducePaiByGang(numIndexGang);
    }
    /// <summary>
    /// 最开始发牌时候，牌堆4张4张的减少
    /// </summary>
    public void PaiDuiReduceForStart()
    {
        numIndexWhole = CalculateIndex(GameInfo.zhuang, dicNum1, dicNum2);

        ReducePaisForStart();
        numIndexGang = numIndexWhole - 1;
        if (GameInfo.roomInfo.is_yuanque == 1 || GameInfo.isYuanQue == 1)
        {
            YuanQue();
        }
    }
    /// <summary>
    /// 重连之后，根据摸牌的总数，和杠牌的数目，还原牌堆
    /// </summary>
    /// <param name="moNum"></param>
    /// <param name="gangNum"></param>
    public void PaiDuiReconnect(int moNum, int gangNum)
    {
        PaiDuiInit();
       
        numIndexWhole = CalculateIndex(GameInfo.zhuang, dicNum1, dicNum2);
        numIndexGang = numIndexWhole - 1;
        for (int i = 0; i < moNum; i++)
        {
            ReducePaiByOne(numIndexWhole);
        }
        for (int i = 0; i < gangNum; i++)
        {
            ReducePaiByGang(numIndexGang);
        }
    }
    public void ClearPaiDui()
    {
        for (int i = 0; i < 108; i++)
        {
            PaiDuiReduceOneForMo();
        }
    }

    #region  私有方法

    /// <summary>
    /// 桌面盖子陷落,和抬起
    /// </summary>
    private void DownDeskCap()
    {
        twe = _TransDeskCap.DOMoveY(-0.7f, 1f);
        twe.OnComplete(UpDeskCap);
    }
    private void UpDeskCap()
    {
        PaiDuiInit();
        PaiDuiDown();
        _TransDeskCap.DOMoveY(0.21336f, 1f);
        PaiDuiUp();
    }
    /// <summary>
    /// 默认缺牌，直接杠掉36张
    /// </summary>
    private void YuanQue()
    {
        for (int i = 0; i < 36; i++)
        {
            PaiDuiReduceOneForGang();
        }

    }
    /// <summary>
    /// 将刚刚初始化的牌堆移动到牌桌以下
    /// </summary>
    private void PaiDuiDown()
    {
        foreach (var item in _ArrayDownPais)
        {
            item.transform.position = item.transform.position - _VeceUpOffset * 2;
        }
        foreach (var item in _ArrayUpPais)
        {
            item.transform.position = item.transform.position - _VeceUpOffset * 2;
        }
    }

    private void PaiDuiUp()
    {
        foreach (var item in _ArrayDownPais)
        {
            item.transform.DOMove(item.transform.position + 2 * _VeceUpOffset, 1f);
        }
        foreach (var item in _ArrayUpPais)
        {
            item.transform.DOMove(item.transform.position + 2 * _VeceUpOffset, 1f);
        }
    }
    /// <summary>
    /// 牌堆升起之后，打色子，需要生成两张色子面，并延时，开始减少牌
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    private void DaShaiZi(int num1, int num2)
    {
        //色子动画没做

        Invoke("PaiDuiReduceForStart", 2f);
    }
    /// <summary>
    /// 牌堆生成初始化
    /// </summary>
    private void PaiDuiInit()
    {
        FW fw = GameInfo.GetFW(1);
        ConfirmPositionOrder(fw);
        InitPaiDui(fw);

    }
    private void ConfirmPositionOrder(FW fw)
    {
        switch (fw)
        {
            case FW.North:
                _FirstPosition = _TransNorthUp.position;
                _SecondPosition = _TransEastUp.position;
                _ThirdPosition = _TransSouthUp.position;
                _FourthPosition = _TransWestUp.position;
                break;
            case FW.East:
                _FirstPosition = _TransEastUp.position;
                _SecondPosition = _TransSouthUp.position;
                _ThirdPosition = _TransWestUp.position;
                _FourthPosition = _TransNorthUp.position;
                break;
            case FW.South:
                _FirstPosition = _TransSouthUp.position;
                _SecondPosition = _TransWestUp.position;
                _ThirdPosition = _TransNorthUp.position;
                _FourthPosition = _TransEastUp.position;
                break;
            case FW.West:
                _FirstPosition = _TransWestUp.position;
                _SecondPosition = _TransNorthUp.position;
                _ThirdPosition = _TransEastUp.position;
                _FourthPosition = _TransSouthUp.position;
                break;
        }
    }

    private void InitPaiDui(FW fw)
    {
        GameObject go;
        for (int i = 0; i < 54; i++)
        {
            if (i < 14)
            {
                go = (GameObject)GameObject.Instantiate(_GOCube, _FirstPosition + ConfirmMessageMethod(fw, 1).offset * i, ConfirmMessageMethod(fw, 1).quater, ConfirmMessageMethod(fw, 1).trans);
                _ArrayUpPais[i] = go;
                go = (GameObject)GameObject.Instantiate(_GOCube, _ArrayUpPais[i].transform.position - _VeceUpOffset, ConfirmMessageMethod(fw, 1).quater, ConfirmMessageMethod(fw, 1).trans);
                _ArrayDownPais[i] = go;
            }
            if (i > 13 && i < 27)
            {
                go = (GameObject)GameObject.Instantiate(_GOCube, _SecondPosition + ConfirmMessageMethod(fw, 2).offset * (i - 14), ConfirmMessageMethod(fw, 2).quater, ConfirmMessageMethod(fw, 2).trans);
                _ArrayUpPais[i] = go;
                go = (GameObject)GameObject.Instantiate(_GOCube, _ArrayUpPais[i].transform.position - _VeceUpOffset, ConfirmMessageMethod(fw, 2).quater, ConfirmMessageMethod(fw, 2).trans);
                _ArrayDownPais[i] = go;
            }
            if (i > 26 && i < 41)
            {
                go = (GameObject)GameObject.Instantiate(_GOCube, _ThirdPosition + ConfirmMessageMethod(fw, 3).offset * (i - 27), ConfirmMessageMethod(fw, 3).quater, ConfirmMessageMethod(fw, 3).trans);
                _ArrayUpPais[i] = go;
                go = (GameObject)GameObject.Instantiate(_GOCube, _ArrayUpPais[i].transform.position - _VeceUpOffset, ConfirmMessageMethod(fw, 3).quater, ConfirmMessageMethod(fw, 3).trans);
                _ArrayDownPais[i] = go;
            }

            if (i > 40 && i < 54)
            {
                go = (GameObject)GameObject.Instantiate(_GOCube, _FourthPosition + ConfirmMessageMethod(fw, 4).offset * (i - 41), ConfirmMessageMethod(fw, 4).quater, ConfirmMessageMethod(fw, 4).trans);
                _ArrayUpPais[i] = go;
                go = (GameObject)GameObject.Instantiate(_GOCube, _ArrayUpPais[i].transform.position - _VeceUpOffset, ConfirmMessageMethod(fw, 4).quater, ConfirmMessageMethod(fw, 4).trans);
                _ArrayDownPais[i] = go;
            }
        }

    }

    private ConfirmMessages ConfirmMessageMethod(FW fw, int orderNum)
    {

        switch (fw)
        {
            case FW.North:
                switch (orderNum)
                {
                    case 1:
                        message.offset = _VecNorthOffset;
                        message.trans = _TransPaiDuiNorthParent;
                        message.quater = _QuaNorth;
                        break;
                    case 2:
                        message.offset = _VecEastOffset;
                        message.trans = _TransPaiDuiEastParent;
                        message.quater = _QuaEast;
                        break;
                    case 3:
                        message.offset = _VecSouthOffset;
                        message.trans = _TransPaiDuiSouthParent;
                        message.quater = _QuaSouth;
                        break;
                    case 4:
                        message.offset = _VecWestOffset;
                        message.trans = _TransPaiDuiWestParent;
                        message.quater = _QuaWest;
                        break;
                }
                break;
            case FW.East:
                switch (orderNum)
                {
                    case 1:
                        message.offset = _VecEastOffset;
                        message.trans = _TransPaiDuiEastParent;
                        message.quater = _QuaEast;
                        break;
                    case 2:
                        message.offset = _VecSouthOffset;
                        message.trans = _TransPaiDuiSouthParent;
                        message.quater = _QuaSouth;
                        break;
                    case 3:
                        message.offset = _VecWestOffset;
                        message.trans = _TransPaiDuiWestParent;
                        message.quater = _QuaWest;
                        break;
                    case 4:
                        message.offset = _VecNorthOffset;
                        message.trans = _TransPaiDuiNorthParent;
                        message.quater = _QuaNorth;
                        break;
                }
                break;
            case FW.South:
                switch (orderNum)
                {
                    case 1:
                        message.offset = _VecSouthOffset;
                        message.trans = _TransPaiDuiSouthParent;
                        message.quater = _QuaSouth;
                        break;
                    case 2:
                        message.offset = _VecWestOffset;
                        message.trans = _TransPaiDuiWestParent;
                        message.quater = _QuaWest;
                        break;
                    case 3:
                        message.offset = _VecNorthOffset;
                        message.trans = _TransPaiDuiNorthParent;
                        message.quater = _QuaNorth;
                        break;
                    case 4:
                        message.offset = _VecEastOffset;
                        message.trans = _TransPaiDuiEastParent;
                        message.quater = _QuaEast;
                        break;
                }
                break;
            case FW.West:
                switch (orderNum)
                {
                    case 1:
                        message.offset = _VecWestOffset;
                        message.trans = _TransPaiDuiWestParent;
                        message.quater = _QuaWest;
                        break;
                    case 2:
                        message.offset = _VecNorthOffset;
                        message.trans = _TransPaiDuiNorthParent;
                        message.quater = _QuaNorth;
                        break;
                    case 3:
                        message.offset = _VecEastOffset;
                        message.trans = _TransPaiDuiEastParent;
                        message.quater = _QuaEast;
                        break;
                    case 4:
                        message.offset = _VecSouthOffset;
                        message.trans = _TransPaiDuiSouthParent;
                        message.quater = _QuaSouth;
                        break;
                }
                break;
        }
        return message;
    }

    private int CalculateIndex(int zhuangNum, int diceNum1, int diceNum2)
    {
        FW zhuangfw = GameInfo.GetFW(zhuangNum);
        FW fangzhufw = GameInfo.GetFW(1);
        int indexNum = 0;
        int minNum = diceNum1 > diceNum2 ? diceNum2 : diceNum1;
        int num = diceNum1 + diceNum2;

        switch (fangzhufw)
        {
            case FW.East:
                switch (zhuangfw)
                {
                    case FW.East:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 41;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 27;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 14;
                                break;
                        }
                        break;
                    case FW.West:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 14;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 41;
                                break;
                        }
                        break;
                    case FW.North:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 27;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 14;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum;
                                break;
                        }
                        break;
                    case FW.South:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 41;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 27;
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;
            case FW.West:
                switch (zhuangfw)
                {
                    case FW.East:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 14;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 27;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 41;
                                break;
                        }
                        break;
                    case FW.West:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 41;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 27;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 14;
                                break;
                        }
                        break;
                    case FW.North:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 41;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 27;
                                break;
                        }
                        break;
                    case FW.South:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 27;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 14;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum;
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;
            case FW.North:
                switch (zhuangfw)
                {
                    case FW.East:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 41;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 27;
                                break;
                        }
                        break;
                    case FW.West:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 27;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 14;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum;
                                break;
                        }
                        break;
                    case FW.North:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 41;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 27;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 14;
                                break;
                        }
                        break;
                    case FW.South:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 14;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 41;
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;
            case FW.South:
                switch (zhuangfw)
                {
                    case FW.East:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 27;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 14;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum;
                                break;
                        }
                        break;
                    case FW.West:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 41;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 27;
                                break;
                        }
                        break;
                    case FW.North:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 14;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 41;
                                break;
                        }
                        break;
                    case FW.South:
                        switch (num)
                        {
                            case 2:
                            case 6:
                            case 10:
                                indexNum = minNum + 41;
                                break;

                            case 3:
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                indexNum = minNum + 27;
                                break;
                            case 4:
                            case 8:
                            case 12:
                                indexNum = minNum + 14;
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;
        }
        return indexNum;
    }

    private void ReducePaiByGang(int num)
    {
        if (numIndexGang < 0)
        {
            numIndexGang = 53;
        }
        if (_ArrayUpPais[numIndexGang] != null&&isSkipGang)
        {
            Destroy(_ArrayUpPais[numIndexGang]);
            isSkipGang = false;
        }
        else
        {
            Destroy(_ArrayDownPais[numIndexGang]);
            numIndexGang--;
            isSkipGang = true;
        }

    }

    private void ReducePaiByOne(int num)
    {
        if (numIndexWhole > 53)
        {
            numIndexWhole = 0;
        }
        if (_ArrayUpPais[numIndexWhole] != null && isSkip)
        {
            Destroy(_ArrayUpPais[numIndexWhole]);
            isSkip = false;
        }
        else
        {
            Destroy(_ArrayDownPais[numIndexWhole]);
            numIndexWhole++;
            isSkip = true;
        }
    }
    private void ReducePaisForStart()
    {
        if (GameInfo.room_peo==2)
        {
            Invoke("ReducePaiByFoue", 0.1f);
            Invoke("ReducePaiByFoue", 0.2f);
            Invoke("ReducePaiByFoue", 0.3f);
            Invoke("ReducePaiByFoue", 0.4f);
            Invoke("ReducePaiByFoue", 0.5f);
            Invoke("ReducePaiByFoue", 0.6f);
            Invoke("ReducePaiForBuMo", 0.7f);
            Invoke("ReducePaiForBuMo", 0.8f);
            Invoke("ReducePaiForBuMo", 0.9f);

        }
        else if (GameInfo.room_peo == 3)
        {
            Invoke("ReducePaiByFoue", 0.1f);
            Invoke("ReducePaiByFoue", 0.2f);
            Invoke("ReducePaiByFoue", 0.3f);
            Invoke("ReducePaiByFoue", 0.4f);
            Invoke("ReducePaiByFoue", 0.5f);
            Invoke("ReducePaiByFoue", 0.6f);
            Invoke("ReducePaiByFoue", 0.7f);
            Invoke("ReducePaiByFoue", 0.8f);
            Invoke("ReducePaiByFoue", 0.9f);
            Invoke("ReducePaiByFoue", 1f);
        }
        else
        {
            Invoke("ReducePaiByFoue", 0.1f);
            Invoke("ReducePaiByFoue", 0.2f);
            Invoke("ReducePaiByFoue", 0.3f);
            Invoke("ReducePaiByFoue", 0.4f);
            Invoke("ReducePaiByFoue", 0.5f);
            Invoke("ReducePaiByFoue", 0.6f);
            Invoke("ReducePaiByFoue", 0.7f);
            Invoke("ReducePaiByFoue", 0.8f);
            Invoke("ReducePaiByFoue", 0.9f);
            Invoke("ReducePaiByFoue", 1f);
            Invoke("ReducePaiByFoue", 1.1f);
            Invoke("ReducePaiByFoue", 1.2f);
            Invoke("ReducePaiByFoue", 1.3f);
            Invoke("ReducePaiForBuMo", 1.4f);
        }
       
       // _ScriptStartGame.GameReturnStartGame();
        _ScriptStartGame.GameReturnStartGameNew();
    }

    private void ReducePaiForBuMo()
    {
        ReducePaiByOne(numIndexWhole + 26);
    }
    private void ReducePaiByFoue()
    {
        for (int i = 0; i < 4; i++)
        {
            ReducePaiByOne(numIndexWhole);
        }
    }



    #endregion

}
