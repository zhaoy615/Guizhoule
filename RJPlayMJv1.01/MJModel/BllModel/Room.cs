using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJModel.BllModel
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
        /// 是否上下鸡
        /// </summary>
        public bool is_xinqiji { get; set; }


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
        public List<MaJiang> RoomPai = new List<MaJiang>();


        public int startgame { get; set; }

        /// <summary>
        /// 当前活跃用户
        /// </summary>
        public int DQHY { get; set; }

        /// <summary>
        /// 当前操作
        /// </summary>
        public string DQcz { get; set; }

        /// <summary>
        /// 当前牌hs碰杠
        /// </summary>
        public MaJiang PaiHSCZ { get; set; }


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
        public MaJiang LastMoMJ { get; set; }
        /// <summary>
        /// 最后打出的牌
        /// </summary>
        public MaJiang LastChuMJ { get; set; }

        /// <summary>
        /// 是否胡牌
        /// </summary>
        public bool Is_Hu { get; set; }
    }

    public class RoomInfo
    {
        /// <summary>
        ///房间信息ID
        /// </summary>
        public string RoomInfoID { get; set; }
        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 是否本鸡
        /// </summary>
        public int IsBenJi { get; set; }
        /// <summary>
        /// 是否乌骨鸡
        /// </summary>
        public int IsWGJ { get; set; }
        /// <summary>
        /// 是否是星期鸡
        /// </summary>
        public int IsXinQiJi { get; set; }

        /// <summary>
        /// 是否上下鸡
        /// </summary>
        public int IsSangXiaJi { get; set; }
        /// <summary>
        /// 算分模式0为一扣二1为一扣三3为连庄
        /// </summary>
        public int CountPointsType { get; set; }
        /// <summary>
        /// 房间人数
        /// </summary>
        public int RoomPeo { get; set; }
        /// <summary>
        /// 房间游戏局数
        /// </summary>
        public int RoomNumber { get; set; }
        /// <summary>
        /// 房间游戏局数
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建房间的用户ID
        /// </summary>
        public long CreateUserID { get; set; }

        public RoomInfo(Room room, long userID)
        {
            RoomInfoID = Guid.NewGuid().ToString();
            RoomID = room.RoomID;
            IsBenJi = room.is_benji ? 1 : 0;
            IsWGJ = room.is_wgj ? 1 : 0;
            IsXinQiJi = room.is_xinqiji ? 1 : 0;
            IsSangXiaJi = room.is_shangxiaji ? 1 : 0;
            CountPointsType = room.is_yikousan ? 1 : (room.is_lianz ? 3 : 0);
            RoomPeo = room.room_peo;
            RoomNumber = room.count;
            CreateDate = DateTime.Now;
            CreateUserID = userID;
        }

    }
}
