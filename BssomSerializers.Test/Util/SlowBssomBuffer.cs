using System.Runtime.CompilerServices;
namespace BssomSerializers.Test
{
    public class SlowBssomBuffer : IBssomBuffer
    {
        private byte[] _buffer;
        private int _offset;
        private int _buffered;

        public SlowBssomBuffer(byte[] buffer)
        {
            _buffer = buffer;
        }

        public long Position => _offset;
        public long Length => _buffer.Length;
        public long Buffered => _buffered;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            if (_offset == _buffered)
            {
                _buffered += count;
                _offset = _buffered;
            }
            else
            {
                _offset += count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte ReadRef(int sizeHint = 0)
        {
            if (_offset + sizeHint > _buffer.Length)
                return ref BssomSerializationOperationException.ReaderEndOfBufferException();
            return ref _buffer[_offset];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetRefWithDilatation(int sizeHint)
        {
            if (_offset + sizeHint > _buffer.Length)
                return ref BssomSerializationOperationException.ReaderEndOfBufferException();
            return ref _buffer[_offset];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Seek(long postion, BssomSeekOrgin orgin)
        {
            SeekWithOutVerify(postion, orgin);
            if (_offset > _buffer.Length)
                BssomSerializationOperationException.ReaderEndOfBufferException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SeekWithOutVerify(long postion, BssomSeekOrgin orgin)
        {
            if (orgin == BssomSeekOrgin.Current)
            {
                _offset += (int)postion;
            }
            else
            {
                _offset = (int)postion;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte TryReadFixedRef(int size, out bool haveEnoughSizeAndCanBeFixed)
        {
            haveEnoughSizeAndCanBeFixed = false;
            return ref _buffer[0];
        }

        public void UnFixed()
        {
        }

    }
}
