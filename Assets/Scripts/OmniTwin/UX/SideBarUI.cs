using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using SimpleJSON;

namespace OmniTwin.UI
{
    public class SideBarUI : MonoBehaviour, IVisibility
    {
        public static readonly string URL = "http://api.openweathermap.org/data/2.5/";

        [SerializeField] private UIDocument m_Document;
        [SerializeField] private string m_APIKey;

        private VisualElement m_Root;
        private Button m_CloseBtn;
        private TextElement m_LongitudeLbl;
        private TextElement m_LocationLbl;
        private TextElement m_LatitudeLbl;
        private ProgressBar m_HumidityIdxBar;
        private ProgressBar m_FloodAlertsLbl;
        private ProgressBar m_AirPollutionIdxBar;
        private ProgressBar m_RiskIdxBar;

        public void UpateDetails(string location, double lon, double lat)
        {
            this.StartCoroutine(this.ReadWeather(location, lon, lat));
            this.StartCoroutine(this.ReadPollution(lon, lat));
        }

        public void SetVisible(bool active)
        {
            this.m_Root.visible = active;
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
                JSONNode responsePollution = JSON.Parse(fetchAirPollution.downloadHandler.text);

                this.m_AirPollutionIdxBar.value = float.Parse(responsePollution["list"][0]["components"]["so2"]);
                this.m_AirPollutionIdxBar.title = responsePollution["list"][0]["components"]["so2"];
            }
        }

        private void Start()
        {
            UXManager manager = UXManager.Instance;
            manager.SideBarUI = this;

            this.m_Root = this.m_Document.rootVisualElement;

            this.m_CloseBtn = this.m_Root.Q<Button>("close-btn");
            this.m_LocationLbl = this.m_Root.Q<TextElement>("location-lbl");
            this.m_LongitudeLbl = this.m_Root.Q<TextElement>("longitude-lbl");
            this.m_LatitudeLbl = this.m_Root.Q<TextElement>("latitude-lbl");
            this.m_FloodAlertsLbl = this.m_Root.Q<ProgressBar>("flood-alerts-bar");
            this.m_AirPollutionIdxBar = this.m_Root.Q<ProgressBar>("api-bar");
            this.m_HumidityIdxBar = this.m_Root.Q<ProgressBar>("humid-idx-bar");
            this.m_RiskIdxBar = this.m_Root.Q<ProgressBar>("risk-idx-bar");

            this.m_CloseBtn.clicked += () => manager.SetUIInactive(this);

            // set visibility to false by default
            this.SetVisible(false);
        }
    }
}
