using ProtoBuf;
using System.IO;

namespace BssomSerializers.Benchmark
{
    public class ProtobufNet : SerializerBase
    {
        public override T Deserialize<T>(byte[] input)
        {
            using (var ms = new MemoryStream(input))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public override T DeserializeStream<T>(byte[] input)
        {
            using (MemoryStream stream = new MemoryStream(input))
                return Serializer.Deserialize<T>(stream);
        }
        
        public override byte[] Serialize<T>(T input)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, input);
                return ms.ToArray();
            }
        }

        public override void SerializeStream<T>(T input)
        {
            using (MemoryStream stream = new MemoryStream())
                Serializer.Serialize<T>(stream, input);
        }
    }
}
