using ProtoBuf;
using System;
using System.Collections.Generic;

namespace BssomSerializers.Benchmark
{
    [ProtoContract]
    public class PrimitiveFieldComplexClass
    {
        [ProtoMember(1)]
        public int PrimitiveField_1;
        [ProtoMember(2)]
        public long PrimitiveField_2;
        [ProtoMember(3)]
        public short PrimitiveField_3;
        [ProtoMember(4)]
        public char PrimitiveField_4;
        [ProtoMember(5)]
        public float PrimitiveField_5;
        [ProtoMember(6)]
        public double PrimitiveField_6;
    }
}
