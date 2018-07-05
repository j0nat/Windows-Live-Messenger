using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtoBuf;
using System.IO;
using NetworkCommsDotNet.DPSBase;

namespace WLMData.Data.Packets
{
    [ProtoContract]
    public class FriendRequestResponse : Serializer, IExplicitlySerialize
    {
        [ProtoMember(1)]
        public string userID { get; private set; }

        [ProtoMember(2)]
        public int responseCode { get; private set; }

        private FriendRequestResponse() { }

        public FriendRequestResponse(string userID, int responseCode)
        {
            this.userID = userID;
            this.responseCode = responseCode;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, userID);
            SerializeData(outputStream, responseCode);
        }

        public void Deserialize(Stream inputStream)
        {
            userID = DeserializeString(inputStream);
            responseCode = DeserializeInt(inputStream);
        }

        public static void Deserialize(Stream inputStream, out FriendRequestResponse response)
        {
            response = new FriendRequestResponse();
            response.Deserialize(inputStream);
        }
    }
}
