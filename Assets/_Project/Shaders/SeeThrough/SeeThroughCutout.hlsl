#ifndef BENZIN_SEE_THROUGH_CUTOUT_INCLUDED
#define BENZIN_SEE_THROUGH_CUTOUT_INCLUDED

// Duvarın arkasında kalan oyuncunun etrafında ekran uzayında yuvarlak bir delik açar.
// Değerler SeeThroughController tarafından global olarak gönderilir (materyal başına ayar yok).

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// xyz: oyuncunun gövde noktası (dünya), w: ayak yüksekliği (dünya y)
float4 _SeeThroughTarget;
// x: açıklık (0-1), y: yarıçap (ekran yüksekliğinin oranı), z: kenar yumuşaklığı (0-1), w: dither (0 = keskin kenar, 1 = noktalı kenar)
float4 _SeeThroughParams;
// x: ayaktan itibaren kesilmeyecek yükseklik (zemin delinmesin), y: oyuncudan kameraya doğru derinlik payı
float4 _SeeThroughParams2;

// 4x4 Bayer matrisi, 0-1 arası eşik
float SeeThroughBayer4x4(uint2 p)
{
    const float bayer[16] =
    {
         0.0,  8.0,  2.0, 10.0,
        12.0,  4.0, 14.0,  6.0,
         3.0, 11.0,  1.0,  9.0,
        15.0,  7.0, 13.0,  5.0
    };
    return (bayer[(p.y & 3) * 4 + (p.x & 3)] + 0.5) / 16.0;
}

// positionCS: fragment'taki SV_POSITION (piksel koordinatı + cihaz derinliği)
void SeeThroughClip(float4 positionCS)
{
    float amount = _SeeThroughParams.x;
    if (amount <= 0.001)
        return;

    // Pikselin dünya pozisyonunu geri hesapla (her pass'ta çalışsın diye Varyings'e bağlı değil)
    float2 screenUV = positionCS.xy / _ScaledScreenParams.xy;
    float3 pixelWS = ComputeWorldSpacePosition(screenUV, positionCS.z, UNITY_MATRIX_I_VP);

    // Zemin ve ayak hizasındaki şeyler kesilmez
    if (pixelWS.y < _SeeThroughTarget.w + _SeeThroughParams2.x)
        return;

    float3 pixelVS = TransformWorldToView(pixelWS);
    float3 targetVS = TransformWorldToView(_SeeThroughTarget.xyz);

    // Sadece kameraya oyuncudan daha yakın pikseller kesilir (view space'te -z = derinlik)
    if (-pixelVS.z > -targetVS.z - _SeeThroughParams2.y)
        return;

    // Ek olarak piksel, yatayda oyuncunun kamera tarafında olmalı.
    // Yoksa oyuncunun arkasındaki duvarın üst kenarı (yüksek olduğu için kameraya daha yakın sayılır) da kesilir.
    float2 toCamXZ = _WorldSpaceCameraPos.xz - _SeeThroughTarget.xz;
    float toCamLen = length(toCamXZ);
    if (toCamLen > 0.5)
    {
        float side = dot(pixelWS.xz - _SeeThroughTarget.xz, toCamXZ / toCamLen);
        if (side < _SeeThroughParams2.y)
            return;
    }

    // İkisini de aynı projeksiyonla ekrana taşı, en-boy oranını düzelt
    float4 pixelCS = mul(UNITY_MATRIX_P, float4(pixelVS, 1.0));
    float4 targetCS = mul(UNITY_MATRIX_P, float4(targetVS, 1.0));
    float2 delta = pixelCS.xy / pixelCS.w - targetCS.xy / targetCS.w;
    delta.x *= _ScaledScreenParams.x / _ScaledScreenParams.y;
    float dist = length(delta) * 0.5; // ekran yüksekliğinin oranı cinsinden

    float radius = _SeeThroughParams.y * amount;
    float inner = radius * (1.0 - saturate(_SeeThroughParams.z));
    float cut = 1.0 - smoothstep(inner, radius + 1e-5, dist);

    float threshold = lerp(0.5, SeeThroughBayer4x4((uint2)positionCS.xy), _SeeThroughParams.w);
    clip(threshold - cut);
}

#endif
