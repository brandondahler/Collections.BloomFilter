namespace System.Collections.Test
{
    using System.Collections.Generic;
    using Xunit;

    public class BloomFilterTests
    {
        [Fact]
        public void NonGeneric_Constructable()
        {
            Assert.DoesNotThrow(() => {
                new BloomFilter(1, item =>  null);
            });
        }

    }
}
