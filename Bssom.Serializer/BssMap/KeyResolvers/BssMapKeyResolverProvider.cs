//using System.Runtime.CompilerServices;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer.BssMap.KeyResolvers
{
    internal static class BssMapKeyResolverProvider
    {
        private static Dictionary<Type, IBssMapKeyResolver> bssMapKeys = new Dictionary<Type, IBssMapKeyResolver>() {
            {  typeof(String),BssMapStringKeyResolver.Insance},
            {  typeof(sbyte),BssMapInt8KeyResolver.Insance},
            {  typeof(Int16),BssMapInt16KeyResolver.Insance},
            {  typeof(Int32),BssMapInt32KeyResolver.Insance},
            {  typeof(Int64),BssMapInt64KeyResolver.Insance},
            {  typeof(byte),BssMapUInt8KeyResolver.Insance},
            {  typeof(UInt16),BssMapUInt16KeyResolver.Insance},
            {  typeof(UInt32),BssMapUInt32KeyResolver.Insance},
            {  typeof(UInt64),BssMapUInt64KeyResolver.Insance},
            {  typeof(Single),BssMapFloat32KeyResolver.Insance},
            {  typeof(Double),BssMapFloat64KeyResolver.Insance},
            {  typeof(Boolean),BssMapBooleanKeyResolver.Insance},
            {  typeof(Char),BssMapCharKeyResolver.Insance},
            {  typeof(Decimal),BssMapDecimalKeyResolver.Insance},
            {  typeof(Guid),BssMapGuidKeyResolver.Insance},
            {  typeof(DateTime),BssMapDateTimeKeyResolver.Insance},//Special
        };

        private static Dictionary<byte, IBssMapKeyResolver> bssMapNativeTypeKeys = new Dictionary<byte, IBssMapKeyResolver>()
        {
            {  NativeBssomType.CharCode,BssMapCharKeyResolver.Insance},
            {  NativeBssomType.DecimalCode,BssMapDecimalKeyResolver.Insance},
            {  NativeBssomType.GuidCode,BssMapGuidKeyResolver.Insance},
            {  NativeBssomType.DateTimeCode,BssMapDateTimeKeyResolver.Insance},//Special
        };

        private static Dictionary<byte, IBssMapKeyResolver> bssMapbuildInTypeKeys = new Dictionary<byte, IBssMapKeyResolver>()
        {
            {  BssomType.StringCode,BssMapStringKeyResolver.Insance },
            {  BssomType.Int8Code,BssMapInt8KeyResolver.Insance},
            {  BssomType.Int16Code,BssMapInt16KeyResolver.Insance},
            {  BssomType.Int32Code,BssMapInt32KeyResolver.Insance},
            {  BssomType.Int64Code,BssMapInt64KeyResolver.Insance},
            {  BssomType.UInt8Code,BssMapUInt8KeyResolver.Insance},
            {  BssomType.UInt16Code,BssMapUInt16KeyResolver.Insance},
            {  BssomType.UInt32Code,BssMapUInt32KeyResolver.Insance},
            {  BssomType.UInt64Code,BssMapUInt64KeyResolver.Insance},
            {  BssomType.Float32Code,BssMapFloat32KeyResolver.Insance},
            {  BssomType.Float64Code,BssMapFloat64KeyResolver.Insance},
            {  BssomType.BooleanCode,BssMapBooleanKeyResolver.Insance},
        };

        public static readonly IBssMapKeyResolver<string> StringBssMapKeyResolver = BssMapStringKeyResolver.Insance;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetBssMapKeyResolver(Type keyType, out IBssMapKeyResolver mapKeyConvertor)
        {
            return bssMapKeys.TryGetValue(keyType, out mapKeyConvertor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBssMapKeyResolver GetAndVertiyBssMapKeyResolver(Type keyType)
        {
            if (!TryGetBssMapKeyResolver(keyType, out IBssMapKeyResolver mapKeyConvertor))
            {
                return BssomSerializationTypeFormatterException.BssomMapKeyUnsupportedType<IBssMapKeyResolver>(keyType);
            }

            return mapKeyConvertor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBssMapKeyResolver GetAndVertiyBssMapKeyResolver(bool isNativeType, byte keyType)
        {
            IBssMapKeyResolver keyConvertor;
            if (isNativeType)
            {
                if (bssMapNativeTypeKeys.TryGetValue(keyType, out keyConvertor))
                {
                    return keyConvertor;
                }
            }
            else
            {
                if (bssMapbuildInTypeKeys.TryGetValue(keyType, out keyConvertor))
                {
                    return keyConvertor;
                }
            }

            return BssomSerializationOperationException.UnexpectedCodeRead<IBssMapKeyResolver>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBssMapKeyResolver<T> GetAndVertiyBssMapKeyResolver<T>()
        {
            return IBssMapKeyStaticResolverCache<T>.Instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void VertyBssMapKeyType(object key)
        {
            if (key == null)
            {
                ThrowArgumentNullException(key);
            }

            Type keyType = key.GetType();
            if (!TryGetBssMapKeyResolver(keyType, out IBssMapKeyResolver mapKeyConvertor))
            {
                BssomSerializationTypeFormatterException.BssomMapKeyUnsupportedType<IBssMapKeyResolver>(keyType);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void VertyBssMapStringKey(object key)
        {
            if (key == null)
            {
                ThrowArgumentNullException(key);
            }

            if (((string)key) == string.Empty)
            {
                BssomSerializationArgumentException.BssomMapStringKeyIsEmpty();
            }
        }

        private static void ThrowArgumentNullException(object key)
        {
            throw new ArgumentNullException(nameof(key));
        }
    }
}
