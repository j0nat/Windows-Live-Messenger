using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WLMData.Data.Packets;
using WLMClient.UI.Data;
using WLMClient.UI.Data.Enums;
using WLMClient.Locale;
using WLMData.Enums;
using WLMClient.Layout;
using WLMClient.UI.Controls.WinForms;
using WLMClient.Network;

namespace WLMClient.UI.Controls
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        private List<ContactListEntryData> listContacts;
        private bool isEnterKeyDownInComment;
        private bool isCommentEditSubmitted;
        private bool isCommentBeingEdited;

        public Main()
        {
            listContacts = new List<ContactListEntryData>();

            InitializeComponent();

            UpdatePersonalInformation();

            menuItemIconArrowAvailable.Source = LoadResource.GetSmallIconFromStatus(UserStatus.Available);
            menuItemIconArrowBusy.Source = LoadResource.GetSmallIconFromStatus(UserStatus.Busy);
            menuItemIconArrowAway.Source = LoadResource.GetSmallIconFromStatus(UserStatus.Away);
            menuItemIconArrowOffline.Source = LoadResource.GetSmallIconFromStatus(UserStatus.Offline);

            isEnterKeyDownInComment = false;
            isCommentEditSubmitted = false;
            isCommentBeingEdited = false;
        }

        private void btnArrowStatus_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            contextMenuArrow.IsOpen = true;
        }

        private void txtQuickMessage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                isEnterKeyDownInComment = true;
                isCommentBeingEdited = false;
            }

            if (e.Key == Key.Escape)
            {
                ResetComment();
                isCommentBeingEdited = false;
                e.Handled = true;
            }
        }

        private void txtQuickMessage_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (isEnterKeyDownInComment)
            {
                isCommentEditSubmitted = true;
                txtQuickMessage.SelectionStart = 0;
                Keyboard.ClearFocus();
                Personal.USER_INFO.comment = txtQuickMessage.Text.Trim();
                isEnterKeyDownInComment = false;

                Client.SendUserUpdate();
                UpdatePersonalInformation();

                isCommentBeingEdited = false;

                ResetComment();
            }
        }

        private void txtQuickMessage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Personal.USER_INFO.comment.Length == 0)
            {
                txtQuickMessage.Clear();
                txtQuickMessage.Focus();
            }

            isCommentBeingEdited = true;
        }

        private void txtQuickMessage_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ResetComment();
        }

        public void ResetComment()
        {
            if (!isCommentEditSubmitted)
            {
                if (Personal.USER_INFO.comment.Length == 0)
                {
                    txtQuickMessage.Text = "Share a quick message";
                }
                else
                {
                    txtQuickMessage.Text = Personal.USER_INFO.comment;
                }
            }

            Keyboard.ClearFocus();
            isCommentBeingEdited = false;
            isCommentEditSubmitted = false;
        }

        public void WindowSizeChanged(double width, double height)
        {
            if (width >= 555)
            {
                background.Source = new BitmapImage(new Uri(Resource.Images.Identifiers.CHAT_WINDOW_BACKGROUND_WIDE, UriKind.Absolute));
            }

            if (width < 555)
            {
                background.Source = new BitmapImage(new Uri(Resource.Images.Identifiers.CHAT_WINDOW_BACKGROUND_SKINNY, UriKind.Absolute));
            }

            foreach (ContactListEntryData contact in listContacts)
            {
                contact.richTextBox.Width = listContactBorder.ActualWidth - 2;
            }

            mainControl.Height = height;
        }

        public void UpdatePersonalInformation()
        {
            txtName.Text = Personal.USER_INFO.name;
            txtStatus.Text = "(" + ((UserStatus)Personal.USER_INFO.status).ToString() + ")";

            if (Personal.USER_INFO.comment.Length != 0)
            {
                txtQuickMessage.Text = Personal.USER_INFO.comment;
            }
            else
            {
                txtQuickMessage.Text = "Share a quick message";
            }

            imagePartnerFrame.Source = LoadResource.GetAvatarFrameFromStatus((UserStatus)Personal.USER_INFO.status, AvatarSize.Small);

            if (Personal.USER_INFO.avatar != "")
            {
                BitmapImage image = LoadResource.GetAvatar(Personal.USER_INFO.avatar);

                if (image != null)
                {
                    imagePartnerAvatar.Source = LoadResource.Resize(image, 50, 50, BitmapScalingMode.HighQuality);
                }
                else
                {
                    imagePartnerAvatar.Source = LoadResource.Resize(LoadResource.GetDefaultAvatarImage(), 50, 50, BitmapScalingMode.HighQuality);
                }
            }
            else
            {
                imagePartnerAvatar.Source = LoadResource.Resize(LoadResource.GetDefaultAvatarImage(), 50, 50, BitmapScalingMode.HighQuality);
            }

            Window mainWindow = Application.Current.MainWindow;
            if (Personal.USER_INFO.status == 0) // Offline
            {
                mainWindow.Icon = new BitmapImage(new Uri(Resource.Images.Identifiers.APP_ICON_STATUS_OFFLINE, UriKind.Absolute));
            }

            if (Personal.USER_INFO.status == 1) // Busy
            {
                mainWindow.Icon = new BitmapImage(new Uri(Resource.Images.Identifiers.APP_ICON_STATUS_BUSY, UriKind.Absolute));
            }

            if (Personal.USER_INFO.status == 2) // Away
            {
                mainWindow.Icon = new BitmapImage(new Uri(Resource.Images.Identifiers.APP_ICON_STATUS_AWAY, UriKind.Absolute));
            }

            if (Personal.USER_INFO.status == 3) // Available
            {
                mainWindow.Icon = new BitmapImage(new Uri(Resource.Images.Identifiers.APP_ICON_STATUS_AVAILABLE, UriKind.Absolute));
            }

            TextParser.ParseText(txtQuickMessage, false);
            TextParser.ParseText(txtName, false);

            ManageChatWindows.UpdateChatWindowPersonal();
        }

        public void AddContactToList(UserInfo contact)
        {
            RichTextBox txtContact = new RichTextBox();
            txtContact.PreviewMouseDoubleClick += TxtContact_PreviewMouseDoubleClick;

            txtContact.IsDocumentEnabled = true;

            ContextMenu context = new ContextMenu();
            MenuItem blockItem = new MenuItem(); blockItem.Tag = contact.id;
            MenuItem deleteItem = new MenuItem(); deleteItem.Tag = contact.id;
            MenuItem openChatItem = new MenuItem(); openChatItem.Tag = contact.id;

            if (contact.blocked)
            {
                blockItem.Header = "Unblock";
            }
            else
            {
                blockItem.Header = "Block";
            }

            openChatItem.Header = "Send Message";
            deleteItem.Header = "Remove Contact";

            context.Items.Add(openChatItem);
            context.Items.Add(new Separator());
            context.Items.Add(blockItem);
            context.Items.Add(deleteItem);

            blockItem.Click += BlockItem_Click;
            deleteItem.Click += DeleteItem_Click;
            openChatItem.Click += OpenChatItem_Click;

            txtContact.ContextMenu = context;

            txtContact.PreviewMouseDown += new MouseButtonEventHandler(txtContact_PreviewMouseDown);
            txtContact.MouseEnter += new MouseEventHandler(txtContact_MouseEnter);
            txtContact.MouseLeave += new MouseEventHandler(txtContact_MouseLeave);
            txtContact.Document.Blocks.Clear();
            txtContact.Margin = new Thickness(0, 0, 0, 2);
            txtContact.BorderThickness = new Thickness(0);
            txtContact.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FCFCFC"));
            txtContact.VerticalAlignment = VerticalAlignment.Top;
            txtContact.IsReadOnly = true;
            txtContact.Height = 30;
            txtContact.Tag = contact.id;
            txtContact.ToolTip = contact.id;

            Style style = this.FindResource("txtContactStyle") as Style;
            txtContact.Style = style;
            txtContact.Cursor = Cursors.Arrow;

            Image imgStatus = new Image();
            imgStatus.SnapsToDevicePixels = true;
            imgStatus.Source = LoadResource.GetSmallIconFromStatus((UserStatus)contact.status);
            imgStatus.Width = 16;
            imgStatus.Height = 16;
            imgStatus.Stretch = Stretch.Fill;
            imgStatus.Margin = new Thickness(0, 4, 0, 0);

            RenderOptions.SetBitmapScalingMode(imgStatus, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(imgStatus, EdgeMode.Aliased);
            InlineUIContainer container = new InlineUIContainer(imgStatus);

            Paragraph paragraph = new Paragraph(container);
            paragraph.Padding = new Thickness(0, 0, 0, 0);
            paragraph.Margin = new Thickness(6, 0, 0, 0);
            paragraph.TextAlignment = TextAlignment.Left;

            txtContact.Document.Blocks.Add(paragraph);

            TextPointer tp = paragraph.ContentEnd;
            TextBlock txtName = new TextBlock();

            if (contact.comment.Length > 0)
            {
                txtName.Text = contact.name + " - ";
            }
            else
            {
                txtName.Text = contact.name;
            }

            txtName.Margin = new Thickness(5, 0, 0, 1);
            txtName.FontFamily = new FontFamily("Segoe UI");
            txtName.FontSize = 12;
            txtName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#333333"));
            txtName.FontWeight = FontWeights.Normal;

            InlineUIContainer iui = new InlineUIContainer(txtName, tp);
            iui.BaselineAlignment = BaselineAlignment.TextBottom;
            TextParser.ParseText(txtName, false);

            TextBlock txtQuickMessage = new TextBlock();

            tp = paragraph.ContentEnd;
            txtQuickMessage.Text = contact.comment;
            txtQuickMessage.Margin = new Thickness(5, 0, 0, 1);
            txtQuickMessage.FontFamily = new FontFamily("Segoe UI");
            txtQuickMessage.FontSize = 12;
            txtQuickMessage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#888888"));
            txtQuickMessage.FontWeight = FontWeights.Normal;

            iui = new InlineUIContainer(txtQuickMessage, tp);
            iui.BaselineAlignment = BaselineAlignment.TextBottom;

            TextParser.ParseText(txtQuickMessage, true);

            listContacts.Add(new ContactListEntryData(txtContact, imgStatus, txtName, txtQuickMessage));

            if (contact.status != 0)
            {
                contactListView.Items.Insert(0, txtContact);
            }
            else
            {
                contactListView.Items.Insert(contactListView.Items.Count, txtContact);
            }
        }

        private void TxtContact_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (UserInfo contact in Personal.USER_CONTACTS)
            {
                if (contact.id.ToString() == ((RichTextBox)sender).Tag.ToString())
                {
                    OpenChatWindow(contact);

                    e.Handled = true;

                    break;
                }
            }
        }

        public void OpenChatWindow(UserInfo userInfo)
        {
            Windows.ChatWindow chatWindow = ManageChatWindows.GetChatWindow(userInfo.id);
            if (chatWindow != null)
            {
                chatWindow.Focus();
            }
        }

        private void OpenChatItem_Click(object sender, RoutedEventArgs e)
        {
            Windows.ChatWindow chatWindow = ManageChatWindows.GetChatWindow(((MenuItem)sender).Tag.ToString());
            if (chatWindow != null)
            {
                chatWindow.Focus();
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            Client.DeleteContact(((MenuItem)sender).Tag.ToString());
        }

        public void RemoveContact(string userID)
        {
            lock (contactListView)
            {
                for (int i = 0; i < contactListView.Items.Count; i++)
                {
                    if (contactListView.Items[i].GetType() == typeof(RichTextBox))
                    {
                        if (((RichTextBox)contactListView.Items[i]).Tag.ToString() == userID)
                        {
                            ManageChatWindows.RemoveChatWindow(userID);

                            contactListView.Items.Remove(((RichTextBox)contactListView.Items[i]));
                            UpdateContactCount();

                            ContactListEntryData contactListData = null;
                            UserInfo user = null;
                            foreach (ContactListEntryData contactData in listContacts)
                            {
                                if (contactData.richTextBox.Tag.ToString() == userID)
                                {
                                    contactListData = contactData;
                                    break;
                                }
                            }

                            foreach (UserInfo userData in Personal.USER_CONTACTS)
                            {
                                if (userData.id == userID)
                                {
                                    user = userData;
                                    break;
                                }
                            }

                            if (user != null)
                            {
                                lock (Personal.USER_CONTACTS)
                                {
                                    Personal.USER_CONTACTS.Remove(user);
                                }
                            }

                            if (contactListData != null)
                            {
                                lock (listContacts)
                                {
                                    listContacts.Remove(contactListData);
                                }
                            }

                            UpdateContactCount();
                        }
                    }
                }
            }
        }

        private void BlockItem_Click(object sender, RoutedEventArgs e)
        {
            lock (contactListView)
            {
                for (int i = 0; i < contactListView.Items.Count; i++)
                {
                    if (contactListView.Items[i].GetType() == typeof(RichTextBox))
                    {
                        if (((RichTextBox)contactListView.Items[i]).Tag.ToString() == ((MenuItem)sender).Tag.ToString())
                        {
                            ContextMenu contextMenu = ((RichTextBox)contactListView.Items[i]).ContextMenu;

                            string blockMenuHeaderText = ((MenuItem)contextMenu.Items[2]).Header.ToString();

                            if (blockMenuHeaderText == "Block")
                            {
                                ((MenuItem)contextMenu.Items[2]).Header = "Unblock";
                            }
                            else
                            {
                                ((MenuItem)contextMenu.Items[2]).Header = "Block";
                            }

                            break;
                        }
                    }
                }
            }

            Client.BlockContact(((MenuItem)sender).Tag.ToString());
        }

        void txtContact_MouseLeave(object sender, MouseEventArgs e)
        {
            if (((RichTextBox)sender).IsFocused == false)
            {
                ((RichTextBox)sender).BorderThickness = new Thickness(0);
                ((RichTextBox)sender).Background = Brushes.Transparent;
            }
        }

        void txtContact_MouseEnter(object sender, MouseEventArgs e)
        {
            if (((RichTextBox)sender).IsFocused == false)
            {
                LinearGradientBrush gradientBrush = new LinearGradientBrush(Color.FromRgb(235, 243, 253), Color.FromRgb(252, 253, 254), new Point(0.5, 0), new Point(0.5, 1));
                ((RichTextBox)sender).Background = gradientBrush;
                ((RichTextBox)sender).BorderThickness = new Thickness(1, 1, 1, 1);
                ((RichTextBox)sender).BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8D6FB"));
            }
        }

        void txtContact_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (ContactListEntryData contact in listContacts)
            {
                contact.richTextBox.BorderThickness = new Thickness(0);
                contact.richTextBox.Background = Brushes.Transparent;
            }

            LinearGradientBrush gradientBrush = new LinearGradientBrush(Color.FromRgb(235, 244, 254), Color.FromRgb(207, 228, 254), new Point(0.5, 0), new Point(0.5, 1));
            ((RichTextBox)sender).Background = gradientBrush;
            ((RichTextBox)sender).BorderThickness = new Thickness(1, 1, 1, 1);
            ((RichTextBox)sender).BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#84ACDD"));
        }

        public void UpdateContact(UserInfo userInfo)
        {
            lock (Personal.USER_CONTACTS)
            {
                UserInfo userFound = Personal.USER_CONTACTS.FirstOrDefault(p => p.id == userInfo.id.Trim());

                if (userFound == null)
                {
                    Personal.USER_CONTACTS.Add(userInfo);
                    AddContactToList(userInfo);
                }
                else
                {
                    bool userJustLoggedOn = false;
                    bool userJustLoggedOff = false;

                    ManageChatWindows.UpdateChatWindowUser(userInfo);

                    if (userFound.status != userInfo.status & userFound.status == Convert.ToInt16(UserStatus.Offline)
                        & userInfo.status != Convert.ToInt16(UserStatus.Offline))
                    {
                        userJustLoggedOn = true;
                    }

                    if (userFound.status != userInfo.status & userFound.status != Convert.ToInt16(UserStatus.Offline)
                        & userInfo.status == Convert.ToInt16(UserStatus.Offline))
                    {
                        userJustLoggedOff = true;
                    }

                    userFound.name = userInfo.name;
                    userFound.status = userInfo.status;
                    userFound.avatar = userInfo.avatar;
                    userFound.comment = userInfo.comment;

                    userFound = userInfo;

                    foreach (ContactListEntryData contact in listContacts)
                    {
                        if (contact.richTextBox.ToolTip.ToString() == userFound.id.ToString())
                        {
  
                            contact.image.Source = LoadResource.GetSmallIconFromStatus((UserStatus)userInfo.status);
                            if (userInfo.comment.Length == 0)
                            {
                                contact.name.Text = userInfo.name;
                            }
                            else
                            {
                                contact.name.Text = userInfo.name + " - ";
                            }


                            if (userJustLoggedOn)
                            {
                                contactListView.Items.Remove(contact.richTextBox);
                                contactListView.Items.Insert(0, contact.richTextBox);
                            }

                            if (userJustLoggedOff)
                            {
                                contactListView.Items.Remove(contact.richTextBox);
                                contactListView.Items.Insert(contactListView.Items.Count, contact.richTextBox);
                            }

                            contact.comment.Text = userInfo.comment;

                            TextParser.ParseText(contact.name, false);
                            TextParser.ParseText(contact.comment, true);

                            break;
                        }
                    }

                    if (userJustLoggedOn & Personal.USER_INFO.status == Convert.ToInt16(UserStatus.Available) & !userInfo.blocked)
                    {
                        Notification.NotificationManager.Showpopup(userInfo.name, "has just signed in.", null);

                        Resource.Sounds.Player.PlaySound(Resource.Sounds.Identifiers.ONLINE);
                    }
                }
            }

            UpdateContactCount();
        }

        public void UpdateContactCount()
        {
            int countOnlineContacts = 0;

            foreach (UserInfo contact in Personal.USER_CONTACTS)
            {
                if ((UserStatus)contact.status != UserStatus.Offline)
                {
                    countOnlineContacts++;
                }
            }

            txtFriends.Text = String.Format("Friends ({0}/{1})", countOnlineContacts, Personal.USER_CONTACTS.Count);
        }

        private void btnAddFriend_Click(object sender, RoutedEventArgs e)
        {
            FrmAddNewFriend frmAddNewFriend = new FrmAddNewFriend();
            frmAddNewFriend.ShowDialog();

            borderAddFriend.BorderThickness = new Thickness(0);
        }

        private void btnAddFriend_MouseEnter(object sender, MouseEventArgs e)
        {
            borderAddFriend.BorderThickness = new Thickness(1);
            borderAddFriend.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D6D6D6"));
        }

        private void btnAddFriend_MouseLeave(object sender, MouseEventArgs e)
        {
            borderAddFriend.BorderThickness = new Thickness(0);
        }

        private void menuItemArrowBusy_Click(object sender, RoutedEventArgs e)
        {
            Personal.USER_INFO.status = Convert.ToInt16(UserStatus.Busy);
            Client.SendUserUpdate();
        }

        private void menuItemArrowAvailable_Click(object sender, RoutedEventArgs e)
        {
            Personal.USER_INFO.status = Convert.ToInt16(UserStatus.Available);
            Client.SendUserUpdate();
        }

        private void menuItemArrowAway_Click(object sender, RoutedEventArgs e)
        {
            Personal.USER_INFO.status = Convert.ToInt16(UserStatus.Away);
            Client.SendUserUpdate();
        }

        private void menuItemArrowOffline_Click(object sender, RoutedEventArgs e)
        {
            Personal.USER_INFO.status = Convert.ToInt16(UserStatus.Offline);
            Client.SendUserUpdate();
        }

        private void menuItemArrowOptions_Click(object sender, RoutedEventArgs e)
        {
            FrmOptions frmOptions = new FrmOptions();
            frmOptions.ShowDialog();
        }

        private void menuItemArrowExit_Click(object sender, RoutedEventArgs e)
        {
            ManageChatWindows.CloseAllOpenChatWindows();

            Environment.Exit(0);
        }

        private void txtQuickMessage_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!isCommentBeingEdited)
            {
                Keyboard.ClearFocus();
            }
        }
    }
}
