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
    public partial class FormEditPersHarValues : Form
    {
        FormMain mFormMain;

        public String[,] sPersHar;
        public int iPersHarCount = 3;
        public int iPersHarAttrCount = 4;
        int listBox1PreIdx = -1;
        public String sFilePrefix = "";

        public FormEditPersHarValues(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            tbKeywords.Visible = false;
            lblKeywords.Visible = false;
            cbKeywords.Visible = false;
            button4.Visible = false;
            numericUpDown1.Visible = false;
            cbKeywords.SelectedIndex = 0;
        }

        public void Setup(bool showKeywords=false, bool showCounter=false)
        {
            if (showKeywords)
            {
                lblKeywords.Visible = true;
                lblKeywords.Text = "Ключевые слова";
                tbKeywords.Visible = true;
                cbKeywords.Visible = true;
                button4.Visible = true;
            }
            else if (showCounter)
            {
                lblKeywords.Visible = true;
                lblKeywords.Text = "Количество";
                numericUpDown1.Visible = true;
            }
            listBox1.SelectedIndex = -1;
            listBox1.Items.Clear();
            for (int i = 0; i < iPersHarCount; i++)
                listBox1.Items.Add(sPersHar[i, 1]+" ("+sPersHar[i, iPersHarAttrCount]+")");
            listBox1.SelectedIndex = 0;
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
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_" + sFilePrefix + "_har_" + sPersHar[i, 0] + ".txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_" + sFilePrefix + "_har_" + sPersHar[i, 0] + ".txt"));
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
                sPersHar[i, iPersHarAttrCount] = comboBox1.Text;
                listBox1.Items[i] = sPersHar[i, 1] + " (" + sPersHar[i, iPersHarAttrCount] + ")";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < iPersHarCount; i++)
            {
                if (sPersHar[i, iPersHarAttrCount].Length > 0)
                {
                    List<String> lstValues = new List<String>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_" + sFilePrefix + "_har_" + sPersHar[i, 0] + ".txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_" + sFilePrefix + "_har_" + sPersHar[i, 0] + ".txt"));
                        lstValues = new List<String>(srcFile);
                    }
                    if (!lstValues.Contains(sPersHar[i, iPersHarAttrCount]))
                    {
                        lstValues.Add(sPersHar[i, iPersHarAttrCount]);
                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_" + sFilePrefix + "_har_" + sPersHar[i, 0] + ".txt"), lstValues, Encoding.UTF8);
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
            tbKeywords.Text = "";
            cbKeywords.SelectedIndex = 0;
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

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                comboBox1.Text = "";
                comboBox1_TextUpdate(null, null);
            }
        }
    }
}
