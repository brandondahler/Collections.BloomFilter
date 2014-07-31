using System;
using System.Collections.Generic;
using System.Collections.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Collections.Test.Extensions
{
    public class ByteExtensions_Tests
    {
        [Fact]
        public void ToIntegers_dataBytes_Null_Throws()
        {
            Assert.Equal("dataBytes", 
                Assert.Throws<ArgumentNullException>(() => 
                    ((byte[]) null).ToIntegers(1))
                .ParamName);
        }

        [Fact]
        public void ToIntegers_integerBitLength_Invalid_Throws()
        {
            var testBytes = new byte[1000];

            foreach (var invalidIntegerBitLength in new[] { int.MinValue, short.MinValue, -1, 0, 32, short.MaxValue, int.MaxValue})
            {
                Assert.Equal("integerBitLength", 
                    Assert.Throws<ArgumentOutOfRangeException>(() =>
                        testBytes.ToIntegers(invalidIntegerBitLength))
                    .ParamName);
            }
        }

        [Fact]
        public void ToIntegers_integerBitLength_Valid_DoesNotThrow()
        {
            var testBytes = new byte[1000];

            foreach (var integerBitLength in Enumerable.Range(1, 31))
            {
                Assert.DoesNotThrow(() =>
                    testBytes.ToIntegers(integerBitLength));
            }
        }

        [Fact]
        public void ToIntegers_Works()
        {
            byte[] testBytes = {
                0x38, 0xb7, 0xe0, 0xf8, 0x68, 0xdb, 0xe3, 0xc9, 
                0x63, 0x9b, 0x56, 0xf9, 0x4a, 0xc0, 0x4b, 0x2d, 
                0x28 
            };

            var expectedIntegersDict = new Dictionary<int, IEnumerable<int>>() {
                {  
                    4, 
                    new[] { 
                        0x8, 0x3, 0x7, 0xb, 0x0, 0xe, 0x8, 0xf, 0x8, 0x6, 0xb, 0xd, 0x3, 0xe, 0x9, 0xc, 
                        0x3, 0x6, 0xb, 0x9, 0x6, 0x5, 0x9, 0xf, 0xa, 0x4, 0x0, 0xc, 0xb, 0x4, 0xd, 0x2, 
                        0x8, 0x2 
                    } 
                },

                {  
                    8, 
                    new[] { 
                        0x38, 0xb7, 0xe0, 0xf8, 0x68, 0xdb, 0xe3, 0xc9, 
                        0x63, 0x9b, 0x56, 0xf9, 0x4a, 0xc0, 0x4b, 0x2d, 
                        0x28 
                    } 
                },

                {  
                    16, 
                    new[] { 
                        0xb738, 0xf8e0, 0xdb68, 0xc9e3,
                        0x9b63, 0xf956, 0xc04a, 0x2d4b 
                    } 
                },

                { 
                    31, 
                    new[] { 0x78e0b738, 0x13c7b6d1, 0x655a6d8f, 0x6a5e0257} 
                },
            };


            foreach (var expectedIntegers in expectedIntegersDict)
            {
                Assert.Equal(
                    expectedIntegers.Value,
                    testBytes.ToIntegers(expectedIntegers.Key));
            }
        }
    }
}
