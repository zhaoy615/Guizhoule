using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;

public class GPSManager : MonoBehaviour
{
    public static GPSManager instance;

    string url = "http://api.map.baidu.com/location/ip?ak=bretF4dm6W5gqjQAXuvP0NXW6FeesRXb&coor=bd09ll";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {

    }
    public void StartCrt()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
     StartCoroutine(StartGPS());

#endif

#if UNITY_IPHONE && !UNITY_EDITOR
       
#endif
    }
    IEnumerator StartGPS()
    {
        WWW www = new WWW(url);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
         
            ResponseBody req = JsonConvert.DeserializeObject<ResponseBody>(www.text);

            GameInfo.province = req.content.address_detail.province;
            GameInfo.city = req.content.address_detail.city;
            GameInfo.Latitude = req.content.point.x + "," + req.content.point.y;
            Debug.Log("gps获取到值 : " + GameInfo.province + "  " + GameInfo.city);

        }
        else
        {
            Debug.Log(" [贵阳麻将] :无法获取gps数据");
        }
    }

}


public class ResponseBody
{

    public string address;
    public Content content;
    public int status;
}

public class Content
{
    public string address;
    public Address_Detail address_detail;
    public Point point;
}
public class Address_Detail
{
    public string city;
    public int city_code;
    public string district;
    public string province;
    public string street;
    public string street_number;
    public Address_Detail(string city, int city_code, string district, string province, string street, string street_number)
    {
        this.city = city;
        this.city_code = city_code;
        this.district = district;
        this.province = province;
        this.street = street;
        this.street_number = street_number;
    }
}
public class Point
{
    public string x;
    public string y;
    public Point(string x, string y)
    {
        this.x = x;
        this.y = y;
    }
}

