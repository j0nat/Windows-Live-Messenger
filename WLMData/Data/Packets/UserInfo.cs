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
    public class UserInfo : Serializer, IExplicitlySerialize
    {
        [ProtoMember(1)]
        public string id { get; private set; }

        [ProtoMember(2)]
        public string name { get; set; }

        [ProtoMember(3)]
        public string comment { get; set; }

        [ProtoMember(4)]
        public int status { get; set; }

        [ProtoMember(5)]
        public string avatar { get; set; }

        [ProtoMember(6)]
        public bool blocked { get; set; }

        private UserInfo() { }

        public UserInfo(string id, string name, string comment, int status, string avatar, bool blocked)
        {
            this.id = id;
            this.name = name;
            this.comment = comment;
            this.status = status;
            this.avatar = avatar;
            this.blocked = blocked;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, id);
            SerializeData(outputStream, name);
            SerializeData(outputStream, comment);
            SerializeData(outputStream, status);
            SerializeData(outputStream, avatar);
            SerializeData(outputStream, blocked);
        }

        public void Deserialize(Stream inputStream)
        {
            id = DeserializeString(inputStream);
            name = DeserializeString(inputStream);
            comment = DeserializeString(inputStream);
            status = DeserializeInt(inputStream);
            avatar = DeserializeString(inputStream);
            blocked = DeserializeBool(inputStream);
        }

        public static void Deserialize(System.IO.Stream inputStream, out UserInfo result)
        {
            result = new UserInfo();
            result.Deserialize(inputStream);
        }
    }
}