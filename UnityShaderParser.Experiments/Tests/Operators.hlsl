// ============================================================================
// ARITHMETIC OPERATORS - Scalars
// ============================================================================

[Test]
void Operator_ScalarAddition()
{
    float a = 5.0;
    float b = 3.0;
    float result = a + b;
    ASSERT(result == 8.0);
    
    // Negative values
    result = -5.0 + 3.0;
    ASSERT(result == -2.0);
    
    // Both negative
    result = -5.0 + (-3.0);
    ASSERT(result == -8.0);
    
    // Integer addition
    int ia = 10;
    int ib = 20;
    ASSERT(ia + ib == 30);
}

[Test]
void Operator_ScalarSubtraction()
{
    float result = 10.0 - 3.0;
    ASSERT(result == 7.0);
    
    // Negative result
    result = 3.0 - 10.0;
    ASSERT(result == -7.0);
    
    // Subtracting negative
    result = 10.0 - (-3.0);
    ASSERT(result == 13.0);
    
    // Integer subtraction
    int ia = 50;
    int ib = 30;
    ASSERT(ia - ib == 20);
}

[Test]
void Operator_ScalarMultiplication()
{
    float result = 5.0 * 3.0;
    ASSERT(result == 15.0);
    
    // Negative multiplication
    result = -5.0 * 3.0;
    ASSERT(result == -15.0);
    
    // Both negative
    result = -5.0 * (-3.0);
    ASSERT(result == 15.0);
    
    // By zero
    result = 5.0 * 0.0;
    ASSERT(result == 0.0);
    
    // Integer multiplication
    int ia = 7;
    int ib = 6;
    ASSERT(ia * ib == 42);
}

[Test]
void Operator_ScalarDivision()
{
    float result = 15.0 / 3.0;
    ASSERT(result == 5.0);
    
    // Negative division
    result = -15.0 / 3.0;
    ASSERT(result == -5.0);
    
    // Both negative
    result = -15.0 / (-3.0);
    ASSERT(result == 5.0);
    
    // Fractional result
    result = 7.0 / 2.0;
    ASSERT(abs(result - 3.5) < 0.001);
    
    // Integer division
    int ia = 20;
    int ib = 4;
    ASSERT(ia / ib == 5);
}

[Test]
void Operator_ScalarModulo()
{
    int result = 10 % 3;
    ASSERT(result == 1);
    
    // Exact division
    result = 10 % 5;
    ASSERT(result == 0);
    
    // Negative dividend
    result = -10 % 3;
    ASSERT(result == -1);
    
    // Large values
    result = 100 % 7;
    ASSERT(result == 2);
}

[Test]
void Operator_UnaryMinus()
{
    float a = 5.0;
    float result = -a;
    ASSERT(result == -5.0);
    
    // Double negative
    result = -(-a);
    ASSERT(result == 5.0);
    
    // On negative value
    float b = -3.0;
    result = -b;
    ASSERT(result == 3.0);
}

[Test]
void Operator_UnaryPlus()
{
    float a = 5.0;
    float result = +a;
    ASSERT(result == 5.0);
    
    float b = -3.0;
    result = +b;
    ASSERT(result == -3.0);
}

// ============================================================================
// ARITHMETIC OPERATORS - Vectors
// ============================================================================

[Test]
void Operator_VectorAddition()
{
    float3 a = float3(1.0, 2.0, 3.0);
    float3 b = float3(4.0, 5.0, 6.0);
    float3 result = a + b;
    
    ASSERT(result.x == 5.0);
    ASSERT(result.y == 7.0);
    ASSERT(result.z == 9.0);
    
    // With negative components
    float2 c = float2(-1.0, 2.0);
    float2 d = float2(3.0, -4.0);
    float2 result2 = c + d;
    ASSERT(result2.x == 2.0);
    ASSERT(result2.y == -2.0);
}

[Test]
void Operator_VectorSubtraction()
{
    float3 a = float3(10.0, 8.0, 6.0);
    float3 b = float3(4.0, 3.0, 2.0);
    float3 result = a - b;
    
    ASSERT(result.x == 6.0);
    ASSERT(result.y == 5.0);
    ASSERT(result.z == 4.0);
    
    // Resulting in negative
    float2 c = float2(1.0, 2.0);
    float2 d = float2(5.0, 3.0);
    float2 result2 = c - d;
    ASSERT(result2.x == -4.0);
    ASSERT(result2.y == -1.0);
}

[Test]
void Operator_VectorMultiplication()
{
    // Component-wise multiplication
    float3 a = float3(2.0, 3.0, 4.0);
    float3 b = float3(5.0, 6.0, 7.0);
    float3 result = a * b;
    
    ASSERT(result.x == 10.0);
    ASSERT(result.y == 18.0);
    ASSERT(result.z == 28.0);
    
    // With negative
    float2 c = float2(-2.0, 3.0);
    float2 d = float2(4.0, -5.0);
    float2 result2 = c * d;
    ASSERT(result2.x == -8.0);
    ASSERT(result2.y == -15.0);
}

