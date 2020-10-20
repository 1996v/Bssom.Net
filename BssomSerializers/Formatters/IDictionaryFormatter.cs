//using System.Runtime.CompilerServices;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
using BssomSerializers.BssMap;

namespace BssomSerializers.Formatters
{
    /// <summary>
    /// Format <see cref="IDictionary"/> as BssomType.Map1 or BssomType.Map2
    /// </summary>
    public sealed class IDictionaryFormatter : IBssomFormatter<IDictionary>
    {
        public static readonly IDictionaryFormatter Instance = new IDictionaryFormatter();

        private IDictionaryFormatter()
        {
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, IDictionary value)
        {
            MapFormatterHelper.SerializeIDictionary(ref writer, ref context, value);
        }

        public IDictionary Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return MapFormatterHelper.GenericDictionaryDeserialize<object, object>(ref reader,ref context);
        }

        public int Size(ref BssomSizeContext context, IDictionary value)
        {
            return MapFormatterHelper.SizeIDictionary(ref context,value);
        }
    }
}
