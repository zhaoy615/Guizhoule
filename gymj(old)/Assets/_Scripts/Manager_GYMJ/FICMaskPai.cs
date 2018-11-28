using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using MJBLL.common;
using DNL;
using System.Text;

/// <summary>
/// 手牌缺一门，听牌，弃牌区提示遮罩
/// </summary>
public class FICMaskPai : MonoBehaviour
{

    private FICStartGame startGame;
    private FICpaipaipai paipaipai;
    private FICMyCards myCards;

    public bool isHaveTong = false;
    public bool isHaveTiao = false;
    public bool isHaveWan = false;
    //public  GameObject maskGO;

    public List<GameObject> maskList = new List<GameObject>();
    private List<GameObject> specialPaiList = new List<GameObject>();
    private List<GameObject> allPaisOnDesk = new List<GameObject>();
    private int lastHS = 0;
    private GameObject Interaction_UIGO;
    private GameObject QueYiMen;
    public Image southImage;
    public Vector3 southImageVec;
    public Vector3 southQuePos;

    private Button queWanButton;
    private Button queTiaoButton;
    private Button queTongButton;

    private int count = 1;//第一次点击缺一门的时候，手牌最后一张移动到摸牌区，只用一次

    private void Start()
    {
        // maskGO = Resources.Load("mask") as GameObject;
        paipaipai = gameObject.GetComponent<FICpaipaipai>();
        startGame = gameObject.GetComponent<FICStartGame>();
        myCards = gameObject.GetComponent<FICMyCards>();
        Interaction_UIGO = GameObject.Find("Interaction_UI");
        QueYiMen = Interaction_UIGO.transform.Find("can_que").gameObject;
        queWanButton = QueYiMen.transform.Find("btn_que_wan").GetComponent<Button>();
        queTiaoButton = QueYiMen.transform.Find("btn_que_tiao").GetComponent<Button>();
        queTongButton = QueYiMen.transform.Find("btn_que_tong").GetComponent<Button>();
        southImage = transform.Find("/Game_UI/Fixed_UI/Image_que/southImage").GetComponent<Image>();
        southImageVec = southImage.gameObject.transform.position;
        southQuePos = transform.Find("/Game_UI/Fixed_UI/Image_que/southImage1").transform.position;

        queWanButton.onClick.AddListener(delegate { OnQueWanButtonClick(); });
        //queTiaoButton.onClick.AddListener(OnQueTiaoButtonClick);
        //queTongButton.onClick.AddListener(OnQueTongButtonClick);

    }
    /// <summary>
    /// 缺一门，筒
    /// </summary>
    public void OnQueTongButtonClick()
    {
        GameInfo.queType = GameInfo.QueType.Tong;
        OnQueYiMenButtonClick(GameInfo.QueType.Tong);
        SendQueYiMenMessage(GameInfo.QueType.Tong);
        southImage.gameObject.SetActive(true);
        southImage.overrideSprite = startGame.QueTypeArray[1];
        southImage.transform.DOMove(southQuePos, 1f);
        southImage.SetNativeSize();
        southImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
        if (myCards.shouPaiGOList.Count == 14)
        {
            myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1].transform.position += myCards.shouPaiOffsetX;

        }

