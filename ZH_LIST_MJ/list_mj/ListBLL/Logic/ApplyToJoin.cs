using DAL.DAL;
using ListBLL.common;
using ListBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.Logic
{
    public class ApplyToJoin : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11040"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
            var sendApplyToJoin = SendApplyToJoin.ParseFrom(requestInfo.Body);
            RedisUserInfoModel user = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, sendApplyToJoin.Openid, sendApplyToJoin.Unionid));

            if (groupInfoDAL.GetGroupInfoByGroupID(sendApplyToJoin.GroupID, user.Type) ==null)
            {
               var returnApplyToJoin = ReturnApplyToJoin.CreateBuilder().SetStatus(0).SetMessage("圈子不存在"). Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1041, returnApplyToJoin.Length, requestInfo.MessageNum, returnApplyToJoin)));
            }
            else if(groupInfoDAL.PlayerApplyRecord(user.UserID, sendApplyToJoin.GroupID,0)==1  ) 
            {
                var returnApplyToJoin = ReturnApplyToJoin.CreateBuilder().SetStatus(0).SetMessage("已申请，待群主通过").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1041, returnApplyToJoin.Length, requestInfo.MessageNum, returnApplyToJoin)));
            }
            else if (groupInfoDAL.PlayerApplyRecord(user.UserID, sendApplyToJoin.GroupID, 1) == 1|| groupInfoDAL.IsUserInGroup(sendApplyToJoin.GroupID, user.UserID)!=0)
            {
                var returnApplyToJoin = ReturnApplyToJoin.CreateBuilder().SetStatus(0).SetMessage("已再圈子中，请勿重复申请").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1041, returnApplyToJoin.Length, requestInfo.MessageNum, returnApplyToJoin)));
            }
            //else if (true)
            //{
            //    var returnApplyToJoin = ReturnApplyToJoin.CreateBuilder().SetStatus(0).SetMessage("已再圈子中，请勿重复申请").Build().ToByteArray();
            //    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1041, returnApplyToJoin.Length, requestInfo.MessageNum, returnApplyToJoin)));
            //}
            else
            {              
                groupInfoDAL.ApplyToJoin(sendApplyToJoin.GroupID, user.UserID);
                var returnApplyToJoin = ReturnApplyToJoin.CreateBuilder().SetStatus(1).SetMessage("申请成功").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1041, returnApplyToJoin.Length, requestInfo.MessageNum, returnApplyToJoin)));
            }           
        }
    }
}
