using MJBLL.common;
using MJBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.mjrule
{
    public class RoomExist : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "19901";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            if (requestInfo.MessageResNum != 19990)
            {
                session.Close();
                session.Logger.Debug("非法用户请求 "+ session.RemoteEndPoint.AddressFamily);
                return;
            }
            var RoomExist = SendRoomExist.ParseFrom(requestInfo.Body);
           var muInfo= Gongyong.mulist.Find(w => w.Openid.Equals(RoomExist.Openid));
            if (muInfo == null)
            {
                var roomInfo = Gongyong.roomlist.Find(w => w.RoomID == RoomExist.RoomID);
                if (roomInfo != null)
                {
                    Gongyong.roomlist.Remove(roomInfo);
                }
                else
                    RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, RoomExist.RoomID.ToString(), ""));
                RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, RoomExist.Openid, RoomExist.Unionid));
                var data = ReturnRoomExist.CreateBuilder().SetIsExist(0).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 9902, data.Length, 19990, data)));
            }
            else
            {
                var data = ReturnRoomExist.CreateBuilder().SetIsExist(1).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 9902, data.Length, 19990, data)));
            }

        }
    }
}
