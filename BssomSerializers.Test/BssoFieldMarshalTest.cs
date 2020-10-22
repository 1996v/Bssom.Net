using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
namespace BssomSerializers.Test
{
    public class BssomFieldMarshalTest
    {
        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Int64))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(UInt16))]
        [InlineData(typeof(UInt32))]
        [InlineData(typeof(UInt64))]
        [InlineData(typeof(Char))]
        [InlineData(typeof(Boolean))]
        [InlineData(typeof(Single))]
        [InlineData(typeof(Double))]
        [InlineData(typeof(string))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(Decimal))]

        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<Int16, List<object>>))]
        [InlineData(typeof(Dictionary<Int32, List<object>>))]
        [InlineData(typeof(Dictionary<Int64, List<object>>))]
        [InlineData(typeof(Dictionary<byte, List<object>>))]
        [InlineData(typeof(Dictionary<UInt16, List<object>>))]
        [InlineData(typeof(Dictionary<UInt32, List<object>>))]
        [InlineData(typeof(Dictionary<UInt64, List<object>>))]
        [InlineData(typeof(Dictionary<Char, List<object>>))]
        [InlineData(typeof(Dictionary<Boolean, List<object>>))]
        [InlineData(typeof(Dictionary<Single, List<object>>))]
        [InlineData(typeof(Dictionary<Double, List<object>>))]
        [InlineData(typeof(Dictionary<string, List<object>>))]
        [InlineData(typeof(Dictionary<Guid, List<object>>))]
        [InlineData(typeof(Dictionary<DateTime, List<object>>))]
        [InlineData(typeof(Dictionary<Decimal, List<object>>))]

        [InlineData(typeof(List<byte>))]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(List<DateTime>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<object>))]
        [InlineData(typeof(List<List<object>>))]

        [InlineData(typeof(byte[]))]
        [InlineData(typeof(char[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(DateTime[]))]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(object[]))]

        [InlineData(typeof(ArraySegment<byte>))]
        [InlineData(typeof(ArraySegment<decimal>))]
        [InlineData(typeof(ArraySegment<ulong>))]
        [InlineData(typeof(ArraySegment<DateTime>))]
        [InlineData(typeof(ArraySegment<string>))]
        [InlineData(typeof(ArraySegment<object>))]

        [InlineData(typeof(_class1))]
        [InlineData(typeof(_class2))]
        [InlineData(typeof(_class3))]
        public void SizeTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            VerifyHelper.VerifySize(obj);
        }

        [Theory]
        [InlineData(typeof(Dictionary<sbyte, List<object>>))]
        [InlineData(typeof(Dictionary<UInt64, List<object>>))]
        [InlineData(typeof(Dictionary<Char, List<object>>))]
        [InlineData(typeof(Dictionary<string, List<object>>))]
        [InlineData(typeof(Dictionary<Guid, List<object>>))]
        [InlineData(typeof(Dictionary<DateTime, List<object>>))]
        [InlineData(typeof(Dictionary<Decimal, List<object>>))]
        public void Map2SizeTest(Type type)
        {
            var obj = RandomHelper.RandomValueWithOutStringEmpty(type);
            VerifyHelper.VerifySize(obj, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void WriteStringWithNotPredictingLengthTest()
        {
            var str = RandomHelper.RandomValueWithOutStringEmpty<string>();
            var buf = BssomSerializer.Serialize(str);
            buf.Length.Is(BssomSerializer.Size(str));//Simulate StringSize

            var bsfw = new BssomFieldMarshaller(buf);
            bsfw.TryWrite(BssomFieldOffsetInfo.Zero, str).IsTrue();// is call WriteStringWithNotPredictingLength
            bsfw.ReadValueSize(BssomFieldOffsetInfo.Zero).Is(buf.Length);
            bsfw.ReadValue<string>(BssomFieldOffsetInfo.Zero).Is(str);
        }

        [Fact]
        public void MultipleStringTryWrite_LastElementIsWriteStringWithNotPredictingLength()
        {
            var str = new string[] { "a", "b12345678" };
            var buf = BssomSerializer.Serialize(str);
            buf.Length.Is(BssomSerializer.Size(str));//Simulate StringSize

            var bsfw = new BssomFieldMarshaller(buf);
            bsfw.TryWrite(BssomFieldOffsetInfo.Zero, str).IsTrue();//first element is fast stringwrite, last element is notPredicting(slow) write
            bsfw.ReadValueSize(BssomFieldOffsetInfo.Zero).Is(buf.Length);
            bsfw.ReadValue<string[]>(BssomFieldOffsetInfo.Zero).Is(str);
        }

        [Theory]
        [MemberData(nameof(ReadBlankTestData))]
        public void ReadByIgnoringBlankCharactersTest(object val, Func<object, bool> test)
        {
            var obj = new byte[3000];
            var buf = BssomSerializer.Serialize(obj);

            var bsfw = new BssomFieldMarshaller(buf);
            bsfw.TryWrite(BssomFieldOffsetInfo.Zero, val).IsTrue();
            test(BssomSerializer.Deserialize(buf, 0, buf.Length, val.GetType())).IsTrue();

            bsfw.TryWrite(BssomFieldOffsetInfo.Zero, val).IsTrue();
            test(BssomSerializer.Deserialize(buf, 0, buf.Length, val.GetType())).IsTrue();
        }

        public static object[][] ReadBlankTestData = new object[][]
        {
            new object[]{ sbyte.MaxValue, (Func<object,bool>)(e=> (sbyte)e==sbyte.MaxValue)  },
            new object[]{ Int16.MaxValue, (Func<object,bool>)(e=> (Int16)e== Int16.MaxValue)  },
            new object[]{ Int32.MaxValue, (Func<object,bool>)(e=> (Int32)e== Int32.MaxValue)  },
            new object[]{ Int64.MaxValue, (Func<object,bool>)(e=> (Int64)e== Int64.MaxValue)  },
            new object[]{ byte.MaxValue, (Func<object,bool>)(e=> (byte)e== byte.MaxValue)  },
            new object[]{ UInt16.MaxValue, (Func<object,bool>)(e=> (UInt16)e== UInt16.MaxValue)  },
            new object[]{ UInt32.MaxValue, (Func<object,bool>)(e=> (UInt32)e== UInt32.MaxValue)  },
            new object[]{ UInt64.MaxValue, (Func<object,bool>)(e=> (UInt64)e== UInt64.MaxValue)  },
            new object[]{ Char.MaxValue, (Func<object,bool>)(e=> (Char)e== Char.MaxValue)  },
            new object[]{ true, (Func<object,bool>)(e=> (bool)e== true)  },
            new object[]{ Single.MaxValue, (Func<object,bool>)(e=> (Single)e== Single.MaxValue)  },
            new object[]{ Double.MaxValue, (Func<object,bool>)(e=> (Double)e== Double.MaxValue)  },
            new object[]{ "aa", (Func<object,bool>)(e=> (string)e=="aa")  },
            new object[]{ Guid.Parse("17a5dd95-4aaf-4075-9aec-e6daa0751fe8"), (Func<object,bool>)(e=> (Guid)e== Guid.Parse("17a5dd95-4aaf-4075-9aec-e6daa0751fe8"))  },
            new object[]{ DateTime.MaxValue, (Func<object,bool>)(e=> (DateTime)e== DateTime.MaxValue)  },
            new object[]{ Decimal.MaxValue, (Func<object,bool>)(e=> (Decimal)e== Decimal.MaxValue)  },

            new object[]{ new Dictionary<sbyte, List<object>> { { 3,new List<object> { 1} } }, (Func<object,bool>)(e=> {
                 var dic= (Dictionary<sbyte, List<object>>)e;
                 object o = dic[3][0];
                 bool b = ((int)o)==1;
                 return b;
                })},
            new object[]{ new Dictionary<DateTime, List<object>> { { DateTime.MaxValue,new List<object> { 1} } }, (Func<object,bool>)(e=> {
                 var dic= (Dictionary<DateTime, List<object>>)e;
                 object o = dic[DateTime.MaxValue][0];
                 bool b = ((int)o)==1;
                 return b;
                })},

            new object[]{ new List<byte> { 1,2}, (Func<object,bool>)(e=> {
                  var val= (List<byte>)e;
                  bool b = val[0]==1&&val[1]==2;
                  return b;
            })},

            new object[]{ new List<DateTime> { DateTime.MaxValue}, (Func<object,bool>)(e=> {
                  var val= (List<DateTime>)e;
                  bool b = val[0]==DateTime.MaxValue;
                  return b;
            })  },
            new object[]{ new List<string> { "f"}, (Func<object,bool>)(e=> {
                  var val= (List<string>)e;
                  bool b = val[0]=="f";
                  return b;
            })  },
            new object[]{ new List<object> { 1,2}, (Func<object,bool>)(e=> {
                  var val= (List<object>)e;
                  bool b = (int)val[0]==1&&(int)val[1]==2;
                  return b;
            })  },

            new object[]{ new byte[] { 1,2}, (Func<object,bool>)(e=> {
                  var val= (byte[])e;
                  bool b = (int)val[0]==1&&(int)val[1]==2;
                  return b;
            })  },
            new object[]{ new double[] { 1,2}, (Func<object,bool>)(e=> {
                  var val= (double[])e;
                  bool b = (int)val[0]==1&&(int)val[1]==2;
                  return b;
            })  },
            new object[]{ new DateTime[] { DateTime.MaxValue }, (Func<object,bool>)(e=> {
                  var val= (DateTime[])e;
                  bool b = val[0]==DateTime.MaxValue;
                  return b;
            })  },
            new object[]{ new string[] { "f"}, (Func<object,bool>)(e=> {
                  var val= (string[])e;
                  bool b = val[0]=="f";
                  return b;
            })  },

            new object[]{ new ArraySegment<byte>(new byte[] { 1,2}), (Func<object,bool>)(e=> {
                  var val= ((ArraySegment<byte>)e).Array;
                  bool b = (int)val[0]==1&&(int)val[1]==2;
                  return b;
            })  },
            new object[]{ new ArraySegment<DateTime>(new DateTime[] { DateTime.MaxValue }) , (Func<object,bool>)(e=> {
                  var val= ((ArraySegment<DateTime>)e).Array;
                  bool b = val[0]==DateTime.MaxValue;
                  return b;
            })  },
            new object[]{ new ArraySegment<string>(new string[] { "f"}), (Func<object,bool>)(e=> {
                  var val= ((ArraySegment<string>)e).Array;
                  bool b = val[0]=="f";
                  return b;
            })  },
            new object[]{ new ArraySegment<object>(new object[] { 1,2}), (Func<object,bool>)(e=> {
                  var val= ((ArraySegment<object>)e).Array;
                  bool b = (int)val[0]==1&&(int)val[1]==2;
                  return b;
            })  },

            new object[]{ new _class1 { B1231231VV=3}, (Func<object,bool>)(e=> {
                  var val= (_class1)e;
                  bool b = val.B1231231VV==3;
                  return b;
            })  },
        };

        [Fact]
        public void StringInputDataSourceTest()
        {
            var sids = new StringInputDataSource("[A]$1213[sdf][ff]$9987$234");
            sids.MoveNext().IsTrue();
            sids.GetCurrentSegmentString().Is("A");
            sids.MoveNext().IsTrue();
            sids.GetCurrentArrayIndex().Is(1213);
            sids.MoveNext().IsTrue();
            sids.GetCurrentSegmentString().Is("sdf");
            sids.MoveNext().IsTrue();
            sids.GetCurrentSegmentString().Is("ff");
            sids.MoveNext().IsTrue();
            sids.GetCurrentArrayIndex().Is(9987);
            sids.MoveNext().IsTrue();
            sids.GetCurrentArrayIndex().Is(234);
        }

        [Fact]
        public void NullType_IndexOf_NullExceptionToAnyInput()
        {
            var buf = BssomSerializer.Serialize<Dictionary<string, string>>(null);
            var bsfm = new BssomFieldMarshaller(buf);
            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.IndexOf("[r]"),
          ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.OperationObjectIsNull);
        }

        [Fact]
        public void MapType_IndexOf_InputFormatAndBufTypeFormatNotMatch_IsException()
        {
            var buf = BssomSerializer.Serialize(new Dictionary<object, string>() { { "w", "f" } });
            var bsfm = new BssomFieldMarshaller(buf);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("$1"),
              ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.IndexOf("[r]$1"),
            ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.OperationObjectIsNull);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.IndexOf("[w][r]"),
           ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.UnsupportedOperation);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.IndexOf("[r][h]"),
             ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.OperationObjectIsNull);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("rrr"),
           ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);
        }

        [Fact]
        public void ArrayType_IndexOf_ArrayTypeIndexOutOfBoundsException()
        {
            var buf = BssomSerializer.Serialize(new List<object>() { 1 });
            var bsfm = new BssomFieldMarshaller(buf);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.IndexOf("$1"),
              ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.ArrayTypeIndexOutOfBounds);
        }

        [Fact]
        public void ArrayType_IndexOf_InputFormatAndBufTypeFormatNotMatch_IsException()
        {
            var buf = BssomSerializer.Serialize(new List<object>() { 1 });
            var bsfm = new BssomFieldMarshaller(buf);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.IndexOf("$0$3"),
              ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.UnsupportedOperation);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("[r]"),
            ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("[r]$1"),
            ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("rrr"),
           ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);
        }

        [Fact]
        public void MixtureType_FirstArrayType_IndexOf_InputFormatAndBufTypeFormatNotMatch_IsException()
        {
            var buf = BssomSerializer.Serialize(new List<Dictionary<object, string>>() { new Dictionary<object, string>() { { "r", "f" } } });
            var bsfm = new BssomFieldMarshaller(buf);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.IndexOf("$0[r][h]"),
        ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.UnsupportedOperation);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("[r]$1"),
            ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("[r][h]"),
             ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("rrr"),
           ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);
        }

        [Fact]
        public void MixtureType_FirstMapType_IndexOf_InputFormatAndBufTypeFormatNotMatch_IsException()
        {
            var buf = BssomSerializer.Serialize(new Dictionary<object, List<object>>() { { "r", new List<object>() { 3 } } });
            var bsfm = new BssomFieldMarshaller(buf);

            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.IndexOf("[r]$1[h]"),
    ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.ArrayTypeIndexOutOfBounds);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("$1[r]"),
            ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("$1$3"),
             ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);

            VerifyHelper.Throws<BssomSerializationArgumentException>(() => bsfm.IndexOf("rrr"),
           ex => ex.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.InvalidFormatOfMembers);
        }

        [Theory]
        [InlineData((byte)10)]
        [InlineData((short)-10)]
        [InlineData((int)-10)]
        [InlineData((long)-120)]
        [InlineData((float)0.000006)]
        [InlineData((double)0.000006)]
        [InlineData('\u9999')]
        [InlineData("12345678901234567")]
        public void IndexOf_MustBe_Is_ArrayAndMapType_CanCall(object value)
        {
            VerifyHelper.Throws<BssomSerializationOperationException>(() =>
                {
                    var buf = BssomSerializer.Serialize(value);
                    BssomFieldMarshaller.IndexOf(buf, 0, buf.Length, "[A]");
                },
         ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.UnsupportedOperation);

            var buf2 = BssomSerializer.Serialize(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, object>>());
            BssomFieldMarshaller.IndexOf(buf2, 0, buf2.Length, "[A]").IsType<BssomFieldOffsetInfo>();
            buf2 = BssomSerializer.Serialize(RandomHelper.RandomValueWithOutStringEmpty<List<object>>());
            BssomFieldMarshaller.IndexOf(buf2, 0, buf2.Length, "$1").IsType<BssomFieldOffsetInfo>();
        }

        [Fact]
        public void Map1_IndexOf_FunctionIsCorrectly()
        {
            var val = new Dictionary<string, object>() {
                { "A",(int)3},
                { "B",(DateTime)DateTime.MaxValue},
                { "C",(string)"1234567"}
            };
            var option = BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true);
            var buf = BssomSerializer.Serialize(val, option);

            var bsfm = new BssomFieldMarshaller(buf);
            bsfm.IndexOf("[A]").IsExists.IsTrue();
            bsfm.IndexOf("[B]").IsExists.IsTrue();
            bsfm.IndexOf("[C]").IsExists.IsTrue();

            bsfm.IndexOf("[H]").IsExists.IsFalse();
            bsfm.IndexOf("[H123123123]").IsExists.IsFalse();
        }

        [Fact]
        public void Map2_IndexOf_FunctionIsCorrectly()
        {
            var val = RandomHelper.RandomValue<_class1>();
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);
            bsfm.IndexOf("[" + nameof(_class1.A) + "]").IsExists.IsTrue();
            bsfm.IndexOf("[" + nameof(_class1.B123123123) + "]").IsExists.IsTrue();
            bsfm.IndexOf("[" + nameof(_class1.B1231231VV) + "]").IsExists.IsTrue();
            bsfm.IndexOf("[" + nameof(_class1.C) + "]").IsExists.IsTrue();

            bsfm.IndexOf("[H]").IsExists.IsFalse();
            bsfm.IndexOf("[H123123123]").IsExists.IsFalse();
        }

        [Theory]
        [InlineData(typeof(List<byte>))]//Array1
        [InlineData(typeof(List<int>))]//Array1
        [InlineData(typeof(List<DateTime>))]//Array1
        [InlineData(typeof(List<string>))]//Array2
        [InlineData(typeof(List<object>))]//Array2
        public void Array_IndexOf_FunctionIsCorrectly(Type t)
        {
            var val = RandomHelper.RandomValue(t);
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);
            int count = ((ICollection)val).Count;
            for (int i = 0; i < count; i++)
            {
                bsfm.IndexOf($"${i}").IsExists.IsTrue();
            }

            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.IndexOf($"${count}"),
                ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.ArrayTypeIndexOutOfBounds);
        }

        [Fact]
        public void Map1_ReadStrongType_FunctionIsCorrectly()
        {
            var val = new Dictionary<string, object>() {
                { "A",(int)3},
                { "B",(DateTime)DateTime.MaxValue},
                { "C",(string)"1234567"}
            };

            var option = BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true);
            var buf = BssomSerializer.Serialize(val, option);

            var bsfm = new BssomFieldMarshaller(buf);

            bsfm.ReadValue<int>(bsfm.IndexOf("[A]")).Is(val["A"]);
            bsfm.ReadValue<DateTime>(bsfm.IndexOf("[B]")).Is(val["B"]);
            bsfm.ReadValue<string>(bsfm.IndexOf("[C]")).Is(val["C"]);
        }

        [Fact]
        public void Map2_ReadStrongType_FunctionIsCorrectly()
        {
            var val = RandomHelper.RandomValue<_class1>();
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);
            bsfm.ReadValue<DateTime>(bsfm.IndexOf("[" + nameof(_class1.A) + "]")).Is(val.A);
            bsfm.ReadValue<Guid>(bsfm.IndexOf("[" + nameof(_class1.B123123123) + "]")).Is(val.B123123123);
            bsfm.ReadValue<int>(bsfm.IndexOf("[" + nameof(_class1.B1231231VV) + "]")).Is(val.B1231231VV);
            bsfm.ReadValue<int>(bsfm.IndexOf("[" + nameof(_class1.C) + "]")).Is(val.C);
        }

        [Fact]
        public void Array1_ReadStrongType_FunctionIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<byte>>();
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);
            int count = ((ICollection)val).Count;
            for (int i = 0; i < count; i++)
            {
                bsfm.ReadValue<byte>(bsfm.IndexOf($"${i}")).Is(val[i]);
            }
        }

        [Fact]
        public void Array2_ReadStrongType_FunctionIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<string>>();
            var buf = BssomSerializer.Serialize(val);
            var r = BssomSerializer.Deserialize<List<string>>(buf);

            var bsfm = new BssomFieldMarshaller(buf);
            int count = ((ICollection)val).Count;
            for (int i = 0; i < count; i++)
            {
                bsfm.ReadValue<string>(bsfm.IndexOf($"${i}")).Is(val[i]);
            }
        }

        [Fact]
        public void MixtureType_ReadStrongType_FunctionIsCorrectly()
        {
            var val = new List<Dictionary<string, int[]>>() {
                new Dictionary<string, int[]>() {
                    { "A",new int[]{1,2,3 } },
                    { "B",new int[]{4,5,6 } }
                },
                new Dictionary<string, int[]>() {
                    { "A2",new int[]{1,2,3 } },
                    { "B2",new int[]{4,5,6 } }
                }
            };
            var buf = BssomSerializer.Serialize(val);
            var bsfm = new BssomFieldMarshaller(buf);

            bsfm.ReadValue<int>(bsfm.IndexOf("$0[A]$0")).Is(1);
            bsfm.ReadValue<int>(bsfm.IndexOf("$0[A]$1")).Is(2);
            bsfm.ReadValue<int>(bsfm.IndexOf("$0[B]$2")).Is(6);

            bsfm.ReadValue<int>(bsfm.IndexOf("$1[A2]$0")).Is(1);
            bsfm.ReadValue<int>(bsfm.IndexOf("$1[A2]$1")).Is(2);
            bsfm.ReadValue<int>(bsfm.IndexOf("$1[B2]$2")).Is(6);
        }

        [Fact]
        public void Map1_ReadBssomValue_FunctionIsCorrectly()
        {
            var val = new Dictionary<string, object>() {
                { "A",(int)3},
                { "B",(DateTime)DateTime.MaxValue},
                { "C",(string)"1234567"}
            };

            var option = BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true);
            var buf = BssomSerializer.Serialize(val, option);

            var bsfm = new BssomFieldMarshaller(buf);

            bsfm.ReadValue<BssomValue>(bsfm.IndexOf("[A]")).Is(BssomValue.Create(val["A"]));
            bsfm.ReadValue<BssomValue>(bsfm.IndexOf("[B]")).Is(BssomValue.Create(val["B"]));
            bsfm.ReadValue<BssomValue>(bsfm.IndexOf("[C]")).Is(BssomValue.Create(val["C"]));
        }

        [Fact]
        public void Map2_ReadBssomValue_FunctionIsCorrectly()
        {
            var val = RandomHelper.RandomValue<_class1>();
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);
            bsfm.ReadValue<BssomValue>(bsfm.IndexOf("[" + nameof(_class1.A) + "]")).Is(BssomValue.Create(val.A));
            bsfm.ReadValue<BssomValue>(bsfm.IndexOf("[" + nameof(_class1.B123123123) + "]")).Is(BssomValue.Create(val.B123123123));
            bsfm.ReadValue<BssomValue>(bsfm.IndexOf("[" + nameof(_class1.B1231231VV) + "]")).Is(BssomValue.Create(val.B1231231VV));
            bsfm.ReadValue<BssomValue>(bsfm.IndexOf("[" + nameof(_class1.C) + "]")).Is(BssomValue.Create(val.C));
        }

        [Theory]
        [InlineData(typeof(List<byte>))]//Array1
        [InlineData(typeof(List<int>))]//Array1
        [InlineData(typeof(List<decimal>))]//Array1
        [InlineData(typeof(List<DateTime>))]//Array1
        [InlineData(typeof(List<string>))]//Array2
        [InlineData(typeof(List<object>))]//Array2
        public void Array_ReadBssomValue_FunctionIsCorrectly(Type type)
        {
            var val = RandomHelper.RandomValue(type);
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);
            int count = ((ICollection)val).Count;
            for (int i = 0; i < count; i++)
            {
                bsfm.ReadValue<BssomValue>(bsfm.IndexOf($"${i}")).Is(BssomValue.Create(((IList)val)[i]));
            }
        }

        [Fact]
        public void Map_WriteField_TypeAndTypeLengthIsSame_IsCorrectly()
        {
            var val = RandomHelper.RandomValue<_class1>();
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);

            var val2 = RandomHelper.RandomValue<_class1>();
            bsfm.TryWrite(bsfm.IndexOf("[" + nameof(_class1.A) + "]"), val2.A).IsTrue();
            bsfm.TryWrite(bsfm.IndexOf("[" + nameof(_class1.B123123123) + "]"), val2.B123123123).IsTrue();
            bsfm.TryWrite(bsfm.IndexOf("[" + nameof(_class1.B1231231VV) + "]"), val2.B1231231VV).IsTrue();
            bsfm.TryWrite(bsfm.IndexOf("[" + nameof(_class1.C) + "]"), val2.C).IsTrue();

            var val3 = BssomSerializer.Deserialize<_class1>(buf);

            val3.A.Is(val2.A);
            val3.B123123123.Is(val2.B123123123);
            val3.B1231231VV.Is(val2.B1231231VV);
            val3.C.Is(val2.C);
        }

        [Fact]
        public void Map_WriteFieldWithBlankLength_InsufficientLengthWritten_IsCorrectly()
        {
            var val = RandomHelper.RandomValue<_class2>();
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);

            long A1 = RandomHelper.RandomValue<long>();
            bsfm.TryWrite(bsfm.IndexOf("[" + nameof(_class2.A) + "]"), A1).IsFalse();
        }

        [Fact]
        public void Map_WriteFieldWithBlankLength_IsExtraLengthWriting_FixTypeSize_IsCorrectly()
        {
            var val = RandomHelper.RandomValue<_class2>();
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);


            int B1 = RandomHelper.RandomValue<int>();
            bsfm.TryWrite(bsfm.IndexOf("[" + nameof(_class2.B) + "]"), B1).IsTrue();

            var val3 = BssomSerializer.Deserialize<BssomMap>(buf);
            val3["B"].Is(BssomValue.Create(B1));
            bsfm.ReadValue<int>(bsfm.IndexOf("[" + nameof(_class2.B) + "]")).Is(B1);
        }

        [Fact]
        public void Map_WriteFieldWithBlankLength_IsExtraLengthWriting_VariableTypeSize_IsCorrectly()
        {
            var val = new _class2() { C = "123456789" };
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);


            string C1 = "123";
            bsfm.TryWrite(bsfm.IndexOf("[" + nameof(_class2.C) + "]"), C1).IsTrue();

            var val3 = BssomSerializer.Deserialize<BssomMap>(buf);
            val3["C"].Is(BssomValue.Create(C1));
            bsfm.ReadValue<string>(bsfm.IndexOf("[" + nameof(_class2.C) + "]")).Is(C1);
        }

        [Theory]
        [InlineData((byte)10)]
        [InlineData((short)-10)]
        [InlineData((int)-10)]
        [InlineData((long)-120)]
        [InlineData((float)0.000006)]
        [InlineData((double)0.000006)]
        [InlineData('\u9999')]
        [InlineData("12345678901234567")]//17size-> Blank0 critical value
        public void Map_WriteFieldWithBlankLength_MultipleWritesOfInconsistentLength_IsCorrectly(object fieldValue)
        {
            var val = new _class2() { C = "123456789012345678" };//18size
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);

            var cOffset = bsfm.IndexOf("[" + nameof(_class2.C) + "]");

            bsfm.TryWrite(cOffset, fieldValue).IsTrue();
            bsfm.ReadValue<BssomValue>(cOffset).Is(BssomValue.Create(fieldValue));
        }

        [Theory]
        [InlineData(typeof(decimal), typeof(List<decimal>))]//Array1
        [InlineData(typeof(char), typeof(List<char>))]//Array1
        [InlineData(typeof(byte), typeof(List<byte>))]//Array1
        [InlineData(typeof(int), typeof(List<int>))]//Array1
        [InlineData(typeof(DateTime), typeof(List<DateTime>))]//Array1
        public void Array1_WriteField_TypeAndTypeLengthIsSame_IsCorrectly(Type eleType, Type arrayType)
        {
            var val = RandomHelper.RandomValue(arrayType);
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);

            var list = (IList)val;
            for (int i = 0; i < list.Count; i++)
            {
                var ran = RandomHelper.RandomValue(eleType);
                bsfm.TryWrite(bsfm.IndexOf($"${i}"), ran).IsTrue();
                bsfm.ReadValue<object>(bsfm.IndexOf($"${i}")).Is(ran);
            }
        }

        [Theory]
        [InlineData(typeof(int), typeof(List<decimal>))]//Array1
        [InlineData(typeof(int), typeof(List<char>))]//Array1
        [InlineData(typeof(int), typeof(List<double>))]//Array1
        [InlineData(typeof(int), typeof(List<DateTime>))]//Array1
        public void Array1_WriteField_ElementTypeMustCompleteConsistency_IsRequiredToWrite(Type eleType, Type arrayType)
        {
            var val = RandomHelper.RandomValue(arrayType);
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);
            var ran = RandomHelper.RandomValue(eleType);

            VerifyHelper.Throws<InvalidCastException>(() => bsfm.TryWrite(bsfm.IndexOf("$0"), ran));
        }

        [Theory]
        [InlineData(typeof(byte), typeof(List<string>))]//Array2
        [InlineData(typeof(byte), typeof(List<_class2>))]//Array2
        [InlineData(typeof(byte), typeof(List<List<object>>))]//Array2
        public void Array2_WriteFieldWithBlankLength_IsExtraLengthWriting_IsCorrectly(Type eleType, Type arrayType)
        {
            var val = RandomHelper.RandomValueWithOutStringEmpty(arrayType);
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);

            var list = (IList)val;
            for (int i = 0; i < list.Count; i++)
            {
                var ran = RandomHelper.RandomValue(eleType);
                var index = bsfm.IndexOf($"${i}");
                bsfm.TryWrite(index, ran).IsTrue();
                bsfm.ReadValue<object>(index).Is(ran);
            }
        }

        [Fact]
        public void Array2_WriteFieldWithBlankLength_EmptyInt16Code_Test()
        {
            var val = new List<byte[]> { new byte[300] };
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);

            int num = 3;
            var of = bsfm.IndexOf("$0");
            bsfm.TryWrite(of, num).IsTrue();
            bsfm.ReadValue<int>(of).Is(num);
        }

        [Fact]
        public void Array2_WriteFieldWithBlankLength_EmptyInt32Code_Test()
        {
            var val = new List<byte[]> { new byte[short.MaxValue + 1] };
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);

            int num = 3;
            var of = bsfm.IndexOf("$0");
            bsfm.TryWrite(of, num).IsTrue();
            bsfm.ReadValue<int>(of).Is(num);
        }

        [Theory]
        [InlineData((byte)10)]
        [InlineData((short)-10)]
        [InlineData((int)-10)]
        [InlineData((long)-120)]
        [InlineData((float)0.000006)]
        [InlineData((double)0.000006)]
        [InlineData('\u9999')]
        [InlineData("12345678901234567")]//17size-> Blank0 critical value
        public void Array2_WriteFieldWithBlankLength_MultipleWritesOfInconsistentLength_IsCorrectly(object fieldValue)
        {
            var val = new List<string>();//18size
            val.Add("123456789012345678");
            var buf = BssomSerializer.Serialize(val);

            var bsfm = new BssomFieldMarshaller(buf);

            var cOffset = bsfm.IndexOf("$0");

            bsfm.TryWrite(cOffset, fieldValue).IsTrue();
            bsfm.ReadValue<BssomValue>(cOffset).Is(BssomValue.Create(fieldValue));
        }

        [Theory]
        [InlineData(BssomValueType.Decimal, NativeBssomType.DecimalCode, true, typeof(List<decimal>), true)]//Array1
        [InlineData(BssomValueType.Char, NativeBssomType.CharCode, true, typeof(List<char>), true)]//Array1
        [InlineData(BssomValueType.Number, BssomType.UInt8Code, false, typeof(List<byte>), true)]//Array1
        [InlineData(BssomValueType.Number, BssomType.Int32Code, false, typeof(List<int>), true)]//Array1
        [InlineData(BssomValueType.DateTime, BssomType.DateTimeCode, false, typeof(List<DateTime>), true)]//Array1
        [InlineData(BssomValueType.DateTime, NativeBssomType.DateTimeCode, true, typeof(List<DateTime>), false)]//Array1
        [InlineData(BssomValueType.String, BssomType.StringCode, false, typeof(List<string>), false)]//Array2
        [InlineData(BssomValueType.Array, BssomType.Array2, false, typeof(List<List<string>>), false)]//Array2
        [InlineData(BssomValueType.Array, BssomType.Array1, false, typeof(List<List<byte>>), false)]//Array2
        [InlineData(BssomValueType.Map, BssomType.Map1, false, typeof(List<Dictionary<long, string>>), false)]//Array2
        [InlineData(BssomValueType.Map, BssomType.Map2, false, typeof(List<_class1>), false)]//Array2
        public void Array_ReadValueTypeAndTypeCode_IsCorrectly(BssomValueType valueType, byte typeCode, bool isNativeType, Type arrayType, bool isUseStantDateTime)
        {
            var val = RandomHelper.RandomValue(arrayType);
            var buf = BssomSerializer.Serialize(val, BssomSerializerOptions.Default.WithIsUseStandardDateTime(isUseStantDateTime));

            var bsfm = new BssomFieldMarshaller(buf);

            var list = (IList)val;
            for (int i = 0; i < list.Count; i++)
            {
                bsfm.ReadValueTypeCode(bsfm.IndexOf($"${i}"), out bool isNative).Is(typeCode);
                isNative.Is(isNativeType);

                bsfm.ReadValueType(bsfm.IndexOf($"${i}")).Is(valueType);
            }
        }





        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(char))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(List<int>))]
        public void ReadAllKeysByMapType_TypeIsNotMap_ThrowTypeCodeReadError(Type t)
        {
            var val = RandomHelper.RandomValue(t);
            var buf = BssomSerializer.Serialize(val);
            var bsfm = new BssomFieldMarshaller(buf);
            VerifyHelper.Throws<BssomSerializationOperationException>(() => bsfm.ReadAllKeysByMapType<object>(BssomFieldOffsetInfo.Zero),
                ex => ex.ErrorCode == BssomSerializationOperationException.SerializationErrorCode.IncorrectTypeCode);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_ReadAllKeysByMapType_IsCorrectly(bool isSerializeMap1Type)
        {
            var val = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, int>>();
            var buf = BssomSerializer.Serialize(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(isSerializeMap1Type));

            var bsfm = new BssomFieldMarshaller(buf);
            var keys = bsfm.ReadAllKeysByMapType<string>(BssomFieldOffsetInfo.Zero);
            foreach (var item in keys)
            {
                val.TryGetValue(item.Key, out int itemVal).IsTrue();
                itemVal.Is(bsfm.ReadValue<int>(item.Value));
                val.Remove(item.Key).IsTrue();
            }
        }

    }
    public class _class2
    {
        public int A;
        public DateTime B;
        public String C;
    }
    public class _class1
    {
        public DateTime A;
        public Guid B123123123;
        public int B1231231VV;
        public int C;
    }
    public class _class3
    {

    }
}
