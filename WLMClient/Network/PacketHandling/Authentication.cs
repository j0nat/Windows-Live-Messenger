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
    class Authentication : PacketHandler
    {
        public Authentication(MainWindow mainWindow) : base(mainWindow) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<LoginResult>(PacketName.sendLoginResult.ToString(), Authenticate);
        }

        protected void Authenticate(PacketHeader header, Connection connection, LoginResult loginResult)
        {
            if (loginResult.loginSuccess && loginResult.verifiedVersion)
            {
                Personal.USER_INFO = loginResult.userInfo;
                Config.Properties.AVATAR_IMAGE_UPLOAD_URL = loginResult.avatarUploadAddress;

                mainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
                {
                    mainWindow.LoginSuccess();
                }));
            }
            else
            {
                if (!loginResult.loginSuccess)
                {
                    mainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
                    {
                        System.Windows.MessageBox.Show("We can't sign you into Windows Live Messenger", "Wrong username / password", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }));
                }

                if (!loginResult.verifiedVersion)
                {
                    mainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
                    {
                        System.Windows.MessageBox.Show("This version of Windows Live Messenger is outdated.", "Outdated software", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }));
                }
            }
        }
    }
}
