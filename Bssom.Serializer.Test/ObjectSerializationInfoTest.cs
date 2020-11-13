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

        public class _base { 
            
            protected int A { get; set; }

            public int B;

        }
        public class _sub:_base {

            private int C;

            internal int D;

            public int E;
        }

        [Fact]
        public void ExpentdType_PublicMembersIsSerialized()
        {
            var objserInfo = new ObjectSerializationInfo(typeof(_sub), false);
            objserInfo.SerializeMemberInfos.Length.Is(2);
            objserInfo.SerializeMemberInfos[0].Name.Is(nameof(_sub.E));
            objserInfo.SerializeMemberInfos[1].Name.Is(nameof(_base.B));
        }

        [Fact]
        public void ExpentdType_PrivateMembersNonPublicSetterIndex_IsCorrectly()
        {
            var objserInfo = new ObjectSerializationInfo(typeof(_sub), true);
            objserInfo.SerializeMemberInfos.Length.Is(5);
            objserInfo.SerializeMemberInfos[0].NonPublicSetterIndex.Is(0);
            objserInfo.SerializeMemberInfos[1].NonPublicSetterIndex.Is(1);
            objserInfo.SerializeMemberInfos[2].NonPublicSetterIndex.Is(2);
            objserInfo.SerializeMemberInfos[0].Name.Is("A");
            objserInfo.SerializeMemberInfos[1].Name.Is("C");
            objserInfo.SerializeMemberInfos[2].Name.Is("D");

            objserInfo.SerializeMemberInfos[3].NonPublicSetterIndex.Is(-1);
            objserInfo.SerializeMemberInfos[4].NonPublicSetterIndex.Is(-1);
        }
    }
}
