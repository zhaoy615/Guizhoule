using MJBLL.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MJBLL.common
{
    public class TimerOut
    {
        public void TimeOutUser()
        {
            System.Threading.Timer timerClose = new System.Threading.Timer(new TimerCallback(timer_Elapsed), null, Timeout.Infinite, 36000);



        }

        /// <summary>
        /// 回调处理用户
        /// </summary>
        /// <param name="sender"></param>
        void timer_Elapsed(object sender)
        {

            Gongyong.userlist.RemoveAll(u => u.session.Connected == false);

        }


       

    }
}
