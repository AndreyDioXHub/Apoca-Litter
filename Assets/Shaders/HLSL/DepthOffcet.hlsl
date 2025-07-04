
#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    //#include "Random.cginc"
#endif

void DepthOffcet_half(
    in float4 Position,
    in float Noise,
    in UnityTexture2D MainTexture,
    in float2 UV,
    in UnitySamplerState SampleState,
    in float3 TangentViewDirection, 
    in float Offset,
    out float Out) 
{
    float uOffsetDelta = Offset* TangentViewDirection.x;
    float vOffsetDelta = Offset * TangentViewDirection.y;

    float2 offsettedUV = float2(uOffsetDelta, vOffsetDelta);

    half4 mainColor = SAMPLE_TEXTURE2D(MainTexture, SampleState, UV + offsettedUV);

    Out = mainColor.r * Noise;
}