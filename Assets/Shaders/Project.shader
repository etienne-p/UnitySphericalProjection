Shader "Unlit/Project"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale("Scale", Float) = 0.05
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            
            float3 _PlaneNormal;
            float3 _PlanePoint;
            float4x4 _WorldToPlane;
            
            float3 _EyePosition;
            float _Scale;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // projection will be computed in world space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float rayPlaneIntersection(float3 rayOrigin, float3 rayDir, float3 planePoint, float3 planeNormal)
            {
                return dot(planePoint - rayOrigin, planeNormal) / dot(rayDir, planeNormal);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 eyeDir = normalize(i.worldPos - _EyePosition);
                float t = rayPlaneIntersection(_EyePosition, eyeDir, _PlanePoint, _PlaneNormal);
                float3 planeIntersect = _EyePosition + eyeDir * t;
                // convert to plane coordinates
                float4 planeCoords = mul(_WorldToPlane, planeIntersect);
                float2 uv = saturate(planeCoords.xz * float2(1, -1) * _Scale + 0.5) - 0.5;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
