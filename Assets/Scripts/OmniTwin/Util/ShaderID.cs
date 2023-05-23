using UnityEngine;

public static class ShaderID
{
    public static readonly int _ThreadCount = Shader.PropertyToID("_ThreadCount");
    public static readonly int _Dimension = Shader.PropertyToID("_Dimension");
    public static readonly int _Radius = Shader.PropertyToID("_Radius");
    public static readonly int _Height = Shader.PropertyToID("_Height");
    public static readonly int _Center = Shader.PropertyToID("_Center");
    public static readonly int _Seed = Shader.PropertyToID("_Seed");
    public static readonly int _RandomStrength = Shader.PropertyToID("_RandomStrength");

    public static readonly int tex_Depth = Shader.PropertyToID("tex_Depth");
    public static readonly int tex_WaterHeight = Shader.PropertyToID("tex_WaterHeight");
    public static readonly int gb_Heights = Shader.PropertyToID("gb_Heights");
    public static readonly int gb_WaterCoords = Shader.PropertyToID("gb_WaterCoords");
    public static readonly int gb_WaterHeights = Shader.PropertyToID("gb_WaterHeights");
}
