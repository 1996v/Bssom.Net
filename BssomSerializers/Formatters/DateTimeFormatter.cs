using System;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.BssMap;

namespace Bssom.Serializer.Formatters
{
    /// <summary>
    /// Format <see cref="DateTime"/> as BssomType.DateTime or NativeBssomType.DateTime
    /// </summary>
    public sealed class DateTimeFormatter : IBssomFormatter<DateTime>
    {
        public static readonly DateTimeFormatter Instance = new DateTimeFormatter();

        private DateTimeFormatter()
        {
        }
        public void Deserialize1(ref BssomReader reader)
        {
            reader.BssomBuffer.TryReadFixedRef(3, out bool r);
        }
        public int Size(ref BssomSizeContext context, DateTime value)
        {
            return BssomBinaryPrimitives.DateTimeSize(value,context.Option.IsUseStandardDateTime);
        }

        public DateTime Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadDateTime();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, DateTime value)
        {
            writer.Write(value,context.Option.IsUseStandardDateTime);
        }
    }
}
