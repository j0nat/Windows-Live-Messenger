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
    class AddNewContact : PacketHandler
    {
        public AddNewContact(Server server) : base(server) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(PacketName.requestNewContact.ToString(), AddContact);
        }

        protected void AddContact(PacketHeader header, Connection connection, string userID)
        {
            if (server.accountManager.GetUser(userID) != null)
            {
                ConnectedUser senderUser = server.GetConnectedUser(connection);
                ContactList senderContactList = senderUser.contactList;

                if (!senderContactList.IsUserInContactList(userID))
                {
                    senderContactList.AddNewUser(userID, false, false);

                    server.accountManager.SaveContactData(senderUser.user.id, senderContactList.CreateData());
                    server.accountManager.AddNewFriendRequest(senderUser.user.id, userID);

                    Connection UserConnection = server.GetConnectionFromUserID(userID);
                    if (UserConnection != null)
                    {
                        server.SendFriendRequestToUser(UserConnection, senderUser.user);
                    }

                    server.SendUsersContactList(connection, senderUser.user.id);
                }
            }
        }
    }
}
