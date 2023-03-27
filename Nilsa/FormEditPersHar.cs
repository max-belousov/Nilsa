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
    public partial class FormEditPersHar : Form
    {
        private FormMain mFormMain;
        public String[,] sPersHar;
        public int iPersHarCount = 3;
        public int iPersHarAttrCount = 4;
        Boolean bReorderEnable, bShowReorderButtons;
        private int HarEditMode = 0;

        public FormEditPersHar(FormMain formmain)
        {
            bShowReorderButtons = false;
            mFormMain = formmain;
            bReorderEnable = true;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        public void Setup(Boolean bReorder, int _HarEditMode)
        {
            HarEditMode = _HarEditMode;
            bReorderEnable = bReorder;
            if (!bReorderEnable)
            {
                button4.Enabled = false;
                button3.Enabled = false;
                button4.Visible = false;
                button3.Visible = false;
                listBox1.Location = new System.Drawing.Point(11, 23);
                listBox1.Size = new System.Drawing.Size(211, 212);
                comboBox2.Visible = true;
                textBox3.Visible = false;
                label4.Visible = true;
                button5.Visible = true;

                label3.Visible = true;
                label2.Visible = true;
                comboBox1.Visible = true;
                comboBox3.Items.Clear();
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    comboBox3.Items.Add(mFormMain.sContHar[i, 1]);
                }
                comboBox4.Items.Clear();
                for (int i = 0; i < mFormMain.iPersHarCount; i++)
                {
                    comboBox4.Items.Add(mFormMain.sPersHar[i, 1]);
                }
            }
            else
            {
                if (bShowReorderButtons)
                {
                    button4.Visible = true;
                    button3.Visible = true;
                    listBox1.Location = new System.Drawing.Point(55, 23);
                    listBox1.Size = new System.Drawing.Size(167, 212);
                }
                else
                {
                    button4.Enabled = false;
                    button3.Enabled = false;
                    button4.Visible = false;
                    button3.Visible = false;
                    listBox1.Location = new System.Drawing.Point(11, 23);
                    listBox1.Size = new System.Drawing.Size(211, 212);
                }

                comboBox2.Visible = false;
                textBox3.Visible = false;
                label4.Visible = false;
                button5.Visible = true;

                label2.Visible = false;
                label3.Visible = false;
                comboBox1.Visible = false;
            }

            if (HarEditMode == 3)
                pictureBoxIcon.Visible = false;

            comboBox2.Items.Clear();
            foreach (var vstr in FormMain.dictMsgKoefStrings)
                comboBox2.Items.Add(vstr.Key);

            listBox1.Items.Clear();
            for (int i = 0; i < iPersHarCount; i++)
                listBox1.Items.Add(sPersHar[i, 1]);
            listBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void LoadPicture()
        {
            if (HarEditMode == 3)
                return;

            if (listBox1.SelectedIndex >= 0)
            {
                String spref = "_msg_har_";
                if (HarEditMode == 0)
                    spref = "_pers_har_";
                else if (HarEditMode == 1)
                    spref = "_cont_har_";
                else if (HarEditMode == 3)
                    spref = "_grp_har_";
                string _picfile = Path.Combine(Application.StartupPath, "Images\\" + spref + listBox1.SelectedIndex.ToString() + ".png");
                if (File.Exists(_picfile))
                {
                    FileStream stream = new FileStream(_picfile, FileMode.Open, FileAccess.Read);
                    pictureBoxIcon.Image = Image.FromStream(stream);
                    stream.Close();
                }
                else
                    pictureBoxIcon.Image = Nilsa.Properties.Resources.labelbg;
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                LoadPicture();
                int i = listBox1.SelectedIndex;
                textBox1.Text = sPersHar[i, 1];
                if (sPersHar[i, 2].IndexOf("*") > 0)
                {
                    comboBox1.SelectedIndex = 2;
                }
                else if (sPersHar[i, 2].IndexOf("#") > 0)
                {
                    comboBox1.SelectedIndex = 3;
                }
                else
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(sPersHar[i, 2]);
                }

                if ((sPersHar[i, 2].IndexOf("*") > 0) || (sPersHar[i, 2].IndexOf("#") > 0) || (comboBox1.SelectedIndex == 0))
                    comboBox1_SelectedIndexChanged(sender, e);

                if (!bReorderEnable)
                {
                    if (listBox1.SelectedIndex > 0) comboBox1.Enabled = true; else comboBox1.Enabled = false;

                    if (comboBox2.Items.Contains(sPersHar[i, 3]))
                        comboBox2.SelectedIndex = comboBox2.Items.IndexOf(sPersHar[i, 3]);
                    else
                        comboBox2.SelectedIndex = 1;
                }
                else textBox3.Text = sPersHar[i, 3];

                if (bReorderEnable && bShowReorderButtons && listBox1.SelectedIndex < iPersHarCount - 1) button4.Enabled = true; else button4.Enabled = false;
                if (bReorderEnable && bShowReorderButtons && listBox1.SelectedIndex > 0) button3.Enabled = true; else button3.Enabled = false;

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                sPersHar[listBox1.SelectedIndex, 1] = textBox1.Text;
                listBox1.Items[listBox1.SelectedIndex] = textBox1.Text;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                sPersHar[listBox1.SelectedIndex, 3] = textBox3.Text;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((listBox1.SelectedIndex >= 0) && (comboBox1.SelectedIndex >= 0))
            {
                if (comboBox1.SelectedIndex < 2)
                {
                    sPersHar[listBox1.SelectedIndex, 2] = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                    comboBox3.Visible = false;
                    comboBox4.Visible = false;
                    if (HarEditMode == 2)
                    {
                        if (listBox1.SelectedIndex > 0) button5.Enabled = true; else button5.Enabled = false;
                    }
                    else
                        button5.Enabled = true;
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    comboBox3.Visible = true;
                    comboBox4.Visible = false;
                    button5.Enabled = false;
                    int i = listBox1.SelectedIndex;
                    if (sPersHar[i, 2].IndexOf("*") <= 0)
                    {
                        sPersHar[i, 2] = comboBox1.Items[comboBox1.SelectedIndex].ToString() + "*1";
                    }

                    if (sPersHar[i, 2].IndexOf("*") > 0)
                    {
                        String s = sPersHar[i, 2].Substring(sPersHar[i, 2].IndexOf("*") + 1);
                        for (int j = 0; j < mFormMain.iContHarCount; j++)
                        {
                            if (mFormMain.sContHar[j, 0].Equals(s))
                            {
                                comboBox3.SelectedIndex = j;
                                break;
                            }
                        }

                    }

                }
                else if (comboBox1.SelectedIndex == 3)
                {
                    comboBox4.Visible = true;
                    comboBox3.Visible = false;
                    button5.Enabled = false;
                    int i = listBox1.SelectedIndex;
                    if (sPersHar[i, 2].IndexOf("#") <= 0)
                    {
                        sPersHar[i, 2] = comboBox1.Items[comboBox1.SelectedIndex].ToString() + "#1";
                    }

                    if (sPersHar[i, 2].IndexOf("#") > 0)
                    {
                        String s = sPersHar[i, 2].Substring(sPersHar[i, 2].IndexOf("#") + 1);
                        for (int j = 0; j < mFormMain.iPersHarCount; j++)
                        {
                            if (mFormMain.sPersHar[j, 0].Equals(s))
                            {
                                comboBox4.SelectedIndex = j;
                                break;
                            }
                        }

                    }

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 0)
            {
                int i = listBox1.SelectedIndex;
                int j = i - 1;
                String[] s = new String[iPersHarAttrCount];
                for (int jj = 0; jj < iPersHarAttrCount; jj++)
                    s[jj] = sPersHar[i, jj];
                for (int jj = 0; jj < iPersHarAttrCount; jj++)
                    sPersHar[i, jj] = sPersHar[j, jj];
                for (int jj = 0; jj < iPersHarAttrCount; jj++)
                    sPersHar[j, jj] = s[jj];
                listBox1.Items[i] = sPersHar[i, 1];
                listBox1.Items[j] = sPersHar[j, 1];
                listBox1.SelectedIndex = j;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < iPersHarCount - 1)
            {
                int i = listBox1.SelectedIndex;
                int j = i + 1;
                String[] s = new String[iPersHarAttrCount];
                for (int jj = 0; jj < iPersHarAttrCount; jj++)
                    s[jj] = sPersHar[i, jj];
                for (int jj = 0; jj < iPersHarAttrCount; jj++)
                    sPersHar[i, jj] = sPersHar[j, jj];
                for (int jj = 0; jj < iPersHarAttrCount; jj++)
                    sPersHar[j, jj] = s[jj];
                listBox1.Items[i] = sPersHar[i, 1];
                listBox1.Items[j] = sPersHar[j, 1];
                listBox1.SelectedIndex = j;
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((listBox1.SelectedIndex >= 0) && (comboBox2.SelectedIndex >= 0))
            {
                sPersHar[listBox1.SelectedIndex, 3] = comboBox2.Items[comboBox2.SelectedIndex].ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                FormEditMsgHarDefalutValues fe = new FormEditMsgHarDefalutValues(mFormMain);
                fe.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Список значений характеристики")+": " + textBox1.Text;
                fe.groupBox1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Список значений характеристики");
                fe.textBox1.Text = "";
                String spref = "_msg_har_";
                if (HarEditMode == 0)
                    spref = "_pers_har_";
                else if (HarEditMode == 1)
                    spref = "_cont_har_";
                else if (HarEditMode == 3)
                    spref = "_grp_har_";

                if (File.Exists(Path.Combine(FormMain.sDataPath, spref + sPersHar[listBox1.SelectedIndex, 0] + ".txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, spref + sPersHar[listBox1.SelectedIndex, 0] + ".txt"));
                    fe.textBox1.Lines = srcFile;
                    fe.textBox1.SelectionStart = 0;
                    fe.textBox1.SelectionLength = 0;
                }

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    if (fe.textBox1.Text.Length > 0)
                    {
                        List<String> lstValues = new List<String>();
                        foreach (String str in fe.textBox1.Lines)
                        {
                            if (!lstValues.Contains(str))
                                lstValues.Add(str);
                        }

                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, spref + sPersHar[listBox1.SelectedIndex, 0] + ".txt"), lstValues, Encoding.UTF8);
                    }
                    else
                        File.Delete(Path.Combine(FormMain.sDataPath, spref + sPersHar[listBox1.SelectedIndex, 0] + ".txt"));
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((listBox1.SelectedIndex >= 0) && (comboBox3.SelectedIndex >= 0))
            {
                sPersHar[listBox1.SelectedIndex, 2] = comboBox1.Items[comboBox1.SelectedIndex].ToString() + "*" + mFormMain.sContHar[comboBox3.SelectedIndex, 0];
            }
        }

        private void pictureBoxIcon_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                int idx = listBox1.SelectedIndex;
                openFileDialog.Filter = "PNG-files|*.png|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (File.Exists(openFileDialog.FileName))
                    {
                        String spref = "_msg_har_";
                        if (HarEditMode == 0)
                            spref = "_pers_har_";
                        else if (HarEditMode == 1)
                            spref = "_cont_har_";
                        else if (HarEditMode == 3)
                            spref = "_grp_har_";

                        string _picfile = Path.Combine(Application.StartupPath, "Images\\" + spref + listBox1.SelectedIndex.ToString() + ".png");
                        try
                        {
                            File.Copy(openFileDialog.FileName, _picfile, true);
                        }
                        catch
                        {

                        }
                        LoadPicture();
                    }
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((listBox1.SelectedIndex >= 0) && (comboBox4.SelectedIndex >= 0))
            {
                sPersHar[listBox1.SelectedIndex, 2] = comboBox1.Items[comboBox1.SelectedIndex].ToString() + "#" + mFormMain.sPersHar[comboBox4.SelectedIndex, 0];
            }
        }
    }
}
