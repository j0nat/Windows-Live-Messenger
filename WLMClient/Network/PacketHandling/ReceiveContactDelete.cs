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
    class ReceiveContactDelete : PacketHandler
    {
        public ReceiveContactDelete(MainWindow mainWindow) : base(mainWindow) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<UserInfo>(PacketName.sendContactDelete.ToString(), IncomingDeleteContact);
        }

        protected void IncomingDeleteContact(PacketHeader header, Connection connection, UserInfo userInfo)
        {
            PostLoginAction(userInfo);
        }

        public override void PostLoginAction(object packet)
        {
            base.PostLoginAction(packet);

            mainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                mainWindow.GetMainPage().RemoveContact(((UserInfo)packet).id);
            }));
        }
    }
}
