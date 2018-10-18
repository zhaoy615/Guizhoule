using MJBLL.common;
using MJBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.mjrule
{
   public class AddServer : ICommand<GameSession, ProtobufRequestInfo>
    {

        public string Name
        {
            get { return "11011"; }

        }
        /// <summary>
        /// 加入服务器
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            
            SendAddServer sendInfo = SendAddServer.ParseFrom(requestInfo.Body);
            var key = RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, sendInfo.Openid, sendInfo.Unionid);
           
            var userInfo = RedisUtility.Get<RedisUserInfoModel>(key);
         
            var returnAddServer = ReturnAddServer.CreateBuilder();

            if (userInfo == null)
            {
                returnAddServer.SetStatus(2);
            }
            else
            {
                returnAddServer.SetStatus(1);
                var info = Gongyong.userlist.Find(w => w.openid.Equals(userInfo.Openid));
                if (info == null)
                {
                    var user = new UserInfo(userInfo);
                    user.session = session;
                    Gongyong.userlist.Add(user);
                    session.Logger.Debug("加入游戏服务器成功----" + sendInfo.Openid + "------" + DateTime.Now);
                }
                else
                {
                    info.nickname = userInfo.Nickname;
                    info.sex = userInfo.Sex;
                    info.headimg = userInfo.Headimg;
                    if (info.session != null)
                        info.session.Close();
                    info.session = session;
                    mjuser mju = Gongyong.mulist.Find(u => u.Openid == info.openid);

                    if (mju != null)
                    {
                        mju.ConnectionStatus = 1;
                        // SendCL(session, info.openid, requestInfo);
                    }
                }
            }
            var data = returnAddServer.Build().ToByteArray();
            session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1012, data.Length, requestInfo.MessageNum, data)));

        }
        /// <summary>
        /// 断线重连操作
        /// </summary>
        /// <param name="session"></param>
        /// <param name="openid"></param>
        /// <param name="requestInfo"></param>
        //public void SendCL(GameSession session, string openid, ProtobufRequestInfo requestInfo)
        //{
        //    mjuser mj = Gongyong.mulist.Find(u => u.Openid == openid);
        //    List<mjuser> mjlist = Gongyong.mulist.FindAll(u => u.RoomID == mj.RoomID);
        //    var roommsg = ReturnRoomMsg.CreateBuilder();

        //    Room rm = Gongyong.roomlist.Find(u => u.RoomID == mj.RoomID);

        //    var userinfo = Userinfo.CreateBuilder();
        //    ///返回重连状态
        //    var ifconn = ReturnRecon.CreateBuilder().SetState(1).Build();
        //    ///返回房间用户信息
        //    var userinfobyroom = ReturnUserInfo.CreateBuilder();
        //    var mjhy = MaJiang.CreateBuilder();
        //    var hyyh = ReturnHyUser.CreateBuilder();
        //    if (!string.IsNullOrEmpty(rm.DQHY.ToString()))
        //    {
        //        hyyh.SetFw(rm.DQHY);
        //    }
        //    else
        //    {
        //        hyyh.SetFw(0);
        //    }


        //    if (!string.IsNullOrEmpty(rm.DQcz))
        //    {
        //        hyyh.SetCz(rm.DQcz);
        //    }
        //    else
        //    {
        //        hyyh.SetCz("");
        //    }

        //    if (rm.PaiHSCZ != null)
        //    {
        //        mjhy.SetPaiID(rm.PaiHSCZ.PaiId);
        //        mjhy.SetPaiHS(rm.PaiHSCZ.PaiId);
        //    }
        //    else
        //    {
        //        mjhy.SetPaiID(0);
        //        mjhy.SetPaiHS(0);
        //    }
        //    hyyh.SetMj(mjhy);

        //    var returndata = ReturnConnData.CreateBuilder();
        //    List<byte> returnzhuang = new List<byte>();
        //    foreach (mjuser mjuseritem in mjlist)
        //    {

        //        var MJ = MaJiang.CreateBuilder();
        //        ///手牌集合
        //        var shoupai = UserShou.CreateBuilder();
        //        //碰牌集合
        //        var pengpai = UserPeng.CreateBuilder();
        //        ///出牌集合
        //        var chupai = UserChu.CreateBuilder();
        //        ///杠
        //        var gangpai = UserGang.CreateBuilder();


        //        var userzrj = JpConn.CreateBuilder();
        //        userzrj.SetFW(mjuseritem.ZS_Fw);
        //        if (mjuseritem.Is_cfj == true)
        //        {
        //            userzrj.SetCFJ(1);
        //        }

        //        if (mjuseritem.Is_zrj == -1)
        //        {
        //            userzrj.SetCFJ(2);
        //        }


        //        if (mjuseritem.is_cfwg == true)
        //        {
        //            userzrj.SetWGJ(1);
        //        }

        //        if (mjuseritem.is_zrwg == -1)
        //        {
        //            userzrj.SetWGJ(2);
        //        }

        //        /**********************返回房间用户信息************************************************/
        //        UserInfo user = Gongyong.userlist.Find(u => u.openid == mjuseritem.Openid);
        //        userinfo.SetOpenid(user.openid);
        //        userinfo.SetNickname(user.nickname);
        //        userinfo.SetHeadimg(user.headimg);
        //        userinfo.SetSex(int.Parse(user.sex));
        //        userinfo.SetUserBean(0);
        //        userinfo.SetUserGold(0);
        //        userinfo.SetUserFW(mjuseritem.ZS_Fw);
        //        userinfo.SetUserID(user.UserID);
        //        CardUser card = Gongyong.FKUser.Find(u => u.roomid == mjuseritem.RoomID);
        //        if (card != null)
        //        {
        //            userinfo.SetCumulativeScore(card.GetScoreByPosition(mjuseritem.ZS_Fw));
        //            if (returnzhuang.Count == 0)
        //                returnzhuang.AddRange(ReturnZhuang.CreateBuilder().SetZhuang(card.win).Build().ToByteArray());
        //        }
        //        userinfobyroom.AddUserinfo(userinfo);
        //        /***********************返回手牌集合***************************************/
        //        int paicount = 0;
        //        if (mjuseritem.Openid == openid)
        //        {
        //            foreach (var item in mjuseritem.majiangs)
        //            {
        //                paicount++;
        //                MJ.SetPaiHS(item.PaiHs);
        //                MJ.SetPaiID(item.PaiId);
        //                shoupai.AddMj(MJ);
        //            }
        //        }
        //        else
        //        {
        //            foreach (var item in mjuseritem.majiangs)
        //            {
        //                paicount++;
        //                MJ.SetPaiHS(0);
        //                MJ.SetPaiID(0);
        //                shoupai.AddMj(MJ);
        //            }
        //        }
        //        shoupai.SetFW(mjuseritem.ZS_Fw);
        //        shoupai.SetPcount(paicount);
        //        shoupai.SetQYM(mjuseritem.QYM);
        //        shoupai.SetTianting(mjuseritem.Is_tiantX);
        //        shoupai.SetBaoting(mjuseritem.Is_baotin ? 1 : 0);
        //        /*****碰牌集合*****************************/
        //        foreach (var itemP in mjuseritem.Peng)
        //        {
        //            MJ.SetPaiHS(itemP.PaiHs);
        //            MJ.SetPaiID(itemP.PaiId);
        //            pengpai.AddMj(MJ);
        //        }
        //        pengpai.SetFW(mjuseritem.ZS_Fw);

        //        /****************出牌集合*********************************/
        //        foreach (var itemC in mjuseritem.chuda)
        //        {
        //            MJ.SetPaiHS(itemC.PaiHs);
        //            MJ.SetPaiID(itemC.PaiId);
        //            chupai.AddMj(MJ);
        //        }
        //        chupai.SetFW(mjuseritem.ZS_Fw);

        //        gangpai.SetFW(mjuseritem.ZS_Fw);
        //        if (!string.IsNullOrEmpty(mjuseritem.Gong))
        //        {
        //            gangpai.SetGang(mjuseritem.Gong);

        //        }
        //        else
        //        {
        //            gangpai.SetGang("");
        //        }
        //        returndata.AddChu(chupai);
        //        returndata.AddPeng(pengpai);
        //        returndata.AddGang(gangpai);
        //        returndata.AddShoupai(shoupai);
        //        returndata.AddJp(userzrj);
        //        //chupai.Clear();
        //        //pengpai.Clear();
        //        //gangpai.Clear();
        //        //shoupai.Clear();
        //    }
        //    roommsg.SetCount(rm.count).SetIsBenji(rm.is_benji == true ? 1 : 0).SetIsShangxiaji(rm.is_shangxiaji == true ? 1 : 0).SetIsWgj(rm.is_wgj == true ? 1 : 0).SetIsXinqiji(rm.is_xinqiji == true ? 1 : 0).SetIsYikousan(rm.is_yikousan == true ? 1 : 0).SetRoomPeo(rm.room_peo).SetIsLianzhuang(rm.is_lianz == true ? 1 : 0);
        //    returndata.SetRoomID(mj.RoomID);
        //    byte[] roommsgb = roommsg.Build().ToByteArray();

        //    byte[] hyczall = hyyh.Build().ToByteArray();
        //    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7004, hyczall.Length, requestInfo.MessageNum, hyczall)));

        //    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7003, roommsgb.Length, requestInfo.MessageNum, roommsgb)));
        //    byte[] rData = returndata.Build().ToByteArray();
        //    byte[] uData = userinfobyroom.Build().ToByteArray();
        //    byte[] cState = ifconn.ToByteArray();
        //   // session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 6001, cState.Length, requestInfo.MessageNum, cState)));
        //    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7001, rData.Length, requestInfo.MessageNum, rData)));
        //    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2005, uData.Length, requestInfo.MessageNum, uData)));

        //    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5020, returnzhuang.Count, GameInformationBase.BASEAGREEMENTNUMBER + 7093, returnzhuang.ToArray())));
        //    if (mj.SendData != null)
        //        session.TrySend(mj.SendData);



        //}

    }
}
