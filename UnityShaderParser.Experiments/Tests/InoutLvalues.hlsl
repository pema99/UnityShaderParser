struct Bar
{
    int x;
    int y;
};

struct VecHolder
{
    float4 v;
};

void Increment(inout int val)
{
    val += 1;
}

void Increment1Thread(inout int val)
{
    if (WaveGetLaneIndex() == 0)
        val += 1;
}

void Swap(inout float2 v)
{
    v.xy = v.yx;
}

void AddWeird(inout float4 v)
{
    v.zyx.yx += float2(1, 2);
}

void Double(inout float2 v)
{
    v *= 2;
}

void Double1Thread(inout float2 v)
{
    if (WaveGetLaneIndex() == 0)
        v *= 2;
}

void SetXY(out int a, out int b)
{
    a = 10;
    b = 20;
}

void SetTo99(inout int val)
{
    val = 99;
}

void Chain(inout int val)
{
    SetTo99(val);
}

void ConditionalSet(inout int val, bool cond)
{
    if (cond)
    {
        val = 42;
    }
}

void AssignLaneIndex(inout int val)
{
    val = WaveGetLaneIndex();
}

void IncrementFloat(inout float val)
{
    val += 1;
}

[Test]
void InoutStructField_WritesBackToOriginalStruct()
{
    Bar b;
    b.x = 5;
    Increment(b.x);
    ASSERT(b.x == 6);
}

[Test]
void InoutArrayElement_WritesBackToOriginalArray()
{
    int arr[3];
    arr[0] = 1;
    arr[1] = 2;
    arr[2] = 3;
    Increment(arr[1]);
    ASSERT(arr[0] == 1);
    ASSERT(arr[1] == 3);
    ASSERT(arr[2] == 3);
}

[Test]
void InoutVectorSwizzle_WritesBackToOriginalVector()
{
    float4 v = float4(1, 2, 3, 4);
    Double(v.xy);
    ASSERT(v.x == 2);
    ASSERT(v.y == 4);
    ASSERT(v.z == 3);
    ASSERT(v.w == 4);
}

[Test]
void InoutVectorSwizzle_WritesBackToOriginalVector()
{
    float4 v = float4(1, 2, 3, 4);
    Double(v.xy);
    ASSERT(v.x == 2);
    ASSERT(v.y == 4);
    ASSERT(v.z == 3);
    ASSERT(v.w == 4);
}

[Test]
void InoutVectorSwizzleInsideFunction_WritesBackToOriginalVector()
{
    float4 v = float4(1, 2);
    Swap(v);
    ASSERT(v.x == 2);
    ASSERT(v.y == 1);
}

[Test]
void InoutNestedVectorSwizzleInsideFunction_WritesBackToOriginalVector()
{
    float4 v = float4(1, 2, 3, 4);
    AddWeird(v);
    ASSERT(v.x == 1);
    ASSERT(v.y == 3);
    ASSERT(v.z == 5);
    ASSERT(v.w == 4);
}

[Test]
void InoutNestedVectorSwizzle_WritesBackToOriginalVector()
{
    float4 v = float4(1, 2, 3, 4);
    Double(v.zw.yx);
    ASSERT(v.x == 1);
    ASSERT(v.y == 2);
    ASSERT(v.z == 6);
    ASSERT(v.w == 8);
}

[Test]
void InoutNestedAccess_StructArrayField_WritesBack()
{
    Bar arr[2];
    arr[0].x = 7;
    arr[0].y = 0;
    Increment(arr[0].x);
    ASSERT(arr[0].x == 8);
}

[Test]
void OutMultipleNonSimpleLvalues_WritesBack()
{
    Bar b;
    SetXY(b.x, b.y);
    ASSERT(b.x == 10);
    ASSERT(b.y == 20);
}

// Case 1: Chaining inout — foo(inout a) calls bar(inout b) passing a through
[Test]
void InoutChained_WritebackPropagatesThroughChain()
{
    int x = 0;
    Chain(x);
    ASSERT(x == 99);
}

// Case 2: Varying control flow — only some threads take the branch
[Test]
[WarpSize(2, 1)]
void InoutVaryingControlFlow_OnlyActiveThreadsModified()
{
    int val = WaveGetLaneIndex(); // Thread 0: val=0, Thread 1: val=1
    ConditionalSet(val, WaveGetLaneIndex() == 0); // Only thread 0's branch fires
    ASSERT(WaveReadLaneAt(val, 0) == 42);
    ASSERT(WaveReadLaneAt(val, 1) == 1);
}

// Case 3: Varying RHS — the value assigned inside the function differs per thread
[Test]
[WarpSize(2, 1)]
void InoutVaryingRHS_EachThreadWritesOwnValue()
{
    int val = 0;
    AssignLaneIndex(val);
    ASSERT(WaveReadLaneAt(val, 0) == 0);
    ASSERT(WaveReadLaneAt(val, 1) == 1);
}

