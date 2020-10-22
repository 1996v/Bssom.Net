using BssomSerializers.Resolver;
using System;
using Xunit;

namespace DynamicAssemblyDumper
{
    public class MapCodeGenResolverDump
    {
        public class _Test1
        {
            public int A;
            public long B;
            public short C;
            public char D;
        }

        public class _Test2
        {
            public int PrimitiveField_1;
            public byte[] PrimitiveField_10;
        }

        [Fact]
        public void Test1()
        {
            MapCodeGenResolver.Instance.GetFormatter<_Test2>();
            MapCodeGenResolver.Instance.Save();
        }
    }
}
