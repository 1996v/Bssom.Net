using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolvers;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class BuildInResolverTest_EnumType
    {
        private enum Int8Enum : sbyte
        {
            A, B, C
        }
        private enum Int64Enum : long
        {
            A, B, C
        }

        [Fact]
        public void ResolverGetFormatter_EnumFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Int8Enum>().Is(EnumFormatter<Int8Enum>.Instance);
            BuildInResolver.Instance.GetFormatter<Int64Enum>().Is(EnumFormatter<Int64Enum>.Instance);
        }

        [Fact]
        public void EnumFormatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<Int8Enum>(Int8Enum.A);
            VerifyHelper.VerifySimpleType<Int8Enum>(Int8Enum.B);
            VerifyHelper.VerifySimpleType<Int64Enum>(Int64Enum.A);
            VerifyHelper.VerifySimpleType<Int64Enum>(Int64Enum.B);
        }
    }
}
