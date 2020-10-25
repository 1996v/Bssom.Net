using Bssom.Serializer.BssMap;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.Internal;
using Bssom.Serializer.Resolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;
namespace Bssom.Serializer.Test
{
    public class AliasAttributeTest
    {
        [Fact]
        public void Test()
        {
            var val2 = RandomHelper.RandomValue<_Attribute_Class_1>();
            VerifyHelper.VerifyEntityWithMap2(val2);

            var buf = BssomSerializer.Serialize(val2);
            var helper = new BssomFieldMarshaller(buf);
            helper.IndexOf("[AAA]").IsExists.IsTrue();
            helper.IndexOf("[A1]").IsExists.IsFalse();

            //-------

            var val3 = RandomHelper.RandomValue<_Attribute_Class_2>();
            VerifyHelper.VerifyEntityWithMap2(val3);

            var buf2 = BssomSerializer.Serialize(val3);
            var helper2 = new BssomFieldMarshaller(buf2);
            helper2.IndexOf("[AAA]").IsExists.IsFalse();
            helper2.IndexOf("[A1]").IsExists.IsTrue();
        }

        public class _Attribute_Class_1
        {
            [Alias("AAA")]
            public int A1;
            public long B1;
        }

        public class _Attribute_Class_2
        {
            public int A1;
            public long B1;
        }

    }
}
