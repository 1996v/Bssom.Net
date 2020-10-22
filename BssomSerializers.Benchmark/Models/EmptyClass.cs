using ProtoBuf;

namespace BssomSerializers.Benchmark
{
    [ProtoContract]
    public class EmptyClass
    {
        [ProtoMember(1)]
        public byte Empty;
    }


}
