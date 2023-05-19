using UnityEngine;
using Unity.Mathematics;

public static class FloodSimulation
{
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
