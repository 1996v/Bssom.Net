//using System.Runtime.CompilerServices;

using System;
using System.Collections.Generic;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
namespace Bssom.Serializer.Internal
{
    internal static partial class Array1FormatterHelper
    {
        private static readonly Dictionary<Type, KeyValuePair<string, byte>> _array1ItemTypes = new Dictionary<Type, KeyValuePair<string, byte>>() 
        {
           { typeof(sbyte),new KeyValuePair<string,byte>(  nameof(BssomType.Int8Code),BssomType.Int8Code) },
           { typeof(Int16),new KeyValuePair<string,byte>( nameof(BssomType.Int16Code),BssomType.Int16Code) },
           { typeof(Int32),new KeyValuePair<string,byte>( nameof(BssomType.Int32Code),BssomType.Int32Code) },
           { typeof(Int64),new KeyValuePair<string,byte>( nameof(BssomType.Int64Code),BssomType.Int64Code) },
           { typeof(byte),new KeyValuePair<string,byte>( nameof(BssomType.UInt8Code),BssomType.UInt8Code) },
           { typeof(UInt16),new KeyValuePair<string,byte>( nameof(BssomType.UInt16Code),BssomType.UInt16Code) },
           { typeof(UInt32),new KeyValuePair<string,byte>( nameof(BssomType.UInt32Code),BssomType.UInt32Code) },
           { typeof(UInt64),new KeyValuePair<string,byte>( nameof(BssomType.UInt64Code),BssomType.UInt64Code) },
           { typeof(Single),new KeyValuePair<string,byte>( nameof(BssomType.Float32Code) ,BssomType.Float32Code)},
           { typeof(double),new KeyValuePair<string,byte>( nameof(BssomType.Float64Code),BssomType.Float64Code)},
           { typeof(Boolean),new KeyValuePair<string,byte>( nameof(BssomType.BooleanCode) ,BssomType.BooleanCode)},
           { typeof(DateTime),new KeyValuePair<string,byte>( nameof(BssomType.TimestampCode),BssomType.TimestampCode) },

           { typeof(Guid),new KeyValuePair<string,byte>(nameof(NativeBssomType.GuidCode),NativeBssomType.GuidCode) },
           { typeof(char),new KeyValuePair<string,byte>(nameof(NativeBssomType.CharCode),NativeBssomType.CharCode) },
           { typeof(decimal),new KeyValuePair<string,byte>(nameof(NativeBssomType.DecimalCode),NativeBssomType.DecimalCode) },
        };
        private static readonly HashSet<Type> _nativeArray1ItemTypes = new HashSet<Type>() {
            typeof(Guid),typeof(char),typeof(decimal)
        };

        public static bool IsArray1Type(Type t)
        {
            return _array1ItemTypes.ContainsKey(t);
        }

        public static bool IsArray1Type(Type t, out bool isNativeType, out byte typeCode, out string typeCodeName)
        {
            isNativeType = _nativeArray1ItemTypes.Contains(t);
            if (_array1ItemTypes.TryGetValue(t, out var pair))
            {
                typeCode = pair.Value;
                typeCodeName = pair.Key;
                return true;
            }
            typeCode = default; typeCodeName = default;
            return false;
        }
    }

}
