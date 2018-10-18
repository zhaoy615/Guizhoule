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
    public class DeleteGroup : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11109"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();

            var sendDeleteGroup = SendDeleteGroup.ParseFrom(requestInfo.Body);
            byte[] returnDate;
            try
            {
                groupInfoDAL.DeleteGroup(sendDeleteGroup.GroupID, sendDeleteGroup.AdminID);
                returnDate = ReturnMessgae.CreateBuilder().SetMessage("解散朋友圈成攻").SetStatue(1).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnDate.Length, requestInfo.MessageNum, returnDate)));
            }
            catch (Exception)
            {
                returnDate = ReturnMessgae.CreateBuilder().SetMessage("解散朋友圈失败").SetStatue(0).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnDate.Length, requestInfo.MessageNum, returnDate)));

            }
         
        }
    }
}
