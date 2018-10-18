using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GPSManager : MonoBehaviour
{
    public static GPSManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
#if UNITY_ANDROID  && !UNITY_EDITOR
        OnStartGPS();
#endif
    }
    public void StartCrt()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GetGPSLatitude();
#endif

#if UNITY_IPHONE && !UNITY_EDITOR
         StartCoroutine(StartGPS());
#endif
    }
    IEnumerator StartGPS()
    {
        //txt.text = "开始获取GPS信息";
        // 检查位置服务是否可用
        if (!Input.location.isEnabledByUser)
        {
            yield break;
        }

        // 查询位置之前先开启位置服务
        //txt.text = "启动位置服务";
        Input.location.Start();

        // 等待服务初始化
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return new WaitForSeconds(1);
        }

        // 连接失败
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            yield break;
        }
        else
        {
            GameInfo.Latitude = Input.location.lastData.longitude.ToString() + "," + Input.location.lastData.latitude.ToString();
        }
        Input.location.Stop();
    }
    public void OnStartGPS()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("OnStartLocation");
    }
    public void GetGPSLatitude()
    {
        Debug.Log("经纬度1：" + GameInfo.Latitude);
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        GameInfo.Latitude = jo.Call<string>("GetInfo");
        if (GameInfo.Latitude.Substring(0, 8) == "location")
        {
            Debug.Log("Gps错误信息：" + GameInfo.Latitude);
            GameInfo.Latitude = "0,0";
        }
        Debug.Log("经纬度2：" + GameInfo.Latitude);
    }
    public void OnStopGPS()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("OnStopLocation");
    }
}

