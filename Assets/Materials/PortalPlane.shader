// (c) 2018 Guidev
// This code is licensed under MIT license (see LICENSE.txt for details)

Shader "Unlit/PortalPlane"
{

    Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
		[Enum(CullMode)] _CullMode("Cull Mode", Int) = 0 // culling is disabled
		//[Enum(Equal,3,NotEqual,6)] _StencilTest ("Stencil Test", int) = 6
	}
    
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Zwrite Off
		ColorMask 0
		Cull [_CullMode]

		Pass
		{
			Stencil{
				Ref 1
				Comp always
				Pass replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(0.0, 0.0, 0.0, 0.0);
			}
			ENDCG
		}
	}
}
