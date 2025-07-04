
#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//#include "Random.cginc"
#endif

void FresnelEffectCustom_half(
    float3 Normal, 
    float3 ViewDir, 
    float Power, 
    out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}