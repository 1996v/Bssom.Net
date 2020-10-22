using System;

namespace Bssom.Serializer.Benchmark
{
    public class MyImage
    {
        public int Id;
        public Guid UUID;
        public string Path;
        public byte[] Binary;
        public DateTime CreateTime;
        public DateTime UpdateTime;
        public float Version;
        public int[] ColdDatas;
    }
}
