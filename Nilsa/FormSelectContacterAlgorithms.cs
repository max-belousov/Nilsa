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
    public partial class FormSelectContacterAlgorithms : Form
    {
        private long iUserID;
        List<String> lstAllAlgorithmsList;
        List<String> lstUserEnabledAlgorithmsList;
        FormMain mFormMain;
        public AlgorithmsDBRecord SelectedAlgorithm=null;

        public FormSelectContacterAlgorithms(FormMain formmain)
        {
            mFormMain = formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        public void Setup(long iID)
        {
            iUserID = iID;
            lstAllAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"));
                lstAllAlgorithmsList = new List<String>(srcFile);
            }

            lstUserEnabledAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + iUserID.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + iUserID.ToString() + ".txt"));
                lstUserEnabledAlgorithmsList = new List<String>(srcFile);
            }
            //else
            //    lstUserEnabledAlgorithmsList.Add("0");

            button1.Enabled = false;
            clbDBs.BeginUpdate();
            clbDBs.Items.Clear();
            foreach (String str in lstAllAlgorithmsList)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                AlgorithmsDBRecord dbr = AlgorithmsDBRecord.FromRecordString(str);
                if (lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                    clbDBs.Items.Add(dbr);
            }
            clbDBs.EndUpdate();
            if (clbDBs.Items.Count > 0)
                clbDBs.SelectedIndex = 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (clbDBs.SelectedIndex >= 0)
            {
                SelectedAlgorithm = (AlgorithmsDBRecord)clbDBs.Items[clbDBs.SelectedIndex];
                DialogResult = DialogResult.OK;
            }
        }

        private void clbDBs_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = clbDBs.SelectedIndex >= 0;
        }
    }
}
