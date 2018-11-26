using MJBLL.common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;

namespace AuthServer
{
    public class GameServer : AppServer<GameSession, ProtobufRequestInfo>
    {

        public GameServer()
            : base(new DefaultReceiveFilterFactory<ProtobufReceiveFilter, ProtobufRequestInfo>())
        {
        }
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            Console.WriteLine("Auth服务器启动ip : " + config.Ip + " ，端口号 ：" + config.Port);
            return base.Setup(rootConfig, config);
        }

        protected override void OnStarted()
        {
            //new TimerOut().TimeOutUser();
            //Thread timer = new Thread(ConnectionStatusLogic.ChackConnection);
           // timer.Start();

          
            base.OnStarted();

        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

    }
}