[Test]
void Operator_VectorDivision()
{
    float3 a = float3(10.0, 20.0, 30.0);
    float3 b = float3(2.0, 4.0, 5.0);
    float3 result = a / b;
    
    ASSERT(result.x == 5.0);
    ASSERT(result.y == 5.0);
    ASSERT(result.z == 6.0);
    
    // With negative
    float2 c = float2(-10.0, 15.0);
    float2 d = float2(2.0, -3.0);
    float2 result2 = c / d;
    ASSERT(result2.x == -5.0);
    ASSERT(result2.y == -5.0);
}

[Test]
void Operator_VectorScalarMultiplication()
{
    // Vector * scalar
    float3 v = float3(2.0, 3.0, 4.0);
    float3 result = v * 3.0;
    
    ASSERT(result.x == 6.0);
    ASSERT(result.y == 9.0);
    ASSERT(result.z == 12.0);
    
    // Scalar * vector
    result = 2.0 * v;
    ASSERT(result.x == 4.0);
    ASSERT(result.y == 6.0);
    ASSERT(result.z == 8.0);
    
    // Negative scalar
    result = v * (-1.0);
    ASSERT(result.x == -2.0);
    ASSERT(result.y == -3.0);
    ASSERT(result.z == -4.0);
}

[Test]
void Operator_VectorScalarDivision()
{
    float3 v = float3(10.0, 20.0, 30.0);
    float3 result = v / 2.0;
    
    ASSERT(result.x == 5.0);
    ASSERT(result.y == 10.0);
    ASSERT(result.z == 15.0);
    
    // Negative scalar
    result = v / (-5.0);
    ASSERT(result.x == -2.0);
    ASSERT(result.y == -4.0);
    ASSERT(result.z == -6.0);
}

[Test]
void Operator_VectorUnaryMinus()
{
    float3 v = float3(1.0, -2.0, 3.0);
    float3 result = -v;
    
    ASSERT(result.x == -1.0);
    ASSERT(result.y == 2.0);
    ASSERT(result.z == -3.0);
    
    // Double negative
    result = -(-v);
    ASSERT(result.x == 1.0);
    ASSERT(result.y == -2.0);
    ASSERT(result.z == 3.0);
}

// ============================================================================
// ARITHMETIC OPERATORS - Matrices
// ============================================================================

[Test]
void Operator_MatrixAddition()
{
    float2x2 a = float2x2(1, 2, 3, 4);
    float2x2 b = float2x2(5, 6, 7, 8);
    float2x2 result = a + b;
    
    ASSERT(result[0][0] == 6);
    ASSERT(result[0][1] == 8);
    ASSERT(result[1][0] == 10);
    ASSERT(result[1][1] == 12);
}

[Test]
void Operator_MatrixSubtraction()
{
    float2x2 a = float2x2(10, 9, 8, 7);
    float2x2 b = float2x2(1, 2, 3, 4);
    float2x2 result = a - b;
    
    ASSERT(result[0][0] == 9);
    ASSERT(result[0][1] == 7);
    ASSERT(result[1][0] == 5);
    ASSERT(result[1][1] == 3);
}

[Test]
void Operator_MatrixScalarMultiplication()
{
    float2x2 m = float2x2(1, 2, 3, 4);
    float2x2 result = m * 3.0;
    
    ASSERT(result[0][0] == 3);
    ASSERT(result[0][1] == 6);
    ASSERT(result[1][0] == 9);
    ASSERT(result[1][1] == 12);
    
    // Scalar * matrix
    result = 2.0 * m;
    ASSERT(result[0][0] == 2);
    ASSERT(result[0][1] == 4);
    ASSERT(result[1][0] == 6);
    ASSERT(result[1][1] == 8);
}

[Test]
void Operator_MatrixMultiplication()
{
    float2x2 a = float2x2(1, 2, 3, 4);
    float2x2 b = float2x2(5, 6, 7, 8);
    float2x2 result = mul(a, b);
    
    // [1 2] * [5 6] = [19 22]
    // [3 4]   [7 8]   [43 50]
    ASSERT(result[0][0] == 19);
    ASSERT(result[0][1] == 22);
    ASSERT(result[1][0] == 43);
    ASSERT(result[1][1] == 50);
}

[Test]
void Operator_MatrixVectorMultiplication()
{
    float2x2 m = float2x2(1, 2, 3, 4);
    float2 v = float2(5, 6);
    float2 result = mul(m, v);
    
    // [1 2] * [5] = [17]
    // [3 4]   [6]   [39]
    ASSERT(result.x == 17);
    ASSERT(result.y == 39);
}

// ============================================================================
// COMPARISON OPERATORS
// ============================================================================

[Test]
void Operator_Equality()
{
    // Scalars
    ASSERT(5.0 == 5.0);
    ASSERT(!(5.0 == 3.0));
    ASSERT(0.0 == 0.0);
    ASSERT(-5.0 == -5.0);
    
    // Integers
    ASSERT(10 == 10);
    ASSERT(!(10 == 11));
    
    // Negative
    ASSERT(-5 == -5);
    ASSERT(!(-5 == 5));
}

