using UnityEngine;
using DNL;
using MJBLL.common;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class UserGamePlayback : MonoBehaviour {
    private Manager_Hall hall;
    private string keyTemp = "";
    private Button News_Btn;
//======================News_Pel面板=========================================
    private GameObject News_Pel;
    private Dictionary<string, UserRecord> userRecords = new Dictionary<string, UserRecord>();
    private Transform News_Pel_Content;
    private GameObject Info_Frame;
    private GameObject Info_FrameTemp;
    private Text News_Pel_timeTxt;
    private Image[] News_Pel_headIge = new Image[4];
    private Text[] News_Pel_scoreTxt = new Text[4];
    private GameObject[] FangzhuGo = new GameObject[4];
    private Text RoomInfoTxt;
    private Button playback_Btn;
    private GameObject News_Pel_bgLogo;
//===========================================================================
//======================Newss_Pel面板=========================================
    private GameObject Newss_Pel;
    private List<GameOperationProcess> GameOperationProcesss= new List<GameOperationProcess>();
    private Transform Newss_Pel_Content;
    private GameObject Info_FrameDetails;
    private GameObject Info_FrameDetailsTemp;
    private Text Newss_Pel_timeTxt;
    private Image[] Newss_Pel_headIge = new Image[4];
    private Text[] Newss_Pel_scoreTxt = new Text[4];
    private GameObject[] zhuang = new GameObject[4];
    private Text timeTxt;
    private Text rule;
    private Button playbacks_Btn;
    private GameObject Newss_Pel_bgLogo;
//===========================================================================

    void Start () {
        hall = GetComponent<Manager_Hall>();
        News_Btn = transform.Find("/Game_UI/One_UI/News_Btn").GetComponent<Button>();
        News_Btn.onClick.AddListener(delegate { OnSendGetUserRecord(); });
//==========================================================================================================================
        News_Pel = transform.Find("/Game_UI/Two_UI/News_Pel").gameObject;
        News_Pel_Content = News_Pel.transform.Find("Scroll View/Viewport/Content");
        Info_FrameTemp = News_Pel_Content.GetChild(0).gameObject;
        News_Pel_bgLogo = News_Pel.transform.Find("bgLogo").gameObject;
//==========================================================================================================================
        Newss_Pel = transform.Find("/Game_UI/Two_UI/Newss_Pel").gameObject;
        Newss_Pel_Content = Newss_Pel.transform.Find("Scroll View/Viewport/Content");
        Info_FrameDetailsTemp = Newss_Pel_Content.GetChild(0).gameObject;
        timeTxt = Newss_Pel.transform.Find("info/time").GetComponent<Text>();
        rule = Newss_Pel.transform.Find("info/rule").GetComponent<Text>();
        Newss_Pel_bgLogo = Newss_Pel.transform.Find("bgLogo").gameObject;
//==========================================================================================================================
    }
    void Update () {
		//如果服务器返回的用户记录不为空并且isPYQExploits=false
        if (GameInfo.returnUserRecord != null && !GameInfo.isPYQExploits)
        {
            Debug.Log("战绩");
			//清除用户记录
            userRecords.Clear();
			//删除对象的子对象，确保对象的子对象只有1个
            OnDestroyObj(News_Pel_Content);

            foreach (var userRecord in GameInfo.returnUserRecord.userRecord.OrderByDescending(w => w.CreateDate))
            {
                userRecords[userRecord.RoomInfoID] = userRecord;
            }
            ShowUserRecord();
            GameInfo.returnUserRecord = null;
        }
        if (GameInfo.returnGetUserGamePlayback != null)
        {
            GameOperationProcesss.Clear();
            OnDestroyObj(Newss_Pel_Content);
            GameOperationProcesss = GameInfo.returnGetUserGamePlayback.gameOperationProcess;
            ShowUserGamePlayback();
            GameInfo.returnGetUserGamePlayback = null;
        }
    }
    /// <summary>
    /// 请求获取玩家战绩
    /// </summary>
    public void OnSendGetUserRecord()
    {
        News_Pel.SetActive(true);
        SendGetUserRecord sendGetUserRecord = new SendGetUserRecord();
        sendGetUserRecord.UserID = GameInfo.userID;
        sendGetUserRecord.openid = GameInfo.OpenID;
        sendGetUserRecord.unionid = GameInfo.unionid;
        Debug.Log("请求战绩");
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGetUserRecord);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 9001, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }
		
    /// <summary>
    /// 显示战绩界面信息
    /// </summary>
    public void ShowUserRecord()
    {
		//提示"没有战绩"的文字物体设置显示
        News_Pel_bgLogo.SetActive(true);
        foreach (var key in userRecords.Keys)
        {
			//提示"没有战绩"的文字物体设置隐藏
            News_Pel_bgLogo.SetActive(false);
            Info_Frame = Instantiate(Info_FrameTemp, News_Pel_Content) as GameObject;
            InitFrame(key);
        }
        News_Pel.transform.Find("Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, null);
    }
    /// <summary>
    /// 赋值每大局信息
    /// </summary>
    /// <param name="key"></param>
    public void  InitFrame(string key)
    {
        News_Pel_timeTxt = Info_Frame.transform.Find("Time").GetComponent<Text>();
        for (int i = 0; i < 4; i++)
        {
            News_Pel_headIge[i] = Info_Frame.transform.Find("hard" + (i + 1) + "/head").GetComponent<Image>();
            News_Pel_scoreTxt[i] = Info_Frame.transform.Find("hard" + (i + 1) + "/score").GetComponent<Text>();
            FangzhuGo[i] = Info_Frame.transform.Find("hard" + (i + 1) + "/Fangzhu").gameObject;
        }
        RoomInfoTxt = Info_Frame.transform.Find("RoomInfo").GetComponent<Text>();
        playback_Btn = Info_Frame.transform.Find("playback_Btn").GetComponent<Button>();



        var time = TimeToLong.ConvertIntDateTime(userRecords[key].CreateDate);
        News_Pel_timeTxt.text = time.Year + "-" + time.Month + "-" + time.Day + "\n" + time.Hour.ToString().PadLeft(2, '0') + ":" + time.Minute.ToString().PadLeft(2, '0');
        for (int i = 0; i < userRecords[key].recordUserInfo.Count; i++)
        {
            News_Pel_headIge[i].transform.parent.gameObject.SetActive(true);
            LoadImage.Instance.LoadPicture(userRecords[key].recordUserInfo[i].headimg, News_Pel_headIge[i]);
            News_Pel_scoreTxt[i].text = userRecords[key].recordUserInfo[i].Score;
            if (userRecords[key].recordUserInfo[i].UserID == userRecords[key].UserID) FangzhuGo[i].SetActive(true);
        }
        RoomInfoTxt.text = hall.RoomInfo(userRecords[key].RoomMsg);
        playback_Btn.onClick.AddListener(delegate { OnSendGetUserGamePlayback(key); });
    }

	/// <summary>
	/// 根据牌桌信息ID请求牌局信息
	/// </summary>
	public void OnSendGetUserGamePlayback(string str)
	{
		Newss_Pel.SetActive(true);
		keyTemp = str;
		SendGetUserGamePlayback sendGetUserGamePlayback = new SendGetUserGamePlayback();
		sendGetUserGamePlayback.RoomInfoID = str;
		byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGetUserGamePlayback);
		byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 9003, body.Length, 0, body);
		GameInfo.cs.Send(data);
	}
    /// <summary>
    /// 显示牌局回放记录列表
    /// </summary>
    public void ShowUserGamePlayback()
    {
        Newss_Pel_bgLogo.SetActive(true);
        for (int i = 0; i < GameOperationProcesss.Count; i++)
        {
            Newss_Pel_bgLogo.SetActive(false);
            Info_FrameDetails = Instantiate(Info_FrameDetailsTemp, Newss_Pel_Content) as GameObject;
            InitFrameDetails(i);
        }
        Newss_Pel.transform.Find("Scroll View").GetComponent<ScrollRectControl>().InitScrollRect(1, null);
    }
    /// <summary>
    /// 赋值每局信息
    /// </summary>
    /// <param name="index"></param>
    public void InitFrameDetails(int index)
    {
        Newss_Pel_timeTxt = Info_FrameDetails.transform.Find("Time").GetComponent<Text>();
        for (int i = 0; i < 4; i++)
        {
            Newss_Pel_headIge[i] = Info_FrameDetails.transform.Find("hard" + (i + 1) + "/head").GetComponent<Image>();
            Newss_Pel_scoreTxt[i] = Info_FrameDetails.transform.Find("hard" + (i + 1) + "/score").GetComponent<Text>();
            zhuang[i] = Info_FrameDetails.transform.Find("hard" + (i + 1) + "/zhuang").gameObject;
        }
        playbacks_Btn = Info_FrameDetails.transform.Find("playback_Btn").GetComponent<Button>();



        Newss_Pel_timeTxt.text = index + 1 + "/" + GameOperationProcesss.Count;
        zhuang[GameOperationProcesss[index].gameOperationInfo[0].OperationFW-1].SetActive(true);
        var jsInfo = ProtobufUtility.DeserializeProtobuf<ReturnJS>(GameOperationProcesss[index].JieSuanInfo);
        for (int i = 0; i < userRecords[keyTemp].recordUserInfo.Count; i++)
        {
            Newss_Pel_headIge[i].transform.parent.gameObject.SetActive(true);
            LoadImage.Instance.LoadPicture(userRecords[keyTemp].recordUserInfo[i].headimg, Newss_Pel_headIge[i]);
            Newss_Pel_scoreTxt[i].text = jsInfo.js[i].FS.ToString();
        }
        var time = TimeToLong.ConvertIntDateTime(userRecords[keyTemp].CreateDate);
        timeTxt.text = time.Year + "-" + time.Month + "-" + time.Day + "\n" + time.Hour.ToString().PadLeft(2, '0') + ":" + time.Minute.ToString().PadLeft(2, '0');
        rule.text = hall.RoomInfo(userRecords[keyTemp].RoomMsg);
        playbacks_Btn.onClick.AddListener(delegate { GamePlayback(GameOperationProcesss[index]); });
    }

	/// <summary>
	/// 牌局回放
	/// </summary>
	public void GamePlayback(GameOperationProcess temp)
	{
		GameInfo.gameOperationProcess = temp;
		SceneManager.LoadScene("Game_GYMJ");
	}

    /// <summary>
    /// 删除对象的子对象，确保对象的子对象只有1个
    /// </summary>
    /// <param name="tempObjs"></param>
    private void OnDestroyObj(Transform tempObjs)
    {
		//GetSiblingIndex()待研究
        foreach (Transform item in tempObjs)
        {
           if(item.GetSiblingIndex() != 0) Destroy(item.gameObject);
        }
    }

}
