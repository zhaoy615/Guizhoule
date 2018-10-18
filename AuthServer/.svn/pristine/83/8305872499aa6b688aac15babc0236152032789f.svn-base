using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class RoomInfo
    {
        /// <summary>
        ///房间信息ID
        /// </summary>
        public string RoomInfoID { get; set; }
        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 是否本鸡
        /// </summary>
        public int IsBenJi { get; set; }
        /// <summary>
        /// 是否乌骨鸡
        /// </summary>
        public int IsWGJ { get; set; }
        /// <summary>
        /// 是否是星期鸡
        /// </summary>
        public int IsXinQiJi { get; set; }

        /// <summary>
        /// 是否上下鸡
        /// </summary>
        public int IsSangXiaJi { get; set; }
        /// <summary>
        /// 算分模式0为一扣二1为一扣三3为连庄
        /// </summary>
        public int CountPointsType { get; set; }
        /// <summary>
        /// 房间人数
        /// </summary>
        public int RoomPeo { get; set; }
        /// <summary>
        /// 房间游戏局数
        /// </summary>
        public int RoomNumber { get; set; }
        /// <summary>
        /// 是否原缺，1为是 0为否
        /// </summary>
        public int IsYuanQue { get; set; }
        /// <summary>
        /// 快速出牌， 1为是0为否
        /// </summary>
        public int QuickCard { get; set; }
        /// <summary>
        /// 房间创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建房间的用户ID
        /// </summary>
        public long CreateUserID { get; set; }
        /// <summary>
        /// 房间结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 房间结束状态 0为正常结束，1为房主解散房间，2为房主退出解散房间，3为全部同意解散房间
        /// </summary>
        public int EndStatus { get; set; }
        /// <summary>
        /// 房间结算 格式UserID,Score|UserID,Score
        /// </summary>
        public string UserWinLose { get; set; }
      
    }
}
