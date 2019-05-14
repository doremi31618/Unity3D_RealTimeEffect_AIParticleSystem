// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Star" {
    Properties{
        _Alpha("Alpha",Range(0.0,1)) = 1
    }
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

                float _Alpha;
				struct data {
					float4 vertex : POSITION;
					fixed4 color: COLOR;
				};

				data vert (data v) {
					v.vertex = UnityObjectToClipPos(v.vertex);
					return v;
				}

				fixed4 frag(data f) : COLOR {
					return f.color * _Alpha;
				}
			ENDCG
		}
	}
}