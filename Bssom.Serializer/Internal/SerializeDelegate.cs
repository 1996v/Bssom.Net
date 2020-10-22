using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bssom.Serializer.Internal
{
    internal delegate void Serialize<T>(ref BssomWriter writer, ref BssomSerializeContext context, T t);
    internal delegate T Deserialize<T>(ref BssomReader reader, ref BssomDeserializeContext context);
    internal delegate int Size<T>(ref BssomSizeContext context, T t);
    internal delegate void Map1FormatterDeserialize<T>(ref BssomDeserializeContext context, ref BssomReader reader, ref T t);
}
