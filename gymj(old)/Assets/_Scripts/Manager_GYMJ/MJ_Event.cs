using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using MJBLL.common;
using System.Linq;
using UnityEngine.UI;

public class MJ_Event : MonoBehaviour
{


    private FICpaipaipai paipaipai;
    private Manager_PengGang managerPengGang;
    private FICStartGame startGame;
    private FICMaskPai maskPai;
    private FICMyCards myCards;
    

    public bool isSelect = false;
    private Vector3 figurePos;
    private bool isDown = false;
    public GameObject MJ_Throw;
    public List<GameObject> MJ_Throw_List = new List<GameObject>();
    public GameObject ReminderLine;
    private List<int> ssPai = new List<int>();
    double t1;
    double t2;
    Vector3 Start_MJposition;
    public bool isCanOut = true;
    public bool isCanGang = false;

    public Camera SPCamera;
    public bool isDrag = false;
    public GameObject cloneMj;
    //引用paipaipai脚本的手牌偏移量，测试
    private void Awake()
    {
        Start_MJposition = this.transform.position;
        maskPai = transform.Find("/Main Camera").GetComponent<FICMaskPai>();
        paipaipai = transform.Find("/Main Camera").GetComponent<FICpaipaipai>();
        managerPengGang = transform.Find("/Main Camera").GetComponent<Manager_PengGang>();
        startGame = transform.Find("/Main Camera").GetComponent<FICStartGame>();

        myCards = transform.Find("/Main Camera").GetComponent<FICMyCards>();
        SPCamera = transform.Find("/3D_SP_Camera").gameObject.GetComponent<Camera>();
        ReminderLine = transform.Find("/Game_UI/Interaction_UI/desktop_UI/ReminderLine").gameObject;
    }


    /// <summary>
    /// 将非选中麻将还原位置
    /// </summary>
    public void SetOtherMjPos()
    {

        for (int i = 0; i < myCards.shouPaiGOList.Count; i++)
        {
            if (myCards.shouPaiGOList[i]. GetComponent<MJ_Event>().isSelect)
            {
                myCards.shouPaiGOList[i].transform.position -= myCards.shouPaiOffsetY;
                myCards.shouPaiGOList[i].GetComponent<MJ_Event>().isSelect = false;
            }
        }
        maskPai.CloseSpecialPaiInTabel();
    }

    #region 判定单双击的方法
    private float time1 = 0;
    private float time2 = 0;
    private float time = 0;
    private bool isAdd = false;
    private bool isDouble = false;
    private int count = 0;


    private void Update()
    {
        if (isAdd)
        {
            time += Time.deltaTime;
            if (time > 0.1f)
            {
                isAdd = false;
                time = 0;
                count = 0;
                // Debug.Log("ONECLIK");
                OnePiontClick();
            }
        }
    }

    void OnMouseUp()
    {
        time1 = Time.time;
        ReminderLine.SetActive(false);
        if (Input.mousePosition.y > 280)
        {
            //执行出牌
            DaPaiAndRestOther();
            //transform.position = Start_MJposition;
        }
        else
        {
            //transform.position = Start_MJposition;
        }
        if (!isDouble && !isDrag)
        {
            count++;
            if (count == 1)
            {
                isAdd = true;
            }
            else
            {
                count = 0;
            }
        }
        if (cloneMj != null)
        {
            Destroy(cloneMj);
            if (GameInfo.cloneMjs != null && GameInfo.cloneMjs.Count > 0)
            {
                foreach (var item in GameInfo.cloneMjs)
                {
                    Destroy(item);
                }
            }
        }
        isDrag = false;
    }
    void OnMouseDown()
    {
        time2 = Time.time;
        Start_MJposition = transform.position;
        IsDoubleClick();
    }
    private void OnMouseDrag()
    {
        Vector3 pos1 = SPCamera.WorldToScreenPoint(Start_MJposition);
        Vector3 pos3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pos1.z);
        if (Vector3.Distance(SPCamera.WorldToScreenPoint(Start_MJposition), pos3) > 100f)
        {
            if (!isDrag) { cloneMj = Instantiate(gameObject); GameInfo.cloneMjs.Add(cloneMj); }
            ReminderLine.SetActive(true);
            isDrag = true;
        }
        if (isDrag && cloneMj != null) cloneMj.transform.position = new Vector3(SPCamera.ScreenToWorldPoint(pos3).x, SPCamera.ScreenToWorldPoint(pos3).y + 0.3f, Start_MJposition.z - 1f);
    }
    void IsDoubleClick()
    {
        isDouble = false;
        if (0 < (time2 - time1) && (time2 - time1) < 0.5f)
        {
            //mj1.transform.DOMove(mj2.transform.position, 2f, true).SetEase(Ease.InBounce);
            //Debug.Log("DOUBLE");
            DoubleClick();
            isDouble = true;
            isAdd = false;
            time = 0;
            count = 0;

        }
       

    }
