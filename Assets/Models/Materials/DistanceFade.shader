Shader "Tutorial/042_Dithering/DistanceFade"{
    //show values to edit in inspector
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _DitherPattern("Dithering Pattern", 2D) = "white" {}
        _Color("Color", Color) = (1,0.9568627,0.8392157,1)
        _ShadowColor("Shadow Color", Color) = (0.8,0.7647059,0.6627451,1)
       /* _MinDistance("Minimum Fade Distance", Float) = 1
        _MaxDistance("Maximum Fade Distance", Float) = 3*/
    }

        SubShader{
        Cull off
        //Lighting Off
            //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
            Tags
            { 
                "LightMode" = "ForwardBase"
                "PassFlags" = "OnlyDirectional"
                /*"RenderType" = "Opaque" 
                "Queue" = "Geometry"*/
            }

            CGPROGRAM

            //the shader is a surface shader, meaning that it will be extended by unity in the background to have fancy lighting and other features
            //our surface shader function is called surf and we use the default PBR lighting model
            #pragma surface surf Unlit 
            //#pragma target 3.0

            #include "UnityCG.cginc"
            // Files below include macros and functions to assist
            // with lighting and shadows.
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            //texture and transforms of the texture
            sampler2D _MainTex;

        //The dithering pattern
        sampler2D _DitherPattern;
        float4 _DitherPattern_TexelSize;

        //remapping of distance
        //float _MinDistance;
        //float _MaxDistance;

        float4 _Color;
        float4 _ShadowColor;

        //input struct which is automatically filled by unity
        struct Input {
            float2 uv_MainTex;
            float4 screenPos;
            float3 worldNormal;// : NORMAL;
            SHADOW_COORDS(2)
        };
        
        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.rgb =  s.Albedo*0.5;
            c.a = 1;
            return c;
        }

        //the surface shader function which sets parameters the lighting function then uses
        void surf(Input i, inout SurfaceOutput  o) 
        {
            float _MinDistance = 1;
            float _MaxDistance = 2;
            //read texture and write it to diffuse color
            half4  texColor = tex2D(_MainTex, i.uv_MainTex);

            float3 normal = normalize(i.worldNormal);
            float NdotL = dot(_WorldSpaceLightPos0, normal); 
            float shadow = SHADOW_ATTENUATION(i); 
            float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
            float4 light = lightIntensity * _LightColor0;
            light = clamp(light, 0, 1);

            float4 texturecolor = clamp(_Color * texColor, 0, 1);
            float4 albedo = lerp(texturecolor * _ShadowColor, texturecolor, light.r);
            o.Albedo =  albedo.rgb;// albedo.rgb;// i.worldNormal;//texColor.rgb;
            o.Alpha = 1;
            //value from the dither pattern
            float2 screenPos = i.screenPos.xy / i.screenPos.w;
            float2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
            float ditherValue = tex2D(_DitherPattern, ditherCoordinate).r;

            //get relative distance from the camera
            float relDistance = i.screenPos.w; 
            relDistance = relDistance - _MinDistance;
            relDistance = relDistance / (_MaxDistance - _MinDistance);
            //discard pixels accordingly
            clip(relDistance - ditherValue);
        }
        ENDCG
        }
            //UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
            FallBack "Diffuse"
}