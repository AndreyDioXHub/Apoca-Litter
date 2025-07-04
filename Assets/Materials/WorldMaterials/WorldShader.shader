// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Bumped shader. Differences from regular Bumped one:
// - no Main Color
// - Normalmap uses Tiling/Offset of the Base texture
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "World Shader" 
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _HDRMask("HDR Mask (RGB)", 2D) = "white" {}
        _Color("Main Color", Color) = (1, 1, 1, 1)
        _TextureScale("Texture scale", float) = 1
        _Scale("scale", float) = 1

        //[NoScaleOffset] _BumpMap("Normalmap", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull Off
        LOD 250

        CGPROGRAM
        #pragma surface surf Lambert noforwardadd

        sampler2D _MainTex;
        sampler2D _HDRMask;
        float _TextureScale;
        float _Scale;
        fixed4 _Color;
        //sampler2D _BumpMap;
    
        struct Input 
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
        };

        float my_fmod(float a, float b) 
        {
            float c = frac(abs(a / b))* abs(b);
            return c;
        }

        void surf(Input IN, inout SurfaceOutput o) 
        {
            //fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

            float x = IN.worldPos.x * _TextureScale;
            float y = IN.worldPos.y * _TextureScale;
            float z = IN.worldPos.z * _TextureScale;

            float isUp = abs(IN.worldNormal.y);

            //float2 offcet = float2(fmod(z + x * (1 - isUp) , 0.0625), fmod(y + x * isUp, 0.0625));
            //float2 offcet = float2(my_fmod(x + z * (1 - isUp) , 0.0625), my_fmod(y + z * isUp, 0.0625));
            float2 offcet = float2(my_fmod(x + z * (1 - isUp) , _Scale), my_fmod(y + z * isUp, _Scale));
            //float2 offcet = float2(my_fmod(x, _Scale), my_fmod(z, _Scale));
            //float2 offcet = float2(fmod(x + y * isUp, 0.0625), fmod(z + y * (1 - isUp), 0.0625));

            float valx = IN.uv_MainTex.x / 256;
            int colorNum = valx - (IN.uv_MainTex.x % 256) / 256;
            float uvx = IN.uv_MainTex.x - colorNum * 256;

            int han = (colorNum - colorNum % 100) / 100;
            int dex = ((colorNum - han * 100) - (colorNum - han * 100) % 10) / 10;
            int one = colorNum - han * 100 - dex * 10;

            float red = float((8 - han)) / float(8);
            float green = float((8 - dex)) / float(8);
            float blue = float((8 - one)) / float(8);

            float4 color = float4(red, green, blue, 1);

            float valy = IN.uv_MainTex.y / 256;
            float damagelvl = valy - (IN.uv_MainTex.y % 256) / 256;
            float uvy = IN.uv_MainTex.y - damagelvl * 256;

            float2 mainuv = float2(uvx, uvy);

            fixed4 c = tex2D(_MainTex, mainuv / 256 + offcet);
            c = lerp(c, _Color, float(damagelvl / 5) * 0.8);
            float hdr = tex2D(_HDRMask, mainuv / 256 + offcet)*3 + 1;
            o.Albedo = (c.rgb * color.rgb) * hdr;
            clip(c.a - 0.5);
            o.Alpha = c.a;
            //o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
        }
        ENDCG
    }

        FallBack "Mobile/Diffuse"
}