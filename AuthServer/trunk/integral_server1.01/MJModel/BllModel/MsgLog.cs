using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJModel.BllModel
{
    public class MsgLog
    {
        
        public string openid { get; set; }

        public int RoomID { get; set; }

        public ArraySegment<byte> msg { get; set; }
    }
}
