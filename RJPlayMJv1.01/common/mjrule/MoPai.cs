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
using DAL.Model;
using MJBLL.model;
using Newtonsoft.Json;
using Google.ProtocolBuffers;

namespace MJBLL.mjrule
{
    public class MoPai : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "13002"; }

        }
        /// <summary>
        /// 摸牌操作
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {

            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("MoPai : 非法连接");
                session.Close();
                return;
            }

            var getinfo = SendMP.ParseFrom(requestInfo.Body);
            session.Logger.Debug("摸牌"+getinfo);
            var setmsg = ReturnMP.CreateBuilder();
            var mjr = MaJiang.CreateBuilder();
            Room r = Gongyong.roomlist.Find(u => u.RoomID == getinfo.Roomid);
            mjuser mjuser = Gongyong.mulist.Find(u => u.RoomID == getinfo.Roomid && u.Openid == getinfo.Openid);

            if (mjuser == null)
            {
                session.Logger.Debug("无此用户");
                return;
            }
            var GRuesult = ReturnGang.CreateBuilder();
            mjuser.SendData.Clear();
            var Gdata = mjuser.majiangs.GroupBy(a => a.PaiHs).Select(g => new { paihs = g.Key, cou = g.Count() });




            if (mjuser.ByteData == requestInfo.Key)
            {
                session.Logger.Debug("摸牌消息重复" + mjuser.Openid);
                return;
            }
            else
            {
                mjuser.ByteData = requestInfo.Key;
            }
            if (r.DQHY != mjuser.ZS_Fw)
            {
                session.Logger.Debug("不是活跃用户"+ mjuser.Openid);
                return;
            }
            if (mjuser.Is_tiant || mjuser.Is_baotin)
            { }
            else
            {
                mjuser.IsGuoHu = false;
            }
            List<mjuser> listAll = Gongyong.mulist.FindAll(u => u.RoomID == r.RoomID);
            ThreadUtility.RemoveManagedThread(mjuser.Openid);//当用户操作时 取消用户的倒计时
            session.Logger.Debug("判断黄牌");
            r.DQcz = "";
            ///黄牌判断
            if (r.RoomPai.Count == 0)
            {
                List<mjuser> listxj = Gongyong.mulist.FindAll(u => u.RoomID == r.RoomID && u.Is_jiao == true);
                CardUser card = Gongyong.FKUser.Find(u => u.roomid == r.RoomID);
                var returnallmj = ReturnUserSPai.CreateBuilder();
                var jsall = ReturnJS.CreateBuilder();
                ///返回各鸡牌对应分数
                JIpai jipai = new Ting().GetJIpai(r.is_yikousan, r.is_lianz);
                card.lianzCount += 1;//黄牌应该庄家加一
                if (listxj.Count == r.room_peo)
                {

                    foreach (var item in listxj)
                    {
                        UserInfo userjsmsg = Gongyong.userlist.Find(u => u.openid == item.Openid);
                        var userjs = UserJS.CreateBuilder();
                        userjs.AddDc(Dou.CreateBuilder());
                        userjs.AddJp(JiPaiF.CreateBuilder());
                        userjs.SetFS(0);
                        userjs.SetIsJiao(1);
                        userjs.SetUserinfo(Userinfo.CreateBuilder().SetHeadimg(userjsmsg.headimg).SetNickname(userjsmsg.nickname).SetOpenid(userjsmsg.openid).SetSex(userjsmsg.sex == "男" ? 1 : 2).SetUserFW(item.ZS_Fw).SetUserGold(0).SetUserBean(0).SetUserID(userjsmsg.UserID));
                        jsall.AddJs(userjs);
                    }
                }
                else
                {
                    if (listxj.Count == 0)
                    {
                        foreach (var item in listAll)
                        {
                            UserInfo userjsmsg = Gongyong.userlist.Find(u => u.openid == item.Openid);
                            var userjs = UserJS.CreateBuilder();
                            userjs.AddDc(Dou.CreateBuilder());
                            userjs.AddJp(JiPaiF.CreateBuilder());
                            userjs.SetFS(0);
                            userjs.SetIsJiao(0);
                            userjs.SetUserinfo(Userinfo.CreateBuilder().SetHeadimg(userjsmsg.headimg).SetNickname(userjsmsg.nickname).SetOpenid(userjsmsg.openid).SetSex(userjsmsg.sex == "男" ? 1 : 2).SetUserFW(item.ZS_Fw).SetUserGold(0).SetUserBean(0).SetUserID(userjsmsg.UserID));
                            jsall.AddJs(userjs);
                        }
                        // card.lianzCount += 1;
                    }
                    else
                    {
                        List<mjuser> listmjuser = Gongyong.mulist.FindAll(u => u.RoomID == r.RoomID && u.Is_jiao == false);//没叫牌的玩家
                                                                                                                           // int userj = 0;

                        List<int> zrwgjlist = new List<int>() { 0, 0, 0, 0 };//责任乌骨鸡特殊算法
                        List<int> zryaojilist = new List<int>() { 0, 0, 0, 0 };//责任幺鸡特殊算法
                        List<int> userScore = new List<int>() { 0, 0, 0, 0 };//得分
                        int lianzhuang = 0;

                        List<UserSettle> listuserSettle = new List<UserSettle>();
                        foreach (var item in listmjuser)
                        {
                            int userj = 0;
                            UserSettle userT = new UserSettle();
                            userT.openid = item.Openid;
                            //为叫牌赋值唯0
                            userT.is_jiao = 0;
                            if (r.is_wgj)
                            {
                                #region 判断包的乌骨鸡
                                //存在冲锋鸡，判断打出去的8筒数
                                if (item.is_cfwg == true)
                                {
                                    ///鸡牌复制
                                    UserJPOne cfwg = new UserJPOne();
                                    cfwg.PaiHS = 8;
                                    cfwg.PaiScare = 0 - jipai.cfwuguji;
                                    cfwg.type = 4;
                                    userT.jp.Add(cfwg);
                                    userj += cfwg.PaiScare;
                                    ///查找出的8筒
                                    int wuCount = item.chuda.FindAll(u => u.PaiHs == 8).Count - 1;
                                    ///查找碰的8筒
                                    int wuP = item.Peng.FindAll(u => u.PaiHs == 8).Count;
                                    int ACount = wuP + wuCount;
                                    if (ACount > 0)
                                    {
                                        UserJPOne wgp = new UserJPOne();
                                        wgp.PaiHS = 8;
                                        wgp.PaiScare = 0 - (ACount * jipai.wuguji);
                                        userj += wgp.PaiScare;
                                        wgp.type = 3;
                                        userT.jp.Add(wgp);

                                    }
                                }
                                ///不存在冲锋鸡，判断打出的8筒
                                else
                                {
                                    ////存在责任鸡
                                    if (item.is_zrwg != 0)
                                    {
                                        if (item.is_zrwg > 0 && listxj.Any(w => w.is_zrwg == 1))
                                        {
                                            UserJPOne zrcfwg = new UserJPOne();
                                            zrcfwg.PaiHS = 8;
                                            zrcfwg.PaiScare = 0 - jipai.zr_wugu;
                                            zrcfwg.type = 8;//责任鸡
                                            userT.jp.Add(zrcfwg);
                                            zrwgjlist[item.ZS_Fw - 1] += zrcfwg.PaiScare;//如果有责任鸡但是没下叫需要反过来给对方负责
                                        }
                                        else if (listxj.Any(w => w.is_zrwg == 1))
                                        {
                                            UserJPOne zrcfwg = new UserJPOne();
                                            zrcfwg.PaiHS = 8;
                                            zrcfwg.PaiScare = 0 - jipai.zr_wugu;
                                            zrcfwg.type = 8;//责任鸡
                                            userT.jp.Add(zrcfwg);

                                            //如果需要负责的那一家也没有下叫则不需要向对方负责
                                            zrwgjlist[item.ZS_Fw - 1] += zrcfwg.PaiScare;

                                        }
                                    }
                                    ///不存在责任鸡
                                    //else
                                    //{
                                    int CCount = item.chuda.FindAll(u => u.PaiHs == 8).Count;
                                    int PCount = item.Peng.FindAll(u => u.PaiHs == 8).Count;

                                    int ACount = CCount + PCount;
                                    if (!string.IsNullOrEmpty(item.Gong))
                                    {
                                        string gang = string.Copy(item.Gong);
                                        //替换相似字符，防止找错
                                        gang = gang.Replace("18", "八条");
                                        gang = gang.Replace("28", "八万");
                                        gang = gang.Replace("8", "八筒");
                                        if (gang.Contains("八筒"))
                                        {
                                            ACount = 4 + ACount;
                                            UserJPOne wg = new UserJPOne();
                                            wg.PaiHS = 8;
                                            wg.PaiScare = 0 - (ACount * jipai.wuguji);
                                            wg.type = 3;
                                            userT.jp.Add(wg);
                                            userj += wg.PaiScare;
                                        }
                                    }
                                    else
                                    {
                                        if (ACount > 0)
                                        {
                                            UserJPOne wg = new UserJPOne();
                                            wg.PaiHS = 8;
                                            wg.PaiScare = 0 - (ACount * jipai.wuguji);
                                            wg.type = 3;
                                            userT.jp.Add(wg);
                                            userj += wg.PaiScare;
                                        }
                                    }
                                }
                                #endregion
                            }
                            #region 未叫牌，判断幺鸡

                            ///判断是否冲锋鸡
                            if (item.Is_cfj == true)
                            {
                                UserJPOne cfg = new UserJPOne();
                                cfg.PaiHS = 11;
                                cfg.PaiScare = 0 - jipai.cfj;
                                cfg.type = 2;
                                userT.jp.Add(cfg);
                                userj += cfg.PaiScare;
                                ///查找打出的幺鸡
                                int CYJ = item.chuda.FindAll(u => u.PaiHs == 11).Count - 1;

                                ///查找碰的幺鸡
                                int PYJ = item.Peng.FindAll(u => u.PaiHs == 11).Count;
                                int Acount = CYJ + PYJ;
                                if (Acount > 0)
                                {
                                    UserJPOne Pj = new UserJPOne();
                                    Pj.PaiHS = 11;
                                    Pj.PaiScare = 0 - (jipai.yaoji * Acount);
                                    Pj.type = 1;
                                    userT.jp.Add(Pj);
                                    userj += Pj.PaiScare;
                                }
                            }
                            ///不存在冲锋鸡，判断打出的幺鸡
                            else
                            {
                                ///判断责任鸡

                                if (item.Is_zrj != 0)
                                {
                                    if (item.Is_zrj > 0 && listxj.Any(w => w.Is_zrj == 1))
                                    {
                                        UserJPOne zrcfg = new UserJPOne();
                                        zrcfg.PaiHS = 11;
                                        zrcfg.PaiScare = 0 - jipai.zr_yao;
                                        zrcfg.type = 9;
                                        userT.jp.Add(zrcfg);
                                        //userj += zrcfg.PaiScare;
                                        zryaojilist[item.ZS_Fw - 1] += zrcfg.PaiScare;
                                    }
                                    else if (listxj.Any(w => w.Is_zrj == 1))//如果需要负责的那一家也没有下叫则不需要向对方负责
                                    {
                                        UserJPOne zrcfg = new UserJPOne();
                                        zrcfg.PaiHS = 11;
                                        zrcfg.PaiScare = 0 - jipai.zr_yao;
                                        zrcfg.type = 9;
                                        userT.jp.Add(zrcfg);
                                        //userj += zrcfg.PaiScare;

                                        zryaojilist[item.ZS_Fw - 1] += zrcfg.PaiScare;
                                    }
                                }

                                ///查找打出的幺鸡
                                int cyj = item.chuda.FindAll(u => u.PaiHs == 11).Count;

                                ///查找碰的幺鸡
                                int pyj = item.Peng.FindAll(u => u.PaiHs == 11).Count;
                                int Acount = cyj + pyj;

                                if (!string.IsNullOrEmpty(item.Gong))
                                {

                                    if (item.Gong.Contains("11"))
                                    {
                                        Acount = Acount + 4;
                                        UserJPOne pj = new UserJPOne();
                                        pj.PaiHS = 11;
                                        pj.PaiScare = 0 - (Acount * jipai.yaoji);
                                        pj.type = 1;
                                        userT.jp.Add(pj);
                                        userj += pj.PaiScare;
                                    }
                                    else
                                    {
                                        if (Acount > 0)
                                        {
                                            UserJPOne pj = new UserJPOne();
                                            pj.PaiHS = 11;
                                            pj.PaiScare = 0 - (Acount * jipai.yaoji);
                                            pj.type = 1;
                                            userT.jp.Add(pj);
                                            userj += pj.PaiScare;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Acount > 0)
                                    {
                                        UserJPOne pj = new UserJPOne();
                                        pj.PaiHS = 11;
                                        pj.PaiScare = 0 - (Acount * jipai.yaoji);
                                        pj.type = 1;
                                        userT.jp.Add(pj);
                                        userj += pj.PaiScare;
                                    }
                                }
                                //}
                            }

                            #endregion

                            if (r.is_lianz)
                            {
                                if (item.ZS_Fw == card.win)
                                {
                                    UserJPOne zrcfg = new UserJPOne();
                                    zrcfg.PaiHS = 99;
                                    zrcfg.PaiScare = 0 - card.lianzCount;
                                    zrcfg.type = 10;
                                    userT.jp.Add(zrcfg);
                                    lianzhuang = zrcfg.PaiScare;
                                    userj += zrcfg.PaiScare;//玩家结算得分
                                }
                            }


                            UserInfo userjsmsg = Gongyong.userlist.Find(u => u.openid == item.Openid);

                            //userT.jp.Select(w => JiPaiF.CreateBuilder().SetPaiHS(w.PaiHS).SetType(w.type).SetCount(w.PaiScare).Build());
                            //foreach (var itrmj in userT.jp)
                            //{
                            //    var jpone = JiPaiF.CreateBuilder().SetPaiHS(itrmj.PaiHS).SetType(itrmj.type).SetCount(itrmj.PaiScare);
                            //    userjs.AddJp(jpone);
                            //}


                            userT.is_jiao = 0;
                            // userjs.SetUserinfo(Userinfo.CreateBuilder().SetHeadimg(userjsmsg.headimg).SetNickname(userjsmsg.nickname).SetOpenid(userjsmsg.openid).SetSex(int.Parse(userjsmsg.sex)).SetUserFW(item.ZS_Fw).SetUserGold(0).SetUserBean(0).SetUserID(userjsmsg.UserID));


                            #region 旧代码
                            /* foreach (var s in listall)
                             {
                                 string hxx = new Ting().GetTing(s.majiangs);
                                 new Ting().GetPaiXin(hxx,s);
                                 userj -= new Ting().GetPaixinFen(jipai, s.paixinfs);

                                 //JIpai jp = new Ting().GetJIpai(r.is_yikousan, r.is_lianz);
                                 //switch (s.paixinfs)
                                 //{
                                 //    case 1:

                                 //        scare += jp.xqidui;
                                 //        break;
                                 //    case 2:
                                 //        scare += jp.qys;
                                 //        break;
                                 //    case 3:
                                 //        scare += jp.dduizi;
                                 //        break;
                                 //    case 4:
                                 //        scare += jp.longqidui;
                                 //        break;
                                 //    case 5:
                                 //        scare += jp.zimo;
                                 //        break;
                                 //    case 6:
                                 //        scare += jp.qys;
                                 //        scare += jp.xqidui;
                                 //        break;
                                 //    case 7:
                                 //        scare += jp.qys * 2;

                                 //        break;
                                 //    case 8:
                                 //        scare += jp.qys;
                                 //        scare += jp.dduizi;
                                 //        break;
                                 //    case 9:
                                 //        scare += jp.qys;
                                 //        scare += jp.longqidui;
                                 //        break;
                                 //    case 10:
                                 //        scare += jp.qys;
                                 //        break;
                                 //    default:
                                 //        break;
                                 //} 


                             }*/
                            #endregion
                          //  userj += zrwgjlist[item.ZS_Fw - 1];
                          //  userj += zryaojilist[item.ZS_Fw - 1];
                            userScore[item.ZS_Fw - 1] = userj;
                            listuserSettle.Add(userT);
                            // userjs.SetFS(userj);
                            //jsall.AddJs(userjs);

                        }

                        foreach (var item in listxj)
                        {
                            int scare = 0;
                            UserSettle userT = new UserSettle();
                            userT.openid = item.Openid;
                            UserInfo userjsmsg = Gongyong.userlist.Find(u => u.openid == item.Openid);

                            List<model.ServerMaJiang> listmj = new List<model.ServerMaJiang>();
                            listmj.AddRange(item.majiangs);
                            string hxx = new Ting().GetTing(listmj);
                            new Ting().GetPaiXin(hxx, item);
                            scare = new Ting().GetPaixinFen(jipai, item.paixinfs, r.is_yikousan, r.is_lianz);

                            if (item.Is_zrj != 0)
                            {
                                if (item.Is_zrj > 0 && listmjuser.Any(w => w.Is_zrj == -1))
                                {
                                    UserJPOne zrcfg = new UserJPOne();
                                    zrcfg.PaiHS = 11;
                                    zrcfg.PaiScare =jipai.zr_yao;
                                    zrcfg.type = 9;
                                    userT.jp.Add(zrcfg);
                                    //userj += zrcfg.PaiScare;
                                    zryaojilist[item.ZS_Fw - 1] += zrcfg.PaiScare;
                                }
                                else if (listmjuser.Any(w => w.Is_zrj == 1))//如果需要负责的那一家也没有下叫则对方需要向自己负责
                                {
                                    UserJPOne zrcfg = new UserJPOne();
                                    zrcfg.PaiHS = 11;
                                    zrcfg.PaiScare = jipai.zr_yao;
                                    zrcfg.type = 9;
                                    userT.jp.Add(zrcfg);
                                    //userj += zrcfg.PaiScare;

                                    zryaojilist[item.ZS_Fw - 1] += zrcfg.PaiScare;

                                }
                            }
                            if (item.is_zrwg != 0)
                            {
                                if (item.is_zrwg > 0 && listmjuser.Any(w => w.is_zrwg == -1))
                                {
                                    UserJPOne zr = new UserJPOne();
                                    zr.PaiHS = 8;
                                    zr.PaiScare = jipai.zr_wugu;
                                    zr.type = 8;
                                    userT.jp.Add(zr);
                                    zrwgjlist[item.ZS_Fw - 1] += zr.PaiScare;
                                    //userj += zr.PaiScare;
                                }
                                else if (listmjuser.Any(w => w.is_zrwg == 1))//如果需要负责的那一家也没有下叫则对方需要向自己负责
                                {
                                    UserJPOne zr = new UserJPOne();
                                    zr.PaiHS = 8;

                                    zr.PaiScare = jipai.zr_wugu;
                                    zr.type = 8;
                                    userT.jp.Add(zr);
                                    //userj += zr.PaiScare;
                                    zrwgjlist[item.ZS_Fw - 1] += zr.PaiScare;
                                }
                            }
                            userT.jp.Add(new UserJPOne { PaiHS = item.paixinfs, PaiScare = scare, type = 11 });
                            //scare += zrwgjlist[item.ZS_Fw - 1]; //如果加上责任鸡 得分 会在总得分处 多扣一次
                          //  scare += zryaojilist[item.ZS_Fw - 1];
                            userScore[item.ZS_Fw - 1] = scare;
                            listuserSettle.Add(userT);
                        }


                        foreach (var item in listAll)
                        {
                            UserInfo userjsmsg = Gongyong.userlist.Find(u => u.openid == item.Openid);
                            var userjs = UserJS.CreateBuilder();
                            userjs.AddDc(Dou.CreateBuilder());
                            userjs.AddRangeJp(listuserSettle.Find(w => w.openid.Equals(item.Openid)).jp.Select(w => JiPaiF.CreateBuilder().SetPaiHS(w.PaiHS).SetType(w.type).SetCount(w.PaiScare).Build()));

                            userjs.SetIsJiao(item.Is_jiao ? 1 : 0);
                            userjs.SetUserinfo(Userinfo.CreateBuilder().SetHeadimg(userjsmsg.headimg).SetNickname(userjsmsg.nickname).SetOpenid(userjsmsg.openid).SetSex(userjsmsg.sex == "男" ? 1 : 2).SetUserFW(item.ZS_Fw).SetUserGold(0).SetUserBean(0).SetUserID(userjsmsg.UserID));
                            if (item.Is_jiao)
                            {
                                userjs.SetFS(((userScore[item.ZS_Fw - 1] * listmjuser.Count) - userScore.Sum(w => w > 0 ? 0 : w)) + zrwgjlist[item.ZS_Fw - 1]+ zryaojilist[item.ZS_Fw - 1]);//因为是负数 所以是减号
                            }
                            else
                            {
                                userjs.SetFS((userScore[item.ZS_Fw - 1] * listxj.Count - userScore.Sum(w => w < 0 ? 0 : w))  +zrwgjlist[item.ZS_Fw - 1] + zryaojilist[item.ZS_Fw - 1]);
                            }
                            switch (item.ZS_Fw)
                            {
                                case 1:
                                    card.dong += userjs.FS;
                                    break;
                                case 2:
                                    card.nan += userjs.FS;
                                    break;
                                case 3:
                                    card.xi += userjs.FS;
                                    break;
                                case 4:
                                    card.bei += userjs.FS;
                                    break;
                                default:
                                    break;
                            }

                            jsall.AddJs(userjs);
                        }

                    }
                }
                var jsddr = UserAJS.CreateBuilder();
                var senddjs = ReturnDJS.CreateBuilder().SetState(1);
                foreach (var item in listAll)
                {


                    var usermjs = UserSPai.CreateBuilder();
                    usermjs.SetFw(item.ZS_Fw);
                    var returnmj = MaJiang.CreateBuilder();
                    foreach (var itemmj in item.majiangs)
                    {
                        returnmj.SetPaiHS(itemmj.PaiHs);
                        returnmj.SetPaiID(itemmj.PaiId);
                        usermjs.AddMj(returnmj);
                    }
                    returnallmj.AddUsp(usermjs);

                    if (r.Dcount == r.count)
                    {
                        new CardsLogic().Clear(item, r);
                        UserInfo gamer = Gongyong.userlist.Find(u => u.openid == item.Openid);
                        var users = Userinfo.CreateBuilder().SetHeadimg(gamer.headimg).SetNickname(gamer.nickname).SetOpenid(gamer.openid).SetSex(int.Parse(gamer.sex)).SetUserFW(item.ZS_Fw).SetUserBean(0).SetUserGold(0).SetUserID(gamer.UserID).Build();

                        jsddr.SetUser(users);
                        jsddr.SetZdou(item.zwd_count);
                        jsddr.SetZimo(item.zm_count);
                        jsddr.SetAndou(item.ad_count);
                        jsddr.SetMdou(item.MD_count);
                        jsddr.SetDianpao(item.dp_count);
                        switch (item.ZS_Fw)
                        {
                            case 1:
                                jsddr.SetScare(card.dong);
                                break;
                            case 2:
                                jsddr.SetScare(card.nan);
                                break;
                            case 3:
                                jsddr.SetScare(card.xi);
                                break;
                            case 4:
                                jsddr.SetScare(card.bei);
                                break;
                            default:
                                break;
                        }
                        senddjs.AddUserjs(jsddr);
                    }



                }

                foreach (var item in listAll)
                {
                    UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);

                    byte[] stringnew = ReturnPaiCount.CreateBuilder().SetPaiCount(r.RoomPai.Count).Build().ToByteArray();
                    var sendData7006 = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7006, stringnew.Length, requestInfo.MessageNum, stringnew));
                    item.SendData.Add(sendData7006);
                    // user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7006, stringnew.Length, requestInfo.MessageNum, stringnew)));


                    byte[] allsp = returnallmj.Build().ToByteArray();

                    var sendData7008 = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7008, allsp.Length, requestInfo.MessageNum, allsp));
                    item.SendData.Add(sendData7008);
                    //user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7008, allsp.Length, requestInfo.MessageNum, allsp)));

                    byte[] settlebyte = jsall.Build().ToByteArray();
                    r.gameOperationProcess.SetJieSuanInfo(ByteString.CopyFrom(settlebyte));
                    var sendData7009 = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7009, settlebyte.Length, requestInfo.MessageNum, settlebyte));
                    // user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7009, settlebyte.Length, requestInfo.MessageNum, settlebyte)));
                    item.SendData.Add(sendData7009);
                    if (r.Dcount == r.count)
                    {
                        byte[] senddjsbyte = senddjs.Build().ToByteArray();
                        var sendData5014 = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5014, senddjsbyte.Length, requestInfo.MessageNum, senddjsbyte));
                        item.SendData.Add(sendData5014);

                        // user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5014, senddjsbyte.Length, requestInfo.MessageNum, senddjsbyte)));
                    }
                    foreach (var senddata in item.SendData)
                    {
                        user.session.TrySend(senddata);
                    }
                }

                ///对局结束 清空所有信息
                if (r.Dcount == r.count)
                {
                    Gongyong.mulist.RemoveAll(u => u.RoomID == r.RoomID);
                    Gongyong.FKUser.RemoveAll(u => u.roomid == r.RoomID);
                    Gongyong.roomlist.RemoveAll(u => u.RoomID == r.RoomID);
                }
                else
                {

                    if (r != null)
                    {
                        r.RoomPai = new CreateMj().CreateMJ(r.IsYuanQue);
                        r.Dcount += 1;

                    }

                }
                ThreadUtility.RemoveManagedThreadByRoomID(r.RoomID);
                r.startgame = 2;
                CardsLogic.AddboarddetailsTB(r);
                return;
            }



            session.Logger.Debug("判断杠");
            ///判断杠
            foreach (var item in Gdata)
            {
                if (item.cou == 4)
                {
                    if (mjuser.chuda.Count == 0)
                    {
                        var GD = GangMSG.CreateBuilder();
                        switch (mjuser.QYM)
                        {
                            case 1:
                                if (item.paihs > 10)
                                {
                                    r.DQcz = "30082,3";
                                    GD.SetFw(mjuser.ZS_Fw);
                                    GD.SetType(3);
                                    GD.SetMj(MaJiang.CreateBuilder().SetPaiHS(item.paihs).SetPaiID(0));
                                    GRuesult.AddGang(GD);
                                }
                                break;
                            case 2:
                                if (item.paihs < 10 || item.paihs > 20)
                                {
                                    r.DQcz = "30082,3";
                                    GD.SetFw(mjuser.ZS_Fw);
                                    GD.SetType(3);
                                    GD.SetMj(MaJiang.CreateBuilder().SetPaiHS(item.paihs).SetPaiID(0));
                                    GRuesult.AddGang(GD);
                                }
                                break;
                            case 3:
                                if (item.paihs < 20)
                                {
                                    r.DQcz = "30082,3";

                                    GD.SetFw(mjuser.ZS_Fw);
                                    GD.SetType(3);
                                    GD.SetMj(MaJiang.CreateBuilder().SetPaiHS(item.paihs).SetPaiID(0));
                                    GRuesult.AddGang(GD);
                                }
                                break;
                            default:
                                if (r.room_peo == 4)
                                {
                                    r.DQcz = "30082,3";
                                    GD.SetFw(mjuser.ZS_Fw);
                                    GD.SetType(3);
                                    GD.SetMj(MaJiang.CreateBuilder().SetPaiHS(item.paihs).SetPaiID(0));
                                    GRuesult.AddGang(GD);
                                }
                                break;
                        }
                    }
                    else
                    {
                        var GDss = GangMSG.CreateBuilder();
                        switch (mjuser.QYM)
                        {
                            case 1:
                                if (item.paihs > 10)
                                {
                                    r.DQcz = "30082,4";
                                    GDss.SetFw(mjuser.ZS_Fw);
                                    GDss.SetType(4);
                                    GDss.SetMj(MaJiang.CreateBuilder().SetPaiHS(item.paihs).SetPaiID(0));
                                    GRuesult.AddGang(GDss);
                                }
                                break;
                            case 2:
                                if (item.paihs < 10 || item.paihs > 20)
                                {
                                    r.DQcz = "30082,4";

                                    GDss.SetFw(mjuser.ZS_Fw);
                                    GDss.SetType(4);
                                    GDss.SetMj(MaJiang.CreateBuilder().SetPaiHS(item.paihs).SetPaiID(0));
                                    GRuesult.AddGang(GDss);
                                }
                                break;
                            case 3:
                                if (item.paihs < 20)
                                {
                                    r.DQcz = "30082,4";
                                    GDss.SetFw(mjuser.ZS_Fw);
                                    GDss.SetType(4);
                                    GDss.SetMj(MaJiang.CreateBuilder().SetPaiHS(item.paihs).SetPaiID(0));
                                    GRuesult.AddGang(GDss);
                                }
                                break;
                            default:
                                r.DQcz = "30082,4";
                                GDss.SetFw(mjuser.ZS_Fw);
                                GDss.SetType(4);
                                GDss.SetMj(MaJiang.CreateBuilder().SetPaiHS(item.paihs).SetPaiID(0));
                                GRuesult.AddGang(GDss);
                                break;
                        }
                    }
                }
            }

           // session.Logger.Debug("判断碰");
            foreach (var item in mjuser.majiangs)
            {
                int count = mjuser.Peng.FindAll(u => u.PaiHs == item.PaiHs).Count;
                if (count > 0)
                {
                    r.DQcz = "30082,4";
                    var GDss = GangMSG.CreateBuilder();
                    GDss.SetFw(mjuser.ZS_Fw);
                    GDss.SetType(4);
                    GDss.SetMj(MaJiang.CreateBuilder().SetPaiHS(item.PaiHs).SetPaiID(item.PaiId));
                    GRuesult.AddGang(GDss);
                }
            }


            switch (r.room_peo)
            {
                case 2:
                    if (r.RoomPai.Count == 108)
                    {
                        r.RoomPai.RemoveRange(0, 27);
                    }
                    break;
                case 3:
                    if (r.RoomPai.Count == 108)
                    {
                        r.RoomPai.RemoveRange(0, 40);
                    }
                    break;
                case 4:
                    if (r.RoomPai.Count == 108)
                    {
                        r.RoomPai.RemoveRange(0, 53);
                    }
                    break;
                default:
                    break;
            }





            model.ServerMaJiang mj = r.RoomPai[0];
            r.RoomPai.Remove(r.RoomPai[0]);
            mjr.SetPaiHS(mj.PaiHs);
            mjr.SetPaiID(mj.PaiId);
            setmsg.SetMj(mjr);
            r.LastMoMJ = mj;
            var shy = ReturnHyUser.CreateBuilder();
            // string cz = "";


            session.Logger.Debug("判断胡牌前");

            List<model.ServerMaJiang> mjlist = mjuser.majiangs.FindAll(u => u.PaiHs == mj.PaiHs);
            List<model.ServerMaJiang> mjls = new List<model.ServerMaJiang>();
            mjls = mjuser.Peng.FindAll(u => u.PaiHs == mj.PaiHs);
            if (mjlist.Count == 3 || mjls.Count == 3)
            {
                //setmsg.SetGang(1);
                var PGH = ReturnAll.CreateBuilder();
                var mjG = MaJiang.CreateBuilder();

                mjG.SetPaiID(mj.PaiId);
                mjG.SetPaiHS(mj.PaiHs);

                var GDss = GangMSG.CreateBuilder();
                GDss.SetFw(mjuser.ZS_Fw);
                if (mjlist.Count == 3)
                {
                    GDss.SetType(3);
                }
                if (mjls.Count == 3)
                {
                    GDss.SetType(2);
                }
                GDss.SetMj(mjG);
                GRuesult.AddGang(GDss);

                //PGH.SetFw(mjuser.ZS_Fw);
                //PGH.SetGang(1);
                //PGH.SetMj(mjr);
                //byte[] bytegang = PGH.Build().ToByteArray();
                r.DQHY = mjuser.ZS_Fw;
                r.DQcz += "|30082" + "," + GDss.Type;

                r.PaiHSCZ = mj;
                //session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3008, bytegang.Length, requestInfo.MessageNum, bytegang)));
                session.Logger.Debug("方位------" + mjuser.ZS_Fw + "转弯刚" + mjr.PaiHS);
            }
            mjuser.majiangs.Add(mj);
            Ting t = new Ting();
            List<model.ServerMaJiang> listT = new List<model.ServerMaJiang>();
            listT.AddRange(mjuser.majiangs.ToArray());
            listT.Sort((a, b) => -b.PaiHs.CompareTo(a.PaiHs));

            r.DQHY = mjuser.ZS_Fw;
            r.DQcz = "3001";

            // int QYMcount = 0;
            bool is_hu = true;
            if (r.room_peo < 4)
            {
                switch (mjuser.QYM)
                {
                    case 1:
                        if (mjuser.majiangs.FindAll(u => u.PaiHs < 10).Count > 0)
                        {
                            is_hu = false;
                        }
                        break;
                    case 2:
                        if (mjuser.majiangs.FindAll(u => u.PaiHs > 10 && u.PaiHs < 20).Count > 0)
                        {
                            is_hu = false;
                        }
                        break;
                    case 3:
                        if (mjuser.majiangs.FindAll(u => u.PaiHs > 20).Count > 0)
                        {
                            is_hu = false;
                        }
                        break;
                    default:
                        break;
                }
            }

            #region 判断自摸旧
            ///判断自摸
            //switch (mjuser.QYM)
            //{
            //    case 1:
            //        //QYMcount = mjuser.majiangs.FindAll(u => u.PaiHs < 10).Count;
            //        //if (QYMcount == 0)
            //        //{
            //        //    Hu(t, listT, mjuser, getinfo, session,r, requestInfo.MessageNum,mj);
            //        //}
            //        break;
            //    case 2:

            //        QYMcount = mjuser.majiangs.FindAll(u => u.PaiHs > 10 && u.PaiHs < 20).Count;
            //        if (QYMcount == 0)
            //        {
            //            //if (t.GetTing(listT) == "H")
            //            //{
            //            //    var mjG = MaJiang.CreateBuilder().SetPaiHS(mj.PaiHs).SetPaiID(mj.PaiId);
            //            //    byte[] hdaya;
            //            //    if (getinfo.MType == 1)
            //            //    {
            //            //        hdaya = ReturnHByType.CreateBuilder().SetFWZ(mjuser.ZS_Fw).SetMJ(mjG).SetType(5).Build().ToByteArray();
            //            //    }
            //            //    else
            //            //    {
            //            //        hdaya = ReturnHByType.CreateBuilder().SetFWZ(mjuser.ZS_Fw).SetMJ(mjG).SetType(1).Build().ToByteArray();
            //            //    }

            //            //    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, hdaya.Length, requestInfo.MessageNum, hdaya)));
            //            //    r.DQHY = mjuser.ZS_Fw;
            //            //    r.DQcz += "|30084";
            //            //    r.PaiHSCZ = mj;
            //            //}
            //            Hu(t, listT, mjuser, getinfo, session, r, requestInfo.MessageNum, mj);
            //        }
            //        break;
            //    case 3:
            //        QYMcount = mjuser.majiangs.FindAll(u => u.PaiHs > 20).Count;
            //        if (QYMcount == 0)
            //        {
            //            //if (t.GetTing(listT) == "H")
            //            //{
            //            //    var mjG = MaJiang.CreateBuilder().SetPaiHS(mj.PaiHs).SetPaiID(mj.PaiId);


            //            //    byte[] hdaya;
            //            //    if (getinfo.MType == 1)
            //            //    {
            //            //        hdaya = ReturnHByType.CreateBuilder().SetFWZ(mjuser.ZS_Fw).SetMJ(mjG).SetType(5).Build().ToByteArray();
            //            //    }
            //            //    else
            //            //    {
            //            //        hdaya = ReturnHByType.CreateBuilder().SetFWZ(mjuser.ZS_Fw).SetMJ(mjG).SetType(1).Build().ToByteArray();
            //            //    }
            //            //    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, hdaya.Length, requestInfo.MessageNum, hdaya)));
            //            //    r.DQHY = mjuser.ZS_Fw;
            //            //    r.DQcz += "|30084";
            //            //    r.PaiHSCZ = mj;
            //            //}

            //            Hu(t, listT, mjuser, getinfo, session, r, requestInfo.MessageNum, mj);
            //        }
            //        break;
            //    case 0:
            //        //if (t.GetTing(listT) == "H")
            //        //{
            //        //    var mjG = MaJiang.CreateBuilder().SetPaiHS(mj.PaiHs).SetPaiID(mj.PaiId);


            //        //    byte[] hdaya;
            //        //    if (getinfo.MType == 1)
            //        //    {
            //        //        hdaya = ReturnHByType.CreateBuilder().SetFWZ(mjuser.ZS_Fw).SetMJ(mjG).SetType(5).Build().ToByteArray();
            //        //    }
            //        //    else
            //        //    {
            //        //        hdaya = ReturnHByType.CreateBuilder().SetFWZ(mjuser.ZS_Fw).SetMJ(mjG).SetType(1).Build().ToByteArray();
            //        //    }
            //        //    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, hdaya.Length, requestInfo.MessageNum, hdaya)));
            //        //    r.DQHY = mjuser.ZS_Fw;
            //        //    r.DQcz += "|30084";
            //        //    r.PaiHSCZ = mj;
            //        //}
            //        Hu(t, listT, mjuser, getinfo, session, r, requestInfo.MessageNum, mj);
            //        break;
            //    default:
            //        break;
            //} 
            #endregion


            if (is_hu)
            {
                Hu(t, listT, mjuser, getinfo, session, r, requestInfo.MessageNum, mj);

                if (mjuser.majiangs.Count == 14 && mjuser.MopaiNumber <= 1 && mjuser.Peng.Count == 0 && string.IsNullOrEmpty(mjuser.Gong))
                {
                    List<model.ServerMaJiang> Returnmj = new List<model.ServerMaJiang>();
                    Returnmj.AddRange(mjuser.majiangs.ToArray());
                    List<model.ServerMaJiang> Ruturnjsmj = new List<model.ServerMaJiang>();
                    Ruturnjsmj = t.ReturnJMJ(Returnmj);
                    if (Ruturnjsmj.Count > 0)
                    {
                        var returntp = ReturnTP.CreateBuilder();
                        var tmj = MaJiang.CreateBuilder();
                        foreach (var item in Ruturnjsmj)
                        {
                            tmj.SetPaiHS(item.PaiHs);
                            tmj.SetPaiID(item.PaiId);
                            returntp.AddMj(tmj);
                        }
                        byte[] tmjsr = returntp.Build().ToByteArray();
                        session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2009, tmjsr.Length, requestInfo.MessageNum, tmjsr)));
                    }
                }
            }

            shy.SetFw(mjuser.ZS_Fw);
            shy.SetCz(r.DQcz);
            byte[] sby = shy.Build().ToByteArray();




            byte[] by = setmsg.Build().ToByteArray();
            session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 3003, by.Length, requestInfo.MessageNum, by)));
            //牌局回放 摸牌
            r.gameOperationProcess.AddGameOperationInfo(
            GameOperationInfo.CreateBuilder()
            .SetSerialNumber(r.gameOperationProcess.GameOperationInfoCount)
            .SetOperationFW(mjuser.ZS_Fw)
            .SetOperationType(1)
            .AddMJ(setmsg.Mj)
            );


            if (GRuesult.GangCount != 0)
            {
                byte[] returngmsg = GRuesult.Build().ToByteArray();
                session.Logger.Debug("发送杠牌信息" + GRuesult.ToString() + ";手牌信息：" + JsonConvert.SerializeObject(mjuser.majiangs) + ";碰牌集合" + JsonConvert.SerializeObject(mjuser.Peng));
                var senddata5022 = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5022, returngmsg.Length, requestInfo.MessageNum, returngmsg));
                mjuser.SendData.Add(senddata5022);
                session.TrySend(senddata5022);
            }
        

            session.Logger.Debug("方位------" + mjuser.ZS_Fw + "--摸--" + mj.PaiHs);

            // List<mjuser> lisrts = Gongyong.mulist.FindAll(u => u.RoomID == r.RoomID);
            foreach (var item in listAll)
            {
                var user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                // user.IsActive = true;
                user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7004, sby.Length, requestInfo.MessageNum, sby)));
                byte[] stringnew = ReturnPaiCount.CreateBuilder().SetPaiCount(r.RoomPai.Count).Build().ToByteArray();
                user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7006, stringnew.Length, requestInfo.MessageNum, stringnew)));
            }

            //string paidui = "";
            //foreach (var item in r.RoomPai)
            //{
            //    paidui += item.PaiHs + ",";
            //}


            // session.Logger.Error("摸牌牌堆" + paidui);
            mjuser.MopaiNumber++;
            if (string.IsNullOrEmpty(r.DQcz))
            {

            }
            ThreadUtility.StartManagedThread(Gongyong.mulist.Find(w => w.ZS_Fw == r.DQHY && w.RoomID == r.RoomID).Openid, r.RoomID,r.DQcz);
        }

        private void Hu(Ting t, List<model.ServerMaJiang> listT, mjuser mjuser, SendMP getinfo, GameSession session, Room r, int messageNum, model.ServerMaJiang mj)
        {
            if (t.GetTing(listT) == "H")
            {
                var mjG = MaJiang.CreateBuilder().SetPaiHS(mj.PaiHs).SetPaiID(mj.PaiId);
                r.Is_Hu = true;

                byte[] hdaya;
                if (getinfo.MType == 1)
                {
                    hdaya = ReturnHByType.CreateBuilder().SetFWZ(mjuser.ZS_Fw).SetMJ(mjG).SetType(5).Build().ToByteArray();
                }
                else
                {
                    hdaya = ReturnHByType.CreateBuilder().SetFWZ(mjuser.ZS_Fw).SetMJ(mjG).SetType(1).Build().ToByteArray();
                }
                var data = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5015, hdaya.Length, messageNum, hdaya));
                mjuser.SendData.Add(data);
                session.TrySend(data);
                r.DQHY = mjuser.ZS_Fw;
                r.DQcz += "|30080";
                r.PaiHSCZ = mj;
            }
            else
                r.Is_Hu = false;
        }


    }
}
