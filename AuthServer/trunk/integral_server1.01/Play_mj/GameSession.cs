using MJBLL.common;
using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Play_mj
{
    public class GameSession : AppSession<GameSession, ProtobufRequestInfo>
    {
        protected override void OnSessionStarted()
        {
           
        }

       
        protected override void HandleException(Exception e)
        {
            this.Send("Application error: {0}", e.Message);
            
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            //add you logics which will be executed after the session is closed
            base.OnSessionClosed(reason);
        }
    }
}
