using DAL.DAL;
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
    public class UserRecordLogic : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "19001";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var userInfo = SendGetUserRecord.ParseFrom(requestInfo.Body);
            RedisLoginModel olduser = RedisUtility.Get<RedisLoginModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, userInfo.Openid, userInfo.Unionid));
            if (olduser == null)
                return;
          var list=  new userRecordLOGDAL().Get48hourInfoByUserID(userInfo.UserID, userInfo.HasGroupID? userInfo.GroupID:0);
            var userinfoDAL = new mjuserinfoDAL();

            var returnUserRecord = ReturnUserRecord.CreateBuilder();
            long userid;
            foreach (var item in list)
            {
                var userRecord = UserRecord.CreateBuilder();
                userRecord.SetRoomInfoID(item.RoomInfoID);
                userRecord.SetCreateDate(TimeToLong.ConvertDateTimeInt(item.CreateDate));
                userRecord.SetEndTime(TimeToLong.ConvertDateTimeInt(item.EndTime));
                userRecord.SetUserID(item.CreateUserID);//需要显示房主标签
                userRecord.SetRoomMsg(
                    ReturnRoomMsg.CreateBuilder()
                    .SetCount(item.RoomNumber)
                    .SetIsBenji(item.IsBenJi)
                    .SetIsLianzhuang(item.CountPointsType == 3 ? 1 : 0)
                    .SetIsShangxiaji(item.IsSangXiaJi)
                    .SetIsWgj(item.IsWGJ)
                    .SetIsXinqiji(item.IsXinQiJi)
                    .SetIsYikousan(item.CountPointsType == 1 ? 1 : 0)
                    .SetIsYuanque(item.IsYuanQue)
                    .SetQuickCard(item.QuickCard)
                    .SetRoomPeo(item.RoomPeo)
                    );
                foreach (var userWinLose in item.UserWinLose.Split('|'))
                {
                    long userID = 0;
                    string[] info = userWinLose.Split(',');
                    if (long.TryParse(info[0], out userID))
                    {
                        if (userRecord.RecordUserInfoList.Any(w => w.UserID == userID))
                        {
                            var userInfoData = userRecord.RecordUserInfoList.First(w => w.UserID == userID);
                            userRecord.AddRecordUserInfo(RecordUserInfo.CreateBuilder()
                                    .SetHeadimg(userInfoData.Headimg)
                                    .SetNickname(HttpUtility.UrlDecode(HttpUtility.UrlDecode( userInfoData.Nickname)))
                                    .SetOpenid(userInfoData.Openid)
                                    .SetScore(info[1])
                                    .SetSex(userInfoData.Sex)
                                    .SetUserID(userInfoData.UserID));
                        }
                        else
                        {
                            var userInfoData = userinfoDAL.GetModel(userID);
                            userRecord.AddRecordUserInfo(RecordUserInfo.CreateBuilder()
                                      .SetHeadimg(userInfoData.headimg)
                                      .SetNickname(HttpUtility.UrlDecode(HttpUtility.UrlDecode(userInfoData.nickname)))
                                      .SetOpenid(userInfoData.openid)
                                      .SetScore(info[1])
                                      .SetSex(userInfoData.sex.Value)
                                      .SetUserID(userInfoData.id));
                        }
                    }

                }
                returnUserRecord.AddUserRecord(userRecord);
            }
            var data = returnUserRecord.Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 9002, data.Length, requestInfo.MessageNum, data)));
        }
    }
}
