using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using MJBLL.common;
using MJBLL.logic;
using MJBLL.model;

namespace MJBLL.mjrule
{

    /// <summary>
    /// 用户退出房间
    /// </summary>
    public class UserRemove : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "15008"; }
        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
           
                if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
                {
                    session.Logger.Debug("UserRemove : 非法连接");
                    session.Close();
                    return;
                }
                var getdata = SendRemove.ParseFrom(requestInfo.Body);
                mjuser usermj = Gongyong.mulist.Find(u => u.Openid == getdata.Openid);
                //判断是否存在该玩家
                if (usermj == null)
                {
                    return;
                }
                if (usermj.IsHomeowner)//如果是房主则不能是退出房间，而应该是解散牌桌
                {
                    session.Logger.Debug("房主请求退出");
                    return;
                }


                new UserExitLogic().UserExit(usermj, getdata.RoomID, getdata.Openid, requestInfo.MessageNum, session);
          
        
        }

     
    }
}
