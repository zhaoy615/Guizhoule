using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleColor : MonoBehaviour {
    
	void Update () {

        GetComponent<Text>().color = transform.GetComponentInParent<Toggle>().isOn ? Color.green : Color.black;

    }
}
