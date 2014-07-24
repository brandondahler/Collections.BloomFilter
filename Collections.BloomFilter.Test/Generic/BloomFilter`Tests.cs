
namespace System.Collections.Test.Generic
{
    using System.Collections.Generic;
    using Xunit;

    public abstract class BloomFilterTests<ElementT>
    {
        protected abstract ElementT ElementA { get; }
        protected abstract ElementT ElementB { get; }
        protected abstract ElementT ElementC { get; }

        protected abstract ElementT ElementFromInt(int seed);

        #region Testing Utilities

        /// <summary>
        /// Allows peeking into the protected data of a BloomFilter`1[T]
        /// </summary>
        protected class PeekableBloomFilter : BloomFilter<ElementT>
        {
            public PeekableBloomFilter(int bitCount, Func<ElementT, IEnumerable<int>> hashFunctions)
                : base(bitCount, hashFunctions)
            { 
            
            }

            public BitArray BitSet { get { return _bitSet; } }

            public Func<ElementT, IEnumerable<int>> HashFunctions { get { return _hashFunctions; } }
        }


        /// <summary>
        /// Creates new PeekableBloomFilter that hashes as:
        ///     ElementA => 101010
        ///     ElementB => 010101
        ///     ElementC => 000111.
        /// </summary>
        private PeekableBloomFilter CreateSimpleBloomFilter()
        {
            return new PeekableBloomFilter(6, item => {                 
                if (item.Equals(ElementA))
                    return new[] { 0, 2, 4 };
                else if (item.Equals(ElementB))
                    return new[] { 1, 3, 5 };
                else
                    return new[] { 3, 4, 5 };
            });
        }

        [Fact]
        private BloomFilter<ElementT> CreateDefaultBloomFilter()
        {
            return BloomFilter<ElementT>.Create(128*1024, 1000);
        }

        #endregion

        #region Consturctors

        [Fact]
        public void Generic_Constructable()
        {
            Assert.DoesNotThrow(() => {
                new BloomFilter<ElementT>(1, item => new[] { 0 });
            });
        }

        #endregion

        #region Simple

        #region Add

        [Fact]
        public void Generic_Simple_Add_Empty()
        {
            var bf = CreateSimpleBloomFilter();
            var expectedBitArray = new BitArray(new[] { false, false, false, false, false, false });

            Assert.Equal(expectedBitArray, bf.BitSet);
        }


        [Fact]
        public void Generic_Simple_Add_ElementA()
        {
            var bf = CreateSimpleBloomFilter();
            var expectedBitArray = new BitArray(new[] { true, false, true, false, true, false });

            bf.Add(ElementA);

            Assert.Equal(expectedBitArray, bf.BitSet);
        }

        [Fact]
        public void Generic_Simple_Add_ElementB()
        {
            var bf = CreateSimpleBloomFilter();
            var expectedBitArray = new BitArray(new[] { false, true, false, true, false, true });

            bf.Add(ElementB);

            Assert.Equal(expectedBitArray, bf.BitSet);
        }

        [Fact]
        public void Generic_Simple_Add_ElementC()
        {
            var bf = CreateSimpleBloomFilter();
            var expectedBitArray = new BitArray(new[] { false, false, false, true, true, true });

            bf.Add(ElementC);

            Assert.Equal(expectedBitArray, bf.BitSet);
        }

        [Fact]
        public void Generic_Simple_Add_ElementAElementB()
        {
            var bf = CreateSimpleBloomFilter();
            var expectedBitArray = new BitArray(new[] { true, true, true, true, true, true });

            bf.Add(ElementA);
            bf.Add(ElementB);

            Assert.Equal(expectedBitArray, bf.BitSet);
        }

        [Fact]
        public void Generic_Simple_Add_ElementAElementC()
        {
            var bf = CreateSimpleBloomFilter();
            var expectedBitArray = new BitArray(new[] { true, false, true, true, true, true });

            bf.Add(ElementA);
            bf.Add(ElementC);

            Assert.Equal(expectedBitArray, bf.BitSet);
        }

        [Fact]
        public void Generic_Simple_Add_ElementBElementC()
        {
            var bf = CreateSimpleBloomFilter();
            var expectedBitArray = new BitArray(new[] { false, true, false, true, true, true });

            bf.Add(ElementB);
            bf.Add(ElementC);

            Assert.Equal(expectedBitArray, bf.BitSet);
        }

        [Fact]
        public void Generic_Simple_Add_ElementAElementBElemntC()
        {
            var bf = CreateSimpleBloomFilter();
            var expectedBitArray = new BitArray(new[] { true, true, true, true, true, true });
            
            bf.Add(ElementA);
            bf.Add(ElementB);
            bf.Add(ElementC);

            Assert.Equal(expectedBitArray, bf.BitSet);
        }

        #endregion

        #region ProbablyContains

        [Fact]
        public void Generic_Simple_ProbablyContains_Empty()
        {
            var bf = CreateSimpleBloomFilter();

            Assert.False(bf.ProbablyContains(ElementA));
            Assert.False(bf.ProbablyContains(ElementB));
            Assert.False(bf.ProbablyContains(ElementC));
        }
        
        [Fact]
        public void Generic_Simple_ProbablyContains_ElementA()
        {
            var bf = CreateSimpleBloomFilter();

            bf.Add(ElementA);

            Assert.True(bf.ProbablyContains(ElementA));
            Assert.False(bf.ProbablyContains(ElementB));
            Assert.False(bf.ProbablyContains(ElementC));
        }
        
        [Fact]
        public void Generic_Simple_ProbablyContains_ElementB()
        {
            var bf = CreateSimpleBloomFilter();

            bf.Add(ElementB);

            Assert.False(bf.ProbablyContains(ElementA));
            Assert.True(bf.ProbablyContains(ElementB));
            Assert.False(bf.ProbablyContains(ElementC));
        }
        
        [Fact]
        public void Generic_Simple_ProbablyContains_ElementC()
        {
            var bf = CreateSimpleBloomFilter();

            bf.Add(ElementC);

            Assert.False(bf.ProbablyContains(ElementA));
            Assert.False(bf.ProbablyContains(ElementB));
            Assert.True(bf.ProbablyContains(ElementC));
        }

        
        [Fact]
        public void Generic_Simple_ProbablyContains_ElementAElementB()
        {
            var bf = CreateSimpleBloomFilter();

            bf.Add(ElementA);
            bf.Add(ElementB);

            Assert.True(bf.ProbablyContains(ElementA));
            Assert.True(bf.ProbablyContains(ElementB));
            Assert.True(bf.ProbablyContains(ElementC)); // False positive, expected
        }

        [Fact]
        public void Generic_Simple_ProbablyContains_ElementAElementC()
        {
            var bf = CreateSimpleBloomFilter();

            bf.Add(ElementA);
            bf.Add(ElementC);

            Assert.True(bf.ProbablyContains(ElementA));
            Assert.False(bf.ProbablyContains(ElementB));
            Assert.True(bf.ProbablyContains(ElementC));
        }

        [Fact]
        public void Generic_Simple_ProbablyContains_ElementBElementC()
        {
            var bf = CreateSimpleBloomFilter();

            bf.Add(ElementB);
            bf.Add(ElementC);

            Assert.False(bf.ProbablyContains(ElementA));
            Assert.True(bf.ProbablyContains(ElementB));
            Assert.True(bf.ProbablyContains(ElementC));
        }

        [Fact]
        public void Generic_Simple_ProbablyContains_ElementAElementBElementC()
        {
            var bf = CreateSimpleBloomFilter();

            bf.Add(ElementA);
            bf.Add(ElementB);
            bf.Add(ElementC);

            Assert.True(bf.ProbablyContains(ElementA));
            Assert.True(bf.ProbablyContains(ElementB));
            Assert.True(bf.ProbablyContains(ElementC));
        }

        #endregion

        #endregion

        #region Function
        
        [Fact]    
        public void Generic_Function_Randomized()
        {
            var r = new Random();
            var bf = CreateDefaultBloomFilter();
            
            var addedValues = new HashSet<ElementT>();

            for (int x = 0; x < 1000; ++x)
            {
                var randElement = ElementFromInt(r.Next());

                bf.Add(randElement);
                addedValues.Add(randElement);
            }

            foreach (var addedValue in addedValues)
                Assert.True(bf.ProbablyContains(addedValue));
        }

        #endregion


    }

    #region Concrete Test Classes

    public class BloomFilterTest_int : BloomFilterTests<int>
    {
        protected override int ElementA { get { return 0; } }
        protected override int ElementB { get { return 1; } }
        protected override int ElementC { get { return 2; } }

        protected override int ElementFromInt(int seed)
        {
            return seed;
        }
    }

    public class BloomFilterTest_char : BloomFilterTests<char>
    {
        protected override char ElementA { get { return 'a'; } }
        protected override char ElementB { get { return 'b'; } }
        protected override char ElementC { get { return 'c'; } }

        protected override char ElementFromInt(int seed)
        {
            return (char) (seed % 256);
        }
    }


    public class BloomFilterTest_string : BloomFilterTests<string>
    {
        protected override string ElementA { get { return "a"; } }
        protected override string ElementB { get { return "b"; } }
        protected override string ElementC { get { return "c"; } }

        protected override string ElementFromInt(int seed)
        {
            return seed.ToString();
        }
    }

    public class BloomFilterTest_object : BloomFilterTests<object>
    {
        protected override object ElementA { get { return 0; } }
        protected override object ElementB { get { return 'b'; } }
        protected override object ElementC { get { return "c"; } }

        protected override object ElementFromInt(int seed)
        {
            return seed;
        }
    }

    public class BloomFilterTest_dynamic : BloomFilterTests<dynamic>
    {
        protected override dynamic ElementA { get { return 0; } }
        protected override dynamic ElementB { get { return 'b'; } }
        protected override dynamic ElementC { get { return "c"; } }

        protected override dynamic ElementFromInt(int seed)
        {
            return seed;
        }
    }

    public class BloomFilterTest_struct : BloomFilterTests<BloomFilterTest_struct.TestStruct>
    {
        protected override TestStruct ElementA { get { return new TestStruct() { Member = 'a' }; } }
        protected override TestStruct ElementB { get { return new TestStruct() { Member = 'b' }; } }
        protected override TestStruct ElementC { get { return new TestStruct() { Member = 'c' }; } }

        protected override TestStruct ElementFromInt(int seed)
        {
            return new TestStruct() { Member = (char)(seed % 256) };
        }

        [Serializable]
        public struct TestStruct
        {
            public char Member;
        }
    }

    public class BloomFilterTest_class : BloomFilterTests<BloomFilterTest_class.TestClass>
    {
        protected override TestClass ElementA { get { return new TestClass() { Property = 'a' }; } }
        protected override TestClass ElementB { get { return new TestClass() { Property = 'b' }; } }
        protected override TestClass ElementC { get { return new TestClass() { Property = 'c' }; } }

        protected override TestClass ElementFromInt(int seed)
        {
            return new TestClass() { Property = (char)(seed % 256) };
        }

        [Serializable]
        public class TestClass
        {
            public char Property { get; set; }

            public override bool Equals(object obj)
            {
                var objTestClass = obj as TestClass;

                return objTestClass != null && 
                    Property == objTestClass.Property;
            }

            public override int GetHashCode()
            {
                return 0x5a9c5731 ^ Property.GetHashCode();
            }
        }
    }

    public class BloomFilterTest_enum : BloomFilterTests<BloomFilterTest_enum.TestEnum>
    {
        protected override TestEnum ElementA { get { return TestEnum.ElementA; } }
        protected override TestEnum ElementB { get { return TestEnum.ElementB; } }
        protected override TestEnum ElementC { get { return TestEnum.ElementC; } }

        protected override TestEnum ElementFromInt(int seed)
        {
            return (TestEnum) (seed % 3);
        }

        public enum TestEnum
        {
            ElementA = 0,
            ElementB,
            ElementC
        }
    }

    #endregion

}
