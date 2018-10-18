using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using MJBLL.common;
using DNL;
using System.Text;

public enum FW
{
    East = 1,
    West = 2,
    North = 3,
    South = 4
}

public enum PPTYPE
{
    Peng = 1,
    Gang = 2,
    // AnGang = 3,
    //ZWGang = 4
}
public class FICpaipaipai : MonoBehaviour
{


    //private Material _LuceMaterial;
    private Shader _LucenShader;
    public Quaternion eShouPaiQua = Quaternion.Euler(180, 90, 0);
    Quaternion ePengQiPaiQua = Quaternion.Euler(90, 90, 0);
    Quaternion eGangPaiQua = Quaternion.Euler(90, 90, 90);


    public Quaternion wShouPaiQua = Quaternion.Euler(0, 90, 180);
    Quaternion wPengQiPaiQua = Quaternion.Euler(90, 90, 180);
    Quaternion wGangPaiQua = Quaternion.Euler(90, 90, 90);

    public Quaternion nShouPaiQua = Quaternion.Euler(0, 180, 180);
    Quaternion nPengQiPaiQua = Quaternion.Euler(90, 180, 180);
    Quaternion nGangPaiQua = Quaternion.Euler(90, 90, 180);

    Quaternion sPengQiPaiQua = Quaternion.Euler(90, 0, 180);
    Quaternion sGangPaiQua = Quaternion.Euler(90, -90, 180);


    private FICMaskPai maskPai;//从mj_event复制过来的
    private FICMyCards myCards;
    private GameObject diamond;
    private FICStartGame startGame;
    //新添定义，下一张碰牌区牌的位置，牌的高度，牌的宽度(暂时没用)，
    public Vector3 eNextPos;
    public Vector3 wNextPos;
    public Vector3 nNextPos;
    public Vector3 sNextPos;

    public Vector3 reseteNextPos;
    public Vector3 resetwNextPos;
    public Vector3 resetnNextPos;
    public Vector3 resetsNextPos;
    private float moveTime = 0.6f;

    private Manager_Audio managerAudio;

    //private float paiHeight;
    //private float paiWeith;

    #region 各种定义
    //右边玩家手牌，弃牌，碰杠牌的预制体定义,也许用一个来代替，代码控制
    //private GameObject eShouPaiGO;
    //private GameObject eQiPaiGO;
    //private GameObject ePengPaiGO;
    //右边玩家手牌，弃牌，碰牌父物体，第一张手牌，5张碰牌，2张弃牌，求出四个偏移量

    private Transform eShouPaiContainerTrans;
    public Transform eQiPaiContainerTrans;
    private Transform ePengGangPaiContainerTrans;

    public Transform eFirstShouPaiTrans;

    public Transform eFirstPengPaiTrans;
    private Transform eSecondPengPaiTrans;
    private Transform eThirdPengPaiTrans;
    private Transform eFourPengPaiTrans;
    private Transform eFivePengPaiTrans;

    public Transform eFirstQiPaiTrans;
    private Transform eThirdQiPaiTrans;

    public Vector3 eOffsetX;
    private Vector3 eOffsetLX;
    private Vector3 eGangOffsetZ;
    public Vector3 eQiOffsetZ;
    //右边玩家所需要的集合
    public List<GameObject> shoupaiEList = new List<GameObject>();
    public List<GameObject> qipaiEList = new List<GameObject>();
    public List<GameObject> pengpaiEList = new List<GameObject>();
    public GameObject emptyGO;//为了动画
    private GameObject koupaiGO;
    //两次杠牌在相对位置的时候，间距要多加，所以设置了3个bool变量
    private bool eisNorth = false;//E先杠north，之后杠south，需多偏移一点儿
    private bool wisSouth = false;//w先杠south，之后杠north，需多偏移一点儿
    private bool nisWest = false;//n先杠west，之后杠east，需多偏移一点儿
    //当碰牌，暗杠，杠牌，转弯杠，当最右边牌为竖排的时候，杠左边的玩家，需多便宜一点儿，本来打算设个标志位，现在发现将上边三个bool值取反即可
    //最后发现，只要杠左边，加个偏移就行了，分类讨论个毛线


    //左边玩家手牌，弃牌，碰杠牌的预制体定义
    //private GameObject wShouPaiGO;
    //private GameObject wQiPaiGO;
    //private GameObject wPengPaiGO;
    //右边玩家手牌，弃牌，碰牌父物体，第一张手牌，5张碰牌，2张弃牌，求出四个偏移量

    private Transform wShouPaiContainerTrans;
    public Transform wQiPaiContainerTrans;
    private Transform wPengGangPaiContainerTrans;

    public Transform wFirstShouPaiTrans;

    public Transform wFirstPengPaiTrans;
    private Transform wSecondPengPaiTrans;
    private Transform wThirdPengPaiTrans;
    private Transform wFourPengPaiTrans;
    private Transform wFivePengPaiTrans;

    public Transform wFirstQiPaiTrans;
    private Transform wThirdQiPaiTrans;

    public Vector3 wOffsetX;
    private Vector3 wOffsetLX;
    private Vector3 wGangOffsetZ;
    public Vector3 wQiOffsetZ;
    //右边玩家所需要的集合
    public List<GameObject> shoupaiWList = new List<GameObject>();
    public List<GameObject> qipaiWList = new List<GameObject>();
    public List<GameObject> pengpaiWList = new List<GameObject>();

    //对边玩家手牌，弃牌，碰杠牌的预制体定义
    //private GameObject nShouPaiGO;
    //private GameObject nQiPaiGO;
    //private GameObject nPengPaiGO;
    //右边玩家手牌，弃牌，碰牌父物体，第一张手牌，5张碰牌，2张弃牌，求出四个偏移量

    private Transform nShouPaiContainerTrans;
    public Transform nQiPaiContainerTrans;
    private Transform nPengGangPaiContainerTrans;

    public Transform nFirstShouPaiTrans;

    public Transform nFirstPengPaiTrans;
    private Transform nSecondPengPaiTrans;
    private Transform nThirdPengPaiTrans;
    private Transform nFourPengPaiTrans;
    private Transform nFivePengPaiTrans;

    public Transform nFirstQiPaiTrans;
    private Transform nThirdQiPaiTrans;

    public Vector3 nOffsetX;
    private Vector3 nOffsetLX;
    private Vector3 nGangOffsetZ;
    public Vector3 nQiOffsetZ;
    //对边玩家所需要的集合
    public List<GameObject> shoupaiNList = new List<GameObject>();
    public List<GameObject> qipaiNList = new List<GameObject>();
    public List<GameObject> pengpaiNList = new List<GameObject>();




    public int huaSe = 0;//从mj_event 剪切过来的

