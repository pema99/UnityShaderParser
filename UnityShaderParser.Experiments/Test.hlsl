#include "HLSLTest.hlsl"

[Test]
[TestCase(float3(1,2,3))]
[TestCase(float3(1,1,1))]
void TestNormalize(float3 testVal)
{
    ASSERT_MSG(length(normalize(testVal)) < 1.01, "Should be normalized");
}

[Test]
void TestSaturate()
{
    float f = saturate(50);
    ASSERT(f >= 0 && f <= 1);
}


int add(int a, int b)
{
    return a + b;
}

int testReturn()
{
    printf("BOO");
    if (WaveGetLaneIndex() < 2)
    {
        if (true)
        {
            return 1;
        }
    }
    printf("FOO");
}

[Test]
void main()
{    
    testReturn();
    
    printf("Printing stuff %f!", 42.69);
    
    // Values varying per thread
    int laneIndex = WaveGetLaneIndex();
    printf("%d", laneIndex);
    
    int i = 0;
    while (i < laneIndex)
    {
        printf("LOOP %d", i);
        i++;
    }

    // Derivatives
    float e = exp(laneIndex);
    printf("ddx(%f): %f", e, ddx_fine(e));

    // Coherent control flow
    if (lerp(1, 2, 0.5) > 0.5)
        printf("I'm coherent");

    // Divergent control flow
    if (laneIndex >= 2)
        printf("I'm divergent");

    // Function calls
    printf("Func call: %d", add(laneIndex, 2));
}
