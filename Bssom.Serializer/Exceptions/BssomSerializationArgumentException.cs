namespace Bssom.Serializer
{
    /// <summary>
    /// The exception that is thrown when one of the arguments provided to a method is not valid.
    /// </summary>
    public class BssomSerializationArgumentException : BssomSerializationException
    {
        public enum SerializationErrorCode
        {
            InvalidValueOfMembers = 0,
            InvalidFormatOfMembers = 1,
            BssomMapStringKeyIsEmpty = 2,
            BssomMapKeySame = 3,
        }
        public SerializationErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializationArgumentException"/> class with a specified errorCode and error message.
        /// </summary>
        public BssomSerializationArgumentException(SerializationErrorCode errorCode, string message) : base($"ErrorCode : {errorCode}{errorCode.ToString()} , message: {message}")
        {
            ErrorCode = errorCode;
        }

        internal static BssomSerializationArgumentException BssMapKeyUnsupportedSameRawValue(byte key1Type, bool key1IsNative, byte key2Type, bool key2IsNative, ulong value)
        {
            throw new BssomSerializationArgumentException(SerializationErrorCode.BssomMapKeySame, $"BssomMap的Key中存在相同宽度类型({BssomType.GetTypeName(key1IsNative, key1Type)} 和 {BssomType.GetTypeName(key2IsNative, key2Type)})的相同的值{value}. The same value {value} of the same width type ({BssomType.GetTypeName(key1IsNative, key1Type)} and {BssomType.GetTypeName(key2IsNative, key2Type)} exist in the Key of BssomMap.");
        }

        internal static T InvalidOffsetInfoFormat<T>()
        {
            throw new BssomSerializationArgumentException(SerializationErrorCode.InvalidFormatOfMembers, "无效的OffsetInfo格式, 要读取或写入的类型与OffsetInfo值不同. Invalid OffsetInfo format, the type to be read or written is different from the OffsetInfo value");
        }

        internal static BssomSerializationArgumentException InvalidOffsetInfoValue()
        {
            return new BssomSerializationArgumentException(SerializationErrorCode.InvalidValueOfMembers, "无效的OffsetInfo值. Invalid OffsetInfo value.");
        }

        internal static BssomSerializationArgumentException InvalidTypeCode()
        {
            return new BssomSerializationArgumentException(SerializationErrorCode.InvalidValueOfMembers, "无效的TypeCode值. Invalid TypeCode value.");
        }

        internal static void BssomMapStringKeyIsEmpty()
        {
            throw new BssomSerializationArgumentException(SerializationErrorCode.BssomMapStringKeyIsEmpty, "在BssomMap中的字符串类型的Key值不能为empty. The Key value of String type in BssomMap cannot be empty");
        }

        internal static BssomSerializationArgumentException InputDataFormatterError()
        {
            return new BssomSerializationArgumentException(SerializationErrorCode.InvalidFormatOfMembers, "输入数据格式错误. Input data format error");
        }
    }


}
