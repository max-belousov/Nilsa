using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nilsa
{
    public partial class FormSelectPersoneLogin : Form
    {
        FormMain mFormMain;

        public List<String> lstPersonenList;
        public FormSelectPersoneLogin(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        public void Setup(int iSelUser=0)
        {
            comboBox1.SelectedIndex = -1;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("<"+NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Новый Персонаж")+">");

            lstPersonenList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix() + ".txt"));
                lstPersonenList = new List<String>(srcFile);
            }

            foreach (String str in lstPersonenList)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                String value =str, sUsrName, sUsrID;
                sUsrID=value.Substring(0,value.IndexOf("|"));
                value=value.Substring(value.IndexOf("|")+1);
                sUsrName=value.Substring(0,value.IndexOf("|"));
                comboBox1.Items.Add(sUsrName + " ["+sUsrID+"]");
            }

            if (iSelUser < comboBox1.Items.Count)
                comboBox1.SelectedIndex = iSelUser;
            else
                comboBox1.SelectedIndex = 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
