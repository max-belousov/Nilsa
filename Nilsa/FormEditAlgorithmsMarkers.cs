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
    public partial class FormEditAlgorithmsMarkers : Form
    {
        FormMain mFormMain;
        FormEditAlgorithms _formEditAlgorithms;
        AlgorithmsDBRecord algorithmsDBRecord;

        AdvancedComboBox[] cbMarkerAlgorithms;
        AdvancedComboBox[] cbMarkerContacterHar;
        AdvancedComboBox[] cbMarkerContacterHarValue;
        AdvancedComboBox[] cbMarkerMsgHar;
        AdvancedComboBox[] cbMarkerMsgHarValue;
        TextBox[] tbMarkerInMessages;

        public FormEditAlgorithmsMarkers(FormMain _formmain, FormEditAlgorithms formEditAlgorithms)
        {
            mFormMain = _formmain;
            _formEditAlgorithms = formEditAlgorithms;
            algorithmsDBRecord = AlgorithmsDBRecord.FromRecordString(((AlgorithmsDBRecord)_formEditAlgorithms.comboBoxAlgorithmsItems.Items[_formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex]).ToRecordString());

            InitializeComponent();

            cbMarkerAlgorithms = new AdvancedComboBox[FormMain.MaxMarkerCount];
            cbMarkerContacterHar = new AdvancedComboBox[FormMain.MaxMarkerCount];
            cbMarkerContacterHarValue = new AdvancedComboBox[FormMain.MaxMarkerCount];
            cbMarkerMsgHar = new AdvancedComboBox[FormMain.MaxMarkerCount];
            cbMarkerMsgHarValue = new AdvancedComboBox[FormMain.MaxMarkerCount];
            tbMarkerInMessages = new TextBox[FormMain.MaxMarkerCount];

            this.SuspendLayout();
            Width = mFormMain.Width - 20;

            //---
            tableLayoutPanel2.SuspendLayout();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Label lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lbl.Location = new System.Drawing.Point(600, 22);
                    lbl.Name = "lbl_marker_" + (i * 4 + j + 1).ToString();
                    lbl.Size = new System.Drawing.Size(11, 13);
                    lbl.TabIndex = 2;
                    lbl.Text = (i < 2 ? (NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Входящий Маркер")+" " + (i * 4 + j + 1).ToString()) : (NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Исходящий Маркер")+" " + ((i - 2) * 4 + j + 1).ToString()));
                    lbl.Dock = System.Windows.Forms.DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.BackColor = i < 2 ? SystemColors.ActiveCaption : SystemColors.InactiveCaptionText;
                    lbl.ForeColor = i < 2 ? SystemColors.ActiveCaptionText : SystemColors.InactiveCaption;
                    lbl.Tag = (Int32)(i * 4 + j);
                    lbl.Cursor = Cursors.Hand;

                    tableLayoutPanel2.Controls.Add(lbl, j * 2, i * 8);
                    tableLayoutPanel2.SetColumnSpan(lbl, 2);

                    lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lbl.Location = new System.Drawing.Point(600, 22);
                    lbl.Name = "lbl_alg_" + (i * 4 + j + 1).ToString();
                    lbl.Size = new System.Drawing.Size(11, 13);
                    lbl.TabIndex = 2;
                    lbl.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", "FormEditAlgorithms", "Новый алгоритм");
                    lbl.Dock = System.Windows.Forms.DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    tableLayoutPanel2.Controls.Add(lbl, j * 2, i * 8 + 1);
                    tableLayoutPanel2.SetColumnSpan(lbl, 2);

                    cbMarkerAlgorithms[i * 4 + j] = new AdvancedComboBox();
                    cbMarkerAlgorithms[i * 4 + j].SelectorColor=(i < 2 ? mFormMain.bgContact1 : mFormMain.bgPerson1);
                    cbMarkerAlgorithms[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbMarkerAlgorithms[i * 4 + j].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                    cbMarkerAlgorithms[i * 4 + j].FormattingEnabled = true;
                    cbMarkerAlgorithms[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbMarkerAlgorithms[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbMarkerAlgorithms[i * 4 + j].Name = "cb_markeralgorithm_" + (i * 4 + j + 1).ToString();
                    cbMarkerAlgorithms[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbMarkerAlgorithms[i * 4 + j].TabIndex = 0;
                    cbMarkerAlgorithms[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbMarkerAlgorithms[i * 4 + j].SelectedIndexChanged += cbMarkerAlgorithms_SelectedIndexChanged;
                    tableLayoutPanel2.Controls.Add(cbMarkerAlgorithms[i * 4 + j], j * 2, i * 8 + 2);
                    tableLayoutPanel2.SetColumnSpan(cbMarkerAlgorithms[i * 4 + j], 2);

                    lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lbl.Location = new System.Drawing.Point(600, 22);
                    lbl.Name = "lbl_cont_" + (i * 4 + j + 1).ToString();
                    lbl.Size = new System.Drawing.Size(11, 13);
                    lbl.TabIndex = 2;
                    lbl.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Характеристика Контактера");
                    lbl.Dock = System.Windows.Forms.DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    tableLayoutPanel2.Controls.Add(lbl, j * 2, i * 8 + 3);
                    tableLayoutPanel2.SetColumnSpan(lbl, 2);

                    cbMarkerContacterHar[i * 4 + j] = new AdvancedComboBox();
                    cbMarkerContacterHar[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbMarkerContacterHar[i * 4 + j].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                    cbMarkerContacterHar[i * 4 + j].FormattingEnabled = true;
                    cbMarkerContacterHar[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbMarkerContacterHar[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbMarkerContacterHar[i * 4 + j].Name = "cb_markercontacterhar_" + (i * 4 + j + 1).ToString();
                    cbMarkerContacterHar[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbMarkerContacterHar[i * 4 + j].TabIndex = 0;
                    cbMarkerContacterHar[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbMarkerContacterHar[i * 4 + j].Items.Add("");
                    for (int ihar = 0; ihar < mFormMain.iContHarCount; ihar++)
                        cbMarkerContacterHar[i * 4 + j].Items.Add(mFormMain.sContHar[ihar, 1]);
                    cbMarkerContacterHar[i * 4 + j].SelectedIndexChanged += cbMarkerContacterHar_SelectedIndexChanged;
                    tableLayoutPanel2.Controls.Add(cbMarkerContacterHar[i * 4 + j], j * 2, i * 8 + 4);

                    cbMarkerContacterHarValue[i * 4 + j] = new AdvancedComboBox();
                    cbMarkerContacterHarValue[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbMarkerContacterHarValue[i * 4 + j].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                    cbMarkerContacterHarValue[i * 4 + j].FormattingEnabled = true;
                    cbMarkerContacterHarValue[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbMarkerContacterHarValue[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbMarkerContacterHarValue[i * 4 + j].Name = "cb_markercontacterharvalue_" + (i * 4 + j + 1).ToString();
                    cbMarkerContacterHarValue[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbMarkerContacterHarValue[i * 4 + j].TabIndex = 0;
                    cbMarkerContacterHarValue[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbMarkerContacterHarValue[i * 4 + j].SelectedIndexChanged += cbMarkerContacterHarValue_SelectedIndexChanged;
                    cbMarkerContacterHarValue[i * 4 + j].MouseDown += cbMarkerContacterHarValue_OnMouseDown;
                    tableLayoutPanel2.Controls.Add(cbMarkerContacterHarValue[i * 4 + j], j * 2 + 1, i * 8 + 4);

                    lbl = new Label();
                    lbl.AutoSize = true;
                    lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lbl.Location = new System.Drawing.Point(600, 22);
                    lbl.Name = "lbl_msg_" + (i * 4 + j + 1).ToString();
                    lbl.Size = new System.Drawing.Size(11, 13);
                    lbl.TabIndex = 2;
                    lbl.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Характеристика Сообщения");
                    lbl.Dock = System.Windows.Forms.DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    tableLayoutPanel2.Controls.Add(lbl, j * 2, i * 8 + 5);
                    tableLayoutPanel2.SetColumnSpan(lbl, 2);

                    cbMarkerMsgHar[i * 4 + j] = new AdvancedComboBox();
                    cbMarkerMsgHar[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbMarkerMsgHar[i * 4 + j].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                    cbMarkerMsgHar[i * 4 + j].FormattingEnabled = true;
                    cbMarkerMsgHar[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbMarkerMsgHar[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbMarkerMsgHar[i * 4 + j].Name = "cb_markermsghar_" + (i * 4 + j + 1).ToString();
                    cbMarkerMsgHar[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbMarkerMsgHar[i * 4 + j].TabIndex = 0;
                    cbMarkerMsgHar[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbMarkerMsgHar[i * 4 + j].Items.Add("");
                    for (int ihar = 0; ihar < FormMain.iMsgHarCount; ihar++)
                        cbMarkerMsgHar[i * 4 + j].Items.Add(mFormMain.sMsgHar[ihar, 1]);
                    cbMarkerMsgHar[i * 4 + j].SelectedIndexChanged += cbMarkerMsgHar_SelectedIndexChanged;
                    tableLayoutPanel2.Controls.Add(cbMarkerMsgHar[i * 4 + j], j * 2, i * 8 + 6);

                    cbMarkerMsgHarValue[i * 4 + j] = new AdvancedComboBox();
                    cbMarkerMsgHarValue[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    cbMarkerMsgHarValue[i * 4 + j].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                    cbMarkerMsgHarValue[i * 4 + j].FormattingEnabled = true;
                    cbMarkerMsgHarValue[i * 4 + j].Location = new System.Drawing.Point(1121, 22);
                    cbMarkerMsgHarValue[i * 4 + j].Margin = new System.Windows.Forms.Padding(0);
                    cbMarkerMsgHarValue[i * 4 + j].Name = "cb_markermsgharvalue_" + (i * 4 + j + 1).ToString();
                    cbMarkerMsgHarValue[i * 4 + j].Size = new System.Drawing.Size(60, 21);
                    cbMarkerMsgHarValue[i * 4 + j].TabIndex = 0;
                    cbMarkerMsgHarValue[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    cbMarkerMsgHarValue[i * 4 + j].SelectedIndexChanged += cbMarkerMsgHarValue_SelectedIndexChanged;
                    cbMarkerMsgHarValue[i * 4 + j].MouseDown += cbMarkerMsgHarValue_OnMouseDown;
                    tableLayoutPanel2.Controls.Add(cbMarkerMsgHarValue[i * 4 + j], j * 2 + 1, i * 8 + 6);

                    tbMarkerInMessages[i * 4 + j] = new TextBox();
                    tbMarkerInMessages[i * 4 + j].Location = new System.Drawing.Point(600, 0);
                    tbMarkerInMessages[i * 4 + j].Name = "tb_msg_" + (i * 4 + j + 1).ToString();
                    tbMarkerInMessages[i * 4 + j].Size = new System.Drawing.Size(11, 20);
                    tbMarkerInMessages[i * 4 + j].TabIndex = 2;
                    tbMarkerInMessages[i * 4 + j].Text = "";
                    tbMarkerInMessages[i * 4 + j].Margin = new Padding(0);
                    tbMarkerInMessages[i * 4 + j].Dock = System.Windows.Forms.DockStyle.Fill;
                    tbMarkerInMessages[i * 4 + j].Tag = (Int32)(i * 4 + j);
                    tbMarkerInMessages[i * 4 + j].TextChanged += tbMarkerInMessages_TextChanged;

                    tableLayoutPanel2.Controls.Add(tbMarkerInMessages[i * 4 + j], j * 2, i * 8 + 7);
                    tableLayoutPanel2.SetColumnSpan(tbMarkerInMessages[i * 4 + j], 2);
                    //---
                }
            }
            tableLayoutPanel2.ResumeLayout();

            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
            {
                cbMarkerAlgorithms[i].SelectedIndex = -1;
                cbMarkerContacterHar[i].SelectedIndex = -1;
                cbMarkerContacterHarValue[i].SelectedIndex = -1;
                cbMarkerMsgHar[i].SelectedIndex = -1;
                cbMarkerMsgHarValue[i].SelectedIndex = -1;
                tbMarkerInMessages[i].Text = "";
            }
            //---

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            this.ResumeLayout();

            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
        }

        public void Setup()
        {
            for (int ialg = 0; ialg < FormMain.MaxMarkerCount; ialg++)
                cbMarkerAlgorithms[ialg].Items.Add(new AlgoritmID(-1, ""));

            for (int j = 0; j < _formEditAlgorithms.comboBoxAlgorithmsItems.Items.Count; j++)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)_formEditAlgorithms.comboBoxAlgorithmsItems.Items[j];

                for (int ialg = 0; ialg < FormMain.MaxMarkerCount; ialg++)
                    cbMarkerAlgorithms[ialg].Items.Add(new AlgoritmID(dbr.ID, dbr.Name));
            }

            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
            {
                //cbMarkerAlgorithms[i].SelectedIndex = -1;
                if (algorithmsDBRecord.iMarkerAlgorithmsID[i] == -1)
                    cbMarkerAlgorithms[i].SelectedIndex = 0;
                else
                {
                    for (int jj = 0; jj < cbMarkerAlgorithms[i].Items.Count; jj++)
                    {
                        AlgoritmID atempid = (AlgoritmID)cbMarkerAlgorithms[i].Items[jj];
                        if (atempid.ID >= 0)
                        {
                            if (atempid.ID == algorithmsDBRecord.iMarkerAlgorithmsID[i])
                            {
                                cbMarkerAlgorithms[i].SelectedIndex = jj;
                                break;
                            }
                        }
                    }
                }

                cbMarkerContacterHar[i].SelectedIndex = algorithmsDBRecord.iMarkerContHarID[i];
                int iharval = cbMarkerContacterHarValue[i].Items.IndexOf(algorithmsDBRecord.sMarkerContHarValues[i]);
                cbMarkerContacterHarValue[i].SelectedIndex = iharval;

                cbMarkerMsgHar[i].SelectedIndex = algorithmsDBRecord.iMarkerMsgHarID[i];
                int imsgharval = cbMarkerMsgHarValue[i].Items.IndexOf(algorithmsDBRecord.sMarkerMsgHarValues[i]);
                cbMarkerMsgHarValue[i].SelectedIndex = imsgharval;

                tbMarkerInMessages[i].Text = algorithmsDBRecord.sMarkerInMessages[i];
            }

        }

        private void HighlightBlock(int _idx)
        {
            bool _highlight = cbMarkerAlgorithms[_idx].SelectedIndex > 0 || cbMarkerContacterHar[_idx].SelectedIndex > 0 || cbMarkerMsgHar[_idx].SelectedIndex > 0 || tbMarkerInMessages[_idx].Text.Length > 0;
            Color backcolor = _highlight ? (_idx<8 ? mFormMain.bgContact1: mFormMain.bgPerson1) : SystemColors.Control;

            cbMarkerAlgorithms[_idx].HighlightColor = backcolor;
            cbMarkerContacterHar[_idx].HighlightColor = backcolor;
            cbMarkerContacterHarValue[_idx].HighlightColor = backcolor;
            cbMarkerMsgHar[_idx].HighlightColor = backcolor;
            cbMarkerMsgHarValue[_idx].HighlightColor = backcolor;
            tbMarkerInMessages[_idx].BackColor = backcolor;

            cbMarkerAlgorithms[_idx].Invalidate();
            cbMarkerContacterHar[_idx].Invalidate();
            cbMarkerContacterHarValue[_idx].Invalidate();
            cbMarkerMsgHar[_idx].Invalidate();
            cbMarkerMsgHarValue[_idx].Invalidate();
            tbMarkerInMessages[_idx].Invalidate();
        }

        private void cbMarkerContacterHar_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdvancedComboBox cb = sender as AdvancedComboBox;
            if (cb != null)
            {
                if (cb.SelectedIndex >= 0)
                {
                    int idx = (Int32)(cb.Tag);
                    algorithmsDBRecord.iMarkerContHarID[idx] = cb.SelectedIndex;
                    if (cb.SelectedIndex == 0)
                        algorithmsDBRecord.sMarkerContHarValues[idx] = "";
                    HighlightBlock(idx);

                    cbMarkerContacterHarValue[idx].Items.Clear();
                    cbMarkerContacterHarValue[idx].SelectedIndex = -1;
                    cbMarkerContacterHarValue[idx].Enabled = false;
                    cbMarkerContacterHarValue[idx].Items.Add("");
                    if (cb.SelectedIndex > 0)
                    {
                        if (File.Exists(Path.Combine(FormMain.sDataPath, "_cont_har_" + mFormMain.sContHar[algorithmsDBRecord.iMarkerContHarID[idx] - 1, 0] + ".txt")))
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_cont_har_" + mFormMain.sContHar[algorithmsDBRecord.iMarkerContHarID[idx] - 1, 0] + ".txt"));
                            foreach (var line in srcFile)
                            {
                                cbMarkerContacterHarValue[idx].Items.Add(line);
                            }
                        }
                        cbMarkerContacterHarValue[idx].Enabled = true;
                    }
                }
            }

        }

        private void cbMarkerContacterHarValue_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                AdvancedComboBox cb = sender as AdvancedComboBox;
                if (cb != null)
                {
                    int idx = (Int32)(cb.Tag);
                    if (cbMarkerContacterHar[idx].SelectedIndex >= 0)
                    {
                        string value = "";
                        if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Добавление хар-ки Контактера"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "Новое значение:"), ref value) == DialogResult.OK)
                        {
                            if (cb.Items.IndexOf(value) < 0)
                            {
                                List<String> lstValues = new List<string>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_cont_har_" + mFormMain.sContHar[algorithmsDBRecord.iMarkerContHarID[idx] - 1, 0] + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_cont_har_" + mFormMain.sContHar[algorithmsDBRecord.iMarkerContHarID[idx] - 1, 0] + ".txt"));
                                    foreach (var line in srcFile)
                                    {
                                        lstValues.Add(line);
                                    }
                                }
                                lstValues.Add(value);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_cont_har_" + mFormMain.sContHar[algorithmsDBRecord.iMarkerContHarID[idx] - 1, 0] + ".txt"), lstValues, Encoding.UTF8);
                                cb.Items.Add(value);
                                cb.SelectedIndex = cb.Items.Count - 1;
                            }
                            else
                            {
                                int iharval = cb.Items.IndexOf(value);
                                cbMarkerContacterHarValue[idx].SelectedIndex = iharval;
                            }
                        }

                    }
                }
            }
            base.OnMouseDown(e);
        }

        void cbMarkerContacterHarValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdvancedComboBox cb = sender as AdvancedComboBox;
            if (cb != null)
            {
                if (cb.SelectedIndex >= 0)
                {
                    int idx = (Int32)(cb.Tag);
                    if (!cb.Items[cb.SelectedIndex].ToString().Equals(algorithmsDBRecord.sMarkerContHarValues[idx]))
                    {
                        algorithmsDBRecord.sMarkerContHarValues[idx] = cb.Items[cb.SelectedIndex].ToString();
                    }
                }
            }

        }

        //---
        void cbMarkerMsgHar_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdvancedComboBox cb = sender as AdvancedComboBox;
            if (cb != null)
            {
                if (cb.SelectedIndex >= 0)
                {
                    int idx = (Int32)(cb.Tag);
                    algorithmsDBRecord.iMarkerMsgHarID[idx] = cb.SelectedIndex;
                    if (cb.SelectedIndex == 0)
                        algorithmsDBRecord.sMarkerMsgHarValues[idx] = "";
                    HighlightBlock(idx);

                    cbMarkerMsgHarValue[idx].Items.Clear();
                    cbMarkerMsgHarValue[idx].SelectedIndex = -1;
                    cbMarkerMsgHarValue[idx].Enabled = false;
                    cbMarkerMsgHarValue[idx].Items.Add("");
                    if (cb.SelectedIndex > 0)
                    {
                        if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, algorithmsDBRecord.DefaultAlgorithmTheme) + mFormMain.sMsgHar[algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, 0] + ".txt")))
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, algorithmsDBRecord.DefaultAlgorithmTheme) + mFormMain.sMsgHar[algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, 0] + ".txt"));
                            foreach (var line in srcFile)
                            {
                                cbMarkerMsgHarValue[idx].Items.Add(line);
                            }

                        }
                        cbMarkerMsgHarValue[idx].Enabled = true;
                    }
                }
            }

        }

        private void cbMarkerMsgHarValue_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                AdvancedComboBox cb = sender as AdvancedComboBox;
                if (cb != null)
                {
                    int idx = (Int32)(cb.Tag);
                    if (cbMarkerMsgHar[idx].SelectedIndex >= 0)
                    {
                        string value = "";
                        if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_7", this.Name, "Добавление хар-ки Cообщения"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_8", this.Name, "Новое значение:"), ref value) == DialogResult.OK)
                        {
                            if (cb.Items.IndexOf(value) < 0)
                            {
                                List<String> lstValues = new List<string>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, algorithmsDBRecord.DefaultAlgorithmTheme) + mFormMain.sMsgHar[algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, 0] + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, algorithmsDBRecord.DefaultAlgorithmTheme) + mFormMain.sMsgHar[algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, 0] + ".txt"));
                                    foreach (var line in srcFile)
                                    {
                                        lstValues.Add(line);
                                    }
                                }
                                lstValues.Add(value);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, algorithmsDBRecord.DefaultAlgorithmTheme) + mFormMain.sMsgHar[algorithmsDBRecord.iMarkerMsgHarID[idx] - 1, 0] + ".txt"), lstValues, Encoding.UTF8);
                                cb.Items.Add(value);
                                cb.SelectedIndex = cb.Items.Count - 1;
                            }
                            else
                            {
                                int iharval = cb.Items.IndexOf(value);
                                cbMarkerMsgHarValue[idx].SelectedIndex = iharval;
                            }
                        }

                    }
                }
            }
            base.OnMouseDown(e);
        }

        private void cbMarkerMsgHarValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdvancedComboBox cb = sender as AdvancedComboBox;
            if (cb != null)
            {
                if (cb.SelectedIndex >= 0)
                {
                    int idx = (Int32)(cb.Tag);
                    if (!cb.Items[cb.SelectedIndex].ToString().Equals(algorithmsDBRecord.sMarkerMsgHarValues[idx]))
                    {
                        algorithmsDBRecord.sMarkerMsgHarValues[idx] = cb.Items[cb.SelectedIndex].ToString();
                    }
                }
            }

        }


        //---
        private void cbMarkerAlgorithms_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdvancedComboBox cb = sender as AdvancedComboBox;
            if (cb != null)
            {
                if (cb.SelectedIndex >= 0)
                {
                    int idx = (Int32)(cb.Tag);
                    AlgoritmID atempid = (AlgoritmID)cb.Items[cb.SelectedIndex];
                    algorithmsDBRecord.iMarkerAlgorithmsID[idx] = (cb.SelectedIndex == 0 ? -1 : atempid.ID);
                    HighlightBlock(idx);
                }
            }

        }

        private void tbMarkerInMessages_TextChanged(object sender, EventArgs e)
        {
            TextBox cb = sender as TextBox;
            if (cb != null)
            {
                int idx = (Int32)(cb.Tag);
                String sText = cb.Text;
                if (sText.IndexOf("|") >= 0)
                {
                    sText = sText.Replace('|', ' ');
                    algorithmsDBRecord.sMarkerInMessages[idx] = sText;
                    cb.Text = sText;
                }
                else
                    algorithmsDBRecord.sMarkerInMessages[idx] = sText;
                HighlightBlock(idx);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _formEditAlgorithms.comboBoxAlgorithmsItems.Items[_formEditAlgorithms.comboBoxAlgorithmsItems.SelectedIndex] = algorithmsDBRecord;
            _formEditAlgorithms.SaveAlgorithmsDBList();
        }

        // End of class
    }
}
