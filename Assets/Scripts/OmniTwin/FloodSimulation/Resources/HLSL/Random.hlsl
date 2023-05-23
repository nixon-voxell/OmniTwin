#pragma once

/// Random struct referencing Unity.Mathematics.Random https://github.com/needle-mirror/com.unity.mathematics/blob/master/Unity.Mathematics/random.cs
struct Random
{
    uint m_State;

    inline static Random CreateFromState(uint state)
    {
        Random rand;
        rand.m_State = state;
        return rand;
    }

    /// Constructs a Random instance with an index that gets hashed. The index must not be uint.MaxValue.
    inline static Random CreateFromIndex(uint index)
    {
        // Wang hash will hash 61 to zero but we want uint.MaxValue to hash to zero. To make this happen we must offset by 62.
        Random rand;
        rand.m_State = Random::StateFromIndex(index);
        return rand;
    }

    /// Constructs a Random instance with an index that gets hashed. The index must not be uint.MaxValue.
    inline static uint StateFromIndex(uint index)
    {
        // Wang hash will hash 61 to zero but we want uint.MaxValue to hash to zero. To make this happen we must offset by 62.
        return Random::WangHash(index + 62u);
    }

    inline static uint WangHash(uint n)
    {
        // https://gist.github.com/badboy/6267743#hash-function-construction-principles
        // Wang hash: this has the property that none of the outputs will
        // collide with each other, which is important for the purposes of
        // seeding a random number generator.  This was verified empirically
        // by checking all 2^32 uints.
        n = (n ^ 61u) ^ (n >> 16);
        n *= 9u;
        n = n ^ (n >> 4);
        n *= 0x27d4eb2du;
        n = n ^ (n >> 15);

        return n;
    }

    /// Returns a uniformly random uint3 value with all components in the interval [0, 4294967294].
    inline uint NextUInt()
    {
        return NextState() - 1u;
    }

    /// Returns a uniformly random uint value in the interval [0, max).
    inline uint NextUInt(uint maxNum)
    {
        return NextState() % maxNum;
    }

    /// Returns a uniformly random uint value in the interval [min, max).
    inline uint NextUInt(uint minNum, uint maxNum)
    {
        uint range = maxNum - minNum;
        return (NextState() % range) + minNum;
    }

    /// Returns a uniformly random int value in the interval [-2147483647, 2147483647].
    inline int NextInt()
    {
        return (int)NextState() ^ -2147483648;
    }

    /// Returns a uniformly random int value in the interval [0, max).
    inline int NextInt(int maxNum)
    {
        return (int)(NextState() % (uint)maxNum);
    }

    /// Returns a uniformly random int value in the interval [min, max).
    inline int NextInt(int minNum, int maxNum)
    {
        uint range = (uint)(maxNum - minNum);
        return (int)(NextState() % range) + minNum;
    }

    /// Returns a uniformly random float value in the interval [0, 1).
    inline float NextFloat()
    {
        return asfloat(0x3f800000 | (NextState() >> 9)) - 1.0f;
    }

    /// Returns a uniformly random float value in the interval [0, max).
    inline float NextFloat(float max)
    {
        return NextFloat() * max;
    }

    /// Returns a uniformly random float value in the interval [min, max).
    inline float NextFloat(float min, float max)
    {
        return NextFloat() * (max - min) + min;
    }

    inline uint NextState()
    {
        uint t = m_State;
        m_State ^= m_State << 13;
        m_State ^= m_State >> 17;
        m_State ^= m_State << 5;
        return t;
    }
};
