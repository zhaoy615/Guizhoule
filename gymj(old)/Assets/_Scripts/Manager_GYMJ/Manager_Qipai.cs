using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;

public class Manager_Qipai : MonoBehaviour
{
    //public Texture[] HS;
    public Sprite[] Hs;
    public GameObject MJ_Touch;
    private GameObject qipai_S;
    private GameObject qipai_W;
    private GameObject qipai_E;
    private GameObject qipai_N;


    Image peng_S;
    GameObject peng_W;
    GameObject peng_E;
    GameObject peng_N;
    GameObject gang_W;
    GameObject gang_E;
    GameObject gang_N;

    public Vector3 beginPos_E;
    public Vector3 beginPos_S;
    public Vector3 beginPos_W;
    public Vector3 beginPos_N;

    public List<GameObject> MJList = new List<GameObject>();
    public List<Image> mjList_S = new List<Image>();
    public float MJ_w;
    public float MJ_h;
    int countS = 0;
    int countN = 0;
    int countE = 0;
    int countW = 0;
    public FICpaipaipai managerPai;

    private void Start()
    {
        managerPai = GameObject.Find("Main Camera").GetComponent<FICpaipaipai>();
        //手指触摸屏幕下滑上滑的麻将
        if (MJ_Touch && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            MJ_Touch.transform.position = beginPos_S;
        }
    }

    //弃牌S
    public void AddPai(int hs)
    {
        GameObject mj = Instantiate(qipai_S);
        mj.GetComponent<MJ_SP>().HS = hs;
        mj.transform.position = new Vector3(0.0301f + countS % 6 * MJ_w, 0, 0.0684f + countS / 6 * -MJ_h);
        // mj.transform.parent = GameObject.Find("Qipai_S").transform;
        mj.transform.SetParent(GameObject.Find("Qipai_S").transform);
        mj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

        countS++;
        MJList.Add(mj);
        // mj.transform.parent = GameObject.Find("Qipai_S").transform;
        mj.transform.SetParent(GameObject.Find("Qipai_S").transform);
    }

    //弃牌W
    public void AddPai_W(int hs, int id, Transform transform)
    {
        GameObject mj = Instantiate(qipai_W);
        mj.GetComponent<MJ_SP>().HS = hs;
        mj.GetComponent<MJ_SP>().ID = id;
        mj.transform.position = new Vector3(0.0468f - countW / 6 * MJ_h, 0, -0.0316f - countW % 6 * MJ_w);
        //mj.transform.parent = transform;
        mj.transform.SetParent(transform);

        mj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
        countW++;
        MJList.Add(mj);
        //mj.transform.parent = transform;
        mj.transform.SetParent(transform);
    }

    //弃牌E
    public void AddPai_E(int hs, int id, Transform transform)
    {
        GameObject mj = Instantiate(qipai_E);
        mj.GetComponent<MJ_SP>().HS = hs;
        mj.GetComponent<MJ_SP>().ID = id;
        //Debug.LogError(mj.GetComponent<MJ_SP>().Index);

        mj.transform.position = new Vector3(-0.0422f + countE / 6 * -MJ_h, 0, 0.0526f + countE % 6 * -MJ_w);
        //mj.transform.parent = transform;
        mj.transform.SetParent(transform);

        countE++;
        mj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

        MJList.Add(mj);
        //mj.transform.parent = transform;
        mj.transform.SetParent(transform);
    }

    //弃牌N
    public void AddPai_N(int hs, int id, Transform transform)
    {
        GameObject mj = Instantiate(qipai_N);
        mj.GetComponent<MJ_SP>().HS = hs;
        mj.GetComponent<MJ_SP>().ID = id;
        mj.transform.position = new Vector3(-0.0458f + countN % 6 * -MJ_w, 0, -0.0628f + countN / 6 * MJ_h);
        //mj.transform.parent = transform;
        mj.transform.SetParent(transform);
        countN++;

        mj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

        MJList.Add(mj);
        // mj.transform.parent = transform;
        mj.transform.SetParent(transform);
    }

    //S碰
    /*
    public void mj_PengS(int hs)
    {
        for (int i = 1; i <= 3; i++)
        {
            Image mjs = Instantiate(peng_S);
            //生成手牌的rectTransform不跟随父节点改变
            mjs.transform.SetParent(GameObject.Find("Cans_UI").transform, false);
            mjs.GetComponent<MJ_SP>().HS = hs;
            mjs.sprite = GameObject.Find("Main Camera").GetComponent<FICStartGame>().Huase[hs];
            //mjs.transform.position = new Vector3(-285.2137f + mjList_S.Count * 40f, -208.8759f, -1.018678f);
            //mjs.transform.position = GameObject.Find("peng_S").transform.position;
           // mjs.transform.position = managerPai.sFPTrans.position + i * managerPai.s2dOffsetY;

            mjs.transform.parent = GameObject.Find("Peng_S").transform;
            mjList_S.Add(mjs);
        }
    }
    */
    //W碰
    public void mj_PengW(int hs, Transform parent)
    {
        for (int i = 1; i <= 3; i++)
        {
            GameObject mj = Instantiate(peng_W);
            mj.GetComponent<MJ_SP>().HS = hs;

            mj.transform.position = new Vector3(-0.124f, 0.0012f, 0.109f);
            Debug.Log(mj.transform.position.z);
            //  mj.transform.parent = parent;
            mj.transform.SetParent(parent);
            mj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
            MJList.Add(mj);
            //mj.transform.parent = GameObject.Find("Peng_W").transform;
            mj.transform.SetParent(GameObject.Find("Peng_W").transform);
        }
    }

