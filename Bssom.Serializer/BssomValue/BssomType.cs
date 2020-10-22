//using System.Runtime.CompilerServices;

using System;

namespace Bssom.Serializer
{
    /// <summary>
    /// Bssom type 
    /// </summary>
    public static class BssomType
    {
        //Empty : 0~127,  128:uint16  129:uint32
        public const int MinBlankCodeValue = 0;
        public const int MaxVarBlankCodeValue = BlankInt16Code - 1;
        public const int MaxBlankCodeValue = BlankInt32Code;//129
        public const int BlankInt16Code = sbyte.MaxValue + 1;//128
        public const int BlankInt32Code = sbyte.MaxValue + 2;//129

        public const int MinFixLenTypeCode = NullCode;
        public const int MaxFixLenTypeCode = TimestampCode;

        public const int NullCode = 130;
        public const int Int8Code = 131;
        public const int Int16Code = 132;
        public const int Int32Code = 133;
        public const int Int64Code = 134;
        public const int UInt8Code = 135;
        public const int UInt16Code = 136;
        public const int UInt32Code = 137;
        public const int UInt64Code = 138;
        public const int Float32Code = 139;
        public const int Float64Code = 140;
        public const int BooleanCode = 141;
        public const int TimestampCode = 142;
        public const int StringCode = 143;

        public const int Map1 = 0xc1;
        public const int Map2 = 0xc2;

        public const int Array1 = 0xd1;
        public const int Array2 = 0xd2;

        public const int ExtendCode = 0xf1;
        public const int NativeCode = 0xf2;

        /// <summary>
        /// <para>根据<paramref name="isNativeType"/>和<paramref name="typeCode"/>来获得Bssom类型名称</para>
        /// <para>Obtain the Bssom type name according to <paramref name="isNativeType"/> and <paramref name="typeCode"/></para>
        /// </summary>
        /// <returns>Bssom type name</returns>
        public static string GetTypeName(bool isNativeType, byte typeCode)
        {
            if (isNativeType)
            {
                switch (typeCode)
                {
                    case NativeBssomType.CharCode:
                        return nameof(NativeBssomType.CharCode);
                    case NativeBssomType.DateTimeCode:
                        return nameof(NativeBssomType.DateTimeCode);
                    case NativeBssomType.DecimalCode:
                        return nameof(NativeBssomType.DecimalCode);
                    case NativeBssomType.GuidCode:
                        return nameof(NativeBssomType.GuidCode);
                }
            }
            else
            {
                switch (typeCode)
                {
                    case BssomType.NullCode: return nameof(BssomType.NullCode);
                    case BssomType.Int8Code: return nameof(BssomType.Int8Code);
                    case BssomType.Int16Code: return nameof(BssomType.Int16Code);
                    case BssomType.Int32Code: return nameof(BssomType.Int32Code);
                    case BssomType.Int64Code: return nameof(BssomType.Int64Code);
                    case BssomType.UInt8Code: return nameof(BssomType.UInt8Code);
                    case BssomType.UInt16Code: return nameof(BssomType.UInt16Code);
                    case BssomType.UInt32Code: return nameof(BssomType.UInt32Code);
                    case BssomType.UInt64Code: return nameof(BssomType.UInt64Code);
                    case BssomType.Float32Code: return nameof(BssomType.Float32Code);
                    case BssomType.Float64Code: return nameof(BssomType.Float64Code);
                    case BssomType.BooleanCode: return nameof(BssomType.BooleanCode);
                    case BssomType.TimestampCode: return nameof(BssomType.TimestampCode);
                    case BssomType.StringCode: return nameof(BssomType.StringCode);
                    case BssomType.Map1: return nameof(BssomType.Map1);
                    case BssomType.Map2: return nameof(BssomType.Map2);
                    case BssomType.Array1: return nameof(BssomType.Array1);
                    case BssomType.Array2: return nameof(BssomType.Array2);
                }
            }
            throw BssomSerializationArgumentException.InvalidTypeCode();
        }

        /// <summary>
        /// <para>根据<paramref name="isNativeType"/>和<paramref name="typeCode"/>来获得BssomValueType</para>
        /// <para>Obtain the BssomValueType name according to <paramref name="isNativeType"/> and <paramref name="typeCode"/></para>
        /// </summary>
        /// <returns>BssomValueType</returns>
        public static BssomValueType GetBssomValueType(bool isNativeType, byte typeCode)
        {
            if (isNativeType)
            {
                switch (typeCode)
                {
                    case NativeBssomType.CharCode:
                        return BssomValueType.Char;
                    case NativeBssomType.DateTimeCode:
                        return BssomValueType.DateTime;
                    case NativeBssomType.DecimalCode:
                        return BssomValueType.Decimal;
                    case NativeBssomType.GuidCode:
                        return BssomValueType.Guid;
                }
            }
            else
            {
                switch (typeCode)
                {
                    case BssomType.NullCode:
                        return BssomValueType.Null;
                    case BssomType.Int8Code:
                    case BssomType.Int16Code:
                    case BssomType.Int32Code:
                    case BssomType.Int64Code:
                    case BssomType.UInt8Code:
                    case BssomType.UInt16Code:
                    case BssomType.UInt32Code:
                    case BssomType.UInt64Code:
                        return BssomValueType.Number;
                    case BssomType.Float32Code:
                    case BssomType.Float64Code:
                        return BssomValueType.Float;
                    case BssomType.BooleanCode:
                        return BssomValueType.Boolean;
                    case BssomType.TimestampCode:
                        return BssomValueType.DateTime;
                    case BssomType.StringCode:
                        return BssomValueType.String;
                    case BssomType.Map1:
                    case BssomType.Map2:
                        return BssomValueType.Map;
                    case BssomType.Array1:
                    case BssomType.Array2:
                        return BssomValueType.Array;
                }
            }
            throw BssomSerializationArgumentException.InvalidTypeCode();
        }
    }

}
