//using System.Runtime.CompilerServices;

namespace BssomSerializers.BssMap
{
    internal enum AutomateState
    {
        ReadBranch,
        ReadKey,
        ReadChildren,
        CheckEnd
    }

    internal enum AutomateReadOneKeyState
    {
        ReadNextBranch,
        ReadChildren,
    }
}
