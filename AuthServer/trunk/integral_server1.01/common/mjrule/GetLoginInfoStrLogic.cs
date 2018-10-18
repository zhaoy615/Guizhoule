
using DAL.DBHelp;
using MJBLL.common;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.Logic
{
    public class GetLoginInfoStrLogic : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "11003";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var loginStr = GetLoginInfoStr.ParseFrom(requestInfo.Body);
            string loginInfoStr = string.Empty;
            try
            {
                loginInfoStr = common.DESEncrypt.Decrypt(loginStr.CertStr);

             }
            catch (Exception ex)
            {
                session.Logger.Error(ex);
            }
            var data = ReturnLoginInfoStr.CreateBuilder().SetLoginInfoStr(loginInfoStr).Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(11004, data.Length, 100003, data)));
        }
    }
}
