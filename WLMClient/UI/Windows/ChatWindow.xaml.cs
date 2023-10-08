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
using System.Windows.Shapes;

using WLMClient.UI.Data.Enums;
using WLMClient.Layout;
using WLMData.Enums;
using WLMData.Data.Packets;
using WLMClient.Locale;
using WLMClient.UI.Data;

using System.Threading;

namespace WLMClient.UI.Windows
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private DateTime lastMessageReceivedDateTime;
        private UserInfo contactUserInfo;
        private StackPanel emoticonPanel;
        private string lastMessageFrom;
        private bool textInputChanged;
        private bool isMouseHoveringEmoticonPanel;
        private bool isWindowClosing;
        private bool isShiftDown;
        private bool isWritingMessage;

        public ChatWindow(UserInfo userInfo)
        {
            InitializeComponent();

            contactUserInfo = userInfo;
            lastMessageReceivedDateTime = DateTime.Now;
            isMouseHoveringEmoticonPanel = false;
            textInputChanged = false;
            isWindowClosing = false;
            isShiftDown = false;
            isWritingMessage = false;
            lastMessageFrom = "";

            LoadEmoticonPanel();

            btnSmiley.Source = LoadResource.chatWindowButtonSmileys(ButtonState.None);
            btnNudge.Source = LoadResource.chatWindowButtonNudge(ButtonState.None);

            background.Source = new BitmapImage(new Uri(Resource.Images.Identifiers.CHAT_WINDOW_BACKGROUND_SKINNY, UriKind.Absolute));

            txtChat.Document.Blocks.Clear();

            UpdatePersonal();
            UpdateContact(userInfo);

            Thread threadParseInputText = new Thread(TextInputParser);
            threadParseInputText.IsBackground = true;
            threadParseInputText.Start();
        }

        public string GetContactID()
        {
            return contactUserInfo.id;
        }

        public string GetContactName()
        {
            return contactUserInfo.name;
        }

        private void TextInputParser()
        {
            while (!isWindowClosing)
            {
                if (textInputChanged)
                {
                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
                    {
                        TextParser.ParseText(txtSend, false);
                    }));

                    textInputChanged = false;
                }

                Thread.Sleep(666);
            }
        }

        public void AddChatMessage(string from, string messageText)
        {
            if (from == contactUserInfo.name)
            {
                txtLastUpdate.Document.Blocks.Clear();
                txtLastUpdate.Document.Blocks.Add(new Paragraph(new Run("Last message received at " +
                    lastMessageReceivedDateTime.ToShortTimeString() +
                    " on " + lastMessageReceivedDateTime.ToShortDateString() + ".")));
            }

            Paragraph txtFrom = new Paragraph();
            Paragraph txtText = new Paragraph();

            txtFrom.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;

            txtFrom.Inlines.Add(from + " says");
            txtFrom.TextAlignment = TextAlignment.Justify;
            txtFrom.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#A5A5A5"));
            txtFrom.FontSize = 14;
            txtFrom.FontWeight = FontWeights.Normal;
            txtFrom.Margin = new Thickness(0, 24, 0, 0);
            txtFrom.Padding = new Thickness(0, 0, 0, 0);
            txtFrom.LineHeight = 0.1;
            
            Image paragraphDot = LoadResource.getParagraphRectangle();
            TextPointer pointerParagraphDot = txtChat.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);
            InlineUIContainer inlineParagraphDot = new InlineUIContainer(paragraphDot, pointerParagraphDot);
            txtText.Inlines.Add(inlineParagraphDot);

            paragraphDot.Margin = new Thickness(-20, 0, 0, 2);

            txtText.Inlines.Add(messageText.TrimEnd());
            txtText.LineHeight = 0.1;
            txtText.Margin = new Thickness(12, 3, 0, 0);

            if (lastMessageFrom != from)
            {
                txtChat.Document.Blocks.Add(txtFrom);
                TextParser.ProcessInlines(txtChat, txtFrom.Inlines, false);
            }

            txtChat.Document.Blocks.Add(txtText);
            TextParser.ProcessInlines(txtChat, txtText.Inlines, true);

            txtChat.ScrollToEnd();

            lastMessageFrom = from;
        }

        public void IsWritingAMessage(bool value)
        {
            if (value)
            {
                txtLastUpdate.Document.Blocks.Clear();

                Paragraph lastUpdateParagraph = new Paragraph(new Run(contactUserInfo.name + " is writing a message."));

                txtLastUpdate.Document.Blocks.Add(lastUpdateParagraph);

                TextParser.ProcessInlines(txtLastUpdate, lastUpdateParagraph.Inlines, false);

            }
            else
            {
                txtLastUpdate.Document.Blocks.Clear();
                txtLastUpdate.Document.Blocks.Add(new Paragraph(new Run("Last message received at " +
                    lastMessageReceivedDateTime.ToShortTimeString() +
                    " on " + lastMessageReceivedDateTime.ToShortDateString() + ".")));
            }
        }

        public void UpdatePersonal()
        {
            imageUserAvatar.Source = LoadResource.GetDefaultAvatarImage();
            imageUserFrame.Source = LoadResource.GetAvatarFrameFromStatus((UserStatus)Personal.USER_INFO.status, AvatarSize.Big);

            if (Personal.USER_INFO.avatar != "")
            {
                BitmapImage image = LoadResource.GetAvatar(Personal.USER_INFO.avatar);
                if (image != null)
                {
                    imageUserAvatar.Source = image;
                }
                else
                {
                    imageUserAvatar.Source = LoadResource.GetDefaultAvatarImage();
                }
            }
            else
            {
                imageUserAvatar.Source = LoadResource.GetDefaultAvatarImage();
            }
        }

        public void UpdateContact(UserInfo userInfo)
        {
            txtName.Text = userInfo.name;
            TextParser.ParseText(txtName, false);
            this.Title = userInfo.name;
            txtStatus.Text = "(" + ((UserStatus)userInfo.status).ToString() + ")";

            if (userInfo.status == (int)UserStatus.Offline || Personal.USER_INFO.status == (int)UserStatus.Offline || userInfo.blocked == true)
            {
                this.IsEnabled = false;
            }
            else
            {
                this.IsEnabled = true;
            }

            if (userInfo.avatar != "")
            {
                BitmapImage image = LoadResource.GetAvatar(userInfo.avatar);
                if (image != null)
                {
                    imagePartnerAvatar.Source = image;
                }
                else
                {
                    imagePartnerAvatar.Source = LoadResource.GetDefaultAvatarImage();
                }
            }
            else
            {
                imagePartnerAvatar.Source = LoadResource.GetDefaultAvatarImage();
            }

            imagePartnerFrame.Source = LoadResource.GetAvatarFrameFromStatus((UserStatus)userInfo.status, AvatarSize.Big);

            contactUserInfo = userInfo;
        }

        private void LoadEmoticonPanel()
        {
            emoticonPanel = new StackPanel();

            emoticonPanel.Background = Brushes.White;
            emoticonPanel.Margin = new Thickness(151, 0, 0, 73);
            emoticonPanel.VerticalAlignment = VerticalAlignment.Bottom;
            emoticonPanel.HorizontalAlignment = HorizontalAlignment.Left;

            Controls.Emoticons emoticonPage = new Controls.Emoticons(txtSend);
            emoticonPage.IsVisibleChanged += EmoticonPage_IsVisibleChanged;
            emoticonPanel.Children.Add(emoticonPage);

            emoticonPanel.MouseEnter += Emoticons_MouseEnter;
            emoticonPanel.MouseLeave += Emoticons_MouseLeave;
        }

        private void EmoticonPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                window.Children.Remove(emoticonPanel);

                emoticonPanel.Children[0].Visibility = Visibility.Visible;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isWindowClosing)
            {
                isWindowClosing = true;

                ManageChatWindows.RemoveChatWindow(this);
            }
        }

        public void CloseWindow()
        {
            if (!isWindowClosing)
            {
                isWindowClosing = true;

                ManageChatWindows.RemoveChatWindow(this);

                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (emoticonPanel != null)
            {
                if (!isMouseHoveringEmoticonPanel)
                {
                    window.Children.Remove(emoticonPanel);
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void btnGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void imageUserFrame_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void imageUserFrame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void imageUserFrame_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void txtSend_TextChanged(object sender, TextChangedEventArgs e)
        {
            textInputChanged = true;

            var start = txtSend.Document.ContentStart;
            var end = txtSend.Document.ContentEnd;
            int difference = start.GetOffsetToPosition(end);

            if (difference != 0)
            {
                if (!isWritingMessage)
                {
                    Network.Client.SendWritingStatus(contactUserInfo.id, true);

                    isWritingMessage = true;
                }
            }
            else
            {
                Network.Client.SendWritingStatus(contactUserInfo.id, false);
                isWritingMessage = false;
            }
        }

        private void txtSend_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift | e.Key == Key.RightShift)
            {
                isShiftDown = false;
            }

            if (e.Key == Key.Enter & !isShiftDown)
            {
                string txtSendChat = TextParser.GetPlainText(txtSend.Document);

                if (txtSendChat.Length == 0 || txtSendChat.Trim() == "")
                {
                    return;
                }

                AddChatMessage(Personal.USER_INFO.name, txtSendChat);

                Network.Client.SendMessage(new Message(contactUserInfo.id, txtSendChat));

                txtSend.Document.Blocks.Clear();
            }
        }

        private void txtSend_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift | e.Key == Key.RightShift)
            {
                isShiftDown = true;
            }

            if (e.Key == Key.Enter & !isShiftDown)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter & isShiftDown)
            {
                InsertText("\n", txtSend);

                e.Handled = true;
            }
        }

        private void InsertText(String text, RichTextBox rtb)
        {
            rtb.CaretPosition = rtb.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            rtb.CaretPosition.InsertTextInRun(text);
        }

        private void txtSend_PreviewDrop(object sender, DragEventArgs e)
        {

        }

        private void txtSend_PreviewDragOver(object sender, DragEventArgs e)
        {

        }

        private void btnSmiley_MouseEnter(object sender, MouseEventArgs e)
        {
            btnSmiley.Source = LoadResource.chatWindowButtonSmileys(ButtonState.Hover);
        }

        private void btnSmiley_MouseLeave(object sender, MouseEventArgs e)
        {
            btnSmiley.Source = LoadResource.chatWindowButtonSmileys(ButtonState.None);
        }

        private void btnSmiley_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnSmiley.Source = LoadResource.chatWindowButtonSmileys(ButtonState.Pressed);
        }

        private void btnSmiley_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnSmiley.Source = LoadResource.chatWindowButtonSmileys(ButtonState.Hover);

            window.Children.Add(emoticonPanel);
        }

        private void Emoticons_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseHoveringEmoticonPanel = false;
        }

        private void Emoticons_MouseEnter(object sender, MouseEventArgs e)
        {
            isMouseHoveringEmoticonPanel = true;
        }

        private void btnNudge_MouseEnter(object sender, MouseEventArgs e)
        {
            btnNudge.Source = LoadResource.chatWindowButtonNudge(ButtonState.Hover);
        }

        private void btnNudge_MouseLeave(object sender, MouseEventArgs e)
        {
            btnNudge.Source = LoadResource.chatWindowButtonNudge(ButtonState.None);
        }

        private void btnNudge_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnNudge.Source = LoadResource.chatWindowButtonNudge(ButtonState.Pressed);
        }

        private void btnNudge_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnNudge.Source = LoadResource.chatWindowButtonNudge(ButtonState.Hover);

            AddNudgeMessage("You have just sent a Nudge!");
            Network.Client.SendNudge(contactUserInfo.id);
        }

        public void AddNudgeMessage(string text)
        {
            lastMessageFrom = "";

            Paragraph txtText = new Paragraph();
            txtText.Inlines.Add(text);
            txtText.TextAlignment = TextAlignment.Justify;
            txtText.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#29292B"));
            txtText.FontSize = 13;
            txtText.FontWeight = FontWeights.Normal;
            txtText.LineHeight = 18;
            txtText.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            txtText.Margin = new Thickness(0, 10, 0, 0);
            txtText.Padding = new Thickness(5);

            txtText.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#eaeaea"));
            txtText.BorderThickness = new Thickness(0, 2, 0, 2);

            txtChat.Document.Blocks.Add(txtText);
            TextParser.ProcessInlines(txtChat, txtText.Inlines, false);
            
            txtChat.ScrollToEnd();
        }

        public void Nudge()
        {
            AddNudgeMessage(contactUserInfo.name + " just sent you a Nudge!");

            if (Personal.USER_INFO.status == 3 & this.WindowState != System.Windows.WindowState.Maximized)
            {
                if (this.WindowState == System.Windows.WindowState.Minimized)
                {
                    this.WindowState = System.Windows.WindowState.Normal;
                }

                this.Activate();

                Resource.Sounds.Player.PlaySound(Resource.Sounds.Identifiers.NUDGE);

                Thread threadNudge = new Thread(NudgeWindow);
                threadNudge.Start();
            }
        }

        public void NudgeWindow()
        {
            for (int i = 0; i < 25; i++)
            {
                moveWindow(5, 0);
                Thread.Sleep(10);

                moveWindow(0, 5);
                Thread.Sleep(10);

                moveWindow(-5, 0);
                Thread.Sleep(10);

                moveWindow(0, -5);
                Thread.Sleep(10);
            }
        }

        public void moveWindow(int top, int left)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                this.Top = this.Top + top;
                this.Left = this.Left + left;
            }));
        }

        public void Hyperlink_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            Process.Start(hyperlink.NavigateUri.ToString());
        }
    }
}
