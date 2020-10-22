using MessagePack;
using MessagePack.Resolvers;
using System.IO;

namespace BssomSerializers.Benchmark
{
    public class MessagePack : SerializerBase
    {
        static MessagePackSerializerOptions Option =
               MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);

        public override T Deserialize<T>(byte[] input)
        {
            return MessagePackSerializer.Deserialize<T>(input, Option);
        }

        public override T DeserializeStream<T>(byte[] input)
        {
            using (MemoryStream stream = new MemoryStream(input))
                return MessagePackSerializer.Deserialize<T>(stream, Option);
        }

        public override byte[] Serialize<T>(T input)
        {
            return MessagePackSerializer.Serialize<T>(input, Option);
        }

        public override void SerializeStream<T>(T input)
        {
            using (MemoryStream stream = new MemoryStream())
                MessagePackSerializer.Serialize<T>(stream, input, Option);
        }
    }
}
