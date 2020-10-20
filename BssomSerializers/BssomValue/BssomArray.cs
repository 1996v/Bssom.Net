using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
using System.Linq;

namespace BssomSerializers
{
    /// <summary>
    /// Represents a BSSOM array.
    /// </summary>
    public class BssomArray : BssomValue, IList<object>, IEnumerable<BssomValue>
    {
        private IEnumerable _rawValue;//must is List<>

        /// <summary>
        /// The representation of raw Value
        /// </summary>
        public IEnumerable Value => _rawValue;
        /// <summary>
        /// Element count
        /// </summary>
        public int Count => Unsafe.As<IEnumerable, List<int>>(ref _rawValue).Count;
        /// <summary>
        /// Element type
        /// </summary>
        public BssomArrayElementType ElementType { get; }
        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.Array;
        /// <summary>
        /// Raw value
        /// </summary>
        public override object RawValue => _rawValue;

        bool ICollection<object>.IsReadOnly => false;
        object IList<object>.this[int index] { get => GetRawValue(index); set => SetRawValue(index, value); }

        private BssomArray(IEnumerable value, BssomArrayElementType elementType)
        {
            if (value is null)
                throw new ArgumentNullException();

            _rawValue = value;
            ElementType = elementType;
        }

        /// <summary>
        /// Initializes array2 BssomArray
        /// </summary>
        public BssomArray(IEnumerable value)
        {
            if (value is null)
                throw new ArgumentNullException();

            List<object> list = new List<object>();
            foreach (var item in value)
            {
                list.Add(item);
            }

            _rawValue = list;
            ElementType = BssomArrayElementType.BssomValue;
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<byte> value) : this(new List<byte>(value), BssomArrayElementType.Byte)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<sbyte> value) : this(new List<sbyte>(value), BssomArrayElementType.SByte)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<Int16> value) : this(new List<Int16>(value), BssomArrayElementType.Int16)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<Int32> value) : this(new List<Int32>(value), BssomArrayElementType.Int32)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<Int64> value) : this(new List<Int64>(value), BssomArrayElementType.Int64)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<UInt16> value) : this(new List<UInt16>(value), BssomArrayElementType.UInt16)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<UInt32> value) : this(new List<UInt32>(value), BssomArrayElementType.UInt32)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<UInt64> value) : this(new List<UInt64>(value), BssomArrayElementType.UInt64)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<Char> value) : this(new List<Char>(value), BssomArrayElementType.Char)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<Single> value) : this(new List<Single>(value), BssomArrayElementType.Single)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<Double> value) : this(new List<Double>(value), BssomArrayElementType.Double)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<Decimal> value) : this(new List<Decimal>(value), BssomArrayElementType.Decimal)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<Guid> value) : this(new List<Guid>(value), BssomArrayElementType.Guid)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<DateTime> value) : this(new List<DateTime>(value), BssomArrayElementType.DateTime)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        public BssomArray(IEnumerable<Boolean> value) : this(new List<Boolean>(value), BssomArrayElementType.Boolean)
        {
        }

        /// <summary>
        /// Initializes array2 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<object> value, bool isCopy = true) : this(isCopy ? new List<object>(value) : value, BssomArrayElementType.BssomValue)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<byte> value, bool isCopy = true) : this(isCopy ? new List<byte>(value) : value, BssomArrayElementType.Byte)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<sbyte> value, bool isCopy = true) : this(isCopy ? new List<sbyte>(value) : value, BssomArrayElementType.SByte)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<Int16> value, bool isCopy = true) : this(isCopy ? new List<Int16>(value) : value, BssomArrayElementType.Int16)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<Int32> value, bool isCopy = true) : this(isCopy ? new List<Int32>(value) : value, BssomArrayElementType.Int32)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<Int64> value, bool isCopy = true) : this(isCopy ? new List<Int64>(value) : value, BssomArrayElementType.Int64)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<UInt16> value, bool isCopy = true) : this(isCopy ? new List<UInt16>(value) : value, BssomArrayElementType.UInt16)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<UInt32> value, bool isCopy = true) : this(isCopy ? new List<UInt32>(value) : value, BssomArrayElementType.UInt32)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<UInt64> value, bool isCopy = true) : this(isCopy ? new List<UInt64>(value) : value, BssomArrayElementType.UInt64)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<Char> value, bool isCopy = true) : this(isCopy ? new List<Char>(value) : value, BssomArrayElementType.Char)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<Single> value, bool isCopy = true) : this(isCopy ? new List<Single>(value) : value, BssomArrayElementType.Single)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<Double> value, bool isCopy = true) : this(isCopy ? new List<Double>(value) : value, BssomArrayElementType.Double)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<Decimal> value, bool isCopy = true) : this(isCopy ? new List<Decimal>(value) : value, BssomArrayElementType.Decimal)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<Guid> value, bool isCopy = true) : this(isCopy ? new List<Guid>(value) : value, BssomArrayElementType.Guid)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<DateTime> value, bool isCopy = true) : this(isCopy ? new List<DateTime>(value) : value, BssomArrayElementType.DateTime)
        {
        }

        /// <summary>
        /// Initializes array1 BssomArray
        /// </summary>
        /// <param name="isCopy">If true, a copy of <paramref name="value"/> is used</param>
        public BssomArray(List<Boolean> value, bool isCopy = true) : this(isCopy ? new List<Boolean>(value) : value, BssomArrayElementType.Boolean)
        {
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值不是BssomValue类型,则对该值创建BssomValue并返回</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is not of type BssomValue, create a BssomValue for the value and return</para>
        /// </summary>
        /// <returns>若对应的值不是BssomValue类型,则对该值创建BssomValue并返回. If the corresponding value is not of type BssomValue, create a BssomValue for the value and return</returns>
        public BssomValue this[int index]
        {
            get
            {
                var raw = GetRawValue(index);
                if (raw is BssomValue val)
                    return val;

                return BssomValue.Create(raw);
            }
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Int32类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Int32, there is no unboxing overhead</para>
        /// </summary>
        public Int32 GetInt32(int index)
        {
            if (ElementType == BssomArrayElementType.Int32)
                return Unsafe.As<IEnumerable, List<Int32>>(ref this._rawValue)[index];
            return (Int32)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Int64类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Int64, there is no unboxing overhead</para>
        /// </summary>
        public Int64 GetInt64(int index)
        {
            if (ElementType == BssomArrayElementType.Int64)
                return Unsafe.As<IEnumerable, List<Int64>>(ref this._rawValue)[index];
            return (Int64)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Int16类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Int16, there is no unboxing overhead</para>
        /// </summary>
        public Int16 GetInt16(int index)
        {
            if (ElementType == BssomArrayElementType.Int16)
                return Unsafe.As<IEnumerable, List<Int16>>(ref this._rawValue)[index];
            return (Int16)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是UInt32类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type UInt32, there is no unboxing overhead</para>
        /// </summary>
        public UInt32 GetUInt32(int index)
        {
            if (ElementType == BssomArrayElementType.UInt32)
                return Unsafe.As<IEnumerable, List<UInt32>>(ref this._rawValue)[index];
            return (UInt32)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是UInt16类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type UInt16, there is no unboxing overhead</para>
        /// </summary>
        public UInt16 GetUInt16(int index)
        {
            if (ElementType == BssomArrayElementType.UInt16)
                return Unsafe.As<IEnumerable, List<UInt16>>(ref this._rawValue)[index];
            return (UInt16)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是UInt64类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type UInt64, there is no unboxing overhead</para>
        /// </summary>
        public UInt64 GetUInt64(int index)
        {
            if (ElementType == BssomArrayElementType.UInt64)
                return Unsafe.As<IEnumerable, List<UInt64>>(ref this._rawValue)[index];
            return (UInt64)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Byte类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Byte, there is no unboxing overhead</para>
        /// </summary>
        public Byte GetByte(int index)
        {
            if (ElementType == BssomArrayElementType.Byte)
                return Unsafe.As<IEnumerable, List<Byte>>(ref this._rawValue)[index];
            return (Byte)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是SByte类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type SByte, there is no unboxing overhead</para>
        /// </summary>
        public SByte GetSByte(int index)
        {
            if (ElementType == BssomArrayElementType.SByte)
                return Unsafe.As<IEnumerable, List<SByte>>(ref this._rawValue)[index];
            return (SByte)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Char类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Char, there is no unboxing overhead</para>
        /// </summary>
        public Char GetChar(int index)
        {
            if (ElementType == BssomArrayElementType.Char)
                return Unsafe.As<IEnumerable, List<Char>>(ref this._rawValue)[index];
            return (Char)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Single类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Single, there is no unboxing overhead</para>
        /// </summary>
        public Single GetSingle(int index)
        {
            if (ElementType == BssomArrayElementType.Single)
                return Unsafe.As<IEnumerable, List<Single>>(ref this._rawValue)[index];
            return (Single)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Double类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Double, there is no unboxing overhead</para>
        /// </summary>
        public Double GetDouble(int index)
        {
            if (ElementType == BssomArrayElementType.Double)
                return Unsafe.As<IEnumerable, List<Double>>(ref this._rawValue)[index];
            return (Double)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Decimal类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Decimal, there is no unboxing overhead</para>
        /// </summary>
        public Decimal GetDecimal(int index)
        {
            if (ElementType == BssomArrayElementType.Decimal)
                return Unsafe.As<IEnumerable, List<Decimal>>(ref this._rawValue)[index];
            return (Decimal)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Guid类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Guid, there is no unboxing overhead</para>
        /// </summary>
        public Guid GetGuid(int index)
        {
            if (ElementType == BssomArrayElementType.Guid)
                return Unsafe.As<IEnumerable, List<Guid>>(ref this._rawValue)[index];
            return (Guid)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是DateTime类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type DateTime, there is no unboxing overhead</para>
        /// </summary>
        public DateTime GetDateTime(int index)
        {
            if (ElementType == BssomArrayElementType.DateTime)
                return Unsafe.As<IEnumerable, List<DateTime>>(ref this._rawValue)[index];
            return (DateTime)GetRawValue(index);
        }

        /// <summary>
        /// <para>根据Index在数组中获取对应的值,若值是Boolean类型,则没有拆箱开销</para>
        /// <para>Get the corresponding value in the array according to Index. If the value is of type Boolean, there is no unboxing overhead</para>
        /// </summary>
        public Boolean GetBoolean(int index)
        {
            if (ElementType == BssomArrayElementType.Boolean)
                return Unsafe.As<IEnumerable, List<Boolean>>(ref this._rawValue)[index];
            return (Boolean)GetRawValue(index);
        }

        /// <summary>
        /// <para>确定数组中特定项的索引</para>
        /// <para>Determines the index of a specific item in the array</para>
        /// </summary>
        /// <returns>如果在数组中没有找到项的索引则返回-1. The index of item if found in the array; otherwise, -1.</returns>
        public int IndexOf(object item)
        {
            switch (ElementType)
            {
                case BssomArrayElementType.Int32:
                    return Unsafe.As<IEnumerable, List<Int32>>(ref this._rawValue).IndexOf((Int32)item);
                case BssomArrayElementType.Int16:
                    return Unsafe.As<IEnumerable, List<Int16>>(ref this._rawValue).IndexOf((Int16)item);
                case BssomArrayElementType.Int64:
                    return Unsafe.As<IEnumerable, List<Int64>>(ref this._rawValue).IndexOf((Int64)item);
                case BssomArrayElementType.UInt64:
                    return Unsafe.As<IEnumerable, List<UInt64>>(ref this._rawValue).IndexOf((UInt64)item);
                case BssomArrayElementType.UInt32:
                    return Unsafe.As<IEnumerable, List<UInt32>>(ref this._rawValue).IndexOf((UInt32)item);
                case BssomArrayElementType.UInt16:
                    return Unsafe.As<IEnumerable, List<UInt16>>(ref this._rawValue).IndexOf((UInt16)item);
                case BssomArrayElementType.Byte:
                    return Unsafe.As<IEnumerable, List<Byte>>(ref this._rawValue).IndexOf((Byte)item);
                case BssomArrayElementType.SByte:
                    return Unsafe.As<IEnumerable, List<SByte>>(ref this._rawValue).IndexOf((SByte)item);
                case BssomArrayElementType.Char:
                    return Unsafe.As<IEnumerable, List<Char>>(ref this._rawValue).IndexOf((Char)item);
                case BssomArrayElementType.Single:
                    return Unsafe.As<IEnumerable, List<Single>>(ref this._rawValue).IndexOf((Single)item);
                case BssomArrayElementType.Double:
                    return Unsafe.As<IEnumerable, List<Double>>(ref this._rawValue).IndexOf((Double)item);
                case BssomArrayElementType.Decimal:
                    return Unsafe.As<IEnumerable, List<Decimal>>(ref this._rawValue).IndexOf((Decimal)item);
                case BssomArrayElementType.Guid:
                    return Unsafe.As<IEnumerable, List<Guid>>(ref this._rawValue).IndexOf((Guid)item);
                case BssomArrayElementType.DateTime:
                    return Unsafe.As<IEnumerable, List<DateTime>>(ref this._rawValue).IndexOf((DateTime)item);
                case BssomArrayElementType.Boolean:
                    return Unsafe.As<IEnumerable, List<Boolean>>(ref this._rawValue).IndexOf((Boolean)item);
                default://BssomValue
                    {
                        return Unsafe.As<IEnumerable, List<BssomValue>>(ref this._rawValue).IndexOf((BssomValue)item);
                    }
            }
        }

        /// <summary>
        /// <para>将项插入到数组的指定索引处</para>
        /// <para>Inserts an item to the array at the specified index</para>
        /// </summary>
        public void Insert(int index, object item)
        {
            switch (ElementType)
            {
                case BssomArrayElementType.Int32:
                    Unsafe.As<IEnumerable, List<Int32>>(ref this._rawValue).Insert(index, (Int32)item); break;
                case BssomArrayElementType.Int16:
                    Unsafe.As<IEnumerable, List<Int16>>(ref this._rawValue).Insert(index, (Int16)item); break;
                case BssomArrayElementType.Int64:
                    Unsafe.As<IEnumerable, List<Int64>>(ref this._rawValue).Insert(index, (Int64)item); break;
                case BssomArrayElementType.UInt64:
                    Unsafe.As<IEnumerable, List<UInt64>>(ref this._rawValue).Insert(index, (UInt64)item); break;
                case BssomArrayElementType.UInt32:
                    Unsafe.As<IEnumerable, List<UInt32>>(ref this._rawValue).Insert(index, (UInt32)item); break;
                case BssomArrayElementType.UInt16:
                    Unsafe.As<IEnumerable, List<UInt16>>(ref this._rawValue).Insert(index, (UInt16)item); break;
                case BssomArrayElementType.Byte:
                    Unsafe.As<IEnumerable, List<Byte>>(ref this._rawValue).Insert(index, (Byte)item); break;
                case BssomArrayElementType.SByte:
                    Unsafe.As<IEnumerable, List<SByte>>(ref this._rawValue).Insert(index, (SByte)item); break;
                case BssomArrayElementType.Char:
                    Unsafe.As<IEnumerable, List<Char>>(ref this._rawValue).Insert(index, (Char)item); break;
                case BssomArrayElementType.Single:
                    Unsafe.As<IEnumerable, List<Single>>(ref this._rawValue).Insert(index, (Single)item); break;
                case BssomArrayElementType.Double:
                    Unsafe.As<IEnumerable, List<Double>>(ref this._rawValue).Insert(index, (Double)item); break;
                case BssomArrayElementType.Decimal:
                    Unsafe.As<IEnumerable, List<Decimal>>(ref this._rawValue).Insert(index, (Decimal)item); break;
                case BssomArrayElementType.Guid:
                    Unsafe.As<IEnumerable, List<Guid>>(ref this._rawValue).Insert(index, (Guid)item); break;
                case BssomArrayElementType.DateTime:
                    Unsafe.As<IEnumerable, List<DateTime>>(ref this._rawValue).Insert(index, (DateTime)item); break;
                case BssomArrayElementType.Boolean:
                    Unsafe.As<IEnumerable, List<Boolean>>(ref this._rawValue).Insert(index, (Boolean)item); break;
                default://BssomValue
                    {
                        Unsafe.As<IEnumerable, List<BssomValue>>(ref this._rawValue).Insert(index, (BssomValue)item);
                        break;
                    }
            }
        }

        /// <summary>
        /// <para>移除指定索引处的元素</para>
        /// <para> Removes the array item at the specified index</para>
        /// </summary>
        public void RemoveAt(int index)
        {
            switch (ElementType)
            {
                case BssomArrayElementType.Int32:
                    Unsafe.As<IEnumerable, List<Int32>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.Int16:
                    Unsafe.As<IEnumerable, List<Int16>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.Int64:
                    Unsafe.As<IEnumerable, List<Int64>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.UInt64:
                    Unsafe.As<IEnumerable, List<UInt64>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.UInt32:
                    Unsafe.As<IEnumerable, List<UInt32>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.UInt16:
                    Unsafe.As<IEnumerable, List<UInt16>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.Byte:
                    Unsafe.As<IEnumerable, List<Byte>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.SByte:
                    Unsafe.As<IEnumerable, List<SByte>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.Char:
                    Unsafe.As<IEnumerable, List<Char>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.Single:
                    Unsafe.As<IEnumerable, List<Single>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.Double:
                    Unsafe.As<IEnumerable, List<Double>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.Decimal:
                    Unsafe.As<IEnumerable, List<Decimal>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.Guid:
                    Unsafe.As<IEnumerable, List<Guid>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.DateTime:
                    Unsafe.As<IEnumerable, List<DateTime>>(ref this._rawValue).RemoveAt(index); break;
                case BssomArrayElementType.Boolean:
                    Unsafe.As<IEnumerable, List<Boolean>>(ref this._rawValue).RemoveAt(index); break;
                default://BssomValue
                    Unsafe.As<IEnumerable, List<BssomValue>>(ref this._rawValue).RemoveAt(index);
                    break;
            }
        }

        /// <summary>
        /// <para> 添加一个元素到数组中.</para>
        /// <para> Adds an item to the array.</para>
        /// </summary>
        /// <param name="item"></param>
        public void Add(object item)
        {
            this.Insert(Count, item);
        }

        /// <summary>
        /// <para>清理数组中的所有元素.</para>
        /// <para>Clean up all elements in the array.</para>
        /// </summary>
        public void Clear()
        {
            Unsafe.As<List<int>>(_rawValue).Clear();
        }

        /// <summary>
        /// <para>判断在数组中该元素有没有被包含</para>
        /// <para>Determine whether the element is included in the array.</para>
        /// </summary>
        public bool Contains(object item)
        {
            return this.IndexOf(item) != -1;
        }

        /// <summary>
        /// <para>在数组中删除该元素</para>
        /// <para>Delete the element from the array</para>
        /// </summary>
        public bool Remove(object item)
        {
            var index = IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        void ICollection<object>.CopyTo(object[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException();

            if (array.Length - arrayIndex < Count)
                throw new Exception();//目标长度不够

            foreach (var item in _rawValue)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var item in _rawValue)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<BssomValue> IEnumerable<BssomValue>.GetEnumerator()
        {
            foreach (var item in _rawValue)
            {
                if (item is BssomValue bsVal)
                    yield return bsVal;
                else
                    yield return BssomValue.Create(item);
            }
        }

        private object GetRawValue(int index)
        {
            switch (ElementType)
            {
                case BssomArrayElementType.Int32:
                    return Unsafe.As<IEnumerable, List<Int32>>(ref this._rawValue)[index];
                case BssomArrayElementType.Int16:
                    return Unsafe.As<IEnumerable, List<Int16>>(ref this._rawValue)[index];
                case BssomArrayElementType.Int64:
                    return Unsafe.As<IEnumerable, List<Int64>>(ref this._rawValue)[index];
                case BssomArrayElementType.UInt64:
                    return Unsafe.As<IEnumerable, List<UInt64>>(ref this._rawValue)[index];
                case BssomArrayElementType.UInt32:
                    return Unsafe.As<IEnumerable, List<UInt32>>(ref this._rawValue)[index];
                case BssomArrayElementType.UInt16:
                    return Unsafe.As<IEnumerable, List<UInt16>>(ref this._rawValue)[index];
                case BssomArrayElementType.Byte:
                    return Unsafe.As<IEnumerable, List<Byte>>(ref this._rawValue)[index];
                case BssomArrayElementType.SByte:
                    return Unsafe.As<IEnumerable, List<SByte>>(ref this._rawValue)[index];
                case BssomArrayElementType.Char:
                    return Unsafe.As<IEnumerable, List<Char>>(ref this._rawValue)[index];
                case BssomArrayElementType.Single:
                    return Unsafe.As<IEnumerable, List<Single>>(ref this._rawValue)[index];
                case BssomArrayElementType.Double:
                    return Unsafe.As<IEnumerable, List<Double>>(ref this._rawValue)[index];
                case BssomArrayElementType.Decimal:
                    return Unsafe.As<IEnumerable, List<Decimal>>(ref this._rawValue)[index];
                case BssomArrayElementType.Guid:
                    return Unsafe.As<IEnumerable, List<Guid>>(ref this._rawValue)[index];
                case BssomArrayElementType.DateTime:
                    return Unsafe.As<IEnumerable, List<DateTime>>(ref this._rawValue)[index];
                case BssomArrayElementType.Boolean:
                    return Unsafe.As<IEnumerable, List<Boolean>>(ref this._rawValue)[index];
                default://BssomValue
                    return Unsafe.As<IEnumerable, List<BssomValue>>(ref this._rawValue)[index];
            }
        }

        private void SetRawValue(int index, object value)
        {
            switch (ElementType)
            {
                case BssomArrayElementType.Int32:
                    Unsafe.As<IEnumerable, List<Int32>>(ref this._rawValue)[index] = (int)value; break;
                case BssomArrayElementType.Int16:
                    Unsafe.As<IEnumerable, List<Int16>>(ref this._rawValue)[index] = (Int16)value; break;
                case BssomArrayElementType.Int64:
                    Unsafe.As<IEnumerable, List<Int64>>(ref this._rawValue)[index] = (Int64)value; break;
                case BssomArrayElementType.UInt64:
                    Unsafe.As<IEnumerable, List<UInt64>>(ref this._rawValue)[index] = (UInt64)value; break;
                case BssomArrayElementType.UInt32:
                    Unsafe.As<IEnumerable, List<UInt32>>(ref this._rawValue)[index] = (UInt32)value; break;
                case BssomArrayElementType.UInt16:
                    Unsafe.As<IEnumerable, List<UInt16>>(ref this._rawValue)[index] = (UInt16)value; break;
                case BssomArrayElementType.Byte:
                    Unsafe.As<IEnumerable, List<Byte>>(ref this._rawValue)[index] = (Byte)value; break;
                case BssomArrayElementType.SByte:
                    Unsafe.As<IEnumerable, List<SByte>>(ref this._rawValue)[index] = (SByte)value; break;
                case BssomArrayElementType.Char:
                    Unsafe.As<IEnumerable, List<Char>>(ref this._rawValue)[index] = (Char)value; break;
                case BssomArrayElementType.Single:
                    Unsafe.As<IEnumerable, List<Single>>(ref this._rawValue)[index] = (Single)value; break;
                case BssomArrayElementType.Double:
                    Unsafe.As<IEnumerable, List<Double>>(ref this._rawValue)[index] = (Double)value; break;
                case BssomArrayElementType.Decimal:
                    Unsafe.As<IEnumerable, List<Decimal>>(ref this._rawValue)[index] = (Decimal)value; break;
                case BssomArrayElementType.Guid:
                    Unsafe.As<IEnumerable, List<Guid>>(ref this._rawValue)[index] = (Guid)value; break;
                case BssomArrayElementType.DateTime:
                    Unsafe.As<IEnumerable, List<DateTime>>(ref this._rawValue)[index] = (DateTime)value; break;
                case BssomArrayElementType.Boolean:
                    Unsafe.As<IEnumerable, List<Boolean>>(ref this._rawValue)[index] = (Boolean)value; break;
                default://BssomValue
                    Unsafe.As<IEnumerable, List<BssomValue>>(ref this._rawValue)[index] = (BssomValue)value; break;
            }
        }
    }
}
