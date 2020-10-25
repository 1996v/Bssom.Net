//using System.Runtime.CompilerServices;

using Bssom.Serializer.Binary;

namespace Bssom.Serializer
{
    /// <summary>
    /// Bssom native type , for .net platform
    /// </summary>
    public static class NativeBssomType
    {
        /// <summary>
        /// Utf16, Fixed occupy 2 bytes
        /// </summary>
        public const byte CharCode = BssomBinaryPrimitives.CharSize;
        /// <summary>
        /// .Net Guid, Fixed occupy 17 bytes
        /// </summary>
        public const byte GuidCode = BssomBinaryPrimitives.GuidSize;
        /// <summary>
        /// .Net Decimal, Fixed occupy 16 bytes
        /// </summary>
        public const byte DecimalCode = BssomBinaryPrimitives.DecimalSize;
        /// <summary>
        /// .Net Decimal, Fixed occupy 8 bytes
        /// </summary>
        public const byte DateTimeCode = BssomBinaryPrimitives.NativeDateTimeSize;

        internal const int MaxCode = GuidCode;
    }

}