[Test]
void Operator_Inequality()
{
    ASSERT(5.0 != 3.0);
    ASSERT(!(5.0 != 5.0));
    ASSERT(0.0 != 1.0);
    
    // Integers
    ASSERT(10 != 11);
    ASSERT(!(10 != 10));
    
    // Negative
    ASSERT(-5 != 5);
    ASSERT(!(-5 != -5));
}

[Test]
void Operator_LessThan()
{
    ASSERT(3.0 < 5.0);
    ASSERT(!(5.0 < 3.0));
    ASSERT(!(5.0 < 5.0));
    
    // Negative values
    ASSERT(-5.0 < -3.0);
    ASSERT(-5.0 < 0.0);
    ASSERT(-5.0 < 5.0);
    
    // Integers
    ASSERT(10 < 20);
    ASSERT(!(20 < 10));
}

[Test]
void Operator_LessThanOrEqual()
{
    ASSERT(3.0 <= 5.0);
    ASSERT(5.0 <= 5.0);
    ASSERT(!(5.0 <= 3.0));
    
    // Negative values
    ASSERT(-5.0 <= -3.0);
    ASSERT(-5.0 <= -5.0);
    ASSERT(-5.0 <= 0.0);
    
    // Integers
    ASSERT(10 <= 20);
    ASSERT(10 <= 10);
}

[Test]
void Operator_GreaterThan()
{
    ASSERT(5.0 > 3.0);
    ASSERT(!(3.0 > 5.0));
    ASSERT(!(5.0 > 5.0));
    
    // Negative values
    ASSERT(-3.0 > -5.0);
    ASSERT(0.0 > -5.0);
    ASSERT(5.0 > -5.0);
    
    // Integers
    ASSERT(20 > 10);
    ASSERT(!(10 > 20));
}

[Test]
void Operator_GreaterThanOrEqual()
{
    ASSERT(5.0 >= 3.0);
    ASSERT(5.0 >= 5.0);
    ASSERT(!(3.0 >= 5.0));
    
    // Negative values
    ASSERT(-3.0 >= -5.0);
    ASSERT(-5.0 >= -5.0);
    ASSERT(0.0 >= -5.0);
    
    // Integers
    ASSERT(20 >= 10);
    ASSERT(10 >= 10);
}

[Test]
void Operator_VectorComparison()
{
    float3 a = float3(1.0, 2.0, 3.0);
    float3 b = float3(1.0, 2.0, 3.0);
    float3 c = float3(4.0, 5.0, 6.0);
    
    // Component-wise less than
    bool3 result = a < c;
    ASSERT(result.x && result.y && result.z);
    
    // Component-wise greater than
    result = c > a;
    ASSERT(result.x && result.y && result.z);
    
    // Mixed comparison
    float3 d = float3(0.0, 5.0, 2.0);
    result = a < d;
    ASSERT(!result.x && result.y && !result.z);
}

// ============================================================================
// LOGICAL OPERATORS
// ============================================================================

[Test]
void Operator_LogicalAnd()
{
    ASSERT(true && true);
    ASSERT(!(true && false));
    ASSERT(!(false && true));
    ASSERT(!(false && false));
    
    // With comparisons
    ASSERT((5 > 3) && (10 > 8));
    ASSERT(!((5 > 3) && (10 < 8)));
}

[Test]
void Operator_LogicalOr()
{
    ASSERT(true || true);
    ASSERT(true || false);
    ASSERT(false || true);
    ASSERT(!(false || false));
    
    // With comparisons
    ASSERT((5 > 3) || (10 < 8));
    ASSERT((5 < 3) || (10 > 8));
    ASSERT(!((5 < 3) || (10 < 8)));
}

[Test]
void Operator_LogicalNot()
{
    ASSERT(!false);
    ASSERT(true);
    
    // Double negation
    ASSERT(!(!true));
    ASSERT(!(!false) == false);
    
    // With comparisons
    ASSERT(!(5 < 3));
    ASSERT(!(10 == 5));
}

[Test]
void Operator_LogicalCombinations()
{
    // Complex expressions
    ASSERT((true && true) || false);
    ASSERT(!((true && false) || false));
    ASSERT((true || false) && true);
    ASSERT(!((false || false) && true));
    
    // With comparisons
    ASSERT(((5 > 3) && (10 > 8)) || (2 < 1));
    ASSERT(!((5 < 3) && (10 > 8)));
}

// ============================================================================
// BITWISE OPERATORS
// ============================================================================

[Test]
void Operator_BitwiseAnd()
{
    uint a = 0b1100;
    uint b = 0b1010;
    uint result = a & b;
    ASSERT(result == 0b1000);
    
    // With all bits
    ASSERT((0xFF & 0xFF) == 0xFF);
    ASSERT((0xFF & 0x00) == 0x00);
    
    // Masking
    uint mask = 0x0F;
    uint value = 0xAB;
    ASSERT((value & mask) == 0x0B);
}

