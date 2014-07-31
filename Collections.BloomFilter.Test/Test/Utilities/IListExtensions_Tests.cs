using System;
using System.Collections.Generic;
using System.Collections.Test.Utilities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Collections.Test.Test.Utilities
{
    public class IListExtensions_Tests
    {
        [Fact]
        public void IList_Shuffle_original_Null_Throws()
        {
            Assert.Equal("original",
                Assert.Throws<ArgumentNullException>(() =>
                    ((IList<int>) null).Shuffle())
                .ParamName);
        }

        [Fact]
        public void IList_Shuffle_Works()
        {
            var r = new Random();


            var original = new List<int>(16384);

            for (var x = 0; x < original.Capacity; ++x)
            {
                var tempBytes = new byte[4];
                r.NextBytes(tempBytes);

                original.Add(BitConverter.ToInt32(tempBytes, 0));
            }



            var shuffled = original.Shuffle();


            Assert.Equal(
                original.Count, 
                shuffled.Count);

            Assert.Equal(
                original
                    .GroupBy(n => n, 
                        (n, nList) => new { 
                            Value = n, 
                            Count = nList.Count() })
                    .OrderBy(n => n.Value),
                shuffled
                    .GroupBy(n => n,
                        (n, nList) => new
                        {
                            Value = n,
                            Count = nList.Count()
                        })
                    .OrderBy(n => n.Value));


            var equalItems = 0;

            for (var x = 0; x < original.Count; ++x)
            {
                if (original[x] == shuffled[x])
                    ++equalItems;
            }

            // Ensure less than 1% of items in the same position are equal.
            Assert.True(((double) equalItems / original.Count) < 0.01d);
        }
    }
}
