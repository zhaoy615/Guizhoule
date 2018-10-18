using MJBLL.common;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CommonClass;


namespace BLL.Longbao
{
    public class UserLogin : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11005"; }
        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("ColoredConsoleAppender");
            //  ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            var info = LBUserLogin.ParseFrom(requestInfo.Body);

            DAL.DAL.Longbao longbao = new DAL.DAL.Longbao();

           // HashPasswordForStoringInConfigFile(a, "MD5")

           var JsonStr= JsonConvert.SerializeObject(longbao.GetUserInfo(info.UserName, EncryptDecrypt.GetMd5Hash( info.Pwd)));

            var data = ReturnLBUserLoginInfo.CreateBuilder().SetUserInfoJson(JsonStr).Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(11006, data.Length, requestInfo.MessageNum, data)));
           // session.Close();
           // return;

        }
    }
}
