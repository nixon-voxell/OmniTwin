using UnityEngine;

public static class ShaderID
{
    public static readonly int _ThreadCount = Shader.PropertyToID("_ThreadCount");
    public static readonly int _Radius = Shader.PropertyToID("_Radius");
    public static readonly int _Center = Shader.PropertyToID("_Center");
    public static readonly int _Seed = Shader.PropertyToID("_Seed");
    public static readonly int _RandomStrength = Shader.PropertyToID("_RandomStrength");

    public static readonly int gb_Height = Shader.PropertyToID("gb_Height");
    public static readonly int gb_WaterCoords = Shader.PropertyToID("gb_WaterCoords");
}
