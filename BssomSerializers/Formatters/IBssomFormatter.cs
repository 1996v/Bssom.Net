using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;

namespace Bssom.Serializer
{
    /// <summary>
    /// A base interface for <see cref="IBssomFormatter{T}"/> so that all generic implementations can be detected by a common base type.
    /// </summary>
    public interface IBssomFormatter
    {
    }
    /// <summary>
    /// The contract for serialization of some specific type.
    /// </summary>
    /// <typeparam name="T">The type to be serialized or deserialized</typeparam>
    public interface IBssomFormatter<T> : IBssomFormatter
    {
        /// <summary>
        /// Gets the serialized size of the value
        /// </summary>
        /// <param name="context">The size context to use when getting the serialized size</param>
        /// <param name="value">The value to be size</param>
        /// <returns>the serialized size of the value</returns>
        int Size(ref BssomSizeContext context, T value);

        /// <summary>
        /// Serializes a value
        /// </summary>
        /// <param name="writer">The writer to use when serializing the value</param>
        /// <param name="context">The serialization context to use when serializing the value</param>
        /// <param name="value">The value to be serialized</param>
        void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, T value);

        /// <summary>
        /// Deserializes a value
        /// </summary>
        /// <param name="reader">The reader to deserialize from</param>
        /// <param name="context">The deserialization context to use when deserializing the value</param>
        /// <returns>The deserialized value</returns>
        T Deserialize(ref BssomReader reader, ref BssomDeserializeContext context);
    }
}
