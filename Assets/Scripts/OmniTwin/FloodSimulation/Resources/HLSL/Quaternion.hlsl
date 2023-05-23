#pragma once

inline static float3x3 look_rotation(float3 forward, float3 up)
{
    float3 c0 = normalize(cross(up, forward));
    float3 c1 = cross(forward, c0);
    float3 c2 = forward;

    return float3x3(
        c0[0], c1[0], c2[0],
        c0[1], c1[1], c2[1],
        c0[2], c1[2], c2[2]
    );
}

inline static float3x3 look_rotation(float3 forward)
{
    float3 up = float3(0.0f, 1.0f, 0.0f);
    return look_rotation(forward, up);
}

inline static float4 chgsign(float4 x, float4 y)
{
    return asfloat(asuint(x) ^ (asuint(y) & 0x80000000));
}

inline float4 nlerp(in float4 q1, in float4 q2, float t)
{
    return normalize(q1 + t * (chgsign(q2, dot(q1, q2)) - q1));
}

inline float4 slerp(in float4 q1, in float4 q2, float t)
{
    float dt = dot(q1, q2);
    if (dt < 0.0f)
    {
        dt = -dt;
        q2 = -q2;
    }

    if (dt < 0.9995f)
    {
        float angle = acos(dt);
        float s = rsqrt(1.0f - dt * dt);    // 1.0f / sin(angle)
        float w1 = sin(angle * (1.0f - t)) * s;
        float w2 = sin(angle * t) * s;

        return float4(q1 * w1 + q2 * w2);
    } else
    {
        // if the angle is small, use linear interpolation
        return nlerp(q1, q2, t);
    }
}
