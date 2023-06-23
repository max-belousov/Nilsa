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
using VkNet;
using System.Threading;
using System.Diagnostics;
using Nilsa.Data;
using System.Security.Cryptography;
using VkNet.Model;
using Nilsa.NilsaAndInterface;
using Newtonsoft.Json;
using Nilsa.TinderAssistent;

namespace Nilsa
{
    public partial class FormEditPersonenDB : Form
    {
        FormMain mFormMain;
        public String[,] sContHar;
        public int iContHarCount = 16;
        public int iContHarAttrCount = 4;
        List<String> lstErrorsLogList;
        String slstErrorsInit;
        long iPersUserID;
        List<String> lstContHarValues;
        List<DBPerson> lstPersonenList;

        public const int USERFIELD_ID = 0;
        public const int USERFIELD_NAME = 1;
        public const int USERFIELD_LOGIN = 2;
        public const int USERFIELD_PASSWORD = 3;

        bool bDoImportContacts;
        int iImportMode;
        bool bShowMode;

        public Boolean bNeedPersoneChange, bNeedPersoneReread;
        public String suSelLogin, suSelPwd, suSelID;
        public int suSelSocialNetwork = 3;

        private string sUnknownAge;

        public FormEditPersonenDB(FormMain _formmain)
        {
            Application.EnableVisualStyles();
            mFormMain = _formmain;
            InitializeComponent();
            //button11.Visible = false;

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
            this.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Редактирование базы Персонажей");
            sUnknownAge = mFormMain.sDBDataItemsStrings_AgeUnknown;

            //this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;

            //var pos = this.PointToScreen(labelProgress.Location);
            //pos = pbProgress.PointToClient(pos);
            //labelProgress.Parent = pbProgress;
            //labelProgress.Location = pos;
            //labelProgress.BackColor = Color.Transparent;
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

        private void updateShowModeImage()
        {
            if (bShowMode)
            {
                tbShowMode.Image = Nilsa.Properties.Resources.nilsa_pers;
                tbShowMode.Text = "Все социальные сети";
            }
            else
            {
                switch (FormMain.SocialNetwork)
                {
                    case 0:
                        tbShowMode.Image = Nilsa.Properties.Resources.social_vkontakte;
                        tbShowMode.Text = "VKontakte";
                        break;
                    case 1:
                        tbShowMode.Image = Nilsa.Properties.Resources.social_nilsa;
                        tbShowMode.Text = "NILSA";
                        break;
                    case 2:
                        tbShowMode.Image = Nilsa.Properties.Resources.social_facebook;
                        tbShowMode.Text = "Facebook";
                        break;
                    case 3:
                        tbShowMode.Image = Nilsa.Properties.Resources.social_tinder;
                        tbShowMode.Text = "Tinder";
                        break;
                }
            }
        }

        public void Setup(String sErrorHdr, long iUsrID, bool _bShowMode = false, string _userlogin = "", string _userpwd = "")
        {
            bool bSearchLoginPwd = _userlogin.Length > 0 && _userpwd.Length > 0;
            bShowMode = _bShowMode;
            updateShowModeImage();

            toolStripDropDownButton2.Enabled = mFormMain.iContactsGroupsMode == 0;
            bDoImportContacts = true;
            bNeedPersoneChange = false;
            bNeedPersoneReread = false;
            slstErrorsInit = sErrorHdr;
            iPersUserID = iUsrID;
            LoadErrorsLogList();
            PersonenList_Load();

            button17.Enabled = true;
            button15.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button12.Enabled = false;
            //button11.Enabled = false;
            button23.Enabled = false;
            buttonCopyParameters.Enabled = false;
            buttonPasteParameters.Enabled = false;
            toolStripButtonCopyToClipboard.Enabled = false;


            Cursor = Cursors.WaitCursor;

            lvList.SelectedIndices.Clear();
            lvList.BeginUpdate();
            lvList.Items.Clear();

            if (iContHarCount <= 16)
            {
                for (int i = 0; i < iContHarCount; i++)
                    lvList.Columns[i + 4].Text = sContHar[i, 1];
            }

            for (int i = 0; i < lstPersonenList.Count; i++)
            {
                PersonenList_AddUserToVisualList(lstPersonenList[i]);
            }
            lvList.EndUpdate();
            if (NilsaUtils.LoadLongValue(5, 0) == 1)
                ApplyFilter();
            else
                button4.Image = Nilsa.Properties.Resources.filter_list_on;

            int iSelIdx = -1;
            for (int i = 0; i < lvList.Items.Count; i++)
            {
                String sUID = lvList.Items[i].SubItems[1].Text;
                if (iUsrID == Convert.ToInt64(sUID))
                    iSelIdx = i;
                if (bSearchLoginPwd)
                {
                    String sULogin = lvList.Items[i].SubItems[2].Text;
                    String sUPwd = lvList.Items[i].SubItems[3].Text;
                    if (sULogin.Equals(_userlogin) && sUPwd.Equals(_userpwd))
                        iSelIdx = i;
                }
            }

            LoadColumnsOrderAndWidth();

            Cursor = Cursors.Arrow;
            if (lvList.Items.Count > 0)
            {
                if (iSelIdx >= 0)
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
            fe.sFilePrefix = "pers";
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
                FilterList(RQV);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text.Equals(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Отключить фильтр")))
            {
                FilterList(ClearFilter());
                NilsaUtils.SaveLongValue(5, 0);
            }
            else
            {
                ApplyFilter(false);
                NilsaUtils.SaveLongValue(5, 1);
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

        private void FilterList(String[] RQV)
        {
            //---
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            //button11.Enabled = false;
            button23.Enabled = false;
            buttonCopyParameters.Enabled = false;
            buttonPasteParameters.Enabled = false;
            toolStripButtonCopyToClipboard.Enabled = false;

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
            bool bFiltered = false;
            for (int i = 0; i < lstPersonenList.Count; i++)
            {
                Boolean bEquals = bRQVEmpty;
                if (!bEquals)
                {
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(lstPersonenList[i].SocialNetwork) + lstPersonenList[i].UserID + ".txt")))
                    {
                        //List<String> lstContHarValues = new List<String>();
                        //var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(lstPersonenList[i].SocialNetwork) + lstPersonenList[i].UserID + ".txt"));
                        //lstContHarValues = new List<String>(srcFile);
                        //String[] EQV = new String[iContHarCount];
                        //foreach (String str in lstContHarValues)
                        //    EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1).ToLower();
                        String[] EQV = loadPersoneDataFile(lstPersonenList[i].SocialNetwork, lstPersonenList[i].UserID);
                        for (int iv = 0; iv < EQV.Length; iv++)
                            EQV[iv] = EQV[iv].ToLower();

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
                    PersonenList_AddUserToVisualList(lstPersonenList[i]);
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
            //---
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

        private String[] loadPersoneDataFile(int _SocialNetwork, String _UserID)
        {
            String[] EQV = new String[iContHarCount];

            for (int ii = 0; ii < EQV.Length; ii++)
                EQV[ii] = "";

            if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(_SocialNetwork) + _UserID + ".txt")))
            {
                List<String> lstContHarValues = new List<String>();
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(_SocialNetwork) + _UserID + ".txt"));
                lstContHarValues = new List<String>(srcFile);

                if (lstContHarValues != null)
                    foreach (String str in lstContHarValues)
                        if (str.Contains("|"))
                        {
                            int haridx = -1;
                            try
                            {
                                haridx = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1;
                            }
                            catch
                            {
                                haridx = -1;
                            }

                            if (haridx >= 0 && haridx < EQV.Length)
                                EQV[haridx] = str.Substring(str.IndexOf("|") + 1);
                        }
            }
            return EQV;
        }
        private int PersonenList_AddUserToVisualList(DBPerson dbPerson, int iVLIdx = -1)
        {
            //String[] EQV = new String[iContHarCount];
            //for (int ii = 0; ii < EQV.Length; ii++)
            //    EQV[ii] = "";
            //if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(dbPerson.SocialNetwork) + dbPerson.UserID + ".txt")))
            //{
            //    List<String> lstContHarValues = new List<String>();
            //    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(dbPerson.SocialNetwork) + dbPerson.UserID + ".txt"));
            //    lstContHarValues = new List<String>(srcFile);
            //    foreach (String str in lstContHarValues)
            //        if (str.Contains("|"))
            //        {
            //            int haridx = -1;
            //            try
            //            {
            //                haridx = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1;
            //            }
            //            catch
            //            {
            //                haridx = -1;
            //            }

            //            if (haridx >= 0 && haridx < EQV.Length)
            //                EQV[haridx] = str.Substring(str.IndexOf("|") + 1);
            //        }
            //}
            String[] EQV = loadPersoneDataFile(dbPerson.SocialNetwork, dbPerson.UserID);

            ListViewItem lvi;
            if (iVLIdx >= 0)
            {
                lvi = lvList.Items[iVLIdx];
                lvi.ImageIndex = dbPerson.SocialNetwork;
                lvi.Text = dbPerson.UserName;
                lvi.SubItems[1].Text = dbPerson.UserID;
                lvi.SubItems[2].Text = dbPerson.UserLogin;
                lvi.SubItems[3].Text = dbPerson.UserPassword;

                if (iContHarCount <= 16)
                    for (int iOffs = 0; iOffs < iContHarCount; iOffs++)
                        lvi.SubItems[4 + iOffs].Text = EQV[iOffs];
            }
            else
            {
                lvi = new ListViewItem(dbPerson.UserName);
                lvi.ImageIndex = dbPerson.SocialNetwork;
                lvi.SubItems.Add(dbPerson.UserID);
                lvi.SubItems.Add(dbPerson.UserLogin);
                lvi.SubItems.Add(dbPerson.UserPassword);
                if (iContHarCount <= 16)
                    for (int iOffs = 0; iOffs < iContHarCount; iOffs++)
                        lvi.SubItems.Add(EQV[iOffs]);
                lvList.Items.Add(lvi);
            }

            return lvi.Index;
        }

        private void PersonenList_Load()
        {
            lstPersonenList = new List<DBPerson>();
            if (bShowMode)
            {
                for (int iSNIDX = 0; iSNIDX <= 2; iSNIDX++)
                {
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix(iSNIDX) + ".txt")))
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix(iSNIDX) + ".txt"));
                        foreach (string str in srcFile)
                        {
                            if (str == null)
                                continue;

                            if (str.Length == 0)
                                continue;

                            lstPersonenList.Add(DBPerson.FromRecordString(iSNIDX, str));
                        }
                        //lstPersonenList = new List<String>(srcFile);
                    }
                }
            }
            else
            {
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix() + ".txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix() + ".txt"));
                    foreach (string str in srcFile)
                    {
                        if (str == null)
                            continue;

                        if (str.Length == 0)
                            continue;

                        lstPersonenList.Add(DBPerson.FromRecordString(FormMain.SocialNetwork, str));
                    }
                    //lstPersonenList = new List<String>(srcFile);
                }
            }
        }

        private void PersonenList_RemoveUser(int iSocialNetwork, String sUD)
        {
            int iuserIdx = PersonenList_GetUserIdx(iSocialNetwork, sUD);
            if (iuserIdx >= 0)
            {
                lstPersonenList.RemoveAt(iuserIdx);
                if (lstPersonenList.Count > 0)
                    PersonenList_Store(iSocialNetwork);
                else
                    File.Delete(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_dialogsinitperday_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_dialogsinitperday_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + "_contacter" + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + "_contacter" + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + "_groups" + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + "_groups" + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_outgoing_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_msg_outgoing_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_outdelayed_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_msg_outdelayed_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt"));

                string[] files = System.IO.Directory.GetFiles(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + "_*.txt");
                foreach (string f in files)
                    File.Delete(Path.Combine(FormMain.sDataPath, f));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_messagesdb_disabled_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_messagesdb_disabled_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt"));

                files = System.IO.Directory.GetFiles(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + "_*.txt");
                foreach (string f in files)
                    File.Delete(Path.Combine(FormMain.sDataPath, f));

                //if (File.Exists(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + sUD + ".txt")))
                //    File.Delete(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_initdialog_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_initdialog_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + ".txt"));

                files = System.IO.Directory.GetFiles(FormMain.sDataPath, "_lastmessage_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + "_*.txt");
                foreach (string f in files)
                    File.Delete(Path.Combine(FormMain.sDataPath, f));

                files = System.IO.Directory.GetFiles(FormMain.sDataPath, "_prevlastmessage_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + "_*.txt");
                foreach (string f in files)
                    File.Delete(Path.Combine(FormMain.sDataPath, f));

                files = System.IO.Directory.GetFiles(FormMain.sDataPath, "_flag_init_dialog_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUD + "_*.txt");
                foreach (string f in files)
                    File.Delete(Path.Combine(FormMain.sDataPath, f));

            }
        }

        private int PersonenList_GetUserIdx(int iSocialNerwork, String sUD)
        {
            int iuserIdx = -1;
            for (int i = 0; i < lstPersonenList.Count; i++)
            {
                //String str = lstPersonenList[i];
                if (lstPersonenList[i].SocialNetwork == iSocialNerwork && lstPersonenList[i].UserID.Equals(sUD))
                {
                    iuserIdx = i;
                    break;
                }
            }
            return iuserIdx;
        }

        private String PersonenList_GetUserRecord(int iSocialNerwork, String sUD)
        {
            int idx = PersonenList_GetUserIdx(iSocialNerwork, sUD);

            return idx >= 0 ? lstPersonenList[idx].ToRecordString() : "";
        }

        public String PersonenList_GetUserField(int iSocialNerwork, String sUD, int iFieldIdx) // 0 - usrID, 1 - usrName, 2 - usrLogin, 3 - usrPwd
        {
            String retval = PersonenList_GetUserRecord(iSocialNerwork, sUD);
            if (retval.Length > 0)
            {
                for (int i = 0; i < iFieldIdx; i++)
                    retval = retval.Substring(retval.IndexOf("|") + 1);
                if (iFieldIdx < 3)
                    retval = retval.Substring(0, retval.IndexOf("|"));
            }
            return retval;
        }

        private void PersonenList_Store(int iSocialNetwork)
        {
            List<string> lstList = new List<string>();
            foreach (DBPerson dbPerson in lstPersonenList)
                if (dbPerson.SocialNetwork == iSocialNetwork)
                    lstList.Add(dbPerson.ToRecordString());

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + ".txt"), lstList, Encoding.UTF8);
        }

        public DBPerson PersonenList_AddUser(int iSocialNetworkID, String sUD, String sUName, String sULogin, String sUPwd)
        {
            int iuserIdx = PersonenList_GetUserIdx(iSocialNetworkID, sUD);
            DBPerson dbPerson = new DBPerson(iSocialNetworkID, sUD, sUName, sULogin, sUPwd);
            if (iuserIdx >= 0)
                lstPersonenList[iuserIdx] = dbPerson;
            else
                lstPersonenList.Add(dbPerson);

            PersonenList_Store(iSocialNetworkID);
            return dbPerson;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedIndices.Count <= 0)
            {
                if (lvList.SelectedIndices.Count > 0)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Вы уверены, что хотите удалить этого Персонажа?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Персонажа"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bool bCont = true;
                        int iSelIdx = lvList.SelectedIndices[0];
                        String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                        int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;

                        if (iSocialNetwork == 1 && sUID.Equals("0"))
                        {
                            MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "Персонаж NILSA не может быть удален."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        if (sUID.Equals(iPersUserID.ToString()) && iSocialNetwork == FormMain.SocialNetwork)
                        {
                            bCont = false;
                            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_7", this.Name, "Этот Персонаж сейчас активен, при его удалении Вам придется сменить Персонаж после выхода из редактирования базы персонажей.\n\nВы уверены, что хотите удалить активного Персонажа?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Удаление Персонажа"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                bCont = true;
                        }

                        if (bCont)
                        {
                            if (sUID.Length > 0)
                            {
                                lvList.SelectedIndices.Clear();
                                lvList.Items.RemoveAt(iSelIdx);
                                PersonenList_RemoveUser(iSocialNetwork, sUID);

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
                                    button23.Enabled = false;

                                    button3.Enabled = false;
                                    button5.Enabled = false;
                                    button7.Enabled = false;
                                    button8.Enabled = false;
                                    //button11.Enabled = false;
                                    buttonCopyParameters.Enabled = false;
                                    buttonPasteParameters.Enabled = false;
                                    toolStripButtonCopyToClipboard.Enabled = false;
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
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Вы уверены, что хотите удалить всех отмеченных Персонажей?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_8", this.Name, "Удаление Персонажей"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();
                        lvList.BeginUpdate();

                        foreach (ListViewItem item in lvList.CheckedItems)
                        {
                            String sUID = item.SubItems[1].Text;
                            int iSocialNetwork = item.ImageIndex;
                            if (!(iSocialNetwork == 1 && sUID.Equals("0")))
                                PersonenList_RemoveUser(iSocialNetwork, sUID);
                        }

                        foreach (ListViewItem item in lvList.CheckedItems)
                        {
                            String sUID = item.SubItems[1].Text;
                            int iSocialNetwork = item.ImageIndex;
                            if (!(iSocialNetwork == 1 && sUID.Equals("0")))
                                lvList.Items.Remove(item);
                        }

                        lvList.EndUpdate();
                        EnableAllButtons();
                        FilterList(ClearFilter());
                    }
                }
            }

        }

        private void DisableAllButtons()
        {
            //this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button23.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            this.button6.Enabled = false;
            this.button7.Enabled = false;
            this.button8.Enabled = false;
            this.button9.Enabled = false;
            this.button10.Enabled = false;
            this.button12.Enabled = false;
            this.button11.Enabled = false;
            this.button13.Enabled = false;
            this.button14.Enabled = false;
            this.button20.Enabled = false;
            this.button15.Enabled = false;
            this.button16.Enabled = false;
            this.button19.Enabled = false;
            this.button18.Enabled = false;
            this.button17.Enabled = false;
            buttonCopyParameters.Enabled = false;
            buttonPasteParameters.Enabled = false;
            this.toolStripButtonCheckPersonenActivityAndCopyContacters.Enabled = false;
            this.buttonOpenPersoneWebPage.Enabled = false;
            tbJoinCommunity.Enabled = false;
            buttonExportPersonenServer.Enabled = false;
            buttonExportPersonenAsContactersServer.Enabled = false;
            tbLeaveCommunity.Enabled = false;
            tbRepostGroup.Enabled = false;
            tbRepostWall.Enabled = false;
            tbFriendsAdd.Enabled = false;
            tbFriendsDelete.Enabled = false;
            tbLikeGroup.Enabled = false;
            tbLikeWall.Enabled = false;
            toolStripButtonCopyToClipboard.Enabled = false;
        }

        private void EnableAllButtons()
        {
            //this.button1.Enabled = true;
            this.button2.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            this.button23.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            this.button3.Enabled = lvList.SelectedItems.Count > 0;
            this.button4.Enabled = true;
            this.button5.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            this.button6.Enabled = true;
            this.button7.Enabled = lvList.SelectedItems.Count > 0;
            this.button8.Enabled = lvList.SelectedItems.Count > 0;
            this.button9.Enabled = true;
            this.button11.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            this.button17.Enabled = true;
            this.button15.Enabled = true;
            toolStripButtonCopyToClipboard.Enabled = lvList.SelectedItems.Count > 0;
            buttonCopyParameters.Enabled = lvList.SelectedItems.Count > 0;
            buttonPasteParameters.Enabled = copyedParameters != null && (lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0);
            lvList_SelectedIndexChanged(null, null);
            lvList_ItemChecked(null, null);

        }

        private void setListCounter()
        {
            groupBox1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_10", this.Name, "Персонажи") + " (" + (lvList.CheckedItems.Count > 0 ? lvList.CheckedItems.Count.ToString() + " / " : "") + lvList.Items.Count.ToString() + ")";
        }

        private void SaveFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            for (int i = 0; i < iContHarCount; i++)
                lstContHar.Add(fe.sPersHar[i, iContHarAttrCount]);

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesPersonen_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"), lstContHar, Encoding.UTF8);
        }

        private void LoadFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesPersonen_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesPersonen_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"));
                lstContHar = new List<String>(srcFile);
            }

            if (lstContHar.Count > 0)
            {
                for (int i = 0; i < iContHarCount; i++)
                {
                    fe.sPersHar[i, iContHarAttrCount] = lstContHar[i];
                }
            }
        }

        private String ResolveID(String text)
        {
            //String shortname = "";
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
                    //do
                    //{
                    //    try
                    //    {
                    //        VkNet.Model.VkObject obj = FormMain.api.Utils.ResolveScreenName(text);
                    //        if (obj.Id.HasValue)
                    //            return obj.Id.Value.ToString();
                    //        break;
                    //    }
                    //    catch (VkNet.Exception.AccessTokenInvalidException)
                    //    {
                    //        if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                    //            break;
                    //    }
                    //    catch (System.Net.WebException)
                    //    {
                    //        if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                    //            break;
                    //    }
                    //    catch (VkNet.Exception.VkApiException vkapexeption)
                    //    {
                    //        if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                    //            break;
                    //    }
                    //    catch (Exception exp)
                    //    {
                    //        mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                    //        break;
                    //    }
                    //}
                    //while (true);
                }
            }
            return "-1";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormEnterLogin formEnterLogin = new FormEnterLogin();

            if (formEnterLogin.ShowDialog() == DialogResult.OK)
            {
                String sULogin = formEnterLogin.tbLogin.Text;
                String sUPwd = formEnterLogin.tbPassword.Text;
                int iSocialNetwork = formEnterLogin.cbSocialNetwork.SelectedIndex;

                if (iSocialNetwork == 1)
                {
                    String srec = NILSA_getUserRecord(sULogin, sUPwd);

                    if (srec != null)
                    {
                        String sUID = NILSA_GetFieldFromStringRec(srec, 0);
                        String sUName = NILSA_GetFieldFromStringRec(srec, 1);
                        if (PersonenList_GetUserIdx(iSocialNetwork, sUID) >= 0)
                        {
                            MessageBox.Show("Персонаж с указанными данными уже есть в базе. Если он не отображается, смените настройки фильтра базы персонажей по характеристикам...", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd));

                            lvList.SelectedIndices.Clear();
                            if (lvList.Items.Count > 0)
                            {
                                lvList.SelectedIndices.Add(lvList.Items.Count - 1);
                                lvList.EnsureVisible(lvList.Items.Count - 1);
                            }
                            lvList_ItemChecked(null, null);
                        }
                    }
                    else
                        MessageBox.Show("Ошибка запроса Персонажа с указанными данными...", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (iSocialNetwork == 0)
                {
                    long uid = AutorizeVK(sULogin, sUPwd);
                    if (uid >= 0)
                    {
                        String sUID = uid.ToString();

                        if (PersonenList_GetUserIdx(iSocialNetwork, sUID) >= 0)
                        {
                            MessageBox.Show("Персонаж с указанными данными уже есть в базе. Если он не отображается, смените настройки фильтра базы персонажей по характеристикам...", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
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
                            fe.sFilePrefix = "pers";
                            fe.Setup();

                            if (fe.ShowDialog() == DialogResult.OK)
                            {
                                SaveFormEditPersHarValues(fe, 100000, 1);
                                List<String> lstContHar = new List<String>();
                                for (int i = 0; i < iContHarCount; i++)
                                    lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);

                                mFormMain.fwbVKontakte.Setup(sULogin, sUPwd, WebBrowserCommand.GetPersoneAttributes, uid);
                                mFormMain.fwbVKontakte.WaitResult();
                                FormWebBrowser.Persone usrAdr = mFormMain.fwbVKontakte.personeAtrributes;

                                //---
                                String sUName = usrAdr.FirstName + " " + usrAdr.LastName;

                                List<String> lstContHarValues = new List<String>();

                                if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
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
                                                String sexValue = lstContHarValues[i].Substring(lstContHar[i].IndexOf("|") + 1);
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

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarValues, Encoding.UTF8);

                                PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd));

                                lvList.SelectedIndices.Clear();
                                if (lvList.Items.Count > 0)
                                {
                                    lvList.SelectedIndices.Add(lvList.Items.Count - 1);
                                    lvList.EnsureVisible(lvList.Items.Count - 1);
                                }
                                lvList_ItemChecked(null, null);
                                //---
                                /*
                                do
                                {
                                    try
                                    {
                                        VkNet.Model.User usrAdr = FormMain.api.Users.Get(uid, ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.City | ProfileFields.Sex | ProfileFields.BirthDate | ProfileFields.Country | ProfileFields.Relation | ProfileFields.Online | ProfileFields.Counters | ProfileFields.LastSeen);
                                        String sUName = usrAdr.FirstName + " " + usrAdr.LastName;

                                        List<String> lstContHarValues = new List<String>();

                                        if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                                        {
                                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                                            lstContHarValues = new List<String>(srcFile);
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

                                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarValues, Encoding.UTF8);

                                        PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd));

                                        lvList.SelectedIndices.Clear();
                                        if (lvList.Items.Count > 0)
                                        {
                                            lvList.SelectedIndices.Add(lvList.Items.Count - 1);
                                            lvList.EnsureVisible(lvList.Items.Count - 1);
                                        }
                                        lvList_ItemChecked(null, null);
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
                                    catch (VkNet.Exception.VkApiException vkapexeption)
                                    {
                                        if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
                                            break;
                                    }
                                    catch (Exception exp)
                                    {
                                        ExceptionToLogList("FormEditPersonenDB.button6_Click", sUID, exp);
                                        MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_14", this.Name, "Ошибка запроса Персонажа с указанным ID..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        break;
                                    }
                                    finally { }
                                }
                                while (true);
                                */
                            }
                        }
                    }
                    else
                        MessageBox.Show("Ошибка запроса Персонажа с указанными данными...", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (FormMain.SocialNetwork == 0 && iPersUserID >= 0)
                        AutorizeVK(mFormMain.userLogin, mFormMain.userPassword);
                }
                else if (iSocialNetwork == 3)
                {
                    //доработать, сделано первично
                    String srec = Tinder_getUserRecord(sULogin, sUPwd);

                    if (srec != null)
                    {
                        String sUID = NILSA_GetFieldFromStringRec(srec, 0);
                        String sUName = NILSA_GetFieldFromStringRec(srec, 1);
                        if (PersonenList_GetUserIdx(iSocialNetwork, sUID) >= 0)
                        {
                            MessageBox.Show("Персонаж с указанными данными уже есть в базе. Если он не отображается, смените настройки фильтра базы персонажей по характеристикам...", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            sUID = lstTinder_UserDB.Count().ToString();
                            PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd));

                            lvList.SelectedIndices.Clear();
                            if (lvList.Items.Count > 0)
                            {
                                lvList.SelectedIndices.Add(lvList.Items.Count - 1);
                                lvList.EnsureVisible(lvList.Items.Count - 1);
                            }
                            lvList_ItemChecked(null, null);
                            lstTinder_UserDB.Add(lstPersonenList[lstPersonenList.Count() - 1].ToRecordString());
                        }
                    }
                    else
                        MessageBox.Show("Ошибка запроса Персонажа с указанными данными...", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Tinder_SaveUserDB();
                }
            }

            //string value = "";
            //if (FormMain.InputBox(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_12", this.Name, "Введите ID Персонажа:"), ref value) == DialogResult.OK)
            //{
            //    String sUID = ResolveID(value);
            //    if (PersonenList_GetUserIdx(FormMain.SocialNetwork, sUID) >= 0)
            //    {
            //        MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_13", this.Name, "Персонаж с указанным ID уже есть в базе. Если он не отображается, смените настройки фильтра базы персонажей по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //    else
            //    {
            //        if (FormMain.SocialNetwork == 1)
            //        {
            //            String srec = NILSA_getUserRecord(sUID);
            //            if (srec != null)
            //            {
            //                String sUName = NILSA_GetFieldFromStringRec(srec, 1);
            //                String sULogin = NILSA_GetFieldFromStringRec(srec, 2);
            //                String sUPwd = NILSA_GetFieldFromStringRec(srec, 3);

            //                PersonenList_AddUserToVisualList(PersonenList_AddUser(FormMain.SocialNetwork, sUID, sUName, sULogin, sUPwd));

            //                lvList.SelectedIndices.Clear();
            //                if (lvList.Items.Count > 0)
            //                {
            //                    lvList.SelectedIndices.Add(lvList.Items.Count - 1);
            //                    lvList.EnsureVisible(lvList.Items.Count - 1);
            //                }
            //                lvList_ItemChecked(null, null);
            //            }
            //            else
            //                MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_14", this.Name, "Ошибка запроса Персонажа с указанным ID..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else if (FormMain.SocialNetwork == 0)
            //        {
            //            if (iPersUserID >= 0)
            //            {
            //                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            //                fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
            //                for (int i = 0; i < iContHarCount; i++)
            //                {
            //                    for (int j = 0; j < iContHarAttrCount; j++)
            //                        fe.sPersHar[i, j] = sContHar[i, j];
            //                    fe.sPersHar[i, iContHarAttrCount] = "";
            //                }
            //                LoadFormEditPersHarValues(fe, 100000, 1);

            //                fe.iPersHarAttrCount = iContHarAttrCount;
            //                fe.iPersHarCount = iContHarCount;
            //                fe.sFilePrefix = "pers";
            //                fe.Setup();

            //                if (fe.ShowDialog() == DialogResult.OK)
            //                {
            //                    SaveFormEditPersHarValues(fe, 100000, 1);
            //                    List<String> lstContHar = new List<String>();
            //                    for (int i = 0; i < iContHarCount; i++)
            //                        lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);

            //                    do
            //                    {
            //                        try
            //                        {
            //                            VkNet.Model.User usrAdr = FormMain.api.Users.Get(Convert.ToInt64(sUID), ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.City | ProfileFields.Sex | ProfileFields.BirthDate | ProfileFields.Country | ProfileFields.Relation | ProfileFields.Online | ProfileFields.Counters | ProfileFields.LastSeen);
            //                            String sUName = usrAdr.FirstName + " " + usrAdr.LastName;

            //                            List<String> lstContHarValues = new List<String>();

            //                            if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt")))
            //                            {
            //                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt"));
            //                                lstContHarValues = new List<String>(srcFile);
            //                                for (int i = 0; i < iContHarCount; i++)
            //                                {
            //                                    String sCV = lstContHar[i].Substring(lstContHar[i].IndexOf("|") + 1);
            //                                    if (sCV.Length > 0)
            //                                        lstContHarValues[i] = lstContHar[i];
            //                                }
            //                            }
            //                            else
            //                            {
            //                                for (int i = 0; i < iContHarCount; i++)
            //                                    lstContHarValues.Add(lstContHar[i]);
            //                            }

            //                            for (int i = 0; i < iContHarCount; i++)
            //                            {
            //                                if (lstContHarValues[i].IndexOf("#sex#") > 0)
            //                                {
            //                                    String svkv = usrAdr.Sex != null ? (usrAdr.Sex == Sex.Male ? "Мужской" : (usrAdr.Sex == Sex.Female ? "Женский" : "Не указан")) : "Не указан";
            //                                    lstContHarValues[i] = lstContHarValues[i].Replace("#sex#", svkv);
            //                                }
            //                                else if (lstContHarValues[i].IndexOf("#city#") > 0)
            //                                {
            //                                    lstContHarValues[i] = lstContHarValues[i].Replace("#city#", usrAdr.City != null ? usrAdr.City.Title : "");
            //                                }
            //                                else if (lstContHarValues[i].IndexOf("#country#") > 0)
            //                                {
            //                                    lstContHarValues[i] = lstContHarValues[i].Replace("#country#", usrAdr.Country != null ? usrAdr.Country.Title : "");
            //                                }
            //                                else if (lstContHarValues[i].IndexOf("#relation#") > 0)
            //                                {
            //                                    lstContHarValues[i] = lstContHarValues[i].Replace("#relation#", usrAdr.Relation != null ? (usrAdr.Relation == RelationType.Amorous ? "Влюблен(-а)" : (usrAdr.Relation == RelationType.Engaged ? "Помолвлен(-а)" : (usrAdr.Relation == RelationType.HasFriend ? "Есть друг (подруга)" : (usrAdr.Relation == RelationType.InActiveSearch ? "В активном поиске" : (usrAdr.Relation == RelationType.ItsComplex ? "Все сложно" : (usrAdr.Relation == RelationType.Married ? "Женат (замужем)" : (usrAdr.Relation == RelationType.NotMarried ? "Не женат (не замужем)" : "Не указано"))))))) : "Не указано");
            //                                }
            //                                else if (lstContHarValues[i].IndexOf("#online#") > 0)
            //                                {
            //                                    lstContHarValues[i] = lstContHarValues[i].Replace("#online#", usrAdr.Online != null ? (usrAdr.Online.Value ? "ON line" : "OFF line") : "Unknown");
            //                                }
            //                                else if (lstContHarValues[i].IndexOf("#birthdate#") > 0)
            //                                {
            //                                    lstContHarValues[i] = lstContHarValues[i].Replace("#birthdate#", usrAdr.BirthDate != null ? usrAdr.BirthDate : "");
            //                                }
            //                                else if (lstContHarValues[i].IndexOf("#counters_friends#") > 0)
            //                                {
            //                                    lstContHarValues[i] = lstContHarValues[i].Replace("#counters_friends#", usrAdr.Counters != null ? usrAdr.Counters.Friends.Value.ToString() : "");
            //                                }
            //                                else if (lstContHarValues[i].IndexOf("#age#") > 0)
            //                                {
            //                                    String birthdate = usrAdr.BirthDate != null ? usrAdr.BirthDate : "";
            //                                    String sAge = sUnknownAge;
            //                                    if (birthdate.Length > 0)
            //                                    {
            //                                        String sDD = birthdate.Substring(0, birthdate.IndexOf("."));
            //                                        birthdate = birthdate.Substring(birthdate.IndexOf(".") + 1);
            //                                        if (birthdate.IndexOf(".") > 0)
            //                                        {
            //                                            String sMM = birthdate.Substring(0, birthdate.IndexOf("."));
            //                                            birthdate = birthdate.Substring(birthdate.IndexOf(".") + 1);
            //                                            DateTime bday = new DateTime(Convert.ToInt32(birthdate), Convert.ToInt32(sMM), Convert.ToInt32(sDD));
            //                                            DateTime today = DateTime.Today;
            //                                            int age = today.Year - bday.Year;
            //                                            if (bday > today.AddYears(-age)) age--;
            //                                            sAge = Convert.ToString(age);
            //                                        }
            //                                    }

            //                                    lstContHarValues[i] = lstContHarValues[i].Replace("#age#", sAge);
            //                                }
            //                                else if (lstContHarValues[i].IndexOf("#clear#") > 0)
            //                                {
            //                                    lstContHarValues[i] = lstContHarValues[i].Replace("#clear#", "");
            //                                }

            //                            }

            //                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt"), lstContHarValues, Encoding.UTF8);

            //                            PersonenList_AddUserToVisualList(PersonenList_AddUser(FormMain.SocialNetwork, sUID, sUName, "", ""));

            //                            lvList.SelectedIndices.Clear();
            //                            if (lvList.Items.Count > 0)
            //                            {
            //                                lvList.SelectedIndices.Add(lvList.Items.Count - 1);
            //                                lvList.EnsureVisible(lvList.Items.Count - 1);
            //                            }
            //                            lvList_ItemChecked(null, null);
            //                            break;
            //                        }
            //                        catch (VkNet.Exception.AccessTokenInvalidException atiexp)
            //                        {
            //                            if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
            //                                break;
            //                        }
            //                        catch (System.Net.WebException)
            //                        {
            //                            if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
            //                                break;
            //                        }
            //                        catch (VkNet.Exception.VkApiException vkapexeption)
            //                        {
            //                            if (!mFormMain.ReAutorize(mFormMain.userLogin, mFormMain.userPassword))
            //                                break;
            //                        }
            //                        catch (Exception exp)
            //                        {
            //                            ExceptionToLogList("FormEditPersonenDB.button6_Click", sUID, exp);
            //                            MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_14", this.Name, "Ошибка запроса Персонажа с указанным ID..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Добавление Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                            break;
            //                        }
            //                        finally { }
            //                    }
            //                    while (true);
            //                }
            //            }//if (iPersUserID >= 0)
            //            else
            //            {
            //                List<String> lstContHar = new List<String>();
            //                for (int i = 0; i < iContHarCount; i++)
            //                    lstContHar.Add(sContHar[i, 0] + "|");

            //                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt"), lstContHar, Encoding.UTF8);

            //                PersonenList_AddUserToVisualList(PersonenList_AddUser(FormMain.SocialNetwork, sUID, sUID, "", ""));
            //            }
            //        }
            //    }
            //}
        }

        List<String> lstNILSA_UserDB;
        List<String> lstTinder_UserDB;
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

        private void Tinder_LoadUserDB()
        {
            lstTinder_UserDB = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "tinder_userdb.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "tinder_userdb.txt"));
                lstTinder_UserDB = new List<String>(srcFile);
            }
        }

        private void NILSA_SaveUserDB()
        {
            if (lstNILSA_UserDB.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "nilsa_userdb.txt"), lstNILSA_UserDB, Encoding.UTF8);

        }

        private void Tinder_SaveUserDB()
        {
            if (lstTinder_UserDB.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "tinder_userdb.txt"), lstTinder_UserDB, Encoding.UTF8);

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

        public String NILSA_getUserRecord(String uLogin, String uPassword)
        {
            NILSA_LoadUserDB();
            foreach (String srec in lstNILSA_UserDB)
            {
                if (srec.EndsWith("|" + uLogin + "|" + uPassword))
                    return srec;
            }
            return null;
        }
        public String Tinder_getUserRecord(String uLogin, String uPassword)
        {
            Tinder_LoadUserDB();
            var srec = "";
            foreach (var c in lstTinder_UserDB)
            {
                if (c.EndsWith("|" + uLogin + "|" + uPassword))
                {
                    srec = c;
                    break;
                }
            }
            return srec;
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

        private void LoadContactParamersValues(int iSocialNetwork, String sID)
        {
            lstContHarValues = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sID + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sID + ".txt"));
                lstContHarValues = new List<String>(srcFile);
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
                    int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;

                    if (sUID.Length > 0)
                    {
                        LoadContactParamersValues(iSocialNetwork, sUID);
                        FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                        fe.Text += " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_15", this.Name, "Персонажа");
                        fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
                        for (int i = 0; i < iContHarCount; i++)
                        {
                            for (int j = 0; j < iContHarAttrCount; j++)
                                fe.sPersHar[i, j] = sContHar[i, j];
                            fe.sPersHar[i, iContHarAttrCount] = GetContactParametersValue(sContHar[i, 0]);
                        }

                        fe.iPersHarAttrCount = iContHarAttrCount;
                        fe.iPersHarCount = iContHarCount;
                        fe.sFilePrefix = "pers";
                        fe.Setup();

                        if (fe.ShowDialog() == DialogResult.OK)
                        {
                            List<String> lstContHar = new List<String>();
                            for (int i = 0; i < iContHarCount; i++)
                                lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHar, Encoding.UTF8);

                            PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, lvList.Items[iSelIdx].SubItems[0].Text, lvList.Items[iSelIdx].SubItems[2].Text, lvList.Items[iSelIdx].SubItems[3].Text), iSelIdx);
                        }
                    }
                }
            }
            else
            {
                if (lvList.CheckedItems.Count > 0)
                {
                    List<String> lstContHar = GetHarValues(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_16", this.Name, "Замена/установка значений характеристик Персонажей"));
                    if (lstContHar == null)
                        return;

                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_18", this.Name, "Вы уверены, что хотите задать характеристики для всех отмеченных Персонажей?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_17", this.Name, "Редактирование характеристик Персонажей"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        for (int k = 0; k < lvList.Items.Count; k++)
                        {
                            if (lvList.Items[k].Checked)
                            {
                                String sUID = lvList.Items[k].SubItems[1].Text;
                                String sUName = lvList.Items[k].SubItems[0].Text;
                                String sULogin = lvList.Items[k].SubItems[2].Text;
                                String sUPwd = lvList.Items[k].SubItems[3].Text;
                                int iSocialNetwork = lvList.Items[k].ImageIndex;

                                List<String> lstContHarCurrent = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
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

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), k);

                            }
                        }

                        EnableAllButtons();
                    }
                }

            }
        }

        private List<String> GetHarValues(String sTitle)
        {
            FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
            fe.sPersHar = new String[iContHarCount, iContHarAttrCount + 1];
            for (int i = 0; i < iContHarCount; i++)
            {
                for (int j = 0; j < iContHarAttrCount; j++)
                    fe.sPersHar[i, j] = sContHar[i, j];
                fe.sPersHar[i, iContHarAttrCount] = "";
            }

            fe.iPersHarAttrCount = iContHarAttrCount;
            fe.iPersHarCount = iContHarCount;
            fe.sFilePrefix = "pers";
            fe.Text = sTitle;
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                List<String> lstContHar = new List<String>();
                for (int i = 0; i < iContHarCount; i++)
                    lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);
                return lstContHar;
            }
            return null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (lvList.SelectedIndices.Count > 0)
            {
                int iSelIdx = lvList.SelectedIndices[0];
                String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;

                if (iSocialNetwork == 1 && sUID.Equals("0"))
                {
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_20", this.Name, "Логин и пароль Персонажа NILSA не могут быть изменены."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_19", this.Name, "Смена логина и пароля Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (sUID.Length > 0)
                {
                    string value = PersonenList_GetUserField(iSocialNetwork, sUID, USERFIELD_LOGIN);
                    if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_21", this.Name, "Изменение логина и пароля"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Введите логин Персонажа:"), ref value) == DialogResult.OK)
                    {
                        String sULogin = value;
                        value = PersonenList_GetUserField(iSocialNetwork, sUID, USERFIELD_PASSWORD);
                        if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_21", this.Name, "Изменение логина и пароля"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_23", this.Name, "Введите пароль Персонажа:"), ref value) == DialogResult.OK)
                        {
                            String sUPwd = value;
                            String sUName = PersonenList_GetUserField(iSocialNetwork, sUID, USERFIELD_NAME);

                            PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), iSelIdx);
                        }
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (lvList.SelectedIndices.Count > 0)
            {
                int iSelIdx = lvList.SelectedIndices[0];
                String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;

                if (sUID.Length > 0)
                {
                    String sULogin = PersonenList_GetUserField(iSocialNetwork, sUID, USERFIELD_LOGIN);
                    String sUPwd = PersonenList_GetUserField(iSocialNetwork, sUID, USERFIELD_PASSWORD);
                    if (sULogin.Length == 0 || sUPwd.Length == 0)
                    {
                        MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_25", this.Name, "Невозможно сменить Персонаж на выбранный, сначала для него необходимо задать логин и пароль для входа в ВКонтакте."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_24", this.Name, "Смена Персонажа"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        bNeedPersoneChange = true;
                        suSelSocialNetwork = iSocialNetwork;
                        suSelLogin = sULogin;
                        suSelPwd = sUPwd;
                        suSelID = sUID;
                        DialogResult = DialogResult.OK;
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (lvList.SelectedIndices.Count > 0)
            {
                int iSelIdx = lvList.SelectedIndices[0];
                String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;

                if (sUID.Length > 0)
                {
                    if (lvList.CheckedItems.Count > 0)
                        if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_27", this.Name, "Вы уверены, что хотите редактировать Разрешенные алгоритмы для всех отмеченных Персонажей?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_26", this.Name, "Редактирование Разрешенных алгоритмов"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return;

                    FormEditUserEnabledAlgorithmsList fe = new FormEditUserEnabledAlgorithmsList(mFormMain);
                    fe.Setup(Convert.ToInt32(sUID), iSocialNetwork);

                    if (fe.ShowDialog() == DialogResult.OK)
                    {
                        //---
                        if (lvList.CheckedItems.Count > 0)
                        {
                            List<String> _lstMessagesDBUserList = new List<String>();
                            if (File.Exists(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                                _lstMessagesDBUserList = new List<String>(srcFile);
                            }

                            DisableAllButtons();

                            for (int k = 0; k < lvList.Items.Count; k++)
                            {
                                if (lvList.Items[k].Checked)
                                {
                                    String sUIDSelected = lvList.Items[k].SubItems[1].Text;
                                    int iSocialNetworkSelected = lvList.Items[k].ImageIndex;
                                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(iSocialNetworkSelected) + sUIDSelected + ".txt"), _lstMessagesDBUserList, Encoding.UTF8);
                                }
                            }

                            EnableAllButtons();
                        }

                        //---
                    }
                }
            }

        }

        private void lvList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool bBS = lvList.SelectedIndices.Count > 0;
            button2.Enabled = bBS || lvList.CheckedItems.Count > 0;
            button23.Enabled = bBS || lvList.CheckedItems.Count > 0;
            button3.Enabled = bBS;
            button5.Enabled = bBS || lvList.CheckedItems.Count > 0;
            button7.Enabled = bBS;
            button8.Enabled = bBS;
            button11.Enabled = bBS || lvList.CheckedItems.Count > 0;
            buttonOpenPersoneWebPage.Enabled = bBS;
            toolStripButtonCopyToClipboard.Enabled = bBS;
            buttonCopyParameters.Enabled = bBS;
            buttonPasteParameters.Enabled = copyedParameters != null && (bBS || lvList.CheckedItems.Count > 0);
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
                this.bInterruptOperation = true;
                this.button9.Enabled = false;
                if (!bDoImportContacts)
                    this.bwProgress.CancelAsync();
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
            lvList.BeginUpdate();
            for (int k = 0; k < lvList.Items.Count; k++)
            {
                lvList.Items[k].Checked = true;
            }
            lvList.EndUpdate();
            Cursor = Cursors.Arrow;
        }

        private void button12_Click(object sender, EventArgs e)
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

            for (int i = 0; i < lvList.Items.Count; i++)
            {

                String sUID = lvList.Items[i].SubItems[1].Text;
                int iSocialNetwork = lvList.Items[i].ImageIndex;
                Boolean bEquals = bRQVEmpty;
                if (!bEquals)
                {
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                    {
                        List<String> lstContHarValues = new List<String>();
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
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
                    lvList.Items[i].Checked = true;
            }
            lvList.EndUpdate();
            Cursor = Cursors.Arrow;
            //---
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
            fe.sFilePrefix = "pers";
            fe.Setup();

            if (fe.ShowDialog() == DialogResult.OK)
            {
                String[] RQV = new String[iContHarCount];
                for (int iii = 0; iii < iContHarCount; iii++)
                {
                    RQV[Convert.ToInt32(fe.sPersHar[iii, 0]) - 1] = fe.sPersHar[iii, iContHarAttrCount];
                    sContHar[iii, iContHarAttrCount] = fe.sPersHar[iii, iContHarAttrCount];
                }
                SelectFilterList(RQV);
            }
        }

        private void lvList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                button12.Enabled = true;
                button14.Enabled = true;
                button20.Enabled = true;
                button16.Enabled = true;
                button19.Enabled = true;
                toolStripButtonCheckPersonenActivityAndCopyContacters.Enabled = true;
                button18.Enabled = true;
                tbJoinCommunity.Enabled = true;
                buttonExportPersonenServer.Enabled = true;
                buttonExportPersonenAsContactersServer.Enabled = true;
                tbLeaveCommunity.Enabled = true;
                tbRepostGroup.Enabled = true;
                tbRepostWall.Enabled = true;
                tbLikeGroup.Enabled = true;
                tbLikeWall.Enabled = true;
                tbFriendsAdd.Enabled = true;
                tbFriendsDelete.Enabled = true;
            }
            else
            {
                button12.Enabled = false;
                button14.Enabled = false;
                button20.Enabled = false;
                button16.Enabled = false;
                button19.Enabled = false;
                toolStripButtonCheckPersonenActivityAndCopyContacters.Enabled = false;
                button18.Enabled = false;
                tbJoinCommunity.Enabled = false;
                buttonExportPersonenServer.Enabled = false;
                buttonExportPersonenAsContactersServer.Enabled = false;
                tbLeaveCommunity.Enabled = false;
                tbRepostGroup.Enabled = false;
                tbRepostWall.Enabled = false;
                tbLikeGroup.Enabled = false;
                tbLikeWall.Enabled = false;
                tbFriendsAdd.Enabled = false;
                tbFriendsDelete.Enabled = false;
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
            button23.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            button5.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            button11.Enabled = lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0;
            buttonCopyParameters.Enabled = lvList.SelectedItems.Count > 0;
            buttonPasteParameters.Enabled = copyedParameters != null && (lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0);
            toolStripButtonCopyToClipboard.Enabled = lvList.SelectedItems.Count > 0;
        }

        private void StartImportContactsFromGroupThread(String sGID, List<String> lstContHar, List<String> lstFilterHar)
        {
            //if (FormMain.SocialNetwork == 1)
            //{
            //    Cursor = Cursors.WaitCursor;
            //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
            //    {
            //        Cursor = Cursors.Arrow;
            //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обработка Персонажей ВКонтакте");
            //        return;
            //    }
            //    Cursor = Cursors.Arrow;
            //}

            // This method runs on the main thread.
            this.pbProgress.Visible = true;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Value = 0;
            this.pbProgress.Maximum = 10;
            this.pbProgress.Text = "...";
            this.bDoImportContacts = false;

            DisableAllButtons();

            bInterruptOperation = false;
            button9.Text = "X";
            button9.Enabled = true;

            //if (iImportMode == 3)
            //{
            //    this.button14.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_28", this.Name, "Прервать");
            //    this.button14.Enabled = true;
            //}
            //else if (iImportMode == 4)
            //{
            //    this.button15.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_28", this.Name, "Прервать");
            //    this.button15.Enabled = true;
            //}


            // Initialize the object that the background worker calls.
            ImportContactsFromGroup WC = new ImportContactsFromGroup(this, mFormMain);
            WC.sGroupID = sGID;
            WC.iPersUserID = iPersUserID;
            WC.sPersUsersIDs = "";
            WC.lstContHar = lstContHar;
            WC.lstFilterHar = lstFilterHar;
            WC.iContHarCount = iContHarCount;
            WC.maxContacterCount = 99999999;
            // Start the asynchronous operation.
            bwProgress.RunWorkerAsync(WC);
        }

        delegate void StringParameterDelegate(DBPerson dbPerson, int idx);
        public void UpdateProgress_AddUserToVisualList(DBPerson dbPerson, int idx)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new StringParameterDelegate(UpdateProgress_AddUserToVisualList), new object[] { dbPerson, idx });
                return;
            }
            // Must be on the UI thread if we've got this far
            int idxList = PersonenList_AddUserToVisualList(dbPerson, idx);
            if (idxList >= 0)
            {
                lvList.SelectedIndices.Add(idxList);
                lvList.EnsureVisible(idxList);
            }
        }

        public int PersonenList_GetVisualListIdx(int iSocialNetwork, String sUD)
        {
            int iuserIdx = -1;
            for (int k = 0; k < lvList.Items.Count; k++)
            {
                String str = lvList.Items[k].SubItems[1].Text;
                if (str.Equals(sUD) && lvList.Items[k].ImageIndex == iSocialNetwork)
                {
                    iuserIdx = k;
                    break;
                }
            }
            return iuserIdx;
        }

        delegate int UpdateProgress_GetVisualListIdxDelegate(int isn, string uid);
        public int UpdateProgress_GetVisualListIdx(int isn, string uid)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                return (int)this.Invoke(new UpdateProgress_GetVisualListIdxDelegate(PersonenList_GetVisualListIdx), new object[] { isn, uid });
            }
            // Must be on the UI thread if we've got this far
            else
                return PersonenList_GetVisualListIdx(isn, uid);
        }

        private void bwProgress_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;

            // Get the Words object and call the main method.
            ImportContactsFromGroup WC = (ImportContactsFromGroup)e.Argument;
            WC.ImportContacts(worker, e);
        }

        private void bwProgress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ImportContactsFromGroup.CurrentState state = (ImportContactsFromGroup.CurrentState)e.UserState;
            if (this.pbProgress.Maximum != state.ContactsTotal)
                this.pbProgress.Maximum = state.ContactsTotal;
            this.pbProgress.Value = state.ContactsImported;
            //if (iImportMode == 3)
            this.pbProgress.Text = state.ContactsImported.ToString() + "/" + state.ContactsTotal.ToString();
            //else if (iImportMode == 4)
            //    this.labelProgress.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_28", this.Name, "Прервать") + " (" + state.ContactsImported.ToString() + "/" + state.ContactsTotal.ToString() + ")";

            this.Text = state.sUName + (state.dtLastSeen.Length > 0 ? (" (" + state.dtLastSeen + ")") : "");
        }

        private void bwProgress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.pbProgress.Visible = false;
            this.bDoImportContacts = true;
            this.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Редактирование базы Персонажей");

            //if (iImportMode == 3)
            //{
            //    this.button14.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button14", this.Name, "Обновить хар-ки отм.");
            //}
            //else if (iImportMode == 4)
            //{
            //    this.button15.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button15", this.Name, "Импорт из файла");
            //}
            bInterruptOperation = false;
            button9.Text = ">";
            EnableAllButtons();

            if (FormMain.SocialNetwork == 0 && iPersUserID >= 0)
                AutorizeVK(mFormMain.userLogin, mFormMain.userPassword);

            if (e.Error != null)
                MessageBox.Show("Error: " + e.Error.Message, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_29", this.Name, "Импорт данных Персонажей"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (e.Cancelled)
            {
                MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_30", this.Name, "Импорт данных Персонажей прерван. Если какие-то из Персонажей не отображаются в списке, смените настройки фильтра базы Персонажей по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_29", this.Name, "Импорт данных Персонажей"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_31", this.Name, "Импорт данных Персонажей завершен. Если какие-то из Персонажей не отображаются в списке, смените настройки фильтра базы Персонажей по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_29", this.Name, "Импорт данных Персонажей"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //String vkontakteLogin = "+79533411342";     // Aslin Bikase
        //String vkontaktePassword = "jadR678ypp";

        private void button14_Click(object sender, EventArgs e)
        {
            if (bDoImportContacts)
            {
                if (lvList.CheckedItems.Count > 0)
                {
                    String sImportIDs = "";
                    foreach (ListViewItem item in lvList.CheckedItems)
                    {
                        String sUID = item.SubItems[1].Text;
                        int iSocialNetwork = item.ImageIndex;
                        if (iSocialNetwork == 0)
                            sImportIDs += sUID + "|";
                    }

                    if (sImportIDs.Length == 0)
                    {
                        MessageBox.Show("Не выбрано ни одного Персонажа из социальной сети ВКонтакте", "Обновление хар-к отмеченных Персонажей");
                        return;
                    }

                    //if (FormMain.SocialNetwork == 1)
                    //{
                    //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
                    //    {
                    //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обновление хар-к отмеченных Персонажей");
                    //        return;
                    //    }
                    //}

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
                    fe.sFilePrefix = "pers";
                    fe.Setup();

                    if (fe.ShowDialog() == DialogResult.OK)
                    {
                        SaveFormEditPersHarValues(fe, iImportMode, 1);
                        List<String> lstContHar = new List<String>();
                        for (int i = 0; i < iContHarCount; i++)
                            lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);

                        if (lstFilterHar != null)
                            StartImportContactsFromGroupThread("UIDs=" + sImportIDs, lstContHar, lstFilterHar);
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
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersonenDB.columns"), lstcolumns, Encoding.UTF8);
        }

        private void LoadColumnsOrderAndWidth()
        {
            List<String> lstcolumns = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersonenDB.columns")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersonenDB.columns"));
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

        private void FormEditPersonenDB_FormClosing(object sender, FormClosingEventArgs e)
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

        private void gotoPersoneWebPage()
        {
            if (lvList.SelectedIndices.Count > 0)
            {
                int iSelIdx = lvList.SelectedIndices[0];
                String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                String sUName = lvList.Items[iSelIdx].SubItems[0].Text;
                String sULogin = lvList.Items[iSelIdx].SubItems[2].Text;
                String sUPwd = lvList.Items[iSelIdx].SubItems[3].Text;
                int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;

                if (iSocialNetwork == 0)
                {
                    if (sULogin.Length == 0 || sUPwd.Length == 0 || !mFormMain.FormWebBrowserEnabled)
                        System.Diagnostics.Process.Start("http://vk.com/id" + sUID);
                    else
                    {
                        FormWebBrowser fwb = new FormWebBrowser(mFormMain);
                        fwb.Setup(sULogin, sUPwd, WebBrowserCommand.Autorize, Convert.ToInt64(sUID));
                        fwb.ShowDialog();
                    }
                }
            }
        }

        private void lvList_DoubleClick(object sender, EventArgs e)
        {
            gotoPersoneWebPage();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            FormSelectSocialNetwork fssn = new FormSelectSocialNetwork();
            fssn.Setup(true);
            fssn.cbSocialNetwork.SelectedIndex = 0;
            if (fssn.ShowDialog() != DialogResult.OK)
                return;
            int iSocialNetwork = fssn.cbSocialNetwork.SelectedIndex;
            bool bIgnoreExists = fssn.IgnoreExists();

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
                        int iFieldName = -1, iFieldID = -1, iFieldLogin = -1, iFieldPwd = -1;
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
                        for (int i = 0; i < IW.Length; i++)
                            if (IW[i].ToLower().Equals("логин"))
                            {
                                iFieldLogin = i;
                                break;
                            }
                        for (int i = 0; i < IW.Length; i++)
                            if (IW[i].ToLower().Equals("пароль"))
                            {
                                iFieldPwd = i;
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
                                String sULogin = iFieldLogin >= 0 ? IWRecord[iFieldLogin] : "";
                                String sUPwd = iFieldPwd >= 0 ? IWRecord[iFieldPwd] : "";
                                String sAttrFileName = "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt";

                                if (bIgnoreExists)
                                    if (PersonenList_GetUserIdx(iSocialNetwork, sUID) >= 0)
                                        continue;

                                List<String> lstContHarValuesTemp = new List<String>();
                                for (int i = 0; i < iContHarCount; i++)
                                {
                                    lstContHarValuesTemp.Add(sContHar[i, 0] + "|" + (iFieldHar[i] >= 0 ? IWRecord[iFieldHar[i]] : ""));
                                }

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName), lstContHarValuesTemp, Encoding.UTF8);

                                int idxList = PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), PersonenList_GetVisualListIdx(iSocialNetwork, sUID));
                                if (idxList >= 0)
                                {
                                    lvList.SelectedIndices.Add(idxList);
                                    lvList.EnsureVisible(idxList);
                                }
                            }

                        }
                    }
                }//if (File.Exists(openFileDialog.FileName))
            }

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

                if (lvList.CheckedIndices.Count <= 0)
                {
                    if (lvList.SelectedIndices.Count > 0)
                    {
                        int iSelIdx = lvList.SelectedIndices[0];
                        String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                        String sUName = lvList.Items[iSelIdx].SubItems[0].Text;
                        String sULogin = lvList.Items[iSelIdx].SubItems[2].Text;
                        String sUPwd = lvList.Items[iSelIdx].SubItems[3].Text;
                        int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;

                        if (sUID.Length > 0)
                        {
                            List<String> lstContHarCurrent = new List<String>();
                            if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                                lstContHarCurrent = new List<String>(srcFile);
                                lstContHarCurrent[iAlgValHar] = lstContHarCurrent[iAlgValHar].Substring(0, lstContHarCurrent[iAlgValHar].IndexOf("|") + 1) + SelectedAlgorithm.Name;

                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), iSelIdx);

                            }
                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstAlgorithmsDBRecordTS, Encoding.UTF8);
                        }
                    }
                }
                else
                {
                    if (lvList.CheckedItems.Count > 0)
                    {
                        if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_33", this.Name, "Вы уверены, что хотите установить выбранный алгоритм для всех отмеченных Персонажей?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_32", this.Name, "Установка алгоритма Персонажей"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            DisableAllButtons();

                            for (int k = 0; k < lvList.Items.Count; k++)
                            {
                                if (lvList.Items[k].Checked)
                                {
                                    String sUID = lvList.Items[k].SubItems[1].Text;
                                    String sUName = lvList.Items[k].SubItems[0].Text;
                                    String sULogin = lvList.Items[k].SubItems[2].Text;
                                    String sUPwd = lvList.Items[k].SubItems[3].Text;
                                    int iSocialNetwork = lvList.Items[k].ImageIndex;

                                    List<String> lstContHarCurrent = new List<String>();
                                    if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                                    {
                                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                                        lstContHarCurrent = new List<String>(srcFile);
                                        lstContHarCurrent[iAlgValHar] = lstContHarCurrent[iAlgValHar].Substring(0, lstContHarCurrent[iAlgValHar].IndexOf("|") + 1) + SelectedAlgorithm.Name;

                                        File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                        PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), k);

                                    }
                                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstAlgorithmsDBRecordTS, Encoding.UTF8);

                                }
                            }

                            EnableAllButtons();
                        }
                    }

                }
            }

        }

        private long AutorizeVK(String sUsrSelLogin, String sUsrSelPwd)
        {
            if (mFormMain.fwbVKontakte == null)
            {
                mFormMain.fwbVKontakte = new FormWebBrowser(mFormMain, true);
                mFormMain.fwbVKontakte.Init();
            }

            //stopTimers();

            mFormMain.fwbVKontakte.Setup(sUsrSelLogin, sUsrSelPwd, WebBrowserCommand.LoginPersone);
            if (!mFormMain.fwbVKontakteFirstShow)
            {
                mFormMain.fwbVKontakteFirstShow = true;
                mFormMain.fwbVKontakte.Show();
            }
            mFormMain.fwbVKontakte.WaitResult();
            long persID = mFormMain.fwbVKontakte.loggedPersoneID;
            return persID;


            /*
            FormMain.api = new VkApi();

            long? captcha_sid = null;
            string captcha_key = null;

            for (int iAppID = 0; iAppID < FormMain.userAppIds.Length; iAppID++)
                if (FormMain.userAppId == FormMain.userAppIds[iAppID])
                {
                    FormMain.userAppIdsPos = iAppID;
                    FormMain.userAppIdsPosStart = iAppID;
                    break;
                }
            do
            {
                Settings settings = Settings.All; // уровень доступа к данным

                try
                {
                    FormMain.api.Authorize(FormMain.userAppId, sUsrSelLogin, sUsrSelPwd, settings, captcha_sid, captcha_key); // авторизуемся

                    if (FormMain.api.UserId.HasValue)
                    {
                        VkNet.Model.User usrAdr = FormMain.api.Users.Get(FormMain.api.UserId.Value);
                        mFormMain.HideFormWait();
                        return FormMain.api.UserId.Value;
                    }
                }
                catch (VkNet.Exception.CaptchaNeededException ex)
                {
                    captcha_sid = ex.Sid;
                    mFormMain.HideFormWait();
                    var form = new FormCaptcha(mFormMain, ex.Img, ex.Sid);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        captcha_key = form.CaptchaKey.Text;
                        mFormMain.ShowFormWait();
                        continue;
                    }
                    mFormMain.ShowFormWait();
                }
                catch (VkNet.Exception.VkApiException exapi)
                {
                    if (exapi.Message != null)
                    {
                        if (exapi.Message.Contains("(401)"))
                        {
                            FormMain.userAppIdsPos++;
                            if (FormMain.userAppIdsPos >= FormMain.userAppIds.Length)
                                FormMain.userAppIdsPos = 0;

                            if (FormMain.userAppIdsPos == FormMain.userAppIdsPosStart)
                                return -1;

                            FormMain.userAppId = FormMain.userAppIds[FormMain.userAppIdsPos];
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    //ExceptionToLogList("Autorize", userLogin + "/" + userPassword, e);
                }
                finally { }

                return -1;

            } while (true);
            */
        }

        bool bInterruptOperation = false;
        private async void button19_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_35", this.Name, "Вы хотите провести проверку активности для всех отмеченных Персонажей?"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_34", this.Name, "Проверка активности Персонажей"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            string s14v = lvList.Items[k].SubItems[4 + 14].Text;
                            lvList.Items[k].SubItems[4 + 14].Text = "?";
                            Application.DoEvents();

                            if (iSocialNetwork == 0)
                                await SleepDelay(5000);

                            if (bInterruptOperation)
                            {
                                lvList.Items[k].SubItems[4 + 14].Text = s14v;
                                break;
                            }
                            List<String> lstContHarCurrent = new List<String>();
                            if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                                lstContHarCurrent = new List<String>(srcFile);
                                for (int i = 0; i < iContHarCount; i++)
                                {
                                    if (i == 14)
                                    {
                                        if (iSocialNetwork == 0)
                                            lstContHarCurrent[i] = "15|" + (AutorizeVK(sULogin, sUPwd) >= 0 ? "Active" : "Blocked");
                                        else if (iSocialNetwork == 1)
                                            lstContHarCurrent[i] = "15|" + "Active";
                                    }
                                }
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), k);
                            }
                        }
                    }
                    if (FormMain.SocialNetwork == 0 && iPersUserID >= 0)
                        AutorizeVK(mFormMain.userLogin, mFormMain.userPassword);

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();
                }
            }

        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
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

                for (int k = 0; k < lvList.Items.Count; k++)
                {
                    if (lvList.Items[k].Checked)
                    {
                        String sUID = lvList.Items[k].SubItems[1].Text;
                        String sUName = lvList.Items[k].SubItems[0].Text;
                        String sULogin = lvList.Items[k].SubItems[2].Text;
                        String sUPwd = lvList.Items[k].SubItems[3].Text;
                        int iSocialNetwork = lvList.Items[k].ImageIndex;

                        List<String> lstContHarCurrent = new List<String>();
                        if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                            lstContHarCurrent = new List<String>(srcFile);

                            int algid = getPersoneAlgorithmID(Convert.ToInt64(sUID));
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
                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                            PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), k);
                        }

                    }
                }

                EnableAllButtons();
            }

        }

        private int getPersoneAlgorithmID(long contid)
        {
            int algid = -1;
            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + contid.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algotithm_settings_" + FormMain.getSocialNetworkPrefix() + contid.ToString() + ".txt"));
                lstTS = new List<String>(srcFile);
                algid = Convert.ToInt32(lstTS[0]);

                if (algid < 0)
                    algid = -1;
            }
            return algid;
        }

        async Task SleepDelay(int delay)
        {
            await Task.Delay(delay);
        }

        private async void buttonOpenPersoneWebPage_Click(object sender, EventArgs e)
        {
            if (!mFormMain.FormWebBrowserEnabled)
            {
                MessageBox.Show("Использование встроенного браузера запрещено в настройках программы. Для работы команды необходимо отключить опцию 'Настройки' -> 'Отключить использование встроенного браузера'", "Проверка страниц Персонажей", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (lvList.CheckedIndices.Count <= 0)
            {
                if (lvList.SelectedIndices.Count > 0)
                {
                    int iSelIdx = lvList.SelectedIndices[0];
                    String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                    String sUName = lvList.Items[iSelIdx].SubItems[0].Text;
                    String sULogin = lvList.Items[iSelIdx].SubItems[2].Text;
                    String sUPwd = lvList.Items[iSelIdx].SubItems[3].Text;
                    int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;
                    int k = iSelIdx;
                    if (iSocialNetwork == 0 && sUID.Length > 0 && mFormMain.FormWebBrowserEnabled)
                    {
                        FormWebBrowser fwb = new FormWebBrowser(mFormMain);
                        fwb.Setup(sULogin, sUPwd, WebBrowserCommand.CheckPersonePage);
                        fwb.ShowDialog();

                        //---
                        string s14v = lvList.Items[k].SubItems[4 + 14].Text;
                        lvList.Items[k].SubItems[4 + 14].Text = "?";
                        Application.DoEvents();

                        if (iSocialNetwork == 0)
                            await SleepDelay(500);

                        List<String> lstContHarCurrent = new List<String>();
                        if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                            lstContHarCurrent = new List<String>(srcFile);
                            for (int i = 0; i < iContHarCount; i++)
                            {
                                if (i == 14)
                                {
                                    if (iSocialNetwork == 0)
                                        lstContHarCurrent[i] = "15|" + (AutorizeVK(sULogin, sUPwd) >= 0 ? "Active" : "Blocked");
                                    else if (iSocialNetwork == 1)
                                        lstContHarCurrent[i] = "15|" + "Active";
                                }
                            }
                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                            PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), k);
                        }

                        //---
                        if (FormMain.SocialNetwork == 0 && iPersUserID >= 0)
                            AutorizeVK(mFormMain.userLogin, mFormMain.userPassword);

                    }
                }
            }
            else
            {
                if (lvList.CheckedItems.Count > 0)
                {
                    if (MessageBox.Show("Вы хотите провести проверку страниц во встроенном браузере для всех отмеченных Персонажей?", "Проверка страниц Персонажей", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();
                            Application.DoEvents();
                            await SleepDelay(2000);

                            if (bInterruptOperation)
                                break;

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            if (iSocialNetwork == 0 && sUID.Length > 0 && mFormMain.FormWebBrowserEnabled)
                            {
                                FormWebBrowser fwb = new FormWebBrowser(mFormMain);
                                fwb.Setup(sULogin, sUPwd, WebBrowserCommand.CheckPersonePage);
                                fwb.ShowDialog();

                                //---
                                string s14v = lvList.Items[k].SubItems[4 + 14].Text;
                                lvList.Items[k].SubItems[4 + 14].Text = "?";
                                Application.DoEvents();

                                if (iSocialNetwork == 0)
                                    await SleepDelay(500);

                                if (bInterruptOperation)
                                {
                                    lvList.Items[k].SubItems[4 + 14].Text = s14v;
                                    break;
                                }
                                List<String> lstContHarCurrent = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                                    lstContHarCurrent = new List<String>(srcFile);
                                    for (int i = 0; i < iContHarCount; i++)
                                    {
                                        if (i == 14)
                                        {
                                            if (iSocialNetwork == 0)
                                                lstContHarCurrent[i] = "15|" + (AutorizeVK(sULogin, sUPwd) >= 0 ? "Active" : "Blocked");
                                            else if (iSocialNetwork == 1)
                                                lstContHarCurrent[i] = "15|" + "Active";
                                        }
                                    }
                                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                    PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), k);
                                }

                                //---
                            }
                        }
                    }

                    if (FormMain.SocialNetwork == 0 && iPersUserID >= 0)
                        AutorizeVK(mFormMain.userLogin, mFormMain.userPassword);

                    this.pbProgress.Visible = false;
                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();
                }

            }


        }

        private async void ImportDjekxaruButton_Click(object sender, EventArgs e)
        {
            if (bDoImportContacts)
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;
                String sImportIDs = "";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    List<String> lstImportIDs = new List<String>();
                    List<String> lstErrorsIDs = new List<String>();
                    if (File.Exists(openFileDialog.FileName))
                    {
                        var srcFile = File.ReadAllLines(openFileDialog.FileName);
                        lstImportIDs = new List<String>(srcFile);
                        DisableAllButtons();
                        bInterruptOperation = false;
                        button9.Text = "X";
                        button9.Enabled = true;

                        int _iCount = 0;
                        int _nCount = lstImportIDs.Count;
                        this.pbProgress.Visible = true;
                        this.pbProgress.Minimum = 0;
                        this.pbProgress.Value = 0;
                        this.pbProgress.Maximum = _nCount;
                        this.pbProgress.Text = "...";

                        foreach (String str in lstImportIDs)
                        {
                            if (bInterruptOperation)
                                break;

                            if (str == null)
                                continue;

                            if (str.Length == 0)
                                continue;

                            String value = str;
                            String sLogin = value.Substring(0, value.IndexOf(":"));
                            value = value.Substring(value.IndexOf(":") + 1);
                            String sPassword = "";
                            String sEmail = "";
                            String sEmailPassword = "";
                            if (value.IndexOf("|") > 0)
                            {
                                sPassword = value.Substring(0, value.IndexOf("|"));
                                value = value.Substring(value.IndexOf("|") + 1);
                                sEmail = value.Substring(0, value.IndexOf(":"));
                                value = value.Substring(value.IndexOf(":") + 1);
                                sEmailPassword = value;
                            }
                            else
                            {
                                sPassword = value;
                            }

                            List<String> lstContHarCurrent = new List<String>();
                            for (int i = 0; i < iContHarCount; i++)
                            {
                                lstContHarCurrent.Add((i + 1).ToString() + "|");
                            }

                            long uid = AutorizeVK(sLogin, sPassword);
                            //if (uid < 0 && mFormMain.FormWebBrowserEnabled)
                            //{
                            //    FormWebBrowser fwb = new FormWebBrowser(mFormMain);
                            //    fwb.Setup(sLogin, sPassword, WebBrowserCommand.CheckPersonePage);
                            //    fwb.ShowDialog();
                            //    uid = AutorizeVK(sLogin, sPassword);
                            //}
                            String sUID = uid.ToString();
                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            await SleepDelay(3000);

                            if (bInterruptOperation)
                                break;

                            if (uid >= 0)
                            {
                                sImportIDs += sUID + "|";
                                lstContHarCurrent[14] = "15|" + "Active";
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(0) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                int idxList = PersonenList_AddUserToVisualList(PersonenList_AddUser(0, sUID, "Unknown", sLogin, sPassword), PersonenList_GetVisualListIdx(0, sUID));
                                if (idxList >= 0)
                                {
                                    lvList.SelectedIndices.Add(idxList);
                                    lvList.EnsureVisible(idxList);
                                }

                            }
                            else
                                lstErrorsIDs.Add(str);
                        }

                        if (iPersUserID >= 0 && FormMain.SocialNetwork == 0)
                            AutorizeVK(mFormMain.userLogin, mFormMain.userPassword);

                        this.pbProgress.Visible = false;
                        bInterruptOperation = false;
                        button9.Text = ">";
                        EnableAllButtons();

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
                        fe.sFilePrefix = "pers";
                        fe.Setup();

                        List<String> lstContHar = new List<String>();
                        for (int i = 0; i < iContHarCount; i++)
                            lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);

                        if (lstFilterHar != null)
                            StartImportContactsFromGroupThread("UIDs=" + sImportIDs, lstContHar, lstFilterHar);

                        if (lstErrorsIDs.Count > 0)
                        {
                            File.WriteAllLines(Path.Combine(Application.StartupPath, "errorimport.txt"), lstErrorsIDs, Encoding.UTF8);
                            Process.Start(Path.Combine(Application.StartupPath, "errorimport.txt"));
                        }
                    }
                }
            }
        }

        private void tbShowMode_Click(object sender, EventArgs e)
        {
            bShowMode = !bShowMode;
            Setup(slstErrorsInit, iPersUserID, bShowMode);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void tbJoinCommunity_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                //if (FormMain.SocialNetwork == 1)
                //{
                //    Cursor = Cursors.WaitCursor;
                //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
                //    {
                //        Cursor = Cursors.Arrow;
                //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обработка Персонажей ВКонтакте");
                //        return;
                //    }
                //    Cursor = Cursors.Arrow;
                //}

                string value = "";
                if (FormMain.InputBox(this, "Вступление в группу", "Введите ID группы или групп через запятую:", ref value) == DialogResult.OK)
                {
                    string sGroups = value.Trim();
                    if (sGroups.Length == 0)
                        return;

                    if (!checkFormatGroups(sGroups))
                    {
                        MessageBox.Show("Ошибка формата.\n\nФормат: ID_группы1,...ID_группыN\n\n(допустимо также указать только одну группу)", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            if (iSocialNetwork == 1)
                                continue;

                            DateTime dt = DateTime.Now;
                            if (sUID == iPersUserID.ToString() && FormMain.SocialNetwork == iSocialNetwork)
                            {
                                mFormMain.lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "JOIN_COMMUNITY " + sGroups);
                            }
                            else
                            {
                                List<String> lstReceivedMessagesTemp = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"));
                                    lstReceivedMessagesTemp = new List<String>(srcFile);
                                }
                                lstReceivedMessagesTemp.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "JOIN_COMMUNITY " + sGroups);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                            }
                        }
                    }

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();

                    MessageBox.Show("Выполнение операции запланировано. Запустите отмеченных Персонажей в ротацию для физического выполнения операции каждым Персонажем.", "Операции с Персонажами", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void tbLeaveCommunity_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                //if (FormMain.SocialNetwork == 1)
                //{
                //    Cursor = Cursors.WaitCursor;
                //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
                //    {
                //        Cursor = Cursors.Arrow;
                //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обработка Персонажей ВКонтакте");
                //        return;
                //    }
                //    Cursor = Cursors.Arrow;
                //}

                string value = "";
                if (FormMain.InputBox(this, "Выход из группы", "Введите ID группы или групп через запятую:", ref value) == DialogResult.OK)
                {
                    string sGroups = value.Trim();
                    if (sGroups.Length == 0)
                        return;

                    if (!checkFormatGroups(sGroups))
                    {
                        MessageBox.Show("Ошибка формата.\n\nФормат: ID_группы1,...ID_группыN\n\n(допустимо также указать только одну группу)", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            if (iSocialNetwork == 1)
                                continue;

                            DateTime dt = DateTime.Now;
                            if (sUID == iPersUserID.ToString() && FormMain.SocialNetwork == iSocialNetwork)
                            {
                                mFormMain.lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "LEAVE_COMMUNITY " + sGroups);
                            }
                            else
                            {
                                List<String> lstReceivedMessagesTemp = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"));
                                    lstReceivedMessagesTemp = new List<String>(srcFile);
                                }
                                lstReceivedMessagesTemp.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "LEAVE_COMMUNITY " + sGroups);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                            }
                        }
                    }

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();

                    MessageBox.Show("Выполнение операции запланировано. Запустите отмеченных Персонажей в ротацию для физического выполнения операции каждым Персонажем.", "Операции с Персонажами", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
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

        private bool checkFormatRepost(string repost_wall_posts)
        {
            string repost_wall_group = "";
            if (repost_wall_posts.Length > 0 && repost_wall_posts.IndexOf(" ") > 0)
            {
                repost_wall_group = repost_wall_posts.Substring(0, repost_wall_posts.IndexOf(" "));
                repost_wall_posts = repost_wall_posts.Substring(repost_wall_posts.IndexOf(" ") + 1).Trim();
            }
            else
            {
                repost_wall_group = repost_wall_posts;
                repost_wall_posts = "";
            }

            if (repost_wall_posts.Length == 0)
                repost_wall_posts = "1,";

            if (repost_wall_group.Length > 0)
            {
                string sgID = ResolveID(repost_wall_group);
                if (!sgID.Equals("-1"))
                {
                    if (repost_wall_posts[repost_wall_posts.Length - 1] != ',')
                        repost_wall_posts += ",";
                    while (repost_wall_posts.Length > 0)
                    {
                        string sgid = repost_wall_posts.Substring(0, repost_wall_posts.IndexOf(',')).Trim();
                        repost_wall_posts = repost_wall_posts.Substring(repost_wall_posts.IndexOf(',') + 1).Trim();
                        if (sgid.Length > 0)
                        {
                            try
                            {
                                int lgid = Convert.ToInt32(sgid);
                                if (lgid < 1)
                                    return false;
                            }
                            catch
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }

            }
            return false;
        }

        private bool checkFormatGroups(string repost_wall_posts)
        {
            if (repost_wall_posts[repost_wall_posts.Length - 1] != ',')
                repost_wall_posts += ",";

            while (repost_wall_posts.Length > 0)
            {
                string sgid = repost_wall_posts.Substring(0, repost_wall_posts.IndexOf(',')).Trim();
                repost_wall_posts = repost_wall_posts.Substring(repost_wall_posts.IndexOf(',') + 1).Trim();
                if (sgid.Length > 0)
                {
                    string sgIDs = ResolveID(sgid);
                    if (sgIDs.Equals("-1"))
                        return false;
                }
            }
            return true;
        }

        private void tbRepostGroup_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                //if (FormMain.SocialNetwork == 1)
                //{
                //    Cursor = Cursors.WaitCursor;
                //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
                //    {
                //        Cursor = Cursors.Arrow;
                //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обработка Персонажей ВКонтакте");
                //        return;
                //    }
                //    Cursor = Cursors.Arrow;
                //}

                string value = "";
                if (FormMain.InputBox(this, "Репост из группы", "Введите ID группы и через пробел номера постов через запятую:", ref value) == DialogResult.OK)
                {
                    string sGroups = value.Trim();
                    if (sGroups.Length == 0)
                        return;
                    if (!checkFormatRepost(sGroups))
                    {
                        MessageBox.Show("Ошибка формата.\n\nФормат: ID_группы ID_поста1,...,ID_постаN\n\n(допустимо также не указывать ID постов, тогда будет репост поста 1, либо указать только один пост)", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            if (iSocialNetwork == 1)
                                continue;

                            DateTime dt = DateTime.Now;
                            if (sUID == iPersUserID.ToString() && FormMain.SocialNetwork == iSocialNetwork)
                            {
                                mFormMain.lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "REPOST_GROUP " + sGroups);
                            }
                            else
                            {
                                List<String> lstReceivedMessagesTemp = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"));
                                    lstReceivedMessagesTemp = new List<String>(srcFile);
                                }
                                lstReceivedMessagesTemp.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "REPOST_GROUP " + sGroups);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                            }
                        }
                    }

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();

                    MessageBox.Show("Выполнение операции запланировано. Запустите отмеченных Персонажей в ротацию для физического выполнения операции каждым Персонажем.", "Операции с Персонажами", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void tbRepostWall_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                //if (FormMain.SocialNetwork == 1)
                //{
                //    Cursor = Cursors.WaitCursor;
                //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
                //    {
                //        Cursor = Cursors.Arrow;
                //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обработка Персонажей ВКонтакте");
                //        return;
                //    }
                //    Cursor = Cursors.Arrow;
                //}

                string value = "";
                if (FormMain.InputBox(this, "Репост со стены Контактера", "Введите ID Контактера и через пробел номера постов через запятую:", ref value) == DialogResult.OK)
                {
                    string sGroups = value.Trim();
                    if (sGroups.Length == 0)
                        return;
                    if (!checkFormatRepost(sGroups))
                    {
                        MessageBox.Show("Ошибка формата.\n\nФормат: ID_контактера ID_поста1,...,ID_постаN\n\n(допустимо также не указывать ID постов, тогда будет репост поста 1, либо указать только один пост)", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            if (iSocialNetwork == 1)
                                continue;

                            DateTime dt = DateTime.Now;
                            if (sUID == iPersUserID.ToString() && FormMain.SocialNetwork == iSocialNetwork)
                            {
                                mFormMain.lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "REPOST_WALL " + sGroups);
                            }
                            else
                            {
                                List<String> lstReceivedMessagesTemp = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"));
                                    lstReceivedMessagesTemp = new List<String>(srcFile);
                                }
                                lstReceivedMessagesTemp.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "REPOST_WALL " + sGroups);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                            }
                        }
                    }

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();

                    MessageBox.Show("Выполнение операции запланировано. Запустите отмеченных Персонажей в ротацию для физического выполнения операции каждым Персонажем.", "Операции с Персонажами", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void tbFriendsAdd_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                //if (FormMain.SocialNetwork == 1)
                //{
                //    Cursor = Cursors.WaitCursor;
                //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
                //    {
                //        Cursor = Cursors.Arrow;
                //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обработка Персонажей ВКонтакте");
                //        return;
                //    }
                //    Cursor = Cursors.Arrow;
                //}

                string value = "";
                if (FormMain.InputBox(this, "Подружиться с Контактерами", "Введите ID Контактера или Контактеров через запятую:", ref value) == DialogResult.OK)
                {
                    string sGroups = value.Trim();
                    if (sGroups.Length == 0)
                        return;

                    if (!checkFormatGroups(sGroups))
                    {
                        MessageBox.Show("Ошибка формата.\n\nФормат: ID_контактера1,...ID_контактераN\n\n(допустимо также указать только одного Контактера)", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            if (iSocialNetwork == 1)
                                continue;

                            DateTime dt = DateTime.Now;
                            if (sUID == iPersUserID.ToString() && FormMain.SocialNetwork == iSocialNetwork)
                            {
                                mFormMain.lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "FRIENDS_ADD " + sGroups);
                            }
                            else
                            {
                                List<String> lstReceivedMessagesTemp = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"));
                                    lstReceivedMessagesTemp = new List<String>(srcFile);
                                }
                                lstReceivedMessagesTemp.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "FRIENDS_ADD " + sGroups);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                            }
                        }
                    }

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();

                    MessageBox.Show("Выполнение операции запланировано. Запустите отмеченных Персонажей в ротацию для физического выполнения операции каждым Персонажем.", "Операции с Персонажами", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void tbFriendsDelete_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                //if (FormMain.SocialNetwork == 1)
                //{
                //    Cursor = Cursors.WaitCursor;
                //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
                //    {
                //        Cursor = Cursors.Arrow;
                //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обработка Персонажей ВКонтакте");
                //        return;
                //    }
                //    Cursor = Cursors.Arrow;
                //}

                string value = "";
                if (FormMain.InputBox(this, "Раздружиться с Контактерами", "Введите ID Контактера или Контактеров через запятую:", ref value) == DialogResult.OK)
                {
                    string sGroups = value.Trim();
                    if (sGroups.Length == 0)
                        return;

                    if (!checkFormatGroups(sGroups))
                    {
                        MessageBox.Show("Ошибка формата.\n\nФормат: ID_контактера1,...ID_контактераN\n\n(допустимо также указать только одного Контактера)", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            if (iSocialNetwork == 1)
                                continue;

                            DateTime dt = DateTime.Now;
                            if (sUID == iPersUserID.ToString() && FormMain.SocialNetwork == iSocialNetwork)
                            {
                                mFormMain.lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "FRIENDS_DELETE " + sGroups);
                            }
                            else
                            {
                                List<String> lstReceivedMessagesTemp = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"));
                                    lstReceivedMessagesTemp = new List<String>(srcFile);
                                }
                                lstReceivedMessagesTemp.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "FRIENDS_DELETE " + sGroups);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                            }
                        }
                    }

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();

                    MessageBox.Show("Выполнение операции запланировано. Запустите отмеченных Персонажей в ротацию для физического выполнения операции каждым Персонажем.", "Операции с Персонажами", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void tbLikeGroup_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                //if (FormMain.SocialNetwork == 1)
                //{
                //    Cursor = Cursors.WaitCursor;
                //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
                //    {
                //        Cursor = Cursors.Arrow;
                //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обработка Персонажей ВКонтакте");
                //        return;
                //    }
                //    Cursor = Cursors.Arrow;
                //}

                string value = "";
                if (FormMain.InputBox(this, "Лайк записи в группе", "Введите ID группы и через пробел номера постов через запятую:", ref value) == DialogResult.OK)
                {
                    string sGroups = value.Trim();
                    if (sGroups.Length == 0)
                        return;
                    if (!checkFormatRepost(sGroups))
                    {
                        MessageBox.Show("Ошибка формата.\n\nФормат: ID_группы ID_поста1,...,ID_постаN\n\n(допустимо также не указывать ID постов, тогда будет репост поста 1, либо указать только один пост)", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            if (iSocialNetwork == 1)
                                continue;

                            DateTime dt = DateTime.Now;
                            if (sUID == iPersUserID.ToString() && FormMain.SocialNetwork == iSocialNetwork)
                            {
                                mFormMain.lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "LIKE_GROUP " + sGroups);
                            }
                            else
                            {
                                List<String> lstReceivedMessagesTemp = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"));
                                    lstReceivedMessagesTemp = new List<String>(srcFile);
                                }
                                lstReceivedMessagesTemp.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "LIKE_GROUP " + sGroups);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                            }
                        }
                    }

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();

                    MessageBox.Show("Выполнение операции запланировано. Запустите отмеченных Персонажей в ротацию для физического выполнения операции каждым Персонажем.", "Операции с Персонажами", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void tbLikeWall_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                //if (FormMain.SocialNetwork == 1)
                //{
                //    Cursor = Cursors.WaitCursor;
                //    if (AutorizeVK(vkontakteLogin, vkontaktePassword) < 0)
                //    {
                //        Cursor = Cursors.Arrow;
                //        MessageBox.Show("Не удается авторизоваться в социальной сети ВКонтакте", "Обработка Персонажей ВКонтакте");
                //        return;
                //    }
                //    Cursor = Cursors.Arrow;
                //}

                string value = "";
                if (FormMain.InputBox(this, "Лайк записи на стене Контактера", "Введите ID Контактера и через пробел номера постов через запятую:", ref value) == DialogResult.OK)
                {
                    string sGroups = value.Trim();
                    if (sGroups.Length == 0)
                        return;
                    if (!checkFormatRepost(sGroups))
                    {
                        MessageBox.Show("Ошибка формата.\n\nФормат: ID_контактера ID_поста1,...,ID_постаN\n\n(допустимо также не указывать ID постов, тогда будет репост поста 1, либо указать только один пост)", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            if (iSocialNetwork == 1)
                                continue;

                            DateTime dt = DateTime.Now;
                            if (sUID == iPersUserID.ToString() && FormMain.SocialNetwork == iSocialNetwork)
                            {
                                mFormMain.lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "LIKE_WALL " + sGroups);
                            }
                            else
                            {
                                List<String> lstReceivedMessagesTemp = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"));
                                    lstReceivedMessagesTemp = new List<String>(srcFile);
                                }
                                lstReceivedMessagesTemp.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "LIKE_WALL " + sGroups);
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + "_contacter" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                            }
                        }
                    }

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();

                    MessageBox.Show("Выполнение операции запланировано. Запустите отмеченных Персонажей в ротацию для физического выполнения операции каждым Персонажем.", "Операции с Персонажами", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void toolStripButtonCopyToClipboard_Click(object sender, EventArgs e)
        {
            if (lvList.SelectedIndices.Count > 0)
            {
                int iSelIdx = lvList.SelectedIndices[0];
                String sValue = "";

                for (int i = 0; i < lvList.Items[iSelIdx].SubItems.Count; i++)
                    sValue += lvList.Items[iSelIdx].SubItems[i].Text + "\n";
                System.Windows.Forms.Clipboard.SetText(sValue);
            }
        }

        private void buttonExportPersonenServer_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                if (MessageBox.Show("Вы уверены, что хотите передать всех отмеченных Персонажей на Сервер?", "Передача Персонажей на Сервер", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int bSuccess = 0;
                    DisableAllButtons();
                    lvList.BeginUpdate();

                    List<String> listUIDs = new List<string>();
                    List<String> listExportedPersonen = new List<string>();
                    foreach (ListViewItem item in lvList.CheckedItems)
                    {
                        String sUID = item.SubItems[1].Text;
                        int iSocialNetwork = item.ImageIndex;
                        if (sUID.Equals(iPersUserID.ToString()) && iSocialNetwork == FormMain.SocialNetwork)
                            continue;

                        if (iSocialNetwork != 0)
                            continue;

                        listUIDs.Add(sUID);
                        listExportedPersonen.Add(PersonenList_GetUserRecord(iSocialNetwork, sUID));
                    }

                    if (listUIDs.Count == 0)
                    {
                        MessageBox.Show("Нет отмеченных Персонажей ВКонтакте для передачи на Сервер", "Передача Персонажей на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_exported_personen.txt"), listExportedPersonen, Encoding.UTF8);
                    bSuccess = mFormMain.tsmiExportPersonenFTP(listUIDs);

                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_exported_personen.txt")))
                        File.Delete(Path.Combine(FormMain.sDataPath, "_exported_personen.txt"));

                    // Delete from list when complete
                    if (bSuccess == 0)
                    {
                        foreach (ListViewItem item in lvList.CheckedItems)
                        {
                            String sUID = item.SubItems[1].Text;
                            int iSocialNetwork = item.ImageIndex;
                            if (sUID.Equals(iPersUserID.ToString()) && iSocialNetwork == FormMain.SocialNetwork)
                                continue;

                            if (iSocialNetwork != 0)
                                continue;

                            PersonenList_RemoveUser(iSocialNetwork, sUID);
                        }

                        foreach (ListViewItem item in lvList.CheckedItems)
                        {
                            String sUID = item.SubItems[1].Text;
                            int iSocialNetwork = item.ImageIndex;

                            if (sUID.Equals(iPersUserID.ToString()) && iSocialNetwork == FormMain.SocialNetwork)
                                continue;

                            if (iSocialNetwork != 0)
                                continue;

                            lvList.Items.Remove(item);
                        }
                    }

                    lvList.EndUpdate();
                    EnableAllButtons();
                    //FilterList(ClearFilter());

                    if (bSuccess != 2)
                    {
                        if (bSuccess == 1)
                            MessageBox.Show("Во время передачи Персонажей на Сервер произошла ошибка. Проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", "Передача Персонажей на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("Передача Персонажей на Сервер успешна выполнена", "Передача Персонажей на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
            }
        }

        private void buttonImportPersonenServer_Click(object sender, EventArgs e)
        {
            int bSuccess = mFormMain.tsmiImportPersonenFTP();
            if (bSuccess == 0)
            {
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_exported_personen.txt")))
                {

                    List<DBPerson> lstImportedPersonenList = new List<DBPerson>();
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_exported_personen.txt"));
                    foreach (string str in srcFile)
                    {
                        if (str == null)
                            continue;

                        if (str.Length == 0)
                            continue;

                        lstImportedPersonenList.Add(DBPerson.FromRecordString(0, str));
                    }

                    DisableAllButtons();
                    lvList.BeginUpdate();

                    foreach (DBPerson person in lstImportedPersonenList)
                    {
                        int idxList = PersonenList_AddUserToVisualList(PersonenList_AddUser(person.SocialNetwork, person.UserID, person.UserName, person.UserLogin, person.UserPassword), PersonenList_GetVisualListIdx(person.SocialNetwork, person.UserID));
                        if (idxList >= 0)
                        {
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);
                        }
                    }

                    lvList.EndUpdate();
                    EnableAllButtons();
                    MessageBox.Show("Загрузка Персонажей с Сервера успешна выполнена", "Загрузка Персонажей с Сервера", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void buttonExportPersonenAsContactersServer_Click(object sender, EventArgs e)
        {
            string value = "perscont" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + 
                "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_");
            string filename = Path.Combine(Application.StartupPath, value + FormMain.FTP_SERVER_CONT_NAME_POSTFIX);

            List<String> lstUsersToExport = new List<string>();

            String sColumnNames = "";
            for (int j = 0; j < 2; ++j)
            {
                sColumnNames += lvList.Columns[j].Text + ";";
                //lstcolumns.Add(column.Index.ToString() + "|" + column.DisplayIndex.ToString() + "|" + column.Width.ToString());
            }
            for (int j = 0; j < mFormMain.iContHarCount; ++j)
            {
                sColumnNames += mFormMain.sContHar[j, 1] + ";";
            }
            lstUsersToExport.Add(sColumnNames);

            foreach (ListViewItem item in lvList.CheckedItems)
            {
                String sColumnValues = "";
                for (int j = 0; j < 2; ++j)
                    sColumnValues += item.SubItems[j].Text + ";";

                for (int j = 0; j < mFormMain.iContHarCount; ++j)
                    sColumnValues += item.SubItems[j + 4].Text + ";";

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
                        MessageBox.Show("Во время экспорта Персонажей как Контактеров на Сервер произошла ошибка. Проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", "Экспорт Персонажей как Контактеров на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Экспорт Персонажей как Контактеров на Сервер успешно выполнен", "Экспорт Персонажей как Контактеров на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }

        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (lvList.CheckedItems.Count > 0)
            {
                if (MessageBox.Show("Вы хотите провести проверку активности для всех отмеченных Персонажей с копированием всех Контактёров от заблокированных Персонажей?", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_34", this.Name, "Проверка активности Персонажей"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DisableAllButtons();
                    bInterruptOperation = false;
                    button9.Text = "X";
                    button9.Enabled = true;

                    int _iCount = 0;
                    int _nCount = lvList.CheckedItems.Count;
                    this.pbProgress.Visible = true;
                    this.pbProgress.Minimum = 0;
                    this.pbProgress.Value = 0;
                    this.pbProgress.Maximum = _nCount;
                    this.pbProgress.Text = "...";

                    for (int k = 0; k < lvList.Items.Count; k++)
                    {
                        if (bInterruptOperation)
                            break;

                        if (lvList.Items[k].Checked)
                        {
                            int idxList = lvList.Items[k].Index;
                            lvList.SelectedIndices.Add(idxList);
                            lvList.EnsureVisible(idxList);

                            String sUID = lvList.Items[k].SubItems[1].Text;
                            String sUName = lvList.Items[k].SubItems[0].Text;
                            String sULogin = lvList.Items[k].SubItems[2].Text;
                            String sUPwd = lvList.Items[k].SubItems[3].Text;
                            int iSocialNetwork = lvList.Items[k].ImageIndex;

                            _iCount++;
                            this.pbProgress.Value = _iCount;
                            this.pbProgress.Text = _iCount.ToString() + " / " + _nCount.ToString();

                            string s14v = lvList.Items[k].SubItems[4 + 14].Text;
                            lvList.Items[k].SubItems[4 + 14].Text = "?";
                            Application.DoEvents();

                            if (iSocialNetwork == 0)
                                await SleepDelay(5000);

                            if (bInterruptOperation)
                            {
                                lvList.Items[k].SubItems[4 + 14].Text = s14v;
                                break;
                            }
                            List<String> lstContHarCurrent = new List<String>();
                            if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                                lstContHarCurrent = new List<String>(srcFile);
                                for (int i = 0; i < iContHarCount; i++)
                                {
                                    if (i == 14)
                                    {
                                        if (iSocialNetwork == 0)
                                        {
                                            bool bActivePersone = AutorizeVK(sULogin, sUPwd) >= 0;
                                            lstContHarCurrent[i] = "15|" + (bActivePersone ? "Active" : "Blocked");
                                            if (!bActivePersone)
                                                copyContactsToMasterPersone(sUID);
                                        }
                                        else if (iSocialNetwork == 1)
                                            lstContHarCurrent[i] = "15|" + "Active";
                                    }
                                }
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), k);
                            }
                        }
                    }
                    if (FormMain.SocialNetwork == 0 && iPersUserID >= 0)
                        AutorizeVK(mFormMain.userLogin, mFormMain.userPassword);

                    //button19.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "button19", this.Name, "Проверить активность отм.");
                    this.pbProgress.Visible = false;

                    bInterruptOperation = false;
                    button9.Text = ">";
                    EnableAllButtons();
                }
            }

        }

        private void copyContactsToMasterPersone(string currentPersoneID)
        {
            if (String.IsNullOrEmpty(currentPersoneID))
                return;

            // check VKontakte
            //if (SocialNetwork != 0)
            //    return;

            List<String> lstPersonenListVKontakte = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix(0) + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix(0) + ".txt"));
                    lstPersonenListVKontakte = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstPersonenListVKontakte = new List<String>();
                }
            }

            if (lstPersonenListVKontakte.Count == 0)
                return;

            List<String> lstCurrentPersoneContactsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix(0) + currentPersoneID + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix(0) + currentPersoneID + ".txt"));
                    lstCurrentPersoneContactsList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstCurrentPersoneContactsList = new List<String>();
                }
            }

            // check Contacts exists
            if (lstCurrentPersoneContactsList.Count == 0)
                return;

            // search Master Persone
            bool bFoundMaster = false;
            bool bFoundPersone = false;
            string masterPersoneID = "";
            string personeName = "";

            foreach (string record in lstPersonenListVKontakte)
            {
                if (String.IsNullOrEmpty(record))
                    continue;
                if (record.IndexOf("|") <= 0)
                    continue;

                string sUID = record.Substring(0, record.IndexOf("|"));

                if (sUID.Equals(currentPersoneID))
                {
                    personeName = record.Substring(record.IndexOf("|") + 1);
                    personeName = personeName.Substring(0, personeName.IndexOf("|"));
                    bFoundPersone = true;
                    if (bFoundMaster && bFoundPersone)
                        break;
                    else
                        continue;
                }

                List<String> lstPersoneData = new List<String>();

                if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(0) + sUID + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(0) + sUID + ".txt"));
                        lstPersoneData = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        lstPersoneData = new List<String>();
                    }
                }


                if (lstPersoneData.Count > 0)
                {
                    foreach (string data in lstPersoneData)
                    {
                        if (data.StartsWith("9|")) // Роль в общении
                        {
                            string value = data.Substring(data.IndexOf("|") + 1).Trim().ToLower();

                            if (value.Equals("мастер персонаж"))
                            {
                                masterPersoneID = sUID;
                                bFoundMaster = true;
                            }
                            break;
                        }
                    }//foreach (string data in lstPersoneData)
                }//if (lstPersoneData.Count>0)

                if (bFoundMaster && bFoundPersone)
                    break;
            }//foreach (string record in lstPersonenListVKontakte)

            if (bFoundMaster && bFoundPersone)
            {
                List<String> lstMasterPersoneContactsList = new List<String>();
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix(0) + masterPersoneID + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix(0) + masterPersoneID + ".txt"));
                        lstMasterPersoneContactsList = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        lstMasterPersoneContactsList = new List<String>();
                    }
                }

                bool bContactsAdded = false;
                foreach (string contact in lstCurrentPersoneContactsList)
                {
                    if (String.IsNullOrEmpty(contact))
                        continue;
                    if (contact.IndexOf("|") <= 0)
                        continue;

                    string sUID = contact.Substring(0, contact.IndexOf("|"));

                    List<String> lstContactData = new List<String>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix(0) + currentPersoneID + "_" + sUID + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix(0) + currentPersoneID + "_" + sUID + ".txt"));
                            lstContactData = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                            lstContactData = new List<String>();
                        }
                    }

                    // check Contact Data exists
                    if (lstContactData.Count == 0)
                        continue;

                    int iStatusField = -1;
                    for (int i = 0; i < lstContactData.Count; i++)
                    {
                        String contactData = lstContactData[i];

                        if (String.IsNullOrEmpty(contactData))
                            continue;
                        if (contactData.IndexOf("|") <= 0)
                            continue;

                        if (contactData.StartsWith("15|"))
                        {
                            iStatusField = i;
                            break;
                        }
                    }

                    if (iStatusField == -1)
                        continue;

                    lstContactData[iStatusField] = "15|" + personeName + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "    https://vk.com/id" + currentPersoneID;
                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix(0) + masterPersoneID + "_" + sUID + ".txt"), lstContactData, Encoding.UTF8);

                    // add contact to Master Persone Contacts List
                    bool bNeedAdd = true;
                    int iReplacedUsr = -1;
                    for (int i = 0; i < lstMasterPersoneContactsList.Count; i++)
                    {
                        String masterContact = lstMasterPersoneContactsList[i];

                        if (String.IsNullOrEmpty(masterContact))
                            continue;
                        if (masterContact.IndexOf("|") <= 0)
                            continue;

                        if (sUID.Equals(masterContact.Substring(0, masterContact.IndexOf("|"))))
                        {
                            bNeedAdd = false;
                            iReplacedUsr = i;
                            break;
                        }
                    }

                    if (bNeedAdd)
                        lstMasterPersoneContactsList.Add(contact);
                    else
                    {
                        if (iReplacedUsr >= 0)
                        {
                            lstMasterPersoneContactsList[iReplacedUsr] = contact;
                        }
                    }

                    bContactsAdded = true;
                }

                if (bContactsAdded)
                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix(0) + masterPersoneID + ".txt"), lstMasterPersoneContactsList, Encoding.UTF8);
            }

        }

        String[] copyedParameters = null;

        private void buttonCopyParameters_Click(object sender, EventArgs e)
        {
            //
            if (lvList.SelectedIndices.Count > 0)
            {
                int iSelIdx = lvList.SelectedIndices[0];
                String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;

                if (sUID.Length > 0)
                {
                    LoadContactParamersValues(iSocialNetwork, sUID);
                    copyedParameters = new string[iContHarCount];
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        copyedParameters[i] = GetContactParametersValue(sContHar[i, 0]);
                    }
                }
            }
            buttonPasteParameters.Enabled = copyedParameters != null && (lvList.SelectedItems.Count > 0 || lvList.CheckedItems.Count > 0);
        }

        private void updatePersName_Click(object sender, EventArgs e)
        {
            if (lvList.SelectedIndices.Count > 0)
            {
                var selectedIndexInList = lvList.SelectedIndices[0];
                var currentUserID = lvList.Items[selectedIndexInList].SubItems[1].Text;
                var socialNetwork = lvList.Items[selectedIndexInList].ImageIndex;
                var networkPrefix = "";
                FormEditPersName formEditPersName;
                //определяем идекс соцсети для поиска нужного файла
                switch (socialNetwork)
                {
                    case 3:
                        networkPrefix = "ti";
                        break;
                }
                var path = Path.Combine(Application.StartupPath, "Data");
                path = Path.Combine(path, "persone_name_" + networkPrefix + currentUserID + ".txt");
                if (currentUserID.Length > 0)
                {
                    var dbUserName = "";
                    if (File.Exists(path))
                    {
                        try
                        {
                            dbUserName = File.ReadAllText(path);
                        }
                        catch (Exception c)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", c);
                        }
                    }
                    if (dbUserName.Length > 0)
                    {
                        var data = dbUserName.Split('|');
                        var userFirstName = data[1];
                        var userLastName = data[2];
                        var photoPersURL = data[3];
                        formEditPersName = new FormEditPersName(userFirstName, userLastName, photoPersURL);
                    }
                    else { formEditPersName = new FormEditPersName(); }
                    if (formEditPersName.ShowDialog() == DialogResult.OK)
                    {
                        var result = $"{currentUserID}|{formEditPersName.Persone.FirstName}|{formEditPersName.Persone.LastName}|{formEditPersName.Persone.PhotoUrl}";
                        File.WriteAllText(path, result);
                        var name = formEditPersName.Persone.FirstName + " " + formEditPersName.Persone.LastName;
                        PersonenList_AddUserToVisualList(PersonenList_AddUser(socialNetwork, currentUserID, name,
                            lvList.Items[selectedIndexInList].SubItems[2].Text, lvList.Items[selectedIndexInList].SubItems[3].Text), selectedIndexInList);
                    }
                }
            }
        }

        private async void firstAuthorizationButton_Click(object sender, EventArgs e)
        {
            var interfaceListener = new InterfaceListener();
            if (lvList.SelectedIndices.Count <= 0)
            {
                var selectedIndexInList = lvList.SelectedIndices[0];
                var currentUserID = lvList.Items[selectedIndexInList].SubItems[1].Text;
                var socialNetwork = lvList.Items[selectedIndexInList].ImageIndex;
                var currentUserLogin = lvList.Items[selectedIndexInList].SubItems[2].Text;
                var networkPrefix = lvList.Items[selectedIndexInList].SubItems[13].Text.ToLower();
                if (networkPrefix.Length <= 0)
                {
                    MessageBox.Show("Обязательно заполните хозяина персонажа");
                    return;
                }
                switch (networkPrefix)
                {
                    case "tinder":
                        networkPrefix = "Tinder";
                        break;
                    case "whatsapp":
                        networkPrefix = "Whatsapp";
                        break;
                    case "telegam":
                        networkPrefix = "Telegram";
                        break;
                }
                var requestString = $"{networkPrefix}FirstAuthorization\nLogin: {currentUserLogin}\nId: {currentUserID}";
                interfaceListener.NilsaWriteToRequestFile(requestString);
                var response = JsonConvert.DeserializeObject<TinderResponse>(await interfaceListener.NilsaReadFromResponseFile());
                if (response.STATUS != 200) MessageBox.Show(response.MESSAGE, "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (response.STATUS == 200 && MessageBox.Show("Получилось авторизовать персонажа?", "Проверка авторизации персонажа", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //todo доделать запись кукис в пароль или самоописание или туда и туда пока
                    var saveCookiesRequest = $"SaveCookies\nId: {currentUserID}";
                    interfaceListener.NilsaWriteToRequestFile(saveCookiesRequest);
                    response = JsonConvert.DeserializeObject<TinderResponse>(await interfaceListener.NilsaReadFromResponseFile());
                    if (response.STATUS == 200) MessageBox.Show("Успешная авторизация", "Завершение авторизации", MessageBoxButtons.OK);
                    else MessageBox.Show(response.MESSAGE, "Завершение авторизации", MessageBoxButtons.OK);
                }

                if (response.STATUS == 200 && MessageBox.Show("Получилось авторизовать персонажа?", "Проверка авторизации персонажа",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    var saveCookiesRequest = $"DeleteCookies\nId: {currentUserID}";
                    interfaceListener.NilsaWriteToRequestFile(saveCookiesRequest);
                    response = JsonConvert.DeserializeObject<TinderResponse>(await interfaceListener.NilsaReadFromResponseFile());
                    if (response.STATUS == 200) MessageBox.Show("Данные удалены", "Удаление данных", MessageBoxButtons.OK);
                    else MessageBox.Show(response.MESSAGE, "Удаление данных", MessageBoxButtons.OK);
                }
            }
            else if (lvList.CheckedIndices.Count > 0)
            {

            }
        }

        
        
        private void buttonPasteParameters_Click(object sender, EventArgs e)
        {
            if (copyedParameters == null)
                return;
            //
            if (lvList.CheckedIndices.Count <= 0)
            {
                if (lvList.SelectedIndices.Count > 0)
                {
                    if (MessageBox.Show("Вы уверены, что хотите вставить скопированные характеристики для текущего Персонажа?", "Вставка характеристик Персонажей", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        int iSelIdx = lvList.SelectedIndices[0];
                        String sUID = lvList.Items[iSelIdx].SubItems[1].Text;
                        int iSocialNetwork = lvList.Items[iSelIdx].ImageIndex;

                        if (sUID.Length > 0)
                        {
                            LoadContactParamersValues(iSocialNetwork, sUID);

                            List<String> lstContHar = new List<String>();
                            for (int i = 0; i < iContHarCount; i++)
                            {
                                if (i == 4 || (i >= 6 && i <= 13))
                                    lstContHar.Add(sContHar[i, 0] + "|" + copyedParameters[i]);
                                else
                                    lstContHar.Add(sContHar[i, 0] + "|" + GetContactParametersValue(sContHar[i, 0]));
                            }
                            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHar, Encoding.UTF8);

                            PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, lvList.Items[iSelIdx].SubItems[0].Text, lvList.Items[iSelIdx].SubItems[2].Text, lvList.Items[iSelIdx].SubItems[3].Text), iSelIdx);
                        }
                    }
                }
            }
            else
            {
                if (lvList.CheckedItems.Count > 0)
                {

                    if (MessageBox.Show("Вы уверены, что хотите вставить скопированные характеристики для всех отмеченных Персонажей?", "Вставка характеристик Персонажей", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DisableAllButtons();

                        for (int k = 0; k < lvList.Items.Count; k++)
                        {
                            if (lvList.Items[k].Checked)
                            {
                                String sUID = lvList.Items[k].SubItems[1].Text;
                                String sUName = lvList.Items[k].SubItems[0].Text;
                                String sULogin = lvList.Items[k].SubItems[2].Text;
                                String sUPwd = lvList.Items[k].SubItems[3].Text;
                                int iSocialNetwork = lvList.Items[k].ImageIndex;

                                List<String> lstContHarCurrent = new List<String>();
                                if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt")))
                                {
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"));
                                    lstContHarCurrent = new List<String>(srcFile);
                                    for (int i = 0; i < iContHarCount; i++)
                                    {
                                        if (i == 4 || (i >= 6 && i <= 13))
                                            lstContHarCurrent[i] = sContHar[i, 0] + "|" + copyedParameters[i];
                                    }

                                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(iSocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);

                                    PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), k);
                                }

                            }
                        }

                        EnableAllButtons();
                    }
                }

            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (bDoImportContacts)
            {
                FormSelectSocialNetwork fssn = new FormSelectSocialNetwork();
                fssn.cbSocialNetwork.SelectedIndex = FormMain.SocialNetwork;
                if (fssn.ShowDialog() != DialogResult.OK)
                    return;
                int iSocialNetwork = fssn.cbSocialNetwork.SelectedIndex;

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

                            String value = str;
                            if (value.IndexOf("|") > 0)
                            {
                                String sUID = value.Substring(0, value.IndexOf("|")); // usrID
                                value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                                String sUName = value.Substring(0, value.IndexOf("|"));
                                value = value.Substring(value.IndexOf("|") + 1); // skip usrName
                                String sULogin = value.Substring(0, value.IndexOf("|")); // usrLogin
                                value = value.Substring(value.IndexOf("|") + 1); // skip usrLogin
                                String sUPwd = value;
                                if (Convert.ToUInt64(sUID) > 0)
                                {
                                    if (iSocialNetwork == 0)
                                    {
                                        sImportIDs += sUID + "|";
                                        PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName, sULogin, sUPwd), PersonenList_GetVisualListIdx(iSocialNetwork, sUID));
                                    }
                                    else if (iSocialNetwork == 1)
                                    {
                                        String srec = NILSA_getUserRecord(sUID);
                                        if (srec != null)
                                        {
                                            sImportIDs += sUID + "|";
                                            String sUName1 = NILSA_GetFieldFromStringRec(srec, 1);
                                            String sULogin1 = NILSA_GetFieldFromStringRec(srec, 2);
                                            String sUPwd1 = NILSA_GetFieldFromStringRec(srec, 3);

                                            PersonenList_AddUserToVisualList(PersonenList_AddUser(iSocialNetwork, sUID, sUName1, sULogin1, sUPwd1), PersonenList_GetVisualListIdx(iSocialNetwork, sUID));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Convert.ToUInt64(str) > 0)
                                    sImportIDs += str + "|";
                            }

                        }
                    }

                    if (iSocialNetwork == 0)
                    {
                        if (sImportIDs.Length > 0)
                        {
                            iImportMode = 4;
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

                            fe.iPersHarAttrCount = iContHarAttrCount;
                            fe.iPersHarCount = iContHarCount;
                            fe.sFilePrefix = "pers";
                            fe.Setup();

                            if (fe.ShowDialog() == DialogResult.OK) 
                            {
                                List<String> lstContHar = new List<String>();
                                for (int i = 0; i < iContHarCount; i++)
                                    lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iContHarAttrCount]);

                                if (lstFilterHar != null)
                                    StartImportContactsFromGroupThread("UIDs=" + sImportIDs, lstContHar, lstFilterHar);
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

        private void button16_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<String> lstUsersToExport = new List<string>();
                foreach (ListViewItem item in lvList.CheckedItems)
                {
                    String sName = item.SubItems[0].Text;
                    String sUID = item.SubItems[1].Text;
                    String sULogin = item.SubItems[2].Text;
                    String sUPwd = item.SubItems[3].Text;
                    lstUsersToExport.Add(sUID + "|" + sName + "|" + sULogin + "|" + sUPwd);
                }

                File.WriteAllLines(saveFileDialog.FileName, lstUsersToExport, Encoding.UTF8);
            }
        }


    }
}
