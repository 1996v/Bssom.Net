
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
namespace Bssom.Serializer.Formatters
{
    /// <summary>
    /// Format <see cref="ArraySegment{SByte}"/> as BssomType.Array1
    /// </summary>
    public sealed class Int8ArraySegmentFormatter : IBssomFormatter<ArraySegment<SByte>>
    {
        public static readonly Int8ArraySegmentFormatter Instance = new Int8ArraySegmentFormatter();

        private Int8ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<SByte> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int8Size, value.Count);
        }

        public ArraySegment<SByte> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int8Code))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new SByte[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadInt8WithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<SByte>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<SByte> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int8Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int8Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Int16}"/> as BssomType.Array1
    /// </summary>
    public sealed class Int16ArraySegmentFormatter : IBssomFormatter<ArraySegment<Int16>>
    {
        public static readonly Int16ArraySegmentFormatter Instance = new Int16ArraySegmentFormatter();

        private Int16ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Int16> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int16Size, value.Count);
        }

        public ArraySegment<Int16> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int16Code))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new Int16[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadInt16WithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<Int16>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Int16> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int16Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int16Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Int32}"/> as BssomType.Array1
    /// </summary>
    public sealed class Int32ArraySegmentFormatter : IBssomFormatter<ArraySegment<Int32>>
    {
        public static readonly Int32ArraySegmentFormatter Instance = new Int32ArraySegmentFormatter();

        private Int32ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Int32> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int32Size, value.Count);
        }

        public ArraySegment<Int32> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int32Code))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new Int32[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadInt32WithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<Int32>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Int32> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int32Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int32Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Int64}"/> as BssomType.Array1
    /// </summary>
    public sealed class Int64ArraySegmentFormatter : IBssomFormatter<ArraySegment<Int64>>
    {
        public static readonly Int64ArraySegmentFormatter Instance = new Int64ArraySegmentFormatter();

        private Int64ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Int64> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int64Size, value.Count);
        }

        public ArraySegment<Int64> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int64Code))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new Int64[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadInt64WithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<Int64>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Int64> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int64Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int64Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{UInt16}"/> as BssomType.Array1
    /// </summary>
    public sealed class UInt16ArraySegmentFormatter : IBssomFormatter<ArraySegment<UInt16>>
    {
        public static readonly UInt16ArraySegmentFormatter Instance = new UInt16ArraySegmentFormatter();

        private UInt16ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<UInt16> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt16Size, value.Count);
        }

        public ArraySegment<UInt16> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt16Code))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new UInt16[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadUInt16WithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<UInt16>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<UInt16> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt16Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt16Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{UInt32}"/> as BssomType.Array1
    /// </summary>
    public sealed class UInt32ArraySegmentFormatter : IBssomFormatter<ArraySegment<UInt32>>
    {
        public static readonly UInt32ArraySegmentFormatter Instance = new UInt32ArraySegmentFormatter();

        private UInt32ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<UInt32> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt32Size, value.Count);
        }

        public ArraySegment<UInt32> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt32Code))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new UInt32[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadUInt32WithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<UInt32>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<UInt32> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt32Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt32Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{UInt64}"/> as BssomType.Array1
    /// </summary>
    public sealed class UInt64ArraySegmentFormatter : IBssomFormatter<ArraySegment<UInt64>>
    {
        public static readonly UInt64ArraySegmentFormatter Instance = new UInt64ArraySegmentFormatter();

        private UInt64ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<UInt64> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt64Size, value.Count);
        }

        public ArraySegment<UInt64> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt64Code))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new UInt64[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadUInt64WithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<UInt64>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<UInt64> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt64Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt64Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Single}"/> as BssomType.Array1
    /// </summary>
    public sealed class Float32ArraySegmentFormatter : IBssomFormatter<ArraySegment<Single>>
    {
        public static readonly Float32ArraySegmentFormatter Instance = new Float32ArraySegmentFormatter();

        private Float32ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Single> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Float32Size, value.Count);
        }

        public ArraySegment<Single> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Float32Code))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new Single[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadFloat32WithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<Single>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Single> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Float32Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Float32Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Double}"/> as BssomType.Array1
    /// </summary>
    public sealed class Float64ArraySegmentFormatter : IBssomFormatter<ArraySegment<Double>>
    {
        public static readonly Float64ArraySegmentFormatter Instance = new Float64ArraySegmentFormatter();

        private Float64ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Double> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Float64Size, value.Count);
        }

        public ArraySegment<Double> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Float64Code))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new Double[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadFloat64WithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<Double>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Double> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Float64Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Float64Size, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Boolean}"/> as BssomType.Array1
    /// </summary>
    public sealed class BooleanArraySegmentFormatter : IBssomFormatter<ArraySegment<Boolean>>
    {
        public static readonly BooleanArraySegmentFormatter Instance = new BooleanArraySegmentFormatter();

        private BooleanArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Boolean> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.BooleanSize, value.Count);
        }

        public ArraySegment<Boolean> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.BooleanCode))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new Boolean[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadBooleanWithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<Boolean>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Boolean> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.BooleanCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.BooleanSize, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Char}"/> as BssomType.Array1
    /// </summary>
    public sealed class CharArraySegmentFormatter : IBssomFormatter<ArraySegment<Char>>
    {
        public static readonly CharArraySegmentFormatter Instance = new CharArraySegmentFormatter();

        private CharArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Char> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.CharSize, value.Count);
        }

        public ArraySegment<Char> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1NativeType(NativeBssomType.CharCode))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new Char[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadCharWithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<Char>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Char> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.CharCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.CharSize, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Decimal}"/> as BssomType.Array1
    /// </summary>
    public sealed class DecimalArraySegmentFormatter : IBssomFormatter<ArraySegment<Decimal>>
    {
        public static readonly DecimalArraySegmentFormatter Instance = new DecimalArraySegmentFormatter();

        private DecimalArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Decimal> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.DecimalSize, value.Count);
        }

        public ArraySegment<Decimal> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1NativeType(NativeBssomType.DecimalCode))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new Decimal[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadDecimalWithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<Decimal>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Decimal> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.DecimalCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.DecimalSize, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Guid}"/> as BssomType.Array1
    /// </summary>
    public sealed class GuidArraySegmentFormatter : IBssomFormatter<ArraySegment<Guid>>
    {
        public static readonly GuidArraySegmentFormatter Instance = new GuidArraySegmentFormatter();

        private GuidArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Guid> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.GuidSize, value.Count);
        }

        public ArraySegment<Guid> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1NativeType(NativeBssomType.GuidCode))
                return default;
            context.Option.Security.DepthStep(ref context);
            reader.SkipVariableNumber();
            int len = reader.ReadVariableNumber();
            var val = new Guid[len];
            for (int i = 0; i < len; i++)
            {
                val[i] = reader.ReadGuidWithOutTypeHead();
            }
            context.Depth--;
            return new ArraySegment<Guid>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Guid> value)
        {
            if(value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.GuidCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.GuidSize, value.Count));
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                writer.WriteWithOutTypeHead(value.Array[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{DateTime}"/> as BssomType.Array1
    /// </summary>
    public sealed class DateTimeArraySegmentFormatter : IBssomFormatter<ArraySegment<DateTime>>
    {
        public static readonly DateTimeArraySegmentFormatter Instance = new DateTimeArraySegmentFormatter();

        private DateTimeArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<DateTime> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (context.Option.IsUseStandardDateTime == false)
                return BssomBinaryPrimitives.NativeDateTimeArraySize(value.Count);
            else
                return BssomBinaryPrimitives.StandardDateTimeArraySize(value.Count);
        }

        public ArraySegment<DateTime> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array1))
                return default;
            context.Option.Security.DepthStep(ref context);
            DateTime[] val;
            byte type = reader.ReadBssomType();
            switch (type)
            {
                case BssomType.TimestampCode:
                    reader.SkipVariableNumber();
                    int len = reader.ReadVariableNumber();
                    val = new DateTime[len];
                    for (int i = 0; i < len; i++)
                    {
                        val[i] = reader.ReadStandDateTimeWithOutTypeHead();
                    }
                    break;
                case BssomType.NativeCode:
                    reader.EnsureType(NativeBssomType.DateTimeCode);
                    reader.SkipVariableNumber();
                    len = reader.ReadVariableNumber();
                    val = new DateTime[len];
                    for (int i = 0; i < len; i++)
                    {
                        val[i] = reader.ReadNativeDateTimeWithOutTypeHead();
                    }
                    break;
                default:
                    throw BssomSerializationOperationException.UnexpectedCodeRead(type, reader.Position);
            }

            context.Depth--;
            return new ArraySegment<DateTime>(val);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<DateTime> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (context.Option.IsUseStandardDateTime)
                writer.WriteArray1BuildInType(BssomType.TimestampCode);
            else
                writer.WriteArray1NativeType(NativeBssomType.DateTimeCode);
            long posLen = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(value.Count);
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                writer.Write(value.Array[i], context.Option.IsUseStandardDateTime, false);
            }
            writer.WriteBackFixNumber(posLen,checked((int)(writer.Position - posLen - BssomBinaryPrimitives.FixUInt32NumberSize)));
        }
    }
    /// <summary>
    /// Format <see cref="ArraySegment{Byte}"/> as BssomType.Array1
    /// </summary>
    public sealed class UInt8ArraySegmentFormatter : IBssomFormatter<ArraySegment<Byte>>
    {
        public static readonly UInt8ArraySegmentFormatter Instance = new UInt8ArraySegmentFormatter();

        private UInt8ArraySegmentFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, ArraySegment<Byte> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt8Size, value.Count);
        }

        public ArraySegment<Byte> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return new ArraySegment<Byte>(reader.ReadBytes());
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<Byte> value)
        {
            writer.Write(value);
        }
    }

    /// <summary>
    /// Format <see cref="ArraySegment{T}"/> as BssomType.Array2
    /// </summary>
    public sealed class ArraySegmentFormatter<T> : IBssomFormatter<ArraySegment<T>>
    {
        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ArraySegment<T> value)
        {
            if (value.Array == null)
            {
                writer.WriteNull();
                return;
            }
            
            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

            writer.WriteArray2Type();
            long posLen = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(value.Count);

            for (int i = value.Offset; i < value.Offset + value.Count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                formatter.Serialize(ref writer, ref context, value.Array[i]);
            }

            writer.WriteBackFixNumber(posLen,checked((int)(writer.Position - posLen - BssomBinaryPrimitives.FixUInt32NumberSize)));
        }

        public ArraySegment<T> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {           
            T[] array = context.Option.FormatterResolver.GetFormatterWithVerify<T[]>().Deserialize(ref reader, ref context);
            if(array == null)
                return default;
            return new ArraySegment<T>(array);
        }

        public int Size(ref BssomSizeContext context, ArraySegment<T> value)
        {
            if (value.Array == null)
                return BssomBinaryPrimitives.NullSize;

            var formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();
            long len = 0;
            for (int i = value.Offset; i < value.Count + value.Offset; i++)
            {
                len += formatter.Size(ref context, value.Array[i]);
            }
            return BssomBinaryPrimitives.Array2TypeSize(value.Count, len);
        }
    }
}
