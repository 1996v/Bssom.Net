using Bssom.Serializer.BssMap;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Bssom.Serializer.Resolvers;
using Bssom.Serializer.Binary;
using Xunit;
namespace Bssom.Serializer.Test
{
    public partial class MapCodeGenResolverTest
    {
        [Fact]
        public void ResolverGetFormatter_CustomClassTypeImpl_IsDynamicGenerateFormatterType()
        {
            MapCodeGenResolver.Instance.GetFormatter<_Class_1>().IsNotNull();
            MapCodeGenResolver.Instance.GetFormatter<_Class_2<object, int>>().IsNotNull();
        }

        [Fact]
        public void ResolverGetFormatter_CustomStructTypeImpl_IsDynamicGenerateFormatterType()
        {
            MapCodeGenResolver.Instance.GetFormatter<_Struct_1>().IsNotNull();
            MapCodeGenResolver.Instance.GetFormatter<_Struct_2<object, int>>().IsNotNull();
        }

        [Fact]
        public void CustomGenericClassType_FormatterIsCorrectly()
        {
            var val2 = RandomHelper.RandomValue<_Class_2<int, object>>();
            VerifyHelper.ConvertObjectAndVerifyEntity(val2);
        }

        [Fact]
        public void CustomClassType_NonField_FormatterIsCorrectly()
        {
            var val3 = RandomHelper.RandomValue<_Class_3>();
            VerifyHelper.ConvertObjectAndVerifyEntity(val3);
        }

        [Fact]
        public void CustomClassType_FormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<_Class_1>();
            VerifyHelper.ConvertObjectAndVerifyEntity(val);

            var val2 = RandomHelper.RandomValue<_Class_5>();
            VerifyHelper.ConvertObjectAndVerifyEntity(val2);

            var val3 = RandomHelper.RandomValue<_Class_4>();
            VerifyHelper.ConvertObjectAndVerifyEntity(val3);
        }

        [Fact]
        public void CustomStructType_FormatterIsCorrectly()
        {
            var val1 = RandomHelper.RandomValue<_Struct_1>();
            VerifyHelper.ConvertObjectAndVerifyEntity(val1);

            var val2 = RandomHelper.RandomValue<_Struct_2<int, object>>();
            VerifyHelper.ConvertObjectAndVerifyEntity(val2);
        }


        public struct _Map1_1
        {
            public int A1;
            public int A3;
        }
        [Fact]
        public void Map1Deserialize_AtSameLayer_IfNotEqualThenSKip()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("A1", 1);
            dic.Add("A2", 2);

