using UnityEngine;
using System.Collections;

public class DeskSkillDestory : MonoBehaviour {

    private float time = 2.3f;
    private float huTime = 1.5f;
	// Use this for initialization
	void Start () {

        if (name== "item_zimonew(Clone)"|| name == "item_gskhnew(Clone)" || name == "item_gskhnew(Clone)")
        {
            Destroy(this.gameObject, huTime);
        }
        else
        {

            Destroy(this.gameObject, time);
        }
     
	}

}
