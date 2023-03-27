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
    public partial class FormEditUserEnabledAlgorithmsList : Form
    {
        private long iUserID;
        private int iSocialNetwork;
        List<String> lstAllAlgorithmsList;
        List<String> lstUserEnabledAlgorithmsList;
        FormMain mFormMain;

        public FormEditUserEnabledAlgorithmsList(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        public void Setup(long iID, int _iSocialNetwork)
        {
            iUserID = iID;
            iSocialNetwork = _iSocialNetwork;
            lstAllAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"));
                lstAllAlgorithmsList = new List<String>(srcFile);
            }

            lstUserEnabledAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + iUserID.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + iUserID.ToString() + ".txt"));
                lstUserEnabledAlgorithmsList = new List<String>(srcFile);
            }
            //else
            //    lstUserEnabledAlgorithmsList.Add("0");

            clbDBs.BeginUpdate();
            clbDBs.Items.Clear();
            foreach (String str in lstAllAlgorithmsList)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                AlgorithmsDBRecord dbr = AlgorithmsDBRecord.FromRecordString(str);
                clbDBs.Items.Add(dbr);
                bool bCheck = false;
                if (lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                    bCheck = true;
                clbDBs.SetItemChecked(clbDBs.Items.Count - 1, bCheck);
            }
            clbDBs.EndUpdate();

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lstUserEnabledAlgorithmsList.Clear();
            for (int i = 0; i < lstAllAlgorithmsList.Count; i++ )
            {
                if (clbDBs.GetItemCheckState(i) == CheckState.Checked)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)clbDBs.Items[i];
                    lstUserEnabledAlgorithmsList.Add(dbr.ID.ToString());
                }
            }
            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + iUserID.ToString() + ".txt"), lstUserEnabledAlgorithmsList, Encoding.UTF8);
            DialogResult = DialogResult.OK;
        }

        private void clbDBs_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            /*
            AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)clbDBs.Items[e.Index];
            if (dbr.Name.ToUpper().Equals("BASIC"))
            {
                if (e.NewValue != CheckState.Checked)
                {
                    clbDBs.SetItemChecked(e.Index, true);
                    e.NewValue = CheckState.Checked;
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Алгоритм BASIC всегда разрешен."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Разрешенные алгоритмы"), MessageBoxButtons.OK);
                }
            }
            */
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool _allcheked = true;
            for (int i = 0; i < clbDBs.Items.Count; i++)
                if (clbDBs.GetItemCheckState(i) != CheckState.Checked)
                {
                    _allcheked = false;
                    break;
                }

            for (int i = 0; i < clbDBs.Items.Count; i++)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)clbDBs.Items[i];
                //if (!dbr.Name.ToUpper().Equals("BASIC"))
                    clbDBs.SetItemChecked(i, !_allcheked);
            }
        }
    }
}
