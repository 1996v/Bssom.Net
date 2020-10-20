namespace BssomSerializers
{
    /// <summary>
    /// Represents a BSSOM Decimal.
    /// </summary>
    public class BssomDecimal : BssomValue
    {
        private object _decimal;

        internal BssomDecimal(object value)
        {
            _decimal = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomDecimal"/> class.
        /// </summary>
        public BssomDecimal(decimal value)
        {
            _decimal = value;
        }
        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.Decimal;
        /// <summary>
        /// Raw value
        /// </summary>
        public override object RawValue => _decimal;
        /// <summary>
        /// Get Decimal
        /// </summary>
        public decimal GetDecimal() => (decimal)_decimal;

        public override bool Equals(object obj)
        {
            return obj is decimal raw && raw == GetDecimal() || 
                obj is BssomDecimal bssom && bssom.GetDecimal() == this.GetDecimal();
        }

        public override int GetHashCode()
        {
            return GetDecimal().GetHashCode();
        }
    }
}
