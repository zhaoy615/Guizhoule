using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
  public  class GroupInfo
    {
        public long GroupID { get; set; }

        public string GroupName { get; set; }

        public string GroupIntroduction { get; set; }

        public DateTime CreateTime { get; set; }

        public long CreateUserID { get; set; }

        public string NikeName { get; set; }
        public string CreateUserUnionID { get; set; }
    }
}
