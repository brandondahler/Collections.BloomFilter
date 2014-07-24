using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic.BloomFilter.HashFunctionGenerators
{
    public class KeyedHashAlgorithm_HashFunctionGenerator<ElementT, KeyedHashAlgorithmT> : HashAlgorithm_HashFunctionGenerator<ElementT, KeyedHashAlgorithmT>
        where KeyedHashAlgorithmT : KeyedHashAlgorithm, new()
    {
        public override Func<ElementT, IEnumerable<int>> Generate(int maxValue, int indexCount)
        {
            var indexBits = (int) Math.Ceiling(Math.Log(maxValue, 2));
            var totalBitsNeeded = indexBits * indexCount;

            int hashesToCalculate;

            using (var kha = new KeyedHashAlgorithmT())
                hashesToCalculate = (int) Math.Ceiling(((double) totalBitsNeeded) / kha.HashSize);

            return (item) => {

                byte[] combinedHashBytes;
                using (var kha = new KeyedHashAlgorithmT())
                    combinedHashBytes = new byte[(kha.HashSize / 8) * hashesToCalculate];

                Parallel.ForEach(Enumerable.Range(0, hashesToCalculate), (hashNumber) => {
                    using (var kha = new KeyedHashAlgorithmT())
                    {
                        kha.Key = BitConverter.GetBytes(hashNumber);
                        
                        var hashBytes = kha.ComputeHash(ItemToBytes(item));
                     
                        // Guarenteed to not overlap, therefore should be treadsafe
                        hashBytes.CopyTo(combinedHashBytes, (kha.HashSize / 8) * hashNumber);
                    }
                });

                var hashBitArray = new BitArray(combinedHashBytes);
                return SplitBitArray(hashBitArray, indexBits, indexCount);
            };
        }
    }
}
