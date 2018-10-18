using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using DG.Tweening;
using System.Net;
using System.Text;
using System;

public class GameVersion
{
    public string Version;       //版本号
    public int Status;           //状态
    public string url;           //地址
}
public class VersionDetection : MonoBehaviour
{
    
    public enum updateStatus                   //更新状态
    {
        NO,                                    //不用更新
        needUpdated,                           //可以更新   
        forcedUpdated,                         //强制更新
        NotNetWork                             //网络异常
    }
    private WWW DownloadProgress;              
    private WWW getInfoData;
    Manager_Login login;
    string downloadPath;
    updateStatus Status;
    Slider progressBar;
    Text updateInfo;
    Text localVersion;
    Button downloadBtn;
    Button closeBtn;
    string fileName;
    void Awake() {
        progressBar = transform.parent.Find("progressBar").GetComponent<Slider>();
        updateInfo = transform.Find("updateInfo").GetComponent<Text>();
        downloadBtn = transform.Find("downloadBtn").GetComponent<Button>();
        closeBtn = transform.Find("closeBtn").GetComponent<Button>();
        localVersion = GameObject.Find("versionNum").GetComponent<Text>();
        login = GameObject.Find("Main Camera").GetComponent<Manager_Login>();
        Status = updateStatus.NO;
    }
    void Start()
    {
        downloadBtn.onClick.AddListener(delegate {
            StartCoroutine(DownUpdate(downloadPath));
            transform.DOScale(0, 0.2f);
            login.progressCircle.SetActive(false);
        });
        localVersion.text = "V" + Application.version;
        fileName = "/" + DateTime.Now.ToFileTimeUtc().ToString() + "GY_Mahjong.apk";
        Status = DetectionUpdate();
        Invoke("switchStatus",1);
    }
    void Update()
    {
        if (DownloadProgress != null && Status != updateStatus.NO)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = DownloadProgress.progress;

            if (DownloadProgress.isDone)
            {
                Debug.Log(DownloadProgress.text);
                GetGameVersion(fileName);
                InstallAPK(Application.persistentDataPath + fileName);
            }
        }
    }
    /// <summary>
    /// 切换状态
    /// </summary>
    private void switchStatus()
    {
        switch (Status)
        {
            case updateStatus.NO:
                login.ShowLoginButton();
                break;
            case updateStatus.needUpdated:
                GetComponent<DOTweenAnimation>().DOPlay();
                closeBtn.onClick.AddListener(delegate {
                    transform.DOScale(0, 0.2f);
                    login.ShowLoginButton();
                });
                break;
            case updateStatus.forcedUpdated:
                GetComponent<DOTweenAnimation>().DOPlay();
                closeBtn.onClick.AddListener(delegate {
                    Application.Quit();
                });
                break;
            case updateStatus.NotNetWork:
                FICWaringPanel._instance.Show("网络异常！");
                FICWaringPanel._instance.WarnningMethods = delegate { Invoke("switchStatus", 2); };
                Status = DetectionUpdate();
                break;
        }
    }
    /// <summary>
    /// 检测更新
    /// </summary>
    /// <returns></returns>
    private updateStatus DetectionUpdate()
    {
        GameVersion newVersion = ServerVersion();
        //比较本地版本信息与服务器版本信息
        try
        {
            string[] str1 = newVersion.Version.Split('.');
            string[] str2 = Application.version.Split('.');
            for (int i = 0; i < str1.Length; i++)
            {
                if (int.Parse(str1[i]) > int.Parse(str2[i]))
                {
                    downloadPath = newVersion.url;
                    return (updateStatus)newVersion.Status;
                }
            }
            return updateStatus.NO;

        }
        catch (System.Exception)
        {
           return updateStatus.NotNetWork;
        }
    }
    /// <summary>
    /// 从服务器获取最新版本信息
    /// </summary>
    GameVersion ServerVersion()
    {
        GameVersion json = new GameVersion();
        WebClient webC = new WebClient();
        byte[] data = new byte[2014];
        string dataPath = "http://download.gzqyrj.com/Version/api/Values/GetVersion";
        try
        {
            data = webC.DownloadData(dataPath);
        }
        catch (System.Exception)
        {
            return json;
        }
        string dataStr = Encoding.UTF8.GetString(data);
        json = JsonUtility.FromJson<GameVersion>(dataStr) as GameVersion;
        return json;
    }
    /// <summary>
    /// 获取游戏资源
    /// </summary>
    /// <param name="fileName">保存文件名</param>
    private void GetGameVersion(string fileName)
    {
        byte[] DataInfo = DownloadProgress.bytes;
        DownloadProgress = null;
        Debug.Log(DataInfo.Length);
        using (FileStream fs = new FileStream(Application.persistentDataPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            fs.Write(DataInfo, 0, DataInfo.Length);
        }
    }
    /// <summary>
    /// 下载更新
    /// </summary>
    /// <param name="path">下载路径</param>
    /// <returns></returns>
    IEnumerator DownUpdate(string path)
    {
        string urlPath = "http://" + path;
        DownloadProgress = new WWW(urlPath);
        yield return DownloadProgress;
    }
    /// <summary>
    /// 调用安装安装APK
    /// </summary>
    /// <param name="path">APK路径</param>
    public void InstallAPK(string path)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("install",path);
    }
}

