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
    public class GroupUsersManager : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11050"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();


           var requestUsersManager=  SendGroupUsersManager.ParseFrom(requestInfo.Body);
            // RedisUserInfoModel user = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, requestUsersManager.Openid, requestUsersManager.Unionid));
            var nowUser = Gongyong.userlist.First(w => w != null && w.unionid.Equals(requestUsersManager.Unionid));
            if (nowUser == null)
            {
                session.Logger.Debug("管理朋友圈用户 未登录！");
                return;
            }
            var player = Player.CreateBuilder();
            var returnPlayerList = ReturnPlayerList.CreateBuilder();
            var list=  groupInfoDAL.GetUsersIDListByGroupID(requestUsersManager.GroupID);
            byte[] returnMessage = null;

            var groupInfo = groupInfoDAL.GetGroupInfoByGroupID(requestUsersManager.GroupID, nowUser.Type);
            if (nowUser.Type == 0)
            {
                if (groupInfo.CreateUserID != nowUser.UserID)
                {
                    session.Logger.Debug("管理朋友圈用户不是圈主！");
                    return;
                }
            }
            else if (nowUser.Type == 1)//龙宝用户
            {
                if (!groupInfo.CreateUserUnionID.Equals(nowUser.unionid))
                {
                    session.Logger.Debug("管理朋友圈用户不是圈主！");
                    return;
                }
            }
            //删除用户
            if (requestUsersManager.HasDelByUserID&&!requestUsersManager.HasAddUsers)
            {
             
                if (groupInfo.CreateUserID == requestUsersManager.DelByUserID)
                {
                    returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("群主不能删除自己").Build().ToByteArray();
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                    return;
                }
                else
                {

                   // groupInfoDAL.DelUsersByUserID(requestUsersManager.GroupID, requestUsersManager.DelByUserID);
                    groupInfoDAL.DelUsersByUserIDTransaction(requestUsersManager.GroupID, requestUsersManager.DelByUserID, 3, requestUsersManager.DelByUserID);


                    returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage(string.Format("删除用户{0}成功", requestUsersManager.DelByUserID)).Build().ToByteArray();
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                    return;
                }

            }
            //添加用户
            else if (!requestUsersManager.HasDelByUserID && requestUsersManager.HasAddUsers)
            {
                //加入类型，1圈主添加，2申请加入，3推荐加入
               // byte[] returnMessage = null;
                //var groupInfo = groupInfoDAL.GetGroupInfoByGroupID(requestUsersManager.GroupID);
                //if (groupInfo.CreateUserID == requestUsersManager.AddUsers)
                //{
                //    returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("添加失败").Build().ToByteArray();
                //    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                //    return;
                //}
                if (new DAL.DAL.mjuserinfoDAL().GetModel(requestUsersManager.AddUsers) != null)
                {
                    var err = groupInfoDAL.AddUserToGroup(requestUsersManager.GroupID, requestUsersManager.AddUsers, 1);

                    if (err == 0)
                    {
                        returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("添加失败，已存在该用户").Build().ToByteArray();
                        session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
                    }
                    else
                    {
                        returnMessage = ReturnMessgae.CreateBuilder().SetStatue(1).SetMessage("添加成功").Build().ToByteArray();


                        //添加成狗后把用户丢到RedisUs  避免需重复登陆问题
                        RedisUserInfoModel user = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, requestUsersManager.AddUsers.ToString(), requestUsersManager.AddUsers.ToString()));
                        if (user != null)
                        {
                            user.GroupID.Add((int)requestUsersManager.GroupID);
                            RedisUtility.Set(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, user.Openid, user.Unionid), user);
                            var userInfo = Gongyong.userlist.Find(w => w.UserID == user.UserID);
                            if (userInfo != null)
                                userInfo.GroupID.Add((int)requestUsersManager.GroupID);

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
            else if (requestUsersManager.HasCheckRecordByUserID)
            {
                var createRommRecord = CreateRommRecord.CreateBuilder();
                var returnRecordList = ReturnRecordList.CreateBuilder();
                var listRecord = groupInfoDAL.GetCreateRoomRecord(requestUsersManager.CheckRecordByUserID,requestUsersManager.GroupID);
                foreach (var item in listRecord)
                {
                    createRommRecord.SetGroupID(item.GroupID).SetRoomID(item.RoomID).SetCreateUserID(item.CreateUserID).SetCreateDate(TimeToLong.ConvertDateTimeInt( item.CreateDate)).SetUseRoomCard(item.UseRoomCard);
                    returnRecordList.AddCreateRommRecordList(createRommRecord);
                }
            
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1052, returnRecordList.Build().ToByteArray().Length, requestInfo.MessageNum, returnRecordList.Build().ToByteArray())));
                // session.Close();
                return;
            }

            foreach (var userID in list)
            {
                DAL.Model.mjuserinfo userinfo = new DAL.DAL.mjuserinfoDAL().GetModel(userID);
                if (userinfo == null)
                {
                    groupInfoDAL.DelUsersByUserID(requestUsersManager.GroupID, userID);
                }
                else
                {
                    player.SetGroupUserID(userID).SetNickName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(userinfo.nickname))).SetPicture(userinfo.headimg);
                    returnPlayerList.AddPlayerList(player);
                }
            }

            //返回待审核人数
            int counts =  groupInfoDAL.GroupApplyRecord(requestUsersManager.GroupID).Count();
            returnPlayerList.SetApplyUsers(counts);
            var returnPlayerListData = returnPlayerList.Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1051, returnPlayerListData.Length, requestInfo.MessageNum, returnPlayerListData)));
        }
    }
}
