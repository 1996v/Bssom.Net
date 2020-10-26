namespace Bssom.Serializer
{
    /// <summary>
    /// Represents a BSSOM char.
    /// </summary>
    public class BssomChar : BssomValue
    {
        private object _char;

        internal BssomChar(object value)
        {
            _char = value;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BssomChar"/> class.
        /// </summary>
        public BssomChar(char value)
        {
            _char = value;
        }
        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.Char;
        /// <summary>
        /// Raw value
        /// </summary>
        public override object RawValue => _char;
        /// <summary>
        /// Get char
        /// </summary>
        public char GetChar()
        {
            return (char)_char;
        }

        public override bool Equals(object obj)
        {
            return obj is char raw && raw == GetChar() ||
                obj is BssomChar bssom && bssom.GetChar() == GetChar();
        }

        public override int GetHashCode()
        {
            return GetChar().GetHashCode();
        }
    }
}
