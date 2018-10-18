using DDZ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp_UI : Singleton<PopUp_UI>
{
    private Transform WestHand;
    private Transform EastHand;
    private Transform SouthHand;
    private Text CardsCount_East;
    private Text CardsCount_West;

    private Transform WestQiPaiArea;
    private Transform EastQiPaiArea;
    private Transform SouthQiPaiArea;

    private Dictionary<int, Sprite> OwnHsList = new Dictionary<int, Sprite>();
    private Dictionary<int, Sprite> QiPaiHsList = new Dictionary<int, Sprite>();

    public List<Transform> westCardList = new List<Transform>();
    public List<Transform> eastCardList = new List<Transform>();

    private void Start()
    {
        InitUI();
    }
    void InitUI()
    {
        WestHand = transform.Find("WestHand");
        EastHand = transform.Find("EastHand");
        SouthHand = transform.Find("SouthHand");
        CardsCount_East = EastHand.Find("count").GetComponent<Text>();
        CardsCount_West = WestHand.Find("count").GetComponent<Text>();
        WestQiPaiArea = transform.Find("WestQiPaiArea");
        EastQiPaiArea = transform.Find("EastQiPaiArea");
        SouthQiPaiArea = transform.Find("SouthQiPaiArea");

        foreach (Sprite item in Resources.LoadAll<Sprite>("Game_DDZ/Textures/CardsFaceBig"))
        {
            OwnHsList.Add(int.Parse(item.name),item);
        }
        foreach (Sprite item in Resources.LoadAll<Sprite>("Game_DDZ/Textures/CardsFaceMid"))
        {
            QiPaiHsList.Add(int.Parse(item.name), item);
        }
    }
    /// <summary>
    /// 出牌
    /// </summary>
    /// <param name="fw">方位</param>
    /// <param name="playCardsList">出牌集合</param>
    public void playCards(int fw, List<Transform> playCardsList)
    {
        HideQiPai();
        for (int i = 0; i < playCardsList.Count; i++)
        {
            ObjectPool.Instance.Unspawn(playCardsList[i].gameObject);
            SouthOperationArea.Instance.DelSouthCard(playCardsList[i]);
            GameObject go = ObjectPool.Instance.Spawn("QiPaiTemplate");
            int hs = playCardsList[i].GetComponent<Cards>().PaiHS;
            int id = playCardsList[i].GetComponent<Cards>().PaiID;
            string name = playCardsList[i].GetComponent<Cards>().Name;
            go.GetComponent<Cards>().SetCardsInfo(hs, id, name, OwnHsList[hs + id]); ;
            switch (fw)
            {
                case 1:
                    go.GetComponent<Cards>().SetParent(SouthQiPaiArea);
                    break;
                case 2:
                    go.GetComponent<Cards>().SetParent(EastQiPaiArea);
                    break;
                case 3:
                    go.GetComponent<Cards>().SetParent(WestQiPaiArea);
                    break;
            }
            Desktop_UI.Instance.SetNoAdNum(hs);
        }
        SetCardsCount(fw, -playCardsList.Count);
    }
    /// <summary>
    /// 创建手牌
    /// </summary>
    /// <param name="fw">方位</param>
    /// <param name="CardsCount">手牌数</param>
    /// <param name="cardsHS">手牌花色</param>
    public void CreateCards(List<ddzPInfo> pInfo)
    {
        StartCoroutine(Deal(pInfo));
    }
    IEnumerator Deal(List<ddzPInfo> pInfo)
    {
        foreach (ddzPInfo info in pInfo)
        {
            //创建方位1
            GameObject go_South = ObjectPool.Instance.Spawn("OwnCardsTemplate");
            go_South.GetComponent<Cards>().SetCardsInfo(info.PaiHS, info.PaiID, info.Name, OwnHsList[info.PaiHS + info.PaiID]);
            go_South.GetComponent<Cards>().SetParent(SouthHand);
            SouthOperationArea.Instance.AddSouthCard(go_South.transform);
            //创建方位2
            GameObject go_East = ObjectPool.Instance.Spawn("OppositeCardsTemplate");
            go_East.GetComponent<Cards>().SetParent(EastHand);
            eastCardList.Add(go_East.transform);
            //创建方位3
            GameObject go_West = ObjectPool.Instance.Spawn("OppositeCardsTemplate");
            go_West.GetComponent<Cards>().SetParent(WestHand);
            westCardList.Add(go_West.transform);

            yield return new WaitForSeconds(0.1f);
        }
        SetCardsCount(2, pInfo.Count);
        SetCardsCount(3, pInfo.Count);
    }
    /// <summary>
    /// 设置手牌数
    /// </summary>
    /// <param name="fw">方位</param>
    /// <param name="cardsCount">手牌数</param>
    public void SetCardsCount(int fw ,int cardsCount)
    {
        int count = 0;
        switch (fw)
        {
            case 2:
                CardsCount_East.transform.SetSiblingIndex(CardsCount_East.transform.parent.childCount - 1);
                int.TryParse(CardsCount_East.text,out count);
                CardsCount_East.text = (count + cardsCount).ToString();
                break;
            case 3:
                CardsCount_West.transform.SetSiblingIndex(CardsCount_West.transform.parent.childCount - 1);
                int.TryParse(CardsCount_West.text, out count);
                CardsCount_West.text = (count + cardsCount).ToString();
                break;
        }
    }
    /// <summary>
    /// 回收手牌
    /// </summary>
    public void ClearCards()
    {
        foreach (var item in SouthOperationArea.Instance.southCardList)
        {
            ObjectPool.Instance.Unspawn(item.gameObject);
            SouthOperationArea.Instance.DelSouthCard(item);
        }
        foreach (var item in westCardList)
        {
            ObjectPool.Instance.Unspawn(item.gameObject);
        }
        westCardList.Clear();
        foreach (var item in eastCardList)
        {
            ObjectPool.Instance.Unspawn(item.gameObject);
        }
        eastCardList.Clear();
    }
    /// <summary>
    /// 隐藏弃牌
    /// </summary>
    /// <param name="fw">方位</param>
    private void HideQiPai()
    {
        foreach (Transform item in SouthQiPaiArea)
        {
            if (item.gameObject.activeSelf) ObjectPool.Instance.Unspawn(item.gameObject);
        }
        foreach (Transform item in EastQiPaiArea)
        {
            if (item.gameObject.activeSelf) ObjectPool.Instance.Unspawn(item.gameObject);
        }
        foreach (Transform item in WestQiPaiArea)
        {
            if (item.gameObject.activeSelf) ObjectPool.Instance.Unspawn(item.gameObject);
        }
    }
}
