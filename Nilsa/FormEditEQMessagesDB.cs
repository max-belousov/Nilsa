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
using System.Collections;
using SourceGrid;

namespace Nilsa
{
    public partial class FormEditEQMessagesDB : Form
    {
        FormMain mFormMain;

        public String[,] sMsgHar;
        public int iMsgHarCount = 16;
        public int iMsgHarAttrCount = 4;

        List<String> lstErrorsLogList;
        String slstErrorsInit;
        public long iPersUserID;
        public String SelectedMsgRecord;

        public Boolean bNeedSetMessage;
        bool mInMsgBase;

        public List<String> lstEQMessagesDB;
        public HashSet<String> hashsetEQMessagesDB;
        public int mSettingValueID;
        public String mFileNamePrefix;
        public bool mNeedSaveEQMessageDB;
        public String mTitleSuffix;
        public String mCSVPrefix;


        public FormEditEQMessagesDB(FormMain _formmain, bool inMsgBase)
        {
            //Width=this.Size = Screen.PrimaryScreen.WorkingArea.Size
            mInMsgBase = inMsgBase;
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
            mNeedSaveEQMessageDB = false;

            this.Text = "Редактирование базы Сообщений ";

            if (mInMsgBase)
            {
                mTitleSuffix = "Контактера";
                lstEQMessagesDB = mFormMain.lstEQInMessagesDB;
                hashsetEQMessagesDB = mFormMain.hashsetEQInMessagesDB;
                mSettingValueID = 6;
                mFileNamePrefix = "In";
                changeDelimitersToTilda.Visible = true;
                mCSVPrefix = "in";
            }
            else
            {
                mTitleSuffix = "Персонажа";
                lstEQMessagesDB = mFormMain.lstEQOutMessagesDB;
                hashsetEQMessagesDB = mFormMain.hashsetEQOutMessagesDB;
                mSettingValueID = 7;
                mFileNamePrefix = "Out";
                changeDelimitersToTilda.Visible = false;
                mCSVPrefix = "out";
            }
            this.Text += mTitleSuffix;

            //this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
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

        private void setListCounter()
        {
            groupBox1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Сообщения") + " (" + (gridCheckedCount > 0 ? gridCheckedCount.ToString() + " / " : "") + (grid1.RowsCount - 1).ToString() + ")";
        }

        int gridCheckedCount;
        int gridSelectedIndex;
        HashSet<int> gridCheckedIndexes;
        const int ATTRIBUTES_COLUMN_OFFS = 1;
        const int ATTRIBUTES_COLUMN_TEXT = 0;
        int ATTRIBUTES_COLUMN_MARKER;

        private void initGrid(bool bFirstInit)
        {
            mNotLockItemChecked = true;
            gridCheckedCount = 0;
            gridCheckedIndexes = new HashSet<int>();
            if (bFirstInit)
                itemCheckedChangedEvent = new ItemCheckedChangedEvent(this);
            ATTRIBUTES_COLUMN_MARKER = iMsgHarCount + ATTRIBUTES_COLUMN_OFFS;
            grid1.RowsCount = 1;
            grid1.ColumnsCount = 3 + iMsgHarCount;
            grid1.FixedRows = 1;
            grid1.FixedColumns = 1;
            grid1.SelectionMode = SourceGrid.GridSelectionMode.Row;
            grid1.Selection.EnableMultiSelection = false;
            grid1.AutoStretchColumnsToFitWidth = false;
            grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            grid1.Columns[0].Width = 25;
            grid1.Columns[0].MaximalWidth = 25;
            grid1.Columns[0].MinimalWidth = 25;
            grid1.SelectionMode = GridSelectionMode.Row;
            if (bFirstInit)
                grid1.Selection.SelectionChanged += Selection_SelectionChanged;

            SourceGrid.Cells.Header l_00Header = new SourceGrid.Cells.Header(null);
            grid1[0, 0] = new SourceGrid.Cells.ColumnHeader("*");

            grid1[0, 1] = new SourceGrid.Cells.ColumnHeader("Текст");

            for (int i = 0; i < iMsgHarCount; i++)
                grid1[0, ATTRIBUTES_COLUMN_OFFS + i + 1] = new SourceGrid.Cells.ColumnHeader(i.ToString());

            grid1[0, ATTRIBUTES_COLUMN_MARKER + 1] = new SourceGrid.Cells.ColumnHeader("Маркер");

        }

        private void Selection_SelectionChanged(object sender, RangeRegionChangedEventArgs e)
        {
            gridSelectedIndex = -1;

            if (e.AddedRange != null)
            {
                if (grid1.Selection.IsSelectedRow(0))
                    grid1.Selection.SelectRow(0, false);
            }

            if (grid1.Selection.ActivePosition != null)
            {
                SourceGrid.RangeRegion rangeRegion = grid1.Selection.GetSelectionRegion();
                int[] indexes = rangeRegion.GetRowsIndex();
                if (indexes.Length > 1)
                    gridSelectedIndex = indexes[0];
                else
                {
                    if (indexes.Length > 0)
                    {
                        if (indexes[0] != 0)
                            gridSelectedIndex = indexes[0];
                    }
                }
            }

            grid_SelectedIndexChanged();
        }

        private void grid_SelectedIndexChanged()
        {
            bool bBS = gridSelectedIndex > 0;
            var selectedMsg = MessagesDB_GetFromVisualList(gridSelectedIndex);
            //MessageBox.Show(selectedMsg);
            button2.Enabled = bBS || gridCheckedCount > 0;
            button5.Enabled = bBS || gridCheckedCount > 0;
            button7.Enabled = bBS || gridCheckedCount > 0;
            button3.Enabled = bBS && (mFormMain.iContUserID >= 0 || selectedMsg.Contains("authoriz") || selectedMsg.Contains("Authoriz") || selectedMsg.Contains("update")); 
        }

        private void grid_ItemChecked()
        {
            if (gridCheckedCount > 0)
            {
                button12.Enabled = true;
                button8.Enabled = true;
                buttonExportCSVToServer.Enabled = true;
            }
            else
            {
                button12.Enabled = false;
                button8.Enabled = false;
                buttonExportCSVToServer.Enabled = false;
            }

            if (grid1.RowsCount - 1 > 0 && gridCheckedCount != grid1.RowsCount - 1)
            {
                button10.Enabled = true;
                button13.Enabled = true;
            }
            else
            {
                button10.Enabled = false;
                button13.Enabled = false;
            }

            setListCounter();
            button2.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            button5.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            button7.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;

        }

        
        public void Setup(String sErrorHdr, long iUsrID, String sSelMsg = "", bool bFirstInit=true)
        {
            initGrid(bFirstInit);

            SelectedMsgRecord = sSelMsg;
            bNeedSetMessage = false;
            slstErrorsInit = sErrorHdr;
            iPersUserID = iUsrID;
            LoadErrorsLogList();

            button2.Enabled = false;
            button5.Enabled = false;
            button3.Enabled = false;
            button7.Enabled = false;

            Cursor = Cursors.WaitCursor;

            if (iMsgHarCount <= 16)
            {
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    grid1.Columns[ATTRIBUTES_COLUMN_OFFS + i + 1].Width = 100;
                    grid1[0, ATTRIBUTES_COLUMN_OFFS + i + 1].Value = sMsgHar[i, 1];
                }
            }

            LoadColumnsOrderAndWidth();

            if (NilsaUtils.LoadLongValue(mSettingValueID, 0) == 1)
                ApplyFilter();
            else
            {
                for (int i = 0; i < lstEQMessagesDB.Count; i++)
                {

                    String value = lstEQMessagesDB[i];
                    String sUID = value.Substring(0, value.IndexOf("|")); // usrID
                    value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                    String sUName = value;
                    MessagesDB_AddToVisualList(lstEQMessagesDB[i]);
                }

                button4.Image = Nilsa.Properties.Resources.filter_list_on;
            }

            int iSelIdx = 1;
            for (int i = 1; i < grid1.RowsCount; i++)
            {
                if (SelectedMsgRecord.Equals(MessagesDB_GetFromVisualList(i)))
                    iSelIdx = i;
            }

            Cursor = Cursors.Arrow;
            selectRow(iSelIdx);
            grid_ItemChecked();
        }

