Shader "Custom/MySprite"
{
   Properties
   {
      _MainTex("Sprite Texture", 2D) = "white" {}
      [PerRendererData]_Color("Dont change this", Color) = (1,1,1,1)
      [PerRendererData]_TintColor("Dont change this too", Color) = (1,1,1,1)
      [PerRendererData] _Outline ("Outline", Float) = 1
      _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
      _OutlineAlpha("Outline Alpha",Range (0.0, 1)) = 1
      _OutlineWidth ("Outline width", Range (0.0, 50.0)) = .005
      _GradientColor("Gradient Color",Range (0.0, 0.2)) = 0.111
      _Diff("Diff", Range(0, 1)) = 0.5
      _BlurRadius ("BlurRadius", Range(1, 15)) = 5
      _TextureSize ("TextureSize", Float) = 256
      _GlobalAlpha("Global Alpha",Range(0,1)) = 1
      [Toggle]_OutlineUseGradient("OutlineUseGradient",Float) = 1
      //_BlurRadius ("BlurRadius", Range (0.0, 1)) = .005
      [HideInInspector][MaterialToggle] PixelSnap("Pixel snap", Float) = 0
      [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
      [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
      [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
      [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
   }
   SubShader
   {
      Tags
      {
         //"Queue"="Transparent"
         "IgnoreProjector"="True"
         "RenderType"="Transparent"
         "PreviewType"="Plane"
         "CanUseSpriteAtlas"="True"
      }

      Pass
      {

      Name "Base"
      Cull Off
      Lighting Off
      ZWrite Off
      Blend One OneMinusSrcAlpha
      CGPROGRAM
         #pragma vertex vert
         #pragma fragment MySpriteFrag
         //#pragma target 3.0
         #pragma multi_compile_instancing
         #pragma multi_compile _ PIXELSNAP_ON
         //#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
         #include "UnityCG.cginc"
         #include "UnitySprites.cginc"
         //fixed4 _Color;
         fixed4 _TintColor;
         //float4 _MainTex;
         float4 _MainTex_TexelSize;
         float _OutlineWidth;
         fixed4 _OutlineColor;
         float  _OutlineAlpha;
         float _OutlineUseGradient;
         float _GradientColor;
         float _GlobalAlpha;
         float _Diff;
         int _BlurRadius;
         float _TextureSize;
         //float _BlurRadius;
         struct v2f_blur
         {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;


                UNITY_VERTEX_OUTPUT_STEREO
         };

         v2f_blur vert(appdata_t IN)
         {
            v2f_blur OUT;

            UNITY_SETUP_INSTANCE_ID (IN);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

            OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
            OUT.vertex = UnityObjectToClipPos(OUT.vertex);
            OUT.texcoord = IN.texcoord;
            OUT.color = IN.color * _Color * _RendererColor;

            #ifdef PIXELSNAP_ON
            OUT.vertex = UnityPixelSnap (OUT.vertex);
            #endif

            return OUT;
         }
           float GetGaussWeight(float x, float y, float sigma)
            {
                float sigma2 = pow(sigma, 2.0f);
                float left = 1 / (2 * sigma2 * 3.1415926f);
                float right = exp(-(x*x+y*y)/(2*sigma2));
                return left * right;
            }
         
            fixed4 GaussBlur(float2 uv)
            {
                //因为高斯函数中3σ以外的点的权重已经很小了，因此σ取半径r/3的值
                float sigma = (float)_BlurRadius / 3.0f;
                float4 col = float4(0, 0, 0, 0) ;
                for (int x = - _BlurRadius; x <= _BlurRadius; ++x)
                {
                    for (int y = - _BlurRadius; y <= _BlurRadius; ++y)
                    {
                        //获取周围像素的颜色
                        //因为uv是0-1的一个值，而像素坐标是整形，我们要取材质对应位置上的颜色，需要将整形的像素坐标
                        //转为uv上的坐标值
                        float4 color = tex2D(_MainTex, uv + float2(x / _TextureSize, y / _TextureSize));
                        //获取此像素的权重
                        float weight = GetGaussWeight(x, y, sigma);
                        //计算此点的最终颜色
                        col += color * weight;
                                    
                    }
                }
                return col;
            }
            fixed4 GaussBlur2(float2 uv)
            {
                //因为高斯函数中3σ以外的点的权重已经很小了，因此σ取半径r/3的值
                float sigma = (float)_BlurRadius / 3.0f;
                float4 col = float4(0, 0, 0, 0) ;
                for (int x = - _BlurRadius; x <= _BlurRadius; ++x)
                {
                    for (int y = - _BlurRadius; y <= _BlurRadius; ++y)
                    {
                        //获取周围像素的颜色
                        //因为uv是0-1的一个值，而像素坐标是整形，我们要取材质对应位置上的颜色，需要将整形的像素坐标
                        //转为uv上的坐标值
                        float4 color = tex2D(_MainTex, uv + float2(x / _TextureSize, y / _TextureSize));
                        //获取此像素的权重
                        float weight = GetGaussWeight(x, y, sigma);
                        //计算此点的最终颜色
                        col += color * weight;
                                    
                    }
                }
                return fixed4(_TintColor.r,_TintColor.g,_TintColor.b,col.a);
            }
         fixed4 MySampleSpriteTexture(float2 uv)
         {
            fixed4 color = tex2D(_MainTex,uv);
            //GaussBlur(uv);
         #if ETC1_EXTERNAL_ALPHA
            fixed4 alpha = tex2D(_AlphaTex, uv);
            color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
         #endif
            color.rgb = _TintColor.rbg;
            return color;
         }
    
         fixed4 MySpriteFrag(v2f IN) : SV_Target
         {
            fixed4 c = MySampleSpriteTexture(IN.texcoord);
            fixed4 tintColorOrOutline = _RendererColor;
            if (_OutlineWidth > 0 && c.a != 0)
            {
                // Get the neighbouring four pixels.
                fixed4 pixelUp        = tex2D(_MainTex, IN.texcoord + fixed2(0,_OutlineWidth / _MainTex_TexelSize.w));
                fixed4 pixelRightUp   = tex2D(_MainTex, IN.texcoord + fixed2(_OutlineWidth / _MainTex_TexelSize.z, _OutlineWidth / _MainTex_TexelSize.w));
                fixed4 pixelLeftUp    = tex2D(_MainTex, IN.texcoord - fixed2(_OutlineWidth / _MainTex_TexelSize.z, 0)+ fixed2(0, _OutlineWidth / _MainTex_TexelSize.w));
                fixed4 pixelDown      = tex2D(_MainTex, IN.texcoord - fixed2(0, _OutlineWidth / _MainTex_TexelSize.w));
                fixed4 pixelDownRight = tex2D(_MainTex, IN.texcoord - fixed2(0, _OutlineWidth / _MainTex_TexelSize.w)+ fixed2(_OutlineWidth / _MainTex_TexelSize.z, 0));
                fixed4 pixelDownleft  = tex2D(_MainTex, IN.texcoord - fixed2(0, _OutlineWidth / _MainTex_TexelSize.w)- fixed2(_OutlineWidth / _MainTex_TexelSize.z, 0));
                fixed4 pixelRight     = tex2D(_MainTex, IN.texcoord + fixed2(_OutlineWidth / _MainTex_TexelSize.z, 0));
                fixed4 pixelLeft      = tex2D(_MainTex, IN.texcoord - fixed2(_OutlineWidth / _MainTex_TexelSize.z, 0));

                // If one of the neighbouring pixels is invisible, we render an outline.
                if (pixelRightUp.a * pixelLeftUp.a *pixelDownRight.a *pixelDownleft.a* pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a == 0)
                {
                    c.rgba = _OutlineColor;
                    c.a*= _OutlineAlpha;
                    tintColorOrOutline = _OutlineColor;
                }

                if(_OutlineUseGradient == 1)
                {
                    c+=pixelRightUp;
                    c+=pixelLeftUp;
                    c+=pixelDownRight;
                    c+=pixelDownleft;
                    c+=pixelDown;
                    c+=pixelUp;
                    c+=pixelRight;
                    c+=pixelLeft;
                    c*=_GradientColor;
                }
             }

            //fixed3 srcColor = tex2D(_MainTex, i.uv).rgb;
            float dx = IN.texcoord.x - 0.5;  
            float dy = IN.texcoord.y - 0.5;  
            float dstSq = pow(dx, 2.0) + pow(dy, 2.0); 
            float v = (dstSq / _Diff); 
            float r = clamp(c.r + v, 0.0, 1.0);  
            float g = clamp(c.g + v, 0.0, 1.0);  
            float b = clamp(c.b + v, 0.0, 1.0);
            c.a *= _GlobalAlpha;
            return fixed4(r * c.a, g*c.a, b*c.a,c.a)* GaussBlur(IN.texcoord) * tintColorOrOutline ;
                //
                //return c;
         }
      ENDCG
      }

    }
}