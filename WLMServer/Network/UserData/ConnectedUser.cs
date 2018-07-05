using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WLMData.Data.Packets;

namespace WLMServer.Network.UserData
{
    class ConnectedUser
    {
        public UserInfo user { get; private set; }
        public ContactList contactList { get; private set; }

        public ConnectedUser(UserInfo user, ContactList contactList)
        {
            this.user = user;
            this.contactList = contactList;
        }
    }
}
