[Test]
[WarpSize(2, 2)]
void Intrinsic_QuadReadAcrossDiagonal()
{
    uint lane = WaveGetLaneIndex();
    float value = lane * 10.0;
    
    // Test diagonal reads in a 2x2 quad
    // Lane layout: 0 1
    //              2 3
    float diag = QuadReadAcrossDiagonal(value);
    
    ASSERT(WaveReadLaneAt(diag, 0) == 30.0); // Lane 0 reads from lane 3
    ASSERT(WaveReadLaneAt(diag, 1) == 20.0); // Lane 1 reads from lane 2
    ASSERT(WaveReadLaneAt(diag, 2) == 10.0); // Lane 2 reads from lane 1
    ASSERT(WaveReadLaneAt(diag, 3) == 0.0);  // Lane 3 reads from lane 0
    
    // Test with negative values
    float negValue = -lane * 5.0;
    float negDiag = QuadReadAcrossDiagonal(negValue);
    
    ASSERT(WaveReadLaneAt(negDiag, 0) == -15.0);
    ASSERT(WaveReadLaneAt(negDiag, 1) == -10.0);
    ASSERT(WaveReadLaneAt(negDiag, 2) == -5.0);
    ASSERT(WaveReadLaneAt(negDiag, 3) == 0.0);
    
    // Test in varying control flow
    float conditional = 0.0;
    if (lane >= 2)
        conditional = QuadReadAcrossDiagonal(100.0);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == 0.0);
    ASSERT(WaveReadLaneAt(conditional, 1) == 0.0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 100.0); // Reads from lane 3
    ASSERT(WaveReadLaneAt(conditional, 3) == 100.0); // Reads from lane 2
}

[Test]
[WarpSize(2, 2)]
void Intrinsic_QuadReadLaneAt()
{
    uint lane = WaveGetLaneIndex();
    float value = lane * 10.0;
    
    // Each lane reads from lane 0
    float fromLane0 = QuadReadLaneAt(value, 0);
    ASSERT(fromLane0 == 0.0);
    
    // Each lane reads from lane 1
    float fromLane1 = QuadReadLaneAt(value, 1);
    ASSERT(fromLane1 == 10.0);
    
    // Each lane reads from lane 2
    float fromLane2 = QuadReadLaneAt(value, 2);
    ASSERT(fromLane2 == 20.0);
    
    // Each lane reads from lane 3
    float fromLane3 = QuadReadLaneAt(value, 3);
    ASSERT(fromLane3 == 30.0);
    
    // Test with negative values
    float negValue = -lane - 1.0;
    ASSERT(QuadReadLaneAt(negValue, 0) == -1.0);
    ASSERT(QuadReadLaneAt(negValue, 3) == -4.0);
    
    // Test reading same lane
    float sameLane = QuadReadLaneAt(value, lane);
    ASSERT(sameLane == value);
}

[Test]
[WarpSize(2, 2)]
void Intrinsic_QuadReadAcrossX()
{
    uint lane = WaveGetLaneIndex();
    float value = lane * 10.0;
    
    // Test horizontal swap in a 2x2 quad
    // Lane layout: 0 1
    //              2 3
    float acrossX = QuadReadAcrossX(value);
    
    ASSERT(WaveReadLaneAt(acrossX, 0) == 10.0); // Lane 0 reads from lane 1
    ASSERT(WaveReadLaneAt(acrossX, 1) == 0.0);  // Lane 1 reads from lane 0
    ASSERT(WaveReadLaneAt(acrossX, 2) == 30.0); // Lane 2 reads from lane 3
    ASSERT(WaveReadLaneAt(acrossX, 3) == 20.0); // Lane 3 reads from lane 2
    
    // Test with vector values
    float3 vecValue = float3(lane, lane * 2.0, lane * 3.0);
    float3 vecAcross = QuadReadAcrossX(vecValue);
    
    ASSERT(WaveReadLaneAt(vecAcross.x, 0) == 1.0);
    ASSERT(WaveReadLaneAt(vecAcross.y, 0) == 2.0);
    ASSERT(WaveReadLaneAt(vecAcross.z, 0) == 3.0);
    
    // Test with negative values
    float negValue = -lane - 1.0;
    float negAcross = QuadReadAcrossX(negValue);
    
    ASSERT(WaveReadLaneAt(negAcross, 0) == -2.0);
    ASSERT(WaveReadLaneAt(negAcross, 1) == -1.0);
}

