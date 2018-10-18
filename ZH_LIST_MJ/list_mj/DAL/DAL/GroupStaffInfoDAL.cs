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
   public class GroupStaffInfoDAL
    {
       // IDbConnection conn = new MySqlConnection(DbHelperMySQL.connectionString);
   
        public IEnumerable<int> GetGroupIDByUserID(long userID)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select a.GroupID from groupstaffinfo_tb a,groupinfo_tb b where a.GroupID=b.GroupID and b.isExist=0 and a.GroupUserID=@GroupUserID";
                return Conn.Query<int>(sql, new { GroupUserID = userID });
            }
        }
    }
}
