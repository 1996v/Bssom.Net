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
            VerifyHelper.Throws<BssomSerializationTypeFormatterException>(() => Array3CodeGenResolver.Instance.GetFormatter<_IncompleteMarkKeyAttributeClass>());
        }

        [Fact]
        public void RepeatedValuesMarkKeyAttribute_IsSerializationTypeError()
        {
            VerifyHelper.Throws<BssomSerializationTypeFormatterException>(() => Array3CodeGenResolver.Instance.GetFormatter<_RepeatedValueMarkKeyAttributeClass>());
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
            VerifyHelper.ConvertArray3ObjectAndVerifyEntity(val);
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
