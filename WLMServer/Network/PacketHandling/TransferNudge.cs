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
    class TransferNudge : PacketHandler
    {
        public TransferNudge(Server server) : base(server) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(PacketName.sendNudge.ToString(), Transfer);
        }

        protected void Transfer(PacketHeader header, Connection connection, string userID)
        {
            Connection targetUserConnection = server.GetConnectionFromUserID(userID);
            ConnectedUser senderUser = server.GetConnectedUser(connection);

            if (targetUserConnection != null)
            {
                server.SendPacket(targetUserConnection, PacketName.sendNudge.ToString(), senderUser.user.id);
            }
        }
    }
}