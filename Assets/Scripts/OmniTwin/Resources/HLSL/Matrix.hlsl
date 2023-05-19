#pragma once

static float3x3 quaternion_to_matrix(in float4 q)
{
    float4 q2 = q + q;

    uint3 npn = uint3(0x80000000, 0x00000000, 0x80000000);
    uint3 nnp = uint3(0x80000000, 0x80000000, 0x00000000);
    uint3 pnn = uint3(0x00000000, 0x80000000, 0x80000000);
    float3 c0 = q2.y * asfloat(asuint(q.yxw) ^ npn) - q2.z * asfloat(asuint(q.zwx) ^ pnn) + float3(1, 0, 0);
    float3 c1 = q2.z * asfloat(asuint(q.wzy) ^ nnp) - q2.x * asfloat(asuint(q.yxw) ^ npn) + float3(0, 1, 0);
    float3 c2 = q2.x * asfloat(asuint(q.zwx) ^ pnn) - q2.y * asfloat(asuint(q.wzy) ^ nnp) + float3(0, 0, 1);

    return float3x3(
        c0[0], c1[0], c2[0],
        c0[1], c1[1], c2[1],
        c0[2], c1[2], c2[2]
    );
}

static float4 matrix_to_quaternion(in float3x3 m)
{
    float3 u = float3(m[0][0], m[1][0], m[2][0]);
    float3 v = float3(m[0][1], m[1][1], m[2][1]);
    float3 w = float3(m[0][2], m[1][2], m[2][2]);

    uint u_sign = (asuint(u.x) & 0x80000000);
    float t = v.y + asfloat(asuint(w.z) ^ u_sign);
    uint4 u_mask = (int)u_sign >> 31;
    uint4 t_mask = asint(t) >> 31;

    float tr = 1.0f + abs(u.x);

    uint4 sign_flips = uint4(0x00000000, 0x80000000, 0x80000000, 0x80000000) ^ (u_mask & uint4(0x00000000, 0x80000000, 0x00000000, 0x80000000)) ^ (t_mask & uint4(0x80000000, 0x80000000, 0x80000000, 0x00000000));

    float4 value = float4(tr, u.y, w.x, v.z) + asfloat(asuint(float4(t, v.x, u.z, w.y)) ^ sign_flips);   // +---, +++-, ++-+, +-++

    value = asfloat((asuint(value) & ~u_mask) | (asuint(value.zwxy) & u_mask));
    value = asfloat((asuint(value.wzyx) & ~t_mask) | (asuint(value) & t_mask));
    value = normalize(value);

    return value;
}

static float4 matrix_to_quaternion(in float4x4 m)
{
    float3 u = float3(m[0][0], m[1][0], m[2][0]);
    float3 v = float3(m[0][1], m[1][1], m[2][1]);
    float3 w = float3(m[0][2], m[1][2], m[2][2]);

    uint u_sign = (asuint(u.x) & 0x80000000);
    float t = v.y + asfloat(asuint(w.z) ^ u_sign);
    uint4 u_mask = (int)u_sign >> 31;
    uint4 t_mask = asint(t) >> 31;

    float tr = 1.0f + abs(u.x);

    uint4 sign_flips = uint4(0x00000000, 0x80000000, 0x80000000, 0x80000000) ^ (u_mask & uint4(0x00000000, 0x80000000, 0x00000000, 0x80000000)) ^ (t_mask & uint4(0x80000000, 0x80000000, 0x80000000, 0x00000000));

    float4 value = float4(tr, u.y, w.x, v.z) + asfloat(asuint(float4(t, v.x, u.z, w.y)) ^ sign_flips);   // +---, +++-, ++-+, +-++

    value = asfloat((asuint(value) & ~u_mask) | (asuint(value.zwxy) & u_mask));
    value = asfloat((asuint(value.wzyx) & ~t_mask) | (asuint(value) & t_mask));

    value = normalize(value);

    return value;
}

static float4x4 trs_to_matrix(in float3 translation, in float4 rotation, in float3 scale)
{
    float3x3 rot = quaternion_to_matrix(rotation);

    float3 rot_c0 = float3(rot[0][0], rot[1][0], rot[2][0]) * scale[0];
    float3 rot_c1 = float3(rot[0][1], rot[1][1], rot[2][1]) * scale[1];
    float3 rot_c2 = float3(rot[0][2], rot[1][2], rot[2][2]) * scale[2];

    return float4x4(
        rot_c0[0], rot_c1[0], rot_c2[0], translation[0],
        rot_c0[1], rot_c1[1], rot_c2[1], translation[1],
        rot_c0[2], rot_c1[2], rot_c2[2], translation[2],
        0.0f     , 0.0f     , 0.0f     , 1.0f
    );
}

static void matrix_to_trs(in float4x4 m, out float3 translation, out float4 rotation, out float3 scale)
{
    translation[0] = m[0][3];
    translation[1] = m[1][3];
    translation[2] = m[2][3];

    float3 rot_c0 = float3(m[0][0], m[1][0], m[2][0]);
    float3 rot_c1 = float3(m[0][1], m[1][1], m[2][1]);
    float3 rot_c2 = float3(m[0][2], m[1][2], m[2][2]);

    scale[0] = length(rot_c0);
    scale[1] = length(rot_c1);
    scale[2] = length(rot_c2);

    rot_c0 /= scale[0];
    rot_c1 /= scale[1];
    rot_c2 /= scale[2];

    rotation = matrix_to_quaternion(
        float3x3(
            rot_c0[0], rot_c1[0], rot_c2[0],
            rot_c0[1], rot_c1[1], rot_c2[1],
            rot_c0[2], rot_c1[2], rot_c2[2]
        )
    );
}
