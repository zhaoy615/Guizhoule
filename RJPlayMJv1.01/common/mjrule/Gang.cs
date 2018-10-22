﻿using System;
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
    public class Gang : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "14001"; }

        }


        /// <summary>
        /// 碰杠过消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {

            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug(" Gang : 非法连接");
                session.Close();
                return;
            }

            var returnmsg = ReturnMsg.CreateBuilder();

            var retumsg = ReturnPeng.CreateBuilder();
            var sendinfo = SendPeng.ParseFrom(requestInfo.Body);
            session.Logger.Debug(sendinfo);
            var jp = ReturnFJ.CreateBuilder();
            Room r = Gongyong.roomlist.Find(u => u.RoomID == sendinfo.Roomid);
            mjuser mjuser = Gongyong.mulist.Find(u => u.RoomID == sendinfo.Roomid && u.Openid == sendinfo.Openid);
            List<mjuser> mjlist = Gongyong.mulist.FindAll(u => u.RoomID == sendinfo.Roomid);
            List<model.ServerMaJiang> listmj = new List<model.ServerMaJiang>();
            model.ServerMaJiang mjfh = new model.ServerMaJiang();

            if (!string.IsNullOrEmpty(mjuser.ByteData) && mjuser.ByteData.Equals(sendinfo.ToString()))
            {
                session.Logger.Debug("消息重复");
                return;
            }
            else
            {
                mjuser.ByteData = sendinfo.ToString();
            }

            if (mjuser == null)
            {
                session.Logger.Debug("用户不存在");
                return;
            }

            if (r.DQHY != 0 && r.DQHY != mjuser.ZS_Fw)
            {
                session.Logger.Debug("不是活跃用户");
                return;
            }
            mjuser.SendData.Clear();
            ThreadUtility.RemoveManagedThread(mjuser.Openid);//当用户已做操作 取消用户的倒计时
            var zrjxx = ReturnZR.CreateBuilder();
            List<HuInfo> huList = new List<HuInfo>();//处理一炮多响新增一个胡牌类型对象
            var info = mjlist.FirstOrDefault(w => w.ZS_Fw == sendinfo.FW);
            bool isDelete = false;
            if (info != null)//先删除牌，否则过牌时判断冲锋鸡 会有问题
            {
                session.Logger.Debug("删除牌前手牌有 " + info.chuda.Count);
                isDelete= info.chuda.Remove(info.chuda.Find(w => w.PaiHs == sendinfo.Mj.PaiHS && w.PaiId == sendinfo.Mj.PaiID));
                session.Logger.Debug("删除牌前手牌后 " + info.chuda.Count);
            }
            ///不是过
            if (sendinfo.State == 1)
            {
           
                switch (sendinfo.Types)
                {
                    ///碰
                    case 1:
                        if (sendinfo.Mj.PaiHS != r.LastChuMJ.PaiHs || sendinfo.Mj.PaiID != r.LastChuMJ.PaiId)
                        {
                            session.Logger.Debug(r.LastChuMJ + "碰的牌不是最后一张打出的牌" + mjuser.Openid);
                            return;
                        }
                        listmj = mjuser.majiangs.FindAll(u => u.PaiHs == sendinfo.Mj.PaiHS);
                        List<model.ServerMaJiang> listP = mjuser.majiangs.FindAll(u => u.PaiHs == sendinfo.Mj.PaiHS);
                        if (listP.Count < 2)
                        {
                            session.Logger.Debug("碰的牌不够2张" + mjuser.Openid);
                            return;
                        }
                        mjuser.majiangs.Remove(listP[0]);
                        mjuser.majiangs.Remove(listP[1]);
                        mjuser.Peng.AddRange(listmj);
                        mjuser.Peng.Add(listmj[0]);
                        retumsg.SetState(1);
                        retumsg.SetMj(sendinfo.Mj);
                        retumsg.SetFw(mjuser.ZS_Fw);
                        retumsg.SetPfw(sendinfo.FW);

                        int cfj = 0;
                        int wgj = 0;
                        mjuser mjnewusers = Gongyong.mulist.Find(u => u.ZS_Fw == sendinfo.FW && u.RoomID == sendinfo.Roomid);
                        //判断责任鸡
                        if (sendinfo.Mj.PaiHS == 11)
                        {
                            foreach (var item in mjlist)
                            {
                                int count = item.chuda.FindAll(u => u.PaiHs == 11).Count;
                                if (count > 0)
                                {
                                    cfj++;
                                    break;
                                }
                            }
                            if (cfj == 0)
                            {

                                mjuser.Is_zrj = 1;
                                mjnewusers.Is_cfj = false;
                                mjnewusers.Is_zrj = -1;
                                zrjxx.SetGtype(1);
                                zrjxx.SetZrfw(mjnewusers.ZS_Fw);
                                zrjxx.SetDzrfw(mjuser.ZS_Fw);
                            }

                        }
                        ///判断责任乌骨
                        if (r.is_wgj)
                        {

                            if (sendinfo.Mj.PaiHS == 8)
                            {
                                foreach (var item in mjlist)
                                {
                                    int count = item.chuda.FindAll(u => u.PaiHs == 8).Count;
                                    if (count > 0)
                                    {
                                        wgj++;
                                        break;
                                    }
                                }
                                if (wgj == 0)
                                {

                                    mjuser.is_zrwg = 1;
                                    mjnewusers.is_cfwg = false;
                                    mjnewusers.is_zrwg = -1;
                                    zrjxx.SetGtype(2);
                                    zrjxx.SetZrfw(mjnewusers.ZS_Fw);
                                    zrjxx.SetDzrfw(mjuser.ZS_Fw);
                                }
                            }
                        }
                        r.DQHY = mjuser.ZS_Fw;
                        r.DQcz = "3001";//出牌
                                        //牌局回放 碰牌
                        r.gameOperationProcess.AddGameOperationInfo(
                        GameOperationInfo.CreateBuilder()
                        .SetSerialNumber(r.gameOperationProcess.GameOperationInfoCount)
                        .SetOperationFW(mjuser.ZS_Fw)
                        .SetOperationType(3)
                        .AddMJ(sendinfo.Mj)
                        .SetPengFW(mjnewusers.ZS_Fw)
                        .SetMJType(mjuser.Is_zrj == 1 ? "ZRJ" : (mjuser.is_zrwg == 1 ? "ZRWG" : ""))
                        );

                        break;
                    //杠
                    case 2:
                        int gangType = 0;
                        ///杠类型判断
                        switch (sendinfo.Gtype)
                        {
                            case "A":
                                //if (r.LastMoMJ != null && (sendinfo.Mj.PaiHS != r.LastMoMJ.PaiHs || sendinfo.Mj.PaiID != r.LastMoMJ.PaiId))
                                //{
                                //    session.Logger.Debug("M杠的牌不是最后一张摸到的牌" + mjuser.Openid);
                                //    return;
                                //}
                                if (mjuser.majiangs.Where(w => w.PaiHs == sendinfo.Mj.PaiHS).Count() < 3)
                                {
                                    session.Logger.Debug("A杠的牌不是不够3张" + mjuser.Openid);
                                    return;
                                }
                                mjuser.ad_count += 1;
                                gangType = 5;
                                break;
                            case "Z":
                                if (sendinfo.Mj.PaiHS != r.LastMoMJ.PaiHs || sendinfo.Mj.PaiID != r.LastMoMJ.PaiId)
                                {
                                    session.Logger.Debug("M杠的牌不是最后一张摸到的牌" + mjuser.Openid);
                                    return;
                                }
                                if (!mjuser.Peng.Any(u => u.PaiHs == sendinfo.Mj.PaiHS))
                                {
                                    session.Logger.Debug("Z杠的牌不是不够3张" + mjuser.Openid);
                                    return;
                                }
                                mjuser.zwd_count += 1;
                                gangType = 6;
                                break;
                            case "M":
                                if (sendinfo.Mj.PaiHS != r.LastChuMJ.PaiHs || sendinfo.Mj.PaiID != r.LastChuMJ.PaiId)
                                {
                                    session.Logger.Debug("M杠的牌不是最后一张打出的牌" + mjuser.Openid);
                                    return;
                                }
                                if (mjuser.majiangs.Where(w => w.PaiHs == sendinfo.Mj.PaiHS).Count() < 3)
                                {
                                    session.Logger.Debug("M杠的牌不是不够3张" + mjuser.Openid);
                                    return;
                                }
                                mjuser.MD_count += 1;
                                gangType = 4;
                                break;
                            case "H":
                                if (mjuser.majiangs.Where(w => w.PaiHs == sendinfo.Mj.PaiHS).Count() < 3 && !mjuser.Peng.Any(u => u.PaiHs == sendinfo.Mj.PaiHS))
                                {
                                    session.Logger.Debug("H杠的牌不是不够3张或者碰牌区没有" + mjuser.Openid);
                                    return;
                                }
                                gangType = 7;
                                break;
                            default:
                                break;
                        }

                        mjuser.majiangs.RemoveAll(u => u.PaiHs == sendinfo.Mj.PaiHS);
                        mjuser.Gong += sendinfo.Mj.PaiHS + "|" + sendinfo.FW + "|" + sendinfo.Gtype + ",";
                        retumsg.SetState(2);
                        retumsg.SetMj(sendinfo.Mj);
                        retumsg.SetFw(mjuser.ZS_Fw);
                        retumsg.SetPfw(sendinfo.FW);
                        retumsg.SetGtype(sendinfo.Gtype);
                        //var gangInfo = mjlist.FirstOrDefault(w => w.ZS_Fw == sendinfo.FW);
                        //if (gangInfo != null)
                        //    gangInfo.chuda.Remove(gangInfo.chuda.Find(w => w.PaiHs == sendinfo.Mj.PaiHS && w.PaiId == sendinfo.Mj.PaiID));
                        ///明豆
                        if (sendinfo.Gtype == "M")
                        {
                            mjuser mjnewuser = Gongyong.mulist.Find(u => u.ZS_Fw == sendinfo.FW && u.RoomID == sendinfo.Roomid);

                            if (sendinfo.Mj.PaiHS == 11)
                            {
                                mjuser.Is_zrj = 1;
                                mjnewuser.Is_cfj = false;
                                mjnewuser.Is_zrj = -1;
                                zrjxx.SetGtype(1);
                                zrjxx.SetZrfw(mjnewuser.ZS_Fw);
                                zrjxx.SetDzrfw(mjuser.ZS_Fw);
                            }

                            if (r.is_wgj)
                            {


                                if (sendinfo.Mj.PaiHS == 8)
                                {
                                    mjuser.is_zrwg = 1;
                                    mjnewuser.is_cfwg = false;
                                    mjnewuser.is_zrwg = -1;
                                    zrjxx.SetGtype(2);
                                    zrjxx.SetZrfw(mjnewuser.ZS_Fw);
                                    zrjxx.SetDzrfw(mjuser.ZS_Fw);
                                }
                            }
                        }
                        ///转弯都抢杠
                        if (sendinfo.Gtype == "Z")
                        {

                            mjuser.Peng.RemoveAll(u => u.PaiHs == sendinfo.Mj.PaiHS);
                            foreach (var item in mjlist)
                            {
                                if (item.ZS_Fw != mjuser.ZS_Fw)
                                {

                                    List<model.ServerMaJiang> mah = new List<model.ServerMaJiang>();
                                    mah.AddRange(item.majiangs.ToArray());
                                    model.ServerMaJiang mjone = new model.ServerMaJiang()
                                    {
                                        PaiHs = sendinfo.Mj.PaiHS,
                                        PaiId = sendinfo.Mj.PaiID
                                    };
                                    mah.Add(mjone);

                                    if (new Ting().GetTing(mah) == "H")
                                    {
                                        //var sendh = ReturnHByType.CreateBuilder();
                                        //sendh.SetFWZ(item.ZS_Fw);
                                        //sendh.SetFWB(mjuser.ZS_Fw);
                                        //sendh.SetType(3);
                                        //sendh.SetMJ(sendinfo.Mj);
                                        //byte[] rbyte = sendh.Build().ToByteArray();
                                        //Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, rbyte.Length, requestInfo.MessageNum, rbyte)));
                                        HuInfo huInfo = new HuInfo();
                                        huInfo.HuIndex = item.ZS_Fw;
                                        huInfo.HuType = 3;
                                        huInfo.MJ = mjone;
                                        huInfo.OpenID = item.Openid;
                                        huInfo.PaoIndex = mjuser.ZS_Fw;
                                        huInfo.RoomID = r.RoomID;
                                        huList.Add(huInfo);
                                        item.IsGrabBars = true;
                                        item.WasGrabbedUserOpenID = mjuser.Openid;
                                        r.DQHY = item.ZS_Fw;
                                    }

                                }
                            }
                            //if (mjlist.Any(w => w.IsGrabBars))
                            //    return;
                        }
                        //憨包豆
                        if (sendinfo.Gtype == "H")
                        {
                            if (mjuser.Peng.Any(u => u.PaiHs == sendinfo.Mj.PaiHS))
                            {
                                mjuser.Peng.RemoveAll(u => u.PaiHs == sendinfo.Mj.PaiHS);
                                foreach (var item in mjlist)
                                {
                                    if (item.ZS_Fw != mjuser.ZS_Fw)
                                    {

                                        List<model.ServerMaJiang> mah = new List<model.ServerMaJiang>();
                                        mah.AddRange(item.majiangs.ToArray());
                                        model.ServerMaJiang mjone = new model.ServerMaJiang()
                                        {
                                            PaiHs = sendinfo.Mj.PaiHS,
                                            PaiId = sendinfo.Mj.PaiID
                                        };
                                        mah.Add(mjone);

                                        if (new Ting().GetTing(mah) == "H")
                                        {
                                            //var sendh = ReturnHByType.CreateBuilder();
                                            //sendh.SetFWZ(item.ZS_Fw);
                                            //sendh.SetFWB(mjuser.ZS_Fw);
                                            //sendh.SetType(3);
                                            //sendh.SetMJ(sendinfo.Mj);
                                            //byte[] rbyte = sendh.Build().ToByteArray();
                                            //Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, rbyte.Length, requestInfo.MessageNum, rbyte)));

                                            HuInfo huInfo = new HuInfo();
                                            huInfo.HuIndex = item.ZS_Fw;
                                            huInfo.HuType = 3;
                                            huInfo.MJ = mjone;
                                            huInfo.OpenID = item.Openid;
                                            huInfo.PaoIndex = mjuser.ZS_Fw;
                                            huInfo.RoomID = r.RoomID;
                                            huList.Add(huInfo);

                                            item.IsGrabBars = true;
                                            item.WasGrabbedUserOpenID = mjuser.Openid;
                                            r.DQHY = item.ZS_Fw;
                                        }

                                    }
                                }
                            }
                        }
                        r.DQHY = mjuser.ZS_Fw;
                        //牌局回放 杠牌
                        r.gameOperationProcess.AddGameOperationInfo(
                        GameOperationInfo.CreateBuilder()
                        .SetSerialNumber(r.gameOperationProcess.GameOperationInfoCount)
                        .SetOperationFW(mjuser.ZS_Fw)
                        .SetOperationType(4)
                        .AddMJ(sendinfo.Mj)
                        .SetPengFW(sendinfo.FW)
                        .SetTingHuType(gangType)
                        .SetMJType(mjuser.Is_zrj == 1 ? "ZRJ" : (mjuser.is_zrwg == 1 ? "ZRWG" : ""))
                        );
                        break;
                    case 3:
                        //if (r.count == r.Dcount)
                        //{
                        //    Gongyong.roomlist.Remove(r);
                        //    Gongyong.mulist.RemoveAll(u => u.RoomID == sendinfo.Roomid);
                        //}
                        //mjfh = r.RoomPai[0];
                        //jp.SetHS(mjfh.PaiHs);
                        break;

                    default:
                        break;
                }
            }
            //过
            else
            {
                session.Logger.Debug("过牌");
                string mjType = string.Empty;

                //过类型
                if (sendinfo.Types == 1||( sendinfo.Types == 2&& sendinfo.Gtype.Equals("M")) || (sendinfo.Types == 3&& sendinfo.FW!= mjuser.ZS_Fw))
                {


                    //判断幺鸡过，是否下发冲锋鸡消息
                    if (sendinfo.Mj.PaiHS == 11)
                    {

                        bool cfj = true;
                        // List<mjuser> listnewJP = Gongyong.mulist.FindAll(U => U.RoomID == sendinfo.Roomid);
                        foreach (var item in mjlist)
                        {
                            int Ccount = item.chuda.FindAll(u => u.PaiHs == 11).Count;
                            if (Ccount > 0)
                            {
                                cfj = false;

                            }
                        }

                        if (cfj)
                        {
                            returnmsg.SetFW(sendinfo.FW);
                            returnmsg.SetMj(MaJiang.CreateBuilder().SetPaiHS(0).SetPaiID(0));
                            returnmsg.SetMsg("CFJ");
                            info.Is_cfj = true;
                            //  listnewJP.Find(w => w.ZS_Fw == sendinfo.FW).chuda.Add(new model.ServerMaJiang { PaiHs = 11, PaiId = sendinfo.HasMj ? sendinfo.Mj.PaiID : 211 });
                            mjType = returnmsg.Msg;
                            byte[] byteball = returnmsg.Build().ToByteArray();
                            foreach (var item in mjlist)
                            {

                                Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3009, byteball.Length, requestInfo.MessageNum, byteball)));
                            }
                        }
                    }

                    ///判断乌骨鸡过是否下发乌骨鸡消息
                    if (sendinfo.Mj.PaiHS == 8)
                    {

                        bool WGJ = true;
                        //  List<mjuser> listnewJPW = Gongyong.mulist.FindAll(U => U.RoomID == sendinfo.Roomid);
                        foreach (var item in mjlist)
                        {
                            int Ccount = item.chuda.FindAll(u => u.PaiHs == 8).Count;
                            if (Ccount > 0)
                            {
                                WGJ = false;

                            }
                        }

                        if (WGJ)
                        {
                            returnmsg.SetFW(sendinfo.FW);
                            returnmsg.SetMj(MaJiang.CreateBuilder().SetPaiHS(0).SetPaiID(0));
                            returnmsg.SetMsg("CFWG");
                            info.is_cfwg = true;
                            // listnewJPW.Find(w => w.ZS_Fw == sendinfo.FW).chuda.Add(new model.ServerMaJiang { PaiHs = 8, PaiId = sendinfo.HasMj ? sendinfo.Mj.PaiID : 208 });
                            mjType = returnmsg.Msg;
                            byte[] byteball = returnmsg.Build().ToByteArray();
                            foreach (var item in mjlist)
                            {

                                Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3009, byteball.Length, requestInfo.MessageNum, byteball)));
                            }

                        }
                    }
                  
                }
                if (isDelete)
                    info.chuda.Add(new model.ServerMaJiang { PaiHs = sendinfo.Mj.PaiHS, PaiId = sendinfo.HasMj ? sendinfo.Mj.PaiID : 208 });

                //牌局回放 过牌
                if (returnmsg.HasMsg)
                {
                    r.gameOperationProcess.AddGameOperationInfo(
               GameOperationInfo.CreateBuilder()
                  .SetSerialNumber(r.gameOperationProcess.GameOperationInfoCount)
                  .SetOperationFW(mjuser.ZS_Fw)
                  .SetOperationType(7)
                  .SetPengFW(returnmsg.FW)
                  .SetMJType(returnmsg.Msg)
                     );
                }
                else
                {
                    r.gameOperationProcess.AddGameOperationInfo(
             GameOperationInfo.CreateBuilder()
                .SetSerialNumber(r.gameOperationProcess.GameOperationInfoCount)
                .SetOperationFW(mjuser.ZS_Fw)
                .SetOperationType(7)
                   );
                }
                List<RoomMsgWirte> msglist = new List<RoomMsgWirte>();
                msglist = Gongyong.roommsg.FindAll(u => u.roomid == r.RoomID);
                session.Logger.Debug("过牌判断" + msglist.Count);
                if (msglist.Count > 0 && sendinfo.Types != 4 && sendinfo.Types != 5)
                {
                    foreach (var item in msglist)
                    {

                        var userInfo = Gongyong.userlist.Find(u => u.openid == item.openid);
                        if (userInfo != null)
                        {
                            userInfo.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + item.xiaoxihao, item.ArrList.Length, requestInfo.MessageNum, item.ArrList)));
                            r.DQHY = mjlist.Find(u => u.Openid.Equals(item.openid)).ZS_Fw;
                            r.DQcz = item.operating;
                            session.Logger.Debug("过牌后根据之前的消息分配活跃用户" + r.DQHY);
                        }
                    }

                    Gongyong.roommsg.RemoveAll(u => u.roomid == r.RoomID);
                    retumsg.SetState(4);
                    retumsg.SetMj(sendinfo.Mj);
                }
                else if (mjuser.IsGrabBars)
                {
                    session.Logger.Debug("过牌判断抢杠");
                    mjuser.IsGrabBars = false;
                    var PGH = ReturnAll.CreateBuilder();
                    PGH.SetMo(1);
                    PGH.SetFw(mjuser.ZS_Fw);
                    PGH.SetMType(1);
                    mjuser.Mtype = 1;
                    byte[] bytegou = PGH.Build().ToByteArray();
                    var user = Gongyong.userlist.Find(u => u.openid.Equals(mjuser.WasGrabbedUserOpenID));
                    var mjaure = mjlist.Find(w => w.Openid.Equals(mjuser.WasGrabbedUserOpenID));
                    r.DQHY = mjaure.ZS_Fw;
                    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, bytegou.Length, requestInfo.MessageNum, bytegou)));
                }

                ///下发摸牌出牌消息
                else
                {
                    session.Logger.Debug("过牌摸牌");
                    var shy = ReturnHyUser.CreateBuilder();
                    CardUser card = Gongyong.FKUser.Find(u => u.roomid == mjuser.RoomID);
                    if (sendinfo.Types == 4 && mjuser.Is_tiant)//天听过牌
                    {
                        mjuser.Is_tiantX = 0;
                        mjuser.Is_tiant = false;
                        if (!mjlist.Any(w => w.Is_tiantX == -1))
                        {
                            r.DQHY = card.win;
                            r.DQcz = "3001";
                            byte[] startHY = ReturnHyUser.CreateBuilder().SetCz("3001").SetFw(card.win).Build().ToByteArray();
                            foreach (var item in mjlist)
                            {
                                UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                                user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7004, startHY.Length, requestInfo.MessageNum, startHY)));
                            }
                            msglist = Gongyong.roommsg.FindAll(u => u.roomid == r.RoomID);
                            if (msglist.Count > 0)
                            {
                                // Room r = Gongyong.roomlist.Find(u => u.RoomID == r.RoomID);
                                foreach (var msgitem in msglist)
                                {
                                    var userInfo = Gongyong.userlist.Find(u => u.openid == msgitem.openid);
                                    if (userInfo != null)
                                    {

                                        var mjUser = mjlist.Find(u => u.Openid.Equals(msgitem.openid));
                                        r.DQHY = mjUser.ZS_Fw;
                                        mjUser.SendData.Clear();
                                        var data = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + msgitem.xiaoxihao, msgitem.ArrList.Length, requestInfo.MessageNum, msgitem.ArrList));
                                        userInfo.session.TrySend(data);
                                        //session.Logger.Debug("过牌后根据之前的消息分配活跃用户" + r.DQHY);
                                    }
                                }
                                Gongyong.roommsg.RemoveAll(u => u.roomid == r.RoomID);
                            }
                        }
                        return;
                    }
                    else if (sendinfo.Types == 5 && mjuser.Is_tianHu)
                    {
                        mjuser.Is_tianHu = false;
                        r.DQHY = card.win;
                        r.DQcz = "3001";
                        byte[] startHY = ReturnHyUser.CreateBuilder().SetCz("3001").SetFw(card.win).Build().ToByteArray();
                        foreach (var item in mjlist)
                        {
                            UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                            user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7004, startHY.Length, requestInfo.MessageNum, startHY)));
                        }
                        return;
                    }

                    else if ((sendinfo.Gtype.ToUpper() != "M" && sendinfo.Types == 2) || (sendinfo.Types == 3 && sendinfo.FW == mjuser.ZS_Fw))
                    {
                        if (mjuser.majiangs.Any(w => w.PaiId == sendinfo.Mj.PaiID)|| sendinfo.Mj.PaiID==0)// 有些情况在放炮过牌的时候会让过牌的用户出牌，然而应该摸牌
                        {
                            shy.SetCz("3001");
                            shy.SetFw(mjuser.ZS_Fw);
                            retumsg.SetState(4);
                            retumsg.SetMj(sendinfo.Mj);
                            retumsg.SetFw(mjuser.ZS_Fw);
                            retumsg.SetPfw(sendinfo.FW);
                            r.DQcz = "3001";
                        }
                        else
                        {
                            UserInfo user = Gongyong.userlist.Find(u => u.openid == mjuser.Openid);
                            r.DQcz = "30083";
                            var PGH = ReturnAll.CreateBuilder();
                            PGH.SetMo(1);
                            PGH.SetFw(mjuser.ZS_Fw);
                            mjuser.Mtype = 0;
                            byte[] bsss = PGH.Build().ToByteArray();
                            user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, bsss.Length, requestInfo.MessageNum, bsss)));
                        }
                    }
                    else //if( (sendinfo.Types == 3 && sendinfo.FW != mjuser.ZS_Fw))
                    {
                        if (sendinfo.Types == 3 && sendinfo.FW != mjuser.ZS_Fw)
                            mjuser.IsGuoHu = true;
                        shy.SetCz("30083");
                        // int peocount = Convert.ToInt32(r.room_peo.Remove(1, 1));
                        int peocount = r.room_peo;
                        if (sendinfo.FW == peocount)
                        {
                            r.DQHY = 1;
                            r.DQcz = "30083";
                            string openid = mjlist.Find(u => u.ZS_Fw == 1).Openid;
                            UserInfo user = Gongyong.userlist.Find(u => u.openid == openid);


                            shy.SetFw(1);

                            var PGH = ReturnAll.CreateBuilder();
                            PGH.SetMo(1);
                            PGH.SetFw(mjuser.ZS_Fw);
                            mjuser.Mtype = 0;
                            byte[] bsss = PGH.Build().ToByteArray();
                            user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, bsss.Length, requestInfo.MessageNum, bsss)));

                        }
                        else
                        {
                            r.DQHY = sendinfo.FW + 1;
                            r.DQcz = "30083";
                            string openid = mjlist.Find(u => u.ZS_Fw == sendinfo.FW + 1).Openid;

                            UserInfo user = Gongyong.userlist.Find(u => u.openid == openid);
                            var PGH = ReturnAll.CreateBuilder();
                            PGH.SetMo(1);
                            mjuser.Mtype = 0;
                            PGH.SetFw(Gongyong.mulist.Find(u => u.ZS_Fw == sendinfo.FW + 1).ZS_Fw);
                            shy.SetFw(Gongyong.mulist.Find(u => u.ZS_Fw == sendinfo.FW + 1).ZS_Fw);
                            byte[] bsss = PGH.Build().ToByteArray();
                            user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, bsss.Length, requestInfo.MessageNum, bsss)));
                        }
                        retumsg.SetState(4);
                        if (sendinfo.Mj.HasPaiHS)
                            retumsg.SetMj(sendinfo.Mj);
                        retumsg.SetFw(mjuser.ZS_Fw);
                        retumsg.SetPfw(sendinfo.FW);
                        r.DQcz = "30083";
                    }



                    byte[] senhy = shy.Build().ToByteArray();

                    session.Logger.Debug(shy.Build());
                    foreach (var item in mjlist)
                    {

                        UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                        user.IsActive = true;
                        user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7004, senhy.Length, requestInfo.MessageNum, senhy)));
                    }
                }
            }
            if (huList.Count > 0)
            {
                if (huList.Count > 1 || huList[0].HuType == 3)
                {
                    r.Is_Hu = true;
                    var sendh = SendHu.CreateBuilder();
                    foreach (var item in huList)
                    {
                        if (!sendh.HasFWZ)
                            sendh.SetFWZ(item.HuIndex);
                        if (!sendh.HasFWB)
                            sendh.SetFWB(item.PaoIndex);
                        sendh.SetType(item.HuType);
                        sendh.SetMJ(sendinfo.Mj);
                        if (!sendh.HasOpenid)
                            sendh.SetOpenid(item.OpenID);
                        if (!sendh.HasRoomid)
                            sendh.SetRoomid(r.RoomID);
                        sendh.AddDXInfo(DuoXiangHu.CreateBuilder().SetDXFW(item.HuIndex).SetDXType(item.HuType));
                    }
                    byte[] rbyte = sendh.Build().ToByteArray();
                    //Gongyong.userlist.Find(u => u.openid == item.Openid).session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5016, rbyte.Length, requestInfo.MessageNum, rbyte)));
                    int messageNum = (GameInformationBase.BASEAGREEMENTNUMBER + 5016);
                    //抢杠必胡，所以需要服务器自己返回胡牌的信息。
                    new GetH().ExecuteCommand(Gongyong.userlist.Find(u => u.openid == huList[0].OpenID).session,
                            new ProtobufRequestInfo { Body = rbyte, Key = messageNum.ToString(), Messagelength = rbyte.Length, MessageNum = messageNum, MessageResNum = 0 });
                }
                else
                {
                    var sendh = ReturnHByType.CreateBuilder();
                    sendh.SetFWZ(huList[0].HuIndex);
                    sendh.SetFWB(huList[0].PaoIndex);
                    sendh.SetType(huList[0].HuType);
                    sendh.SetMJ(sendinfo.Mj);
                    r.DQHY = huList[0].HuIndex;
                    byte[] rbyte = sendh.Build().ToByteArray();
                    var sendData = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, rbyte.Length, requestInfo.MessageNum, rbyte));
                    mjlist.Find(w => w.Openid.Equals(huList[0].OpenID)).SendData.Add(sendData);
                    Gongyong.userlist.Find(u => u.openid == huList[0].OpenID).session.TrySend(sendData);
                }
            }
            else if (sendinfo.State == 1 && sendinfo.Types == 2)
            {
                r.Is_Hu = false;
                var PGH = ReturnAll.CreateBuilder();
                PGH.SetMo(1);
                PGH.SetFw(mjuser.ZS_Fw);
                PGH.SetMType(1);
                mjuser.Mtype = 1;
                byte[] bytegou = PGH.Build().ToByteArray();
                session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, bytegou.Length, requestInfo.MessageNum, bytegou)));

            }
            else if(sendinfo.State==2)//过牌后 会摸牌，不能强制修改为 出牌
            { }
            else
            {
                r.Is_Hu = false;
                r.DQcz = "3001";
            }

            var settle = ReturnJS.CreateBuilder();
            session.Logger.Debug("处理完成下发消息");
            foreach (var item in mjlist)
            {
                UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);

                //if (sendinfo.Types == 3 && sendinfo.State == 1)
                //{
                //    byte[] jpr = jp.Build().ToByteArray();
                //    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7005, jpr.Length, requestInfo.MessageNum, jpr)));
                //    byte[] settlebyte = settle.Build().ToByteArray();
                //    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7009, settlebyte.Length, requestInfo.MessageNum, settlebyte)));
                //}
                //else
                //{
                byte[] b = retumsg.Build().ToByteArray();
                user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 4002, b.Length, requestInfo.MessageNum, b)));
                //}


                if (zrjxx.Dzrfw != 0)
                {
                    byte[] zrj = zrjxx.Build().ToByteArray();

                    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5018, zrj.Length, requestInfo.MessageNum, zrj)));

                }


            }
            ThreadUtility.StartManagedThread(Gongyong.mulist.Find(w => w.ZS_Fw == r.DQHY && w.RoomID == r.RoomID).Openid, r.RoomID,r.DQcz);
        }
    }
}