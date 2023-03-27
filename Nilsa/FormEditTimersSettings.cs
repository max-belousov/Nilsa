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
    public partial class FormEditTimersSettings : Form
    {
        FormMain mFormMain;

        public FormEditTimersSettings(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            //NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            //NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            //NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }
    }
}
