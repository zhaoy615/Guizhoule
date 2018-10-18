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
       
        public int Add(userRecordlogLOG userRecordLOGInfo)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("INSERT INTO `userRecordlog`(UserID,RoomInfoID,CreateDate,EndTime,UserWinLose,RoomID)");
                strSql.Append(" VALUES(@UserID, @RoomInfoID, @CreateDate, @EndTime,@UserWinLose,@RoomID)");


                //"insert into login_log(");
                // strSql.Append("id,openid,login_time,login_state)");
                // strSql.Append(" values (");
                // strSql.Append("@id,@openid,@login_time,@login_state)");
                return Conn.Execute(strSql.ToString(), userRecordLOGInfo);
            }
        }

        /// <summary>
        /// 根据userID 获取用户48小时内战绩
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IEnumerable<userRecordlogLOG> Get48hourInfoByUserID(long userID,int groupid)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select u.UserID,u.RoomInfoID,u.CreateDate,u.EndTime,u.UserWinLose,u.RoomID
                            , r.CreateUserID, r.IsBenJi, r.IsSangXiaJi, r.IsWGJ, r.IsXinQiJi, r.IsYuanQue, r.QuickCard, r.RoomPeo, r.RoomNumber
                            from userRecordlog u, roominfotb r where u.RoomInfoID = r.RoomInfoID");
                strSql.Append(" and u.UserID=@UserID and u.EndTime>@EndTime ");
                if(groupid!=0)
                { 
                strSql.Append(" and r.GroupID=@GroupID");
                    return Conn.Query<userRecordlogLOG>(strSql.ToString(), new { UserID = userID, EndTime = DateTime.Now.AddHours(-48), GroupID= groupid });
                }
                else
                return Conn.Query<userRecordlogLOG>(strSql.ToString(), new { UserID = userID, EndTime = DateTime.Now.AddHours(-48) });
            }
        }


    }
}
