using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using Facebook.Unity;
using System.Net;
using LitJson;
using Vuforia;
using Mapbox.Examples.LocationProvider;
using Mapbox.Unity.Map;

/* 컨트롤러 클래스 */
public class ServerController : MonoBehaviour
{

    //플랫폼
    Coroutine loginroutine;
    private bool CheckLogin = false;
    private bool coupon = false;
    private bool back_move = false;

    /* 서버 변수 */
    public int version = 1;
    public int bundlecount = 0;
    public AssetBundle bundle;
    WWW www;
    private bool modelon = false;
    /* 이미지 중복 다운로드 방지 변수 */
    private string ImageURL = "";
    /* Model 생성 및 연결 */
    FoodData FData;
    ShopData SData;
    LoginAndNewUser LData;
    UIEventHandler UIController;
    UserData user;
    vbScript Vbscipt;

    /* 생성자 */
    public ServerController()
    {
        Debug.Log("서버컨트롤러 생성");
    }
    /* 생성시 초기화 */
    public void initial()
    {
        gameObject.AddComponent<FoodData>();
        FData = FindObjectOfType<FoodData>();
        LData = FindObjectOfType<LoginAndNewUser>();
        UIController = FindObjectOfType<UIEventHandler>();
        UIController.Connect(this);
        Vbscipt = FindObjectOfType<vbScript>();
        if (Vbscipt != null)
        {
            Vbscipt.Connect(this);
        }
    }
    /* 메뉴판 재인식관련 함수 */
    public void imagemiss()
    {
        UIController.setMenu(Color.red);
    }
    public void imagefound()
    {
        UIController.setMenu(Color.green);
    }
    /* 메뉴판 이름 저장 */
    public bool SetMenuName(string name)
    {
        UIController.setState2("");
        if (modelon)
        {
            bundle.Unload(true);
            modelon = false;
        }
        FData.setNull();
        FData.SetMenuName(name);
        DownloadData(name);
        string[] a = name.Split('_');
        UIController.setMenu(Color.green);
        if (a[0].Equals("coupon"))
        {
            coupon = true;
            return false;
        }
        else
        {
            if (a[0].Equals("test"))
            {
                if (coupon == false)
                {
                    UIController.Show_BackButton(true);

                }

                coupon = true;
                return false;
            }

            if (coupon == true)
            {
                UIController.Show_BackButton(false);
            }

            coupon = false;
            return true;
        }

        //다운로드 후 분할저장된 데이터 UI에 저장
    }
    public void unloadimage()
    {
        if (modelon)
        {
            bundle.Unload(true);
            modelon = false;
        }
    }
    public FoodData getFoodData()
    {
        return FData;
    }
    /* 메뉴판 이름 받아오기 */
    public string GetMenuName()
    {
        return FData.GetMenuName();
    }

