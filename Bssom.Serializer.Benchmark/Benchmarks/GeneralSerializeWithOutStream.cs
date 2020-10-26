using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Bssom.Serializer.Benchmark
{
    [Config(typeof(BenchmarkConfig))]
    public class GeneralSerializeWithOutStream<T>
    {
        [ParamsSource(nameof(Serializers))]
        public SerializerBase Serializer;

        public IEnumerable<SerializerBase> Serializers => new SerializerBase[]
        {
            new BssomSerializer(),
            new MessagePack(),
            new ProtobufNet(),
        };

        static T Input = RandomHelper.RandomValueWithOutStringEmpty<T>();
        static byte[] OutPut;

        [GlobalSetup]
        public void Setup()
        {
            OutPut = Serializer.Serialize<T>(Input);
        }

        [Benchmark]
        public void Serialize()
        {
            Serializer.Serialize(Input);
        }

        [Benchmark]
        public void Deserialize()
        {
            Serializer.Deserialize<T>(OutPut);
        }

     
    }
}