    //Transform jiFirst = GameObject.Find("Ji_S_Up").GetComponent<Transform>();
    //Transform jiSecond = GameObject.Find("Ji_S").GetComponent<Transform>();
    //Vector3 JiSum;
    #endregion
    public void Start()
    {
       // _LuceMaterial = new Material(Shader.Find("CookBookShaders/Cover Translucent"));
        //_LucenShader = Shader.Find("CookBookShaders/Cover Translucent");
        _LucenShader = Resources.Load<Shader>("Cover Translucent");
        managerAudio = GameObject.Find("Main Camera").GetComponent<Manager_Audio>();
        myCards = gameObject.GetComponent<FICMyCards>();
        //JiSum = jiSecond.position - jiFirst.position;
        diamond = transform.Find("/Game_Prefabs/diamond").gameObject;
        maskPai = transform.Find("/Main Camera").GetComponent<FICMaskPai>();
        startGame=gameObject.GetComponent<FICStartGame>();
        //paiHeight = transform.Find("/Lights/emptyGO").gameObject.GetComponent<Collider>().bounds.size.y;
        //paiWeith = transform.Find("/Lights/emptyGO").gameObject.GetComponent<Collider>().bounds.size.x;
        //右边玩家之前定义字段赋值
        //=====================为手牌，弃牌，碰牌的父物体赋值===============      
        eShouPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/EastContainer/ShouPaiContainer");
        eQiPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/EastContainer/QiPaiContainer");
        ePengGangPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/EastContainer/PengGangPaiContainer");
        //======================手牌预制体==============       
        //eShouPaiGO = Resources.Load("shoupai_e") as GameObject;
        //eQiPaiGO = Resources.Load("qipai_e") as GameObject;
        //ePengPaiGO = Resources.Load("peng_e") as GameObject;
        //======================手牌位置确定==============
        eFirstShouPaiTrans = eShouPaiContainerTrans.transform.Find("MJbox124");
        //======================弃牌位置确定==============
        eFirstQiPaiTrans = eQiPaiContainerTrans.transform.Find("MJbox257");
        eThirdQiPaiTrans = eQiPaiContainerTrans.transform.Find("MJbox258");
        //======================碰牌位置确定==============
        eFirstPengPaiTrans = ePengGangPaiContainerTrans.transform.Find("MJbox191");
        eSecondPengPaiTrans = ePengGangPaiContainerTrans.transform.Find("MJbox188");
        eThirdPengPaiTrans = ePengGangPaiContainerTrans.transform.Find("MJbox192");
        eFourPengPaiTrans = ePengGangPaiContainerTrans.transform.Find("MJbox201");
        eFivePengPaiTrans = ePengGangPaiContainerTrans.transform.Find("MJbox189");
        //======================偏移量确定==============
        eOffsetX = eSecondPengPaiTrans.position - eFirstPengPaiTrans.position;
        eOffsetLX = eFivePengPaiTrans.position - eFourPengPaiTrans.position;
        eGangOffsetZ = eThirdPengPaiTrans.position - eSecondPengPaiTrans.position;
        eQiOffsetZ = eThirdQiPaiTrans.position - eFirstQiPaiTrans.position;
        //======================生成下一张牌的位置确定=======
        emptyGO = transform.Find("/Game_Prefabs/emptyGO").gameObject;
        eNextPos = eFirstPengPaiTrans.position;
        reseteNextPos = eNextPos;


        //左边玩家之前定义字段赋值
        //=====================为手牌，弃牌，碰牌的父物体赋值===============      
        wShouPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/WestContainer/ShouPaiContainer");
        wQiPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/WestContainer/QiPaiContainer");
        wPengGangPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/WestContainer/PengGangPaiContainer");
        //======================手牌预制体==============       
        //wShouPaiGO = Resources.Load("shoupai_e") as GameObject;
        //wQiPaiGO = Resources.Load("qipai_e") as GameObject;
        //wPengPaiGO = Resources.Load("peng_e") as GameObject;
        //======================手牌位置确定==============
        wFirstShouPaiTrans = wShouPaiContainerTrans.transform.Find("MJbox147");
        //======================弃牌位置确定==============
        wFirstQiPaiTrans = wQiPaiContainerTrans.transform.Find("MJbox293");
        wThirdQiPaiTrans = wQiPaiContainerTrans.transform.Find("MJbox294");
        //======================碰牌位置确定==============
        wFirstPengPaiTrans = wPengGangPaiContainerTrans.transform.Find("MJbox173");
        wSecondPengPaiTrans = wPengGangPaiContainerTrans.transform.Find("MJbox169");
        wThirdPengPaiTrans = wPengGangPaiContainerTrans.transform.Find("MJbox175");
        wFourPengPaiTrans = wPengGangPaiContainerTrans.transform.Find("MJbox171");
        wFivePengPaiTrans = wPengGangPaiContainerTrans.transform.Find("MJbox170");
        //======================偏移量确定==============
        wOffsetX = wSecondPengPaiTrans.position - wFirstPengPaiTrans.position;
        wOffsetLX = wFivePengPaiTrans.position - wFourPengPaiTrans.position;
        wGangOffsetZ = wThirdPengPaiTrans.position - wSecondPengPaiTrans.position;
        wQiOffsetZ = wThirdQiPaiTrans.position - wFirstQiPaiTrans.position;
        //======================生成下一张牌的位置确定=======
        wNextPos = wFirstPengPaiTrans.position;
        resetwNextPos = wNextPos;
        //对边玩家之前定义字段赋值
        //=====================为手牌，弃牌，碰牌的父物体赋值===============      
        nShouPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/NorthContainer/ShouPaiContainer");
        nQiPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/NorthContainer/QiPaiContainer");
        nPengGangPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/NorthContainer/PengGangPaiContainer");
        //======================手牌预制体==============       
        //nShouPaiGO = Resources.Load("shoupai_e") as GameObject;
        //nQiPaiGO = Resources.Load("qipai_e") as GameObject;
        //nPengPaiGO = Resources.Load("peng_e") as GameObject;
        //======================手牌位置确定==============
        nFirstShouPaiTrans = nShouPaiContainerTrans.transform.Find("MJbox147");
        //======================弃牌位置确定==============
        nFirstQiPaiTrans = nQiPaiContainerTrans.transform.Find("MJbox275");
        nThirdQiPaiTrans = nQiPaiContainerTrans.transform.Find("MJbox276");
        //======================碰牌位置确定==============
        nFirstPengPaiTrans = nPengGangPaiContainerTrans.transform.Find("MJbox208");
        nSecondPengPaiTrans = nPengGangPaiContainerTrans.transform.Find("MJbox210");
        nThirdPengPaiTrans = nPengGangPaiContainerTrans.transform.Find("MJbox209");
        nFourPengPaiTrans = nPengGangPaiContainerTrans.transform.Find("MJbox203");
        nFivePengPaiTrans = nPengGangPaiContainerTrans.transform.Find("MJbox204");
        //======================偏移量确定==============
        nOffsetX = nSecondPengPaiTrans.position - nFirstPengPaiTrans.position;
        nOffsetLX = nFivePengPaiTrans.position - nFourPengPaiTrans.position;
        nGangOffsetZ = nThirdPengPaiTrans.position - nSecondPengPaiTrans.position;
        nQiOffsetZ = nThirdQiPaiTrans.position - nFirstQiPaiTrans.position;
        //======================生成下一张牌的位置确定=======
        nNextPos = nFirstPengPaiTrans.position;
        resetnNextPos = nNextPos;
    }

    //各种与3d牌有关的操作和动作
    //1.牌局开始，每家生成13张手牌，根据方位和数量生成//did
    //2.摸牌玩家在最右边生成一张手牌，偏移量14？(2d摸牌待做)
    //3.普通出牌，右，左，对边三家，随机打出一张手牌到弃牌区，生成相应牌面，将最后一张手牌放到手牌空出位置（也可随机生成一个位置）//did
    //4.自己出牌，（需调用排序方法，涉及到2维动画，以后再做）
    //5.普通碰牌，手牌减少两张，（现在直接销毁前两张，）碰牌区添加三张牌，生成相应牌面，被碰方位弃牌区减少一张，
    //5.碰牌出牌：次奥 忘了，直接将最后一张手牌移动到弃牌区，生成相应牌面，如果位置不够，再调整为第一张好了
    //6.杠牌三种情况，自己三张杠别人一张，自己暗杠，碰后杠（杠牌牌面以及朝向，以后再做）
    //普通杠牌，手牌减少三张，（现在直接销毁前三张，）碰牌区添加四张牌，被杠牌方位弃牌区减少一张，
    //暗杠，手牌减少四张，(销毁前三张以及最后刚刚的摸牌，己方摸牌区生成一张手牌），或者（直接销毁前四张，摸牌区不动，只改变牌面），现在直接销毁前四张
    //碰后杠，手牌减少一张，（现在直接销毁最前面一张），摸牌区不动，碰牌区生成一张碰牌(碰牌，杠牌顺序，位置、牌面算法未做)
    //7.胡牌，所有玩家亮出所有手牌（至于切换场景，清空所有人手牌碰牌弃牌集合，以后做）
    //

    /// <summary>
    /// 根据方位和数量生成手牌数量,将手牌加入手牌集合
    /// </summary>
    /// <param name="fw">方位</param>
    /// <param name="count">数量</param>
    public void CeretShouPai(FW fw, int count)
    {
        int index = 0;
        if (count < 13)
        {
            index = 12 - count;
        }
        switch ((int)fw)
        {
            case 1:
                for (int i = 0; i < count; i++)
                {
                    GameObject goe = GameObject.Instantiate(emptyGO, eFirstShouPaiTrans.position + (int)((index + i) + (index + i) / 13) * eOffsetX, eShouPaiQua, eShouPaiContainerTrans) as GameObject;
                    goe.layer = 10;
                    shoupaiEList.Add(goe);

                }
                break;
            case 2:
                for (int i = 0; i < count; i++)
                {
                    GameObject gow = GameObject.Instantiate(emptyGO, wFirstShouPaiTrans.position + (int)((index + i) + (index + i) / 13) * wOffsetX, wShouPaiQua, wShouPaiContainerTrans) as GameObject;
                    gow.layer = 9;
                    shoupaiWList.Add(gow);
                }
                break;
            case 3:
                for (int i = 0; i < count; i++)
                {
                    GameObject gon = GameObject.Instantiate(emptyGO, nFirstShouPaiTrans.position + (int)((index + i) + (index + i) / 13) * nOffsetX, nShouPaiQua, nShouPaiContainerTrans) as GameObject;
                    shoupaiNList.Add(gon);
                }
                break;

        }

    }



