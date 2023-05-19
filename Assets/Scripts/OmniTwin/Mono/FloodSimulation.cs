using UnityEngine;
using Unity.Mathematics;
using CesiumForUnity;

public class FloodSimulation : MonoBehaviour
{
    [SerializeField] private int2 m_Size;
    [SerializeField] private Camera m_HeightCamera;
    [SerializeField] private RenderTexture m_tex_Depth;
    [SerializeField] private Texture2D m_tex_HeigthMap;

    private void Start()
    {
        this.m_tex_Depth = new RenderTexture(this.m_Size.x, this.m_Size.y, 32, RenderTextureFormat.Depth);

        // setup camera to render depth
        this.m_HeightCamera.depthTextureMode = DepthTextureMode.Depth;
        this.m_HeightCamera.targetTexture = this.m_tex_Depth;

        this.m_tex_HeigthMap = new Texture2D(this.m_Size.x, this.m_Size.y, TextureFormat.RFloat, false);
    }

    [ContextMenu("Render")]
    private void Render()
    {
        this.m_HeightCamera.Render();
        RenderTexture.active = this.m_tex_Depth;
        this.m_tex_HeigthMap.ReadPixels(new Rect(0, 0, this.m_tex_HeigthMap.width, this.m_tex_HeigthMap.height), 0, 0);
        this.m_tex_HeigthMap.Apply();
        RenderTexture.active = null;
    }

    private void OnDestroy()
    {
        this.m_tex_Depth.Release();
        Object.Destroy(this.m_tex_HeigthMap);
    }
}
