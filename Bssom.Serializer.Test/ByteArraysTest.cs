using System;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class ByteArraysTest
    {
        [Fact]
        public void ByteArrayLengthIsZero_FormatterIsCorrectly()
        {
            var val = new byte[0];
            VerifyHelper.IsSafeAction(() => BssomSerializer.Deserialize<byte[]>(BssomSerializer.Serialize(val)));
        }

        [Fact]
        public void ByteArrayLengthIsZero_DeserializeIsThrow()
        {
            var val = new byte[0];
            VerifyHelper.Throws<ArgumentException>(() => BssomSerializer.Deserialize<object>(val).IsNotNull());
        }

        [Fact]
        public void BytesSegmentLengthIsZero_FormatterIsCorrectly()
        {
            var val = new ArraySegment<byte>(new byte[0]);
            VerifyHelper.IsSafeAction(() => BssomSerializer.Deserialize<ArraySegment<byte>>(BssomSerializer.Serialize(val)));
        }
    }
}
