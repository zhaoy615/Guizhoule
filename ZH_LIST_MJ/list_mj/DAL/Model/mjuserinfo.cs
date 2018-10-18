using System;
using System.Web;

namespace DAL.Model
{
	/// <summary>
	/// mjuserinfo:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class mjuserinfo
	{
		public mjuserinfo()
		{}
		#region Model
		private long _id;
		private string _openid;
		private string _nickname;
		private string _headimg;
		private string _userphone;
		private int? _sex;
		private string _province;
		private string _city;
		private string _unionid;
		private int? _is_band;
		private DateTime? _addtime;
		private DateTime? _band_time;
        /// <summary>
        /// 旧头像地址
        /// </summary>
       public string Oldheadimg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long id
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
		public string nickname
		{
			set{ _nickname= HttpUtility.UrlEncode(value);}
			get{return _nickname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string headimg
		{
			set{ _headimg=value;}
			get{return _headimg;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string userphone
		{
			set{ _userphone=value;}
			get{return _userphone;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? sex
		{
			set{ _sex=value;}
			get{return _sex;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string province
		{
			set{ _province=value;}
			get{return _province;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string city
		{
			set{ _city=value;}
			get{return _city;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string unionid
		{
			set{ _unionid=value;}
			get{return _unionid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? is_band
		{
			set{ _is_band=value;}
			get{return _is_band;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? addtime
		{
			set{ _addtime=value;}
			get{return _addtime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? band_time
		{
			set{ _band_time=value;}
			get{return _band_time;}
		}
		#endregion Model

	}
}

