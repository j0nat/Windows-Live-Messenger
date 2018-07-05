using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WLMData.Data.Packets;
using System.IO;
using NetworkCommsDotNet.DPSBase;

namespace WLMData.Data
{
    public class Serializer
    {
        public void SerializeData(Stream outputStream, Object data)
        {
            byte[] strData = Encoding.UTF8.GetBytes(data.ToString());
            byte[] strLengthData = BitConverter.GetBytes(strData.Length);

            outputStream.Write(strLengthData, 0, strLengthData.Length);
            outputStream.Write(strData, 0, strData.Length);
        }

        public string DeserializeString(Stream inputStream)
        {
            byte[] strLengthData = new byte[sizeof(int)]; inputStream.Read(strLengthData, 0, sizeof(int));
            byte[] strData = new byte[BitConverter.ToInt32(strLengthData, 0)]; inputStream.Read(strData, 0, strData.Length);
            return new String(Encoding.UTF8.GetChars(strData));
        }

        public int DeserializeInt(Stream inputStream)
        {
            byte[] intLengthData = new byte[sizeof(int)]; inputStream.Read(intLengthData, 0, sizeof(int));
            byte[] intData = new byte[BitConverter.ToInt32(intLengthData, 0)]; inputStream.Read(intData, 0, intData.Length);
            return Convert.ToInt32(intData);
        }

        public bool DeserializeBool(Stream inputStream)
        {
            byte[] boolLengthData = new byte[sizeof(int)]; inputStream.Read(boolLengthData, 0, sizeof(int));
            byte[] boolData = new byte[BitConverter.ToInt32(boolLengthData, 0)]; inputStream.Read(boolData, 0, boolData.Length);
            return Convert.ToBoolean(boolData);
        }

        public UserInfo DeserializeUserInfo(Stream inputStream)
        {
            byte[] userInfoLengthData = new byte[sizeof(int)]; inputStream.Read(userInfoLengthData, 0, sizeof(int));
            byte[] userInfoData = new byte[BitConverter.ToInt32(userInfoLengthData, 0)]; inputStream.Read(userInfoData, 0, userInfoData.Length);

            return Deserialize.FromBytes<UserInfo>(userInfoData);
        }
    }
}
