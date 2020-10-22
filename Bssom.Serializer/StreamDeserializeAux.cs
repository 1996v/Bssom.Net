using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssomBuffer;

namespace Bssom.Serializer
{
    internal class StreamDeserializeAux : IDisposable
    {
        private Stream stream;
        private byte[] buffer;
        private int position;
        private bool bufferHasExpanded;

        public StreamDeserializeAux(Stream stream)
        {
            this.stream = stream;
            this.buffer = BssomSerializerIBssomBufferWriterBufferCache.GetUnsafeBssomArrayCache();
            this.position = 0;
            this.bufferHasExpanded = false;
        }

        public IBssomBuffer GetBssomBuffer()
        {
            FillSizeOfOneObjectToBuffer();
            return new SimpleBufferWriter(buffer, 0, position);
        }

        public async Task<IBssomBuffer> GetBssomBufferAsync()
        {
            await FillSizeOfOneObjectToBufferAsync();
            return new SimpleBufferWriter(buffer, 0, position);
        }

        private int GetSizeOfOneObjectToBuffer()
        {
            int size;
            var objType = GetBssomTypeFromStreamPack.Get(stream, buffer);
            buffer[0] = objType;
            position++;

            if (!BssomBinaryPrimitives.TryGetPrimitiveTypeSizeFromStaticTypeSizes(objType,
                out size))
            {
                switch (objType)
                {
                    case BssomType.StringCode:
                        size = (int)ReadVariableNumberCore();
                        break;
                    case BssomType.NativeCode:
                        {
                            ReadStreamToBuffer(1);
                            objType = buffer[position - 1];
                            if (objType == NativeBssomType.CharCode)
                                size = BssomBinaryPrimitives.CharSize;
                            else if (objType == NativeBssomType.DateTimeCode)
                                size = BssomBinaryPrimitives.NativeDateTimeSize;
                            else if (objType == NativeBssomType.DecimalCode)
                                size = BssomBinaryPrimitives.DecimalSize;
                            else if (objType == NativeBssomType.GuidCode)
                                size = BssomBinaryPrimitives.GuidSize;
                            else
                                throw BssomSerializationOperationException.UnexpectedCodeRead(objType);
                            break;
                        }
                    case BssomType.Map1:
                    case BssomType.Map2:
                    case BssomType.Array2:
                        size = (int)ReadVariableNumberCore();
                        break;

                    case BssomType.Array1:
                        {
                            ReadStreamToBuffer(1);
                            if (buffer[position - 1] /*elementType*/ == BssomType.NativeCode)
                                ReadStreamToBuffer(1);
                            size = (int)ReadVariableNumberCore();
                            break;
                        }

                    default:
                        throw BssomSerializationOperationException.UnexpectedCodeRead(objType);
                }
            }

            return size;
        }

        private void FillSizeOfOneObjectToBuffer()
        {
            ReadStreamToBuffer(GetSizeOfOneObjectToBuffer());
        }

        private async Task FillSizeOfOneObjectToBufferAsync()
        {
            await ReadStreamToBufferAsync(GetSizeOfOneObjectToBuffer());
        }

        private void BufferResize(int len)
        {
            var newBuf = ArrayPool<byte>.Shared.Rent(position + len);
            Array.Copy(buffer, 0, newBuf, 0, position);
            if (bufferHasExpanded)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
            else
            {
                bufferHasExpanded = true;
            }
            buffer = newBuf;
        }

        private void ReadStreamToBuffer(int len)
        {
            if (buffer.Length < position + len)
                BufferResize(position + len);

            int readNum = stream.Read(buffer, position, len);
            if (readNum != len)
                throw new Exception();//格式错误
            position += len;
        }

        private async Task ReadStreamToBufferAsync(int len)
        {
            if (buffer.Length < position + len)
                BufferResize(position + len);

            int readNum = await stream.ReadAsync(buffer, position, len);
            if (readNum != len)
                throw new Exception();//格式错误
            position += len;
        }

        private ulong ReadVariableNumberCore()
        {
            ReadStreamToBuffer(1);
            byte code = buffer[position - 1];
            ulong val;
            switch (code)
            {
                case BssomBinaryPrimitives.VariableUInt9:
                    {
                        ReadStreamToBuffer(1);
                        val = (ulong)(BssomBinaryPrimitives.VariableUInt8Max + buffer[position - 1]);
                        break;
                    }

                case BssomBinaryPrimitives.FixUInt8:
                    {
                        ReadStreamToBuffer(1);
                        val = buffer[position - 1];
                        break;
                    }
                case BssomBinaryPrimitives.FixUInt16:
                    {
                        ReadStreamToBuffer(2);
                        val = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref buffer[position - 2]);
                        break;
                    }

                case BssomBinaryPrimitives.FixUInt32:
                    {
                        ReadStreamToBuffer(4);
                        val = BssomBinaryPrimitives.ReadUInt32LittleEndian(ref buffer[position - 4]);
                        break;
                    }

                case BssomBinaryPrimitives.FixUInt64:
                    {
                        ReadStreamToBuffer(8);
                        val = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref buffer[position - 8]);
                        break;
                    }
                default:
                    return code;
            }
            return val;
        }

        public void Dispose()
        {
            if (bufferHasExpanded)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        internal static class GetBssomTypeFromStreamPack
        {
            public static byte Get(Stream stream, byte[] buffer)
            {
                byte one = ReadStreamByte(stream);

                if (one <= BssomType.MaxBlankCodeValue)
                {
                    if (one <= BssomType.MaxVarBlankCodeValue)
                    {
                        AdvanceStream(stream, buffer, one);
                    }
                    else
                    {
                        int len;
                        if (one == BssomType.BlankInt16Code)
                        {
                            stream.Read(buffer, 0, buffer[2]);
                            len = BssomBinaryPrimitives.ReadInt16LittleEndian(ref buffer[0]);
                        }
                        else /*if (one == BssomType.EmptyInt32Code)*/
                        {
                            stream.Read(buffer, 0, buffer[4]);
                            len = BssomBinaryPrimitives.ReadInt32LittleEndian(ref buffer[0]);
                        }
                        AdvanceStream(stream, buffer, len);
                    }

                    one = ReadStreamByte(stream);
                }

                return one;
            }

            private static byte ReadStreamByte(Stream stream)
            {
                int one = stream.ReadByte();
                if (one == -1)
                    throw new Exception();//end
                return (byte)one;
            }
            private static void AdvanceStream(Stream stream, byte[] buffer, int size)
            {
                if (stream.CanSeek)
                    stream.Seek(size, SeekOrigin.Current);
                else
                {
                    while (size > 0)
                    {
                        if (buffer.Length > size)
                        {
                            StreamSkip(stream, buffer, size);
                            size = 0;
                        }
                        else
                        {
                            StreamSkip(stream, buffer, buffer.Length);
                            size -= buffer.Length;
                        }
                    }
                }
            }
            private static void StreamSkip(Stream stream, byte[] buffer, int size)
            {
                int readNum = stream.Read(buffer, 0, size);
                if (readNum != size)
                    throw new Exception();//格式错误
            }
        }
    }
}