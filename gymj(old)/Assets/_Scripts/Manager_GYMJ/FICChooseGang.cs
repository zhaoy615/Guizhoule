using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FICChooseGang : MonoBehaviour
{

    private int hs;
    private Image hsImage;
    private Button gangButton;
    private Manager_PengGang penggang;
    private FICStartGame startGame;
    private void Awake()
    {
        hsImage = transform.Find("image/001").GetComponent<Image>();
        gangButton = gameObject.GetComponent<Button>();
        gangButton.onClick.AddListener(OnGangButtonClick);

        hsImage.sprite = startGame.HuaseArray[hs];
    }
   
    private void OnGangButtonClick()
    {
        penggang.Gang_S(hs);
        penggang.HideMJSCanGang();
    }


    public void ShowPaiHsImage(Manager_PengGang penggang, FICStartGame startGame,int hs)
    {
        this.penggang = penggang;
        this.hs = hs;
        this.startGame = startGame;
        hsImage = transform.Find("image/001").GetComponent<Image>();
        hsImage.sprite = startGame.HuaseArray[hs];
    }


}
