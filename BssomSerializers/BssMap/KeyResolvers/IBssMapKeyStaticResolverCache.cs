//using System.Runtime.CompilerServices;

namespace BssomSerializers.BssMap.KeyResolvers
{
    internal static class IBssMapKeyStaticResolverCache<T>
    {
        public static IBssMapKeyResolver<T> Instance;

        static IBssMapKeyStaticResolverCache()
        {
            if (BssMapKeyResolverProvider.TryGetBssMapKeyResolver(typeof(T), out var convertor))
                Instance = (IBssMapKeyResolver<T>)convertor;
            else
                BssomSerializationTypeFormatterException.BssomMapKeyUnsupportedType(typeof(T));
        }
    }
}
