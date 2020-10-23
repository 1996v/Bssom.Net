using System.Collections.Generic;
using System.Linq.Expressions;
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
            AssertSchema("[12]EqualLast1 KeyBytes(49) KeyType(StringCode) ValOffset(21) [20]NoChildren", "1");
        }

        [Fact]
        public void Map2_WithOutKeyU64AndLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]EqualNext2 [13]NextOff(25) KeyBytes(49,49) KeyType(StringCode) ValOffset(48) [24]NoChildren " +
                "[25]EqualNext2 [26]NextOff(38) KeyBytes(50,50) KeyType(StringCode) ValOffset(53) [37]NoChildren " +
                "[38]EqualLast2 KeyBytes(51,51) KeyType(StringCode) ValOffset(58) [47]NoChildren"
                ,
                "11", "22", "33");
        }

        [Fact]
        public void Map2_HasOneEqual8_WithOutLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]EqualNext2 [13]NextOff(25) KeyBytes(50,50) KeyType(StringCode) ValOffset(54) [24]NoChildren " +
                "[25]EqualNext2 [26]NextOff(38) KeyBytes(51,51) KeyType(StringCode) ValOffset(59) [37]NoChildren " +
                "[38]EqualLast8 KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(64) [53]NoChildren"
                ,
                "12345678", "22", "33");
        }

        [Fact]
        public void Map2_HasOneEqualN_WithOutLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]EqualNext2 [13]NextOff(25) KeyBytes(50,50) KeyType(StringCode) ValOffset(57) [24]NoChildren " +
                "[25]EqualNext2 [26]NextOff(38) KeyBytes(51,51) KeyType(StringCode) ValOffset(62) [37]NoChildren " +
                "[38]EqualLastN KeyU64(4050765991979987505) [47]HasChildren " +
                        "[48]EqualLast1 KeyBytes(57) KeyType(StringCode) ValOffset(67) [56]NoChildren"
                 ,
                "123456789", "22", "33");
        }

        [Fact]
        public void Map2_Equal8AndHasChildren_WithOutLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]EqualLast8 KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(49) [27]HasChildren " +
                "[28]EqualNext1 [29]NextOff(40) KeyBytes(49) KeyType(StringCode) ValOffset(54) [39]NoChildren " +
                "[40]EqualLast1 KeyBytes(50) KeyType(StringCode) ValOffset(59) [48]NoChildren"
                ,
                "12345678", "123456781", "123456782");
        }

        [Fact]
        public void Map2_FourKey_OnlyOneLessBranch_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]LessThen2 [13]NextOff(41) KeyBytes(50,50) " +
                        "[18]EqualNext2 [19]NextOff(31) KeyBytes(49,49) KeyType(StringCode) ValOffset(65) [30]NoChildren " +
                        "[31]EqualLast2 KeyBytes(50,50) KeyType(StringCode) ValOffset(70) [40]NoChildren " +
                "[41]LessElse " +
                        "[42]EqualNext2 [43]NextOff(55) KeyBytes(51,51) KeyType(StringCode) ValOffset(75) [54]NoChildren " +
                        "[55]EqualLast2 KeyBytes(52,52) KeyType(StringCode) ValOffset(80) [64]NoChildren"
                 ,
                "11", "22", "33", "44");
        }

        [Fact]
        public void Map2_Equal8AndHasChildren_HavingSameParentKey_WithOneLessBranch_SchemaIsCorrectly()
        {
            AssertSchema("" +
                "[12]EqualLast8 KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(88) [27]HasChildren " +
                        "[28]LessThen1 [29]NextOff(54) KeyBytes(50) " +
                                "[33]EqualNext1 [34]NextOff(45) KeyBytes(49) KeyType(StringCode) ValOffset(93) [44]NoChildren " +
                                "[45]EqualLast1 KeyBytes(50) KeyType(StringCode) ValOffset(98) [53]NoChildren " +
                        "[54]LessElse " +
                                "[55]EqualNext1 [56]NextOff(67) KeyBytes(51) KeyType(StringCode) ValOffset(103) [66]NoChildren " +
                                "[67]EqualNext1 [68]NextOff(79) KeyBytes(52) KeyType(StringCode) ValOffset(108) [78]NoChildren " +
                                "[79]EqualLast1 KeyBytes(53) KeyType(StringCode) ValOffset(113) [87]NoChildren"
             ,
             "12345678", "123456781", "123456782", "123456783", "123456784", "123456785");
        }

        [Fact]
        public void Map2_Equal8AndHasChildren_WithLessBranch_SchemaIsCorrectly()
        {
            AssertSchema("" +
                "[12]LessThen2 [13]NextOff(40) KeyBytes(98,98) " +
                        "[18]EqualNext1 [19]NextOff(30) KeyBytes(97) KeyType(StringCode) ValOffset(91) [29]NoChildren " +
                        "[30]EqualLast2 KeyBytes(98,98) KeyType(StringCode) ValOffset(96) [39]NoChildren " +
                "[40]LessElse " +
                        "[41]EqualNext2 [42]NextOff(54) KeyBytes(99,99) KeyType(StringCode) ValOffset(101) [53]NoChildren " +
                        "[54]EqualLast8 KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(106) [69]HasChildren " +
                                "[70]EqualNext1 [71]NextOff(82) KeyBytes(49) KeyType(StringCode) ValOffset(111) [81]NoChildren " +
                                "[82]EqualLast1 KeyBytes(50) KeyType(StringCode) ValOffset(116) [90]NoChildren"
                ,
                "12345678", "123456781", "123456782", "a", "bb", "cc");
        }

        [Fact]
        public void Map2_Complex_SchemaIsCorrectly()
        {
            AssertSchema(
           "[12]LessThen3 [13]NextOff(43) KeyBytes(103,103,103) " +
                  "[19]EqualNext2 [20]NextOff(32) KeyBytes(97,97) KeyType(StringCode) ValOffset(172) [31]NoChildren " +
                  "[32]EqualLast3 KeyBytes(103,103,103) KeyType(StringCode) ValOffset(177) [42]NoChildren " +
           "[43]LessElse " +
                  "[44]EqualNext8 [45]NextOff(72) KeyU64(3544726963336143987) KeyType(StringCode) ValOffset(182) [62]HasChildren " +
                            "[63]EqualLast1 KeyBytes(50) KeyType(StringCode) ValOffset(187) [71]NoChildren " +
                   "[72]EqualNextN [73]NextOff(96) KeyU64(3978425819141910903) [84]HasChildren " +
                            "[85]EqualLast3 KeyBytes(56,57,48) KeyType(StringCode) ValOffset(192) [95]NoChildren " +
                   "[96]EqualLast8 KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(197) [111]HasChildren " +
                            "[112]LessThen1 [113]NextOff(138) KeyBytes(50) " +
                                "[117]EqualNext1 [118]NextOff(129) KeyBytes(49) KeyType(StringCode) ValOffset(202) [128]NoChildren " +
                                "[129]EqualLast1 KeyBytes(50) KeyType(StringCode) ValOffset(207) [137]NoChildren " +
                            "[138]LessElse " +
                                "[139]EqualNext1 [140]NextOff(151) KeyBytes(51) KeyType(StringCode) ValOffset(212) [150]NoChildren " +
                                "[151]EqualNext1 [152]NextOff(163) KeyBytes(52) KeyType(StringCode) ValOffset(217) [162]NoChildren " +
                                "[163]EqualLast1 KeyBytes(53) KeyType(StringCode) ValOffset(222) [171]NoChildren"
           ,
                "12345678", "123456781", "123456782", "123456783", "123456784", "123456785", "aa", "sdfsdf11", "sdfsdf112", "w1234567890", "ggg");
        }

        [Fact]
        public void Map2_Complex2_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]LessThen8 [13]NextOff(63) KeyBytes(97,49,50,51,52,53,54,55) " +
                "[24]EqualNext2 [25]NextOff(37) KeyBytes(112,49) KeyType(StringCode) ValOffset(113) [36]NoChildren " +
                "[37]EqualLast8 KeyU64(3978425819141910881) KeyType(StringCode) ValOffset(118) [52]HasChildren " +
                "[53]EqualLast2 KeyBytes(98,49) KeyType(StringCode) ValOffset(123) [62]NoChildren " +
                "[63]LessElse " +
                "[64]EqualNextN [65]NextOff(87) KeyU64(3978425819141910883) [76]HasChildren " +
                "[77]EqualLast2 KeyBytes(100,49) KeyType(StringCode) ValOffset(128) [86]NoChildren " +
                "[87]EqualLastN KeyU64(3978425819141910885) [96]HasChildren " +
                "[97]EqualLast8 KeyU64(3978425819141910898) KeyType(StringCode) ValOffset(133) [112]NoChildren",
                "a1234567b1", "a1234567", "c1234567d1", "p1", "e1234567r1234567");
        }

        [Fact]
        public void Map2_Complex3_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]LessThen1 [13]NextOff(65) KeyBytes(52) " +
                     "[17]LessThen1 [18]NextOff(43) KeyBytes(50) " +
                         "[22]EqualNext1 [23]NextOff(34) KeyBytes(49) KeyType(StringCode) ValOffset(126) [33]NoChildren " +
                         "[34]EqualLast1 KeyBytes(50) KeyType(StringCode) ValOffset(131) [42]NoChildren " +
                     "[43]LessElse " +
                         "[44]EqualNext1 [45]NextOff(56) KeyBytes(51) KeyType(StringCode) ValOffset(136) [55]NoChildren " +
                         "[56]EqualLast1 KeyBytes(52) KeyType(StringCode) ValOffset(141) [64]NoChildren [65]" +
                "LessElse " +
                     "[66]LessThen1 [67]NextOff(92) KeyBytes(54) " +
                         "[71]EqualNext1 [72]NextOff(83) KeyBytes(53) KeyType(StringCode) ValOffset(146) [82]NoChildren " +
                         "[83]EqualLast1 KeyBytes(54) KeyType(StringCode) ValOffset(151) [91]NoChildren " +
                     "[92]LessElse " +
                         "[93]EqualNext1 [94]NextOff(105) KeyBytes(55) KeyType(StringCode) ValOffset(156) [104]NoChildren " +
                        "[105]EqualNext1 [106]NextOff(117) KeyBytes(56) KeyType(StringCode) ValOffset(161) [116]NoChildren " +
                        "[117]EqualLast1 KeyBytes(57) KeyType(StringCode) ValOffset(166) [125]NoChildren",

                "1", "2", "3", "4", "5","6","7","8","9");
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
