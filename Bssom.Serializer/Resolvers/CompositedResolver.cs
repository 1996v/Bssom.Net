//using System.Runtime.CompilerServices;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Resolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Bssom.Serializer.Resolvers
{
    /// <summary>
    /// Default composited resolver, Object -> Primitive -> Attribute -> BssomValue -> BuildIn -> IDictionary -> ICollection -> MapCodeGen.
    /// </summary>
    public sealed class CompositedResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly CompositedResolver Instance = new CompositedResolver();

        private static readonly IFormatterResolver[] Resolvers = new IFormatterResolver[] {
            ObjectResolver.Instance,
            PrimitiveResolver.Instance,
            AttributeFormatterResolver.Instance,
            BssomValueResolver.Instance,
            BuildInResolver.Instance,
            IDictionaryResolver.Instance,
            ICollectionResolver.Instance,
            MapCodeGenResolver.Instance
        };

        public IBssomFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IBssomFormatter<T> Formatter;

            static FormatterCache()
            {
                foreach (IFormatterResolver item in Resolvers)
                {
                    IBssomFormatter<T> f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        Formatter = f;
                        return;
                    }
                }
            }
        }
    }
}
