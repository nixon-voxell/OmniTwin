using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Mathematics;
using TMPro;

public class OpenStreetMapTest : MonoBehaviour
{
    [SerializeField] private string m_OpenStreetMapURL = "https://www.openstreetmap.org/api";
    // [SerializeField] private MinMaxAABB m_OsmAabb;

    [SerializeField] private float m_OsmVersion;
    [SerializeField] private TextMeshProUGUI m_BoundsDataTMPro;

    private XmlDocument m_Doc;
    private XmlNode m_XmlAabb;
    private XmlNodeList m_XmlNodes;
    private XmlNodeList m_XmlWays;

    private IEnumerator Start()
    {
        this.StartCoroutine(this.RequestVersion());
        // this.StartCoroutine(this.RequestBounds(this.m_OsmAabb, 0.6f));

        while (this.m_Doc == null)
        {
            yield return new WaitForEndOfFrame();
        }

        this.m_XmlAabb = this.m_Doc.SelectSingleNode("/osm/bounds");
        this.m_XmlNodes = this.m_Doc.SelectNodes("/osm/node");
        this.m_XmlWays = this.m_Doc.SelectNodes("/osm/way");

        Debug.Log(this.m_XmlNodes.Count);
        Debug.Log(this.m_XmlWays.Count);
    }

    private void Update()
    {
        if (this.m_Doc == null) return;
    }

    private IEnumerator RequestVersion()
    {
        // https://www.openstreetmap.org/api/versions
        string requestURL = $"{this.m_OpenStreetMapURL}/versions";

        UnityWebRequest request = UnityWebRequest.Get(requestURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            request.Dispose();
        } else
        {
            Debug.Log(request.downloadHandler.text);
            request.Dispose();
        }
    }

    // private IEnumerator RequestBounds(MinMaxAABB OSMBounds, float version)
    // {
    //     // https://www.openstreetmap.org/api/0.6/map?bbox=0.5,0.5,0.6,0.6
    //     float2 minBound = OSMBounds.Min.xy;
    //     float2 maxBound = OSMBounds.Max.xy;

    //     string requestURL = $"{this.m_OpenStreetMapURL}/{version}/map?bbox={minBound.x},{minBound.y},{maxBound.x},{maxBound.y}";

    //     UnityWebRequest request = UnityWebRequest.Get(requestURL);
    //     yield return request.SendWebRequest();

    //     if (request.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log(request.error);
    //         request.Dispose();
    //     } else
    //     {
    //         string rawXmlText = request.downloadHandler.text;

    //         this.m_Doc = new XmlDocument();
    //         this.m_Doc.LoadXml(rawXmlText);

    //         Debug.Log("OSM Street Map done");

    //         request.Dispose();
    //     }
    // }
}
