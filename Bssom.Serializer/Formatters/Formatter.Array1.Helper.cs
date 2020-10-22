
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
namespace Bssom.Serializer.Internal
{
    internal static partial class Array1FormatterHelper
    {
        public readonly static string DeserializeSetPrefix = "DeserializeSet";
        public readonly static string FillPrefix = "Fill";

        #region Serialize/Size IEnumerable<SByte> to Array1 . Deserialize to HashSet<SByte> /Fill to class:IColloction<SByte>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<SByte> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int8Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int8Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<SByte> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int8Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.Int8Size, count);
        }

        public static HashSet<SByte> DeserializeSetInt8(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int8Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<SByte> hash = new HashSet<SByte>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadInt8WithOutTypeHead());
            }
            return hash;
        }

        public static void FillInt8<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<SByte>
        {
            var coll = (ICollection<SByte>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadInt8WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Int16> to Array1 . Deserialize to HashSet<Int16> /Fill to class:IColloction<Int16>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Int16> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int16Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int16Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Int16> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int16Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.Int16Size, count);
        }

        public static HashSet<Int16> DeserializeSetInt16(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int16Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Int16> hash = new HashSet<Int16>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadInt16WithOutTypeHead());
            }
            return hash;
        }

        public static void FillInt16<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Int16>
        {
            var coll = (ICollection<Int16>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadInt16WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Int32> to Array1 . Deserialize to HashSet<Int32> /Fill to class:IColloction<Int32>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Int32> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int32Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int32Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Int32> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int32Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.Int32Size, count);
        }

        public static HashSet<Int32> DeserializeSetInt32(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int32Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Int32> hash = new HashSet<Int32>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadInt32WithOutTypeHead());
            }
            return hash;
        }

        public static void FillInt32<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Int32>
        {
            var coll = (ICollection<Int32>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadInt32WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Int64> to Array1 . Deserialize to HashSet<Int64> /Fill to class:IColloction<Int64>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Int64> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Int64Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Int64Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Int64> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Int64Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.Int64Size, count);
        }

        public static HashSet<Int64> DeserializeSetInt64(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int64Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Int64> hash = new HashSet<Int64>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadInt64WithOutTypeHead());
            }
            return hash;
        }

        public static void FillInt64<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Int64>
        {
            var coll = (ICollection<Int64>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadInt64WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Byte> to Array1 . Deserialize to HashSet<Byte> /Fill to class:IColloction<Byte>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Byte> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt8Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt8Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Byte> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt8Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.UInt8Size, count);
        }

        public static HashSet<Byte> DeserializeSetUInt8(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt8Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Byte> hash = new HashSet<Byte>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadUInt8WithOutTypeHead());
            }
            return hash;
        }

        public static void FillUInt8<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Byte>
        {
            var coll = (ICollection<Byte>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadUInt8WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<UInt16> to Array1 . Deserialize to HashSet<UInt16> /Fill to class:IColloction<UInt16>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<UInt16> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt16Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt16Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<UInt16> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt16Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.UInt16Size, count);
        }

        public static HashSet<UInt16> DeserializeSetUInt16(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt16Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<UInt16> hash = new HashSet<UInt16>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadUInt16WithOutTypeHead());
            }
            return hash;
        }

        public static void FillUInt16<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<UInt16>
        {
            var coll = (ICollection<UInt16>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadUInt16WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<UInt32> to Array1 . Deserialize to HashSet<UInt32> /Fill to class:IColloction<UInt32>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<UInt32> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt32Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt32Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<UInt32> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt32Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.UInt32Size, count);
        }

        public static HashSet<UInt32> DeserializeSetUInt32(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt32Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<UInt32> hash = new HashSet<UInt32>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadUInt32WithOutTypeHead());
            }
            return hash;
        }

        public static void FillUInt32<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<UInt32>
        {
            var coll = (ICollection<UInt32>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadUInt32WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<UInt64> to Array1 . Deserialize to HashSet<UInt64> /Fill to class:IColloction<UInt64>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<UInt64> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.UInt64Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.UInt64Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<UInt64> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.UInt64Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.UInt64Size, count);
        }

        public static HashSet<UInt64> DeserializeSetUInt64(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.UInt64Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<UInt64> hash = new HashSet<UInt64>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadUInt64WithOutTypeHead());
            }
            return hash;
        }

        public static void FillUInt64<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<UInt64>
        {
            var coll = (ICollection<UInt64>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadUInt64WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Single> to Array1 . Deserialize to HashSet<Single> /Fill to class:IColloction<Single>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Single> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Float32Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Float32Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Single> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Float32Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.Float32Size, count);
        }

        public static HashSet<Single> DeserializeSetFloat32(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Float32Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Single> hash = new HashSet<Single>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadFloat32WithOutTypeHead());
            }
            return hash;
        }

        public static void FillFloat32<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Single>
        {
            var coll = (ICollection<Single>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadFloat32WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Double> to Array1 . Deserialize to HashSet<Double> /Fill to class:IColloction<Double>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Double> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.Float64Code);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.Float64Size, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Double> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.Float64Size, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.Float64Size, count);
        }

        public static HashSet<Double> DeserializeSetFloat64(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Float64Code))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Double> hash = new HashSet<Double>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadFloat64WithOutTypeHead());
            }
            return hash;
        }

        public static void FillFloat64<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Double>
        {
            var coll = (ICollection<Double>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadFloat64WithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Boolean> to Array1 . Deserialize to HashSet<Boolean> /Fill to class:IColloction<Boolean>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Boolean> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1BuildInType(BssomType.BooleanCode);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.BooleanSize, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Boolean> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1BuildInTypeSize(BssomBinaryPrimitives.BooleanSize, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1BuildInTypeWithNeedFillCount(BssomBinaryPrimitives.BooleanSize, count);
        }

        public static HashSet<Boolean> DeserializeSetBoolean(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.BooleanCode))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Boolean> hash = new HashSet<Boolean>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadBooleanWithOutTypeHead());
            }
            return hash;
        }

        public static void FillBoolean<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Boolean>
        {
            var coll = (ICollection<Boolean>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadBooleanWithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Char> to Array1 . Deserialize to HashSet<Char> /Fill to class:IColloction<Char>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Char> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.CharCode);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.CharSize, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Char> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.CharSize, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1NativeTypeWithNeedFillCount(BssomBinaryPrimitives.CharSize, count);
        }

        public static HashSet<Char> DeserializeSetChar(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1NativeType(NativeBssomType.CharCode))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Char> hash = new HashSet<Char>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadCharWithOutTypeHead());
            }
            return hash;
        }

        public static void FillChar<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Char>
        {
            var coll = (ICollection<Char>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadCharWithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Decimal> to Array1 . Deserialize to HashSet<Decimal> /Fill to class:IColloction<Decimal>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Decimal> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.DecimalCode);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.DecimalSize, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Decimal> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.DecimalSize, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1NativeTypeWithNeedFillCount(BssomBinaryPrimitives.DecimalSize, count);
        }

        public static HashSet<Decimal> DeserializeSetDecimal(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1NativeType(NativeBssomType.DecimalCode))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Decimal> hash = new HashSet<Decimal>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadDecimalWithOutTypeHead());
            }
            return hash;
        }

        public static void FillDecimal<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Decimal>
        {
            var coll = (ICollection<Decimal>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadDecimalWithOutTypeHead());
            }
        }

        #endregion

        #region Serialize/Size IEnumerable<Guid> to Array1 . Deserialize to HashSet<Guid> /Fill to class:IColloction<Guid>

        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<Guid> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteArray1NativeType(NativeBssomType.GuidCode);
            if (value.TryGetICollectionCount(out int count))
            {
                writer.WriteVariableNumber(BssomBinaryPrimitives.Array1TypeSizeWithOutTypeHeadAndLength(BssomBinaryPrimitives.GuidSize, count));
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.WriteWithOutTypeHead(item);
                }
            }
            else
            {
                count = 0;
                long posLen = writer.FillUInt32FixNumber();//len
                long posCount = writer.FillUInt32FixNumber();//count
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.WriteWithOutTypeHead(item);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<Guid> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                return BssomBinaryPrimitives.Array1NativeTypeSize(BssomBinaryPrimitives.GuidSize, count);
            }
            count = value.Count();
            return BssomBinaryPrimitives.Array1NativeTypeWithNeedFillCount(BssomBinaryPrimitives.GuidSize, count);
        }

        public static HashSet<Guid> DeserializeSetGuid(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureArray1NativeType(NativeBssomType.GuidCode))
                return default;

            reader.SkipVariableNumber();
            int count = reader.ReadVariableNumber();
            HashSet<Guid> hash = new HashSet<Guid>();
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                hash.Add(reader.ReadGuidWithOutTypeHead());
            }
            return hash;
        }

        public static void FillGuid<T>(ref T t, ref BssomReader reader, ref BssomDeserializeContext context, int count) where T : ICollection<Guid>
        {
            var coll = (ICollection<Guid>)t;
            for (int i = 0; i < count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                coll.Add(reader.ReadGuidWithOutTypeHead());
            }
        }

        #endregion

        #region DateTime
        public static void SerializeIEnumerable(ref BssomWriter writer, ref BssomSerializeContext context, IEnumerable<DateTime> value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            if (value.TryGetICollectionCount(out int count))
            {
                if (context.Option.IsUseStandardDateTime)
                    writer.WriteArray1BuildInType(BssomType.DateTimeCode);
                else
                    writer.WriteArray1NativeType(NativeBssomType.DateTimeCode);

                long posLen = writer.FillUInt32FixNumber();//len
                writer.WriteVariableNumber(count);
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    writer.Write(item, context.Option.IsUseStandardDateTime, false);
                }

                writer.WriteBackFixNumber(posLen, checked((int)(writer.Position - posLen - BssomBinaryPrimitives.FixUInt32NumberSize)));
            }
            else
            {
                if (context.Option.IsUseStandardDateTime)
                    writer.WriteArray1BuildInType(BssomType.DateTimeCode);
                else
                    writer.WriteArray1NativeType(NativeBssomType.DateTimeCode);
                count = 0;
                long posLen = writer.FillUInt32FixNumber();
                long posCount = writer.FillUInt32FixNumber();
                foreach (var item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    count++;
                    writer.Write(item, context.Option.IsUseStandardDateTime, false);
                }
                long cPos = writer.Position;
                writer.BufferWriter.Seek(posLen);
                writer.WriteBackFixNumber((int)(cPos - posCount));
                writer.WriteBackFixNumber(count);
                writer.BufferWriter.Seek(cPos);
            }
        }

        public static int SizeIEnumerable(ref BssomSizeContext context, IEnumerable<DateTime> value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            if (value.TryGetICollectionCount(out int count))
            {
                if (context.Option.IsUseStandardDateTime)
                {
                    return BssomBinaryPrimitives.StandardDateTimeArraySize(count);
                }
                else
                {
                    return BssomBinaryPrimitives.NativeDateTimeArraySize(count);
                }
            }
            else
            {
                if (context.Option.IsUseStandardDateTime)
                {
                    return BssomBinaryPrimitives.StandardDateTimeArraySizeWithOutCount(value.Count());
                }
                else
                {
                    return BssomBinaryPrimitives.NativeDateTimeArraySizeWithOutCount(value.Count());
                }
            }
        }

        public static HashSet<DateTime> DeserializeSetDateTime(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array1))
                return default;

            HashSet<DateTime> hash;
            int count;
            byte type = reader.ReadBssomType();
            switch (type)
            {
                case BssomType.DateTimeCode:
                    reader.SkipVariableNumber();
                    count = reader.ReadVariableNumber();
                    hash = new HashSet<DateTime>();
                    for (int i = 0; i < count; i++)
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();
                        hash.Add(reader.ReadStandDateTimeWithOutTypeHead());
                    }
                    break;
                case BssomType.NativeCode:
                    reader.EnsureType(NativeBssomType.DateTimeCode);
                    reader.SkipVariableNumber();
                    count = reader.ReadVariableNumber();
                    hash = new HashSet<DateTime>();
                    for (int i = 0; i < count; i++)
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();
                        hash.Add(reader.ReadNativeDateTimeWithOutTypeHead());
                    }
                    break;
                default:
                    throw BssomSerializationOperationException.UnexpectedCodeRead(type, reader.Position);
            }

            return hash;
        }
        #endregion
    }
}