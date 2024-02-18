using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

namespace OmniTwin
{
    using UI;

    public class DisasterIndicator : MonoBehaviour
    {
        [System.Serializable]
        private struct DisasterIcon
        {
            public string Name;
            public Texture2D Icon;
        }

        public static readonly string URL = "https://firestore.googleapis.com/v1/projects/mranti-a39d6/databases/(default)/documents/detection";

        [SerializeField] private DisasterIndicatorUI m_IndicatorPrefab;
        [SerializeField] private DisasterIcon[] m_Icons;

        private Dictionary<string, Texture2D> m_IconMaps;

        private DisasterIndicatorUI[] m_IndicatorUIPool;

        private void OnEnable()
        {
            this.StartCoroutine(this.ReadDisasterData());
            this.m_IconMaps = new Dictionary<string, Texture2D>(this.m_Icons.Length);

            for (int i = 0; i < this.m_Icons.Length; i++)
            {
                DisasterIcon icon = this.m_Icons[i];
                this.m_IconMaps.Add(icon.Name, icon.Icon);
            }
        }

        private IEnumerator ReadDisasterData()
        {
            UnityWebRequest request = UnityWebRequest.Get(URL);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                JSONNode response = JSON.Parse(request.downloadHandler.text);

                JSONNode documents = response["documents"];
                DisasterData[] disasterData = new DisasterData[documents.Count];

                for (int f = 0; f < disasterData.Length; f++)
                {
                    JSONNode fields = documents[f]["fields"];
                    double lat = double.Parse(fields["latitude"]["stringValue"]);
                    double lon = double.Parse(fields["longitude"]["stringValue"]);
                    string imageURL = fields["detectionImageUrl"]["stringValue"];
                    string category = fields["categories"]["stringValue"];

                    disasterData[f] = new DisasterData(lat, lon, imageURL, category);
                }

                // disable all existing indicators and create new ones
                if (this.m_IndicatorUIPool != null)
                {
                    for (int i = 0; i < this.m_IndicatorUIPool.Length; i++)
                    {
                        if (this.m_IndicatorUIPool[i] != null)
                        {
                            this.m_IndicatorUIPool[i].gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    // initialize pool array
                    this.m_IndicatorUIPool = new DisasterIndicatorUI[disasterData.Length];
                    // instantiate game objects
                    for (int i = 0; i < this.m_IndicatorUIPool.Length; i++)
                    {
                        this.m_IndicatorUIPool[i] = Object.Instantiate(this.m_IndicatorPrefab, this.transform);
                        this.m_IndicatorUIPool[i].gameObject.SetActive(false);
                    }
                }

                // increase m_IndicatorUIs array size to accomodate more game objects
                if (this.m_IndicatorUIPool.Length < disasterData.Length)
                {
                    DisasterIndicatorUI[] tempIndicators = new DisasterIndicatorUI[disasterData.Length];

                    // copy original pool to larger pool
                    for (int i = 0; i < this.m_IndicatorUIPool.Length; i++)
                    {
                        tempIndicators[i] = this.m_IndicatorUIPool[i];
                    }

                    // fill in larger pool by instantiating new game objects
                    for (int i = this.m_IndicatorUIPool.Length; i < disasterData.Length; i++)
                    {
                        tempIndicators[i] = Object.Instantiate(this.m_IndicatorPrefab, this.transform);
                        tempIndicators[i].gameObject.SetActive(false);
                    }

                    // replace origin pool with larger pool
                    this.m_IndicatorUIPool = tempIndicators;
                }

                // initialize disaster ui with data
                for (int f = 0; f < disasterData.Length; f++)
                {
                    DisasterData data = disasterData[f];

                    // set icon if available
                    Texture2D icon;
                    Debug.Log(data.Category);
                    if (data.Category != null)
                    {
                        this.m_IconMaps.TryGetValue(data.Category, out icon);
                        if (icon != null)
                        {
                            DisasterIndicatorUI indicatorUI = this.m_IndicatorUIPool[f];
                            indicatorUI.gameObject.SetActive(true);
                            indicatorUI.Init(data);
                            indicatorUI.SetIcon(icon);
                        }
                    }
                }
            }
        }
    }
}
