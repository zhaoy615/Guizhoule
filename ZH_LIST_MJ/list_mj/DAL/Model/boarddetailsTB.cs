using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   public class boarddetailsTB
    {
        /// <summary>
        /// 牌局详情ID
        /// </summary>
        public string InningsID { get; set; }
        /// <summary>
        /// 房间信息ID
        /// </summary>
        public string RoomInfoID { get; set; }
        /// <summary>
        /// 结算信息
        /// </summary>
        public byte[] GameSummary { get; set; }
        /// <summary>
        /// 游戏详情
        /// </summary>
        public byte[] MatchDetails { get; set; }
    }
}
