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
        IDbConnection conn = new MySqlConnection(DbHelperMySQL.connectionString);
        // strSql.Append("SELECT CAST(SCOPE_IDENTITY() as int)");返回自增列ID
        //var id = connection.Query<int>(sql, mytable).FirstOrDefault();返回自增列ID

        public int Add(RoomInfo roomInfo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO `roominfotb`(RoomInfoID,RoomID,IsBenJi,IsWGJ,IsXinQiJi,IsSangXiaJi,CountPointsType,RoomPeo,RoomNumber,CreateDate,CreateUserID,IsYuanQue,QuickCard)");
            strSql.Append(" VALUES(@RoomInfoID, @RoomID, @IsBenJi, @IsWGJ, @IsXinQiJi, @IsSangXiaJi,@CountPointsType, @RoomPeo,@RoomNumber, @CreateDate,@CreateUserID,@IsYuanQue,@QuickCard)");


            //"insert into login_log(");
            // strSql.Append("id,openid,login_time,login_state)");
            // strSql.Append(" values (");
            // strSql.Append("@id,@openid,@login_time,@login_state)");
            return conn.Execute(strSql.ToString(), roomInfo);
        }

        /// <summary>
        /// 判断主键是否存在
        /// </summary>
        /// <param name="roomInfoID"></param>
        /// <returns></returns>
        public bool GetExistsByRoomInfoID(string roomInfoID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select COUNT(*) from `roominfotb` where RoomInfoID=@RoomInfoID ");
            return conn.QueryFirstOrDefault<int>(strSql.ToString(), new { RoomInfoID= roomInfoID } )>=1;
        }
        /// <summary>
        /// 修改房间结束信息
        /// </summary>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        public int UpdateEndRoomInfoByRoomInfoID(RoomInfo roomInfo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("UPDATE roominfotb set EndTime=@EndTime,EndStatus=@EndStatus,UserWinLose=@UserWinLose where RoomInfoID = @RoomInfoID");


            //"insert into login_log(");
            // strSql.Append("id,openid,login_time,login_state)");
            // strSql.Append(" values (");
            // strSql.Append("@id,@openid,@login_time,@login_state)");
            return conn.Execute(strSql.ToString(), roomInfo);
        }
    }
}
