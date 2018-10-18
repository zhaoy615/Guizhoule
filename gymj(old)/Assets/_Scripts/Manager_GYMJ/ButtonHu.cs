using UnityEngine;
using System.Collections;

public class ButtonHu : Buttons {


    public void OnMouseUpAsButton()
    {
        startGame.Hu();
        OnAnyButtonClick();
    }
}
