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

namespace Nilsa
{
    public partial class FormEditGroupsDB : Form
    {
        FormMain mFormMain;

        public String[,] sGroupHar;
        public int iGroupHarCount = 16;
        public int iGroupHarAttrCount = 4;

        List<String> lstErrorsLogList;
        String slstErrorsInit;
        public long iPersUserID;
        List<String> lstGroupHarValues;
        List<String> lstGroupsList;

        bool bDoImportGroups;

        int iImportMode;
        public bool bInitDialogs = false;
        public Boolean bNeedPersoneChange = false;
        public long iSelGroupID = -1;
        private string sAdditionalPersonen = "";

        public FormEditGroupsDB(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            //this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
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

        public void Setup(String sErrorHdr, long _iPersUserID, string _sAdditionalPersonen, Boolean initDialog = false)
        {
            sAdditionalPersonen = _sAdditionalPersonen;
            this.button7.Enabled = FormMain.SocialNetwork == 0;
            buttonImportContacterGroup.Enabled = FormMain.SocialNetwork == 0;
            this.button19.Enabled = FormMain.SocialNetwork == 0;

            bNeedPersoneChange = false;
            iSelGroupID = -1;

            pictureBoxPersone.Image = mFormMain.personPicture;
            toolTip1.SetToolTip(pictureBoxPersone, mFormMain.userNameName + " " + mFormMain.userNameFamily);

            bDoImportGroups = true;
            pbProgress.Visible = false;
            slstErrorsInit = sErrorHdr;
            iPersUserID = _iPersUserID;

            LoadErrorsLogList();
            GroupsList_Load();

            button26.Enabled = FormMain.SocialNetwork == 0;
            button2.Enabled = false;
            button3.Enabled = false;
            buttonSelectGroup.Enabled = false;
            button5.Enabled = false;
            button12.Enabled = false;
            button16.Enabled = false;

            Cursor = Cursors.WaitCursor;
            lvList.CheckBoxes = true;
            lvList.SelectedIndices.Clear();
            lvList.BeginUpdate();
            lvList.Items.Clear();

            if (iGroupHarCount <= 16)
            {
                for (int i = 0; i < iGroupHarCount; i++)
                    lvList.Columns[i + 2].Text = sGroupHar[i, 1];
            }

            for (int i = 0; i < lstGroupsList.Count; i++)
            {

                String value = lstGroupsList[i];
                String sUID = value.Substring(0, value.IndexOf("|")); // usrID
                value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                String sUName = value;
                GroupsList_AddToVisualList(sUID, sUName);
            }
            lvList.EndUpdate();
            ApplyFilter();

            int iSelIdx = -1;
            //for (int i = 0; i < lvList.Items.Count; i++)
            //{
            //    String sUID = lvList.Items[i].SubItems[1].Text;
            //    if (iContUID == Convert.ToInt64(sUID))
            //        iSelIdx = i;
            //}

            LoadColumnsOrderAndWidth();

            Cursor = Cursors.Arrow;
            if (lvList.Items.Count > 0)
            {
                if (iSelIdx >= 0 && iSelIdx < lvList.Items.Count)
                {
                    lvList.Items[iSelIdx].Selected = true;
                    lvList.Items[iSelIdx].EnsureVisible();
                    lvList.TopItem = lvList.Items[iSelIdx];
                }
                else
                {
                    lvList.SelectedIndices.Add(0);
                    lvList.EnsureVisible(0);
                }
            }
            lvList_ItemChecked(null, null);

            if (initDialog)
            {
                if (lvList.Items.Count > 0)
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
        }

        private void ApplyFilter(bool bNotShowDialog = true)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
            for (int i = 0; i < iGroupHarCount; i++)
                for (int j = 0; j < iGroupHarAttrCount + 1; j++)
                    fe.sPersHar[i, j] = sGroupHar[i, j];

            LoadFormEditPersHarValues(fe, 100001, 1);

            fe.iPersHarAttrCount = iGroupHarAttrCount;
            fe.iPersHarCount = iGroupHarCount;
            fe.sFilePrefix = "grp";
            fe.Setup();

            if (bNotShowDialog || fe.ShowDialog() == DialogResult.OK)
            {
                SaveFormEditPersHarValues(fe, 100001, 1);
                String[] RQV = new String[iGroupHarCount];
                for (int iii = 0; iii < iGroupHarCount; iii++)
                {
                    RQV[Convert.ToInt32(fe.sPersHar[iii, 0]) - 1] = fe.sPersHar[iii, iGroupHarAttrCount];
                    sGroupHar[iii, iGroupHarAttrCount] = fe.sPersHar[iii, iGroupHarAttrCount];
                }

                FilterList(RQV);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text.Equals(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Отключить фильтр")))
            {
                FilterList(ClearFilter());
            }
            else
            {
                ApplyFilter(false);
            }
        }

        private String[] ClearFilter()
        {
            String[] RQV = new String[iGroupHarCount];
            for (int iii = 0; iii < iGroupHarCount; iii++)
            {
                RQV[iii] = "";
                sGroupHar[iii, iGroupHarAttrCount] = "";
            }

            return RQV;
        }

        private void FilterList(String[] RQV)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            buttonSelectGroup.Enabled = false;
            button5.Enabled = false;
            button16.Enabled = false;

            Cursor = Cursors.WaitCursor;
            lvList.SelectedIndices.Clear();
            lvList.BeginUpdate();
            lvList.Items.Clear();

            Boolean bRQVEmpty = true;
            for (int iv = 0; iv < RQV.Length; iv++)
            {
                if (RQV[iv].Length > 0)
                {
                    bRQVEmpty = false;
                    break;
                }
            }

            bool bFiltered = false;
            for (int i = 0; i < lstGroupsList.Count; i++)
            {

                String value = lstGroupsList[i];
                String sUID = value.Substring(0, value.IndexOf("|")); // usrID
                value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                String sUName = value;

                Boolean bEquals = bRQVEmpty;
                if (!bEquals)
                {
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                    {
                        List<String> lstGroupHarValues = new List<String>();
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                        lstGroupHarValues = new List<String>(srcFile);
                        String[] EQV = new String[iGroupHarCount];
                        foreach (String str in lstGroupHarValues)
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

                            if (!RQV[iv].ToLower().Equals(EQV[iv]))
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
                    else
                        bEquals = false;
                }

                if (bEquals)
                    GroupsList_AddToVisualList(sUID, sUName);
                else
                    bFiltered = true;
            }
            lvList.EndUpdate();
            Cursor = Cursors.Arrow;
            if (lvList.Items.Count > 0)
            {
                lvList.SelectedIndices.Add(0);
                lvList.EnsureVisible(0);
            }
            lvList_ItemChecked(null, null);

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

        public void GroupsList_AddToVisualList(String sUID, String sUName, int iVLIdx = -1)
        {
            String[] EQV = new String[iGroupHarCount];
            if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
            {
                List<String> lstGroupHarValues = new List<String>();
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                lstGroupHarValues = new List<String>(srcFile);
                foreach (String str in lstGroupHarValues)
                {
                    if (str == null)
                        continue;

                    if (str.Length == 0)
                        continue;

                    if (str.Length > 1)
                        EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                }
            }

            ListViewItem lvi;
            if (iVLIdx >= 0)
            {
                lvi = lvList.Items[iVLIdx];
                lvi.Text = sUName;
                lvi.SubItems[1].Text = sUID;

                if (iGroupHarCount <= 16)
                    for (int iOffs = 0; iOffs < iGroupHarCount; iOffs++)
                        lvi.SubItems[2 + iOffs].Text = EQV[iOffs];
            }
            else
            {
                lvi = new ListViewItem(sUName);
                lvi.SubItems.Add(sUID);
                if (iGroupHarCount <= 16)
                    for (int iOffs = 0; iOffs < iGroupHarCount; iOffs++)
                        lvi.SubItems.Add(EQV[iOffs]);
                lvList.Items.Add(lvi);
            }
        }

        private void GroupsList_Load()
        {
            lstGroupsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                lstGroupsList = new List<String>(srcFile);
            }
        }

        private void GroupsList_Remove(String sUD)
        {
            int iuserIdx = GroupsList_GetIdx(sUD);
            if (iuserIdx >= 0)
            {
                lstGroupsList.RemoveAt(iuserIdx);
                if (lstGroupsList.Count > 0)
                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstGroupsList, Encoding.UTF8);
                else
                    File.Delete(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));

                long _ownerId = Convert.ToInt64(sUD);
                if (mFormMain.dictWallMonitoring.ContainsKey(_ownerId))
                {
                    mFormMain.dictWallMonitoring.Remove(_ownerId);
                    mFormMain.Wall_SavePostToMonitoring();
                }

                if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + sUD + ".txt"));
            }
        }

        public int GroupsList_GetIdx(String sUD)
        {
            int iuserIdx = -1;
            for (int i = 0; i < lstGroupsList.Count; i++)
            {
                String str = lstGroupsList[i];
                if (str.Substring(0, str.IndexOf("|")).Equals(sUD))
                {
                    iuserIdx = i;
                    break;
                }
            }
            return iuserIdx;
        }

