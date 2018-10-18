using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MJBLL.common;
using UnityEngine.SceneManagement;
using DNL;

public class FICEnterRoom : MonoBehaviour
{
    int roomID;
    // bool isClosed = false;
    

    InputField textNum;
    int index = 0;
    public string roomidStr;
    private void Start()
    {
        textNum = transform.Find("BG/InputField").GetComponent<InputField>();
    }

    /*
    public  void OnEnterRoomLoaded()
    {
        roomID = int.Parse(roomidStr);
        SendAddRoom addRoom = SendAddRoom.CreateBuilder()
                               .SetRoomID(roomID)
                               .SetOpenid(GameInfo.OpenID)
                               .Build();
        byte[] body = addRoom.ToByteArray();
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 2003, body.Length, 0, body);
        Debug.Log("jiaru" + roomID + ",id:" + GameInfo.OpenID);
        GameInfo.cs.Send(data);

        GameInfo.MJplayers.Clear();//加入房间时，清空字典，以免加入别的房间，数据不对
        roomidStr = null;
    }*/

	/// <summary>
	/// 根据数字按钮传递参数的不同，在界面上显示相应的数字，
	/// </summary>
	/// <param name="num"></param>
	public void OnNumberButtonClick(int num)
	{
		textNum.text += num;
		index++;
		if (index >= 6)
		{
			OnSureButtonClick();
		}
	}

	/// <summary>
	/// 当确定按钮点击的时候，将roomidstr 转换成数字类型的roomid发送给服务端，这里直接调用的原有程序实现
	/// </summary>
	public void OnSureButtonClick()
	{
		roomidStr = textNum.text;
		//执行检测玩家距离

		OnEnterRoomClick();
		OnClearButtonClick();
	}

