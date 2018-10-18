using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class Show2dJiPai : MonoBehaviour
{


    private Manager_Qipai qipai;

    private GameObject itemJiS;
    private GameObject itemJiN;
    private GameObject itemJiW;
    private GameObject itemJiE;

    private GameObject panel_jiGO;

    private Transform con_jis;
    private Transform con_jin;
    private Transform con_jiw;
    private Transform con_jie;

    private Transform jis1;
    private Transform jis2;
    private Transform jin1;
    private Transform jin2;
    private Transform jiw1;
    private Transform jiw2;
    private Transform jie1;
    private Transform jie2;

    private Vector3 offsetJiS;
    private Vector3 offsetJiN;
    private Vector3 offsetJiW;
    private Vector3 offsetJiE;

    private Vector3 sNextPos;
    private Vector3 nNextPos;
    private Vector3 wNextPos;
    private Vector3 eNextPos;

    public List<int> jisList = new List<int>();
    public List<int> jinList = new List<int>();
    public List<int> jiwList = new List<int>();
    public List<int> jieList = new List<int>();

    public Dictionary<int, int> jisDic = new Dictionary<int, int>();
    public Dictionary<int, int> jinDic = new Dictionary<int, int>();
    public Dictionary<int, int> jiwDic = new Dictionary<int, int>();
    public Dictionary<int, int> jieDic = new Dictionary<int, int>();

    private List<GameObject> sJiALLList=new List<GameObject>();
    private List<GameObject> nJiALLList = new List<GameObject>();
    private List<GameObject> eJiALLList = new List<GameObject>();
    private List<GameObject> wJiALLList = new List<GameObject>();


    public GameObject zhuoJiGO;
    public GameObject huangpaiGO;
    private Image fanji2d;
    private FICpaipaipai paipaipai;
    private FICStartGame startGame;


    private Sprite chongJiImage;
    private Sprite chongWuImage;
    private Sprite zeJiImage;
    private Sprite zeWuImage;
    private Sprite baoImage;

    public int fanJiHS;
    public bool sIsBao = false;
    public bool nIsBao = false;
    public bool wIsBao = false;
    public bool eIsBao = false;
    private void Start()
    {
        qipai = transform.Find("/Main Camera").GetComponent<Manager_Qipai>();
        startGame = transform.Find("/Main Camera").GetComponent<FICStartGame>();
        paipaipai = transform.Find("/Main Camera").GetComponent<FICpaipaipai>();
        itemJiS = Resources.Load("Game_GYMJ/Prefabs/item_2dji_s") as GameObject;
        itemJiN = Resources.Load("Game_GYMJ/Prefabs/item_2dji_n") as GameObject;
        itemJiW = Resources.Load("Game_GYMJ/Prefabs/item_2dji_w") as GameObject;
        itemJiE = Resources.Load("Game_GYMJ/Prefabs/item_2dji_e") as GameObject;

        con_jis = transform.Find("panel_jis/con_jis_s");
        con_jin = transform.Find("panel_jis/con_jis_n");
        con_jiw = transform.Find("panel_jis/con_jis_w");
        con_jie = transform.Find("panel_jis/con_jis_e");

        panel_jiGO = transform.Find("panel_jis").gameObject;

        jis1 = transform.Find("panel_jis/con_jis_s/item_2dji_s1");
        jis2 = transform.Find("panel_jis/con_jis_s/item_2dji_s2");
        jin1 = transform.Find("panel_jis/con_jis_n/item_2dji_n1");
        jin2 = transform.Find("panel_jis/con_jis_n/item_2dji_n2");
        jiw1 = transform.Find("panel_jis/con_jis_w/item_2dji_w1");
        jiw2 = transform.Find("panel_jis/con_jis_w/item_2dji_w2");
        jie1 = transform.Find("panel_jis/con_jis_e/item_2dji_e1");
        jie2 = transform.Find("panel_jis/con_jis_e/item_2dji_e2");

        sNextPos = jis1.position;
        nNextPos = jin1.position;
        wNextPos = jiw1.position;
        eNextPos = jie1.position;

        offsetJiS = jis2.position - jis1.position;
        offsetJiN = jin2.position - jin1.position;
        offsetJiW = jiw2.position - jiw1.position;
        offsetJiE = jie2.position - jie1.position;
        chongJiImage = Resources.Load("Game_GYMJ/Texture/Game_UI/game_icon_CFJ", typeof(Sprite)) as Sprite;
        chongWuImage = Resources.Load("Game_GYMJ/Texture/Game_UI/game_icon_WGJ", typeof(Sprite)) as Sprite;
        zeJiImage = Resources.Load("Game_GYMJ/Texture/Game_UI/game_icon_ZRJ", typeof(Sprite)) as Sprite;
        zeWuImage = Resources.Load("Game_GYMJ/Texture/Game_UI/game_icon_ZRWG", typeof(Sprite)) as Sprite;
        baoImage = Resources.Load("Game_GYMJ/Texture/Game_UI/game_icon_BAO",typeof(Sprite)) as Sprite;

        zhuoJiGO = transform.Find("image_zhuoji").gameObject;
        
        huangpaiGO = transform.Find("image_huangpai").gameObject;

        fanji2d = transform.Find("panel_jis/image_fanji2d").GetComponent<Image>();


    }
    /// <summary>
    /// 显示翻鸡图片，显示3d翻鸡牌，2f后隐藏，然后调用显示各家所有鸡牌
    /// </summary>
    public void ShowFanJi()
    {
        Invoke("CloseFanJiGO", 2f);
    }
    void CloseFanJiGO()
    {
        startGame._NorthIsJiaoAndPao.gameObject.SetActive(false);
        startGame._EastIsJiaoAndPao.gameObject.SetActive(false);
        startGame._SouthIsJiaoAndPao.gameObject.SetActive(false);
        startGame._WestIsJiaoAndPao.gameObject.SetActive(false);
        startGame.fanJiPaiGO.SetActive(false);
        panel_jiGO.SetActive(true);
        ShowAll2DJiBeforeJieSuan();
    }
    
    void ShowWeiJiaoPaiWhen2DJi()
    {
        foreach (var item in GameInfo.cunJS.js)
        {
            if (item.is_jiao==0)
            {
                switch (GameInfo.GetFW(item.userinfo.user_FW))
                {
                    case FW.East:
                        startGame.ShowIsPaoAndIsJiao(FW.East, 13);
                        break;
                    case FW.West:
                        startGame.ShowIsPaoAndIsJiao(FW.West, 13);
                        break;
                    case FW.North:
                        startGame.ShowIsPaoAndIsJiao(FW.North, 13);
                        break;
                    case FW.South:
                        startGame.ShowIsPaoAndIsJiao(FW.South, 13);
                        break;
                }


            }
        }
       
    }
    /// <summary>
    /// 关闭鸡牌panel
    /// </summary>
    public void CloseFanJiPanel()
    {
        panel_jiGO.SetActive(false);
    }

    
    /// <summary>
    /// 显示各家所有的鸡牌
    /// </summary>
    public void ShowAll2DJiBeforeJieSuan()
    {
        if (fanJiHS!=-1)
        {
            fanji2d.sprite = startGame.HuaseArray[fanJiHS];
        }
        GameObject gos;
        GameObject gon;
        GameObject gow;
        GameObject goe;
        for (int i = 0; i < jisList.Count; i++)
        {
             gos = GameObject.Instantiate(itemJiS, sNextPos, Quaternion.Euler(Vector3.zero), con_jis) as GameObject;
            gos.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[jisList[i]];
            gos.transform.localRotation = Quaternion.Euler(Vector3.zero);
            sJiALLList.Add(gos);
                sNextPos += offsetJiS;
            //不需要偏移量了
            
        } 
        for (int i = 0; i < jinList.Count; i++)
        {
             gon = GameObject.Instantiate(itemJiN, nNextPos, Quaternion.Euler(Vector3.zero), con_jin) as GameObject;
            gon.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[jinList[i]];
            gon.transform.localRotation = Quaternion.Euler(Vector3.zero);
            nJiALLList.Add(gon);
                nNextPos += offsetJiN;
        }
        for (int i = 0; i < jiwList.Count; i++)
        {
             gow = GameObject.Instantiate(itemJiW, wNextPos, Quaternion.Euler(Vector3.zero), con_jiw) as GameObject;
            gow.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[jiwList[i]];
            gow.transform.localRotation = Quaternion.Euler(Vector3.zero);
            wJiALLList.Add(gow);
            wNextPos += offsetJiW;
            
        }
        for (int i = 0; i < jieList.Count; i++)
        {
             goe = GameObject.Instantiate(itemJiE, eNextPos, Quaternion.Euler(Vector3.zero), con_jie) as GameObject;
            goe.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[jieList[i]];
            goe.transform.localRotation = Quaternion.Euler(Vector3.zero);
            eJiALLList.Add(goe);
                eNextPos += offsetJiE;
            
        }

        ShowAbnormalJiForEast();
        ShowAbnormalJiForSouth();
        ShowAbnormalJiForWest();
        ShowAbnormalJiForNorth();

        ShowBaoJi();
        ShowWeiJiaoPaiWhen2DJi();
    }
    /// <summary>
    /// 怎么就不会把方法合并成一个哩？
    /// </summary>
    void ShowAbnormalJiForSouth()
    {
        if (jisDic.ContainsKey(2))
        {
            GameObject gos = GameObject.Instantiate(itemJiS, sNextPos, Quaternion.identity, con_jis) as GameObject;
            gos.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[11];
           // gos.transform.Find("image_type").gameObject.SetActive(true);
          //  gos.transform.Find("image_type").GetComponent<Image>().sprite = chongJiImage;
             sJiALLList.Add(gos);
            sNextPos += offsetJiS;
        }
        if (jisDic.ContainsKey(4))
        {
            GameObject gos = GameObject.Instantiate(itemJiS, sNextPos, Quaternion.identity, con_jis) as GameObject;
            gos.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[8];
            //gos.transform.Find("image_type").gameObject.SetActive(true);
           // gos.transform.Find("image_type").GetComponent<Image>().sprite = chongWuImage;
            sJiALLList.Add(gos);
            sNextPos += offsetJiS;
        }

        //责任鸡不显示，
        //if (jisDic.ContainsKey(8))
        //{
        //    GameObject gos = GameObject.Instantiate(itemJiS, sNextPos, Quaternion.identity, con_jis) as GameObject;
        //    gos.transform.Find("Image").GetComponent<Image>().sprite = qipai.Hs[8];
        //    gos.transform.Find("image_type").gameObject.SetActive(true);
        //    gos.transform.Find("image_type").GetComponent<Image>().sprite = zeWuImage;
        //    sJiALLList.Add(gos);
        //    sNextPos += offsetJiS;
        //}
        //if (jisDic.ContainsKey(9))
        //{
        //    GameObject gos = GameObject.Instantiate(itemJiS, sNextPos, Quaternion.identity, con_jis) as GameObject;
        //    gos.transform.Find("Image").GetComponent<Image>().sprite = qipai.Hs[11];
        //    gos.transform.Find("image_type").gameObject.SetActive(true);
        //    gos.transform.Find("image_type").GetComponent<Image>().sprite = zeJiImage;
        //    sJiALLList.Add(gos);
        //    sNextPos += offsetJiS;
        //}
    }

    /// <summary>
    /// 怎么就不会把方法合并成一个哩？
    /// </summary>
    void ShowAbnormalJiForNorth()
    {
        if (jinDic.ContainsKey(2))
        {
            GameObject gon = GameObject.Instantiate(itemJiN, nNextPos, Quaternion.identity, con_jin) as GameObject;
            gon.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[11];
            gon.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //gon.transform.Find("image_type").gameObject.SetActive(true);
            //gon.transform.Find("image_type").GetComponent<Image>().sprite = chongJiImage;
            nJiALLList.Add(gon);
            nNextPos += offsetJiN;
        }
        if (jinDic.ContainsKey(4))
        {
            GameObject gon = GameObject.Instantiate(itemJiN, nNextPos, Quaternion.identity, con_jin) as GameObject;
            gon.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[8];
            gon.transform.localRotation = Quaternion.Euler(Vector3.zero);
            // gon.transform.Find("image_type").gameObject.SetActive(true);
            //gon.transform.Find("image_type").GetComponent<Image>().sprite = chongWuImage;
            nJiALLList.Add(gon);
            nNextPos += offsetJiN;
        }
        //if (jinDic.ContainsKey(8))
        //{
        //    GameObject gon = GameObject.Instantiate(itemJiN, nNextPos, Quaternion.identity, con_jin) as GameObject;
        //    gon.transform.Find("Image").GetComponent<Image>().sprite = qipai.Hs[8];
        //    gon.transform.Find("image_type").gameObject.SetActive(true);
        //    gon.transform.Find("image_type").GetComponent<Image>().sprite = zeWuImage;
        //    nJiALLList.Add(gon);
        //    nNextPos += offsetJiN;
        //}
        //if (jinDic.ContainsKey(9))
        //{
        //    GameObject gon = GameObject.Instantiate(itemJiN, nNextPos, Quaternion.identity, con_jin) as GameObject;
        //    gon.transform.Find("Image").GetComponent<Image>().sprite = qipai.Hs[11];
        //    gon.transform.Find("image_type").gameObject.SetActive(true);
        //    gon.transform.Find("image_type").GetComponent<Image>().sprite = zeJiImage;
        //    nJiALLList.Add(gon);
        //    nNextPos += offsetJiN;
        //}

    }
    /// <summary>
    /// 怎么就不会把方法合并成一个哩？
    /// </summary>
    void ShowAbnormalJiForWest()
    {
        if (jiwDic.ContainsKey(2))
        {
            GameObject gow = GameObject.Instantiate(itemJiW, wNextPos, Quaternion.identity, con_jiw) as GameObject;
            gow.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[11];
            gow.transform.localRotation = Quaternion.Euler(Vector3.zero);
            // gow.transform.Find("image_type").gameObject.SetActive(true);
            // gow.transform.Find("image_type").GetComponent<Image>().sprite = chongJiImage;
            wJiALLList.Add(gow);
            wNextPos += offsetJiW;
        }
        if (jiwDic.ContainsKey(4))
        {
            GameObject gow = GameObject.Instantiate(itemJiW, wNextPos, Quaternion.identity, con_jiw) as GameObject;
            gow.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[8];
            gow.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //gow.transform.Find("image_type").gameObject.SetActive(true);
            //gow.transform.Find("image_type").GetComponent<Image>().sprite = chongWuImage;
            wJiALLList.Add(gow);
            wNextPos += offsetJiW;
        }
        //if (jiwDic.ContainsKey(8))
        //{
        //    GameObject gow = GameObject.Instantiate(itemJiW, wNextPos, Quaternion.identity, con_jiw) as GameObject;
        //    gow.transform.Find("Image").GetComponent<Image>().sprite = qipai.Hs[8];
        //    gow.transform.Find("image_type").gameObject.SetActive(true);
        //    gow.transform.Find("image_type").GetComponent<Image>().sprite = zeWuImage;
        //    wJiALLList.Add(gow);
        //    wNextPos += offsetJiW;
        //}
        //if (jiwDic.ContainsKey(9))
        //{
        //    GameObject gow = GameObject.Instantiate(itemJiW, wNextPos, Quaternion.identity, con_jiw) as GameObject;
        //    gow.transform.Find("Image").GetComponent<Image>().sprite = qipai.Hs[11];
        //    gow.transform.Find("image_type").gameObject.SetActive(true);
        //    gow.transform.Find("image_type").GetComponent<Image>().sprite = zeJiImage;
        //    wJiALLList.Add(gow);
        //    wNextPos += offsetJiW;
        //}
    }
    /// <summary>
    /// 怎么就不会把方法合并成一个哩？
    /// </summary>
    void ShowAbnormalJiForEast()
    {
        if (jieDic.ContainsKey(2))
        {
            GameObject goe = GameObject.Instantiate(itemJiE, eNextPos, Quaternion.identity, con_jie) as GameObject;
            goe.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[11];
            goe.transform.localRotation = Quaternion.Euler(Vector3.zero);
            // goe.transform.Find("image_type").gameObject.SetActive(true);
            // goe.transform.Find("image_type").GetComponent<Image>().sprite = chongJiImage;
            eJiALLList.Add(goe);
            eNextPos += offsetJiE;
        }
        if (jieDic.ContainsKey(4))
        {
            GameObject goe = GameObject.Instantiate(itemJiE, eNextPos, Quaternion.identity, con_jie) as GameObject;
            goe.transform.Find("Image").GetComponent<Image>().sprite = startGame.HuaseArray[8];
            goe.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //goe.transform.Find("image_type").gameObject.SetActive(true);
            //goe.transform.Find("image_type").GetComponent<Image>().sprite = chongWuImage;
            eJiALLList.Add(goe);
            eNextPos += offsetJiE;
        }
        //if (jieDic.ContainsKey(8))
        //{
        //    GameObject goe = GameObject.Instantiate(itemJiE, eNextPos, Quaternion.identity, con_jie) as GameObject;
        //    goe.transform.Find("Image").GetComponent<Image>().sprite = qipai.Hs[8];
        //    goe.transform.Find("image_type").gameObject.SetActive(true);
        //    goe.transform.Find("image_type").GetComponent<Image>().sprite = zeWuImage;
        //    eJiALLList.Add(goe);
        //    eNextPos += offsetJiE;
        //}
        //if (jieDic.ContainsKey(9))
        //{
        //    GameObject goe = GameObject.Instantiate(itemJiE, eNextPos, Quaternion.identity, con_jie) as GameObject;
        //    goe.transform.Find("Image").GetComponent<Image>().sprite = qipai.Hs[11];
        //    goe.transform.Find("image_type").gameObject.SetActive(true);
        //    goe.transform.Find("image_type").GetComponent<Image>().sprite = zeJiImage;
        //    eJiALLList.Add(goe);
        //    eNextPos += offsetJiE;
        //}
    }

    public void ShowBaoJi()
    {
        if (eIsBao)
        {
            foreach (var item in eJiALLList)
            {
                item.transform.Find("image_type").GetComponent<Image>().sprite = baoImage;
                item.transform.Find("image_type").gameObject.SetActive(true);
            }

        }
        if (wIsBao)
        {
            foreach (var item in wJiALLList)
            {
                item.transform.Find("image_type").GetComponent<Image>().sprite = baoImage;
                item.transform.Find("image_type").gameObject.SetActive(true);
            }
        }

        if (nIsBao)
        {
            foreach (var item in nJiALLList)
            {
                item.transform.Find("image_type").GetComponent<Image>().sprite = baoImage;
                item.transform.Find("image_type").gameObject.SetActive(true);
            }

        }   

        if (sIsBao)
        {
            foreach (var item in sJiALLList)
            {
                item.transform.Find("image_type").GetComponent<Image>().sprite = baoImage;
                item.transform.Find("image_type").gameObject.SetActive(true);
            }
        }
    }


    public void ClearAll()
    {
        jisList.Clear();
        jinList.Clear();
        jiwList.Clear();
        jieList.Clear();

        jieDic.Clear();
        jiwDic.Clear();
        jisDic.Clear();
        jinDic.Clear();




      sIsBao = false;
      nIsBao = false;
      wIsBao = false;
      eIsBao = false;

        foreach (GameObject item in sJiALLList)
        {
            Destroy(item);
        }
        foreach (GameObject item in wJiALLList)
        {
            Destroy(item);
        }
        foreach (GameObject item in eJiALLList)
        {
            Destroy(item);
        }
        foreach (GameObject item in nJiALLList)
        {
            Destroy(item);
        }
        sJiALLList.Clear();
        nJiALLList.Clear();
        wJiALLList.Clear();
        eJiALLList.Clear();

    }

}
