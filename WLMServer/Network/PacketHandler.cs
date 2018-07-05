using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLMServer.Network
{
    class PacketHandler
    {
        public Server server { get; private set; }

        public PacketHandler(Server server)
        {
            this.server = server;

            InitializePacket();
        }

        public virtual void InitializePacket() { }
    }
}
