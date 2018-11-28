using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using MJBLL.common;
using MJBLL.Logic;
using MJBLL.model;

namespace MJBLL.mjrule
{
    public class JX_Game : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "15017"; }
        }
        /// <summary>
        /// 继续游戏
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {

            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("JX_Game : 非法连接");
                session.Close();
                return;
            }
            var getdata = SendGetGame.ParseFrom(requestInfo.Body);


            Room r = Gongyong.roomlist.Find(u => u.RoomID == getdata.RoomID);



            if (r == null)
            {
                return;
            }
            if (r.startgame != 2)
            {
                return;
            }
            List<mjuser> listuser = Gongyong.mulist.FindAll(u => u.RoomID == getdata.RoomID);
            Room_JX room = new Room_JX();
            room = Gongyong.room_JX.Find(u => u.room_id == getdata.RoomID);
            if (room == null)
            {
                room = new Room_JX()
                {
                    room_id = getdata.RoomID,
                    room_peo = 1,
                    UsersFW = new List<int>() { getdata.Fw }
                };


                Gongyong.room_JX.Add(room);
            }
            else
            {
                if (!room.UsersFW.Any(w => w == getdata.Fw))//已点击过继续游戏的玩家不能再次进入计算
                {
                    room.room_peo += 1;
                    room.UsersFW.Add(getdata.Fw);
                }
            }

            if (room.room_peo == r.room_peo)
            {
                //发送当前局数的信息
                foreach (var item in listuser)
                {
                    new CardsLogic().Clear(item, r);
                }
                CardsLogic logic = new CardsLogic();
                r.startgame = 1;
                int number = 0;//发牌次数，从0开始计数
                foreach (var item in listuser)
                {
                    //UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                    //ReturnStart startgame = ReturnStart.CreateBuilder().SetStart(1).Build();
                    //byte[] bstart = startgame.ToByteArray();
                    //user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2008, bstart.Length, requestInfo.MessageNum, bstart)));

                    item.SendData.Clear();
                    ReturnStartGame.Builder sendmj = ReturnStartGame.CreateBuilder();
                    ReturnGang.Builder ReturnGangMSG = ReturnGang.CreateBuilder();
                    List<model.ServerMaJiang> Ruturnjsmj = new List<model.ServerMaJiang>();
                    item.paixinfs = 5;
                    Gongyong.mulist.Remove(item);
                    if (r.IsYuanQue)
                        item.QYM = 3;
                    //logic.GetMyCards(requestInfo, sendmj, ReturnGangMSG, Ruturnjsmj, session, getdata.RoomID, item, item.Openid, ref number);
                    var dcount = r.Dcount;
                    logic.GetMyCards(requestInfo, sendmj, ReturnGangMSG, Ruturnjsmj, session, getdata.RoomID, item, item.Openid, ref dcount);
                }
                if (listuser.Any(w => w.Is_tianHu))
                {
                    var user = listuser.Find(w => w.Is_tianHu);

                    var humj = user.majiangs.Last();
                    var hupai = ReturnHByType.CreateBuilder().SetFWZ(user.ZS_Fw).SetMJ(MaJiang.CreateBuilder().SetPaiHS(humj.PaiHs).SetPaiID(humj.PaiId)).SetType(1);
                    byte[] data = hupai.Build().ToByteArray();
                    if (listuser.Any(w => w.Is_tiant))
                    {
                        RoomMsgWirte msgri = new RoomMsgWirte()
                        {
                            openid = user.Openid,
                            xiaoxihao = 5015,
                            ArrList = data,
                            roomid = user.RoomID
                            ,operating = "30080"
                        };
                        Gongyong.roommsg.Add(msgri);
                    }
                    else
                    {
                        var userSend = Gongyong.userlist.Find(w => w.openid.Equals(user.Openid));
                        userSend.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, data.Length, requestInfo.MessageNum, data)));

                    }
                    // userSendJ.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, data.Length, requestInfo.MessageNum, data)));

                }
                Gongyong.room_JX.Remove(Gongyong.room_JX.Find(w => w.room_id == getdata.RoomID));
            }

        }
    }
}
