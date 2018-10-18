using ListBLL.common;
using ListBLL.model;
using Newtonsoft.Json;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.Logic
{
    public class LoginPW : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "11003";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var loginInfo = SendLoginPW.ParseFrom(requestInfo.Body);
            dynamic info = null;
            string infoStr = string.Empty;
            if (loginInfo.HasCertificate)//如果有传证书
            {
                 infoStr = RoomCardUtility.GetloginInfoByCertStr(CompressUtility.DecompressString( loginInfo.Certificate));
                if (string.IsNullOrEmpty(infoStr))//3证书无效
                {
                    byte[] msg = ReturnLogin.CreateBuilder().SetLoginstat(3).SetUserID(0).SetUserRoomCard(0).Build().ToByteArray();
                    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));
                    return;
                }
                else
                {
                    try
                    {
                         info = JsonConvert.DeserializeObject<dynamic>(infoStr);
                        if ((DateTime.Now - ((DateTime)info.dateTime)).TotalHours >= 168)//证书过期
                        {
                            byte[] msg = ReturnLogin.CreateBuilder().SetLoginstat(4).SetUserID(0).SetUserRoomCard(0).Build().ToByteArray();
                            session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));
                            return;
                        }
                        info.Score = RoomCardUtility.GetLongBaoNumber(info.ID.ToString());
                    }
                    catch (Exception ex)
                    {
                        session.Logger.Error(ex);
                        byte[] msg = ReturnLogin.CreateBuilder().SetLoginstat(3).SetUserID(0).SetUserRoomCard(0).Build().ToByteArray();
                        session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));
                        return;
                    }
                }
            }
            else
            {
                 infoStr = RoomCardUtility.GetloginInfoByPWD( loginInfo.UserAccount,loginInfo.Pwd);
                if(string.IsNullOrEmpty(infoStr)|| JsonConvert.DeserializeObject<dynamic>(infoStr)==null) //5账号密码错误，
                {
                    session.Logger.Debug(infoStr+ "infoStr" + infoStr);
                    byte[] msg = ReturnLogin.CreateBuilder().SetLoginstat(5).SetUserID(0).SetUserRoomCard(0).Build().ToByteArray();
                    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));
                    return;
                }
                 info = JsonConvert.DeserializeObject<dynamic>(infoStr);
            }
            SendLogin loginInfobuild =  SendLogin.CreateBuilder().SetCity(loginInfo.City).SetHeadimg(string.IsNullOrEmpty(info.HeadImg1.ToString()) ?"1": string.Format("http://www.qytfkj.com{0}", info.HeadImg1)).SetLatitude(loginInfo.Latitude).SetNickname(info.TrueName.ToString())
                  .SetOpenid(info.ID.ToString()).SetProvince(loginInfo.Province).SetSex(info.Sex.ToString().Equals("1") ? "2" : "1").SetUnionid(info.ID.ToString()).Build();
            var loginInfoByte = loginInfobuild.ToByteArray();
            var login = new Login();
         var json=   new
            {
             ID= info.ID,
             Sex= info.Sex,
             HeadImg1=info.HeadImg1,
             TrueName= info.TrueName,
             Score= info.Score,
             dateTime=DateTime.Now
         };
            login.CerStr = CompressUtility.CompressString( RoomCardUtility.GetCertStrByloginInfoStr(JsonConvert.SerializeObject(json)));
            login.UserType = 1;
            login.UserLongBao = (long)info.Score;
            login.ExecuteCommand(session, new ProtobufRequestInfo { Body = loginInfoByte });
        }
    }
}
