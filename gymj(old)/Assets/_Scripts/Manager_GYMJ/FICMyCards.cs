using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DNL;
/// <summary>
/// 处理个人牌，包括手牌，碰牌，弃牌
/// </summary>
public class FICMyCards : MonoBehaviour
{
    Quaternion quaShou = Quaternion.Euler(0, 0, 180);
    Quaternion quaPengQi = Quaternion.Euler(90, 0, 180);
    Quaternion quaGang = Quaternion.Euler(90, -90, 180);
    Quaternion quaAnGang = Quaternion.Euler(180, 180, 90);
    Vector3 vecDown = new Vector3(0, 0, -0.4f);
    //=====================对其他脚本的获取=============
    private FICpaipaipai paipaipai;
    private FICStartGame startGame;
    private FICMaskPai maskPai;
    private Manager_PengGang pengGang;
    //==============个人手牌，弃牌，碰牌的父物体=================
    public Transform shouPaiContainerTrans;
    private Transform qiPaiContainerTrans;
    private Transform pengGangPaiContainerTrans;
    public Transform tempShouPaiContainerTrans;
    //======================手牌预制体==============
    public GameObject shouPaiGO;
    public GameObject tempGO;
    //======================手牌位置确定==============
    public Transform firstShouPaiTrans;
    private Transform secondShouPaiTrans;
    private Transform thirdShouPaiTrans;
    public Vector3 newFirstShouPaiTrans;
    public Transform moPaiTrans;
    //======================弃牌位置确定==============
    public Transform firstQiPaiTrans;
    private Transform secondQiPaiTrans;
    private Transform thirdQiPaiTrans;
    //======================碰牌位置确定==============
    public Transform firstPengPaiTrans;
    private Transform secondPengPaiTrans;
    private Transform thirdPengPaiTrans;
    private Transform fourPengPaiTrans;
    private Transform fivePengPaiTrans;
    //======================偏移量确定==============
    public Vector3 shouPaiOffsetX;
    public Vector3 shouPaiOffsetY;
    public Vector3 qiPaiOffsetX;
    public Vector3 qiPaiOffsetZ;
    private Vector3 pengPaiOffsetX;
    private Vector3 pengPaiOffsetZ;
    private Vector3 pengPaiOffsetLX;
    //======================生成下一张牌的位置确定=======
    public Vector3 nextPengPaiPos;
    public Vector3 nextQiPaiPos;

    //======================Array==============
    public Texture[] textureArray;//3d牌的材质贴图
                                  // public List<Texture> texture
                                  //=======================List=============
    public List<GameObject> shouPaiGOList;
    public List<GameObject> pengPaiGOList;
    public List<GameObject> qiPaiGOList;
    private List<MaJiang> myMaJiangList;
    //====================临时变量===========
    private Vector3 tempPos;
    private GameObject sortTempGO;
    private Manager_Audio managerAudio; ///单例

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        myMaJiangList = new List<MaJiang>();
           //=====================对其他脚本的获取=============
           paipaipai = gameObject.GetComponent<FICpaipaipai>();
        maskPai = gameObject.GetComponent<FICMaskPai>();
        startGame = gameObject.GetComponent<FICStartGame>();
        pengGang = gameObject.GetComponent<Manager_PengGang>();
        //=====================为个人手牌，弃牌，碰牌的父物体赋值===============
        shouPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/SouthContainer/ShouPaiContainer");
        qiPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/SouthContainer/QiPaiContainer");
        pengGangPaiContainerTrans = transform.Find("/Game_Prefabs/TABLE/Game_3DPai/SouthContainer/PengGangPaiContainer");
        tempShouPaiContainerTrans = shouPaiContainerTrans;
        //======================手牌预制体==============
        shouPaiGO = Resources.Load<GameObject>("Game_GYMJ/Prefabs/3Dpai");
        tempGO = transform.Find("/Game_Prefabs/emptyGO").gameObject;
        //======================手牌位置确定==============
        firstShouPaiTrans = shouPaiContainerTrans.Find("FirstShouPai");
        secondShouPaiTrans = shouPaiContainerTrans.Find("SecondShouPai");
        thirdShouPaiTrans = shouPaiContainerTrans.Find("ThirdShouPai");
        newFirstShouPaiTrans = firstShouPaiTrans.position;
        moPaiTrans = shouPaiContainerTrans.Find("MoPai");
        //======================弃牌位置确定==============
        firstQiPaiTrans = qiPaiContainerTrans.Find("FirstQiPai");
        secondQiPaiTrans = qiPaiContainerTrans.Find("SecondQiPai");
        thirdQiPaiTrans = qiPaiContainerTrans.Find("ThirdQiPai");
        //======================碰牌位置确定==============
        firstPengPaiTrans = pengGangPaiContainerTrans.Find("FirstPengPai");
        secondPengPaiTrans = pengGangPaiContainerTrans.Find("SecondPengPai");
        thirdPengPaiTrans = pengGangPaiContainerTrans.Find("ThirdPengPai");
        fourPengPaiTrans = pengGangPaiContainerTrans.Find("FourPengPai");
        fivePengPaiTrans = pengGangPaiContainerTrans.Find("FivePengPai");
        //======================偏移量确定==============
        shouPaiOffsetX = secondShouPaiTrans.position - firstShouPaiTrans.position;
        shouPaiOffsetY = thirdShouPaiTrans.position - firstShouPaiTrans.position;
        qiPaiOffsetX = secondQiPaiTrans.position - firstQiPaiTrans.position;
        qiPaiOffsetZ = thirdQiPaiTrans.position - firstQiPaiTrans.position;

