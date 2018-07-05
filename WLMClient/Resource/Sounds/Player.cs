using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Resources;
using System.Media;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace WLMClient.Resource.Sounds
{
    class Player
    {
        public static void PlaySound(string url)
        {
            Uri uri = new Uri(url);
            StreamResourceInfo streamResourceIfno = Application.GetResourceStream(uri);
            SoundPlayer sound = new SoundPlayer(streamResourceIfno.Stream);
            sound.Play();
        }
    }
}
