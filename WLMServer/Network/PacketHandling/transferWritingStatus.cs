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
    class TransferWritingStatus: PacketHandler
    {
        public TransferWritingStatus(Server server) : base(server) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<WritingStatus>(PacketName.sendWritingStatus.ToString(), Transfer);
        }

        protected void Transfer(PacketHeader header, Connection connection, WritingStatus writingStatus)
        {
            Connection targetUserConnection = server.GetConnectionFromUserID(writingStatus.id);
            ConnectedUser senderUser = server.GetConnectedUser(connection);

            if (targetUserConnection != null)
            {
                server.SendPacket(targetUserConnection, PacketName.sendWritingStatus.ToString(), new WritingStatus(senderUser.user.id, writingStatus.isWriting));
            }
        }
    }
}