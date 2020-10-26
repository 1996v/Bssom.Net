using System.IO;
using System.Threading.Tasks;

namespace Bssom.Serializer.BssomBuffer
{
    internal struct BufferSpan
    {
        public byte[] Buffer { get; }
        public int Start { get; }
        public int Boundary { get; private set; }

        public int MaxBoundary => Buffer.Length - Start;

        public BufferSpan(byte[] buffer) : this(buffer, 0)
        {
        }

        public BufferSpan(byte[] buffer, int start)
        {
            Buffer = buffer;
            Start = start;
            Boundary = buffer.Length - start;
        }

        public ref byte ReadRef(int len, int position)
        {
            if (position + len > Boundary)
            {
                BssomSerializationOperationException.CapacityOfBufferIsInsufficient(len);
            }

            return ref Buffer[Start + position];
        }

        public ref byte GetBufferStartRef()
        {
            return ref Buffer[Start];
        }

        public bool AnyLength(int length)
        {
            return MaxBoundary >= length;
        }

        public void BoundaryMaxExpand()
        {
            Boundary = MaxBoundary;
        }

        public void SetBoundary(int boundary)
        {
            Boundary = Start + boundary;
        }

        internal void WriteTo(Stream stream, int count)
        {
            stream.Write(Buffer, Start, count);
        }

        internal Task WriteToAsync(Stream stream, int count)
        {
            return stream.WriteAsync(Buffer, Start, count);
        }
    }
}