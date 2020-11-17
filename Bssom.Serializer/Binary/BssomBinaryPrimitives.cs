
using Bssom.Serializer.Internal;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bssom.Serializer.Binary
{
    /*
     * TODO: The next code refactoring will split BssomBinaryPrimitives into BssomValue type.
     */

    //variable length write/read
    public static partial class BssomBinaryPrimitives
    {
        internal const int VariableUInt8Max = 0xfa;
        internal const int VariableUInt9Max = VariableUInt8Max + 0xff;

        internal const int VariableUInt9 = 0xfb;
        internal const int FixUInt8 = 0xfc;
        internal const int FixUInt16 = 0xfd;
        internal const int FixUInt32 = 0xfe;
        internal const int FixUInt64 = 0xff;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteVariableNumber(IBssomBufferWriter writer, ulong value)
        {
            if (value <= VariableUInt8Max)
            {
                ref byte refb = ref writer.GetRef(1);
                refb = unchecked((byte)value);
                writer.Advance(1);
            }
            else
            {
                WriteVariableNumberCore(writer, value);
            }
        }

        public static int WriteVariableNumber(ref byte refb, ulong value)
        {
            if (value <= VariableUInt8Max)
            {
                refb = unchecked((byte)value);
                return 1;
            }
            else if (value <= VariableUInt9Max)
            {
                refb = VariableUInt9;
                Unsafe.Add(ref refb, 1) = unchecked((byte)(value - VariableUInt8Max));
                return 2;
            }
            else if (value <= ushort.MaxValue)
            {
                refb = FixUInt16;
                WriteUInt16LittleEndian(ref Unsafe.Add(ref refb, 1), (ushort)value);
                return 3;
            }
            else if (value <= uint.MaxValue)
            {
                refb = FixUInt32;
                WriteUInt32LittleEndian(ref Unsafe.Add(ref refb, 1), (uint)value);
                return 5;
            }
            else
            {
                refb = FixUInt64;
                WriteUInt64LittleEndian(ref Unsafe.Add(ref refb, 1), value);
                return 9;
            }
        }

        private static void WriteVariableNumberCore(IBssomBufferWriter writer, ulong value)
        {
            if (value <= VariableUInt8Max)
            {
                ref byte refb = ref writer.GetRef(1);
                refb = unchecked((byte)value);
                writer.Advance(1);
            }
            else if (value <= VariableUInt9Max)
            {
                ref byte refb = ref writer.GetRef(2);
                refb = VariableUInt9;
                Unsafe.Add(ref refb, 1) = unchecked((byte)(value - VariableUInt8Max));
                writer.Advance(2);
            }
            else if (value <= ushort.MaxValue)
            {
                ref byte refb = ref writer.GetRef(3);
                refb = FixUInt16;
                WriteUInt16LittleEndian(ref Unsafe.Add(ref refb, 1), (ushort)value);
                writer.Advance(3);
            }
            else if (value <= uint.MaxValue)
            {
                ref byte refb = ref writer.GetRef(5);
                refb = FixUInt32;
                WriteUInt32LittleEndian(ref Unsafe.Add(ref refb, 1), (uint)value);
                writer.Advance(5);
            }
            else
            {
                ref byte refb = ref writer.GetRef(9);
                refb = FixUInt64;
                WriteUInt64LittleEndian(ref Unsafe.Add(ref refb, 1), value);
                writer.Advance(9);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixNumber(IBssomBufferWriter writer, ulong value)
        {
            if (value <= byte.MaxValue)
            {
                ref byte refb = ref writer.GetRef(2);
                refb = FixUInt8;
                Unsafe.Add(ref refb, 1) = unchecked((byte)value);
                writer.Advance(2);
            }
            else
            {
                WriteFixNumberCore(writer, value);
            }
        }

        public static void WriteFixNumberCore(IBssomBufferWriter writer, ulong value)
        {
            if (value <= byte.MaxValue)
            {
                ref byte refb = ref writer.GetRef(2);
                refb = FixUInt8;
                Unsafe.Add(ref refb, 1) = unchecked((byte)value);
                writer.Advance(2);
            }
            else if (value <= ushort.MaxValue)
            {
                ref byte refb = ref writer.GetRef(3);
                refb = FixUInt16;
                WriteUInt16LittleEndian(ref Unsafe.Add(ref refb, 1), (ushort)value);
                writer.Advance(3);
            }
            else if (value <= uint.MaxValue)
            {
                ref byte refb = ref writer.GetRef(5);
                refb = FixUInt32;
                WriteUInt32LittleEndian(ref Unsafe.Add(ref refb, 1), (uint)value);
                writer.Advance(5);
            }
            else
            {
                ref byte refb = ref writer.GetRef(9);
                refb = FixUInt64;
                WriteUInt64LittleEndian(ref Unsafe.Add(ref refb, 1), value);
                writer.Advance(9);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32FixNumber(IBssomBufferWriter writer, uint value)
        {
            ref byte refb = ref writer.GetRef(5);
            WriteUInt32FixNumber(ref refb, value);
            writer.Advance(5);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteUInt32FixNumber(ref byte refb, uint value)
        {
            refb = FixUInt32;
            WriteUInt32LittleEndian(ref Unsafe.Add(ref refb, 1), (uint)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16FixNumber(IBssomBufferWriter writer, ushort value)
        {
            ref byte refb = ref writer.GetRef(3);
            refb = FixUInt16;
            WriteUInt16LittleEndian(ref Unsafe.Add(ref refb, 1), value);
            writer.Advance(3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBackFixNumber(IBssomBufferWriter writer, ulong value)
        {
            ref byte refb = ref writer.GetRef(1);
            DEBUG.Assert(refb > VariableUInt9);
            writer.Advance(1);

            switch (refb)
            {
                case FixUInt8:
                    {
                        refb = ref writer.GetRef(1);
                        refb = unchecked((byte)value);
                        writer.Advance(1);
                        break;
                    }
                case FixUInt16:
                    {
                        refb = ref writer.GetRef(2);
                        WriteUInt16LittleEndian(ref refb, (ushort)value);
                        writer.Advance(2);
                        break;
                    }
                case FixUInt32:
                    {
                        refb = ref writer.GetRef(4);
                        WriteUInt32LittleEndian(ref refb, (uint)value);
                        writer.Advance(4);
                        break;
                    }
                case FixUInt64:
                    {
                        refb = ref writer.GetRef(8);
                        WriteUInt64LittleEndian(ref refb, (uint)value);
                        writer.Advance(8);
                        break;
                    }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDateTime(IBssomBufferWriter writer, DateTime value)
        {
            if (value.Kind == DateTimeKind.Local)
            {
                value = value.ToUniversalTime();
            }

            long utcTicks = value.Ticks - DateTimeConstants.UnixSeconds;
            long utcSeconds = utcTicks / TimeSpan.TicksPerSecond;
            long utcNanoseconds = utcTicks % TimeSpan.TicksPerSecond;

            ref byte destination = ref writer.GetRef(StandardDateTimeSize);
            WriteInt64LittleEndian(ref destination, utcSeconds);
            WriteUInt32LittleEndian(ref Unsafe.Add(ref destination, 8), (uint)utcNanoseconds);
            writer.Advance(StandardDateTimeSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void WriteStringWithNotPredictingLength(IBssomBufferWriter writer, string value, long writeBackExactStringCountQuantityPos)
        {
            DEBUG.Assert(value != null);

            int len = UTF8Encoding.UTF8.GetByteCount(value);
            writer.Seek(writeBackExactStringCountQuantityPos, BssomSeekOrgin.Begin);//Simulate StringSize, consistent with WriteString behavior
            WriteBackFixNumber(writer, (uint)len);
            ref byte refb = ref writer.GetRef(len);
            fixed (char* pValue = value)
            fixed (byte* pRefb = &refb)
            {
                UTF8Encoding.UTF8.GetBytes(pValue, value.Length, pRefb, len);
            }
            writer.Advance(len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteString(IBssomBufferWriter writer, string value)
        {
            DEBUG.Assert(value != null);

            int maxSize = UTF8Encoding.UTF8.GetMaxByteCount(value.Length);
            long pos = writer.Position;
            WriteFixNumber(writer, (uint)maxSize);

            if (!writer.CanGetSizeRefForProvidePerformanceInTryWrite(maxSize))
            {
                WriteStringWithNotPredictingLength(writer, value, pos);
                return;
            }

            int count = 0;
            ref byte refb = ref writer.GetRef(maxSize);
            fixed (char* pValue = value)
            fixed (byte* pRefb = &refb)
            {
                count = UTF8Encoding.UTF8.GetBytes(pValue, value.Length, pRefb, maxSize);
            }
            writer.Seek(pos, BssomSeekOrgin.Begin);
            WriteBackFixNumber(writer, (uint)count);
            writer.Advance(count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SkipVariableNumber(IBssomBuffer reader)
        {
            byte code = reader.ReadRef(1);
            reader.SeekWithOutVerify(1, BssomSeekOrgin.Current);
            if (code <= VariableUInt8Max)
            {
                return;
            }
            else if (code == VariableUInt9 || code == FixUInt8)
            {
                reader.Seek(1, BssomSeekOrgin.Current);
            }
            else if (code == FixUInt16)
            {
                reader.Seek(2, BssomSeekOrgin.Current);
            }
            else if (code == FixUInt32)
            {
                reader.Seek(4, BssomSeekOrgin.Current);
            }
            else /*if (code == FixUInt64)*/
            {
                reader.Seek(8, BssomSeekOrgin.Current);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int VariableNumberSizeByCode(int code)
        {
            if (code <= VariableUInt8Max)
            {
                return 1;
            }
            else if (code == VariableUInt9 || code == FixUInt8)
            {
                return 1 + 1;
            }
            else if (code == FixUInt16)
            {
                return 1 + 2;
            }
            else if (code == FixUInt32)
            {
                return 1 + 4;
            }
            else /*if (code == FixUInt64)*/
            {
                return 1 + 8;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadVariableNumber(IBssomBuffer reader)
        {
            byte code = reader.ReadRef(1);
            reader.SeekWithOutVerify(1, BssomSeekOrgin.Current);
            if (code <= VariableUInt8Max)
            {
                return code;
            }

            return ReadVariableNumberCore(code, reader);
        }

        private static ulong ReadVariableNumberCore(byte code, IBssomBuffer reader)
        {
            ulong val;
            switch (code)
            {
                case VariableUInt9:
                    {
                        val = (ulong)(VariableUInt8Max + reader.ReadRef(1));
                        reader.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                        break;
                    }

                case FixUInt8:
                    {
                        val = reader.ReadRef(1);
                        reader.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                        break;
                    }
                case FixUInt16:
                    {
                        val = ReadUInt16LittleEndian(ref reader.ReadRef(2));
                        reader.SeekWithOutVerify(2, BssomSeekOrgin.Current);
                        break;
                    }

                case FixUInt32:
                    {
                        val = ReadUInt32LittleEndian(ref reader.ReadRef(4));
                        reader.SeekWithOutVerify(4, BssomSeekOrgin.Current);
                        break;
                    }

                case FixUInt64:
                    {
                        val = ReadUInt64LittleEndian(ref reader.ReadRef(8));
                        reader.SeekWithOutVerify(8, BssomSeekOrgin.Current);
                        break;
                    }
                default:
                    return code;
            }
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadFixNumber(IBssomBuffer reader)
        {
            ulong val;
            byte code = reader.ReadRef(1);
            reader.SeekWithOutVerify(1, BssomSeekOrgin.Current);
            if (code == FixUInt8)
            {
                val = reader.ReadRef(1);
                reader.SeekWithOutVerify(1, BssomSeekOrgin.Current);
            }
            else if (code == FixUInt16)
            {
                val = ReadUInt16LittleEndian(ref reader.ReadRef(2));
                reader.SeekWithOutVerify(2, BssomSeekOrgin.Current);
            }
            else if (code == FixUInt32)
            {
                val = ReadUInt32LittleEndian(ref reader.ReadRef(4));
                reader.SeekWithOutVerify(4, BssomSeekOrgin.Current);
            }
            else if (code == FixUInt64)
            {
                val = ReadUInt64LittleEndian(ref reader.ReadRef(8));
                reader.SeekWithOutVerify(8, BssomSeekOrgin.Current);
            }
            else
            {
                return BssomSerializationOperationException.UnexpectedCodeRead<ulong>(code, reader.Position);
            }
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string ReadString(IBssomBuffer reader)
        {
            ulong len = ReadVariableNumber(reader);
            if (len == 0)
            {
                return string.Empty;
            }

            ref byte refb = ref reader.ReadRef((int)len);
            fixed (byte* pRefb = &refb)
            {
#if NET45
                string str = new string((sbyte*)pRefb, 0, (int)len, UTF8Encoding.UTF8);
#else
                var str = UTF8Encoding.UTF8.GetString(pRefb, (int)len);
#endif
                reader.SeekWithOutVerify((int)len, BssomSeekOrgin.Current);
                return str;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ReadDateTime(IBssomBuffer reader)
        {
            ref byte refb = ref reader.ReadRef(StandardDateTimeSize);
            long utcSeconds = ReadInt64LittleEndian(ref refb);
            uint utcNanoseconds = ReadUInt32LittleEndian(ref Unsafe.Add(ref refb, 8));
            reader.SeekWithOutVerify(StandardDateTimeSize, BssomSeekOrgin.Current);
            return DateTimeConstants.UnixEpoch.AddSeconds(utcSeconds).AddTicks(utcNanoseconds);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DateTimeSize(bool isStandardDate)
        {
            if (isStandardDate)
            {
                return StandardDateTimeSize + BuildInTypeCodeSize;
            }
            else
            {
                return NativeDateTimeSize + NativeTypeCodeSize;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StringSize(string value)
        {
            int size = UTF8Encoding.UTF8.GetByteCount(value);
            return FixNumberSize((ulong)UTF8Encoding.UTF8.GetMaxByteCount(value.Length)) + size;
        }
    }

    //base
    public static partial class BssomBinaryPrimitives
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReverseEndianness(short value)
        {
            return (short)ReverseEndianness((ushort)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReverseEndianness(int value)
        {
            return (int)ReverseEndianness((uint)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReverseEndianness(long value)
        {
            return (long)ReverseEndianness((ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReverseEndianness(ushort value)
        {
            // Don't need to AND with 0xFF00 or 0x00FF since the final
            // cast back to ushort will clear out all bits above [ 15 .. 00 ].
            // This is normally implemented via "movzx eax, ax" on the return.
            // Alternatively, the compiler could elide the movzx instruction
            // entirely if it knows the caller is only going to access "ax"
            // instead of "eax" / "rax" when the function returns.

            return (ushort)((value >> 8) + (value << 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReverseEndianness(uint value)
        {
            // This takes advantage of the fact that the JIT can detect
            // ROL32 / ROR32 patterns and output the Correctly intrinsic.
            //
            // Input: value = [ ww xx yy zz ]
            //
            // First line generates : [ ww xx yy zz ]
            //                      & [ 00 FF 00 FF ]
            //                      = [ 00 xx 00 zz ]
            //             ROR32(8) = [ zz 00 xx 00 ]
            //
            // Second line generates: [ ww xx yy zz ]
            //                      & [ FF 00 FF 00 ]
            //                      = [ ww 00 yy 00 ]
            //             ROL32(8) = [ 00 yy 00 ww ]
            //
            //                (sum) = [ zz yy xx ww ]
            //
            // Testing shows that throughput increases if the AND
            // is performed before the ROL / ROR.

            return BitOperations.RotateRight(value & 0x00FF00FFu, 8) // xx zz
                + BitOperations.RotateLeft(value & 0xFF00FF00u, 8); // ww yy
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReverseEndianness(ulong value)
        {
            // Operations on 32-bit values have higher throughput than
            // operations on 64-bit values, so decompose.

            return ((ulong)ReverseEndianness((uint)value) << 32)
                + ReverseEndianness((uint)(value >> 32));
        }
    }

    //write
    public static partial class BssomBinaryPrimitives
    {
        internal static class DateTimeConstants
        {
            internal const long UnixSeconds = 621355968000000000;
            internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBuildInType(ref byte destination, byte buildInType)
        {
            WriteUInt8(ref destination, buildInType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNativeType(ref byte destination, byte nativeType)
        {
            WriteUInt8(ref destination, BssomType.NativeCode);
            WriteUInt8(ref Unsafe.Add(ref destination, 1), nativeType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBoolean(ref byte destination, bool value)
        {
            Unsafe.WriteUnaligned(ref destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt8(ref byte destination, sbyte value)
        {
            Unsafe.WriteUnaligned(ref destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16LittleEndian(ref byte destination, short value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                value = ReverseEndianness(value);
            }

            Unsafe.WriteUnaligned(ref destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32LittleEndian(ref byte destination, int value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                value = ReverseEndianness(value);
            }

            Unsafe.WriteUnaligned(ref destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64LittleEndian(ref byte destination, long value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                value = ReverseEndianness(value);
            }

            Unsafe.WriteUnaligned(ref destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt8(ref byte destination, byte value)
        {
            Unsafe.WriteUnaligned(ref destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16LittleEndian(ref byte destination, ushort value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                value = ReverseEndianness(value);
            }
            Unsafe.WriteUnaligned(ref destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32LittleEndian(ref byte destination, uint value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                value = ReverseEndianness(value);
            }
            Unsafe.WriteUnaligned(ref destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64LittleEndian(ref byte destination, ulong value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                value = ReverseEndianness(value);
            }
            Unsafe.WriteUnaligned(ref destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteFloat32LittleEndian(ref byte destination, float value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                int tmp = ReverseEndianness(*((int*)&value));
                Unsafe.WriteUnaligned(ref destination, tmp);
            }
            else
            {
                Unsafe.WriteUnaligned(ref destination, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteFloat64LittleEndian(ref byte destination, double value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                long tmp = ReverseEndianness(BitConverter.DoubleToInt64Bits(value));
                Unsafe.WriteUnaligned(ref destination, tmp);
            }
            else
            {
                Unsafe.WriteUnaligned(ref destination, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCharLittleEndian(ref byte destination, char value)
        {
            WriteUInt16LittleEndian(ref destination, (ushort)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteGuid(ref byte destination, Guid value)
        {
            GuidBinaryBits.GetGuidBinaryBits(value).Write(ref destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDecimal(ref byte destination, decimal value)
        {
            NativeDecimalGetterHelper.GetPack(value).Write(ref destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNativeDateTime(ref byte destination, DateTime value)
        {
            WriteInt64LittleEndian(ref destination, value.ToBinary());
        }
    }

    //reade
    public static partial class BssomBinaryPrimitives
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(ref byte destination)
        {
            return Unsafe.ReadUnaligned<bool>(ref destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadInt8(ref byte destination)
        {
            return Unsafe.ReadUnaligned<sbyte>(ref destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16LittleEndian(ref byte destination)
        {
            short result = Unsafe.ReadUnaligned<short>(ref destination);
            if (!BitConverter.IsLittleEndian)
            {
                result = ReverseEndianness(result);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32LittleEndian(ref byte destination)
        {
            int result = Unsafe.ReadUnaligned<int>(ref destination);
            if (!BitConverter.IsLittleEndian)
            {
                result = ReverseEndianness(result);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64LittleEndian(ref byte destination)
        {
            long result = Unsafe.ReadUnaligned<long>(ref destination);
            if (!BitConverter.IsLittleEndian)
            {
                result = ReverseEndianness(result);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadUInt8(ref byte destination)
        {
            return Unsafe.ReadUnaligned<byte>(ref destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16LittleEndian(ref byte destination)
        {
            ushort result = Unsafe.ReadUnaligned<ushort>(ref destination);
            if (!BitConverter.IsLittleEndian)
            {
                result = ReverseEndianness(result);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32LittleEndian(ref byte destination)
        {
            uint result = Unsafe.ReadUnaligned<uint>(ref destination);
            if (!BitConverter.IsLittleEndian)
            {
                result = ReverseEndianness(result);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64LittleEndian(ref byte destination)
        {
            ulong result = Unsafe.ReadUnaligned<ulong>(ref destination);
            if (!BitConverter.IsLittleEndian)
            {
                result = ReverseEndianness(result);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadRawUInt64LittleEndian(ulong rawUlong)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return ReverseEndianness(rawUlong);
            }

            return rawUlong;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float ReadFloat32LittleEndian(ref byte destination)
        {
            if (!BitConverter.IsLittleEndian)
            {
                int tm = ReverseEndianness(Unsafe.ReadUnaligned<int>(ref destination));
                return *(float*)&tm;
            }
            return Unsafe.ReadUnaligned<float>(ref destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadFloat64LittleEndian(ref byte destination)
        {
            return !BitConverter.IsLittleEndian ?
                BitConverter.Int64BitsToDouble(ReverseEndianness(Unsafe.ReadUnaligned<long>(ref destination))) :
                Unsafe.ReadUnaligned<double>(ref destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadCharLittleEndian(ref byte destination)
        {
            return (char)(ReadInt16LittleEndian(ref destination));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid ReadGuid(ref byte destination)
        {
            return new GuidBinaryBits(ref destination).ToGuid();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimal(ref byte destination)
        {
            return DecimalBinaryBits.Read(ref destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ReadNativeDateTime(ref byte destination)
        {
            return DateTime.FromBinary(ReadInt64LittleEndian(ref destination));
        }

    }

    //size
    public static partial class BssomBinaryPrimitives
    {
        public const int BooleanSize = 1;
        public const int Int8Size = 1;
        public const int Int16Size = 2;
        public const int Int32Size = 4;
        public const int Int64Size = 8;
        public const int UInt8Size = 1;
        public const int UInt16Size = 2;
        public const int UInt32Size = 4;
        public const int UInt64Size = 8;
        public const int Float32Size = 4;
        public const int Float64Size = 8;
        public const int StandardDateTimeSize = 12;

        public const int CharSize = 2;
        public const int GuidSize = GuidBinaryBits.Size;
        public const int DecimalSize = DecimalBinaryBits.Size;
        public const int NativeDateTimeSize = 8;

        public const int NullSize = 1;

        public const int BuildInTypeCodeSize = 1;
        public const int NativeTypeCodeSize = 2;


        public const int Array1BuildInTypeCodeSize = BuildInTypeCodeSize + BuildInTypeCodeSize;
        public const int Array1NativeTypeCodeSize = BuildInTypeCodeSize + NativeTypeCodeSize;
        public const int Array2TypeCodeSize = 1;

        public const int FixUInt32NumberSize = 1 + UInt32Size;
        public const int FixUInt16NumberSize = 1 + UInt16Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int VariableNumberSize(ulong value)
        {
            if (value <= VariableUInt8Max)
            {
                return 1;
            }
            else if (value <= VariableUInt9Max)
            {
                return 2;
            }
            else if (value <= ushort.MaxValue)
            {
                return 3;
            }
            else if (value <= uint.MaxValue)
            {
                return 5;
            }
            else
            {
                return 9;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FixNumberSize(ulong value)
        {
            if (value <= byte.MaxValue)
            {
                return 2;
            }
            else if (value <= ushort.MaxValue)
            {
                return 3;
            }
            else if (value <= uint.MaxValue)
            {
                return 5;
            }
            else
            {
                return 9;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NativeDateTimeArraySize(int count)
        {
            return Array1NativeTypeCodeSize + FixUInt32NumberSize + VariableNumberSize((ulong)count) + checked(count * NativeDateTimeSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StandardDateTimeArraySize(int count)
        {
            return Array1BuildInTypeCodeSize + FixUInt32NumberSize + VariableNumberSize((ulong)count) + checked(count * StandardDateTimeSize);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StandardDateTimeArraySizeWithOutCount(int count)
        {
            return Array1BuildInTypeCodeSize + FixUInt32NumberSize + FixUInt32NumberSize + checked(count * StandardDateTimeSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NativeDateTimeArraySizeWithOutCount(int count)
        {
            return Array1NativeTypeCodeSize + FixUInt32NumberSize + FixUInt32NumberSize + checked(count * NativeDateTimeSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array2TypeSize(int count, long dataLen)
        {
            //Array2Type,len,count,dataLen
            return Array2TypeCodeSize + FixUInt32NumberSize + VariableNumberSize((ulong)count) + checked((int)dataLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array2TypeSizeWithFixU32Count(long dataLen)
        {
            return Array2TypeCodeSize + FixUInt32NumberSize + FixUInt32NumberSize + checked((int)dataLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array1BuildInTypeWithNeedFillCount(int elementSize, int count)
        {
            return checked(Array1BuildInTypeCodeSize + FixUInt32NumberSize + FixUInt32NumberSize + (elementSize * count));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array1NativeTypeWithNeedFillCount(int elementSize, int count)
        {
            return checked(Array1NativeTypeCodeSize + FixUInt32NumberSize + FixUInt32NumberSize + (elementSize * count));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ArrayTypeSizeWithOutTypeHead(int count, long dataLen)
        {
            //len,count,dataLen
            int countWidth = VariableNumberSize((ulong)count);
            int len = VariableNumberSize((ulong)checked((int)(countWidth + dataLen)));
            return VariableNumberSize((ulong)len) + len;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array1TypeSizeWithOutData(int elementSize, int count)
        {
            //type,len,count,data
            int dataSize = checked(elementSize * count);
            int countSize = VariableNumberSize((ulong)count);
            int lenSize = VariableNumberSize((ulong)(countSize + dataSize));
            return BuildInTypeCodeSize + lenSize + countSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array1TypeSizeWithOutTypeHeadAndLength(int elementSize, int count)
        {
            //count+data
            int len = VariableNumberSize((ulong)count);
            checked { len += (elementSize * count); }
            return len;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array1TypeSizeWithOutTypeHead(int elementSize, int count)
        {
            //len+count+data
            int len = Array1TypeSizeWithOutTypeHeadAndLength(elementSize, count);
            return VariableNumberSize((ulong)len) + len;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array1BuildInTypeSize(int elementSize, int count)
        {
            return Array1BuildInTypeCodeSize + Array1TypeSizeWithOutTypeHead(elementSize, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array1NativeTypeSize(int elementSize, int count)
        {
            return Array1NativeTypeCodeSize + Array1TypeSizeWithOutTypeHead(elementSize, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Array3HeaderSize(int count)
        {
            //typeCode + len + Varlen(count) +  BssomBinaryPrimitives.FixUInt32NumberSize * count
            return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.FixUInt32NumberSize + BssomBinaryPrimitives.VariableNumberSize((ulong)count) + BssomBinaryPrimitives.FixUInt32NumberSize * count;
        }

        private static readonly int[] StaticPrimitiveTypeSizes = new int[] { BssomBinaryPrimitives.NullSize, BssomBinaryPrimitives.Int8Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.Int16Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.Int32Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.Int64Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.UInt8Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.UInt16Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.UInt32Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.UInt64Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.Float32Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.Float64Size + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.BooleanSize + BssomBinaryPrimitives.BuildInTypeCodeSize, BssomBinaryPrimitives.StandardDateTimeSize + BssomBinaryPrimitives.BuildInTypeCodeSize, };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetPrimitiveTypeSizeFromStaticTypeSizes(byte type, out int size)
        {
            if (type >= BssomType.MinFixLenTypeCode && type <= BssomType.MaxFixLenTypeCode)
            {
                size = StaticPrimitiveTypeSizes[type - BssomType.MinFixLenTypeCode];
                return true;
            }
            size = default;
            return false;
        }

        private static readonly sbyte[] StaticNativeTypeSizes;

        static BssomBinaryPrimitives()
        {
            StaticNativeTypeSizes = new sbyte[NativeBssomType.MaxCode + 1];
            StaticNativeTypeSizes[NativeBssomType.CharCode] = BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.CharSize;
            StaticNativeTypeSizes[NativeBssomType.GuidCode] = BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.GuidSize;
            StaticNativeTypeSizes[NativeBssomType.DecimalCode] = BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.DecimalSize;
            StaticNativeTypeSizes[NativeBssomType.DateTimeCode] = BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.NativeDateTimeSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetTypeSizeFromStaticTypeSizes(bool isNativeType, byte type, out int size)
        {
            if (!isNativeType)
            {
                if (type >= BssomType.MinFixLenTypeCode && type <= BssomType.MaxFixLenTypeCode)
                {
                    size = StaticPrimitiveTypeSizes[type - BssomType.MinFixLenTypeCode];
                    return true;
                }
                size = default;
                return false;
            }
            else
            {
                return TryGetNativeTypeSizeFromStaticTypeSizes(type, out size);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNativeTypeSizeFromStaticTypeSizes(byte type, out int size)
        {
            if (type <= NativeBssomType.MaxCode)
            {
                size = StaticNativeTypeSizes[type];
                if (size != 0)
                {
                    return true;
                }
            }
            size = default;
            return false;
        }
    }
}
