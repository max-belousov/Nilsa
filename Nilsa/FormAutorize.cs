using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nilsa
{
    public partial class FormAutorize : Form
    {
        FormMain mFormMain;
        public FormAutorize(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
            ChangeButtonStatus();
        }

        private void bAutorize_Click(object sender, EventArgs e)
        {
            
        }

        private void ChangeButtonStatus()
        {
            bAutorize.Enabled = ((tbLogin.Text.Trim().Length > 0) && (tbPwd.Text.Trim().Length > 0));
        }

        private void tbLogin_TextChanged(object sender, EventArgs e)
        {
            ChangeButtonStatus();
        }

        private void tbPwd_TextChanged(object sender, EventArgs e)
        {
            ChangeButtonStatus();
        }
    }
}
