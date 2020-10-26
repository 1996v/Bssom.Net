using System;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer.BssomBuffer
{
    internal class SimpleBuffer : IBssomBuffer
    {
        protected byte[] buffer;
        protected int start;
        protected int position;

        public long Position => position;

        public int Length => buffer.Length - start;

        internal SimpleBuffer(byte[] buffer) : this(buffer, 0)
        {
        }

        internal SimpleBuffer(byte[] buffer, int start)
        {
            this.buffer = buffer;
            this.start = start;
            position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte ReadRef(int sizeHint = 0)
        {
            if (position + sizeHint > Length)
            {
                return ref BssomSerializationOperationException.ReaderEndOfBufferException();
            }

            return ref buffer[start + position];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Seek(long postion, BssomSeekOrgin orgin = BssomSeekOrgin.Begin)
        {
            SeekWithOutVerify(postion, orgin);
            if (position > Length)
            {
                BssomSerializationOperationException.ReaderEndOfBufferException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SeekWithOutVerify(long postion, BssomSeekOrgin orgin)
        {
            if (orgin == BssomSeekOrgin.Current)
            {
                position += (int)postion;
            }
            else
            {
                position = (int)postion;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte TryReadFixedRef(int size, out bool haveEnoughSizeAndCanBeFixed)
        {
            haveEnoughSizeAndCanBeFixed = true;
            if (position + size > Length)
            {
                return ref BssomSerializationOperationException.ReaderEndOfBufferException();
            }

            return ref ReadRef(size);
        }

        public void UnFixed()
        {
        }
    }

    internal sealed class SimpleBufferWriter : SimpleBuffer, IBssomBufferWriter
    {
        private int len;
        private int buffered;

        internal SimpleBufferWriter(byte[] buffer) : this(buffer, 0, buffer.Length)
        {

        }

        public SimpleBufferWriter(byte[] buffer, int start, int len) : base(buffer, start)
        {
            const string args = "buffer," + "start," + "len";
            if (buffer == null || checked((uint)start) >= (uint)buffer.Length ||
                checked((uint)len) > (uint)(buffer.Length - start))
            {
                throw new ArgumentException(args);
            }

            this.len = len;
            buffered = 0;
        }

        public new long Length => len;

        public long Buffered => buffered;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            if (position == buffered)
            {
                buffered += count;
            }

            position += count;
        }

        public IBssomBuffer GetBssomBuffer()
        {
            return this;
        }

        public bool CanGetSizeRefForProvidePerformanceInTryWrite(int size)
        {
            if (position + size > len)
            {
                return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetRef(int sizeHint)
        {
            if (position + sizeHint > len)
            {
                return ref BssomSerializationOperationException.ReaderEndOfBufferException();
            }

            return ref buffer[start + position];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void Seek(long postion, BssomSeekOrgin orgin = BssomSeekOrgin.Begin)
        {
            SeekWithOutVerify(postion, orgin);
            if (position > len)
            {
                BssomSerializationOperationException.ReaderEndOfBufferException();
            }
        }

    }

}


