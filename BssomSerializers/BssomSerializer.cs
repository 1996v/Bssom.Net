using BssomSerializers;
using BssomSerializers.BssomBuffer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BssomSerializers.Resolver;


namespace BssomSerializers
{
    public static class BssomSerializer
    {
        /// <summary>
        /// <para>在不进行序列化的情况下,获取对象被序列化后的二进制数据大小</para>
        /// <para>In the case of no serialization,Gets the size of the binary data after the object is serialized</para>
        /// </summary>
        /// <param name="value">要获取序列化大小的值. The value to get the serialization size</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <returns>对象被序列化后的数据大小. The size of the data after the object is serialized</returns>
        public static int Size<T>(T value, BssomSerializerOptions option = null)
        {
            if (option == null)
                option = BssomSerializerOptions.Default;

            BssomSizeContext sizeContext = new BssomSizeContext(option);
            return option.FormatterResolver.GetFormatterWithVerify<T>().Size(ref sizeContext, value);
        }

        /// <summary>
        /// <para>在不进行序列化的情况下,获取对象被序列化后的二进制数据大小</para>
        /// <para>In the case of no serialization,Gets the size of the binary data after the object is serialized</para>
        /// </summary>
        /// <param name="context">序列化大小所需要的上下文. The context required for the serialization size</param>
        /// <param name="value">要获取序列化大小的值. The value to get the serialization size</param>
        /// <returns>对象被序列化后的数据大小. The size of the data after the object is serialized</returns>
        public static int Size<T>(ref BssomSizeContext context, T value)
        {
            if (context.Option == null)
                context.Option = BssomSerializerOptions.Default;

            return context.Option.FormatterResolver.GetFormatterWithVerify<T>().Size(ref context, value);
        }

        /// <summary>
        /// <para>将给定的值序列化到内置的缓冲区中</para>
        /// <para>Serializes a given value with the built-in buffer</para>
        /// </summary>
        /// <param name="value">要序列化的值. The value to serialize</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>具有序列化值的字节数组. A byte array with the serialized value</returns>
        public static byte[] Serialize<T>(T value, BssomSerializerOptions option = null, CancellationToken cancellationToken = default)
        {
            if (option == null)
                option = BssomSerializerOptions.Default;

            BssomSerializeContext context = new BssomSerializeContext(option, cancellationToken);
            using (var buffer = ExpandableBufferWriter.CreateGlobar())
            {
                var writer = new BssomWriter(buffer);
                option.FormatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref context, value);
                return buffer.GetBufferedArray();
            }
        }

