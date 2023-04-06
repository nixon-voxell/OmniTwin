using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Defective.JSON;
using Voxell.Util;

public class OpenWeatherAPI : MonoBehaviour
{
    [SerializeField] private string m_APIKey = "edbbd1d7b50f468668e1e376376f730d";
    [SerializeField] private string m_OpenWeatherURL = "http://api.openweathermap.org/data/2.5/weather";
    [SerializeField] private string m_City = "Kuala Lumpur";
    [Tooltip("In seconds.")] [SerializeField] private float m_UpdateInterval;
    private float m_IntervalAccum;

    [Header("Icon")]
    [SerializeField] private string m_IconURL = "http://openweathermap.org/img/wn/";
    [SerializeField] private RawImage m_IconRawImage;
    [SerializeField, InspectOnly] private string m_CurrIconName = "";
    [SerializeField, InspectOnly] private Texture m_IconTexture;

    [Header("TMPro")]
    [SerializeField] private TextMeshProUGUI m_MainTMPro;
    [SerializeField] private TextMeshProUGUI m_DescriptionTMPro;
    [SerializeField] private TextMeshProUGUI m_TempTMPro;
    [SerializeField] private TextMeshProUGUI m_PressureTMPro;
    [SerializeField] private TextMeshProUGUI m_HumidityTMPro;

    private void Start()
    {
        this.StartCoroutine(this.RequestWeatherData());
        this.m_IntervalAccum = 0.0f;
    }

    private void Update()
    {
        this.m_IntervalAccum += Time.deltaTime;

        if (this.m_IntervalAccum > this.m_UpdateInterval)
        {
            Debug.Log("Update Weather");
            this.StartCoroutine(RequestWeatherData());
            this.m_IntervalAccum = 0.0f;
        }
    }

    private IEnumerator RequestWeatherData()
    {
        string requestURL = this.m_OpenWeatherURL + "?appid=" + this.m_APIKey + "&q=" + this.m_City + "&units=metric";
        UnityWebRequest request = UnityWebRequest.Get(requestURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            request.Dispose();
        } else
        {
            JSONObject rawObj = new JSONObject(request.downloadHandler.text);
            JSONObject weatherObj = rawObj["weather"][0];
            JSONObject mainObj = rawObj["main"];

            string main = weatherObj["main"].stringValue;
            string description = weatherObj["description"].stringValue;
            string icon = weatherObj["icon"].stringValue;
            float temp = mainObj["temp"].floatValue;
            float pressure = mainObj["pressure"].floatValue;
            float humidity = mainObj["humidity"].floatValue;

            this.m_MainTMPro.text = main;
            this.m_DescriptionTMPro.text = description;
            this.m_TempTMPro.text = temp.ToString() + " °C";
            this.m_PressureTMPro.text = pressure.ToString() + " Pa";
            this.m_HumidityTMPro.text = humidity.ToString() + " g.m<sup>-3</sup>";

            if (this.m_CurrIconName != icon || this.m_IconTexture == null)
            {
                yield return this.RequestIcon(icon);
            }
            request.Dispose();
        }
    }

    private IEnumerator RequestIcon(string icon)
    {
        string requestURL = this.m_IconURL + icon + "@4x.png";
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(requestURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            request.Dispose();
        } else
        {
            if (this.m_IconTexture != null)
            {
                Object.Destroy(this.m_IconTexture);
            }

            this.m_CurrIconName = icon;
            this.m_IconTexture = DownloadHandlerTexture.GetContent(request);
            this.m_IconRawImage.texture = this.m_IconTexture;
            request.Dispose();
        }
    }
}