        pengPaiOffsetX = secondPengPaiTrans.position - firstPengPaiTrans.position;
        pengPaiOffsetZ = thirdPengPaiTrans.position - secondPengPaiTrans.position;
        pengPaiOffsetLX = fivePengPaiTrans.position - fourPengPaiTrans.position;
        //======================生成下一张牌的位置确定=======
        nextPengPaiPos = firstPengPaiTrans.position;
        nextQiPaiPos = firstQiPaiTrans.position;
        //======================动态加载3d牌贴图==============
        textureArray = new Texture[30];
        InitTextureFor3D();

        managerAudio = GameObject.Find("Main Camera").GetComponent<Manager_Audio>(); ///单例ManagerAudio脚本

    }
    /// <summary>
    /// 动态加载3d牌的贴图
    /// </summary>
    private void InitTextureFor3D()
    {

        string textPath = null;
        for (int i = 0; i < 30; i++)
        {
            try
            {
                if (i % 10 != 0)
                {
                    textPath = "Game_GYMJ/Texture/Game_MJBOX/" + i;
                    textureArray[i] = Resources.Load(textPath, typeof(Texture)) as Texture;
                }
            }
            catch { }

        }

    }


    /// <summary>
    /// 牌局开始，生成个人玩家手牌，13张或者14张
    /// </summary>
    /// <param name="maJiangIList"></param>
    public void CreatCardsAtStart(IList<MaJiang> maJiangIList)
    {
        for (int i = 0; i < maJiangIList.Count; i++)
        {
            GameObject go = (GameObject)GameObject.Instantiate(shouPaiGO, firstShouPaiTrans.position + shouPaiOffsetX * i, quaShou, shouPaiContainerTrans);
            go.transform.localEulerAngles = quaShou.eulerAngles;
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[maJiangIList[i].PaiHS];
            go.GetComponent<MJ_SP>().HS = maJiangIList[i].PaiHS;
            go.GetComponent<MJ_SP>().ID = maJiangIList[i].PaiID;
            shouPaiGOList.Add(go);
        }
    }


    public void CreatCatdsAtStartByFour(IList<MaJiang> maJiangIList)
    {
        for (int i = 0; i < maJiangIList.Count; i++)
        {
            myMaJiangList.Add(maJiangIList[i]) ;
        }
        Invoke("CreatCatdsByFirstFour", 0.3f);
    }
    #region  4张4张生成个人手牌的笨办法
    private void CreatCatdsByFirstFour()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject go = (GameObject)GameObject.Instantiate(shouPaiGO, firstShouPaiTrans.position + shouPaiOffsetX * i, quaShou, shouPaiContainerTrans);
            go.transform.localEulerAngles = quaShou.eulerAngles;
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[myMaJiangList[i].PaiHS];
            go.GetComponent<MJ_SP>().HS = myMaJiangList[i].PaiHS;
            go.GetComponent<MJ_SP>().ID = myMaJiangList[i].PaiID;
            go.GetComponent<MJ_Event>().isCanOut = false;
            shouPaiGOList.Add(go);
        }
        Invoke("CreatCatdsBySecondFour", 0.3f);
    }
    private void CreatCatdsBySecondFour()
    {
        for (int i = 4; i < 8; i++)
        {
            GameObject go = (GameObject)GameObject.Instantiate(shouPaiGO, firstShouPaiTrans.position + shouPaiOffsetX * i, quaShou, shouPaiContainerTrans);
            go.transform.localEulerAngles = quaShou.eulerAngles;
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[myMaJiangList[i].PaiHS];
            go.GetComponent<MJ_SP>().HS = myMaJiangList[i].PaiHS;
            go.GetComponent<MJ_SP>().ID = myMaJiangList[i].PaiID;
            go.GetComponent<MJ_Event>().isCanOut = false;
            shouPaiGOList.Add(go);
        }
        Invoke("CreatCatdsByThirdFour", 0.3f);
    }
    private void CreatCatdsByThirdFour()
    {
        for (int i = 8; i < 12; i++)
        {
            GameObject go = (GameObject)GameObject.Instantiate(shouPaiGO, firstShouPaiTrans.position + shouPaiOffsetX * i, quaShou, shouPaiContainerTrans);
            go.transform.localEulerAngles = quaShou.eulerAngles;
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[myMaJiangList[i].PaiHS];
            go.GetComponent<MJ_SP>().HS = myMaJiangList[i].PaiHS;
            go.GetComponent<MJ_SP>().ID = myMaJiangList[i].PaiID;
            go.GetComponent<MJ_Event>().isCanOut = false;
            shouPaiGOList.Add(go);
        }
        Invoke("CreatCatdsByFourFour", 0.3f);
    }
    private void CreatCatdsByFourFour()
    {
        for (int i = 12; i < myMaJiangList.Count; i++)
        {
            GameObject go = (GameObject)GameObject.Instantiate(shouPaiGO, firstShouPaiTrans.position + shouPaiOffsetX * i, quaShou, shouPaiContainerTrans);
            go.transform.localEulerAngles = quaShou.eulerAngles;
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[myMaJiangList[i].PaiHS];
            go.GetComponent<MJ_SP>().HS = myMaJiangList[i].PaiHS;
            go.GetComponent<MJ_SP>().ID = myMaJiangList[i].PaiID;
            go.GetComponent<MJ_Event>().isCanOut = false;
            shouPaiGOList.Add(go);
        }

        SortAllShouPai();
        startGame.managerGame.OnSendManaged();
        if (shouPaiGOList.Count == 14)
        {
            shouPaiGOList[shouPaiGOList.Count - 1].transform.position += shouPaiOffsetX;
        }
        startGame.GameReturnStartGameNewSide();
    }
