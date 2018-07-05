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
    class TransferMessage : PacketHandler
    {
        public TransferMessage(Server server) : base(server) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<Message>(PacketName.sendMessage.ToString(), Transfer);
        }

        protected void Transfer(PacketHeader header, Connection connection, Message message)
        {
            Connection targetUserConnection = server.GetConnectionFromUserID(message.id);
            ConnectedUser senderUser = server.GetConnectedUser(connection);

            if (targetUserConnection != null)
            {
                server.SendPacket(targetUserConnection, PacketName.sendMessage.ToString(), new Message(senderUser.user.id, message.message));
            }
        }
    }
}