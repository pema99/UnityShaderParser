using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
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

        public HLSLRegister<T> Copy()
        {
            if (IsVarying)
            {
                T[] input = VaryingValues;
                return new HLSLRegister<T>(input.ToArray());
            }
            else
            {
                return new HLSLRegister<T>(UniformValue);
            }
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
        public abstract int ThreadCount { get; }
        public abstract bool IsUniform { get; }
        public bool IsVarying => !IsUniform;

        public abstract HLSLValue Copy();
    }
    
    // Reference to another value, i.e. refcell (in/inout)
    public class ReferenceValue : HLSLValue
    {
        public readonly Func<HLSLValue> Get;
        public readonly Action<HLSLValue> Set;

        public override int ThreadCount => Get().ThreadCount;
        public override bool IsUniform => Get().IsUniform;

        public override HLSLValue Copy()
        {
            return new ReferenceValue(Get, Set);
        }

        public ReferenceValue(Func<HLSLValue> get, Action<HLSLValue> set)
        {
            Get = get;
            Set = set;
        }

        public override string ToString() => $"Ref({Get()})";
    }

    public abstract class NumericValue : HLSLValue
    {
        public readonly ScalarType Type;

        public NumericValue(ScalarType type)
        {
            Type = type;
        }

        public abstract (int rows, int columns) TensorSize { get; }

        public abstract VectorValue BroadcastToVector(int size);
        public abstract MatrixValue BroadcastToMatrix(int rows, int columns);
        public abstract NumericValue Cast(ScalarType type);
        public abstract object GetThreadValue(int threadIndex);
        public abstract NumericValue SetThreadValue(int threadIndex, object set);
        public abstract NumericValue Map(Func<object, object> mapper);
        public abstract NumericValue MapThreads(Func<object, int, object> mapper);
        public abstract ScalarValue[] ToScalars();

        public abstract NumericValue Vectorize(int threadCount);
        public abstract NumericValue Scalarize(int threadIndex);

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

        public static implicit operator NumericValue(int v) => (ScalarValue)v;
        public static implicit operator NumericValue(uint v) => (ScalarValue)v;
        public static implicit operator NumericValue(float v) => (ScalarValue)v;
        public static implicit operator NumericValue(double v) => (ScalarValue)v;
        public static implicit operator NumericValue(bool v) => (ScalarValue)v;
        public static implicit operator NumericValue(char v) => (ScalarValue)v;
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

        public override int ThreadCount => Members.Max(x => x.Value.ThreadCount);
        public override bool IsUniform => Members.All(x => x.Value.IsUniform);

        public override HLSLValue Copy()
        {
            Dictionary<string, HLSLValue> members = new Dictionary<string, HLSLValue>();
            foreach (KeyValuePair<string, HLSLValue> member in Members)
            {
                members.Add(member.Key, member.Value.Copy());
            }
            return new StructValue(Name, members);
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

        public ScalarValue(ScalarType type, HLSLRegister<object> value)
            : base(type)
        {
            Value = value;
        }

        public override (int rows, int columns) TensorSize => (1, 1);
        public override int ThreadCount => Value.Size;
        public override bool IsUniform => Value.IsUniform;

        public override ScalarValue[] ToScalars()
        {
            return new ScalarValue[] { (ScalarValue)Copy() };
        }

        public override HLSLValue Copy()
        {
            return new ScalarValue(Type, Value.Copy());
        }

        public override MatrixValue BroadcastToMatrix(int rows, int columns)
        {
            return new MatrixValue(Type, rows, columns, Value.Map(x =>
            {
                object[] res = new object[rows * columns];
                Array.Fill(res, x);
                return res;
            }));
        }

        public override VectorValue BroadcastToVector(int size)
        {
            return new VectorValue(Type, Value.Map(x =>
            {
                object[] res = new object[size];
                Array.Fill(res, x);
                return res;
            }));
        }

        public override NumericValue Cast(ScalarType type)
        {
            return new ScalarValue(type, Value.Map(x => HLSLValueUtils.CastNumeric(type, x)));
        }

        public override object GetThreadValue(int threadIndex)
        {
            return Value.Get(threadIndex);
        }

        public override NumericValue Map(Func<object, object> mapper)
        {
            return new ScalarValue(Type, Value.Map(mapper));
        }

        public override NumericValue MapThreads(Func<object, int, object> mapper)
        {
            return new ScalarValue(Type, Value.MapThreads(mapper));
        }

        public override NumericValue Scalarize(int threadIndex)
        {
            return new ScalarValue(Type, Value.Scalarize(threadIndex));
        }

        public override NumericValue Vectorize(int threadCount)
        {
            return new ScalarValue(Type, Value.Vectorize(threadCount));
        }

        public override NumericValue SetThreadValue(int threadIndex, object set)
        {
            return new ScalarValue(Type, Value.Set(threadIndex, set));
        }

        public override string ToString()
        {
            if (Value.IsVarying)
            {
                string[] vals = new string[Value.VaryingValues.Length];
                for (int i = 0; i < vals.Length; i++)
                    vals[i] = $"{ToString(i)}";
                return $"Varying({string.Join(", ", vals)})";
            }
            else
            {
                return ToString(0);
            }
        }
        public string ToString(int threadIndex) => Convert.ToString(Value.Get(threadIndex), CultureInfo.InvariantCulture);

        public static ScalarValue operator +(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left + (NumericValue)right);
        public static ScalarValue operator -(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left - (NumericValue)right);
        public static ScalarValue operator *(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left * (NumericValue)right);
        public static ScalarValue operator /(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left / (NumericValue)right);
        public static ScalarValue operator %(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left % (NumericValue)right);
        public static ScalarValue operator <(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left < (NumericValue)right);
        public static ScalarValue operator >(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left > (NumericValue)right);
        public static ScalarValue operator <=(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left <= (NumericValue)right);
        public static ScalarValue operator >=(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left >= (NumericValue)right);
        public static ScalarValue operator ==(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left == (NumericValue)right);
        public static ScalarValue operator !=(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left != (NumericValue)right);
        public static ScalarValue operator ^(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left ^ (NumericValue)right);
        public static ScalarValue operator |(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left | (NumericValue)right);
        public static ScalarValue operator &(ScalarValue left, ScalarValue right) => (ScalarValue)((NumericValue)left & (NumericValue)right);
        public static ScalarValue operator ~(ScalarValue left) => (ScalarValue)(~(NumericValue)left);
        public static ScalarValue operator !(ScalarValue left) => (ScalarValue)(!(NumericValue)left);
        public static ScalarValue operator -(ScalarValue left) => (ScalarValue)(-(NumericValue)left);

        public static implicit operator ScalarValue(int v) => new ScalarValue(ScalarType.Int, new HLSLRegister<object>(v));
        public static implicit operator ScalarValue(uint v) => new ScalarValue(ScalarType.Uint, new HLSLRegister<object>(v));
        public static implicit operator ScalarValue(float v) => new ScalarValue(ScalarType.Float, new HLSLRegister<object>(v));
        public static implicit operator ScalarValue(double v) => new ScalarValue(ScalarType.Double, new HLSLRegister<object>(v));
        public static implicit operator ScalarValue(bool v) => new ScalarValue(ScalarType.Bool, new HLSLRegister<object>(v));
        public static implicit operator ScalarValue(char v) => new ScalarValue(ScalarType.Char, new HLSLRegister<object>(v));

        public static ScalarValue Null => new ScalarValue(ScalarType.Void, new HLSLRegister<object>(0));
    }

    public sealed class VectorValue : NumericValue
    {
        public readonly HLSLRegister<object[]> Values;

        public VectorValue(ScalarType type, HLSLRegister<object[]> values)
            : base(type)
        {
            Values = values;
        }

        public int Size => Values.Get(0).Length;
        public override (int rows, int columns) TensorSize => (Size, 1);
        public override int ThreadCount => Values.Size;
        public override bool IsUniform => Values.IsUniform;

        public ScalarValue this[int channel]
        {
            get
            {
                if (IsVarying)
                {
                    object[] perThreadValue = new object[ThreadCount];
                    for (int threadIndex = 0; threadIndex < ThreadCount; threadIndex++)
                        perThreadValue[threadIndex] = Values.Get(threadIndex)[channel];
                    return new ScalarValue(Type, HLSLValueUtils.MakeScalarVGPR(perThreadValue));
                }
                else
                {
                    return new ScalarValue(Type, HLSLValueUtils.MakeScalarSGPR(Values.UniformValue[channel]));
                }
            }
        }
        public ScalarValue x => this[0];
        public ScalarValue y => this[1];
        public ScalarValue z => this[2];
        public ScalarValue w => this[3];

        public NumericValue Swizzle(string swizzle)
        {
            object[][] perThreadSwizzle = new object[ThreadCount][];
            for (int threadIndex = 0; threadIndex < perThreadSwizzle.Length; threadIndex++)
            {
                perThreadSwizzle[threadIndex] = new object[swizzle.Length];
                for (int component = 0; component < swizzle.Length; component++)
                {
                    switch (swizzle[component])
                    {
                        case 'r':
                        case 'x':
                        case 's':
                            perThreadSwizzle[threadIndex][component] = Values.Get(threadIndex)[0];
                            break;
                        case 'g':
                        case 'y':
                        case 't':
                            perThreadSwizzle[threadIndex][component] = Values.Get(threadIndex)[1];
                            break;
                        case 'b':
                        case 'z':
                        case 'p':
                            perThreadSwizzle[threadIndex][component] = Values.Get(threadIndex)[2];
                            break;
                        case 'a':
                        case 'w':
                        case 'q':
                            perThreadSwizzle[threadIndex][component] = Values.Get(threadIndex)[3];
                            break;
                    }
                }
            }
            if (ThreadCount == 1)
            {
                if (swizzle.Length == 1)
                    return new ScalarValue(Type, HLSLValueUtils.MakeScalarSGPR(perThreadSwizzle[0][0]));
                else
                    return new VectorValue(Type, HLSLValueUtils.MakeVectorSGPR(perThreadSwizzle[0]));
            }
            else
            {
                if (swizzle.Length == 1)
                    return new ScalarValue(Type, HLSLValueUtils.MakeScalarVGPR(perThreadSwizzle.Select(x => x[0])));
                else
                    return new VectorValue(Type, HLSLValueUtils.MakeVectorVGPR(perThreadSwizzle));
            }
        }

        public VectorValue SwizzleAssign(string swizzle, NumericValue value)
        {
            object GetComponent(object arrayOrSingle, int index)
            {
                if (arrayOrSingle is object[] array)
                    return array[index];
                return arrayOrSingle;
            }

            int maxThreadCount = Math.Max(ThreadCount, value.ThreadCount);
            object[][] perThreadSwizzle = new object[maxThreadCount][];
            for (int threadIndex = 0; threadIndex < perThreadSwizzle.Length; threadIndex++)
            {
                // Write current values
                perThreadSwizzle[threadIndex] = new object[Size];
                for (int component = 0; component < Size; component++)
                {
                    perThreadSwizzle[threadIndex][component] = Values.Get(threadIndex)[component];
                }

                // Splat swizzle assign
                for (int component = 0; component < swizzle.Length; component++)
                {
                    switch (swizzle[component])
                    {
                        case 'r':
                        case 'x':
                        case 's':
                            perThreadSwizzle[threadIndex][0] = GetComponent(value.GetThreadValue(threadIndex), component);
                            break;
                        case 'g':
                        case 'y':
                        case 't':
                            perThreadSwizzle[threadIndex][1] = GetComponent(value.GetThreadValue(threadIndex), component);
                            break;
                        case 'b':
                        case 'z':
                        case 'p':
                            perThreadSwizzle[threadIndex][2] = GetComponent(value.GetThreadValue(threadIndex), component);
                            break;
                        case 'a':
                        case 'w':
                        case 'q':
                            perThreadSwizzle[threadIndex][3] = GetComponent(value.GetThreadValue(threadIndex), component);
                            break;
                    }
                }
            }
            if (maxThreadCount == 1)
            {
                return new VectorValue(Type, HLSLValueUtils.MakeVectorSGPR(perThreadSwizzle[0]));
            }
            else
            {
                return new VectorValue(Type, HLSLValueUtils.MakeVectorVGPR(perThreadSwizzle));
            }
        }

        public static VectorValue FromScalars(params ScalarValue[] scalars)
        {
            return new VectorValue(scalars[0].Type, HLSLValueUtils.RegisterFromScalars(scalars));
        }

        public override ScalarValue[] ToScalars()
        {
            return HLSLValueUtils.RegisterToScalars(Type, Values);
        }  

        public override HLSLValue Copy()
        {
            return new VectorValue(Type, Values.Copy());
        }

        public override MatrixValue BroadcastToMatrix(int rows, int columns)
        {
            throw new InvalidOperationException();
        }

        public override VectorValue BroadcastToVector(int size)
        {
            return new VectorValue(Type, Values.Map(x =>
            {
                int sizeDiff = size - x.Length;
                if (sizeDiff > 0) // Expansion
                {
                    object[] res = new object[size];
                    Array.Copy(x, res, x.Length);
                    for (int i = 0; i < sizeDiff; i++)
                        res[x.Length + i] = HLSLValueUtils.GetZeroValue(Type);
                    return res;
                }
                else if (size < x.Length) // Truncation
                {
                    object[] res = new object[size];
                    Array.Copy(x, res, size);
                    return res;
                }
                else
                    return x;
            }));
        }

        public override NumericValue Cast(ScalarType type)
        {
            return new VectorValue(type, Values.Map(x =>
            {
                object[] res = new object[x.Length];
                for (int i = 0; i < res.Length; i++)
                    res[i] = HLSLValueUtils.CastNumeric(type, x[i]);
                return res;
            }));
        }

        public override object GetThreadValue(int threadIndex)
        {
            return Values.Get(threadIndex);
        }

        public override NumericValue Map(Func<object, object> mapper)
        {
            return new VectorValue(Type, Values.Map(x =>
            {
                object[] res = new object[x.Length];
                for (int i = 0; i < res.Length; i++)
                    res[i] = mapper(x[i]);
                return res;
            }));
        }

        public override NumericValue MapThreads(Func<object, int, object> mapper)
        {
            return new VectorValue(Type, Values.MapThreads((a, b) => (object[])mapper(a, b)));
        }

        public override NumericValue Scalarize(int threadIndex)
        {
            return new VectorValue(Type, Values.Scalarize(threadIndex));
        }

        public override NumericValue Vectorize(int threadCount)
        {
            return new VectorValue(Type, Values.Vectorize(threadCount));
        }

        public override NumericValue SetThreadValue(int threadIndex, object set)
        {
            return new VectorValue(Type, Values.Set(threadIndex, (object[])set));
        }

        public override string ToString()
        {
            if (Values.IsVarying)
            {
                string[] vals = new string[Values.VaryingValues.Length];
                for (int i = 0; i < vals.Length; i++)
                    vals[i] = $"{ToString(i)}";
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
            return $"{type}{Size}({string.Join(", ", Values.Get(threadIndex).Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)))})";
        }

        public static VectorValue operator +(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left + (NumericValue)right);
        public static VectorValue operator -(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left - (NumericValue)right);
        public static VectorValue operator *(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left * (NumericValue)right);
        public static VectorValue operator /(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left / (NumericValue)right);
        public static VectorValue operator %(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left % (NumericValue)right);
        public static VectorValue operator <(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left < (NumericValue)right);
        public static VectorValue operator >(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left > (NumericValue)right);
        public static VectorValue operator <=(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left <= (NumericValue)right);
        public static VectorValue operator >=(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left >= (NumericValue)right);
        public static VectorValue operator ==(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left == (NumericValue)right);
        public static VectorValue operator !=(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left != (NumericValue)right);
        public static VectorValue operator ^(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left ^ (NumericValue)right);
        public static VectorValue operator |(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left | (NumericValue)right);
        public static VectorValue operator &(VectorValue left, VectorValue right) => (VectorValue)((NumericValue)left & (NumericValue)right);
        public static VectorValue operator ~(VectorValue left) => (VectorValue)(~(NumericValue)left);
        public static VectorValue operator !(VectorValue left) => (VectorValue)(!(NumericValue)left);
        public static VectorValue operator -(VectorValue left) => (VectorValue)(-(NumericValue)left);
    }

    public sealed class MatrixValue : NumericValue
    {
        public readonly int Rows;
        public readonly int Columns;
        public readonly HLSLRegister<object[]> Values;

        public MatrixValue(ScalarType type, int rows, int columns, HLSLRegister<object[]> values)
            : base(type)
        {
            Rows = rows;
            Columns = columns;
            Values = values;
        }

        public override (int rows, int columns) TensorSize => (Rows, Columns);
        public override int ThreadCount => Values.Size;
        public override bool IsUniform => Values.IsUniform;

        public ScalarValue this[int channel]
        {
            get
            {
                if (IsVarying)
                {
                    object[] perThreadValue = new object[ThreadCount];
                    for (int threadIndex = 0; threadIndex < ThreadCount; threadIndex++)
                        perThreadValue[threadIndex] = Values.Get(threadIndex)[channel];
                    return new ScalarValue(Type, HLSLValueUtils.MakeScalarVGPR(perThreadValue));
                }
                else
                {
                    return new ScalarValue(Type, HLSLValueUtils.MakeScalarSGPR(Values.UniformValue[channel]));
                }
            }
        }

        public ScalarValue this[int row, int col] => this[row * Columns + col];

        public static MatrixValue FromScalars(int rows, int columns, params ScalarValue[] scalars)
        {
            return new MatrixValue(scalars[0].Type, rows, columns, HLSLValueUtils.RegisterFromScalars(scalars));
        }

        public override ScalarValue[] ToScalars()
        {
            return HLSLValueUtils.RegisterToScalars(Type, Values);
        }

        public override HLSLValue Copy()
        {
            return new MatrixValue(Type, Rows, Columns, Values.Copy());
        }

        public override MatrixValue BroadcastToMatrix(int rows, int columns)
        {
            if (Rows != rows || Columns != columns)
            {
                var scalars = ToScalars();
                ScalarValue[] newScalars = new ScalarValue[rows * columns];
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col  < columns; col++)
                    {
                        if (row < Rows && col < Columns)
                            newScalars[row * columns + col] = scalars[row * Columns + col];
                        else
                            newScalars[row * columns + col] = (ScalarValue)HLSLValueUtils.GetZeroValue(scalars[0]);
                    }
                }
                return FromScalars(rows, columns, newScalars);
            }
            return (MatrixValue)Copy();
        }

        public override VectorValue BroadcastToVector(int size)
        {
            throw new InvalidOperationException();
        }

        public override NumericValue Cast(ScalarType type)
        {
            return new MatrixValue(type, Rows, Columns, Values.Map(x =>
            {
                object[] res = new object[x.Length];
                for (int i = 0; i < res.Length; i++)
                    res[i] = HLSLValueUtils.CastNumeric(type, x[i]);
                return res;
            }));
        }

        public override object GetThreadValue(int threadIndex)
        {
            return Values.Get(threadIndex);
        }

        public override NumericValue Map(Func<object, object> mapper)
        {
            return new MatrixValue(Type, Rows, Columns, Values.Map(x =>
            {
                object[] res = new object[x.Length];
                for (int i = 0; i < res.Length; i++)
                    res[i] = mapper(x[i]);
                return res;
            }));
        }

        public override NumericValue MapThreads(Func<object, int, object> mapper)
        {
            return new MatrixValue(Type, Rows, Columns, Values.MapThreads((a, b) => (object[])mapper(a, b)));
        }

        public override NumericValue Scalarize(int threadIndex)
        {
            return new MatrixValue(Type, Rows, Columns, Values.Scalarize(threadIndex));
        }

        public override NumericValue Vectorize(int threadCount)
        {
            return new MatrixValue(Type, Rows, Columns, Values.Vectorize(threadCount));
        }

        public override NumericValue SetThreadValue(int threadIndex, object set)
        {
            return new MatrixValue(Type, Rows, Columns, Values.Set(threadIndex, (object[])set));
        }

        public override string ToString()
        {
            if (Values.IsVarying)
            {
                string[] vals = new string[Values.VaryingValues.Length];
                for (int i = 0; i < vals.Length; i++)
                    vals[i] = $"{ToString(i)}";
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
            return $"{type}{Rows}x{Columns}({string.Join(", ", Values.Get(threadIndex).Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)))})";
        }

        public static MatrixValue operator +(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left + (NumericValue)right);
        public static MatrixValue operator -(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left - (NumericValue)right);
        public static MatrixValue operator *(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left * (NumericValue)right);
        public static MatrixValue operator /(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left / (NumericValue)right);
        public static MatrixValue operator %(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left % (NumericValue)right);
        public static MatrixValue operator <(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left < (NumericValue)right);
        public static MatrixValue operator >(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left > (NumericValue)right);
        public static MatrixValue operator <=(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left <= (NumericValue)right);
        public static MatrixValue operator >=(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left >= (NumericValue)right);
        public static MatrixValue operator ==(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left == (NumericValue)right);
        public static MatrixValue operator !=(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left != (NumericValue)right);
        public static MatrixValue operator ^(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left ^ (NumericValue)right);
        public static MatrixValue operator |(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left | (NumericValue)right);
        public static MatrixValue operator &(MatrixValue left, MatrixValue right) => (MatrixValue)((NumericValue)left & (NumericValue)right);
        public static MatrixValue operator ~(MatrixValue left) => (MatrixValue)(~(NumericValue)left);
        public static MatrixValue operator !(MatrixValue left) => (MatrixValue)(!(NumericValue)left);
        public static MatrixValue operator -(MatrixValue left) => (MatrixValue)(-(NumericValue)left);
    }

    public sealed class PredefinedObjectValue : HLSLValue
    {
        public readonly PredefinedObjectType Type;
        public readonly TypeNode[] TemplateArguments;

        public PredefinedObjectValue(PredefinedObjectType type, TypeNode[] templateArguments)
        {
            Type = type;
            TemplateArguments = templateArguments;
        }
        public override int ThreadCount => 1;
        public override bool IsUniform => true;

        public override HLSLValue Copy()
        {
            return new PredefinedObjectValue(Type, TemplateArguments);
        }

        public override string ToString()
        {
            string type = PrintingUtil.GetEnumName(Type);
            return $"{type}<{string.Join(", ", (IEnumerable<TypeNode>)TemplateArguments)}>";
        }
    }

    public sealed class ArrayValue : HLSLValue
    {
        public readonly HLSLValue[] Values;

        public override int ThreadCount => Values.Max(x => x.ThreadCount);
        public override bool IsUniform => Values.All(x => x.IsUniform);

        public override HLSLValue Copy()
        {
            return new ArrayValue(Values.Select(x => x.Copy()).ToArray());
        }
        
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

        public static object GetOneValue(ScalarType type)
        {
            switch (type)
            {
                case ScalarType.Void:
                    return null;
                case ScalarType.Bool:
                    return true;
                case ScalarType.Int:
                case ScalarType.Min16Int:
                case ScalarType.Min12Int:
                    return 1;
                case ScalarType.Uint:
                case ScalarType.Min16Uint:
                case ScalarType.Min12Uint:
                    return 1u;
                case ScalarType.Half:
                case ScalarType.Float:
                case ScalarType.Min16Float:
                case ScalarType.Min10Float:
                case ScalarType.UNormFloat:
                case ScalarType.SNormFloat:
                    return 1.0f;
                case ScalarType.Double:
                    return 1.0;
                case ScalarType.String:
                    return string.Empty;
                case ScalarType.Char:
                    return (char)1;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static NumericValue GetZeroValue(NumericValue val)
        {
            return val.Map(x =>
            {
                return GetZeroValue(val.Type);
            });
        }

        public static NumericValue GetOneValue(NumericValue val)
        {
            return val.Map(x =>
            {
                return GetOneValue(val.Type);
            });
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
                case ScalarType.Min12Uint: return 1;
                case ScalarType.Min16Uint: return 2;
                case ScalarType.Uint: return 3;
                case ScalarType.Min12Int: return 4;
                case ScalarType.Min16Int: return 5;
                case ScalarType.Int: return 6;
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
                    else if (value is double di) return (int)di;
                    else if (value is uint ui) return (int)ui;
                    else return Convert.ToInt32(value);
                case ScalarType.Uint:
                case ScalarType.Min16Uint:
                case ScalarType.Min12Uint:
                    if (value is float fu) return (uint)fu;
                    else if (value is double du) return (uint)du;
                    else if (value is int iu) return (uint)iu;
                    else return Convert.ToUInt32(value);
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

        public static (NumericValue newLeft, NumericValue newRight) PromoteThreadCount(NumericValue left, NumericValue right)
        {
            int leftThreadCount = left.ThreadCount;
            int rightThreadCount = right.ThreadCount;
            if (leftThreadCount < rightThreadCount)
                left = left.Vectorize(rightThreadCount);
            else if (rightThreadCount < leftThreadCount)
                right = right.Vectorize(leftThreadCount);

            return (left, right);
        }

        public static (NumericValue newLeft, NumericValue newRight) PromoteShape(NumericValue left, NumericValue right)
        {
            bool needMatrix = left is MatrixValue || right is MatrixValue;
            bool needVector = left is VectorValue || right is VectorValue;

            if (needMatrix)
            {
                (int leftRows, int leftColumns) = left.TensorSize;
                (int rightRows, int rightColumns) = right.TensorSize;
                int newRows = Math.Max(leftRows, rightRows);
                int newColumns = Math.Max(leftColumns, rightColumns);
                if (leftRows != newRows || leftColumns != newColumns)
                    left = left.BroadcastToMatrix(newRows, newColumns);
                if (rightRows != newRows || rightColumns != newColumns)
                    right = right.BroadcastToMatrix(newRows, newColumns);
            }

            else if (needVector)
            {
                (int leftSize, _) = left.TensorSize;
                (int rightSize, _) = right.TensorSize;
                int newSize = Math.Max(leftSize, rightSize);
                if (leftSize != newSize)
                    left = left.BroadcastToVector(newSize);
                if (rightSize != newSize)
                    right = right.BroadcastToVector(newSize);
            }

            return (left, right);
        }

        public static (NumericValue newLeft, NumericValue newRight) PromoteType(NumericValue left, NumericValue right, bool bitwiseOp)
        {
            ScalarType type = bitwiseOp
                ? PromoteForBitwiseBinOp(left.Type, right.Type)
                : PromoteScalarType(left.Type, right.Type);

            return (left.Cast(type), right.Cast(type));
        }

        // Given 2 params, promote such that no information is lost.
        // This includes promoting SGPR to VGPR.
        public static (NumericValue newLeft, NumericValue newRight) Promote(NumericValue left, NumericValue right, bool bitwiseOp)
        {
            (left, right) = PromoteThreadCount(left, right);

            // Fast path
            if (left.Type == right.Type && left.GetType() == right.GetType())
                return (left, right);

            (left, right) = PromoteShape(left, right);

            return PromoteType(left, right, bitwiseOp);
        }

        // Cast "right" to match the type of "left" and return it.
        // Performs any implicit conversions, either promotion or demotion, needed for an assignment,
        public static HLSLValue CastForAssignment(HLSLValue left, HLSLValue right)
        {
            if (left is NumericValue leftNum && right is NumericValue rightNum)
            {
                int leftThreadCount = leftNum.ThreadCount;
                int rightThreadCount = rightNum.ThreadCount;
                if (leftThreadCount < rightThreadCount)
                    leftNum = leftNum.Vectorize(rightThreadCount);
                else if (rightThreadCount < leftThreadCount)
                    rightNum = rightNum.Vectorize(leftThreadCount);

                ScalarType type = leftNum.Type;

                bool needMatrix = leftNum is MatrixValue;
                bool needVector = leftNum is VectorValue;

                if (needMatrix)
                {
                    (int newRows, int newColumns) = leftNum.TensorSize;
                    var resizedRight = rightNum.BroadcastToMatrix(newRows, newColumns);
                    return resizedRight.Cast(type);
                }

                if (needVector)
                {
                    int newSize = leftNum.TensorSize.rows;
                    var resizedRight = rightNum.BroadcastToVector(newSize);
                    return resizedRight.Cast(type);
                }

                return rightNum.Cast(type);
            }

            return right;
        }

        private static HLSLRegister<T> Map2Registers<T>(HLSLRegister<T> left, HLSLRegister<T> right, Func<T, T, T> mapper)
        {
            if (left.IsVarying && right.IsVarying)
            {
                T[] mapped = new T[left.VaryingValues.Length];
                for (int i = 0; i < mapped.Length; i++)
                    mapped[i] = mapper(left.VaryingValues[i], right.VaryingValues[i]);
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
            if (left.TensorSize != right.TensorSize)
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

        public static HLSLValue Scalarize(HLSLValue value, int threadIndex)
        {
            switch (value)
            {
                case NumericValue num:
                    return num.Scalarize(threadIndex);
                case StructValue str:
                    Dictionary<string, HLSLValue> members = new Dictionary<string, HLSLValue>();
                    foreach (var kvp in members)
                        members.Add(kvp.Key, Scalarize(kvp.Value, threadIndex));
                    return new StructValue(str.Name, members);
                case ArrayValue arr:
                    HLSLValue[] vals = new HLSLValue[arr.Values.Length];
                    for (int i = 0; i < vals.Length; i++)
                        vals[i] = Scalarize(arr.Values[i], threadIndex);
                    return new ArrayValue(vals);
                default:
                    throw new InvalidOperationException();
            }
        }

        public static HLSLValue Vectorize(HLSLValue value, int threadCount)
        {
            switch (value)
            {
                case NumericValue num:
                    return num.Vectorize(threadCount);
                case StructValue str:
                    Dictionary<string, HLSLValue> members = new Dictionary<string, HLSLValue>();
                    foreach (var kvp in members)
                        members.Add(kvp.Key, Vectorize(kvp.Value, threadCount));
                    return new StructValue(str.Name, members);
                case ArrayValue arr:
                    HLSLValue[] vals = new HLSLValue[arr.Values.Length];
                    for (int i = 0; i < vals.Length; i++)
                        vals[i] = Vectorize(arr.Values[i], threadCount);
                    return new ArrayValue(vals);
                default:
                    throw new InvalidOperationException();
            }
        }

        public static HLSLValue SetThreadValue(HLSLValue allValue, int threadIndex, HLSLValue threadValue)
        {
            if (allValue is NumericValue numLeft && threadValue is NumericValue numRight)
            {
                (numLeft, numRight) = Promote(numLeft, numRight, false);
                return numLeft.SetThreadValue(threadIndex, numRight.GetThreadValue(threadIndex));
            }

            if (allValue is StructValue strLeft && threadValue is StructValue strRight)
            {
                Dictionary<string, HLSLValue> members = new Dictionary<string, HLSLValue>();
                foreach (var kvp in strLeft.Members)
                {
                    if (strRight.Members.TryGetValue(kvp.Key, out var rightV))
                        members.Add(kvp.Key, SetThreadValue(kvp.Value, threadIndex, rightV));
                }
                return new StructValue(strLeft.Name, members);
            }

            if (allValue is ArrayValue arrLeft && threadValue is ArrayValue arrRight)
            {
                HLSLValue[] vals = new HLSLValue[arrLeft.Values.Length];
                for (int i = 0; i < vals.Length; i++)
                    vals[i] = SetThreadValue(arrLeft.Values[i], threadIndex, arrRight.Values[i]);
                return new ArrayValue(vals);
            }

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

        public static HLSLRegister<object[]> RegisterFromScalars(ScalarValue[] scalars)
        {
            ScalarType type = scalars[0].Type;
            foreach (var scalar in scalars)
            {
                type = HLSLValueUtils.PromoteScalarType(type, scalar.Type);
            }

            int maxThreadCount = scalars.Max(x => x.ThreadCount);
            object[][] result = new object[maxThreadCount][];
            for (int threadIndex = 0; threadIndex < maxThreadCount; threadIndex++)
            {
                result[threadIndex] = new object[scalars.Length];
                for (int channel = 0; channel < scalars.Length; channel++)
                {
                    var scalar = scalars[channel];
                    if (scalar.Type != type)
                        scalar = (ScalarValue)scalar.Cast(type);
                    result[threadIndex][channel] = scalar.GetThreadValue(threadIndex);
                }
            }

            if (maxThreadCount == 1)
                return MakeVectorSGPR(result[0]);
            else
                return MakeVectorVGPR(result);
        }

        public static ScalarValue[] RegisterToScalars(ScalarType type, HLSLRegister<object[]> scalars)
        {
            ScalarValue[] scalarValues = new ScalarValue[scalars.Get(0).Length];
            if (scalars.IsUniform)
            {
                for (int i = 0; i < scalarValues.Length; i++)
                {
                    scalarValues[i] = new ScalarValue(type, HLSLValueUtils.MakeScalarSGPR(scalars.UniformValue[i]));
                }
            }
            else
            {
                for (int i = 0; i < scalarValues.Length; i++)
                {
                    object[] perThreadValues = new object[scalars.Size];
                    for (int threadIndex = 0; threadIndex < scalars.Size; threadIndex++)
                    {
                        perThreadValues[threadIndex] = scalars.VaryingValues[threadIndex][i];
                    }
                    scalarValues[i] = new ScalarValue(type, HLSLValueUtils.MakeScalarVGPR(perThreadValues));
                }
            }
            return scalarValues;
        }

        // Cast "value" to match the given type and return it.
        // Performs any implicit conversions, either promotion or demotion, needed to pass as a function parameter.
        public static HLSLValue CastForParameter(HLSLExpressionEvaluator evaluator, HLSLValue value, TypeNode typeNode)
        {
            if (value is NumericValue valueNum && typeNode is NumericTypeNode typeNum)
            {
                HLSLValue reshaped = valueNum;
                if (typeNode is MatrixTypeNode mat)
                    valueNum = valueNum.BroadcastToMatrix(mat.FirstDimension, mat.SecondDimension);

                if (typeNode is VectorTypeNode vec)
                    valueNum = valueNum.BroadcastToVector(vec.Dimension);

                if (typeNode is GenericMatrixTypeNode matGen)
                {
                    int rows = Convert.ToInt32(((ScalarValue)evaluator.Visit(matGen.FirstDimension)).Cast(ScalarType.Int).GetThreadValue(0));
                    int cols = Convert.ToInt32(((ScalarValue)evaluator.Visit(matGen.SecondDimension)).Cast(ScalarType.Int).GetThreadValue(0));
                    valueNum = valueNum.BroadcastToMatrix(rows, cols);
                }

                if (typeNode is GenericVectorTypeNode vecGen)
                    valueNum = valueNum.BroadcastToVector(Convert.ToInt32(((ScalarValue)evaluator.Visit(vecGen.Dimension)).Cast(ScalarType.Int).GetThreadValue(0)));

                return valueNum.Cast(typeNum.Kind);
            }
            return value;
        }

        public static bool TypeEquals(HLSLExpressionEvaluator evaluator, HLSLValue from, TypeNode to, IList<ArrayRankNode> arrayRanks = null)
        {
            if (to is ScalarTypeNode scalarType &&
                from is ScalarValue scalarValue &&
                scalarType.Kind == scalarValue.Type)
                return true;
            if (to is VectorTypeNode vecType &&
                from is VectorValue vecValue &&
                vecType.Kind == vecValue.Type &&
                vecType.Dimension == vecValue.Size)
                return true;
            if (to is GenericVectorTypeNode genVecType &&
                from is VectorValue genVecValue &&
                genVecType.Kind == genVecValue.Type &&
                Convert.ToInt32(((ScalarValue)evaluator.Visit(genVecType.Dimension)).Cast(ScalarType.Int).GetThreadValue(0)) == genVecValue.Size)
                return true;
            if (to is MatrixTypeNode matType
                && from is MatrixValue matValue
                && matType.Kind == matValue.Type
                && matType.FirstDimension == matValue.Rows
                && matType.SecondDimension == matValue.Columns)
                return true;
            if (to is GenericMatrixTypeNode genMatType
                && from is MatrixValue genMatValue
                && genMatType.Kind == genMatValue.Type
                && Convert.ToInt32(((ScalarValue)evaluator.Visit(genMatType.FirstDimension)).Cast(ScalarType.Int).GetThreadValue(0)) == genMatValue.Rows
                && Convert.ToInt32(((ScalarValue)evaluator.Visit(genMatType.SecondDimension)).Cast(ScalarType.Int).GetThreadValue(0)) == genMatValue.Columns)
                return true;
            if (to is StructTypeNode strType &&
                from is StructValue strValue &&
                strType.Name.GetName() == strValue.Name)
                return true;
            if (to is NamedTypeNode namedType &&
                from is StructValue namedStrValue &&
                namedType.GetName()== namedStrValue.Name)
                return true;
            if (to is QualifiedNamedTypeNode qualNamedType &&
                from is StructValue qualNamedStrValue &&
               qualNamedType.GetName() == qualNamedStrValue.Name)
                return true;
            if (to is PredefinedObjectTypeNode preType &&
                from is PredefinedObjectValue preValue &&
                preType.Kind == preValue.Type &&
                preType.TemplateArguments?.Count == preValue.TemplateArguments?.Length)
            {
                for (int i = 0; i < preType.TemplateArguments?.Count; i++)
                {
                    if (preValue.TemplateArguments[i].GetPrettyPrintedCode() != preType.TemplateArguments[i].GetPrettyPrintedCode())
                        return false;
                }
                return true;
            }
            if (arrayRanks != null &&
                from is ArrayValue arrValue &&
                arrValue.Values.Length > 0 &&
                arrayRanks.Count > 0 &&
                arrayRanks[0].Dimension is LiteralExpressionNode litDim &&
                int.Parse(litDim.Lexeme) == arrValue.Values.Length)
            {
                return TypeEquals(evaluator, arrValue.Values[0], to, arrayRanks.Skip(1).ToList());
            }
            if (from is ReferenceValue reference)
            {
                return TypeEquals(evaluator, reference.Get(), to);
            }

            return false;
        }

        // Can we convert a value to a type without loss of information?
        public static bool CanPromoteTo(HLSLExpressionEvaluator evaluator, HLSLValue from, TypeNode to)
        {
            if (from is NumericValue fromNum && to is NumericTypeNode toNum)
            {
                // Casting into a less informative type is not promotion
                if (GetScalarRank(fromNum.Type) > GetScalarRank(toNum.Kind))
                    return false;
                // Scalars broadcast
                if (fromNum is ScalarValue)
                    return true;
                // Vector extension
                if (fromNum is VectorValue fromVec && toNum is VectorTypeNode toVec)
                    return fromVec.Size <= toVec.Dimension;
                // Matrix extension
                if (fromNum is MatrixValue fromMat && toNum is MatrixTypeNode toMat)
                    return fromMat.Rows <= toMat.FirstDimension && fromMat.Columns <= toMat.SecondDimension;
                // Same but for generic
                if (fromNum is VectorValue fromVecGen && toNum is GenericVectorTypeNode toVecGen)
                    return fromVecGen.Size <= Convert.ToInt32(((ScalarValue)evaluator.Visit(toVecGen.Dimension)).Cast(ScalarType.Int).GetThreadValue(0));
                if (fromNum is MatrixValue fromMatGen && toNum is GenericMatrixTypeNode toMatGen)
                {
                    int rows = Convert.ToInt32(((ScalarValue)evaluator.Visit(toMatGen.FirstDimension)).Cast(ScalarType.Int).GetThreadValue(0));
                    int cols = Convert.ToInt32(((ScalarValue)evaluator.Visit(toMatGen.SecondDimension)).Cast(ScalarType.Int).GetThreadValue(0));
                    return fromMatGen.Rows <= rows && fromMatGen.Columns <= cols;
                }
            }
            return false;
        }

        // Can we convert a value to a type with loss of information?
        public static bool CanDemoteTo(HLSLExpressionEvaluator evaluator, HLSLValue from, TypeNode to)
        {
            if (from is NumericValue fromNum && to is NumericTypeNode toNum)
            {
                // Anything can be demoted to an scalar
                if (toNum is ScalarTypeNode)
                    return true;
                // If both are the same type, compare sizes - the from type must be strictly larger
                if (fromNum is VectorValue fromVec && toNum is VectorTypeNode toVec)
                    return fromVec.Size > toVec.Dimension;
                if (fromNum is MatrixValue fromMat && toNum is MatrixTypeNode toMat)
                    return fromMat.Rows > toMat.FirstDimension && fromMat.Columns > toMat.SecondDimension;
                if (fromNum is VectorValue fromVecGen && toNum is GenericVectorTypeNode toVecGen)
                    return fromVecGen.Size > Convert.ToInt32(((ScalarValue)evaluator.Visit(toVecGen.Dimension)).Cast(ScalarType.Int).GetThreadValue(0));
                if (fromNum is MatrixValue fromMatGen && toNum is GenericMatrixTypeNode toMatGen)
                {
                    int rows = Convert.ToInt32(((ScalarValue)evaluator.Visit(toMatGen.FirstDimension)).Cast(ScalarType.Int).GetThreadValue(0));
                    int cols = Convert.ToInt32(((ScalarValue)evaluator.Visit(toMatGen.SecondDimension)).Cast(ScalarType.Int).GetThreadValue(0));
                    return fromMatGen.Rows > rows && fromMatGen.Columns > cols;
                }
            }
            return false;
        }

        // Given a function and a list of parameters, evaluate how well the function matches the parameters
        public static int GetOverloadScore(HLSLExpressionEvaluator evaluator, FunctionDefinitionNode candidate, IList<HLSLValue> parameters)
        {
            if (parameters.Count != candidate.Parameters.Count)
                return -1;

            int score = 0;
            for (int i = 0; i < parameters.Count; i++)
            {
                var from = parameters[i];
                var to = candidate.Parameters[i].ParamType;
                if (TypeEquals(evaluator, from, to, candidate.Parameters[i].Declarator.ArrayRanks))
                    score += 3; // Exact match, best case
                else if (CanPromoteTo(evaluator, from, to))
                    score += 2; // Promotion is almost as good
                else if (CanDemoteTo(evaluator, from, to))
                    score += 1; // Demotion is a last resort
            }
            return score;
        }

        // Pick a function overload from a list of candidates based on the parameters. Returns null if no viable overload.
        public static FunctionDefinitionNode PickOverload(HLSLExpressionEvaluator evaluator, IEnumerable<FunctionDefinitionNode> candidates, IList<HLSLValue> parameters)
        {
            int bestScore = -1;
            FunctionDefinitionNode selected = null;
            foreach (var candidate in candidates)
            {
                int score = GetOverloadScore(evaluator, candidate, parameters);
                if (score > bestScore)
                {
                    bestScore = score;
                    selected = candidate;
                }
            }
            return selected;
        }
    }
}
