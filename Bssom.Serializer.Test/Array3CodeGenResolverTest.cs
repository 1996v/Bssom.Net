using Bssom.Serializer.BssMap;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Bssom.Serializer.Resolvers;
using Bssom.Serializer.Binary;
using Xunit;
namespace Bssom.Serializer.Test
{
    public class Array3CodeGenResolverTest
    {
        [Fact]
        public void ResolverGetFormatter_NonField_FormatterIsCorrectly()
        {
            Array3CodeGenResolver.Instance.GetFormatter<_nonField>().IsNotNull();
        }

        [Fact]
        public void ResolverGetFormatter_OneFieldInterface_FormatterIsCorrectly()
        {
            Array3CodeGenResolver.Instance.GetFormatter<_oneFieldInterface>().IsNotNull();
        }

        [Fact]
        public void IncompleteMarkKeyAttribute_IsSerializationTypeError()
        {
            VerifyHelper.Throws<TypeInitializationException>(() => Array3CodeGenResolver.Instance.GetFormatter<_IncompleteMarkKeyAttributeClass>());
        }

        [Fact]
        public void RepeatedValuesMarkKeyAttribute_IsSerializationTypeError()
        {
            VerifyHelper.Throws<TypeInitializationException>(() => Array3CodeGenResolver.Instance.GetFormatter<_RepeatedValueMarkKeyAttributeClass>());
        }

        [Fact]
        public void CustomClassType_FormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<_class1>();
            VerifyHelper.ConvertArray3ObjectAndVerifyEntity(val);
        }

        [Fact]
        public void CustomStructType_FormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<_struct1>();
            VerifyHelper.ConvertArray3ObjectAndVerifyEntity(val);
        }

        [Fact]
        public void NonField_FormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<_nonField>();
            VerifyHelper.ConvertArray3ObjectAndVerifyEntity(val);
        }

        [Fact]
        public void SpacingValues_FormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<SpacingValuesClass>();
            var ary3 = VerifyHelper.ConvertArray3ObjectAndVerifyEntity(val);

            ary3.Count.Is(6);
            ary3.GetObject(0).Equals(val.A).IsTrue();
            ary3.GetObject(1).Equals(BssomNull.Value).IsTrue();
            ary3.GetObject(2).Equals(BssomNull.Value).IsTrue();
            ary3.GetObject(3).Equals(val.B).IsTrue();
            ary3.GetObject(4).Equals(BssomNull.Value).IsTrue();
            ary3.GetObject(5).Equals(val.C).IsTrue();
        }

        [Fact]
        public void ExtensionTypeAllowPrivate_FormatterIsCorrectly()
        {
            var val = new _sub().Init();
            var ary3 = VerifyHelper.ConvertArray3ObjectAndVerifyEntity(val, BssomSerializerOptions.IntKeyCompositedAllowPrivateResolverOption);

            ary3.Count.Is(5);
            ary3.GetObject(0).Equals(val.A1).IsTrue();
            ary3.GetObject(1).Equals(val.B1).IsTrue();
            ary3.GetObject(2).Equals(val.C1).IsTrue();
            ary3.GetObject(3).Equals(val.D).IsTrue();
            ary3.GetObject(4).Equals(val.E).IsTrue();
        }

        public class _base
        {
            [Key(0)]
            internal int A { get; set; }
            [Key(1)]
            protected int B { get; set; }
            [Key(2)]
            private int C { get; set; }

            public int A1 => A;
            public int B1 => B;
            public int C1 => C;

            public void Init()
            {
                A = RandomHelper.RandomValue<int>();
                B = RandomHelper.RandomValue<int>();
                C = RandomHelper.RandomValue<int>();
            }
        }

        public class _sub : _base
        {
            [Key(3)]
            public int D { get; set; }
            [Key(4)]
            public int E { get; set; }

            public new _sub Init()
            {
                D = RandomHelper.RandomValue<int>();
                E = RandomHelper.RandomValue<int>();

                base.Init();

                return this;
            }
        }

        public class _nonField
        {
        }

        public interface _oneFieldInterface
        {
            [Key(0)]
            int A { get; set; }
        }

        public class SpacingValuesClass
        {
            [Key(0)]
            public int A { get; set; }
            [Key(3)]
            public int B { get; set; }
            [Key(5)]
            public int C { get; set; }
        }

        public class _class1
        {
            [Key(0)]
            public int A { get; set; }
            [Key(1)]
            public int B { get; set; }
        }

        public class _struct1
        {
            [Key(0)]
            public int A { get; set; }
            [Key(1)]
            public int B { get; set; }
        }

        public class _IncompleteMarkKeyAttributeClass
        {
            [Key(0)]
            public int A { get; set; }
            public int B { get; set; }
        }

        public class _RepeatedValueMarkKeyAttributeClass
        {
            [Key(0)]
            public int A { get; set; }
            [Key(0)]
            public int B { get; set; }
        }
    }
}
