using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using WLMServer.Network;

using WLMData.Enums;
using WLMData.Data.Packets;
using WLMServer.Network.UserData;

namespace WLMServer.Network.PacketHandling
{
    class PersonalUserUpdate : PacketHandler
    {
        public PersonalUserUpdate(Server server) : base(server) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<UserInfo>(PacketName.sendUserUpdate.ToString(), UpdateUser);
        }

        protected void UpdateUser(PacketHeader header, Connection connection, UserInfo userInfo)
        {
            ConnectedUser senderUser = server.GetConnectedUser(connection);

            senderUser.user.avatar = userInfo.avatar;
            senderUser.user.comment = userInfo.comment;
            senderUser.user.status = userInfo.status;
            senderUser.user.name = userInfo.name;

            server.accountManager.UpdateAccount(userInfo.id, userInfo.name, userInfo.comment, userInfo.avatar);

            if (!Config.Properties.AVATAR_ENABLE)
            {
                senderUser.user.avatar = "";
            }
            else
            {
                Uri baseUri = new Uri(Config.Properties.AVATAR_IMAGE_URL);
                Uri address = new Uri(baseUri, userInfo.avatar);

                senderUser.user.avatar = address.ToString();
            }

            server.SendPersonalUserUpdate(connection, senderUser.user);
            server.SendUpdateToUsersContactList(connection, senderUser.user);
        }
    }
}
