float fTimer;
sampler ColorMapSampler : register(s0);

float4 PixelShaderFunction(float2 Tex : TEXCOORD0) : COLOR0
{
	Tex.x += sin(fTimer+Tex.x*10)*0.02f;
	Tex.y += cos(fTimer+Tex.y*10)*0.02f;

	float4 Color = tex2D(ColorMapSampler, Tex); 
	return Color;
}

technique PostProcess
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
