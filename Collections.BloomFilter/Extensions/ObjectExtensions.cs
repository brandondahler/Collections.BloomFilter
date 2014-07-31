using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Extensions
{
    internal static class ObjectExtensions
    {
        public static byte[] Serialize(this object item)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, item);

                return ms.ToArray();
            }
        }
    }
}
