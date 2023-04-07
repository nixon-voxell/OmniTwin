using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class OpenStreetMapTileLoader : MonoBehaviour
{
    public string url = "http://tile.openstreetmap.org/{0}/{1}/{2}.png";
    public int zoom = 14;
    public Vector2 latLon = new Vector2(51.5074f, 0.1278f);

    void Start()
    {
        StartCoroutine(LoadTiles());
    }

    IEnumerator LoadTiles()
    {
        int tileX;
        int tileY;
        // latLon.GetTileXY(zoom, out tileX, out tileY);
        Vector2Extensions.LatLongToPixelXYOSM(latLon.x, latLon.y, zoom, out tileX, out tileY);
        int surroundTile = 1;

        for (int x = tileX - surroundTile; x <= tileX + surroundTile; x++)
        {
            for (int y = tileY - surroundTile; y <= tileY + surroundTile; y++)
            {
                string tileUrl = string.Format(url, zoom, x, y);
                WWW www = new WWW(tileUrl);
                yield return www;
                Texture2D texture = new Texture2D(256, 256);
                www.LoadImageIntoTexture(texture);
                // GameObject tile = new GameObject("Tile " + x + ", " + y);
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

    Mesh CreateMesh(int x, int y)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3 (x * 256f, y * 256f),
            new Vector3 ((x + 1) * 256f, y * 256f),
            new Vector3 ((x + 1) * 256f, (y + 1) * 256f),
            new Vector3 (x * 256f, (y + 1) * 256f)
        };
        mesh.uv = new Vector2[] {
            new Vector2 (0f, 0f),
            new Vector2 (1f, 0f),
            new Vector2 (1f, 1f),
            new Vector2 (0f, 1f)
        };
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        return mesh;
    }
}

public static class Vector2Extensions
{
    public static void GetTileXY(this Vector2 latLon, int zoom, out int xTile, out int yTile)
    {
        float pi = Mathf.PI;
        float sinLatitude = Mathf.Sin(latLon.x * pi / 180f);
        float pixelX = ((latLon.y + 180f) / 360f) * (float)(1 << zoom);
        float pixelY = (0.5f - Mathf.Log(((1f + sinLatitude) / (1f - sinLatitude))) / (4f * pi)) * (1 << zoom);
        xTile = Mathf.FloorToInt((float)(pixelX / 256d));
        yTile = Mathf.FloorToInt((float)(pixelY / 256d));
    }

    public static void LatLongToPixelXYOSM(float latitude, float longitude, int zoomLevel, out int tilex, out int tiley)
    {
        // Dim MinLatitude = -85.05112878
        // Dim MaxLatitude = 85.05112878
        // Dim MinLongitude = -180
        // Dim MaxLongitude = 180
        float mapSize = Mathf.Pow(2, zoomLevel) * 256;

        // latitude = Clip(latitude, MinLatitude, MaxLatitude)
        // longitude = Clip(longitude, MinLongitude, MaxLongitude)

        Vector2 p = new Vector2();
        p.x = (longitude + 180.0f) / 360.0f * (1 << zoomLevel);
        p.y = (1.0f - Mathf.Log(Mathf.Tan(latitude * Mathf.PI / 180.0f) + 1.0f / Mathf.Cos(math.radians(latitude))) / Mathf.PI) / 2.0f * (1 << zoomLevel);

        tilex = (int)p.x;
        tiley = (int)p.y;
        // pixelX = (int)ClipByRange((tilex * 256) + ((p.x - tilex) * 256), mapSize - 1);
        // pixelY = (int)ClipByRange((tiley * 256) + ((p.y - tiley) * 256), mapSize - 1);
    }

    private static float ClipByRange(float n, float range)
    {
        return n % range;
    }
}
