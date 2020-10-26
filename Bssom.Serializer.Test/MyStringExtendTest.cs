using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Bssom.Serializer.Resolvers;
using Bssom.Serializer.BssomBuffer;
using Xunit;
using Xunit.Abstractions;
using Bssom.Serializer.Formatters;
using System;

namespace Bssom.Serializer.Test
{
    public sealed class MyStringFormatterResolver : IFormatterResolver
    {
        public static MyStringFormatterResolver Instance = new MyStringFormatterResolver();

        public IBssomFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IBssomFormatter<T> Formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(string))
                    Formatter = (IBssomFormatter<T>)(object)MyStringFormatter.Instance;
            }
        }
    }

    public sealed class MyStringFormatter : IBssomFormatter<string>
    {
        public static MyStringFormatter Instance = new MyStringFormatter();

        public unsafe string Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
            {
                return null;
            }

            reader.EnsureType(BssomType.StringCode);
            int dataLen = reader.ReadVariableNumber();
            ref byte refb = ref reader.BssomBuffer.ReadRef((int)dataLen);
            fixed (byte* pRefb = &refb)
            {
                return new string((sbyte*)pRefb, 0, (int)dataLen, UTF8Encoding.UTF8);
            }
        }

        public unsafe void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, string value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            int valueUtf8Size = context.ContextDataSlots.PopMyStringSize();

            writer.WriteBuildInType(BssomType.StringCode);
            writer.WriteVariableNumber(valueUtf8Size);

            ref byte refb = ref writer.BufferWriter.GetRef(valueUtf8Size);
            fixed (char* pValue = value)
            fixed (byte* pRefb = &refb)
            {
                UTF8Encoding.UTF8.GetBytes(pValue, value.Length, pRefb, valueUtf8Size);
            }
            writer.BufferWriter.Advance(valueUtf8Size);
        }

        public int Size(ref BssomSizeContext context, string value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            int dataSize = UTF8Encoding.UTF8.GetByteCount(value);
            context.ContextDataSlots.PushMyStringSize(dataSize);
            return BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.VariableNumberSize((ulong)dataSize) + dataSize;
        }
    }

    public static class MyStringFormatterContextExtend
    {
        public static void PushMyStringSize(this ContextDataSlots context, int len)
        {
            if (!context.TryGetNamedDataSlot("_mystring", out object stackVal))
            {
                stackVal = new Stack<int>();
                context.AllocateNamedDataSlot("_mystring", stackVal);
            }

            var stack = (Stack<int>)stackVal;
            stack.Push(len);
        }

        public static int PopMyStringSize(this ContextDataSlots context)
        {
            context.TryGetNamedDataSlot("_mystring", out object stackVal);
            var stack = (Stack<int>)stackVal;
            return stack.Pop();
        }

        public static void SetMyStringStack(this ContextDataSlots context, ContextDataSlots target)
        {
            context.TryGetNamedDataSlot("_mystring", out object stackVal);
            target.AllocateNamedDataSlot("_mystring", stackVal);
        }
    }

    public class MyStringFormatterTest
    {
        [Fact]
        public void MyTest()
        {
            var option = BssomSerializerOptions.Default.WithFormatterResolver(MyStringFormatterResolver.Instance);
            string str = RandomHelper.RandomValue<string>();
            BssomSizeContext sizeContext = new BssomSizeContext(option);
            int len = BssomSerializer.Size(ref sizeContext, str);
            if (len > 1000)
                throw new Exception("Size of value storage binary exceeded");

            BssomSerializeContext serContext = new BssomSerializeContext(option);
            sizeContext.ContextDataSlots.SetMyStringStack(serContext.ContextDataSlots);
            var bytes = BssomSerializer.Serialize(ref serContext, str);
            var deStr = BssomSerializer.Deserialize<string>(bytes);

            Assert.Equal(str,deStr);
        }

    }
}