    /* 이미지 다운로드 */
    IEnumerator DownloadAndCache(int index)
    {
        if (ImageURL.Equals(FData.GetURL(index)))
        {
            yield break;
        }
        ImageURL = FData.GetURL(index);
        if (modelon)
        {
            bundle.Unload(true);
            modelon = false;
        }
        ///캐시초기화 기능
        ///같은 이름의 새로운모델을 확인하기 위해 사용
        UIController.setState2("캐시찾는중");
        while (!Caching.ready)
        {
            yield return null;
        }
        WWW www = WWW.LoadFromCacheOrDownload(ImageURL, version);
        UIController.setState2("캐시데이터 없음");
        UIController.setState2("Loading");
        yield return www;
        if (www.error != null)
        {
            Debug.Log("fail :(");
            UIController.setState2("");
        }
        bundle = www.assetBundle;
        Instantiate(bundle.LoadAsset<GameObject>(FData.GetBundleName(index)));
        UIController.setState2("");
        modelon = true;
        yield break;
    }
    IEnumerator FindShopList()
    {
        SData = FindObjectOfType<ShopData>();
        if (SData == null)
        {
            gameObject.AddComponent<ShopData>();
            SData = FindObjectOfType<ShopData>();
        }
        Dictionary<string, string> shoplist = new Dictionary<string, string>();
        var encoding = new System.Text.UTF8Encoding();
        string shoplist_data = LitJson.JsonMapper.ToJson(shoplist); // dictionary를 Json 으로 바꿈
        Dictionary<string, string> header = new Dictionary<string, string>(); // header 
        header.Add("Content-Type", "text/json");
        header.Add("Content-Length", System.Convert.ToString(shoplist_data.Length));
        WWW wwwURL = new WWW("http://52.78.20.5/SearchShopList_lon_lat_json.php", encoding.GetBytes(shoplist_data), header);
        yield return wwwURL;

        JsonData user_json = JsonMapper.ToObject(wwwURL.text);

        for (int i = 0; i < user_json.Count; i++)
        {
            SData.SetShopData(new shop(
           int.Parse(user_json[i]["shop_id"].ToString()),
           user_json[i]["shop_name"].ToString(),
           user_json[i]["shop_address"].ToString(),
           user_json[i]["shop_phone"].ToString(),
           2 * float.Parse(user_json[i]["shop_rating"].ToString()),
           int.Parse(user_json[i]["shop_cnt_rating"].ToString()),
           user_json[i]["shop_info_id"].ToString(),
           user_json[i]["shop_category"].ToString(),
           user_json[i]["latitude"].ToString(),
           user_json[i]["longitude"].ToString(),
           user_json[i]["3d_type"].ToString()));

        }
        // UIController.settingShopList(SData.count, SData.GetAllName(), SData.GetCate(), SData.GetAllRate());
        //SData.setCategory();
        //UIController.settingdropdown(SData.GetAllCategory());
        FindObjectOfType<AbstractMap>().level2();
        //FindObjectOfType<PositionWithLocationProvider>().getLocation();
        yield break;
    }
    IEnumerator FindFoodList(string foodname)
    {
        //파라미터로 샵 아이디를 입력하게 된다.
        string shop_id = VuforiaRuntimeInitialization.shop_id;
        /* 샵 아이디를 이용해 음식리스트를 가져옴 */
        WWW wwwURL = new WWW("");
        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> foodlist = new Dictionary<string, string>();
        string foodlist_data = LitJson.JsonMapper.ToJson(foodlist); // dictionary를 Json 으로 바꿈
        Dictionary<string, string> shopid = new Dictionary<string, string>();
        Dictionary<string, string> header = new Dictionary<string, string>(); // header 
        header.Add("Content-Type", "text/json");
        header.Add("Content-Length", System.Convert.ToString(foodlist_data.Length));
        shopid.Add("id", shop_id);
        string shop_id_data = LitJson.JsonMapper.ToJson(shopid); // dictionary를 Json 으로 바꿈
        header["Content-Length"] = System.Convert.ToString(shop_id_data.Length);

        switch (Application.platform)
        {
            //수정이 필요한 부분.
            //현재 안드로이드의 이미지만 가져오는데 윈도우와 아이폰의 이미지를 불러오는 URL입력
            case RuntimePlatform.WindowsEditor:
                wwwURL = new WWW("http://52.78.20.5/SearchFoodAndroid_json.php", encoding.GetBytes(shop_id_data), header);
                break;
            case RuntimePlatform.Android:
                wwwURL = new WWW("http://52.78.20.5/SearchFoodAndroid_json.php", encoding.GetBytes(shop_id_data), header);
                break;
            case RuntimePlatform.IPhonePlayer:
                wwwURL = new WWW("http://52.78.20.5/SearchFoodAndroid_json.php", encoding.GetBytes(shop_id_data), header);
                break;
        }

        yield return wwwURL;

        if (!string.IsNullOrEmpty(wwwURL.error))
        {
            Debug.Log("error");
        }
        else
        {
            if (wwwURL.text == "")
            {
                Debug.Log("ERROR : LIST IS EMPTY");
            }
        }

        JsonData user_json = JsonMapper.ToObject(wwwURL.text);

        for (int i = 0; i < user_json.Count; i++)
        {
            FData.SetFoodData(new food(int.Parse(user_json[i]["food_id"].ToString()), user_json[i]["food_name"].ToString(), int.Parse(user_json[i]["food_price"].ToString()), user_json[i]["url"].ToString(), 2 * float.Parse(user_json[i]["food_rating"].ToString()), int.Parse(user_json[i]["food_cnt_rating"].ToString()), user_json[i]["Category"].ToString(), user_json[i]["Name"].ToString()));
        }

        FData.SortByRating();
        UIController.settingList(FData.count, FData.GetAllName(), FData.GetAllPrice(), FData.GetAllRate());
        FData.setCategory();
        UIController.settingdropdown(FData.GetAllCategory());

        //샵 아이디를 이용해서 샵의 네이버 ID를 가져옴
        wwwURL = new WWW("http://52.78.20.5/SearchByName_Shop_Info_json.php", encoding.GetBytes(shop_id_data), header);

        yield return wwwURL;

        JsonData shop_info_json_data = JsonMapper.ToObject(wwwURL.text); // URL에 있는 string을 JSON으로 맵핑

        Debug.Log(shop_info_json_data["shop_info_id"].ToString());

        if (!string.IsNullOrEmpty(wwwURL.error))
        {
            Debug.Log("error");
        }

        else if (shop_info_json_data["shop_info_id"].ToString().Equals(null))
        {
            Debug.Log("샵의 네이버 id를 가져오지 못했습니다.");
        }

        else
        {
            UIController.setShopID(shop_info_json_data["shop_info_id"].ToString());
        }

        yield break;
    }