        private void selectRow(int iSelIdx)
        {
            if (grid1.RowsCount > 1)
            {
                if (iSelIdx > 0)
                {
                    grid1.Selection.ResetSelection(true);
                    grid1.ShowCell(new Position(iSelIdx, 0), false);
                    grid1.Selection.SelectRow(iSelIdx, true);
                    grid1.Selection.FocusRow(iSelIdx);
                }
                else
                {
                    grid1.Selection.ResetSelection(true);
                    grid1.ShowCell(new Position(1, 0), false);
                    grid1.Selection.SelectRow(1, true);
                    grid1.Selection.FocusRow(1);
                }
            }
        }

        private void SaveFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            for (int i = 0; i < iMsgHarCount; i++)
                lstContHar.Add(fe.sPersHar[i, iMsgHarAttrCount]);

            lstContHar.Add(fe.tbKeywords.Text);
            lstContHar.Add(fe.cbKeywords.SelectedIndex.ToString());

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesEQ" + mFileNamePrefix + "_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"), lstContHar, Encoding.UTF8);
        }

        private void LoadFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesEQ" + mFileNamePrefix + "_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesEQ" + mFileNamePrefix + "_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"));
                lstContHar = new List<String>(srcFile);
            }

            if (lstContHar.Count > 0)
            {
                if (lstContHar.Count == 6)
                {
                    for (int i = 0; i < iMsgHarCount; i++)
                    {

                        if (i < 4)
                            fe.sPersHar[i, iMsgHarAttrCount] = lstContHar[i];
                        else
                            fe.sPersHar[i, iMsgHarAttrCount] = ""; 
                    }

                            fe.tbKeywords.Text = "";
                            fe.cbKeywords.SelectedIndex = 0;
                }
                else
                {
                    for (int i = 0; i < iMsgHarCount; i++)
                    {

                        if (i < lstContHar.Count)
                            fe.sPersHar[i, iMsgHarAttrCount] = lstContHar[i];
                    }

                    if (lstContHar.Count > iMsgHarCount)
                    {
                        if (iMsgHarCount + 1 < lstContHar.Count)
                        {
                            fe.tbKeywords.Text = lstContHar[iMsgHarCount];
                            fe.cbKeywords.SelectedIndex = Convert.ToInt32(lstContHar[iMsgHarCount + 1]);
                        }
                    }
                }
            }
        }

        private void ApplyFilter(bool bNotShowDialog = true)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sPersHar[i, j] = sMsgHar[i, j];
                fe.sPersHar[i, iMsgHarAttrCount] = sMsgHar[i, iMsgHarAttrCount];
            }
            LoadFormEditPersHarValues(fe, 100001, 1);

            fe.iPersHarAttrCount = iMsgHarAttrCount;
            fe.iPersHarCount = iMsgHarCount;
            fe.sFilePrefix = "msg";
            fe.Setup(true);

            if (bNotShowDialog || fe.ShowDialog() == DialogResult.OK)
            {
                SaveFormEditPersHarValues(fe, 100001, 1);
                String[] RQV = new String[iMsgHarCount];
                for (int iii = 0; iii < iMsgHarCount; iii++)
                {
                    RQV[iii] = fe.sPersHar[iii, iMsgHarAttrCount].Trim().ToLower();
                    sMsgHar[iii, iMsgHarAttrCount] = fe.sPersHar[iii, iMsgHarAttrCount];
                }

                FilterList(RQV, fe.tbKeywords.Text, fe.cbKeywords.SelectedIndex == 0);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text.Equals(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Отключить фильтр")))
            {
                FilterList(ClearFilter(), "", true);
                NilsaUtils.SaveLongValue(mSettingValueID, 0);
            }
            else
            {
                ApplyFilter(false);
                NilsaUtils.SaveLongValue(mSettingValueID, 1);
            }

        }

        private String[] ClearFilter()
        {
            String[] RQV = new String[iMsgHarCount];
            for (int iii = 0; iii < iMsgHarCount; iii++)
            {
                RQV[iii] = "";
                sMsgHar[iii, iMsgHarAttrCount] = "";
            }

            return RQV;
        }

        private bool TextSearchFilterNew_FindWord(String sSrc, String sWord)
        {
            int iPos = 0;
            int nWordLen = sWord.Length;
            int nSrcLen = sSrc.Length;

            if (nWordLen == 0)
                return false;

            bool bRightDoneStar = false;
            if (sWord[nWordLen - 1] == '*')
            {
                bRightDoneStar = true;
                sWord = sWord.Substring(0, nWordLen - 1);
                nWordLen--;
            }

            if (nWordLen == 0)
                return false;

            while ((iPos = sSrc.IndexOf(sWord, iPos)) >= 0)
            {
                bool bLeftDone = iPos == 0;
                if (!bLeftDone)
                    bLeftDone = !(Char.IsDigit(sSrc[iPos - 1]) || Char.IsLetter(sSrc[iPos - 1]));

                if (bLeftDone)
                {
                    bool bRightDone = (iPos + nWordLen >= nSrcLen) || bRightDoneStar;

                    if (!bRightDone)
                        bRightDone = !(Char.IsDigit(sSrc[iPos + nWordLen]) || Char.IsLetter(sSrc[iPos + nWordLen]));

                    if (bRightDone)
                        return true;
                }
                iPos++;

                if (iPos >= nSrcLen)
                    return false;
            }
            return false;
        }

        private void FilterList(String[] RQV, String skeywords, bool bLogic)
        {
            button2.Enabled = false;
            button5.Enabled = false;
            button3.Enabled = false;
            button7.Enabled = false;

            Cursor = Cursors.WaitCursor;
            grid1.Redim(1, grid1.ColumnsCount);
            gridCheckedCount = 0;
            gridCheckedIndexes.Clear();

            String[] keywords = skeywords.ToLower().Trim().Split(',');
            Boolean bRQVEmpty = true;
            for (int iv = 0; iv < RQV.Length; iv++)
            {
                if (RQV[iv].Length > 0)
                {
                    bRQVEmpty = false;
                    break;
                }
            }

            List<String> lstEQMessagesList = new List<String>();
            bool bFiltered = false;
            foreach (String EQ in lstEQMessagesDB)
            {
                Boolean bEquals = bRQVEmpty;

                bool bKeywords = bLogic;
                String msgText = EQ.Substring(EQ.IndexOf("|@!") + 3).ToLower();
                foreach (string keyword in keywords)
                {
                    if (keyword.Length == 0)
                        continue;

                    if (bLogic)
                    {
                        if (!TextSearchFilterNew_FindWord(msgText, keyword.Trim()))
                        {
                            bKeywords = false;
                            break;
                        }
                    }
                    else
                    {
                        if (TextSearchFilterNew_FindWord(msgText, keyword.Trim()))
                        {
                            bKeywords = true;
                            break;
                        }
                    }
                }

                if (!bEquals && bKeywords)
                {
                    //double dW = 0;

                    String EQText = EQ.Substring(EQ.IndexOf("|") + 1);
                    EQText = EQText.Substring(0, EQText.IndexOf("|@!"));
                    String[] OQV = EQText.ToLower().Split('|');
                    bEquals = true;

                    for (int iv = 0; iv < RQV.Length; iv++)
                    {
                        if (RQV[iv].Length == 0/* || OQV[iv].Length == 0*/)
                            continue;

                        bool bEqualsValues = false;

                        if (RQV[iv].Equals(OQV[iv]) || OQV[iv].StartsWith(RQV[iv]))
                            bEqualsValues = true;//dW += mFormMain.iMsgHarKoef[iv];
                        else
                        {
                            //+---
                            if (RQV[iv].IndexOf("~") >= 0 || RQV[iv].IndexOf("|") >= 0)
                            {
                                //bEquals = false;
                                String sFilter = RQV[iv];
                                bool bInverse = false;

                                String sValue = OQV[iv].Trim();
                                if (sFilter[0] == '~')
                                {
                                    bInverse = true;
                                    bEqualsValues = true;
                                    sFilter = sFilter.Substring(1);
                                }

                                if (sFilter.Length > 0)
                                {
                                    if (sFilter[sFilter.Length - 1] != '|')
                                        sFilter += "|";

                                    while (sFilter.Length > 0)
                                    {
                                        String sPart = sFilter.Substring(0, sFilter.IndexOf("|")).Trim();
                                        sFilter = sFilter.Substring(sFilter.IndexOf("|") + 1).Trim();
                                        if (!bInverse)
                                        {
                                            if (sPart.Equals(sValue))
                                            {
                                                bEqualsValues = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (sPart.Equals(sValue))
                                            {
                                                bEqualsValues = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                            }
                            else if (RQV[iv].IndexOf("-") >= 0)
                            {
                                bEqualsValues = true;
                                String sval = RQV[iv].Trim();
                                String svalMin = sval.Substring(0, sval.IndexOf("-")).Trim();
                                String svalMax = sval.Substring(sval.IndexOf("-") + 1).Trim();
                                long ivalmin = 0;
                                long ivalmax = 9999999999;
                                try
                                {
                                    if (svalMin.Length > 0)
                                        ivalmin = Convert.ToInt32(svalMin);
                                    if (svalMax.Length > 0)
                                        ivalmax = Convert.ToInt32(svalMax);

                                    sval = OQV[iv].Trim();
                                    try
                                    {
                                        int ival = Convert.ToInt32(sval);
                                        if (ival < ivalmin || ival > ivalmax)
                                            bEqualsValues = false;
                                    }
                                    catch
                                    {
                                        bEqualsValues = false;
                                    }
                                }
                                catch
                                {
                                    bEqualsValues = false;
                                }
                            }

                            if (!bEqualsValues)
                            {
                                bEquals = false;
                                break;//dW += mFormMain.iMsgHarKoef[iv];
                            }
                            //+---
                        }

                    }
                    //dW = dW * 1000 / iMsgHarCount;
                    //if (dW > 0)
                    //{
                    //    String value = ((int)dW).ToString("000000").Substring(0, 6) + "|" + EQ.Substring(EQ.IndexOf("|") + 1);
                    //    lstEQOuttMessagesList.Add(value);
                    //}
                    //else
                    //    bFiltered = true;
                }

                if (bEquals && bKeywords)
                    lstEQMessagesList.Add(EQ);
                else
                    bFiltered = true;
            }

            lstEQMessagesList = lstEQMessagesList.OrderByDescending(i => i).ToList();

            foreach (String EQ in lstEQMessagesList)
            {
                String value = "000000|" + EQ.Substring(EQ.IndexOf("|") + 1);
                MessagesDB_AddToVisualList(value);
            }

            Cursor = Cursors.Arrow;
            selectRow(1);
            grid_ItemChecked();

            if (bFiltered)
            {
                button4.Image = Nilsa.Properties.Resources.filter_list_off;
                //button4.BackColor = SystemColors.Control;
                button4.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Отключить фильтр");
            }
            else
            {
                button4.Image = Nilsa.Properties.Resources.filter_list_on;
                //button4.BackColor = SystemColors.ControlLight;
                button4.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Фильтрация списка");
            }
        }

        private String MessagesDB_GetFromVisualList(int iVLIdx)
        {
            String sMsgRec = "";
            if (iVLIdx > 0 && iVLIdx < grid1.RowsCount && (iMsgHarCount <= 16))
            {
                sMsgRec = "000000|";
                for (int iOffs = 0; iOffs < iMsgHarCount; iOffs++)
                    sMsgRec = sMsgRec + grid1[iVLIdx, columnOrder[iOffs + ATTRIBUTES_COLUMN_OFFS]].Value + "|";
                sMsgRec = sMsgRec + "@!" + grid1[iVLIdx, columnOrder[ATTRIBUTES_COLUMN_TEXT]].Value + (grid1[iVLIdx, columnOrder[ATTRIBUTES_COLUMN_MARKER]].Value != null ? ("|!*#0" + grid1[iVLIdx, columnOrder[ATTRIBUTES_COLUMN_MARKER]].Value) : "");
            }
            return sMsgRec;
        }

        ItemCheckedChangedEvent itemCheckedChangedEvent;
        bool mNotLockItemChecked;

        public class ItemCheckedChangedEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            FormEditEQMessagesDB mForm;

            public ItemCheckedChangedEvent(FormEditEQMessagesDB _mForm)
            {
                mForm = _mForm;
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                bool bChecked = (bool)sender.Value;

                mForm.gridCheckedCount += bChecked ? 1 : -1;
                if (bChecked)
                    mForm.gridCheckedIndexes.Add(sender.Position.Row);
                else
                    mForm.gridCheckedIndexes.Remove(sender.Position.Row);

                if (mForm.mNotLockItemChecked)
                    mForm.grid_ItemChecked();
                //string val = "Value of cell {0} is '{1}'";

                //MessageBox.Show(sender.Grid, string.Format(val, sender.Position, sender.Value));
            }
        }

        private void MessagesDB_AddToVisualList(String sMsgRec, int iVLIdx = -1)
        {
            String sText, sRQ = sMsgRec;
            sRQ = sRQ.Substring(sRQ.IndexOf("|") + 1);
            sRQ = sRQ.Substring(0, sRQ.IndexOf("|@!"));
            sText = sMsgRec.Substring(sMsgRec.IndexOf("|@!") + 3);
            String sMarker = "";
            if (sText.IndexOf("|!*#0") >= 0)
            {
                sMarker = sText.Substring(sText.IndexOf("|!*#0") + 5);
                sText = sText.Substring(0, sText.IndexOf("|!*#0"));
            }

            String[] EQV = sRQ.Split('|');

            if (iVLIdx > 0)
            {
                if (iMsgHarCount <= 16)
                    for (int iOffs = 0; iOffs < iMsgHarCount; iOffs++)
                        grid1[iVLIdx, columnOrder[iOffs + ATTRIBUTES_COLUMN_OFFS]].Value = EQV[iOffs];
                grid1[iVLIdx, columnOrder[ATTRIBUTES_COLUMN_MARKER]].Value = sMarker;
                grid1[iVLIdx, columnOrder[ATTRIBUTES_COLUMN_TEXT]].Value = sText;

            }
            else
            {
                int row = grid1.RowsCount;
                grid1.Rows.Insert(row);

                grid1[row, 0] = new SourceGrid.Cells.CheckBox(null, false);
                grid1[row, 0].AddController(itemCheckedChangedEvent);
                if (iMsgHarCount <= 16)
                    for (int iOffs = 0; iOffs < iMsgHarCount; iOffs++)
                        grid1[row, columnOrder[iOffs + ATTRIBUTES_COLUMN_OFFS]] = new SourceGrid.Cells.Cell(EQV[iOffs]);
                grid1[row, columnOrder[ATTRIBUTES_COLUMN_MARKER]] = new SourceGrid.Cells.Cell(sMarker);
                grid1[row, columnOrder[ATTRIBUTES_COLUMN_TEXT]] = new SourceGrid.Cells.Cell(sText);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (gridCheckedCount <= 0)
            {
                if (gridSelectedIndex > 0)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "Вы уверены, что хотите удалить это Сообщение?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Сообщения"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        int iSelIdx = gridSelectedIndex;
                        String sMsgRec = MessagesDB_GetFromVisualList(iSelIdx);

                        grid1.Rows.Remove(iSelIdx);
                        lstEQMessagesDB.Remove(sMsgRec);
                        hashsetEQMessagesDB.Remove(sMsgRec);
                        mNeedSaveEQMessageDB = true;

                        if (grid1.RowsCount > 1)
                        {
                            if (iSelIdx >= grid1.RowsCount - 1)
                                iSelIdx--;
                            selectRow(iSelIdx);
                        }
                        else
                        {
                            button2.Enabled = false;
                            button5.Enabled = false;
                            button3.Enabled = false;
                            button7.Enabled = false;
                        }
                        grid_ItemChecked();
                    }
                }
            }
            else
            {
                if (gridCheckedCount > 0)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_7", this.Name, "Вы уверены, что хотите удалить все отмеченные Сообщения?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Удаление Сообщений"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        foreach (int idx in gridCheckedIndexes)
                        {
                            String sMsgRec = MessagesDB_GetFromVisualList(idx);
                            lstEQMessagesDB.Remove(sMsgRec);
                            hashsetEQMessagesDB.Remove(sMsgRec);
                        }
                        mNeedSaveEQMessageDB = true;

                        List<int> indexes = gridCheckedIndexes.OrderByDescending(i => i).ToList();
                        foreach (int idx in indexes)
                        {
                            grid1.Rows.Remove(idx);
                        }
                        gridCheckedIndexes.Clear();
                        gridCheckedCount = 0;

                        EnableAllButtons();
                        //FilterList(ClearFilter());
                    }
                }
            }

        }

        private List<String> GetHarValues(String sTitle)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sPersHar[i, j] = sMsgHar[i, j];
                fe.sPersHar[i, iMsgHarAttrCount] = "";
            }

            fe.iPersHarAttrCount = iMsgHarAttrCount;
            fe.iPersHarCount = iMsgHarCount;
            fe.sFilePrefix = "msg";
            fe.Text = sTitle;
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                List<String> lstContHar = new List<String>();
                for (int i = 0; i < iMsgHarCount; i++)
                    lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iMsgHarAttrCount]);
                return lstContHar;
            }
            return null;
        }

        private void EditEQMessageParametersValues(int iSelIdx = -1)
        {
            FormEditMsgValues fe = new FormEditMsgValues(mFormMain);
            fe.Text += " " + "Сообщения " + mTitleSuffix;
            fe.sPersHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sPersHar[i, j] = sMsgHar[i, j];

                if (mInMsgBase)
                    fe.sPersHar[i, iMsgHarAttrCount] = sMsgHar[i, 3];
                else
                    fe.sPersHar[i, iMsgHarAttrCount] = (iSelIdx == -1 ? (FormMain.IdealRQV.Length > 0 ? FormMain.IdealRQV[i] : sMsgHar[i, 3]) : sMsgHar[i, 3]);
            }

            if (iSelIdx == -1)
                fe.sPersHar[FormMain.MSG_ID_COLUMN, iMsgHarAttrCount] = (mInMsgBase ? FormMain.iMsgINMaxID : FormMain.iMsgOUTMaxID + 1).ToString();

            fe.iPersHarAttrCount = iMsgHarAttrCount;
            fe.iPersHarCount = iMsgHarCount;
            fe.textBox1.Text = NilsaUtils.StringToText(iSelIdx == -1 ? "" : grid1[iSelIdx, columnOrder[ATTRIBUTES_COLUMN_TEXT]].Value.ToString());
            fe.comboBox2.SelectedIndex = iSelIdx == -1 ? 0 : (grid1[iSelIdx, columnOrder[ATTRIBUTES_COLUMN_MARKER]].Value != null ? (Convert.ToInt32(grid1[iSelIdx, columnOrder[ATTRIBUTES_COLUMN_MARKER]].Value.ToString())) : 0);
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                String sMsgNewRec = "000000|";
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    sMsgHar[i, 3] = fe.sPersHar[i, iMsgHarAttrCount].Trim();
                    sMsgNewRec = sMsgNewRec + fe.sPersHar[i, iMsgHarAttrCount] + "|";
                }

                sMsgNewRec = sMsgNewRec + "@!" + NilsaUtils.TextToString(fe.textBox1.Text) + (fe.comboBox2.SelectedIndex > 0 ? ("|!*#0" + Convert.ToString(fe.comboBox2.SelectedIndex)) : "");

                if (iSelIdx >= 0)
                {
                    String sMsgOldRec = MessagesDB_GetFromVisualList(iSelIdx);
                    if (sMsgOldRec != sMsgNewRec)
                    {
                        lstEQMessagesDB.Remove(sMsgOldRec);
                        hashsetEQMessagesDB.Remove(sMsgOldRec);
                    }
                }

                if (!hashsetEQMessagesDB.Contains(sMsgNewRec))
                {
                    lstEQMessagesDB.Add(sMsgNewRec);
                    hashsetEQMessagesDB.Add(sMsgNewRec);
                    mNeedSaveEQMessageDB = true;

                    if (iSelIdx == -1)
                    {
                        if (mInMsgBase)
                        {
                            FormMain.iMsgINMaxID++;
                            NilsaUtils.SaveLongValue(0, FormMain.iMsgINMaxID);
                        }
                        else
                        {
                            FormMain.iMsgOUTMaxID++;
                            NilsaUtils.SaveLongValue(1, FormMain.iMsgOUTMaxID);
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < grid1.RowsCount; i++)
                    {
                        String sMsgRec = MessagesDB_GetFromVisualList(i);
                        if (sMsgRec == sMsgNewRec)
                        {
                            iSelIdx = i;
                            break;
                        }
                    }
                }

                MessagesDB_AddToVisualList(sMsgNewRec, iSelIdx);
                if (grid1.RowsCount > 1)
                {
                    if (iSelIdx > 0)
                        selectRow(iSelIdx);
                    else
                        selectRow(grid1.RowsCount - 1);
                }
                grid_ItemChecked();
            }

        }

        private void DisableAllButtons()
        {
            //this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            this.button6.Enabled = false;
            this.button7.Enabled = false;
            this.button9.Enabled = false;
            this.button10.Enabled = false;
            this.button12.Enabled = false;
            this.button13.Enabled = false;
            this.button8.Enabled = false;
            buttonExportCSVToServer.Enabled = false;

        }

        private void EnableAllButtons()
        {
            //this.button1.Enabled = true;
            this.button2.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            this.button3.Enabled = gridSelectedIndex > 0 && mFormMain.iContUserID >= 0;
            this.button4.Enabled = true;
            this.button5.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            this.button7.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            this.button6.Enabled = true;
            this.button9.Enabled = true;
            grid_SelectedIndexChanged();
            grid_ItemChecked();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetEQMessageParametersDefaultValues(true);
            EditEQMessageParametersValues();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (gridCheckedCount <= 0)
            {
                SetEQMessageParametersDefaultValues(false);
                if (gridSelectedIndex > 0)
                    EditEQMessageParametersValues(gridSelectedIndex);
            }
            else
            {
                if (gridCheckedCount > 0)
                {
                    List<String> lstContHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_8", this.Name, "Замена/установка значений характеристик"));
                    if (lstContHar == null)
                        return;

                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_10", this.Name, "Вы уверены, что хотите задать характеристики для всех отмеченных Сообщений?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Редактирование характеристик Сообщений"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        foreach (int k in gridCheckedIndexes)
                        {
                            String sMsgOldRec = MessagesDB_GetFromVisualList(k);
                            String sValues = sMsgOldRec.Substring(sMsgOldRec.IndexOf("|") + 1);
                            String sMsgNewRec = "000000|";
                            for (int i = 0; i < iMsgHarCount; i++)
                            {
                                String sCV = lstContHar[i].Substring(lstContHar[i].IndexOf("|") + 1);
                                String sField = sValues.Substring(0, sValues.IndexOf("|"));
                                sValues = sValues.Substring(sValues.IndexOf("|") + 1);
                                if (sCV.Length > 0)
                                {
                                    if (sCV == "#clear#")
                                        sField = "";
                                    else
                                        sField = sCV;
                                }
                                sMsgNewRec += sField + "|";
                            }
                            sMsgNewRec += sValues;

                            if (sMsgOldRec != sMsgNewRec)
                            {
                                lstEQMessagesDB.Remove(sMsgOldRec);
                                hashsetEQMessagesDB.Remove(sMsgOldRec);
                            }

                            if (!hashsetEQMessagesDB.Contains(sMsgNewRec))
                            {
                                lstEQMessagesDB.Add(sMsgNewRec);
                                hashsetEQMessagesDB.Add(sMsgNewRec);
                            }

                            MessagesDB_AddToVisualList(sMsgNewRec, k);
                        }

                        mNeedSaveEQMessageDB = true;

                        EnableAllButtons();
                    }
                }

            }
        }

        private void SetEQMessageParametersDefaultValues(bool bClear)
        {
            if (gridSelectedIndex > 0)
            {
                int iSelIdx = gridSelectedIndex;
                String value = MessagesDB_GetFromVisualList(gridSelectedIndex);
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    value = value.Substring(value.IndexOf("|") + 1);
                    String s = value.Substring(0, value.IndexOf("|"));
                    sMsgHar[i, 3] = (!bClear || i == 0) ? s : ""; // Значение атрибутов
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (gridSelectedIndex > 0)
            {
                int iSelIdx = gridSelectedIndex;
                bNeedSetMessage = true;
                SelectedMsgRecord = MessagesDB_GetFromVisualList(gridSelectedIndex);
                DialogResult = DialogResult.OK;
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0) button9.Enabled = true; else button9.Enabled = false;

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (grid1.RowsCount > 1)
            {
                int iSelIdx = 0;
                if (gridSelectedIndex > 0)
                    iSelIdx = gridSelectedIndex + 1;

                if (iSelIdx >= grid1.RowsCount - 1)
                    iSelIdx = 1;

                int iSelIdxStart = iSelIdx;
                bool bNotFound = true;
                String[] RQV = textBox1.Text.ToLower().Trim().Split(' ');
                int RQVwc = RQV.Length;
                if (RQVwc == 0)
                    return;

                do
                {
                    String[] EQV = grid1[iSelIdx, columnOrder[ATTRIBUTES_COLUMN_TEXT]].Value.ToString().ToLower().Trim().Split(' ');
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
                        if (iSelIdx >= grid1.RowsCount - 1)
                            iSelIdx = 1;

                        if (iSelIdxStart == iSelIdx)
                            break;
                    }
                }
                while (bNotFound);

                if (!bNotFound)
                {
                    selectRow(iSelIdx);
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                button9_Click(null, null);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            mNotLockItemChecked = false;
            for (int k = 1; k < grid1.RowsCount; k++)
            {
                grid1[k, 0].Value = true;
            }
            mNotLockItemChecked = true;
            grid_ItemChecked();
            Cursor = Cursors.Arrow;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            mNotLockItemChecked = false;
            for (int k = 1; k < grid1.RowsCount; k++)
            {
                if ((bool)grid1[k, 0].Value)
                    grid1[k, 0].Value = false;
            }
            mNotLockItemChecked = true;
            grid_ItemChecked();
            Cursor = Cursors.Arrow;
        }

        private void tbInvertSelection_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            mNotLockItemChecked = false;
            for (int k = 1; k < grid1.RowsCount; k++)
            {
                grid1[k, 0].Value = !((bool)grid1[k, 0].Value);
            }
            mNotLockItemChecked = true;
            grid_ItemChecked();
            Cursor = Cursors.Arrow;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sPersHar[i, j] = sMsgHar[i, j];
                fe.sPersHar[i, iMsgHarAttrCount] = sMsgHar[i, iMsgHarAttrCount];
            }

            fe.iPersHarAttrCount = iMsgHarAttrCount;
            fe.iPersHarCount = iMsgHarCount;
            fe.sFilePrefix = "msg";
            fe.Setup(true);

            if (fe.ShowDialog() == DialogResult.OK)
            {
                String[] RQV = new String[iMsgHarCount];
                for (int iii = 0; iii < iMsgHarCount; iii++)
                {
                    RQV[iii] = fe.sPersHar[iii, iMsgHarAttrCount].Trim().ToLower();
                    sMsgHar[iii, iMsgHarAttrCount] = fe.sPersHar[iii, iMsgHarAttrCount];
                }
                string skeywords = fe.tbKeywords.Text;
                bool bLogic = fe.cbKeywords.SelectedIndex == 0;

                Cursor = Cursors.WaitCursor;
                mNotLockItemChecked = false;

                String[] keywords = skeywords.ToLower().Trim().Split(',');
                Boolean bRQVEmpty = true;
                for (int iv = 0; iv < RQV.Length; iv++)
                {
                    if (RQV[iv].Length > 0)
                    {
                        bRQVEmpty = false;
                        break;
                    }
                }

                for (int k = 1; k < grid1.RowsCount; k++)
                {
                    String EQ = MessagesDB_GetFromVisualList(k);
                    Boolean bEquals = bRQVEmpty;

                    bool bKeywords = bLogic;
                    String msgText = EQ.Substring(EQ.IndexOf("|@!") + 3).ToLower();
                    foreach (string keyword in keywords)
                    {
                        if (keyword.Length == 0)
                            continue;

                        if (bLogic)
                        {
                            if (!TextSearchFilterNew_FindWord(msgText, keyword.Trim()))
                            {
                                bKeywords = false;
                                break;
                            }
                        }
                        else
                        {
                            if (TextSearchFilterNew_FindWord(msgText, keyword.Trim()))
                            {
                                bKeywords = true;
                                break;
                            }
                        }
                    }

                    if (!bEquals && bKeywords)
                    {
                        //double dW = 0;
                        String EQText = EQ.Substring(EQ.IndexOf("|") + 1);
                        EQText = EQText.Substring(0, EQText.IndexOf("|@!"));
                        String[] OQV = EQText.ToLower().Split('|');
                        bEquals = true;

                        for (int iv = 0; iv < RQV.Length; iv++)
                        {
                            if (RQV[iv].Length == 0/* || OQV[iv].Length == 0*/)
                                continue;

                            bool bEqualsValues = false;

                            if (RQV[iv].Equals(OQV[iv]) || OQV[iv].StartsWith(RQV[iv]))
                                bEqualsValues = true;//dW += mFormMain.iMsgHarKoef[iv];
                            else
                            {
                                //+---
                                if (RQV[iv].IndexOf("~") >= 0 || RQV[iv].IndexOf("|") >= 0)
                                {
                                    //bEquals = false;
                                    String sFilter = RQV[iv];
                                    bool bInverse = false;

                                    String sValue = OQV[iv].Trim();
                                    if (sFilter[0] == '~')
                                    {
                                        bInverse = true;
                                        bEqualsValues = true;
                                        sFilter = sFilter.Substring(1);
                                    }

                                    if (sFilter.Length > 0)
                                    {
                                        if (sFilter[sFilter.Length - 1] != '|')
                                            sFilter += "|";

                                        while (sFilter.Length > 0)
                                        {
                                            String sPart = sFilter.Substring(0, sFilter.IndexOf("|")).Trim();
                                            sFilter = sFilter.Substring(sFilter.IndexOf("|") + 1).Trim();
                                            if (!bInverse)
                                            {
                                                if (sPart.Equals(sValue))
                                                {
                                                    bEqualsValues = true;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (sPart.Equals(sValue))
                                                {
                                                    bEqualsValues = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                }
                                else if (RQV[iv].IndexOf("-") >= 0)
                                {
                                    bEqualsValues = true;
                                    String sval = RQV[iv].Trim();
                                    String svalMin = sval.Substring(0, sval.IndexOf("-")).Trim();
                                    String svalMax = sval.Substring(sval.IndexOf("-") + 1).Trim();
                                    long ivalmin = 0;
                                    long ivalmax = 9999999999;
                                    try
                                    {
                                        if (svalMin.Length > 0)
                                            ivalmin = Convert.ToInt32(svalMin);
                                        if (svalMax.Length > 0)
                                            ivalmax = Convert.ToInt32(svalMax);

                                        sval = OQV[iv].Trim();
                                        try
                                        {
                                            int ival = Convert.ToInt32(sval);
                                            if (ival < ivalmin || ival > ivalmax)
                                                bEqualsValues = false;
                                        }
                                        catch
                                        {
                                            bEqualsValues = false;
                                        }
                                    }
                                    catch
                                    {
                                        bEqualsValues = false;
                                    }
                                }

                                if (!bEqualsValues)
                                {
                                    bEquals = false;
                                    break;//dW += mFormMain.iMsgHarKoef[iv];
                                }
                                //+---
                            }

                        }
                        //dW = dW * 1000 / iMsgHarCount;
                        //if (dW > 0)
                        //{
                        //    String value = ((int)dW).ToString("000000").Substring(0, 6) + "|" + EQ.Substring(EQ.IndexOf("|") + 1);
                        //    lstEQOuttMessagesList.Add(value);
                        //}
                        //else
                        //    bFiltered = true;
                    }

                    if (bEquals && bKeywords)
                        grid1[k, 0].Value = true;
                }

                mNotLockItemChecked = true;
                grid_ItemChecked();
                Cursor = Cursors.Arrow;
            }

        }

        public class ColumnOrderItem : Object
        {
            public String mName;
            public int iOriginalIdx;
            public int iDisplayIdx;
            public int Width;

            public ColumnOrderItem(String _mName, int _iOriginalIdx, int _iDisplayIdx, int _Width)
            {
                mName = _mName;
                iOriginalIdx = _iOriginalIdx;
                iDisplayIdx = _iDisplayIdx;
                Width = _Width;
            }

            override public String ToString()
            {
                return mName;
            }
        }

        private void toolStripMenuItemColumnsOrder_Click(object sender, EventArgs e)
        {
            FormColumnsOrder fco = new FormColumnsOrder();
            fco.listBoxColumns.Items.Clear();
            List<String> listColumns = new List<string>();
            for (int i = 1; i < grid1.ColumnsCount; i++)
            {
                ColumnOrderItem coi = new ColumnOrderItem(grid1[0, i].Value.ToString(), columnOrderReverce[i - 1], i - 1, grid1.Columns[i].Width);
                fco.listBoxColumns.Items.Add(coi);
            }
            fco.Setup();

            if (fco.ShowDialog() == DialogResult.OK)
            {
                List<ColumnOrderItem> lstcolumns = new List<ColumnOrderItem>();
                for (int i = 0; i < fco.listBoxColumns.Items.Count; i++)
                {
                    ColumnOrderItem coi = (ColumnOrderItem)fco.listBoxColumns.Items[i];
                    coi.iDisplayIdx = i;
                    lstcolumns.Add(coi);
                }
                lstcolumns = lstcolumns.OrderBy(i => i.iOriginalIdx).ToList();
                List<String> lstList = new List<string>();
                foreach(ColumnOrderItem coi in lstcolumns)
                {
                    lstList.Add(coi.iOriginalIdx.ToString()+ "|"+ coi.iDisplayIdx.ToString() + "|"+coi.Width.ToString());
                }
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditEQ" + mFileNamePrefix + "MessagesDB.columns"), lstList, Encoding.UTF8);

                Setup(slstErrorsInit, iPersUserID, SelectedMsgRecord, false);
            }
        }

        private void SaveColumnsOrderAndWidth()
        {
            List<String> lstcolumns = new List<String>();
            for (int i = 1; i < grid1.ColumnsCount; i++)
            {
                lstcolumns.Add((i - 1).ToString() + "|" + (columnOrder[i - 1] - 1).ToString() + "|" + grid1.Columns[columnOrder[i - 1]].Width.ToString());
            }
            if (lstcolumns.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditEQ" + mFileNamePrefix + "MessagesDB.columns"), lstcolumns, Encoding.UTF8);
        }

        int[] columnOrder;
        int[] columnOrderReverce;
        private void LoadColumnsOrderAndWidth()
        {
            List<String> lstcolumns = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditEQ" + mFileNamePrefix + "MessagesDB.columns")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditEQ" + mFileNamePrefix + "MessagesDB.columns"));
                lstcolumns = new List<String>(srcFile);
            }

            if (lstcolumns.Count > 0)
            {
                int[] iPos = new int[lstcolumns.Count];
                columnOrder = new int[lstcolumns.Count];
                columnOrderReverce = new int[lstcolumns.Count];
                ColumnInfo[] columnInfo = new ColumnInfo[lstcolumns.Count];

                foreach (String value in lstcolumns)
                {
                    String str = value;
                    int iidx = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                    str = str.Substring(str.IndexOf("|") + 1);
                    int idispidx = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                    str = str.Substring(str.IndexOf("|") + 1);
                    int iwidth = Convert.ToInt32(str);

                    columnInfo[iidx] = grid1.Columns[iidx + 1];
                    if (iwidth < 2)
                        iwidth = 2;
                    else if (iwidth > this.Width - 100)
                        iwidth = this.Width - 100;
                    columnInfo[iidx].Width = iwidth;
                    columnInfo[iidx].MinimalWidth = 2;
                    columnInfo[iidx].MaximalWidth= this.Width - 100;

                    iPos[idispidx] = iidx;
                    columnOrder[iidx] = idispidx + 1;
                    columnOrderReverce[idispidx] = iidx;
                }

                for (int i = 0; i < iPos.Length; i++)
                {
                    int idx = grid1.Columns.IndexOf(columnInfo[iPos[i]]);
                    int newidx = i + 1;
                    grid1.Columns.Move(idx, newidx);
                }
            }
            else
            {
                columnOrder = new int[ATTRIBUTES_COLUMN_MARKER+1];
                for (int i=0; i< columnOrder.Length; i++)
                {
                    columnOrder[i] = i+1;
                    columnOrderReverce[i] = i;
                }
            }
        }

        private void FormEditEQMessagesDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveColumnsOrderAndWidth();
            if (mNeedSaveEQMessageDB)
            {
                if (mInMsgBase)
                    mFormMain.SaveEQInMessageDB();
                else
                    mFormMain.SaveEQOutMessageDB();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string sCopyName = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_13", this.Name, "Копия");
            if (gridCheckedCount <= 0)
            {
                if (gridSelectedIndex > 0)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_12", this.Name, "Вы уверены, что хотите скопировать это Сообщение?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Копирование Сообщения"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        int iSelIdx = gridSelectedIndex;
                        String sMsgNewRec = MessagesDB_GetFromVisualList(iSelIdx);

                        String sFields = sMsgNewRec.Substring(0, sMsgNewRec.IndexOf("|@!"));
                        sFields = sFields.Substring(0, sFields.LastIndexOf("|"));
                        sFields = sFields + "|" + (mInMsgBase ? FormMain.iMsgINMaxID : FormMain.iMsgOUTMaxID + 1).ToString() + "|@!";
                        sMsgNewRec = sFields + sCopyName + " " + sMsgNewRec.Substring(sMsgNewRec.IndexOf("|@!") + 3);

                        iSelIdx = -1;
                        if (!hashsetEQMessagesDB.Contains(sMsgNewRec))
                        {
                            lstEQMessagesDB.Add(sMsgNewRec);
                            hashsetEQMessagesDB.Add(sMsgNewRec);
                            mNeedSaveEQMessageDB = true;

                            if (mInMsgBase)
                            {
                                FormMain.iMsgINMaxID++;
                                NilsaUtils.SaveLongValue(0, FormMain.iMsgINMaxID);
                            }
                            else
                            {
                                FormMain.iMsgOUTMaxID++;
                                NilsaUtils.SaveLongValue(1, FormMain.iMsgOUTMaxID);
                            }
                        }
                        else
                        {
                            for (int i = 1; i < grid1.RowsCount; i++)
                            {
                                String sMsgRec = MessagesDB_GetFromVisualList(i);
                                if (sMsgRec == sMsgNewRec)
                                {
                                    iSelIdx = i;
                                    break;
                                }
                            }
                        }
                        MessagesDB_AddToVisualList(sMsgNewRec, iSelIdx);
                        if (grid1.RowsCount > 1)
                        {
                            if (iSelIdx >= 0)
                                selectRow(iSelIdx);
                            else
                                selectRow(grid1.RowsCount - 1);
                        }
                        grid_ItemChecked();
                    }
                }
            }
            else
            {
                if (gridCheckedCount > 0)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_15", this.Name, "Вы уверены, что хотите скопировать все отмеченные Cообщения?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_14", this.Name, "Копирование Cообщений"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        foreach (int k in gridCheckedIndexes)
                        {
                            String sMsgNewRec = MessagesDB_GetFromVisualList(k);
                            String sFields = sMsgNewRec.Substring(0, sMsgNewRec.IndexOf("|@!"));
                            sFields = sFields.Substring(0, sFields.LastIndexOf("|"));
                            sFields = sFields + "|" + (mInMsgBase ? FormMain.iMsgINMaxID : FormMain.iMsgOUTMaxID + 1).ToString() + "|@!";
                            sMsgNewRec = sFields + sCopyName + " " + sMsgNewRec.Substring(sMsgNewRec.IndexOf("|@!") + 3);

                            int iSelIdx = -1;
                            if (!hashsetEQMessagesDB.Contains(sMsgNewRec))
                            {
                                lstEQMessagesDB.Add(sMsgNewRec);
                                hashsetEQMessagesDB.Add(sMsgNewRec);

                                if (mInMsgBase)
                                {
                                    FormMain.iMsgINMaxID++;
                                    NilsaUtils.SaveLongValue(0, FormMain.iMsgINMaxID);
                                }
                                else
                                {
                                    FormMain.iMsgOUTMaxID++;
                                    NilsaUtils.SaveLongValue(1, FormMain.iMsgOUTMaxID);
                                }
                            }
                            else
                            {
                                for (int i = 1; i < grid1.RowsCount; i++)
                                {
                                    String sMsgRec = MessagesDB_GetFromVisualList(i);
                                    if (sMsgRec == sMsgNewRec)
                                    {
                                        iSelIdx = i;
                                        break;
                                    }
                                }
                            }
                            MessagesDB_AddToVisualList(sMsgNewRec, iSelIdx);
                        }
                        mNeedSaveEQMessageDB = true;

                        EnableAllButtons();

                        if (grid1.RowsCount > 1)
                            selectRow(grid1.RowsCount - 1);

                        button12_Click(null, null);
                        //FilterList(ClearFilter());
                    }
                }
            }

        }

        private bool exportToCSV(String _filename)
        {
            FormMsgImportExportSelectColumns fmiesc = new FormMsgImportExportSelectColumns(mFormMain);
            fmiesc.button1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_16", this.Name, "Экспорт");
            fmiesc.checkedListBoxFields.Items.Clear();
            fmiesc.checkedListBoxFields.CheckOnClick = true;

            bool[] igetorderenabled = new bool[columnOrder.Length];

            for (int j = 0; j < columnOrder.Length; j++)
            {
                fmiesc.checkedListBoxFields.Items.Add(grid1[0, columnOrder[j]].Value, true);
                igetorderenabled[j] = false;
            }

            if (fmiesc.ShowDialog() == DialogResult.OK)
            {
                String fileToSave = _filename;
                if (_filename.Length == 0)
                {
                    saveFileDialog.Filter = "CSV-files (*.eq"+mCSVPrefix+ ".csv)|*.eq" + mCSVPrefix + ".csv|All files (*.*)|*.*";
                    saveFileDialog.FilterIndex = 0;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileToSave = saveFileDialog.FileName;
                    }
                }

                if (fileToSave.Length > 0)
                {
                    List<String> lstUsersToExport = new List<string>();

                    String sColumnNames = "";

                    foreach (int item in fmiesc.checkedListBoxFields.CheckedIndices)
                    {
                        igetorderenabled[item] = true;
                    }

                    for (int j = 0; j < columnOrder.Length; j++)
                    {
                        if (igetorderenabled[j])
                            sColumnNames += grid1[0, columnOrder[j]].Value + "|";
                    }
                    lstUsersToExport.Add(sColumnNames);

                    foreach (int k in gridCheckedIndexes)
                    {
                        String sColumnValues = "";
                        for (int j = 0; j < columnOrder.Length; j++)
                            if (igetorderenabled[j])
                                sColumnValues += grid1[k, columnOrder[j]].Value + "|";

                        lstUsersToExport.Add(sColumnValues);
                    }

                    File.WriteAllLines(fileToSave, lstUsersToExport, Encoding.UTF8);
                    return true;
                }
            }
            return false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            exportToCSV("");
        }

        private bool importFromCSV(String _filename)
        {
            if (File.Exists(_filename))
            {
                var srcFile = File.ReadAllLines(_filename);
                List<String> lstImportIDs = new List<String>(srcFile);
                if (lstImportIDs.Count > 1)
                {
                    String sHeader = lstImportIDs[0];
                    int iFieldText = -1, iFieldID = -1, iFieldMarker = -1;
                    String[] IW = sHeader.Split('|');

                    FormMsgImportExportSelectColumns fmiesc = new FormMsgImportExportSelectColumns(mFormMain);
                    fmiesc.button1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Импорт");
                    fmiesc.checkedListBoxFields.Items.Clear();
                    fmiesc.checkedListBoxFields.CheckOnClick = true;
                    List<String> listColumns = new List<string>();
                    for (int i = 0; i < IW.Length; i++)
                    {
                        String sColName = IW[i].ToLower();
                        for (int j = 0; j < columnOrder.Length; j++)
                        {
                            if (grid1[0, columnOrder[j]].Value.ToString().ToLower().Equals(sColName))
                            {
                                fmiesc.checkedListBoxFields.Items.Add(IW[i], true);
                                listColumns.Add(IW[i]);
                                break;
                            }
                        }
                    }

                    if (fmiesc.ShowDialog() == DialogResult.OK)
                    {
                        //IW = new String[listColumns.Count];
                        bool[] igetorderenabled = new bool[IW.Length];

                        for (int i = 0; i < IW.Length; i++)
                        {
                            //IW[i] = listColumns[i];
                            igetorderenabled[i] = false;
                        }

                        foreach (int idx in fmiesc.checkedListBoxFields.CheckedIndices)
                        {
                            for (int i = 0; i < IW.Length; i++)
                            {
                                if (IW[i].ToLower().Equals(listColumns[idx].ToLower()))
                                    igetorderenabled[i] = true;
                            }
                        }

                        for (int i = 0; i < IW.Length; i++)
                            if (IW[i].ToLower().Equals("текст"))
                            {
                                if (igetorderenabled[i])
                                    iFieldText = i;
                                break;
                            }
                        for (int i = 0; i < IW.Length; i++)
                            if (IW[i].ToLower().Equals("id"))
                            {
                                if (igetorderenabled[i])
                                    iFieldID = i;
                                break;
                            }

                        for (int i = 0; i < IW.Length; i++)
                            if (IW[i].ToLower().Equals("маркер"))
                            {
                                if (igetorderenabled[i])
                                    iFieldMarker = i;
                                break;
                            }

                        int[] iFieldHar = new int[iMsgHarCount];

                        for (int i = 0; i < iMsgHarCount; i++)
                        {
                            iFieldHar[i] = -1;
                            for (int j = 0; j < IW.Length; j++)
                            {
                                if (sMsgHar[i, 1].ToLower().Equals(IW[j].ToLower()))
                                {
                                    if (igetorderenabled[j])
                                        iFieldHar[i] = j;
                                    break;
                                }
                            }
                        }
                        Cursor = Cursors.WaitCursor;
                        for (int iRec = 1; iRec < lstImportIDs.Count; iRec++)
                        {
                            String sRecord = lstImportIDs[iRec];
                            String[] IWRecord = sRecord.Split('|');
                            if (IWRecord.Length != IW.Length)
                                continue;

                            String sUID = iFieldID >= 0 ? IWRecord[iFieldID] : "";
                            String sUText = iFieldText >= 0 ? IWRecord[iFieldText] : "EMPTY";
                            //String sUMarker = iFieldMarker >= 0 ? "|!*#0"+IWRecord[iFieldMarker] : "";
                            String sUMarker = (iFieldMarker >= 0 ? (IWRecord[iFieldMarker].Length > 0 ? "|!*#0" + IWRecord[iFieldMarker] : "") : "");

                            //---
                            String sMsgNewRec = "000000|";
                            for (int i = 0; i < iMsgHarCount; i++)
                            {
                                sMsgNewRec = sMsgNewRec + (iFieldHar[i] >= 0 ? IWRecord[iFieldHar[i]] : "") + "|";
                            }

                            sMsgNewRec = sMsgNewRec + "@!" + NilsaUtils.TextToString(sUText) + sUMarker;

                            if (!hashsetEQMessagesDB.Contains(sMsgNewRec))
                            {
                                lstEQMessagesDB.Add(sMsgNewRec);
                                hashsetEQMessagesDB.Add(sMsgNewRec);
                                mNeedSaveEQMessageDB = true;
                                MessagesDB_AddToVisualList(sMsgNewRec, -1);
                            }

                            //---

                        }
                        if (grid1.RowsCount > 1)
                            selectRow(grid1.RowsCount - 1);
                        Cursor = Cursors.Arrow;
                        return true;
                    }//if (iFieldID >= 0 && iFieldName >= 0)
                }//if (lstImportIDs.Count > 1)

            }//if (File.Exists(openFileDialog.FileName))
            return false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "CSV-files (*.eq" + mCSVPrefix + ".csv)|*.eq" + mCSVPrefix + ".csv|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (importFromCSV(openFileDialog.FileName))
                    MessageBox.Show("Импорт сообщений из файла завершен", "Импорт сообщений из файла", MessageBoxButtons.OK);
                else
                    MessageBox.Show("Импорт отменен или файл содержит ошибки", "Импорт сообщений из файла", MessageBoxButtons.OK);
            }//if (openFileDialog.ShowDialog() == DialogResult.OK)
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonImportCSVFromServer_Click(object sender, EventArgs e)
        {
            String filename = mFormMain.tsmiImportMessagesCSVFTP(mInMsgBase ? 3 : 2);

            if (filename.Length == 0)
                return;

            string openFileDialog_FileName = Path.Combine(Application.StartupPath, filename);

            if (importFromCSV(openFileDialog_FileName))
                MessageBox.Show("Загрузка сообщений с Сервера успешно завершена", "Загрузка сообщений с Сервера", MessageBoxButtons.OK);
            else
                MessageBox.Show("Импорт отменен или файл содержит ошибки", "Загрузка сообщений с Сервера", MessageBoxButtons.OK);

            if (File.Exists(openFileDialog_FileName))
                File.Delete(openFileDialog_FileName);
        }

        private void buttonExportCSVToServer_Click(object sender, EventArgs e)
        {
            string value = "eq" + mCSVPrefix + "msg" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_");
            string filename = Path.Combine(Application.StartupPath, value + (mInMsgBase ? FormMain.FTP_SERVER_EQIN_NAME_POSTFIX: FormMain.FTP_SERVER_EQOUT_NAME_POSTFIX));

            if (exportToCSV(filename))
            {
                int bSuccess = mFormMain.tsmiExportMessagesCSVFTP(mInMsgBase ? 3 : 2, value);

                if (File.Exists(filename))
                    File.Delete(filename);

                if (bSuccess == 0)
                {

                    if (bSuccess != 2)
                    {
                        if (bSuccess == 1)
                            MessageBox.Show("Во время экспорта сообщений на Сервер произошла ошибка. Проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", "Экспорт сообщений на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("Экспорт сообщений на Сервер успешно выполнен", "Экспорт сообщений на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
            }

        }

        private void changeDelimitersToTilda_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите заменить разделители запятые на разделители тильды во всех сообщениях?", "Замена разделителей", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                hashsetEQMessagesDB.Clear();

                for (int i = 0; i < lstEQMessagesDB.Count; i++)
                {
                    String EQ = lstEQMessagesDB[i];
                    String msgText = EQ.Substring(EQ.IndexOf("|@!"));
                    String msgVector = EQ.Substring(0, EQ.IndexOf("|@!"));
                    msgText = msgText.Replace(',', '~');
                    lstEQMessagesDB[i] = msgVector + msgText;
                    hashsetEQMessagesDB.Add(msgVector + msgText);
                }

                ApplyFilter();
                MessageBox.Show("Операция выполнена.", "Замена разделителей", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
