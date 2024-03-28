
#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED


// This is a neat trick to work around a bug in the shader graph when
// enabling shadow keywords. Created by @cyanilux
// https://github.com/Cyanilux/URP_ShaderGraphCustomLighting
#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#if (SHADERPASS != SHADERPASS_FORWARD)
#undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
#endif
#endif


struct CustomLightingData {
    //Position and Orientation
    float3 positionWS;
    float3 normalWS;
    float3 viewDirectionWS;
    float4 shadowCoord;
    
    // Surface attributes
    float3 albedo;
    float smoothness;
    float toonRampOffset;
    float toonRampSmoothness;
    float3 toonRampTinting;
};

    // Translate a [0,1] smoothness value to an exponent 
float GetSmoothnessPower(float rawSmoothness){
    return exp2(10 * rawSmoothness + 1);
}
#ifndef SHADERGRAPH_PREVIEW
float3 CustomLightHandling(CustomLightingData d, Light light) {
    
    float3 radiance = light.color * (light.distanceAttenuation * light.shadowAttenuation);
    
    float diffuse = saturate(dot(d.normalWS, light.direction));
    half toonRamp = smoothstep(d.toonRampOffset, d.toonRampOffset + d.toonRampSmoothness, diffuse);
    float specularDot = saturate(dot(d.normalWS, normalize(light.direction + d.viewDirectionWS)));
    
    // Multiply with diffuse so highlights  can only appear in lighted areas
    float specular = pow(specularDot, GetSmoothnessPower(d.smoothness)) * toonRamp;
    toonRamp *= light.shadowAttenuation;
    float3 color = d.albedo * radiance * (toonRamp + d.toonRampTinting + specular);
    
    return color;
}
#endif
float3 CalculateCustomLighting(CustomLightingData d) {
    
    #ifdef SHADERGRAPH_PREVIEW
    // int preview, estimate diffuse + specular
    float3 lightDir = float3(0.5,0.5,0);
    float intensity = saturate(dot(d.normalWS, lightDir)) + pow(saturate(dot(d.normalWS, normalize(d.viewDirectionWS + lightDir))),GetSmoothnessPower(d.smoothness));
    return d.albedo * intensity;
    #else
    // Get the main light, Located in URP/ShaderLibrary/Lighting.hlsl
    Light mainLight = GetMainLight(d.shadowCoord, d.positionWS, 1);
    
    float3 color = 0;
    //Shade the main light
    color += CustomLightHandling(d, mainLight);
    
    #ifdef _ADDITIONAL_LIGHTS 
        // Shade additional cone CustomLightHandling point lights. Functions int URP/ShaderLibrary/CalculateCustomLighting.hlsl
        
        uint numAdditionalLights = GetAdditionalLightsCount();
        for (uint lightI = 0; lightI < numAdditionalLights; lightI++){
        Light light = GetAdditionalLight(lightI, d.positionWS, 1);
        color += CustomLightHandling(d, light);
        }
    
    #endif
    
    
    return color;
    #endif
}

void CalculateCustomLighting_float(float3 Position, float3 Normal, float3 ViewDirection, float3 Albedo, float Smoothness, float ToonRampOffset, float ToonRampSmoothness, float3 ToonRampTinting,
    out float3 Color) {

    CustomLightingData d;
    d.positionWS = Position;
    d.normalWS = Normal;
    d.viewDirectionWS = ViewDirection;
    d.albedo = Albedo;
    d.smoothness = Smoothness;
    d.toonRampOffset = ToonRampOffset;
    d.toonRampSmoothness = ToonRampSmoothness;
    d.toonRampTinting = ToonRampTinting;

    #ifdef SHADERGRAPH_PREVIEW
        //noise shadow in preview
        d.shadowCoord = 0;
    #else
        //Calculate the main light shadow coord
        //There are two types depending on if cascades are enabled
        float4 positionCS = TransformWorldToHClip(Position);
        #if SHADOWS_SCREEN
            d.shadowCoord = ComputeScreenPos(positionCS);
        #else
            d.shadowCoord = TransformWorldToShadowCoord(Position);
        #endif
    #endif
    Color = CalculateCustomLighting(d);
}

#endif