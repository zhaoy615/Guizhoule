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
    private AudioSource _audioMusic; //用于控制音乐的AudioSource组件  
    private AudioSource _audioSound; //用于控制音效的AudioSource组件  
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
    /// 对象的第一帧时被调用
    /// </summary>
    void Start()
    {
        quickTime = 0;
        _audioMusic.Play();  //游戏开始播放背景音乐
        music = gameObject.GetComponent<Manager_Game>();        //获取播放音源的对象
        imgVoice = transform.Find("/Game_UI/PopUp_UI/Voice").gameObject; //快捷语音界面赋值

        //======================保存游戏中音量=======================//
        _ConMusic.value = PlayerPrefs.GetFloat("musicVoice");
        _ConSound.value = PlayerPrefs.GetFloat("soundVoice");

        //======================快捷语音按钮====================================//
        quickVoice1 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_1").GetComponent<Button>();
        quickVoice2 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_2").GetComponent<Button>();
        quickVoice3 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_3").GetComponent<Button>();
        quickVoice4 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_4").GetComponent<Button>();
        quickVoice5 = transform.Find("/Game_UI/PopUp_UI/Voice/info/quickVoice_5").GetComponent<Button>();

        shortVoiceButton = transform.Find("/Game_UI/Fixed_UI/Voice_Dd").GetComponent<Button>();
        shortVoiceMask = shortVoiceButton.transform.Find("mask").GetComponent<Image>();
        shortVoicePanel = transform.Find("/Game_UI/PopUp_UI/Voice").gameObject;

        shortVoiceButton.onClick.AddListener(OnShortVoiceButtonClick);
        delayQuickVoice();
    }
    private void OnShortVoiceButtonClick()
    {
        shortVoicePanel.SetActive(true);
        shortVoiceMask.gameObject.SetActive(true);
        shortVoiceMask.fillAmount = 1;
        shortVoiceButton.enabled = false;
        coolDown = true;
        Invoke("CloseShortVoicePanel", 3);
    }

    private void Update()
    {
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
    private void CloseShortVoicePanel()
    {
        shortVoicePanel.gameObject.SetActive(false);
    }

    public void delayQuickVoice()
    {
        //======================  发送快捷语音按钮事件 ======================================//
        quickVoice1.onClick.AddListener(delegate { SendQuickVoice(81); imgVoice.SetActive(false); });
        quickVoice2.onClick.AddListener(delegate { SendQuickVoice(82); imgVoice.SetActive(false); });
        quickVoice3.onClick.AddListener(delegate { SendQuickVoice(83); imgVoice.SetActive(false); });
        quickVoice4.onClick.AddListener(delegate { SendQuickVoice(84); imgVoice.SetActive(false); });
        quickVoice5.onClick.AddListener(delegate { SendQuickVoice(85); imgVoice.SetActive(false); });
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
    /// 脚本对象实例化时被调用
    /// </summary>
    void Awake()
    {
        transform.Find("/Game_UI/PopUp_UI/SheZhi/Music").GetComponent<Slider>().onValueChanged.AddListener(delegate { MusicClick(); });       //调用调节音乐大小的方法
        transform.Find("/Game_UI/PopUp_UI/SheZhi/Sound").GetComponent<Slider>().onValueChanged.AddListener(delegate { SoundClick(); });      //调用调节音效大小的方法
        _audioMusic = this.gameObject.AddComponent<AudioSource>();  //在添加此脚本的对象中添加AudioSource组件，此处为摄像机  
        _audioSound = this.gameObject.AddComponent<AudioSource>();
        _audioMusic.loop = true;     //设置循环播放  
        _audioMusic.clip = _bgMusic;  //设置audioClip  
    }

    /// <summary>
    /// 调节游戏背景音乐
    /// </summary>
    public void MusicClick()
    {
        _audioMusic.volume = _ConMusic.value;
        ///保存游戏音量
        PlayerPrefs.SetFloat("musicVoice", _audioMusic.volume);
    }

    /// <summary>
    /// 调节游戏音效
    /// </summary>
    public void SoundClick()
    {
        _audioSound.volume = _ConSound.value;
        ///保存游戏音量
        PlayerPrefs.SetFloat("soundVoice", _audioSound.volume);
    }
    

    /// <summary>
    /// 发送播放选择的快捷语音
    /// </summary>
    public void SendQuickVoice(int voiceNum)
    {

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
