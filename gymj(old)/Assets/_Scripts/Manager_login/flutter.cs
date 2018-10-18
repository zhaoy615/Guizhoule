using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class flutter : MonoBehaviour {

     public Transform flower;
    public Transform flowerTemp;
    private int count = 0;
    // Use this for initialization
    void Start () {
        StartCoroutine(RandomFlower());
    }
    IEnumerator RandomFlower()
    {
        while (count < 10)
        {
                flowerTemp = Instantiate(flower, transform.parent) as Transform;
                count++;
            flowerTemp.localScale = Vector3.one;
            flowerTemp.GetComponent<Animator>().SetInteger("flowerInt", Random.Range(1,3));
            flowerTemp.GetComponent<RectTransform>().localPosition = new Vector2(Random.Range(0, 1500), 540);
            Tweener tr = flowerTemp.GetComponent<RectTransform>().DOLocalMove(new Vector2(Random.Range(-2000, 0), -600), 5).SetLoops(-1);
           // Destroy(tf.gameObject, 5);

            yield return new WaitForSeconds(Random.Range(0.5f,1f));
        }
      
    }
}
