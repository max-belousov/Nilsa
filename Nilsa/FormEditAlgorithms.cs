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
    public partial class FormEditAlgorithms : Form
    {
        FormMain mFormMain;
        int iSelectedDBID;

        // Algorithms DB
        int iAlgorithmsDBMaxID;
        String sNewAlgName;
        List<String> lstUserEnabledAlgorithmsList;

        //Dictionary<int, AlgorithmsDBRecord> lstAlgorithmsDB;

        Dictionary<String, String>[] dictPairs;
        Dictionary<String, int> AllowedAdditionalThemes;

        private void LoadAlgorithmsAllowedAdditionalThemes()
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AllowedAdditionalThemes.Clear();

                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                string[] AllowedAdditionalThemesList = dbr.AllowedAdditionalThemesList.Split('~');

                foreach (String str in AllowedAdditionalThemesList)
                {
                    if (str.Length > 0)
                    {
                        String value = str;
                        String key = value.Substring(0, value.IndexOf("`"));
                        value = value.Substring(value.IndexOf("`") + 1);
                        if (key.Length > 0 && value.Length > 0)
                        {
                            try
                            {
                                int iValue = Convert.ToInt32(value);
                                AllowedAdditionalThemes.Add(key, iValue);
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private void SaveAlgorithmsAllowedAdditionalThemes()
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                String AllowedAdditionalThemesList = "";
                foreach (var pair in AllowedAdditionalThemes)
                {
                    AllowedAdditionalThemesList += pair.Key + "`" + pair.Value.ToString() + "~";
                }

                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.AllowedAdditionalThemesList = AllowedAdditionalThemesList;

                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        public void LoadAlgorithmsPairs()
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                for (int i = 0; i < FormMain.iMsgHarCount; i++)
                    dictPairs[i].Clear();

                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                List<String> lstList = new List<String>();
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_" + dbr.ID.ToString() + ".txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_" + dbr.ID.ToString() + ".txt"));
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
            }
        }

        public void SaveAlgorithmsDBList()
        {
            List<String> lstList = new List<String>();
            for (int j = 0; j < comboBoxAlgorithmsItems.Items.Count; j++)
                lstList.Add(((AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[j]).ToRecordString());

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"), lstList, Encoding.UTF8);
        }

        private void LoadDefaultAlgorithmThemes()
        {
            comboBoxDefaultAlgorithmTheme.SelectedIndex = -1;
            comboBoxDefaultAlgorithmTheme.Items.Clear();

            comboBoxDefaultAlgorithmTheme.Items.Add("");
            foreach (var pair in mFormMain.lstMessagesDB)
            {
                comboBoxDefaultAlgorithmTheme.Items.Add(pair.Key.ToString());
            }
        }


        public int ERROR_ALG_ID;
        private int LoadAlgorithmsDBList()
        {
            int retval = -1;
            int retvalERROR = -1;
            List<String> lstList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"));
                lstList = new List<String>(srcFile);
            }

            /*
            lstUserEnabledAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + mFormMain.iPersUserID.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + mFormMain.iPersUserID.ToString() + ".txt"));
                lstUserEnabledAlgorithmsList = new List<String>(srcFile);
            }
            //else
            //    lstUserEnabledAlgorithmsList.Add("0");
            */
            lstUserEnabledAlgorithmsList = readAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + mFormMain.iPersUserID.ToString() + ".txt"));

            iAlgorithmsDBMaxID = -1;
            //lstAlgorithmsDB = new Dictionary<int, AlgorithmsDBRecord>();
            comboBoxAlgorithmsItems.SelectedIndex = -1;
            comboBoxAlgorithmsItems.Items.Clear();

            foreach (String value in lstList)
            {
                AlgorithmsDBRecord dbr = AlgorithmsDBRecord.FromRecordString(value);

                if (dbr.ID > iAlgorithmsDBMaxID)
                    iAlgorithmsDBMaxID = dbr.ID;
                //lstAlgorithmsDB.Add(dbr.ID, dbr);
                comboBoxAlgorithmsItems.Items.Add(dbr);

                bool bCheck = false;
                if (lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                    bCheck = true;

                if (iSelectedDBID >= 0)
                    if (iSelectedDBID == dbr.ID)
                        retval = comboBoxAlgorithmsItems.Items.Count - 1;

                if (dbr.Name.ToLower().Equals("error"))
                {
                    retvalERROR = comboBoxAlgorithmsItems.Items.Count - 1;
                    ERROR_ALG_ID = dbr.ID;
                }
            }

            if (retval == -1)
                return retvalERROR;

            return retval;
        }

        public FormEditAlgorithms(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            sNewAlgName = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Новый алгоритм");

            comboBoxCompareLexicalLevel.Items.Clear();
            comboBoxCompareVectorLevel.Items.Clear();
            comboBoxCompareStarLevel.Items.Clear();
            for (int i = 0; i < 100; i++)
            {
                comboBoxCompareLexicalLevel.Items.Add(i.ToString());
                comboBoxCompareVectorLevel.Items.Add(i.ToString());
                comboBoxCompareStarLevel.Items.Add(i.ToString());
            }
            comboBoxCompareStarLevel.Items.Add("100");

            comboBox1.SelectedIndex = 1;
            dictPairs = new Dictionary<string, string>[FormMain.iMsgHarCount];
            AllowedAdditionalThemes = new Dictionary<string, int>();

            for (int i = 0; i < FormMain.iMsgHarCount; i++)
                dictPairs[i] = new Dictionary<string, string>();

            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            this.buttonSelect.Location= new Point(Screen.PrimaryScreen.WorkingArea.Size.Width - this.buttonSelect.Width - 30, 0);
            this.comboBox1.Location = new Point(Screen.PrimaryScreen.WorkingArea.Size.Width - this.buttonSelect.Width - this.comboBox1.Width - 30, 0);

        }

        public void Setup(int _iSelectedDBID, bool bVisualInterface)
        {
            mVisualInterfaceSetup = bVisualInterface;

            iSelectedDBID = _iSelectedDBID;
            comboBoxAlgorithmsItems.SelectedIndex = -1;
            comboBoxAlgorithmsItems_SelectedIndexChanged(null, null);

            LoadDefaultAlgorithmThemes();
            int idx = LoadAlgorithmsDBList();
            if (comboBoxAlgorithmsItems.Items.Count > 0)
                comboBoxAlgorithmsItems.SelectedIndex = idx >= 0 ? idx : 0;
            else
            {
                sNewAlgName = "ERROR";
                buttonAdd_Click(null, null);
                sNewAlgName = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Новый алгоритм");
            }

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            iAlgorithmsDBMaxID++;
            int[] imhc = new int[FormMain.iMsgHarCount];
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
                imhc[i] = mFormMain.iMsgHarKoef[i];
            int[] imht = new int[FormMain.iMsgHarCount];
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
            {
                imht[i] = (mFormMain.sMsgHar[i, 2] == "Строка" ? 0 : (mFormMain.sMsgHar[i, 2] == "Число" ? 1 : 2));
            }
            bool[] bmrv = new bool[FormMain.iMsgHarCount];
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
                bmrv[i] = false;

            int[] _iMarkerAlgorithmsID = new int[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _iMarkerAlgorithmsID[i] = -1;

            int[] _iMarkerContHarID = new int[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _iMarkerContHarID[i] = 0;

            String[] _sMarkerContHarValues = new String[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _sMarkerContHarValues[i] = "";

            int[] _iMarkerMsgHarID = new int[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _iMarkerMsgHarID[i] = 0;

            String[] _sMarkerMsgHarValues = new String[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _sMarkerMsgHarValues[i] = "";

            String[] _sMarkerInMessages = new String[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _sMarkerInMessages[i] = "";

            comboBoxAlgorithmsItems.Items.Add(new AlgorithmsDBRecord(iAlgorithmsDBMaxID, sNewAlgName, mFormMain.CompareLexicalLevel, mFormMain.CompareVectorLevel, imhc, imht, bmrv, _iMarkerAlgorithmsID, _iMarkerContHarID, _sMarkerContHarValues, _iMarkerMsgHarID, _sMarkerMsgHarValues, true, true, true, true, true, false, false, "", true, true, false, true, _sMarkerInMessages, true, true, true, true, false,"", false, true, 90));
            comboBoxAlgorithmsItems.SelectedIndex = comboBoxAlgorithmsItems.Items.Count - 1;
            SaveAlgorithmsDBList();

            LoadTimersSettings(iAlgorithmsDBMaxID);
            LoadTextSearchSettings(iAlgorithmsDBMaxID);
            LoadFormEditPersHarValues(iAlgorithmsDBMaxID, 2);
        }

        private void comboBoxAlgorithmsItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];

                LoadAlgorithmsPairs();
                LoadAlgorithmsAllowedAdditionalThemes();

                comboBoxCompareLexicalLevel.SelectedIndex = dbr.CompareLexicalLevel;
                comboBoxCompareVectorLevel.SelectedIndex = dbr.CompareVectorLevel;
                comboBoxCompareLexicalLevel.Enabled = true;
                comboBoxCompareVectorLevel.Enabled = true;
                comboBoxCompareStarLevel.SelectedIndex = dbr.CompareStarLevel;
                comboBoxCompareStarLevel.Enabled = true;

                checkBoxLockDuplicateSend.Checked = dbr.bLockDuplicateSend;
                checkBoxLockDuplicateSend.Enabled = true;
                checkBoxFriendsGet.Checked = dbr.bFriendsGet;
                checkBoxFriendsGet.Enabled = true;
                checkBoxFriendsAdd.Checked = dbr.bFriendsAdd;
                checkBoxFriendsAdd.Enabled = true;
                checkBoxFriendsMarker.Checked = dbr.bFriendsMarker;
                checkBoxFriendsMarker.Enabled = true;

                checkBoxGroupingOutMessages.Checked = dbr.bGroupingOutMessages;
                checkBoxGroupingOutMessages.Enabled = true;
                checkBoxIgnoreMessagesFromNotContacter.Checked = !dbr.bIgnoreMessagesFromNotContacter;
                checkBoxIgnoreMessagesFromNotContacter.Enabled = true;
                checkBoxChangeThemeAlgorithmINMessageTheme.Checked = dbr.ChangeThemeAlgorithmINMessageTheme;
                checkBoxChangeThemeAlgorithmINMessageTheme.Enabled = true;
                checkBoxChangeThemeAlgorithmNotStoreLastMsg.Checked = !dbr.ChangeThemeAlgorithmNotStoreLastMsg;
                checkBoxChangeThemeAlgorithmNotStoreLastMsg.Enabled = true;
                checkBoxIgnoreUnknownMessagesWOUnknownGeneration.Checked = dbr.IgnoreUnknownMessagesWOUnknownGeneration;
                checkBoxIgnoreUnknownMessagesWOUnknownGeneration.Enabled = true;
                checkBoxMergeInMessages.Checked = dbr.MergeInMessages;
                checkBoxMergeInMessages.Enabled = true;
                checkBoxUseSetKoefWithLexicalCompare.Checked = dbr.UseSetKoefWithLexicalCompare;
                checkBoxUseSetKoefWithLexicalCompare.Enabled = true;
                checkBoxSplitTextIntoSentences.Checked = dbr.SplitTextIntoSentences;
                checkBoxSplitTextIntoSentences.Enabled = true;
                checkBoxLinkAdditionalThemes.Checked = dbr.LinkAdditionalThemes;
                checkBoxLinkAdditionalThemes.Enabled = true;
                buttonAllowedThemesList.Enabled = true;
                checkBoxPlayReceiveMsg.Checked = dbr.PlayReceiveMsg;
                checkBoxPlayReceiveMsg.Enabled = true;
                checkBoxPlaySendMsg.Checked = dbr.PlaySendMsg;
                checkBoxPlaySendMsg.Enabled = true;
                checkBoxPlayMarker.Checked = dbr.PlayMarker;
                checkBoxPlayMarker.Enabled = true;
                checkBoxShowErrorDetails.Checked = dbr.ShowErrorDetails;
                checkBoxShowErrorDetails.Enabled = true;


                checkBoxNotGenerateUnknownMessages.Checked = !dbr.bNotGenerateUnknownMessages;
                checkBoxNotGenerateUnknownMessages.Enabled = true;
                int iharval = comboBoxDefaultAlgorithmTheme.Items.IndexOf(dbr.DefaultAlgorithmTheme);
                comboBoxDefaultAlgorithmTheme.SelectedIndex = iharval;
                comboBoxDefaultAlgorithmTheme.Enabled = true;

                buttonRename.Enabled = true;
                buttonDelete.Enabled = true;
                buttonCopy.Enabled = true;

                buttonSelect.Enabled = lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString());

                buttonEnable.Enabled = !dbr.Name.ToUpper().Equals("ERROR");
                buttonEnableToAll.Enabled = !dbr.Name.ToUpper().Equals("ERROR");

                if (lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                {
                    buttonEnable.BackColor = SystemColors.Control;
                    buttonEnable.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Запретить алгоритм");
                }
                else
                {
                    buttonEnable.BackColor = SystemColors.ControlLight;
                    buttonEnable.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Разрешить алгоритм");
                }

                if (CheckEnabledAlgForAllUsers(dbr.ID.ToString()))
                {
                    buttonEnableToAll.BackColor = SystemColors.Control;
                    buttonEnableToAll.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Запретить для всех");
                }
                else
                {
                    buttonEnableToAll.BackColor = SystemColors.ControlLight;
                    buttonEnableToAll.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Разрешить для всех");
                }

                buttonTimersSettings.Enabled = true;
                buttonLexicalCompare.Enabled = true;
                buttonAlgorithmMarkers.Enabled = true;
                buttonVectorCompare.Enabled = true;
                buttonContacterSettings.Enabled = true;

                comboBoxAlgorithmsItems.Enabled = true;
            }
            else
            {
                for (int i = 0; i < FormMain.iMsgHarCount; i++)
                    dictPairs[i].Clear();
                AllowedAdditionalThemes.Clear();

                comboBoxCompareLexicalLevel.SelectedIndex = -1;
                comboBoxCompareVectorLevel.SelectedIndex = -1;
                comboBoxCompareLexicalLevel.Enabled = false;
                comboBoxCompareVectorLevel.Enabled = false;
                comboBoxCompareStarLevel.SelectedIndex = -1;
                comboBoxCompareStarLevel.Enabled = false;

                checkBoxLockDuplicateSend.Checked = false;
                checkBoxLockDuplicateSend.Enabled = false;
                checkBoxFriendsGet.Checked = false;
                checkBoxFriendsGet.Enabled = false;
                checkBoxFriendsAdd.Checked = false;
                checkBoxFriendsAdd.Enabled = false;
                checkBoxFriendsMarker.Checked = false;
                checkBoxFriendsMarker.Enabled = false;
                checkBoxGroupingOutMessages.Checked = false;
                checkBoxGroupingOutMessages.Enabled = false;
                checkBoxIgnoreMessagesFromNotContacter.Checked = false;
                checkBoxIgnoreMessagesFromNotContacter.Enabled = false;
                checkBoxChangeThemeAlgorithmINMessageTheme.Checked = false;
                checkBoxChangeThemeAlgorithmINMessageTheme.Enabled = false;
                checkBoxChangeThemeAlgorithmNotStoreLastMsg.Checked = false;
                checkBoxChangeThemeAlgorithmNotStoreLastMsg.Enabled = false;
                checkBoxIgnoreUnknownMessagesWOUnknownGeneration.Checked = false;
                checkBoxIgnoreUnknownMessagesWOUnknownGeneration.Enabled = false;
                checkBoxMergeInMessages.Checked = false;
                checkBoxMergeInMessages.Enabled = false;
                checkBoxUseSetKoefWithLexicalCompare.Checked = false;
                checkBoxUseSetKoefWithLexicalCompare.Enabled = false;
                checkBoxSplitTextIntoSentences.Checked = false;
                checkBoxSplitTextIntoSentences.Enabled = false;
                checkBoxLinkAdditionalThemes.Checked = false;
                checkBoxLinkAdditionalThemes.Enabled = false;
                buttonAllowedThemesList.Enabled = false;
                checkBoxPlayReceiveMsg.Checked = false;
                checkBoxPlayReceiveMsg.Enabled = false;
                checkBoxPlaySendMsg.Checked = false;
                checkBoxPlaySendMsg.Enabled = false;
                checkBoxPlayMarker.Checked = false;
                checkBoxPlayMarker.Enabled = false;
                checkBoxShowErrorDetails.Checked = false;
                checkBoxShowErrorDetails.Enabled = false;

                checkBoxNotGenerateUnknownMessages.Checked = false;
                checkBoxNotGenerateUnknownMessages.Enabled = false;
                comboBoxDefaultAlgorithmTheme.SelectedIndex = -1;
                comboBoxDefaultAlgorithmTheme.Enabled = false;

                buttonRename.Enabled = false;
                buttonDelete.Enabled = false;
                buttonCopy.Enabled = false;

                buttonSelect.Enabled = false;
                buttonEnable.Enabled = false;
                buttonEnableToAll.Enabled = false;
                buttonTimersSettings.Enabled = false;
                buttonContacterSettings.Enabled = false;
                buttonLexicalCompare.Enabled = false;
                buttonAlgorithmMarkers.Enabled = false;
                buttonVectorCompare.Enabled = false;

                comboBoxAlgorithmsItems.Enabled = false;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                string value = dbr.Name;

                if (dbr.Name.ToLower().Equals("error"))
                {
                    MessageBox.Show("Нельзя удалить алгоритм ERROR! Если Вы хотите удалить данный алгоритм, то переименуйте его, а потом удалите. Но не забудьте тогда создать новый базовый алгоритм с именем ERROR!", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_7", this.Name, "Запрет удаления алгоритма"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_" + dbr.ID.ToString() + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_algorithms_" + dbr.ID.ToString() + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_algtimers_settings_" + dbr.ID.ToString() + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_algtimers_settings_" + dbr.ID.ToString() + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_text_search_settings_" + dbr.ID.ToString() + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_text_search_settings_" + dbr.ID.ToString() + ".txt"));

                if (File.Exists(Path.Combine(FormMain.sDataPath, "FormEditPersHarValues_2_" + dbr.ID.ToString() + ".values")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "FormEditPersHarValues_2_" + dbr.ID.ToString() + ".values"));

                int isel = comboBoxAlgorithmsItems.SelectedIndex;
                comboBoxAlgorithmsItems.SelectedIndex = -1;
                comboBoxAlgorithmsItems.Items.RemoveAt(isel);

                if (isel < comboBoxAlgorithmsItems.Items.Count)
                    comboBoxAlgorithmsItems.SelectedIndex = isel;
                else if (comboBoxAlgorithmsItems.Items.Count > 0)
                    comboBoxAlgorithmsItems.SelectedIndex = comboBoxAlgorithmsItems.Items.Count - 1;

                SaveAlgorithmsDBList();
            }
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                string value = dbr.Name;
                if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_8", this.Name, "Переименование алгоритма"), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Алгоритм:"), ref value) == DialogResult.OK)
                {
                    dbr.Name = value;
                    comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;

                    SaveAlgorithmsDBList();
                }
            }
        }

        private void comboBoxCompareLexicalLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCompareLexicalLevel.SelectedIndex >= 0)
            {
                comboBoxCompareLexicalLevel.SelectionStart = comboBoxCompareLexicalLevel.Text.Length;
                comboBoxCompareLexicalLevel.SelectionLength = 0;
                if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                    dbr.CompareLexicalLevel = comboBoxCompareLexicalLevel.SelectedIndex;
                    comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;

                    SaveAlgorithmsDBList();
                }
            }
        }

        private void comboBoxCompareVectorLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCompareVectorLevel.SelectedIndex >= 0)
            {
                comboBoxCompareVectorLevel.SelectionStart = comboBoxCompareVectorLevel.Text.Length;
                comboBoxCompareVectorLevel.SelectionLength = 0;
                if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                    dbr.CompareVectorLevel = comboBoxCompareVectorLevel.SelectedIndex;
                    comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;

                    SaveAlgorithmsDBList();
                }
            }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                if (comboBox1.SelectedIndex == 2)
                {
                    FormSelectAlgorithmsList fsal = new FormSelectAlgorithmsList(mFormMain);
                    fsal.Setup();

                    if (fsal.ShowDialog() != DialogResult.OK)
                        return;
                }

                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                mFormMain.adbrCurrent = dbr;
                for (int i = 0; i < FormMain.iMsgHarCount; i++)
                {
                    mFormMain.adbrCurrentDictPairs[i].Clear();
                    foreach (var pair in dictPairs[i])
                        mFormMain.adbrCurrentDictPairs[i].Add(pair.Key.ToLower(), pair.Value);
                }

                mFormMain.adbrCurrentAllowedAdditionalThemes.Clear();
                foreach (var pair in AllowedAdditionalThemes)
                    mFormMain.adbrCurrentAllowedAdditionalThemes.Add(pair.Key.ToLower(), pair.Value);

                mFormMain.LoadTimersSettings();
                mFormMain.LoadTextSearchSettings();
                DialogResult = DialogResult.OK;
            }
        }

        public bool SelectAlgorithm(bool bForce = true)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                int oldAldID = mFormMain.adbrCurrent.ID;
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                if (lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                {
                    if (oldAldID != dbr.ID || bForce)
                    {
                        mFormMain.adbrCurrent = dbr;
                        for (int i = 0; i < FormMain.iMsgHarCount; i++)
                        {
                            mFormMain.adbrCurrentDictPairs[i].Clear();
                            foreach (var pair in dictPairs[i])
                            {
                                try
                                {
                                    if (!mFormMain.adbrCurrentDictPairs[i].ContainsKey(pair.Key.ToLower()))
                                        mFormMain.adbrCurrentDictPairs[i].Add(pair.Key.ToLower(), pair.Value);
                                    else
                                        mFormMain.adbrCurrentDictPairs[i][pair.Key.ToLower()] = pair.Value;
                                }
                                catch
                                {

                                }
                            }
                        }

                        mFormMain.adbrCurrentAllowedAdditionalThemes.Clear();
                        foreach (var pair in AllowedAdditionalThemes)
                        {
                            try
                            {
                                if (!mFormMain.adbrCurrentAllowedAdditionalThemes.ContainsKey(pair.Key.ToLower()))
                                    mFormMain.adbrCurrentAllowedAdditionalThemes.Add(pair.Key.ToLower(), pair.Value);
                                else
                                    mFormMain.adbrCurrentAllowedAdditionalThemes[pair.Key.ToLower()] = pair.Value;
                            }
                            catch
                            {

                            }
                        }

                        mFormMain.LoadTimersSettings();
                        mFormMain.LoadTextSearchSettings();
                    }
                    return true;
                }
                else
                {
                    int _erroralg = getAlgorithmsIdx("ERROR");
                    if (_erroralg >= 0)
                    {
                        dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[_erroralg];
                        mFormMain.adbrCurrent = dbr;
                        for (int i = 0; i < FormMain.iMsgHarCount; i++)
                        {
                            mFormMain.adbrCurrentDictPairs[i].Clear();
                            foreach (var pair in dictPairs[i])
                            {
                                try
                                {
                                    if (!mFormMain.adbrCurrentDictPairs[i].ContainsKey(pair.Key.ToLower()))
                                        mFormMain.adbrCurrentDictPairs[i].Add(pair.Key.ToLower(), pair.Value);
                                    else
                                        mFormMain.adbrCurrentDictPairs[i][pair.Key.ToLower()] = pair.Value;
                                }
                                catch
                                {

                                }
                            }
                        }

                        mFormMain.adbrCurrentAllowedAdditionalThemes.Clear();
                        foreach (var pair in AllowedAdditionalThemes)
                        {
                            try
                            {
                                if (!mFormMain.adbrCurrentAllowedAdditionalThemes.ContainsKey(pair.Key.ToLower()))
                                    mFormMain.adbrCurrentAllowedAdditionalThemes.Add(pair.Key.ToLower(), pair.Value);
                                else
                                    mFormMain.adbrCurrentAllowedAdditionalThemes[pair.Key.ToLower()] = pair.Value;
                            }
                            catch
                            {

                            }
                        }

                        mFormMain.LoadTimersSettings();
                        mFormMain.LoadTextSearchSettings();
                    }
                    return false;
                }

            }
            return false;
        }

        private int getAlgorithmsID(String strName)
        {
            String queryName = strName.ToLower();

            foreach (AlgorithmsDBRecord dbr in comboBoxAlgorithmsItems.Items)
            {
                if (dbr.Name.ToLower().Equals(queryName))
                    return dbr.ID;
            }

            return -1;
        }

        private int getAlgorithmsIdx(String strName)
        {
            String queryName = strName.ToLower();

            for (int i = 0; i < comboBoxAlgorithmsItems.Items.Count; i++)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[i];

                if (dbr.Name.ToLower().Equals(queryName))
                    return i;
            }

            return -1;
        }

        public void applyCloseButtonClick()
        {
            DialogResult = DialogResult.Cancel;
            if (iSelectedDBID >= 0)
            {
                if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                    if (dbr.ID == iSelectedDBID)
                    {
                        comboBox1.SelectedIndex = 3;
                        mFormMain.adbrCurrent = dbr;
                        for (int i = 0; i < FormMain.iMsgHarCount; i++)
                        {
                            mFormMain.adbrCurrentDictPairs[i].Clear();
                            foreach (var pair in dictPairs[i])
                                mFormMain.adbrCurrentDictPairs[i].Add(pair.Key.ToLower(), pair.Value);
                        }

                        mFormMain.adbrCurrentAllowedAdditionalThemes.Clear();
                        foreach (var pair in AllowedAdditionalThemes)
                            mFormMain.adbrCurrentAllowedAdditionalThemes.Add(pair.Key.ToLower(), pair.Value);

                        mFormMain.LoadTimersSettings();
                        mFormMain.LoadTextSearchSettings();

                        DialogResult = DialogResult.OK;
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            applyCloseButtonClick();
        }

        /* Timers
        int DefaultTimerNewMessagesRereadCycle, DefaultTimerSendAnswerCycle, DefaultTimerSkipMessageCycle, DefaultTimerPersoneChangeCycle, DefaultTimerPhysicalSendCycle, DefaultTimerNewMessagesRereadDelayCycle;
        */
        int[] timersValues = new int[8];
        public void LoadTimersSettings(int ialgid)
        {
            timersValues[0] = 10;
            timersValues[1] = 2400;
            timersValues[2] = 20;
            timersValues[3] = 10;
            timersValues[4] = 240;
            timersValues[5] = 60;
            timersValues[6] = 10;
            timersValues[7] = 3600;

            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algtimers_settings_" + ialgid.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algtimers_settings_" + ialgid.ToString() + ".txt"));
                lstTS = new List<String>(srcFile);

                if (lstTS.Count > 5)
                {
                    for (int i=0;i<6;i++)
                        timersValues[i]= Convert.ToInt32(lstTS[i]);

                    if (lstTS.Count > 6) timersValues[6] = Convert.ToInt32(lstTS[6]);
                    if (lstTS.Count > 7) timersValues[7] = Convert.ToInt32(lstTS[7]);
                }
                else
                    SaveTimersSettings(ialgid);
            }
            else
                SaveTimersSettings(ialgid);
        }

        List<String> lstContHarAlgValues;
        private void SaveFormEditPersHarValues(int importmode, int submode)
        {
            if (lstContHarAlgValues == null)
                return;

            if (lstContHarAlgValues.Count > 0)
                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"), lstContHarAlgValues, Encoding.UTF8);
        }

        private void LoadFormEditPersHarValues(int importmode, int submode)
        {
            lstContHarAlgValues = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"));
                lstContHarAlgValues = new List<String>(srcFile);
            }
            else
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValues_" + Convert.ToString(1) + "_" + Convert.ToString(100000) + ".values")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValues_" + Convert.ToString(1) + "_" + Convert.ToString(100000) + ".values"));
                    lstContHarAlgValues = new List<String>(srcFile);
                }
                else
                {
                    for (int i = 0; i < mFormMain.iContHarCount; i++)
                        lstContHarAlgValues.Add("");
                }

            }
        }

        public void setTimersDialog(int ialgid)
        {
            LoadTimersSettings(ialgid);
            FormEditTimersSettings fe = new FormEditTimersSettings(mFormMain);
            fe.numericUpDown1.Value = timersValues[0];
            fe.numericUpDown2.Value = timersValues[1];
            fe.numericUpDown3.Value = timersValues[2];
            fe.numericUpDown4.Value = timersValues[3];
            fe.numericUpDown5.Value = timersValues[4];
            fe.numericUpDown6.Value = timersValues[5];
            fe.numericUpDown7.Value = timersValues[6];
            fe.numericUpDown8.Value = timersValues[7];

            if (fe.ShowDialog() == DialogResult.OK)
            {
                timersValues[0] = (int)fe.numericUpDown1.Value;
                timersValues[1] = (int)fe.numericUpDown2.Value;
                timersValues[2] = (int)fe.numericUpDown3.Value;
                timersValues[3] = (int)fe.numericUpDown4.Value;
                timersValues[4] = (int)fe.numericUpDown5.Value;
                timersValues[5] = (int)fe.numericUpDown6.Value;
                timersValues[6] = (int)fe.numericUpDown7.Value;
                timersValues[7] = (int)fe.numericUpDown8.Value;

                SaveTimersSettings(ialgid);

                if (fe.checkBoxApplyToAll.Checked)
                {
                    for (int j = 0; j < comboBoxAlgorithmsItems.Items.Count; j++)
                    {
                        AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[j];
                        if ((!dbr.Name.ToUpper().Equals("ERROR")) && lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                            SaveTimersSettings(dbr.ID);
                    }
                }

            }
        }

        private void SaveTimersSettings(int ialgid)
        {
            List<String> lstTS = new List<String>();
            for (int i = 0; i < 8; i++)
                lstTS.Add(timersValues[i].ToString());

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algtimers_settings_" + ialgid.ToString() + ".txt"), lstTS, Encoding.UTF8);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                int isel = comboBoxAlgorithmsItems.SelectedIndex;
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[isel];
                //String sDBAlg = dbr.ToRecordString();
                setTimersDialog(dbr.ID);
            }
        }

        String TextSearchFilteredChars, TextSearchIgnoredWords, TextSearchTextParsingChars;
        int TextSearchMinWordsLen;
        private void SaveTextSearchSettings(int ialgid)
        {
            List<String> lstTS = new List<String>();
            lstTS.Add(TextSearchFilteredChars);
            lstTS.Add(TextSearchIgnoredWords);
            lstTS.Add(TextSearchMinWordsLen.ToString());
            lstTS.Add(TextSearchTextParsingChars);

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_text_search_settings_" + ialgid.ToString() + ".txt"), lstTS, Encoding.UTF8);
        }

        public void LoadTextSearchSettings(int ialgid)
        {
            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_text_search_settings_" + ialgid.ToString() + ".txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_text_search_settings_" + ialgid.ToString() + ".txt"));
                lstTS = new List<String>(srcFile);

                if (lstTS.Count > 0) TextSearchFilteredChars = lstTS[0];
                if (lstTS.Count > 1) TextSearchIgnoredWords = lstTS[1];
                if (lstTS.Count > 2) TextSearchMinWordsLen = Convert.ToInt32(lstTS[2]);
                if (lstTS.Count > 3) TextSearchTextParsingChars = lstTS[3]; else TextSearchTextParsingChars = ".|...|!|?|<br>";

            }
            else
            {
                TextSearchFilteredChars = ".,";
                TextSearchIgnoredWords = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_10", this.Name, "из|под|с|по|в|");
                TextSearchMinWordsLen = 1;
                TextSearchTextParsingChars = ".|...|!|?|<br>";

                SaveTextSearchSettings(ialgid);
            }
        }

        public bool setLexicalAndVectorCompareSettingsDialog(int ialgid)
        {
            LoadTextSearchSettings(ialgid);
            FormEditTextSearchSettings fe = new FormEditTextSearchSettings(mFormMain);
            fe.numericUpDown1.Value = TextSearchMinWordsLen;
            fe.textBox1.Text = TextSearchFilteredChars;
            fe.textBox2.Text = TextSearchIgnoredWords;
            fe.textBox3.Text = TextSearchTextParsingChars;
            //fe.comboBox1.SelectedIndex = CompareLexicalLevel;
            //fe.comboBox2.SelectedIndex = CompareVectorLevel;

            if (fe.ShowDialog() == DialogResult.OK)
            {
                TextSearchMinWordsLen = (int)fe.numericUpDown1.Value;
                TextSearchFilteredChars = fe.textBox1.Text.Trim();
                TextSearchIgnoredWords = fe.textBox2.Text.Trim();
                TextSearchTextParsingChars = fe.textBox3.Text.Trim();
                //CompareLexicalLevel = fe.comboBox1.SelectedIndex;
                //CompareVectorLevel = fe.comboBox2.SelectedIndex;

                SaveTextSearchSettings(ialgid);
                if (fe.checkBoxApplyToAll.Checked)
                {
                    for (int j = 0; j < comboBoxAlgorithmsItems.Items.Count; j++)
                    {
                        AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[j];
                        if ((!dbr.Name.ToUpper().Equals("ERROR")) && lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                            SaveTextSearchSettings(dbr.ID);
                    }
                }

                return true;
            }
            return false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                int isel = comboBoxAlgorithmsItems.SelectedIndex;
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[isel];
                //String sDBAlg = dbr.ToRecordString();
                setLexicalAndVectorCompareSettingsDialog(dbr.ID);
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                int isel = comboBoxAlgorithmsItems.SelectedIndex;
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[isel];
                String sDBAlg = dbr.ToRecordString();
                int iSrcAlg = dbr.ID;

                List<String> lstList = new List<String>();
                for (int i = 0; i < FormMain.iMsgHarCount; i++)
                {
                    foreach (var pair in dictPairs[i])
                        lstList.Add(i.ToString() + "|" + pair.Key + "|" + pair.Value);
                }

                iAlgorithmsDBMaxID++;
                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_" + iAlgorithmsDBMaxID.ToString() + ".txt"), lstList, Encoding.UTF8);

                AlgorithmsDBRecord copyeddbr = AlgorithmsDBRecord.FromRecordString(sDBAlg);
                copyeddbr.ID = iAlgorithmsDBMaxID;
                copyeddbr.Name = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Копия")+" " + copyeddbr.Name;

                comboBoxAlgorithmsItems.Items.Add(copyeddbr);
                comboBoxAlgorithmsItems.SelectedIndex = comboBoxAlgorithmsItems.Items.Count - 1;

                SaveAlgorithmsDBList();

                LoadTimersSettings(iSrcAlg);
                SaveTimersSettings(copyeddbr.ID);
                LoadTextSearchSettings(iSrcAlg);
                SaveTextSearchSettings(copyeddbr.ID);
                LoadFormEditPersHarValues(iSrcAlg, 2);
                SaveFormEditPersHarValues(copyeddbr.ID, 2);

            }
        }

        private void checkBoxLockDuplicateSend_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.bLockDuplicateSend = checkBoxLockDuplicateSend.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxFriendsGet_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.bFriendsGet = checkBoxFriendsGet.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxFriendsAdd_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.bFriendsAdd = checkBoxFriendsAdd.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxNotGenerateUnknownMessages_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.bNotGenerateUnknownMessages = !checkBoxNotGenerateUnknownMessages.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }

        }

        private void comboBoxDefaultAlgorithmTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                if (comboBoxDefaultAlgorithmTheme.SelectedIndex >= 0)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                    if (!comboBoxDefaultAlgorithmTheme.Items[comboBoxDefaultAlgorithmTheme.SelectedIndex].ToString().Equals(dbr.DefaultAlgorithmTheme))
                    {
                        dbr.DefaultAlgorithmTheme = comboBoxDefaultAlgorithmTheme.Items[comboBoxDefaultAlgorithmTheme.SelectedIndex].ToString();
                        comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                        SaveAlgorithmsDBList();
                    }
                }
            }

        }

        private void checkBoxChangeThemeAlgorithmINMessageTheme_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.ChangeThemeAlgorithmINMessageTheme = checkBoxChangeThemeAlgorithmINMessageTheme.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }

        }

        private void checkBoxChangeThemeAlgorithmNotStoreLastMsg_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.ChangeThemeAlgorithmNotStoreLastMsg = !checkBoxChangeThemeAlgorithmNotStoreLastMsg.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }

        }

        private void checkBoxSplitTextIntoSentences_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.SplitTextIntoSentences = checkBoxSplitTextIntoSentences.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxLinkAdditionalThemes_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.LinkAdditionalThemes = checkBoxLinkAdditionalThemes.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxPlayReceiveMsg_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.PlayReceiveMsg = checkBoxPlayReceiveMsg.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxPlaySendMsg_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.PlaySendMsg = checkBoxPlaySendMsg.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxPlayMarker_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.PlayMarker = checkBoxPlayMarker.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxShowErrorDetails_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.ShowErrorDetails = checkBoxShowErrorDetails.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private static SolidBrush _Brush_BG_comboBoxAlgorithmsItems = new SolidBrush(SystemColors.ControlLight);
        private static SolidBrush _Brush_Window_comboBoxAlgorithmsItems = new SolidBrush(SystemColors.Window);
        private void comboBoxAlgorithmsItems_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            if (e.Index != -1)
            {
                e.Graphics.FillRectangle(selected ? _Brush_BG_comboBoxAlgorithmsItems : _Brush_Window_comboBoxAlgorithmsItems, new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height));
                // selected item 
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[e.Index];
                //Draw the image in combo box using its bound and ItemHeight 
                if (lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                    e.Graphics.DrawImage(global::Nilsa.Properties.Resources.star_orange_full, e.Bounds.X, e.Bounds.Y, 16, 16);
                else
                    e.Graphics.DrawImage(global::Nilsa.Properties.Resources.empty_16_16, e.Bounds.X, e.Bounds.Y, 16, 16);

                //we need to draw the item as string because we made drawmode to ownervariable
                e.Graphics.DrawString(dbr.Name, Font, Brushes.Black, new RectangleF(e.Bounds.X + 16, e.Bounds.Y + 1, comboBoxAlgorithmsItems.DropDownWidth, 16));
            }
            //draw rectangle over the item selected 
            e.DrawFocusRectangle();
        }

        private void buttonEnable_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                if (lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                {
                    lstUserEnabledAlgorithmsList.Remove(dbr.ID.ToString());
                    writeAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + mFormMain.iPersUserID.ToString() + ".txt"), lstUserEnabledAlgorithmsList);
                    //File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + mFormMain.iPersUserID.ToString() + ".txt"), lstUserEnabledAlgorithmsList, Encoding.UTF8);
                }
                else
                {
                    lstUserEnabledAlgorithmsList.Add(dbr.ID.ToString());
                    writeAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + mFormMain.iPersUserID.ToString() + ".txt"), lstUserEnabledAlgorithmsList);
                    //File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + mFormMain.iPersUserID.ToString() + ".txt"), lstUserEnabledAlgorithmsList, Encoding.UTF8);
                }
                int idx = comboBoxAlgorithmsItems.SelectedIndex;
                comboBoxAlgorithmsItems.SelectedIndex = -1;
                comboBoxAlgorithmsItems.SelectedIndex = idx;
            }
        }

        private List<String> readAllLines(String path)
        {
            List<string> lines = new List<string>();
            if (File.Exists(path))
            {
                using (FileStream myStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(myStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            lines.Add(reader.ReadLine());
                        }
                    }
                }
            }
            return lines;
        }

        private void writeAllLines(String path, List<String> lines)
        {
            using (FileStream myStream = File.Open(path, FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.Write))
            {
                using (StreamWriter myWriter = new StreamWriter(myStream))
                {
                    foreach (String str in lines)
                    {
                        myWriter.WriteLine(str); 
                    }
                }
            }
        }

        private bool mVisualInterfaceSetup = false;

        private bool CheckEnabledAlgForAllUsers(String salgid)
        {
            if (!mVisualInterfaceSetup)
                return false;

            foreach (String str in mFormMain.lstPersonenList)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                String sUID = str.Substring(0, str.IndexOf("|"));

                /*
                List<String> _lstUserEnabledAlgorithmsList = new List<String>();
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt"));
                    _lstUserEnabledAlgorithmsList = new List<String>(srcFile);
                }
                //else
                //    _lstUserEnabledAlgorithmsList.Add("0");
                */
                List<String> _lstUserEnabledAlgorithmsList = readAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt"));
                if (!_lstUserEnabledAlgorithmsList.Contains(salgid))
                    return false;
            }

            return true;
        }

        private void EnableDisableAlgForAllUsers(String salgid, bool action)
        {
            if (action)
            {
                if (!lstUserEnabledAlgorithmsList.Contains(salgid))
                    lstUserEnabledAlgorithmsList.Add(salgid);
            }
            else
                lstUserEnabledAlgorithmsList.Remove(salgid);

            foreach (String str in mFormMain.lstPersonenList)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                String sUID = str.Substring(0, str.IndexOf("|"));

                /*
                List<String> _lstUserEnabledAlgorithmsList = new List<String>();
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt"));
                    _lstUserEnabledAlgorithmsList = new List<String>(srcFile);
                }
                //else
                //    _lstUserEnabledAlgorithmsList.Add("0");
                */
                List<String> _lstUserEnabledAlgorithmsList = readAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt"));

                if (action)
                {
                    if (!_lstUserEnabledAlgorithmsList.Contains(salgid))
                        _lstUserEnabledAlgorithmsList.Add(salgid);
                }
                else
                    _lstUserEnabledAlgorithmsList.Remove(salgid);

                writeAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt"), _lstUserEnabledAlgorithmsList);
                //File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix() + sUID + ".txt"), _lstUserEnabledAlgorithmsList, Encoding.UTF8);
            }

        }

        private void buttonEnableToAll_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                if (CheckEnabledAlgForAllUsers(dbr.ID.ToString()))
                    EnableDisableAlgForAllUsers(dbr.ID.ToString(), false);
                else
                    EnableDisableAlgForAllUsers(dbr.ID.ToString(), true);

                int idx = comboBoxAlgorithmsItems.SelectedIndex;
                comboBoxAlgorithmsItems.SelectedIndex = -1;
                comboBoxAlgorithmsItems.SelectedIndex = idx;
            }
        }

        private void buttonApplyToAllSoundEffect_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbrSrc = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                bool bval_PlayReceiveMsg = dbrSrc.PlayReceiveMsg;
                bool bval_PlaySendMsg = dbrSrc.PlaySendMsg;
                bool bval_PlayMarker = dbrSrc.PlayMarker;

                for (int j = 0; j < comboBoxAlgorithmsItems.Items.Count; j++)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[j];
                    if ((!dbr.Name.ToUpper().Equals("ERROR")) && lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                    {
                        dbr.PlayReceiveMsg = bval_PlayReceiveMsg;
                        dbr.PlaySendMsg = bval_PlaySendMsg;
                        dbr.PlayMarker = bval_PlayMarker;
                        SaveAlgorithmsDBList();
                    }
                }
            }
        }

        private void buttonApplyToAllLexVectLevels_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbrSrc = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                int bval_CompareLexicalLevel = dbrSrc.CompareLexicalLevel;
                int bval_CompareVectorLevel = dbrSrc.CompareVectorLevel;
                int bval_CompareStarLevel = dbrSrc.CompareStarLevel;
                bool bval_ChangeThemeAlgorithmINMessageTheme = dbrSrc.ChangeThemeAlgorithmINMessageTheme;
                bool bval_ChangeThemeAlgorithmNotStoreLastMsg = dbrSrc.ChangeThemeAlgorithmNotStoreLastMsg;
                bool bval_SplitTextIntoSentences = dbrSrc.SplitTextIntoSentences;

                for (int j = 0; j < comboBoxAlgorithmsItems.Items.Count; j++)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[j];
                    if ((!dbr.Name.ToUpper().Equals("ERROR")) && lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                    {
                        dbr.CompareLexicalLevel = bval_CompareLexicalLevel;
                        dbr.CompareVectorLevel = bval_CompareVectorLevel;
                        dbr.ChangeThemeAlgorithmINMessageTheme = bval_ChangeThemeAlgorithmINMessageTheme;
                        dbr.ChangeThemeAlgorithmNotStoreLastMsg = bval_ChangeThemeAlgorithmNotStoreLastMsg;
                        dbr.SplitTextIntoSentences = bval_SplitTextIntoSentences;
                        dbr.CompareStarLevel = bval_CompareStarLevel;
                        SaveAlgorithmsDBList();
                    }
                }
            }
        }

        private void buttonApplyToAllFriendsRequest_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbrSrc = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                bool bval_bFriendsGet = dbrSrc.bFriendsGet;
                bool bval_bFriendsAdd = dbrSrc.bFriendsAdd;
                bool bval_bFriendsMarker = dbrSrc.bFriendsMarker;

                for (int j = 0; j < comboBoxAlgorithmsItems.Items.Count; j++)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[j];
                    if ((!dbr.Name.ToUpper().Equals("ERROR")) && lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                    {
                        dbr.bFriendsGet = bval_bFriendsGet;
                        dbr.bFriendsAdd = bval_bFriendsAdd;
                        dbr.bFriendsMarker = bval_bFriendsMarker;
                        SaveAlgorithmsDBList();
                    }
                }
            }
        }

        private void buttonApplyToAllSendMessages_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbrSrc = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                bool bval_bLockDuplicateSend = dbrSrc.bLockDuplicateSend;
                bool bval_bGroupingOutMessages = dbrSrc.bGroupingOutMessages;
                bool bval_ShowErrorDetails = dbrSrc.ShowErrorDetails;

                for (int j = 0; j < comboBoxAlgorithmsItems.Items.Count; j++)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[j];
                    if ((!dbr.Name.ToUpper().Equals("ERROR")) && lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                    {
                        dbr.bLockDuplicateSend = bval_bLockDuplicateSend;
                        dbr.bGroupingOutMessages = bval_bGroupingOutMessages;
                        dbr.ShowErrorDetails = bval_ShowErrorDetails;
                        SaveAlgorithmsDBList();
                    }
                }
            }
        }

        private void buttonApplyToAllReceiveMessages_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbrSrc = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                bool bval_bIgnoreMessagesFromNotContacter = dbrSrc.bIgnoreMessagesFromNotContacter;
                bool bval_bNotGenerateUnknownMessages = dbrSrc.bNotGenerateUnknownMessages;
                bool bval_IgnoreUnknownMessagesWOUnknownGeneration = dbrSrc.IgnoreUnknownMessagesWOUnknownGeneration;
                bool bval_LinkAdditionalThemes = dbrSrc.LinkAdditionalThemes;
                bool bval_MergeInMessages = dbrSrc.MergeInMessages;
                bool bval_UseSetKoefWithLexicalCompare = dbrSrc.UseSetKoefWithLexicalCompare;
                string bval_AllowedAdditionalThemesList = dbrSrc.AllowedAdditionalThemesList;

                for (int j = 0; j < comboBoxAlgorithmsItems.Items.Count; j++)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[j];
                    if ((!dbr.Name.ToUpper().Equals("ERROR")) && lstUserEnabledAlgorithmsList.Contains(dbr.ID.ToString()))
                    {
                        dbr.bIgnoreMessagesFromNotContacter = bval_bIgnoreMessagesFromNotContacter;
                        dbr.bNotGenerateUnknownMessages = bval_bNotGenerateUnknownMessages;
                        dbr.IgnoreUnknownMessagesWOUnknownGeneration = bval_IgnoreUnknownMessagesWOUnknownGeneration;
                        dbr.MergeInMessages = bval_MergeInMessages;
                        dbr.UseSetKoefWithLexicalCompare = bval_UseSetKoefWithLexicalCompare;
                        dbr.LinkAdditionalThemes = bval_LinkAdditionalThemes;
                        dbr.AllowedAdditionalThemesList = bval_AllowedAdditionalThemesList;
                        SaveAlgorithmsDBList();
                    }
                }
            }
        }

        private void comboBoxCompareLexicalLevel_TextUpdate(object sender, EventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(comboBoxCompareLexicalLevel.Text);
                if (value < 0 || value >= 100)
                    value = 0;
                comboBoxCompareLexicalLevel.SelectedIndex = -1;
                comboBoxCompareLexicalLevel.SelectedIndex = value;
            }
            catch
            {
                comboBoxCompareLexicalLevel.SelectedIndex = -1;
                comboBoxCompareLexicalLevel.SelectedIndex = 0;
            }
        }

        private void comboBoxCompareVectorLevel_TextUpdate(object sender, EventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(comboBoxCompareVectorLevel.Text);
                if (value < 0 || value >= 100)
                    value = 0;
                comboBoxCompareVectorLevel.SelectedIndex = -1;
                comboBoxCompareVectorLevel.SelectedIndex = value;
            }
            catch
            {
                comboBoxCompareVectorLevel.SelectedIndex = -1;
                comboBoxCompareVectorLevel.SelectedIndex = 0;
            }

        }

        private void buttonAllowedThemesList_Click(object sender, EventArgs e)
        {
            List<String> lstList=new List<string>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_messages_db.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_messages_db.txt"));
                lstList = new List<String>(srcFile);
            }

            List<String> lstThemes = new List<string>();
            List<int> lstThemesKoef = new List<int>();
            foreach (String str in lstList)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                int iID = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                String sName = str.Substring(str.IndexOf("|") + 1);
                if (sName.Length>0)
                {
                    lstThemes.Add(sName);
                    if (AllowedAdditionalThemes.Keys.Contains(sName.ToLower()))
                        lstThemesKoef.Add(AllowedAdditionalThemes[sName.ToLower()]);
                    else
                        lstThemesKoef.Add(0);
                }

            }

            int nListCount = lstThemes.Count;
            if (nListCount > 0)
            {
                FormEditThemesKoef fetk = new FormEditThemesKoef(mFormMain);
                fetk.nListCount = nListCount;
                fetk.stringThemesList = new string[nListCount];
                fetk.intThemesKoefList = new int[nListCount];
                for (int i = 0; i < nListCount; i++)
                {
                    fetk.stringThemesList[i] = lstThemes[i];
                    fetk.intThemesKoefList[i] = lstThemesKoef[i];
                }
                fetk.Setup();

                if (fetk.ShowDialog() == DialogResult.OK)
                {
                    AllowedAdditionalThemes.Clear();
                    for (int i = 0; i < nListCount; i++)
                    {
                        if (fetk.intThemesKoefList[i]>0)
                        {
                            AllowedAdditionalThemes.Add(fetk.stringThemesList[i].ToLower(), fetk.intThemesKoefList[i]);
                        }
                    }
                    SaveAlgorithmsAllowedAdditionalThemes();
                }
                
            }
        }

        private void checkBoxUseSetKoefWithLexicalCompare_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.UseSetKoefWithLexicalCompare = checkBoxUseSetKoefWithLexicalCompare.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxMergeInMessages_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.MergeInMessages = checkBoxMergeInMessages.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxIgnoreUnknownMessagesWOUnknownGeneration_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.IgnoreUnknownMessagesWOUnknownGeneration = checkBoxIgnoreUnknownMessagesWOUnknownGeneration.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxFriendsMarker_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.bFriendsMarker = checkBoxFriendsMarker.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void checkBoxGroupingOutMessages_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.bGroupingOutMessages = checkBoxGroupingOutMessages.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }
        }

        private void buttonAlgorithmMarkers_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                FormEditAlgorithmsMarkers feam = new FormEditAlgorithmsMarkers(mFormMain, this);
                feam.Text = dbr.Name;
                feam.Setup();
                feam.ShowDialog();
            }
        }

        private void buttonContacterSettings_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                int isel = comboBoxAlgorithmsItems.SelectedIndex;
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[isel];

                FormEditPersHarValues fe = new FormEditPersHarValues(mFormMain);
                fe.sPersHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                        fe.sPersHar[i, j] = mFormMain.sContHar[i, j];
                    fe.sPersHar[i, mFormMain.iContHarAttrCount] = "";
                }

                LoadFormEditPersHarValues(dbr.ID, 2);

                if (lstContHarAlgValues.Count > 0)
                {
                    for (int i = 0; i < mFormMain.iContHarCount; i++)
                    {
                        fe.sPersHar[i, mFormMain.iContHarAttrCount] = lstContHarAlgValues[i];
                    }
                }

                fe.iPersHarAttrCount = mFormMain.iContHarAttrCount;
                fe.iPersHarCount = mFormMain.iContHarCount;
                fe.sFilePrefix = "cont";
                fe.Setup();

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    lstContHarAlgValues = new List<String>();
                    for (int i = 0; i < mFormMain.iContHarCount; i++)
                        lstContHarAlgValues.Add(fe.sPersHar[i, mFormMain.iContHarAttrCount]);
                    SaveFormEditPersHarValues(dbr.ID, 2);
                }
            }
        }

        private void comboBoxCompareStarLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCompareStarLevel.SelectedIndex >= 0)
            {
                comboBoxCompareStarLevel.SelectionStart = comboBoxCompareStarLevel.Text.Length;
                comboBoxCompareStarLevel.SelectionLength = 0;
                if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
                {
                    AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                    dbr.CompareStarLevel = comboBoxCompareStarLevel.SelectedIndex;
                    comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;

                    SaveAlgorithmsDBList();
                }
            }

        }

        private void comboBoxCompareStarLevel_TextUpdate(object sender, EventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(comboBoxCompareStarLevel.Text);
                if (value < 0 || value > 100)
                    value = 0;
                comboBoxCompareStarLevel.SelectedIndex = -1;
                comboBoxCompareStarLevel.SelectedIndex = value;
            }
            catch
            {
                comboBoxCompareStarLevel.SelectedIndex = -1;
                comboBoxCompareStarLevel.SelectedIndex = 0;
            }

        }

        private void buttonVectorCompare_Click(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                FormEditAlgorithmsVectors feam = new FormEditAlgorithmsVectors(mFormMain, this);
                feam.Text = dbr.Name;
                feam.Setup();
                feam.ShowDialog();
                LoadAlgorithmsPairs();
            }
        }

        private void checkBoxIgnoreMessagesFromNotContacter_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex];
                dbr.bIgnoreMessagesFromNotContacter = !checkBoxIgnoreMessagesFromNotContacter.Checked;
                comboBoxAlgorithmsItems.Items[comboBoxAlgorithmsItems.SelectedIndex] = dbr;
                SaveAlgorithmsDBList();
            }

        }

    }
}
