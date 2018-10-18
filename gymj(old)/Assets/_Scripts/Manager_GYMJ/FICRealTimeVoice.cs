using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using gcloud_voice;
public class FICRealTimeVoice : MonoBehaviour
{
    private Transform voiceTrans;
    private Toggle micToggle;
    private Toggle speakerToggle;
    private Text txt;
    private IGCloudVoice m_voiceengine = null;
    private string m_roomName ;
    private static bool bIsStart = false;
    private void Start()
    {
        voiceTrans = transform.Find("/Game_UI/Fixed_UI/Voices/");
        micToggle = voiceTrans.transform.Find("Mic").GetComponent<Toggle>();
        speakerToggle = voiceTrans.transform.Find("horn").GetComponent<Toggle>();
        txt = voiceTrans.transform.Find("Text").GetComponent<Text>();
        m_roomName = GameInfo.room_id+"";
        if (m_voiceengine == null)
        {
            txt.text += "开始";
               m_voiceengine = GCloudVoice.GetEngine();
            txt.text += "调用了，设置openid";
            m_voiceengine.SetAppInfo("1981614680", "829b1c3747cd948f6dad1e9b151692ef", GameInfo.OpenID);
            txt.text += "设置openID了，开始init";
            m_voiceengine.Init();
            txt.text += "init结束，设置实时语音";
            m_voiceengine.SetMode(GCloudVoiceMode.RealTime);
            txt.text += "设置实时语音";
        }
        micToggle.onValueChanged.AddListener(
            delegate (bool isOn)
            {
                OnMicToggleClick(isOn);
            }
            );
        speakerToggle.onValueChanged.AddListener(delegate(bool isOn) { OnSpeakerToggleClick(isOn); });
        JoinRoomBtn_Click();

    }
    void Update()
    {
        if (m_voiceengine != null)
        {
            m_voiceengine.Poll();
         
        }
    }
    private void OnMicToggleClick(bool ison)
    {
        if (ison)
        {
            txt.text += "打开麦克风";
            int ret = m_voiceengine.OpenMic();
            txt.text += "打开了麦克风" + ret;
            ret = m_voiceengine.GetMicLevel();

            txt.text += "麦克风声音" + ret;
        }
        else
        {
            txt.text = "关闭麦克风";
            int ret = m_voiceengine.CloseMic();
            txt.text = "关闭了麦克风" + ret;

        }
    }
    private void OnSpeakerToggleClick(bool isOn)
    {
        if (isOn)
        {
            txt.text += "打开音响";
            int ret = m_voiceengine.OpenSpeaker();
            txt.text += "打开了音响" + ret;
            ret = m_voiceengine.GetSpeakerLevel();
            txt.text += "音响声音" + ret;
            ret = m_voiceengine.SetSpeakerVolume(100);

            txt.text += "设置音响声音" + ret;
        }
        else
        {
            txt.text = "关闭音响";
            int ret = m_voiceengine.CloseSpeaker();
            txt.text = "关闭了音响" + ret;
        }
      
    }
    public void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (m_voiceengine == null)
            {
                return;
            }
            m_voiceengine.Pause();
        }
        else
        {
            if (m_voiceengine == null)
            {
                return;
            }
            m_voiceengine.Resume();
        }
    }

    public void JoinRoomBtn_Click()
    {
        txt.text += "加入房间" +m_roomName;
        int ret = m_voiceengine.JoinTeamRoom(m_roomName, 15000);
        txt.text += "加入了房间" +ret;
    }


    private void OnDestroy()
    {
        txt.text = "退出房间";
       int ret= m_voiceengine.QuitRoom(m_roomName, 15000);
        txt.text = "退出了房间" +ret;

    }
    public void QuitRoomBtn_Click()
    {
        m_voiceengine.QuitRoom(m_roomName, 15000);
    }
}
