using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Nilsa.SelfLearning
{
    public partial class FormMainSelfLearning : Form
    {
        FormMain mFormMain;
		FormEditAlgorithms formEditAlgorithms;
        SelfLearningSettingsManager settingsManager;
        private bool _referenceBase;
        private bool _allBase;
        private bool _noBase;
        public FormMainSelfLearning(FormMain pf, FormEditAlgorithms formEditAlgorithms, AlgorithmsDBRecord dbr)
        {
            mFormMain = pf;
            InitializeComponent();
            this.formEditAlgorithms = formEditAlgorithms;
            settingsManager = new SelfLearningSettingsManager(dbr.ID);
            _referenceBase = settingsManager.ReferenceBase;
            checkBox1.Checked = settingsManager.ReferenceBase;
            _allBase = settingsManager.AllBase;
            checkBox2.Checked = settingsManager.AllBase;
            _noBase = settingsManager.NoBase;
            checkBox3.Checked = settingsManager.NoBase;
        }

        private void OpenFormSelfLearningSettings(string title, string filePath, int algorithmId)
        {
            var fe = new FormSelfLearningSettings(mFormMain);
            fe.Text = title;
            fe.sPersHar = new String[mFormMain.iMsgHarCountNonStatic, mFormMain.iMsgHarAttrCount + 1];

            for (int i = 0; i < mFormMain.iMsgHarCountNonStatic; i++)
            {
                for (int j = 0; j < mFormMain.iMsgHarAttrCount; j++)
                {
                    fe.sPersHar[i, j] = mFormMain.sMsgHar[i, j];
                }
                fe.sPersHar[i, mFormMain.iMsgHarAttrCount] = "";
            }

            var lstMsgHarAlgValues = new List<String>();

            if (File.Exists(filePath))
            {
                var srcFile = File.ReadAllLines(filePath);
                lstMsgHarAlgValues = new List<String>(srcFile);
            }
            else
            {
                for (int i = 0; i < mFormMain.iMsgHarCountNonStatic; i++)
                {
                    lstMsgHarAlgValues.Add("");
                }
            }

            if (lstMsgHarAlgValues.Count > 0)
            {
                for (int i = 0; i < mFormMain.iMsgHarCountNonStatic; i++)
                {
                    fe.sPersHar[i, mFormMain.iMsgHarAttrCount] = lstMsgHarAlgValues[i];
                }
            }

            fe.iPersHarAttrCount = mFormMain.iMsgHarAttrCount;
            fe.iPersHarCount = mFormMain.iMsgHarCountNonStatic;
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                lstMsgHarAlgValues = new List<String>();

                for (int i = 0; i < mFormMain.iMsgHarCountNonStatic; i++)
                {
                    lstMsgHarAlgValues.Add(fe.sPersHar[i, mFormMain.iMsgHarAttrCount]);
                }

                if (lstMsgHarAlgValues.Count > 0)
                {
                    File.WriteAllLines(filePath, lstMsgHarAlgValues, Encoding.UTF8);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                int isel = formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex;
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)formEditAlgorithms.comboBoxAlgorithmsItems.Items[isel];
                OpenFormSelfLearningSettings(button.Text, Path.Combine(FormMain.sDataPath, $"_msg_selflearning_har_etalonBase{Convert.ToString(dbr.ID)}.values"), dbr.ID);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                int isel = formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex;
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)formEditAlgorithms.comboBoxAlgorithmsItems.Items[isel];
                OpenFormSelfLearningSettings(button.Text, Path.Combine(FormMain.sDataPath, $"_msg_selflearning_har_allBase{Convert.ToString(dbr.ID)}.values"), dbr.ID);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                int isel = formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex;
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)formEditAlgorithms.comboBoxAlgorithmsItems.Items[isel];
                OpenFormSelfLearningSettings(button.Text, Path.Combine(FormMain.sDataPath, $"_msg_selflearning_har_noBase{Convert.ToString(dbr.ID)}.values"), dbr.ID);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            settingsManager.UpdateSettings(_referenceBase, _allBase, _noBase);
            DialogResult = DialogResult.OK;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _referenceBase = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _allBase = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _noBase = checkBox3.Checked;
        }
    }
}
