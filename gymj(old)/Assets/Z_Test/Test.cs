using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DDZ;


public class Test : MonoBehaviour {
    List<Transform> temp = new List<Transform>();
    public int fw;
    public string type;
    void Start() {
        ////发牌
        //PopUp_UI.Instance.CreateCards(1, 20, new int[] { 22, 45, 28, 36, 24, 46, 2, 14, 26, 13, 18, 44, 7, 4, 33, 41, 39, 42, 37, 40 });
        //PopUp_UI.Instance.CreateCards(2, 17);
        //PopUp_UI.Instance.CreateCards(3, 17);
        ////设置底牌
        //Desktop_UI.Instance.SetBottomCards(1, 2, 3);
        //Desktop_UI.Instance.ShowSouthGameOperation(30, new int[] { 0, 1, 2 });



    }

    // Update is called once per frame
    void Update() {

    }
    public void Click()
    {
        Desktop_UI.Instance.ShowTx(type, fw);
    }
    public void aaa(byte[] body)
    {
      
    }
    public void bbb()
    {

    }
}
