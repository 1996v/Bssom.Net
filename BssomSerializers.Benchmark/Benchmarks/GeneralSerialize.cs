using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;

namespace BssomSerializers.Benchmark
{
    [Config(typeof(BenchmarkConfig))]
    public class GeneralSerialize<T>
    {
        [ParamsSource(nameof(Serializers))]
        public SerializerBase Serializer;

        public IEnumerable<SerializerBase> Serializers => new SerializerBase[]
        {
            new BssomSerializers(),
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

        [Benchmark]
        public void SerializeStream()
        {
            Serializer.SerializeStream(Input);
        }

        [Benchmark]
        public void DeserializeStream()
        {
            Serializer.DeserializeStream<T>(OutPut);
        }
    }
}
