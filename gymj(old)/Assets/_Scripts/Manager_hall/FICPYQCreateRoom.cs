using UnityEngine;
using System.Collections;

public class FICPYQCreateRoom : MonoBehaviour
{

	public void OnPYQCreateRoom(GameObject obj)
    {
        obj.SetActive(true);
        var createRoom = obj.GetComponent<FICCreatRoom>();
        createRoom.OnCreatRoomClick();
    }





}
