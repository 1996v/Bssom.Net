using System;

namespace Bssom.Serializer
{
    /// <summary>
    /// Represents a BSSOM number
    /// </summary>
    public class BssomNumber : BssomValue
    {
        public enum BssomNumberType
        {
            Byte,
            SByte,
            Short,
            Int,
            Long,
            UShort,
            UInt,
            ULong,
        }
        private object _number;

        /// <summary>
        /// Number type
        /// </summary>
        public BssomNumberType NumberType { get; }

        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.Number;

        /// <summary>
        /// Raw value 
        /// </summary>
        public override object RawValue => _number;

        internal BssomNumber(object value, BssomNumberType numberType)
        {
            _number = value;
            NumberType = numberType;
        }

        public BssomNumber(byte value) : this(value, BssomNumberType.Byte)
        {
        }
        public BssomNumber(sbyte value) : this(value, BssomNumberType.SByte)
        {
        }
        public BssomNumber(Int16 value) : this(value, BssomNumberType.Short)
        {
        }
        public BssomNumber(Int32 value) : this(value, BssomNumberType.Int)
        {
        }
        public BssomNumber(Int64 value) : this(value, BssomNumberType.Long)
        {
        }
        public BssomNumber(UInt16 value) : this(value, BssomNumberType.UShort)
        {
        }
        public BssomNumber(UInt32 value) : this(value, BssomNumberType.UInt)
        {
        }
        public BssomNumber(UInt64 value) : this(value, BssomNumberType.ULong)
        {
        }

        public byte GetByte()
        {
            return (byte)_number;
        }

        public sbyte GetSByte()
        {
            return (sbyte)_number;
        }

        public short GetShort()
        {
            return (short)_number;
        }

        public int GetInt()
        {
            return (int)_number;
        }

        public long GetLong()
        {
            return (long)_number;
        }

        public ushort GetUShort()
        {
            return (ushort)_number;
        }

        public uint GetUInt()
        {
            return (uint)_number;
        }

        public ulong GetULong()
        {
            return (ulong)_number;
        }

        public override bool Equals(object obj)
        {
            switch (NumberType)
            {
                case BssomNumberType.Byte:
                    {
                        return obj is byte raw && raw == GetByte() ||
                                obj is BssomNumber bssom && bssom.NumberType == NumberType && bssom.GetByte() == GetByte();
                    }
                case BssomNumberType.SByte:
                    {
                        return obj is sbyte raw && raw == GetSByte() ||
                                  obj is BssomNumber bssom && bssom.NumberType == NumberType && bssom.GetSByte() == GetSByte();
                    }

                case BssomNumberType.Short:
                    {
                        return obj is short raw && raw == GetShort() ||
                                  obj is BssomNumber bssom && bssom.NumberType == NumberType && bssom.GetShort() == GetShort();
                    }
                case BssomNumberType.Int:
                    {
                        return obj is int raw && raw == GetInt() ||
                                  obj is BssomNumber bssom && bssom.NumberType == NumberType && bssom.GetInt() == GetInt();
                    }
                case BssomNumberType.Long:
                    {
                        return obj is long raw && raw == GetLong() ||
                                  obj is BssomNumber bssom && bssom.NumberType == NumberType && bssom.GetLong() == GetLong();
                    }
                case BssomNumberType.UShort:
                    {
                        return obj is ushort raw && raw == GetUShort() ||
                                  obj is BssomNumber bssom && bssom.NumberType == NumberType && bssom.GetUShort() == GetUShort();
                    }
                case BssomNumberType.UInt:
                    {
                        return obj is uint raw && raw == GetUInt() ||
                                  obj is BssomNumber bssom && bssom.NumberType == NumberType && bssom.GetUInt() == GetUInt();
                    }
                default:// case BssomNumberType.ULong:
                    {
                        return obj is ulong raw && raw == GetULong() ||
                                  obj is BssomNumber bssom && bssom.NumberType == NumberType && bssom.GetULong() == GetULong();
                    }
            }
        }

        public override int GetHashCode()
        {
            switch (NumberType)
            {
                case BssomNumberType.Byte:
                    return GetByte().GetHashCode();
                case BssomNumberType.SByte:
                    return GetSByte().GetHashCode();
                case BssomNumberType.Short:
                    return GetShort().GetHashCode();
                case BssomNumberType.Int:
                    return GetInt().GetHashCode();
                case BssomNumberType.Long:
                    return GetLong().GetHashCode();
                case BssomNumberType.UShort:
                    return GetUShort().GetHashCode();
                case BssomNumberType.UInt:
                    return GetUInt().GetHashCode();
                default://case BssomNumberType.ULong:
                    return GetULong().GetHashCode();
            }
        }
    }
}
