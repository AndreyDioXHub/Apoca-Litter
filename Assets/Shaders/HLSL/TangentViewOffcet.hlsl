/*
    Basically, the shader samples the cracks multiple times (Samples parameter)
    Each sample slightly changes the UV based on tangent space's view direction.
    At the end the shader combines the cracks texture and combined color of all samples.

    This technique is called Parallax Mapping, you can read more about it here:
    https://en.wikipedia.org/wiki/Parallax_mapping

    Code is based on a tutorial by Jettelly https://www.youtube.com/watch?v=_dZthyScKF8
*/


// Unnecessary includes, used only for autocompletion
#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

void TangentViewOffcet_half(
    in float2 Offset,
    in float3 TangentViewDirection,
    out float2 Out)
{
    
    float uOffsetDelta = Offset.x * TangentViewDirection.x;
    float vOffsetDelta = Offset.y * TangentViewDirection.y;

    float2 offsettedUV;
    
    offsettedUV = float2(uOffsetDelta, vOffsetDelta);
    
    Out = offsettedUV;
}