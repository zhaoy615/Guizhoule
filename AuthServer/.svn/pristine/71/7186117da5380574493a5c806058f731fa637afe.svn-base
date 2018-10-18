using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.model
{
    public class Room
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomID { get; set; }
        /// <summary>
        /// 是否乌骨鸡
        /// </summary>
        public bool is_wgj { get; set; }
     /// <summary>
     /// 是否是星期鸡
     /// </summary>
        public bool is_xinqiji { get; set; }

        /// <summary>
        /// 是否上下鸡
        /// </summary>
        public bool is_shangxiaji { get; set; }
        /// <summary>
        /// 是否本鸡
        /// </summary>
        public bool is_benji { get; set; }
        /// <summary>
        /// 是否一扣三
        /// </summary>
        public bool is_yikousan { get; set; }
        /// <summary>
        /// 房间人数
        /// </summary>
        public int room_peo { get; set; }
        /// <summary>
        /// 房间游戏局数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 当前游戏局数
        /// </summary>
        public int Dcount { get; set; }
        /// <summary>
        /// 麻将集合
        /// </summary>
        public List<ServerMaJiang> RoomPai = new List<ServerMaJiang>();


        public int startgame { get; set; }

        /// <summary>
        /// 当前活跃用户
        /// </summary>
        public int DQHY { get; set; }

        /// <summary>
        /// 当前操作 30083:摸牌， 30082：杠牌， 30081：碰， 30080：胡，30070：天胡,30071天听，   3001 出牌 3002选择缺一门
        /// 30082，1：1,明杠，2转弯杠，3暗杠，4憨包杠
        /// </summary>
        public string DQcz { get; set; }

        /// <summary>
        /// 当前牌hs碰杠
        /// </summary>
        public ServerMaJiang PaiHSCZ { get; set; }


        /// <summary>
        /// 获取手牌的玩家数
        /// </summary>
        public int MPS { get; set; }

        /// <summary>
        /// 是否连庄
        /// </summary>
        public bool is_lianz { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> listOpenid = new List<string>();



        public bool Is_Jin { get; set; }


        public int Juser1 { get; set; }
        public int Juser2 { get; set; }
        /// <summary>
        /// 最后摸到的牌
        /// </summary>
        public ServerMaJiang LastMoMJ { get; set; }
        /// <summary>
        /// 最后打出的牌
        /// </summary>
        public ServerMaJiang LastChuMJ { get; set; }

        /// <summary>
        /// 是否胡牌
        /// </summary>
        public bool Is_Hu { get; set; }

        /// <summary>
        /// 牌桌信息表ID
        /// </summary>
        public string RoomInfoID { get; set; }
        /// <summary>
        /// 牌桌创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 牌局操作流程
        /// </summary>
        public GameOperationProcess.Builder gameOperationProcess { get; set; }

        /// <summary>
        /// 是否为原缺
        /// </summary>
        public bool IsYuanQue { get; set; }
        /// <summary>
        /// 快速出牌
        /// </summary>
        public bool QuickCard{ get; set; }
        public Room()
        {
            gameOperationProcess = GameOperationProcess.CreateBuilder();
        }
    }

   
}
