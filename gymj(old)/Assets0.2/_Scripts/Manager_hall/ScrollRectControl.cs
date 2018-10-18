using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ScrollRectControl : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    ScrollRect rect;
    Scrollbar bar;
    int index = 0;
    int showNum = 5;
    public int startNum;
    public Action refresh;
    List<GameObject> items = new List<GameObject>();
    Vector2 point;

    public void InitScrollRect(int num, Action act)
    {
        if (!transform.parent.gameObject.activeSelf) return;
        startNum = num;
        rect = GetComponent<ScrollRect>();
        bar = rect.verticalScrollbar;
        StartCoroutine(AddObj());
        refresh = null;
        refresh = act;
    }
    IEnumerator AddObj()
    {
        yield return 0;
        items.Clear();
        foreach (Transform item in rect.content)
        {
            items.Add(item.gameObject);
        }
        addItem();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        point = eventData.position;
    }
    public void OnEndDrag(PointerEventData data)
    {
        if (bar != null && bar.value <= 0 && (data.position.y - point.y) > 400)
        {
                addItem();
        }
        Debug.Log(data.position.y - point.y);
        if (bar != null && bar.value >= 1 && (data.position.y - point.y) < -400)
        {
            Debug.Log("下拉刷新");
            items.Clear();
            SureMethodRun(refresh);
            index = 0;
        }
    }
    void addItem()
    {
        for (int i = startNum; i < items.Count; i++)
        {
            items[i].SetActive(true);
            if ((i - index) > showNum)
            {
                index = i;
                break;
            }
            if (i + 1 == items.Count)
            {
                index = i;
                break;
            }
        }
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
