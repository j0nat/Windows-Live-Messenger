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
    class DeleteAndBlockContact : PacketHandler
    {
        public DeleteAndBlockContact(Server server) : base(server) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(PacketName.sendContactBlock.ToString(), BlockContact);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(PacketName.sendContactDelete.ToString(), DeleteContact);
        }

        protected void DeleteContact(PacketHeader header, Connection connection, string userID)
        {
            ConnectedUser senderUser = server.GetConnectedUser(connection);
            ConnectedUser targetUser = null;

            Connection targetConnection = server.GetConnectionFromUserID(userID);
            if (targetConnection != null)
            {
                targetUser = server.GetConnectedUser(targetConnection);
            }
            else
            {
                targetUser = new ConnectedUser(server.accountManager.GetUser(userID), new ContactList(
                        server.accountManager.GetContacts(userID)));
            }

            senderUser.contactList.RemoveUser(userID);
            targetUser.contactList.RemoveUser(senderUser.user.id);

            server.accountManager.SaveContactData(senderUser.user.id, senderUser.contactList.CreateData());
            server.accountManager.SaveContactData(targetUser.user.id, targetUser.contactList.CreateData());

            server.SendDeleteContact(connection, targetUser.user);

            if (targetConnection != null)
            {
                UserInfo offlineUser = new UserInfo(senderUser.user.id, senderUser.user.name, senderUser.user.comment,
                    (int)UserStatus.Offline, senderUser.user.avatar, senderUser.user.blocked);

                server.SendDeleteContact(targetConnection, senderUser.user);
            }
        }

        protected void BlockContact(PacketHeader header, Connection connection, string userID)
        {
            ConnectedUser senderUser = server.GetConnectedUser(connection);

            if (senderUser.contactList.IsUserBlocked(userID))
            {
                senderUser.contactList.SetUserBlocked(userID, false);
            }
            else
            {
                senderUser.contactList.SetUserBlocked(userID, true);
            }

            server.accountManager.SaveContactData(senderUser.user.id, senderUser.contactList.CreateData());

            server.SendUsersContactList(connection, userID);
            server.SendUpdateToUsersContactList(connection, senderUser.user);
        }
    }
}