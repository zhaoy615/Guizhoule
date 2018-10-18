using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MJBLL.logic;

namespace MJBLL.common
{
   public  class ThreadUtility
    {
      
       static  List<CountdownLogic> countdownLogicList = new List<CountdownLogic>();
        static List<IsManagedLogic> managedLogicList = new List<IsManagedLogic>();
        public static object myObject = new object();
        /// <summary>
        /// 添加指定名字的删除牌桌时钟线程
        /// </summary>
        /// <param name="threadName"></param>
        public static bool AddCountdownRemoveRoomThread(string threadName)
        {
            CountdownLogic countdownLogic = new CountdownLogic();
            //if (ThreadList.Any(w => w.Name.Equals(threadName)))
            //    return false;
            //Thread thread = new Thread(new ThreadStart(countdownLogic.CountdownRemoveRoom));
            //thread.Name = threadName;
            //ThreadList.Add(thread);
            //return true;
            if (countdownLogicList.Any(w => w.Name.Equals(threadName)))
                return false;
            countdownLogic.Name = threadName;
       
            countdownLogicList.Add(countdownLogic);
            countdownLogic.Initialization();//启动计时器
            return true;

        }
        /// <summary>
        /// 开始线程， 删除牌桌操作需要等待90秒，所以开始后挂起90秒
        /// </summary>
        /// <param name="threadName"></param>
        /// <returns></returns>
        public static bool StartCountdownRemoveRoomThread(string threadName)
        {
           var info = countdownLogicList.First(w => w.Name.Equals(threadName));
            if (info == null)
                return false;
            info.Start();
            return true;
        }
        /// <summary>
        /// 删除线程
        /// </summary>
        /// <param name="threadName"></param>
        /// <returns></returns>
        public static bool RemoveCountdownRemoveRoomThread(string threadName)
        {
            var info = countdownLogicList.FirstOrDefault(w => w.Name.Equals(threadName));
            if (info == null)
                return false;
            countdownLogicList.Remove(info);
            info.MyTimer.Dispose();
            return true;
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="threadName"></param>
        /// <returns></returns>
        public static bool IsExist(string threadName)
        {
           return countdownLogicList.Any(w => w.Name.Equals(threadName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadName"></param>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public static bool StartManagedThread(string threadName, int roomID,string currentOperation)
        {
            IsManagedLogic ManagedLogic = new IsManagedLogic();
            string openID = threadName;
            threadName = "Managed:" + threadName;
            if (managedLogicList.Any(w => w.Name.Equals(threadName)))
            {
                var info = managedLogicList.Find(w => w.Name.Equals(threadName));
                info.currentOperation = currentOperation;
                info.Start();              
            }
            else
            {
                ManagedLogic.Name = threadName;
                ManagedLogic.openID = openID;
                ManagedLogic.roomID = roomID;
                ManagedLogic.currentOperation = currentOperation;
                managedLogicList.Add(ManagedLogic);
                ManagedLogic.Initialization();//启动计时器             
            }
            return true;
        }
        public static void Change(string threadName)
        {
            threadName = "Managed:" + threadName;
            var info = managedLogicList.FirstOrDefault(w => w.Name.Equals(threadName));
            if (info == null)
                return;
            info.Change();
        }
        public static bool RemoveManagedThread(string threadName)
        {
            threadName = "Managed:" + threadName;
            var info = managedLogicList.FirstOrDefault(w => w.Name.Equals(threadName));
            if (info == null)
                return false;
            info.Dispose();
            return true;
        }
        /// <summary>
        /// 根据房间ID 删除所有托管 
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public static bool RemoveManagedThreadByRoomID(int roomID)
        {
          
            var list = managedLogicList.FindAll(w => w.roomID== roomID);
            if (!list.Any())
                return false;
            foreach (var item in list)
            {
                item.Dispose();
            }
            managedLogicList.RemoveAll(w=>w.roomID== roomID);
            return true;
        }
    }
}
