//using System.Runtime.CompilerServices;

using System.Reflection;
using System.Reflection.Emit;
using Bssom.Serializer.Internal;
using System;

namespace Bssom.Serializer.Resolvers
{
    /// <summary>
    /// <para>对实体(对象)进行动态代码生成BssomMap格式的<see cref="IBssomFormatter"/></para>
    /// <para>The entity(object) is dynamically coded to generate <see cref="IBssomFormatter"/> in BssomMap format</para>
    /// </summary>
    /// <remarks>
    /// <para>对对象中的所有(公开的,非公开的)的可读且可写的元素进行字段编组</para>
    /// <para>Field marshalling of all readable and writable elements in an object (public, non-public)</para>
    /// </remarks>
    public sealed class MapCodeGenResolverAllowPrivate : IFormatterResolver
    {
        internal const string ModuleName = "Bssom.Serializer.Resolvers.MapCodeGenResolverAllowPrivate";
        internal static readonly DynamicFormatterAssembly DynamicAssembly;

        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly MapCodeGenResolverAllowPrivate Instance;

        static MapCodeGenResolverAllowPrivate()
        {
            Instance = new MapCodeGenResolverAllowPrivate();
            DynamicAssembly = new DynamicFormatterAssembly(ModuleName);
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
                Formatter = (IBssomFormatter<T>)Activator.CreateInstance(MapCodeGenResolverBuilder.Build(DynamicAssembly, new ObjectSerializationInfo(t, true)));
            }
        }

#if NETFRAMEWORK 
        public AssemblyBuilder Save()
        {
            return DynamicAssembly.Save();
        }
#endif
    }
}
