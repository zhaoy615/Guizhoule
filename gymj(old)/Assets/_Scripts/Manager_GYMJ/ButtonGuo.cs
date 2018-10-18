using UnityEngine;
using System.Collections;

public class ButtonGuo : Buttons {

    public void OnMouseUpAsButton()
    {
        startGame.Guo();
        OnAnyButtonClick();
    }
}
