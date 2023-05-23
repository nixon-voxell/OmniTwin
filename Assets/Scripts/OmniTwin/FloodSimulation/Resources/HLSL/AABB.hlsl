#pragma once

struct AABB
{
    float3 minCoor;
    float3 maxCoor;

    inline bool IsValid()
    {
        return all(minCoor <= maxCoor);
    }

    inline float SurfaceArea()
    {
        float3 diff = maxCoor - minCoor;
        return 2 * dot(diff, diff.yzx);
    }

    inline bool Contains(in float3 x)
    {
        return all((x >= minCoor) & (x <= maxCoor));
    }

    inline bool Contains(in AABB aabb)
    {
        all((minCoor <= aabb.minCoor) & (maxCoor >= aabb.maxCoor));
    }

    inline bool Overlaps(in AABB aabb)
    {
        return all((maxCoor >= aabb.minCoor) & (minCoor <= aabb.maxCoor));
    }

    inline void Expand(in float signedDistance)
    {
        minCoor -= signedDistance;
        maxCoor += signedDistance;
    }

    inline void Encapsulate(in AABB aabb)
    {
        minCoor = min(minCoor, aabb.minCoor);
        maxCoor = max(maxCoor, aabb.maxCoor);
    }

    inline void Encapsulate(in float3 x)
    {
        minCoor = min(minCoor, x);
        maxCoor = max(maxCoor, x);
    }
};
