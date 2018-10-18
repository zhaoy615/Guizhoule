using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Manager_Login : MonoBehaviour
{
    public GameObject progressCircle;
    public GameObject loginBtn;
    private AudioSource Sound;
    private Manager_Login music;
    // Use this for initialization
    void Start()
    {
        music = gameObject.GetComponent<Manager_Login>();
        Sound = GameObject.Find("Audio Source").GetComponent<AudioSource>();
    }
    void Update()
    {
        progressCircle.transform.Rotate(Vector3.forward, -4.5f);
    }
    public void ShowLoginButton()
    {
      
        progressCircle.SetActive(false);
        loginBtn.SetActive(true);
        transform.Find("/Canvas/loginBtn").GetComponent<Button>().onClick.AddListener(delegate { btnVoice("btn1"); }); ///登录按钮音效
    }

    public void btnVoice(string type)
    {
        string voiceName = "";
        switch (type)
        {
            case "btn1":
                voiceName = "btn1";
                break;
        }
                music.LoginMusicPlay(voiceName);
                voiceName = null;
        
}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    public void LoginMusicPlay(string str)
    {
        Sound.clip = (AudioClip)Resources.Load("AudioSource/Sound/" + str, typeof(AudioClip));//调用Resources方法加载AudioClip资源
        Sound.loop = false;              //开始设置AudioSource不循环不放
        Sound.Play();
        Sound.volume = PlayerPrefs.GetFloat("soundVoice");///给音效的拖动按钮赋值
    }
}
