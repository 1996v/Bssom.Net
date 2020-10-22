
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
    /// Format <see cref="SByte"/>[] as BssomType.Array1
    /// </summary>
    public sealed class Int8ArrayFormatter : IBssomFormatter<SByte[]>
    {
        public static readonly Int8ArrayFormatter Instance = new Int8ArrayFormatter();

        private Int8ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, SByte[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int8Size, value.Length);
        }

        public SByte[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, SByte[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int8Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int8Size, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="Int16"/>[] as BssomType.Array1
    /// </summary>
    public sealed class Int16ArrayFormatter : IBssomFormatter<Int16[]>
    {
        public static readonly Int16ArrayFormatter Instance = new Int16ArrayFormatter();

        private Int16ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Int16[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int16Size, value.Length);
        }

        public Int16[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Int16[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int16Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int16Size, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="Int32"/>[] as BssomType.Array1
    /// </summary>
    public sealed class Int32ArrayFormatter : IBssomFormatter<Int32[]>
    {
        public static readonly Int32ArrayFormatter Instance = new Int32ArrayFormatter();

        private Int32ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Int32[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int32Size, value.Length);
        }

        public Int32[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Int32[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int32Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int32Size, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="Int64"/>[] as BssomType.Array1
    /// </summary>
    public sealed class Int64ArrayFormatter : IBssomFormatter<Int64[]>
    {
        public static readonly Int64ArrayFormatter Instance = new Int64ArrayFormatter();

        private Int64ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Int64[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int64Size, value.Length);
        }

        public Int64[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Int64[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int64Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int64Size, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="UInt16"/>[] as BssomType.Array1
    /// </summary>
    public sealed class UInt16ArrayFormatter : IBssomFormatter<UInt16[]>
    {
        public static readonly UInt16ArrayFormatter Instance = new UInt16ArrayFormatter();

        private UInt16ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, UInt16[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt16Size, value.Length);
        }

        public UInt16[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, UInt16[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt16Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt16Size, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="UInt32"/>[] as BssomType.Array1
    /// </summary>
    public sealed class UInt32ArrayFormatter : IBssomFormatter<UInt32[]>
    {
        public static readonly UInt32ArrayFormatter Instance = new UInt32ArrayFormatter();

        private UInt32ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, UInt32[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt32Size, value.Length);
        }

        public UInt32[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, UInt32[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt32Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt32Size, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="UInt64"/>[] as BssomType.Array1
    /// </summary>
    public sealed class UInt64ArrayFormatter : IBssomFormatter<UInt64[]>
    {
        public static readonly UInt64ArrayFormatter Instance = new UInt64ArrayFormatter();

        private UInt64ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, UInt64[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt64Size, value.Length);
        }

        public UInt64[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, UInt64[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt64Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt64Size, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="Single"/>[] as BssomType.Array1
    /// </summary>
    public sealed class Float32ArrayFormatter : IBssomFormatter<Single[]>
    {
        public static readonly Float32ArrayFormatter Instance = new Float32ArrayFormatter();

        private Float32ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Single[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Float32Size, value.Length);
        }

        public Single[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Single[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Float32Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Float32Size, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="Double"/>[] as BssomType.Array1
    /// </summary>
    public sealed class Float64ArrayFormatter : IBssomFormatter<Double[]>
    {
        public static readonly Float64ArrayFormatter Instance = new Float64ArrayFormatter();

        private Float64ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Double[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Float64Size, value.Length);
        }

        public Double[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Double[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Float64Code);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Float64Size, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="Boolean"/>[] as BssomType.Array1
    /// </summary>
    public sealed class BooleanArrayFormatter : IBssomFormatter<Boolean[]>
    {
        public static readonly BooleanArrayFormatter Instance = new BooleanArrayFormatter();

        private BooleanArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Boolean[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.BooleanSize, value.Length);
        }

        public Boolean[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Boolean[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.BooleanCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.BooleanSize, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="Char"/>[] as BssomType.Array1
    /// </summary>
    public sealed class CharArrayFormatter : IBssomFormatter<Char[]>
    {
        public static readonly CharArrayFormatter Instance = new CharArrayFormatter();

        private CharArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Char[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.CharSize, value.Length);
        }

        public Char[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Char[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.CharCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.CharSize, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="Decimal"/>[] as BssomType.Array1
    /// </summary>
    public sealed class DecimalArrayFormatter : IBssomFormatter<Decimal[]>
    {
        public static readonly DecimalArrayFormatter Instance = new DecimalArrayFormatter();

        private DecimalArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Decimal[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.DecimalSize, value.Length);
        }

        public Decimal[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Decimal[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.DecimalCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.DecimalSize, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="Guid"/>[] as BssomType.Array1
    /// </summary>
    public sealed class GuidArrayFormatter : IBssomFormatter<Guid[]>
    {
        public static readonly GuidArrayFormatter Instance = new GuidArrayFormatter();

        private GuidArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Guid[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.GuidSize, value.Length);
        }

        public Guid[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Guid[] value)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.GuidCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.GuidSize, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value[i]);
            }
        }
    }
    /// <summary>
    /// Format <see cref="DateTime"/>[] as BssomType.Array1
    /// </summary>
    public sealed class DateTimeArrayFormatter : IBssomFormatter<DateTime[]>
    {
        public static readonly DateTimeArrayFormatter Instance = new DateTimeArrayFormatter();

        private DateTimeArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, DateTime[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (context.Option.IsUseStandardDateTime == false)
                return BssomBinaryPrimitives.NativeDateTimeArraySize(value.Length);
            else
                return BssomBinaryPrimitives.StandardDateTimeArraySize(value.Length);
        }

        public DateTime[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
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
            return val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, DateTime[] value)
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
            long pos = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.Write(value[i], context.Option.IsUseStandardDateTime, false);
            }
            writer.WriteBackFixNumber(pos,checked((int)(writer.Position - pos - BssomBinaryPrimitives.FixUInt32NumberSize)));
        }
    }
    /// <summary>
    /// Format <see cref="Byte"/>[] as BssomType.Array1
    /// </summary>
    public sealed class UInt8ArrayFormatter : IBssomFormatter<Byte[]>
    {
        public static readonly UInt8ArrayFormatter Instance = new UInt8ArrayFormatter();

        private UInt8ArrayFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Byte[] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt8Size, value.Length);
        }

        public Byte[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadBytes();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Byte[] value)
        {
            writer.Write(value);
        }
    }
}
