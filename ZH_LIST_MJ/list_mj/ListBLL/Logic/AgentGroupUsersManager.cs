using DAL.DAL;
using ListBLL.common;
using ListBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ListBLL.Logic
{
    public class AgentGroupUsersManager : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11107"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();


            var groupUsersManager = SendAgentGroupUsersManager.ParseFrom(requestInfo.Body);
            // RedisUserInfoModel user = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO,  groupUsersManager.Openid,  groupUsersManager.Unionid));

            var player = Player.CreateBuilder();
            var returnPlayerList = ReturnPlayerList.CreateBuilder();
            var list = groupInfoDAL.GetUsersIDListByGroupID(groupUsersManager.GroupID);
            var deluserInfo=  new mjuserinfoDAL().GetModel(groupUsersManager.DelByUserID);
            byte[] returnMessage = null;
            if (deluserInfo == null)
            {
                returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("没有该用户").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                // session.Close();
                return;
            }
            var groupInfo = groupInfoDAL.GetGroupInfoByGroupID(groupUsersManager.GroupID, deluserInfo.is_band.Value);
            if (groupInfo == null)
            {
                returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("没有该用户").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
               // session.Close();
                return;
            }
            //删除用户
            if (groupUsersManager.HasDelByUserID && !groupUsersManager.HasAddUsers)
            {
               
                if (groupInfo.CreateUserID == groupUsersManager.DelByUserID)
                {
                    returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("群主不能删除自己").Build().ToByteArray();
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                    return;
                }

               // groupInfoDAL.DelUsersByUserIDTransaction( groupUsersManager.GroupID,  groupUsersManager.DelByUserID);
                groupInfoDAL.DelUsersByUserIDTransaction( groupUsersManager.GroupID,  groupUsersManager.DelByUserID, 3, groupUsersManager.DelByUserID);
                returnMessage = ReturnMessgae.CreateBuilder().SetStatue(1).SetMessage("删除成功").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                return;
            }
            //添加用户
            else if (! groupUsersManager.HasDelByUserID &&  groupUsersManager.HasAddUsers)
            {
                //var groupInfo = groupInfoDAL.GetGroupInfoByGroupID(groupUsersManager.GroupID);
                //if (groupInfo.CreateUserID == groupUsersManager.AddUsers)
                //{
                //    returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("添加失败").Build().ToByteArray();
                //    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                //    return;
                //}
                if (new DAL.DAL.mjuserinfoDAL().GetModel(groupUsersManager.AddUsers) != null)
                {
                    //加入类型，1圈主添加，2申请加入，3推荐加入
                    var err = groupInfoDAL.AddUserToGroup(groupUsersManager.GroupID, groupUsersManager.AddUsers, 1);
                    if (err == 0)
                    {
                        returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("添加失败，已存在该用户").Build().ToByteArray();
                        session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                    }
                    else
                    {
                        returnMessage = ReturnMessgae.CreateBuilder().SetStatue(1).SetMessage("添加成功").Build().ToByteArray();
                        groupInfoDAL.AddGroupTips(groupUsersManager.GroupID, groupUsersManager.AddUsers, 1);

                        //添加成狗后把用户丢到RedisUs  避免需重复登陆问题
                        RedisUserInfoModel user = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, groupUsersManager.AddUsers.ToString(), groupUsersManager.AddUsers.ToString()));
                        if (user != null)
                        {
                            user.GroupID.Add((int)groupUsersManager.GroupID);
                            RedisUtility.Set(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, user.Openid, user.Unionid), user);
                            var userInfo = Gongyong.userlist.Find(w => w.UserID == user.UserID);
                            if (userInfo != null)
                                userInfo.GroupID.Add((int)groupUsersManager.GroupID);

                        }

                        session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));

                       
                    }
                }
                else
                {
                    returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("无此用户").Build().ToByteArray();
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                    return;
                }
                return;
            }
        

            foreach (var userID in list)
            {
                DAL.Model.mjuserinfo userinfo = new DAL.DAL.mjuserinfoDAL().GetModel(userID);
                if (userinfo == null)
                {
                    groupInfoDAL.DelUsersByUserID(groupUsersManager.GroupID, userID);
                }
                else
                {
                    player.SetGroupUserID(userID).SetNickName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(userinfo.nickname))).SetPicture(userinfo.headimg);
                    returnPlayerList.AddPlayerList(player);
                }
            }

            //返回待审核人数
            int counts = groupInfoDAL.GroupApplyRecord( groupUsersManager.GroupID).Count();
            returnPlayerList.SetApplyUsers(counts);
            var returnPlayerListData = returnPlayerList.Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1051, returnPlayerListData.Length, requestInfo.MessageNum, returnPlayerListData)));
        }
    }
}
