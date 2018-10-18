using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    /// <summary>
    /// 圈子成员信息
    /// </summary>
    public class GroupStaffInfo
    {
        public long GroupID { get; set; }
        public long GroupUserID { get; set; }
        public DateTime AddTime { get; set; }
        public int AddType { get; set; }
    }
}
