using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using MySql.Data.MySqlClient;

using WLMData.Data.Packets;

namespace WLMServer.Database
{
    class AccountManager : DBConnection
    {
        public void UpdateAccount(string id, string name, string comment, string avatar)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", id);
            cmd.Parameters.AddWithValue("@1", name);
            cmd.Parameters.AddWithValue("@2", comment);
            cmd.Parameters.AddWithValue("@3", avatar);

            Write("UPDATE account SET name=@1, comment=@2, avatar=@3 WHERE id=@0", cmd);
        }

        public void InsertNewAccount(string username, string password)
        {
            password = PasswordEncrypter.GetEncryptedPassword(password);

            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", username);
            cmd.Parameters.AddWithValue("@1", password);

            Write("INSERT INTO account VALUES(@0, 'New User', @1, '', '', '')", cmd);
        }

        public bool IsUserInDatabase(string userID)
        {
            bool returnValue = false;

            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", userID);

            MySqlDataReader reader = Read("SELECT id FROM account WHERE id=@0", cmd);
            while (reader.Read())
            {
                if (reader.GetString("id") == userID)
                {
                    returnValue = true;
                    break;
                }
            }

            reader.Close();

            return returnValue;
        }

        public bool AuthenticateAccount(string username, string password, out UserInfo userInfo)
        {
            bool returnValue = false;
            userInfo = null;

            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", username);
            cmd.Parameters.AddWithValue("@1", password);

            string encryptedPassword = PasswordEncrypter.GetEncryptedPassword(password);

            MySqlDataReader reader = Read("SELECT * FROM account WHERE id=@0", cmd);
            while (reader.Read())
            {
                if (reader.GetString("password").Equals(encryptedPassword))
                {
                    // Only return userInfo if the password is correct.
                    userInfo = new UserInfo(reader.GetString("id"), reader.GetString("name"), reader.GetString("comment"), 1, reader.GetString("avatar"), false);

                    if (!Config.Properties.AVATAR_ENABLE)
                    {
                        userInfo.avatar = "";
                    }
                    else
                    {
                        Uri baseUri = new Uri(Config.Properties.AVATAR_IMAGE_URL);
                        Uri address = new Uri(baseUri, userInfo.avatar);

                        userInfo.avatar = address.ToString();
                    }

                    returnValue = true;
                }

                break;
            }

            reader.Close();
            return returnValue;
        }

        public UserInfo GetUser(string id)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", id);

            MySqlDataReader reader = Read("SELECT * FROM account WHERE id=@0", cmd);

            UserInfo userInfo = null;
            while (reader.Read())
            {
                userInfo = new UserInfo(reader.GetString("id"),
                    reader.GetString("name"), reader.GetString("comment"), 0, reader.GetString("avatar"), false);
                break;
            }

            reader.Close();

            if (!Config.Properties.AVATAR_ENABLE)
            {
                userInfo.avatar = "";
            }
            else
            {
                Uri baseUri = new Uri(Config.Properties.AVATAR_IMAGE_URL);
                Uri address = new Uri(baseUri, userInfo.avatar);

                userInfo.avatar = address.ToString();
            }

            return userInfo;
        }

        public List<string> GetFriendRequests(string requesterID)
        {
            List<string> requests = new List<string>();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", requesterID);

            MySqlDataReader reader = Read("SELECT requesterID FROM friend_requests WHERE targetID=@0", cmd);

            while (reader.Read())
            {
                requests.Add(reader.GetString("requesterID"));

                break;
            }

            reader.Close();

            return requests;
        }

        public string GetContacts(string id)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", id);

            string contacts = "";
            MySqlDataReader reader = Read("SELECT contacts FROM account WHERE id=@0", cmd);
            while (reader.Read())
            {
                contacts = reader.GetString("contacts");

                break;
            }

            reader.Close();

            return contacts;
        }

        public void SaveContactData(string id, string contactData)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", id);
            cmd.Parameters.AddWithValue("@1", contactData);

            Write("UPDATE account SET contacts=@1 WHERE id=@0", cmd);
        }

        public void AddNewFriendRequest(string requesterID, string targetID)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", requesterID);
            cmd.Parameters.AddWithValue("@1", targetID);

            Write("INSERT INTO friend_requests VALUES(@0, @1)", cmd);
        }

        public void RemoveFriendRequest(string requesterID, string targetID)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.AddWithValue("@0", requesterID);
            cmd.Parameters.AddWithValue("@1", targetID);

            Write("DELETE FROM friend_requests WHERE requesterID=@0 AND targetID=@1", cmd);
        }
    }
}
