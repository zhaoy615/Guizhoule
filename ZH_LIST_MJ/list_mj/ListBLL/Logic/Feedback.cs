using DAL.DAL;
using DAL.Model;
using ListBLL.common;
using ListBLL.model;
using MJBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.Logic
{
    public class Feedback : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "19005";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            //if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            //{
            //    session.Logger.Debug("非法连接");
            //    session.Close();
            //    return;
            //}
            var sendInfo = SendFeedback.ParseFrom(requestInfo.Body);
            RedisLoginModel olduser = RedisUtility.Get<RedisLoginModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, sendInfo.Openid, sendInfo.Unionid));
            if (olduser == null)
            {
                session.Close();
                return;
            }
         
            Feedback_log fb = new Feedback_log();
            fb.UserID = sendInfo.UserID;
            fb.Title = sendInfo.Title;
            if (sendInfo.HasContent)
                fb.Content = sendInfo.Content;
            fb.Datetime = DateTime.Now;
            if (sendInfo.HasGameLog)
                fb.GameLog = sendInfo.GameLog;
            if (sendInfo.HasImage)
                fb.image = sendInfo.Image;
            new FeedbackDAL().Add(fb);
        }
    }
}
