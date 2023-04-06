using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Mathematics;
using TMPro;

public class OpenStreetMap : MonoBehaviour
{
    [SerializeField] private string m_OpenStreetMapURL = "https://www.openstreetmap.org/api";
    [SerializeField] private MinMaxAABB m_OSMAABB;

    [SerializeField] private float m_OSMVersion;
    [SerializeField] private TextMeshProUGUI m_BoundsDataTMPro;

    private void Start()
    {
        this.StartCoroutine(this.RequestVersion());
        this.StartCoroutine(this.RequestBounds(this.m_OSMAABB, 0.6f));
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

    private IEnumerator RequestBounds(MinMaxAABB OSMBounds, float version)
    {
        // https://www.openstreetmap.org/api/0.6/map?bbox=0.5,0.5,0.6,0.6
        float2 minBound = OSMBounds.Min.xy;
        float2 maxBound = OSMBounds.Max.xy;

        string requestURL = $"{this.m_OpenStreetMapURL}/{version}/map?bbox={minBound.x},{minBound.y},{maxBound.x},{maxBound.y}";

        UnityWebRequest request = UnityWebRequest.Get(requestURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            request.Dispose();
        } else
        {
            this.m_BoundsDataTMPro.text = request.downloadHandler.text[^100..^1];
            request.Dispose();
        }
    }
}
