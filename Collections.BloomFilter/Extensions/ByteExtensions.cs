using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Extensions
{
    internal static class ByteExtensions
    {
        public static IList<int> ToIntegers(this IList<byte> dataBytes, int integerBitLength)
        {
            if (dataBytes == null)
                throw new ArgumentNullException("dataBytes");

            if (integerBitLength < 1 || integerBitLength > 31)
                throw new ArgumentOutOfRangeException("integerBitLength", "integerBitLength must be in the range [1, 31].");



            var values = new List<int>();

            for (int x = 0; x < (dataBytes.Count * 8) / integerBitLength; ++x)
            {
                var bitStart = x * integerBitLength;
                var bitEnd = (x + 1) * integerBitLength;


                int currentValue = 0;
                var currentBit = bitStart;

                while (currentBit < bitEnd)
                {
                    var dataByte = dataBytes[currentBit / 8];
                    var interByteBit = (currentBit % 8);

                    var currentData = dataByte >> interByteBit;


                    currentValue |= currentData << (currentBit - bitStart);
                    currentBit += 8 - interByteBit;
                }

                currentValue &= int.MaxValue >> (31 - integerBitLength);
                values.Add(currentValue);

            }



                    //foreach (var dataByte in dataBytes)
                    //{
                    //        currentValue |= ((int) dataByte) << currentBitCount;
                    //        currentBitCount += 8;

                    //        var remainingBits = integerBitLength - currentBitCount;

                    //        if (remainingBits <= 0)
                    //        {
                    //            currentValue &= int.MaxValue >> (31 - integerBitLength);
                    //            values.Add(currentValue);

                    //            currentValue = ((int) dataByte) >> (remainingBits + 8);
                    //            currentBitCount = -remainingBits;
                    //        }
                    //}

                    //if (currentBitCount > 0)
                    //{
                    //    currentValue &= int.MaxValue >> (31 - integerBitLength);
                    //    values.Add(currentValue);
                    //}

                    return values;
        }
    }
}
