//using System.Runtime.CompilerServices;

using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;

namespace Bssom.Serializer.BssMap
{
    internal unsafe sealed class BssMapObjMarshalReader<TKey, TValue> : IMapDataSource<TKey, TValue>
    {
        private static Func<bool, byte, BssmapAnalysisStack, byte, TKey> readKeyFunc;
        private static IBssMapKeyResolver<TKey> staticKeyResolver;

        internal BssomReader _reader;
        internal BssomDeserializeContext _context;

        static BssMapObjMarshalReader()
        {
            if (typeof(TKey) == typeof(object))
                readKeyFunc = GetObjectKey;
            else
            {
                staticKeyResolver = BssMapKeyResolverProvider.GetAndVertiyBssMapKeyResolver<TKey>();
                readKeyFunc = GetKey;
            }
        }

        private BssMapObjMarshalReader()
        {
        }

        public BssomReader Reader => _reader;
        public BssomDeserializeContext Context => _context;
        public BssMapHeadPackInfo Paras { get; private set; }
        public IBssomFormatter<TValue> ValueFormatter { get; private set; }
        public int Count => Paras.MapHead.ElementCount;
        public long EndPosition => Paras.DataEndPosition;
        public bool IsOnlyReadFieldOffset => ValueFormatter == null;

        public static BssMapObjMarshalReader<TKey, TValue> Create(ref BssomReader reader, ref BssomDeserializeContext context, bool isOnlyReadFieldOffset = false)
        {
            BssMapObjMarshalReader<TKey, TValue> apr = new BssMapObjMarshalReader<TKey, TValue>();
            apr._reader = reader;
            apr._context = context;
            apr.Paras = BssMapHeadPackInfo.Create(ref reader);
            if (!isOnlyReadFieldOffset)
                apr.ValueFormatter = context.Option.FormatterResolver.GetFormatterWithVerify<TValue>();
            return apr;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (Count == 0)
            {
                return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            }

            Reader.BssomBuffer.TryReadFixedRef(Paras.MapHead.MetaLength, out bool haveEnoughSizeAndCanBeFixed);
            if (haveEnoughSizeAndCanBeFixed)
                return new Enumerator(this);
            else
                return new EnumeratorSlow(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static TKey GetKey(bool isNativeType, byte keyType, BssmapAnalysisStack stack, byte valueByteCount)
        {
            if (keyType != staticKeyResolver.KeyType && isNativeType != staticKeyResolver.KeyIsNativeType)
                return BssomSerializationOperationException.UnexpectedCodeRead<TKey>();
            return staticKeyResolver.ReadMap2Key(stack.ToUlongs((byte)valueByteCount));
        }

        private static TKey GetObjectKey(bool isNativeType, byte keyType, BssmapAnalysisStack stack, byte valueByteCount)
        {
            var convert = BssMapKeyResolverProvider.GetAndVertiyBssMapKeyResolver(isNativeType, keyType);
            return (TKey)convert.ReadMap2Key(stack.ToUlongs((byte)valueByteCount));
        }

        public class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
        {
            private BssMapObjMarshalReader<TKey, TValue> bssmapReader;
            private BssmapAnalysisStack bssaStack;
            private AutomateState state;
            private int count;
            private ushort pos;

            public Enumerator(BssMapObjMarshalReader<TKey, TValue> bssReader)
            {
                bssmapReader = bssReader;
                bssaStack = new BssmapAnalysisStack(bssmapReader.Paras.MapHead.MaxDepth);
                count = bssmapReader.Paras.MapHead.ElementCount;
                state = AutomateState.ReadBranch;
                pos = 0;
                Current = default;
            }

            public KeyValuePair<TKey, TValue> Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (count == 0)
                    return false;

                bssmapReader.Reader.BssomBuffer.Seek(bssmapReader.Paras.MapRouteDataStartPos);
                ref byte refBase = ref bssmapReader.Reader.BssomBuffer.ReadRef(0);
                ref byte ref1 = ref Unsafe.Add(ref refBase, pos);
                byte nextKeyByteCount = 0;
                BssMapRouteToken token = default;
                switch (state)
                {
                    case AutomateState.ReadBranch:
                        {
                            token = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            bssaStack.PushToken(token);

                            if (token >= BssMapRouteToken.EqualNext1 && token <= BssMapRouteToken.EqualNext8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(token);

                                ref1 = ref Unsafe.Add(ref ref1, 1 + 3);//ignore nextOff
                                goto case AutomateState.ReadKey;
                            }
                            else if (token >= BssMapRouteToken.EqualLast1 && token <= BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(token);

                                ref1 = ref Unsafe.Add(ref ref1, 1);
                                goto case AutomateState.ReadKey;
                            }
                            else if (token == BssMapRouteToken.EqualNextN)
                            {
                                ref1 = ref Unsafe.Add(ref ref1, 1 + 3);//ignore nextOff
                                bssaStack.PushValue(BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                                goto case AutomateState.ReadBranch;
                            }
                            else if (token == BssMapRouteToken.EqualLastN)
                            {
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                                bssaStack.PushValue(BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                                goto case AutomateState.ReadBranch;
                            }
                            else if (token >= BssMapRouteToken.LessThen1 && token <= BssMapRouteToken.LessThen8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(token);

                                ref1 = ref Unsafe.Add(ref ref1, 1 + 3 + nextKeyByteCount);//ignore nextOff
                                goto case AutomateState.ReadBranch;
                            }
                            else if (token == BssMapRouteToken.LessElse)
                            {
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                                goto case AutomateState.ReadBranch;
                            }
                            else
                            {
                                throw BssomSerializationOperationException.UnexpectedCodeRead((byte)token, bssmapReader.Paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                            }
                        }
                    case AutomateState.ReadKey:
                        {
                            if (nextKeyByteCount == 8)
                            {
                                //Read Little Endian 8Bytes
                                bssaStack.PushValue(BssomBinaryPrimitives.ReadUInt64LittleEndian(ref ref1));
                                ref1 = ref Unsafe.Add(ref ref1, 8);
                            }
                            else
                            {
                                //Read Raw(lessthan 8 byte)
                                ulong val = 0;
                                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref val), ref ref1, nextKeyByteCount);
                                ref1 = ref Unsafe.Add(ref ref1, nextKeyByteCount);
                                bssaStack.PushValue(BssomBinaryPrimitives.ReadRawUInt64LittleEndian(val));
                            }

                            TKey key;
                            var keyType = Unsafe.ReadUnaligned<byte>(ref ref1);
                            if (keyType == BssomType.NativeCode)
                            {
                                ref1 = ref Unsafe.Add(ref ref1, 1);
                                key = readKeyFunc(true, Unsafe.ReadUnaligned<byte>(ref ref1), bssaStack, nextKeyByteCount);
                            }
                            else
                                key = readKeyFunc(false, keyType, bssaStack, nextKeyByteCount);
                            ref1 = ref Unsafe.Add(ref ref1, 1 + 1);//skip FixUInt32Code
                            var offset = BssomBinaryPrimitives.ReadUInt32LittleEndian(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 4);

                            if (bssmapReader.IsOnlyReadFieldOffset == false)
                            {
                                bssmapReader.Reader.BssomBuffer.Seek(bssmapReader.Paras.ReadPosition + offset);
                                var value = bssmapReader.ValueFormatter.Deserialize(ref bssmapReader._reader, ref bssmapReader._context);
                                Current = new KeyValuePair<TKey, TValue>(key, value);
                            }
                            else
                            {
                                var offsetInfo = new BssomFieldOffsetInfo(bssmapReader.Paras.ReadPosition + offset);
                                Current = new KeyValuePair<TKey, TValue>(key, Unsafe.As<BssomFieldOffsetInfo, TValue>(ref offsetInfo));
                            }
                            count--;

                            pos = (ushort)Unsafe.ByteOffset(ref refBase, ref ref1);
                            state = AutomateState.ReadChildren;
                            return true;
                        }
                    case AutomateState.ReadChildren:
                        {
                            token = Unsafe.ReadUnaligned<BssMapRouteToken>(ref ref1);
                            ref1 = ref Unsafe.Add(ref ref1, 1);
                            if (token == BssMapRouteToken.HasChildren)
                            {
                                goto case AutomateState.ReadBranch;
                            }
                            else if (token == BssMapRouteToken.NoChildren)
                            {
                            GO1:
                                token = bssaStack.PeekToken();
                                if (token >= BssMapRouteToken.EqualNext1 && token <= BssMapRouteToken.EqualNextN)
                                {
                                    bssaStack.PopToken();
                                    bssaStack.PopValue();
                                    goto case AutomateState.ReadBranch;
                                }
                                else if (token >= BssMapRouteToken.EqualLast1 && token <= BssMapRouteToken.EqualLastN)
                                {
                                    bssaStack.PopToken();
                                    bssaStack.PopValue();
                                    if (bssaStack.TokenCount > 0)
                                        goto GO1;
                                    else
                                        goto case AutomateState.CheckEnd;
                                }
                                else if (token >= BssMapRouteToken.LessThen1 && token <= BssMapRouteToken.LessThen8)
                                {
                                    bssaStack.PopToken();
                                    goto case AutomateState.ReadBranch;
                                }
                                else if (token == BssMapRouteToken.LessElse)
                                {
                                    bssaStack.PopToken();
                                    if (bssaStack.TokenCount > 0)
                                        goto GO1;
                                    goto case AutomateState.CheckEnd;
                                }
                                else
                                    throw BssomSerializationOperationException.UnexpectedCodeRead((byte)token, bssmapReader.Paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                            }
                            else
                                throw BssomSerializationOperationException.UnexpectedCodeRead((byte)token, bssmapReader.Paras.MapRouteDataStartPos + (int)Unsafe.ByteOffset(ref refBase, ref ref1));
                        }
                    case AutomateState.CheckEnd:
                        {
                            if (count != 0)
                            {
                                goto case AutomateState.ReadBranch;
                            }
                            break;
                        }
                }

                pos = (ushort)Unsafe.ByteOffset(ref refBase, ref ref1);
                return false;
            }

            public void Reset()
            {
                bssaStack = new BssmapAnalysisStack(bssmapReader.Paras.MapHead.MaxDepth);
                count = bssmapReader.Paras.MapHead.ElementCount;
                state = AutomateState.ReadBranch;
                pos = 0;
                Current = default;
            }
        }

        public class EnumeratorSlow : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
        {
            private BssMapObjMarshalReader<TKey, TValue> bssmapReader;
            private BssmapAnalysisStack bssaStack;
            private AutomateState state;
            private int count;

            public EnumeratorSlow(BssMapObjMarshalReader<TKey, TValue> bssReader)
            {
                bssmapReader = bssReader;
                count = bssmapReader.Paras.MapHead.ElementCount;
                state = AutomateState.ReadBranch;
                bssaStack = new BssmapAnalysisStack(bssmapReader.Paras.MapHead.MaxDepth);

                Current = default;
            }

            public KeyValuePair<TKey, TValue> Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (count == 0)
                    return false;

                byte nextKeyByteCount = 0;
                BssMapRouteToken token = default;
                switch (state)
                {
                    case AutomateState.ReadBranch:
                        {
                            token = bssmapReader.Reader.ReadMapToken();
                            bssaStack.PushToken(token);

                            if (token >= BssMapRouteToken.EqualNext1 && token <= BssMapRouteToken.EqualNext8 )
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(token);

                                bssmapReader.Reader.BssomBuffer.SeekWithOutVerify(3, BssomSeekOrgin.Current);//ignore nextOff
                                goto case AutomateState.ReadKey;
                            }
                            else if (token >= BssMapRouteToken.EqualLast1 && token <= BssMapRouteToken.EqualLast8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetEqualNextOrLastByteCount(token);

                                goto case AutomateState.ReadKey;
                            }
                            else if (token == BssMapRouteToken.EqualNextN )
                            {
                                bssmapReader.Reader.BssomBuffer.SeekWithOutVerify(3, BssomSeekOrgin.Current);//ignore nextOff
                                bssaStack.PushValue(bssmapReader.Reader.ReadUInt64WithOutTypeHead());
                                goto case AutomateState.ReadBranch;
                            }
                            else if (token == BssMapRouteToken.EqualLastN)
                            {
                                bssaStack.PushValue(bssmapReader.Reader.ReadUInt64WithOutTypeHead());
                                goto case AutomateState.ReadBranch;
                            }
                            else if (token >= BssMapRouteToken.LessThen1 && token <= BssMapRouteToken.LessThen8)
                            {
                                nextKeyByteCount = BssMapRouteTokenHelper.GetLessThenByteCount(token);
                                bssmapReader.Reader.BssomBuffer.Seek(3 + nextKeyByteCount, BssomSeekOrgin.Current);//ignore nextOff
                                goto case AutomateState.ReadBranch;
                            }
                            else if (token == BssMapRouteToken.LessElse)
                            {
                                goto case AutomateState.ReadBranch;
                            }
                            else
                                throw BssomSerializationOperationException.UnexpectedCodeRead((byte)token, bssmapReader.Reader.Position);
                        }
                    case AutomateState.ReadKey:
                        {
                            if (nextKeyByteCount == 8)
                            {
                                //Read Little Endian 8Bytes
                                bssaStack.PushValue(bssmapReader.Reader.ReadUInt64WithOutTypeHead());
                            }
                            else
                            {
                                //Read Raw(lessthan 8 byte)
                                bssaStack.PushValue(BssomBinaryPrimitives.ReadRawUInt64LittleEndian(bssmapReader.Reader.ReadRaw64(nextKeyByteCount)));
                            }

                            TKey key;
                            var keyType = bssmapReader.Reader.ReadBssomType();
                            if (keyType == BssomType.NativeCode)
                                key = readKeyFunc(true, bssmapReader.Reader.ReadBssomType(), bssaStack, nextKeyByteCount);
                            else
                                key = readKeyFunc(false, keyType, bssaStack, nextKeyByteCount);

                            bssmapReader.Reader.BssomBuffer.Seek(1, BssomSeekOrgin.Current);//Skip FixUInt32Code
                            int valOffset = (int)bssmapReader.Reader.ReadUInt32WithOutTypeHead();

                            if (bssmapReader.IsOnlyReadFieldOffset == false)
                            {
                                long curPos = bssmapReader.Reader.Position;
                                bssmapReader.Reader.BssomBuffer.Seek(bssmapReader.Paras.ReadPosition + valOffset);
                                var value = bssmapReader.ValueFormatter.Deserialize(ref bssmapReader._reader, ref bssmapReader._context);
                                Current = new KeyValuePair<TKey, TValue>(key, value);
                                bssmapReader.Reader.BssomBuffer.Seek(curPos);
                            }
                            else
                            {
                                var offsetInfo = new BssomFieldOffsetInfo(bssmapReader.Paras.ReadPosition + valOffset);
                                Current = new KeyValuePair<TKey, TValue>(key, Unsafe.As<BssomFieldOffsetInfo, TValue>(ref offsetInfo));
                            }
                            count--;


                            state = AutomateState.ReadChildren;
                            return true;
                        }
                    case AutomateState.ReadChildren:
                        {
                            token = bssmapReader.Reader.ReadMapToken();
                            if (token == BssMapRouteToken.HasChildren)
                            {
                                goto case AutomateState.ReadBranch;
                            }
                            else if (token == BssMapRouteToken.NoChildren)
                            {
                            GO1:
                                token = bssaStack.PeekToken();
                                if (token >= BssMapRouteToken.EqualNext1 && token <= BssMapRouteToken.EqualNextN)
                                {
                                    bssaStack.PopToken();
                                    bssaStack.PopValue();
                                    goto case AutomateState.ReadBranch;
                                }
                                else if (token >= BssMapRouteToken.EqualLast1 && token <= BssMapRouteToken.EqualLastN)
                                {
                                    bssaStack.PopToken();
                                    bssaStack.PopValue();
                                    if (bssaStack.TokenCount > 0)
                                        goto GO1;
                                    else
                                        goto case AutomateState.CheckEnd;
                                }
                                else if (token >= BssMapRouteToken.LessThen1 && token <= BssMapRouteToken.LessThen8)
                                {
                                    bssaStack.PopToken();
                                    goto case AutomateState.ReadBranch;
                                }
                                else if (token == BssMapRouteToken.LessElse)
                                {
                                    bssaStack.PopToken();
                                    if (bssaStack.TokenCount > 0)
                                        goto GO1;
                                    goto case AutomateState.CheckEnd;
                                }
                                else
                                    throw BssomSerializationOperationException.UnexpectedCodeRead((byte)token, bssmapReader.Reader.Position);
                            }
                            else
                                throw BssomSerializationOperationException.UnexpectedCodeRead((byte)token, bssmapReader.Reader.Position);
                        }
                    case AutomateState.CheckEnd:
                        {
                            if (count != 0)
                            {
                                goto case AutomateState.ReadBranch;
                            }
                            break;
                        }
                }

                return false;
            }

            public void Reset()
            {
                count = bssmapReader.Paras.MapHead.ElementCount;
                state = AutomateState.ReadBranch;
                bssaStack = new BssmapAnalysisStack(bssmapReader.Paras.MapHead.MaxDepth);

                Current = default;
            }
        }
    }
}
