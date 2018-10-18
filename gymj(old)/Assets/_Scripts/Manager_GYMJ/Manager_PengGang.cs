using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using MJBLL.common;
using System.Linq;
using System.Collections.Generic;
using DNL;
using System.Text;
using UnityEngine.UI;

public class Manager_PengGang : MonoBehaviour
{
    //public GameObject mj_Peng_E;
    //public GameObject mj_Peng_W;
    //public GameObject mj_Peng_N;
    private FICMaskPai maskpai;
    public string gangType;
    private FICStartGame startGame;
    private FICMyCards myCards;
    private Transform gangContentTrans;
    private GameObject buttonGang1;
    private GameObject buttonGang2;
    private GameObject buttonGang3;

    void Start()
    {
        var trans = GameObject.Find("ShouPaiContainer").transform.position;
        maskpai = gameObject.GetComponent<FICMaskPai>();
        startGame = transform.Find("/Main Camera").GetComponent<FICStartGame>();
        myCards = GetComponent<FICMyCards>();
		//扛的按钮的父物体
        gangContentTrans = transform.Find("/Game_UI/Interaction_UI/can_buttons_pghg/grid_gang");
		//扛的按钮
        buttonGang1 = gangContentTrans.Find("item_button_gang1").gameObject;
        buttonGang2 = gangContentTrans.Find("item_button_gang2").gameObject;
        buttonGang3 = gangContentTrans.Find("item_button_gang3").gameObject;

    }

