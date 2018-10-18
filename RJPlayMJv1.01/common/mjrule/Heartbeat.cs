
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
    /// <summary>
    /// 处理心跳
    /// </summary>
    public class Heartbeat : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return (GameInformationBase.BASEAGREEMENTNUMBER + 1).ToString(); }
        }
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug(" Heartbeat : 非法连接" + session.SessionID);
                session.Close();
                return;
            }

            var info = MaintainHeartbeat.ParseFrom(requestInfo.Body);
            var userInfo = Gongyong.userlist.Find(w => w.openid.Equals(info.Openid));
            if (userInfo != null)
            {
                if (info.HasLatitude)
                    userInfo.Lat = info.Latitude;
                userInfo.UserIP = session.RemoteEndPoint.Address.ToString();
                //session.Logger.Debug("定位" + userInfo.Lat+"名称"+userInfo.nickname);
            }
            session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1, requestInfo.Body.Length, requestInfo.MessageNum, requestInfo.Body)));
            Gongyong.msg.Remove(null);
            var messageList = Gongyong.msg.FindAll(w => w.openid.Equals(info.Openid));
            if (messageList.Count > 0)
            {
                while (messageList.Count > 0)
                {
                    session.TrySend(messageList[0].msg);

                    Gongyong.msg.Remove(messageList[0]);
                    messageList.RemoveAt(0);
                }
            }


            //UserInfo user = Gongyong.userlist.Find(u => u.openid == info.Openid);
            //if (user == null)
            //    return;
            //if (user.session.SessionID != session.SessionID)
            //{
            //    user.session = session;
            //}
            //List<MsgLog> list = Gongyong.msg.FindAll(u => u.openid == user.openid);
            //if (list.Count > 0)
            //{

            //    var usermsg = ReturnMsgList.CreateBuilder();
            //    foreach (var item in list)
            //    {

            //        Google.ProtocolBuffers.ByteString bytes = Google.ProtocolBuffers.ByteString.CopyFrom(item.msg.ToArray());
            //        usermsg.AddMsg(bytes);
            //        Gongyong.msg.Remove(item);
            //    }
            //    byte[] arr = usermsg.Build().ToByteArray();
            //    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7002, arr.Length, requestInfo.MessageNum, arr)));
            //}


            //}
            //catch (Exception ex)
            //{
            //    //  Logger .Error()
            //    session.Logger.Error(ex.ToString());
            //    session.Close();

            //}
        }
    }
}
