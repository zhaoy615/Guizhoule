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
using DAL.DAL;
using DAL.Model;
using Google.ProtocolBuffers;

namespace MJBLL.mjrule
{
    /// <summary>
    /// 胡牌
    /// </summary>
    public class GetH : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name { get { return "15016"; } }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            string userWinLose = string.Empty;//玩家输赢记录。房间信息表


            if (!Gongyong.userlist.Any(w => w.session.SessionID.Equals(session.SessionID)))
            {
                session.Logger.Debug("GetH : 非法连接");
                session.Close();
                return;
            }
            var getdata = SendHu.ParseFrom(requestInfo.Body);

            Room r = Gongyong.roomlist.Find(u => u.RoomID == getdata.Roomid);
            if (r == null)
                return;
            if (!r.Is_Hu)
            {
                session.Logger.Debug("房间并无胡牌");
                return;
            }
            ThreadUtility.RemoveManagedThreadByRoomID(r.RoomID);
            CardUser card = Gongyong.FKUser.Find(u => u.roomid == getdata.Roomid);

            model.ServerMaJiang mjfh = new model.ServerMaJiang();
            var jp = ReturnFJ.CreateBuilder();
            var muuser = Gongyong.mulist.Find(u => u.Openid.Equals(getdata.Openid) && u.RoomID == getdata.Roomid);
            if (muuser == null)
                return;
            muuser.Is_jiao = true;
            if (muuser.ByteData == requestInfo.Key)
            {
                return;
            }
            else
            {
                muuser.ByteData = requestInfo.Key;
            }

            var huType = ReturnHType.CreateBuilder().SetFWZ(getdata.FWZ).SetFWB(getdata.FWB).SetMJ(getdata.MJ).SetType(getdata.Type);
            if (getdata.DXInfoCount > 1)
                huType.AddRangeDXInfo(getdata.DXInfoList);
            byte[] HType = huType.Build().ToByteArray();
            if (getdata.Type != 1 && getdata.Type != 5)
            {
                model.ServerMaJiang mjh = new model.ServerMaJiang()
                {
                    PaiHs = getdata.MJ.PaiHS,
                    PaiId = getdata.MJ.PaiID
                };

                var roomUsers = Gongyong.mulist.FindAll(u => u.RoomID == getdata.Roomid);
                if (getdata.DXInfoCount > 1)
                {
                    for (int i = 0; i < getdata.DXInfoList.Count; i++)
                    {
                        foreach (var item in roomUsers)
                        {
                            if (item.ZS_Fw == getdata.DXInfoList[i].DXFW)//如果当期玩家为一炮多响的玩家之一，则在手牌中加上 放炮的牌
                                item.majiangs.Add(mjh);
                        }
                    }

                }
                else
                {
                    roomUsers.Find(u => u.ZS_Fw == getdata.FWZ).majiangs.Add(mjh);
                }
            }

            if (getdata.Type == 1 || getdata.Type == 5)
            {
                Gongyong.mulist.Find(u => u.ZS_Fw == getdata.FWZ && u.RoomID == getdata.Roomid).zm_count += 1;
            }
            else
            {
                var mjInfo = Gongyong.mulist.Find(u => u.ZS_Fw == getdata.FWB && u.RoomID == getdata.Roomid);
                mjInfo.dp_count += 1;
                mjInfo.chuda.RemoveAll(w => w.PaiHs == getdata.MJ.PaiHS && w.PaiId == getdata.MJ.PaiID);
            }


            ///没有翻鸡牌
            if (r.RoomPai.Count == 0)
            {
                mjfh.PaiHs = -1;
                mjfh.PaiId = 0;
            }
            else
            {
                mjfh = r.RoomPai[0];
            }



            jp.SetHS(mjfh.PaiHs);
            ///清除牌
            r.RoomPai.Clear();

            List<mjuser> mjlist = Gongyong.mulist.FindAll(u => u.RoomID == getdata.Roomid);
            var settle = ReturnJS.CreateBuilder();
            var returnallmj = ReturnUserSPai.CreateBuilder();
            List<UserSettle> listjs = new List<UserSettle>();
            listjs = new Ting().Settle(r.RoomID, mjfh, getdata);
            var senddjs = ReturnDJS.CreateBuilder();

