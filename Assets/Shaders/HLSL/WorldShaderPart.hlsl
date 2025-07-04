
void WorldShaderPart_float(float2 uv_MainTex, float2 offcet,
out float2 mainuv, out float damagelvl, out float4 color)
{
    /*
    float _TextureScale = 1;
    float _Scale = 1;
    
    float x = worldPos.x * _TextureScale;
    float y = worldPos.y * _TextureScale;
    float z = worldPos.z * _TextureScale;   */
    
    float valx = uv_MainTex.x / 256;
    int colorNum = valx - (uv_MainTex.x % 256) / 256;
    float uvx = uv_MainTex.x - colorNum * 256;

    int han = (colorNum - colorNum % 100) / 100;
    int dex = ((colorNum - han * 100) - (colorNum - han * 100) % 10) / 10;
    int one = colorNum - han * 100 - dex * 10;

    float red = float((8 - han)) / float(8);
    float green = float((8 - dex)) / float(8);
    float blue = float((8 - one)) / float(8);

    color = float4(red, green, blue, 1);

    float valy = uv_MainTex.y / 256;
    damagelvl = valy - (uv_MainTex.y % 256) / 256;
    damagelvl = (damagelvl / 5) * 0.8;
    float uvy = uv_MainTex.y - damagelvl * 256;

    mainuv = uv_MainTex; //float2(uvx, uvy);
    
    offcet = offcet;// * 2; //offcet.x == 104 && offcet.y == 200 ? offcet / 2 : offcet;
    
    mainuv = mainuv / 256 + offcet;
    
    
    
    /*
    float4 c = tex2D(_MainTex, mainuv / 256 + offcet);
    c = lerp(c, _Color, float(damagelvl / 5) * 0.8);
    float hdr = tex2D(_HDRMask, mainuv / 256 + offcet) * 3 + 1;
    clip(c.a - 0.5);
    Color = c;*/
}