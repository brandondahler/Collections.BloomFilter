using System.Collections.Extensions;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections
{
    namespace Generic
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
            : IBloomFilter<ElementT>, IBloomFilter
        {
            /// <summary>
            /// Base storage of information for the bloom filter.
            /// </summary>
            protected readonly BitArray _bitArray;

            /// <summary>
            /// List of hash functions that, when ran, will produce the correct number of bits needed to 
            ///   manipulate or validate an item within the bloom filter's bit array
            /// </summary>
            protected readonly IEnumerable<IHashFunction> _extendedHashFunctions;

            /// <summary>Number of bits needed to produce a single index value in the _bitArray.</summary>
            protected readonly int _indexBitLength = 0;


            /// <summary>
            /// Creates a new <see cref="BloomFilter{ElementT}"/> instance with a set bit count, 
            ///   number of bits per item, and set of <see cref="IHashFunction"/> instances.
            /// </summary>
            /// <param name="bitArraySize">Size of filter's bit array, in bits.</param>
            /// <param name="bitsPerItem">Number of bits a single item will set when added to the bloom filter.</param>
            /// <param name="hashFunction">A <see cref="IHashFunction"/> instance to use when hashing item values.</param>
            public BloomFilter(int bitArraySize, int bitsPerItem, IHashFunction hashFunction)
                : this(bitArraySize, bitsPerItem, (hashFunction != null ? new[] { hashFunction } : null))
            {

            }

            /// <summary>
            /// Creates a new <see cref="BloomFilter{ElementT}"/> instance with a set bit count, 
            ///   number of bits per item, and set of <see cref="IHashFunction"/> instances.
            /// </summary>
            /// <param name="bitArraySize">Size of filter's bit array, in bits.  Will be rounded up to the nearest 8 bits.</param>
            /// <param name="bitsPerItem">Number of bits a single item will set when added to the bloom filter.</param>
            /// <param name="hashFunctions">A list of <see cref="IHashFunction"/> instances to use when hashing item values.</param>
            public BloomFilter(int bitArraySize, int bitsPerItem, IEnumerable<IHashFunction> hashFunctions)
            {
                if (bitArraySize <= 0)
                    throw new ArgumentOutOfRangeException("bitArraySize", "bitArraySize must be greater than zero.");

                if (bitsPerItem <= 0)
                    throw new ArgumentOutOfRangeException("bitsPerItem", "bitsPerItem must be greater than zero.");


                if (hashFunctions == null)
                    throw new ArgumentNullException("hashFunctions");

                if (!hashFunctions.Any())
                    throw new ArgumentException("hashFunctions must contain at least one IHashFunction instance.", "hashFunctions");

                if (hashFunctions.Any(hf => hf == null))
                    throw new ArgumentException("hashFunctions cannot contain any null IHashFunction instances.", "hashFunctions");

                if (!hashFunctions.Any(hf => hf.HashSize > 0))
                    throw new ArgumentException("hashFunctions must have at least one item whose HashSize is greater than 0.", "hashFunctions");


                if (bitArraySize % 8 > 0)
                    bitArraySize += 8 - (bitArraySize % 8);

                _bitArray = new BitArray(bitArraySize, false);


                _indexBitLength = 0;

                while (bitArraySize > 0)
                {
                    bitArraySize >>= 1;
                    ++_indexBitLength;
                }


                _extendedHashFunctions = hashFunctions.ExtendList(_indexBitLength * bitsPerItem);
            }

        

            /// <summary>
            /// Creates new <see cref="IBloomFilter{ElementT}"/> instance based on 
            ///   a target memory usage, expected number of items, and a <see cref="IHashFunction"/> instance.
            ///   
            /// These parameters are met by adjusting the number of hashes that must be computed when adding or checking the membership of an item.
            /// </summary>
            /// <param name="targetMemoryUsage">Target memory usage, in bytes, for this bloom filter.</param>
            /// <param name="expectedItemCount">Approximate number of items that will be added to this bloom filter.</param>
            /// <param name="hashFunction">A <see cref="IHashFunction"/> to use when hashing item values.</param>
            /// <returns>An instance of <see cref="IBloomFilter{Element}"/> with the properties requested.</returns>
            public static BloomFilter<ElementT> Create(int targetMemoryUsage, int expectedItemCount, IHashFunction hashFunction)
            {
                return Create(
                    targetMemoryUsage, 
                    expectedItemCount, 
                    (hashFunction != null ? new[] { hashFunction } : null));
            }

            /// <summary>
            /// Creates new <see cref="IBloomFilter{ElementT}"/> instance based on 
            ///   a target memory usage, expected number of items, and a list of <see cref="IHashFunction"/> instances.
            ///   
            /// These parameters are met by adjusting the number of hashes that must be computed when adding or checking the membership of an item.
            /// </summary>
            /// <param name="targetMemoryUsage">Target memory usage, in bytes, for this bloom filter.</param>
            /// <param name="expectedItemCount">Approximate number of items that will be added to this bloom filter.</param>
            /// <param name="hashFunctions">A list of <see cref="IHashFunction"/> instances to use when hashing item values.</param>
            /// <returns>An instance of <see cref="IBloomFilter{Element}"/> with the properties requested.</returns>
            public static BloomFilter<ElementT> Create(int targetMemoryUsage, int expectedItemCount, IEnumerable<IHashFunction> hashFunctions)
            {
                if (targetMemoryUsage <= 0 || targetMemoryUsage > (int.MaxValue / 8))
                    throw new ArgumentOutOfRangeException("targetMemoryUsage", "targetMemoryUsage must be in the range [1, 2^28)");

                if (expectedItemCount <= 0)
                    throw new ArgumentOutOfRangeException("expectedItemCount", "expectedItemCount must be in the range [1, 2^31)");



                var bitsPerItem = ((targetMemoryUsage * 8.0f) / expectedItemCount);
                var indexesToProduce = (int) Math.Ceiling(bitsPerItem * Math.Log(2));

            
                return new BloomFilter<ElementT>(
                        targetMemoryUsage * 8,
                        indexesToProduce,
                        hashFunctions);
            }


            /// <summary>
            /// Creates new <see cref="IBloomFilter{ElementT}"/> instance based on 
            ///   a target failure rate, expected number of items, and a <see cref="IHashFunction"/> instance.
            ///   
            /// These parameters are met by adjusting the memory usage of the bloom filter and 
            ///   the number of hashes that must be computed when adding or checking the membership of an item.
            /// </summary>
            /// <param name="targetFalsePositiveRate">Probability value between 0.0 and 1.0 (exclusive) that is acceptable for the requested bloom filter.</param>
            /// <param name="expectedItemCount">Approximate number of items that will be added to this bloom filter.</param>
            /// <param name="hashFunction">A <see cref="IHashFunction"/> to use when hashing item values.</param>
            /// <returns>An instance of <see cref="IBloomFilter{Element}"/> with the properties requested.</returns>
            public static BloomFilter<ElementT> Create(double targetFalsePositiveRate, int expectedItemCount, IHashFunction hashFunction)
            {
                return Create(
                    targetFalsePositiveRate, 
                    expectedItemCount, 
                    (hashFunction != null ? new[] { hashFunction } : null));
            }

            /// <summary>
            /// Creates new <see cref="IBloomFilter{ElementT}"/> instance based on 
            ///   a target failure rate, expected number of items, and a list of <see cref="IHashFunction"/> instances.
            ///   
            /// These parameters are met by adjusting the memory usage of the bloom filter and 
            ///   the number of hashes that must be computed when adding or checking the membership of an item.
            /// </summary>
            /// <param name="targetFalsePositiveRate">Probability value between 0.0 and 1.0 (exclusive) that is acceptable for the requested bloom filter.</param>
            /// <param name="expectedItemCount">Approximate number of items that will be added to this bloom filter.</param>
            /// <param name="hashFunctions">A list of <see cref="IHashFunction"/> instances to use when hashing item values.</param>
            /// <returns>An instance of <see cref="IBloomFilter{Element}"/> with the properties requested.</returns>
            public static BloomFilter<ElementT> Create(double targetFalsePositiveRate, int expectedItemCount, IEnumerable<IHashFunction> hashFunctions)
            {
                if (targetFalsePositiveRate <= 0.0d || targetFalsePositiveRate >= 1.0d || double.IsNaN(targetFalsePositiveRate))
                    throw new ArgumentOutOfRangeException("targetFalsePositiveRate", "targetFalsePositiveRate must be in the range (0.0, 1.0).");

                if (expectedItemCount <= 0)
                    throw new ArgumentOutOfRangeException("expectedItemCount", "expectedItemCount must be in the range [1, 2^31)");


                var optimalMemoryUsage = (int) Math.Ceiling(
                    -1 * Math.Log(targetFalsePositiveRate) * expectedItemCount 
                    / Math.Pow(Math.Log(2), 2) 
                    / 8);

                if (optimalMemoryUsage > (int.MaxValue / 8))
                    optimalMemoryUsage = int.MaxValue / 8;

                return Create(optimalMemoryUsage, expectedItemCount, hashFunctions);
            }


            /// <inheritdoc/>
            public virtual void Add(ElementT item)
            {
                foreach (var index in CalculateIndexes(item))
                    _bitArray.Set(index % _bitArray.Length, true);
            }

            /// <inheritdoc/>
            public virtual bool ProbablyContains(ElementT item)
            {
                foreach (var index in CalculateIndexes(item))
                {
                    if (!_bitArray[index % _bitArray.Length])
                        return false;
                }

                return true;
            }

            #region IBloomFilter Implementations

            /// <inheritdoc/>
            /// <remarks>Throws an <see cref="ArgumentException"/> if given ElementT is not assignable from given item.</remarks>
            void IBloomFilter.Add(object item)
            {
                if (!(item is ElementT))
                    throw new ArgumentException("Invalid object type specified.", "item");

                Add((ElementT) item);
            }

            /// <inheritdoc/>
            /// <remarks>Throws an <see cref="ArgumentException"/> if given ElementT is not assignable from given item.</remarks>
            bool IBloomFilter.ProbablyContains(object item)
            {
                if (!(item is ElementT))
                    throw new ArgumentException("Invalid object type specified.", "item");

                return ProbablyContains((ElementT) item);
            }

            #endregion

            
            /// <summary>
            /// Hashes the item to produce the proper amount of indexes needed.
            /// </summary>
            /// <param name="item">Item to hash.</param>
            /// <returns>An enumerable set of indexes.</returns>
            protected virtual IEnumerable<int> CalculateIndexes(ElementT item)
            {
                return _extendedHashFunctions
                    .CalculateHashes(item)
                    .ToIntegers(_indexBitLength);
            }
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Convenience alias for <see cref="Generic.BloomFilter{ElementT}"/>.
    /// </remarks>
    public class BloomFilter
        : Generic.BloomFilter<object>
    {
        /// <inheritdoc/>
        public BloomFilter(int filterbits, int indexesToProduce, IHashFunction hashFunction)
            : base(filterbits, indexesToProduce, hashFunction)
        {

        }

        /// <inheritdoc/>
        public BloomFilter(int filterBits, int indexesToProduce, IEnumerable<IHashFunction> hashFunctions)
            : base(filterBits, indexesToProduce, hashFunctions)
        {

        }
    }
}
