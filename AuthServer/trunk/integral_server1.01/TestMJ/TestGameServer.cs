using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MJBLL.common;
using System.IO;

namespace TestMJ
{
    public partial class TestGameServer : Form
    {
        public TestGameServer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          var  myThread = new Thread(new ThreadStart(ConnectionList));
            myThread.IsBackground = true;
            myThread.Start();
           // ConnectionList();
            
        }

        private void ConnectionList()
        {
            IPAddress ipaddress;
            EndPoint point;
            Socket sc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipaddress = IPAddress.Parse("192.168.2.54");
            point = new IPEndPoint(ipaddress, int.Parse("2018"));
            sc.Connect(point);
            sc.Blocking = true;
            string openid = Guid.NewGuid().ToString();
            SendLogin login = SendLogin.CreateBuilder()
                       .SetOpenid(openid)
                       .SetNickname("测试用户" + openid.Substring(0, 3))
                       .SetSex("1")
                       .SetProvince("贵州")
                       .SetCity("贵阳")
                       .SetHeadimg("http://imgsrc.baidu.com/forum/pic/item/34aa0df3d7ca7bcb1620282fb8096b63f724a8ff.jpg")
                       .SetUnionid(openid)
                       .SetLatitude("0,0")
                       .Build();


            byte[] body = login.ToByteArray();
            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1001, body.Length, 0, body);
            // if (clientSocket.Connected)
            //Connect();
            sc.Send(data);
            ReceiveMessage(sc, openid);
        }

        /// <summary>
        /// 接受从服务器发来的消息，根据不同消息号，分发给不同的方法
        /// </summary>
        public void ReceiveMessage(Socket sc,string openid)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                while (true)
                {
                    int count;
                    byte[] data = new byte[2048];
                    // ms = new MemoryStream();
                    while (0 != (count = sc.Receive(data)))
                    {
                        ms.Write(data, 0, count);
                        if (count != data.Length)
                            break;
                        // if (count < 1024) break;
                    }
                    while (ms.Length > 0)
                    {
                        var datainfo = ms.ToArray();
                        int number = IntToByte.bytesToInt(datainfo, 0);//消息号
                        int length = IntToByte.bytesToInt(datainfo, 4);//消息长度
                        int resnumber = IntToByte.bytesToInt(datainfo, 8);//返回消息号
                      //  Debug.Log(number + ",长度" + length + ",包长" + datainfo.Length); OutLog.log(number + ",长度" + length + ",包长" + datainfo.Length);
                        if (number < CreateHead.CSXYNUM || number > CreateHead.CSXYNUM * 2)
                        {
                            ms = new MemoryStream();
                            break;
                        }
                        if (length > ms.Length)
                            break;
                        byte[] body = new byte[length];
                        Array.Copy(datainfo, 12, body, 0, length);
                        ms = new MemoryStream();
                        if (datainfo.Length > length + 12)
                        {
                            ms.Write(datainfo, length + 12, datainfo.Length - length - 12);
                        }
                        //  Debug.Log(number);
                        MethodsByNew(number, body, openid,sc);
                    }
                }
            }
            catch (Exception ex)
            {
               // Debug.Log(ex.ToString());
            }
        }

        /// <summary>
        /// 根据消息号分发给不同的方法
        /// </summary>
        /// <param name="news"></param>
        public void MethodsByNew(int news, byte[] body,string openid ,Socket sc, int operation=1,int roomid=0)
        {
            switch (news)
            {
                case CreateHead.CSXYNUM + 1010:
                    var returnServerIP = ReturnServerIP.ParseFrom(body);
                    CreatNewSocket(returnServerIP, sc, openid);
                    //GameInfo.returnServerIP = returnServerIP;
                    //Debug.Log(CreateHead.CSXYNUM + 1010 + "务器返回IP和端口" + returnServerIP.ToString()); OutLog.log(CreateHead.CSXYNUM + 1010 + returnServerIP.ToString());
                    break;
                case CreateHead.CSXYNUM + 1012:
                    SendCreatRoomMessage(sc, openid, 1, 0, 1, 1, 1, 4, 8, "0,0", 1, 1);
                    //var returnAddServer = ReturnAddServer.ParseFrom(body);
                    //GameInfo.returnAddServer = returnAddServer;
                    //Debug.Log(CreateHead.CSXYNUM + 1012 + "连接上服务器后,服务器返回状态" + returnAddServer.ToString()); OutLog.log(CreateHead.CSXYNUM + 1012 + returnAddServer.ToString());
                    break;
                case 11002://服务器返回登录信息
                  
                    OnCreatRoomClick(sc, openid, operation, roomid);
                 //   GameInfo.returnlogin = ReturnLogin.ParseFrom(body);
                    //Debug.Log("11002:" + GameInfo.returnlogin.Loginstat);
                    //OutLog.log("11002:" + GameInfo.returnlogin.Loginstat);
                    break;
                case 12002://服务器返回房间信息
                //  GameInfo.returnCreatRoom = ReturnCreateRoom.ParseFrom(body);
                  //  Debug.Log("12002:" + "返回房间信息");// + GameInfo.returnCreatRoom.ToString());
                 //   OutLog.log("12002:");// + GameInfo.returnCreatRoom.ToString());
                    break;
                case 12004://服务器返回加入信息
                  //  GameInfo.returnAddRoom = ReturnAddRoom.ParseFrom(body);
                    //Debug.Log("12004:" + GameInfo.returnAddRoom.ToString());
                    //  OutLog.log("12004:" + GameInfo.returnAddRoom.ToString());
                    break;
                case 12005://服务器主动推送加入玩家信息
                    //var returnUserInfo = ReturnUserInfo.ParseFrom(body);
                    //GameInfo.returnUserInfo = returnUserInfo;
                    //Debug.Log("12005:" + "服务端主动推送加入玩家信息" + returnUserInfo.ToString()); OutLog.log("12005:" + returnUserInfo.ToString());
                    break;
                case 12006://服务器返回主动推送游戏开始信息
                    //var returnStartGame = ReturnStartGame.ParseFrom(body);
                    //GameInfo.returnStartGame = returnStartGame;
                    //Debug.Log("12006:" + "服务端推送游戏开始信息" + returnStartGame.ToString()); OutLog.log("12006:" + returnStartGame.ToString());
                    break;
                case 12008://服务器返回开始游戏
                    //var returnStart = ReturnStart.ParseFrom(body);
                    //GameInfo.returnStart = returnStart;
                    //Debug.Log("12008:" + "开始游戏" + returnStart.ToString()); OutLog.log("12008:" + returnStart.ToString());
                    break;
                case 12010://服务器返回开始游戏
                    //var returnTTorTH = ReturnTTATH.ParseFrom(body);
                    //GameInfo.returnTTOrTH = returnTTorTH;
                    //Debug.Log("12010:" + "返回打出叫牌的集合" + returnTTorTH.ToString()); OutLog.log("12010:" + returnTTorTH.ToString());
                    break;

                case 13009://返回出牌信息，出牌方，出牌的花色
                    //var returnMsg = ReturnMsg.ParseFrom(body);
                    //GameInfo.returnMsg = returnMsg;
                    //Debug.Log("13009:" + "下发打出牌的消息" + returnMsg.ToString()); OutLog.log("13009:" + returnMsg.ToString());
                    break;
                case 13008://返回碰杠胡摸 四个信息和 方位
                    //var returnAll = ReturnAll.ParseFrom(body);
                    //GameInfo.returnAll = returnAll;
                    //Debug.Log("13008:" + "下发碰杠胡消息" + returnAll.ToString()); OutLog.log("13008:" + returnAll.ToString());
                    break;
                case CreateHead.CSXYNUM + 3003://返回摸牌
                    //var returnMP = ReturnMP.ParseFrom(body);
                    //GameInfo.returnMP = returnMP;
                    //Debug.Log(CreateHead.CSXYNUM + 3003 + "返回摸牌 " + returnMP.ToString()); OutLog.log(CreateHead.CSXYNUM + 3003 + returnMP.ToString());
                    break;
                case CreateHead.CSXYNUM + 4002://返回下发碰杠胡消息
                    //var returnPeng = ReturnPeng.ParseFrom(body);
                    //GameInfo.returnPeng = returnPeng;
                    //Debug.Log(CreateHead.CSXYNUM + 4002 + "下发碰杠胡后消息" + returnPeng.ToString()); OutLog.log(CreateHead.CSXYNUM + 4002 + returnPeng.ToString());
                    break;

                case CreateHead.CSXYNUM + 5002://返回听牌方位
                    //var returnBTMsg = ReturnBTMsg.ParseFrom(body);
                    //GameInfo.returnBTMSG = returnBTMsg;
                    //Debug.Log(CreateHead.CSXYNUM + 5002 + "返回听牌方位" + returnBTMsg.ToString()); OutLog.log(CreateHead.CSXYNUM + 5002 + returnBTMsg.ToString());
                    break;
                case CreateHead.CSXYNUM + 5013://返回听牌方位
                    //var returnTT = ReturnTT.ParseFrom(body);
                    //GameInfo.returnTT = returnTT;
                    //Debug.Log(CreateHead.CSXYNUM + 5013 + "返回天听方位(所有人)" + returnTT.ToString()); OutLog.log(CreateHead.CSXYNUM + 5013 + returnTT.ToString());
                    break;

                case CreateHead.CSXYNUM + 6001://返回是否、
                    //var returnRecon = ReturnRecon.ParseFrom(body);
                    //GameInfo.returnRecon = returnRecon;
                    //Debug.Log(CreateHead.CSXYNUM + 6001 + "判断是否断线重连" + returnRecon.ToString()); OutLog.log(CreateHead.CSXYNUM + 6001 + returnRecon.ToString());
                    break;
                case CreateHead.CSXYNUM + 7001://返回断线前消息
                    //var returnConnData = ReturnConnData.ParseFrom(body);
                    //GameInfo.returnConnData = returnConnData;
                    //Debug.Log(CreateHead.CSXYNUM + 7001 + "返回断线前信息" + returnConnData.ToString()); OutLog.log(CreateHead.CSXYNUM + 7001 + returnConnData.ToString());
                    break;
                case CreateHead.CSXYNUM + 7002://返回重新发牌信息
                    //var returnMsgList = ReturnMsgList.ParseFrom(body);
                    //GameInfo.returnMsgList = returnMsgList;
                    //Debug.Log(CreateHead.CSXYNUM + 7002 + "返回重发信息" + returnMsgList.ToString()); OutLog.log(CreateHead.CSXYNUM + 7002 + returnMsgList.ToString());
                    break;
                case CreateHead.CSXYNUM + 7004://返回活跃用户
                    //while (true)
                    //{
                    //    if (GameInfo.returnHyUser == null)
                    //    {
                    //        var returnHyUser = ReturnHyUser.ParseFrom(body);
                    //        GameInfo.returnHyUser = returnHyUser;
                    //        Debug.Log(CreateHead.CSXYNUM + 7004 + "返回活跃用户" + returnHyUser.ToString()); OutLog.log(CreateHead.CSXYNUM + 7004 + returnHyUser.ToString());
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        Thread.Sleep(200);
                    //    }
                    //}
                    break;
                case CreateHead.CSXYNUM + 7005://返回翻鸡牌
                    //var returnFJ = ReturnFJ.ParseFrom(body);
                    //GameInfo.returnFJ = returnFJ;
                    //Debug.Log(CreateHead.CSXYNUM + 7005 + "返回翻鸡牌" + returnFJ.ToString()); OutLog.log(CreateHead.CSXYNUM + 7005 + returnFJ.ToString());
                    break;
                case CreateHead.CSXYNUM + 2009://返回打出叫牌的集合
                    //var returnTP = ReturnTP.ParseFrom(body);
                    //GameInfo.returnTP = returnTP;
                    //Debug.Log(CreateHead.CSXYNUM + 2009 + "返回打出叫牌的集合" + returnTP.ToString()); OutLog.log(CreateHead.CSXYNUM + 2009 + returnTP.ToString());
                    break;
                case CreateHead.CSXYNUM + 7009://返回结算信息(全体)
                    //var returnJS = ReturnJS.ParseFrom(body);
                    //GameInfo.returnJS = returnJS;
                    //Debug.Log(CreateHead.CSXYNUM + 7009 + "返回结算信息(全体)" + returnJS.ToString()); OutLog.log(CreateHead.CSXYNUM + 7009 + returnJS.ToString());
                    break;
                case CreateHead.CSXYNUM + 5004://返回请求解散房间信息(所有人接收，客户端自主判断是否显示)
                    //var returnJSMsg = ReturnJSMsg.ParseFrom(body);
                    //GameInfo.returnJSMsg = ReturnJSMsg.ParseFrom(body);
                    //Debug.Log(CreateHead.CSXYNUM + 5004 + "返回请求解散房间信息" + returnJSMsg.ToString()); OutLog.log(CreateHead.CSXYNUM + 5004 + returnJSMsg.ToString());
                    break;
                case CreateHead.CSXYNUM + 5006://返回单个用户同意解散房间信息
                    //var returnJSByOnew = ReturnJSByOnew.ParseFrom(body);
                    //GameInfo.returnJSByOnew = returnJSByOnew;
                    //Debug.Log(CreateHead.CSXYNUM + 5006 + "返回单个用户同意信息" + returnJSByOnew.ToString()); OutLog.log(CreateHead.CSXYNUM + 5006 + returnJSByOnew.ToString());
                    break;
                case CreateHead.CSXYNUM + 5007://返回集体消息是否解散房间
                    //var returnALLIdea = ReturnAllIdea.ParseFrom(body);
                    //GameInfo.returnALLIdea = returnALLIdea;
                    //Debug.Log(CreateHead.CSXYNUM + 5007 + "返回集体消息是否解散房间" + returnALLIdea.ToString()); OutLog.log(CreateHead.CSXYNUM + 5007 + returnALLIdea.ToString());
                    break;
                case CreateHead.CSXYNUM + 5009://服务端返回退出玩家 
                    //var returnRemoveUser = ReturnRemoveUser.ParseFrom(body);
                    //GameInfo.returnRemoveUser = returnRemoveUser;
                    //Debug.Log(CreateHead.CSXYNUM + 5009 + "服务端返回退出玩家" + returnRemoveUser.ToString()); OutLog.log(CreateHead.CSXYNUM + 5009 + returnRemoveUser.ToString());
                    break;
                case CreateHead.CSXYNUM + 7003://返回房间信息
                    //var returnRoomMsg = ReturnRoomMsg.ParseFrom(body);
                    //GameInfo.returnRoomMsg = returnRoomMsg;
                    //Debug.Log(CreateHead.CSXYNUM + 7003 + "返回房间信息" + returnRoomMsg.ToString()); OutLog.log(CreateHead.CSXYNUM + 7003 + returnRoomMsg.ToString());
                    break;
                case CreateHead.CSXYNUM + 7008://玩家手牌集合
                    //var returnUserSPai = ReturnUserSPai.ParseFrom(body);
                    //GameInfo.returnUserSPai = returnUserSPai;
                    //Debug.Log(CreateHead.CSXYNUM + 7008 + "玩家手牌集合" + returnUserSPai.ToString()); OutLog.log(CreateHead.CSXYNUM + 7008 + returnUserSPai.ToString());
                    break;
                case CreateHead.CSXYNUM + 5012://服务器下发定缺信息
                    //var returnAYM = ReturnAYM.ParseFrom(body);
                    //GameInfo.returnAYM = returnAYM;
                    //Debug.Log(CreateHead.CSXYNUM + 5012 + "返回缺牌信息" + returnAYM.ToString()); OutLog.log(CreateHead.CSXYNUM + 5012 + returnAYM.ToString());
                    break;
                case CreateHead.CSXYNUM + 5015://返回胡牌（替换原3008的胡
                    //var returnHByType = ReturnHByType.ParseFrom(body);
                    //GameInfo.returnHByType = returnHByType;
                    //Debug.Log(CreateHead.CSXYNUM + 5015 + "返回胡牌" + returnHByType.ToString()); OutLog.log(CreateHead.CSXYNUM + 5015 + returnHByType.ToString());
                    break;
                case CreateHead.CSXYNUM + 7006://返回剩余牌堆数目
                    //var returnPaiCount = ReturnPaiCount.ParseFrom(body);
                    //GameInfo.returnPaiCount = returnPaiCount;
                    //Debug.Log(CreateHead.CSXYNUM + 7006 + "返回剩余牌堆数" + returnPaiCount.ToString()); OutLog.log(CreateHead.CSXYNUM + 7006 + returnPaiCount.ToString());
                    break;
                case CreateHead.CSXYNUM + 1://心跳
                   //GameInfo.UserHearbeat = 0;
                    break;
                case CreateHead.CSXYNUM + 5014:
                    //var returnDJS = ReturnDJS.ParseFrom(body);
                    //GameInfo.returnDJS = returnDJS;
                    //Debug.Log(CreateHead.CSXYNUM + 5014 + "大结算信息" + returnDJS.ToString()); OutLog.log(CreateHead.CSXYNUM + 5014 + returnDJS.ToString());
                    break;
                case CreateHead.CSXYNUM + 5018:
                    //var returnZR = ReturnZR.ParseFrom(body);
                    //GameInfo.returnZR = returnZR;
                    //Debug.Log(CreateHead.CSXYNUM + 5018 + "下发责任鸡消息" + returnZR.ToString()); OutLog.log(CreateHead.CSXYNUM + 5018 + returnZR.ToString());
                    break;
                case CreateHead.CSXYNUM + 5020:
                    //var returnZhuang = ReturnZhuang.ParseFrom(body);
                    //GameInfo.returnZhuang = returnZhuang;
                    //Debug.Log(CreateHead.CSXYNUM + 5020 + "返回庄" + returnZhuang.ToString()); OutLog.log(CreateHead.CSXYNUM + 5020 + returnZhuang.ToString());
                    break;
                case CreateHead.CSXYNUM + 5021:
                    //var returnHType = ReturnHType.ParseFrom(body);
                    //GameInfo.returnHType = returnHType;
                    //Debug.Log(CreateHead.CSXYNUM + 5021 + "返回胡信息" + returnHType.ToString()); OutLog.log(CreateHead.CSXYNUM + 5021 + returnHType.ToString());
                    break;
                case CreateHead.CSXYNUM + 5022:
                    //var returnGang = ReturnGang.ParseFrom(body);
                    //GameInfo.returnGang = returnGang;
                    //Debug.Log(CreateHead.CSXYNUM + 5022 + "返回杠消息" + returnGang.ToString()); OutLog.log(CreateHead.CSXYNUM + 5022 + returnGang.ToString());
                    break;
                case CreateHead.CSXYNUM + 7090:
                    //var returnRoomAdd = ReturnRoomAdd.ParseFrom(body);
                    //GameInfo.returnRoomAdd = returnRoomAdd;
                    //Debug.Log(CreateHead.CSXYNUM + 7090 + "返回加入房间状态" + returnRoomAdd.ToString()); OutLog.log(CreateHead.CSXYNUM + 7090 + returnRoomAdd.ToString());
                    break;
                case CreateHead.CSXYNUM + 7091:
                    //var returnDis = ReturnDis.ParseFrom(body);
                    //GameInfo.returnDis = returnDis;
                    //Debug.Log(CreateHead.CSXYNUM + 7091 + "返回用户距离" + returnDis.ToString()); OutLog.log(CreateHead.CSXYNUM + 7091 + returnDis.ToString());
                    break;
                case CreateHead.CSXYNUM + 7092:
                 //   QueRenKaishi(sc, openid);
                    //var returnIsJ = ReturnIsJ.ParseFrom(body);
                    //GameInfo.returnIsJ = returnIsJ;
                    //Debug.Log(CreateHead.CSXYNUM + 7092 + "返回是否有相近用户" + returnIsJ.ToString()); OutLog.log(CreateHead.CSXYNUM + 7092 + returnIsJ.ToString());
                    break;
                case CreateHead.CSXYNUM + 7093:
                    //var returnIPSame = ReturnIPSame.ParseFrom(body);
                    //GameInfo.returnIPSame = returnIPSame;
                    //Debug.Log(CreateHead.CSXYNUM + 7093 + "返回是否有相同IP" + returnIPSame.ToString()); OutLog.log(CreateHead.CSXYNUM + 7093 + returnIPSame.ToString());
                    break;
             
                case CreateHead.CSXYNUM + 8001:
                    //var returnAnnouncement = ReturnAnnouncement.ParseFrom(body);
                    //GameInfo.returnAnnouncement = returnAnnouncement;
                    //Debug.Log(CreateHead.CSXYNUM + 8001 + "返回公告信息" + returnAnnouncement.ToString()); OutLog.log(CreateHead.CSXYNUM + 8001 + returnAnnouncement.ToString());
                    break;

                case CreateHead.CSXYNUM + 8003:
                    //var returnVoice = ReturnVoice.ParseFrom(body);
                    //GameInfo.returnVoice = returnVoice;
                    //Debug.Log(CreateHead.CSXYNUM + 8003 + "返回语音信息" + returnVoice.ToString()); OutLog.log(CreateHead.CSXYNUM + 8003 + returnVoice.ToString());
                    break;
                case CreateHead.CSXYNUM + 8004:
                    //var returnConnectionStatus = ReturnConnectionStatus.ParseFrom(body);
                    //GameInfo.returnConnectionStatus = returnConnectionStatus;
                    //Debug.Log(CreateHead.CSXYNUM + 8004 + "返回连接状态" + returnConnectionStatus.ToString()); OutLog.log(CreateHead.CSXYNUM + 8004 + returnConnectionStatus.ToString());
                    break;
                case CreateHead.CSXYNUM + +9002:
                    //var returnUserRecord = ReturnUserRecord.ParseFrom(body);
                    //GameInfo.returnUserRecord = returnUserRecord;
                    //Debug.Log(CreateHead.CSXYNUM + 9002 + "返回用户战绩列表" + returnUserRecord.ToString()); OutLog.log(CreateHead.CSXYNUM + 9002 + returnUserRecord.ToString());
                    break;
                case CreateHead.CSXYNUM + +9004:
                    //var returnGetUserGamePlayback = ReturnGetUserGamePlayback.ParseFrom(body);
                    //GameInfo.returnGetUserGamePlayback = returnGetUserGamePlayback;
                    //Debug.Log(CreateHead.CSXYNUM + 9004 + "根据牌桌信息ID返回多局牌集合" + returnGetUserGamePlayback.ToString()); OutLog.log(CreateHead.CSXYNUM + 9004 + returnGetUserGamePlayback.ToString());
                    break;
                case CreateHead.CSXYNUM + +7095:
                    //var returnIsJ5Seconds = ReturnIsJ5Seconds.ParseFrom(body);
                    //GameInfo.returnIsJ5Seconds = returnIsJ5Seconds;
                    //Debug.Log(CreateHead.CSXYNUM + 7095 + "每五秒返回是否有相近用户" + returnIsJ5Seconds.ToString()); OutLog.log(CreateHead.CSXYNUM + 7095 + returnIsJ5Seconds.ToString());
                    break;
                case CreateHead.CSXYNUM + +7096:
                    //var returnIPSame5Seconds = ReturnIPSame5Seconds.ParseFrom(body);
                    //GameInfo.returnIPSame5Seconds = returnIPSame5Seconds;
                    //Debug.Log(CreateHead.CSXYNUM + 7096 + "每五秒返回是否y有相同IP" + returnIPSame5Seconds.ToString()); OutLog.log(CreateHead.CSXYNUM + 7096 + returnIPSame5Seconds.ToString());
                    break;
                case CreateHead.CSXYNUM + +7097:
                    //var returnCloseGPS = ReturnCloseGPS.ParseFrom(body);
                    //GameInfo.returnCloseGPS = returnCloseGPS;
                    //Debug.Log(CreateHead.CSXYNUM + 7097 + "返回是否开启GPS" + returnCloseGPS.ToString()); OutLog.log(CreateHead.CSXYNUM + 7097 + returnCloseGPS.ToString());
                    break;
            }

        }

        private void CreatNewSocket(ReturnServerIP returnServerIP, Socket sc,string openID)
        {
            sc.Close();
            IPAddress ipaddress;
            EndPoint point;
             sc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipaddress = IPAddress.Parse(returnServerIP.Ip);
            point = new IPEndPoint(ipaddress, int.Parse(returnServerIP.Port));
            sc.Connect(point);
            SendAddServer sendAddServer = SendAddServer.CreateBuilder()
         .SetOpenid(openID)
         .SetUnionid(openID)
         .Build();
            byte[] body = sendAddServer.ToByteArray();
            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1011, body.Length, 0, body);
            sc.Send(data);
        }

        /// <summary>
        /// 发送创建房间信息
        /// </summary>
        /// <param name="openid">玩家账号</param>
        /// <param name="is_wgj">是否乌骨鸡(1:是,0:否)</param>
        /// <param name="is_xinqiji">是否星期鸡(1:是,0:否)</param>
        /// <param name="is_shangxiaji">是否上下鸡(1:是,0:否)</param>
        /// <param name="is_benji">是否本鸡(1:是,0:否)</param>
        /// <param name="is_yikousan">是否一扣3(1:是,0:否，否则默认一扣2)</param>
        /// <param name="room_peo">房间人数(2厅：2T，3厅:3T)可选，不传为默认4人局</param>
        /// <param name="count">房间局数</param>
        public void SendCreatRoomMessage(Socket sc, string openid, int is_wgj, int is_xinqiji, int is_shangxiaji, int is_benji, int is_yikousan, int room_peo, int count, string latitude, int isYuanQuan, int isSMCP, int is_lianzhuang = 3)
        {
            SendCreateRoom creatRoom = SendCreateRoom.CreateBuilder()
                                        .SetOpenid(openid)
                                        .SetIsWgj(is_wgj)
                                        .SetIsXinqiji(is_xinqiji)
                                        .SetIsShangxiaji(is_shangxiaji)
                                        .SetIsBenji(is_benji)
                                        .SetIsYikousan(is_yikousan)
                                        .SetRoomPeo(room_peo)
                                        .SetCount(count).SetIsLianzhuang(is_lianzhuang)
                                        .SetLatitude(latitude)
                                        .SetIsYuanque(isYuanQuan)
                                        .SetQuickCard(isSMCP)
                                        .Build();
            byte[] body = creatRoom.ToByteArray();
            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 2001, body.Length, 0, body);
            //Debug.Log("2001" + "创建房间协议" + creatRoom);

            //Connect();
            sc.Send(data);
            ReceiveMessage(sc, openid);
        }

        /// <summary>
        ///  发送创建房间信息
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="openid"></param>
        /// <param name="operation">操作//1创建房间，2加入房间</param>
        /// <param name="room_id"></param>
        public void OnCreatRoomClick(Socket sc, string openid,int operation, int room_id )
        {

          
         
            SendGameOperation sendGameOperation = SendGameOperation.CreateBuilder()
                .SetOpenid(openid)
                .SetUnionid(openid)
                .SetOperation(operation)
                .SetRoomID(room_id.ToString())
                .Build();
            byte[] body = sendGameOperation.ToByteArray();
            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1020, body.Length, 0, body);
            sc.Send(data);
            ReceiveMessage(sc, openid);
        }
    }
}
