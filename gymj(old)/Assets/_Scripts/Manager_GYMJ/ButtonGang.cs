using UnityEngine;
using System.Collections;

public class ButtonGang : Buttons {

    public void OnMouseUpAsButton()
    {
        pengGang.Gang_S();
        OnAnyButtonClick();
    }
}
