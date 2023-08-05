using UnityEngine;

public static class ShaderID
{
    public static readonly int _ThreadCount = Shader.PropertyToID("_ThreadCount");
    public static readonly int _Start = Shader.PropertyToID("_Start");
    public static readonly int _Number = Shader.PropertyToID("_Number");
    public static readonly int _Dimension = Shader.PropertyToID("_Dimension");
    public static readonly int _Radius = Shader.PropertyToID("_Radius");
    public static readonly int _Height = Shader.PropertyToID("_Height");
    public static readonly int _WaterBlockHeight = Shader.PropertyToID("_WaterBlockHeight");
    public static readonly int _WaterBlockCount = Shader.PropertyToID("_WaterBlockCount");
    public static readonly int _Center = Shader.PropertyToID("_Center");
    public static readonly int _Seed = Shader.PropertyToID("_Seed");
    public static readonly int _RandomStrength = Shader.PropertyToID("_RandomStrength");
    public static readonly int _PropagateSpeed = Shader.PropertyToID("_PropagateSpeed");
    public static readonly int _InvMaxWaterHeight = Shader.PropertyToID("_InvMaxWaterHeight");
    public static readonly int _ShallowWaterColor = Shader.PropertyToID("_ShallowWaterColor");
    public static readonly int _DeepWaterColor = Shader.PropertyToID("_DeepWaterColor");

    public static readonly int tex_Depth = Shader.PropertyToID("tex_Depth");
    public static readonly int tex_WaterHeight = Shader.PropertyToID("tex_WaterHeight");
    public static readonly int tex_Composite = Shader.PropertyToID("tex_Composite");
    public static readonly int gb_Array = Shader.PropertyToID("gb_Array");
    public static readonly int gb_Heights = Shader.PropertyToID("gb_Heights");
    public static readonly int gb_WaterCoords = Shader.PropertyToID("gb_WaterCoords");
    public static readonly int gb_WaterBlockHeights = Shader.PropertyToID("gb_WaterBlockHeights");
    public static readonly int gb_WaterHeights = Shader.PropertyToID("gb_WaterHeights");
}
