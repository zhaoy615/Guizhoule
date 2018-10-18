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
  public  class QuitGroup : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11090"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();


            var sendQuitGroup = SendQuitGroup.ParseFrom(requestInfo.Body);
            byte[] resultData = null;
            var deluserInfo = new mjuserinfoDAL().GetModel(sendQuitGroup.UserID);
            var groupInfo=  groupInfoDAL.GetGroupInfoByGroupID(sendQuitGroup.GroupID, deluserInfo.is_band.Value);
            if (groupInfo == null)
            {
                session.Logger.Debug("用户退出不存在朋友圈 关闭连接");
                session.Close();
                return;
            }
            if(groupInfoDAL.GetIsExistenceInGroup(sendQuitGroup.GroupID, sendQuitGroup.UserID, 4) != 1)
            {
                resultData = ReturnQuitGroup.CreateBuilder().SetStatus(1).SetMessage("您不是该圈子的成员！").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1091, resultData.Length, requestInfo.MessageNum, resultData)));
                return;
            }
            if (groupInfoDAL.GetIsExistenceApplyStatus(sendQuitGroup.GroupID, sendQuitGroup.UserID, 4) == 1)
            {
                resultData = ReturnQuitGroup.CreateBuilder().SetStatus(1).SetMessage("您已经申请过退出，请等待群主通过！").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1091, resultData.Length, requestInfo.MessageNum, resultData)));
            }
            else if (groupInfoDAL.ApplyUsersOutByUserIDTransaction(sendQuitGroup.GroupID, sendQuitGroup.UserID,4, groupInfo.CreateUserID,true) !=0)
            {
              // groupInfoDAL.DelApplyByUserID(sendQuitGroup.GroupID, sendQuitGroup.UserID);
               //groupInfoDAL.ChangeApplyStatus(sendQuitGroup.GroupID, sendQuitGroup.UserID, 4);
               resultData =  ReturnQuitGroup.CreateBuilder().SetStatus(1).SetMessage("申请退出成功，等待群主通过！").Build().ToByteArray();
               session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1091, resultData.Length, requestInfo.MessageNum, resultData)));
            }
            else
            {
                resultData = ReturnQuitGroup.CreateBuilder().SetStatus(1).SetMessage("申请退出失败！").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1091, resultData.Length, requestInfo.MessageNum, resultData)));
            }
            


        }
    }
}
