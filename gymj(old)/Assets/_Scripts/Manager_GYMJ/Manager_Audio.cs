using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Script_me;
using UnityEngine.UI;
using MJBLL.common;
using DNL;
using System.Text;

public class Manager_Audio : MonoBehaviour
{
    [HideInInspector]
    public Manager_Game music;   ///游戏界面播放对象
    [SerializeField]
    AudioClip _bgMusic;          //背景音乐
    private AudioClip[] _Sound = new AudioClip[10];  //按钮音效
    private AudioSource audioMusic; //用于控制音乐的AudioSource组件  
    private AudioSource audioSound; //用于控制音效的AudioSource组件  
    public Slider _ConMusic;//音乐大小调节按钮
    public Slider _ConSound;//音效大小调节
    private GameObject _Pathset;//申明设置窗体加载路径
    private GameObject imgVoice;//游戏界面快捷语音界面
    public float quickTime;

    //=================快捷语音按钮======================//

    private Button shortVoiceButton;
    private Image shortVoiceMask;
    private GameObject shortVoicePanel;
    private bool coolDown = false;

    private Button quickVoice1;
    private Button quickVoice2;
    private Button quickVoice3;
    private Button quickVoice4;
    private Button quickVoice5;
    //private Button quickVoice6;
    //private Button quickVoice7;
    //private Button quickVoice8;
    //private Button quickVoice9;
    //private Button quickVoice10;

	/// <summary>
	/// 脚本对象实例化时被调用
	/// </summary>
	void Awake()
	{ 

		transform.Find("/Game_UI/PopUp_UI/SheZhi/Music").GetComponent<Slider>().onValueChanged.AddListener(delegate { MusicClick(); });       //调用调节音乐大小的方法
		transform.Find("/Game_UI/PopUp_UI/SheZhi/Sound").GetComponent<Slider>().onValueChanged.AddListener(delegate { SoundClick(); });      //调用调节音效大小的方法
		//背景音乐播放器
		audioMusic = this.gameObject.AddComponent<AudioSource>();  //在添加此脚本的对象中添加AudioSource组件，此处为摄像机  
		//背景音效播放器
		audioSound = this.gameObject.AddComponent<AudioSource>();
		audioMusic.loop = true;     //设置循环播放  
		audioMusic.clip = _bgMusic;  //设置audioClip  
	}


    /// <summary>
    /// 对象的第一帧时被调用
    /// </summary>
    void Start()
    {
		audioMusic.Play();  //游戏开始播放背景音乐

        //======================保存游戏中音量=======================//
        _ConMusic.value = PlayerPrefs.GetFloat("musicVoice");
        _ConSound.value = PlayerPrefs.GetFloat("soundVoice");

        //======================快捷语音按钮====================================//
        quickVoice1 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_1").GetComponent<Button>();
        quickVoice2 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_2").GetComponent<Button>();
        quickVoice3 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_3").GetComponent<Button>();
        quickVoice4 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_4").GetComponent<Button>();
        quickVoice5 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_5").GetComponent<Button>();
		//弹出快捷语音按钮的按钮
        shortVoiceButton = transform.Find("/Game_UI/Fixed_UI/Voice_Dd").GetComponent<Button>();
		shortVoiceButton.onClick.AddListener(OnShortVoiceButtonClick);
		//弹出快捷语音按钮的按钮的遮罩
        shortVoiceMask = shortVoiceButton.transform.Find("mask").GetComponent<Image>();
		//快捷语音按钮的父物体
        shortVoicePanel = transform.Find("/Game_UI/PopUp_UI/Voice").gameObject;
		//快捷语音的父物体
		imgVoice = transform.Find("/Game_UI/PopUp_UI/Voice").gameObject; //快捷语音界面赋值
		//给快捷语音按钮绑定事件
		delayQuickVoice();

		//----------------------------没什么用
		quickTime = 0;
		music = gameObject.GetComponent<Manager_Game>();        //获取播放音源的对象
    }
	/// <summary>
	/// 调节游戏背景音乐
	/// </summary>
	public void MusicClick()
	{
		audioMusic.volume = _ConMusic.value;
		///保存游戏音量
		PlayerPrefs.SetFloat("musicVoice", audioMusic.volume);
	}

	/// <summary>
	/// 调节游戏音效
	/// </summary>
	public void SoundClick()
	{
		audioSound.volume = _ConSound.value;
		///保存游戏音量
		PlayerPrefs.SetFloat("soundVoice", audioSound.volume);
	}

	//弹出快捷语音按钮的选择框
    private void OnShortVoiceButtonClick()
    {
        shortVoicePanel.SetActive(true);
        shortVoiceMask.gameObject.SetActive(true);
        shortVoiceMask.fillAmount = 1;
        shortVoiceButton.enabled = false;
        coolDown = true;
        Invoke("CloseShortVoicePanel", 3);
    }
	//关闭快捷语音按钮的选择框
	private void CloseShortVoicePanel()
	{
		shortVoicePanel.gameObject.SetActive(false);
	}
	//快捷语音按钮方法
	public void delayQuickVoice()
	{
		//======================  发送快捷语音按钮事件 ======================================//
		quickVoice1.onClick.AddListener(delegate { SendQuickVoice(81); imgVoice.SetActive(false); });
		quickVoice2.onClick.AddListener(delegate { SendQuickVoice(82); imgVoice.SetActive(false); });
		quickVoice3.onClick.AddListener(delegate { SendQuickVoice(83); imgVoice.SetActive(false); });
		quickVoice4.onClick.AddListener(delegate { SendQuickVoice(84); imgVoice.SetActive(false); });
		quickVoice5.onClick.AddListener(delegate { SendQuickVoice(85); imgVoice.SetActive(false); });
	}

    private void Update()
    {
		//计时，当shortVoiceMask显示的时候，随着时间渐渐不显示(转圈消失)，当完全消失的时候，shortVoiceButton重新启用。
        if (coolDown)
        {
            shortVoiceMask.fillAmount -= 0.2f * Time.deltaTime;
            if (shortVoiceMask.fillAmount<=0)
            {
                shortVoiceMask.gameObject.SetActive(false);
                shortVoiceMask.fillAmount = 1;
                shortVoiceButton.enabled = true;

                coolDown = false;
            }
        }
    }

    /// <summary>
    /// 所有需要播放的音效
    /// </summary>
    //void nextVoice(string str)
    //{
    //    index++;
    //    index = index % Clips.Length;
    //    music.Sound.clip = Clips[index];
    //    music.Sound.Play();
    //}

    /// <summary>
    /// 发送播放选择的快捷语音
    /// </summary>
    public void SendQuickVoice(int voiceNum)
    {
		//？？把要播放的语音上传到服务器
        SendVoice sencGameOperation = new SendVoice();
        sencGameOperation.openid = GameInfo.OpenID;
        sencGameOperation.RoomID = GameInfo.room_id;
        sencGameOperation.VoiceNumber = voiceNum;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sencGameOperation);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 8002, body.Length, 0, body);
        Debug.Log(data);
        GameInfo.cs.Send(data);
        GameInfo.isScoketClose = true;
    }

}
