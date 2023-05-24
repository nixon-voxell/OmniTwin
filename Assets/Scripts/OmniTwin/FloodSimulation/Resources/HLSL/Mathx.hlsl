#pragma once

#define EPSILON 0.0001f
#define PI 3.14159f

inline static float lengthsq(float3 x)
{
    return dot(x, x);
}

inline static float cot_theta(float3 x, float3 y)
{
    float cos_theta = dot(x, y);
    float sin_theta = length(cross(x, y));
    return cos_theta / sin_theta;
}

inline static uint batch_count(uint length, uint batchSize)
{
    return (length + batchSize - 1u) / batchSize;
}

inline static uint flatten_uint3(uint3 cellIndex, uint3 dimension)
{
    // flatten index = x + y(Dx) + z(Dx)(Dy)
    //               = x + Dx (y + z(Dy))
    // return cellIndex.x + dimension.x * (cellIndex.y + cellIndex.z * dimension.y);
    return cellIndex.x +
        dimension.x * cellIndex.y +
        dimension.x * dimension.y * cellIndex.z;
}

inline static uint flatten_uint2(uint2 cellIndex, uint2 dimension)
{
    return cellIndex.x + dimension.x * cellIndex.y;
}
