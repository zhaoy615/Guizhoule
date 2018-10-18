using MJBLL.common;
using MJBLL.mjrule;
using MJBLL.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MJBLL.logic
{
   public class IsManagedLogic:IMyTimer
    {
        /// <summary>
        /// 线程名称
        /// </summary>
        public string Name { get; set; }
        public Timer MyTimer { get; set; }
        /// <summary>
        /// 房间ID
        /// </summary>
        public int roomID { get; set; }
        /// <summary>
        /// 用户openID
        /// </summary>
        public string openID { get; set; }
        /// <summary>
        /// 当前操作编号30083:摸牌， 30082：杠牌， 30081：碰， 30080：胡，30070：天胡,30071天听，   3001 出牌 3002选择缺一门
        /// 30082，1：1,明杠，2转弯杠，3暗杠，4憨包杠
        /// </summary>
        public string currentOperation{ get; set; }
        public void CallBack(object state)
        {
            var roomInfo = Gongyong.roomlist.Find(w => w.RoomID == roomID);
            if (roomInfo == null)
                return;
            var muInfo = Gongyong.mulist.Find(w => w.Openid.Equals(openID));
            if (muInfo == null)
                return;

            //lock (ThreadUtility.myObject)
            //{ }
            if (roomInfo.QuickCard)//只有快速出牌才会有10秒出牌roomInfo.DQHY== mulist.ZS_Fw
            {
                var info = new ProtobufRequestInfo();
                var userInfo = Gongyong.userlist.Find(w => w.openid.Equals(muInfo.Openid));
                if (currentOperation.IndexOf("30071") >= 0 && muInfo.Is_tiantX == -1)//天听
                {
                    userInfo.session.Logger.Debug("10秒后自动30071:" + userInfo.openid);
                    info = new ProtobufRequestInfo();
                    // var userInfo = Gongyong.userlist.Find(w => w.openid.Equals(muInfo.Openid));
                    var guo = SendPeng.CreateBuilder()
                        .SetState(2)
                        .SetOpenid(userInfo.openid)
                        .SetRoomid(roomID)
                        .SetTypes(4);
                    info = new ProtobufRequestInfo();

                    guo.SetFW(roomInfo.DQHY);
                    info.Body = guo.Build().ToByteArray();
                    info.Key = "14001";
                    info.Messagelength = info.Body.Length;
                    info.MessageNum = 14001;
                    info.MessageResNum = 0;
                    new Gang().ExecuteCommand(userInfo.session, info);
                }
                else if (currentOperation.IndexOf("3002") >= 0 && muInfo.QYM == 0)
                {
                    userInfo.session.Logger.Debug("10秒后自动3002:" + userInfo.openid);
                    //foreach (var item in Gongyong.mulist.FindAll(w =>w.RoomID==roomID&& w.QYM == 0))
                    //{  }
                    info = new ProtobufRequestInfo();
                    userInfo = Gongyong.userlist.Find(w => w.openid.Equals(muInfo.Openid));
                    var qym = muInfo.majiangs.Select(w => new { Tong = (w.PaiHs < 10 ? 1 : 0), Tiao = (w.PaiHs > 10 && w.PaiHs < 20 ? 1 : 0), Wan = (w.PaiHs > 20 ? 1 : 0) });//遍历所有花色
                    var mydata = from r in qym
                                 group r by new { r.Tiao, r.Tong, r.Wan } into g
                                 select new { key = g.Key, number = g.Count() };//统计花色
                    var key = mydata.OrderBy(w => w.number).FirstOrDefault().key;//取出最少的花色
                    info.Body = SendQYM.CreateBuilder().SetType(key.Tiao == 1 ? 2 : (key.Tong == 1 ? 1 : 3)).SetOpenid(muInfo.Openid).Build().ToByteArray();
                    info.Key = "15011";
                    info.Messagelength = info.Body.Length;
                    info.MessageNum = 14001;
                    info.MessageResNum = 0;
                    new QYM().ExecuteCommand(userInfo.session, info);

                }
                else if (roomInfo.DQHY == muInfo.ZS_Fw)
                {
                    // var muInfo = Gongyong.mulist.Find(w => w.ZS_Fw == roomInfo.DQHY && w.RoomID == roomID);

                    info = new ProtobufRequestInfo();
                    if (currentOperation.Equals("30083"))
                    {
                        if (muInfo.ByteData == "13002")//因为摸牌逻辑比较多处理时间长，
                        {
                            return;
                        }
                        if (userInfo != null)
                        {
                            var mp = SendMP.CreateBuilder().SetMType(0).SetOpenid(muInfo.Openid).SetRoomid(roomID).Build();
                            var data = mp.ToByteArray();
                            info.Body = data;
                            info.Key = "13002";
                            info.Messagelength = data.Length;
                            info.MessageNum = 13002;
                            info.MessageResNum = 0;
                            new MoPai().ExecuteCommand(userInfo.session, info);
                            userInfo.session.Logger.Debug("10秒后自动摸牌" + mp);
                        }
                    }

                    if (currentOperation.IndexOf("30082") >= 0)//杠牌
                    {
                        userInfo.session.Logger.Debug("10秒后自动30082:" + userInfo.openid);
                        var guo = SendPeng.CreateBuilder()
                            .SetState(2)
                            .SetMj(MaJiang.CreateBuilder()
                            .SetPaiHS(roomInfo.PaiHSCZ.PaiHs)
                            .SetPaiID(roomInfo.PaiHSCZ.PaiId))
                                    .SetOpenid(userInfo.openid)
                                    .SetRoomid(roomID)
                                    .SetTypes(2);
                        info = new ProtobufRequestInfo();
                        var dqcz = currentOperation.Split(',');
                        switch (dqcz[1])
                        {
                            case "1":
                                guo.SetFW(int.Parse(dqcz[2])).SetGtype("M");
                                break;
                            case "2":
                                guo.SetFW(roomInfo.DQHY).SetGtype("Z");
                                break;
                            case "3":
                                guo.SetFW(roomInfo.DQHY).SetGtype("A");
                                break;
                            case "4":
                                guo.SetFW(roomInfo.DQHY).SetGtype("h");
                                break;
                        }
                        info.Body = guo.Build().ToByteArray();
                        info.Key = "14001";
                        info.Messagelength = info.Body.Length;
                        info.MessageNum = 14001;
                        info.MessageResNum = 0;
                        new Gang().ExecuteCommand(userInfo.session, info);
                    }
                    else if (currentOperation.IndexOf("30081") >= 0)//碰
                    {
                        userInfo.session.Logger.Debug("10秒后自动30081:" + userInfo.openid);
                        var guo = SendPeng.CreateBuilder()
                            .SetState(2)
                            .SetMj(MaJiang.CreateBuilder()
                            .SetPaiHS(roomInfo.PaiHSCZ.PaiHs)
                            .SetPaiID(roomInfo.PaiHSCZ.PaiId))
                            .SetOpenid(userInfo.openid)
                            .SetRoomid(roomID)
                            .SetTypes(1);
                        info = new ProtobufRequestInfo();
                        var dqcz = currentOperation.Split(',');
                        guo.SetFW(int.Parse(dqcz[1]));
                        info.Body = guo.Build().ToByteArray();
                        info.Key = "14001";
                        info.Messagelength = info.Body.Length;
                        info.MessageNum = 14001;
                        info.MessageResNum = 0;
                        new Gang().ExecuteCommand(userInfo.session, info);
                    }

                    else if (currentOperation.IndexOf("30070") >= 0)//天胡
                    {
                        userInfo.session.Logger.Debug("10秒后自动30070:" + userInfo.openid);
                        var guo = SendPeng.CreateBuilder()
                           .SetState(2)
                           .SetOpenid(userInfo.openid)
                           .SetRoomid(roomID)
                           .SetTypes(5);
                        info = new ProtobufRequestInfo();
                        var dqcz = currentOperation.Split(',');
                        guo.SetFW(roomInfo.DQHY);
                        info.Body = guo.Build().ToByteArray();
                        info.Key = "14001";
                        info.Messagelength = info.Body.Length;
                        info.MessageNum = 14001;
                        info.MessageResNum = 0;
                        new Gang().ExecuteCommand(userInfo.session, info);
                    }
                    else if (currentOperation.IndexOf("30080") >= 0)//胡
                    {
                        userInfo.session.Logger.Debug("30080:" + userInfo.openid);
                        var guo = SendPeng.CreateBuilder()
                           .SetState(2)
                           .SetOpenid(userInfo.openid)
                           .SetRoomid(roomID)
                           .SetTypes(3).SetMj(MaJiang.CreateBuilder()
                            .SetPaiHS(roomInfo.PaiHSCZ.PaiHs)
                            .SetPaiID(roomInfo.PaiHSCZ.PaiId));
                        info = new ProtobufRequestInfo();
                       // var dqcz = currentOperation.Split(',');
                        guo.SetFW(roomInfo.DQHY);
                        info.Body = guo.Build().ToByteArray();
                        info.Key = "14001";
                        info.Messagelength = info.Body.Length;
                        info.MessageNum = 14001;
                        info.MessageResNum = 0;
                        new Gang().ExecuteCommand(userInfo.session, info);
                    }
                    else if (currentOperation.IndexOf("3001") >= 0)//出牌
                    {
                        userInfo.session.Logger.Debug("10秒后自动3001:" + userInfo.openid);
                        var chu = SendCP.CreateBuilder()
                         .SetRoomid(roomID)
                         .SetOpenid(userInfo.openid)
                         .SetType(0);
                        switch (muInfo.QYM)
                        {
                            case 1:
                                if (muInfo.majiangs.Any(u => u.PaiHs < 10))
                                {
                                    var mj = muInfo.majiangs.Find(u => u.PaiHs < 10);
                                    chu.SetMj(MaJiang.CreateBuilder()
                                             .SetPaiHS(mj.PaiHs)
                                             .SetPaiID(mj.PaiId));
                                }
                                else
                                {
                                    chu.SetMj(MaJiang.CreateBuilder()
                                             .SetPaiHS(muInfo.majiangs.Last().PaiHs)
                                             .SetPaiID(muInfo.majiangs.Last().PaiId));
                                }
                                break;
                            case 2:
                                if (muInfo.majiangs.Any(u => u.PaiHs > 10 && u.PaiHs < 20))
                                {
                                    var mj = muInfo.majiangs.Find(u => u.PaiHs > 10 && u.PaiHs < 20);
                                    chu.SetMj(MaJiang.CreateBuilder()
                                             .SetPaiHS(mj.PaiHs)
                                             .SetPaiID(mj.PaiId));
                                }
                                else
                                {
                                    chu.SetMj(MaJiang.CreateBuilder()
                                             .SetPaiHS(muInfo.majiangs.Last().PaiHs)
                                             .SetPaiID(muInfo.majiangs.Last().PaiId));
                                }
                                break;
                            case 3:
                                if (muInfo.majiangs.Any(u => u.PaiHs > 20))
                                {
                                    var mj = muInfo.majiangs.Find(u => u.PaiHs > 20);
                                    chu.SetMj(MaJiang.CreateBuilder()
                                             .SetPaiHS(mj.PaiHs)
                                             .SetPaiID(mj.PaiId));
                                }
                                else
                                {
                                    chu.SetMj(MaJiang.CreateBuilder()
                                             .SetPaiHS(muInfo.majiangs.Last().PaiHs)
                                             .SetPaiID(muInfo.majiangs.Last().PaiId));
                                }
                                break;
                            default:
                                chu.SetMj(MaJiang.CreateBuilder()
                                            .SetPaiHS(muInfo.majiangs.Last().PaiHs)
                                            .SetPaiID(muInfo.majiangs.Last().PaiId));
                                break;
                        }

                        info = new ProtobufRequestInfo();
                        info.Body = chu.Build().ToByteArray();
                        info.Key = "13001";
                        info.Messagelength = info.Body.Length;
                        info.MessageNum = 14001;
                        info.MessageResNum = 0;
                        new ChuPai().ExecuteCommand(userInfo.session, info);
                    }

                    //if (roomInfo.DQHY == muInfo.ZS_Fw)
                    //    this.CallBack(null);
                    if (userInfo.session != null && userInfo.session.Connected)//发送托管请求
                    {
                        var data = ReturnManaged.CreateBuilder().SetState(1).Build().ToByteArray();
                        userInfo.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2011, data.Length, 0, data)));
                    }
                }
            }
            //  GC.Collect();
            // MyTimer.Dispose();
           // this.Dispose();
           
        }

        public void Initialization()
        {
            MyTimer = new Timer(CallBack, null, GameInformationBase.SYSTEMHOSTING, 0);
        }

        public void Start()
        {
           
            // MyTimer.Change(GameInformationBase.SYSTEMHOSTING, 0);
            MyTimer = new Timer(CallBack, null, GameInformationBase.SYSTEMHOSTING, 0);
        }
        public void Dispose()
        {
            MyTimer.Dispose();
        }
        public void Change()
        {

             MyTimer.Change(GameInformationBase.SYSTEMHOSTING, 0);
            // MyTimer = new Timer(CallBack, null, GameInformationBase.SYSTEMHOSTING, 0);
        }
    }
   
}
