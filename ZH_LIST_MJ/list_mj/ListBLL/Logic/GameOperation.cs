using DAL.DAL;
using ListBLL.common;
using ListBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.Logic
{
    public class GameOperation : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11020"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var gameOperation = SendGameOperation.ParseFrom(requestInfo.Body);
            RedisLoginModel olduser = RedisUtility.Get<RedisLoginModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, gameOperation.Openid, gameOperation.Unionid));
            if (olduser == null)
                return;
            int GroupID = gameOperation.HasGroupID ? gameOperation.GroupID : 0;
            var serverGameOperation = ReturnGameOperation.CreateBuilder();
            switch (gameOperation.Operation)
            {

                case 1:
                    if (ISUserInGruop(gameOperation))
                    {
                        var reslut = RedisUtility.GetServerIP(GameInformationBase.DEFAULTGAMESERVERNAME, requestInfo.MessageNum, session, 1, gameOperation.Openid, gameOperation.Unionid);
                        if(reslut == true)
                        {
                            //根据groupid来查找所有在线的圈子session
                            var groupList = Gongyong.userlist.FindAll(w => { return w.GroupID.Contains(GroupID) && w.session != session; });
                            for(var i = 0; i < groupList.Count; i++)
                            {
                                Console.WriteLine("get : "+groupList[i].nickname+" , ",groupList[i]);
                            }

                            //向日志里面添加朋友圈耗卡信息
                            //GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
                            //var userInfo = Gongyong.userlist.Find(w => { return w.session.Equals(session); });
                            //var listRecord = groupInfoDAL.AddCreateRoomRecord(userInfo.UserID, GroupID,);

                        }


                    }
                    else//不是圈内成员
                    {
                        var data = serverGameOperation.SetUnionid(gameOperation.Unionid).SetOpenid(gameOperation.Openid).SetStatus(-2).Build().ToByteArray();
                        session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1021, data.Length, requestInfo.MessageNum, data)));
                    }
                    break;
                case 2:
                    if (!gameOperation.HasRoomID)
                        return;
                    var roomInfo = RedisUtility.Get<RedisGameRoomInfo>(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, gameOperation.RoomID, string.Empty));
                   var ddzRoomInfo = RedisUtility.Get<RedisDDZGameRoomInfo>(RedisUtility.GetKey(GameInformationBase.COMMUNITYDDZROOMINFO, gameOperation.RoomID, string.Empty));
                    var status = 0;

                    if (roomInfo == null && ddzRoomInfo == null)
                    {
                        status = 3;
                    }
                    else if ((roomInfo != null && roomInfo.room.listOpenid.Count >= roomInfo.room.room_peo) || (ddzRoomInfo != null && ddzRoomInfo.room.listOpenid.Count >= 3))
                    {
                        status = 2;
                    }
                    else
                    {
                        if (ISUserInGruop(gameOperation))
                        {
                            status = 1;//1:加入成功
                            RedisUtility.GetServerIP(roomInfo.ServerName, requestInfo.MessageNum, session, 1, gameOperation.Openid, gameOperation.Unionid, false, 0, status, roomInfo==null?1:0);
                        }
                        else//不是圈内成员
                        {
                            var data = serverGameOperation.SetUnionid(gameOperation.Unionid).SetOpenid(gameOperation.Openid).SetStatus(-2).Build().ToByteArray();
                            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1021, data.Length, requestInfo.MessageNum, data)));
                        }
                    }
                    if (status != 1)
                    {
                        var data = ReturnRoomAdd.CreateBuilder().SetStart(status).Build().ToByteArray();
                        session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7090, data.Length, requestInfo.MessageNum, data)));
                    }
                    break;

            }
        }
        /// <summary>
        /// 判断用户是否是圈子成员
        /// </summary>
        /// <param name="gameOperation"></param>
        /// <returns></returns>
        private bool ISUserInGruop(SendGameOperation gameOperation)
        {
            if (gameOperation.HasGroupID)
            {
               return Gongyong.userlist.Find(w => w.unionid.Equals(gameOperation.Unionid) && w.GroupID.Any(q => q == gameOperation.GroupID))!=null;
            }
            else
            {
                return true;
            }
        }
    }
}
