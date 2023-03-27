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
    public partial class FormSelectAlgorithmsList : Form
    {
        List<String> lstAllAlgorithmsList;
        List<String> lstUserEnabledAlgorithmsList;
        FormMain mFormMain;

        public FormSelectAlgorithmsList(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        bool bSetupDone = false;
        public void Setup()
        {
            lstAllAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"));
                lstAllAlgorithmsList = new List<String>(srcFile);
            }

            lstUserEnabledAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_selectedalgorithms.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_selectedalgorithms.txt"));
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

            button1.Enabled = lstUserEnabledAlgorithmsList.Count > 0;
            bSetupDone = true;

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
            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_selectedalgorithms.txt"), lstUserEnabledAlgorithmsList, Encoding.UTF8);
            DialogResult = DialogResult.OK;
        }

        private void clbDBs_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!bSetupDone)
                return;

            bool bCanSelect = false;
            for (int i = 0; i < clbDBs.Items.Count; i++)
            {
                if (i==e.Index)
                {
                    if (e.NewValue == CheckState.Checked)
                    {
                        bCanSelect = true;
                        break;
                    }
                }
                else if (clbDBs.GetItemCheckState(i) == CheckState.Checked)
                {
                    bCanSelect = true;
                    break;
                }
            }

            bCanSelect = bCanSelect || (e.NewValue == CheckState.Checked);

            button1.Enabled = bCanSelect;
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
                    clbDBs.SetItemChecked(i, !_allcheked);
            }
        }
    }
}
