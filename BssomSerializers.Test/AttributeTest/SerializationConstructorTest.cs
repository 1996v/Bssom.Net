using System;
using System.Collections.Generic;
using Xunit;
namespace Bssom.Serializer.Test
{
    public class SerializationConstructorTest
    {
        [Fact]
        public void ClassType_NoPublicCtorIsUnsupported_Test()
        {
            var val2 = _Attribute_Class_1.Create();
            VerifyHelper.Throws<TypeInitializationException>(() => BssomSerializer.Serialize(val2));
        }

        [Fact]
        public void ClassType_DefaultPopulateConstructParameters_Test()
        {
            var val2 = new _Attribute_Class_2(0);
            val2.Orgin.Is(4);

            var buf = BssomSerializer.Serialize(val2);
            var val3 = BssomSerializer.Deserialize<_Attribute_Class_2>(buf);
            val3.Orgin.Is(4);
        }

        public class _Attribute_Class_1
        {
            public string Orgin;
            public int Code;

            [SerializationConstructor]
            private _Attribute_Class_1()
            {
            }

            public static _Attribute_Class_1 Create() => new _Attribute_Class_1();
        }

        public class _Attribute_Class_2
        {
            public int Orgin;


            public _Attribute_Class_2(int orgin)
            {
                Orgin = 4;
            }

        }
    }
}
