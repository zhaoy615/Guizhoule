using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using MJBLL.common;
using DNL;
using UnityEngine.SceneManagement;

public class NetManager {

    private static NetManager _instance = null;
    public static NetManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new NetManager();
            }
            return _instance;
        }
    }
    private Socket clientSocket = null;

    private IPAddress iPAddress = null;
    private IPEndPoint endPort = null;
    private string nowSelfIp = null;
    private string ip = null;
    private NetManager()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        iPAddress = IPAddress.Parse(GameInfo.listIp);
        endPort = new IPEndPoint(iPAddress, int.Parse(GameInfo.listPort));
        ConnectServer();
        nowSelfIp = Network.player.ipAddress;
    }

    private void Start()
    {
        clientSocket.BeginReceive(Data, StartIndex, RemainSize, SocketFlags.None, ReceiveCallback, null);
    }
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            int count = clientSocket.EndReceive(ar);

            ReadMessage(count);

            Start();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    private byte[] data = new byte[2048];//存储接收的消息
    private byte[] nowData =null;//存储取出来的一条消息
    private int startIndex = 0;//我们存取了多少个字节的数据在数组里面
    public byte[] Data
    {
        get { return data; }
    }
    public byte[] NowData
    {
        get { return nowData; }
    }
    public int StartIndex
    {
        get { return startIndex; }
    }
    public int RemainSize
    {
        get { return data.Length - startIndex; }
    }
    
    /// <summary>
    /// 解析数据或者叫做读取数据
    /// </summary>
    public void ReadMessage(int newDataAmount)
    {
        startIndex += newDataAmount;
        while (true)
        {
            if (startIndex <= 12) return;
            int number = IntToByte.bytesToInt(data, 0);//消息号
            int length = IntToByte.bytesToInt(data, 4);//消息长度
            int resnumber = IntToByte.bytesToInt(data, 8);//返回消息号
            if ((startIndex - 12) >= length)
            {
                nowData = new byte[length];
                 Array.Copy(data,12,nowData,0,length);
                MethodsByNew(number,nowData);
                nowData = null;
                Array.Copy(data, length + 12, data, 0, startIndex -length-12 );
                startIndex -= (length + 4);
            }
            else
            {
                break;
            }
        }
    }

    public void MethodsByNew(int news, byte[] body)
    {
        Debug.Log("newsNumber" + news.ToString());
        GameInfo.UserHearbeat = 0;
        switch (news)
        {
            case 11002://服务器返回登录信息
                //GameInfo.returnlogin = ReturnLogin.ParseFrom(body);
                GameInfo.returnlogin = ProtobufUtility.DeserializeProtobuf<ReturnLogin>(body);
                // Debug.Log("11002:" + GameInfo.returnlogin.Loginstat);
                OutLog.log("11002:登录");
                break;
            case 12002://服务器返回房间信息
                //GameInfo.returnCreatRoom = ReturnCreateRoom.ParseFrom(body);
                GameInfo.returnCreatRoom = ProtobufUtility.DeserializeProtobuf<ReturnCreateRoom>(body);
                Debug.Log("12002:" + "返回房间信息" + GameInfo.returnCreatRoom.ToString());
                //OutLog.log("12002:" + GameInfo.returnCreatRoom.ToString());
                break;
            case 12004://服务器返回加入信息
                       // GameInfo.returnAddRoom = ReturnAddRoom.ParseFrom(body);
                GameInfo.returnAddRoom = ProtobufUtility.DeserializeProtobuf<ReturnAddRoom>(body);
                Debug.Log("12004:" + GameInfo.returnAddRoom.ToString());
                OutLog.log("12004:服务器返回加入信息");
                break;
            case 12005://服务器主动推送加入玩家信息
                //var returnUserInfo = ReturnUserInfo.ParseFrom(body);
                var returnUserInfo = ProtobufUtility.DeserializeProtobuf<ReturnUserInfo>(body);
                GameInfo.returnUserInfo = returnUserInfo;
                Debug.Log("12005:" + "服务端主动推送加入玩家信息" + returnUserInfo.ToString());
                OutLog.log("12005:服务端主动推送加入玩家信息");
                break;
            case 12006://服务器返回主动推送游戏开始信息
                       // var returnStartGame = ReturnStartGame.ParseFrom(body);
                var returnStartGame = ProtobufUtility.DeserializeProtobuf<ReturnStartGame>(body);
                GameInfo.returnStartGame = returnStartGame;
                Debug.Log("12006:" + "服务端推送游戏开始信息" + returnStartGame.ToString());
                OutLog.log("12006:服务端推送游戏开始信息");
                break;
            case 12008://服务器返回开始游戏
                //var returnStart = ReturnStart.ParseFrom(body);
                var returnStart = ProtobufUtility.DeserializeProtobuf<ReturnStart>(body);
                GameInfo.returnStart = returnStart;
                Debug.Log("12008:" + "开始游戏" + returnStart.ToString());
                OutLog.log("12008:开始游戏");
                break;
            case 12010://服务器返回开始游戏
                //var returnTTorTH = ReturnTTATH.ParseFrom(body);
                var returnTTorTH = ProtobufUtility.DeserializeProtobuf<ReturnTTATH>(body);
                GameInfo.returnTTOrTH = returnTTorTH;
                Debug.Log("12010:" + "返回打出叫牌的集合" + returnTTorTH.ToString());
                OutLog.log("12010:返回打出叫牌的集合");
                break;

            case 13009://返回出牌信息，出牌方，出牌的花色
                //var returnMsg = ReturnMsg.ParseFrom(body);
                var returnMsg = ProtobufUtility.DeserializeProtobuf<ReturnMsg>(body);
                GameInfo.returnMsg = returnMsg;
                Debug.Log("13009:" + "下发打出牌的消息" + returnMsg.ToString());
                OutLog.log("13009:下发打出牌的消息");
                break;
            case 13008://返回碰杠胡摸 四个信息和 方位
                       //var returnAll = ReturnAll.ParseFrom(body);
                var returnAll = ProtobufUtility.DeserializeProtobuf<ReturnAll>(body);
                GameInfo.returnAll = returnAll;
                Debug.Log("13008:" + "下发碰杠胡消息" + returnAll.ToString());
                OutLog.log("13008:下发碰杠胡消息");
                break;
            case CreateHead.CSXYNUM + 3003://返回摸牌
                //var returnMP = ReturnMP.ParseFrom(body);
                var returnMP = ProtobufUtility.DeserializeProtobuf<ReturnMP>(body);
                GameInfo.returnMP = returnMP;
                Debug.Log(CreateHead.CSXYNUM + 3003 + "返回摸牌 " + returnMP.ToString());
                OutLog.log("13003:返回摸牌");
                break;
            case CreateHead.CSXYNUM + 4002://返回下发碰杠胡消息
                //var returnPeng = ReturnPeng.ParseFrom(body);
                var returnPeng = ProtobufUtility.DeserializeProtobuf<ReturnPeng>(body);
                GameInfo.returnPeng = returnPeng;
                Debug.Log(CreateHead.CSXYNUM + 4002 + "下发碰杠胡后消息" + returnPeng.ToString());
                OutLog.log("14002:下发碰杠胡后消息");
                break;

            case CreateHead.CSXYNUM + 5002://返回听牌方位
                //var returnBTMsg = ReturnBTMsg.ParseFrom(body);
                var returnBTMsg = ProtobufUtility.DeserializeProtobuf<ReturnBTMsg>(body);
                GameInfo.returnBTMSG = returnBTMsg;
                Debug.Log(CreateHead.CSXYNUM + 5002 + "返回听牌方位" + returnBTMsg.ToString());
                OutLog.log("15002:返回听牌方位");
                break;
            case CreateHead.CSXYNUM + 5013://返回听牌方位
                //var returnTT = ReturnTT.ParseFrom(body);
                var returnTT = ProtobufUtility.DeserializeProtobuf<ReturnTT>(body);
                GameInfo.returnTT = returnTT;
                Debug.Log(CreateHead.CSXYNUM + 5013 + "返回天听方位(所有人)" + returnTT.ToString());
                OutLog.log("15013:返回天听方位(所有人)");
                break;

            case CreateHead.CSXYNUM + 6001://返回是否、
                //var returnRecon = ReturnRecon.ParseFrom(body);
                var returnRecon = ProtobufUtility.DeserializeProtobuf<ReturnRecon>(body);
                GameInfo.returnRecon = returnRecon;
                Debug.Log(CreateHead.CSXYNUM + 6001 + "判断是否断线重连" + returnRecon.ToString());
                OutLog.log("16001:判断是否断线重连");
                break;
            case CreateHead.CSXYNUM + 7001://返回断线前消息
                //var returnConnData = ReturnConnData.ParseFrom(body);
                var returnConnData = ProtobufUtility.DeserializeProtobuf<ReturnConnData>(body);
                GameInfo.returnConnData = returnConnData;
                Debug.Log(CreateHead.CSXYNUM + 7001 + "返回断线前信息" + returnConnData.ToString());
                OutLog.log("17001:返回断线前信息");
                break;
            case CreateHead.CSXYNUM + 7002://返回重新发牌信息
                //var returnMsgList = ReturnMsgList.ParseFrom(body);
                var returnMsgList = ProtobufUtility.DeserializeProtobuf<ReturnMsgList>(body);
                GameInfo.returnMsgList = returnMsgList;
                //  Debug.Log(CreateHead.CSXYNUM + 7002 + "返回重发信息" + returnMsgList.ToString());
                OutLog.log("17002:返回重发信息");
                break;
            case CreateHead.CSXYNUM + 7004://返回活跃用户
                  if (GameInfo.returnHyUser == null)
                    {
                        //var returnHyUser = ReturnHyUser.ParseFrom(body);
                        var returnHyUser = ProtobufUtility.DeserializeProtobuf<ReturnHyUser>(body);
                        GameInfo.returnHyUser = returnHyUser;
                        Debug.Log(CreateHead.CSXYNUM + 7004 + "返回活跃用户" + returnHyUser.ToString());
                        OutLog.log("17004:返回活跃用户");
                        break;
                    }
             
                break;
            case CreateHead.CSXYNUM + 7005://返回翻鸡牌
                //var returnFJ = ReturnFJ.ParseFrom(body);
                var returnFJ = ProtobufUtility.DeserializeProtobuf<ReturnFJ>(body);
                GameInfo.returnFJ = returnFJ;
                Debug.Log(CreateHead.CSXYNUM + 7005 + "返回翻鸡牌" + returnFJ.ToString());
                OutLog.log("17005:返回翻鸡牌");
                break;
            case CreateHead.CSXYNUM + 2009://返回打出叫牌的集合
                //var returnTP = ReturnTP.ParseFrom(body);
                var returnTP = ProtobufUtility.DeserializeProtobuf<ReturnTP>(body);
                GameInfo.returnTP = returnTP;
                Debug.Log(CreateHead.CSXYNUM + 2009 + "返回打出叫牌的集合" + returnTP.ToString());
                OutLog.log("12009:返回打出叫牌的集合");
                break;
            case CreateHead.CSXYNUM + 7009://返回结算信息(全体)
                //var returnJS = ReturnJS.ParseFrom(body);
                var returnJS = ProtobufUtility.DeserializeProtobuf<ReturnJS>(body);
                GameInfo.returnJS = returnJS;
                //  Debug.Log(CreateHead.CSXYNUM + 7009 + "返回结算信息(全体)" + returnJS.ToString());
                OutLog.log("17009:返回结算信息(全体)");
                break;
            case CreateHead.CSXYNUM + 5004://返回请求解散房间信息(所有人接收，客户端自主判断是否显示)
                //var returnJSMsg = ReturnJSMsg.ParseFrom(body);
                var returnJSMsg = ProtobufUtility.DeserializeProtobuf<ReturnJSMsg>(body);
                GameInfo.returnJSMsg = returnJSMsg;
                //  Debug.Log(CreateHead.CSXYNUM + 5004 + "返回请求解散房间信息" + returnJSMsg.ToString());
                OutLog.log("15004:返回请求解散房间信息");
                break;
            case CreateHead.CSXYNUM + 5006://返回单个用户同意解散房间信息
                //var returnJSByOnew = ReturnJSByOnew.ParseFrom(body);
                var returnJSByOnew = ProtobufUtility.DeserializeProtobuf<ReturnJSByOnew>(body);
                GameInfo.returnJSByOnew = returnJSByOnew;
                Debug.Log(CreateHead.CSXYNUM + 5006 + "返回单个用户同意信息" + returnJSByOnew.ToString());
                OutLog.log("15006:返回单个用户同意信息");
                break;
            case CreateHead.CSXYNUM + 5007://返回集体消息是否解散房间
                //var returnALLIdea = ReturnAllIdea.ParseFrom(body);
                var returnALLIdea = ProtobufUtility.DeserializeProtobuf<ReturnAllIdea>(body);
                GameInfo.returnALLIdea = returnALLIdea;
                Debug.Log(CreateHead.CSXYNUM + 5007 + "返回集体消息是否解散房间" + returnALLIdea.ToString());
                OutLog.log("15007:返回集体消息是否解散房间");
                break;
            case CreateHead.CSXYNUM + 5009://服务端返回退出玩家 
                //var returnRemoveUser = ReturnRemoveUser.ParseFrom(body);
                var returnRemoveUser = ProtobufUtility.DeserializeProtobuf<ReturnRemoveUser>(body);
                GameInfo.returnRemoveUser = returnRemoveUser;
                Debug.Log(CreateHead.CSXYNUM + 5009 + "服务端返回退出玩家" + returnRemoveUser.ToString());
                OutLog.log("15009:服务端返回退出玩家");
                break;
            case CreateHead.CSXYNUM + 7003://返回房间信息
                //var returnRoomMsg = ReturnRoomMsg.ParseFrom(body);
                var returnRoomMsg = ProtobufUtility.DeserializeProtobuf<ReturnRoomMsg>(body);
                GameInfo.returnRoomMsg = returnRoomMsg;
                Debug.Log(CreateHead.CSXYNUM + 7003 + "返回房间信息" + returnRoomMsg.ToString());
                OutLog.log("17003:返回房间信息");
                break;
            case CreateHead.CSXYNUM + 7008://玩家手牌集合
                //var returnUserSPai = ReturnUserSPai.ParseFrom(body);
                var returnUserSPai = ProtobufUtility.DeserializeProtobuf<ReturnUserSPai>(body);
                GameInfo.returnUserSPai = returnUserSPai;
                Debug.Log(CreateHead.CSXYNUM + 7008 + "玩家手牌集合" + returnUserSPai.ToString());
                OutLog.log("17008:玩家手牌集合");
                break;
            case CreateHead.CSXYNUM + 5012://服务器下发定缺信息
                //var returnAYM = ReturnAYM.ParseFrom(body);
                var returnAYM = ProtobufUtility.DeserializeProtobuf<ReturnAYM>(body);
                GameInfo.returnAYM = returnAYM;
                Debug.Log(CreateHead.CSXYNUM + 5012 + "返回缺牌信息" + returnAYM.ToString());
                OutLog.log("15012:返回缺牌信息");
                break;
            case CreateHead.CSXYNUM + 5015://返回胡牌（替换原3008的胡
                //var returnHByType = ReturnHByType.ParseFrom(body);
                var returnHByType = ProtobufUtility.DeserializeProtobuf<ReturnHByType>(body);
                GameInfo.returnHByType = returnHByType;
                Debug.Log(CreateHead.CSXYNUM + 5015 + "返回胡牌" + returnHByType.ToString());
                OutLog.log("15015:返回胡牌");
                break;
            case CreateHead.CSXYNUM + 7006://返回剩余牌堆数目
                //var returnPaiCount = ReturnPaiCount.ParseFrom(body);
                var returnPaiCount = ProtobufUtility.DeserializeProtobuf<ReturnPaiCount>(body);
                GameInfo.returnPaiCount = returnPaiCount;
                Debug.Log(CreateHead.CSXYNUM + 7006 + "返回剩余牌堆数" + returnPaiCount.ToString());
                OutLog.log("17006:返回剩余牌堆数");
                break;
            case CreateHead.CSXYNUM + 1://心跳
                GameInfo.UserHearbeat = 0;
                break;
            case CreateHead.CSXYNUM + 5014:
                //var returnDJS = ReturnDJS.ParseFrom(body);
                var returnDJS = ProtobufUtility.DeserializeProtobuf<ReturnDJS>(body);
                GameInfo.returnDJS = returnDJS;
                Debug.Log(CreateHead.CSXYNUM + 5014 + "大结算信息" + returnDJS.ToString());
                OutLog.log("15014:大结算信息");
                break;
            case CreateHead.CSXYNUM + 5018:
                //var returnZR = ReturnZR.ParseFrom(body);
                var returnZR = ProtobufUtility.DeserializeProtobuf<ReturnZR>(body);
                GameInfo.returnZR = returnZR;
                Debug.Log(CreateHead.CSXYNUM + 5018 + "下发责任鸡消息" + returnZR.ToString());
                OutLog.log("15018:下发责任鸡消息");
                break;
            case CreateHead.CSXYNUM + 5020:
                //var returnZhuang = ReturnZhuang.ParseFrom(body);
                var returnZhuang = ProtobufUtility.DeserializeProtobuf<ReturnZhuang>(body);
                GameInfo.returnZhuang = returnZhuang;
                Debug.Log(CreateHead.CSXYNUM + 5020 + "返回庄" + returnZhuang.ToString());
                OutLog.log("15020:返回庄");
                break;
            case CreateHead.CSXYNUM + 5021:
                //var returnHType = ReturnHType.ParseFrom(body);
                var returnHType = ProtobufUtility.DeserializeProtobuf<ReturnHType>(body);
                GameInfo.returnHType = returnHType;
                Debug.Log(CreateHead.CSXYNUM + 5021 + "返回胡信息" + returnHType.ToString());
                OutLog.log("15021:返回胡信息");
                break;
            case CreateHead.CSXYNUM + 5022:
                //var returnGang = ReturnGang.ParseFrom(body);
                var returnGang = ProtobufUtility.DeserializeProtobuf<ReturnGang>(body);
                GameInfo.returnGang = returnGang;
                Debug.Log(CreateHead.CSXYNUM + 5022 + "返回杠消息" + returnGang.ToString());
                OutLog.log("15022:返回杠消息");
                break;
            case CreateHead.CSXYNUM + 7090:
                //var returnRoomAdd = ReturnRoomAdd.ParseFrom(body);
                var returnRoomAdd = ProtobufUtility.DeserializeProtobuf<ReturnRoomAdd>(body);
                GameInfo.returnRoomAdd = returnRoomAdd;
                Debug.Log(CreateHead.CSXYNUM + 7090 + "返回加入房间状态" + returnRoomAdd.ToString());
                OutLog.log("17090:返回加入房间状态");
                break;
            case CreateHead.CSXYNUM + 7091:
                //var returnDis = ReturnDis.ParseFrom(body);
                var returnDis = ProtobufUtility.DeserializeProtobuf<ReturnDis>(body);
                GameInfo.returnDis = returnDis;
                Debug.Log(CreateHead.CSXYNUM + 7091 + "返回用户距离" + returnDis.ToString());
                OutLog.log(returnDis.FW + "与" + returnDis.FWT + "相距：" + returnDis.dis);
                Debug.Log(returnDis.FW + "与" + returnDis.FWT + "相距：" + returnDis.dis);
                break;
            case CreateHead.CSXYNUM + 7092:
                //var returnIsJ = ReturnIsJ.ParseFrom(body);
                var returnIsJ = ProtobufUtility.DeserializeProtobuf<ReturnIsJ>(body);
                GameInfo.returnIsJ = returnIsJ;
                Debug.Log(CreateHead.CSXYNUM + 7092 + "返回是否有相近用户" + returnIsJ.ToString());
                OutLog.log("17092:返回是否有相近用户");
                break;
            case CreateHead.CSXYNUM + 7093:
                //var returnIPSame = ReturnIPSame.ParseFrom(body);
                var returnIPSame = ProtobufUtility.DeserializeProtobuf<ReturnIPSame>(body);
                GameInfo.returnIPSame = returnIPSame;
                Debug.Log(CreateHead.CSXYNUM + 7093 + "返回是否有相同IP" + returnIPSame.ToString());
                OutLog.log("17093:返回是否有相同IP");
                break;
            case CreateHead.CSXYNUM + 1010:
                //var returnServerIP = ReturnServerIP.ParseFrom(body);
                var returnServerIP = ProtobufUtility.DeserializeProtobuf<ReturnServerIP>(body);
                GameInfo.returnServerIP = returnServerIP;
                Debug.Log(CreateHead.CSXYNUM + 1010 + "服务器返回IP和端口" + returnServerIP.ToString());
                OutLog.log("11010:服务器返回IP和端口");
                break;
            case CreateHead.CSXYNUM + 1012:
                //var returnAddServer = ReturnAddServer.ParseFrom(body);
                var returnAddServer = ProtobufUtility.DeserializeProtobuf<ReturnAddServer>(body);
                GameInfo.returnAddServer = returnAddServer;
                Debug.Log(CreateHead.CSXYNUM + 1012 + "连接上服务器后,服务器返回状态" + returnAddServer.ToString());
                OutLog.log("11012:连接上服务器后,服务器返回状态");
                break;
            case CreateHead.CSXYNUM + 8001:
                //var returnAnnouncement = ReturnAnnouncement.ParseFrom(body);
                var returnAnnouncement = ProtobufUtility.DeserializeProtobuf<ReturnAnnouncement>(body);
                GameInfo.returnAnnouncement = returnAnnouncement;
                Debug.Log(CreateHead.CSXYNUM + 8001 + "返回公告信息" + returnAnnouncement.ToString());
                OutLog.log("18001:返回公告信息");
                break;

            case CreateHead.CSXYNUM + 8003:
                //var returnVoice = ReturnVoice.ParseFrom(body);
                var returnVoice = ProtobufUtility.DeserializeProtobuf<ReturnVoice>(body);
                GameInfo.returnVoice = returnVoice;
                Debug.Log(CreateHead.CSXYNUM + 8003 + "返回语音信息" + returnVoice.ToString());
                OutLog.log("18003:返回语音信息");
                break;
            case CreateHead.CSXYNUM + 8004:
                //var returnConnectionStatus = ReturnConnectionStatus.ParseFrom(body);
                var returnConnectionStatus = ProtobufUtility.DeserializeProtobuf<ReturnConnectionStatus>(body);
                GameInfo.returnConnectionStatus = returnConnectionStatus;
                Debug.Log(CreateHead.CSXYNUM + 8004 + "返回连接状态" + returnConnectionStatus.ToString());
                //OutLog.log(CreateHead.CSXYNUM + 8004 + returnConnectionStatus.ToString() );
                break;
            case CreateHead.CSXYNUM + +9002:
                //var returnUserRecord = ReturnUserRecord.ParseFrom(body);
                var returnUserRecord = ProtobufUtility.DeserializeProtobuf<ReturnUserRecord>(body);
                GameInfo.returnUserRecord = returnUserRecord;
                Debug.Log(CreateHead.CSXYNUM + 9002 + "返回用户战绩列表" + returnUserRecord.ToString());
                OutLog.log("19002:返回用户战绩列表");
                break;
            case CreateHead.CSXYNUM + +9004:
                //var returnGetUserGamePlayback = ReturnGetUserGamePlayback.ParseFrom(body);
                var returnGetUserGamePlayback = ProtobufUtility.DeserializeProtobuf<ReturnGetUserGamePlayback>(body);
                GameInfo.returnGetUserGamePlayback = returnGetUserGamePlayback;
                Debug.Log(CreateHead.CSXYNUM + 9004 + "根据牌桌信息ID返回多局牌集合" + returnGetUserGamePlayback.ToString());
                OutLog.log("19004:根据牌桌信息ID返回多局牌集合");
                break;
            case CreateHead.CSXYNUM + +7095:
                //var returnIsJ5Seconds = ReturnIsJ5Seconds.ParseFrom(body);
                var returnIsJ5Seconds = ProtobufUtility.DeserializeProtobuf<ReturnIsJ5Seconds>(body);
                GameInfo.returnIsJ5Seconds = returnIsJ5Seconds;
                Debug.Log(CreateHead.CSXYNUM + 7095 + "每五秒返回是否有相近用户" + returnIsJ5Seconds.ToString());
                //OutLog.log(CreateHead.CSXYNUM + 7095 + returnIsJ5Seconds.ToString() );
                break;
            case CreateHead.CSXYNUM + +7096:
                //var returnIPSame5Seconds = ReturnIPSame5Seconds.ParseFrom(body);
                var returnIPSame5Seconds = ProtobufUtility.DeserializeProtobuf<ReturnIPSame5Seconds>(body);
                GameInfo.returnIPSame5Seconds = returnIPSame5Seconds;
                Debug.Log(CreateHead.CSXYNUM + 7096 + "每五秒返回是否y有相同IP" + returnIPSame5Seconds.ToString());
                //OutLog.log(CreateHead.CSXYNUM + 7096 + returnIPSame5Seconds.ToString() );
                break;
            case CreateHead.CSXYNUM + +7097:
                //var returnCloseGPS = ReturnCloseGPS.ParseFrom(body);
                var returnCloseGPS = ProtobufUtility.DeserializeProtobuf<ReturnCloseGPS>(body);
                GameInfo.returnCloseGPS = returnCloseGPS;
                Debug.Log(CreateHead.CSXYNUM + 7097 + "返回是否开启GPS" + returnCloseGPS.ToString());
                //OutLog.log(CreateHead.CSXYNUM + 7097 + returnCloseGPS.ToString() );
                break;
            case CreateHead.CSXYNUM + +7098:
                var returnCloseGPS5Seconds = ProtobufUtility.DeserializeProtobuf<ReturnCloseGPS5Seconds>(body);
                GameInfo.returnCloseGPS5Seconds = returnCloseGPS5Seconds;
                Debug.Log(CreateHead.CSXYNUM + 7098 + "返回是否开启GPS" + returnCloseGPS5Seconds.ToString());
                //OutLog.log(CreateHead.CSXYNUM + 7098 + returnCloseGPS5Seconds.ToString() );
                break;
            case CreateHead.CSXYNUM + +2011:
                //var returnManaged = ReturnManaged.ParseFrom(body);
                var returnManaged = ProtobufUtility.DeserializeProtobuf<ReturnManaged>(body);
                GameInfo.returnManaged = returnManaged;
                Debug.Log(CreateHead.CSXYNUM + 2011 + "返回十秒出牌" + returnManaged.ToString());
                //OutLog.log(CreateHead.CSXYNUM + 2011 + returnManaged.ToString() );
                break;
            case CreateHead.CSXYNUM + +5010:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnDisbandedFailure = ProtobufUtility.DeserializeProtobuf<ReturnDisbandedFailure>(body);
                GameInfo.returnDisbandedFailure = returnDisbandedFailure;
                Debug.Log(CreateHead.CSXYNUM + 5010 + "解散失败后，返回状态" + returnDisbandedFailure.ToString());
                OutLog.log("15010:解散失败后，返回状态");
                break;

            case CreateHead.CSXYNUM + +1031:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnGroupInfo = ProtobufUtility.DeserializeProtobuf<ReturnGroupInfo>(body);
                GameInfo.returnGroupInfo = returnGroupInfo;
                Debug.Log(CreateHead.CSXYNUM + 1031 + "返回朋友圈信息" + returnGroupInfo.ToString());
                OutLog.log("11031:返回朋友圈信息");
                break;
            case CreateHead.CSXYNUM + +1061:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnGroupApplyInfo = ProtobufUtility.DeserializeProtobuf<ReturnGroupApplyInfo>(body);
                GameInfo.returnGroupApplyInfo = returnGroupApplyInfo;
                Debug.Log(CreateHead.CSXYNUM + 1061 + "返回申请记录" + returnGroupApplyInfo.ToString());
                OutLog.log("11061:返回申请记录");
                break;

            case CreateHead.CSXYNUM + +1051:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnPlayerList = ProtobufUtility.DeserializeProtobuf<ReturnPlayerList>(body);
                GameInfo.returnPlayerList = returnPlayerList;
                Debug.Log(CreateHead.CSXYNUM + 1051 + "获取圈内玩家列表" + returnPlayerList.ToString());
                OutLog.log("11051:获取圈内玩家列表");
                break;
            case CreateHead.CSXYNUM + +1052:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnRecordList = ProtobufUtility.DeserializeProtobuf<ReturnRecordList>(body);
                GameInfo.returnRecordList = returnRecordList;
                Debug.Log(CreateHead.CSXYNUM + 1052 + "圈内玩家最近开房记录" + returnRecordList.ToString());
                OutLog.log("11052:圈内玩家最近开房记录");
                break;
            case CreateHead.CSXYNUM + +1081:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnLobbyInfo = ProtobufUtility.DeserializeProtobuf<ReturnLobbyInfo>(body);
                GameInfo.returnLobbyInfo = returnLobbyInfo;
                Debug.Log(CreateHead.CSXYNUM + 1081 + "返回大厅信息" + returnLobbyInfo.ToString());
                OutLog.log("11081:返回大厅信息");
                break;
            case CreateHead.CSXYNUM + +1021:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnGameOperation = ProtobufUtility.DeserializeProtobuf<ReturnGameOperation>(body);
                GameInfo.returnGameOperation = returnGameOperation;
                Debug.Log(CreateHead.CSXYNUM + 1021 + "返回操作是否成功" + returnGameOperation.ToString());
                OutLog.log("11021:返回操作是否成功");
                break;
            case CreateHead.CSXYNUM + +1041:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnApplyToJoin = ProtobufUtility.DeserializeProtobuf<ReturnApplyToJoin>(body);
                GameInfo.returnApplyToJoin = returnApplyToJoin;
                Debug.Log(CreateHead.CSXYNUM + 1021 + "返回申请结果" + returnApplyToJoin.ToString());
                OutLog.log("11021:返回申请结果");
                break;
            case CreateHead.CSXYNUM + +1091:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnQuitGroup = ProtobufUtility.DeserializeProtobuf<ReturnQuitGroup>(body);
                GameInfo.returnQuitGroup = returnQuitGroup;
                Debug.Log(CreateHead.CSXYNUM + 1091 + "返回退出结果" + returnQuitGroup.ToString());
                OutLog.log("11091:返回退出结果");
                break;
            case CreateHead.CSXYNUM + +1023:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnGetRoomCard = ProtobufUtility.DeserializeProtobuf<ReturnGetRoomCard>(body);
                GameInfo.returnGetRoomCard = returnGetRoomCard;
                Debug.Log(CreateHead.CSXYNUM + 1023 + "返回房卡信息" + returnGetRoomCard.ToString());
                OutLog.log("11023:返回房卡信息");
                break;
            case CreateHead.CSXYNUM + +1104:
                //var returnDisbandedFailure = ReturnDisbandedFailure.ParseFrom(body);
                var returnMessgae = ProtobufUtility.DeserializeProtobuf<ReturnMessgae>(body);
                GameInfo.returnMessgae = returnMessgae;
                Debug.Log(CreateHead.CSXYNUM + 1104 + "返回消息" + returnMessgae.ToString());
                OutLog.log("11104:返回消息");
                break;
        }
    }

    public void SendDataToServer(byte[] sendData)
    {
        if (clientSocket != null)
        {

            if (clientSocket.Connected)
            {
                if (JudgeIsChangeSelfIpChange())
                {
                    clientSocket.Send(sendData);
                }
                else
                {
                    if (!GameInfo.isAllreadyStart)
                    {
                        FICWaringPanel._instance.Show("您的IP发生改变，房间已解散或退出，点击确定返回大厅！");
                        FICWaringPanel._instance.WarnningMethods = delegate { SceneManager.LoadScene("Scene_Hall"); };
                        nowSelfIp = Network.player.ipAddress;
                        ReconnnectServer();

                    }
                    else
                    {
                        AddServer();

                    }
                }
            }
            else
            {
                OutLog.log("clientsocket 没有连接，不能发送");
                AddServer();
            }
        }
        else
        {
            OutLog.log("clientsocket 为空，不能发送");
        }
    }
    public void ChangeIp(string ip,string port)
    {
        CloseConnect();
        iPAddress = IPAddress.Parse(ip);
        endPort = new IPEndPoint(iPAddress, int.Parse(port));
        ConnectServer();
    }
    public void ReconnnectServer()
    {
        CloseConnect();
        ConnectServer();
        AddServer();
    }
    private void ConnectServer()
    {
        try
        {
            clientSocket.Connect(endPort);
            Start();
        }
        catch (Exception e)
        {
            OutLog.log("无法连接到服务器端，请检查您的网络！！" + e);
        }

    }
    public void CloseConnect()
    {
        try
        {
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法关闭跟服务器端的连接！！" + e);
        }
    }
    public void AddServer()
    {
        SendAddServer sendAddServer = new SendAddServer();//..CreateBuilder()
        sendAddServer.openid = GameInfo.OpenID;
        sendAddServer.unionid = GameInfo.unionid;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendAddServer);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1011, body.Length, 0, body);
        SendDataToServer(data);
    }
  
    private bool JudgeIsChangeSelfIpChange()
    {
        if (nowSelfIp== Network.player.ipAddress)
        {
            return true;
        }
        else
        {
            nowSelfIp = Network.player.ipAddress;
            return false;
        }
    }

    #region 两个方法，有空挪
    public void SendCreatRoomMessage(string openid, int is_wgj, int is_xinqiji, int is_shangxiaji, int is_benji, int is_yikousan, int room_peo, int count, string latitude, int isYuanQuan, int isSMCP, int groupID, int is_lianzhuang = 3)
    {

        // SendCreateRoom creatRoom = SendCreateRoom.CreateBuilder()
        //.SetOpenid(openid)
        //.SetIsWgj(is_wgj)
        //.SetIsXinqiji(is_xinqiji)
        //.SetIsShangxiaji(is_shangxiaji)
        //.SetIsBenji(is_benji)
        //.SetIsYikousan(is_yikousan)
        //.SetRoomPeo(room_peo)
        //.SetCount(count).SetIsLianzhuang(is_lianzhuang)
        //.SetLatitude(latitude)
        //.SetIsYuanque(isYuanQuan)
        //.SetQuickCard(isSMCP)
        //.Build();

        //byte[] body = creatRoom.ToByteArray();
        SendCreateRoom creatRoom = new SendCreateRoom();
        creatRoom.openid = openid;
        creatRoom.is_wgj = is_wgj;
        creatRoom.is_xinqiji = is_xinqiji;
        creatRoom.is_shangxiaji = is_shangxiaji;
        creatRoom.is_benji = is_benji;
        creatRoom.is_yikousan = is_yikousan;
        creatRoom.room_peo = room_peo;
        creatRoom.count = count;
        creatRoom.Latitude = latitude;
        creatRoom.Is_yuanque = isYuanQuan;
        creatRoom.QuickCard = isSMCP;
        creatRoom.GroupID = groupID;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(creatRoom);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 2001, body.Length, 0, body);
        Debug.Log("2001" + "创建房间协议" + creatRoom);
        SendDataToServer(data);
    }
    /// <summary>
    /// 发送登录信息
    /// </summary>
    /// <param name="openid">玩家账号</param>
    /// <param name="nickname">玩家昵称</param>
    /// <param name="sex">玩家性别</param>
    /// <param name="province">玩家地址</param>
    /// <param name="city">玩家所在地</param>
    /// <param name="headimg">玩家头像</param>
    /// <param name="unionid">唯一标识符</param>
    public void SentUserLoginMessage(string openid, string nickname, string sex, string province, string city, string headimg, string unionid, string latitude)
    {
        //    SendLogin login = SendLogin.CreateBuilder()
        //                    .SetOpenid(openid)
        //                    .SetNickname(nickname)
        //                    .SetSex(sex)
        //                    .SetProvince(province)
        //                    .SetCity(city)
        //                    .SetHeadimg(headimg)
        //                    .SetUnionid(unionid)
        //                    .SetLatitude(latitude)
        //                    .Build();


        //    byte[] body = login.ToByteArray();
        SendLogin login = new SendLogin();
        login.openid = openid;
        login.nickname = nickname;
        login.sex = sex;
        login.province = province;
        login.city = city;
        login.headimg = headimg;
        login.unionid = unionid;
        login.Latitude = latitude;
        Debug.Log(openid + "\t" + nickname + "\t" + sex + "\t" + province + "\t" + city + "\t" + headimg + "\t" + unionid + "\t" + latitude);
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(login);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1001, body.Length, 0, body);
        // if (clientSocket.Connected)




        // Deseriliz(body);



      SendDataToServer(data);
      

    }

    #endregion
}