            if (getdata.DXInfoCount <= 1)
            {
                if (getdata.FWZ != card.win)
                {
                    card.win = getdata.FWZ;
                    card.lianzCount = 1;
                }
                else// (getdata.FWZ == card.win)
                {
                    card.lianzCount += 1;
                }
            }
            else
            {//连庄情况 ，庄家放炮，胡牌的庄， 一炮多响情况 就是 胡牌的最近的人是庄
             //一炮多响非连庄，  谁放炮 谁是庄
                if (r.is_lianz)
                {
                    int zhuang = 0;
                    if (getdata.FWB == card.win)
                    {
                        if (getdata.FWB == 4)
                            zhuang = 1;
                        else
                            zhuang = getdata.FWB + 1;
                        for (int i = 1; i <= 4; i++)
                        {
                            if (getdata.FWB == 4)
                                zhuang = 1;
                            if (getdata.DXInfoList.Any(w => w.DXFW == zhuang))
                            {
                                card.win = zhuang;
                                break;
                            }
                            zhuang++;
                        }

                    }
                    else
                    {
                        card.lianzCount += 1;
                    }
                }
                else { card.win = getdata.FWB; }//非连庄谁放炮谁 是庄
            }

            foreach (var item in mjlist)
            {
                var jsddr = UserAJS.CreateBuilder();
                #region 旧代码
                //if (item.ZS_Fw == getdata.FWZ)
                //{
                //    ///判断清一色
                //    //if (item.majiangs[item.majiangs.Count - 1].PaiHs - item.majiangs[0].PaiHs < 8)
                //    //{
                //    //    int paihs = item.majiangs[0].PaiHs;
                //    //    int qyshs = 0;
                //    //    int qy = 0;
                //    //    if (paihs < 10)
                //    //    {
                //    //        qyshs = 1;
                //    //    }
                //    //    else if (paihs > 10 && paihs < 20)
                //    //    {
                //    //        qyshs = 2;
                //    //    }
                //    //    else
                //    //    {
                //    //        qyshs = 3;
                //    //    }

                //    //    switch (qyshs)
                //    //    {

                //    //        case 1:
                //    //            int count = item.Peng.FindAll(u => u.PaiHs > 10).Count;
                //    //            if (count > 0)
                //    //            {
                //    //                qy++;
                //    //            }
                //    //            if (!string.IsNullOrEmpty(item.Gong))
                //    //            {
                //    //                string gong = string.Copy(item.Gong);
                //    //                string[] arr = gong.Remove(gong.Length - 1, 1).Split(',');
                //    //                foreach (var itemgong in arr)
                //    //                {
                //    //                    if (int.Parse(itemgong[0].ToString()) > 10)
                //    //                    {
                //    //                        qy++;
                //    //                    }
                //    //                }
                //    //            }

                //    //            break;
                //    //        case 2:

                //    //            int countt = item.Peng.FindAll(u => u.PaiHs < 10 || u.PaiHs > 20).Count;
                //    //            if (countt > 0)
                //    //            {
                //    //                qy++;
                //    //            }
                //    //            if (!string.IsNullOrEmpty(item.Gong))
                //    //            {
                //    //                string gong = string.Copy(item.Gong);
                //    //                string[] arr = gong.Remove(gong.Length - 1, 1).Split(',');
                //    //                foreach (var itemgong in arr)
                //    //                {
                //    //                    if (int.Parse(itemgong[0].ToString()) < 10 || int.Parse(itemgong[0].ToString()) > 20)
                //    //                    {
                //    //                        qy++;
                //    //                    }
                //    //                }
                //    //            }

                //    //            break;
                //    //        case 3:

                //    //            int countw = item.Peng.FindAll(u => u.PaiHs < 20).Count;
                //    //            if (countw > 0)
                //    //            {
                //    //                qy++;
                //    //            }
                //    //            if (!string.IsNullOrEmpty(item.Gong))
                //    //            {
                //    //                string gong = string.Copy(item.Gong);
                //    //                string[] arr = gong.Remove(gong.Length - 1, 1).Split(',');
                //    //                foreach (var itemgong in arr)
                //    //                {
                //    //                    if (int.Parse(itemgong[0].ToString()) < 20)
                //    //                    {
                //    //                        qy++;
                //    //                    }
                //    //                }
                //    //            }

                //    //            break;
                //    //        default:
                //    //            break;
                //    //    }
                //    //    if (qy == 0)
                //    //    {
                //    //        item.paixinfs = 10;
                //    //    }

                //    //}
                //} 
                #endregion


                UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
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
                UserInfo gamer = Gongyong.userlist.Find(u => u.openid == item.Openid);
                var users = Userinfo.CreateBuilder().SetHeadimg(gamer.headimg).SetNickname(gamer.nickname).SetOpenid(gamer.openid).SetSex(int.Parse(gamer.sex)).SetUserFW(item.ZS_Fw).SetUserBean(0).SetUserGold(0).SetUserID(gamer.UserID).Build();
                ///分数加减
                foreach (var items in listjs)
                {

                    if (items.openid == item.Openid)
                    {
                        var usersone = UserJS.CreateBuilder();

                        foreach (var itrmj in items.jp)
                        {
                            var jpone = JiPaiF.CreateBuilder().SetPaiHS(itrmj.PaiHS).SetType(itrmj.type).SetCount(itrmj.PaiScare);
                            usersone.AddJp(jpone);
                        }
                        foreach (var itemD in items.gang)
                        {
                            var jpdou = Dou.CreateBuilder().SetCount(itemD.DScare).SetType(itemD.DType);
                            usersone.AddDc(jpdou);
                        }
                        if (getdata.HasFWB)
                        {

                            var info = mjlist.Find(w => w.ZS_Fw == getdata.FWB && w.Openid.Equals(items.openid));
                            if (info != null)
                            {
                                if (items.pai_type != 11)
                                    items.pai_type = 11;
                            }

                        }
                        usersone.SetIsPao(items.pai_type);
                        usersone.SetIsJiao(items.is_jiao);
                        usersone.SetFS(items.scare);
                        usersone.SetUserinfo(users);
                        usersone.SetDyFs(items.dy_fs);
                        var HuLeiXin = getdata.DXInfoList.FirstOrDefault(w => w.DXFW == item.ZS_Fw);
                        if (HuLeiXin != null)
                        {
                            usersone.SetHuType(HuLeiXin.DXType);
                        }

                        settle.AddJs(usersone);


                        switch (item.ZS_Fw)
                        {
                            case 1:
                                card.dong += items.scare;
                                break;
                            case 2:
                                card.nan += items.scare;
                                break;
                            case 3:
                                card.xi += items.scare;
                                break;
                            case 4:
                                card.bei += items.scare;
                                break;
                            default:
                                break;
                        }
                    }
                }





                ///牌局结束，返回大结算
                if (r.Dcount == r.count)
                {
                    new CardsLogic().Clear(item, r);
                    jsddr.SetUser(users);
                    jsddr.SetZdou(item.zwd_count);
                    jsddr.SetZimo(item.zm_count);
                    jsddr.SetAndou(item.ad_count);
                    jsddr.SetMdou(item.MD_count);
                    jsddr.SetDianpao(item.dp_count);

                    //switch (item.ZS_Fw)
                    //{
                    //    case 1:
                    //        jsddr.SetScare(card.dong);
                    //        break;
                    //    case 2:
                    //        jsddr.SetScare(card.nan);
                    //        break;
                    //    case 3:
                    //        jsddr.SetScare(card.xi);
                    //        break;
                    //    case 4:
                    //        jsddr.SetScare(card.bei);
                    //        break;
                    //    default:
                    //        break;
                    //}
                    jsddr.SetScare(card.GetScoreByPosition(item.ZS_Fw));
                    senddjs.AddUserjs(jsddr);
                    senddjs.SetState(1);

                    RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, gamer.openid, gamer.unionid));

