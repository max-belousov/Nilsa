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
    public partial class FormEditThemesKoef : Form
    {
        FormMain mFormMain;

        public String[] stringThemesList;
        public int[] intThemesKoefList;
        public int nListCount = 3;

        int listBox1PreIdx = -1;
        public String sFilePrefix = "";

        public FormEditThemesKoef(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        public void Setup()
        {
            listBox1.SelectedIndex = -1;
            listBox1.Items.Clear();
            for (int i = 0; i < nListCount; i++)
            {
                setListBoxItemText(i, true);
                if (!comboBox1.Items.Contains(intThemesKoefList[i].ToString()))
                    comboBox1.Items.Add(intThemesKoefList[i].ToString());
            }

            listBox1.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                int i = listBox1.SelectedIndex;
                comboBox1.Text = intThemesKoefList[i].ToString();
                if (!comboBox1.Items.Contains(intThemesKoefList[i].ToString()))
                    comboBox1.Items.Add(intThemesKoefList[i].ToString());
            }
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                int i =listBox1.SelectedIndex;
                try
                {
                    intThemesKoefList[i] = Convert.ToInt32(comboBox1.Text);
                    if (intThemesKoefList[i] < 0)
                        intThemesKoefList[i] = 0;
                    else if (intThemesKoefList[i] > 1000)
                        intThemesKoefList[i] = 1000;
                }
                catch
                {
                    intThemesKoefList[i] = 0;
                }
                comboBox1.SelectionStart = comboBox1.Text.Length;
                comboBox1.SelectionLength = 0;
                setListBoxItemText(i);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
                comboBox1_TextUpdate(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < nListCount; i++)
            {
                intThemesKoefList[i] = 0;
                setListBoxItemText(i);
            }
            comboBox1.Text = "";
        }

        private void setListBoxItemText(int i, bool add=false)
        {
            if (add)
                listBox1.Items.Add(stringThemesList[i] + (intThemesKoefList[i] > 0 ? (" (" + intThemesKoefList[i].ToString() + ")") : ""));
            else
                listBox1.Items[i] = stringThemesList[i] + (intThemesKoefList[i] > 0 ? (" (" + intThemesKoefList[i].ToString() + ")") : "");
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            if (!comboBox1.DroppedDown)
                comboBox1.DroppedDown = true;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            int index = e.Index;
            if (index >= 0 && index < listBox1.Items.Count)
            {
                String value = listBox1.Items[index].ToString();
                bool hasValue = intThemesKoefList[index] > 0;

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
    }
}
