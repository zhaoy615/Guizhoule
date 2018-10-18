using UnityEngine;
using System.Collections;
using System;

namespace cn.sharesdk.unity3d 
{
	[Serializable]
	public class DevInfoSet
	{
		
		public WeChat wechat;
		public WeChatMoments wechatMoments; 
		public WeChatFavorites wechatFavorites;
	
	}

	public class DevInfo 
	{	
		public bool Enable = true;
	}


	

	[Serializable]
	public class QQ : DevInfo 
	{
		#if UNITY_ANDROID
		public const int type = (int) PlatformType.QQ;
		public string SortId = "2";
		public string AppId = "100371282";
		public string AppKey = "aed9b0303e3ed1e27bae87c33761161d";
		public bool ShareByAppClient = true;
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.QQ;
		public string app_id = "100371282";
		public string app_key = "aed9b0303e3ed1e27bae87c33761161d";
		public string auth_type = "both";  //can pass "both","sso",or "web" 
		#endif
	}

	[Serializable]
	public class QZone : DevInfo 
	{
		#if UNITY_ANDROID
		public string SortId = "1";
		public const int type = (int) PlatformType.QZone;
		public string AppId = "100371282";
		public string AppKey = "ae36f4ee3946e1cbb98d6965b0b2ff5c";
		public bool ShareByAppClient = true;
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.QZone;
		public string app_id = "100371282";
		public string app_key = "aed9b0303e3ed1e27bae87c33761161d";
		public string auth_type = "both";  //can pass "both","sso",or "web" 
		#endif
	}
	

	
	[Serializable]
	public class WeChat : DevInfo 
	{	
		#if UNITY_ANDROID
		public string SortId = "5";
		public const int type = (int) PlatformType.WeChat;
		public string AppId = "wxb83585f13841307a";
		public string AppSecret = "0b492027eb6bfccc0dbbb10c4f339b02";
		public string userName = "gh_afb25ac019c9@app";
		public string path = "/page/API/pages/share/share";
		public bool BypassApproval = false;
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.WeChat;
		public string app_id = "wx4868b35061f87885";
		public string app_secret = "64020361b8ec4c99936c0e3999a9f249";
		#endif
	}

	[Serializable]
	public class WeChatMoments : DevInfo 
	{
		#if UNITY_ANDROID
		public string SortId = "6";
		public const int type = (int) PlatformType.WeChatMoments;
		public string AppId = "wxb83585f13841307a";
		public string AppSecret = "0b492027eb6bfccc0dbbb10c4f339b02";
		public bool BypassApproval = false;
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.WeChatMoments;
		public string app_id = "wx4868b35061f87885";
		public string app_secret = "64020361b8ec4c99936c0e3999a9f249";
		#endif
	}

	[Serializable]
	public class WeChatFavorites : DevInfo 
	{
		#if UNITY_ANDROID
		public string SortId = "7";
		public const int type = (int) PlatformType.WeChatFavorites;
		public string AppId = "wxb83585f13841307a";
		public string AppSecret = "0b492027eb6bfccc0dbbb10c4f339b02";
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.WeChatFavorites;
		public string app_id = "wx4868b35061f87885";
		public string app_secret = "64020361b8ec4c99936c0e3999a9f249";
		#endif
	}




	[Serializable]
	public class WechatSeries : DevInfo 
	{	
		#if UNITY_ANDROID
		//for android,please set the configuraion in class "Wechat" ,class "WechatMoments" or class "WechatFavorite"
		//对于安卓端，请在类Wechat,WechatMoments或WechatFavorite中配置相关信息↑	
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.WechatPlatform;
		public string app_id = "wx4868b35061f87885";
		public string app_secret = "64020361b8ec4c99936c0e3999a9f249";
		#endif
	}

	[Serializable]
	public class QQSeries : DevInfo 
	{	
		#if UNITY_ANDROID
		//for android,please set the configuraion in class "QQ" and  class "QZone"
		//对于安卓端，请在类QQ或QZone中配置相关信息↑	
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.QQPlatform;
		public string app_id = "100371282";
		public string app_key = "aed9b0303e3ed1e27bae87c33761161d";
		public string auth_type = "both";  //can pass "both","sso",or "web" 
		#endif
	}

}
