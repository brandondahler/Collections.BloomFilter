﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// <remarks>
    /// Convienence alias for <see cref="Generic.BloomFilter{object}"/>.
    /// </remarks>
    public class BloomFilter : Generic.BloomFilter<object>
    {
        public BloomFilter(int bitCount, Func<object, IEnumerable<int>> hashFunctions)
            : base(bitCount, hashFunctions)
        {

        }

    }
}
