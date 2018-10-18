using UnityEngine;
using DDZ;
using MJBLL.common;

public class DDZSendMessage : Singleton<DDZSendMessage>
{
    /// <summary>
    /// 发送创建房间
    /// </summary>
    public void SendDZCreateRoom()
    {
        ddzSendDZCreateRoom sendDZCreateRoom = new ddzSendDZCreateRoom();
        sendDZCreateRoom.openid = GameInfo.OpenID;
        sendDZCreateRoom.roomNumber = DDZData.roomInfo.inningNum;
        sendDZCreateRoom.roomtype = DDZData.roomInfo.playType;
        sendDZCreateRoom.multiple = DDZData.roomInfo.multiple;
        sendDZCreateRoom.Latitude = GameInfo.Latitude;
        sendDZCreateRoom.GroupID = (int)GameInfo.GroupID;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendDZCreateRoom);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUMD + 2001, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }
    /// <summary>
    /// 发送加入房间
    /// </summary>
    /// <param name="roomId">房间ID</param>
    public void SendAddRoom(int roomId)
    {
        ddzSendAddRoom sendAddRoom = new ddzSendAddRoom();
        sendAddRoom.roomID = roomId;
        sendAddRoom.openid = GameInfo.OpenID;
        sendAddRoom.Latitude = GameInfo.Latitude;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendAddRoom);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUMD + 2003, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }
    /// <summary>
    /// 发送请求开始游戏
    /// </summary>
    public void SendStart()
    {
        ddzSendStart sendStart = new ddzSendStart();
        sendStart.roomid = DDZData.room_id;
        sendStart.openid = GameInfo.OpenID;
        sendStart.UserID = GameInfo.userID;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendStart);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUMD + 2007, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }
    /// <summary>
    /// 发送叫地主
    /// </summary>
    /// <param name="type">叫牌类型</param>
    /// <param name="value">叫牌值</param>
    public void SendCallLandlord(int type, int value)
    {
        ddzSendCallLandlord sendCallLandlord = new ddzSendCallLandlord();
        sendCallLandlord.FW = DDZData.fw;
        sendCallLandlord.openid = GameInfo.OpenID;
        sendCallLandlord.UserID = GameInfo.userID;
        sendCallLandlord.CallType = type;
        sendCallLandlord.value = value;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendCallLandlord);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUMD + 2009, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }
    /// <summary>
    /// 发送出牌信息
    /// </summary>
    /// <param name="cardsStr">//牌ID集合 中间以,分隔  比如 103,104,105 不传为过牌</param>
    public void SendOutCard(string cardsStr)
    {
        ddzSendOutCard sendOutCard = new ddzSendOutCard();
        sendOutCard.openid = GameInfo.OpenID;
        sendOutCard.UserID = GameInfo.userID;
        sendOutCard.FW = DDZData.fw;
        sendOutCard.cardsStr = cardsStr;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendOutCard);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUMD + 2011, body.Length, 0, body);
        GameInfo.cs.Send(data);
    }
}
