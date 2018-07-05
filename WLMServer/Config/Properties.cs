using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLMServer.Config
{
    class Properties
    {
        public static int SERVER_PORT;
        public static string SERVER_ENCRYPTION_KEY;
        public static string DATABASE_PASSWORD_ENCRYPTION_KEY;
        public static string DATABASE_PASSWORD_ENCRYPTION_IV;
        public static string DATABASE_HOST;
        public static string DATABASE_ID;
        public static string DATABASE_PASSWORD;
        public static bool AVATAR_ENABLE;
        public static string AVATAR_IMAGE_URL;
        public static string AVATAR_IMAGE_UPLOAD_URL;
        public static int BROADCAST_INTERVAL;
    }
}
