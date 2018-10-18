using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SouthOperationArea : Singleton<SouthOperationArea> ,IPointerClickHandler,IPointerDownHandler,IPointerUpHandler{

    public List<Transform> southCardList = new List<Transform>();
    public List<Transform> selectCardList = new List<Transform>();
    public float offsetMax = 110;
    public float offsetMin = 30;
    public float OriginX;
    public Transform OriginCard;
    public float EndX;
    public Transform EndCard;
    private bool isSelect = false;
    private bool bl = false;

    private void Start()
    {
    }
    void InitUI()
    {
    }
    public void AddSouthCard(Transform tf)
    {
        southCardList.Add(tf);
    }
    public void DelSouthCard(Transform tf)
    {
        southCardList.Remove(tf);
    }
    void DragSelectCards()
    {
        selectCardList.Clear();
        foreach (Transform card in southCardList)
         {
            if (!card.gameObject.activeSelf) continue;
            float tempX = Camera.main.WorldToScreenPoint(card.position).x;
            if (EndX >= OriginX)
            {
                if (OriginX > tempX - offsetMax && OriginX < tempX - offsetMin)
                {
                    //得到起点牌
                    OriginCard = card;
                    isSelect = true;
                }
                if (EndX > tempX - offsetMax && EndX < tempX + offsetMax)
                {
                    //得到终点牌
                    EndCard = card;
                    if(southCardList.IndexOf(card) == southCardList.Count - 1) isSelect = true;
                    bl = true;
                }
                else
                {
                    if(bl) isSelect = false;
                }
                if (isSelect) card.GetComponent<Cards>().OnSelect();
            }
            else
            {
                if (EndX > tempX - offsetMax && EndX < tempX - offsetMin)
                {
                    //得到起点牌
                    EndCard = card;
                    isSelect = true;
                }
                if (OriginX > tempX - offsetMax && OriginX < tempX + offsetMax)
                {
                    //得到终点牌
                    OriginCard = card;
                    if (southCardList.IndexOf(card) == southCardList.Count - 1) isSelect = true;
                    bl = true;
                }
                else
                {
                    if (bl) isSelect = false;
                }
                if (isSelect) card.GetComponent<Cards>().OnSelect();
            }
            if (card.GetComponent<Cards>().clickStatus && !selectCardList.Contains(card)) selectCardList.Add(card);

        }
        bl = false;
        isSelect = false;
        Debug.Log(selectCardList.Count);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        EndX = eventData.position.x;
        Debug.Log("抬起");
        DragSelectCards();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OriginX = eventData.position.x;
        Debug.Log("按下");
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
