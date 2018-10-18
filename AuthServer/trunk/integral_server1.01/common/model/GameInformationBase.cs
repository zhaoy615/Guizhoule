using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.model
{
  public  class GameInformationBase
    {
        /// <summary>
        /// 基础协议号
        /// </summary>
        public const int BASEAGREEMENTNUMBER = 10000;
        /// <summary>
        /// 自动解散牌桌等待时间 毫秒
        /// </summary>
        public const int DISBANDTABLECOUNTDOWN= 10000;

        /// <summary>
        /// 距离检查的距离
        /// </summary>
        public const int DISTANCE =3;
        /// <summary>
        /// 系统托管的时间 毫秒
        /// </summary>
        public const int SYSTEMHOSTING = 10000;
        /// <summary>
        /// 公共用户集合
        /// 用来区分用户所在list服务器
        /// </summary>
        public const string COMMUNITYUSERLIST = "Community:UserList";
        /// <summary>
        /// 公共用户集合
        /// 用来区分用户所在game服务器
        /// </summary>
        public const string COMMUNITYUSERGAME = "Community:UserGame";
        /// <summary>
        /// 公共用户信息集合
        /// 将用户信息保存至缓存服务器
        /// </summary>
        public const string COMMUNITYUSERINFO = "Community:UserInfo";
        /// <summary>
        /// 公共房间信息集合
        /// 将房间信息保存至缓存服务器
        /// </summary>
        public const string COMMUNITYROOMINFO = "Community:RoomInfo";
        /// <summary>
        /// 防止有人恶意调用解散房间 保存一个点击解散时间    
        /// </summary>
        public const string COMMUNITYDISBANDED = "Community:Disbanded";
        /// <summary>
        /// 默认游戏服务器名称
        /// </summary>
        public const string DEFAULTGAMESERVERNAME = "GameServer";


        /// <summary>
        /// 服务器名称
        /// </summary>
        public static string serverName = ConfigurationManager.AppSettings["ServerName"];

    }
}
