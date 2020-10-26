using Bssom.Serializer.Binary;
namespace Bssom.Serializer.Formatters
{
    /// <summary>
    /// Format <see cref="T"/>? as BssomType.Null or BssomValue
    /// </summary>
    public sealed class NullableFormatter<T> : IBssomFormatter<T?>
    where T : struct
    {
        public NullableFormatter()
        {

        }

        public T? Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
            {
                return default;
            }

            return context.Option.FormatterResolver.GetFormatterWithVerify<T>().Deserialize(ref reader, ref context);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, T? value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            context.Option.FormatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref context, value.Value);
        }

        public int Size(ref BssomSizeContext context, T? value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return context.Option.FormatterResolver.GetFormatterWithVerify<T>().Size(ref context, value.Value);
        }
    }
}
