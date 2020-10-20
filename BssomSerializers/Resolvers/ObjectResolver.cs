using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
using BssomSerializers.BssMap;
using BssomSerializers.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BssomSerializers.Formatters;
namespace BssomSerializers.Resolver
{
    /// <summary>
    /// <para>获取object类型的<see cref="IBssomFormatter"/></para>
    /// <para>Gets the <see cref="IBssomFormatter"/> of type object</para>
    /// </summary>
    public sealed class ObjectResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly ObjectResolver Instance = new ObjectResolver();

        public IBssomFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }
        private static class FormatterCache<T>
        {
            public static readonly IBssomFormatter<T> Formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                    Formatter = (IBssomFormatter<T>)(IBssomFormatter)ObjectFormatter.Instance;
            }
        }
    }
}
