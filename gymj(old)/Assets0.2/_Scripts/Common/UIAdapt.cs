using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAdapt : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (((float)Screen.width/ Screen.height) >= (1920.0f/1080))
            GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        else GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
    }
}
