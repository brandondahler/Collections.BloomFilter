using System;
using System.Collections.Generic;
using System.Collections.Generic.BloomFilter;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Collections.Test.Generic.BloomFilter
{
    public abstract class IHashFunctionGeneratorTests<ElementT, HashAlgorithmGeneratorT>
        where HashAlgorithmGeneratorT : IHashFunctionGenerator<ElementT>, new()
    {
        protected abstract ElementT ElementA { get; }

        protected abstract int MaxBitsGeneratable { get; }


        private const int MaxBitsToTestAgainst = 65536;


        [Fact]
        public void IHashFunctionGenerator_Constructable()
        {
            Assert.DoesNotThrow(() => {
                var hag = new HashAlgorithmGeneratorT();
            });
        }

        [Fact]
        public void IHashFunctionGenerator_Generate_ProducesSomething()
        {
            var hfg = new HashAlgorithmGeneratorT();

            Assert.DoesNotThrow(() => hfg.Generate(16, 1));
            Assert.NotNull(hfg.Generate(16, 1));
        }

        [Fact]
        public virtual void IHashFunctionGenerator_Generate_ProducesCorrectNumberOfResults()
        {
            var hfg = new HashAlgorithmGeneratorT();
            var maxTestValue = Math.Min(MaxBitsGeneratable, MaxBitsToTestAgainst);

            int x = 1;
            while (x <= maxTestValue)
            {
                var indexResults = hfg.Generate(2, x)(ElementA);
                Assert.Equal(x, indexResults.Count());

                x <<= 1;

                if (x > maxTestValue && (x >> 1) != maxTestValue)
                    x = maxTestValue;
            }
        }

        [Fact]
        public void IHashFunctionGenerator_Generate_ThrowsIfBounded()
        {
            var hfg = new HashAlgorithmGeneratorT();

            Assert.DoesNotThrow(() => hfg.Generate(2, Math.Min(MaxBitsGeneratable, MaxBitsToTestAgainst) - 1));
            Assert.DoesNotThrow(() => hfg.Generate(2, Math.Min(MaxBitsGeneratable, MaxBitsToTestAgainst)));

            if (MaxBitsGeneratable <= MaxBitsToTestAgainst)
                Assert.Throws<ArgumentException>(() => hfg.Generate(2, MaxBitsGeneratable + 1));
        }
    }
}
