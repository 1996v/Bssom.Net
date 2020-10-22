namespace Bssom.Serializer
{
    /// <summary>
    /// Represents a BSSOM null
    /// </summary>
    public class BssomNull : BssomValue
    {
        /// <summary>
        /// Default instance
        /// </summary>
        public static readonly BssomNull Value = new BssomNull();

        private BssomNull() { }

        /// <summary>
        /// Raw value , is null
        /// </summary>
        public override object RawValue => null;

        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.Null;

        public override string ToString()
        {
            return "Null";
        }
    }
}
