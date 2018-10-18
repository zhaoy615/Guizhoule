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
    /// <summary>
    /// 根据圈子ID获取朋友圈用户信息
    /// </summary>
    public class GetGroupUserInfoByGroupID : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name => "11034";

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var sendData = SendGroupUserInfoByGroupID.ParseFrom(requestInfo.Body);
            var userInfo = RedisUtility.Get<RedisUserInfoModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, sendData.Openid, sendData.Unionid));
            if (userInfo == null || !userInfo.GroupID.Any(w => w == sendData.GroupID))
            {
               // session.Close();
                return;
            }
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
            var groupInfo = groupInfoDAL.GetGroupInfoByGroupID(sendData.GroupID, userInfo.Type);
            var returnGroupInfo = ReturnGroupUserInfoByGroupID.CreateBuilder();
          
            if (groupInfo != null)
            {
               var list= groupInfoDAL.GetGroupStaffInfoByGroupID(sendData.GroupID);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        var userInfoDB = new mjuserinfoDAL().GetModel(item.GroupUserID);
                        if (userInfoDB != null)
                        {
                            var groupUserInfo = GroupUserInfo.CreateBuilder();
                            groupUserInfo.SetGroupUserID(item.GroupUserID).SetNickName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(userInfoDB.nickname)))
                                .SetPicture(userInfoDB.headimg).SetStatus(0);
                            if (Gongyong.userlist.Any(w => w.UserID.Equals(item.GroupUserID)))
                                groupUserInfo.SetStatus(1);
                            if (RedisUtility.Get<RedisGameModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, userInfoDB.openid, userInfoDB.unionid)) != null)
                                groupUserInfo.SetStatus(2);
                            returnGroupInfo.AddUserList(groupUserInfo);
                        }
                    }
                    var data = returnGroupInfo.SetStatus(1).Build().ToByteArray();
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1035, data.Length, requestInfo.MessageNum, data)));
                    return;
                }
            }
            var dataF = returnGroupInfo.SetStatus(0).Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1035, dataF.Length, requestInfo.MessageNum, dataF)));


        }
    }
}
