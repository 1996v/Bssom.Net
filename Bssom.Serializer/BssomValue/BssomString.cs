using System;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer
{
    /// <summary>
    /// Represents a BSSOM string
    /// </summary>
    public class BssomString : BssomValue
    {
        private string _value;

        internal BssomString(object str)
        {
            _value = (string)str;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomString"/> class.
        /// </summary>
        public BssomString(string str)
        {
            _value = str;
        }
        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.String;
        /// <summary>
        /// Raw value 
        /// </summary>
        public override object RawValue => _value;
        /// <summary>
        /// The representation of raw Value
        /// </summary>
        public String Value => _value;
        /// <summary>
        /// Get string
        /// </summary>
        public string GetString() => _value;

        public override bool Equals(object obj)
        {
            return obj is string raw && raw == GetString() || 
                obj is BssomString bssom && bssom.GetString() == this.GetString();
        }

        public override int GetHashCode()
        {
            return GetString().GetHashCode();
        }
    }
}
