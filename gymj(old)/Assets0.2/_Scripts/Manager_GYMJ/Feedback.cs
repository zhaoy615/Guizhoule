using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using MJBLL.common;
using System;
using DNL;
using System.Text;
using System.Net;
using System.Threading;
using System.Collections.Generic;

public class Feedback : MonoBehaviour {
    
    public GameObject feedback_Pel;
    private Texture2D tempTextyre;
    private string outLogPath = "";
    private byte[] bts;
    public Thread myThread;
    public InputField inputF;
    public string label = "";
    private float time;

    void Start () {
        feedback_Pel.transform.Find("Send").GetComponent<Button>().onClick.AddListener(delegate {  SendFeedbackInfo(); });
        inputF = feedback_Pel.transform.Find("InputField").GetComponent<InputField>();
        inputF.MoveTextEnd(false);
        outLogPath = Application.persistentDataPath + "/outLog.txt";

        GetComponent<Button>().onClick.AddListener(delegate {
            Screenshot();
        });
    }
    public void ShowBugTexture()
    {
        feedback_Pel.SetActive(true);
        feedback_Pel.transform.Find("InfoImage").GetComponent<Image>().sprite = Sprite.Create(tempTextyre, new Rect(0, 0, tempTextyre.width, tempTextyre.height), Vector2.zero);
        
    }
    public void SendFeedbackInfo()
    {
        if (string.IsNullOrEmpty(inputF.text))
        {
            FICWaringPanel._instance.Show("请输入反馈内容！");
            return;
        }
        time = Time.time;
        label = GameInfo.userID + DateTime.Now.ToFileTimeUtc().ToString();
        //获取反馈标题
        string title = feedback_Pel.transform.Find("Dropdown").GetComponent<Dropdown>().captionText.text;
        if (tempTextyre != null)
        {
            bts = tempTextyre.EncodeToJPG();
            StartCoroutine(Upload(label + "BUGpicture.jpg","img", bts));
        }

        StartCoroutine(Read());

        SendFeedback sendFeedback = new SendFeedback();
        sendFeedback.UserID = GameInfo.userID;
        sendFeedback.Title = title;
        sendFeedback.Content = inputF.text;
        sendFeedback.GameLog = label + "outLog.txt";
        sendFeedback.image = label + "BUGpicture.jpg";
        sendFeedback.openid = GameInfo.OpenID;
        sendFeedback.unionid = GameInfo.unionid;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(sendFeedback);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 9005, body.Length, 0, body);
        GameInfo.cs.Send(data);

        inputF.text = "";
        inputF.text = "";
        if (tempTextyre != null) feedback_Pel.transform.Find("InfoImage").GetComponent<Image>().sprite = null;
        feedback_Pel.transform.Find("Dropdown").GetComponent<Dropdown>().value = 0;
        feedback_Pel.SetActive(false);
    }
    public IEnumerator Read()
    {
        string strReadLine = "";
        if (File.Exists(outLogPath))
        {
            StreamReader srReadFile = new StreamReader(outLogPath);
            while (!srReadFile.EndOfStream)
            {
                strReadLine += srReadFile.ReadLine(); //读取每行数据
                yield return 0;
            }
            srReadFile.Close();
            StartCoroutine(Upload(label + "outLog.txt", "txt", Encoding.UTF8.GetBytes(strReadLine)));
        }
        yield return 0;
    }
    public void Screenshot()
    {
        if ((Time.time - time) > 10 || time == 0)
        {
            if (GameInfo.cs.serverType == ServerType.GameServer)
            {
                StartCoroutine(WWWTexture());
            }
            else
            {
                feedback_Pel.SetActive(true);
            }
        }
        else
        {
            FICWaringPanel._instance.Show("反馈过于频繁，请稍后！");
            return;
        }
        
    }
    IEnumerator WWWTexture()
    {
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        Texture2D tempTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        yield return new WaitForEndOfFrame();
        tempTexture.ReadPixels(rect, 0, 0);
        //tempTexture = ScaleTexture(tempTexture, 1920, 1080);
        tempTexture.Apply();
        tempTextyre = tempTexture;
        ShowBugTexture();
    }
    Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }

    private IEnumerator Upload(string str1, string str2, byte[] bytes)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField(str2, str1);
        wwwf.AddBinaryData(str2, bytes, str1);
        WWW www = new WWW("http://download.gzqyrj.com/api/obj.ashx", wwwf);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            FICWaringPanel._instance.Show("上传错误");
            Debug.Log(www.error);
            OutLog.Log(www.error);
        }
        else
        {
            FICWaringPanel._instance.Show("上传成功");
        }
    }
}
