using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nilsa
{
    public partial class FormEnterLogin : Form
    {
        public FormEnterLogin()
        {
            InitializeComponent();
            buttonOk.Enabled = false;
            cbSocialNetwork.SelectedIndex = 3;
        }

        private void tbLogin_TextChanged(object sender, EventArgs e)
        {
            buttonOk.Enabled = tbLogin.Text.Length > 0 && tbPassword.Text.Length > 0;
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            buttonOk.Enabled = tbLogin.Text.Length > 0 && tbPassword.Text.Length > 0;
        }
    }
}
