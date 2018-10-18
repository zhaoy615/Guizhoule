using UnityEngine;
using System.Collections;
using DDZ;

public class MainLogic : Singleton<MainLogic>
{
    public enum GameStatus
    {
        prepare,//准备
        begin,//开始
        play,//玩
        over//结束
    }
    private GameStatus gameStatus = GameStatus.prepare;
	void Start () {
	
	}
	
	void Update () {
        GameLogicFSM();
    }
    void GameLogicFSM()
    {
        switch (gameStatus)
        {
            case GameStatus.prepare:
                //准备阶段，加载开始游戏需要的资源
                if (DDZData.returnUserInfo != null) InitGame.Instance.OnReturnUserInfo();
                if (DDZData.returnStart != null) gameStatus = GameStatus.begin;
                DDZSendMessage.Instance.SendStart();
                break;
            case GameStatus.begin:
                if (DDZData.returnCallLandlord != null) OnReturnCallLandlord();
                if (DDZData.returnStartGame != null) OnReturnStartGame();
                //开始游戏，发牌，玩家开始抢地主
                //每家玩家发牌17张
                //庄家开始  是否叫地主 或出多少分 可不叫
                //其余玩家轮流抢地主 或出分 可不抢    叫地主次数为4次   出分模式分数最高者为地主
                break;
            case GameStatus.play:
                if (DDZData.returnOutCard != null);
                if (DDZData.returnOutCardAll != null);
                //开始打牌逻辑，直到地主或农民胜出
                //每个玩家可以有出牌或不出两种操作
                break;
            case GameStatus.over:
                //显示结算界面，准备开始下一局或返回大厅
                break;
        }
    }
    public void OnReturnStartGame()
    {
        PopUp_UI.Instance.CreateCards(DDZData.returnStartGame.pai);
        Desktop_UI.Instance.SetPaiJuInfo(4,DDZData.returnStartGame.gamenumber.ToString());
        DDZData.returnStartGame = null;
    }
    /// <summary>
    /// 叫地主
    /// </summary>
    /// <param name="fw">方位</param>
    public void OnReturnCallLandlord()
    {
        switch (DDZData.returnCallLandlord.CallType)
        {
            case 1:
                //显示玩家叫地主
                break;
            case 2:
                //显示玩家叫分
                break;
        }
        switch (DDZData.returnCallLandlord.IsOver)
        {
            case 1:
                Desktop_UI.Instance.SetLandlordIcon(DDZData.returnStartGame.chuuser);
                gameStatus = GameStatus.play;
                break;
            case 0:
                break;
            case -1:
                LiuJu();
                break;
        }
        int oneCard = DDZData.returnCallLandlord.dipai[0].PaiHS + DDZData.returnCallLandlord.dipai[0].PaiID;
        int twoCard = DDZData.returnCallLandlord.dipai[1].PaiHS + DDZData.returnCallLandlord.dipai[1].PaiID;
        int threeCard = DDZData.returnCallLandlord.dipai[2].PaiHS + DDZData.returnCallLandlord.dipai[2].PaiID;
        Desktop_UI.Instance.SetBottomCards(oneCard, twoCard, threeCard);
        DDZData.returnCallLandlord = null;
    }

    public void LiuJu()
    {
        //需要清除每个玩家手牌
        PopUp_UI.Instance.ClearCards();
    }
    /// <summary>
    /// 不叫
    /// </summary>
    public void NotCry()
    {
        Debug.Log("不叫");
    }
    /// <summary>
    /// 加倍
    /// </summary>
    public void AddMultiple()
    {
        DDZData.multiple *= 2;
        Debug.Log("加倍");
    }
    /// <summary>
    /// 不加倍
    /// </summary>
    public void NotAddMultiple()
    {
        Debug.Log("不加倍");
    }
    /// <summary>
    /// 不出
    /// </summary>
    public void Pass()
    {
        Debug.Log("不出");
    }
    /// <summary>
    /// 提示可出牌
    /// </summary>
    public void HintPlayCards()
    {
        Debug.Log("提示可出牌");
    }
 
}
