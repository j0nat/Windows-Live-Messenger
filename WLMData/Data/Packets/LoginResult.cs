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
    public class LoginResult : Serializer, IExplicitlySerialize
    {
        [ProtoMember(1)]
        public bool loginSuccess { get; private set; }

        [ProtoMember(2)]
        public bool verifiedVersion { get; private set; }

        [ProtoMember(3)]
        public UserInfo userInfo { get; private set; }

        [ProtoMember(4)]
        public string avatarUploadAddress { get; private set; }

        private LoginResult() { }

        public LoginResult(bool loginSuccess, bool verifiedVersion, UserInfo userInfo, string avatarUploadAddress)
        {
            this.loginSuccess = loginSuccess;
            this.verifiedVersion = verifiedVersion;
            this.userInfo = userInfo;
            this.avatarUploadAddress = avatarUploadAddress;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, loginSuccess);
            SerializeData(outputStream, verifiedVersion);
            SerializeData(outputStream, userInfo);
            SerializeData(outputStream, avatarUploadAddress);
        }

        public void Deserialize(Stream inputStream)
        {
            loginSuccess = DeserializeBool(inputStream);
            verifiedVersion = DeserializeBool(inputStream);
            userInfo = DeserializeUserInfo(inputStream);
            avatarUploadAddress = DeserializeString(inputStream);
        }

        public static void Deserialize(Stream inputStream, out LoginResult loginResult)
        {
            loginResult = new LoginResult();
            loginResult.Deserialize(inputStream);
        }
    }
}
