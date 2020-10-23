using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
namespace Bssom.Serializer.BssMap
{
    internal static class BssMapObjMarshalReader
    {
        #region TryGetValue-Inline

        public static unsafe Byte TryGetUInt8Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadUInt8();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe SByte TryGetInt8Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadInt8();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Char TryGetCharValue(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadChar();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Int16 TryGetInt16Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadInt16();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe UInt16 TryGetUInt16Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadUInt16();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Int32 TryGetInt32Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadInt32();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe UInt32 TryGetUInt32Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadUInt32();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Int64 TryGetInt64Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadInt64();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe UInt64 TryGetUInt64Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadUInt64();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Single TryGetFloat32Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadFloat32();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Double TryGetFloat64Value(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadFloat64();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Decimal TryGetDecimalValue(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadDecimal();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Boolean TryGetBooleanValue(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadBoolean();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Guid TryGetGuidValue(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadGuid();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe DateTime TryGetDateTimeValue(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadDateTime();

        ReturnFalse:
            isGet = false;
            return default;
        }
 
        #endregion

        #region TryGetValueSlow-Inline

        public static unsafe Byte TryGetUInt8ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadUInt8();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe SByte TryGetInt8ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadInt8();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Char TryGetCharValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadChar();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Int16 TryGetInt16ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadInt16();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe UInt16 TryGetUInt16ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadUInt16();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Int32 TryGetInt32ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadInt32();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe UInt32 TryGetUInt32ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadUInt32();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Int64 TryGetInt64ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadInt64();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe UInt64 TryGetUInt64ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadUInt64();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Single TryGetFloat32ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadFloat32();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Double TryGetFloat64ValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadFloat64();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Decimal TryGetDecimalValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadDecimal();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Boolean TryGetBooleanValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadBoolean();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe Guid TryGetGuidValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadGuid();

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe DateTime TryGetDateTimeValueSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet)
        {  
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return reader.ReadDateTime();

        ReturnFalse:
            isGet = false;
            return default;
        }
 
        #endregion


        #region Seek
        public static unsafe bool TrySeek(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref byte ref1)
        {
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //return
            return true;

        ReturnFalse:
            return false;
        }
        public static unsafe bool TrySeekSlow(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader)
        {
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //return
            return true;

        ReturnFalse:
            return false;
        }
        public static unsafe bool TrySeek(ISegment<ulong> key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref byte ref1)
        {
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //return
            return true;

        ReturnFalse:
            return false;
        }
        public static unsafe bool TrySeekSlow(ISegment<ulong> key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader)
        {
            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //return
            return true;

        ReturnFalse:
            return false;
        }

        #endregion

        public static MethodInfo TryGetValueUlongsKeyMethodInfo = typeof(BssMapObjMarshalReader).GetMethods().Where(e => e.Name == nameof(TryGetValue) && e.IsGenericMethod && e.GetParameters()[0].ParameterType == typeof(ulong[])).First();
        public static MethodInfo TryGetValueSlowUlongsKeyMethodInfo = typeof(BssMapObjMarshalReader).GetMethods().Where(e => e.Name == nameof(TryGetValueSlow) && e.GetParameters()[0].ParameterType == typeof(ulong[])).First();
        public static unsafe TValue TryGetValue<TValue>(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet, IBssomFormatter<TValue> formatter = null)
        {
            if (formatter == null)
                formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TValue>();

            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return formatter.Deserialize(ref reader, ref context);

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe TValue TryGetValueSlow<TValue>(ulong[] key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet, IBssomFormatter<TValue> formatter = null)
        {
            if (formatter == null)
                formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TValue>();

            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return formatter.Deserialize(ref reader, ref context);

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe TValue TryGetValue<TValue>(ISegment<ulong> key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, ref byte ref1, out bool isGet, IBssomFormatter<TValue> formatter = null)
        {
            if (formatter == null)
                formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TValue>();

            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            ref byte refBase = ref ref1;
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;
            int mapHeadSize = paras.MapHeadSize;
            
            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        ref1 = ref Unsafe.Add(ref ref1, 1);

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);

                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref Unsafe.Add(ref ref1, 2));
                                ref1 = ref Unsafe.Add(ref ref1, 2 + 8);
                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref Unsafe.Add(ref ref1, 2), nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, 2 + nextKeyByteCount);
                            }

                            if (key[keyPos] > value1)//goto lessElse
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);
                                if (Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1) != BssMapRouteToken.LessElse)
                                    throw BssomSerializationOperationException.UnexpectedCodeRead(ref1, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }

                            //read children branch head
                            t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);//skip FixUInt16Code
                            ushort nextOff = BssomBinaryPrimitives.ReadUInt16LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 2);

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }
                            
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                ref1 = ref Unsafe.Add(ref refBase, nextOff - mapHeadSize);

                                //read next loop head
                                t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1);
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref ref1, nextKeyByteCount);
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        ref1 = ref Unsafe.Add(ref ref1, 1 + 1 + 4);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }

                case AutomateReadOneKeyState.ReadChildren:
                    {
                        t = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                        if (t == BssMapRouteToken.HasChildren)
                        {
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            goto case AutomateReadOneKeyState.ReadNextBranch;
                        }
                        goto ReturnFalse;
                    }
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
            {
                if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                    throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                ref1 = Unsafe.Add(ref ref1, 1);
            }
            if (Unsafe.ReadUnaligned<byte>(ref ref1) != keyType)
                   throw BssomSerializationOperationException.UnexpectedCodeRead(keyType, paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));

            //seek valOffset
            reader.BssomBuffer.Seek(BssomBinaryPrimitives.ReadUInt32LittleEndian(ref Unsafe.Add(ref ref1, 1 + 1)) + paras.ReadPosition);
            //read val
            isGet = true;
            return formatter.Deserialize(ref reader, ref context);

        ReturnFalse:
            isGet = false;
            return default;
        }
        public static unsafe TValue TryGetValueSlow<TValue>(ISegment<ulong> key, byte keyType, bool keyIsNativeType, ref BssMapHeadPackInfo paras, ref BssomReader reader, ref BssomDeserializeContext context, out bool isGet, IBssomFormatter<TValue> formatter = null)
        {
            if (formatter == null)
                formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TValue>();

            int keyLength = key.Length;
            if (paras.MapHead.MaxDepth < keyLength)
                goto ReturnFalse;

            reader.BssomBuffer.Seek(paras.MapRouteDataStartPos);
            int keyPos = 0;
            AutomateReadOneKeyState token = AutomateReadOneKeyState.ReadNextBranch;
            BssMapRouteToken t = default;
            byte nextKeyByteCount = 0;
            ulong value1 = 0;

            switch (token)
            {
                case AutomateReadOneKeyState.ReadNextBranch:
                    {
                        t = reader.ReadMapToken();

                        while (t >= BssMapRouteToken.LessThen1 && t <= BssMapRouteToken.LessThen8)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();
                            //read branch keyByte
                            if (t == BssMapRouteToken.LessThen8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();

                            }
                            else
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            if (key[keyPos] > value1)
                            {
                                reader.BssomBuffer.Seek(nextOff + paras.ReadPosition);
                                reader.EnsureMapToken(BssMapRouteToken.LessElse);
                            }
                            t = reader.ReadMapToken();
                        }

                        while (t >= BssMapRouteToken.EqualNext1 && t <= BssMapRouteToken.EqualNextN)
                        {
                            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//skip FixUInt16Code
                            ushort nextOff = reader.ReadUInt16WithOutTypeHead();

                            //read keyByte
                            if (t == BssMapRouteToken.EqualNextN || t == BssMapRouteToken.EqualNext8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualNext1 && t < BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }
                            ulong keyValue = key[keyPos];
                            if (keyValue > value1)
                            {
                                reader.BssomBuffer.SeekWithOutVerify(nextOff + paras.ReadPosition, BssomSeekOrgin.Begin);
                                t = reader.ReadMapToken();
                            }
                            else if (keyValue == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualNextN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }

                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualNextN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else//keyValue < value1
                            {
                                goto ReturnFalse;
                            }
                        }

                        if (t >= BssMapRouteToken.EqualLast1 && t <= BssMapRouteToken.EqualLastN)
                        {
                            //read keyByte
                            if (t == BssMapRouteToken.EqualLastN || t == BssMapRouteToken.EqualLast8)
                            {
                                //Read Little Endian 8Bytes
                                value1 = reader.ReadUInt64WithOutTypeHead();
                            }
                            else // t >= BssMapRouteToken.EqualLast1 && t < BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(t);

                                //Read Raw(lessthan 8 byte)
                                value1 = BssomBinaryPrimitives.ReadRawUInt64LittleEndian(reader.ReadRaw64(nextKeyByteCount));
                            }

                            if (key[keyPos] == value1)
                            {
                                if (keyPos != keyLength - 1)
                                {
                                    keyPos++;
                                    if (t != BssMapRouteToken.EqualLastN)
                                    {
                                        //skip keyType and valoffset
                                        reader.BssomBuffer.SeekWithOutVerify(1 + 1 + 4, BssomSeekOrgin.Current);
                                        goto case AutomateReadOneKeyState.ReadChildren;
                                    }
                                    goto case AutomateReadOneKeyState.ReadNextBranch;
                                }
                                else
                                {
                                    if (t == BssMapRouteToken.EqualLastN)
                                        goto ReturnFalse;

                                    goto TryReadValue;
                                }
                            }
                            else
                            {
                                goto ReturnFalse;
                            }
                        }

                        throw BssomSerializationOperationException.UnexpectedCodeRead((byte)t,reader.Position);
                    }
                case AutomateReadOneKeyState.ReadChildren:
                    t = reader.ReadMapToken();
                    if (t == BssMapRouteToken.HasChildren)
                    {
                        goto case AutomateReadOneKeyState.ReadNextBranch;
                    }
                    goto ReturnFalse;
            }

        TryReadValue:
            //verify keyType
            if (keyIsNativeType)
                reader.EnsureType(BssomType.NativeCode);
            reader.EnsureType(keyType);
            //seek valOffset
            reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);
            reader.BssomBuffer.Seek(reader.ReadUInt32WithOutTypeHead() + paras.ReadPosition);
            //read val
            isGet = true;
            return formatter.Deserialize(ref reader, ref context);

        ReturnFalse:
            isGet = false;
            return default;
        }

    }
}