#if !HLSL_TEST
#define printf
#endif

printf("Printing stuff %f!", 42.69);

// Values varying per thread
int laneIndex = WaveGetLaneIndex();
printf("%d", laneIndex);

// Derivatives
float e = exp(laneIndex);
printf("ddx(%f): %f", e, ddx_fine(e));

// Coherent control flow
if (lerp(1, 2, 0.5) > 0.5)
    printf("I'm coherent");

// Divergent control flow
if (laneIndex >= 2)
    printf("I'm divergent");
