Shader "Custom/RadialGradient" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}  // 添加这个必需的纹理属性
        _ColorInner ("Inner Color", Color) = (1,1,1,1)
        _ColorOuter ("Outer Color", Color) = (0,0,0,1)
        _Radius ("Radius", Range(0, 1)) = 0.5
    }
    SubShader {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "PreviewType"="Plane"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;  // 声明纹理
            fixed4 _ColorInner;
            fixed4 _ColorOuter;
            float _Radius;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
            float2 center = float2(0.5, 0.5);
            float dist = distance(i.uv, center);
    
            // 固定的完美圆形（带抗锯齿）
            float fixedCircle = 1.0 - smoothstep(0.48, 0.5, dist);
    
            // Radius只控制颜色渐变，不影响形状
            float colorT = saturate(dist / _Radius);
            fixed4 col = lerp(_ColorInner, _ColorOuter, colorT);
    
            // 应用固定的圆形Alpha
            col.a *= fixedCircle;
    
            return col;
            }
            ENDCG
        }
    }
}