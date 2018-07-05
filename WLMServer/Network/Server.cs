using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using NetworkCommsDotNet;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using NetworkCommsDotNet.Connections.TCP;

using WLMData.Enums;
using WLMData.Data;
using WLMData.Data.Packets;
using WLMServer.Network.UserData;
using WLMServer.Database;

using System.Threading;

namespace WLMServer.Network
{
    class Server
    {
        public string version = "1.0.0";
        private Dictionary<Connection, ConnectedUser> users;
        private PacketHandler authentication, addNewContact, addNewContactResponse, personalUserUpdate,
            deleteAndBlockContact, transferMessage, transferNudge, transferWritingStatus;
        public AccountManager accountManager;

        public Server()
        {
            users = new Dictionary<Connection, ConnectedUser>();

            authentication = new PacketHandling.Authentication(this);
            addNewContact = new PacketHandling.AddNewContact(this);
            addNewContactResponse = new PacketHandling.AddNewContactResponse(this);
            personalUserUpdate = new PacketHandling.PersonalUserUpdate(this);
            deleteAndBlockContact = new PacketHandling.DeleteAndBlockContact(this);
            transferMessage = new PacketHandling.TransferMessage(this);
            transferNudge = new PacketHandling.TransferNudge(this);
            transferWritingStatus = new PacketHandling.TransferWritingStatus(this);

            accountManager = new AccountManager();

            NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);

            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(), NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);
            if (!NetworkComms.DefaultSendReceiveOptions.DataProcessors.Contains(DPSManager.GetDataProcessor<RijndaelPSKEncrypter>()))
            {
                RijndaelPSKEncrypter.AddPasswordToOptions(NetworkComms.DefaultSendReceiveOptions.Options, Config.Properties.SERVER_ENCRYPTION_KEY);
                NetworkComms.DefaultSendReceiveOptions.DataProcessors.Add(DPSManager.GetDataProcessor<RijndaelPSKEncrypter>());
            }