    /// <summary>
    /// 根据方位生成摸牌，参数可以更改，暂定Int类型（可更改为方位类型）
    /// </summary>
    /// <param name="i">1，右侧，2左侧，3对面</param>
    public void MoPai(FW fw)
    {


        switch ((int)fw)
        {
            case 1:
                GameObject goe = GameObject.Instantiate(emptyGO, shoupaiEList[shoupaiEList.Count - 1].transform.position + eOffsetX * 2, eShouPaiQua, eShouPaiContainerTrans) as GameObject;
                goe.layer = 10;
                shoupaiEList.Add(goe);
                break;
            case 2:
                GameObject gow = GameObject.Instantiate(emptyGO, shoupaiWList[shoupaiWList.Count - 1].transform.position + wOffsetX * 2, wShouPaiQua, wShouPaiContainerTrans) as GameObject;
                gow.layer = 9;
                shoupaiWList.Add(gow);
                break;
            case 3:
                GameObject gon = GameObject.Instantiate(emptyGO, shoupaiNList[shoupaiNList.Count - 1].transform.position + nOffsetX * 2, nShouPaiQua, nShouPaiContainerTrans) as GameObject;
                shoupaiNList.Add(gon);
                break;
        }
    }
    /// <summary>
    /// 根据方位,花色，牌ID生3d成出牌，手牌区随机减少一张，将减少的牌移动到弃牌区，同时将摸牌移动到空缺处（也可判断是否打出的是否是摸的牌，）
    /// </summary>
    /// <param name="i">1,右侧，2左侧，3对面</param>
    public void ChuPai(FW fw, int hs, int id, bool isConn = false)
    {
        switch ((int)fw)
        {
            case 1:
                if (isConn)
                {
                    GameObject goe = GameObject.Instantiate(emptyGO, eFirstQiPaiTrans.position + (qipaiEList.Count % 7) * eOffsetX + (qipaiEList.Count / 7) * eQiOffsetZ, ePengQiPaiQua, eQiPaiContainerTrans) as GameObject;
                    goe.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
                    goe.GetComponent<MJ_SP>().HS = hs;
                    goe.GetComponent<MJ_SP>().ID = id;
                    qipaiEList.Add(goe);
                }
                else
                {
                    int j = 0;
                    if (shoupaiEList.Count > 2)
                    {
                        //弃牌区集合以及牌的操作
                        j = Random.Range(1, shoupaiEList.Count - 1);//产生一个随机牌的下标，不算最新摸的牌
                    }
                    else
                    {
                        j = shoupaiEList.Count - 1;
                    }
                    shoupaiEList[j].GetComponent<MJ_SP>().HS = hs;//接下来代码是，根据方法传的参数，生成出牌牌面
                    shoupaiEList[j].GetComponent<MJ_SP>().ID = id;
                    shoupaiEList[j].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

                    qipaiEList.Add(shoupaiEList[j]);//将这张牌添加到弃牌集合
                    int k = qipaiEList.Count - 1;//获得弃牌区牌偏移的数目
                    qipaiEList[k].layer = 0;
                    //qipaiEList[k].transform.DOMove(eFirstQiPaiTrans.position + (k % 7) * eOffsetX + (k / 7) * eQiOffsetZ, moveTime);//移动到弃牌位置
                    qipaiEList[k].AddComponent<FICMyTween>().Move(eFirstQiPaiTrans.position + (k % 7) * eOffsetX + (k / 7) * eQiOffsetZ, moveTime);
                    qipaiEList[k].transform.DORotate(ePengQiPaiQua.eulerAngles, moveTime);//移动过程中旋转
                    //StartCoroutine(ForceAllPaiPosition(FW.East,k));
                    qipaiEList[k].transform.SetParent(eQiPaiContainerTrans);
                    if (shoupaiEList.Count > 2)
                    {
                        //手牌区集合以及牌的操作
                        shoupaiEList.RemoveAt(j);
                        shoupaiEList.Add(emptyGO);
                        for (int l = shoupaiEList.Count; l > j + 1; l--)
                        {
                            shoupaiEList[l - 1] = shoupaiEList[l - 2];
                        }
                        shoupaiEList[j] = shoupaiEList[shoupaiEList.Count - 1];
                        shoupaiEList.RemoveAt(shoupaiEList.Count - 1);//以上为打一张牌之后，将最后一张牌放到刚刚打出牌的数组所在的位置
                                                                      //shoupaiEList[j].transform.DOMove(shoupaiEList[0].transform.position + j * eOffsetX, 2f);

                        shoupaiEList[j].transform.DOMoveZ(shoupaiEList[0].transform.position.z + j * eOffsetX.z, 0.5f);
                        shoupaiEList[j].transform.DOMoveY(shoupaiEList[0].transform.position.y + emptyGO.transform.position.y, 0.1f);
                        shoupaiEList[j].transform.DOMoveY(shoupaiEList[0].transform.position.y, 0.3f).SetDelay(0.1f);//
                    }
                    else
                    {
                        shoupaiEList.RemoveAt(shoupaiEList.Count - 1);
                    }
                    ResetDiamondPosition(eFirstQiPaiTrans.position + (k % 7) * eOffsetX + (k / 7) * eQiOffsetZ);
                }

                break;
            case 2:
                if (isConn)
                {
                    GameObject gow = GameObject.Instantiate(emptyGO, wFirstQiPaiTrans.position + (qipaiWList.Count % 7) * wOffsetX + (qipaiWList.Count / 7) * wQiOffsetZ, wPengQiPaiQua, wQiPaiContainerTrans) as GameObject;
                    gow.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
                    gow.GetComponent<MJ_SP>().HS = hs;
                    gow.GetComponent<MJ_SP>().ID = id;
                    qipaiWList.Add(gow);
                }
                else
                {
                    int j = 0;
                    if (shoupaiWList.Count > 2)
                    {
                        //弃牌区集合以及牌的操作
                        j = Random.Range(1, shoupaiWList.Count - 1);//产生一个随机牌的下标，不算最新摸的牌
                    }
                    else
                    {
                        j = shoupaiWList.Count - 1;
                    }
                    shoupaiWList[j].GetComponent<MJ_SP>().HS = hs;//接下来代码是，根据方法传的参数，生成出牌牌面
                    shoupaiWList[j].GetComponent<MJ_SP>().ID = id;
                    shoupaiWList[j].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

                    qipaiWList.Add(shoupaiWList[j]);//将这张牌添加到弃牌集合
                    int k = qipaiWList.Count - 1;//获得弃牌区牌偏移的数目
                    qipaiWList[k].layer = 0;
                    //qipaiWList[k].transform.DOMove(wFirstQiPaiTrans.position + (k % 7) * wOffsetX + (k / 7) * wQiOffsetZ, moveTime);//移动到弃牌位置
                    qipaiWList[k].AddComponent<FICMyTween>().Move(wFirstQiPaiTrans.position + (k % 7) * wOffsetX + (k / 7) * wQiOffsetZ, moveTime);
                    qipaiWList[k].transform.DORotate(wPengQiPaiQua.eulerAngles, moveTime);//移动过程中旋转
                    //StartCoroutine(ForceAllPaiPosition(FW.West,k));
                    qipaiWList[k].transform.SetParent(wQiPaiContainerTrans);
                    if (shoupaiWList.Count > 2)
                    {
                        //手牌区集合以及牌的操作
                        shoupaiWList.RemoveAt(j);
                        shoupaiWList.Add(emptyGO);
                        for (int l = shoupaiWList.Count; l > j + 1; l--)
                        {
                            shoupaiWList[l - 1] = shoupaiWList[l - 2];
                        }
                        shoupaiWList[j] = shoupaiWList[shoupaiWList.Count - 1];
                        shoupaiWList.RemoveAt(shoupaiWList.Count - 1);//以上为打一张牌之后，将最后一张牌放到刚刚打出牌的数组所在的位置
                                                                      //shoupaiWList[j].transform.DOMove(shoupaiWList[0].transform.position + j * wOffsetX, 2f);
                        shoupaiWList[j].transform.DOMoveZ(shoupaiWList[0].transform.position.z + j * wOffsetX.z, 1f);
                        shoupaiWList[j].transform.DOMoveY(shoupaiWList[0].transform.position.y + emptyGO.transform.position.y, 0.5f);
                        shoupaiWList[j].transform.DOMoveY(shoupaiWList[0].transform.position.y, 0.5f).SetDelay(0.5f);
                    }
                    else
                    {
                        shoupaiWList.RemoveAt(shoupaiWList.Count - 1);
                    }

                    ResetDiamondPosition(wFirstQiPaiTrans.position + (k % 7) * wOffsetX + (k / 7) * wQiOffsetZ);
                }

                break;
            case 3:
                //二人麻将，与其他情况不同，弃牌区需加至13张，现做代码修改，后期需要在修改场景，代码优化什么的以后做吧

                if (GameInfo.room_peo != 2)
                {
                    if (isConn)
                    {
                        GameObject gon = GameObject.Instantiate(emptyGO, nFirstQiPaiTrans.position + (qipaiNList.Count % 7) * nOffsetX + (qipaiNList.Count / 7) * nQiOffsetZ, nPengQiPaiQua, nQiPaiContainerTrans) as GameObject;
                        gon.transform.localPosition += Vector3.forward * 0.4f;
                        gon.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
                        gon.GetComponent<MJ_SP>().HS = hs;
                        gon.GetComponent<MJ_SP>().ID = id;
                        qipaiNList.Add(gon);
                    }
                    else
                    {
                        int j = 0;
                        if (shoupaiNList.Count > 2)
                        {

                            //弃牌区集合以及牌的操作
                            j = Random.Range(1, shoupaiNList.Count - 1);//产生一个随机牌的下标，不算最新摸的牌
                        }
                        else
                        {

                            j = shoupaiNList.Count - 1;
                        }
                        shoupaiNList[j].GetComponent<MJ_SP>().HS = hs;
                        shoupaiNList[j].GetComponent<MJ_SP>().ID = id;
                        shoupaiNList[j].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

                        qipaiNList.Add(shoupaiNList[j]);//将这张牌添加到弃牌集合
                        int k = qipaiNList.Count - 1;//获得弃牌区牌偏移的数目
                        //qipaiNList[k].transform.DOMove((nFirstQiPaiTrans.position + Vector3.up * 0.4f) + (k % 7) * nOffsetX + (k / 7) * nQiOffsetZ, moveTime);//移动到弃牌位置
                        qipaiNList[k].AddComponent<FICMyTween>().Move((nFirstQiPaiTrans.position + Vector3.up * 0.4f) + (k % 7) * nOffsetX + (k / 7) * nQiOffsetZ, moveTime);
                        qipaiNList[k].transform.localEulerAngles = new Vector3(0, 180, 0);
                         qipaiNList[k].transform.DORotate(nPengQiPaiQua.eulerAngles, moveTime);//移动过程中旋转
                        //StartCoroutine(ForceAllPaiPosition(FW.North,k));
                        qipaiNList[k].transform.SetParent(nQiPaiContainerTrans);
                        //qipaiNList[k].transform.position += Vector3.forward * 0.4f;
                        if (shoupaiNList.Count > 2)
                        {
                            //手牌区集合以及牌的操作
                            shoupaiNList.RemoveAt(j);

                            shoupaiNList.Add(emptyGO);
                            for (int l = shoupaiNList.Count; l > j; l--)
                            {
                                shoupaiNList[l - 1] = shoupaiNList[l - 2];
                            }
                            shoupaiNList[j] = shoupaiNList[shoupaiNList.Count - 1];
                            shoupaiNList.RemoveAt(shoupaiNList.Count - 1);//以上为打一张牌之后，将最后一张牌放到刚刚打出牌的数组所在的位置
                                                                          //shoupaiNList[j].transform.DOMove(shoupaiNList[0].transform.position + j * nOffsetX, 2f);
                            shoupaiNList[j].transform.DOMoveX(shoupaiNList[0].transform.position.x + j * nOffsetX.x, 1f);
                            shoupaiNList[j].transform.DOMoveY(shoupaiNList[0].transform.position.y + emptyGO.transform.position.y, 0.5f);
                            shoupaiNList[j].transform.DOMoveY(shoupaiNList[0].transform.position.y, 0.5f).SetDelay(0.5f);

                        }
                        else
                        {

                            shoupaiNList.RemoveAt(shoupaiNList.Count - 1);
                        }
                        ResetDiamondPosition(nFirstQiPaiTrans.position + (k % 7) * nOffsetX + (k / 7) * nQiOffsetZ);

                    }
                }
                else
                {
                    if (isConn)
                    {
                        GameObject gon = GameObject.Instantiate(emptyGO, (nFirstQiPaiTrans.position - 3 * nOffsetX) + (qipaiNList.Count % 13) * nOffsetX + (qipaiNList.Count / 13) * nQiOffsetZ, nPengQiPaiQua, nQiPaiContainerTrans) as GameObject;
                        gon.transform.localPosition += Vector3.forward * 0.4f;
                        gon.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
                        gon.GetComponent<MJ_SP>().HS = hs;
                        gon.GetComponent<MJ_SP>().ID = id;
                        qipaiNList.Add(gon);
                    }
                    else
                    {
                        int j = 0;
                        if (shoupaiNList.Count > 2)
                        {

                            //弃牌区集合以及牌的操作
                            j = Random.Range(1, shoupaiNList.Count - 1);//产生一个随机牌的下标，不算最新摸的牌
                        }
                        else
                        {

                            j = shoupaiNList.Count - 1;
                        }
                        shoupaiNList[j].GetComponent<MJ_SP>().HS = hs;
                        shoupaiNList[j].GetComponent<MJ_SP>().ID = id;
                        shoupaiNList[j].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

                        qipaiNList.Add(shoupaiNList[j]);//将这张牌添加到弃牌集合
                        int k = qipaiNList.Count - 1;//获得弃牌区牌偏移的数目
                                                     //qipaiNList[k].transform.DOMove((nFirstQiPaiTrans.position + Vector3.up * 0.4f - 3 * nOffsetX) + (k % 13) * nOffsetX + (k / 13) * nQiOffsetZ, moveTime);//移动到弃牌位置
                        qipaiNList[k].AddComponent<FICMyTween>().Move((nFirstQiPaiTrans.position + Vector3.up * 0.4f - 3 * nOffsetX) + (k % 13) * nOffsetX + (k / 13) * nQiOffsetZ, moveTime);                                                                                                                                                                                                                                                                                     //2016.8.6劉磊改

                        qipaiNList[k].transform.localEulerAngles = new Vector3(0, 180, 0);
                        
                        //qipaiNList[k].transform.DORotate(nPengQiPaiQua.eulerAngles, moveTime);//移动过程中旋转
                        //StartCoroutine(ForceAllPaiPosition(FW.North,k));
                        qipaiNList[k].transform.SetParent(nQiPaiContainerTrans);
                        //qipaiNList[k].transform.position += Vector3.forward * 0.4f;
                        if (shoupaiNList.Count > 2)
                        {
                            //手牌区集合以及牌的操作
                            shoupaiNList.RemoveAt(j);

                            shoupaiNList.Add(emptyGO);
                            for (int l = shoupaiNList.Count; l > j; l--)
                            {
                                shoupaiNList[l - 1] = shoupaiNList[l - 2];
                            }
                            shoupaiNList[j] = shoupaiNList[shoupaiNList.Count - 1];
                            shoupaiNList.RemoveAt(shoupaiNList.Count - 1);//以上为打一张牌之后，将最后一张牌放到刚刚打出牌的数组所在的位置
                                                                          //shoupaiNList[j].transform.DOMove(shoupaiNList[0].transform.position + j * nOffsetX, 2f);
                            shoupaiNList[j].transform.DOMoveX(shoupaiNList[0].transform.position.x + j * nOffsetX.x, 1f);
                            shoupaiNList[j].transform.DOMoveY(shoupaiNList[0].transform.position.y + emptyGO.transform.position.y, 0.5f);
                            shoupaiNList[j].transform.DOMoveY(shoupaiNList[0].transform.position.y, 0.5f).SetDelay(0.5f);

                        }
                        else
                        {

                            shoupaiNList.RemoveAt(shoupaiNList.Count - 1);
                        }
                        ResetDiamondPosition((nFirstQiPaiTrans.position - 3 * nOffsetX) + (k % 13) * nOffsetX + (k / 13) * nQiOffsetZ);
                    }

                }

                break;
            case 4:
                //二人麻将与其他玩法不同，弃牌区每排多生成6张弃牌，现代码更改，后期不符合要求再对场景进行添加更改

                myCards.QiPai(hs,id);

                break;
        }

    }
    /// <summary>
    /// 强制改正所有牌的位置，现在修改弃牌位置
    /// </summary>
    /// <param name="fw"></param>
    IEnumerator ForceAllPaiPosition(FW fw,int num)
    {
        print(num);
        yield return new WaitForSeconds(0.2f);
        switch (fw)
        {
            case FW.East:
                if (qipaiEList[num].transform.position != eFirstQiPaiTrans.position + (num % 7) * eOffsetX + (num / 7) * eQiOffsetZ || qipaiEList[num].transform.rotation != ePengQiPaiQua)
                {
                    qipaiEList[num].transform.rotation = ePengQiPaiQua;
                    qipaiEList[num].transform.position = eFirstQiPaiTrans.position + (num % 7) * eOffsetX + (num / 7) * eQiOffsetZ;
                }
                break;
            case FW.West:
                if (qipaiWList[num].transform.position != wFirstQiPaiTrans.position + (num % 7) * wOffsetX + (num / 7) * wQiOffsetZ || qipaiWList[num].transform.rotation != wPengQiPaiQua)
                {
                    qipaiWList[num].transform.rotation = wPengQiPaiQua;
                    qipaiWList[num].transform.position = wFirstQiPaiTrans.position + (num % 7) * wOffsetX + (num / 7) * wQiOffsetZ;
                }
                break;
            case FW.North:
                if (GameInfo.room_peo != 2)
                {
                    if (qipaiNList[num].transform.position != nFirstQiPaiTrans.position + Vector3.up * 0.4f + (num % 7) * nOffsetX + (num / 7) * nQiOffsetZ || qipaiNList[num].transform.rotation != nPengQiPaiQua)
                    {
                        qipaiNList[num].transform.rotation = nPengQiPaiQua;
                        qipaiNList[num].transform.position = nFirstQiPaiTrans.position + (num % 7) * nOffsetX + (num / 7) * nQiOffsetZ;
                    }
                }
                else
                {
                    if (qipaiNList[num].transform.position != nFirstQiPaiTrans.position + Vector3.up * 0.4f - 3 * nOffsetX + (num % 13) * nOffsetX + (num / 13) * nQiOffsetZ || qipaiNList[num].transform.rotation != nPengQiPaiQua)
                    {
                        qipaiNList[num].transform.rotation = nPengQiPaiQua;
                        qipaiNList[num].transform.position = nFirstQiPaiTrans.position + (num % 13) * nOffsetX + (num / 13) * nQiOffsetZ;
                    }

                }
                break;
        }
    }
    /// <summary>
    /// 重新定位指示钻石的位置，调用了8次，你个傻逼
    /// </summary>
    /// <param name="referTO"></param>
    public void ResetDiamondPosition(Vector3 referTO)
    {
        diamond.transform.position = referTO + new Vector3(0, 0.1f, 0);
    }

