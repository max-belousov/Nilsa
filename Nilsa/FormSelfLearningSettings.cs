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
    public partial class FormSelfLearningSettings : Form
    {
        public String[,] sPersHar;
        public int iPersHarCount = 3;
        public int iPersHarAttrCount = 4;
        int listBox1PreIdx = -1;
        FormMain mFormMain;

        public FormSelfLearningSettings(FormMain pf)
        {
            mFormMain = pf;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        public void Setup()
        {
            listBox1.SelectedIndex = -1;
            listBox1.Items.Clear();
            for (int i = 0; i < iPersHarCount; i++)
                listBox1.Items.Add(sPersHar[i, 1]+" ("+sPersHar[i, iPersHarAttrCount]+")");
            listBox1.SelectedIndex = 0;
            //if (textBox1.Text.Length > 0) button1.Enabled = true; else button1.Enabled = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                int i = listBox1.SelectedIndex;
                if (i != listBox1PreIdx)
                {
                    listBox1PreIdx = i;
                    comboBox1.Items.Clear();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(i, sPersHar[0, iPersHarAttrCount]) +sPersHar[i, 0] + ".txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(i, sPersHar[0, iPersHarAttrCount]) + sPersHar[i, 0] + ".txt"));
                        foreach (var line in srcFile)
                            comboBox1.Items.Add(line);
                    }
                    comboBox1.Text = sPersHar[i, iPersHarAttrCount];
                }
            }
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                int i =listBox1.SelectedIndex;
                string sval = comboBox1.Text;
                string svallow = sval.ToLower();
                bool bChangeReg = false;
                for (int j=0;j< comboBox1.Items.Count;j++)
                {
                    if (comboBox1.Items[j].ToString().ToLower().Equals(svallow))
                    {
                        if (!comboBox1.Items[j].ToString().Equals(sval))
                        {
                            comboBox1.Items[j] = sval;
                            bChangeReg = true;
                        }
                        break;
                    }
                }
                if (bChangeReg)
                {
                    List<String> lstValues = new List<String>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(i, sPersHar[0, iPersHarAttrCount]) + sPersHar[i, 0] + ".txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(i, sPersHar[0, iPersHarAttrCount]) + sPersHar[i, 0] + ".txt"));
                        lstValues = new List<String>(srcFile);
                    }
                    for (int j = 0; j < lstValues.Count; j++)
                    {
                        if (lstValues[j].ToLower().Equals(svallow))
                        {
                            if (!lstValues[j].Equals(sval))
                            {
                                lstValues[j] = sval;
                            }
                            break;
                        }
                    }
                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(i, sPersHar[0, iPersHarAttrCount]) + sPersHar[i, 0] + ".txt"), lstValues, Encoding.UTF8);

                    lstValues = new List<String>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + sPersHar[i, 0] + ".txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + sPersHar[i, 0] + ".txt"));
                        lstValues = new List<String>(srcFile);
                    }
                    for (int j = 0; j < lstValues.Count; j++)
                    {
                        if (lstValues[j].ToLower().Equals(svallow))
                        {
                            if (!lstValues[j].Equals(sval))
                            {
                                lstValues[j] = sval;
                            }
                            break;
                        }
                    }
                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + sPersHar[i, 0] + ".txt"), lstValues, Encoding.UTF8);
                }
                sPersHar[i, iPersHarAttrCount] = sval;
                listBox1.Items[i] = sPersHar[i, 1] + " (" + sPersHar[i, iPersHarAttrCount] + ")";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //if (textBox1.Text.Length > 0) button1.Enabled = true; else button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sPersHar[0, iPersHarAttrCount].Trim().Length == 0)
            {
                MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Вы не задали тематику в первой характеристике. Пожалуйста, задайте тематику."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Ошибка задания характеристик Сообщения"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (mFormMain.lstMessagesDB.ContainsKey(sPersHar[0, iPersHarAttrCount].Trim()))
            {
                int iDBID = mFormMain.lstMessagesDB[sPersHar[0, iPersHarAttrCount].Trim()];
                if (mFormMain.lstMessagesDBUserList.Contains(iDBID.ToString()))
                {
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Указанная тематика Вам недоступна. Пожалуйста, подключите эту тематику или задайте другую тематику в первой характеристике."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Ошибка задания характеристик Сообщения"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            for (int i=0; i<iPersHarCount; i++)
            {
                if (sPersHar[i, iPersHarAttrCount].Length > 0)
                {
                    List<String> lstValues=new List<String>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(i, sPersHar[0, iPersHarAttrCount]) + sPersHar[i, 0] + ".txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(i, sPersHar[0, iPersHarAttrCount]) + sPersHar[i, 0] + ".txt"));
                        lstValues = new List<String>(srcFile);
                    }
                    if (!lstValues.Contains(sPersHar[i, iPersHarAttrCount]))
                    {
                        lstValues.Add(sPersHar[i, iPersHarAttrCount]);
                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(i, sPersHar[0, iPersHarAttrCount]) + sPersHar[i, 0] + ".txt"), lstValues, Encoding.UTF8);
                    }

                    lstValues = new List<String>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + sPersHar[i, 0] + ".txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + sPersHar[i, 0] + ".txt"));
                        lstValues = new List<String>(srcFile);
                    }
                    if (!lstValues.Contains(sPersHar[i, iPersHarAttrCount]))
                    {
                        lstValues.Add(sPersHar[i, iPersHarAttrCount]);
                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + sPersHar[i, 0] + ".txt"), lstValues, Encoding.UTF8);
                    }
                }
            }
            DialogResult = DialogResult.OK;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
                comboBox1_TextUpdate(null, null);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < iPersHarCount; i++)
            {
                sPersHar[i, iPersHarAttrCount] = "";
                listBox1.Items[i] = sPersHar[i, 1] + " (" + sPersHar[i, iPersHarAttrCount] + ")";
            }
            comboBox1.Text = "";
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            if (!comboBox1.DroppedDown)
                comboBox1.DroppedDown = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                comboBox1.Text = "";
                comboBox1_TextUpdate(null, null);
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            int index = e.Index;
            if (index >= 0 && index < listBox1.Items.Count)
            {
                String value = listBox1.Items[index].ToString();
                bool hasValue = sPersHar[index, iPersHarAttrCount].Length > 0;

                Graphics g = e.Graphics;

                TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine;

                //background:
                SolidBrush backgroundBrush;
                Color foreColor; 
                if (selected)
                {
                    backgroundBrush = NilsaUtils.brushListBoxSelected;
                    foreColor = NilsaUtils.foreColorListBoxSelected;
                }
                else
                {
                    backgroundBrush = hasValue ? NilsaUtils.brushListBoxHasValue : NilsaUtils.brushListBoxEmptyValue;
                    foreColor = hasValue ? NilsaUtils.foreColorListBoxHasValue : NilsaUtils.foreColorListBoxEmptyValue;
                }

                g.FillRectangle(backgroundBrush, new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height));

                

                //flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine;
                TextRenderer.DrawText(e.Graphics, value, e.Font, new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height), foreColor, flags);

            }

            e.DrawFocusRectangle();
        }

        private void checkBoxSelfLearning_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxUseTeachinhMessageType_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
