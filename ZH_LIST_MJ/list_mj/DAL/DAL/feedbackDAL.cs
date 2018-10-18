using DAL.DBHelp;
using DAL.Model;
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
   public class FeedbackDAL
    {
      
        public int Add(Feedback_log mode)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("INSERT INTO `feedback_log`(UserID,Title,Content,GameLog,Datetime,image)");
                strSql.Append(" VALUES(@UserID, @Title, @Content, @GameLog,@Datetime,@image)");


                //"insert into login_log(");
                // strSql.Append("id,openid,login_time,login_state)");
                // strSql.Append(" values (");
                // strSql.Append("@id,@openid,@login_time,@login_state)");
                return Conn.Execute(strSql.ToString(), mode);
            }
        }
    }
}