        public int GroupsList_GetVisualListIdx(String sUD)
        {
            int iuserIdx = -1;
            for (int k = 0; k < lvList.Items.Count; k++)
            {
                String str = lvList.Items[k].SubItems[1].Text;
                if (str.Equals(sUD))
                {
                    iuserIdx = k;
                    break;
                }
            }
            return iuserIdx;
        }

        private String GroupsList_GetRecord(String sUD)
        {
            for (int i = 0; i < lstGroupsList.Count; i++)
            {
                String str = lstGroupsList[i];
                if (str.Substring(0, str.IndexOf("|")).Equals(sUD))
                {
                    return str;
                }
            }
            return "";
        }

        private String GroupsList_GetField(String sUD, int iFieldIdx) // 0 - usrID, 1 - usrName
        {
            String retval = GroupsList_GetRecord(sUD);
            if (retval.Length > 0)
            {
                for (int i = 0; i < iFieldIdx; i++)
                    retval = retval.Substring(retval.IndexOf("|") + 1);
                if (iFieldIdx < 3)
                    retval = retval.Substring(0, retval.IndexOf("|"));
            }
            return retval;
        }

        public void ExternalGroupsList_Add(String ExternalPersonenID, String sUD, String sUName)
        {
            List<String> lstExternalContactsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt"));
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

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt"), lstExternalContactsList, Encoding.UTF8);
        }

