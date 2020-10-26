//using System.Runtime.CompilerServices;

using Bssom.Serializer.Binary;
using Bssom.Serializer.Internal;
using System.Collections.Generic;

namespace Bssom.Serializer.BssMap
{

    internal ref struct MapRouteSegmentWriter
    {
        private BssomWriter writer;
        private long _basePosition;

        public MapRouteSegmentWriter(BssomWriter writer, long basePosition)
        {
            this.writer = writer;
            _basePosition = basePosition;
        }

        public void WriteRouteData<TValue>(BssMapObjMarshal<TValue>.Entry[] entries, int index, int length, Queue<KeyValuePair<int, TValue>> valueMapOffsets)
        {
            if (length < 4)
            {
                // write equalNext/equalLast
                int nextNextBranchPos = -1;
                int len = index + length;

                for (int i = index; i < len; i++)
                {
                    // write back the offset value of a branch
                    if (nextNextBranchPos != -1)
                    {
                        WriteBackNextBranchOff(nextNextBranchPos);
                        nextNextBranchPos = -1;
                    }

                    if (i != len - 1)
                    {
                        //equalNext
                        WriteEqualNext(entries[i].IsKey, entries[i].LastValueByteCount);
                        // fillNextBranchOff 
                        nextNextBranchPos = FillNextBranchOff();
                    }
                    else
                    {
                        //equalLast
                        WriteEqualLast(entries[i].IsKey, entries[i].LastValueByteCount);
                    }

                    // determine if it is key
                    if (entries[i].IsKey)
                    {
                        //if it is the key
                        int valueOffset = WriteKey(entries[i].LastValueByteCount, entries[i].KeyType, entries[i].KeyIsNativeType, entries[i].CurrentUInt64Value);
                        valueMapOffsets.Enqueue(new KeyValuePair<int, TValue>(valueOffset, entries[i].Value));
                    }
                    else
                    {
                        //if not key
                        //writeLittleEndian8Bytes
                        WriteKeyBytes(8, entries[i].CurrentUInt64Value);
                    }

                    // determine if there are childrens
                    if (entries[i].Chidlerns != null)
                    {
                        if (entries[i].IsKey)
                        {
                            WriteHasChidlerns(true);
                        }

                        //writeReleation
                        WriteRouteData(entries[i].Chidlerns, 0, entries[i].ChidlernLength, valueMapOffsets);
                    }
                    else
                    {
                        WriteHasChidlerns(false);
                    }

                }
            }
            else
            {
                // write lessThen/lessElse
                int middle = index + length / 2;

                //lessThan
                WriteLessThen(entries[middle - 1].LastValueByteCount);
                // 1.  fillNextBranchOff 
                int a1Pos = FillNextBranchOff();
                // 2.  writeKeyBytes
                WriteKeyBytes(entries[middle - 1].LastValueByteCount, entries[middle - 1].CurrentUInt64Value);
                // 3.  writeReleation
                WriteRouteData(entries, index, middle - index, valueMapOffsets);
                // 4.  write back the offset value of a branch
                WriteBackNextBranchOff(a1Pos);

                //lessElse
                WriteLessElse();
                // 1. writeReleation
                WriteRouteData(entries, middle, length - (middle - index), valueMapOffsets);
            }
        }

        public BssomWriter GetBssomWriter()
        {
            return writer;
        }

        private int GetRelativePosition()
        {
            return (int)(writer.Position - _basePosition);
        }

        private void SeekRelativePosition(int pos)
        {
            writer.BufferWriter.Seek(pos + _basePosition);
        }

        private void WriteEqualNext(bool isKey, int byteCount)
        {
            if (isKey)
            {
                WriteMapToken((BssMapRouteToken)byteCount);
            }
            else
            {
                WriteMapToken(BssMapRouteToken.EqualNextN);
            }
        }

        private void WriteEqualLast(bool isKey, int byteCount)
        {
            if (isKey)
            {
                WriteMapToken((BssMapRouteToken)(10 + byteCount));
            }
            else
            {
                WriteMapToken(BssMapRouteToken.EqualLastN);
            }
        }

        private void WriteLessThen(int byteCount)
        {
            WriteMapToken((BssMapRouteToken)(20 + byteCount));
        }

        private void WriteLessElse()
        {
            WriteMapToken(BssMapRouteToken.LessElse);
        }

        private void WriteHasChidlerns(bool hasChidlerns)
        {
            WriteMapToken(hasChidlerns ? BssMapRouteToken.HasChildren : BssMapRouteToken.NoChildren);
        }

        private void WriteBackNextBranchOff(int postion)
        {
            int offValue = GetRelativePosition();

            SeekRelativePosition(postion);
            writer.WriteUInt16FixNumber(checked((ushort)offValue));
            SeekRelativePosition(offValue);
        }

        private int FillNextBranchOff()
        {
            int pos = GetRelativePosition();
            writer.WriteUInt16FixNumber(0);
            return pos;
        }

        private unsafe void WriteKeyBytes(byte lastValueByteCount, ulong keyBytes)
        {
            DEBUG.Assert(lastValueByteCount != 0 && lastValueByteCount <= 8);
            if (lastValueByteCount == 8)
            {
                writer.WriteWithOutTypeHead(keyBytes);
            }
            else
            {
                writer.WriteRaw64(keyBytes, lastValueByteCount);
            }
        }

        private unsafe int WriteKey(byte lastValueByteCount, byte keyType, bool keyIsNativeType, ulong keyBytes)
        {
            WriteKeyBytes(lastValueByteCount, keyBytes);
            if (keyIsNativeType)
            {
                writer.WriteNativeType(keyType);
            }
            else
            {
                writer.WriteBuildInType(keyType);
            }

            int valueOffset = GetRelativePosition();
            writer.FillUInt32FixNumber();
            return valueOffset;
        }

        private void WriteMapToken(BssMapRouteToken token)
        {
            writer.WriteWithOutTypeHead((byte)token);
        }
    }

