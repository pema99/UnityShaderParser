// ============================================================================
// IF STATEMENT TESTS
// ============================================================================

[Test]
void ControlFlow_SimpleIf_True()
{
    float result = 0.0;
    if (true)
        result = 10.0;
    
    ASSERT(result == 10.0);
}

[Test]
void ControlFlow_SimpleIf_False()
{
    float result = 0.0;
    if (false)
        result = 10.0;
    
    ASSERT(result == 0.0);
}

[Test]
void ControlFlow_IfElse_TrueBranch()
{
    float result = 0.0;
    if (5 > 3)
        result = 1.0;
    else
        result = 2.0;
    
    ASSERT(result == 1.0);
}

[Test]
void ControlFlow_IfElse_FalseBranch()
{
    float result = 0.0;
    if (5 < 3)
        result = 1.0;
    else
        result = 2.0;
    
    ASSERT(result == 2.0);
}

[Test]
void ControlFlow_IfElseIf_FirstBranch()
{
    int value = 1;
    float result = 0.0;
    
    if (value == 1)
        result = 10.0;
    else if (value == 2)
        result = 20.0;
    else if (value == 3)
        result = 30.0;
    else
        result = 40.0;
    
    ASSERT(result == 10.0);
}

[Test]
void ControlFlow_IfElseIf_MiddleBranch()
{
    int value = 2;
    float result = 0.0;
    
    if (value == 1)
        result = 10.0;
    else if (value == 2)
        result = 20.0;
    else if (value == 3)
        result = 30.0;
    else
        result = 40.0;
    
    ASSERT(result == 20.0);
}

[Test]
void ControlFlow_IfElseIf_LastElse()
{
    int value = 5;
    float result = 0.0;
    
    if (value == 1)
        result = 10.0;
    else if (value == 2)
        result = 20.0;
    else if (value == 3)
        result = 30.0;
    else
        result = 40.0;
    
    ASSERT(result == 40.0);
}

[Test]
void ControlFlow_NestedIf()
{
    float result = 0.0;
    
    if (true)
    {
        if (true)
        {
            result = 5.0;
        }
    }
    
    ASSERT(result == 5.0);
}

[Test]
void ControlFlow_NestedIf_MixedBranches()
{
    float result = 0.0;
    int x = 5;
    int y = 10;
    
    if (x > 0)
    {
        if (y > 5)
            result = 1.0;
        else
            result = 2.0;
    }
    else
    {
        if (y > 5)
            result = 3.0;
        else
            result = 4.0;
    }
    
    ASSERT(result == 1.0);
}

[Test]
void ControlFlow_IfWithCompoundStatement()
{
    float a = 0.0;
    float b = 0.0;
    
    if (true)
    {
        a = 1.0;
        b = 2.0;
    }
    
    ASSERT(a == 1.0);
    ASSERT(b == 2.0);
}

[Test]
void ControlFlow_IfModifiesMultipleVariables()
{
    float x = 1.0;
    float y = 2.0;
    float z = 3.0;
    
    if (x < y)
    {
        x = 10.0;
        y = 20.0;
        z = 30.0;
    }
    
    ASSERT(x == 10.0 && y == 20.0 && z == 30.0);
}

// ============================================================================
// IF STATEMENT TESTS - VARYING CONTROL FLOW
// ============================================================================

