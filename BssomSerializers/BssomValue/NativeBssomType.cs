//using System.Runtime.CompilerServices;

namespace BssomSerializers
{
    /// <summary>
    /// Bssom native type , for .net platform
    /// </summary>
    public static class NativeBssomType
    {
        /// <summary>
        /// Utf16, Fixed occupy 2 bytes
        /// </summary>
        public const byte CharCode = 0;
        /// <summary>
        /// .Net Guid, Fixed occupy 16 bytes
        /// </summary>
        public const byte GuidCode = 1;
        /// <summary>
        /// .Net Decimal, Fixed occupy 16 bytes
        /// </summary>
        public const byte DecimalCode = 2;
        /// <summary>
        /// .Net Decimal, Fixed occupy 8 bytes
        /// </summary>
        public const byte DateTimeCode = 3;

        internal const int MinCode = CharCode;
        internal const int MaxCode = DateTimeCode;
    }

}
