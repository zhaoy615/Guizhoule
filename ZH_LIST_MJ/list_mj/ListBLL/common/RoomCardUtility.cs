using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.common
{
    public class RoomCardUtility
    {
        static string host = ConfigurationManager.AppSettings["RoomCardServerIP"];
        static int port = int.Parse(ConfigurationManager.AppSettings["RoomCardServerPort"]);
        /// <summary>
        /// 获取房卡
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static long GetRoomCard(long userID)
        {
            var data = RequestUserInfo.CreateBuilder().SetUserID(userID.ToString()).Build().ToByteArray();
            try
            {
                var resData = new MySocket(host, port).SendReceive(CreateHead.CreateMessage(10040, data.Length, 0, data));
                if (resData.FirstOrDefault() != null)
                {
                    var roomcard= ResponseUserInfo.ParseFrom(resData.FirstOrDefault().Data).RoomCard;
                   // MyLogger.Logger.Info("请求房卡:" + roomcard);
                    return roomcard;
                }

            }
            catch (Exception ex)
            {
                MyLogger.Logger.Error("请求扣除房卡异常:" + ex);
            }
           return 0;

            //测试
           // return 5000;
        }
        /// <summary>
        /// 扣除房卡
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static long ReduceRoomCard(long userID, int number)
        {
            var data = RoomCardBusiness.CreateBuilder().SetUserID(userID.ToString()).SetBusinessID("6").SetCounts(number).Build().ToByteArray();
            try
            {
                var resData = new MySocket(host, port).SendReceive(CreateHead.CreateMessage(10040, data.Length, 0, data));
                if (resData.FirstOrDefault() != null)
                {
                    var status = Result.ParseFrom(resData.FirstOrDefault().Data).Status;
                    if (status == 1)
                        return 1;
                    else
                    {
                        MyLogger.Logger.Info("请求扣除房卡" + status);
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.Logger.Info("请求扣除房卡:" + ex);
            }
            return 0;
        }
        /// <summary>
        /// 根据证书获取用户登录信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string GetloginInfoByCertStr(string certStr)
        {
            var data = GetLoginInfoStr.CreateBuilder().SetCertStr(certStr).Build().ToByteArray();
            try
            {
                var resData = new MySocket(host, port).SendReceive(CreateHead.CreateMessage(11003, data.Length, 0, data));
                if (resData.FirstOrDefault() != null)
                {
                   return  ReturnLoginInfoStr.ParseFrom(resData.FirstOrDefault().Data).LoginInfoStr;
                }
            }
            catch (Exception ex)
            {
                MyLogger.Logger.Info("请求用户登录:" + ex);
            }
            return "";
        }
        /// <summary>
        /// 根据用户登录信息获取证书
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string GetCertStrByloginInfoStr(string loginInfoStr)
        {
            var data = GetCertStr.CreateBuilder().SetLoginInfoStr(loginInfoStr).Build().ToByteArray();
            try
            {
                var resData = new MySocket(host, port).SendReceive(CreateHead.CreateMessage(11001, data.Length, 0, data));
                if (resData.FirstOrDefault() != null)
                {
                    return ReturnCertStr.ParseFrom(resData.FirstOrDefault().Data).CertStr;
                }
            }
            catch (Exception ex)
            {
                MyLogger.Logger.Info("请求用户登录:" + ex);
            }
            return "";
        }
        /// <summary>
        /// 根据账号密码获取用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string GetloginInfoByPWD(string loginInfoStr, string pwd)
        {
            var data = LBUserLogin.CreateBuilder().SetUserName(loginInfoStr).SetPwd(pwd).Build().ToByteArray();
            try
            {
                var resData = new MySocket(host, port).SendReceive(CreateHead.CreateMessage(11005, data.Length, 0, data));
                if (resData.FirstOrDefault() != null)
                {
                    return ReturnLBUserLoginInfo.ParseFrom(resData.FirstOrDefault().Data).UserInfoJson;
                }
            }
            catch (Exception ex)
            {
                MyLogger.Logger.Info("请求用户登录:" + ex);
            }
            return "";
        }
        /// <summary>
        /// 获取龙宝数量
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static int GetLongBaoNumber(string unionid)
        {
            var data = GetLongBaoCount.CreateBuilder().SetID(unionid) .Build().ToByteArray();
            try
            {
                var resData = new MySocket(host, port).SendReceive(CreateHead.CreateMessage(11011, data.Length, 0, data));
                if (resData.FirstOrDefault() != null)
                {
                    return ReturnLongBaoCount.ParseFrom(resData.FirstOrDefault().Data).Count;
                }
            }
            catch (Exception ex)
            {
                MyLogger.Logger.Info("请求用户登录:" + ex);
            }
            return 0;
        }
    }
}