[Test]
void Operator_BitwiseOr()
{
    uint a = 0b1100;
    uint b = 0b1010;
    uint result = a | b;
    ASSERT(result == 0b1110);
    
    // With all bits
    ASSERT((0xFF | 0xFF) == 0xFF);
    ASSERT((0xFF | 0x00) == 0xFF);
    ASSERT((0x00 | 0x00) == 0x00);
    
    // Combining flags
    uint flag1 = 0x01;
    uint flag2 = 0x04;
    ASSERT((flag1 | flag2) == 0x05);
}

[Test]
void Operator_BitwiseXor()
{
    uint a = 0b1100;
    uint b = 0b1010;
    uint result = a ^ b;
    ASSERT(result == 0b0110);
    
    // XOR with self is zero
    ASSERT((0xFF ^ 0xFF) == 0x00);
    
    // XOR with zero is identity
    ASSERT((0xAB ^ 0x00) == 0xAB);
    
    // Double XOR returns original
    uint value = 0x5A;
    uint key = 0x3C;
    ASSERT(((value ^ key) ^ key) == value);
}

[Test]
void Operator_BitwiseNot()
{
    uint a = 12;
    uint result = ~a;

    // Lower 4 bits should be 0011, upper bits all 1
    ASSERT((result & 0xF) == 3);
    
    // Double negation
    ASSERT(~(~a) == a);
    
    // NOT of 0 is all 1s
    ASSERT(~0u == 0xFFFFFFFF);
}

[Test]
void Operator_LeftShift()
{
    uint a = 0b0001;
    ASSERT((a << 0) == 0b0001);
    ASSERT((a << 1) == 0b0010);
    ASSERT((a << 2) == 0b0100);
    ASSERT((a << 3) == 0b1000);
    
    // Shifting by multiple positions
    uint b = 5; // 0b0101
    ASSERT((b << 2) == 20); // 0b10100
    
    // Power of 2 multiplication
    ASSERT((3 << 4) == 48); // 3 * 16
}

[Test]
void Operator_RightShift()
{
    uint a = 0b1000;
    ASSERT((a >> 0) == 0b1000);
    ASSERT((a >> 1) == 0b0100);
    ASSERT((a >> 2) == 0b0010);
    ASSERT((a >> 3) == 0b0001);
    
    // Shifting by multiple positions
    uint b = 20; // 0b10100
    ASSERT((b >> 2) == 5); // 0b0101
    
    // Division by power of 2
    ASSERT((48 >> 4) == 3); // 48 / 16
}

[Test]
void Operator_BitwiseCombinations()
{
    uint a = 0b1100;
    uint b = 0b1010;
    
    // (A & B) | (A ^ B) == A | B
    ASSERT(((a & b) | (a ^ b)) == (a | b));
    
    // A & ~A == 0
    ASSERT((a & ~a) == 0);
    
    // A | ~A == all 1s
    ASSERT((a | ~a) == 0xFFFFFFFF);
    
    // De Morgan's law: ~(A & B) == ~A | ~B
    ASSERT(~(a & b) == (~a | ~b));
    
    // De Morgan's law: ~(A | B) == ~A & ~B
    ASSERT(~(a | b) == (~a & ~b));
}

// ============================================================================
// TERNARY OPERATOR
// ============================================================================

[Test]
void Operator_Ternary()
{
    // Basic usage
    int result = (5 > 3) ? 10 : 20;
    ASSERT(result == 10);
    
    result = (5 < 3) ? 10 : 20;
    ASSERT(result == 20);
    
    // With floats
    float fresult = (2.0 > 1.0) ? 3.5 : 7.5;
    ASSERT(fresult == 3.5);
    
    // Nested ternary
    int a = 5;
    result = (a > 10) ? 1 : (a > 5) ? 2 : 3;
    ASSERT(result == 3);
    
    a = 7;
    result = (a > 10) ? 1 : (a > 5) ? 2 : 3;
    ASSERT(result == 2);
    
    // With vectors
    float3 v = (true) ? float3(1, 2, 3) : float3(4, 5, 6);
    ASSERT(v.x == 1 && v.y == 2 && v.z == 3);
}

// ============================================================================
// COMPOUND ASSIGNMENT OPERATORS
// ============================================================================

[Test]
void Operator_AddAssign()
{
    float a = 10.0;
    a += 5.0;
    ASSERT(a == 15.0);
    
    // Multiple operations
    a += 3.0;
    ASSERT(a == 18.0);
    
    // With negative
    a += -8.0;
    ASSERT(a == 10.0);
    
    // Integers
    int ia = 5;
    ia += 10;
    ASSERT(ia == 15);
}

[Test]
void Operator_SubtractAssign()
{
    float a = 10.0;
    a -= 3.0;
    ASSERT(a == 7.0);
    
    // Multiple operations
    a -= 2.0;
    ASSERT(a == 5.0);
    
    // With negative
    a -= -5.0;
    ASSERT(a == 10.0);
    
    // Integers
    int ia = 20;
    ia -= 8;
    ASSERT(ia == 12);
}

