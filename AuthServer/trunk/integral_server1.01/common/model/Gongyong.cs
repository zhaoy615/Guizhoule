using DAL.Model;
using MJBLL.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.model
{
    public class Gongyong
    {
        /// <summary>
        /// 房间集合
        /// </summary>
        public static List<Room> roomlist = new List<Room>();

        /// <summary>
        /// 用户集合
        /// </summary>
        public static List<UserInfo> userlist = new List<UserInfo>();

        /// <summary>
        /// 玩家操作集合
        /// </summary>
        public static List<mjuser> mulist = new List<mjuser>();
        /// <summary>
        /// 房卡麻将记分类
        /// </summary>
        public static List<CardUser> FKUser = new List<CardUser>();

        /// <summary>
        /// 消息重发类
        /// </summary>
        public static List<MsgLog> msg = new List<MsgLog>();


        /// <summary>
        /// 
        /// </summary>
        public static List<RoomClear> roomclear = new List<RoomClear>();

        /// <summary>
        /// 
        /// </summary>
        public static List<Room_JX> room_JX = new List<Room_JX>();

        /// <summary>
        /// 
        /// </summary>
        public static List<RoomMsgWirte> roommsg = new List<RoomMsgWirte>();
    }
}