            var buf = BssomSerializer.Serialize(dic);
            var c1 = BssomSerializer.Deserialize<_Map1_1>(buf);
            c1.A1.Is(1);
            c1.A3.Is(0);
        }

        public struct _Map1_1_1
        {
            public int A1;
            public int A3;
            public int B1234567;
            public int C12345678;
        }

        [Fact]
        public void Map1Deserialize_MultiAtSameLayer_IfNotEqualThenSKip()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("A1", 1);
            dic.Add("A2", 2);
            dic.Add("B12345678", 3);
            dic.Add("C12345678", 4);
            var buf = BssomSerializer.Serialize(dic);
            var c1 = BssomSerializer.Deserialize<_Map1_1>(buf);
            c1.A1.Is(1);
            c1.A3.Is(0);
        }

        public struct _Map1_2
        {
            public int A1234567;
            public int A12345679;
        }

        [Fact]
        public void Map1Deserialize_NotSameLayer_IfNotEqualThenSKip()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("A1234567", 1);
            dic.Add("A12345678", 2);

            var buf = BssomSerializer.Serialize(dic);
            var c1 = BssomSerializer.Deserialize<_Map1_2>(buf);
            c1.A1234567.Is(1);
            c1.A12345679.Is(0);
        }

        public struct _Map1_3
        {
            public int A1234567;
            public int A12345679;
            public int B1234567;
            public int B12345679;
        }
        [Fact]
        public void Map1Deserialize_MultiNotSameLayer_IfNotEqualThenSKip()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("A1234567", 1);
            dic.Add("A12345678", 2);
            dic.Add("B1234567", 3);
            dic.Add("B12345678", 4);

            var buf = BssomSerializer.Serialize(dic);
            var c1 = BssomSerializer.Deserialize<_Map1_3>(buf);
            c1.A1234567.Is(1);
            c1.A12345679.Is(0);
            c1.B1234567.Is(3);
            c1.B12345679.Is(0);
        }

        public struct _Map1_4
        {
            public int A123;
            public int B123;
            public int C123;
            public int D123;
        }

        [Fact]
        public void Map1Deserialize_KeyIsNotNextLayer()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("A123", 1);
            dic.Add("B123", 2);
            dic.Add("C123", 3);
            dic.Add("D123", 4);

            var buf = BssomSerializer.Serialize(dic);
            var c1 = BssomSerializer.Deserialize<_Map1_4>(buf);
            c1.A123.Is(1);
            c1.B123.Is(2);
            c1.C123.Is(3);
            c1.D123.Is(4);
        }

        public struct _Map1_4_1
        {
        }

        [Fact]
        public void Map1Deserialize_NonField_FormatterIsCorrectly()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("A12345678", 1);
            dic.Add("B12345678", 2);
            dic.Add("C12345678", 3);

            var buf = BssomSerializer.Serialize(dic);
            var c1 = BssomSerializer.Deserialize<_Map1_4_1>(buf);
        }

        public struct _Map1_5
        {
            public int A12345678;
            public int B12345678;
            public int C12345678;
        }

        [Fact]
        public void Map1Deserialize_KeyIsNotFirstLayer()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("A12345678", 1);
            dic.Add("B12345678", 2);
            dic.Add("C12345678", 3);

            var buf = BssomSerializer.Serialize(dic);
            var c1 = BssomSerializer.Deserialize<_Map1_5>(buf);
            c1.A12345678.Is(1);
            c1.B12345678.Is(2);
            c1.C12345678.Is(3);
        }

        [Fact]
        public void Map1Deserialize_FormatterIsCorrectly()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(nameof(_Class_1.A1), (int)3);
            dic.Add(nameof(_Class_1.B1), (long)3);
            dic.Add(nameof(_Class_1.C1), (short)3);
            dic.Add(nameof(_Class_1.D1), (byte)3);
            dic.Add(nameof(_Class_1._1234567A), "f");
            dic.Add(nameof(_Class_1._2234567B), Guid.NewGuid());
            dic.Add(nameof(_Class_1._3234567C), DateTime.Now);

            var buf = BssomSerializer.Serialize(dic);
            var c1 = BssomSerializer.Deserialize<_Class_1>(buf);
            c1.A1.Is(dic["A1"]);
            c1.B1.Is(dic["B1"]);
            c1.C1.Is(dic["C1"]);
            c1.D1.Is(dic["D1"]);
            c1._1234567A.Is(dic["_1234567A"]);
            c1._2234567B.Is(dic["_2234567B"]);
            c1._3234567C.Is(dic["_3234567C"]);
        }

        [Fact]
        public void Map1Deserialize_OneZeroAndOneNotZero()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("A", 1);
            dic.Add("B12345678", 2);
            var buf = BssomSerializer.Serialize(dic);
            var c1 = BssomSerializer.Deserialize<_Map1_6>(buf);
            c1.A.Is(1);
            c1.B12345678.Is(2);

        }

        public struct _Map1_6
        {
            public int A;
            public int B12345678;
        }

        [Fact]
        public void Map2Deserialize_TryGetSlowLogicIsCorrectly()
        {
            var val = RandomHelper.RandomValue<_Class_1>();
            var buf = BssomSerializer.Serialize(val);
            var c1 = BssomSerializer.Deserialize<_Class_1>(new SlowBssomBuffer(buf));
            val.A1.Is(c1.A1);
            val.B1.Is(c1.B1);
            val.C1.Is(c1.C1);
            val.D1.Is(c1.D1);
            val._1234567A.Is(c1._1234567A);
            val._2234567B.Is(c1._2234567B);
            val._3234567C.Is(c1._3234567C);
        }

        [Fact]
        public void Map2Deserialize_EnumeratorSlowIsCorrectly()
        {
            var val = RandomHelper.RandomValue<_Class_1>();
            var buf = BssomSerializer.Serialize(val);
            var c1 = BssomSerializer.Deserialize<Dictionary<string, object>>(new SlowBssomBuffer(buf));
            val.A1.Is(c1["A1"]);
            val.B1.Is(c1["B1"]);
            val.C1.Is(c1["C1"]);
            val.D1.Is(c1["D1"]);
            val._1234567A.Is(c1["_1234567A"]);
            val._2234567B.Is(c1["_2234567B"]);
            val._3234567C.Is(c1["_3234567C"]);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map2Deserialize_NestDictionaryTest(bool b)
        {
            var val = new _NestClass0()
            {
                A1 = "a",
                B1 = new Dictionary<int, int>() { { 1, 2 }, { 3, 4 } },
                C1 = "c"
            };
            var buf = BssomSerializer.Serialize(val,
                BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(b));
            var c1 = BssomSerializer.Deserialize<_NestClass0>(buf);

            val.A1.Is("a");
            val.B1.Count.Is(2);
            val.B1[1].Is(2);
            val.B1[3].Is(4);
            val.C1.Is("c");
        }

        [Fact]
        public void Map2Deserialize_NestStructTest()
        {
            var val = new _NestClass11()
            {
                B1 = new _Struct_1()
                {
                    A1 = 1,
                    B1 = 2,
                    C1 = 3,
                    _1234567A = "ff",
                },
                A1 = 3,
                C1 = "ww"
            };
            var buf = BssomSerializer.Serialize(val);
            var val2 = BssomSerializer.Deserialize<_NestClass11>(buf);

            val.A1.Is(val2.A1);
            val.C1.Is(val2.C1);
            val.B1.A1.Is(val2.B1.A1);
            val.B1.B1.Is(val2.B1.B1);
            val.B1.C1.Is(val2.B1.C1);
            val.B1._1234567A.Is(val2.B1._1234567A);
        }

        [Fact]
        public void Map2_MultipleFieldClassTest()
        {
            var val = RandomHelper.RandomValueWithOutStringEmpty<MultipleFieldClass>();
            var buf = VerifyHelper.VerifyEntityWithMap2(val);
        }

        [Fact]
        public void Map1Deserialize_MultipleFieldClassTest()
        {
            var val = RandomHelper.RandomValueWithOutStringEmpty<MultipleFieldClass>();
            var dict = val.GetPublicMembersWithDynamicObject().ToIDict();
            var buf = BssomSerializer.Serialize(dict);
            var val2 = BssomSerializer.Deserialize<MultipleFieldClass>(buf);

            VerifyHelper.VerifyWithJson(val, val2);
        }

        [Fact]
        public void MapCodeGenOnlySerializePublicElement()
        {
            _PrivateSet w = new _PrivateSet();
            w.Assgin(1, 3, true);
            var buf = BssomSerializer.Serialize(w);
            var w2 = BssomSerializer.Deserialize<_PrivateSet>(buf);
            w2.A.Is(0);
            w2.B.Is(3);
            w2.C.IsFalse();
        }

        public class _Class_1
        {
            public int A1;
            public long B1;
            public short C1;
            public byte D1;
            public string _1234567A { get; set; }
            public Guid _2234567B { get; set; }
            public DateTime _3234567C { get; set; }
        }

        public class _Class_2<T1, T2>
        {
            public T1 A1;
            public T2 B1;
            public short C1;
            public byte D1;
            public string _1234567A { get; set; }
            public Guid _2234567B { get; set; }
            public DateTime _3234567C { get; set; }
            public List<string> _3234567C1 { get; set; }
        }

        public class _Class_3
        {
        }

        public class _Class_4
        {
            public short C1;
            public byte D1;
        }

        public class _Class_5
        {
            public short C1sdfsdfsf;
            public byte D1fffsdfsd;
            public byte D2;
            public byte D3;
        }

        public class _PrivateSet
        {
            public long A { get; private set; }
            public int B { get; set; }
            public bool C { get; internal set; }

            public void Assgin(long a, int b, bool c)
            {
                A = a;
                B = b;
                C = c;
            }
        }


        public struct _Struct_2<T1, T2>
        {
            public T1 A1;
            public T2 B1;
            public short C1;
            public byte D1;
            public string _1234567A { get; set; }
            public Guid _2234567B { get; set; }
            public DateTime _3234567C { get; set; }
            public List<string> _3234567C1 { get; set; }
        }

        public class _NestClass0
        {
            public string A1;
            public Dictionary<int, int> B1;
            public string C1;
        }

        public class _NestClass11
        {
            public int A1;
            public _Struct_1 B1;
            public string C1;
        }
    }
    public struct _Struct_1
    {
        public int A1;
        public long B1;
        public short C1;
        public byte D1;
        public string _1234567A { get; set; }
        public Guid _2234567B { get; set; }
        public DateTime _3234567C { get; set; }
    }

    public class MultipleFieldClass
    {
        public int PrimitiveField_1;
        public long PrimitiveField_2;
        public short PrimitiveField_3;
        public string PrimitiveField_4;
        public short PrimitiveField_5;
        public Guid PrimitiveField_6;
        public DateTime PrimitiveField_7;
        public Dictionary<string, object> PrimitiveField_8;
        public List<object> PrimitiveField_9;
        public byte[] PrimitiveField_10;
        public float PrimitiveField_11;
        public double PrimitiveField_12;
        public decimal PrimitiveField_13;
        public byte[] PrimitiveField_14;
        public float PrimitiveField_15;
        public double PrimitiveField_16;
        public decimal PrimitiveField_17;
        public int primitiveField_1;
        public long primitiveField_2;
        public short primitiveField_3;
        public string primitiveField_4;
        public short primitiveField_5;
        public Guid primitiveField_6;
        public DateTime primitiveField_7;
        public Dictionary<string, object> primitiveField_8;
        public List<object> primitiveField_9;
        public byte[] primitiveField_10;
        public float primitiveField_11;
        public double primitiveField_12;
        public decimal primitiveField_13;
        public byte[] primitiveField_14;
        public float primitiveField_15;
        public double primitiveField_16;
        public decimal primitiveField_17;
    }


    public class MultipleFieldClass2
    {
        public int eField_1;
        public long eField_2;

    }
}
