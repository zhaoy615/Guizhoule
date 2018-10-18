using ListBLL.common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace List_mj
{
    public class GameServer : AppServer<GameSession, ProtobufRequestInfo>
    {
        public GameServer()
            : base(new DefaultReceiveFilterFactory<ProtobufReceiveFilter, ProtobufRequestInfo>())
        {
        }
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }

        protected override void OnStarted()
        {
           
            base.OnStarted();

        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }
    }
}
