//using System.Runtime.CompilerServices;

namespace Bssom.Serializer.Internal
{
    internal interface ISegment<T>: IIteration<T>
    {
        T this[int i] { get; }
    }

    internal interface IMapKeySegment: ISegment<ulong>
    {
        int LastValueByteCount { get; }
    }
}
