
#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

void Wave_half(
    in float2 UV,
    in float AmplitudeA,
    in float FreqA,
    in float SpeedA,
    in float2 DirectionA,
    in float AmplitudeB,
    in float FreqB,
    in float SpeedB,
    in float2 DirectionB,
    in float AmplitudeC,
    in float FreqC,
    in float SpeedC,
    in float2 DirectionC,
    out float OutNormal)
{
    float4 WaveA = float4(UV.x, UV.y, 0, 0);
    float kA = 2 * PI / FreqA;
    float fA = kA * (dot(DirectionA, WaveA.xy) - SpeedA);
    
    WaveA.x = WaveA.x + AmplitudeA * cos(fA);
    WaveA.z = AmplitudeA * sin(fA);

    float4 WaveB = float4(UV.x, UV.y, 0, 0);
    float kB = 2 * PI / FreqB;
    float fB = kB * (dot(DirectionB, WaveB.xy) - SpeedB);

    WaveB.x = WaveB.x + AmplitudeB * cos(fB);
    WaveB.z = AmplitudeB * sin(fB);

    float4 WaveC = float4(UV.x, UV.y, 0, 0);
    float kC = 2 * PI / FreqC;
    float fC = kC * (dot(DirectionC, WaveC.xy) - SpeedC);

    WaveC.x = WaveC.x + AmplitudeC * cos(fC);
    WaveC.z = AmplitudeC * sin(fC);

    float Out = WaveA.z + WaveB.z + WaveC.z;
    float AmplitudeSumm = AmplitudeA + AmplitudeB + AmplitudeC;
    OutNormal = (Out + AmplitudeSumm) / (2 * AmplitudeSumm);
}