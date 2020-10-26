using System;

namespace Bssom.Serializer.Internal
{
    internal sealed class Int8Array1ElementWriter : IArray1ElementWriter<SByte>
    {
        public static readonly Int8Array1ElementWriter Instance = new Int8Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, SByte value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Int8Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<SByte>();
            }
        }
        public SByte ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Int8Code)
            {
                return reader.ReadInt8WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<SByte>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (SByte)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class Int16Array1ElementWriter : IArray1ElementWriter<Int16>
    {
        public static readonly Int16Array1ElementWriter Instance = new Int16Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Int16 value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Int16Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Int16>();
            }
        }
        public Int16 ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Int16Code)
            {
                return reader.ReadInt16WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Int16>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Int16)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class Int32Array1ElementWriter : IArray1ElementWriter<Int32>
    {
        public static readonly Int32Array1ElementWriter Instance = new Int32Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Int32 value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Int32Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Int32>();
            }
        }
        public Int32 ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Int32Code)
            {
                return reader.ReadInt32WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Int32>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Int32)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class Int64Array1ElementWriter : IArray1ElementWriter<Int64>
    {
        public static readonly Int64Array1ElementWriter Instance = new Int64Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Int64 value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Int64Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Int64>();
            }
        }
        public Int64 ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Int64Code)
            {
                return reader.ReadInt64WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Int64>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Int64)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class UInt8Array1ElementWriter : IArray1ElementWriter<Byte>
    {
        public static readonly UInt8Array1ElementWriter Instance = new UInt8Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Byte value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.UInt8Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Byte>();
            }
        }
        public Byte ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.UInt8Code)
            {
                return reader.ReadUInt8WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Byte>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Byte)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class UInt16Array1ElementWriter : IArray1ElementWriter<UInt16>
    {
        public static readonly UInt16Array1ElementWriter Instance = new UInt16Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, UInt16 value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.UInt16Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<UInt16>();
            }
        }
        public UInt16 ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.UInt16Code)
            {
                return reader.ReadUInt16WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<UInt16>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (UInt16)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class UInt32Array1ElementWriter : IArray1ElementWriter<UInt32>
    {
        public static readonly UInt32Array1ElementWriter Instance = new UInt32Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, UInt32 value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.UInt32Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<UInt32>();
            }
        }
        public UInt32 ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.UInt32Code)
            {
                return reader.ReadUInt32WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<UInt32>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (UInt32)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class UInt64Array1ElementWriter : IArray1ElementWriter<UInt64>
    {
        public static readonly UInt64Array1ElementWriter Instance = new UInt64Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, UInt64 value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.UInt64Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<UInt64>();
            }
        }
        public UInt64 ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.UInt64Code)
            {
                return reader.ReadUInt64WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<UInt64>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (UInt64)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class Float32Array1ElementWriter : IArray1ElementWriter<Single>
    {
        public static readonly Float32Array1ElementWriter Instance = new Float32Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Single value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Float32Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Single>();
            }
        }
        public Single ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Float32Code)
            {
                return reader.ReadFloat32WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Single>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Single)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class Float64Array1ElementWriter : IArray1ElementWriter<Double>
    {
        public static readonly Float64Array1ElementWriter Instance = new Float64Array1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Double value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Float64Code)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Double>();
            }
        }
        public Double ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.Float64Code)
            {
                return reader.ReadFloat64WithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Double>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Double)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class BooleanArray1ElementWriter : IArray1ElementWriter<Boolean>
    {
        public static readonly BooleanArray1ElementWriter Instance = new BooleanArray1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Boolean value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.BooleanCode)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Boolean>();
            }
        }
        public Boolean ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.BooleanCode)
            {
                return reader.ReadBooleanWithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Boolean>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Boolean)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class CharArray1ElementWriter : IArray1ElementWriter<Char>
    {
        public static readonly CharArray1ElementWriter Instance = new CharArray1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Char value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == true && offsetInfo.Array1ElementType == NativeBssomType.CharCode)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Char>();
            }
        }
        public Char ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == true && offsetInfo.Array1ElementType == NativeBssomType.CharCode)
            {
                return reader.ReadCharWithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Char>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Char)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class DecimalArray1ElementWriter : IArray1ElementWriter<Decimal>
    {
        public static readonly DecimalArray1ElementWriter Instance = new DecimalArray1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Decimal value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == true && offsetInfo.Array1ElementType == NativeBssomType.DecimalCode)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Decimal>();
            }
        }
        public Decimal ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == true && offsetInfo.Array1ElementType == NativeBssomType.DecimalCode)
            {
                return reader.ReadDecimalWithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Decimal>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Decimal)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class GuidArray1ElementWriter : IArray1ElementWriter<Guid>
    {
        public static readonly GuidArray1ElementWriter Instance = new GuidArray1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Guid value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == true && offsetInfo.Array1ElementType == NativeBssomType.GuidCode)
            {
                writer.WriteWithOutTypeHead(value);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<Guid>();
            }
        }
        public Guid ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == true && offsetInfo.Array1ElementType == NativeBssomType.GuidCode)
            {
                return reader.ReadGuidWithOutTypeHead();
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<Guid>();
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (Guid)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
    internal sealed class DateTimeArray1ElementWriter : IArray1ElementWriter<DateTime>
    {
        public static readonly DateTimeArray1ElementWriter Instance = new DateTimeArray1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, DateTime value)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.TimestampCode)
            {
                writer.Write(value, true, false);
            }
            else if (offsetInfo.Array1ElementTypeIsNativeType == true && offsetInfo.Array1ElementType == NativeBssomType.DateTimeCode)
            {
                writer.Write(value, false, false);
            }
            else
            {
                BssomSerializationArgumentException.InvalidOffsetInfoFormat<DateTime>();
            }
        }
        public DateTime ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            if (offsetInfo.Array1ElementTypeIsNativeType == false && offsetInfo.Array1ElementType == BssomType.TimestampCode)
            {
                return reader.ReadStandDateTimeWithOutTypeHead();
            }
            else if (offsetInfo.Array1ElementTypeIsNativeType == true && offsetInfo.Array1ElementType == NativeBssomType.DateTimeCode)
            {
                return reader.ReadNativeDateTimeWithOutTypeHead();
            }
            else
            {
                return BssomSerializationArgumentException.InvalidOffsetInfoFormat<DateTime>();
            }
        }
        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            WriteElement(ref writer, option, offsetInfo, (DateTime)value);
        }
        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadElement(ref reader, option, offsetInfo);
        }
    }
}