    /// <summary>
    /// 自己杠
    /// </summary>
    /// <param name="hs">花色 </param>
    /// <param name="Gangfw">被杠牌的方位</param>
    public void mj_GangS(int hs, int Gangfw = 0)
    {
        for (int i = 1; i <= 3; i++)
        {
            Image obj = Instantiate(peng_S);
            //生成手牌的rectTransform不跟随父节点改变
            obj.rectTransform.SetParent(GameObject.Find("Cans_UI").transform, false);
            obj.sprite = GameObject.Find("Main Camera").GetComponent<FICStartGame>().HuaseArray[hs];

            obj.transform.position = new Vector3(10f + mjList_S.Count * 40f, beginPos_S.y * 64f, beginPos_S.z);
            //obj.transform.parent = GameObject.Find("Gang_S").transform;
            obj.transform.SetParent(GameObject.Find("Gang_S").transform);
            mjList_S.Add(obj);
            // obj.transform.parent = GameObject.Find("Gang_S").transform;
            obj.transform.SetParent(GameObject.Find("Gang_S").transform);

            if (i == 2)
            {
                Image objs_4 = Instantiate(peng_S);
                objs_4.sprite = GameObject.Find("Main Camera").GetComponent<FICStartGame>().HuaseArray[hs];
                objs_4.transform.position = new Vector3(obj.transform.position.x + 41f, 60f, 0);
                // objs_4.transform.parent = GameObject.Find("Gang_S").transform;
                obj.transform.SetParent(GameObject.Find("Gang_S").transform);
            }
        }
    }

    //w杠
    /// <summary>
    /// 左边杠牌
    /// </summary>
    /// <param name="hs"></param>
    public void mj_GangW(int hs)
    {
        for (int i = 1; i <= 3; i++)
        {
            GameObject obj = Instantiate(gang_W);
            obj.GetComponent<MJ_SP>().HS = hs;

            // obj.transform.parent = GameObject.Find("Gang_W").transform;
            obj.transform.SetParent(GameObject.Find("Gang_W").transform);
            obj.transform.position = new Vector3(0.1214f, 0.0012f, -0.136f + i * 0.0154f);

            obj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
            MJList.Add(obj);

            if (i == 2)
            {
                GameObject objw_4 = Instantiate(gang_W);
                objw_4.transform.position = new Vector3(0.1214f, 0.0012f + 0.002f, -0.136f);
                //objw_4.transform.parent = GameObject.Find("Gang_W").transform;
                obj.transform.SetParent(GameObject.Find("Gang_W").transform);
                obj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
            }
        }
    }

    /// <summary>
    /// 右边杠牌
    /// </summary>
    /// <param name="hs"></param>
    public void mj_GangE(int hs)
    {

        for (int i = 1; i <= 3; i++)
        {
            GameObject obj = Instantiate(gang_E);

            //obj.transform.parent = GameObject.Find("Gang_E").transform;
            obj.transform.SetParent(GameObject.Find("Gang_E").transform);
            obj.transform.position = new Vector3(-0.124f, 0.0012f, 106.209f + i * -0.015f);

            obj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];

            if (i == 2)
            {
                GameObject obje_4 = Instantiate(gang_W);
                obje_4.transform.position = new Vector3(0.1214f, 0.0012f + 0.002f, -0.136f);
                // obje_4.transform.parent = GameObject.Find("Gang_E").transform;
                obj.transform.SetParent(GameObject.Find("Gang_E").transform);
                obj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
            }
        }
    }

    /// <summary>
    /// 上方杠牌
    /// </summary>
    /// <param name="hs"></param>
    public void mj_GangN(int hs)
    {
        for (int i = 1; i <= 3; i++)
        {
            GameObject obj = Instantiate(gang_N);
            obj.GetComponent<MJ_SP>().HS = hs;
            // obj.transform.parent = GameObject.Find("Gang_N").transform;
            obj.transform.SetParent(GameObject.Find("Gang_N").transform);
            obj.transform.position = new Vector3(-0.135f + i * 0.0143f, 0.0012f, -0.162f);

            //obj.transform.parent = this.transform;
            obj.transform.SetParent(this.transform);
            obj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
            MJList.Add(obj);

            if (i == 2)
            {
                GameObject objn_4 = Instantiate(gang_N);
                objn_4.transform.position = new Vector3(-0.135f, 0.0012f + 0.002f, -0.162f);
                //objn_4.transform.parent = GameObject.Find("Gang_N").transform;
                obj.transform.SetParent(GameObject.Find("Gang_N").transform);
                obj.GetComponent<MeshRenderer>().material.mainTexture = transform.GetComponent<FICMyCards>().textureArray[hs];
            }
        }

    }
}