    /// <summary>
    /// 碰牌
    /// </summary>
    public void Peng_S()
    {
		//碰按钮
        GameObject.Find("Main Camera").GetComponent<FICStartGame>().pengButtonGO.SetActive(false);
		//过按钮
        GameObject.Find("Main Camera").GetComponent<FICStartGame>().guoButtonGO.SetActive(false);
		//杠按钮
        GameObject.Find("Main Camera").GetComponent<FICStartGame>().gangButtonGO.SetActive(false);
		//胡按钮
        GameObject.Find("Main Camera").GetComponent<FICStartGame>().huButtonGO.SetActive(false);
        GameObject qipai = GameObject.Find("Main Camera");

        //qipai.GetComponent<FICpaipaipai>().SPengGangPai(GameInfo.pengInfo.Mj.PaiHS, PPTYPE.Peng);
		//碰牌区生成3张碰杠牌，并加入碰杠牌集合,并消除手牌中相应的牌
        myCards.PengPaiEntir(GameInfo.pengInfo.mj.PaiHS);
		//个人手牌，弃牌，碰牌的父物体
        GameObject game = myCards.shouPaiContainerTrans.gameObject;

		//弃牌区的最后一张牌删除
        qipai.GetComponent<FICpaipaipai>().DelQIpai(GameInfo.GetFW(GameInfo.pengInfo.fw));
		// 当缺一门的牌确定后，将非缺门牌遮罩，只显示缺门牌，并排序到最右侧1-39筒。41-79条，81-119万，最开始调用的一次，之后的用其他方法
        gameObject.GetComponent<FICMaskPai>().OnQueYiMenButtonClick(GameInfo.queType);
        //var sp = SendPeng.CreateBuilder().SetTypes(1)
        //      .SetState(1)
        //      .SetOpenid(GameInfo.OpenID)
        //      .SetRoomid(GameInfo.room_id)
        //      .SetFW(GameInfo.pengInfo.Fw)
        //      .SetMj(GameInfo.pengInfo.Mj).Build();
        //byte[] body = sp.ToByteArray();

        var sp = new SendPeng();
        sp.types = 1;
        sp.state = 1;
        sp.openid = GameInfo.OpenID;
        sp.roomid = GameInfo.room_id;
        sp.FW = GameInfo.pengInfo.fw;
        sp.mj = GameInfo.pengInfo.mj;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sp);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 4001, body.Length, 0, body);
		//将碰的信息上传
        GameInfo.cs.Send(data);
        GameInfo.pengInfo = null;
        GameInfo.huPaiInfo = null;
        GameInfo.returnHByType = null;
		// 根据传过来的方位和方式显示，碰杠特效
        startGame.ShowDeskSkillsOnDirection(FW.South, PPTYPE.Peng);

    }



    //S杠牌
    public void Gang_S(int ganghs = 0)
    {
       
        GameObject qipai = GameObject.Find("Main Camera");
		//？？？
        GangMSG gangInfo = null;


        if (GameInfo.cunGang == null)
            return;
	    //用来存杠的合集里，如果杠的信息等于1
        if (GameInfo.cunGang.gang.Count == 1)
        {
            gangInfo = GameInfo.cunGang.gang[0];
			//根据服务器返回的Type(int)，决定杠的类型gangtype(string)
            gangType = qipai.GetComponent<FICpaipaipai>().GetGangType(gangInfo.Type);
        }
        else if (GameInfo.cunGang.gang.Count > 1 && ganghs == 0)
        {
            //当多个杠牌存在时根据花色显示牌让用户选择。当点击后再调用此方法并传递选择的花色
            //这里传递所有的杠牌集合，当选择具体牌面之后，再调用此杠牌方法，将花色传递
            ShowAllCardsCanGang(GameInfo.cunGang.gang);
            return;
        }
        else if (ganghs != 0)
        {

            gangInfo = GameInfo.cunGang.gang.First(w => w.mj.PaiHS == ganghs);
            if (gangInfo == null)
                return;//如果用户选择的花色不存在需要执行的操作
            //隐藏选择花色的牌
            gangType = qipai.GetComponent<FICpaipaipai>().GetGangType(gangInfo.Type);

        }
        else
            return;
        // qipai.GetComponent<FICpaipaipai>().SPengGangPai(gangInfo.Mj.PaiHS, PPTYPE.Gang, true, gangType,GameInfo.GetFW(gangInfo.Fw));

        //myCards.GangREConnect(gangInfo.Mj.PaiHS, gangType, GameInfo.GetFW(gangInfo.Fw));
        //GameObject game = myCards.shouPaiContainerTrans.gameObject;
        //int peng = 0;
        switch (gangType)
        {
            case "M":
			// 杠牌之后，将手牌中所有的相关牌全部销毁，并从集合中移除
                myCards.DestroyGangShouPais(gangInfo.mj.PaiHS);

                break;
            case "H":
            case "A":
			// 杠牌之后，将手牌中所有的相关牌全部销毁，并从集合中移除
                myCards.DestroyGangShouPais(gangInfo.mj.PaiHS);

                break;
            case "Z":
			// 杠牌之后，将手牌中所有的相关牌全部销毁，并从集合中移除
                myCards.DestroyGangShouPais(gangInfo.mj.PaiHS);

                break;
        }
        myCards.GangPai(gangInfo, true);
        if (gangType.Equals(GangType.mingGang))
        {
            qipai.GetComponent<FICpaipaipai>().DelQIpai(GameInfo.GetFW(gangInfo.fw));
          
        }
        GameObject.Find("Main Camera").GetComponent<FICStartGame>().pengButtonGO.SetActive(false);
        GameObject.Find("Main Camera").GetComponent<FICStartGame>().gangButtonGO.SetActive(false);
        GameObject.Find("Main Camera").GetComponent<FICStartGame>().guoButtonGO.SetActive(false);
        GameObject.Find("Main Camera").GetComponent<FICStartGame>().huButtonGO.SetActive(false);
        //var sp = SendPeng.CreateBuilder().SetTypes(2)
        //    .SetState(1)
        //    .SetOpenid(GameInfo.OpenID)
        //    .SetRoomid(GameInfo.room_id)
        //    .SetFW(gangInfo.Fw)
        //    .SetMj(gangInfo.Mj)
        //    .SetGtype(gangType)
        //    .Build();
        //byte[] body = sp.ToByteArray();

        var sp = new SendPeng();
        sp.types = 2;
        sp.state = 1;
        sp.openid = GameInfo.OpenID;
        sp.roomid = GameInfo.room_id;
        sp.FW = gangInfo.fw;
        sp.mj = gangInfo.mj;
        sp.Gtype = gangType;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sp);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 4001, body.Length, 0, body);
        GameInfo.cs.Send(data);
        GameInfo.IsGang = true;
        GameInfo.pengInfo = null;
        GameInfo.cunGang = null;
        GameInfo.huPaiInfo = null;
        GameInfo.returnHByType = null;
        GameInfo.nowFW = GameInfo.nowFW == GameInfo.room_peo ? 1 : GameInfo.nowFW + 1;
        maskpai.OnQueYiMenButtonClick(GameInfo.queType);

        startGame.ShowDeskSkillsOnDirection(FW.South, PPTYPE.Gang);
    }

    /// <summary>
    /// 点击杠牌，将所有手牌全部遮罩（标志位设置为false）在list中的牌显示，点击未被遮罩的牌之后，直接杠牌
    /// </summary>
    void ShowAllCardsCanGang(IList<GangMSG> gangList)
    {
        foreach (var item in myCards.shouPaiGOList)
        {
            item.GetComponent<MeshRenderer>().material.color = startGame.darkColor;
            item.GetComponent<MJ_Event>().isCanGang = false;
        }

        for (int i = 0; i < gangList.Count; i++)
        {

            switch (i)
            {
                case 0:
                    ShowMJSCanGang(buttonGang1, gangList[0].mj.PaiHS);
                    break;
                case 1:
                    ShowMJSCanGang(buttonGang2, gangList[1].mj.PaiHS);
                    break;
                case 2:
                    ShowMJSCanGang(buttonGang3, gangList[2].mj.PaiHS);
                    break;

            }
            //for (int j = 0; j < myCards.shouPaiGOList.Count; j++)
            //{
            //    if (myCards.shouPaiGOList[j].GetComponent<MJ_SP>().HS == gangList[i].mj.PaiHS)
            //    {
            //        myCards.shouPaiGOList[j].GetComponent<MeshRenderer>().material.color = startGame.resetColor;
            //        myCards.shouPaiGOList[j].GetComponent<MJ_Event>().isCanGang = true;
            //    }
            //}

        }

    }

    private void ShowMJSCanGang( GameObject buttonGO,  int hs)
    {
        buttonGO.GetComponent<FICChooseGang>().ShowPaiHsImage(this,startGame, hs);
        buttonGO.SetActive(true);
    }
    public void HideMJSCanGang()
    {

        for (int i = 0; i < gangContentTrans.childCount; i++)
        {
            gangContentTrans.GetChild(i).gameObject.SetActive(false);
        }
    }
    //W杠
    public void Gang_W(ReturnPeng rp)
    {
        GameObject qipai = GameObject.Find("Main Camera");
        qipai.GetComponent<Manager_Qipai>().mj_GangW(rp.mj.PaiHS);

        if ((GameInfo.FW + 1 > 4 ? 1 : GameInfo.FW + 1) == rp.fw)
        {
            //右边用户
            var qipaiQU = GameObject.Find("Qipai_E").transform;//找到对应座位的弃牌区
            for (int i = 0; i < qipaiQU.childCount; i++)
            {
                if (qipaiQU.GetChild(i).GetComponent<MJ_SP>().ID == rp.mj.PaiID)
                {
                    Destroy(qipaiQU.GetChild(i).gameObject);
                }
            }
        }
        if ((GameInfo.FW == 1 ? 4 : GameInfo.FW - 1) == rp.fw)
        {
            //左边用户
            var qipaiQU = GameObject.Find("Qipai_W").transform;//找到对应座位的弃牌区
            for (int i = 0; i < qipaiQU.childCount; i++)
            {
                if (qipaiQU.GetChild(i).GetComponent<MJ_SP>().ID == rp.mj.PaiID)
                {
                    Destroy(qipaiQU.GetChild(i).gameObject);
                }
            }
        }
        if (((GameInfo.FW + 2) > 4 ? (GameInfo.FW + 2) - 4 : (GameInfo.FW + 2)) == rp.fw)
        {
            //上方用户
            var qipaiQU = GameObject.Find("Qipai_N").transform;//找到对应座位的弃牌区
            for (int i = 0; i < qipaiQU.childCount; i++)
            {
                if (qipaiQU.GetChild(i).GetComponent<MJ_SP>().ID == rp.mj.PaiID)
                {
                    Destroy(qipaiQU.GetChild(i).gameObject);
                }
            }
        }
    }

    //E杠
    public void Gang_E(ReturnPeng rp)
    {
        GameObject qipai = GameObject.Find("Main Camera");
        qipai.GetComponent<Manager_Qipai>().mj_GangE(rp.mj.PaiHS);
        if ((GameInfo.FW + 1 > 4 ? 1 : GameInfo.FW + 1) == rp.fw)
        {
            //右边用户
            var qipaiQU = GameObject.Find("Qipai_E").transform;//找到对应座位的弃牌区
            for (int i = 0; i < qipaiQU.childCount; i++)
            {
                if (qipaiQU.GetChild(i).GetComponent<MJ_SP>().ID == rp.mj.PaiID)
                {
                    Destroy(qipaiQU.GetChild(i).gameObject);
                }
            }
        }
        if ((GameInfo.FW == 1 ? 4 : GameInfo.FW - 1) == rp.fw)
        {
            //左边用户
            var qipaiQU = GameObject.Find("Qipai_W").transform;//找到对应座位的弃牌区
            for (int i = 0; i < qipaiQU.childCount; i++)
            {
                if (qipaiQU.GetChild(i).GetComponent<MJ_SP>().ID == rp.mj.PaiID)
                {
                    Destroy(qipaiQU.GetChild(i).gameObject);
                }
            }
        }
        if (((GameInfo.FW + 2) > 4 ? (GameInfo.FW + 2) - 4 : (GameInfo.FW + 2)) == rp.fw)
        {
            //上方用户
            var qipaiQU = GameObject.Find("Qipai_N").transform;//找到对应座位的弃牌区
            for (int i = 0; i < qipaiQU.childCount; i++)
            {
                if (qipaiQU.GetChild(i).GetComponent<MJ_SP>().ID == rp.mj.PaiID)
                {
                    Destroy(qipaiQU.GetChild(i).gameObject);
                }
            }
        }
    }

    //N杠
    public void Gang_N(ReturnPeng rp)
    {
        GameObject qipai = GameObject.Find("Main Camera");
        qipai.GetComponent<Manager_Qipai>().mj_GangN(rp.mj.PaiHS);

        if ((GameInfo.FW + 1 > 4 ? 1 : GameInfo.FW + 1) == rp.fw)
        {
            //右边用户
            var qipaiQU = GameObject.Find("Qipai_E").transform;//找到对应座位的弃牌区
            for (int i = 0; i < qipaiQU.childCount; i++)
            {
                if (qipaiQU.GetChild(i).GetComponent<MJ_SP>().ID == rp.mj.PaiID)
                {
                    Destroy(qipaiQU.GetChild(i).gameObject);
                }
            }
        }
        if ((GameInfo.FW == 1 ? 4 : GameInfo.FW - 1) == rp.fw)
        {
            //左边用户
            var qipaiQU = GameObject.Find("Qipai_W").transform;//找到对应座位的弃牌区
            for (int i = 0; i < qipaiQU.childCount; i++)
            {
                if (qipaiQU.GetChild(i).GetComponent<MJ_SP>().ID == rp.mj.PaiID)
                {
                    Destroy(qipaiQU.GetChild(i).gameObject);
                }
            }
        }
        if (((GameInfo.FW + 2) > 4 ? (GameInfo.FW + 2) - 4 : (GameInfo.FW + 2)) == rp.fw)
        {
            //上方用户
            var qipaiQU = GameObject.Find("Qipai_N").transform;//找到对应座位的弃牌区
            for (int i = 0; i < qipaiQU.childCount; i++)
            {
                if (qipaiQU.GetChild(i).GetComponent<MJ_SP>().ID == rp.mj.PaiID)
                {
                    Destroy(qipaiQU.GetChild(i).gameObject);
                }
            }
        }
    }
}
