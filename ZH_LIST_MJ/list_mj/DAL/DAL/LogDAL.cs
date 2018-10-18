using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using DAL.DBHelp;
using DAL.Model;

namespace DAL.DAL
{
    public class LogDAL
    {
       
        public int Add(login_log logInfo)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into login_log(");
                strSql.Append("id,openid,login_time,login_state,City)");
                strSql.Append(" values (");
                strSql.Append("@id,@openid,@login_time,@login_state,@City)");
                return Conn.Execute(strSql.ToString(), new { ID = logInfo.ID, openid = logInfo.openid, login_time = logInfo.login_time, login_state = logInfo.login_state, City= logInfo.City });
            }
        }
    }
}
