//using System.Runtime.CompilerServices;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;

namespace Bssom.Serializer.Formatters
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
