using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.model
{
    public class RoomMsgWirte
    {

        public string openid { get; set; }

        public int xiaoxihao { get; set; }

        public byte[] ArrList { get; set; }


        public int roomid { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string operating{ get; set; }
    }
}
