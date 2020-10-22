using System.IO;

namespace Bssom.Serializer.Benchmark
{
    public abstract class SerializerBase
    {
        public abstract byte[] Serialize<T>(T input);

        public abstract T Deserialize<T>(byte[] input);

        public abstract void SerializeStream<T>(T input);

        public abstract T DeserializeStream<T>(byte[] input);
    }
}
