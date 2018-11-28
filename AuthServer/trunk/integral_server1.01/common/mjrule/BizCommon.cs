using DAL.Model;
using MJBLL.common;
using SuperSocket.SocketBase.Command;
using System;

namespace MJBLL.mjrule
{
    public class BizCommon : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "10040"; }
        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("ColoredConsoleAppender");
            //  ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            var info = RequestUserInfo.ParseFrom(requestInfo.Body);

            DAL.DAL.Business business = new DAL.DAL.Business();
            //获取用户积分消费卷信息
            IntegralInfo integralInfo = business.GetIntegralInfo(info.UserID);
            
            byte[] data = null;
            if (integralInfo == null)
            {

                log.Error("用户不存在");
                business.InsertIntegralInfo(info.UserID);
                data = ResponseUserInfo.CreateBuilder().SetUserID(info.UserID).SetRoomCard(0).SetIntegral(0).SetCoupons(0).Build().ToByteArray();
                session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(10041, data.Length, requestInfo.MessageNum, data)));
                session.Close();
                return;
            }
            data = ResponseUserInfo.CreateBuilder().SetUserID(integralInfo.userID).SetRoomCard((int)integralInfo.roomCard).SetIntegral((double)integralInfo.integral).SetCoupons((double)integralInfo.coupons).Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(10041, data.Length, requestInfo.MessageNum, data)));
            session.Close();
            return;

        }
    }
}
