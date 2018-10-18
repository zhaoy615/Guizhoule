using ListBLL.common;
using ListBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ListBLL.Logic
{
   public class LobbyInfo : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11080"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var sendLobbyInfo = SendLobbyInfo.ParseFrom(requestInfo.Body);

            DAL.DAL.RoomInfoDAL roomInfoDAL = new DAL.DAL.RoomInfoDAL();
            byte[] returnLobbyInfoData = null;
            var returnLobbyInfo = ReturnLobbyInfo.CreateBuilder();
            //var roomInfo = RoomInfo.CreateBuilder();           
            var roomIDList=  roomInfoDAL.GetroomInfoByGroupID(sendLobbyInfo.GroupID);

            foreach (var item in roomIDList)
            {
                //获取房间信息
                RedisGameRoomInfo redisRoomInfo = RedisUtility.Get<RedisGameRoomInfo>(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, item.ToString(), item.ToString()));


                if (redisRoomInfo!=null)
                {
                    var roomInfo = LobbyRoomInfo.CreateBuilder();
                    roomInfo.SetRoomID(redisRoomInfo.room.RoomID).SetCreateDate(TimeToLong.ConvertDateTimeInt(redisRoomInfo.room.CreateDate)).SetIsWgj(redisRoomInfo.room.is_wgj ? 1 : 0).SetIsXinqiji(redisRoomInfo.room.is_xinqiji ? 1 : 0)
                   .SetIsShangxiaji(redisRoomInfo.room.is_shangxiaji ? 1 : 0).SetIsBenji(redisRoomInfo.room.is_benji ? 1 : 0).SetIsYikousan(redisRoomInfo.room.is_yikousan ? 1 : 0).SetIsLianzhuang(redisRoomInfo.room.is_lianz ? 1 : 0)
                   .SetRoomPeo(redisRoomInfo.room.room_peo).SetCount(redisRoomInfo.room.count).SetQuickCard(redisRoomInfo.room.QuickCard ? 1 : 0).SetIsYuanQue(redisRoomInfo.room.IsYuanQue ? 1 : 0);

                  

                    //获取房间里用户信息
                    foreach (var userOpenid in redisRoomInfo.room.listOpenid)
                    {
                        var player = Player.CreateBuilder();
                        RedisUserInfoModel user = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, userOpenid, string.Empty));
                        player.SetGroupUserID(user.UserID).SetNickName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(user.Nickname))).SetPicture(user.Headimg);
                        roomInfo.AddPlayerList(player);
                    }
                    returnLobbyInfo.AddRoomListInfo(roomInfo);
                }
            }
            returnLobbyInfoData = returnLobbyInfo.Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1081, returnLobbyInfoData.Length, requestInfo.MessageNum, returnLobbyInfoData)));


        }
    }
}
