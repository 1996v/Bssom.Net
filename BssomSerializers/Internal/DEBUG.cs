using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BssomSerializers.Internal
{
    internal class DebugAssertException : Exception
    {
    }
    internal static class DEBUG
    {
        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void Assert(bool condition)
        {
            if (!condition)
                throw new DebugAssertException();
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        internal static void Throw(Exception e)
        {
            throw new DebugAssertException();
        }
    }
}
