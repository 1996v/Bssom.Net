using System;
using System.Diagnostics;

namespace Bssom.Serializer.Internal
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
            {
                throw new DebugAssertException();
            }
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        internal static void Throw(Exception e)
        {
            throw new DebugAssertException();
        }
    }
}
