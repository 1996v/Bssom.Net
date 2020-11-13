using Bssom.Serializer.Resolvers;
using System;
using Xunit;

namespace DynamicAssemblyDumper
{
    public class MapCodeGenResolverAllowPrivateDump
    {
        public class _PrivateTest1
        {
            private int A { get; set; }

            private int B;

            public _PrivateTest1 Init()
            {
                Random ran = new Random();

                A = ran.Next(0,int.MaxValue);
                B = ran.Next(0, int.MaxValue);
                return this;
            }
        }

        public class _PrivateMembersClass
        {
            private int A1;
            private int B1 { get; set; }
            public int C1 { get; set; }

            public _PrivateMembersClass Init()
            {
                return this;
            }

            public override bool Equals(object obj)
            {
                if (obj is _PrivateMembersClass p)
                {
                    return A1 == p.A1 && B1 == p.B1 && C1 == p.C1;
                }

                return false;
            }
        }

        [Fact]
        public void Test1()
        {
            MapCodeGenResolverAllowPrivate.Instance.GetFormatter<_PrivateMembersClass>();
            MapCodeGenResolverAllowPrivate.Instance.Save();
        }
    }
}
