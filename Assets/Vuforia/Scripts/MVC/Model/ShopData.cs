using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct shop
{
    /* 테스트를 위해 퍼블릭으로 설정. 후에 겟셋으로 바꾸면 좋을듯 */
    public int shop_id;
    public string shop_name;
    public string shop_address;
    public string shop_phone;
    public float shop_rating;
    public int shop_cnt_rating;
    public string shop_info_id;
    public string category;
    public string lat;
    public string lon;
    public string type;
    public shop(
        int id,
        string sname,
        string address,
        string number,
        float rate,
        int cnt,
        string info,
        string cate,
        string lat,
        string lon,
        string types
        )
    {
        shop_id = id;
        shop_name = sname;
        shop_address = address;
        shop_phone = number;
        shop_rating = rate;
        shop_cnt_rating = cnt;
        shop_info_id = info;
        category = cate;
        this.lat = lat;
        this.lon = lon;
        type = types;
    }
}

public class ShopData : MonoBehaviour
{
    public List<shop> datamanager = new List<shop>();
    private List<string> cate = new List<string>();
    //정렬된 데이터를 출력하기 위한 변수들 필요
    public int count;
    public int filteringcount;
    //모든 URL과 bundle을 저장할 변수
    //정렬된 URL과 bundle을 저장할 변수

    void Start()
    {
        Debug.Log("샵생성");
    }
    public List<shop> Location()
    {
        
        //List<string[]> a = new List<string[]>();
        /*
        string[] b;
        for (int i = 0; i < datamanager.Count; i++)
        {
            b = new string[5];
            b[0] = datamanager[i].shop_id.ToString();
            b[1] = datamanager[i].shop_name;
            b[2] = datamanager[i].lat;
            b[3] = datamanager[i].lon;
            b[4] = datamanager[i].type;

            Debug.Log(b[0]+","+datamanager[i].shop_id);
            Debug.Log(b[1]+ "," + datamanager[i].shop_name);
            Debug.Log(b[2]+ "," + datamanager[i].lat);
            Debug.Log(b[3]+ "," + datamanager[i].lon);
            Debug.Log(b[4]+ "," + datamanager[i].type);

            a.Add(b);
            b = null;
        }
        */
        return datamanager;
    }
    public void SetShopData(shop s)
    {
        datamanager.Add(s);
        count = datamanager.Count;
    }
    public int getID(int index)
    {
        return datamanager[index].shop_id;
    }
    public string GetRealName(int bundlecount)
    {
        return datamanager[bundlecount].shop_name;
    }
    public string[] GetAllName()
    {
        string[] allname = new string[count];
        for (int i = 0; i < count; i++)
            allname[i] = datamanager[i].shop_name;
        return allname;
    }
    public string[] GetCate()
    {
        string[] allname = new string[count];
        for (int i = 0; i < count; i++)
            allname[i] = datamanager[i].category;
        return allname;
    }
    public string[] GetAllCategoryByFiltering(int index)
    {
        string filter = cate[index];
        List<string> filt = new List<string>();
        for (int i = 0; i < count; i++)
        {
            if (datamanager[i].category.Equals(filter))
            {
                filt.Add(datamanager[i].category);
            }
        }
        string[] output = new string[filt.Count];
        for (int i = 0; i < filt.Count; i++)
        {
            output[i] = filt[i];
        }
        return output;
    }
    public string[] GetAllCategory()
    {
        if (count == 0)
        {
            return null;
        }
        string[] catelist = new string[cate.Count];
        for (int i = 0; i < cate.Count; i++)
        {
            catelist[i] = cate[i];
        }
        return catelist;
    }
    public string GetCategory(int index)
    {
        return cate[index];
    }
    public void SortByRating()
    {
        shop temp;
        for (int i = 0; i < count - 1; i++)
        {
            for (int j = i + 1; j < count; j++)
            {
                if (datamanager[i].shop_rating < datamanager[j].shop_rating)
                {
                    temp = datamanager[i];
                    datamanager[i] = datamanager[j];
                    datamanager[j] = temp;
                }
            }
        }
    }
    public void setCategory()
    {
        if (count == 0)
        {
            return;
        }
        cate.Add(datamanager[0].category);
        bool dif = false;
        for (int i = 1; i < count; i++)
        {
            dif = false;
            for (int j = 0; j < cate.Count; j++)
            {
                if (cate[j].Equals(datamanager[i].category))
                {
                    break;
                }
                if (j == cate.Count - 1)
                {
                    dif = true;
                }
            }
            if (dif)
            {
                cate.Add(datamanager[i].category);
            }
        }
    }
    public int GetIndexByFilteringIndex(int index, string category)
    {
        for (int i = 0; i < count; i++)
        {
            if (datamanager[i].category.Equals(category))
            {
                if (index == 0)
                {
                    return i;
                }
                else
                    index--;
            }
        }
        return 0;
    }
    public int[] GetAllRate()
    {
        int[] rate = new int[count];
        for (int i = 0; i < count; i++)
        {
            rate[i] = (int)datamanager[i].shop_rating;
        }
        return rate;
    }
    public string[] GetAllNameByCategory(int index)
    {
        string filter = cate[index];
        List<string> filt = new List<string>();
        for (int i = 0; i < count; i++)
        {
            if (datamanager[i].category.Equals(filter))
            {
                filt.Add(datamanager[i].shop_name);
            }
        }
        string[] output = new string[filt.Count];
        for (int i = 0; i < filt.Count; i++)
        {
            output[i] = filt[i];
        }
        filteringcount = filt.Count;
        return output;
    }
    public int[] GetAllIDByCategory(int index)
    {
        string filter = cate[index];
        List<int> filt = new List<int>();
        for (int i = 0; i < count; i++)
        {
            if (datamanager[i].category.Equals(filter))
            {
                filt.Add(datamanager[i].shop_id);
            }
        }
        int[] output = new int[filt.Count];
        for (int i = 0; i < filt.Count; i++)
        {
            output[i] = filt[i];
        }
        return output;
    }
    public int GetIdByIndex(int index)
    {
        return datamanager[index].shop_id;
    }
    public int[] GetAllRatingByCategory(int index)
    {
        string filter = cate[index];
        List<int> filt = new List<int>();
        for (int i = 0; i < count; i++)
        {
            if (datamanager[i].category.Equals(filter))
            {
                filt.Add((int)datamanager[i].shop_rating);
            }
        }
        int[] output = new int[filt.Count];
        for (int i = 0; i < filt.Count; i++)
        {
            output[i] = filt[i];
        }
        return output;
    }
    public void setNull()
    {
        filteringcount = 0;
        datamanager = null;
        datamanager = new List<shop>();
        cate = null;
        cate = new List<string>();
    }
}