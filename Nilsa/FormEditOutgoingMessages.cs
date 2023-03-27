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
    public partial class FormEditOutgoingMessages : Form
    {
        FormMain mFormMain;
        public FormEditOutgoingMessages(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        public void Setup()
        {
            listBox1.Items.Clear();
            for (int i=0; i<mFormMain.lstOutgoingMessages.Count;i++)
            {
                String srec = mFormMain.lstOutgoingMessages[i];
                String suid = "";
                String sname = "";
                String schannel = "";
                if (srec.StartsWith("*#|"))
                {
                    srec = srec.Substring(srec.IndexOf("|") + 1);
                    schannel = srec.Substring(0, srec.IndexOf("|"));
                    srec = srec.Substring(srec.IndexOf("|") + 1);
                    suid = srec.Substring(0, srec.IndexOf("|"));
                    srec = srec.Substring(srec.IndexOf("|") + 1);
                    sname = srec.Substring(0, srec.IndexOf("|"));
                    srec = srec.Substring(srec.IndexOf("|") + 1);
                    if (schannel.Equals("0"))
                    {
                        schannel = " "+ NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "(ЛС)");
                    }
                    else if (schannel.Equals("1"))
                    {
                        schannel = " "+ NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "(Стена)");
                    }
                    else
                    {
                        schannel = " (" + schannel + ")";
                    }
                }
                else
                {
                    suid = srec.Substring(0, srec.IndexOf("|"));
                    srec = srec.Substring(srec.IndexOf("|")+1);
                    sname = srec.Substring(0, srec.IndexOf("|"));
                    srec = srec.Substring(srec.IndexOf("|") + 1);
                }

                listBox1.Items.Add(sname + (schannel.Length > 0 ? schannel : "") + ": " + srec);
            }
            if (mFormMain.lstOutgoingMessages.Count > 0)
                button1.Enabled = true;
            else
            {
                button2.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Закрыть");
                button1.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            mFormMain.lstOutgoingMessages.Clear();
            button1.Enabled = false;
            button3.Enabled = false;
            button2.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Закрыть");
        }
    }
}
