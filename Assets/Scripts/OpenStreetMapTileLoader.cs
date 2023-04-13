using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Unity.Mathematics;

public class OpenStreetMapTileLoader : MonoBehaviour
{
    public string url = "http://tile.openstreetmap.org/{0}/{1}/{2}.png";
    public int Zoom = 14;
    public float2 GeoCoordinate = new float2(3.157764f, 101.7119f);

    private IEnumerator Start()
    {
        Debug.Log("Start");
        int2 tile = LatLongToPixelXYOSM(this.GeoCoordinate, this.Zoom);
        int surroundTile = 1;

        Debug.Log(tile);
        for (int x = tile.x - surroundTile; x <= tile.x + surroundTile; x++)
        {
            for (int y = tile.y - surroundTile; y <= tile.y + surroundTile; y++)
            {
                Debug.Log(x);
                Debug.Log(y);
                Texture2D texture;
                string tileUrl = string.Format(url, this.Zoom, x, y);
                // persistentDataPath = C:/Users/cheng/AppData/LocalLow/APU_CREDIT/OmniTwin
                string tilePath = $"{Application.persistentDataPath}/{tileUrl}.osmtile";

                if (File.Exists(tilePath))
                {
                    Debug.Log($"Reading tile from path: {tilePath}");
                    texture = new Texture2D(256, 256);
                    // load texture from file
                    texture.LoadRawTextureData(File.ReadAllBytes(tilePath));
                } else
                {
                    UnityWebRequest request = UnityWebRequestTexture.GetTexture(tileUrl);
                    yield return request;

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(request.error);
                        request.Dispose();
                        texture = null;
                    } else
                    {
                        Debug.Log($"Loading tile from url: {tileUrl}");
                        texture = DownloadHandlerTexture.GetContent(request);
                        // write texture to file
                        Debug.Log($"Writing tile from url: {tilePath}");
                        File.WriteAllBytes(tilePath, texture.GetRawTextureData());
                        request.Dispose();
                    }
                }

                GameObject tileQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tileQuad.name = $"Tile {x}, {y}";
                tileQuad.transform.parent = transform;
                tileQuad.transform.localPosition = new Vector3(x - tile.x, -(y - tile.y), 0.0f);

                MeshFilter meshFilter = tileQuad.GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = tileQuad.GetComponent<MeshRenderer>();
                Material material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                material.mainTexture = texture;
                meshRenderer.material = material;
            }
        }
    }

    /*
    private IEnumerator LoadTiles()
    {
        int tileX;
        int tileY;
        // latLon.GetTileXY(zoom, out tileX, out tileY);
        LatLongToPixelXYOSM(GeoCoordinate.x, GeoCoordinate.y, Zoom, out tileX, out tileY);
        int surroundTile = 1;

        for (int x = tileX - surroundTile; x <= tileX + surroundTile; x++)
        {
            for (int y = tileY - surroundTile; y <= tileY + surroundTile; y++)
            {
                string tileUrl = string.Format(url, Zoom, x, y);
                WWW www = new WWW(tileUrl);
                yield return www;

                Texture2D texture = new Texture2D(256, 256);
                www.LoadImageIntoTexture(texture);
                // Debug.Log(texture.GetRawTextureData().Length);

                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.name = $"Tile {x}, {y}";
                tile.transform.parent = transform;
                tile.transform.localPosition = new Vector3(x - tileX, -(y - tileY), 0.0f);

                MeshFilter meshFilter = tile.GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = tile.GetComponent<MeshRenderer>();
                Material material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                material.mainTexture = texture;
                meshRenderer.material = material;
            }
        }
    }
    */

    public static int2 LatLongToPixelXYOSM(float2 geoCoordinate, int zoomLevel)
    {
        float mapSize = Mathf.Pow(2, zoomLevel) * 256;

        float2 p;
        p.x = (geoCoordinate.x + 180.0f) / 360.0f * (1 << zoomLevel);
        p.y = (1.0f - math.log(math.tan(geoCoordinate.y * math.PI / 180.0f) + 1.0f / math.cos(math.radians(geoCoordinate.y))) / math.PI) / 2.0f * (1 << zoomLevel);

        return (int2)p;
    }
}
