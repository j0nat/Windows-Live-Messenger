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
    class PersonalUserUpdate : PacketHandler
    {
        public PersonalUserUpdate(MainWindow mainWindow) : base(mainWindow) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<UserInfo>(PacketName.sendPersonalUserUpdate.ToString(), IncomingContact);
        }

        protected void IncomingContact(PacketHeader header, Connection connection, UserInfo userInfo)
        {
            UserInfo foundUser = Personal.USER_CONTACTS.FirstOrDefault(x => x.id.Trim() == userInfo.id.Trim());

            if (foundUser == null || !(foundUser.name == userInfo.name & foundUser.status == userInfo.status &
                foundUser.comment == userInfo.comment & foundUser.blocked == userInfo.blocked & foundUser.avatar == userInfo.avatar))
            {
                PostLoginAction(userInfo);
            }
        }

        public override void PostLoginAction(object packet)
        {
            base.PostLoginAction(packet);

            mainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                Personal.USER_INFO = (UserInfo)packet;

                mainWindow.GetMainPage().UpdatePersonalInformation();
            }));
        }
    }
}
