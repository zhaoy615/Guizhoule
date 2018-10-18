using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


/*1000 区分语音类型   110区分语音片段   1区分随机播放选项有几个
 * 普通话女 1000
 * 贵阳话女2000
 * 普通话男3000
 *贵阳话男4000
 * 010-090 筒  110-190条 210-290万
 * 
 * 鲜花101-？01你这个牌打的真的好
 * 鸡蛋120- 21你棉毛裤是不是，打快点 22快点出牌嘛
 * 手140-141必须点个赞
 * 快捷框180-81菩萨菩萨来个卡阿卡 82想哭都哭不出来 83 打张牌给我碰嘛 84自摸自摸钱多多 85我这牌撞到鬼了
 * 
 * 
 */
/// <summary>
/// 是普通话还是方言
/// </summary>
public enum VoidType
{
    putonghua,
    fangyan
}

public class FICAudioPlay : MonoBehaviour
{
    public static FICAudioPlay _instance;

    /// <summary>
    /// 播放器的音量
    /// </summary>
    private float audioVolum = 1.0f;
    /// <summary>
    /// 是方言还是普通话
    /// </summary>
    public VoidType yuyanType = VoidType.fangyan;
    /// <summary>
    /// 将所有的语音播放片段根据名字加入到字典
    /// </summary>
    private Dictionary<string, AudioClip> audioDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> shortCutVoiceDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> yinxiaoDic = new Dictionary<string, AudioClip>();
    /// <summary>
    /// 每种语音有几种选择？一筒，大饼，月饼 的选择有几种1-29  
    /// </summary>
    private int[] clipChoices = new int[] { 0, 3, 3, 3, 3, 3, 3, 2, 3, 4, 0, 3, 3, 3, 3, 2, 2, 2, 3, 3, 0, 3, 3, 2, 3, 2, 2, 2, 3, 2 };
    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        InitSound();
    }
    private void InitSound()
    {
		//获取牌名与动作语音资源
        AudioClip[] audioClipS = Resources.LoadAll<AudioClip>("AudioSource/Sound");
		//获取玩家间对话语音资源
        AudioClip[] voiceClips = Resources.LoadAll<AudioClip>("AudioSource/Voice");
		//
        AudioClip[] yinxiaoClips = Resources.LoadAll<AudioClip>("AudioSource/Music");
        foreach (AudioClip item in audioClipS)
        {
            audioDic[item.name] = item;
        }
        foreach (AudioClip item in voiceClips)
        {
            shortCutVoiceDic[item.name] = item;
        }
        foreach (AudioClip item in yinxiaoClips)
        {
            yinxiaoDic[item.name] = item;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sex">声音源的性别，自己或者别人</param>
    /// <param name="soundName"></param>
    /// <param name="voiceType">是方言还是普通话</param>
    /// <param name="hs"></param>
    public void PlaySound(int fw, string soundName = null, int hs = 0)
    {
        int sex = GameInfo.MJplayers[fw].sex;
        if (GameInfo.fangyan)
        {
            yuyanType = VoidType.fangyan;
        }
        else
        {
            yuyanType = VoidType.putonghua;
        }
        string clipName = GetAudioClip(sex, yuyanType, soundName, hs);
        Play(clipName);
    }
    /// <summary>
    /// 鲜花，手，鸡蛋
    /// </summary>
    /// <param name="sex"></param>
    /// <param name="buttonNum"></param>
    public void PlayShorVoice(int fsex, int buttonNum)
    {
        int sex = fsex;
        int voiceClip = sex == 1 ? 1 : 2;
        int voiceNum = 0;
        switch (buttonNum)
        {
            case 1:
                voiceNum = 1;
                break;
            case 2:
                voiceNum = UnityEngine.Random.Range(21, 23);
                break;
            case 3:
                voiceNum = 41;
                break;
        }
        if (buttonNum!=1&&buttonNum!=2&&buttonNum!=3)
        {
            voiceNum = buttonNum;
        }
        voiceClip = voiceClip * 100 + voiceNum;
        Play(voiceClip);
    }
    #region 私有方法
    /// <summary>
    /// 根据游戏信息获取语音片段名称
    /// </summary>
    /// <param name="sex">声音源的性别，自己或者别人</param>
    /// <param name="fangyantype">是方言还是普通话</param>
    /// <param name="soundName"></param>
    /// <param name="hs"></param>
    /// <returns></returns>
    private string GetAudioClip(int sex, VoidType yuyanType, string soundName, int hs = 0)
    {
        string clipName = null;
        int clipID = 0;
        int randeID = 0;
        if (hs != 0)
        {
            clipID = sex == 1 ? (yuyanType == VoidType.fangyan ? 4000 : 3000) : (yuyanType == VoidType.fangyan ? 2000 : 1000);
            clipID += hs * 10;
            randeID = UnityEngine.Random.Range(1, clipChoices[hs]);
            clipID += randeID;
            clipName = clipID.ToString();
        }
        else
        {
            clipID = sex == 1 ? (yuyanType == VoidType.fangyan ? 4 : 3) : (yuyanType == VoidType.fangyan ? 2 : 1);

            clipName = clipID + soundName;
        }
        return clipName;
    }

	//牌名与动作播放方法
    private void Play(string clipName)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.volume = PlayerPrefs.GetFloat("soundVoice");
        audioSource.clip = audioDic[clipName];
        audioSource.Play();
        Destroy(audioSource, audioSource.clip.length);
    }
	//玩家间对话语音播放方法
    private void Play(int voiceID)
    {
        string voiceName = voiceID.ToString();
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("soundVoice");
        audioSource.clip = shortCutVoiceDic[voiceName];
        audioSource.Play();
        Destroy(audioSource, audioSource.clip.length);
    }
	//按键音效播放方法
    public void ButtonPlay()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("soundVoice");
        audioSource.clip = yinxiaoDic["btn1"];
        audioSource.Play();
        Destroy(audioSource, audioSource.clip.length);
    }
    
    #endregion
}