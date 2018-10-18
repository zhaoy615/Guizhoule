using UnityEngine;
using System.Collections;

public class CameraAdapt : MonoBehaviour {

    Camera _camera;
    float standardPixel = 1.7777f;
    float currentPixel;
    float error = 0.1f;
    public float offsetW;
    public float offsetH;

    void Start()
    {
        _camera = GetComponent<Camera>();
        currentPixel = (float)Screen.width / Screen.height;
        offsetW = (Screen.height * standardPixel - Screen.width) / (2 * Screen.width);
        offsetH = (Screen.width / standardPixel - Screen.height) / (2 * Screen.height);
        if (currentPixel > standardPixel + error)
        {
            _camera.rect = new Rect(Mathf.Abs(offsetW), 0, 1 - 2 * Mathf.Abs(offsetW), 1);
        }
        else if (currentPixel < standardPixel - error)
        {
            _camera.rect = new Rect(0, Mathf.Abs(offsetH), 1, 1 - 2 * Mathf.Abs(offsetH));
        }
        else
        {
            _camera.aspect = 1920.0f / 1080;
            offsetW = 0;
            offsetH = 0;
        }
    }
}
