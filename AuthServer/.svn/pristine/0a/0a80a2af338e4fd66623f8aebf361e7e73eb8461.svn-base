using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJModel.BllModel
{

    /// <summary>
    /// 用户结算
    /// </summary>
    public class UserSettle
    {

        /// <summary>
        /// 用户OPENID
        /// </summary>
        public string  openid  { get; set; }

        /// <summary>
        /// 鸡牌和对应分数
        /// </summary>
        public List<UserJPOne> jp = new List<UserJPOne>();


        /// <summary>
        /// 杠牌和对应分数
        /// </summary>
        public List<UserGangJS> gang = new List<UserGangJS>();

        /// <summary>
        /// 合计分数
        /// </summary>
        public int scare { get; set; }

        /// <summary>
        /// 是否叫牌
        /// </summary>
        public int is_jiao { get; set; }

        /// <summary>
        /// 牌型
        /// </summary>
        public int pai_type { get; set; }

        /// <summary>
        /// 胡牌分数
        /// </summary>
        public int dy_fs { get; set; }

    }



    /// <summary>
    /// 用户鸡牌
    /// </summary>
    public class UserJPOne
    {

        /// <summary>
        /// 鸡牌花色
        /// </summary>
        public int PaiHS { get; set; }
        /// <summary>
        /// 鸡牌对应分数
        /// </summary>
        public int PaiScare { get; set; }
        /// <summary>
        /// 鸡牌类型
        /// </summary>
        public int type { get; set; }
    }

    /// <summary>
    /// 用户杠的数
    /// </summary>
    public class UserGangJS {
        /// <summary>
        /// 豆类型
        /// </summary>
        public int DType { get; set; }
        /// <summary>
        /// 豆对应分数
        /// </summary>
        public int DScare { get; set; }
    }
}
