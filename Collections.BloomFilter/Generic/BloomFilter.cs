// Copyright (c) 2014 Collections.BloomFilter Developers. 
// See License.txt in the project root for license information.

namespace System.Collections.Generic
{
    /// <summary>
    /// A Bloom filter is a space-efficient probabilistic data structure that is used to test 
    /// whether an element is a member of a set. 
    /// 
    /// False positive matches are possible, but false negatives are not.
    /// 
    /// See http://wikipedia.org/wiki/Bloom_filter for more information.
    /// </summary>
    /// <typeparam name="ElementT">Data type of elements</typeparam>
    public class BloomFilter<ElementT>
    {
        protected readonly BitArray _bitSet;
        protected readonly IEnumerable<Func<ElementT, int>> _hashFunctions = null;

        /// <summary>
        /// Initializes bloom filter with specific bit count and set of hash functions
        /// </summary>
        /// <param name="bitCount">Size of bit array</param>
        /// <param name="hashFunctions">Hash functions to filter with</param>
        public BloomFilter(int bitCount, IEnumerable<Func<ElementT, int>> hashFunctions)
        {
            _bitSet = new BitArray(bitCount, false);
            _hashFunctions = hashFunctions;
        }

        public virtual void Add(ElementT item)
        {
            foreach (var index in HashElement(item))
                _bitSet.Set(index, true);
        }

        public virtual bool ProbablyContains(ElementT item)
        {
            foreach (var index in HashElement(item))
            {
                if (!_bitSet[index])
                    return false;
            }

            return true;
        }

        protected virtual IEnumerable<int> HashElement(ElementT item)
        {
            foreach (var hashFunction in _hashFunctions)
                yield return (hashFunction(item) % _bitSet.Length);
        }
    }
}
