using System;
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

    public class ExpandableBssomBufferTest
    {
        [Fact]
        public void InitializationData_IsNormal()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            buf.Position.Is(0);
            buf.Buffered.Is(0);
            buf.GetBufferedArray().Length.Is(0);
        }

        [Fact]
        public void Advance_OneBufferSpan_DataIsCorrectly()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);
            buf.Position.Is(4);
            buf.Buffered.Is(4);
            var buf2 = buf.GetBufferedArray();
            buf2.Length.Is(4);
            Unsafe.ReadUnaligned<int>(ref buf2[0]).Is(int.MaxValue);
        }

        [Fact]
        public void GetRefExpandCapacity_DataIsCorrectly()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MinValue);
            buf.Position.Is(4);
            buf.Buffered.Is(4);
            var buf2 = buf.GetBufferedArray();
            buf2.Length.Is(4);
            Unsafe.ReadUnaligned<int>(ref buf2[0]).Is(int.MaxValue);
        }

        [Fact]
        public void GetRefAndAdvance_TwoBufferSpan_DataIsCorrectly()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MinValue);
            buf.Advance(4);

            buf.Position.Is(8);
            buf.Buffered.Is(8);
            var buf2 = buf.GetBufferedArray();
            buf2.Length.Is(8);
            Unsafe.ReadUnaligned<int>(ref buf2[0]).Is(int.MaxValue);
            Unsafe.ReadUnaligned<int>(ref buf2[4]).Is(int.MinValue);
        }

        [Fact]
        public void OneBufferSpan_SeekBeforeBuffered_AndAdvance_IsNotIncreaseBuffered()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            buf.GetRef(4);
            buf.Advance(4);

            buf.Seek(2);
            buf.Buffered.Is(4);
            buf.Position.Is(2);
        }

        [Fact]
        public void SeekBufferedEnd_IsNotException()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            buf.Advance(4);
            buf.Seek(0);
            buf.Seek(4);
        }

        [Fact]
        public void GetRefAndAdvance_SeekBufferedEnd_IsNotException()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            buf.GetRef(8);
            buf.Advance(8);
            buf.Seek(0);
            buf.Seek(8);
        }

        [Fact]
        public void TwoBufferSpan_SeekBeforeBuffered_AndAdvance_IsNotIncreaseBuffered()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            buf.Advance(4);
            buf.GetRef(4);
            buf.Advance(4);

            buf.Seek(2);
            buf.Buffered.Is(8);
            buf.Position.Is(2);

            buf.Seek(6);
            buf.Buffered.Is(8);
            buf.Position.Is(6);
        }

        [Fact]
        public void GetRefAndAdvance_SeekBeginWithFirstBufSpan_AndUpdateData_DataIsCorrectly()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MinValue);
            buf.Advance(4);

            buf.Seek(2);
            Unsafe.WriteUnaligned(ref buf.GetRef(2), short.MaxValue);
            buf.Advance(2);

            buf.Seek(0);
            Unsafe.WriteUnaligned(ref buf.GetRef(2), short.MinValue);
            buf.Advance(2);

            buf.Position.Is(2);
            buf.Buffered.Is(8);
            var buf2 = buf.GetBufferedArray();
            buf2.Length.Is(8);
            Unsafe.ReadUnaligned<short>(ref buf2[0]).Is(short.MinValue);
            Unsafe.ReadUnaligned<short>(ref buf2[2]).Is(short.MaxValue);
            Unsafe.ReadUnaligned<int>(ref buf2[4]).Is(int.MinValue);
        }

        [Fact]
        public void GetRefAndAdvance_SeekBeginWithSecondBufSpan_AndUpdateData_DataIsCorrectly()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MinValue);
            buf.Advance(4);

            buf.Seek(6);
            Unsafe.WriteUnaligned(ref buf.GetRef(2), short.MaxValue);
            buf.Advance(2);

            buf.Seek(4);
            Unsafe.WriteUnaligned(ref buf.GetRef(2), short.MinValue);
            buf.Advance(2);

            buf.Position.Is(6);
            buf.Buffered.Is(8);
            var buf2 = buf.GetBufferedArray();
            buf2.Length.Is(8);
            Unsafe.ReadUnaligned<int>(ref buf2[0]).Is(int.MaxValue);
            Unsafe.ReadUnaligned<short>(ref buf2[4]).Is(short.MinValue);
            Unsafe.ReadUnaligned<short>(ref buf2[6]).Is(short.MaxValue);
        }

        [Fact]
        public void GetRefAndAdvance_SeekCurrentWithFirstBufSpan_AndUpdateData_DataIsCorrectly()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MinValue);
            buf.Advance(4);

            buf.Seek(-6, BssomSeekOrgin.Current);
            Unsafe.WriteUnaligned(ref buf.GetRef(2), short.MaxValue);
            buf.Advance(2);

            buf.Seek(-4, BssomSeekOrgin.Current);
            Unsafe.WriteUnaligned(ref buf.GetRef(2), short.MinValue);
            buf.Advance(2);

            buf.Position.Is(2);
            buf.Buffered.Is(8);
            var buf2 = buf.GetBufferedArray();
            buf2.Length.Is(8);
            Unsafe.ReadUnaligned<short>(ref buf2[0]).Is(short.MinValue);
            Unsafe.ReadUnaligned<short>(ref buf2[2]).Is(short.MaxValue);
            Unsafe.ReadUnaligned<int>(ref buf2[4]).Is(int.MinValue);
        }

        [Fact]
        public void GetRefAndAdvance_SeekCurrentWithSecondBufSpan_AndUpdateData_DataIsCorrectly()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MinValue);
            buf.Advance(4);

            buf.Seek(-2, BssomSeekOrgin.Current);
            Unsafe.WriteUnaligned(ref buf.GetRef(2), short.MaxValue);
            buf.Advance(2);

            buf.Seek(-4, BssomSeekOrgin.Current);
            Unsafe.WriteUnaligned(ref buf.GetRef(2), short.MinValue);
            buf.Advance(2);

            buf.Position.Is(6);
            buf.Buffered.Is(8);
            var buf2 = buf.GetBufferedArray();
            buf2.Length.Is(8);
            Unsafe.ReadUnaligned<int>(ref buf2[0]).Is(int.MaxValue);
            Unsafe.ReadUnaligned<short>(ref buf2[4]).Is(short.MinValue);
            Unsafe.ReadUnaligned<short>(ref buf2[6]).Is(short.MaxValue);
        }

        [Fact]
        public void GetBssomBuffer_RestoreBoundaryValue()
        {
            var buf = new ExpandableBufferWriter(new byte[6]);
            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MinValue);
            buf.Advance(4);

            var buf2 = buf.GetBssomBuffer();
            buf2.Position.Is(8);

            buf.Position.Is(8);
            buf.Buffered.Is(8);

            Unsafe.WriteUnaligned(ref buf.GetRef(4), int.MaxValue);
            buf.Advance(4);
            buf.Position.Is(12);
            buf.Buffered.Is(12);
        }
    }
}