#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void CalculateAdditionalLight_float(float3 WorldPos, out float3 Direction, out float4 Color,
    out half DistanceAtten, out half ShadowAtten) {
#if SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(WorldPos);
    half4 shadowCoord = ComputeScreenPos(clipPos);
#else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    int lightCount = GetAdditionalLightsCount();
    
    for (int i = 0; i < lightCount; i++)
    {
        Light addLight = GetAdditionalLight(i, shadowCoord);        
        Direction = addLight.direction;
        Color.rgb += addLight.color;
        DistanceAtten += addLight.distanceAttenuation;
        ShadowAtten += addLight.shadowAttenuation;
    }
    
    Color.a = 1;
    
#endif
}

#endif