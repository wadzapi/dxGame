texture2D tex0;
static float TexSize = 2048.0f;

float4x4 ViewProj : VIEWPROJECTION;

sampler2D texSampler = sampler_state
{
	Texture = <tex0>;
	MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
};

struct VertexInputDB
{
	float3 pos: POSITION;
	float2 Tex: TEXCOORD0;
};

struct VertexInputBgr
{	
	float3 pos : POSITION;
	float corner: TEXCOORD0;
};

struct VertexInputHW
{
	//stream 0
	float3 pos : POSITION;
	float corner: TEXCOORD0;
	//stream 1
	float4 model_matrix0 : TEXCOORD1;
	float4 model_matrix1 : TEXCOORD2;
	float4 model_matrix2 : TEXCOORD3;
	float4 model_matrix3 : TEXCOORD4;
	float4 UVWH : TEXCOORD5;
};

struct VertexOutput
{
	float4 pos : POSITION;
	float2 Tex : TEXCOORD0;
};

VertexOutput BgrVS(VertexInputBgr input)
{
	VertexOutput output = (VertexOutput)0;
	if (input.corner == 0)
	{
		output.pos = mul(float4(-512.0f, 623.6624f, 250.0f, 1.0f), ViewProj);
		output.Tex = float2(0.0f, 0.0f);
		return output;
	}
	else if (input.corner == 1)
	{
		output.pos = mul(float4(512.0f, 623.6624f, 250.0f, 1.0f), ViewProj);
		output.Tex = float2(1.0f, 0.0f);
		return output;
	}
	else if (input.corner == 2)
	{
		output.pos = mul(float4(512.0f, -144.3376f, 250.0f, 1.0f), ViewProj);
		output.Tex = float2(1.0f, 0.75f);
		return output;
	}
	else if (input.corner == 3)
	{
		output.pos = mul(float4(-512.0f, -144.3376f, 250.0f, 1.0f), ViewProj);
		output.Tex = float2(0.0f, 0.75f);
		return output;
	}
	else
	{
		return output;
	}
}

VertexOutput DBMainVS(VertexInputDB input)
{
	VertexOutput output = (VertexOutput)0;
	output.pos = mul(float4(input.pos.xyz, 1), ViewProj);
	output.Tex = input.Tex;
	return output;
}

VertexOutput HWMainVS(VertexInputHW input)
{
	float4x4 modelMatrix = 
	{
		input.model_matrix0,
		input.model_matrix1,
		input.model_matrix2,
		input.model_matrix3
	};
	VertexOutput output = (VertexOutput)0;
	output.pos = mul(float4(input.pos.xyz, 1.0), modelMatrix);
	output.pos = mul(output.pos, ViewProj);
	if (input.corner == 0)
	{
		output.Tex = float2(input.UVWH.xy/TexSize);
		return output;
	}
	else if (input.corner == 1)
	{
		output.Tex = float2((input.UVWH.x + input.UVWH.z)/TexSize, input.UVWH.y/TexSize);
		return output;
	}
	else if (input.corner == 2)
	{
		output.Tex = float2((input.UVWH.x + input.UVWH.z)/TexSize, (input.UVWH.y + input.UVWH.w)/TexSize);
		return output;
	}
	else if (input.corner == 3)
	{
		output.Tex = float2(input.UVWH.x/TexSize, (input.UVWH.y + input.UVWH.w)/TexSize);
		return output;
	}
	else
	{
		return output;
	}
}

float4 MainPS(float2 Tex: TEXCOORD0):COLOR
{
	float4 texel = tex2D(texSampler, Tex);
	if (texel.a < 0.3) discard;
	return texel;
}

technique DBInstancing
{
	pass p0
	{
		ZEnable= true;
		CullMode = None;
		AlphaBlendEnable = true;
		BlendOp = Add;
		SrcBlend = SRCALPHA;
		DestBlend = INVSRCALPHA;
		VertexShader = compile vs_1_1 DBMainVS();
		PixelShader = compile ps_2_0 MainPS();
	}
}

technique HWInstancing
{
	pass p0
	{
		//PointSize = 1414745673
		CullMode = None;
		AlphaBlendEnable = true;
		BlendOp = Add;
		SrcBlend = SRCALPHA;
		DestBlend = INVSRCALPHA;
		VertexShader = compile vs_3_0 HWMainVS();
		PixelShader = compile ps_2_0 MainPS();
	}
}

technique Background
{
	pass p0
	{
		Lighting = false;
		AlphaBlendEnable = false;
		AlphaTestEnable = false;
		ZwriteEnable = false;
		ZEnable = false;
		CullMode = None;
		VertexShader = compile vs_1_1 BgrVS();
		PixelShader = compile ps_2_0 MainPS();
	}
}