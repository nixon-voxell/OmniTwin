#pragma once
#include "Packages/twothree.physics/TwoThree.Physics.Algorithm.Compute/HLSL/BoneWeight.hlsl"

inline uint LoadIndex16FromByteAddr(in ByteAddressBuffer gb_indices, uint idx)
{
    uint entryOffset = idx & 1u;
    idx = idx >> 1;
    uint read = gb_indices.Load(idx << 2);
    return entryOffset == 1 ? read >> 16 : read & 0xffff;
}

inline uint LoadIndex16FromByteAddr(in RWByteAddressBuffer gb_indices, uint idx)
{
    uint entryOffset = idx & 1u;
    idx = idx >> 1;
    uint read = gb_indices.Load(idx << 2);
    return entryOffset == 1 ? read >> 16 : read & 0xffff;
}

inline uint LoadIndex32FromByteAddr(in ByteAddressBuffer gb_indices, uint idx)
{
    return gb_indices.Load(idx << 2);
}

inline uint LoadIndex32FromByteAddr(in RWByteAddressBuffer gb_indices, uint idx)
{
    return gb_indices.Load(idx << 2);
}

inline float3 LoadVertPosFromByteAddr(in ByteAddressBuffer gb_vertices, uint vertIdx, uint vertStrideSize, uint posByteOffset)
{
    return asfloat(gb_vertices.Load3(vertIdx * vertStrideSize + posByteOffset));
}

inline float3 LoadVertPosFromByteAddr(in RWByteAddressBuffer gb_vertices, uint vertIdx, uint vertStrideSize, uint posByteOffset)
{
    return asfloat(gb_vertices.Load3(vertIdx * vertStrideSize + posByteOffset));
}

BoneWeight LoadBoneWeightFromByteAddr(in ByteAddressBuffer gb_boneWeights, uint boneIdx)
{
    uint byteIdx = boneIdx * 32;
    BoneWeight boneWeight;
    boneWeight.weight0 = asfloat(gb_boneWeights.Load(byteIdx)); byteIdx += 4;
    boneWeight.weight1 = asfloat(gb_boneWeights.Load(byteIdx)); byteIdx += 4;
    boneWeight.weight2 = asfloat(gb_boneWeights.Load(byteIdx)); byteIdx += 4;
    boneWeight.weight3 = asfloat(gb_boneWeights.Load(byteIdx)); byteIdx += 4;
    boneWeight.boneIndex0 = gb_boneWeights.Load(byteIdx); byteIdx += 4;
    boneWeight.boneIndex1 = gb_boneWeights.Load(byteIdx); byteIdx += 4;
    boneWeight.boneIndex2 = gb_boneWeights.Load(byteIdx); byteIdx += 4;
    boneWeight.boneIndex3 = gb_boneWeights.Load(byteIdx);

    return boneWeight;
}

void LoadTriIndicesFromByteAddr(in ByteAddressBuffer gb_indices, uint startIdx, uint indStrideSize, out uint indices[3])
{
    uint i;
    if (indStrideSize == 2)
    {
        [unroll]
        for (i = 0; i < 3; i++)
        {
            indices[i] = LoadIndex16FromByteAddr(gb_indices, startIdx + i);
        }
    } else
    {
        [unroll]
        for (i = 0; i < 3; i++)
        {
            indices[i] = LoadIndex32FromByteAddr(gb_indices, startIdx + i);
        }
    }
}

void LoadTriIndicesFromByteAddr(in RWByteAddressBuffer gb_indices, uint startIdx, uint indStrideSize, out uint indices[3])
{
    uint i;
    if (indStrideSize == 2)
    {
        [unroll]
        for (i = 0; i < 3; i++)
        {
            indices[i] = LoadIndex16FromByteAddr(gb_indices, startIdx + i);
        }
    } else
    {
        [unroll]
        for (i = 0; i < 3; i++)
        {
            indices[i] = LoadIndex32FromByteAddr(gb_indices, startIdx + i);
        }
    }
}

void AtomicAddDeltaToByteAddr(in RWByteAddressBuffer gb_buffer, uint address, float newDeltaVal)
{
    uint i_val = asuint(newDeltaVal);
    uint tmp0 = 0;
    uint tmp1;

    [allow_uav_condition]
    while (true)
    {
        gb_buffer.InterlockedCompareExchange(address, tmp0, i_val, tmp1);

        if (tmp1 == tmp0) break;

        tmp0 = tmp1;
        i_val = asuint(newDeltaVal + asfloat(tmp1));
    }

    return;
}
