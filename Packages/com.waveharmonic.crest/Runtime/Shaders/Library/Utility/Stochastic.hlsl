// Crest Water System
// Copyright © 2024 Wave Harmonic. All rights reserved.

// Source:
// https://pastebin.com/sDrnzYxB

// License:
// CC0 Creative Commons License
// "Yes by all means use it how you wish. I'm sharing it as CC0."
// https://www.reddit.com/r/Unity3D/comments/dhr5g2/comment/f3rz6rc/

#ifndef d_WaveHarmonic_Utility_Stochastic
#define d_WaveHarmonic_Utility_Stochastic

m_UtilityNameSpace

float2 Hash2D2D(const float2 s)
{
    // Magic numbers.
    return frac(sin(fmod(float2(dot(s, float2(127.1, 311.7)), dot(s, float2(269.5, 183.3))), PI)) * 43758.5453);
}

float4 SampleStochastic(const Texture2D i_Texture, const SamplerState i_Sampler, const float2 i_UV)
{
    // Triangle vertices and blend weights.
    // BW_vx[0...2].xyz = triangle verts
    // BW_vx[3].xy = blend weights (z is unused)
    float4x3 BW_vx;

    // UV transformed into triangular grid space with UV scaled by approximation of 2 * sqrt(3)
    float2 skewUV = mul(float2x2 (1.0, 0.0, -0.57735027, 1.15470054), i_UV * 3.464);

    // Vertex IDs and barycentric coords
    float2 vxID = float2 (floor(skewUV));
    float3 barry = float3 (frac(skewUV), 0);
    barry.z = 1.0 - barry.x - barry.y;

    BW_vx = ((barry.z > 0) ?
        float4x3(float3(vxID, 0), float3(vxID + float2(0, 1), 0), float3(vxID + float2(1, 0), 0), barry.zyx) :
        float4x3(float3(vxID + float2 (1, 1), 0), float3(vxID + float2 (1, 0), 0), float3(vxID + float2 (0, 1), 0), float3(-barry.z, 1.0 - barry.y, 1.0 - barry.x)));

    // Calculate derivatives to avoid triangular grid artifacts
    float2 dx = ddx(i_UV);
    float2 dy = ddy(i_UV);

    // Blend samples with calculated weights.
    return mul(SAMPLE_TEXTURE2D_GRAD(i_Texture, i_Sampler, i_UV + Hash2D2D(BW_vx[0].xy), dx, dy), BW_vx[3].x) +
           mul(SAMPLE_TEXTURE2D_GRAD(i_Texture, i_Sampler, i_UV + Hash2D2D(BW_vx[1].xy), dx, dy), BW_vx[3].y) +
           mul(SAMPLE_TEXTURE2D_GRAD(i_Texture, i_Sampler, i_UV + Hash2D2D(BW_vx[2].xy), dx, dy), BW_vx[3].z);
}

m_UtilityNameSpaceEnd

#endif // d_WaveHarmonic_Utility_Stochastic
