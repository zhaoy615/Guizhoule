using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MJBLL.common;
using DNL;
using System.Text;

public class LoadingAlter : MonoBehaviour
{

    private Slider slider;
    private Text progressText;
    AsyncOperation asy;
    private bool isHaveSend = false;
    // Use this for initialization
    void Start()
    {
        progressText = transform.Find("/Canvas/text_progress").GetComponent<Text>();
        slider = transform.Find("/Canvas/Slider").GetComponent<Slider>();
        StartCoroutine(load());
        FICWaringPanel._instance.Hide();


    }

    IEnumerator load()
    {
       asy = SceneManager.LoadSceneAsync(GameInfo.sceneID);
        asy.allowSceneActivation = false;
        yield return asy;
    }
    // Update is called once per frame
    void Update()
    {
        if (slider.value < asy.progress)
        {
            slider.value += 0.01f;
        }
        if (slider.value > 0.88f)
        {
            slider.value += 0.01f;
        }
        if (slider.value > 0.99f)
        {
            if (GameInfo.gameName == "GYMJ"&&!isHaveSend)
            {
                OnEnterRoomLoading();
                isHaveSend = true;
            }
            slider.value = 1;
            asy.allowSceneActivation = true;
        }
        progressText.text = (int)(slider.value * 100) + "%";


        
    }

    /// <summary>
    /// 当进度条完成的时候，发送加入房间信息
    /// </summary>
    void OnEnterRoomLoading()
    {

        //SendAddRoom addRoom = SendAddRoom.CreateBuilder()
        //                       .SetRoomID(GameInfo.room_id)
        //                       .SetOpenid(GameInfo.OpenID)
        //                       .SetLatitude(GameInfo.Latitude)
        //                       .Build();
        //byte[] body = addRoom.ToByteArray();

        SendAddRoom addRoom = new SendAddRoom();
        addRoom.roomID = GameInfo.room_id;
        addRoom.openid = GameInfo.OpenID;
        addRoom.Latitude = GameInfo.Latitude;
        byte[] body = ProtobufUtility.GetByteFromProtoBuf(addRoom);
        byte[] data = CreateHead.CreateMessage(CreateHead.CSXYNUM + 2003, body.Length, 0, body);
        Debug.Log("jiaru" + GameInfo.room_id + ",id:" + GameInfo.OpenID);
        GameInfo.cs.Send(data);

        GameInfo.MJplayers.Clear();//加入房间时，清空字典，以免加入别的房间，数据不对


    }
}