[Test]
[WarpSize(2, 2)]
void Intrinsic_QuadReadAcrossY()
{
    uint lane = WaveGetLaneIndex();
    float value = lane * 10.0;
    
    // Test vertical swap in a 2x2 quad
    // Lane layout: 0 1
    //              2 3
    float acrossY = QuadReadAcrossY(value);
    
    ASSERT(WaveReadLaneAt(acrossY, 0) == 20.0); // Lane 0 reads from lane 2
    ASSERT(WaveReadLaneAt(acrossY, 1) == 30.0); // Lane 1 reads from lane 3
    ASSERT(WaveReadLaneAt(acrossY, 2) == 0.0);  // Lane 2 reads from lane 0
    ASSERT(WaveReadLaneAt(acrossY, 3) == 10.0); // Lane 3 reads from lane 1
    
    // Test with vector values
    float2 vecValue = float2(lane, lane * 5.0);
    float2 vecAcross = QuadReadAcrossY(vecValue);
    
    ASSERT(WaveReadLaneAt(vecAcross.x, 0) == 2.0);
    ASSERT(WaveReadLaneAt(vecAcross.y, 0) == 10.0);
    ASSERT(WaveReadLaneAt(vecAcross.x, 3) == 1.0);
    ASSERT(WaveReadLaneAt(vecAcross.y, 3) == 5.0);
    
    // Test with negative values
    float negValue = -lane * 2.0;
    float negAcross = QuadReadAcrossY(negValue);
    
    ASSERT(WaveReadLaneAt(negAcross, 0) == -4.0);
    ASSERT(WaveReadLaneAt(negAcross, 2) == 0.0);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveAllEqual()
{
    uint lane = WaveGetLaneIndex();
    
    // All lanes have same value
    bool allSame = WaveActiveAllEqual(5.0);
    ASSERT(allSame);
    
    // All lanes have different values
    bool allDifferent = WaveActiveAllEqual(lane);
    ASSERT(!allDifferent);
    
    // Test in uniform control flow with same value
    float uniformValue = 42.0;
    ASSERT(WaveActiveAllEqual(uniformValue));
    
    // Test in varying control flow
    float varyingValue = (lane >= 2) ? 1.0 : 0.0;
    bool varyingEqual = WaveActiveAllEqual(varyingValue);
    ASSERT(!varyingEqual);
    
    // Test with negative values
    ASSERT(WaveActiveAllEqual(-10.0));
    
    // Test with zero
    ASSERT(WaveActiveAllEqual(0.0));
    
    // Test where only some lanes match
    float partialMatch = (lane == 0 || lane == 1) ? 100.0 : 200.0;
    ASSERT(!WaveActiveAllEqual(partialMatch));
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveBitAnd()
{
    uint lane = WaveGetLaneIndex();
    
    // All bits set
    uint allOnes = WaveActiveBitAnd(0xFFFFFFFF);
    ASSERT(allOnes == 0xFFFFFFFF);
    
    // Specific bit pattern
    uint pattern = WaveActiveBitAnd(0xFF00FF00);
    ASSERT(pattern == 0xFF00FF00);
    
    // Different values per lane (0, 1, 2, 3)
    // Binary: 0000, 0001, 0010, 0011
    // AND result: 0000
    uint laneAnd = WaveActiveBitAnd(lane);
    ASSERT(laneAnd == 0);
    
    // Test with mask where lane 0,1,2,3 have different bits
    // Lane 0: 1111, Lane 1: 1110, Lane 2: 1101, Lane 3: 1100
    uint mask = 0xF & ~lane;
    uint maskAnd = WaveActiveBitAnd(mask);
    ASSERT(maskAnd == 0xC); // Only bits set in all lanes: 1100
    
    // Test in varying control flow
    uint conditional = 0xFFFFFFFF;
    if (lane >= 2)
        conditional = 0x0000FFFF;
    uint condAnd = WaveActiveBitAnd(conditional);
    // Only lanes >= 2 participate, so result is 0x0000FFFF
    ASSERT(WaveReadLaneAt(condAnd, 2) == 0x0000FFFF);
    ASSERT(WaveReadLaneAt(condAnd, 3) == 0x0000FFFF);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveBitOr()
{
    uint lane = WaveGetLaneIndex();
    
    // All zeros
    uint allZeros = WaveActiveBitOr(0);
    ASSERT(allZeros == 0);
    
    // Single bit per lane
    // Lane 0: 0001, Lane 1: 0010, Lane 2: 0100, Lane 3: 1000
    uint singleBit = WaveActiveBitOr(1 << lane);
    ASSERT(singleBit == 0xF); // 1111
    
    // Mix of patterns
    uint pattern = lane * 0x11111111;
    uint orResult = WaveActiveBitOr(pattern);
    // Lane 0: 0x00000000
    // Lane 1: 0x11111111
    // Lane 2: 0x22222222
    // Lane 3: 0x33333333
    // OR: 0x33333333
    ASSERT(orResult == 0x33333333);
    
    // Test with full bits
    ASSERT(WaveActiveBitOr(0xFFFFFFFF) == 0xFFFFFFFF);
    
    // Test in varying control flow
    uint conditional = 0;
    if (lane >= 2)
        conditional = 1 << lane;
    uint condOr = WaveActiveBitOr(conditional);
    // Lanes 2 and 3: 0100 | 1000 = 1100
    ASSERT(WaveReadLaneAt(condOr, 2) == 0xC);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveBitXor()
{
    uint lane = WaveGetLaneIndex();
    
    // All same value - XOR should be 0
    uint allSame = WaveActiveBitXor(0xFF);
    ASSERT(allSame == 0); // Even number of lanes (4)
    
    // Lane indices: 0, 1, 2, 3
    // XOR: 0 ^ 1 ^ 2 ^ 3 = 0
    uint laneXor = WaveActiveBitXor(lane);
    ASSERT(laneXor == 0);
    
    // Single bit set in each lane
    uint singleBit = WaveActiveBitXor(1 << lane);
    ASSERT(singleBit == 0xF); // All bits XOR together
    
    // Alternating pattern
    uint alternating = (lane & 1) ? 0xAAAAAAAA : 0x55555555;
    uint altXor = WaveActiveBitXor(alternating);
    ASSERT(altXor == 0x00000000); // Two of each cancel out in XOR
    
    // Test with zero
    ASSERT(WaveActiveBitXor(0) == 0);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveCountBits()
{
    uint lane = WaveGetLaneIndex();
    
    // All true
    uint allTrue = WaveActiveCountBits(true);
    ASSERT(allTrue == 4);
    
    // All false
    uint allFalse = WaveActiveCountBits(false);
    ASSERT(allFalse == 0);
    
    // Half true, half false
    bool halfTrue = lane < 2;
    uint halfCount = WaveActiveCountBits(halfTrue);
    ASSERT(halfCount == 2);
    
    // Only first lane
    bool firstOnly = lane == 0;
    uint firstCount = WaveActiveCountBits(firstOnly);
    ASSERT(firstCount == 1);
    
    // Only last lane
    bool lastOnly = lane == 3;
    uint lastCount = WaveActiveCountBits(lastOnly);
    ASSERT(lastCount == 1);
    
    // Odd lanes
    bool oddLanes = (lane & 1) == 1;
    uint oddCount = WaveActiveCountBits(oddLanes);
    ASSERT(oddCount == 2);
    
    // Test in varying control flow
    uint conditional = 0;
    if (lane >= 2)
        conditional = WaveActiveCountBits(true);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == 0);
    ASSERT(WaveReadLaneAt(conditional, 1) == 0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 2); // Lanes 2 and 3 active
    ASSERT(WaveReadLaneAt(conditional, 3) == 2);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveMax()
{
    uint lane = WaveGetLaneIndex();
    
    // All same value
    float allSame = WaveActiveMax(5.0);
    ASSERT(allSame == 5.0);
    
    // Lane indices as values (0, 1, 2, 3)
    float laneMax = WaveActiveMax(lane);
    ASSERT(laneMax == 3.0);
    
    // Negative values
    float negMax = WaveActiveMax(-lane);
    ASSERT(negMax == 0.0); // Max of 0, -1, -2, -3
    
    // Mixed positive and negative
    float mixedMax = WaveActiveMax((lane < 2) ? lane : -lane);
    ASSERT(mixedMax == 1.0); // Max of 0, 1, -2, -3
    
    // Large values
    float largeMax = WaveActiveMax(lane * 1000.0);
    ASSERT(largeMax == 3000.0);
    
    // Test in varying control flow
    float conditional = -999.0;
    if (lane >= 2)
        conditional = WaveActiveMax(lane * 10.0);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == -999.0);
    ASSERT(WaveReadLaneAt(conditional, 1) == -999.0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 30.0); // Max of lanes 2,3
    ASSERT(WaveReadLaneAt(conditional, 3) == 30.0);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveMin()
{
    uint lane = WaveGetLaneIndex();
    
    // All same value
    float allSame = WaveActiveMin(5.0);
    ASSERT(allSame == 5.0);
    
    // Lane indices as values (0, 1, 2, 3)
    float laneMin = WaveActiveMin(lane);
    ASSERT(laneMin == 0.0);
    
    // Negative values
    float negMin = WaveActiveMin(-lane);
    ASSERT(negMin == -3.0); // Min of 0, -1, -2, -3
    
    // Mixed positive and negative
    float mixedMin = WaveActiveMin((lane < 2) ? lane : -lane);
    ASSERT(mixedMin == -3.0); // Min of 0, 1, -2, -3
    
    // Large values
    float largeMin = WaveActiveMin(lane * 1000.0 + 100.0);
    ASSERT(largeMin == 100.0);
    
    // Test in varying control flow
    float conditional = 999.0;
    if (lane >= 2)
        conditional = WaveActiveMin(lane * 10.0);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == 999.0);
    ASSERT(WaveReadLaneAt(conditional, 1) == 999.0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 20.0); // Min of lanes 2,3
    ASSERT(WaveReadLaneAt(conditional, 3) == 20.0);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveProduct()
{
    uint lane = WaveGetLaneIndex();
    
    // All ones
    float allOnes = WaveActiveProduct(1.0);
    ASSERT(allOnes == 1.0);
    
    // All twos: 2 * 2 * 2 * 2 = 16
    float allTwos = WaveActiveProduct(2.0);
    ASSERT(allTwos == 16.0);
    
    // Lane indices + 1: 1 * 2 * 3 * 4 = 24
    float laneProduct = WaveActiveProduct(lane + 1.0);
    ASSERT(laneProduct == 24.0);
    
    // With zero (product should be zero)
    float withZero = WaveActiveProduct((lane == 2) ? 0.0 : 1.0);
    ASSERT(withZero == 0.0);
    
    // Negative values: -1 * -2 * -3 * -4 = 24
    float negProduct = WaveActiveProduct(-(lane + 1.0));
    ASSERT(negProduct == 24.0); // Even number of negatives
    
    // Fractional values: 0.5^4 = 0.0625
    float fracProduct = WaveActiveProduct(0.5);
    ASSERT(abs(fracProduct - 0.0625) < 0.001);
    
    // Test in varying control flow
    float conditional = 1.0;
    if (lane >= 2)
        conditional = WaveActiveProduct(2.0);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == 1.0);
    ASSERT(WaveReadLaneAt(conditional, 1) == 1.0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 4.0); // 2 * 2 from lanes 2,3
    ASSERT(WaveReadLaneAt(conditional, 3) == 4.0);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveSum()
{
    uint lane = WaveGetLaneIndex();
    
    // All same value
    float allOnes = WaveActiveSum(1.0);
    ASSERT(allOnes == 4.0);
    
    // Lane indices: 0 + 1 + 2 + 3 = 6
    float laneSum = WaveActiveSum(lane);
    ASSERT(laneSum == 6.0);
    
    // Negative values: -0 + -1 + -2 + -3 = -6
    float negSum = WaveActiveSum(-lane);
    ASSERT(negSum == -6.0);
    
    // Mixed positive and negative
    float mixedSum = WaveActiveSum((lane < 2) ? lane : -lane);
    ASSERT(mixedSum == -4.0); // 0 + 1 + (-2) + (-3)
    
    // Large values
    float largeSum = WaveActiveSum(lane * 100.0);
    ASSERT(largeSum == 600.0);
    
    // Zero
    ASSERT(WaveActiveSum(0.0) == 0.0);
    
    // Test in varying control flow
    float conditional = 0.0;
    if (lane >= 2)
        conditional = WaveActiveSum(10.0);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == 0.0);
    ASSERT(WaveReadLaneAt(conditional, 1) == 0.0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 20.0); // Sum from lanes 2,3
    ASSERT(WaveReadLaneAt(conditional, 3) == 20.0);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveAllTrue()
{
    uint lane = WaveGetLaneIndex();
    
    // All lanes true
    bool allTrue = WaveActiveAllTrue(true);
    ASSERT(allTrue);
    
    // All lanes false
    bool allFalse = WaveActiveAllTrue(false);
    ASSERT(!allFalse);
    
    // Some lanes true
    bool someTrue = WaveActiveAllTrue(lane < 2);
    ASSERT(!someTrue);
    
    // Condition that varies
    bool varyingCondition = WaveActiveAllTrue(lane >= 0); // Always true
    ASSERT(varyingCondition);
    
    // Single false breaks it
    bool oneFalse = WaveActiveAllTrue(lane != 2);
    ASSERT(!oneFalse);
    
    // Test in varying control flow - all active lanes true
    bool conditional = false;
    if (lane >= 2)
        conditional = WaveActiveAllTrue(true);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == false);
    ASSERT(WaveReadLaneAt(conditional, 1) == false);
    ASSERT(WaveReadLaneAt(conditional, 2) == true); // Only lanes 2,3 active, both true
    ASSERT(WaveReadLaneAt(conditional, 3) == true);
    
    // Test in varying control flow - not all active lanes true
    bool conditional2 = true;
    if (lane >= 2)
        conditional2 = WaveActiveAllTrue(lane == 2);
    
    ASSERT(WaveReadLaneAt(conditional2, 2) == false); // Lane 3 is false
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveAnyTrue()
{
    uint lane = WaveGetLaneIndex();
    
    // All lanes true
    bool allTrue = WaveActiveAnyTrue(true);
    ASSERT(allTrue);
    
    // All lanes false
    bool allFalse = WaveActiveAnyTrue(false);
    ASSERT(!allFalse);
    
    // Only first lane true
    bool firstTrue = WaveActiveAnyTrue(lane == 0);
    ASSERT(firstTrue);
    
    // Only last lane true
    bool lastTrue = WaveActiveAnyTrue(lane == 3);
    ASSERT(lastTrue);
    
    // Some lanes true
    bool someTrue = WaveActiveAnyTrue(lane < 2);
    ASSERT(someTrue);
    
    // Single true is enough
    bool oneTrue = WaveActiveAnyTrue(lane == 2);
    ASSERT(oneTrue);
    
    // Test in varying control flow - any active lane true
    bool conditional = false;
    if (lane >= 2)
        conditional = WaveActiveAnyTrue(lane == 2);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == false);
    ASSERT(WaveReadLaneAt(conditional, 1) == false);
    ASSERT(WaveReadLaneAt(conditional, 2) == true); // Lane 2 is true
    ASSERT(WaveReadLaneAt(conditional, 3) == true);
    
    // Test in varying control flow - no active lane true
    bool conditional2 = true;
    if (lane >= 2)
        conditional2 = WaveActiveAnyTrue(false);
    
    ASSERT(WaveReadLaneAt(conditional2, 2) == false);
    ASSERT(WaveReadLaneAt(conditional2, 3) == false);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveActiveBallot()
{
    uint lane = WaveGetLaneIndex();
    
    // All true - all bits set
    uint4 allTrue = WaveActiveBallot(true);
    ASSERT((allTrue.x & 0xF) == 0xF); // Bits 0-3 set for 4 lanes
    
    // All false - no bits set
    uint4 allFalse = WaveActiveBallot(false);
    ASSERT((allFalse.x & 0xF) == 0x0);
    
    // Only first lane
    uint4 firstOnly = WaveActiveBallot(lane == 0);
    ASSERT((firstOnly.x & 0xF) == 0x1); // Bit 0 set
    
    // Only last lane
    uint4 lastOnly = WaveActiveBallot(lane == 3);
    ASSERT((lastOnly.x & 0xF) == 0x8); // Bit 3 set
    
    // First two lanes
    uint4 firstTwo = WaveActiveBallot(lane < 2);
    ASSERT((firstTwo.x & 0xF) == 0x3); // Bits 0-1 set
    
    // Odd lanes
    uint4 oddLanes = WaveActiveBallot((lane & 1) == 1);
    ASSERT((oddLanes.x & 0xF) == 0xA); // Bits 1,3 set (1010)
    
    // Even lanes
    uint4 evenLanes = WaveActiveBallot((lane & 1) == 0);
    ASSERT((evenLanes.x & 0xF) == 0x5); // Bits 0,2 set (0101)
    
    // Test in varying control flow
    uint4 conditional = uint4(0, 0, 0, 0);
    if (lane >= 2)
        conditional = WaveActiveBallot(true);
    
    ASSERT(WaveReadLaneAt(conditional.x, 0) == 0);
    ASSERT(WaveReadLaneAt(conditional.x, 1) == 0);
    ASSERT((WaveReadLaneAt(conditional.x, 2) & 0xF) == 0xC); // Bits 2,3 set
    ASSERT((WaveReadLaneAt(conditional.x, 3) & 0xF) == 0xC);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveGetLaneCount()
{
    // Should return 4 for all lanes
    uint count = WaveGetLaneCount();
    ASSERT(count == 4);
    
    // Test that it's uniform across all lanes
    uint lane = WaveGetLaneIndex();
    ASSERT(WaveReadLaneAt(count, 0) == 4);
    ASSERT(WaveReadLaneAt(count, 1) == 4);
    ASSERT(WaveReadLaneAt(count, 2) == 4);
    ASSERT(WaveReadLaneAt(count, 3) == 4);
    
    // Test in varying control flow (should still be same)
    uint conditional = 0;
    if (lane >= 2)
        conditional = WaveGetLaneCount();
    
    ASSERT(WaveReadLaneAt(conditional, 2) == 4);
    ASSERT(WaveReadLaneAt(conditional, 3) == 4);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveGetLaneIndex()
{
    uint lane = WaveGetLaneIndex();
    
    // Each lane should have unique index
    ASSERT(WaveReadLaneAt(lane, 0) == 0);
    ASSERT(WaveReadLaneAt(lane, 1) == 1);
    ASSERT(WaveReadLaneAt(lane, 2) == 2);
    ASSERT(WaveReadLaneAt(lane, 3) == 3);
    
    // Lane index should be less than lane count
    ASSERT(lane < WaveGetLaneCount());
    
    // Test in varying control flow
    uint conditional = 99;
    if (lane >= 2)
        conditional = WaveGetLaneIndex();
    
    ASSERT(WaveReadLaneAt(conditional, 0) == 99);
    ASSERT(WaveReadLaneAt(conditional, 1) == 99);
    ASSERT(WaveReadLaneAt(conditional, 2) == 2);
    ASSERT(WaveReadLaneAt(conditional, 3) == 3);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveIsFirstLane()
{
    uint lane = WaveGetLaneIndex();
    
    // Only lane 0 should be first
    bool isFirst = WaveIsFirstLane();
    ASSERT(WaveReadLaneAt(isFirst, 0) == true);
    ASSERT(WaveReadLaneAt(isFirst, 1) == false);
    ASSERT(WaveReadLaneAt(isFirst, 2) == false);
    ASSERT(WaveReadLaneAt(isFirst, 3) == false);
    
    // Count how many lanes think they're first (should be 1)
    uint firstCount = WaveActiveCountBits(isFirst);
    ASSERT(firstCount == 1);
    
    // Test in varying control flow - first active lane should be "first"
    bool conditional = false;
    if (lane >= 2)
        conditional = WaveIsFirstLane();
    
    ASSERT(WaveReadLaneAt(conditional, 0) == false);
    ASSERT(WaveReadLaneAt(conditional, 1) == false);
    ASSERT(WaveReadLaneAt(conditional, 2) == true);  // First of active lanes
    ASSERT(WaveReadLaneAt(conditional, 3) == false);
    
    // Test with different control flow pattern
    bool conditional2 = false;
    if (lane == 1 || lane == 3)
        conditional2 = WaveIsFirstLane();
    
    ASSERT(WaveReadLaneAt(conditional2, 1) == true);  // First of lanes 1,3
    ASSERT(WaveReadLaneAt(conditional2, 3) == false);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WavePrefixCountBits()
{
    uint lane = WaveGetLaneIndex();
    
    // All true - should count lanes before current
    uint allTrue = WavePrefixCountBits(true);
    ASSERT(WaveReadLaneAt(allTrue, 0) == 0); // No lanes before 0
    ASSERT(WaveReadLaneAt(allTrue, 1) == 1); // Lane 0 before 1
    ASSERT(WaveReadLaneAt(allTrue, 2) == 2); // Lanes 0,1 before 2
    ASSERT(WaveReadLaneAt(allTrue, 3) == 3); // Lanes 0,1,2 before 3
    
    // All false
    uint allFalse = WavePrefixCountBits(false);
    ASSERT(WaveReadLaneAt(allFalse, 0) == 0);
    ASSERT(WaveReadLaneAt(allFalse, 1) == 0);
    ASSERT(WaveReadLaneAt(allFalse, 2) == 0);
    ASSERT(WaveReadLaneAt(allFalse, 3) == 0);
    
    // Even lanes only
    uint evenOnly = WavePrefixCountBits((lane & 1) == 0);
    ASSERT(WaveReadLaneAt(evenOnly, 0) == 0); // No even lanes before 0
    ASSERT(WaveReadLaneAt(evenOnly, 1) == 1); // Lane 0 before 1
    ASSERT(WaveReadLaneAt(evenOnly, 2) == 1); // Lane 0 before 2
    ASSERT(WaveReadLaneAt(evenOnly, 3) == 2); // Lanes 0,2 before 3
    
    // Odd lanes only
    uint oddOnly = WavePrefixCountBits((lane & 1) == 1);
    ASSERT(WaveReadLaneAt(oddOnly, 0) == 0);
    ASSERT(WaveReadLaneAt(oddOnly, 1) == 0); // No odd lanes before 1
    ASSERT(WaveReadLaneAt(oddOnly, 2) == 1); // Lane 1 before 2
    ASSERT(WaveReadLaneAt(oddOnly, 3) == 1); // Lane 1 before 3
    
    // Test in varying control flow
    uint conditional = 0;
    if (lane >= 2)
        conditional = WavePrefixCountBits(true);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == 0);
    ASSERT(WaveReadLaneAt(conditional, 1) == 0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 0); // No active lanes before 2
    ASSERT(WaveReadLaneAt(conditional, 3) == 1); // Lane 2 before 3
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WavePrefixProduct()
{
    uint lane = WaveGetLaneIndex();
    
    // All ones - product stays 1
    float allOnes = WavePrefixProduct(1.0);
    ASSERT(WaveReadLaneAt(allOnes, 0) == 1.0);
    ASSERT(WaveReadLaneAt(allOnes, 1) == 1.0);
    ASSERT(WaveReadLaneAt(allOnes, 2) == 1.0);
    ASSERT(WaveReadLaneAt(allOnes, 3) == 1.0);
    
    // All twos - exponential growth
    float allTwos = WavePrefixProduct(2.0);
    ASSERT(WaveReadLaneAt(allTwos, 0) == 1.0);   // Identity
    ASSERT(WaveReadLaneAt(allTwos, 1) == 2.0);   // 2
    ASSERT(WaveReadLaneAt(allTwos, 2) == 4.0);   // 2 * 2
    ASSERT(WaveReadLaneAt(allTwos, 3) == 8.0);   // 2 * 2 * 2
    
    // Lane index + 1: progressive factorial-like
    float laneProduct = WavePrefixProduct(lane + 1.0);
    ASSERT(WaveReadLaneAt(laneProduct, 0) == 1.0);  // Identity
    ASSERT(WaveReadLaneAt(laneProduct, 1) == 1.0);  // 1
    ASSERT(WaveReadLaneAt(laneProduct, 2) == 2.0);  // 1 * 2
    ASSERT(WaveReadLaneAt(laneProduct, 3) == 6.0);  // 1 * 2 * 3
    
    // With zero in the middle
    float withZero = WavePrefixProduct((lane == 2) ? 0.0 : 2.0);
    ASSERT(WaveReadLaneAt(withZero, 0) == 1.0);
    ASSERT(WaveReadLaneAt(withZero, 1) == 2.0);
    ASSERT(WaveReadLaneAt(withZero, 2) == 4.0);
    ASSERT(WaveReadLaneAt(withZero, 3) == 0.0);  // Includes the zero
    
    // Negative values
    float negValues = WavePrefixProduct(-2.0);
    ASSERT(WaveReadLaneAt(negValues, 0) == 1.0);
    ASSERT(WaveReadLaneAt(negValues, 1) == -2.0);
    ASSERT(WaveReadLaneAt(negValues, 2) == 4.0);   // Two negatives
    ASSERT(WaveReadLaneAt(negValues, 3) == -8.0);  // Three negatives
    
    // Test in varying control flow
    float conditional = 0.0;
    if (lane >= 2)
        conditional = WavePrefixProduct(3.0);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == 0.0);
    ASSERT(WaveReadLaneAt(conditional, 1) == 0.0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 1.0); // Identity for first active
    ASSERT(WaveReadLaneAt(conditional, 3) == 3.0); // Product of lane 2's value
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WavePrefixSum()
{
    uint lane = WaveGetLaneIndex();
    
    // All ones - cumulative sum
    float allOnes = WavePrefixSum(1.0);
    ASSERT(WaveReadLaneAt(allOnes, 0) == 0.0); // No lanes before 0
    ASSERT(WaveReadLaneAt(allOnes, 1) == 1.0); // Sum of lane 0
    ASSERT(WaveReadLaneAt(allOnes, 2) == 2.0); // Sum of lanes 0,1
    ASSERT(WaveReadLaneAt(allOnes, 3) == 3.0); // Sum of lanes 0,1,2
    
    // Lane indices: 0, 1, 2, 3
    float laneSum = WavePrefixSum(lane);
    ASSERT(WaveReadLaneAt(laneSum, 0) == 0.0); // No sum before 0
    ASSERT(WaveReadLaneAt(laneSum, 1) == 0.0); // 0
    ASSERT(WaveReadLaneAt(laneSum, 2) == 1.0); // 0 + 1
    ASSERT(WaveReadLaneAt(laneSum, 3) == 3.0); // 0 + 1 + 2
    
    // All tens
    float allTens = WavePrefixSum(10.0);
    ASSERT(WaveReadLaneAt(allTens, 0) == 0.0);
    ASSERT(WaveReadLaneAt(allTens, 1) == 10.0);
    ASSERT(WaveReadLaneAt(allTens, 2) == 20.0);
    ASSERT(WaveReadLaneAt(allTens, 3) == 30.0);
    
    // Negative values
    float negSum = WavePrefixSum(-lane);
    ASSERT(WaveReadLaneAt(negSum, 0) == 0.0);
    ASSERT(WaveReadLaneAt(negSum, 1) == 0.0);   // -0
    ASSERT(WaveReadLaneAt(negSum, 2) == -1.0);  // -0 + -1
    ASSERT(WaveReadLaneAt(negSum, 3) == -3.0);  // -0 + -1 + -2
    
    // Mixed positive and negative
    float mixed = WavePrefixSum((lane < 2) ? 1.0 : -1.0);
    ASSERT(WaveReadLaneAt(mixed, 0) == 0.0);
    ASSERT(WaveReadLaneAt(mixed, 1) == 1.0);
    ASSERT(WaveReadLaneAt(mixed, 2) == 2.0);
    ASSERT(WaveReadLaneAt(mixed, 3) == 1.0);  // 1 + 1 + (-1)
    
    // Test in varying control flow
    float conditional = -999.0;
    if (lane >= 2)
        conditional = WavePrefixSum(5.0);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == -999.0);
    ASSERT(WaveReadLaneAt(conditional, 1) == -999.0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 0.0);  // No active lanes before 2
    ASSERT(WaveReadLaneAt(conditional, 3) == 5.0);  // Lane 2's value
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveReadLaneFirst()
{
    uint lane = WaveGetLaneIndex();
    
    // Each lane has different value, all read from lane 0
    float laneValue = lane * 10.0;
    float firstValue = WaveReadLaneFirst(laneValue);
    ASSERT(firstValue == 0.0);
    
    // All lanes have same value
    float sameValue = WaveReadLaneFirst(42.0);
    ASSERT(sameValue == 42.0);
    
    // Negative values
    float negValue = WaveReadLaneFirst(-lane);
    ASSERT(negValue == 0.0);
    
    // Test with vector
    float3 vecValue = float3(lane, lane * 2.0, lane * 3.0);
    float3 firstVec = WaveReadLaneFirst(vecValue);
    ASSERT(firstVec.x == 0.0 && firstVec.y == 0.0 && firstVec.z == 0.0);
    
    // Test in varying control flow - reads from first active lane
    float conditional = 0.0;
    if (lane >= 2)
        conditional = WaveReadLaneFirst(lane * 100.0);
    
    ASSERT(WaveReadLaneAt(conditional, 0) == 0.0);
    ASSERT(WaveReadLaneAt(conditional, 1) == 0.0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 200.0); // First active is lane 2
    ASSERT(WaveReadLaneAt(conditional, 3) == 200.0);
    
    // Test with different control flow pattern
    float conditional2 = 0.0;
    if (lane == 1 || lane == 3)
        conditional2 = WaveReadLaneFirst(lane * 50.0);
    
    ASSERT(WaveReadLaneAt(conditional2, 1) == 50.0);  // First active is lane 1
    ASSERT(WaveReadLaneAt(conditional2, 3) == 50.0);
}

[Test]
[WarpSize(4, 1)]
void Intrinsic_WaveReadLaneAt()
{
    uint lane = WaveGetLaneIndex();
    
    // Read from each specific lane
    float value = lane * 10.0;
    
    ASSERT(WaveReadLaneAt(value, 0) == 0.0);
    ASSERT(WaveReadLaneAt(value, 1) == 10.0);
    ASSERT(WaveReadLaneAt(value, 2) == 20.0);
    ASSERT(WaveReadLaneAt(value, 3) == 30.0);
    
    // Read from same lane
    float sameLane = WaveReadLaneAt(value, lane);
    ASSERT(sameLane == value);
    
    // All lanes read from lane 0
    float fromZero = WaveReadLaneAt(value, 0);
    ASSERT(fromZero == 0.0);
    
    // All lanes read from lane 3
    float fromThree = WaveReadLaneAt(value, 3);
    ASSERT(fromThree == 30.0);
    
    // Test with negative values
    float negValue = -lane - 1.0;
    ASSERT(WaveReadLaneAt(negValue, 0) == -1.0);
    ASSERT(WaveReadLaneAt(negValue, 1) == -2.0);
    ASSERT(WaveReadLaneAt(negValue, 2) == -3.0);
    ASSERT(WaveReadLaneAt(negValue, 3) == -4.0);
    
    // Test with vector
    float2 vecValue = float2(lane, lane * 5.0);
    float2 vecRead = WaveReadLaneAt(vecValue, 2);
    ASSERT(vecRead.x == 2.0 && vecRead.y == 10.0);
    
    // Test in varying control flow
    float conditional = -1.0;
    if (lane >= 2)
        conditional = lane * 7.0;
    
    // Reading from inactive lane should get their pre-branch value
    ASSERT(WaveReadLaneAt(conditional, 0) == -1.0);
    ASSERT(WaveReadLaneAt(conditional, 1) == -1.0);
    ASSERT(WaveReadLaneAt(conditional, 2) == 14.0);
    ASSERT(WaveReadLaneAt(conditional, 3) == 21.0);
    
    // Cross-read between active and inactive
    float crossRead1 = WaveReadLaneAt(conditional, (lane + 1) % 4);
    ASSERT(WaveReadLaneAt(crossRead1, 0) == -1.0); // Lane 0 reads lane 1
    ASSERT(WaveReadLaneAt(crossRead1, 1) == 14.0); // Lane 1 reads lane 2
    ASSERT(WaveReadLaneAt(crossRead1, 2) == 21.0); // Lane 2 reads lane 3
    ASSERT(WaveReadLaneAt(crossRead1, 3) == -1.0); // Lane 3 reads lane 0
}