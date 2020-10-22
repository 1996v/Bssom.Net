using Xunit;
namespace Bssom.Serializer.Test
{


    public class IgnoreKeyAttributeTest
    {
        [Fact]
        public void Test()
        {
            var val2 = RandomHelper.RandomValue<_Attribute_Class_1>();
            var buf = BssomSerializer.Serialize(val2);
            var bsfm = new BssomFieldMarshaller(buf);
            bsfm.IndexOf("[Name]").IsExists.IsFalse();
            bsfm.IndexOf("[Sex]").IsExists.IsTrue();
        }

        public class _Attribute_Class_1
        {
            [IgnoreKey]
            public string Name;
            public int Sex;
        }
    }




}
