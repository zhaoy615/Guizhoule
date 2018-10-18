using System;

namespace MJBLL.model
{
    /// <summary>
    /// redis存储登录信息对象
    /// </summary>
    public  class RedisLoginModel
    {
        /// <summary>
        /// 用户openid
        /// </summary>
        public string Openid { get; set; }
        /// <summary>
        /// 唯一标示符
        /// </summary>
        public string Unionid { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; }
    }
    /// <summary>
    /// redis存储用户信息对象
    /// </summary>
    public class RedisUserInfoModel
    {
        /// <summary>
        /// 用户openid
        /// </summary>
        public string Openid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 性别(1：男，2：女)
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Headimg { get; set; }
        /// <summary>
        /// 唯一标示符
        /// </summary>
        public string Unionid { get; set; }
        /// <summary>
        /// 断开连接时间
        /// </summary>
        public DateTime ConnTime { get; set; }
        public string UserIP { get; set; }
        /// <summary>
        /// 经纬度
        /// </summary>
        public string Lat { get; set; }


        public long UserID { get; set; }
        public RedisUserInfoModel(UserInfo userinfo)
        {
            Openid = userinfo.openid;
            Nickname = userinfo.nickname;
            Sex = userinfo.sex;
            Province = userinfo.province;
            City = userinfo.city;
            Headimg = userinfo.headimg;
            Unionid = userinfo.unionid;
            ConnTime = userinfo.ConnTime;
            UserIP = userinfo.UserIP;
            Lat = userinfo.Lat;
            UserID = userinfo.UserID;
        }
        public RedisUserInfoModel()
        {
        }
    }
    /// <summary>
    /// redis存储用户游戏对象
    /// </summary>
    public class RedisGameModel
    {
        /// <summary>
        /// 用户openid
        /// </summary>
        public string Openid { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 房间号
        /// </summary>
        public int RoomID { get; set; }
        /// <summary>
        /// 唯一标示符
        /// </summary>
        public string Unionid { get; set; }
    }
    /// <summary>
    /// redis存储房间信息对象
    /// </summary>
    public class RedisGameRoomInfo
    {
        /// <summary>
        /// 房间信息
        /// </summary>
        public Room room { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; }
    }
}