        /// <summary>
        /// <para>将给定的值序列化到<paramref name="bufferWriter"/></para>
        /// <para>Serializes a given value with the specified <paramref name="bufferWriter"/></para>
        /// </summary>
        /// <param name="value">要序列化的值. The value to serialize</param>
        /// <param name="bufferWriter">要序列化的缓冲区写入器. The buffer writer to serialize with</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        public static void Serialize<T>(T value, IBssomBufferWriter bufferWriter, BssomSerializerOptions option = null, CancellationToken cancellationToken = default)
        {
            if (bufferWriter == null)
                throw new ArgumentNullException(nameof(bufferWriter));

            if (option == null)
                option = BssomSerializerOptions.Default;

            BssomSerializeContext context = new BssomSerializeContext(option, cancellationToken);
            var writer = new BssomWriter(bufferWriter);
            option.FormatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref context, value);
        }

        /// <summary>
        /// <para>将给定的值序列化到内置的缓冲区</para>
        /// <para>Serializes a given value with the built-in buffer</para>
        /// </summary>
        /// <param name="context">序列化所需要的上下文. The context required for the serialization </param>
        /// <param name="value">要序列化的值. The value to serialize</param>
        /// <returns>具有序列化值的字节数组. A byte array with the serialized value</returns>
        public static byte[] Serialize<T>(ref BssomSerializeContext context, T value)
        {
            if (context.Option == null)
                context.Option = BssomSerializerOptions.Default;

            var buffer = ExpandableBufferWriter.CreateGlobar();
            var writer = new BssomWriter(buffer);
            context.Option.FormatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref context, value);
            return buffer.GetBufferedArray();
        }

        /// <summary>
        /// <para>将给定的值序列化到<paramref name="bufferWriter"/></para>
        /// <para>Serializes a given value with the specified <paramref name="bufferWriter"/></para>
        /// </summary>
        /// <param name="context">序列化所需要的上下文. The context required for the serialization </param>
        /// <param name="value">要序列化的值. The value to serialize</param>
        /// <param name="bufferWriter">要序列化的缓冲区写入器. The buffer writer to serialize with</param>
        public static void Serialize<T>(ref BssomSerializeContext context, T value, IBssomBufferWriter bufferWriter)
        {
            if (context.Option == null)
                context.Option = BssomSerializerOptions.Default;

            var writer = new BssomWriter(bufferWriter);
            context.Option.FormatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref context, value);
        }

        /// <summary>
        /// <para>将给定的值序列化到<paramref name="stream"/>中</para>
        /// <para>Serializes a given value with the specified <paramref name="stream"/></para>
        /// </summary>
        /// <param name="context">序列化所需要的上下文. The context required for the serialization </param>
        /// <param name="value">要序列化的值. The value to serialize</param>
        /// <param name="stream">要序列化的流. The stream to serialize with</param>
        public static void Serialize<T>(ref BssomSerializeContext context, T value, Stream stream)
        {
            using (var buffer = ExpandableBufferWriter.CreateGlobar())
            {
                Serialize(ref context, value, buffer);
                try
                {
                    buffer.CopyTo(stream, context.CancellationToken);
                }
                catch (Exception ex)
                {
                    BssomSerializationOperationException.CopyStreamError(ex);
                }
            }
        }

        /// <summary>
        /// <para>将给定的值序列化到<paramref name="stream"/>中</para>
        /// <para>Serializes a given value with the specified <paramref name="stream"/></para>
        /// </summary>
        /// <param name="stream">要序列化的流. The stream to serialize with</param>
        /// <param name="value">要序列化的值. The value to serialize</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>具有序列化值的字节数组. A byte array with the serialized value</returns>
        public static void Serialize<T>(Stream stream, T value, BssomSerializerOptions option = null, CancellationToken cancellationToken = default)
        {
            if (option == null)
                option = BssomSerializerOptions.Default;

            BssomSerializeContext context = new BssomSerializeContext(option, cancellationToken);
            Serialize(ref context, value, stream);
        }

        /// <summary>
        /// <para>异步的将给定的值序列化到<paramref name="stream"/>中</para>
        /// <para>Asynchronously serializes a given value with the specified <paramref name="stream"/></para>
        /// </summary>
        /// <param name="stream">要序列化的流. The stream to serialize with</param>
        /// <param name="value">要序列化的值. The value to serialize</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>具有序列化值的字节数组. A byte array with the serialized value</returns>
        public static async Task SerializeAsync<T>(Stream stream, T value, BssomSerializerOptions option = null, CancellationToken cancellationToken = default)
        {
            if (option == null)
                option = BssomSerializerOptions.Default;

            BssomSerializeContext context = new BssomSerializeContext(option, cancellationToken);
            using (var buffer = ExpandableBufferWriter.CreateGlobar())
            {
                Serialize(ref context, value, buffer);
                try
                {
                    await buffer.CopyToAsync(stream, context.CancellationToken);
                }
                catch (Exception ex)
                {
                    BssomSerializationOperationException.CopyStreamError(ex);
                }
            }
        }

        private static bool TryDeserializeFromMemoryStream<T>(ref BssomDeserializeContext context, Stream stream, out T t)
        {
#if NETSTANDARD && !NET45
            if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> streamBuffer))
            {
                var buffer = new SimpleBufferWriter(streamBuffer.Array, streamBuffer.Offset, streamBuffer.Count);
                t = Deserialize<T>(ref context, buffer);
                stream.Seek(buffer.Position, SeekOrigin.Current);
                return true;
            }
#endif
            t = default;
            return false;
        }

        private static bool TryDeserializeFromMemoryStream(ref BssomDeserializeContext context, Stream stream, Type type, out object t)
        {
#if NETSTANDARD && !NET45
            if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> streamBuffer))
            {
                var buffer = new SimpleBufferWriter(streamBuffer.Array, streamBuffer.Offset, streamBuffer.Count);
                t = Deserialize(ref context, buffer, type);
                stream.Seek(buffer.Position, SeekOrigin.Current);
                return true;
            }
