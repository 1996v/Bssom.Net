using BssomSerializers.Formatters;
using BssomSerializers.Resolver;
using System;
using Xunit;

namespace BssomSerializers.Test
{
    public class ICollectionResolverTest_Array2_MultipleDimensionalArray
    {
        [Fact]
        public void ResolverGetFormatter_TwoDimensionalArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<int[,]>().IsType<TwoDimensionalArrayFormatter<int>>();
        }

        [Fact]
        public void ResolverGetFormatter_ThreeDimensionalArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<int[,,]>().IsType<ThreeDimensionalArrayFormatter<int>>();
        }

        [Fact]
        public void ResolverGetFormatter_FourDimensionalArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<int[,,,]>().IsType<FourDimensionalArrayFormatter<int>>();
        }

        [Fact]
        public void ResolverGetFormatter_FiveDimensionalArray_IsNotSupported()
        {
            VerifyHelper.Throws<Exception>(() => ICollectionResolver.Instance.GetFormatter<int[,,,,]>(), e => true);
        }

        [Fact]
        public void TwoDimensionalArrayFormatter_FormatterTypeIsArray2TypeAndFormatterIsCorrectly()
        {
            int[,] val = new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            VerifyHelper.VerifyEntityWithArray2(val);
        }

        [Fact]
        public void ThreeDimensionalArrayFormatter_FormatterTypeIsArray2TypeAndFormatterIsCorrectly()
        {
            int[,,] val = new int[,,] { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };
            VerifyHelper.VerifyEntityWithArray2(val);
        }

        [Fact]
        public void FourDimensionalArrayFormatter_FormatterTypeIsArray2TypeAndFormatterIsCorrectly()
        {
            int[,,,] val = new int[,,,] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 9 } } } };
            VerifyHelper.VerifyEntityWithArray2(val);
        }
    }
}
