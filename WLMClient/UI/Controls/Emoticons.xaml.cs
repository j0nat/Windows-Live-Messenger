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

namespace WLMClient.UI.Controls
{
    /// <summary>
    /// Interaction logic for Emoticons.xaml
    /// </summary>
    public partial class Emoticons : UserControl
    {
        private RichTextBox richTextBox;

        public Emoticons(RichTextBox richTextBox)
        {
            InitializeComponent();

            this.richTextBox = richTextBox;
            populateList();
        }
        
        public void populateList()
        {
            int positionX = 0;
            int positionY = 0;
            int count = 0;
            
            foreach (string emoticon in Resource.Images.Emoticons.INDEX_IN_IMAGE.Keys)
            {
                if (count > 5)
                {
                    positionY += 35;
                    positionX = 0;
                    count = 0;
                }
                
                CroppedBitmap smiley = Layout.LoadResource.GetEmoticon(emoticon);

                Border border = new Border();
                border.BorderBrush = Brushes.Transparent;
                border.BorderThickness = new Thickness(2);
                border.Width = 25;
                border.Height = 25;
                border.Margin = new Thickness(positionX, positionY, 0, 0);
                border.VerticalAlignment = VerticalAlignment.Top;
                border.HorizontalAlignment = HorizontalAlignment.Left;
                border.ToolTip = emoticon;
                border.MouseEnter += new MouseEventHandler(Br_MouseEnter);
                border.MouseLeave += new MouseEventHandler(Br_MouseLeave);
                border.MouseDown += new MouseButtonEventHandler(Br_MouseDown);

                Image img = new Image();
                img.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                img.Width = 19;
                img.SnapsToDevicePixels = true;
                img.Height = 19;
                img.Source = smiley;

                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(img, EdgeMode.Aliased);

                positionX += 35;

                border.Child = img;

                gridEmoticons.Children.Add(border);

                count++;
            }
        }

        void Br_MouseDown(object sender, MouseButtonEventArgs e)
        {
            InsertText(((Border)sender).ToolTip.ToString(), richTextBox);

            this.Visibility = Visibility.Hidden;

            e.Handled = true;
        }

        void Br_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).BorderBrush = Brushes.Transparent;
        }

        void Br_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#afafaf")); ;
        }

        private void InsertText(String text, RichTextBox rtb)
        {
            rtb.CaretPosition = rtb.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            rtb.CaretPosition.InsertTextInRun(text);
        }
    }
}
