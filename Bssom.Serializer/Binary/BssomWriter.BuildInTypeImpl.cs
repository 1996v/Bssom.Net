using Bssom.Serializer.Binary;
using System;
using System.Runtime.CompilerServices;
namespace Bssom.Serializer
{
    //BuildInTypeCode
    public partial struct BssomWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(SByte value)
        {
            const int len = BssomBinaryPrimitives.Int8Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.Int8Code);
            BssomBinaryPrimitives.WriteInt8(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(SByte value)
        {
            BssomBinaryPrimitives.WriteInt8(ref BufferWriter.GetRef(BssomBinaryPrimitives.Int8Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.Int8Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Int16 value)
        {
            const int len = BssomBinaryPrimitives.Int16Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.Int16Code);
            BssomBinaryPrimitives.WriteInt16LittleEndian(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Int16 value)
        {
            BssomBinaryPrimitives.WriteInt16LittleEndian(ref BufferWriter.GetRef(BssomBinaryPrimitives.Int16Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.Int16Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Int32 value)
        {
            const int len = BssomBinaryPrimitives.Int32Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.Int32Code);
            BssomBinaryPrimitives.WriteInt32LittleEndian(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Int32 value)
        {
            BssomBinaryPrimitives.WriteInt32LittleEndian(ref BufferWriter.GetRef(BssomBinaryPrimitives.Int32Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.Int32Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Int64 value)
        {
            const int len = BssomBinaryPrimitives.Int64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.Int64Code);
            BssomBinaryPrimitives.WriteInt64LittleEndian(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Int64 value)
        {
            BssomBinaryPrimitives.WriteInt64LittleEndian(ref BufferWriter.GetRef(BssomBinaryPrimitives.Int64Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.Int64Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Byte value)
        {
            const int len = BssomBinaryPrimitives.UInt8Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.UInt8Code);
            BssomBinaryPrimitives.WriteUInt8(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Byte value)
        {
            BssomBinaryPrimitives.WriteUInt8(ref BufferWriter.GetRef(BssomBinaryPrimitives.UInt8Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.UInt8Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(UInt16 value)
        {
            const int len = BssomBinaryPrimitives.UInt16Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.UInt16Code);
            BssomBinaryPrimitives.WriteUInt16LittleEndian(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(UInt16 value)
        {
            BssomBinaryPrimitives.WriteUInt16LittleEndian(ref BufferWriter.GetRef(BssomBinaryPrimitives.UInt16Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.UInt16Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(UInt32 value)
        {
            const int len = BssomBinaryPrimitives.UInt32Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.UInt32Code);
            BssomBinaryPrimitives.WriteUInt32LittleEndian(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(UInt32 value)
        {
            BssomBinaryPrimitives.WriteUInt32LittleEndian(ref BufferWriter.GetRef(BssomBinaryPrimitives.UInt32Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.UInt32Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(UInt64 value)
        {
            const int len = BssomBinaryPrimitives.UInt64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.UInt64Code);
            BssomBinaryPrimitives.WriteUInt64LittleEndian(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(UInt64 value)
        {
            BssomBinaryPrimitives.WriteUInt64LittleEndian(ref BufferWriter.GetRef(BssomBinaryPrimitives.UInt64Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.UInt64Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Single value)
        {
            const int len = BssomBinaryPrimitives.Float32Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.Float32Code);
            BssomBinaryPrimitives.WriteFloat32LittleEndian(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Single value)
        {
            BssomBinaryPrimitives.WriteFloat32LittleEndian(ref BufferWriter.GetRef(BssomBinaryPrimitives.Float32Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.Float32Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Double value)
        {
            const int len = BssomBinaryPrimitives.Float64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.Float64Code);
            BssomBinaryPrimitives.WriteFloat64LittleEndian(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Double value)
        {
            BssomBinaryPrimitives.WriteFloat64LittleEndian(ref BufferWriter.GetRef(BssomBinaryPrimitives.Float64Size), value);
            BufferWriter.Advance(BssomBinaryPrimitives.Float64Size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Boolean value)
        {
            const int len = BssomBinaryPrimitives.BooleanSize + BssomBinaryPrimitives.BuildInTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteBuildInType(ref refb,BssomType.BooleanCode);
            BssomBinaryPrimitives.WriteBoolean(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.BuildInTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Boolean value)
        {
            BssomBinaryPrimitives.WriteBoolean(ref BufferWriter.GetRef(BssomBinaryPrimitives.BooleanSize), value);
            BufferWriter.Advance(BssomBinaryPrimitives.BooleanSize);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Char value)
        {
            const int len = BssomBinaryPrimitives.CharSize + BssomBinaryPrimitives.NativeTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteNativeType(ref refb, NativeBssomType.CharCode);
            BssomBinaryPrimitives.WriteCharLittleEndian(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.NativeTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Char value)
        {
            BssomBinaryPrimitives.WriteCharLittleEndian(ref BufferWriter.GetRef(BssomBinaryPrimitives.CharSize), value);
            BufferWriter.Advance(BssomBinaryPrimitives.CharSize);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Decimal value)
        {
            const int len = BssomBinaryPrimitives.DecimalSize + BssomBinaryPrimitives.NativeTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteNativeType(ref refb, NativeBssomType.DecimalCode);
            BssomBinaryPrimitives.WriteDecimal(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.NativeTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Decimal value)
        {
            BssomBinaryPrimitives.WriteDecimal(ref BufferWriter.GetRef(BssomBinaryPrimitives.DecimalSize), value);
            BufferWriter.Advance(BssomBinaryPrimitives.DecimalSize);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(Guid value)
        {
            const int len = BssomBinaryPrimitives.GuidSize + BssomBinaryPrimitives.NativeTypeCodeSize;
            ref byte refb = ref BufferWriter.GetRef(len);
            BssomBinaryPrimitives.WriteNativeType(ref refb, NativeBssomType.GuidCode);
            BssomBinaryPrimitives.WriteGuid(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.NativeTypeCodeSize), value);
            BufferWriter.Advance(len);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteWithOutTypeHead(Guid value)
        {
            BssomBinaryPrimitives.WriteGuid(ref BufferWriter.GetRef(BssomBinaryPrimitives.GuidSize), value);
            BufferWriter.Advance(BssomBinaryPrimitives.GuidSize);
        }
    }
}

