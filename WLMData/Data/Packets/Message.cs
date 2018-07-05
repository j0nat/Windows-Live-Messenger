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
    public class Message : Serializer, IExplicitlySerialize
    {
        [ProtoMember(1)]
        public string id { get; private set; }

        [ProtoMember(2)]
        public string message { get; private set; }

        private Message() { }

        public Message(string id, string message)
        {
            this.id = id;
            this.message = message;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, id);
            SerializeData(outputStream, message);
        }

        public void Deserialize(Stream inputStream)
        {
            id = DeserializeString(inputStream);
            message = DeserializeString(inputStream);
        }

        public static void Deserialize(Stream inputStream, out Message message)
        {
            message = new Message();
            message.Deserialize(inputStream);
        }
    }
}