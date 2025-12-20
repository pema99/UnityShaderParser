int a = WaveGetLaneIndex();
printf("%d", lerp(float4(float2(1,2),3,4), 1 + a * 4, 0.5));