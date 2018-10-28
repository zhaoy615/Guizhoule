using UnityEngine;
using System.Collections.Generic;
using MJBLL.common;
using System;
using DNL;
public enum SceneID
{
    Init,
    Login,
    Hall,
    MJ
}
public enum LoginType
{
    none,
    longbao,
    fangka
}

public static class GameInfo
{
    public struct RoomInfo
    {
        public int juSu;
        public int roomPeo;
        public int is_wgj;
        public int is_xingqiji;
        public int is_shangxiaji;
        public int is_benji;
        public int is_yikousan;
        public int is_yuanque;
        public int is_smcp;
    }
    public enum QueType
    {
        Tong = 1,
        Tiao = 2,
        Wan = 3,
        no
    }
    public static LoginType loginType;
    public static SceneID sceneWhich = SceneID.Init;
	public static string OpenID;
// 2018.3.16 改为从 Manager_Login.cs 获取 
//	public static string OpenID = SystemInfo.deviceUniqueIdentifier;

    public static string NickName;//昵称
    public static long userID;//用戶ID
    public static string userIP;//用戶IP
    public static long userRoomCard;//房卡數量
    public static string province=" ";
    public static string city=" ";
    public static string unionid;
    public static int Sex;//性别 1男 2女
    public static string certificate;
    public static string HeadImg;//头像
    public static int room_id;//房间号
    public static int FW;//方位
    public static int Gold;//乐币
    public static int Bean;//豆子
    public static bool isPlayback = false;
    public static float speed = 1f;
    public static bool isHideWarn = false;
    public static int cunHtypeType;//用来显示胡还是平胡
    public static int zhuang;//庄
    public static bool isLZhuang = false;
    public static int HYFw = 0;
    public static int nowFW = 0;//当前出牌的方位
    public static int room_peo = 0;//房间类型
    public static int isYuanQue = 0;
    public static bool recon = false;//是否是断线重连
    public static bool cfj = false;//是否已经打出过冲锋鸡
    public static bool cfwgj = false;//是否已经打出过冲锋乌骨鸡
    public static int CardsNumber = 0;//出牌的次数，用来判断报听
    public static bool isTT = false;
    public static bool TT_bl = false;//确认是否天听
    public static bool isTH = false;
    public static bool isHhouTing = false;
    public static bool isRealTing = false;
    public static int gameCount = 1;//创建房间的局数
    public static bool isClosed = false;//是否关闭连接
    public static QueType queType = QueType.no;
    public static bool IsTingPai = false;//是否已经听牌
    public static bool IsDingQue = false;//是否定缺
    public static bool IsDingQueClick = false;//发送天听天胡不要显示定缺，不能和上一个共用，它有别的引用地方
    public static bool IsDingQueGang = false;//在显示选择定缺的同时判断手牌中是否可以杠牌
    public static bool IsGang = false;//是否杠牌
    public static bool IsSetRoomInfo = false; //判断是获取到房间信息
    public static int gameNum = 1;//游戏局数,已经在打第几局
    public static bool fangyan = true;//是否是方言
    public static string roomRlue;
    public static byte[] sendbyte;//防止客户端多次请求
    public static string sceneID =null;//场景名称
    public static string gameName = null;
    public static string Latitude = "0,0";//经纬度
    public static int operation = 0; //1創建房間/2加入房間
    public static string ip = null;   //返回服務器IP
    public static string port = null;//返回服務器端口
    public static long GroupID = 0;  //朋友圈ID
    //public static string listIp = "192.168.2.54";   //登录后的列表服务器IP
    //public static string listIp = "192.168.2.222";   //登录后的列表服务器IP
    public static string listIp = "192.168.1.103";   //内网
    //public static string listIp = "47.106.16.58";//线上服务器
    public static string listPort = "2018";//登录后的列表服务器Port
    public static int status = 0;//状态 1为正常跳转， 2为有未结束的游戏，需要重新连接
    public static int addStatus = 0;//返回加入服务器状态rr
    public static bool isScoketClose = false;
    public static bool isAllreadyStart = false;//是否已经开始游戏了，决定解散面板是解散还是退出
    public static bool isStartGame = false;//游戏是否开始过对局
    public static string jieSanOpenid = null;
    public static int gameVoice= 1; ///快捷语音传出的方位
    public static bool isPYQExploits = false;
    

//-----------------------------静态字典变量-----------------------------------------------------------------------------------------------------------------
    public static Dictionary<int, Userinfo> MJplayers = new Dictionary<int, Userinfo>();
    public static Dictionary<string, int> MJplayersWhoQuit = new Dictionary<string, int>();
    public static Dictionary<string, int> MJVoteDic = new Dictionary<string, int>();
    public static Dictionary<int, int> integrals = new Dictionary<int, int>();
//----------------------------------------------------------------------------------------------------------------------------------------------------------
 