#endregion
    /// <summary>
    /// 摸牌，在摸牌位置生成一张摸牌，并添加到手牌集合
    /// </summary>
    /// <param name="hs"></param>
    public void MoPai(int hs, int id)
    {
        GameInfo.HYFw = GameInfo.FW;
        GameObject go = (GameObject)GameObject.Instantiate(shouPaiGO, moPaiTrans.position, quaShou, shouPaiContainerTrans);
        //if (go=null)
        //{
        //    go = (GameObject)GameObject.Instantiate(shouPaiGO, moPaiTrans.position, quaShou, shouPaiContainerTrans);
        //}
        go.transform.localEulerAngles = quaShou.eulerAngles;
        go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
        go.GetComponent<MJ_SP>().HS = hs;
        go.GetComponent<MJ_SP>().ID = id;

        //JudgeIfNotCanout(go);
        shouPaiGOList.Add(go);
        //go.GetComponent<MJ_Event>().SetOtherMjPos();
        maskPai.OnQueYiMenButtonClick(GameInfo.queType, false);

        if (GameInfo.isRealTing || GameInfo.isTT)
        {
            for (int i = 0; i < shouPaiGOList.Count - 1; i++)
            {
                shouPaiGOList[i].GetComponent<MeshRenderer>().material.color = startGame.darkColor;
                shouPaiGOList[i].GetComponent<MJ_Event>().isCanOut = false;
            }
        }
    }
    /// <summary>
    /// 断线重连之后，如果有听牌，要先封牌，让不能出的牌封牌
    /// </summary>
    public void ReconnectTingMask()
    {
        if (GameInfo.isRealTing || GameInfo.isTT)
        {
            for (int i = 0; i < shouPaiGOList.Count - 1; i++)
            {
                shouPaiGOList[i].GetComponent<MeshRenderer>().material.color = startGame.darkColor;
                shouPaiGOList[i].GetComponent<MJ_Event>().isCanOut = false;
            }
        }
    }
    /// <summary>
    /// 判定摸摸的牌是否能狗打出
    /// </summary>
    /// <param name="go"></param>
    private void JudgeIfNotCanout(GameObject go)
    {
        switch (GameInfo.queType)
        {
            case GameInfo.QueType.Tong:
                if (go.GetComponent<MJ_SP>().HS > 10)
                {
                    go.GetComponent<MJ_Event>().isCanOut = false;
                }
                break;
            case GameInfo.QueType.Tiao:
                if (go.GetComponent<MJ_SP>().HS < 10 || go.GetComponent<MJ_SP>().HS > 20)
                {
                    go.GetComponent<MJ_Event>().isCanOut = false;
                }
                break;
            case GameInfo.QueType.Wan:
                if (go.GetComponent<MJ_SP>().HS < 20)
                {
                    go.GetComponent<MJ_Event>().isCanOut = false;
                }
                break;
            case GameInfo.QueType.no:
                break;
        }
    }
    /// <summary>
    /// message GangMSG{
    // required int32 fw=1;//杠的方位
    //required int32 Type=2;//杠的方式
    //required MaJiang mj=3;//杠的牌}

    public void PengPaiEntir(int hs, bool isRecon = true)
    {
		//碰牌区生成3张碰牌，并加入碰杠牌集合
        PengPai(hs, isRecon);
        GameInfo.HYFw = GameInfo.FW;
		//重新定位指示钻石的位置
        paipaipai.ResetDiamondPosition(paipaipai.emptyGO.transform.position);
		//碰牌之后，将手牌中的相关牌销毁两张，并从集合中移除
        if (isRecon) DestroyPengShouPais(hs);
        //firstShouPaiTrans.position =newFirstShouPaiTrans + pengPaiGOList.Count * shouPaiOffsetX;
		//调整手牌位置
        if (isRecon) firstShouPaiTrans.position = newFirstShouPaiTrans + (14 - shouPaiGOList.Count) * shouPaiOffsetX;
    }

    /// <summary>
    /// 碰牌区生成3张碰牌，并加入碰杠牌集合
    /// </summary>
    /// <param name="hs"></param>
    private void PengPai(int hs, bool isNotConn = true)
    {
		//如果isNotConn=false,tempGO=emptyGO
        if (!isNotConn) tempGO = transform.Find("/Game_Prefabs/emptyGO").gameObject;
        for (int i = 0; i < 3; i++)
        {
			//                                                  物体         位置          方向        父物体
            GameObject go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
			//给生成的碰牌指定材质
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
			//每生成一个，位置就加一个pengPaiOffsetX
            nextPengPaiPos += pengPaiOffsetX;
			//给生成物体的<MJ_SP>()的HS赋值
            go.GetComponent<MJ_SP>().HS = hs;
			//将生成的物体添加到集合pengPaiGOList中
            pengPaiGOList.Add(go);
        }
        nextPengPaiPos += pengPaiOffsetLX / 2;
    }
    /// <summary>
    /// 杠牌区，根据杠牌的方位和类型，调用不同杠牌方法
    /// </summary>
    /// <param name="gangMsg"></param>
    /// <param name="isNotConn"></param>
    public void GangPai(GangMSG gangMsg, bool isNotConn = true)
    {
        FW fw = GameInfo.GetFW(gangMsg.fw);
        GameInfo.HYFw = GameInfo.FW;

        paipaipai.ResetDiamondPosition(paipaipai.emptyGO.transform.position);
        switch (fw)
        {
            case FW.East:
                GangEast(gangMsg.mj.PaiHS);
                break;
            case FW.West:
                GangWest(gangMsg.mj.PaiHS);
                break;
            case FW.North:
                GangNorth(gangMsg.mj.PaiHS);
                break;
            case FW.South:

                switch (gangMsg.Type)
                {
                    case 1:
                        print("明杠，应该不存在");
                        break;
                    case 2:
                        print("转弯杠");
                        ZhuanWanGang(gangMsg.mj.PaiHS);
                        break;
                    case 3:
                        print("暗杠");
                        AnGang(gangMsg.mj.PaiHS);
                        break;
                    case 4:
                        print("憨包杠");
                        HanBaoGang(gangMsg.mj.PaiHS);
                        break;
                }
                break;
        }

        //firstShouPaiTrans.position = newFirstShouPaiTrans + pengPaiGOList.Count * shouPaiOffsetX;
        firstShouPaiTrans.position = newFirstShouPaiTrans + (13 - shouPaiGOList.Count) * shouPaiOffsetX;
    }
    /// <summary>
    /// 杠左手边的牌，生成4张牌（后三张调用了碰牌方法），加入碰牌集合
    /// </summary>
    /// <param name="hs"></param>
    private void GangWest(int hs, bool isNotConn = true)
    {

        GameObject go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaGang, pengGangPaiContainerTrans);
        go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
        nextPengPaiPos += pengPaiOffsetLX;
        go.GetComponent<MJ_SP>().HS = hs;
        pengPaiGOList.Add(go);
        PengPai(hs);

    }
    /// <summary>
    /// 杠右手边的牌，生成4张牌，加入碰牌集合
    /// </summary>
    /// <param name="hs"></param>
    private void GangEast(int hs)
    {
        GameObject go;
        for (int i = 0; i < 3; i++)
        {
            go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
            nextPengPaiPos += pengPaiOffsetX;
            go.GetComponent<MJ_SP>().HS = hs;
            pengPaiGOList.Add(go);
        }
        nextPengPaiPos += pengPaiOffsetLX - pengPaiOffsetX;

        go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaGang, pengGangPaiContainerTrans);
        go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
        go.GetComponent<MJ_SP>().HS = hs;
        pengPaiGOList.Add(go);
        nextPengPaiPos += pengPaiOffsetLX;// - pengPaiOffsetX;

        // managerAudio.AudioClick("Gang"); ///杠牌音效


    }
    /// <summary>
    /// 杠对面的牌，生成4张牌，加入碰牌集合
    /// </summary>
    /// <param name="hs"></param>
    private void GangNorth(int hs)
    {
        GameObject go;
        go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
        go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
        nextPengPaiPos += pengPaiOffsetX;
        go.GetComponent<MJ_SP>().HS = hs;
        pengPaiGOList.Add(go);

        go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
        go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
        go.GetComponent<MJ_SP>().HS = hs;
        pengPaiGOList.Add(go);

        go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos + pengPaiOffsetZ, quaGang, pengGangPaiContainerTrans);
        go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
        go.GetComponent<MJ_SP>().HS = hs;
        nextPengPaiPos += pengPaiOffsetX;
        pengPaiGOList.Add(go);

        go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
        go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
        go.GetComponent<MJ_SP>().HS = hs;
        nextPengPaiPos += pengPaiOffsetLX;
        pengPaiGOList.Add(go);

        // managerAudio.AudioClick("Gang"); ///杠牌音效
    }
    /// <summary>
    /// 重连杠牌
    /// </summary>
    /// <param name="hs"></param>
    /// <param name="type"></param>
    /// <param name="fw"></param>
    public void GangREConnect(int hs, string type, FW fw)
    {
        switch (fw)
        {
            case FW.East:
                GangEast(hs);
                break;
            case FW.West:
                GangWest(hs);
                break;
            case FW.North:
                GangNorth(hs);
                break;
            case FW.South:

                switch (type)
                {
                    case "M":
                        print("明杠，应该不存在");
                        break;
                    case "Z":
                        print("转弯杠");
                        ReconZhuanWanGang(hs);
                        break;
                    case "A":
                        print("暗杠");
                        AnGang(hs);
                        break;
                    case "H":
                        print("憨包杠");
                        HanBaoGang(hs);
                        break;
                }
                break;
        }
        //firstShouPaiTrans.position = newFirstShouPaiTrans + (13 - shouPaiGOList.Count) * shouPaiOffsetX;
        Debug.Log(shouPaiOffsetX);
    }

    /// <summary>
    /// 转弯杠，碰牌区生成一张牌，集合靠后的牌，向右移动
    /// </summary>
    /// <param name="hs"></param>
    public void ZhuanWanGang(int hs, bool isHanBao = false)
    {
        GameObject go;
        int lastHSIndex = pengPaiGOList.FindLastIndex(U => U.GetComponent<MJ_SP>().HS == hs);
        if (isHanBao)
        {
            go = (GameObject)GameObject.Instantiate(tempGO, pengPaiGOList[lastHSIndex].transform.position + pengPaiOffsetX, quaPengQi, pengGangPaiContainerTrans);
        }
        else
        {
            go = (GameObject)GameObject.Instantiate(tempGO, pengPaiGOList[lastHSIndex].transform.position + pengPaiOffsetX, quaAnGang, pengGangPaiContainerTrans);
            go.transform.localPosition += vecDown;
            go.transform.localRotation = quaAnGang;
        }
        go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
        go.GetComponent<MJ_SP>().HS = hs;

        pengPaiGOList.Add(tempGO);

        for (int i = pengPaiGOList.Count - 1; i > lastHSIndex + 1; i--)
        {
            pengPaiGOList[i] = pengPaiGOList[i - 1];
            pengPaiGOList[i - 1].transform.position += pengPaiOffsetX;
        }
        nextPengPaiPos += pengPaiOffsetLX;
        pengPaiGOList[lastHSIndex + 1] = go;

    }
    /// <summary>
    /// 重连转弯杠
    /// </summary>
    /// <param name="hs"></param>
    /// <param name="isHanBao"></param>
    private void ReconZhuanWanGang(int hs, bool isHanBao = false)
    {
        GameObject go;
        for (int i = 0; i < 3; i++)
        {
            go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
            nextPengPaiPos += pengPaiOffsetX;
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
            go.GetComponent<MJ_SP>().HS = hs;
            pengPaiGOList.Add(go);
        }
        if (isHanBao)
        {
            go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
            go.GetComponent<MJ_SP>().HS = hs;
            pengPaiGOList.Add(go);
        }
        else
        {
            go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
            go.transform.localPosition += vecDown;
            go.transform.localRotation = quaAnGang;
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
            go.GetComponent<MJ_SP>().HS = hs;
            pengPaiGOList.Add(go);
        }


        //pengPaiGOList.Add(tempGO);
        nextPengPaiPos += pengPaiOffsetLX;
    }
    /// <summary>
    /// 暗杠，碰牌区生成4张牌，
    /// </summary>
    /// <param name="hs"></param>
    public void AnGang(int hs)
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
            go.GetComponent<MJ_SP>().HS = hs;
            pengPaiGOList.Add(go);
            nextPengPaiPos += pengPaiOffsetX;

        }
        for (int i = 0; i < 2; i++)
        {
            GameObject go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaAnGang, pengGangPaiContainerTrans);
            go.transform.localPosition += vecDown;
            go.transform.localRotation = quaAnGang;
            go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
            go.GetComponent<MJ_SP>().HS = hs;
            pengPaiGOList.Add(go);
            nextPengPaiPos += pengPaiOffsetX;
        }
        nextPengPaiPos += pengPaiOffsetLX - pengPaiOffsetX;

    }
    /// <summary>
    /// 憨包杠，如果是碰后憨包杠，就直接调用转弯杠生成，否则生成4张牌的憨包杠
    /// </summary>
    /// <param name="hs"></param>
    public void HanBaoGang(int hs)
    {
        int index = -1;
        index = pengPaiGOList.FindIndex(u => u.GetComponent<MJ_SP>().HS == hs);
        if (index != -1)
        {
            ZhuanWanGang(hs, true);
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject go = (GameObject)GameObject.Instantiate(tempGO, nextPengPaiPos, quaPengQi, pengGangPaiContainerTrans);
                go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
                go.GetComponent<MJ_SP>().HS = hs;
                pengPaiGOList.Add(go);
                nextPengPaiPos += pengPaiOffsetX;
            }
            nextPengPaiPos += pengPaiOffsetLX - pengPaiOffsetX;
        }

    }
    /// <summary>
    /// 杠牌之后，将手牌中所有的相关牌全部销毁，并从集合中移除
    /// </summary>
    /// <param name="hs"></param>
    public void DestroyGangShouPais(int hs)
    {
        List<GameObject> goList = shouPaiGOList.FindAll(u => u.GetComponent<MJ_SP>().HS == hs);
        for (int i = 0; i < goList.Count; i++)
        {
            Destroy(goList[i]);
        }
        shouPaiGOList.RemoveAll(u => u.GetComponent<MJ_SP>().HS == hs);
        goList.Clear();
    }
    /// <summary>
    /// 碰牌之后，将手牌中的相关牌销毁两张，并从集合中移除
    /// </summary>
    /// <param name="hs"></param>
    private void DestroyPengShouPais(int hs)
    {
        int count = 0;
        foreach (var item in shouPaiGOList)
        {
            if (count < 2)
            {
                if (item.GetComponent<MJ_SP>().HS == hs)
                {
                    Destroy(item);
                    count++;
                }

            }
        }
        for (int i = 0; i < 2; i++)
        {
            shouPaiGOList.Remove(shouPaiGOList.Find(u => u.GetComponent<MJ_SP>().HS == hs));
        }
    }
    /// <summary>
    /// 打牌时候，消除这张手牌，并从集合中移除
    /// </summary>
    /// <param name="thisGO"></param>
    public void DestroyThisShouPai(GameObject thisGO)
    {
        Destroy(thisGO);
        shouPaiGOList.Remove(thisGO);
        SortAllShouPai();
    }
    /// <summary>
    /// 给手牌排序
    /// </summary>
    public void SortAllShouPai()
    {

        for (int i = 0; i < shouPaiGOList.Count; i++)
        {
            for (int j = 0; j < shouPaiGOList.Count - i - 1; j++)
            {
                if (shouPaiGOList[j].GetComponent<MJ_SP>().HS > shouPaiGOList[j + 1].GetComponent<MJ_SP>().HS)
                {

                    tempPos = shouPaiGOList[j + 1].transform.position;
                    sortTempGO = shouPaiGOList[j + 1];
                    shouPaiGOList[j + 1].transform.position = shouPaiGOList[j].transform.position;
                    shouPaiGOList[j + 1] = shouPaiGOList[j];
                    shouPaiGOList[j].transform.position = tempPos;
                    shouPaiGOList[j] = sortTempGO;
                }
            }
        }
        for (int i = 0; i < shouPaiGOList.Count; i++)
        {
            shouPaiGOList[i].transform.position = firstShouPaiTrans.position + shouPaiOffsetX * i;
            //shouPaiGOList[i].GetComponent<MJ_Event>().isSelect = false;
            shouPaiGOList[i].GetComponent<MJ_Event>().UpOnlyWhenSort();
        }

    }
    /// <summary>
    /// 弃牌，根据花色在弃牌区生成一张牌，2人与多人生成牌的位置不同
    /// </summary>
    /// <param name="hs"></param>
    public void QiPai(int hs, int id)
    {
        GameObject go;
        if (GameInfo.room_peo != 2)
        {
            go = GameObject.Instantiate(tempGO, firstQiPaiTrans.position + (qiPaiGOList.Count % 7) * qiPaiOffsetX + (qiPaiGOList.Count / 7) * qiPaiOffsetZ, quaPengQi, qiPaiContainerTrans) as GameObject;
        }
        else
        {
            go = GameObject.Instantiate(tempGO, (firstQiPaiTrans.position - 3 * qiPaiOffsetX) + (qiPaiGOList.Count % 13) * qiPaiOffsetX + (qiPaiGOList.Count / 13) * qiPaiOffsetZ, quaPengQi, qiPaiContainerTrans) as GameObject;

            //2017.8.6劉磊添加
            //go.transform.localPosition += Vector3.forward * 0.4f;
        }

        go.GetComponent<MeshRenderer>().material.mainTexture = textureArray[hs];
        go.GetComponent<MJ_SP>().HS = hs;
        go.GetComponent<MJ_SP>().ID = id;
        qiPaiGOList.Add(go);

        if (GameInfo.queType != GameInfo.QueType.no)
        {
            maskPai.OnQueYiMenButtonClick(GameInfo.queType, true);
        }
        else
        {

            SortAllShouPai();
        }
        paipaipai.ResetDiamondPosition(qiPaiGOList[qiPaiGOList.Count - 1].transform.position);
        pengGang.HideMJSCanGang();
    }


    public void ClearAll()
    {
        shouPaiGOList.Clear();
        pengPaiGOList.Clear();
        qiPaiGOList.Clear();
        myMaJiangList.Clear();
    }
}
