using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using MJBLL.common;
using MJBLL.Logic;
using MJBLL.model;
using DAL.DAL;

namespace MJBLL.mjrule
{
    /// <summary>
    /// 加入房间
    /// </summary>
    public class AddRoom : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "12003"; }

        }
        ReturnStartGame.Builder sendmj = ReturnStartGame.CreateBuilder();
        ReturnGang.Builder ReturnGangMSG = ReturnGang.CreateBuilder();
        List<model.ServerMaJiang> Ruturnjsmj = new List<model.ServerMaJiang>();


        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {


            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("AddRoom : 非法连接");
                session.Close();
                return;
            }
            ///定义返回状态码
            string staut = "10000";
            var nowsend = ReturnAddRoom.CreateBuilder();
            List<Userinfo> listuser = new List<Userinfo>();
            ///当前返回信息
            var getdata = SendAddRoom.ParseFrom(requestInfo.Body);
            ///根据上传信息查找房间
            Room rm = Gongyong.roomlist.Find(u => u.RoomID == getdata.RoomID);
            var roommsg = ReturnRoomMsg.CreateBuilder();
            //储存IP
            List<UserInfo> ListUserIP = new List<UserInfo>();

            if (rm != null)
            {

                List<mjuser> mulist = Gongyong.mulist.FindAll(u => u.RoomID == getdata.RoomID);
                mjuser mjuser = Gongyong.mulist.Find(u => u.RoomID == getdata.RoomID && u.Openid == getdata.Openid);

                if (mulist.Count == rm.room_peo)
                {
                    staut = "10002";
                }
                else
                {
                    List<int> shuzhu = new List<int>() { 1, 2, 3, 4 };
                    ///移除以坐的方位,并添加以有玩家信息
                    var senddata = ReturnUserInfo.CreateBuilder();

                    //for(int i=0;i< mulist.Count();i++)
                    foreach (mjuser item in mulist)
                    {

                        UserInfo gamer = Gongyong.userlist.Find(u => u.openid == item.Openid);
                        Userinfo users = Userinfo.CreateBuilder().SetHeadimg(gamer.headimg).SetNickname(gamer.nickname).SetOpenid(gamer.openid).SetSex(int.Parse(gamer.sex)).SetUserFW(item.ZS_Fw).SetUserBean(0).SetUserGold(0).SetUserID(gamer.UserID).SetUserIP(gamer.UserIP).Build();
                        listuser.Add(users);
                        shuzhu.Remove(item.ZS_Fw);
                        senddata.AddUserinfo(users);
                        nowsend.AddUserinfo(users);
                    }
                    if (mjuser != null)
                    {
                        return;
                    }
                    else
                    {
                        ///添加新加入玩家信息
                        mjuser mj = new mjuser()
                        {

                            Openid = getdata.Openid,
                            ZS_Fw = shuzhu[0],
                            RoomID = getdata.RoomID,
                            ConfirmationStarts = true,
                            SendData = new List<ArraySegment<byte>>(),
                            IsGuoHu = false
                        };
                        Gongyong.mulist.Add(mj);
                    }


                    ///将当前的用户信息添加到下发文件包
                    UserInfo gameuser = Gongyong.userlist.Find(u => u.openid == getdata.Openid);
                    gameuser.Lat = getdata.Latitude;
                    Userinfo newuser = Userinfo.CreateBuilder().SetHeadimg(gameuser.headimg).SetNickname(gameuser.nickname).SetOpenid(gameuser.openid).SetSex(int.Parse(gameuser.sex)).SetUserFW(shuzhu[0]).SetUserGold(0).SetUserBean(0).SetUserID(gameuser.UserID).SetUserIP(gameuser.UserIP).Build();
                    listuser.Add(newuser);
                    senddata.AddUserinfo(newuser);
                    nowsend.AddUserinfo(newuser);
                    List<mjuser> mu = Gongyong.mulist.FindAll(u => u.RoomID == getdata.RoomID);
                    var dis = ReturnDis.CreateBuilder();
                    var closeGPS = ReturnCloseGPS.CreateBuilder();

                    ///向已有玩家下发信息
                    foreach (mjuser items in mu)
                    {
                        UserInfo gamersend = Gongyong.userlist.Find(u => u.openid == items.Openid);
                        if (gamersend.Lat.Equals("0,0") || string.IsNullOrEmpty(gamersend.Lat))
                        {
                            //Console.WriteLine("AddRoom : " + gamersend.session.Config.Ip + " lat 为 0");
                            //Console.WriteLine(" : " + mjuser.)
                            closeGPS.AddFW(items.ZS_Fw);
                            //gamersend.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7091, dataGPS.Length, requestInfo.MessageNum, dataGPS)));
                        }
                        else if (gameuser.Lat.Equals("0,0") || string.IsNullOrEmpty(gameuser.Lat))
                        {
                            if (items.Openid.Equals(gameuser.openid))
                            {
                                //Console.WriteLine("AddRoom : " + gamersend.session.Config.Ip + " lat 为 0");
                                if (!closeGPS.FWList.Any(w => w == items.ZS_Fw))
                                    closeGPS.AddFW(items.ZS_Fw);
                            }
                        }
                        else
                        {
                            var userdis = ReturnDis.CreateBuilder();


                            //double jl = Erth.GetDistance(gamersend.Lat, gameuser.Lat);
                            double jl = 1;  //  测试修改
                            if (shuzhu[0] != items.ZS_Fw)
                            {
                                //if (jl < GameInformationBase.DISTANCE)
                                if (jl < 0.001f) //任光粤测试修改
                                {
                                    Console.WriteLine("距离为 ： " + jl + gamersend.nickname+":" + gamersend.Lat + " , " + gameuser.nickname + " : " + gameuser.Lat);
                                    rm.Is_Jin = true;

                                    rm.Juser1 = shuzhu[0];
                                    rm.Juser2 = items.ZS_Fw;

                                }
                            }
                            dis.SetDis(jl.ToString());
                            dis.SetFW(items.ZS_Fw);
                            userdis.SetFW(shuzhu[0]);
                            userdis.SetDis(jl.ToString());
                            ListUserIP.Add(gamersend);


                            byte[] dataMJ = userdis.Build().ToByteArray();
                            byte[] datauser = dis.Build().ToByteArray();
                            AppServer userserver = new AppServer();
                            gamersend.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7091, dataMJ.Length, requestInfo.MessageNum, dataMJ)));
                            gameuser.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7091, datauser.Length, requestInfo.MessageNum, datauser)));
                        }
                        System.Threading.Thread.Sleep(100);
                        byte[] data = senddata.Build().ToByteArray();
                        gamersend.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2005, data.Length, requestInfo.MessageNum, data)));
                        ///如果人数满足，下发游戏开始信息
                        if (mu.Count == rm.room_peo)
                        {
                            //牌局回放保存用户信息
                            rm.gameOperationProcess.SetUserInfo(senddata);
                            foreach (var item in Gongyong.mulist.FindAll(u => u.RoomID == rm.RoomID))
                            {

                                if (!item.ConfirmationStarts)
                                    rm.startgame = 2;

                            }
                            rm.startgame = rm.startgame == 2 ? 0 : 1;

                            //ReturnStart startgame = ReturnStart.CreateBuilder().SetStart(1).Build();
                            //byte[] bstart = startgame.ToByteArray();
                            //gamersend.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2008, bstart.Length, requestInfo.MessageNum, bstart)));
                        }
                    }

                    if (closeGPS.FWCount > 0)
                    {
                        foreach (mjuser items in mu)
                        {
                            var dataGPS = closeGPS.Build().ToByteArray();
                            UserInfo gamersend = Gongyong.userlist.Find(u => u.openid == items.Openid);
                            gamersend.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7097, dataGPS.Length, requestInfo.MessageNum, dataGPS)));
                        }
                    }
                }
            }
            else
            {
                staut = "10001";
            }
            ///判断房间不为空 发生房间信息
            if (rm != null)
            {
                roommsg.SetCount(rm.count)
                .SetIsBenji(rm.is_benji == true ? 1 : 0)
                .SetIsShangxiaji(rm.is_shangxiaji == true ? 1 : 0)
                .SetIsWgj(rm.is_wgj == true ? 1 : 0)
                .SetIsXinqiji(rm.is_xinqiji == true ? 1 : 0)
                .SetIsYikousan(rm.is_yikousan == true ? 1 : 0)
                .SetRoomPeo(rm.room_peo)
                .SetIsLianzhuang(rm.is_lianz == true ? 1 : 0)
                .SetIsYuanque(rm.IsYuanQue ? 1 : 0)
                .SetQuickCard(rm.QuickCard?1:0);
                byte[] roommsgb = roommsg.Build().ToByteArray();


                session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7003, roommsgb.Length, requestInfo.MessageNum, roommsgb)));

            }



            System.Threading.Thread.Sleep(100);
            nowsend.SetState(int.Parse(staut));

            byte[] datanew = nowsend.Build().ToByteArray();
            session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2004, datanew.Length, requestInfo.MessageNum, datanew)));

            //如果有玩家距离过近，提醒牌桌所有用户，并接受确认
            if (rm.Is_Jin)
            {
                byte[] jusers = ReturnIsJ.CreateBuilder().SetFWO(rm.Juser1).SetFWW(rm.Juser2).Build().ToByteArray();
                rm.startgame = 0;

                SendRoomAllUser(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7092, jusers.Length, requestInfo.MessageNum, jusers)), getdata.RoomID);
            }
            List<UserInfo> waringList = new List<UserInfo>();
            //如果有玩家IP一样
            foreach (var item in ListUserIP)
            {
                if (ListUserIP.Where(w => w.UserIP.Equals(item.UserIP)).Count() > 1)
                    waringList.Add(item);
            }
            if (waringList.Count > 1)
            {
                var ipSame = ReturnIPSame.CreateBuilder();
                foreach (var item in ListUserIP)
                {
                    var userInfo = Gongyong.mulist.First(w => w.Openid.Equals(item.openid));
                    if (userInfo != null)
                        ipSame.AddFW(userInfo.ZS_Fw);
                }
                var jusers = ipSame.Build().ToByteArray();
                rm.startgame = 0;
                SendRoomAllUser(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7093, jusers.Length, requestInfo.MessageNum, jusers)), getdata.RoomID);
            }
            rm.gameOperationProcess.SetMessage(roommsg);//牌局回放保存房间信息
            session.Logger.Debug("用户请求加入房间:" + getdata.RoomID + ",openid:" + getdata.Openid + "----------" + DateTime.Now);
            if (rm.startgame == 1)
            {

                //将牌桌信息保存
                RedisUtility.Set<RedisGameRoomInfo>(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, rm.RoomID.ToString(), ""),
                               new RedisGameRoomInfo { room = rm, ServerName = GameInformationBase.serverName });
                foreach (var item in Gongyong.mulist.FindAll(u => u.RoomID == rm.RoomID))
                {
                    if (!item.ConfirmationStarts)
                        return;
                }
                session.Logger.Debug("发牌");
                ///查找当前玩家集合
                List<mjuser> mjList = Gongyong.mulist.FindAll(u => u.RoomID == rm.RoomID);

                CardsLogic logic = new CardsLogic();
                int number = 0;//发牌次数，从0开始计数

                //根据
                ///获取房主信息
                ///


                string homeownerOpenid = "";
                foreach (var item in mjList)
                {
                    if (item.IsHomeowner)
                    {
                        //如果是房主，记录该房主的openid
                        homeownerOpenid = item.Openid;
                    }
                }

             
                var roominfodal = new RoomInfoDAL();
                var createuserid = roominfodal.GetCreateUserIdByRoomId(rm.RoomID);
                //var groupid = roominfodal.GetGroupInfoByGroupID(createuserid);
                var groupid = rm.GroupID;

                //不是圈子用户，直接进行扣费
                if (groupid == 0)
                {
                    RoomCardUtility.ReduceRoomCard(createuserid, (rm.count / 4) * 1);
                }
                else
                {
                    GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
                    var creategroupuderid = groupInfoDAL.GetUserIDByGuoupID(groupid);
                    var reducecount = (rm.count / 8) * 1;
                    RoomCardUtility.ReduceRoomCard(creategroupuderid, reducecount);

                    ////向日志里面添加朋友圈耗卡信息
                    groupInfoDAL.AddCreateRoomRecord(creategroupuderid, groupid, rm.RoomID, reducecount);
                    
                }

                var dcount = rm.Dcount;
                foreach (var item in mjList)
                {
                    ReturnStartGame.Builder sendmj = ReturnStartGame.CreateBuilder();
                    ReturnGang.Builder ReturnGangMSG = ReturnGang.CreateBuilder();
                    List<model.ServerMaJiang> Ruturnjsmj = new List<model.ServerMaJiang>();
                    item.paixinfs = 5;
                    Gongyong.mulist.Remove(item);
                    if (rm.IsYuanQue)
                        item.QYM = 3;
                    logic.GetMyCards(requestInfo, sendmj, ReturnGangMSG, Ruturnjsmj, session, rm.RoomID, item, item.Openid, ref dcount);

                }

            }

        }

        private void SendRoomAllUser(ArraySegment<byte> data, int roomID)
        {
            foreach (var item in Gongyong.mulist.FindAll(u => u.RoomID == roomID))
            {
                UserInfo userSendJ = Gongyong.userlist.Find(u => u.openid == item.Openid);
                item.ConfirmationStarts = false;
                userSendJ.session.Send(data);

            }
        }
    }
}