// Single vector component via swizzle (v.x) and index (v[1]) passed as inout
[Test]
void InoutVectorSwizzleSingle_WritesBack()
{
    float4 v = float4(1, 2, 3, 4);
    Increment(v.x);
    ASSERT(v.x == 2);
    ASSERT(v.y == 2); // unchanged
}

[Test]
void InoutVectorIndex_WritesBack()
{
    float4 v = float4(1, 2, 3, 4);
    Increment(v[1]);
    ASSERT(v.x == 1); // unchanged
    ASSERT(v.y == 3);
}

[Test]
[WarpSize(2, 1)]
void InoutVectorVaryingIndex_ScatteredWriteBack()
{
    float4 v = float4(1, 2, 3, 4);
    // Thread 0 increments v[0], thread 1 increments v[1]
    Increment(v[WaveGetLaneIndex()]);
    // Each thread only saw its own write-back
    ASSERT(WaveReadLaneAt(v.x, 0) == 2); // thread 0 incremented v[0]: 1->2
    ASSERT(WaveReadLaneAt(v.y, 0) == 2); // thread 0 left v[1] unchanged
    ASSERT(WaveReadLaneAt(v.x, 1) == 1); // thread 1 left v[0] unchanged
    ASSERT(WaveReadLaneAt(v.y, 1) == 3); // thread 1 incremented v[1]: 2->3
}

// Swizzle nested inside a struct field passed as inout — MyStruct.MyVector.xz
[Test]
void InoutSwizzleNestedInStruct_WritesBack()
{
    VecHolder s;
    s.v = float4(1, 2, 3, 4);
    Double(s.v.xz);
    ASSERT(s.v.x == 2);
    ASSERT(s.v.y == 2); // unchanged
    ASSERT(s.v.z == 6);
    ASSERT(s.v.w == 4); // unchanged
}

// Matrix row passed as inout — write-back updates the original row in the matrix
[Test]
void InoutMatrixRow_WritesBackToOriginalMatrix()
{
    float4x4 m = float4x4(
        1, 2, 3, 4,
        5, 6, 7, 8,
        9, 10, 11, 12,
        13, 14, 15, 16
    );
    Double(m[0]);
    ASSERT(m[0][0] == 2);
    ASSERT(m[0][1] == 4);
    ASSERT(m[0][2] == 6);
    ASSERT(m[0][3] == 8);
    ASSERT(m[1][0] == 5); // other rows unchanged
}

// Case 5: Varying LHS — mat[threadIdx] as inout, each thread writes back to its own row
[Test]
[WarpSize(2, 1)]
void InoutMatrixVaryingRow_EachThreadWritesOwnRow()
{
    float4x4 m = float4x4(
        1, 2, 3, 4,
        5, 6, 7, 8,
        9, 10, 11, 12,
        13, 14, 15, 16
    );
    // Thread 0 doubles row 0, Thread 1 doubles row 1
    Double(m[WaveGetLaneIndex()]);
    ASSERT(WaveReadLaneAt(m[0][0], 0) == 2);
    ASSERT(WaveReadLaneAt(m[0][1], 0) == 4);
    ASSERT(WaveReadLaneAt(m[1][0], 0) == 5); // thread 0 left row 1 unchanged
    ASSERT(WaveReadLaneAt(m[1][0], 1) == 10);
    ASSERT(WaveReadLaneAt(m[1][1], 1) == 12);
    ASSERT(WaveReadLaneAt(m[0][0], 1) == 1); // thread 1 left row 0 unchanged
}

// Case 4: Varying LHS — arr[threadIdx] as inout, each thread writes back to its own slot
[Test]
[WarpSize(2, 1)]
void InoutScatteredWrite_VaryingIndex_EachThreadWritesOwnSlot()
{
    int arr[2];
    arr[0] = 10;
    arr[1] = 20;
    Increment(arr[WaveGetLaneIndex()]); // Thread 0: Increment(arr[0]), Thread 1: Increment(arr[1])
    ASSERT(arr[0] == 11);
    ASSERT(arr[1] == 21);
    float4 a = float4(1,2,3,4);
}

// =====================================================================
// Scenario 1: inout lvalue inside a divergent if-statement at the call site
// =====================================================================

[Test]
[WarpSize(2, 1)]
void InoutNamedExpression_VaryingControlFlow_WritesBackOnlyOnActiveLanes()
{
    int a = 10;
    Increment1Thread(a);
    ASSERT(WaveReadLaneAt(a, 0) == 11); // thread 0 incremented
    ASSERT(WaveReadLaneAt(a, 1) == 10); // thread 1 did not
}

[Test]
[WarpSize(2, 1)]
void InoutArrayElement_VaryingControlFlow_WritesBackOnlyOnActiveLanes()
{
    int arr[2];
    arr[0] = 10;
    arr[1] = 10;
    Increment1Thread(arr[0]);
    ASSERT(WaveReadLaneAt(arr[0], 0) == 11); // thread 0 incremented
    ASSERT(WaveReadLaneAt(arr[0], 1) == 10); // thread 1 did not
}

