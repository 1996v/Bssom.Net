using System;

namespace Bssom.Serializer
{
    /// <summary>
    /// Represents a BSSOM Guid.
    /// </summary>
    public class BssomGuid : BssomValue
    {
        private object _guid;

        internal BssomGuid(object value)
        {
            _guid = value;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BssomGuid"/> class.
        /// </summary>
        public BssomGuid(Guid value)
        {
            _guid = value;
        }
        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.Guid;
        /// <summary>
        /// Raw value
        /// </summary>
        public override object RawValue => _guid;
        /// <summary>
        /// Get Guid
        /// </summary>
        public Guid GetGuid()
        {
            return (Guid)_guid;
        }

        public override bool Equals(object obj)
        {
            return obj is Guid raw && raw == GetGuid() ||
                obj is BssomGuid bssom && bssom.GetGuid() == GetGuid();
        }

        public override int GetHashCode()
        {
            return GetGuid().GetHashCode();
        }
    }
}
