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
    /// 缺一门
    /// </summary>
    public class QYM : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "15011"; }
        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {

            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("QYM : 非法连接");
                session.Close();
                return;
            }
            ///获取缺一门数据
            var getdata = SendQYM.ParseFrom(requestInfo.Body);
            ///根据openid找到用户
            mjuser userrun = Gongyong.mulist.Find(u => u.Openid == getdata.Openid);
            if (userrun == null)
                return;

            if (userrun.ByteData == requestInfo.Key)
            {
                return;
            }
            else
            {
                userrun.ByteData = requestInfo.Key;
            }

            ///如果用户为空则直接返回
            if (userrun == null)
            {
                return;
            }

            if (userrun.QYM != 0 && !string.IsNullOrEmpty(userrun.QYM.ToString()))
            {
                return;
            }
            ///设置用户缺的麻将
            userrun.QYM = getdata.Type;
            ///根据房间ID找到当前房间
            List<mjuser> userlist = Gongyong.mulist.FindAll(u => u.RoomID == userrun.RoomID);
            mjuser mjlist = Gongyong.mulist.Find(u => u.RoomID == userrun.RoomID && (u.QYM == 0 || string.IsNullOrEmpty(u.QYM.ToString())));

            CardUser cu = Gongyong.FKUser.Find(u => u.roomid == userrun.RoomID);

            if (mjlist == null)
            {
                ThreadUtility.RemoveManagedThread(getdata.Openid);//当用户操作时 取消用户的倒计时
                var returndata = ReturnAYM.CreateBuilder();
                var mjuserAll = Gongyong.mulist.FindAll(u => u.RoomID == userrun.RoomID);
                var R = Gongyong.roomlist.Find(u => u.RoomID == userrun.RoomID);
                Ting t = new Ting();
                var ReturnGangMSG = ReturnGang.CreateBuilder();
                foreach (var item in mjuserAll)
                {
                    item.SendData.Clear();
                    ReturnTTATH.Builder sendmj = ReturnTTATH.CreateBuilder();
                    UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                    //sendmj.AddMj(MaJiang.CreateBuilder());
                    List<model.ServerMaJiang> listT = new List<model.ServerMaJiang>();
                    listT.AddRange(item.majiangs.ToArray());
                    var Ruturnjsmj = new List<model.ServerMaJiang>();
                   
                    string hxx = t.GetTing(listT);
                    if (hxx != "MJ")
                    {
                        if (cu.win == item.ZS_Fw)
                        {
                            if (hxx == "H")
                            {
                                switch (item.QYM)
                                {
                                    case 1:
                                        if (item.majiangs.FindAll(u => u.PaiHs < 10).Count <= 0)
                                        {
                                            sendmj.SetState(214);
                                            item.Is_tianHu = true;
                                        }
                                        break;
                                    case 2:
                                        if (item.majiangs.FindAll(u => u.PaiHs > 10 && u.PaiHs < 20).Count <= 0)
                                        {
                                            sendmj.SetState(214);
                                            item.Is_tianHu = true;
                                        }
                                        break;
                                    case 3:
                                        if (item.majiangs.FindAll(u => u.PaiHs > 20).Count <= 0)
                                        {
                                            sendmj.SetState(214);
                                            item.Is_tianHu = true;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }

                            else if (R.room_peo < 4)
                            {

                                switch (item.QYM)
                                {
                                    case 1:
                                        if (item.majiangs.FindAll(u => u.PaiHs < 10).Count <= 0)
                                        {
                                            ///庄家报听判断
                                            List<model.ServerMaJiang> JPMJ = new List<model.ServerMaJiang>();
                                            JPMJ.AddRange(item.majiangs.ToArray());

                                            Ruturnjsmj = t.ReturnJMJ(JPMJ);
                                            //item.Is_tiant = true;
                                        }

                                        break;
                                    case 2:
                                        if (item.majiangs.FindAll(u => u.PaiHs > 10 && u.PaiHs < 20).Count <= 0)
                                        {
                                            ///庄家报听判断
                                            List<model.ServerMaJiang> JPMJ = new List<model.ServerMaJiang>();
                                            JPMJ.AddRange(item.majiangs.ToArray());
                                            Ruturnjsmj = t.ReturnJMJ(JPMJ);
                                            //item.Is_tiant = true;
                                        }
                                        break;
                                    case 3:
                                        if (item.majiangs.FindAll(u => u.PaiHs > 20).Count <= 0)
                                        {
                                            ///庄家报听判断
                                            List<model.ServerMaJiang> JPMJ = new List<model.ServerMaJiang>();
                                            JPMJ.AddRange(item.majiangs.ToArray());
                                            Ruturnjsmj = t.ReturnJMJ(JPMJ);
                                            //item.Is_tiant = true;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                ///庄家报听判断
                                List<model.ServerMaJiang> JPMJ = new List<model.ServerMaJiang>();
                                JPMJ.AddRange(item.majiangs.ToArray());
                                Ruturnjsmj = t.ReturnJMJ(JPMJ);
                                // item.Is_tiant = true;
                            }
                        }
                        else
                        {
                            if (R.room_peo < 4)
                            {
                                switch (item.QYM)
                                {
                                    case 1:
                                        if (item.majiangs.FindAll(u => u.PaiHs < 10).Count <= 0)
                                        {

                                            sendmj.SetState(213);
                                            var TT = ReturnTT.CreateBuilder().SetFw(item.ZS_Fw);
                                            byte[] TTR = TT.Build().ToByteArray();
                                            List<mjuser> listmjuser = Gongyong.mulist.FindAll(u => u.RoomID == item.RoomID);
                                            //foreach (var mjuser in listmjuser)
                                            //{
                                            //    UserInfo user = Gongyong.userlist.Find(u => u.openid == mjuser.Openid);
                                            //    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5013, TTR.Length, GameInformationBase.BASEAGREEMENTNUMBER + 7093, TTR)));
                                            //}
                                            item.Is_tiant = true;
                                            R.DQHY = item.ZS_Fw;
                                            R.DQcz = "30071";
                                            item.Is_tiantX = -1;
                                        }
                                        break;
                                    case 2:
                                        if (item.majiangs.FindAll(u => u.PaiHs > 10 && u.PaiHs < 20).Count <= 0)
                                        {

                                            sendmj.SetState(213);
                                            var TT = ReturnTT.CreateBuilder().SetFw(item.ZS_Fw);
                                            byte[] TTR = TT.Build().ToByteArray();
                                            List<mjuser> listmjuser = Gongyong.mulist.FindAll(u => u.RoomID == item.RoomID);
                                            R.DQHY = item.ZS_Fw;
                                            R.DQcz = "30071";
                                            item.Is_tiant = true;
                                            item.Is_tiantX = -1;
                                        }
                                        break;
                                    case 3:
                                        if (item.majiangs.FindAll(u => u.PaiHs > 20).Count <= 0)
                                        {

                                            sendmj.SetState(213);
                                            var TT = ReturnTT.CreateBuilder().SetFw(item.ZS_Fw);
                                            byte[] TTR = TT.Build().ToByteArray();
                                            List<mjuser> listmjuser = Gongyong.mulist.FindAll(u => u.RoomID == item.RoomID);

                                            item.Is_tiant = true;
                                            R.DQHY = item.ZS_Fw;
                                            R.DQcz = "30071";
                                            item.Is_tiantX = -1;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                sendmj.SetState(213);
                                var TT = ReturnTT.CreateBuilder().SetFw(item.ZS_Fw);
                                byte[] TTR = TT.Build().ToByteArray();
                                List<mjuser> listmjuser = Gongyong.mulist.FindAll(u => u.RoomID == item.RoomID);
                                item.Is_tiant = true;
                                item.Is_tiantX = -1;
                                R.DQHY = item.ZS_Fw;
                                R.DQcz = "30071";
                            }
                        }
                    }
                    if (Ruturnjsmj.Count > 0)
                    {
                        var returntp = ReturnTP.CreateBuilder();
                        var tmj = MaJiang.CreateBuilder();
                        foreach (var mjItem in Ruturnjsmj)
                        {
                            tmj.SetPaiHS(mjItem.PaiHs);
                            tmj.SetPaiID(mjItem.PaiId);
                            returntp.AddMj(tmj);
                        }
                        byte[] tmjsr = returntp.Build().ToByteArray();
                        var data = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2009, tmjsr.Length, requestInfo.MessageNum, tmjsr));
                        item.SendData.Add(data);
                        user.session.TrySend(data);
                    }
                    if (sendmj.HasState)
                    {
                        var data = sendmj.Build().ToByteArray();
                        var dataArray = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2010, data.Length, requestInfo.MessageNum, data));
                        item.SendData.Add(dataArray);
                        user.session.TrySend(dataArray);
                    }

                    //验证杠牌
                    if (cu.win == item.ZS_Fw)
                    {
                       // listT.AddRange(item.majiangs.ToArray());

                        var listchuan = listT.GroupBy(w => w.PaiHs).Select(w => new { PaiHs = w.Key, Count = w.Count() });
                        foreach (var gangInfo in listchuan.Where(w => w.Count == 4))
                        {

                            switch (item.QYM)
                            {
                                case 1:
                                    if (gangInfo.PaiHs > 10)
                                    {
                                        var PGH = ReturnAll.CreateBuilder();
                                        var mjG = MaJiang.CreateBuilder();
                                        mjG.SetPaiHS(gangInfo.PaiHs);
                                        mjG.SetPaiID(401);

                                        ReturnGangMSG.AddGang(GangMSG.CreateBuilder().SetFw(cu.win).SetMj(mjG).SetType(3));
                                    }
                                    break;
                                case 2:
                                    if (gangInfo.PaiHs < 10 && gangInfo.PaiHs > 20)
                                    {
                                        var PGH = ReturnAll.CreateBuilder();
                                        var mjG = MaJiang.CreateBuilder();
                                        mjG.SetPaiHS(gangInfo.PaiHs);
                                        mjG.SetPaiID(401);

                                        ReturnGangMSG.AddGang(GangMSG.CreateBuilder().SetFw(cu.win).SetMj(mjG).SetType(3));
                                    }
                                    break;
                                case 3:
                                    if (gangInfo.PaiHs < 20)
                                    {
                                        var PGH = ReturnAll.CreateBuilder();
                                        var mjG = MaJiang.CreateBuilder();
                                        mjG.SetPaiHS(gangInfo.PaiHs);
                                        mjG.SetPaiID(401);

                                        ReturnGangMSG.AddGang(GangMSG.CreateBuilder().SetFw(cu.win).SetMj(mjG).SetType(3));
                                    }
                                    break;
                                default:
                                    break;
                            }

                        }


                        if (ReturnGangMSG.GangCount > 0)
                        {
                            byte[] bytegang = ReturnGangMSG.Build().ToByteArray();
                            user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5022, bytegang.Length, requestInfo.MessageNum, bytegang)));
                        }
                    }
                }


                if (userlist.Any(w => w.Is_tianHu))
                {
                    var user = userlist.Find(w => w.Is_tianHu);
                    user.SendData.Clear();
                    var humj = user.majiangs.Last();
                    var hupai = ReturnHByType.CreateBuilder().SetFWZ(user.ZS_Fw).SetMJ(MaJiang.CreateBuilder().SetPaiHS(humj.PaiHs).SetPaiID(humj.PaiId)).SetType(1);
                    byte[] data = hupai.Build().ToByteArray();
                    if (userlist.Any(w => w.Is_tiant))
                    {
                        RoomMsgWirte msgri = new RoomMsgWirte()
                        {
                            openid = user.Openid,
                            xiaoxihao = 5015,
                            ArrList = data,
                            roomid = user.RoomID
                            ,
                            operating = "30070"
                        };
                        Gongyong.roommsg.Add(msgri);
                    }
                    else
                    {
                        var userSend = Gongyong.userlist.Find(w => w.openid.Equals(user.Openid));

                        var dataArray = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, data.Length, requestInfo.MessageNum, data));
                        user.SendData.Add(dataArray);
                        userSend.session.TrySend(dataArray);
                        R.DQHY = user.ZS_Fw;
                        R.DQcz = "30070";
                        ThreadUtility.StartManagedThread(user.Openid, R.RoomID, R.DQcz);
                    }
                    // userSendJ.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, data.Length, requestInfo.MessageNum, data)));

                }
                else if (userlist.Any(w => w.Is_tiant))
                {
                    R.DQHY = 0;
                    R.DQcz = "30071";
                    foreach (var item in userlist)
                    {
                        ThreadUtility.StartManagedThread(item.Openid, R.RoomID, R.DQcz);
                    }
                }
                else
                {
                    byte[] startHY = ReturnHyUser.CreateBuilder().SetCz("3001").SetFw(cu.win).Build().ToByteArray();
                    R.DQHY = cu.win;
                    R.DQcz = "3001";
                    foreach (var item in userlist)
                    {
                        UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                        user.IsActive = true;
                        user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7004, startHY.Length, GameInformationBase.BASEAGREEMENTNUMBER + 7093, startHY)));

                    }
                    ThreadUtility.StartManagedThread(userlist.Find(w => w.ZS_Fw == cu.win).Openid, R.RoomID, R.DQcz);
                }
                ///下发缺牌信息
                foreach (var item in userlist)
                {
                    var senddata = ReturnQYM.CreateBuilder().SetFw(item.ZS_Fw).SetType(item.QYM);
                    returndata.AddQP(senddata);
                    //牌局回放 缺一门
                    R.gameOperationProcess.AddGameOperationInfo(
                        GameOperationInfo.CreateBuilder()
                        .SetSerialNumber(R.gameOperationProcess.GameOperationInfoCount)
                        .SetOperationFW(item.ZS_Fw)
                        .SetOperationType(8)
                        .SetQYM(item.QYM)
                        );
                }
                byte[] RBT = returndata.Build().ToByteArray();




                foreach (var item in userlist)
                {
                    UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                    var data = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5012, RBT.Length, requestInfo.MessageNum, RBT));
                    item.SendData.Add(data);
                    user.session.TrySend(data);

                }
            }



        }
    }
}
