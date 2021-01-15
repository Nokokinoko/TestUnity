Shader "Custom/EqualityTilingShader"
{
    Properties
    {
        _PixelPerUnit ("Pixels Per Unit", Float) = 128.0
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.5
        [Gamma] _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
        _BumpScale ("Bump Scale", Float) = 1.0
        [Normal][NoScaleOffset] _BumpMap ("Normal", 2D) = "bump" {}
        [NoScaleOffset] _OcclusionMap ("Occlusion", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
            "DisableBatching" = "True"
        }

        CGPROGRAM

        #pragma exclude_renderers gles
        #pragma surface surf Standard vertex:vert fullforwardshadows addshadow
        #pragma target 3.5

        float _PixelPerUnit;
        fixed4 _Color;
        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;
        half _Glossiness;
        half _Metallic;
        half _BumpScale;
        sampler2D _BumpMap;
        sampler2D _OcclusionMap;

        struct Input
        {
            float3 objectUV;
            float3 objectNormal;
            float4 objectTangent;
        };

        // カットオフ方式の混合比算出
        float3 cutoffBlending(float3 normalAbs, float cutoff)
        {
            float3 factor = max(normalAbs - cutoff, 0.0001);
            return factor / dot(factor, (float3)1.0);
        }

        // べき乗方式の混合比算出
        float3 powerBlending(float3 normalAbs, float exponent)
        {
            float3 factor = pow(normalAbs, exponent);
            return factor / dot(factor, (float3)1.0);
        }

        // Y-XZ非対称方式の混合比算出
        float3 asymmetricBlending(float3 normalAbs, float exponent, float capCos, float borderHalfWidth)
        {
            float normalAbsXZMax = max(normalAbs.x, normalAbs.z);
            float2 factorXZ = pow(normalAbsXZMax > 0.0 ? normalAbs.xz / normalAbsXZMax : (float2)1.0, exponent);
            factorXZ /= dot(factorXZ, (float2)1.0);
            float factorY = smoothstep(capCos - borderHalfWidth, capCos + borderHalfWidth, normalAbs.y);
            float factorH = 1.0 - factorY;
            float3 factor = float3(factorXZ.x * factorH, factorY, factorXZ.y * factorH);
            return factor;
        }

        // 虚部xyz、実部wの回転クォータニオンを行列にする
        float3x3 rotationFromQuaternion(float4 q)
        {
            float3 nA = q.xyz * 2.0;
            float3 nB = q.xyz * nA;
            float3 nC = q.xxy * nA.yzz;
            float3 nD = q.www * nA;
            float3 nE = 1.0 - (nB.yzx + nB.zxy);
            float3 nF = nC + nD.zyx;
            float3 nG = nC - nD.zyx;
            return float3x3(
                nE.x, nG.x, nF.y,
                nF.x, nE.y, nG.z,
                nG.y, nF.z, nE.z
            );
        }

        // fromからtoへの最短回転行列を作成
        float3x3 fromToRotation(float3 from, float3 to)
        {
            float3 axisU = cross(from, to);
            float axisULength = length(axisU);
            float clampedAxisULength = max(axisULength, 0.001);
            float angleIsValid = sign(axisULength);
            float cosTheta = dot(from, to);
            float3 axis = angleIsValid > 0.0 ? axisU / clampedAxisULength : float3(1.0, 0.0, 0.0);
            float2 cosThetaHSinThetaH = sqrt(((float2)1.0 + float2(1.0, -1.0) * cosTheta) * 0.5);
            return rotationFromQuaternion(float4(axis * cosThetaHSinThetaH.y, cosThetaHSinThetaH.x));
        }

        void vert(inout appdata_full v, out Input IN)
        {
            UNITY_INITIALIZE_OUTPUT(Input, IN);

            // 3面のUV座標
            IN.objectUV = v.vertex.xyz * float3(
                length(unity_ObjectToWorld._m00_m10_m20),
                length(unity_ObjectToWorld._m01_m11_m21),
                length(unity_ObjectToWorld._m02_m12_m22)
            );

            IN.objectNormal = v.normal; // 法線
            IN.objectTangent = v.tangent; // 接線
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // オブジェクト空間からUnity接空間への変換
            float3 objectNormal = normalize(IN.objectNormal);
            float3 objectTangent = normalize(IN.objectTangent.xyz);
            float3 objectBinormal = cross(objectNormal, objectTangent) * IN.objectTangent.w;
            float3x3 objectToTangent = float3x3(objectTangent, objectBinormal, objectNormal);
            // オブジェクト法線の成分の絶対値
            float3 objectNormalAbs = abs(objectNormal);
            // オブジェクト法線の成分の符号
            float3 s = sign(objectNormal);

            // 3方向のUV座標
            float2 uvMultiplier = _PixelPerUnit * _MainTex_TexelSize.xy * _MainTex_ST.xy;
            float2 objectUVX = _MainTex_ST.zw + uvMultiplier * IN.objectUV.zy * float2(s.x, 1.0);
            float2 objectUVY = _MainTex_ST.zw + uvMultiplier * IN.objectUV.zx * float2(-1.0, s.y);
            float2 objectUVZ = _MainTex_ST.zw + uvMultiplier * IN.objectUV.xy * float2(-s.z, 1.0);

            // 3方向の平面接空間からオブジェクト空間への変換
            float3 tPXNormal = float3(s.x, 0.0, 0.0);
            float3 tPYNormal = float3(0.0, s.y, 0.0);
            float3 tPZNormal = float3(0.0, 0.0, s.z);
            float3 tPXTangent = float3(0.0, 0.0, s.x);
            float3 tPYTangent = float3(0.0, 0.0, -1.0);
            float3 tPZTangent = float3(-s.z, 0.0, 0.0);
            float3 tPXBinormal = float3(0.0, 1.0, 0.0);
            float3 tPYBinormal = float3(s.y, 0.0, 0.0);
            float3 tPZBinormal = float3(0.0, 1.0, 0.0);
            float3x3 tPXToObject = transpose(float3x3(tPXTangent, tPXBinormal, tPXNormal));
            float3x3 tPYToObject = transpose(float3x3(tPYTangent, tPYBinormal, tPYNormal));
            float3x3 tPZToObject = transpose(float3x3(tPZTangent, tPZBinormal, tPZNormal));

            // 3方向の法線からオブジェクト法線への回転を求め、平面接空間からUnity接空間への合成変換
            float3x3 tPXToTangent = mul(objectToTangent, mul(fromToRotation(tPXNormal, objectNormal), tPXToObject));
            float3x3 tPYToTangent = mul(objectToTangent, mul(fromToRotation(tPYNormal, objectNormal), tPYToObject));
            float3x3 tPZToTangent = mul(objectToTangent, mul(fromToRotation(tPZNormal, objectNormal), tPZToObject));

            // 3方向の法線をテクスチャから取得、Unity接空間上のベクトルに変換
            float3x3 normals = transpose(float3x3(
                mul(tPXToTangent, UnpackScaleNormal(tex2D(_BumpMap, objectUVX), _BumpScale)),
                mul(tPYToTangent, UnpackScaleNormal(tex2D(_BumpMap, objectUVY), _BumpScale)),
                mul(tPZToTangent, UnpackScaleNormal(tex2D(_BumpMap, objectUVZ), _BumpScale))
            ));

            // 3方向のアルベド・遮蔽率をテクスチャから取得
            float4x3 albedoOcclusions = transpose(float3x4(
                tex2D(_MainTex, objectUVX).rgb, tex2D(_OcclusionMap, objectUVX).a,
                tex2D(_MainTex, objectUVY).rgb, tex2D(_OcclusionMap, objectUVY).a,
                tex2D(_MainTex, objectUVZ).rgb, tex2D(_OcclusionMap, objectUVZ).a
            ));

            // 3方向の混合比を算出
            float3 factor = asymmetricBlending(objectNormalAbs, 64.0, 0.5, 1.0 / 64.0);

            // アルベド・遮蔽率を合成
            float4 albedoOcclusion = mul(albedoOcclusions, factor) * float4(_Color.rgb, 1.0);

            // 法線を合成
            float3 normal = normalize(mul(normals, factor));

            o.Albedo = albedoOcclusion.rgb;
            o.Normal = float3(0.0, 0.0, 1.0);
            o.Metallic = 0.0;
            o.Smoothness = 0.0;
            o.Occlusion = albedoOcclusion.a;
            o.Normal = normal;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack Off
}
