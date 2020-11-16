//using System.Runtime.CompilerServices;

namespace Bssom.Serializer.Resolvers
{
    /// <summary>
    /// IntKey Composited resolver, with faster serialization performance and more compact packets, members in entities must be marked with <see cref="KeyAttribute"/>, Object -> Primitive -> Attribute -> BssomValue -> BuildIn -> IDictionary -> ICollection -> Array3CodeGenResolverAllowPrivate.
    /// </summary>
    public sealed class IntKeyCompositedResolverAllowPrivate : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly IntKeyCompositedResolverAllowPrivate Instance = new IntKeyCompositedResolverAllowPrivate();

        private static readonly IFormatterResolver[] Resolvers = new IFormatterResolver[] {
            ObjectResolver.Instance,
            PrimitiveResolver.Instance,
            AttributeFormatterResolver.Instance,
            BssomValueResolver.Instance,
            BuildInResolver.Instance,
            IDictionaryResolver.Instance,
            ICollectionResolver.Instance,
            Array3CodeGenResolverAllowPrivate.Instance
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
