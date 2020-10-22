namespace Bssom.Serializer
{
    /// <summary>
    /// Represents BssomValueType
    /// </summary>
    public enum BssomValueType
    {
        /// <summary>
        /// Bssom PrimitiveType, is <see cref="BssomType.NullCode"/>
        /// </summary>
        Null,

        /// <summary>
        /// Bssom PrimitiveType, include <see cref="BssomType.Int16Code"/>,<see cref="BssomType.Int32Code"/>,<see cref="BssomType.Int64Code"/>,<see cref="BssomType.UInt16Code"/>,<see cref="BssomType.UInt32Code"/>,<see cref="BssomType.UInt64Code"/>
        /// </summary>
        Number,

        /// <summary>
        /// Bssom PrimitiveType, include <see cref="BssomType.Float32Code"/>,<see cref="BssomType.Float64Code"/>
        /// </summary>
        Float,

        /// <summary>
        /// Bssom PrimitiveType, is <see cref="BssomType.BooleanCode"/>
        /// </summary>
        Boolean,

        /// <summary>
        /// Bssom PrimitiveType, is <see cref="BssomType.StringCode"/>
        /// </summary>
        String,

        /// <summary>
        /// Bssom PrimitiveType, include <see cref="BssomType.Array1"/>,<see cref="BssomType.Array2"/>
        /// </summary>
        Array,

        /// <summary>
        /// Bssom PrimitiveType, include <see cref="BssomType.Map1"/>,<see cref="BssomType.Map2"/>
        /// </summary>
        Map,

        /// <summary>
        /// This type has two representations in marshal
        /// <para>1. Bssom PrimitiveType, is <see cref="BssomType.TimestampCode"/> , Binary uses standard dynamic length format</para>
        /// <para>2. Bssom NativeType <see cref="BssomType.NativeCode"/>, NativeExtensionType is <see cref="NativeBssomType.DateTimeCode"/> , using the fixed binary format of the .net platform </para>
        /// </summary>
        DateTime,

        /// <summary>
        /// Bssom NativeType <see cref="BssomType.NativeCode"/>, NativeExtensionType is <see cref="NativeBssomType.DecimalCode"/>
        /// </summary>
        Decimal,

        /// <summary>
        /// Bssom NativeType <see cref="BssomType.NativeCode"/>, NativeExtensionType is <see cref="NativeBssomType.GuidCode"/>
        /// </summary>
        Guid,

        /// <summary>
        /// Bssom NativeType <see cref="BssomType.NativeCode"/>, NativeExtensionType is <see cref="NativeBssomType.CharCode"/>
        /// </summary>
        Char,
    }
}
