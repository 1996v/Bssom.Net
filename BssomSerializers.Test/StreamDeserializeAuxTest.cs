using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class StreamDeserializeAuxTest
    {
        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Int64))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(UInt16))]
        [InlineData(typeof(UInt32))]
        [InlineData(typeof(UInt64))]
        [InlineData(typeof(Char))]
        [InlineData(typeof(Boolean))]
        [InlineData(typeof(Single))]
        [InlineData(typeof(Double))]
        [InlineData(typeof(string))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(Decimal))]

        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(Dictionary<Int32, List<object>>))]
        [InlineData(typeof(Dictionary<Int64, List<object>>))]
        [InlineData(typeof(Dictionary<byte, List<object>>))]
        [InlineData(typeof(Dictionary<UInt16, List<object>>))]
        [InlineData(typeof(Dictionary<UInt32, List<object>>))]
        [InlineData(typeof(Dictionary<UInt64, List<object>>))]
        [InlineData(typeof(Dictionary<Char, List<object>>))]
        [InlineData(typeof(Dictionary<Boolean, List<object>>))]
        [InlineData(typeof(Dictionary<Single, List<object>>))]
        [InlineData(typeof(Dictionary<Double, List<object>>))]
        [InlineData(typeof(Dictionary<string, List<object>>))]
        [InlineData(typeof(Dictionary<Guid, List<object>>))]
        [InlineData(typeof(Dictionary<DateTime, List<object>>))]
        [InlineData(typeof(Dictionary<Decimal, List<object>>))]

        [InlineData(typeof(List<byte>))]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(List<DateTime>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]

        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(object[]))]

        [InlineData(typeof(ArraySegment<byte>))]
        [InlineData(typeof(ArraySegment<decimal>))]
        [InlineData(typeof(ArraySegment<ulong>))]
        [InlineData(typeof(ArraySegment<DateTime>))]
        [InlineData(typeof(ArraySegment<string>))]
        [InlineData(typeof(ArraySegment<object>))]

        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void ReadOneObjectBytesFromStreamIsCorrectly(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            var buf = BssomSerializer.Serialize(obj);
            var stream = new MemoryStream(buf);

            var sda = new StreamDeserializeAux(stream);
            var bssomBuf = sda.GetBssomBuffer();
            ref byte refb = ref bssomBuf.ReadRef(buf.Length);

            SequenceEqual(buf,ref refb,buf.Length);
        
            stream.Seek(0, SeekOrigin.Begin);
            var sda2 = new StreamDeserializeAux(stream);
            ref byte refb2 = ref sda.GetBssomBufferAsync().Result.ReadRef(buf.Length);
            SequenceEqual(buf, ref refb2, buf.Length);
        }

        private void SequenceEqual(byte[] buf, ref byte refb, int len)
        {
#if NETFRAMEWORK
            for (int i = 0; i < len; i++)
            {
                buf[i].Is(Unsafe.Add(ref refb, i));
            }
#else
            MemoryExtensions.SequenceEqual(new ReadOnlySpan<byte>(buf), MemoryMarshal.CreateSpan(ref refb, len)).Is(true);
#endif
        }
    }
}