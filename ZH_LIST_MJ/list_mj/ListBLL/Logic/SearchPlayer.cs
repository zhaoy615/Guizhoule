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
    public class SearchPlayer : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11108"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
            var sendSearchPlayer = SendSearchPlayer.ParseFrom(requestInfo.Body);

            mjuserinfoDAL mjuserinfoDAL = new mjuserinfoDAL();
            var userinfo =   mjuserinfoDAL.GetModel(sendSearchPlayer.UserID);

            if (userinfo!=null)
            {
                var returnDate = Player.CreateBuilder().SetGroupUserID(userinfo.id).SetNickName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(userinfo.nickname))).SetPicture(userinfo.headimg).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER , returnDate.Length, requestInfo.MessageNum, returnDate)));
            }
            else
            {
                var  returnMessage = ReturnMessgae.CreateBuilder().SetStatue(0).SetMessage("该用户不存在").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1104, returnMessage.Length, requestInfo.MessageNum, returnMessage)));
            }

         
        }
    }
}
