using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DNL;

public class GamePlayback : MonoBehaviour {

    private FICStartGame startGame;
    private GameObject playback_Btns;
    private int operationIndex = -1;
    private Sprite[] pauseSp = new Sprite[2];  
    private bool isSpeed = false;
    private bool isReverse = false;
    private bool isPause = false;
    private bool isEnd = false;
    void Start () {
        startGame = GetComponent<FICStartGame>();
        playback_Btns = transform.Find("/Game_UI/Fixed_UI/playback_Btns").gameObject;
        playback_Btns.transform.Find("back").GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("Scene_Hall"); RelievePause(); });
        playback_Btns.transform.Find("reverse").GetComponent<Button>().onClick.AddListener(delegate { isReverse = true; RelievePause(); });
        playback_Btns.transform.Find("pause").GetComponent<Button>().onClick.AddListener(delegate {
            isPause = !isPause;
        });
        playback_Btns.transform.Find("speed").GetComponent<Button>().onClick.AddListener(delegate { isSpeed = true; RelievePause(); });
        playback_Btns.transform.Find("end").GetComponent<Button>().onClick.AddListener(delegate { isEnd = true; RelievePause(); });
        pauseSp[0] = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_UI/play");
        pauseSp[1] = Resources.Load<Sprite>("Game_GYMJ/Texture/Game_UI/pause");
    }
    public IEnumerator OperationPlayback(IList<GameOperationInfo> gameOperationInfo , byte[] bytes)
    {
        GameInfo.speed = 1f;
        isPause = false;
        isReverse = false;
        isSpeed = false;
        isEnd = false;

        playback_Btns.SetActive(true);



        var data =bytes;
        GameInfo.cunJS = ProtobufUtility.DeserializeProtobuf<ReturnJS>(data);
        startGame.isShowFanji = false;


       
        yield return new WaitForSeconds(2);
        startGame.managerGame.juNum.text = "回放";
        startGame.managerGame.juNum.gameObject.SetActive(true);
        for (int i = 0; i < gameOperationInfo.Count; i++)
        {
            OnOperation(gameOperationInfo[i].OperationFW,ref i);
            Debug.Log(gameOperationInfo[i].OperationType);
            if (isPause) { yield return 0; continue; }
            startGame.SeZiQi(gameOperationInfo[i].OperationFW);
            switch (gameOperationInfo[i].OperationType)
            {
                case 0:
                    FaPai(gameOperationInfo[i].OperationFW, gameOperationInfo[i].MJ);
                    break;
                case 1:
                    MoPai(gameOperationInfo[i].OperationFW, gameOperationInfo[i].MJ);
                    break;
                case 2:
                    ChuPai(gameOperationInfo[i].OperationFW, gameOperationInfo[i].MJType, gameOperationInfo[i].MJ);
                    break;
                case 3:
                    PengPai(gameOperationInfo[i].OperationFW, gameOperationInfo[i].PengFW, gameOperationInfo[i].MJ);
                    break;
                case 4:
                    GangPai(gameOperationInfo[i].OperationFW, gameOperationInfo[i].PengFW, gameOperationInfo[i].TingHuType, gameOperationInfo[i].MJ);
                    break;
                case 5:
                    Hu(gameOperationInfo[i].OperationFW, gameOperationInfo[i].PaoFW, gameOperationInfo[i].DXInfo);
                    break;
                case 6:
                    Ting(gameOperationInfo[i].OperationFW, gameOperationInfo[i].TingHuType);
                    break;
                case 7:
                    Guo();
                    break;
                case 8:
                    QueYiMen(gameOperationInfo[i].OperationFW, gameOperationInfo[i].QYM);
                    break;
                case 9:
                    startGame.isShowFanji = true;
                    startGame.fanjipaihs = gameOperationInfo[i].MJ[0].PaiHS;
                    break;
            }
            if(GameInfo.speed != 0) yield return new WaitForSeconds(GameInfo.speed);
        }
        yield return new WaitForSeconds(3);
        startGame.ShowJieSuanPanel(); 
        startGame.jiesuanPanel.transform.Find("littlesettlement/btn_continue").gameObject.SetActive(false);
        startGame.jiesuanPanel.transform.Find("littlesettlement/btn_showfinal").gameObject.SetActive(false);
        startGame.jiesuanPanel.transform.Find("finalsettlement").gameObject.SetActive(true);
    }
    public void OnOperation(int oFw,ref int op)
    {
        //暂停
        if (isPause) { op--; playback_Btns.transform.Find("pause").GetComponent<Image>().sprite = pauseSp[0]; }
        else {  playback_Btns.transform.Find("pause").GetComponent<Image>().sprite = pauseSp[1]; }
        //结束
        if (isEnd) { GameInfo.speed = 0; }
        //减速
        if (isReverse)
        {
            startGame.recon.OnClearObjList();
            GameInfo.speed = 0;
            operationIndex = op - 5;
            if (operationIndex < 0) operationIndex = 0;
             op = 0;
            isReverse = false;
        }
        //加速
        if (isSpeed)
        {
            GameInfo.speed = 0;
            operationIndex = op + 5;
            isSpeed = false;
        }
        if (op == operationIndex) { GameInfo.speed = 1f; }// isPause = true; }
    }
    public void RelievePause()
    {
        isPause = false;
        Time.timeScale = 1;
    }
    public void FaPai(int fw ,IList<MaJiang> mj)
    {
        if (GameInfo.FW == (int)fw)
        {
            startGame.myCards.CreatCardsAtStart(mj);
            foreach (var item in startGame.myCards.shouPaiGOList)
            {
             item.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            startGame.managerPai.CeretShouPai(GameInfo.GetFW(fw), mj.Count);
            List<GameObject> gos = ReturnShouPaiFwList(fw);
            for (int i = 0; i < gos.Count; i++)
            {
                try
                {
                    ShowMJHS(gos[i], mj[i].PaiHS, mj[i].PaiID);
                }
                catch (System.Exception)
                {
                }
            }
            PBSortAllShouPai(ReturnShouPaiFwList(fw),fw);
        }
    }
    public void MoPai(int fw, IList<MaJiang> mj)
    {
        if (GameInfo.FW == (int)fw)
        {
            startGame.myCards.MoPai(mj[0].PaiHS, mj[0].PaiID);
            startGame.myCards.shouPaiGOList[startGame.myCards.shouPaiGOList.Count - 1].GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            startGame.managerPai.MoPai(GameInfo.GetFW(fw));
            List<GameObject> gos = ReturnShouPaiFwList(fw);
            ShowMJHS(gos[gos.Count - 1], mj[0].PaiHS, mj[0].PaiID);
        }
    }
    public void ChuPai(int fw, string mjType,IList<MaJiang> mj)
    {
        if (GameInfo.FW == fw)
        {
            var mjInfo = startGame.myCards.shouPaiGOList.Find(w => w.GetComponent<MJ_SP>().ID == mj[0].PaiID);
            startGame.myCards.DestroyThisShouPai(mjInfo.gameObject);
            startGame.managerPai.ChuPai(GameInfo.GetFW(fw), mj[0].PaiHS, mj[0].PaiID);
        }
        else
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            GameObject go = ReturnShouPaiFwList(fw).Find(w => w.GetComponent<MJ_SP>().ID == mj[0].PaiID);
            ReturnShouPaiFwList(fw).Remove(go);
            Transform tf = transform;
            Transform qipaiTf = transform;
            List<GameObject> qipaiList = new List<GameObject>();
            Vector3 tempX = new Vector3();
            Vector3 tempZ = new Vector3();
            switch (GameInfo.GetFW(fw))
            {
                case FW.East:
                    tf = startGame.managerPai.eFirstQiPaiTrans;
                    qipaiTf = startGame.managerPai.eQiPaiContainerTrans;
                    qipaiList = startGame.managerPai.qipaiEList;
                    tempX = startGame.managerPai.eOffsetX;
                    tempZ = startGame.managerPai.eQiOffsetZ;
                    break;
                case FW.West:
                    tf = startGame.managerPai.wFirstQiPaiTrans;
                    qipaiTf = startGame.managerPai.wQiPaiContainerTrans;
                    qipaiList = startGame.managerPai.qipaiWList;
                    tempX = startGame.managerPai.wOffsetX;
                    tempZ = startGame.managerPai.wQiOffsetZ;
                    break;
                case FW.North:
                    tf = startGame.managerPai.nFirstQiPaiTrans;
                    qipaiTf = startGame.managerPai.nQiPaiContainerTrans;
                    qipaiList = startGame.managerPai.qipaiNList;
                    tempX = startGame.managerPai.nOffsetX;
                    tempZ = startGame.managerPai.nQiOffsetZ;
                    break;
            }
            qipaiList.Add(go);
            int k = qipaiList.Count - 1;
            qipaiList[k].layer = 0;
            if (GameInfo.room_peo != 2)
            {
                qipaiList[k].transform.position = tf.position + (k % 7) * tempX + (k / 7) * tempZ;
                if(GameInfo.GetFW(fw) != FW.West && GameInfo.GetFW(fw) != FW.East) qipaiList[k].transform.localPosition += Vector3.forward * 0.4f;
            }
            else
            {
                qipaiList[k].transform.position = (tf.position - 3 * tempX) + (k % 13) * tempX + (k / 13) * tempZ;
                if (GameInfo.GetFW(fw) != FW.West && GameInfo.GetFW(fw) != FW.East) qipaiList[k].transform.localPosition += Vector3.forward * 0.4f;
            }
            qipaiList[k].transform.SetParent(qipaiTf);
            startGame.managerPai.ResetDiamondPosition(qipaiList[k].transform.position);
            PBSortAllShouPai(ReturnShouPaiFwList(fw), fw);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        if (mjType != "" && GameInfo.speed != 0)
        {
            int tXIndex = 0;
            switch (mjType)
            {
                case "CFWG":
                    tXIndex = 6;
                    break;
                case "CFJ":
                    tXIndex = 5;
                    break;
                case "ZRJ":
                    if (mj[0].PaiHS == 8) tXIndex = 7;
                    else tXIndex = 8;
                    break;
            }
            switch (GameInfo.GetFW(fw))
            {
                case FW.East:
                    Instantiate(startGame.deskSkillGOArray[tXIndex], startGame.AniJi_E.transform.position, Quaternion.identity, startGame.deskSkillTran);
                    break;
                case FW.West:
                    Instantiate(startGame.deskSkillGOArray[tXIndex], startGame.AniJi_W.transform.position, Quaternion.identity, startGame.deskSkillTran);
                    break;
                case FW.North:
                    Instantiate(startGame.deskSkillGOArray[tXIndex], startGame.AniJi_N.transform.position, Quaternion.identity, startGame.deskSkillTran);
                    break;
                case FW.South:
                    Instantiate(startGame.deskSkillGOArray[tXIndex], startGame.AniJi_S.transform.position, Quaternion.identity, startGame.deskSkillTran);
                    break;
            }
        }
    }
    public void PengPai(int fw,int pengFw, IList<MaJiang> mj)
    {
        if (GameInfo.FW == (int)fw)
        {
            startGame.myCards.PengPaiEntir(mj[0].PaiHS);
            startGame.managerPai.DelQIpai(GameInfo.GetFW(pengFw));
        }
        else
        {
            GameObject.Destroy(ReturnShouPaiFwList(fw).Find(w => w.GetComponent<MJ_SP>().HS == mj[0].PaiHS));
            ReturnShouPaiFwList(fw).RemoveAt(ReturnShouPaiFwList(fw).FindIndex(w => w.GetComponent<MJ_SP>().HS == mj[0].PaiHS));
            GameObject.Destroy(ReturnShouPaiFwList(fw).Find(w => w.GetComponent<MJ_SP>().HS == mj[0].PaiHS));
            ReturnShouPaiFwList(fw).RemoveAt(ReturnShouPaiFwList(fw).FindIndex(w => w.GetComponent<MJ_SP>().HS == mj[0].PaiHS));
            startGame.managerPai.PengGangPai( GameInfo.GetFW(fw), GameInfo.GetFW(pengFw), mj[0].PaiHS, PPTYPE.Peng,false);
            PBSortAllShouPai(ReturnShouPaiFwList(fw), fw);
        }
        if(GameInfo.speed != 0) startGame.ShowDeskSkillsOnDirection(GameInfo.GetFW(fw),PPTYPE.Peng);
    }
    public void GangPai(int fw, int gangFw,int gangType, IList<MaJiang> mj)
    {
        if (GameInfo.FW == fw)
        {
            startGame.myCards.DestroyGangShouPais(mj[0].PaiHS);
            switch (gangType)
            {
                case 4:
                    break;
                case 5:
                    startGame.myCards.AnGang(mj[0].PaiHS);
                    break;
                case 6:
                    startGame.myCards.ZhuanWanGang(mj[0].PaiHS);
                    break;
                case 7:
                    startGame.myCards.HanBaoGang(mj[0].PaiHS);
                    break;
            }
            startGame.myCards.firstShouPaiTrans.position = startGame.myCards.newFirstShouPaiTrans + (13 - startGame.myCards.shouPaiGOList.Count) * startGame.myCards.shouPaiOffsetX;
            if (gangType.Equals(GangType.mingGang))
            {
                startGame.managerPai.DelQIpai((FW)gangFw);
            }
        }
        else
        {
            string type = "";
            switch (gangType)
            {
                case 4:
                    type = "M";
                    break;
                case 5:
                    type = "A";
                    break;
                case 6:
                    type = "Z";
                    break;
                case 7:
                    type = "H";
                    break;
            }


            while (ReturnShouPaiFwList(fw).Find(w => w.GetComponent<MJ_SP>().HS == mj[0].PaiHS))
            {
                GameObject.Destroy(ReturnShouPaiFwList(fw).Find(w => w.GetComponent<MJ_SP>().HS == mj[0].PaiHS));
                ReturnShouPaiFwList(fw).RemoveAt(ReturnShouPaiFwList(fw).FindIndex(w => w.GetComponent<MJ_SP>().HS == mj[0].PaiHS));
            }
            startGame.managerPai.PengGangPai(GameInfo.GetFW(fw), GameInfo.GetFW(gangFw), mj[0].PaiHS, PPTYPE.Gang, type == "Z"?true:false, type);
            startGame.managerPai.DelQIpai(GameInfo.GetFW(gangFw));
            PBSortAllShouPai(ReturnShouPaiFwList(fw), fw);
        }
        if (GameInfo.speed != 0) startGame.ShowDeskSkillsOnDirection(GameInfo.GetFW(fw), PPTYPE.Gang);
    }
    public void Hu(int fw,int pFw,IList<DuoXiangHu> duoXiang)
    {
        foreach (var item in duoXiang)
        {
            switch (GameInfo.GetFW(item.DXFW))
            {
                case FW.East:
                    Instantiate(startGame.huSkillGOArray[GameInfo.cunJS.js[item.DXFW - 1].huType], startGame.AniJi_E.transform.position, Quaternion.identity, startGame.deskSkillTran);
                    break;
                case FW.West:
                    Instantiate(startGame.huSkillGOArray[GameInfo.cunJS.js[item.DXFW - 1].huType], startGame.AniJi_W.transform.position, Quaternion.identity, startGame.deskSkillTran);
                    break;
                case FW.North:
                    Instantiate(startGame.huSkillGOArray[GameInfo.cunJS.js[item.DXFW - 1].huType], startGame.AniJi_N.transform.position, Quaternion.identity, startGame.deskSkillTran);
                    break;
                case FW.South:
                    Instantiate(startGame.huSkillGOArray[GameInfo.cunJS.js[item.DXFW - 1].huType], startGame.AniJi_S.transform.position, Quaternion.identity, startGame.deskSkillTran);
                    break;
            }
        }
        switch (GameInfo.GetFW(fw))
        {
            case FW.East:
                Instantiate(startGame.huSkillGOArray[GameInfo.cunJS.js[fw - 1].huType], startGame.AniJi_E.transform.position, Quaternion.identity, startGame.deskSkillTran);
                break;
            case FW.West:
                Instantiate(startGame.huSkillGOArray[GameInfo.cunJS.js[fw - 1].huType], startGame.AniJi_W.transform.position, Quaternion.identity, startGame.deskSkillTran);
                break;
            case FW.North:
                Instantiate(startGame.huSkillGOArray[GameInfo.cunJS.js[fw - 1].huType], startGame.AniJi_N.transform.position, Quaternion.identity, startGame.deskSkillTran);
                break;
            case FW.South:
                Instantiate(startGame.huSkillGOArray[GameInfo.cunJS.js[fw - 1].huType], startGame.AniJi_S.transform.position, Quaternion.identity, startGame.deskSkillTran);
                break;
        }
    }
    public void Ting(int fw, int TingType)
    {
        int index = TingType == 2 ? 2 : TingType == 3 ? 9:0;
        switch (GameInfo.GetFW(fw))
        {
            case FW.East:
                GameObject.Instantiate(startGame.deskSkillGOArray[index], startGame.AniJi_E.transform.position, Quaternion.identity, startGame.deskSkillTran);
                //GameObject gojie = GameObject.Instantiate(startGame.tingGO, startGame.eJiGridTrans) as GameObject;
                //gojie.transform.localPosition = Vector3.zero;
                //gojie.transform.localScale = Vector3.one;
                break;
            case FW.West:
                GameObject.Instantiate(startGame.deskSkillGOArray[index], startGame.AniJi_W.transform.position, Quaternion.identity, startGame.deskSkillTran);
                //GameObject gojiw = GameObject.Instantiate(startGame.tingGO, startGame.wJiGridTrans) as GameObject;
                //gojiw.transform.localPosition = Vector3.zero;
                //gojiw.transform.localScale = Vector3.one;
                break;
            case FW.North:
                GameObject.Instantiate(startGame.deskSkillGOArray[index], startGame.AniJi_N.transform.position, Quaternion.identity, startGame.deskSkillTran);
                //GameObject gojin = GameObject.Instantiate(startGame.tingGO, startGame.nJiGridTrans) as GameObject;
                //gojin.transform.localPosition = Vector3.zero;
                //gojin.transform.localScale = Vector3.one;
                break;
            case FW.South:
                GameObject.Instantiate(startGame.deskSkillGOArray[index], startGame.AniJi_S.transform.position, Quaternion.identity, startGame.deskSkillTran);
                //GameObject gojis = GameObject.Instantiate(startGame.tingGO, startGame.sJiGridTrans) as GameObject;
                //gojis.transform.localPosition = Vector3.zero;
                //gojis.transform.localScale = Vector3.one;
                break;
        }
    }
    public void Guo()
    {
        
    }
    public void QueYiMen(int fw, int QueType)
    {
        if (GameInfo.FW == fw)
        {
            switch ((GameInfo.QueType)QueType)
            {
                case GameInfo.QueType.Tong:
                    startGame.maskPai.OnQueTongButtonClick();
                    break;
                case GameInfo.QueType.Tiao:
                    startGame.maskPai.OnQueTiaoButtonClick();
                    break;
                case GameInfo.QueType.Wan:
                    startGame.maskPai.OnQueWanButtonClick();
                    break;
            }
        }
        else
        {
            startGame.dingquezhongGO.transform.Find("east_queconfiging").gameObject.SetActive(false);
            startGame.dingquezhongGO.transform.Find("west_queconfiging").gameObject.SetActive(false);
            startGame.dingquezhongGO.transform.Find("north_queconfiging").gameObject.SetActive(false);
            switch (GameInfo.GetFW(fw))
            {
                case global::FW.East:
                    startGame.eastImage.gameObject.SetActive(true);
                    startGame.eastImage.overrideSprite = startGame.QueTypeArray[QueType];
                    startGame.eastImage.transform.DOMove(startGame.eastQuePos, 1f);
                    startGame.eastImage.SetNativeSize();
                    startGame.eastImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
                    break;
                case global::FW.West:
                    startGame.westImage.gameObject.SetActive(true);
                    startGame.westImage.overrideSprite = startGame.QueTypeArray[QueType];
                    startGame.westImage.transform.DOMove(startGame.westQuePos, 1f);
                    startGame.westImage.SetNativeSize();
                    startGame.westImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
                    break;
                case global::FW.North:
                    startGame.northImage.gameObject.SetActive(true);
                    startGame.northImage.overrideSprite = startGame.QueTypeArray[QueType];
                    startGame.northImage.transform.DOMove(startGame.northQuePos, 1f);
                    startGame.northImage.SetNativeSize();
                    startGame.northImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
                    break;
            }
        }
        GameInfo.IsDingQue = false;
    }
    public List<GameObject> ReturnShouPaiFwList(int fw)
    {
        switch (GameInfo.GetFW(fw))
        {
            case FW.East:
                return startGame.managerPai.shoupaiEList;
            case FW.West:
                return startGame.managerPai.shoupaiWList;
            case FW.North:
                return startGame.managerPai.shoupaiNList;
        }
        return null;
    }
    public void ShowMJHS(GameObject go ,int hs,int id)
    {
        go.GetComponent<MeshRenderer>().material.mainTexture = startGame.myCards.textureArray[hs];
        go.GetComponent<MJ_SP>().HS = hs;
        go.GetComponent<MJ_SP>().ID = id;
    }
    public void PBSortAllShouPai(List<GameObject> SPGOLost , int fw)
    {
        Vector3 tempPos;
        GameObject sortTempGO;
        for (int i = 0; i < SPGOLost.Count; i++)
        {
            for (int j = 0; j < SPGOLost.Count - i - 1; j++)
            {
                if (SPGOLost[j].GetComponent<MJ_SP>().HS > SPGOLost[j + 1].GetComponent<MJ_SP>().HS)
                {

                    tempPos = SPGOLost[j + 1].transform.position;
                    sortTempGO = SPGOLost[j + 1];
                    SPGOLost[j + 1].transform.position = SPGOLost[j].transform.position;
                    SPGOLost[j + 1] = SPGOLost[j];
                    SPGOLost[j].transform.position = tempPos;
                    SPGOLost[j] = sortTempGO;
                }
            }
        }
        for (int i = 0; i < SPGOLost.Count; i++)
        {
            
            switch (GameInfo.GetFW(fw))
            {
                case FW.East:
                    SPGOLost[i].transform.position = startGame.managerPai.eFirstShouPaiTrans.position + startGame.managerPai.eOffsetX * (i + 13 - SPGOLost.Count);
                    break;
                case FW.West:
                    SPGOLost[i].transform.position = startGame.managerPai.wFirstShouPaiTrans.position + startGame.managerPai.wOffsetX * (i + 13 - SPGOLost.Count);
                    break;
                case FW.North:
                    SPGOLost[i].transform.position = startGame.managerPai.nFirstShouPaiTrans.position + startGame.managerPai.nOffsetX * (i +  13 - SPGOLost.Count);
                    break;
                case FW.South:
                    SPGOLost[i].transform.position = startGame.myCards.firstShouPaiTrans.position + startGame.myCards.shouPaiOffsetX * i;
                    break;
            }
        }
    }
}
