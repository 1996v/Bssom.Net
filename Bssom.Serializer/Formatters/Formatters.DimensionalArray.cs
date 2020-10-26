//using System.Runtime.CompilerServices;

using Bssom.Serializer.Binary;
using Bssom.Serializer.Internal;
namespace Bssom.Serializer.Formatters
{
    /// <summary>
    /// Format <see cref="T"/>[] as BssomType.Array2
    /// </summary>
    public sealed class OneDimensionalArrayFormatter<T> : IBssomFormatter<T[]>
    {
        public T[] Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
            {
                return default;
            }

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();
            reader.SkipVariableNumber();
            T[] array = new T[reader.ReadVariableNumber()];

            for (int i = 0; i < array.Length; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                array[i] = formatter.Deserialize(ref reader, ref context);
            }

            return array;
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, T[] value)
        {
            Array2FormatterHelper.SerializeGenerIList(ref writer, ref context, value);
        }

        public int Size(ref BssomSizeContext context, T[] value)
        {
            return Array2FormatterHelper.SizeGenericIEnumerable<T>(ref context, value);
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

            int i = value.GetLength(0);
            int j = value.GetLength(1);

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
            {
                return default;
            }

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

            reader.SkipVariableNumber();
            if (reader.ReadVariableNumber() != ArrayLength)
            {
                throw BssomSerializationTypeFormatterException.TypeFormatterError(typeof(T[,]), "Invalid T[,] format");
            }

            byte iLength = reader.ReadUInt8();
            byte jLength = reader.ReadUInt8();
            reader.SkipBlankCharacterAndEnsureType(BssomType.Array2);
            reader.SkipVariableNumber();//len
            int maxLen = reader.ReadVariableNumber();

            T[,] array = new T[iLength, jLength];

            int i = 0;
            int j = -1;
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
            {
                return BssomBinaryPrimitives.NullSize;
            }

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

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

            int i = value.GetLength(0);
            int j = value.GetLength(1);
            int k = value.GetLength(2);

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
            {
                return default;
            }

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();
            reader.SkipVariableNumber();
            if (reader.ReadVariableNumber() != ArrayLength)
            {
                throw BssomSerializationTypeFormatterException.TypeFormatterError(typeof(T[,,]), "Invalid T[,,] format");
            }

            byte iLength = reader.ReadUInt8();
            byte jLength = reader.ReadUInt8();
            byte kLength = reader.ReadUInt8();
            reader.SkipBlankCharacterAndEnsureType(BssomType.Array2);
            reader.SkipVariableNumber();
            int maxLen = reader.ReadVariableNumber();

            T[,,] array = new T[iLength, jLength, kLength];

            int i = 0;
            int j = 0;
            int k = -1;
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
            {
                return BssomBinaryPrimitives.NullSize;
            }

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

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
                int i = value.GetLength(0);
                int j = value.GetLength(1);
                int k = value.GetLength(2);
                int l = value.GetLength(3);

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
            {
                return default;
            }

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();
            reader.SkipVariableNumber();
            if (reader.ReadVariableNumber() != ArrayLength)
            {
                throw BssomSerializationTypeFormatterException.TypeFormatterError(typeof(T[,,,]), "Invalid T[,,,] format");
            }

            byte iLength = reader.ReadUInt8();
            byte jLength = reader.ReadUInt8();
            byte kLength = reader.ReadUInt8();
            byte lLength = reader.ReadUInt8();
            reader.SkipBlankCharacterAndEnsureType(BssomType.Array2);
            reader.SkipVariableNumber();
            int maxLen = reader.ReadVariableNumber();
            T[,,,] array = new T[iLength, jLength, kLength, lLength];

            int i = 0;
            int j = 0;
            int k = 0;
            int l = -1;
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
            {
                return BssomBinaryPrimitives.NullSize;
            }

            IBssomFormatter<T> formatter = context.Option.FormatterResolver.GetFormatterWithVerify<T>();

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
