using Bssom.Serializer.Internal;
using System;
using System.Reflection.Emit;

namespace Bssom.Serializer.Resolvers
{
    public sealed class Array3CodeGenResolverAllowPrivate : IFormatterResolver
    {
        internal const string ModuleName = "Bssom.Serializer.Resolvers.Array3CodeGenResolverAllowPrivate";
        internal static readonly DynamicFormatterAssembly DynamicAssembly;

        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly Array3CodeGenResolverAllowPrivate Instance;

        static Array3CodeGenResolverAllowPrivate()
        {
            Instance = new Array3CodeGenResolverAllowPrivate();
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
                Formatter = (IBssomFormatter<T>)Activator.CreateInstance(Array3CodeGenResolverBuilder.Build(DynamicAssembly, new ObjectSerializationInfo(t, true)));
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
