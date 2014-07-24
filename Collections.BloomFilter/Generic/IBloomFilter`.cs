using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public interface IBloomFilter<ElementT> : IBloomFilter
    {
        void Add(ElementT item);

        bool ProbablyContains(ElementT item);
    }
}
