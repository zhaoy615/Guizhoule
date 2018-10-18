using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.model
{
    public class MsgLog
    {
        
        public string openid { get; set; }

        public ArraySegment<byte> msg { get; set; }
    }
}
