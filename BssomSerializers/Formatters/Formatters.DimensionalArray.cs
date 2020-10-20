//using System.Runtime.CompilerServices;

using System;

using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
namespace BssomSerializers.Formatters
{
    /// <summary>
    /// Format <see cref="T"/>[] as BssomType.Array2
    /// </summary>
    public sealed class OneDimensionalArrayFormatter<T> : IBssomFormatter<T[]>
    {
        public T[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
                return default;

            var formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();
            reader.SkipVariableNumber();
            var array = new T[reader.ReadVariableNumber()];

            for (int i = 0; i < array.Length; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                array[i] = formatter.Deserialize(ref reader, ref context);
            }

            return array;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, T[] value)
        {
            Array2FormatterHelper.SerializeGenerIList(ref writer,ref context, value);
        }

        public int Size(ref BssomSizeContext context, T[] value)
        {
            return Array2FormatterHelper.SizeGenericIEnumerable<T>(ref context,value);
        }
    }

    /// <summary>
    /// Format <see cref="T"/>[,] as BssomType.Array2
    /// </summary>
    public sealed class TwoDimensionalArrayFormatter<T> : IBssomFormatter<T[,]>
    {
        private const int ArrayLength = 3;

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, T[,] value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var i = value.GetLength(0);
            var j = value.GetLength(1);

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

            writer.WriteArray2Type();
            long pos = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(ArrayLength);

            //first
            writer.Write((byte)i);

            //second
            writer.Write((byte)j);

            //third
            writer.WriteArray2Type();
            long posSecond = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(value.Length);

            foreach (T item in value)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                formatter.Serialize(ref writer, ref context, item);
            }

