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
    public class GetGroupInfoByGroupID : ICommand<GameSession, ProtobufRequestInfo>
    {/// <summary>
     /// 根据圈子ID获取朋友圈信息
     /// </summary>
        public string Name => "11032";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var sendData = SendGroupInfoByGroupID.ParseFrom(requestInfo.Body);
            var userInfo = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, sendData.Openid, sendData.Unionid));
            if (userInfo == null || !userInfo.GroupID.Any(w => w == sendData.GroupID))
            {
                session.Close();
                return;
            }
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
            var groupInfo = groupInfoDAL.GetGroupInfoByGroupID(sendData.GroupID, userInfo.Type);
            var returnGroupInfo = ReturnGroupInfoByGroupID.CreateBuilder();
            if (groupInfo != null)
            {
                var data = returnGroupInfo.SetStatus(1). SetCreateTime(TimeToLong.ConvertDateTimeInt(groupInfo.CreateTime)).SetGroupID(groupInfo.GroupID)
                      .SetGroupName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(groupInfo.GroupName))).SetNikeName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(groupInfo.NikeName))).SetCreateUserID(groupInfo.CreateUserID)
                      .SetGroupNumberPeople(groupInfoDAL.GetGroupPeopleNumber(sendData.GroupID)).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1033, data.Length, requestInfo.MessageNum, data)));
            }
            else
            {
                var data = returnGroupInfo.SetStatus(0).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1033, data.Length, requestInfo.MessageNum, data)));
            }
        }
    }
}
