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
    public partial class FormEditAlgorithmsVectors : Form
    {
        FormMain mFormMain;
        FormEditAlgorithms _formEditAlgorithms;
        AlgorithmsDBRecord algorithmsDBRecord;

        ComboBox[] cbMHC;
        ComboBox[] cbMHT;
        ComboBox[] cbIn;
        ComboBox[] cbOut;
        CheckBox[] cbReplace;
        Dictionary<String, String>[] dictPairs;

        public FormEditAlgorithmsVectors(FormMain _formmain, FormEditAlgorithms formEditAlgorithms)
        {
            mFormMain = _formmain;
            _formEditAlgorithms = formEditAlgorithms;
            algorithmsDBRecord = (AlgorithmsDBRecord)_formEditAlgorithms.comboBoxAlgorithmsItems.Items[_formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex];

            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            cbReplace = new CheckBox[FormMain.iMsgHarCount];
            cbMHC = new ComboBox[FormMain.iMsgHarCount];
            cbMHT = new ComboBox[FormMain.iMsgHarCount];
            cbIn = new ComboBox[FormMain.iMsgHarCount];
            cbOut = new ComboBox[FormMain.iMsgHarCount];
            dictPairs = new Dictionary<string, string>[FormMain.iMsgHarCount];

            for (int i = 0; i < FormMain.iMsgHarCount; i++)
                dictPairs[i] = new Dictionary<string, string>();

            this.SuspendLayout();
            Width = mFormMain.Width - 20;
            tableLayoutPanel1.SuspendLayout();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Label lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lbl.Location = new System.Drawing.Point(600, 22);
                    lbl.Name = "lbl_har_" + (i * 4 + j + 1).ToString();
                    lbl.Size = new System.Drawing.Size(11, 13);
                    lbl.TabIndex = 2;
                    lbl.Text = mFormMain.sMsgHar[i * 4 + j, 1];// +" (" + mFormMain.sMsgHar[i * 4 + j, 2] + ")";
                    lbl.Dock = System.Windows.Forms.DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.TopCenter;
                    lbl.BackColor = SystemColors.InactiveCaption;
                    lbl.ForeColor = SystemColors.InactiveCaptionText;
                    lbl.Tag = (Int32)(i * 4 + j);
                    lbl.Cursor = Cursors.Hand;

                    tableLayoutPanel1.Controls.Add(lbl, j * 2, i * 6);
                    tableLayoutPanel1.SetColumnSpan(lbl, 2);

                    cbMHT[i * 4 + j] = new ComboBox();
                    cbMHT[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbMHT[i * 4 + j].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                    cbMHT[i * 4 + j].FormattingEnabled = true;
                    cbMHT[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbMHT[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbMHT[i * 4 + j].Name = "cb_koef_" + (i * 4 + j + 1).ToString();
                    cbMHT[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbMHT[i * 4 + j].TabIndex = 0;
                    cbMHT[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbMHT[i * 4 + j].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Строка"));
                    cbMHT[i * 4 + j].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Число"));
                    cbMHT[i * 4 + j].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Соответствие"));
                    cbMHT[i * 4 + j].SelectedIndexChanged += cbMHT_SelectedIndexChanged;
                    tableLayoutPanel1.Controls.Add(cbMHT[i * 4 + j], j * 2, i * 6 + 1);
                    //tableLayoutPanel1.SetColumnSpan(cbMHT[i * 4 + j], 2);

                    cbReplace[i * 4 + j] = new CheckBox();
                    cbReplace[i * 4 + j].Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Подставлять");
                    cbReplace[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbReplace[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbReplace[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbReplace[i * 4 + j].Name = "cb_replace_" + (i * 4 + j + 1).ToString();
                    cbReplace[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbReplace[i * 4 + j].TabIndex = 0;
                    cbReplace[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbReplace[i * 4 + j].CheckedChanged += cbReplace_CheckedChanged;
                    tableLayoutPanel1.Controls.Add(cbReplace[i * 4 + j], j * 2 + 1, i * 6 + 1);
                    //tableLayoutPanel1.SetColumnSpan(cbReplace[i * 4 + j], 2);


                    lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lbl.Location = new System.Drawing.Point(600, 22);
                    lbl.Name = "lbl_in_" + (i * 4 + j + 1).ToString();
                    lbl.Size = new System.Drawing.Size(11, 13);
                    lbl.TabIndex = 2;
                    lbl.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Входящее");
                    lbl.Dock = System.Windows.Forms.DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.TopCenter;
                    tableLayoutPanel1.Controls.Add(lbl, j * 2, i * 6 + 2);

                    lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lbl.Location = new System.Drawing.Point(600, 22);
                    lbl.Name = "lbl_out_" + (i * 4 + j + 1).ToString();
                    lbl.Size = new System.Drawing.Size(11, 13);
                    lbl.TabIndex = 2;
                    lbl.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "Исходящее");
                    lbl.Dock = System.Windows.Forms.DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.TopCenter;
                    tableLayoutPanel1.Controls.Add(lbl, j * 2 + 1, i * 6 + 2);

                    cbIn[i * 4 + j] = new ComboBox();
                    cbIn[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbIn[i * 4 + j].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                    cbIn[i * 4 + j].FormattingEnabled = true;
                    cbIn[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbIn[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbIn[i * 4 + j].Name = "cb_in_" + (i * 4 + j + 1).ToString();
                    cbIn[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbIn[i * 4 + j].TabIndex = 17;
                    cbIn[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbIn[i * 4 + j].SelectedIndexChanged += cbIn_SelectedIndexChanged;
                    cbIn[i * 4 + j].DrawMode = DrawMode.OwnerDrawFixed;
                    cbIn[i * 4 + j].DrawItem += cbIn_DrawItem;
                    tableLayoutPanel1.Controls.Add(cbIn[i * 4 + j], j * 2, i * 6 + 3);

                    cbOut[i * 4 + j] = new ComboBox();
                    cbOut[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbOut[i * 4 + j].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                    cbOut[i * 4 + j].FormattingEnabled = true;
                    cbOut[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbOut[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbOut[i * 4 + j].Name = "cb_out_" + (i * 4 + j + 1).ToString();
                    cbOut[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbOut[i * 4 + j].TabIndex = 17;
                    cbOut[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbOut[i * 4 + j].SelectedIndexChanged += cbOut_SelectedIndexChanged;
                    cbOut[i * 4 + j].DrawMode = DrawMode.OwnerDrawFixed;
                    cbOut[i * 4 + j].DrawItem += cbOut_DrawItem;
                    tableLayoutPanel1.Controls.Add(cbOut[i * 4 + j], j * 2 + 1, i * 6 + 3);

                    lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lbl.Location = new System.Drawing.Point(600, 22);
                    lbl.Name = "lbl_koef_" + (i * 4 + j + 1).ToString();
                    lbl.Size = new System.Drawing.Size(11, 13);
                    lbl.TabIndex = 2;
                    lbl.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_7", this.Name, "Важность");
                    lbl.Dock = System.Windows.Forms.DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.TopCenter;
                    tableLayoutPanel1.Controls.Add(lbl, j * 2, i * 6 + 4);
                    tableLayoutPanel1.SetColumnSpan(lbl, 2);

                    cbMHC[i * 4 + j] = new ComboBox();
                    cbMHC[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbMHC[i * 4 + j].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                    cbMHC[i * 4 + j].FormattingEnabled = true;
                    cbMHC[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbMHC[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbMHC[i * 4 + j].Name = "cb_koef_" + (i * 4 + j + 1).ToString();
                    cbMHC[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbMHC[i * 4 + j].TabIndex = 0;
                    cbMHC[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbMHC[i * 4 + j].SelectedIndexChanged += cbMHC_SelectedIndexChanged;
                    foreach (var vstr in FormMain.dictMsgKoefStrings)
                        cbMHC[i * 4 + j].Items.Add(vstr.Key);
                    tableLayoutPanel1.Controls.Add(cbMHC[i * 4 + j], j * 2, i * 6 + 5);
                    tableLayoutPanel1.SetColumnSpan(cbMHC[i * 4 + j], 2);

                }
            }
            tableLayoutPanel1.ResumeLayout();

            for (int i = 0; i < FormMain.iMsgHarCount; i++)
            {
                dictPairs[i].Clear();
                cbMHC[i].SelectedIndex = -1;
                cbMHC[i].Enabled = false;
                cbMHT[i].SelectedIndex = -1;
                cbMHT[i].Enabled = false;
                cbReplace[i].Checked = false;
                cbReplace[i].Enabled = false;
                cbIn[i].SelectedIndex = -1;
                cbIn[i].Enabled = false;
                cbOut[i].SelectedIndex = -1;
                cbOut[i].Enabled = false;

            }


            this.ResumeLayout();

            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;

        }

        private void cbIn_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            int idx = (Int32)(cb.Tag);
            if (e.Index < 0)
            {
                if (dictPairs[idx].Count > 0)
                    e.Graphics.FillRectangle(new SolidBrush(ColorRedPair), e.Bounds);
                return;
            }

            String value = cbIn[idx].Items[e.Index].ToString();

            Font font = cbIn[idx].Font;

            if (dictPairs[idx].Keys.Contains(value))
                font = new Font(font, FontStyle.Bold);

            if (cb.DroppedDown)
            {
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e.Graphics.FillRectangle(new SolidBrush(ColorRedPair),
                                             e.Bounds);
                else
                    e.Graphics.FillRectangle(new SolidBrush(cb.BackColor),
                                             e.Bounds);
            }
            else
            {
                if (dictPairs[idx].Count > 0)
                    e.Graphics.FillRectangle(new SolidBrush(ColorRedPair), e.Bounds);
                else
                    e.Graphics.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            }

            e.Graphics.DrawString(value, font, new SolidBrush(e.ForeColor), e.Bounds);

        }

        Color ColorRedPair = Color.Orange;

        private void cbOut_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            int idx = (Int32)(cb.Tag);
            if (e.Index < 0)
            {
                if (dictPairs[idx].Count > 0)
                    e.Graphics.FillRectangle(new SolidBrush(ColorRedPair), e.Bounds);
                return;
            }

            String value = cbOut[idx].Items[e.Index].ToString();

            Font font = cbOut[idx].Font;

            if (dictPairs[idx].Values.Contains(value))
                font = new Font(font, FontStyle.Bold);

            if (cb.DroppedDown)
            {
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e.Graphics.FillRectangle(new SolidBrush(ColorRedPair),
                                             e.Bounds);
                else
                    e.Graphics.FillRectangle(new SolidBrush(cb.BackColor),
                                             e.Bounds);
            }
            else
            {
                if (dictPairs[idx].Count > 0)
                    e.Graphics.FillRectangle(new SolidBrush(ColorRedPair), e.Bounds);
                else
                    e.Graphics.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            }

            e.Graphics.DrawString(value, font, new SolidBrush(e.ForeColor), e.Bounds);

        }

        void cbIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb != null)
            {
                if (cb.SelectedIndex >= 0)
                {
                    int idx = (Int32)(cb.Tag);
                    if (dictPairs[idx].Keys.Contains(cb.Items[cb.SelectedIndex].ToString()))
                    {
                        string str = dictPairs[idx][cb.Items[cb.SelectedIndex].ToString()];
                        for (int i = 0; i < cbOut[idx].Items.Count; i++)
                            if (cbOut[idx].Items[i].ToString() == str)
                            {
                                cbOut[idx].SelectedIndex = i;
                                break;
                            }
                    }
                    else
                        cbOut[idx].SelectedIndex = cb.SelectedIndex;

                    cb.Invalidate();
                    cbOut[idx].Invalidate();
                }
            }
        }

        void cbOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb != null)
            {
                if (cb.SelectedIndex >= 0)
                {
                    int idx = (Int32)(cb.Tag);
                    if (cbIn[idx].SelectedIndex >= 0)
                    {
                        string inval = cbIn[idx].Items[cbIn[idx].SelectedIndex].ToString();
                        bool bSave = false;
                        if (dictPairs[idx].Keys.Contains(inval))
                        {
                            dictPairs[idx].Remove(inval);
                            bSave = true;
                        }

                        if (cb.Items[cb.SelectedIndex].ToString() != inval)
                        {
                            dictPairs[idx].Add(inval, cb.Items[cb.SelectedIndex].ToString());
                            bSave = true;
                        }

                        //if (bSave)
                        //    SaveAlgorithmsPairs();
                    }
                    cbIn[idx].Invalidate();
                    cbOut[idx].Invalidate();

                }
            }
        }

        private void cbReplace_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                int idx = (Int32)(cb.Tag);
                algorithmsDBRecord.bMsgReplaceValue[idx] = cb.Checked;
            }
        }


        void cbMHT_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb != null)
            {
                if (cb.SelectedIndex >= 0)
                {
                    int idx = (Int32)(cb.Tag);
                    algorithmsDBRecord.iMsgHarTypes[idx] = cb.SelectedIndex;

                    cbIn[idx].Items.Clear();
                    cbIn[idx].SelectedIndex = -1;
                    cbIn[idx].Enabled = false;

                    cbOut[idx].Items.Clear();
                    cbOut[idx].SelectedIndex = -1;
                    cbOut[idx].Enabled = false;

                    if (cb.SelectedIndex == 0)
                    {
                        if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(idx, algorithmsDBRecord.DefaultAlgorithmTheme) +mFormMain.sMsgHar[idx, 0] + ".txt")))
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(idx, algorithmsDBRecord.DefaultAlgorithmTheme) + mFormMain.sMsgHar[idx, 0] + ".txt"));
                            foreach (var line in srcFile)
                            {
                                cbIn[idx].Items.Add(line);
                                cbOut[idx].Items.Add(line);
                                cbIn[idx].Enabled = true;
                                cbOut[idx].Enabled = true;
                            }

                        }
                    }
                    else if (cb.SelectedIndex == 1)
                    {
                        cbIn[idx].Enabled = true;
                        cbOut[idx].Enabled = true;
                        cbIn[idx].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Число"));
                        cbOut[idx].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Число"));
                        cbOut[idx].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_8", this.Name, "Число+1"));

                    }
                }
            }

        }

        void cbMHC_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb != null)
            {
                if (cb.SelectedIndex >= 0)
                {
                    algorithmsDBRecord.iMsgHarKoef[(Int32)(cb.Tag)] = FormMain.MsgKoefFromString(cb.Items[cb.SelectedIndex].ToString());
                }
            }

        }

        private void SaveAlgorithmsPairs()
        {
            List<String> lstList = new List<String>();
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
            {
                foreach (var pair in dictPairs[i])
                    lstList.Add(i.ToString() + "|" + pair.Key + "|" + pair.Value);
            }
            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_" + algorithmsDBRecord.ID.ToString() + ".txt"), lstList, Encoding.UTF8);
        }

        private void LoadAlgorithmsPairs()
        {
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
                dictPairs[i].Clear();

            List<String> lstList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_" + algorithmsDBRecord.ID.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_" + algorithmsDBRecord.ID.ToString() + ".txt"));
                lstList = new List<String>(srcFile);
                foreach (String str in lstList)
                {
                    if (str == null)
                        continue;

                    if (str.Length == 0)
                        continue;

                    String value = str;
                    int iID = Convert.ToInt32(value.Substring(0, value.IndexOf("|")));
                    value = value.Substring(value.IndexOf("|") + 1);
                    String key = value.Substring(0, value.IndexOf("|"));
                    value = value.Substring(value.IndexOf("|") + 1);
                    dictPairs[iID].Add(key, value);
                }
            }

            for (int idx = 0; idx < FormMain.iMsgHarCount; idx++)
            {
                String keysToRemove="";
                if (dictPairs[idx].Count>0)
                foreach (var pair in dictPairs[idx])
                {
                    bool valueExists = false;
                    for (int i = 0; i < cbIn[idx].Items.Count; i++)
                    {
                        if (pair.Key.Equals(cbIn[idx].Items[i].ToString()) || pair.Value.Equals(cbIn[idx].Items[i].ToString()))
                        {
                            valueExists = true;
                            break;
                        }
                    }
                    if (!valueExists)
                    {
                            keysToRemove += pair.Key + "|";
                    }
                }

                String[] keyRemove = keysToRemove.Split('|');
                foreach(string key in keyRemove)
                {
                    if (key.Length>0)
                        dictPairs[idx].Remove(key);
                }
            }
        }

        public void Setup()
        {
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
            {
                cbIn[i].SelectedIndex = -1;
                cbOut[i].SelectedIndex = -1;
                //cbMHC[i].SelectedIndex = -1;
                //cbMHT[i].SelectedIndex = -1;
                cbMHC[i].SelectedIndex = cbMHC[i].Items.IndexOf(FormMain.MsgKoefToString(algorithmsDBRecord.iMsgHarKoef[i]));
                cbMHT[i].SelectedIndex = algorithmsDBRecord.iMsgHarTypes[i];
                cbReplace[i].Checked = algorithmsDBRecord.bMsgReplaceValue[i];
                cbReplace[i].Enabled = true;
                cbMHC[i].Enabled = true;
                cbMHT[i].Enabled = true;
            }

            LoadAlgorithmsPairs();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveAlgorithmsPairs();
            _formEditAlgorithms.comboBoxAlgorithmsItems.Items[_formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex] = algorithmsDBRecord;
            _formEditAlgorithms.SaveAlgorithmsDBList();

        }
    }
}
