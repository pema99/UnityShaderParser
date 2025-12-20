int laneIndex = WaveGetLaneIndex();

printf("%d", 123);

printf("=== Lerp test ===");
float4 result = lerp(float4(float2(1,2),3,4), 1 + laneIndex * 4, 0.5);
printf("%d", result);

printf("=== Exp test ===");
float e = exp(laneIndex);
printf("exp(%d): %d", laneIndex, e);

printf("=== DDX test ===");
printf("DDX: %d", ddx_fine(e));