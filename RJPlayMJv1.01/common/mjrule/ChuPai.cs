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
using System.Threading;

namespace MJBLL.mjrule
{
    public class ChuPai : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "13001"; }

        }

        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {

            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("ChuPai : 非法连接");
                session.Close();
                return;
            }

            var returnmsg = ReturnMsg.CreateBuilder();
            var PGH = ReturnAll.CreateBuilder();
            var chu = SendCP.ParseFrom(requestInfo.Body);
            mjuser thisuser = Gongyong.mulist.Find(u => u.RoomID == chu.Roomid && u.Openid == chu.Openid);
            if (thisuser == null)
            {
                return;
            }

            if (thisuser.ByteData == requestInfo.Key)
            {
                return;
            }
            else
            {
                thisuser.ByteData = requestInfo.Key;
            }


            model.ServerMaJiang mj = new model.ServerMaJiang();
            Room r = Gongyong.roomlist.Find(u => u.RoomID == chu.Roomid);
            if (r.room_peo != 4)
            {
                if (Gongyong.mulist.Any(w => w.RoomID == chu.Roomid && w.QYM == 0))
                {
                    thisuser.ByteData = string.Empty;
                    session.Logger.Debug("牌桌玩家没有全部定缺");
                    return;
                }
            }
            if (r.DQcz != "3001")
            {
                session.Logger.Debug("当前操作不是出牌 当前操作为：" + r.DQcz);
                thisuser.ByteData = string.Empty;
                return;
            }
            mj = thisuser.majiangs.Find(u => u.PaiHs == chu.Mj.PaiHS && u.PaiId == chu.Mj.PaiID);

            //mj.PaiHs = chu.Mj.PaiHS;
            //mj.PaiId = chu.Mj.PaiID;
            if (r.DQHY != thisuser.ZS_Fw)
            {
                thisuser.ByteData = string.Empty;
                session.Logger.Debug("不是活跃用户");
                return;
            }
            if (mj == null)
            {
                thisuser.ByteData = string.Empty;
                session.Logger.Debug("用户不存在");
                return;
            }

            if (thisuser.Is_baotin || thisuser.Is_tiant)//报听天听用户必须摸牌打牌。不能换牌
            {
                if (r.LastMoMJ != null && (chu.Mj.PaiHS != r.LastMoMJ.PaiHs || chu.Mj.PaiID != r.LastMoMJ.PaiId))
                {
                    session.Logger.Debug("出牌不是摸到的");
                    thisuser.ByteData = string.Empty;
                    return;
                }
            }
            ThreadUtility.RemoveManagedThread(thisuser.Openid);//当用户操作时 取消用户的倒计时
            r.LastChuMJ = mj;
            thisuser.SendData.Clear();
            thisuser.majiangs.Remove(mj);
            List<model.ServerMaJiang> listmjs = new List<model.ServerMaJiang>();
            listmjs.AddRange(thisuser.majiangs.ToArray());
            //StringBuilder  paidui = new StringBuilder();
            //foreach (var item in r.RoomPai)
            //{
            //    paidui.Append(item.PaiHs + ",");
            //}
            //session.Logger.Error("出牌堆" + paidui);
            string is_paixin = new Ting().GetTing(listmjs);
            thisuser.majiangs.Sort((a, b) => -b.PaiHs.CompareTo(a.PaiHs));
            r.DQcz = "";
            int hStart = 0;//胡牌的数量？
            int hzs_fw = 0;

            GetPaiXin(is_paixin, thisuser);


            int index = 0;
            List<mjuser> mjuserss = Gongyong.mulist.FindAll(u => u.RoomID == chu.Roomid);
            session.Logger.Debug("判断放炮");
            List<HuInfo> huList = new List<HuInfo>();//处理一炮多响新增一个胡牌类型对象
                                                     ///判断各种放炮
            foreach (var item in mjuserss)
            {
                HuInfo huInfo = new HuInfo();
                int Qcount = 0;
                if (r.room_peo < 4)
                {
                    switch (item.QYM)
                    {
                        case 1:
                            if (chu.Mj.PaiHS < 10)
                            {
                                Qcount++;
                            }
                            break;
                        case 2:
                            if (chu.Mj.PaiHS > 10 && chu.Mj.PaiHS < 20)
                            {
                                Qcount++;
                            }
                            break;
                        case 3:
                            if (chu.Mj.PaiHS > 20)
                            {
                                Qcount++;
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (Qcount == 0)
                {
                    if (item.ZS_Fw != thisuser.ZS_Fw)
                    {
                        List<model.ServerMaJiang> listgett = new List<model.ServerMaJiang>();
                        List<model.ServerMaJiang> listnew = new List<model.ServerMaJiang>();
                        listgett.AddRange(item.majiangs.ToArray());
                        listnew.AddRange(item.majiangs.ToArray());
                        string hxx = new Ting().GetTing(listnew);
                        model.ServerMaJiang mjh = new model.ServerMaJiang()
                        {
                            PaiHs = chu.Mj.PaiHS,
                            PaiId = chu.Mj.PaiID
                        };
                        listgett.Add(mjh);
                        GetPaiXin(hxx, item);
                        if (thisuser.Mtype == 1)
                        {
                            if (new Ting().GetTing(listgett) == "H")
                            {

                                hStart++;
                                index++;
                                hzs_fw = item.ZS_Fw;
                                r.DQHY = item.ZS_Fw;
                                session.Logger.Debug("热炮");
                                huInfo.HuIndex = item.ZS_Fw;
                                huInfo.HuType = 4;
                                huInfo.MJ = mjh;
                                huInfo.OpenID = item.Openid;
                                huInfo.PaoIndex = thisuser.ZS_Fw;
                                huInfo.RoomID = r.RoomID;
                                huList.Add(huInfo);
                                r.DQHY = item.ZS_Fw;
                                r.DQcz = "30080";
                                #region 旧热炮
                                //var sendh = SendHu.CreateBuilder();
                                //sendh.SetFWZ(item.ZS_Fw);
                                //sendh.SetFWB(thisuser.ZS_Fw);
                                //sendh.SetType(4);
                                //sendh.SetMJ(chu.Mj);
                                //sendh.SetOpenid(thisuser.Openid);
                                //sendh.SetRoomid(r.RoomID);
                                //byte[] rbyte = sendh.Build().ToByteArray();
                                // r.DQHY = item.ZS_Fw;
                                //  Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5016, rbyte.Length, requestInfo.MessageNum, rbyte)));
                                //int messageNum = (GameInformationBase.BASEAGREEMENTNUMBER + 5016);
                                //热炮必胡，所以需要服务器自己返回胡牌的信息。
                                //new GetH().ExecuteCommand(Gongyong.userlist.Find(u => u.openid == item.Openid).session,
                                //        new ProtobufRequestInfo { Body = rbyte, Key = messageNum.ToString(), Messagelength = rbyte.Length, MessageNum = messageNum, MessageResNum = 0 });

                                // session.Logger.Debug("热炮");
                                //returnmsg.SetMj(chu.Mj);
                                //returnmsg.SetFW(thisuser.ZS_Fw);
                                //byte[] ball = returnmsg.Build().ToByteArray();
                                //foreach (var newitems in mjuserss)
                                //{
                                //    UserInfo user = Gongyong.userlist.Find(u => u.openid == newitems.Openid);
                                //    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3009, ball.Length, requestInfo.MessageNum, ball)));
                                //}
                                //return; 
                                #endregion
                            }

                        }
                        else if ((item.Is_baotin || item.Is_tiant) && !item.IsGuoHu)
                        {
                            if (new Ting().GetTing(listgett) == "H")
                            {

                                hStart++;
                                index++;
                                hzs_fw = item.ZS_Fw;
                                huInfo.HuIndex = item.ZS_Fw;
                                huInfo.HuType = 2;
                                huInfo.MJ = mjh;
                                huInfo.OpenID = item.Openid;
                                huInfo.PaoIndex = thisuser.ZS_Fw;
                                huInfo.RoomID = r.RoomID;
                                huList.Add(huInfo);
                                //var sendh = ReturnHByType.CreateBuilder();
                                //sendh.SetFWZ(item.ZS_Fw);
                                //sendh.SetFWB(thisuser.ZS_Fw);
                                //sendh.SetType(2);
                                //sendh.SetMJ(chu.Mj);
                                //byte[] rbyte = sendh.Build().ToByteArray();
                                //r.DQHY = item.ZS_Fw;
                                //Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, rbyte.Length, requestInfo.MessageNum, rbyte)));

                                r.DQHY = item.ZS_Fw;
                                r.DQcz = "30080";
                                session.Logger.Debug("报听放炮");
                            }
                        }
                        #region 旧代码
                        //}

                        //if (item.ZS_Fw != thisuser.ZS_Fw)
                        //{
                        //if ((hxx == "QD" || hxx == "DD" || hxx == "DDDD" || hxx == "LQD")&& !item.IsGuoHu)
                        //{
                        //    if (item.ZS_Fw != thisuser.ZS_Fw)
                        //    {
                        //        if (new Ting().GetTing(listgett) == "H")
                        //        {

                        //            hStart++;
                        //            index++;


                        //            hzs_fw = item.ZS_Fw;
                        //            var sendh = ReturnHByType.CreateBuilder();
                        //            //sendh.SetFWZ(item.ZS_Fw);
                        //            //sendh.SetFWB(thisuser.ZS_Fw);
                        //            //sendh.SetType(2);
                        //            //sendh.SetMJ(chu.Mj);
                        //            //r.DQHY = item.ZS_Fw;
                        //            //byte[] rbyte = sendh.Build().ToByteArray();
                        //            //Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, rbyte.Length, requestInfo.MessageNum, rbyte)));
                        //            huInfo.HuIndex = item.ZS_Fw;
                        //            huInfo.HuType =2;
                        //            huInfo.MJ = mjh;
                        //            huInfo.OpenID = item.Openid;
                        //            huInfo.PaoIndex = thisuser.ZS_Fw;
                        //            huInfo.RoomID = r.RoomID;
                        //            huList.Add(huInfo);
                        //            session.Logger.Debug("大牌"+ hxx+"牌"+ chu.Mj.PaiHS);
                        //        }
                        //    }

                        //}
                        //}


                        //if (item.ZS_Fw  
                        #endregion!= thisuser.ZS_Fw)
                        //{
                        else if ((!string.IsNullOrEmpty(item.Gong)) && !item.IsGuoHu)
                        {

                            if (new Ting().GetTing(listgett) == "H")
                            {
                                hStart++;
                                index++;
                                hzs_fw = item.ZS_Fw;
                                var sendh = ReturnHByType.CreateBuilder();
                                //sendh.SetFWZ(item.ZS_Fw);
                                //sendh.SetFWB(thisuser.ZS_Fw);
                                //sendh.SetType(2);
                                //sendh.SetMJ(chu.Mj);
                                //r.DQHY = item.ZS_Fw;
                                //byte[] rbyte = sendh.Build().ToByteArray();
                                //Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, rbyte.Length, requestInfo.MessageNum, rbyte)));
                                huInfo.HuIndex = item.ZS_Fw;
                                huInfo.HuType = 2;
                                huInfo.MJ = mjh;
                                huInfo.OpenID = item.Openid;
                                huInfo.PaoIndex = thisuser.ZS_Fw;
                                huInfo.RoomID = r.RoomID;
                                huList.Add(huInfo);
                                r.DQHY = item.ZS_Fw;
                                r.DQcz = "30080";
                                session.Logger.Debug("有杠" + hxx + "牌" + chu.Mj.PaiHS);
                            }
                        }
                        //}


                        //if (item.ZS_Fw != thisuser.ZS_Fw)
                        //{

                        else if ((item.paixinfs != 5 && item.paixinfs != 0) && !item.IsGuoHu)
                        {
                            var newlist = new List<model.ServerMaJiang>();
                            newlist.AddRange(listgett);
                            Ting t = new Ting();
                            if (t.GetTing(listgett) == "H")
                            {
                                hxx = t.GetHPaiPaiXin(newlist, item.Peng.GroupBy(w => w.PaiHs).Count(), string.IsNullOrEmpty(item.Gong) ? 0 : item.Gong.Split(',').Length, chu.Mj.PaiHS);
                                t.GetPaiXin(hxx, item);
                                if (item.paixinfs != 5 && item.paixinfs != 0)
                                {
                                    hStart++;
                                    index++;
                                    hzs_fw = item.ZS_Fw;
                                    huInfo.HuIndex = item.ZS_Fw;
                                    huInfo.HuType = 2;
                                    huInfo.MJ = mjh;
                                    huInfo.OpenID = item.Openid;
                                    huInfo.PaoIndex = thisuser.ZS_Fw;
                                    huInfo.RoomID = r.RoomID;
                                    huList.Add(huInfo);
                                    r.DQHY = item.ZS_Fw;
                                    r.DQcz = "30080";
                                    session.Logger.Debug("胡牌" + hxx + "牌" + chu.Mj.PaiHS);
                                }
                            }
                        }
                        if (huList.Count == 0 || huList[0].HuType != 4)//热炮不能有碰
                        {
                            //}

                            //if (item.ZS_Fw != thisuser.ZS_Fw)
                            //{
                            UserInfo userp = Gongyong.userlist.Find(u => u.openid == item.Openid);
                            List<model.ServerMaJiang> UmjList = item.majiangs.FindAll(u => u.PaiHs == mj.PaiHs);
                            if (item.Is_tiant || item.Is_baotin)//天听或者报听 不能有碰杠牌
                            { }
                            else
                            {
                                if (UmjList.Count >= 2)
                                {
                                    index++;
                                    PGH.SetPeng(1);
                                    PGH.SetFw(thisuser.ZS_Fw);
                                    PGH.SetMj(chu.Mj);
                                    byte[] xia = PGH.Build().ToByteArray();
                                    if (huList.Count > 0 && hzs_fw != item.ZS_Fw)
                                    {
                                        RoomMsgWirte msgri = new RoomMsgWirte()
                                        {
                                            openid = item.Openid,
                                            xiaoxihao = 3008,
                                            ArrList = xia,
                                            roomid = r.RoomID,
                                            operating = "30081" + "," + thisuser.ZS_Fw

                                        };
                                        Gongyong.roommsg.Add(msgri);
                                    }
                                    else
                                    {
                                        item.SendData.Add(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, xia.Length, requestInfo.MessageNum, xia)));

                                        r.DQHY = item.ZS_Fw;
                                        r.DQcz = "30081" + "," + thisuser.ZS_Fw;
                                        r.PaiHSCZ = mj;
                                        userp.session.TrySend(item.SendData);
                                        session.Logger.Debug("方位------" + item.ZS_Fw + "--碰--" + chu.Mj.PaiHS);
                                    }
                                }
                                if (UmjList.Count == 3)
                                {
                                    index++;

                                    var returngang = GangMSG.CreateBuilder();
                                    returngang.SetFw(thisuser.ZS_Fw);
                                    returngang.SetMj(chu.Mj);
                                    returngang.SetType(1);

                                    byte[] xia = ReturnGang.CreateBuilder().AddGang(returngang).Build().ToByteArray();
                                    if (huList.Count > 0 && hzs_fw != item.ZS_Fw)
                                    {

                                        RoomMsgWirte msgri = new RoomMsgWirte()
                                        {
                                            ArrList = xia,
                                            xiaoxihao = 5022,
                                            openid = item.Openid,
                                            roomid = r.RoomID,
                                            operating = "30082,1" + "," + thisuser.ZS_Fw
                                        };
                                        Gongyong.roommsg.Add(msgri);

                                    }
                                    else
                                    {
                                        item.SendData.Add(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5022, xia.Length, requestInfo.MessageNum, xia)));

                                        r.DQHY = item.ZS_Fw;
                                        r.DQcz = "30082,1" + "," + thisuser.ZS_Fw;
                                        r.PaiHSCZ = mj;
                                        userp.session.TrySend(item.SendData);
                                        session.Logger.Debug("方位------" + item.ZS_Fw + "--杠--" + chu.Mj.PaiHS);
                                    }

                                }
                            }
                        }
                    }
                }
            }
            if (huList.Count > 0)
            {
                r.Is_Hu = true;
                session.Logger.Debug(huList);
                if (huList.Count > 1 || huList[0].HuType == 4)
                {
                    var sendh = SendHu.CreateBuilder();
                    foreach (var item in huList)
                    {
                        if (!sendh.HasFWZ)
                            sendh.SetFWZ(item.HuIndex);
                        if (!sendh.HasFWB)
                            sendh.SetFWB(item.PaoIndex);
                        sendh.SetType(item.HuType);
                        sendh.SetMJ(chu.Mj);
                        if (!sendh.HasOpenid)
                            sendh.SetOpenid(item.OpenID);
                        if (!sendh.HasRoomid)
                            sendh.SetRoomid(r.RoomID);
                        sendh.AddDXInfo(DuoXiangHu.CreateBuilder().SetDXFW(item.HuIndex).SetDXType(item.HuType));
                    }
                    byte[] rbyte = sendh.Build().ToByteArray();
                    //Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5016, rbyte.Length, requestInfo.MessageNum, rbyte)));
                    int messageNum = (GameInformationBase.BASEAGREEMENTNUMBER + 5016);
                    //热炮必胡，所以需要服务器自己返回胡牌的信息。
                    new GetH().ExecuteCommand(Gongyong.userlist.Find(u => u.openid == huList[0].OpenID).session,
                            new ProtobufRequestInfo { Body = rbyte, Key = messageNum.ToString(), Messagelength = rbyte.Length, MessageNum = messageNum, MessageResNum = 0 });
                    return;

                }
                else
                {
                    var sendh = ReturnHByType.CreateBuilder();
                    sendh.SetFWZ(huList[0].HuIndex);
                    sendh.SetFWB(huList[0].PaoIndex);
                    sendh.SetType(huList[0].HuType);
                    sendh.SetMJ(chu.Mj);
                    r.DQHY = huList[0].HuIndex;
                    byte[] rbyte = sendh.Build().ToByteArray();
                    var sendData = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, rbyte.Length, requestInfo.MessageNum, rbyte));
                    mjuserss.Find(w => w.Openid.Equals(huList[0].OpenID)).SendData.Add(sendData);
                    Gongyong.userlist.Find(u => u.openid == huList[0].OpenID).session.TrySend(sendData);
                }

            }
            else
                r.Is_Hu = false;
            thisuser.chuda.Add(mj);
            thisuser.Mtype = 0;//不将自己的摸牌类型修改 会导致碰牌后的出牌也被判断成热炮

            ///下发出的牌
            if (index > 0)
            {
                returnmsg.SetMj(chu.Mj);
                returnmsg.SetFW(thisuser.ZS_Fw);
                byte[] ball = returnmsg.Build().ToByteArray();
                foreach (var newitems in mjuserss)
                {
                    UserInfo user = Gongyong.userlist.Find(u => u.openid == newitems.Openid);
                    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3009, ball.Length, requestInfo.MessageNum, ball)));
                }
                //牌局回放 
                r.gameOperationProcess.AddGameOperationInfo(
                    GameOperationInfo.CreateBuilder()
                    .SetSerialNumber(r.gameOperationProcess.GameOperationInfoCount)
                    .SetOperationFW(thisuser.ZS_Fw)
                    .SetOperationType(2)
                    .AddMJ(chu.Mj)
                    );
            }

            ///下发冲锋鸡消息
            if (index == 0)
            {


                if (mj.PaiHs == 8 && r.is_wgj)
                {
                    int wgjc = 0;
                    foreach (var item in mjuserss)
                    {
                        int Pcount = item.Peng.FindAll(u => u.PaiHs == 8).Count;

                        int Ccount = item.chuda.FindAll(u => u.PaiHs == 8).Count;
                        if (item.ZS_Fw == thisuser.ZS_Fw)
                        {
                            Ccount = Ccount - 1;
                        }
                        if (!string.IsNullOrEmpty(item.Gong))
                        {
                            item.Gong.Remove(item.Gong.Length - 1, 1);
                            string[] G = item.Gong.Split(',');
                            foreach (var items in G)
                            {
                                string[] GOne = items.Split('|');
                                if (GOne[0] == "8")
                                {
                                    wgjc++;
                                }
                            }
                        }
                        if (Pcount > 0 || Ccount > 0)
                        {
                            wgjc++;
                        }
                    }
                    if (wgjc == 0)
                    {
                        thisuser.is_cfwg = true;
                        returnmsg.SetMsg("CFWG");
                    }
                    else
                    {
                        returnmsg.SetMsg("");
                    }


                }

                if (mj.PaiHs == 11)
                {

                    int cfjc = 0;
                    foreach (var item in mjuserss)
                    {
                        int Pcount = item.Peng.FindAll(u => u.PaiHs == 11).Count;
                        int Ccount = item.chuda.FindAll(u => u.PaiHs == 11).Count;
                        if (item.ZS_Fw == thisuser.ZS_Fw)
                        {
                            Ccount = Ccount - 1;
                        }
                        if (!string.IsNullOrEmpty(item.Gong))
                        {
                            item.Gong.Remove(item.Gong.Length - 1, 1);
                            string[] G = item.Gong.Split(',');
                            foreach (var items in G)
                            {
                                string[] GOne = items.Split('|');
                                if (GOne[0] == "11")
                                {
                                    cfjc++;
                                }
                            }
                        }
                        if (Pcount > 0 || Ccount > 0)
                        {
                            cfjc++;
                        }
                    }
                    if (cfjc == 0)
                    {
                        thisuser.Is_cfj = true;
                        returnmsg.SetMsg("CFJ");

                    }
                    else
                    {
                        returnmsg.SetMsg("");
                    }


                }

                returnmsg.SetMj(chu.Mj);
                returnmsg.SetFW(thisuser.ZS_Fw);
                byte[] ball = returnmsg.Build().ToByteArray();

                var paiCount = ReturnPaiCount.CreateBuilder();
                if (r.RoomPai.Count == 108)
                {
                    switch (r.room_peo)
                    {
                        case 2:
                            if (r.RoomPai.Count == 108)
                            {
                                paiCount.SetPaiCount(81);
                            }
                            break;
                        case 3:
                            if (r.RoomPai.Count == 108)
                            {
                                paiCount.SetPaiCount(68);
                            }
                            break;
                        case 4:
                            if (r.RoomPai.Count == 108)
                            {
                                paiCount.SetPaiCount(55);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    paiCount.SetPaiCount(r.RoomPai.Count);
                }

                byte[] returnPaiCount = paiCount.Build().ToByteArray();
                var sby = ReturnHyUser.CreateBuilder();
                foreach (var items in mjuserss)
                {

                    UserInfo user = Gongyong.userlist.Find(u => u.openid == items.Openid);
                    bool is_send = user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3009, ball.Length, requestInfo.MessageNum, ball)));
                    PGH.SetMo(1);
                    // user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7006, returnPaiCount.Length, requestInfo.MessageNum, returnPaiCount)));

                    //if (is_send)
                    //{
                    //   session.Logger.Debug("方位------" + items.ZS_Fw + "--发送成功-- 3009");
                    //}
                    //else
                    //{
                    //   session.Logger.Debug("方位------" + items.ZS_Fw + "--发送失败-- 3009");
                    //}
                    session.Logger.Debug("方位------" + items.ZS_Fw + "--发送-- 3009" + "牌" + chu.Mj.PaiHS);

                    if (thisuser.ZS_Fw == r.room_peo)
                    {
                        if (items.ZS_Fw == 1)
                        {
                            PGH.SetFw(items.ZS_Fw);
                            items.Mtype = 0;
                            byte[] xia = PGH.Build().ToByteArray();
                            items.SendData.Add(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, xia.Length, requestInfo.MessageNum, xia)));
                            user.session.TrySend(items.SendData);
                            r.DQHY = items.ZS_Fw;
                            r.DQcz = "30083";
                            session.Logger.Debug("方位------" + items.ZS_Fw + "--摸牌--");
                        }
                    }
                    else
                    {
                        if (items.ZS_Fw == thisuser.ZS_Fw + 1)
                        {
                            PGH.SetFw(items.ZS_Fw);
                            items.Mtype = 0;
                            byte[] xia = PGH.Build().ToByteArray();
                            items.SendData.Add(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, xia.Length, requestInfo.MessageNum, xia)));
                            user.session.TrySend(items.SendData);
                            r.DQHY = items.ZS_Fw;
                            r.DQcz = "30083";
                            session.Logger.Debug("方位------" + items.ZS_Fw + "--摸牌-- ");
                        }
                    }


                    #region 旧摸牌
                    /* if (thisuser.ZS_Fw == 4)
                     {
                         if (items.ZS_Fw == 1)
                         {
                             PGH.SetFw(items.ZS_Fw);
                             byte[] xia = PGH.Build().ToByteArray();

                             r.DQHY = items.ZS_Fw;
                             r.DQcz = "30083";
                             user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, xia.Length, requestInfo.MessageNum, xia)));
                         }
                     }
                     else
                     {
                         if (hStart == 0)
                         {
                             if (r.room_peo == 2)
                             {
                                 if (thisuser.ZS_Fw == 2 || items.ZS_Fw - 1 == thisuser.ZS_Fw)
                                 {

                                     if (items.ZS_Fw != thisuser.ZS_Fw)
                                     {
                                         PGH.SetFw(items.ZS_Fw);
                                         items.Mtype = 0;
                                         byte[] xia = PGH.Build().ToByteArray();
                                         items.SendData.Add(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, xia.Length, requestInfo.MessageNum, xia)));
                                         user.session.TrySend(items.SendData);
                                         r.DQHY = items.ZS_Fw;
                                         r.DQcz = "30083";
                                         session.Logger.Debug("方位------" + thisuser.ZS_Fw + "--出--" + chu.Mj.PaiHS);
                                     }

                                 }
                             }
                             else if (r.room_peo == 3)
                             {
                                 if (thisuser.ZS_Fw == 3)
                                 {
                                     if (items.ZS_Fw == 1)
                                     {
                                         PGH.SetFw(items.ZS_Fw);
                                         byte[] xia = PGH.Build().ToByteArray();
                                         r.DQHY = items.ZS_Fw;
                                         r.DQcz = "30083";
                                         items.Mtype = 0;
                                         items.SendData.Add(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, xia.Length, requestInfo.MessageNum, xia)));
                                         user.session.TrySend(items.SendData);
                                     }
                                 }
                                 else
                                 {
                                     if (items.ZS_Fw - 1 == thisuser.ZS_Fw)
                                     {
                                         PGH.SetFw(items.ZS_Fw);
                                         byte[] xia = PGH.Build().ToByteArray();
                                         r.DQHY = items.ZS_Fw;
                                         r.DQcz = "30083";
                                         items.Mtype = 0;
                                         items.SendData.Add(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, xia.Length, requestInfo.MessageNum, xia)));
                                         user.session.TrySend(items.SendData);
                                     }
                                 }
                             }
                             else
                             {
                                 if (thisuser.ZS_Fw == 4)
                                 {
                                     if (items.ZS_Fw == 1)
                                     {
                                         PGH.SetFw(items.ZS_Fw);
                                         byte[] xia = PGH.Build().ToByteArray();
                                         r.DQHY = items.ZS_Fw;
                                         r.DQcz = "30083";
                                         items.Mtype = 0;
                                         items.SendData.Add(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, xia.Length, requestInfo.MessageNum, xia)));
                                         user.session.TrySend(items.SendData);
                                     }
                                 }
                                 else
                                 {
                                     if (items.ZS_Fw - 1 == thisuser.ZS_Fw)
                                     {
                                         PGH.SetFw(items.ZS_Fw);
                                         byte[] xia = PGH.Build().ToByteArray();
                                         r.DQHY = items.ZS_Fw;
                                         r.DQcz = "30083";
                                         items.Mtype = 0;
                                         items.SendData.Add(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, xia.Length, requestInfo.MessageNum, xia)));
                                         user.session.TrySend(items.SendData);
                                     }
                                 }
                             }
                         }
                     } */
                    #endregion
                }
             
                //牌局回放 出牌
                r.gameOperationProcess.AddGameOperationInfo(
                    GameOperationInfo.CreateBuilder()
                    .SetSerialNumber(r.gameOperationProcess.GameOperationInfoCount)
                    .SetOperationFW(thisuser.ZS_Fw)
                    .SetOperationType(2)
                    .AddMJ(chu.Mj)
                    .SetMJType(returnmsg.Msg)
                    );
                // Thread.Sleep(100);
                sby.SetFw(r.DQHY);

                sby.SetCz(r.DQcz);
                byte[] hybyte = sby.Build().ToByteArray();

                foreach (var item in mjuserss)
                {
                    UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                    user.IsActive = true;
                    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7004, hybyte.Length, requestInfo.MessageNum, hybyte)));
                }
            }
            ThreadUtility.StartManagedThread(Gongyong.mulist.Find(w => w.ZS_Fw == r.DQHY && w.RoomID == r.RoomID).Openid, r.RoomID, r.DQcz);

        }
        /// <summary>
        /// 叫牌牌型赋值
        /// </summary>
        /// <param name="is_paixin"></param>
        /// <param name="thisuser"></param>
        /// <param name="is_qys"></param>
        private void GetPaiXin(string is_paixin, mjuser thisuser)
        {
            bool is_qys = false;
            ///判断牌型
            if (thisuser.majiangs[thisuser.majiangs.Count - 1].PaiHs - thisuser.majiangs[0].PaiHs < 9 && thisuser.majiangs[0].PaiHs / 10 == thisuser.majiangs[thisuser.majiangs.Count - 1].PaiHs / 10)
            {
                if (thisuser.majiangs[0].PaiHs > 10 && thisuser.majiangs[0].PaiHs < 20)
                {
                    int count = thisuser.Peng.FindAll(u => u.PaiHs < 10 || u.PaiHs > 20).Count;
                    if (!string.IsNullOrEmpty(thisuser.Gong))
                    {


                        string gang = thisuser.Gong.Remove(thisuser.Gong.Length - 1, 1);
                        string[] arr = gang.Split(',');
                        int panduan = 0;
                        foreach (var item in arr)
                        {
                            string[] newarr = item.Split('|');
                            if (int.Parse(newarr[0]) < 10 || int.Parse(newarr[0]) > 20)
                            {
                                panduan++;
                                is_qys = false;
                                break;
                            }
                        }
                        if (panduan == 0 && count == 0)
                        {
                            is_qys = true;
                        }
                    }
                    else
                    {
                        if (count == 0)
                        {
                            is_qys = true;
                        }
                    }
                }
                else if (thisuser.majiangs[0].PaiHs < 10)
                {
                    int count = thisuser.Peng.FindAll(u => u.PaiHs > 10).Count;
                    if (!string.IsNullOrEmpty(thisuser.Gong))
                    {
                        string gang = thisuser.Gong.Remove(thisuser.Gong.Length - 1, 1);
                        string[] arr = gang.Split(',');
                        int panduan = 0;
                        foreach (var item in arr)
                        {
                            string[] newarr = item.Split('|');
                            if (int.Parse(newarr[0]) > 10)
                            {
                                panduan++;
                                is_qys = false;
                                break;
                            }
                        }
                        if (panduan == 0 && count == 0)
                        {
                            is_qys = true;
                        }
                    }
                    else
                    {
                        if (count == 0)
                        {
                            is_qys = true;
                        }
                    }
                }
                else
                {
                    int count = thisuser.Peng.FindAll(u => u.PaiHs < 20).Count;
                    if (!string.IsNullOrEmpty(thisuser.Gong))
                    {
                        string gang = thisuser.Gong.Remove(thisuser.Gong.Length - 1, 1);
                        string[] arr = gang.Split(',');
                        int panduan = 0;
                        foreach (var item in arr)
                        {
                            string[] newarr = item.Split('|');
                            if (int.Parse(newarr[0]) < 20)
                            {
                                panduan++;
                                is_qys = false;
                                break;
                            }
                        }
                        if (panduan == 0 && count == 0)
                        {
                            is_qys = true;
                        }
                    }
                    else
                    {
                        if (count == 0)
                        {
                            is_qys = true;
                        }
                    }
                }
            }
            if (is_paixin != "MJ")
            {
                thisuser.Is_jiao = true;
                switch (is_paixin)
                {
                    case "QD":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 6;
                        }
                        else
                        {
                            thisuser.paixinfs = 1;
                        }

                        break;
                    case "DDDD":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 7;
                        }
                        else
                        {
                            thisuser.paixinfs = 2;
                        }

                        break;
                    case "DD":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 8;
                        }
                        else
                        {
                            thisuser.paixinfs = 3;
                        }

                        break;
                    case "LQD":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 9;
                        }
                        else
                        {
                            thisuser.paixinfs = 4;
                        }

                        break;
                    case "SP":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 10;
                        }
                        else
                        {
                            thisuser.paixinfs = 5;
                        }

                        break;
                    default:
                        break;
                }



            }
            else
            {
                thisuser.Is_tiant = false;
                thisuser.Is_jiao = false;
                thisuser.paixinfs = 0;
            }
        }


    }
}
