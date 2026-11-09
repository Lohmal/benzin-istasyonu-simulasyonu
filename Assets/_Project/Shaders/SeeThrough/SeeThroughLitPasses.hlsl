#ifndef BENZIN_SEE_THROUGH_LIT_PASSES_INCLUDED
#define BENZIN_SEE_THROUGH_LIT_PASSES_INCLUDED

// URP Lit pass fragment fonksiyonlarını saran versiyonlar: önce delik kontrolü, sonra orijinal fonksiyon.
// Her pass, URP'nin kendi pass include'undan SONRA bu dosyayı ve ilgili SEE_THROUGH_PASS_* tanımını kullanır.
// ShadowCaster bilerek sarılmaz: ışığın bakış açısında delik açılırsa gölgede rastgele delikler oluşur.

#include "Assets/_Project/Shaders/SeeThrough/SeeThroughCutout.hlsl"

#if defined(SEE_THROUGH_PASS_FORWARD)
void LitPassFragmentSeeThrough(
    Varyings input
    , out half4 outColor : SV_Target0
#ifdef _WRITE_RENDERING_LAYERS
    , out uint outRenderingLayers : SV_Target1
#endif
)
{
    SeeThroughClip(input.positionCS);
    LitPassFragment(input, outColor
#ifdef _WRITE_RENDERING_LAYERS
        , outRenderingLayers
#endif
    );
}
#endif

#if defined(SEE_THROUGH_PASS_GBUFFER)
GBufferFragOutput LitGBufferPassFragmentSeeThrough(Varyings input)
{
    SeeThroughClip(input.positionCS);
    return LitGBufferPassFragment(input);
}
#endif

#if defined(SEE_THROUGH_PASS_DEPTHONLY)
half DepthOnlyFragmentSeeThrough(Varyings input) : SV_TARGET
{
    SeeThroughClip(input.positionCS);
    return DepthOnlyFragment(input);
}
#endif

#if defined(SEE_THROUGH_PASS_DEPTHNORMALS)
void DepthNormalsFragmentSeeThrough(
    Varyings input
    , out half4 outNormalWS : SV_Target0
#ifdef _WRITE_RENDERING_LAYERS
    , out uint outRenderingLayers : SV_Target1
#endif
)
{
    SeeThroughClip(input.positionCS);
    DepthNormalsFragment(input, outNormalWS
#ifdef _WRITE_RENDERING_LAYERS
        , outRenderingLayers
#endif
    );
}
#endif

#endif
