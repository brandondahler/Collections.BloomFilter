using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic.BloomFilter.HashFunctionGenerators
{
    public abstract class BaseHashFunctionGenerator<ElementT>
        : IHashFunctionGenerator<ElementT>
    {
        public abstract Func<ElementT, IEnumerable<int>> Generate(int maxValue, int indexCount);

        protected virtual IEnumerable<int> SplitBitArray(BitArray bitArray, int size, int count)
        {
            if (size > 31)
                throw new ArgumentException("Item size too large");

            if (size * count > bitArray.Length)
                throw new ArgumentException("BitArray not large enough to split as requested");

            for (int x = 0; x < count; ++x)
            {
                int item = 0;
                for (int y = size - 1; y >= 0; --y)
                {
                    item |= (bitArray[(x * size) + y] ? 1 : 0);
                    item <<= 1;
                }

                yield return item;
            }
        }

    }
}
