using DAL.DBHelp;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DAL
{
    public class GroupInfoDAL
    {
        /// <summary>
        /// 根据groupid来获取userid
        /// </summary>
        /// <returns></returns>
        public long GetUserIDByGuoupID(long GroupID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = @"select CreateUserID from groupinfo_tb where GroupID=@GroupID;";
                //string sql = @"select GroupID from groupinfo_tb where CreateUserID=@CreateUserID and isExist=0";
                return Conn.QueryFirstOrDefault<long>(sql, new { GroupID = GroupID });
            }
        }

        public int AddCreateRoomRecord(long CreatUserID, long GuoupID, long RoomID, int UserRoomCard)
        {

            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "INSERT group_log (GuoupID,RoomID,CreatUserID,CreateDate,UserRoomCard) VALUES(@GuoupID,@RoomID,@CreatUserID,@CreateDate,@UserRoomCard)";
                return Conn.Execute(sql, new { GuoupID = GuoupID, RoomID = RoomID, CreateUserID = CreatUserID, CreateDate = DateTime.Now, UserRoomCard = UserRoomCard});
            }

        }
    }
}