    public static GameOperationProcess gameOperationProcess = null;
    public static CreatSocket cs = new CreatSocket();
    public static IList<AnnouncementInfo> announcementInfo;//公告信息列表
    public static List<int> fwGpsList = new List<int>();
    public static List<GameObject> cloneMjs = new List<GameObject>();

    //-----------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 服务器返回值
    /// </summary>
    public static ReturnLogin returnlogin;//返回登录信息
    public static ReturnCreateRoom returnCreatRoom;//返回创建房间信息
    public static ReturnRoomAdd returnRoomAdd;
    public static ReturnStartGame returnStartGame;//返回主动推送游戏开始信息
    public static ReturnAddRoom returnAddRoom;//返回加入房间信息
    public static ReturnUserInfo returnUserInfo;//返回主动推送加入万家信息
    public static ReturnStart returnStart;//返回开始游戏
    public static ReturnTTATH returnTTOrTH;
    public static ReturnMsg returnMsg;//返回出牌信息
    public static ReturnAll returnAll;//返回 1碰，2杠,3和,4摸，状态和方位
    public static ReturnMP returnMP;//返回摸牌信息
    public static ReturnPeng returnPeng;//返回碰杠胡后消息
    public static ReturnRecon returnRecon;//返回重连信息
    public static ReturnConnData returnConnData;//返回重连信息返回断线前信息
    public static ReturnMsgList returnMsgList;//返回重新发牌信息
    public static ReturnHyUser returnHyUser;//返回活跃用户
    public static ReturnFJ returnFJ;//返回翻鸡牌
    public static ReturnTP returnTP;//返回打出叫牌的集合
    public static ReturnBTMsg returnBTMSG;//返回听牌方位
    public static ReturnTT returnTT;
    public static ReturnJS returnJS;//返回结算信息(全体)
    public static ReturnJS cunJS;
    public static ReturnDJS returnDJS;//返回大结算信息（全体）
    public static ReturnJSMsg returnJSMsg;//返回请求解散房间信息
    public static ReturnRemoveUser returnRemoveUser;
    public static ReturnJSByOnew returnJSByOnew;//返回单个用户同意解散房间信息
    public static ReturnAllIdea returnALLIdea;//返回集体消息是否解散房间
    public static ReturnRoomMsg returnRoomMsg;//返回房间信息
    public static ReturnUserSPai returnUserSPai;//玩家手牌集合
    public static ReturnAYM returnAYM;//玩家手牌集合
    public static ReturnPaiCount returnPaiCount;
    public static ReturnHByType returnHByType;//返回胡牌（替换原3008的胡）
    public static ReturnHByType huPaiInfo;//返回胡牌（替换原3008的胡）用来发送给服务器胡牌信息
    public static ReturnZR returnZR;//下发责任鸡消息
    public static ReturnHType returnHType;//返回翻鸡牌之前返回的确定胡牌信息
    public static ReturnHType cunHType;
    public static ReturnDisbandedFailure returnDisbandedFailure = null;
    public static ReturnGroupInfo returnGroupInfo = null;
    public static ReturnGroupApplyInfo returnGroupApplyInfo = null;
    public static ReturnPlayerList returnPlayerList = null;
    public static ReturnRecordList returnRecordList = null;
    public static ReturnLobbyInfo returnLobbyInfo = null;
    public static ReturnGameOperation returnGameOperation = null;
    public static ReturnApplyToJoin returnApplyToJoin = null;
    public static ReturnQuitGroup returnQuitGroup = null;

    public static ReturnGetRoomCard returnGetRoomCard = null;
    public static ReturnMessgae returnMessgae = null;
    public static ReturnZhuang returnZhuang;//返回庄
    public static ReturnGang returnGang;//返回杠消息，5022
    public static ReturnGang cunGang;//将返回杠消息存起来，用来处理多个杠，不同类型的杠
    public static ReturnDis returnDis;//返回用户距离
    public static ReturnIsJ returnIsJ;//返回是否有相近用户
    public static ReturnIsJ5Seconds returnIsJ5Seconds;
    public static ReturnIPSame5Seconds returnIPSame5Seconds;
    public static ReturnIPSame returnIPSame;//返回是否有相同IP
    public static ReturnServerIP returnServerIP;//服務器返回IP和端口
    public static ReturnAddServer returnAddServer;//返回服務器狀態
    public static ReturnAnnouncement returnAnnouncement;//返回公告信息
    public static ReturnVoice returnVoice;           ///请求语音信息
    public static RoomInfo roomInfo = new RoomInfo();
    public static ReturnCloseGPS returnCloseGPS = null;
    public static ReturnCloseGPS5Seconds returnCloseGPS5Seconds = null;
    public static ReturnManaged returnManaged = null;
    public static ReturnConnectionStatus returnConnectionStatus = null;
    public static ReturnUserRecord returnUserRecord = null;
    public static ReturnGetUserGamePlayback returnGetUserGamePlayback = null;
    public static ReturnAll pengInfo;//碰杠牌的信息
    public static ReturnGroupInfoByGroupID returnGroupInfoByGroupID = null;
    public static ReturnGroupUserInfoByGroupID returnGroupUserInfoByGroupID = null;
    public static ReturnMessgaeList returnMessgaeList = null;
    //----------------------------------------------------------------------------------------------------------------------


