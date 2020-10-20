using BssomSerializers.Formatters;
using BssomSerializers.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace BssomSerializers.Test
{
    public class ICollectionResolverTest_Array2_Array
    {
        [Fact]
        public void ResolverGetFormatter_StringArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<string[]>().IsType<OneDimensionalArrayFormatter<string>>();
        }

        [Fact]
        public void StringArrayFormatter_FormatterTypeIsArray2TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<string[]>();
            VerifyHelper.VerifyEntityWithArray2(val);
        }
    }
  

  
}
