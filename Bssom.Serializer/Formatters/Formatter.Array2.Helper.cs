//using System.Runtime.CompilerServices;

using Bssom.Serializer.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace Bssom.Serializer.Internal
{
    //Array2
    internal static class Array2FormatterHelper
    {
        //Serialize IList<> / IReadOnlyList<>
        public static void SerializeGenerIList<TElement>(ref BssomWriter writer, ref BssomSerializeContext context,
            IEnumerable<TElement> value)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                DEBUG.Assert(value is IList<TElement> || value is IReadOnlyList<TElement>);
                IBssomFormatter<TElement> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TElement>();

                writer.WriteArray2Type();
                long pos = writer.FillUInt32FixNumber();
                if (value is IList<TElement> list)
                {
                    writer.WriteVariableNumber(list.Count);
                    for (int i = 0; i < list.Count; i++)
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();
                        formatter.Serialize(ref writer, ref context, list[i]);
                    }
                }
                else
                {
                    IReadOnlyList<TElement> rels = Unsafe.As<IEnumerable<TElement>, IReadOnlyList<TElement>>(ref value);
                    writer.WriteVariableNumber(rels.Count);
                    for (int i = 0; i < rels.Count; i++)
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();
                        formatter.Serialize(ref writer, ref context, rels[i]);
                    }
                }
                writer.WriteBackFixNumber(pos, checked((int)(writer.Position - pos - BssomBinaryPrimitives.FixUInt32NumberSize)));
            }
        }

        //Serialize IList
        public static void SerializeIList(ref BssomWriter writer, ref BssomSerializeContext context, IList value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            IBssomFormatter<object> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<object>();

            writer.WriteArray2Type();
            long pos = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                formatter.Serialize(ref writer, ref context, value[i]);
            }
            writer.WriteBackFixNumber(pos, checked((int)(writer.Position - pos - BssomBinaryPrimitives.FixUInt32NumberSize)));
        }

        //Serialize IEnumerable<>
        public static void SerializeGenericIEnumerable<TElement>(ref BssomWriter writer,
            ref BssomSerializeContext context, IEnumerable<TElement> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            IBssomFormatter<TElement> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TElement>();

            writer.WriteArray2Type();
            long pos = writer.FillUInt32FixNumber();
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(count);
                foreach (TElement item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    formatter.Serialize(ref writer, ref context, item);
                }
                writer.WriteBackFixNumber(pos, checked((int)(writer.Position - pos - BssomBinaryPrimitives.FixUInt32NumberSize)));
            }
            else
            {
                count = 0;
                long posCount = writer.FillUInt32FixNumber();
                foreach (TElement item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    formatter.Serialize(ref writer, ref context, item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(pos);
                writer.WriteBackFixNumber(checked((int)(writer.Position - posCount)));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        //Serialize IEnumerable
        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context,
            IEnumerable value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            IBssomFormatter<object> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<object>();

            writer.WriteArray2Type();
            long pos = writer.FillUInt32FixNumber();
            if (value is ICollection coll)
            {
                writer.WriteVariableNumber(coll.Count);
                foreach (object item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    formatter.Serialize(ref writer, ref context, item);
                }
                writer.WriteBackFixNumber(pos, checked((int)(writer.Position - pos - BssomBinaryPrimitives.FixUInt32NumberSize)));
            }
            else
            {
                int count = 0;
                long posCount = writer.FillUInt32FixNumber();
                foreach (object item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    formatter.Serialize(ref writer, ref context, item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(pos);
                writer.WriteBackFixNumber(checked((int)(writer.Position - posCount)));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        //Size IEnumerable
        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            IBssomFormatter<object> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<object>();

            long dataLen = 0;
            foreach (object item in value)
            {
                dataLen += formatter.Size(ref context, item);
            }

            if (value is ICollection coll)
            {
                return BssomBinaryPrimitives.Array2TypeSize(coll.Count, dataLen);
            }
            else
            {
                return BssomBinaryPrimitives.Array2TypeSizeWithFixU32Count(dataLen);
            }
        }

        //Size IEnumerable<>
        public static int SizeGenericIEnumerable<TElement>(ref BssomSizeContext context, IEnumerable<TElement> value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            IBssomFormatter<TElement> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TElement>();
            long dataLen = 0;
            foreach (TElement item in value)
            {
                dataLen += formatter.Size(ref context, item);
            }

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array2TypeSize(count, dataLen);
            }
            else
            {
                return BssomBinaryPrimitives.Array2TypeSizeWithFixU32Count(dataLen);
            }
        }

        //Deserialize :  IEnumerable ICollection IList,IEnumerable<> IList<> ICollection<> IReadOnlyList<> IReadOnlyCollection<>
        public static List<TElement> DeserializeList<TElement>(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
            {
                return default;
            }

            IBssomFormatter<TElement> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TElement>();

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            List<TElement> list = new List<TElement>(count);
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                list.Add(formatter.Deserialize(ref reader, ref context));
            }
            return list;
        }

        //Deserialize :  ISet<>
        public static HashSet<TElement> DeserializeSet<TElement>(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
            {
                return default;
            }

            IBssomFormatter<TElement> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TElement>();

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<TElement> hash = new HashSet<TElement>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(formatter.Deserialize(ref reader, ref context));
            }
            return hash;
        }

        public static void Fill_ImplICollection<T, TElement>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<TElement>
        {
            IBssomFormatter<TElement> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<TElement>();
            ICollection<TElement> coll = (ICollection<TElement>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(formatter.Deserialize(ref reader, ref context));
            }
        }

        public static void Fill_ImplIList<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : IList
        {
            IBssomFormatter<object> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<object>();
            IList coll = (IList)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(formatter.Deserialize(ref reader, ref context));
            }
        }
    }
}
