using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using MJBLL.common;
using MJBLL.logic;
using MJBLL.model;

namespace MJBLL.mjrule
{
    /// <summary>
    /// 解散房间
    /// </summary>
    public class RemoveRoom : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "15003"; }
        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("RemoveRoom : 非法连接");
                session.Close();
                return;
            }
            var getdata = SendJS.ParseFrom(requestInfo.Body);
            mjuser rst = Gongyong.mulist.Find(u => u.Openid == getdata.Openid);
            int roomid = getdata.Roomid;
            Room r = Gongyong.roomlist.Find(u => u.RoomID == roomid);
            if (rst == null)
            {
                if (r != null)
                {
                    if (!Gongyong.mulist.Any(w=>w.RoomID== roomid))
                    {
                        Gongyong.roomlist.Remove(r);
                        RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, r.RoomID.ToString(), ""));
                    }
                }
                RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, getdata.Openid, getdata.Unionid));
                return;
            }
            if (r == null)
                return;

            int fw = Gongyong.mulist.Find(u => u.Openid == getdata.Openid).ZS_Fw;
            session.Logger.Debug(Name + "|" + getdata.ToString());
            string nickname = Gongyong.userlist.Find(u => u.openid == getdata.Openid).nickname;
            var disbandedRoomInfo = RedisUtility.Get<DisbandedRoomInfo>((RedisUtility.GetKey(GameInformationBase.COMMUNITYDISBANDED, getdata.Openid, getdata.Unionid)));
            List <mjuser> listuser = Gongyong.mulist.FindAll(u => u.RoomID == roomid);
            ///判断游戏是否开始（未开始直接解散，开始则发送消息）
            if (r.startgame != 1 && rst.IsHomeowner)
            {
                new UserExitLogic().DissolutionRoom(r, requestInfo.MessageNum, 1);
            }
            else if(disbandedRoomInfo != null&&( DateTime.Now- disbandedRoomInfo.Time).TotalSeconds<5)//
            {
              var data=  ReturnDisbandedFailure.CreateBuilder().SetStatus(1).Build().ToByteArray();
                session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5010, data.Length, requestInfo.MessageNum, data)));
            }
            else
            {
                if (disbandedRoomInfo == null)
                {
                    var disbandedRoom = new DisbandedRoomInfo { OpenID = getdata.Openid, RoomID = roomid, Time = DateTime.Now };
                    RedisUtility.Set(RedisUtility.GetKey(GameInformationBase.COMMUNITYDISBANDED, getdata.Openid, getdata.Unionid), disbandedRoom, 1);
                }
                if (!Gongyong.roomclear.Any(w => w.RoomID == roomid))
                {
                    RoomClear rc = new RoomClear()
                    {
                        Status = 1,
                        RoomID = roomid
                    };
                    var Rdata = ReturnJSMsg.CreateBuilder().SetFw(fw).SetNickName(nickname);
                    byte[] Rtbyte = Rdata.Build().ToByteArray();
                    foreach (var item in listuser)
                    {
                        if (item.Openid != getdata.Openid)
                        {
                            rc.RoomPeo.Add(item.Openid);
                        }
                        UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                        user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5004, Rtbyte.Length, requestInfo.MessageNum, Rtbyte)));

                    }

                    Gongyong.roomclear.Add(rc);
                    ThreadUtility.AddCountdownRemoveRoomThread(roomid.ToString());//当有用户申请解散房间，倒计时开始。
                }
                else
                {
                    var data = SendJSIdea.CreateBuilder().SetOpenid(getdata.Openid).SetRoomid(roomid).SetState(1).Build().ToByteArray();
                    //  new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5005, data.Length, requestInfo.MessageNum, data));
                    new ClearByUser().ExecuteCommand(session,
                          new ProtobufRequestInfo { Body = data, Key = (GameInformationBase.BASEAGREEMENTNUMBER + 5005).ToString(), Messagelength = data.Length, MessageNum = GameInformationBase.BASEAGREEMENTNUMBER + 5005, MessageResNum = 0 });
                }
                //  ThreadUtility.StartCountdownRemoveRoomThread(roomid.ToString());
            }


        }

    }
}
public class DisbandedRoomInfo
{
    public string OpenID { get; set; }
    public int RoomID { get; set; }
    public DateTime Time { get; set; }
}
