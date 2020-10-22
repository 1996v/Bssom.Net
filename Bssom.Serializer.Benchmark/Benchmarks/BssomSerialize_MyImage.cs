using BenchmarkDotNet.Attributes;
using System;

namespace Bssom.Serializer.Benchmark
{
    [Config(typeof(BenchmarkConfig))]
    public class BssomSerialize_MyImage
    {
        static MyImage Input;
        static byte[] OutPut;
        static BssomFieldOffsetInfo UpdateDateOffsetInfo;

        static BssomSerialize_MyImage()
        {
            Input = RandomHelper.RandomValueWithOutStringEmpty<MyImage>();
            Input.Binary = new byte[4096 * 200];
            OutPut = BssomSerializer.Serialize(Input);
            UpdateDateOffsetInfo = BssomFieldMarshaller.IndexOf(OutPut, 0, OutPut.Length, "[UpdateTime]");
        }

        [Benchmark]
        public void Serialize()
        {
            BssomSerializer.Serialize(Input);
        }

        [Benchmark]
        public void Deserialize()
        {
            BssomSerializer.Deserialize<MyImage>(OutPut);
        }

        [Benchmark]
        public void Size()
        {
            BssomSerializer.Size(Input);
        }

        [Benchmark]
        public void ReadField()
        {
            BssomFieldMarshaller.ReadValue<DateTime>(OutPut, 0, OutPut.Length, UpdateDateOffsetInfo);
        }

        [Benchmark]
        public void WriteField()
        {
            BssomFieldMarshaller.TryWrite(OutPut, 0, OutPut.Length, UpdateDateOffsetInfo, DateTime.Now);
        }

        [Benchmark]
        public void GetAllFieldMetas()
        {
            foreach (var item in BssomFieldMarshaller.ReadAllKeysByMapType<string>(OutPut, 0, OutPut.Length, BssomFieldOffsetInfo.Zero))
            {
            }
        }

        [Benchmark]
        public void ArrayElementIndexOf()
        {
            BssomFieldMarshaller.IndexOf(OutPut, 0, OutPut.Length, "[ColdDatas]$1");
        }

        [Benchmark]
        public void FieldIndexOf()
        {
            BssomFieldMarshaller.IndexOf(OutPut, 0, OutPut.Length, "[ColdDatas]");
        }
    }
}
