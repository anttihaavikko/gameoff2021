Shader "CustomSprites/Comicfy"
{
    Properties{
        _Edge ("Edge Step", Float) = 0.8
        _Gray ("Gray Step", Float) = 0.4
        _GrayMod ("Gray Amount", Float) = 1
        _Invert ("Invert", Int) = 0
        
        _Color ("Main Color", Color) = (1,1,1,1)
        _GrayColor ("Shade Color", Color) = (0.5,0.5,0.5,1)
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader{
        Tags{ 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite off
        Cull off

        Pass{

            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color, _GrayColor;

            float _Speed, _Edge, _Gray, _GrayMod;
            int _Invert;

            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f{
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v){
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET{
                fixed4 col = tex2D(_MainTex, i.uv);
                float val = step(_Gray, col.r);
                float g = 1 - step(_Edge, col.r);
                float4 amount = float4(val, val, val, 1) - g * _GrayColor * _GrayMod;
                amount = _Invert > 0 ? float4(1, 1, 1, 1) - amount : amount;
                return float4(amount.rgb, col.a * _Color.a);
            }

            ENDCG
        }
    }
}
