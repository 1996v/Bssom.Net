using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Bssom.Serializer.Benchmark
{
    [ProtoContract]
    public class BenchmarkComplexClass2
    {
        [ProtoMember(1)]
        public int OrderId = 1024;
        [ProtoMember(2)]
        public string CustomerId = "abce";
        [ProtoMember(3)]
        public long EmployeeId = 9;
        [ProtoMember(4)]
        public DateTime OrderData = DateTime.Now;
        [ProtoMember(5)]
        public DateTime ShippedData = DateTime.Now;
        [ProtoMember(6)]
        public long ShipVia = 999;
        [ProtoMember(7)]
        public double Freight = 0.23;
        [ProtoMember(8)]
        public string ShipName = "hernyfan";
        [ProtoMember(9)]
        public string ShipAddress = "guangdong";
        [ProtoMember(10)]
        public string ShipCity = "gz";
        [ProtoMember(11)]
        public string ShipRegion = "abc";
    }


    [ProtoContract]
    public class BenchmarkComplexClass
    {
        [ProtoMember(1)]
        public int PrimitiveField_1;
        [ProtoMember(2)]
        public long PrimitiveField_2;
        [ProtoMember(3)]
        public short PrimitiveField_3;
        [ProtoMember(4)]
        public string PrimitiveField_4;
        [ProtoMember(5)]
        public _Enum PrimitiveField_5;
        [ProtoMember(6)]
        public Guid PrimitiveField_6;
        [ProtoMember(7)]
        public DateTime PrimitiveField_7;
        [ProtoMember(8)]
        public Dictionary<int, long> PrimitiveField_8;
        [ProtoMember(9)]
        public List<char> PrimitiveField_9;
        [ProtoMember(10)]
        public byte[] PrimitiveField_10;
        [ProtoMember(11)]
        public float PrimitiveField_11;
        [ProtoMember(12)]
        public double PrimitiveField_12;
        [ProtoMember(13)]
        public decimal PrimitiveField_13;
    }


}
