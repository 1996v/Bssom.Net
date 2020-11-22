//using System.Runtime.CompilerServices;

using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.Internal;
using System;
using System.Collections.Generic;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>提供了对Bssom字段的直接访问和编组功能</para> 
    /// <para>Provides direct access to and marshalling of Bssom fields</para>
    /// </summary>
    public struct BssomFieldMarshaller
    {
        private IBssomBufferWriter bufferWriter;
        private long basePos;

        /// <summary>
        /// <para>通过数组来初始化<see cref="BssomFieldMarshaller"/></para>
        /// <para>Initialize <see cref="BssomFieldMarshaller"/> with an array</para>
        /// </summary>
        /// <param name="buffer">binary</param>
        public BssomFieldMarshaller(byte[] buffer) : this(buffer, 0, buffer.Length)
        {
        }

        /// <summary>
        /// <para>通过数组来初始化<see cref="BssomFieldMarshaller"/></para>
        /// <para>Initialize <see cref="BssomFieldMarshaller"/> with an array</para>
        /// </summary>
        /// <param name="buffer">binary</param>
        /// <param name="start">binary start</param>
        /// <param name="len">binary length</param>
        public BssomFieldMarshaller(byte[] buffer, int start, int len) : this(new SimpleBufferWriter(buffer, start, len))
        {
        }

        /// <summary>
        /// <para>通过<see cref="IBssomBufferWriter"/>来初始化<see cref="BssomFieldMarshaller"/></para>
        /// <para>Through <see cref="IBssomBufferWriter"/> to initialize <see cref="BssomFieldMarshaller"/></para>
        /// </summary>
        /// <param name="bufferWriter">buffer writer</param>
        public BssomFieldMarshaller(IBssomBufferWriter bufferWriter)
        {
            this.bufferWriter = bufferWriter;
            basePos = bufferWriter.Position;
        }

        /// <summary>
        /// <para>通过定义的<paramref name="indexOfInputSource"/>来查找字段在缓冲区中的位置</para>
        /// <para>Find the position of the field in the buffer by defining <paramref name="indexOfInputSource"/></para>
        /// </summary>
        /// <param name="indexOfInputSource">输入的查找源. Input search source</param>
        /// <param name="indexOfStartPosition">在缓冲区开始进行访问的位置,若值为-1,则使用构造<see cref="BssomFieldMarshaller"/>时注入的缓冲区位置当做起始访问位置. The position where the buffer starts to be accessed, if the value is -1, the buffer position injected when constructing <see cref="BssomFieldMarshaller"/> is used as the starting access position</param>
        /// <returns>返回该字段在缓冲区中的位置结构,若没有找到该字段,则<see cref="BssomFieldOffsetInfo.Offset"/>值为-1. Return the position structure of the field in the buffer. If the field is not found, the value of <see cref="BssomFieldOffsetInfo.Offset"/> is -1</returns>
        /// <exception cref="ArgumentNullException">input is null</exception>
        /// <exception cref="BssomSerializationArgumentException">input format error</exception>
        /// <exception cref="BssomSerializationOperationException">
        ///     <para><see cref="BssomSerializationOperationException.SerializationErrorCode.ArrayTypeIndexOutOfBounds"/>: 对BssomArray的访问超出了元素索引界限. Access to BssomArray exceeds the element index limit</para>
        ///     <para><see cref="BssomSerializationOperationException.SerializationErrorCode.OperationObjectIsNull"/>: 需要读取或操作的对象为Null. The object that needs to be read or manipulated is Null</para>
        ///     <para><see cref="BssomSerializationOperationException.SerializationErrorCode.UnsupportedOperation"/>: 当前缓冲区的元素类型不支持通过IndexOf来进行查找,目前只支持查找Map类型和Array类型. The element type of the current buffer does not support searching by IndexOf. Currently, only Map type and Array type are supported.</para>
        ///     <para><see cref="BssomSerializationOperationException.SerializationErrorCode.IncorrectTypeCode"/>: 缓冲区数据格式错误. Buffer data format error</para>
        /// </exception>
        public BssomFieldOffsetInfo IndexOf(IIndexOfInputSource indexOfInputSource, long indexOfStartPosition = -1)
        {
            if (indexOfStartPosition == -1)
            {
                indexOfStartPosition = basePos;
            }

            if (bufferWriter.Position != indexOfStartPosition)
            {
                bufferWriter.Seek(indexOfStartPosition, BssomSeekOrgin.Begin);
            }

            return IndexOf(bufferWriter.GetBssomBuffer(), indexOfInputSource);
        }

        /// <summary>
        /// <para>通过固定的字段访问语言来查找字段在缓冲区中的位置,该语言在查找Map类型时仅支持Key为String类型的元素</para>
        /// <para>Find the position of the field in the buffer through the fixed field access language,the language only supports elements whose Key is String type when searching for Map</para>
        /// </summary>
        /// <param name="simpleMemberAccessLang">特定的访问格式,[{Key:String}]: 通过Key访问BssomMap的Value, ${Index:Int32}: 通过索引访问BssomArray的元素.  Specific access format, [{Key: String}]: access the value of BssomMap by key, $ {Index: Int32}: access the element of BssomArray by index</param>
        /// <param name="indexOfStartPosition">在缓冲区开始进行访问的位置,若值为-1,则使用构造<see cref="BssomFieldMarshaller"/>时注入的缓冲区位置当做起始访问位置. The position where the buffer starts to be accessed, if the value is -1, the buffer position injected when constructing <see cref="BssomFieldMarshaller"/> is used as the starting access position</param>
        /// <returns>返回该字段在缓冲区中的位置结构,若没有找到该字段,则<see cref="BssomFieldOffsetInfo.Offset"/>值为-1. Return the position structure of the field in the buffer. If the field is not found, the value of <see cref="BssomFieldOffsetInfo.Offset"/> is -1</returns>
        /// <exception cref="ArgumentNullException">input is null</exception>
        /// <exception cref="BssomSerializationArgumentException">input format error</exception>
        /// <exception cref="BssomSerializationOperationException">
        ///     <para><see cref="BssomSerializationOperationException.SerializationErrorCode.ArrayTypeIndexOutOfBounds"/>: 对BssomArray的访问超出了元素索引界限. Access to BssomArray exceeds the element index limit</para>
        ///     <para><see cref="BssomSerializationOperationException.SerializationErrorCode.OperationObjectIsNull"/>: 需要读取或操作的对象为Null. The object that needs to be read or manipulated is Null</para>
        ///     <para><see cref="BssomSerializationOperationException.SerializationErrorCode.UnsupportedOperation"/>: 当前缓冲区的元素类型不支持通过IndexOf来进行查找,目前只支持查找Map类型和Array类型. The element type of the current buffer does not support searching by IndexOf. Currently, only Map type and Array type are supported.</para>
        ///     <para><see cref="BssomSerializationOperationException.SerializationErrorCode.IncorrectTypeCode"/>: 缓冲区数据格式错误. Buffer data format error</para>
        /// </exception>
        /// <example>
        /// <code>
        ///     Buffer: { "Group1" : [ 255,254 ] , "Group2" : { "Name" : "Zeng" } }
        ///     IndexOf("[Group1]$0") --> 255
        ///     IndexOf("[Group2][Name]") --> "Zeng"
        /// </code>
        /// </example>
        public unsafe BssomFieldOffsetInfo IndexOf(string simpleMemberAccessLang, long indexOfStartPosition = -1)
        {
            if (indexOfStartPosition == -1)
            {
                indexOfStartPosition = basePos;
            }

            if (bufferWriter.Position != indexOfStartPosition)
            {
                bufferWriter.Seek(indexOfStartPosition, BssomSeekOrgin.Begin);
            }

            return IndexOf(bufferWriter.GetBssomBuffer(), simpleMemberAccessLang);
        }

        /// <summary>
        /// 通过指定缓冲区的位置信息来读取元素. Read the element by specifying the location information of the buffer
        /// </summary>
        /// <typeparam name="T">被读取元素的类型,如不确认可以为<see cref="object"/>. The type of the read element, if not confirmed, it can be <seealso cref="object"/></typeparam>
        /// <param name="offsetInfo">用于读取的位置信息. Location information for reading</param>
        /// <param name="option">过程中所使用的配置. Configuration used in the process</param>
        /// <returns>被读取的元素. Element being read</returns>
        public T ReadValue<T>(BssomFieldOffsetInfo offsetInfo, BssomSerializerOptions option = null)
        {
            return ReadValue<T>(bufferWriter.GetBssomBuffer(), offsetInfo, option);
        }

        /// <summary>
        /// 通过指定缓冲区的位置信息来读取元素的类型. Read the element type by specifying the location information of the buffer
        /// </summary>
        /// <param name="offsetInfo">用于读取的位置信息. Location information for reading</param>
        /// <returns>被读取的元素类型. Element type being read</returns>
        public BssomValueType ReadValueType(BssomFieldOffsetInfo offsetInfo)
        {
            return ReadValueType(bufferWriter.GetBssomBuffer(), offsetInfo);
        }

        /// <summary>
        /// 通过指定缓冲区的位置信息来读取元素的类型码. Read the element typecode by specifying the location information of the buffer
        /// </summary>
        /// <param name="offsetInfo">用于读取元素的位置信息. Location information for reading</param>
        /// <param name="isNativeType">若对象是本地类型,则为true. True if the object is a native type</param>
        /// <returns>被读取的元素类型码. Element typecode being read</returns>
        public byte ReadValueTypeCode(BssomFieldOffsetInfo offsetInfo, out bool isNativeType)
        {
            return ReadValueTypeCode(bufferWriter.GetBssomBuffer(), offsetInfo, out isNativeType);
        }

        /// <summary>
        /// 通过指定缓冲区的位置信息来读取元素在缓冲区存储的大小(包括从此位置开始的空白符). Read the size of the element stored in the buffer by specifying the location information of the buffer(includes white space from this position)
        /// </summary>
        /// <param name="offsetInfo">用于读取元素的位置信息. Location information for reading</param>
        /// <returns>被读取的元素大小,包含空白符大小. Element typecode being read size,contains the blank size</returns>
        public int ReadValueSize(BssomFieldOffsetInfo offsetInfo)
        {
            return ReadValueSize(bufferWriter.GetBssomBuffer(), offsetInfo);
        }

        /// <summary>
        /// 通过指定缓冲区的位置信息来读取Array对象的元素数量. Read the number of elements of the Array object by specifying the location information of the buffer
        /// </summary>
        /// <param name="offsetInfo">用于读取元素的位置信息. Location information for reading</param>
        /// <returns>Array对象的元素数量. The number of elements of the Array object</returns>
        /// <exception cref="BssomSerializationOperationException">缓冲区<paramref name="offsetInfo"/>位置处的对象并不是Array类型,类型读取错误. The object at the position of the buffer <paramref name="offsetInfo"/> is not a Array type, and the type is read incorrectly</exception>
        public int ReadArrayCountByArrayType(BssomFieldOffsetInfo offsetInfo)
        {
            return ReadArrayCountByArrayType(bufferWriter.GetBssomBuffer(), offsetInfo);
        }

        /// <summary>
        /// 通过指定缓冲区的位置信息来读取Map对象的字段位置信息. Read the field location information of the Map object by specifying the location information of the buffer
        /// </summary>
        /// <typeparam name="TKey">Map对象的Key类型,如不确认可以为<see cref="object"/>. The Key type of the Map object, if not confirmed, it can be <seealso cref="object"/></typeparam>
        /// <param name="offsetInfo">用于读取的位置信息. Location information for reading</param>
        /// <param name="option">过程中所使用的配置. Configuration used in the process</param>
        /// <returns>Map对象的字段位置信息. Field location information of the Map object</returns>
        /// <exception cref="BssomSerializationOperationException">缓冲区<paramref name="offsetInfo"/>位置处的对象并不是Map类型,类型读取错误. The object at the position of the buffer <paramref name="offsetInfo"/> is not a Map type, and the type is read incorrectly</exception>
        public IEnumerable<KeyValuePair<TKey, BssomFieldOffsetInfo>> ReadAllKeysByMapType<TKey>(BssomFieldOffsetInfo offsetInfo, BssomSerializerOptions option = null)
        {
            return ReadAllKeysByMapType<TKey>(bufferWriter.GetBssomBuffer(), offsetInfo, option);
        }

        /// <summary>
        /// <para>尝试在指定的位置对缓冲区进行对象写入</para>
        /// <para>Attempt to write an object to the buffer at the specified location</para>
        /// </summary>
        /// <typeparam name="T">被写入元素的类型. The type of element being written</typeparam>
        /// <param name="offsetInfo">用于写入元素的位置信息. Used to write the position information of the element</param>
        /// <param name="value">被写入的对象. Value being written</param>
        /// <param name="option">过程中所使用的配置. Configuration used in the process</param>
        /// <returns>
        /// <para>如果对象的二进制大小小于对象在缓冲区中的大小,则成功写入,返回true</para>
        /// <para>If the binary size of the value is smaller than the size of the member in the buffer, it is successfully written and true is returned</para>
        /// </returns>
        public bool TryWrite<T>(BssomFieldOffsetInfo offsetInfo, T value, BssomSerializerOptions option = null)
        {
            return TryWrite<T>(bufferWriter, offsetInfo, value, option);
        }

        /// <summary>
        /// <para>获取Array3格式中指定下标元素的偏移量</para>
        /// <para>Gets the offset of the index element specified in Array3 format</para>
        /// </summary>
        /// <param name="index">需要访问元素的下标. need to access the index of the element</param>
        /// <returns>返回该字段在缓冲区中的位置结构,若没有找到该字段,则<see cref="BssomFieldOffsetInfo.Offset"/>值为-1. Return the position structure of the field in the buffer. If the field is not found, the value of <see cref="BssomFieldOffsetInfo.Offset"/> is -1</returns>
        public BssomFieldOffsetInfo IndexOfArray3Item(int index, long indexOfStartPosition = -1)
        {
            if (indexOfStartPosition == -1)
            {
                indexOfStartPosition = basePos;
            }

            if (bufferWriter.Position != indexOfStartPosition)
            {
                bufferWriter.Seek(indexOfStartPosition, BssomSeekOrgin.Begin);
            }

            return IndexOfArray3Item(index, bufferWriter.GetBssomBuffer());
        }

        #region StaticMethod

        public static BssomFieldOffsetInfo IndexOf(byte[] buffer, int start, int end, IIndexOfInputSource indexOfInputSource)
        {
            return IndexOf(new SimpleBufferWriter(buffer, start, end), indexOfInputSource);
        }

        public static BssomFieldOffsetInfo IndexOf(byte[] buffer, int start, int end, string simpleMemberAccessLang)
        {
            return IndexOf(new SimpleBufferWriter(buffer, start, end), simpleMemberAccessLang);
        }

        public static T ReadValue<T>(byte[] buffer, int start, int end, BssomFieldOffsetInfo offsetInfo, BssomSerializerOptions option = null)
        {
            return ReadValue<T>(new SimpleBufferWriter(buffer, start, end), offsetInfo, option);
        }

        public static int ReadValueSize(byte[] buffer, int start, int end, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadValueSize(new SimpleBufferWriter(buffer, start, end), offsetInfo);
        }

        public static BssomValueType ReadValueType(byte[] buffer, int start, int end, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadValueType(new SimpleBufferWriter(buffer, start, end), offsetInfo);
        }

        public static byte ReadValueTypeCode(byte[] buffer, int start, int end, BssomFieldOffsetInfo offsetInfo, out bool isNativeType)
        {
            return ReadValueTypeCode(new SimpleBufferWriter(buffer, start, end), offsetInfo, out isNativeType);
        }

        public static int ReadArrayCount(byte[] buffer, int start, int end, BssomFieldOffsetInfo offsetInfo)
        {
            return ReadArrayCountByArrayType(new SimpleBufferWriter(buffer, start, end), offsetInfo);
        }

        public static IEnumerable<KeyValuePair<TKey, BssomFieldOffsetInfo>> ReadAllKeysByMapType<TKey>(byte[] buffer, int start, int end, BssomFieldOffsetInfo offsetInfo, BssomSerializerOptions option = null)
        {
            return ReadAllKeysByMapType<TKey>(new SimpleBufferWriter(buffer, start, end), offsetInfo, option);
        }

        public static bool TryWrite<T>(byte[] buffer, int start, int end, BssomFieldOffsetInfo offsetInfo, T value, BssomSerializerOptions option = null)
        {
            return TryWrite(new SimpleBufferWriter(buffer, start, end), offsetInfo, value, option);
        }

        public static BssomFieldOffsetInfo IndexOf(IBssomBuffer buffer, IIndexOfInputSource indexOfInputSource)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (indexOfInputSource is null)
            {
                throw new ArgumentNullException(nameof(indexOfInputSource));
            }

            BssomReader reader = new BssomReader(buffer);
            BssomFieldOffsetInfo info = new BssomFieldOffsetInfo();

            indexOfInputSource.Reset();
            if (!indexOfInputSource.MoveNext())
            {
                throw BssomSerializationOperationException.InputDataSouceIsEmpty();
            }

        Next:
            byte bssOjectType = reader.SkipBlankCharacterAndReadBssomType();
            switch (bssOjectType)
            {
                case BssomType.Map1:
                    {
                        object key = indexOfInputSource.CurrentMapKey();
                        if (!BssMapKeyResolverProvider.TryGetBssMapKeyResolver(key.GetType(), out IBssMapKeyResolver resolver))
                        {
                            throw BssomSerializationTypeFormatterException.BssomMapKeyUnsupportedType(key.GetType());
                        }

                        reader.SkipVariableNumber();//Skip Length 
                        int count = reader.ReadVariableNumber();
                        Raw64BytesISegment keyISegment = resolver.GetMap1KeySegment(key);
                        for (int i = 0; i < count; i++)
                        {
                            int keyLength = reader.GetMap1KeyLength();
                            if (keyISegment.DataLen != keyLength)
                            {
                                reader.BssomBuffer.Seek(keyLength, BssomSeekOrgin.Current);//Advance Key
                                reader.SkipObject();//Skip Value
                                continue;
                            }

                            //determine if the key is equal
                            bool keyIsEqual = true;
                            for (int z = 0; z < keyISegment.Length; z++)
                            {
                                if (keyISegment[z] != reader.ReadRaw64(ref keyLength))
                                {
                                    reader.BssomBuffer.Seek(keyLength, BssomSeekOrgin.Current);//Advance remaining keyLength
                                    reader.SkipObject();//Skip Value
                                    keyIsEqual = false;
                                    break;
                                }
                            }

                            if (keyIsEqual)
                            {
                                if (!indexOfInputSource.MoveNext())
                                {
                                    info.Offset = reader.Position;
                                    info.IsArray1Type = false;
                                    return info;
                                }
                                else
                                {
                                    goto Next;
                                }
                            }
                        }
                        if (indexOfInputSource.MoveNext())
                        {
                            throw BssomSerializationOperationException.BssomMapIsNull(key);
                        }

                        info.Offset = -1;
                        return info;
                    }
                case BssomType.Map2:
                    {
                        object key = indexOfInputSource.CurrentMapKey();
                        if (!BssMapKeyResolverProvider.TryGetBssMapKeyResolver(key.GetType(), out IBssMapKeyResolver resolver))
                        {
                            throw BssomSerializationTypeFormatterException.BssomMapKeyUnsupportedType(key.GetType());
                        }

                        UInt64BytesISegment keyISegment = resolver.GetMap2KeySegment(key);
                        BssMapHeadPackInfo aprp = BssMapHeadPackInfo.Create(ref reader);
                        if (aprp.MapHead.ElementCount > 0)
                        {
                            ref byte refb = ref reader.BssomBuffer.TryReadFixedRef(aprp.MapHead.RouteLength, out bool haveEnoughSizeAndCanBeFixed);
                            if (haveEnoughSizeAndCanBeFixed)
                            {
                                if (BssMapObjMarshalReader.TrySeek(keyISegment, resolver.KeyType, resolver.KeyIsNativeType, ref aprp, ref reader, ref refb))
                                {
                                    if (!indexOfInputSource.MoveNext())
                                    {
                                        info.Offset = reader.Position;
                                        info.IsArray1Type = false;
                                        return info;
                                    }
                                    else
                                    {
                                        goto Next;
                                    }
                                }
                            }
                            else
                            {
                                if (BssMapObjMarshalReader.TrySeekSlow(keyISegment, resolver.KeyType, resolver.KeyIsNativeType, ref aprp, ref reader))
                                {
                                    if (!indexOfInputSource.MoveNext())
                                    {
                                        info.Offset = reader.Position;
                                        info.IsArray1Type = false;
                                        return info;
                                    }
                                    else
                                    {
                                        goto Next;
                                    }
                                }
                            }
                        }
                        if (indexOfInputSource.MoveNext())
                        {
                            throw BssomSerializationOperationException.BssomMapIsNull(key);
                        }

                        info.Offset = -1;
                        return info;
                    }
                case BssomType.Array1:
                    {
                        int index = indexOfInputSource.CurrentArrayIndex();

                        info.IsArray1Type = true;
                        info.Array1ElementType = reader.ReadBssomType();
                        if (info.Array1ElementType == BssomType.NativeCode)
                        {
                            info.Array1ElementType = reader.ReadBssomType();
                            info.Array1ElementTypeIsNativeType = true;
                        }

                        reader.SkipVariableNumber();//len
                        int count = reader.ReadVariableNumber();//count
                        if (index < count)
                        {
                            if (!BssomBinaryPrimitives.TryGetTypeSizeFromStaticTypeSizes(info.Array1ElementTypeIsNativeType, info.Array1ElementType, out int eleSize))
                            {
                                throw BssomSerializationOperationException.UnexpectedCodeRead(info.Array1ElementType, reader.Position);
                            }

                            if (info.Array1ElementTypeIsNativeType)
                                eleSize -= BssomBinaryPrimitives.NativeTypeCodeSize;
                            else
                                eleSize -= BssomBinaryPrimitives.BuildInTypeCodeSize;

                            if (!indexOfInputSource.MoveNext())
                            {
                                info.Offset = reader.Position + index * eleSize;
                                return info;
                            }
                            else
                            {
                                reader.BssomBuffer.Seek(index * eleSize, BssomSeekOrgin.Current);
                                goto Next;
                            }
                        }
                        throw BssomSerializationOperationException.ArrayTypeIndexOutOfBounds(index, count);
                    }
                case BssomType.Array2:
                    {
                        int index = indexOfInputSource.CurrentArrayIndex();

                        reader.SkipVariableNumber();//len
                        int count = reader.ReadVariableNumber();//count
                        if (index < count)
                        {
                            for (int i = 0; i < index; i++)
                            {
                                reader.SkipObject();
                            }
                            if (!indexOfInputSource.MoveNext())
                            {
                                info.Offset = reader.Position;
                                info.IsArray1Type = false;
                                return info;
                            }
                            else
                            {
                                goto Next;
                            }
                        }
                        throw BssomSerializationOperationException.ArrayTypeIndexOutOfBounds(index, count);
                    }
                case BssomType.Array3:
                    {
                        int index = indexOfInputSource.CurrentArrayIndex();

                        long basePosition = reader.Position - 1;
                        reader.SkipVariableNumber();//len
                        int count = reader.ReadVariableNumber();//count
                        if (index < count)
                        {
                            var skipBytes = index * BssomBinaryPrimitives.FixUInt32NumberSize;
                            if (skipBytes != 0)
                                reader.BssomBuffer.Seek(skipBytes, BssomSeekOrgin.Current);
                            int off = reader.ReadVariableNumber();

                            info.Offset = basePosition + off;
                            if (!indexOfInputSource.MoveNext())
                            {
                                info.IsArray1Type = false;
                                return info;
                            }
                            else
                            {
                                reader.BssomBuffer.Seek(info.Offset);
                                goto Next;
                            }
                        }
                        throw BssomSerializationOperationException.ArrayTypeIndexOutOfBounds(index, count);
                    }
                case BssomType.NullCode:
                    throw BssomSerializationOperationException.BssomObjectIsNull();
                default:
                    throw BssomSerializationOperationException.BssomValueTypeReadFromStreamNotSupportIndexOf(reader.Position);
            }
        }

        public static BssomFieldOffsetInfo IndexOf(IBssomBuffer buffer, string simpleMemberAccessLang)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (simpleMemberAccessLang is null)
            {
                throw new ArgumentNullException(nameof(simpleMemberAccessLang));
            }

            BssomReader reader = new BssomReader(buffer);
            StringInputDataSource stringInputDataSource = new StringInputDataSource(simpleMemberAccessLang);
            BssomFieldOffsetInfo info = new BssomFieldOffsetInfo();

            if (!stringInputDataSource.MoveNext())
            {
                throw BssomSerializationOperationException.InputDataSouceIsEmpty();
            }

        Next:
            byte bssOjectType = reader.SkipBlankCharacterAndReadBssomType();
            switch (bssOjectType)
            {
                case BssomType.Map1:
                    {
                        Raw64BytesISegment keyISegment = stringInputDataSource.GetCurrentSegmentFromMap1StringKey();

                        reader.SkipVariableNumber();//Skip Length 
                        int count = reader.ReadVariableNumber();
                        for (int i = 0; i < count; i++)
                        {
                            int keyLength = reader.GetMap1KeyLength();
                            if (keyISegment.DataLen != keyLength)
                            {
                                reader.BssomBuffer.Seek(keyLength, BssomSeekOrgin.Current);//Advance Key
                                reader.SkipObject();//Skip Value
                                continue;
                            }

                            //determine if the key is equal
                            bool keyIsEqual = true;
                            for (int z = 0; z < keyISegment.Length; z++)
                            {
                                if (keyISegment[z] != reader.ReadRaw64(ref keyLength))
                                {
                                    reader.BssomBuffer.Seek(keyLength, BssomSeekOrgin.Current);//Advance remaining keyLength
                                    reader.SkipObject();//Skip Value
                                    keyIsEqual = false;
                                    break;
                                }
                            }

                            if (keyIsEqual)
                            {
                                if (!stringInputDataSource.MoveNext())
                                {
                                    info.Offset = reader.Position;
                                    info.IsArray1Type = false;
                                    return info;
                                }
                                else
                                {
                                    goto Next;
                                }
                            }
                        }
                        if (stringInputDataSource.MoveNext())
                        {
                            throw BssomSerializationOperationException.BssomMapIsNull(stringInputDataSource.GetCurrentSegmentString());
                        }

                        info.Offset = -1;
                        return info;
                    }
                case BssomType.Map2:
                    {
                        UInt64BytesISegment keyISegment = stringInputDataSource.GetCurrentSegmentFromMap2StringKey();
                        BssMapHeadPackInfo aprp = BssMapHeadPackInfo.Create(ref reader);
                        if (aprp.MapHead.ElementCount > 0)
                        {
                            ref byte refb = ref reader.BssomBuffer.TryReadFixedRef(aprp.MapHead.RouteLength, out bool haveEnoughSizeAndCanBeFixed);
                            if (haveEnoughSizeAndCanBeFixed)
                            {
                                if (BssMapObjMarshalReader.TrySeek(keyISegment, BssomType.StringCode, false, ref aprp, ref reader, ref refb))
                                {
                                    if (!stringInputDataSource.MoveNext())
                                    {
                                        info.Offset = reader.Position;
                                        info.IsArray1Type = false;
                                        return info;
                                    }
                                    else
                                    {
                                        goto Next;
                                    }
                                }
                            }
                            else
                            {
                                if (BssMapObjMarshalReader.TrySeekSlow(keyISegment, BssomType.StringCode, false, ref aprp, ref reader))
                                {
                                    if (!stringInputDataSource.MoveNext())
                                    {
                                        info.Offset = reader.Position;
                                        info.IsArray1Type = false;
                                        return info;
                                    }
                                    else
                                    {
                                        goto Next;
                                    }
                                }
                            }
                        }
                        if (stringInputDataSource.MoveNext())
                        {
                            throw BssomSerializationOperationException.BssomMapIsNull(stringInputDataSource.GetCurrentSegmentString());
                        }

                        info.Offset = -1;
                        return info;
                    }
                case BssomType.Array1:
                    {
                        int index = stringInputDataSource.GetCurrentArrayIndex();

                        info.IsArray1Type = true;
                        info.Array1ElementType = reader.ReadBssomType();
                        if (info.Array1ElementType == BssomType.NativeCode)
                        {
                            info.Array1ElementType = reader.ReadBssomType();
                            info.Array1ElementTypeIsNativeType = true;
                        }

                        reader.SkipVariableNumber();//len
                        int count = reader.ReadVariableNumber();//count
                        if (index < count)
                        {
                            if (!BssomBinaryPrimitives.TryGetTypeSizeFromStaticTypeSizes(info.Array1ElementTypeIsNativeType, info.Array1ElementType, out int eleSize))
                            {
                                throw BssomSerializationOperationException.UnexpectedCodeRead(info.Array1ElementType, reader.Position);
                            }

                            if (info.Array1ElementTypeIsNativeType)
                                eleSize -= BssomBinaryPrimitives.NativeTypeCodeSize;
                            else
                                eleSize -= BssomBinaryPrimitives.BuildInTypeCodeSize;

                            if (!stringInputDataSource.MoveNext())
                            {
                                info.Offset = reader.Position + index * eleSize;
                                return info;
                            }
                            else
                            {
                                reader.BssomBuffer.Seek(index * eleSize, BssomSeekOrgin.Current);
                                goto Next;
                            }
                        }
                        throw BssomSerializationOperationException.ArrayTypeIndexOutOfBounds(index, count);
                    }
                case BssomType.Array2:
                    {
                        int index = stringInputDataSource.GetCurrentArrayIndex();

                        reader.SkipVariableNumber();//len
                        int count = reader.ReadVariableNumber();//count
                        if (index < count)
                        {
                            for (int i = 0; i < index; i++)
                            {
                                reader.SkipObject();
                            }
                            if (!stringInputDataSource.MoveNext())
                            {
                                info.Offset = reader.Position;
                                info.IsArray1Type = false;
                                return info;
                            }
                            else
                            {
                                goto Next;
                            }
                        }
                        throw BssomSerializationOperationException.ArrayTypeIndexOutOfBounds(index, count);
                    }
                case BssomType.Array3:
                    {
                        int index = stringInputDataSource.GetCurrentArrayIndex();

                        long basePosition = reader.Position - 1;
                        reader.SkipVariableNumber();//len
                        int count = reader.ReadVariableNumber();//count
                        if (index < count)
                        {
                            var skipBytes = index * BssomBinaryPrimitives.FixUInt32NumberSize;
                            if (skipBytes != 0)
                                reader.BssomBuffer.Seek(skipBytes, BssomSeekOrgin.Current);
                            int off = reader.ReadVariableNumber();

                            info.Offset = basePosition + off;
                            if (!stringInputDataSource.MoveNext())
                            {
                                info.IsArray1Type = false;
                                return info;
                            }
                            else
                            {
                                reader.BssomBuffer.Seek(info.Offset);
                                goto Next;
                            }
                        }
                        throw BssomSerializationOperationException.ArrayTypeIndexOutOfBounds(index, count);
                    }
                case BssomType.NullCode:
                    throw BssomSerializationOperationException.BssomObjectIsNull();
                default:
                    throw BssomSerializationOperationException.BssomValueTypeReadFromStreamNotSupportIndexOf(reader.Position);
            }
        }

        public static BssomFieldOffsetInfo IndexOfArray3Item(int index, IBssomBuffer buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index < 0)
            {
                throw new ArgumentException(nameof(index));
            }

            BssomReader reader = new BssomReader(buffer);

            byte typeCode = reader.SkipBlankCharacterAndReadBssomType();
            if (typeCode == BssomType.Array3)
            {
                long basePos = reader.Position - 1;
                reader.SkipVariableNumber();
                int count = reader.ReadVariableNumber();
                if (index < count)
                {
                    reader.BssomBuffer.Seek(index * BssomBinaryPrimitives.FixUInt32NumberSize, BssomSeekOrgin.Current);
                    int off = reader.ReadVariableNumber();
                    return new BssomFieldOffsetInfo(basePos + off);
                }
                throw BssomSerializationOperationException.ArrayTypeIndexOutOfBounds(index, count);
            }
            throw BssomSerializationOperationException.UnexpectedCodeRead(typeCode, reader.Position);
        }

        public static T ReadValue<T>(IBssomBuffer buffer, BssomFieldOffsetInfo offsetInfo, BssomSerializerOptions option = null)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offsetInfo.Offset == -1)
            {
                throw BssomSerializationArgumentException.InvalidOffsetInfoValue();
            }

            if (option == null)
            {
                option = BssomSerializerOptions.Default;
            }

            BssomReader reader = new BssomReader(buffer);
            BssomDeserializeContext context = new BssomDeserializeContext(option);

            if (reader.Position != offsetInfo.Offset)
            {
                reader.BssomBuffer.Seek(offsetInfo.Offset, BssomSeekOrgin.Begin);
            }

            if (offsetInfo.IsArray1Type)
            {
                return Array1ElementWriterFactory<T>.ReadElement(ref reader, option, offsetInfo);
            }
            IBssomFormatter<T> formatter = option.FormatterResolver.GetFormatterWithVerify<T>();
            return formatter.Deserialize(ref reader, ref context);
        }

        public static int ReadValueSize(IBssomBuffer buffer, BssomFieldOffsetInfo offsetInfo)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offsetInfo.Offset == -1)
            {
                throw BssomSerializationArgumentException.InvalidOffsetInfoValue();
            }

            BssomReader reader = new BssomReader(buffer);

            if (reader.Position != offsetInfo.Offset)
            {
                reader.BssomBuffer.Seek(offsetInfo.Offset, BssomSeekOrgin.Begin);
            }

            if (offsetInfo.IsArray1Type)
            {
                if (BssomBinaryPrimitives.TryGetTypeSizeFromStaticTypeSizes(offsetInfo.Array1ElementTypeIsNativeType, offsetInfo.Array1ElementType, out int size))
                {
                    if (offsetInfo.Array1ElementTypeIsNativeType)
                        size -= BssomBinaryPrimitives.NativeTypeCodeSize;
                    else
                        size -= BssomBinaryPrimitives.BuildInTypeCodeSize;
                    return size;
                }

                throw BssomSerializationOperationException.UnexpectedCodeRead(offsetInfo.Array1ElementType, reader.Position);
            }

            return reader.GetObjectLengthWithBlank();
        }

        public static BssomValueType ReadValueType(IBssomBuffer buffer, BssomFieldOffsetInfo offsetInfo)
        {
            byte typeCode = ReadValueTypeCode(buffer, offsetInfo, out bool isNativeType);
            return BssomType.GetBssomValueType(isNativeType, typeCode);
        }

        public static byte ReadValueTypeCode(IBssomBuffer buffer, BssomFieldOffsetInfo offsetInfo, out bool isNativeType)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offsetInfo.Offset == -1)
            {
                throw BssomSerializationArgumentException.InvalidOffsetInfoValue();
            }

            BssomReader reader = new BssomReader(buffer);

            if (offsetInfo.IsArray1Type)
            {
                isNativeType = offsetInfo.Array1ElementTypeIsNativeType;
                return offsetInfo.Array1ElementType;
            }

            if (reader.Position != offsetInfo.Offset)
            {
                reader.BssomBuffer.Seek(offsetInfo.Offset, BssomSeekOrgin.Begin);
            }

            byte typeCode = reader.SkipBlankCharacterAndReadBssomType();
            if (typeCode == BssomType.NativeCode)
            {
                isNativeType = true;
                return reader.ReadBssomType();
            }
            isNativeType = false;
            return typeCode;
        }

        public static int ReadArrayCountByArrayType(IBssomBuffer buffer, BssomFieldOffsetInfo offsetInfo)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offsetInfo.Offset == -1)
            {
                throw BssomSerializationArgumentException.InvalidOffsetInfoValue();
            }

            BssomReader reader = new BssomReader(buffer);

            if (reader.Position != offsetInfo.Offset)
            {
                reader.BssomBuffer.Seek(offsetInfo.Offset, BssomSeekOrgin.Begin);
            }

            byte typeCode = reader.SkipBlankCharacterAndReadBssomType();
            if (typeCode == BssomType.Array1)
            {
                if (reader.ReadBssomType() == BssomType.NativeCode)
                {
                    reader.BssomBuffer.Seek(BssomBinaryPrimitives.BuildInTypeCodeSize, BssomSeekOrgin.Current);
                }
                reader.SkipVariableNumber();
                return reader.ReadVariableNumber();
            }
            else if (typeCode == BssomType.Array2 || typeCode == BssomType.Array3)
            {
                reader.SkipVariableNumber();
                return reader.ReadVariableNumber();
            }
            throw BssomSerializationOperationException.UnexpectedCodeRead(typeCode, reader.Position);
        }

        public static IEnumerable<KeyValuePair<TKey, BssomFieldOffsetInfo>> ReadAllKeysByMapType<TKey>(IBssomBuffer buffer, BssomFieldOffsetInfo offsetInfo, BssomSerializerOptions option = null)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offsetInfo.Offset == -1)
            {
                throw BssomSerializationArgumentException.InvalidOffsetInfoValue();
            }

            if (option == null)
            {
                option = BssomSerializerOptions.Default;
            }

            BssomReader reader = new BssomReader(buffer);
            BssomDeserializeContext context = new BssomDeserializeContext(option);

            if (reader.Position != offsetInfo.Offset)
            {
                reader.BssomBuffer.Seek(offsetInfo.Offset, BssomSeekOrgin.Begin);
            }

            return MapFormatterHelper.ReadAllKeys<TKey>(ref reader, ref context);
        }

        public static bool TryWrite<T>(IBssomBufferWriter bufferWriter, BssomFieldOffsetInfo offsetInfo, T value, BssomSerializerOptions option = null)
        {
            if (bufferWriter is null)
            {
                throw new ArgumentNullException(nameof(bufferWriter));
            }

            if (offsetInfo.Offset == -1)
            {
                throw BssomSerializationArgumentException.InvalidOffsetInfoValue();
            }

            if (option == null)
            {
                option = BssomSerializerOptions.Default;
            }

            BssomSerializeContext serializeContext = new BssomSerializeContext(option);
            BssomSizeContext sizeContext = new BssomSizeContext(option);
            BssomWriter writer = new BssomWriter(bufferWriter);

            if (writer.Position != offsetInfo.Offset)
            {
                writer.BufferWriter.Seek(offsetInfo.Offset, BssomSeekOrgin.Begin);
            }

            if (offsetInfo.IsArray1Type)
            {
                Array1ElementWriterFactory<T>.WriteElement(ref writer, option, offsetInfo, value);
                return true;
            }
            else
            {
                IBssomFormatter<T> formatter = option.FormatterResolver.GetFormatterWithVerify<T>();
                BssomReader reader = writer.GetReader();
                int len = reader.GetObjectLengthWithBlank();
                len -= formatter.Size(ref sizeContext, value);

                if (len < 0)
                {
                    return false;
                }

                if (writer.Position != offsetInfo.Offset)
                {
                    writer.BufferWriter.Seek(offsetInfo.Offset, BssomSeekOrgin.Begin);
                }

                if (len > 0)
                {
                    if (len <= BssomType.MaxVarBlankCodeValue)
                    {
                        writer.WriteWithOutTypeHead((byte)(len - 1));
                        writer.BufferWriter.Advance(len - 1);
                    }
                    else if (len <= ushort.MaxValue)
                    {
                        writer.WriteBuildInType(BssomType.BlankUInt16Code);
                        writer.WriteWithOutTypeHead((ushort)(len - 1 - 2));
                        writer.BufferWriter.Advance(len - 1 - 2);
                    }
                    else
                    {
                        writer.WriteBuildInType(BssomType.BlankUInt32Code);
                        writer.WriteWithOutTypeHead((uint)(len - 1 - 4));
                        writer.BufferWriter.Advance(len - 1 - 4);
                    }
                }

                formatter.Serialize(ref writer, ref serializeContext, value);
                return true;
            }
        }

        #endregion
    }
}