        QueYiMen.SetActive(false);
    }
    /// <summary>
    /// 缺一门，条
    /// </summary>
    public void OnQueTiaoButtonClick()
    {
        GameInfo.queType = GameInfo.QueType.Tiao;
        OnQueYiMenButtonClick(GameInfo.QueType.Tiao);
        SendQueYiMenMessage(GameInfo.QueType.Tiao);

        southImage.gameObject.SetActive(true);
        southImage.overrideSprite = startGame.QueTypeArray[2];
        southImage.transform.DOMove(southQuePos, 1f);
        southImage.SetNativeSize();
        southImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
        if (myCards.shouPaiGOList.Count == 14)
        {
            myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1].transform.position += myCards.shouPaiOffsetX;

        }

        QueYiMen.SetActive(false);
    }
    /// <summary>
    /// 缺一门，万
    /// </summary>
    public void OnQueWanButtonClick()
    {
        GameInfo.queType = GameInfo.QueType.Wan;
        OnQueYiMenButtonClick(GameInfo.QueType.Wan);
        SendQueYiMenMessage(GameInfo.QueType.Wan);

        southImage.gameObject.SetActive(true);
        southImage.overrideSprite = startGame.QueTypeArray[3];
        southImage.transform.DOMove(southQuePos, 1f);
        southImage.SetNativeSize();
        southImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
        if (myCards.shouPaiGOList.Count == 14)
        {
            myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1].transform.position += myCards.shouPaiOffsetX;

        }
        QueYiMen.SetActive(false);
    }
    /// <summary>
    /// 自动缺牌
    /// </summary>
    public void OnQue(GameInfo.QueType que)
    {
        GameInfo.queType = que;
        OnQueYiMenButtonClick(que);

        southImage.gameObject.SetActive(true);
        southImage.overrideSprite = startGame.QueTypeArray[(int)que];
        southImage.transform.DOMove(southQuePos, 1f);
        southImage.SetNativeSize();
        southImage.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 1);
        if (myCards.shouPaiGOList.Count == 14)
        {
            myCards.shouPaiGOList[myCards.shouPaiGOList.Count - 1].transform.position += myCards.shouPaiOffsetX;

        }
        QueYiMen.SetActive(false);
    }

    /// <summary>
    /// 当缺一门的牌确定后，将非缺门牌遮罩，只显示缺门牌，并排序到最右侧1-39筒。41-79条，81-119万，最开始调用的一次，之后的用其他方法
    /// </summary>
    /// <param name="queType"></param>
    public void OnQueYiMenButtonClick(GameInfo.QueType queType, bool ispaixu = true)
    {//每次调用的时候，清空所有遮罩，生成特定遮罩//修改为遍历所有手牌，而不只是清空masklist ，多个杠牌会出错？？
        try
        {
            for (int i = 0; i < myCards.shouPaiGOList.Count; i++)
            {
                myCards.shouPaiGOList[i].GetComponent<MeshRenderer>().material.color = startGame.resetColor;
                myCards.shouPaiGOList[i].GetComponent<MJ_Event>().isCanOut = true;
            }
        }
        catch (System.Exception ex)
        {

            OutLog.log("清空所有遮罩，异常！！！！！！！！！！！"+ex.ToString());
        }
        maskList.Clear();
        isHaveTiao = false;
        isHaveTong = false;
        isHaveWan = false;

        switch (queType)
        {
            case GameInfo.QueType.Tong:
                foreach (GameObject item in myCards.shouPaiGOList)
                {
                    if (item != null)
                    {
                        if (item.GetComponent<MJ_SP>().HS < 10)
                        {
                            isHaveTong = true;
                            item.GetComponent<MJ_SP>().HS += 200;
                        }
                    }
                }
                if (ispaixu)
                    myCards.SortAllShouPai();
                foreach (GameObject item in myCards.shouPaiGOList)
                {
                    if (item != null)
                    {
                        if (item.GetComponent<MJ_SP>().HS > 200)
                        {
                            item.GetComponent<MJ_SP>().HS -= 200;
                        }
                        else if (isHaveTong)
                        {
                            item.GetComponent<MeshRenderer>().material.color = startGame.darkColor;
                            item.GetComponent<MJ_Event>().isCanOut = false;
                            maskList.Add(item.gameObject);
                        }
                    }
                }

                break;
            case GameInfo.QueType.Tiao:
                foreach (GameObject item in myCards.shouPaiGOList)
                {
                    if (item != null)
                    {
                        if (item.GetComponent<MJ_SP>().HS > 10 && item.GetComponent<MJ_SP>().HS < 20)
                        {
                            isHaveTiao = true;
                            item.GetComponent<MJ_SP>().HS += 200;
                        }
                    }
                }
                if (ispaixu)
                    // GameInfo.PaiXu(myCards.shouPaiGOList, GameInfo.MJ_S_Flode);// 先排序，后减掉权重
                    myCards.SortAllShouPai();
                foreach (GameObject item in myCards.shouPaiGOList)
                {
                    if (item != null)
                    {
                        if (item.GetComponent<MJ_SP>().HS > 200)
                        {
                            item.GetComponent<MJ_SP>().HS -= 200;
                        }
                        else if (isHaveTiao)
                        {
                            item.GetComponent<MeshRenderer>().material.color = startGame.darkColor;
                            item.GetComponent<MJ_Event>().isCanOut = false;
                            maskList.Add(item.gameObject);
                        }
                    }
                }
                break;
            case GameInfo.QueType.Wan:
                foreach (GameObject item in myCards.shouPaiGOList)
                {
                    if (item != null)
                    {
                        if (item.GetComponent<MJ_SP>().HS > 20)
                        {
                            isHaveWan = true;
                        }
                    }
                }
                if (ispaixu)
                    //GameInfo.PaiXu(myCards.shouPaiGOList, GameInfo.MJ_S_Flode);
                    myCards.SortAllShouPai();
                foreach (GameObject item in myCards.shouPaiGOList)
                {
                    if (item != null)
                    {
                        if (item.GetComponent<MJ_SP>().HS < 20 && isHaveWan)
                        {
                            item.GetComponent<MeshRenderer>().material.color = startGame.darkColor;
                            item.GetComponent<MJ_Event>().isCanOut = false;
                            maskList.Add(item.gameObject);
                        }
                    }

                }

                break;
            case GameInfo.QueType.no:
                if (ispaixu)
                    //GameInfo.PaiXu(myCards.shouPaiGOList, GameInfo.MJ_S_Flode);
                    myCards.SortAllShouPai();
                break;
        }
     
    }
    void SendQueYiMenMessage(GameInfo.QueType queType)
    {
        //发送缺一门消息给服务器
        //var qym = SendQYM.CreateBuilder().SetType((int)queType).SetOpenid(GameInfo.OpenID).Build();
        //byte[] body = qym.ToByteArray();
        var qym = new SendQYM();
        qym.type = (int)queType;
        qym.openid = GameInfo.OpenID;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(qym);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 5011, body.Length, 0, body);
        Debug.Log("发送就缺一门数据 : " + qym.type);
        GameInfo.cs.Send(data);
    }


    /// <summary>
    /// 点击手牌，会给牌桌上相同花色的牌加一层遮罩,
    /// </summary>
    public void ShowSpecialPaiInTable(int hs)
    {
        if (hs != lastHS)
        {
            CloseSpecialPaiInTabel();
        }
        lastHS = hs;

        InitAllPaiOnDiskList();
        foreach (GameObject item in allPaisOnDesk)
        {
            if (item.GetComponent<MJ_SP>().HS == hs)
            {
                item.GetComponent<MeshRenderer>().material.color = startGame.darkColor;
                specialPaiList.Add(item);
            }
        }

    }
    /// <summary>
    /// 将桌面上所有能看到的牌加入集合，便于加遮罩
    /// </summary>
    private void InitAllPaiOnDiskList()
    {
        allPaisOnDesk.Clear();
        allPaisOnDesk.AddRange(paipaipai.qipaiEList);
        allPaisOnDesk.AddRange(paipaipai.pengpaiEList);
        allPaisOnDesk.AddRange(paipaipai.qipaiNList);
        allPaisOnDesk.AddRange(paipaipai.pengpaiNList);
        allPaisOnDesk.AddRange(paipaipai.qipaiWList);
        allPaisOnDesk.AddRange(paipaipai.pengpaiWList);
        // allPaisOnDesk.AddRange(paipaipai.qipaiSList);
        allPaisOnDesk.AddRange(myCards.qiPaiGOList);
        allPaisOnDesk.AddRange(myCards.pengPaiGOList);

    }

    public void CloseSpecialPaiInTabel()
    {
        foreach (var item in specialPaiList)
        {
            try
            {

                item.GetComponent<MeshRenderer>().material.color = startGame.resetColor;
            }
            catch { }

        }
        specialPaiList.Clear();
    }

    /// <summary>
    /// 清空本脚本所涉及的所有集合，位置信息
    /// </summary>
    public void ClearAllListsAndPositions()
    {

    }
}
