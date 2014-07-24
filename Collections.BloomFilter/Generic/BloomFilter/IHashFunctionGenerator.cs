using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic.BloomFilter
{
    public interface IHashFunctionGenerator<ElementT>
    {
        Func<ElementT, IEnumerable<int>> Generate(int maxValue, int indexCount);
    }
}
