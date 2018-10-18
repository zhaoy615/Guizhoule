using DAL.DBHelp;
using DAL.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DAL
{
   public class userRecordLOGDAL
    {
        IDbConnection conn = new MySqlConnection(DbHelperMySQL.connectionString);
        public int Add(userRecordlogLOG userRecordLOGInfo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO `userRecordlog`(UserID,RoomInfoID,CreateDate,EndTime,UserWinLose,RoomID)");
            strSql.Append(" VALUES(@UserID, @RoomInfoID, @CreateDate, @EndTime,@UserWinLose,@RoomID)");


            //"insert into login_log(");
            // strSql.Append("id,openid,login_time,login_state)");
            // strSql.Append(" values (");
            // strSql.Append("@id,@openid,@login_time,@login_state)");
            return conn.Execute(strSql.ToString(), userRecordLOGInfo);
        }

        /// <summary>
        /// 根据userID 获取用户48小时内战绩
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IEnumerable<userRecordlogLOG> Get48hourInfoByUserID(string userID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select UserID,RoomInfoID,CreateDate,EndTime,UserWinLose from  userRecordlog ");
            strSql.Append("where UserID=@UserID and EndTime>@EndTime");
            return conn.Query<userRecordlogLOG>(strSql.ToString(), new { UserID = userID, EndTime = DateTime.Now.AddHours(-48) });
        }


    }
}