[Test]
void Operator_MultiplyAssign()
{
    float a = 5.0;
    a *= 3.0;
    ASSERT(a == 15.0);
    
    // Multiple operations
    a *= 2.0;
    ASSERT(a == 30.0);
    
    // With negative
    a *= -1.0;
    ASSERT(a == -30.0);
    
    // Integers
    int ia = 7;
    ia *= 4;
    ASSERT(ia == 28);
}

[Test]
void Operator_DivideAssign()
{
    float a = 20.0;
    a /= 4.0;
    ASSERT(a == 5.0);
    
    // Multiple operations
    a /= 2.0;
    ASSERT(a == 2.5);
    
    // Integers
    int ia = 100;
    ia /= 5;
    ASSERT(ia == 20);
}

[Test]
void Operator_ModuloAssign()
{
    int a = 17;
    a %= 5;
    ASSERT(a == 2);
    
    a %= 2;
    ASSERT(a == 0);
}

[Test]
void Operator_BitwiseAndAssign()
{
    uint a = 0b1110;
    a &= 0b1100;
    ASSERT(a == 0b1100);
    
    a &= 0b0100;
    ASSERT(a == 0b0100);
}

[Test]
void Operator_BitwiseOrAssign()
{
    uint a = 0b1000;
    a |= 0b0100;
    ASSERT(a == 0b1100);
    
    a |= 0b0011;
    ASSERT(a == 0b1111);
}

[Test]
void Operator_BitwiseXorAssign()
{
    uint a = 0b1100;
    a ^= 0b1010;
    ASSERT(a == 0b0110);
    
    // XOR with self becomes zero
    a ^= a;
    ASSERT(a == 0);
}

[Test]
void Operator_LeftShiftAssign()
{
    uint a = 1;
    a <<= 2;
    ASSERT(a == 4);
    
    a <<= 3;
    ASSERT(a == 32);
}

[Test]
void Operator_RightShiftAssign()
{
    uint a = 32;
    a >>= 2;
    ASSERT(a == 8);
    
    a >>= 2;
    ASSERT(a == 2);
}

[Test]
void Operator_VectorCompoundAssign()
{
    float3 v = float3(1.0, 2.0, 3.0);
    
    v += float3(1.0, 1.0, 1.0);
    ASSERT(v.x == 2.0 && v.y == 3.0 && v.z == 4.0);
    
    v *= 2.0;
    ASSERT(v.x == 4.0 && v.y == 6.0 && v.z == 8.0);
    
    v -= float3(1.0, 2.0, 3.0);
    ASSERT(v.x == 3.0 && v.y == 4.0 && v.z == 5.0);
}

// ============================================================================
// INCREMENT AND DECREMENT OPERATORS
// ============================================================================

[Test]
void Operator_PreIncrement()
{
    int a = 5;
    int result = ++a;
    ASSERT(a == 6);
    ASSERT(result == 6);
    
    // Multiple increments
    ++a;
    ASSERT(a == 7);
    
    // In expression
    int b = ++a + 10;
    ASSERT(a == 8);
    ASSERT(b == 18);
}

[Test]
void Operator_PostIncrement()
{
    int a = 5;
    int result = a++;
    ASSERT(a == 6);
    ASSERT(result == 5); // Returns old value
    
    // Multiple increments
    a++;
    ASSERT(a == 7);
    
    // In expression
    int b = a++ + 10;
    ASSERT(a == 8);
    ASSERT(b == 17); // Uses old value of a (7)
}

[Test]
void Operator_PreDecrement()
{
    int a = 5;
    int result = --a;
    ASSERT(a == 4);
    ASSERT(result == 4);
    
    // Multiple decrements
    --a;
    ASSERT(a == 3);
    
    // In expression
    int b = --a + 10;
    ASSERT(a == 2);
    ASSERT(b == 12);
}

[Test]
void Operator_PostDecrement()
{
    int a = 5;
    int result = a--;
    ASSERT(a == 4);
    ASSERT(result == 5); // Returns old value
    
    // Multiple decrements
    a--;
    ASSERT(a == 3);
    
    // In expression
    int b = a-- + 10;
    ASSERT(a == 2);
    ASSERT(b == 13); // Uses old value of a (3)
}

[Test]
void Operator_IncrementDecrementFloat()
{
    float a = 5.5;
    a++;
    ASSERT(abs(a - 6.5) < 0.001);
    
    a--;
    ASSERT(abs(a - 5.5) < 0.001);
    
    ++a;
    ASSERT(abs(a - 6.5) < 0.001);
    
    --a;
    ASSERT(abs(a - 5.5) < 0.001);
}

// ============================================================================
// OPERATOR PRECEDENCE
// ============================================================================

[Test]
void Operator_Precedence_MultiplicationBeforeAddition()
{
    int result = 2 + 3 * 4;
    ASSERT(result == 14); // Not 20
    
    result = 3 * 4 + 2;
    ASSERT(result == 14);
    
    // With parentheses
    result = (2 + 3) * 4;
    ASSERT(result == 20);
}

