using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Bssom.Serializer.Internal;

namespace Bssom.Serializer.BssomBuffer
{
    internal sealed partial class BssomComplexBuffer : IBssomBuffer
    {
        private const int MinimumBufferSize = short.MaxValue;

        public BufferSpan[] Spans;
        public long[] SpansCumulativeBoundary;

        public BssomComplexBuffer(byte[] buffer, int start = 0)
        {
            Spans = new BufferSpan[] { new BufferSpan(buffer,start), };
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