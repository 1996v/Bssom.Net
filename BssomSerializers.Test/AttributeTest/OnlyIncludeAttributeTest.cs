using Xunit;
namespace BssomSerializers.Test
{
    public class OnlyIncludeAttributeTest
    {
        [Fact]
        public void SingleOnlyIncludeTest()
        {
            var val2 = RandomHelper.RandomValue<_Attribute_Class_1>();

            var buf = BssomSerializer.Serialize(val2);
            var bsfm = new BssomFieldMarshaller(buf);
            bsfm.IndexOf("[Name]").IsExists.IsFalse();
            bsfm.IndexOf("[Sex]").IsExists.IsTrue();
        }

        public class _Attribute_Class_1
        {
            public string Name;
            [OnlyInclude]
            public int Sex;
        }

        [Fact]
        public void MultipleOnlyIncludeTest()
        {
            var val2 = RandomHelper.RandomValue<_Attribute_Class_2>();

            var buf = BssomSerializer.Serialize(val2);
            var map = (BssomMap)BssomSerializer.Deserialize<object>(buf);
            map.Count.Is(2);
            map["Sex"].Is(val2.Sex);
            map["Sex2"].Is(val2.Sex2);

            var bsfm = new BssomFieldMarshaller(buf);
            bsfm.IndexOf("[Sex]").IsExists.IsTrue();
            bsfm.IndexOf("[Sex2]").IsExists.IsTrue();

            bsfm.IndexOf("[Name]").IsExists.IsFalse();
            bsfm.IndexOf("[Name2]").IsExists.IsFalse();
            bsfm.IndexOf("[Name3]").IsExists.IsFalse();
            bsfm.IndexOf("[Name4]").IsExists.IsFalse();
        }

        public class _Attribute_Class_2
        {
            [IgnoreKey]
            public string Name;
            public string Name2;
            public string Name3;
            public string Name4;

            [OnlyInclude]
            public int Sex;
            [OnlyInclude]
            public int Sex2;
        }
    }




}
