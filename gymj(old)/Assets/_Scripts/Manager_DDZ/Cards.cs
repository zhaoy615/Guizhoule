using System;
using UnityEngine;
using UnityEngine.UI;

public class Cards : ReusbleObject{
    public bool clickStatus = false;
    public int PaiID = 0;
    public int PaiHS = 0;
    public string Name = "";
    public void SetCardsInfo(int hs,int id, string name, Sprite spt)
    {
        PaiHS = hs;
        PaiID = id;
        Name = name;
        transform.Find("chartlet").GetComponent<Image>().sprite = spt;
    }


    private bool isBottomCard = false;
    public void SetBottomCard(bool status)
    {
        isBottomCard = status;
        transform.Find("chartlet/mask").gameObject.SetActive(isBottomCard);
    }
   
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }
    public void OnSelect()
    {
        if (clickStatus)
        {
            transform.Find("chartlet").position -= Vector3.up * 5;
            clickStatus = false;
        }
        else
        {
            transform.Find("chartlet").position += Vector3.up * 5;
            clickStatus = true;
        }
    }
    public override void OnSpawn()
    {
        //throw new NotImplementedException();
    }

    public override void OnUnspawn()
    {
        //throw new NotImplementedException();
    }
}
