using DDZ;
using UnityEngine;

public class InitGame : Singleton<InitGame> {
    private void Start()
    {
        LoadUserInfo(DDZData.userInfoList[GameInfo.userID]);
    }
    private void Update()
    {
        
    }
    public void OnReturnUserInfo()
    {
        foreach (ddzUserinfo info in DDZData.returnUserInfo.userinfo)
        {
            if (!DDZData.userInfoList.ContainsKey(info.UserID))
            {
                DDZData.userInfoList[info.UserID] = info;
                LoadUserInfo(info);
            } 
        }
        DDZData.returnUserInfo = null;
    }
    public void LoadUserInfo(ddzUserinfo userInfo)
    {
        Desktop_UI.Instance.SetHeads(userInfo.user_FW, userInfo.headimg);
        Desktop_UI.Instance.SetUserName(userInfo.user_FW, userInfo.nickname);
    }
}
