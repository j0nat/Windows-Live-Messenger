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

        private NetworkComms.ConnectionEstablishShutdownDelegate closeHandler;

        public override void InitializePacket()
        {
            closeHandler = new NetworkComms.ConnectionEstablishShutdownDelegate((Connection connection) =>
            {
                mainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
                {
                    mainWindow.ConnectionClosedLogOut();
                }));
            });

            Open();
        }

        public void Open()
        {
            NetworkComms.AppendGlobalConnectionCloseHandler(closeHandler);
        }

        public void Close()
        {
            NetworkComms.RemoveGlobalConnectionCloseHandler(closeHandler);
        }
    }
}
