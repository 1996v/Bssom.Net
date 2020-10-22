//using System.Runtime.CompilerServices;

using System;
using System.Text;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Internal;

namespace Bssom.Serializer
{
    internal struct StringInputDataSource
    {
        private string _str;

        private int curIndex;
        private int aryIndexNumber;
        private int mapKeyIndex;
        private int mapKeyLength;
        private bool curIsMapKey;

        public StringInputDataSource(string str) : this()
        {
            if (str is null)
                throw new ArgumentNullException(nameof(str));

            if (str.Length < 1)
                throw BssomSerializationArgumentException.InputDataFormatterError();

            _str = str;
        }

        public bool MoveNext()
        {
            if (curIndex == _str.Length)
                return false;

            switch (_str[curIndex])
            {
                case '[':
                    {
                        curIsMapKey = true;
                        curIndex++;
                        mapKeyIndex = curIndex;
                        for (; curIndex < _str.Length; curIndex++)
                        {
                            if (_str[curIndex] == ']')
                            {
                                mapKeyLength = curIndex - mapKeyIndex;
                                if (mapKeyLength == 0)
                                    BssomSerializationArgumentException.BssomMapStringKeyIsEmpty(); 
                                curIndex++;
                                return true;
                            }
                        }
                        throw BssomSerializationArgumentException.InputDataFormatterError();
                    }
                case '$':
                    {
                        curIsMapKey = false;
                        curIndex++;
                        aryIndexNumber = ReadInt();

                        return true;
                    }
                default:
                    throw BssomSerializationArgumentException.InputDataFormatterError();
            }
        }

        private int ReadInt()
        {
            if (_str[curIndex] < '0' || _str[curIndex] > '9')
                throw BssomSerializationArgumentException.InputDataFormatterError();

            long num = 0;
            num += (_str[curIndex] - '0');
            curIndex++;

            while (_str.Length - curIndex - 1 >= 0)
            {
                char c = _str[curIndex];
                if (c < '0' || c > '9')
                {
                    break;
                }

                num *= 10;
                num += (c - '0');
                curIndex++;
            }

            return checked((int)num);
        }

        public unsafe Raw64BytesISegment GetCurrentSegmentFromMap1StringKey()
        {
            if (!curIsMapKey)
                throw BssomSerializationArgumentException.InputDataFormatterError();

            byte[] buffer = GetBuffer(UTF8Encoding.UTF8.GetMaxByteCount(mapKeyLength));
            fixed (char* pChar = _str) fixed (byte* pByte = &buffer[0])
            {
                int length = UTF8Encoding.UTF8.GetBytes(pChar + mapKeyIndex, mapKeyLength, pByte, buffer.Length);
                return new Raw64BytesISegment(buffer, length);
            }
        }

        public unsafe UInt64BytesISegment GetCurrentSegmentFromMap2StringKey()
        {
            if (!curIsMapKey)
                throw BssomSerializationArgumentException.InputDataFormatterError();

            byte[] buffer = GetBuffer(UTF8Encoding.UTF8.GetMaxByteCount(mapKeyLength));
            fixed (char* pChar = _str) fixed (byte* pByte = &buffer[0])
            {
                int length = UTF8Encoding.UTF8.GetBytes(pChar + mapKeyIndex, mapKeyLength, pByte, buffer.Length);
                return new UInt64BytesISegment(buffer, length);
            }
        }

        public int GetCurrentArrayIndex()
        {
            if (curIsMapKey)
                throw BssomSerializationArgumentException.InputDataFormatterError();

            return aryIndexNumber;
        }

        public string GetCurrentSegmentString()
        {
            if (curIsMapKey)
                return _str.Substring(mapKeyIndex, mapKeyLength);
            else
                return "$" + aryIndexNumber.ToString();
        }

        [ThreadStatic]
        private static byte[] StaticBuffer;
        private static byte[] GetBuffer(int len)
        {
            if (len > 256)
                return new byte[len];

            if (StaticBuffer == null)
                StaticBuffer = new byte[256];

            return StaticBuffer;
        }
    }

}
