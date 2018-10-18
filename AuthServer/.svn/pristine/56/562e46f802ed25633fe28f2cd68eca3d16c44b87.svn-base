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
        IDbConnection conn = new MySqlConnection(DbHelperMySQL.connectionString);
        public int Add(boarddetailsTB boarddetailsInfo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO `boarddetailstb`(InningsID,RoomInfoID,GameSummary,MatchDetails)");
            strSql.Append(" VALUES(@InningsID, @RoomInfoID, @GameSummary, @MatchDetails)");


            //"insert into login_log(");
            // strSql.Append("id,openid,login_time,login_state)");
            // strSql.Append(" values (");
            // strSql.Append("@id,@openid,@login_time,@login_state)");
            return conn.Execute(strSql.ToString(), boarddetailsInfo);
        }

        /// <summary>
        /// 判断主键是否存在
        /// </summary>
        /// <param name="roomInfoID"></param>
        /// <returns></returns>
        public bool GetExistsByInningsIDID(string boarddetailsID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select COUNT(*) from `boarddetailstb` where InningsID=@InningsID ");
            return conn.Query<int>(strSql.ToString(), new { InningsID = boarddetailsID }).FirstOrDefault() >= 1;
        }
    }
}
