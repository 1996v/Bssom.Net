using Bssom.Serializer.BssMap.KeyResolvers;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Internal;

//using System.Runtime.CompilerServices;

namespace Bssom.Serializer
{
    /*
     * TODO: .net currently does not open ByReference<T>  type, so I cannot do partial de-virtualization optimization for IBssomBuffer. If bssom supports Span<T> in the future, then BssomReader has meaning, but at present, BssomReader is only IBssomBuffer A native call wrapper
     */

    /// <summary>
    /// <para>用于Bssom格式的读取器</para>
    /// <para>A bss types reader for the Bssom format</para>
    /// </summary>
    public readonly partial struct BssomReader
    {
        /// <summary>
        /// Bssombuffer
        /// </summary>
        public readonly IBssomBuffer BssomBuffer { get; }

        /// <summary>
        /// <para>初始化一个<see cref="BssomReader"/>结构</para>
        /// <para>Initializes a new instance of the <see cref="BssomReader"/> struct</para>
        /// </summary>
        /// <param name="bssomBuffer">要从中读取的缓冲区 The buffer to read from. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BssomReader(IBssomBuffer bssomBuffer)
        {
            BssomBuffer = bssomBuffer;
        }

        /// <summary>
        /// <para>缓冲区中的当前位置</para>
        /// <para>The current position within the buffer.</para>
        /// </summary>
        public long Position => BssomBuffer.Position;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe T PeekOne<T>() where T : unmanaged
        {
            return Unsafe.ReadUnaligned<T>(ref BssomBuffer.ReadRef(1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe T ReadOne<T>() where T : unmanaged
        {
            T t = Unsafe.ReadUnaligned<T>(ref BssomBuffer.ReadRef(1));
            BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal byte ReadBssomType()
        {
            return ReadOne<byte>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryReadNull()
        {
            if (PeekOne<byte>() == BssomType.NullCode)
            {
                BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureMapToken(BssMapRouteToken token)
        {
            var t = ReadOne<BssMapRouteToken>();
            if (t != token)
                BssomSerializationOperationException.UnexpectedCodeRead((byte)t, Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BssMapRouteToken ReadMapToken()
        {
            return ReadOne<BssMapRouteToken>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureType(byte buildInType)
        {
            var t = ReadOne<byte>();
            if (t != buildInType)
                BssomSerializationOperationException.UnexpectedCodeRead(t, Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureTypeWithSkipBlankCharacter(byte buildInType)
        {
            var t = SkipBlankCharacterAndReadBssomType();
            if (t != buildInType)
                BssomSerializationOperationException.UnexpectedCodeRead(t, Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureNativeType(byte nativeType)
        {
            if (ReadBssomType() != BssomType.NativeCode || ReadBssomType() != nativeType)
                BssomSerializationOperationException.UnexpectedCodeRead(Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureNativeTypeWithSkipBlankCharacter(byte nativeType)
        {
            if (SkipBlankCharacterAndReadBssomType() != BssomType.NativeCode || ReadBssomType() != nativeType)
                BssomSerializationOperationException.UnexpectedCodeRead(Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SkipBlankCharacterAndEnsureType(byte buildInType)
        {
            var t = SkipBlankCharacterAndReadBssomType();
            if (t != buildInType)
                BssomSerializationOperationException.UnexpectedCodeRead(t, Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryReadNullWithEnsureBuildInType(byte buildInType)
        {
            byte type = SkipBlankCharacterAndPeekBssomType();
            if (type == BssomType.NullCode)
            {
                BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.NullSize, BssomSeekOrgin.Current);
                return true;
            }
            else if (type == buildInType)
            {
                BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.BuildInTypeCodeSize, BssomSeekOrgin.Current);
                return false;
            }
            return BssomSerializationOperationException.UnexpectedCodeRead<bool>(type, Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryReadNullWithEnsureArray1BuildInType(byte buildInType)
        {
            byte type = SkipBlankCharacterAndPeekBssomType();
            if (type == BssomType.NullCode)
            {
                BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.NullSize, BssomSeekOrgin.Current);
                return true;
            }
            else if (type == BssomType.Array1)
            {
                BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.BuildInTypeCodeSize, BssomSeekOrgin.Current);
                this.EnsureType(buildInType);
                return false;
            }
            return BssomSerializationOperationException.UnexpectedCodeRead<bool>(type, Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryReadNullWithEnsureArray1NativeType(byte nativeType)
        {
            byte type = SkipBlankCharacterAndPeekBssomType();
            if (type == BssomType.NullCode)
            {
                BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.NullSize, BssomSeekOrgin.Current);
                return true;
            }
            else if (type == BssomType.Array1)
            {
                BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.BuildInTypeCodeSize, BssomSeekOrgin.Current);
                this.EnsureNativeType(nativeType);
                return false;
            }
            return BssomSerializationOperationException.UnexpectedCodeRead<bool>(type, Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal DateTime ReadDateTime()
        {
            byte type = SkipBlankCharacterAndReadBssomType();
            switch (type)
            {
                case BssomType.TimestampCode:
                    return ReadStandDateTimeWithOutTypeHead();

                case BssomType.NativeCode:
                    EnsureType(NativeBssomType.DateTimeCode);
                    return ReadNativeDateTimeWithOutTypeHead();

                default:
                    throw BssomSerializationOperationException.UnexpectedCodeRead(type, BssomBuffer.Position);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SkipStandDateTimeWithOutTypeHead()
        {
            byte code = BssomBuffer.ReadRef(1);
            if (code == 4)
                BssomBuffer.Seek(5, BssomSeekOrgin.Current);
            else if (code == 8)
                BssomBuffer.Seek(9, BssomSeekOrgin.Current);
            else if (code == 12)
                BssomBuffer.Seek(13, BssomSeekOrgin.Current);
            else
                throw BssomSerializationOperationException.UnexpectedCodeRead(code, BssomBuffer.Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal DateTime ReadStandDateTimeWithOutTypeHead()
        {
            return BssomBinaryPrimitives.ReadDateTime(BssomBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal DateTime ReadNativeDateTimeWithOutTypeHead()
        {
            var dt = BssomBinaryPrimitives.ReadNativeDateTime(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.NativeDateTimeSize));
            BssomBuffer.Seek(BssomBinaryPrimitives.NativeDateTimeSize, BssomSeekOrgin.Current);
            return dt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string ReadString()
        {
            if (TryReadNullWithEnsureBuildInType(BssomType.StringCode))
                return default;

            return BssomBinaryPrimitives.ReadString(BssomBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string ReadStringWithOutTypeHead()
        {
            return BssomBinaryPrimitives.ReadString(BssomBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SkipBlankCharacter()
        {
            byte one = PeekOne<byte>();
            if (one > BssomType.MaxBlankCodeValue)
                return;
            BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
            SkipBlankCharacterFromBcCode(one);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal byte SkipBlankCharacterAndReadBssomType()
        {
            byte one = ReadOne<byte>();
            if (one > BssomType.MaxBlankCodeValue)
                return one;
            SkipBlankCharacterFromBcCode(one);
            return ReadOne<byte>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal byte SkipBlankCharacterAndPeekBssomType()
        {
            byte one = PeekOne<byte>();
            if (one > BssomType.MaxBlankCodeValue)
                return one;
            BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
            SkipBlankCharacterFromBcCode(one);
            return PeekOne<byte>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SkipBlankCharacterFromBcCode(byte bcCode)
        {
            if (bcCode < BssomType.BlankUInt16Code)
                BssomBuffer.Seek(bcCode, BssomSeekOrgin.Current);
            else if (bcCode == BssomType.BlankUInt16Code)
                BssomBuffer.Seek(ReadUInt16WithOutTypeHead(), BssomSeekOrgin.Current);
            else//(bcCode == BssomType.EmptyUInt32Code)
                BssomBuffer.Seek(ReadUInt32WithOutTypeHead(), BssomSeekOrgin.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SkipVariableNumber()
        {
            BssomBinaryPrimitives.SkipVariableNumber(BssomBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int ReadVariableNumber()
        {
            return (int)BssomBinaryPrimitives.ReadVariableNumber(BssomBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetMap1KeyLength()
        {
            byte keyType = ReadBssomType();

            if (keyType == BssomType.StringCode)
            {
                return (int)BssomBinaryPrimitives.ReadVariableNumber(BssomBuffer);
            }
            else if (keyType == BssomType.NativeCode)
            {
                keyType = ReadBssomType();
                if (BssomBinaryPrimitives.TryGetNativeTypeSizeFromStaticTypeSizes(keyType, out int size))
                    return size;
            }
            else if (keyType != BssomType.NullCode)
            {
                if (BssomBinaryPrimitives.TryGetPrimitiveTypeSizeFromStaticTypeSizes(keyType, out int size))
                    return size;
            }
            return BssomSerializationOperationException.UnexpectedCodeRead<int>(keyType, Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetMapStringKeyLength()
        {
            byte keyType = ReadBssomType();
            if (keyType == BssomType.StringCode)
                return (int)BssomBinaryPrimitives.ReadVariableNumber(BssomBuffer);
            return BssomSerializationOperationException.UnexpectedCodeRead<int>(keyType, Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SkipObject(byte typeCode)
        {
            if (typeCode <= BssomType.MaxBlankCodeValue)
            {
                if (typeCode <= BssomType.MaxVarBlankCodeValue)
                    BssomBuffer.Seek(typeCode, BssomSeekOrgin.Current);
                else
                {
                    if (typeCode == BssomType.BlankUInt16Code)
                        BssomBuffer.Seek(ReadUInt16WithOutTypeHead(), BssomSeekOrgin.Current);
                    else //if (type == BssomType.EmptyInt32Code)
                        BssomBuffer.Seek(ReadUInt32WithOutTypeHead(), BssomSeekOrgin.Current);
                }
                typeCode = ReadBssomType();
            }

            BssomBuffer.Seek(GetRemainingLengthOfObject(typeCode), BssomSeekOrgin.Current);
        }

        internal int GetObjectLengthWithBlank()
        {
            var type = ReadBssomType();
            int len = 1;
            if (type <= BssomType.MaxBlankCodeValue)
            {
                int empty = 0;
                if (type <= BssomType.MaxVarBlankCodeValue)
                {
                    empty = type;
                    BssomBuffer.Seek(type, BssomSeekOrgin.Current);
                    empty += 1;
                }
                else
                {
                    if (type == BssomType.BlankUInt16Code)
                    {
                        empty = ReadUInt16WithOutTypeHead();
                        BssomBuffer.Seek(empty, BssomSeekOrgin.Current);
                        empty += 2;
                    }
                    else /*if (type == BssomType.EmptyInt32Code)*/
                    {
                        empty = checked((int)ReadUInt32WithOutTypeHead());
                        BssomBuffer.Seek(empty, BssomSeekOrgin.Current);
                        empty += 4;
                    }
                }
                type = ReadBssomType();
                len += empty;
            }

            long pos = Position;
            len += GetRemainingLengthOfObject(type);
            len += (int)(Position - pos);
            return len;
        }

        internal int GetRemainingLengthOfObject(byte objType)
        {
            if (BssomBinaryPrimitives.TryGetPrimitiveTypeSizeFromStaticTypeSizes(objType, out int size))
                return size;

            switch (objType)
            {
                case BssomType.StringCode:
                    return (int)BssomBinaryPrimitives.ReadVariableNumber(BssomBuffer);
                case BssomType.NativeCode:
                    {
                        objType = ReadBssomType();
                        if (objType == NativeBssomType.CharCode)
                            return BssomBinaryPrimitives.CharSize;
                        else if (objType == NativeBssomType.DateTimeCode)
                            return BssomBinaryPrimitives.NativeDateTimeSize;
                        else if (objType == NativeBssomType.DecimalCode)
                            return BssomBinaryPrimitives.DecimalSize;
                        else if (objType == NativeBssomType.GuidCode)
                            return BssomBinaryPrimitives.GuidSize;
                        throw BssomSerializationOperationException.UnexpectedCodeRead(objType, BssomBuffer.Position);
                    }
                case BssomType.Map1:
                case BssomType.Map2:
                case BssomType.Array2:
                    return ReadVariableNumber();

                case BssomType.Array1:
                    {
                        if (ReadBssomType()/*elementType*/ == BssomType.NativeCode)
                            BssomBuffer.Seek(BssomBinaryPrimitives.BuildInTypeCodeSize, BssomSeekOrgin.Current);
                        return ReadVariableNumber();
                    }

                default:
                    throw BssomSerializationOperationException.UnexpectedCodeRead(objType, BssomBuffer.Position);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SkipObject()
        {
            SkipObject(ReadBssomType());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe ulong ReadRaw64(ref int remaining)
        {
            ulong val = 0;
            if (remaining > 8)
            {
                val = Unsafe.ReadUnaligned<ulong>(ref BssomBuffer.ReadRef(8));
                BssomBuffer.SeekWithOutVerify(8, BssomSeekOrgin.Current);
                remaining -= 8;
            }
            else
            {
                ref byte refb = ref BssomBuffer.ReadRef(remaining);
                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref val), ref refb, (uint)remaining);
                BssomBuffer.SeekWithOutVerify(remaining, BssomSeekOrgin.Current);
                remaining = 0;
            }
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe ulong ReadRaw64(int count)
        {
            DEBUG.Assert(count <= 8);
            ulong val = 0;
            ref byte refb = ref BssomBuffer.ReadRef(count);
            Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref val), ref refb, (uint)count);
            BssomBuffer.SeekWithOutVerify(count, BssomSeekOrgin.Current);
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SeekAndSkipObject(int count)
        {
            BssomBuffer.Seek(count, BssomSeekOrgin.Current);
            SkipObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal byte[] ReadBytes()
        {
            if (TryReadNullWithEnsureArray1BuildInType(BssomType.UInt8Code))
                return null;
            SkipVariableNumber();
            byte[] val = new byte[ReadVariableNumber()];
            ref byte refb = ref BssomBuffer.ReadRef(val.Length);
            Unsafe.CopyBlock(ref val[0], ref refb, (uint)val.Length);
            BssomBuffer.SeekWithOutVerify(val.Length, BssomSeekOrgin.Current);
            return val;
        }
    }
}
