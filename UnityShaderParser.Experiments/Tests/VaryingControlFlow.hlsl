[Test]
[WarpSize(2, 2)]
void AssignToFloat_InVaryingControlFlow_OnlyAssignsActiveLane()
{
    uint lane = WaveGetLaneIndex();

    float a = 1;
    if (lane >= 2)
        a = 2;

    ASSERT(WaveReadLaneAt(a, 0) == 1);
    ASSERT(WaveReadLaneAt(a, 1) == 1);
    ASSERT(WaveReadLaneAt(a, 2) == 2);
    ASSERT(WaveReadLaneAt(a, 3) == 2);
}

[Test]
[WarpSize(2, 2)]
void AssignToFloatField_InVaryingControlFlow_OnlyAssignsActiveLane()
{
    struct SimpleStruct
    {
        float a;
    };

    SimpleStruct str;
    str.a = 1;

    uint lane = WaveGetLaneIndex();

    if (lane >= 2)
        str.a = 2;

    ASSERT(WaveReadLaneAt(str.a, 0) == 1);
    ASSERT(WaveReadLaneAt(str.a, 1) == 1);
    ASSERT(WaveReadLaneAt(str.a, 2) == 2);
    ASSERT(WaveReadLaneAt(str.a, 3) == 2);
}

[Test]
[WarpSize(2, 2)]
void AssignToFloatArray_InVaryingControlFlow_OnlyAssignsActiveLane()
{
    float arr[2];
    arr[0] = 0;
    arr[1] = 1;

    uint lane = WaveGetLaneIndex();

    if (lane >= 2)
        arr[1] = 2;

    ASSERT(WaveReadLaneAt(arr[1], 0) == 1);
    ASSERT(WaveReadLaneAt(arr[1], 1) == 1);
    ASSERT(WaveReadLaneAt(arr[1], 2) == 2);
    ASSERT(WaveReadLaneAt(arr[1], 3) == 2);
}