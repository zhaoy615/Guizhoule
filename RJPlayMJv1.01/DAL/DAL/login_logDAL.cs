using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using DAL.Model;
using DAL.DBHelp;
namespace DAL.DAL
{
	/// <summary>
	/// 数据访问类:login_log
	/// </summary>
	public partial class login_logDAL
	{
		public login_logDAL()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperMySQL.GetMaxID("id", "login_log"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from login_log");
			strSql.Append(" where id=@id ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32,10)			};
			parameters[0].Value = id;

			return DbHelperMySQL.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(login_log model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into login_log(");
			strSql.Append("id,openid,login_time,login_state)");
			strSql.Append(" values (");
			strSql.Append("@id,@openid,@login_time,@login_state)");
			MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32,10),
					new MySqlParameter("@openid", MySqlDbType.VarChar,100),
					new MySqlParameter("@login_time", MySqlDbType.DateTime),
					new MySqlParameter("@login_state", MySqlDbType.Int32,10)};
			parameters[0].Value = model.id;
			parameters[1].Value = model.openid;
			parameters[2].Value = model.login_time;
			parameters[3].Value = model.login_state;

			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
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
		public bool Update(login_log model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update login_log set ");
			strSql.Append("openid=@openid,");
			strSql.Append("login_time=@login_time,");
			strSql.Append("login_state=@login_state");
			strSql.Append(" where id=@id ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@openid", MySqlDbType.VarChar,100),
					new MySqlParameter("@login_time", MySqlDbType.DateTime),
					new MySqlParameter("@login_state", MySqlDbType.Int32,10),
					new MySqlParameter("@id", MySqlDbType.Int32,10)};
			parameters[0].Value = model.openid;
			parameters[1].Value = model.login_time;
			parameters[2].Value = model.login_state;
			parameters[3].Value = model.id;

			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
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
		public bool Delete(int id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from login_log ");
			strSql.Append(" where id=@id ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32,10)			};
			parameters[0].Value = id;

			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
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
		public bool DeleteList(string idlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from login_log ");
			strSql.Append(" where id in ("+idlist + ")  ");
			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString());
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
		public login_log GetModel(int id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select id,openid,login_time,login_state from login_log ");
			strSql.Append(" where id=@id ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@id", MySqlDbType.Int32,10)			};
			parameters[0].Value = id;

			login_log model=new login_log();
			DataSet ds=DbHelperMySQL.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				return DataRowToModel(ds.Tables[0].Rows[0]);
			}
			else
			{
				return null;
			}
		}

        /// <summary>
        /// 获取最大ID，如无则获取默认值
        /// </summary>
        /// <returns></returns>
        public int GetMaxID()
        {
            string sql = "select MAX(id) as Bid from login_log";
            DataSet ds = DbHelperMySQL.Query(sql);
            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Bid"].ToString()))
            {
                return int.Parse(ds.Tables[0].Rows[0]["Bid"].ToString());
            }
            else
            {
                return 1;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public login_log DataRowToModel(DataRow row)
		{
			login_log model=new login_log();
			if (row != null)
			{
				if(row["id"]!=null && row["id"].ToString()!="")
				{
					model.id=int.Parse(row["id"].ToString());
				}
				if(row["openid"]!=null)
				{
					model.openid=row["openid"].ToString();
				}
				if(row["login_time"]!=null && row["login_time"].ToString()!="")
				{
					model.login_time=DateTime.Parse(row["login_time"].ToString());
				}
				if(row["login_state"]!=null && row["login_state"].ToString()!="")
				{
					model.login_state=int.Parse(row["login_state"].ToString());
				}
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select id,openid,login_time,login_state ");
			strSql.Append(" FROM login_log ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperMySQL.Query(strSql.ToString());
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) FROM login_log ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
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
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.id desc");
			}
			strSql.Append(")AS Row, T.*  from login_log T ");
			if (!string.IsNullOrEmpty(strWhere.Trim()))
			{
				strSql.Append(" WHERE " + strWhere);
			}
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
			return DbHelperMySQL.Query(strSql.ToString());
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
			parameters[0].Value = "login_log";
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

