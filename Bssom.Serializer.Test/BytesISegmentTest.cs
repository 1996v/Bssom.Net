using System.Text;
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

        [Fact]
        public void UInt64BytesISegment_Value_Is_Correctly()
        {
            UInt64BytesISegment se;
            se = new UInt64BytesISegment(UTF8Encoding.UTF8.GetBytes("a123456789"));
            se.Len.Is(10);
            se.UInt64Count.Is(2);
            se.LastValueByteCount.Is(2);
            se[0].Is((ulong)3978425819141910881);
            se[1].Is((ulong)14648);

            se = new UInt64BytesISegment(UTF8Encoding.UTF8.GetBytes("b123"));
            se.Len.Is(4);
            se.UInt64Count.Is(1);
            se.LastValueByteCount.Is(4);
            se[0].Is((ulong)858927458);

            se = new UInt64BytesISegment(UTF8Encoding.UTF8.GetBytes("a1234567"));
            se.Len.Is(8);
            se.UInt64Count.Is(1);
            se.LastValueByteCount.Is(8);
            se[0].Is((ulong)3978425819141910881);
        }
    }
}
