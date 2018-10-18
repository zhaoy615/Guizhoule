using UnityEngine;
using System.Collections;

public class TimeOfDuration : MonoBehaviour {
    public float time;

    private void OnEnable()
    {
        StartCoroutine(delay());
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(time);
        ObjectPool.Instance.Unspawn(gameObject);
    }
}
