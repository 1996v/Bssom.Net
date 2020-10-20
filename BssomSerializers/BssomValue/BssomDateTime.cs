using System;

namespace BssomSerializers
{
    /// <summary>
    /// Represents a BSSOM DateTime.
    /// </summary>
    public class BssomDateTime : BssomValue
    {
        private object _dateTime;

        internal BssomDateTime(object value)
        {
            _dateTime = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomDateTime"/> class.
        /// </summary>
        public BssomDateTime(DateTime value)
        {
            _dateTime = value;
        }
        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.DateTime;
        /// <summary>
        /// Raw value
        /// </summary>
        public override object RawValue => _dateTime;
        /// <summary>
        /// Get DateTime
        /// </summary>
        public DateTime GetDateTime() => (DateTime)_dateTime;

        public override bool Equals(object obj)
        {
            return obj is DateTime raw && raw == GetDateTime() || 
                obj is BssomDateTime bssom && bssom.GetDateTime() == this.GetDateTime();
        }

        public override int GetHashCode()
        {
            return GetDateTime().GetHashCode();
        }
    }
}
