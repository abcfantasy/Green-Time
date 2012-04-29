sampler TextureSampler : register(s0);

float4 main( float4 color : COLOR0, float2 texCoord : TEXCOORD0 ) : COLOR0
{
    float4 tex = tex2D(TextureSampler, texCoord);   
    float4 outputColor = color;
    outputColor.r = (tex.r * 0.393) + (tex.g * 0.769) + (tex.b * 0.189);
    outputColor.g = (tex.r * 0.349) + (tex.g * 0.686) + (tex.b * 0.168);    
    outputColor.b = (tex.r * 0.272) + (tex.g * 0.534) + (tex.b * 0.131);
	outputColor.a = lerp(0, tex.a, color.a);
	
    return outputColor;
}

technique Sepia
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}
