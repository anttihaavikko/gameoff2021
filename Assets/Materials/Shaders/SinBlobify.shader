Shader "CustomSprites/SinBlobify"
{
    Properties{
        _Speed ("Speed", Float) = 1
        _RotationSpeed ("RotationSpeed", Float) = 30
        _Offset ("Offset", Float) = 0
        _Amount ("Amount", Float) = 0.03
        _Peaks ("Peaks", Float) = 2
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

            float _Speed;
            float _RotationSpeed;
            float _Offset;
            float _Amount;
            float _Peaks;

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
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float phase = sin((_Time + _Offset) * 100.0 * _Speed);

                float x = v.uv.x - 0.5;
                float y = v.uv.y - 0.5;
                float angle = atan2(y, x);

                float diff = sin(angle * _Peaks + _Time * _RotationSpeed) * phase;

                o.position = UnityObjectToClipPos(v.vertex) + float4(x * _Amount * diff, y * _Amount * diff, 0, 0);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET{
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.color;
                return col;
            }

            ENDCG
        }
    }
}