    /// <summary>
    ///  清空本脚本所涉及的所有集合，更改
    /// </summary>
    public static void ClearAllListsAndChanges()
    {
        nowFW = 0;
        HYFw = 0;
        recon = false;//?
        cfj = false;
        cfwgj = false;
        CardsNumber = 0;
        isTT = false;
        isHhouTing = false;
        isClosed = false;
        queType = QueType.no;
        IsTingPai = false;
        isRealTing = false;
        IsDingQue = false;
        IsDingQueClick = false;
        IsDingQueGang = false;
        IsGang = false;
        pengInfo = null;
        huPaiInfo = null;
        returnHByType = null;
        returnTP = null;
        cunGang = null;
        jieSanOpenid = null;
        GroupID = 0;
        isStartGame = false;
        TT_bl = false;
        gameName = "";
        cunHtypeType = 0;
        addStatus = 0;
    }

    public static int UserHearbeat = 0;//记录心跳异常次数,未完成
    /// <summary>
    /// 心跳包
    /// </summary>
    /// <returns></returns>
    public static int Xintiao()
    {
		//MaintainHeartbeat()来自Google.ProtocolBuffers
        var hear = new MaintainHeartbeat();
        hear.state = 1;
		//openID是设备唯一标识符
        hear.openid = GameInfo.OpenID;
		//用户ID
        hear.userID = userID;
		//经纬度
        hear.Latitude = GameInfo.Latitude;
		//还是设备唯一标识符
        hear.unionid = GameInfo.unionid;
		//当Server的状态为game服务器时，房间ID也要上传至心跳包
        if (cs.serverType == ServerType.GameServer) hear.RoomID = room_id.ToString();

        byte[] body = ProtobufUtility.GetByteFromProtoBuf(hear);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1, body.Length, 0, body);

        try
        {

            GameInfo.cs.Send(data);
            UserHearbeat ++;
            
            return UserHearbeat;
        }
        catch (Exception ex)
        {
            UserHearbeat++;
            return UserHearbeat;
        }
    }
    /// <summary>
    /// 根据方位获取座位方向
    /// </summary>
    /// <param name="fw"></param>
    /// <returns></returns>
    public static FW GetFW(int fw)
    {
        if (GameInfo.room_peo != 2 && GameInfo.FW != fw)
        {
            if (GameInfo.room_peo == 4)
            {
                    switch (GameInfo.FW)
                    {
                        case 1:
                            return fw == 2 ? global::FW.East : (fw == 3 ? global::FW.North : global::FW.West);
                        case 2:
                            return fw == 3 ? global::FW.East : (fw == 4 ? global::FW.North : global::FW.West);
                        case 3:
                            return fw == 4 ? global::FW.East : (fw == 1 ? global::FW.North : global::FW.West);
                        case 4:
                            return fw == 1 ? global::FW.East : (fw == 2 ? global::FW.North : global::FW.West);
                    }
            }
            else
            {
                switch (GameInfo.FW)
                {
                    case 1:
                        return fw == 2 ? global::FW.East : global::FW.West;
                    case 2:
                        return fw == 3 ? global::FW.East : global::FW.West;
                    case 3:
                        return fw == 1 ? global::FW.East : global::FW.West;
                }
            }
        }
        else
        {
            if (GameInfo.FW != fw)
                return global::FW.North;
        }
        return global::FW.South;
    }
    /// <summary>
    /// 1,东 2，南  3，西  4，北
    /// </summary>
    /// <param name="thisfw"></param>
    /// <returns></returns>
    internal static int Rfw(int thisfw)
    {
        if (room_peo != 4)
        {
            if (thisfw == 2 && (room_peo == 2 || GameInfo.FW == 3))//如果是2人桌 第二个人坐在西方， 
            { return 3; }
            else if (thisfw == 3 && room_peo == 3 && GameInfo.FW != 2)//如果是3人桌  第三个人 坐在北方
            { return 4; }
        }
        return thisfw;
    }
}
/// <summary>
/// 杠牌类型
/// </summary>
public class GangType
{
    public static string mingGang = "M";
    public static string anGang = "A";
    public static string zhuanWanGang = "Z";
    public static string hanBaoGang = "H";
}


