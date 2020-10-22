using BssomSerializers.Binary;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
//using System.Runtime.CompilerServices;

namespace BssomSerializers
{
    /*
     * TODO: .net currently does not open ByReference<T>  type, so I cannot do partial de-virtualization optimization for IBssomBuffer. If bssom supports Span<T> in the future, then BssomReader has meaning, but at present, BssomReader is only IBssomBuffer A native call wrapper
     */

    /// <summary>
    /// <para>用于Bssom格式的写入器</para>
    /// <para>A bss types writer for the Bssom format</para>
    /// </summary>
    public readonly partial struct BssomWriter
    {
        /// <summary>
        /// Bssombuffer writer
        /// </summary>
        public readonly IBssomBufferWriter BufferWriter { get; }

        /// <summary>
        /// <para>初始化一个<see cref="BssomWriter"/>结构</para>
        /// <para>Initializes a new instance of the <see cref="BssomWriter"/> struct</para>
        /// </summary>
        /// <param name="bssomBufferWriter">用于bssom缓冲区的写入器实现. A writer implementation for the bssom buffer</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BssomWriter(IBssomBufferWriter bssomBufferWriter)
        {
            BufferWriter = bssomBufferWriter;
        }

        /// <summary>
        /// <para>缓冲区中的当前位置</para>
        /// <para>The current position within the buffer.</para>
        /// </summary>
        public long Position => BufferWriter.Position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteNativeType(byte nativeType)
        {
            ref byte refb = ref BufferWriter.GetRef(2);
            refb = BssomType.NativeCode;
            Unsafe.Add(ref refb, 1) = nativeType;
            BufferWriter.Advance(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteBuildInType(byte type)
        {
            WriteWithOutTypeHead(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteArray2Type()
        {
            WriteBuildInType(BssomType.Array2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteArray1BuildInType(byte type)
        {
            ref byte refb = ref BufferWriter.GetRef(2);
            refb = BssomType.Array1;
            Unsafe.Add(ref refb, 1) = type;
            BufferWriter.Advance(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteArray1NativeType(byte type)
        {
            ref byte refb = ref BufferWriter.GetRef(3);
            refb = BssomType.Array1;
            Unsafe.Add(ref refb, 1) = BssomType.NativeCode;
            Unsafe.Add(ref refb, 2) = type;
            BufferWriter.Advance(3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteVariableNumber(int value)
        {
            BssomBinaryPrimitives.WriteVariableNumber(BufferWriter, unchecked((ulong)value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteFixNumber(ulong value)
        {
            BssomBinaryPrimitives.WriteFixNumber(BufferWriter, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal long FillUInt32FixNumber()
        {
            long pos = Position;
            WriteUInt32FixNumber(0);
            return pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal long FillUInt16FixNumber()
        {
            long pos = Position;
            WriteUInt16FixNumber(0);
            return pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteUInt32FixNumber(uint value)
        {
            BssomBinaryPrimitives.WriteUInt32FixNumber(BufferWriter, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteUInt16FixNumber(ushort value)
        {
            BssomBinaryPrimitives.WriteUInt16FixNumber(BufferWriter, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteBackFixNumber(int value)
        {
            BssomBinaryPrimitives.WriteBackFixNumber(BufferWriter, unchecked((ulong)value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteBackFixNumber(long postion, int value)
        {
            long curPos = BufferWriter.Position;
            BufferWriter.Seek(postion, BssomSeekOrgin.Begin);
            BssomBinaryPrimitives.WriteBackFixNumber(BufferWriter, unchecked((ulong)value));
            BufferWriter.Seek(curPos, BssomSeekOrgin.Begin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteFixUInt32WithRefPos(uint value, long pos)
        {
            BufferWriter.SeekWithOutVerify(pos + 1, BssomSeekOrgin.Begin);
            BssomBinaryPrimitives.WriteUInt32LittleEndian(ref BufferWriter.GetRef(0), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteNull()
        {
            WriteBuildInType(BssomType.NullCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(string value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteBuildInType(BssomType.StringCode);
            BssomBinaryPrimitives.WriteString(BufferWriter, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(DateTime value, bool isUseStandardDateTime, bool isWriteTypeCode = true)
        {
            if (!isUseStandardDateTime && isWriteTypeCode)
            {
                const int len = BssomBinaryPrimitives.NativeDateTimeSize + BssomBinaryPrimitives.NativeTypeCodeSize;
                ref byte refb = ref BufferWriter.GetRef(len);
                BssomBinaryPrimitives.WriteNativeType(ref refb, NativeBssomType.DateTimeCode);
                BssomBinaryPrimitives.WriteNativeDateTime(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.NativeTypeCodeSize), value);
                BufferWriter.Advance(len);
            }
            else
            {
                WriteDateTimeCore(value, isUseStandardDateTime, isWriteTypeCode);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(byte[] value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }

            WriteArray1BuildInType(BssomType.UInt8Code);
            WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt8Size, value.Length));//len
            WriteVariableNumber(value.Length);
            ref byte refb = ref BufferWriter.GetRef(value.Length);
            Unsafe.CopyBlock(ref refb, ref value[0], (uint)value.Length);
            BufferWriter.Advance(value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Write(ArraySegment<byte> value)
        {
            WriteArray1BuildInType(BssomType.UInt8Code);
            WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt8Size, value.Count));//len
            WriteVariableNumber(value.Count);
            ref byte refb = ref BufferWriter.GetRef(value.Count);
            Unsafe.CopyBlock(ref refb, ref value.Array[value.Offset], (uint)value.Count);
            BufferWriter.Advance(value.Count);
        }

        private void WriteDateTimeCore(DateTime value, bool isUseStandardDateTime, bool isWriteTypeCode)
        {
            if (isUseStandardDateTime)
            {
                if (isWriteTypeCode)
                    WriteBuildInType(BssomType.DateTimeCode);
                BssomBinaryPrimitives.WriteDateTime(BufferWriter, value);
            }
            else
            {
                if (isWriteTypeCode)
                {
                    const int len = BssomBinaryPrimitives.NativeDateTimeSize + BssomBinaryPrimitives.NativeTypeCodeSize;
                    ref byte refb = ref BufferWriter.GetRef(len);
                    BssomBinaryPrimitives.WriteNativeType(ref refb, NativeBssomType.DateTimeCode);
                    BssomBinaryPrimitives.WriteNativeDateTime(ref Unsafe.Add(ref refb, BssomBinaryPrimitives.NativeTypeCodeSize), value);
                    BufferWriter.Advance(len);
                }
                else
                {
                    ref byte refb = ref BufferWriter.GetRef(BssomBinaryPrimitives.NativeDateTimeSize);
                    BssomBinaryPrimitives.WriteNativeDateTime(ref refb, value);
                    BufferWriter.Advance(BssomBinaryPrimitives.NativeDateTimeSize);
                }
            }
        }

        internal unsafe void WriteRaw64(ulong bytes, int byteCount)
        {
            ref byte refb = ref BufferWriter.GetRef(byteCount);
            if (byteCount == 8)
            {
                Unsafe.WriteUnaligned(ref refb, bytes);
            }
            else
            {
                byte* p = (byte*)&bytes;
                for (int i = 0; i < byteCount; i++)
                {
                    Unsafe.WriteUnaligned(ref Unsafe.Add(ref refb, i), *(p + i));
                }
            }
            BufferWriter.Advance(byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteRaw(byte[] value)
        {
            WriteRaw(value, 0, value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteRaw(byte[] value, int start, int len)
        {
            Unsafe.CopyBlockUnaligned(ref BufferWriter.GetRef(len), ref value[start], (uint)len);
            BufferWriter.Advance(len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BssomReader GetReader()
        {
            return new BssomReader(BufferWriter.GetBssomBuffer());
        }
    }
}
