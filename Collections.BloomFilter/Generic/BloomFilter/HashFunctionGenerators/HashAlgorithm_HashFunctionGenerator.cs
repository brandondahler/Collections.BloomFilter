using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic.BloomFilter.HashFunctionGenerators
{
    public class HashAlgorithm_HashFunctionGenerator<ElementT, HashAlgorithmT> : IHashFunctionGenerator<ElementT>
        where HashAlgorithmT : HashAlgorithm, new()
    {
        public Encoding StringEncoding { get; set; }

        public HashAlgorithm_HashFunctionGenerator()
        {
            StringEncoding = Encoding.UTF8;
        }

        public virtual Func<ElementT, IEnumerable<int>> Generate(int maxValue, int indexCount)
        {
            var indexBits = (int) Math.Ceiling(Math.Log(maxValue, 2));
            var totalBitsNeeded = indexBits * indexCount;

            using (var ha = new HashAlgorithmT())
            {
                if (totalBitsNeeded > ha.HashSize)
                    throw new ArgumentException("Too many hash bits needed.");
            }

            return (item) => {
                using (var ha = new HashAlgorithmT())
                {
                    var hashBytes = ha.ComputeHash(ItemToBytes(item));

                    var hashBitArray = new BitArray(hashBytes);
                    return SplitBitArray(hashBitArray, indexBits, indexCount);
                }
            };
        }

        protected virtual byte[] ItemToBytes(ElementT item)
        {
            using (var ms = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(ms, item);
                return ms.ToArray();
            }
        }

        protected virtual IEnumerable<int> SplitBitArray(BitArray bitArray, int size, int count)
        {
            if (size > 31)
                throw new ArgumentException("Item size too large");

            if (size * count > bitArray.Length)
                throw new ArgumentException("BitArray not large enough to split as requested");

            for (int x = 0; x < count; ++x)
            {
                int item = (bitArray[((x + 1) * size) - 1] ? 1 : 0);
                for (int y = size - 2; y >= 0 ; --y)
                {
                    item <<= 1;
                    item |= (bitArray[(x * size) + y] ? 1 : 0);
                }

                yield return item;
            }
        }

    }
}
