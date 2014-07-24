// Copyright (c) 2014 Collections.BloomFilter Developers. 
// See License.txt in the project root for license information.

namespace System.Collections.Generic
{
    using System.Collections.Generic.BloomFilter;
    using System.Collections.Generic.BloomFilter.HashFunctionGenerators;
    using System.Security.Cryptography;

    /// <summary>
    /// A Bloom filter is a space-efficient probabilistic data structure that is used to test 
    /// whether an element is a member of a set. 
    /// 
    /// False positive matches are possible, but false negatives are not.
    /// 
    /// See http://wikipedia.org/wiki/Bloom_filter for more information.
    /// </summary>
    /// <typeparam name="ElementT">Data type of elements</typeparam>
    public class BloomFilter<ElementT> : IBloomFilter<ElementT>, IBloomFilter
    {
        public static IHashFunctionGenerator<ElementT> DefaultHashFunctionGenerator = new KeyedHashAlgorithm_HashFunctionGenerator<ElementT, HMACSHA512>();

        protected readonly BitArray _bitSet;
        protected readonly Func<ElementT, IEnumerable<int>> _hashFunctions = null;

        /// <summary>
        /// Initializes bloom filter with specific bit count and set of hash functions.
        /// </summary>
        /// <param name="bitCount">Size of bit array</param>
        /// <param name="hashFunctions">Hash functions to filter with</param>
        /// <remarks>Lowest-level constructor.</remarks>
        public BloomFilter(int bitCount, Func<ElementT, IEnumerable<int>> hashFunctions)
        {
            _bitSet = new BitArray(bitCount, false);
            _hashFunctions = hashFunctions;
        }

        /// <summary>
        /// Initializes bloom filter with specific bit count, hash function count, and HashFunctionGenerator.
        /// </summary>
        /// <param name="bitCount">Size of bit array</param>
        /// <param name="hashFunctionCount">Number of hash functions to be generated</param>
        /// <param name="hashFunctionGenerator">Generator to produce the hash functions to filter with, null defaults to DefaultHashFunctionGenerator</param>
        /// <remarks>Lowest-level constructor.</remarks>
        public BloomFilter(int bitCount, int hashFunctionCount, IHashFunctionGenerator<ElementT> hashFunctionGenerator)
            : this(bitCount, (hashFunctionGenerator ?? DefaultHashFunctionGenerator).Generate(bitCount, hashFunctionCount))
        {
        }


        /// <summary>
        /// Initializes bloom filter to produce minimal false positives given the desired memory usage and expected element count.
        /// </summary>
        /// <param name="desiredMemorySize">Approximate amount of memory that should be taken up, in bytes.</param>
        /// <param name="expectedItemCount">Expected number of items that will be added, used to minimize false positives. </param>
        /// <param name="hashFunctionGenerator">Generator to produce the hash functions to filter with, null defaults to DefaultHashFunctionGenerator</param>
        /// <remarks>
        /// The Bloom filter will continue to work reguardless of the actual number of items added.
        /// 
        /// Significantly over-estimating the expcected item count will (relatively) waste CPU resources.
        /// Significantly under-estimating the expected item count will cause more false positives.
        /// </remarks>
        public static BloomFilter<ElementT> Create(int desiredMemorySize, int expectedItemCount, IHashFunctionGenerator<ElementT> hashFunctionGenerator = null)
        {
            return new BloomFilter<ElementT>(
                desiredMemorySize * 8, 
                (int) Math.Round(((double) desiredMemorySize * 8 / expectedItemCount) * Math.Log(2)),
                hashFunctionGenerator
            );
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

        #region IBloomFilter Implementations

        void IBloomFilter.Add(object item)
        {
            if (!typeof(ElementT).IsAssignableFrom(item.GetType()))
                throw new ArgumentException("Invalid object type specified.");

            Add((ElementT)item);
        }

        bool IBloomFilter.ProbablyContains(object item)
        {
            if (!typeof(ElementT).IsAssignableFrom(item.GetType()))
                throw new ArgumentException("Invalid object type specified.");

            return ProbablyContains((ElementT)item);
        }

        #endregion


        protected virtual IEnumerable<int> HashElement(ElementT item)
        {
            foreach (var index in _hashFunctions(item))
                yield return index % _bitSet.Length;
        }
    }
}
