using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WLMClient.Locale;
using WLMClient.Network;

namespace WLMClient.UI.Controls.WinForms
{
    public partial class FrmAddNewFriend : Form
    {
        public FrmAddNewFriend()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
           if (txtName.Text.Length > 0 && (txtName.Text.Length < 30) &&
                !txtName.Text.Contains(" ") && (txtName.Text.Trim().ToLower() != Personal.USER_INFO.id))
            {
                Client.AddNewContact(txtName.Text);

                this.Close();
            }
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSubmit_Click(null, null);
            }
        }

        private void frmAddNewFriend_Load(object sender, EventArgs e)
        {
            txtName.Focus();
            txtName.Select();
        }
    }
}
