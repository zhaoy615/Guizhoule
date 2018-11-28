using ListBLL.model;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ListBLL.common
{
   public class RedisUtility
    {
        static string host = ConfigurationManager.AppSettings["redisIP"];
        static int port = int.Parse(ConfigurationManager.AppSettings["redisport"]);
        static string password = ConfigurationManager.AppSettings["redispassword"]; /*密码*/
                                                                                    // static ConnectionMultiplexer client = null;
        static int Database = int.Parse(ConfigurationManager.AppSettings["dbbase"]);
        static ConfigurationOptions config = new ConfigurationOptions()
        {
            Password = password,
            EndPoints = { { host, port } },
            KeepAlive = 180,
            AbortOnConnectFail = false,
            //ConnectRetry=0,
            ConnectTimeout = 1000

        };
        static ConnectionMultiplexer _redis;
        static object _locker = new object();
        public static ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis == null || !_redis.IsConnected)
                        {
                            _redis = ConnectionMultiplexer.Connect(config);
                        }
                    }
                }
                //注册如下事件
                //_redis.ConnectionFailed += MuxerConnectionFailed;
                //_redis.ConnectionRestored += MuxerConnectionRestored;
                _redis.ErrorMessage += MuxerErrorMessage;
                //_redis.ConfigurationChanged += MuxerConfigurationChanged;
                _redis.HashSlotMoved += MuxerHashSlotMoved;
                _redis.InternalError += MuxerInternalError;
                return _redis;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static ConnectionMultiplexer Redis { get => Manager; set => _redis = value; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static ConnectionMultiplexer GetManager(ConfigurationOptions connectionString = null)
        {
            if (connectionString == null)
            {
                connectionString = config;
            }
            return ConnectionMultiplexer.Connect(connectionString);
        }


        public static string GetKey(string mainNode, string openid, string unionid)
        {
            return mainNode + ":" + (string.IsNullOrEmpty(unionid) ? openid : unionid);
        } 

        /// <summary>
        /// 存值并设置过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="t">实体</param>
        /// <param name="ts">过期时间间隔</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Set<T>(string key, T t, int hour = 24)
        {
            TimeSpan ts = new TimeSpan(hour, 0, 0);
            var str = JsonConvert.SerializeObject(t);
            return Redis.GetDatabase(Database).StringSet(key, str, ts);

        }

        /// <summary>
        /// 
        /// 根据Key获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        public static T Get<T>(string key) where T : class
        {
            var strValue = Redis.GetDatabase(Database).StringGet(key);
            return string.IsNullOrEmpty(strValue) ? null : JsonConvert.DeserializeObject<T>(strValue);

        }
        /// <summary>

        /// 存储List

        /// </summary>

        /// <typeparam name="T"></typeparam>

        /// <param name="key"></param>

        /// <param name="value"></param>

        public static void ListSet<T>(string key, List<T> value, int hour = 24)
        {
            TimeSpan ts = new TimeSpan(hour, 0, 0);
            
            Redis.GetDatabase(Database).StringSet(key, JsonConvert.SerializeObject(value), ts);
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> ListGet<T>(string key)
        {
            return JsonConvert.DeserializeObject<List<T>>(Redis.GetDatabase(Database).StringGet(key));           
        }
        public static void Remove(string key)
        {
            Redis.GetDatabase(Database).KeyDelete(key);
        }
        /// <summary>
        /// Dispose DB connection 释放DB相关链接
        /// </summary>
        public static void DbConnectionStop()
        {
            Redis.Dispose();
        }
        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            MyLogger.Logger.Error("Configuration changed: " + e.EndPoint);
        }
        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            MyLogger.Logger.Error("ErrorMessage: " + e.Message);
        }
        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            MyLogger.Logger.Error("ConnectionRestored: " + e.EndPoint);
        }
        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            MyLogger.Logger.Error("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }
        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            MyLogger.Logger.Error("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }
        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            MyLogger.Logger.Error("InternalError:Message" + e.Exception.Message);
        }
        /// <summary>
        /// 根据服务器名称获取并发送给客户端 ip和端口
        /// </summary>
        /// <param name="serverName">服务器名称</param>
        /// <param name="messageNum">收到的消息号</param>
        /// <param name="session"></param>
        /// <param name="status">状态 1为正常切换服务器，2为有牌局未结束</param>
        /// <param name="openid"></param>
        /// <param name="unionid"></param>
        /// <param name="cutover">是否自动切换服务器</param>
        /// <returns></returns>
        public static bool GetServerIP(string serverName, int messageNum, GameSession session, int status, string openid, string unionid, bool cutover = true, int roomID = 0, int roomAddStatus = 0,int roomType=0)
        {
            IPEndPoint ipInfo = GetServerIP(serverName);
            if (ipInfo == null)
            {
                if (cutover)
                    return false;
                session.Logger.Error(string.Format("服务器配置文件不存在 {0} 服务器", serverName));
                ipInfo = GetServerIP(GameInformationBase.serverName);//如果服务器名称不存在 则返回本机ip和端口，重新连接本服务器

                Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, openid, unionid));
            }
            byte[] buffer = new byte[1024];
            ReturnServerIP.Builder serverIP = ReturnServerIP.CreateBuilder();
            serverIP.SetIp(ipInfo.Address.ToString()).SetPort(ipInfo.Port.ToString()).SetStatus(status).SetRoomType(roomType);
            if (status == 2)
                serverIP.SetRoomID(roomID.ToString());
            if (roomAddStatus != 0)
                serverIP.SetStatus(roomAddStatus);
            byte[] serverIPData = serverIP.Build().ToByteArray();
            

            //告知Play服务器进行房间号存储
            Socket sc = new Socket(ipInfo.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var roomData = SendRoomExist.CreateBuilder().SetOpenid(openid).SetUnionid(unionid).SetRoomID(roomID).Build().ToByteArray();
            if (roomID == 0)
            {
                session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1010, serverIPData.Length, messageNum, serverIPData)));
                return true;
            }
            try
            {
                sc.Connect(GetServerIP(serverName));
                sc.Send(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 9901, roomData.Length, 19990, roomData));
                int count = 0;
                count = sc.Receive(buffer);
                var ms = new MemoryStream();
                ms.Write(buffer, 0, count);
                var datainfo = ms.ToArray();
                int number = IntToByte.bytesToInt(datainfo, 0);//消息号
                int length = IntToByte.bytesToInt(datainfo, 4);//消息长度
                int resnumber = IntToByte.bytesToInt(datainfo, 8);//返回消息号
                byte[] body = new byte[length];
                Array.Copy(datainfo, 12, body, 0, length);
                var roomExist = ReturnRoomExist.ParseFrom(body);
                if (roomExist.IsExist == 1)
                {
                    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1010, serverIPData.Length, messageNum, serverIPData)));
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                session.Logger.Debug(ex.ToString());
                return false;
            }


        }
        /// <summary>
        /// 从ip配置表中获取对应服务器名称的 ip和端口
        /// </summary>
        /// <param name="serverName">服务器名称</param>
        /// <param name="isServer">是否是服务器访问</param>
        /// <returns></returns>
        public static IPEndPoint GetServerIP(string serverName)
        {
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;//忽略文档里面的注释

            using (var reader = XmlReader.Create(@"ServerNameList.xml", settings))
            {
                doc.Load(reader);
                XmlNode nodeList = doc.SelectSingleNode("servers");
                foreach (XmlNode item in nodeList.ChildNodes)
                {
                    XmlElement xe = (XmlElement)item;
                    if (xe.GetAttribute("name").Equals(serverName))
                    {
                        return new IPEndPoint(IPAddress.Parse(xe.ChildNodes[0].InnerText), int.Parse(xe.ChildNodes[1].InnerText));
                    }
                }
                return null;
            }
        }

    }
}
