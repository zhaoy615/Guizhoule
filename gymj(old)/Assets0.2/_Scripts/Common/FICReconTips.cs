using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public enum PingState
{
    PingIng,
    PingOK,
    CanNotConnectServer
}
class FICReconTips : MonoBehaviour
{
    public static FICReconTips _instance;
    private GameObject tipGo;
    private Text tipText;
    private Image circleImage;
    private Transform circleTrans;
    private bool isRoating;
    private Vector3 rotaVec;
    private PingState m_PingResServerState;
    private void Awake()
    {
        _instance = this;
        tipGo = transform.Find("tipsGO").gameObject;
           tipText = transform.Find("tipsGO/Text").GetComponent<Text>();
        circleImage = transform.Find("tipsGO/circle").GetComponent<Image>();
        circleTrans = circleImage.transform;
        isRoating = true;
        rotaVec = new Vector3(0, 0, 1);
        InvokeRepeating("CheckResServerNetWorkReady", 5, 1);
    }
    private void Update()
    {
        if (isRoating)
        {
            circleTrans.Rotate(rotaVec, -90 * Time.deltaTime);
        }
    }
    public void ShowTips(string str)
    {
        tipGo.SetActive(true);
        tipText.text = str;
    }
    public void ShowTips()
    {
        tipGo.SetActive(true);
        tipText.text = "网络不好，正在重连...";
    }
   
    public void HideTips()
    {
        tipGo.SetActive(false);
    }
    private string GetCurrentNormalIP()
    {
        switch (GameInfo.cs.serverType)
        {
            case ServerType.ListServer:
                return GameInfo.listIp;
            case ServerType.GameServer:
                return GameInfo.ip;
            default:
                return null;
        }
    }
    private void CheckResServerNetWorkReady()
    {
        StopCoroutine(PingConnect());
        StartCoroutine(PingConnect());
    }

    IEnumerator PingConnect()
    {
        m_PingResServerState = PingState.PingIng;
        //ResServer IP 
        string ResServerIP = GetCurrentNormalIP();
        if (ResServerIP!=null)
        { //Ping網站 
            Ping ping = new Ping(ResServerIP);

            int nTime = 0;

            while (!ping.isDone)
            {
                yield return new WaitForSeconds(0.1f);

                if (nTime > 50) // time 2 sec, OverTime 
                {
                    nTime = 0;
                    ShowTips();
                    m_PingResServerState = PingState.CanNotConnectServer;
                    yield break;
                }
                nTime++;
            }
            if (ping.isDone)
            {
                yield return ping.time;
                m_PingResServerState = PingState.PingOK;
                HideTips();
            }
        }
       
    }
}