#endif
            t = default;
            return false;
        }

        /// <summary>
        /// <para>从指定的<paramref name="stream"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="stream"/></para>
        /// </summary>
        /// <param name="context">反序列化所需要的上下文. The context required for the deserialization </param>
        /// <param name="stream">要进行反序列化的流. The stream to deserialize from</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static T Deserialize<T>(ref BssomDeserializeContext context, Stream stream)
        {
            if (TryDeserializeFromMemoryStream<T>(ref context, stream, out T result))
            {
                return result;
            }

            var aux = new StreamDeserializeAux(stream);
            result = Deserialize<T>(ref context, aux.GetBssomBuffer());
            aux.Dispose();
            return result;
        }

        /// <summary>
        /// <para>从指定的<paramref name="stream"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="stream"/></para>
        /// </summary>
        /// <param name="stream">反序列化所需要的的流. The stream to deserialize from</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static T Deserialize<T>(Stream stream, BssomSerializerOptions option = null,
            CancellationToken cancellationToken = default)
        {
            BssomDeserializeContext context = new BssomDeserializeContext(option, cancellationToken);
            return Deserialize<T>(ref context, stream);
        }

        /// <summary>
        /// <para>异步的从指定的<paramref name="stream"/>反序列化给定类型的值</para>
        /// <para>Asynchronously deserializes the value of the given type from the specified <paramref name="stream"/></para>
        /// </summary>
        /// <param name="stream">反序列化所需要的的流. The stream to deserialize from</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static async Task<T> DeserializeAsync<T>(Stream stream, BssomSerializerOptions option = null,
            CancellationToken cancellationToken = default)
        {
            BssomDeserializeContext context = new BssomDeserializeContext(option, cancellationToken);
            if (TryDeserializeFromMemoryStream<T>(ref context, stream, out T result))
            {
                return result;
            }

            var aux = new StreamDeserializeAux(stream);
            var bssomBuf = await aux.GetBssomBufferAsync().ConfigureAwait(false);
            result = Deserialize<T>(ref context, bssomBuf);
            aux.Dispose();
            return result;
        }

        /// <summary>
        /// <para>从指定的<paramref name="stream"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="stream"/></para>
        /// </summary>
        /// <param name="context">反序列化所需要的上下文. The context required for the deserialization </param>
        /// <param name="stream">反序列化所需要的的缓冲区. The stream to deserialize from</param>
        /// <param name="type">要反序列化的类型. The type to deserialize</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static object Deserialize(ref BssomDeserializeContext context, Stream stream, Type type)
        {
            if (TryDeserializeFromMemoryStream(ref context, stream, type, out object result))
            {
                return result;
            }

            var aux = new StreamDeserializeAux(stream);
            result = Deserialize(ref context, aux.GetBssomBuffer(), type);
            aux.Dispose();
            return result;
        }

        /// <summary>
        /// <para>从指定的<paramref name="stream"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="stream"/></para>
        /// </summary>
        /// <param name="stream">反序列化所需要的的流. The stream to deserialize from</param>
        /// <param name="type">要反序列化的类型. The type to deserialize</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static object Deserialize(Stream stream, Type type, BssomSerializerOptions option = null,
            CancellationToken cancellationToken = default)
        {
            BssomDeserializeContext context = new BssomDeserializeContext(option, cancellationToken);
            return Deserialize(ref context, stream, type);
        }

        /// <summary>
        /// <para>异步的从指定的<paramref name="stream"/>反序列化给定类型的值</para>
        /// <para>Asynchronously deserializes the value of the given type from the specified <paramref name="stream"/></para>
        /// </summary>
        /// <param name="stream">反序列化所需要的的流. The stream to deserialize from</param>
        /// <param name="type">要反序列化的类型. The type to deserialize</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static async Task<object> DeserializeAsync(Stream stream,Type type, BssomSerializerOptions option = null,
            CancellationToken cancellationToken = default)
        {
            BssomDeserializeContext context = new BssomDeserializeContext(option, cancellationToken);
            if (TryDeserializeFromMemoryStream(ref context, stream, out object result))
            {
                return result;
            }

            var aux = new StreamDeserializeAux(stream);
            var bssomBuf = await aux.GetBssomBufferAsync().ConfigureAwait(false);
            result = Deserialize(ref context, bssomBuf, type);
            aux.Dispose();
            return result;
        }

        /// <summary>
        /// <para>从指定的<paramref name="buffer"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="buffer"/></para>
        /// </summary>
        /// <param name="buffer">反序列化所需要的的缓冲区. The buffer to deserialize from</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static T Deserialize<T>(IBssomBuffer buffer, BssomSerializerOptions option = null, CancellationToken cancellationToken = default)
        {
            if (option == null)
                option = BssomSerializerOptions.Default;

            BssomDeserializeContext context = new BssomDeserializeContext(option, cancellationToken);
            var reader = new BssomReader(buffer);
            return option.FormatterResolver.GetFormatterWithVerify<T>().Deserialize(ref reader, ref context);
        }

        /// <summary>
        /// <para>从指定的<paramref name="buffer"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="buffer"/></para>
        /// </summary>
        /// <param name="context">反序列化所需要的上下文. The context required for the deserialization </param>
        /// <param name="buffer">要进行反序列化的缓冲区. The buffer to deserialize from</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static T Deserialize<T>(ref BssomDeserializeContext context, IBssomBuffer buffer)
        {
            if (context.Option == null)
                context.Option = BssomSerializerOptions.Default;

            var reader = new BssomReader(buffer);
            return context.Option.FormatterResolver.GetFormatterWithVerify<T>().Deserialize(ref reader, ref context);
        }

        /// <summary>
        /// <para>从指定的<paramref name="buffer"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="buffer"/></para>
        /// </summary>
        /// <param name="buffer">反序列化所需要的的二进制数组. The binary array to deserialize from</param>
        /// <param name="start">数组的起始位置. array start</param>
        /// <param name="length">数组的长度. array length</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static T Deserialize<T>(byte[] buffer, int start, int length, BssomSerializerOptions option = null, CancellationToken cancellationToken = default)
        {
            return Deserialize<T>(new SimpleBufferWriter(buffer, start, length), option);
        }

        /// <summary>
        /// <para>从指定的<paramref name="buffer"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="buffer"/></para>
        /// </summary>
        /// <param name="context">反序列化所需要的上下文. The context required for the deserialization </param>
        /// <param name="buffer">反序列化所需要的的二进制数组. The binary array to deserialize from</param>
        /// <param name="start">数组的起始位置. array start</param>
        /// <param name="length">数组的长度. array length</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static T Deserialize<T>(ref BssomDeserializeContext context, byte[] buffer, int start, int length)
        {
            return Deserialize<T>(ref context, new SimpleBufferWriter(buffer, start, length));
        }

        /// <summary>
        /// <para>从指定的<paramref name="buffer"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="buffer"/></para>
        /// </summary>
        /// <param name="buffer">反序列化所需要的的二进制数组. The binary array to deserialize from</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static T Deserialize<T>(byte[] buffer, BssomSerializerOptions option = null, CancellationToken cancellationToken = default)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return Deserialize<T>(buffer, 0, buffer.Length, option);
        }

        /// <summary>
        /// <para>从指定的<paramref name="buffer"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="buffer"/></para>
        /// </summary>
        /// <param name="buffer">反序列化所需要的的缓冲区. The buffer to deserialize from</param>
        /// <param name="type">要反序列化的类型. The type to deserialize</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static object Deserialize(IBssomBuffer buffer, Type type, BssomSerializerOptions option = null, CancellationToken cancellationToken = default)
        {
            BssomDeserializeContext context = new BssomDeserializeContext(option, cancellationToken);
            return Deserialize(ref context, buffer, type);
        }

        /// <summary>
        /// <para>从指定的<paramref name="buffer"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="buffer"/></para>
        /// </summary>
        /// <param name="buffer">反序列化所需要的的二进制数组. The binary array to deserialize from</param>
        /// <param name="start">数组的起始位置. array start</param>
        /// <param name="length">数组的长度. array length</param>
        /// <param name="type">要反序列化的类型. The type to deserialize</param>
        /// <param name="option">使用的配置,如果为<c>null</c>,则使用默认配置. The options. Use <c>null</c> to use default options</param>
        /// <param name="cancellationToken">取消该操作的令牌. The token to cancel the operation</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static object Deserialize(byte[] buffer, int start, int length, Type type, BssomSerializerOptions option = null, CancellationToken cancellationToken = default)
        {
            return Deserialize(new SimpleBufferWriter(buffer, start, length), type, option);
        }

        /// <summary>
        /// <para>从指定的<paramref name="buffer"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="buffer"/></para>
        /// </summary>
        /// <param name="context">反序列化所需要的上下文. The context required for the deserialization </param>
        /// <param name="buffer">反序列化所需要的的二进制数组. The binary array to deserialize from</param>
        /// <param name="start">数组的起始位置. array start</param>
        /// <param name="length">数组的长度. array length</param>
        /// <param name="type">要反序列化的类型. The type to deserialize</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static object Deserialize(ref BssomDeserializeContext context, byte[] buffer, int start, int length, Type type)
        {
            return Deserialize(ref context, new SimpleBufferWriter(buffer, start, length), type);
        }

        /// <summary>
        /// <para>从指定的<paramref name="buffer"/>反序列化给定类型的值</para>
        /// <para>Deserializes the value of the given type from the specified <paramref name="buffer"/></para>
        /// </summary>
        /// <param name="context">反序列化所需要的上下文. The context required for the deserialization </param>
        /// <param name="buffer">反序列化所需要的的缓冲区. The buffer to deserialize from</param>
        /// <param name="type">要反序列化的类型. The type to deserialize</param>
        /// <returns>反序列化的值. The deserialized value</returns>
        public static object Deserialize(ref BssomDeserializeContext context, IBssomBuffer buffer, Type type)
        {
            if (context.Option == null)
                context.Option = BssomSerializerOptions.Default;

            var reader = new BssomReader(buffer);
            return RawObjectDeserializer.Deserialize(type, ref reader, ref context);
        }
    }
}
