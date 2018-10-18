using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
  public  class Package
    {
        /// <summary>
        /// 用户openID
        /// </summary>
        public int OpenID { get; set; }

        /// <summary>
        /// 奖品ID
        /// </summary>
        public int PrizeID { get; set; }

        /// <summary>
        /// 奖品数量
        /// </summary>
        public int PrizeCounts { get; set; }

        /// <summary>
        /// 奖品名称
        /// </summary>
        public string prizeName { get; set; }

        /// <summary>
        /// 奖品图标
        /// </summary>
        public string prizeImage { get; set; }

        /// <summary>
        /// 奖品详情
        /// </summary>
        public string prizeDetails { get; set; }

    }
}
