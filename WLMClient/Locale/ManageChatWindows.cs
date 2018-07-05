using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WLMClient.UI.Windows;
using WLMClient.Locale;
using WLMData.Data.Packets;
using WLMClient.Resource.Sounds;
using WLMData.Enums;

namespace WLMClient.Locale
{
    class ManageChatWindows
    {
        public static void CloseAllOpenChatWindows()
        {
            lock (Personal.OPEN_CHAT_WINDOWS)
            {
                List<ChatWindow> closeChatWindows = new List<ChatWindow>();

                foreach (ChatWindow chatWindow in Personal.OPEN_CHAT_WINDOWS)
                {
                    closeChatWindows.Add(chatWindow);
                }

                foreach (ChatWindow chatWindow in closeChatWindows)
                {
                    chatWindow.CloseWindow();
                }

                Personal.OPEN_CHAT_WINDOWS.Clear();
            }
        }

        public static void RemoveChatWindow(ChatWindow chatWindow)
        {
            lock (Personal.OPEN_CHAT_WINDOWS)
            {
                if (Personal.OPEN_CHAT_WINDOWS.Contains(chatWindow))
                {
                    Personal.OPEN_CHAT_WINDOWS.Remove(chatWindow);
                    chatWindow.CloseWindow();
                }
            }
        }

        public static void RemoveChatWindow(string userID)
        {
            bool isChatWindowOpen = IsChatWindowOpen(userID);

            if (isChatWindowOpen)
            {
                ChatWindow foundChatWindow = GetChatWindow(userID);

                foundChatWindow.CloseWindow();
                RemoveChatWindow(foundChatWindow);
            }
        }

        public static void UpdateChatWindowPersonal()
        {
            lock (Personal.OPEN_CHAT_WINDOWS)
            {
                foreach (ChatWindow chatWindow in Personal.OPEN_CHAT_WINDOWS)
                {
                    chatWindow.UpdatePersonal();
                }
            }
        }

        public static void ReceiveNudge(string userID)
        {
            bool isChatWindowOpen = IsChatWindowOpen(userID);
            ChatWindow foundChatWindow = GetChatWindow(userID);

            if (foundChatWindow != null)
            {
                foundChatWindow.Nudge();

                if (!foundChatWindow.IsFocused & Personal.USER_INFO.status == Convert.ToInt16(UserStatus.Available))
                {
                    Player.PlaySound(Identifiers.NUDGE);

                    if (!isChatWindowOpen)
                    {
                        string truncatedMessage = "Sent you a nudge!";
                        Notification.NotificationManager.Showpopup(foundChatWindow.GetContactName(), truncatedMessage, foundChatWindow);
                    }
                }

                FlashWindowManager.FlashWindow(foundChatWindow, 5);
            }
        }

        public static void ReceiveWritingStatus(WritingStatus writingStatus)
        {
            bool isChatWindowOpen = IsChatWindowOpen(writingStatus.id);

            if (isChatWindowOpen)
            {
                ChatWindow foundChatWindow = GetChatWindow(writingStatus.id);
                foundChatWindow.IsWritingAMessage(writingStatus.isWriting);
            }
        }

        public static void UpdateChatWindowUser(UserInfo userInfo)
        {
            bool isChatWindowOpen = IsChatWindowOpen(userInfo.id);

            if (isChatWindowOpen)
            {
                ChatWindow foundChatWindow = GetChatWindow(userInfo.id);

                foundChatWindow.UpdateContact(userInfo);
            }
        }

        public static void ReceiveMessage(Message chatMessage)
        {
            bool isChatWindowOpen = IsChatWindowOpen(chatMessage.id);
            ChatWindow foundChatWindow = GetChatWindow(chatMessage.id);

            if (foundChatWindow != null)
            {
                foundChatWindow.AddChatMessage(foundChatWindow.GetContactName(), chatMessage.message);

                if (!foundChatWindow.IsFocused & Personal.USER_INFO.status == Convert.ToInt16(UserStatus.Available))
                {
                    Player.PlaySound(Identifiers.NEW_MSG);

                    if (!isChatWindowOpen)
                    {
                        string truncatedMessage = chatMessage.message.Length <= 20 ? chatMessage.message : chatMessage.message.Substring(0, 20) + "...";
                        Notification.NotificationManager.Showpopup(foundChatWindow.GetContactName(), truncatedMessage, foundChatWindow);
                    }
                }

                FlashWindowManager.FlashWindow(foundChatWindow, 5);
            }
        }

        public static bool IsChatWindowOpen(string userID)
        {
            bool returnValue = false;

            lock (Personal.OPEN_CHAT_WINDOWS)
            {
                foreach (ChatWindow chatWindow in Personal.OPEN_CHAT_WINDOWS)
                {
                    if (chatWindow.GetContactID().Equals(userID))
                    {
                        returnValue = true;
                        break;
                    }
                }
            }

            return returnValue;
        }

        public static ChatWindow GetChatWindow(string userID)
        {
            ChatWindow foundChatWindow = null;

            foreach (ChatWindow chatWindow in Personal.OPEN_CHAT_WINDOWS)
            {
                if (chatWindow.GetContactID().Equals(userID))
                {
                    foundChatWindow = chatWindow;
                    break;
                }
            }

            if (foundChatWindow == null)
            {
                UserInfo userInfo = Personal.USER_CONTACTS.FirstOrDefault(x => x.id.Trim() == userID.Trim());
                if (userInfo != null)
                {
                    foundChatWindow = new ChatWindow(userInfo);
                    foundChatWindow.Show();

                    Personal.OPEN_CHAT_WINDOWS.Add(foundChatWindow);
                }
            }

            return foundChatWindow;
        }
    }
}
