using UnityEngine;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

public class FloodBuffer : System.IDisposable
{
    public RenderTexture tex_Depth;
    public RenderTexture tex_WaterHeight;
    public RenderTexture tex_Composite;
    public GraphicsBuffer gb_Heights;
    public GraphicsBuffer gb_WaterCoords;
    public GraphicsBuffer gb_WaterBlockHeights;
    public GraphicsBuffer gb_WaterHeights;

    public FloodBuffer(uint2 size, uint maxWaterCount)
    {
        int2 _size = (int2)size;
        int count = _size.x * _size.y;

        // create depth only render texture
        this.tex_Depth = new RenderTexture(_size.x, _size.y, 32, RenderTextureFormat.Depth);
        this.tex_Depth.filterMode = FilterMode.Point;
        this.tex_Depth.Create();
        // create water height int render texture
        this.tex_WaterHeight = new RenderTexture(_size.x, _size.y, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        this.tex_WaterHeight.enableRandomWrite = true;
        this.tex_WaterHeight.filterMode = FilterMode.Point;
        this.tex_WaterHeight.Create();

        this.tex_Composite = new RenderTexture(_size.x, _size.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        this.tex_Composite.enableRandomWrite = true;
        this.tex_Composite.filterMode = FilterMode.Point;
        this.tex_Composite.Create();

        this.gb_Heights = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, UnsafeUtility.SizeOf<float>());
        this.gb_WaterCoords = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)maxWaterCount, UnsafeUtility.SizeOf<float2>());
        this.gb_WaterBlockHeights = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)maxWaterCount, UnsafeUtility.SizeOf<uint>());
        this.gb_WaterHeights = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, UnsafeUtility.SizeOf<uint>());
    }

    public void Dispose()
    {
        this.tex_Depth.Release();
        this.tex_WaterHeight.Release();
        this.gb_Heights.Dispose();
        this.gb_WaterCoords.Dispose();
        this.gb_WaterBlockHeights.Dispose();
        this.gb_WaterHeights.Dispose();
    }
}
