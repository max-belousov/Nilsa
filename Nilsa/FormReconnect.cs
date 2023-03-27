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
    public partial class FormReconnect : Form
    {
        FormMain mFormMain;
        int iCycle = 30;
        bool bCanGoToRotation;
        bool bAutoMode;

        public FormReconnect(FormMain _formmain, bool _bCanGoToRotation, bool _bAutoMode)
        {
            bCanGoToRotation = _bCanGoToRotation;
            bAutoMode = _bAutoMode;

            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            buttonGotoRotation.Text = "Ротация Персонажа";
            buttonTryAgain.Text = "Повторить Авторизацию";

            if (bAutoMode)
            {
                if (bCanGoToRotation)
                    buttonGotoRotation.Text = "Ротация " + " " + iCycle.ToString() + " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "сек.");
                else
                    buttonTryAgain.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Повторить") + " " + iCycle.ToString() + " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "сек.");
            }
            else
                buttonTryAgain.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Повторить") + " " + iCycle.ToString() + " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "сек.");

            buttonGotoRotation.Enabled = _bCanGoToRotation;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            iCycle--;

            if (bAutoMode)
            {
                if (bCanGoToRotation)
                    buttonGotoRotation.Text = "Ротация " + " " + iCycle.ToString() + " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "сек.");
                else
                    buttonTryAgain.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Повторить") + " " + iCycle.ToString() + " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "сек.");
            }
            else
                buttonTryAgain.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Повторить") + " " + iCycle.ToString() + " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "сек.");

            if (iCycle == 0)
            {
                if (bAutoMode)
                {
                    if (bCanGoToRotation)
                        buttonGotoRotation_Click(null, null);
                    else
                        buttonTryAgain_Click(null, null);
                }
                else
                    buttonTryAgain_Click(null, null);
            }
            else
                timer1.Enabled = true;
        }

        private void buttonTryAgain_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            timer1.Enabled = false;
        }

        private void buttonGotoRotation_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
            timer1.Enabled = false;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            timer1.Enabled = false;
        }
    }
}
