using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLMServer.Network.UserData
{
    class ContactList
    {
        private Dictionary<String, Contact> contacts;

        public ContactList(string data)
        {
            contacts = new Dictionary<String, Contact>();

            data = data.Trim();
            data = data.Replace(" ", "");
            data = data.Replace("\n", "");

            if (data.Length == 0)
            {
                return;
            }

            string[] contactArray = data.Split('[');
            for (int i = 1; i < contactArray.Length; i++)
            {
                string[] contactData = contactArray[i].Split(']')[0].Split(',');

                string contactName = contactData[0];
                string blockedData = contactData[1];
                string acceptedData = contactData[2];

                bool isBlocked = false;
                bool isAccepted = false;

                if (blockedData != "0")
                {
                    isBlocked = true;
                }

                if (acceptedData != "0")
                {
                    isAccepted = true;
                }

                Contact contact = new Contact(contactName, isBlocked, isAccepted);
                contacts.Add(contactName, contact);
            }
        }

        public void AddNewUser(string username, bool isBlocked, bool isAccepted)
        {
            lock (contacts)
            {
                contacts.Add(username, new Contact(username, isBlocked, isAccepted));
            }
        }

        public void RemoveUser(string username)
        {
            lock (contacts)
            {
                if (contacts.ContainsKey(username))
                {
                    contacts.Remove(username);
                }
            }
        }

        public bool IsUserBlocked(string username)
        {
            bool isUSerBlocked = false;

            if (contacts.ContainsKey(username))
            {
                isUSerBlocked = contacts[username].isBlocked;
            }

            return isUSerBlocked;
        }

        public bool IsUserAccepted(string username)
        {
            bool isUserAccepted = false;

            if (contacts.ContainsKey(username))
            {
                isUserAccepted = contacts[username].isAccepted;
            }

            return isUserAccepted;
        }

        public void SetUserBlocked(string username, bool value)
        {
            if (contacts.ContainsKey(username))
            {
                contacts[username].isBlocked = value;
            }
        }

        public void SetUserAccepted(string username, bool value)
        {
            if (contacts.ContainsKey(username))
            {
                contacts[username].isAccepted = value;
            }
        }

        public bool IsUserInContactList(string username)
        {
            bool isUserInList = false;

            if (contacts.ContainsKey(username))
            {
                isUserInList = true;
            }

            return isUserInList;
        }

        public string CreateData()
        {
            string data = "";

            foreach (KeyValuePair<String, Contact> contact in contacts)
            {
                string isBlockedValue = "0";
                string isAcceptedValue = "0";

                if (contact.Value.isBlocked)
                {
                    isBlockedValue = "1";
                }

                if (contact.Value.isAccepted)
                {
                    isAcceptedValue = "1";
                }

                data += "[" + contact.Key.ToString() + "," + isBlockedValue + "," + isAcceptedValue + "]";
            }

            data = data.Trim();
            data = data.Replace(" ", "");
            data = data.Replace("\n", "");

            return data;
        }

        public List<Contact> GetContacts()
        {
            List<Contact> contactsList = new List<Contact>();

            foreach (KeyValuePair<String, Contact> contact in contacts)
            {
                contactsList.Add(contact.Value);
            }

            return contactsList;
        }
    }
}
