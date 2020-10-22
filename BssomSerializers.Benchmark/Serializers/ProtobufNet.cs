using ProtoBuf;
using System.IO;

namespace Bssom.Serializer.Benchmark
{
    public class ProtobufNet : SerializerBase
    {
        public override T Deserialize<T>(byte[] input)
        {
            using (var ms = new MemoryStream(input))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }

        public override T DeserializeStream<T>(byte[] input)
        {
            using (MemoryStream stream = new MemoryStream(input))
                return ProtoBuf.Serializer.Deserialize<T>(stream);
        }
        
        public override byte[] Serialize<T>(T input)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, input);
                return ms.ToArray();
            }
        }

        public override void SerializeStream<T>(T input)
        {
            using (MemoryStream stream = new MemoryStream())
                ProtoBuf.Serializer.Serialize<T>(stream, input);
        }
    }
}
