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
    public class UserOffline : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "18005";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
           var userConnStatus= SendConnectionStatus.ParseFrom(requestInfo.Body);
            var mjUser = Gongyong.mulist.Find(w => w.Openid.Equals(userConnStatus.Openid));
            if(mjUser!=null)
            {
                mjUser.ConnectionStatus = userConnStatus.ConnectionStatus;

            }

        }
    }
}
