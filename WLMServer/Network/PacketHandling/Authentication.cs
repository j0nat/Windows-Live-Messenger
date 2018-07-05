using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using WLMServer.Network;

using WLMData.Enums;
using WLMData.Data.Packets;

namespace WLMServer.Network.PacketHandling
{
    class Authentication : PacketHandler
    {
        public Authentication(Server server) : base(server) { }

        public override void InitializePacket()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<LoginRequest>(PacketName.requestLogin.ToString(), Authenticate);
        }

        protected void Authenticate(PacketHeader header, Connection connection, LoginRequest incomingMessage)
        {
            if (server.GetConnectionFromUserID(incomingMessage.username) == null)
            {
                UserInfo userInfo = null;
                bool authResult = server.accountManager.AuthenticateAccount(incomingMessage.username, incomingMessage.password, out userInfo);

                if (userInfo != null)
                {
                    userInfo.status = incomingMessage.status;
                }

                LoginResult loginAuth;
                if (Config.Properties.AVATAR_ENABLE)
                {
                    loginAuth = new LoginResult(authResult, false, userInfo, Config.Properties.AVATAR_IMAGE_UPLOAD_URL);
                }
                else
                {
                    loginAuth = new LoginResult(authResult, false, userInfo, "");
                }

                if (incomingMessage.version == server.version)
                {
                    loginAuth = new LoginResult(loginAuth.loginSuccess, true, loginAuth.userInfo,
                        loginAuth.avatarUploadAddress);
                }
                else
                {
                    loginAuth = new LoginResult(loginAuth.loginSuccess, false, loginAuth.userInfo,
                        loginAuth.avatarUploadAddress);
                }

                server.SendPacket(connection, PacketName.sendLoginResult.ToString(), loginAuth);

                if (loginAuth.loginSuccess && loginAuth.verifiedVersion)
                {
                    server.AddUserToConnectedUsersList(connection, userInfo);

                    server.SendUsersContactList(connection, userInfo.id);

                    server.SendUpdateToUsersContactList(connection, userInfo);

                    List<string> friendRequestsList = server.accountManager.GetFriendRequests(userInfo.id);
                    foreach (string requesterID in friendRequestsList)
                    {
                        server.SendFriendRequestToUser(connection, server.accountManager.GetUser(requesterID));
                    }
                }
            }
            else
            {
                server.SendPacket(connection, PacketName.sendLoginResult.ToString(), new LoginResult(false, true, new UserInfo("", "", "", 0, "", false), ""));
            }
        }
    }
}
