using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Vuforia;
using UnityEngine.UI;
using System;

public class Mouse_Touch : MonoBehaviour
{
    public GameObject shop;

    public Text ShowState_mapdown;
    public UnityEngine.UI.Image showstatus_mapdown;

    public Dictionary<string, string> shop_info = new Dictionary<string, string>();

    private GameObject ar_Panel;
    string shop_id;
    public string url;

    void OnMouseDown() // 따로따로 바뀜
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began) // Touch시에 동작(핸드폰)
        {
            Debug.Log(this.gameObject.name);

            string gameObject_name = this.gameObject.name.ToString();

            string[] get_shop_id_1 = gameObject_name.Split('_');

            Debug.Log(get_shop_id_1[1]);

            //string[] get_real_shop_id = get_shop_id_1[1].Split('(');

            //shop_id = get_real_shop_id[0];
            Debug.Log(get_shop_id_1[2]);
            VuforiaRuntimeInitialization.shop_id = get_shop_id_1[2];
            //VuforiaRuntimeInitialization.shop_id = get_real_shop_id[0];

            //Debug.Log(shop_info.Count);

            /*
            foreach (var item in shop_info)
            {
                if (shop_info.Keys.Equals(get_real_shop_id[0].ToString()))
                {
                    url = shop_info.Values.ToString();
                    Debug.Log(url);
                }
            }
            */

            /*
            ShopData s = FindObjectOfType<ShopData>();
            for (int i=0;i< s.datamanager.Count;i++) {
                
                if(s.datamanager[i].shop_id == Int.Parse(get_real_shop_id[0]))
                {
                    url = s.datamanager[i].shop_info_id;
                    break;
                }
            }
            */
            string[] uu = get_shop_id_1[3].Split('(');
            url = uu[0];

            Debug.Log(url);
            Application.OpenURL(url);

            //ar_Panel.transform.localPosition.Set(ar_Panel.transform.localPosition.x, 0.0f, ar_Panel.transform.localPosition.z);
            if (get_shop_id_1[0].Equals("Food"))
            {
                ar_Panel.transform.Translate(0.0f, -272.0f, 0.0f);
            }

            //SceneManager.LoadScene(4);
        }
    
    /*
        if (Input.GetMouseButtonDown(0)) // 마우스 클릭시에 동작(컴퓨터)
        {
            Debug.Log(this.gameObject.name);

            string gameObject_name = this.gameObject.name.ToString();

            string[] get_shop_id_1 = gameObject_name.Split('_');

            Debug.Log(get_shop_id_1[1]);

            //string[] get_real_shop_id = get_shop_id_1[1].Split('(');

            //shop_id = get_real_shop_id[0];
            Debug.Log(get_shop_id_1[2]);
            VuforiaRuntimeInitialization.shop_id = get_shop_id_1[2];
            //VuforiaRuntimeInitialization.shop_id = get_real_shop_id[0];

            //Debug.Log(shop_info.Count);

            /*
            foreach (var item in shop_info)
            {
                if (shop_info.Keys.Equals(get_real_shop_id[0].ToString()))
                {
                    url = shop_info.Values.ToString();
                    Debug.Log(url);
                }
            }*/
            /*
            string[] uu = get_shop_id_1[3].Split('(');
            url = uu[0];

            Debug.Log(url);
            Application.OpenURL(url);

            //ar_Panel.transform.localPosition.Set(ar_Panel.transform.localPosition.x, 0.0f, ar_Panel.transform.localPosition.z);
            if (get_shop_id_1[0].Equals("Food"))
            {
                ar_Panel.transform.Translate(0.0f, -272.0f, 0.0f);
            }

            SceneManager.LoadScene(4);
        }
    */
    }

    public void ok_Button_Click()
    {
        SceneManager.LoadScene(4);
    }

    public void close_Button_Click()
    {
        ar_Panel.transform.Translate(0.0f, 272.0f, 0.0f);
    }

    // Use this for initialization
    void Start()
    {
        ar_Panel = GameObject.Find("ar_Panel");
    }


    // Update is called once per frame
    void Update() // 업데이트에 삽입시에는 같이 바뀜
    {

    }

    public void setShopinfo(string shop_id, string shop_info_id)
    {
        shop_info.Add(shop_id, shop_info_id);
    }

    public void SetState(string text)
    {
        ShowState_mapdown.text = text;

        if (text.Equals(""))
        {
            showstatus_mapdown.GetComponent<UnityEngine.UI.Image>().enabled = false;
        }
    }
}
