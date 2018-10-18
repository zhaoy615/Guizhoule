using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MJBLL.model
{
   public interface IMyTimer
    {
         string Name { get; set; }
         Timer MyTimer { get; set; }

        /// <summary>
        /// 初始化

        /// </summary>
        void Initialization();
        /// <summary>
        /// 开始线程， 
        /// </summary>
        void Start();
        /// <summary>
        /// 回调方法
        /// </summary>
        /// <param name="state"></param>
        void CallBack(object state);
      
    }
}
