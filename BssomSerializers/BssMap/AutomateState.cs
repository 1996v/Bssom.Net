//using System.Runtime.CompilerServices;

namespace Bssom.Serializer.BssMap
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
