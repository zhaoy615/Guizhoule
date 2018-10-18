using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class InputFieldWrapper : MonoBehaviour, ISelectHandler
{
    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        StartCoroutine(MoveTextEnd());
        Debug.Log("start");
    }

    IEnumerator MoveTextEnd()
    {
        yield return 0;
        GetComponent<InputField>().text = "";
        Debug.Log("End");
    }
}