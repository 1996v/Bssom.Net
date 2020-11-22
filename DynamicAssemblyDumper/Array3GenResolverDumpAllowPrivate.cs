using Bssom.Serializer;
using Bssom.Serializer.Internal;
using Bssom.Serializer.Resolvers;
using Xunit;

namespace DynamicAssemblyDumper
{
    public class Array3GenResolverDumpAllowPrivate
    {
        public class _base
        {
            [Key(0)]
            internal int A { get; set; }
            [Key(1)]
            protected int B { get; set; }
            [Key(2)]
            private int C { get; set; }
        }

        public class _sub : _base
        {
            [Key(3)]
            public int D { get; set; }
            [Key(4)]
            public int E { get; set; }
        }

        [Fact]
        public void Test1()
        {
            Array3CodeGenResolverAllowPrivate.Instance.GetFormatter<_sub>();
            Array3CodeGenResolverAllowPrivate.Instance.Save();
            StackallocBlockProvider.AssemblySave();
        }
    }
}
