using UnityEngine;
using System.Collections;

public class ButtonPeng : Buttons {

    
    public void OnMouseUpAsButton()
    {
        pengGang.Peng_S();
        OnAnyButtonClick();
    }
}
