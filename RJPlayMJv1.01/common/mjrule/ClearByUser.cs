using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using MJBLL.common;
using System.Threading;
using MJBLL.logic;
using MJBLL.model;

namespace MJBLL.mjrule
{
    /// <summary>
    /// 返回是否同意解散
    /// </summary>
    public class ClearByUser : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "15005"; }
        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
           

                if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
                {
                    session.Logger.Debug("ClearByUser : 非法连接");
                    session.Close();
                    return;
                }
                var getdata = SendJSIdea.ParseFrom(requestInfo.Body);
                session.Logger.Debug(getdata.ToString());
                RoomClear r = Gongyong.roomclear.Find(u => u.RoomID == getdata.Roomid);
                if (r == null)
                {
                    return;
                }
                else
                {

                    if (getdata.State == 1)
                    {
                        r.RoomPeo.RemoveAll(u => u == getdata.Openid);
                        if (ThreadUtility.IsExist(r.RoomID.ToString()))
                            ThreadUtility.StartCountdownRemoveRoomThread(r.RoomID.ToString());
                    }
                    else
                    {
                        r.Status = 0;
                        r.RoomPeo.RemoveAll(u => u == getdata.Openid);
                        ThreadUtility.StartCountdownRemoveRoomThread(r.RoomID.ToString());
                    }
                    List<mjuser> listmj = Gongyong.mulist.FindAll(u => u.RoomID == getdata.Roomid);
                    if (listmj.Count == 0)
                        return;
                    int fw = listmj.Find(w => w.Openid.Equals(getdata.Openid)).ZS_Fw;
                    var rdatabyone = ReturnJSByOnew.CreateBuilder().SetNickName(Gongyong.userlist.Find(u => u.openid == getdata.Openid).nickname).SetState(getdata.State).SetFw(fw).Build();
                    byte[] rd = rdatabyone.ToByteArray();
                    foreach (var item in listmj)
                    {

                        UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);

                        user.session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5006, rd.Length, requestInfo.MessageNum, rd)));
                    }
                    if (r.RoomPeo.Count == 0)
                    {


                        #region 旧代码
                        /*
                        //var senddjs = ReturnDJS.CreateBuilder();
                        //CardUser card = Gongyong.FKUser.Find(u => u.roomid == getdata.Roomid);
                        //foreach (var item in Gongyong.mulist.FindAll(u => u.RoomID == getdata.Roomid))
                        //{
                        //    UserInfo gamer = Gongyong.userlist.Find(u => u.openid == item.Openid);
                        //    var users = Userinfo.CreateBuilder().SetHeadimg(gamer.headimg).SetNickname(gamer.nickname).SetOpenid(gamer.openid).SetSex(int.Parse(gamer.sex)).SetUserFW(item.ZS_Fw).SetUserBean(0).SetUserGold(0).Build();
                        //    var jsddr = UserAJS.CreateBuilder();
                        //    jsddr.SetUser(users);
                        //    jsddr.SetZdou(item.zwd_count);
                        //    jsddr.SetZimo(item.zm_count);
                        //    jsddr.SetAndou(item.ad_count);
                        //    jsddr.SetMdou(item.MD_count);
                        //    jsddr.SetDianpao(item.dp_count);

                        //    switch (item.ZS_Fw)
                        //    {
                        //        case 1:
                        //            jsddr.SetScare(card.dong);
                        //            break;
                        //        case 2:
                        //            jsddr.SetScare(card.nan);
                        //            break;
                        //        case 3:
                        //            jsddr.SetScare(card.xi);
                        //            break;
                        //        case 4:
                        //            jsddr.SetScare(card.bei);
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //    senddjs.AddUserjs(jsddr);
                        //    senddjs.SetState(2);

                        //}




                        //Gongyong.mulist.RemoveAll(u => u.RoomID == getdata.Roomid);
                        //Gongyong.roomlist.RemoveAll(u => u.RoomID == getdata.Roomid);
                        //var alldata = ReturnAllIdea.CreateBuilder().SetState(1).Build();
                        //byte[] bytesss = alldata.ToByteArray();
                        //byte[] bsenddjs = senddjs.Build().ToByteArray();
                        //foreach (var item in listmj)
                        //{
                        //    UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);

                        //    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5007, bytesss.Length, requestInfo.MessageNum, bytesss)));
                        //    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5014, bsenddjs.Length, requestInfo.MessageNum, bsenddjs)));
                        //    RedisUtility.GetClient().Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, user.openid, user.unionid));
                        //}
                        //RedisUtility.GetClient().Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, r.RoomID.ToString(), ""));
                        */
                        #endregion
                        if (r.Status == 1)
                        {
                            ThreadUtility.RemoveCountdownRemoveRoomThread(r.RoomID.ToString());
                            CountdownLogic countdownLogic = new CountdownLogic();
                            countdownLogic.DisbandTable(new CountdownInfo { roomID = r.RoomID, MessageNum = requestInfo.MessageNum });
                        }
                        else
                        {
                            ThreadUtility.RemoveCountdownRemoveRoomThread(r.RoomID.ToString());
                            var alldata = ReturnAllIdea.CreateBuilder().SetState(0).SetMessgaeType(0).Build();
                            byte[] bytesss = alldata.ToByteArray();
                            foreach (var item in listmj)
                            {
                                UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                                user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5007, bytesss.Length, requestInfo.MessageNum, bytesss)));
                            }
                        }
                        Gongyong.roomclear.Remove(r);
                    }
                }
           
        }


        //void remove_room(object sender) {

        //}
    }
}
