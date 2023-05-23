using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Voxell.Util;

public class FloodVisualization : MonoBehaviour
{
    [SerializeField] private uint2 m_Size;
    [SerializeField] private uint m_MaxWaterCount;
    // nunber of water blocks to add per frame
    [SerializeField] private uint m_AddWaterCount;
    [SerializeField] private float m_Radius;
    [SerializeField] private float2 m_Center;
    [SerializeField, InspectOnly] private uint m_WaterCount;
    [SerializeField] private Camera m_TargetCamera;

    [SerializeField] private Material m_mat_Height;
    [SerializeField] private Material m_mat_WaterHeight;
    private FloodBuffer m_FloodBuffer;

    public int2 Size => (int2)this.m_Size;
    public int Count => this.Size.x * this.Size.y;

    private void Awake()
    {
        FloodSimulation.Initialize();
    }

    private void Start()
    {
        this.m_FloodBuffer = new FloodBuffer(this.m_Size, this.m_MaxWaterCount);
        this.m_WaterCount = 0;

        // FloodSimulation.SetupCamera(this.m_TargetCamera, this.m_FloodBuffer.tex_Depth);

        CommandBuffer cmd = new CommandBuffer();

        FloodSimulation.Compute_AddWater(
            cmd,
            (int)this.m_AddWaterCount, this.m_Radius, this.m_Center, 0,
            this.m_FloodBuffer
        );
        this.m_WaterCount += this.m_AddWaterCount;

        FloodSimulation.Compute_CalculateWaterHeight(cmd, (int)this.m_WaterCount, this.m_FloodBuffer);

        FloodSimulation.Compute_WaterHeightToTexture(cmd, this.m_FloodBuffer);

        Graphics.ExecuteCommandBuffer(cmd);
    }

    private void OnDestroy()
    {
        if (this.m_FloodBuffer != null)
        {
            this.m_FloodBuffer.Dispose();
        }
    }
}
