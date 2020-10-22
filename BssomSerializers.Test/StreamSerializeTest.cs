using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class StreamSerializeTest
    {
        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void MemoryStreamSerializeTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            byte[] buf = BssomSerializer.Serialize(obj);

            MemoryStream stream = new MemoryStream();
            BssomSerializer.Serialize(stream, obj);

            stream.Position.Is(buf.Length);
            buf.Is(stream.ToArray());
        }

        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void MemoryStreamSerializeAsyncTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            byte[] buf = BssomSerializer.Serialize(obj);

            MemoryStream stream = new MemoryStream();
            BssomSerializer.SerializeAsync(stream, obj).Wait();

            stream.Position.Is(buf.Length);
            buf.Is(stream.ToArray());
        }

        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void MemoryStreamDeserializeTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            byte[] buf = BssomSerializer.Serialize(obj);

            MemoryStream stream = new MemoryStream(buf);
            obj = BssomSerializer.Deserialize(stream, type);
            buf.Is(BssomSerializer.Serialize(obj));
        }


        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void MemoryStreamDeserializeAsyncTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            byte[] buf = BssomSerializer.Serialize(obj);

            MemoryStream stream = new MemoryStream(buf);
            obj = BssomSerializer.DeserializeAsync(stream, type).Result;
            buf.Is(BssomSerializer.Serialize(obj));
        }


        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void PipeStreamSerializeTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            byte[] buf = BssomSerializer.Serialize(obj);

            FakePipeStream stream = new FakePipeStream();
            BssomSerializer.Serialize(stream, obj);

            stream.CurrentCursor().Is(buf.Length);
            buf.Is(stream.ToArray());
        }

        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void PipeStreamSerializeAsyncTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            byte[] buf = BssomSerializer.Serialize(obj);

            FakePipeStream stream = new FakePipeStream();
            BssomSerializer.SerializeAsync(stream, obj).Wait();

            stream.CurrentCursor().Is(buf.Length);
            buf.Is(stream.ToArray());
        }

        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void PipeStreamDeserializeTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            byte[] buf = BssomSerializer.Serialize(obj);

            FakePipeStream stream = new FakePipeStream(buf);
            obj = BssomSerializer.Deserialize(stream, type);
            buf.Is(BssomSerializer.Serialize(obj));
        }

        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void PipeStreamDeserializeAsyncTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            byte[] buf = BssomSerializer.Serialize(obj);

            FakePipeStream stream = new FakePipeStream(buf);
            obj = BssomSerializer.DeserializeAsync(stream, type).Result;
            buf.Is(BssomSerializer.Serialize(obj));
        }

        [Fact]
        public void MemoryStreamSerializeAndDeserialize_MultipleValues_ConsecutiveReadsAndWrites_IsCorrect()
        {
            var obj1 = new List<int>() { 1 };
            var obj2 = new Dictionary<string, object>() { { "A", DateTime.MinValue } };
            var obj3 = double.MaxValue;

            MemoryStream stream = new MemoryStream();
            BssomSerializer.Serialize(stream, obj1);
            BssomSerializer.Serialize(stream, obj2);
            BssomSerializer.Serialize(stream, obj3);

            stream.Seek(0, SeekOrigin.Begin);

            var oobj1 = BssomSerializer.Deserialize<List<int>>(stream);
            var oobj2 = BssomSerializer.Deserialize<Dictionary<string, object>>(stream);
            var oobj3 = BssomSerializer.Deserialize<double>(stream);

            oobj1.Count.Is(obj1.Count);
            oobj1[0].Is(oobj1[0]);
            oobj2.Count.Is(obj2.Count);
            oobj2["A"].Is(oobj2["A"]);
            oobj3.Is(obj3);
        }


        [Fact]
        public void PipeStreamSerializeAndDeserialize_MultipleValues_ConsecutiveReadsAndWrites_IsCorrect()
        {
            var obj1 = new List<int>() { 1 };
            var obj2 = new Dictionary<string, object>() { { "A", DateTime.MinValue } };
            var obj3 = double.MaxValue;

            FakePipeStream stream = new FakePipeStream();
            BssomSerializer.Serialize(stream, obj1);
            BssomSerializer.Serialize(stream, obj2);
            BssomSerializer.Serialize(stream, obj3);

            stream.ResetCursor();
            var oobj1 = BssomSerializer.Deserialize<List<int>>(stream);
            var oobj2 = BssomSerializer.Deserialize<Dictionary<string, object>>(stream);
            var oobj3 = BssomSerializer.Deserialize<double>(stream);

            oobj1.Count.Is(obj1.Count);
            oobj1[0].Is(oobj1[0]);
            oobj2.Count.Is(obj2.Count);
            oobj2["A"].Is(oobj2["A"]);
            oobj3.Is(obj3);
        }
    }

    internal class FakePipeStream : Stream
    {
        private readonly MemoryStream stream;

        internal FakePipeStream(MemoryStream stream)
        {
            this.stream = stream;
        }

        internal FakePipeStream(byte[] buffer)
        {
            this.stream = new MemoryStream(buffer);
        }

        internal FakePipeStream()
        {
            this.stream = new MemoryStream();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => this.stream.Length;

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            this.stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        public long CurrentCursor() => this.stream.Position;

        public void ResetCursor() => this.stream.Position = 0;

        public byte[] ToArray() => this.stream.ToArray();
    }
}