Shader "Unlit/CanvasOutLine"
{
    Properties
    {
       //_MainTex ("Texture", 2D) = "white" {}
        _OtlinedObjectsTexture("Texture", 2D) = "white"{}
        _HideObjectsTexture("Texture", 2D) = "white"{}
        _StrokeWidth ("Stroke Widtht", Float) = 10
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _ScreenHeight ("Screen Height", Float) = 600
        _ScreenWidth ("Screen Width", Float) = 1000
        
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        //Tags { "RenderType"="Opaque" }
        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 1

        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            //sampler2D _MainTex;
            sampler2D _OtlinedObjectsTexture;
            sampler2D _HideObjectsTexture;
            fixed4 _OutlineColor;
            float _StrokeWidth;
            float _ScreenHeight;
            float _ScreenWidth;
            float4 _OtlinedObjectsTexture_ST;

            //float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;//TRANSFORM_TEX(v.uv, _OtlinedObjectsTexture);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float col = tex2D(_OtlinedObjectsTexture, i.uv);

                float depthpp = tex2D(_OtlinedObjectsTexture, float2(i.uv.x + _StrokeWidth/_ScreenWidth, i.uv.y + _StrokeWidth/_ScreenHeight));
                float semyrespp = depthpp - col;

                float depthpm = tex2D(_OtlinedObjectsTexture, float2(i.uv.x + _StrokeWidth/_ScreenWidth, i.uv.y - _StrokeWidth/_ScreenHeight));
                float semyrespm = depthpm - col;

                float depthmm = tex2D(_OtlinedObjectsTexture, float2(i.uv.x - _StrokeWidth/_ScreenWidth, i.uv.y - _StrokeWidth/_ScreenHeight));
                float semyresmm = depthmm - col;

                float depthmp = tex2D(_OtlinedObjectsTexture, float2(i.uv.x - _StrokeWidth/_ScreenWidth, i.uv.y + _StrokeWidth/_ScreenHeight));
                float semyresmp = depthmp - col;

                float depthcol = semyrespp + semyrespm + semyresmm + semyresmp;

                float globaldepthpp = tex2D(_HideObjectsTexture ,float2(i.uv.x + _StrokeWidth/_ScreenWidth, i.uv.y + _StrokeWidth/_ScreenHeight));
                float deptmaskpp = globaldepthpp > depthpp ? globaldepthpp : depthpp;

                float globaldepthpm = tex2D(_HideObjectsTexture ,float2(i.uv.x + _StrokeWidth/_ScreenWidth, i.uv.y - _StrokeWidth/_ScreenHeight));
                float deptmaskpm = globaldepthpm > depthpm ? globaldepthpm : depthpm;

                float globaldepthmm = tex2D(_HideObjectsTexture ,float2(i.uv.x - _StrokeWidth/_ScreenWidth, i.uv.y - _StrokeWidth/_ScreenHeight));
                float deptmaskmm = globaldepthmm > depthmm ? globaldepthmm : depthmm;

                float globaldepthmp = tex2D(_HideObjectsTexture ,float2(i.uv.x - _StrokeWidth/_ScreenWidth, i.uv.y + _StrokeWidth/_ScreenHeight));
                float deptmaskmp = globaldepthmp > depthmp ? globaldepthmp : depthmp;

                float globalcol = deptmaskpp + deptmaskmm + deptmaskpm + deptmaskmp - 4*col - globaldepthpp - globaldepthpm - globaldepthmm - globaldepthmp;

                float depthaddgloabal = step(0.001, globalcol);

                fixed4 resul = fixed4(depthaddgloabal*_OutlineColor.r, depthaddgloabal*_OutlineColor.g, depthaddgloabal*_OutlineColor.b, depthaddgloabal);

                return resul;
            }
            ENDCG
        }
    }
}
