using UnityEngine;
using System.Collections;

public class Buttons : MonoBehaviour {

    protected FICStartGame startGame;
    protected Manager_PengGang pengGang;
    protected Manager_Game managerGame;
    protected Transform pghgGridTrans;

    protected GameObject guoButtonGO;
    protected GameObject pengButtonGO;
    protected GameObject gangButtonGO;
    protected GameObject huButtonGO;
    protected GameObject tingButtonGO;
    // Use this for initialization
    void Start () {
        startGame = transform.Find("/Main Camera").GetComponent<FICStartGame>();
        pengGang = transform.Find("/Main Camera").GetComponent<Manager_PengGang>();
        managerGame= transform.Find("/Main Camera").GetComponent<Manager_Game>();
        pghgGridTrans = transform.Find("/Game_UI/Interaction_UI/can_buttons_pghg/grid_pghg");

        guoButtonGO = transform.Find("/Game_UI/Interaction_UI/can_buttons_pghg/grid_pghg/item_button_guo").gameObject;
        pengButtonGO = transform.Find("/Game_UI/Interaction_UI/can_buttons_pghg/grid_pghg/item_button_peng").gameObject;
        gangButtonGO = transform.Find("/Game_UI/Interaction_UI/can_buttons_pghg/grid_pghg/item_button_gang").gameObject;
        huButtonGO = transform.Find("/Game_UI/Interaction_UI/can_buttons_pghg/grid_pghg/item_button_hu").gameObject;
        tingButtonGO = transform.Find("/Game_UI/Interaction_UI/can_buttons_pghg/grid_pghg/item_button_ting").gameObject;
    }
    /// <summary>
    /// 按钮点击以后，遍历grid，只留下最后一个过，将其他子物体销毁
    /// </summary>
    protected void OnAnyButtonClick()
    {

        for (int i = 0; i < pghgGridTrans.childCount; i++)
        {
            pghgGridTrans.GetChild(i).gameObject.SetActive(false);
        }

    }

}
