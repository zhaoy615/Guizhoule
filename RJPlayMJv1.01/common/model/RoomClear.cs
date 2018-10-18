using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.model
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomClear
    {
        public int RoomID { get; set; }


        public List<string> RoomPeo = new List<string>();
        /// <summary>
        /// 状态
        /// 1为可解散 0为不可解散
        /// </summary>
        public int Status { get; set; }


    }

}
