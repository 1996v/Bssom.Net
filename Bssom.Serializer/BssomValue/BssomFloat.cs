namespace Bssom.Serializer
{
    /// <summary>
    /// Represents a BSSOM Float.
    /// </summary>
    public class BssomFloat : BssomValue
    {
        public enum BssomFloatType
        {
            Single,
            Double
        }
        private object _float;

        internal BssomFloat(object value, BssomFloatType type)
        {
            this._float = value;
            this.FloatType = type;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BssomFloat"/> class.
        /// </summary>
        public BssomFloat(float value) : this(value, BssomFloatType.Single)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BssomFloat"/> class.
        /// </summary>
        public BssomFloat(double value) : this(value, BssomFloatType.Double)
        {
        }

        public BssomFloatType FloatType { get; }
        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.Float;
        /// <summary>
        /// Raw value
        /// </summary>
        public override object RawValue => _float;
        /// <summary>
        /// Get Float32
        /// </summary>
        public float GetFloat32()
        {
            return (float)_float;
        }
        /// <summary>
        /// Get Float64
        /// </summary>
        public double GetFloat64()
        {
            return (double)_float;
        }

        public override bool Equals(object obj)
        {
            switch (FloatType)
            {
                case BssomFloatType.Single:
                    {
                        return obj is float raw && raw == GetFloat32() ||
                              obj is BssomFloat bssom && bssom.FloatType == this.FloatType && bssom.GetFloat32() == GetFloat32();
                    }
                default://case BssomFloatType.Double:
                    {
                        return obj is double raw && raw == GetFloat64() ||
                                  obj is BssomFloat bssom && bssom.FloatType == this.FloatType && bssom.GetFloat64() == GetFloat64();
                    }
            }
        }

        public override int GetHashCode()
        {
            switch (FloatType)
            {
                case BssomFloatType.Single:
                    return GetFloat32().GetHashCode();
                default://case BssomFloatType.Double:
                    return GetFloat64().GetHashCode();
            }
        }
    }
}
