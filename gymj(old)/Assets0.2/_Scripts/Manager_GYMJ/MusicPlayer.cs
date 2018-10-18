using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{

    public AudioSource Sound;

    public void Play(string str)
    {
        Sound.clip = (AudioClip)Resources.Load(str, typeof(AudioClip));//调用Resources方法加载AudioClip资源
        Sound.Play();
    }

}