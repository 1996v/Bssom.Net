
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
namespace Bssom.Serializer.BssMap
{
    internal static class MapFormatterHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Size<TKey, TValue>(ref BssomSizeContext context, IEnumerable<KeyValuePair<TKey, TValue>> value, int count)
        {
            if (typeof(TKey) == typeof(DateTime))
            {
                context.Option = context.Option.WithIsUseStandardDateTime(false);
            }

            if (context.Option.IDictionaryIsSerializeMap1Type)
            {
                return MapFormatterHelper_Map1.Size(ref context, value, count);
            }
            else
            {
                return MapFormatterHelper_Map2.Size(ref context, value, count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Serialize<TKey, TValue>(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<KeyValuePair<TKey, TValue>> value, int count)
        {
            if (typeof(TKey) == typeof(DateTime))
            {
                context.Option = context.Option.WithIsUseStandardDateTime(false);
            }

            if (context.Option.IDictionaryIsSerializeMap1Type)
            {
                MapFormatterHelper_Map1.Serialize(ref writer, value, ref context, count);
            }
            else
            {
                MapFormatterHelper_Map2.Serialize(ref writer, value, ref context, count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMapDataSource<TKey, TValue> Deserialize<TKey, TValue>(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            byte type = reader.SkipBlankCharacterAndReadBssomType();
            if (type == BssomType.NullCode)
            {
                return default;
            }

            IMapDataSource<TKey, TValue> map;
            if (type == BssomType.Map1)
            {
                map = new Map1DataSource<TKey, TValue>(reader, context);
            }
            else if (type == BssomType.Map2)
            {
                map = BssMapObjMarshalReader<TKey, TValue>.Create(ref reader, ref context);
            }
            else
            {
                map = BssomSerializationOperationException.UnexpectedCodeRead<IMapDataSource<TKey, TValue>>(type, reader.Position);
            }

            return map;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMapDataSource<TKey, BssomFieldOffsetInfo> ReadAllKeys<TKey>(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            byte type = reader.SkipBlankCharacterAndReadBssomType();
            if (type == BssomType.NullCode)
            {
                return default;
            }

            IMapDataSource<TKey, BssomFieldOffsetInfo> map;
            if (type == BssomType.Map1)
            {
                map = new Map1DataSource<TKey, BssomFieldOffsetInfo>(reader, context, true);
            }
            else if (type == BssomType.Map2)
            {
                map = BssMapObjMarshalReader<TKey, BssomFieldOffsetInfo>.Create(ref reader, ref context, true);
            }
            else
            {
                map = BssomSerializationOperationException.UnexpectedCodeRead<IMapDataSource<TKey, BssomFieldOffsetInfo>>(type, reader.Position);
            }

            return map;
        }

        //Serialize IDictionary
        public static void SerializeIDictionary(ref BssomWriter writer, ref BssomSerializeContext context, IDictionary value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Serialize<object, object>(ref writer, ref context, value.GetGetEnumerator(), value.Count);
        }

        //Size IDictionary<TKey,TValue>/IReadOnlyDictionary<TKey,TValue>
        public static int SizeIDictionary(ref BssomSizeContext context, IDictionary value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return Size(ref context, value.GetGetEnumerator(), value.Count);
        }

        //Size IDictionary<TKey,TValue>/IReadOnlyDictionary<TKey,TValue>
        public static int SizeGenericDictionary<TKey, TValue>(ref BssomSizeContext context, IEnumerable<KeyValuePair<TKey, TValue>> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return Size(ref context, value, value.GetIDictionaryCount());
        }

        //Serialize IDictionary<TKey,TValue>/IReadOnlyDictionary<TKey,TValue>
        public static void SerializeGenericDictionary<TKey, TValue>(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<KeyValuePair<TKey, TValue>> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Serialize(ref writer, ref context, value, value.GetIDictionaryCount());
        }

        //Deserialize IDictionary<TKey,TValue>/IDictionary
        public static Dictionary<TKey, TValue> GenericDictionaryDeserialize<TKey, TValue>(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            IMapDataSource<TKey, TValue> map = Deserialize<TKey, TValue>(ref reader, ref context);
            if (map == null)
            {
                return null;
            }

            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>(map.Count);
            foreach (KeyValuePair<TKey, TValue> item in map)
            {
                dict.Add(item.Key, item.Value);
            }

            reader = map.Reader;
            context = map.Context;
            reader.BssomBuffer.Seek(map.EndPosition);

            return dict;
        }

        //Deserialize IReadOnlyDictionary<TKey,TValue>
        public static ReadOnlyDictionary<TKey, TValue> ReadOnlyGenericDictionaryDeserialize<TKey, TValue>(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            Dictionary<TKey, TValue> dict = GenericDictionaryDeserialize<TKey, TValue>(ref reader, ref context);
            if (dict == null)
            {
                return null;
            }

            return new ReadOnlyDictionary<TKey, TValue>(dict);
        }

        public static void FillGenericIDictionaryData<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> pairs, ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (KeyValuePair<TKey, TValue> item in pairs)
            {
                collection.Add(item);
            }
        }

        public static void FillIDictionaryData(IEnumerable<KeyValuePair<object, object>> pairs, IDictionary dictionary)
        {
            foreach (KeyValuePair<object, object> item in pairs)
            {
                dictionary.Add(item.Key, item.Value);
            }
        }
    }

    internal static class MapFormatterHelper_Map1
    {
        public static int Size<TKey, TValue>(ref BssomSizeContext context, IEnumerable<KeyValuePair<TKey, TValue>> value, int count = -1)
        {
            DEBUG.Assert(value != null);

            IBssomFormatter<TKey> keyFormatter = context.Option.FormatterResolver.GetFormatterWithVerify<TKey>();
            IBssomFormatter<TValue> valueFormatter = context.Option.FormatterResolver.GetFormatterWithVerify<TValue>();
            bool keyIsObjectType = typeof(TKey) == typeof(object);
            bool keyIsStringType = typeof(TKey) == typeof(string);

            int len = BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.FixUInt32NumberSize;
            if (count == -1)
            {
                len += BssomBinaryPrimitives.FixUInt32NumberSize;
            }
            else
            {
                len += BssomBinaryPrimitives.VariableNumberSize((ulong)count);
            }

            foreach (KeyValuePair<TKey, TValue> item in value)
            {
                if (keyIsObjectType)
                {
                    BssMapKeyResolverProvider.VertyBssMapKeyType(item.Key);
                }
                else if (keyIsStringType)
                {
                    BssMapKeyResolverProvider.VertyBssMapStringKey(item.Key);
                }

                len += keyFormatter.Size(ref context, item.Key);
                len += valueFormatter.Size(ref context, item.Value);
            }

            return len;
        }

        public static void Serialize<TKey, TValue>(ref BssomWriter writer, IEnumerable<KeyValuePair<TKey, TValue>> value, ref BssomSerializeContext context, int count = -1)
        {
            DEBUG.Assert(value != null);

            IBssomFormatter<TKey> keyFormatter = context.Option.FormatterResolver.GetFormatterWithVerify<TKey>();
            IBssomFormatter<TValue> valueFormatter = context.Option.FormatterResolver.GetFormatterWithVerify<TValue>();
            bool keyIsObjectType = typeof(TKey) == typeof(object);
            bool keyIsStringType = typeof(TKey) == typeof(string);

            //Map1 + len + count
            writer.WriteBuildInType(BssomType.Map1);
            long lenPos = writer.FillUInt32FixNumber();
            if (count != -1)
            {
                writer.WriteVariableNumber(count);
            }
            else
            {
                writer.FillUInt32FixNumber();
            }

            int num = 0;
            foreach (KeyValuePair<TKey, TValue> item in value)
            {
                if (keyIsObjectType)
                {
                    BssMapKeyResolverProvider.VertyBssMapKeyType(item.Key);
                }
                else if (keyIsStringType)
                {
                    BssMapKeyResolverProvider.VertyBssMapStringKey(item.Key);
                }

                //Write Key
                keyFormatter.Serialize(ref writer, ref context, item.Key);

                //Write Value
                valueFormatter.Serialize(ref writer, ref context, item.Value);

                num++;
            }

            if (count == -1)
            {
                long currentPos = writer.Position;
                writer.BufferWriter.Seek(lenPos);
                writer.WriteBackFixNumber(checked((int)(currentPos - lenPos - BssomBinaryPrimitives.FixUInt32NumberSize)));
                writer.WriteBackFixNumber(num);
                writer.BufferWriter.Seek(currentPos);
            }
            else
            {
                writer.WriteBackFixNumber(lenPos, checked((int)(writer.Position - lenPos - BssomBinaryPrimitives.FixUInt32NumberSize)));
            }
        }
    }

    internal static class MapFormatterHelper_Map2
    {
        private static BssRow<TValue>[] GenericRows<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> value, int count)
        {
            bool keyIsObjectType = typeof(TKey) == typeof(object);
            IBssMapKeyResolver<TKey> genericKeyConvert = default;
            if (keyIsObjectType == false)
            {
                genericKeyConvert = BssMapKeyResolverProvider.GetAndVertiyBssMapKeyResolver<TKey>();
            }
            ArrayPack<BssRow<TValue>> rows = new ArrayPack<BssRow<TValue>>(count);
            foreach (KeyValuePair<TKey, TValue> item in value)
            {
                if (keyIsObjectType)
                {
                    //VertyNull(item.Key)
                    IBssMapKeyResolver keyConvert = BssMapKeyResolverProvider.GetAndVertiyBssMapKeyResolver(item.Key.GetType());
                    rows.Add(new BssRow<TValue>(keyConvert.GetMap2KeySegment(item.Key), item.Value, keyConvert.KeyType, keyConvert.KeyIsNativeType));
                }
                else
                {
                    rows.Add(new BssRow<TValue>(genericKeyConvert.GetMap2KeySegment(item.Key), item.Value, genericKeyConvert.KeyType, genericKeyConvert.KeyIsNativeType));
                }
            }

            return rows.GetArray();
        }

        public static int Size<TKey, TValue>(ref BssomSizeContext context, IEnumerable<KeyValuePair<TKey, TValue>> value, int count)
        {
            DEBUG.Assert(value != null);

            BssMapObjMarshal<TValue> ap = new BssMapObjMarshal<TValue>(GenericRows(value, count));
            return BssomBinaryPrimitives.BuildInTypeCodeSize + ap.Size(ref context);
        }

        public static void Serialize<TKey, TValue>(ref BssomWriter writer, IEnumerable<KeyValuePair<TKey, TValue>> value, ref BssomSerializeContext context, int count)
        {
            DEBUG.Assert(value != null);

            writer.WriteBuildInType(BssomType.Map2);
            BssMapObjMarshal<TValue> ap = new BssMapObjMarshal<TValue>(GenericRows(value, count));
            ap.Write(ref writer, ref context);
        }
    }


}