[Test]
void Operator_Precedence_DivisionBeforeSubtraction()
{
    int result = 10 - 6 / 2;
    ASSERT(result == 7); // Not 2
    
    result = 6 / 2 - 1;
    ASSERT(result == 2);
    
    // With parentheses
    result = (10 - 6) / 2;
    ASSERT(result == 2);
}

[Test]
void Operator_Precedence_UnaryVsBinary()
{
    int result = -2 + 5;
    ASSERT(result == 3);
    
    result = 5 + -2;
    ASSERT(result == 3);
    
    result = -2 * 3;
    ASSERT(result == -6);
    
    result = 3 * -2;
    ASSERT(result == -6);
}

[Test]
void Operator_Precedence_ComparisonVsLogical()
{
    // Comparison has higher precedence than logical AND
    bool result = 5 > 3 && 10 > 8;
    ASSERT(result == true);
    
    result = 5 < 3 || 10 > 8;
    ASSERT(result == true);
    
    // Complex expression
    result = 5 > 3 && 10 > 8 || 2 > 5;
    ASSERT(result == true); // (5 > 3 && 10 > 8) || 2 > 5
}

[Test]
void Operator_Precedence_BitwiseVsComparison()
{
    // Bitwise has lower precedence than comparison
    uint a = 5;
    uint b = 3;
    
    // This is ((a > b) & 1), not (a > (b & 1))
    bool result = (a > b & 1);
    ASSERT(result == true); // (5 > 3) & 1 = 1 & 1 = 1
}

[Test]
void Operator_Precedence_ShiftVsArithmetic()
{
    // Shift has lower precedence than addition
    int result = 1 << 2 + 1;
    ASSERT(result == 8); // 1 << (2 + 1) = 1 << 3 = 8
    
    // With parentheses
    result = (1 << 2) + 1;
    ASSERT(result == 5); // 4 + 1 = 5
}

[Test]
void Operator_Precedence_TernaryVsLogical()
{
    // Ternary has very low precedence
    int result = true ? 1 : 0 + 5;
    ASSERT(result == 1); // true ? 1 : (0 + 5)
    
    result = false ? 1 : 0 + 5;
    ASSERT(result == 5);
    
    // Logical OR has higher precedence than ternary
    bool bresult = false || true ? true : false;
    ASSERT(bresult == true); // (false || true) ? true : false
}

[Test]
void Operator_AssociativityLeftToRight()
{
    // Addition is left-associative
    int result = 10 - 5 - 2;
    ASSERT(result == 3); // (10 - 5) - 2 = 3, not 10 - (5 - 2) = 7
    
    // Division is left-associative
    result = 100 / 10 / 2;
    ASSERT(result == 5); // (100 / 10) / 2 = 5, not 100 / (10 / 2) = 20
}

[Test]
void Operator_AssociativityRightToLeft()
{
    // Assignment is right-associative
    int a, b, c;
    a = b = c = 5;
    ASSERT(a == 5 && b == 5 && c == 5);
    
    // Ternary is right-associative
    int result = true ? 1 : false ? 2 : 3;
    ASSERT(result == 1); // true ? 1 : (false ? 2 : 3)
    
    result = false ? 1 : true ? 2 : 3;
    ASSERT(result == 2); // false ? 1 : (true ? 2 : 3)
}

// ============================================================================
// COMPLEX OPERATOR COMBINATIONS
// ============================================================================

[Test]
void Operator_ComplexArithmetic()
{
    float result = (5.0 + 3.0) * 2.0 - 10.0 / 2.0;
    ASSERT(result == 11.0); // 8 * 2 - 5 = 16 - 5 = 11
    
    result = 2.0 * (3.0 + 4.0) / (5.0 - 3.0);
    ASSERT(result == 7.0); // 2 * 7 / 2 = 14 / 2 = 7
}

[Test]
void Operator_ComplexLogical()
{
    bool result = (5 > 3 && 10 < 20) || (15 == 15);
    ASSERT(result == true);
    
    result = !(5 < 3) && (10 > 5 || 3 < 2);
    ASSERT(result == true); // true && (true || false) = true
    
    result = (5 > 3 || 10 < 5) && !(20 == 20);
    ASSERT(result == false); // true && false = false
}

[Test]
void Operator_ComplexBitwise()
{
    uint a = 0b1100;
    uint b = 0b1010;
    uint c = 0b0110;
    
    uint result = (a & b) | c;
    ASSERT(result == 0b1110); // 0b1000 | 0b0110 = 0b1110
    
    result = a & (b | c);
    ASSERT(result == 0b1100); // 0b1100 & 0b1110 = 0b1100
    
    result = (a ^ b) & c;
    ASSERT(result == 0b0110); // 0b0110 & 0b0110 = 0b0110
}

