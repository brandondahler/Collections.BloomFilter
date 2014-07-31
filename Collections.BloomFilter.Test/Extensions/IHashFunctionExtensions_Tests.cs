using Moq;
using MoreLinq;
using System;
using System.Collections.Extensions;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Collections.Test.Extensions
{
    public class IHashFunctionExtensions_Tests
    {
        public class ExtendList_Tests
        {

            [Fact]
            public void IHashFunction_ExtendList_hashFunctions_Null_Throws()
            {
                Assert.Equal("hashFunctions", 
                    Assert.Throws<ArgumentNullException>(() =>
                        ((IHashFunction[]) null).ExtendList(1))
                    .ParamName);
            }

            [Fact]
            public void IHashFunction_ExtendList_hashFunctions_Invalid_Throws()
            {
                var zeroHashMock = new Mock<JenkinsOneAtATime>();

                zeroHashMock
                    .SetupGet(joaat => joaat.HashSize)
                    .Returns(0);


                var zeroHash = zeroHashMock.Object;
                var validHashFunction = new JenkinsOneAtATime();


                Assert.Equal("hashFunctions",
                    Assert.Throws<ArgumentException>(() =>
                        (new[] { zeroHash }).ExtendList(1))
                    .ParamName);


                Assert.Equal("hashFunctions",
                    Assert.Throws<ArgumentException>(() =>
                        (new[] { zeroHash, zeroHash }).ExtendList(1))
                    .ParamName);
            }

            [Fact]
            public void IHashFunction_ExtendList_hashFunctions_Valid_DoesNotThrow()
            {
                var zeroHashMock = new Mock<JenkinsOneAtATime>();

                zeroHashMock
                    .SetupGet(joaat => joaat.HashSize)
                    .Returns(0);


                var zeroHash = zeroHashMock.Object;
                var validHashFunction = new JenkinsOneAtATime();


                Assert.DoesNotThrow(() =>
                    (new[] { validHashFunction }).ExtendList(1));


                Assert.DoesNotThrow(() =>
                    (new[] { validHashFunction, validHashFunction }).ExtendList(1));

                Assert.DoesNotThrow(() =>
                    (new[] { validHashFunction, zeroHash }).ExtendList(1));

                Assert.DoesNotThrow(() =>
                    (new[] { zeroHash, validHashFunction }).ExtendList(1));
            }


            [Fact]
            public void IHashFunction_ExtendList_bitsNeeded_Invalid_Throws()
            {
                var testHashFunctions = new[] { new JenkinsOneAtATime() };

                foreach (var invalidBitLength in new[] { int.MinValue, short.MinValue, -1, 0 })
                {
                    Assert.Equal("bitsNeeded",
                        Assert.Throws<ArgumentOutOfRangeException>(() =>
                            testHashFunctions.ExtendList(invalidBitLength))
                        .ParamName);
                }
            }


            [Fact]
            public void IHashFunction_ExtendList_Works()
            {
                var zeroHashFunctionMock = new Mock<JenkinsOneAtATime>();

                zeroHashFunctionMock
                    .SetupGet(joaat => joaat.HashSize)
                    .Returns(0);


                var zeroHashFunction = zeroHashFunctionMock.Object;
                var validHashFunction = new JenkinsOneAtATime();

                Assert.Equal( 0, zeroHashFunction.HashSize);
                Assert.Equal(32, validHashFunction.HashSize);


                /// Tuples represent <input, expected value 1-32 bits, expected value 33-64 bits>
                var hashFunctionLists = new[] {
                    new Tuple<IHashFunction[], IHashFunction[], IHashFunction[]>(
                        new[] { validHashFunction }, 
                        new[] { validHashFunction }, 
                        new[] { validHashFunction, validHashFunction }),
                    
                    new Tuple<IHashFunction[], IHashFunction[], IHashFunction[]>(
                        new[] { validHashFunction, validHashFunction }, 
                        new[] { validHashFunction }, 
                        new[] { validHashFunction, validHashFunction }),

                    new Tuple<IHashFunction[], IHashFunction[], IHashFunction[]>(
                        new[] { validHashFunction, zeroHashFunction },
                        new[] { validHashFunction },
                        new[] { validHashFunction, zeroHashFunction, validHashFunction }),

                    new Tuple<IHashFunction[], IHashFunction[], IHashFunction[]>(
                        new[] { zeroHashFunction, validHashFunction },
                        new[] { zeroHashFunction, validHashFunction },
                        new[] { zeroHashFunction, validHashFunction, zeroHashFunction, validHashFunction }),
                };


                foreach (var hashFunctionList in hashFunctionLists)
                {
                    foreach (var bitsNeeded in Enumerable.Range(1, 32))
                    {
                        Assert.Equal(
                            hashFunctionList.Item2,
                            hashFunctionList.Item1.ExtendList(bitsNeeded),
                            new IHashFunction_Comparer());
                    }

                    foreach (var bitsNeeded in Enumerable.Range(33, 32))
                    {
                        Assert.Equal(
                            hashFunctionList.Item3,
                            hashFunctionList.Item1.ExtendList(bitsNeeded),
                            new IHashFunction_Comparer());
                    }
                }
            }

            private class IHashFunction_Comparer
                : IEqualityComparer<IHashFunction>
            {
                public bool Equals(IHashFunction x, IHashFunction y)
                {
                    return ReferenceEquals(x, y);
                }

                public int GetHashCode(IHashFunction obj)
                {
                    return obj.GetHashCode();
                }
            }

        }

        public class CalculateHashes_Tests
        {
            [Fact]
            public void IHashFunction_CalculateHashes_hashFunctions_Null_Throws()
            {
                Assert.Equal("hashFunctions",
                    Assert.Throws<ArgumentNullException>(() => 
                        ((IHashFunction[]) null).CalculateHashes(0))
                    .ParamName);
            }


            [Fact]
            public void IHashFunction_CalculateHashes_hashFunctions_Empty_Throws()
            {
                var exception = Assert.Throws<ArgumentException>(() =>
                    (new IHashFunction[] { }).CalculateHashes(0));

                Assert.Equal("hashFunctions", exception.ParamName);
                Assert.Contains("must contain at least", exception.Message);
            }

            [Fact]
            public void IHashFunction_CalculateHashes_hashFunctions_ContainsNull_Throws()
            {
                var r = new Random();
                var jenkinsHash = new JenkinsOneAtATime();
                

                foreach (var listSize in new[] { 1, 2, 32, 128, 256 })
                {
                    var testHashFunctions = new List<IHashFunction>(listSize);

                    for (var x = listSize; x > 0; --x)
                        testHashFunctions.Add(jenkinsHash);

                    testHashFunctions[r.Next(0, listSize)] = null;


                    var exception = Assert.Throws<ArgumentException>(() =>
                        testHashFunctions.CalculateHashes(0));

                    Assert.Equal("hashFunctions", exception.ParamName);
                    Assert.Contains("cannot contain any null", exception.Message);
                }
            }


            [Fact]
            public void IHashFunction_CalculateHashes_Works()
            {
                var jenkinsHash = new JenkinsOneAtATime();
                Assert.Equal(32, jenkinsHash.HashSize);

                foreach (var repeatCount in new[] { 1, 2, 255, 256, ushort.MaxValue, (int) ushort.MaxValue + 1 })
                {
                    var testHashFunctions = new List<IHashFunction>(repeatCount);
                    
                    for (var x = repeatCount; x > 0; --x)
                        testHashFunctions.Add(jenkinsHash);



                    var actualResults = testHashFunctions
                        .CalculateHashes(0)
                        .Batch(4)
                        .ToList();

                    
                    Assert.Equal(repeatCount, actualResults.Count);

                    // Ensure that all resulting hash values were distinct, despite hashing same value
                    Assert.Equal(
                        actualResults.Count, 
                        actualResults.Distinct().Count());
                }
            }

        }
    }
}
