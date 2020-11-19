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
using System.Reflection.Emit;
using System.Runtime.InteropServices;

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

        struct s
        {
            public uint num2;
            public uint num3;
            public uint num4;
            public uint num5;
            public uint num6;
            public uint num7;
        }
        [StructLayout(LayoutKind.Explicit, Size = 24)]
        struct hh { }
        public static void Serialize2(SpacingValuesClass value)
        {
            BssomSerializeContext context = new BssomSerializeContext(BssomSerializerOptions.IntKeyCompositedResolverOption, default);
            using (ExpandableBufferWriter buffer = ExpandableBufferWriter.CreateGlobar())
            {
                BssomWriter writer = new BssomWriter(buffer);
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }
                long num = writer.WriteArray3Header(6);
                uint num2 = 0, num3 = 0, num4 = 0, num5 = 0, num6 = 0, num7 = 0;

                num2 = (uint)(writer.Position - num);
                writer.Write(value.A);
                num3 = (uint)(writer.Position - num);
                writer.WriteNull();
                num4 = (uint)(writer.Position - num);
                writer.WriteNull();
                num5 = (uint)(writer.Position - num);
                writer.Write(value.B);
                num6 = (uint)(writer.Position - num);
                writer.WriteNull();
                num7 = (uint)(writer.Position - num);
                writer.Write(value.C);
              
                writer.WriteBackArray3Header(num, ref num2, 6);
            }
        }
        public static unsafe void Serialize(SpacingValuesClass value)
        {
            BssomSerializeContext context = new BssomSerializeContext(BssomSerializerOptions.IntKeyCompositedResolverOption, default);
            using (ExpandableBufferWriter buffer = ExpandableBufferWriter.CreateGlobar())
            {
                BssomWriter writer = new BssomWriter(buffer);
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }
                long num = writer.WriteArray3Header(6);
                hh h = new hh();
                Unsafe.Add(ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref h)), 0) = (uint)(writer.Position - num);
                writer.Write(value.A);
                Unsafe.Add(ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref h)), 1) = (uint)(writer.Position - num);
                writer.WriteNull();
                Unsafe.Add(ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref h)), 2) = (uint)(writer.Position - num);
                writer.WriteNull();
                Unsafe.Add(ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref h)), 3) = (uint)(writer.Position - num);
                writer.Write(value.B);
                Unsafe.Add(ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref h)), 4) = (uint)(writer.Position - num);
                writer.WriteNull();
                Unsafe.Add(ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref h)), 5) = (uint)(writer.Position - num);
                writer.Write(value.C);
               

                writer.WriteBackArray3Header(num, ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref h)), 6);

            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void ss(uint n1, uint n2, uint n3, uint n41, uint n11, uint n31)
        {
            if (n1 != n2 && n2 == n3 && n3 == n41 && n3 == n11 && n3 == n31)
            {
                throw new Exception();
            }
        }

        unsafe delegate byte* sss(int num);
        private static sss qwe()
        {

            DynamicMethod dynamicMethod = new DynamicMethod("Add", typeof(byte*), new Type[] { typeof(int) }, true);
            var iLGenerator = dynamicMethod.GetILGenerator();
            dynamicMethod.DefineParameter(0, System.Reflection.ParameterAttributes.In, "i");//Ldarg_0


            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Conv_U);
            iLGenerator.Emit(OpCodes.Localloc);
            //iLGenerator.Emit(OpCodes.Ret);
            return (sss)dynamicMethod.CreateDelegate(typeof(sss));
        }

        [Fact]
        public unsafe void SpacingValues_FormatterIsCorrectly()
        {
            //int po = 3;
            //hh h = new hh();
            //void* ww = Unsafe.AsPointer(ref po);
            //void* ww2 = Unsafe.AsPointer(ref h);

            //sss r1 = qwe();
            //byte* pp = r1(4);
            var val = RandomHelper.RandomValue<SpacingValuesClass>();
            //Serialize(val);

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