    //message SendAddRoomOne{
    //required string openid = 1;//用户OPENID
    //required int32 RoomID=2;//房间号}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    public void OnEnterRoomClick()
    {
        //if (GameInfo.isScoketClose)
        //    GameInfo.cs.Closed();
        //GameInfo.cs.serverType = ServerType.ListServer;
        roomID = int.Parse(roomidStr);
        GameInfo.room_id = roomID;
        //2017.8.2添加劉磊 開始
        GameInfo.operation = 2;
        //SendGameOperation sendGameOperation = SendGameOperation.CreateBuilder()
        //    .SetOpenid(GameInfo.OpenID)
        //    .SetUnionid(GameInfo.unionid)
        //    .SetOperation(GameInfo.operation)
        //    .SetRoomID(GameInfo.room_id.ToString())
        //    .Build();
        //byte[] body = sendGameOperation.ToByteArray();

        SendGameOperation sendGameOperation = new SendGameOperation();
        sendGameOperation.openid = GameInfo.OpenID;
        sendGameOperation.unionid = GameInfo.unionid;
        sendGameOperation.Operation = GameInfo.operation;
        sendGameOperation.RoomID = GameInfo.room_id.ToString();
        sendGameOperation.GroupID = (int)GameInfo.GroupID;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendGameOperation);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 1020, body.Length, 0, body);
        GameInfo.cs.Send(data);
       // GameInfo.isScoketClose = true;
        DebugLog(body);
        //結束
	}
	private void DebugLog(byte[] body)
	{
		SendGameOperation ope = ProtobufUtility.DeserializeProtobuf<SendGameOperation>(body);
		Debug.Log("加入房间，游戏openid" + ope.openid);
		Debug.Log("加入房间，游戏unionid" + ope.unionid);
		Debug.Log("加入房间，游戏operation" + ope.Operation);
		Debug.Log("加入房间，游戏roomid" + ope.RoomID);
	}
		
	/// <summary>
	/// 当清除按钮点击的时候，将roomid显示框清空，将roomidstr清空
	/// </summary>
	public void OnClearButtonClick()
	{
		textNum.text = "";
		index = 0; roomidStr = null;
	}
	/// <summary>
	/// 关闭按钮被点击的时候，关闭输入房间弹框，清空房间号码文字框
	/// </summary>
	public void OnCloseButtonClick()
	{
		OnClearButtonClick();
		StartCoroutine(delay());
	}
	IEnumerator delay()
	{
		yield return new WaitForSeconds(0.5f);
		gameObject.SetActive(false);
	}

	/// <summary>
	/// 当删除按钮被点击的时候，将最后一位房间号码文字框清空，
	/// </summary>
	public void OnBackSpaceButtonClick()
	{
		index--;
		if (index < 0)
		{
			index = 0;
		}
		textNum.text = textNum.text.Substring(0, index);
	}

    // Update is called once per frame
    void Update()
    {
        if (GameInfo.operation == 2 && GameInfo.addStatus == 1)
        {
            GameInfo.cs.serverType = ServerType.GameServer;
            GameInfo.operation = 0;
            GameInfo.addStatus = 0;
           // SendAddRoomOne addRoomOne = SendAddRoomOne.CreateBuilder().SetRoomID(roomID).SetOpenid(GameInfo.OpenID).Build();

            // byte[] body = addRoomOne.ToByteArray();
            SendAddRoomOne addRoomOne = new SendAddRoomOne();
            addRoomOne.RoomID = roomID;
            addRoomOne.openid = GameInfo.OpenID;
            addRoomOne.GroupID = (int)GameInfo.GroupID;
            byte[] body = ProtobufUtility.GetByteFromProtoBuf(addRoomOne);
            byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 7089, body.Length, 0, body);
            GameInfo.cs.Send(data);
        }
		//GameInfo.returnAddRoom返回加入房间信息
        if (GameInfo.returnAddRoom != null)
        {
            if (GameInfo.returnAddRoom.state == 10000)
            {
                GameInfo.room_id = roomID;
                //   isClosed = true;
                GameObject.Find("Main Camera").GetComponent<Manager_Hall>().isClosed = true;
                //加入房间成功
                // FICCreatRoom.MJplayers.Add(GameInfo.dir, GameInfo.returnAddRoom.UserinfoList[GameInfo.returnAddRoom.UserinfoCount - 1]);
                foreach (var item in GameInfo.returnAddRoom.userinfo)
                {
                    if (item.openid.Equals(GameInfo.OpenID))
                        GameInfo.FW = item.user_FW;
                    if (GameInfo.MJplayers.ContainsKey(item.user_FW))
                        GameInfo.MJplayers[item.user_FW] = item;
                    else
                        GameInfo.MJplayers.Add(item.user_FW, item);

                    GameInfo.MJplayersWhoQuit[item.openid] = item.user_FW;
                }
                //GameInfo.sceneID = "Scene_Game";
                // SceneManager.LoadScene("LoadingHall");
                //SceneManager.LoadScene("Scene_Game");
                GameInfo.returnAddRoom = null;
            }
            else if (GameInfo.returnAddRoom.state == 10001)
            {
                GameInfo.returnAddRoom = null;
                FICWaringPanel._instance.Show("房间不存在!");
                GameInfo.cs.Closed();
                GameInfo.cs.serverType = ServerType.ListServer;
            }
            else if (GameInfo.returnAddRoom.state == 10002)
            {
                GameInfo.returnAddRoom = null;
                //房间人数已满
                FICWaringPanel._instance.Show("房间人数已满!");
                GameInfo.cs.Closed();
                GameInfo.cs.serverType = ServerType.ListServer;
            }
        }
        if (GameInfo.returnRoomAdd != null)
        {
            if (GameInfo.returnRoomAdd.Start == 1)
            {
                GameObject.Find("Main Camera").GetComponent<Manager_Hall>().isClosed = true;
                GameInfo.gameName = "GYMJ";
                GameInfo.sceneID = "Game_GYMJ";
                GameInfo.isScoketClose = false;
                SceneManager.LoadScene("LoadingHall");
                GameInfo.returnRoomAdd = null;
            }
            else if (GameInfo.returnRoomAdd.Start == 2)
            {
                FICWaringPanel._instance.Show("房间人数已满!");
                GameInfo.cs.Closed();
                GameInfo.cs.serverType = ServerType.ListServer;
            }
            else if (GameInfo.returnRoomAdd.Start == 3)
            {
                FICWaringPanel._instance.Show("房间不存在!");
                GameInfo.cs.Closed();
                GameInfo.cs.serverType = ServerType.ListServer;
            }

            GameInfo.returnRoomAdd = null;
        }
    }


    //private void OnDestroy()
    //{
    //    if (!isClosed)
    //        GameInfo.cs.Closed();
    //}
}
