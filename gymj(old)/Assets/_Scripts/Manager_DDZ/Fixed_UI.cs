using UnityEngine;
using UnityEngine.UI;

public class Fixed_UI : MonoBehaviour {
    public Transform JieSuan_Pel;
    private Text bottomScore;
    private Text userOne;
    private Text userTwo;
    private Text userThree;
    private Button continueBtn;

    void Start () {
	
	}
	
	void Update () {
	
	}
    void InitUI()
    {
        bottomScore = JieSuan_Pel.Find("bottomScore").GetComponent<Text>();
        userOne = JieSuan_Pel.Find("userOne").GetComponent<Text>();
        userTwo = JieSuan_Pel.Find("userTwo").GetComponent<Text>();
        userThree = JieSuan_Pel.Find("userThree").GetComponent<Text>();
        continueBtn = JieSuan_Pel.Find("continueBtn").GetComponent<Button>();
    }
    
    public void SetJieSuanPanel()
    {

    }
}
