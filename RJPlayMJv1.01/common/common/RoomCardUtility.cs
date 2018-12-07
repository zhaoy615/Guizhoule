using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.common
{
   public  class RoomCardUtility
    {
        static string host = ConfigurationManager.AppSettings["RoomCardServerIP"];
        static int port = int.Parse(ConfigurationManager.AppSettings["RoomCardServerPort"]);

        public static bool ReduceRoomCard(long userID, int number)
        {
            var data = RoomCardBusiness.CreateBuilder().SetUserID(userID.ToString()).SetBusinessID("6").SetCounts(number).Build().ToByteArray();
            try
            {
                var resData = new MySocket(host, port).SendReceive(CreateHead.CreateMessage(10030, data.Length, 0, data));
                if (resData.FirstOrDefault() != null)
                {
                    var status = Result.ParseFrom(resData.FirstOrDefault().Data).Status;
                    if (status == 1)
                        return true;
                    else
                    {
                        MyLogger.Logger.Info("请求扣除房卡" + status);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.Logger.Info("请求扣除房卡:" + ex);
            }
            return false;
        }
    }
}
