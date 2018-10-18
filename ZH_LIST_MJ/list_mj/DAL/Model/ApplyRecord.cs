using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   public class ApplyRecord
    {
        public long id { get; set; }
        public long GroupID { get; set; }

        public string GroupName { get; set; }
        public long ApplyJoinUserID { get; set; }

        public DateTime ApplyDateTime { get; set; }

        public int ApplyStatus { get; set; }
    }
}
