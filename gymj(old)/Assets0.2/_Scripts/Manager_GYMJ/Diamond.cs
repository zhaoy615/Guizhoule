using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Diamond : MonoBehaviour {

    private Transform location;
    private new Vector3 rota= new Vector3(0, 1, 0);
    void Start () {
        gameObject.transform.DOLocalMoveY(-3.5f, 0.38f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
        location = gameObject.GetComponent<Transform>();
    }

    private void Update()
    {
       // rota = new Vector3(0, 0, 0.001f);

        //location.localRotation = Quaternion.Euler(rota);
        //location.Rotate(rota);
    }

}
