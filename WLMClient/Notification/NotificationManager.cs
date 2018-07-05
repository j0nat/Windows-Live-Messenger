using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WLMClient.UI.Windows;
using WLMClient.Resource.Sounds;

namespace WLMClient.Notification
{
    class NotificationManager
    {
        public static void Showpopup(string line1, string line2, ChatWindow chatWindow)
        {
            Popup popup = new Popup(line1, line2, chatWindow);
            popup.Show();
        }
    }
}
