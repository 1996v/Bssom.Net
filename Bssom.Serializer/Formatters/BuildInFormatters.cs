
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.BssomBuffer;

namespace Bssom.Serializer.Formatters
{
    /// <summary>
    /// Format <see cref="Version"/> as BssomType.String
    /// </summary>
    public sealed class VersionFormatter : IBssomFormatter<Version>
    {
        public static readonly IBssomFormatter<Version> Instance = new VersionFormatter();

        private VersionFormatter()
        {
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Version value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.ToString());
        }

        public Version Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            var val = reader.ReadString();
            if (val == null)
                return default;
            return new Version(val);
        }

        public int Size(ref BssomSizeContext context, Version value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.StringSize(value.ToString()) + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }
    }

    /// <summary>
    /// Format <see cref="Uri"/> as BssomType.String
    /// </summary>
    public sealed class UriFormatter : IBssomFormatter<Uri>
    {
        public static readonly UriFormatter Instance = new UriFormatter();

        private UriFormatter()
        {
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, Uri value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.Write(value.ToString());
        }

        public Uri Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            var val = reader.ReadString();
            if (val == null)
                return default;
            return new Uri(val, UriKind.RelativeOrAbsolute);
        }

        public int Size(ref BssomSizeContext context, Uri value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.StringSize(value.ToString()) + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }
    }

    /// <summary>
    /// Format <see cref="TimeSpan"/> as BssomType.Int64
    /// </summary>
    public sealed class TimeSpanFormatter : IBssomFormatter<TimeSpan>
    {
        public static readonly TimeSpanFormatter Instance = new TimeSpanFormatter();

        private TimeSpanFormatter()
        {
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, TimeSpan value)
        {
            writer.Write(value.Ticks);
            return;
        }

        public TimeSpan Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return new TimeSpan(reader.ReadInt64());
        }

        public int Size(ref BssomSizeContext context, TimeSpan value)
        {
            return BssomBinaryPrimitives.Int64Size + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }
    }

    /// <summary>
    /// Format <see cref="DBNull"/> as BssomType.Null
    /// </summary>
    public sealed class DBNullFormatter : IBssomFormatter<DBNull>
    {
        public static readonly DBNullFormatter Instance = new DBNullFormatter();

        private DBNullFormatter()
        {
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, DBNull value)
        {
            writer.WriteNull();
        }

        public DBNull Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            reader.SkipBlankCharacterAndEnsureType(BssomType.NullCode);
            return DBNull.Value;
        }

        public int Size(ref BssomSizeContext context, DBNull value)
        {
            return BssomBinaryPrimitives.NullSize;
        }
    }

    /// <summary>
    /// Format <see cref="DataTable"/> as BssomType.Array2
    /// </summary>
    public sealed class DataTableFormatter : IBssomFormatter<DataTable>
    {
        public static readonly DataTableFormatter Instance = new DataTableFormatter();

        private DataTableFormatter()
        {
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, DataTable value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteArray2Type();
            writer.WriteVariableNumber(value.Rows.Count);
            foreach (DataRow row in value.Rows)
            {
                MapFormatterHelper.Serialize(ref writer, ref context, row.ToIEnumerable(), row.Table.Columns.Count);
            }
        }

        public DataTable Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
                return default;

            int len = reader.ReadVariableNumber();
            DataTable dt = new DataTable();

            for (int i = 0; i < len; i++)
            {
                DataRow dr = dt.NewRow();
                var map = MapFormatterHelper.Deserialize<string, object>(ref reader, ref context);
                foreach (var item in map)
                {
                    DataColumn column = dt.Columns[item.Key];
                    if (column == null)
                    {
                        column = new DataColumn(item.Key, typeof(object));
                        dt.Columns.Add(column);
                    }

                    dr[item.Key] = item.Value;
                }
                dt.Rows.Add(dr);

                reader = map.Reader;
                context = map.Context;
                reader.BssomBuffer.Seek(map.EndPosition);
            }
            return dt;

        }

        public int Size(ref BssomSizeContext context, DataTable value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            return BssomBinaryPrimitives.Array2TypeCodeSize + BssomBinaryPrimitives.VariableNumberSize((ulong)value.Rows.Count) + GetDataRowCollectionSize(ref context, value.Rows);
        }

        static int GetDataRowCollectionSize(ref BssomSizeContext context, DataRowCollection dataRow)
        {
            int len = 0;
            foreach (DataRow row in dataRow)
            {
                len += MapFormatterHelper.Size(ref context, row.ToIEnumerable(), row.Table.Columns.Count);
            }
            return len;
        }


    }

    /// <summary>
    /// Format <see cref="NameValueCollection"/> as BssomType.Map1 or BssomType.Map2
    /// </summary>
    public sealed class NameValueCollectionFormatter : IBssomFormatter<NameValueCollection>
    {
        public static readonly NameValueCollectionFormatter Instance = new NameValueCollectionFormatter();

        private NameValueCollectionFormatter()
        {
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, NameValueCollection value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            MapFormatterHelper.Serialize(ref writer, ref context, value.ToIEnumerable(), value.Count);
        }

        public NameValueCollection Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            var map = MapFormatterHelper.Deserialize<string, string>(ref reader, ref context);
            if (map == null)
                return default;
            NameValueCollection val = new NameValueCollection(map.Count);
            foreach (var item in map)
            {
                val.Add(item.Key, item.Value);
            }
            reader = map.Reader;
            context = map.Context;
            reader.BssomBuffer.Seek(map.EndPosition);
            return val;
        }

        public int Size(ref BssomSizeContext context, NameValueCollection value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return MapFormatterHelper.Size(ref context, value.ToIEnumerable(), value.Count);
        }
    }

    /// <summary>
    /// Format <see cref="BitArray"/> as BssomType.Array1 
    /// </summary>
    public sealed class BitArrayFormatter : IBssomFormatter<BitArray>
    {
        public static readonly BitArrayFormatter Instance = new BitArrayFormatter();

        private BitArrayFormatter()
        {
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext options, BitArray value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteArray1BuildInType(BssomType.BooleanCode);
            writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.BooleanSize, value.Length));
            writer.WriteVariableNumber(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteWithOutTypeHead(value.Get(i));
            }
        }

        public BitArray Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.BooleanCode))
                return default;

            reader.SkipVariableNumber();
            var array = new BitArray(reader.ReadVariableNumber());
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadBooleanWithOutTypeHead();
            }

            return array;
        }

        public int Size(ref BssomSizeContext context, BitArray value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.BooleanSize, value.Length);
        }
    }

    /// <summary>
    /// Format <see cref="StringBuilder"/> as BssomType.String
    /// </summary>
    public sealed class StringBuilderFormatter : IBssomFormatter<StringBuilder>
    {
        public static readonly StringBuilderFormatter Instance = new StringBuilderFormatter();

        private StringBuilderFormatter()
        {
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext options, StringBuilder value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.Write(value.ToString());
        }

        public StringBuilder Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            var val = reader.ReadString();
            if (val == null)
                return default;

            return new StringBuilder(val);
        }

        public int Size(ref BssomSizeContext context, StringBuilder value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.StringSize(value.ToString()) + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }
    }

    /// <summary>
    /// Format <see cref="StringDictionary"/> as BssomType.Map1 or BssomType.Map2
    /// </summary>
    public sealed class StringDictionaryFormatter : IBssomFormatter<StringDictionary>
    {
        public static readonly StringDictionaryFormatter Instance = new StringDictionaryFormatter();

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, StringDictionary value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            MapFormatterHelper.Serialize(ref writer, ref context, value.ToIEnumerable(), value.Count);
        }



        public StringDictionary Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            var map = MapFormatterHelper.Deserialize<string, string>(ref reader, ref context);
            if (map == null)
                return null;

            StringDictionary dict = new StringDictionary();
            foreach (var item in map)
            {
                dict.Add(item.Key, item.Value);
            }

            reader = map.Reader;
            context = map.Context;
            reader.BssomBuffer.Seek(map.EndPosition);
            return dict;
        }

        public int Size(ref BssomSizeContext context, StringDictionary value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return MapFormatterHelper.Size(ref context, value.ToIEnumerable(), value.Count);
        }
    }

    /// <summary>
    /// Format <see cref="ILookup{TKey,TElement}"/> as BssomType.Array2
    /// </summary>
    public sealed class InterfaceILookupFormatter<TKey, TElement> : IBssomFormatter<ILookup<TKey, TElement>>
    {
        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, ILookup<TKey, TElement> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            context.Option.FormatterResolver.GetFormatterWithVerify<IEnumerable<IGrouping<TKey, TElement>>>().Serialize(ref writer, ref context, value);
        }

        public ILookup<TKey, TElement> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            var intermediateCollection = context.Option.FormatterResolver.GetFormatterWithVerify<IEnumerable<IGrouping<TKey, TElement>>>().Deserialize(ref reader, ref context);
            if (intermediateCollection == null)
                return null;
            return new Internal.Lookup<TKey, TElement>(intermediateCollection);
        }

        public int Size(ref BssomSizeContext context, ILookup<TKey, TElement> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return context.Option.FormatterResolver.GetFormatterWithVerify<IEnumerable<IGrouping<TKey, TElement>>>().Size(ref context, value);
        }
    }

    /// <summary>
    /// Format <see cref="IGrouping{TKey,TElement}"/> as BssomType.Array2
    /// </summary>
    public sealed class InterfaceGroupingFormatter<TKey, TElement> : IBssomFormatter<IGrouping<TKey, TElement>>
    {
        private const int Length = 2;
        private static int LengthSize = BssomBinaryPrimitives.VariableNumberSize((ulong)Length);

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context,
            IGrouping<TKey, TElement> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteArray2Type();
            long pos = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(Length);
            context.Option.FormatterResolver.GetFormatterWithVerify<TKey>().Serialize(ref writer, ref context, value.Key);
            context.Option.FormatterResolver.GetFormatterWithVerify<IEnumerable<TElement>>().Serialize(ref writer, ref context, value);
            writer.WriteBackFixNumber(pos, checked((int)(writer.Position - pos - LengthSize)));
        }

        public IGrouping<TKey, TElement> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
            {
                return null;
            }
            reader.SkipVariableNumber();
            var count = reader.ReadVariableNumber();
            if (count != Length)
            {
                throw BssomSerializationTypeFormatterException.TypeFormatterError(typeof(IGrouping<TKey, TElement>), "Invalid format");
            }

            context.Option.Security.DepthStep(ref context);
            TKey key = context.Option.FormatterResolver.GetFormatterWithVerify<TKey>().Deserialize(ref reader, ref context);
            IEnumerable<TElement> value = context.Option.FormatterResolver.GetFormatterWithVerify<IEnumerable<TElement>>().Deserialize(ref reader, ref context);
            context.Depth--;
            return new Grouping<TKey, TElement>(key, value);
        }

        public int Size(ref BssomSizeContext context, IGrouping<TKey, TElement> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.Array2TypeSize(LengthSize,
                  context.Option.FormatterResolver.GetFormatterWithVerify<TKey>().Size(ref context, value.Key)
                  + context.Option.FormatterResolver.GetFormatterWithVerify<IEnumerable<TElement>>().Size(ref context, value));
        }
    }

    /// <summary>
    /// Format <see cref="Lazy{T}"/> as BssomType.Null or BssomValue
    /// </summary>
    public sealed class LazyFormatter<T> : IBssomFormatter<Lazy<T>>
    {
        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext options, Lazy<T> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            IBssomFormatter<T> formatter = options.Option.FormatterResolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, ref options, value.Value);
        }

        public Lazy<T> Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNull())
            {
                return null;
            }

            context.Option.Security.DepthStep(ref context);
            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();
            T v = formatter.Deserialize(ref reader, ref context);
            context.Depth--;
            return new Lazy<T>(() => v);
        }

        public int Size(ref BssomSizeContext context, Lazy<T> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return context.Option.FormatterResolver.GetFormatterWithVerify<T>().Size(ref context, value.Value);
        }
    }

    internal sealed class AnonymousTypeFormatter<T> : IBssomFormatter<T>
    {
        public static readonly AnonymousTypeFormatter<T> Instance = new AnonymousTypeFormatter<T>();

        public T Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            throw BssomSerializationTypeFormatterException.TypeFormatterError(typeof(T), "You need to deserialize the type to Object");
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, T value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var properties = typeof(T).GetProperties();
            MapFormatterHelper.Serialize(ref writer, ref context, properties.Select(e => new KeyValuePair<string, object>(e.Name, e.GetValue(value))), properties.Length);
        }

        public int Size(ref BssomSizeContext context, T value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;
            var properties = typeof(T).GetProperties();
            return MapFormatterHelper.Size(ref context, properties.Select(e => new KeyValuePair<string, object>(e.Name, e.GetValue(value))), properties.Length);
        }
    }
}
