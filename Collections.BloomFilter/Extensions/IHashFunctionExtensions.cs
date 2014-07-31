using System;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Extensions
{
    internal static class IHashFunctionExtensions
    {
        /// <summary>
        /// Repeats given <see cref="IHashFunction"/>s as few or many times as needed to produce the requested number of bits.
        /// </summary>
        /// <param name="hashFunctions">List of hash functions to use.</param>
        /// <param name="bitsNeeded">Number of bits the extended hash function list needs to produce.</param>
        /// <returns>A list of <see cref="IHashFunction"/>s that, when calculated against, produces at least bitsNeeded total bits.</returns>
        public static IList<IHashFunction> ExtendList(
            this IEnumerable<IHashFunction> hashFunctions, int bitsNeeded)
        {
            if (hashFunctions == null)
                throw new ArgumentNullException("hashFunctions");

            if (!hashFunctions.Any(hf => hf.HashSize > 0))
                throw new ArgumentException("hashFunctions must have at least one item whose HashSize is greater than 0.", "hashFunctions");

            
            if (bitsNeeded <= 0)
                throw new ArgumentOutOfRangeException("bitsNeeded", "bitsNeeded must be greater than 0.");



            var extendedList = new List<IHashFunction>();

            while (bitsNeeded > 0)
            {
                foreach (var hashFunction in hashFunctions)
                {
                    extendedList.Add(hashFunction);

                    bitsNeeded -= hashFunction.HashSize;

                    if (bitsNeeded <= 0)
                        break;
                }
            }

            return extendedList;
        }


        public static IList<byte> CalculateHashes<ElementT>(this IEnumerable<IHashFunction> hashFunctions, ElementT item)
        {
            if (hashFunctions == null)
                throw new ArgumentNullException("hashFunctions");


            if (!hashFunctions.Any())
                throw new ArgumentException("hashFunctions must contain at least one IHashFunction instance.", "hashFunctions");

            if (hashFunctions.Any(hf => hf == null))
                throw new ArgumentException("hashFunctions cannot contain any null IHashFunction instances.", "hashFunctions");



            var results = new List<byte[]>(hashFunctions.Count());
            var itemBytes = item.Serialize();


            int seedBytesSize = 1;

            if (results.Capacity >= byte.MaxValue)
            {
                if (results.Capacity <= ushort.MaxValue)
                    seedBytesSize = 2;
                else
                    seedBytesSize = 4;
            }


            var currentSeedValue = 0;

            foreach (var hashFunction in hashFunctions)
            {
                byte[] seedBytes = null;

                switch (seedBytesSize)
                {
                    case 1:
                        seedBytes = BitConverter.GetBytes((byte) currentSeedValue);
                        break;

                    case 2:
                        seedBytes = BitConverter.GetBytes((ushort)currentSeedValue);
                        break;

                    case 4: 
                        seedBytes = BitConverter.GetBytes(currentSeedValue);
                        break;
                }


                using (var ms = new MemoryStream())
                {
                    ms.Write(seedBytes, 0, seedBytes.Length);
                    ms.Write(itemBytes, 0, itemBytes.Length);

                    ms.Seek(0, SeekOrigin.Begin);

                    results.Add(hashFunction.ComputeHash(ms));
                }


                ++currentSeedValue;
            }
            

            return results
                .SelectMany(r => r)
                .ToList();
        }

    }
}
