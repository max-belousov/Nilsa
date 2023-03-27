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

namespace Nilsa
{
    public partial class FormInitContactDialog : Form
    {
        public String[,] sContHar;
        public int iContHarCount = 16;
        public int iContHarAttrCount = 4;
        List<String> lstErrorsLogList;
        String slstErrorsInit;
        long iPersUserID;
        List<String> lstContactsList;
        FormMain mFormMain;
        Label[] lblEQOutHarNames;
        Label[] lblEQOutHarValues;
        String sCurrentEQOutMessageRecordOut;
        String sCurrentEQOutMessageRecordDefault;
        public int[] iCompareVectorsKoefOut;
        List<String> lstPersHarValues;
        List<String> lstContHarValues;
        List<String> lstEQOutMessagesList;
        List<String> lstUserEnabledAlgorithmsList;

        private void LoadComboBoxAlgSetList()
        {
            int retval = 0;
            List<String> lstList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"));
                lstList = new List<String>(srcFile);
            }

            lstUserEnabledAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(FormMain.SocialNetwork) + mFormMain.iPersUserID.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(FormMain.SocialNetwork) + mFormMain.iPersUserID.ToString() + ".txt"));
                lstUserEnabledAlgorithmsList = new List<String>(srcFile);
            }
            //else
            //    lstUserEnabledAlgorithmsList.Add("0");

            //lstAlgorithmsDB = new Dictionary<int, AlgorithmsDBRecord>();
            comboBoxAlgSetList.SelectedIndex = -1;
            comboBoxAlgSetList.Items.Clear();

            long retvalid = -1, retvalerrorid = -1;
            foreach (String value in lstList)
            {
                AlgorithmsDBRecord dbr = AlgorithmsDBRecord.FromRecordString(value);

                if (lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                {

                    comboBoxAlgSetList.Items.Add(dbr);

                    if (dbr.Name.ToLower().Equals("error"))
                    {
                        retval = comboBoxAlgSetList.Items.Count - 1;
                        retvalerrorid = dbr.ID;
                    }
                }
            }

            int retvalerror = retval;
            retvalid = NilsaUtils.LoadLongValue(2, retvalerrorid);

            for (int i = 0; i < comboBoxAlgSetList.Items.Count; i++)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgSetList.Items[i];
                if (dbr.ID == retvalid)
                {
                    retval = i;
                    break;
                }
            }

            if (retval < comboBoxAlgSetList.Items.Count)
                comboBoxAlgSetList.SelectedIndex = retval;
            else
            {
                if (retvalerror < comboBoxAlgSetList.Items.Count)
                    comboBoxAlgSetList.SelectedIndex = retvalerror;
            }
        }

        public FormInitContactDialog(FormMain formmain)
        {
            mFormMain = formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            lblEQOutHarValues = new Label[FormMain.iMsgHarCount];
            lblEQOutHarNames = new Label[FormMain.iMsgHarCount];

            lstContHarValues = new List<String>();
            contextMenuStripVectorKoefOut.Items.Clear();
            foreach (var vstr in FormMain.dictMsgKoefStrings)
            {
                ToolStripMenuItem tsmiout = new ToolStripMenuItem(vstr.Key, null, contextMenuStripVectorKoefOut_Click);
                tsmiout.Tag = (Int32)vstr.Value;
                contextMenuStripVectorKoefOut.Items.Add(tsmiout);
            }

            comboBoxAlgSetAction.SelectedIndex = 1;
            LoadComboBoxAlgSetList();

            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel7.ColumnCount = 8;
            tableLayoutPanel7.RowCount = 4;

            for (int j = 0; j < 2; j++)
            {
                //tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
                //tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
                for (int i = 0; i < FormMain.iMsgHarCount / 2; i++)
                {
                    int idx = j * FormMain.iMsgHarCount / 2 + i;
                    lblEQOutHarNames[idx] = new Label();
                    lblEQOutHarNames[idx].AutoSize = true;
                    lblEQOutHarNames[idx].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lblEQOutHarNames[idx].Location = new System.Drawing.Point(600, 22);
                    lblEQOutHarNames[idx].Name = "labelEQOutN" + idx.ToString();
                    lblEQOutHarNames[idx].Size = new System.Drawing.Size(11, 13);
                    lblEQOutHarNames[idx].TabIndex = 2;
                    lblEQOutHarNames[idx].Text = " ";

                    lblEQOutHarNames[idx].Dock = System.Windows.Forms.DockStyle.Fill;
                    lblEQOutHarNames[idx].Tag = (Int32)idx;
                    lblEQOutHarNames[idx].Image = global::Nilsa.Properties.Resources.down_arrow_square_silver_Shapes4FREE;
                    lblEQOutHarNames[idx].ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
                    lblEQOutHarNames[idx].Cursor = Cursors.Hand;
                    lblEQOutHarNames[idx].MouseClick += lblEQOutHarNames_MouseClick;

                    lblEQOutHarValues[idx] = new Label();
                    lblEQOutHarValues[idx].AutoSize = true;
                    lblEQOutHarValues[idx].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    lblEQOutHarValues[idx].Location = new System.Drawing.Point(600, 22);
                    lblEQOutHarValues[idx].Name = "labelEQOutV" + idx.ToString();
                    lblEQOutHarValues[idx].Size = new System.Drawing.Size(11, 13);
                    lblEQOutHarValues[idx].TabIndex = 2;
                    lblEQOutHarValues[idx].Text = " ";
                    lblEQOutHarValues[idx].Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);

                    lblEQOutHarValues[idx].Dock = System.Windows.Forms.DockStyle.Fill;
                    lblEQOutHarValues[idx].Tag = (Int32)idx;
                    lblEQOutHarValues[idx].Image = global::Nilsa.Properties.Resources.down_arrow_square_silver_Shapes4FREE;
                    lblEQOutHarValues[idx].ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
                    lblEQOutHarValues[idx].Cursor = Cursors.Hand;
                    lblEQOutHarValues[idx].MouseClick += lblEQOutHarValues_MouseClick;

                    tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
                    tableLayoutPanel7.Controls.Add(lblEQOutHarNames[idx], i, j * 2);
                    tableLayoutPanel7.Controls.Add(lblEQOutHarValues[idx], i, j * 2 + 1);
                }
            }
            numericUpDown1.Value = NilsaUtils.LoadLongValue(4, 20);
            numericUpDownInitDialogPerPersoneLimit.Value = NilsaUtils.LoadLongValue(117, 40);
            tableLayoutPanel7.PerformLayout();


            tableLayoutPanel7.ResumeLayout();

        }

        private void ChangeVectorKoefOut(int idx, int value)
        {
            iCompareVectorsKoefOut[idx] = value;
            lblEQOutHarNames[idx].Text = mFormMain.sMsgHar[idx, 1] + " (" + FormMain.MsgKoefToString(iCompareVectorsKoefOut[idx]) + ")";
            SetEQOutMessageListOut(sCurrentEQOutMessageRecordOut);
        }

        private void contextMenuStripVectorKoefOut_Click(object sender, EventArgs e)
        {
            ChangeVectorKoefOut((Int32)(contextMenuStripVectorKoefOut.Tag), (Int32)(((ToolStripMenuItem)sender).Tag));
        }

        void lblEQOutHarNames_MouseClick(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl != null)
            {
                contextMenuStripVectorKoefOut.Tag = (Int32)(lbl.Tag);
                contextMenuStripVectorKoefOut.Show(lbl.PointToScreen(new Point(e.X, e.Y)));
            }
        }

        void lblEQOutHarValues_MouseClick(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;

            if (lbl != null)
            {
                bool bShow = false;
                int idx = (Int32)(lbl.Tag);
                String path = Path.Combine(FormMain.sDataPath, "_msg_har_" + mFormMain.getMessagesDBThemeID(idx, lblEQOutHarValues[0].Text) + (idx + 1).ToString() + ".txt");
                if (idx == 0)
                    path = Path.Combine(FormMain.sDataPath, "_msg_har_" + (idx + 1).ToString() + ".txt");

                if (File.Exists(path))
                {
                    var srcFile = File.ReadAllLines(path);
                    List<String> lstValues = new List<String>(srcFile);

                    contextMenuStripEQOutMsgValues.Items.Clear();
                    contextMenuStripEQOutMsgValues.Tag = (Int32)(lbl.Tag);

                    System.Windows.Forms.ToolStripMenuItem tsmiItem = new System.Windows.Forms.ToolStripMenuItem();
                    tsmiItem.Name = "lblEQOutHarValuestsmiItemZero";
                    tsmiItem.Size = new System.Drawing.Size(147, 22);
                    tsmiItem.Text = "";
                    tsmiItem.Click += contextMenuStripEQOutMsgValuesItem_Click;

                    contextMenuStripEQOutMsgValues.Items.Add(tsmiItem);

                    if (idx >= 0)
                    {
                        for (int i = 0; i < lstValues.Count; i++)
                        {
                            tsmiItem = new System.Windows.Forms.ToolStripMenuItem();
                            tsmiItem.Name = "lblEQOutHarValuestsmiItem" + i.ToString();
                            tsmiItem.Size = new System.Drawing.Size(147, 22);
                            tsmiItem.Text = lstValues[i];
                            tsmiItem.Click += contextMenuStripEQOutMsgValuesItem_Click;

                            contextMenuStripEQOutMsgValues.Items.Add(tsmiItem);
                        }
                    }
                    //else
                    //{
                    //    int i = 0;
                    //    foreach (var pair in mFormMain.lstMessagesDB)
                    //    {
                    //        if (!mFormMain.lstMessagesDBUserList.Contains(pair.Value.ToString()))
                    //        {
                    //            tsmiItem = new System.Windows.Forms.ToolStripMenuItem();
                    //            tsmiItem.Name = "lblEQOutHarValuestsmiItem" + i.ToString();
                    //            tsmiItem.Size = new System.Drawing.Size(147, 22);
                    //            tsmiItem.Text = pair.Key;
                    //            tsmiItem.Click += contextMenuStripEQOutMsgValuesItem_Click;

                    //            contextMenuStripEQOutMsgValues.Items.Add(tsmiItem);
                    //            i++;
                    //        }
                    //    }
                    //}
                    bShow = true;
                    contextMenuStripEQOutMsgValues.Show(lbl.PointToScreen(new Point(e.X, e.Y)));
                }
                else
                {
                    contextMenuStripEQOutMsgValues.Items.Clear();
                    contextMenuStripEQOutMsgValues.Tag = (Int32)(lbl.Tag);

                    System.Windows.Forms.ToolStripMenuItem tsmiItem = new System.Windows.Forms.ToolStripMenuItem();
                    tsmiItem.Name = "lblEQOutHarValuestsmiItemZero";
                    tsmiItem.Size = new System.Drawing.Size(147, 22);
                    tsmiItem.Text = "";
                    tsmiItem.Click += contextMenuStripEQOutMsgValuesItem_Click;

                    contextMenuStripEQOutMsgValues.Items.Add(tsmiItem);
                    bShow = true;
                    contextMenuStripEQOutMsgValues.Show(lbl.PointToScreen(new Point(e.X, e.Y)));

                }
                if (!bShow)
                    toolTipMessage.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Нет возможных значений характеристик..."), this, lbl.PointToScreen(new Point(e.X, e.Y)), 1000);

            }
        }

        void setEQOutHarValues(int _idx, String _text, bool _update)
        {
            lblEQOutHarValues[_idx].Text = _text;
            String value = sCurrentEQOutMessageRecordDefault;
            if (value.Length == 0)
            {
                value = NilsaUtils.LoadStringValue(2, "000000|" + FormMain.strDB0Name + "||||||||||||||||@!-");
            }
            sCurrentEQOutMessageRecordDefault = value.Substring(0, value.IndexOf("|") + 1);

            for (int i = 0; i < FormMain.iMsgHarCount; i++)
            {
                value = value.Substring(value.IndexOf("|") + 1);
                String s = value.Substring(0, value.IndexOf("|"));
                if (i == _idx)
                    s = _text;
                sCurrentEQOutMessageRecordDefault += s + "|";
            }
            sCurrentEQOutMessageRecordDefault += value.Substring(value.IndexOf("|") + 1);
            NilsaUtils.SaveStringValue(2, sCurrentEQOutMessageRecordDefault);

            if (_update)
            {
                if (listBox1.SelectedIndex >= 0)
                {
                    String srec = listBox1.Items[listBox1.SelectedIndex].ToString();
                    String sUID = srec.Substring(0, srec.IndexOf(" "));
                    LoadContactParamersValues(sUID);
                    SetEQOutMessageListOut(sCurrentEQOutMessageRecordOut);
                }
                else
                    SetEQOutMessageListOut(sCurrentEQOutMessageRecordDefault);
            }
        }

        void contextMenuStripEQOutMsgValuesItem_Click(object sender, EventArgs e)
        {
            int idx = (Int32)(contextMenuStripEQOutMsgValues.Tag);
            String text = ((System.Windows.Forms.ToolStripMenuItem)sender).Text;

            setEQOutHarValues(idx, text, true);
        }

        private void SetEQOutMessageParametersDefaultValuesOut()
        {
            if (sCurrentEQOutMessageRecordDefault.Length > 0)
            {
                String value = sCurrentEQOutMessageRecordDefault;
                // Установка параметров по умолчанию
                for (int i = 0; i < FormMain.iMsgHarCount; i++)
                {
                    value = value.Substring(value.IndexOf("|") + 1);
                    String s = value.Substring(0, value.IndexOf("|"));
                    //sMsgHar[i, 3] = s; // Значение атрибутов
                    lblEQOutHarValues[i].Text = s;
                }
            }
        }

        private String GetMessageTextWithMarker(String sMsgText)
        {
            if (sMsgText.IndexOf("|!*#0") >= 0)
            {
                sMsgText = sMsgText.Replace("|!*#0", "(" + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Маркер") + " ");
                sMsgText += ")";
            }
            return sMsgText;
        }

        private void SetEQOutMessageListOut(String sRQ)
        {
            //Set_labelOutEqMsgHarTitleValue_Text("");

            //for (int i = 0; i < iMsgHarCount; i++)
            //    lblEQOutHarValues[i].Text = "";
            SetEQOutMessageParametersDefaultValuesOut();
            listBoxOutMsg.SelectedIndex = -1;
            listBoxOutMsg.Items.Clear();

            if (sRQ.Length > 0)
            {
                lstEQOutMessagesList = new List<String>();
                sRQ = sRQ.Substring(sRQ.IndexOf("|") + 1);
                sRQ = sRQ.Substring(0, sRQ.IndexOf("|@!"));

                String[] RQV = sRQ.ToLower().Split('|');
                double dCompareVectorLevel = 0;// comboBoxCompareVectorLevel.SelectedIndex * 10;
                double dWMax = 0;
                for (int i = 0; i < FormMain.iMsgHarCount; i++)
                    dWMax += (iCompareVectorsKoefOut[i] == 1111 ? 100 : iCompareVectorsKoefOut[i]);

                if (dWMax > 0)
                {
                    foreach (String EQ in mFormMain.lstEQOutMessagesDB)
                    {
                        double dW = 0;
                        bool bSuccess = true;
                        String EQText = EQ.Substring(EQ.IndexOf("|") + 1);
                        EQText = EQText.Substring(0, EQText.IndexOf("|@!"));
                        String[] OQV = EQText.ToLower().Split('|');

                        for (int iv = 0; iv < RQV.Length; iv++)
                        {
                            //if (iCompareVectorsKoefOut[iv] == 1111 && RQV[iv].Length > 0 && OQV[iv].Length == 0)
                            //    bSuccess = false;
                            if (iCompareVectorsKoefOut[iv] == 1111 && RQV[iv].Length > 0 && OQV[iv].Length == 0)
                            {
                                if (iv == 12 || iv == 13)
                                {
                                    dW += iCompareVectorsKoefOut[iv] == 1111 ? 100 : iCompareVectorsKoefOut[iv];
                                    continue;
                                }
                                else
                                    bSuccess = false;
                            }

                            if ("*".Equals(OQV[iv]))
                            {
                                dW += (float)((iCompareVectorsKoefOut[iv] == 1111 ? 100 : iCompareVectorsKoefOut[iv]) * 90.0) / 100.0;
                                continue;
                            }

                            if (RQV[iv].Length == 0 && OQV[iv].Length == 0)
                                dW += 0.1 * (iCompareVectorsKoefOut[iv] == 1111 ? 100 : iCompareVectorsKoefOut[iv]);

                            if (RQV[iv].Length == 0 || OQV[iv].Length == 0)
                                continue;

                            String sInVal = RQV[iv];
                            /*
                            if (mFormMain.adbrCurrent.ID >= 0)
                            {
                                if (mFormMain.adbrCurrent.iMsgHarTypes[iv] == 0)
                                {
                                    if (mFormMain.adbrCurrentDictPairs[iv].Keys.Contains(sInVal))
                                        sInVal = mFormMain.adbrCurrentDictPairs[iv][sInVal].ToLower();
                                }
                                else if (mFormMain.adbrCurrent.iMsgHarTypes[iv] == 1)
                                {
                                    if (mFormMain.adbrCurrentDictPairs[iv].Count > 0)
                                    {
                                        try
                                        {
                                            int iinval = Convert.ToInt32(sInVal);
                                            iinval++;
                                            sInVal = iinval.ToString();
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                else
                                {
                                }
                            }
                            */

                            if (sInVal.Equals(OQV[iv]))
                                dW += (iCompareVectorsKoefOut[iv] == 1111 ? 100 : iCompareVectorsKoefOut[iv]);
                            else
                            {
                                if (iCompareVectorsKoefOut[iv] == 1111)
                                    bSuccess = false;
                            }
                        }
                        dW = Math.Round(dW * 100 / dWMax);
                        if (dW > dCompareVectorLevel && bSuccess)
                        {
                            String value = ((int)dW).ToString("000000").Substring(0, 6) + "|" + EQ.Substring(EQ.IndexOf("|") + 1);
                            lstEQOutMessagesList.Add(value);
                        }
                    }
                }
                lstEQOutMessagesList = lstEQOutMessagesList.OrderByDescending(i => i).ToList();

                foreach (String EQ in lstEQOutMessagesList)
                {
                    listBoxOutMsg.Items.Add(Convert.ToInt32(EQ.Substring(0, EQ.IndexOf("|"))).ToString("00") + "% " + GetMessageTextWithMarker(EQ.Substring(EQ.IndexOf("|@!") + 3)));
                }

                if (lstEQOutMessagesList.Count > 0)
                {
                    listBoxOutMsg.SelectedIndex = 0;
                }
            }
        }

        private String GetPersoneParametersValue(String sPersHarID)
        {
            for (int i = 0; i < lstPersHarValues.Count; i++)
            {
                if (lstPersHarValues[i].Substring(0, lstPersHarValues[i].IndexOf("|")) == sPersHarID)
                {
                    return lstPersHarValues[i].Substring(lstPersHarValues[i].IndexOf("|") + 1);
                }
            }
            return "";
        }

        private String GetContactParametersValue(String sContHarID)
        {
            for (int i = 0; i < lstContHarValues.Count; i++)
            {
                if (lstContHarValues[i].Substring(0, lstContHarValues[i].IndexOf("|")) == sContHarID)
                {
                    return lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1);
                }
            }
            return "";
        }

        private void ExceptionToLogList(String sMethod, String sErrorsParameters, Exception e)
        {
            lstErrorsLogList.Add(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + Application.ProductVersion + " - " + slstErrorsInit + "\n" + sMethod + ": " + sErrorsParameters + "\n" + e.ToString());
            SaveErrorsLogList();
        }

        private void SaveErrorsLogList()
        {
            if (lstErrorsLogList.Count >= 500)
                lstErrorsLogList = lstErrorsLogList.GetRange(lstErrorsLogList.Count - 500, 500);

            if (lstErrorsLogList.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "_errors_log_list.txt"), lstErrorsLogList, Encoding.UTF8);
        }

        private void LoadErrorsLogList()
        {
            lstErrorsLogList = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "_errors_log_list.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_errors_log_list.txt"));
                lstErrorsLogList = new List<String>(srcFile);
            }
        }

        private void LoadPersoneParametersValues()
        {
            long lPersID = iPersUserID;
            lstPersHarValues = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix() + Convert.ToString(lPersID) + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix() + Convert.ToString(lPersID) + ".txt"));
                lstPersHarValues = new List<String>(srcFile);
            }
        }

        public void Setup(String sErrorHdr, long iUsrID, bool initDialog)
        {
            slstErrorsInit = sErrorHdr;
            iPersUserID = iUsrID;

            LoadErrorsLogList();
            LoadPersoneParametersValues();
            ContactsList_Load();
            button6.Enabled = FormMain.SocialNetwork == 0;


            comboBox1.SelectedIndex = 0;

            sCurrentEQOutMessageRecordDefault = NilsaUtils.LoadStringValue(2, "000000|" + FormMain.strDB0Name + "||||||||||||||||@!-");
            sCurrentEQOutMessageRecordOut = sCurrentEQOutMessageRecordDefault;
            SetEQOutMessageParametersDefaultValuesOut();

            iCompareVectorsKoefOut = new int[FormMain.iMsgHarCount];
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
            {
                iCompareVectorsKoefOut[i] = (mFormMain.adbrCurrent.ID >= 0 ? mFormMain.adbrCurrent.iMsgHarKoef[i] : mFormMain.iMsgHarKoef[i]);
                lblEQOutHarNames[i].Text = mFormMain.sMsgHar[i, 1] + " (" + FormMain.MsgKoefToString(iCompareVectorsKoefOut[i]) + ")";
            }
            button2.Enabled = false;
            button7.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;

            Cursor = Cursors.WaitCursor;
            listBox1.SelectedIndex = -1;
            listBox1.BeginUpdate();
            listBox1.Items.Clear();

            for (int i = 0; i < lstContactsList.Count; i++)
            {

                String value = lstContactsList[i];
                String sUID = value.Substring(0, value.IndexOf("|")); // usrID
                value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                String sUName = value;
                ContactsList_AddUserToVisualList(sUID, sUName);
            }
            listBox1.EndUpdate();
            tabControl1.SelectedIndex = 1;

            if (lstContactsList.Count > 0)
                listBox1.SelectedIndex = 0;


            Cursor = Cursors.Arrow;
            if (initDialog)
            {
                INIT_PERSONE_DIALOG = true;
                button2_Click(null, null);
                INIT_PERSONE_DIALOG = true;
                timerClose.Enabled = true;
            }
        }

        private bool INIT_PERSONE_DIALOG = false;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                if (listBox1.Items.Count > 0)
                {
                    button2.Enabled = true;
                    button7.Enabled = mFormMain.lstPersoneChange.Count > 0 && comboBox1.SelectedIndex == 0 && tabControl1.SelectedIndex == 1;
                    if (listBox1.SelectedIndex >= 0) button3.Enabled = true; else button3.Enabled = false;
                }
                else
                {
                    button2.Enabled = false;
                    button7.Enabled = false;
                    button3.Enabled = false;
                }
            }
            else
            {
                button2.Enabled = false;
                button7.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void LoadContactParamersValues(String sUID)
        {
            lstContHarValues = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                lstContHarValues = new List<String>(srcFile);

            }
            String value = sCurrentEQOutMessageRecordDefault;
            if (value.Length == 0)
            {
                value = NilsaUtils.LoadStringValue(2, "000000|" + FormMain.strDB0Name + "||||||||||||||||@!-");
            }
            sCurrentEQOutMessageRecordOut = value.Substring(0, value.IndexOf("|") + 1);

            for (int i = 0; i < FormMain.iMsgHarCount; i++)
            {
                value = value.Substring(value.IndexOf("|") + 1);
                String s = value.Substring(0, value.IndexOf("|"));
                if (s.Length == 0)
                {
                    if (mFormMain.sMsgHar[i, 2].IndexOf("*") > 0)
                    {
                        String s1 = mFormMain.sMsgHar[i, 2].Substring(mFormMain.sMsgHar[i, 2].IndexOf("*") + 1);
                        for (int j = 0; j < iContHarCount; j++)
                        {
                            if (sContHar[j, 0].Equals(s1))
                            {
                                s = GetContactParametersValue(mFormMain.sContHar[j, 0]);
                                //lblEQOutHarValues[i].Text = s;
                                break;
                            }
                        }
                    }
                    else if (mFormMain.sMsgHar[i, 2].IndexOf("#") > 0)
                    {
                        String s1 = mFormMain.sMsgHar[i, 2].Substring(mFormMain.sMsgHar[i, 2].IndexOf("#") + 1);
                        for (int j = 0; j < mFormMain.iPersHarCount; j++)
                        {
                            if (mFormMain.sPersHar[j, 0].Equals(s1))
                            {
                                s = GetPersoneParametersValue(mFormMain.sPersHar[j, 0]);
                                //lblEQOutHarValues[i].Text = s;
                                break;
                            }
                        }
                    }
                }

                sCurrentEQOutMessageRecordOut += s + "|";
            }
            sCurrentEQOutMessageRecordOut += value.Substring(value.IndexOf("|") + 1);
        }



        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                button5.Enabled = true;
                String srec = listBox1.Items[listBox1.SelectedIndex].ToString();
                String sUID = srec.Substring(0, srec.IndexOf(" "));
                LoadContactParamersValues(sUID);
                SetEQOutMessageListOut(sCurrentEQOutMessageRecordOut);
                if (tabControl1.SelectedIndex == 0)
                {
                    if (textBox1.Text.Length > 0) button3.Enabled = true; else button3.Enabled = false;
                }
                else
                {
                    if (listBoxOutMsg.Items.Count > 0) button3.Enabled = true; else button3.Enabled = false;
                }
            }
            else button3.Enabled = false;

            if (listBox1.Items.Count > 0)
            {
                button2.Enabled = true;
                button7.Enabled = mFormMain.lstPersoneChange.Count > 0 && comboBox1.SelectedIndex == 0 && tabControl1.SelectedIndex == 1;
            }
            else
            {
                button2.Enabled = false;
                button7.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String sMsgTextToSend = "";
            String sMsgRecToSave = "";
            if (tabControl1.SelectedIndex == 0)
            {
                if ((listBox1.SelectedIndex >= 0) && (textBox1.Text.Length > 0))
                {
                    sMsgTextToSend = textBox1.Text.Trim();
                }
            }
            else
            {
                if ((listBox1.SelectedIndex >= 0) && (listBoxOutMsg.Items.Count > 0))
                {
                    sMsgTextToSend = lstEQOutMessagesList[0];
                    sMsgTextToSend = sMsgTextToSend.Substring(sMsgTextToSend.IndexOf("|@!") + 3);
                    if (sMsgTextToSend.IndexOf("|!*#0") >= 0)
                        sMsgTextToSend = sMsgTextToSend.Substring(0, sMsgTextToSend.IndexOf("|!*#0"));
                    sMsgRecToSave = lstEQOutMessagesList[0];
                }
            }
            if (sMsgTextToSend.Length > 0)
            {
                String srec = listBox1.Items[listBox1.SelectedIndex].ToString();
                String sUID = srec.Substring(0, srec.IndexOf(" "));
                String suname = srec.Substring(srec.IndexOf(" - ") + 3);
                //if (sMsgRecToSave.Length>0)
                //    SaveLastMessage(sUID, sMsgRecToSave);
                //lstOutgoingMessages_New(sUID, suname, sMsgTextToSend);
                lstReceivedMessages_New(sUID, suname, sMsgTextToSend, sMsgRecToSave);
                DeleteContacterLastMessage(sUID);
                if (comboBoxAlgSetAction.SelectedIndex == 1)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgSetList.SelectedItem;
                    if (dbr != null)
                        SaveAlgorithmToContacter(sUID, dbr.ID);
                }

                //mFormMain.timerPhysicalSendStart();
            }

            DialogResult = DialogResult.OK;
        }

        private void ContactsList_AddUserToVisualList(String sUID, String sUName, int iVLIdx = -1)
        {
            if (iVLIdx >= 0)
                listBox1.Items[iVLIdx] = sUID + " - " + sUName;
            else
                listBox1.Items.Add(sUID + " - " + sUName);
        }

        private void ContactsList_Load()
        {
            lstContactsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_initdialog_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_initdialog_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                lstContactsList = new List<String>(srcFile);
            }
        }

        private String SetMessageFields(String message, String uname, String uid)
        {
            String retval = message;
            String firstname = "", lastname = "";
            int delim = uname.IndexOf(" ");
            if (delim > 0)
            {
                firstname = uname.Substring(0, delim);
                lastname = uname.Substring(delim + 1);
            }
            else
                firstname = uname;
            retval = retval.Replace("#firstname#", firstname);
            retval = retval.Replace("#lastname#", lastname);
            retval = retval.Replace("#contacter_id#", uid);

            return retval;
        }

        private void lstOutgoingMessages_New(String uid, String uname, String text)
        {
            mFormMain.lstOutgoingMessages.Add("*#|" + comboBox1.SelectedIndex.ToString() + "|" + uid + "|" + uname + "|" + SetMessageFields(text.Trim(), uname, uid));
        }

        private void lstReceivedMessages_New(String uid, String uname, String text, String msgtosend)
        {
            if (comboBox1.SelectedIndex == 0 && tabControl1.SelectedIndex == 1)
            {
                DateTime dt = DateTime.Now;
                if (INIT_PERSONE_DIALOG)
                    mFormMain.lstReceivedMessages.Insert(1, "0|" + uid + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "INIT_DIALOG|" + SetMessageFields(text.Trim(), uname, uid) + "|" + msgtosend);
                else
                    mFormMain.lstReceivedMessages.Insert(0, "0|" + uid + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "INIT_DIALOG|" + SetMessageFields(text.Trim(), uname, uid) + "|" + msgtosend);
            }
            else
            {
                mFormMain.lstOutgoingMessages.Add("*#|" + comboBox1.SelectedIndex.ToString() + "|" + uid + "|" + uname + "|" + SetMessageFields(text.Trim(), uname, uid));
            }
        }

        private void SaveAlgorithmToContacter(String suid, int algid)
        {
            List<String> lstTS = new List<String>();
            lstTS.Add(algid.ToString());

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + suid + ".txt"), lstTS, Encoding.UTF8);
        }

        private void DeleteContacterLastMessage(String suid)
        {
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + suid + ".txt")))
                File.Delete(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + suid + ".txt"));
        }

        private void setPersoneDialogsInit(String sPersoneID, long dialogsInit)
        {
            List<String> lstContHar = new List<String>();
            lstContHar.Add(DateTime.Now.ToBinary().ToString());
            lstContHar.Add(dialogsInit.ToString());

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_dialogsinitperday_" + FormMain.getSocialNetworkPrefix() + sPersoneID + ".txt"), lstContHar, Encoding.UTF8);
        }

        private long getPersoneDialogsInit(String sPersoneID)
        {
            long dialogsInit = 0;
            DateTime dtSaved = DateTime.Now;

            if (File.Exists(Path.Combine(FormMain.sDataPath, "_dialogsinitperday_" + FormMain.getSocialNetworkPrefix() + sPersoneID + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_dialogsinitperday_" + FormMain.getSocialNetworkPrefix() + sPersoneID + ".txt"));
                List<String> lstList = new List<String>(srcFile);

                dtSaved = lstList.Count > 0 ? DateTime.FromBinary(Convert.ToInt64(lstList[0])) : dtSaved;
                dialogsInit = lstList.Count > 1 ? Convert.ToInt64(lstList[1]) : dialogsInit;

            }

            if (!DateTime.Now.Date.Equals(dtSaved.Date))
                return 0;

            return dialogsInit;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int iMaxContacterCounter = (int)numericUpDown1.Value;
            int iContacterCounter = 0;
            long dialogsInited = getPersoneDialogsInit(iPersUserID.ToString());

            if (tabControl1.SelectedIndex == 0)
            {
                if ((listBox1.Items.Count > 0) && (textBox1.Text.Length > 0))
                {
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        if (dialogsInited >= (long)(numericUpDownInitDialogPerPersoneLimit.Value))
                            break;

                        dialogsInited++;

                        String srec = listBox1.Items[i].ToString();
                        String sUID = srec.Substring(0, srec.IndexOf(" "));
                        String suname = srec.Substring(srec.IndexOf(" - ") + 3);
                        //lstOutgoingMessages_New(sUID, suname, textBox1.Text);
                        lstReceivedMessages_New(sUID, suname, textBox1.Text, "");
                        DeleteContacterLastMessage(sUID);
                        if (comboBoxAlgSetAction.SelectedIndex == 1)
                        {
                            AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgSetList.SelectedItem;
                            if (dbr != null)
                                SaveAlgorithmToContacter(sUID, dbr.ID);
                        }
                        iContacterCounter++;
                        if (iContacterCounter >= iMaxContacterCounter)
                            break;
                    }
                    //mFormMain.timerPhysicalSendStart();
                }
            }
            else
            {
                if (listBox1.Items.Count > 0)
                {
                    bool bSendMsg = false;
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        if (dialogsInited >= (long)(numericUpDownInitDialogPerPersoneLimit.Value))
                            break;

                        dialogsInited++;

                        String srec = listBox1.Items[i].ToString();
                        String sUID = srec.Substring(0, srec.IndexOf(" "));
                        String suname = srec.Substring(srec.IndexOf(" - ") + 3);
                        LoadContactParamersValues(sUID);
                        SetEQOutMessageListOut(sCurrentEQOutMessageRecordOut);
                        if (listBoxOutMsg.Items.Count > 0)
                        {
                            String sMsgTextToSend = lstEQOutMessagesList[0];
                            sMsgTextToSend = sMsgTextToSend.Substring(sMsgTextToSend.IndexOf("|@!") + 3);
                            if (sMsgTextToSend.IndexOf("|!*#0") >= 0)
                                sMsgTextToSend = sMsgTextToSend.Substring(0, sMsgTextToSend.IndexOf("|!*#0"));
                            if (sMsgTextToSend.Length > 0)
                            {
                                //SaveLastMessage(sUID, lstEQOutMessagesList[0]);
                                //lstOutgoingMessages_New(sUID, suname, sMsgTextToSend);
                                lstReceivedMessages_New(sUID, suname, sMsgTextToSend, lstEQOutMessagesList[0]);
                                DeleteContacterLastMessage(sUID);
                                if (comboBoxAlgSetAction.SelectedIndex == 1)
                                {
                                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgSetList.SelectedItem;
                                    if (dbr != null)
                                        SaveAlgorithmToContacter(sUID, dbr.ID);
                                }
                                bSendMsg = true;

                                iContacterCounter++;
                                if (iContacterCounter >= iMaxContacterCounter)
                                    break;
                            }
                        }

                    }

                    //if (bSendMsg)
                    //    mFormMain.timerPhysicalSendStart();
                }
            }

            setPersoneDialogsInit(iPersUserID.ToString(), dialogsInited);

            if (!INIT_PERSONE_DIALOG)
                DialogResult = DialogResult.OK;
        }

        private void SaveLastMessage(String sUID, String sMsgRecord)
        {
            if (sUID.Length > 0)
            {
                File.Delete(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                if (sMsgRecord.Length > 0)
                {
                    List<String> lstTS = new List<String>();
                    lstTS.Add(sMsgRecord);
                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstTS, Encoding.UTF8);
                }
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
            for (int i = 0; i < iContHarCount; i++)
                for (int j = 0; j < iContHarAttrCount + 1; j++)
                    fe.sPersHar[i, j] = sContHar[i, j];

            fe.iPersHarAttrCount = iContHarAttrCount;
            fe.iPersHarCount = iContHarCount;
            fe.sFilePrefix = "cont";
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                String[] RQV = new String[iContHarCount];
                for (int iii = 0; iii < iContHarCount; iii++)
                {
                    RQV[Convert.ToInt32(fe.sPersHar[iii, 0]) - 1] = fe.sPersHar[iii, iContHarAttrCount];
                    sContHar[iii, iContHarAttrCount] = fe.sPersHar[iii, iContHarAttrCount];
                }

                button2.Enabled = false;
                button7.Enabled = false;
                button5.Enabled = false;

                Cursor = Cursors.WaitCursor;
                listBox1.SelectedIndex = -1;
                listBox1.BeginUpdate();
                listBox1.Items.Clear();

                Boolean bRQVEmpty = true;
                for (int iv = 0; iv < RQV.Length; iv++)
                {
                    if (RQV[iv].Length > 0)
                    {
                        bRQVEmpty = false;
                        break;
                    }
                }

                for (int i = 0; i < lstContactsList.Count; i++)
                {

                    String value = lstContactsList[i];
                    String sUID = value.Substring(0, value.IndexOf("|")); // usrID
                    value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                    String sUName = value;

                    Boolean bEquals = bRQVEmpty;
                    if (!bEquals)
                    {
                        if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                        {
                            List<String> lstContHarValues = new List<String>();
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                            lstContHarValues = new List<String>(srcFile);
                            String[] EQV = new String[iContHarCount];
                            foreach (String str in lstContHarValues)
                            {
                                if (str == null)
                                    continue;

                                if (str.Length == 0)
                                    continue;

                                EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                            }

                            bEquals = true;
                            for (int iv = 0; iv < RQV.Length; iv++)
                            {
                                if (RQV[iv].Length == 0/* || EQV[iv].Length == 0*/)
                                    continue;
                                if (!RQV[iv].Equals(EQV[iv]))
                                {
                                    bEquals = false;
                                    break;
                                }
                            }
                        }
                        else
                            bEquals = false;
                    }

                    if (bEquals)
                        ContactsList_AddUserToVisualList(sUID, sUName);
                }
                listBox1.EndUpdate();
                Cursor = Cursors.Arrow;
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                int iSelIdx = listBox1.SelectedIndex;
                listBox1.SelectedIndex = -1;
                listBox1.Items.RemoveAt(iSelIdx);

                if (listBox1.Items.Count > 0)
                {
                    if (iSelIdx >= listBox1.Items.Count)
                        iSelIdx--;
                    listBox1.SelectedIndex = iSelIdx;
                }
                else
                {
                    button2.Enabled = false;
                    button7.Enabled = false;
                    button3.Enabled = false;
                    button5.Enabled = false;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string value = "";
            if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Добавление Контактера в список"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Введите ID Контактера:"), ref value) == DialogResult.OK)
            {
                String sUID = value;
                /*
                do
                {
                    try
                    {
                        VkNet.Model.User usrAdr = FormMain.api.Users.Get(Convert.ToInt64(sUID));
                        String sUName = usrAdr.FirstName + " " + usrAdr.LastName + (usrAdr.Nickname != null ? " (" + usrAdr.Nickname + ")" : "");
                        ContactsList_AddUserToVisualList(sUID, sUName);
                        break;
                    }
                    catch (Exception exp)
                    {
                        ExceptionToLogList("FormInitContactDialog.button6_Click", sUID, exp);
                        MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Ошибка запроса Контактера с указанным ID..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Добавление Контактера в список"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    finally { }
                }
                while (true);
                */
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if ((listBox1.SelectedIndex >= 0) && (textBox1.Text.Length > 0)) button3.Enabled = true; else button3.Enabled = false;
                if ((listBox1.Items.Count > 0) && (textBox1.Text.Length > 0))
                {
                    button2.Enabled = true;
                    button7.Enabled = mFormMain.lstPersoneChange.Count > 0 && comboBox1.SelectedIndex == 0 && tabControl1.SelectedIndex == 1;
                }
                else
                {
                    button2.Enabled = false;
                    button7.Enabled = false;
                }
            }
            else
            {
                if (listBox1.Items.Count > 0)
                {
                    button2.Enabled = true;
                    button7.Enabled = mFormMain.lstPersoneChange.Count > 0 && comboBox1.SelectedIndex == 0 && tabControl1.SelectedIndex == 1;
                }
                else
                {
                    button2.Enabled = false;
                    button7.Enabled = false;
                }

                if (listBoxOutMsg.Items.Count > 0)
                {
                    if (listBox1.SelectedIndex >= 0) button3.Enabled = true; else button3.Enabled = false;
                }
                else
                {
                    button3.Enabled = false;
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0) button9.Enabled = true; else button9.Enabled = false;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                button9_Click(null, null);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                int iSelIdx = 0;
                if (listBox1.SelectedIndex > 0)
                    iSelIdx = listBox1.SelectedIndex + 1;

                if (iSelIdx >= listBox1.Items.Count)
                    iSelIdx = 0;

                int iSelIdxStart = iSelIdx;
                bool bNotFound = true;
                String[] RQV = textBox2.Text.ToLower().Trim().Split(' ');
                int RQVwc = RQV.Length;
                if (RQVwc == 0)
                    return;

                do
                {
                    String srec = listBox1.Items[iSelIdx].ToString();
                    String sUID = srec.Substring(0, srec.IndexOf(" "));
                    String suname = srec.Substring(srec.IndexOf(" - ") + 3);

                    String[] EQV = suname.ToLower().Trim().Split(' ');
                    int EQVwc = EQV.Length;

                    if (EQVwc >= RQVwc)
                    {
                        for (int i = 0; i < RQVwc; i++)
                        {
                            int j = i;
                            int iFC = 0;
                            int k = 0;
                            do
                            {
                                String sQWord = RQV[j];
                                while (k < EQVwc)
                                {
                                    String sTWord = EQV[k];
                                    if (sTWord.StartsWith(sQWord))
                                    {
                                        iFC++;
                                        k++;
                                        break;
                                    }
                                    k++;
                                }

                                if ((k >= EQVwc) || (iFC == RQVwc))
                                    break;

                                j++;
                                if (j >= RQVwc)
                                    j = 0;
                            }
                            while (j != i);

                            if (iFC == RQVwc)
                            {
                                bNotFound = false;
                                break;
                            }
                        }
                    }
                    if (bNotFound)
                    {
                        iSelIdx++;
                        if (iSelIdx >= listBox1.Items.Count)
                            iSelIdx = 0;

                        if (iSelIdxStart == iSelIdx)
                            break;
                    }
                }
                while (bNotFound);

                if (!bNotFound)
                {
                    listBox1.SelectedIndex = iSelIdx;
                    //listBox1.EnsureVisible(iSelIdx);
                }
            }

        }

        private void comboBoxAlgSetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgSetList.SelectedIndex >= 0)
            {
                Cursor = Cursors.WaitCursor;
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgSetList.Items[comboBoxAlgSetList.SelectedIndex];
                mFormMain.applyAlgorithm(dbr.ID);
                if (lblEQOutHarValues[0] != null)
                    setEQOutHarValues(0, dbr.DefaultAlgorithmTheme, false);
                listBox1_SelectedIndexChanged(null, null);

                iCompareVectorsKoefOut = new int[FormMain.iMsgHarCount];
                for (int i = 0; i < FormMain.iMsgHarCount; i++)
                {
                    iCompareVectorsKoefOut[i] = (mFormMain.adbrCurrent.ID >= 0 ? mFormMain.adbrCurrent.iMsgHarKoef[i] : mFormMain.iMsgHarKoef[i]);
                    if (lblEQOutHarNames[i] != null)
                        lblEQOutHarNames[i].Text = mFormMain.sMsgHar[i, 1] + " (" + FormMain.MsgKoefToString(iCompareVectorsKoefOut[i]) + ")";
                }
                if (lblEQOutHarNames[0] != null)
                    SetEQOutMessageListOut(sCurrentEQOutMessageRecordOut);

                NilsaUtils.SaveLongValue(2, dbr.ID);
                Cursor = Cursors.Arrow;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "Вы уверены, что хотите инициировать диалог с указанными настройками от всех Персонажей в ротации?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_7", this.Name, "Инициация диалога от всех Персонажей в ротации"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (string sUID in mFormMain.lstPersoneChange)
                {
                    DateTime dt = DateTime.Now;
                    if (sUID == iPersUserID.ToString())
                    {
                        mFormMain.lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "INIT_PERSONE_DIALOG");
                    }
                    else
                    {
                        List<String> lstReceivedMessagesTemp = new List<String>();
                        if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix() + sUID + "_contacter" + ".txt")))
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix() + sUID + "_contacter" + ".txt"));
                            lstReceivedMessagesTemp = new List<String>(srcFile);
                        }
                        int ilstpos = 0;
                        while (ilstpos < lstReceivedMessagesTemp.Count)
                        {
                            if (lstReceivedMessagesTemp[ilstpos].IndexOf("|INIT_PERSONE_DIALOG") >= 0)
                                lstReceivedMessagesTemp.RemoveAt(ilstpos);
                            else
                                ilstpos++;
                        }
                        lstReceivedMessagesTemp.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "INIT_PERSONE_DIALOG");
                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix() + sUID + "_contacter" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                    }
                }
                DialogResult = DialogResult.OK;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            NilsaUtils.SaveLongValue(4, (long)(numericUpDown1.Value));
        }

        private void timerClose_Tick(object sender, EventArgs e)
        {
            timerClose.Enabled = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            NilsaUtils.SaveLongValue(117, (long)(numericUpDownInitDialogPerPersoneLimit.Value));
        }
    }
}
