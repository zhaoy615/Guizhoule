using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DNL;
using MJBLL.common;
using System;
using System.Collections.Generic;
using System.Linq;

public class PengYQ : MonoBehaviour {
    private Transform Two_UI;


    private Button PengYQ_Btn;
    //-----------朋友圈页面对象
    private GameObject PengYQMain_Pel;
    private GameObject PengYQLobby_Pel;
    private GameObject PengYQUserManager_Pel;
    private GameObject PengYQApply_Pel;
    private GameObject PengYQApplyRecord_Pel;
    private GameObject PengYQJoin_Pel;
    private GameObject PengYQCreateRommRecord_Pel;
    private GameObject PengYQInfo_Pel;
    private GameObject PengYQUserExploits_Pel;
    //----------临时变量
    private long groupID = 0;
    private bool record;
    private int count = 0;
    private long userID = 0;
    private GroupInfo groupInfo;
    private FICEnterRoom enterRoom;
    private Manager_Hall hall;
    private Dictionary<string, GroupInfo> dicTemp = new Dictionary<string, GroupInfo>();

    void Start() {
        PengYQ_Btn = GetComponent<Button>();
        PengYQ_Btn.onClick.AddListener(delegate { SendGroupInfo(); });
        Two_UI = transform.parent.parent.Find("Two_UI");
		//我的朋友圈
        PengYQMain_Pel = Two_UI.Find("PengYQMain_Pel").gameObject;
		//朋友圈大厅
        PengYQLobby_Pel = Two_UI.Find("PengYQLobby_Pel").gameObject;
		//用户管理
        PengYQUserManager_Pel = Two_UI.Find("PengYQUserManager_Pel").gameObject;
		//用户申请记录
        PengYQApply_Pel = Two_UI.Find("PengYQApply_Pel").gameObject;
		//圈子申请记录
        PengYQApplyRecord_Pel = Two_UI.Find("PengYQApplyRecord_Pel").gameObject;
		//加入
        PengYQJoin_Pel = Two_UI.Find("PengYQJoin_Pel").gameObject;
		//创建房间记录
        PengYQCreateRommRecord_Pel = Two_UI.Find("PengYQCreateRommRecord_Pel").gameObject;
		//朋友圈信息
        PengYQInfo_Pel = Two_UI.Find("PengYQInfo_Pel").gameObject;
		//朋友圈战绩
        PengYQUserExploits_Pel = Two_UI.Find("PengYQUserExploits_Pel").gameObject;
		//加入房间的代码
        enterRoom = transform.Find("/Game_UI/Two_UI/Join_Pel").GetComponent<FICEnterRoom>();
        hall = transform.Find("/Main Camera").GetComponent<Manager_Hall>();
        AddButtonEvent();
    }
	/// <summary>
	/// 获取朋友圈信息
	/// </summary>
	void SendGroupInfo()
	{
		SendGroupInfo sendGroupInfo = new SendGroupInfo();
		sendGroupInfo.openid = GameInfo.OpenID;
		sendGroupInfo.unionid = GameInfo.unionid;
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGroupInfo);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1030, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}

	/// <summary>
	/// 添加朋友圈界面按键事件
	/// </summary>
	void AddButtonEvent()
	{
		PengYQJoin_Pel.transform.Find("btn/btn_num0").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(0); });
		PengYQJoin_Pel.transform.Find("btn/btn_num1").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(1); });
		PengYQJoin_Pel.transform.Find("btn/btn_num2").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(2); });
		PengYQJoin_Pel.transform.Find("btn/btn_num3").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(3); });
		PengYQJoin_Pel.transform.Find("btn/btn_num4").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(4); });
		PengYQJoin_Pel.transform.Find("btn/btn_num5").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(5); });
		PengYQJoin_Pel.transform.Find("btn/btn_num6").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(6); });
		PengYQJoin_Pel.transform.Find("btn/btn_num7").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(7); });
		PengYQJoin_Pel.transform.Find("btn/btn_num8").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(8); });
		PengYQJoin_Pel.transform.Find("btn/btn_num9").GetComponent<Button>().onClick.AddListener(delegate { InputPengYQID(9); });
		PengYQJoin_Pel.transform.Find("btn/btn_clear").GetComponent<Button>().onClick.AddListener(delegate { clearNum(); });
		PengYQJoin_Pel.transform.Find("btn/btn_backspace").GetComponent<Button>().onClick.AddListener(delegate { backspace(); });
		PengYQMain_Pel.transform.Find("close_Btn").GetComponent<Button>().onClick.AddListener(delegate { GameInfo.GroupID = 0; });

		//我的朋友圈的申请记录按钮
		PengYQMain_Pel.transform.Find("ApplyRecordBtn").GetComponent<Button>().onClick.AddListener(delegate {
			SendApplyRecord();
		});
		//我的朋友圈的申请加入按钮
		PengYQMain_Pel.transform.Find("ApplyJoinBtn").GetComponent<Button>().onClick.AddListener(delegate {
			PengYQJoin_Pel.transform.Find("GroupID/Placeholder").GetComponent<Text>().text = "请输入朋友圈ID...";
			PengYQJoin_Pel.transform.Find("headline/Text").GetComponent<Text>().text = "加入朋友圈";
			PengYQJoin_Pel.transform.Find("joinUserBtn").GetComponent<Button>().onClick.RemoveAllListeners();
			PengYQJoin_Pel.transform.Find("joinUserBtn").GetComponent<Button>().onClick.AddListener(delegate { PengYQJoin();
			});
		});
		//用户管理的加入用户按钮
		PengYQUserManager_Pel.transform.Find("JoinUserBtn").GetComponent<Button>().onClick.AddListener(delegate {
			PengYQJoin_Pel.transform.Find("GroupID/Placeholder").GetComponent<Text>().text = "请输入用户ID...";
			PengYQJoin_Pel.transform.Find("headline/Text").GetComponent<Text>().text = "加入用户";
			PengYQJoin_Pel.transform.Find("joinUserBtn").GetComponent<Button>().onClick.RemoveAllListeners();
			PengYQJoin_Pel.transform.Find("joinUserBtn").GetComponent<Button>().onClick.AddListener(delegate { PengYQJoinUser();
			}); ;
		});
		//用户管理的查看申请按钮
		PengYQUserManager_Pel.transform.Find("LookApplyBtn").GetComponent<Button>().onClick.AddListener(delegate {
			SendApplyRecord(groupID);
		});
		//用户管理的搜索按钮
		PengYQUserManager_Pel.transform.Find("SearchBtn").GetComponent<Button>().onClick.AddListener(delegate { SearchUser(PengYQUserManager_Pel.transform.Find("InputField").GetComponent<InputField>().textComponent.text, PengYQUserManager_Pel.transform.Find("Scroll View/Viewport/Content")); });
		//朋友圈大厅的关闭按钮
		PengYQLobby_Pel.transform.Find("close_Btn").GetComponent<Button>().onClick.AddListener(delegate { GameInfo.GroupID = 0; });
		//朋友圈大厅的朋友圈信息按钮
		PengYQLobby_Pel.transform.Find("LookInfoBtn").GetComponent<Button>().onClick.AddListener(delegate { SendGroupInfoByGroupID(); });
		//朋友圈大厅的朋友圈战绩按钮
		PengYQLobby_Pel.transform.Find("UserExploitsBtn").GetComponent<Button>().onClick.AddListener(delegate { SendGetUserRecord(); });
		//朋友圈信息的退出按钮
		PengYQInfo_Pel.transform.Find("Info_Pel/QuitBtn").GetComponent<Button>().onClick.AddListener(delegate { SendQuitGroup(); });
		//朋友圈信息的圈子用户按钮
		PengYQInfo_Pel.transform.Find("page/User_Tge").GetComponent<Toggle>().onValueChanged.AddListener(delegate { SendGroupUserInfoByGroupID(); });
		//朋友圈信息的基本信息按钮
		PengYQInfo_Pel.transform.Find("page/Info_Tge").GetComponent<Toggle>().onValueChanged.AddListener(delegate { SendGroupInfoByGroupID(); });


	}
	void InputPengYQID(int num)
	{
		string str = PengYQJoin_Pel.transform.Find("GroupID").GetComponent<InputField>().text;
		if (str.Length > 8) return;
		PengYQJoin_Pel.transform.Find("GroupID").GetComponent<InputField>().text += num;
	}

	//清空按钮
	void clearNum()
	{
		PengYQJoin_Pel.transform.Find("GroupID").GetComponent<InputField>().text = "";
	}
	//删除按钮
	void backspace()
	{
		string str = PengYQJoin_Pel.transform.Find("GroupID").GetComponent<InputField>().text;
		if (str.Length > 0)
		{
			PengYQJoin_Pel.transform.Find("GroupID").GetComponent<InputField>().text = str.Substring(0, str.Length - 1);
		}
	}
	/// <summary>
	/// 我的朋友圈的申请记录按钮和用户管理的查看申请按钮    查看申请记录
	/// </summary>
	void SendApplyRecord(long groupID = 0)
	{
		//用在Update()处
		record = groupID == 0 ?  false : true;
		SendApplyRecord sendApplyRecord = new SendApplyRecord();
		sendApplyRecord.UserID = (int)GameInfo.userID;
		sendApplyRecord.GroupID = groupID;
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendApplyRecord);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1060, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}

	/// <summary>
	/// 我的朋友圈的申请加入按钮 加入朋友圈
	/// </summary>
	void PengYQJoin()
	{
		int num = 0;
		if (int.TryParse(PengYQJoin_Pel.transform.Find("GroupID").GetComponent<InputField>().text, out num))
		{
			//请求加入圈子
			SendApplyToJoin(num);
			PengYQJoin_Pel.transform.Find("GroupID").GetComponent<InputField>().text = "";
			//获取朋友圈信息
			SendGroupInfo();
		}
		else
		{
			FICWaringPanel._instance.Show("输入朋友圈ID错误！");
		}
	}
	/// <summary>
	/// 请求加入圈子
	/// </summary>
	void SendApplyToJoin(int groupID)
	{
		SendApplyToJoin sendApplyToJoin = new SendApplyToJoin();
		sendApplyToJoin.GroupID = groupID;
		sendApplyToJoin.openid = GameInfo.OpenID;
		sendApplyToJoin.unionid = GameInfo.unionid;
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendApplyToJoin);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1040, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}


	/// <summary>
	/// 用户管理的加入用户按钮 圈主加入用户
	/// </summary>
	void PengYQJoinUser()
	{
		int num = 0;
		if (int.TryParse(PengYQJoin_Pel.transform.Find("GroupID").GetComponent<InputField>().text, out num))
		{
			SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID, 0, 0, num);
			PengYQJoin_Pel.transform.Find("GroupID").GetComponent<InputField>().text = "";
			PengYQJoin_Pel.SetActive(false);
			SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID);
		}
		else
		{
			FICWaringPanel._instance.Show("输入用户ID错误！");
		}
	}
	/// <summary>
	/// 获取圈内玩家列表，管理用户
	/// </summary>
	void SendGroupUsersManager(string openid, string unionid, Int64 groupID, Int64 delByUserID = 0, Int64 checkRecordByUserID = 0,long addUsers = 0)
	{
		SendGroupUsersManager sendPlayerList = new SendGroupUsersManager();
		sendPlayerList.openid = openid;
		sendPlayerList.unionid = unionid;
		sendPlayerList.GroupID = groupID;
		sendPlayerList.delByUserID = delByUserID;
		sendPlayerList.checkRecordByUserID = checkRecordByUserID;
		sendPlayerList.addUsers = addUsers;
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendPlayerList);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1050, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}
	/// <summary>
	/// 用户管理的搜索按钮   搜索用户
	/// </summary>
	/// <param name="str">用户名或ID</param>
	/// <param name="tf">用户对象父物体</param>
	void SearchUser(string str, Transform tf)
	{
		if (str != "")
		{
			try
			{
				if (str.Length != 6) { FICWaringPanel._instance.Show("输入ID错误！"); return; }
				int.Parse(str);
			}
			catch (Exception)
			{
				FICWaringPanel._instance.Show("输入ID错误！");
				return;
			}
			//暂无用户的Label设为显示
			PengYQUserManager_Pel.transform.Find("bgLogo").gameObject.SetActive(true);
			foreach (Transform item in tf)
			{
				string id = item.Find("UserId").GetComponent<Text>().text;
				if (id.Length > 5 && id.Substring(5, 6) == str)
				{
					item.gameObject.SetActive(true);
					PengYQUserManager_Pel.transform.Find("bgLogo").gameObject.SetActive(false);
				}
				else
				{
					item.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			for (int i = 1; i < tf.childCount; i++)
			{
				tf.GetChild(i).gameObject.SetActive(true);

			}
			PengYQUserManager_Pel.transform.Find("bgLogo").gameObject.SetActive(tf.childCount == 1 ? true : false);
		}

	}
	//朋友圈大厅的朋友圈信息按钮和朋友圈信息的基本信息按钮
	void SendGroupInfoByGroupID()
	{
		SendGroupInfoByGroupID sendGroupInfoByGroupID = new SendGroupInfoByGroupID();
		sendGroupInfoByGroupID.openid = GameInfo.OpenID;
		sendGroupInfoByGroupID.GroupID = (int)groupInfo.GroupID;
		sendGroupInfoByGroupID.unionid = GameInfo.unionid;
		Debug.Log(groupInfo.GroupID);
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGroupInfoByGroupID);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1032, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}
	/// <summary>
	/// 朋友圈大厅的朋友圈战绩按钮  请求获取圈子玩家战绩
	/// </summary>
	public void SendGetUserRecord()
	{
		GameInfo.isPYQExploits = true;
		SendGetUserRecord sendGetUserRecord = new SendGetUserRecord();
		sendGetUserRecord.UserID = GameInfo.userID;
		sendGetUserRecord.openid = GameInfo.OpenID;
		sendGetUserRecord.unionid = GameInfo.unionid;
		sendGetUserRecord.GroupID = (int)groupInfo.GroupID;
		Debug.Log("请求圈子战绩" + groupInfo.GroupID);
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGetUserRecord);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 9001, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}
	//朋友圈信息的退出按钮
	void SendQuitGroup()
	{
		SendQuitGroup sendQuitGroup = new SendQuitGroup();
		sendQuitGroup.GroupID = (int)groupInfo.GroupID;
		sendQuitGroup.UserID = GameInfo.userID;
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendQuitGroup);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1090, body.Length, 0, body);
		GameInfo.cs.Send(data); 
	}
	//朋友圈信息的圈子用户按钮
	void SendGroupUserInfoByGroupID()
	{
		SendGroupUserInfoByGroupID sendGroupUserInfoByGroupID = new SendGroupUserInfoByGroupID();
		sendGroupUserInfoByGroupID.openid = GameInfo.OpenID;
		sendGroupUserInfoByGroupID.GroupID = (int)groupInfo.GroupID;
		sendGroupUserInfoByGroupID.unionid = GameInfo.unionid;
		Debug.Log(groupInfo.GroupID);
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGroupUserInfoByGroupID);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1034, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}
		
    void Update() {
        if (GameInfo.returnGroupInfo != null) ShowPengYouQuan();
        if (GameInfo.returnGroupApplyInfo != null) if (record) ShowPengYQApply(); else ShowPengYQApplyRecord();
        if (GameInfo.returnPlayerList != null) ShowPengYQUserManager();
        if (GameInfo.returnRecordList != null) ShowPengYQCreateRommRecord();
        if (GameInfo.returnLobbyInfo != null) ShowPengYQLobby();
        if (GameInfo.returnApplyToJoin != null) ApplyToJoinStatus();
        if (GameInfo.returnQuitGroup != null) QuitPengYQ();
        if (GameInfo.returnMessgae != null) ReturnMessgae();
        if (GameInfo.returnGroupInfoByGroupID != null) ShowPengYQInfo();
        if (GameInfo.returnGroupUserInfoByGroupID != null) ShowPengYQUserStatus();
        if (GameInfo.returnUserRecord != null && GameInfo.isPYQExploits) ShowPengYQUserExploits();
    }
		
	/// <summary>
	/// 显示朋友圈界面
	/// </summary>
	void ShowPengYouQuan()
	{
		Transform tf = PengYQMain_Pel.transform.Find("Scroll View/Viewport/Content/QuanZi");
		dicTemp.Clear();
		Clear(tf.parent);
		PengYQMain_Pel.transform.Find("bgLogo").gameObject.SetActive(true);
		foreach (var item in GameInfo.returnGroupInfo.groupInfo)
		{
			PengYQMain_Pel.transform.Find("bgLogo").gameObject.SetActive(false);
			Transform tf1 = Instantiate(tf, tf.parent) as Transform;
			//tf1.gameObject.SetActive(true);
			tf1.name = item.GroupID.ToString();
			tf1.Find("Name").GetComponent<Text>().text = "圈名：" + item.GroupName;
			tf1.Find("QuanHao").GetComponent<Text>().text = "圈号：" + item.GroupID.ToString();
			Debug.Log(TimeToLong.ConvertIntDateTime(item.CreateTime).Date.ToString());
			tf1.Find("Time").GetComponent<Text>().text = "创建时间：" + TimeToLong.ConvertIntDateTime(item.CreateTime).ToShortDateString().ToString();
			//tf1.Find("Intro").GetComponent<Text>().text = "圈子简介：\n\t\t" + item.GroupIntroduction;
			tf1.Find("FKstate").GetComponent<Text>().text = "房卡状态：" + (item.RoomCardCounts>0 ? "<color=lime>充足</color>" : "<color=red>不足</color>");
			if (item.IsGroupLord)
			{
				tf1.Find("QZManagerBtn").GetComponent<Button>().onClick.AddListener(delegate { SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, long.Parse(tf1.name)); groupID = long.Parse(tf1.name); });
				// tempAction = delegate { SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, long.Parse(tf1.name)); };
				tf1.SetSiblingIndex(1);
			}
			else
			{
				tf1.Find("QZManagerBtn").gameObject.SetActive(false);
			}
			//dicTemp.Add(tf1.name, item);
			dicTemp[tf1.name] = item;
			tf1.Find("CreateRoomBtn").GetComponent<Button>().onClick.AddListener(delegate { GameInfo.GroupID = long.Parse(tf1.name);  });
			tf1.Find("EnterLobbiesBtn").GetComponent<Button>().onClick.AddListener(delegate {
				groupInfo = dicTemp[tf1.name];
				SendLobbyInfo(long.Parse(tf1.name));
				// tempAction = delegate { SendLobbyInfo(long.Parse(tf1.name)); };
			});
           
        }
		PengYQMain_Pel.transform.Find("Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, delegate { SendGroupInfo(); });
		GameInfo.returnGroupInfo = null;
	}

	void SendLobbyInfo(long groupID)
	{
		SendLobbyInfo sendLobbyInfo = new SendLobbyInfo();
		sendLobbyInfo.GroupID = groupID;
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendLobbyInfo);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1080, body.Length, 0, body);
		GameInfo.cs.Send(data); 
	}

	/// <summary>
	/// 圈主查看申请
	/// </summary>
	void ShowPengYQApply()
	{

		Transform tf = PengYQApply_Pel.transform.Find("Scroll View/Viewport/Content/QuanZi");

		Clear(tf.parent);
		PengYQApply_Pel.transform.Find("bgLogo").gameObject.SetActive(true);
		foreach (var item in GameInfo.returnGroupApplyInfo.GroupApplyInfoList.OrderByDescending(w => w.ApplyDateTime))
		{
			PengYQApply_Pel.transform.Find("bgLogo").gameObject.SetActive(false);
			Transform tf1 = Instantiate(tf, tf.parent) as Transform;
			//tf1.gameObject.SetActive(true);
			tf1.Find("UserName").GetComponent<Text>().text = "用户名：" + item.PlayerInfo[GameInfo.returnGroupApplyInfo.GroupApplyInfoList.IndexOf(item)].NickName;
			tf1.Find("UserId").GetComponent<Text>().text = "用户ID：" + item.PlayerInfo[GameInfo.returnGroupApplyInfo.GroupApplyInfoList.IndexOf(item)].GroupUserID.ToString();
			tf1.name = item.PlayerInfo[GameInfo.returnGroupApplyInfo.GroupApplyInfoList.IndexOf(item)].GroupUserID.ToString();
			tf1.Find("Time").GetComponent<Text>().text = "申请时间：" + TimeToLong.ConvertIntDateTime(item.ApplyDateTime).ToShortDateString().ToString();

			LoadImage.Instance.LoadPicture(item.PlayerInfo[GameInfo.returnGroupApplyInfo.GroupApplyInfoList.IndexOf(item)].picture, tf1.Find("hard/head").GetComponent<Image>());

			if (item.ApplyStatus == 4)
			{
				tf1.transform.Find("ConsentBtn/Text").GetComponent<Text>().text = "同意退出";
				tf1.transform.Find("RejectBtn/Text").GetComponent<Text>().text = "拒绝退出";
				tf1.transform.Find("ConsentBtn").GetComponent<Button>().onClick.AddListener(delegate { SendChangeApplyStatus(5, long.Parse(tf1.name)); SendApplyRecord(groupID); SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID); });
				tf1.transform.Find("RejectBtn").GetComponent<Button>().onClick.AddListener(delegate { SendChangeApplyStatus(6, long.Parse(tf1.name)); SendApplyRecord(groupID); SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID); });
			}
			else
			{
				tf1.transform.Find("ConsentBtn").GetComponent<Button>().onClick.AddListener(delegate { SendChangeApplyStatus(1, long.Parse(tf1.name)); SendApplyRecord(groupID); SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID); });
				tf1.transform.Find("RejectBtn").GetComponent<Button>().onClick.AddListener(delegate { SendChangeApplyStatus(2, long.Parse(tf1.name)); SendApplyRecord(groupID); SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID); });
			}

		}
		PengYQApply_Pel.transform.Find("Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, delegate { SendApplyRecord(groupID);Debug.Log(groupID); });
		GameInfo.returnGroupApplyInfo = null;
	}
	/// <summary>
	/// 发送申请状态
	/// </summary>
	/// <param name="status">申请状态，0申请中，1申请通过，2申请拒绝</param>
	void SendChangeApplyStatus(int status,long userID)
	{
		SendChangeApplyStatus sendChangeApplyStatus = new SendChangeApplyStatus();
		sendChangeApplyStatus.UserID = userID;
		sendChangeApplyStatus.GroupID = groupID;
		sendChangeApplyStatus.ApplyStatus = status;
		sendChangeApplyStatus.OperationUserID = GameInfo.userID;
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendChangeApplyStatus);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1070, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}
	/// <summary>
	/// 显示申请记录
	/// </summary>
	void ShowPengYQApplyRecord()
	{
		Transform tf = PengYQApplyRecord_Pel.transform.Find("Scroll View/Viewport/Content/QuanZi");
		Clear(tf.parent);
		PengYQApplyRecord_Pel.transform.Find("bgLogo").gameObject.SetActive(true);
		foreach (var item in GameInfo.returnGroupApplyInfo.GroupApplyInfoList.OrderByDescending(w => w.ApplyDateTime))
		{
			PengYQApplyRecord_Pel.transform.Find("bgLogo").gameObject.SetActive(false);
			Transform tf1 = Instantiate(tf, tf.parent) as Transform;
			//tf1.gameObject.SetActive(true);
			tf1.Find("UserName").GetComponent<Text>().text = "圈名：" + item.GroupName;
			tf1.Find("UserId").GetComponent<Text>().text = "圈号：" + item.GroupID.ToString();
			tf1.Find("Time").GetComponent<Text>().text = "申请时间：" + TimeToLong.ConvertIntDateTime(item.ApplyDateTime).ToShortDateString().ToString();
			tf1.Find("State").GetComponent<Text>().text = item.ApplyStatus == 0 ? "申请中": item.ApplyStatus == 1 ? "申请通过" : "申请拒绝";
		}
		PengYQApplyRecord_Pel.transform.Find("Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, delegate { SendApplyRecord(); });
		GameInfo.returnGroupApplyInfo = null;
	}
	/// <summary>
	/// 显示朋友圈管理界面
	/// </summary>
	void ShowPengYQUserManager()
	{
		Transform tf = PengYQUserManager_Pel.transform.Find("Scroll View/Viewport/Content/QuanZi");

		Clear(tf.parent);
		PengYQUserManager_Pel.transform.Find("bgLogo").gameObject.SetActive(true);
		foreach (var item in GameInfo.returnPlayerList.PlayerList)
		{
			PengYQUserManager_Pel.transform.Find("bgLogo").gameObject.SetActive(false);
			Transform tf1 = Instantiate(tf, tf.parent) as Transform;
			//tf1.gameObject.SetActive(true);
			LoadImage.Instance.LoadPicture(item.picture, tf1.Find("hard/head").GetComponent<Image>());
			tf1.Find("UserName").GetComponent<Text>().text = "用户名：" + item.NickName;
			tf1.Find("UserId").GetComponent<Text>().text = "用户ID：" + item.GroupUserID.ToString();
			tf1.name = item.GroupUserID.ToString();
			tf1.Find("LookKFRecordBtn").GetComponent<Button>().onClick.AddListener(
				delegate { SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID, 0, long.Parse(tf1.name));
					long.TryParse(tf1.name, out userID);
					// tempActionKFRecord = delegate { SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID, 0, long.Parse(tf1.name)); };
				});
			tf1.Find("DelUserBtn").GetComponent<Button>().onClick.AddListener(delegate {
				FICWaringPanel._instance.ShowQuit("确定要删除用户吗？");
				FICWaringPanel._instance.WarnningMethods = delegate {
					SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID, long.Parse(tf1.name));
					SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID);
				};
			});
		}
		PengYQUserManager_Pel.transform.Find("LookApplyBtn/count/Text").GetComponent<Text>().text = GameInfo.returnPlayerList.ApplyUsers.ToString();
		PengYQUserManager_Pel.transform.Find("InputField").GetComponent<InputField>().text = "";
		PengYQUserManager_Pel.transform.Find("Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, delegate { SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid,groupID);Debug.Log(groupID); });
		GameInfo.returnPlayerList = null;
	}

	/// <summary>
	/// 显示开房记录
	/// </summary>
	void ShowPengYQCreateRommRecord()
	{
		Transform tf = PengYQCreateRommRecord_Pel.transform.Find("Scroll View/Viewport/Content/QuanZi");
		Clear(tf.parent);
		PengYQCreateRommRecord_Pel.transform.Find("bgLogo").gameObject.SetActive(true);
		foreach (var item in GameInfo.returnRecordList.CreateRommRecordList.OrderByDescending(w => w.CreateDate))
		{
			PengYQCreateRommRecord_Pel.transform.Find("bgLogo").gameObject.SetActive(false);
			Transform tf1 = Instantiate(tf, tf.parent) as Transform;
			//tf1.gameObject.SetActive(true);
			var time = TimeToLong.ConvertIntDateTime(item.CreateDate);
			tf1.Find("Time").GetComponent<Text>().text = "创建时间：" + time.Month + "/" + time.Day + " " + time.Hour.ToString().PadLeft(2, '0') + ":" + time.Minute.ToString().PadLeft(2, '0');
			tf1.Find("CreateUserID").GetComponent<Text>().text = "创建用户ID：" + item.CreateUserID.ToString();
			tf1.Find("GroupID").GetComponent<Text>().text = "圈号：" + item.GroupID.ToString();
			tf1.Find(" RoomID").GetComponent<Text>().text = "房间号：" + item.RoomID.ToString();
			tf1.Find("UseRoomCard").GetComponent<Text>().text = "消耗房卡：" + item.UseRoomCard.ToString();
		}
		PengYQCreateRommRecord_Pel.transform.Find("Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, delegate {
			SendGroupUsersManager(GameInfo.OpenID, GameInfo.unionid, groupID, 0, userID);
			Debug.Log(userID);
		});
		GameInfo.returnRecordList = null;
	}

    /// <summary>
    /// 显示朋友圈大厅
    /// </summary>
    void ShowPengYQLobby()
    {
        Transform tf = PengYQLobby_Pel.transform.Find("Scroll View/Viewport/Content/QuanZi");
        Clear(tf.parent);
        PengYQLobby_Pel.transform.Find("bgLogo").gameObject.SetActive(true);
        foreach (var item in GameInfo.returnLobbyInfo.RoomListInfo)
        {
            PengYQLobby_Pel.transform.Find("bgLogo").gameObject.SetActive(false);
            Transform tf1 = Instantiate(tf, tf.parent) as Transform;
            //tf1.gameObject.SetActive(true);
            for (int i = 0; i < item.room_peo; i++)
            {
                tf1.Find("hard" + (i + 1)).gameObject.SetActive(true);
                if(i < item.PlayerList.Count) LoadImage.Instance.LoadPicture(item.PlayerList[i].picture, tf1.Find("hard" + (i + 1) + "/head").GetComponent<Image>());
            }
            tf1.Find("roomId").GetComponent<Text>().text = "房间号：" + item.roomID.ToString();
            tf1.name = item.roomID.ToString();

            var time = TimeToLong.ConvertIntDateTime(item.CreateDate);
            tf1.Find("Time").GetComponent<Text>().text = "创建时间：" + time.Month + "/" + time.Day +" "+ time.Hour.ToString().PadLeft(2, '0') + ":" + time.Minute.ToString().PadLeft(2, '0');
            

            ReturnRoomMsg msg = new ReturnRoomMsg();
            msg.is_benji = item.is_benji;
            msg.is_shangxiaji = item.is_shangxiaji;
            msg.is_wgj = item.is_wgj;
            msg.is_xinqiji = item.is_xinqiji;
            msg.is_lianzhuang = item.is_lianzhuang;
            msg.is_yikousan = item.is_yikousan;
            msg.Is_yuanque = item.IsYuanQue;
            msg.QuickCard = item.QuickCard;

            tf1.Find("method").GetComponent<Text>().text = "房间规则：" + hall.RoomInfo(msg);
            if (item.room_peo > item.PlayerList.Count)
            {
                tf1.Find("JoinRoomBtn").GetComponent<Button>().onClick.AddListener(delegate { enterRoom.gameObject.SetActive(true); enterRoom.roomidStr = tf1.name; GameInfo.GroupID = groupInfo.GroupID ; enterRoom.OnEnterRoomClick(); });
                tf1.SetSiblingIndex(1);
            }
            else
            {
                tf1.Find("JoinRoomBtn").gameObject.SetActive(false);
                tf1.SetSiblingIndex(tf.parent.childCount - 1);
            }
        }
        PengYQLobby_Pel.transform.Find("Name").GetComponent<Text>().text = "圈名：" + groupInfo.GroupName;
        PengYQLobby_Pel.transform.Find("QuanHao").GetComponent<Text>().text = "圈号：" + groupInfo.GroupID.ToString();
        //PengYQLobby_Pel.transform.Find("Intro").GetComponent<Text>().text = "圈子简介：\n\t\t" + groupInfo.GroupIntroduction;
        //PengYQLobby_Pel.transform.Find("LookInfoBtn").gameObject.SetActive(!groupInfo.IsGroupLord);
        PengYQInfo_Pel.transform.Find("Info_Pel/QuitBtn").gameObject.SetActive(!groupInfo.IsGroupLord);
        PengYQLobby_Pel.transform.Find("Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, delegate { SendLobbyInfo(groupInfo.GroupID); Debug.Log(groupInfo.GroupID); });
        GameInfo.returnLobbyInfo = null;
    }
	void ApplyToJoinStatus()
	{
		FICWaringPanel._instance.Show(GameInfo.returnApplyToJoin.Message);
		if (GameInfo.returnApplyToJoin.Status == 1) SendGroupInfo();
		GameInfo.returnApplyToJoin = null;
	}

	void QuitPengYQ()
	{
		switch (GameInfo.returnQuitGroup.Status)
		{
		case 0:
			FICWaringPanel._instance.Show(GameInfo.returnQuitGroup.Message);
			break;
		case 1:
			FICWaringPanel._instance.Show(GameInfo.returnQuitGroup.Message);
			PengYQLobby_Pel.SetActive(false);
			SendGroupInfo();
			break;
		}
		GameInfo.returnQuitGroup = null;
	}
	void ReturnMessgae()
	{
		switch (GameInfo.returnMessgae.Statue)
		{
		case 0:
			FICWaringPanel._instance.Show(GameInfo.returnMessgae.Message);
			break;
		case 1:
			FICWaringPanel._instance.Show(GameInfo.returnMessgae.Message);
			break;
		}
		GameInfo.returnMessgae = null;
	}

	/// <summary>
	/// 显示朋友圈信息
	/// </summary>
	void ShowPengYQInfo()
	{

		if (GameInfo.returnGroupInfoByGroupID.Status == 0)
		{
			FICWaringPanel._instance.Show("圈子不存在，或者 不是圈子用户，需要刷新圈子信息。");
			return;
		}

		PengYQInfo_Pel.transform.Find("Info_Pel/tag").GetComponent<Text>().text = "圈子号：" + GameInfo.returnGroupInfoByGroupID.GroupID +
			"\n圈子名：" + GameInfo.returnGroupInfoByGroupID.GroupName +
			"\n创建人：" + GameInfo.returnGroupInfoByGroupID.NikeName +
			"\n\n创建时间：" + TimeToLong.ConvertIntDateTime(GameInfo.returnGroupInfoByGroupID.CreateTime).Date +
			"\n圈子人数：" + GameInfo.returnGroupInfoByGroupID.GroupNumberPeople;
		GameInfo.returnGroupInfoByGroupID = null;
	}

	/// <summary>
	/// 显示圈子用户状态
	/// </summary>
	void ShowPengYQUserStatus()
	{
		if (GameInfo.returnGroupUserInfoByGroupID.Status == 0)
		{
			FICWaringPanel._instance.Show("圈子不存在，或者 不是圈子用户，需要刷新圈子信息。");
			return;
		}


		Transform tf = PengYQInfo_Pel.transform.Find("User_Pel/Scroll View/Viewport/Content/QuanZi");
		Clear(tf.parent);
		foreach (var item in GameInfo.returnGroupUserInfoByGroupID.userList.OrderByDescending(w=>w.Status))
		{
			Transform tf1 = Instantiate(tf, tf.parent) as Transform;
			LoadImage.Instance.LoadPicture(item.picture, tf1.Find("hard/head").GetComponent<Image>());
			tf1.Find("UserID").GetComponent<Text>().text = item.GroupUserID.ToString();
			tf1.Find("UserName").GetComponent<Text>().text = item.NickName;

			switch (item.Status)
			{
			case 0:
				tf1.Find("UserStatus").GetComponent<Text>().text = "<color=grey>离线</color>";
				break;
			case 1:
				tf1.Find("UserStatus").GetComponent<Text>().text = "<color=green>在线</color>";
				break;
			case 2:
				tf1.Find("UserStatus").GetComponent<Text>().text = "<color=red>游戏中</color>";
				break;
			}
		}
		PengYQInfo_Pel.transform.Find("User_Pel/Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, delegate {
			SendGroupUserInfoByGroupID();
		});
		GameInfo.returnGroupUserInfoByGroupID = null;
	}
	/// <summary>
	/// 圈子用户战绩查看
	/// </summary>
	void ShowPengYQUserExploits()
	{
		Debug.Log("圈子战绩");
		Transform tf = PengYQUserExploits_Pel.transform.Find("Scroll View/Viewport/Content/QuanZi");
		Clear(tf.parent);
		int index = 0;
		PengYQUserExploits_Pel.transform.Find("bgLogo").gameObject.SetActive(true);
		foreach (var item in GameInfo.returnUserRecord.userRecord.OrderByDescending(w => w.CreateDate))
		{
			PengYQUserExploits_Pel.transform.Find("bgLogo").gameObject.SetActive(false);
			Transform tf1 = Instantiate(tf, tf.parent) as Transform;
			var time = TimeToLong.ConvertIntDateTime(item.CreateDate);
			tf1.Find("time").GetComponent<Text>().text = "时间：" + time.Month + "/" + time.Day + " " + time.Hour.ToString().PadLeft(2, '0') + ":" + time.Minute.ToString().PadLeft(2, '0');
			tf1.Find("index").GetComponent<Text>().text = "序号：" + ++index;
			for (int i = 0; i < item.recordUserInfo.Count; i++)
			{
				tf1.Find("hard" + (i + 1)).gameObject.SetActive(true);

				tf1.Find("hard" + (i + 1) + "/UserId").GetComponent<Text>().text = item.recordUserInfo[i].UserID.ToString();
				tf1.Find("hard" + (i + 1) + "/UserName").GetComponent<Text>().text = item.recordUserInfo[i].nickname;
				tf1.Find("hard" + (i + 1) + "/Score").GetComponent<Text>().text = int.Parse(item.recordUserInfo[i].Score) >= 0 ? "<color=green>+" + item.recordUserInfo[i].Score+"</color>" : "<color=red>"+ item.recordUserInfo[i].Score + "</color>";//if (int.Parse(item.recordUserInfo[i].Score) >= 0) { } ? "<color=lime>充足</color>" : "<color=red>不足</color>");
				LoadImage.Instance.LoadPicture(item.recordUserInfo[i].headimg, tf1.Find("hard" + (i + 1) + "/head").GetComponent<Image>());
			}
		}
		PengYQUserExploits_Pel.transform.Find("Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, delegate
			{
				SendGetUserRecord();
			});
		GameInfo.returnUserRecord = null;
		GameInfo.isPYQExploits = false;

	}
		
    /// <summary>
    /// 清除对象集合
    /// </summary>
    /// <param name="tf">对象集合父物体</param>
    void Clear(Transform tfParent)
    {
        for (int i = 1; i < tfParent.childCount; i++)
        {
            Destroy(tfParent.GetChild(i).gameObject);
        }
    }
}
