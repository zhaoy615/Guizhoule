using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.model
{



    /// <summary>
    /// 单个用户鸡牌数和分值
    /// </summary>
    public class MJScore
    {
        public string OPENID { get; set; }


        public List<MJJP> listJP = new List<MJJP>();

        public List<MJGANG> listG = new List<MJGANG>();


        public int is_jiao { get; set; }


        public int H_type { get; set; }

        public int userScore { get; set; }
    }


    /// <summary>
    /// 鸡排和分数
    /// </summary>
    public class MJJP
    {


        /// <summary>
        /// 鸡排的花色
        /// </summary>
        public int HS { get; set; }
        /// <summary>
        /// 鸡排的分数
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 鸡排类型
        /// </summary>
        public int Jtype { get; set; }
    }

    /// <summary>
    /// 杠牌集合
    /// </summary>
    public class MJGANG
    {
        /// <summary>
        /// 杠牌类型
        /// </summary>
        public int Gtype { get; set; }
        /// <summary>
        /// 杠牌分数
        /// </summary>
        public int Score { get; set; }
    }
}
