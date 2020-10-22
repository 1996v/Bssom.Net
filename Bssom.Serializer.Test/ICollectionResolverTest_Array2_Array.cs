using System;
using System.Collections;
using System.Collections.Generic;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolver;
using Xunit;

namespace Bssom.Serializer.Test
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
