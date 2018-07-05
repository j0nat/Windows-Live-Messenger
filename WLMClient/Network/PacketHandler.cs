using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WLMClient.UI.Windows;

namespace WLMClient.Network
{
    class PacketHandler
    {
        public MainWindow mainWindow { get; private set; }

        public PacketHandler(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            InitializePacket();
        }

        public virtual void InitializePacket() { }

        public virtual void PostLoginAction(object packet)
        {
            if (mainWindow.IsPageMainNull())
            {
                return;
            }
        }
    }
}
