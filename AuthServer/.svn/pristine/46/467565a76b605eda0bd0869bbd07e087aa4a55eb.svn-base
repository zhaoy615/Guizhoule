using MJBLL.common;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.Logic
{
    public class GetCertStrLogic : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "11001";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var loginStr = GetCertStr.ParseFrom(requestInfo.Body);
            var data = ReturnCertStr.CreateBuilder().SetCertStr(DESEncrypt.Encrypt(loginStr.LoginInfoStr)).Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(11002, data.Length, 100001, data)));
        }
    }
}
