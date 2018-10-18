using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MJBLL.common;
using DNL;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.SceneManagement;

public enum ReconState
{
    initData,
    recon,
    recoverOperation
}
public class Reconnection : MonoBehaviour {

    FICStartGame ficSGame;
    bool isDown;
    public string NetworkIP;

    private void Start()
    {
        ficSGame = GetComponent<FICStartGame>();
        isDown = false;
        
        NetworkIP = Network.player.ipAddress;
        StartCoroutine(PingConnect());
    }
    private void Update()
    {
        if (GameInfo.returnAddServer != null)
        {
            switch (GameInfo.returnAddServer.status)
            {
                case 1:
                    GameInfo.UserHearbeat = 0; ReconServer();
                    break;
                case 2:
                    break;
            }
            GameInfo.returnAddServer = null;
        }

    }
    /// <summary>
    /// 清楚断线重连后需要刷新的物体
    /// </summary>
    public void OnClearObjList()
    {
        GameInfo.CardsNumber = 0;
       ficSGame.isBaoTing = false;
        //清除N方位
        OnDestroyObj(ref ficSGame.managerPai.shoupaiNList);
        OnDestroyObj(ref ficSGame.managerPai.qipaiNList);
        OnDestroyObj(ref ficSGame.managerPai.pengpaiNList);
        ficSGame.managerPai.nNextPos = ficSGame.managerPai.nFirstPengPaiTrans.position;
        OnDestroyObj(ficSGame.nJiGridTrans);

        //清除W方位
        OnDestroyObj(ref ficSGame.managerPai.shoupaiWList);
        OnDestroyObj(ref ficSGame.managerPai.qipaiWList);
        OnDestroyObj(ref ficSGame.managerPai.pengpaiWList);
        ficSGame.managerPai.wNextPos = ficSGame.managerPai.wFirstPengPaiTrans.position;
        OnDestroyObj(ficSGame.wJiGridTrans);

        //清除E方位
        OnDestroyObj(ref ficSGame.managerPai.shoupaiEList);
        OnDestroyObj(ref ficSGame.managerPai.qipaiEList);
        OnDestroyObj(ref ficSGame.managerPai.pengpaiEList);
        ficSGame.managerPai.eNextPos = ficSGame.managerPai.eFirstPengPaiTrans.position;
        OnDestroyObj(ficSGame.eJiGridTrans);

        //清除S方位
        OnDestroyObj(ref ficSGame.myCards.shouPaiGOList);
        OnDestroyObj(ref ficSGame.myCards.qiPaiGOList);
        OnDestroyObj(ref ficSGame.myCards.pengPaiGOList);
        ficSGame.myCards.nextPengPaiPos = ficSGame.myCards.firstPengPaiTrans.position;
        ficSGame.myCards.firstShouPaiTrans.position = ficSGame.myCards.newFirstShouPaiTrans;
        OnDestroyObj(ficSGame.sJiGridTrans);
        //清除牌堆
        OnDestroyObj(ref ficSGame.paidui._ArrayDownPais);
        OnDestroyObj(ref ficSGame.paidui._ArrayUpPais);
        ficSGame.paidui.isSkip = true;

        ficSGame.ClearJieSuan();
        ficSGame.ClearJiPanel();
        if (GameInfo.cloneMjs != null && GameInfo.cloneMjs.Count > 0)
        {
            foreach (var item in GameInfo.cloneMjs)
            {
                Destroy(item);
            }
        }
        //清除天听
        for (int i = 4; i < ficSGame.deskSkillTran.childCount; i++)
        {
            Destroy(ficSGame.deskSkillTran.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// 循环Transform，清除子物体
    /// </summary>
    /// <param name="tempObjs">父物体</param>
    private void OnDestroyObj(Transform tempObjs)
    {
        foreach (Transform item in tempObjs)
        {
            Destroy(item.gameObject);
        }
    }
    /// <summary>
    /// 循环游戏对象集合，清除对象
    /// </summary>
    /// <param name="tempObjs">游戏对象集合</param>
    private void OnDestroyObj(ref List<GameObject> tempObjs)
    {
        foreach (var item in tempObjs)
        {
            DestroyImmediate(item,true);
        }
        tempObjs = new List<GameObject>();
    }

    /// <summary>
    /// 循环游戏对象数组，清除对象
    /// </summary>
    /// <param name="tempObjs">游戏对象集合</param>
    private void OnDestroyObj(ref GameObject[] tempObjs)
    {
        foreach (var item in tempObjs)
        {
            DestroyImmediate(item, true);
        }
        tempObjs = new GameObject[54];
    }
    /// <summary>
    /// 显示缺一门信息
    /// </summary>
    /// <param name="fw">方位</param>
    /// <param name="qym">缺一门</param>
    public void OnShowQYM(int fw,int qym)
    {
        if (qym == 0)
        {

            if (fw == GameInfo.FW)
            { ficSGame.QueYiMen.SetActive(true); }
            else
            {
                ficSGame.dingquezhongGO.SetActive(true);
                switch (GameInfo.GetFW(fw))
                {
                    case FW.East:
                        ficSGame.dingquezhongGO.transform.Find("east_queconfiging").gameObject.SetActive(true);
                        break;
                    case FW.West:
                        ficSGame.dingquezhongGO.transform.Find("west_queconfiging").gameObject.SetActive(true);
                        break;
                    case FW.North:
                        ficSGame.dingquezhongGO.transform.Find("north_queconfiging").gameObject.SetActive(true);
                        break;
                }
            }
            return;
        }
        Sprite tempSpe = qym == 1 ? ficSGame.QueTypeArray[1] : qym == 2 ? ficSGame.QueTypeArray[2] : ficSGame.QueTypeArray[3];
        switch (GameInfo.GetFW(fw))
        {
            case FW.East:
                ficSGame.eastImage.gameObject.SetActive(true);
                ficSGame.eastImage.overrideSprite = tempSpe;
                ficSGame.eastImage.SetNativeSize();
                ficSGame.eastImage.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                ficSGame.eastImage.transform.position = ficSGame.eastQuePos;
                break;
            case FW.West:
                ficSGame.westImage.gameObject.SetActive(true);
                ficSGame.westImage.overrideSprite = tempSpe;
                ficSGame.westImage.SetNativeSize();
                ficSGame.westImage.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                ficSGame.westImage.transform.position = ficSGame.westQuePos;
                break;
            case FW.North:
                ficSGame.northImage.gameObject.SetActive(true);
                ficSGame.northImage.overrideSprite = tempSpe;
                ficSGame.northImage.SetNativeSize();
                ficSGame.northImage.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                ficSGame.northImage.transform.position = ficSGame.northQuePos;
                break;
            case FW.South:
                GameInfo.queType = (GameInfo.QueType)qym;
                ficSGame.maskPai.southImage.gameObject.SetActive(true);
                ficSGame.maskPai.southImage.overrideSprite = tempSpe;
                ficSGame.maskPai.southImage.SetNativeSize();
                ficSGame.maskPai.southImage.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                ficSGame.maskPai.southImage.transform.position = ficSGame.maskPai.southQuePos;
                break;
        }
    }
    /// <summary>
    /// 获取鸡牌位置
    /// </summary>
    /// <param name="fw"></param>
    /// <returns></returns>
    public Transform GetJiPaiTrans(int fw)
    {
        switch (GameInfo.GetFW(fw))
        {
            case FW.East:
                return ficSGame.eJiGridTrans;
            case FW.West:
                return ficSGame.wJiGridTrans;
            case FW.North:
                return ficSGame.nJiGridTrans;
            case FW.South:
                return ficSGame.sJiGridTrans;
            default:
                return null;
        }
    }
    /// <summary>
    /// 重连听牌
    /// </summary>
    /// <param name="fw"></param>
    /// <param name="tingState"></param>
    /// <param name="tingName"></param>
    public void ReconTingPaiInfo(FW fw,int tingState ,string tingName)
    {
        GameObject tempGo = new GameObject();
        Transform tempTf = transform;
        switch (fw)
        {
            case FW.South:
                 tempTf = ficSGame.AniJi_S.transform;
                break;
            case FW.East:
                 tempTf = ficSGame.AniJi_E.transform;
                break;
            case FW.North:
                 tempTf = ficSGame.AniJi_N.transform;
                break;
            case FW.West:
                 tempTf = ficSGame.AniJi_W.transform;
                break;
        }

        if (tingName == "tianting")
        {
            GameInfo.isTT = true;
            GameInfo.isRealTing = true;
            tempGo = ficSGame.deskSkillGOArray[9];
        }
        else
        {
            GameInfo.IsTingPai = true;
            GameInfo.isRealTing = true;
            ficSGame.isBaoTing = true;
            tempGo = ficSGame.deskSkillGOArray[2];
        }
        switch (tingState)
        {
            case -1:
                ficSGame.tingButtonGO.SetActive(true);
                ficSGame.guoButtonGO.SetActive(true);
                break;
            case 1:
               // GameObject.Instantiate(ficSGame.tingGO, GetJiPaiTrans((int)fw));
                GameObject go = GameObject.Instantiate(tempGo, tempTf.position, Quaternion.identity, ficSGame.deskSkillTran) as GameObject;
                go.transform.Find("Big1boom").gameObject.SetActive(false);
                break;
        }
    }
    public void ReconTingPaiInfo(FW fw,string tingName,int tingState)
    {
        GameObject tempGo = new GameObject();
        Transform tempTf = transform;
        switch (fw)
        {
            case FW.South:
                tempTf = ficSGame.AniJi_S.transform;
                break;
            case FW.East:
                tempTf = ficSGame.AniJi_E.transform;
                break;
            case FW.North:
                tempTf = ficSGame.AniJi_N.transform;
                break;
            case FW.West:
                tempTf = ficSGame.AniJi_W.transform;
                break;
        }

        if (tingName == "tianting")
        {
            tempGo = ficSGame.deskSkillGOArray[9];

        }
        else
        {
            tempGo = ficSGame.deskSkillGOArray[2];
        }
        switch (tingState)
        {
            case -1:
                break;
            case 1:
                GameObject go = GameObject.Instantiate(tempGo, tempTf.position, Quaternion.identity, ficSGame.deskSkillTran) as GameObject;
                go.transform.Find("Big1boom").gameObject.SetActive(false);
                break;
        }
    }
    /// <summary>
    /// 移动指示器
    /// </summary>
    /// <param name="tempList"></param>
    /// <param name="hs"></param>
    /// <param name="id"></param>
    public void OnMoveDiamond(List<GameObject> tempList,int hs,int id)
    {
        var info = tempList.FirstOrDefault(w => w.GetComponent<MJ_SP>().HS == hs && w.GetComponent<MJ_SP>().ID == id);
        if (info != null)
        {
            ficSGame.managerPai.ResetDiamondPosition(info.transform.position);
        }
    }
    public IEnumerator PingConnect()
    {
        while (ficSGame.isGameServer)
        {
            try
            {
                if (NetworkIP != Network.player.ipAddress)
                {
                    if (!GameInfo.isAllreadyStart)
                    {
                        GameInfo.cs.Closed();
                        GameInfo.cs.serverType = ServerType.ListServer;
                        FICWaringPanel._instance.Show("您的IP发生改变，房间已解散或退出，点击确定返回大厅！");
                        FICWaringPanel._instance.WarnningMethods = delegate { SceneManager.LoadScene("Scene_Haaaaaaaaaaaaaaaaaaaaaaaaaall"); };
                        NetworkIP = Network.player.ipAddress;
                    }
                    else
                    {
                        GameInfo.cs.Closed();
                        NetworkIP = Network.player.ipAddress;
                        AddServer();
                    }
                }
                else if (!GameInfo.cs.Connected)
                {
                    AddServer();
                }
              
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void ShowScore(int fw,int score)
    {
        GameInfo.integrals.Add(fw, score);
        ficSGame.managerGame.ShowIntegral(fw,score);
    }
    public void ReconServer()
    {
        GameInfo.cs.Closed();
        SendAddServer sendAddServer = new SendAddServer();
        sendAddServer.openid = GameInfo.OpenID;
        sendAddServer.unionid = GameInfo.unionid;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendAddServer);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1011, body.Length, 0, body);
        GameInfo.cs.Send(data);
        ficSGame.isRecon = true;
    }
    void AddServer()
    {
        SendAddServer sendAddServer = new SendAddServer();
        sendAddServer.openid = GameInfo.OpenID;
        sendAddServer.unionid = GameInfo.unionid;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendAddServer);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1011, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }
    public void clearReturn()
    {
        GameInfo.returnMsg = null;
        GameInfo.returnAll = null;
        GameInfo.returnHyUser = null;
        GameInfo.returnTTOrTH = null;
        GameInfo.returnAddRoom = null;
        GameInfo.returnMP = null;
        GameInfo.returnPeng = null;
        GameInfo.returnGang = null;
        GameInfo.returnMsgList = null;
        GameInfo.returnFJ = null;
        GameInfo.returnBTMSG = null;
        GameInfo.returnTT = null;
        GameInfo.returnTP = null;
        GameInfo.returnAYM = null;
        GameInfo.returnHByType = null;
        GameInfo.returnJS = null;
        GameInfo.returnPaiCount = null;
        GameInfo.returnHType = null;
        GameInfo.returnVoice = null;
        //GameInfo.returnDJS = null;
    }
}

