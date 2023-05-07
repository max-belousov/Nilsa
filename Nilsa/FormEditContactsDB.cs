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
using VkNet.Enums.Filters;
using VkNet.Enums;
using System.Collections;
using SourceGrid;
using System.Threading;

namespace Nilsa
{
    public partial class FormEditContactsDB : Form
    {
        FormMain mFormMain;

        public String[,] sContHar;
        public int iContHarCount = 16;
        public int iContHarAttrCount = 4;
        List<String> lstErrorsLogList;
        String slstErrorsInit;
        public long iPersUserID, iContUserID;
        public String sContUserID;
        List<String> lstContHarValues;
        List<String> lstContactsList;

        private string sUnknownAge;

        public Boolean bNeedPersoneChange, bNeedPersoneReread;
        bool bDoImportContacts;
        bool bDoGroupOperation;
        int iImportMode;
        public bool bInitDialogs = false;

        bool closing = false;

        public FormEditContactsDB(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();
            buttonSelectContacters.Visible = false;

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
            sUnknownAge = mFormMain.sDBDataItemsStrings_AgeUnknown;

            //this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;

            // HACK - disable checkinf of ilegal calls
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;


            // ensure that thread is aborted, when we exit the form before 
            // the thread exits
            this.Closing += delegate
            {
                closing = true;
            };
        }

        public void ExceptionToLogList(String sMethod, String sErrorsParameters, Exception e)
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

        public int gridCheckedCount;
        public int gridSelectedIndex;
        public HashSet<int> gridCheckedIndexes;
        const int ATTRIBUTES_COLUMN_OFFS = 2;
        const int ATTRIBUTES_COLUMN_TEXT = 0;
        const int ATTRIBUTES_COLUMN_ID = 1;

        private void initGrid(bool bFirstInit)
        {
            mNotLockItemChecked = true;
            gridCheckedCount = 0;
            gridSelectedIndex = -1;
            gridCheckedIndexes = new HashSet<int>();
            if (bFirstInit)
                itemCheckedChangedEvent = new ItemCheckedChangedEvent(this);
            grid1.RowsCount = 1;
            grid1.ColumnsCount = 3 + iContHarCount;
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

            grid1[0, 1] = new SourceGrid.Cells.ColumnHeader("Имя");

            grid1[0, 2] = new SourceGrid.Cells.ColumnHeader("ID");

            for (int i = 0; i < iContHarCount; i++)
                grid1[0, ATTRIBUTES_COLUMN_OFFS + i + 1] = new SourceGrid.Cells.ColumnHeader(i.ToString());


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
            button2.Enabled = bBS || gridCheckedCount > 0 || tbAllPersonenInRotation.Checked;
            button23.Enabled = bBS || gridCheckedCount > 0;
            button16.Enabled = bBS || gridCheckedCount > 0;
            button5.Enabled = bBS || gridCheckedCount > 0 || tbAllPersonenInRotation.Checked;
            button3.Enabled = bBS;
            button11.Enabled = bBS && FormMain.SocialNetwork == 0;
            toolStripButtonCopyToClipboard.Enabled = bBS;
            buttonSelectContacters.Enabled = bBS || gridCheckedCount > 0;
            buttonCopyParameters.Enabled = bBS;
            buttonPasteParameters.Enabled = copyedParameters != null && (bBS || gridCheckedCount > 0);
        }

        ItemCheckedChangedEvent itemCheckedChangedEvent;
        bool mNotLockItemChecked;

        public class ItemCheckedChangedEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            FormEditContactsDB mForm;

            public ItemCheckedChangedEvent(FormEditContactsDB _mForm)
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

        private void grid_ItemChecked()
        {
            if (gridCheckedCount > 0)
            {
                button12.Enabled = true;
                button14.Enabled = FormMain.SocialNetwork == 0;
                button17.Enabled = FormMain.SocialNetwork == 0;
                button18.Enabled = true;
                buttonExportContacterServer.Enabled = true;
                button25.Enabled = true;
                button24.Enabled = true;
                button20.Enabled = true;
                button21.Enabled = true;
                button22.Enabled = true;
                toolStripButtonSearchDuplicate.Enabled = FormMain.SocialNetwork == 0;
            }
            else
            {
                button12.Enabled = false;
                button14.Enabled = tbAllPersonenInRotation.Checked;
                button17.Enabled = tbAllPersonenInRotation.Checked;
                button18.Enabled = false;
                buttonExportContacterServer.Enabled = false;
                button25.Enabled = false;
                button24.Enabled = false;
                button20.Enabled = false;
                button21.Enabled = false;
                button22.Enabled = false;
                toolStripButtonSearchDuplicate.Enabled = false;
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
            button2.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0 || tbAllPersonenInRotation.Checked;
            button23.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            button16.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            button5.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0 || tbAllPersonenInRotation.Checked;
            buttonCopyParameters.Enabled = gridSelectedIndex > 0;
            buttonPasteParameters.Enabled = copyedParameters != null && (gridSelectedIndex > 0 || gridCheckedCount > 0);
            buttonSelectContacters.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            toolStripButtonCopyToClipboard.Enabled = gridSelectedIndex > 0;
        }

        //public FormWebBrowser fwbVKontakte = null;

        public void Setup(String sErrorHdr, long iUsrID, long iContUID = 0, Boolean initDialog = false, bool bFirstInit = true)
        {
            initGrid(bFirstInit);

            exportDialogsFromPersonen.Enabled = mFormMain.lstPersoneChange.Count > 0 && FormMain.SocialNetwork == 0;
            reportDialogsFromPersonen.Enabled = mFormMain.lstPersoneChange.Count > 0 && FormMain.SocialNetwork == 0;
            this.button7.Enabled = FormMain.SocialNetwork == 0;
            this.buttonImportPersonenAsContacter.Enabled = FormMain.SocialNetwork == 0;
            this.button8.Enabled = FormMain.SocialNetwork == 0;
            this.button11.Enabled = FormMain.SocialNetwork == 0;
            this.button19.Enabled = FormMain.SocialNetwork == 0;
            this.tbAllPersonenInRotation.Enabled = mFormMain.lstPersoneChangeOriginal.Count > 0;

            pictureBoxPersone.Image = mFormMain.personPicture;
            toolTip1.SetToolTip(pictureBoxPersone, mFormMain.userNameName + " " + mFormMain.userNameFamily);

            bDoImportContacts = true;
            bDoGroupOperation = true;
            pbProgress.Visible = false;
            bNeedPersoneChange = false;
            bNeedPersoneReread = false;
            slstErrorsInit = sErrorHdr;
            iPersUserID = iUsrID;
            iContUserID = iContUID;
            sContUserID = iContUserID.ToString();
            LoadErrorsLogList();
            ContactsList_Load();

            button26.Enabled = FormMain.SocialNetwork == 0;
            buttonImportContacterServer.Enabled = FormMain.SocialNetwork == 0;
            button2.Enabled = false;
            button5.Enabled = false;
            button3.Enabled = false;
            button23.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            button16.Enabled = false;
            buttonSelectContacters.Enabled = false;
            toolStripButtonSearchDuplicate.Enabled = false;
            toolStripButtonCopyToClipboard.Enabled = false;
            buttonCopyParameters.Enabled = false;
            buttonPasteParameters.Enabled = false;

            if (iContHarCount <= 16)
            {
                for (int i = 0; i < iContHarCount; i++)
                {
                    grid1.Columns[ATTRIBUTES_COLUMN_OFFS + i + 1].Width = 100;
                    grid1[0, ATTRIBUTES_COLUMN_OFFS + i + 1].Value = sContHar[i, 1];
                }
            }

            LoadColumnsOrderAndWidth();

            if (initDialog || NilsaUtils.LoadLongValue(8, 0) == 1)
            {
                Cursor = Cursors.WaitCursor;

                ApplyFilter();

                int iSelIdx = 1;
                for (int i = 1; i < grid1.RowsCount; i++)
                {
                    if (sContUserID.Equals(getItemID(i)))
                    {
                        iSelIdx = i;
                        break;
                    }
                }

                selectRow(iSelIdx);
                grid_ItemChecked();

                Cursor = Cursors.Arrow;
            }
            else
            {
                //new Thread(fillData).Start();

                Cursor = Cursors.WaitCursor;

                grid1.SuspendLayout();
                for (int i = 0; i < lstContactsList.Count; i++)
                {

                    String value = lstContactsList[i];
                    String sUID = value.Substring(0, value.IndexOf("|")); // usrID
                    value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                    String sUName = value;
                    ContactsList_AddUserToVisualList(sUID, sUName);
                }
                grid1.ResumeLayout();

                button4.Image = Nilsa.Properties.Resources.filter_list_on;

                int iSelIdx = 1;
                for (int i = 1; i < grid1.RowsCount; i++)
                {
                    if (sContUserID.Equals(getItemID(i)))
                    {
                        iSelIdx = i;
                        break;
                    }
                }

                selectRow(iSelIdx);
                grid_ItemChecked();

                Cursor = Cursors.Arrow;

            }


            if (initDialog)
            {
                if (grid1.RowsCount - 1 > 0)
                {
                    button10_Click(null, null);
                    Application.DoEvents();
                    INIT_PERSONE_DIALOG = true;
                    button16_Click(null, null);
                    INIT_PERSONE_DIALOG = false;
                    Application.DoEvents();
                }
                else
                {
                    Application.DoEvents();
                    INIT_PERSONE_DIALOG = false;
                    timerClose.Enabled = true;
                }
            }
            //else
            //{
            //    if (FormMain.SocialNetwork == 0)
            //    {
            //        if (fwbVKontakte == null)
            //        {
            //            fwbVKontakte = new FormWebBrowser(mFormMain, true);
            //            fwbVKontakte.Init();
            //        }

            //        //stopTimers();

            //        fwbVKontakte.Setup(mFormMain.userLogin, mFormMain.userPassword, WebBrowserCommand.LoginPersone);
            //        //if (!fwbVKontakteFirstShow)
            //        //{
            //        //   fwbVKontakteFirstShow = true;
            //            fwbVKontakte.Show();
            //        //}
            //        fwbVKontakte.WaitResult();
            //        //iPersUserID = fwbVKontakte.loggedPersoneID;
            //    }
            //}
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

        public String getItemID(int rowIndex)
        {
            return grid1[rowIndex, columnOrder[ATTRIBUTES_COLUMN_ID]].Value.ToString();
        }

        private String getItemName(int rowIndex)
        {
            return grid1[rowIndex, columnOrder[ATTRIBUTES_COLUMN_TEXT]].Value.ToString();
        }

        private void ApplyFilter(bool bNotShowDialog = true)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
            for (int i = 0; i < iContHarCount; i++)
                for (int j = 0; j < iContHarAttrCount + 1; j++)
                    fe.sPersHar[i, j] = sContHar[i, j];

            LoadFormEditPersHarValues(fe, 100001, 1);

            fe.iPersHarAttrCount = iContHarAttrCount;
            fe.iPersHarCount = iContHarCount;
            fe.sFilePrefix = "cont";
            fe.Setup(false, true);
            if (bNotShowDialog || fe.ShowDialog() == DialogResult.OK)
            {
                SaveFormEditPersHarValues(fe, 100001, 1);
                String[] RQV = new String[iContHarCount];
                for (int iii = 0; iii < iContHarCount; iii++)
                {
                    RQV[Convert.ToInt32(fe.sPersHar[iii, 0]) - 1] = fe.sPersHar[iii, iContHarAttrCount];
                    sContHar[iii, iContHarAttrCount] = fe.sPersHar[iii, iContHarAttrCount];
                }

                FilterList(RQV, (int)fe.numericUpDown1.Value);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text.Equals(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Отключить фильтр")))
            {
                FilterList(ClearFilter());
                NilsaUtils.SaveLongValue(8, 0);
            }
            else
            {
                ApplyFilter(false);
                NilsaUtils.SaveLongValue(8, 1);
            }
        }

        private String[] ClearFilter()
        {
            String[] RQV = new String[iContHarCount];
            for (int iii = 0; iii < iContHarCount; iii++)
            {
                RQV[iii] = "";
                sContHar[iii, iContHarAttrCount] = "";
            }

            return RQV;
        }

        private void FilterList(String[] RQV, int counterMax = 0)
        {
            button2.Enabled = tbAllPersonenInRotation.Checked;
            button5.Enabled = tbAllPersonenInRotation.Checked;
            button3.Enabled = false;
            button11.Enabled = false;
            button16.Enabled = false;
            button23.Enabled = false;
            buttonSelectContacters.Enabled = false;
            toolStripButtonCopyToClipboard.Enabled = false;
            buttonCopyParameters.Enabled = false;
            buttonPasteParameters.Enabled = false;

            Cursor = Cursors.WaitCursor;
            grid1.SuspendLayout();
            grid1.Redim(1, grid1.ColumnsCount);
            gridCheckedCount = 0;
            gridCheckedIndexes.Clear();

            Boolean bRQVEmpty = true;
            for (int iv = 0; iv < RQV.Length; iv++)
            {
                if (RQV[iv].Length > 0)
                {
                    bRQVEmpty = false;
                    break;
                }
            }

            //---
            bool bAcceptEmptyAge = false;
            int iAcceptAgeMin = -1;
            int iAcceptAgeMax = -1;
            int iAgeField = 1;

            if (iAgeField >= 0)
            {
                if (RQV[iAgeField].Length > 0)
                {
                    String sAgeFilter = RQV[iAgeField].Trim();
                    if (sAgeFilter[0] == '!')
                    {
                        bAcceptEmptyAge = true;
                        sAgeFilter = sAgeFilter.Substring(1).Trim();
                    }
                    if (sAgeFilter.Length > 0)
                    {
                        if (sAgeFilter.IndexOf("-") >= 0)
                        {
                            String sAgeMin = sAgeFilter.Substring(0, sAgeFilter.IndexOf("-")).Trim();
                            String sAgeMax = sAgeFilter.Substring(sAgeFilter.IndexOf("-") + 1).Trim();
                            iAcceptAgeMin = 0;
                            iAcceptAgeMax = 1000;
                            try
                            {
                                if (sAgeMin.Length > 0)
                                    iAcceptAgeMin = Convert.ToInt32(sAgeMin);

                                if (sAgeMax.Length > 0)
                                    iAcceptAgeMax = Convert.ToInt32(sAgeMax);
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            if (sUnknownAge.Equals(sAgeFilter))
                                bAcceptEmptyAge = true;
                            else
                            {
                                iAcceptAgeMin = Convert.ToInt32(sAgeFilter);
                                iAcceptAgeMax = iAcceptAgeMin;
                            }
                        }
                    }
                }
            }
            //---

            bool bFiltered = false;
            int counterFiltered = 0;
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

                            EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1).ToLower();
                        }

                        bEquals = true;
                        for (int iv = 0; iv < RQV.Length; iv++)
                        {
                            if (RQV[iv].Length == 0/* || EQV[iv].Length == 0*/)
                                continue;

                            if (!(RQV[iv].ToLower().Equals(EQV[iv]) || EQV[iv].StartsWith(RQV[iv].ToLower())))
                            {
                                //---
                                if (iAgeField == iv)
                                {
                                    String sAge = EQV[iv];
                                    if (sUnknownAge.Equals(sAge))
                                    {
                                        bEquals = bAcceptEmptyAge;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            int iAge = Convert.ToInt32(sAge);
                                            if (iAge < iAcceptAgeMin || iAge > iAcceptAgeMax)
                                                bEquals = false;
                                        }
                                        catch
                                        {
                                            bEquals = false;
                                        }
                                    }

                                    if (!bEquals)
                                        break;
                                }
                                else
                                {
                                    //+---
                                    if (RQV[iv].IndexOf("~") >= 0 || RQV[iv].IndexOf("|") >= 0)
                                    {
                                        bEquals = false;
                                        String sFilter = RQV[iv].ToLower();
                                        bool bInverse = false;
                                        String sValue = EQV[iv].Trim();
                                        if (sFilter[0] == '~')
                                        {
                                            bInverse = true;
                                            bEquals = true;
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
                                                        bEquals = true;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (sPart.Equals(sValue))
                                                    {
                                                        bEquals = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    else if (RQV[iv].IndexOf("-") >= 0)
                                    {
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

                                            sval = EQV[iv].Trim();
                                            try
                                            {
                                                int ival = Convert.ToInt32(sval);
                                                if (ival < ivalmin || ival > ivalmax)
                                                    bEquals = false;
                                            }
                                            catch
                                            {
                                                bEquals = false;
                                            }
                                        }
                                        catch
                                        {
                                            bEquals = false;
                                        }
                                    }
                                    else
                                        bEquals = false;
                                    //---
                                    if (!bEquals)
                                        break;
                                    //+---
                                }
                            }
                        }
                    }
                    else
                        bEquals = false;
                }

                if (bEquals)
                {
                    counterFiltered++;

                    ContactsList_AddUserToVisualList(sUID, sUName);

                    if (counterMax > 0 && counterFiltered >= counterMax)
                        break;
                }
                else
                    bFiltered = true;
            }
            grid1.ResumeLayout();
            Cursor = Cursors.Arrow;
            selectRow(1);
            grid_ItemChecked();

            if (bFiltered)
            {
                button4.Image = Nilsa.Properties.Resources._filter_off;
                button4.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Отключить фильтр");
            }
            else
            {
                button4.Image = Nilsa.Properties.Resources._filter_on;
                button4.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Фильтрация списка");
            }

        }

