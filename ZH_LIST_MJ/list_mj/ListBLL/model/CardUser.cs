using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.model
{
    public class CardUser
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public int roomid { get; set; }
        /// <summary>
        /// 东方位
        /// </summary>
        public int dong { get; set; }
        /// <summary>
        /// 南方位
        /// </summary>
        public int nan { get; set; }
        /// <summary>
        /// 西方位
        /// </summary>
        public int xi { get; set; }
        /// <summary>
        /// 北方位
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



        public int lianzCount { get; set; }
    }



    public class Room_JX {
        public int room_id { get; set; }

        public int room_peo { get; set; }
    }
}
