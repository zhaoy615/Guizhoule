using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.UI;

public class LoadImage : MonoBehaviour
{
    public static LoadImage Instance = null;
    public Sprite error;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void LoadPicture(string url, Image image)
    {
        if (!File.Exists(Application.persistentDataPath + "/picture/" + url.GetHashCode() + ".jpg"))
        {
            StartCoroutine(LoadThumbnail(url, image));
        }
        else
        {
            StartCoroutine(LoadLocalImage(url, image));
        }
    }
    private IEnumerator LoadThumbnail(string url, Image image)
    {
        WWW www = new WWW((new Uri(url)).AbsoluteUri);
        yield return www;

        if (string.IsNullOrEmpty(www.error) && www.isDone && !image.IsDestroyed())
        {
            Debug.Log(www.texture.width + "|" + www.texture.height);
            image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
            //    将图片保存至缓存路径  


            if (www.texture.width != 8 && www.texture.height != 8)
            {
                byte[] pngData = www.texture.EncodeToPNG();

                if (!Directory.Exists(Application.persistentDataPath + "/picture"))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/picture");
                }
                using (FileStream fs = new FileStream(Application.persistentDataPath + "/picture/" + url.GetHashCode() + ".jpg", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Write(pngData, 0, pngData.Length);
                }
            }
        }
        else if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("加载头像出错:" + www.error);
            image.sprite = error;
        }
    }
    private IEnumerator LoadLocalImage(string url, Image image)
    {
        // 已在本地缓存  
        string filePath = "file:///" + Application.persistentDataPath + "/picture/" + url.GetHashCode() + ".jpg";
        WWW www = new WWW(filePath);
        yield return www;
        if (string.IsNullOrEmpty(www.error) && www.isDone && !image.IsDestroyed())
        {
            image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
        }
    }
}