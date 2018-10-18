using MJBLL.common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.common
{
    public class GameSession : AppSession<GameSession, ProtobufRequestInfo>
    {
        protected override void OnSessionStarted()
        {
           
        }


        protected override void HandleException(Exception e)
        {
            this.Logger.Error(e);
            this.Send("Application error: {0}", e);

        }

        public IServerConfig getip()
        {
            return Config;
        }


    //    public override bool TrySend(ArraySegment<byte> segment)
    //    {
    //        bool state = false;
        
    //        try
    //        {
           
    //            state = base.TrySend(segment);
    //            if (!state)
    //            {
    //UserInfo users = Gongyong.userlist.Find(u => u.session.SessionID == SessionID);
    //              var mjUser=  Gongyong.mulist.Find(w => w.Openid.Equals(users.openid));
    //                if (mjUser != null && users != null)
    //                {
    //                    MsgLog msg = new MsgLog()
    //                    {
    //                        msg = segment,
    //                        openid = users.openid,
    //                        RoomID = mjUser.RoomID
    //                    };
    //                    Gongyong.msg.Add(msg);
    //                    users.ConnTime = DateTime.Now;
    //                }
                  
    //            }
    //            return state;

    //        }
    //        catch (Exception ex)
    //        {

    //            this.Logger.Error(ex.Message);
    //            return state;
    //        }
           

    //    }

        protected override void OnSessionClosed(CloseReason reason)
        {
            //add you logics which will be executed after the session is closed
            base.OnSessionClosed(reason);
            //if (Gongyong.userlist.Count == 0)
            //    return;
            //var info = Gongyong.userlist.FirstOrDefault(w => w.session.SessionID.Equals( this.SessionID));
            //if (info != null)
            //{

              
            //    if (Gongyong.mulist.Count == 0)
            //        return;
            //    var muInfo = Gongyong.mulist.FirstOrDefault(w => w.Openid.Equals(info.openid));
            //    if (muInfo != null)
            //    {
            //        var room = Gongyong.roomlist.FirstOrDefault(u => u.RoomID == muInfo.RoomID);
            //        if (room != null && room.startgame == 0)
            //        {
            //            RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, info.openid, info.unionid));
            //            // RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, info.openid, info.unionid));
            //            UserExitLogic exit = new UserExitLogic();
            //            //Gongyong.mulist.Remove(muInfo);
            //            if (muInfo.IsHomeowner)
            //            {
            //               Gongyong.userlist.Remove(info);
            //                exit.DissolutionRoom(room, 0, 2);
            //            }
            //            else
            //            {
            //                Gongyong.userlist.Remove(info);
            //                exit.UserExit(muInfo, room.RoomID, info.openid, 0, null, true);
            //            }
            //        }
            //    }
            //}
        }
    }
}
