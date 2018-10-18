using MJBLL.common;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Longbao
{
    public class GetLongBao : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11011"; }
        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var getLongBao = GetLongBaoCount.ParseFrom(requestInfo.Body);
            DAL.DAL.Longbao longbao = new DAL.DAL.Longbao();
           int count=  longbao.GetLongBaoCount(getLongBao.ID);

            var data = ReturnLongBaoCount.CreateBuilder().SetCount(count).Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(11012, data.Length, requestInfo.MessageNum, data)));
        }
    }
}
