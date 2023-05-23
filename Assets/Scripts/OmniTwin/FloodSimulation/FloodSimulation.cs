using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

public static class FloodSimulation
{
    public static ComputeShader cs_AddWater { get; private set; }
    public static ComputeShader cs_CalculateWaterHeight { get; private set; }
    public static ComputeShader cs_DepthToHeight { get; private set; }
    public static ComputeShader cs_PropagateWater { get; private set; }

    public static void Initialize()
    {
        cs_AddWater = Resources.Load<ComputeShader>("Computes/AddWater");
        cs_CalculateWaterHeight = Resources.Load<ComputeShader>("Computes/CalculateWaterHeight");
        cs_DepthToHeight = Resources.Load<ComputeShader>("Computes/DepthToHeight");
        cs_PropagateWater = Resources.Load<ComputeShader>("Computes/PropagateWater");
    }

    public static void Compute_AddWater(
        CommandBuffer cmd,
        int count, float radius, float2 center, uint seed,
        GraphicsBuffer gb_Height, GraphicsBuffer gb_WaterCoords
    ) {
        cmd.SetComputeIntParam(cs_AddWater, ShaderID._ThreadCount, count);
        cmd.SetComputeFloatParam(cs_AddWater, ShaderID._Radius, radius);
        cmd.SetComputeVectorParam(cs_AddWater, ShaderID._Center, new Vector4(center.x, center.y, 0.0f, 0.0f));
        cmd.SetComputeIntParam(cs_AddWater, ShaderID._Seed, (int)seed);

        cmd.DispatchCompute(cs_AddWater, 0, mathx.batch_count(count, 64), 1, 1);
    }

    /// <summary>Create depth render texture.</summary>
    private static RenderTexture CreateTexture(int2 size, int depthBits = 32)
    {
        RenderTexture tex_depth = new RenderTexture(size.x, size.y, depthBits, RenderTextureFormat.Depth);
        tex_depth.Create();
        return tex_depth;
    }

    /// <summary>Setup camera to render depth.</summary>
    private static void SetupCamera(Camera camera, RenderTexture tex_depth)
    {
        camera.depthTextureMode = DepthTextureMode.Depth;
        camera.targetTexture = tex_depth;
    }
}
