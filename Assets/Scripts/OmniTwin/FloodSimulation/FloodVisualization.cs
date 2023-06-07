using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace OmniTwin
{
    public class FloodVisualization : MonoBehaviour
    {
        [Tooltip("Resolution of flood simulation.")]
        [SerializeField] private uint2 m_Size;
        [SerializeField] private uint m_PropagateSteps;

        [Header("Water")]
        [Tooltip("Maximum number of water blocks.")]
        [SerializeField] private uint m_MaxWaterBlockCount;
        // current number of water blocks
        private uint m_WaterBlockCount;

        [Tooltip("Nunber of water blocks to add per frame.")]
        [SerializeField] private uint m_AddBlockWaterCount;
        [Tooltip("Height of a single water block.")]
        [SerializeField] private float m_WaterBlockHeight;

        [SerializeField] private float m_Radius;
        [SerializeField] private float2 m_Center;

        [Header("Simulation")]
        [SerializeField] private float m_RandomStrength;
        [SerializeField] private float m_PropagateSpeed;

        [Header("Visualization")]
        [SerializeField] private Camera m_TargetCamera;

        [SerializeField] private Material m_mat_Depth;
        [SerializeField] private Material m_mat_WaterHeight;
        [SerializeField] private Material m_mat_Water;

        private FloodBuffer m_FloodBuffer;

        // properties
        public int2 Size => (int2)this.m_Size;
        public int Count => this.Size.x * this.Size.y;

        private void Awake()
        {
            FloodSimulation.Initialize();
        }

        private void Start()
        {
            this.m_FloodBuffer = new FloodBuffer(this.m_Size, this.m_MaxWaterBlockCount);
            this.m_WaterBlockCount = 0;

            FloodSimulation.SetupCamera(this.m_TargetCamera, this.m_FloodBuffer.tex_Depth);
        }

        private void Update()
        {
            uint seed = (uint)UnityEngine.Random.Range(-123456789, 123456789);
            this.m_TargetCamera.Render();

            CommandBuffer cmd = new CommandBuffer();
            FloodSimulation.Compute_DepthToHeight(cmd, this.m_TargetCamera.farClipPlane, this.m_FloodBuffer);

            if (this.m_WaterBlockCount <= this.m_MaxWaterBlockCount - this.m_AddBlockWaterCount)
            {
                if (this.m_AddBlockWaterCount > 0)
                {
                    FloodSimulation.Compute_AddWaterBlock(
                        cmd,
                        this.m_AddBlockWaterCount, this.m_WaterBlockCount,
                        this.m_Radius, this.m_Center, seed,
                        this.m_FloodBuffer
                    );
                    this.m_WaterBlockCount += this.m_AddBlockWaterCount;
                }
            }

            if (this.m_WaterBlockCount > 0)
            {
                FloodSimulation.Compute_CalculateWaterHeight(cmd, this.m_WaterBlockCount, this.m_FloodBuffer);

                FloodSimulation.Compute_WaterHeightToTexture(cmd, this.m_WaterBlockHeight, this.m_FloodBuffer);

                for (uint p = 0; p < this.m_PropagateSteps; p++)
                {
                    FloodSimulation.Compute_PropagateWater(
                        cmd,
                        this.m_WaterBlockCount, this.m_WaterBlockHeight,
                        this.m_PropagateSpeed, this.m_RandomStrength, seed + 1,
                        this.m_FloodBuffer
                    );
                }
            }

            Graphics.ExecuteCommandBuffer(cmd);

            this.m_mat_Depth.SetTexture(ShaderID.tex_Depth, this.m_FloodBuffer.tex_Depth);
            this.m_mat_WaterHeight.SetTexture(ShaderID.tex_WaterHeight, this.m_FloodBuffer.tex_WaterHeight);

            // this.m_mat_Water.SetTexture(ShaderID.tex_Depth, this.m_FloodBuffer.tex_Depth);
            // this.m_mat_Water.SetTexture(ShaderID.tex_WaterHeight, this.m_FloodBuffer.tex_WaterHeight);
        }

        private void OnDestroy()
        {
            if (this.m_FloodBuffer != null)
            {
                this.m_FloodBuffer.Dispose();
            }
        }
    }
}
