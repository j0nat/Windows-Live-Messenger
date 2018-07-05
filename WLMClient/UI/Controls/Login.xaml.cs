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

using WLMClient.Config;
using WLMData.Enums;
using WLMClient.UI.Data.Enums;

namespace WLMClient.UI.Controls
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        private UserStatus selectedUserStatus;
        private SaveData loginConfiguration;

        public Login()
        {
            InitializeComponent();

            imagePartnerAvatar.Source = Layout.LoadResource.GetDefaultAvatarImage();
            imagePartnerFrame.Source = Layout.LoadResource.GetAvatarFrameFromStatus(UserStatus.Offline, AvatarSize.Big);

            background.Source = new BitmapImage(new Uri(Resource.Images.Identifiers.CHAT_WINDOW_BACKGROUND_WIDE, UriKind.Absolute));
           
            checkAvailable.Source = Layout.LoadResource.GetSmallIconFromStatus(UserStatus.Available);
            checkBusy.Source = Layout.LoadResource.GetSmallIconFromStatus(UserStatus.Busy);
            checkAway.Source = Layout.LoadResource.GetSmallIconFromStatus(UserStatus.Away);
            checkOffline.Source = Layout.LoadResource.GetSmallIconFromStatus(UserStatus.Offline);

            SetStatusSelection(UserStatus.Available);

            loginConfiguration = SaveDataManager.GetConfiguration();

            if (loginConfiguration.rememberId)
            {
                txtId.Text = loginConfiguration.saveId;
                checkRememberMe.IsChecked = true;
            }

            if (loginConfiguration.rememberPassword)
            {
                txtPass.Password = loginConfiguration.savePass;
                checkRememberMyPassword.IsChecked = true;
            }

            if (loginConfiguration.autoLogin)
            {
                btnLogin_Click(null, null);
                checkSignInAutomatically.IsChecked = true;
            }
        }

        private void checkAvailable_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetStatusSelection(UserStatus.Available);
        }

        private void checkBusy_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetStatusSelection(UserStatus.Busy);
        }

        private void checkAway_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetStatusSelection(UserStatus.Away);
        }

        private void checkOffline_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetStatusSelection(UserStatus.Offline);
        }

        private void checkRememberMe_Click(object sender, RoutedEventArgs e)
        {
            bool checkValue = ((CheckBox)sender).IsChecked.Value;

            loginConfiguration.rememberId = checkValue;
        }

        private void checkRememberMyPassword_Click(object sender, RoutedEventArgs e)
        {
            bool checkValue = ((CheckBox)sender).IsChecked.Value;

            loginConfiguration.rememberPassword = checkValue;
        }

        private void checkSignInAutomatically_Click(object sender, RoutedEventArgs e)
        {
            bool checkValue = ((CheckBox)sender).IsChecked.Value;

            loginConfiguration.autoLogin = checkValue;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (checkRememberMe.IsChecked.Value & !checkRememberMyPassword.IsChecked.Value)
            {
                loginConfiguration.saveId = txtId.Text;
                loginConfiguration.savePass = "";
            }

            if (checkRememberMyPassword.IsChecked.Value)
            {
                loginConfiguration.saveId = txtId.Text;
                loginConfiguration.savePass = txtPass.Password;
            }

            SaveDataManager.SaveConfiguration(loginConfiguration);

            Network.Client.Connect();
            Network.Client.AuthenticateUser(txtId.Text, txtPass.Password, Convert.ToInt16(selectedUserStatus));
        }

        private void txtPass_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(null, null);
            }
        }

        public void WindowSizeChanged(double height)
        {
            this.Height = height;
        }

        private void SetStatusSelection(UserStatus status)
        {
            borderAvailable.BorderThickness = new Thickness(0);
            borderBusy.BorderThickness = new Thickness(0);
            borderOffline.BorderThickness = new Thickness(0);
            borderAway.BorderThickness = new Thickness(0);

            if (status == UserStatus.Available)
            {
                borderAvailable.BorderThickness = new Thickness(3);
            }

            if (status == UserStatus.Busy)
            {
                borderBusy.BorderThickness = new Thickness(3);
            }

            if (status == UserStatus.Away)
            {
                borderAway.BorderThickness = new Thickness(3);
            }

            if (status == UserStatus.Offline)
            {
                borderOffline.BorderThickness = new Thickness(3);
            }

            selectedUserStatus = status;
        }
    }
}
