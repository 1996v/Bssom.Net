//using System.Runtime.CompilerServices;

using Bssom.Serializer.Formatters;
using Bssom.Serializer.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

namespace Bssom.Serializer.Resolvers
{
    /// <summary>
    /// Provides formatters for <see cref="StringDictionary"/>,<see cref="StringBuilder"/>,<see cref="BitArray"/>,<see cref="NameValueCollection"/>,<see cref="Version"/>,<see cref="Uri"/>,<see cref="TimeSpan"/>,<see cref="DBNull"/>,<see cref="DataTable"/> , Provides nullable inline formatters for types in the <see cref="PrimitiveResolver"/>
    /// </summary>
    public sealed class BuildInResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly BuildInResolver Instance = new BuildInResolver();

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

                if (BuildInResolverGetFormatterHelper.TryGetFormatter(t, out IBssomFormatter formatter))
                {
                    Formatter = (IBssomFormatter<T>)formatter;
                    return;
                }

                if (t.IsEnum)
                {
                    Formatter = EnumFormatter<T>.Instance;
                    return;
                }

                if (t.IsGenericType)
                {
                    Type genericType = t.GetGenericTypeDefinition();
                    if (genericType == typeof(Nullable<>))
                    {
                        Formatter = (IBssomFormatter<T>)Activator.CreateInstance(typeof(NullableFormatter<>).MakeGenericType(t.GetGenericArguments()));
                        return;
                    }
                    else if (genericType == typeof(Lazy<>))
                    {
                        Formatter = (IBssomFormatter<T>)Activator.CreateInstance(typeof(LazyFormatter<>).MakeGenericType(t.GetGenericArguments()));
                        return;
                    }
                    else if (genericType == typeof(IGrouping<,>))
                    {
                        Formatter = (IBssomFormatter<T>)Activator.CreateInstance(typeof(InterfaceGroupingFormatter<,>).MakeGenericType(t.GetGenericArguments()));
                        return;
                    }
                    else if (genericType == typeof(ILookup<,>))
                    {
                        Formatter = (IBssomFormatter<T>)Activator.CreateInstance(typeof(InterfaceILookupFormatter<,>).MakeGenericType(t.GetGenericArguments()));
                        return;
                    }
                }

                if (t.IsAnonymousType())
                {
                    Formatter = AnonymousTypeFormatter<T>.Instance;
                    return;
                }
            }
        }
    }
}
namespace Bssom.Serializer.Internal
{
    internal static class BuildInResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, IBssomFormatter> FormatterMap = new Dictionary<Type, IBssomFormatter>()
        {
            { typeof(StringDictionary), StringDictionaryFormatter.Instance },
            { typeof(StringBuilder), StringBuilderFormatter.Instance },
            { typeof(BitArray), BitArrayFormatter.Instance },
            { typeof(NameValueCollection),NameValueCollectionFormatter.Instance },
            { typeof(Version),VersionFormatter.Instance },
            { typeof(Uri),UriFormatter.Instance },
            { typeof(TimeSpan), TimeSpanFormatter.Instance },
            { typeof(DBNull), DBNullFormatter.Instance },
            { typeof(DataTable), DataTableFormatter.Instance },

            { typeof(sbyte?), StaticNullableInt8Formatter.Instance },
            { typeof(Int16?), StaticNullableInt16Formatter.Instance },
            { typeof(Int32?), StaticNullableInt32Formatter.Instance },
            { typeof(Int64?), StaticNullableInt64Formatter.Instance },
            { typeof(byte?), StaticNullableUInt8Formatter.Instance },
            { typeof(UInt16?), StaticNullableUInt16Formatter.Instance },
            { typeof(UInt32?), StaticNullableUInt32Formatter.Instance },
            { typeof(UInt64?), StaticNullableUInt64Formatter.Instance },
            { typeof(Single?), StaticNullableFloat32Formatter.Instance },
            { typeof(Double?), StaticNullableFloat64Formatter.Instance },
            { typeof(bool?), StaticNullableBooleanFormatter.Instance },
            { typeof(char?), StaticNullableCharFormatter.Instance },
            { typeof(Guid?), StaticNullableGuidFormatter.Instance },
            { typeof(Decimal?), StaticNullableDecimalFormatter.Instance },
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
