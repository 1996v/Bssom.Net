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
                "[12]EqualNext2 [13]NextOff(25) KeyBytes(50,50) KeyType(StringCode) ValOffset(56) [24]NoChildren " +
                "[25]EqualNext2 [26]NextOff(38) KeyBytes(51,51) KeyType(StringCode) ValOffset(61) [37]NoChildren " +
                "[38]EqualLastN KeyU64(4050765991979987505) " +
                        "[47]EqualLast1 KeyBytes(57) KeyType(StringCode) ValOffset(66) [55]NoChildren"
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
                  "[19]EqualNext2 [20]NextOff(32) KeyBytes(97,97) KeyType(StringCode) ValOffset(171) [31]NoChildren " +
                  "[32]EqualLast3 KeyBytes(103,103,103) KeyType(StringCode) ValOffset(176) [42]NoChildren " +
           "[43]LessElse " +
                  "[44]EqualNext8 [45]NextOff(72) KeyU64(3544726963336143987) KeyType(StringCode) ValOffset(181) [62]HasChildren " +
                            "[63]EqualLast1 KeyBytes(50) KeyType(StringCode) ValOffset(186) [71]NoChildren " +
                   "[72]EqualNextN [73]NextOff(95) KeyU64(3978425819141910903) " +
                            "[84]EqualLast3 KeyBytes(56,57,48) KeyType(StringCode) ValOffset(191) [94]NoChildren " +
                   "[95]EqualLast8 KeyU64(4050765991979987505) KeyType(StringCode) ValOffset(196) [110]HasChildren " +
                            "[111]LessThen1 [112]NextOff(137) KeyBytes(50) " +
                                "[116]EqualNext1 [117]NextOff(128) KeyBytes(49) KeyType(StringCode) ValOffset(201) [127]NoChildren " +
                                "[128]EqualLast1 KeyBytes(50) KeyType(StringCode) ValOffset(206) [136]NoChildren " +
                            "[137]LessElse " +
                                "[138]EqualNext1 [139]NextOff(150) KeyBytes(51) KeyType(StringCode) ValOffset(211) [149]NoChildren " +
                                "[150]EqualNext1 [151]NextOff(162) KeyBytes(52) KeyType(StringCode) ValOffset(216) [161]NoChildren " +
                                "[162]EqualLast1 KeyBytes(53) KeyType(StringCode) ValOffset(221) [170]NoChildren"
           ,
                "12345678", "123456781", "123456782", "123456783", "123456784", "123456785", "aa", "sdfsdf11", "sdfsdf112", "w1234567890", "ggg");
        }

        [Fact]
        public void Map2_Complex2_SchemaIsCorrectly()
        {
            AssertSchema(
                "[12]LessThen8 [13]NextOff(63) KeyU64(3978425819141910881) " +
                     "[24]EqualNext2 [25]NextOff(37) KeyBytes(112,49) KeyType(StringCode) ValOffset(111) [36]NoChildren " +
                     "[37]EqualLast8 KeyU64(3978425819141910881) KeyType(StringCode) ValOffset(116) [52]HasChildren " +
                          "[53]EqualLast2 KeyBytes(98,49) KeyType(StringCode) ValOffset(121) [62]NoChildren " +
                "[63]LessElse " +
                     "[64]EqualNextN [65]NextOff(86) KeyU64(3978425819141910883) " +
                          "[76]EqualLast2 KeyBytes(100,49) KeyType(StringCode) ValOffset(126) [85]NoChildren " +
                     "[86]EqualLastN KeyU64(3978425819141910885) " +
                          "[95]EqualLast8 KeyU64(3978425819141910898) KeyType(StringCode) ValOffset(131) [110]NoChildren",
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

                "1", "2", "3", "4", "5", "6", "7", "8", "9");
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
