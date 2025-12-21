using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public readonly struct HLSLRegister<T>
    {
        public readonly T UniformValue;
        public readonly T[] VaryingValues;
        public readonly bool IsVarying;
        public bool IsUniform => !IsVarying;

        public HLSLRegister(T value)
        {
            VaryingValues = null;
            UniformValue = value;
            IsVarying = false;
        }

        public HLSLRegister(T[] values)
        {
            VaryingValues = values;
            UniformValue = default;
            IsVarying = true;
        }

        public HLSLRegister<U> Map<U>(Func<T, U> mapper)
        {
            if (IsVarying)
            {
                T[] input = VaryingValues;
                return new HLSLRegister<U>(input.Select(mapper).ToArray());
            }
            else
            {
                return new HLSLRegister<U>(mapper(UniformValue));
            }
        }

        public HLSLRegister<U> MapThreads<U>(Func<T, int, U> mapper)
        {
            if (IsVarying)
            {
                T[] input = VaryingValues;
                return new HLSLRegister<U>(input.Select(mapper).ToArray());
            }
            else
            {
                return new HLSLRegister<U>(mapper(UniformValue, 0));
            }
        }

        public T Get(int threadIndex)
        {
            if (IsVarying)
                return VaryingValues[threadIndex];
            else
                return UniformValue;
        }

        public HLSLRegister<T> Set(int threadIndex, T value)
        {
            if (IsVarying)
            {
                var valCopy = VaryingValues.ToArray();
                valCopy[threadIndex] = value;
                return new HLSLRegister<T>(valCopy);
            }
            else
            {
                return new HLSLRegister<T>(value);
            }
        }

        public HLSLRegister<T> Vectorize(int threadCount)
        {
            if (IsVarying)
                return new HLSLRegister<T>(VaryingValues.ToArray());
            else
                return new HLSLRegister<T>(Enumerable.Repeat(UniformValue, threadCount).ToArray());
        }

        public HLSLRegister<T> Scalarize(int threadIndex)
        {
            return new HLSLRegister<T>(Get(threadIndex));
        }

        // If all lanes agree, collapse to scalar
        public HLSLRegister<T> Converge()
        {
            if (IsUniform)
                return new HLSLRegister<T>(Get(0));

            T first = Get(0);
            bool allSame = true;
            foreach (T value in VaryingValues)
            {
                if (!StructuralComparisons.StructuralEqualityComparer.Equals(first, value))
                {
                    allSame = false;
                    break;
                }
            }

            if (allSame)
                return new HLSLRegister<T>(first);
            else
                return new HLSLRegister<T>(VaryingValues.ToArray());
        }

        public int Size => IsVarying ? VaryingValues.Length : 1;
    }

    public abstract class HLSLValue
    {
    }

    // TODO: Use a union over "object" to avoid boxing of every individual value.
    public abstract class NumericValue : HLSLValue
    {
        public readonly ScalarType Type;

        public NumericValue(ScalarType type)
        {
            Type = type;
        }

        public static NumericValue operator +(NumericValue left, NumericValue right) => HLSLOperators.Add(left, right);
        public static NumericValue operator -(NumericValue left, NumericValue right) => HLSLOperators.Sub(left, right);
        public static NumericValue operator *(NumericValue left, NumericValue right) => HLSLOperators.Mul(left, right);
        public static NumericValue operator /(NumericValue left, NumericValue right) => HLSLOperators.Div(left, right);
        public static NumericValue operator %(NumericValue left, NumericValue right) => HLSLOperators.Mod(left, right);
        public static NumericValue operator <(NumericValue left, NumericValue right) => HLSLOperators.Less(left, right);
        public static NumericValue operator >(NumericValue left, NumericValue right) => HLSLOperators.Greater(left, right);
        public static NumericValue operator <=(NumericValue left, NumericValue right) => HLSLOperators.LessEqual(left, right);
        public static NumericValue operator >=(NumericValue left, NumericValue right) => HLSLOperators.GreaterEqual(left, right);
        public static NumericValue operator ==(NumericValue left, NumericValue right) => HLSLOperators.Equal(left, right);
        public static NumericValue operator !=(NumericValue left, NumericValue right) => HLSLOperators.NotEqual(left, right);
        public static NumericValue operator ^(NumericValue left, NumericValue right) => HLSLOperators.BitXor(left, right);
        public static NumericValue operator |(NumericValue left, NumericValue right) => HLSLOperators.BitOr(left, right);
        public static NumericValue operator &(NumericValue left, NumericValue right) => HLSLOperators.BitAnd(left, right);
        public static NumericValue operator ~(NumericValue left) => HLSLOperators.BitNot(left);
        public static NumericValue operator !(NumericValue left) => HLSLOperators.BoolNegate(left);
        public static NumericValue operator -(NumericValue left) => HLSLOperators.Negate(left);

        public static implicit operator NumericValue(int v) => new ScalarValue(ScalarType.Int, new HLSLRegister<object>(v));
        public static implicit operator NumericValue(uint v) => new ScalarValue(ScalarType.Uint, new HLSLRegister<object>(v));
        public static implicit operator NumericValue(float v) => new ScalarValue(ScalarType.Float, new HLSLRegister<object>(v));
        public static implicit operator NumericValue(double v) => new ScalarValue(ScalarType.Double, new HLSLRegister<object>(v));
        public static implicit operator NumericValue(bool v) => new ScalarValue(ScalarType.Bool, new HLSLRegister<object>(v));
        public static implicit operator NumericValue(char v) => new ScalarValue(ScalarType.Char, new HLSLRegister<object>(v));
    }

    public sealed class StructValue : HLSLValue
    {
        public readonly string Name;
        public readonly Dictionary<string, HLSLValue> Members;

        public StructValue(string name, Dictionary<string, HLSLValue> members)
        {
            Name = name;
            Members = members;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("struct ");
            sb.AppendLine(Name);
            sb.AppendLine("{");
            foreach (var kvp in Members)
            {
                sb.Append("    ");
                sb.Append(kvp.Key);
                sb.Append(": ");
                sb.Append(Convert.ToString(kvp.Value, CultureInfo.InvariantCulture));
                sb.AppendLine(";");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    public sealed class ScalarValue : NumericValue
    {
        public readonly HLSLRegister<object> Value;

        //public ScalarValue(ScalarType type, object value)
        //    : this(type, new UniformRegister(value)) 
        //{
        //}

        public ScalarValue(ScalarType type, HLSLRegister<object> value)
            : base(type)
        {
            Value = value;
        }

        public override string ToString()
        {
            if (Value.IsVarying)
            {
                string[] vals = new string[Value.VaryingValues.Length];
                for (int i = 0; i < vals.Length; i++)
                    vals[i] = $"{i}: {ToString(i)}";
                return $"Varying({string.Join(", ", vals)})";
            }
            else
            {
                return ToString(0);
            }
        }
        public string ToString(int threadIndex) => Convert.ToString(Value.Get(threadIndex), CultureInfo.InvariantCulture);
    }

    public sealed class VectorValue : NumericValue
    {
        public readonly HLSLRegister<object[]> Values;
        public int Size => Values.Get(0).Length;

        //public VectorValue(ScalarType type, object[] values)
        //    : this(type, values.Select(x => new UniformRegister(x)).ToArray())
        //{
        //}

        public VectorValue(ScalarType type, HLSLRegister<object[]> values)
            : base(type)
        {
            Values = values;
        }

        public override string ToString()
        {
            if (Values.IsVarying)
            {
                string[] vals = new string[Values.VaryingValues.Length];
                for (int i = 0; i < vals.Length; i++)
                    vals[i] = $"{i}: {ToString(i)}";
                return $"Varying({string.Join(", ", vals)})";
            }
            else
            {
                return ToString(0);
            }
        }
        public string ToString(int threadIndex)
        {
            string type = PrintingUtil.GetEnumName(Type);
            return $"{type}{Size}({string.Join(", ", Values.Get(threadIndex).Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)))})"; // TODO: not thread 0
        }
    }

    public sealed class MatrixValue : NumericValue
    {
        public readonly int Rows;
        public readonly int Columns;
        public readonly HLSLRegister<object[]> Values;

        //public MatrixValue(ScalarType type, int rows, int columns, object[] values)
        //    : this(type, rows, columns, values.Select(x => new UniformRegister(x)).ToArray())
        //{
        //}

        public MatrixValue(ScalarType type, int rows, int columns, HLSLRegister<object[]> values)
            : base(type)
        {
            Rows = rows;
            Columns = columns;
            Values = values;
        }

        public override string ToString()
        {
            if (Values.IsVarying)
            {
                string[] vals = new string[Values.VaryingValues.Length];
                for (int i = 0; i < vals.Length; i++)
                    vals[i] = $"{i}: {ToString(i)}";
                return $"Varying({string.Join(", ", vals)})";
            }
            else
            {
                return ToString(0);
            }
        }
        public string ToString(int threadIndex)
        {
            string type = PrintingUtil.GetEnumName(Type);
            return $"{type}{Rows}{Columns}({string.Join(", ", Values.Get(threadIndex).Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)))})"; // TODO: not thread 0
        }
    }

    public sealed class PredefinedObjectValue : HLSLValue
    {
        public readonly PredefinedObjectType Type;
        public readonly HLSLValue[] TemplateArguments;

        public PredefinedObjectValue(PredefinedObjectType type, HLSLValue[] templateArguments)
        {
            Type = type;
            TemplateArguments = templateArguments;
        }

        public override string ToString()
        {
            string type = PrintingUtil.GetEnumName(Type);
            return $"{type}<{string.Join(", ", (IEnumerable<HLSLValue>)TemplateArguments)}>";
        }
    }

    public sealed class ArrayValue : HLSLValue
    {
        public readonly HLSLValue[] Values;

        public ArrayValue(HLSLValue[] values)
        {
            Values = values;
        }

        public override string ToString()
        {
            return string.Join(", ", (IEnumerable<HLSLValue>)Values);
        }
    }

    public static class HLSLValueUtils
    {
        public static object GetZeroValue(ScalarType type)
        {
            switch (type)
            {
                case ScalarType.Void:
                    return null;
                case ScalarType.Bool:
                    return default(bool);
                case ScalarType.Int:
                case ScalarType.Min16Int:
                case ScalarType.Min12Int:
                    return default(int);
                case ScalarType.Uint:
                case ScalarType.Min16Uint:
                case ScalarType.Min12Uint:
                    return default(uint);
                case ScalarType.Half:
                case ScalarType.Float:
                case ScalarType.Min16Float:
                case ScalarType.Min10Float:
                case ScalarType.UNormFloat:
                case ScalarType.SNormFloat:
                    return default(float);
                case ScalarType.Double:
                    return default(double);
                case ScalarType.String:
                    return string.Empty;
                case ScalarType.Char:
                    return default(char);
                default:
                    throw new InvalidOperationException();
            }
        }

        public static bool IsFloat(ScalarType type)
        {
            switch (type)
            {
                case ScalarType.Float:
                case ScalarType.Double:
                case ScalarType.Min16Float:
                case ScalarType.Min10Float:
                case ScalarType.UNormFloat:
                case ScalarType.SNormFloat:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsUint(ScalarType type)
        {
            switch (type)
            {
                case ScalarType.Uint:
                case ScalarType.Min16Uint:
                case ScalarType.Min12Uint:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInt(ScalarType type)
        {
            switch (type)
            {
                case ScalarType.Int:
                case ScalarType.Min16Int:
                case ScalarType.Min12Int:
                    return true;
                default:
                    return false;
            }
        }

        public static int GetScalarRank(ScalarType left)
        {
            switch (left)
            {
                case ScalarType.Bool: return 0;
                case ScalarType.Min12Int: return 1;
                case ScalarType.Min16Int: return 2;
                case ScalarType.Int: return 3;
                case ScalarType.Min12Uint: return 4;
                case ScalarType.Min16Uint: return 5;
                case ScalarType.Uint: return 6;
                case ScalarType.Min10Float: return 7;
                case ScalarType.Min16Float: return 8;
                case ScalarType.Half: return 9;
                case ScalarType.SNormFloat: return 10;
                case ScalarType.UNormFloat: return 11;
                case ScalarType.Float: return 12;
                case ScalarType.Double: return 13;
                default:
                    return -1;
            }
        }

        // Given 2 params, pick the one with highest rank.
        public static ScalarType PromoteScalarType(ScalarType left, ScalarType right)
        {
            return GetScalarRank(left) > GetScalarRank(right) ? left : right;
        }

        public static ScalarType PromoteForBitwiseBinOp(ScalarType left, ScalarType right)
        {
            if (IsUint(left) && IsUint(right))
                return PromoteScalarType(left, right);
            if (IsInt(left) && IsInt(right))
                return PromoteScalarType(left, right);
            if (IsUint(left))
                return left;
            if (IsInt(right))
                return right;
            if (IsInt(left))
                return left;
            if (IsInt(right))
                return right;
            return ScalarType.Uint;
        }

        public static VectorValue BroadcastToVector(NumericValue value, int size)
        {
            if (value is VectorValue vec)
            {
                return new VectorValue(vec.Type, vec.Values.Map(x =>
                {
                    int sizeDiff = size - x.Length;
                    if (sizeDiff > 0) // Expansion
                        return x.Append(Enumerable.Repeat(GetZeroValue(vec.Type), sizeDiff)).ToArray();
                    else if (size < x.Length) // Truncation
                        return x.Take(size).ToArray();
                    else
                        return x;
                }));
            }
            if (value is ScalarValue scalar)
                return new VectorValue(scalar.Type, scalar.Value.Map(x => Enumerable.Repeat(x, size).ToArray()));
            throw new InvalidOperationException();
        }

        public static MatrixValue BroadcastToMatrix(NumericValue value, int rows, int columns)
        {
            if (value is MatrixValue mat)
            {
                if (mat.Rows != rows || mat.Columns != columns)
                    throw new NotImplementedException();
                return mat;
            }
            if (value is ScalarValue scalar)
                return new MatrixValue(scalar.Type, rows, columns, scalar.Value.Map(x => Enumerable.Repeat(x, rows * columns).ToArray()));
            throw new InvalidOperationException();
        }

        public static (int rows, int columns) GetTensorSize(NumericValue value)
        {
            if (value is ScalarValue)
                return (1, 1);
            if (value is VectorValue vector)
                return (vector.Size, 1);
            if (value is MatrixValue matrix)
                return (matrix.Rows, matrix.Columns);
            return (0, 0);
        }

        public static object CastNumeric(ScalarType type, object value)
        {
            switch (type)
            {
                case ScalarType.Void:
                    return null;
                case ScalarType.Bool:
                    return Convert.ToBoolean(value);
                case ScalarType.Int:
                case ScalarType.Min16Int:
                case ScalarType.Min12Int:
                    if (value is float fi) return (int)fi;
                    if (value is double di) return (int)di;
                    return Convert.ToInt32(value);
                case ScalarType.Uint:
                case ScalarType.Min16Uint:
                case ScalarType.Min12Uint:
                    if (value is float fu) return (uint)fu;
                    if (value is double du) return (uint)du;
                    return Convert.ToUInt32(value);
                case ScalarType.Double:
                    return Convert.ToDouble(value);
                case ScalarType.Half:
                case ScalarType.Float:
                case ScalarType.Min16Float:
                case ScalarType.Min10Float:
                case ScalarType.UNormFloat:
                case ScalarType.SNormFloat:
                    return Convert.ToSingle(value);
                case ScalarType.String:
                    return Convert.ToString(value);
                case ScalarType.Char:
                    return Convert.ToChar(value);
                default:
                    throw new InvalidOperationException();
            }
        }

        public static NumericValue CastNumeric(ScalarType type, NumericValue value)
        {
            if (value is ScalarValue scalar)
                return new ScalarValue(type, scalar.Value.Map(x => CastNumeric(type, x)));
            if (value is VectorValue vector)
                return new VectorValue(type, vector.Values.Map(x => x.Select(y => CastNumeric(type, y)).ToArray()));
            if (value is MatrixValue matrix)
                return new MatrixValue(type, matrix.Rows, matrix.Columns, matrix.Values.Map(x => x.Select(y => CastNumeric(type, y)).ToArray()));
            throw new InvalidOperationException();
        }

        public static int GetThreadCount(NumericValue value)
        {
            switch (value)
            {
                case ScalarValue scalar: return scalar.Value.Size;
                case VectorValue vector: return vector.Values.Size;
                case MatrixValue matrix: return matrix.Values.Size;
                default: return 1;
            }
        }

        public static bool IsUniform(NumericValue value)
        {
            return GetThreadCount(value) == 1;
        }

        public static bool IsVarying(NumericValue value)
        {
            return GetThreadCount(value) > 1;
        }

        public static NumericValue Vectorize(NumericValue value, int threadCount)
        {
            if (value is ScalarValue scalar)
                return new ScalarValue(scalar.Type, scalar.Value.Vectorize(threadCount));
            if (value is VectorValue vector)
                return new VectorValue(vector.Type, vector.Values.Vectorize(threadCount));
            if (value is MatrixValue matrix)
                return new MatrixValue(matrix.Type, matrix.Rows, matrix.Columns, matrix.Values.Vectorize(threadCount));
            throw new InvalidOperationException();
        }

        public static HLSLValue Vectorize(HLSLValue value, int threadCount)
        {
            if (value is NumericValue num)
                return Vectorize(num, threadCount);
            if (value is StructValue str)
                return new StructValue(str.Name, str.Members.Select(x => (x.Key, Vectorize(x.Value, threadCount))).ToDictionary(x => x.Key, y => y.Item2));
            if (value is ArrayValue arr)
                return new ArrayValue(arr.Values.Select(x => Vectorize(x, threadCount)).ToArray());
            throw new InvalidOperationException();
        }

        public static NumericValue Scalarize(NumericValue value, int threadIndex)
        {
            if (value is ScalarValue scalar)
                return new ScalarValue(scalar.Type, scalar.Value.Scalarize(threadIndex));
            if (value is VectorValue vector)
                return new VectorValue(vector.Type, vector.Values.Scalarize(threadIndex));
            if (value is MatrixValue matrix)
                return new MatrixValue(matrix.Type, matrix.Rows, matrix.Columns, matrix.Values.Scalarize(threadIndex));
            throw new InvalidOperationException();
        }

        public static HLSLValue Scalarize(HLSLValue value, int threadIndex)
        {
            if (value is NumericValue num)
                return Scalarize(num, threadIndex);
            if (value is StructValue str)
                return new StructValue(str.Name, str.Members.Select(x => (x.Key, Scalarize(x.Value, threadIndex))).ToDictionary(x => x.Key, y => y.Item2));
            if (value is ArrayValue arr)
                return new ArrayValue(arr.Values.Select(x => Scalarize(x, threadIndex)).ToArray());
            throw new InvalidOperationException();
        }

        public static object ExtractThread(NumericValue value, int threadIndex)
        {
            if (value is ScalarValue scalar)
                return scalar.Value.Get(threadIndex);
            if (value is VectorValue vector)
                return vector.Values.Get(threadIndex);
            if (value is MatrixValue matrix)
                return matrix.Values.Get(threadIndex);
            throw new InvalidOperationException();
        }

        public static NumericValue SetThreadValue(NumericValue value, int threadIndex, object set)
        {
            if (value is ScalarValue scalar)
                return new ScalarValue(scalar.Type, scalar.Value.Set(threadIndex, set));
            if (value is VectorValue vector)
                return new VectorValue(vector.Type, vector.Values.Set(threadIndex, (object[])set));
            if (value is MatrixValue matrix)
                return new MatrixValue(matrix.Type, matrix.Rows, matrix.Columns, matrix.Values.Set(threadIndex, (object[])set));
            throw new InvalidOperationException();
        }

        // Given 2 params, promote such that no information is lost.
        // This includes promoting SGPR to VGPR.
        public static (NumericValue newLeft, NumericValue newRight) Promote(NumericValue left, NumericValue right, bool bitwiseOp)
        {
            int leftThreadCount = GetThreadCount(left);
            int rightThreadCount = GetThreadCount(right);
            if (leftThreadCount < rightThreadCount)
                left = Vectorize(left, rightThreadCount);
            else if (rightThreadCount < leftThreadCount)
                right = Vectorize(right, leftThreadCount);

            ScalarType type = bitwiseOp
                ? PromoteForBitwiseBinOp(left.Type, right.Type)
                : PromoteScalarType(left.Type, right.Type);

            bool needMatrix = left is MatrixValue || right is MatrixValue;
            bool needVector = left is VectorValue || right is VectorValue;

            if (needMatrix)
            {
                (int leftRows, int leftColumns) = GetTensorSize(left);
                (int rightRows, int rightColumns) = GetTensorSize(right);
                int newRows = Math.Max(leftRows, rightRows);
                int newColumns = Math.Max(rightRows, rightColumns);
                var resizedLeft = BroadcastToMatrix(left, newRows, newColumns);
                var resizedRight = BroadcastToMatrix(right, newRows, newColumns);
                return (CastNumeric(type, resizedLeft), CastNumeric(type, resizedRight));
            }

            if (needVector)
            {
                (int leftSize, _) = GetTensorSize(left);
                (int rightSize, _) = GetTensorSize(right);
                int newSize = Math.Max(leftSize, rightSize);
                var resizedLeft = BroadcastToVector(left, newSize);
                var resizedRight = BroadcastToVector(right, newSize);
                return (CastNumeric(type, resizedLeft),  CastNumeric(type, resizedRight));
            }

            return (CastNumeric(type, left), CastNumeric(type, right));
        }

        public static NumericValue Map(NumericValue value, Func<object, object> mapper)
        {
            if (value is ScalarValue scalar)
                return new ScalarValue(scalar.Type, scalar.Value.Map(mapper));
            if (value is VectorValue vector)
                return new VectorValue(vector.Type, vector.Values.Map(x => x.Select(mapper).ToArray()));
            if (value is MatrixValue matrix)
                return new MatrixValue(matrix.Type, matrix.Rows, matrix.Columns, matrix.Values.Map(x => x.Select(mapper).ToArray()));
            throw new InvalidOperationException();
        }

        private static HLSLRegister<T> Map2Registers<T>(HLSLRegister<T> left, HLSLRegister<T> right, Func<T, T, T> mapper)
        {
            if (left.IsVarying && right.IsVarying)
            {
                T[] mapped = left.VaryingValues.Zip(right.VaryingValues, mapper).ToArray();
                return new HLSLRegister<T>(mapped);
            }
            else if (!left.IsVarying && !right.IsVarying)
            {
                return new HLSLRegister<T>(mapper(left.UniformValue, right.UniformValue));
            }
            else
            {
                throw new InvalidOperationException("Cannot map varying and uniform register together.");
            }
        }

        public static NumericValue Map2(NumericValue left, NumericValue right, Func<object, object, object> mapper)
        {

            if (GetTensorSize(left) != GetTensorSize(right))
                throw new ArgumentException("Sizes of operands must match.");
            if (left is ScalarValue scalarLeft && right is ScalarValue scalarRight)
                return new ScalarValue(scalarLeft.Type, Map2Registers(scalarLeft.Value, scalarRight.Value, mapper));
            if (left is VectorValue vectorLeft && right is VectorValue vectorRight)
            {
                var mapped = Map2Registers(vectorLeft.Values, vectorRight.Values, (x, y) =>
                {
                    object[] result = new object[vectorLeft.Size];
                    for (int i = 0; i < result.Length; i++)
                        result[i] = mapper(x[i], y[i]);
                    return result;
                });
                return new VectorValue(vectorLeft.Type, mapped);
            }
            if (left is MatrixValue matrixLeft && right is MatrixValue matrixRight)
            {
                var mapped = Map2Registers(matrixLeft.Values, matrixRight.Values, (x, y) =>
                {
                    object[] result = new object[x.Length];
                    for (int i = 0; i < result.Length; i++)
                        result[i] = mapper(x[i], y[i]);
                    return result;
                });
                return new MatrixValue(matrixLeft.Type, matrixLeft.Rows, matrixLeft.Columns, mapped);
            }
            throw new InvalidOperationException();
        }

        public static NumericValue MapThreads(NumericValue value, Func<object, int, object> mapper)
        {
            if (value is ScalarValue scalar)
                return new ScalarValue(scalar.Type, scalar.Value.MapThreads(mapper));
            if (value is VectorValue vector)
                return new VectorValue(vector.Type, vector.Values.MapThreads((a, b) => (object[])mapper(a, b)));
            if (value is MatrixValue matrix)
                return new MatrixValue(matrix.Type, matrix.Rows, matrix.Columns, matrix.Values.MapThreads((a, b) => (object[])mapper(a, b)));
            throw new InvalidOperationException();
        }

        public static HLSLRegister<object> MakeScalarSGPR<T>(T val)
        {
            return new HLSLRegister<object>(val);
        }

        public static HLSLRegister<object> MakeScalarVGPR<T>(IEnumerable<T> val)
        {
            return new HLSLRegister<object>(val.Select(x => (object)x).ToArray());
        }

        public static HLSLRegister<object[]> MakeVectorSGPR<T>(IEnumerable<T> val)
        {
            return new HLSLRegister<object[]>(val.Select(x => (object)x).ToArray());
        }

        public static HLSLRegister<object[]> MakeVectorVGPR<T>(IEnumerable<IEnumerable<T>> val)
        {
            return new HLSLRegister<object[]>(val.Select(x => x.Select(y => (object)y).ToArray()).ToArray());
        }
    }
}
