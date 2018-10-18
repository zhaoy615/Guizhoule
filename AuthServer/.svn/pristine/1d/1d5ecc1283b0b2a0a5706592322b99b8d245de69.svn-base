using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MJBLL.common;
using MJBLL.model;
using MJBLL.mjrule;
using DAL.DAL;
using DAL.Model;

namespace MJBLL.logic
{
    public class CountdownLogic : IMyTimer
    {
        public string Name { get; set ; }
        public Timer MyTimer { get ; set; }

        public void CallBack(object state)
        {
            //倒计时后自动解散牌桌用户体验不好， 应倒计时后自动为玩家选择确定
            var countdownInfo = state as CountdownInfo;
            RoomClear r = Gongyong.roomclear.Find(u => u.RoomID == countdownInfo.roomID);
            if (r == null)
            {
                return;
            }
            int messageNum = (GameInformationBase.BASEAGREEMENTNUMBER + 5005);
            List<SendJsInfo> list = new List<SendJsInfo>();
            foreach (var item in r.RoomPeo)
            {
                var jsInfo = SendJSIdea.CreateBuilder().SetRoomid(countdownInfo.roomID).SetOpenid(item).SetState(1).Build();
                byte[] rbyte = jsInfo.ToByteArray();
                list.Add(new SendJsInfo { data = rbyte, OpenID = item });
            }
            foreach (var item in list)
            {
                new ClearByUser().ExecuteCommand(Gongyong.userlist.Find(u => u.openid.Equals(item.OpenID)).session,
                         new ProtobufRequestInfo { Body = item.data, Key = messageNum.ToString(), Messagelength = item.data.Length, MessageNum = messageNum, MessageResNum = 0 });

            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialization()
        {
            MyTimer = new Timer(CallBack, new CountdownInfo { roomID = int.Parse(Name), MessageNum = 15007 }, GameInformationBase.DISBANDTABLECOUNTDOWN, 0);
        }
        /// <summary>
        /// 重置时间
        /// </summary>
        public void Start()
        {
            MyTimer.Change(GameInformationBase.DISBANDTABLECOUNTDOWN, 0);
        }

      
     
        /// <summary>
        /// 解散牌桌
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="MessageNum"></param>
       public void DisbandTable(object state)
        {

            var countdownInfo = state as CountdownInfo;

            RoomClear r = Gongyong.roomclear.Find(u => u.RoomID == countdownInfo.roomID);
            if (r == null)
            {

                return;
            }
            ThreadUtility.RemoveManagedThreadByRoomID(countdownInfo.roomID);//取消托管
            List<mjuser> listmj = Gongyong.mulist.FindAll(u => u.RoomID == countdownInfo.roomID);
            var senddjs = ReturnDJS.CreateBuilder();
            CardUser card = Gongyong.FKUser.Find(u => u.roomid == countdownInfo.roomID);
            string userWinLose = string.Empty;
            foreach (var item in listmj)
            {
                UserInfo gamer = Gongyong.userlist.Find(u => u.openid == item.Openid);
                if (gamer == null)
                    continue;
                var users = Userinfo.CreateBuilder().SetHeadimg(gamer.headimg).SetNickname(gamer.nickname).SetOpenid(gamer.openid).SetSex(int.Parse(gamer.sex)).SetUserFW(item.ZS_Fw).SetUserBean(0).SetUserGold(0).SetUserID(gamer.UserID).Build();
                var jsddr = UserAJS.CreateBuilder();
                jsddr.SetUser(users);
                jsddr.SetZdou(item.zwd_count);
                jsddr.SetZimo(item.zm_count);
                jsddr.SetAndou(item.ad_count);
                jsddr.SetMdou(item.MD_count);
                jsddr.SetDianpao(item.dp_count);

                switch (item.ZS_Fw)
                {
                    case 1:
                        jsddr.SetScare(card == null ? 0 : card.dong);
                        break;
                    case 2:
                        jsddr.SetScare(card == null ? 0 : card.nan);
                        break;
                    case 3:
                        jsddr.SetScare(card == null ? 0 : card.xi);
                        break;
                    case 4:
                        jsddr.SetScare(card == null ? 0 : card.bei);
                        break;
                    default:
                        break;
                }
                senddjs.AddUserjs(jsddr);
                senddjs.SetState(2);
                if (!string.IsNullOrEmpty(userWinLose))
                {
                    userWinLose += "|";
                }
                userWinLose += gamer.UserID + "," + jsddr.Scare;
            }
            senddjs.SetState(2);
            Gongyong.mulist.RemoveAll(u => u.RoomID == countdownInfo.roomID);
            var roomInfo = Gongyong.roomlist.Find(w => w.RoomID == countdownInfo.roomID);
            Gongyong.roomlist.RemoveAll(u => u.RoomID == countdownInfo.roomID);
            var alldata = ReturnAllIdea.CreateBuilder().SetState(1).SetMessgaeType(3).Build();
            byte[] bytesss = alldata.ToByteArray();
            byte[] bsenddjs = senddjs.Build().ToByteArray();
            foreach (var item in listmj)
            {
                UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                if (user != null)
                {
                    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5007, bytesss.Length, countdownInfo.MessageNum, bytesss)));//yi
                    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5014, bsenddjs.Length, countdownInfo.MessageNum, bsenddjs)));
                    RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, user.openid, user.unionid));
                }
            }
            foreach (var item in Gongyong.msg.FindAll(w => w.RoomID == r.RoomID))
            {
                Gongyong.msg.Remove(item);
            }

            RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, r.RoomID.ToString(), ""));

            new RoomInfoDAL().UpdateEndRoomInfoByRoomInfoID(new RoomInfo { RoomInfoID = roomInfo.RoomInfoID, EndTime = DateTime.Now, EndStatus = 3, UserWinLose = userWinLose });
            foreach (var item in listmj)
            {
                UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                new userRecordLOGDAL().Add(new userRecordlogLOG { UserID = user.UserID, RoomInfoID = roomInfo.RoomInfoID, UserWinLose = userWinLose, EndTime = DateTime.Now, CreateDate = roomInfo.CreateDate, RoomID=r.RoomID });
            }
        }

     
     
    }

    public class SendJsInfo
    {
        public string OpenID { get; set; }
        public byte[] data { get; set; }
    }
    public class CountdownInfo
    {
        public int roomID { get; set; }
        public int MessageNum { get ; set; }
    }
}