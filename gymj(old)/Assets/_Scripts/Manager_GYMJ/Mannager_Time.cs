using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mannager_Time : MonoBehaviour
{
    public Text time_15; //自动取消解散房间倒计时
    int time15 = 15;
    public Text time_60; //自动同意解散房间到计时
    int SZQ60 = 60;
    public Text SZQtime_60; //等待戳拍倒计时
    int SQZ60 = 60;
    public Sprite[] countDownArray;
    public Text countDownText;
    public bool isShimiao = false;
    public bool isFawanpai = false;
    private Text JScountDownText;

    public float szqTime = 10;
    public float JSagreeTime = 10;
    private float szqTimer = 0;
    private bool szqDown = false;
    bool timeIsrun = false;
    bool time60Isrun = false;
    bool SZQ60Isrun = false;
    private FICStartGame startGame;

    void Start()
    {
        startGame = gameObject.GetComponent<FICStartGame>();
        countDownText = transform.Find("/Game_UI/Interaction_UI/desktop_UI/countDown").GetComponent<Text>();//骰子器倒计时
        JScountDownText = transform.Find("/Game_UI/PopUp_UI/SQ_jiesan/title/time_60").GetComponent<Text>();///解散房间默认同意倒计时
    }
    #region
    //void CountDown15()
    //{
    //    timeIsrun = true;
    //    if (time15 <= 0)
    //    {
    //        CloImgrun();
    //    }
    //    time_15.text = (time15).ToString();
    //    time15--;

    //}

    //void CountDown60()
    //{
    //    if (SZQ60 <= 0)
    //    {
    //        GameObject.Find("Main Camera").GetComponent<FICjiesan>().SendAgreeMessage();
    //    }
    //    time_60.text = (SZQ60).ToString();
    //    SZQ60--;
    //}


    //void SZQCountDown60()
    //{
    //    if (SQZ60 <= 0)
    //    {

    //    }
    //    SZQtime_60.text = (SZQ60).ToString();
    //    SZQ60--;
    //}

    //倒计时15秒后自动取消申请
    //public void CloImgrun()
    //{
    //    if (GameObject.Find("Image_run") != null)
    //        GameObject.Find("Image_run").SetActive(false);
    //    CancelInvoke("CountDown15");
    //    timeIsrun = false;
    //}

    //倒计时60秒后自动同意解散房间
    //public void CloImSQ_JS()
    //{
    //    //  GameObject.Find("SQ_jiesan").SetActive(false);
    //    CancelInvoke("CountDown60");
    //    timeIsrun = false;
    //}

    //public void Isjiesan()
    //{
    //    if (!timeIsrun)
    //    {
    //        //StartCoroutine(Count_15());
    //        //time15 = 15;
    //        //time_15.text = time15.ToString();
    //        time15 = 15;
    //        InvokeRepeating("CountDown15", 0, 1);
    //    }
    //}

    //public void AgreeJS()
    //{
    //    if (!time60Isrun)
    //    {
    //        SZQ60 = 60;
    //        InvokeRepeating("CountDown60", 0, 1);
    //    }
    //}

    /// <summary>
    /// 
    /// </summary>
	/// 未使用
    public void seziqi_Time()
    {
        if (!SZQ60Isrun)
        {
            SZQ60 = 60;
            InvokeRepeating("SZQCountDown60", 0, 1);
        }
    }
    #endregion

	private void FixedUpdate()
	{
		//计时器贴图没去找，现在注释掉，不显示
		if (szqDown)
		{
			//骰子时间每帧减少0.02秒
			szqTime -= Time.deltaTime;
		}
		if (szqTime > 0)
		{
			ShowSZQCountImage(szqTime, GameInfo.nowFW);
		}

		if (szqDown)
		{
			JSagreeTime -= Time.deltaTime;
		}
		if (JSagreeTime > 0)
		{
			ShowJSCountImage(JSagreeTime);
		}
	}

	//设置骰szqTime10，显示计秒数的UI
    public void ShowTenDaojiShi()
    {
        if (isShimiao==true&&isFawanpai==true)
        {
            szqTime = 10f;
            countDownText.gameObject.SetActive(true);
        }
    }
	//
    private void ShowSZQCountImage(float szqTime, int fw)
    {
		//变化显示骰子的文本，使其和szqTime一致。
        countDownText.text = ((int)szqTime).ToString();
		//一个int类型的数值代表庄是谁
        int zhuang = GameInfo.Rfw(GameInfo.zhuang);

        ///判断当前玩家最后三秒没有出牌警告
        if ((int)szqTime <= 3 /*&& GameInfo.returnHyUser != null*/)
        {
			//根据庄家来切换网格物体材质的图片
            switch (GameInfo.Rfw(fw))
            {
                case 1://东
                    GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_E").GetComponent<Renderer>().material.mainTexture = startGame.touziqiTexture[zhuang == 1 ? 4 : 4];
                    break;
                case 2://南
                    GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_S").GetComponent<Renderer>().material.mainTexture = startGame.touziqiTexture[zhuang == 2 ? 4 : 4];
                    break;
                case 3://西
                    GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_W").GetComponent<Renderer>().material.mainTexture = startGame.touziqiTexture[zhuang == 3 ? 4 : 4];
                    break;
                case 4://北
                    GameObject.Find("/Game_Prefabs/TABLE/touziqi/touziqi_N").GetComponent<Renderer>().material.mainTexture = startGame.touziqiTexture[zhuang == 4 ? 4 : 4];
                    break;
            }
        }
    }
	/// <summary>
	/// 
	/// </summary>
	/// <param name="szqTime"></param>
	/// 解散房间的时间
	private void ShowJSCountImage(float szqTime)
	{
		JScountDownText.text = (int)szqTime + "";
	}
	//重置骰子时间
    public void ResetSZQDown()
    {
        szqTime = 10f;
        szqDown = true;
    }
    public void ResetShimiao()
    {
        isFawanpai = false;
    }
	//重置解散时间	
    public void ResetJSDown()
    {
        JSagreeTime = 10f;
        szqDown = true;
    }
 
}