        public void GroupsList_Add(String sUD, String sUName)
        {
            int iuserIdx = GroupsList_GetIdx(sUD);
            String userRec = sUD + "|" + sUName;
            if (iuserIdx >= 0)
                lstGroupsList[iuserIdx] = userRec;
            else
                lstGroupsList.Add(userRec);

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstGroupsList, Encoding.UTF8);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedIndices.Count <= 0)
            {
                if (lvList.SelectedIndices.Count > 0)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Вы уверены, что хотите удалить эту Группу?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Группы"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bool bCont = true;
                        int iSelIdx = lvList.SelectedIndices[0];
                        String sUID = lvList.Items[iSelIdx].SubItems[1].Text;

                        if (bCont)
                        {
                            if (sUID.Length > 0)
                            {
                                lvList.SelectedIndices.Clear();
                                lvList.Items.RemoveAt(iSelIdx);
                                GroupsList_Remove(sUID);

                                if (lvList.Items.Count > 0)
                                {
                                    if (iSelIdx >= lvList.Items.Count)
                                        iSelIdx--;
                                    lvList.SelectedIndices.Add(iSelIdx);
                                    lvList.EnsureVisible(iSelIdx);
                                }
                                else
                                {
                                    button2.Enabled = false;
                                    button3.Enabled = false;
                                    buttonSelectGroup.Enabled = false;
                                    button5.Enabled = false;
                                    button16.Enabled = false;
                                }
                                lvList_ItemChecked(null, null);

                            }
                        }
                    }
                }
            }
            else
            {
                if (lvList.CheckedItems.Count > 0)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_8", this.Name, "Вы уверены, что хотите удалить все отмеченные Группы?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Групп"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        foreach (ListViewItem item in lvList.CheckedItems)
                        {
                            String sUID = item.SubItems[1].Text;
                            GroupsList_Remove(sUID);
                        }

                        lvList.BeginUpdate();
                        foreach (ListViewItem item in lvList.CheckedItems)
                        {
                            String sUID = item.SubItems[1].Text;
                            lvList.Items.Remove(item);
                        }
                        lvList.EndUpdate();

                        EnableAllButtons();
                        FilterList(ClearFilter());
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
                    else if (text.StartsWith("https://vk.com/id"))
                    {
                        return ResolveID(text.Substring("https://vk.com/id".Length));
                    }
                    else if (text.StartsWith("http://vk.com/club"))
                    {
                        return ResolveID(text.Substring("http://vk.com/club".Length));
                    }
                    else if (text.StartsWith("https://vk.com/club"))
                    {
                        return ResolveID(text.Substring("https://vk.com/club".Length));
                    }
                    else if (text.StartsWith("http://vk.com/"))
                    {
                        return ResolveID(text.Substring("http://vk.com/".Length));
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
                            catch (Exception exp)
                            {
                                mFormMain.ExceptionToLogList("ResolveID", "ResolveID", exp);
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

        private void button6_Click(object sender, EventArgs e)
        {
            string value = "";
            if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Добавление Группы"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_10", this.Name, "Введите ID Группы:"), ref value) == DialogResult.OK)
            {
                String sUID = ResolveID(value);
                if (sUID.Equals(-1))
                {
                    MessageBox.Show("Группа не найдена", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Добавление Группы"));
                    return;
                }
                if (GroupsList_GetIdx(sUID) >= 0)
                {
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Группа с указанным ID уже есть в базе. Если она не отображается, смените настройки фильтра базы Групп по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Добавление Группы"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (FormMain.SocialNetwork == 0)
                    {
                        FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                        fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
                        for (int i = 0; i < iGroupHarCount; i++)
                        {
                            for (int j = 0; j < iGroupHarAttrCount; j++)
                                fe.sPersHar[i, j] = sGroupHar[i, j];
                            fe.sPersHar[i, iGroupHarAttrCount] = "";
                        }

                        LoadFormEditPersHarValues(fe, 100000, 1);

                        fe.iPersHarAttrCount = iGroupHarAttrCount;
                        fe.iPersHarCount = iGroupHarCount;
                        fe.sFilePrefix = "grp";
                        fe.Setup();

                        if (fe.ShowDialog() == DialogResult.OK)
                        {
                            SaveFormEditPersHarValues(fe, 100000, 1);
                            List<String> lstContHar = new List<String>();
                            for (int i = 0; i < iGroupHarCount; i++)
                                lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);
                            /*
                            do
                            {
                                try
                                {
                                    VkNet.Model.Group usrAdr = FormMain.api.Groups.GetById(Convert.ToInt64(sUID), GroupsFields.CanPost | GroupsFields.Counters | GroupsFields.MembersCount);
                                    String sUName = usrAdr.Name;

                                    List<String> lstGroupHarValues = new List<String>();

                                    if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                                    {
                                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                                        lstGroupHarValues = new List<String>(srcFile);
                                        for (int i = 0; i < iGroupHarCount; i++)
                                        {
                                            String sCV = lstContHar[i].Substring(lstContHar[i].IndexOf("|") + 1);
                                            if (sCV.Length > 0)
                                                lstGroupHarValues[i] = lstContHar[i];
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < iGroupHarCount; i++)
                                            lstGroupHarValues.Add(lstContHar[i]);
                                    }

                                    for (int i = 0; i < iGroupHarCount; i++)
                                    {
                                        if (lstGroupHarValues[i].IndexOf("#canpost#") > 0)
                                        {
                                            String svkv = usrAdr.CanPost ? "Yes" : "No";
                                            lstGroupHarValues[i] = lstGroupHarValues[i].Replace("#canpost#", svkv);
                                        }
                                        else if (lstGroupHarValues[i].IndexOf("#counters_followers#") > 0)
                                        {
                                            lstGroupHarValues[i] = lstGroupHarValues[i].Replace("#counters_followers#", usrAdr.Counters != null ? (usrAdr.Counters.Followers != null ? usrAdr.Counters.Followers.Value.ToString() : "") : "");
                                        }
                                        else if (lstGroupHarValues[i].IndexOf("#counters_members#") > 0)
                                        {
                                            lstGroupHarValues[i] = lstGroupHarValues[i].Replace("#counters_members#", usrAdr.MembersCount.HasValue ? usrAdr.MembersCount.ToString() : "");
                                        }
                                        else if (lstGroupHarValues[i].IndexOf("#clear#") > 0)
                                        {
                                            lstGroupHarValues[i] = lstGroupHarValues[i].Replace("#clear#", "");
                                        }

                                    }

                                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstGroupHarValues, Encoding.UTF8);
                                    GroupsList_Add(sUID, sUName);
                                    GroupsList_AddToVisualList(sUID, sUName);

                                    lvList.SelectedIndices.Clear();
                                    if (lvList.Items.Count > 0)
                                    {
                                        lvList.SelectedIndices.Add(lvList.Items.Count - 1);
                                        lvList.EnsureVisible(lvList.Items.Count - 1);
                                    }
                                    lvList_ItemChecked(null, null);
                                    break;
                                }
                                catch (Exception exp)
                                {
                                    ExceptionToLogList("FormEditGroupsDB.button6_Click", sUID, exp);
                                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_12", this.Name, "Ошибка запроса Группы с указанным ID..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Добавление Группы"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                                finally { }
                            }
                            while (true);
                            */
                        }
                    }
                }
            }
        }

        private String GetGroupParametersValue(String sContHarID)
        {
            for (int i = 0; i < lstGroupHarValues.Count; i++)
            {
                if (lstGroupHarValues[i].Substring(0, lstGroupHarValues[i].IndexOf("|")) == sContHarID)
                {
                    return lstGroupHarValues[i].Substring(lstGroupHarValues[i].IndexOf("|") + 1);
                }
            }
            return "";
        }

        private void LoadGroupParamersValues(String sID)
        {
            lstGroupHarValues = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sID + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sID + ".txt"));
                lstGroupHarValues = new List<String>(srcFile);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedIndices.Count <= 0)
            {
                if (lvList.SelectedIndices.Count > 0)
                {
                    int iSelIdx = lvList.SelectedIndices[0];
                    String sUID = lvList.Items[iSelIdx].SubItems[1].Text;

                    if (sUID.Length > 0)
                    {
                        LoadGroupParamersValues(sUID);
                        FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                        fe.Text += " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_13", this.Name, "Группы");
                        fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
                        for (int i = 0; i < iGroupHarCount; i++)
                        {
                            for (int j = 0; j < iGroupHarAttrCount; j++)
                                fe.sPersHar[i, j] = sGroupHar[i, j];
                            fe.sPersHar[i, iGroupHarAttrCount] = GetGroupParametersValue(sGroupHar[i, 0]);
                        }

                        fe.iPersHarAttrCount = iGroupHarAttrCount;
                        fe.iPersHarCount = iGroupHarCount;
                        fe.sFilePrefix = "grp";
                        fe.Setup();

                        if (fe.ShowDialog() == DialogResult.OK)
                        {
                            List<String> lstContHar = new List<String>();
                            for (int i = 0; i < iGroupHarCount; i++)
                                lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);
                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHar, Encoding.UTF8);
                            GroupsList_AddToVisualList(sUID, lvList.Items[iSelIdx].SubItems[0].Text, iSelIdx);
                        }
                    }
                }
            }
            else
            {
                if (lvList.CheckedItems.Count > 0)
                {
                    List<String> lstContHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_14", this.Name, "Замена/установка значений характеристик Групп"), -1);
                    if (lstContHar == null)
                        return;

                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_15", this.Name, "Вы уверены, что хотите задать характеристики для всех отмеченных Групп?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_16", this.Name, "Редактирование характеристик Групп"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        for (int k = 0; k < lvList.Items.Count; k++)
                        {
                            if (lvList.Items[k].Checked)
                            {
                                String sUID = lvList.Items[k].SubItems[1].Text;
                                String sUName = lvList.Items[k].SubItems[0].Text;

                                List<String> lstContHarCurrent = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                                    lstContHarCurrent = new List<String>(srcFile);
                                    for (int i = 0; i < iGroupHarCount; i++)
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
                                    for (int i = 0; i < iGroupHarCount; i++)
                                        lstContHarCurrent.Add(lstContHar[i]);
                                }

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                GroupsList_AddToVisualList(sUID, sUName, k);
                                GroupsList_Add(sUID, sUName);
                            }
                        }

                        EnableAllButtons();
                    }
                }

            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
            {
                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 1);
                fecic.groupBox2.Visible = false;

                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;

                String sImportIDs = SelectPersonen("Выберите Персонажей, группы которых будут импортированы");
                if (sImportIDs.Length == 0)
                    return;

                iImportMode = 1;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
                for (int i = 0; i < iGroupHarCount; i++)
                {
                    for (int j = 0; j < iGroupHarAttrCount; j++)
                        fe.sPersHar[i, j] = sGroupHar[i, j];
                    fe.sPersHar[i, iGroupHarAttrCount] = "";
                }
                LoadFormEditPersHarValues(fe, iImportMode, 1);

                fe.iPersHarAttrCount = iGroupHarAttrCount;
                fe.iPersHarCount = iGroupHarCount;
                fe.sFilePrefix = "grp";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValues(fe, iImportMode, 1);
                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < iGroupHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);
                    List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                    if (lstFilterHar != null)
                        StartImportGroupsThread("UserIDs=" + sImportIDs, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, true);
                }

            }
            else
            {
                this.button7.Enabled = false;
                this.bwProgress.CancelAsync();
            }

        }

        private void lvList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool bBS = lvList.SelectedIndices.Count > 0;
            button2.Enabled = bBS || lvList.CheckedItems.Count > 0;
            button3.Enabled = bBS || lvList.CheckedItems.Count > 0;
            buttonSelectGroup.Enabled = bBS || lvList.CheckedItems.Count > 0; ;
            button16.Enabled = bBS || lvList.CheckedItems.Count > 0;
            button5.Enabled = bBS || lvList.CheckedItems.Count > 0;
        }


        private void DisableAllButtons()
        {
            this.buttonSelectGroup.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            buttonSelectGroup.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            this.button6.Enabled = false;
            this.button7.Enabled = false;
            buttonImportContacterGroup.Enabled = false;
            this.button9.Enabled = false;
            this.button10.Enabled = false;
            this.button12.Enabled = false;
            this.button13.Enabled = false;
            this.button14.Enabled = false;
            this.button15.Enabled = false;
            this.button16.Enabled = false;
            this.button18.Enabled = false;
            this.button19.Enabled = false;
            this.button20.Enabled = false;
            this.button21.Enabled = false;
            this.button25.Enabled = false;
            this.button26.Enabled = false;
            tbImportAdminOfGroupsAsContacters.Enabled = false;
            tbImportAsContactersFromGroupsAdmins.Enabled = false;
            tbImportAsContactersFromGroupsAllUsers.Enabled = false;
            tbImportAsContactersFromGroupsAuthors100.Enabled = false;
            tbImportAsContactersFromGroupsAuthorsAndComments.Enabled = false;
            tbImportAsContactersFromGroupsComments.Enabled = false;

        }

        private void EnableAllButtons()
        {
            this.buttonSelectGroup.Enabled = true;
            this.button2.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            this.button3.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            buttonSelectGroup.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            this.button4.Enabled = true;
            this.button5.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            this.button6.Enabled = true;
            this.button7.Enabled = FormMain.SocialNetwork == 0;
            buttonImportContacterGroup.Enabled = FormMain.SocialNetwork == 0;
            this.button9.Enabled = true;
            this.button15.Enabled = true;
            this.button26.Enabled = FormMain.SocialNetwork == 0;
            this.button16.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            this.button19.Enabled = FormMain.SocialNetwork == 0;
            lvList_SelectedIndexChanged(null, null);
            lvList_ItemChecked(null, null);

        }

        private void StartImportGroupsThread(String sGID, List<String> lstContHar, List<String> lstFilterHar, String sPersUsersIDs, int maxGroupsCount, bool GroupsOrContacts, Dictionary<long, HashSet<long>> usersGroupIDs=null, int _nudDuplicatesAction=0)
        {
            // This method runs on the main thread.
            this.pbProgress.Visible = true;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Value = 0;
            this.pbProgress.Maximum = 10;
            this.bDoImportGroups = false;

            DisableAllButtons();
            button9.Text = "X";
            button9.Enabled = true;

            // Initialize the object that the background worker calls.
            if (GroupsOrContacts)
            {
                ImportGroups WC = new ImportGroups(this, mFormMain);
                WC.sGroupID = sGID;
                WC.iPersUserID = iPersUserID;
                WC.sPersUsersIDs = sPersUsersIDs;
                WC.lstContHar = lstContHar;
                WC.lstFilterHar = lstFilterHar;
                WC.iGroupHarCount = iGroupHarCount;
                WC.maxGroupsCount = maxGroupsCount;
                WC.nudDuplicatesAction = _nudDuplicatesAction;
                // Start the asynchronous operation.
                bwProgress.RunWorkerAsync(WC);
            }
            else
            {
                ImportContactsFromGroup WC = new ImportContactsFromGroup(this, mFormMain, true);
                if (usersGroupIDs != null)
                    WC.usersGroupIDs = usersGroupIDs;
                WC.sGroupID = sGID;
                WC.iPersUserID = iPersUserID;
                WC.sPersUsersIDs = sPersUsersIDs;
                WC.lstContHar = lstContHar;
                WC.lstFilterHar = lstFilterHar;
                WC.iContHarCount = iGroupHarCount;
                WC.maxContacterCount = maxGroupsCount;
                WC.nudDuplicatesAction = _nudDuplicatesAction;
                // Start the asynchronous operation.
                bwProgress.RunWorkerAsync(WC);
            }
        }

        delegate void StringParameterDelegate(string uid, string uname, int idx);
        public void UpdateProgress_AddToVisualList(string uid, string uname, int idx)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new StringParameterDelegate(UpdateProgress_AddToVisualList), new object[] { uid, uname, idx });
                return;
            }
            // Must be on the UI thread if we've got this far
            GroupsList_AddToVisualList(uid, uname, idx);
        }

        delegate int UpdateProgress_GetVisualListIdxDelegate(string uid);
        public int UpdateProgress_GetVisualListIdx(string uid)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                return (int)this.Invoke(new UpdateProgress_GetVisualListIdxDelegate(GroupsList_GetVisualListIdx), new object[] { uid });
            }
            // Must be on the UI thread if we've got this far
            else
                return GroupsList_GetVisualListIdx(uid);
        }

        private void bwProgress_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;

            // Get the Words object and call the main method.
            if (e.Argument is ImportContactsFromGroup)
            {
                ImportContactsFromGroup WC = (ImportContactsFromGroup)e.Argument;
                stateImported = null;
                WC.ImportContacts(worker, e);
            }
            else if (e.Argument is ImportGroups)
            {
                ImportGroups WC = (ImportGroups)e.Argument;
                stateImported = null;
                WC.ImportContacts(worker, e);
            }
        }

        ImportContactsFromGroup.CurrentState stateImportedContacts = null;
        ImportGroups.CurrentState stateImported = null;
        private void bwProgress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is ImportContactsFromGroup.CurrentState)
            {
                stateImportedContacts = (ImportContactsFromGroup.CurrentState)e.UserState;
                if (this.pbProgress.Maximum != stateImportedContacts.ContactsTotal)
                    this.pbProgress.Maximum = stateImportedContacts.ContactsTotal;
                this.pbProgress.Value = stateImportedContacts.ContactsImported;
                this.pbProgress.Text = stateImportedContacts.ContactsImported.ToString() + "/" + stateImportedContacts.ContactsTotal.ToString() + " (" + stateImportedContacts.ContactsAdded.ToString() + ")";

                this.Text = stateImportedContacts.sUName + (stateImportedContacts.dtLastSeen.Length > 0 ? (" (" + stateImportedContacts.dtLastSeen + ")") : "");
            }
            else if (e.UserState is ImportGroups.CurrentState)
            {
                stateImported = (ImportGroups.CurrentState)e.UserState;
                if (this.pbProgress.Maximum != stateImported.GroupsTotal)
                    this.pbProgress.Maximum = stateImported.GroupsTotal;

                this.pbProgress.Value = stateImported.GroupsImported;
                this.pbProgress.Text = stateImported.GroupsImported.ToString() + "/" + stateImported.GroupsTotal.ToString() + " (" + stateImported.GroupsAdded.ToString() + ")";

                this.Text = stateImported.sUName;
            }
        }

        private void bwProgress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.pbProgress.Visible = false;
            this.bDoImportGroups = true;
            this.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_21", this.Name, "Редактирование базы Групп");
            button9.Text = ">";
            EnableAllButtons();

            if (e.Error != null)
                MessageBox.Show("Error: " + e.Error.Message, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Группы"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (e.Cancelled)
            {
                if (stateImported != null)
                    MessageBox.Show((stateImported != null ? ("Импортировано " + stateImported.GroupsAdded.ToString() + " групп\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_23", this.Name, "Импорт данных Групп прерван. Если какие-то из Группы не отображаются в списке, смените настройки фильтра базы Групп по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Групп"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (stateImportedContacts != null)
                    MessageBox.Show((stateImportedContacts != null ? ("Импортировано " + stateImportedContacts.ContactsAdded.ToString() + " контактеров\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_23", this.Name, "Импорт данных Контактеров прерван. Если какие-то из Контактеров не отображаются в списке, смените настройки фильтра базы Контактеров по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (stateImported != null)
                    MessageBox.Show((stateImported != null ? ("Импортировано " + stateImported.GroupsAdded.ToString() + " групп\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_24", this.Name, "Импорт данных Групп завершен. Если какие-то из Группы не отображаются в списке, смените настройки фильтра базы Групп по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Групп"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if (stateImportedContacts != null)
                    MessageBox.Show((stateImportedContacts != null ? ("Импортировано " + stateImportedContacts.ContactsAdded.ToString() + " контактеров\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_24", this.Name, "Импорт данных Контактеров завершен. Если какие-то из Контактеров не отображаются в списке, смените настройки фильтра базы Контактеров по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (lvList.Items.Count > 0)
                {
                    int iSelIdx = 0;
                    if (lvList.SelectedIndices.Count > 0)
                        iSelIdx = lvList.SelectedIndices[0] + 1;

                    if (iSelIdx >= lvList.Items.Count)
                        iSelIdx = 0;

                    int iSelIdxStart = iSelIdx;
                    bool bNotFound = true;
                    String[] RQV = textBox1.Text.ToLower().Trim().Split(' ');
                    int RQVwc = RQV.Length;
                    if (RQVwc == 0)
                        return;

                    do
                    {
                        String[] EQV = lvList.Items[iSelIdx].SubItems[0].Text.ToLower().Trim().Split(' ');
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
                            if (iSelIdx >= lvList.Items.Count)
                                iSelIdx = 0;

                            if (iSelIdxStart == iSelIdx)
                                break;
                        }
                    }
                    while (bNotFound);

                    if (!bNotFound)
                    {
                        lvList.SelectedIndices.Add(iSelIdx);
                        lvList.EnsureVisible(iSelIdx);
                    }
                }
            }
            else
            {
                this.button9.Enabled = false;
                if (!bDoImportGroups)
                    this.bwProgress.CancelAsync();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                button9_Click(null, null);
        }

        private void SaveFormEditPersHarValuesContacters(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            for (int i = 0; i < mFormMain.iContHarCount; i++)
                lstContHar.Add(fe.sPersHar[i, mFormMain.iContHarAttrCount]);

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"), lstContHar, Encoding.UTF8);
        }

        private void LoadFormEditPersHarValuesContacters(FormEditPersHarValues fe, int importmode, int submode)
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
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    fe.sPersHar[i, mFormMain.iContHarAttrCount] = lstContHar[i];
                }
            }
        }

        //---
        private void SaveFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            for (int i = 0; i < iGroupHarCount; i++)
                lstContHar.Add(fe.sPersHar[i, iGroupHarAttrCount]);

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesGroups_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"), lstContHar, Encoding.UTF8);
        }

        private void LoadFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesGroups_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesGroups_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"));
                lstContHar = new List<String>(srcFile);
            }

            if (lstContHar.Count > 0)
            {
                for (int i = 0; i < iGroupHarCount; i++)
                {
                    fe.sPersHar[i, iGroupHarAttrCount] = lstContHar[i];
                }
            }
        }

        //---
        private List<String> GetHarValues(String sTitle, int importmode)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
            for (int i = 0; i < iGroupHarCount; i++)
            {
                for (int j = 0; j < iGroupHarAttrCount; j++)
                    fe.sPersHar[i, j] = sGroupHar[i, j];
                fe.sPersHar[i, iGroupHarAttrCount] = "";
            }
            LoadFormEditPersHarValues(fe, importmode, 0);

            fe.iPersHarAttrCount = iGroupHarAttrCount;
            fe.iPersHarCount = iGroupHarCount;
            fe.sFilePrefix = "grp";
            fe.Text = sTitle;
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                SaveFormEditPersHarValues(fe, importmode, 0);

                List<String> lstContHar = new List<String>();
                for (int i = 0; i < iGroupHarCount; i++)
                    lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);
                return lstContHar;
            }
            return null;
        }

        private List<String> GetHarValuesContacters(String sTitle, int importmode)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
            for (int i = 0; i < mFormMain.iContHarCount; i++)
            {
                for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                    fe.sPersHar[i, j] = mFormMain.sContHar[i, j];
                fe.sPersHar[i, mFormMain.iContHarAttrCount] = "";
            }
            LoadFormEditPersHarValuesContacters(fe, importmode, 0);

            fe.iPersHarAttrCount = mFormMain.iContHarAttrCount;
            fe.iPersHarCount = mFormMain.iContHarCount;
            fe.sFilePrefix = "cont";
            fe.Text = sTitle;
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                SaveFormEditPersHarValuesContacters(fe, importmode, 0);

                List<String> lstContHar = new List<String>();
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                    lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, mFormMain.iContHarAttrCount]);
                return lstContHar;
            }
            return null;


        }

        private void button10_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            lvList.BeginUpdate();
            for (int k = 0; k < lvList.Items.Count; k++)
            {
                lvList.Items[k].Checked = true;
            }
            lvList.EndUpdate();
            Cursor = Cursors.Arrow;
        }

        private void lvList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                button12.Enabled = true;
                button14.Enabled = FormMain.SocialNetwork == 0;
                button18.Enabled = true;
                button25.Enabled = true;
                tbImportAdminOfGroupsAsContacters.Enabled = true;
                tbImportAsContactersFromGroupsAdmins.Enabled = true;
                tbImportAsContactersFromGroupsAllUsers.Enabled = true;
                tbImportAsContactersFromGroupsAuthors100.Enabled = true;
                tbImportAsContactersFromGroupsAuthorsAndComments.Enabled = true;
                tbImportAsContactersFromGroupsComments.Enabled = true;
                button20.Enabled = true;
                button21.Enabled = true;
            }
            else
            {
                button12.Enabled = false;
                button14.Enabled = false;
                button18.Enabled = false;
                button25.Enabled = false;
                tbImportAdminOfGroupsAsContacters.Enabled = false;
                tbImportAsContactersFromGroupsAdmins.Enabled = false;
                tbImportAsContactersFromGroupsAllUsers.Enabled = false;
                tbImportAsContactersFromGroupsAuthors100.Enabled = false;
                tbImportAsContactersFromGroupsAuthorsAndComments.Enabled = false;
                tbImportAsContactersFromGroupsComments.Enabled = false;
                button20.Enabled = false;
                button21.Enabled = false;
            }

            if (lvList.Items.Count > 0 && lvList.CheckedItems.Count != lvList.Items.Count)
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
            button2.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            button3.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            buttonSelectGroup.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            button16.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            button5.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            lvList.BeginUpdate();
            for (int k = 0; k < lvList.Items.Count; k++)
            {
                lvList.Items[k].Checked = false;
            }
            lvList.EndUpdate();
            Cursor = Cursors.Arrow;
        }

        private void SelectFilterList(String[] RQV)
        {
            Cursor = Cursors.WaitCursor;

            lvList.BeginUpdate();

            Boolean bRQVEmpty = true;
            for (int iv = 0; iv < RQV.Length; iv++)
            {
                if (RQV[iv].Length > 0)
                {
                    bRQVEmpty = false;
                    break;
                }
            }


            for (int i = 0; i < lvList.Items.Count; i++)
            {

                String sUID = lvList.Items[i].SubItems[1].Text;

                Boolean bEquals = bRQVEmpty;
                if (!bEquals)
                {
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                    {
                        List<String> lstGroupHarValues = new List<String>();
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                        lstGroupHarValues = new List<String>(srcFile);
                        String[] EQV = new String[iGroupHarCount];
                        foreach (String str in lstGroupHarValues)
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

                            if (!RQV[iv].ToLower().Equals(EQV[iv]))
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
                                    String sval = RQV[iv].Trim().ToLower();
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
                    else
                        bEquals = false;
                }

                if (bEquals)
                    lvList.Items[i].Checked = true;
            }
            lvList.EndUpdate();
            Cursor = Cursors.Arrow;

        }

        private void button13_Click(object sender, EventArgs e)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
            for (int i = 0; i < iGroupHarCount; i++)
                for (int j = 0; j < iGroupHarAttrCount + 1; j++)
                    fe.sPersHar[i, j] = sGroupHar[i, j];

            fe.iPersHarAttrCount = iGroupHarAttrCount;
            fe.iPersHarCount = iGroupHarCount;
            fe.sFilePrefix = "grp";
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                String[] RQV = new String[iGroupHarCount];
                for (int iii = 0; iii < iGroupHarCount; iii++)
                {
                    RQV[Convert.ToInt32(fe.sPersHar[iii, 0]) - 1] = fe.sPersHar[iii, iGroupHarAttrCount];
                    sGroupHar[iii, iGroupHarAttrCount] = fe.sPersHar[iii, iGroupHarAttrCount];
                }

                SelectFilterList(RQV);
            }

        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
            {
                if (lvList.CheckedItems.Count > 0)
                {
                    String sImportIDs = "";
                    foreach (ListViewItem item in lvList.CheckedItems)
                    {
                        String sUID = item.SubItems[1].Text;
                        sImportIDs += sUID + "|";
                    }

                    iImportMode = 3;
                    //string value = "";
                    FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                    List<String> lstFilterHar = new List<String>();
                    fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
                    for (int i = 0; i < iGroupHarCount; i++)
                    {
                        for (int j = 0; j < iGroupHarAttrCount; j++)
                            fe.sPersHar[i, j] = sGroupHar[i, j];
                        fe.sPersHar[i, iGroupHarAttrCount] = "";
                        lstFilterHar.Add(fe.sPersHar[i, 0] + "|");
                    }
                    LoadFormEditPersHarValues(fe, iImportMode, 1);

                    fe.iPersHarAttrCount = iGroupHarAttrCount;
                    fe.iPersHarCount = iGroupHarCount;
                    fe.sFilePrefix = "grp";
                    fe.Setup();

                    if (fe.ShowDialog() == DialogResult.OK)
                    {
                        SaveFormEditPersHarValues(fe, iImportMode, 1);
                        List<String> lstContHar = new List<String>();
                        for (int i = 0; i < iGroupHarCount; i++)
                            lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);

                        if (lstFilterHar != null)
                            StartImportGroupsThread("UIDs=" + sImportIDs, lstContHar, lstFilterHar, "", 100000, true);
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
            foreach (ColumnHeader column in lvList.Columns)
            {
                //Use the column.Width Property to get the width and save it to XML.
                lstcolumns.Add(column.Index.ToString() + "|" + column.DisplayIndex.ToString() + "|" + column.Width.ToString());
            }

            if (lstcolumns.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditGroupsDB.columns"), lstcolumns, Encoding.UTF8);
        }

        private void LoadColumnsOrderAndWidth()
        {
            List<String> lstcolumns = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditGroupsDB.columns")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditGroupsDB.columns"));
                lstcolumns = new List<String>(srcFile);
            }
            if (lstcolumns.Count > 0)
            {
                lvList.BeginUpdate();
                foreach (String value in lstcolumns)
                {
                    String str = value;
                    int iidx = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                    str = str.Substring(str.IndexOf("|") + 1);
                    int idispidx = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                    str = str.Substring(str.IndexOf("|") + 1);
                    int iwidth = Convert.ToInt32(str);
                    //Use the column.Width Property to get the width and save it to XML.
                    lvList.Columns[iidx].DisplayIndex = idispidx;
                    lvList.Columns[iidx].Width = iwidth;
                }
                lvList.EndUpdate();
            }
        }

        private void FormEditGroupsDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveColumnsOrderAndWidth();
        }

        private void lvList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                lvList.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (lvList.Sorting == SortOrder.Ascending)
                    lvList.Sorting = SortOrder.Descending;
                else
                    lvList.Sorting = SortOrder.Ascending;
            }

            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.lvList.ListViewItemSorter = new ListViewItemComparer(e.Column, lvList.Sorting);
            // Call the sort method to manually sort.
            lvList.Sort();
        }

        private int sortColumn = -1;
        class ListViewItemComparer : IComparer
        {
            private int col;
            private SortOrder order;
            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }
            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }
            public int Compare(object x, object y)
            {
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                        ((ListViewItem)y).SubItems[col].Text);
                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                return returnVal;
            }
        }

        private String SelectPersonen(string dialog_ditle = "")
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
            fepdb.Text = dialog_ditle.Length == 0 ? NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_25", this.Name, "Выбор Персонажей") : dialog_ditle;
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
            if (bDoImportGroups)
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
                            fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
                            for (int i = 0; i < iGroupHarCount; i++)
                            {
                                for (int j = 0; j < iGroupHarAttrCount; j++)
                                    fe.sPersHar[i, j] = sGroupHar[i, j];
                                fe.sPersHar[i, iGroupHarAttrCount] = "";
                            }
                            LoadFormEditPersHarValues(fe, iImportMode, 1);

                            fe.iPersHarAttrCount = iGroupHarAttrCount;
                            fe.iPersHarCount = iGroupHarCount;
                            fe.sFilePrefix = "grp";
                            fe.Setup();

                            if (fe.ShowDialog() == DialogResult.OK)
                            {
                                SaveFormEditPersHarValues(fe, iImportMode, 1);
                                List<String> lstContHar = new List<String>();
                                for (int i = 0; i < iGroupHarCount; i++)
                                    lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);
                                List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                                if (lstFilterHar != null)
                                    StartImportGroupsThread("UIDs=" + sImportIDs, lstContHar, lstFilterHar, sPersUIDs, 100000, true);
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
            groupBox1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_27", this.Name, "Группы") + " (" + (lvList.CheckedItems.Count > 0 ? lvList.CheckedItems.Count.ToString() + " / " : "") + lvList.Items.Count.ToString() + ")";
        }

        private bool INIT_PERSONE_DIALOG = false;
        private void button16_Click(object sender, EventArgs e)
        {
            List<String> lstInitDialogContacts = new List<String>();
            //---
            if (lvList.CheckedIndices.Count <= 0)
            {
                if (lvList.SelectedIndices.Count > 0)
                {
                    int iSelIdx = lvList.SelectedIndices[0];
                    String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                    String sUName = lvList.Items[iSelIdx].SubItems[0].Text;

                    if (sUID.Length > 0)
                    {
                        lstInitDialogContacts.Add(sUID + "|" + sUName);
                    }
                }
            }
            else
            {
                if (lvList.CheckedItems.Count > 0)
                {
                    DisableAllButtons();

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (lvList.Items[k].Checked)
                        {
                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            lstInitDialogContacts.Add(sUID + "|" + sUName);
                        }
                    }

                    EnableAllButtons();
                }

            }

            if (lstInitDialogContacts.Count > 0)
            {
                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_initdialog_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstInitDialogContacts, Encoding.UTF8);

                //---
                int oldAlgorithmID = mFormMain.adbrCurrent.ID;
                FormInitGroupDialog fe = new FormInitGroupDialog(mFormMain);
                fe.sContHar = new String[mFormMain.iGroupHarCount, mFormMain.iGroupHarAttrCount + 1];
                for (int i = 0; i < mFormMain.iGroupHarCount; i++)
                {
                    for (int j = 0; j < mFormMain.iGroupHarAttrCount; j++)
                        fe.sContHar[i, j] = mFormMain.sGroupHar[i, j];
                    fe.sContHar[i, iGroupHarAttrCount] = "";
                }
                fe.iContHarCount = mFormMain.iGroupHarCount;
                fe.iContHarAttrCount = mFormMain.iGroupHarAttrCount;
                fe.Setup(iPersUserID.ToString() + " (" + mFormMain.userLogin + ", " + mFormMain.userPassword + ")", iPersUserID, INIT_PERSONE_DIALOG, sAdditionalPersonen);
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
                int[] igetorder = new int[lvList.Columns.Count];
                foreach (ColumnHeader column in lvList.Columns)
                {
                    igetorder[column.DisplayIndex] = column.Index;
                }

                String sColumnNames = "";
                for (int j = 0; j < igetorder.Length; ++j)
                {
                    sColumnNames += lvList.Columns[igetorder[j]].Text + ";";
                    //lstcolumns.Add(column.Index.ToString() + "|" + column.DisplayIndex.ToString() + "|" + column.Width.ToString());
                }
                lstUsersToExport.Add(sColumnNames);

                foreach (ListViewItem item in lvList.CheckedItems)
                {
                    String sColumnValues = "";
                    for (int j = 0; j < igetorder.Length; ++j)
                        sColumnValues += item.SubItems[igetorder[j]].Text + ";";

                    lstUsersToExport.Add(sColumnValues);
                }

                File.WriteAllLines(saveFileDialog.FileName, lstUsersToExport, Encoding.UTF8);
            }

        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
            {
                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 1);
                fecic.groupBox2.Visible = false;
                fecic.groupBox3.Visible = true;

                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton5.Checked ? 0 : 1;

                iImportMode = 6;

                String value = "";
                if (FormMain.InputBox(this, "Импорт через Поиск", "Тематика групп:", ref value) == DialogResult.OK)
                {
                    FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                    fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
                    for (int i = 0; i < iGroupHarCount; i++)
                    {
                        for (int j = 0; j < iGroupHarAttrCount; j++)
                            fe.sPersHar[i, j] = sGroupHar[i, j];
                        fe.sPersHar[i, iGroupHarAttrCount] = "";
                    }
                    LoadFormEditPersHarValues(fe, iImportMode, 1);

                    fe.iPersHarAttrCount = iGroupHarAttrCount;
                    fe.iPersHarCount = iGroupHarCount;
                    fe.sFilePrefix = "grp";
                    fe.Setup();

                    if (fe.ShowDialog() == DialogResult.OK)
                    {
                        SaveFormEditPersHarValues(fe, iImportMode, 1);
                        List<String> lstContHar = new List<String>();
                        for (int i = 0; i < iGroupHarCount; i++)
                            lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);
                        List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                        if (lstFilterHar != null)
                            StartImportGroupsThread("Search=" + value, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, true, null, nudDuplicatesAction);
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
                if (lvList.SelectedIndices.Count > 0)
                {
                    int iSelIdx = lvList.SelectedIndices[0];
                    String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                    System.Diagnostics.Process.Start("http://vk.com/club" + sUID);
                }

            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_28", this.Name, "Вы действительно хотите скопировать отмеченные Группы другим Персонажам?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_29", this.Name, "Копирование Групп"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                String sPersUsersIDs = SelectPersonen();
                if (sPersUsersIDs.Length == 0)
                    return;

                if (("|" + sPersUsersIDs).IndexOf("|" + iPersUserID.ToString() + "|") >= 0)
                {
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_30", this.Name, "Нельзя скопировать Группы активному Персонажу"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_29", this.Name, "Копирование Групп"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (lvList.CheckedItems.Count > 0)
                {
                    DisableAllButtons();

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (lvList.Items[k].Checked)
                        {
                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;

                            String scpuID = sPersUsersIDs.Substring(0, sPersUsersIDs.IndexOf("|"));
                            sPersUsersIDs = sPersUsersIDs.Substring(sPersUsersIDs.IndexOf("|") + 1);
                            sPersUsersIDs = sPersUsersIDs + scpuID + "|";
                            String sAttrFileName = "grp_" + FormMain.getSocialNetworkPrefix() + scpuID + "_" + sUID + ".txt";

                            if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                            {
                                File.Copy(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), Path.Combine(FormMain.sDataPath, sAttrFileName), true);
                                ExternalGroupsList_Add(scpuID, sUID, sUName);
                            }
                        }
                    }

                    EnableAllButtons();

                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_31", this.Name, "Копирование отмеченных Групп выбранным Персонажам завершено."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_29", this.Name, "Копирование Групп"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_33", this.Name, "Вы действительно хотите переместить отмеченные Группы другим Персонажам?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_32", this.Name, "Перемещение Групп"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                String sPersUsersIDs = SelectPersonen();
                if (sPersUsersIDs.Length == 0)
                    return;

                if (("|" + sPersUsersIDs).IndexOf("|" + iPersUserID.ToString() + "|") >= 0)
                {
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_34", this.Name, "Нельзя переместить Группы активному Персонажу"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_32", this.Name, "Перемещение Групп"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (lvList.CheckedItems.Count > 0)
                {
                    DisableAllButtons();

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (lvList.Items[k].Checked)
                        {
                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;

                            String scpuID = sPersUsersIDs.Substring(0, sPersUsersIDs.IndexOf("|"));
                            sPersUsersIDs = sPersUsersIDs.Substring(sPersUsersIDs.IndexOf("|") + 1);
                            sPersUsersIDs = sPersUsersIDs + scpuID + "|";
                            String sAttrFileName = "grp_" + FormMain.getSocialNetworkPrefix() + scpuID + "_" + sUID + ".txt";

                            if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                            {
                                File.Copy(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), Path.Combine(FormMain.sDataPath, sAttrFileName), true);
                                ExternalGroupsList_Add(scpuID, sUID, sUName);
                            }
                        }
                    }

                    foreach (ListViewItem item in lvList.CheckedItems)
                    {
                        String sUID = item.SubItems[1].Text;
                        if (!(FormMain.SocialNetwork == 1 && iPersUserID == 0 && sUID.Equals("1")))
                            GroupsList_Remove(sUID);
                    }

                    foreach (ListViewItem item in lvList.CheckedItems)
                    {
                        String sUID = item.SubItems[1].Text;
                        if (!(FormMain.SocialNetwork == 1 && iPersUserID == 0 && sUID.Equals("1")))
                            lvList.Items.Remove(item);
                    }

                    EnableAllButtons();
                    FilterList(ClearFilter());

                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_35", this.Name, "Перемещение отмеченных Групп выбранным Персонажам завершено."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_32", this.Name, "Перемещение Групп"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void button25_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<String> lstUsersToExport = new List<string>();
                foreach (ListViewItem item in lvList.CheckedItems)
                {
                    String sUID = item.SubItems[1].Text;
                    lstUsersToExport.Add(sUID);
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
                            int[] iFieldHar = new int[iGroupHarCount];

                            for (int i = 0; i < IW.Length; i++)

                                if (IW[i].ToLower().Equals("id"))
                                {
                                    iFieldID = i;
                                    break;
                                }
                            for (int i = 0; i < iGroupHarCount; i++)
                            {
                                iFieldHar[i] = -1;
                                for (int j = 0; j < IW.Length; j++)
                                {
                                    if (sGroupHar[i, 1].ToLower().Equals(IW[j].ToLower()))
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
                                String sAttrFileName = "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt";

                                List<String> lstContHarValuesTemp = new List<String>();
                                for (int i = 0; i < iGroupHarCount; i++)
                                {
                                    lstContHarValuesTemp.Add(sGroupHar[i, 0] + "|" + (iFieldHar[i] >= 0 ? IWRecord[iFieldHar[i]] : ""));
                                }

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName), lstContHarValuesTemp, Encoding.UTF8);
                                GroupsList_Add(sUID, sUName);
                                GroupsList_AddToVisualList(sUID, sUName, GroupsList_GetVisualListIdx(sUID));

                            }

                        }
                    }
                }//if (File.Exists(openFileDialog.FileName))
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                List<String> lstContHarVal = new List<String>();
                int iAlgValHar = 15;
                if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values"));
                    lstContHarVal = new List<String>(srcFile);
                    for (int i = 0; i < iGroupHarCount; i++)
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
                    for (int i = 0; i < iGroupHarCount; i++)
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

                for (int k = 0; k < lvList.Items.Count; k++)
                {
                    if (lvList.Items[k].Checked)
                    {
                        String sUID = lvList.Items[k].SubItems[1].Text;
                        String sUName = lvList.Items[k].SubItems[0].Text;

                        List<String> lstContHarCurrent = new List<String>();
                        if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                            lstContHarCurrent = new List<String>(srcFile);

                            int algid = getGroupAlgorithmID(Convert.ToInt64(sUID));
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
                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                            GroupsList_AddToVisualList(sUID, sUName, k);
                            GroupsList_Add(sUID, sUName);
                        }

                    }
                }

                EnableAllButtons();
            }

        }

        private int getGroupAlgorithmID(long contid)
        {
            int algid = -1;
            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + contid.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + contid.ToString() + ".txt"));
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
                    for (int i = 0; i < iGroupHarCount; i++)
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
                    for (int i = 0; i < iGroupHarCount; i++)
                    {
                        if (i == 15)
                            lstContHarVal.Add("#algorithm#");
                        else
                            lstContHarVal.Add("");
                    }

                    File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values"), lstContHarVal, Encoding.UTF8);
                }

                if (lvList.CheckedIndices.Count <= 0)
                {
                    if (lvList.SelectedIndices.Count > 0)
                    {
                        int iSelIdx = lvList.SelectedIndices[0];
                        String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                        String sUName = lvList.Items[iSelIdx].SubItems[0].Text;

                        if (sUID.Length > 0)
                        {
                            List<String> lstContHarCurrent = new List<String>();
                            if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                                lstContHarCurrent = new List<String>(srcFile);
                                lstContHarCurrent[iAlgValHar] = lstContHarCurrent[iAlgValHar].Substring(0, lstContHarCurrent[iAlgValHar].IndexOf("|") + 1) + SelectedAlgorithm.Name;

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + sUID + ".txt"), lstAlgorithmsDBRecordTS, Encoding.UTF8);

                                GroupsList_AddToVisualList(sUID, sUName, iSelIdx);
                                GroupsList_Add(sUID, sUName);
                            }
                        }
                    }
                }
                else
                {
                    if (lvList.CheckedItems.Count > 0)
                    {
                        if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_37", this.Name, "Вы уверены, что хотите установить выбранный алгоритм для всех отмеченных Групп?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_36", this.Name, "Установка алгоритма Групп"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            DisableAllButtons();

                            for (int k = 0; k < lvList.Items.Count; k++)
                            {
                                if (lvList.Items[k].Checked)
                                {
                                    String sUID = lvList.Items[k].SubItems[1].Text;
                                    String sUName = lvList.Items[k].SubItems[0].Text;

                                    List<String> lstContHarCurrent = new List<String>();
                                    if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                                    {
                                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                                        lstContHarCurrent = new List<String>(srcFile);
                                        lstContHarCurrent[iAlgValHar] = lstContHarCurrent[iAlgValHar].Substring(0, lstContHarCurrent[iAlgValHar].IndexOf("|") + 1) + SelectedAlgorithm.Name;

                                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);
                                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + sUID + ".txt"), lstAlgorithmsDBRecordTS, Encoding.UTF8);

                                        GroupsList_AddToVisualList(sUID, sUName, k);
                                        GroupsList_Add(sUID, sUName);
                                    }

                                }
                            }

                            EnableAllButtons();
                        }
                    }

                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedIndices.Count <= 0)
            {
                if (lvList.SelectedIndices.Count > 0)
                {
                    int iSelIdx = lvList.SelectedIndices[0];
                    String sUID = lvList.Items[iSelIdx].SubItems[1].Text;

                    if (sUID.Length > 0)
                    {
                        StartWorkGroupsThread(sUID);
                    }
                }
            }
            else
            {
                if (lvList.CheckedItems.Count > 0)
                {
                    DisableAllButtons();

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (lvList.Items[k].Checked)
                        {
                            String sUID = lvList.Items[k].SubItems[1].Text;
                            //String sUName = lvList.Items[k].SubItems[0].Text;
                            StartWorkGroupsThread(sUID);
                        }
                    }

                    EnableAllButtons();
                }

            }
        }

        private void StartWorkGroupsThread(String sGID)
        {
            // This method runs on the main thread.
            this.pbProgress.Visible = true;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Value = 0;
            this.pbProgress.Maximum = 10;
            this.bDoImportGroups = false;

            DisableAllButtons();

            this.button3.Text = "Прервать";
            this.button3.Enabled = true;

            // Initialize the object that the background worker calls.
            WorkGroups WC = new WorkGroups(this, mFormMain);
            WC.sGroupID = sGID;
            // Start the asynchronous operation.
            backgroundWorker.RunWorkerAsync(WC);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;

            // Get the Words object and call the main method.
            WorkGroups WC = (WorkGroups)e.Argument;
            WC.doWork(worker, e);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkGroups.CurrentState state = (WorkGroups.CurrentState)e.UserState;
            if (this.pbProgress.Maximum != state.Total)
                this.pbProgress.Maximum = state.Total;

            this.pbProgress.Value = state.Worked;
            this.button3.Text = "Прервать" + " (" + state.Worked.ToString() + "/" + state.Total.ToString() + "/" + state.Deleted.ToString() + ")";

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.pbProgress.Visible = false;
            this.bDoImportGroups = true;
            this.button3.Text = "Удалить блокированных";

            EnableAllButtons();
        }

        private void tbInvertSelection_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            lvList.BeginUpdate();
            for (int k = 0; k < lvList.Items.Count; k++)
            {
                lvList.Items[k].Checked = !lvList.Items[k].Checked;
            }
            lvList.EndUpdate();
            Cursor = Cursors.Arrow;
        }

        private void buttonSelectGroup_Click(object sender, EventArgs e)
        {
            if (lvList.SelectedIndices.Count > 0)
            {
                int iSelIdx = lvList.SelectedIndices[0];
                String sUID = lvList.Items[iSelIdx].SubItems[1].Text;

                if (sUID.Length > 0)
                {
                    bNeedPersoneChange = true;
                    iSelGroupID = Convert.ToInt64(sUID);
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private String SelectContacters()
        {
            //---
            FormEditContactsDB fepdb = new FormEditContactsDB(mFormMain);
            fepdb.sContHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
            for (int i = 0; i < mFormMain.iContHarCount; i++)
            {
                for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                    fepdb.sContHar[i, j] = mFormMain.sContHar[i, j];
                fepdb.sContHar[i, mFormMain.iContHarAttrCount] = "";
            }
            fepdb.iContHarCount = mFormMain.iContHarCount;
            fepdb.iContHarAttrCount = mFormMain.iContHarAttrCount;
            fepdb.Text = "Выберите Контактеров для импорта их групп";
            fepdb.button3.Visible = false;
            fepdb.buttonSelectContacters.Visible = true;
            //fepdb.button1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_26", this.Name, "Отмена");
            fepdb.Setup(iPersUserID.ToString() + " (" + mFormMain.userLogin + ", " + mFormMain.userPassword + ")", iPersUserID, 0, false);
            if (fepdb.ShowDialog() != DialogResult.OK)
                return "";

            String sPersonenCheckedList = "";
            if (fepdb.gridCheckedCount <= 0)
            {
                if (fepdb.gridSelectedIndex > 0)
                {
                    int iSelIdx = fepdb.gridSelectedIndex;
                    sPersonenCheckedList = fepdb.getItemID(iSelIdx) + "|";
                }
            }
            else
            {
                foreach (int i in fepdb.gridCheckedIndexes)
                {
                    sPersonenCheckedList += fepdb.getItemID(i) + "|";
                }
            }
            //---
            return sPersonenCheckedList;
        }


        private void buttonImportContacterGroup_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
            {
                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 1);
                fecic.groupBox2.Visible = false;

                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;

                String sImportIDs = SelectContacters();
                if (sImportIDs.Length == 0)
                    return;

                iImportMode = 7;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
                for (int i = 0; i < iGroupHarCount; i++)
                {
                    for (int j = 0; j < iGroupHarAttrCount; j++)
                        fe.sPersHar[i, j] = sGroupHar[i, j];
                    fe.sPersHar[i, iGroupHarAttrCount] = "";
                }
                LoadFormEditPersHarValues(fe, iImportMode, 1);

                fe.iPersHarAttrCount = iGroupHarAttrCount;
                fe.iPersHarCount = iGroupHarCount;
                fe.sFilePrefix = "grp";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValues(fe, iImportMode, 1);
                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < iGroupHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);
                    List<String> lstFilterHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                    if (lstFilterHar != null)
                        StartImportGroupsThread("UserIDs=" + sImportIDs, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, true);
                }

            }
            else
            {
                this.buttonImportContacterGroup.Enabled = false;
                this.bwProgress.CancelAsync();
            }

        }

        private void tbImportAdminOfGroupsAsContacters_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                if (MessageBox.Show("Вы уверены, что хотите экспортировать администрацию из всех отмеченных Групп как Контактеров?", "Экспорт администрации групп как Контактеров", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DisableAllButtons();

                    List<long> usrGroupMembersIDs = new List<long>();
                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (lvList.Items[k].Checked)
                        {
                            String sUID = lvList.Items[k].SubItems[1].Text;

                            List<long> usrGroupMembersIDsFromGroup = getManagersFromGroup(sUID);
                            foreach (long usrID in usrGroupMembersIDsFromGroup)
                                if (!usrGroupMembersIDs.Contains(usrID))
                                    usrGroupMembersIDs.Add(usrID);
                        }
                    }

                    if (usrGroupMembersIDs.Count > 0)
                    {
                        saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                        saveFileDialog.FilterIndex = 0;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            List<String> lstUsersToExport = new List<string>();
                            foreach (long usrID in usrGroupMembersIDs)
                                lstUsersToExport.Add(usrID.ToString());

                            File.WriteAllLines(saveFileDialog.FileName, lstUsersToExport, Encoding.UTF8);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Список контактеров пуст!", "Экспорт администрации групп как Контактеров", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    EnableAllButtons();
                }
            }
        }

        private List<long> getManagersFromGroup(string groupID)
        {
            int totalCount = 0;
            var usrGroupMembersIDs = new List<long>();
            /*
            do
            {
                try
                {
                    VkNet.Model.Group group = FormMain.api.Groups.GetById(Convert.ToInt64(groupID), GroupsFields.Contacts);

                    if (group.Contacts != null)
                    {
                        foreach (VkNet.Model.Contact contact in group.Contacts)
                        {
                            if (contact.UserId.HasValue)
                                if (!usrGroupMembersIDs.Contains(contact.UserId.Value))
                                    usrGroupMembersIDs.Add(contact.UserId.Value);
                        }
                    }
                    break;
                }
                catch (Exception exp)
                {
                    mFormMain.ExceptionToLogList("getManagersFromGroup", "getManagersFromGroup", exp);
                    break;
                }
            }
            while (true);
            */
            return usrGroupMembersIDs;
        }

        private void timerClose_Tick(object sender, EventArgs e)
        {
            timerClose.Enabled = false;
            DialogResult = DialogResult.Cancel;
            Close();
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


        private void tbImportAsContactersFromGroupsAdmins_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
            {
                Dictionary<long, HashSet<long>> usersGroupIDs= new Dictionary<long, HashSet<long>>();
                List<long> usrGroupMembersIDs = new List<long>();
                for (int k = 0; k < lvList.Items.Count; k++)
                {
                    if (lvList.Items[k].Checked)
                    {
                        String sUID = lvList.Items[k].SubItems[1].Text;

                        long lCurGroupID = Convert.ToInt64(sUID);

                        List<long> usrGroupMembersIDsFromGroup = getManagersFromGroup(sUID);
                        foreach (long usrID in usrGroupMembersIDsFromGroup)
                        {
                            if (!usrGroupMembersIDs.Contains(usrID))
                                usrGroupMembersIDs.Add(usrID);

                            if (!usersGroupIDs.ContainsKey(usrID))
                                usersGroupIDs.Add(usrID, new HashSet<long>());
                            usersGroupIDs[usrID].Add(lCurGroupID);
                        }
                    }
                }

                if (usrGroupMembersIDs.Count == 0)
                {
                    MessageBox.Show("Список контактеров пуст!", "Экспорт администрации групп как Контактеров", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                String sImportIDs = "";
                foreach (long usrID in usrGroupMembersIDs)
                    sImportIDs += usrID.ToString() + "|";

                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);

                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                iImportMode = 8;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = mFormMain.sContHar[i, j];
                    fe.sPersHar[i, mFormMain.iContHarAttrCount] = "";
                }
                LoadFormEditPersHarValuesContacters(fe, iImportMode, 1);

                fe.iPersHarAttrCount = mFormMain.iContHarAttrCount;
                fe.iPersHarCount = mFormMain.iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValuesContacters(fe, iImportMode, 1);
                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < mFormMain.iContHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, mFormMain.iContHarAttrCount]);
                    List<String> lstFilterHar = GetHarValuesContacters(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                    if (lstFilterHar != null)
                        StartImportGroupsThread("UIDs=" + sImportIDs, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, false, usersGroupIDs, nudDuplicatesAction);
                }

            }
            else
            {
                this.tbImportAsContactersFromGroupsAdmins.Enabled = false;
                this.bwProgress.CancelAsync();
            }
        }

        private void tbImportAsContactersFromGroupsAllUsers_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
            {
                String sImportIDs = "";
                for (int k = 0; k < lvList.Items.Count; k++)
                {
                    if (lvList.Items[k].Checked)
                    {
                        String sUID = lvList.Items[k].SubItems[1].Text;
                        sImportIDs += sUID + "|";
                    }
                }

                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);
                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                iImportMode = 8;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = mFormMain.sContHar[i, j];
                    fe.sPersHar[i, mFormMain.iContHarAttrCount] = "";
                }
                LoadFormEditPersHarValuesContacters(fe, iImportMode, 1);

                fe.iPersHarAttrCount = mFormMain.iContHarAttrCount;
                fe.iPersHarCount = mFormMain.iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValuesContacters(fe, iImportMode, 1);
                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < mFormMain.iContHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, mFormMain.iContHarAttrCount]);
                    List<String> lstFilterHar = GetHarValuesContacters(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                    if (lstFilterHar != null)
                        StartImportGroupsThread("GroupsIDs=" + sImportIDs, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, false, null, nudDuplicatesAction);
                }

            }
            else
            {
                this.tbImportAsContactersFromGroupsAdmins.Enabled = false;
                this.bwProgress.CancelAsync();
            }
        }

        private void tbImportAsContactersFromGroupsAuthors100_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
            {
                String sImportIDs = "";
                for (int k = 0; k < lvList.Items.Count; k++)
                {
                    if (lvList.Items[k].Checked)
                    {
                        String sUID = lvList.Items[k].SubItems[1].Text;
                        sImportIDs += sUID + "|";
                    }
                }

                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);
                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                iImportMode = 8;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = mFormMain.sContHar[i, j];
                    fe.sPersHar[i, mFormMain.iContHarAttrCount] = "";
                }
                LoadFormEditPersHarValuesContacters(fe, iImportMode, 1);

                fe.iPersHarAttrCount = mFormMain.iContHarAttrCount;
                fe.iPersHarCount = mFormMain.iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValuesContacters(fe, iImportMode, 1);
                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < mFormMain.iContHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, mFormMain.iContHarAttrCount]);
                    List<String> lstFilterHar = GetHarValuesContacters(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                    if (lstFilterHar != null)
                        StartImportGroupsThread("GrWallIDs=" + sImportIDs, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, false, null, nudDuplicatesAction);
                }

            }
            else
            {
                this.tbImportAsContactersFromGroupsAuthors100.Enabled = false;
                this.bwProgress.CancelAsync();
            }
        }

        private void tbImportAsContactersFromGroupsAuthorsAndComments_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
            {
                String sImportIDs = "";
                for (int k = 0; k < lvList.Items.Count; k++)
                {
                    if (lvList.Items[k].Checked)
                    {
                        String sUID = lvList.Items[k].SubItems[1].Text;
                        sImportIDs += sUID + "|";
                    }
                }

                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);
                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                iImportMode = 8;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = mFormMain.sContHar[i, j];
                    fe.sPersHar[i, mFormMain.iContHarAttrCount] = "";
                }
                LoadFormEditPersHarValuesContacters(fe, iImportMode, 1);

                fe.iPersHarAttrCount = mFormMain.iContHarAttrCount;
                fe.iPersHarCount = mFormMain.iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValuesContacters(fe, iImportMode, 1);
                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < mFormMain.iContHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, mFormMain.iContHarAttrCount]);
                    List<String> lstFilterHar = GetHarValuesContacters(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                    if (lstFilterHar != null)
                        StartImportGroupsThread("GrWCmtIDs=" + sImportIDs, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, false, null, nudDuplicatesAction);
                }

            }
            else
            {
                this.tbImportAsContactersFromGroupsAuthorsAndComments.Enabled = false;
                this.bwProgress.CancelAsync();
            }
        }

        private void tbImportAsContactersFromGroupsComments_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
            {
                String sImportIDs = "";
                for (int k = 0; k < lvList.Items.Count; k++)
                {
                    if (lvList.Items[k].Checked)
                    {
                        String sUID = lvList.Items[k].SubItems[1].Text;
                        sImportIDs += sUID + "|";
                    }
                }

                String sPersUIDs = SelectPersonen();
                if (sPersUIDs.Length == 0)
                    return;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDs.Split('|').Length - 1, 0);
                if (fecic.ShowDialog() != DialogResult.OK)
                    return;
                int nudContacterCount = (int)fecic.nudContacterCount.Value;
                int nudDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

                iImportMode = 8;
                //string value = "";
                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = mFormMain.sContHar[i, j];
                    fe.sPersHar[i, mFormMain.iContHarAttrCount] = "";
                }
                LoadFormEditPersHarValuesContacters(fe, iImportMode, 1);

                fe.iPersHarAttrCount = mFormMain.iContHarAttrCount;
                fe.iPersHarCount = mFormMain.iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    SaveFormEditPersHarValuesContacters(fe, iImportMode, 1);
                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < mFormMain.iContHarCount; i++)
                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, mFormMain.iContHarAttrCount]);
                    List<String> lstFilterHar = GetHarValuesContacters(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Фильтр значений характеристик"), iImportMode);
                    if (lstFilterHar != null)
                        StartImportGroupsThread("GrCmtsIDs=" + sImportIDs, lstContHar, lstFilterHar, sPersUIDs, nudContacterCount, false, null, nudDuplicatesAction);
                }

            }
            else
            {
                this.tbImportAsContactersFromGroupsComments.Enabled = false;
                this.bwProgress.CancelAsync();
            }
        }
    }
}
