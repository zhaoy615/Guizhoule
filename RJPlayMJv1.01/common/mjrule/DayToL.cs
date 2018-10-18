using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using MJBLL.common;
using MJBLL.model;

namespace MJBLL.mjrule
{
    /// <summary>
    /// 天听报听返回
    /// </summary>
    public class DayToL : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "15001"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {

            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("DayToL : 非法连接");
                session.Close();
                return;
            }
            var getMsg = SendBT.ParseFrom(requestInfo.Body);
            List<mjuser> listuser = Gongyong.mulist.FindAll(u => u.RoomID == getMsg.RoomID);
            mjuser mjuser = Gongyong.mulist.Find(u => u.Openid == getMsg.Openid && u.RoomID == getMsg.RoomID);
            var room = Gongyong.roomlist.Find(w => w.RoomID == getMsg.RoomID);
            if (room == null)
                return;
            if (mjuser == null)
                return;
            if (getMsg.Type == 1 && mjuser.majiangs.Count != 14)//天听，报听时手牌还未扣除，因此为14张
                return;
            if ((getMsg.Type == 2 && !mjuser.Is_tiant))
                return;
            int fw = mjuser.ZS_Fw;

            if (mjuser.ByteData == requestInfo.Key)
            {
                return;
            }
            else
            {
                mjuser.ByteData = requestInfo.Key;
            }
            Ting t = new Ting();
            List<model.ServerMaJiang> Returnmj = new List<model.ServerMaJiang>();
            Returnmj.AddRange(mjuser.majiangs.ToArray());
            if (getMsg.Type == 1 && !mjuser.Is_tianHu)
            {


                List<model.ServerMaJiang> Ruturnjsmj = new List<model.ServerMaJiang>();

                Ruturnjsmj = t.ReturnJMJ(Returnmj);
                Returnmj.Remove(Returnmj.Find(w => w.PaiHs == getMsg.Mj.PaiHS && w.PaiId == getMsg.Mj.PaiID));
                if (!Ruturnjsmj.Any(w => w.PaiHs == getMsg.Mj.PaiHS))
                    return;//出的牌必须是打出听牌的集合中的一张
            }
            if (getMsg.Type == 2 && t.GetTing(Returnmj) == "MJ")
            {
                mjuser.Is_tiant = false;
                return;
            }
            bool isShenTianHu = false;
            List<mjuser> list = new List<mjuser>();
            foreach (var item in listuser)
            {
                list.Add(item);
                if (!isShenTianHu)
                    item.SendData.Clear();
                if (item.Openid.Equals(getMsg.Openid))
                {
                    switch (getMsg.Type)
                    {
                        case 1:
                            if (mjuser.Is_tianHu)
                            {
                                mjuser.Is_tianHu = false;
                            }
                            var roomInfo = Gongyong.roomlist.FirstOrDefault(w => w.RoomID.Equals(getMsg.RoomID));
                            if (roomInfo != null)
                                roomInfo.LastMoMJ = new model.ServerMaJiang { PaiHs = getMsg.Mj.PaiHS, PaiId = getMsg.Mj.PaiID };
                            item.Is_baotin = true;
                            break;
                        case 2:
                            item.Is_tiant = true;
                            CardUser cu = Gongyong.FKUser.Find(u => u.roomid == getMsg.RoomID);
                            item.Is_tiantX = 1;
                            byte[] startHY = ReturnHyUser.CreateBuilder().SetCz("3001").SetFw(cu.win).Build().ToByteArray();
                            if (!listuser.Any(w => w.Is_tiantX == -1))
                            {

                                if (room != null)
                                {
                                    room.DQHY = cu.win;
                                    room.DQcz = "3001";
                                }
                                List<mjuser> listmjuserstart = Gongyong.mulist.FindAll(u => u.RoomID == getMsg.RoomID);

                                foreach (var items in listmjuserstart)
                                {
                                    UserInfo users = Gongyong.userlist.Find(u => u.openid == items.Openid);
                                    users.IsActive = true;
                                    users.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7004, startHY.Length, requestInfo.MessageNum, startHY)));
                                }
                                var msglist = Gongyong.roommsg.FindAll(u => u.roomid == getMsg.RoomID);
                                if (msglist.Count > 0)
                                {
                                    Room r = Gongyong.roomlist.Find(u => u.RoomID == getMsg.RoomID);
                                    foreach (var msgitem in msglist)
                                    {
                                        var userInfo = Gongyong.userlist.Find(u => u.openid == msgitem.openid);
                                        if (userInfo != null)
                                        {
                                            isShenTianHu = true;
                                            var mjUser = listmjuserstart.Find(u => u.Openid.Equals(msgitem.openid));
                                            r.DQHY = mjUser.ZS_Fw;
                                            if (!list.Any(w => w.Openid.Equals(msgitem.openid)))//如果外面的遍历已经遍历过当前用户 则不需要清理集合
                                                mjUser.SendData.Clear();
                                            var data = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + msgitem.xiaoxihao, msgitem.ArrList.Length, requestInfo.MessageNum, msgitem.ArrList));
                                            mjUser.SendData.Add(data);
                                            userInfo.session.TrySend(data);

                                            //session.Logger.Debug("过牌后根据之前的消息分配活跃用户" + r.DQHY);
                                        }
                                    }
                                    Gongyong.roommsg.RemoveAll(u => u.roomid == r.RoomID);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                if (getMsg.Type == 1)
                {
                    var renturnmsg = ReturnBTMsg.CreateBuilder().SetFw(fw).Build();
                    byte[] r = renturnmsg.ToByteArray();
                    var data = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5002, r.Length, requestInfo.MessageNum, r));
                    item.SendData.Add(data);
                    user.session.TrySend(data);
                }
                else
                {
                    var renturnmsg = ReturnTT.CreateBuilder().SetFw(fw).Build();
                    byte[] r = renturnmsg.ToByteArray();
                    var data = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5013, r.Length, requestInfo.MessageNum, r));
                    item.SendData.Add(data);
                    user.session.TrySend(data);
                }
            }

            //牌局回放 摸牌

            room.gameOperationProcess.AddGameOperationInfo(
            GameOperationInfo.CreateBuilder()
            .SetSerialNumber(room.gameOperationProcess.GameOperationInfoCount)
            .SetOperationFW(mjuser.ZS_Fw)
            .SetOperationType(6)
            .SetTingHuType(getMsg.Type + 1)
            );

        }
    }
}
