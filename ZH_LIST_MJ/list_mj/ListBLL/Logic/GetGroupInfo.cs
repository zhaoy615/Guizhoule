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
    public class GetGroupInfo : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11030"; }
        }
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            List<GroupInfo> groupInfoList = new List<GroupInfo>();
            var requestGroupInfo = SendGroupInfo.ParseFrom(requestInfo.Body);
            RedisUserInfoModel user = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, requestGroupInfo.Openid, requestGroupInfo.Unionid));
            if (user == null)
            {
                session.Logger.Debug(string.Format("缓存无此用户{0}", string.IsNullOrEmpty(requestGroupInfo.Openid) ? "用户openid为空" : ""));
                return;
            }
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
            var list = groupInfoDAL.GetGroupIDListByUserID(user.UserID, user.Type);
            var returnGroupInfo = ReturnGroupInfo.CreateBuilder();
            var GroupInfodata = GroupInfo.CreateBuilder();
            Byte[] returnGroupInfoData = null;
            if (list==null||list.Count == 0)
            {
                returnGroupInfoData = returnGroupInfo.Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1031, returnGroupInfoData.Length, requestInfo.MessageNum, returnGroupInfoData)));
            }
            else
            {
                foreach (var GroupID in list.Distinct())
                {
                    DAL.Model.GroupInfo groupInfoModel = groupInfoDAL.GetGroupInfoByGroupID(GroupID, user.Type);
                    if (groupInfoModel != null)
                    {
                        if (groupInfoModel.CreateUserID == user.UserID)
                        {
                            GroupInfodata.SetGroupID(GroupID).SetGroupIntroduction(groupInfoModel.GroupIntroduction ?? "").SetGroupName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(groupInfoModel.GroupName))).SetNikeName(groupInfoModel.GroupName).SetRoomCardCounts(RoomCardUtility.GetRoomCard(groupInfoModel.CreateUserID)).SetCreateTime(TimeToLong.ConvertDateTimeInt(groupInfoModel.CreateTime)).SetIsGroupLord(true);
                            returnGroupInfo.AddGroupInfo(GroupInfodata);
                            //data.SetGroupInfo(1,GroupInfo)
                        }
                        else
                        {
                            GroupInfodata.SetGroupID(GroupID).SetGroupIntroduction(groupInfoModel.GroupIntroduction ?? "").SetGroupName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(groupInfoModel.GroupName))).SetNikeName(groupInfoModel.GroupName).SetRoomCardCounts(RoomCardUtility.GetRoomCard(groupInfoModel.CreateUserID)).SetCreateTime(TimeToLong.ConvertDateTimeInt(groupInfoModel.CreateTime)).SetIsGroupLord(false);
                            returnGroupInfo.AddGroupInfo(GroupInfodata);
                        }
                    }

                }
                returnGroupInfoData = returnGroupInfo.Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1031, returnGroupInfoData.Length, requestInfo.MessageNum, returnGroupInfoData)));
            }
        }
    }
}
