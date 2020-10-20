using System;
using System.Data;

namespace BssomSerializers
{
    /// <summary>
    /// The exception that is thrown when an invalid operation occurs in a method.
    /// </summary>
    public class BssomSerializationOperationException : BssomSerializationException
    {
        public enum SerializationErrorCode
        {
            InputDataUnmatch, 
            InputDataSouceIsEmpty,
            OperationObjectIsNull,
            UnsupportedOperation,
            ArrayTypeIndexOutOfBounds,
            IncorrectTypeCode,
            ReachedEndOfBuffer,
            CapacityOfBufferIsInsufficient,
            CopyStreamError,
        }
        public SerializationErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializationOperationException"/> class with a specified errorCode and error message.
        /// </summary>
        public BssomSerializationOperationException(SerializationErrorCode errorCode, string message) : base($"ErrorCode : {errorCode}{errorCode.ToString()} , message: {message}")
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializationOperationException"/> class with a specified errorCode and error message.
        /// </summary>
        public BssomSerializationOperationException(SerializationErrorCode errorCode, string message,Exception inner) : base($"ErrorCode : {errorCode}{errorCode.ToString()} , message: {message}", inner)
        {
            ErrorCode = errorCode;
        }

        internal static BssomSerializationOperationException InputDataSouceIsEmpty()
        {
            return new BssomSerializationOperationException(SerializationErrorCode.InputDataSouceIsEmpty, "输入数据源是空的. Input data source is empty");
        }

        internal static BssomSerializationOperationException ArrayTypeIndexOutOfBounds(int inputIndex, int valueCountInData)
        {
            return new BssomSerializationOperationException(SerializationErrorCode.ArrayTypeIndexOutOfBounds, $"在对Array类型的读取中发生了索引超出边界错误,要访问的数组索引为{inputIndex},数组实际元素数量为{valueCountInData}. An index out of bounds error occurred during a read of type Array. The Array index to be accessed is {inputIndex}, and the actual number of arrays is {valueCountInData}");
        }


        internal static BssomSerializationOperationException BssomMapIsNull(object key)
        {
            return new BssomSerializationOperationException(SerializationErrorCode.OperationObjectIsNull, $"BssomMap[{key.ToString()}] is null");
        }

        internal static BssomSerializationOperationException BssomObjectIsNull()
        {
            return new BssomSerializationOperationException(SerializationErrorCode.OperationObjectIsNull, $"BssomObject is null");
        }

        public static BssomSerializationOperationException BssomValueTypeReadFromStreamNotSupportIndexOf(long position)
        {
            return new BssomSerializationOperationException(SerializationErrorCode.UnsupportedOperation, "从缓冲区中读取的BssomValue类型不支持IndexOf,只有Map,Array类型才支持. The BssomValue type read from the buffer does not support IndexOf, only Map and Array types support");

        }

        internal static BssomSerializationOperationException UnexpectedCodeRead(byte code, long position)
        {
            return new BssomSerializationOperationException(SerializationErrorCode.IncorrectTypeCode, $"读取到了意外的byte. An unexpected byte was read. Code: {code} ,Position: {position} ");
        }

        internal static BssomSerializationOperationException UnexpectedCodeRead(long position)
        {
            return new BssomSerializationOperationException(SerializationErrorCode.IncorrectTypeCode, $"读取到了意外的byte. An unexpected byte was read. Position : {position} ");
        }

        internal static BssomSerializationOperationException UnexpectedCode()
        {
            throw new BssomSerializationOperationException(SerializationErrorCode.IncorrectTypeCode, "读取到了意外的byte. An unexpected byte was read");
        }

        internal static T UnexpectedCodeRead<T>(byte typeCode, long position)
        {
            throw UnexpectedCodeRead(typeCode, position);
        }

        internal static T UnexpectedCodeRead<T>()
        {
            throw UnexpectedCode();
        }

        internal static ref byte ReaderEndOfBufferException()
        {
            throw new BssomSerializationOperationException(SerializationErrorCode.ReachedEndOfBuffer, "尝试在缓冲区的末尾进行读取. Reading is attempted past the end of a buffer.");
        }

        internal static void CapacityOfBufferIsInsufficient(int remaining)
        {
            throw new BssomSerializationOperationException(SerializationErrorCode.CapacityOfBufferIsInsufficient, $"缓冲区容量不足,读取器需要保证缓冲区最低有{remaining}个字节的剩余容量. The buffer capacity is insufficient, reader needs to ensure that the stream has a minimum of {remaining} bytes of remaining capacity.");
        }

        internal static void CopyStreamError(Exception ex)
        {
            throw new BssomSerializationOperationException(SerializationErrorCode.CopyStreamError, $"在拷贝流时发生错误. An error occurred while copying the stream.",ex);
        }
    }
}
