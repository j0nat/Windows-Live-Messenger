using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WLMClient.Resource.Images;

namespace WLMClient.Layout
{
    class Images
    {
        public static BitmapImage BITMAP_AVATAR_FRAME = new BitmapImage();
        public static BitmapImage BITMAP_CHAT_WINDOW_BUTTONS = new BitmapImage();
        public static BitmapImage BITMAP_EMOTICONS = new BitmapImage();
        public static BitmapImage BITMAP_WINDOW_SMALL_ICONS = new BitmapImage();
        public static BitmapImage BITMAP_CHAT_PARAGRAPH_RECTANGLE = new BitmapImage();

        public static void Load()
        {
            BITMAP_AVATAR_FRAME.BeginInit();
            BITMAP_AVATAR_FRAME.UriSource = new Uri(Identifiers.AVATAR_FRAMES_PATH, UriKind.Absolute);
            BITMAP_AVATAR_FRAME.CacheOption = BitmapCacheOption.OnLoad;
            BITMAP_AVATAR_FRAME.EndInit();

            BITMAP_CHAT_WINDOW_BUTTONS.BeginInit();
            BITMAP_CHAT_WINDOW_BUTTONS.UriSource = new Uri(Identifiers.CHAT_WINDOW_BUTTONS, UriKind.Absolute);
            BITMAP_CHAT_WINDOW_BUTTONS.CacheOption = BitmapCacheOption.OnLoad;
            BITMAP_CHAT_WINDOW_BUTTONS.EndInit();

            BITMAP_EMOTICONS.BeginInit();
            BITMAP_EMOTICONS.UriSource = new Uri(Identifiers.EMOTICONS, UriKind.Absolute);
            BITMAP_EMOTICONS.CacheOption = BitmapCacheOption.OnLoad;
            BITMAP_EMOTICONS.EndInit();

            BITMAP_WINDOW_SMALL_ICONS.BeginInit();
            BITMAP_WINDOW_SMALL_ICONS.UriSource = new Uri(Identifiers.MAIN_WINDOW_SMALL_ICONS, UriKind.Absolute);
            BITMAP_WINDOW_SMALL_ICONS.CacheOption = BitmapCacheOption.OnLoad;
            BITMAP_WINDOW_SMALL_ICONS.EndInit();

            BITMAP_CHAT_PARAGRAPH_RECTANGLE.BeginInit();
            BITMAP_CHAT_PARAGRAPH_RECTANGLE.UriSource = new Uri(Identifiers.CHAT_PARAGRAPH_RECTANGLE, UriKind.Absolute);
            BITMAP_CHAT_PARAGRAPH_RECTANGLE.CacheOption = BitmapCacheOption.OnLoad;
            BITMAP_CHAT_PARAGRAPH_RECTANGLE.EndInit();

            Emoticons.INDEX_IN_IMAGE[":)"] = 1;
            Emoticons.INDEX_IN_IMAGE[":d"] = 2;
            Emoticons.INDEX_IN_IMAGE[";)"] = 3;
            Emoticons.INDEX_IN_IMAGE[":o"] = 4;
            Emoticons.INDEX_IN_IMAGE[":p"] = 5;
            Emoticons.INDEX_IN_IMAGE["(h)"] = 6;
            Emoticons.INDEX_IN_IMAGE[":@"] = 7;
            Emoticons.INDEX_IN_IMAGE[":§"] = 8;
            Emoticons.INDEX_IN_IMAGE[":s"] = 9;
            Emoticons.INDEX_IN_IMAGE[":("] = 10;
            Emoticons.INDEX_IN_IMAGE[":'("] = 11;
            Emoticons.INDEX_IN_IMAGE[":|"] = 12;
            Emoticons.INDEX_IN_IMAGE["(6)"] = 13;
            Emoticons.INDEX_IN_IMAGE["(a)"] = 14;
            Emoticons.INDEX_IN_IMAGE["(l)"] = 15;
            Emoticons.INDEX_IN_IMAGE["(u)"] = 16;

            Emoticons.INDEX_IN_IMAGE["(s)"] = 20;
            Emoticons.INDEX_IN_IMAGE["(*)"] = 21;
            Emoticons.INDEX_IN_IMAGE["(8)"] = 23;
            Emoticons.INDEX_IN_IMAGE["(f)"] = 25;
            Emoticons.INDEX_IN_IMAGE["(w)"] = 26;
            Emoticons.INDEX_IN_IMAGE["(o)"] = 27;
            Emoticons.INDEX_IN_IMAGE["(k)"] = 28;
            Emoticons.INDEX_IN_IMAGE["(g)"] = 29;
            Emoticons.INDEX_IN_IMAGE["(^)"] = 30;
            Emoticons.INDEX_IN_IMAGE["(i)"] = 32;
            Emoticons.INDEX_IN_IMAGE["(c)"] = 33;
            Emoticons.INDEX_IN_IMAGE["(b)"] = 37;
            Emoticons.INDEX_IN_IMAGE["(d)"] = 38;

            Emoticons.INDEX_IN_IMAGE["(y)"] = 41;
            Emoticons.INDEX_IN_IMAGE["(n)"] = 42;
            Emoticons.INDEX_IN_IMAGE[":["] = 43;
            Emoticons.INDEX_IN_IMAGE[":-#"] = 48;
            Emoticons.INDEX_IN_IMAGE["8o|"] = 49;
            Emoticons.INDEX_IN_IMAGE["8-|"] = 50;
            Emoticons.INDEX_IN_IMAGE["^o)"] = 51;
            Emoticons.INDEX_IN_IMAGE["+o("] = 53;
            Emoticons.INDEX_IN_IMAGE["(sn)"] = 54;
            Emoticons.INDEX_IN_IMAGE["(pi)"] = 58;
            Emoticons.INDEX_IN_IMAGE["(bah)"] = 71;
            Emoticons.INDEX_IN_IMAGE["<:o)"] = 75;
            Emoticons.INDEX_IN_IMAGE["(ci)"] = 77;
        }
    }
}
