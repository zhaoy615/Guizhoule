using System;
namespace DAL.Model
{
	/// <summary>
	/// login_log:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class login_log
	{
		public login_log()
		{}
		#region Model
		private string _id;
		private string _openid;
		private DateTime _login_time;
		private int? _login_state;
      public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string openid
		{
			set{ _openid=value;}
			get{return _openid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime login_time
		{
			set{ _login_time=value;}
			get{return _login_time;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? login_state
		{
			set{ _login_state=value;}
			get{return _login_state;}
		}
		#endregion Model

	}
}

