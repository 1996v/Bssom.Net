using Bssom.Serializer;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Xunit
{
    internal static class VerifyHelper
    {
        public static void Throws<T>(Action testCode, Func<T, bool> ex) where T : Exception
        {
            try
            {
                testCode();
            }
            catch (T t)
            {
                if (ex(t) == false)
                    throw new Exception("Different exception information", t);
                return;
            }
            catch
            {
                throw;
            }
            throw new Exception("No exception");
        }

        public static void Throws<T>(Action testCode) where T : Exception
        {
            try
            {
                testCode();
            }
            catch (T)
            {
                return;
            }
            catch
            {
                throw;
            }
            throw new Exception("No exception");
        }

        public static void VerifyNullableIsNull<T>(BssomSerializerOptions option = null) where T : struct
        {
            var buf = BssomSerializer.Serialize<Nullable<T>>(null, option);
            BssomSerializer.Size<Nullable<T>>(null, option).Is(1).Is(buf.Length);
            BssomSerializer.Deserialize<Nullable<T>>(buf, option).IsNull();
        }

        public static void VerifyTypeNull<T>(BssomSerializerOptions option = null) where T : class
        {
            var buf = BssomSerializer.Serialize<T>(null, option);
            BssomSerializer.Size<T>(null, option).Is(1).Is(buf.Length);
            BssomSerializer.Deserialize<T>(buf, option).IsNull();
        }

        public static void VerifySimpleType<T>(T value, int size, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(size).Is(buf.Length);
            BssomSerializer.Deserialize<T>(buf, option).Is(value);
        }

        private static byte[] SimplicityVerify<T>(T value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            BssomSerializer.Deserialize<T>(buf, option).Is(value);
            return buf;
        }

        public static void VerifySimpleType<T>(T value, BssomSerializerOptions option = null)
        {
            SimplicityVerify(value, option);
        }

        public static byte[] VerifyIDictAndReturnSerializeBytes<T, TKey, TValue>(T value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            ((IEnumerable<KeyValuePair<TKey, TValue>>)BssomSerializer.Deserialize<T>(buf, option)).IsDict((IEnumerable<KeyValuePair<TKey, TValue>>)value);
            return buf;
        }

        public static byte[] VerifyIDictAndReturnSerializeBytes(IDictionary value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            BssomSerializer.Deserialize<IDictionary>(buf, option).GetGetEnumerator().IsDict(value.GetGetEnumerator());
            return buf;
        }

        public static void IsMap1(this byte[] bytes)
        {
            var reader = new BssomReader(new SimpleBufferWriter(bytes));
            reader.ReadBssomType().Is(BssomType.Map1);
        }

        public static void VerifyMap1Type<T>(T value, BssomSerializerOptions option = null)
        {
            SimplicityVerify(value, option).IsMap1();
        }

        public static void IsMap2(this byte[] bytes)
        {
            var reader = new BssomReader(new SimpleBufferWriter(bytes));
            reader.ReadBssomType().Is(BssomType.Map2);
        }

        public static void VerifyIDictWithMap2Type<T, TKey, TValue>(T value, BssomSerializerOptions option = null)
        {
            VerifyIDictAndReturnSerializeBytes<T, TKey, TValue>(value, option).IsMap2();
        }

        public static void VerifyIDictWithMap2Type(IDictionary value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            BssomSerializer.Deserialize<IDictionary>(buf, option).GetGetEnumerator().IsDict(value.GetGetEnumerator());
            buf.IsMap2();
        }

        public static BssomValueType ConvertObjectAndVerifyBssomValueType<T>(T value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize<object>(value, option);
            BssomSerializer.Size<object>(value, option).Is(buf.Length);
            ((T)BssomSerializer.Deserialize<object>(buf, (option ?? BssomSerializerOptions.Default).WithIsPriorityToDeserializeObjectAsBssomValue(false))).Is(value);
            (BssomSerializer.Deserialize<object>(buf, (option ?? BssomSerializerOptions.Default).WithIsPriorityToDeserializeObjectAsBssomValue(true))).Is(BssomValue.Create(value));
            return new BssomFieldMarshaller(buf).ReadValueType(BssomFieldOffsetInfo.Zero);
        }

        public static BssomValueType ConvertArrayObjectAndVerifyBssomValueType<T>(T value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize<object>(value, option);
            BssomSerializer.Size<object>(value, option).Is(buf.Length);
            var ary = (BssomSerializer.Deserialize<object>(buf, (option ?? BssomSerializerOptions.Default).WithIsPriorityToDeserializeObjectAsBssomValue(false)));
            ary.Is(value);
            var ary2 = (BssomArray)(BssomSerializer.Deserialize<object>(buf, (option ?? BssomSerializerOptions.Default).WithIsPriorityToDeserializeObjectAsBssomValue(true)));

            var _ary2 = (BssomArray)BssomValue.Create(value);

            ary2.Count.Is(_ary2.Count);
            for (int i = 0; i < ary2.Count; i++)
            {
                ary2[i].Is(_ary2[i]);
            }
            //ary2.Is((IList<object>)BssomValue.Create(value));
            return new BssomFieldMarshaller(buf).ReadValueType(BssomFieldOffsetInfo.Zero);
        }

        public static BssomValueType ConvertObjectAndVerifyBssomMapType<TKey, TValue>(object value, BssomSerializerOptions option = null)
        {
            if (option == null)
                option = BssomSerializerOptions.Default;

            var buf = BssomSerializer.Serialize<object>(value, option);
            BssomSerializer.Size<object>(value, option).Is(buf.Length);

            var dicObj = ((IEnumerable<KeyValuePair<object, object>>)BssomSerializer.Deserialize<object>(buf, option.WithIsPriorityToDeserializeObjectAsBssomValue(false))).Select(e => new KeyValuePair<TKey, TValue>((TKey)e.Key, (TValue)e.Value));
            dicObj.IsDict((IEnumerable<KeyValuePair<TKey, TValue>>)value);
            var dicObj2 = ((IEnumerable<KeyValuePair<object, object>>)BssomSerializer.Deserialize<object>(buf, option.WithIsPriorityToDeserializeObjectAsBssomValue(true)));
            dicObj2.IsDict((IDictionary<object, object>)BssomValue.Create(value));
            return new BssomFieldMarshaller(buf).ReadValueType(BssomFieldOffsetInfo.Zero);
        }

        public static BssomValueType ConvertAndVerifyBssomValueType<T>(T value, BssomSerializerOptions option = null)
        {
            var buf = SimplicityVerify(value, option);
            return new BssomFieldMarshaller(buf).ReadValueType(BssomFieldOffsetInfo.Zero);
        }

        public static byte[] VerifyEntityWithMap2<T>(T value, BssomSerializerOptions option = null)
        {
            var buf = UseJsonToolForValidation(value, option);
            buf.IsMap2();
            return buf;
        }

        public static void ConvertObjectAndVerifyEntity(object value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            buf.IsMap2();
            var map = (BssomMap)BssomSerializer.Deserialize<object>(buf, option);

            value.GetPublicMembersWithDynamicObject().IsMap(map);
        }

        public static void VerifyMap(BssomMap value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            var map = (BssomMap)BssomSerializer.Deserialize<object>(buf, option);

            value.IsMap(map);
        }

        public static void VerifyArray(BssomArray value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            var ary = (BssomArray)BssomSerializer.Deserialize<object>(buf, option);

            ary.IsArray(value);
        }

        public static void VerifySize(object value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            buf.Length.Is(BssomSerializer.Size(value, option));

            var reader = new BssomReader(new SimpleBufferWriter(buf));
            reader.SkipObject();
            reader.Position.Is(buf.Length);

            var bsfm = new BssomFieldMarshaller(buf);
            bsfm.ReadValueSize(BssomFieldOffsetInfo.Zero).Is(buf.Length);
        }

        public static void IsArray1(this byte[] bytes)
        {
            var reader = new BssomReader(new SimpleBufferWriter(bytes));
            reader.ReadBssomType().Is(BssomType.Array1);
        }

        public static void IsArray2(this byte[] bytes)
        {
            var reader = new BssomReader(new SimpleBufferWriter(bytes));
            reader.ReadBssomType().Is(BssomType.Array2);
        }

        public static void VerifyEntityWithArray1<T>(T value, BssomSerializerOptions option = null)
        {
            var buf = SimplicityVerify(value, option);
            buf.IsArray1();
        }

        public static void VerifyEntityWithArray2<T>(T value, BssomSerializerOptions option = null)
        {
            var buf = SimplicityVerify(value, option);
            buf.IsArray2();
        }

        public static void VerifySpecific<T>(IGrouping<T, T> value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            var group = BssomSerializer.Deserialize<IGrouping<T, T>>(buf, option);
            group.Key.Is(value.Key);
            group.Is(value);
        }

        public static void VerifySpecific<T>(ILookup<T, T> value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            var val = BssomSerializer.Deserialize<ILookup<T, T>>(buf, option);
            val.Count.Is(value.Count);
            foreach (var item in value)
            {
                VerifySpecific(item, option);
            }
        }

        public static void VerifySpecific<T>(Lazy<T> value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            BssomSerializer.Deserialize<Lazy<T>>(buf, option).Value.Is(value.Value);
        }

        public static void VerifySpecific(StringDictionary value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            var dict = new Dictionary<string, string>(BssomSerializer.Deserialize<StringDictionary>(buf, option).ToIEnumerable().ToIDict());
            var dict2 = new Dictionary<string, string>(value.ToIEnumerable().ToIDict());
            dict.IsDict(dict2);
        }

        public static void VerifySpecific(NameValueCollection value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            var r = BssomSerializer.Deserialize<NameValueCollection>(buf, option).ToIEnumerable();
            var dict = new Dictionary<string, string>(r.ToIDict());
            var dict2 = new Dictionary<string, string>(value.ToIEnumerable().ToIDict());
            dict.IsDict(dict2);
        }

        public static void VerifySpecific(DataTable value, BssomSerializerOptions option = null)
        {
            UseJsonToolForValidation(value, option);
        }

        public static void VerifyWithJson<T>(T value,T value2)
        {
            Kooboo.Json.JsonSerializer.ToJson(value).Is(Kooboo.Json.JsonSerializer.ToJson(value2));
        }

        private static byte[] UseJsonToolForValidation<T>(T value, BssomSerializerOptions option = null)
        {
            var buf = BssomSerializer.Serialize(value, option);
            BssomSerializer.Size(value, option).Is(buf.Length);
            var t = BssomSerializer.Deserialize<T>(buf, option);
            Kooboo.Json.JsonSerializer.ToJson(t).Is(Kooboo.Json.JsonSerializer.ToJson(value));
            return buf;
        }

        public static void IsDict<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, IEnumerable<KeyValuePair<TKey, TValue>> dict1)
        {
            IDictionary<TKey, TValue> d1 = new Dictionary<TKey, TValue>(dict.ToIDict());
            var d2 = new Dictionary<TKey, TValue>(dict1.ToIDict());
            Assert.Equal(d1.Count, d2.Count());
            foreach (var item in d2)
            {
                Assert.Contains(item.Key, d1);
                item.Value.Is(d1[item.Key]);
            }
        }

        public static void IsMap(this IEnumerable<KeyValuePair<string, object>> dict, BssomMap map)
        {
            Assert.Equal(dict.Count(), map.Count);
            foreach (var item in dict)
            {
                Assert.Contains(item.Key, map);
                item.Value.Is(map[item.Key]);
            }
        }

        public static void IsMap(this BssomMap dict, BssomMap map)
        {
            Assert.Equal(dict.Count, map.Count);
            foreach (var item in dict)
            {
                Assert.Contains(item.Key, map);
                item.Value.Is(map[item.Key]);
            }
        }

        public static void IsArray(this BssomArray dict, BssomArray map)
        {
            Assert.Equal(dict.Count, map.Count);
            for (int i = 0; i < dict.Count; i++)
            {
                dict[i].Is(map[i]);
            }
        }

        public static void IsNotNull<T>(this T value)
        {
            Assert.NotNull(value);
        }

        public static void IsTrue(this bool value)
        {
            Assert.True(value);
        }

        public static void IsFalse(this bool value)
        {
            Assert.False(value);
        }

        public static void IsType<T>(this object value)
        {
            Assert.IsType<T>(value);
        }

        public static void IsType(this object value, Type t)
        {
            Assert.IsType(t, value);
        }

        //#if NETFRAMEWORK
        public static void IsNull<T>(this T value)
        {
            Assert.Null(value);
        }
        //#endif
        public static void Is(this byte actual, int expected)
        {
            Assert.Equal(actual, expected);
        }

        //http://chainingassertion.codeplex.com/
        public static T Is<T>(this T actual, T expected)
        {
            if (expected == null)
            {
                Assert.Null(actual);
                return actual;
            }

            if (typeof(T) != typeof(string) && typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeof(T)))
            {
                Assert.Equal(
                    ((IEnumerable)expected).Cast<object>().ToArray(),
                    ((IEnumerable)actual).Cast<object>().ToArray());
                return actual;
            }

            Assert.Equal(expected, actual);
            return actual;
        }

        public static Dictionary<TK, TV> ToIDict<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> val)
        {
            Dictionary<TK, TV> dict = new Dictionary<TK, TV>();
            foreach (var item in val)
            {
                dict.Add(item.Key, item.Value);
            }
            return dict;
        }
    }
}
