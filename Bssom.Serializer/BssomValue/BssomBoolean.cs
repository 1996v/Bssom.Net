using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
namespace Bssom.Serializer
{
    /// <summary>
    /// Represents a BSSOM boolean.
    /// </summary>
    public class BssomBoolean : BssomValue
    {
        private object _boolean;

        internal BssomBoolean(object value)
        {
            _boolean = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomBoolean"/> class.
        /// </summary>
        public BssomBoolean(bool value)
        {
            _boolean = value;
        }

        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.Boolean;
        /// <summary>
        /// Raw value
        /// </summary>
        public override object RawValue => _boolean;
        /// <summary>
        /// Get boolean
        /// </summary>
        public bool GetBoolean() => (bool)_boolean;

        public override bool Equals(object obj)
        {
            return obj is bool raw && raw == GetBoolean() ||
                obj is BssomBoolean bssom && bssom.GetBoolean() == this.GetBoolean();
        }

        public override int GetHashCode()
        {
            return GetBoolean().GetHashCode();
        }
    }
}
