using System.IO;

namespace BssomSerializers.Benchmark
{
    public class BssomSerializers : SerializerBase
    {
        public override T Deserialize<T>(byte[] input)
        {
            return BssomSerializer.Deserialize<T>(input);
        }

        public override T DeserializeStream<T>(byte[] input)
        {
            using (MemoryStream stream = new MemoryStream(input))
                return BssomSerializer.Deserialize<T>(stream);
        }

        public override byte[] Serialize<T>(T input)
        {
            return BssomSerializer.Serialize<T>(input);
        }

        public override void SerializeStream<T>(T input)
        {
            using (MemoryStream stream = new MemoryStream()) 
                BssomSerializer.Serialize<T>(stream, input);
        }
    }
}
