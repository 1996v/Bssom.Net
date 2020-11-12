//using System.Runtime.CompilerServices;

namespace Bssom.Serializer.Resolvers
{
    /// <summary>
    /// Composited resolver, Object -> Primitive -> Attribute -> BssomValue -> BuildIn -> IDictionary -> ICollection -> MapCodeGenAllowPrivate.
    /// </summary>
    public sealed class CompositedResolverAllowPrivate : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly CompositedResolverAllowPrivate Instance = new CompositedResolverAllowPrivate();

        private static readonly IFormatterResolver[] Resolvers = new IFormatterResolver[] {
            ObjectResolver.Instance,
            PrimitiveResolver.Instance,
            AttributeFormatterResolver.Instance,
            BssomValueResolver.Instance,
            BuildInResolver.Instance,
            IDictionaryResolver.Instance,
            ICollectionResolver.Instance,
            MapCodeGenResolverAllowPrivate.Instance
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
