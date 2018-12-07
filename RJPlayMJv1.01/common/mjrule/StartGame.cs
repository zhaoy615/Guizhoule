using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using MJBLL.common;
using SuperSocket.SocketBase.Command;
using MJBLL.Logic;
using MJBLL.model;
using DAL.DAL;

namespace MJBLL.mjrule
{
    /// <summary>
    /// 麻将开始发牌判断天听
    /// </summary>
    public class StartGame : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return (GameInformationBase.BASEAGREEMENTNUMBER + 7094).ToString(); }

        }



        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {

            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("StartGame : 非法连接");
                session.Close();
                return;
            }
            var result = SendConfirmationStarts.ParseFrom(requestInfo.Body);
            session.Logger.Debug(result);
            var userInfo = Gongyong.mulist.Find(u => u.RoomID == result.RoomID && u.Openid == result.Openid);
            if (userInfo == null)
                return;
            userInfo.ConfirmationStarts = true;
            List<mjuser> mjList = Gongyong.mulist.FindAll(u => u.RoomID == result.RoomID);
            var r = Gongyong.roomlist.Find(w => w.RoomID == result.RoomID);
            if (r.startgame == 1)
                return;
            if (mjList.Count != r.room_peo)
                return;

           
            foreach (var item in mjList)
            {
                if (!item.ConfirmationStarts)
                    return;
            }
            r.startgame = 1;
            ///查找当前玩家集合
            session.Logger.Debug("全部确认，开始游戏发牌" + mjList.Count);

            CardsLogic logic = new CardsLogic();
            int number = 0;//发牌次数，从0开始计数

            

            //switch (r.room_peo)
            //{
            //    case "2T":
            //        if (r.RoomPai.Count == 108)
            //        {
            //            r.RoomPai.RemoveRange(0, 27);
            //        }
            //        break;
            //    case "3T":
            //        if (r.RoomPai.Count == 108)
            //        {
            //            r.RoomPai.RemoveRange(0, 40);
            //        }
            //        break;
            //    case "4T":
            //        if (r.RoomPai.Count == 108)
            //        {
            //            r.RoomPai.RemoveRange(0, 53);
            //        }
            //        break;
            //    default:
            //        break;
            //}
            var dcount = r.Dcount;
            foreach (var item in mjList)
            {
                ReturnStartGame.Builder sendmj = ReturnStartGame.CreateBuilder();
                ReturnGang.Builder ReturnGangMSG = ReturnGang.CreateBuilder();
                List<model.ServerMaJiang> Ruturnjsmj = new List<model.ServerMaJiang>();
                item.paixinfs = 5;
                Gongyong.mulist.Remove(item);
                if (r.IsYuanQue)
                    item.QYM = 3;
                logic.GetMyCards(requestInfo, sendmj, ReturnGangMSG, Ruturnjsmj, session, result.RoomID, item, item.Openid, ref dcount);
                var user = Gongyong.userlist.Find(w => w.openid.Equals(item.Openid));

                byte[] stringnew = ReturnPaiCount.CreateBuilder().SetPaiCount(r.RoomPai.Count).Build().ToByteArray();
                user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7006, stringnew.Length, requestInfo.MessageNum, stringnew)));

            }
            if (mjList.Any(w => w.Is_tianHu))
            {
                var user = mjList.Find(w => w.Is_tianHu);

                var humj = user.majiangs.Last();
                var hupai = ReturnHByType.CreateBuilder().SetFWZ(user.ZS_Fw).SetMJ(MaJiang.CreateBuilder().SetPaiHS(humj.PaiHs).SetPaiID(humj.PaiId)).SetType(1);
                byte[] data = hupai.Build().ToByteArray();
                if (mjList.Any(w => w.Is_tiant))
                {
                    model.RoomMsgWirte msgri = new RoomMsgWirte()
                    {
                        openid = user.Openid,
                        xiaoxihao = 5015,
                        ArrList = data,
                        roomid = user.RoomID,
                        operating = "30070"
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

        }
    }
}
