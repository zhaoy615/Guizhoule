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
  public  class BoarddetailstbDAL
    {
       
        public int Add(boarddetailsTB boarddetailsInfo)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("INSERT INTO `boarddetailstb`(InningsID,RoomInfoID,GameSummary,MatchDetails)");
                strSql.Append(" VALUES(@InningsID, @RoomInfoID, @GameSummary, @MatchDetails)");


                //"insert into login_log(");
                // strSql.Append("id,openid,login_time,login_state)");
                // strSql.Append(" values (");
                // strSql.Append("@id,@openid,@login_time,@login_state)");
                return Conn.Execute(strSql.ToString(), boarddetailsInfo);
            }
        }

        /// <summary>
        /// 判断主键是否存在
        /// </summary>
        /// <param name="roomInfoID"></param>
        /// <returns></returns>
        public bool GetExistsByInningsIDID(string boarddetailsID)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select COUNT(*) from `boarddetailstb` where InningsID=@InningsID ");
                return Conn.Query<int>(strSql.ToString(), new { InningsID = boarddetailsID }).FirstOrDefault() >= 1;
            }
        }
        /// <summary>
        /// 根据牌桌信息ID获取 牌局回放信息
        /// </summary>
        /// <param name="roomInfoID"></param>
        /// <returns></returns>
        public IEnumerable< boarddetailsTB> GetListByRoomInfoID(string roomInfoID)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select InningsID,RoomInfoID,GameSummary,MatchDetails from `boarddetailstb` where RoomInfoID=@RoomInfoID ");
                return Conn.Query<boarddetailsTB>(strSql.ToString(), new { RoomInfoID = roomInfoID });
            }
        }
    }
}
