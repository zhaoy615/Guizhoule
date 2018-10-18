using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Script_me;
using UnityEngine.UI;

public class Manager_HallAudio : MonoBehaviour
{
    private Manager_Hall music;///游戏界面播放对象
    [SerializeField]
    AudioClip _bgMusic;///背景音乐
    private AudioClip[] _Sound = new AudioClip[10];///按钮音效
    private AudioSource _audioMusic;///用于控制音乐的AudioSource组件  
    private AudioSource _audioSound;///用于控制音效的AudioSource组件  
    public Slider _ConMusic;///音乐大小调节按钮
    public Slider _ConSound;///音效大小调节
    private GameObject _Pathset;///申明设置窗体加载路径

    void Start()
    {
        //=================保存游戏中音量=======================//
            _ConMusic.value = PlayerPrefs.GetFloat("musicVoice",1);
            _ConSound.value = PlayerPrefs.GetFloat("soundVoice",1);
        
        _audioMusic.Play();//游戏开始播放背景音乐
        music = GameObject.Find("Main Camera").GetComponent<Manager_Hall>();//获取播放音源的对象
    }

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        transform.Find("/Game_UI/PopUp_UI/SheZhi/Music").GetComponent<Slider>().onValueChanged.AddListener(delegate { MusicClick(); });//调用调节音乐大小的方法
        GameObject.Find("/Game_UI/PopUp_UI/SheZhi/Sound").GetComponent<Slider>().onValueChanged.AddListener(delegate { SoundClick(); });//调用调节音效大小的方法
        _audioMusic = this.gameObject.AddComponent<AudioSource>();//在添加此脚本的对象中添加AudioSource组件，此处为摄像机  
        _audioSound = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        _audioMusic.loop = true;//设置循环播放  
        _audioMusic.volume = 1.0f;//设置音量为最大，区间在0-1之间  
        _audioSound.volume = 1.0f;
        _audioMusic.clip = _bgMusic;//设置audioClip  
    }
    #region 旧代码
    // void Update()
    //{
    //    if (_ConMusic.fillAmount>0||_ConMusic.fillAmount <1)
    //    {
    //        MusicClick();
    //    }   
    //    else if (_ConSound.fillAmount >0||_ConSound.fillAmount < 1)
    //    {
    //        SoundClick();
    //    }
    //}

    //void OnGUI()
    //{
    //音量控制Label  

    //GUI.Label(new Rect(10, 130, 100, 30), "音量大小调节");
    //_audioSource.volume = GUI.HorizontalSlider(new Rect(120, 130, 100, 20), _audioSource.volume, 0.0f, 1.0f);



    // Debug.LogError(_audioSource.volume); 
    //}
    #endregion

    /// <summary>
    /// 调节游戏背景音乐音量
    /// </summary>
    public void MusicClick()
    {
        _audioMusic.volume = _ConMusic.value;
        PlayerPrefs.SetFloat("musicVoice", _audioMusic.volume); ///保存游戏音量
    }
    /// <summary>
	/// 调节游戏音效音量
    /// </summary>
    public void SoundClick()
    {
        _audioSound.volume = _ConSound.value;
        PlayerPrefs.SetFloat("soundVoice", _audioSound.volume); ///保存游戏音量
    }

    /// <summary>
    /// 控制大厅按钮音效
    /// </summary>
    /// <param name="type"></param>
    public void AudioClick(string type)
    {
        string audioName = "";

        switch (type)
        {
            case "btn1":
                audioName = "btn1";
                break;
            case "btn2":
                audioName = "btn2";
                break;
        }
        music.HallMusicPlay(audioName); ///游戏界面场景播放音乐

    }

}
#region 旧代码
//------------------------------//
/// <summary>
/// 单例模式
/// </summary>
//public static readonly Manager_Audio instance = new Manager_Audio();
//private void SoundManager()
//{

//}

//------------------------------//


/// <summary>
/// 将声音放入字典中，方便管理
/// </summary>
//private Dictionary<string, AudioClip> SoundDictionary;
//private AudioSource[] audioSources;

//private AudioSource bgAudioSource;
//private AudioSource audioSourceEffect;

//void Awake()
//{
//    //instance = this;

//    SoundDictionary = new Dictionary<string, AudioClip>();

//    //本地加载 
//    AudioClip[] audioArray = Resources.LoadAll<AudioClip>("AudioCilp");

//    audioSources = GetComponents<AudioSource>();
//    bgAudioSource = audioSources[0];
//    audioSourceEffect = audioSources[1];

//    //存放到字典
//    foreach (AudioClip item in audioArray)
//    {
//        SoundDictionary.Add(item.name, item);
//    }
//}

////播放背景音乐
//public void PlayBGaudio(string audioName)
//{
//    if (SoundDictionary.ContainsKey(audioName))
//    {
//        bgAudioSource.clip = SoundDictionary[audioName];
//        bgAudioSource.Play();
//    }
//}
////播放音效
//public void PlayAudioEffect(string audioEffectName)
//{
//    if (SoundDictionary.ContainsKey(audioEffectName))
//    {
//        audioSourceEffect.clip = SoundDictionary[audioEffectName];
//        audioSourceEffect.Play();
//    }
//}
//}
# endregion
