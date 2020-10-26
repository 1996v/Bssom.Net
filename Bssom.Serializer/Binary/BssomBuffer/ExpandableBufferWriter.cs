using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Bssom.Serializer.BssomBuffer
{
    internal sealed class ExpandableBufferWriter : IBssomBufferWriter, IDisposable
    {
        private const int MinimumBufferSize = short.MaxValue;

        private BssomComplexBuffer complexBuffer;
        private int[] bufferedsRelativeSpan;

        public ExpandableBufferWriter(byte[] bufData, int start = 0) : this(new BssomComplexBuffer(bufData, start))
        {
        }

        public ExpandableBufferWriter(BssomComplexBuffer complexBuffer)
        {
            this.complexBuffer = complexBuffer;
            bufferedsRelativeSpan = new int[complexBuffer.Spans.Length];
        }

        /// <summary>
        /// <see cref="complexBuffer"/> position
        /// </summary>
        public long Position => complexBuffer.Position;
        /// <summary>
        /// Records all the buffered in simpleBuffer. When SpanBuffered in simpleBuffer is changed, this parameter will be changed at the same time
        /// </summary>
        public long Buffered { get; private set; } = 0;
        /// <summary>
        /// BufferSpans count
        /// </summary>
        public int BufferSpanCount => complexBuffer.Spans.Length;
        /// <summary>
        /// Records the Buffered of the CurrentSpan in simpleBuffer, this value is only used internally
        /// </summary>
        private int CurrentSpanBuffered => bufferedsRelativeSpan[bufferedsRelativeSpan.Length - 1];

        /// <summary>
        /// flush the boundary of the last Span in the <see cref="complexBuffer"/>, and then return 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IBssomBuffer GetBssomBuffer()
        {
            FlushLastSpanBoundary();
            return complexBuffer;
        }

        /// <summary>
        /// Only when the current Span is position and buffered in <see cref="complexBuffer"/> are the same, advance forward
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            if (complexBuffer.CurrentSpanPosition == CurrentSpanBuffered)
            {
                Buffered += count;
                bufferedsRelativeSpan[complexBuffer.CurrentSpanIndex] += count;
            }

            complexBuffer.SeekWithOutVerify(count, BssomSeekOrgin.Current);
        }

        /// <summary>
        /// There are no restrictions, so call the <see cref="complexBuffer"/> is method directly
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SeekWithOutVerify(long postion, BssomSeekOrgin orgin)
        {
            complexBuffer.SeekWithOutVerify(postion, orgin);
        }

        /// <summary>
        /// Seeking in the writer will be constrained by <see cref="Buffered"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Seek(long position, BssomSeekOrgin orgin = BssomSeekOrgin.Begin)
        {
            complexBuffer.Seek(position, orgin, Buffered);
        }

        /// <summary>
        /// When the capacity of <see cref="complexBuffer"/> is not enough to write size, a new span will be generated
        /// If the span found is not the last one and the size has exceeded the boundary, so in order to keep the upper-level logical behavior consistent, an exception will be thrown
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetRef(int sizeHint = 0)
        {
            if (complexBuffer.CurrentSpanPosition + sizeHint > complexBuffer.CurrentSpan.Boundary)
            {
                MoveNextSpan(sizeHint);
            }

            return ref complexBuffer.ReadRef(sizeHint);
        }

        /// <summary>
        /// If <paramref name="size"/> exceeds the writer boundary, false is returned; otherwise, true forever
        /// </summary>
        public bool CanGetSizeRefForProvidePerformanceInTryWrite(int size)
        {
            if (complexBuffer.CurrentSpanPosition + size > complexBuffer.CurrentSpan.Boundary)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Traverse each span in <see cref="complexBuffer"/>, combine the Buffered parts of each span and return
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBufferedArray()
        {
            byte[] array;
            if (complexBuffer.Spans.Length == 1)
            {
                array = new byte[Buffered];
                if (array.Length != 0)
                {
                    Unsafe.CopyBlock(ref array[0], ref complexBuffer.CurrentSpan.GetBufferStartRef(), (uint)Buffered);
                }
            }
            else
            {
                array = GetBufferedArrayCore();
            }
            return array;
        }

        /// <summary>
        /// If there is only one buffer, return it directly; otherwise, the <see cref="BufferSpan.Start"/> front portion of the first buffer is kept, and only the buffered portion is taken for all subsequent buffers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBufferedArrayWithKeepFirstBuffer()
        {
            if (complexBuffer.Spans.Length == 1)
            {
                return complexBuffer.CurrentSpan.Buffer;
            }

            FlushLastSpanBoundary();
            byte[] array = new byte[complexBuffer.Spans[0].Start + Buffered];
            //save first buffer start front port
            int copyLen = complexBuffer.Spans[0].Start + bufferedsRelativeSpan[0];
            Array.Copy(complexBuffer.Spans[0].Buffer, array, copyLen);

            int start = copyLen;
            for (int i = 1; i < complexBuffer.Spans.Length; i++)
            {
                if (bufferedsRelativeSpan[i] != 0)
                {
                    Unsafe.CopyBlock(ref array[start], ref complexBuffer.Spans[i].GetBufferStartRef(), (uint)bufferedsRelativeSpan[i]);
                    start += bufferedsRelativeSpan[i];
                }
            }
            return array;
        }

        private byte[] GetBufferedArrayCore()
        {
            FlushLastSpanBoundary();
            byte[] array = new byte[Buffered];
            int start = 0;
            for (int i = 0; i < complexBuffer.Spans.Length; i++)
            {
                if (bufferedsRelativeSpan[i] != 0)
                {
                    Unsafe.CopyBlock(ref array[start], ref complexBuffer.Spans[i].GetBufferStartRef(), (uint)bufferedsRelativeSpan[i]);
                    start += bufferedsRelativeSpan[i];
                }
            }
            return array;
        }

        public void CopyTo(Stream stream, CancellationToken CancellationToken)
        {
            if (Buffered > 0)
            {
                FlushLastSpanBoundary();
                for (int i = 0; i < complexBuffer.Spans.Length; i++)
                {
                    if (bufferedsRelativeSpan[i] != 0)
                    {
                        CancellationToken.ThrowIfCancellationRequested();
                        complexBuffer.Spans[i].WriteTo(stream, bufferedsRelativeSpan[i]);
                    }
                }
            }
        }

        public async Task CopyToAsync(Stream stream, CancellationToken CancellationToken)
        {
            if (Buffered > 0)
            {
                FlushLastSpanBoundary();
                for (int i = 0; i < complexBuffer.Spans.Length; i++)
                {
                    if (bufferedsRelativeSpan[i] != 0)
                    {
                        CancellationToken.ThrowIfCancellationRequested();
                        await complexBuffer.Spans[i].WriteToAsync(stream, bufferedsRelativeSpan[i]).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Look for spans that satisfy that capacity, and if the current span is not the last, then logically go back to <see cref="GetRef"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MoveNextSpan(int size)
        {
            if (complexBuffer.CurrentSpanIsLast)
            {
                if (complexBuffer.CurrentSpan.AnyLength(complexBuffer.CurrentSpanPosition + size))
                {
                    //Only when GetBuffer is called can the branch be entered,so need to restore the logic of refreshing Boundary to Buffered in GetBuffer, and re-assign BufferLength to Boundary
                    complexBuffer.Spans[complexBuffer.CurrentSpanIndex].BoundaryMaxExpand();
                    return;
                }
                //Add
                FlushLastSpanBoundary();
                CreateBufferSpan(size * 2);
            }

            complexBuffer.MoveToNextSpan();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateBufferSpan(int size)
        {
            if (size < MinimumBufferSize)
            {
                size = MinimumBufferSize;
            }

            BufferSpan span = new BufferSpan(ArrayPool<byte>.Shared.Rent(size));

            SpansResize(complexBuffer.Spans.Length + 1);

            complexBuffer.Spans[complexBuffer.Spans.Length - 1] = span;
            complexBuffer.SpansCumulativeBoundary[complexBuffer.SpansCumulativeBoundary.Length - 1] = complexBuffer.SpansCumulativeBoundary[complexBuffer.SpansCumulativeBoundary.Length - 2] + complexBuffer.Spans[complexBuffer.Spans.Length - 2].Boundary;
        }

        private void SpansResize(int capacity)
        {
            complexBuffer.SpansResize(capacity);
            Array.Resize(ref complexBuffer.SpansCumulativeBoundary, capacity);
            Array.Resize(ref bufferedsRelativeSpan, capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FlushLastSpanBoundary()
        {
            complexBuffer.Spans[complexBuffer.Spans.Length - 1].SetBoundary(bufferedsRelativeSpan[bufferedsRelativeSpan.Length - 1]);
        }

        public static ExpandableBufferWriter CreateTemporary()
        {
            return new ExpandableBufferWriter(new BssomComplexBuffer(ArrayPool<byte>.Shared.Rent(1024)));
        }

        public static ExpandableBufferWriter CreateGlobar()
        {
            return new ExpandableBufferWriter(new BssomComplexBuffer(BssomSerializerIBssomBufferWriterBufferCache.GetUnsafeBssomArrayCache()));
        }

        public void Dispose()
        {
            //first is ThreadStatic or User injected
            if (complexBuffer.Spans.Length > 1)
            {
                for (int i = 1; i < complexBuffer.Spans.Length; i++)
                {
                    ArrayPool<byte>.Shared.Return(complexBuffer.Spans[i].Buffer);
                }
            }
        }


    }
}