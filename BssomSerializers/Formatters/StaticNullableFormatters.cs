
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bssom.Serializer.Formatters
{
     /// <summary>
    /// Format <see cref="SByte"/>? as BssomType.Null or BssomType.Int8 
    /// </summary>
    public sealed class StaticNullableInt8Formatter : IBssomFormatter<SByte?>
    {
        public static readonly StaticNullableInt8Formatter Instance = new StaticNullableInt8Formatter();

        private StaticNullableInt8Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, SByte? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.Int8Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public SByte? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadInt8();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, SByte? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="Int16"/>? as BssomType.Null or BssomType.Int16 
    /// </summary>
    public sealed class StaticNullableInt16Formatter : IBssomFormatter<Int16?>
    {
        public static readonly StaticNullableInt16Formatter Instance = new StaticNullableInt16Formatter();

        private StaticNullableInt16Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Int16? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.Int16Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Int16? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadInt16();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Int16? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="Int32"/>? as BssomType.Null or BssomType.Int32 
    /// </summary>
    public sealed class StaticNullableInt32Formatter : IBssomFormatter<Int32?>
    {
        public static readonly StaticNullableInt32Formatter Instance = new StaticNullableInt32Formatter();

        private StaticNullableInt32Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Int32? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.Int32Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Int32? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadInt32();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Int32? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="Int64"/>? as BssomType.Null or BssomType.Int64 
    /// </summary>
    public sealed class StaticNullableInt64Formatter : IBssomFormatter<Int64?>
    {
        public static readonly StaticNullableInt64Formatter Instance = new StaticNullableInt64Formatter();

        private StaticNullableInt64Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Int64? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.Int64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Int64? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadInt64();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Int64? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="Byte"/>? as BssomType.Null or BssomType.UInt8 
    /// </summary>
    public sealed class StaticNullableUInt8Formatter : IBssomFormatter<Byte?>
    {
        public static readonly StaticNullableUInt8Formatter Instance = new StaticNullableUInt8Formatter();

        private StaticNullableUInt8Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Byte? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.UInt8Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Byte? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadUInt8();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Byte? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="UInt16"/>? as BssomType.Null or BssomType.UInt16 
    /// </summary>
    public sealed class StaticNullableUInt16Formatter : IBssomFormatter<UInt16?>
    {
        public static readonly StaticNullableUInt16Formatter Instance = new StaticNullableUInt16Formatter();

        private StaticNullableUInt16Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, UInt16? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.UInt16Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public UInt16? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadUInt16();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, UInt16? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="UInt32"/>? as BssomType.Null or BssomType.UInt32 
    /// </summary>
    public sealed class StaticNullableUInt32Formatter : IBssomFormatter<UInt32?>
    {
        public static readonly StaticNullableUInt32Formatter Instance = new StaticNullableUInt32Formatter();

        private StaticNullableUInt32Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, UInt32? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.UInt32Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public UInt32? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadUInt32();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, UInt32? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="UInt64"/>? as BssomType.Null or BssomType.UInt64 
    /// </summary>
    public sealed class StaticNullableUInt64Formatter : IBssomFormatter<UInt64?>
    {
        public static readonly StaticNullableUInt64Formatter Instance = new StaticNullableUInt64Formatter();

        private StaticNullableUInt64Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, UInt64? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.UInt64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public UInt64? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadUInt64();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, UInt64? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="Single"/>? as BssomType.Null or BssomType.Float32 
    /// </summary>
    public sealed class StaticNullableFloat32Formatter : IBssomFormatter<Single?>
    {
        public static readonly StaticNullableFloat32Formatter Instance = new StaticNullableFloat32Formatter();

        private StaticNullableFloat32Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Single? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.Float32Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Single? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadFloat32();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Single? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="Double"/>? as BssomType.Null or BssomType.Float64 
    /// </summary>
    public sealed class StaticNullableFloat64Formatter : IBssomFormatter<Double?>
    {
        public static readonly StaticNullableFloat64Formatter Instance = new StaticNullableFloat64Formatter();

        private StaticNullableFloat64Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Double? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.Float64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Double? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadFloat64();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Double? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="Boolean"/>? as BssomType.Null or BssomType.Boolean 
    /// </summary>
    public sealed class StaticNullableBooleanFormatter : IBssomFormatter<Boolean?>
    {
        public static readonly StaticNullableBooleanFormatter Instance = new StaticNullableBooleanFormatter();

        private StaticNullableBooleanFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Boolean? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.BooleanSize + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Boolean? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadBoolean();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Boolean? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }

     /// <summary>
    /// Format <see cref="Char"/>? as BssomType.Null or BssomType.Char 
    /// </summary>
    public sealed class StaticNullableCharFormatter : IBssomFormatter<Char?>
    {
        public static readonly StaticNullableCharFormatter Instance = new StaticNullableCharFormatter();

        private StaticNullableCharFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Char? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.CharSize + BssomBinaryPrimitives.NativeTypeCodeSize;
        }

        public Char? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadChar();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Char? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="Decimal"/>? as BssomType.Null or BssomType.Decimal 
    /// </summary>
    public sealed class StaticNullableDecimalFormatter : IBssomFormatter<Decimal?>
    {
        public static readonly StaticNullableDecimalFormatter Instance = new StaticNullableDecimalFormatter();

        private StaticNullableDecimalFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Decimal? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.DecimalSize + BssomBinaryPrimitives.NativeTypeCodeSize;
        }

        public Decimal? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadDecimal();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Decimal? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
    /// <summary>
    /// Format <see cref="Guid"/>? as BssomType.Null or BssomType.Guid 
    /// </summary>
    public sealed class StaticNullableGuidFormatter : IBssomFormatter<Guid?>
    {
        public static readonly StaticNullableGuidFormatter Instance = new StaticNullableGuidFormatter();

        private StaticNullableGuidFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Guid? value)
        {
            if(value == null)
                return BssomBinaryPrimitives.NullSize;
            return BssomBinaryPrimitives.GuidSize + BssomBinaryPrimitives.NativeTypeCodeSize;
        }

        public Guid? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
                return default;
            return reader.ReadGuid();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Guid? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.Value);
        }
    }
}


