using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLMServer.Data
{
    class Contact
    {
        public string id { get; set; }
        public bool isBlocked { get; set; }
        public bool isAccepted { get; set; }

        public Contact(string id, bool isBlocked, bool isAccepted)
        {
            this.id = id;
            this.isBlocked = isBlocked;
            this.isAccepted = isAccepted;
        }
    }

    class ContactRegister
    {
        private Dictionary<String, Contact> contacts;

        public ContactRegister(string contactDataString)
        {
            contacts = new Dictionary<String, Contact>();

            contactDataString = contactDataString.Trim();
            contactDataString = contactDataString.Replace(" ", "");
            contactDataString = contactDataString.Replace("\n", "");

            if (contactDataString.Length == 0)
            {
                return;
            }

            string[] contactArray = contactDataString.Split('[');
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
    }
}
