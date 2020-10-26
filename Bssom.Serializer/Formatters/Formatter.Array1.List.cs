
using Bssom.Serializer.Binary;
using System;
using System.Collections.Generic;
namespace Bssom.Serializer.Formatters
{
    /// <summary>
    /// Format <see cref="List{SByte}"/> as BssomType.Array1
    /// </summary>
    public sealed class Int8ListFormatter : IBssomFormatter<List<SByte>>
    {
        public static readonly Int8ListFormatter Instance = new Int8ListFormatter();

        private Int8ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<SByte> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int8Size, value.Count);
        }

        public List<SByte> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int8Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<sbyte> val = new List<SByte>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadInt8WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<SByte> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int8Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int8Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Int16}"/> as BssomType.Array1
    /// </summary>
    public sealed class Int16ListFormatter : IBssomFormatter<List<Int16>>
    {
        public static readonly Int16ListFormatter Instance = new Int16ListFormatter();

        private Int16ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Int16> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int16Size, value.Count);
        }

        public List<Int16> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int16Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<short> val = new List<Int16>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadInt16WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Int16> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int16Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int16Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Int32}"/> as BssomType.Array1
    /// </summary>
    public sealed class Int32ListFormatter : IBssomFormatter<List<Int32>>
    {
        public static readonly Int32ListFormatter Instance = new Int32ListFormatter();

        private Int32ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Int32> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int32Size, value.Count);
        }

        public List<Int32> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int32Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<int> val = new List<Int32>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadInt32WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Int32> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int32Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int32Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Int64}"/> as BssomType.Array1
    /// </summary>
    public sealed class Int64ListFormatter : IBssomFormatter<List<Int64>>
    {
        public static readonly Int64ListFormatter Instance = new Int64ListFormatter();

        private Int64ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Int64> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int64Size, value.Count);
        }

        public List<Int64> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int64Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<long> val = new List<Int64>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadInt64WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Int64> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int64Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int64Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Byte}"/> as BssomType.Array1
    /// </summary>
    public sealed class UInt8ListFormatter : IBssomFormatter<List<Byte>>
    {
        public static readonly UInt8ListFormatter Instance = new UInt8ListFormatter();

        private UInt8ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Byte> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt8Size, value.Count);
        }

        public List<Byte> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt8Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<byte> val = new List<Byte>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadUInt8WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Byte> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt8Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt8Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{UInt16}"/> as BssomType.Array1
    /// </summary>
    public sealed class UInt16ListFormatter : IBssomFormatter<List<UInt16>>
    {
        public static readonly UInt16ListFormatter Instance = new UInt16ListFormatter();

        private UInt16ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<UInt16> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt16Size, value.Count);
        }

        public List<UInt16> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt16Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<ushort> val = new List<UInt16>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadUInt16WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<UInt16> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt16Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt16Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{UInt32}"/> as BssomType.Array1
    /// </summary>
    public sealed class UInt32ListFormatter : IBssomFormatter<List<UInt32>>
    {
        public static readonly UInt32ListFormatter Instance = new UInt32ListFormatter();

        private UInt32ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<UInt32> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt32Size, value.Count);
        }

        public List<UInt32> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt32Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<uint> val = new List<UInt32>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadUInt32WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<UInt32> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt32Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt32Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{UInt64}"/> as BssomType.Array1
    /// </summary>
    public sealed class UInt64ListFormatter : IBssomFormatter<List<UInt64>>
    {
        public static readonly UInt64ListFormatter Instance = new UInt64ListFormatter();

        private UInt64ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<UInt64> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt64Size, value.Count);
        }

        public List<UInt64> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt64Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<ulong> val = new List<UInt64>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadUInt64WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<UInt64> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt64Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt64Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Single}"/> as BssomType.Array1
    /// </summary>
    public sealed class Float32ListFormatter : IBssomFormatter<List<Single>>
    {
        public static readonly Float32ListFormatter Instance = new Float32ListFormatter();

        private Float32ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Single> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Float32Size, value.Count);
        }

        public List<Single> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Float32Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<float> val = new List<Single>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadFloat32WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Single> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Float32Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Float32Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Double}"/> as BssomType.Array1
    /// </summary>
    public sealed class Float64ListFormatter : IBssomFormatter<List<Double>>
    {
        public static readonly Float64ListFormatter Instance = new Float64ListFormatter();

        private Float64ListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Double> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Float64Size, value.Count);
        }

        public List<Double> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Float64Code))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<double> val = new List<Double>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadFloat64WithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Double> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Float64Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Float64Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Boolean}"/> as BssomType.Array1
    /// </summary>
    public sealed class BooleanListFormatter : IBssomFormatter<List<Boolean>>
    {
        public static readonly BooleanListFormatter Instance = new BooleanListFormatter();

        private BooleanListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Boolean> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.BooleanSize, value.Count);
        }

        public List<Boolean> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.BooleanCode))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<bool> val = new List<Boolean>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadBooleanWithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Boolean> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.BooleanCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.BooleanSize, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Char}"/> as BssomType.Array1
    /// </summary>
    public sealed class CharListFormatter : IBssomFormatter<List<Char>>
    {
        public static readonly CharListFormatter Instance = new CharListFormatter();

        private CharListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Char> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.CharSize, value.Count);
        }

        public List<Char> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1NativeType(NativeBssomType.CharCode))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<char> val = new List<Char>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadCharWithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Char> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.CharCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.CharSize, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Decimal}"/> as BssomType.Array1
    /// </summary>
    public sealed class DecimalListFormatter : IBssomFormatter<List<Decimal>>
    {
        public static readonly DecimalListFormatter Instance = new DecimalListFormatter();

        private DecimalListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Decimal> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.DecimalSize, value.Count);
        }

        public List<Decimal> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1NativeType(NativeBssomType.DecimalCode))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<decimal> val = new List<Decimal>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadDecimalWithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Decimal> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.DecimalCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.DecimalSize, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{Guid}"/> as BssomType.Array1
    /// </summary>
    public sealed class GuidListFormatter : IBssomFormatter<List<Guid>>
    {
        public static readonly GuidListFormatter Instance = new GuidListFormatter();

        private GuidListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<Guid> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.GuidSize, value.Count);
        }

        public List<Guid> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1NativeType(NativeBssomType.GuidCode))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            List<Guid> val = new List<Guid>(len);
            for (int i = 0; i < len; i++)
            {
                val.Add(reader.ReadGuidWithOutTypeHead());
            }
            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<Guid> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.GuidCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.GuidSize, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="List{DateTime}"/> as BssomType.Array1
    /// </summary>
    public sealed class DateTimeListFormatter : IBssomFormatter<List<DateTime>>
    {
        public static readonly DateTimeListFormatter Instance = new DateTimeListFormatter();

        private DateTimeListFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, List<DateTime> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            if (context.Option.IsUseStandardDateTime == false)
            {
                return BssomBinaryPrimitives.NativeDateTimeArraySize(value.Count);
            }
            else
            {
                return BssomBinaryPrimitives.StandardDateTimeArraySize(value.Count);
            }
        }

        public List<DateTime> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array1))
            {
                return default;
            }

            context.Option.Security.DepthStep(ref context);
            List<DateTime> val;
            byte type = reader.ReadBssomType();
            switch (type)
            {
                case BssomType.TimestampCode:
                    reader.SkipVariableNumber();
                    int len = reader.ReadVariableNumber();
                    val = new List<DateTime>(len);
                    for (int i = 0; i < len; i++)
                    {
                        val.Add(reader.ReadStandDateTimeWithOutTypeHead());
                    }
                    break;
                case BssomType.NativeCode:
                    reader.EnsureType(NativeBssomType.DateTimeCode);
                    reader.SkipVariableNumber();
                    len = reader.ReadVariableNumber();
                    val = new List<DateTime>(len);
                    for (int i = 0; i < len; i++)
                    {
                        val.Add(reader.ReadNativeDateTimeWithOutTypeHead());
                    }
                    break;
                default:
                    throw BssomSerializationOperationException.UnexpectedCodeRead(type, reader.Position);
            }

            context.Depth--;
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, List<DateTime> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (context.Option.IsUseStandardDateTime)
            {
                writer.WriteArray1BuildInType(BssomType.TimestampCode);
            }
            else
            {
                writer.WriteArray1NativeType(NativeBssomType.DateTimeCode);
            }

            long posLen = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                writer.Write(value[i], context.Option.IsUseStandardDateTime, false);
            }
            writer.WriteBackFixNumber(posLen, checked((int)(writer.Position - posLen - BssomBinaryPrimitives.FixUInt32NumberSize)));
        }
    }
}
