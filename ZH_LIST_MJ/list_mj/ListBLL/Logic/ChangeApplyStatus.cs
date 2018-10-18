using DAL.DAL;
using ListBLL.common;
using ListBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Linq;

namespace ListBLL.Logic
{
    public class ChangeApplyStatus : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11070"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var sendChangeApplyStatus = SendChangeApplyStatus.ParseFrom(requestInfo.Body);

            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
            byte[] resultData = null;
DAL.DAL.mjuserinfoDAL mjuserinfo = new mjuserinfoDAL();
            if (sendChangeApplyStatus.ApplyStatus == 1 || sendChangeApplyStatus.ApplyStatus == 2)
            {

                var userinfo = mjuserinfo.GetModel(sendChangeApplyStatus.UserID);
                if (userinfo != null)
                {//=========================================================   
                    if (groupInfoDAL.GetIsExistenceApplyStatus(sendChangeApplyStatus.GroupID, sendChangeApplyStatus.UserID, 0) == 1)
                    {
                        if (groupInfoDAL.AgreeApplyStatus(sendChangeApplyStatus.GroupID, sendChangeApplyStatus.UserID, sendChangeApplyStatus.ApplyStatus,0) != 0)
                        {
                            if (sendChangeApplyStatus.ApplyStatus == 1)
                            {
                                groupInfoDAL.AddUserToGroup(sendChangeApplyStatus.GroupID, sendChangeApplyStatus.UserID, 2);
                                RedisUserInfoModel user = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, userinfo.openid, userinfo.unionid));
                                if (user != null)
                                {
                                    user.GroupID.Add((int)sendChangeApplyStatus.GroupID);
                                    RedisUtility.Set(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, user.Openid, user.Unionid), user);
                                    var userInfo = Gongyong.userlist.Find(w => w.UserID == user.UserID);
                                    if (userInfo != null)
                                        userInfo.GroupID.Add((int)sendChangeApplyStatus.GroupID);

                                }
                                resultData = ReturnChangeApplyStatus.CreateBuilder().SetStatus(1).SetMessage("修改成功").Build().ToByteArray();
                                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1071, resultData.Length, requestInfo.MessageNum, resultData)));
                            }
                        }
                    }
                resultData = ReturnChangeApplyStatus.CreateBuilder().SetStatus(1).SetMessage("修改失败").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1071, resultData.Length, requestInfo.MessageNum, resultData)));
                }
            }
            else if (sendChangeApplyStatus.ApplyStatus == 5 || sendChangeApplyStatus.ApplyStatus == 6)
            {
                var userinfo = mjuserinfo.GetModel(sendChangeApplyStatus.UserID);
                if (userinfo != null)
                {
                    if (groupInfoDAL.GetIsExistenceApplyStatus(sendChangeApplyStatus.GroupID, sendChangeApplyStatus.UserID, 4) == 1)
                    {
                        if (groupInfoDAL.AgreeApplyStatus(sendChangeApplyStatus.GroupID, sendChangeApplyStatus.UserID, sendChangeApplyStatus.ApplyStatus) != 0)
                        {
                            if (sendChangeApplyStatus.ApplyStatus == 5)
                            {
                                groupInfoDAL.DelUsersByUserIDTransaction(sendChangeApplyStatus.GroupID, sendChangeApplyStatus.UserID, 5);
                                RedisUserInfoModel user = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, userinfo.openid, userinfo.unionid));
                                if (user != null)                                                                                                                                                                                      
                                {
                                    user.GroupID.Remove((int)sendChangeApplyStatus.GroupID);
                                    RedisUtility.Set(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, user.Openid, user.Unionid), user);
                                    var userInfo = Gongyong.userlist.Find(w => w.UserID == user.UserID);
                                    if (userInfo != null)
                                        userInfo.GroupID.Remove((int)sendChangeApplyStatus.GroupID);

                                }
                                resultData = ReturnChangeApplyStatus.CreateBuilder().SetStatus(1).SetMessage("修改成功").Build().ToByteArray();
                                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1071, resultData.Length, requestInfo.MessageNum, resultData)));
                            }
                        }
                    }
                }
            }
        }
    }
}
