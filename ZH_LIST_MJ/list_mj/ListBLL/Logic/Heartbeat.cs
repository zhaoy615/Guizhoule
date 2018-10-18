

using DAL.DAL;
using ListBLL.common;
using ListBLL.model;
using MJBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.Logic
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
            try
            {
                GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
                var info = MaintainHeartbeat.ParseFrom(requestInfo.Body);
                var userInfo = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, info.Openid, info.Unionid));
                if (userInfo != null)
                {
                    var olduser = RedisUtility.Get<RedisLoginModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, info.Openid, info.Unionid));
                    if (olduser == null)
                        RedisUtility.Set(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, userInfo.Openid, userInfo.Unionid), new RedisLoginModel { Openid = userInfo.Openid, ServerName = GameInformationBase.serverName, Unionid = userInfo.Unionid });
                    if (!Gongyong.userlist.Any(w => w.unionid.Equals(userInfo.Unionid)))
                    {
                        Gongyong.userlist.Add(new UserInfo
                        {
                            city = userInfo.City,
                            ConnTime = userInfo.ConnTime,
                            GroupID = userInfo.GroupID,
                            headimg = userInfo.Headimg,
                            Lat = userInfo.Lat,
                            nickname = userInfo.Nickname,
                            openid = userInfo.Openid,
                            province = userInfo.Province,
                            session = session,
                            sex = userInfo.Sex,
                            unionid = userInfo.Unionid,
                            UserID = userInfo.UserID,
                            UserIP = session.RemoteEndPoint.Address.ToString(),
                             Type=userInfo.Type
                        });
                    }
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1, requestInfo.Body.Length, requestInfo.MessageNum, requestInfo.Body)));
                    var list = groupInfoDAL.GetUpdateTipsByUserID(info.UserID);
                    if (list.Any())
                    {
                        var data = ReturnMessgaeList.CreateBuilder().AddRangeMessgaeList(list).Build().ToByteArray();
                        session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1111, data.Length, requestInfo.MessageNum, data)));
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


            }
            catch (Exception ex)
            {
               //  Logger .Error()
                session.Logger.Error(ex.ToString());
                session.Close();
           
            }
        }
    }
}
