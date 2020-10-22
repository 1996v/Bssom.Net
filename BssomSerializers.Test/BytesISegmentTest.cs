using Bssom.Serializer.BssMap;
using Bssom.Serializer.Internal;
using Xunit;
namespace Bssom.Serializer.Test
{
    public class BytesISegmentTest
    {
        [Fact]
        public void Raw64BytesISegmentTest()
        {
            var se = new Raw64BytesISegment(new byte[15]);
            se.Len.Is(15);
            se.Raw64Count.Is(2);
            se.LastValueByteCount.Is(7);
        }

        [Fact]
        public void UInt64BytesISegmentTest()
        {
            var se = new UInt64BytesISegment(new byte[15]);
            se.Len.Is(15);
            se.UInt64Count.Is(2);
            se.LastValueByteCount.Is(7);
        }

    }
}
