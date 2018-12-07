using UnityEngine;
using System.Collections;

public class FICPYQCreateRoom : MonoBehaviour
{

	public void OnPYQCreateRoom(GameObject obj)
    {
        var groupid = int.Parse(this.transform.parent.name);
        GameInfo.GroupID = groupid;
       
    }





}
