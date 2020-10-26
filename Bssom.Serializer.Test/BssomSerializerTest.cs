using Bssom.Serializer.BssomBuffer;
using Xunit;
namespace Bssom.Serializer.Test
{
    public class BssomSerializerTest
    {
        [Fact]
        public void SizeTest()
        {
            int val = int.MaxValue;
            BssomSerializer.Size(val).Is(5);
        }

        [Fact]
        public void SizeTestWithContext()
        {
            int val = int.MaxValue;
            BssomSizeContext context = new BssomSizeContext();
            BssomSerializer.Size(ref context, val).Is(5);
        }

        [Fact]
        public void SerializeTest()
        {
            int val = int.MaxValue;
            BssomSerializer.Serialize(val).Length.Is(5);
        }

        [Fact]
        public void SerializeTestWithRefBytes()
        {
            int val = int.MaxValue;
            byte[] buf = new byte[8];
            buf[1] = 3;
            BssomSerializer.Serialize(ref buf, 2, val).Is(5);
            buf.Length.Is(8);
            buf[1].Is(3);
            BssomSerializer.Deserialize<int>(buf, 2, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void SerializeTestWithRefBytes_Expanded()
        {
            int val = int.MaxValue;
            byte[] buf = new byte[3];
            buf[1] = 3;
            BssomSerializer.Serialize(ref buf, 2, val).Is(5);
            buf.Length.Is(7);
            buf[1].Is(3);
            BssomSerializer.Deserialize<int>(buf, 2, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void SerializeTestWithRefBytes_Context()
        {
            BssomSerializeContext context = new BssomSerializeContext();
            int val = int.MaxValue;
            byte[] buf = new byte[8];
            buf[1] = 3;
            BssomSerializer.Serialize(ref context, ref buf, 2, val).Is(5);
            buf.Length.Is(8);
            buf[1].Is(3);
            BssomSerializer.Deserialize<int>(buf, 2, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void SerializeTestWithIBssomBufferWriter()
        {
            int val = int.MaxValue;
            var bufw = ExpandableBufferWriter.CreateGlobar();
            BssomSerializer.Serialize(val, bufw);
            var buf = bufw.GetBufferedArray();
            buf.Length.Is(5);
            BssomSerializer.Deserialize<int>(buf, 0, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void SerializeTestWithContext()
        {
            int val = int.MaxValue;
            var context = new BssomSerializeContext();
            var buf = BssomSerializer.Serialize(ref context, val);
            buf.Length.Is(5);
            BssomSerializer.Deserialize<int>(buf, 0, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void SerializeTestWithContext_IBssomBufferWriter()
        {
            int val = int.MaxValue;
            var context = new BssomSerializeContext();
            var bufw = ExpandableBufferWriter.CreateGlobar();
            BssomSerializer.Serialize(ref context, val, bufw);
            var buf = bufw.GetBufferedArray();
            buf.Length.Is(5);
            BssomSerializer.Deserialize<int>(buf, 0, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void SerializeTestWithContext_Stream()
        {
            int val = int.MaxValue;
            var context = new BssomSerializeContext();
            var stream = new FakePipeStream();
            BssomSerializer.Serialize(ref context, val, stream);
            stream.CurrentCursor().Is(5);
            BssomSerializer.Deserialize<int>(stream.ToArray(), 0, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void SerializeTestWithStream()
        {
            int val = int.MaxValue;
            var stream = new FakePipeStream();
            BssomSerializer.Serialize(stream, val);
            stream.CurrentCursor().Is(5);
            BssomSerializer.Deserialize<int>(stream.ToArray(), 0, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void SerializeTestWithStreamAsync()
        {
            int val = int.MaxValue;
            var stream = new FakePipeStream();
            BssomSerializer.SerializeAsync(stream, val).Wait();
            stream.CurrentCursor().Is(5);
            BssomSerializer.Deserialize<int>(stream.ToArray(), 0, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void DeserializeTestWithStream()
        {
            int val = int.MaxValue;
            var buf = BssomSerializer.Serialize(val);
            var stream = new FakePipeStream(buf);
            BssomSerializer.Deserialize<int>(stream).Is(int.MaxValue);
            stream.CurrentCursor().Is(5);
        }

        [Fact]
        public void DeserializeTestWithStream_Context()
        {
            int val = int.MaxValue;
            var context = new BssomDeserializeContext();
            var buf = BssomSerializer.Serialize(val);
            var stream = new FakePipeStream(buf);
            BssomSerializer.Deserialize<int>(ref context, stream).Is(int.MaxValue);
            stream.CurrentCursor().Is(5);
        }

        [Fact]
        public void DeserializeTestWithStreamAsync()
        {
            int val = int.MaxValue;
            var buf = BssomSerializer.Serialize(val);
            var stream = new FakePipeStream(buf);
            BssomSerializer.DeserializeAsync<int>(stream).Result.Is(int.MaxValue);
            stream.CurrentCursor().Is(5);
        }

        [Fact]
        public void DeserializeTestWithStream_Type()
        {
            int val = int.MaxValue;
            var buf = BssomSerializer.Serialize(val);
            var stream = new FakePipeStream(buf);
            ((int)BssomSerializer.Deserialize(stream, typeof(int))).Is(int.MaxValue);
            stream.CurrentCursor().Is(5);
        }

        [Fact]
        public void DeserializeTestWithStream_Type_Context()
        {
            int val = int.MaxValue;
            var context = new BssomDeserializeContext();
            var buf = BssomSerializer.Serialize(val);
            var stream = new FakePipeStream(buf);
            ((int)BssomSerializer.Deserialize(ref context, stream, typeof(int))).Is(int.MaxValue);
            stream.CurrentCursor().Is(5);
        }

        [Fact]
        public void DeserializeTestWithStreamAsync_Type()
        {
            int val = int.MaxValue;
            var buf = BssomSerializer.Serialize(val);
            var stream = new FakePipeStream(buf);
            ((int)(BssomSerializer.DeserializeAsync(stream, typeof(int)).Result)).Is(int.MaxValue);
            stream.CurrentCursor().Is(5);
        }

        [Fact]
        public void DeserializeTestWithT_IBssomBuffer()
        {
            int val = int.MaxValue;
            var buf = BssomSerializer.Serialize(val);
            var sbuf = new SimpleBuffer(buf);
            BssomSerializer.Deserialize<int>(sbuf).Is(int.MaxValue);
        }

        [Fact]
        public void DeserializeTestWithT_IBssomBuffer_Context()
        {
            int val = int.MaxValue;
            var buf = BssomSerializer.Serialize(val);
            var sbuf = new SimpleBuffer(buf);
            var context = new BssomDeserializeContext();
            BssomSerializer.Deserialize<int>(ref context, sbuf).Is(int.MaxValue);
        }

        [Fact]
        public void DeserializeTestWithT_ReadSize()
        {
            int val = int.MaxValue;
            var buf = new byte[7];
            BssomSerializer.Serialize(ref buf, 1, val);
            BssomSerializer.Deserialize<int>(buf, 1, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void DeserializeTestWithT_ReadSize_Context()
        {
            int val = int.MaxValue;
            var buf = new byte[7];
            BssomSerializer.Serialize(ref buf, 1, val);
            var context = new BssomDeserializeContext();
            BssomSerializer.Deserialize<int>(ref context, buf, 1, out int readSize).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void DeserializeTestWithT_ReadSize_Type()
        {
            int val = int.MaxValue;
            var buf = new byte[7];
            BssomSerializer.Serialize(ref buf, 1, val);
            ((int)BssomSerializer.Deserialize(buf, 1, out int readSize, typeof(int))).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void DeserializeTestWithT_ReadSize_Type_Context()
        {
            int val = int.MaxValue;
            var buf = new byte[7];
            BssomSerializer.Serialize(ref buf, 1, val);
            var context = new BssomDeserializeContext();
            ((int)BssomSerializer.Deserialize(ref context, buf, 1, out int readSize, typeof(int))).Is(int.MaxValue);
            readSize.Is(5);
        }

        [Fact]
        public void DeserializeTestWithT_IBssomBuffer_Type()
        {
            int val = int.MaxValue;
            var buf = BssomSerializer.Serialize(val);
            var sbuf = new SimpleBuffer(buf);
            ((int)BssomSerializer.Deserialize(sbuf, typeof(int))).Is(int.MaxValue);
        }

        [Fact]
        public void DeserializeTestWithT_IBssomBuffer_Type_Context()
        {
            int val = int.MaxValue;
            var buf = BssomSerializer.Serialize(val);
            var sbuf = new SimpleBuffer(buf);
            var context = new BssomDeserializeContext();
            ((int)BssomSerializer.Deserialize(ref context, sbuf, typeof(int))).Is(int.MaxValue);
        }
    }
}
