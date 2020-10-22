//using System.Runtime.CompilerServices;

using System;
using System.Reflection;

namespace Bssom.Serializer.Resolver
{
    /// <summary>
    /// <para>获取<see cref ="BssomFormatterAttribute"/>的格式化程序</para>
    /// <para>Get formatter from <see cref="BssomFormatterAttribute"/></para>
    /// </summary>
    public sealed class AttributeFormatterResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly AttributeFormatterResolver Instance = new AttributeFormatterResolver();

        private AttributeFormatterResolver()
        {
        }

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

                BssomFormatterAttribute attr = t.GetCustomAttribute<BssomFormatterAttribute>();

                if (attr == null)
                {
                    return;
                }

                var formatterType = attr.FormatterType;
                if (formatterType.IsGenericType && !formatterType.IsConstructedGenericType)
                {
                    formatterType = formatterType.MakeGenericType(t.GetGenericArguments());
                }

                if (formatterType != t) throw BssomSerializationTypeFormatterException.AttributeFormatterTypeMismatch(formatterType, t);

                if (attr.Arguments == null)
                {
                    Formatter = (IBssomFormatter<T>)Activator.CreateInstance(formatterType);
                }
                else
                {
                    Formatter = (IBssomFormatter<T>)Activator.CreateInstance(formatterType, attr.Arguments);
                }
            }
        }
    }
}