            writer.WriteBackFixNumber(pos, checked((int)(writer.Position - pos - BssomBinaryPrimitives.FixUInt32NumberSize)));
            writer.WriteBackFixNumber(posSecond, checked((int)(writer.Position - posSecond - BssomBinaryPrimitives.FixUInt32NumberSize)));
        }

        public T[,] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
                return default;

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

            reader.SkipVariableNumber();
            if (reader.ReadVariableNumber() != ArrayLength)
                throw BssomSerializationTypeFormatterException.TypeFormatterError(typeof(T[,]),"Invalid T[,] format");

            var iLength = reader.ReadUInt8();
            var jLength = reader.ReadUInt8();
            reader.SkipBlankCharacterAndEnsureType(BssomType.Array2);
            reader.SkipVariableNumber();//len
            var maxLen = reader.ReadVariableNumber();

            var array = new T[iLength, jLength];

            var i = 0;
            var j = -1;
            context.Option.Security.DepthStep(ref context);

            for (int loop = 0; loop < maxLen; loop++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                if (j < jLength - 1)
                {
                    j++;
                }
                else
                {
                    j = 0;
                    i++;
                }

                array[i, j] = formatter.Deserialize(ref reader, ref context);
            }

            context.Depth--;
            return array;
        }

        public int Size(ref BssomSizeContext context, T[,] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            var formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

            long dataLen = 0;
            foreach (T item in value)
            {
                dataLen += formatter.Size(ref context, item);
            }
            dataLen = BssomBinaryPrimitives.Array2TypeSize(value.Length, dataLen);
            dataLen += (BssomBinaryPrimitives.UInt8Size + BssomBinaryPrimitives.BuildInTypeCodeSize) * (ArrayLength - 1);

            return BssomBinaryPrimitives.Array2TypeSize(ArrayLength, dataLen);
        }
    }

    /// <summary>
    /// Format <see cref="T"/>[,,] as BssomType.Array2
    /// </summary>
    public sealed class ThreeDimensionalArrayFormatter<T> : IBssomFormatter<T[,,]>
    {
        private const int ArrayLength = 4;

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, T[,,] value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var i = value.GetLength(0);
            var j = value.GetLength(1);
            var k = value.GetLength(2);

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

            writer.WriteArray2Type();
            long pos = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(ArrayLength);
            //first
            writer.Write((byte)i);
            //second
            writer.Write((byte)j);
            //third
            writer.Write((byte)k);
            //fourth
            writer.WriteArray2Type();
            long posSecond = writer.FillUInt32FixNumber();
            writer.WriteVariableNumber(value.Length);

            foreach (T item in value)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                formatter.Serialize(ref writer, ref context, item);
            }

            writer.WriteBackFixNumber(pos, checked((int)(writer.Position - posSecond + BssomBinaryPrimitives.FixUInt32NumberSize)));
            writer.WriteBackFixNumber(pos, checked((int)(writer.Position - pos + BssomBinaryPrimitives.FixUInt32NumberSize)));
        }

        public T[,,] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
                return default;

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();
            reader.SkipVariableNumber();
            if (reader.ReadVariableNumber() != ArrayLength)
            {
                throw BssomSerializationTypeFormatterException.TypeFormatterError(typeof(T[,,]), "Invalid T[,,] format");
            }

            var iLength = reader.ReadUInt8();
            var jLength = reader.ReadUInt8();
            var kLength = reader.ReadUInt8();
            reader.SkipBlankCharacterAndEnsureType(BssomType.Array2);
            reader.SkipVariableNumber();
            var maxLen = reader.ReadVariableNumber();

            var array = new T[iLength, jLength, kLength];

            var i = 0;
            var j = 0;
            var k = -1;
            context.Option.Security.DepthStep(ref context);

            for (int loop = 0; loop < maxLen; loop++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                if (k < kLength - 1)
                {
                    k++;
                }
                else if (j < jLength - 1)
                {
                    k = 0;
                    j++;
                }
                else
                {
                    k = 0;
                    j = 0;
                    i++;
                }

                array[i, j, k] = formatter.Deserialize(ref reader, ref context);
            }

            context.Depth--;
            return array;
        }

        public int Size(ref BssomSizeContext context, T[,,] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            var formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

            long dataLen = 0;
            foreach (T item in value)
            {
                dataLen += formatter.Size(ref context, item);
            }
            dataLen = BssomBinaryPrimitives.Array2TypeSize(value.Length, dataLen);
            dataLen += (BssomBinaryPrimitives.UInt8Size + BssomBinaryPrimitives.BuildInTypeCodeSize) * (ArrayLength - 1);

            return BssomBinaryPrimitives.Array2TypeSize(ArrayLength, dataLen);
        }
    }

    /// <summary>
    /// Format <see cref="T"/>[,,,] as BssomType.Array2
    /// </summary>
    public sealed class FourDimensionalArrayFormatter<T> : IBssomFormatter<T[,,,]>
    {
        private const int ArrayLength = 5;

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, T[,,,] value)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var i = value.GetLength(0);
                var j = value.GetLength(1);
                var k = value.GetLength(2);
                var l = value.GetLength(3);

                IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

                writer.WriteArray2Type();
                long pos = writer.FillUInt32FixNumber();
                writer.WriteVariableNumber(ArrayLength);
                writer.Write((byte)i);
                writer.Write((byte)j);
                writer.Write((byte)k);
                writer.Write((byte)l);

                writer.WriteArray2Type();
                long posSecond = writer.FillUInt32FixNumber();
                writer.WriteVariableNumber(value.Length);
                foreach (T item in value)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    formatter.Serialize(ref writer, ref context, item);
                }

                writer.WriteBackFixNumber(pos, checked((int)(writer.Position - posSecond + BssomBinaryPrimitives.FixUInt32NumberSize)));
                writer.WriteBackFixNumber(pos, checked((int)(writer.Position - pos + BssomBinaryPrimitives.FixUInt32NumberSize)));

            }
        }

        public T[,,,] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
                return default;

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();
            reader.SkipVariableNumber();
            if (reader.ReadVariableNumber() != ArrayLength)
            {
                throw BssomSerializationTypeFormatterException.TypeFormatterError(typeof(T[,,,]), "Invalid T[,,,] format");
            }

            var iLength = reader.ReadUInt8();
            var jLength = reader.ReadUInt8();
            var kLength = reader.ReadUInt8();
            var lLength = reader.ReadUInt8();
            reader.SkipBlankCharacterAndEnsureType(BssomType.Array2);
            reader.SkipVariableNumber();
            var maxLen = reader.ReadVariableNumber();
            var array = new T[iLength, jLength, kLength, lLength];

            var i = 0;
            var j = 0;
            var k = 0;
            var l = -1;
            context.Option.Security.DepthStep(ref context);

            for (int loop = 0; loop < maxLen; loop++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                if (l < lLength - 1)
                {
                    l++;
                }
                else if (k < kLength - 1)
                {
                    l = 0;
                    k++;
                }
                else if (j < jLength - 1)
                {
                    l = 0;
                    k = 0;
                    j++;
                }
                else
                {
                    l = 0;
                    k = 0;
                    j = 0;
                    i++;
                }

                array[i, j, k, l] = formatter.Deserialize(ref reader, ref context);
            }

            context.Depth--;
            return array;
        }

        public int Size(ref BssomSizeContext context, T[,,,] value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            var formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

            long dataLen = 0;
            foreach (T item in value)
            {
                dataLen += formatter.Size(ref context, item);
            }
            dataLen = BssomBinaryPrimitives.Array2TypeSize(value.Length, dataLen);
            dataLen += (BssomBinaryPrimitives.UInt8Size + BssomBinaryPrimitives.BuildInTypeCodeSize) * (ArrayLength - 1);

            return BssomBinaryPrimitives.Array2TypeSize(ArrayLength, dataLen);
        }
    }
}
