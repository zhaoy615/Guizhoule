using ListBLL.common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;

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
            Console.WriteLine("List服务器启动ip : " + config.Ip + " ，端口号 ：" + config.Port);
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