    /// <summary>
    /// 로그인을 위한 코드
    /// </summary>
    /* 로그인 코드 */

    IEnumerator Logining(string address, string id, string pw)
    {
        Dictionary<string, string> user = new Dictionary<string, string>(); // 로그인 데이터 
        var encoding = new System.Text.UTF8Encoding();
        user.Add("id", id);
        user.Add("user_pw", pw);
        string data = LitJson.JsonMapper.ToJson(user); // dictionary를 Json 으로 바꿈
        Dictionary<string, string> header = new Dictionary<string, string>(); // header 
        header.Add("Content-Type", "text/json");
        header.Add("Content-Length", System.Convert.ToString(data.Length));
        WWW wwwURL = new WWW(address, encoding.GetBytes(data), header);
        yield return wwwURL;
        Debug.Log(wwwURL.text);
        JsonData user_json = JsonMapper.ToObject(wwwURL.text); // URL에 있는 string을 JSON으로 맵핑
        Debug.Log(user_json["result_code"].ToString());

        if (!string.IsNullOrEmpty(wwwURL.error))
        {
            UIController.setState("서버오류");
        }
        else if (user_json["result_code"].ToString().Equals("0000")) // result_code => 0000 이 로그인이 성공했다는 표시
        {
            SceneManager.LoadScene(3);

            yield break;
        }
        else
        {
            UIController.setState("로그인 실패\n정보가 틀립니다");
        }
    }
    /* 회원가입 */
    IEnumerator New_userr(string address, string id, string pw, string name, string email, string owner)
    {
        Dictionary<string, string> new_user = new Dictionary<string, string>();

        var encoding = new System.Text.UTF8Encoding();

        new_user.Add("id", id);
        new_user.Add("user_pw", pw);
        new_user.Add("user_name", name);
        new_user.Add("user_email", email);

        string data = LitJson.JsonMapper.ToJson(new_user); // dictionary를 Json 으로 바꿈

        //Debug.Log(data);

        Dictionary<string, string> header = new Dictionary<string, string>(); // header 

        header.Add("Content-Type", "text/json");
        header.Add("Content-Length", System.Convert.ToString(data.Length));

        WWW wwwURL = new WWW(address, encoding.GetBytes(data), header);

        yield return wwwURL;

        Debug.Log(wwwURL.text);

        JsonData user_json = JsonMapper.ToObject(wwwURL.text); // URL에 있는 string을 JSON으로 맵핑
        Debug.Log(user_json["result_code"].ToString());

        if (!string.IsNullOrEmpty(wwwURL.error))
        {
            UIController.setState("회원가입 실패\n중복된 아이디 입니다.");
        }

        else if (user_json["result_code"].ToString().Equals("0000"))
        {
            Debug.Log(wwwURL.text);
            SceneManager.LoadScene(1);
        }

        yield break;
    }
    // 평점 업데이트
    IEnumerator RatingUpdate(int id, int score)//파라미터로 푸드 아이디
    {
        /*
        JsonData user_json = JsonMapper.ToObject(wwwURL.text); // URL에 있는 string을 JSON으로 맵핑
        Debug.Log(user_json["result_code"].ToString());
        */

        Dictionary<string, int> food_id = new Dictionary<string, int>();

        var encoding = new System.Text.UTF8Encoding();

        food_id.Add("id", id);

        string food_id_data = LitJson.JsonMapper.ToJson(food_id); // dictionary를 Json 으로 바꿈

        Dictionary<string, string> header = new Dictionary<string, string>(); // header 

        header.Add("Content-Type", "text/json");
        header.Add("Content-Length", System.Convert.ToString(food_id_data.Length));

        WWW wwwURL = new WWW("http://52.78.20.5/GetRate_json.php", encoding.GetBytes(food_id_data), header);

        yield return wwwURL;

        JsonData food_rating_json = JsonMapper.ToObject(wwwURL.text); // URL에 있는 string을 JSON으로 맵핑

        int a = int.Parse(food_rating_json["food_cnt_raing"].ToString()) + 1;
        string b = ((float.Parse(food_rating_json["food_cnt_raing"].ToString()) * float.Parse(food_rating_json["food_raing"].ToString()) + ((float)score) / 2) / (float.Parse(food_rating_json["food_cnt_raing"].ToString()) + 1)).ToString();
        Debug.Log(a);
        Debug.Log(b);

        Dictionary<string, string> food_update_id = new Dictionary<string, string>();
        food_update_id.Add("id", id.ToString());
        food_update_id.Add("cnt", a.ToString());
        food_update_id.Add("score", b.ToString());

        string food_update_data = LitJson.JsonMapper.ToJson(food_update_id); // dictionary를 Json 으로 바꿈

        header["Content-Length"] = System.Convert.ToString(food_update_data.Length);
        wwwURL = new WWW("http://52.78.20.5/SetRate_json.php", encoding.GetBytes(food_update_data), header);

        yield return wwwURL;

        JsonData food_update_comple = JsonMapper.ToObject(wwwURL.text);

        if (!string.IsNullOrEmpty(wwwURL.error))
        {
            Debug.Log("error");
        }

        else if (food_update_comple["result_code"].ToString().Equals("0000"))
        {
            Debug.Log("등록 성공");
        }

        else
        {
            Debug.Log("등록 실패");
        }

        yield break;
    }