[Test]
[WarpSize(2, 1)]
void InoutVectorElement_VaryingControlFlow_WritesBackOnlyOnActiveLanes()
{
    int4 v = int4(10, 20, 30, 40);
    Increment1Thread(v[0]);
    ASSERT(WaveReadLaneAt(v[0], 0) == 11); // thread 0 incremented
    ASSERT(WaveReadLaneAt(v[0], 1) == 10); // thread 1 did not
}

[Test]
[WarpSize(2, 1)]
void InoutMatrixElement_VaryingControlFlow_WritesBackOnlyOnActiveLanes()
{
    int4x4 m = int4x4(
        1, 2, 3, 4,
        5, 6, 7, 8,
        9, 10, 11, 12,
        13, 14, 15, 16
    );
    Increment1Thread(m[0][0]);
    ASSERT(WaveReadLaneAt(m[0][0], 0) == 2); // thread 0 incremented
    ASSERT(WaveReadLaneAt(m[0][0], 1) == 1); // thread 1 did not
}

[Test]
[WarpSize(2, 1)]
void InoutStructMember_VaryingControlFlow_WritesBackOnlyOnActiveLanes()
{
    Bar b;
    b.x = 5;
    Increment1Thread(b.x);
    ASSERT(WaveReadLaneAt(b.x, 0) == 6); // thread 0 incremented
    ASSERT(WaveReadLaneAt(b.x, 1) == 5); // thread 1 did not
}

[Test]
[WarpSize(2, 1)]
void InoutVectorSwizzle_VaryingControlFlow_WritesBackOnlyOnActiveLanes()
{
    float2 v = float2(1, 2);
    Double1Thread(v.xy);
    ASSERT(WaveReadLaneAt(v.x, 0) == 2.0); // thread 0 doubled
    ASSERT(WaveReadLaneAt(v.y, 0) == 4.0);
    ASSERT(WaveReadLaneAt(v.x, 1) == 1.0); // thread 1 unchanged
    ASSERT(WaveReadLaneAt(v.y, 1) == 2.0);
}

// =====================================================================
// Scenario 2: inout parameter is itself varying (VGPR) before the call
// =====================================================================

[Test]
[WarpSize(2, 1)]
void InoutNamedExpression_VaryingParam_WritesBackCorrectly()
{
    int val = WaveGetLaneIndex(); // VGPR: thread 0 -> 0, thread 1 -> 1
    Increment(val);
    ASSERT(WaveReadLaneAt(val, 0) == 1);
    ASSERT(WaveReadLaneAt(val, 1) == 2);
}

[Test]
[WarpSize(2, 1)]
void InoutArrayElement_VaryingParam_WritesBackCorrectly()
{
    int arr[1];
    arr[0] = WaveGetLaneIndex(); // VGPR: thread 0 -> 0, thread 1 -> 1
    Increment(arr[0]);
    ASSERT(WaveReadLaneAt(arr[0], 0) == 1);
    ASSERT(WaveReadLaneAt(arr[0], 1) == 2);
}

[Test]
[WarpSize(2, 1)]
void InoutVectorElement_VaryingParam_WritesBackCorrectly()
{
    int4 v = 0;
    v[0] = WaveGetLaneIndex();
    Increment(v[0]);
    ASSERT(WaveReadLaneAt(v[0], 0) == 1);
    ASSERT(WaveReadLaneAt(v[0], 1) == 2);
}

[Test]
[WarpSize(2, 1)]
void InoutMatrixElement_VaryingParam_WritesBackCorrectly()
{
    float4x4 m = float4x4(
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0
    );
    m[0][1] = (float)WaveGetLaneIndex(); // VGPR: thread 0 -> 0.0, thread 1 -> 1.0
    IncrementFloat(m[0][1]);
    ASSERT(WaveReadLaneAt(m[0][1], 0) == 1.0);
    ASSERT(WaveReadLaneAt(m[0][1], 1) == 2.0);
}

[Test]
[WarpSize(2, 1)]
void InoutStructMember_VaryingParam_WritesBackCorrectly()
{
    Bar b;
    b.x = WaveGetLaneIndex(); // VGPR: thread 0 -> 0, thread 1 -> 1
    Increment(b.x);
    ASSERT(WaveReadLaneAt(b.x, 0) == 1);
    ASSERT(WaveReadLaneAt(b.x, 1) == 2);
}

[Test]
[WarpSize(2, 1)]
void InoutVectorSwizzle_VaryingParam_WritesBackCorrectly()
{
    float2 v;
    v.x = (float)WaveGetLaneIndex(); // VGPR: thread 0 -> 0.0, thread 1 -> 1.0
    v.y = 0;
    Double(v.xy);
    ASSERT(WaveReadLaneAt(v.x, 0) == 0.0); // 0 * 2 = 0
    ASSERT(WaveReadLaneAt(v.x, 1) == 2.0); // 1 * 2 = 2
}
