using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TingMove : MonoBehaviour {
    
    public IEnumerator Move(Vector2 pos)
    {
        yield return new WaitForSeconds(1.9f);
        transform.Find("Big1boom").gameObject.SetActive(false);
        transform.DOLocalMove(pos,0.787f);
    }
}
