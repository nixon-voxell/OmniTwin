using System.Runtime.CompilerServices;
using Unity.Mathematics;

public static class mathx
{
    /// <summary>
    /// We prefer a slightly larger epsilon to prevent floating point inaccuracy.
    /// </summary>
    public const float EPSILON = 0.0001f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float cot_theta(float3 x, float3 y)
    {
        float cos_theta = math.dot(x, y);
        float sin_theta = math.length(math.cross(x, y));
        return cos_theta / sin_theta;
    }

    /// <summary>Calculate the least amount of batches needed based on batch size.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int batch_count(int length, int batchSize) => (length + batchSize - 1) / batchSize;

    /// <summary>Calculate the least amount of batches needed based on batch size.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 batch_count(int2 length, int2 batchSize) => (length + batchSize - 1) / batchSize;

    /// <summary>Calculate the least amount of batches needed based on batch size.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int3 batch_count(int3 length, int3 batchSize) => (length + batchSize - 1) / batchSize;

    /// <summary>Calculate the least amount of batches needed based on batch size.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint batch_count(uint length, uint batchSize) => (length + batchSize - 1u) / batchSize;

    /// <summary>Flatten a int3 index into a int index.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int flatten_int3(int3 cellIndex, int3 dimension)
    {
        // flatten index = x + y(Dx) + z(Dx)(Dy)
        //               = x + Dx (y + z(Dy))
        return cellIndex.x + dimension.x * (cellIndex.y + cellIndex.z * dimension.y);
    }
}
