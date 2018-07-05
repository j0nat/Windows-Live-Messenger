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

using WLMClient.UI.Controls;

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;

using WLMClient.UI.Windows;
using WLMData.Enums;
using WLMData.Data.Packets;

using System.Configuration;

namespace WLMClient.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Login pageLogin = null;
        private Main pageMain = null;

        public MainWindow()
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            Config.Properties.SERVER_ADDRESS = ConfigurationManager.AppSettings["server_address"];
            Config.Properties.SERVER_PORT = Convert.ToInt32(ConfigurationManager.AppSettings["server_port"]);
            
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            InitializeComponent();

            Layout.Images.Load();
            Network.Client.Load(this);

            pageLogin = new Login();
            controlPanel.Children.Add(pageLogin);

            this.Icon = new BitmapImage(new Uri(Resource.Images.Identifiers.APP_ICON_STATUS_OFFLINE, UriKind.Absolute));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (pageMain != null)
            {
                pageMain.WindowSizeChanged(this.ActualWidth, this.ActualHeight);
            }

            if (pageLogin != null)
            {
                pageLogin.WindowSizeChanged(this.ActualHeight);
            }
        }

        public void LoginSuccess()
        {
            ClearLogin();

            pageMain = new Main();
            controlPanel.Children.Add(pageMain);

            UpdateLayout();
            pageMain.UpdateLayout();
            pageMain.WindowSizeChanged(this.ActualWidth, this.ActualHeight);
        }

        public void ClearLogin()
        {
            controlPanel.Children.Remove(pageLogin);
            pageLogin = null;
            controlPanel.Children.Clear();
        }

        public void ConnectionClosedLogOut()
        {
            if (pageLogin == null)
            {
                Locale.ManageChatWindows.CloseAllOpenChatWindows();

                pageMain = null;
                controlPanel.Children.Clear();

                pageLogin = new Login();
                controlPanel.Children.Add(pageLogin);

                UpdateLayout();
                pageLogin.UpdateLayout();
                pageLogin.WindowSizeChanged(this.ActualHeight);

                this.Icon = new BitmapImage(new Uri(Resource.Images.Identifiers.APP_ICON_STATUS_OFFLINE, UriKind.Absolute));
            }
            else
            {
                MessageBox.Show("Windows Live Messenger was not able to contact the server. Please check your internet connection.", "Unable to connect to server.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool IsPageMainNull()
        {
            bool returnValue = false;

            if (pageMain == null)
            {
                returnValue = true;
            }

            return returnValue;
        }

        public Main GetMainPage()
        {
            return pageMain;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (pageMain != null)
            {
                this.WindowState = WindowState.Minimized;

                e.Cancel = true;
            }
            else
            {
                Locale.ManageChatWindows.CloseAllOpenChatWindows();

                Environment.Exit(0);
            }
        }
    }
}
