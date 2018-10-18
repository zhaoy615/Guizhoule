using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using cn.sharesdk.unity3d;
using MJBLL.common;
using UnityEngine.UI;
using DNL;
using DG.Tweening;
using System.Text;

public class FICCreatRoom : MonoBehaviour
{
    //虚拟的创建房间
    string openid = GameInfo.OpenID;
    int is_wgj=0;
    int is_xingqiji=0;
    int is_shangxiaji=0;
    int is_benji=0;
    int is_yikousan=0;
    int room_peo= 2;
    int count =8;
    int is_yuanque = 0;
    int is_smcp = 0;
    Button createRoomButton;
    GameObject CreateRoom_pel;
    //  bool isClosed = false;
    FICStartGame startGame;
    private void Start()
    {
        //startGame = transform.Find("/Main Camera").GetComponent<FICStartGame>();
        createRoomButton = transform.Find("/Game_UI/Two_UI/CreateRoom_pel/Game_Type/Create_Btn").gameObject.GetComponent<Button>();
        CreateRoom_pel = transform.Find("/Game_UI/Two_UI/CreateRoom_pel").gameObject;
        if (PlayerPrefs.GetString("roominfo") != null)
        {
            try
            {
                GameInfo.roomInfo = JsonUtility.FromJson<RoomInfo>(PlayerPrefs.GetString("roominfo"));
            }
            catch
            {
            }
           
        }
        InitRoomInfo();
    }

    #region toogleGroup 创建房间测试
    public void SetFourPeo(bool isFour)
    {
        if (isFour) { room_peo = 4; }
    }
    public void SetYuanQue(bool isYuanQue)
    {
        if (isYuanQue) { is_yuanque = 1; } else { is_yuanque = 0; }
    }
    public void SetThreePeo(bool isThree)
    {
        if (isThree) { room_peo = 3; }
    }
    public void SetTwoPeo(bool isTwo)
    {
        if (isTwo) { room_peo = 2; }
    }
    public void SetIsWGJ(bool iswgj)
    {
        if (iswgj) { is_wgj = 1; } else { is_wgj = 0; GameInfo.cfwgj = true;}//不再会有冲锋乌骨鸡
        }
    public void SetIsXQJ(bool isxqj)
    {
        if (isxqj) { is_xingqiji = 1; } else { is_xingqiji = 0; }
    }
    public void SetIsSXJ(bool issxj)
    {
        if (issxj) { is_shangxiaji = 1; } else { is_shangxiaji = 0; }
    }
    public void SetIsSMCP(bool issmcp)
    {
        if (issmcp) { is_smcp = 1; } else { is_smcp = 0; }
    }
    public void SetIsBJ(bool isbj)
    {
        if (isbj) { is_benji = 1; } else { is_benji = 0; }
    }
    public void SetIsYKS(bool isyks)
    {
        if (isyks) { is_yikousan = 1; } else { is_yikousan = 0; }
    }
    public void SetIsLian(bool isLian)
    {
        if (isLian)
        {
            is_yikousan = 3;
        }
    }
   
    public void SetEightCount(bool isEight)
    {
        if (isEight) { count = 8;
        }
    }
    public void SetSixteenCount(bool isSixteen)
    {
        if (isSixteen) { count = 12; }
    }
    public void SetThirtythreeCount(bool isThirtythree)
    {
        if (isThirtythree) { count = 16; }
    }
    #endregion
    
