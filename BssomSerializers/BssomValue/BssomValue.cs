using Bssom.Serializer.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Bssom.Serializer.BssomFloat;
using static Bssom.Serializer.BssomNumber;

namespace Bssom.Serializer
{
    /// <summary>
    /// Represents a BSSOM array.
    /// </summary>
    public abstract class BssomValue
    {
        /// <summary>
        /// The value of bssom raw
        /// </summary>
        public virtual object RawValue { get; }

        /// <summary>
        /// Bssom value type
        /// </summary>
        public abstract BssomValueType ValueType { get; }//改名分类也可以category

        public override string ToString()
        {
            return RawValue.ToString();
        }

        /// <summary>
        /// Create a BssomValue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BssomValue Create(object value)
        {
            if (value == null) return BssomNull.Value;

            if (value is Int16) return new BssomNumber(value, BssomNumber.BssomNumberType.Short);
            else if (value is Int32) return new BssomNumber(value, BssomNumber.BssomNumberType.Int);
            else if (value is Int64) return new BssomNumber(value, BssomNumber.BssomNumberType.Long);
            else if (value is UInt16) return new BssomNumber(value, BssomNumber.BssomNumberType.UShort);
            else if (value is UInt32) return new BssomNumber(value, BssomNumber.BssomNumberType.UInt);
            else if (value is UInt64) return new BssomNumber(value, BssomNumber.BssomNumberType.ULong);
            else if (value is Byte) return new BssomNumber(value, BssomNumber.BssomNumberType.Byte);
            else if (value is SByte) return new BssomNumber(value, BssomNumber.BssomNumberType.SByte);
            else if (value is Boolean) return new BssomBoolean(value);
            else if (value is String) return new BssomString(value);
            else if (value is Single) return new BssomFloat(value, BssomFloat.BssomFloatType.Single);
            else if (value is Double) return new BssomFloat(value, BssomFloat.BssomFloatType.Double);
            else if (value is Decimal) return new BssomDecimal(value);
            else if (value is DateTime) return new BssomDateTime(value);
            else if (value is Char) return new BssomChar(value);
            else if (value is Guid) return new BssomGuid(value);
            else if (value is BssomValue bsv) return BssomValue.Create(bsv.RawValue);
            else
            {
                var enumerable = value as IEnumerable;
                var dictionary = value as IDictionary;

                if (dictionary != null)
                {
                    return new BssomMap(((IDictionary)value).GetGetEnumerator());
                }
                else if (enumerable != null)
                {
                    return new BssomArray((IEnumerable)value);
                }
                else
                {
                    Dictionary<object, object> mapcore = new Dictionary<object, object>();
                    foreach (var item in value.GetPublicMembersWithDynamicObject())
                    {
                        mapcore.Add(item.Key, item.Value);
                    }
                    return new BssomMap(mapcore);
                }
            }
        }
    }
}
