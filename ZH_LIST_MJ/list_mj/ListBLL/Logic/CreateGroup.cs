using DAL.DAL;
using ListBLL.common;
using ListBLL.model;
using Newtonsoft.Json;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ListBLL.Logic
{
    public class CreateGroup : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11103"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var sendCreateGroup = SendCreateGroup.ParseFrom(requestInfo.Body);
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
            DAL.Model.mjuserinfo userinfo = new DAL.DAL.mjuserinfoDAL().GetModel(sendCreateGroup.UserID);
            byte[] returnDate = null;
            if (userinfo != null)
            {
                int groupID = groupInfoDAL.CreateGroup(userinfo.nickname, userinfo.id, userinfo.unionid);
                //groupInfoDAL.AddUserToGroup(groupID, userinfo.id, 1);
                 
                returnDate = ReturnMessgae.CreateBuilder().SetStatue(1).SetMessage(JsonConvert.SerializeObject(new { GroupID = groupID, Nickname = userinfo.nickname, UserID = userinfo.id })).Build().ToByteArray();//"{\"圈子ID\":{0},\"圈子名称\":{1},\"圈主ID\":{2}}").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnDate.Length, requestInfo.MessageNum, returnDate)));
            }
            else
            {
                returnDate = ReturnMessgae.CreateBuilder().SetMessage("不是游戏用户").SetStatue(0).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnDate.Length, requestInfo.MessageNum, returnDate)));
            }


        }
    }
}
