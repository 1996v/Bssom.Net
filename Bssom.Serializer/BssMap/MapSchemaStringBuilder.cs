//using System.Runtime.CompilerServices;

using System.Runtime.CompilerServices;
using System.Text;

namespace Bssom.Serializer.BssMap
{
    internal class MapSchemaStringBuilder
    {
        private StringBuilder sb = new StringBuilder();

        public void AppendRouteToken(long position, BssMapRouteToken value)
        {
            sb.Append($"[{position.ToString()}]");
            sb.Append(value.ToString());
            sb.Append(" ");
        }

        public void AppendNextOff(long position, ushort value)
        {
            sb.Append($"[{position.ToString()}]");
            sb.Append("NextOff(" + value.ToString() + ")");
            sb.Append(" ");
        }

        public void AppendUInt64Val(long position, ulong value)
        {
            sb.Append("KeyU64(" + value.ToString() + ")");
            sb.Append(" ");
        }

        public void AppendUInt64Val(long position, ref byte refb, int len)
        {
            sb.Append("KeyBytes(");
            for (int i = 0; i < len; i++)
            {
                sb.Append(Unsafe.Add(ref refb, i).ToString());
                if (i != len - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append(")");
            sb.Append(" ");
        }

        public void AppendKeyType(long position, bool isNativeType, byte typeCode)
        {
            if (isNativeType && typeCode == NativeBssomType.DateTimeCode)
            {
                sb.Append("KeyType(NativeDateTimeCode)");
            }
            else
            {
                sb.Append("KeyType(" + BssomType.GetTypeName(isNativeType, typeCode) + ")");
            }
            sb.Append(" ");
        }

        public void AppendValOffset(long position, uint value)
        {
            sb.Append("ValOffset(" + value.ToString() + ")");
            sb.Append(" ");
        }

        public override string ToString()
        {
            sb.Length--;
            return sb.ToString();
        }
    }
}
