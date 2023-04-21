using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UIElements;
using UnityEngine.Networking;
using SimpleJSON;
using System;

public class SideBarUI : MonoBehaviour
{
    public UIDocument uiDoc;
    private TextElement longitude;
    private TextElement locationName;
    private TextElement latitude;
    private ProgressBar humidityIndex;
    private ProgressBar floodAlerts;
    private ProgressBar airPollutionIndex;
    private ProgressBar riskIndex;
    // Start is called before the first frame update

    private IEnumerator ReadWeather()
    {
        var api_key = "edbbd1d7b50f468668e1e376376f730d";
        var url = "https://api.openweathermap.org/data/2.5/";
        var final_url = url + "weather?lat="+ "3.0285"+"&lon="+"101.7385" + "&appid=" + api_key;
  
        UnityWebRequest fetchWeatherAPI = UnityWebRequest.Get(final_url);
       
        yield return fetchWeatherAPI.SendWebRequest();

        if (fetchWeatherAPI.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(fetchWeatherAPI.error);

        }
        else
        {
            Debug.Log("Fecth Weather Successfully!");
            var responseWeather = JSON.Parse(fetchWeatherAPI.downloadHandler.text);
            Debug.Log(responseWeather);
            locationName.text = responseWeather["name"];
            longitude.text = responseWeather["coord"]["lon"];
            latitude.text = responseWeather["coord"]["lat"];
            humidityIndex.title = responseWeather["main"]["humidity"];
            humidityIndex.value = float.Parse(responseWeather["main"]["humidity"]);
        }
       
    }
    private IEnumerator ReadPollution()
    {
        var api_key = "edbbd1d7b50f468668e1e376376f730d";
        var url = "http://api.openweathermap.org/data/2.5/";
        var pollution_url = url + "air_pollution?lat=" + "3.0285" + "&lon=" + "101.7385" + "&appid=" + api_key;
        UnityWebRequest fetchAirPollution = UnityWebRequest.Get(pollution_url);
        yield return fetchAirPollution.SendWebRequest();
        if (fetchAirPollution.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(fetchAirPollution.error);
        }
        else
        {
            Debug.Log("Fetch Pollution Successfully!");
            var responsePollution = JSON.Parse(fetchAirPollution.downloadHandler.text);
            Debug.Log(responsePollution["list"][0]["components"]["so2"]);
            airPollutionIndex.value = float.Parse(responsePollution["list"][0]["components"]["so2"]);
        }

    }
    private void TestClickEvent(ClickEvent clickEv)
    {
        Debug.Log("Registered Callback Click");
    }
    void Start()
    {
        var weather = StartCoroutine(ReadWeather());
        var pollution = StartCoroutine(ReadPollution());
        var root = GetComponent<UIDocument>().rootVisualElement;
        locationName = root.Q<TextElement>("location_name");
        longitude = root.Q<TextElement>("longitude");
        latitude = root.Q<TextElement>("latitude");
        humidityIndex = root.Q<ProgressBar>("HumidityIndex");
        floodAlerts = root.Q<ProgressBar>("FloodAlerts");
        airPollutionIndex = root.Q<ProgressBar>("AirPollutionIndex");
        riskIndex = root.Q<ProgressBar>("RiskIndex");

        //uiButton.clicked += UiButton_Clicked;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