    /// <summary>
    /// 发送创建房间信息
    /// </summary>
    public void OnCreatRoomClick()
    {
        if (GameInfo.isScoketClose)
            GameInfo.cs.Closed();
        GameInfo.cs.serverType = ServerType.ListServer;
        GameInfo.room_peo = room_peo;
        GameInfo.gameCount = count;
        GameInfo.operation = 1;
        //SendGameOperation sendGameOperation = SendGameOperation.CreateBuilder()
        //    .SetOpenid(GameInfo.OpenID)
        //    .SetUnionid(GameInfo.unionid)
        //    .SetOperation(GameInfo.operation)
        //    .SetRoomID(GameInfo.room_id.ToString())
        //    .Build();
        //byte[] body = sendGameOperation.ToByteArray();
        SendGameOperation sendGameOperation = new SendGameOperation();
        sendGameOperation.openid = GameInfo.OpenID;
        sendGameOperation.unionid = GameInfo.unionid;
        sendGameOperation.Operation = GameInfo.operation;
        sendGameOperation.RoomID = GameInfo.room_id.ToString();
        sendGameOperation.GroupID = (int)GameInfo.GroupID;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGameOperation);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1020, body.Length, 0, body);
        GameInfo.cs.Send(data);
        GameInfo.isScoketClose = true;
        createRoomButton.enabled = false;

    }
    /// <summary>
    /// 保存创建房间信息，服务器不给自己发，所以多开一个字段存，多一个方法显示
    /// </summary>
    void SaveRoomRule()
    {
        GameInfo.roomRlue = null;
        Debug.Log(is_wgj + ","+ is_xingqiji + "," + is_shangxiaji + "," + is_benji + "," + is_yikousan + "," + room_peo + "," + count + "," + is_smcp);   
        string roomRule = "";
        if (is_smcp == 1) { roomRule += "  十秒出牌"; GameInfo.roomInfo.is_smcp = 1; }
        else { GameInfo.roomInfo.is_smcp = 0; }
        if (is_yuanque == 1) { roomRule += "  原缺"; GameInfo.roomInfo.is_yuanque = 1; }
        else { GameInfo.roomInfo.is_yuanque = 0; }
        if (is_benji == 1) { roomRule += "  本鸡"; GameInfo.roomInfo.is_benji = 1; }
        else { GameInfo.roomInfo.is_benji = 0; }
        if (is_shangxiaji == 1) { roomRule += "  摇摆鸡"; GameInfo.roomInfo.is_shangxiaji = 1; }
        else { GameInfo.roomInfo.is_shangxiaji = 0; }
        if (is_xingqiji == 1) { roomRule += "  星期鸡"; GameInfo.roomInfo.is_xingqiji = 1; }
        else { GameInfo.roomInfo.is_xingqiji = 0; }
        if (is_wgj == 1) { roomRule += "  乌骨鸡"; GameInfo.roomInfo.is_wgj = 1; }
        else { GameInfo.roomInfo.is_wgj = 0; }
       
        
        switch (is_yikousan)
            {
                case 0:
                    roomRule += "  1扣2";
                GameInfo.roomInfo.is_yikousan = 0;
                    break;
                case 1:
                    roomRule += "  通三";
                GameInfo.roomInfo.is_yikousan = 1;
                    break;
                case 3:
                    roomRule += "  连庄";
                GameInfo.roomInfo.is_yikousan = 3;
                    break;


            }

        GameInfo.roomInfo.juSu = count;
        GameInfo.roomInfo.roomPeo = room_peo;
        GameInfo.roomRlue = roomRule;
        PlayerPrefs.SetString("roominfo", JsonUtility.ToJson(GameInfo.roomInfo));
    }



    public void Update()
    {
            //返回加入服务器成功，创建房间
            if (GameInfo.operation == 1 && GameInfo.addStatus == 1)
            {
            GameInfo.cs.serverType = ServerType.GameServer;
            GameInfo.operation = 0;
            GameInfo.addStatus = 0;
            Debug.Log(GameInfo.OpenID);
            GameInfo.cs.SendCreatRoomMessage(openid
                , is_wgj
                , is_xingqiji
                , is_shangxiaji
                , is_benji
                , is_yikousan
                , room_peo
                , count
                ,GameInfo.Latitude
                ,is_yuanque
                , is_smcp
                ,(int)GameInfo.GroupID);
                //创建房间时，清空字典，以免加入别的房间，数据不对
                GameInfo.MJplayers.Clear();
                SaveRoomRule();
            }
        if (GameInfo.returnCreatRoom != null)
        {
            GameObject.Find("Main Camera").GetComponent<Manager_Hall>().isClosed = true;
            GameInfo.room_id = GameInfo.returnCreatRoom.roomID;

            GameInfo.OpenID = GameInfo.returnCreatRoom.userinfo.openid;
            GameInfo.NickName = GameInfo.returnCreatRoom.userinfo.nickname;
            GameInfo.HeadImg = GameInfo.returnCreatRoom.userinfo.headimg;
            GameInfo.Sex = GameInfo.returnCreatRoom.userinfo.sex;
            GameInfo.Bean = GameInfo.returnCreatRoom.userinfo.user_bean;
            GameInfo.Gold = GameInfo.returnCreatRoom.userinfo.user_gold;
            GameInfo.FW = GameInfo.returnCreatRoom.userinfo.user_FW;
            GameInfo.userIP = GameInfo.returnCreatRoom.userinfo.UserIP;

            GameInfo.MJplayers.Add(GameInfo.FW, GameInfo.returnCreatRoom.userinfo);

            GameInfo.MJplayersWhoQuit[GameInfo.OpenID] = GameInfo.FW;
            GameInfo.returnCreatRoom = null;
            GameInfo.IsSetRoomInfo = true;
            GameInfo.sceneID = "Game_GYMJ";
            GameInfo.isScoketClose = false;
            SceneManager.LoadScene("LoadingHall");
            //SceneManager.LoadScene("Scene_Game");
        }
    }
    public void UIMove(bool state)
    {
        CreateRoom_pel.transform.Find("peopleNum").GetChild(5).gameObject.SetActive(!state);
    }
    public void InitRoomInfo()
    {
        CreateRoom_pel.transform.Find("inningNum").GetChild(GameInfo.roomInfo.juSu == 16 ? 3 : 2).GetComponent<Toggle>().isOn = true;

        CreateRoom_pel.transform.Find("peopleNum").GetChild(GameInfo.roomInfo.roomPeo == 3 ? 3 : GameInfo.roomInfo.roomPeo == 4 ? 4 : 2).GetComponent<Toggle>().isOn = true;
        CreateRoom_pel.transform.Find("peopleNum").GetChild(5).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_yuanque == 1 ? true : false;
        CreateRoom_pel.transform.Find("peopleNum").GetChild(6).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_smcp == 1 ? true : false;

        CreateRoom_pel.transform.Find("JI_Type").GetChild(GameInfo.roomInfo.is_shangxiaji == 1 ? 3 : 6 ).GetComponent<Toggle>().isOn = true;
        CreateRoom_pel.transform.Find("JI_Type").GetChild(2).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_benji == 1 ? true : false;
        CreateRoom_pel.transform.Find("JI_Type").GetChild(4).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_xingqiji == 1 ? true : false;
        CreateRoom_pel.transform.Find("JI_Type").GetChild(5).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_wgj == 1 ? true : false;
        
        

        CreateRoom_pel.transform.Find("playType").GetChild(GameInfo.roomInfo.is_yikousan == 3 ? 2 : GameInfo.roomInfo.is_yikousan == 1? 4 : 3).GetComponent<Toggle>().isOn = true;

    }
    //private void OnDestroy()
    //{
    //    if (!isClosed)
    //        GameInfo.cs.Closed();

    //}
}