#endregion

    /// <summary>
    /// 单击事件触发，调
    /// 用弹起和落下方法，遮罩选中的牌
    /// </summary>
    void OnePiontClick()
    {
        if (isCanGang)
        {
            WhenGangNeedToChoose(this.GetComponent<MJ_SP>().HS);
            return;
        }

        UpDownAndMask();

    }
    /// <summary>
    /// 牌的弹起或者落回,并遮罩
    /// </summary>
    void UpDownAndMask()
    {
        if (!isSelect)
        {
            SetOtherMjPos();
            //this.transform.Translate(0, 20, 0);
            this.transform.position += myCards.shouPaiOffsetY;//抬起选中手牌
            isSelect = true;
            maskPai.ShowSpecialPaiInTable(gameObject.GetComponent<MJ_SP>().HS);
        }
        else
        {
            //this.transform.Translate(0, -20, 0);
           this.transform.position -= myCards.shouPaiOffsetY;
            isSelect = false;
            maskPai.CloseSpecialPaiInTabel();
            DoubleClick();
        }
    }

    public void UpOnlyWhenSort()
    {
        if (isSelect)
        {
            //SetOtherMjPos();
            //this.transform.Translate(0, 20, 0);
            this.transform.position += myCards.shouPaiOffsetY;//抬起选中手牌
            isSelect = true;
            maskPai.ShowSpecialPaiInTable(gameObject.GetComponent<MJ_SP>().HS);
        }
    }
    /// <summary>
    /// 当有多个杠牌的时候徐亚选择哪一张牌
    /// </summary>
    void WhenGangNeedToChoose(int hs)
    {
        managerPengGang.Gang_S(hs);
        foreach (var item in myCards.shouPaiGOList)
        {
            item.GetComponent<MJ_Event>().isCanGang = false;
        }
    }

    /// <summary>
    /// 双击事件触发，能打牌就打牌，不能打牌不做操作，并将所有的选中的牌还原位
    /// </summary>
    void DoubleClick()
    {
        //    if (!startGame.gangs.activeInHierarchy)
        //    {
        //        startGame.Guo();
        //    }
        DaPaiAndRestOther();

    }

    void DaPaiAndRestOther()
    {
        if (!GameInfo.IsDingQue && isCanOut && GameInfo.HYFw == GameInfo.FW)
        {  
            var gb = GameObject.Find("Main Camera");
            var startGame = gb.GetComponent<FICStartGame>();

            /**/  if (!startGame.huButtonGO.activeInHierarchy
                 && !startGame.guoButtonGO.activeInHierarchy
                 && !startGame.pengButtonGO.activeInHierarchy
                 && !startGame.gangButtonGO.activeInHierarchy
                 && !startGame.tingButtonGO.activeInHierarchy)
              {
                  paipaipai.ChuPai(this);
            //    foreach (var item in myCards.shouPaiGOList)
            //    {
            //        item.GetComponent<MJ_Event>().isCanOut = false;
            //}
                }
        }
        SetOtherMjPos();
    }




    /// <summary>
    /// 双击触发出牌，直接取消所有碰杠胡过信息
    /// </summary>
    void DoubleClickThenGuo()
    {
        //这几句代码有用？
        startGame.pengButtonGO.SetActive(false);
        startGame.gangButtonGO.SetActive(false);
        startGame.guoButtonGO.SetActive(false);
        startGame.huButtonGO.SetActive(false);
        startGame.tingButtonGO.SetActive(false);
        GameInfo.pengInfo = null;
        GameInfo.huPaiInfo = null;
        GameInfo.returnHByType = null;
    }
    private void OnDestroy()
    {
        try
        {
            ReminderLine.SetActive(false);
        }
        catch (Exception ex)
        {
        }
    }
}
