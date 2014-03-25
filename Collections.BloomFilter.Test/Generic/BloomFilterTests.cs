
namespace System.Collections.Test.Generic
{
    using System.Collections.Generic;
    using Xunit;

    public abstract class BloomFilterTests<T>
    {
        public abstract T ElementInFilter { get; }

        public abstract T ElementNotInFilter { get; }

        protected class PeekableBloomFilter : BloomFilter<T>
        {
            public PeekableBloomFilter(int bitCount, IEnumerable<Func<T, int>> hashFunctions)
                : base(bitCount, hashFunctions)
            { 
            
            }

            public BitArray BitSet { get { return _bitSet; } }

            public IEnumerable<Func<T, int>> HashFunctions { get { return _hashFunctions; } }
        }

        [Fact]
        public void Generic_Constructable()
        {
            Assert.DoesNotThrow(() => {
                new BloomFilter<T>(1, new Func<T, int>[] { item => 0 });
            });
        }

        [Fact]
        public void Generic_SimpleAdd()
        {
            var expectedBitArray = new BitArray(new[] { false, true, false, true, false });
            var bf = new PeekableBloomFilter(5, new Func<T, int>[] { item => 1, item => 3 });
            
            bf.Add(ElementInFilter);

            Assert.Equal(expectedBitArray, bf.BitSet);
        }

        [Fact]
        public void Generic_SimpleProbablyContains()
        {
            var bf = new PeekableBloomFilter(2, new Func<T, int>[] { item => (item.Equals(ElementInFilter) ? 0 : 1) });

            bf.Add(ElementInFilter);

            Assert.True(bf.ProbablyContains(ElementInFilter));
            Assert.False(bf.ProbablyContains(ElementNotInFilter));
        }



    }

    public class BloomFilterTest_int : BloomFilterTests<int>
    {
        public override int ElementInFilter { get { return 5; } }

        public override int ElementNotInFilter { get { return 9; } }
    }

    public class BloomFilterTest_string : BloomFilterTests<string>
    {

        public override string ElementInFilter { get { return "in"; } }

        public override string ElementNotInFilter { get { return "not in"; } }
    }

    public class BloomFilterTest_object : BloomFilterTests<object>
    {
        public override object ElementInFilter { get { return 1; } }

        public override object ElementNotInFilter { get { return "not in"; } }
    }


    public class BloomFilterTest_struct : BloomFilterTests<BloomFilterTest_struct.TestStruct>
    {
        public struct TestStruct
        {
            public string Member;
        }

        public override TestStruct ElementInFilter { get { return new TestStruct() { Member = "in" }; } }

        public override TestStruct ElementNotInFilter { get { return new TestStruct() { Member = "not in" }; } }
    }

    public class BloomFilterTest_class : BloomFilterTests<BloomFilterTest_class.TestClass>
    {
        public class TestClass
        {
            public string Property { get; set; }

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

        public override TestClass ElementInFilter { get { return new TestClass() { Property = "in" }; } }

        public override TestClass ElementNotInFilter { get { return new TestClass() { Property = "not in" }; } }
    }

    public class BloomFilterTest_enum : BloomFilterTests<BloomFilterTest_enum.TestEnum>
    {
        public enum TestEnum
        {
            In = 0,
            NotIn
        }

        public override TestEnum ElementInFilter { get { return TestEnum.In; } }

        public override TestEnum ElementNotInFilter { get { return TestEnum.NotIn; } }
    }

}
