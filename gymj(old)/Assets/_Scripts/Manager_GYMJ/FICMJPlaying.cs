using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MJBLL.common;
using UnityEngine.EventSystems;

using DNL;
public class FICMJPlaying : MonoBehaviour
{
    private  Userinfo userBack;
    private  Userinfo userLeft;
    private  Userinfo userRright;
    private  Userinfo userFront;
    //back位置
    public GameObject backHeadFGO;
    public GameObject backGridGO;
    public Image backHeadImage;
    public Text MeNickNameText;
    public Text distanceText;
    public Text backIPText;
    public Text backIDText;
    public Text backName;
    private List<int> backCardsInList = new List<int>();
    private List<int> backCardsOutList = new List<int>();
    //left位置
    public GameObject leftHeadFGO;
    public GameObject leftGridGO;
    public Image leftHeadImage;
    public Text leftNickNameText;
    public Text leftDistanceText;
    public Text leftIPText;
    public Text leftIDText;
    public Text leftName;
    public Text leftCityText;
    private List<int> leftCardsInList = new List<int>();
    private List<int> leftCardsOutList = new List<int>();

    //right位置
    public GameObject rightHeadFGO;
    public GameObject rightGridGO;
    public Image rightHeadImage;
    public Text rightNickNameText;
    public Text rightDistanceText;
    public Text rightIPText;
    public Text rightIDText;
    public Text rightName;
    public Text rightCityText;
    private List<int> rightCardsInList = new List<int>();
    private List<int> rightCardsOutList = new List<int>();

    //front位置
    public GameObject frontHeadFGO;
    public GameObject frontGridGO;
    public Image frontHeadImage;
    public Text frontNickNameText;
    public Text frontDistanceText;
    public Text frontIPText;
    public Text frontIDText;
    public Text frontName;
    public Text frontCityText;
    private List<int> frontCardsInList = new List<int>();
    private List<int> frontCardsOutList = new List<int>();

    public Manager_Game managerGame;

    private void Start()
    {
        managerGame = GetComponent<Manager_Game>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //=====================south头像，昵称，距离，鸡牌Grid赋值========================
        backHeadFGO = transform.Find("/Game_UI/Fixed_UI/Heads/Head_south").gameObject;
        backGridGO = backHeadFGO.transform.Find("grid_ji").gameObject;
        backHeadImage = backHeadFGO.transform.Find("bottom").GetComponent<Image>();
        MeNickNameText = managerGame.userInfoPel_S.transform.Find("Image/TxtLeft").GetComponent<Text>();
        distanceText = managerGame.userInfoPel_S.transform.Find("Image/UsrDistance").GetComponent<Text>();
        backIDText = managerGame.userInfoPel_S.transform.Find("Image/ID").GetComponent<Text>();
        backIPText = managerGame.userInfoPel_S.transform.Find("Image/IP").GetComponent<Text>();
        backName = backHeadFGO.transform.Find("name").GetComponent<Text>();
        //=====================west头像，昵称，距离，鸡牌Grid赋值========================
        leftHeadFGO = transform.Find("/Game_UI/Fixed_UI/Heads/Head_west").gameObject;
        leftGridGO = leftHeadFGO.transform.Find("grid_ji").gameObject;
        leftHeadImage = leftHeadFGO.transform.Find("bottom").GetComponent<Image>();
        leftNickNameText = managerGame.userInfoPel_W.transform.Find("Image/TxtLeft").GetComponent<Text>();
        leftDistanceText = managerGame.userInfoPel_W.transform.Find("Image/UsrDistance").GetComponent<Text>();
        leftIDText = managerGame.userInfoPel_W.transform.Find("Image/ID").GetComponent<Text>();
        leftIPText = managerGame.userInfoPel_W.transform.Find("Image/IP").GetComponent<Text>();
        leftName = leftHeadFGO.transform.Find("name").GetComponent<Text>();
        leftCityText= leftHeadFGO.transform.Find("city").GetComponent<Text>();
        //=====================east头像，昵称，距离，鸡牌Grid赋值========================
        rightHeadFGO = transform.Find("/Game_UI/Fixed_UI/Heads/Head_east").gameObject;
        rightGridGO = rightHeadFGO.transform.Find("grid_ji").gameObject;
        rightHeadImage = rightHeadFGO.transform.Find("bottom").GetComponent<Image>();
        rightNickNameText = managerGame.userInfoPel_E.transform.Find("Image/TxtLeft").GetComponent<Text>();
        rightDistanceText = managerGame.userInfoPel_E.transform.Find("Image/UsrDistance").GetComponent<Text>();
        rightIDText = managerGame.userInfoPel_E.transform.Find("Image/ID").GetComponent<Text>();
        rightIPText = managerGame.userInfoPel_E.transform.Find("Image/IP").GetComponent<Text>();
        rightName = rightHeadFGO.transform.Find("name").GetComponent<Text>();
        rightCityText= rightHeadFGO.transform.Find("city").GetComponent<Text>();
        //=====================north头像，昵称，距离，鸡牌Grid赋值========================
        frontHeadFGO = transform.Find("/Game_UI/Fixed_UI/Heads/Head_north").gameObject;
        frontGridGO = frontHeadFGO.transform.Find("grid_ji").gameObject;
        frontHeadImage = frontHeadFGO.transform.Find("bottom").GetComponent<Image>();
        frontNickNameText = managerGame.userInfoPel_N.transform.Find("Image/TxtLeft").GetComponent<Text>();
        frontDistanceText = managerGame.userInfoPel_N.transform.Find("Image/UsrDistance").GetComponent<Text>();
        frontIDText = managerGame.userInfoPel_N.transform.Find("Image/ID").GetComponent<Text>();
        frontIPText = managerGame.userInfoPel_N.transform.Find("Image/IP").GetComponent<Text>();
        frontName = frontHeadFGO.transform.Find("name").GetComponent<Text>();
        frontCityText= frontHeadFGO.transform.Find("city").GetComponent<Text>();
        //=====================头像全部隐藏========================
        backHeadFGO.gameObject.SetActive(false);
        leftHeadFGO.gameObject.SetActive(false);
        rightHeadFGO.gameObject.SetActive(false);
        frontHeadFGO.gameObject.SetActive(false);

        

        if (GameInfo.FW == 1||GameInfo.gameNum>1)
        {
            ShowCards(GameInfo.FW);
        }
    }

