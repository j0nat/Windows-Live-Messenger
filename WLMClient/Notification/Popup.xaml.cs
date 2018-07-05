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

using WLMClient.UI.Data;
using WLMClient.UI.Windows;
using System.Windows.Threading;

namespace WLMClient.Notification
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        ChatWindow window = null;

        public Popup(string Line1, string Line2, ChatWindow Window)
        {
            InitializeComponent();

            line1.Text = Line1;
            line2.Text = Line2;
            window = Window;
            
            TextParser.ParseText(line1, false);
            TextParser.ParseText(line2, false);

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var workingArea = System.Windows.SystemParameters.WorkArea;
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

                this.Left = corner.X - this.ActualWidth - 5;
                this.Top = corner.Y - this.ActualHeight - 5;
            }));
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (window != null)
            {
                window.Show();
                window.Activate();

                window.WindowState = System.Windows.WindowState.Normal;

                this.Close();
            }
            else
            {
                this.Close();
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Border_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
