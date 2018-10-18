using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class ReadConfig : MonoBehaviour
{
    private void Start()
    {
#if !UNITY_EDITOR
        LoadConfigFile();
#endif
    }
    void LoadConfigFile()
    {
        
        if (!File.Exists(Application.persistentDataPath + "/Config.xml"))
        {
            TextAsset txt = Resources.Load<TextAsset>("Configs/Config");
            byte[] DataInfo = txt.bytes;
            using (FileStream fs = new FileStream(Application.persistentDataPath + "/Config.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Write(DataInfo, 0, DataInfo.Length);
            }
        }
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.persistentDataPath + "/Config.xml");
        XmlNode ip = xml.SelectSingleNode("data/SeverIP");
        Debug.Log("当前列表服务器IP:" + ip.InnerText);
        OutLog.Log("当前列表服务器IP:" + ip.InnerText);
        GameInfo.listIp = ip.InnerText;
        XmlNode port = xml.SelectSingleNode("data/SeverPort");
        Debug.Log("当前列表服务器prot:" + port.InnerText);
        OutLog.Log("当前列表服务器prot:" + port.InnerText);
        GameInfo.listPort = port.InnerText;
    }
}