using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;

using WLMClient.UI.Windows;
using WLMData.Enums;
using WLMData.Data.Packets;
using WLMClient.Locale;

namespace WLMClient.Network.PacketHandling
{
    class ConnectionClosed : PacketHandler
    {
        public ConnectionClosed(MainWindow mainWindow) : base(mainWindow) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalConnectionCloseHandler(Closed);
        }

        protected void Closed(Connection connection)
        {
            mainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                mainWindow.ConnectionClosedLogOut();
            }));
        }
    }
}