    /// <summary>
    /// 碰牌出牌，直接将最后一张手牌移动到弃牌区，如果位置不够，再调整为第一张好了，
    /// </summary>
    /// <param name="i"></param>
    //public void PengPaiChuPai(FW fw, int hs, int id)
    //{
    //    switch ((int)fw)
    //    {
    //        case 1:
    //            qipaiEList.Add(shoupaiEList[shoupaiEList.Count - 1]);//将手牌最后一张牌添加到弃牌集合

    //            int k = qipaiEList.Count - 1;//获得弃牌区牌的数目

    //            qipaiEList[k].GetComponent<MJ_SP>().HS = hs;
    //            qipaiEList[k].GetComponent<MJ_SP>().ID = id;
    //            qipaiEList[k].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

    //            //Texture a=  qipaiEList[k].transform.FindChild("Mian").GetComponent<MeshRenderer>().material.mainTexture;

    //            qipaiEList[k].transform.DOMove(eFirstQiPaiTrans.position + (k % 7) * eOffsetX + (k / 7) * eQiOffsetZ, moveTime);//移动到弃牌位置
    //            qipaiEList[k].transform.DORotate(new Vector3(-90, -90, 0), moveTime);//移动过程中旋转
    //            qipaiEList[k].transform.SetParent(eQiPaiContainerTrans);
    //            shoupaiEList.RemoveAt(shoupaiEList.Count - 1);
    //            ResetDiamondPosition(eFirstQiPaiTrans.position + (k % 7) * eOffsetX + (k / 7) * eQiOffsetZ);
    //            break;
    //        case 2:
    //            qipaiWList.Add(shoupaiWList[shoupaiWList.Count - 1]);//将手牌最后一张牌添加到弃牌集合
    //            k = qipaiWList.Count - 1;//获得弃牌区牌的数目


    //            qipaiWList[k].GetComponent<MJ_SP>().HS = hs;
    //            qipaiWList[k].GetComponent<MJ_SP>().ID = id;
    //            qipaiWList[k].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

    //            qipaiWList[k].transform.DOMove(wFirstQiPaiTrans.position + (k % 7) * wOffsetX + (k / 7) * wQiOffsetZ, moveTime);//移动到弃牌位置
    //            qipaiWList[k].transform.DORotate(new Vector3(-90, 90, 0), moveTime);//移动过程中旋转
    //            qipaiWList[k].transform.SetParent(wQiPaiContainerTrans);
    //            shoupaiWList.RemoveAt(shoupaiWList.Count - 1);
    //            ResetDiamondPosition(wFirstQiPaiTrans.position + (k % 7) * wOffsetX + (k / 7) * wQiOffsetZ);
    //            break;
    //        case 3:
    //            qipaiNList.Add(shoupaiNList[shoupaiNList.Count - 1]);//将手牌最后一张牌添加到弃牌集合
    //            k = qipaiNList.Count - 1;//获得弃牌区牌的数目


