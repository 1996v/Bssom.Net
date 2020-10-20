
using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BssomSerializers.Formatters
{
     /// <summary>
    /// Format <see cref="SByte"/> as BssomType.Int8
    /// </summary>
    public sealed class Int8Formatter : IBssomFormatter<SByte>
    {
        public static readonly Int8Formatter Instance = new Int8Formatter();

        private Int8Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, SByte value)
        {
            return BssomBinaryPrimitives.Int8Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public SByte Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadInt8();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, SByte value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="Int16"/> as BssomType.Int16
    /// </summary>
    public sealed class Int16Formatter : IBssomFormatter<Int16>
    {
        public static readonly Int16Formatter Instance = new Int16Formatter();

        private Int16Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Int16 value)
        {
            return BssomBinaryPrimitives.Int16Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Int16 Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadInt16();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Int16 value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="Int32"/> as BssomType.Int32
    /// </summary>
    public sealed class Int32Formatter : IBssomFormatter<Int32>
    {
        public static readonly Int32Formatter Instance = new Int32Formatter();

        private Int32Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Int32 value)
        {
            return BssomBinaryPrimitives.Int32Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Int32 Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadInt32();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Int32 value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="Int64"/> as BssomType.Int64
    /// </summary>
    public sealed class Int64Formatter : IBssomFormatter<Int64>
    {
        public static readonly Int64Formatter Instance = new Int64Formatter();

        private Int64Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Int64 value)
        {
            return BssomBinaryPrimitives.Int64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Int64 Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadInt64();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Int64 value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="Byte"/> as BssomType.UInt8
    /// </summary>
    public sealed class UInt8Formatter : IBssomFormatter<Byte>
    {
        public static readonly UInt8Formatter Instance = new UInt8Formatter();

        private UInt8Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Byte value)
        {
            return BssomBinaryPrimitives.UInt8Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Byte Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadUInt8();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Byte value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="UInt16"/> as BssomType.UInt16
    /// </summary>
    public sealed class UInt16Formatter : IBssomFormatter<UInt16>
    {
        public static readonly UInt16Formatter Instance = new UInt16Formatter();

        private UInt16Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, UInt16 value)
        {
            return BssomBinaryPrimitives.UInt16Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public UInt16 Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadUInt16();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, UInt16 value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="UInt32"/> as BssomType.UInt32
    /// </summary>
    public sealed class UInt32Formatter : IBssomFormatter<UInt32>
    {
        public static readonly UInt32Formatter Instance = new UInt32Formatter();

        private UInt32Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, UInt32 value)
        {
            return BssomBinaryPrimitives.UInt32Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public UInt32 Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadUInt32();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, UInt32 value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="UInt64"/> as BssomType.UInt64
    /// </summary>
    public sealed class UInt64Formatter : IBssomFormatter<UInt64>
    {
        public static readonly UInt64Formatter Instance = new UInt64Formatter();

        private UInt64Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, UInt64 value)
        {
            return BssomBinaryPrimitives.UInt64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public UInt64 Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadUInt64();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, UInt64 value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="Single"/> as BssomType.Float32
    /// </summary>
    public sealed class Float32Formatter : IBssomFormatter<Single>
    {
        public static readonly Float32Formatter Instance = new Float32Formatter();

        private Float32Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Single value)
        {
            return BssomBinaryPrimitives.Float32Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Single Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadFloat32();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Single value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="Double"/> as BssomType.Float64
    /// </summary>
    public sealed class Float64Formatter : IBssomFormatter<Double>
    {
        public static readonly Float64Formatter Instance = new Float64Formatter();

        private Float64Formatter()
        {
        }

        public int Size(ref BssomSizeContext context, Double value)
        {
            return BssomBinaryPrimitives.Float64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Double Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadFloat64();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Double value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="Boolean"/> as BssomType.Boolean
    /// </summary>
    public sealed class BooleanFormatter : IBssomFormatter<Boolean>
    {
        public static readonly BooleanFormatter Instance = new BooleanFormatter();

        private BooleanFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Boolean value)
        {
            return BssomBinaryPrimitives.BooleanSize + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public Boolean Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadBoolean();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Boolean value)
        {
            writer.Write(value);
        }
    }

     /// <summary>
    /// Format <see cref="Char"/> as NativeBssomType.Char
    /// </summary>
    public sealed class CharFormatter : IBssomFormatter<Char>
    {
        public static readonly CharFormatter Instance = new CharFormatter();

        private CharFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Char value)
        {
            return BssomBinaryPrimitives.CharSize + BssomBinaryPrimitives.NativeTypeCodeSize;
        }

        public Char Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadChar();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Char value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="Decimal"/> as NativeBssomType.Decimal
    /// </summary>
    public sealed class DecimalFormatter : IBssomFormatter<Decimal>
    {
        public static readonly DecimalFormatter Instance = new DecimalFormatter();

        private DecimalFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Decimal value)
        {
            return BssomBinaryPrimitives.DecimalSize + BssomBinaryPrimitives.NativeTypeCodeSize;
        }

        public Decimal Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadDecimal();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Decimal value)
        {
            writer.Write(value);
        }
    }
    /// <summary>
    /// Format <see cref="Guid"/> as NativeBssomType.Guid
    /// </summary>
    public sealed class GuidFormatter : IBssomFormatter<Guid>
    {
        public static readonly GuidFormatter Instance = new GuidFormatter();

        private GuidFormatter()
        {
        }

        public int Size(ref BssomSizeContext context, Guid value)
        {
            return BssomBinaryPrimitives.GuidSize + BssomBinaryPrimitives.NativeTypeCodeSize;
        }

        public Guid Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadGuid();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Guid value)
        {
            writer.Write(value);
        }
    }
}


