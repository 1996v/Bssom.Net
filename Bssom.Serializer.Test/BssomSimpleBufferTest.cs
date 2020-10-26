using System.Runtime.CompilerServices;
using Bssom.Serializer.BssomBuffer;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class BssomSimpleBufferTest
    {
        [Fact]
        public void InitializationData_IsNormal()
        {
            var buf = new SimpleBuffer(new byte[6], 2);
            buf.Position.Is(0);
        }

        [Fact]
        public void PositionIsCorrectly()
        {
            var buf = new SimpleBuffer(new byte[5], 2);
            buf.Seek(1);
            buf.Position.Is(1);

            buf.Seek(2);
            buf.Position.Is(2);

            buf.Seek(3);
            buf.Position.Is(3);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => buf.Seek(4), e => e.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.ReachedEndOfBuffer);
        }

        [Fact]
        public void ReadRefIsCorrectly()
        {
            var buf = new SimpleBuffer(new byte[5], 2);
            buf.ReadRef(3);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => buf.ReadRef(4), e => e.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.ReachedEndOfBuffer);
        }

        [Fact]
        public void TryReadFixedRefIsCorrectly()
        {
            var buf = new SimpleBuffer(new byte[5], 2);
            buf.TryReadFixedRef(3,out bool b1);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => buf.TryReadFixedRef(4, out bool b2), e => e.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.ReachedEndOfBuffer);
        }
    }

    public class BssomSimpleBufferWriterTest
    {
        [Fact]
        public void InitializationData_IsNormal()
        {
            var buf = new SimpleBufferWriter(new byte[6]);
            buf.Position.Is(0);
            buf.Buffered.Is(0);
        }

        [Fact]
        public void BufferedAndPositionIsCorrectly()
        {
            var buf = new SimpleBufferWriter(new byte[6], 2, 3);
            buf.Position.Is(0);
            buf.Buffered.Is(0);
            buf.Advance(1);
            buf.Position.Is(1);
            buf.Buffered.Is(1);
        }

        [Fact]
        public void SeekAndBufferedIsCorrectly()
        {
            var buf = new SimpleBufferWriter(new byte[60], 2, 10);
            buf.Seek(5);
            buf.Advance(1);
            buf.Position.Is(6);
            buf.Buffered.Is(0);
        }

        [Fact]
        public void SeekAnyPositionIsCorrectly()
        {
            var buf = new SimpleBufferWriter(new byte[60], 2, 10);
            buf.Buffered.Is(0);

            buf.Seek(1);
            buf.Position.Is(1);
            buf.Seek(5);
            buf.Position.Is(5);
        }

        [Fact]
        public void SeekBufferLengthIsNotException()
        {
            var buf = new SimpleBufferWriter(new byte[60], 2, 10);
            buf.Seek(10);
            buf.Position.Is(10);
        }

        [Fact]
        public void WriteDataIsCorrectly()
        {
            var buf = new SimpleBufferWriter(new byte[60], 2, 10);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MinValue);
            buf.Advance(4);
            buf.Position.Is(4);
            buf.Buffered.Is(4);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);
            buf.Position.Is(8);
            buf.Buffered.Is(8);

            buf.Seek(0);
            Unsafe.ReadUnaligned<int>(ref buf.ReadRef()).Is(int.MinValue);

            buf.Seek(4);
            Unsafe.ReadUnaligned<int>(ref buf.ReadRef()).Is(int.MaxValue);
        }
    }
}