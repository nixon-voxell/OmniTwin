using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace OmniTwin
{
    public static class FloodSimulation
    {
        public static ComputeShader cs_IntSet { get; private set; }
        public static ComputeShader cs_DepthToHeight { get; private set; }
        public static ComputeShader cs_AddWaterBlock { get; private set; }
        public static ComputeShader cs_CalculateWaterHeight { get; private set; }
        public static ComputeShader cs_WaterHeightToTexture { get; private set; }
        public static ComputeShader cs_PropagateWater { get; private set; }
        public static ComputeShader cs_CompositeSimulation { get; private set; }

        public static void Initialize()
        {
            cs_IntSet = Resources.Load<ComputeShader>("Computes/IntSet");
            cs_DepthToHeight = Resources.Load<ComputeShader>("Computes/DepthToHeight");
            cs_AddWaterBlock = Resources.Load<ComputeShader>("Computes/AddWaterBlock");
            cs_CalculateWaterHeight = Resources.Load<ComputeShader>("Computes/CalculateWaterHeight");
            cs_WaterHeightToTexture = Resources.Load<ComputeShader>("Computes/WaterHeightToTexture");
            cs_PropagateWater = Resources.Load<ComputeShader>("Computes/PropagateWater");
            cs_CompositeSimulation = Resources.Load<ComputeShader>("Computes/CompositeSimulation");
        }

        /// <summary>Set a range of integers in the buffer to a number.</summary>
        /// <param name="start">Starting index of the buffer to set.</param>
        /// <param name="length">Number of integers to set.</param>
        public static void Compute_IntSet(
            CommandBuffer cmd,
            GraphicsBuffer gb_array,
            int number, int start, int length
        ) {
            cmd.BeginSample("IntSet");

            int threadCount = length;
            cmd.SetComputeIntParam(cs_IntSet, ShaderID._ThreadCount, threadCount);
            cmd.SetComputeIntParam(cs_IntSet, ShaderID._Start, start);
            cmd.SetComputeIntParam(cs_IntSet, ShaderID._Number, number);

            cmd.SetComputeBufferParam(cs_IntSet, 0, ShaderID.gb_Array, gb_array);

            cmd.DispatchCompute(
                cs_IntSet, 0,
                mathx.batch_count(threadCount, 64), 1, 1
            );

            cmd.EndSample("IntSet");
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

        public static void Compute_AddWaterBlock(
            CommandBuffer cmd,
            uint addWaterBlockCount, uint waterBlockCount, float radius, float2 center, uint seed,
            FloodBuffer floodBuffer
        ) {
            cmd.BeginSample("AddWaterBlock");

            cmd.SetComputeIntParam(cs_AddWaterBlock, ShaderID._ThreadCount, (int)addWaterBlockCount);
            cmd.SetComputeFloatParam(cs_AddWaterBlock, ShaderID._Radius, radius);
            cmd.SetComputeFloatParams(cs_AddWaterBlock, ShaderID._Center, center.x, center.y);
            cmd.SetComputeIntParam(cs_AddWaterBlock, ShaderID._Seed, (int)seed);
            cmd.SetComputeIntParam(cs_AddWaterBlock, ShaderID._WaterBlockCount, (int)waterBlockCount);

            cmd.SetComputeBufferParam(cs_AddWaterBlock, 0, ShaderID.gb_Heights, floodBuffer.gb_Heights);
            cmd.SetComputeBufferParam(cs_AddWaterBlock, 0, ShaderID.gb_WaterCoords, floodBuffer.gb_WaterCoords);

            cmd.DispatchCompute(
                cs_AddWaterBlock, 0,
                (int)mathx.batch_count(addWaterBlockCount, 64), 1, 1
            );

            cmd.EndSample("AddWaterBlock");
        }

        public static void Compute_CalculateWaterHeight(CommandBuffer cmd, uint waterBlockCount, FloodBuffer floodBuffer)
        {
            cmd.BeginSample("CalculateWaterHeight");

            Compute_IntSet(cmd, floodBuffer.gb_WaterHeights, 0, 0, floodBuffer.gb_WaterHeights.count);

            int width = floodBuffer.tex_WaterHeight.width;
            int height = floodBuffer.tex_WaterHeight.height;
            cmd.SetComputeIntParam(cs_CalculateWaterHeight, ShaderID._ThreadCount, (int)waterBlockCount);
            cmd.SetComputeIntParams(cs_CalculateWaterHeight, ShaderID._Dimension, width, height);

            cmd.SetComputeBufferParam(cs_CalculateWaterHeight, 0, ShaderID.gb_WaterHeights, floodBuffer.gb_WaterHeights);
            cmd.SetComputeBufferParam(cs_CalculateWaterHeight, 0, ShaderID.gb_WaterCoords, floodBuffer.gb_WaterCoords);
            cmd.SetComputeBufferParam(cs_CalculateWaterHeight, 0, ShaderID.gb_WaterBlockHeights, floodBuffer.gb_WaterBlockHeights);

            cmd.DispatchCompute(
                cs_CalculateWaterHeight, 0,
                (int)mathx.batch_count(waterBlockCount, 64), 1, 1
            );

            cmd.EndSample("CalculateWaterHeight");
        }

        public static void Compute_WaterHeightToTexture(CommandBuffer cmd, float waterBlockHeight, FloodBuffer floodBuffer)
        {
            cmd.BeginSample("WaterHeightToTexture");

            int width = floodBuffer.tex_WaterHeight.width;
            int height = floodBuffer.tex_WaterHeight.height;
            cmd.SetComputeIntParams(cs_WaterHeightToTexture, ShaderID._Dimension, width, height);
            cmd.SetComputeFloatParam(cs_WaterHeightToTexture, ShaderID._WaterBlockHeight, waterBlockHeight);

            cmd.SetComputeBufferParam(cs_WaterHeightToTexture, 0, ShaderID.gb_WaterHeights, floodBuffer.gb_WaterHeights);
            cmd.SetComputeTextureParam(cs_WaterHeightToTexture, 0, ShaderID.tex_WaterHeight, floodBuffer.tex_WaterHeight);

            cmd.DispatchCompute(
                cs_WaterHeightToTexture, 0,
                mathx.batch_count(width, 8),
                mathx.batch_count(height, 8), 1
            );

            cmd.EndSample("WaterHeightToTexture");
        }

        public static void Compute_PropagateWater(
            CommandBuffer cmd,
            uint waterBlockCount, float waterBlockHeight, float propagateSpeed, float randomStrength, uint seed,
            FloodBuffer floodBuffer
        ) {
            cmd.BeginSample("PropagateWater");

            int width = floodBuffer.tex_WaterHeight.width;
            int height = floodBuffer.tex_WaterHeight.height;
            cmd.SetComputeIntParam(cs_PropagateWater, ShaderID._ThreadCount, (int)waterBlockCount);
            cmd.SetComputeIntParams(cs_PropagateWater, ShaderID._Dimension, width, height);
            cmd.SetComputeFloatParam(cs_PropagateWater, ShaderID._WaterBlockHeight, waterBlockHeight);
            cmd.SetComputeFloatParam(cs_PropagateWater, ShaderID._PropagateSpeed, propagateSpeed);
            cmd.SetComputeFloatParam(cs_PropagateWater, ShaderID._RandomStrength, randomStrength);
            cmd.SetComputeIntParam(cs_PropagateWater, ShaderID._Seed, (int)seed);

            cmd.SetComputeBufferParam(cs_PropagateWater, 0, ShaderID.gb_Heights, floodBuffer.gb_Heights);
            cmd.SetComputeBufferParam(cs_PropagateWater, 0, ShaderID.gb_WaterHeights, floodBuffer.gb_WaterHeights);
            cmd.SetComputeBufferParam(cs_PropagateWater, 0, ShaderID.gb_WaterCoords, floodBuffer.gb_WaterCoords);
            cmd.SetComputeBufferParam(cs_PropagateWater, 0, ShaderID.gb_WaterBlockHeights, floodBuffer.gb_WaterBlockHeights);

            cmd.DispatchCompute(
                cs_PropagateWater, 0,
                (int)mathx.batch_count(waterBlockCount, 64), 1, 1
            );

            cmd.EndSample("PropagateWater");
        }

        public static void Compute_CompositeSimulation(
            CommandBuffer cmd,
            float maxWaterHeight, Color shallowWaterColor, Color deepWaterColor,
            FloodBuffer floodBuffer
        ) {
            cmd.BeginSample("CompositeSimulation");

            int width = floodBuffer.tex_Composite.width;
            int height = floodBuffer.tex_Composite.height;

            cmd.SetComputeIntParams(cs_CompositeSimulation, ShaderID._Dimension, width, height);
            cmd.SetComputeFloatParam(cs_CompositeSimulation, ShaderID._InvMaxWaterHeight, 1.0f / maxWaterHeight);
            cmd.SetComputeFloatParams(cs_CompositeSimulation, ShaderID._ShallowWaterColor, shallowWaterColor.r, shallowWaterColor.g, shallowWaterColor.b);
            cmd.SetComputeFloatParams(cs_CompositeSimulation, ShaderID._DeepWaterColor, deepWaterColor.r, deepWaterColor.g, deepWaterColor.b);

            cmd.SetComputeTextureParam(
                cs_CompositeSimulation, 0, ShaderID.tex_Depth,
                floodBuffer.tex_Depth
            );
            cmd.SetComputeTextureParam(
                cs_CompositeSimulation, 0, ShaderID.tex_WaterHeight,
                floodBuffer.tex_WaterHeight
            );
            cmd.SetComputeTextureParam(
                cs_CompositeSimulation, 0, ShaderID.tex_Composite,
                floodBuffer.tex_Composite
            );

            cmd.DispatchCompute(
                cs_CompositeSimulation, 0,
                mathx.batch_count(width, 8),
                mathx.batch_count(height, 8), 1
            );

            cmd.EndSample("CompositeSimulation");
        }

        /// <summary>Setup camera to render depth.</summary>
        public static void SetupCamera(Camera camera, RenderTexture tex_depth)
        {
            camera.depthTextureMode = DepthTextureMode.Depth;
            camera.targetTexture = tex_depth;
        }
    }
}
