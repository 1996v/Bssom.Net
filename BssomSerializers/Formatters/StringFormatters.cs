
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
namespace BssomSerializers.Formatters
{
    /// <summary>
    /// Format <see cref="String"/>? as BssomType.String
    /// </summary>
    public sealed class StringFormatter : IBssomFormatter<String>
    {
        public static readonly StringFormatter Instance = new StringFormatter();

        private StringFormatter()
        {
            
        }

        public int Size(ref BssomSizeContext context, string value)
        {
            if (value == null)
                return BssomBinaryPrimitives.NullSize;

            return BssomBinaryPrimitives.StringSize(value) + BssomBinaryPrimitives.BuildInTypeCodeSize;
        }

        public string Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return reader.ReadString();
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, string value)
        {
            writer.Write(value);
        }
    }
}
