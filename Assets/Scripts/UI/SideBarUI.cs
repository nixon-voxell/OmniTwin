using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using SimpleJSON;

public class SideBarUI : MonoBehaviour
{
    public static readonly string URL = "http://api.openweathermap.org/data/2.5/";

    [SerializeField] private UIDocument m_Document;
    [SerializeField] private string m_APIKey;

    private TextElement m_LongitudeLbl;
    private TextElement m_LocationLbl;
    private TextElement m_LatitudeLbl;
    private ProgressBar m_HumidityIdxBar;
    private ProgressBar m_FloodAlertsLbl;
    private ProgressBar m_AirPollutionIdxBar;
    private ProgressBar m_RiskIdxBar;

    public void UpateDetails(string location, double lon, double lat)
    {
        this.Start();
        this.StartCoroutine(this.ReadWeather(location, lon, lat));
        this.StartCoroutine(this.ReadPollution(lon, lat));
    }

    public void SetDocumentActive(bool active)
    {
        this.m_Document.enabled = active;
    }

    private IEnumerator ReadWeather(string location, double lon, double lat)
    {
        string weatherURL = $"{URL}weather?lon={lon}&lat={lat}&appid={this.m_APIKey}";

        UnityWebRequest fetchWeatherAPI = UnityWebRequest.Get(weatherURL);
        yield return fetchWeatherAPI.SendWebRequest();

        if (fetchWeatherAPI.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(fetchWeatherAPI.error);
        } else
        {
            Debug.Log("Fecth Weather Successfully!");
            JSONNode responseWeather = JSON.Parse(fetchWeatherAPI.downloadHandler.text);

            // this.m_LocationLbl.text = responseWeather["name"];
            this.m_LocationLbl.text = location;
            this.m_LongitudeLbl.text = responseWeather["coord"]["lon"];
            this.m_LatitudeLbl.text = responseWeather["coord"]["lat"];
            this.m_HumidityIdxBar.title = responseWeather["main"]["humidity"] + '%';
            this.m_HumidityIdxBar.value = float.Parse(responseWeather["main"]["humidity"]);
        }
       
    }

    private IEnumerator ReadPollution(double lon, double lat)
    {
        string pollutionURL = $"{URL}air_pollution?lon={lon}&lat={lat}&appid={this.m_APIKey}";

        UnityWebRequest fetchAirPollution = UnityWebRequest.Get(pollutionURL);
        yield return fetchAirPollution.SendWebRequest();

        if (fetchAirPollution.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(fetchAirPollution.error);
        } else
        {
            Debug.Log("Fetch Pollution Successfully!");
            JSONNode responsePollution = JSON.Parse(fetchAirPollution.downloadHandler.text);

            this.m_AirPollutionIdxBar.value = float.Parse(responsePollution["list"][0]["components"]["so2"]);
            this.m_AirPollutionIdxBar.title = responsePollution["list"][0]["components"]["so2"];
        }
    }

    private void Start()
    {
        UIManager.Instance.SideBarUI = this;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        this.m_LocationLbl = root.Q<TextElement>("location-lbl");
        this.m_LongitudeLbl = root.Q<TextElement>("longitude-lbl");
        this.m_LatitudeLbl = root.Q<TextElement>("latitude-lbl");
        this.m_FloodAlertsLbl = root.Q<ProgressBar>("flood-alerts-bar");
        this.m_AirPollutionIdxBar = root.Q<ProgressBar>("api-bar");
        this.m_HumidityIdxBar = root.Q<ProgressBar>("humid-idx-bar");
        this.m_RiskIdxBar = root.Q<ProgressBar>("risk-idx-bar");
    }
}
