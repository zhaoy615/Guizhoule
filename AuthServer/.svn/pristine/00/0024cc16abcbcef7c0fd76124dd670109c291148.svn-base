using MJBLL.common;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Longbao
{
    public class SubtractLongBao : ICommand<GameSession, ProtobufRequestInfo>
    {

       // const int count = 3; //默认扣除龙宝数量
        
        public string Name
        {
            get { return "11007"; }
        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
           var substractLongBao= SendSubtractLongBao.ParseFrom(requestInfo.Body);
            DAL.DAL.Longbao longbao = new DAL.DAL.Longbao();
            int state = 0;
            try
            {
                state = longbao.SubSubtractLongBao(substractLongBao.UserID, substractLongBao.Count, substractLongBao.Logging);
            }
            catch (Exception ex)
            {
                MyLogger.Logger.Error(ex);
            }
            if (state==1)
            {
                var data = ReturnMessage.CreateBuilder().SetState(state).SetMessage("扣除成功").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(11010, data.Length, requestInfo.MessageNum, data)));
                //longbao.SubLongBaoLog(substractLongBao.UserID, substractLongBao.Logging,count);
                //Console.WriteLine(substractLongBao.UserID, substractLongBao.Logging,count);
            }
            else
            {
                var data = ReturnMessage.CreateBuilder().SetState(state).SetMessage("扣除失败").Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(11010, data.Length, requestInfo.MessageNum, data)));
            }
        }
    }
}
