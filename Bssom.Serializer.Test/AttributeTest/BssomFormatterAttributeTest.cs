using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Bssom.Serializer.Resolver;
using Bssom.Serializer.BssomBuffer;
using Xunit;
using Xunit.Abstractions;

namespace Bssom.Serializer.Test
{
    public class BssomFormatterAttributeTest
    {
        [Fact]
        public void ClassTypeFormatterAttribute_Test()
        {
            var val2 = RandomHelper.RandomValue<Attribute_Class_3BssomFormatter>();
            VerifyHelper.VerifyEntityWithMap2(val2);
        }

        [BssomFormatter(typeof(Attribute_Class_3BssomFormatter))]
        public class _Attribute_Class_3
        {
            public string Name;
            public string Address;
            public int Age;
            public int Sex;

            internal string GenerateToken() => Name + Address;
            internal long GeneratePwd() => Age + Sex;
        }

        public class BssomSerializerOptionsExtend : BssomSerializerOptions
        {
            public BssomSerializerOptionsExtend(int level) { Level = level; }
            public int Level { get; }

            public void SecurityVerification(string token, long pwd)
            {
                switch (Level)
                {
                    case 1:
                        //logic -> Pass
                        break;
                    case 2:
                        //logic -> No Pass -> throw
                        throw new System.Exception();
                }
            }
        }


        public class Attribute_Class_3BssomFormatter : IBssomFormatter<_Attribute_Class_3>
        {
            public _Attribute_Class_3 Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
            {
                return MapCodeGenResolver.Instance.GetFormatter<_Attribute_Class_3>().Deserialize(ref reader, ref context);
            }

            public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, _Attribute_Class_3 value)
            {
                SecurityVerification(value, context.Option);

                MapCodeGenResolver.Instance.GetFormatter<_Attribute_Class_3>().Serialize(ref writer, ref context, value);
            }

            public int Size(ref BssomSizeContext context, _Attribute_Class_3 value)
            {
                SecurityVerification(value, context.Option);

                return MapCodeGenResolver.Instance.GetFormatter<_Attribute_Class_3>().Size(ref context, value);
            }

            public void SecurityVerification(_Attribute_Class_3 value, BssomSerializerOptions option)
            {
                if (value != null)
                {
                    var exOption = (BssomSerializerOptionsExtend)option;
                    exOption.SecurityVerification(value.GenerateToken(), value.GeneratePwd());
                }
            }
        }


        [Fact]
        public void FieldTypeFormatterAttribute_Test()
        {
            var val = new _Attribute_Class_4()
            {
                Name = "a",
                Address = "b",
                Age = 10,
                Sex = 1
            };
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Size(val).Is(buf.Length);
            var t = BssomSerializer.Deserialize<_Attribute_Class_4>(buf);

            t.Name.Is("a");
            t.Address.Is("b");
            t.Age.Is(6);
            t.Sex.Is(1);
        }

        [Fact]
        public void FieldTypeFormatterAttribute_SlowDeserialize_Test()
        {
            var val = new _Attribute_Class_4()
            {
                Name = "a",
                Address = "b",
                Age = 10,
                Sex = 1
            };
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Size(val).Is(buf.Length);
            var t = BssomSerializer.Deserialize<_Attribute_Class_4>(new SlowBssomBuffer(buf));
            t.Name.Is("a");
            t.Address.Is("b");
            t.Age.Is(6);
            t.Sex.Is(1);
        }

        [Fact]
        public void FieldTypeFormatterAttribute_Map1Deserialize_Test()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>() {
                { "Name","a"},
                { "Age",10 }
            };
            var buf = BssomSerializer.Serialize(dict);
            var t = BssomSerializer.Deserialize<_Attribute_Class_4>(buf);
            t.Name.Is("a");
            t.Age.Is(11);
        }

        public class _Attribute_Class_55
        {
            public int Age;
        }
        

        private ITestOutputHelper _o;
        public BssomFormatterAttributeTest(ITestOutputHelper output)
        {
            this._o = output;
        }


        public class _Attribute_Class_4
        {
            public string Name;
            public string Address;
            [BssomFormatter(typeof(SubtractFiveBssomFormatter))]
            public int Age;
            public int Sex;
        }

        public class SubtractFiveBssomFormatter : IBssomFormatter<int>
        {
            public int Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
            {
                return PrimitiveResolver.Instance.GetFormatter<int>().Deserialize(ref reader, ref context) + 1;
            }

            public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, int value)
            {
                value -= 5;
                PrimitiveResolver.Instance.GetFormatter<int>().Serialize(ref writer, ref context, value);
            }

            public int Size(ref BssomSizeContext context, int value)
            {
                return PrimitiveResolver.Instance.GetFormatter<int>().Size(ref context, value);
            }
        }
    }
}
