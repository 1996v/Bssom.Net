using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.BssomBuffer;
using Xunit;
namespace Bssom.Serializer.Test
{
    public class BssomSchemaTest
    {
        [Fact]
        public void Map2_OneEqual_SchemaIsCorrectly()
        {
            AssertSchema("[12]EqualLast1 [13]NextOff(24) KeyBytes(49) KeyType(StringCode) ValOffset(24) [23]NoChildren", "1");
        }

        [Fact]
        public void Map2_WithOutKeyU64AndLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]EqualNext2 [13]NextOff(25) KeyBytes(49,49) KeyType(StringCode) ValOffset(51) [24]NoChildren " +
                "[25]EqualNext2 [26]NextOff(38) KeyBytes(50,50) KeyType(StringCode) ValOffset(56) [37]NoChildren " +
                "[38]EqualLast2 [39]NextOff(51) KeyBytes(51,51) KeyType(StringCode) ValOffset(61) [50]NoChildren"
                ,
                "11", "22", "33");
        }

        [Fact]
        public void Map2_HasOneEqual8_WithOutLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]EqualNext2 [13]NextOff(25) KeyBytes(50,50) KeyType(StringCode) ValOffset(57) [24]NoChildren " +
                "[25]EqualNext2 [26]NextOff(38) KeyBytes(51,51) KeyType(StringCode) ValOffset(62) [37]NoChildren " +
                "[38]EqualLast8 [39]NextOff(57) KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(67) [56]NoChildren"
                ,
                "12345678", "22", "33");
        }

        [Fact]
        public void Map2_HasOneEqualN_WithOutLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]EqualNext2 [13]NextOff(25) KeyBytes(50,50) KeyType(StringCode) ValOffset(63) [24]NoChildren " +
                "[25]EqualNext2 [26]NextOff(38) KeyBytes(51,51) KeyType(StringCode) ValOffset(68) [37]NoChildren " +
                "[38]EqualLastN [39]NextOff(63) KeyU64(4050765991979987505) [50]HasChildren " +
                        "[51]EqualLast1 [52]NextOff(63) KeyBytes(57) KeyType(StringCode) ValOffset(73) [62]NoChildren"
                 ,
                "123456789", "22", "33");
        }

        [Fact]
        public void Map2_Equal8AndHasChildren_WithOutLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]EqualLast8 [13]NextOff(55) KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(55) [30]HasChildren " +
                "[31]EqualNext1 [32]NextOff(43) KeyBytes(49) KeyType(StringCode) ValOffset(60) [42]NoChildren " +
                "[43]EqualLast1 [44]NextOff(55) KeyBytes(50) KeyType(StringCode) ValOffset(65) [54]NoChildren"
                ,
                "12345678", "123456781", "123456782");
        }

        [Fact]
        public void Map2_FourKey_OnlyOneLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]LessThen2 [13]NextOff(44) KeyBytes(50,50) " +
                        "[18]EqualNext2 [19]NextOff(31) KeyBytes(49,49) KeyType(StringCode) ValOffset(71) [30]NoChildren " +
                        "[31]EqualLast2 [32]NextOff(44) KeyBytes(50,50) KeyType(StringCode) ValOffset(76) [43]NoChildren " +
                "[44]LessElse " +
                        "[45]EqualNext2 [46]NextOff(58) KeyBytes(51,51) KeyType(StringCode) ValOffset(81) [57]NoChildren " +
                        "[58]EqualLast2 [59]NextOff(71) KeyBytes(52,52) KeyType(StringCode) ValOffset(86) [70]NoChildren"
                 ,
                "11", "22", "33", "44");
        }

        [Fact]
        public void Map2_Equal8AndHasChildren_HavingSameParentKey_WithOneLessBranch_SchemaIsCorrectly()
        {
            AssertSchema("" +
                "[12]EqualLast8 [13]NextOff(97) KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(97) [30]HasChildren " +
                        "[31]LessThen1 [32]NextOff(60) KeyBytes(50) " +
                                "[36]EqualNext1 [37]NextOff(48) KeyBytes(49) KeyType(StringCode) ValOffset(102) [47]NoChildren " +
                                "[48]EqualLast1 [49]NextOff(60) KeyBytes(50) KeyType(StringCode) ValOffset(107) [59]NoChildren " +
                        "[60]LessElse " +
                                "[61]EqualNext1 [62]NextOff(73) KeyBytes(51) KeyType(StringCode) ValOffset(112) [72]NoChildren " +
                                "[73]EqualNext1 [74]NextOff(85) KeyBytes(52) KeyType(StringCode) ValOffset(117) [84]NoChildren " +
                                "[85]EqualLast1 [86]NextOff(97) KeyBytes(53) KeyType(StringCode) ValOffset(122) [96]NoChildren"
             ,
             "12345678", "123456781", "123456782", "123456783", "123456784", "123456785");
        }

        [Fact]
        public void Map2_Equal8AndHasChildren_WithLessBranch_SchemaIsCorrectly()
        {
            AssertSchema("" +
                "[12]LessThen2 [13]NextOff(43) KeyBytes(98,98) " +
                        "[18]EqualNext1 [19]NextOff(30) KeyBytes(97) KeyType(StringCode) ValOffset(100) [29]NoChildren " +
                        "[30]EqualLast2 [31]NextOff(43) KeyBytes(98,98) KeyType(StringCode) ValOffset(105) [42]NoChildren " +
                "[43]LessElse " +
                        "[44]EqualNext2 [45]NextOff(57) KeyBytes(99,99) KeyType(StringCode) ValOffset(110) [56]NoChildren " +
                        "[57]EqualLast8 [58]NextOff(100) KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(115) [75]HasChildren " +
                                "[76]EqualNext1 [77]NextOff(88) KeyBytes(49) KeyType(StringCode) ValOffset(120) [87]NoChildren " +
                                "[88]EqualLast1 [89]NextOff(100) KeyBytes(50) KeyType(StringCode) ValOffset(125) [99]NoChildren"
                ,
                "12345678", "123456781", "123456782", "a", "bb", "cc");
        }

        [Fact]
        public void Map2_Complex_SchemaIsCorrectly()
        {
            AssertSchema(
           "[12]LessThen3 [13]NextOff(46) KeyBytes(103,103,103) " +
                  "[19]EqualNext2 [20]NextOff(32) KeyBytes(97,97) KeyType(StringCode) ValOffset(190) [31]NoChildren " +
                  "[32]EqualLast3 [33]NextOff(46) KeyBytes(103,103,103) KeyType(StringCode) ValOffset(195) [45]NoChildren " +
           "[46]LessElse " +
                  "[47]EqualNext8 [48]NextOff(78) KeyU64(3544726963336143987) KeyType(StringCode) ValOffset(200) [65]HasChildren " +
                            "[66]EqualLast1 [67]NextOff(78) KeyBytes(50) KeyType(StringCode) ValOffset(205) [77]NoChildren " +
                   "[78]EqualNextN [79]NextOff(105) KeyU64(3978425819141910903) [90]HasChildren " +
                            "[91]EqualLast3 [92]NextOff(105) KeyBytes(56,57,48) KeyType(StringCode) ValOffset(210) [104]NoChildren " +
                   "[105]EqualLast8 [106]NextOff(190) KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(215) [123]HasChildren " +
                            "[124]LessThen1 [125]NextOff(153) KeyBytes(50) " +
                                "[129]EqualNext1 [130]NextOff(141) KeyBytes(49) KeyType(StringCode) ValOffset(220) [140]NoChildren " +
                                "[141]EqualLast1 [142]NextOff(153) KeyBytes(50) KeyType(StringCode) ValOffset(225) [152]NoChildren " +
                            "[153]LessElse " +
                                "[154]EqualNext1 [155]NextOff(166) KeyBytes(51) KeyType(StringCode) ValOffset(230) [165]NoChildren " +
                                "[166]EqualNext1 [167]NextOff(178) KeyBytes(52) KeyType(StringCode) ValOffset(235) [177]NoChildren " +
                                "[178]EqualLast1 [179]NextOff(190) KeyBytes(53) KeyType(StringCode) ValOffset(240) [189]NoChildren"
           ,
                "12345678", "123456781", "123456782", "123456783", "123456784", "123456785", "aa", "sdfsdf11", "sdfsdf112", "w1234567890", "ggg");
        }

        private Dictionary<string, int> GenerateDict(params string[] keys)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (var item in keys)
            {
                dict.Add(item, 1);
            }
            return dict;
        }

        private void AssertSchema(string schemaString, params string[] keys)
        {
            var buf = BssomSerializer.Serialize(GenerateDict(keys), BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
            var reader = new BssomReader(new SimpleBufferWriter(buf));

            reader.EnsureType(BssomType.Map2);
            string ss = BssMapObjMarshal.GetSchemaString(ref reader);
            Assert.Equal(schemaString, ss);
        }
    }
}
