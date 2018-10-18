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
    public class RoomInfoDAL
    {
       
        // strSql.Append("SELECT CAST(SCOPE_IDENTITY() as int)");返回自增列ID
        //var id = connection.Query<int>(sql, mytable).FirstOrDefault();返回自增列ID

        public int Add(RoomInfo roomInfo)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("INSERT INTO `roominfotb`(RoomInfoID,RoomID,IsBenJi,IsWGJ,IsXinQiJi,IsSangXiaJi,CountPointsType,RoomPeo,RoomNumber,CreateDate,CreateUserID)");
                strSql.Append(" VALUES(@RoomInfoID, @RoomID, @IsBenJi, @IsWGJ, @IsXinQiJi, @IsSangXiaJi,@CountPointsType, @RoomPeo,@RoomNumber, @CreateDate,@CreateUserID)");


                //"insert into login_log(");
                // strSql.Append("id,openid,login_time,login_state)");
                // strSql.Append(" values (");
                // strSql.Append("@id,@openid,@login_time,@login_state)");
                return Conn.Execute(strSql.ToString(), roomInfo);
            }
        }

        /// <summary>
        /// 判断主键是否存在
        /// </summary>
        /// <param name="roomInfoID"></param>
        /// <returns></returns>
        public bool GetExistsByRoomInfoID(string roomInfoID)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select COUNT(*) from `roominfotb` where RoomInfoID=@RoomInfoID ");
                return Conn.Query<int>(strSql.ToString(), new { RoomInfoID = roomInfoID }).FirstOrDefault() >= 1;
            }
        }
        /// <summary>
        /// 修改房间结束信息
        /// </summary>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        public int UpdateEndRoomInfoByRoomInfoID(RoomInfo roomInfo)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("UPDATE roominfotb set EndTime=@EndTime,EndStatus=@EndStatus,UserWinLose=@UserWinLose where RoomInfoID = @RoomInfoID");


                //"insert into login_log(");
                // strSql.Append("id,openid,login_time,login_state)");
                // strSql.Append(" values (");
                // strSql.Append("@id,@openid,@login_time,@login_state)");
                return Conn.Execute(strSql.ToString(), roomInfo);
            }
        }
        /// <summary>
        /// 获取圈子中未结束的房间
        /// </summary>
        /// <param name="roomInfoID"></param>
        /// <returns></returns>
        public IEnumerable<long> GetroomInfoByGroupID(long groupID)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
               strSql.Append("select RoomID from `roominfotb` where GroupID=@GroupID and EndStatus is NULL and CreateDate>@CreateDate");
               //  strSql.Append("select RoomID from `roominfotb` where  EndStatus is NULL and CreateDate>@CreateDate");
                return Conn.Query<long>(strSql.ToString(), new { GroupID = groupID, CreateDate = DateTime.Now.AddHours(-24) });
            }
        }
    }
}
