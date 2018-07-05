using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using WLMServer.Network;

namespace WLMServer
{
    class Program
    {
        private static Server server;

        static void Main(string[] args)
        {
            Console.Title = "Messenger Server";

            Console.WriteLine("Type /help to see commands.");

            ImportMessengerConfig();

            server = new Server();

            while (true)
            {
                string input = Console.ReadLine();

                if (input.ToLower() == "/help")
                {
                    Console.Clear();
                    Console.WriteLine("----COMMANDS----");
                    Console.WriteLine("1. /help           - Shows commands");
                    Console.WriteLine("2. /online         - Get online count");
                    Console.WriteLine("3. /listonline     - Shows list of online users");
                    Console.WriteLine("4. /create         - Register new account");
                    Console.WriteLine("---------------");
                }

                if (input.ToLower() == "/online")
                {
                    WriteToConsole("There are " + server.GetUserCount() + " users currently online.");
                }

                if (input.ToLower() == "/listonline")
                {
                    server.ConsoleEchoOnlineUsers();
                }

                if (input.ToLower() == "/create")
                {
                    Console.WriteLine("Type a username");
                    string inputUsername = Console.ReadLine().Trim();
                    Console.WriteLine("Type a password");
                    string inputPassword = Console.ReadLine().Trim();
                    WriteToConsole(server.ConsoleRegisterNewUser(inputUsername, inputPassword));
                }
            }
        }

        public static void WriteToConsole(string input)
        {
            Console.WriteLine(DateTime.Now.ToString() + ": " + input);
        }

        public static void ImportMessengerConfig()
        {
            Config.Properties.DATABASE_HOST = ConfigurationManager.AppSettings["database_host"];
            Config.Properties.DATABASE_ID = ConfigurationManager.AppSettings["database_id"];
            Config.Properties.DATABASE_PASSWORD = ConfigurationManager.AppSettings["database_password"];
            Config.Properties.SERVER_PORT = Convert.ToInt32(ConfigurationManager.AppSettings["server_port"]);
            Config.Properties.SERVER_ENCRYPTION_KEY = ConfigurationManager.AppSettings["server_encryption_key"];
            Config.Properties.DATABASE_PASSWORD_ENCRYPTION_KEY = ConfigurationManager.AppSettings["database_password_encryption_key"];
            Config.Properties.DATABASE_PASSWORD_ENCRYPTION_IV = ConfigurationManager.AppSettings["database_password_encryption_iv"];
            Config.Properties.BROADCAST_INTERVAL = Convert.ToInt32(ConfigurationManager.AppSettings["broadcast_interval"]);
            Config.Properties.AVATAR_ENABLE = Convert.ToBoolean(ConfigurationManager.AppSettings["avatars_enabled"]);
            Config.Properties.AVATAR_IMAGE_URL = ConfigurationManager.AppSettings["avatars_address"];
            Config.Properties.AVATAR_IMAGE_UPLOAD_URL = ConfigurationManager.AppSettings["avatars_address_upload"];
        }
    }
}