    public void ShowCards(int fW)
    {
        if (fW == 0)
        {
            return;
        }
        //分情况，给名字
        int dir = 0;
        if( GameInfo.MJplayers.TryGetValue(fW, out userBack))
        {
            // Debug.Log("daole");
            //  Debug.Log(userBack.Nickname);
           // MeNickNameText.text = userBack.Nickname;
            MeNickNameText.text = userBack.nickname;
            //backName.text = userBack.Nickname;

            backName.text = userBack.nickname;
            GameInfo.MJVoteDic["back"] = fW;
            backIPText.text = GameInfo.userIP;
            backIDText.text = GameInfo.userID + "";
            //StartCoroutine(GetImage(userBack.Headimg, backHeadImage));
            LoadImage.Instance.LoadPicture(userBack.headimg, backHeadImage);
            backHeadFGO.gameObject.SetActive(true);
            // Debug.LogError(MeNickNameText.text);
        }

        if (GameInfo.IsSetRoomInfo)
        {
            if (GameInfo.room_peo != 2)
            {
                dir = fW == 1 ? GameInfo.room_peo : fW - 1;
                if (GameInfo.MJplayers.TryGetValue(dir, out userLeft))
                {
                    leftNickNameText.text = userLeft.nickname;
                    leftName.text = userLeft.nickname;
                    leftCityText.text = userLeft.City.Split(',')[0];
                    LoadImage.Instance.LoadPicture(userLeft.headimg, leftHeadImage);
                    GameInfo.MJVoteDic["left"] = dir;
                    leftHeadFGO.gameObject.SetActive(true);
                    managerGame.ShowUserInfo(userLeft.UserID, userLeft.UserIP, userLeft.user_FW);
                }
                dir = fW == GameInfo.room_peo ? 1 : fW + 1;
                if (GameInfo.MJplayers.TryGetValue(dir, out userRright))
                {
                    rightNickNameText.text = userRright.nickname;
                    rightName.text = userRright.nickname;
                    rightCityText.text = userRright.City.Split(',')[0];
                    LoadImage.Instance.LoadPicture(userRright.headimg, rightHeadImage);

                    GameInfo.MJVoteDic["right"] = dir;
                    rightHeadFGO.gameObject.SetActive(true);
                    managerGame.ShowUserInfo(userRright.UserID, userRright.UserIP, userRright.user_FW);
                }
                if (GameInfo.room_peo == 4)
                {
                    dir = (fW + 2) > GameInfo.room_peo ? (fW + 2) - 4 : (fW + 2);
                    if (GameInfo.MJplayers.TryGetValue(dir, out userFront))
                    {

                        frontNickNameText.text = userFront.nickname;
                        frontName.text = userFront.nickname;
                        frontCityText.text = userFront.City.Split(',')[0];
                        LoadImage.Instance.LoadPicture(userFront.headimg, frontHeadImage);

                        GameInfo.MJVoteDic["front"] = dir;
                        frontHeadFGO.gameObject.SetActive(true);
                        managerGame.ShowUserInfo(userFront.UserID, userFront.UserIP, userFront.user_FW);
                    }
                }
            }
            else
            {
                if (GameInfo.MJplayers.TryGetValue(fW == 2 ? 1 : 2, out userFront))
                {
                    dir = fW == 2 ? 1 : 2;
                    frontNickNameText.text = userFront.nickname;
                    frontName.text = userFront.nickname;
                    frontCityText.text = userFront.City.Split(',')[0];
                    LoadImage.Instance.LoadPicture(userFront.headimg, frontHeadImage);

                    GameInfo.MJVoteDic["front"] = dir;
                    frontHeadFGO.gameObject.SetActive(true);
                    managerGame.ShowUserInfo(userFront.UserID, userFront.UserIP, userFront.user_FW);
                }
            }

            StartCoroutine(WaitAndPrint(2.0F));
            gameObject.GetComponent<FICStartGame>().SeZiQi(0);
            ////GameInfo.IsSetRoomInfo = false;
            //FICPaiDui._instance.PaiDuiInit();
        }
    }
    IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //等待之后执行的动作  
    }

    public bool isCanStart = false;
    private void Update()
    {
        ///获得服务器主动推送的用户信息
        if (GameInfo.returnUserInfo != null)
        {
            
            foreach (var item in GameInfo.returnUserInfo.userinfo)
            {
               // managerGame.ShowUserInfo(item.UserID, item.UserIP, item.UserFW);
                if (GameInfo.MJplayers.ContainsKey(item.user_FW))
                    GameInfo.MJplayers[item.user_FW] = item;
                else
                    GameInfo.MJplayers.Add(item.user_FW, item);
                if (item.openid.Equals(GameInfo.OpenID))//如果是断线重联 需要将自己的位置保持起来
                { GameInfo.FW = item.user_FW;
                    GameInfo.userID = item.UserID;
                    GameInfo.userIP = item.UserIP;
                    GameInfo.NickName = item.nickname;
                        }
                GameInfo.MJplayersWhoQuit[item.openid] = item.user_FW;
            }
            ShowCards(GameInfo.FW);
            GameInfo.returnUserInfo = null;
        }
    }
}
