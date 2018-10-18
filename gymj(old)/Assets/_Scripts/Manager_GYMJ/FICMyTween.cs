using UnityEngine;
using System.Collections;

public class FICMyTween : MonoBehaviour {

    //private Vector3 start;
    private Vector3 end;
    private float time=1f;
    private bool move = false;
    private float dis = 0.2f;

    //public TestMoveScrpt(Vector3 startPos,Vector3 endPos,float moveTime)
    //{
    //    this.start = startPos;
    //    this.end = endPos;
    //    time = moveTime;
    //    move = true;
    //}

    public void Move(Vector3 endPos, float moveTime)
    {
        //    this.start = startPos;
        this.end = endPos;
        time = moveTime;
        move = true;
    }

    private void Update()
    {
        if (move)
        {
            if (transform.position != end)
            {

                transform.position = Vector3.Lerp(transform.position, end, (1 / time)*10 * Time.deltaTime);
                if (Vector3.Distance(transform.position, end) < dis)
                {
                    transform.position = end;
                }
            }
            else
            {

                Destroy(this);
            }
        }
    }
}