[Test]
void Operator_MixedTypeArithmetic()
{
    // Int and float
    float result = 5 + 3.5;
    ASSERT(abs(result - 8.5) < 0.001);
    
    result = 10 / 4.0;
    ASSERT(abs(result - 2.5) < 0.001);
    
    // Vector and scalar
    float3 v = float3(1, 2, 3) * 2.0;
    ASSERT(v.x == 2.0 && v.y == 4.0 && v.z == 6.0);
}

[Test]
void Operator_ChainedComparisons()
{
    int a = 5;
    int b = 10;
    int c = 15;
    
    // Note: Can't do a < b < c directly in HLSL, need logical AND
    bool result = (a < b) && (b < c);
    ASSERT(result == true);
    
    result = (a < b) && (b > c);
    ASSERT(result == false);
    
    result = (a == 5) && (b == 10) && (c == 15);
    ASSERT(result == true);
}

[Test]
void Operator_CompoundAssignmentChain()
{
    int a = 10;
    a += 5;  // 15
    a *= 2;  // 30
    a -= 10; // 20
    a /= 4;  // 5
    ASSERT(a == 5);
    
    uint b = 0b0001;
    b <<= 2; // 0b0100
    b |= 0b0011; // 0b0111
    b &= 0b0101; // 0b0101
    ASSERT(b == 0b0101);
}

[Test]
void Operator_VectorArithmeticComplex()
{
    float3 a = float3(1, 2, 3);
    float3 b = float3(4, 5, 6);
    float3 c = float3(2, 2, 2);
    
    float3 result = (a + b) * c - float3(1, 1, 1);
    // (1+4, 2+5, 3+6) * (2,2,2) - (1,1,1)
    // (5, 7, 9) * (2,2,2) - (1,1,1)
    // (10, 14, 18) - (1,1,1)
    // (9, 13, 17)
    ASSERT(result.x == 9.0 && result.y == 13.0 && result.z == 17.0);
}

[Test]
void Operator_TernaryNested()
{
    int a = 7;
    int result = (a > 10) ? 100 :
                 (a > 5) ? 50 :
                 (a > 0) ? 10 : 0;
    ASSERT(result == 50);
    
    a = 15;
    result = (a > 10) ? 100 :
             (a > 5) ? 50 :
             (a > 0) ? 10 : 0;
    ASSERT(result == 100);
    
    a = -5;
    result = (a > 10) ? 100 :
             (a > 5) ? 50 :
             (a > 0) ? 10 : 0;
    ASSERT(result == 0);
}

[Test]
void Operator_BitwiseShiftCombinations()
{
    uint value = 0b00001111;
    
    // Shift left then right
    uint result = (value << 2) >> 2;
    ASSERT(result == value);
    
    // Shift and mask
    result = (value << 4) & 0xFF;
    ASSERT(result == 0b11110000);
    
    // Complex shifting
    result = ((value << 1) | (value >> 1));
    ASSERT(result == 0b00011111);
}

// ============================================================================
// EDGE CASES AND SPECIAL VALUES
// ============================================================================

[Test]
void Operator_DivisionByZero()
{
    // Division by zero should produce infinity
    float result = 1.0 / 0.0;
    ASSERT(isinf(result));
    
    result = -1.0 / 0.0;
    ASSERT(isinf(result));
}

[Test]
void Operator_ZeroDividedByZero()
{
    // 0/0 should produce NaN
    float result = 0.0 / 0.0;
    ASSERT(isnan(result));
}

[Test]
void Operator_MultiplicationByZero()
{
    ASSERT(5.0 * 0.0 == 0.0);
    ASSERT(0.0 * 5.0 == 0.0);
    ASSERT(-5.0 * 0.0 == 0.0);
    
    // Vector by zero
    float3 v = float3(1, 2, 3) * 0.0;
    ASSERT(v.x == 0.0 && v.y == 0.0 && v.z == 0.0);
}

[Test]
void Operator_NegativeZero()
{
    float a = 0.0;
    float b = -0.0;
    
    // -0.0 should equal 0.0
    ASSERT(a == b);
    
    // But sign might be preserved in some operations
    float result = -a;
    ASSERT(result == 0.0 || result == -0.0);
}

[Test]
void Operator_LargeNumbers()
{
    float large = 1e30;
    float result = large + 1.0;
    
    // Adding small number to very large number might lose precision
    // But should not overflow
    ASSERT(isfinite(result));
    
    result = large * 2.0;
    ASSERT(isfinite(result) || isinf(result));
}

[Test]
void Operator_SmallNumbers()
{
    float small = 1e-30;
    float result = small * 0.1;
    
    // Very small numbers should not underflow to zero unless truly tiny
    ASSERT(result >= 0.0);
    
    result = small / 1e10;
    ASSERT(result >= 0.0);
}

[Test]
void Operator_IntegerOverflow()
{
    // Note: Overflow behavior may be undefined or wrap around
    int maxInt = 2147483647; // Assuming 32-bit int
    int result = maxInt + 1;
    
    // Typically wraps to negative
    ASSERT(result < 0 || result == maxInt + 1);
}

