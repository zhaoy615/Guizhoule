using MJBLL.common;
using System;

namespace MJBLL.model
{
    public class UserInfo
    {
        /// <summary>
        /// 用户openid
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 性别(1：男，2：女)
        /// </summary>
        public string sex { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string headimg { get; set; }
        /// <summary>
        /// 唯一标示符
        /// </summary>
        public string unionid { get; set; }
        /// <summary>
        /// session
        /// </summary>
        public GameSession session { get; set; }


        /// <summary>
        /// 断开连接时间
        /// </summary>
        public DateTime ConnTime { get; set; }



        public string UserIP { get; set; }


        /// <summary>
        /// 经纬度
        /// </summary>
        public string Lat { get; set; }
        /// <summary>
        /// 是活跃用户
        /// </summary>
        public bool IsActive { get; set; }

        public long UserID { get; set; }
        public UserInfo()
        { }
        public UserInfo(RedisUserInfoModel userInfo)
        {
            openid = userInfo.Openid;
            nickname = userInfo.Nickname;
            sex = userInfo.Sex;
            province = userInfo.Province;
            city = userInfo.City;
            headimg = userInfo.Headimg;
            unionid = userInfo.Unionid;
            ConnTime = userInfo.ConnTime;
            UserIP = userInfo.UserIP;
            Lat = userInfo.Lat;
            UserID = userInfo.UserID;
        }

    }
}
