using DAL.DAL;
using MJBLL.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.ddz.DZModel
{
    /// <summary>
    /// 斗地主房间对象
    /// </summary>
    public class DDZRoom
    {
        /// <summary>
        /// 房间id
        /// </summary>
        public int RoomID { get; set; }
        /// <summary>
        /// 房间局数
        /// </summary>
        public int RoomNumber { get; set; }
        /// <summary>
        /// 房间类型(1叫地主,2叫分)
        /// </summary>
        public int RoomType { get; set; }
        /// <summary>
        /// 最高倍数(16，32，64)
        /// </summary>
        public int MaxMultiple { get; set; }

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
       // public GameOperationProcess.Builder gameOperationProcess { get; set; }
        /// <summary>
        /// 圈子ID
        /// </summary>
        public int GroupID { get; set; }
        /// <summary>
        /// 创建房间的用户ID
        /// </summary>
        public long CreateUserID { get; set; }
        /// <summary>
        /// 当前游戏局数
        /// </summary>
        public int CurrentRoomNumber { get; set; }
        /// <summary>
        /// 当前游戏倍数
        /// </summary>
        public int CurrentMultiple { get; set; }
        /// <summary>
        /// 牌桌数据
        /// </summary>
       // public TableData MyTableData { get; set; }


        public bool Is_Jin { get; set; }


        public int Juser1 { get; set; }
        public int Juser2 { get; set; }
        /// <summary>
        /// 确认开始的玩家人数
        /// </summary>
        public int ConfirmStartingNumber { get; set; }
        /// <summary>
        /// 是否存在警告确认提示
        /// </summary>
        public bool WarningConfirmation { get; set; }
        /// <summary>
        /// 上局赢家
        /// </summary>
        public long winner { get; set; }

        /// <summary>
        /// 牌桌玩家OpendID集合
        /// </summary>
        public List<string> listOpenid = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <param name="roomNumber">创建的房间局数</param>
        /// <param name="roomType">房间类型</param>
        /// <param name="multiple">房间最高倍数</param>
        /// <param name="groupID">圈子ID</param>
        /// <param name="createUserID"></param>
        public DDZRoom(int roomID, int roomNumber, int roomType, int maxMultiple, int groupID, long createUserID)
        {
            RoomID = roomID;
            RoomNumber = roomNumber;
            RoomType = roomType;
            MaxMultiple = maxMultiple;
            GroupID = groupID;
            CreateUserID = createUserID;
            CreateDate = DateTime.Now;
            bool exists = false;
            string roomInfoID = string.Empty;
            RoomInfoDAL dal = new RoomInfoDAL();
            do
            {
                roomInfoID = Guid.NewGuid().ToString();
                exists = dal.GetExistsByRoomInfoID(roomInfoID);
            } while (exists);
            RoomInfoID = roomInfoID;
            CurrentRoomNumber = 1;
            CurrentMultiple = 1;

        }

        /// <summary>
        /// 获取牌桌数据
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="tableData"></param>
        /// <returns></returns>
        //public static bool TryGetTableValue(int roomID, out TableData tableData)
        //{
        //    tableData = null;
        //    var info = Gongyong.ddzroomlist.First(w => w.Key == roomID);
        //    if (info.Value != null)
        //        tableData = info.Value.MyTableData;
        //    return info.Value != null;
        //}
    }
}
