using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using WLMData;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Tools;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using WLMData.Data.Packets;
using WLMData.Enums;
using WLMClient.Network.PacketHandling;
using WLMClient.UI.Windows;
using WLMClient.Locale;

namespace WLMClient.Network
{
    public class Client
    {
        public static string version = "1.0.0";
        public static ConnectionInfo connectionInfo { get; set; }

        private static PacketHandler connectionedClosed, authentication, receiveContact, receiveMessage, receiveNudge,
            receiveContactDelete, receiveWritingStatus, receiveFriendRequest, personalUserUpdate;

        public static void Load(MainWindow mainWindow)
        {
            authentication = new Authentication(mainWindow);
            receiveContact = new ReceiveContact(mainWindow);
            receiveMessage = new ReceiveMessage(mainWindow);
            receiveNudge = new ReceiveNudge(mainWindow);
            receiveFriendRequest = new ReceiveFriendRequest(mainWindow);
            personalUserUpdate = new PersonalUserUpdate(mainWindow);
            receiveContactDelete = new ReceiveContactDelete(mainWindow);
            connectionedClosed = new ConnectionClosed(mainWindow);
            receiveWritingStatus = new ReceiveWritingStatus(mainWindow);

            Personal.USER_CONTACTS = new List<UserInfo>();
            Personal.USER_INFO = null;
            Personal.OPEN_CHAT_WINDOWS = new List<ChatWindow>();

            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);

            RijndaelPSKEncrypter.AddPasswordToOptions(NetworkComms.DefaultSendReceiveOptions.Options, Config.Properties.SERVER_ENCRYPTION_KEY);
            NetworkComms.DefaultSendReceiveOptions.DataProcessors.Add(DPSManager.GetDataProcessor<RijndaelPSKEncrypter>());
        }

        public static void Connect()
        {
            ((ConnectionClosed)connectionedClosed).Close();

            NetworkComms.Shutdown();

            ((ConnectionClosed)connectionedClosed).Open();

            try
            {
                connectionInfo = new ConnectionInfo(Config.Properties.SERVER_ADDRESS, Config.Properties.SERVER_PORT);
            }
            catch
            {
                System.Windows.MessageBox.Show("Server configuration is wrong.", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        public static void SendPacket(string packetType, object packetData)
        {
            try
            {
                TCPConnection.GetConnection(connectionInfo).SendObject(packetType, packetData);
            }
            catch
            {
                NetworkComms.Shutdown();
            }
        }

        public static void AuthenticateUser(string userID, string password, int status)
        {
            LoginRequest loginRequest = new LoginRequest(userID, password, status, version);

            SendPacket(PacketName.requestLogin.ToString(), loginRequest);
        }

        public static void AddNewContact(string userID)
        {
            SendPacket(PacketName.requestNewContact.ToString(), userID);
        }

        public static void SendFriendRequestResponse(string userID, FriendRequestResponseCode responseCode)
        {
            SendPacket(PacketName.sendFriendRequestResponse.ToString(), new FriendRequestResponse(userID, (int)responseCode));
        }

        public static void SendUserUpdate()
        {
            SendPacket(PacketName.sendUserUpdate.ToString(), Personal.USER_INFO);
        }

        public static void BlockContact(string userID)
        {
            SendPacket(PacketName.sendContactBlock.ToString(), userID);
        }

        public static void DeleteContact(string userID)
        {
            SendPacket(PacketName.sendContactDelete.ToString(), userID);
        }

        public static void SendMessage(Message message)
        {
            SendPacket(PacketName.sendMessage.ToString(), message);
        }

        public static void SendNudge(string userID)
        {
            SendPacket(PacketName.sendNudge.ToString(), userID);
        }

        public static void SendWritingStatus(string userID, bool isWriting)
        {
            SendPacket(PacketName.sendWritingStatus.ToString(), new WritingStatus(userID, isWriting));
        }
    }
}
