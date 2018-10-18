using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {

    public GameObject Box;
    public List<string> messages;

    void Start () {
    }
    void Update() {
        if (GameInfo.returnMessgaeList != null)
        {
            messages = GameInfo.returnMessgaeList.MessgaeList;
           StartCoroutine(ShowMessage());
            GameInfo.returnMessgaeList = null;
        }
    }
    IEnumerator ShowMessage()
    {
        foreach (var str in messages)
        {
            if (string.IsNullOrEmpty(str)) continue;
            GameObject go = Instantiate(Box,transform) as GameObject;
            go.gameObject.SetActive(true);
            go.GetComponentInChildren<Text>().text = str;
            yield return new WaitForSeconds(1);
            go.transform.DOLocalMoveY(300, 3);
            Destroy(go,3);
            yield return new WaitForSeconds(0.5f);
        }
      
    }
}
