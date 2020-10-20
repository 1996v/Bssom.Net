using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
using BssomSerializers.BssMap;
using BssomSerializers.Resolver;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace BssomSerializers
{
    /// <summary>
    /// <para>对特定的类型查找一个格式化器用来序列化和反序列化</para>
    /// <para>Find a Formatter for a particular type to serialize and deserialize</para>
    /// </summary>
    public interface IFormatterResolver
    {
        /// <summary>
        /// <para>获取一个<see cref ="IBssomFormatter{T}" />实例，该实例可以序列化或反序列化某些类型<typeparamref name ="T"/></para>
        /// <para>Gets an <see cref="IBssomFormatter{T}"/> instance that can serialize or deserialize some type <typeparamref name="T"/></para>
        /// </summary>
        /// <typeparam name="T">要序列化或反序列化的值的类型. The type of value to be serialized or deserialized</typeparam>
        /// <returns>
        /// <para>如果解析器提供了该类型的格式化器,则返回它,否则返回Null</para>
        /// <para>If the resolver provides a formatter of that type, return it, otherwise return Null</para>
        /// </returns>
        IBssomFormatter<T> GetFormatter<T>();
    }

    public static class FormatterResolverExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBssomFormatter<T> GetFormatterWithVerify<T>(this IFormatterResolver resolver)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            IBssomFormatter<T> formatter;
            try
            {
                formatter = resolver.GetFormatter<T>();
            }
            catch (TypeInitializationException ex)
            {
                return Throw<T>(ex);
            }

            if (formatter == null)
                Throw(typeof(T), resolver);

            return formatter;
        }

        private static IBssomFormatter<T> Throw<T>(TypeInitializationException ex)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
            return default;
        }

        private static void Throw(Type type, IFormatterResolver resolver)
        {
            BssomSerializationTypeFormatterException.ThrowFormatterNotRegistered(type, resolver);
        }

        private static readonly ConcurrentDictionary<Type, Func<IFormatterResolver, IBssomFormatter>> FormatterGetters = new ConcurrentDictionary<Type, Func<IFormatterResolver, IBssomFormatter>>();
        private static readonly MethodInfo GetFormatterMethodInfo = typeof(IFormatterResolver).GetMethod(nameof(IFormatterResolver.GetFormatter), Type.EmptyTypes);

        public static IBssomFormatter GetFormatterWithVerify(this IFormatterResolver resolver, Type type)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!FormatterGetters.TryGetValue(type, out var formatterGetter))
            {
                var genericMethod = GetFormatterMethodInfo.MakeGenericMethod(type);
                var inputResolver = Expression.Parameter(typeof(IFormatterResolver), "inputResolver");
                formatterGetter = Expression.Lambda<Func<IFormatterResolver, IBssomFormatter>>(
                    Expression.Call(inputResolver, genericMethod), inputResolver).Compile();
                FormatterGetters.TryAdd(type, formatterGetter);
            }

            return formatterGetter(resolver);
        }
    }
}
