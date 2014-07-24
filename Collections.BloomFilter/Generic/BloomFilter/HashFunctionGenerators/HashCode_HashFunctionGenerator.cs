using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic.BloomFilter.HashFunctionGenerators
{
    public class HashCode_HashFunctionGenerator<ElementT>
        : BaseHashFunctionGenerator<ElementT>
    {
        
        public override Func<ElementT, IEnumerable<int>> Generate(int maxValue, int indexCount)
        {
            var indexBits = 1 << ((int) Math.Ceiling(Math.Log(maxValue, 2)) - 1);
            var totalBitsNeeded = indexBits * indexCount;

            if (totalBitsNeeded > sizeof(int) * 8)
                throw new ArgumentException("Too many hash bits needed.");

            return (item) => {
                var hashBitArray = new BitArray(BitConverter.GetBytes(item.GetHashCode()));

                return SplitBitArray(hashBitArray, indexBits, indexCount);
            };
        }
    }
}