    internal ref struct MapMetaSegmentWriterAux
    {
        private long position;
        private int advanced;

        public int Advanced => advanced;

        public void SizeHeadData(int valueCount, int maxDepth)
        {
            AdvanceSize(BssMapObjMarshal.SizeMapHead(valueCount, maxDepth));
        }

        public void SizeRouteData<TValue>(BssMapObjMarshal<TValue>.Entry[] entries, int index, int length, Queue<KeyValuePair<int, TValue>> valueMapOffsets)
        {
            if (length < 4)
            {
                // write equalNext/equalLast
                int nextNextBranchPos = -1;
                int len = index + length;

                for (int i = index; i < len; i++)
                {
                    // write back the offset value of a branch
                    if (nextNextBranchPos != -1)
                    {
                        WriteBackNextBranchOff(nextNextBranchPos);
                        nextNextBranchPos = -1;
                    }

                    if (i != len - 1)
                    {
                        //equalNext
                        WriteEqualNext(entries[i].IsKey, entries[i].LastValueByteCount);
                        // fillNextBranchOff 
                        nextNextBranchPos = FillNextBranchOff();
                    }
                    else
                    {
                        //equalLast
                        WriteEqualLast(entries[i].IsKey, entries[i].LastValueByteCount);
                    }


                    // determine if it is key
                    if (entries[i].IsKey)
                    {
                        //if it is the key
                        int valueOffset = WriteKey(entries[i].LastValueByteCount, entries[i].KeyType, entries[i].KeyIsNativeType, entries[i].CurrentUInt64Value);
                        valueMapOffsets.Enqueue(new KeyValuePair<int, TValue>(valueOffset, entries[i].Value));
                    }
                    else
                    {
                        //if not key
                        //writeLittleEndian8Bytes
                        WriteKeyBytes(8, entries[i].CurrentUInt64Value);
                    }

                    // determine if there are childrens
                    if (entries[i].Chidlerns != null)
                    {
                        if (entries[i].IsKey)
                        {
                            WriteHasChidlerns(true);
                        }

                        //writeReleation
                        SizeRouteData(entries[i].Chidlerns, 0, entries[i].ChidlernLength, valueMapOffsets);
                    }
                    else
                    {
                        WriteHasChidlerns(false);
                    }

                }
            }
            else
            {
                // write lessThen/lessElse
                int middle = index + length / 2;

                //lessThan
                WriteLessThen(entries[middle - 1].LastValueByteCount);
                // 1.  fillNextBranchOff 
                int a1Pos = FillNextBranchOff();
                // 2.  writeKeyBytes
                WriteKeyBytes(entries[middle - 1].LastValueByteCount, entries[middle - 1].CurrentUInt64Value);
                // 3.  writeReleation
                SizeRouteData(entries, index, middle - index, valueMapOffsets);
                // 4.  write back the offset value of a branch
                WriteBackNextBranchOff(a1Pos);

                //lessElse
                WriteLessElse();
                // 1. writeReleation
                SizeRouteData(entries, middle, length - (middle - index), valueMapOffsets);
            }
        }

        private int GetRelativePosition()
        {
            return (int)position;
        }

        private void SeekRelativePosition(int pos)
        {
            position = pos;
        }

        private void WriteEqualNext(bool isKey, int byteCount)
        {
            if (isKey)
            {
                WriteMapToken((BssMapRouteToken)byteCount);
            }
            else
            {
                WriteMapToken(BssMapRouteToken.EqualNextN);
            }
        }

        private void WriteEqualLast(bool isKey, int byteCount)
        {
            if (isKey)
            {
                WriteMapToken((BssMapRouteToken)(10 + byteCount));
            }
            else
            {
                WriteMapToken(BssMapRouteToken.EqualLastN);
            }
        }

        private void WriteLessThen(int byteCount)
        {
            WriteMapToken((BssMapRouteToken)(20 + byteCount));
        }

        private void WriteLessElse()
        {
            WriteMapToken(BssMapRouteToken.LessElse);
        }

        private void WriteHasChidlerns(bool hasChidlerns)
        {
            WriteMapToken(hasChidlerns ? BssMapRouteToken.HasChildren : BssMapRouteToken.NoChildren);
        }

        private void WriteBackNextBranchOff(int postion)
        {
            int offValue = GetRelativePosition();

            SeekRelativePosition(postion);
            WriteUInt16FixNumber(checked((ushort)offValue));
            SeekRelativePosition(offValue);
        }

        private int FillNextBranchOff()
        {
            int pos = GetRelativePosition();
            WriteUInt16FixNumber(0);
            return pos;
        }

        private unsafe void WriteKeyBytes(byte lastValueByteCount, ulong keyBytes)
        {
            DEBUG.Assert(lastValueByteCount != 0 && lastValueByteCount <= 8);
            AdvanceSize(lastValueByteCount);
        }

        private unsafe int WriteKey(byte lastValueByteCount, byte keyType, bool keyIsNativeType, ulong keyBytes)
        {
            WriteKeyBytes(lastValueByteCount, keyBytes);
            if (keyIsNativeType)
            {
                WriteNativeType(keyType);
            }
            else
            {
                WriteBuildInType(keyType);
            }

            int valueOffset = GetRelativePosition();
            FillUInt32FixNumber();
            return valueOffset;
        }

        private void WriteNativeType(byte keyType)
        {
            AdvanceSize(BssomBinaryPrimitives.NativeTypeCodeSize);
        }

        private void WriteBuildInType(byte keyType)
        {
            AdvanceSize(BssomBinaryPrimitives.BuildInTypeCodeSize);
        }

        private void WriteMapToken(BssMapRouteToken token)
        {
            AdvanceSize(1);
        }

        private void WriteUInt16FixNumber(ushort val)
        {
            AdvanceSize(BssomBinaryPrimitives.FixUInt16NumberSize);
        }

        private void FillUInt32FixNumber()
        {
            AdvanceSize(BssomBinaryPrimitives.FixUInt32NumberSize);
        }

        private void AdvanceSize(int size)
        {
            if (position == advanced)
            {
                advanced += size;
            }

            position += size;
        }
    }
}
