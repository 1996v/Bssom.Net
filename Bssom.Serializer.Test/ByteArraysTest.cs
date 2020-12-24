using System;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class ByteArraysTest
    {
        [Fact]
        public void ByteArrayLengthIsZero_SerializeIsCorrectly()
        {
            var val = new byte[0];
            VerifyHelper.IsSafeAction(() => BssomSerializer.Serialize(val));
        }

        [Fact]
        public void ByteArrayLengthIsZero_DeserializeIsThrow()
        {
            var val = new byte[0];
            VerifyHelper.Throws<ArgumentException>(() => BssomSerializer.Deserialize<object>(val).IsNotNull());
        }

        [Fact]
        public void BytesSegmentLengthIsZero_SerializeIsCorrectly()
        {
            var val = new ArraySegment<byte>(new byte[0]);
            VerifyHelper.IsSafeAction(() => BssomSerializer.Serialize(val));
        }
    }
}
