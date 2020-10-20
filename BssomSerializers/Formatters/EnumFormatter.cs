
using System;
using System.Runtime.CompilerServices;
using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
namespace BssomSerializers.Formatters
{
    /// <summary>
    /// Format Enum as BssomNumber
    /// </summary>
    internal sealed class EnumFormatter<T> : IBssomFormatter<T>
    {
        public static readonly EnumFormatter<T> Instance = new EnumFormatter<T>();

        private readonly Serialize<T> serializer;
        private readonly Deserialize<T> deserializer;
        private readonly int size;

        private EnumFormatter()
        {
            var underlyingType = typeof(T).GetEnumUnderlyingType();
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.Byte:
                    serializer = (ref BssomWriter writer, ref BssomSerializeContext context, T value) =>
                    {
                        writer.Write(Unsafe.As<T, Byte>(ref value));
                    };
                    deserializer = (ref BssomReader reader, ref BssomDeserializeContext context) =>
                    {
                        byte v = reader.ReadUInt8();
                        return Unsafe.As<Byte, T>(ref v);
                    };
                    size = BssomBinaryPrimitives.UInt8Size;
                    break;
                case TypeCode.Int16:
                    serializer = (ref BssomWriter writer, ref BssomSerializeContext context, T value) =>
                    {
                        writer.Write(Unsafe.As<T, Int16>(ref value));
                    };
                    deserializer = (ref BssomReader reader, ref BssomDeserializeContext context) =>
                    {
                        short v = reader.ReadInt16();
                        return Unsafe.As<Int16, T>(ref v);
                    };
                    size = BssomBinaryPrimitives.Int16Size;
                    break;
                case TypeCode.Int32:
                    serializer = (ref BssomWriter writer, ref BssomSerializeContext context, T value) =>
                    {
                        writer.Write(Unsafe.As<T, Int32>(ref value));
                    };
                    deserializer = (ref BssomReader reader, ref BssomDeserializeContext context) =>
                    {
                        int v = reader.ReadInt32();
                        return Unsafe.As<Int32, T>(ref v);
                    };
                    size = BssomBinaryPrimitives.Int32Size;
                    break;
                case TypeCode.Int64:
                    serializer = (ref BssomWriter writer, ref BssomSerializeContext context, T value) =>
                    {
                        writer.Write(Unsafe.As<T, Int64>(ref value));
                    };
                    deserializer = (ref BssomReader reader, ref BssomDeserializeContext context) =>
                    {
                        long v = reader.ReadInt64();
                        return Unsafe.As<Int64, T>(ref v);
                    };
                    size = BssomBinaryPrimitives.Int64Size;
                    break;
                case TypeCode.SByte:
                    serializer = (ref BssomWriter writer, ref BssomSerializeContext context, T value) =>
                    {
                        writer.Write(Unsafe.As<T, SByte>(ref value));
                    };
                    deserializer = (ref BssomReader reader, ref BssomDeserializeContext context) =>
                    {
                        SByte v = reader.ReadInt8();
                        return Unsafe.As<SByte, T>(ref v);
                    };
                    size = BssomBinaryPrimitives.Int8Size;
                    break;
                case TypeCode.UInt16:
                    serializer = (ref BssomWriter writer, ref BssomSerializeContext context, T value) =>
                    {
                        writer.Write(Unsafe.As<T, UInt16>(ref value));
                    };
                    deserializer = (ref BssomReader reader, ref BssomDeserializeContext context) =>
                    {
                        UInt16 v = reader.ReadUInt16();
                        return Unsafe.As<UInt16, T>(ref v);
                    };
                    size = BssomBinaryPrimitives.UInt16Size;
                    break;
                case TypeCode.UInt32:
                    serializer = (ref BssomWriter writer, ref BssomSerializeContext context, T value) =>
                    {
                        writer.Write(Unsafe.As<T, UInt32>(ref value));
                    };
                    deserializer = (ref BssomReader reader, ref BssomDeserializeContext context) =>
                    {
                        UInt32 v = reader.ReadUInt32();
                        return Unsafe.As<UInt32, T>(ref v);
                    };
                    size = BssomBinaryPrimitives.UInt32Size;
                    break;
                case TypeCode.UInt64:
                    serializer = (ref BssomWriter writer, ref BssomSerializeContext context, T value) =>
                    {
                        writer.Write(Unsafe.As<T, UInt64>(ref value));
                    };
                    deserializer = (ref BssomReader reader, ref BssomDeserializeContext context) =>
                    {
                        UInt64 v = reader.ReadUInt64();
                        return Unsafe.As<UInt64, T>(ref v);
                    };
                    size = BssomBinaryPrimitives.UInt64Size;
                    break;
                default:
                    break;
            }
        }

        public T Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return deserializer(ref reader, ref context);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, T value)
        {
            serializer(ref writer,ref context, value);
        }

        public int Size(ref BssomSizeContext context, T value)
        {
            return size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }
    }
}
