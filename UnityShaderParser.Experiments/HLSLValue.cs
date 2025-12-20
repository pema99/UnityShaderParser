using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityShaderParser.Common;
using UnityShaderParser.HLSL;

namespace UnityShaderParser.Test
{
    public abstract class HLSLValue
    {
    }

    public abstract class NumericValue : HLSLValue
    {
        public ScalarType Type;

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

        public static implicit operator NumericValue(int v) => new ScalarValue(ScalarType.Int, v);
        public static implicit operator NumericValue(uint v) => new ScalarValue(ScalarType.Uint, v);
        public static implicit operator NumericValue(float v) => new ScalarValue(ScalarType.Float, v);
        public static implicit operator NumericValue(double v) => new ScalarValue(ScalarType.Double, v);
        public static implicit operator NumericValue(bool v) => new ScalarValue(ScalarType.Bool, v);
        public static implicit operator NumericValue(char v) => new ScalarValue(ScalarType.Char, v);
    }

    public sealed class StructValue : HLSLValue
    {
        public string Name;
        public Dictionary<string, HLSLValue> Members;

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
        public object Value;

        public ScalarValue(ScalarType type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString() => Convert.ToString(Value, CultureInfo.InvariantCulture);
    }

    public sealed class VectorValue : NumericValue
    {
        public object[] Values;
        public int Size => Values.Length;

        public VectorValue(ScalarType type, object[] values)
        {
            Type = type;
            Values = values;
        }

        public override string ToString()
        {
            string type = PrintingUtil.GetEnumName(Type);
            return $"{type}{Size}({string.Join(", ", Values.Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)))})";
        }
    }

    public sealed class MatrixValue : NumericValue
    {
        public int Rows;
        public int Columns;
        public object[] Values;

        public MatrixValue(ScalarType type, int rows, int columns, object[] values)
        {
            Type = type;
            Rows = rows;
            Columns = columns;
            Values = values;
        }

        public override string ToString()
        {
            string type = PrintingUtil.GetEnumName(Type);
            return $"{type}{Rows}{Columns}({string.Join(", ", Values.Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)))})";
        }
    }

    public sealed class PredefinedObjectValue : HLSLValue
    {
        public PredefinedObjectType Type;
        public HLSLValue[] TemplateArguments;

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
        public HLSLValue[] Values;

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
                    return null;
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
                int sizeDiff = size - vec.Values.Length;
                if (sizeDiff > 0) // Expansion
                    return new VectorValue(vec.Type, vec.Values.Append(Enumerable.Repeat(GetZeroValue(vec.Type), sizeDiff)).ToArray());
                else if (size < vec.Values.Length) // Truncation
                    return new VectorValue(vec.Type, vec.Values.Take(size).ToArray());
                else
                    return vec;
            }
            if (value is ScalarValue scalar)
                return new VectorValue(scalar.Type, Enumerable.Repeat(scalar.Value, size).ToArray());
            return null;
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
                return new MatrixValue(scalar.Type, rows, columns, Enumerable.Repeat(scalar.Value, rows * columns).ToArray());
            return null;
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
                    return null;
            }
        }

        public static NumericValue CastNumeric(ScalarType type, NumericValue value)
        {
            if (value is ScalarValue scalar)
                return new ScalarValue(type, CastNumeric(type, scalar.Value));
            if (value is VectorValue vector)
                return new VectorValue(type, vector.Values.Select(x => CastNumeric(type, x)).ToArray());
            if (value is MatrixValue matrix)
                return new MatrixValue(type, matrix.Rows, matrix.Columns, matrix.Values.Select(x => CastNumeric(type, x)).ToArray());
            return null;
        }

        // Given 2 params, promote such that no information is lost.
        public static (NumericValue newLeft, NumericValue newRight) Promote(NumericValue left, NumericValue right, bool bitwiseOp)
        {
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
                return new ScalarValue(scalar.Type, mapper(scalar.Value));
            if (value is VectorValue vector)
                return new VectorValue(vector.Type, vector.Values.Select(mapper).ToArray());
            if (value is MatrixValue matrix)
                return new MatrixValue(matrix.Type, matrix.Rows, matrix.Columns, matrix.Values.Select(mapper).ToArray());
            return null;
        }

        public static NumericValue Map2(NumericValue left, NumericValue right, Func<object, object, object> mapper)
        {
            if (GetTensorSize(left) != GetTensorSize(right))
                throw new ArgumentException("Sizes of operands must match.");
            if (left is ScalarValue scalarLeft && right is ScalarValue scalarRight)
                return new ScalarValue(scalarLeft.Type, mapper(scalarLeft.Value, scalarRight.Value));
            if (left is VectorValue vectorLeft && right is VectorValue vectorRight)
            {
                object[] result = new object[vectorLeft.Size];
                for (int i = 0; i < result.Length; i++)
                    result[i] = mapper(vectorLeft.Values[i], vectorRight.Values[i]);
                return new VectorValue(vectorLeft.Type, result);
            }
            if (left is MatrixValue matrixLeft && right is MatrixValue matrixRight)
            {
                object[] result = new object[matrixLeft.Values.Length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = mapper(matrixLeft.Values[i], matrixRight.Values[i]);
                return new MatrixValue(matrixLeft.Type, matrixLeft.Rows, matrixLeft.Columns, result);
            }
            return null;
        }
    }
}
