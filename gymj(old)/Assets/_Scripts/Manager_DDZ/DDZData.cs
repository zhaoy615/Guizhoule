using DDZ;
using System.Collections.Generic;

public static class DDZData {
    public struct RoomInfo
    {
        public RoomInfo(int num, int type, int mul)
        {
            inningNum = num;
            playType = type;
            multiple = mul;
        }
        public int inningNum;
        public int playType;
        public int multiple;
    }
    public enum GamePattern
    {
        cryLandlord,
        cryScore
    }
    public enum UserOperation
    {
    }
    public static GamePattern gamePattern = GamePattern.cryLandlord;
    public static int HyUser = 0;
    public static UserOperation userOperation;
    public static int landlord = 0;
    public static int score = 0;
    public static int multiple = 0;
    public static int room_id;
    public static int fw;
    public static Dictionary<long,ddzUserinfo> userInfoList = new Dictionary<long, ddzUserinfo>();
    public static RoomInfo roomInfo = new RoomInfo(8,1,16);
    public static ddzReturnCreateRoom returnCreateRoom = null;
    public static ddzReturnAddRoom returnAddRoom = null;
    public static ddzReturnUserInfo returnUserInfo = null;
    public static ddzReturnStartGame returnStartGame = null;
    public static ddzReturnStart returnStart = null;
    public static ddzReturnCallLandlord returnCallLandlord = null;
    public static ddzReturnOutCard returnOutCard = null;
    public static ddzReturnOutCardAll returnOutCardAll = null;



}
