using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPS : MonoBehaviour
{
    public static GPS Instance { set; get; }

    public float latitude;
    public float longitude;
    public string status;
    public int cnt = 0;

    private void Start()
    {
        Instance = this;
        //DontDestroyOnLoad(gameObject); // Warning
        Application.runInBackground = true;
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {


        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {

            status = "User has not enabled GPS";
            Debug.Log(status);

            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait <= 0)
        {
            status = "Timed out";
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            status = "Unable to determine device location";
            yield break;
        }

        else
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                latitude = Input.location.lastData.latitude;
                longitude = Input.location.lastData.longitude;
                status = "GPS ACTIVATE #" + cnt;
                /*
                text.text =
                    "Lat: " + GPS.Instance.latitude.ToString() +
                    "\nLon: " + GPS.Instance.longitude.ToString() +
                    "\nStatus: " + GPS.Instance.status.ToString();
                */

                cnt++;
            }
        }
    }
}