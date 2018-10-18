using UnityEngine;
using UnityEngine.SceneManagement;
using MJBLL.common;
using UnityEngine.UI;
using DNL;
using DDZ;

public class FICCreatRoom : MonoBehaviour
{
    public enum GameType
    {
        GYMJ = 1,
        DDZ
    }
    //虚拟的创建房间
    string openid = GameInfo.OpenID;
    public GameType gameType = GameType.GYMJ;
    //===================================贵阳麻将===============================================
    int is_wgj = 0;
    int is_xingqiji = 0;
    int is_shangxiaji = 0;
    int is_benji = 0;
    int is_yikousan = 0;
    int room_peo = 2;
    int count = 8;
    int is_yuanque = 0;
    int is_smcp = 0;
    //===================================斗地主=================================================
    public Toggle cryLandlord;
    public Toggle cryScore;
    public Toggle inningNum_8;
    public Toggle inningNum_16;
    public Toggle multiple_16;
    public Toggle multiple_32;
    public Toggle multiple_64;
    //==========================================================================================
    Button createRoomButton;
    GameObject CreateRoom_pel;
    FICStartGame startGame;
    private void Start()
    {
        CreateRoom_pel = transform.Find("/Game_UI/Two_UI/CreateRoom_pel").gameObject;
        createRoomButton = transform.Find("/Game_UI/Two_UI/CreateRoom_pel/Game_Type/Create_Btn").gameObject.GetComponent<Button>();
        InitRoomInfo();
        SwitchGameType(1);

    }
    public void InitRoomInfo()
    {
        //==============================================================贵阳麻将=================================================================================================
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("roominfo")))
        {
            GameInfo.roomInfo = JsonUtility.FromJson<GameInfo.RoomInfo>(PlayerPrefs.GetString("roominfo"));
        }
        CreateRoom_pel.transform.Find("GYMJ_Pel/inningNum").GetChild(GameInfo.roomInfo.juSu == 16 ? 3 : 2).GetComponent<Toggle>().isOn = true;
        CreateRoom_pel.transform.Find("GYMJ_Pel/peopleNum").GetChild(GameInfo.roomInfo.roomPeo == 3 ? 3 : GameInfo.roomInfo.roomPeo == 4 ? 4 : 2).GetComponent<Toggle>().isOn = true;
        CreateRoom_pel.transform.Find("GYMJ_Pel/peopleNum").GetChild(5).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_yuanque == 1 ? true : false;
        CreateRoom_pel.transform.Find("GYMJ_Pel/peopleNum").GetChild(6).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_smcp == 1 ? true : false;
        CreateRoom_pel.transform.Find("GYMJ_Pel/JI_Type").GetChild(GameInfo.roomInfo.is_shangxiaji == 1 ? 3 : 6).GetComponent<Toggle>().isOn = true;
        CreateRoom_pel.transform.Find("GYMJ_Pel/JI_Type").GetChild(2).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_benji == 1 ? true : false;
        CreateRoom_pel.transform.Find("GYMJ_Pel/JI_Type").GetChild(4).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_xingqiji == 1 ? true : false;
        CreateRoom_pel.transform.Find("GYMJ_Pel/JI_Type").GetChild(5).GetComponent<Toggle>().isOn = GameInfo.roomInfo.is_wgj == 1 ? true : false;
        CreateRoom_pel.transform.Find("GYMJ_Pel/playType").GetChild(GameInfo.roomInfo.is_yikousan == 3 ? 2 : GameInfo.roomInfo.is_yikousan == 1 ? 4 : 3).GetComponent<Toggle>().isOn = true;
        //==============================================================斗地主=================================================================================================
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("roominfoDDZ")))
        {
            DDZData.roomInfo = JsonUtility.FromJson<DDZData.RoomInfo>(PlayerPrefs.GetString("roominfoDDZ"));
        }
        cryLandlord.isOn = DDZData.roomInfo.playType == 1 ? true : false;
        cryScore.isOn = DDZData.roomInfo.playType == 2 ? true : false;
        inningNum_8.isOn = DDZData.roomInfo.inningNum == 8 ? true : false;
        inningNum_16.isOn = DDZData.roomInfo.inningNum == 16 ? true : false;
        multiple_16.isOn = DDZData.roomInfo.multiple == 16 ? true : false;
        multiple_32.isOn = DDZData.roomInfo.multiple == 32 ? true : false;
        multiple_64.isOn = DDZData.roomInfo.multiple == 64 ? true : false;
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
        if (iswgj) { is_wgj = 1; } else { is_wgj = 0; GameInfo.cfwgj = true; }//不再会有冲锋乌骨鸡
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
        if (isEight)
        {
            count = 8;
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

    #region toogleGroup 创建斗地主房间测试
    public void SetInningNum_DDZ(int num)
    {
        DDZData.roomInfo.inningNum = num;
    }
    public void SetPlayType_DDZ(int type)
    {
        DDZData.roomInfo.playType = type;
    }
    public void SetMultiple_DDZ(int mul)
    {
        DDZData.roomInfo.multiple = mul;
    }

    #endregion

    public void SwitchGameType(int type)
    {
        gameType = (GameType)type;
        Debug.Log(gameType);
    }
    /// <summary>
    /// 发送创建房间信息
    /// </summary>
    public void OnCreatRoomClick()
    {
        if (GameInfo.isScoketClose)
            GameInfo.cs.Closed();
        GameInfo.cs.serverType = ServerType.ListServer;
        GameInfo.operation = 1;
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
    public void Update()
    {
        //返回加入服务器成功，创建房间
        if (GameInfo.operation == 1 && GameInfo.addStatus == 1)
        {
            GameInfo.cs.serverType = ServerType.GameServer;
            GameInfo.operation = 0;
            GameInfo.addStatus = 0;
            switch (gameType)
            {
                case GameType.GYMJ:
                    GameInfo.cs.SendCreatRoomMessage(openid, is_wgj, is_xingqiji, is_shangxiaji, is_benji, is_yikousan, room_peo, count, GameInfo.Latitude, is_yuanque, is_smcp, (int)GameInfo.GroupID);
                    GameInfo.MJplayers.Clear();
                    SaveRoomRule();
                    break;
                case GameType.DDZ:
                    SendDZCreateRoom();
                    break;
            }
        }
        if (GameInfo.returnCreatRoom != null && GameInfo.returnRoomMsg != null)
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

        }
       
        if (DDZData.returnCreateRoom != null)
        {
            DDZData.room_id = DDZData.returnCreateRoom.roomID;
            DDZData.userInfoList.Clear();
            DDZData.userInfoList[GameInfo.userID] = DDZData.returnCreateRoom.userinfo;
            GameInfo.sceneID = "GAME_DDZ";
            GameInfo.isScoketClose = false;
            SceneManager.LoadScene("LoadingHall");
            DDZData.returnCreateRoom = null;
        }
    }
    /// <summary>
    /// 保存创建房间信息，服务器不给自己发，所以多开一个字段存，多一个方法显示
    /// </summary>
    void SaveRoomRule()
    {
        GameInfo.room_peo = room_peo;
        GameInfo.gameCount = count;
        GameInfo.roomRlue = null;
        Debug.Log(is_wgj + "," + is_xingqiji + "," + is_shangxiaji + "," + is_benji + "," + is_yikousan + "," + room_peo + "," + count + "," + is_smcp);
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

    public void SendDZCreateRoom()
    {
        ddzSendDZCreateRoom sendDZCreateRoom = new ddzSendDZCreateRoom();
        sendDZCreateRoom.openid = GameInfo.OpenID;
        sendDZCreateRoom.roomNumber = DDZData.roomInfo.inningNum;
        sendDZCreateRoom.roomtype = DDZData.roomInfo.playType;
        sendDZCreateRoom.multiple = DDZData.roomInfo.multiple;
        sendDZCreateRoom.Latitude = GameInfo.Latitude;
        sendDZCreateRoom.GroupID = (int)GameInfo.GroupID;
        Debug.Log(DDZData.roomInfo.inningNum + "," + DDZData.roomInfo.playType + "," + DDZData.roomInfo.multiple);
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendDZCreateRoom);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUMD + 2001, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }

}
