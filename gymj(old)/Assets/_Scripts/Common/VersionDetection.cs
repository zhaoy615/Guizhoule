using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using DG.Tweening;
using System.Net;
using System.Text;
using System;
using System.Threading;

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
    Manager_Login login;
    string downloadPath;
    updateStatus Status;
    Slider progressBar;
    Text updateInfo;
    Text localVersion;
    public Text downloadInfo;
    long fileLength;
    string filePath;
    //获取下载文件的总长度
    long totalLength;
    Button downloadBtn;
    Button closeBtn;
    string fileName;
    //下载进度
    public float progress { get; private set; }
    //涉及子线程要注意,Unity关闭的时候子线程不会关闭，所以要有一个标识
    private bool isStop;
    //子线程负责下载，否则会阻塞主线程，Unity界面会卡主
    private Thread thread;
    //表示下载是否完成
    public bool isDone { get; private set; }
    void Awake()
    {
        progressBar = transform.parent.Find("progressBar").GetComponent<Slider>();
        updateInfo = transform.Find("updateInfo").GetComponent<Text>();
        downloadBtn = transform.Find("downloadBtn").GetComponent<Button>();
        closeBtn = transform.Find("closeBtn").GetComponent<Button>();
        localVersion = GameObject.Find("versionNum").GetComponent<Text>();
        login = GameObject.Find("Main Camera").GetComponent<Manager_Login>();
        Status = updateStatus.NO;
        filePath = Application.persistentDataPath;
    }
    void Start()
    {
        downloadBtn.onClick.AddListener(delegate
        {
            progressBar.gameObject.SetActive(true);
            DownLoad("http://" + downloadPath, filePath, delegate { InstallAPK(filePath + fileName); });
            transform.DOScale(0, 0.2f);
            login.progressCircle.SetActive(false);
        });
        localVersion.text = "V" + Application.version;
        //Status = DetectionUpdate();
        //测试，上面才是正规程序
        //	Status = updateStatus.NO;
        Invoke("switchStatus", 1);
    }
    private void Update()
    {
        //实时显示加载进度
        if (progressBar.gameObject.activeSelf)
        {
            downloadInfo.text = ByteToMByte(fileLength) + "/" + ByteToMByte(totalLength);
            progressBar.value = progress;
        }

    }
    private string ByteToMByte(long length)
    {
        float mb = length / 1024f / 1024f;
        if (mb > 0)
        {
            return mb.ToString("0.00") + "MB";
        }
        else
        {
            return (mb * 1024).ToString("0.00") + "KB";
        }

    }
    /// <summary>
    /// 切换状态
    /// </summary>
    private void switchStatus()
    {
        Status = updateStatus.NO;
        switch (Status)
        {
            case updateStatus.NO:
                login.ShowLoginButton();
                break;
            case updateStatus.needUpdated:
                GetComponent<DOTweenAnimation>().DOPlay();
                closeBtn.onClick.AddListener(delegate
                {
                    transform.DOScale(0, 0.2f);
                    login.ShowLoginButton();
                });
                break;
            case updateStatus.forcedUpdated:
                GetComponent<DOTweenAnimation>().DOPlay();
                closeBtn.onClick.AddListener(delegate
                {
                    Application.Quit();
                });
                break;
            case updateStatus.NotNetWork:
                FICWaringPanel._instance.Show("网络异常！");
                FICWaringPanel._instance.WarnningMethods = delegate { Invoke("switchStatus", 2); };
                //Status = DetectionUpdate();
                break;
        }
    }
    //这里判断是否需要更新
    /// <summary>
    /// 检测更新
    /// </summary>
    /// <returns></returns>
    private updateStatus DetectionUpdate()
    {
        GameVersion newVersion = ServerVersion();
        fileName = "/" + newVersion.Version + "GY_Mahjong.apk";
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
        catch (System.Exception e)
        {
            OutLog.log(e.ToString());
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
        catch (System.Exception e)
        {
            OutLog.log("serverVersion error" + e);
            return json;
        }
        string dataStr = Encoding.UTF8.GetString(data);
        json = JsonUtility.FromJson<GameVersion>(dataStr) as GameVersion;

        return json;
    }
    /// <summary>
    /// 调用安装安装APK
    /// </summary>
    /// <param name="path">APK路径</param>
    public void InstallAPK(string path)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("install", path);
    }
    /// <summary>
    /// 下载方法(断点续传)
    /// </summary>
    /// <param name="url">URL下载地址</param>
    /// <param name="savePath">Save path保存路径</param>
    /// <param name="callBack">Call back回调函数</param>
    public void DownLoad(string url, string savePath, Action callBack)
    {
        isStop = false;
        thread = new Thread(delegate ()
        {
            //判断保存路径是否存在
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            //这是要下载的文件名，比如从服务器下载a.zip到D盘，保存的文件名是test
            string filePath = savePath + fileName;

            //使用流操作文件
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            //获取文件现在的长度
            fileLength = fs.Length;
            //获取下载文件的总长度
            totalLength = GetLength(url);

            //如果没下载完
            if (fileLength < totalLength)
            {
                //断点续传核心，设置本地文件流的起始位置
                fs.Seek(fileLength, SeekOrigin.Begin);

                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;

                //断点续传核心，设置远程访问文件流的起始位置
                request.AddRange((int)fileLength);
                Stream stream = request.GetResponse().GetResponseStream();

                byte[] buffer = new byte[1024];
                //使用流读取内容到buffer中
                //注意方法返回值代表读取的实际长度,并不是buffer有多大，stream就会读进去多少
                int length = stream.Read(buffer, 0, buffer.Length);
                Debug.Log(stream.ReadTimeout);
                while (length > 0)
                {
                    //如果Unity客户端关闭，停止下载
                    if (isStop) break;
                    //将内容再写入本地文件中
                    fs.Write(buffer, 0, length);
                    //计算进度
                    fileLength += length;
                    progress = (float)fileLength / (float)totalLength;

                    //类似尾递归
                    length = stream.Read(buffer, 0, buffer.Length);
                }
                Debug.Log(length);
                stream.Close();
                stream.Dispose();

            }
            else
            {
                progress = 1;
            }
            fs.Close();
            fs.Dispose();
            //如果下载完毕，执行回调
            if (progress == 1)
            {
                isDone = true;
                if (callBack != null) callBack();
            }
        });
        thread.IsBackground = true;
        thread.Start();
    }
    /// <summary>
    /// 获取下载文件的大小
    /// </summary>
    /// <returns>The length.</returns>
    /// <param name="url">URL.</param>
    long GetLength(string url)
    {
        HttpWebRequest requet = HttpWebRequest.Create(url) as HttpWebRequest;
        requet.Method = "HEAD";
        HttpWebResponse response = requet.GetResponse() as HttpWebResponse;
        return response.ContentLength;
    }
    private void OnDestroy()
    {
        isStop = true;
    }
}

