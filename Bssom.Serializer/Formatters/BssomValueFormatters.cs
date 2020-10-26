using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap;
using System.Collections.Generic;

namespace Bssom.Serializer.Formatters
{
    /// <summary>
    /// Format <see cref="BssomChar"/> as BssomType.Char
    /// </summary>
    public sealed class BssomCharFormatter : IBssomFormatter<BssomChar>
    {
        public static readonly BssomCharFormatter Instance = new BssomCharFormatter();

        public BssomChar Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object val = BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
            if (val == null)
            {
                return null;
            }

            return (BssomChar)val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomChar value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.GetChar());
        }

        public int Size(ref BssomSizeContext context, BssomChar value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.CharSize;
        }
    }

    /// <summary>
    /// Format <see cref="BssomDateTime"/> as BssomType.DateTime or NativeBssomType.DateTime
    /// </summary>
    public sealed class BssomDateTimeFormatter : IBssomFormatter<BssomDateTime>
    {
        public static readonly BssomDateTimeFormatter Instance = new BssomDateTimeFormatter();

        public BssomDateTime Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object val = BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
            if (val == null)
            {
                return null;
            }

            return (BssomDateTime)val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomDateTime value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.GetDateTime(), context.Option.IsUseStandardDateTime);
        }

        public int Size(ref BssomSizeContext context, BssomDateTime value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            if (context.Option.IsUseStandardDateTime)
            {
                return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.StandardDateTimeSize;
            }

            return BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.NativeDateTimeSize;
        }
    }

    /// <summary>
    /// Format <see cref="BssomGuid"/> as NativeBssomType.Guid
    /// </summary>
    public sealed class BssomGuidFormatter : IBssomFormatter<BssomGuid>
    {
        public static readonly BssomGuidFormatter Instance = new BssomGuidFormatter();

        public BssomGuid Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object val = BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
            if (val == null)
            {
                return null;
            }

            return (BssomGuid)val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomGuid value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.GetGuid());
        }

        public int Size(ref BssomSizeContext context, BssomGuid value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.GuidSize;
        }
    }

    /// <summary>
    /// Format <see cref="BssomDecimal"/> as NativeBssomType.Decimal
    /// </summary>
    public sealed class BssomDecimalFormatter : IBssomFormatter<BssomDecimal>
    {
        public static readonly BssomDecimalFormatter Instance = new BssomDecimalFormatter();

        public BssomDecimal Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object val = BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
            if (val == null)
            {
                return null;
            }

            return (BssomDecimal)val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomDecimal value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.GetDecimal());
        }

        public int Size(ref BssomSizeContext context, BssomDecimal value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.DecimalSize;
        }
    }

    /// <summary>
    /// Format <see cref="BssomFloat"/> as BssomType.Float32 or BssomType.Float64
    /// </summary>
    public sealed class BssomFloatFormatter : IBssomFormatter<BssomFloat>
    {
        public static readonly BssomFloatFormatter Instance = new BssomFloatFormatter();

        public BssomFloat Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object val = BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
            if (val == null)
            {
                return null;
            }

            return (BssomFloat)val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomFloat value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            switch (value.FloatType)
            {
                case BssomFloat.BssomFloatType.Single:
                    writer.Write(value.GetFloat32());
                    break;
                default://case BssomFloatType.Double:
                    writer.Write(value.GetFloat64());
                    break;
            }
        }

        public int Size(ref BssomSizeContext context, BssomFloat value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            switch (value.FloatType)
            {
                case BssomFloat.BssomFloatType.Single:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Float32Size;
                default://case BssomFloatType.Double:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Float64Size;
            }
        }
    }

    /// <summary>
    /// Format <see cref="BssomNumber"/> as BssomType.Int16 or BssomType.Int32 or BssomType.Int64 or BssomType.UInt16 or BssomType.UInt32 or BssomType.UInt64
    /// </summary>
    public sealed class BssomNumberFormatter : IBssomFormatter<BssomNumber>
    {
        public static readonly BssomNumberFormatter Instance = new BssomNumberFormatter();

        public BssomNumber Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
            {
                return null;
            }

            return BssomObjectFormatterHelper.DeserializePrimitiveWithBssomPrimitiveType(ref reader);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomNumber value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            switch (value.NumberType)
            {
                case BssomNumber.BssomNumberType.SByte:
                    writer.Write(value.GetSByte());
                    break;
                case BssomNumber.BssomNumberType.Byte:
                    writer.Write(value.GetByte());
                    break;
                case BssomNumber.BssomNumberType.Short:
                    writer.Write(value.GetShort());
                    break;
                case BssomNumber.BssomNumberType.Int:
                    writer.Write(value.GetInt());
                    break;
                case BssomNumber.BssomNumberType.Long:
                    writer.Write(value.GetLong());
                    break;
                case BssomNumber.BssomNumberType.UShort:
                    writer.Write(value.GetUShort());
                    break;
                case BssomNumber.BssomNumberType.UInt:
                    writer.Write(value.GetUInt());
                    break;
                default://case BssomNumberType.ULong:
                    writer.Write(value.GetULong());
                    break;
            }
        }

        public int Size(ref BssomSizeContext context, BssomNumber value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            switch (value.NumberType)
            {
                case BssomNumber.BssomNumberType.SByte:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int8Size;
                case BssomNumber.BssomNumberType.Byte:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt8Size;
                case BssomNumber.BssomNumberType.Short:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int16Size;
                case BssomNumber.BssomNumberType.Int:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int32Size;
                case BssomNumber.BssomNumberType.Long:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int64Size;
                case BssomNumber.BssomNumberType.UShort:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt16Size;
                case BssomNumber.BssomNumberType.UInt:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt32Size;
                default://case BssomNumberType.ULong:
                    return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt64Size;
            }
        }
    }

    /// <summary>
    /// Format <see cref="BssomBoolean"/> as BssomType.Boolean
    /// </summary>
    public sealed class BssomBooleanFormatter : IBssomFormatter<BssomBoolean>
    {
        public static readonly BssomBooleanFormatter Instance = new BssomBooleanFormatter();

        public BssomBoolean Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object val = BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
            if (val == null)
            {
                return null;
            }

            return (BssomBoolean)val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomBoolean value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.GetBoolean());
        }

        public int Size(ref BssomSizeContext context, BssomBoolean value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.BooleanSize;
        }
    }

    /// <summary>
    /// Format <see cref="BssomArray"/> as BssomType.Array1 or BssomType.Array2
    /// </summary>
    public sealed class BssomArrayFormatter : IBssomFormatter<BssomArray>
    {
        public static readonly BssomArrayFormatter Instance = new BssomArrayFormatter();

        public BssomArray Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object val = BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
            if (val == null)
            {
                return null;
            }

            return (BssomArray)val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomArray value)
        {
            ObjectFormatter.Instance.Serialize(ref writer, ref context, value.RawValue);
        }

        public int Size(ref BssomSizeContext context, BssomArray value)
        {
            return ObjectFormatter.Instance.Size(ref context, value.RawValue);
        }
    }

    /// <summary>
    /// Format <see cref="BssomString"/> as BssomType.String
    /// </summary>
    public sealed class BssomStringFormatter : IBssomFormatter<BssomString>
    {
        public static readonly BssomStringFormatter Instance = new BssomStringFormatter();

        public BssomString Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object val = BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
            if (val == null)
            {
                return null;
            }

            return (BssomString)val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomString value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.GetString());
        }

        public int Size(ref BssomSizeContext context, BssomString value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.StringSize(value.GetString()) + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }
    }

    /// <summary>
    /// Format <see cref="BssomNull"/> as BssomType.Null
    /// </summary>
    public sealed class BssomNullFormatter : IBssomFormatter<BssomNull>
    {
        public static readonly BssomNullFormatter Instance = new BssomNullFormatter();

        public BssomNull Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            reader.EnsureType(BssomType.NullCode);
            return BssomNull.Value;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomNull value)
        {
            writer.WriteNull();
        }

        public int Size(ref BssomSizeContext context, BssomNull value)
        {
            return BssomBinaryPrimitives.NullSize;
        }
    }

    /// <summary>
    /// Format <see cref="BssomMap"/> as BssomType.Map1 or BssomType.Map2
    /// </summary>
    public sealed class BssomMapFormatter : IBssomFormatter<BssomMap>
    {
        public static readonly BssomMapFormatter Instance = new BssomMapFormatter();

        public BssomMap Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object val = BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
            if (val == null)
            {
                return null;
            }

            return (BssomMap)val;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomMap value)
        {
            MapFormatterHelper.SerializeGenericDictionary(ref writer, ref context, value.GetMap());
        }

        public int Size(ref BssomSizeContext context, BssomMap value)
        {
            return MapFormatterHelper.SizeGenericDictionary(ref context, value.GetMap());
        }
    }

    /// <summary>
    ///  Format <see cref="BssomValue"/> as Bssom value
    /// </summary>
    public sealed class BssomValueFormatter : IBssomFormatter<BssomValue>
    {
        public static readonly BssomValueFormatter Instance = new BssomValueFormatter();

        public BssomValue Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return (BssomValue)BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, true);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, BssomValue value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            switch (value.ValueType)
            {
                case BssomValueType.Array:
                    BssomArrayFormatter.Instance.Serialize(ref writer, ref context, (BssomArray)value);
                    break;
                case BssomValueType.Map:
                    BssomMapFormatter.Instance.Serialize(ref writer, ref context, (BssomMap)value);
                    break;
                case BssomValueType.Null:
                    BssomNullFormatter.Instance.Serialize(ref writer, ref context, (BssomNull)(value));
                    break;
                case BssomValueType.Number:
                    BssomNumberFormatter.Instance.Serialize(ref writer, ref context, (BssomNumber)value);
                    break;
                case BssomValueType.Float:
                    BssomFloatFormatter.Instance.Serialize(ref writer, ref context, (BssomFloat)(value));
                    break;
                case BssomValueType.String:
                    BssomStringFormatter.Instance.Serialize(ref writer, ref context, (BssomString)(value));
                    break;
                case BssomValueType.DateTime:
                    BssomDateTimeFormatter.Instance.Serialize(ref writer, ref context, (BssomDateTime)(value));
                    break;
                case BssomValueType.Char:
                    BssomCharFormatter.Instance.Serialize(ref writer, ref context, (BssomChar)value);
                    break;
                default://case BssomValueType.Boolean:
                    BssomBooleanFormatter.Instance.Serialize(ref writer, ref context, (BssomBoolean)(value));
                    break;
            }

        }

        public int Size(ref BssomSizeContext context, BssomValue value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }
            switch (value.ValueType)
            {
                case BssomValueType.Array:
                    return BssomArrayFormatter.Instance.Size(ref context, (BssomArray)value);
                case BssomValueType.Map:
                    return BssomMapFormatter.Instance.Size(ref context, (BssomMap)value);
                case BssomValueType.Null:
                    return BssomNullFormatter.Instance.Size(ref context, (BssomNull)(value));
                case BssomValueType.Number:
                    return BssomNumberFormatter.Instance.Size(ref context, (BssomNumber)value);
                case BssomValueType.String:
                    return BssomStringFormatter.Instance.Size(ref context, (BssomString)(value));
                case BssomValueType.Char:
                    return BssomCharFormatter.Instance.Size(ref context, (BssomChar)(value));
                case BssomValueType.Guid:
                    return BssomGuidFormatter.Instance.Size(ref context, (BssomGuid)(value));
                case BssomValueType.Boolean:
                    return BssomBooleanFormatter.Instance.Size(ref context, (BssomBoolean)(value));
                default://case BssomValueType.DateTime:
                    return BssomDateTimeFormatter.Instance.Size(ref context, (BssomDateTime)(value));
            }
        }
    }

    internal static class BssomObjectFormatterHelper
    {
        public static object DeserializeBssObject(ref BssomReader reader, ref BssomDeserializeContext context,
            bool isPriorityToDeserializeObjectAsBssomValue)
        {
            byte type = reader.SkipBlankCharacterAndPeekBssomType();
            switch (type)
            {
                case BssomType.NullCode:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return BssomNull.Value;
                    }

                    return null;

                case BssomType.BooleanCode:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomBoolean(reader.ReadBooleanWithOutTypeHead());
                    }

                    return reader.ReadBooleanWithOutTypeHead();
                case BssomType.Int32Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomNumber(reader.ReadInt32WithOutTypeHead());
                    }

                    return reader.ReadInt32WithOutTypeHead();
                case BssomType.Int16Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomNumber(reader.ReadInt16WithOutTypeHead());
                    }

                    return reader.ReadInt16WithOutTypeHead();
                case BssomType.Int64Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomNumber(reader.ReadInt64WithOutTypeHead());
                    }

                    return reader.ReadInt64WithOutTypeHead();
                case BssomType.UInt64Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomNumber(reader.ReadUInt64WithOutTypeHead());
                    }

                    return reader.ReadUInt64WithOutTypeHead();
                case BssomType.UInt32Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomNumber(reader.ReadUInt32WithOutTypeHead());
                    }

                    return reader.ReadUInt32WithOutTypeHead();
                case BssomType.UInt16Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomNumber(reader.ReadUInt16WithOutTypeHead());
                    }

                    return reader.ReadUInt16WithOutTypeHead();
                case BssomType.UInt8Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomNumber(reader.ReadUInt8WithOutTypeHead());
                    }

                    return reader.ReadUInt8WithOutTypeHead();
                case BssomType.Int8Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomNumber(reader.ReadInt8WithOutTypeHead());
                    }

                    return reader.ReadInt8WithOutTypeHead();

                case BssomType.Float32Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomFloat(reader.ReadFloat32WithOutTypeHead());
                    }

                    return reader.ReadFloat32WithOutTypeHead();
                case BssomType.Float64Code:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomFloat(reader.ReadFloat64WithOutTypeHead());
                    }

                    return reader.ReadFloat64WithOutTypeHead();

                case BssomType.TimestampCode:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomDateTime(reader.ReadStandDateTimeWithOutTypeHead());
                    }

                    return reader.ReadStandDateTimeWithOutTypeHead();

                case BssomType.StringCode:
                    reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                    if (isPriorityToDeserializeObjectAsBssomValue)
                    {
                        return new BssomString(reader.ReadStringWithOutTypeHead());
                    }

                    return reader.ReadStringWithOutTypeHead();

                case BssomType.NativeCode:
                    {
                        reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                        type = reader.ReadBssomType();
                        switch (type)
                        {
                            case NativeBssomType.CharCode:
                                if (isPriorityToDeserializeObjectAsBssomValue)
                                {
                                    return new BssomChar(reader.ReadCharWithOutTypeHead());
                                }

                                return reader.ReadCharWithOutTypeHead();
                            case NativeBssomType.DecimalCode:
                                if (isPriorityToDeserializeObjectAsBssomValue)
                                {
                                    return new BssomDecimal(reader.ReadDecimalWithOutTypeHead());
                                }

                                return reader.ReadDecimalWithOutTypeHead();
                            case NativeBssomType.DateTimeCode:
                                if (isPriorityToDeserializeObjectAsBssomValue)
                                {
                                    return new BssomDateTime(reader.ReadNativeDateTimeWithOutTypeHead());
                                }

                                return reader.ReadNativeDateTimeWithOutTypeHead();
                            case NativeBssomType.GuidCode:
                                if (isPriorityToDeserializeObjectAsBssomValue)
                                {
                                    return new BssomGuid(reader.ReadGuidWithOutTypeHead());
                                }

                                return reader.ReadGuidWithOutTypeHead();
                        }
                    }
                    break;

                case BssomType.Array1:
                    {
                        reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                        byte eleType = reader.ReadBssomType();
                        if (eleType != BssomType.NativeCode)
                        {
                            reader.BssomBuffer.Seek(-BssomBinaryPrimitives.Array1BuildInTypeCodeSize, BssomSeekOrgin.Current);
                        }

                        switch (eleType)
                        {
                            case BssomType.BooleanCode:
                                return new BssomArray(BooleanListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.Int32Code:
                                return new BssomArray(Int32ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.Int16Code:
                                return new BssomArray(Int16ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.Int64Code:
                                return new BssomArray(Int64ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.UInt64Code:
                                return new BssomArray(UInt64ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.UInt32Code:
                                return new BssomArray(UInt32ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.UInt16Code:
                                return new BssomArray(UInt16ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.UInt8Code:
                                return new BssomArray(UInt8ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.Int8Code:
                                return new BssomArray(Int8ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.Float32Code:
                                return new BssomArray(Float32ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.Float64Code:
                                return new BssomArray(Float64ListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.TimestampCode:
                                return new BssomArray(DateTimeListFormatter.Instance.Deserialize(ref reader, ref context), false);
                            case BssomType.NativeCode:
                                {
                                    type = reader.ReadBssomType();
                                    reader.BssomBuffer.Seek(-BssomBinaryPrimitives.Array1NativeTypeCodeSize, BssomSeekOrgin.Current);
                                    switch (type)
                                    {
                                        case NativeBssomType.CharCode:
                                            return new BssomArray(CharListFormatter.Instance.Deserialize(ref reader, ref context), false);
                                        case NativeBssomType.DecimalCode:
                                            return new BssomArray(DecimalListFormatter.Instance.Deserialize(ref reader, ref context), false);
                                        case NativeBssomType.DateTimeCode:
                                            return new BssomArray(DateTimeListFormatter.Instance.Deserialize(ref reader, ref context), false);
                                        case NativeBssomType.GuidCode:
                                            return new BssomArray(GuidListFormatter.Instance.Deserialize(ref reader, ref context), false);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case BssomType.Array2:
                    {
                        reader.BssomBuffer.SeekWithOutVerify(1, BssomSeekOrgin.Current);
                        reader.SkipVariableNumber();
                        int count = reader.ReadVariableNumber();
                        List<object> ary = new List<object>(count);
                        for (int i = 0; i < count; i++)
                        {
                            ary.Add(DeserializeBssObject(ref reader, ref context, isPriorityToDeserializeObjectAsBssomValue));
                        }
                        return new BssomArray(ary, false);
                    }
                case BssomType.Map1:
                case BssomType.Map2:
                    {
                        return new BssomMap(MapFormatterHelper.GenericDictionaryDeserialize<object, object>(ref reader, ref context));
                    }
            }
            throw BssomSerializationOperationException.UnexpectedCodeRead(type, reader.Position);
        }


        public static BssomNumber DeserializePrimitiveWithBssomPrimitiveType(ref BssomReader reader)
        {
            byte ct = reader.SkipBlankCharacterAndReadBssomType();
            switch (ct)
            {
                case BssomType.Int32Code:
                    return new BssomNumber(reader.ReadInt32WithOutTypeHead());
                case BssomType.Int16Code:
                    return new BssomNumber(reader.ReadInt16WithOutTypeHead());
                case BssomType.Int64Code:
                    return new BssomNumber(reader.ReadInt64WithOutTypeHead());
                case BssomType.UInt64Code:
                    return new BssomNumber(reader.ReadUInt64WithOutTypeHead());
                case BssomType.UInt32Code:
                    return new BssomNumber(reader.ReadUInt32WithOutTypeHead());
                case BssomType.UInt16Code:
                    return new BssomNumber(reader.ReadUInt16WithOutTypeHead());
                case BssomType.UInt8Code:
                    return new BssomNumber(reader.ReadUInt8WithOutTypeHead());
                default://case BssomType.Int8Code:
                    return new BssomNumber(reader.ReadInt8WithOutTypeHead());
            }
        }
    }
}
