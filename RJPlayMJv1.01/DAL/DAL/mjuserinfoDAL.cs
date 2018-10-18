using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using DAL.Model;
using DAL.DBHelp;

namespace DAL.DAL
{
    /// <summary>
    /// 数据访问类:mjuserinfo
    /// </summary>
    public partial class mjuserinfoDAL
    {
        public mjuserinfoDAL()
        { }
        #region  BasicMethod

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(long id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from mjuserinfo");
            strSql.Append(" where id=@id ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@id", MySqlDbType.Int64,8)          };
            parameters[0].Value = id;

            return DbHelperMySQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(mjuserinfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into mjuserinfo(");
            strSql.Append("id,openid,nickname,headimg,userphone,sex,province,city,unionid,is_band,addtime,band_time)");
            strSql.Append(" values (");
            strSql.Append("@id,@openid,@nickname,@headimg,@userphone,@sex,@province,@city,@unionid,@is_band,@addtime,@band_time)");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@id", MySqlDbType.Int64,8),
                    new MySqlParameter("@openid", MySqlDbType.VarChar,200),
                    new MySqlParameter("@nickname", MySqlDbType.VarChar,200),
                    new MySqlParameter("@headimg", MySqlDbType.VarChar,200),
                    new MySqlParameter("@userphone", MySqlDbType.VarChar,11),
                    new MySqlParameter("@sex", MySqlDbType.Int32,1),
                    new MySqlParameter("@province", MySqlDbType.VarChar,200),
                    new MySqlParameter("@city", MySqlDbType.VarChar,200),
                    new MySqlParameter("@unionid", MySqlDbType.VarChar,200),
                    new MySqlParameter("@is_band", MySqlDbType.Int32,1),
                    new MySqlParameter("@addtime", MySqlDbType.DateTime),
                    new MySqlParameter("@band_time", MySqlDbType.DateTime)};
            parameters[0].Value = model.id;
            parameters[1].Value = model.openid;
            parameters[2].Value = model.nickname;
            parameters[3].Value = model.headimg;
            parameters[4].Value = model.userphone;
            parameters[5].Value = model.sex;
            parameters[6].Value = model.province;
            parameters[7].Value = model.city;
            parameters[8].Value = model.unionid;
            parameters[9].Value = model.is_band;
            parameters[10].Value = model.addtime;
            parameters[11].Value = model.band_time;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(mjuserinfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update mjuserinfo set ");
            strSql.Append("openid=@openid,");
            strSql.Append("nickname=@nickname,");
            strSql.Append("headimg=@headimg,");
            strSql.Append("userphone=@userphone,");
            strSql.Append("sex=@sex,");
            strSql.Append("province=@province,");
            strSql.Append("city=@city,");
            strSql.Append("unionid=@unionid,");
            strSql.Append("is_band=@is_band,");
            strSql.Append("addtime=@addtime,");
            strSql.Append("band_time=@band_time");
            strSql.Append(" where id=@id ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@openid", MySqlDbType.VarChar,200),
                    new MySqlParameter("@nickname", MySqlDbType.VarChar,200),
                    new MySqlParameter("@headimg", MySqlDbType.VarChar,200),
                    new MySqlParameter("@userphone", MySqlDbType.VarChar,11),
                    new MySqlParameter("@sex", MySqlDbType.Int32,1),
                    new MySqlParameter("@province", MySqlDbType.VarChar,200),
                    new MySqlParameter("@city", MySqlDbType.VarChar,200),
                    new MySqlParameter("@unionid", MySqlDbType.VarChar,200),
                    new MySqlParameter("@is_band", MySqlDbType.Int32,1),
                    new MySqlParameter("@addtime", MySqlDbType.DateTime),
                    new MySqlParameter("@band_time", MySqlDbType.DateTime),
                    new MySqlParameter("@id", MySqlDbType.Int64,8)};
            parameters[0].Value = model.openid;
            parameters[1].Value = model.nickname;
            parameters[2].Value = model.headimg;
            parameters[3].Value = model.userphone;
            parameters[4].Value = model.sex;
            parameters[5].Value = model.province;
            parameters[6].Value = model.city;
            parameters[7].Value = model.unionid;
            parameters[8].Value = model.is_band;
            parameters[9].Value = model.addtime;
            parameters[10].Value = model.band_time;
            parameters[11].Value = model.id;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(long id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from mjuserinfo ");
            strSql.Append(" where id=@id ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@id", MySqlDbType.Int64,8)          };
            parameters[0].Value = id;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string idlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from mjuserinfo ");
            strSql.Append(" where id in (" + idlist + ")  ");
            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public mjuserinfo GetModel(long id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,openid,nickname,headimg,userphone,sex,province,city,unionid,is_band,addtime,band_time from mjuserinfo ");
            strSql.Append(" where id=@id ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@id", MySqlDbType.Int64,8)          };
            parameters[0].Value = id;

            mjuserinfo model = new mjuserinfo();
            DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public mjuserinfo GetModel(string openid)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,openid,nickname,headimg,userphone,sex,province,city,unionid,is_band,addtime,band_time from mjuserinfo ");
            strSql.Append(" where openid=@openid ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@openid", MySqlDbType.VarChar,200)          };
            parameters[0].Value = openid;

