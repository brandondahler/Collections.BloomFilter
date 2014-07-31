using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections
{
    /// <summary>
    /// A Bloom filter is a space-efficient probabilistic data structure that is used to test 
    /// whether an element is a member of a set. 
    /// 
    /// False positive matches are possible, but false negatives are not.
    /// 
    /// See http://wikipedia.org/wiki/Bloom_filter for more information.
    /// </summary>
    public interface IBloomFilter
    {
        /// <summary>
        /// Adds item to bloom filter.  Once an item is added, it will always report as being contained within the bloom filter.
        /// </summary>
        /// <param name="item">Item to add to the bloom filter.</param>
        void Add(object item);

        /// <summary>
        /// Checks if item has probably been added to the bloom filter at some point.
        /// </summary>
        /// <param name="item">Item to validate existence within the bloom filter.</param>
        /// <returns>False if it is known for a fact that the item is not contained within the bloom filter, otherwise true.</returns>
        /// <remarks>
        /// This function can return false positives--erroneously stating that the bloom filter contains the item 
        ///   even though it wasn't actually added to the bloom filter; however, it will not return a false negative--stating 
        ///   that the bloom filter does not contain the item even though it does.
        /// </remarks>
        bool ProbablyContains(object item);
    }

    namespace Generic
    {

        /// <inheritdoc/>
        public interface IBloomFilter<ElementT>
            : IBloomFilter
        {
            /// <inheritdoc/>
            void Add(ElementT item);

            /// <inheritdoc/>
            bool ProbablyContains(ElementT item);
        }
    
    }
}
