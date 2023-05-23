using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

public static class FloodSimulation
{
    public static ComputeShader cs_DepthToHeight { get; private set; }
    public static ComputeShader cs_AddWater { get; private set; }
    public static ComputeShader cs_CalculateWaterHeight { get; private set; }
    public static ComputeShader cs_PropagateWater { get; private set; }

    public static void Initialize()
    {
        cs_DepthToHeight = Resources.Load<ComputeShader>("Computes/DepthToHeight");
        cs_AddWater = Resources.Load<ComputeShader>("Computes/AddWater");
        cs_CalculateWaterHeight = Resources.Load<ComputeShader>("Computes/CalculateWaterHeight");
        cs_PropagateWater = Resources.Load<ComputeShader>("Computes/PropagateWater");
    }

    public static void Compute_DepthToHeight(
        CommandBuffer cmd,
        float height,
        Texture tex_depth, GraphicsBuffer gb_heights
    ) {
        cmd.BeginSample("DepthToHeight");

        int texWidth = tex_depth.width;
        int texHeight = tex_depth.height;
        cmd.SetComputeIntParams(cs_DepthToHeight, ShaderID._Dimension, texWidth, texWidth);
        cmd.SetComputeFloatParam(cs_DepthToHeight, ShaderID._Height, height);

        cmd.SetComputeTextureParam(cs_DepthToHeight, 0, ShaderID.tex_Depth, tex_depth);
        cmd.SetComputeBufferParam(cs_DepthToHeight, 0, ShaderID.gb_Heights, gb_heights);

        cmd.DispatchCompute(
            cs_DepthToHeight, 0,
            mathx.batch_count(texWidth, 8), mathx.batch_count(texHeight, 8), 1
        );

        cmd.EndSample("DepthToHeight");
    }

    public static void Compute_AddWater(
        CommandBuffer cmd,
        int count, float radius, float2 center, uint seed,
        GraphicsBuffer gb_heights, GraphicsBuffer gb_waterCoords
    ) {
        cmd.BeginSample("AddWater");

        cmd.SetComputeIntParam(cs_AddWater, ShaderID._ThreadCount, count);
        cmd.SetComputeFloatParam(cs_AddWater, ShaderID._Radius, radius);
        cmd.SetComputeVectorParam(cs_AddWater, ShaderID._Center, new Vector4(center.x, center.y, 0.0f, 0.0f));
        cmd.SetComputeIntParam(cs_AddWater, ShaderID._Seed, (int)seed);

        cmd.SetComputeBufferParam(cs_AddWater, 0, ShaderID.gb_Heights, gb_heights);
        cmd.SetComputeBufferParam(cs_AddWater, 0, ShaderID.gb_WaterCoords, gb_waterCoords);

        cmd.DispatchCompute(
            cs_AddWater, 0,
            mathx.batch_count(count, 64), 1, 1
        );

        cmd.EndSample("AddWater");
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
