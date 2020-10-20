using System;
using System.Runtime.CompilerServices;
using BssomSerializers.Internal;

namespace BssomSerializers.BssomBuffer
{
    internal sealed class BssomComplexBuffer : IBssomBuffer
    {
        internal struct BufferSpan
        {
            public byte[] Buffer;
            public int Boundary;

            public int BufferLength => Buffer.Length;

            public BufferSpan(byte[] buffer)
            {
                Buffer = buffer;
                Boundary = buffer.Length;
            }

            public ref byte ReadRef(int len, int position)
            {
                if (position + len > Boundary)
                    BssomSerializationOperationException.CapacityOfBufferIsInsufficient(len);

                return ref Buffer[position];
            }
        }

        private const int MinimumBufferSize = 65536;
        public BufferSpan[] Spans;
        public long[] SpansCumulativeBoundary;

        public BssomComplexBuffer(byte[] buffer)
        {
            Spans = new BufferSpan[] { new BufferSpan(buffer), };
            SpansCumulativeBoundary = new long[] { 0 };
        }

        public long Position { get; private set; } = 0;
        public long Boundary => SpansCumulativeBoundary[SpansCumulativeBoundary.Length - 1] + Spans[Spans.Length - 1].Boundary;
        public int CurrentSpanIndex { get; set; } = 0;
        public int CurrentSpanPosition { get; set; } = 0;
        public int CurrentSpanRemainingRelativePosition => CurrentSpan.Boundary - CurrentSpanPosition;
        public bool CurrentSpanIsLast => CurrentSpanIndex == Spans.Length - 1;
        public BufferSpan CurrentSpan => Spans[CurrentSpanIndex];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte ReadRef(int sizeHint) => ref CurrentSpan.ReadRef(sizeHint, CurrentSpanPosition);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SeekWithOutVerify(long postion, BssomSeekOrgin orgin) => Seek(postion, orgin, -1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Seek(long postion, BssomSeekOrgin orgin) => Seek(postion, orgin, Boundary);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Seek(long postion, BssomSeekOrgin orgin, long limit)
        {
            if (orgin == BssomSeekOrgin.Current)
                Position += postion;
            else
                Position = postion;

            if (limit != -1 && Position > limit)
                BssomSerializationOperationException.ReaderEndOfBufferException();

            if (Spans.Length == 1)
            {
                CurrentSpanPosition = (int)Position;
            }
            else
            {
                CurrentSpanIndex = Array.BinarySearch(SpansCumulativeBoundary, Position);
                if (CurrentSpanIndex <= -1)
                    CurrentSpanIndex = ~CurrentSpanIndex - 1;
                CurrentSpanPosition = (int)(Position - SpansCumulativeBoundary[CurrentSpanIndex]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SpansResize(int num)
        {
            Array.Resize(ref Spans, num);
            Array.Resize(ref SpansCumulativeBoundary, num);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveToNextSpan()
        {
            CurrentSpanIndex++;
            CurrentSpanPosition = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte TryReadFixedRef(int size, out bool haveEnoughSizeAndCanBeFixed)
        {
            if (CurrentSpanRemainingRelativePosition >= size)
            {
                haveEnoughSizeAndCanBeFixed = true;
                return ref CurrentSpan.ReadRef(0, CurrentSpanPosition);
            }
            haveEnoughSizeAndCanBeFixed = false;
            return ref ArrayEmpty<byte>.Empty[0];
        }

        public void UnFixed()
        {
        }
    }
}