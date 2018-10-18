using ListBLL.common;
using ListBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.Logic
{
    public class GetRoomCard : ICommand<GameSession, ProtobufRequestInfo>
    {
        // public string Name => "11022";
        public string Name
        {
            get { return "11022"; }
        }
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var gameOperation = SendGetRoomCard.ParseFrom(requestInfo.Body);
            RedisLoginModel olduser = RedisUtility.Get<RedisLoginModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, gameOperation.Openid, gameOperation.Unionid));
            if (olduser == null)
                return;
            var info = Gongyong.userlist.First(w => w.openid.Equals(olduser.Openid));
            if (info == null)
                return;
            if ( info.Type == 0)
            {
                var msg = ReturnGetRoomCard.CreateBuilder().SetUserRoomCard(RoomCardUtility.GetRoomCard(gameOperation.UserID)).Build().ToByteArray();
                session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1023, msg.Length, requestInfo.MessageNum, msg)));
            }
            else
            {
                var msg = ReturnGetRoomCard.CreateBuilder().SetUserRoomCard(RoomCardUtility.GetLongBaoNumber(info.unionid)).Build().ToByteArray();
                session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1023, msg.Length, requestInfo.MessageNum, msg)));
            }
        }
    }
}
