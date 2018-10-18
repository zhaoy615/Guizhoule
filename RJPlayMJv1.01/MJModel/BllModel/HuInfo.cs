using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJModel.BllModel
{
    /// <summary>
    /// 胡牌信息
    /// </summary>
   public class HuInfo
    {
        /// <summary>
        /// 胡牌方位
        /// </summary>
        public int HuIndex { get; set; }

        /// <summary>
        /// 放炮方位
        /// </summary>
        public int PaoIndex { get; set; }
        /// <summary>
        /// 胡牌类型
        /// (1:自摸；2:胡牌；3:抢杠；4:热炮;5:杠上开花)
        /// </summary>
        public int HuType { get; set; }
        /// <summary>
        /// 胡的那张牌
        /// </summary>
        public MaJiang MJ { get; set; }
        /// <summary>
        /// 胡的用户OpenID
        /// </summary>
        public string OpenID { get; set; }
        /// <summary>
        /// 胡牌的房间号
        /// </summary>
        public int RoomID { get; set; }
    }
}
