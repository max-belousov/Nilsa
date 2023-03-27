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
    public partial class FormEnterCapthaParameters : Form
    {
        public FormEnterCapthaParameters()
        {
            InitializeComponent();
            tbLogin.Text = NilsaUtils.LoadRuCaptcha("");
            buttonOk.Enabled = true;
        }

        private void tbLogin_TextChanged(object sender, EventArgs e)
        {
            //buttonOk.Enabled = tbLogin.Text.Length > 0;
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            //buttonOk.Enabled = tbLogin.Text.Length > 0;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (tbLogin.Text.Length > 0)
                NilsaUtils.SaveRuCaptcha(tbLogin.Text.Trim());
            else
                NilsaUtils.DeleteRuCaptcha();
        }
    }
}
