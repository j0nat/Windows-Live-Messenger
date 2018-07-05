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
    class AddNewContactResponse : PacketHandler
    {
        public AddNewContactResponse(Server server) : base(server) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<FriendRequestResponse>(PacketName.sendFriendRequestResponse.ToString(), Response);
        }

        protected void Response(PacketHeader header, Connection connection, FriendRequestResponse response)
        {
            if (server.accountManager.GetUser(response.userID) != null)
            {
                ConnectedUser senderUser = server.GetConnectedUser(connection);
                ContactList senderContactList = senderUser.contactList;

                ConnectedUser requesterUser = null;

                Connection connectionRequesterUser = server.GetConnectionFromUserID(response.userID);
                if (connectionRequesterUser == null)
                {
                    requesterUser = new ConnectedUser(server.accountManager.GetUser(response.userID), new ContactList(
                        server.accountManager.GetContacts(response.userID)));
                }
                else
                {
                    requesterUser = server.GetConnectedUser(connectionRequesterUser);
                }

                if (response.responseCode == (int)FriendRequestResponseCode.accept)
                {
                    senderUser.contactList.AddNewUser(requesterUser.user.id, false, true);
                    requesterUser.contactList.SetUserAccepted(senderUser.user.id, true);

                    server.accountManager.SaveContactData(senderUser.user.id, senderUser.contactList.CreateData());
                }
                else
                {
                    requesterUser.contactList.RemoveUser(senderUser.user.id);
                }

                server.accountManager.SaveContactData(requesterUser.user.id, requesterUser.contactList.CreateData());
                server.accountManager.RemoveFriendRequest(requesterUser.user.id, senderUser.user.id);

                server.SendUsersContactList(connection, senderUser.user.id);

                if (connectionRequesterUser != null)
                {
                    server.SendUsersContactList(connectionRequesterUser, requesterUser.user.id);
                }
            }
        }
    }
}
