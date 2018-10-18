using UnityEngine;
using System.Collections;

public class Shader_Flash : MonoBehaviour {

    private Material _mat;
    private float _alpha = 0;
    private bool _isFadeIn = true;
    void Start()
    {
        _mat = this.GetComponent<Renderer>().material;
        _mat.color = new Color(1, 1, 1, _alpha);
    }

    void Update()
    {
        _alpha += (_isFadeIn ? 1 : -1) * Time.deltaTime;
        _mat.color = new Color(1, 1, 1, _alpha);
        if (_alpha > 1) _isFadeIn = false;
        if (_alpha < 0) _isFadeIn = true;
    }
}
