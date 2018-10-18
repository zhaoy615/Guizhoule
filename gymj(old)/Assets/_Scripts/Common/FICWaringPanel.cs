using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class FICWaringPanel : MonoBehaviour {


    public static FICWaringPanel _instance;

   // public delegate void DeleGateMethod();
    public Action WarnningMethods;

    private Button _BtnSure;
    private Button btnSureTc;
    private Button btnSureFh;
    private Button close_Btn;
    private Text _TextWarnning;
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
       
            _BtnSure = transform.Find("btnSure").GetComponent<Button>();
            btnSureTc = transform.Find("btnSureTc").GetComponent<Button>();
            btnSureFh = transform.Find("btnSureFh").GetComponent<Button>();
        close_Btn = transform.Find("close_Btn").GetComponent<Button>();
        _TextWarnning = transform.Find("Text").GetComponent<Text>();
        btnSureTc.onClick.AddListener(OnSureButtonClick);
        btnSureFh.onClick.AddListener(delegate { HideQuit(); });
        _BtnSure.onClick.AddListener(OnSureButtonClick);

       Hide();
	

    }
    public void Show(string str)
    {
        this.gameObject.SetActive(true);
        _BtnSure.gameObject.SetActive(true);
        btnSureTc.gameObject.SetActive(false);
        btnSureFh.gameObject.SetActive(false);
        _TextWarnning.text = str;
        close_Btn.onClick = _BtnSure.onClick;
				
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
        WarnningMethods = null;
    }

    public void ShowQuit(string str = "要退出游戏了吗？")
    {
		this.gameObject.SetActive(true);
        _BtnSure.gameObject.SetActive(false);
        btnSureTc.gameObject.SetActive(true);
        btnSureFh.gameObject.SetActive(true);
        close_Btn.onClick = btnSureFh.onClick;
        
        _TextWarnning.text = str;
    }
	//返回键
    public void HideQuit()
    {
        _BtnSure.gameObject.SetActive(true);
        btnSureTc.gameObject.SetActive(false);
        btnSureFh.gameObject.SetActive(false);
        Hide();
    }

    private void OnSureButtonClick()
    {
        SureMethodRun(WarnningMethods);
        Hide();
    }

    private void SureMethodRun(Action del)
    {
        if (del != null)
        {
            del();
        }
        del = null;
    }
}
