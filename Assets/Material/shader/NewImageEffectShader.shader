
Shader "Outline2D" {

    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Main texture Tint", Color) = (1,1,1,1)
        [PerRendererData]_TintColor("Dont change this too", Color) = (1,1,1,1)
        [Header(General Settings)]
        [MaterialToggle] _OutlineEnabled ("Outline Enabled", Float) = 1
        [MaterialToggle] _ConnectedAlpha ("Connected Alpha", Float) = 0
        [HideInInspector] _AlphaThreshold ("Alpha clean", Range (0, 1)) = 0
        _Thickness ("Width (Max recommended 100)", float) = 10
        [KeywordEnum(Solid, Gradient, Image)] _OutlineMode("Outline mode", Float) = 0
        [KeywordEnum(Contour, Frame)] _OutlineShape("Outline shape", Float) = 0
        [KeywordEnum(Inside under sprite, Inside over sprite, Outside)] _OutlinePosition("Outline Position (Frame Only)", Float) = 0

        [Header(Solid Settings)]
        _SolidOutline ("Outline Color Base", Color) = (1,1,1,1)

        [Header(Gradient Settings)]
        _GradientOutline1 ("Outline Color 1", Color) = (1,1,1,1)
        _GradientOutline2 ("Outline Color 2", Color) = (1,1,1,1)
        _Weight ("Weight", Range (0, 1)) = 0.5
        _Angle ("Gradient Angle (General gradient Only)", float) = 45
        //[KeywordEnum(General, Frame directed)] _FrameMode("Frame Mode (Frame Only)", Float) = 0

        [Header(Image Settings)]
        _FrameTex ("Frame Texture", 2D) = "white" {}
        _ImageOutline ("Outline Color Base", Color) = (1,1,1,1)
        [KeywordEnum(Stretch, Tile)] _TileMode("Frame mode", Float) = 0
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        Pass
        {
        CGPROGRAM
            //#pragma vertex vert
            #pragma vertex SpriteVert
            #pragma fragment frag
            //#pragma multi_compile _ PIXELSNAP_ON
            //#pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"
            #include "UnitySprites.cginc"


            fixed4 _TintColor;
            fixed _Thickness;
            fixed _OutlineEnabled;
            fixed _ConnectedAlpha;
            fixed _OutlineShape;
            fixed _OutlinePosition;
            fixed _OutlineMode;

            fixed4 _SolidOutline;

            fixed4 _GradientOutline1;
            fixed4 _GradientOutline2;
            fixed _Weight;
            fixed _AlphaThreshold;
            fixed _Angle;
            //fixed _FrameMode;

            fixed4 _ImageOutline;
            fixed _TileMode;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }
            float _AlphaSplitEnabled;
            uniform float4 _MainTex_TexelSize;

            sampler2D _FrameTex;
            uniform float4 _FrameTex_TexelSize;
            uniform float4 _FrameTex_ST;

            bool CheckOriginalSpriteTexture (float2 uv, bool ifZero)
            {
                float thicknessX = _Thickness / _MainTex_TexelSize.z;
                float thicknessY = _Thickness / _MainTex_TexelSize.w;
                int steps = 100;
                float angle_step = 360.0 / steps;

                float alphaThreshold = _AlphaThreshold / 10;
                float alphaCount = _AlphaThreshold * 10;

                // check if the basic points has an alpha to speed up the process and not use the for loop
                bool outline = false;
                float alphaCounter = 0;

                if(ifZero)
                {
                    
                }
                else
                {
                    outline =   SampleSpriteTexture(uv + fixed2(0, +thicknessY)).a > alphaThreshold ||
                                SampleSpriteTexture(uv + fixed2(0, -thicknessY)).a > alphaThreshold ||
                                SampleSpriteTexture(uv + fixed2(+thicknessX, 0)).a > alphaThreshold ||
                                SampleSpriteTexture(uv + fixed2(-thicknessX, 0)).a > alphaThreshold ||
                                SampleSpriteTexture(uv + fixed2(+thicknessX * cos (3.14 / 4), -thicknessY * sin (3.14 / 4))).a > alphaThreshold ||
                                SampleSpriteTexture(uv + fixed2(-thicknessX * cos (3.14 / 4), +thicknessY * sin (3.14 / 4))).a > alphaThreshold ||
                                SampleSpriteTexture(uv + fixed2(-thicknessX * cos (3.14 / 4), -thicknessY * sin (3.14 / 4))).a > alphaThreshold ||
                                SampleSpriteTexture(uv + fixed2(+thicknessX * cos (3.14 / 4), +thicknessY * sin (3.14 / 4))).a > alphaThreshold;
                }
                if(outline) return outline;

                for(int i = 0; i < steps; i++) // high number and not a variable to avoid stupid compiler bugs
                {
                    float angle = i * angle_step * 2 * 3.14 / 360;
                    if(ifZero && SampleSpriteTexture(uv + fixed2(thicknessX * cos(angle), thicknessY * sin(angle))).a == 0)
                    {
                        alphaCounter++;
                        if(alphaCounter >= alphaCount)
                        {
                            outline = true;
                            break;
                        }
                    }
                    else if(!ifZero && SampleSpriteTexture(uv + fixed2(thicknessX * cos(angle), thicknessY * sin(angle))).a > alphaThreshold)
                    {
                        outline = true;
                        break;
                    }
                }

                return outline;
            }

            fixed4 MySampleSpriteTexture(float2 uv)
             {
                fixed4 color = tex2D(_MainTex, uv);

             #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D(_AlphaTex, uv);
                color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
             #endif
                color.rgb = _TintColor.rgb;
                return color;
             }


            fixed4 frag(v2f IN) : SV_Target
            {
                float thicknessX = _Thickness / _MainTex_TexelSize.z;
                float thicknessY = _Thickness / _MainTex_TexelSize.w;

                fixed4 c = MySampleSpriteTexture(IN.texcoord) * IN.color;
                c.rgb = _TintColor.rgb;
                c.rgb *= c.a;

                fixed alpha;

                fixed4 outlineC = fixed4(0, 0, 0, 1);

                if(_OutlineEnabled != 0)
                {
                    if(_OutlineMode == 0) // Solid
                    {
                        outlineC = _SolidOutline;

                        if(_ConnectedAlpha != 0)
                        {
                            outlineC.a *= _Color.a;
                        }
                        outlineC.rgb *= outlineC.a;
                    }
                    else if(_OutlineMode == 1) // Gradient
                    {
                        float x = IN.texcoord.x;
                        float y = IN.texcoord.y;

                        float ratio1 = 0;
                        float ratio2 = 0;

                        if(_OutlineShape == 0) // contour
                        {
                            if(
                                ((_OutlinePosition != 2 && _OutlineShape == 1) && c.a != 0 &&  // inside and frame
                                    (
                                        IN.texcoord.y + thicknessY > 1 || 
                                        IN.texcoord.y - thicknessY < 0 || 
                                        IN.texcoord.x + thicknessX > 1 || 
                                        IN.texcoord.x - thicknessX < 0 ||
                                        CheckOriginalSpriteTexture(IN.texcoord, true)
                                    )
                                )
                                ||
                                ((_OutlinePosition == 2 || _OutlineShape != 1) && c.a == 0 &&   // outside or contour
                                    CheckOriginalSpriteTexture(IN.texcoord, false)
                                )
                            )
                            {
                                if(_Angle >= 360)
                                {
                                    int div = _Angle / 360;
                                    _Angle = (_Angle / 360 - div) * 360;
                                }
                                _Angle *= 2 * 3.14 / 360;

                                ratio1 = (0.5 - x) * cos(_Angle) + (0.5 - y) * sin(_Angle) + 0.5;
                                ratio2 = (x - 0.5) * cos(_Angle) + (y - 0.5) * sin(_Angle) + 0.5;

                                ratio1 *= 2 * _Weight;
                                ratio2 *= 2 * (1 - _Weight);

                                if(_ConnectedAlpha != 0)
                                {
                                    _GradientOutline1.a *= _Color.a;
                                    _GradientOutline2.a *= _Color.a;
                                    //outlineC.rgb *= outlineC.a;
                                }
                                _GradientOutline1.rgb *= _GradientOutline1.a;
                                _GradientOutline2.rgb *= _GradientOutline2.a;
                                outlineC = _GradientOutline1 * ratio1 + _GradientOutline2 * ratio2;
                            }
                        }
                        else if(_OutlineShape == 1) // frame
                        {
                            if( IN.texcoord.y + thicknessY > 1 ||
                                IN.texcoord.y - thicknessY < 0 ||
                                IN.texcoord.x + thicknessX > 1 ||
                                IN.texcoord.x - thicknessX < 0)
                            {
                                // between down left to up left
                                if (y * thicknessX - x * thicknessY > 0 &&
                                    y * thicknessX + x * thicknessY - thicknessX < 0 &&
                                    x < 0.5f)
                                {
                                    ratio1 = 1 - x / thicknessX;
                                    ratio2 = x / thicknessX;
                                }
                                // between down left to down right
                                else if (y * thicknessX - x * thicknessY < 0 &&
                                        y * thicknessX + x * thicknessY - thicknessY < 0 &&
                                        y < 0.5f)
                                {
                                    ratio1 = 1 - y / thicknessY;
                                    ratio2 = y / thicknessY;
                                }
                                // between down right to up right
                                else if (y * thicknessX - x * thicknessY - thicknessX + thicknessY < 0 &&
                                        y * thicknessX + x * thicknessY - thicknessY > 0 &&
                                        x > 0.5f)
                                {
                                    ratio1 = (x - 1) / thicknessX + 1;
                                    ratio2 = -(x - 1) / thicknessX;
                                }
                                // between up left to up right
                                else if (y * thicknessX - x * thicknessY - thicknessX + thicknessY > 0 &&
                                        y * thicknessX + x * thicknessY - thicknessX > 0 &&
                                        y > 0.5f)
                                {
                                    ratio1 = (y - 1) / thicknessY + 1;
                                    ratio2 = -(y - 1) / thicknessY;
                                }

                                ratio1 *= 2 * _Weight;
                                ratio2 *= 2 * (1 - _Weight);

                                if(_ConnectedAlpha != 0)
                                {
                                    _GradientOutline1.a *= _Color.a;
                                    _GradientOutline2.a *= _Color.a;
                                    //outlineC.rgb *= outlineC.a;
                                }
                                _GradientOutline1.rgb *= _GradientOutline1.a;
                                _GradientOutline2.rgb *= _GradientOutline2.a;
                                outlineC = _GradientOutline1 * ratio1 + _GradientOutline2 * ratio2;
                            }
                        }
                    }
                    else if(_OutlineMode == 2) // Image
                    {
                        outlineC = _ImageOutline;
                        fixed2 frame_coord;

                        if(_TileMode == 0)
                        {
                            frame_coord = IN.texcoord;
                        }
                        else if(_TileMode == 1)
                        {
                            frame_coord = fixed2
                            (
                                _FrameTex_ST.x * IN.texcoord.x * _MainTex_TexelSize.z / _FrameTex_TexelSize.z - _FrameTex_ST.z,
                                _FrameTex_ST.y * IN.texcoord.y * _MainTex_TexelSize.w / _FrameTex_TexelSize.w - _FrameTex_ST.w
                            );

                            if(frame_coord.x > 1)
                            {
                                frame_coord = fixed2
                                (
                                    frame_coord.x - floor(frame_coord.x),
                                    frame_coord.y
                                );
                            }
                            if(frame_coord.y > 1)
                            {
                                frame_coord = fixed2
                                (
                                    frame_coord.x,
                                    frame_coord.y - floor(frame_coord.y)
                                );
                            }
                        }
                        fixed4 text = tex2D(_FrameTex, frame_coord);

                        text.rgb *= text.a;

                        outlineC.rgb *= text.rgb;
                        outlineC.a *= text.a;

                        if(_ConnectedAlpha != 0)
                        {
                            outlineC.a *= _Color.a;
                        }
                        outlineC.rgb *= outlineC.a;
                    }

                    if(_OutlineShape == 1) // Frame
                    {
                        if( IN.texcoord.y + thicknessY > 1 ||
                            IN.texcoord.y - thicknessY < 0 ||
                            IN.texcoord.x + thicknessX > 1 ||
                            IN.texcoord.x - thicknessX < 0)
                        {
                            if(_OutlinePosition == 0 && c.a != 0 && _Thickness > 0)
                            {
                                return c;
                            }
                            else
                            {
                                return outlineC;
                            }
                        }
                        else
                        {
                            return c;
                        }
                    }
                    else if(_OutlineShape == 0 && _Thickness > 0) // Contour
                    {
                        if((_OutlinePosition != 2 && _OutlineShape == 1) && c.a != 0 && // inside and frame
                            (
                                IN.texcoord.y + thicknessY > 1 ||
                                IN.texcoord.y - thicknessY < 0 ||
                                IN.texcoord.x + thicknessX > 1 ||
                                IN.texcoord.x - thicknessX < 0 || 
                                CheckOriginalSpriteTexture(IN.texcoord, true)
                            )
                        )
                        {
                            return outlineC;
                        }
                        else if((_OutlinePosition == 2 || _OutlineShape != 1) && c.a == 0 && // outside orcontour
                                (
                                    CheckOriginalSpriteTexture(IN.texcoord, false)
                                )
                            )
                        {
                            return outlineC;
                        }
                        else
                        {
                            return c;
                        }
                    }
                    else
                    {
                        return c;
                    }
                }
                else
                {
                    return c;
                }

                return c;
                //return c;
            }
        ENDCG
        }
        Pass
      {
      CGPROGRAM
         #pragma vertex SpriteVert
         #pragma fragment MySpriteFrag
         #pragma target 2.0
         #pragma multi_compile_instancing
         #pragma multi_compile _ PIXELSNAP_ON
         #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
         #include "UnityCG.cginc"
         #include "UnitySprites.cginc"
         //fixed4 _Color;
         fixed4 _TintColor;
         float _OutlineWidth;
         fixed4 _OutlineColor;
         float4 _MainTex_TexelSize;

         fixed4 MySampleSpriteTexture(float2 uv)
         {
            fixed4 color = tex2D(_MainTex, uv);

         #if ETC1_EXTERNAL_ALPHA
            fixed4 alpha = tex2D(_AlphaTex, uv);
            color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
         #endif
            color.rgb = _TintColor.rgb;
            return color;
         }

         fixed4 MySpriteFrag(v2f IN) : SV_Target
         {
            fixed4 c = MySampleSpriteTexture(IN.texcoord) * IN.color;
            /*
            if (_OutlineWidth > 0 && c.a != 0) {
                    // Get the neighbouring four pixels.
                    fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, _MainTex_TexelSize.y));
                    fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, _MainTex_TexelSize.y));
                    fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(_MainTex_TexelSize.x, 0));
                    fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(_MainTex_TexelSize.x, 0));

                    // If one of the neighbouring pixels is invisible, we render an outline.
                    if (pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a == 0) {
                        c.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
                    }
            }
            */
            c.rgb *= c.a;

            return c;
         }


      ENDCG
      }
        
    }
}