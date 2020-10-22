using ProtoBuf;

namespace Bssom.Serializer.Benchmark
{
    [ProtoContract]
    public class EmptyClass
    {
        [ProtoMember(1)]
        public byte Empty;
    }


}
