using UnityEngine;
using System.Collections;

public class ButtonTing : Buttons {


    //谁知道听牌干嘛
    public void OnMouseUpAsButton()
    {
        managerGame.Ting();
        OnAnyButtonClick();
    }
}
