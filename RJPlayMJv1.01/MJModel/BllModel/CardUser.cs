using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJModel.BllModel
{
    /// <summary>
    /// 房卡麻将计分类
    /// </summary>
    public class CardUser
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public int roomid { get; set; }
        /// <summary>
        /// 东方位分数
        /// </summary>
        public int dong { get; set; }
        /// <summary>
        /// 南方位分数
        /// </summary>
        public int nan { get; set; }
        /// <summary>
        /// 西方位分数
        /// </summary>
        public int xi { get; set; }
        /// <summary>
        /// 北方位分数
        /// </summary>
        public int bei { get; set; }
        /// <summary>
        /// 上局赢家
        /// </summary>
        public int win { get; set; }


        public int dongM { get; set; }


        public int lanM { get; set; }

        public int xiM { get; set; }




        public int beiM { get; set; }

        /// <summary>
        /// 当前局数
        /// </summary>
        public int jushu { get; set; }


        /// <summary>
        /// 连庄得分
        /// </summary>
        public int lianzCount { get; set; }
        /// <summary>
        /// 根据访问获取方位分数
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int GetScoreByPosition(int position)
        {
            switch (position)
            {
                case 1:
                  return dong;
                case 2:
                    return nan;
                case 3:
                    return xi;
                case 4:
                    return bei;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 摇色子的点数 结构为点数,点数
        /// </summary>
        public string Points { get; set; }
        public void GetPoints()
        {
            if (string.IsNullOrEmpty(Points))
            {
                int points1 = new Random().Next(1, 7);
                int points2 = new Random().Next(1, 7);
                Points = points1 + "," + points2;
            }
        }
        public CardUser()
        {
            GetPoints();
        }

    }



    public class Room_JX {
        public int room_id { get; set; }

        public int room_peo { get; set; }
    }
}