                    if (!string.IsNullOrEmpty(userWinLose))
                    {
                        userWinLose += "|";
                    }
                    userWinLose += user.UserID + "," + jsddr.Scare;
                }



            }
            //牌局回放 翻鸡
            var gameOperationInfoFJ = GameOperationInfo.CreateBuilder().AddMJ(MaJiang.CreateBuilder().SetPaiHS(jp.HS).SetPaiID(0)).SetOperationType(9);

            StringBuilder logtxt = new StringBuilder();

            logtxt.Append("胡牌" + getdata.ToString());
            foreach (var item in mjlist)
            {
                item.SendData.Clear();
                logtxt.Append(string.Join(",", item.majiangs.Select(w => w.PaiHs)));
                UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                var sendData5021 = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5021, HType.Length, requestInfo.MessageNum, HType));
                item.SendData.Add(sendData5021);

                byte[] allsp = returnallmj.Build().ToByteArray();

                byte[] jpr = jp.Build().ToByteArray();
                //7005 返回翻鸡牌
                var sendData7005 = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7005, jpr.Length, requestInfo.MessageNum, jpr));
                item.SendData.Add(sendData7005);

                //7008 : 玩家手牌集合
                var sendData7008 = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7008, allsp.Length, requestInfo.MessageNum, allsp));
                item.SendData.Add(sendData7008);

                byte[] settlebyte = settle.Build().ToByteArray();
                r.gameOperationProcess.SetJieSuanInfo(ByteString.CopyFrom(settlebyte));
                //7009 : 返回结算信息(全体)
                var sendData = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7009, settlebyte.Length, requestInfo.MessageNum, settlebyte));
                item.SendData.Add(sendData);

                if (r.Dcount == r.count)
                {
                    byte[] senddjsbyte = senddjs.Build().ToByteArray();
                    //5014 : 大结算信息
                    var sendData5014 = new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5014, senddjsbyte.Length, requestInfo.MessageNum, senddjsbyte));
                    item.SendData.Add(sendData5014);

                }
                foreach (var senddata in item.SendData)
                {
                    user.session.TrySend(senddata);
                }

            }
            session.Logger.Debug(logtxt);
            //牌局回放 胡牌
            var gameOperationInfo = GameOperationInfo.CreateBuilder()
             .SetSerialNumber(r.gameOperationProcess.GameOperationInfoCount)
             .SetOperationFW(huType.FWZ)
             .SetOperationType(5)
             .SetTingHuType(muuser.Is_tianHu ? 1 : 0)
             .SetPaoFW(huType.FWB);

            if (getdata.DXInfoCount > 1)
                gameOperationInfo.AddRangeDXInfo(getdata.DXInfoList);
            r.gameOperationProcess.AddGameOperationInfo(gameOperationInfo);
            gameOperationInfoFJ.SetSerialNumber(r.gameOperationProcess.GameOperationInfoCount).SetOperationFW(0);
            r.gameOperationProcess.AddGameOperationInfo(gameOperationInfoFJ);
            try
            {
                CardsLogic.AddboarddetailsTB(r);
             
            }
            catch (Exception ex)
            {
                session.Logger.Error(ex);
            }
            ///对局结束 清空所有信息
            if (r.Dcount == r.count)
            {
                Gongyong.mulist.RemoveAll(u => u.RoomID == r.RoomID);
                Gongyong.FKUser.RemoveAll(u => u.roomid == r.RoomID);
                Gongyong.roomlist.RemoveAll(u => u.RoomID == r.RoomID);
                RedisUtility.Remove(RedisUtility.GetKey(GameInformationBase.COMMUNITYROOMINFO, r.RoomID.ToString(), ""));
                new RoomInfoDAL().UpdateEndRoomInfoByRoomInfoID(new RoomInfo { RoomInfoID = r.RoomInfoID, EndTime = DateTime.Now, EndStatus = 0, UserWinLose = userWinLose });
                foreach (var item in mjlist)
                {
                    UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                    new userRecordLOGDAL().Add(new userRecordlogLOG { UserID = user.UserID, RoomInfoID = r.RoomInfoID, UserWinLose = userWinLose, EndTime = DateTime.Now, CreateDate = r.CreateDate, RoomID=r.RoomID });
                }
                r.startgame = 0;
            }
            else
            {

                if (r != null)
                {
                    r.RoomPai = new CreateMj().CreateMJ(r.IsYuanQue);
                    r.Dcount += 1;
                    r.startgame = 2;

                }

            }
           

        }
    }
}
