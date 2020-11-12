using Bssom.Serializer.Internal;
using System.Reflection;
using Xunit;
namespace Bssom.Serializer.Test
{
    public class ObjectSerializationInfoTest
    {
        public class _ReadOnlyField
        {
            public readonly int r;
            public int c;
        }

        [Fact]
        public void ReadOnlyFieldsIsNotSerialized()
        {
            var objserInfo = new ObjectSerializationInfo(typeof(_ReadOnlyField), true);
            objserInfo.SerializeMemberInfos.Length.Is(1);
            objserInfo.SerializeMemberInfos[0].Name.Is(nameof(_ReadOnlyField.c));
        }

        public class _PropertyCompilerGenerated
        {
            public int r { get; set; }
        }

        [Fact]
        public void PropertyCompilerGeneratedFieldsIsNotSerialized()
        {
            var objserInfo = new ObjectSerializationInfo(typeof(_PropertyCompilerGenerated), true);
            objserInfo.SerializeMemberInfos.Length.Is(1);
            objserInfo.SerializeMemberInfos[0].Name.Is(nameof(_PropertyCompilerGenerated.r));
        }

        public class _PrivateMembers
        {
            private int a { get; set; }
            private int b { get; set; }
        }

        [Fact]
        public void PrivateMembersIsSerialized()
        {
            var objserInfo = new ObjectSerializationInfo(typeof(_PrivateMembers), true);
            objserInfo.SerializeMemberInfos.Length.Is(2);
            objserInfo.SerializeMemberInfos[0].Name.Is("a");
            objserInfo.SerializeMemberInfos[1].Name.Is("b");
        }

        [Fact]
        public void PrivateMembersNonPublicSetterIndex_IsCorrectly()
        {
            var objserInfo = new ObjectSerializationInfo(typeof(_PrivateMembers), true);
            objserInfo.SerializeMemberInfos.Length.Is(2);
            objserInfo.SerializeMemberInfos[0].NonPublicSetterIndex.Is(0);
            objserInfo.SerializeMemberInfos[1].NonPublicSetterIndex.Is(1);
        }
    }
}
