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

    public class Managed : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "12012";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("Managed : 非法连接");
                session.Close();
                return;
            }
            var sendData = SendManaged.ParseFrom(requestInfo.Body);
            if (sendData.State == 1)
                ThreadUtility.Change(sendData.Openid);

        }
    }
}
