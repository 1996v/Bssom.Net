using System.IO;

namespace Bssom.Serializer.Benchmark
{
    public class BssomSerializer : SerializerBase
    {
        public override T Deserialize<T>(byte[] input)
        {
            return Serializer.BssomSerializer.Deserialize<T>(input);
        }

        public override T DeserializeStream<T>(byte[] input)
        {
            using (MemoryStream stream = new MemoryStream(input))
                return Serializer.BssomSerializer.Deserialize<T>(stream);
        }

        public override byte[] Serialize<T>(T input)
        {
            return Serializer.BssomSerializer.Serialize<T>(input);
        }

        public override void SerializeStream<T>(T input)
        {
            using (MemoryStream stream = new MemoryStream()) 
                Serializer.BssomSerializer.Serialize<T>(stream, input);
        }
    }
}
