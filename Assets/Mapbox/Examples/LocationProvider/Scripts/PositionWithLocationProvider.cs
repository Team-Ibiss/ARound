namespace Mapbox.Examples.LocationProvider
{
    using Mapbox.Unity.Location;
    using Mapbox.Unity.Utilities;
    using UnityEngine.UI;
    using Mapbox.Unity.Map;
    using UnityEngine;
    using Vuforia;
    using System.Collections.Generic;

    public class PositionWithLocationProvider : MonoBehaviour
    {
        [SerializeField]
        private AbstractMap _map;
        private ServerController server;
        private ShopData SData;
        //location 리스트 설명
        //location[0] = 샵 아이디
        //location[1] = 샵 이름
        //location[2] = lat
        //location[3] = lon
        //location[4] = 타입
        //데이터 갯수 : location.Count

        GPS gps;
        private Mouse_Touch Mouse_Control;
        string[] type;
        
        /// <summary>
        /// The rate at which the transform's position tries catch up to the provided location.
        /// </summary>
        [SerializeField]
        float _positionFollowFactor;

        /// <summary>
        /// Use a mock <see cref="T:Mapbox.Unity.Location.TransformLocationProvider"/>,
        /// rather than a <see cref="T:Mapbox.Unity.Location.EditorLocationProvider"/>. 
        /// </summary>
        [SerializeField]
        bool _useTransformLocationProvider;

        public bool _isInitialized;

        bool _isMark = true;

        GameObject[] shops;
        GPS gpss;

        /// <summary>
        /// The location provider.
        /// This is public so you change which concrete <see cref="T:Mapbox.Unity.Location.ILocationProvider"/> to use at runtime.
        /// </summary>
        ILocationProvider _locationProvider;
        public ILocationProvider LocationProvider
        {
            private get
            {
                if (_locationProvider == null)
                {
                    _locationProvider = _useTransformLocationProvider ?
                       LocationProviderFactory.Instance.TransformLocationProvider : LocationProviderFactory.Instance.DefaultLocationProvider;
                }

                return _locationProvider;
            }
            set
            {
                if (_locationProvider != null)
                {
                    _locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;

                }
                _locationProvider = value;
                _locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
            }
        }

        Vector3 _targetPosition;
        Vector3 _shop_1_Position;
        Vector3 _shop_2_Position;
        Vector3 _shop_3_Position;
        Vector3[] shops_Position;

        void Start()
        {
            gpss = FindObjectOfType<GPS>();
            Debug.Log("Position Location");
            LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
            _map.OnInitialized += () => _isInitialized = true;

            /*
            Debug.Log("Postition 생성");
            
            server = FindObjectOfType<ServerController>();
            if (server == null)
            {
                gameObject.AddComponent<ServerController>();
                server = FindObjectOfType<ServerController>();
            }
            server.DownloadShop();
            */

            //GameObject shop = GameObject.Find("Shop");

            /*
            for (int i = 0; i < shops.Length; i++)
            {
                shops[i] = shop;
                //shops[i].name = "shop_" + (i + 1).ToString();
            }
            */
        }

        public void first()
        {
            server = FindObjectOfType<ServerController>();
            if (server == null)
            {
                gameObject.AddComponent<ServerController>();
                server = FindObjectOfType<ServerController>();
            }
            server.DownloadShop();
        }

        public void getLocation()
        {
            Mouse_Control = FindObjectOfType<Mouse_Touch>();
            SData = FindObjectOfType<ShopData>();
            shops = new GameObject[SData.datamanager.Count];
            shops_Position = new Vector3[SData.datamanager.Count];
            type = new string[SData.datamanager.Count];

            for (int i = 0; i < SData.datamanager.Count; i++)
            {
                //shops[i] = shop;
                shops[i] = GameObject.Find(SData.datamanager[i].type);
                shops[i].name = SData.datamanager[i].type+"_"+ SData.datamanager[i].shop_name + "_" + SData.datamanager[i].shop_id + "_" + SData.datamanager[i].shop_info_id;
                shops_Position[i] = Conversions.GeoToWorldPosition(double.Parse(SData.datamanager[i].lat), double.Parse(SData.datamanager[i].lon),
                                                                   _map.CenterMercator,
                                                                   _map.WorldRelativeScale).ToVector3xz();
                shops_Position[i].y += 4;
                //Mouse_Control.setShopinfo(SData.datamanager[i].shop_id.ToString(), SData.datamanager[i].shop_info_id);
                //Mouse_Control.url = SData.datamanager[i].shop_info_id;
                //Debug.Log(SData.datamanager[i].shop_info_id);
                Instantiate(shops[i], shops_Position[i], Quaternion.identity);
                shops[i].name = SData.datamanager[i].type;
            }

            /*
            SData = FindObjectOfType<ShopData>();
            shops = new GameObject[SData.datamanager.Count];
            shops_Position = new Vector3[SData.datamanager.Count];
            type = new string[SData.datamanager.Count];

            for (int i = 0; i < SData.datamanager.Count; i++)
            {
                Debug.Log(SData.datamanager[i].shop_id);
                Debug.Log(SData.datamanager[i].shop_name);
                Debug.Log(SData.datamanager[i].lat);
                Debug.Log(SData.datamanager[i].lon);
                Debug.Log(SData.datamanager[i].type);

                //shops[i] = shop;
                shops[i] = GameObject.Find(SData.datamanager[i].type);
                shops[i].name = SData.datamanager[i].shop_name+"_"+ SData.datamanager[i].shop_id;
                shops_Position[i] = Conversions.GeoToWorldPosition(double.Parse(SData.datamanager[i].lat), double.Parse(SData.datamanager[i].lon),
                                                                   _map.CenterMercator,
                                                                   _map.WorldRelativeScale).ToVector3xz();
                shops_Position[i].y += 4;

                Instantiate(shops[i], shops_Position[i], Quaternion.identity);
                shops[i].name = SData.datamanager[i].type;
            }
            */

            Debug.Log("get_Location");
            FindObjectOfType<MapBoxAnimation>().Delete();
            LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
        }

        void OnDestroy()
        {
            if (LocationProvider != null)
            {
                LocationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
            }
        }

        void LocationProvider_OnLocationUpdated(object sender, LocationUpdatedEventArgs e)
        {

            if (_isInitialized)
            {
                Debug.Log("asddsa");

                _targetPosition = Conversions.GeoToWorldPosition(e.Location,
                                                                  _map.CenterMercator,
                                                                  _map.WorldRelativeScale).ToVector3xz();
                //Debug.Log("is mark 전");

                /*
                if (_isMark)
                {
                    
                    Debug.Log("is mark!!");

                    Mouse_Control = FindObjectOfType<Mouse_Touch>();
                    SData = FindObjectOfType<ShopData>();
                    shops = new GameObject[SData.datamanager.Count];
                    shops_Position = new Vector3[SData.datamanager.Count];
                    type = new string[SData.datamanager.Count];

                    for (int i = 0; i < SData.datamanager.Count; i++)
                    {
                        //shops[i] = shop;
                        shops[i] = GameObject.Find(SData.datamanager[i].type);
                        shops[i].name = SData.datamanager[i].shop_name + "_" + SData.datamanager[i].shop_id;
                        shops_Position[i] = Conversions.GeoToWorldPosition(double.Parse(SData.datamanager[i].lat), double.Parse(SData.datamanager[i].lon),
                                                                           _map.CenterMercator,
                                                                           _map.WorldRelativeScale).ToVector3xz();
                        shops_Position[i].y += 4;

                        Mouse_Control.setShopinfo(SData.datamanager[i].shop_id.ToString(), SData.datamanager[i].shop_info_id);

                        Instantiate(shops[i], shops_Position[i], Quaternion.identity);
                        shops[i].name = SData.datamanager[i].type;
                    }
                
                _isMark = false;
                
                }  
                */
            }
        }

        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _positionFollowFactor);
        }
    }
}