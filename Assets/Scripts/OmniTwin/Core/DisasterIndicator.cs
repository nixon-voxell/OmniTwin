using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public struct DisasterData
{
    public double Latitude;
    public double Longitude;
    public string DetectionImageURL;
    public string Category;

    public DisasterData(double lat, double lon, string imageURL, string category)
    {
        this.Latitude = lat;
        this.Longitude = lon;
        this.DetectionImageURL = imageURL;
        this.Category = category;
    }
}

public class DisasterIndicator : MonoBehaviour
{
    public static readonly string URL = "https://firestore.googleapis.com/v1/projects/mranti-a39d6/databases/(default)/documents/detection";

    [SerializeField] private DisasterIndicatorUI m_IndicatorPrefab;

    private DisasterIndicatorUI[] m_IndicatorUIPool;

    private void OnEnable()
    {
        this.StartCoroutine(this.ReadDisasterData());
    }

    private IEnumerator ReadDisasterData()
    {
        UnityWebRequest request = UnityWebRequest.Get(URL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        } else
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
            } else
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
                this.m_IndicatorUIPool[f].gameObject.SetActive(true);
                this.m_IndicatorUIPool[f].Init(disasterData[f]);
            }
        }
    }
}
