using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
 public  class IntegralInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string userID { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public decimal integral { get; set; }

        /// <summary>
        /// 消费卷
        /// </summary>
        public decimal coupons { get; set; }

        /// <summary>
        /// 房卡
        /// </summary>
        public decimal roomCard { get; set; }
    }
}
