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
    public class PackageDAL
    {

       

        public List<Package> GetPackage(string openid)
        {
            using (var Conn= new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = @" select a.* , b.prizeName, b.prizeImage, b.prizeDetails from package as a, prize as b where a.openid= @openid and b.id= a.PrizeID";
                return Conn.Query<Package>(sql, new { openid = openid }).ToList<Package>();
            }
        }
    }
}
