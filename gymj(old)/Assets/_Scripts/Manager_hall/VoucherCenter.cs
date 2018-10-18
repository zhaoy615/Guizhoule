using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VoucherCenter : MonoBehaviour
{

    public UniWebView _UniWebView;
    private GameObject voucherCenter_Pel;
    private Transform TopLeft;
    private Transform BottomRight;
    float top = 0;
    float bottom = 0;
    float left = 0;
    float right = 0;

    // Use this for initialization
    void Start()
    {
        voucherCenter_Pel = gameObject;
        TopLeft = voucherCenter_Pel.transform.Find("TopLeft");
        BottomRight = voucherCenter_Pel.transform.Find("BottomRight");
        top = Screen.height - Camera.main.WorldToScreenPoint(TopLeft.position).y;
        left = Camera.main.WorldToScreenPoint(TopLeft.position).x;
        bottom = Camera.main.WorldToScreenPoint(BottomRight.position).y;
        right = Screen.width - Camera.main.WorldToScreenPoint(BottomRight.position).x;
        _UniWebView.insets = new UniWebViewEdgeInsets(ConvertPixelToPoint(top, false), ConvertPixelToPoint(left, true), ConvertPixelToPoint(bottom, false), ConvertPixelToPoint(right, true));
        _UniWebView.url = "http://static.weiwubao.com/upload/800625/image/20150205/resize_20150205150145_44719.jpg";
        _UniWebView.autoShowWhenLoadComplete = false;
        _UniWebView.backButtonEnable = false;
        _UniWebView.SetShowSpinnerWhenLoading(false);
    }
    public void OnLoad()
    {
        //_UniWebView.Load();
    }
    public void Show()
    {
        _UniWebView.Load();
        _UniWebView.Show();
    }
    public void Hide()
    {
        voucherCenter_Pel.SetActive(false);
        _UniWebView.Hide();
    }
    public int ConvertPixelToPoint(float pixel, bool width)
    {
#if UNITY_IOS && !UNITY_EDITOR
        float scale = 0;
        if (width)
        {
            scale = 1f * 1920 / Screen.width;
        }
        else
        {
            scale = 1f * 1080 / Screen.height;
        }

        return (int)(pixel * scale);
#endif

        return (int)pixel;
    }
}