        public void ContactsList_AddUserToVisualList(String sUID, String sUName, int iVLIdx = -1)
        {
            String[] EQV = new String[iContHarCount];

            if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
            {
                List<String> lstContHarValues = new List<String>();
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                lstContHarValues = new List<String>(srcFile);
                foreach (String str in lstContHarValues)
                {
                    if (str == null)
                        continue;

                    if (str.Length == 0)
                        continue;

                    EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                }
            }

            if (iVLIdx >= 0)
            {
                if (iContHarCount <= 16)
                    for (int iOffs = 0; iOffs < iContHarCount; iOffs++)
                        grid1[iVLIdx, columnOrder[iOffs + ATTRIBUTES_COLUMN_OFFS]].Value = EQV[iOffs];
                grid1[iVLIdx, columnOrder[ATTRIBUTES_COLUMN_ID]].Value = sUID;
                grid1[iVLIdx, columnOrder[ATTRIBUTES_COLUMN_TEXT]].Value = sUName;

            }
            else
            {
                int row = grid1.RowsCount;
                grid1.Rows.Insert(row);

                grid1[row, 0] = new SourceGrid.Cells.CheckBox(null, false);
                grid1[row, 0].AddController(itemCheckedChangedEvent);
                if (iContHarCount <= 16)
                    for (int iOffs = 0; iOffs < iContHarCount; iOffs++)
                        grid1[row, columnOrder[iOffs + ATTRIBUTES_COLUMN_OFFS]] = new SourceGrid.Cells.Cell(EQV[iOffs]);
                grid1[row, columnOrder[ATTRIBUTES_COLUMN_ID]] = new SourceGrid.Cells.Cell(sUID);
                grid1[row, columnOrder[ATTRIBUTES_COLUMN_TEXT]] = new SourceGrid.Cells.Cell(sUName);

            }
        }

        private void ContactsList_Load()
        {
            lstContactsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                lstContactsList = new List<String>(srcFile);



                int idx = 0;
                bool isCorrected = false;
                while (idx < lstContactsList.Count)
                {
                    if (lstContactsList[idx].EndsWith("| "))
                    {
                        isCorrected = true;
                        String value = lstContactsList[idx];
                        String sUD = value.Substring(0, value.IndexOf("|"));

                        lstContactsList.RemoveAt(idx);

                        if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt")))
                            File.Delete(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt"));

                        if (File.Exists(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt")))
                            File.Delete(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt"));

                        if (File.Exists(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt")))
                            File.Delete(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt"));

                        if (File.Exists(Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt")))
                            File.Delete(Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt"));
                    }
                    else
                        idx++;

                }

                if (isCorrected)
                {
                    if (lstContactsList.Count > 0)
                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstContactsList, Encoding.UTF8);
                    else
                        File.Delete(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                }
            }
        }

        private void ContactsList_RemoveUser(String sUD)
        {
            int iuserIdx = ContactsList_GetUserIdx(sUD);
            if (iuserIdx >= 0)
            {
                lstContactsList.RemoveAt(iuserIdx);
                if (lstContactsList.Count > 0)
                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstContactsList, Encoding.UTF8);
                else
                    File.Delete(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt"));
            }
        }

        public int ContactsList_GetUserIdx(String sUD)
        {
            int iuserIdx = -1;
            for (int i = 0; i < lstContactsList.Count; i++)
            {
                String str = lstContactsList[i];
                if (str.Substring(0, str.IndexOf("|")).Equals(sUD))
                {
                    iuserIdx = i;
                    break;
                }
            }
            return iuserIdx;
        }

        public int ContactsList_GetVisualListIdx(String sUD)
        {
            int iuserIdx = -1;
            for (int k = 1; k < grid1.RowsCount; k++)
            {
                if (sUD.Equals(getItemID(k)))
                {
                    iuserIdx = k;
                    break;
                }
            }
            return iuserIdx;
        }

        private String ContactsList_GetUserRecord(String sUD)
        {
            for (int i = 0; i < lstContactsList.Count; i++)
            {
                String str = lstContactsList[i];
                if (str.Substring(0, str.IndexOf("|")).Equals(sUD))
                {
                    return str;
                }
            }
            return "";
        }

        private String ContactsList_GetUserField(String sUD, int iFieldIdx) // 0 - usrID, 1 - usrName
        {
            String retval = ContactsList_GetUserRecord(sUD);
            if (retval.Length > 0)
            {
                for (int i = 0; i < iFieldIdx; i++)
                    retval = retval.Substring(retval.IndexOf("|") + 1);
                if (iFieldIdx < 3)
                    retval = retval.Substring(0, retval.IndexOf("|"));
            }
            return retval;
        }

        public void ExternalContactsList_AddUser(String ExternalPersonenID, String sUD, String sUName)
        {
            List<String> lstExternalContactsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt"));
                lstExternalContactsList = new List<String>(srcFile);
            }

            int iuserIdx = -1;
            for (int i = 0; i < lstExternalContactsList.Count; i++)
            {
                String str = lstExternalContactsList[i];
                if (str.Substring(0, str.IndexOf("|")).Equals(sUD))
                {
                    iuserIdx = i;
                    break;
                }
            }

            String userRec = sUD + "|" + sUName;
            if (iuserIdx >= 0)
                lstExternalContactsList[iuserIdx] = userRec;
            else
                lstExternalContactsList.Add(userRec);

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt"), lstExternalContactsList, Encoding.UTF8);
        }

        public void ContactsList_AddUser(String sUD, String sUName)
        {
            int iuserIdx = ContactsList_GetUserIdx(sUD);
            String userRec = sUD + "|" + sUName;
            if (iuserIdx >= 0)
                lstContactsList[iuserIdx] = userRec;
            else
                lstContactsList.Add(userRec);

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstContactsList, Encoding.UTF8);
        }
        //для новых соцсетей
        public void ContactsList_AddUser(String sUD, String sUName, string cid)
        {
            int iuserIdx = ContactsList_GetUserIdx(sUD);
            String userRec = sUD + "|" + sUName + "|" + cid;
            if (iuserIdx >= 0)
                lstContactsList[iuserIdx] = userRec;
            else
                lstContactsList.Add(userRec);

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstContactsList, Encoding.UTF8);
        }

        private String[] initFilterForAllContactsToPersonenInRotation(bool bNotShowDialog = true)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
            for (int i = 0; i < iContHarCount; i++)
                for (int j = 0; j < iContHarAttrCount + 1; j++)
                    fe.sPersHar[i, j] = sContHar[i, j];

            LoadFormEditPersHarValues(fe, 100001, 1);

            fe.iPersHarAttrCount = iContHarAttrCount;
            fe.iPersHarCount = iContHarCount;
            fe.sFilePrefix = "cont";
            fe.Setup();
            if (bNotShowDialog || fe.ShowDialog() == DialogResult.OK)
            {
                SaveFormEditPersHarValues(fe, 100001, 1);
                String[] RQV = new String[iContHarCount];
                for (int iii = 0; iii < iContHarCount; iii++)
                {
                    RQV[Convert.ToInt32(fe.sPersHar[iii, 0]) - 1] = fe.sPersHar[iii, iContHarAttrCount];
                    sContHar[iii, iContHarAttrCount] = fe.sPersHar[iii, iContHarAttrCount];
                }

                return RQV;
            }
            return new String[0];
        }

        public String GetPersoneParametersValue(List<String> lstPersHarValues, String sPersHarID)
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

        private string CSV_formatvalue(string src)
        {
            return "\"" + src.Replace("\"", "\"\"") + "\"";
        }

        private string CSV_getPersoneHarValue(List<string> lstHar, int idx)
        {
            if (lstHar == null)
                return "";

            if (lstHar.Count != mFormMain.iPersHarCount)
                return "";

            return CSV_formatvalue(lstHar[idx]);
        }

        private string CSV_getContacterHarValue(List<string> lstHar, int idx)
        {
            if (lstHar == null)
                return "";

            if (lstHar.Count != mFormMain.iContHarCount)
                return "";

            return CSV_formatvalue(lstHar[idx]);
        }

        private void applyOperationForAllContactsToPersonenInRotation(int iOperation, List<String> lstContHar = null)
        {
            bool bNeedReauthorize = false;

            this.pbProgress.Visible = true;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Value = 0;
            this.pbProgress.Maximum = 10;
            this.bDoImportContacts = false;
            bDoGroupOperation = false;

            int iPersoneVisualIdx = 0;
            int iContactVisualIdx = 0;

            DisableAllButtons();
            button9.Text = "X";
            button9.Enabled = true;

            String sPersonenList = "|";
            foreach (string str in mFormMain.lstPersoneChangeOriginal)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                sPersonenList += str + "|";
            }

            String[] RQV = initFilterForAllContactsToPersonenInRotation(true);

            Boolean bRQVEmpty = true;
            for (int iv = 0; iv < RQV.Length; iv++)
            {
                if (RQV[iv].Length > 0)
                {
                    bRQVEmpty = false;
                    break;
                }
            }

            //---
            bool bAcceptEmptyAge = false;
            int iAcceptAgeMin = -1;
            int iAcceptAgeMax = -1;
            int iAgeField = 1;

            if (iAgeField >= 0)
            {
                if (RQV[iAgeField].Length > 0)
                {
                    String sAgeFilter = RQV[iAgeField].Trim();
                    if (sAgeFilter[0] == '!')
                    {
                        bAcceptEmptyAge = true;
                        sAgeFilter = sAgeFilter.Substring(1).Trim();
                    }
                    if (sAgeFilter.Length > 0)
                    {
                        if (sAgeFilter.IndexOf("-") >= 0)
                        {
                            String sAgeMin = sAgeFilter.Substring(0, sAgeFilter.IndexOf("-")).Trim();
                            String sAgeMax = sAgeFilter.Substring(sAgeFilter.IndexOf("-") + 1).Trim();
                            iAcceptAgeMin = 0;
                            iAcceptAgeMax = 1000;
                            try
                            {
                                if (sAgeMin.Length > 0)
                                    iAcceptAgeMin = Convert.ToInt32(sAgeMin);

                                if (sAgeMax.Length > 0)
                                    iAcceptAgeMax = Convert.ToInt32(sAgeMax);
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            if (sUnknownAge.Equals(sAgeFilter))
                                bAcceptEmptyAge = true;
                            else
                            {
                                iAcceptAgeMin = Convert.ToInt32(sAgeFilter);
                                iAcceptAgeMax = iAcceptAgeMin;
                            }
                        }
                    }
                }
            }
            //---

            if ((RQV.Length == iContHarCount) && File.Exists(Path.Combine(FormMain.sDataPath, "_personen.txt")))
            {
                var srcFilePersonen = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_personen.txt"));
                List<String> lstList = new List<String>(srcFilePersonen);

                List<String> lstCurrentPersoneHarValues = null;
                List<String> lstCurrentContacterHarValues = null;

                int iPersoneIdx = 0;
                foreach (String strPersoneRecord in lstList)
                {
                    if (strPersoneRecord.Length > 0)
                    {

                        if (bDoGroupOperation)
                        {
                            break;
                        }

                        String sPR = strPersoneRecord;
                        String sUID_Persone = sPR.Substring(0, sPR.IndexOf("|"));
                        sPR = sPR.Substring(sPR.IndexOf("|") + 1);
                        String sName_Persone = sPR.Substring(0, sPR.IndexOf("|"));
                        sPR = sPR.Substring(sPR.IndexOf("|") + 1);
                        String sLogin_Persone = sPR.Substring(0, sPR.IndexOf("|"));
                        sPR = sPR.Substring(sPR.IndexOf("|") + 1);
                        String sPwd_Persone = sPR.Trim();

                        if (sPersonenList.IndexOf("|" + sUID_Persone + "|") >= 0)
                        {
                            if (iOperation == 10 || iOperation == 11)
                            {
                                if (!mFormMain.AutorizeVK(sLogin_Persone, sPwd_Persone))
                                    continue;

                                bNeedReauthorize = true;
                            }

                            iPersoneIdx++;
                            if (this.pbProgress.Maximum != mFormMain.lstPersoneChangeOriginal.Count)
                                this.pbProgress.Maximum = mFormMain.lstPersoneChangeOriginal.Count;
                            this.pbProgress.Value = iPersoneIdx;
                            this.pbProgress.Text = "Персонаж " + iPersoneIdx.ToString() + "/" + mFormMain.lstPersoneChangeOriginal.Count.ToString() + " (" + sName_Persone + ")";
                            this.Text = this.pbProgress.Text;
                            Application.DoEvents();

                            if (iOperation == 10 || iOperation == 11)
                            {
                                // Персонаж
                                iPersoneVisualIdx++;
                                iContactVisualIdx = 0;

                                if (iOperation == 10)
                                    lstContHar.Add("<H1>" + iPersoneVisualIdx.ToString() + ". " + sName_Persone + "</H1>");
                                else if (iOperation == 11)
                                    lstCurrentPersoneHarValues = new List<string>();

                                List<String> lstContHarCurrent = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix() + sUID_Persone + ".txt")))
                                {
                                    if (iOperation == 10)
                                        lstContHar.Add("<UL>");
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix() + sUID_Persone + ".txt"));
                                    lstContHarCurrent = new List<String>(srcFile);
                                    for (int i = 0; i < mFormMain.iPersHarCount; i++)
                                    {
                                        String sHarName = mFormMain.sPersHar[i, 1];
                                        String sHarValue = GetPersoneParametersValue(lstContHarCurrent, mFormMain.sPersHar[i, 0]);
                                        if (iOperation == 10)
                                            lstContHar.Add("<LI><B>" + sHarName + ":<B> " + sHarValue + "</LI>");
                                        else if (iOperation == 11)
                                            lstCurrentPersoneHarValues.Add(sHarValue);
                                    }
                                    if (iOperation == 10)
                                        lstContHar.Add("</UL>");
                                }
                            }

                            if (File.Exists(Path.Combine(FormMain.sDataPath, "_contacts_" + sUID_Persone + ".txt")))
                            {
                                var srcFileCont = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + sUID_Persone + ".txt"));
                                List<String> lstListCont = new List<String>(srcFileCont);

                                bool bChanged = false;
                                int iContactIdx = 0;
                                while (iContactIdx < lstListCont.Count)
                                {
                                    if (bDoGroupOperation)
                                    {
                                        break;
                                    }

                                    String strContactRecord = lstListCont[iContactIdx];
                                    if (strContactRecord.Length == 0)
                                    {
                                        iContactIdx++;
                                        continue;
                                    }

                                    String value = strContactRecord;

                                    String sUID = value.Substring(0, value.IndexOf("|")); // usrID
                                    value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                                    String sUName = value;

                                    this.pbProgress.Text = "Персонаж " + iPersoneIdx.ToString() + "/" + mFormMain.lstPersoneChangeOriginal.Count.ToString() + " (" + sName_Persone + ") - " + iContactIdx.ToString() + "/" + lstListCont.Count.ToString() + (iContactVisualIdx > 0 ? " - " + iContactVisualIdx.ToString() : "");
                                    this.Text = this.pbProgress.Text;
                                    Application.DoEvents();

                                    Boolean bEquals = bRQVEmpty;

                                    List<String> lstContHarValues = new List<String>();
                                    bool bFileHarExists = false;
                                    if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + sUID_Persone + "_" + sUID + ".txt")))
                                    {
                                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + sUID_Persone + "_" + sUID + ".txt"));
                                        lstContHarValues = new List<String>(srcFile);
                                        bFileHarExists = true;
                                    }

