using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

public static class FloodSimulation
{
    public static ComputeShader cs_DepthToHeight { get; private set; }
    public static ComputeShader cs_AddWater { get; private set; }
    public static ComputeShader cs_CalculateWaterHeight { get; private set; }
    public static ComputeShader cs_WaterHeightToTexture { get; private set; }
    public static ComputeShader cs_PropagateWater { get; private set; }

    public static void Initialize()
    {
        cs_DepthToHeight = Resources.Load<ComputeShader>("Computes/DepthToHeight");
        cs_AddWater = Resources.Load<ComputeShader>("Computes/AddWater");
        cs_CalculateWaterHeight = Resources.Load<ComputeShader>("Computes/CalculateWaterHeight");
        cs_WaterHeightToTexture = Resources.Load<ComputeShader>("Computes/WaterHeightToTexture");
        cs_PropagateWater = Resources.Load<ComputeShader>("Computes/PropagateWater");
    }

    public static void Compute_DepthToHeight(
        CommandBuffer cmd,
        float height,
        FloodBuffer floodBuffer
    ) {
        cmd.BeginSample("DepthToHeight");

        int texWidth = floodBuffer.tex_Depth.width;
        int texHeight = floodBuffer.tex_Depth.height;
        cmd.SetComputeIntParams(cs_DepthToHeight, ShaderID._Dimension, texWidth, texWidth);
        cmd.SetComputeFloatParam(cs_DepthToHeight, ShaderID._Height, height);

        cmd.SetComputeTextureParam(cs_DepthToHeight, 0, ShaderID.tex_Depth, floodBuffer.tex_Depth);
        cmd.SetComputeBufferParam(cs_DepthToHeight, 0, ShaderID.gb_Heights, floodBuffer.gb_Heights);

        cmd.DispatchCompute(
            cs_DepthToHeight, 0,
            mathx.batch_count(texWidth, 8),
            mathx.batch_count(texHeight, 8), 1
        );

        cmd.EndSample("DepthToHeight");
    }

    public static void Compute_AddWater(
        CommandBuffer cmd,
        int count, float radius, float2 center, uint seed,
        FloodBuffer floodBuffer
    ) {
        cmd.BeginSample("AddWater");

        cmd.SetComputeIntParam(cs_AddWater, ShaderID._ThreadCount, count);
        cmd.SetComputeFloatParam(cs_AddWater, ShaderID._Radius, radius);
        cmd.SetComputeVectorParam(cs_AddWater, ShaderID._Center, new Vector4(center.x, center.y, 0.0f, 0.0f));
        cmd.SetComputeIntParam(cs_AddWater, ShaderID._Seed, (int)seed);

        cmd.SetComputeBufferParam(cs_AddWater, 0, ShaderID.gb_Heights, floodBuffer.gb_Heights);
        cmd.SetComputeBufferParam(cs_AddWater, 0, ShaderID.gb_WaterCoords, floodBuffer.gb_WaterCoords);

        cmd.DispatchCompute(
            cs_AddWater, 0,
            mathx.batch_count(count, 64), 1, 1
        );

        cmd.EndSample("AddWater");
    }

    public static void Compute_CalculateWaterHeight(CommandBuffer cmd, int count, FloodBuffer floodBuffer)
    {
        cmd.BeginSample("CalculateWaterHeight");

        int width = floodBuffer.tex_WaterHeight.width;
        int height = floodBuffer.tex_WaterHeight.height;
        cmd.SetComputeIntParam(cs_CalculateWaterHeight, ShaderID._ThreadCount, count);
        cmd.SetComputeIntParams(cs_CalculateWaterHeight, ShaderID._Dimension, width, height);

        cmd.SetComputeBufferParam(cs_CalculateWaterHeight, 0, ShaderID.gb_WaterHeights, floodBuffer.gb_WaterHeights);
        cmd.SetComputeBufferParam(cs_CalculateWaterHeight, 0, ShaderID.gb_WaterCoords, floodBuffer.gb_WaterCoords);

        cmd.DispatchCompute(
            cs_CalculateWaterHeight, 0,
            mathx.batch_count(width, 8),
            mathx.batch_count(height, 8), 1
        );

        cmd.EndSample("CalculateWaterHeight");
    }

    public static void Compute_WaterHeightToTexture(CommandBuffer cmd, FloodBuffer floodBuffer)
    {
        cmd.BeginSample("WaterHeightToTexture");

        int width = floodBuffer.tex_WaterHeight.width;
        int height = floodBuffer.tex_WaterHeight.height;
        cmd.SetComputeIntParams(cs_WaterHeightToTexture, ShaderID._Dimension, width, height);

        cmd.SetComputeBufferParam(cs_WaterHeightToTexture, 0, ShaderID.gb_WaterHeights, floodBuffer.gb_WaterHeights);
        cmd.SetComputeTextureParam(cs_WaterHeightToTexture, 0, ShaderID.tex_WaterHeight, floodBuffer.tex_WaterHeight);

        cmd.DispatchCompute(
            cs_WaterHeightToTexture, 0,
            mathx.batch_count(width, 8),
            mathx.batch_count(height, 8), 1
        );

        cmd.EndSample("WaterHeightToTexture");
    }

    /// <summary>Setup camera to render depth.</summary>
    public static void SetupCamera(Camera camera, RenderTexture tex_depth)
    {
        camera.depthTextureMode = DepthTextureMode.Depth;
        camera.targetTexture = tex_depth;
    }
}
