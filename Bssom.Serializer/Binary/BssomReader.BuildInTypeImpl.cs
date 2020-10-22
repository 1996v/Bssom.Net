using Bssom.Serializer.Internal;
using Bssom.Serializer.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace Bssom.Serializer
{
    //BuildInTypeCode
    public partial struct BssomReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SByte ReadInt8()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.Int8Code);
            var val = BssomBinaryPrimitives.ReadInt8(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Int8Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Int8Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SByte ReadInt8WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadInt8(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Int8Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Int8Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Int16 ReadInt16()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.Int16Code);
            var val = BssomBinaryPrimitives.ReadInt16LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Int16Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Int16Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Int16 ReadInt16WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadInt16LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Int16Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Int16Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Int32 ReadInt32()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.Int32Code);
            var val = BssomBinaryPrimitives.ReadInt32LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Int32Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Int32Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Int32 ReadInt32WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadInt32LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Int32Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Int32Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Int64 ReadInt64()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.Int64Code);
            var val = BssomBinaryPrimitives.ReadInt64LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Int64Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Int64Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Int64 ReadInt64WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadInt64LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Int64Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Int64Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Byte ReadUInt8()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.UInt8Code);
            var val = BssomBinaryPrimitives.ReadUInt8(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.UInt8Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.UInt8Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Byte ReadUInt8WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadUInt8(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.UInt8Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.UInt8Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UInt16 ReadUInt16()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.UInt16Code);
            var val = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.UInt16Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.UInt16Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UInt16 ReadUInt16WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.UInt16Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.UInt16Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UInt32 ReadUInt32()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.UInt32Code);
            var val = BssomBinaryPrimitives.ReadUInt32LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.UInt32Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.UInt32Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UInt32 ReadUInt32WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadUInt32LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.UInt32Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.UInt32Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UInt64 ReadUInt64()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.UInt64Code);
            var val = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.UInt64Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.UInt64Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UInt64 ReadUInt64WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.UInt64Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.UInt64Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Single ReadFloat32()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.Float32Code);
            var val = BssomBinaryPrimitives.ReadFloat32LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Float32Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Float32Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Single ReadFloat32WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadFloat32LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Float32Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Float32Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Double ReadFloat64()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.Float64Code);
            var val = BssomBinaryPrimitives.ReadFloat64LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Float64Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Float64Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Double ReadFloat64WithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadFloat64LittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.Float64Size));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.Float64Size, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Boolean ReadBoolean()
        {
            EnsureTypeWithSkipBlankCharacter(BssomType.BooleanCode);
            var val = BssomBinaryPrimitives.ReadBoolean(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.BooleanSize));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.BooleanSize, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Boolean ReadBooleanWithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadBoolean(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.BooleanSize));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.BooleanSize, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Char ReadChar()
        {
            EnsureNativeTypeWithSkipBlankCharacter(NativeBssomType.CharCode);
            var val = BssomBinaryPrimitives.ReadCharLittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.CharSize));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.CharSize, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Char ReadCharWithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadCharLittleEndian(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.CharSize));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.CharSize, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Decimal ReadDecimal()
        {
            EnsureNativeTypeWithSkipBlankCharacter(NativeBssomType.DecimalCode);
            var val = BssomBinaryPrimitives.ReadDecimal(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.DecimalSize));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.DecimalSize, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Decimal ReadDecimalWithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadDecimal(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.DecimalSize));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.DecimalSize, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Guid ReadGuid()
        {
            EnsureNativeTypeWithSkipBlankCharacter(NativeBssomType.GuidCode);
            var val = BssomBinaryPrimitives.ReadGuid(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.GuidSize));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.GuidSize, BssomSeekOrgin.Current);
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Guid ReadGuidWithOutTypeHead()
        {
            var val = BssomBinaryPrimitives.ReadGuid(ref BssomBuffer.ReadRef(BssomBinaryPrimitives.GuidSize));
            BssomBuffer.SeekWithOutVerify(BssomBinaryPrimitives.GuidSize, BssomSeekOrgin.Current);
            return val;
        }
    }
}

