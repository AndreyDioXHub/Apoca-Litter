
#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

void GerstnerWaves_half(
    in float3 UV,
    in float4 Wave,
    in float Speed,
    out float3 Position,
    out float3 Tangent,
    out float3 Normal,
    out float Amplitude)
{
    float3 p = UV;
    float k = 2 * PI / Wave.w;
    float a = Wave.z / k;
    float2 d = normalize(Wave.xy);
    float f = k * (dot(d, p.xz) - Speed);

    p.x += d.x * (a * cos(f));
    p.y = a * sin(f);
    p.z += d.y * (a * cos(f));

    float3 tangent = float3(
        1 - d.x * d.x * (Wave.z * sin(f)),
        d.x * (Wave.z * cos(f)),
        -d.x * d.y * (Wave.z * sin(f))
        );

    float3 binormal = float3(
        -d.x * d.y * (Wave.z * sin(f)),
        d.y * (Wave.z * cos(f)),
        1 - d.y * d.y * (Wave.z * sin(f))
        );

    float3 normal = binormal;// normalize(cross(binormal, tangent));

    Position = p; 
    Tangent = tangent; 
    Normal = normal;
    Amplitude = a; 
}
