using Bssom.Serializer;
using Bssom.Serializer.Internal;
using Bssom.Serializer.Resolvers;
using System;
using System.Runtime.InteropServices;
using Xunit;

namespace DynamicAssemblyDumper
{
    public class Array3GenResolverDump
    {
        public class _Uninterrupted
        {
            [Key(0)]
            public int A;
            [Key(1)]
            public long B;
            [Key(2)]
            public short C;
            [Key(3)]
            public char D;
        }

        public class _SpacingValuesClass
        {
            [Key(0)]
            public int A { get; set; }
            [Key(3)]
            public int B { get; set; }
            [Key(5)]
            public int C { get; set; }
        }

        public interface _NonField { }

        public class _class1
        {
            [Key(0)]
            public int A { get; set; }
            [Key(1)]
            public int B { get; set; }
        }

        [Fact]
        public void Test1()
        {
            Array3CodeGenResolver.Instance.GetFormatter<_class1>();
            Array3CodeGenResolver.Instance.GetFormatter<_Uninterrupted>();
            Array3CodeGenResolver.Instance.GetFormatter<_SpacingValuesClass>();
            Array3CodeGenResolver.Instance.GetFormatter<_NonField>();
            Array3CodeGenResolver.Instance.Save();
            StackallocBlockProvider.AssemblySave();
        }

     
    }
 
}
