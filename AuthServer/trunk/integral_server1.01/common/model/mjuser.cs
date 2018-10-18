using MJBLL.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.model
{

    public class mjuser
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomID { get; set; }
 /// <summary>
        /// 是否是房主
        /// </summary>
        public bool IsHomeowner { get; set; }
        /// <summary>
        /// 用户openid
        /// </summary> 
        public string Openid { get; set; }

        /// <summary>
        /// 是否天胡
        /// </summary>
        public bool Is_tianHu { get; set; }
        /// <summary>
        /// 是否天听
        /// </summary>
        public bool Is_tiant { get; set; }
        /// <summary>
        /// 是否选择天听
        /// </summary>
        public int Is_tiantX { get; set; }
        /// <summary>
        /// 是否报听
        /// </summary>
        public bool Is_baotin { get; set; }

        /// <summary>
        /// 是否有冲锋鸡
        /// </summary>
        public bool Is_cfj { get; set; }

        /// <summary>
        /// 判读责任鸡(-1:自己对别人责任；0:无责任;1:别人对自己责任)
        /// </summary>
        public int Is_zrj { get; set; }

        /// <summary>
        /// 是否冲锋乌骨鸡
        /// </summary>
        public bool is_cfwg { get; set; }

        /// <summary>
        /// 判读责任乌骨鸡(-1:自己对别人责任；0:无责任;1:别人对自己责任)
        /// </summary>
        public int is_zrwg { get; set; }

        /// <summary>
        /// 杠牌集合(1111|1,2222|1)|前4位表示杠的牌,|后表示方位
        /// </summary>
        public string Gong { get; set; }

        /// <summary>
        /// 碰牌集合
        /// </summary>
        public List<ServerMaJiang> Peng = new List<ServerMaJiang>();

        /// <summary>
        /// 是否叫牌
        /// </summary>
        public bool Is_jiao { get; set; }

        /// <summary>
        /// 座位(1,2,3,4代表东,南,西,北)
        /// </summary>
        public int ZS_Fw { get; set; }

        /// <summary>
        /// 手牌
        /// </summary>
        public List<ServerMaJiang> majiangs = new List<ServerMaJiang>();

        /// <summary>
        /// 打出去的pai
        /// </summary>
        public List<ServerMaJiang> chuda = new List<ServerMaJiang>();


        /// <summary>
        /// 麻将牌型
        /// </summary>
        public int paixinfs { get; set; }


        /// <summary>
        /// 胡牌方式(0:自摸，1:炮胡，2：杠上炮)
        /// </summary>
        public int HPFS { get; set; }


        /// <summary>
        /// 缺一门（1：筒；2：条；3：万）
        /// </summary>
        public int QYM { get; set; }


        /// <summary>
        /// 自摸次数
        /// </summary>
        public int zm_count { get; set; }

        /// <summary>
        /// 点炮次数
        /// </summary>
        public int dp_count { get; set; }

        /// <summary>
        /// 暗斗次数
        /// </summary>
        public int ad_count { get; set; }

        /// <summary>
        /// 明豆次数
        /// </summary>
        public int MD_count { get; set; }

        /// <summary>
        /// 转弯豆次数
        /// </summary>
        public int zwd_count { get; set; }




        /// <summary>
        /// 上一条消息
        /// </summary>
        public String ByteData { get; set; }
        /// <summary>
        /// 缺一门，碰杠胡消息。断线重连时使用
        /// </summary>
        public List<ArraySegment<byte>> SendData { get; set; }
        /// <summary>
        /// 确认开始
        /// </summary>
        public bool ConfirmationStarts { get; set; }

        /// <summary>
        /// 是抢杠
        /// </summary>
        public bool IsGrabBars { get; set; }
        /// <summary>
        /// 被抢杠的玩家openid
        /// </summary>
        public string WasGrabbedUserOpenID { get; set; }
        /// <summary>
        /// 是否过胡
        /// </summary>
        public bool IsGuoHu { get; set; }
        /// <summary>
        /// 摸牌状态(0:普通状态;1:杠后摸牌)
        /// </summary>
        public int Mtype { get; set; }
        /// <summary>
        /// 能否报听
        /// </summary>
        public bool CanBaoTing { get; set; }

        /// <summary>
        /// 摸牌次数
        /// </summary>
        public int MopaiNumber { get; set; }

        /// <summary>
        ///连接状态
        /// </summary>
        public int ConnectionStatus { get; set; }

        /// <summary>
        ///是否托管
        /// </summary>
        public int IsManaged { get; set; }

        public mjuser()
        {
            ConnectionStatus = 1;
        }
    }
}
