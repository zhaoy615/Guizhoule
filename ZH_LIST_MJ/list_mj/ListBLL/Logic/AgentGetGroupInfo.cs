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
    public class AgentGetGroupInfo : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11105"; }
        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var sendAgentGetGroupInfo = SendAgentGetGroupInfo.ParseFrom(requestInfo.Body);

            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();


            DAL.Model.GroupInfo groupInfoModel =  groupInfoDAL.GetGroupInfoByGroupID(sendAgentGetGroupInfo.GroupID,0);

            if (groupInfoModel!=null)
            {
                var returnDate = ReturnAgentGetGroupInfo.CreateBuilder().SetGroupID(sendAgentGetGroupInfo.GroupID).
                SetGroupName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(groupInfoModel.GroupName))).SetNikeName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(groupInfoModel.GroupName))).
                SetRoomCardCounts(RoomCardUtility.GetRoomCard(sendAgentGetGroupInfo.UserID)).
                SetCreateTime(TimeToLong.ConvertDateTimeInt(groupInfoModel.CreateTime)).
                Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1106, returnDate.Length, requestInfo.MessageNum, returnDate)));
            }
            else
            {
                var returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("圈子不存在").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
            }
        }
    }
}
