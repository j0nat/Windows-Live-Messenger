using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WLMData.Data
{
    public static class Deserialize
    {
        public static T FromBytes<T>(this byte[] inputSource)
        {
            using (MemoryStream memoryStream = new MemoryStream(inputSource))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                memoryStream.Seek(0, SeekOrigin.Begin);

                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}
