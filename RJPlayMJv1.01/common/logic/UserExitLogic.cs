using DAL.DAL;
using DAL.Model;
using MJBLL.common;
using MJBLL.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.logic
{
  public  class UserExitLogic
    {
        /// <summary>
        /// 用户退出牌桌
        /// </summary>
        /// <param name="usermj"></param>
        /// <param name="roomID"></param>
        /// <param name="openid"></param>
        /// <param name="messageNum"></param>
        /// <param name="session"></param>
        /// <param name="isExit">是否已退出</param>
        public void UserExit(mjuser usermj, int roomID, string openid, int messageNum, GameSession session, bool isExit = false)
        {
           

            List<mjuser> listmjuser = Gongyong.mulist.FindAll(u => u.RoomID == roomID);
            ///判断是否该房间是否存在用户
            if (listmjuser.Count <= 0)
            {
                return;
            }
            
            ///向剩余用户下发推出玩家
            var returnData = ReturnRemoveUser.CreateBuilder().SetOpenid(openid).SetStatus(1);
            Room r = Gongyong.roomlist.Find(u => u.RoomID == roomID);
            if (r == null)
                return;
            //判断房间已开始游戏
            if (r.startgame == 1)
            {
                returnData.SetStatus(0);
                byte[] returnbyte = returnData.Build().ToByteArray();
                if(!isExit)
                session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5009, returnbyte.Length, messageNum, returnbyte)));
            }
            else
            {

                byte[] returnbyte = returnData.Build().ToByteArray();
                if(isExit)
               listmjuser.Remove(listmjuser.First(w => w.Openid.Equals(openid)));
                r.listOpenid.Remove(openid);
                foreach (var item in listmjuser)
                {
                    UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                    if(user!=null&&user.session!=null)
                    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5009, returnbyte.Length, messageNum, returnbyte)));
                    
                    if(user.openid.Equals(usermj.Openid))
                    //将用户游戏信息更新
                    RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, user.openid, user.unionid));

                }
                Gongyong.mulist.Remove(usermj);
            }
          
           
        }

        /// <summary>
        /// 解散牌桌
        /// </summary>
        /// <param name="r"></param>
        /// <param name="messageNum">返回消息号</param>
        /// <param name="type">解散类型</param>
        public void DissolutionRoom(Room r,int messageNum, int type)
        {
            List<mjuser> listuser = Gongyong.mulist.FindAll(u => u.RoomID == r.RoomID);
            var Returnjs = ReturnAllIdea.CreateBuilder().SetState(1).SetMessgaeType(type);
            byte[] Sdata = Returnjs.Build().ToByteArray();
            //if (Gongyong.userlist.Count == 0)
            //    return;
            foreach (var item in listuser)
            {
                UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                if (user != null)
                {
                    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5007, Sdata.Length, messageNum, Sdata)));
                    //将用户游戏信息更新                 
                    RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, user.openid, user.unionid));
                }
            }
        
            Gongyong.roomlist.Remove(r);
            Gongyong.mulist.RemoveAll(u => u.RoomID == r.RoomID);
            RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, r.RoomID.ToString(), ""));
            new RoomInfoDAL().UpdateEndRoomInfoByRoomInfoID(new RoomInfo { RoomInfoID = r.RoomInfoID, EndTime = DateTime.Now, EndStatus = type });
        }

        internal void UserExit(mjuser muInfo, int roomID, string openid, int v1, object p, bool v2)
        {
            throw new NotImplementedException();
        }
    }
}