[Test]
[WarpSize(2, 2)]
void ControlFlow_IfVarying_SimpleAssignment()
{
    uint lane = WaveGetLaneIndex();
    
    float a = 1.0;
    if (lane >= 2)
        a = 2.0;
    
    ASSERT(WaveReadLaneAt(a, 0) == 1.0);
    ASSERT(WaveReadLaneAt(a, 1) == 1.0);
    ASSERT(WaveReadLaneAt(a, 2) == 2.0);
    ASSERT(WaveReadLaneAt(a, 3) == 2.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_IfVarying_MultipleConditions()
{
    uint lane = WaveGetLaneIndex();
    
    float result = 0.0;
    
    if (lane < 2)
        result = 10.0;
    else if (lane == 2)
        result = 20.0;
    else
        result = 30.0;
    
    ASSERT(WaveReadLaneAt(result, 0) == 10.0);
    ASSERT(WaveReadLaneAt(result, 1) == 10.0);
    ASSERT(WaveReadLaneAt(result, 2) == 20.0);
    ASSERT(WaveReadLaneAt(result, 3) == 30.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_IfVarying_EvenOddLanes()
{
    uint lane = WaveGetLaneIndex();
    
    float value = 0.0;
    
    if ((lane & 1) == 0)
        value = 100.0;
    else
        value = 200.0;
    
    ASSERT(WaveReadLaneAt(value, 0) == 100.0);
    ASSERT(WaveReadLaneAt(value, 1) == 200.0);
    ASSERT(WaveReadLaneAt(value, 2) == 100.0);
    ASSERT(WaveReadLaneAt(value, 3) == 200.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_IfVarying_NestedDivergence()
{
    uint lane = WaveGetLaneIndex();
    
    float result = 0.0;
    
    if (lane >= 2)
    {
        if (lane == 2)
            result = 20.0;
        else
            result = 30.0;
    }
    else
    {
        if (lane == 0)
            result = 0.0;
        else
            result = 10.0;
    }
    
    ASSERT(WaveReadLaneAt(result, 0) == 0.0);
    ASSERT(WaveReadLaneAt(result, 1) == 10.0);
    ASSERT(WaveReadLaneAt(result, 2) == 20.0);
    ASSERT(WaveReadLaneAt(result, 3) == 30.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_IfVarying_ModifyBeforeAndAfter()
{
    uint lane = WaveGetLaneIndex();
    
    float value = lane * 10.0;
    
    if (lane >= 2)
        value = value + 100.0;
    
    value = value + 1.0;
    
    ASSERT(WaveReadLaneAt(value, 0) == 1.0);   // 0 + 1
    ASSERT(WaveReadLaneAt(value, 1) == 11.0);  // 10 + 1
    ASSERT(WaveReadLaneAt(value, 2) == 121.0); // 20 + 100 + 1
    ASSERT(WaveReadLaneAt(value, 3) == 131.0); // 30 + 100 + 1
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_IfVarying_SingleLaneActive()
{
    uint lane = WaveGetLaneIndex();
    
    float value = 0.0;
    
    if (lane == 2)
        value = 42.0;
    
    ASSERT(WaveReadLaneAt(value, 0) == 0.0);
    ASSERT(WaveReadLaneAt(value, 1) == 0.0);
    ASSERT(WaveReadLaneAt(value, 2) == 42.0);
    ASSERT(WaveReadLaneAt(value, 3) == 0.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_IfVarying_MultipleVariables()
{
    uint lane = WaveGetLaneIndex();
    
    float a = 1.0;
    float b = 2.0;
    float c = 3.0;
    
    if (lane >= 2)
    {
        a = 10.0;
        b = 20.0;
        c = 30.0;
    }
    
    ASSERT(WaveReadLaneAt(a, 0) == 1.0 && WaveReadLaneAt(b, 0) == 2.0 && WaveReadLaneAt(c, 0) == 3.0);
    ASSERT(WaveReadLaneAt(a, 1) == 1.0 && WaveReadLaneAt(b, 1) == 2.0 && WaveReadLaneAt(c, 1) == 3.0);
    ASSERT(WaveReadLaneAt(a, 2) == 10.0 && WaveReadLaneAt(b, 2) == 20.0 && WaveReadLaneAt(c, 2) == 30.0);
    ASSERT(WaveReadLaneAt(a, 3) == 10.0 && WaveReadLaneAt(b, 3) == 20.0 && WaveReadLaneAt(c, 3) == 30.0);
}

// ============================================================================
// FOR LOOP TESTS
// ============================================================================

[Test]
void ControlFlow_ForLoop_SimpleIteration()
{
    float sum = 0.0;
    for (int i = 0; i < 5; i++)
    {
        sum = sum + i;
    }
    
    ASSERT(sum == 10.0); // 0 + 1 + 2 + 3 + 4
}

[Test]
void ControlFlow_ForLoop_EmptyLoop()
{
    float sum = 0.0;
    for (int i = 0; i < 0; i++)
    {
        sum = sum + 1.0;
    }
    
    ASSERT(sum == 0.0);
}

[Test]
void ControlFlow_ForLoop_SingleIteration()
{
    float sum = 0.0;
    for (int i = 0; i < 1; i++)
    {
        sum = sum + 10.0;
    }
    
    ASSERT(sum == 10.0);
}

[Test]
void ControlFlow_ForLoop_CountDown()
{
    float sum = 0.0;
    for (int i = 5; i > 0; i--)
    {
        sum = sum + i;
    }
    
    ASSERT(sum == 15.0); // 5 + 4 + 3 + 2 + 1
}

[Test]
void ControlFlow_ForLoop_StepBy2()
{
    float sum = 0.0;
    for (int i = 0; i < 10; i = i + 2)
    {
        sum = sum + i;
    }
    
    ASSERT(sum == 20.0); // 0 + 2 + 4 + 6 + 8
}

[Test]
void ControlFlow_ForLoop_NestedLoop()
{
    float sum = 0.0;
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 2; j++)
        {
            sum = sum + 1.0;
        }
    }
    
    ASSERT(sum == 6.0); // 3 * 2
}

[Test]
void ControlFlow_ForLoop_NestedWithVariables()
{
    float sum = 0.0;
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            sum = sum + (i * 10 + j);
        }
    }
    
    // i=0: 0+1+2 = 3
    // i=1: 10+11+12 = 33
    // i=2: 20+21+22 = 63
    // Total: 99
    ASSERT(sum == 99.0);
}

[Test]
void ControlFlow_ForLoop_WithBreak()
{
    float sum = 0.0;
    for (int i = 0; i < 10; i++)
    {
        if (i == 5)
            break;
        sum = sum + i;
    }
    
    ASSERT(sum == 10.0); // 0 + 1 + 2 + 3 + 4
}

[Test]
void ControlFlow_ForLoop_WithContinue()
{
    float sum = 0.0;
    for (int i = 0; i < 5; i++)
    {
        if (i == 2)
            continue;
        sum = sum + i;
    }
    
    ASSERT(sum == 8.0); // 0 + 1 + 3 + 4 (skips 2)
}

[Test]
void ControlFlow_ForLoop_MultipleBreaks()
{
    float sum = 0.0;
    for (int i = 0; i < 10; i++)
    {
        if (i == 3)
            break;
        if (i == 7)
            break;
        sum = sum + i;
    }
    
    ASSERT(sum == 3.0); // 0 + 1 + 2 (breaks at 3)
}

[Test]
void ControlFlow_ForLoop_MultipleContinues()
{
    float sum = 0.0;
    for (int i = 0; i < 6; i++)
    {
        if (i == 1)
            continue;
        if (i == 3)
            continue;
        sum = sum + i;
    }
    
    ASSERT(sum == 11.0);
}

[Test]
void ControlFlow_ForLoop_ArrayAccess()
{
    float arr[5];
    for (int i = 0; i < 5; i++)
    {
        arr[i] = i * 2.0;
    }
    
    ASSERT(arr[0] == 0.0);
    ASSERT(arr[1] == 2.0);
    ASSERT(arr[2] == 4.0);
    ASSERT(arr[3] == 6.0);
    ASSERT(arr[4] == 8.0);
}

// ============================================================================
// FOR LOOP TESTS - VARYING CONTROL FLOW
// ============================================================================

[Test]
[WarpSize(4, 1)]
void ControlFlow_ForLoopVarying_DifferentIterationCount()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    for (int i = 0; i < lane; i++)
    {
        sum = sum + 1.0;
    }
    
    ASSERT(WaveReadLaneAt(sum, 0) == 0.0);
    ASSERT(WaveReadLaneAt(sum, 1) == 1.0);
    ASSERT(WaveReadLaneAt(sum, 2) == 2.0);
    ASSERT(WaveReadLaneAt(sum, 3) == 3.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_ForLoopVarying_ConditionalBreak()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    for (int i = 0; i < 10; i++)
    {
        if (i >= lane)
            break;
        sum = sum + 1.0;
    }
    
    ASSERT(WaveReadLaneAt(sum, 0) == 0.0);
    ASSERT(WaveReadLaneAt(sum, 1) == 1.0);
    ASSERT(WaveReadLaneAt(sum, 2) == 2.0);
    ASSERT(WaveReadLaneAt(sum, 3) == 3.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_ForLoopVarying_ConditionalContinue()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    for (int i = 0; i < 4; i++)
    {
        if (i == lane)
            continue;
        sum = sum + i;
    }
    
    ASSERT(WaveReadLaneAt(sum, 0) == 6.0); // 1+2+3
    ASSERT(WaveReadLaneAt(sum, 1) == 5.0); // 0+2+3
    ASSERT(WaveReadLaneAt(sum, 2) == 4.0); // 0+1+3
    ASSERT(WaveReadLaneAt(sum, 3) == 3.0); // 0+1+2
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_ForLoopVarying_NestedWithDivergence()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    for (int i = 0; i < 2; i++)
    {
        if (lane >= 2)
        {
            sum = sum + 10.0;
        }
        else
        {
            sum = sum + 1.0;
        }
    }
    
    ASSERT(WaveReadLaneAt(sum, 0) == 2.0);  // 2 iterations of +1
    ASSERT(WaveReadLaneAt(sum, 1) == 2.0);  // 2 iterations of +1
    ASSERT(WaveReadLaneAt(sum, 2) == 20.0); // 2 iterations of +10
    ASSERT(WaveReadLaneAt(sum, 3) == 20.0); // 2 iterations of +10
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_ForLoopVarying_DivergentArrayWrite()
{
    uint lane = WaveGetLaneIndex();
    
    float arr[4];
    for (int i = 0; i < 4; i++)
        arr[i] = 0.0;
    
    for (int i = 0; i < 4; i++)
    {
        if (i < lane)
            arr[i] = 100.0;
    }
    
    // Lane 0: all 0
    ASSERT(WaveReadLaneAt(arr[0], 0) == 0.0);
    
    // Lane 1: arr[0] = 100
    ASSERT(WaveReadLaneAt(arr[0], 1) == 100.0);
    ASSERT(WaveReadLaneAt(arr[1], 1) == 0.0);
    
    // Lane 2: arr[0,1] = 100
    ASSERT(WaveReadLaneAt(arr[0], 2) == 100.0);
    ASSERT(WaveReadLaneAt(arr[1], 2) == 100.0);
    ASSERT(WaveReadLaneAt(arr[2], 2) == 0.0);
    
    // Lane 3: arr[0,1,2] = 100
    ASSERT(WaveReadLaneAt(arr[0], 3) == 100.0);
    ASSERT(WaveReadLaneAt(arr[1], 3) == 100.0);
    ASSERT(WaveReadLaneAt(arr[2], 3) == 100.0);
    ASSERT(WaveReadLaneAt(arr[3], 3) == 0.0);
}

// ============================================================================
// WHILE LOOP TESTS
// ============================================================================

[Test]
void ControlFlow_WhileLoop_SimpleIteration()
{
    float sum = 0.0;
    int i = 0;
    while (i < 5)
    {
        sum = sum + i;
        i = i + 1;
    }
    
    ASSERT(sum == 10.0); // 0 + 1 + 2 + 3 + 4
}

[Test]
void ControlFlow_WhileLoop_ZeroIterations()
{
    float sum = 0.0;
    int i = 0;
    while (i < 0)
    {
        sum = sum + 1.0;
        i = i + 1;
    }
    
    ASSERT(sum == 0.0);
}

[Test]
void ControlFlow_WhileLoop_WithBreak()
{
    float sum = 0.0;
    int i = 0;
    while (i < 10)
    {
        if (i == 5)
            break;
        sum = sum + i;
        i = i + 1;
    }
    
    ASSERT(sum == 10.0); // 0 + 1 + 2 + 3 + 4
}

[Test]
void ControlFlow_WhileLoop_WithContinue()
{
    float sum = 0.0;
    int i = 0;
    while (i < 5)
    {
        i = i + 1;
        if (i == 3)
            continue;
        sum = sum + i;
    }
    
    ASSERT(sum == 12.0); // 1 + 2 + 4 + 5 (skips 3)
}

[Test]
void ControlFlow_WhileLoop_NestedWhile()
{
    float sum = 0.0;
    int i = 0;
    while (i < 3)
    {
        int j = 0;
        while (j < 2)
        {
            sum = sum + 1.0;
            j = j + 1;
        }
        i = i + 1;
    }
    
    ASSERT(sum == 6.0); // 3 * 2
}

// ============================================================================
// WHILE LOOP TESTS - VARYING CONTROL FLOW
// ============================================================================

[Test]
[WarpSize(4, 1)]
void ControlFlow_WhileLoopVarying_DifferentIterationCount()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    int i = 0;
    while (i < lane)
    {
        sum = sum + 1.0;
        i = i + 1;
    }
    
    ASSERT(WaveReadLaneAt(sum, 0) == 0.0);
    ASSERT(WaveReadLaneAt(sum, 1) == 1.0);
    ASSERT(WaveReadLaneAt(sum, 2) == 2.0);
    ASSERT(WaveReadLaneAt(sum, 3) == 3.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_WhileLoopVarying_ConditionalBreak()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    int i = 0;
    while (i < 10)
    {
        if (i >= lane)
            break;
        sum = sum + 1.0;
        i = i + 1;
    }
    
    ASSERT(WaveReadLaneAt(sum, 0) == 0.0);
    ASSERT(WaveReadLaneAt(sum, 1) == 1.0);
    ASSERT(WaveReadLaneAt(sum, 2) == 2.0);
    ASSERT(WaveReadLaneAt(sum, 3) == 3.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_WhileLoopVarying_DivergentBody()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    int i = 0;
    while (i < 3)
    {
        if (lane >= 2)
            sum = sum + 10.0;
        else
            sum = sum + 1.0;
        i = i + 1;
    }
    
    ASSERT(WaveReadLaneAt(sum, 0) == 3.0);
    ASSERT(WaveReadLaneAt(sum, 1) == 3.0);
    ASSERT(WaveReadLaneAt(sum, 2) == 30.0);
    ASSERT(WaveReadLaneAt(sum, 3) == 30.0);
}

// ============================================================================
// DO-WHILE LOOP TESTS
// ============================================================================

[Test]
void ControlFlow_DoWhileLoop_SimpleIteration()
{
    float sum = 0.0;
    int i = 0;
    do
    {
        sum = sum + i;
        i = i + 1;
    } while (i < 5);
    
    ASSERT(sum == 10.0); // 0 + 1 + 2 + 3 + 4
}

[Test]
void ControlFlow_DoWhileLoop_MinimumOneIteration()
{
    float sum = 0.0;
    int i = 0;
    do
    {
        sum = sum + 1.0;
        i = i + 1;
    } while (i < 0);
    
    ASSERT(sum == 1.0); // Executes once even though condition is false
}

[Test]
void ControlFlow_DoWhileLoop_WithBreak()
{
    float sum = 0.0;
    int i = 0;
    do
    {
        if (i == 3)
            break;
        sum = sum + i;
        i = i + 1;
    } while (i < 10);
    
    ASSERT(sum == 3.0); // 0 + 1 + 2
}

[Test]
void ControlFlow_DoWhileLoop_WithContinue()
{
    float sum = 0.0;
    int i = 0;
    do
    {
        i = i + 1;
        if (i == 2)
            continue;
        sum = sum + i;
    } while (i < 4);
    
    ASSERT(sum == 8.0); // 1 + 3 + 4 (skips 2)
}

// ============================================================================
// DO-WHILE LOOP TESTS - VARYING CONTROL FLOW
// ============================================================================

[Test]
[WarpSize(4, 1)]
void ControlFlow_DoWhileLoopVarying_DifferentIterationCount()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    int i = 0;
    do
    {
        sum = sum + 1.0;
        i = i + 1;
    } while (i < lane);
    
    // All lanes execute at least once
    ASSERT(WaveReadLaneAt(sum, 0) == 1.0); // Executes once (do-while minimum)
    ASSERT(WaveReadLaneAt(sum, 1) == 1.0);
    ASSERT(WaveReadLaneAt(sum, 2) == 2.0);
    ASSERT(WaveReadLaneAt(sum, 3) == 3.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_DoWhileLoopVarying_DivergentBody()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    int i = 0;
    do
    {
        if (lane >= 2)
            sum = sum + 10.0;
        else
            sum = sum + 1.0;
        i = i + 1;
    } while (i < 2);
    
    ASSERT(WaveReadLaneAt(sum, 0) == 2.0);
    ASSERT(WaveReadLaneAt(sum, 1) == 2.0);
    ASSERT(WaveReadLaneAt(sum, 2) == 20.0);
    ASSERT(WaveReadLaneAt(sum, 3) == 20.0);
}

// ============================================================================
// MIXED CONTROL FLOW TESTS
// ============================================================================

[Test]
void ControlFlow_IfInsideForLoop()
{
    float sum = 0.0;
    for (int i = 0; i < 10; i++)
    {
        if (i % 2 == 0)
            sum = sum + i;
    }
    
    ASSERT(sum == 20.0); // 0 + 2 + 4 + 6 + 8
}

[Test]
void ControlFlow_ForLoopInsideIf()
{
    float sum = 0.0;
    bool condition = true;
    
    if (condition)
    {
        for (int i = 0; i < 5; i++)
        {
            sum = sum + i;
        }
    }
    
    ASSERT(sum == 10.0);
}

[Test]
void ControlFlow_ComplexNesting()
{
    float result = 0.0;
    
    for (int i = 0; i < 3; i++)
    {
        if (i > 0)
        {
            for (int j = 0; j < 2; j++)
            {
                if (j == 0)
                    result = result + 1.0;
                else
                    result = result + 2.0;
            }
        }
    }
    
    // i=0: nothing (i > 0 is false)
    // i=1: j=0 (+1), j=1 (+2) = +3
    // i=2: j=0 (+1), j=1 (+2) = +3
    // Total: 6
    ASSERT(result == 6.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_ComplexVarying_NestedLoopsAndIfs()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    
    for (int i = 0; i < 3; i++)
    {
        if (i < lane)
        {
            for (int j = 0; j < 2; j++)
            {
                if ((lane & 1) == 0)
                    sum = sum + 1.0;
                else
                    sum = sum + 2.0;
            }
        }
    }
    
    // Lane 0: never enters (i < 0 is always false)
    ASSERT(WaveReadLaneAt(sum, 0) == 0.0);
    
    // Lane 1 (odd): i=0, j=0,1: +2+2 = 4
    ASSERT(WaveReadLaneAt(sum, 1) == 4.0);
    
    // Lane 2 (even): i=0,1, j=0,1 each: 2*(1+1) = 4
    ASSERT(WaveReadLaneAt(sum, 2) == 4.0);
    
    // Lane 3 (odd): i=0,1,2, j=0,1 each: 3*(2+2) = 12
    ASSERT(WaveReadLaneAt(sum, 3) == 12.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_VaryingBreakInNestedLoop()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    
    for (int i = 0; i < 5; i++)
    {
        for (int j = 0; j < 5; j++)
        {
            if (i + j >= lane)
                break;
            sum = sum + 1.0;
        }
    }
    
    // Lane 0: i+j >= 0 immediately, never increments
    ASSERT(WaveReadLaneAt(sum, 0) == 0.0);
    
    // Lane 1: breaks when i+j >= 1
    // i=0,j=0: sum++, then breaks at j=1
    // i=1: breaks immediately at j=0
    // Total: 1
    ASSERT(WaveReadLaneAt(sum, 1) == 1.0);
    
    // Lane 2: breaks when i+j >= 2
    // i=0: j=0,1 (sum+=2)
    // i=1: j=0 (sum+=1)
    // i=2+: breaks immediately
    // Total: 3
    ASSERT(WaveReadLaneAt(sum, 2) == 3.0);
    
    // Lane 3: breaks when i+j >= 3
    // i=0: j=0,1,2 (sum+=3)
    // i=1: j=0,1 (sum+=2)
    // i=2: j=0 (sum+=1)
    // i=3+: breaks immediately
    // Total: 6
    ASSERT(WaveReadLaneAt(sum, 3) == 6.0);
}

[Test]
void ControlFlow_WhileInsideForLoop()
{
    float sum = 0.0;
    
    for (int i = 0; i < 3; i++)
    {
        int j = 0;
        while (j < 2)
        {
            sum = sum + 1.0;
            j = j + 1;
        }
    }
    
    ASSERT(sum == 6.0);
}

[Test]
void ControlFlow_ForInsideWhileLoop()
{
    float sum = 0.0;
    int i = 0;
    
    while (i < 3)
    {
        for (int j = 0; j < 2; j++)
        {
            sum = sum + 1.0;
        }
        i = i + 1;
    }
    
    ASSERT(sum == 6.0);
}

[Test]
void ControlFlow_DoWhileInsideForLoop()
{
    float sum = 0.0;
    
    for (int i = 0; i < 3; i++)
    {
        int j = 0;
        do
        {
            sum = sum + 1.0;
            j = j + 1;
        } while (j < 2);
    }
    
    ASSERT(sum == 6.0);
}

[Test]
void ControlFlow_MultipleBreakLevels()
{
    float sum = 0.0;
    bool shouldBreak = false;
    
    for (int i = 0; i < 5; i++)
    {
        if (shouldBreak)
            break;
        
        for (int j = 0; j < 5; j++)
        {
            sum = sum + 1.0;
            
            if (i == 2 && j == 2)
            {
                shouldBreak = true;
                break;
            }
        }
    }
    
    // i=0: 5 iterations
    // i=1: 5 iterations
    // i=2: 3 iterations (breaks at j=2, after incrementing)
    // Total: 13
    ASSERT(sum == 13.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_VaryingContinuePattern()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    
    for (int i = 0; i < 4; i++)
    {
        if (i == lane)
            continue;
        
        if ((i & 1) == (lane & 1))
            sum = sum + 10.0;
        else
            sum = sum + 1.0;
    }
    
    // Lane 0 (even): skips i=0, processes i=1,2,3
    // i=1 (odd != even): +1
    // i=2 (even == even): +10
    // i=3 (odd != even): +1
    // Total: 12
    ASSERT(WaveReadLaneAt(sum, 0) == 12.0);
    
    // Lane 1 (odd): skips i=1, processes i=0,2,3
    // i=0 (even != odd): +1
    // i=2 (even != odd): +1
    // i=3 (odd == odd): +10
    // Total: 12
    ASSERT(WaveReadLaneAt(sum, 1) == 12.0);
    
    // Lane 2 (even): skips i=2, processes i=0,1,3
    // i=0 (even == even): +10
    // i=1 (odd != even): +1
    // i=3 (odd != even): +1
    // Total: 12
    ASSERT(WaveReadLaneAt(sum, 2) == 12.0);
    
    // Lane 3 (odd): skips i=3, processes i=0,1,2
    // i=0 (even != odd): +1
    // i=1 (odd == odd): +10
    // i=2 (even != odd): +1
    // Total: 12
    ASSERT(WaveReadLaneAt(sum, 3) == 12.0);
}

// ============================================================================
// EARLY RETURN TESTS
// ============================================================================

float EarlyReturnFunction(float x)
{
    if (x < 0.0)
        return -1.0;
    
    if (x > 10.0)
        return 10.0;
    
    return x;
}

[Test]
void ControlFlow_EarlyReturn_FirstBranch()
{
    float result = EarlyReturnFunction(-5.0);
    ASSERT(result == -1.0);
}

[Test]
void ControlFlow_EarlyReturn_SecondBranch()
{
    float result = EarlyReturnFunction(15.0);
    ASSERT(result == 10.0);
}

[Test]
void ControlFlow_EarlyReturn_FallThrough()
{
    float result = EarlyReturnFunction(5.0);
    ASSERT(result == 5.0);
}

float ReturnInLoop(int limit)
{
    for (int i = 0; i < 10; i++)
    {
        if (i == limit)
            return i * 2.0;
    }
    return -1.0;
}

[Test]
void ControlFlow_ReturnInLoop_Early()
{
    float result = ReturnInLoop(3);
    ASSERT(result == 6.0);
}

[Test]
void ControlFlow_ReturnInLoop_NoMatch()
{
    float result = ReturnInLoop(15);
    ASSERT(result == -1.0);
}

// ============================================================================
// LOOP VARIABLE SCOPE TESTS
// ============================================================================

[Test]
void ControlFlow_LoopVariableScope_ForLoop()
{
    float sum = 0.0;
    
    for (int i = 0; i < 3; i++)
    {
        sum = sum + i;
    }
    
    // Can use same variable name in new scope
    for (int i = 0; i < 2; i++)
    {
        sum = sum + i * 10.0;
    }
    
    ASSERT(sum == 13.0); // (0+1+2) + (0+10)
}

[Test]
void ControlFlow_NestedLoopVariableScope()
{
    float sum = 0.0;
    
    for (int i = 0; i < 2; i++)
    {
        for (int i = 0; i < 3; i++) // Shadows outer i
        {
            sum = sum + i;
        }
    }
    
    ASSERT(sum == 6.0); // 2 * (0+1+2)
}

// ============================================================================
// CONDITIONAL OPERATOR (TERNARY) TESTS
// ============================================================================

[Test]
void ControlFlow_Ternary_TrueBranch()
{
    float result = (5 > 3) ? 10.0 : 20.0;
    ASSERT(result == 10.0);
}

[Test]
void ControlFlow_Ternary_FalseBranch()
{
    float result = (5 < 3) ? 10.0 : 20.0;
    ASSERT(result == 20.0);
}

[Test]
void ControlFlow_Ternary_Nested()
{
    int x = 5;
    float result = (x < 3) ? 1.0 : (x < 7) ? 2.0 : 3.0;
    ASSERT(result == 2.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_TernaryVarying()
{
    uint lane = WaveGetLaneIndex();
    
    float result = (lane >= 2) ? 100.0 : 50.0;
    
    ASSERT(WaveReadLaneAt(result, 0) == 50.0);
    ASSERT(WaveReadLaneAt(result, 1) == 50.0);
    ASSERT(WaveReadLaneAt(result, 2) == 100.0);
    ASSERT(WaveReadLaneAt(result, 3) == 100.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_TernaryVarying_InExpression()
{
    uint lane = WaveGetLaneIndex();
    
    float base = 10.0;
    float result = base + ((lane & 1) ? 5.0 : 2.0);
    
    ASSERT(WaveReadLaneAt(result, 0) == 12.0); // 10 + 2
    ASSERT(WaveReadLaneAt(result, 1) == 15.0); // 10 + 5
    ASSERT(WaveReadLaneAt(result, 2) == 12.0); // 10 + 2
    ASSERT(WaveReadLaneAt(result, 3) == 15.0); // 10 + 5
}

// ============================================================================
// SWITCH STATEMENT TESTS (if supported)
// ============================================================================

// Note: HLSL supports switch statements, add tests if your interpreter does

// ============================================================================
// EDGE CASES AND STRESS TESTS
// ============================================================================

[Test]
void ControlFlow_DeepNesting()
{
    float sum = 0.0;
    
    if (true)
    {
        if (true)
        {
            if (true)
            {
                if (true)
                {
                    if (true)
                    {
                        sum = 42.0;
                    }
                }
            }
        }
    }
    
    ASSERT(sum == 42.0);
}

[Test]
void ControlFlow_EmptyBranches()
{
    float x = 5.0;
    
    if (x > 10.0)
    {
        // Empty
    }
    else if (x > 20.0)
    {
        // Empty
    }
    else
    {
        x = x + 1.0;
    }
    
    ASSERT(x == 6.0);
}

[Test]
void ControlFlow_ManySequentialIfs()
{
    float sum = 0.0;
    int x = 5;
    
    if (x > 0) sum = sum + 1.0;
    if (x > 1) sum = sum + 1.0;
    if (x > 2) sum = sum + 1.0;
    if (x > 3) sum = sum + 1.0;
    if (x > 4) sum = sum + 1.0;
    if (x > 5) sum = sum + 1.0;
    
    ASSERT(sum == 5.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_VaryingWithAllBranchesTaken()
{
    uint lane = WaveGetLaneIndex();
    
    float result = 0.0;
    
    // Each lane takes a different branch
    if (lane == 0)
        result = 10.0;
    else if (lane == 1)
        result = 20.0;
    else if (lane == 2)
        result = 30.0;
    else
        result = 40.0;
    
    ASSERT(WaveReadLaneAt(result, 0) == 10.0);
    ASSERT(WaveReadLaneAt(result, 1) == 20.0);
    ASSERT(WaveReadLaneAt(result, 2) == 30.0);
    ASSERT(WaveReadLaneAt(result, 3) == 40.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_VaryingLoopWithEarlyExit()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    
    for (int i = 0; i < 100; i++)
    {
        sum = sum + 1.0;
        
        if (i >= lane * 2)
            break;
    }
    
    ASSERT(WaveReadLaneAt(sum, 0) == 1.0);
    ASSERT(WaveReadLaneAt(sum, 1) == 3.0);
    ASSERT(WaveReadLaneAt(sum, 2) == 5.0);
    ASSERT(WaveReadLaneAt(sum, 3) == 7.0);
}

[Test]
void ControlFlow_LoopWithNoBody()
{
    int i = 0;
    for (; i < 5; i++);
    
    ASSERT(i == 5);
}

[Test]
void ControlFlow_LoopWithComplexCondition()
{
    float sum = 0.0;
    int i = 0;
    int j = 10;
    
    while (i < 5 && j > 5)
    {
        sum = sum + 1.0;
        i = i + 1;
        j = j - 1;
    }
    
    ASSERT(sum == 5.0);
}

[Test]
[WarpSize(4, 1)]
void ControlFlow_VaryingNestedBreakContinue()
{
    uint lane = WaveGetLaneIndex();
    
    float sum = 0.0;
    
    for (int i = 0; i < 5; i++)
    {
        if (i == 3 && lane < 2)
            break;
        
        for (int j = 0; j < 3; j++)
        {
            if (j == 1 && (lane & 1))
                continue;
            
            sum = sum + 1.0;
        }
    }
    
    // Lane 0 (even, breaks at i=3): i=0,1,2 with j=0,1,2 = 3*3 = 9
    ASSERT(WaveReadLaneAt(sum, 0) == 9.0);
    
    // Lane 1 (odd, breaks at i=3, skips j=1): i=0,1,2 with j=0,2 = 3*2 = 6
    ASSERT(WaveReadLaneAt(sum, 1) == 6.0);
    
    // Lane 2 (even, no break): i=0,1,2,3,4 with j=0,1,2 = 5*3 = 15
    ASSERT(WaveReadLaneAt(sum, 2) == 15.0);
    
    // Lane 3 (odd, no break, skips j=1): i=0,1,2,3,4 with j=0,2 = 5*2 = 10
    ASSERT(WaveReadLaneAt(sum, 3) == 10.0);
}

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

// Basic switch statement
[Test]
void Switch_BasicCase()
{
    int value = 2;
    int result = 0;
    
    switch (value)
    {
        case 1:
            result = 10;
            break;
        case 2:
            result = 20;
            break;
        case 3:
            result = 30;
            break;
    }
    
    ASSERT(result == 20);
}

// Switch with default case
[Test]
void Switch_DefaultCase()
{
    int value = 5;
    int result = 0;
    
    switch (value)
    {
        case 1:
            result = 10;
            break;
        case 2:
            result = 20;
            break;
        default:
            result = 99;
            break;
    }
    
    ASSERT(result == 99);
}

// Switch with empty case fall-through
[Test]
void Switch_EmptyCaseFallthrough()
{
    int value = 2;
    int result = 0;
    
    switch (value)
    {
        case 1:
        case 2:
        case 3:
            result = 10;
            break;
        case 4:
        case 5:
            result = 20;
            break;
    }
    
    ASSERT(result == 10);
}

// Multiple empty cases falling through
[Test]
void Switch_MultipleEmptyCases()
{
    int value = 4;
    int result = 0;
    
    switch (value)
    {
        case 1:
        case 2:
        case 3:
            result = 100;
            break;
        case 4:
        case 5:
        case 6:
        case 7:
            result = 200;
            break;
        default:
            result = 300;
            break;
    }
    
    ASSERT(result == 200);
}

// Switch with first case matching
[Test]
void Switch_FirstCase()
{
    int value = 0;
    int result = 0;
    
    switch (value)
    {
        case 0:
            result = 100;
            break;
        case 1:
            result = 200;
            break;
        case 2:
            result = 300;
            break;
    }
    
    ASSERT(result == 100);
}

// Switch with last case matching
[Test]
void Switch_LastCase()
{
    int value = 3;
    int result = 0;
    
    switch (value)
    {
        case 0:
            result = 100;
            break;
        case 1:
            result = 200;
            break;
        case 3:
            result = 300;
            break;
    }
    
    ASSERT(result == 300);
}

// Switch with no matching case and no default
[Test]
void Switch_NoMatchNoDefault()
{
    int value = 10;
    int result = 5;
    
    switch (value)
    {
        case 1:
            result = 10;
            break;
        case 2:
            result = 20;
            break;
    }
    
    ASSERT(result == 5); // Unchanged
}

// Switch with negative case values
[Test]
void Switch_NegativeCases()
{
    int value = -2;
    int result = 0;
    
    switch (value)
    {
        case -3:
            result = 1;
            break;
        case -2:
            result = 2;
            break;
        case -1:
            result = 3;
            break;
        case 0:
            result = 4;
            break;
    }
    
    ASSERT(result == 2);
}

// Switch with large case values
[Test]
void Switch_LargeCaseValues()
{
    int value = 1000;
    int result = 0;
    
    switch (value)
    {
        case 100:
            result = 1;
            break;
        case 500:
            result = 2;
            break;
        case 1000:
            result = 3;
            break;
        case 5000:
            result = 4;
            break;
    }
    
    ASSERT(result == 3);
}

// Switch with default at beginning
[Test]
void Switch_DefaultAtBeginning()
{
    int value = 99;
    int result = 0;
    
    switch (value)
    {
        default:
            result = 100;
            break;
        case 1:
            result = 10;
            break;
        case 2:
            result = 20;
            break;
    }
    
    ASSERT(result == 100);
}

// Switch with default in middle
[Test]
void Switch_DefaultInMiddle()
{
    int value = 99;
    int result = 0;
    
    switch (value)
    {
        case 1:
            result = 10;
            break;
        default:
            result = 100;
            break;
        case 2:
            result = 20;
            break;
    }
    
    ASSERT(result == 100);
}

// Switch inside loop
[Test]
void Switch_InsideLoop()
{
    int sum = 0;
    
    for (int i = 0; i < 5; i++)
    {
        switch (i)
        {
            case 0:
                sum += 1;
                break;
            case 1:
                sum += 2;
                break;
            case 2:
                sum += 3;
                break;
            case 3:
                sum += 4;
                break;
            case 4:
                sum += 5;
                break;
        }
    }
    
    ASSERT(sum == 15); // 1 + 2 + 3 + 4 + 5
}

// Loop inside switch
[Test]
void Switch_LoopInside()
{
    int value = 2;
    int result = 0;
    
    switch (value)
    {
        case 1:
            for (int i = 0; i < 3; i++)
                result += 1;
            break;
        case 2:
            for (int i = 0; i < 5; i++)
                result += 2;
            break;
        case 3:
            for (int i = 0; i < 2; i++)
                result += 3;
            break;
    }
    
    ASSERT(result == 10); // 5 * 2
}

// Nested switch statements
[Test]
void Switch_Nested()
{
    int outer = 2;
    int inner = 3;
    int result = 0;
    
    switch (outer)
    {
        case 1:
            result = 10;
            break;
        case 2:
            switch (inner)
            {
                case 1:
                    result = 21;
                    break;
                case 2:
                    result = 22;
                    break;
                case 3:
                    result = 23;
                    break;
            }
            break;
        case 3:
            result = 30;
            break;
    }
    
    ASSERT(result == 23);
}

// Switch with if-else inside
[Test]
void Switch_IfElseInside()
{
    int value = 2;
    int condition = 1;
    int result = 0;
    
    switch (value)
    {
        case 1:
            result = 10;
            break;
        case 2:
            if (condition > 0)
                result = 20;
            else
                result = 25;
            break;
        case 3:
            result = 30;
            break;
    }
    
    ASSERT(result == 20);
}

// If-else with switch inside
[Test]
void Switch_InsideIfElse()
{
    int condition = 1;
    int value = 2;
    int result = 0;
    
    if (condition > 0)
    {
        switch (value)
        {
            case 1:
                result = 10;
                break;
            case 2:
                result = 20;
                break;
        }
    }
    else
    {
        result = 99;
    }
    
    ASSERT(result == 20);
}

// Switch with multiple statements per case
[Test]
void Switch_MultipleStatementsPerCase()
{
    int value = 2;
    int result = 0;
    int multiplier = 1;
    
    switch (value)
    {
        case 1:
            result = 10;
            multiplier = 2;
            result *= multiplier;
            break;
        case 2:
            result = 5;
            multiplier = 3;
            result *= multiplier;
            break;
        case 3:
            result = 2;
            multiplier = 5;
            result *= multiplier;
            break;
    }
    
    ASSERT(result == 15); // 5 * 3
}

// Switch with break in nested loop
[Test]
void Switch_BreakInNestedLoop()
{
    int value = 2;
    int count = 0;
    
    switch (value)
    {
        case 1:
            count = 10;
            break;
        case 2:
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                    break;
                count++;
            }
            break;
        case 3:
            count = 30;
            break;
    }
    
    ASSERT(count == 5);
}

// Switch inside loop with break (break exits switch, not loop)
[Test]
void Switch_InsideLoopWithBreak()
{
    int sum = 0;
    
    for (int i = 0; i < 5; i++)
    {
        switch (i)
        {
            case 0:
                sum += 1;
                break;
            case 1:
                sum += 2;
                break;
            case 2:
                break; // Does nothing, continues loop
            case 3:
                sum += 4;
                break;
            default:
                sum += 5;
                break;
        }
    }
    
    ASSERT(sum == 12); // 1 + 2 + 0 + 4 + 5
}

// Switch with only default case
[Test]
void Switch_OnlyDefault()
{
    int value = 5;
    int result = 0;
    
    switch (value)
    {
        default:
            result = 100;
            break;
    }
    
    ASSERT(result == 100);
}

// Switch with single case
[Test]
void Switch_SingleCase()
{
    int value = 1;
    int result = 0;
    
    switch (value)
    {
        case 1:
            result = 50;
            break;
    }
    
    ASSERT(result == 50);
}

// Switch with many cases
[Test]
void Switch_ManyCases()
{
    int value = 7;
    int result = 0;
    
    switch (value)
    {
        case 0: result = 0; break;
        case 1: result = 1; break;
        case 2: result = 2; break;
        case 3: result = 3; break;
        case 4: result = 4; break;
        case 5: result = 5; break;
        case 6: result = 6; break;
        case 7: result = 7; break;
        case 8: result = 8; break;
        case 9: result = 9; break;
        default: result = -1; break;
    }
    
    ASSERT(result == 7);
}

// Switch with non-contiguous case values
[Test]
void Switch_NonContiguousCases()
{
    int value = 50;
    int result = 0;
    
    switch (value)
    {
        case 10:
            result = 1;
            break;
        case 50:
            result = 2;
            break;
        case 100:
            result = 3;
            break;
        case 200:
            result = 4;
            break;
    }
    
    ASSERT(result == 2);
}

// Switch with variable modified in case
[Test]
void Switch_ModifyVariable()
{
    int value = 2;
    int counter = 0;
    
    switch (value)
    {
        case 1:
            counter++;
            counter *= 2;
            break;
        case 2:
            counter += 5;
            counter *= 3;
            break;
        case 3:
            counter += 10;
            break;
    }
    
    ASSERT(counter == 15); // (0 + 5) * 3
}

// Switch evaluating expression
[Test]
void Switch_Expression()
{
    int a = 5;
    int b = 3;
    int result = 0;
    
    switch (a - b)
    {
        case 0:
            result = 10;
            break;
        case 1:
            result = 20;
            break;
        case 2:
            result = 30;
            break;
        case 3:
            result = 40;
            break;
    }
    
    ASSERT(result == 30); // a - b = 2
}

// Switch with variable declared in case
[Test]
void Switch_VariableInCase()
{
    int value = 2;
    int result = 0;
    
    switch (value)
    {
        case 1:
        {
            int temp = 10;
            result = temp;
            break;
        }
        case 2:
        {
            int temp = 20;
            result = temp;
            break;
        }
        case 3:
        {
            int temp = 30;
            result = temp;
            break;
        }
    }
    
    ASSERT(result == 20);
}

// Switch with zero case
[Test]
void Switch_ZeroCase()
{
    int value = 0;
    int result = 0;
    
    switch (value)
    {
        case -1:
            result = 10;
            break;
        case 0:
            result = 20;
            break;
        case 1:
            result = 30;
            break;
    }
    
    ASSERT(result == 20);
}

// Switch with compound operations in cases
[Test]
void Switch_CompoundOperations()
{
    int value = 2;
    int a = 10;
    int b = 5;
    
    switch (value)
    {
        case 1:
            a += b;
            break;
        case 2:
            a -= b;
            b *= 2;
            break;
        case 3:
            a *= b;
            break;
    }
    
    ASSERT(a == 5);   // 10 - 5
    ASSERT(b == 10);  // 5 * 2
}

// Switch with empty cases at beginning
[Test]
void Switch_EmptyCasesAtBeginning()
{
    int value = 1;
    int result = 0;
    
    switch (value)
    {
        case 1:
        case 2:
        case 3:
            result = 100;
            break;
        case 4:
            result = 200;
            break;
        default:
            result = 300;
            break;
    }
    
    ASSERT(result == 100);
}

// Switch with empty cases at end
[Test]
void Switch_EmptyCasesAtEnd()
{
    int value = 6;
    int result = 0;
    
    switch (value)
    {
        case 1:
            result = 100;
            break;
        case 4:
        case 5:
        case 6:
            result = 200;
            break;
        default:
            result = 300;
            break;
    }
    
    ASSERT(result == 200);
}

// Switch default without break (falls off end)
[Test]
void Switch_DefaultNoBreak()
{
    int value = 99;
    int result = 0;
    
    switch (value)
    {
        case 1:
            result = 10;
            break;
        default:
            result = 100;
            // No break - falls off end
    }
    
    ASSERT(result == 100);
}

// Switch with boolean converted to int
[Test]
void Switch_BooleanValue()
{
    bool flag = true;
    int result = 0;
    
    switch (flag ? 1 : 0)
    {
        case 0:
            result = 10;
            break;
        case 1:
            result = 20;
            break;
    }
    
    ASSERT(result == 20);
}

// Empty case falling to default
[Test]
void Switch_EmptyCaseFallToDefault()
{
    int value = 5;
    int result = 0;
    
    switch (value)
    {
        case 1:
            result = 10;
            break;
        case 2:
            result = 20;
            break;
        case 5:
        default:
            result = 100;
            break;
    }
    
    ASSERT(result == 100);
}

// Multiple empty cases with different targets
[Test]
void Switch_MultipleEmptyCaseGroups()
{
    int value = 8;
    int result = 0;
    
    switch (value)
    {
        case 1:
        case 2:
            result = 10;
            break;
        case 3:
        case 4:
        case 5:
            result = 20;
            break;
        case 6:
        case 7:
        case 8:
        case 9:
            result = 30;
            break;
        default:
            result = 40;
            break;
    }
    
    ASSERT(result == 30);
}

// Switch with all empty cases except one
[Test]
void Switch_MostlyEmptyCases()
{
    int value = 3;
    int result = 0;
    
    switch (value)
    {
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
        case 10:
            result = 999;
            break;
        default:
            result = 1;
            break;
    }
    
    ASSERT(result == 999);
}

// Switch matching first of empty cases
[Test]
void Switch_MatchFirstEmptyCase()
{
    int value = 10;
    int result = 0;
    
    switch (value)
    {
        case 5:
            result = 50;
            break;
        case 10:
        case 11:
        case 12:
            result = 100;
            break;
        case 20:
            result = 200;
            break;
    }
    
    ASSERT(result == 100);
}

// Switch matching middle of empty cases
[Test]
void Switch_MatchMiddleEmptyCase()
{
    int value = 11;
    int result = 0;
    
    switch (value)
    {
        case 5:
            result = 50;
            break;
        case 10:
        case 11:
        case 12:
            result = 100;
            break;
        case 20:
            result = 200;
            break;
    }
    
    ASSERT(result == 100);
}

// Switch matching last of empty cases
[Test]
void Switch_MatchLastEmptyCase()
{
    int value = 12;
    int result = 0;
    
    switch (value)
    {
        case 5:
            result = 50;
            break;
        case 10:
        case 11:
        case 12:
            result = 100;
            break;
        case 20:
            result = 200;
            break;
    }
    
    ASSERT(result == 100);
}

// Switch with no break on last case
[Test]
void Switch_LastCaseNoBreak()
{
    int value = 3;
    int result = 0;
    
    switch (value)
    {
        case 1:
            result = 10;
            break;
        case 2:
            result = 20;
            break;
        case 3:
            result = 30;
            // No break, but it's the last case
    }
    
    ASSERT(result == 30);
}

