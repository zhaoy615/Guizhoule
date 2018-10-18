using DAL.DAL;
using DAL.Model;
using MJBLL.common;
using MJBLL.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.Logic
{
    /// <summary>
    /// 牌逻辑
    /// </summary>
    public class CardsLogic
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestInfo">请求信息</param>
        /// <param name="sendmj">返回开始游戏集合</param>
        /// <param name="ReturnGangMSG">返回杠牌集合信息</param>
        /// <param name="Ruturnjsmj">返回打出叫牌的集合</param>
        /// <param name="session">ss请求</param>
        /// <param name="roomID">房间ID</param>
        /// <param name="mj">当前玩家</param>
        /// <param name="number">请求次数</param>
        /// <param name="openid"></param>
        public void GetMyCards(ProtobufRequestInfo requestInfo, ReturnStartGame.Builder sendmj, ReturnGang.Builder ReturnGangMSG,
                                List<model.ServerMaJiang> Ruturnjsmj, GameSession session, int roomID, mjuser mj, string openid, ref int number)
        {
            ReturnGangMSG = ReturnGang.CreateBuilder();
            Ruturnjsmj = new List<model.ServerMaJiang>();
            ///房间牌堆
            List<model.ServerMaJiang> listmj = new List<model.ServerMaJiang>();
            CreateMj C = new CreateMj();

            mj.MopaiNumber = 0;

            ///房间牌堆赋值
            Room R = new Room();
            Room_JX rjx = new Room_JX();
            rjx = Gongyong.room_JX.Find(u => u.room_id == roomID);
            if (rjx != null)
            {
                rjx.room_peo = 0;
            }

            R = Gongyong.roomlist.Find(u => u.RoomID == roomID);
            R.MPS++;

            if (Gongyong.roomlist.Find(u => u.RoomID == roomID).RoomPai.Count == 0)
            {
                listmj = C.CreateMJ(R.IsYuanQue);

            }
            else
                listmj = R.RoomPai;
#if DEBUG

         /*    listmj.Insert(0, new ServerMaJiang { PaiHs = 2, PaiId = 201 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 2, PaiId = 202 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 2, PaiId = 203 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 2, PaiId = 204 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 1, PaiId = 205 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 1, PaiId = 206 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 1, PaiId = 207 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 1, PaiId = 208 });
         listmj.Insert(0, new ServerMaJiang { PaiHs = 5, PaiId = 209 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 5, PaiId = 210 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 6, PaiId = 211 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 6, PaiId = 212 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 7, PaiId = 213 });
            listmj.Insert(0, new ServerMaJiang { PaiHs = 7, PaiId = 214 });  */
#endif
            R.startgame = 1;
            CardUser cu = Gongyong.FKUser.Find(u => u.roomid == roomID);
            if (cu == null)
            {
                cu = new CardUser();
                cu.roomid = roomID;
                cu.win = 1;
                cu.dong = 0;
                cu.xi = 0;
                cu.nan = 0;
                cu.bei = 0;
                cu.jushu = 0;
                cu.dongM = 0;
                cu.xiM = 0;
                cu.nan = 0;
                cu.beiM = 0;
                cu.lianzCount = 1;
                Gongyong.FKUser.Add(cu);
            }
            else
            {
                cu.dongM = 0;
                cu.xiM = 0;

                cu.beiM = 0;
            }

            //  int x = 0;
            int y = 0;

            #region 根据玩家方位判断玩家手牌

            string logs = "";
            for (int i = 0; i < listmj.Count; i++)
            {
                logs += listmj[i].PaiHs + ",";
            }

            session.Logger.Debug("牌堆" + logs);

            //int x = 0;
            //int y = 0;

            if (mj.ZS_Fw == cu.win)
            {
                //  x = 0;
                y = 14;
            }
            else
            {
                // x = 0;
                y = 13;

            }
            #endregion


            ///组装下发数据
            foreach (var item in listmj.Take(y))
            {
                MaJiang fanmj = MaJiang.CreateBuilder().SetPaiHS(item.PaiHs).SetPaiID(item.PaiId).Build();
                mj.majiangs.Add(item);
                int count = mj.majiangs.Count;
                sendmj.AddMj(fanmj);
            }

            // number += y;
            listmj.RemoveRange(0, y);
            R.RoomPai = listmj;
            mj.majiangs.Sort((a, b) => -b.PaiHs.CompareTo(a.PaiHs));
            Gongyong.mulist.Add(mj);
            Ting t = new Ting();
            List<model.ServerMaJiang> listT = new List<model.ServerMaJiang>();
            listT.AddRange(mj.majiangs.ToArray());

            UserInfo userSendJ = Gongyong.userlist.Find(u => u.openid == openid);
            int PaiHS = 0;
            if (R.room_peo == 4 || R.IsYuanQue)
            {
                if (cu.win == mj.ZS_Fw)
                {
                    foreach (var item in listT)
                    {
                        List<model.ServerMaJiang> listchuan = listT.FindAll(u => u.PaiHs == item.PaiHs);
                        if (listchuan.Count == 4)
                        {


                            if (PaiHS != item.PaiHs)
                            {
                                var PGH = ReturnAll.CreateBuilder();
                                var mjG = MaJiang.CreateBuilder();
                                mjG.SetPaiHS(item.PaiHs);
                                mjG.SetPaiID(item.PaiId);

                                ReturnGangMSG.AddGang(GangMSG.CreateBuilder().SetFw(cu.win).SetMj(mjG).SetType(3));
                            }
                            PaiHS = item.PaiHs;

                        }

                    }


                    if (ReturnGangMSG.GangCount > 0)
                    {
                        byte[] bytegang = ReturnGangMSG.Build().ToByteArray();
                        userSendJ.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5022, bytegang.Length, requestInfo.MessageNum, bytegang)));
                    }
                }
            }
            var ttAthState = ReturnTTATH.CreateBuilder();

            if (cu.win == mj.ZS_Fw)
            {

                if (t.GetTing(listT) == "H")
                {
                    if (R.room_peo < 4)
                    {
                        switch (mj.QYM)
                        {
                            case 1:
                                if (mj.majiangs.FindAll(u => u.PaiHs < 10).Count <= 0)
                                {
                                    // mj.Is_tiantX = -1;
                                    ttAthState.SetState(214);
                                    mj.Is_tianHu = true;
                                }
                                break;
                            case 2:
                                if (mj.majiangs.FindAll(u => u.PaiHs > 10 && u.PaiHs < 20).Count <= 0)
                                {
                                    // mj.Is_tiantX = -1;
                                    ttAthState.SetState(214);
                                    mj.Is_tianHu = true;
                                }
                                break;
                            case 3:
                                if (mj.majiangs.FindAll(u => u.PaiHs > 20).Count <= 0)
                                {
                                    //mj.Is_tiantX = -1;
                                    ttAthState.SetState(214);
                                    mj.Is_tianHu = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ttAthState.SetState(214);
                        // mj.Is_tiantX = -1;
                        mj.Is_tianHu = true;
                    }
                }
                else
                {

                    if (R.room_peo < 4)
                    {

                        switch (mj.QYM)
                        {
                            case 1:
                                if (mj.majiangs.FindAll(u => u.PaiHs < 10).Count <= 0)
                                {
                                    ///庄家报听判断
                                    List<model.ServerMaJiang> JPMJ = new List<model.ServerMaJiang>();
                                    JPMJ.AddRange(mj.majiangs.ToArray());

                                    Ruturnjsmj = t.ReturnJMJ(JPMJ);
                                }

                                break;
                            case 2:
                                if (mj.majiangs.FindAll(u => u.PaiHs > 10 && u.PaiHs < 20).Count <= 0)
                                {
                                    ///庄家报听判断
                                    List<model.ServerMaJiang> JPMJ = new List<model.ServerMaJiang>();
                                    JPMJ.AddRange(mj.majiangs.ToArray());
                                    Ruturnjsmj = t.ReturnJMJ(JPMJ);
                                }
                                break;
                            case 3:
                                if (mj.majiangs.FindAll(u => u.PaiHs > 20).Count <= 0)
                                {
                                    ///庄家报听判断
                                    List<model.ServerMaJiang> JPMJ = new List<model.ServerMaJiang>();
                                    JPMJ.AddRange(mj.majiangs.ToArray());
                                    Ruturnjsmj = t.ReturnJMJ(JPMJ);
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
                        JPMJ.AddRange(mj.majiangs.ToArray());
                        Ruturnjsmj = t.ReturnJMJ(JPMJ);
                    }

                }
            }
            else
            {    ///判断天听
                if (t.GetTing(listT) != "MJ")
                {
                    if (R.room_peo < 4)
                    {
                        switch (mj.QYM)
                        {
                            case 1:
                                if (mj.majiangs.FindAll(u => u.PaiHs < 10).Count <= 0)
                                {

                                    ttAthState.SetState(213);

                                    mj.Is_tiantX = -1;
                                    mj.Is_tiant = true;
                                }
                                break;
                            case 2:
                                if (mj.majiangs.FindAll(u => u.PaiHs > 10 && u.PaiHs < 20).Count <= 0)
                                {

                                    ttAthState.SetState(213);


                                    mj.Is_tiantX = -1;
                                    mj.Is_tiant = true;
                                }
                                break;
                            case 3:
                                if (mj.majiangs.FindAll(u => u.PaiHs > 20).Count <= 0)
                                {

                                    ttAthState.SetState(213);

                                    mj.Is_tiantX = -1;
                                    mj.Is_tiant = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ttAthState.SetState(213);

                        mj.Is_tiantX = -1;
                        mj.Is_tiant = true;
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
                userSendJ.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2009, tmjsr.Length, requestInfo.MessageNum, tmjsr)));
            }
            sendmj.SetChuuser(cu.win);
            //牌局回放 发牌
            R.gameOperationProcess.AddGameOperationInfo(GameOperationInfo.CreateBuilder()
                .SetOperationFW(mj.ZS_Fw)
                .AddRangeMJ(sendmj.MjList)
                .SetSerialNumber(R.gameOperationProcess.GameOperationInfoCount)
                .SetOperationType(0)
                .SetChuUser(cu.win)
                );
            byte[] sendbyte = sendmj.Build().ToByteArray();
            userSendJ.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2006, sendbyte.Length, requestInfo.MessageNum, sendbyte)));

            if (ttAthState.HasState)
            {
                byte[] sendbytes = ttAthState.Build().ToByteArray();
                userSendJ.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 2010, sendbytes.Length, requestInfo.MessageNum, sendbytes)));
            }
            ///判断房间人数满足 并且没有天听用户才发生活跃用户
            if (R.MPS == R.room_peo)
            {
                List<mjuser> listmjuserstart = Gongyong.mulist.FindAll(u => u.RoomID == mj.RoomID);
                var userbool = Gongyong.mulist.FindAll(u => u.RoomID == R.RoomID && u.Is_tiant == true);
                if (userbool.Count == 0 && (R.room_peo == 4 || R.IsYuanQue))//当原缺或者四人桌，没有人天听的时候 为庄家挂上倒计时出牌
                {
                    byte[] startHY = ReturnHyUser.CreateBuilder().SetCz("3001").SetFw(cu.win).Build().ToByteArray();
                    R.DQHY = cu.win;
                    R.DQcz = "3001";
                    foreach (var item in listmjuserstart)
                    {
                        UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                        user.IsActive = true;
                        user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 7004, startHY.Length, GameInformationBase.BASEAGREEMENTNUMBER + 7093, startHY)));

                    }
                    if (number == 0)
                        ThreadUtility.StartManagedThread(listmjuserstart.Find(w => w.ZS_Fw == cu.win).Openid, R.RoomID, R.DQcz);
                }
                else if (userbool.Count > 0)//当有天听时， 为每个天听用户挂上倒计时
                {
                    R.DQHY = 0;
                    R.DQcz = "30071";
                    if (number == 0)
                    {
                        foreach (var item in userbool)
                        {
                            ThreadUtility.StartManagedThread(item.Openid, R.RoomID, R.DQcz);
                        }
                    }

                }
                else//需要选择缺一门时 为每个用户挂上倒计时
                {
                    R.DQHY = 0;
                    R.DQcz = "3002";
                    if (number == 0)
                    {
                        foreach (var item in listmjuserstart)
                        {
                            ThreadUtility.StartManagedThread(item.Openid, R.RoomID, R.DQcz);
                        }
                    }
                }
                foreach (var item in listmjuserstart)
                {
                    UserInfo user = Gongyong.userlist.Find(u => u.openid == item.Openid);
                    byte[] returnzhuang = ReturnZhuang.CreateBuilder().SetZhuang(cu.win).SetSeizi(cu.Points).SetZhuangCount(cu.lianzCount).Build().ToByteArray();
                    user.session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 5020, returnzhuang.Length, GameInformationBase.BASEAGREEMENTNUMBER + 7093, returnzhuang)));
                }
                // ThreadUtility.StartManagedThread(R.RoomID.ToString(), R.RoomID);
                StringBuilder mjrz = new StringBuilder();
                //foreach (var item in mj.majiangs)
                //{

                //    mjrz.Append(item.PaiHs + ",");

                //}
                //session.Logger.Debug("方位------" + mj.ZS_Fw + "----" + mjrz);
                number++;
            }

        }
        /// <summary>
        /// 清空当前局数玩家信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="r"></param>
        internal void Clear(mjuser item, Room r)
        {
            ///清空当前局数玩家信息
            item.majiangs.Clear();
            item.Peng.Clear();
            item.paixinfs = 0;
            item.QYM = 0;
            item.Gong = "";
            item.chuda.Clear();
            item.ByteData = string.Empty;
            item.Is_baotin = false;
            item.is_cfwg = false;
            item.Is_cfj = false;
            item.Is_zrj = 0;
            item.is_zrwg = 0;
            item.Is_jiao = false;
            item.Is_tiant = false;
            item.Is_baotin = false;
            item.IsGuoHu = false;
            item.Is_tianHu = false;
            item.Is_tiantX = 0;
            r.RoomPai.Clear();
            r.MPS = 0;
            r.PaiHSCZ = null;
            r.DQcz = "";
            r.DQHY = 0;
        }
        public static RoomInfo SetRoomInfoTb(Room room, long userID)
        {
            bool exists = false;
            RoomInfoDAL dal = new RoomInfoDAL();
            string roomInfoID = string.Empty;
            do
            {
                roomInfoID= Guid.NewGuid().ToString();
                exists = dal.GetExistsByRoomInfoID(roomInfoID);
            } while (exists);
          
            RoomInfo roomInfo = new RoomInfo();
            roomInfo. RoomInfoID = roomInfoID;
            room.RoomInfoID= roomInfoID;
            roomInfo.RoomID = room.RoomID;
            roomInfo.IsBenJi = room.is_benji ? 1 : 0;
            roomInfo.IsWGJ = room.is_wgj ? 1 : 0;
            roomInfo.IsXinQiJi = room.is_xinqiji ? 1 : 0;
            roomInfo.IsSangXiaJi = room.is_shangxiaji ? 1 : 0;
            roomInfo.CountPointsType = room.is_yikousan ? 1 : (room.is_lianz ? 3 : 0);
            roomInfo.RoomPeo = room.room_peo;
            roomInfo.RoomNumber = room.count;
            roomInfo.CreateDate = DateTime.Now;
            roomInfo.CreateUserID = userID;
            roomInfo.IsYuanQue = room.IsYuanQue?1:0;
            roomInfo.QuickCard = room.QuickCard ? 1 : 0;
            return roomInfo;
        }
        /// <summary>
        /// 将牌局回放数据存入数据库
        /// </summary>
        /// <param name="r"></param>
        public static void AddboarddetailsTB(Room r)
        {
            string InningsID = string.Empty;
            bool exists = false;
            var dal = new BoarddetailstbDAL();
            do
            {
                InningsID = Guid.NewGuid().ToString();
                exists = dal.GetExistsByInningsIDID(InningsID);
            } while (exists);
            dal.Add(new boarddetailsTB { InningsID = InningsID, RoomInfoID = r.RoomInfoID, GameSummary = r.gameOperationProcess.JieSuanInfo.ToArray(), MatchDetails = r.gameOperationProcess.Build().ToByteArray() });

        }
    }

}