[Test]
void Operator_ModuloNegative()
{
    // Modulo with negative dividend
    int result = -10 % 3;
    ASSERT(result == -1 || result == 2); // Implementation defined
    
    // Modulo with negative divisor
    result = 10 % -3;
    ASSERT(result == 1 || result == -2); // Implementation defined
}

[Test]
void Operator_ComparisonWithNaN()
{
    float nan = 0.0 / 0.0;
    
    // NaN is not equal to anything, including itself
    ASSERT(!(nan == nan));
    ASSERT(nan != nan);
    
    // NaN comparisons are always false except !=
    ASSERT(!(nan < 5.0));
    ASSERT(!(nan > 5.0));
    ASSERT(!(nan <= 5.0));
    ASSERT(!(nan >= 5.0));
}

[Test]
void Operator_ComparisonWithInfinity()
{
    float inf = 1.0 / 0.0;
    float negInf = -1.0 / 0.0;
    
    // Infinity comparisons
    ASSERT(inf > 1000000.0);
    ASSERT(negInf < -1000000.0);
    ASSERT(inf > negInf);
    ASSERT(!(inf < inf));
    ASSERT(inf == inf);
}

// ============================================================================
// SWIZZLE OPERATORS (Read)
// ============================================================================

[Test]
void Operator_SwizzleRead()
{
    float4 v = float4(1, 2, 3, 4);
    
    // Single component
    ASSERT(v.x == 1.0);
    ASSERT(v.y == 2.0);
    ASSERT(v.z == 3.0);
    ASSERT(v.w == 4.0);
    
    // Alternative names
    ASSERT(v.r == 1.0);
    ASSERT(v.g == 2.0);
    ASSERT(v.b == 3.0);
    ASSERT(v.a == 4.0);
    
    // Two components
    float2 xy = v.xy;
    ASSERT(xy.x == 1.0 && xy.y == 2.0);
    
    float2 zw = v.zw;
    ASSERT(zw.x == 3.0 && zw.y == 4.0);
    
    // Three components
    float3 xyz = v.xyz;
    ASSERT(xyz.x == 1.0 && xyz.y == 2.0 && xyz.z == 3.0);
    
    // Repeated components
    float2 xx = v.xx;
    ASSERT(xx.x == 1.0 && xx.y == 1.0);
    
    // Out of order
    float3 zyx = v.zyx;
    ASSERT(zyx.x == 3.0 && zyx.y == 2.0 && zyx.z == 1.0);
    
    // Complex swizzle
    float4 wzyx = v.wzyx;
    ASSERT(wzyx.x == 4.0 && wzyx.y == 3.0 && wzyx.z == 2.0 && wzyx.w == 1.0);
}

[Test]
void Operator_SwizzleArithmetic()
{
    float4 v = float4(1, 2, 3, 4);
    
    // Arithmetic on swizzle
    float2 result = v.xy + v.zw;
    ASSERT(result.x == 4.0 && result.y == 6.0);
    
    result = v.xy * 2.0;
    ASSERT(result.x == 2.0 && result.y == 4.0);
    
    // Swizzle in complex expression
    float3 complex = v.xyz + v.yzw * 2.0;
    // (1,2,3) + (2,3,4)*2 = (1,2,3) + (4,6,8) = (5,8,11)
    ASSERT(complex.x == 5.0 && complex.y == 8.0 && complex.z == 11.0);
}

// ============================================================================
// SWIZZLE OPERATORS (Write)
// ============================================================================

[Test]
void Operator_SwizzleWrite()
{
    float4 v = float4(1, 2, 3, 4);
    
    // Write to single component
    v.x = 10.0;
    ASSERT(v.x == 10.0 && v.y == 2.0 && v.z == 3.0 && v.w == 4.0);
    
    // Write to multiple components
    v.xy = float2(20, 30);
    ASSERT(v.x == 20.0 && v.y == 30.0);
    
    // Write to three components
    v.yzw = float3(40, 50, 60);
    ASSERT(v.y == 40.0 && v.z == 50.0 && v.w == 60.0);
    
    // Write all components
    v.xyzw = float4(100, 200, 300, 400);
    ASSERT(v.x == 100.0 && v.y == 200.0 && v.z == 300.0 && v.w == 400.0);
}

[Test]
void Operator_SwizzleWriteReordered()
{
    float4 v = float4(1, 2, 3, 4);
    
    // Write in different order
    v.yx = float2(10, 20);
    ASSERT(v.x == 20.0 && v.y == 10.0);
    
    v.zyx = float3(30, 40, 50);
    ASSERT(v.x == 50.0 && v.y == 40.0 && v.z == 30.0);
}

[Test]
void Operator_SwizzleCompoundAssign()
{
    float4 v = float4(1, 2, 3, 4);
    
    // Compound assignment on swizzle
    v.xy += float2(10, 10);
    ASSERT(v.x == 11.0 && v.y == 12.0);
    
    v.zw *= 2.0;
    ASSERT(v.z == 6.0 && v.w == 8.0);
    
    v.xyz -= float3(1, 2, 3);
    ASSERT(v.x == 10.0 && v.y == 10.0 && v.z == 3.0);
}