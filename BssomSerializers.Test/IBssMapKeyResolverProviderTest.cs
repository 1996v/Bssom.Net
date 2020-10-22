using System;
using System.Runtime.CompilerServices;
using System.Text;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class IBssMapKeyResolverProviderTest
    {
        [Fact]
        public void MapTypeOnlySupportsSpecifiedKeyType()
        {
            IBssMapKeyResolver keyResolver;
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(sbyte), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(Int16), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(Int32), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(Int64), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(byte), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(UInt16), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(UInt32), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(UInt64), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(Char), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(Boolean), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(Single), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(Double), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(string), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(Guid), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(DateTime), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(Decimal), out keyResolver).IsTrue();
            BssMapKeyResolverProvider.StringBssMapKeyResolver.Is(BssMapStringKeyResolver.Insance);
        }

        private static ulong[] ConvertRaw(byte[] bin)
        {
            int ul = (int)Math.Ceiling((decimal)bin.Length / 8);
            ulong[] u = new ulong[ul];
            Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref u[0]), ref bin[0], (uint)bin.Length);
            return u;
        }

        private static ulong[] ConvertULong(byte[] bin)
        {
            int ul = (int)Math.Ceiling((decimal)bin.Length / 8);
            ulong[] u = new ulong[ul];
            Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref u[0]), ref bin[0], (uint)bin.Length);
            for (int i = 0; i < u.Length; i++)
            {
                u[i] = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.As<ulong, byte>(ref u[i]));
            }
            return u;
        }

        public static object[][] Raw64BytesISegmentStringTestData = new object[][]
        {
                new object[] { "bssom", ConvertRaw(UTF8Encoding.UTF8.GetBytes("bssom")) },
                new object[] { "12345678123",ConvertRaw(UTF8Encoding.UTF8.GetBytes("12345678123")) },
                new object[] { "1234567812345678",ConvertRaw(UTF8Encoding.UTF8.GetBytes("1234567812345678")) },
        };

        [Theory]
        [MemberData(nameof(Raw64BytesISegmentStringTestData))]
        public void Raw64BytesISegment_FunctionIsCorrectly(string val, ulong[] rawKeys)
        {
            Raw64BytesISegment rawSegment = BssMapStringKeyResolver.Insance.GetMap1KeySegment(val);
            rawSegment.Length.Is(rawKeys.Length);
            rawSegment.GetFirstElementReference(out bool isContiguousMemoryArea);
            isContiguousMemoryArea.IsTrue();
            for (int i = 0; i < rawKeys.Length; i++)
            {
                rawSegment[i].Is(rawKeys[i]);
            }
        }

        public static object[][] UInt64BytesISegmentStringTestData = new object[][]
         {
               new object[] { "bssom", ConvertULong(UTF8Encoding.UTF8.GetBytes("bssom")) },
               new object[] { "12345678123", ConvertULong(UTF8Encoding.UTF8.GetBytes("12345678123")) },
               new object[] { "1234567812345678", ConvertULong(UTF8Encoding.UTF8.GetBytes("1234567812345678")) },
         };

        [Theory]
        [MemberData(nameof(UInt64BytesISegmentStringTestData))]
        public void UInt64BytesISegment_FunctionIsCorrectly(string val, ulong[] ulongKeys)
        {
            UInt64BytesISegment rawSegment = BssMapStringKeyResolver.Insance.GetMap2KeySegment(val);
            rawSegment.Length.Is(ulongKeys.Length);
            rawSegment.GetFirstElementReference(out bool isContiguousMemoryArea);
            isContiguousMemoryArea.IsTrue();
            for (int i = 0; i < ulongKeys.Length; i++)
            {
                rawSegment[i].Is(ulongKeys[i]);
            }
        }
    }
}
