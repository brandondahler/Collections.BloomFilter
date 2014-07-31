using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Test.Utilities
{
    internal static class IListExtensions
    {
        public static IList<ElementT> Shuffle<ElementT>(this IEnumerable<ElementT> original)
        {
            if (original == null)
                throw new ArgumentNullException("original");


            var r = new Random();
            var shuffled = new List<ElementT>(original);

            for (var x = shuffled.Count - 1; x >= 0; --x)
            {
                int k = r.Next(x + 1);

                ElementT value = shuffled[k];
                shuffled[k] = shuffled[x];
                shuffled[x] = value;
            }

            return shuffled;
        }
    }
}