    //            qipaiNList[k].GetComponent<MJ_SP>().HS = hs;
    //            qipaiNList[k].GetComponent<MJ_SP>().ID = id;
    //            qipaiNList[k].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

    //            qipaiNList[k].transform.DOMove(nFirstQiPaiTrans.position + (k % 7) * nOffsetX + (k / 7) * nQiOffsetZ, moveTime);//移动到弃牌位置
    //            qipaiNList[k].transform.DORotate(new Vector3(-90, 180, 0), moveTime);//移动过程中旋转
    //            qipaiNList[k].transform.SetParent(nQiPaiContainerTrans);
    //            shoupaiNList.RemoveAt(shoupaiNList.Count - 1);
    //            ResetDiamondPosition(nFirstQiPaiTrans.position + (k % 7) * nOffsetX + (k / 7) * nQiOffsetZ);
    //            break;
    //    }
    //}

    /// <summary>
    /// 胡牌，所有人亮手牌
    /// </summary>
    public void HuPai()
    {
        
        
        for (int i = 0; i < shoupaiEList.Count; i++)
        {
            shoupaiEList[i].transform.DORotate(ePengQiPaiQua.eulerAngles, 0.2f);
        }
        for (int i = 0; i < shoupaiWList.Count; i++)
        {
            shoupaiWList[i].transform.DORotate(wPengQiPaiQua.eulerAngles, 0.2f);
        }
        for (int i = 0; i < shoupaiNList.Count; i++)
        {
            shoupaiNList[i].transform.DORotate(nPengQiPaiQua.eulerAngles, 0.2f);
            //shoupaiNList[i].GetComponent<Renderer>().material = _LuceMaterial;
            shoupaiNList[i].GetComponent<Renderer>().material.shader = _LucenShader;
        }
    }
    /// <summary>
    /// 根据，碰牌人方位，碰的牌的方位，牌的花色以及牌的类型，在碰牌区生成牌，数量，花色，位置，转向
    /// </summary>
    /// <param name="fw">碰牌人方位</param>
    /// <param name="pfw">碰的牌的方位</param>
    /// <param name="hs">牌的花色</param>
    /// <param name="pptype">牌的类型</param>
    /// <param name="isNotConn"></param>
    /// <param name="type"></param>
    public void PengGangPai(FW fw, FW pfw, int hs, PPTYPE pptype, bool isNotConn = true, string type = "")
    {
        switch (fw)
        {
            case FW.East:
                switch (pptype)
                {
                    case PPTYPE.Peng:
                        if (isNotConn)
                        {
                            GameObject.Destroy(shoupaiEList[0]);
                            GameObject.Destroy(shoupaiEList[1]);
                            shoupaiEList.RemoveAt(0);
                            shoupaiEList.RemoveAt(0);
                        }
                        for (int k = 0; k < 3; k++)
                        {
                            GameObject goe = GameObject.Instantiate(emptyGO, eNextPos, ePengQiPaiQua) as GameObject;
                            eNextPos += eOffsetX;
                            pengpaiEList.Add(goe);
                            pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);

                            EPengPaiPaiMian(hs);
                        }
                        eNextPos += (eOffsetLX - eOffsetX);
                        ///20170712路遥添加
                    
                        break;
                    case PPTYPE.Gang:
                        if (fw != pfw || type.Equals(GangType.mingGang, System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (isNotConn)
                            {
                                GameObject.Destroy(shoupaiEList[0]);
                                GameObject.Destroy(shoupaiEList[1]);
                                //GameObject.Destroy(shoupaiEList[2]);
                                //shoupaiEList.RemoveAt(0);
                                shoupaiEList.RemoveAt(0);
                                shoupaiEList.RemoveAt(0);
                            }
                            switch (pfw)
                            {
                                case FW.West:
                                    GameObject goe = GameObject.Instantiate(emptyGO, eNextPos, ePengQiPaiQua) as GameObject;
                                    eNextPos += eOffsetX;
                                    pengpaiEList.Add(goe);
                                    pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);
                                    EPengPaiPaiMian(hs);
                                    goe = GameObject.Instantiate(emptyGO, eNextPos, ePengQiPaiQua) as GameObject;
                                    eNextPos -= eQiOffsetZ;
                                    pengpaiEList.Add(goe);
                                    pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);
                                    EPengPaiPaiMian(hs);
                                    goe = GameObject.Instantiate(emptyGO, eNextPos, eGangPaiQua) as GameObject;
                                    eNextPos += eQiOffsetZ;
                                    pengpaiEList.Add(goe);
                                    pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);
                                    EPengPaiPaiMian(hs);
                                    eNextPos += eOffsetX;
                                    goe = GameObject.Instantiate(emptyGO, eNextPos, ePengQiPaiQua) as GameObject;
                                    eNextPos += eOffsetLX;
                                    pengpaiEList.Add(goe);
                                    pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);
                                    EPengPaiPaiMian(hs);
                                    eisNorth = false;

                               
                                    break;
                                case FW.North:
                                    for (int k = 0; k < 3; k++)
                                    {
                                        goe = GameObject.Instantiate(emptyGO, eNextPos, ePengQiPaiQua) as GameObject;
                                        eNextPos += eOffsetX;
                                        pengpaiEList.Add(goe);
                                        pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);

                                        EPengPaiPaiMian(hs);
                                    }
                                    eNextPos += (eOffsetLX - eOffsetX);
                                    goe = GameObject.Instantiate(emptyGO, eNextPos, eGangPaiQua) as GameObject;
                                    eNextPos += eOffsetLX;
                                    pengpaiEList.Add(goe);
                                    pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);
                                    EPengPaiPaiMian(hs);
                                    eNextPos += (eOffsetLX - eOffsetX);
                                    eisNorth = true;
                                 
                                    break;


                                case FW.South:
                                    eNextPos += (eOffsetLX - eOffsetX);
                                    goe = GameObject.Instantiate(emptyGO, eNextPos, eGangPaiQua) as GameObject;
                                    eNextPos += eOffsetLX;
                                    pengpaiEList.Add(goe);
                                    pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);
                                    EPengPaiPaiMian(hs);
                                    for (int k = 0; k < 3; k++)
                                    {
                                        goe = GameObject.Instantiate(emptyGO, eNextPos, ePengQiPaiQua) as GameObject;
                                        eNextPos += eOffsetX;
                                        pengpaiEList.Add(goe);
                                        pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);

                                        EPengPaiPaiMian(hs);
                                    }
                                    eNextPos += (eOffsetLX - eOffsetX);
                                    eisNorth = false;
                                
                                    break;
                            }
                        }
                        else
                        {
                            if (type.Equals(GangType.anGang, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (isNotConn)
                                {
                                    GameObject.Destroy(shoupaiEList[0]);
                                    GameObject.Destroy(shoupaiEList[1]);
                                    GameObject.Destroy(shoupaiEList[2]);
                                    //GameObject.Destroy(shoupaiEList[3]);
                                    //shoupaiEList.RemoveAt(0);
                                    shoupaiEList.RemoveAt(0);
                                    shoupaiEList.RemoveAt(0);
                                    shoupaiEList.RemoveAt(0);
                                }
                                for (int k = 0; k < 2; k++)
                                {
                                    GameObject goe = GameObject.Instantiate(emptyGO, eNextPos, ePengQiPaiQua) as GameObject;
                                    eNextPos += eOffsetX;
                                    pengpaiEList.Add(goe);
                                    pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);

                                    EPengPaiPaiMian(hs);
                                }
                                for (int k = 0; k < 2; k++)
                                {
                                    GameObject goe = GameObject.Instantiate(emptyGO, eNextPos, ePengQiPaiQua) as GameObject;
                                    goe.transform.localEulerAngles = new Vector3(-90, 0, -90);
                                    goe.transform.position -= Vector3.up * 0.4f;
                                    goe.GetComponent<MJ_SP>().HS = hs;
                                    eNextPos += eOffsetX;
                                    pengpaiEList.Add(goe);
                                    pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);
                                    EPengPaiPaiMian(hs);
                                }

                                eNextPos += (eOffsetLX - eOffsetX);
                              
                            }
                            else if (type.Equals(GangType.zhuanWanGang, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                //GameObject.Destroy(shoupaiEList[0]);
                                //shoupaiEList.RemoveAt(0);
                                int count = 0;
                                for (int i = 0; i < pengpaiEList.Count; i++)
                                {
                                    if (hs == pengpaiEList[i].gameObject.GetComponent<MJ_SP>().HS)
                                    {
                                        count++;
                                        if (count == 3)
                                        {
                                            GameObject goe = GameObject.Instantiate(emptyGO, pengpaiEList[i].transform.position + eOffsetX, ePengQiPaiQua) as GameObject;
                                            //goe.transform.position += new Vector3(0, 0.8f, 0);
                                            goe.transform.localEulerAngles = new Vector3(-90, 0, -90);
                                            goe.transform.position -= Vector3.up * 0.4f;
                                            goe.GetComponent<MJ_SP>().HS = hs;
                                            pengpaiEList.Add(emptyGO);
                                            for (int l = pengpaiEList.Count; l > i + 2; l--)
                                            {
                                                pengpaiEList[l - 1] = pengpaiEList[l - 2];
                                                pengpaiEList[l - 2].transform.position += eOffsetX;
                                            }
                                            eNextPos += eOffsetX;
                                        
                                            pengpaiEList[i + 1] = goe;
                                            pengpaiEList[i + 1].transform.SetParent(ePengGangPaiContainerTrans);
                                            count = 0;
                                        }
                                    }

                                }
                            }
                            else if (type.Equals(GangType.hanBaoGang, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                //   pengpaiEList.Find(w => w.GetComponent<MJ_SP>().HS == hs) == null ||
                                // pengpaiEList.Find(w => w.GetComponent<MJ_SP>().HS == hs) != null ||
                                if (pengpaiEList.Find(w => w.GetComponent<MJ_SP>().HS == hs) == null)
                                {//憨包杠之暗杠转憨包
                                    if (isNotConn)
                                    {
                                        GameObject.Destroy(shoupaiEList[0]);
                                        GameObject.Destroy(shoupaiEList[1]);
                                        GameObject.Destroy(shoupaiEList[2]);
                                        //GameObject.Destroy(shoupaiEList[3]);
                                        //shoupaiEList.RemoveAt(0);
                                        shoupaiEList.RemoveAt(0);
                                        shoupaiEList.RemoveAt(0);
                                        shoupaiEList.RemoveAt(0);
                                    }
                                    for (int k = 0; k < 4; k++)
                                    {
                                        GameObject goe = GameObject.Instantiate(emptyGO, eNextPos, ePengQiPaiQua) as GameObject;
                                        eNextPos += eOffsetX;
                                        pengpaiEList.Add(goe);
                                        pengpaiEList[pengpaiEList.Count - 1].transform.SetParent(ePengGangPaiContainerTrans);
                                        EPengPaiPaiMian(hs);
                                    }

                                    eNextPos += (eOffsetLX - eOffsetX);
                                 
                                }
                                else
                                {//憨包杠之转弯杠转憨包

                                    int count = pengpaiEList.FindLastIndex(u => u.gameObject.GetComponent<MJ_SP>().HS == hs);
                                    if (count > 0)
                                    {
                                        GameObject goe = GameObject.Instantiate(emptyGO, pengpaiEList[count].transform.position + eOffsetX, ePengQiPaiQua) as GameObject;

                                        pengpaiEList.Add(goe);
                                        EPengPaiPaiMian(hs);
                                        for (int l = pengpaiEList.Count; l > count + 2; l--)
                                        {
                                            pengpaiEList[l - 1] = pengpaiEList[l - 2];
                                            pengpaiEList[l - 2].transform.position += eOffsetX;
                                        }
                                        eNextPos += eOffsetX;
                                        ///...待添加
                                   
                                        pengpaiEList[count + 1] = goe;
                                        pengpaiEList[count + 1].transform.SetParent(ePengGangPaiContainerTrans);
                                        count = 0;

                                    }

                                }


                            }
                        }
                        break;
                }
                break;
            case FW.West:
                switch (pptype)
                {
                    case PPTYPE.Peng:
                        if (isNotConn)
                        {
                            GameObject.Destroy(shoupaiWList[0]);
                            GameObject.Destroy(shoupaiWList[1]);
                            shoupaiWList.RemoveAt(0);
                            shoupaiWList.RemoveAt(0);
                        }
                        for (int k = 0; k < 3; k++)
                        {
                            GameObject gow = GameObject.Instantiate(emptyGO, wNextPos, wPengQiPaiQua) as GameObject;
                            wNextPos += wOffsetX;
                            pengpaiWList.Add(gow);
                            pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);

                            int j = pengpaiWList.Count - 1;
                            pengpaiWList[j].GetComponent<MJ_SP>().HS = hs;
                            pengpaiWList[j].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
                        }
                        wNextPos += (wOffsetLX - wOffsetX);
                       
                        break;
                    case PPTYPE.Gang:
                        if (fw != pfw || type.Equals(GangType.mingGang, System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (isNotConn)
                            {
                                GameObject.Destroy(shoupaiWList[0]);
                                GameObject.Destroy(shoupaiWList[1]);
                                //GameObject.Destroy(shoupaiWList[2]);
                                //shoupaiWList.RemoveAt(0);
                                shoupaiWList.RemoveAt(0);
                                shoupaiWList.RemoveAt(0);
                            }
                            switch (pfw)
                            {
                                case FW.East:
                                    GameObject gow = GameObject.Instantiate(emptyGO, wNextPos, wPengQiPaiQua) as GameObject;
                                    wNextPos += wOffsetX;
                                    pengpaiWList.Add(gow);
                                    pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);
                                    WPengPaiPaiMian(hs);
                                    gow = GameObject.Instantiate(emptyGO, wNextPos, wPengQiPaiQua) as GameObject;
                                    wNextPos -= wQiOffsetZ;
                                    pengpaiWList.Add(gow);
                                    pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);
                                    WPengPaiPaiMian(hs);
                                    gow = GameObject.Instantiate(emptyGO, wNextPos, wGangPaiQua) as GameObject;
                                    wNextPos += wQiOffsetZ;
                                    pengpaiWList.Add(gow);
                                    pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);
                                    WPengPaiPaiMian(hs);
                                    wNextPos += wOffsetX;
                                    gow = GameObject.Instantiate(emptyGO, wNextPos, wPengQiPaiQua) as GameObject;
                                    wNextPos += wOffsetLX;
                                    pengpaiWList.Add(gow);
                                    pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);
                                    WPengPaiPaiMian(hs);
                                    wisSouth = false;
                                  
                                    break;
                                case FW.North:
                                    wNextPos += (wOffsetLX - wOffsetX);
                                    gow = GameObject.Instantiate(emptyGO, wNextPos, wGangPaiQua) as GameObject;
                                    wNextPos += wOffsetLX;
                                    pengpaiWList.Add(gow);
                                    pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);
                                    WPengPaiPaiMian(hs);
                                    for (int k = 0; k < 3; k++)
                                    {
                                        gow = GameObject.Instantiate(emptyGO, wNextPos, wPengQiPaiQua) as GameObject;
                                        wNextPos += wOffsetX;
                                        pengpaiWList.Add(gow);
                                        pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);

                                        WPengPaiPaiMian(hs);
                                    }
                                    wNextPos += (wOffsetLX - wOffsetX);
                                    wisSouth = false;
                              
                                    break;
                                case FW.South:
                                    for (int k = 0; k < 3; k++)
                                    {
                                        gow = GameObject.Instantiate(emptyGO, wNextPos, wPengQiPaiQua) as GameObject;
                                        wNextPos += wOffsetX;
                                        pengpaiWList.Add(gow);
                                        pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);

                                        WPengPaiPaiMian(hs);
                                    }
                                    wNextPos += (wOffsetLX - wOffsetX);
                                    gow = GameObject.Instantiate(emptyGO, wNextPos, wGangPaiQua) as GameObject;
                                    wNextPos += wOffsetLX;
                                    pengpaiWList.Add(gow);
                                    pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);
                                    WPengPaiPaiMian(hs);
                                    wNextPos += (wOffsetLX - wOffsetX);
                                    wisSouth = true;
                           
                                    break;
                            }
                        }
                        else
                        {
                            if (type.Equals(GangType.anGang, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (isNotConn)
                                {
                                    GameObject.Destroy(shoupaiWList[0]);
                                    GameObject.Destroy(shoupaiWList[1]);
                                    GameObject.Destroy(shoupaiWList[2]);
                                    //GameObject.Destroy(shoupaiWList[3]);
                                    //shoupaiWList.RemoveAt(0);
                                    shoupaiWList.RemoveAt(0);
                                    shoupaiWList.RemoveAt(0);
                                    shoupaiWList.RemoveAt(0);
                                }
                                for (int k = 0; k < 2; k++)
                                {
                                    GameObject gow = GameObject.Instantiate(emptyGO, wNextPos, wPengQiPaiQua) as GameObject;
                                    wNextPos += wOffsetX;
                                    pengpaiWList.Add(gow);
                                    pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);

                                    WPengPaiPaiMian(hs);
                                }
                                for (int k = 0; k < 2; k++)
                                {
                                    GameObject gow = GameObject.Instantiate(emptyGO, wNextPos, wPengQiPaiQua) as GameObject;
                                    gow.transform.localEulerAngles = new Vector3(-90, 0, 90);
                                    gow.transform.position -= Vector3.up * 0.4f;
                                    gow.GetComponent<MJ_SP>().HS = hs;
                                    wNextPos += wOffsetX;
                                    pengpaiWList.Add(gow);
                                    pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);
                                    WPengPaiPaiMian(hs);
                                }

                                wNextPos += (wOffsetLX - wOffsetX);
                             
                            }
                            else if (type.Equals(GangType.zhuanWanGang, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                //GameObject.Destroy(shoupaiWList[0]);
                                //shoupaiWList.RemoveAt(0);
                                int count = 0;
                                for (int i = 0; i < pengpaiWList.Count; i++)
                                {
                                    if (hs == pengpaiWList[i].gameObject.GetComponent<MJ_SP>().HS)
                                    {
                                        count++;
                                        if (count == 3)
                                        {
                                            GameObject gow = GameObject.Instantiate(emptyGO, pengpaiWList[i].transform.position + wOffsetX, wPengQiPaiQua) as GameObject;
                                            //gow.transform.position += new Vector3(0, 0.8f, 0);
                                            gow.GetComponent<MJ_SP>().HS = hs;
                                            gow.transform.localEulerAngles = new Vector3(-90, 0, 90);
                                            gow.transform.position -= Vector3.up * 0.4f;
                                            pengpaiWList.Add(emptyGO);
                                            for (int l = pengpaiWList.Count; l > i + 2; l--)
                                            {
                                                pengpaiWList[l - 1] = pengpaiWList[l - 2];
                                                pengpaiWList[l - 2].transform.position += wOffsetX;
                                            }
                                            wNextPos += wOffsetX;
                                         
                                            pengpaiWList[i + 1] = gow;
                                            pengpaiWList[i + 1].transform.SetParent(wPengGangPaiContainerTrans);
                                            count = 0;
                                        }
                                    }

                                }
                            }
                            else if (type.Equals(GangType.hanBaoGang, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                //   pengpaiEList.Find(w => w.GetComponent<MJ_SP>().HS == hs) == null ||
                                // pengpaiEList.Find(w => w.GetComponent<MJ_SP>().HS == hs) != null ||
                                if (pengpaiWList.Find(w => w.GetComponent<MJ_SP>().HS == hs) == null)
                                {//憨包杠之暗杠转憨包
                                    if (isNotConn)
                                    {
                                        GameObject.Destroy(shoupaiWList[0]);
                                        GameObject.Destroy(shoupaiWList[1]);
                                        GameObject.Destroy(shoupaiWList[2]);
                                        //GameObject.Destroy(shoupaiEList[3]);
                                        //shoupaiEList.RemoveAt(0);
                                        shoupaiWList.RemoveAt(0);
                                        shoupaiWList.RemoveAt(0);
                                        shoupaiWList.RemoveAt(0);
                                    }
                                    for (int k = 0; k < 4; k++)
                                    {
                                        GameObject gow = GameObject.Instantiate(emptyGO, wNextPos, wPengQiPaiQua) as GameObject;
                                        wNextPos += wOffsetX;
                                        pengpaiWList.Add(gow);
                                        pengpaiWList[pengpaiWList.Count - 1].transform.SetParent(wPengGangPaiContainerTrans);

                                        WPengPaiPaiMian(hs);
                                    }

                                    wNextPos += (wOffsetLX - wOffsetX);
                                
                                }
                                else
                                {//憨包杠之转弯杠杠转憨包

                                    int count = pengpaiWList.FindLastIndex(u => u.gameObject.GetComponent<MJ_SP>().HS == hs);
                                    if (count > 0)
                                    {


                                        GameObject gow = GameObject.Instantiate(emptyGO, pengpaiWList[count].transform.position + wOffsetX, wPengQiPaiQua) as GameObject;


                                        pengpaiWList.Add(gow);
                                        WPengPaiPaiMian(hs);
                                        for (int l = pengpaiWList.Count; l > count + 2; l--)
                                        {
                                            pengpaiWList[l - 1] = pengpaiWList[l - 2];
                                            pengpaiWList[l - 2].transform.position += wOffsetX;
                                        }
                                        wNextPos += wOffsetX;
                                        pengpaiWList[count + 1] = gow;
                                        pengpaiWList[count + 1].transform.SetParent(wPengGangPaiContainerTrans);
                                        count = 0;

                                   
                                    }
                                }
                            }
                            break;
                        }
                        break;
                }
                break;
            case FW.North:
                switch (pptype)
                {
                    case PPTYPE.Peng:
                        if (isNotConn)
                        {
                            GameObject.Destroy(shoupaiNList[0]);
                            GameObject.Destroy(shoupaiNList[1]);
                            shoupaiNList.RemoveAt(0);
                            shoupaiNList.RemoveAt(0);
                        }
                        for (int k = 0; k < 3; k++)
                        {
                            GameObject gon = GameObject.Instantiate(emptyGO, nNextPos, nPengQiPaiQua) as GameObject;
                            nNextPos += nOffsetX;
                            pengpaiNList.Add(gon);
                            pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);

                            int j = pengpaiNList.Count - 1;
                            pengpaiNList[j].GetComponent<MJ_SP>().HS = hs;
                            pengpaiNList[j].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
                            //pengpaiNList[j].GetComponent<Renderer>().material = _LuceMaterial;
                            pengpaiNList[j].GetComponent<Renderer>().material.shader = _LucenShader;
                        }
                        nNextPos += (nOffsetLX - nOffsetX);
                      
                        break;
                    case PPTYPE.Gang:
                        if (fw != pfw || type.Equals(GangType.mingGang, System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (isNotConn)
                            {
                                GameObject.Destroy(shoupaiNList[0]);
                                GameObject.Destroy(shoupaiNList[1]);
                                // GameObject.Destroy(shoupaiNList[2]);
                                //shoupaiNList.RemoveAt(0);
                                shoupaiNList.RemoveAt(0);
                                shoupaiNList.RemoveAt(0);
                                print(3);
                            }
                            switch (pfw)
                            {
                                case FW.East:
                                    nNextPos += (nOffsetLX - nOffsetX);
                                    GameObject gon = GameObject.Instantiate(emptyGO, nNextPos, nGangPaiQua) as GameObject;
                                    nNextPos += nOffsetLX;
                                    pengpaiNList.Add(gon);
                                    pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);
                                    NPengPaiPaiMian(hs);
                                    for (int k = 0; k < 3; k++)
                                    {
                                        gon = GameObject.Instantiate(emptyGO, nNextPos, nPengQiPaiQua) as GameObject;
                                        nNextPos += nOffsetX;
                                        pengpaiNList.Add(gon);
                                        pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);

                                        NPengPaiPaiMian(hs);
                                    }
                                    nNextPos += (nOffsetLX - nOffsetX);
                                 
                                    nisWest = false;
                                    break;
                                case FW.West:
                                    for (int k = 0; k < 3; k++)
                                    {
                                        gon = GameObject.Instantiate(emptyGO, nNextPos, nPengQiPaiQua) as GameObject;
                                        nNextPos += nOffsetX;
                                        pengpaiNList.Add(gon);
                                        pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);

                                        NPengPaiPaiMian(hs);
                                    }
                                    nNextPos += (nOffsetLX - nOffsetX);
                                    gon = GameObject.Instantiate(emptyGO, nNextPos, nGangPaiQua) as GameObject;
                                    nNextPos += nOffsetLX;
                                    pengpaiNList.Add(gon);
                                    pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);
                                    NPengPaiPaiMian(hs);
                                    nNextPos += (nOffsetLX - nOffsetX);
                                    nisWest = true;
                                
                                    break;
                                case FW.South:
                                    gon = GameObject.Instantiate(emptyGO, nNextPos, nPengQiPaiQua) as GameObject;
                                    nNextPos += nOffsetX;
                                    pengpaiNList.Add(gon);
                                    pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);
                                    NPengPaiPaiMian(hs);
                                    gon = GameObject.Instantiate(emptyGO, nNextPos, nPengQiPaiQua) as GameObject;
                                    nNextPos -= nQiOffsetZ;
                                    pengpaiNList.Add(gon);
                                    pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);
                                    NPengPaiPaiMian(hs);
                                    gon = GameObject.Instantiate(emptyGO, nNextPos, nGangPaiQua) as GameObject;
                                    nNextPos += nQiOffsetZ;
                                    pengpaiNList.Add(gon);
                                    pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);
                                    NPengPaiPaiMian(hs);
                                    nNextPos += nOffsetX;
                                    gon = GameObject.Instantiate(emptyGO, nNextPos, nPengQiPaiQua) as GameObject;
                                    nNextPos += nOffsetLX;
                                    pengpaiNList.Add(gon);
                                    pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);
                                    NPengPaiPaiMian(hs);
                                    nisWest = false;
                                    
                                    break;
                            }
                        }
                        else
                        {
                            if (type.Equals(GangType.anGang, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (isNotConn)
                                {
                                    GameObject.Destroy(shoupaiNList[0]);
                                    GameObject.Destroy(shoupaiNList[1]);
                                    GameObject.Destroy(shoupaiNList[2]);
                                    // GameObject.Destroy(shoupaiNList[3]);
                                    shoupaiNList.RemoveAt(0);
                                    shoupaiNList.RemoveAt(0);
                                    shoupaiNList.RemoveAt(0);
                                    //   shoupaiNList.RemoveAt(0);

                                }
                                for (int k = 0; k < 2; k++)
                                {
                                    GameObject gon = GameObject.Instantiate(emptyGO, nNextPos, nPengQiPaiQua) as GameObject;
                                    nNextPos += nOffsetX;
                                    pengpaiNList.Add(gon);
                                    pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);

                                    NPengPaiPaiMian(hs);
                                }
                                for (int k = 0; k < 2; k++)
                                {
                                    GameObject gon = GameObject.Instantiate(emptyGO, nNextPos, Quaternion.Euler(-90, 0, -180)) as GameObject;
                                    gon.transform.position += new Vector3(0, -0.4f, 0);
                                    //gon.GetComponent<MJ_SP>().HS = hs;
                                    nNextPos += nOffsetX;
                                    pengpaiNList.Add(gon);
                                    pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);
                                    NPengPaiPaiMian(hs);
                                }

                            


                            }
                            else if (type.Equals(GangType.zhuanWanGang, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                //GameObject.Destroy(shoupaiNList[0]);
                                //shoupaiNList.RemoveAt(0);
                                if (!isNotConn)
                                {
                                    GameObject go;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        go = GameObject.Instantiate(emptyGO, nNextPos, nPengQiPaiQua) as GameObject;
                                        nNextPos += nOffsetX;
                                        pengpaiNList.Add(go);
                                        pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);

                                        NPengPaiPaiMian(hs);
                                    }
                                    go = GameObject.Instantiate(emptyGO, nNextPos, Quaternion.Euler(-90, 0, -180)) as GameObject;
                                    go.transform.position += new Vector3(0, -0.4f, 0);
                                    //gon.GetComponent<MJ_SP>().HS = hs;
                                    nNextPos += nOffsetLX;
                                    pengpaiNList.Add(go);
                                    pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);
                                    NPengPaiPaiMian(hs);
                                }
                                else
                                {
                                    int count = 0;
                                    for (int i = 0; i < pengpaiNList.Count; i++)
                                    {
                                        if (hs == pengpaiNList[i].gameObject.GetComponent<MJ_SP>().HS)
                                        {
                                            count++;
                                            if (count == 3)
                                            {
                                                GameObject gon = GameObject.Instantiate(emptyGO, pengpaiNList[i].transform.position + nOffsetX, Quaternion.Euler(-90, 0, -180)) as GameObject;
                                                gon.transform.position += new Vector3(0, -0.4f, 0);
                                                gon.GetComponent<MJ_SP>().HS = hs;
                                                pengpaiNList.Add(emptyGO);
                                                for (int l = pengpaiNList.Count; l > i + 2; l--)
                                                {
                                                    pengpaiNList[l - 1] = pengpaiNList[l - 2];
                                                    pengpaiNList[l - 2].transform.position += nOffsetX;
                                                }
                                                nNextPos += nOffsetX;
                                                pengpaiNList[i + 1] = gon;
                                                pengpaiNList[i + 1].transform.SetParent(nPengGangPaiContainerTrans);
                                                count = 0;
                                           
                                            }
                                        }
                                    }
                                }
                            }
                            else if (type.Equals(GangType.hanBaoGang, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (pengpaiNList.Find(w => w.GetComponent<MJ_SP>().HS == hs) == null)
                                {
                                    if (isNotConn)
                                    {
                                        GameObject.Destroy(shoupaiNList[0]);
                                        GameObject.Destroy(shoupaiNList[1]);
                                        GameObject.Destroy(shoupaiNList[2]);
                                        // GameObject.Destroy(shoupaiNList[3]);
                                        shoupaiNList.RemoveAt(0);
                                        shoupaiNList.RemoveAt(0);
                                        shoupaiNList.RemoveAt(0);
                                        //   shoupaiNList.RemoveAt(0);

                                    }
                                    for (int k = 0; k < 4; k++)
                                    {
                                        GameObject gon = GameObject.Instantiate(emptyGO, nNextPos, nPengQiPaiQua) as GameObject;
                                        nNextPos += nOffsetX;
                                        pengpaiNList.Add(gon);
                                        pengpaiNList[pengpaiNList.Count - 1].transform.SetParent(nPengGangPaiContainerTrans);

                                        NPengPaiPaiMian(hs);
                               
                                    }

                                }
                                else
                                {
                                    int count = pengpaiNList.FindLastIndex(u => u.gameObject.GetComponent<MJ_SP>().HS == hs);
                                    if (count > 0)
                                    {


                                        GameObject gon = GameObject.Instantiate(emptyGO, pengpaiNList[count].transform.position + nOffsetX, nPengQiPaiQua) as GameObject;


                                        pengpaiNList.Add(gon);
                                        NPengPaiPaiMian(hs);
                                        for (int l = pengpaiNList.Count; l > count + 2; l--)
                                        {
                                            pengpaiNList[l - 1] = pengpaiNList[l - 2];
                                            pengpaiNList[l - 2].transform.position += nOffsetX;
                                        }
                                        nNextPos += nOffsetX;
                                        pengpaiNList[count + 1] = gon;
                                        pengpaiNList[count + 1].transform.SetParent(nPengGangPaiContainerTrans);
                                        count = 0;
                                 

                                    }



                                }
                            }
                        }
                        break;
                }

                break;
            case FW.South:
                break;
        }
        if (fw != pfw && isNotConn)
        {
            DelQIpai(pfw);

        }
        ResetDiamondPosition(emptyGO.transform.position);
    }

    /// <summary>
    /// 弃牌区的最后一张牌删除
    /// </summary>
    /// <param name="pfw"></param>
    public void DelQIpai(FW pfw)
    {
        switch (pfw)
        {
            case FW.East:
                GameObject.Destroy(qipaiEList[qipaiEList.Count - 1]);
                qipaiEList.RemoveAt(qipaiEList.Count - 1);
                break;
            case FW.West:
                GameObject.Destroy(qipaiWList[qipaiWList.Count - 1]);
                qipaiWList.RemoveAt(qipaiWList.Count - 1);
                break;
            case FW.North:
                GameObject.Destroy(qipaiNList[qipaiNList.Count - 1]);
                qipaiNList.RemoveAt(qipaiNList.Count - 1);
                break;
            case FW.South:
                GameObject.Destroy(myCards.qiPaiGOList[myCards.qiPaiGOList.Count - 1]);
                myCards.qiPaiGOList.RemoveAt(myCards.qiPaiGOList.Count - 1);
                break;
        }
    }

    /// <summary>
    /// East碰牌，杠牌生成牌面
    /// </summary>
    /// <param name="hs"></param>
    private void EPengPaiPaiMian(int hs)
    {
        int j = pengpaiEList.Count - 1;
        pengpaiEList[j].GetComponent<MJ_SP>().HS = hs;
        pengpaiEList[j].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

    }
    /// <summary>
    /// West碰牌，杠牌生成牌面
    /// </summary>
    /// <param name="hs"></param>
    private void WPengPaiPaiMian(int hs)
    {
        int j = pengpaiWList.Count - 1;
        pengpaiWList[j].GetComponent<MJ_SP>().HS = hs;
        pengpaiWList[j].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
    }
    /// <summary>
    /// North碰牌，杠牌生成牌面
    /// </summary>
    /// <param name="hs"></param>
    private void NPengPaiPaiMian(int hs)
    {
        int j = pengpaiNList.Count - 1;
        pengpaiNList[j].GetComponent<MJ_SP>().HS = hs;
        pengpaiNList[j].GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
        // pengpaiNList[j].GetComponent<Renderer>().material = _LuceMaterial;
        pengpaiNList[j].GetComponent<Renderer>().material.shader = _LucenShader;
    }

    public string GetGangType(int type)
    {
        switch (type)
        {
            case 1:
                return "M";
            case 2:
                return "Z";
            case 3:
                return "A";
            case 4:
                return "H";
            default:
                return type.ToString();
        }
    }
    /// <summary>
    /// 出牌
    /// </summary>
    public void ChuPai(MJ_Event chuPai)
    {
        if (GameInfo.FW == GameInfo.nowFW)
        {
            var mj = chuPai.GetComponent<MJ_SP>();
            if ((GameInfo.returnTP != null && GameInfo.returnTP.mj.Count > 0 && GameInfo.CardsNumber == 1 && GameInfo.returnTP.mj.First(w => w.PaiHS == mj.HS) != null && GameInfo.IsTingPai)||GameInfo.isHhouTing)
            {
                //SendBT sbt = SendBT.CreateBuilder().SetOpenid(GameInfo.OpenID).SetRoomID(GameInfo.room_id).SetMj(MaJiang.CreateBuilder().SetPaiHS(mj.HS).SetPaiID(mj.ID).Build()).SetType(1).Build();
                //byte[] sbtbody = sbt.ToByteArray();
                MaJiang majiangs = new MaJiang();
                majiangs.PaiHS = mj.HS;
                majiangs.PaiID = mj.ID;

                SendBT sbt = new SendBT();
                sbt.openid = GameInfo.OpenID;
                sbt.roomID = GameInfo.room_id;
                sbt.mj = majiangs;
                sbt.type = 1;
                byte[] sbtbody = ProtobufUtility.GetByteFromProtoBuf(sbt);

                byte[] sbtdata = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5001, sbtbody.Length, 0, sbtbody);
                GameInfo.cs.Send(sbtdata);
               // GameInfo.IsTingPai = true;
                GameInfo.isRealTing = true;
                GameInfo.returnTP = null;
                GameInfo.isHhouTing = false;


                for (int i = 0; i <myCards. shouPaiGOList.Count ; i++)
                {
                    myCards.shouPaiGOList[i].GetComponent<MeshRenderer>().material.color = startGame.darkColor;
                    myCards.shouPaiGOList[i].GetComponent<MJ_Event>().isCanOut = false;
                }
                SendNotBaoTing sendBao = new SendNotBaoTing();
                sendBao.openid = GameInfo.OpenID;
                sendBao.roomid = GameInfo.room_id;
            }

            //SendCP cp = SendCP.CreateBuilder()
            //            .SetOpenid(GameInfo.OpenID)
            //            .SetRoomid(GameInfo.room_id)
            //            .SetMj(MaJiang.CreateBuilder()
            //                    .SetPaiHS(mj.HS)
            //                    .SetPaiID(mj.ID)
            //                    .Build())
            //                       .SetType(GameInfo.IsGang ? 1 : 0)
            //            .Build();
            //byte[] body = cp.ToByteArray();

            MaJiang majiang = new MaJiang();
            majiang.PaiHS = mj.HS;
            majiang.PaiID = mj.ID;

            SendCP cp = new SendCP();
            cp.openid = GameInfo.OpenID;
            cp.roomid = GameInfo.room_id;
            cp.mj = majiang;
            cp.type = GameInfo.IsGang ? 1 : 0;
            byte[] body = ProtobufUtility.GetByteFromProtoBuf(cp);


            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 3001, body.Length, 0, body);
            GameInfo.cs.Send(data);

            GameInfo.IsGang = false;

            if (true)
            {
                SendNotBaoTing sendBao = new SendNotBaoTing();
                sendBao.openid = GameInfo.OpenID;
                sendBao.roomid = GameInfo.room_id;
            }
            //出牌多排序一次，导致手牌闪
            //   maskPai.OnQueYiMenButtonClick(GameInfo.queType);
            if (GameInfo.returnTP!=null)
            {
                SendNotBaoTing sendBao = new SendNotBaoTing();
                sendBao.openid = GameInfo.OpenID;
                sendBao.roomid = GameInfo.room_id;
                GameInfo.returnTP = null;
            }
        }
    }

    /// <summary>
    /// 隐藏冲锋、乌骨鸡的动画效果
    /// </summary>
    public void HideJi_S()
    {
        // GameObject.Find("AniJi_S").SetActive(false);
        GameObject.Find("Main Camera").GetComponent<FICStartGame>().AniJi_S.SetActive(false);
    }



    /// <summary>
    /// 清空本脚本所涉及的所有集合，更改
    /// </summary>
    public void ClearAllListsAndChanges()
    {
        shoupaiEList.Clear();
        qipaiEList.Clear();
        pengpaiEList.Clear();

        eisNorth = false;
        wisSouth = false;
        nisWest = false;

        shoupaiWList.Clear();
        qipaiWList.Clear();
        pengpaiWList.Clear();

        shoupaiNList.Clear();
        qipaiNList.Clear();
        pengpaiNList.Clear();

        myCards.qiPaiGOList.Clear();

        eNextPos = reseteNextPos;
        wNextPos = resetwNextPos;
        nNextPos = resetnNextPos;
        sNextPos = resetsNextPos;



    }

    private void Test()
    {

    }
}


