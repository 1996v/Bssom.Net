using BssomSerializers.Internal;
using System;
using System.Runtime.CompilerServices;

namespace BssomSerializers.BssomBuffer
{
    internal sealed class SimpleBufferWriter : IBssomBufferWriter, IBssomBuffer
    {
        private byte[] buffer;
        private int start;
        private int len;
        private int position;
        private int buffered;

        internal SimpleBufferWriter(byte[] buffer) : this(buffer, 0, buffer.Length)
        {

        }

        public SimpleBufferWriter(byte[] buffer, int start, int len)
        {
            const string args = "buffer," + "start," + "len";
            if (buffer == null || checked((uint)start) >= (uint)buffer.Length ||
                checked((uint)len) > (uint)(buffer.Length - start))
                throw new ArgumentException(args);

            this.buffer = buffer;
            this.start = start;
            this.len = len;
            this.position = 0;
            this.buffered = 0;
        }

        public long Position => position;

        public long Length => len;

        public long Buffered => buffered;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            if (position == buffered)
                buffered += count;

            position += count;
        }

        public IBssomBuffer GetBssomBuffer()
        {
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte ReadRef(int sizeHint = 0)
        {
            if (position + sizeHint > len)
                return ref BssomSerializationOperationException.ReaderEndOfBufferException();
            return ref buffer[start + position];
        }

        public bool CanGetSizeRefForProvidePerformanceInTryWrite(int size)
        {
            if (position + size > len)
                return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetRef(int sizeHint)
        {
            if (position + sizeHint > len)
                return ref BssomSerializationOperationException.ReaderEndOfBufferException();
            return ref buffer[start + position];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Seek(long postion, BssomSeekOrgin orgin = BssomSeekOrgin.Begin)
        {
            SeekWithOutVerify(postion, orgin);
            if (position > len)
                BssomSerializationOperationException.ReaderEndOfBufferException();
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
            if (position + size > len)
                return ref BssomSerializationOperationException.ReaderEndOfBufferException();
            return ref ReadRef(size);
        }

        public void UnFixed()
        {
        }

       
    }

}


