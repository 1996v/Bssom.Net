using Bssom.Serializer.Resolvers;
using System;
using System.Reflection;
using Xunit;

namespace DynamicAssemblyDumper
{
    class xx
    {
        public static object[] r;
        public int f;

        public int f2 { get; set; }
    }
    class ff
    {
        public void T(MapCodeGenResolverDump xz, int xz2)
        {
            ((Action<MapCodeGenResolverDump, int>)xx.r[1]).Invoke(xz, xz2);
        }

        public void T2(int xz2)
        {
            var r = new xx();
            r.f = xz2;
        }

        public void T3(int xz2)
        {
            var r = new xx();
            r.f2 = xz2;
        }

    }
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