            mjuserinfo model = new mjuserinfo();
            DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public mjuserinfo DataRowToModel(DataRow row)
        {
            mjuserinfo model = new mjuserinfo();
            if (row != null)
            {
                if (row["id"] != null && row["id"].ToString() != "")
                {
                    model.id = long.Parse(row["id"].ToString());
                }
                if (row["openid"] != null)
                {
                    model.openid = row["openid"].ToString();
                }
                if (row["nickname"] != null)
                {
                    model.nickname = row["nickname"].ToString();
                }
                if (row["headimg"] != null)
                {
                    model.headimg = row["headimg"].ToString();
                }
                if (row["userphone"] != null)
                {
                    model.userphone = row["userphone"].ToString();
                }
                if (row["sex"] != null && row["sex"].ToString() != "")
                {
                    model.sex = int.Parse(row["sex"].ToString());
                }
                if (row["province"] != null)
                {
                    model.province = row["province"].ToString();
                }
                if (row["city"] != null)
                {
                    model.city = row["city"].ToString();
                }
                if (row["unionid"] != null)
                {
                    model.unionid = row["unionid"].ToString();
                }
                if (row["is_band"] != null && row["is_band"].ToString() != "")
                {
                    model.is_band = int.Parse(row["is_band"].ToString());
                }
                if (row["addtime"] != null && row["addtime"].ToString() != "")
                {
                    model.addtime = DateTime.Parse(row["addtime"].ToString());
                }
                if (row["band_time"] != null && row["band_time"].ToString() != "")
                {
                    model.band_time = DateTime.Parse(row["band_time"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,openid,nickname,headimg,userphone,sex,province,city,unionid,is_band,addtime,band_time ");
            strSql.Append(" FROM mjuserinfo ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM mjuserinfo ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            object obj = DbHelperMySQL.GetSingle(strSql.ToString());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.id desc");
            }
            strSql.Append(")AS Row, T.*  from mjuserinfo T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取最大ID，如无则获取默认值
        /// </summary>
        /// <returns></returns>
        public int GetMaxID()
        {
            string sql = "select MAX(id) as Bid from mjuserinfo";
            DataSet ds = DbHelperMySQL.Query(sql);
            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Bid"].ToString()))
            {
                return int.Parse(ds.Tables[0].Rows[0]["Bid"].ToString());
            }
            else
            {
                return 100000;
            }
        }

        /*
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		{
			MySqlParameter[] parameters = {
					new MySqlParameter("@tblName", MySqlDbType.VarChar, 255),
					new MySqlParameter("@fldName", MySqlDbType.VarChar, 255),
					new MySqlParameter("@PageSize", MySqlDbType.Int32),
					new MySqlParameter("@PageIndex", MySqlDbType.Int32),
					new MySqlParameter("@IsReCount", MySqlDbType.Bit),
					new MySqlParameter("@OrderType", MySqlDbType.Bit),
					new MySqlParameter("@strWhere", MySqlDbType.VarChar,1000),
					};
			parameters[0].Value = "mjuserinfo";
			parameters[1].Value = "id";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperMySQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

        #endregion  BasicMethod
        #region  ExtensionMethod

        #endregion  ExtensionMethod
    }
}

