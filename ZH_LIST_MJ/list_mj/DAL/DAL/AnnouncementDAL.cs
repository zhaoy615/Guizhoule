using DAL.DBHelp;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DAL.DAL
{
    public class AnnouncementDAL
    {
      
        /// <summary>
        /// 获取公告信息
        /// </summary>
        /// <returns></returns>
        public List<AnnouncementDALInfo> GetAnnouncement()
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select Title,Content from Announcement_Table where Status=1 and StartTime<=@StartTime and OverTime>=@OverTime";
                return Conn.Query<AnnouncementDALInfo>(sql, new { StartTime = DateTime.Now, OverTime = DateTime.Now }).ToList();
            }

        }
    }
    public class AnnouncementDALInfo
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

    }
}
