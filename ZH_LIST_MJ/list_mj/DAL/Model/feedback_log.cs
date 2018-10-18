using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    /// <summary>
    /// 用户反馈日志
    /// </summary>
   public class Feedback_log
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        public long UserID { set; get; }
         /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { set; get; }
       /// <summary>
        /// 游戏日志
        /// </summary>
        public string GameLog { set; get; }
         /// <summary>
        /// 日志添加时间
        /// </summary>
        public DateTime Datetime { set; get; }
        /// <summary>
        /// 截图
        /// </summary>
        public string image { set; get; }
    }
}
