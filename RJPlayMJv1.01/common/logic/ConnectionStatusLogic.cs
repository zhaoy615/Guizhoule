using MJBLL.common;
using MJBLL.model;
using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MJBLL.logic
{
    public class ConnectionStatusLogic
    {
        public static void ChackConnection()
        {
            try
            {
                while (true)
                {
                    for (int i = 0; i < Gongyong.roomlist.Count; i++)//循环发送牌桌内用户是否离线等消息
                    {
                        var roomInfo = Gongyong.roomlist[i];
                        var mjUsers = Gongyong.mulist.Where(w => w.RoomID == roomInfo.RoomID).ToList();
                        var returnConnectionStatus = ReturnConnectionStatus.CreateBuilder();
                        for (int j = 0; j < mjUsers.Count(); j++)
                        {
                            var mjUser = mjUsers[j];
                            var userInfo = Gongyong.userlist.Find(w => w.openid.Equals(mjUser.Openid));
                            if (userInfo != null)
                            {
                                returnConnectionStatus.AddConnectionStatus(ConnectionStatusInfo.CreateBuilder().SetFW(mjUser.ZS_Fw).SetConnectionStatus(mjUser.ConnectionStatus == 0 ? 0 : (userInfo.session.Connected ? 1 : 0)));
                            }
                        }
                        var data = returnConnectionStatus.Build().ToByteArray();
                        var ipsame = ReturnIPSame.CreateBuilder();
                        var closeGPS = ReturnCloseGPS.CreateBuilder();
                        for (int m = 0; m < mjUsers.Count; m++)
                        {
                            var mjUser = mjUsers[m];
                            var returnDis = ReturnDis.CreateBuilder();
                            returnDis.SetFW(mjUser.ZS_Fw);
                            var userInfo = Gongyong.userlist.Find(w => w.openid.Equals(mjUser.Openid));
                            if (userInfo != null)
                            {
                                foreach (var item in mjUsers.Where(w => !w.Openid.Equals(mjUser.Openid)))
                                {
                                    var userjuliInfo = Gongyong.userlist.Find(w => w.openid.Equals(item.Openid));
                                    if (userInfo.UserIP.Equals(userjuliInfo.UserIP))
                                    {
                                        ipsame.AddFW(mjUser.ZS_Fw);
                                        ipsame.AddFW(item.ZS_Fw);
                                    }
                                    if (userjuliInfo.Lat.Equals("0,0") || string.IsNullOrEmpty(userjuliInfo.Lat))
                                    {
                                        if (!closeGPS.FWList.Any(w => w == item.ZS_Fw))
                                            closeGPS.AddFW(item.ZS_Fw);
                                        // gamersend.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7091, dataGPS.Length, requestInfo.MessageNum, dataGPS)));
                                    }
                                    else if (userInfo.Lat.Equals("0,0") || string.IsNullOrEmpty(userInfo.Lat))
                                    {
                                        if (!closeGPS.FWList.Any(w => w == mjUser.ZS_Fw))
                                            closeGPS.AddFW(mjUser.ZS_Fw);
                                    }
                                    else
                                    {
                                        double jl = Erth.GetDistance(userjuliInfo.Lat, userInfo.Lat);

                                        returnDis.SetFWT(item.ZS_Fw).SetDis(jl.ToString());//将距离封装
                                        if (jl <= GameInformationBase.DISTANCE)
                                        {
                                            var datauser = ReturnIsJ.CreateBuilder().SetFWO(mjUser.ZS_Fw).SetFWW(item.ZS_Fw).Build().ToByteArray();
                                            foreach (var mjuser in mjUsers)
                                            {
                                                var user = Gongyong.userlist.Find(w => w.openid.Equals(mjuser.Openid));
                                                user.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7095, datauser.Length, 0, datauser)));
                                            }
                                        }
                                        var disData = returnDis.Build().ToByteArray();
                                        // userInfo.session.Logger.Debug(returnDis.ToString());
                                        userInfo.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 8004, data.Length, 0, data)));
                                        userInfo.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7091, disData.Length, 0, disData)));
                                    }


                                }

                            }
                        }
                        if (ipsame.FWCount > 2)
                        {
                            var dataip = ipsame.Build().ToByteArray();
                            foreach (var mjuser in mjUsers)
                            {
                                var user = Gongyong.userlist.Find(w => w.openid.Equals(mjuser.Openid));
                                user.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7096, dataip.Length, 0, dataip)));
                            }
                        }
                        if (closeGPS.FWCount > 0)
                        {
                            var dataGPS = closeGPS.Build().ToByteArray();
                            foreach (mjuser items in mjUsers)
                            {

                                UserInfo gamersend = Gongyong.userlist.Find(u => u.openid == items.Openid);
                                gamersend.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7097, dataGPS.Length, 0, dataGPS)));
                            }
                        }

                    }
                    Thread.Sleep(5000);

                }
            }
            catch (Exception ex)
            {
                MyLogger.Logger.Debug(ex);
            }
        }
    }
}
