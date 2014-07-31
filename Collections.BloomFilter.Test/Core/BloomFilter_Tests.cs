using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Test.Utilities;
using System.Data.HashFunction;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Collections.Test
{
    public class BloomFilter_Generic_Tests
    {
        public abstract class BloomFilter_Generic_Constructor_TestsBase<ElementT>
        {
            [Fact]
            public void BloomFilter_Generic_Constructor_hashFunction_Null_Throws()
            {
                Assert.Equal("hashFunctions",
                    Assert.Throws<ArgumentNullException>(() =>
                        NewBloomFilter((IHashFunction) null))
                    .ParamName);
            }

            [Fact]
            public void BloomFilter_Generic_Constructor_hashFunctions_Null_Throws()
            {
                Assert.Equal("hashFunctions",
                    Assert.Throws<ArgumentNullException>(() =>
                        NewBloomFilter((IEnumerable<IHashFunction>) null))
                    .ParamName);
            }

            [Fact]
            public void BloomFilter_Generic_Constructor_hashFunctions_Empty_Throws()
            {
                var exception = Assert.Throws<ArgumentException>(() =>
                    NewBloomFilter(new IHashFunction[0]));


                Assert.Equal("hashFunctions", exception.ParamName);
                Assert.Contains("must contain at least one", exception.Message);
            }

            [Fact]
            public void BloomFilter_Generic_Constructor_hashFunctions_ContainsNull_Throws()
            {
                var exception = Assert.Throws<ArgumentException>(() =>
                    NewBloomFilter(new[] { new JenkinsOneAtATime(), null }));


                Assert.Equal("hashFunctions", exception.ParamName);
                Assert.Contains("cannot contain any null", exception.Message);
            }

            [Fact]
            public void BloomFilter_Generic_Constructor_hashFunction_ZeroHashSize_Throws()
            {
                var zeroHashMock = new Mock<JenkinsOneAtATime>();

                zeroHashMock
                    .SetupGet(hf => hf.HashSize)
                    .Returns(0);


                var zeroHash = zeroHashMock.Object;


                var exception = Assert.Throws<ArgumentException>(() =>
                    NewBloomFilter(zeroHash));

                Assert.Equal("hashFunctions", exception.ParamName);
                Assert.Contains("whose HashSize is greater than 0", exception.Message);
            }

            [Fact]
            public void BloomFilter_Generic_Constructor_hashFunctions_ZeroTotalHashSize_Throws()
            {
                var zeroHashMock = new Mock<JenkinsOneAtATime>();

                zeroHashMock
                    .SetupGet(hf => hf.HashSize)
                    .Returns(0);


                var zeroHash = zeroHashMock.Object;


                var testHashFunctions = new[] { 
                    new[] { zeroHash },
                    new[] { zeroHash, zeroHash }
                };

                foreach (var testHashFunction in testHashFunctions)
                {
                    var exception = Assert.Throws<ArgumentException>(() =>
                        NewBloomFilter(testHashFunction));

                    Assert.Equal("hashFunctions", exception.ParamName);
                    Assert.Contains("whose HashSize is greater than 0", exception.Message);
                }
            }

            [Fact]
            public void BloomFilter_Generic_Constructor_hashFunction_Valid_DoesNotThrow()
            {
                Assert.DoesNotThrow(() =>
                    NewBloomFilter(new JenkinsOneAtATime()));
            }

            [Fact]
            public void BloomFilter_Generic_Constructor_hashFunctions_Valid_DoesNotThrow()
            {
                var zeroHashMock = new Mock<JenkinsOneAtATime>();

                zeroHashMock
                    .SetupGet(hf => hf.HashSize)
                    .Returns(0);


                var zeroHash = zeroHashMock.Object;


                var testHashFunctions = new[] { 
                    new[] { new JenkinsOneAtATime() },
                    new[] { new JenkinsOneAtATime(), new JenkinsOneAtATime()},
                    new[] { new JenkinsOneAtATime(), zeroHash },
                    new[] { zeroHash, new JenkinsOneAtATime() },
                
                };

                foreach (var testHashFunction in testHashFunctions)
                {
                    Assert.DoesNotThrow(() =>
                        NewBloomFilter(testHashFunction));
                }
            }


            protected abstract BloomFilter<ElementT> NewBloomFilter(IHashFunction hashFunction);

            protected abstract BloomFilter<ElementT> NewBloomFilter(IEnumerable<IHashFunction> hashFunctions);

        }

        
        public class BloomFilter_Generic_Constructor_Tests
            : BloomFilter_Generic_Constructor_TestsBase<int>
        {

            [Fact]
            public void BloomFilter_Generic_Constructor_bitArraySize_Invalid_Throws()
            {
                foreach (var invalidBitArraySize in new[] { int.MinValue, short.MinValue, -1, 0 })
                {
                    Assert.Equal("bitArraySize",
                        Assert.Throws<ArgumentOutOfRangeException>(() =>
                            new BloomFilter<int>(invalidBitArraySize, 1, new[] { new JenkinsOneAtATime() }))
                        .ParamName);
                }
            }

            [Fact]
            public void BloomFilter_Generic_Constructor_bitsPerItem_Invalid_Throws()
            {
                foreach (var invalidBitsPerItem in new[] { int.MinValue, short.MinValue, -1, 0 })
                {
                    Assert.Equal("bitsPerItem",
                        Assert.Throws<ArgumentOutOfRangeException>(() =>
                            new BloomFilter<int>(1, invalidBitsPerItem, new[] { new JenkinsOneAtATime() }))
                        .ParamName);
                }
            }

            protected override BloomFilter<int> NewBloomFilter(IHashFunction hashFunction)
            {
                return new BloomFilter<int>(1, 1, hashFunction);
            }

            protected override BloomFilter<int> NewBloomFilter(IEnumerable<IHashFunction> hashFunctions)
            {
                return new BloomFilter<int>(1, 1, hashFunctions);
            }
        }


        public class BloomFilter_Generic_Create_Tests
        {
            public class BloomFilter_Generic_Create_targetMemoryUsage_Tests
                : BloomFilter_Generic_Constructor_TestsBase<int>
        {
            [Fact]
            public void BloomFilter_Generic_Create_targetMemoryUsage_targetMemoryUsage_Invalid_Throws()
            {
                foreach(var invalidTargetMemoryUsage in new[] { int.MinValue, short.MinValue, -1, 0, (int.MaxValue / 8) + 1})
                {
                    Assert.Equal("targetMemoryUsage", 
                        Assert.Throws<ArgumentOutOfRangeException>(() =>
                            BloomFilter<int>.Create(invalidTargetMemoryUsage, 1, new[] { new JenkinsOneAtATime() }))
                        .ParamName);
                }
            }

            [Fact]
            public void BloomFilter_Generic_Create_targetMemoryUsage_targetMemoryUsage_Valid_DoesNotThrow()
            {
                foreach (var validTargetMemoryUsage in new[] { 1, 2, short.MaxValue, ushort.MaxValue, (int.MaxValue / 8)})
                {
                    Assert.DoesNotThrow(() => {
                        try
                        {
                            BloomFilter<int>.Create(validTargetMemoryUsage, validTargetMemoryUsage * 8, new[] { new JenkinsOneAtATime() });
                        } catch (OutOfMemoryException) {
                            // Ignore out of memory exceptions
                        }
                    });
                }
            }


            [Fact]
            public void BloomFilter_Generic_Create_targetMemoryUsage_expectedItemCount_Invalid_Throws()
            {
                foreach (var invalidExpectedItemCount in new[] { int.MinValue, short.MinValue, -1, 0 })
                {
                    Assert.Equal("expectedItemCount",
                        Assert.Throws<ArgumentOutOfRangeException>(() =>
                            BloomFilter<int>.Create(1, invalidExpectedItemCount, new[] { new JenkinsOneAtATime() }))
                        .ParamName);
                }
            }

            [Fact]
            public void BloomFilter_Generic_Create_targetMemoryUsage_expectedItemCount_Valid_DoesNotThrow()
            {
                foreach (var validExpectedItemCount in new[] { 1, 2, short.MaxValue, ushort.MaxValue, int.MaxValue })
                {
                    Assert.DoesNotThrow(() => 
                        BloomFilter<int>.Create(1, validExpectedItemCount, new[] { new JenkinsOneAtATime() }));
                }
            }


            protected override BloomFilter<int> NewBloomFilter(IHashFunction hashFunction)
            {
                return BloomFilter<int>.Create(1, 1, hashFunction);
            }
            protected override BloomFilter<int> NewBloomFilter(IEnumerable<IHashFunction> hashFunctions)
            {
                return BloomFilter<int>.Create(1, 1, hashFunctions);
            }

        }

            public class BloomFilter_Generic_Create_targetFalsePositiveRate_Tests
                : BloomFilter_Generic_Constructor_TestsBase<int>
        {
            [Fact]
            public void BloomFilter_Generic_Create_targetFalsePositiveRate_targetFalsePositiveRate_Invalid_Throws()
            {
                var invalidTargetFalsePositiveRates = new[] { 
                    double.NegativeInfinity, double.MinValue, -1.0d, -double.Epsilon, 
                    0.0d, 
                    1.0d + double.Epsilon, 2.0d, double.MaxValue, double.PositiveInfinity, 
                    double.NaN 
                };

                foreach (var invalidTargetFalsePositiveRate in invalidTargetFalsePositiveRates)
                {
                    Assert.Equal("targetFalsePositiveRate",
                        Assert.Throws<ArgumentOutOfRangeException>(() =>
                            BloomFilter<int>.Create(invalidTargetFalsePositiveRate, 1, new[] { new JenkinsOneAtATime() }))
                        .ParamName);
                }
            }

            [Fact]
            public void BloomFilter_Generic_Create_targetFalsePositiveRate_targetFalsePositiveRate_Valid_DoesNotThrow()
            {
                var validTargetFalsePositiveRates = new[] { 
                    double.Epsilon, 0.0005d, 0.1d, 
                    0.5d, 
                    0.9d, 0.9995d, 0.99999999999999989d
                };

                foreach (var validTargetFalsePositiveRate in validTargetFalsePositiveRates)
                {
                    Assert.DoesNotThrow(() =>
                        BloomFilter<int>.Create(validTargetFalsePositiveRate, 1, new[] { new JenkinsOneAtATime() }));
                }
            }


            [Fact]
            public void BloomFilter_Generic_Create_targetFalsePositiveRate_expectedItemCount_Invalid_Throws()
            {
                foreach (var invalidExpectedItemCount in new[] { int.MinValue, short.MinValue, -1, 0 })
                {
                    Assert.Equal("expectedItemCount",
                        Assert.Throws<ArgumentOutOfRangeException>(() =>
                            BloomFilter<int>.Create(0.5d, invalidExpectedItemCount, new[] { new JenkinsOneAtATime() }))
                        .ParamName);
                }
            }

            [Fact]
            public void BloomFilter_Generic_Create_targetFalsePositiveRate_expectedItemCount_Valid_DoesNotThrow()
            {
                foreach (var validExpectedItemCount in new[] { 1, 2, short.MaxValue, ushort.MaxValue, int.MaxValue })
                {
                    Assert.DoesNotThrow(() =>
                        BloomFilter<int>.Create(0.5d, validExpectedItemCount, new[] { new JenkinsOneAtATime() }));
                }
            }


            protected override BloomFilter<int> NewBloomFilter(IHashFunction hashFunction)
            {
                return BloomFilter<int>.Create(0.5d, 1, hashFunction);
            }
            protected override BloomFilter<int> NewBloomFilter(IEnumerable<IHashFunction> hashFunctions)
            {
                return BloomFilter<int>.Create(0.5d, 1, hashFunctions);
            }

        }
        }


        public class BloomFilter_Generic_Add_Tests
        {
            [Fact]
            public void BloomFilter_Generic_Add_item_Valid_DoesNotThrow()
            {
                {
                    var bf = new BloomFilter<int>(1, 1, new[] { new JenkinsOneAtATime() });

                    foreach (var validItem in new[] { int.MinValue, short.MinValue, -1, 0, 1, short.MaxValue, int.MaxValue })
                    {
                        Assert.DoesNotThrow(() =>
                            bf.Add(validItem));
                    }
                }


                {
                    var bf = new BloomFilter<IEnumerable>(1, 1, new[] { new JenkinsOneAtATime() });

                    foreach (var validItem in new IEnumerable[] { new ArrayList(), new int[1], new List<int>() })
                    {
                        Assert.DoesNotThrow(() =>
                            bf.Add(validItem));
                    }
                }
            }
        }
        
        public class BloomFilter_Generic_ProbablyContains_Tests
        {
            [Fact]
            public void BloomFilter_Generic_ProbablyContains_item_Valid_DoesNotThrow()
            {
                {
                    var bf = new BloomFilter<int>(1, 1, new[] { new JenkinsOneAtATime() });

                    foreach (var validItem in new[] { int.MinValue, short.MinValue, -1, 0, 1, short.MaxValue, int.MaxValue })
                    {
                        Assert.DoesNotThrow(() =>
                            bf.ProbablyContains(validItem));
                    }
                }


                {
                    var bf = new BloomFilter<IEnumerable>(1, 1, new[] { new JenkinsOneAtATime() });

                    foreach (var validItem in new IEnumerable[] { new ArrayList(), new int[1], new List<int>() })
                    {
                        Assert.DoesNotThrow(() =>
                            bf.ProbablyContains(validItem));
                    }
                }
            }
        }



        public class BloomFilter_Generic_IBloomFilter_Add_Tests
        {
            [Fact]
            public void BloomFilter_Generic_IBloomFilter_Add_item_Invalid_Throws()
            {
                IBloomFilter bf = new BloomFilter<int>(1, 1, new[] { new JenkinsOneAtATime() });

                foreach (object invalidItem in new object[] { "0", 0L, 0UL, 0.0f, 0.0d, new List<int>(), new object(), null })
                {
                    Assert.Equal("item",
                        Assert.Throws<ArgumentException>(() =>
                            bf.Add(invalidItem))
                        .ParamName);
                }
            }

            [Fact]
            public void BloomFilter_Generic_IBloomFilter_Add_item_Valid_DoesNotThrow()
            {
                {
                    IBloomFilter bf = new BloomFilter<int>(1, 1, new[] { new JenkinsOneAtATime() });

                    foreach (var validItem in new[] { int.MinValue, short.MinValue, -1, 0, 1, short.MaxValue, int.MaxValue })
                    {
                        Assert.DoesNotThrow(() =>
                            bf.Add(validItem));
                    }
                }


                {
                    IBloomFilter bf = new BloomFilter<IEnumerable>(1, 1, new[] { new JenkinsOneAtATime() });

                    foreach (var validItem in new IEnumerable[] { new ArrayList(), new int[1], new List<int>() })
                    {
                        Assert.DoesNotThrow(() =>
                            bf.Add(validItem));
                    }
                }
            }
        }

        
        public class BloomFilter_Generic_IBloomFilter_ProbablyContains_Tests
        {
            [Fact]
            public void BloomFilter_Generic_IBloomFilter_ProbablyContains_item_Invalid_Throws()
            {
                IBloomFilter bf = new BloomFilter<int>(1, 1, new[] { new JenkinsOneAtATime() });

                foreach (object invalidItem in new object[] { "0", 0L, 0UL, 0.0f, 0.0d, new List<int>(), new object(), null })
                {
                    Assert.Equal("item",
                        Assert.Throws<ArgumentException>(() =>
                            bf.ProbablyContains(invalidItem))
                        .ParamName);
                }
            }

            [Fact]
            public void BloomFilter_Generic_IBloomFilter_ProbablyContains_item_Valid_DoesNotThrow()
            {
                {
                    IBloomFilter bf = new BloomFilter<int>(1, 1, new[] { new JenkinsOneAtATime() });

                    foreach (var validItem in new[] { int.MinValue, short.MinValue, -1, 0, 1, short.MaxValue, int.MaxValue })
                    {
                        Assert.DoesNotThrow(() =>
                            bf.ProbablyContains(validItem));
                    }
                }


                {
                    IBloomFilter bf = new BloomFilter<IEnumerable>(1, 1, new[] { new JenkinsOneAtATime() });

                    foreach (var validItem in new IEnumerable[] { new ArrayList(), new int[1], new List<int>() })
                    {
                        Assert.DoesNotThrow(() =>
                            bf.ProbablyContains(validItem));
                    }
                }
            }
        }



        [Fact]
        public void BloomFilter_Generic_NoFalseNegatives()
        {
            var r = new Random();
            var hashFunction = new JenkinsOneAtATime();


            var itemSet = new List<byte[]>(8192);

            for (int x = 0; x < itemSet.Capacity; ++x)
            {
                var randomBytes = new byte[128];
                r.NextBytes(randomBytes);

                itemSet.Add(randomBytes);
            }


            foreach (var falsePositiveRate in new[] { 0.10, 0.01, 0.005 })
            {
                foreach (var valueCount in new[] { itemSet.Count / 64, itemSet.Count / 8, itemSet.Count })
                {
                    var bf = BloomFilter<byte[]>.Create(falsePositiveRate, valueCount, hashFunction);
                    var testItemSet = itemSet.Take(valueCount).ToList();

                    foreach (var item in testItemSet)
                        bf.Add(item);

                    Assert.True(testItemSet
                        .Shuffle()
                        .All(item =>
                            bf.ProbablyContains(item)));
                }
            }
        }

        [Fact]
        public void BloomFilter_Generic_YieldsExpectedFalsePositiveRate()
        {
            for (var run = 0; run < 5; ++run)
            {
                var r = new Random();
                var hashFunction = new JenkinsOneAtATime();


                var itemSet = new HashSet<byte[]>();
                var orthogonalItemSet = new HashSet<byte[]>();

                for (int x = 0; x < 8192; ++x)
                {
                    var randomBytes = new byte[128];
                    r.NextBytes(randomBytes);

                    itemSet.Add(randomBytes);
                }

                for (int x = 0; x < 8192; ++x)
                {
                    var randomBytes = new byte[128];

                    do
                    {
                        r.NextBytes(randomBytes);
                    } while (itemSet.Contains(randomBytes) || orthogonalItemSet.Contains(randomBytes));


                    orthogonalItemSet.Add(randomBytes);
                }

                bool retry = false;

                foreach (var falsePositiveRate in new[] { 0.0005, 0.005, 0.01, 0.05, 0.10, 0.20, 0.50 })
                {
                    var bf = BloomFilter<byte[]>.Create(falsePositiveRate, itemSet.Count, hashFunction);

                    foreach (var item in itemSet)
                        bf.Add(item);


                    var falsePositiveCount = 0;
                    foreach (var item in orthogonalItemSet)
                    {
                        if (bf.ProbablyContains(item))
                            ++falsePositiveCount;
                    }

                    var actualFalsePositiveRate = ((double)falsePositiveCount / orthogonalItemSet.Count);

                    // Allow it to actually rate as high as x + (x - x^2), 
                    //   which is approximately 2x for small numbers scaling down exponentially to 1.5x at 0.5.
                    var acceptableFalsePositiveRate = falsePositiveRate + (falsePositiveRate - Math.Pow(falsePositiveRate, 2));

                    if (actualFalsePositiveRate > acceptableFalsePositiveRate)
                    {
                        retry = true;
                        break;
                    }
                }

                if (!retry)
                    return;
            }

            // We tried with new data 5 times, fail.
            Assert.True(false);
        }
    }

    public class BloomFilter_Tests
    {
        public class BloomFilter_Constructor_Tests
            : BloomFilter_Generic_Tests.BloomFilter_Generic_Constructor_TestsBase<object>
        {
            protected override BloomFilter<object> NewBloomFilter(IHashFunction hashFunction)
            {
                return new BloomFilter(1, 1, hashFunction);
            }

            protected override BloomFilter<object> NewBloomFilter(IEnumerable<IHashFunction> hashFunctions)
            {
                return new BloomFilter(1, 1, hashFunctions);
            }
        }
    }
}