            NetworkComms.Shutdown();
            Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, Config.Properties.SERVER_PORT));

            foreach (IPEndPoint listenEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
            {
                Program.WriteToConsole("Local End Point: " + listenEndPoint.Address + ":" + listenEndPoint.Port);
            }

            BroadCastContacts(Config.Properties.BROADCAST_INTERVAL);
        }

        public void BroadCastContacts(int intervalSeconds)
        {
            new Thread(delegate ()
            {
                while (true)
                {
                    Thread.Sleep(intervalSeconds * 1000);

                    lock (users)
                    {
                        foreach (KeyValuePair<Connection, ConnectedUser> user in users)
                        {
                            SendUsersContactList(user.Key, user.Value.user.id);
                        }
                    }
                }
            }).Start();
        }

        public void SendPacket(Connection connection, string packetType, object packetData)
        {
            try
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(packetType, packetData);
            }
            catch
            {
                HandleConnectionClosed(connection);
            }
        }

        private void HandleConnectionClosed(Connection connection)
        {
            lock (users)
            {
                if (users.ContainsKey(connection))
                {
                    UserInfo user = users[connection].user;
                    user.status = 0;
                    SendUpdateToUsersContactList(connection, user);

                    users.Remove(connection);
                }
            }
        }

        public ConnectedUser GetConnectedUser(Connection connection)
        {
            return users[connection];
        }

        public Connection GetConnectionFromUserID(string userID)
        {
            return users.FirstOrDefault(x => x.Value.user.id.Trim().ToLower() == userID.Trim().ToLower()).Key;
        }

        public void SendFriendRequestToUser(Connection connection, UserInfo userInfo)
        {
            SendPacket(connection, PacketName.sendFriendRequestToUser.ToString(), userInfo);
        }

        public void AddUserToConnectedUsersList(Connection connection, UserInfo userInfo)
        {
            users[connection] = new ConnectedUser(userInfo, new ContactList(accountManager.GetContacts(userInfo.id)));
        }

        public void SendPersonalUserUpdate(Connection connection, UserInfo userInfo)
        {
            SendPacket(connection, PacketName.sendPersonalUserUpdate.ToString(), userInfo);
        }

        public void SendDeleteContact(Connection connection, UserInfo userInfo)
        {
            SendPacket(connection, PacketName.sendContactDelete.ToString(), userInfo);
        }

        public void SendUsersContactList(Connection connection, string id)
        {
            ContactList contactList = users[connection].contactList;

            List<Contact> contacts = contactList.GetContacts();

            foreach (Contact contact in contacts)
            {
                Connection sendConnection = users.FirstOrDefault(x => x.Value.user.id.Trim() == contact.id.Trim()).Key;
                ConnectedUser sendUser = null;
                string sendUserName = "";
                int sendUserStatus = 0;
                bool sendUserIsBlocked = false;
                string sendUserComment = "";
                string sendUserAvatar = "";

                if (sendConnection == null)
                {
                    sendUser = new ConnectedUser(accountManager.GetUser(contact.id), new ContactList(contact.id.Trim()));

                    if (sendUser == null)
                    {
                        continue;
                    }
                }
                else
                {
                    sendUser = users[sendConnection];
                }

                sendUserStatus = sendUser.user.status;
                sendUserName = sendUser.user.name;
                sendUserComment = sendUser.user.comment;
                sendUserAvatar = sendUser.user.avatar;

                bool isBlocked = sendUser.contactList.IsUserBlocked(users[connection].user.id);
                bool isAvailable = contactList.IsUserAccepted(contact.id);

                if (!isAvailable)
                {
                    sendUserAvatar = "";
                    sendUserName = sendUser.user.id;
                    sendUserStatus = (int)UserStatus.Offline;
                    sendUserComment = "";
                }

                if (sendUser.user.status != (int)UserStatus.Offline)
                {
                    if (isBlocked)
                    {
                        sendUserStatus = (int)UserStatus.Offline;
                    }
                }

                if (contactList.IsUserBlocked(sendUser.user.id))
                {
                    sendUserIsBlocked = true;
                    sendUserName = "(BLOCKED) " + sendUserName;
                }
                
                SendPacket(connection, PacketName.sendContact.ToString(), new UserInfo(sendUser.user.id, sendUserName, sendUserComment,
                    sendUserStatus, sendUserAvatar, sendUserIsBlocked));
            }
        }

        public void SendUpdateToUsersContactList(Connection connection, UserInfo userInfo)
        {
            ContactList contactList = users[connection].contactList;
            List<Contact> contacts = contactList.GetContacts();

            foreach (Contact contact in contacts)
            {
                Connection contactConnection = users.FirstOrDefault(x => x.Value.user.id.Trim() == contact.id.Trim()).Key;

                if (contactConnection != null & contactConnection != connection)
                {
                    UserInfo sendUser = new UserInfo(userInfo.id, userInfo.name, userInfo.comment,
                        (int)userInfo.status, userInfo.avatar, false);

                    string sendUserName = sendUser.name;
                    bool sendUserIsBlocked = false;
                    int sendUserStatus = (int)sendUser.status;

                    if (!contactList.IsUserAccepted(contact.id))
                    {
                        continue;
                    }

                    if (users[contactConnection].contactList.IsUserBlocked(userInfo.id))
                    {
                        sendUserIsBlocked = true;
                        sendUserName = "(BLOCKED) " + sendUserName;
                    }

                    if (contactList.IsUserBlocked(contact.id))
                    {
                        sendUserStatus = (int)UserStatus.Offline;
                    }

                    SendPacket(contactConnection, PacketName.sendContact.ToString(), new UserInfo(sendUser.id, sendUserName, sendUser.comment,
                        sendUserStatus, sendUser.avatar, sendUserIsBlocked));
                }
            }
        }

        public int GetUserCount()
        {
            return users.Count;
        }

        public void ConsoleEchoOnlineUsers()
        {
            foreach (KeyValuePair<Connection, ConnectedUser> user in users)
            {
                Program.WriteToConsole("User ID " + user.Value.user.id + " logged in from " + ((IPEndPoint)user.Key.ConnectionInfo.RemoteEndPoint).Address);
            }
        }

        public string ConsoleRegisterNewUser(string username, string password)
        {
            string returnValue = "";

            if (username.Length > 0 && password.Length > 0 && !accountManager.IsUserInDatabase(username))
            {
                accountManager.InsertNewAccount(username, password);

                returnValue = "RESULT: Account created successfully.";
            }
            else
            {
                returnValue = "RESULT: Account creation failed.";
            }
            
            return returnValue;
        }
    }
}
