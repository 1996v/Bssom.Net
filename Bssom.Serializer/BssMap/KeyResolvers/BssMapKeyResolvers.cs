
using Bssom.Serializer.Binary;
using Bssom.Serializer.Internal;
using System;
using System.Runtime.CompilerServices;
namespace Bssom.Serializer.BssMap.KeyResolvers
{
    internal class BssMapInt8KeyResolver : IBssMapKeyResolver<SByte>
    {
        public static BssMapInt8KeyResolver Insance = new BssMapInt8KeyResolver();

        public byte KeyType => BssomType.Int8Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(SByte key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Int8Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((SByte)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(SByte key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Int8Size];
            BssomBinaryPrimitives.WriteInt8(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((SByte)key);
        }

        public unsafe SByte ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadInt8(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapInt16KeyResolver : IBssMapKeyResolver<Int16>
    {
        public static BssMapInt16KeyResolver Insance = new BssMapInt16KeyResolver();

        public byte KeyType => BssomType.Int16Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(Int16 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Int16Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Int16)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Int16 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Int16Size];
            BssomBinaryPrimitives.WriteInt16LittleEndian(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Int16)key);
        }

        public unsafe Int16 ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadInt16LittleEndian(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapInt32KeyResolver : IBssMapKeyResolver<Int32>
    {
        public static BssMapInt32KeyResolver Insance = new BssMapInt32KeyResolver();

        public byte KeyType => BssomType.Int32Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(Int32 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Int32Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Int32)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Int32 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Int32Size];
            BssomBinaryPrimitives.WriteInt32LittleEndian(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Int32)key);
        }

        public unsafe Int32 ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadInt32LittleEndian(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapInt64KeyResolver : IBssMapKeyResolver<Int64>
    {
        public static BssMapInt64KeyResolver Insance = new BssMapInt64KeyResolver();

        public byte KeyType => BssomType.Int64Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(Int64 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Int64Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Int64)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Int64 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Int64Size];
            BssomBinaryPrimitives.WriteInt64LittleEndian(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Int64)key);
        }

        public unsafe Int64 ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadInt64LittleEndian(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapUInt8KeyResolver : IBssMapKeyResolver<Byte>
    {
        public static BssMapUInt8KeyResolver Insance = new BssMapUInt8KeyResolver();

        public byte KeyType => BssomType.UInt8Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(Byte key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.UInt8Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Byte)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Byte key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.UInt8Size];
            BssomBinaryPrimitives.WriteUInt8(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Byte)key);
        }

        public unsafe Byte ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadUInt8(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapUInt16KeyResolver : IBssMapKeyResolver<UInt16>
    {
        public static BssMapUInt16KeyResolver Insance = new BssMapUInt16KeyResolver();

        public byte KeyType => BssomType.UInt16Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(UInt16 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.UInt16Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((UInt16)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(UInt16 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.UInt16Size];
            BssomBinaryPrimitives.WriteUInt16LittleEndian(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((UInt16)key);
        }

        public unsafe UInt16 ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadUInt16LittleEndian(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapUInt32KeyResolver : IBssMapKeyResolver<UInt32>
    {
        public static BssMapUInt32KeyResolver Insance = new BssMapUInt32KeyResolver();

        public byte KeyType => BssomType.UInt32Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(UInt32 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.UInt32Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((UInt32)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(UInt32 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.UInt32Size];
            BssomBinaryPrimitives.WriteUInt32LittleEndian(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((UInt32)key);
        }

        public unsafe UInt32 ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadUInt32LittleEndian(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapUInt64KeyResolver : IBssMapKeyResolver<UInt64>
    {
        public static BssMapUInt64KeyResolver Insance = new BssMapUInt64KeyResolver();

        public byte KeyType => BssomType.UInt64Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(UInt64 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.UInt64Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((UInt64)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(UInt64 key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.UInt64Size];
            BssomBinaryPrimitives.WriteUInt64LittleEndian(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((UInt64)key);
        }

        public unsafe UInt64 ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadUInt64LittleEndian(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapFloat32KeyResolver : IBssMapKeyResolver<Single>
    {
        public static BssMapFloat32KeyResolver Insance = new BssMapFloat32KeyResolver();

        public byte KeyType => BssomType.Float32Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(Single key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Float32Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Single)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Single key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Float32Size];
            BssomBinaryPrimitives.WriteFloat32LittleEndian(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Single)key);
        }

        public unsafe Single ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadFloat32LittleEndian(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapFloat64KeyResolver : IBssMapKeyResolver<Double>
    {
        public static BssMapFloat64KeyResolver Insance = new BssMapFloat64KeyResolver();

        public byte KeyType => BssomType.Float64Code;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(Double key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Float64Size];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Double)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Double key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.Float64Size];
            BssomBinaryPrimitives.WriteFloat64LittleEndian(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Double)key);
        }

        public unsafe Double ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadFloat64LittleEndian(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapBooleanKeyResolver : IBssMapKeyResolver<Boolean>
    {
        public static BssMapBooleanKeyResolver Insance = new BssMapBooleanKeyResolver();

        public byte KeyType => BssomType.BooleanCode;

        public bool KeyIsNativeType => false;

        public Raw64BytesISegment GetMap1KeySegment(Boolean key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.BooleanSize];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Boolean)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Boolean key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.BooleanSize];
            BssomBinaryPrimitives.WriteBoolean(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Boolean)key);
        }

        public unsafe Boolean ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadBoolean(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapCharKeyResolver : IBssMapKeyResolver<Char>
    {
        public static BssMapCharKeyResolver Insance = new BssMapCharKeyResolver();

        public byte KeyType => NativeBssomType.CharCode;

        public bool KeyIsNativeType => true;

        public Raw64BytesISegment GetMap1KeySegment(Char key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.CharSize];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Char)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Char key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.CharSize];
            BssomBinaryPrimitives.WriteCharLittleEndian(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Char)key);
        }

        public unsafe Char ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadCharLittleEndian(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapDecimalKeyResolver : IBssMapKeyResolver<Decimal>
    {
        public static BssMapDecimalKeyResolver Insance = new BssMapDecimalKeyResolver();

        public byte KeyType => NativeBssomType.DecimalCode;

        public bool KeyIsNativeType => true;

        public Raw64BytesISegment GetMap1KeySegment(Decimal key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.DecimalSize];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Decimal)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Decimal key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.DecimalSize];
            BssomBinaryPrimitives.WriteDecimal(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Decimal)key);
        }

        public unsafe Decimal ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadDecimal(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
    internal class BssMapGuidKeyResolver : IBssMapKeyResolver<Guid>
    {
        public static BssMapGuidKeyResolver Insance = new BssMapGuidKeyResolver();

        public byte KeyType => NativeBssomType.GuidCode;

        public bool KeyIsNativeType => true;

        public Raw64BytesISegment GetMap1KeySegment(Guid key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.GuidSize];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((Guid)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(Guid key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.GuidSize];
            BssomBinaryPrimitives.WriteGuid(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((Guid)key);
        }

        public unsafe Guid ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadGuid(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
}
