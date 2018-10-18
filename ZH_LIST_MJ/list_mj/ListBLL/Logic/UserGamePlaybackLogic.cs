using DAL.DAL;
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
    public class UserGamePlaybackLogic : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "19003";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var RoomInfoID = SendGetUserGamePlayback.ParseFrom(requestInfo.Body);
          var list=  new BoarddetailstbDAL().GetListByRoomInfoID(RoomInfoID.RoomInfoID);
            var returnGetUserGamePlayback= ReturnGetUserGamePlayback.CreateBuilder();
            foreach (var item in list)
            {
                returnGetUserGamePlayback.AddGameOperationProcess(GameOperationProcess.ParseFrom(item.MatchDetails));
            }
            var data = returnGetUserGamePlayback.Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 9004, data.Length, requestInfo.MessageNum, data)));
        }
    }
}
