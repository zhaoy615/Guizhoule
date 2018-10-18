
using ListBLL.model;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.common
{
    public class GameSession : AppSession<GameSession, ProtobufRequestInfo>
    {
        protected override void OnSessionStarted()
        {

        }


        protected override void HandleException(Exception e)
        {
            this.Logger.Error(e);
            this.Send("Application error: {0}", e);

        }

        public IServerConfig getip()
        {
            return Config;
        }




        protected override void OnSessionClosed(CloseReason reason)
        {
            //add you logics which will be executed after the session is closed
            base.OnSessionClosed(reason);
            //if (Gongyong.userlist.Count == 0)
            //    return;

            var info = Gongyong.userlist.FirstOrDefault(w => w.session == this);
            if (info != null)
            {
                RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, info.openid, info.unionid));

                Gongyong.userlist.Remove(info);
                //if (Gongyong.mulist.Count > 0)
                //{
                //    var muInfo = Gongyong.mulist.First(w => w.Openid.Equals(info.openid));
                //    if (muInfo != null)
                //    {
                //        Gongyong.mulist.Remove(muInfo);
                //    }
                //}
            }
        }
    }
}