                                    if (!bEquals)
                                    {
                                        if (bFileHarExists)
                                        {
                                            String[] EQV = new String[iContHarCount];
                                            foreach (String str in lstContHarValues)
                                            {
                                                if (str == null)
                                                    continue;

                                                if (str.Length == 0)
                                                    continue;

                                                EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1).ToLower();
                                            }

                                            bEquals = true;
                                            for (int iv = 0; iv < RQV.Length; iv++)
                                            {
                                                if (RQV[iv].Length == 0/* || EQV[iv].Length == 0*/)
                                                    continue;

                                                if (!(RQV[iv].ToLower().Equals(EQV[iv]) || EQV[iv].StartsWith(RQV[iv].ToLower())))
                                                {
                                                    //---
                                                    if (iAgeField == iv)
                                                    {
                                                        String sAge = EQV[iv];
                                                        if (sUnknownAge.Equals(sAge))
                                                        {
                                                            bEquals = bAcceptEmptyAge;
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                int iAge = Convert.ToInt32(sAge);
                                                                if (iAge < iAcceptAgeMin || iAge > iAcceptAgeMax)
                                                                    bEquals = false;
                                                            }
                                                            catch
                                                            {
                                                                bEquals = false;
                                                            }
                                                        }

                                                        if (!bEquals)
                                                            break;
                                                    }
                                                    else
                                                    {
                                                        //+---
                                                        if (RQV[iv].IndexOf("~") >= 0 || RQV[iv].IndexOf("|") >= 0)
                                                        {
                                                            bEquals = false;
                                                            String sFilter = RQV[iv].ToLower();
                                                            bool bInverse = false;
                                                            String sValue = EQV[iv].Trim();
                                                            if (sFilter[0] == '~')
                                                            {
                                                                bInverse = true;
                                                                bEquals = true;
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
                                                                            bEquals = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (sPart.Equals(sValue))
                                                                        {
                                                                            bEquals = false;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                        else if (RQV[iv].IndexOf("-") >= 0)
                                                        {
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

                                                                sval = EQV[iv].Trim();
                                                                try
                                                                {
                                                                    int ival = Convert.ToInt32(sval);
                                                                    if (ival < ivalmin || ival > ivalmax)
                                                                        bEquals = false;
                                                                }
                                                                catch
                                                                {
                                                                    bEquals = false;
                                                                }
                                                            }
                                                            catch
                                                            {
                                                                bEquals = false;
                                                            }
                                                        }
                                                        else
                                                            bEquals = false;
                                                        //---
                                                        if (!bEquals)
                                                            break;
                                                        //+---
                                                    }
                                                }
                                            }
                                        }
                                        else
                                            bEquals = false;
                                    }

