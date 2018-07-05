using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;

using WLMClient.Locale;
using WLMClient.Network;

namespace WLMClient.UI.Controls.WinForms
{
    public partial class FrmOptions : Form
    {
        public Bitmap _currentBitmap;
        public string newImage = "";
        string current_Directory = "";

        public FrmOptions()
        {
            InitializeComponent();

            txtName.Text = Personal.USER_INFO.name;
            current_Directory = AppDomain.CurrentDomain.BaseDirectory;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (Config.Properties.AVATAR_IMAGE_UPLOAD_URL == "")
            {
                System.Windows.MessageBox.Show("Changing avatar has been disabled.", "Unable to change avatar.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                return;
            }

            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            DialogResult result = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (result == DialogResult.OK)
            {
                Random rndm = new Random();
                string name = rndm.Next(1000000, 99999999).ToString() + ".png";
                _currentBitmap = new Bitmap(openFileDialog1.FileName);
                imgResize(100, 100);

                _currentBitmap.Save(current_Directory + name);

                try
                {
                    string uploadValue = upload(AppDomain.CurrentDomain.BaseDirectory + name);

                    if (uploadValue == "0")
                    {
                        MessageBox.Show("Failed to upload imagine.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        newImage = uploadValue;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Unable to upload image. " + err.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                System.IO.File.Delete(name);
                imgAvatar.Image = _currentBitmap;
            }
        }

        public static string upload(string file)
        {
            string returnValue = "0";

            string url = Config.Properties.AVATAR_IMAGE_UPLOAD_URL;
            using (var client = new WebClient())
            {
                byte[] result = client.UploadFile(url, "POST", file);
                string responseAsString = Encoding.Default.GetString(result);

                if (responseAsString.Trim() != "0")
                {
                    returnValue = responseAsString;
                }
            }

            return returnValue;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (newImage != "")
            {
                Personal.USER_INFO.avatar = newImage;
            }

            Personal.USER_INFO.name = txtName.Text;

            Client.SendUserUpdate();

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void imgResize(int newWidth, int newHeight)
        {
            if (newWidth != 0 && newHeight != 0)
            {
                Bitmap temp = (Bitmap)_currentBitmap;
                Bitmap bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat);

                double nWidthFactor = (double)temp.Width / (double)newWidth;
                double nHeightFactor = (double)temp.Height / (double)newHeight;

                double fx, fy, nx, ny;
                int cx, cy, fr_x, fr_y;
                Color color1 = new Color();
                Color color2 = new Color();
                Color color3 = new Color();
                Color color4 = new Color();
                byte nRed, nGreen, nBlue;

                byte bp1, bp2;

                for (int x = 0; x < bmap.Width; ++x)
                {
                    for (int y = 0; y < bmap.Height; ++y)
                    {

                        fr_x = (int)Math.Floor(x * nWidthFactor);
                        fr_y = (int)Math.Floor(y * nHeightFactor);
                        cx = fr_x + 1;
                        if (cx >= temp.Width) cx = fr_x;
                        cy = fr_y + 1;
                        if (cy >= temp.Height) cy = fr_y;
                        fx = x * nWidthFactor - fr_x;
                        fy = y * nHeightFactor - fr_y;
                        nx = 1.0 - fx;
                        ny = 1.0 - fy;

                        color1 = temp.GetPixel(fr_x, fr_y);
                        color2 = temp.GetPixel(cx, fr_y);
                        color3 = temp.GetPixel(fr_x, cy);
                        color4 = temp.GetPixel(cx, cy);

                        // Blue
                        bp1 = (byte)(nx * color1.B + fx * color2.B);

                        bp2 = (byte)(nx * color3.B + fx * color4.B);

                        nBlue = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Green
                        bp1 = (byte)(nx * color1.G + fx * color2.G);

                        bp2 = (byte)(nx * color3.G + fx * color4.G);

                        nGreen = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Red
                        bp1 = (byte)(nx * color1.R + fx * color2.R);

                        bp2 = (byte)(nx * color3.R + fx * color4.R);

                        nRed = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        bmap.SetPixel(x, y, System.Drawing.Color.FromArgb(255, nRed, nGreen, nBlue));
                    }
                }
                _currentBitmap = (Bitmap)bmap.Clone();
            }
        }
    }
}
