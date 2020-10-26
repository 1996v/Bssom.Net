using Bssom.Serializer.Formatters;
using Bssom.Serializer.Internal;
using System;
using System.Collections.Generic;

namespace Bssom.Serializer.Resolvers
{
    /// <summary>
    /// <para>获取<see cref ="BssomValue"/>的类型的<see cref="IBssomFormatter"/></para>
    /// <para>Gets <see cref="IBssomFormatter"/> of the type <see cref="BssomValue"/></para>
    /// </summary>
    public sealed class BssomValueResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly BssomValueResolver Instance = new BssomValueResolver();

        public IBssomFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }
        private static class FormatterCache<T>
        {
            public static readonly IBssomFormatter<T> Formatter;

            static FormatterCache()
            {
                Type t = typeof(T);
                if (BssomValueResolverGetFormatterHelper.TryGetFormatter(t, out IBssomFormatter formatter))
                {
                    Formatter = (IBssomFormatter<T>)formatter;
                    return;
                }
            }
        }
    }
}
namespace Bssom.Serializer.Internal
{
    internal static class BssomValueResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, IBssomFormatter> FormatterMap = new Dictionary<Type, IBssomFormatter>()
        {
            { typeof(BssomNumber), BssomNumberFormatter.Instance },
            { typeof(BssomArray), BssomArrayFormatter.Instance },
            { typeof(BssomString), BssomStringFormatter.Instance },
            { typeof(BssomNull),BssomNullFormatter.Instance },
            { typeof(BssomMap),BssomMapFormatter.Instance },
            { typeof(BssomValue),BssomValueFormatter.Instance },
            { typeof(BssomGuid), BssomGuidFormatter.Instance },
            { typeof(BssomFloat), BssomFloatFormatter.Instance },
            { typeof(BssomDecimal), BssomDecimalFormatter.Instance },
            { typeof(BssomBoolean), BssomBooleanFormatter.Instance },
            { typeof(BssomDateTime), BssomDateTimeFormatter.Instance },
            { typeof(BssomChar), BssomCharFormatter.Instance },
        };

        internal static bool TryGetFormatter(Type t, out IBssomFormatter formatter)
        {
            if (FormatterMap.TryGetValue(t, out formatter))
            {
                return true;
            }

            return false;
        }
    }
}