                                    if (bEquals)
                                    {
                                        if (iOperation == 0) // Delete
                                        {
                                            lstListCont.RemoveAt(iContactIdx);
                                            ExternalList_RemoveUserFiles(sUID_Persone, sUID);
                                            bChanged = true;
                                        }
                                        else if (iOperation == 10 || iOperation == 11)
                                        {
                                            iContactVisualIdx++;

                                            if (iOperation == 10)
                                            {
                                                lstContHar.Add("<H2>" + iPersoneVisualIdx.ToString() + "." + iContactVisualIdx.ToString() + ". " + sUName + "</H2>");
                                                lstContHar.Add("<UL>");
                                            }
                                            else
                                                lstCurrentContacterHarValues = new List<string>();

                                            for (int i = 0; i < mFormMain.iContHarCount; i++)
                                            {
                                                String sHarName = mFormMain.sContHar[i, 1];
                                                String sHarValue = GetPersoneParametersValue(lstContHarValues, mFormMain.sContHar[i, 0]);
                                                if (iOperation == 10)
                                                    lstContHar.Add("<LI><B>" + sHarName + ":<B> " + sHarValue + "</LI>");
                                                else if (iOperation == 11)
                                                    lstCurrentContacterHarValues.Add(sHarValue);
                                            }
                                            if (iOperation == 10)
                                                lstContHar.Add("</UL>");

                                            List<String> lstMsgsLst = ReadAllUserMessagesContacter(Convert.ToInt64(sUID), Convert.ToInt64(sUID_Persone), iOperation == 10);
                                            String sDialogText = "";
                                            if (lstMsgsLst.Count > 0)
                                            {
                                                if (iOperation == 10)
                                                {
                                                    lstContHar.Add("<P>");
                                                    foreach (string str in lstMsgsLst)
                                                        lstContHar.Add(str);

                                                    lstContHar.Add("</P>");
                                                }
                                                else if (iOperation == 11)
                                                {
                                                    foreach (string str in lstMsgsLst)
                                                        sDialogText += str + "\n";

                                                    if (sDialogText.Length > 0)
                                                        sDialogText = sDialogText.Substring(0, sDialogText.Length - 1);
                                                }
                                            }

                                            if (iOperation == 11)
                                            {
                                                // вывод строки CSV
                                                //lstHTMLFileText.Add("Имя персонажа;Id персонажа;Имя контактёра;Id контактёра;Проект;Роль в общении;Отношение персонажа к контактёру;Отношение контактёра к персонажу;Алгоритм;Стадия общения;Диалог");
                                                lstContHar.Add(CSV_formatvalue(sName_Persone) + ";" + CSV_formatvalue("https://vk.com/id" + sUID_Persone) + ";" + CSV_formatvalue(sUName) + ";" + CSV_formatvalue("https://vk.com/id" + sUID) + ";" + CSV_getPersoneHarValue(lstCurrentPersoneHarValues, 11) + ";" + CSV_getContacterHarValue(lstCurrentContacterHarValues, 8) + ";" + CSV_getContacterHarValue(lstCurrentContacterHarValues, 11) + ";" + CSV_getContacterHarValue(lstCurrentContacterHarValues, 10) + ";" + CSV_getContacterHarValue(lstCurrentContacterHarValues, 15) + ";" + CSV_getContacterHarValue(lstCurrentContacterHarValues, 9) + ";" + CSV_formatvalue(sDialogText));
                                            }

                                            iContactIdx++;
                                        }
                                        else if (iOperation == 1) // Edit
                                        {
                                            if (lstContHarValues.Count > 0)
                                            {
                                                for (int i = 0; i < iContHarCount; i++)
                                                {
                                                    String sCV = lstContHar[i].Substring(lstContHar[i].IndexOf("|") + 1);
                                                    if (sCV.Length > 0)
                                                    {
                                                        if (sCV == "#clear#")
                                                            lstContHarValues[i] = lstContHar[i].Replace("#clear#", "");
                                                        else
                                                            lstContHarValues[i] = lstContHar[i];
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int i = 0; i < iContHarCount; i++)
                                                    lstContHarValues.Add(lstContHar[i]);
                                            }
                                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + sUID_Persone + "_" + sUID + ".txt"), lstContHarValues, Encoding.UTF8);
                                            iContactIdx++;
                                        }
                                        else if (iOperation == 2 || iOperation == 3) // Update status
                                        {
                                            VkNet.Model.User usrAdr = null;
                                            /*
                                            do
                                            {
                                                try
                                                {
                                                    usrAdr = FormMain.api.Users.Get(Convert.ToInt64(sUID), ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.City | ProfileFields.Sex | ProfileFields.BirthDate | ProfileFields.Country | ProfileFields.Relation | ProfileFields.Online | ProfileFields.Counters | ProfileFields.LastSeen);
                                                    break;
                                                }
                                                catch (Exception exp)
                                                {
                                                    mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                                                    break;
                                                }
                                            }
                                            while (true);
                                            */

                                            if (usrAdr != null)
                                            {
                                                sUName = usrAdr.FirstName + " " + usrAdr.LastName;
                                                this.pbProgress.Text = "Персонаж " + sName_Persone + " (" + iPersoneIdx.ToString() + " / " + mFormMain.lstPersoneChangeOriginal.Count.ToString() + ")" + ", Контактер " + sUName + " (" + (iContactIdx + 1).ToString() + "/" + lstListCont.Count.ToString() + ")";
                                                this.Text = this.pbProgress.Text;
                                                Application.DoEvents();

                                                if (lstContHarValues.Count > 0)
                                                {
                                                    for (int i = 0; i < iContHarCount; i++)
                                                    {
                                                        String sCV = lstContHar[i].Substring(lstContHar[i].IndexOf("|") + 1);
                                                        if (sCV.Length > 0)
                                                            lstContHarValues[i] = lstContHar[i];
                                                    }
                                                }
                                                else
                                                {
                                                    for (int i = 0; i < iContHarCount; i++)
                                                        lstContHarValues.Add(lstContHar[i]);
                                                }


                                                for (int i = 0; i < iContHarCount; i++)
                                                {
                                                    if (lstContHarValues[i].IndexOf("#sex#") > 0)
                                                    {
                                                        String svkv = usrAdr.Sex != null ? (usrAdr.Sex == Sex.Male ? "Мужской" : (usrAdr.Sex == Sex.Female ? "Женский" : "Не указан")) : "Не указан";
                                                        lstContHarValues[i] = lstContHarValues[i].Replace("#sex#", svkv);
                                                    }
                                                    else if (lstContHarValues[i].IndexOf("#city#") > 0)
                                                    {
                                                        lstContHarValues[i] = lstContHarValues[i].Replace("#city#", usrAdr.City != null ? usrAdr.City.Title : "");
                                                    }
                                                    else if (lstContHarValues[i].IndexOf("#country#") > 0)
                                                    {
                                                        lstContHarValues[i] = lstContHarValues[i].Replace("#country#", usrAdr.Country != null ? usrAdr.Country.Title : "");
                                                    }
                                                    else if (lstContHarValues[i].IndexOf("#relation#") > 0)
                                                    {
                                                        lstContHarValues[i] = lstContHarValues[i].Replace("#relation#", usrAdr.Relation != null ? (usrAdr.Relation == RelationType.Amorous ? "Влюблен(-а)" : (usrAdr.Relation == RelationType.Engaged ? "Помолвлен(-а)" : (usrAdr.Relation == RelationType.HasFriend ? "Есть друг (подруга)" : (usrAdr.Relation == RelationType.InActiveSearch ? "В активном поиске" : (usrAdr.Relation == RelationType.ItsComplex ? "Все сложно" : (usrAdr.Relation == RelationType.Married ? "Женат (замужем)" : (usrAdr.Relation == RelationType.NotMarried ? "Не женат (не замужем)" : "Не указано"))))))) : "Не указано");
                                                    }
                                                    else if (lstContHarValues[i].IndexOf("#online#") > 0)
                                                    {
                                                        lstContHarValues[i] = lstContHarValues[i].Replace("#online#", usrAdr.Online != null ? (usrAdr.Online.Value ? "ON line" : "OFF line") : "Unknown");
                                                    }
                                                    else if (lstContHarValues[i].IndexOf("#birthdate#") > 0)
                                                    {
                                                        lstContHarValues[i] = lstContHarValues[i].Replace("#birthdate#", usrAdr.BirthDate != null ? usrAdr.BirthDate : "");
                                                    }
                                                    else if (lstContHarValues[i].IndexOf("#counters_friends#") > 0)
                                                    {
                                                        lstContHarValues[i] = lstContHarValues[i].Replace("#counters_friends#", usrAdr.Counters != null ? usrAdr.Counters.Friends.Value.ToString() : "");
                                                    }
                                                    else if (lstContHarValues[i].IndexOf("#age#") > 0)
                                                    {
                                                        iAgeField = i;
                                                        String birthdate = usrAdr.BirthDate != null ? usrAdr.BirthDate : "";
                                                        String sAge = sUnknownAge;
                                                        if (birthdate.Length > 0)
                                                        {
                                                            String sDD = birthdate.Substring(0, birthdate.IndexOf("."));
                                                            birthdate = birthdate.Substring(birthdate.IndexOf(".") + 1);
                                                            if (birthdate.IndexOf(".") > 0)
                                                            {
                                                                String sMM = birthdate.Substring(0, birthdate.IndexOf("."));
                                                                birthdate = birthdate.Substring(birthdate.IndexOf(".") + 1);
                                                                DateTime bday = new DateTime(Convert.ToInt32(birthdate), Convert.ToInt32(sMM), Convert.ToInt32(sDD));
                                                                DateTime today = DateTime.Today;
                                                                int age = today.Year - bday.Year;
                                                                if (bday > today.AddYears(-age)) age--;
                                                                sAge = Convert.ToString(age);
                                                            }
                                                        }

                                                        lstContHarValues[i] = lstContHarValues[i].Replace("#age#", sAge);
                                                    }
                                                    else if (lstContHarValues[i].IndexOf("#clear#") > 0)
                                                    {
                                                        lstContHarValues[i] = lstContHarValues[i].Replace("#clear#", "");
                                                    }

                                                }

                                            }

                                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + sUID_Persone + "_" + sUID + ".txt"), lstContHarValues, Encoding.UTF8);
                                            iContactIdx++;
                                        }
                                    }
                                    else
                                        iContactIdx++;

                                }

                                if (bChanged)
                                {
                                    if (lstListCont.Count > 0)
                                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + sUID_Persone + ".txt"), lstListCont, Encoding.UTF8);
                                    else
                                        File.Delete(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + sUID_Persone + ".txt"));
                                }
                            }
                        }//if (sPersonenList.IndexOf("|" + sUID_Persone + "|") >= 0)
                    } //if (strPersoneRecord.Length > 0)
                }
            }//if (File.Exists(Path.Combine(FormMain.sDataPath, "_personen.txt")))

            if ((iOperation == 10 || iOperation == 11) && bNeedReauthorize)
            {
                mFormMain.AutorizeVK(mFormMain.userLogin, mFormMain.userPassword);
            }

            this.pbProgress.Visible = false;
            this.bDoImportContacts = true;
            bDoGroupOperation = true;
            this.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_21", this.Name, "Редактирование базы Контактеров");
            button9.Text = ">";
            EnableAllButtons();
            ContactsList_Load();
            ApplyFilter();
        }

        private void ExternalList_RemoveUserFiles(String personeID, String sUD)
        {
            if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + personeID + "_" + sUD + ".txt")))
                File.Delete(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + personeID + "_" + sUD + ".txt"));

            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + personeID + "_" + sUD + ".txt")))
                File.Delete(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + personeID + "_" + sUD + ".txt"));

            if (File.Exists(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + personeID + "_" + sUD + ".txt")))
                File.Delete(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + personeID + "_" + sUD + ".txt"));

            if (File.Exists(Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + FormMain.getSocialNetworkPrefix() + personeID + "_" + sUD + ".txt")))
                File.Delete(Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + FormMain.getSocialNetworkPrefix() + personeID + "_" + sUD + ".txt"));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (tbAllPersonenInRotation.Checked)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить всех подпадающих под фильтр видимости Контактеров у всех Персонажей в ротации?", "Удаление Контактеров", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    applyOperationForAllContactsToPersonenInRotation(0);
                }
                return;
            }
            if (gridCheckedCount <= 0)
            {
                if (gridSelectedIndex > 0)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Вы уверены, что хотите удалить этого Контактера?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Контактера"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bool bCont = true;
                        int iSelIdx = gridSelectedIndex;
                        String sUID = getItemID(iSelIdx);

                        if (FormMain.SocialNetwork == 1 && sUID.Equals("1") && iPersUserID == 0)
                        {
                            MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "Контактер OPERATOR не может быть удален."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Контактера"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        if (sUID.Equals(iContUserID.ToString()))
                        {
                            bCont = false;
                            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_7", this.Name, "Этот Контактер сейчас активен.\n\nВы уверены, что хотите удалить активного Контактера?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Контактера"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                bCont = true;
                        }

                        if (bCont)
                        {
                            if (sUID.Length > 0)
                            {
                                grid1.Rows.Remove(iSelIdx);
                                ContactsList_RemoveUser(sUID);

                                if (grid1.RowsCount > 1)
                                {
                                    if (iSelIdx >= grid1.RowsCount - 1)
                                        iSelIdx--;
                                    selectRow(iSelIdx);
                                }
                                else
                                {
                                    button2.Enabled = tbAllPersonenInRotation.Checked;
                                    button5.Enabled = tbAllPersonenInRotation.Checked;
                                    button3.Enabled = false;
                                    button11.Enabled = false;
                                    button16.Enabled = false;
                                    button23.Enabled = false;
                                    buttonSelectContacters.Enabled = false;
                                    toolStripButtonCopyToClipboard.Enabled = false;
                                    buttonCopyParameters.Enabled = false;
                                    buttonPasteParameters.Enabled = false;
                                }
                                grid_ItemChecked();

                            }
                        }
                    }
                }
            }
            else
            {
                if (gridCheckedCount > 0)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_8", this.Name, "Вы уверены, что хотите удалить всех отмеченных Контактеров?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Контактеров"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        foreach (int idx in gridCheckedIndexes)
                        {
                            String sUID = getItemID(idx);
                            if (!(FormMain.SocialNetwork == 1 && sUID.Equals("1")))
                                ContactsList_RemoveUser(sUID);
                        }

                        List<int> indexes = gridCheckedIndexes.OrderByDescending(i => i).ToList();
                        foreach (int idx in indexes)
                        {
                            String sUID = getItemID(idx);
                            if (!(FormMain.SocialNetwork == 1 && sUID.Equals("1")))
                                grid1.Rows.Remove(idx);
                        }
                        gridCheckedIndexes.Clear();
                        gridCheckedCount = 0;

                        EnableAllButtons();
                        ApplyFilter();
                    }
                }
            }
        }

        private String ResolveID(String text)
        {
            if (text.Length > 0)
            {
                try
                {
                    long _id = Convert.ToInt64(text);
                    return text;
                }
                catch
                {
                    if (text.StartsWith("http://vk.com/id"))
                    {
                        return ResolveID(text.Substring("http://vk.com/id".Length));
                    }
                    else if (text.StartsWith("http://vk.com/club"))
                    {
                        return ResolveID(text.Substring("http://vk.com/club".Length));
                    }
                    else if (text.StartsWith("http://vk.com/"))
                    {
                        return ResolveID(text.Substring("http://vk.com/".Length));
                    }
                    else if (text.StartsWith("https://vk.com/id"))
                    {
                        return ResolveID(text.Substring("https://vk.com/id".Length));
                    }
                    else if (text.StartsWith("https://vk.com/club"))
                    {
                        return ResolveID(text.Substring("https://vk.com/club".Length));
                    }
                    else if (text.StartsWith("https://vk.com/"))
                    {
                        return ResolveID(text.Substring("https://vk.com/".Length));
                    }
                    else
                    {
                        /*
                        do
                        {
                            try
                            {
                                VkNet.Model.VkObject obj = FormMain.api.Utils.ResolveScreenName(text);
                                if (obj.Id.HasValue)
                                    return obj.Id.Value.ToString();
                                break;
                            }
                            catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                            {
                                if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                                    break;
                            }
                            catch (System.Net.WebException)
                            {
                                if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                                    break;
                            }
                            catch (VkNet.Exception.VkApiException)
                            {
                                if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                                    break;
                            }
                            catch (Exception exp)
                            {
                                mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                                break;
                            }
                        }
                        while (true);
                        */
                    }
                }
            }
            return "-1";
        }

        private void addContButton_Click1(object sender, EventArgs e)
        {
            string value = "";
            if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Добавление Контактера"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_10", this.Name, "Введите ID Контактера:"), ref value) == DialogResult.OK)
            {
                String sUID = ResolveID(value);
                if (ContactsList_GetUserIdx(sUID) >= 0)
                {
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Контактер с указанным ID уже есть в базе. Если он не отображается, смените настройки фильтра базы контактеров по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Добавление Контактера"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (FormMain.SocialNetwork == 1)
                    {
                        String srec = NILSA_getUserRecord(sUID);
                        if (srec != null)
                        {
                            String sUName = NILSA_GetFieldFromStringRec(srec, 1);

                            ContactsList_AddUser(sUID, sUName);
                            ContactsList_AddUserToVisualList(sUID, sUName);

                            selectRow(grid1.RowsCount - 1);
                            grid_ItemChecked();
                        }
                        else
                            MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_12", this.Name, "Ошибка запроса Контактера с указанным ID..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Добавление Контактера"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (FormMain.SocialNetwork == 0)
                    {
                        FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                        fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                        for (int i = 0; i < iContHarCount; i++)
                        {
                            for (int j = 0; j < iContHarAttrCount; j++)
                                fe.sPersHar[i, j] = sContHar[i, j];
                            fe.sPersHar[i, iContHarAttrCount] = "";
                        }

                        LoadFormEditPersHarValues(fe, 100000, 1);

                        fe.iPersHarAttrCount = iContHarAttrCount;
                        fe.iPersHarCount = iContHarCount;
                        fe.sFilePrefix = "cont";
                        fe.Setup();

                        if (fe.ShowDialog() == DialogResult.OK)
                        {
                            SaveFormEditPersHarValues(fe, 100000, 1);
                            List<String> lstContHar = new List<String>();
                            for (int i = 0; i < iContHarCount; i++)
                                lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);

                            mFormMain.fwbVKontakte.Setup(mFormMain.userLogin, mFormMain.userPassword, WebBrowserCommand.GetContactAttributes, Convert.ToInt64(sUID));
                            mFormMain.fwbVKontakte.WaitResult();
                            FormWebBrowser.Persone usrAdr = mFormMain.fwbVKontakte.contactAtrributes;

                            if (usrAdr != null)
                            {
                                if (usrAdr.FirstName.Length == 0 && usrAdr.LastName.Length == 0)
                                    usrAdr = null;
                            }

                            if (usrAdr != null)
                            {

                                String sUName = usrAdr.FirstName + " " + usrAdr.LastName;

                                List<String> lstContHarValues = new List<String>();

                                if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                                    lstContHarValues = new List<String>(srcFile);
                                    for (int i = 0; i < iContHarCount; i++)
                                    {
                                        String sCV = lstContHar[i].Substring(lstContHar[i].IndexOf("|") + 1);
                                        //if (sCV.Length > 0)
                                        //    lstContHarValues[i] = lstContHar[i];
                                        if (sCV.Length > 0)
                                        {
                                            if (sCV.Equals("#sex#"))
                                            {
                                                String sexValue = lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1);
                                                if (sexValue.Length == 0 || sexValue.ToLower().Equals("не указан"))
                                                    lstContHarValues[i] = lstContHar[i];
                                            }
                                            else
                                                lstContHarValues[i] = lstContHar[i];
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < iContHarCount; i++)
                                        lstContHarValues.Add(lstContHar[i]);
                                }

                                for (int i = 0; i < iContHarCount; i++)
                                {
                                    if (lstContHarValues[i].IndexOf("#sex#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#sex#", usrAdr.Sex);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#city#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#city#", usrAdr.City);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#age#") > 0)
                                    {
                                        String birthdate = usrAdr.BirthDate != null ? usrAdr.BirthDate : "";
                                        String sAge = sUnknownAge;
                                        if (birthdate.Length > 0)
                                        {
                                            String sDD = birthdate.Substring(0, birthdate.IndexOf("."));
                                            birthdate = birthdate.Substring(birthdate.IndexOf(".") + 1);
                                            if (birthdate.IndexOf(".") > 0)
                                            {
                                                String sMM = birthdate.Substring(0, birthdate.IndexOf("."));
                                                birthdate = birthdate.Substring(birthdate.IndexOf(".") + 1);
                                                DateTime bday = new DateTime(Convert.ToInt32(birthdate), Convert.ToInt32(sMM), Convert.ToInt32(sDD));
                                                DateTime today = DateTime.Today;
                                                int age = today.Year - bday.Year;
                                                if (bday > today.AddYears(-age)) age--;
                                                sAge = Convert.ToString(age);
                                            }
                                        }

                                        lstContHarValues[i] = lstContHarValues[i].Replace("#age#", sAge);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#birthdate#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#birthdate#", usrAdr.BirthDate != null ? usrAdr.BirthDate : "");
                                    }
                                    else if (lstContHarValues[i].IndexOf("#relation#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#relation#", usrAdr.Relation);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#country#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#country#", usrAdr.Country);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#online#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#online#", usrAdr.Online);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#counters_friends#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#counters_friends#", usrAdr.CountersFriends);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#clear#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#clear#", "");
                                    }

                                }

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarValues, Encoding.UTF8);
                                ContactsList_AddUser(sUID, sUName);
                                ContactsList_AddUserToVisualList(sUID, sUName);

                                selectRow(grid1.RowsCount - 1);
                                grid_ItemChecked();
                            }
                            else
                            {
                                MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_12", this.Name, "Ошибка запроса Контактера с указанным ID..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Добавление Контактера"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void addContButton_Click(object sender, EventArgs e)
        {
            if (FormMain.SocialNetwork != 3) return;
            FormAddCont formAddCont = new FormAddCont();
            if (formAddCont.ShowDialog() == DialogResult.Cancel) return;
            var cid = formAddCont.Persone.Login;
            var firstName = formAddCont.Persone.FirstName;
            var lastName = formAddCont.Persone.LastName;
            var socialNetwork = formAddCont.Persone.Owner;
            var isUniq = true;
            foreach (var cont in lstContactsList)
            {
                if (cont.Contains(cid))
                {
                    isUniq = false;
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Контактер с указанным ID уже есть в базе. Если он не отображается, смените настройки фильтра базы контактеров по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Добавление Контактера"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
            }
            if (isUniq && (cid.Length != 0 || firstName.Length != 0 || lastName.Length != 0))
            {
                var currentContId = lstContactsList.Count()+1;
                var currentListContHar = new List<string>();
                for (int i = 0; i < iContHarCount; i++)
                    currentListContHar.Add($"{i+1}|");
                currentListContHar[5] += cid.ToString();
                currentListContHar[7] += socialNetwork;
                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + currentContId + ".txt"), currentListContHar, Encoding.UTF8);
                ContactsList_AddUser(currentContId.ToString(), firstName + " " + lastName, cid);
                ContactsList_AddUserToVisualList(currentContId.ToString(), firstName + " " + lastName);

                selectRow(grid1.RowsCount - 1);
                grid_ItemChecked();
            }
        }

        List<String> lstNILSA_UserDB;
        private void NILSA_LoadUserDB()
        {
            lstNILSA_UserDB = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "nilsa_userdb.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "nilsa_userdb.txt"));
                lstNILSA_UserDB = new List<String>(srcFile);
            }
            else
            {
                lstNILSA_UserDB.Add("0|NILSA|nilsa|nilsa");
                lstNILSA_UserDB.Add("1|OPERATOR|operator|operator");
                NILSA_SaveUserDB();
            }
        }

        private void NILSA_SaveUserDB()
        {
            if (lstNILSA_UserDB.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "nilsa_userdb.txt"), lstNILSA_UserDB, Encoding.UTF8);

        }

        public String NILSA_getUserRecord(String uID)
        {
            NILSA_LoadUserDB();
            foreach (String srec in lstNILSA_UserDB)
            {
                if (srec.StartsWith(uID + "|"))
                    return srec;
            }
            return null;
        }

        private String NILSA_GetFieldFromStringRec(String srec, int iFieldIdx)
        {
            String retval = srec + "|";
            if (srec.Length > 0)
            {
                for (int i = 0; i < iFieldIdx; i++)
                    retval = retval.Substring(retval.IndexOf("|") + 1);
                retval = retval.Substring(0, retval.IndexOf("|"));
                return retval;
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

        private void LoadContactParamersValues(String sID)
        {
            lstContHarValues = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sID + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sID + ".txt"));
                lstContHarValues = new List<String>(srcFile);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tbAllPersonenInRotation.Checked)
            {
                List<String> lstContHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_14", this.Name, "Замена/установка значений характеристик Контактеров"), -1);
                if (lstContHar == null)
                    return;

                if (MessageBox.Show("Вы уверены, что хотите заменить/установить значения характеристик всех подпадающих под фильтр видимости Контактеров у всех Персонажей в ротации?", "Замена/установка значений характеристик Контактеров", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    applyOperationForAllContactsToPersonenInRotation(1, lstContHar);
                }
                return;
            }
            if (gridCheckedCount <= 0)
            {
                if (gridSelectedIndex > 0)
                {
                    int iSelIdx = gridSelectedIndex;
                    String sUID = getItemID(iSelIdx);

                    if (sUID.Length > 0)
                    {
                        LoadContactParamersValues(sUID);
                        FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                        fe.Text += " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_13", this.Name, "Контактера");
                        fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                        for (int i = 0; i < iContHarCount; i++)
                        {
                            for (int j = 0; j < iContHarAttrCount; j++)
                                fe.sPersHar[i, j] = sContHar[i, j];
                            fe.sPersHar[i, iContHarAttrCount] = GetContactParametersValue(sContHar[i, 0]);
                        }

                        fe.iPersHarAttrCount = iContHarAttrCount;
                        fe.iPersHarCount = iContHarCount;
                        fe.sFilePrefix = "cont";
                        fe.Setup();

                        if (fe.ShowDialog() == DialogResult.OK)
                        {
                            List<String> lstContHar = new List<String>();
                            for (int i = 0; i < iContHarCount; i++)
                                lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHar, Encoding.UTF8);
                            ContactsList_AddUserToVisualList(sUID, getItemName(iSelIdx), iSelIdx);
                        }
                    }
                }
            }
            else
            {
                if (gridCheckedCount > 0)
                {
                    List<String> lstContHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_14", this.Name, "Замена/установка значений характеристик Контактеров"), -1);
                    if (lstContHar == null)
                        return;

                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_15", this.Name, "Вы уверены, что хотите задать характеристики для всех отмеченных Контактеров?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_16", this.Name, "Редактирование характеристик Контактеров"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        foreach (int k in gridCheckedIndexes)
                        {
                            String sUID = getItemID(k);
                            String sUName = getItemName(k);

                            List<String> lstContHarCurrent = new List<String>();
                            if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                                lstContHarCurrent = new List<String>(srcFile);
                                for (int i = 0; i < iContHarCount; i++)
                                {
                                    String sCV = lstContHar[i].Substring(lstContHar[i].IndexOf("|") + 1);
                                    if (sCV.Length > 0)
                                    {
                                        if (sCV == "#clear#")
                                            lstContHarCurrent[i] = lstContHar[i].Replace("#clear#", "");
                                        else
                                            lstContHarCurrent[i] = lstContHar[i];
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < iContHarCount; i++)
                                    lstContHarCurrent.Add(lstContHar[i]);
                            }

                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                            ContactsList_AddUserToVisualList(sUID, sUName, k);
                            ContactsList_AddUser(sUID, sUName);
                        }

                        EnableAllButtons();
                    }
                }

            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (bDoImportContacts)
            {
                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);
                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                iImportMode = 1;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                for (int i = 0; i < iContHarCount; i++)
                {
                    for (int j = 0; j < iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = sContHar[i, j];
                    fe.sPersHar[i, iContHarAttrCount] = "";
                }
                LoadFormEditPersHarValues(fe, iImportMode, 1);

                fe.iPersHarAttrCount = iContHarAttrCount;
                fe.iPersHarCount = iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValues(fe, iImportMode, 1);
                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < iContHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                    List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                    if (lstFilterHar != null)
                        StartImportContactsFromGroupThread("UserID=" + iPersUserID.ToString(), lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, true, nudDuplicatesAction);
                }

            }
            else
            {
                this.button7.Enabled = false;
                this.bwProgress.CancelAsync();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (gridSelectedIndex > 0)
            {
                int iSelIdx = gridSelectedIndex;
                String sUID = getItemID(iSelIdx);

                if (sUID.Length > 0)
                {
                    bNeedPersoneChange = true;
                    iContUserID = Convert.ToInt64(sUID);
                    DialogResult = DialogResult.OK;
                }
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (bDoImportContacts)
            {
                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);
                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                iImportMode = 0;
                string value = NilsaUtils.LoadStringValue(0, "");
                if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_18", this.Name, "Импорт из Группы"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_19", this.Name, "Введите ID Группы:"), ref value) == DialogResult.OK)
                {
                    NilsaUtils.SaveStringValue(0, value);
                    value = ResolveID(value);
                    FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                    fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        for (int j = 0; j < iContHarAttrCount; j++)
                            fe.sPersHar[i, j] = sContHar[i, j];
                        fe.sPersHar[i, iContHarAttrCount] = "";
                    }
                    LoadFormEditPersHarValues(fe, iImportMode, 1);

                    fe.iPersHarAttrCount = iContHarAttrCount;
                    fe.iPersHarCount = iContHarCount;
                    fe.sFilePrefix = "cont";
                    fe.Setup();

                    if (fe.ShowDialog() == DialogResult.OK)
                    {
                        SaveFormEditPersHarValues(fe, iImportMode, 1);
                        List<String> lstContHar = new List<String>();
                        for (int i = 0; i < iContHarCount; i++)
                            lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                        List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                        if (lstFilterHar != null)
                            StartImportContactsFromGroupThread("GroupID=" + value, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, true, nudDuplicatesAction);
                    }

                }
            }
            else
            {
                this.button8.Enabled = false;
                this.bwProgress.CancelAsync();
            }

        }

        private void DisableAllButtons()
        {
            //this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            this.addContButton.Enabled = false;
            buttonBaseFilter.Enabled = false;
            this.button7.Enabled = false;
            this.button8.Enabled = false;
            this.button9.Enabled = false;
            this.button10.Enabled = false;
            this.button11.Enabled = false;
            this.button12.Enabled = false;
            this.button13.Enabled = false;
            this.button14.Enabled = false;
            this.button15.Enabled = false;
            this.button16.Enabled = false;
            this.button17.Enabled = false;
            this.button18.Enabled = false;
            buttonExportContacterServer.Enabled = false;
            this.button19.Enabled = false;
            this.button20.Enabled = false;
            this.button21.Enabled = false;
            this.button22.Enabled = false;
            this.button23.Enabled = false;
            this.button24.Enabled = false;
            this.button25.Enabled = false;
            this.button26.Enabled = false;
            buttonCopyParameters.Enabled = false;
            buttonPasteParameters.Enabled = false;
            buttonImportContacterServer.Enabled = false;
            toolStripButtonSearchDuplicate.Enabled = false;
            toolStripButtonCopyToClipboard.Enabled = false;
            buttonImportPersonenAsContacter.Enabled = false;
            buttonSelectContacters.Enabled = false;
            tbAllPersonenInRotation.Enabled = false;
            exportDialogsFromPersonen.Enabled = false;
            reportDialogsFromPersonen.Enabled = false;

        }

        private void EnableAllButtons()
        {
            //this.button1.Enabled = true;
            this.button2.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0 || tbAllPersonenInRotation.Checked;
            this.button23.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            this.button3.Enabled = gridSelectedIndex > 0;
            this.button4.Enabled = true;
            this.button5.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0 || tbAllPersonenInRotation.Checked;
            this.addContButton.Enabled = true;
            buttonBaseFilter.Enabled = true;
            this.button7.Enabled = FormMain.SocialNetwork == 0;
            buttonImportPersonenAsContacter.Enabled = FormMain.SocialNetwork == 0;
            this.button8.Enabled = FormMain.SocialNetwork == 0;
            this.button9.Enabled = true;
            this.button11.Enabled = FormMain.SocialNetwork == 0;
            this.button15.Enabled = true;
            this.button26.Enabled = FormMain.SocialNetwork == 0;
            buttonImportContacterServer.Enabled = FormMain.SocialNetwork == 0;
            this.button16.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            buttonSelectContacters.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0;
            this.button19.Enabled = FormMain.SocialNetwork == 0;
            toolStripButtonCopyToClipboard.Enabled = gridSelectedIndex > 0;
            buttonCopyParameters.Enabled = gridSelectedIndex > 0;
            buttonPasteParameters.Enabled = copyedParameters != null && (gridSelectedIndex > 0 || gridCheckedCount > 0);
            grid_SelectedIndexChanged();
            grid_ItemChecked();
            tbAllPersonenInRotation.Enabled = mFormMain.lstPersoneChangeOriginal.Count > 0;

            exportDialogsFromPersonen.Enabled = mFormMain.lstPersoneChangeOriginal.Count > 0;
            reportDialogsFromPersonen.Enabled = mFormMain.lstPersoneChangeOriginal.Count > 0;
        }

        private void StartImportContactsFromGroupThread(String sGID, List<String> lstContHar, List<String> lstFilterHar, String sPersUsersIDs, int maxContacterCount, bool _bImportMode, int _nudDuplicatesAction = 0)
        {
            // This method runs on the main thread.
            this.pbProgress.Visible = true;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Value = 0;
            this.pbProgress.Maximum = 10;
            this.bDoImportContacts = false;

            DisableAllButtons();
            button9.Text = "X";
            button9.Enabled = true;

            // Initialize the object that the background worker calls.
            ImportContactsFromGroup WC = new ImportContactsFromGroup(this, mFormMain, _bImportMode);
            WC.sGroupID = sGID;
            WC.iPersUserID = iPersUserID;
            WC.sPersUsersIDs = sPersUsersIDs;
            WC.lstContHar = lstContHar;
            WC.lstFilterHar = lstFilterHar;
            WC.iContHarCount = iContHarCount;
            WC.maxContacterCount = maxContacterCount;
            WC.nudDuplicatesAction = _nudDuplicatesAction;
            // Start the asynchronous operation.
            bwProgress.RunWorkerAsync(WC);
        }

        delegate void StringParameterDelegate(string uid, string uname, int idx);
        public void UpdateProgress_AddUserToVisualList(string uid, string uname, int idx)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new StringParameterDelegate(UpdateProgress_AddUserToVisualList), new object[] { uid, uname, idx });
                return;
            }
            // Must be on the UI thread if we've got this far
            ContactsList_AddUserToVisualList(uid, uname, idx);
        }

        delegate int UpdateProgress_GetVisualListIdxDelegate(string uid);
        public int UpdateProgress_GetVisualListIdx(string uid)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                return (int)this.Invoke(new UpdateProgress_GetVisualListIdxDelegate(ContactsList_GetVisualListIdx), new object[] { uid });
            }
            // Must be on the UI thread if we've got this far
            else
                return ContactsList_GetVisualListIdx(uid);
        }

        private void bwProgress_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;

            // Get the Words object and call the main method.
            ImportContactsFromGroup WC = (ImportContactsFromGroup)e.Argument;
            stateImported = null;
            WC.ImportContacts(worker, e);
        }

        ImportContactsFromGroup.CurrentState stateImported = null;
        private void bwProgress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            stateImported = (ImportContactsFromGroup.CurrentState)e.UserState;
            if (this.pbProgress.Maximum != stateImported.ContactsTotal)
                this.pbProgress.Maximum = stateImported.ContactsTotal;
            this.pbProgress.Value = stateImported.ContactsImported;
            this.pbProgress.Text = stateImported.ContactsImported.ToString() + "/" + stateImported.ContactsTotal.ToString() + " (" + stateImported.ContactsAdded.ToString() + ")";

            this.Text = stateImported.sUName + (stateImported.dtLastSeen.Length>0 ? (" (" + stateImported.dtLastSeen + ")") : "");
        }

        private void bwProgress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.pbProgress.Visible = false;
            this.bDoImportContacts = true;
            this.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_21", this.Name, "Редактирование базы Контактеров");
            button9.Text = ">";
            EnableAllButtons();


            if (e.Error != null)
                MessageBox.Show("Error: " + e.Error.Message, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (e.Cancelled)
            {
                MessageBox.Show((stateImported != null ? ("Импортировано " + stateImported.ContactsAdded.ToString() + " контактеров\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_23", this.Name, "Импорт данных Контактеров прерван. Если какие-то из Контактеров не отображаются в списке, смените настройки фильтра базы Контактеров по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show((stateImported != null ? ("Импортировано " + stateImported.ContactsAdded.ToString() + " контактеров\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_24", this.Name, "Импорт данных Контактеров завершен. Если какие-то из Контактеров не отображаются в списке, смените настройки фильтра базы Контактеров по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0) button9.Enabled = true; else button9.Enabled = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (button9.Text.Equals(">"))
            {
                if (grid1.RowsCount - 1 > 0)
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
                        String[] EQV = getItemName(iSelIdx).ToLower().Trim().Split(' ');
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
            else
            {
                this.button9.Enabled = false;
                if (!bDoImportContacts)
                    this.bwProgress.CancelAsync();
                bDoGroupOperation = true;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                button9_Click(null, null);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (bDoImportContacts)
            {
                String sUID = "";
                if (gridSelectedIndex > 0)
                {
                    int iSelIdx = gridSelectedIndex;
                    sUID = getItemID(iSelIdx);
                }
                if (sUID.Length <= 0)
                    return;

                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);
                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                iImportMode = 2;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                for (int i = 0; i < iContHarCount; i++)
                {
                    for (int j = 0; j < iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = sContHar[i, j];
                    fe.sPersHar[i, iContHarAttrCount] = "";
                }
                LoadFormEditPersHarValues(fe, iImportMode, 1);

                fe.iPersHarAttrCount = iContHarAttrCount;
                fe.iPersHarCount = iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValues(fe, iImportMode, 1);

                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < iContHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);

                    List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                    if (lstFilterHar != null)
                        StartImportContactsFromGroupThread("UserID=" + sUID, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, true, nudDuplicatesAction);
                }

            }
            else
            {
                this.button11.Enabled = false;
                this.bwProgress.CancelAsync();
            }
        }

        //---
        private void SaveFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            for (int i = 0; i < iContHarCount; i++)
                lstContHar.Add(fe.sPersHar[i, iContHarAttrCount]);

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"), lstContHar, Encoding.UTF8);
        }

        private void LoadFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"));
                lstContHar = new List<String>(srcFile);
            }

            if (lstContHar.Count > 0)
            {
                for (int i = 0; i < iContHarCount; i++)
                {
                    fe.sPersHar[i, iContHarAttrCount] = lstContHar[i];
                }
            }
            else
            {
                if (importmode== 100002)
                {
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        if (i < 14)
                            fe.sPersHar[i, iContHarAttrCount] = "";
                        else if (i == 14)
                            fe.sPersHar[i, iContHarAttrCount] = "~DUPLICATE";
                        else if (i == 15)
                            fe.sPersHar[i, iContHarAttrCount] = "NONE";
                    }
                }
            }
        }

        //---
        private List<String> GetHarValues(String sTitle, int importmode)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
            for (int i = 0; i < iContHarCount; i++)
            {
                for (int j = 0; j < iContHarAttrCount; j++)
                    fe.sPersHar[i, j] = sContHar[i, j];
                fe.sPersHar[i, iContHarAttrCount] = "";
            }
            LoadFormEditPersHarValues(fe, importmode, 0);

            fe.iPersHarAttrCount = iContHarAttrCount;
            fe.iPersHarCount = iContHarCount;
            fe.sFilePrefix = "cont";
            fe.Text = sTitle;
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                SaveFormEditPersHarValues(fe, importmode, 0);

                List<String> lstContHar = new List<String>();
                for (int i = 0; i < iContHarCount; i++)
                    lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                return lstContHar;
            }
            return null;
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

        private void button12_Click_1(object sender, EventArgs e)
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

        private void SelectFilterList(String[] RQV, int counterMax)
        {
            Cursor = Cursors.WaitCursor;

            Boolean bRQVEmpty = true;
            for (int iv = 0; iv < RQV.Length; iv++)
            {
                if (RQV[iv].Length > 0)
                {
                    bRQVEmpty = false;
                    break;
                }
            }

            //---
            bool bAcceptEmptyAge = false;
            int iAcceptAgeMin = -1;
            int iAcceptAgeMax = -1;
            int iAgeField = 1;

            if (iAgeField >= 0)
            {
                if (RQV[iAgeField].Length > 0)
                {
                    String sAgeFilter = RQV[iAgeField].Trim();
                    if (sAgeFilter[0] == '!')
                    {
                        bAcceptEmptyAge = true;
                        sAgeFilter = sAgeFilter.Substring(1).Trim();
                    }
                    if (sAgeFilter.Length > 0)
                    {
                        if (sAgeFilter.IndexOf("-") >= 0)
                        {
                            String sAgeMin = sAgeFilter.Substring(0, sAgeFilter.IndexOf("-")).Trim();
                            String sAgeMax = sAgeFilter.Substring(sAgeFilter.IndexOf("-") + 1).Trim();
                            iAcceptAgeMin = 0;
                            if (sAgeMin.Length > 0)
                                iAcceptAgeMin = Convert.ToInt32(sAgeMin);
                            iAcceptAgeMax = 1000;
                            if (sAgeMax.Length > 0)
                                iAcceptAgeMax = Convert.ToInt32(sAgeMax);
                        }
                        else
                        {
                            if (sUnknownAge.Equals(sAgeFilter))
                                bAcceptEmptyAge = true;
                            else
                            {
                                iAcceptAgeMin = Convert.ToInt32(sAgeFilter);
                                iAcceptAgeMax = iAcceptAgeMin;
                            }
                        }
                    }
                }
            }
            //---

            int counterSelected = 0;
            for (int i = 1; i < grid1.RowsCount; i++)
            {

                String sUID = getItemID(i);

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

                            EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1).ToLower();
                        }

                        bEquals = true;
                        for (int iv = 0; iv < RQV.Length; iv++)
                        {
                            if (RQV[iv].Length == 0/* || EQV[iv].Length == 0*/)
                                continue;

                            if (!(RQV[iv].ToLower().Equals(EQV[iv]) || EQV[iv].StartsWith(RQV[iv].ToLower())))
                            {
                                //---
                                if (iAgeField == iv)
                                {
                                    String sAge = EQV[iv];
                                    if (sUnknownAge.Equals(sAge))
                                    {
                                        bEquals = bAcceptEmptyAge;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            int iAge = Convert.ToInt32(sAge);
                                            if (iAge < iAcceptAgeMin || iAge > iAcceptAgeMax)
                                                bEquals = false;
                                        }
                                        catch
                                        {
                                            bEquals = false;
                                        }
                                    }

                                    if (!bEquals)
                                        break;
                                }
                                else
                                {
                                    //+---
                                    if (RQV[iv].IndexOf("~") >= 0 || RQV[iv].IndexOf("|") >= 0)
                                    {
                                        bEquals = false;
                                        String sFilter = RQV[iv].ToLower();
                                        bool bInverse = false;
                                        String sValue = EQV[iv].Trim();
                                        if (sFilter[0] == '~')
                                        {
                                            bInverse = true;
                                            bEquals = true;
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
                                                        bEquals = true;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (sPart.Equals(sValue))
                                                    {
                                                        bEquals = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    else if (RQV[iv].IndexOf("-") >= 0)
                                    {
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

                                            sval = EQV[iv].Trim();
                                            try
                                            {
                                                int ival = Convert.ToInt32(sval);
                                                if (ival < ivalmin || ival > ivalmax)
                                                    bEquals = false;
                                            }
                                            catch
                                            {
                                                bEquals = false;
                                            }
                                        }
                                        catch
                                        {
                                            bEquals = false;
                                        }
                                    }
                                    else
                                        bEquals = false;
                                    //---
                                    if (!bEquals)
                                        break;
                                    //+---
                                }
                            }
                        }
                    }
                    else
                        bEquals = false;
                }

                if (bEquals)
                {
                    counterSelected++;
                    grid1[i, 0].Value = true;

                    if (counterMax > 0 && counterSelected >= counterMax)
                        break;
                }
            }
            mNotLockItemChecked = true;
            grid_ItemChecked();

            Cursor = Cursors.Arrow;

        }

        private void button13_Click(object sender, EventArgs e)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
            for (int i = 0; i < iContHarCount; i++)
                for (int j = 0; j < iContHarAttrCount + 1; j++)
                    fe.sPersHar[i, j] = sContHar[i, j];

            fe.iPersHarAttrCount = iContHarAttrCount;
            fe.iPersHarCount = iContHarCount;
            fe.sFilePrefix = "cont";
            fe.Setup(false, true);

            if (fe.ShowDialog() == DialogResult.OK)
            {
                String[] RQV = new String[iContHarCount];
                for (int iii = 0; iii < iContHarCount; iii++)
                {
                    RQV[Convert.ToInt32(fe.sPersHar[iii, 0]) - 1] = fe.sPersHar[iii, iContHarAttrCount];
                    sContHar[iii, iContHarAttrCount] = fe.sPersHar[iii, iContHarAttrCount];
                }

                SelectFilterList(RQV, (int)fe.numericUpDown1.Value);
            }

        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (tbAllPersonenInRotation.Checked)
            {
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                for (int i = 0; i < iContHarCount; i++)
                {
                    for (int j = 0; j < iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = sContHar[i, j];
                    fe.sPersHar[i, iContHarAttrCount] = "";
                }
                LoadFormEditPersHarValues(fe, iImportMode, 1);

                fe.iPersHarAttrCount = iContHarAttrCount;
                fe.iPersHarCount = iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                List<String> lstContHar = new List<String>();
                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValues(fe, iImportMode, 1);
                    for (int i = 0; i < iContHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                }
                else
                    return;

                if (MessageBox.Show("Вы уверены, что хотите обновить значения характеристик всех подпадающих под фильтр видимости Контактеров у всех Персонажей в ротации?", "Обновление значений характеристик Контактеров", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    applyOperationForAllContactsToPersonenInRotation(3, lstContHar);
                }
                return;
            }

            if (bDoImportContacts)
            {
                if (gridCheckedCount > 0)
                {
                    String sImportIDs = "";
                    foreach (int k in gridCheckedIndexes)
                    {
                        String sUID = getItemID(k);
                        sImportIDs += sUID + "|";
                    }

                    iImportMode = 3;
                    //string value = "";
                    FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                    List<String> lstFilterHar = new List<String>();
                    fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        for (int j = 0; j < iContHarAttrCount; j++)
                            fe.sPersHar[i, j] = sContHar[i, j];
                        fe.sPersHar[i, iContHarAttrCount] = "";
                        lstFilterHar.Add(fe.sPersHar[i, 0] + "|");
                    }
                    LoadFormEditPersHarValues(fe, iImportMode, 1);

                    fe.iPersHarAttrCount = iContHarAttrCount;
                    fe.iPersHarCount = iContHarCount;
                    fe.sFilePrefix = "cont";
                    fe.Setup();

                    if (fe.ShowDialog() == DialogResult.OK)
                    {
                        SaveFormEditPersHarValues(fe, iImportMode, 1);
                        List<String> lstContHar = new List<String>();
                        for (int i = 0; i < iContHarCount; i++)
                            lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);

                        if (lstFilterHar != null)
                            StartImportContactsFromGroupThread("UIDs=" + sImportIDs, lstContHar, lstFilterHar, "", 100000, false);
                    }
                }

            }
            else
            {
                this.button14.Enabled = false;
                this.bwProgress.CancelAsync();
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
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditContactsDB.columns"), lstcolumns, Encoding.UTF8);
        }


        int[] columnOrder;
        int[] columnOrderReverce;
        private void LoadColumnsOrderAndWidth()
        {
            List<String> lstcolumns = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditContactsDB.columns")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditContactsDB.columns"));
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
                    columnInfo[iidx].MaximalWidth = this.Width - 100;

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
                columnOrder = new int[grid1.ColumnsCount - 1];
                for (int i = 0; i < columnOrder.Length; i++)
                {
                    columnOrder[i] = i + 1;
                    columnOrderReverce[i] = i;
                }
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
                foreach (ColumnOrderItem coi in lstcolumns)
                {
                    lstList.Add(coi.iOriginalIdx.ToString() + "|" + coi.iDisplayIdx.ToString() + "|" + coi.Width.ToString());
                }
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditContactsDB.columns"), lstList, Encoding.UTF8);

                Setup(slstErrorsInit, iPersUserID, iContUserID, false, false);
            }

        }

        private void FormEditContactsDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveColumnsOrderAndWidth();
            applyBaseFilter();
            //if (fwbVKontakte != null)
            //{
            //    if (fwbVKontakte.Visible)
            //    {
            //        fwbVKontakte.exitBrowser = true;
            //        fwbVKontakte.stopAllTimers();
            //        fwbVKontakte.Hide();
            //    }
            //}
        }

        private String SelectPersonen(string dialog_name = "Выбор Персонажей для импорта Контактеров")
        {
            //---
            FormEditPersonenDB fepdb = new FormEditPersonenDB(mFormMain);
            fepdb.sContHar = new String[mFormMain.iPersHarCount, mFormMain.iPersHarAttrCount + 1];
            for (int i = 0; i < mFormMain.iPersHarCount; i++)
            {
                for (int j = 0; j < mFormMain.iPersHarAttrCount; j++)
                    fepdb.sContHar[i, j] = mFormMain.sPersHar[i, j];
                fepdb.sContHar[i, mFormMain.iPersHarAttrCount] = "";
            }
            fepdb.iContHarCount = mFormMain.iPersHarCount;
            fepdb.iContHarAttrCount = mFormMain.iPersHarAttrCount;
            fepdb.Text = dialog_name;
            fepdb.button8.Visible = false;
            fepdb.button11.Visible = true;
            //fepdb.button1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_26", this.Name, "Отмена");
            fepdb.Setup(iPersUserID.ToString() + " (" + mFormMain.userLogin + ", " + mFormMain.userPassword + ")", iPersUserID);
            if (fepdb.ShowDialog() != DialogResult.OK)
                return "";

            String sPersonenCheckedList = "";
            if (fepdb.lvList.CheckedIndices.Count <= 0)
            {
                if (fepdb.lvList.SelectedIndices.Count > 0)
                {
                    int iSelIdx = fepdb.lvList.SelectedIndices[0];
                    sPersonenCheckedList = fepdb.lvList.Items[iSelIdx].SubItems[1].Text + "|";
                }
            }
            else
            {
                for (int i = 0; i < fepdb.lvList.Items.Count; i++)
                {
                    if (fepdb.lvList.Items[i].Checked)
                        sPersonenCheckedList += fepdb.lvList.Items[i].SubItems[1].Text + "|";
                }
            }
            //---
            return sPersonenCheckedList;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (bDoImportContacts)
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;
                String sImportIDs = "";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    List<String> lstImportIDs = new List<String>();
                    if (File.Exists(openFileDialog.FileName))
                    {
                        var srcFile = File.ReadAllLines(openFileDialog.FileName);
                        lstImportIDs = new List<String>(srcFile);
                        foreach (String str in lstImportIDs)
                        {
                            if (str == null)
                                continue;

                            if (str.Length == 0)
                                continue;

                            String value = ResolveID(str.Trim());
                            try
                            {
                                if (Convert.ToUInt64(value) > 0)
                                {
                                    if (FormMain.SocialNetwork == 0)
                                    {
                                        sImportIDs += value + "|";
                                    }
                                    else if (FormMain.SocialNetwork == 1)
                                    {
                                        String srec = NILSA_getUserRecord(value);
                                        if (srec != null)
                                        {
                                            sImportIDs += value + "|";
                                            String sUName1 = NILSA_GetFieldFromStringRec(srec, 1);

                                            ContactsList_AddUser(value, sUName1);
                                            ContactsList_AddUserToVisualList(value, sUName1, ContactsList_GetVisualListIdx(str));
                                        }
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }

                    if (FormMain.SocialNetwork == 0)
                    {
                        if (sImportIDs.Length > 0)
                        {

                            String sPersUIDs = SelectPersonen();
                            if (sPersUIDs.Length == 0)
                                return;

                            iImportMode = 4;
                            //string value = "";
                            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                            fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                            for (int i = 0; i < iContHarCount; i++)
                            {
                                for (int j = 0; j < iContHarAttrCount; j++)
                                    fe.sPersHar[i, j] = sContHar[i, j];
                                fe.sPersHar[i, iContHarAttrCount] = "";
                            }
                            LoadFormEditPersHarValues(fe, iImportMode, 1);

                            fe.iPersHarAttrCount = iContHarAttrCount;
                            fe.iPersHarCount = iContHarCount;
                            fe.sFilePrefix = "cont";
                            fe.Setup();

                            if (fe.ShowDialog() == DialogResult.OK)
                            {
                                SaveFormEditPersHarValues(fe, iImportMode, 1);
                                List<String> lstContHar = new List<String>();
                                for (int i = 0; i < iContHarCount; i++)
                                    lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                                List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                                if (lstFilterHar != null)
                                    StartImportContactsFromGroupThread("UIDs=" + sImportIDs, lstContHar, lstFilterHar, sPersUIDs, 100000, true);
                            }
                        }
                    }
                }
            }
            else
            {
                this.button15.Enabled = false;
                this.bwProgress.CancelAsync();
            }


        }

        private void setListCounter()
        {
            groupBox1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_27", this.Name, "Контактеры") + " (" + (gridCheckedCount > 0 ? gridCheckedCount.ToString() + " / " : "") + (grid1.RowsCount - 1).ToString() + ")";
        }

        private bool INIT_PERSONE_DIALOG = false;
        private void button16_Click(object sender, EventArgs e)
        {
            List<String> lstInitDialogContacts = new List<String>();
            //---
            if (gridCheckedCount <= 0)
            {
                if (gridSelectedIndex > 0)
                {
                    int iSelIdx = gridSelectedIndex;
                    String sUID = getItemID(iSelIdx);
                    String sUName = getItemName(iSelIdx);

                    if (sUID.Length > 0)
                    {
                        lstInitDialogContacts.Add(sUID + "|" + sUName);
                    }
                }
            }
            else
            {
                if (gridCheckedCount > 0)
                {
                    DisableAllButtons();

                    foreach (int k in gridCheckedIndexes)
                    {
                        String sUID = getItemID(k);
                        String sUName = getItemName(k);
                        lstInitDialogContacts.Add(sUID + "|" + sUName);
                    }

                    EnableAllButtons();
                }

            }

            if (lstInitDialogContacts.Count > 0)
            {
                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_initdialog_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstInitDialogContacts, Encoding.UTF8);

                //---
                int oldAlgorithmID = mFormMain.adbrCurrent.ID;
                FormInitContactDialog fe = new FormInitContactDialog(mFormMain);
                fe.sContHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                        fe.sContHar[i, j] = mFormMain.sContHar[i, j];
                    fe.sContHar[i, iContHarAttrCount] = "";
                }
                fe.iContHarCount = mFormMain.iContHarCount;
                fe.iContHarAttrCount = mFormMain.iContHarAttrCount;
                fe.Setup(iPersUserID.ToString() + " (" + mFormMain.userLogin + ", " + mFormMain.userPassword + ")", iPersUserID, INIT_PERSONE_DIALOG);
                INIT_PERSONE_DIALOG = false;

                try
                {
                    DialogResult dr = fe.ShowDialog();

                    mFormMain.applyAlgorithm(oldAlgorithmID);
                    if (dr == DialogResult.OK)
                    {
                        bInitDialogs = true;
                        timerClose.Enabled = true;
                    }
                }
                catch
                {
                    bInitDialogs = true;
                    timerClose.Enabled = true;
                }
            }
            INIT_PERSONE_DIALOG = false;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "CSV-files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<String> lstUsersToExport = new List<string>();

                String sColumnNames = "";
                for (int j = 0; j < columnOrder.Length; j++)
                {
                    sColumnNames += grid1[0, columnOrder[j]].Value + ";";
                }
                lstUsersToExport.Add(sColumnNames);

                foreach (int k in gridCheckedIndexes)
                {
                    String sColumnValues = "";
                    for (int j = 0; j < columnOrder.Length; j++)
                        sColumnValues += (grid1[k, columnOrder[j]].Value != null ? grid1[k, columnOrder[j]].Value.ToString() : "") + ";";

                    lstUsersToExport.Add(sColumnValues);
                }

                File.WriteAllLines(saveFileDialog.FileName, lstUsersToExport, Encoding.UTF8);
            }

        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (bDoImportContacts)
            {
                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);
                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                iImportMode = 6;

                FormSearchQuery fsq = new FormSearchQuery(mFormMain);
                fsq.Setup();
                if (fsq.ShowDialog() == DialogResult.OK)
                {
                    String value = fsq.stringResult;
                    FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                    fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        for (int j = 0; j < iContHarAttrCount; j++)
                            fe.sPersHar[i, j] = sContHar[i, j];
                        fe.sPersHar[i, iContHarAttrCount] = "";
                    }
                    LoadFormEditPersHarValues(fe, iImportMode, 1);

                    fe.iPersHarAttrCount = iContHarAttrCount;
                    fe.iPersHarCount = iContHarCount;
                    fe.sFilePrefix = "cont";
                    fe.Setup();

                    if (fe.ShowDialog() == DialogResult.OK)
                    {
                        SaveFormEditPersHarValues(fe, iImportMode, 1);
                        List<String> lstContHar = new List<String>();
                        for (int i = 0; i < iContHarCount; i++)
                            lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                        List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                        if (lstFilterHar != null)
                            StartImportContactsFromGroupThread("Search=" + value, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, true, nudDuplicatesAction);
                    }
                }
            }
            else
            {
                this.button19.Enabled = false;
                this.bwProgress.CancelAsync();
            }


        }

        private void lvList_DoubleClick(object sender, EventArgs e)
        {
            if (FormMain.SocialNetwork == 0)
            {
                if (gridSelectedIndex > 0)
                {
                    int iSelIdx = gridSelectedIndex;
                    String sUID = getItemID(iSelIdx);
                    if (mFormMain.FormWebBrowserEnabled)
                    {
                        FormWebBrowser fwb = new FormWebBrowser(mFormMain);
                        fwb.Setup(mFormMain.userLogin, mFormMain.userPassword, WebBrowserCommand.GoToContactPage, Convert.ToInt64(sUID));
                        fwb.ShowDialog();
                        //tbUpdateLists_Click(null, null);
                    }
                    else
                        System.Diagnostics.Process.Start("http://vk.com/id" + sUID);
                }

            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_28", this.Name, "Вы действительно хотите скопировать отмеченных контактеров другим Персонажам?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_29", this.Name, "Копирование Контактеров"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                String sPersUsersIDs = SelectPersonen();
                if (sPersUsersIDs.Length == 0)
                    return;

                //if (("|" + sPersUsersIDs).IndexOf("|" + iPersUserID.ToString() + "|") >= 0)
                //{
                //    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_30", this.Name, "Нельзя скопировать Контактеров активному Персонажу"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_29", this.Name, "Копирование Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                if (gridCheckedCount > 0)
                {
                    DisableAllButtons();

                    String curPersID = iPersUserID.ToString();
                    String[] pers = sPersUsersIDs.Split('|');
                    foreach (string scpuID in pers)
                    {
                        if (scpuID.Length == 0)
                            continue;

                        if (scpuID.Equals(curPersID))
                            continue;

                        foreach (int k in gridCheckedIndexes)
                        {
                            String sUID = getItemID(k);
                            String sUName = getItemName(k);

                            //String scpuID = sPersUsersIDs.Substring(0, sPersUsersIDs.IndexOf("|"));
                            //sPersUsersIDs = sPersUsersIDs.Substring(sPersUsersIDs.IndexOf("|") + 1);
                            //sPersUsersIDs = sPersUsersIDs + scpuID + "|";
                            String sAttrFileName = "cont_" + FormMain.getSocialNetworkPrefix() + scpuID + "_" + sUID + ".txt";

                            if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                            {
                                File.Copy(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), Path.Combine(FormMain.sDataPath, sAttrFileName), true);
                                ExternalContactsList_AddUser(scpuID, sUID, sUName);
                            }
                        }
                    }
                    EnableAllButtons();

                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_31", this.Name, "Копирование отмеченных Контактеров выбранным Персонажам завершено."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_29", this.Name, "Копирование контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_33", this.Name, "Вы действительно хотите переместить отмеченных Контактеров другим Персонажам?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_32", this.Name, "Перемещение Контактеров"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                String sPersUsersIDs = SelectPersonen();
                if (sPersUsersIDs.Length == 0)
                    return;

                //if (("|" + sPersUsersIDs).IndexOf("|" + iPersUserID.ToString() + "|") >= 0)
                //{
                //    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_34", this.Name, "Нельзя переместить Контактеров активному Персонажу"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_32", this.Name, "Перемещение Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                if (gridCheckedCount > 0)
                {
                    DisableAllButtons();

                    String curPersID = iPersUserID.ToString();
                    HashSet<String> hashsetNotMoved = new HashSet<string>();

                    foreach (int k in gridCheckedIndexes)
                    {
                        String sUID = getItemID(k);
                        String sUName = getItemName(k);

                        String scpuID = sPersUsersIDs.Substring(0, sPersUsersIDs.IndexOf("|"));
                        sPersUsersIDs = sPersUsersIDs.Substring(sPersUsersIDs.IndexOf("|") + 1);
                        sPersUsersIDs = sPersUsersIDs + scpuID + "|";

                        if (scpuID.Equals(curPersID))
                        {
                            hashsetNotMoved.Add(sUID);
                            continue;
                        }

                        String sAttrFileName = "cont_" + FormMain.getSocialNetworkPrefix() + scpuID + "_" + sUID + ".txt";

                        if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                        {
                            File.Copy(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), Path.Combine(FormMain.sDataPath, sAttrFileName), true);
                            ExternalContactsList_AddUser(scpuID, sUID, sUName);
                        }
                    }

                    foreach (int idx in gridCheckedIndexes)
                    {
                        String sUID = getItemID(idx);

                        if (hashsetNotMoved.Contains(sUID))
                            continue;

                        if (!(FormMain.SocialNetwork == 1 && iPersUserID == 0 && sUID.Equals("1")))
                            ContactsList_RemoveUser(sUID);
                    }

                    List<int> indexes = gridCheckedIndexes.OrderByDescending(i => i).ToList();
                    foreach (int idx in indexes)
                    {
                        String sUID = getItemID(idx);

                        if (hashsetNotMoved.Contains(sUID))
                            continue;

                        if (!(FormMain.SocialNetwork == 1 && iPersUserID == 0 && sUID.Equals("1")))
                            grid1.Rows.Remove(idx);
                    }
                    gridCheckedIndexes.Clear();
                    gridCheckedCount = 0;


                    EnableAllButtons();
                    FilterList(ClearFilter());

                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_35", this.Name, "Перемещение отмеченных Контактеров выбранным Персонажам завершено."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_32", this.Name, "Перемещение Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (gridCheckedCount > 0)
            {
                List<String> lstContHarVal = new List<String>();
                int iAlgValHar = 15;
                if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values"));
                    lstContHarVal = new List<String>(srcFile);
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        if (lstContHarVal[i].Trim().ToLower().Equals("#algorithm#"))
                        {
                            iAlgValHar = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        if (i == 15)
                            lstContHarVal.Add("#algorithm#");
                        else
                            lstContHarVal.Add("");
                    }

                    File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values"), lstContHarVal, Encoding.UTF8);
                }

                List<String> lstAlgorithmsList = new List<String>();
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"));
                    lstAlgorithmsList = new List<String>(srcFile);
                }
                List<AlgorithmsDBRecord> lstAlgorithmsRecordsList = new List<AlgorithmsDBRecord>();
                foreach (String value in lstAlgorithmsList)
                {
                    AlgorithmsDBRecord dbr = AlgorithmsDBRecord.FromRecordString(value);
                    lstAlgorithmsRecordsList.Add(dbr);
                }

                DisableAllButtons();

                foreach (int k in gridCheckedIndexes)
                {
                    String sUID = getItemID(k);
                    String sUName = getItemName(k);

                    List<String> lstContHarCurrent = new List<String>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                        lstContHarCurrent = new List<String>(srcFile);

                        int algid = getContacterAlgorithmID(Convert.ToInt64(sUID));
                        String algName = "ERROR";
                        if (algid >= 0)
                        {
                            foreach (AlgorithmsDBRecord dbr in lstAlgorithmsRecordsList)
                            {
                                if (dbr.ID == algid)
                                {
                                    algName = dbr.Name;
                                    break;
                                }
                            }
                        }
                        lstContHarCurrent[iAlgValHar] = lstContHarCurrent[iAlgValHar].Substring(0, lstContHarCurrent[iAlgValHar].IndexOf("|") + 1) + algName;
                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                        ContactsList_AddUserToVisualList(sUID, sUName, k);
                        ContactsList_AddUser(sUID, sUName);
                    }
                }

                EnableAllButtons();
            }

        }

        private int getContacterAlgorithmID(long contid)
        {
            int algid = -1;
            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + contid.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + contid.ToString() + ".txt"));
                lstTS = new List<String>(srcFile);
                algid = Convert.ToInt32(lstTS[0]);

                if (algid < 0)
                    algid = -1;
            }
            else
            {
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    lstTS = new List<String>(srcFile);
                    algid = Convert.ToInt32(lstTS[0]);

                    if (algid < 0)
                        algid = -1;
                }
            }

            return algid;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            FormSelectContacterAlgorithms fe = new FormSelectContacterAlgorithms(mFormMain);
            fe.Setup(iPersUserID);


            if (fe.ShowDialog() == DialogResult.OK)
            {
                AlgorithmsDBRecord SelectedAlgorithm = fe.SelectedAlgorithm;

                List<String> lstAlgorithmsDBRecordTS = new List<String>();
                lstAlgorithmsDBRecordTS.Add(SelectedAlgorithm.ID.ToString());

                List<String> lstContHarVal = new List<String>();
                int iAlgValHar = 15;
                if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values"));
                    lstContHarVal = new List<String>(srcFile);
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        if (lstContHarVal[i].Trim().ToLower().Equals("#algorithm#"))
                        {
                            iAlgValHar = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        if (i == 15)
                            lstContHarVal.Add("#algorithm#");
                        else
                            lstContHarVal.Add("");
                    }

                    File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values"), lstContHarVal, Encoding.UTF8);
                }

                if (gridCheckedCount <= 0)
                {
                    if (gridSelectedIndex > 0)
                    {
                        int iSelIdx = gridSelectedIndex;
                        String sUID = getItemID(iSelIdx);
                        String sUName = getItemName(iSelIdx);

                        if (sUID.Length > 0)
                        {
                            List<String> lstContHarCurrent = new List<String>();
                            if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                                lstContHarCurrent = new List<String>(srcFile);
                                lstContHarCurrent[iAlgValHar] = lstContHarCurrent[iAlgValHar].Substring(0, lstContHarCurrent[iAlgValHar].IndexOf("|") + 1) + SelectedAlgorithm.Name;

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstAlgorithmsDBRecordTS, Encoding.UTF8);

                                ContactsList_AddUserToVisualList(sUID, sUName, iSelIdx);
                                ContactsList_AddUser(sUID, sUName);
                            }
                        }
                    }
                }
                else
                {
                    if (gridCheckedCount > 0)
                    {
                        if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_37", this.Name, "Вы уверены, что хотите установить выбранный алгоритм для всех отмеченных Контактеров?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_36", this.Name, "Установка алгоритма Контактеров"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            DisableAllButtons();

                            foreach (int k in gridCheckedIndexes)
                            {
                                String sUID = getItemID(k);
                                String sUName = getItemName(k);

                                List<String> lstContHarCurrent = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                                    lstContHarCurrent = new List<String>(srcFile);
                                    lstContHarCurrent[iAlgValHar] = lstContHarCurrent[iAlgValHar].Substring(0, lstContHarCurrent[iAlgValHar].IndexOf("|") + 1) + SelectedAlgorithm.Name;

                                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);
                                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstAlgorithmsDBRecordTS, Encoding.UTF8);

                                    ContactsList_AddUserToVisualList(sUID, sUName, k);
                                    ContactsList_AddUser(sUID, sUName);
                                }
                            }

                            EnableAllButtons();
                        }
                    }

                }
            }
        }

        private void exportDialogsFromPersonen_Click(object sender, EventArgs e)
        {

            if (mFormMain.lstPersoneChangeOriginal.Count > 0 && FormMain.SocialNetwork == 0)
            {
                saveFileDialog.Filter = "HTML-files (*.html)|*.html|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 0;
                saveFileDialog.FileName = ("Отчет о диалогах Персонажей в ротации" + "_" + DateTime.Now.ToShortDateString()).Replace(" ", "_").Replace(".", "_") + ".html";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    List<String> lstHTMLFileText = new List<string>();

                    lstHTMLFileText.Add("<HTML>");
                    lstHTMLFileText.Add("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");

                    lstHTMLFileText.Add("<BODY>");
                    lstHTMLFileText.Add("<p align=right>" + DateTime.Now.ToShortDateString() + "<br>" + DateTime.Now.ToShortTimeString() + "</p>");
                    lstHTMLFileText.Add("<p align=right>" + FormMain.sLicenseUser + "</p>");

                    applyOperationForAllContactsToPersonenInRotation(10, lstHTMLFileText);

                    lstHTMLFileText.Add("</BODY>");
                    lstHTMLFileText.Add("</HTML>");
                    File.WriteAllLines(saveFileDialog.FileName, lstHTMLFileText, Encoding.UTF8);
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                }
            }
        }

        private void reportDialogsFromPersonen_Click(object sender, EventArgs e)
        {
            if (mFormMain.lstPersoneChangeOriginal.Count > 0 && FormMain.SocialNetwork == 0)
            {
                saveFileDialog.Filter = "CSV-files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 0;
                saveFileDialog.FileName = ("Отчет по диалогам Персонажей в ротации" + "_" + DateTime.Now.ToShortDateString()).Replace(" ", "_").Replace(".", "_") + ".csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    List<String> lstHTMLFileText = new List<string>();

                    // add header

                    //lstHTMLFileText.Add("<HTML>");
                    //lstHTMLFileText.Add("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");

                    //lstHTMLFileText.Add("<BODY>");
                    //lstHTMLFileText.Add("<p align=right>" + DateTime.Now.ToShortDateString() + "<br>" + DateTime.Now.ToShortTimeString() + "</p>");
                    //lstHTMLFileText.Add("<p align=right>" + FormMain.sLicenseUser + "</p>");
                    lstHTMLFileText.Add("Имя персонажа;Id персонажа;Имя контактёра;Id контактёра;Проект;Роль в общении;Отношение персонажа к контактёру;Отношение контактёра к персонажу;Алгоритм;Стадия общения;Диалог");

                    applyOperationForAllContactsToPersonenInRotation(11, lstHTMLFileText);

                    // add footer
                    //lstHTMLFileText.Add("</BODY>");
                    //lstHTMLFileText.Add("</HTML>");

                    File.WriteAllLines(saveFileDialog.FileName, lstHTMLFileText, Encoding.UTF8);
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                }
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "HTML-files (*.html)|*.html|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.FileName = (mFormMain.userName + "_" + DateTime.Now.ToShortDateString()).Replace(" ", "_").Replace(".", "_") + ".html";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                mFormMain.ShowFormWait(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_38", this.Name, "Пожалуйста, подождите, идет формирование отчета..."));
                List<String> lstHTMLFileText = new List<string>();

                lstHTMLFileText.Add("<HTML>");
                lstHTMLFileText.Add("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");

                lstHTMLFileText.Add("<BODY>");
                lstHTMLFileText.Add("<p align=right>" + DateTime.Now.ToShortDateString() + "<br>" + DateTime.Now.ToShortTimeString() + "</p>");
                lstHTMLFileText.Add("<p align=right>" + FormMain.sLicenseUser + "</p>");

                // Персонаж
                lstHTMLFileText.Add("<H1>" + mFormMain.userName + "</H1>");
                lstHTMLFileText.Add("<UL>");
                for (int i = 0; i < mFormMain.iPersHarCount; i++)
                {
                    String sHarName = mFormMain.sPersHar[i, 1];
                    String sHarValue = mFormMain.GetPersoneParametersValue(mFormMain.sPersHar[i, 0]);
                    lstHTMLFileText.Add("<LI><B>" + sHarName + ":<B> " + sHarValue + "</LI>");
                }
                lstHTMLFileText.Add("</UL>");

                foreach (int k in gridCheckedIndexes)
                {
                    // Контактер
                    lstHTMLFileText.Add("<H2>" + getItemName(k) + "</H2>");
                    lstHTMLFileText.Add("<UL>");
                    for (int j = 0; j < columnOrder.Length; ++j)
                    {
                        String sColumnName = grid1[0, columnOrder[j]].Value.ToString();
                        String sColumnValue = grid1[k, columnOrder[j]].Value != null ? grid1[k, columnOrder[j]].Value.ToString() : "";
                        lstHTMLFileText.Add("<LI><B>" + sColumnName + ":<B> " + sColumnValue + "</LI>");

                    }
                    lstHTMLFileText.Add("</UL>");
                    List<String> lstMsgsLst = ReadAllUserMessagesContacter(Convert.ToInt64(getItemID(k)), mFormMain.iPersUserID, true);
                    if (lstMsgsLst.Count > 0)
                    {
                        lstHTMLFileText.Add("<P>");
                        foreach (string str in lstMsgsLst)
                            lstHTMLFileText.Add(str);

                        lstHTMLFileText.Add("</P>");
                    }

                }

                lstHTMLFileText.Add("</BODY>");
                lstHTMLFileText.Add("</HTML>");
                File.WriteAllLines(saveFileDialog.FileName, lstHTMLFileText, Encoding.UTF8);
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
                mFormMain.HideFormWait();
                Cursor = Cursors.Arrow;
            }


        }

        private List<String> ReadAllUserMessagesContacter(long lUserID, long personeID, bool htmlformat)
        {
            List<String> lstUserMessages = new List<String>();

            // получаем id пользователей из группы, макс. кол-во записей = 1000
            int totalCount; // общее кол-во участников
            if (lUserID >= 0)
            {
                if (FormMain.SocialNetwork == 0)
                {
                    List<String> lstHistory = new List<string>();

                    if (File.Exists(Path.Combine(FormMain.sDataPath, "chat_" + FormMain.getSocialNetworkPrefix() + personeID.ToString() + "_" + Convert.ToString(lUserID) + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "chat_" + FormMain.getSocialNetworkPrefix() + personeID.ToString() + "_" + Convert.ToString(lUserID) + ".txt"));
                            lstHistory = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        }
                    }

                    foreach (String str in lstHistory)
                    {
                        if (str == null)
                            continue;

                        if (str.Length == 0)
                            continue;

                        String value = str;
                        String inboundStr = value.Substring(0, value.IndexOf("|"));
                        value = value.Substring(value.IndexOf("|") + 1);
                        String dateStr = value.Substring(0, value.IndexOf("|"));
                        value = value.Substring(value.IndexOf("|") + 1);
                        String timeStr = value.Substring(0, value.IndexOf("|"));
                        value = value.Substring(value.IndexOf("|") + 1);
                        String bodyStr = value;
                        bool inboundMessage = inboundStr.Equals("0");

                        value = "0|";
                        value = value + dateStr + "|";
                        value = value + timeStr + "|";
                        value = value + NilsaUtils.TextToString(bodyStr);

                        lstUserMessages.Add((htmlformat ? "<B>" : "") + (inboundMessage ? (htmlformat ? "&lt;&lt; " : "<- ") : (htmlformat ? "&gt;&gt; " : "-> ")) + (htmlformat ? " </B><U>" : "") + dateStr + " " + timeStr + (htmlformat ? "</U>" : "") + " - " + bodyStr + (htmlformat ? "<BR>" : ""));
                    }
                    /*
                    do
                    {
                        try
                        {
                            var msgsReceived = FormMain.api.Messages.GetHistory(lUserID, false, out totalCount, null, 200);
                            foreach (VkNet.Model.Message msg in msgsReceived)
                            {
                                String value = "";
                                value = Convert.ToString(msg.Id.HasValue ? msg.Id.Value : 0) + "|";
                                value = value + msg.Date.Value.ToShortDateString() + "|";
                                value = value + msg.Date.Value.ToShortTimeString() + "|";
                                value = value + NilsaUtils.TextToString(msg.Body);
                                lstUserMessages.Insert(0, (htmlformat ? "<B>" : "") + ((msg.Type.HasValue && msg.Type.Value == VkNet.Enums.MessageType.Received) ? (htmlformat ? "&lt;&lt; " : "<- ") : (htmlformat ? "&gt;&gt; " : "-> ")) + (htmlformat ? "</B><U>" : "") + msg.Date.Value.ToShortDateString() + " " + msg.Date.Value.ToShortTimeString() + (htmlformat ? "</U>" : "") + " - " + msg.Body + (htmlformat ? "<BR>" : ""));
                            }
                            break;
                        }
                        catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                        {
                            if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                                break;
                        }
                        catch (System.Net.WebException)
                        {
                            if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                                break;
                        }
                        catch (VkNet.Exception.VkApiException)
                        {
                            if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                                break;
                        }

                        catch (Exception e)
                        {
                            ExceptionToLogList("ReadAllUserMessagesContacter", lUserID.ToString(), e);
                            break;
                        }
                        finally { }
                    }
                    while (true);
                    */
                }
                else if (FormMain.SocialNetwork == 1)
                {
                    mFormMain.NILSA_LoadMessagesDB();
                    String sTo0 = iPersUserID.ToString() + "|0|" + lUserID.ToString() + "|";
                    String sTo1 = lUserID.ToString() + "|0|" + iPersUserID.ToString() + "|";
                    String sTo2 = iPersUserID.ToString() + "|1|" + lUserID.ToString() + "|";
                    String sTo3 = lUserID.ToString() + "|1|" + iPersUserID.ToString() + "|";

                    List<String> lstMsgToAdd = new List<string>();
                    for (int i = 0; i < mFormMain.lstNILSA_MessagesDB.Count; i++)
                    {
                        String srec = mFormMain.lstNILSA_MessagesDB[i];
                        if (srec.StartsWith(sTo0) || srec.StartsWith(sTo1) || srec.StartsWith(sTo2) || srec.StartsWith(sTo3))
                        {
                            long date = Convert.ToInt64(NILSA_GetFieldFromStringRec(srec, 3));
                            lstMsgToAdd.Add(date.ToString("yyyy.MM.dd.HH:mm:ss") + "|" + srec);
                        }

                    }

                    if (lstMsgToAdd.Count > 0)
                    {
                        lstMsgToAdd = lstMsgToAdd.OrderByDescending(i => i).ToList();

                        foreach (String EQ in lstMsgToAdd)
                        {
                            String srec = EQ.Substring(EQ.IndexOf("|") + 1);

                            String value = "100|";
                            long date = Convert.ToInt64(NILSA_GetFieldFromStringRec(srec, 3));
                            value = value + new DateTime(date).ToShortDateString() + "|";
                            value = value + new DateTime(date).ToShortTimeString() + "|";
                            value = value + NilsaUtils.TextToString(NILSA_GetFieldFromStringRec(srec, 4));

                            lstUserMessages.Insert(0, (htmlformat ? "<B>" : "") + ((srec.StartsWith(sTo0) || srec.StartsWith(sTo2)) ? (htmlformat ? "&lt;&lt; " : "<- ") : (htmlformat ? "&gt;&gt; " : "-> ")) + (htmlformat ? "</B><U>" : "") + new DateTime(date).ToShortDateString() + " " + new DateTime(date).ToShortTimeString() + (htmlformat ? "</U>" : "") + " - " + NILSA_GetFieldFromStringRec(srec, 4) + (htmlformat ? "<BR>" : ""));
                        }
                    }
                }
            }
            return lstUserMessages;
        }

        private void button25_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<String> lstUsersToExport = new List<string>();
                foreach (int k in gridCheckedIndexes)
                {
                    lstUsersToExport.Add(getItemID(k));
                }

                File.WriteAllLines(saveFileDialog.FileName, lstUsersToExport, Encoding.UTF8);
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "CSV-files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<String> lstImportIDs = new List<String>();
                if (File.Exists(openFileDialog.FileName))
                {
                    var srcFile = File.ReadAllLines(openFileDialog.FileName);
                    lstImportIDs = new List<String>(srcFile);
                    if (lstImportIDs.Count > 1)
                    {
                        String sHeader = lstImportIDs[0];
                        int iFieldName = -1, iFieldID = -1;
                        String[] IW = sHeader.Split(';');
                        for (int i = 0; i < IW.Length; i++)
                            if (IW[i].ToLower().Equals("имя"))
                            {
                                iFieldName = i;
                                break;
                            }
                        for (int i = 0; i < IW.Length; i++)
                            if (IW[i].ToLower().Equals("id"))
                            {
                                iFieldID = i;
                                break;
                            }

                        if (iFieldID >= 0 && iFieldName >= 0)
                        {
                            int[] iFieldHar = new int[iContHarCount];

                            for (int i = 0; i < IW.Length; i++)

                                if (IW[i].ToLower().Equals("id"))
                                {
                                    iFieldID = i;
                                    break;
                                }
                            for (int i = 0; i < iContHarCount; i++)
                            {
                                iFieldHar[i] = -1;
                                for (int j = 0; j < IW.Length; j++)
                                {
                                    if (sContHar[i, 1].ToLower().Equals(IW[j].ToLower()))
                                    {
                                        iFieldHar[i] = j;
                                        break;
                                    }
                                }
                            }

                            for (int iRec = 1; iRec < lstImportIDs.Count; iRec++)
                            {
                                String sRecord = lstImportIDs[iRec];
                                String[] IWRecord = sRecord.Split(';');
                                if (IWRecord.Length != IW.Length)
                                    continue;

                                String sUID = IWRecord[iFieldID];
                                String sUName = IWRecord[iFieldName];
                                String sAttrFileName = "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt";

                                List<String> lstContHarValuesTemp = new List<String>();
                                for (int i = 0; i < iContHarCount; i++)
                                {
                                    lstContHarValuesTemp.Add(sContHar[i, 0] + "|" + (iFieldHar[i] >= 0 ? IWRecord[iFieldHar[i]] : ""));
                                }

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName), lstContHarValuesTemp, Encoding.UTF8);
                                ContactsList_AddUser(sUID, sUName);
                                ContactsList_AddUserToVisualList(sUID, sUName, ContactsList_GetVisualListIdx(sUID));

                            }

                        }
                    }
                }//if (File.Exists(openFileDialog.FileName))
            }
        }

        private void toolStripButtonSearchDuplicate_Click(object sender, EventArgs e)
        {
            if (gridCheckedCount > 0)
            {
                bool bAcceptToAll = false;
                bool bAcceptExisted = false;
                bool bAcceptBreak = false;

                foreach (int k in gridCheckedIndexes)
                {
                    if (bAcceptBreak)
                        break;

                    String sUID_ToSearch = getItemID(k);

                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_personen.txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_personen.txt"));
                        List<String> lstList = new List<String>(srcFile);

                        foreach (String str in lstList)
                            if (str.Length > 0)
                            {
                                if (bAcceptBreak)
                                    break;

                                String sUID_Persone = str.Substring(0, str.IndexOf("|"));
                                long lUID = Convert.ToInt64(sUID_Persone);

                                if (lUID == iPersUserID)
                                    continue;

                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_contacts_" + sUID_Persone + ".txt")))
                                {
                                    var srcFileCont = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + sUID_Persone + ".txt"));
                                    List<String> lstListCont = new List<String>(srcFileCont);

                                    foreach (String strcont in lstListCont)
                                        if (strcont.Length > 0)
                                        {
                                            String sUID_Contact = strcont.Substring(0, strcont.IndexOf("|"));

                                            if (sUID_Contact.Equals(sUID_ToSearch))
                                            {
                                                if (!bAcceptToAll)
                                                {
                                                    String sUName_Contact = strcont.Substring(strcont.IndexOf("|") + 1);
                                                    bAcceptExisted = false;
                                                    FormPersoneExists fpe = new FormPersoneExists();
                                                    fpe.Setup(mFormMain, sUID_Persone, sUID_Contact, sUName_Contact, 1);
                                                    System.Windows.Forms.DialogResult dr = fpe.ShowDialog();
                                                    if (dr == System.Windows.Forms.DialogResult.OK)
                                                        bAcceptExisted = true;
                                                    else if (dr == System.Windows.Forms.DialogResult.Cancel)
                                                        bAcceptExisted = false;
                                                    else if (dr == System.Windows.Forms.DialogResult.Abort)
                                                    {
                                                        bAcceptExisted = false;
                                                        bAcceptBreak = true;
                                                        break;
                                                    }

                                                    bAcceptToAll = fpe.checkBoxToAll.Checked;
                                                }

                                                if (bAcceptExisted)
                                                {
                                                    string sAttrFileName = "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID_Contact + ".txt";
                                                    if (File.Exists(Path.Combine(FormMain.sDataPath, sAttrFileName)))
                                                    {
                                                        var srcFileHar = File.ReadAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName));
                                                        List<String> lstContHarHar = new List<String>(srcFileHar);
                                                        lstContHarHar[14] = "15|DUPLICATE";

                                                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName), lstContHarHar, Encoding.UTF8);
                                                        ContactsList_AddUserToVisualList(sUID_Contact, getItemName(k), k);
                                                    }

                                                }
                                            }
                                        }
                                }
                            }
                    }
                }
                MessageBox.Show("Поиск дубликатов отмеченных Контактеров " + (bAcceptBreak ? "прерван." : "завершен."), "Поиск дубликатов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripButtonCopyToClipboard_Click(object sender, EventArgs e)
        {
            if (gridSelectedIndex > 0)
            {
                int iSelIdx = gridSelectedIndex;
                String sValue = "";

                for (int i = 1; i < grid1.ColumnsCount; i++)
                    sValue += (grid1[iSelIdx, columnOrder[i - 1]].Value != null ? grid1[iSelIdx, columnOrder[i - 1]].Value.ToString() : "") + "\n";
                System.Windows.Forms.Clipboard.SetText(sValue);
            }
        }

        private void buttonImportPersonenAsContacter_Click(object sender, EventArgs e)
        {
            if (bDoImportContacts)
            {
                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);
                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                String sImportsUIDs = SelectPersonen("Выберите импортируемых в качестве Контактеров Персонажей");
                if (sImportsUIDs.Length == 0)
                    return;

                iImportMode = 7;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                for (int i = 0; i < iContHarCount; i++)
                {
                    for (int j = 0; j < iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = sContHar[i, j];
                    fe.sPersHar[i, iContHarAttrCount] = "";
                }
                LoadFormEditPersHarValues(fe, iImportMode, 1);

                fe.iPersHarAttrCount = iContHarAttrCount;
                fe.iPersHarCount = iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValues(fe, iImportMode, 1);
                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < iContHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                    List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);

                    if (lstFilterHar != null)
                        StartImportContactsFromGroupThread("UIDs=" + sImportsUIDs, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, false, nudDuplicatesAction);
                }

            }
            else
            {
                this.buttonImportPersonenAsContacter.Enabled = false;
                this.bwProgress.CancelAsync();
            }

        }

        private void buttonSelectContacters_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void buttonExportContacterServer_Click(object sender, EventArgs e)
        {
            string value = "cont" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_");
            string filename = Path.Combine(Application.StartupPath, value + FormMain.FTP_SERVER_CONT_NAME_POSTFIX);

            List<String> lstUsersToExport = new List<string>();

            String sColumnNames = "";
            for (int j = 0; j < 2; j++)
            {
                sColumnNames += grid1[0, columnOrder[j]].Value + ";";
                //lstcolumns.Add(column.Index.ToString() + "|" + column.DisplayIndex.ToString() + "|" + column.Width.ToString());
            }
            for (int j = 0; j < iContHarCount; ++j)
            {
                sColumnNames += sContHar[j, 1] + ";";
            }
            lstUsersToExport.Add(sColumnNames);

            foreach (int k in gridCheckedIndexes)
            {
                String sColumnValues = "";
                for (int j = 0; j < 2; ++j)
                    sColumnValues += (grid1[k, columnOrder[j]].Value != null ? grid1[k, columnOrder[j]].Value.ToString() : "") + ";";

                for (int j = 0; j < mFormMain.iContHarCount; ++j)
                    sColumnValues += (grid1[k, columnOrder[j + 2]].Value != null ? grid1[k, columnOrder[j + 2]].Value.ToString() : "") + ";";

                lstUsersToExport.Add(sColumnValues);
            }

            File.WriteAllLines(filename, lstUsersToExport, Encoding.UTF8);

            int bSuccess = mFormMain.tsmiExportMessagesCSVFTP(4, value);

            if (File.Exists(filename))
                File.Delete(filename);

            if (bSuccess == 0)
            {

                if (bSuccess != 2)
                {
                    if (bSuccess == 1)
                        MessageBox.Show("Во время экспорта Контактеров на Сервер произошла ошибка. Проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", "Экспорт Контактеров на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Экспорт Контактеров на Сервер успешно выполнен", "Экспорт Контактеров на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }

        private void buttonImportContacterServer_Click(object sender, EventArgs e)
        {
            String filename = mFormMain.tsmiImportMessagesCSVFTP(4);

            if (filename.Length == 0)
                return;

            string openFileDialog_FileName = Path.Combine(Application.StartupPath, filename);

            List<String> lstImportIDs = new List<String>();
            if (File.Exists(openFileDialog_FileName))
            {
                var srcFile = File.ReadAllLines(openFileDialog_FileName);
                lstImportIDs = new List<String>(srcFile);
                if (lstImportIDs.Count > 1)
                {
                    String sHeader = lstImportIDs[0];
                    int iFieldName = -1, iFieldID = -1;
                    String[] IW = sHeader.Split(';');
                    for (int i = 0; i < IW.Length; i++)
                        if (IW[i].ToLower().Equals("имя"))
                        {
                            iFieldName = i;
                            break;
                        }
                    for (int i = 0; i < IW.Length; i++)
                        if (IW[i].ToLower().Equals("id"))
                        {
                            iFieldID = i;
                            break;
                        }

                    if (iFieldID >= 0 && iFieldName >= 0)
                    {
                        int[] iFieldHar = new int[iContHarCount];

                        for (int i = 0; i < IW.Length; i++)

                            if (IW[i].ToLower().Equals("id"))
                            {
                                iFieldID = i;
                                break;
                            }
                        for (int i = 0; i < iContHarCount; i++)
                        {
                            iFieldHar[i] = -1;
                            for (int j = 0; j < IW.Length; j++)
                            {
                                if (sContHar[i, 1].ToLower().Equals(IW[j].ToLower()))
                                {
                                    iFieldHar[i] = j;
                                    break;
                                }
                            }
                        }

                        for (int iRec = 1; iRec < lstImportIDs.Count; iRec++)
                        {
                            String sRecord = lstImportIDs[iRec];
                            String[] IWRecord = sRecord.Split(';');
                            if (IWRecord.Length != IW.Length)
                                continue;

                            String sUID = IWRecord[iFieldID];
                            String sUName = IWRecord[iFieldName];
                            String sAttrFileName = "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt";

                            List<String> lstContHarValuesTemp = new List<String>();
                            for (int i = 0; i < iContHarCount; i++)
                            {
                                lstContHarValuesTemp.Add(sContHar[i, 0] + "|" + (iFieldHar[i] >= 0 ? IWRecord[iFieldHar[i]] : ""));
                            }

                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName), lstContHarValuesTemp, Encoding.UTF8);
                            ContactsList_AddUser(sUID, sUName);
                            ContactsList_AddUserToVisualList(sUID, sUName, ContactsList_GetVisualListIdx(sUID));

                        }

                    }
                }
            }//if (File.Exists(openFileDialog.FileName))

        }

        private void timerClose_Tick(object sender, EventArgs e)
        {
            timerClose.Enabled = false;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void tbAllPersonenInRotation_CheckedChanged(object sender, EventArgs e)
        {
            button14.Enabled = tbAllPersonenInRotation.Checked;
            button17.Enabled = tbAllPersonenInRotation.Checked;
            button2.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0 || tbAllPersonenInRotation.Checked;
            button5.Enabled = gridSelectedIndex > 0 || gridCheckedCount > 0 || tbAllPersonenInRotation.Checked;
        }

        String[] copyedParameters = null;

        private void buttonCopyParameters_Click(object sender, EventArgs e)
        {
            if (gridSelectedIndex > 0)
            {
                int iSelIdx = gridSelectedIndex;
                String sUID = getItemID(iSelIdx);

                if (sUID.Length > 0)
                {
                    LoadContactParamersValues(sUID);
                    copyedParameters = new string[iContHarCount];

                    for (int i = 0; i < iContHarCount; i++)
                    {
                        copyedParameters[i] = GetContactParametersValue(sContHar[i, 0]);
                    }
                }
            }
            buttonPasteParameters.Enabled = copyedParameters != null && (gridSelectedIndex > 0 || gridCheckedCount > 0);

        }

        private void buttonPasteParameters_Click(object sender, EventArgs e)
        {
            if (copyedParameters == null)
                return;

            if (gridCheckedCount <= 0)
            {
                if (gridSelectedIndex > 0)
                {
                    if (MessageBox.Show("Вы уверены, что хотите вставить скопированные характеристики для текущего Контактёра?", "Вставка характеристик Контактёров", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        int iSelIdx = gridSelectedIndex;
                        String sUID = getItemID(iSelIdx);

                        if (sUID.Length > 0)
                        {
                            LoadContactParamersValues(sUID);
                            List<String> lstContHar = new List<String>();

                            for (int i = 0; i < iContHarCount; i++)
                            {
                                if (i == 4 || i == 15 || (i >= 6 && i <= 13))
                                    lstContHar.Add(sContHar[i, 0] + "|" + copyedParameters[i]);
                                else
                                    lstContHar.Add(sContHar[i, 0] + "|" + GetContactParametersValue(sContHar[i, 0]));
                            }

                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHar, Encoding.UTF8);
                            ContactsList_AddUserToVisualList(sUID, getItemName(iSelIdx), iSelIdx);
                        }
                    }
                }
            }
            else
            {
                if (gridCheckedCount > 0)
                {

                    if (MessageBox.Show("Вы уверены, что хотите вставить скопированные характеристики для всех отмеченных Контактёров?", "Вставка характеристик Контактёров", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        foreach (int k in gridCheckedIndexes)
                        {
                            String sUID = getItemID(k);
                            String sUName = getItemName(k);

                            List<String> lstContHarCurrent = new List<String>();
                            if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                                lstContHarCurrent = new List<String>(srcFile);
                                for (int i = 0; i < iContHarCount; i++)
                                { 
                                    if (i == 4 || i == 15 || (i >= 6 && i <= 13))
                                        lstContHarCurrent[i] = sContHar[i, 0] + "|" + copyedParameters[i];
                                }

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                            ContactsList_AddUserToVisualList(sUID, sUName, k);
                            ContactsList_AddUser(sUID, sUName);
                            }
                        }

                        EnableAllButtons();
                    }
                }

            }
            
        }

        private void applyBaseFilter()
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
            for (int i = 0; i < iContHarCount; i++)
                for (int j = 0; j < iContHarAttrCount + 1; j++)
                    fe.sPersHar[i, j] = sContHar[i, j];

            LoadFormEditPersHarValues(fe, 100002, 1);
            SaveFormEditPersHarValues(fe, 100001, 1);
            NilsaUtils.SaveLongValue(8, 1);
        }

        private void buttonBaseFilter_Click(object sender, EventArgs e)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
            for (int i = 0; i < iContHarCount; i++)
                for (int j = 0; j < iContHarAttrCount + 1; j++)
                    fe.sPersHar[i, j] = sContHar[i, j];

            LoadFormEditPersHarValues(fe, 100002, 1);

            fe.iPersHarAttrCount = iContHarAttrCount;
            fe.iPersHarCount = iContHarCount;
            fe.sFilePrefix = "cont";
            fe.Setup(false, true);
            if (fe.ShowDialog() == DialogResult.OK)
            {
                SaveFormEditPersHarValues(fe, 100002, 1);
                SaveFormEditPersHarValues(fe, 100001, 1);
                String[] RQV = new String[iContHarCount];
                for (int iii = 0; iii < iContHarCount; iii++)
                {
                    RQV[Convert.ToInt32(fe.sPersHar[iii, 0]) - 1] = fe.sPersHar[iii, iContHarAttrCount];
                    sContHar[iii, iContHarAttrCount] = fe.sPersHar[iii, iContHarAttrCount];
                }

                FilterList(RQV, (int)fe.numericUpDown1.Value);
                NilsaUtils.SaveLongValue(8, 1);
            }

            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (tbAllPersonenInRotation.Checked)
            {
                List<String> lstContHarVal = new List<String>();
                if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesStatusUpdate.values")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesStatusUpdate.values"));
                    lstContHarVal = new List<String>(srcFile);
                }
                else
                {
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        if (i == 14)
                            lstContHarVal.Add("#online#");
                        else
                            lstContHarVal.Add("");
                    }

                    File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesStatusUpdate.values"), lstContHarVal, Encoding.UTF8);
                }

                List<String> lstContHar = new List<String>();
                for (int i = 0; i < iContHarCount; i++)
                {
                    lstContHar.Add(sContHar[i, 0] + "|" + lstContHarVal[i]);
                }


                if (MessageBox.Show("Вы уверены, что хотите обновить статус всех подпадающих под фильтр видимости Контактеров у всех Персонажей в ротации?", "Обновление статуса Контактеров", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    applyOperationForAllContactsToPersonenInRotation(2, lstContHar);
                }
                return;
            }
            if (bDoImportContacts)
            {
                if (gridCheckedCount > 0)
                {
                    String sImportIDs = "";
                    foreach (int k in gridCheckedIndexes)
                    {
                        String sUID = getItemID(k);
                        sImportIDs += sUID + "|";
                    }

                    iImportMode = 5;

                    List<String> lstContHarVal = new List<String>();
                    if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesStatusUpdate.values")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesStatusUpdate.values"));
                        lstContHarVal = new List<String>(srcFile);
                    }
                    else
                    {
                        for (int i = 0; i < iContHarCount; i++)
                        {
                            if (i == 14)
                                lstContHarVal.Add("#online#");
                            else
                                lstContHarVal.Add("");
                        }

                        File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesStatusUpdate.values"), lstContHarVal, Encoding.UTF8);
                    }

                    List<String> lstContHar = new List<String>();
                    List<String> lstFilterHar = new List<String>();
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        lstContHar.Add(sContHar[i, 0] + "|" + lstContHarVal[i]);
                        lstFilterHar.Add(sContHar[i, 0] + "|");
                    }

                    if (lstFilterHar != null)
                        StartImportContactsFromGroupThread("UIDs=" + sImportIDs, lstContHar, lstFilterHar, "", 100000, false);
                }

            }
            else
            {
                this.button17.Enabled = false;
                this.bwProgress.CancelAsync();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


    }
}
