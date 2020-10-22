//using System.Runtime.CompilerServices;

using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Resolver;
using System;
using System.Collections.Generic;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Internal;

namespace Bssom.Serializer.Resolver
{
    /// <summary>
    /// Provides formatters for <see cref="sbyte"/>,<see cref="Int16"/>,<see cref="Int32"/>,<see cref="Int64"/>,<see cref="byte"/>,<see cref="UInt16"/>,<see cref="UInt32"/>,<see cref="UInt64"/>,<see cref="Single"/>,<see cref="Double"/>,<see cref="bool"/>,<see cref="char"/>,<see cref="Guid"/>,<see cref="Decimal"/>,<see cref="string"/>,<see cref="DateTime"/>
    /// </summary>
    public sealed class PrimitiveResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly PrimitiveResolver Instance = new PrimitiveResolver();

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

                if (PrimitiveResolverGetFormatterHelper.TryGetFormatter(t, out var formatter))
                {
                    Formatter = (IBssomFormatter<T>)formatter;
                }
            }
        }
    }
}
namespace Bssom.Serializer.Internal
{
    internal static class PrimitiveResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, IBssomFormatter> FormatterMap = new Dictionary<Type, IBssomFormatter>()
        {
            { typeof(sbyte), Int8Formatter.Instance },
            { typeof(Int16), Int16Formatter.Instance },
            { typeof(Int32), Int32Formatter.Instance },
            { typeof(Int64), Int64Formatter.Instance },
            { typeof(byte), UInt8Formatter.Instance },
            { typeof(UInt16), UInt16Formatter.Instance },
            { typeof(UInt32), UInt32Formatter.Instance },
            { typeof(UInt64), UInt64Formatter.Instance },
            { typeof(Single), Float32Formatter.Instance },
            { typeof(Double), Float64Formatter.Instance },
            { typeof(bool), BooleanFormatter.Instance },
            { typeof(string), StringFormatter.Instance },
            { typeof(DateTime), DateTimeFormatter.Instance },

            //native
            { typeof(char), CharFormatter.Instance },
            { typeof(Guid), GuidFormatter.Instance },
            { typeof(Decimal), DecimalFormatter.Instance },
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