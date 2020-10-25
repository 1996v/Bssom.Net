using System;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolvers;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class ICollectionResolverTest_Array2_ArraySegment
    {
        [Fact]
        public void ResolverGetFormatter_StringArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<string>>().IsType<ArraySegmentFormatter<string>>();
        }

        [Fact]
        public void StringArraySegmentFormatter_FormatterTypeIsArray2TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<string>>();
            VerifyHelper.VerifyEntityWithArray2(val);
        }
    }
}
