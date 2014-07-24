namespace System.Collections.Test.Generic.BloomFilter.HashFunctionGenerators
{
    using System.Collections.Generic;
using System.Collections.Generic.BloomFilter.HashFunctionGenerators;
using System.Linq;
using System.Security.Cryptography;
using Xunit;

    public abstract class HashAlgorithm_HashFunctionGeneratorTests<ElementT, HashAlgorithmT, HashAlgorithm_HashFunctionGeneratorT>
        : IHashFunctionGeneratorTests<ElementT, HashAlgorithm_HashFunctionGeneratorT>
        where HashAlgorithmT : HashAlgorithm, new()
        where HashAlgorithm_HashFunctionGeneratorT : HashAlgorithm_HashFunctionGenerator<ElementT, HashAlgorithmT>, new()
    {
        protected override int MaxBitsGeneratable
        {
            get 
            {
                using (var ha = new HashAlgorithmT())
                    return ha.HashSize;
            }
        }
    }


    public abstract class HashAlgorithmTypeBase_HashAlgorithm_HashFunctionGeneratorTests<HashAlgorithmT>
        : HashAlgorithm_HashFunctionGeneratorTests<int, HashAlgorithmT, HashAlgorithm_HashFunctionGenerator<int, HashAlgorithmT>>
        where HashAlgorithmT : HashAlgorithm, new()
    {
        protected override int ElementA { get { return 0; } }

        protected abstract int Single16_ExpectedValue { get; }
        protected abstract IEnumerable<int> Multiple16_ExpectedValues { get; }
        protected abstract IEnumerable<int> Multiple19_ExpectedValues { get; }

        [Fact]
        protected void HashFunctionGenerator_HashAlgorithm_HashAlgorithmTypeBase_Single16()
        {
            var hfg = new HashAlgorithm_HashFunctionGenerator<int, HashAlgorithmT>();

            var hashFunction = hfg.Generate(1 << 16, 1);
            var hashResult = hashFunction(2548707).ToList();

            Assert.Equal(1, hashResult.Count);
            Assert.Equal(Single16_ExpectedValue, hashResult[0]);
        }


        [Fact]
        protected void HashFunctionGenerator_HashAlgorithm_HashAlgorithmTypeBase_Multiple16()
        {
            var expectedValuesList = Multiple16_ExpectedValues.ToList();
            Assert.True(expectedValuesList.Count > 1);

            var hfg = new HashAlgorithm_HashFunctionGenerator<int, HashAlgorithmT>();
            var hashFunction = hfg.Generate(1 << 16, expectedValuesList.Count);


            // 2548707 is serialized to 
            // [00, 01, 00, 00, 00, ff, ff, ff, ff, 01, 00, 00, 00, 00, 00, 00,
            //  00, 04, 01, 00, 00, 00, 0c, 53, 79, 73, 74, 65, 6d, 2e, 49, 6e, 
            //  74, 33, 32, 01, 00, 00, 00, 07, 6d, 5f, 76, 61, 6c, 75, 65, 00, 
            //  08, e3, e3, 26, 00, 0b]
            var hashResults = hashFunction(2548707).ToList();
            foreach (var hashResult in hashResults)
                Assert.True(expectedValuesList.Remove(hashResult));

            Assert.Empty(expectedValuesList);
        }

        [Fact]
        protected void HashFunctionGenerator_HashAlgorithm_HashAlgorithmTypeBase_Multiple19()
        {
            var expectedValuesList = Multiple19_ExpectedValues.ToList();
            Assert.True(expectedValuesList.Count > 1);

            var hfg = new HashAlgorithm_HashFunctionGenerator<int, HashAlgorithmT>();
            var hashFunction = hfg.Generate(1 << 19, expectedValuesList.Count);

            var hashResults = hashFunction(2548707).ToList();
            foreach (var hashResult in hashResults)
                Assert.True(expectedValuesList.Remove(hashResult));

            Assert.Empty(expectedValuesList);
        }
    }


    public abstract class ElementTypeBase_HashAlgorithm_HashFunctionGeneratorTests<ElementT>
        : HashAlgorithm_HashFunctionGeneratorTests<ElementT, SHA512Managed, HashAlgorithm_HashFunctionGenerator<ElementT, SHA512Managed>>
    {

    }

    #region Concrete Test Classes

    #region HashAlgorithm Tests

    public class SHA1Managed_HashAlgorithm_HashFunctionGeneratorTests
        : HashAlgorithmTypeBase_HashAlgorithm_HashFunctionGeneratorTests<SHA1Managed>
    {
        // D5 62 10 BB D5 63 31 A8 19 E3 E5 6E 07 18 4C AB
        // 27 84 F9 5E

        protected override int Single16_ExpectedValue { get { return 0x62D5; } }

        protected override IEnumerable<int> Multiple16_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x62D5, 0xBB10, 0x63D5, 0xA831, 0xE319, 0x6EE5, 0x1807, 0xAB4C,
                    0x8427, 0x5EF9
                };
            }
        }

        protected override IEnumerable<int> Multiple19_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x062D5, 0x2B762
                };
            }
        }
    }

    public class SHA256Managed_HashAlgorithm_HashFunctionGeneratorTests
        : HashAlgorithmTypeBase_HashAlgorithm_HashFunctionGeneratorTests<SHA256Managed>
    {
        // 8B 35 8C 49 C8 63 08 02 47 87 FF C7 E8 66 3E D8
        // ED F8 E3 FB 58 31 42 89 59 DC 28 2F F9 9F EF F9

        protected override int Single16_ExpectedValue { get { return 0x358B; } }

        protected override IEnumerable<int> Multiple16_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x358B, 0x498C, 0x63C8, 0x0208, 0x8747, 0xC7FF, 0x66E8, 0xD83E,
                    0xF8ED, 0xFBE3, 0x3158, 0x8942, 0xDC59, 0x2F28, 0x9FF9, 0xF9EF
                };
            }
        }

        protected override IEnumerable<int> Multiple19_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x4358B,
                    0x10931
                };
            }
        }
    }

    public class SHA384Managed_HashAlgorithm_HashFunctionGeneratorTests
        : HashAlgorithmTypeBase_HashAlgorithm_HashFunctionGeneratorTests<SHA384Managed>
    {
        // 81 9A 91 31 98 D0 2D 9D FE 43 36 99 3F 53 46 00
        // 46 54 88 91 0B E9 8F 70 34 35 00 0E 1F 26 27 B5
        // 64 C8 2D DC 1B 81 BF 91 D4 FA 63 1C 36 2E 72 F1

        protected override int Single16_ExpectedValue { get { return 0x9A81; } }

        protected override IEnumerable<int> Multiple16_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x9A81, 0x3191, 0xD098, 0x9D2D, 0x43FE, 0x9936, 0x533F, 0x0046,
                    0x5446, 0x9188, 0xE90B, 0x708F, 0x3534, 0x0E00, 0x261F, 0xB527,
                    0xC864, 0xDC2D, 0x811B, 0x91BF, 0xFAD4, 0x1C63, 0x2E36, 0xF172
                };
            }
        }

        protected override IEnumerable<int> Multiple19_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x19A81,
                    0x30632
                };
            }
        }
    }

    public class SHA512Managed_HashAlgorithm_HashFunctionGeneratorTests
        : HashAlgorithmTypeBase_HashAlgorithm_HashFunctionGeneratorTests<SHA512Managed>
    {
        // 72 1F ED C7 BF 51 64 60 2B 62 4F 32 F1 E0 77 F3
        // 45 9E CA 18 66 87 A7 4B F6 40 E7 8F 88 26 1C 36
        // D6 AB 6E ED 21 B5 27 95 8F 6C DB 5B B0 C5 CA F2
        // 7D FC 03 6B 9D F9 6E D9 B2 6A 3C 2D 7A C3 21 45

        protected override int Single16_ExpectedValue { get { return 0x1F72; } }

        protected override IEnumerable<int> Multiple16_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x1F72, 0xC7ED, 0x51BF, 0x6064, 0x622B, 0x324F, 0xE0F1, 0xF377,
                    0x9E45, 0x18CA, 0x8766, 0x4BA7, 0x40F6, 0x8FE7, 0x2688, 0x361C,
                    0xABD6, 0xED6E, 0xB521, 0x9527, 0x6C8F, 0x5BDB, 0xC5B0, 0xF2CA,
                    0xFC7D, 0x6B03, 0xF99D, 0xD96E, 0x6AB2, 0x2D3C, 0xC37A, 0x4521
                };
            }
        }

        protected override IEnumerable<int> Multiple19_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x51F72,
                    0x7F8FD
                };
            }
        }
    }

    public class MD5CryptoServiceProvider_HashAlgorithm_HashFunctionGeneratorTests
        : HashAlgorithmTypeBase_HashAlgorithm_HashFunctionGeneratorTests<MD5CryptoServiceProvider>
    {
        // AB 63 A8 E7 EC 8F 04 C0 74 20 FC C1 EF 4F D6 58

        protected override int Single16_ExpectedValue { get { return 0x63AB; } }

        protected override IEnumerable<int> Multiple16_ExpectedValues
        { 
            get
            {  
                return new[] { 
                    0x63AB, 0xE7A8, 0x8FEC, 0xC004, 0x2074, 0xC1FC, 0x4FEF, 0x58D6
                };
            } 
        }

        protected override IEnumerable<int> Multiple19_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x063AB,
                    0x59CF5
                };
            }
        }
    }

    public class RIPEMD160Managed_HashAlgorithm_HashFunctionGeneratorTests
        : HashAlgorithmTypeBase_HashAlgorithm_HashFunctionGeneratorTests<RIPEMD160Managed>
    {
        // B7 D7 10 F2 F1 F5 C4 24 1E 38 34 B9 01 6A 76 A4
        // 1F 00 EA C1

        protected override int Single16_ExpectedValue { get { return 0xD7B7; } }

        protected override IEnumerable<int> Multiple16_ExpectedValues
        {
            get
            {
                return new[] { 
                    0xD7B7, 0xF210, 0xF5F1, 0x24C4, 0x381E, 0xB934, 0x6A01, 0xA476,
                    0x001F, 0xC1EA
                };
            }
        }

        protected override IEnumerable<int> Multiple19_ExpectedValues
        {
            get
            {
                return new[] { 
                    0x0D7B7,
                    0x63E42
                };
            }
        }
    }

    #endregion

    #region Type Tests

    public class int_HashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_HashAlgorithm_HashFunctionGeneratorTests<int>
    {
        protected override int ElementA { get { return 0; } }
    }

    public class char_HashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_HashAlgorithm_HashFunctionGeneratorTests<char>
    {
        protected override char ElementA { get { return 'a'; } }
    }

    public class string_HashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_HashAlgorithm_HashFunctionGeneratorTests<string>
    {
        protected override string ElementA { get { return "a"; } }
    }

    public class object_HashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_HashAlgorithm_HashFunctionGeneratorTests<object>
    {
        protected override object ElementA { get { return 0; } }
    }

    public class dynamic_HashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_HashAlgorithm_HashFunctionGeneratorTests<dynamic>
    {
        protected override dynamic ElementA { get { return 0; } }
    }

    public class struct_HashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_HashAlgorithm_HashFunctionGeneratorTests<struct_HashAlgorithm_HashFunctionGeneratorTests.TestStruct>
    {
        protected override TestStruct ElementA { get { return new TestStruct() { Member = 'a' }; } }

        [Serializable]
        public struct TestStruct
        {
            public char Member;
        }
    }

    public class class_HashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_HashAlgorithm_HashFunctionGeneratorTests<class_HashAlgorithm_HashFunctionGeneratorTests.TestClass>
    {
        protected override TestClass ElementA { get { return new TestClass() { Property = 'a' }; } }

        [Serializable]
        public class TestClass
        {
            public char Property { get; set; }
        }
    }

    public class enum_HashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_HashAlgorithm_HashFunctionGeneratorTests<enum_HashAlgorithm_HashFunctionGeneratorTests.TestEnum>
    {
        protected override TestEnum ElementA { get { return TestEnum.ElementA; } }

        public enum TestEnum
        {
            ElementA = 0
        }
    }

    #endregion

    #endregion

}
