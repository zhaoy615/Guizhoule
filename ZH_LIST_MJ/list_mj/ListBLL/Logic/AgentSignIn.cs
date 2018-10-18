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
    public class AgentSignIn : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11110"; }

        }
        /// <summary>
        /// 注册代理后台是先判断是否是游戏用户
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var agentSignIn = SendAgentSignIn.ParseFrom(requestInfo.Body);
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
            DAL.Model.mjuserinfo userinfo = new DAL.DAL.mjuserinfoDAL().GetModel(agentSignIn.UserID);
            byte[] returnDate = null;
            if (userinfo != null)
            {
                //int groupID = groupInfoDAL.CreateGroup(userinfo.nickname, userinfo.id);
                //groupInfoDAL.AddUserToGroup(groupID, userinfo.id, 1);

                returnDate = ReturnMessgae.CreateBuilder().SetStatue(1).SetMessage("是游戏用户").Build().ToByteArray();//"{\"圈子ID\":{0},\"圈子名称\":{1},\"圈主ID\":{2}}").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnDate.Length, requestInfo.MessageNum, returnDate)));
            }
            else
            {
                returnDate = ReturnMessgae.CreateBuilder().SetMessage(string.Format("此游戏ID{0}不是游戏用户", agentSignIn.UserID)).SetStatue(0).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnDate.Length, requestInfo.MessageNum, returnDate)));
            }
        }
    }
}
