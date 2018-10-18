﻿
using DAL.DAL;
using MJBLL.common;
using MJBLL.Logic;
using MJBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MJBLL.mjrule
{
    /// <summary>
    /// 创建房间
    /// </summary>
    public class CreateRoom : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "12001"; }
            /// <summary>
            /// 返回房间ID
            /// </summary>
            /// <param name="session"></param>
            /// <param name="requestInfo"></param>
        }

        // public string Name => "CreateRoom";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {

            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("CreateRoom : 非法连接");
                session.Close();
                return;
            }
            session.Logger.Debug("创建sssionID--------" + session.SessionID);

            var info = SendCreateRoom.ParseFrom(requestInfo.Body);
            ///当前用户信息
            UserInfo user = Gongyong.userlist.Find(u => u.openid.Equals(info.Openid));
            if (user == null)
            {
                session.Logger.Debug("CREATEROOM user == nulll");
                session.Close();
                return;
            }
            if (Gongyong.mulist.Any(w => w.Openid.Equals(info.Openid)))
            {
                var muInfo = Gongyong.mulist.Find(w => w.Openid.Equals(info.Openid));
                if (Gongyong.roomlist.Any(w => w.RoomID == muInfo.RoomID))
                {
                    // ReturnServerIP.CreateBuilder().SetIp
                }
                else
                {
                    Gongyong.mulist.Remove(muInfo);
                }

            }
            ///更具房间信息创建房间
            int roomid = GetRoomID();
            if (info.Count != 8 && info.Count != 16 && info.Count != 32)
            {
                return;
            }
            Room room = new Room()
            {
                count = info.Count,
                is_benji = info.IsBenji == 1 ? true : false,
                is_wgj = info.IsWgj == 1 ? true : false,
                is_xinqiji = info.IsXinqiji == 1 ? true : false,
                is_yikousan = info.IsYikousan == 1 ? true : false,
                is_shangxiaji = info.IsShangxiaji == 1 ? true : false,
                room_peo = info.RoomPeo,
                Dcount = 1,
                RoomID = roomid,
                startgame = 0,
                is_lianz = info.IsYikousan == 3 ? true : false,
                CreateDate = DateTime.Now,
                IsYuanQue = info.HasRoomPeo ? (info.RoomPeo == 4 ? false : (info.HasIsYuanque ? info.IsYuanque == 1 : false)) : false,//如果是4人桌 不能是原缺。
                QuickCard = info.HasQuickCard ? (info.QuickCard == 1) : false//十秒快速出牌
                // RoomPai = new CreateMj().CreateMJ()
            };
            Console.WriteLine("roomid : " + roomid);
            try
            {
                new RoomInfoDAL().Add(CardsLogic.SetRoomInfoTb(room, user.UserID));
            }
            catch (Exception ex)
            {

                session.Logger.Error(ex);
            }
            Gongyong.roomlist.Add(room);

            user.Lat = info.Latitude;

            //将用户游戏信息更新
            RedisUtility.Set<RedisGameModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, user.openid, user.unionid),
                         new RedisGameModel { Openid = user.openid, RoomID = roomid, ServerName = GameInformationBase.serverName, Unionid = user.unionid });
            //将牌桌信息保存
            RedisUtility.Set<RedisGameRoomInfo>(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, roomid.ToString(), ""),
                        new RedisGameRoomInfo { room = room, ServerName = GameInformationBase.serverName });


            ///麻将玩家操作类
            mjuser mu = new mjuser()
            {
                ZS_Fw = 1,
                Openid = user.openid,
                RoomID = roomid,
                SendData = new List<ArraySegment<byte>>(),
                ConfirmationStarts = true,
                IsGuoHu = false
            };
            Gongyong.mulist.Add(mu);


            if (mu.ByteData == requestInfo.Key)
            {
                return;
            }
            else
            {
                mu.ByteData = requestInfo.Key;
            }
            mu.IsHomeowner = true;
            ///当前返回用户信息
            Userinfo uinfo = Userinfo.CreateBuilder().SetHeadimg(user.headimg).SetNickname(user.nickname).SetOpenid(user.openid).SetSex(int.Parse(user.sex)).SetUserFW(1).SetUserBean(0).SetUserGold(0).SetUserID(user.UserID).SetUserIP(user.UserIP).Build();
            ///返回数值
            ReturnCreateRoom cr = ReturnCreateRoom.CreateBuilder().SetRoomID(roomid).SetUserinfo(uinfo).Build();
            byte[] data = cr.ToByteArray();
            session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2002, data.Length, requestInfo.MessageNum, data)));

            var roommsg = ReturnRoomMsg.CreateBuilder();
            roommsg.SetCount(room.count).SetIsBenji(room.is_benji ? 1 : 0)
            .SetIsShangxiaji(room.is_shangxiaji ? 1 : 0)
            .SetIsWgj(room.is_wgj ? 1 : 0)
            .SetIsXinqiji(room.is_xinqiji ? 1 : 0)
            .SetIsYikousan(room.is_yikousan ? 1 : 0).SetRoomPeo(room.room_peo)
            .SetIsLianzhuang(room.is_lianz ? 1 : 0)
            .SetIsYuanque(room.IsYuanQue ? 1 : 0)
            .SetQuickCard(room.QuickCard ? 1 : 0)
            ;
            byte[] roommsgb = roommsg.Build().ToByteArray();
            session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7003, roommsgb.Length, requestInfo.MessageNum, roommsgb)));
            session.Logger.Debug("参数data" + cr + "----------" + DateTime.Now);
            session.Logger.Debug("当前用户创建房间----" + roomid + "------" + DateTime.Now);

        }


        Random ran = new Random();
        public int GetRoomID()
        {
            int roomid = 0;
            do
            {
                roomid = ran.Next(100000, 999999);
                Room rst = Gongyong.roomlist.Find(u => u.RoomID == roomid);
                RedisGameRoomInfo roominfo = RedisUtility.Get<RedisGameRoomInfo>(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, roomid.ToString(), ""));
                if (roominfo == null)
                    break;
                else if (roominfo.ServerName.Equals(GameInformationBase.serverName))
                    break;

            } while (true);


            return roomid;
        }


    }
}
