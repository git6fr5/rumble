#ifndef MY_CUSTOM_FUNC
#define MY_CUSTOM_FUNC

#include "Assets/Resources/Visuals/Materials/Shaders/mycustomfunc.cginc"

// void horWave(float2 uv, float amp, float scrollWavelength, float timeWavelength, float time, out float2 _uv)
// {
//     _uv = float2(amp * sin(scrollWavelength * uv.x + timeWavelength * time), uv.y);
// }

void test(out float3 Out)
{
    Out = float3(1, 0, 0)
}

#endif
