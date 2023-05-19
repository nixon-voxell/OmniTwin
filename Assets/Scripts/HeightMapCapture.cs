using UnityEngine;
using Unity.Mathematics;

public class HeightMapCapture : MonoBehaviour
{
    [SerializeField] private int2 m_Size;
    [SerializeField] private Camera m_TargetCamera;
    [SerializeField] private Material m_mat_Depth;

    [SerializeField] private RenderTexture m_tex_HeightMap;

    private void Start()
    {
        // Initialize the RenderTexture
        m_tex_HeightMap = new RenderTexture(this.m_Size.x, this.m_Size.y, 32, RenderTextureFormat.Depth);
        m_tex_HeightMap.Create();
    }

    private void Update()
    {
        this.Render();
    }

    private void Render()
    {
        m_TargetCamera.depthTextureMode = DepthTextureMode.Depth;
        RenderTexture.active = m_tex_HeightMap;
        m_TargetCamera.targetTexture = m_tex_HeightMap;
        m_TargetCamera.Render();

        this.m_mat_Depth.SetTexture("_DepthTexture", this.m_tex_HeightMap);
    }
}