    /* 다운로드 요청 함수 */
    //일반적으로 다운로드 할 경우 사용
    //음식이미지를 전체적으로 확인할 때 사용
    //실제 앱에서는 사용하지 않는다
    public void DownloadImage(int index)
    {
        if (coupon)
            return;
        StartCoroutine(DownloadAndCache(index));
    }
    public void DownloadData(string name)
    {
        StartCoroutine(FindFoodList(name));
    }
    public void DownloadShop()
    {
        StartCoroutine(FindShopList());
    }
    public void Login()
    {
        LData = FindObjectOfType<LoginAndNewUser>();

        if (LData.getid().Equals("") || LData.getpw().Equals(""))
        {
            UIController.setState("아이디와 비밀번호를 입력하세요");
            return;
        }

        loginroutine = StartCoroutine(Logining(LData.url, LData.getid(), LData.getpw()));

        /*
        string check = new WebClient().DownloadString("http://52.78.20.5/login_db_test.php?id=" + LData.getid() + "&pw=" + LData.getpw());
        Debug.Log(check);

        if(check != "")
        {
            SceneManager.LoadScene(3);
        }
        */
    }
    public void new_user()
    {
        LoginAndNewUser[] a = FindObjectsOfType<LoginAndNewUser>();
        Debug.Log(a.Length);
        LData = FindObjectOfType<LoginAndNewUser>();

        if (LData.user_id.Equals(""))
        {
            UIController.setState("아이디를 입력하세요");
            return;
        }

        else if (LData.user_pw.Equals(""))
        {
            UIController.setState("비밀번호를 입력하세요");
            return;
        }
        else if (LData.user_name.Equals(""))
        {
            UIController.setState("이름을 입력하세요");
            return;
        }
        else if (LData.user_email.Equals(""))
        {
            UIController.setState("이메일을 입력하세요");
            return;
        }

        StartCoroutine(New_userr(LData.url2, LData.user_id, LData.user_pw, LData.user_name, LData.user_email, LData.owner));
    }

    public void filteringdata(int index)
    {
        if (index == 0)
        {
            UIController.settingList(FData.count, FData.GetAllName(), FData.GetAllPrice(), FData.GetAllRate());
            return;
        }
        string[] namelist = FData.GetAllNameByCategory(index - 1);
        UIController.settingList(FData.filteringcount, namelist, FData.GetAllPriceByCategory(index - 1), FData.GetAllRatingByCategory(index - 1));
    }

    public void filterlingdata_Review(int index)
    {
        int[] ss = FData.GetAllIDByCategory(index - 1);
        string[] s = FData.GetAllNameByCategory(index - 1);
        Vbscipt.setFoodList(FData.filteringcount, s, ss);
    }


    //필터링 되었을 경우 출력하는 이미지
    public void DownloadImageFromFilteringIndex(int index, int filteringindex)
    {
        if (coupon)
            return;
        if (filteringindex == 0)
        {
            StartCoroutine(DownloadAndCache(index));
            return;
        }
        //카테고리이름
        StartCoroutine(DownloadAndCache(FData.GetIndexByFilteringIndex(index, FData.GetCategory(filteringindex - 1))));
    }
    public void StartUpdate(int id, int score)
    {
        StartCoroutine(RatingUpdate(id, score));
    }
    public void vbgetcate()
    {
        Vbscipt.getCategory(FData.GetAllCategory());
    }

    /*
    private void Update()
    {
        if (CheckLogin)
        {
            StopCoroutine(loginroutine);
            SceneManager.LoadScene(3);
        }
    }
    */
}