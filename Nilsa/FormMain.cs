using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using CefSharp;
using System.Threading;
using System.Runtime.InteropServices;
using Nilsa.Data;
using VkNet.Model.Attachments;
using mevoronin.RuCaptchaNETClient;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Nilsa
{
    public partial class FormMain : Form
    {
        private static byte[] key = new byte[8] { 4, 2, 3, 1, 5, 6, 7, 8 };
        private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public bool bDebugVersion = false;
        public bool FormWebBrowserEnabled = true;
        public static int userAppId = 6157695;//4435644;//6268414;//6222822;//6222824;//6032776;//6183596; //4435644; // id приложения
        public static int[] userAppIds = { 6157695, 4435644, 6327257, 6327273, 6327284, 6327297, 6327306, 6327317, 6764475 };
        public static int userAppIdsPos = 0;
        public static int userAppIdsPosStart = 0;
        public string userLogin; // email для авторизации
        public string userPassword; // пароль
        public string userID; // пароль
        public string userName;
        public string userNameFamily;
        public string userNameName;
        int userSelectUserIdx;

        private string sessionStartTime;

        const bool externalCommandProcess = false; //!!!
        bool externalActivatedProcess = false; //!!!

        public const int MaxMarkerCount = 16;

        VkInterfaceCommands vkInterface;
        PersoneVkAttributes personeVkAtt;

        public static VkNet.VkApi api = null;
        public static String sDataPath;
        public static String sNILSAImagesPath;

        //public Label[] lblPersHarNames;
        PersoneLabel[] lblPersHarValues;
        public String[,] sPersHar;
        public int iPersHarCount = 16;
        public int iPersHarAttrCount = 4;
        List<String> lstPersHarValues;
        public long iPersUserID;

        //Label[] lblContHarNames;
        PersoneLabel[] lblContHarValues;
        public String[,] sContHar;
        public int iContHarCount = 16;
        public int iContHarAttrCount = 4;
        List<String> lstContHarValues;

        public String[,] sGroupHar;
        public int iGroupHarCount = 16;
        public int iGroupHarAttrCount = 4;

        public long iContUserID;
        public long iGroupAnswerID;
        public long iGroupAnswerPostID;
        public long iGroupAnswerCommentID;
        public string contName;
        public string contNameFamily;
        public string contNameName;

        public static long iMsgINMaxID = 0;
        public static long iMsgOUTMaxID = 0;
        public const int MSG_ID_COLUMN = 15;

        public static string strDB0Name = "BASIC";

        public String[,] sMsgHar;
        public int[] iMsgHarKoef;
        public int[] iCompareVectorsKoef;
        public int[] iCompareVectorsKoefOut;
        public String[] sMsgHarFilter;
        public static int iMsgHarCount = 16;
        public int iMsgHarAttrCount = 4;

        MessagesLabel[] lblMsgHarNames;
        NoPaddingButton[] lblEQInHarNames;
        Label[] lblEQInHarValues;
        NoPaddingButton[] lblEQOutHarNames;
        Label[] lblEQOutHarValues;
        Button[] lblMsgHarCompare;

        public AlgorithmsDBRecord adbrCurrent;
        public Dictionary<String, String>[] adbrCurrentDictPairs;
        public Dictionary<String, int> adbrCurrentAllowedAdditionalThemes;

        public List<String> lstReceivedMessages;
        public List<String> lstReceivedMessagesContacter;
        public List<String> lstReceivedMessagesGroups;
        public List<String> lstOutgoingMessages;
        public List<String> lstOutgoingMessagesDelayed;
        List<String> lstUserMessages;
        long iInMsgID;

        public List<String> lstEQInMessagesDB;
        public List<String> lstEQOutMessagesDB;
        public HashSet<String> hashsetEQInMessagesDB;
        public HashSet<String> hashsetEQOutMessagesDB;

        List<String> lstEQOutMessagesDBFiltered;
        List<String> lstEQInMessagesList;
        List<String> lstEQOutMessagesList;

        List<String> lstErrorsLogList;

        String sCurrentEQInMessageRecord;
        String sUndoMarkerCurrentEQInMessageRecord;
        String sCurrentEQOutMessageRecord;
        String sCurrentEQOutMessageRecordOut;
        String sCurrentContactLastMessageRecord = "";
        String[] sCurrentContactLastMessageFieldArray;
        String sCurrentContactLastMessageText = "";

        String sCurrentContactPrevLastMessageRecord = "";
        String sCurrentContactPrevLastMessageText = "";

        string sGroupAdditinalUsers = "";
        string sCurUserGroupAdditinalUsers = "";
        String sCurUserPrevMsgText = "";
        String sCurUserPrevMsgUID = "";
        String sCurUserPrevName = "";
        String sCurUserPrevGroupAnswerID = "-1";
        String sCurUserPrevGroupAnswerPostID = "-1";
        String sCurUserPrevGroupAnswerCommentID = "-1";

        public static String sLicenseUser;
        public static String sLicenseDate;
        public static String sLicenseUserFormatted;
        public static int iLicenseType;
        public const int LICENSE_TYPE_DEMO = 0;
        public const int LICENSE_TYPE_WORK = 1;
        public const int LICENSE_TYPE_PRO = 2;
        public const int LICENSE_TYPE_ADMIN = 3;
        public const int LICENSE_TYPE_NILS = 4;

        int timerReadCycle;
        int timerDefaultReadCycle;
        int timerWriteCycle;
        int timerDefaultWriteCycle;
        int timerAnswerWaitingCycle;
        int timerDefaultAnswerWaitingCycle;
        int timerChangePersoneCycle;
        int timerDefaultChangePersoneCycle;
        int[] timersValues = new int[8];

        bool bServiceStart = false;

        String TextSearchFilteredChars;
        String TextSearchIgnoredWords, TextSearchTextParsingChars;
        int TextSearchMinWordsLen;
        public int CompareLexicalLevel;
        public int CompareVectorLevel;

        List<String> lstContAllIDs;
        List<String> lstContCurSesIDs;
        long lCounterMessagesAll;
        long lCounterMessagesCurSes;
        long lCounterMessagesPersContAll;
        long lCounterMessagesPersContCurSes;

        public int iContactsGroupsMode = 0;
        public List<String> lstPersonenList;
        List<String> lstContactsList;
        public Dictionary<String, int> lstMessagesDB;
        int iMessagesDBMaxID;
        public List<String> lstMessagesDBUserList;
        bool bSetupDone;
        //String sLastRemovedMessage;
        bool bSelectContacterIndex;
        bool bSelectEQOutMsgIndex;
        bool bUnknownGenerated, bUnknownMessageGenerated;

        ulong cntC1, cntC2, cntC3, cntC4, cntC5, cntC6, cntC7, cntC8, cntC9;
        long persC8 = -1, contC9 = -1;
        ulong cntE1, cntE2, cntE3, cntE4, cntE5, cntE6, cntE7, cntE8, cntE9;
        ulong cntD1, cntD2, cntD3, cntD4, cntD5, cntD6, cntD7, cntD8, cntD9;

        public static Dictionary<String, int> dictMsgKoefStrings;
        public static Dictionary<int, String> dictMsgKoefValues;

        public List<String> lstPersoneChange;
        public List<String> lstPersoneChangeOriginal;
        public List<String> lstPersoneShuffle;

        string labelInMsgHarTitleValue_Text;
        string labelInEqMsgHarTitleValue_Text;
        string labelOutEqMsgHarTitleValue_Text;
        bool bSessionAnswerSended = false;

        public static int SocialNetwork = 0; // 0 - VK, 1 - Local

        string PersonColor1;
        string PersonColor2;
        string ContacterColor1;
        string ContacterColor2;

        public Color bgPerson1; // = System.Drawing.Color.FromArgb(0xbf, 0x9e, 0x0ff);
        public Color bgPerson2; // = System.Drawing.Color.FromArgb(0xd0, 0xb8, 0x0ff);
        public Color bgPerson3; // = System.Drawing.Color.FromArgb(0xd0, 0xb8, 0x0ff);
        public Color bgPerson4; // = System.Drawing.Color.FromArgb(0xd0, 0xb8, 0x0ff);
        public SolidBrush brushPerson1; // = new SolidBrush(bgPerson1);
        public SolidBrush brushPerson2; // = new SolidBrush(bgPerson2);
        public SolidBrush brushPerson3; // = new SolidBrush(bgPerson3);
        public SolidBrush brushPerson4; // = new SolidBrush(bgPerson4);

        public Color bgContact1; // = System.Drawing.Color.FromArgb(0xff, 0xea, 0xb1);
        public Color bgContact2; // = System.Drawing.Color.FromArgb(0xff, 0xf4, 0xd7);
        public Color bgContact3; // = System.Drawing.Color.FromArgb(0xff, 0xf4, 0xd7);
        public Color bgContact4; // = System.Drawing.Color.FromArgb(0xff, 0xf4, 0xd7);
        public SolidBrush brushContact1; // = new SolidBrush(bgContact1);
        public SolidBrush brushContact2; // = new SolidBrush(bgContact2);
        public SolidBrush brushContact3; // = new SolidBrush(bgContact3);
        public SolidBrush brushContact4; // = new SolidBrush(bgContact4);

        Image global_Nilsa_Properties_Resources_empty_image = global::Nilsa.Properties.Resources.empty_16_16;

        public string sDBDataItemsStrings_AgeUnknown;

        private void LoadColorSettings()
        {
            PersonColor1 = "BF9EFF";
            PersonColor2 = "D0B8FF";
            ContacterColor1 = "FFEAB1";
            ContacterColor2 = "FFF4D7";

            if (File.Exists(Path.Combine(Application.StartupPath, "_colors_settings.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_colors_settings.txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 3)
                    {
                        PersonColor1 = lstList[0].Trim().ToUpper();
                        PersonColor2 = lstList[1].Trim().ToUpper();
                        ContacterColor1 = lstList[2].Trim().ToUpper();
                        ContacterColor2 = lstList[3].Trim().ToUpper();
                    }
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    PersonColor1 = "BF9EFF";
                    PersonColor2 = "D0B8FF";
                    ContacterColor1 = "FFEAB1";
                    ContacterColor2 = "FFF4D7";
                }
            }

            if (!SetColorSettings())
            {
                PersonColor1 = "BF9EFF";
                PersonColor2 = "D0B8FF";
                ContacterColor1 = "FFEAB1";
                ContacterColor2 = "FFF4D7";
                SetColorSettings();
            }

            SaveColorSettings();
        }

        private bool SetColorSettings()
        {
            bool bres = false;
            try
            {
                bgPerson1 = System.Drawing.Color.FromArgb(int.Parse(PersonColor1.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(PersonColor1.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(PersonColor1.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                bgPerson2 = System.Drawing.Color.FromArgb(int.Parse(PersonColor2.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(PersonColor2.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(PersonColor2.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                bgPerson3 = System.Drawing.Color.FromArgb(int.Parse(PersonColor2.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(PersonColor2.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(PersonColor2.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                bgPerson4 = System.Drawing.Color.FromArgb(int.Parse(PersonColor2.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(PersonColor2.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(PersonColor2.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                brushPerson1 = new SolidBrush(bgPerson1);
                brushPerson2 = new SolidBrush(bgPerson2);
                brushPerson3 = new SolidBrush(bgPerson3);
                brushPerson4 = new SolidBrush(bgPerson4);

                bgContact1 = System.Drawing.Color.FromArgb(int.Parse(ContacterColor1.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(ContacterColor1.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(ContacterColor1.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                bgContact2 = System.Drawing.Color.FromArgb(int.Parse(ContacterColor2.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(ContacterColor2.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(ContacterColor2.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                bgContact3 = System.Drawing.Color.FromArgb(int.Parse(ContacterColor2.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(ContacterColor2.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(ContacterColor2.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                bgContact4 = System.Drawing.Color.FromArgb(int.Parse(ContacterColor2.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(ContacterColor2.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(ContacterColor2.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                brushContact1 = new SolidBrush(bgContact1);
                brushContact2 = new SolidBrush(bgContact2);
                brushContact3 = new SolidBrush(bgContact3);
                brushContact4 = new SolidBrush(bgContact4);
                bres = true;
            }
            catch
            {
            }

            return bres;
        }

        private void FileWriteAllLines(string path, List<string> list, Encoding encoding)
        {
            if (list == null || String.IsNullOrEmpty(path))
                return;

            try
            {
                File.WriteAllLines(path, list, encoding);
            }
            catch (Exception e)
            {
                ExceptionToLogList("FileWriteAllLines", path, e);
            }
        }

        private void SaveColorSettings()
        {
            List<String> lstList = new List<String>();
            lstList.Add(PersonColor1);
            lstList.Add(PersonColor2);
            lstList.Add(ContacterColor1);
            lstList.Add(ContacterColor2);

            FileWriteAllLines(Path.Combine(Application.StartupPath, "_colors_settings.txt"), lstList, Encoding.UTF8);
        }

        public Dictionary<string, string> userInterface = new Dictionary<string, string>();
        public string CurrentLanguage = "ru";
        private string ftpUser = "bYtFd4fBuOxmiEISoLcbgXfLMT+hId2rqmRa8De1vwoKytOE2NIEwynyxJ531k4yWV9Hdc7af+k=";//"tH3CksO/2EkbdvBS+MzI05KJgIDu1X1OTFNSVx8YIWMamunOT0J2UkhnubrcglkPteQMqhq9k7A=";//"tH3CksO/2EkbdvBS+MzI0wSQvre8iaVlaPoc1Obn+XJzgwA/ql2sew==";
        private bool bInitStart = false;

        public FormMain()
        {
            Application.EnableVisualStyles();
            sessionStartTime = DateTime.Now.ToString();
            bInitStart = true;
            //this.Location = new Point(0, 0);
            //this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            //this.WindowState = FormWindowState.Maximized;
            //this.Visible = false;

            LoadColorSettings();

            lstContAllIDs = new List<string>();
            lstContCurSesIDs = new List<string>();
            lCounterMessagesAll = 0;
            lCounterMessagesCurSes = 0;
            lCounterMessagesPersContAll = 0;
            lCounterMessagesPersContCurSes = 0;

            String strtmp = Decrypt(externalUser);
            externalUserLogin = strtmp.Substring(0, strtmp.IndexOf("|"));
            externalUserPassword = strtmp.Substring(strtmp.IndexOf("|") + 1);

            strtmp = Decrypt(ftpUser);
            FTP_SERVER_LOGIN = strtmp.Substring(0, strtmp.IndexOf("|"));
            FTP_SERVER_PASSWORD = strtmp.Substring(strtmp.IndexOf("|") + 1);

            lstPersoneChange = new List<string>();
            lstPersoneChangeOriginal = new List<string>();
            lstPersoneShuffle = new List<string>();

            adbrCurrent = new AlgorithmsDBRecord(-1, "", 0, 0, new int[0], new int[0], new bool[0], new int[0], new int[0], new String[0], new int[0], new String[0], true, true, true, true, true, false, false, "", true, true, false, true, new String[0], true, true, true, true, false, "", false, true, 90);
            adbrCurrentDictPairs = new Dictionary<string, string>[FormMain.iMsgHarCount];
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
                adbrCurrentDictPairs[i] = new Dictionary<string, string>();
            adbrCurrentAllowedAdditionalThemes = new Dictionary<string, int>();

            sMsgHarFilter = new String[iMsgHarCount];
            for (int i = 0; i < iMsgHarCount; i++)
                sMsgHarFilter[i] = "";
            bSetupDone = false;
            InitializeComponent();

            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);

            typeof(Control).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(progressBarRead.ProgressBar, new object[] { ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true });
            this.progressBarRead.Paint += new System.Windows.Forms.PaintEventHandler(this.progressBarRead_Paint);

            typeof(Control).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(progressBarWrite.ProgressBar, new object[] { ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true });
            this.progressBarWrite.Paint += new System.Windows.Forms.PaintEventHandler(this.progressBarWrite_Paint);

            typeof(Control).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(progressBarAnswerWaiting.ProgressBar, new object[] { ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true });
            this.progressBarAnswerWaiting.Paint += new System.Windows.Forms.PaintEventHandler(this.progressBarAnswerWaiting_Paint);

            typeof(Control).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(progressBarChangePersone.ProgressBar, new object[] { ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true });
            this.progressBarChangePersone.Paint += new System.Windows.Forms.PaintEventHandler(this.progressBarChangePersone_Paint);


            preloadLanguageSettings();
            if (File.Exists(Path.Combine(Application.StartupPath, "UserInterface." + CurrentLanguage + ".lng")))
                userInterface = NilsaUtils.Dictionary_Load(Path.Combine(Application.StartupPath, "UserInterface." + CurrentLanguage + ".lng"));

            dictMsgKoefStrings = new Dictionary<string, int>();
            dictMsgKoefValues = new Dictionary<int, string>();
            dictMsgKoefStrings.Add(NilsaUtils.Dictonary_GetText(userInterface, "textValues_1", this.Name, "Абсолютно важно"), 1111);
            dictMsgKoefStrings.Add(NilsaUtils.Dictonary_GetText(userInterface, "textValues_2", this.Name, "Очень важно"), 100);
            dictMsgKoefStrings.Add(NilsaUtils.Dictonary_GetText(userInterface, "textValues_3", this.Name, "Важно"), 10);
            dictMsgKoefStrings.Add(NilsaUtils.Dictonary_GetText(userInterface, "textValues_4", this.Name, "Не важно"), 1);
            dictMsgKoefStrings.Add(NilsaUtils.Dictonary_GetText(userInterface, "textValues_5", this.Name, "Абсолютно не важно"), 0);

            tableLayoutPanelPerson.BackColor = bgPerson1;
            tableLayoutPanelContact.BackColor = bgContact1;

            contextMenuStripVectorKoef.Items.Clear();
            contextMenuStripVectorKoefOut.Items.Clear();
            foreach (var vstr in dictMsgKoefStrings)
            {
                dictMsgKoefValues.Add(vstr.Value, vstr.Key);
                ToolStripMenuItem tsmi = new ToolStripMenuItem(vstr.Key, null, contextMenuStripVectorKoef_Click);
                tsmi.Tag = (Int32)vstr.Value;
                contextMenuStripVectorKoef.Items.Add(tsmi);

                ToolStripMenuItem tsmiout = new ToolStripMenuItem(vstr.Key, null, contextMenuStripVectorKoefOut_Click);
                tsmiout.Tag = (Int32)vstr.Value;
                contextMenuStripVectorKoefOut.Items.Add(tsmiout);
            }

            comboBoxCompareLexicalLevel.Items.Clear();
            comboBoxCompareVectorLevel.Items.Clear();
            for (int i = 0; i < 100; i++)
            {
                comboBoxCompareLexicalLevel.Items.Add(i.ToString());
                comboBoxCompareVectorLevel.Items.Add(i.ToString());
            }

            tableLayoutPanelContact.SuspendLayout();
            //tableLayoutPanelContact.ColumnCount = 18;
            //lblContHarNames = new Label[iContHarCount];
            lblContHarValues = new PersoneLabel[iContHarCount];
            for (int i = 0; i < iContHarCount; i++)
            {
                lblContHarValues[i] = new PersoneLabel();
                lblContHarValues[i].AutoSize = true;
                lblContHarValues[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                lblContHarValues[i].Location = new System.Drawing.Point(600, 22);
                lblContHarValues[i].Name = "labelContV" + i.ToString();
                lblContHarValues[i].Size = new System.Drawing.Size(11, 13);
                lblContHarValues[i].TabIndex = 2;
                lblContHarValues[i].Text = "";
                lblContHarValues[i].Dock = System.Windows.Forms.DockStyle.Fill;
                lblContHarValues[i].Tag = i;
                lblContHarValues[i].BackColor = bgContact1;
                //lblContHarValues[i].Paint += FormMain_PaintLabelContacter;
                //tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
                if (i < 4)
                {
                    //tableLayoutPanelContact.Controls.Add(lblContHarNames[i], 2, 14 + i);
                    tableLayoutPanelContact.Controls.Add(lblContHarValues[i], 1, 8 + i);
                }
                else
                {
                    //tableLayoutPanelContact.Controls.Add(lblContHarNames[i], 0, i - 1);
                    tableLayoutPanelContact.Controls.Add(lblContHarValues[i], 0, i - 4);
                }
            }
            tableLayoutPanelContact.PerformLayout();
            tableLayoutPanelContact.ResumeLayout();

            tableLayoutPanelPerson.SuspendLayout();
            //tableLayoutPanelPerson.ColumnCount = 18;
            //lblPersHarNames = new Label[iContHarCount];
            lblPersHarValues = new PersoneLabel[iContHarCount];
            for (int i = 0; i < iPersHarCount; i++)
            {
                lblPersHarValues[i] = new PersoneLabel();
                lblPersHarValues[i].AutoSize = true;
                lblPersHarValues[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                lblPersHarValues[i].Location = new System.Drawing.Point(600, 22);
                lblPersHarValues[i].Name = "labelPersV" + i.ToString();
                lblPersHarValues[i].Size = new System.Drawing.Size(11, 13);
                lblPersHarValues[i].TabIndex = 2;
                lblPersHarValues[i].Text = "";
                lblPersHarValues[i].Dock = System.Windows.Forms.DockStyle.Fill;
                lblPersHarValues[i].Tag = i;
                lblPersHarValues[i].BackColor = bgPerson1;
                //lblPersHarValues[i].Paint += FormMain_PaintLabelPersone;
                //tableLayoutPanelPerson.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
                if (i < 4)
                {
                    //tableLayoutPanelPerson.Controls.Add(lblPersHarNames[i], 0, 14 + i);
                    tableLayoutPanelPerson.Controls.Add(lblPersHarValues[i], 0, 8 + i);
                }
                else
                {
                    //tableLayoutPanelPerson.Controls.Add(lblPersHarNames[i], 2, i - 1);
                    tableLayoutPanelPerson.Controls.Add(lblPersHarValues[i], 1, i - 4);
                }
            }
            tableLayoutPanelPerson.PerformLayout();
            tableLayoutPanelPerson.ResumeLayout();

            lblMsgHarNames = new MessagesLabel[iMsgHarCount];
            lblEQInHarValues = new Label[iMsgHarCount];
            lblEQInHarNames = new NoPaddingButton[iMsgHarCount];
            lblEQOutHarValues = new Label[iMsgHarCount];
            lblEQOutHarNames = new NoPaddingButton[iMsgHarCount];
            lblMsgHarCompare = new Button[iMsgHarCount];

            tableLayoutPanelMsgHar.SuspendLayout();
            //tableLayoutPanel6.ColumnCount = 8;
            //tableLayoutPanel6.RowCount = 4;
            int iRowOffs = 0;
            for (int j = 0; j < iMsgHarCount; j++)
            {
                int idx = j;

                lblMsgHarNames[idx] = new MessagesLabel();
                lblMsgHarNames[idx].AutoSize = true;
                lblMsgHarNames[idx].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                lblMsgHarNames[idx].Location = new System.Drawing.Point(600, 22);
                lblMsgHarNames[idx].Name = "lblMsgHarNames_" + idx.ToString();
                lblMsgHarNames[idx].Size = new System.Drawing.Size(11, 13);
                lblMsgHarNames[idx].TabIndex = 2;
                lblMsgHarNames[idx].Text = "";
                lblMsgHarNames[idx].Dock = System.Windows.Forms.DockStyle.Fill;
                lblMsgHarNames[idx].Tag = (Int32)idx;
                tableLayoutPanelMsgHar.Controls.Add(lblMsgHarNames[idx], 0, iRowOffs + idx);

                lblEQInHarNames[idx] = new NoPaddingButton();
                lblEQInHarNames[idx].Dock = System.Windows.Forms.DockStyle.Fill;
                lblEQInHarNames[idx].Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
                lblEQInHarNames[idx].FlatAppearance.BorderSize = 0;
                lblEQInHarNames[idx].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                //lblEQInHarNames[idx].ForeColor = System.Drawing.Color.Transparent;
                lblEQInHarNames[idx].Image = null;// global::Nilsa.Properties.Resources.star_red_three_forths;
                lblEQInHarNames[idx].Location = new System.Drawing.Point(148, 27);
                lblEQInHarNames[idx].Name = "labelEQInN" + idx.ToString();
                //lblEQInHarNames[idx].Size = new System.Drawing.Size(16, 16);
                lblEQInHarNames[idx].Margin = new Padding(0);
                lblEQInHarNames[idx].TabIndex = 3;
                lblEQInHarNames[idx].Tag = (Int32)idx;
                lblEQInHarNames[idx].OwnerDrawText = "";
                lblEQInHarNames[idx].Text = "";
                lblEQInHarNames[idx].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                lblEQInHarNames[idx].UseVisualStyleBackColor = true;
                lblEQInHarNames[idx].BackColor = System.Drawing.Color.Transparent;
                lblEQInHarNames[idx].Click += lblEQInHarNames_Click;
                tableLayoutPanelMsgHar.Controls.Add(lblEQInHarNames[idx], 1, iRowOffs + idx);

                lblEQInHarValues[idx] = new Label();
                lblEQInHarValues[idx].AutoSize = true;
                lblEQInHarValues[idx].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                lblEQInHarValues[idx].Location = new System.Drawing.Point(600, 22);
                lblEQInHarValues[idx].Name = "labelEQInV" + idx.ToString();
                lblEQInHarValues[idx].Size = new System.Drawing.Size(11, 13);
                lblEQInHarValues[idx].TabIndex = 2;
                lblEQInHarValues[idx].Text = "";
                lblEQInHarValues[idx].Padding = new System.Windows.Forms.Padding(0, 1, 0, 3);
                lblEQInHarValues[idx].Dock = System.Windows.Forms.DockStyle.Fill;
                lblEQInHarValues[idx].Tag = (Int32)idx;
                lblEQInHarValues[idx].Image = global::Nilsa.Properties.Resources.down_arrow_square_silver_Shapes4FREE;
                lblEQInHarValues[idx].ImageAlign = System.Drawing.ContentAlignment.TopRight;
                lblEQInHarValues[idx].Cursor = Cursors.Hand;
                lblEQInHarValues[idx].BackColor = System.Drawing.Color.Transparent;
                lblEQInHarValues[idx].MouseClick += lblEQInHarValues_MouseClick;
                tableLayoutPanelMsgHar.Controls.Add(lblEQInHarValues[idx], 2, iRowOffs + idx);

                lblEQOutHarNames[idx] = new NoPaddingButton();
                lblEQOutHarNames[idx].Dock = System.Windows.Forms.DockStyle.Fill;
                lblEQOutHarNames[idx].FlatAppearance.BorderSize = 0;
                lblEQOutHarNames[idx].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                //lblEQOutHarNames[idx].ForeColor = System.Drawing.Color.Transparent;
                lblEQOutHarNames[idx].Image = null;//global::Nilsa.Properties.Resources.star_red_three_forths;
                lblEQOutHarNames[idx].Location = new System.Drawing.Point(148, 27);
                lblEQOutHarNames[idx].Name = "labelEQOutN" + idx.ToString();
                //lblEQOutHarNames[idx].Size = new System.Drawing.Size(16, 16);
                lblEQOutHarNames[idx].TabIndex = 3;
                lblEQOutHarNames[idx].Margin = new Padding(0);
                lblEQOutHarNames[idx].Tag = (Int32)idx;
                lblEQOutHarNames[idx].OwnerDrawText = "";
                lblEQOutHarNames[idx].Text = "";
                lblEQOutHarNames[idx].UseVisualStyleBackColor = true;
                lblEQOutHarNames[idx].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                lblEQOutHarNames[idx].BackColor = System.Drawing.Color.Transparent;
                lblEQOutHarNames[idx].Click += lblEQOutHarNames_Click;
                tableLayoutPanelMsgHar.Controls.Add(lblEQOutHarNames[idx], 4, iRowOffs + idx);

                lblEQOutHarValues[idx] = new Label();
                lblEQOutHarValues[idx].AutoSize = true;
                lblEQOutHarValues[idx].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                lblEQOutHarValues[idx].Location = new System.Drawing.Point(600, 22);
                lblEQOutHarValues[idx].Name = "labelEQOutV" + idx.ToString();
                lblEQOutHarValues[idx].Size = new System.Drawing.Size(11, 13);
                lblEQOutHarValues[idx].TabIndex = 2;
                lblEQOutHarValues[idx].Text = " ";
                lblEQOutHarValues[idx].Padding = new System.Windows.Forms.Padding(0, 1, 0, 3);
                lblEQOutHarValues[idx].Dock = System.Windows.Forms.DockStyle.Fill;
                lblEQOutHarValues[idx].Tag = (Int32)idx;
                lblEQOutHarValues[idx].Image = global::Nilsa.Properties.Resources.down_arrow_square_silver_Shapes4FREE;
                lblEQOutHarValues[idx].ImageAlign = System.Drawing.ContentAlignment.TopRight;
                lblEQOutHarValues[idx].Cursor = Cursors.Hand;
                lblEQOutHarValues[idx].BackColor = System.Drawing.Color.Transparent;
                lblEQOutHarValues[idx].MouseClick += lblEQOutHarValues_MouseClick;
                tableLayoutPanelMsgHar.Controls.Add(lblEQOutHarValues[idx], 3, iRowOffs + idx);

                lblMsgHarCompare[idx] = new Button();
                lblMsgHarCompare[idx].Dock = System.Windows.Forms.DockStyle.Top;
                lblMsgHarCompare[idx].FlatAppearance.BorderSize = 0;
                lblMsgHarCompare[idx].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                lblMsgHarCompare[idx].ForeColor = System.Drawing.Color.Transparent;
                lblMsgHarCompare[idx].Image = global_Nilsa_Properties_Resources_empty_image; //global::Nilsa.Properties.Resources.trafficlight_red_16;
                lblMsgHarCompare[idx].Location = new System.Drawing.Point(148, 27);
                lblMsgHarCompare[idx].Name = "lblMsgHarCompare" + idx.ToString();
                lblMsgHarCompare[idx].Size = new System.Drawing.Size(16, 16);
                lblMsgHarCompare[idx].TabIndex = 3;
                lblMsgHarCompare[idx].Tag = (Int32)idx;
                lblMsgHarCompare[idx].Text = "";
                lblMsgHarCompare[idx].UseVisualStyleBackColor = true;
                tableLayoutPanelMsgHar.Controls.Add(lblMsgHarCompare[idx], 5, iRowOffs + idx);


            }
            tableLayoutPanelMsgHar.CellPaint += new TableLayoutCellPaintEventHandler(tableLayoutPanelMsgHar_CellPaint);
            //tableLayoutPanelMsgMarkers.CellPaint += new TableLayoutCellPaintEventHandler(tableLayoutPanelMsgMarkers_CellPaint);
            //tableLayoutPanelTopMessages.ColumnStyles[2].Width = 85;
            //tableLayoutPanelBottomButtonsAndCounters.ColumnStyles[0].Width = 51;
            //label5.BackColor = System.Drawing.SystemColors.Control; // bgPerson3; //
            labelOutEqMsgHarTitleMarker.BackColor = bgPerson3;
            //label2.BackColor = System.Drawing.SystemColors.Control; //bgContact3; //
            labelInEqMsgHarTitleMarker.BackColor = bgContact3;
            tableLayoutPanelMsgHar.ColumnStyles[1].Width = 32;
            tableLayoutPanelMsgHar.ColumnStyles[4].Width = 32;
            tableLayoutPanelMsgHar.ColumnStyles[5].Width = 25;
            //tableLayoutPanelMsgMarkers.ColumnStyles[1].Width = 32;
            //tableLayoutPanelMsgMarkers.ColumnStyles[4].Width = 32;
            //tableLayoutPanelMsgMarkers.ColumnStyles[5].Width = 25;

            //tableLayoutPanelPerson.ColumnStyles[0].Width = 32;
            //tableLayoutPanelPerson.ColumnStyles[2].Width = 32;
            //tableLayoutPanelContact.ColumnStyles[0].Width = 32;
            //tableLayoutPanelContact.ColumnStyles[2].Width = 32;
            tableLayoutPanelMsgHar.PerformLayout();
            tableLayoutPanelMsgHar.ResumeLayout();

            buttonEditContHarValues.BackColor = bgContact1;
            buttonEditContHarValues.Paint += ButtonEditContHarValues_Paint;
            buttonEditPersHarValues.BackColor = bgPerson1;
            buttonEditPersHarValues.Paint += ButtonEditPersHarValues_Paint;

            listBoxInMsg.BackColor = bgContact4;
            listBoxOutMsg.BackColor = bgPerson4;

            toolStrip3.BackColor = bgContact2;
            toolStrip4.BackColor = bgPerson2;

            tableLayoutPanelTopMessages.CellPaint += new TableLayoutCellPaintEventHandler(tableLayoutPanelTopMessages_CellPaint);

            tableLayoutLeftBottomImages = new Image[9];
            tableLayoutLeftBottomLabels = new Label[9];
            tableLayoutLeftBottomLabels[0] = lblC1;
            tableLayoutLeftBottomLabels[1] = lblC2;
            tableLayoutLeftBottomLabels[2] = lblC3;
            tableLayoutLeftBottomLabels[3] = lblC4;
            tableLayoutLeftBottomLabels[4] = lblC5;
            tableLayoutLeftBottomLabels[5] = lblC6;
            tableLayoutLeftBottomLabels[6] = lblC7;
            tableLayoutLeftBottomLabels[7] = lblC8;
            tableLayoutLeftBottomLabels[8] = lblC9;

            for (int i = 0; i < 9; i++)
            {
                tableLayoutLeftBottomLabels[i].BackColor = System.Drawing.Color.Transparent;
                tableLayoutLeftBottomLabels[i].Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
                tableLayoutLeftBottomLabels[i].Margin = new System.Windows.Forms.Padding(0);
                string _picfile = Path.Combine(Application.StartupPath, "Images\\cnt_c_" + i.ToString() + ".png");
                if (File.Exists(_picfile))
                {
                    FileStream stream = new FileStream(_picfile, FileMode.Open, FileAccess.Read);
                    tableLayoutLeftBottomImages[i] = Image.FromStream(stream);
                    stream.Close();
                }
                else
                    tableLayoutLeftBottomImages[i] = Nilsa.Properties.Resources.lbl_c_bg;
            }
            tableLayoutLeftBottom.CellPaint += new TableLayoutCellPaintEventHandler(tableLayoutLeftBottom_CellPaint);

            tableLayoutRightBottomImages = new Image[24];
            tableLayoutRightBottomLabels = new Control[24];
            tableLayoutRightBottomLabels[0] = button2;
            tableLayoutRightBottomLabels[1] = lblB2;
            tableLayoutRightBottomLabels[2] = lblB3;
            tableLayoutRightBottomLabels[3] = btnB4;
            tableLayoutRightBottomLabels[4] = lblB5;
            tableLayoutRightBottomLabels[5] = lblB6;
            tableLayoutRightBottomLabels[6] = lblE1;
            tableLayoutRightBottomLabels[7] = lblE2;
            tableLayoutRightBottomLabels[8] = lblE3;
            tableLayoutRightBottomLabels[9] = lblE4;
            tableLayoutRightBottomLabels[10] = lblE5;
            tableLayoutRightBottomLabels[11] = lblE6;
            tableLayoutRightBottomLabels[12] = lblE7;
            tableLayoutRightBottomLabels[13] = lblE8;
            tableLayoutRightBottomLabels[14] = lblE9;
            tableLayoutRightBottomLabels[15] = lblD1;
            tableLayoutRightBottomLabels[16] = lblD2;
            tableLayoutRightBottomLabels[17] = lblD3;
            tableLayoutRightBottomLabels[18] = lblD4;
            tableLayoutRightBottomLabels[19] = lblD5;
            tableLayoutRightBottomLabels[20] = lblD6;
            tableLayoutRightBottomLabels[21] = lblD7;
            tableLayoutRightBottomLabels[22] = lblD8;
            tableLayoutRightBottomLabels[23] = lblD9;

            for (int i = 0; i < 24; i++)
            {
                tableLayoutRightBottomLabels[i].BackColor = System.Drawing.Color.Transparent;
                tableLayoutRightBottomLabels[i].Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
                tableLayoutRightBottomLabels[i].Margin = new System.Windows.Forms.Padding(0);
                tableLayoutRightBottomLabels[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
                string _picfile = Path.Combine(Application.StartupPath, "Images\\cnt_r_" + (i + 1).ToString() + ".png");
                if (File.Exists(_picfile))
                {
                    FileStream stream = new FileStream(_picfile, FileMode.Open, FileAccess.Read);
                    tableLayoutRightBottomImages[i] = Image.FromStream(stream);
                    stream.Close();
                }
                else
                    tableLayoutRightBottomImages[i] = Nilsa.Properties.Resources.lbl_r_bg;
            }
            tableLayoutRightBottom.CellPaint += new TableLayoutCellPaintEventHandler(tableLayoutRightBottom_CellPaint);

            bSelectContacterIndex = false;
            bSelectEQOutMsgIndex = false;
            lstEQInMessagesList = new List<string>();
            lstEQOutMessagesList = new List<string>();

            labelContMsgCount.Parent = buttonEditContHarValues;
            labelContMsgCount.BackColor = Color.FromArgb(128, bgContact4.R, bgContact4.G, bgContact4.B); //Color.Transparent;
            labelPersMsgCount.Parent = buttonEditPersHarValues;
            labelPersMsgCount.BackColor = Color.FromArgb(128, bgPerson4.R, bgPerson4.G, bgPerson4.B); //Color.Transparent;

            labelPersActivationCounter.Parent = buttonEditPersHarValues;
            labelPersActivationCounter.BackColor = Color.FromArgb(128, bgPerson4.R, bgPerson4.G, bgPerson4.B); //Color.Transparent;

            labelCont1FIO.Parent = buttonEditContHarValues;
            labelCont1FIO.BackColor = Color.FromArgb(128, bgContact4.R, bgContact4.G, bgContact4.B); //Color.Transparent;
            labelPers1FIO.Parent = buttonEditPersHarValues;
            labelPers1FIO.BackColor = Color.FromArgb(128, bgPerson4.R, bgPerson4.G, bgPerson4.B); //Color.Transparent;


            LoadErrorsLogList();


            //FormMsgImportExportSelectColumns form = new FormMsgImportExportSelectColumns(this);
            //form.ShowDialog();

            //---
            try
            {
                sDataPath = Path.Combine(Application.StartupPath, "Data");
                if (!System.IO.Directory.Exists(sDataPath))
                    System.IO.Directory.CreateDirectory(sDataPath);
            }
            catch (Exception e)
            {
                ExceptionToLogList("FormMain", sDataPath, e);
            }
            finally { }

            try
            {
                sNILSAImagesPath = Path.Combine(Application.StartupPath, "Images");
                if (!System.IO.Directory.Exists(sNILSAImagesPath))
                    System.IO.Directory.CreateDirectory(sNILSAImagesPath);
            }
            catch (Exception e)
            {
                ExceptionToLogList("FormMain", sNILSAImagesPath, e);
            }
            finally { }

            CheckLicense(true);
            LoadProgramSettings();

            if (userInterface.Count > 0)
            {
                NilsaUtils.Dictonary_ApplyAllControlsText(userInterface, this, this.Name, toolTipMessage);
            }
            NilsaUtils.Dictonary_AddAllControlsText(userInterface, this, this.Name, toolTipMessage);
            NilsaUtils.Dictionary_Save(userInterface, Path.Combine(Application.StartupPath, "UserInterface." + CurrentLanguage + ".lng"));
            sDBDataItemsStrings_AgeUnknown = "Не известен";

            //this.Text = "NILSA, v" + Application.ProductVersion + ((bDebugVersion || showSessionStartTime) ? " (" + sessionStartTime + ")" : "");
            setStandardCaption();

            //--- Init --- Begin ---
            string[] filesToDelete = System.IO.Directory.GetFiles(sDataPath, "_timers_settings_*.txt");
            foreach (string f in filesToDelete)
            {
                try
                {
                    File.Delete(f);
                }
                catch
                {

                }
            }

            filesToDelete = System.IO.Directory.GetFiles(Application.StartupPath, "captha_*.bmp");
            foreach (string f in filesToDelete)
            {
                try
                {
                    File.Delete(f);
                }
                catch
                {

                }
            }
            removeReinitDialogsWhenFreeFlag();
            removeChangeModeWhenFreeFlag();
            if (!File.Exists(Path.Combine(Application.StartupPath, "_initprogram_delete_messages_db.flag")))
            {
                List<string> _lstFlag = new List<string>();
                _lstFlag.Add("1");

                string[] files = System.IO.Directory.GetFiles(sDataPath, "_messages_db_*.txt");
                foreach (string f in files)
                {
                    File.Delete(f);
                }
                FileWriteAllLines(Path.Combine(Application.StartupPath, "_initprogram_delete_messages_db.flag"), _lstFlag, Encoding.UTF8);
            }

            if (!File.Exists(Path.Combine(Application.StartupPath, "_initprogram_rename_pershar.flag")))
            {
                List<string> _lstFlag = new List<string>();
                _lstFlag.Add("1");

                string[] files = System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues*_*.values");
                foreach (string f in files)
                {
                    if (f.Contains("FormEditPersHarValuesAlgorithmUpdate.values"))
                        continue;
                    if (f.Contains("FormEditPersHarValuesStatusUpdate.values"))
                        continue;

                    if (f.Contains("FormEditPersHarValuesPersonen"))
                    {
                        if (!f.Contains("FormEditPersHarValuesPersonen_"))
                            File.Move(f, f.Replace("FormEditPersHarValuesPersonen", "FormEditPersHarValuesPersonen_"));
                    }
                    else if (f.Contains("FormEditPersHarValuesEQIn"))
                    {
                        if (!f.Contains("FormEditPersHarValuesEQIn_"))
                            File.Move(f, f.Replace("FormEditPersHarValuesEQIn", "FormEditPersHarValuesEQIn_"));
                    }
                    else if (f.Contains("FormEditPersHarValuesEQOut"))
                    {
                        if (!f.Contains("FormEditPersHarValuesEQOut_"))
                            File.Move(f, f.Replace("FormEditPersHarValuesEQOut", "FormEditPersHarValuesEQOut_"));
                    }
                    else if (f.Contains("FormEditPersHarValues"))
                    {
                        if (!f.Contains("FormEditPersHarValues_"))
                            File.Move(f, f.Replace("FormEditPersHarValues", "FormEditPersHarValues_"));
                    }

                    FileWriteAllLines(Path.Combine(Application.StartupPath, "_initprogram_rename_pershar.flag"), _lstFlag, Encoding.UTF8);
                }
            }
            //--- Init --- End ---

            //LoadProgramCounters();
            LoadProgramCountersC1C2C3();
            LoadProgramCountersE1E2E3();
            LoadProgramCountersD1D2D3();
            UpdateProgramCountersInfoB2B5B6();
            iMsgINMaxID = NilsaUtils.LoadLongValue(0, 0);
            iMsgOUTMaxID = NilsaUtils.LoadLongValue(1, 0);

            webBrowserInMessageText.Dock = System.Windows.Forms.DockStyle.Fill;
            webBrowserOutEqMessageText.Dock = System.Windows.Forms.DockStyle.Fill;
            //String pwdpwd = GetHashMd5("Какой то текст, без разницы.");
            //MessageBox.Show(pwdpwd);
            webBrowserInMessageText.DocumentText = " ";
            webBrowserOutEqMessageText.DocumentText = " ";

            api = new VkApi();
            //SocialNetwork = 0;
            //toolStripButtonSocialNetworkChange_Click(null, null);
            //Setup();
            //this.Location = new Point(0, 0);
            //this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            //this.WindowState = FormWindowState.Maximized;
            //this.Visible = true;
            //this.Scale(0.5f);
        }

        Image[] tableLayoutLeftBottomImages;
        Label[] tableLayoutLeftBottomLabels;
        void tableLayoutLeftBottom_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row >= 1 && e.Row <= 3)
            {
                int idx = (e.Row - 1) * 3 + e.Column;
                Graphics g = e.Graphics;
                Rectangle r = e.CellBounds;
                g.FillRectangle(brushContact3, r);

                if (tableLayoutLeftBottomImages[idx] != null)
                    e.Graphics.DrawImage(tableLayoutLeftBottomImages[idx], r/*, ClientRectangle.Width, ClientRectangle.Height*/);

            }
        }

        Image[] tableLayoutRightBottomImages;
        Control[] tableLayoutRightBottomLabels;
        void tableLayoutRightBottom_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            int idx = e.Row * 3 + e.Column;
            Graphics g = e.Graphics;
            Rectangle r = e.CellBounds;
            g.FillRectangle(brushPerson3, r);

            if (tableLayoutRightBottomImages[idx] != null)
                e.Graphics.DrawImage(tableLayoutRightBottomImages[idx], r/*, ClientRectangle.Width, ClientRectangle.Height*/);
        }

        class PersoneLabel : Label
        {
            const int textoffs = 27;
            public PersoneLabel()
            {
                SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                //base.OnPaint(e);
                e.Graphics.FillRectangle(new SolidBrush(BackColor), Bounds);

                if (Image != null)
                    e.Graphics.DrawImage(Image, 0, 0/*, ClientRectangle.Width, ClientRectangle.Height*/);

                if (!string.IsNullOrEmpty(Text))
                {
                    /*
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(ClientRectangle.Left+25, ClientRectangle.Top, ClientRectangle.Width-25, ClientRectangle.Height), stringFormat);
                    */
                    TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;
                    TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(ClientRectangle.Left + ClientRectangle.Height + 2, ClientRectangle.Top, ClientRectangle.Width - ClientRectangle.Height - 2, ClientRectangle.Height), ForeColor, flags);
                }

            }
        }

        class MessagesLabel : Label
        {
            const int textoffs = 25;
            const int iwidth = 160;
            const int iheight = 20;
            public MessagesLabel()
            {
                SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                //base.OnPaint(e);
                e.Graphics.FillRectangle(new SolidBrush(BackColor), Bounds);

                if (Image != null)
                    e.Graphics.DrawImage(Image, 0, 0, iwidth, iheight);

                if (!string.IsNullOrEmpty(Text))
                {
                    /*
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(ClientRectangle.Left+25, ClientRectangle.Top, ClientRectangle.Width-25, ClientRectangle.Height), stringFormat);
                    */
                    TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;
                    TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(ClientRectangle.Left + ClientRectangle.Height + 2, ClientRectangle.Top, ClientRectangle.Width - ClientRectangle.Height - 2, ClientRectangle.Height), ForeColor, flags);
                }

            }
        }

        private void ButtonEditContHarValues_Paint(object sender, PaintEventArgs e)
        {
            float w = buttonEditContHarValues.ClientSize.Width;
            float h = buttonEditContHarValues.ClientSize.Height;

            e.Graphics.FillRectangle(brushContact1, buttonEditContHarValues.ClientRectangle);
            float ww = w;
            if (h < ww)
                ww = h;
            float offs = 0;
            if (h > w)
                offs = h - w;
            if (buttonEditContHarValues.BackgroundImage != null)
                e.Graphics.DrawImage(buttonEditContHarValues.BackgroundImage, 0, offs, ww, ww);
        }

        private void ButtonEditPersHarValues_Paint(object sender, PaintEventArgs e)
        {
            float w = buttonEditPersHarValues.ClientSize.Width;
            float h = buttonEditPersHarValues.ClientSize.Height;

            e.Graphics.FillRectangle(brushPerson1, buttonEditPersHarValues.ClientRectangle);
            float ww = w;
            if (h < ww)
                ww = h;
            float offs = 0;
            if (h > w)
                offs = h - w;
            if (buttonEditPersHarValues.BackgroundImage != null)
                e.Graphics.DrawImage(buttonEditPersHarValues.BackgroundImage, 0, offs, ww, ww);

        }

        public class NoPaddingButton : Button
        {
            private string ownerDrawText;
            public string OwnerDrawText
            {
                get { return ownerDrawText; }
                set { ownerDrawText = value; Invalidate(); }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(ownerDrawText))
                {
                    //StringFormat stringFormat = new StringFormat();
                    //stringFormat.Alignment = StringAlignment.Near;
                    //stringFormat.LineAlignment = StringAlignment.Near;

                    //e.Graphics.DrawString(ownerDrawText, Font, new SolidBrush(ForeColor), ClientRectangle, stringFormat);
                    TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine;
                    TextRenderer.DrawText(e.Graphics, ownerDrawText, Font, e.ClipRectangle, ForeColor, flags);

                }
            }
        }

        void tableLayoutPanelMsgHar_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row >= 0 && e.Row < 16)
            {
                if (e.Column == 1)
                {
                    Graphics g = e.Graphics;
                    Rectangle r = e.CellBounds;
                    g.FillRectangle(e.Row % 2 == 0 ? brushContact3 : brushContact1, r);
                }
                else if (e.Column == 2)
                {
                    Graphics g = e.Graphics;
                    Rectangle r = e.CellBounds;
                    g.FillRectangle(e.Row % 2 == 0 ? brushContact1 : brushContact3, r);
                }
                else if (e.Column == 3)
                {
                    Graphics g = e.Graphics;
                    Rectangle r = e.CellBounds;
                    g.FillRectangle(e.Row % 2 == 0 ? brushPerson3 : brushPerson1, r);
                }
                else if (e.Column == 4)
                {
                    Graphics g = e.Graphics;
                    Rectangle r = e.CellBounds;
                    g.FillRectangle(e.Row % 2 == 0 ? brushPerson1 : brushPerson3, r);
                }
                else if (e.Column == 5)
                {
                    Brush brush = brush_trafficlight_off_color;
                    if (lblMsgHarCompare[e.Row].BackColor == trafficlight_green_color)
                        brush = brush_trafficlight_green_color;
                    else if (lblMsgHarCompare[e.Row].BackColor == trafficlight_red_color)
                        brush = brush_trafficlight_red_color;
                    else if (lblMsgHarCompare[e.Row].BackColor == trafficlight_yellow_color)
                        brush = brush_trafficlight_yellow_color;
                    Graphics g = e.Graphics;
                    Rectangle r = e.CellBounds;
                    g.FillRectangle(brush, r);
                }
            }
        }

        void tableLayoutPanelMsgMarkers_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Column == 1 || e.Column == 2)
            {
                Graphics g = e.Graphics;
                Rectangle r = e.CellBounds;
                g.FillRectangle(brushContact3, r);
            }
            else if (e.Column == 3 || e.Column == 4)
            {
                Graphics g = e.Graphics;
                Rectangle r = e.CellBounds;
                g.FillRectangle(brushPerson3, r);
            }
        }

        void tableLayoutPanelTopMessages_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Column == 0)
            {
                Graphics g = e.Graphics;
                Rectangle r = e.CellBounds;
                g.FillRectangle(brushContact2, r);
            }
            else if (e.Column == 1)
            {
                Graphics g = e.Graphics;
                Rectangle r = e.CellBounds;
                g.FillRectangle(brushPerson2, r);
            }
        }

        private void LoadProgramCountersC1C2C3()
        {
            cntC1 = cntC2 = cntC3 = cntC7 = 0;
            if (File.Exists(Path.Combine(Application.StartupPath, "_program_counters_C1C2C3.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_counters_C1C2C3.txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0) cntC1 = Convert.ToUInt64(lstList[0]);
                    if (lstList.Count > 1) cntC2 = Convert.ToUInt64(lstList[1]);
                    if (lstList.Count > 2) cntC3 = Convert.ToUInt64(lstList[2]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    cntC1 = cntC2 = cntC3 = cntC7 = 0;
                }
            }

            UpdateProgramCountersInfoC1C2C3(false);
        }

        private void SaveProgramCountersC1C2C3()
        {
            List<String> lstList = new List<String>();
            lstList.Add(Convert.ToString(cntC1));
            lstList.Add(Convert.ToString(cntC2));
            lstList.Add(Convert.ToString(cntC3));
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_counters_C1C2C3.txt"), lstList, Encoding.UTF8);

        }

        private void UpdateProgramCountersInfoC1C2C3(bool bUpdateMonitorTime = true)
        {
            lblC1.Text = (cntC1 / 3600).ToString("00") + ":" + ((cntC1 / 60) % 60).ToString("00");
            lblC2.Text = (cntC2 / 3600).ToString("00") + ":" + ((cntC2 / 60) % 60).ToString("00");
            lblC3.Text = (cntC3 / 3600).ToString("00") + ":" + ((cntC3 / 60) % 60).ToString("00");
            lblC7.Text = (cntC7 / 3600).ToString("00") + ":" + ((cntC7 / 60) % 60).ToString("00");

            if (bUpdateMonitorTime)
                setMonitorTime(bServiceStart);
        }

        public bool isProgramInAutoMode = false;
        private void setMonitorTime(bool bRunned)
        {
            if (fwbVKontakte != null)
            {
                try
                {
                    fwbVKontakte.TopMost = bRunned;
                }
                catch
                {

                }
            }

            isProgramInAutoMode = bRunned;
            List<String> lstList = new List<String>();
            if (bRunned)
                lstList.Add(DateTime.Now.ToBinary().ToString());
            else
                lstList.Add("0");
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_monitor_value.txt"), lstList, Encoding.UTF8);
        }

        private void LoadProgramCountersE1E2E3()
        {
            cntE1 = cntE2 = cntE3 = cntE7 = cntE8 = cntE9 = 0;
            if (File.Exists(Path.Combine(Application.StartupPath, "_program_counters_E1E2E3.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_counters_E1E2E3.txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0) cntE1 = Convert.ToUInt64(lstList[0]);
                    if (lstList.Count > 1) cntE2 = Convert.ToUInt64(lstList[1]);
                    if (lstList.Count > 2) cntE3 = Convert.ToUInt64(lstList[2]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    cntE1 = cntE2 = cntE3 = cntE7 = cntE8 = cntE9 = 0;
                }
            }

            UpdateProgramCountersInfoE1E2E3();
        }

        private void SaveProgramCountersE1E2E3()
        {
            List<String> lstList = new List<String>();
            lstList.Add(Convert.ToString(cntE1));
            lstList.Add(Convert.ToString(cntE2));
            lstList.Add(Convert.ToString(cntE3));
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_counters_E1E2E3.txt"), lstList, Encoding.UTF8);

        }

        private void UpdateProgramCountersInfoE1E2E3()
        {
            lblE1.Text = cntE1.ToString();
            lblE2.Text = cntE2.ToString();
            lblE3.Text = cntE3.ToString();
            lblE7.Text = cntE7.ToString();
            lblE8.Text = cntE8.ToString();
            lblE9.Text = cntE9.ToString();
        }

        private void LoadProgramCountersE4E5E6()
        {
            cntE4 = cntE5 = cntE6 = 0;
            if (File.Exists(Path.Combine(Application.StartupPath, "_program_counters_E4E5E6_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_counters_E4E5E6_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0) cntE4 = Convert.ToUInt64(lstList[0]);
                    if (lstList.Count > 1) cntE5 = Convert.ToUInt64(lstList[1]);
                    if (lstList.Count > 2) cntE6 = Convert.ToUInt64(lstList[2]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    cntE4 = cntE5 = cntE6 = 0;
                }
            }

            UpdateProgramCountersInfoE4E5E6();
        }

        private void SaveProgramCountersE4E5E6()
        {
            List<String> lstList = new List<String>();
            lstList.Add(Convert.ToString(cntE4));
            lstList.Add(Convert.ToString(cntE5));
            lstList.Add(Convert.ToString(cntE6));
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_counters_E4E5E6_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstList, Encoding.UTF8);

        }

        private void UpdateProgramCountersInfoE4E5E6()
        {
            lblE4.Text = cntE4.ToString();
            lblE5.Text = cntE5.ToString();
            lblE6.Text = cntE6.ToString();
        }

        private void LoadProgramCountersD1D2D3()
        {
            cntD1 = cntD2 = cntD3 = cntD7 = cntD8 = cntD9 = 0;
            if (File.Exists(Path.Combine(Application.StartupPath, "_program_counters_D1D2D3.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_counters_D1D2D3.txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0) cntD1 = Convert.ToUInt64(lstList[0]);
                    if (lstList.Count > 1) cntD2 = Convert.ToUInt64(lstList[1]);
                    if (lstList.Count > 2) cntD3 = Convert.ToUInt64(lstList[2]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    cntD1 = cntD2 = cntD3 = cntD7 = cntD8 = cntD9 = 0;
                }
            }

            UpdateProgramCountersInfoD1D2D3();
        }

        private void SaveProgramCountersD1D2D3()
        {
            List<String> lstList = new List<String>();
            lstList.Add(Convert.ToString(cntD1));
            lstList.Add(Convert.ToString(cntD2));
            lstList.Add(Convert.ToString(cntD3));
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_counters_D1D2D3.txt"), lstList, Encoding.UTF8);

        }

        private void UpdateProgramCountersInfoD1D2D3()
        {
            lblD1.Text = cntD1.ToString();
            lblD2.Text = cntD2.ToString();
            lblD3.Text = cntD3.ToString();
            lblD7.Text = cntD7.ToString();
            lblD8.Text = cntD8.ToString();
            lblD9.Text = cntD9.ToString();
        }

        private void LoadProgramCountersD4D5D6()
        {
            cntD4 = cntD5 = cntD6 = 0;
            if (File.Exists(Path.Combine(Application.StartupPath, "_program_counters_D4D5D6_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_counters_D4D5D6_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0) cntD4 = Convert.ToUInt64(lstList[0]);
                    if (lstList.Count > 1) cntD5 = Convert.ToUInt64(lstList[1]);
                    if (lstList.Count > 2) cntD6 = Convert.ToUInt64(lstList[2]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    cntD4 = cntD5 = cntD6 = 0;
                }
            }

            UpdateProgramCountersInfoD4D5D6();
        }

        private void SaveProgramCountersD4D5D6()
        {
            List<String> lstList = new List<String>();
            lstList.Add(Convert.ToString(cntD4));
            lstList.Add(Convert.ToString(cntD5));
            lstList.Add(Convert.ToString(cntD6));
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_counters_D4D5D6_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstList, Encoding.UTF8);

        }

        private void UpdateProgramCountersInfoD4D5D6()
        {
            lblD4.Text = cntD4.ToString();
            lblD5.Text = cntD5.ToString();
            lblD6.Text = cntD6.ToString();
        }

        private void LoadProgramCountersC4C5C6()
        {
            cntC4 = cntC5 = cntC6 = 0;
            if (persC8 != iPersUserID)
            {
                cntC8 = 0;
                contC9 = -1;
                cntC9 = 0;
                UpdateProgramCountersInfoC8C9();
            }
            if (File.Exists(Path.Combine(Application.StartupPath, "_program_counters_C4C5C6_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_counters_C4C5C6_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0) cntC4 = Convert.ToUInt64(lstList[0]);
                    if (lstList.Count > 1) cntC5 = Convert.ToUInt64(lstList[1]);
                    if (lstList.Count > 2) cntC6 = Convert.ToUInt64(lstList[2]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    cntC4 = cntC5 = cntC6 = 0;
                }
            }

            UpdateProgramCountersInfoC4C5C6();
        }

        private void SaveProgramCountersC4C5C6()
        {
            if (iPersUserID >= 0)
            {
                List<String> lstList = new List<String>();
                lstList.Add(Convert.ToString(cntC4));
                lstList.Add(Convert.ToString(cntC5));
                lstList.Add(Convert.ToString(cntC6));
                FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_counters_C4C5C6_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstList, Encoding.UTF8);
            }
        }

        private void UpdateProgramCountersInfoC4C5C6()
        {
            lblC4.Text = (cntC4 / 3600).ToString("00") + ":" + ((cntC4 / 60) % 60).ToString("00");
            lblC5.Text = (cntC5 / 3600).ToString("00") + ":" + ((cntC5 / 60) % 60).ToString("00");
            lblC6.Text = (cntC6 / 3600).ToString("00") + ":" + ((cntC6 / 60) % 60).ToString("00");
        }

        private void UpdateProgramCountersInfoC8C9()
        {
            lblC8.Text = (cntC8 / 60).ToString("00") + "." + (cntC8 % 60).ToString("00");
            lblC9.Text = (cntC9 / 60).ToString("00") + "." + (cntC9 % 60).ToString("00");
        }

        private void clearTrashList(List<String> list)
        {
            int idx = 0;
            while (idx < list.Count)
            {
                if (list[idx] == null)
                    list.RemoveAt(idx);
                else if (list[idx].Length == 0)
                    list.RemoveAt(idx);
                else
                    idx++;
            }
        }

        private void UpdateProgramCountersInfoB2B5B6()
        {
            int cntB2 = 0, cntB5 = 0, cntB6 = 0;
            if (File.Exists(Path.Combine(sDataPath, "_personen.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_personen.txt"));
                    List<String> lstList = new List<String>(srcFile);

                    clearTrashList(lstList);
                    foreach (String str in lstList)
                        if (str.Length > 0)
                        {
                            cntB2++;
                            if (str.IndexOf("|") <= 0)
                                continue;
                            String sUID = str.Substring(0, str.IndexOf("|"));
                            long lUID = Convert.ToInt64(sUID);

                            if (File.Exists(Path.Combine(sDataPath, "_contacts_" + sUID + ".txt")))
                            {
                                try
                                {
                                    var srcFileCont = File.ReadAllLines(Path.Combine(sDataPath, "_contacts_" + sUID + ".txt"));
                                    List<String> lstListCont = new List<String>(srcFileCont);

                                    clearTrashList(lstListCont);
                                    foreach (String strcont in lstListCont)
                                        if (strcont.Length > 0)
                                        {
                                            cntB5++;
                                            if (lUID == iPersUserID)
                                                cntB6++;
                                        }
                                }
                                catch
                                {
                                }
                            }
                        }
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    cntB2 = cntB5 = cntB6 = 0;
                }
            }
            lblB2.Text = cntB2.ToString();
            lblB5.Text = cntB5.ToString();
            lblB6.Text = cntB6.ToString();
        }

        List<long> lstCntB3 = new List<long>();
        private void UpdateProgramCountersInfoB3()
        {
            int cnt = 0;

            if (iPersUserID >= 0 && SocialNetwork != 1)
            {
                if (!lstCntB3.Contains(iPersUserID))
                    lstCntB3.Add(iPersUserID);
                cnt = lstCntB3.Count;
            }
            lblB3.Text = cnt.ToString();
        }

        public static String getSocialNetworkPrefix(int iSocialNetworkID)
        {
            if (iSocialNetworkID == 0)
                return "";

            if (iSocialNetworkID == 2)
                return "fb";

            return "local";
        }

        public static String getSocialNetworkPrefix()
        {
            return getSocialNetworkPrefix(SocialNetwork);
        }

        void lblEQInHarNames_Click(object sender, EventArgs e)
        {
            Button lbl = sender as Button;
            if (lbl != null)
            {
                if (labelInEqMsgHarTitleValue_Text.Length > 0)
                {
                    contextMenuStripVectorKoef.Tag = (Int32)(lbl.Tag);
                    contextMenuStripVectorKoef.Show(lbl.PointToScreen(new Point(0, 0)));
                }
            }
        }

        void lblEQOutHarNames_Click(object sender, EventArgs e)
        {
            Button lbl = sender as Button;
            if (lbl != null)
            {
                if (iContUserID >= 0)
                {
                    contextMenuStripVectorKoefOut.Tag = (Int32)(lbl.Tag);
                    contextMenuStripVectorKoefOut.Show(lbl.PointToScreen(new Point(0, 0)));
                }
            }
        }

        string DisplayEmoji(string input, bool bPerson)
        {
            String bgColor = bPerson ? "#" + PersonColor2 : "#" + ContacterColor2;
            StringBuilder output = new StringBuilder();
            output.Append("<html style=\"font-family: Verdana, Arial; font-size: 14pt; border:none; border: 0px; margin-top:0px; margin-bottom:0px; background: " + bgColor + "\"><body>");
            var enumerator = StringInfo.GetTextElementEnumerator(input);
            while (enumerator.MoveNext())
            {
                string chunk = enumerator.GetTextElement();
                if (char.IsSurrogatePair(chunk, 0))
                {
                    char[] ch0 = chunk.ToCharArray();
                    output.Append("<img src=\"" + "http://vk.com/images/emoji/" + ((int)ch0[0]).ToString("X") + ((int)ch0[1]).ToString("X") + ".png\">");
                    //output.Append((char.ConvertToUtf32(chunk, 0)));
                }
                else
                    output.Append(chunk);
            }
            output.Append("</body></html>");
            return output.ToString();
        }

        public void Set_labelInMsgHarTitleValue_Text(String Text)
        {
            labelInMsgHarTitleValue_Text = Text;
            if (webBrowserInMessageText == null)
                return;
            try
            {
                webBrowserInMessageText.DocumentText = NilsaUtils.TextToString(DisplayEmoji(Text, false));
                toolTipMessage.SetToolTip(webBrowserInMessageText, Text);
            }
            catch
            {

            }
        }

        public void Set_labelInEqMsgHarTitleValue_Text(String Text)
        {
            if (Text.Length == 0) SetEQMessageIdealParameters(new String[0]);

            String sMarker = "";
            String sText = Text;
            if (sText.IndexOf("|!*#0") >= 0)
            {
                sMarker = sText.Substring(sText.IndexOf("|!*#0") + 5);
                sText = sText.Substring(0, sText.IndexOf("|!*#0"));
            }
            labelInEqMsgHarTitleValue_Text = sText;
            labelInEqMsgHarTitleMarker.Text = sMarker;
            //webBrowserInEqMessageText.DocumentText = NilsaUtils.TextToString(DisplayEmoji(sText));
            //toolTipMessage.SetToolTip(webBrowserInEqMessageText, sText);
        }

        private String makeTextSubvariants(String text)
        {
            string pattern = @"{[^{}]*}";
            string result = text;
            Regex newReg = new Regex(pattern);

            MatchCollection matches = newReg.Matches(result);
            while (matches.Count > 0)
            {
                Match mat = matches[0];
                string variant = mat.Value.Substring(1, mat.Value.Length - 2);
                result = result.Substring(0, mat.Index) + makeTextVariants(variant, false, true) + result.Substring(mat.Index + mat.Length);
                matches = newReg.Matches(result);
            }
            return result;
        }

        private String makeTextVariants(String sText, bool makeSubvariants = true, bool enableEmptyVariants = false)
        {
            string text = sText;
            if (makeSubvariants)
                text = makeTextSubvariants(text);

            if (text.IndexOf("~") >= 0)
            {
                string[] textVariant = text.Split('~');
                do
                {
                    Random rnd = new Random();
                    int idx = rnd.Next(textVariant.Length);
                    idx = rnd.Next(textVariant.Length);
                    if (textVariant[idx].Length > 0 || enableEmptyVariants)
                    {
                        text = textVariant[idx];
                        break;
                    }
                } while (true);
            }
            return text;
        }

        private string labelOutEqMsgHarTitleValueFullText = "";
        public void Set_labelOutEqMsgHarTitleValue_Text(String Text)
        {
            String sMarker = "";
            String sText = Text;
            if (sText.IndexOf("|!*#0") >= 0)
            {
                sMarker = sText.Substring(sText.IndexOf("|!*#0") + 5);
                sText = sText.Substring(0, sText.IndexOf("|!*#0"));
            }
            labelOutEqMsgHarTitleValueFullText = sText;
            sText = makeTextVariants(sText);

            labelOutEqMsgHarTitleValue_Text = sText;
            labelOutEqMsgHarTitleMarker.Text = sMarker;
            sText = (sCurUserPrevMsgUID.Length > 0 ? sCurUserPrevMsgText + " " : "") + sText;
            try
            {
                webBrowserOutEqMessageText.DocumentText = NilsaUtils.TextToString(DisplayEmoji(sText, true));
            }
            catch
            {

            }
            toolTipMessage.SetToolTip(webBrowserOutEqMessageText, sText);
        }

        public string Decrypt(string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            try
            {
                byte[] inputbuffer = Convert.FromBase64String(text);
                byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
                return Encoding.Unicode.GetString(outputBuffer);
            }
            catch
            {

            }
            return "";
        }

        private int getLicenseType()
        {
            if (sLicenseUser.StartsWith("NILS_"))
                return LICENSE_TYPE_NILS;

            if (sLicenseUser.StartsWith("WORK_"))
                return LICENSE_TYPE_WORK;

            if (sLicenseUser.StartsWith("DEMO_"))
                return LICENSE_TYPE_DEMO;

            if (sLicenseUser.StartsWith("PRO_"))
                return LICENSE_TYPE_PRO;

            if (sLicenseUser.StartsWith("ADMIN_"))
                return LICENSE_TYPE_ADMIN;

            return LICENSE_TYPE_DEMO;
        }

        private bool CheckLicense(bool silent = false)
        {
            bool bLicensed = false;
            bool bShowError = true;
            String sLicenseFileName = "C:\\nilsa.license";
            if (!File.Exists(sLicenseFileName))
                sLicenseFileName = Path.Combine(Application.StartupPath, "nilsa.license");

            if (File.Exists(sLicenseFileName))
            {
                try
                {
                    var srcFile = File.ReadAllLines(sLicenseFileName);
                    List<String> lstLic = new List<String>(srcFile);
                    if (lstLic.Count == 1)
                    {
                        String sLic = lstLic[0];
                        if (sLic.Length > 0)
                        {
                            String strLic = Decrypt(sLic);
                            if (strLic.Length > 0)
                            {
                                String sDate = strLic.Substring(strLic.IndexOf("|") + 1);
                                int year = Convert.ToInt32(sDate.Substring(0, 4));
                                int month = Convert.ToInt32(sDate.Substring(4, 2));
                                int day = Convert.ToInt32(sDate.Substring(6, 2));
                                DateTime dt = new DateTime(year, month, day);
                                sLicenseUser = sDate.Substring(sDate.IndexOf("|") + 1).Trim();
                                sLicenseUserFormatted = " [" + sLicenseUser + "] ";
                                sLicenseDate = dt.ToShortDateString();
                                iLicenseType = getLicenseType();

                                if (iLicenseType == LICENSE_TYPE_NILS || iLicenseType == LICENSE_TYPE_WORK)
                                    userAppId = 6032784;// 6222822;// 6222824;//6032776;//6183596;// 5555614;

                                if (iLicenseType == LICENSE_TYPE_WORK)
                                    toolStripButtonCreateDialogs.Visible = false;
                                else
                                    toolStripButtonCreateDialogs.Visible = true;

                                //if (sLicenseUser.Trim().Equals("379014"))
                                //    externalActivatedProcess = false;

                                if (dt < DateTime.Now)
                                {
                                    bShowError = false;
                                    if (!silent)
                                    {
                                        HideFormWait();
                                        MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_4", this.Name, "Срок действия лицензии закончился! Пожалуйста, обратитесь к разработчикам для обновления лицензии...") + "\n\n" + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_1", this.Name, "Пользователь:") + " " + sLicenseUser + "\n" + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_2", this.Name, "Срок лицензии:") + " " + sLicenseDate, NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_5", this.Name, "Ошибка лицензии"));
                                        ShowFormWait();
                                    }

                                }
                                else
                                {
                                    bLicensed = true;
                                    int diffDays = (dt - DateTime.Now.Date).Days;
                                    if (diffDays <= 7)
                                    {
                                        if (!silent)
                                        {
                                            HideFormWait();
                                            MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_9", this.Name, "До окончания срока действия лицензии осталось") + " " + diffDays.ToString() + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_10", this.Name, "дн.!") + "\n" + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_11", this.Name, "Рекомендуем обратиться к разработчикам для продления лицензии заранее...") + "\n\n" + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_1", this.Name, "Пользователь:") + " " + sLicenseUser + "\n" + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_2", this.Name, "Срок лицензии:") + " " + sLicenseDate, NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_6", this.Name, "Предупреждение о сроке лицензии"));
                                            ShowFormWait();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionToLogList("CheckLicense", "Reading lists", e);
                }
            }
            if (!bLicensed)
            {
                if (bShowError && !silent)
                {
                    HideFormWait();
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_7", this.Name, "Отсутствует или некорректный лицензионный файл nilsa.license в корне диска C: или в директории установки программы.") + "\n\n" + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_8", this.Name, "Пожалуйста, обратитесь к разработчикам для получения лицензии и разместите полученный лицензионный файл nilsa.license в корне диска C: или в директории установки программы."), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_5", this.Name, "Ошибка лицензии"));
                    ShowFormWait();
                }
            }
            return bLicensed;
        }

        /*
        FormCaptcha formCaptha = null;
        public FormCaptcha showFormCaptha(Uri url, long? sid, bool doNotStart = false)
        {
            if (formCaptha == null)
            {
                formCaptha = new FormCaptcha(this, url, sid);
                return formCaptha;
            }

        }
        */


        FormWait fwait = null;
        bool showFormWait = false;
        public void ShowFormWait(String text = "")
        {
            showFormWait = true;
            if (text.Length == 0)
                text = NilsaUtils.Dictonary_GetText(userInterface, "textValues_6", this.Name, "Пожалуйста, подождите, идет смена персонажа...");
            /*
            if (fwait == null)
                fwait = new FormWait(this);
            fwait.labelText.Text = text;
            fwait.Owner = this;
            fwait.Show();
            */

            toolStripTextBoxWait.Text = text;
            if (!toolStripTextBoxWait.Visible)
                toolStripTextBoxWait.Visible = true;

            if (!showBrowserCommand)
                toolStripTop.BackColor = Color.FromArgb(192, 0, 0);

            Application.DoEvents();
        }

        public void HideFormWait()
        {
            showFormWait = false;
            if (toolStripTextBoxWait.Visible)
                toolStripTextBoxWait.Visible = false;

            if (!showBrowserCommand)
                toolStripTop.BackColor = SystemColors.Control;
            Application.DoEvents();

            //if (fwait.Visible)
            //    fwait.Hide();
        }

        bool showBrowserCommand = false;
        public void ShowBrowserCommand()
        {
            showBrowserCommand = true;
            if (!showFormWait)
                toolStripTop.BackColor = Color.FromArgb(192, 0, 0);
            Application.DoEvents();
        }

        public void HideBrowserCommand()
        {
            showBrowserCommand = false;

            if (!showFormWait)
                toolStripTop.BackColor = SystemColors.Control;
            Application.DoEvents();
        }

        public void Shuffle(List<string> list)
        {
            Random rnd = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void shufflePersonenLists()
        {
            lstPersoneShuffle.Clear();

            clearTrashList(lstPersoneChange);
            foreach (string str in lstPersoneChange)
                lstPersoneShuffle.Add(str);

            if (lstPersoneShuffle.Count > 1)
            {
                Shuffle(lstPersoneShuffle);
            }
        }

        private void initPersonenLists(FormEditPersonenDB fe)
        {
            if (fe == null)
                return;

            lstPersoneChange.Clear();
            for (int i = 0; i < fe.lvList.Items.Count; i++)
            {
                if (fe.lvList.Items[i].Checked)
                    lstPersoneChange.Add(fe.lvList.Items[i].SubItems[1].Text);
            }

            if (lstPersoneChange.Count == 1)
                lstPersoneChange.Clear();

            lstPersoneChangeOriginal.Clear();

            clearTrashList(lstPersoneChange);
            foreach (string str in lstPersoneChange)
                lstPersoneChangeOriginal.Add(str);

            shufflePersonenLists();
            setRandomizeRotatePersonenButtonIcon();
        }

        private void clearPersonenLists()
        {
            lstPersoneChange.Clear();
            lstPersoneChangeOriginal.Clear();
            setRandomizeRotatePersonenButtonIcon();
        }

        private void SelectOtherPersone()
        {
            bool prevTopmost = false;
            if (fwbVKontakte != null)
            {
                prevTopmost = fwbVKontakte.TopMost;
                fwbVKontakte.TopMost = true;
            }

            FormEditPersonenDB fe = new FormEditPersonenDB(this);
            fe.sContHar = new String[iPersHarCount, iPersHarAttrCount + 1];
            for (int i = 0; i < iPersHarCount; i++)
            {
                for (int j = 0; j < iPersHarAttrCount; j++)
                    fe.sContHar[i, j] = sPersHar[i, j];
                fe.sContHar[i, iPersHarAttrCount] = "";
            }
            fe.iContHarCount = iPersHarCount;
            fe.iContHarAttrCount = iPersHarAttrCount;
            fe.Setup("Autorization Error select user", -1, true, userLogin, userPassword);

            if (lstPersoneChange.Count > 0)
            {
                clearTrashList(lstPersoneChange);
                foreach (String str in lstPersoneChange)
                    for (int i = 0; i < fe.lvList.Items.Count; i++)
                    {
                        if (fe.lvList.Items[i].SubItems[1].Text == str)
                        {
                            fe.lvList.Items[i].Checked = true;
                            break;
                        }
                    }
            }

            fe.ShowDialog();

            if (fwbVKontakte != null)
                fwbVKontakte.TopMost = prevTopmost;

            if (fe.bNeedPersoneChange)
            {
                timerAnswerWaitingOff();

                initPersonenLists(fe);

                if (fe.suSelSocialNetwork != SocialNetwork)
                {
                    SocialNetwork = fe.suSelSocialNetwork;
                    OnSocialNetworkChanged();
                }
                onAfterPersonenListChanged();

                userLogin = fe.suSelLogin;
                userPassword = fe.suSelPwd;
                userID = fe.suSelID;

            }
            else
            {
                SocialNetwork = 1;
                userLogin = "nilsa";
                userPassword = "nilsa";
                clearPersonenLists();
                timerAnswerWaitingOff();
                //toolStripMenuItemClearInMsgPullPersonen.Enabled = lstPersoneChange.Count > 0 && SocialNetwork == 0;
                onAfterPersonenListChanged();
                OnSocialNetworkChanged();
            }
        }
        /*
        public bool ReAutorize(String sUsrSelLogin, String sUsrSelPwd)
        {
            // Temp used exception
            if (!AutorizeVK(userLogin, userPassword))
            {
                bool bStatusService = !tbStartService.Enabled;
                tbStopService_Click(null, null);

                bool autorize = false;
                bool bFirstCycle = true;
                bool bCanGoToRotation = lstPersoneChange.Count > 0;
                bool bAutoMode = isProgramInAutoMode;
                DialogResult dr = DialogResult.OK;
                do
                {
                    if (!bFirstCycle)
                        autorize = AutorizeVK(sUsrSelLogin, sUsrSelPwd);

                    bFirstCycle = false;
                } while (!autorize && (dr = new FormReconnect(this, bCanGoToRotation, bAutoMode).ShowDialog()) == DialogResult.OK);

                if (!autorize)
                {
                    if (dr == DialogResult.Ignore)
                    {
                        onChangePersoneByTimer(true, bAutoMode, true);
                        //Environment.Exit(1);
                        return false;
                    }
                    else
                    {
                        SelectOtherPersone();
                        Setup(userLogin, userPassword);
                        //Environment.Exit(1);
                        return false;
                    }
                }

                if (bStatusService)
                    tbStartService_Click(null, null);
            }
            return true;
        }
        */
        public FormWebBrowser fwbVKontakte = null;
        public bool fwbVKontakteFirstShow = false;

        public async void Setup(String sUsrSelLogin = "", String sUsrSelPwd = "", String sUsrSelID="")
        {

            ShowFormWait();
            //sLastRemovedMessage = "";
            bSetupDone = false;
            bSessionAnswerSended = false;

            TextSearchMinWordsLen = 1;
            CompareLexicalLevel = 0;
            CompareVectorLevel = 0;
            TextSearchFilteredChars = ".,";
            TextSearchIgnoredWords = "из|под|с|по|в|";
            TextSearchTextParsingChars = ".|...|!|?|<br>";

            iPersUserID = 0;
            userLogin = "";
            userName = "";
            //userNameFamily = "";
            //userNameName = "";
            userPassword = "";

            if (!CheckLicense())
            {
                Environment.Exit(0);
                return;
            }

            if (fwbVKontakte == null)
            {
                fwbVKontakte = new FormWebBrowser(this, true);
                fwbVKontakte.Init();
            }

            if (!fwbVKontakteFirstShow)
            {
                fwbVKontakteFirstShow = true;
                fwbVKontakte.Show();
            }
            /*
            if (SocialNetwork == 0)
            {
                if (fwbVKontakte == null)
                {
                    fwbVKontakte = new FormWebBrowser(this, true);
                    fwbVKontakte.Init();
                }

                if (!fwbVKontakteFirstShow)
                {
                    fwbVKontakteFirstShow = true;
                    fwbVKontakte.Show();
                }
            }
            */

            PersonenList_Load();
            if (!Autorize(sUsrSelLogin, sUsrSelPwd, sUsrSelID))
            {
                sUsrSelLogin = "";
                sUsrSelPwd = "";
                sUsrSelID = "";
            }
            
            PersonenList_Load();
            PersonenList_SavePersoneLogin();

            Wall_LoadPostToMonitoring();
            ContactsList_Load();
            /*
             * Disable API
            if (SocialNetwork == 0 && externalActivatedProcess)
            {
                try
                {
                    api.Messages.Send(330305148, false, sLicenseUserFormatted + "Activated persone (" + System.Environment.MachineName + " - " + System.Environment.UserName + ")"); // посылаем сообщение пользователю
                }
                catch (VkNet.Exception.AccessTokenInvalidException)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (System.Net.WebException)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (VkNet.Exception.VkApiException vkapexeption)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (Exception exp)
                {
                    ExceptionToLogList("Setup", "UNKNOWN", exp);
                }
            }
            */
            LoadAlgorithmsDB();
            //LoadContacterCounters();
            LoadProgramCountersC4C5C6();
            LoadProgramCountersE4E5E6();
            LoadProgramCountersD4D5D6();
            UpdateProgramCountersInfoB2B5B6();
            UpdateProgramCountersInfoB3();
            //LoadTimersSettings();
            //LoadTextSearchSettings();
            LoadReceivedMessagesPull();
            LoadOutgoingMessagesPull();
            LoadOutgoingDelayedMessagesPull();

            LoadPersoneParametersValues();
            SetPersoneParametersValues();

            iContUserID = -1;
            iGroupAnswerID = -1;
            iGroupAnswerPostID = -1;
            iGroupAnswerCommentID = -1;

            LoadContactParamersValues();
            SetContactParametersValues();

            //GroupsList_Load();
            //LoadGroupParametersDescription();

            LoadMessagesDBList();
            LoadMessageParametersDescription();
            MessagesDB_LoadUserDBs();
            LoadEQInMessageDB();
            LoadEQOutMessageDB();
            SaveEQInMessageDB();
            SaveEQOutMessageDB();

            timerDefaultChangePersoneCycle = 0;
            LoadAlgorithmSettings();

            LoadResendContUserID();

            /*
             Disable API
            if (SocialNetwork == 0)
            {
                if (fwbVKontakte == null)
                {
                    fwbVKontakte = new FormWebBrowser(this, true);
                    fwbVKontakte.Init();
                }

                stopTimers();
                
                fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.LoginPersone);
                if (!fwbVKontakteFirstShow)
                {
                    fwbVKontakteFirstShow = true;
                    fwbVKontakte.Show();
                }
                fwbVKontakte.WaitResult();
                startTimers();

            }
            */

            if (SocialNetwork == 0)
            {
                labelPersActivationCounter.Visible = true;
                incrementPersoneActivationCounter();

            }
            else
                labelPersActivationCounter.Visible = false;

            tbStartService.Enabled = false;
            tbStopService.Enabled = true;
            bServiceStart = true;
            tbNewInMessageEnter.Enabled = true;

            //if (SocialNetwork == 0)
            //{
            //    DateTime dt = DateTime.Now;
            //    lstReceivedMessages.Insert(0, "0|" + iPersUserID.ToString() + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "PERSONE_CHANGED");
            //}

            if (SocialNetwork == 0 && reinitDialogsWhenFree)
            {
                if (iContactsGroupsMode == 0) // Contacts
                {
                    DateTime dt = DateTime.Now;
                    lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "INIT_PERSONE_DIALOG");
                }
                else // Groups
                {
                    DateTime dt = DateTime.Now;
                    lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "INIT_PERSONE_GROUPS_DIALOG");
                }
            }

            timerChangePersoneOn();
            SelectNextReceivedMessage(false);
            bSetupDone = true;
            onAfterPersonenListChanged();
            if (SocialNetwork == 0)
                setBtnB4(iPersUserID.ToString());
            tbStopService_Click(null, null);
            HideFormWait();

        }

        public void ExceptionToLogList(String sMethod, String sErrorsParameters, Exception e)
        {
            if (lstErrorsLogList == null)
                return;

            lstErrorsLogList.Add(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + Application.ProductVersion + " - " + iPersUserID.ToString() + " (" + userLogin + ", " + userPassword + ")\n" + sMethod + ": " + sErrorsParameters + "\n" + e.ToString());
            SaveErrorsLogList();
        }

        bool autoUpdateProgram = true;
        bool autoRestoreProgramState = true;
        bool autoStartNILSAMonitor = true;
        bool showSessionStartTime = true;
        int randomizeRotatePersonen = 0;
        bool reinitDialogsWhenFree = false;
        int ChangeModeWhenFree = 0;
        bool autoUpdateModelFromServer = false;

        private void saveRandomizeRotatePersonen()
        {
            List<String> lstList = new List<String>();
            lstList.Add(randomizeRotatePersonen.ToString());
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_randomize_rotate_personen.txt"), lstList, Encoding.UTF8);
        }

        private void loadRandomizeRotatePersonen()
        {
            randomizeRotatePersonen = 0;
            if (File.Exists(Path.Combine(Application.StartupPath, "_randomize_rotate_personen.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_randomize_rotate_personen.txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0)
                        randomizeRotatePersonen = Convert.ToInt32(lstList[0]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    randomizeRotatePersonen = 0;
                }
            }
            setRandomizeRotatePersonenButtonIcon();
        }

        private void setRandomizeRotatePersonenButtonIcon()
        {
            toolStripButtonRandomizeRotatePersonen.Checked = lstPersoneChange.Count > 1 && SocialNetwork == 0;
            switch (randomizeRotatePersonen)
            {
                case 0:
                    toolStripButtonRandomizeRotatePersonen.Image = Nilsa.Properties.Resources._shuffle_random_2;
                    toolStripButtonRandomizeRotatePersonen.Text = "Последовательная ротация Персонажей в ротации";
                    break;
                case 1:
                    toolStripButtonRandomizeRotatePersonen.Image = Nilsa.Properties.Resources._shuffle_user1;
                    toolStripButtonRandomizeRotatePersonen.Text = "Рандомная смена Персонажей в ротации";
                    break;
                case 2:
                    toolStripButtonRandomizeRotatePersonen.Image = Nilsa.Properties.Resources._shuffle_random_1;
                    toolStripButtonRandomizeRotatePersonen.Text = "Рандомная смена Персонажей в ротации с проходом всех по кругу";
                    break;
            }
        }

        private void saveReinitDialogsWhenFree()
        {
            List<String> lstList = new List<String>();
            lstList.Add(reinitDialogsWhenFree ? "1" : "0");
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_reinit_dialogs_when_free.txt"), lstList, Encoding.UTF8);
        }

        private void loadReinitDialogsWhenFree()
        {
            reinitDialogsWhenFree = false;
            if (File.Exists(Path.Combine(Application.StartupPath, "_reinit_dialogs_when_free.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_reinit_dialogs_when_free.txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0)
                        reinitDialogsWhenFree = lstList[0].Equals("1");
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    reinitDialogsWhenFree = false;
                }
            }
            toolStripButtonReinitDialogsWhenFree.Checked = reinitDialogsWhenFree;
        }

        private void setChangeModeWhenFreeFlag()
        {
            List<String> lstList = new List<String>();
            lstList.Add("1");
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_change_mode_when_free.txt"), lstList, Encoding.UTF8);
        }

        private bool checkChangeModeWhenFreeFlag()
        {
            return File.Exists(Path.Combine(Application.StartupPath, "_change_mode_when_free.txt"));
        }

        private void removeChangeModeWhenFreeFlag()
        {
            if (File.Exists(Path.Combine(Application.StartupPath, "_change_mode_when_free.txt")))
                File.Delete(Path.Combine(Application.StartupPath, "_change_mode_when_free.txt"));
        }


        private void saveChangeModeWhenFree()
        {
            List<String> lstList = new List<String>();
            lstList.Add(ChangeModeWhenFree.ToString());
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_change_mode_when_free_value.txt"), lstList, Encoding.UTF8);
        }

        private void loadChangeModeWhenFree()
        {
            ChangeModeWhenFree = 0;
            if (File.Exists(Path.Combine(Application.StartupPath, "_change_mode_when_free_value.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_change_mode_when_free_value.txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0)
                        ChangeModeWhenFree = Convert.ToInt32(lstList[0]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    ChangeModeWhenFree = 0;
                }
            }
            toolStripButtonChangeModeWhenFree.Checked = lstPersoneChange.Count > 0 && SocialNetwork == 0;
            setChangeModeWhenFreeButtonIcon();
        }

        private void setChangeModeWhenFreeButtonIcon()
        {
            switch (ChangeModeWhenFree)
            {
                case 0:
                    toolStripButtonChangeModeWhenFree.Image = Nilsa.Properties.Resources._mode_contacter;
                    toolStripButtonChangeModeWhenFree.Text = "Режим ротации Персонажей: Личное общение";
                    break;
                case 1:
                    toolStripButtonChangeModeWhenFree.Image = Nilsa.Properties.Resources._mode_groups;
                    toolStripButtonChangeModeWhenFree.Text = "Режим ротации Персонажей: Публичное общение";
                    break;
                case 2:
                    toolStripButtonChangeModeWhenFree.Image = Nilsa.Properties.Resources._mode_groupspersone;
                    toolStripButtonChangeModeWhenFree.Text = "Режим ротации Персонажей: Публичное затем Личное общение";
                    break;
            }
        }

        private void SaveProgramSettings()
        {
            List<String> lstList = new List<String>();
            lstList.Add(userSelectUserIdx.ToString());
            lstList.Add(autoUpdateProgram || iLicenseType != LICENSE_TYPE_NILS ? "1" : "0");
            lstList.Add(compareLexicalNewAlgorithm ? "1" : "0");
            lstList.Add(CurrentLanguage);
            lstList.Add(FormWebBrowserEnabled ? "1" : "0");
            lstList.Add(autoRestoreProgramState ? "1" : "0");
            lstList.Add(autoStartNILSAMonitor ? "1" : "0");
            lstList.Add(showSessionStartTime ? "1" : "0");
            lstList.Add(autoUpdateModelFromServer || iLicenseType == LICENSE_TYPE_WORK ? "1" : "0");

            FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_settings.txt"), lstList, Encoding.UTF8);

            saveRandomizeRotatePersonen();
            saveReinitDialogsWhenFree();
            saveChangeModeWhenFree();

            SaveMessagesDBList();
            SaveProgramState();
        }

        private void preloadLanguageSettings()
        {
            CurrentLanguage = "ru";
            if (File.Exists(Path.Combine(Application.StartupPath, "_program_settings.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_settings.txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 3)
                        CurrentLanguage = lstList[3];
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    CurrentLanguage = "ru";
                }
            }
        }

        private void LoadProgramSettings()
        {
            userSelectUserIdx = 0;
            CurrentLanguage = "ru";
            if (File.Exists(Path.Combine(Application.StartupPath, "_program_settings.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_settings.txt"));
                    List<String> lstList = new List<String>(srcFile);
                    if (lstList.Count > 0)
                        try { userSelectUserIdx = Convert.ToInt32(lstList[0]); }
                        catch { }
                    if (lstList.Count > 1)
                        autoUpdateProgram = lstList[1].Equals("1") || iLicenseType != LICENSE_TYPE_NILS;
                    if (lstList.Count > 2)
                        compareLexicalNewAlgorithm = lstList[2].Equals("1");
                    if (lstList.Count > 3)
                        CurrentLanguage = lstList[3];
                    if (lstList.Count > 4)
                        FormWebBrowserEnabled = lstList[4].Equals("1");
                    if (lstList.Count > 5)
                        autoRestoreProgramState = lstList[5].Equals("1");
                    if (lstList.Count > 6)
                        autoStartNILSAMonitor = lstList[6].Equals("1");
                    if (lstList.Count > 7)
                        showSessionStartTime = lstList[7].Equals("1");
                    if (lstList.Count > 8)
                        autoUpdateModelFromServer = lstList[8].Equals("1") || iLicenseType == LICENSE_TYPE_WORK;
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    userSelectUserIdx = 0;
                    CurrentLanguage = "ru";
                }
            }

            //FormWebBrowserEnabled = false; // Force disable

            loadRandomizeRotatePersonen();
            loadReinitDialogsWhenFree();
            loadChangeModeWhenFree();
            LoadMessagesDBList();
        }

        private void SaveProgramState()
        {
            if (autoRestoreProgramState)
            {
                List<String> lstList = new List<String>();
                lstList.Add(SocialNetwork.ToString());
                lstList.Add(userLogin);
                lstList.Add(userPassword);

                FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_state.txt"), lstList, Encoding.UTF8);

                if (lstPersoneChange.Count > 0)
                    FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_state_personen.txt"), lstPersoneChange, Encoding.UTF8);
                else
                    File.Delete(Path.Combine(Application.StartupPath, "_program_state_personen.txt"));

                if (lstPersoneChangeOriginal.Count > 0)
                    FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_state_personen_original.txt"), lstPersoneChangeOriginal, Encoding.UTF8);
                else
                    File.Delete(Path.Combine(Application.StartupPath, "_program_state_personen_original.txt"));

                if (lstPersoneShuffle.Count > 0)
                    FileWriteAllLines(Path.Combine(Application.StartupPath, "_program_state_personen_shuffle"), lstPersoneShuffle, Encoding.UTF8);
                else
                    File.Delete(Path.Combine(Application.StartupPath, "_program_state_personen_shuffle.txt"));
            }
            else
            {
                File.Delete(Path.Combine(Application.StartupPath, "_program_state.txt"));
                File.Delete(Path.Combine(Application.StartupPath, "_program_state_personen.txt"));
                File.Delete(Path.Combine(Application.StartupPath, "_program_state_personen_original.txt"));
                File.Delete(Path.Combine(Application.StartupPath, "_program_state_personen_shuffle.txt"));
            }
        }

        private void LoadProgramState()
        {
            if (autoRestoreProgramState)
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "_program_state.txt")))
                {
                    try
                    {
                        if (autoStartNILSAMonitor && !isNILSAMonitorRunned())
                        {
                            if (File.Exists(Path.Combine(Application.StartupPath, "NILSA_Monitor.exe")))
                                Process.Start(Path.Combine(Application.StartupPath, "NILSA_Monitor.exe"));

                            int iWaitCount = 0;
                            while (!isNILSAMonitorRunned() && iWaitCount < 50)
                            {
                                Thread.Sleep(200);
                                iWaitCount++;
                            }
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (autoStartNILSAMonitor && isNILSAMonitorRunned())
                            setMonitorTime(true);
                    }
                    catch
                    {

                    }

                    int suSelSocialNetwork = 1;
                    string suSelLogin = "nilsa";
                    string suSelPwd = "nilsa";
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_state.txt"));
                        List<String> lstList = new List<String>(srcFile);
                        if (lstList.Count > 0)
                            suSelSocialNetwork = Convert.ToInt32(lstList[0]);
                        if (lstList.Count > 1)
                            suSelLogin = lstList[1];
                        if (lstList.Count > 2)
                            suSelPwd = lstList[2];
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        suSelSocialNetwork = 1;
                        suSelLogin = "nilsa";
                        suSelPwd = "nilsa";
                    }

                    lstPersoneChange = new List<string>();
                    if (File.Exists(Path.Combine(Application.StartupPath, "_program_state_personen.txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_state_personen.txt"));
                            lstPersoneChange = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                            lstPersoneChange = new List<string>();
                        }
                    }

                    lstPersoneChangeOriginal = new List<string>();
                    if (File.Exists(Path.Combine(Application.StartupPath, "_program_state_personen_original.txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_state_personen_original.txt"));
                            lstPersoneChangeOriginal = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                            lstPersoneChangeOriginal = new List<string>();
                        }
                    }

                    lstPersoneShuffle = new List<string>();
                    if (File.Exists(Path.Combine(Application.StartupPath, "_program_state_personen_shuffle.txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_program_state_personen_shuffle.txt"));
                            lstPersoneShuffle = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                            lstPersoneShuffle = new List<string>();
                        }
                    }

                    if (suSelSocialNetwork != SocialNetwork)
                    {
                        SocialNetwork = suSelSocialNetwork;
                        OnSocialNetworkChanged();
                    }

                    onAfterPersonenListChanged();
                    Setup(suSelLogin, suSelPwd, "");

                    if (autoStartNILSAMonitor && isNILSAMonitorRunned())
                        tbStartService_Click(null, null);
                }
            }
        }

        private void SaveErrorsLogList()
        {
            if (lstErrorsLogList.Count >= 500)
                lstErrorsLogList = lstErrorsLogList.GetRange(lstErrorsLogList.Count - 500, 500);

            if (lstErrorsLogList.Count > 0)
                FileWriteAllLines(Path.Combine(Application.StartupPath, "_errors_log_list.txt"), lstErrorsLogList, Encoding.UTF8);
        }

        private void LoadErrorsLogList()
        {
            lstErrorsLogList = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "_errors_log_list.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_errors_log_list.txt"));
                    lstErrorsLogList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    lstErrorsLogList = new List<String>();
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                }
            }
        }

        private void SaveOutgoingMessagesPull()
        {
            if ((lstOutgoingMessages.Count > 0) && (iPersUserID >= 0))
                FileWriteAllLines(Path.Combine(sDataPath, "_msg_outgoing_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstOutgoingMessages, Encoding.UTF8);
            else
                File.Delete(Path.Combine(sDataPath, "_msg_outgoing_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
        }

        private void LoadOutgoingMessagesPull()
        {
            lstOutgoingMessages = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_msg_outgoing_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_msg_outgoing_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    lstOutgoingMessages = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstOutgoingMessages = new List<String>();
                }
                timerPhysicalSendStart();
            }
        }

        private void SaveOutgoingDelayedMessagesPull()
        {
            if ((lstOutgoingMessagesDelayed.Count > 0) && (iPersUserID >= 0))
                FileWriteAllLines(Path.Combine(sDataPath, "_msg_outdelayed_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstOutgoingMessagesDelayed, Encoding.UTF8);
            else
                File.Delete(Path.Combine(sDataPath, "_msg_outdelayed_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
        }

        private void LoadOutgoingDelayedMessagesPull()
        {
            lstOutgoingMessagesDelayed = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_msg_outdelayed_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_msg_outdelayed_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    lstOutgoingMessagesDelayed = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstOutgoingMessagesDelayed = new List<String>();
                }
            }
        }



        long iResendContUserID = -1;
        private void LoadResendContUserID()
        {
            iResendContUserID = -1;
            if (File.Exists(Path.Combine(sDataPath, "resend_operator_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "resend_operator_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    List<String> lstTmp = new List<String>(srcFile);
                    if (lstTmp.Count > 0) iResendContUserID = Convert.ToInt64(lstTmp[0]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    iResendContUserID = -1;
                }
            }
        }

        private void SaveResendContUserID()
        {
            if (iResendContUserID >= 0)
            {
                List<String> lstTmp = new List<String>();
                lstTmp.Add(iResendContUserID.ToString());
                FileWriteAllLines(Path.Combine(sDataPath, "resend_operator_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstTmp, Encoding.UTF8);
            }
            else
                File.Delete(Path.Combine(sDataPath, "resend_operator_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
        }

        private void SaveReceivedMessagesPull()
        {
            if ((lstReceivedMessages.Count > 0) && (/*userSelectUserIdx*/iPersUserID >= 0))
                FileWriteAllLines(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + (iContactsGroupsMode == 0 ? "_contacter" : "_groups") + ".txt"), lstReceivedMessages, Encoding.UTF8);
            else
                File.Delete(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + (iContactsGroupsMode == 0 ? "_contacter" : "_groups") + ".txt"));

            if (iContactsGroupsMode == 0)
            {
                if ((lstReceivedMessagesGroups.Count > 0) && (iPersUserID >= 0))
                    FileWriteAllLines(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_groups" + ".txt"), lstReceivedMessagesGroups, Encoding.UTF8);
                else
                    File.Delete(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_groups" + ".txt"));
            }
            else
            {
                if ((lstReceivedMessagesContacter.Count > 0) && (iPersUserID >= 0))
                    FileWriteAllLines(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_contacter" + ".txt"), lstReceivedMessagesGroups, Encoding.UTF8);
                else
                    File.Delete(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_contacter" + ".txt"));
            }
        }

        private void incrementPersoneActivationCounter()
        {
            long iCounter = 1;
            List<String> lstList = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_pa_counter_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_pa_counter_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    lstList = new List<String>(srcFile);
                    if (lstList != null && lstList.Count > 0)
                        iCounter = Convert.ToInt64(lstList[0]) + 1;
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    iCounter = 0;
                }
            }

            lstList = new List<String>();
            lstList.Add(Convert.ToString(iCounter));
            FileWriteAllLines(Path.Combine(sDataPath, "_pa_counter_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstList, Encoding.UTF8);
            labelPersActivationCounter.Text = Convert.ToString(iCounter);
        }

        private void deleteAllPersoneActivationCounter()
        {
            labelPersActivationCounter.Text = "0";
            string[] files = Directory.GetFiles(sDataPath, "_pa_counter_*.txt");
            foreach (string f in files)
                File.Delete(f);
        }

        private void LoadReceivedMessagesPull()
        {
            lstReceivedMessages = new List<String>();
            lstReceivedMessagesContacter = new List<String>();
            lstReceivedMessagesGroups = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + (iContactsGroupsMode == 0 ? "_contacter" : "_groups") + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + (iContactsGroupsMode == 0 ? "_contacter" : "_groups") + ".txt"));
                    lstReceivedMessages = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstReceivedMessages = new List<String>();
                }
            }

            if (iContactsGroupsMode == 0)
            {
                if (File.Exists(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_groups" + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_groups" + ".txt"));
                        lstReceivedMessagesGroups = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        lstReceivedMessagesGroups = new List<String>();
                    }
                }
            }
            else
            {
                if (File.Exists(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_contacter" + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_msg_received_pull_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_contacter" + ".txt"));
                        lstReceivedMessagesContacter = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        lstReceivedMessagesContacter = new List<String>();
                    }
                }
            }
        }

        private void ContactsList_Load()
        {
            if (iContactsGroupsMode == 1)
                GroupsList_Load();
            else
            {
                lstContactsList = new List<String>();
                if (File.Exists(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                        lstContactsList = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        lstContactsList = new List<String>();
                        if (SocialNetwork == 1)
                        {
                            if (iPersUserID == 0)
                            {
                                lstContactsList.Add("1|OPERATOR");
                                FileWriteAllLines(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstContactsList, Encoding.UTF8);
                            }
                        }
                    }

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
                else
                {
                    if (SocialNetwork == 1)
                    {
                        if (iPersUserID == 0)
                        {
                            lstContactsList.Add("1|OPERATOR");
                            FileWriteAllLines(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstContactsList, Encoding.UTF8);
                        }
                    }
                }
            }
        }

        private void GroupsList_Load()
        {
            if (iContactsGroupsMode == 0)
                return;

            lstContactsList = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_groups_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_groups_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    lstContactsList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstContactsList = new List<String>();
                }
            }
        }

        private int ContactsList_GetUserIdx(String sUD)
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

        private List<String> LoadFormEditPersHarValues_1_100000()
        {
            List<String> lstContHarAlgValues = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValues_" + Convert.ToString(1) + "_" + Convert.ToString(100000) + ".values")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValues_" + Convert.ToString(1) + "_" + Convert.ToString(100000) + ".values"));
                    lstContHarAlgValues = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstContHarAlgValues = new List<String>();
                    for (int i = 0; i < iContHarCount; i++)
                        lstContHarAlgValues.Add("");
                }
            }
            else
            {
                for (int i = 0; i < iContHarCount; i++)
                    lstContHarAlgValues.Add("");
            }

            return lstContHarAlgValues;
        }

        private List<String> LoadFormEditPersHarValues(int importmode, int submode)
        {
            List<String> lstContHarAlgValues = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"));
                    lstContHarAlgValues = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstContHarAlgValues = LoadFormEditPersHarValues_1_100000();
                }
            }
            else
            {
                lstContHarAlgValues = LoadFormEditPersHarValues_1_100000();
            }

            return lstContHarAlgValues;
        }

        private FormWebBrowser.Persone loadContactAttributes(long id)
        {
            stopTimers();

            ShowBrowserCommand();

            fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.GetContactAttributes, id);
            fwbVKontakte.WaitResult();

            var stringJSON = vkInterface.Setup(userLogin, userPassword, WebBrowserCommand.GetContactAttributes, NilsaOperatingMode.SeleniumMode, id);
            //раскоментить, когда научим интерфейс отвечать
            //var result = JsonConvert.DeserializeObject<ResponseFromInterface>(stringJSON);
            //personeVkAtt = new PersoneVkAttributes(result.PersonId, result.FirstName, result.LastName,
            //    result.Relation, result.BirthDate, result.City, result.Country, result.CountersFriends, result.Online, result.LastSeen);

            FormWebBrowser.Persone usrAdr = fwbVKontakte.contactAtrributes;

            HideBrowserCommand();

            startTimers();
            return fwbVKontakte.contactAtrributes;
        }

        private String UpdateContactUserHarFromVK(String sUID, String _name)
        {
            String sUName = _name;
            if (SocialNetwork == 0)
            {
                try
                {
                    List<String> _lstContHar = LoadFormEditPersHarValues(adbrCurrent.ID, 2);

                    List<String> lstContHar = new List<String>();
                    for (int i = 0; i < iContHarCount; i++)
                        lstContHar.Add(sContHar[i, 0] + "|" + _lstContHar[i]);// (_lstContHar[i].Length>0 ? (_lstContHar[i][0]=='#' ? _lstContHar[i]:""):""));

                    FormWebBrowser.Persone usrAdr = loadContactAttributes(Convert.ToInt64(sUID));

                    //VkNet.Model.User usrAdr = FormMain.api.Users.Get(Convert.ToInt64(sUID), ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.City | ProfileFields.Sex | ProfileFields.BirthDate | ProfileFields.Country | ProfileFields.Relation | ProfileFields.Online);
                    sUName = usrAdr.FirstName + " " + usrAdr.LastName;

                    List<String> lstContHarValues = new List<String>();

                    if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
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
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                            lstContHarValues = new List<String>();
                            for (int i = 0; i < iContHarCount; i++)
                                lstContHarValues.Add(lstContHar[i]);
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
                            String sAge = sDBDataItemsStrings_AgeUnknown;
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

                    FileWriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstContHarValues, Encoding.UTF8);
                }
                /*
                catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (System.Net.WebException)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (VkNet.Exception.VkApiException vkapexeption)
                {
                    ReAutorize(userLogin, userPassword);
                }
                */
                catch (Exception exp)
                {
                    ExceptionToLogList("UpdateContactUserHarFromVK", sUID, exp);
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_12", this.Name, "Ошибка запроса Контактера с указанным ID..."), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_13", this.Name, "Добавление Контактера"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally { }
            }
            else if (SocialNetwork == 1)
            {
                String sur = NILSA_getUserRecord(sUID);
                if (sur != null)
                    sUName = NILSA_GetFieldFromStringRec(sur, 1);
            }

            return sUName;
        }

        private void ContactsList_AddUser(String sUD, String sUName)
        {
            int iuserIdx = ContactsList_GetUserIdx(sUD);
            String userRec = sUD + "|" + sUName;
            if (iuserIdx >= 0)
                lstContactsList[iuserIdx] = userRec;
            else
            {
                userRec = sUD + "|" + UpdateContactUserHarFromVK(sUD, sUName);
                lstContactsList.Add(userRec);
            }

            FileWriteAllLines(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstContactsList, Encoding.UTF8);
            ContactsList_Load();

        }

        private void PersonenList_SavePersoneLogin()
        {
            String sUD = iPersUserID.ToString();
            //PersonenList_AddUser(sUD, userName, userLogin, userPassword); 
            userSelectUserIdx = PersonenList_GetUserIdx(sUD) + 1;
        }

        private void PersonenList_Load()
        {
            lstPersonenList = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_personen" + getSocialNetworkPrefix() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_personen" + getSocialNetworkPrefix() + ".txt"));
                    lstPersonenList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstPersonenList = new List<String>();
                    if (SocialNetwork == 1)
                    {
                        lstPersonenList.Add("0|NILSA|nilsa|nilsa");
                    }
                }
            }
            else
            {
                if (SocialNetwork == 1)
                {
                    lstPersonenList.Add("0|NILSA|nilsa|nilsa");
                }
            }
        }

        private int PersonenList_GetUserIdx(String sUD)
        {
            int iuserIdx = -1;
            for (int i = 0; i < lstPersonenList.Count; i++)
            {
                String str = lstPersonenList[i];
                if (str.Substring(0, str.IndexOf("|")).Equals(sUD))
                {
                    iuserIdx = i;
                    break;
                }
            }
            return iuserIdx;
        }

        private String PersonenList_GetUserRecord(String sUD)
        {
            for (int i = 0; i < lstPersonenList.Count; i++)
            {
                String str = lstPersonenList[i];
                if (str.Substring(0, str.IndexOf("|")).Equals(sUD))
                {
                    return str;
                }
            }
            return "";
        }

        public String PersonenList_GetUserField(String sUD, int iFieldIdx) // 0 - usrID, 1 - usrName, 2 - usrLogin, 3 - usrPwd
        {
            String retval = PersonenList_GetUserRecord(sUD);
            if (retval.Length > 0)
            {
                for (int i = 0; i < iFieldIdx; i++)
                    retval = retval.Substring(retval.IndexOf("|") + 1);
                if (iFieldIdx < 3)
                    retval = retval.Substring(0, retval.IndexOf("|"));
            }
            return retval;
        }

        private void PersonenList_AddUser(String sUD, String sUName, String sULogin, String sUPwd)
        {
            int iuserIdx = PersonenList_GetUserIdx(sUD);
            String userRec = sUD + "|" + sUName + "|" + sULogin + "|" + sUPwd;
            if (iuserIdx >= 0)
                lstPersonenList[iuserIdx] = userRec;
            else
                lstPersonenList.Add(userRec);

            FileWriteAllLines(Path.Combine(sDataPath, "_personen" + getSocialNetworkPrefix() + ".txt"), lstPersonenList, Encoding.UTF8);
        }


        private void SaveTimersSettings()
        {
            List<String> lstTS = new List<String>();
            for (int i = 0; i < 8; i++)
                lstTS.Add(timersValues[i].ToString());

            FileWriteAllLines(Path.Combine(sDataPath, "_algtimers_settings_" + adbrCurrent.ID.ToString() + ".txt"), lstTS, Encoding.UTF8);
        }

        private void SaveLastMessage()
        {
            if (listBoxOutMsg.SelectedItem == null)
                return;
            if (listBoxOutMsg.SelectedItem.ToString().IndexOf("%? ") > 0 && adbrCurrent.ChangeThemeAlgorithmNotStoreLastMsg)
                return;

            if (iContUserID >= 0)
            {
                File.Delete(Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"));
                if (File.Exists(Path.Combine(sDataPath, "_lastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt")))
                {
                    File.Copy(Path.Combine(sDataPath, "_lastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"), Path.Combine(FormMain.sDataPath, "_prevlastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"));
                }

                File.Delete(Path.Combine(FormMain.sDataPath, "_lastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"));
                if (sCurrentEQOutMessageRecord.Length > 0)
                {
                    List<String> lstTS = new List<String>();
                    lstTS.Add(sCurrentEQOutMessageRecord);
                    FileWriteAllLines(Path.Combine(sDataPath, "_lastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"), lstTS, Encoding.UTF8);
                }
            }
        }

        private void LoadLastMessage()
        {
            sCurrentContactLastMessageRecord = "";
            sCurrentContactLastMessageText = "";

            sCurrentContactPrevLastMessageRecord = "";
            sCurrentContactPrevLastMessageText = "";

            if (iContUserID >= 0)
            {
                if (File.Exists(Path.Combine(sDataPath, "_prevlastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_prevlastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"));
                        List<String> lstTS = new List<String>(srcFile);
                        if (lstTS.Count > 0)
                        {
                            sCurrentContactPrevLastMessageRecord = lstTS[0];
                            sCurrentContactPrevLastMessageText = GetMessageTextWOMarker(sCurrentContactPrevLastMessageRecord.Substring(sCurrentContactPrevLastMessageRecord.IndexOf("|@!") + 3));
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        sCurrentContactPrevLastMessageRecord = "";
                        sCurrentContactPrevLastMessageText = "";
                    }
                }

                if (File.Exists(Path.Combine(sDataPath, "_lastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_lastmessage_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"));
                        List<String> lstTS = new List<String>(srcFile);
                        if (lstTS.Count > 0)
                        {
                            sCurrentContactLastMessageRecord = lstTS[0];
                            sCurrentContactLastMessageText = GetMessageTextWOMarker(sCurrentContactLastMessageRecord.Substring(sCurrentContactLastMessageRecord.IndexOf("|@!") + 3));
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        sCurrentContactLastMessageRecord = "";
                        sCurrentContactLastMessageText = "";
                    }
                }

                SetCurrentContactLastMessageFieldArray();
            }
        }

        public void LoadTimersSettings()
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
            if (File.Exists(Path.Combine(sDataPath, "_algtimers_settings_" + adbrCurrent.ID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_algtimers_settings_" + adbrCurrent.ID.ToString() + ".txt"));
                    lstTS = new List<String>(srcFile);

                    if (lstTS.Count > 5)
                    {
                        for (int i = 0; i < 6; i++)
                            timersValues[i] = Convert.ToInt32(lstTS[i]);

                        if (lstTS.Count > 6) timersValues[6] = Convert.ToInt32(lstTS[6]);
                        if (lstTS.Count > 7) timersValues[7] = Convert.ToInt32(lstTS[7]);
                    }
                    else
                        SaveTimersSettings();
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    timersValues[0] = 10;
                    timersValues[1] = 2400;
                    timersValues[2] = 20;
                    timersValues[3] = 10;
                    timersValues[4] = 240;
                    timersValues[5] = 60;
                    timersValues[6] = 10;
                    timersValues[7] = 3600;
                    SaveTimersSettings();
                }

            }
            else
                SaveTimersSettings();

            if (timerChangePersoneCycle > 0 && lstPersoneChange.Count > 0 && SocialNetwork == 0)
            {
                if (timerDefaultChangePersoneCycle != timersValues[7])
                {
                    timerChangePersoneCycle += timersValues[7] - timerDefaultChangePersoneCycle + 1;
                    timerDefaultChangePersoneCycle = timersValues[7];
                    if (timerChangePersoneCycle <= 0)
                        timerChangePersoneCycle = 1;
                    timerChangePersone_Tick(null, null);
                }
            }
            else if (lstPersoneChange.Count > 0 && SocialNetwork == 0)
            {
                if (timerDefaultChangePersoneCycle != timersValues[7] && timerDefaultChangePersoneCycle == 0)
                {
                    timerDefaultChangePersoneCycle = timersValues[7];
                    timerChangePersoneCycle = timerDefaultChangePersoneCycle + 1;
                    timerChangePersone_Tick(null, null);
                }
            }

            /* Timers
            if (TimerNewMessagesRereadCycle > DefaultTimerNewMessagesRereadCycle) TimerNewMessagesRereadCycle = DefaultTimerNewMessagesRereadCycle;
            if (TimerSendAnswerCycle > DefaultTimerSendAnswerCycle) TimerSendAnswerCycle = DefaultTimerSendAnswerCycle;
            if (TimerSkipMessageCycle > DefaultTimerSkipMessageCycle) TimerSkipMessageCycle = DefaultTimerSkipMessageCycle;
            if (TimerPersoneChangeCycle > DefaultTimerPersoneChangeCycle) TimerPersoneChangeCycle = DefaultTimerPersoneChangeCycle;
            if (timerAnswerWaitingCycle > timerDefaultAnswerWaitingCycle) timerAnswerWaitingCycle = timerDefaultAnswerWaitingCycle;
            if (timerChangePersoneCycle > timerDefaultChangePersoneCycle) timerChangePersoneCycle = timerDefaultChangePersoneCycle;
            */

            //TimerSkipMessageCycle = DefaultTimerSkipMessageCycle;
            //TimerPersoneChangeCycle = DefaultTimerPersoneChangeCycle;
            //TimerPhysicalSendCycle = DefaultTimerPhysicalSendCycle;
        }

        private void SaveAlgorithmSettings()
        {
            List<String> lstTS = new List<String>();
            lstTS.Add(adbrCurrent.ID.ToString());

            FileWriteAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstTS, Encoding.UTF8);
        }

        private void SaveAlgorithmSettingsContacter(int algid)
        {
            if (iContUserID == -1)
                return;

            List<String> lstTS = new List<String>();
            lstTS.Add(algid.ToString());

            FileWriteAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"), lstTS, Encoding.UTF8);
        }

        private void SaveAlgorithmSettingsContacter()
        {
            SaveAlgorithmSettingsContacter(adbrCurrent.ID);
        }

        private void ResetAlgorithmSettingsAllContacters(int algid)
        {
            List<String> lstTS = new List<String>();
            lstTS.Add(algid.ToString());

            for (int i = 0; i < lstContactsList.Count; i++)
            {
                String str = lstContactsList[i];
                String sUID = str.Substring(0, str.IndexOf("|"));
                FileWriteAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstTS, Encoding.UTF8);
                UpdateContactParametersValues_Algorithm(sUID, algid);
            }
        }

        private void ResetAlgorithmSettingsAllContactersList(int ERROR_ID, int algid)
        {
            List<String> lstUserEnabledAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_selectedalgorithms.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_selectedalgorithms.txt"));
                    lstUserEnabledAlgorithmsList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    lstUserEnabledAlgorithmsList = new List<String>();
                }
            }
            else
                return;

            bool bSetEmpty = false;
            if (File.Exists(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    List<String> lstTStemp = new List<String>(srcFile);
                    if (lstUserEnabledAlgorithmsList.Contains(lstTStemp[0]))
                        bSetEmpty = true;
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    bSetEmpty = false;

                    if (lstUserEnabledAlgorithmsList.Contains(ERROR_ID.ToString()))
                        bSetEmpty = true;
                }
            }
            else
            {
                if (lstUserEnabledAlgorithmsList.Contains(ERROR_ID.ToString()))
                    bSetEmpty = true;
            }

            List<String> lstTS = new List<String>();
            lstTS.Add(algid.ToString());

            for (int i = 0; i < lstContactsList.Count; i++)
            {
                String str = lstContactsList[i];
                String sUID = str.Substring(0, str.IndexOf("|"));
                if (File.Exists(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"));
                        List<String> lstTStemp = new List<String>(srcFile);

                        if (lstUserEnabledAlgorithmsList.Contains(lstTStemp[0]))
                        {
                            FileWriteAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstTS, Encoding.UTF8);
                            UpdateContactParametersValues_Algorithm(sUID, algid);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        if (bSetEmpty)
                        {
                            FileWriteAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstTS, Encoding.UTF8);
                            UpdateContactParametersValues_Algorithm(sUID, algid);
                        }
                    }
                }
                else
                {
                    if (bSetEmpty)
                    {
                        FileWriteAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt"), lstTS, Encoding.UTF8);
                        UpdateContactParametersValues_Algorithm(sUID, algid);
                    }
                }
            }
        }

        private void setlabelAlgorithmNameText(String name)
        {
            if (name.Length > 14)
                labelAlgorithmName.Text = name.Substring(0, 12) + "...";
            else
                labelAlgorithmName.Text = name;
        }

        private const int PERSONE_COLUMN_ALGORITHM = 15;
        private const int PERSONE_COLUMN_FRIENDS = 5;
        private const int CONTACT_COLUMN_ALGORITHM = 15;
        private const int CONTACT_COLUMN_FRIENDS = 5;
        private const int CONTACT_COLUMN_STATUS = 14;
        private const int CONTACT_COLUMN_SEX = 0;

        private string getAlgorithmName(int algid)
        {
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
            return algName;
        }

        private void UpdatePersoneParametersValues_Algorithm()
        {
            if (iPersUserID >= 0 && lstPersHarValues.Count > 0)
            {
                List<String> lstTS = new List<String>();
                int algid = -1;
                if (File.Exists(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                        lstTS = new List<String>(srcFile);
                        algid = Convert.ToInt32(lstTS[0]);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        algid = -1;
                    }

                }

                try
                {
                    lstPersHarValues[PERSONE_COLUMN_ALGORITHM] = (PERSONE_COLUMN_ALGORITHM + 1).ToString() + "|" + getAlgorithmName(algid);
                    SavePersoneParamersValues();

                    lblPersHarValues[PERSONE_COLUMN_ALGORITHM].Text = lstPersHarValues[PERSONE_COLUMN_ALGORITHM].Substring(lstPersHarValues[PERSONE_COLUMN_ALGORITHM].IndexOf("|") + 1);
                    toolTipMessage.SetToolTip(lblPersHarValues[PERSONE_COLUMN_ALGORITHM], sPersHar[PERSONE_COLUMN_ALGORITHM, 1] + ": " + lblPersHarValues[PERSONE_COLUMN_ALGORITHM].Text);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                }
            }
        }

        private void UpdateContactParametersValues_Algorithm()
        {
            if (iPersUserID >= 0 && iContUserID >= 0 && lstContHarValues.Count > 0)
            {
                List<String> lstTS = new List<String>();
                int algid = -1;
                if (File.Exists(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"));
                        lstTS = new List<String>(srcFile);
                        algid = Convert.ToInt32(lstTS[0]);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        algid = -1;
                    }
                }

                lstContHarValues[CONTACT_COLUMN_ALGORITHM] = (CONTACT_COLUMN_ALGORITHM + 1).ToString() + "|" + getAlgorithmName(algid);
                SaveContactParamersValues();

                lblContHarValues[CONTACT_COLUMN_ALGORITHM].Text = lstContHarValues[CONTACT_COLUMN_ALGORITHM].Substring(lstContHarValues[CONTACT_COLUMN_ALGORITHM].IndexOf("|") + 1);
                toolTipMessage.SetToolTip(lblContHarValues[CONTACT_COLUMN_ALGORITHM], sContHar[CONTACT_COLUMN_ALGORITHM, 1] + ": " + lblContHarValues[CONTACT_COLUMN_ALGORITHM].Text);
            }
        }

        private void UpdateContactParametersValues_Algorithm(string _contid, int _algid)
        {
            if (iPersUserID >= 0 && _contid.Length >= 0)
            {
                List<String> lstTS = new List<String>();
                if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + _contid + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + _contid + ".txt"));
                        lstTS = new List<String>(srcFile);

                        lstTS[CONTACT_COLUMN_ALGORITHM] = (CONTACT_COLUMN_ALGORITHM + 1).ToString() + "|" + getAlgorithmName(_algid);

                        FileWriteAllLines(Path.Combine(FormMain.sDataPath, "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + _contid + ".txt"), lstTS, Encoding.UTF8);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                    }
                }
            }
        }

        private void LoadAlgorithmSettings()
        {
            //adbrCurrent = new AlgorithmsDBRecord(-1, "", 0, 0, new int[0], new int[0], new bool[0], new int[0], new int[0], new String[0], new int[0], new String[0], true);
            //adbrCurrentDictPairs = new Dictionary<string, string>[FormMain.iMsgHarCount];
            //for (int i = 0; i < FormMain.iMsgHarCount; i++)
            //    adbrCurrentDictPairs[i] = new Dictionary<string, string>();
            //labelAlgorithmName.Text = "";

            List<String> lstTS = new List<String>();
            int algid = -1;
            if (File.Exists(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    lstTS = new List<String>(srcFile);
                    algid = Convert.ToInt32(lstTS[0]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    algid = -1;
                }

            }

            FormEditAlgorithms fe = new FormEditAlgorithms(this);
            fe.Setup(algid, false);
            if (!fe.SelectAlgorithm())
                SaveAlgorithmSettings();
            setlabelAlgorithmNameText(adbrCurrent.Name);
        }

        private void ChangeContacterAlgorithm(int algid)
        {
            if (iContUserID == -1)
                return;
            if (isAlgorithmExists(algid))
                SaveAlgorithmSettingsContacter(algid);
        }

        public void applyAlgorithm(int algid)
        {
            if (algid < 0)
                algid = -1;
            FormEditAlgorithms fe = new FormEditAlgorithms(this);
            fe.Setup(algid, false);
            //fe.SelectAlgorithm();
            if (!fe.SelectAlgorithm(true))
                SaveAlgorithmSettingsContacter();
            setlabelAlgorithmNameText(adbrCurrent.Name);
        }

        private int GetContacterAlgorithmID()
        {
            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt")))
            {
                int algid = -1;
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"));
                    lstTS = new List<String>(srcFile);
                    algid = Convert.ToInt32(lstTS[0]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    algid = -1;
                }

                if (algid < 0)
                    algid = -1;
                return algid;
            }
            return -1;
        }

        private void LoadAlgorithmSettingsContacter()
        {
            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt")))
            {
                int algid = -1;
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + iContUserID.ToString() + ".txt"));
                    lstTS = new List<String>(srcFile);
                    algid = Convert.ToInt32(lstTS[0]);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    algid = -1;
                }

                if (algid < 0)
                    algid = -1;

                FormEditAlgorithms fe = new FormEditAlgorithms(this);
                fe.Setup(algid, false);

                if (!fe.SelectAlgorithm())
                {
                    LoadAlgorithmSettings();
                    SaveAlgorithmSettingsContacter();
                }
                else
                {
                    if (adbrCurrent.Name.ToLower().Equals("error"))
                    {
                        LoadAlgorithmSettings();
                        SaveAlgorithmSettingsContacter();
                    }
                }

                setlabelAlgorithmNameText(adbrCurrent.Name);
            }
            else
            {
                if (iGroupAnswerID >= 0)
                {
                    if (File.Exists(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + iGroupAnswerID.ToString() + ".txt")))
                    {
                        int algid = -1;
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_-" + iGroupAnswerID.ToString() + ".txt"));
                            lstTS = new List<String>(srcFile);
                            algid = Convert.ToInt32(lstTS[0]);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                            algid = -1;
                        }

                        if (algid < 0)
                            algid = -1;
                        FormEditAlgorithms fe = new FormEditAlgorithms(this);
                        fe.Setup(algid, false);
                        //fe.SelectAlgorithm();
                        if (!fe.SelectAlgorithm())
                            SaveAlgorithmSettingsContacter();

                        setlabelAlgorithmNameText(adbrCurrent.Name);
                    }
                    else
                        LoadAlgorithmSettings();
                }
                else
                {
                    LoadAlgorithmSettings();
                    SaveAlgorithmSettingsContacter();
                }
            }

            setContacterWorkMode();
            sendPrevUserMessage();
        }

        private void SaveTextSearchSettings()
        {
            List<String> lstTS = new List<String>();
            lstTS.Add(TextSearchFilteredChars);
            lstTS.Add(TextSearchIgnoredWords);
            lstTS.Add(TextSearchMinWordsLen.ToString());
            lstTS.Add(TextSearchTextParsingChars);
            //lstTS.Add(CompareLexicalLevel.ToString());
            //lstTS.Add(CompareVectorLevel.ToString());

            FileWriteAllLines(Path.Combine(sDataPath, "_text_search_settings_" + adbrCurrent.ID.ToString() + ".txt"), lstTS, Encoding.UTF8);
        }

        public void LoadTextSearchSettings()
        {
            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_text_search_settings_" + adbrCurrent.ID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_text_search_settings_" + adbrCurrent.ID.ToString() + ".txt"));
                    lstTS = new List<String>(srcFile);

                    if (lstTS.Count > 0) TextSearchFilteredChars = lstTS[0];
                    if (lstTS.Count > 1) TextSearchIgnoredWords = lstTS[1];
                    if (lstTS.Count > 2) TextSearchMinWordsLen = Convert.ToInt32(lstTS[2]);
                    if (lstTS.Count > 3) TextSearchTextParsingChars = lstTS[3]; else TextSearchTextParsingChars = ".|...|!|?|<br>";
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    TextSearchFilteredChars = ".,";
                    TextSearchIgnoredWords = "из|под|с|по|в|";
                    TextSearchMinWordsLen = 1;
                    TextSearchTextParsingChars = ".|...|!|?|<br>";

                    SaveTextSearchSettings();
                }
            }
            else
            {
                TextSearchFilteredChars = ".,";
                TextSearchIgnoredWords = "из|под|с|по|в|";
                TextSearchMinWordsLen = 1;
                TextSearchTextParsingChars = ".|...|!|?|<br>";

                SaveTextSearchSettings();
            }
        }

        public static int MsgKoefFromString(String sValue)
        {
            int retval = 10;

            if (dictMsgKoefStrings.Keys.Contains(sValue))
                retval = dictMsgKoefStrings[sValue];

            return retval;
        }

        public static String MsgKoefToString(int iKoef)
        {
            String retval = "Важно"; //!!!

            if (dictMsgKoefValues.Keys.Contains(iKoef))
                retval = dictMsgKoefValues[iKoef];

            return retval;
        }

        private String MsgKoefToImage(int koef)
        {
            switch (koef)
            {
                case 1111:
                    return "10";// global::Nilsa.Properties.Resources.star_red_full;
                case 100:
                    return "9";// global::Nilsa.Properties.Resources.star_orange_full;
                case 10:
                    return "5";// global::Nilsa.Properties.Resources.star_green_full1;
                case 1:
                    return "1";// global::Nilsa.Properties.Resources.star_blue_none;
                case 0:
                    return " ";// null;
            }
            return null;
        }

        private void CompareVetors_RestoreDefaultValues()
        {
            try
            {
                comboBoxCompareVectorLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareVectorLevel : CompareVectorLevel);
            }
            catch
            {

            }
            for (int i = 0; i < iMsgHarCount; i++)
            {
                iCompareVectorsKoef[i] = (adbrCurrent.ID >= 0 ? adbrCurrent.iMsgHarKoef[i] : iMsgHarKoef[i]);
                iCompareVectorsKoefOut[i] = (adbrCurrent.ID >= 0 ? adbrCurrent.iMsgHarKoef[i] : iMsgHarKoef[i]);
                lblMsgHarNames[i].Text = sMsgHar[i, 1];
                toolTipMessage.SetToolTip(lblMsgHarNames[i], sMsgHar[i, 1]);
                string _picfile = Path.Combine(Application.StartupPath, "Images\\_msg_har_" + i.ToString() + ".png");
                if (File.Exists(_picfile))
                {
                    FileStream stream = new FileStream(_picfile, FileMode.Open, FileAccess.Read);
                    lblMsgHarNames[i].Image = Image.FromStream(stream);
                    stream.Close();
                }
                else
                    lblMsgHarNames[i].Image = Nilsa.Properties.Resources.labelbg;

                //lblEQInHarNames[i].Text = MsgKoefToString(iCompareVectorsKoef[i]);
                //lblEQOutHarNames[i].Text = MsgKoefToString(iCompareVectorsKoefOut[i]);
                toolTipMessage.SetToolTip(lblEQInHarNames[i], MsgKoefToString(iCompareVectorsKoef[i]));
                toolTipMessage.SetToolTip(lblEQOutHarNames[i], MsgKoefToString(iCompareVectorsKoefOut[i]));
                lblEQInHarNames[i].OwnerDrawText = MsgKoefToImage(iCompareVectorsKoef[i]);
                lblEQOutHarNames[i].OwnerDrawText = MsgKoefToImage(iCompareVectorsKoefOut[i]);
            }
        }

        private Boolean moveLastContacterMessageToTop()
        {
            if (iContUserID != -1 && lstReceivedMessages.Count > 1 /*&& iGroupAnswerID < 0*/)
            {
                for (int i = 1; i < lstReceivedMessages.Count; i++)
                {
                    String value = lstReceivedMessages[i];
                    value = value.Substring(value.IndexOf("|") + 1);
                    String strUsrId = value.Substring(0, value.IndexOf("|"));
                    long iCurMsgContUserID = -1;
                    long iCurGroupAnswerID = -1;
                    long iCurGroupAnswerPostID = -1;
                    long iCurGroupAnswerCommentID = -1;
                    //if (iGroupAnswerID >= 0)
                    //{
                    if (strUsrId.IndexOf('/') > 0)
                    {
                        //Convert.ToString(_ownerId)+"/"+ Convert.ToString(_postId) + "/"+ Convert.ToString(msg.Id) + "/" + Convert.ToString(msg.FromId) + "|";
                        iCurGroupAnswerID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                        strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                        iCurGroupAnswerPostID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                        strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                        iCurGroupAnswerCommentID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                        strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                        iCurMsgContUserID = Convert.ToInt64(strUsrId);
                    }
                    else
                        iCurMsgContUserID = Convert.ToInt64(strUsrId);
                    //}
                    //else
                    //    iCurMsgContUserID = Convert.ToInt64(strUsrId);

                    if (iCurMsgContUserID == iContUserID && iCurGroupAnswerID == iGroupAnswerID && iCurGroupAnswerPostID == iGroupAnswerPostID && iCurGroupAnswerCommentID == iGroupAnswerCommentID)
                    {
                        if (i == 1)
                            return adbrCurrent.bGroupingOutMessages;
                        else
                        {
                            String rec = lstReceivedMessages[i];
                            lstReceivedMessages.RemoveAt(i);
                            lstReceivedMessages.Insert(1, rec);
                            return adbrCurrent.bGroupingOutMessages;
                        }
                    }
                }

            }
            return false;
        }

        private void clearlblEQOutHarValues()
        {
            for (int i = 0; i < iMsgHarCount; i++)
            {
                lblEQOutHarValues[i].Text = "";
                toolTipMessage.SetToolTip(lblEQOutHarValues[i], "");
                lblMsgHarCompare[i].Image = global_Nilsa_Properties_Resources_empty_image; //global::Nilsa.Properties.Resources.trafficlight_off_16;
                lblMsgHarCompare[i].BackColor = SystemColors.Control;
                toolTipMessage.SetToolTip(lblMsgHarCompare[i], "");
            }
        }

        private void api_Messages_MarkAsRead(long _iInMsgID)
        {
            return;

            /*
            try
            {
                api.Messages.MarkAsRead(_iInMsgID);
            }
            catch (VkNet.Exception.AccessTokenInvalidException atiexp)
            {
                ReAutorize(userLogin, userPassword);
            }
            catch (System.Net.WebException)
            {
                ReAutorize(userLogin, userPassword);
            }
            catch (VkNet.Exception.VkApiException exp2)
            {
                ReAutorize(userLogin, userPassword);
            }
            catch (Exception exp)
            {
                ExceptionToLogList("api_Messages_MarkAsRead", "UNKNOWN", exp);
            }
            */
        }

        private string mergeContacterMessageToTop(string message)
        {
            if (iInMsgID <= 0 || message.StartsWith("INIT_GROUP_DIALOG|") || message.StartsWith("INIT_PERSONE_GROUPS_DIALOG|") || message.StartsWith("INIT_DIALOG|") || message.StartsWith("ERROR_SEND_MESSAGE") || message.StartsWith("DIALOG_DONE") || message.Equals("INIT_PERSONE_DIALOG") || message.Equals("CLEAR_PERSONE_DIALOGS"))
                return message;

            string messageText = message;

            if (iContUserID >= 0 && lstReceivedMessages.Count > 1)
            {
                int i = 1;

                while (i < lstReceivedMessages.Count)
                {
                    String value = lstReceivedMessages[i];
                    long iCur_iInMsgID = Convert.ToInt64(value.Substring(0, value.IndexOf("|")));
                    value = value.Substring(value.IndexOf("|") + 1);
                    long iCur_iContUserID = Convert.ToInt64(value.Substring(0, value.IndexOf("|")));
                    value = value.Substring(value.IndexOf("|") + 1);
                    String iCur_sDate = value.Substring(0, value.IndexOf("|"));
                    value = value.Substring(value.IndexOf("|") + 1);
                    String iCur_sTime = value.Substring(0, value.IndexOf("|"));
                    value = value.Substring(value.IndexOf("|") + 1);

                    if (iCur_iContUserID == iContUserID)
                    {
                        if (iCur_iInMsgID <= 0 || message.StartsWith("INIT_GROUP_DIALOG|") || message.StartsWith("INIT_PERSONE_GROUPS_DIALOG|") || value.StartsWith("INIT_DIALOG|") || value.StartsWith("ERROR_SEND_MESSAGE") || value.StartsWith("DIALOG_DONE") || value.Equals("INIT_PERSONE_DIALOG") || value.Equals("CLEAR_PERSONE_DIALOGS"))
                            break;

                        if (SocialNetwork == 0)
                        {
                            if (iCur_iInMsgID > 0)
                                api_Messages_MarkAsRead(iCur_iInMsgID);
                        }

                        messageText += "<br>" + value;
                        lstReceivedMessages.RemoveAt(i);
                    }
                    else
                        i++;
                }

            }
            return messageText;
        }

        List<AlgorithmsDBRecord> lstAlgorithmsRecordsList;

        private void LoadAlgorithmsDB()
        {
            List<String> lstAlgorithmsList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"));
                    lstAlgorithmsList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstAlgorithmsList = new List<String>();
                }
            }

            lstAlgorithmsRecordsList = new List<AlgorithmsDBRecord>();
            foreach (String value in lstAlgorithmsList)
            {
                AlgorithmsDBRecord dbr = AlgorithmsDBRecord.FromRecordString(value);
                lstAlgorithmsRecordsList.Add(dbr);
            }
        }

        private bool ResendFromOperators(string msg)
        {
            if (iResendContUserID >= 0)
            {
                if (iPersUserID >= 0 && iContUserID >= 0 && SocialNetwork == 0)
                {
                    bool bNeedSend = false;
                    String soperators = lblPersHarValues[13].Text.Trim() + ",";
                    string[] sOperIDs = soperators.Split(',');

                    long lgid = -1;
                    foreach (string soperid in sOperIDs)
                    {
                        string soper = soperid.Trim();
                        if (soper.Length > 0)
                        {
                            lgid = -1;
                            try
                            {
                                lgid = Convert.ToInt64(soper);
                            }
                            catch
                            {
                                lgid = -1;
                            }

                            if (lgid >= 0)
                            {
                                if (iContUserID != lgid)
                                    continue;

                                try
                                {
                                    if (msg.ToLower().StartsWith("resend_operator ") || msg.ToLower().StartsWith("напиши "))
                                    {
                                        iResendContUserID = -1;
                                        SaveResendContUserID();
                                        return false;
                                    }
                                    else
                                    {
                                        bNeedSend = true;
                                        api_Messages_Send(iResendContUserID, msg);
                                        //api.Messages.Send(iResendContUserID, false, msg); // посылаем сообщение пользователю
                                    }
                                    break;
                                }
                                /*
                                catch (VkNet.Exception.AccessTokenInvalidException)
                                {
                                    ReAutorize(userLogin, userPassword);
                                }
                                catch (System.Net.WebException)
                                {
                                    ReAutorize(userLogin, userPassword);
                                }
                                catch (VkNet.Exception.VkApiException vkapexeption)
                                {
                                    ReAutorize(userLogin, userPassword);
                                }
                                */
                                catch (Exception exp)
                                {
                                    ExceptionToLogList("Setup", "UNKNOWN", exp);
                                }
                            }
                        }
                    }

                    if (bNeedSend)
                        timerPhysicalSendStart();

                    if (iContUserID == lgid)
                        return true;
                }
            }
            return false;
        }

        private bool CheckOperator(long _id)
        {
            String soperators = lblPersHarValues[13].Text.Trim() + ",";
            string[] sOperIDs = soperators.Split(',');

            if (_id >= 0 && iPersUserID >= 0 && SocialNetwork == 0)
            {
                foreach (string soperid in sOperIDs)
                {
                    string soper = soperid.Trim();
                    if (soper.Length > 0)
                    {
                        long lgid = -1;
                        try
                        {
                            lgid = Convert.ToInt64(soper);
                        }
                        catch
                        {
                            lgid = -1;
                        }

                        if (lgid >= 0)
                        {
                            if (lgid == _id)
                                return true;
                        }
                    }
                }

            }
            return false;
        }

        private bool ResendToOperators(bool _sendphoto = false)
        {
            if (iContUserID != iResendContUserID)
                return false;

            bool bNeedSend = false;
            LoadContactParamersValues();
            SetContactParametersValues();
            ReadAllUserMessages(iContUserID);
            String soperators = lblPersHarValues[13].Text.Trim() + ",";
            string[] sOperIDs = soperators.Split(',');
            //bool bStatusService = !tbStartService.Enabled;
            //tbStopService_Click(null, null);

            if (iPersUserID >= 0 && SocialNetwork == 0)
            {
                string attachlist = null;
                string msg = "Мне пишет\n";
                msg += labelCont1FIO.Text + "\n";
                msg += "http://vk.com/id" + iContUserID.ToString() + "\n";
                msg += iContUserID.ToString();

                /*
                if (_sendphoto)
                {
                    VkNet.Model.User usrAdr = FormMain.api.Users.Get(iContUserID, ProfileFields.PhotoId);
                    if (usrAdr.PhotoId != null)
                    {
                        if (usrAdr.PhotoId.Length > 0)
                        {
                            attachlist = "photo" + usrAdr.PhotoId;

                            foreach (string soperid in sOperIDs)
                            {
                                string soper = soperid.Trim();
                                if (soper.Length > 0)
                                {
                                    long lgid = -1;
                                    try
                                    {
                                        lgid = Convert.ToInt64(soper);
                                    }
                                    catch
                                    {
                                        lgid = -1;
                                    }

                                    if (lgid >= 0)
                                    {
                                        bNeedSend = true;
                                        api_Messages_Send(lgid, msg, attachlist);
                                        //try
                                        //{
                                        //    api.Messages.Send(lgid, false, msg, "", null, null, false, null, null, null, null, null, attachlist); // посылаем сообщение пользователю
                                        //    Delay(350);
                                        //}
                                        //catch (VkNet.Exception.AccessTokenInvalidException)
                                        //{
                                        //    ReAutorize(userLogin, userPassword);
                                        //}
                                        //catch (System.Net.WebException)
                                        //{
                                        //    ReAutorize(userLogin, userPassword);
                                        //}
                                        //catch (VkNet.Exception.VkApiException vkapexeption)
                                        //{
                                        //    ReAutorize(userLogin, userPassword);
                                        //}
                                        //catch (Exception exp)
                                        //{
                                        //    ExceptionToLogList("Setup", "UNKNOWN", exp);
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
                */
                if (/*_sendphoto && */!bNeedSend)
                {
                    foreach (string soperid in sOperIDs)
                    {
                        string soper = soperid.Trim();
                        if (soper.Length > 0)
                        {
                            long lgid = -1;
                            try
                            {
                                lgid = Convert.ToInt64(soper);
                            }
                            catch
                            {
                                lgid = -1;
                            }

                            if (lgid >= 0)
                            {
                                bNeedSend = true;
                                api_Messages_Send(lgid, msg);
                            }
                        }
                    }
                }

                if (_sendphoto)
                    msg = "";
                else
                    msg += "\n\n";

                int iStart = listBoxUserMessages.Items.Count - 25;
                if (iStart < 0)
                    iStart = 0;
                String historyItemPrevDate = "";
                for (int i = iStart; i < listBoxUserMessages.Items.Count; i++)
                {
                    String value = listBoxUserMessages.Items[i].ToString();
                    String historyItemFlag = value.Substring(0, value.IndexOf(" "));
                    bool bPerson = historyItemFlag[0] == '-';
                    value = value.Substring(value.IndexOf(" ") + 1);
                    String historyItemDate = value.Substring(0, value.IndexOf(" "));
                    value = value.Substring(value.IndexOf(" ") + 1);
                    String historyItemTime = value.Substring(0, value.IndexOf(" "));
                    value = value.Substring(value.IndexOf(" ") + 3);
                    String historyItemText = /*NilsaUtils.StringToText(*/
            value;//);
                    if (!historyItemPrevDate.Equals(historyItemDate))
                    {
                        msg += (i > iStart ? "\n" : "") + historyItemDate + "\n";
                        historyItemPrevDate = historyItemDate;
                    }
                    //msg += historyItemTime+" ("+(bPerson ? labelPers1FIO.Text : labelCont1FIO.Text) + "): "+ historyItemText + "\n";
                    //msg += historyItemTime + ", " + (bPerson ? labelPers1FIO.Text : labelCont1FIO.Text) + ": " + historyItemText + "\n";
                    msg += (bPerson ? labelPers1FIO.Text : labelCont1FIO.Text) + ", " + historyItemTime + ": " + historyItemText + "\n";
                }

                foreach (string soperid in sOperIDs)
                {
                    string soper = soperid.Trim();
                    if (soper.Length > 0)
                    {
                        long lgid = -1;
                        try
                        {
                            lgid = Convert.ToInt64(soper);
                        }
                        catch
                        {
                            lgid = -1;
                        }

                        if (lgid >= 0)
                        {
                            bNeedSend = true;
                            api_Messages_Send(lgid, msg);
                            //try
                            //{
                            //    api.Messages.Send(lgid, false, msg); // посылаем сообщение пользователю
                            //    Delay(350);
                            //}
                            //catch (VkNet.Exception.AccessTokenInvalidException)
                            //{
                            //    ReAutorize(userLogin, userPassword);
                            //}
                            //catch (System.Net.WebException)
                            //{
                            //    ReAutorize(userLogin, userPassword);
                            //}
                            //catch (VkNet.Exception.VkApiException vkapexeption)
                            //{
                            //    ReAutorize(userLogin, userPassword);
                            //}
                            //catch (Exception exp)
                            //{
                            //    ExceptionToLogList("Setup", "UNKNOWN", exp);
                            //}
                        }
                    }
                }
            }

            //if (bStatusService)
            //    tbStartService_Click(null, null);

            if (bNeedSend)
                timerPhysicalSendStart();

            return true;
        }

        private bool doOperatorsCommand(String msg)
        {
            if (msg.Length == 0)
                return false;

            String[] commands = { "KNOBY1 ", "KNOBY2 ", "KNOBY3 ", "ACTION1 ", "ACTION2 ", "ACTION3 ", "DOITNOW1 ", "DOITNOW2 ", "DOITNOW3 " };
            bool bNotFound = true;
            String sCommand = "";
            foreach (string scmd in commands)
            {
                if (msg.StartsWith(scmd))
                {
                    bNotFound = false;
                    sCommand = scmd;
                    break;
                }
            }

            if (bNotFound)
                return false;

            String text = msg.Substring(msg.IndexOf(' ') + 1);
            string sUID = "";

            if (text.IndexOf(' ') > 0)
            {
                sUID = text.Substring(0, text.IndexOf(' '));
                text = text.Substring(text.IndexOf(' ') + 1);
            }
            else
            {
                sUID = text;
                text = "";
            }


            long lgid = -1;
            try
            {
                lgid = Convert.ToInt64(sUID);
            }
            catch
            {
                lgid = -1;
            }

            if (lgid == -1)
                return false;



            DateTime dt = DateTime.Now;
            String sCurRec = "0|" + sUID + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + NilsaUtils.TextToString(sCommand + text);
            lstReceivedMessages.Insert(1, sCurRec);

            if (sCommand.StartsWith("KNOBY"))
            {
                SaveInitDialogFlag(sUID);
                //Set_labelInMsgHarTitleValue_Text("INIT_DIALOG");
                cntD1++;
                cntD7++;
                SaveProgramCountersD1D2D3();
                UpdateProgramCountersInfoD1D2D3();

                cntD4++;
                SaveProgramCountersD4D5D6();
                UpdateProgramCountersInfoD4D5D6();
            }
            return true;
        }

        private void SelectNextReceivedMessage(Boolean bDelete = true, bool bBoycootCurrent = false)
        {
            long iLastContacterID = -1;
            long iLastGroupAnswerID = -1;
            long iLastGroupAnswerPostID = -1;
            long iLastGroupAnswerCommentID = -1;

            //timerWriteMessages.Enabled = false;
            //timerSkipMessage.Enabled = false;

            //Set_pbSkipMessage_Default();
            timerWriteMessagesOff();

            tbUndoMarkerChanges.Enabled = false;
            if (bDelete)
            {
                if (lstReceivedMessages.Count > 0)
                    lstReceivedMessages.RemoveAt(0);

                if (lstReceivedMessages.Count == 0)
                    ReadNewReceivedMessages(bBoycootCurrent);
            }
            else
                ReadNewReceivedMessages(bBoycootCurrent);

            if (iContUserID != -1)
            {
                iLastContacterID = iContUserID;
                iLastGroupAnswerID = iGroupAnswerID;
                iLastGroupAnswerPostID = iGroupAnswerPostID;
                iLastGroupAnswerCommentID = iGroupAnswerCommentID;
            }
            else
            {
                if (SocialNetwork == 1)
                {
                    if (iPersUserID == 0)
                        iLastContacterID = 1;
                }
            }

            iContUserID = -1;
            iGroupAnswerID = -1;
            iGroupAnswerPostID = -1;
            iGroupAnswerCommentID = -1;
            //labelCont1.Text = "";
            //cbCont1.SelectedIndex = -1;
            contName = contNameFamily = contNameName = "";
            labelCont1Family.Text = "";
            labelCont1Name.Text = "";
            labelCont1FIO.Text = "";
            sGroupAdditinalUsers = "";
            toolTipMessage.SetToolTip(labelCont1FIO, labelCont1FIO.Text);
            for (int i = 0; i < iContHarCount; i++)
            {
                lblContHarValues[i].Text = "";
                //toolTipMessage.SetToolTip(lblContHarNames[i], lblContHarNames[i].Text);
                toolTipMessage.SetToolTip(lblContHarValues[i], "");
            }
            iInMsgID = -1;
            //labelInMsgHarDateValue.Text = "";
            //labelInMsgHarTimeValue.Text = "";
            Set_labelInMsgHarTitleValue_Text("");
            try
            {
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
            }
            catch
            {

            }


            listBoxInMsg.Items.Clear();
            listBoxOutMsg.Items.Clear();
            //buttonEditInMsgHar.Enabled = false;
            //buttonNewOutMsgHar.Enabled = false;
            //buttonEditInEqMsgHar.Enabled = true;
            //buttonEditOutEqMsgHar.Enabled = true;
            tbSkipMessage.Enabled = false;
            tbDeleteMessage.Enabled = false;
            tbSendOutMessage.Enabled = false;
            Set_labelInEqMsgHarTitleValue_Text("");
            CompareVetors_RestoreDefaultValues();

            clearlblEQInHarValues();
            Set_labelOutEqMsgHarTitleValue_Text("");

            clearlblEQOutHarValues();
            sCurrentEQInMessageRecord = "";
            sCurrentEQOutMessageRecord = "";
            sCurrentEQOutMessageRecordOut = "";

            //? Зачистка подобного входящего и возможного исходящего сообщения
            labelPersMsgCount.Text = lstReceivedMessages.Count.ToString();
            if (lstReceivedMessages.Count > 0)
            {

                String value = lstReceivedMessages[0];
                iInMsgID = Convert.ToInt64(value.Substring(0, value.IndexOf("|")));
                value = value.Substring(value.IndexOf("|") + 1);
                if (iInMsgID > 0)
                    iContUserID = Convert.ToInt64(value.Substring(0, value.IndexOf("|")));
                else
                {
                    string strUsrId = value.Substring(0, value.IndexOf("|"));
                    if (strUsrId.IndexOf('/') > 0)
                    {
                        //Convert.ToString(_ownerId)+"/"+ Convert.ToString(_postId) + "/"+ Convert.ToString(msg.Id) + "/" + Convert.ToString(msg.FromId) + "|";
                        iGroupAnswerID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                        strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                        iGroupAnswerPostID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                        strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                        iGroupAnswerCommentID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                        strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                        iContUserID = Convert.ToInt64(strUsrId);
                    }
                    else
                        iContUserID = Convert.ToInt64(strUsrId);
                }
                cntE1++;
                cntE3++;
                cntE7++;
                cntE9++;
                SaveProgramCountersE1E2E3();
                UpdateProgramCountersInfoE1E2E3();

                cntE4++;
                cntE6++;
                SaveProgramCountersE4E5E6();
                UpdateProgramCountersInfoE4E5E6();

                if (contC9 != iContUserID)
                {
                    contC9 = iContUserID;
                    cntC9 = 0;
                    UpdateProgramCountersInfoC8C9();
                }

                sendPrevUserMessage();
                LoadContactParamersValues();
                LoadAlgorithmSettingsContacter();

                if (adbrCurrent.Name.ToLower().Equals("boycott"))
                {
                    SelectNextReceivedMessage(true, true);
                    return;
                }

                if (SocialNetwork == 0)
                {
                    if (iInMsgID > 0)
                        api_Messages_MarkAsRead(iInMsgID);
                }

                if (adbrCurrent.PlayReceiveMsg)
                {
                    Stream str = Properties.Resources._2;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();
                }

                if (iContUserID == 330305148)
                {
                    SelectNextReceivedMessage();
                    return;
                }

                if (ResendToOperators())
                {
                    SelectNextReceivedMessage();
                    return;
                }

                value = value.Substring(value.IndexOf("|") + 1);
                String sDate = value.Substring(0, value.IndexOf("|"));
                value = value.Substring(value.IndexOf("|") + 1);
                String sTime = value.Substring(0, value.IndexOf("|"));
                value = value.Substring(value.IndexOf("|") + 1);

                if (doOperatorsCommand(value))
                {
                    SelectNextReceivedMessage();
                    return;
                }

                if (ResendFromOperators(value))
                {
                    SelectNextReceivedMessage();
                    return;
                }

                if (iContUserID != iPersUserID)
                {
                    if (iContUserID >= 0 && iContUserID != 330643598)
                    {
                        if (ContactsList_GetUserIdx(iContUserID.ToString()) < 0)
                        {
                            if (adbrCurrent.bIgnoreMessagesFromNotContacter)
                            {
                                SelectNextReceivedMessage();
                                return;
                            }
                            else
                            {
                                if (iContactsGroupsMode == 0)
                                    ContactsList_AddUser(iContUserID.ToString(), "");
                            }
                        }
                    }
                }

                if (adbrCurrent.MergeInMessages)
                {
                    value = mergeContacterMessageToTop(value);
                }

                LoadLastMessage();

                if (value.Contains("<ADD_PERS_LIST>"))
                {
                    sGroupAdditinalUsers = "<ADD_PERS_LIST>" + getTagValue(value, "<ADD_PERS_LIST>", "</ADD_PERS_LIST>") + "</ADD_PERS_LIST>";
                    value = removeTagValue(value, "<ADD_PERS_LIST>", "</ADD_PERS_LIST>");
                }

                if (value.StartsWith("PERSONE_CHANGED"))
                {
                    SelectNextReceivedMessage();
                    return;
                }
                if (value.StartsWith("INIT_DIALOG|"))
                {
                    SaveInitDialogFlag();
                    Set_labelInMsgHarTitleValue_Text("INIT_DIALOG");
                    cntD1++;
                    cntD7++;
                    SaveProgramCountersD1D2D3();
                    UpdateProgramCountersInfoD1D2D3();

                    cntD4++;
                    SaveProgramCountersD4D5D6();
                    UpdateProgramCountersInfoD4D5D6();
                }
                else if (value.StartsWith("INIT_GROUP_DIALOG|"))
                {
                    SaveInitDialogFlag();
                    Set_labelInMsgHarTitleValue_Text("INIT_GROUP_DIALOG");
                    cntD1++;
                    cntD7++;
                    SaveProgramCountersD1D2D3();
                    UpdateProgramCountersInfoD1D2D3();

                    cntD4++;
                    SaveProgramCountersD4D5D6();
                    UpdateProgramCountersInfoD4D5D6();
                }
                else
                {
                    if (CheckInitDialogFlag())
                    {
                        if (!value.StartsWith("ERROR_SEND_MESSAGE"))
                        {
                            cntD2++;
                            cntD8++;
                            SaveProgramCountersD1D2D3();
                            UpdateProgramCountersInfoD1D2D3();

                            cntD5++;
                            SaveProgramCountersD4D5D6();
                            UpdateProgramCountersInfoD4D5D6();
                        }
                    }
                    else if (value.StartsWith("DIALOG_DONE"))
                    {
                        cntD3++;
                        cntD9++;
                        SaveProgramCountersD1D2D3();
                        UpdateProgramCountersInfoD1D2D3();

                        cntD6++;
                        SaveProgramCountersD4D5D6();
                        UpdateProgramCountersInfoD4D5D6();
                    }

                    if (adbrCurrent.SplitTextIntoSentences)
                    {
                        String msgSentenceCurrent = SplitTextIntoSentences(value);
                        String msgSentenceEnd = value.Substring(msgSentenceCurrent.Length).Trim();
                        value = msgSentenceCurrent.Trim();
                        if (msgSentenceEnd.Length > 0)
                        {
                            String sCurRec = iInMsgID.ToString() + "|" + getContUserIDWithGroupID() + "|" + sDate + "|" + sTime + "|" + value;
                            lstReceivedMessages[0] = sCurRec;
                            String sEndRec = iInMsgID.ToString() + "|" + getContUserIDWithGroupID() + "|" + sDate + "|" + sTime + "|" + msgSentenceEnd;
                            lstReceivedMessages.Add(sEndRec);
                        }
                    }

                    value = SetMessageFields(value);
                    Set_labelInMsgHarTitleValue_Text(NilsaUtils.StringToText(value));
                }

                if (comboBoxCompareLexicalLevel != null)
                    comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                //buttonEditInMsgHar.Enabled = true;
                //buttonNewOutMsgHar.Enabled = true;

                LoadContactParamersValues();
                SetContactParametersValues();
                UpdateContactParametersValues_Algorithm();
                //buttonEditContHarValues.Enabled = true;

                lstUndoMarkerChangesContHarValues = new List<string>();
                foreach (string _str in lstContHarValues)
                    lstUndoMarkerChangesContHarValues.Add(_str);
                iUndoMarkerChangesContHarValuesContID = iContUserID;
                iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                sUndoMarkerCurrentEQInMessageRecord = "";
                tbUndoMarkerChanges.Enabled = true;

                tbSkipMessage.Enabled = true;
                tbDeleteMessage.Enabled = true;

                // 2019-04-13
                ReadAllUserMessages(iContUserID);
                if (value.StartsWith("INIT_DIALOG|"))
                {
                    //ReadAndMarkAsReadedNewReceivedMessages(iContUserID);
                    int iMsgPos = 1;
                    while (iMsgPos < lstReceivedMessages.Count)
                    {
                        string _smv = lstReceivedMessages[iMsgPos];
                        _smv = _smv.Substring(_smv.IndexOf("|") + 1);
                        long _imuid = Convert.ToInt64(_smv.Substring(0, _smv.IndexOf("|")));
                        if (_imuid == iContUserID)
                            lstReceivedMessages.RemoveAt(iMsgPos);
                        else
                            iMsgPos++;
                    }

                    value = value.Substring(value.IndexOf("|") + 1);
                    listBoxInMsg.Items.Clear();
                    clearlblEQInHarValues();
                    Set_labelOutEqMsgHarTitleValue_Text(value.Substring(0, value.IndexOf("|")));
                    value = value.Substring(value.IndexOf("|") + 1);
                    if (value.Length > 0)
                    {
                        lstEQOutMessagesList.Clear();
                        lstEQOutMessagesList.Add(value);
                        listBoxOutMsg.Items.Clear();
                        listBoxOutMsg.Items.Add("100% " + GetMessageTextWithMarker(value.Substring(value.IndexOf("|@!") + 3)));
                        listBoxOutMsg.SelectedIndex = 0;
                    }
                }
                else if (value.StartsWith("INIT_GROUP_DIALOG|"))
                {
                    // 2019-04-13
                    //ReadAndMarkAsReadedNewReceivedMessages(iContUserID);
                    int iMsgPos = 1;
                    while (iMsgPos < lstReceivedMessages.Count)
                    {
                        string _smv = lstReceivedMessages[iMsgPos];
                        _smv = _smv.Substring(_smv.IndexOf("|") + 1);
                        long _imuid = -1;
                        try
                        {
                            _imuid = Convert.ToInt64(_smv.Substring(0, _smv.IndexOf("|")));
                        }
                        catch (Exception e)
                        {

                        }
                        if (_imuid == iContUserID)
                            lstReceivedMessages.RemoveAt(iMsgPos);
                        else
                            iMsgPos++;
                    }

                    value = value.Substring(value.IndexOf("|") + 1);
                    listBoxInMsg.Items.Clear();
                    clearlblEQInHarValues();
                    Set_labelOutEqMsgHarTitleValue_Text(value.Substring(0, value.IndexOf("|")));
                    value = value.Substring(value.IndexOf("|") + 1);
                    if (value.Length > 0)
                    {
                        lstEQOutMessagesList.Clear();
                        lstEQOutMessagesList.Add(value);
                        listBoxOutMsg.Items.Clear();
                        listBoxOutMsg.Items.Add("100% " + GetMessageTextWithMarker(value.Substring(value.IndexOf("|@!") + 3)));
                        listBoxOutMsg.SelectedIndex = 0;
                    }
                }
                else if (value.Equals("INIT_PERSONE_DIALOG")/*labelInMsgHarTitleValue_Text.Equals("INIT_PERSONE_DIALOG")*/)
                {
                    Set_labelOutEqMsgHarTitleValue_Text("EXECUTE...");
                    lstEQOutMessagesList.Clear();
                    listBoxOutMsg.Items.Clear();
                    listBoxInMsg.Items.Clear();
                    INIT_PERSONE_DIALOG = true;
                    btnInitContactsDialog_Click(null, null);
                    INIT_PERSONE_DIALOG = false;
                    SelectNextReceivedMessage();
                    //return;
                    //---

                }
                else if (value.Equals("INIT_PERSONE_GROUPS_DIALOG")/*labelInMsgHarTitleValue_Text.Equals("INIT_PERSONE_DIALOG")*/)
                {
                    Set_labelOutEqMsgHarTitleValue_Text("EXECUTE...");
                    lstEQOutMessagesList.Clear();
                    listBoxOutMsg.Items.Clear();
                    listBoxInMsg.Items.Clear();
                    INIT_PERSONE_DIALOG = true;
                    btnInitGroupsDialog_Click(null, null);
                    // InitContactsDialog_Click(null, null);
                    INIT_PERSONE_DIALOG = false;
                    SelectNextReceivedMessage();
                    //return;
                    //---

                }
                else if (value.Equals("CLEAR_PERSONE_DIALOGS"))
                {
                    Set_labelOutEqMsgHarTitleValue_Text("EXECUTE...");
                    lstEQOutMessagesList.Clear();
                    while (lstReceivedMessages.Count > 1)
                        lstReceivedMessages.RemoveAt(1);
                    ReadAndMarkAsReadedNewReceivedMessages();
                    SelectNextReceivedMessage();
                }
                else
                    SetEQInMessageList(labelInMsgHarTitleValue_Text);
            }
            else
            {
                sendPrevUserMessage();
                if (iLastContacterID != -1)
                {
                    iContUserID = iLastContacterID;
                    iGroupAnswerID = iLastGroupAnswerID;
                    iGroupAnswerPostID = iLastGroupAnswerPostID;
                    iGroupAnswerCommentID = iLastGroupAnswerCommentID;

                    if (contC9 != iContUserID)
                    {
                        contC9 = iContUserID;
                        cntC9 = 0;
                        UpdateProgramCountersInfoC8C9();
                    }
                    ContactsList_Load();
                    LoadContactParamersValues();
                    SetContactParametersValues();
                    LoadAlgorithmSettingsContacter();
                    UpdateContactParametersValues_Algorithm();

                    //LoadContactParametersDescription();

                    OnSelectOtherContacter(false);
                }
                else
                    listBoxUserMessages.Items.Clear();
            }
            tbNewOutMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            tbNewInMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            tbInitContactDialogContacter.Enabled = iPersUserID >= 0 && (iContUserID >= 0 || iContUserID < -1);
            tbDeleteContacterMessages.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            //UpdatePersoneParametersValues_Friends();
            UpdatePersoneParametersValues_Algorithm();
        }

        int iUndoMarkerChangesAlgorithm;
        List<String> lstUndoMarkerChangesContHarValues = null;
        long iUndoMarkerChangesContHarValuesContID = -1;

        private String getContUserIDWithGroupID()
        {
            string value;
            if (iGroupAnswerID >= 0)
                value = Convert.ToString(iGroupAnswerID) + "/" + Convert.ToString(iGroupAnswerPostID) + "/" + Convert.ToString(iGroupAnswerCommentID) + "/" + Convert.ToString(iContUserID);
            else
                value = Convert.ToString(iContUserID);

            return value;
        }

        private String SplitTextIntoSentences(String _text)
        {
            String text = _text;
            String[] splitsymbols = TextSearchTextParsingChars.Split('|');
            int iSentence = 9999999;
            foreach (String str in splitsymbols)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                int iCurSym = text.IndexOf(str);
                if (iCurSym > 0)
                {
                    if (iCurSym < iSentence)
                        iSentence = iCurSym;
                }
            }

            if (iSentence < 9999999)
            {
                text = text.Substring(0, iSentence);
                _text = _text.Substring(iSentence);

                bool doCycle = true;
                while (doCycle)
                {
                    doCycle = false;
                    foreach (String str in splitsymbols)
                    {
                        if (_text.Length == 0)
                            break;

                        if (str == null)
                            continue;

                        if (str.Length == 0)
                            continue;

                        if (_text.StartsWith(str) && str.Length > 0)
                        {
                            text = text + str;
                            _text = _text.Substring(str.Length);
                            doCycle = true;
                        }
                    }

                    foreach (char ch in TextSearchFilteredChars)
                    {
                        if (_text.Length == 0)
                            break;

                        if (_text[0] == ch)
                        {
                            text = text + ch;
                            _text = _text.Substring(1);
                            doCycle = true;
                        }
                    }

                    if (_text.Length > 0)
                        if (_text[0] == ' ')
                        {
                            text = text + ' ';
                            _text = _text.Substring(1);
                            doCycle = true;
                        }

                }
            }
            return text;
        }

        private void OnSelectOtherContacter(bool rereadHistory = true)
        {
            //timerWriteMessages.Enabled = false;
            //timerSkipMessage.Enabled = false;
            //Set_pbSkipMessage_Default();
            timerWriteMessagesOff();

            iInMsgID = -1;
            //labelInMsgHarDateValue.Text = "";
            //labelInMsgHarTimeValue.Text = "";
            Set_labelInMsgHarTitleValue_Text("");
            try
            {
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
            }
            catch
            {

            }
            //if (rereadHistory)
            listBoxUserMessages.Items.Clear();
            listBoxInMsg.Items.Clear();
            listBoxOutMsg.Items.Clear();
            //buttonEditContHarValues.Enabled = false;
            //buttonEditInMsgHar.Enabled = false;
            //buttonNewOutMsgHar.Enabled = false;
            //buttonEditInEqMsgHar.Enabled = false;
            //buttonEditOutEqMsgHar.Enabled = false;
            tbSkipMessage.Enabled = false;
            tbDeleteMessage.Enabled = false;
            tbSendOutMessage.Enabled = false;
            //tbDeleteInEQMessage.Enabled = false;
            //tbDeleteOutEQMessage.Enabled = false;
            Set_labelInEqMsgHarTitleValue_Text("");
            CompareVetors_RestoreDefaultValues();
            clearlblEQInHarValues();
            //labelInEqMsgHar2Value.Text = "";
            //labelInEqMsgHar3Value.Text = "";
            //labelInEqMsgHar4Value.Text = "";
            //cbEQOutMsg.SelectedIndex = -1;
            Set_labelOutEqMsgHarTitleValue_Text("");

            clearlblEQOutHarValues();

            sCurrentEQInMessageRecord = "";
            sCurrentEQOutMessageRecord = "";
            sCurrentEQOutMessageRecordOut = "";
            LoadLastMessage();

            // 2019-04-13
            //if (rereadHistory)
            ReadAllUserMessages(iContUserID);

        }

        private void SetPersoneParametersValues()
        {
            labelPers1Name.Text = userNameName;
            labelPers1Family.Text = userNameFamily;
            labelPers1FIO.Text = userNameName + " " + userNameFamily;
            toolTipMessage.SetToolTip(labelPers1FIO, labelPers1FIO.Text);

            //FormWebBrowser.Persone usrAdr = null;
            //if (fwbVKontakte != null)
            //{
            //    if (fwbVKontakteFirstShow)
            //    {
            //        if (fwbVKontakte.personeAtrributes != null)
            //            usrAdr = fwbVKontakte.personeAtrributes;
            //    }
            //}

            //if (usrAdr != null)
            //    if (usrAdr.id != fwbVKontakte.loggedPersoneID)
            //        usrAdr = null;
            //if (usrAdr == null)
            //    usrAdr = loadPersoneAttributes(fwbVKontakte.loggedPersoneID);

            //labelPers1Name.Text = usrAdr.FirstName;
            //labelPers1Family.Text = usrAdr.LastName;
            //labelPers1FIO.Text = usrAdr.FirstName + " " + usrAdr.LastName;
            //toolTipMessage.SetToolTip(labelPers1FIO, labelPers1FIO.Text);



            //contName = usrAdr.FirstName + " " + usrAdr.LastName;

            /*
            if (cbPers1.Items.Contains(userName))
                cbPers1.SelectedIndex = cbPers1.Items.IndexOf(userName);
             */

            LoadPersoneParametersDescription();
            UpdatePersoneParametersValues_Friends();
            UpdatePersoneParametersValues_Algorithm();
        }

        public Image contactPicture = null;
        private long contactPictureID = -1;
        public Image personPicture = null;
        private long personPictureID = -1;

        private void SetUserPictureFromID(long userid, Button button, bool bPerson)
        {
            Image bitmapPicture = null;
            button.BackgroundImage = null;

            if (userid >= 0)
            {
                if (SocialNetwork == 0)
                {
                    bool bNotCached = true;
                    if (bPerson)
                    {
                        if (personPictureID == userid && personPicture != null)
                        {
                            button.BackgroundImage = personPicture;
                            bNotCached = false;
                        }
                    }
                    else
                    {
                        if (contactPictureID == userid && contactPicture != null)
                        {
                            button.BackgroundImage = contactPicture;
                            bNotCached = false;
                        }
                    }

                    if (bNotCached)
                    {
                        ShowBrowserCommand();

                        fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.GetPhotoURL, userid);
                        //для взаимодействия через файловую систему
                        var stringJSON = vkInterface.Setup(userLogin, userPassword, WebBrowserCommand.GetPhotoURL, NilsaOperatingMode.SeleniumMode, userid);
                        //var result = JsonConvert.DeserializeObject<ResponseFromInterface>(stringJSON); //раскоментить при удалении встроенного браузера
                        //string photoURL = result.PhotoUrl;                                             //раскоментить при удалении встроенного браузера
                        fwbVKontakte.WaitResult();
                        string photoURL = fwbVKontakte.photoURL;
                        File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "photoURL: " + photoURL + "\n", Encoding.UTF8);

                        HideBrowserCommand();

                        bool bRead = false;
                        try
                        {
                            if (photoURL.Length > 0)
                            {
                                var request = WebRequest.Create(photoURL);

                                using (var response = request.GetResponse())
                                using (var stream = response.GetResponseStream())
                                {
                                    bitmapPicture = Bitmap.FromStream(stream);
                                    bRead = true;
                                    button.BackgroundImage = bitmapPicture;
                                    if (bPerson)
                                    {
                                        personPicture = bitmapPicture;
                                        personPictureID = userid;
                                        //btnB4.BackgroundImage = bitmapPicture;
                                    }
                                    else
                                    {
                                        contactPicture = bitmapPicture;
                                        contactPictureID = userid;
                                        //pathContacterImageFromGroup();
                                    }
                                }
                            }

                            if (!bRead)
                            {
                                if (bPerson)
                                {
                                    personPicture = null;
                                    personPictureID = -1;
                                    //btnB4.BackgroundImage = Nilsa.Properties.Resources.nilsa_pers;
                                }
                                else
                                {
                                    contactPicture = null;
                                    contactPictureID = -1;
                                }
                            }
                        }
                        catch (Exception){ }

                    }
                    /*
                    try
                    {
                        VkNet.Model.User usrAdr = api.Users.Get(userid, ProfileFields.Photo200);
                        bool bRead = false;
                        if (usrAdr.PhotoPreviews.Photo200 != null)
                            if (usrAdr.PhotoPreviews.Photo200.Length > 0)
                            {
                                var request = WebRequest.Create(usrAdr.PhotoPreviews.Photo200);

                                using (var response = request.GetResponse())
                                using (var stream = response.GetResponseStream())
                                {
                                    bitmapPicture = Bitmap.FromStream(stream);
                                    bRead = true;
                                    button.BackgroundImage = bitmapPicture;
                                    if (bPerson)
                                    {
                                        personPicture = bitmapPicture;
                                        //btnB4.BackgroundImage = bitmapPicture;
                                    }
                                    else
                                    {
                                        contactPicture = bitmapPicture;
                                        pathContacterImageFromGroup();
                                    }
                                }
                            }

                        if (!bRead)
                        {
                            usrAdr = api.Users.Get(userid, ProfileFields.Photo100);
                            if (usrAdr.PhotoPreviews.Photo100 != null)
                                if (usrAdr.PhotoPreviews.Photo100.Length > 0)
                                {
                                    var request = WebRequest.Create(usrAdr.PhotoPreviews.Photo100);

                                    using (var response = request.GetResponse())
                                    using (var stream = response.GetResponseStream())
                                    {
                                        bitmapPicture = Bitmap.FromStream(stream);
                                        bRead = true;
                                        button.BackgroundImage = bitmapPicture;
                                        if (bPerson)
                                        {
                                            personPicture = bitmapPicture;
                                            //btnB4.BackgroundImage = bitmapPicture;
                                        }
                                        else
                                        {
                                            contactPicture = bitmapPicture;
                                            pathContacterImageFromGroup();
                                        }
                                    }
                                }
                        }

                        if (!bRead)
                        {
                            if (bPerson)
                            {
                                personPicture = null;
                                //btnB4.BackgroundImage = Nilsa.Properties.Resources.nilsa_pers;
                            }
                            else
                                contactPicture = null;
                        }
                    }
                    catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (System.Net.WebException)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (VkNet.Exception.VkApiException vkapexeption)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("SetUserPictureFromID", userid.ToString(), e);
                    }
                    finally { }
                    */
                }
                else if (SocialNetwork == 1)
                {
                    if (File.Exists(Path.Combine(sNILSAImagesPath, userid.ToString() + ".png")))
                        bitmapPicture = Image.FromFile(Path.Combine(sNILSAImagesPath, userid.ToString() + ".png"));
                    button.BackgroundImage = bitmapPicture;
                    if (bPerson)
                    {
                        personPicture = bitmapPicture;
                        //btnB4.BackgroundImage = Nilsa.Properties.Resources.nilsa_pers;
                    }
                    else
                        contactPicture = bitmapPicture;
                }
            }
            /*
            else
            {
                if (userid < -1 && SocialNetwork == 0)
                {
                    try
                    {
                        VkNet.Model.Group usrAdr = api.Groups.GetById(-userid);
                        bool bRead = false;
                        if (usrAdr.PhotoPreviews.Photo200 != null)
                            if (usrAdr.PhotoPreviews.Photo200.Length > 0)
                            {
                                var request = WebRequest.Create(usrAdr.PhotoPreviews.Photo200);

                                using (var response = request.GetResponse())
                                using (var stream = response.GetResponseStream())
                                {
                                    bitmapPicture = Bitmap.FromStream(stream);
                                    bRead = true;
                                    button.BackgroundImage = bitmapPicture;
                                    if (bPerson)
                                    {
                                        personPicture = bitmapPicture;
                                        //btnB4.BackgroundImage = bitmapPicture;
                                    }
                                    else
                                        contactPicture = bitmapPicture;
                                }
                            }

                        if (!bRead)
                        {
                            //usrAdr = api.Users.Get(userid, ProfileFields.Photo100);
                            if (usrAdr.PhotoPreviews.Photo100 != null)
                                if (usrAdr.PhotoPreviews.Photo100.Length > 0)
                                {
                                    var request = WebRequest.Create(usrAdr.PhotoPreviews.Photo100);

                                    using (var response = request.GetResponse())
                                    using (var stream = response.GetResponseStream())
                                    {
                                        bitmapPicture = Bitmap.FromStream(stream);
                                        bRead = true;
                                        button.BackgroundImage = bitmapPicture;
                                        if (bPerson)
                                        {
                                            personPicture = bitmapPicture;
                                            //btnB4.BackgroundImage = bitmapPicture;
                                        }
                                        else
                                            contactPicture = bitmapPicture;
                                    }
                                }
                        }

                        if (!bRead)
                        {
                            if (bPerson)
                            {
                                personPicture = null;
                                //btnB4.BackgroundImage = Nilsa.Properties.Resources.nilsa_pers;
                            }
                            else
                                contactPicture = null;
                        }
                    }
                    catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (System.Net.WebException)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (VkNet.Exception.VkApiException vkapexeption)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("SetUserPictureFromID", userid.ToString(), e);
                    }
                    finally { }
                }

            }
            */
        }

        private void pathContacterImageFromGroup()
        {
            /*
            if (iGroupAnswerID > 0 && SocialNetwork == 0)
            {
                try
                {
                    VkNet.Model.Group usrAdr = api.Groups.GetById(iGroupAnswerID);
                    bool bRead = false;
                    if (usrAdr.PhotoPreviews.Photo200 != null)
                        if (usrAdr.PhotoPreviews.Photo200.Length > 0)
                        {
                            var request = WebRequest.Create(usrAdr.PhotoPreviews.Photo200);

                            using (var response = request.GetResponse())
                            using (var stream = response.GetResponseStream())
                            {
                                Image _bitmapPicture = Bitmap.FromStream(stream);
                                bRead = true;
                                Graphics g = Graphics.FromImage(_bitmapPicture);
                                g.DrawImage(buttonEditContHarValues.BackgroundImage, new Rectangle(_bitmapPicture.Width / 2, _bitmapPicture.Height / 2, _bitmapPicture.Width / 2, _bitmapPicture.Height / 2), 0, 0, buttonEditContHarValues.BackgroundImage.Width, buttonEditContHarValues.BackgroundImage.Height, GraphicsUnit.Pixel);
                                g.Dispose();
                                contactPicture = _bitmapPicture;
                                buttonEditContHarValues.BackgroundImage = _bitmapPicture;
                            }
                        }

                    if (!bRead)
                    {
                        //usrAdr = api.Users.Get(userid, ProfileFields.Photo100);
                        if (usrAdr.PhotoPreviews.Photo100 != null)
                            if (usrAdr.PhotoPreviews.Photo100.Length > 0)
                            {
                                var request = WebRequest.Create(usrAdr.PhotoPreviews.Photo100);

                                using (var response = request.GetResponse())
                                using (var stream = response.GetResponseStream())
                                {
                                    Image _bitmapPicture = Bitmap.FromStream(stream);
                                    bRead = true;
                                    Graphics g = Graphics.FromImage(_bitmapPicture);
                                    g.DrawImage(buttonEditContHarValues.BackgroundImage, new Rectangle(_bitmapPicture.Width / 2, _bitmapPicture.Height / 2, _bitmapPicture.Width / 2, _bitmapPicture.Height / 2), 0, 0, buttonEditContHarValues.BackgroundImage.Width, buttonEditContHarValues.BackgroundImage.Height, GraphicsUnit.Pixel);
                                    g.Dispose();
                                    contactPicture = _bitmapPicture;
                                    buttonEditContHarValues.BackgroundImage = _bitmapPicture;
                                }
                            }
                    }

                }
                catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (System.Net.WebException)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (VkNet.Exception.VkApiException vkapexeption)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("pathContacterImageFromGroup", iGroupAnswerID.ToString(), e);
                }
                finally { }
            }
            */
        }

        private FormWebBrowser.Persone loadPersoneAttributesFriends(long id)
        {
            stopTimers();

            ShowBrowserCommand();

            fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.GetPersoneFriendsCount, id);
            fwbVKontakte.WaitResult();
            FormWebBrowser.Persone usrAdr = fwbVKontakte.personeAtrributesFriends;

            HideBrowserCommand();

            startTimers();
            return usrAdr;
        }

        //private FormWebBrowser.Persone loadPersoneAttributes(long id)
        //{
        //    stopTimers();

        //    ShowBrowserCommand();

        //    fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.GetPersoneAttributes, id);
        //    fwbVKontakte.WaitResult();

        //    var stringJSON = vkInterface.Setup(userLogin, userPassword, WebBrowserCommand.GetPersoneAttributes, NilsaOperatingMode.SeleniumMode, id);
        //    //раскоментить, когда научим интерфейс отвечать
        //    //var result = JsonConvert.DeserializeObject<ResponseFromInterface>(stringJSON);
        //    //personeVkAtt = new PersoneVkAttributes(result.PersonId, result.FirstName, result.LastName,
        //    //    result.Relation, result.BirthDate, result.City, result.Country, result.CountersFriends, result.Online, result.LastSeen);

        //    FormWebBrowser.Persone usrAdr = fwbVKontakte.personeAtrributes;

        //    HideBrowserCommand();

        //    startTimers();
        //    return fwbVKontakte.personeAtrributes;
        //}

        private void UpdatePersoneParametersValues_Friends()
        {
            if (iPersUserID >= 0 && lstPersHarValues.Count > 0)
            {
                if (SocialNetwork == 0)
                {
                    if (fwbVKontakte != null)
                    {
                        if (fwbVKontakteFirstShow)
                        {
                            FormWebBrowser.Persone usrAdr = loadPersoneAttributesFriends(iPersUserID);
                            if (usrAdr != null)
                            {
                                lstPersHarValues[PERSONE_COLUMN_FRIENDS] = (PERSONE_COLUMN_FRIENDS + 1).ToString() + "|" + usrAdr.CountersFriends; 
                                SavePersoneParamersValues();

                                lblPersHarValues[PERSONE_COLUMN_FRIENDS].Text = lstPersHarValues[PERSONE_COLUMN_FRIENDS].Substring(lstPersHarValues[PERSONE_COLUMN_FRIENDS].IndexOf("|") + 1);
                                toolTipMessage.SetToolTip(lblPersHarValues[PERSONE_COLUMN_FRIENDS], sPersHar[PERSONE_COLUMN_FRIENDS, 1] + ": " + lblPersHarValues[PERSONE_COLUMN_FRIENDS].Text);
                            }
                        }
                    }
                    /*
                    try
                    {
                        VkNet.Model.User usrAdr = api.Users.Get(iPersUserID, ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Counters);
                        //contName = usrAdr.FirstName + " " + usrAdr.LastName;

                        lstPersHarValues[PERSONE_COLUMN_FRIENDS] = (PERSONE_COLUMN_FRIENDS + 1).ToString() + "|" + (usrAdr.Counters != null ? usrAdr.Counters.Friends.Value.ToString() : "");
                        SavePersoneParamersValues();

                        lblPersHarValues[PERSONE_COLUMN_FRIENDS].Text = lstPersHarValues[PERSONE_COLUMN_FRIENDS].Substring(lstPersHarValues[PERSONE_COLUMN_FRIENDS].IndexOf("|") + 1);
                        toolTipMessage.SetToolTip(lblPersHarValues[PERSONE_COLUMN_FRIENDS], sPersHar[PERSONE_COLUMN_FRIENDS, 1] + ": " + lblPersHarValues[PERSONE_COLUMN_FRIENDS].Text);
                    }
                    catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (System.Net.WebException)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (VkNet.Exception.VkApiException vkapexeption)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (Exception exp)
                    {
                        ExceptionToLogList("UpdatePersoneParametersValues_Friends", "UNKNOWN", exp);
                    }
                    */
                }
            }
        }

        private void SetContactParametersValues()
        {
            if (iContUserID >= 0)
            {
                if (SocialNetwork == 0)
                {
                    FormWebBrowser.Persone usrAdr = null;
                    if (fwbVKontakte != null)
                    {
                        if (fwbVKontakteFirstShow)
                        {
                            if (fwbVKontakte.contactAtrributes != null)
                                usrAdr = fwbVKontakte.contactAtrributes;
                        }
                    }

                    if (usrAdr != null)
                        if (usrAdr.id != iContUserID)
                            usrAdr = null;
                    if (usrAdr == null)
                        usrAdr = loadContactAttributes(iContUserID);

                    contName = usrAdr.FirstName + " " + usrAdr.LastName;
                    if (lstContHarValues.Count > 0)
                    {
                        lstContHarValues[CONTACT_COLUMN_FRIENDS] = (CONTACT_COLUMN_FRIENDS + 1).ToString() + "|" + usrAdr.CountersFriends;
                        lstContHarValues[CONTACT_COLUMN_STATUS] = (CONTACT_COLUMN_STATUS + 1).ToString() + "|" + usrAdr.Online;
                        if (lstContHarValues[CONTACT_COLUMN_SEX].IndexOf("|") > 0)
                        {
                            String sexValue = lstContHarValues[CONTACT_COLUMN_SEX].Substring(lstContHarValues[CONTACT_COLUMN_SEX].IndexOf("|") + 1);
                            if (sexValue.Length == 0 || sexValue.ToLower().Equals("не указан"))
                            {
                                lstContHarValues[CONTACT_COLUMN_SEX] = lstContHarValues[CONTACT_COLUMN_SEX].Substring(0, lstContHarValues[CONTACT_COLUMN_SEX].IndexOf("|") + 1)+ usrAdr.Sex;
                            }
                        }
                        labelContMsgCount.Text = usrAdr.LastSeen;
                        SaveContactParamersValues();
                    }
                    //if (ContactsList_GetUserIdx(iContUserID.ToString()) < 0)
                    //    ContactsList_AddUser(iContUserID.ToString(), contName);

                    contNameFamily = usrAdr.LastName;
                    contNameName = usrAdr.FirstName;
                    labelCont1Family.Text = contNameFamily;
                    labelCont1Name.Text = contNameName;
                    labelCont1FIO.Text = contName;
                    toolTipMessage.SetToolTip(labelCont1FIO, labelCont1FIO.Text);


                    /*
                    try
                    {
                        VkNet.Model.User usrAdr = api.Users.Get(iContUserID, ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Online | ProfileFields.Counters | ProfileFields.LastSeen);
                        contName = usrAdr.FirstName + " " + usrAdr.LastName;
                        if (lstContHarValues.Count > 0)
                        {
                            lstContHarValues[CONTACT_COLUMN_FRIENDS] = (CONTACT_COLUMN_FRIENDS + 1).ToString() + "|" + (usrAdr.Counters != null ? usrAdr.Counters.Friends.Value.ToString() : "");
                            lstContHarValues[CONTACT_COLUMN_STATUS] = (CONTACT_COLUMN_STATUS + 1).ToString() + "|" + (usrAdr.Online != null ? (usrAdr.Online.Value ? "ON line" : "OFF line") : "Unknown");
                            DateTime? dtLastSeen = usrAdr.LastSeen;
                            labelContMsgCount.Text = dtLastSeen.HasValue ? dtLastSeen.Value.ToString(dtLastSeen.Value.Date == DateTime.Now.Date ? "HH:mm" : "dd.MM.yy HH:mm") : "";
                            SaveContactParamersValues();
                        }
                        //if (ContactsList_GetUserIdx(iContUserID.ToString()) < 0)
                        //    ContactsList_AddUser(iContUserID.ToString(), contName);

                        contNameFamily = usrAdr.LastName;
                        contNameName = usrAdr.FirstName;
                        labelCont1Family.Text = contNameFamily;
                        labelCont1Name.Text = contNameName;
                        labelCont1FIO.Text = contName;
                        toolTipMessage.SetToolTip(labelCont1FIO, labelCont1FIO.Text);

                        //buttonEditOutEqMsgHar.Enabled = true;
                    }
                    catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (System.Net.WebException)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (VkNet.Exception.VkApiException vkapexeption)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("SetContactParametersValues", iContUserID.ToString(), e);
                    }
                    finally { }
                    */
                }
                else if (SocialNetwork == 1)
                {
                    contName = contNameFamily = contNameName = "";
                    labelCont1Family.Text = "";
                    labelCont1Name.Text = "";
                    labelCont1FIO.Text = "";
                    toolTipMessage.SetToolTip(labelCont1FIO, labelCont1FIO.Text);
                    String sur = NILSA_getUserRecord(iContUserID.ToString());
                    if (sur != null)
                    {
                        contName = NILSA_GetFieldFromStringRec(sur, 1);
                        if (contName.IndexOf(" ") > 0)
                        {
                            contNameName = contName.Substring(0, contName.IndexOf(" ")); ;
                            contNameFamily = contName.Substring(contName.IndexOf(" ") + 1);
                        }
                        else
                        {
                            contNameName = contName;
                            contNameFamily = "";
                        }
                        labelCont1Family.Text = contNameFamily;
                        labelCont1Name.Text = contNameName;
                        labelCont1FIO.Text = contName;
                        toolTipMessage.SetToolTip(labelCont1FIO, labelCont1FIO.Text);
                        //buttonEditOutEqMsgHar.Enabled = true;
                    }
                }
            }
            /*
            else
            {
                if (iContUserID < -1)
                {
                    try
                    {
                        VkNet.Model.Group usrAdr = api.Groups.GetById(-iContUserID);
                        contName = usrAdr.Name;
                        contNameFamily = "";
                        contNameName = usrAdr.Name;
                        labelCont1Family.Text = contNameFamily;
                        labelCont1Name.Text = contNameName;
                        labelCont1FIO.Text = contName;
                        toolTipMessage.SetToolTip(labelCont1FIO, labelCont1FIO.Text);

                        //buttonEditOutEqMsgHar.Enabled = true;
                    }
                    catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (System.Net.WebException)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (VkNet.Exception.VkApiException vkapexeption)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("SetContactParametersValues", iContUserID.ToString(), e);
                    }
                    finally { }
                }
                else
                {
                    contName = contNameFamily = contNameName = "";
                    labelCont1Family.Text = "";
                    labelCont1Name.Text = "";
                    labelCont1FIO.Text = "";
                    toolTipMessage.SetToolTip(labelCont1FIO, labelCont1FIO.Text);

                    //cbCont1.SelectedIndex = -1;
                }
            }
            */
            SetUserPictureFromID(iContUserID, buttonEditContHarValues, false);
            LoadContactParametersDescription();
        }

        private void SavePersoneParametersDescription()
        {
            List<String> lstPersHar = new List<String>();
            for (int i = 0; i < iPersHarCount; i++)
            {
                String value = "";
                for (int j = 0; j < iPersHarAttrCount; j++)
                    value = value + sPersHar[i, j] + "|";
                lstPersHar.Add(value);
            }

            FileWriteAllLines(Path.Combine(sDataPath, "_pershar.txt"), lstPersHar, Encoding.UTF8);
        }

        private void SaveInitDialogFlag()
        {
            SaveInitDialogFlag(Convert.ToString(iContUserID));
        }

        private void SaveInitDialogFlag(String sContID)
        {
            List<String> lstList = new List<String>();
            lstList.Add("1");
            FileWriteAllLines(Path.Combine(sDataPath, "_flag_init_dialog_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sContID + ".txt"), lstList, Encoding.UTF8);
        }

        private bool CheckInitDialogFlag()
        {
            if (File.Exists(Path.Combine(sDataPath, "_flag_init_dialog_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(iContUserID) + ".txt")))
            {
                File.Delete(Path.Combine(sDataPath, "_flag_init_dialog_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(iContUserID) + ".txt"));
                return true;
            }
            return false;
        }

        private void SaveContactParametersDescription()
        {
            List<String> lstContHar = new List<String>();
            for (int i = 0; i < iContHarCount; i++)
            {
                String value = "";
                for (int j = 0; j < iContHarAttrCount; j++)
                    value = value + sContHar[i, j] + "|";
                lstContHar.Add(value);
            }
            FileWriteAllLines(Path.Combine(sDataPath, "_conthar.txt"), lstContHar, Encoding.UTF8);
        }

        private void SaveMessageParametersDescription()
        {
            List<String> lstMsgHar = new List<String>();
            for (int i = 0; i < iMsgHarCount; i++)
            {
                String value = "";
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    value = value + sMsgHar[i, j] + "|";
                lstMsgHar.Add(value);
            }
            FileWriteAllLines(Path.Combine(sDataPath, "_msghar.txt"), lstMsgHar, Encoding.UTF8);
        }

        private void LoadPersoneParametersValues()
        {
            long lPersID = iPersUserID;
            lstPersHarValues = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + Convert.ToString(lPersID) + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + Convert.ToString(lPersID) + ".txt"));
                    lstPersHarValues = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstPersHarValues = new List<String>();
                    for (int i = 0; i < iPersHarCount; i++)
                    {
                        lstPersHarValues.Add((i + 1).ToString() + "|");
                    }
                }
            }
        }

        private void SavePersoneParamersValues()
        {
            if (iPersUserID < 0)
                return;

            if (lstPersHarValues.Count > 0)
                FileWriteAllLines(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + Convert.ToString(iPersUserID) + ".txt"), lstPersHarValues, Encoding.UTF8);
        }

        private void LoadContactParamersValues()
        {
            long lContID = iContUserID;
            lstContHarValues = new List<String>();
            if (lContID >= 0)
            {
                if (File.Exists(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(lContID) + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(lContID) + ".txt"));
                        lstContHarValues = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        lstContHarValues = new List<String>();
                        for (int i = 0; i < iContHarCount; i++)
                            lstContHarValues.Add((i + 1).ToString() + "|");
                    }
                }
            }
            else
            {
                if (iContUserID < -1)
                    if (File.Exists(Path.Combine(sDataPath, "grp_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(-lContID) + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "grp_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(-lContID) + ".txt"));
                            lstContHarValues = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                            lstContHarValues = new List<String>();
                            for (int i = 0; i < iContHarCount; i++)
                                lstContHarValues.Add((i + 1).ToString() + "|");
                        }
                    }
            }
        }

        private void SaveContactParamersValues()
        {
            if (iContUserID == -1)
                return;

            if (iContUserID >= 0)
            {
                if (lstContHarValues.Count > 0)
                    FileWriteAllLines(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(iContUserID) + ".txt"), lstContHarValues, Encoding.UTF8);
            }
            else
            {
                if (iContUserID < -1 && lstContHarValues.Count > 0)
                    FileWriteAllLines(Path.Combine(sDataPath, "grp_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(-iContUserID) + ".txt"), lstContHarValues, Encoding.UTF8);
            }
        }

        //---
        private List<String> LoadContactHarValues(int harIdx)
        {
            long lContID = iContUserID;
            List<String> lstLS = new List<String>();

            if (lContID >= 0)
            {
                if (File.Exists(Path.Combine(sDataPath, "conthar_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(lContID) + "_" + Convert.ToString(harIdx) + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "conthar_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(lContID) + "_" + Convert.ToString(harIdx) + ".txt"));
                        lstLS = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        lstLS = new List<String>();
                    }
                }
            }
            else
            {
                if (iContUserID < -1)
                    if (File.Exists(Path.Combine(sDataPath, "grphar_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(-lContID) + "_" + Convert.ToString(harIdx) + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "grphar_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(-lContID) + "_" + Convert.ToString(harIdx) + ".txt"));
                            lstLS = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                            lstLS = new List<String>();
                        }
                    }
            }
            return lstLS;
        }

        private String getContactHarValue(int harIdx)
        {
            String retVal = "";
            List<String> lstLS = LoadContactHarValues(harIdx);

            if (lstLS.Count > 0)
            {
                retVal = lstLS[0];
                lstLS.RemoveAt(0);
                SaveContactHarValues(harIdx, lstLS);
            }

            return retVal;
        }

        private void putContactHarValue(int harIdx, String harValue)
        {
            List<String> lstLS = LoadContactHarValues(harIdx);
            lstLS.Insert(0, harValue.Length == 0 ? "#clear#" : harValue);
            SaveContactHarValues(harIdx, lstLS);
        }

        private void SaveContactHarValues(int harIdx, List<String> lstLS)
        {
            if (iContUserID == -1)
                return;

            if (iContUserID >= 0)
            {
                if (lstLS.Count > 0)
                    FileWriteAllLines(Path.Combine(sDataPath, "conthar_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(iContUserID) + "_" + Convert.ToString(harIdx) + ".txt"), lstLS, Encoding.UTF8);
                else
                    File.Delete(Path.Combine(sDataPath, "conthar_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(iContUserID) + "_" + Convert.ToString(harIdx) + ".txt"));
            }
            else
            {
                if (iContUserID < -1 && lstLS.Count > 0)
                    FileWriteAllLines(Path.Combine(sDataPath, "grphar_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(-iContUserID) + "_" + Convert.ToString(harIdx) + ".txt"), lstLS, Encoding.UTF8);
                else
                    File.Delete(Path.Combine(sDataPath, "grphar_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(-iContUserID) + "_" + Convert.ToString(harIdx) + ".txt"));
            }
        }


        //---
        public String GetPersoneParametersValue(String sPersHarID)
        {
            for (int i = 0; i < lstPersHarValues.Count; i++)
            {
                if (lstPersHarValues[i].IndexOf("|") >= 0)
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
                if (lstContHarValues[i].IndexOf("|") >= 0)
                    if (lstContHarValues[i].Substring(0, lstContHarValues[i].IndexOf("|")) == sContHarID)
                    {
                        return lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1);
                    }
            }
            return "";
        }

        private void LoadPersoneParametersDescription()
        {
            List<String> lstPersHar = new List<String>();
            sPersHar = new String[iPersHarCount, iPersHarAttrCount];
            if (File.Exists(Path.Combine(sDataPath, "_pershar.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_pershar.txt"));
                    lstPersHar = new List<String>(srcFile);
                    int iSFSC = lstPersHar.Count;
                    if (lstPersHar.Count == iPersHarCount)
                    {
                        for (int i = 0; i < iPersHarCount; i++)
                        {
                            String value = lstPersHar[i];
                            sPersHar[i, 0] = value.Substring(0, value.IndexOf("|"));
                            value = value.Substring(value.IndexOf("|") + 1);
                            sPersHar[i, 1] = value.Substring(0, value.IndexOf("|"));
                            value = value.Substring(value.IndexOf("|") + 1);
                            sPersHar[i, 2] = value.Substring(0, value.IndexOf("|"));
                            value = value.Substring(value.IndexOf("|") + 1);
                            sPersHar[i, 3] = value.Substring(0, value.IndexOf("|"));
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    for (int i = 0; i < iPersHarCount; i++)
                    {
                        sPersHar[i, 0] = Convert.ToString(i + 1); // id
                        sPersHar[i, 1] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_7", this.Name, "Характеристика") + " " + Convert.ToString(i + 1); // name
                        sPersHar[i, 2] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_8", this.Name, "Строка"); // type
                        sPersHar[i, 3] = ""; // default value
                    }
                }
            }
            else
            {
                for (int i = 0; i < iPersHarCount; i++)
                {
                    sPersHar[i, 0] = Convert.ToString(i + 1); // id
                    sPersHar[i, 1] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_7", this.Name, "Характеристика") + " " + Convert.ToString(i + 1); // name
                    sPersHar[i, 2] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_8", this.Name, "Строка"); // type
                    sPersHar[i, 3] = ""; // default value
                }
            }

            for (int i = 0; i < iPersHarCount; i++)
            {

                string _picfile = Path.Combine(Application.StartupPath, "Images\\_pers_har_" + i.ToString() + ".png");
                if (File.Exists(_picfile))
                {
                    FileStream stream = new FileStream(_picfile, FileMode.Open, FileAccess.Read);
                    lblPersHarValues[i].Image = Image.FromStream(stream);
                    stream.Close();
                }
                else
                    lblPersHarValues[i].Image = Nilsa.Properties.Resources.labelbg;
                lblPersHarValues[i].ImageAlign = ContentAlignment.MiddleLeft;


                //lblPersHarNames[i].Text = "";
                lblPersHarValues[i].Text = GetPersoneParametersValue(sPersHar[i, 0]);
                if (lblPersHarValues[i].Text.Length == 0)
                    lblPersHarValues[i].Text = sPersHar[i, 3];

                //toolTipMessage.SetToolTip(lblPersHarNames[i], sPersHar[i, 1] + ": " + lblPersHarValues[i].Text);
                toolTipMessage.SetToolTip(lblPersHarValues[i], sPersHar[i, 1] + ": " + lblPersHarValues[i].Text);
            }
        }

        private void SetLabelEmptyValue(Label lbl)
        {
            lbl.Text = "";
        }

        private void SetLabelPersValue(Label lbl, int id)
        {
            lbl.Text = GetPersoneParametersValue(sPersHar[id, 0]);
            if (lbl.Text.Length == 0)
                lbl.Text = sPersHar[id, 3];
        }

        private void SaveGroupParametersDescription()
        {
            List<String> lstContHar = new List<String>();
            for (int i = 0; i < iGroupHarCount; i++)
            {
                String value = "";
                for (int j = 0; j < iGroupHarAttrCount; j++)
                    value = value + sGroupHar[i, j] + "|";
                lstContHar.Add(value);
            }
            FileWriteAllLines(Path.Combine(sDataPath, "_grouphar.txt"), lstContHar, Encoding.UTF8);
        }

        private void LoadGroupParametersDescription()
        {
            //if (iContactsGroupsMode == 0)
            //    return;

            List<String> lstContHar = new List<String>();
            sGroupHar = new String[iGroupHarCount, iGroupHarAttrCount];

            if (File.Exists(Path.Combine(sDataPath, "_grouphar.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_grouphar.txt"));
                    lstContHar = new List<String>(srcFile);
                    int iSFSC = lstContHar.Count;
                    if (lstContHar.Count == iGroupHarCount)
                    {
                        for (int i = 0; i < iGroupHarCount; i++)
                        {
                            String value = lstContHar[i];
                            sGroupHar[i, 0] = value.Substring(0, value.IndexOf("|"));
                            value = value.Substring(value.IndexOf("|") + 1);
                            sGroupHar[i, 1] = value.Substring(0, value.IndexOf("|"));
                            value = value.Substring(value.IndexOf("|") + 1);
                            sGroupHar[i, 2] = value.Substring(0, value.IndexOf("|"));
                            value = value.Substring(value.IndexOf("|") + 1);
                            sGroupHar[i, 3] = value.Substring(0, value.IndexOf("|"));
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    for (int i = 0; i < iGroupHarCount; i++)
                    {
                        sGroupHar[i, 0] = Convert.ToString(i + 1); // id
                        sGroupHar[i, 1] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_7", this.Name, "Характеристика") + " " + Convert.ToString(i + 1); // name
                        sGroupHar[i, 2] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_8", this.Name, "Строка"); // type
                        sGroupHar[i, 3] = ""; // default value
                    }
                }
            }
            else
            {
                for (int i = 0; i < iGroupHarCount; i++)
                {
                    sGroupHar[i, 0] = Convert.ToString(i + 1); // id
                    sGroupHar[i, 1] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_7", this.Name, "Характеристика") + " " + Convert.ToString(i + 1); // name
                    sGroupHar[i, 2] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_8", this.Name, "Строка"); // type
                    sGroupHar[i, 3] = ""; // default value
                }
            }

            if (iContUserID < -1)
            {
                for (int i = 0; i < iGroupHarCount; i++)
                {

                    string _picfile = Path.Combine(Application.StartupPath, "Images\\_grp_har_" + i.ToString() + ".png");
                    if (File.Exists(_picfile))
                    {
                        FileStream stream = new FileStream(_picfile, FileMode.Open, FileAccess.Read);
                        lblContHarValues[i].Image = Image.FromStream(stream);
                        stream.Close();
                    }
                    else
                        lblContHarValues[i].Image = Nilsa.Properties.Resources.labelbg;
                    lblContHarValues[i].ImageAlign = ContentAlignment.MiddleLeft;


                    //lblContHarNames[i].Text = "";

                    lblContHarValues[i].Text = GetContactParametersValue(sGroupHar[i, 0]);
                    if (lblContHarValues[i].Text.Length == 0)
                        lblContHarValues[i].Text = sGroupHar[i, 3];
                    //toolTipMessage.SetToolTip(lblContHarNames[i], sContHar[i, 1] + ": " + lblContHarValues[i].Text);
                    toolTipMessage.SetToolTip(lblContHarValues[i], sGroupHar[i, 1] + ": " + lblContHarValues[i].Text);
                }
            }
        }


        private void LoadContactParametersDescription()
        {
            if (iContUserID < -1)
            {
                LoadGroupParametersDescription();
            }
            else
            {
                List<String> lstContHar = new List<String>();
                sContHar = new String[iContHarCount, iContHarAttrCount];

                if (File.Exists(Path.Combine(sDataPath, "_conthar.txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_conthar.txt"));
                        lstContHar = new List<String>(srcFile);
                        int iSFSC = lstContHar.Count;
                        if (lstContHar.Count == iContHarCount)
                        {
                            for (int i = 0; i < iContHarCount; i++)
                            {
                                String value = lstContHar[i];
                                sContHar[i, 0] = value.Substring(0, value.IndexOf("|"));
                                value = value.Substring(value.IndexOf("|") + 1);
                                sContHar[i, 1] = value.Substring(0, value.IndexOf("|"));
                                value = value.Substring(value.IndexOf("|") + 1);
                                sContHar[i, 2] = value.Substring(0, value.IndexOf("|"));
                                value = value.Substring(value.IndexOf("|") + 1);
                                sContHar[i, 3] = value.Substring(0, value.IndexOf("|"));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        for (int i = 0; i < iContHarCount; i++)
                        {
                            sContHar[i, 0] = Convert.ToString(i + 1); // id
                            sContHar[i, 1] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_7", this.Name, "Характеристика") + " " + Convert.ToString(i + 1); // name
                            sContHar[i, 2] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_8", this.Name, "Строка"); // type
                            sContHar[i, 3] = ""; // default value
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        sContHar[i, 0] = Convert.ToString(i + 1); // id
                        sContHar[i, 1] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_7", this.Name, "Характеристика") + " " + Convert.ToString(i + 1); // name
                        sContHar[i, 2] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_8", this.Name, "Строка"); // type
                        sContHar[i, 3] = ""; // default value
                    }
                }
                for (int i = 0; i < iContHarCount; i++)
                {

                    string _picfile = Path.Combine(Application.StartupPath, "Images\\_cont_har_" + i.ToString() + ".png");
                    if (File.Exists(_picfile))
                    {
                        FileStream stream = new FileStream(_picfile, FileMode.Open, FileAccess.Read);
                        lblContHarValues[i].Image = Image.FromStream(stream);
                        stream.Close();
                    }
                    else
                        lblContHarValues[i].Image = Nilsa.Properties.Resources.labelbg;
                    lblContHarValues[i].ImageAlign = ContentAlignment.MiddleLeft;


                    //lblContHarNames[i].Text = "";

                    lblContHarValues[i].Text = GetContactParametersValue(sContHar[i, 0]);
                    if (lblContHarValues[i].Text.Length == 0)
                        lblContHarValues[i].Text = sContHar[i, 3];
                    //toolTipMessage.SetToolTip(lblContHarNames[i], sContHar[i, 1] + ": " + lblContHarValues[i].Text);
                    toolTipMessage.SetToolTip(lblContHarValues[i], sContHar[i, 1] + ": " + lblContHarValues[i].Text);
                }
            }
        }

        private void LoadMessageParametersDescription()
        {
            List<String> lstMsgHar = new List<String>();
            sMsgHar = new String[iMsgHarCount, iMsgHarAttrCount];
            iMsgHarKoef = new int[iMsgHarCount];
            iCompareVectorsKoef = new int[iMsgHarCount];
            iCompareVectorsKoefOut = new int[iMsgHarCount];
            if (File.Exists(Path.Combine(sDataPath, "_msghar.txt")))
            {
                try
                {
                    bool bNeedSave = false;
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_msghar.txt"));
                    lstMsgHar = new List<String>(srcFile);
                    int iSFSC = lstMsgHar.Count;
                    if (lstMsgHar.Count == iMsgHarCount)
                    {
                        for (int i = 0; i < iMsgHarCount; i++)
                        {
                            String value = lstMsgHar[i];
                            sMsgHar[i, 0] = value.Substring(0, value.IndexOf("|"));
                            value = value.Substring(value.IndexOf("|") + 1);
                            sMsgHar[i, 1] = value.Substring(0, value.IndexOf("|"));
                            value = value.Substring(value.IndexOf("|") + 1);
                            sMsgHar[i, 2] = value.Substring(0, value.IndexOf("|"));
                            value = value.Substring(value.IndexOf("|") + 1);
                            sMsgHar[i, 3] = value.Substring(0, value.IndexOf("|"));
                            if (i == 14)
                            {
                                if (!sMsgHar[i, 1].Equals(NilsaUtils.Dictonary_GetText(userInterface, "textValues_9", this.Name, "Игнорировать")))
                                {
                                    sMsgHar[i, 1] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_9", this.Name, "Игнорировать");
                                    bNeedSave = true;
                                }
                            }
                        }
                    }
                    if (bNeedSave)
                        SaveMessageParametersDescription();
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    for (int i = 0; i < iMsgHarCount; i++)
                    {
                        sMsgHar[i, 0] = Convert.ToString(i + 1); // id
                        sMsgHar[i, 1] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_7", this.Name, "Характеристика") + " " + Convert.ToString(i + 1); // name
                        sMsgHar[i, 2] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_8", this.Name, "Строка"); // type
                        sMsgHar[i, 3] = ""; // default value
                    }
                }
            }
            else
            {
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    sMsgHar[i, 0] = Convert.ToString(i + 1); // id
                    sMsgHar[i, 1] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_7", this.Name, "Характеристика") + " " + Convert.ToString(i + 1); // name
                    sMsgHar[i, 2] = NilsaUtils.Dictonary_GetText(userInterface, "textValues_8", this.Name, "Строка"); // type
                    sMsgHar[i, 3] = ""; // default value
                }
            }

            for (int i = 0; i < iMsgHarCount; i++)
            {
                iCompareVectorsKoefOut[i] = iCompareVectorsKoef[i] = iMsgHarKoef[i] = MsgKoefFromString(sMsgHar[i, 3]);
                sMsgHar[i, 3] = "";
            }
            sMsgHar[0, 3] = strDB0Name;

            for (int i = 0; i < iMsgHarCount; i++)
            {
                lblMsgHarNames[i].Text = sMsgHar[i, 1];
                toolTipMessage.SetToolTip(lblMsgHarNames[i], sMsgHar[i, 1]);
                //lblEQOutHarNames[i].Text = sMsgHar[i, 1];
                //lblEQInHarNames[i].Text = sMsgHar[i, 1];
                toolTipMessage.SetToolTip(lblEQInHarNames[i], "");
                toolTipMessage.SetToolTip(lblEQOutHarNames[i], "");
                lblEQInHarNames[i].OwnerDrawText = MsgKoefToImage(0);
                lblEQOutHarNames[i].OwnerDrawText = MsgKoefToImage(0);

            }


        }

        String externalUserLogin;//"+79310029106";
        String externalUserPassword;//"nilsa1990";
        const String externalUser = "2C/U4YxOKl3RrUUApXxb+Tj5vIG7mykzFr+ewQjwknREHgOzwlewTEq1dxmoUot8";

        //Авторизация персонажа в Autorize
        private bool Autorize(String sUsrSelLogin = "", String sUsrSelPwd = "", String sUsrSelID = "")
        {
            Boolean bShowAutorizeForm = sUsrSelLogin.Length == 0 || sUsrSelPwd.Length == 0;

            userLogin = sUsrSelLogin;
            userPassword = sUsrSelPwd;
            userID = sUsrSelID;

            // !!! Delete this
            if (SocialNetwork == 0)
                api = new VkApi();

            /*
            if (SocialNetwork == 0)
                api = new VkApi();

            long? captcha_sid = null;
            string captcha_key = null;
            for (int iAppID = 0; iAppID < FormMain.userAppIds.Length; iAppID++)
                if (FormMain.userAppId == FormMain.userAppIds[iAppID])
                {
                    FormMain.userAppIdsPos = iAppID;
                    FormMain.userAppIdsPosStart = iAppID;
                    break;
                }
                */
            do
            {
                if (bShowAutorizeForm)
                {
                    HideFormWait();

                    bool prevTopmost = false;
                    if (fwbVKontakte != null)
                    {
                        prevTopmost = fwbVKontakte.TopMost;
                        fwbVKontakte.TopMost = true;
                    }

                    FormEditPersonenDB fe = new FormEditPersonenDB(this);
                    fe.sContHar = new String[iPersHarCount, iPersHarAttrCount + 1];
                    for (int i = 0; i < iPersHarCount; i++)
                    {
                        for (int j = 0; j < iPersHarAttrCount; j++)
                            fe.sContHar[i, j] = sPersHar[i, j];
                        fe.sContHar[i, iPersHarAttrCount] = "";
                    }
                    fe.iContHarCount = iPersHarCount;
                    fe.iContHarAttrCount = iPersHarAttrCount;
                    fe.Setup("Autorization Error select user", -1, false, userLogin, userPassword);

                    if (lstPersoneChange.Count > 0)
                    {
                        clearTrashList(lstPersoneChange);
                        foreach (String str in lstPersoneChange)
                            for (int i = 0; i < fe.lvList.Items.Count; i++)
                            {
                                if (fe.lvList.Items[i].SubItems[1].Text == str)
                                {
                                    fe.lvList.Items[i].Checked = true;
                                    break;
                                }
                            }
                    }

                    fe.ShowDialog();

                    if (fwbVKontakte != null)
                    {
                        fwbVKontakte.TopMost = prevTopmost;
                    }

                    
                    if (fe.bNeedPersoneChange)
                    {
                        timerAnswerWaitingOff();

                        initPersonenLists(fe);

                        if (fe.suSelSocialNetwork != SocialNetwork)
                        {
                            SocialNetwork = fe.suSelSocialNetwork;
                            OnSocialNetworkChanged();
                        }

                        //toolStripMenuItemClearInMsgPullPersonen.Enabled = lstPersoneChange.Count > 0 && SocialNetwork == 0;
                        onAfterPersonenListChanged();

                        userLogin = fe.suSelLogin;
                        userPassword = fe.suSelPwd;
                        userID = fe.suSelID;
                        bShowAutorizeForm = userPassword.Length == 0 || userLogin.Length == 0;
                        ShowFormWait();

                    }
                    else
                    {
                        if (MessageBox.Show("Авторизация не прошла. Пожалуйста, проверьте правильность логина и пароля для авторизации в форме редактирования базы Персонажей, либо выберите там другой Персонаж для входа.\n\nПерейти к форме редактирования базы Персонажей?", NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_15", this.Name, "NILSA - Ошибка авторизации"), MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            SocialNetwork = 1;
                            userLogin = "nilsa";
                            userPassword = "nilsa";
                            userID = "";
                            clearPersonenLists();

                            timerAnswerWaitingOff();
                            //toolStripMenuItemClearInMsgPullPersonen.Enabled = lstPersoneChange.Count > 0 && SocialNetwork == 0;
                            onAfterPersonenListChanged();
                            bShowAutorizeForm = false;
                            OnSocialNetworkChanged();
                        }
                        ShowFormWait();
                    }
                }

                if (!bShowAutorizeForm)
                {
                    Settings settings = Settings.All; // уровень доступа к данным

                    if (SocialNetwork == 0)
                    {
                        try
                        {
                            //if (externalCommandProcess)
                            //    ProcessExternalCommand(externalUserLogin, externalUserPassword);

                            if (fwbVKontakte == null)
                            {
                                fwbVKontakte = new FormWebBrowser(this, true);
                                fwbVKontakte.Init();
                            }


                            if (vkInterface == null)
                            {
                                vkInterface = new VkInterfaceCommands(this);
                            }

                            //stopTimers();

                            long curuid = -1;
                            if (userID != null && userID.Length > 0)
                            {
                                try
                                {
                                    curuid=Convert.ToInt64(userID);
                                }
                                catch
                                {
                                    curuid = -1;
                                }
                            }

                            bool bAutorizeAccept = false;
                            if (curuid != -1 && curuid == fwbVKontakte.loggedPersoneID)
                                bAutorizeAccept = true;

                            if (!bAutorizeAccept)
                            {
                                ShowBrowserCommand();
                                var autorizeResultJSON = vkInterface.Setup(userLogin, userPassword, WebBrowserCommand.LoginPersone, NilsaOperatingMode.SeleniumMode);

                                fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.LoginPersone);
                                if (!fwbVKontakteFirstShow)
                                {
                                    fwbVKontakteFirstShow = true;
                                    fwbVKontakte.Show();
                                }
                                fwbVKontakte.WaitResult();
                                HideBrowserCommand();
                                //vkInterface.Setup(userLogin, userPassword, WebBrowserCommand.LoginPersone, NilsaOperatingMode.SeleniumMode);
                                //var autorizeResultJSON = vkInterface.Setup(userLogin, userPassword, WebBrowserCommand.LoginPersone, NilsaOperatingMode.SeleniumMode);
                                //var autorizeResult = JsonConvert.DeserializeObject<ResponseFromInterface>(autorizeResultJSON);
                            }
                            iPersUserID = fwbVKontakte.loggedPersoneID;

                            //startTimers();

                            if (iPersUserID > 0)
                            {
                                fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.GetPersoneName, iPersUserID);
                                if (!fwbVKontakteFirstShow)
                                {
                                    fwbVKontakteFirstShow = true;
                                    fwbVKontakte.Show();
                                }
                                fwbVKontakte.WaitResult();
                                File.AppendAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), "persname: " + fwbVKontakte.personeAtrributes.FirstName + fwbVKontakte.personeAtrributes.LastName, Encoding.UTF8);



                                userName = PersonenList_GetUserField(iPersUserID.ToString(), 1);
                                //string[] name = userName.Split(' ');
                                //userNameName = name.Length > 0 ? name[0] : "";
                                //userNameFamily = name.Length > 1 ? name[1] : "";

                                //FormWebBrowser.Persone usrAdr = fwbVKontakte.personeAtrributes;

                                userNameName = fwbVKontakte.personeAtrributes.FirstName;
                                userNameFamily = fwbVKontakte.personeAtrributes.LastName;

                                File.AppendAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), "persname by consts: " + userNameName + userNameFamily, Encoding.UTF8);

                                //labelPers1Name.Text = userNameName;
                                //labelPers1Family.Text = userNameFamily;
                                //labelPers1FIO.Text = userNameName + " " + userNameFamily;
                                //toolTipMessage.SetToolTip(labelPers1FIO, labelPers1FIO.Text);



                                //userNameName = fwbVKontakte.personeAtrributes.FirstName;
                                //userNameFamily = fwbVKontakte.personeAtrributes.LastName;

                                //if (userNameName.Equals(""))
                                //{
                                //    //fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.GetPersoneName, iPersUserID);
                                //    //if (!fwbVKontakteFirstShow)
                                //    //{
                                //    //    fwbVKontakteFirstShow = true;
                                //    //    fwbVKontakte.Show();
                                //    //}
                                //    //fwbVKontakte.WaitResult();
                                //    userNameName = fwbVKontakte.personeAtrributes.FirstName;
                                //    userNameFamily = fwbVKontakte.personeAtrributes.LastName;
                                //}

                                setStandardCaption();
                                SetPersoneParametersValues();
                                SetUserPictureFromID(iPersUserID, buttonEditPersHarValues, true);
                                return true;
                            }


                            //api.Authorize(userAppId, userLogin, userPassword, settings, captcha_sid, captcha_key); // авторизуемся

                            /*
                            if (api.UserId.HasValue)
                            {
                                VkNet.Model.User usrAdr = api.Users.Get(api.UserId.Value);
                                userName = usrAdr.FirstName + " " + usrAdr.LastName + (usrAdr.Nickname != null ? " (" + usrAdr.Nickname + ")" : "");
                                userNameFamily = usrAdr.LastName;
                                userNameName = usrAdr.FirstName;
                                iPersUserID = api.UserId.Value;
                                setStandardCaption();
                                SetUserPictureFromID(iPersUserID, buttonEditPersHarValues, true);
                                return true;
                            }
                            */
                        }
                        /*
                        catch (VkNet.Exception.CaptchaNeededException ex)
                        {
                            captcha_sid = ex.Sid;
                            HideFormWait();
                            var form = new FormCaptcha(this, ex.Img, ex.Sid);
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                captcha_key = form.CaptchaKey.Text;
                                ShowFormWait();
                                continue;
                            }
                            ShowFormWait();
                        }
                        catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                        {
                            ReAutorize(userLogin, userPassword);
                            continue;
                        }
                        catch (System.Net.WebException)
                        {
                            ReAutorize(userLogin, userPassword);
                            continue;
                        }
                        catch (VkNet.Exception.VkApiException vkexpapi)
                        {
                            if (vkexpapi.Message != null && vkexpapi.Message.StartsWith("Invalid authorization"))
                            {
                                ExceptionToLogList("Autorize", userLogin + "/" + userPassword, vkexpapi);
                            }
                            else if (vkexpapi.Message != null && vkexpapi.Message.Contains("(401)"))
                            {
                                FormMain.userAppIdsPos++;
                                if (FormMain.userAppIdsPos >= FormMain.userAppIds.Length)
                                    FormMain.userAppIdsPos = 0;

                                if (FormMain.userAppIdsPos != FormMain.userAppIdsPosStart)
                                {
                                    FormMain.userAppId = FormMain.userAppIds[FormMain.userAppIdsPos];
                                    continue;
                                }
                            }
                            else
                            {
                                ReAutorize(userLogin, userPassword);
                                continue;
                            }
                        }
                        */
                        catch (Exception e)
                        {
                            ExceptionToLogList("Autorize", userLogin + "/" + userPassword, e);
                        }
                        finally { }
                    }
                    else if (SocialNetwork == 1)
                    {
                        String sur = NILSA_getUserRecord(userLogin, userPassword);
                        if (sur != null)
                        {
                            iPersUserID = Convert.ToInt64(NILSA_GetFieldFromStringRec(sur, 0));
                            userName = NILSA_GetFieldFromStringRec(sur, 1);
                            if (userName.IndexOf(" ") > 0)
                            {
                                userNameName = userName.Substring(0, userName.IndexOf(" ")); ;
                                userNameFamily = userName.Substring(userName.IndexOf(" ") + 1);
                            }
                            else
                            {
                                userNameName = userName;
                                userNameFamily = "";
                            }
                            //this.Text = userName + " - NILSA, v" + Application.ProductVersion + ((bDebugVersion || showSessionStartTime) ? " (" + sessionStartTime + ")" : "");
                            setStandardCaption();
                            SetUserPictureFromID(iPersUserID, buttonEditPersHarValues, true);
                            return true;
                        }
                    }
                    HideFormWait();
                    if (SocialNetwork == 0)
                    {
                        if (MessageBox.Show("Авторизация не прошла. Пожалуйста, проверьте правильность логина и пароля для Авторизации, либо выберите Другой Персонаж для входа.\n\n Скопировать Контактёров этого Персонажа перед выбором другого Персонажа Мастер Персонажу?", NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_15", this.Name, "NILSA - Ошибка авторизации"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            string sUIDToExportContacts = "";
                            foreach (string persone in lstPersonenList)
                            {
                                if (persone.EndsWith(userLogin + "|" + userPassword))
                                {
                                    sUIDToExportContacts = persone.Substring(0, persone.IndexOf("|"));
                                    break;
                                }
                            }
                            if (sUIDToExportContacts.Length > 0)
                                copyContactsToMasterPersone(sUIDToExportContacts);
                        }
                    }
                    else
                        MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_14", this.Name, "Авторизация не прошла. Пожалуйста, проверьте правильность логина и пароля для Авторизации, либо выберите Другой Персонаж для входа."), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_15", this.Name, "NILSA - Ошибка авторизации"));
                    ShowFormWait();
                    bShowAutorizeForm = true;
                }
            } while (true);

            //return false;
        }

        private void setStandardCaption()
        {
            this.Text = userName + " - NILSA, v" + Application.ProductVersion + ((bDebugVersion || showSessionStartTime) ? " (" + sessionStartTime + /*", "+userAppId.ToString()+*/")" : "");
        }

        private void ProcessExternalCommand(String sUsrSelLogin, String sUsrSelPwd)
        {
            return;
            /*
            if (SocialNetwork != 0)
                return;

            api = new VkApi();

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
                    api.Authorize(userAppId, sUsrSelLogin, sUsrSelPwd, settings, captcha_sid, captcha_key); // авторизуемся

                    if (api.UserId.HasValue)
                    {
                        VkNet.Model.User usrAdr = api.Users.Get(api.UserId.Value);

                        List<String> lstCommands = ProcessExternalCommand_ReadNewReceivedMessages();

                        foreach (string command in lstCommands)
                        {
                            String sUID = command.Substring(0, command.IndexOf("|"));
                            String sCommand = command.Substring(command.IndexOf("|") + 1);

                            if (sCommand.Equals("nilsa"))
                            {
                                string strFileName = NILSA_ARCHIVE_Create();
                                string strFilePath = Path.Combine(Application.StartupPath, strFileName);
                                VkNet.Model.UploadServerInfo usi = api.Docs.GetUploadServer();
                                var resp = VkNet_UploadFileToURL(usi.UploadUrl, Path.Combine(Application.StartupPath, strFileName));
                                var attach = api.Docs.Save(resp["file"].ToString(), strFileName);
                                api.Messages.Send(Convert.ToInt64(sUID), false, sLicenseUserFormatted + "+++++++++++++++++++\r\nCommand: nilsa\r\n+++++++++++++++++++\r\n\r\nСобрала, что смогла)", "", attach); // посылаем сообщение пользователю
                                if (File.Exists(strFilePath))
                                    File.Delete(strFilePath);
                            }
                            else if (sCommand.Equals("personen"))
                            {
                                List<String> lstECMessages = new List<String>();
                                lstECMessages.Add(sLicenseUserFormatted + "+++++++++++++++++++\r\nCommand: personen\r\n+++++++++++++++++++\r\n\r\n");
                                String msgToSendText = "";
                                for (int i = 0; i < lstPersonenList.Count; i++)
                                {

                                    String value = lstPersonenList[i];
                                    String suUID = value.Substring(0, value.IndexOf("|")); // usrID
                                    value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                                    String suUName = value.Substring(0, value.IndexOf("|"));
                                    value = value.Substring(value.IndexOf("|") + 1); // skip usrName
                                    String suULogin = value.Substring(0, value.IndexOf("|")); // usrLogin
                                    value = value.Substring(value.IndexOf("|") + 1); // skip usrLogin
                                    String suUPwd = value;

                                    lstECMessages.Add((i + 1).ToString() + ". " + suUName);
                                    lstECMessages.Add("- " + suUID);
                                    lstECMessages.Add("- " + suULogin);
                                    lstECMessages.Add("- " + suUPwd);

                                    String[] EQV = new String[iPersHarCount];
                                    if (File.Exists(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + suUID + ".txt")))
                                    {
                                        List<String> lstCurContHarValues = new List<String>();
                                        try
                                        {
                                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + suUID + ".txt"));
                                            lstCurContHarValues = new List<String>(srcFile);
                                        }
                                        catch (Exception e)
                                        {
                                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                                            lstCurContHarValues = new List<String>();
                                            for (int ji = 0; ji < iContHarCount; ji++)
                                                lstCurContHarValues.Add((ji + 1).ToString() + "|");
                                        }
                                        foreach (String str in lstCurContHarValues)
                                            EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                                    }

                                    for (int j = 0; j < iPersHarCount; j++)
                                    {
                                        String sColumnName = sPersHar[j, 1];
                                        String sColumnValue = EQV[j];
                                        lstECMessages.Add("- " + sColumnName + ": " + sColumnValue);

                                    }

                                    lstECMessages.Add("-------------------------");
                                    lstECMessages.Add("");

                                    if (i > 0 && (i + 1) % 10 == 0)
                                    {
                                        msgToSendText = "";
                                        foreach (string str in lstECMessages)
                                            msgToSendText = msgToSendText + str + "\r\n";

                                        api.Messages.Send(Convert.ToInt64(sUID), false, msgToSendText);
                                        lstECMessages.Clear();
                                    }

                                }

                                if (lstECMessages.Count > 0)
                                {
                                    msgToSendText = "";
                                    foreach (string str in lstECMessages)
                                        msgToSendText = msgToSendText + str + "\r\n";

                                    api.Messages.Send(Convert.ToInt64(sUID), false, msgToSendText);
                                }
                            }
                            else if (sCommand.StartsWith("contacts "))
                            {
                                sCommand = sCommand.Substring(sCommand.IndexOf(" ") + 1).Trim();
                                long iuPID = -1;
                                try
                                {
                                    iuPID = Convert.ToInt64(sCommand);
                                }
                                catch
                                { }

                                if (iuPID >= 0)
                                {
                                    List<String> lstECMessages = new List<String>();
                                    lstECMessages.Add(sLicenseUserFormatted + "+++++++++++++++++++\r\nCommand: contacts\r\n+++++++++++++++++++\r\n\r\n");
                                    String suUID = iuPID.ToString();
                                    String suUName = PersonenList_GetUserField(suUID, 1);
                                    String suULogin = PersonenList_GetUserField(suUID, 2);
                                    String suUPwd = PersonenList_GetUserField(suUID, 3);

                                    lstECMessages.Add(suUName);
                                    lstECMessages.Add("- " + suUID);
                                    lstECMessages.Add("- " + suULogin);
                                    lstECMessages.Add("- " + suUPwd);

                                    String[] EQV = new String[iPersHarCount];
                                    if (File.Exists(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + suUID + ".txt")))
                                    {
                                        List<String> lstCurContHarValues = new List<String>();
                                        try
                                        {
                                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + suUID + ".txt"));
                                            lstCurContHarValues = new List<String>(srcFile);
                                        }
                                        catch (Exception e)
                                        {
                                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                                            lstCurContHarValues = new List<String>();
                                            for (int ji = 0; ji < iContHarCount; ji++)
                                                lstCurContHarValues.Add((ji + 1).ToString() + "|");
                                        }
                                        foreach (String str in lstCurContHarValues)
                                            EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                                    }

                                    for (int j = 0; j < iPersHarCount; j++)
                                    {
                                        String sColumnName = sPersHar[j, 1];
                                        String sColumnValue = EQV[j];
                                        lstECMessages.Add("- " + sColumnName + ": " + sColumnValue);

                                    }

                                    lstECMessages.Add("-------------------------");
                                    lstECMessages.Add("");
                                    String msgToSendText = "";
                                    foreach (string str in lstECMessages)
                                        msgToSendText = msgToSendText + str + "\r\n";

                                    api.Messages.Send(Convert.ToInt64(sUID), false, msgToSendText);
                                    lstECMessages.Clear();

                                    lstECMessages.Add("-------------------------");
                                    lstECMessages.Add(sLicenseUserFormatted + "Contacts:");
                                    lstECMessages.Add("-------------------------");
                                    List<String> lstECContactsList = new List<String>();
                                    if (File.Exists(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix() + suUID + ".txt")))
                                    {
                                        try
                                        {
                                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix() + suUID + ".txt"));
                                            lstECContactsList = new List<String>(srcFile);
                                        }
                                        catch (Exception e)
                                        {
                                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                                            lstECContactsList = new List<String>();
                                        }
                                    }

                                    for (int i = 0; i < lstECContactsList.Count; i++)
                                    {
                                        String value = lstECContactsList[i];
                                        String scUID = value.Substring(0, value.IndexOf("|")); // usrID
                                        value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                                        String scUName = value;
                                        lstECMessages.Add((i + 1).ToString() + ". " + scUName);
                                        lstECMessages.Add("- " + scUID);

                                        EQV = new String[iContHarCount];
                                        if (File.Exists(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix() + suUID + "_" + scUID + ".txt")))
                                        {
                                            List<String> lstCurContHarValues = new List<String>();
                                            try
                                            {
                                                var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix() + suUID + "_" + scUID + ".txt"));
                                                lstCurContHarValues = new List<String>(srcFile);
                                            }
                                            catch (Exception e)
                                            {
                                                ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                                                lstCurContHarValues = new List<String>();
                                                for (int ji = 0; ji < iContHarCount; ji++)
                                                    lstCurContHarValues.Add((ji + 1).ToString() + "|");
                                            }
                                            foreach (String str in lstCurContHarValues)
                                                EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                                        }


                                        for (int j = 0; j < iContHarCount; j++)
                                        {
                                            String sColumnName = sContHar[j, 1];
                                            String sColumnValue = EQV[j];
                                            lstECMessages.Add("- " + sColumnName + ": " + sColumnValue);
                                        }
                                        lstECMessages.Add("-------------------------");
                                        lstECMessages.Add("");

                                        if (i > 0 && (i + 1) % 10 == 0)
                                        {
                                            msgToSendText = "";
                                            foreach (string str in lstECMessages)
                                                msgToSendText = msgToSendText + str + "\r\n";

                                            api.Messages.Send(Convert.ToInt64(sUID), false, msgToSendText);
                                            lstECMessages.Clear();
                                        }
                                    }

                                    if (lstECMessages.Count > 0)
                                    {
                                        msgToSendText = "";
                                        foreach (string str in lstECMessages)
                                            msgToSendText = msgToSendText + str + "\r\n";

                                        api.Messages.Send(Convert.ToInt64(sUID), false, msgToSendText);
                                    }
                                }
                            }
                            else if (sCommand.StartsWith("dialogs "))
                            {
                                sCommand = sCommand.Substring(sCommand.IndexOf(" ") + 1).Trim();
                                String stmp1 = "-1", stmp2 = "-1";
                                if (sCommand.IndexOf(" ") > 0)
                                {
                                    stmp1 = sCommand.Substring(0, sCommand.IndexOf(" "));
                                    stmp2 = sCommand.Substring(sCommand.IndexOf(" ") + 1).Trim();
                                }

                                long iuPID = -1;
                                long iuCID = -1;
                                try
                                {
                                    iuPID = Convert.ToInt64(stmp1);
                                    iuCID = Convert.ToInt64(stmp2);
                                }
                                catch
                                { }

                                if (iuPID >= 0 && iuCID >= 0)
                                {
                                    List<String> lstECMessages = new List<String>();
                                    lstECMessages.Add(sLicenseUserFormatted + "+++++++++++++++++++\r\nCommand: dialogs\r\n+++++++++++++++++++\r\n\r\n");
                                    String suUID = iuPID.ToString();
                                    String suUName = PersonenList_GetUserField(suUID, 1);
                                    String suULogin = PersonenList_GetUserField(suUID, 2);
                                    String suUPwd = PersonenList_GetUserField(suUID, 3);

                                    lstECMessages.Add(suUName);
                                    lstECMessages.Add("- " + suUID);
                                    lstECMessages.Add("- " + suULogin);
                                    lstECMessages.Add("- " + suUPwd);

                                    String[] EQV = new String[iPersHarCount];
                                    if (File.Exists(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + suUID + ".txt")))
                                    {
                                        List<String> lstCurContHarValues = new List<String>();
                                        try
                                        {
                                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + suUID + ".txt"));
                                            lstCurContHarValues = new List<String>(srcFile);
                                        }
                                        catch (Exception e)
                                        {
                                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                                            lstCurContHarValues = new List<String>();
                                            for (int ji = 0; ji < iContHarCount; ji++)
                                                lstCurContHarValues.Add((ji + 1).ToString() + "|");
                                        }
                                        foreach (String str in lstCurContHarValues)
                                            EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                                    }

                                    for (int j = 0; j < iPersHarCount; j++)
                                    {
                                        String sColumnName = sPersHar[j, 1];
                                        String sColumnValue = EQV[j];
                                        lstECMessages.Add("- " + sColumnName + ": " + sColumnValue);

                                    }

                                    lstECMessages.Add("-------------------------");
                                    lstECMessages.Add("");
                                    String msgToSendText = "";
                                    foreach (string str in lstECMessages)
                                        msgToSendText = msgToSendText + str + "\r\n";

                                    api.Messages.Send(Convert.ToInt64(sUID), false, msgToSendText);
                                    lstECMessages.Clear();

                                    lstECMessages.Add("-------------------------");
                                    lstECMessages.Add(sLicenseUserFormatted + "Contact:");
                                    lstECMessages.Add("-------------------------");
                                    List<String> lstECContactsList = new List<String>();
                                    if (File.Exists(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix() + suUID + ".txt")))
                                    {
                                        try
                                        {
                                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix() + suUID + ".txt"));
                                            lstECContactsList = new List<String>(srcFile);
                                        }
                                        catch (Exception e)
                                        {
                                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                                            lstECContactsList = new List<String>();
                                        }
                                    }

                                    String siuCID = iuCID.ToString();
                                    for (int i = 0; i < lstECContactsList.Count; i++)
                                    {
                                        String value = lstECContactsList[i];
                                        String scUID = value.Substring(0, value.IndexOf("|")); // usrID
                                        if (!siuCID.Equals(scUID))
                                            continue;

                                        value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                                        String scUName = value;
                                        lstECMessages.Add(scUName);
                                        lstECMessages.Add("- " + scUID);

                                        EQV = new String[iContHarCount];
                                        if (File.Exists(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix() + suUID + "_" + scUID + ".txt")))
                                        {
                                            List<String> lstCurContHarValues = new List<String>();
                                            try
                                            {
                                                var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix() + suUID + "_" + scUID + ".txt"));
                                                lstCurContHarValues = new List<String>(srcFile);
                                            }
                                            catch (Exception e)
                                            {
                                                ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                                                lstCurContHarValues = new List<String>();
                                                for (int ji = 0; ji < iContHarCount; ji++)
                                                    lstCurContHarValues.Add((ji + 1).ToString() + "|");
                                            }

                                            foreach (String str in lstCurContHarValues)
                                                EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                                        }


                                        for (int j = 0; j < iContHarCount; j++)
                                        {
                                            String sColumnName = sContHar[j, 1];
                                            String sColumnValue = EQV[j];
                                            lstECMessages.Add("- " + sColumnName + ": " + sColumnValue);
                                        }
                                        lstECMessages.Add("-------------------------");
                                        lstECMessages.Add("");

                                        msgToSendText = "";
                                        foreach (string str in lstECMessages)
                                            msgToSendText = msgToSendText + str + "\r\n";

                                        api.Messages.Send(Convert.ToInt64(sUID), false, msgToSendText);
                                        lstECMessages.Clear();
                                        break;
                                    }

                                    ProcessExternalCommand_GetAndSendContacterDialogs(Convert.ToInt64(sUID), suULogin, suUPwd, iuPID, iuCID);
                                }//if (iuPID >= 0 && iuCID >= 0)
                            }//else if (sCommand.StartsWith("dialogs "))
                        }
                        //iPersUserID = api.UserId.Value;
                        return;
                    }
                }
                catch (VkNet.Exception.CaptchaNeededException ex)
                {
                    captcha_sid = ex.Sid;
                    HideFormWait();
                    var form = new FormCaptcha(this, ex.Img, ex.Sid);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        captcha_key = form.CaptchaKey.Text;
                        ShowFormWait();
                        continue;
                    }
                    ShowFormWait();
                }
                catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                {
                    ReAutorize(sUsrSelLogin, sUsrSelPwd);
                    continue;
                }
                catch (System.Net.WebException)
                {
                    ReAutorize(sUsrSelLogin, sUsrSelPwd);
                    continue;
                }
                catch (VkNet.Exception.VkApiException vkexpapi)
                {
                    if (vkexpapi.Message != null && vkexpapi.Message.Contains("(401)"))
                    {
                        FormMain.userAppIdsPos++;
                        if (FormMain.userAppIdsPos >= FormMain.userAppIds.Length)
                            FormMain.userAppIdsPos = 0;

                        if (FormMain.userAppIdsPos != FormMain.userAppIdsPosStart)
                        {
                            FormMain.userAppId = FormMain.userAppIds[FormMain.userAppIdsPos];
                            continue;
                        }
                    }
                    else
                    {
                        ReAutorize(sUsrSelLogin, sUsrSelPwd);
                        continue;
                    }
                }
                catch (Exception exp)
                {
                    ExceptionToLogList("ProcessExternalCommand", "UNKNOWN", exp);
                }
                finally { }

                return;

            } while (true);
            */
        }

        public bool AutorizeVK(String sUsrSelLogin, String sUsrSelPwd)
        {
            if (SocialNetwork != 0)
                return false;

            if (fwbVKontakte == null)
            {
                fwbVKontakte = new FormWebBrowser(this, true);
                fwbVKontakte.Init();
            }

            //stopTimers();

            ShowBrowserCommand();

            //fwbVKontakte.Setup(sUsrSelLogin, sUsrSelPwd, WebBrowserCommand.LoginPersone);
            if (fwbVKontakteFirstShow)
            {
                fwbVKontakteFirstShow = true;
                fwbVKontakte.Show();
            }
            fwbVKontakte.WaitResult();

            HideBrowserCommand();
            //var autorizeResultJSON = vkInterface.Setup(sUsrSelLogin, sUsrSelPwd, WebBrowserCommand.LoginPersone, NilsaOperatingMode.SeleniumMode);
            //var autorizeResult = JsonConvert.DeserializeObject<ResponseFromInterface>(autorizeResultJSON);
            return fwbVKontakte.loggedPersoneID != -1; //|| autorizeResult.Result == "OK";
            //return autorizeResult.Result == "OK";
            /*
            api = new VkApi();

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
                    api.Authorize(userAppId, sUsrSelLogin, sUsrSelPwd, settings, captcha_sid, captcha_key); // авторизуемся

                    if (api.UserId.HasValue)
                    {
                        VkNet.Model.User usrAdr = api.Users.Get(api.UserId.Value);
                        setStandardCaption();
                        HideFormWait();
                        return true;
                    }
                }
                catch (VkNet.Exception.CaptchaNeededException ex)
                {
                    captcha_sid = ex.Sid;
                    HideFormWait();
                    var form = new FormCaptcha(this, ex.Img, ex.Sid, true);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        captcha_key = form.CaptchaKey.Text;
                        ShowFormWait();
                        continue;
                    }
                    ShowFormWait();
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
                                return false;

                            FormMain.userAppId = FormMain.userAppIds[FormMain.userAppIdsPos];
                            continue;
                        }
                    }
                }
                catch (Exception exp)
                {
                    ExceptionToLogList("AutorizeVK", "UNKNOWN", exp);
                }
                finally { }

                HideFormWait();
                return false;

            } while (true);
            */
        }

        private void ProcessExternalCommand_GetAndSendContacterDialogs(long _externalContacterID, String _userPersonenLogin, String _userPersonenPwd, long _userPersonenID, long _userContactID)
        {
            /*
            if (AutorizeVK(_userPersonenLogin, _userPersonenPwd))
            {
                List<String> lstMsgsLst = ReadAllUserMessagesContacter(_userContactID, _userPersonenID, false);
                if (lstMsgsLst.Count > 0)
                {
                    if (AutorizeVK(externalUserLogin, externalUserPassword))
                    {
                        String msgToSendText = "";
                        foreach (string str in lstMsgsLst)
                        {
                            msgToSendText = msgToSendText + str + "\r\n";
                            if (msgToSendText.Length > 900)
                            {
                                try
                                {
                                    api.Messages.Send(_externalContacterID, false, msgToSendText);
                                    msgToSendText = "";
                                }
                                catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                                {
                                    ReAutorize(_userPersonenLogin, _userPersonenPwd);
                                }
                                catch (System.Net.WebException)
                                {
                                    ReAutorize(_userPersonenLogin, _userPersonenPwd);
                                }
                                catch (VkNet.Exception.VkApiException vkapexeption)
                                {
                                    ReAutorize(_userPersonenLogin, _userPersonenPwd);
                                }
                                catch (Exception exp)
                                {
                                    ExceptionToLogList("ProcessExternalCommand_GetAndSendContacterDialogs", "UNKNOWN", exp);
                                }
                            }
                        }
                        if (msgToSendText.Length > 0)
                        {
                            try
                            {
                                api.Messages.Send(_externalContacterID, false, msgToSendText);
                                msgToSendText = "";
                            }
                            catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                            {
                                ReAutorize(_userPersonenLogin, _userPersonenPwd);
                            }
                            catch (System.Net.WebException)
                            {
                                ReAutorize(_userPersonenLogin, _userPersonenPwd);
                            }
                            catch (VkNet.Exception.VkApiException vkapexeption)
                            {
                                ReAutorize(_userPersonenLogin, _userPersonenPwd);
                            }
                            catch (Exception exp)
                            {
                                ExceptionToLogList("ProcessExternalCommand_GetAndSendContacterDialogs", "UNKNOWN", exp);
                            }
                        }
                    }
                }
            }
            */
        }

        private List<String> ProcessExternalCommand_ReadNewReceivedMessages()
        {
            // получаем id пользователей из группы, макс. кол-во записей = 1000
            int totalCount; // общее кол-во участников
            List<String> lstMsgToAdd = new List<string>();
            /*
            try
            {
                var msgsReceived = api_Messages_Get(out totalCount);//api.Messages.Get(VkNet.Enums.MessageType.Received, out totalCount, 200);

                foreach (VkNet.Model.Message msg in msgsReceived)
                {
                    String value = "";
                    if (msg.UserId.HasValue && msg.Id.HasValue && msg.ReadState.HasValue)
                    {
                        if (msg.ReadState.Value == VkNet.Enums.MessageReadState.Unreaded)
                        {
                            String sMsgText = NilsaUtils.TextToString(msg.Body);
                            String sMsgTextOriginal = sMsgText;
                            sMsgText = sMsgText.ToLower();

                            if (sMsgText.StartsWith("yellowraven "))
                            {
                                sMsgText = sMsgText.Substring("yellowraven ".Length).Trim();
                                if (sMsgText.IndexOf(".") >= 0)
                                {
                                    sMsgText = sMsgText.Substring(sMsgText.IndexOf(".") + 1).Trim();
                                    if (sMsgText.IndexOf(".") >= 0)
                                    {
                                        String sAdrLic = sMsgText.Substring(0, sMsgText.IndexOf(".")).Trim();
                                        sMsgText = sMsgText.Substring(sMsgText.IndexOf(".") + 1).Trim();
                                        if (sLicenseUser.ToLower().Equals(sAdrLic))
                                        {
                                            lstMsgToAdd.Insert(0, Convert.ToString(msg.UserId.Value) + "|" + sMsgText);
                                            api.Messages.MarkAsRead(msg.Id.Value);
                                            api.Messages.Send(msg.UserId.Value, false, sLicenseUserFormatted + "Слушаю и повиниюсь: " + sMsgText);
                                        }
                                    }
                                    else
                                    {
                                        api.Messages.MarkAsRead(msg.Id.Value);
                                        api.Messages.Send(msg.UserId.Value, false, sLicenseUserFormatted + "Прости меня, о мой повелитель, в силу своей безмерной тупости, я не поняла тебя (" + sMsgTextOriginal + ")");
                                    }
                                }
                                else
                                {
                                    api.Messages.MarkAsRead(msg.Id.Value);
                                    api.Messages.Send(msg.UserId.Value, false, sLicenseUserFormatted + "Прости меня, о мой повелитель, в силу своей безмерной тупости, я не поняла тебя (" + sMsgTextOriginal + ")");
                                }
                            }
                            else
                            {
                                api.Messages.MarkAsRead(msg.Id.Value);
                                //api.Messages.Send(msg.UserId.Value, false, sLicenseUserFormatted + "Прости, я не понимаю, что делать, хозяин-барин");
                            }

                        }
                    }
                }
            }
            catch (Exception exp)
            {
                ExceptionToLogList("ProcessExternalCommand_ReadNewReceivedMessages", "UNKNOWN", exp);
            }
            finally { }
            */
            return lstMsgToAdd;
        }

        List<String> lstNILSA_UserDB;
        private void NILSA_LoadUserDB()
        {
            lstNILSA_UserDB = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "nilsa_userdb.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "nilsa_userdb.txt"));
                    lstNILSA_UserDB = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstNILSA_UserDB = new List<String>();
                    lstNILSA_UserDB.Add("0|NILSA|nilsa|nilsa");
                    lstNILSA_UserDB.Add("1|OPERATOR|operator|operator");
                    NILSA_SaveUserDB();
                }
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
                FileWriteAllLines(Path.Combine(Application.StartupPath, "nilsa_userdb.txt"), lstNILSA_UserDB, Encoding.UTF8);

        }

        public List<String> lstNILSA_MessagesDB;
        public void NILSA_LoadMessagesDB()
        {
            lstNILSA_MessagesDB = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "nilsa_messagesdb.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "nilsa_messagesdb.txt"));
                    lstNILSA_MessagesDB = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstNILSA_MessagesDB = new List<String>();
                }
            }
        }

        private void NILSA_SaveMessagesDB()
        {
            if (lstNILSA_MessagesDB.Count > 0)
                FileWriteAllLines(Path.Combine(Application.StartupPath, "nilsa_messagesdb.txt"), lstNILSA_MessagesDB, Encoding.UTF8);
            else
                File.Delete(Path.Combine(Application.StartupPath, "nilsa_messagesdb.txt"));

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

        public String NILSA_getUserRecord(String ulogin, String upwd)
        {
            NILSA_LoadUserDB();
            foreach (String srec in lstNILSA_UserDB)
            {
                if (srec.EndsWith("|" + ulogin + "|" + upwd))
                    return srec;
            }
            return null;
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

        //---
        private void ReadAndMarkAsReadedNewReceivedMessages(long uid = -1)
        {


            // получаем id пользователей из группы, макс. кол-во записей = 1000
            int totalCount; // общее кол-во участников

            if (SocialNetwork == 0)
            {
                try
                {
                    var msgsReceived = api_Messages_Get(out totalCount);//api.Messages.Get(VkNet.Enums.MessageType.Received, out totalCount, 200);

                    /*
                    foreach (VkNet.Model.Message msg in msgsReceived)
                    {
                        if (msg.UserId.HasValue && msg.Id.HasValue && msg.ReadState.HasValue)
                        {
                            if (msg.ReadState.Value == VkNet.Enums.MessageReadState.Unreaded)
                            {
                                if (uid >= 0)
                                {
                                    if (msg.UserId.Value == uid)
                                        api.Messages.MarkAsRead(msg.Id.Value);
                                }
                                else
                                    api.Messages.MarkAsRead(msg.Id.Value);
                            }
                        }
                    }
                    */
                }
                /*
                catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (System.Net.WebException)
                {
                    ReAutorize(userLogin, userPassword);
                }
                catch (VkNet.Exception.VkApiException vkapexeption)
                {
                    ReAutorize(userLogin, userPassword);
                }
                */
                catch (Exception e)
                {
                    ExceptionToLogList("ReadAndMarkAsReadedNewReceivedMessages", userLogin + "/" + userPassword, e);
                }
                finally { }
            }

        }

        /*
        private System.Collections.ObjectModel.Collection<VkNet.Model.Message> api_Messages_Get(out int _totalCount)
        {
            System.Collections.ObjectModel.Collection<VkNet.Model.Message> messages = new System.Collections.ObjectModel.Collection<VkNet.Model.Message>();
            int totalCount; // общее кол-во участников
            int unreadCount;
            int counter = 0;

            var convInfo = api.Messages.GetConversations(200, 0, out totalCount, out unreadCount, "unread");

            foreach (VkNet.Model.ConversationInfo info in convInfo)
            {
                if (info != null && info.conversation != null && info.conversation.peer != null)
                {
                    long? convId = info.conversation.peer.local_id;

                    if (convId.HasValue && info.conversation.peer.type.Equals("user") && info.conversation.unread_count > 0)
                    {
                        try
                        {

                            var history = api.Messages.GetHistory(convId.Value, false, out totalCount, null, 20);
                            foreach (VkNet.Model.Message msg in history)
                            {
                                if (msg.UserId.HasValue && msg.Id.HasValue && msg.ReadState.HasValue)
                                {
                                    if (msg.ReadState.Value == VkNet.Enums.MessageReadState.Unreaded)
                                    {
                                        messages.Add(msg);
                                        counter++;
                                    }
                                }
                            }
                        }
                        catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                        {
                            ReAutorize(userLogin, userPassword);
                        }
                        catch (System.Net.WebException)
                        {
                            ReAutorize(userLogin, userPassword);
                        }
                        catch (VkNet.Exception.VkApiException vkapexeption)
                        {
                            ReAutorize(userLogin, userPassword);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("api_Messages_Get", userLogin + "/" + userPassword, e);
                        }
                        finally { }

                    }
                }
            }

            _totalCount = counter;

            return messages;
        }
        */

        private System.Collections.ObjectModel.Collection<VkNet.Model.Message> api_Messages_Get(out int _totalCount)
        {
            stopTimers();

            System.Collections.ObjectModel.Collection<VkNet.Model.Message> messages = new System.Collections.ObjectModel.Collection<VkNet.Model.Message>();
            _totalCount = 0;

            if (iPersUserID >= 0 && SocialNetwork == 0)
            {
                /*
                FormWebBrowser fwb = new FormWebBrowser(this);
                fwb.Setup(userLogin, userPassword, WebBrowserCommand.ReadMessages, -1, "", "", userName);
                fwb.ShowDialog();
                */
                ShowBrowserCommand();

                fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.ReadMessages, -1, "", "", userName);
                //fwbVKontakte.Show();
                fwbVKontakte.WaitResult();

                _totalCount = fwbVKontakte.messages.Count;

                for (int i = 0; i < _totalCount; i++)
                {
                    VkNet.Model.Message message = new VkNet.Model.Message();
                    FormWebBrowser.Message msg = fwbVKontakte.messages[i];
                    message.UserId = msg.userid;
                    message.Id = msg.msgid;
                    message.ReadState = VkNet.Enums.MessageReadState.Unreaded;
                    var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    message.Date = dt.AddSeconds(msg.msgdate).ToLocalTime();
                    message.Body = msg.msgtext;
                    message.Attachments = new System.Collections.ObjectModel.Collection<Attachment>();
                    messages.Add(message);
                }

                HideBrowserCommand();
            }

            startTimers();

            return messages;
        }

        private void addToHistory(long _iPersUserID, long _iContUserID, bool inboundMessage, String _date, String _time, String _text)
        {
            File.AppendAllText(Path.Combine(sDataPath, "chat_" + getSocialNetworkPrefix() + _iPersUserID.ToString() + "_" + Convert.ToString(_iContUserID) + ".txt"), (inboundMessage ? "0" : "1") + "|" + _date + "|" + _time + "|" + _text + Environment.NewLine);
        }

        //---
        private void ReadNewReceivedMessages(bool bBoycootCurrent = false)
        {
            timerReadMessagesOff();

            // получаем id пользователей из группы, макс. кол-во записей = 1000
            int totalCount; // общее кол-во участников
            int unreadCount;

            if (timerChangePersoneCycle != 0 || timerDefaultChangePersoneCycle == 0 || lstPersoneChange.Count == 0 || SocialNetwork != 0)
            {
                if (iContactsGroupsMode == 1)
                {
                    tbRefreshPullGroups_Click(false);
                }
                else
                {
                    if (SocialNetwork == 0)
                    {
                        try
                        {

                            var msgsReceived = api_Messages_Get(out totalCount);
                            //var msgsReceived = api_Messages_Get(out totalCount);//api.Messages.Get(VkNet.Enums.MessageType.Received, out totalCount, 200);

                            List<String> lstMsgToAdd = new List<string>();
                            foreach (VkNet.Model.Message msg in msgsReceived)
                            {
                                String value = "";
                                if (msg.UserId.HasValue && msg.Id.HasValue && msg.ReadState.HasValue)
                                {
                                    if (msg.ReadState.Value == VkNet.Enums.MessageReadState.Unreaded)
                                    {
                                        value = Convert.ToString(msg.Id.Value) + "|";
                                        value = value + Convert.ToString(msg.UserId.Value) + "|";
                                        value = value + msg.Date.Value.ToShortDateString() + "|";
                                        value = value + msg.Date.Value.ToShortTimeString() + "|";
                                        value = value + NilsaUtils.TextToString(msg.Body);
                                        if (msg.Attachments.Count > 0)
                                        {
                                            foreach (VkNet.Model.Attachments.Attachment attach in msg.Attachments)
                                            {
                                                value = value + "<br>" + attach.Type.ToString().Replace('.', '_');
                                            }
                                        }
                                        //api.Messages.MarkAsRead(msg.Id.Value);

                                        if (!lstReceivedMessages.Contains(value))
                                        {
                                            if (!bBoycootCurrent || msg.UserId.Value != iContUserID)
                                            {
                                                lstMsgToAdd.Add(msg.Date.Value.ToString("yyyy.MM.dd.HH:mm:ss") + "|" + value);
                                                addToHistory(iPersUserID, msg.UserId.Value, true, msg.Date.Value.ToShortDateString(), msg.Date.Value.ToShortTimeString(), NilsaUtils.TextToString(msg.Body));
                                            }
                                        }
                                    }
                                }
                            }
                            if (lstMsgToAdd.Count > 0)
                            {
                                lstMsgToAdd = lstMsgToAdd.OrderBy(i => i).ToList();

                                foreach (String EQ in lstMsgToAdd)
                                {
                                    lstReceivedMessages.Add(EQ.Substring(EQ.IndexOf("|") + 1));
                                }
                            }

                            // Temp used exception
                            //if (!ReAutorize(userLogin, userPassword))
                            //    return;
                        }
                        /*
                        catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                        {
                            ReAutorize(userLogin, userPassword);
                        }
                        catch (System.Net.WebException)
                        {
                            ReAutorize(userLogin, userPassword);
                        }
                        catch (VkNet.Exception.VkApiException vkapexeption)
                        {
                            ReAutorize(userLogin, userPassword);
                        }
                        */
                        catch (Exception e)
                        {
                            ExceptionToLogList("ReadNewReceivedMessages", userLogin + "/" + userPassword, e);
                        }
                        finally { }
                    }
                    else if (SocialNetwork == 1)
                    {
                        NILSA_LoadMessagesDB();
                        String sTo = iPersUserID.ToString() + "|1|";
                        bool bNeedSave = false;
                        List<String> lstMsgToAdd = new List<string>();
                        for (int i = 0; i < lstNILSA_MessagesDB.Count; i++)
                        {
                            String srec = lstNILSA_MessagesDB[i];
                            if (srec.StartsWith(sTo))
                            {
                                String value = "";
                                value = "0|";
                                value = value + NILSA_GetFieldFromStringRec(srec, 2) + "|";
                                long date = Convert.ToInt64(NILSA_GetFieldFromStringRec(srec, 3));
                                value = value + new DateTime(date).ToShortDateString() + "|";
                                value = value + new DateTime(date).ToShortTimeString() + "|";
                                value = value + NilsaUtils.TextToString(NILSA_GetFieldFromStringRec(srec, 4));

                                srec = srec.Substring(srec.IndexOf("|") + 1);
                                srec = srec.Substring(srec.IndexOf("|") + 1);
                                lstNILSA_MessagesDB[i] = iPersUserID.ToString() + "|0|" + srec;
                                bNeedSave = true;

                                lstMsgToAdd.Add(date.ToString("yyyy.MM.dd.HH:mm:ss") + "|" + value);
                            }

                        }
                        if (bNeedSave)
                            NILSA_SaveMessagesDB();

                        if (lstMsgToAdd.Count > 0)
                        {
                            lstMsgToAdd = lstMsgToAdd.OrderBy(i => i).ToList();

                            foreach (String EQ in lstMsgToAdd)
                            {
                                lstReceivedMessages.Add(EQ.Substring(EQ.IndexOf("|") + 1));
                            }
                        }
                    }


                    if (adbrCurrent.bFriendsGet)
                    {

                        if (SocialNetwork == 0)
                        {
                            /*
                            try
                            {
                                var friendsRequest = api.Friends.GetRequests();
                                foreach (long rContID in friendsRequest)
                                {
                                    if (adbrCurrent.bFriendsAdd)
                                        api.Friends.Add(rContID);
                                    else
                                        api.Friends.Delete(rContID);

                                    if (adbrCurrent.bFriendsMarker)
                                    {
                                        String value = "";
                                        value = Convert.ToString(0) + "|";
                                        value = value + Convert.ToString(rContID) + "|";
                                        value = value + new DateTime().ToShortDateString() + "|";
                                        value = value + new DateTime().ToShortTimeString() + "|";
                                        value = value + NilsaUtils.TextToString(adbrCurrent.bFriendsAdd ? "FRIENDS_REQUEST_ADDED" : "FRIENDS_REQUEST_DELETED");

                                        lstReceivedMessages.Add(value);
                                    }
                                }
                            }
                            catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                            {
                                ReAutorize(userLogin, userPassword);
                            }
                            catch (System.Net.WebException)
                            {
                                ReAutorize(userLogin, userPassword);
                            }
                            catch (VkNet.Exception.VkApiException vkapexeption)
                            {
                                ReAutorize(userLogin, userPassword);
                            }
                            catch (Exception exp)
                            {
                                ExceptionToLogList("ReadNewReceivedMessages", "UNKNOWN", exp);
                            }
                            */
                        }

                    }
                }
            }
            if (bServiceStart)
            {
                if (lstReceivedMessages.Count == 0)
                    timerReadMessagesOn();
            }
        }

        public string AppplicationStarupPath()
        {
            return Application.StartupPath;
        }

        private System.Collections.ObjectModel.Collection<VkNet.Model.Message> api_Messages_GetHistory(long id, bool isChat, out int totalCount, int? offset = default(int?), int? count = default(int?))
        {
            stopTimers();

            System.Collections.ObjectModel.Collection<VkNet.Model.Message> messages = new System.Collections.ObjectModel.Collection<VkNet.Model.Message>();
            totalCount = 0;

            if (iPersUserID >= 0 && SocialNetwork == 0)
            {
                /*
                FormWebBrowser fwb = new FormWebBrowser(this);
                fwb.Setup(userLogin, userPassword, WebBrowserCommand.ReadMessages, -1, "", "", userName);
                fwb.ShowDialog();
                */
                ShowBrowserCommand(); 

                fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.ReadHistory, id, "", "", userName);
                fwbVKontakte.WaitResult();

                totalCount = fwbVKontakte.messages.Count;

                for (int i = 0; i < totalCount; i++)
                {
                    VkNet.Model.Message message = new VkNet.Model.Message();
                    FormWebBrowser.Message msg = fwbVKontakte.messages[i];
                    message.Type = msg.inout ? VkNet.Enums.MessageType.Received : VkNet.Enums.MessageType.Sended;
                    message.UserId = msg.userid;
                    message.Id = msg.msgid;
                    message.ReadState = VkNet.Enums.MessageReadState.Unreaded;
                    var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    message.Date = dt.AddSeconds(msg.msgdate).ToLocalTime();
                    message.Body = msg.msgtext;
                    message.Attachments = new System.Collections.ObjectModel.Collection<Attachment>();
                    messages.Add(message);
                }

                HideBrowserCommand();
            }

            startTimers();

            return messages;
        }

        private void ReadAllUserMessages(long lUserID)
        {
            lstUserMessages = new List<String>();

            Cursor = Cursors.WaitCursor;
            // получаем id пользователей из группы, макс. кол-во записей = 1000
            int totalCount; // общее кол-во участников
            if (iContUserID >= 0)
            {
                if (SocialNetwork == 0)
                {
                    if (iGroupAnswerID >= 0)
                    {
                        /*
                        try
                        {
                            
                            var msgsReceived = api.Wall.GetComments(-iGroupAnswerID, iGroupAnswerPostID, out totalCount, null, false, 300, null, 0, 0);
                            listBoxUserMessages.Items.Clear();
                            List<String> lstMsgToAdd = new List<string>();
                            foreach (VkNet.Model.Comment msg in msgsReceived)
                            {
                                String value = "";
                                value = "0|";
                                value = value + msg.Date.Value.ToShortDateString() + "|";
                                value = value + msg.Date.Value.ToShortTimeString() + "|";
                                value = value + NilsaUtils.TextToString(msg.Text);
                                //listBoxUserMessages.Items.Add(((msg.Type.HasValue && msg.Type.Value == VkNet.Enums.MessageType.Received) ? "<- " : "-> ") + msg.Date.Value.ToShortDateString() + " " + msg.Date.Value.ToShortTimeString() + " - " + msg.Body);
                                //lstUserMessages.Add(value);
                                listBoxUserMessages.Items.Add(((msg.FromId != iPersUserID) ? "<- " : "-> ") + msg.Date.Value.ToShortDateString() + " " + msg.Date.Value.ToShortTimeString() + " - " + msg.Text);
                                lstUserMessages.Add(value);
                            }
                        }
                        catch
                        {

                        }
                        */
                    }
                    else
                    {
                        //File.AppendAllText(Path.Combine(sDataPath, "chat_" + getSocialNetworkPrefix() + _iPersUserID.ToString() + "_" + Convert.ToString(_iContUserID) + ".txt"), (inboundMessage ? "0" : "1") + "|" + _date + "|" + _time + "|" + _text + Environment.NewLine);
                        List<String> lstHistory = new List<String>();
                        listBoxUserMessages.Items.Clear();
                        if (File.Exists(Path.Combine(sDataPath, "chat_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(iContUserID) + ".txt")))
                        {
                            try
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "chat_" + getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + Convert.ToString(iContUserID) + ".txt"));
                                lstHistory = new List<String>(srcFile);
                            }
                            catch (Exception e)
                            {
                                ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                            }
                        }

                        clearTrashList(lstHistory);
                        foreach (String str in lstHistory)
                        {
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

                            listBoxUserMessages.Items.Add((inboundMessage ? "<- " : "-> ") + dateStr + " " + timeStr + " - " + bodyStr);
                            lstUserMessages.Add(value);
                        }
                        /*
                        try
                        {
                            var msgsReceived = api_Messages_GetHistory(iContUserID, false, out totalCount, null, 200);
                            listBoxUserMessages.Items.Clear();
                            foreach (VkNet.Model.Message msg in msgsReceived)
                            {
                                String value = "";
                                value = Convert.ToString(msg.Id.HasValue ? msg.Id.Value : 0) + "|";
                                value = value + msg.Date.Value.ToShortDateString() + "|";
                                value = value + msg.Date.Value.ToShortTimeString() + "|";
                                value = value + NilsaUtils.TextToString(msg.Body);
                                listBoxUserMessages.Items.Add(((msg.Type.HasValue && msg.Type.Value == VkNet.Enums.MessageType.Received) ? "<- " : "-> ") + msg.Date.Value.ToShortDateString() + " " + msg.Date.Value.ToShortTimeString() + " - " + msg.Body);
                                lstUserMessages.Add(value);
                            }
                        }
                        catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                        {
                            ReAutorize(userLogin, userPassword);
                        }
                        catch (System.Net.WebException)
                        {
                            ReAutorize(userLogin, userPassword);
                        }
                        catch (VkNet.Exception.VkApiException vkapexeption)
                        {
                            ReAutorize(userLogin, userPassword);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("ReadAllUserMessages", iContUserID.ToString(), e);
                        }
                        finally { }
                        */
                    }
                }
                else if (SocialNetwork == 1)
                {
                    listBoxUserMessages.Items.Clear();
                    NILSA_LoadMessagesDB();
                    String sTo0 = iPersUserID.ToString() + "|0|" + iContUserID.ToString() + "|";
                    String sTo1 = iContUserID.ToString() + "|0|" + iPersUserID.ToString() + "|";
                    String sTo2 = iPersUserID.ToString() + "|1|" + iContUserID.ToString() + "|";
                    String sTo3 = iContUserID.ToString() + "|1|" + iPersUserID.ToString() + "|";

                    List<String> lstMsgToAdd = new List<string>();
                    for (int i = 0; i < lstNILSA_MessagesDB.Count; i++)
                    {
                        String srec = lstNILSA_MessagesDB[i];
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

                            //listBoxUserMessages.Items.Add(((srec.StartsWith(sTo0) || srec.StartsWith(sTo2)) ? "<- " : "-> ") + new DateTime(date).ToShortDateString() + " " + new DateTime(date).ToShortTimeString() + " - " + NILSA_GetFieldFromStringRec(srec, 4));
                            //lstUserMessages.Add(value);
                            listBoxUserMessages.Items.Insert(0, ((srec.StartsWith(sTo0) || srec.StartsWith(sTo2)) ? "<- " : "-> ") + new DateTime(date).ToShortDateString() + " " + new DateTime(date).ToShortTimeString() + " - " + NILSA_GetFieldFromStringRec(srec, 4));
                            lstUserMessages.Insert(0, value);
                        }
                    }
                }

                if (listBoxUserMessages.Items.Count > 0)
                {
                    listBoxUserMessages.TopIndex = listBoxUserMessages.Items.Count - 1;

                    //int visibleItems = listBoxUserMessages.ClientSize.Height / listBoxUserMessages.ItemHeight;
                    //listBoxUserMessages.TopIndex = Math.Max(listBoxUserMessages.Items.Count - visibleItems + 1, 0);
                }
            }
            else
                listBoxUserMessages.Items.Clear();
            Cursor = Cursors.Arrow;
        }

        private void listBoxUserMessagesValueChanged()
        {
            //--- Перестройка остальных полей
            //timerWriteMessages.Enabled = false;
            //timerSkipMessage.Enabled = false;
            //Set_pbSkipMessage_Default();
            timerWriteMessagesOff();

            listBoxInMsg.Items.Clear();
            listBoxOutMsg.Items.Clear();
            //buttonEditInEqMsgHar.Enabled = false;
            //buttonEditOutEqMsgHar.Enabled = false;
            tbSendOutMessage.Enabled = false;
            Set_labelInEqMsgHarTitleValue_Text("");
            CompareVetors_RestoreDefaultValues();
            clearlblEQInHarValues();
            //labelInEqMsgHar2Value.Text = "";
            //labelInEqMsgHar3Value.Text = "";
            //labelInEqMsgHar4Value.Text = "";
            //cbEQOutMsg.SelectedIndex = -1;
            Set_labelOutEqMsgHarTitleValue_Text("");

            clearlblEQOutHarValues();
            //labelOutEqMsgHar2Value.Text = "";
            //labelOutEqMsgHar3Value.Text = "";
            //labelOutEqMsgHar4Value.Text = "";

            sCurrentEQInMessageRecord = "";
            sCurrentEQOutMessageRecord = "";
            sCurrentEQOutMessageRecordOut = "";

            //TimerSkipMessageCycle = DefaultTimerSkipMessageCycle;

            //if (bServiceStart)
            //{
            //    Set_pbSkipMessage_Value();
            //    timerSkipMessage.Enabled = true;
            //}

            SetEQInMessageList(labelInMsgHarTitleValue_Text);
        }

        private void listBoxUserMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxUserMessages.SelectedIndex >= 0)
            {
                String value = listBoxUserMessages.Items[listBoxUserMessages.SelectedIndex].ToString();
                value = value.Substring(value.IndexOf(" ") + 1);
                //labelInMsgHarDateValue.Text = value.Substring(0, value.IndexOf(" "));
                value = value.Substring(value.IndexOf(" ") + 1);
                //labelInMsgHarTimeValue.Text = value.Substring(0, value.IndexOf(" "));
                value = value.Substring(value.IndexOf(" ") + 3);
                Set_labelInMsgHarTitleValue_Text(NilsaUtils.StringToText(value));
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);

                listBoxUserMessagesValueChanged();
            }
        }

        private void StopAnswerTimer()
        {
            tbStopService_Click(null, null);
        }

        private void StartAnswerTimer()
        {
            //tbRefreshPull_Click(null, null);
        }

        public void SaveMessagesDBList()
        {
            List<String> lstList = new List<String>();
            foreach (var pair in lstMessagesDB)
            {
                lstList.Add(pair.Value.ToString() + "|" + pair.Key);
            }
            FileWriteAllLines(Path.Combine(sDataPath, "_messages_db.txt"), lstList, Encoding.UTF8);
            lstList.Clear();
            lstList.Add(strDB0Name);
            FileWriteAllLines(Path.Combine(sDataPath, "_default_db_name.txt"), lstList, Encoding.UTF8);
        }

        public void LoadMessagesDBList()
        {
            List<String> lstList = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_default_db_name.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_default_db_name.txt"));
                    lstList = new List<String>(srcFile);
                    strDB0Name = lstList[0];
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    strDB0Name = "BASIC";
                }
            }

            lstList.Clear();
            if (File.Exists(Path.Combine(sDataPath, "_messages_db.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_messages_db.txt"));
                    lstList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstList.Clear();
                    lstList.Add("0|" + strDB0Name);
                }
            }
            else
            {
                lstList.Add("0|" + strDB0Name);
            }

            iMessagesDBMaxID = -1;
            lstMessagesDB = new Dictionary<string, int>();
            foreach (String str in lstList)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                int iID = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                String sName = str.Substring(str.IndexOf("|") + 1);

                if (iID > iMessagesDBMaxID)
                    iMessagesDBMaxID = iID;
                if (!lstMessagesDB.ContainsKey(sName))
                    lstMessagesDB.Add(sName, iID);
            }
        }

        public String getMessagesDBThemeID(int idxHar, String sName)
        {
            if (idxHar == 0)
                return "";

            int iID = -1;
            if (lstMessagesDB.ContainsKey(sName))
            {
                iID = lstMessagesDB[sName];
                return iID.ToString() + "_";
            }
            return "";
        }

        public void SaveEQInMessageDB(bool bReadLists = true, bool bUpdateParent = false)
        {
            Dictionary<int, List<String>> lstDBs = new Dictionary<int, List<String>>();
            foreach (var pair in lstMessagesDB)
            {
                lstDBs.Add(pair.Value, new List<String>());
            }

            bool bAdded = false;
            foreach (String str in lstEQInMessagesDB)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                if (str.IndexOf("|") <= 0)
                    continue;

                String sName = str.Substring(str.IndexOf("|") + 1);
                sName = sName.Substring(0, sName.IndexOf("|"));
                if (lstMessagesDB.ContainsKey(sName))
                {
                    int iID = lstMessagesDB[sName];
                    lstDBs[iID].Add(str);
                }
                else
                {
                    iMessagesDBMaxID++;
                    lstMessagesDB.Add(sName, iMessagesDBMaxID);
                    lstDBs.Add(iMessagesDBMaxID, new List<String>());
                    lstDBs[iMessagesDBMaxID].Add(str);
                    //lstMessagesDBUserList.Add(iMessagesDBMaxID.ToString());
                    bAdded = true;
                }
            }
            foreach (var pair in lstDBs)
            {
                if (!lstMessagesDBUserList.Contains(pair.Key.ToString()))
                {
                    FileWriteAllLines(Path.Combine(sDataPath, "_eqinmsgdb_" + pair.Key.ToString() + ".txt"), pair.Value, Encoding.UTF8);
                    SaveMsgHarValuesFromDB(pair.Key.ToString(), pair.Value, bReadLists, bUpdateParent);
                }
            }

            if (bAdded)
            {
                SaveMessagesDBList();
                //MessagesDB_SaveUserDBs();
            }
        }

        private void SaveMsgHarValuesFromDB(String sDBID, List<String> lstMsgToValues, bool bReadLists, bool bUpdateParent)
        {
            List<String>[] lstValues = new List<String>[iMsgHarCount - 1];
            List<String>[] lstValuesAll = new List<String>[iMsgHarCount];

            for (int i = 0; i < iMsgHarCount - 1; i++)
            {
                lstValues[i] = new List<string>();
                if (bReadLists)
                {
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + sDBID + "_" + (i + 2).ToString() + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + sDBID + "_" + (i + 2).ToString() + ".txt"));
                            lstValues[i] = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                            lstValues[i] = new List<String>();
                        }
                    }
                }
            }

            if (bUpdateParent)
            {
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    lstValuesAll[i] = new List<string>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + (i + 1).ToString() + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + (i + 1).ToString() + ".txt"));
                            lstValuesAll[i] = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                            lstValuesAll[i] = new List<String>();
                        }
                    }
                }
            }

            for (int j = 0; j < lstMsgToValues.Count; j++)
            {
                String value = lstMsgToValues[j];

                if (value.Length == 0)
                    continue;

                for (int i = 0; i < iMsgHarCount; i++)
                {
                    value = value.Substring(value.IndexOf("|") + 1);
                    if (value.IndexOf("|") <= 0)
                        break;
                    String s = value.Substring(0, value.IndexOf("|"));
                    if (i > 0)
                    {
                        if (s.Length > 0)
                            if (!lstValues[i - 1].Contains(s))
                            {
                                lstValues[i - 1].Add(s);
                            }
                    }

                    if (bUpdateParent)
                    {
                        if (s.Length > 0)
                            if (!lstValuesAll[i].Contains(s))
                            {
                                lstValuesAll[i].Add(s);
                            }
                    }
                }
            }

            for (int i = 0; i < iMsgHarCount - 1; i++)
            {
                if (lstValues[i].Count > 0)
                    FileWriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + sDBID + "_" + (i + 2).ToString() + ".txt"), lstValues[i], Encoding.UTF8);
                else
                    File.Delete(Path.Combine(FormMain.sDataPath, "_msg_har_" + sDBID + "_" + (i + 2).ToString() + ".txt"));
            }

            if (bUpdateParent)
            {
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    if (lstValuesAll[i].Count > 0)
                        FileWriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + (i + 1).ToString() + ".txt"), lstValuesAll[i], Encoding.UTF8);
                }
            }
        }

        public void SaveEQOutMessageDB(bool bUpdateParent = false)
        {
            Dictionary<int, List<String>> lstDBs = new Dictionary<int, List<String>>();
            foreach (var pair in lstMessagesDB)
            {
                lstDBs.Add(pair.Value, new List<String>());
            }
            bool bAdded = false;
            foreach (String str in lstEQOutMessagesDB)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                if (str.IndexOf("|") <= 0)
                    continue;

                String sName = str.Substring(str.IndexOf("|") + 1);
                sName = sName.Substring(0, sName.IndexOf("|"));
                if (lstMessagesDB.ContainsKey(sName))
                {
                    int iID = lstMessagesDB[sName];
                    lstDBs[iID].Add(str);
                }
                else
                {
                    iMessagesDBMaxID++;
                    lstMessagesDB.Add(sName, iMessagesDBMaxID);
                    lstDBs.Add(iMessagesDBMaxID, new List<String>());
                    lstDBs[iMessagesDBMaxID].Add(str);
                    //lstMessagesDBUserList.Add(iMessagesDBMaxID.ToString());
                    bAdded = true;
                }
            }
            foreach (var pair in lstDBs)
            {
                if (!lstMessagesDBUserList.Contains(pair.Key.ToString()))
                {
                    FileWriteAllLines(Path.Combine(sDataPath, "_eqoutmsgdb_" + pair.Key.ToString() + ".txt"), pair.Value, Encoding.UTF8);
                    SaveMsgHarValuesFromDB(pair.Key.ToString(), pair.Value, true, bUpdateParent);
                }
            }

            if (bAdded)
            {
                SaveMessagesDBList();
                //MessagesDB_SaveUserDBs();
            }
        }

        private void MessagesDB_SaveUserDBs()
        {
            FileWriteAllLines(Path.Combine(sDataPath, "_messagesdb_disabled_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstMessagesDBUserList, Encoding.UTF8);
        }

        private void MessagesDB_LoadUserDBs()
        {
            lstMessagesDBUserList = new List<String>();
            List<String> _lstMessagesDBUserList = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_messagesdb_disabled_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_messagesdb_disabled_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));
                    _lstMessagesDBUserList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstMessagesDBUserList = new List<String>();
                }
            }

            bool bNeedSave = false;
            foreach (string str in _lstMessagesDBUserList)
            {
                if (str.Length > 0)
                {
                    if (lstMessagesDB.ContainsValue(Convert.ToInt32(str)))
                        lstMessagesDBUserList.Add(str);
                    else
                        bNeedSave = true;
                }
                else
                    bNeedSave = true;
            }
            if (bNeedSave)
                MessagesDB_SaveUserDBs();
        }

        private bool LoadEQInMessageDB()
        {
            bool bCorrect = false;
            lstEQInMessagesDB = new List<String>();
            hashsetEQInMessagesDB = new HashSet<string>();
            foreach (var pair in lstMessagesDB)
            {
                String sUDBN = pair.Value.ToString();
                if (!lstMessagesDBUserList.Contains(sUDBN))
                {
                    if (File.Exists(Path.Combine(sDataPath, "_eqinmsgdb_" + sUDBN + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_eqinmsgdb_" + sUDBN + ".txt"));
                            List<String> lst = new List<String>(srcFile);

                            foreach (String str in lst)
                            {
                                if (str == null)
                                    continue;

                                if (str.Length == 0)
                                    continue;

                                int iMarkerIdx = str.IndexOf("|!*#0");
                                if (iMarkerIdx > 0 && iMarkerIdx + 5 >= str.Length)
                                {
                                    bCorrect = true;
                                    lstEQInMessagesDB.Add(str.Substring(0, str.IndexOf("|!*#0")));
                                    hashsetEQInMessagesDB.Add(str.Substring(0, str.IndexOf("|!*#0")));
                                }
                                else
                                {
                                    lstEQInMessagesDB.Add(str);
                                    hashsetEQInMessagesDB.Add(str);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        }
                    }
                }
            }
            return bCorrect;
        }

        private bool LoadEQOutMessageDB()
        {
            bool bCorrect = false;
            lstEQOutMessagesDB = new List<String>();
            hashsetEQOutMessagesDB = new HashSet<string>();
            foreach (var pair in lstMessagesDB)
            {

                String sUDBN = pair.Value.ToString();
                if (!lstMessagesDBUserList.Contains(sUDBN))
                {
                    if (File.Exists(Path.Combine(sDataPath, "_eqoutmsgdb_" + sUDBN + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_eqoutmsgdb_" + sUDBN + ".txt"));
                            List<String> lst = new List<String>(srcFile);

                            foreach (String str in lst)
                            {
                                if (str == null)
                                    continue;

                                if (str.Length == 0)
                                    continue;

                                int iMarkerIdx = str.IndexOf("|!*#0");
                                if (iMarkerIdx > 0 && iMarkerIdx + 5 >= str.Length)
                                {
                                    bCorrect = true;
                                    lstEQOutMessagesDB.Add(str.Substring(0, str.IndexOf("|!*#0")));
                                    hashsetEQOutMessagesDB.Add(str.Substring(0, str.IndexOf("|!*#0")));
                                }
                                else
                                {
                                    lstEQOutMessagesDB.Add(str);
                                    hashsetEQOutMessagesDB.Add(str);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        }
                    }
                }
            }
            return bCorrect;
        }

        private void SetEQInMessageParametersDefaultValues()
        {
            if (sCurrentEQInMessageRecord.Length > 0)
            {
                String value = sCurrentEQInMessageRecord;
                // Установка параметров по умолчанию
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    value = value.Substring(value.IndexOf("|") + 1);
                    String s = value.Substring(0, value.IndexOf("|"));
                    sMsgHar[i, 3] = s; // Значение атрибутов
                }
            }
        }

        private void SetEQOutMessageParametersDefaultValues()
        {
            if (sCurrentEQOutMessageRecord.Length > 0)
            {
                String value = sCurrentEQOutMessageRecord;
                // Установка параметров по умолчанию
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    value = value.Substring(value.IndexOf("|") + 1);
                    String s = value.Substring(0, value.IndexOf("|"));
                    sMsgHar[i, 3] = s; // Значение атрибутов
                }
            }
        }


        private void SetCurrentContactLastMessageFieldArray()
        {
            sCurrentContactLastMessageFieldArray = new String[iMsgHarCount];
            for (int i = 0; i < iMsgHarCount; i++)
                sCurrentContactLastMessageFieldArray[i] = "";

            if (sCurrentContactLastMessageRecord.Length > 0)
            {
                String value = sCurrentContactLastMessageRecord;
                // Установка параметров по умолчанию
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    value = value.Substring(value.IndexOf("|") + 1);
                    String s = value.Substring(0, value.IndexOf("|"));
                    sCurrentContactLastMessageFieldArray[i] = s; // Значение атрибутов
                }
            }
        }

        private String ChangeMessageParametersValues(String sMsgRecord, int iHarIdx, String sHarValue)
        {
            String value = sMsgRecord;
            String retval = value.Substring(0, value.IndexOf("|") + 1);

            for (int i = 0; i < iMsgHarCount; i++)
            {
                value = value.Substring(value.IndexOf("|") + 1);
                String s = value.Substring(0, value.IndexOf("|"));
                if (i == iHarIdx)
                {
                    s = sHarValue;
                }

                sMsgHar[i, 3] = s;
                retval += sMsgHar[i, 3] + "|";
            }
            retval += value.Substring(value.IndexOf("|") + 1);

            return retval;
        }

        private String[] getLastMessageVector()
        {
            String[] sQV = new String[iMsgHarCount];
            for (int i = 0; i < iMsgHarCount; i++)
                sQV[i] = "";
            if (sCurrentContactLastMessageFieldArray != null && sCurrentContactLastMessageFieldArray.Length > 0)
            {
                for (int i = 0; i < iMsgHarCount; i++)
                {

                    String s = "";
                    if (sCurrentContactLastMessageFieldArray[i].Length > 0)
                        s = sCurrentContactLastMessageFieldArray[i];

                    if (i == 0 /*&& s.Length == 0*/)
                    {
                        if (!adbrCurrent.bMsgReplaceValue[i] || s.Length == 0)
                            s = adbrCurrent.DefaultAlgorithmTheme;
                    }
                    //if (iCompareVectorsKoef[iv] == 1111)
                    //    sInVal = adbrCurrent.DefaultAlgorithmTheme;

                    if (sMsgHar[i, 2].IndexOf("*") > 0)
                    {
                        String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("*") + 1);
                        for (int j = 0; j < iContHarCount; j++)
                        {
                            if (sContHar[j, 0].Equals(s1))
                            {
                                s = GetContactParametersValue(sContHar[j, 0]);
                                break;
                            }
                        }
                    }
                    else if (sMsgHar[i, 2].IndexOf("#") > 0)
                    {
                        String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("#") + 1);
                        for (int j = 0; j < iPersHarCount; j++)
                        {
                            if (sPersHar[j, 0].Equals(s1))
                            {
                                s = GetPersoneParametersValue(sPersHar[j, 0]);
                                break;
                            }
                        }
                    }
                    sQV[i] = s.ToLower();
                }
            }
            return sQV;

        }

        private void GenerateInMessagesFromMarker(String sMarkerInMessages)
        {
            if (sMarkerInMessages.Length == 0 || iContUserID == -1)
                return;

            //int iMID = 0;
            String[] msgSentences = sMarkerInMessages.Split('+');
            foreach (string msg in msgSentences)
            {
                if (msg.Length > 0)
                {
                    DateTime dt = DateTime.Now;
                    String sCurRec = "0|" + getContUserIDWithGroupID() + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + makeTextVariants(msg);
                    lstReceivedMessages.Add(sCurRec);
                    //iMID++;
                }
            }
        }

        private void SetEQInMessageParametersValues(String sMsgRecord, bool bSetAlgNotChanged = true)
        {
            timerWriteMessagesOff();
            //timerWriteMessages.Enabled = false;
            //timerSkipMessage.Enabled = false;

            SetCurrentContactLastMessageFieldArray();

            String value = sMsgRecord;
            sCurrentEQInMessageRecord = sMsgRecord.Substring(0, sMsgRecord.IndexOf("|") + 1); // sMsgRecord;
            // Установка параметров по умолчанию
            String sEQINMessageTheme = "";
            for (int i = 0; i < iMsgHarCount; i++)
            {
                value = value.Substring(value.IndexOf("|") + 1);
                String s = value.Substring(0, value.IndexOf("|"));

                if (bSetAlgNotChanged)
                {
                    if (adbrCurrent.ID >= 0)
                    {
                        if (adbrCurrent.bMsgReplaceValue[i])
                        {
                            if (sCurrentContactLastMessageFieldArray[i].Length > 0)
                                s = sCurrentContactLastMessageFieldArray[i];
                        }
                    }
                }

                if (sMsgHar[i, 2].IndexOf("*") > 0)
                {
                    String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("*") + 1);
                    for (int j = 0; j < iContHarCount; j++)
                    {
                        if (sContHar[j, 0].Equals(s1))
                        {
                            s = GetContactParametersValue(sContHar[j, 0]);
                            break;
                        }
                    }
                }
                else if (sMsgHar[i, 2].IndexOf("#") > 0)
                {
                    String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("#") + 1);
                    for (int j = 0; j < iPersHarCount; j++)
                    {
                        if (sPersHar[j, 0].Equals(s1))
                        {
                            s = GetPersoneParametersValue(sPersHar[j, 0]);
                            break;
                        }
                    }
                }

                sMsgHar[i, 3] = s;
                sCurrentEQInMessageRecord += sMsgHar[i, 3] + "|";

                if (i == 0)
                    sEQINMessageTheme = s;
            }
            sCurrentEQInMessageRecord += value.Substring(value.IndexOf("|") + 1);
            value = value.Substring(value.IndexOf("|") + 3); // Текст

            if (bSetAlgNotChanged)
            {
                int iMarkerIdx = value.IndexOf("|!*#0");
                if ((iMarkerIdx > 0 && iMarkerIdx + 5 < value.Length)/* && bSetAlgNotChanged*/)
                {
                    UndoMarkerChanges();

                    int oldAlgorithmID = adbrCurrent.ID;
                    bool bThemeChanged = false;
                    if (/*bSetAlgNotChanged && */adbrCurrent.ChangeThemeAlgorithmINMessageTheme)
                    {
                        if (!sEQINMessageTheme.Equals(adbrCurrent.Name))
                        {
                            int iMsgAlg = SearchAlgorithmsDBList(sEQINMessageTheme);
                            if (iMsgAlg >= 0)
                            {
                                bThemeChanged = true;
                                applyAlgorithm(iMsgAlg);
                            }
                        }
                    }

                    int sMarker = Convert.ToInt32(value.Substring(value.IndexOf("|!*#0") + 5));

                    if (adbrCurrent.ID >= 0)
                    {
                        if (adbrCurrent.PlayMarker)
                        {
                            Stream str = Properties.Resources._1;
                            SoundPlayer snd = new SoundPlayer(str);
                            snd.Play();
                        }

                        Boolean bAlgChanged = false;
                        if (adbrCurrent.iMarkerAlgorithmsID[sMarker - 1] >= 0)
                        {
                            ChangeContacterAlgorithm(adbrCurrent.iMarkerAlgorithmsID[sMarker - 1]);
                            if (adbrCurrent.ID != adbrCurrent.iMarkerAlgorithmsID[sMarker - 1])
                                bAlgChanged = true;
                        }

                        if (adbrCurrent.iMarkerContHarID[sMarker - 1] > 0)
                        {
                            if (lstContHarValues.Count > 0)
                            {
                                String oldrec = lstContHarValues[adbrCurrent.iMarkerContHarID[sMarker - 1] - 1];
                                lstContHarValues[adbrCurrent.iMarkerContHarID[sMarker - 1] - 1] = oldrec.Substring(0, oldrec.IndexOf("|") + 1) + adbrCurrent.sMarkerContHarValues[sMarker - 1];
                                SaveContactParamersValues();
                                LoadContactParametersDescription();
                            }
                        }
                        if (adbrCurrent.iMarkerMsgHarID[sMarker - 1] > 0)
                        {
                            sCurrentEQInMessageRecord = ChangeMessageParametersValues(sCurrentEQInMessageRecord, adbrCurrent.iMarkerMsgHarID[sMarker - 1] - 1, adbrCurrent.sMarkerMsgHarValues[sMarker - 1]);
                        }

                        GenerateInMessagesFromMarker(adbrCurrent.sMarkerInMessages[sMarker - 1]);

                        if (bAlgChanged)
                        {
                            LoadAlgorithmSettingsContacter();
                            UpdateContactParametersValues_Algorithm();
                            SetEQInMessageParametersValues(sCurrentEQInMessageRecord, false);
                            return;
                        }
                    }
                }
                else
                {
                    int oldAlgorithmID = adbrCurrent.ID;
                    bool bThemeChanged = false;
                    if (/*bSetAlgNotChanged && */adbrCurrent.ChangeThemeAlgorithmINMessageTheme)
                    {
                        if (!sEQINMessageTheme.Equals(adbrCurrent.Name))
                        {
                            int iMsgAlg = SearchAlgorithmsDBList(sEQINMessageTheme);
                            if (iMsgAlg >= 0)
                            {
                                bThemeChanged = true;
                                applyAlgorithm(iMsgAlg);
                            }
                        }
                    }
                }
            }

            Set_labelInEqMsgHarTitleValue_Text(NilsaUtils.StringToText(value));
            CompareVetors_RestoreDefaultValues();

            for (int i = 0; i < iMsgHarCount; i++)
            {
                lblEQInHarValues[i].Text = sMsgHar[i, 3];
                toolTipMessage.SetToolTip(lblEQInHarValues[i], sMsgHar[i, 3]);
            }
            //buttonEditInEqMsgHar.Enabled = true;

            tbSendOutMessage.Enabled = false;
            tbSkipMessage.Enabled = true;
            tbDeleteMessage.Enabled = true;

            //timerWriteMessages.Enabled = false;
            //timerSkipMessage.Enabled = false;
            //Set_pbSkipMessage_Default();
            //timerWriteMessagesOff();

            //TimerSkipMessageCycle = DefaultTimerSkipMessageCycle;
            //if (bServiceStart)
            //{
            //    Set_pbSkipMessage_Value();
            //    timerSkipMessage.Enabled = true;
            //}

            timerWriteMessagesOn();
            SetEQOutMessageList(sCurrentEQInMessageRecord);

        }

        private int SearchAlgorithmsDBList(String strName)
        {
            String queryName = strName.ToLower();

            List<String> lstList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"));
                    lstList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstList = new List<String>();
                }
            }

            foreach (String value in lstList)
            {
                AlgorithmsDBRecord dbr = AlgorithmsDBRecord.FromRecordString(value);

                if (dbr.Name.ToLower().Equals(queryName))
                    return dbr.ID;
            }

            return -1;
        }

        private bool isAlgorithmExists(int algid)
        {
            String queryName = algid.ToString() + "|";

            List<String> lstList = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_algorithms_db.txt"));
                    lstList = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstList = new List<String>();
                }
            }

            foreach (String value in lstList)
            {
                if (value.StartsWith(queryName))
                    return true;
            }

            return false;
        }

        private void ApplyEQOutMessageMarker()
        {
            if (sCurrentEQOutMessageRecord.Length == 0)
                return;

            String value = sCurrentEQOutMessageRecord;
            // Установка параметров по умолчанию
            for (int i = 0; i < iMsgHarCount; i++)
            {
                value = value.Substring(value.IndexOf("|") + 1);
                String s = value.Substring(0, value.IndexOf("|"));
                sMsgHar[i, 3] = s; // Значение атрибутов
            }
            value = value.Substring(value.IndexOf("|") + 3); // Текст

            int iMarkerIdx = value.IndexOf("|!*#0");
            if (iMarkerIdx > 0 && iMarkerIdx + 5 < value.Length)
            {
                int sMarker = 8 + Convert.ToInt32(value.Substring(value.IndexOf("|!*#0") + 5));
                if (adbrCurrent.ID >= 0)
                {
                    if (adbrCurrent.PlayMarker)
                    {
                        Stream str = Properties.Resources._1;
                        SoundPlayer snd = new SoundPlayer(str);
                        snd.Play();
                    }

                    if (adbrCurrent.iMarkerAlgorithmsID[sMarker - 1] >= 0)
                        ChangeContacterAlgorithm(adbrCurrent.iMarkerAlgorithmsID[sMarker - 1]);

                    if (adbrCurrent.iMarkerContHarID[sMarker - 1] > 0)
                    {
                        if (lstContHarValues.Count > 0)
                        {
                            String oldrec = lstContHarValues[adbrCurrent.iMarkerContHarID[sMarker - 1] - 1];
                            lstContHarValues[adbrCurrent.iMarkerContHarID[sMarker - 1] - 1] = oldrec.Substring(0, oldrec.IndexOf("|") + 1) + adbrCurrent.sMarkerContHarValues[sMarker - 1];
                            SaveContactParamersValues();
                            LoadContactParametersDescription();
                        }
                    }

                    if (adbrCurrent.iMarkerMsgHarID[sMarker - 1] > 0)
                    {
                        sCurrentEQOutMessageRecord = ChangeMessageParametersValues(sCurrentEQOutMessageRecord, adbrCurrent.iMarkerMsgHarID[sMarker - 1] - 1, adbrCurrent.sMarkerMsgHarValues[sMarker - 1]);
                    }

                    GenerateInMessagesFromMarker(adbrCurrent.sMarkerInMessages[sMarker - 1]);
                }
            }

        }

        private void SetEQOutMessageParametersValues(String sMsgRecord)
        {
            String value = sMsgRecord;
            sCurrentEQOutMessageRecord = sMsgRecord;
            // Установка параметров по умолчанию
            for (int i = 0; i < iMsgHarCount; i++)
            {
                value = value.Substring(value.IndexOf("|") + 1);
                String s = value.Substring(0, value.IndexOf("|"));
                sMsgHar[i, 3] = s; // Значение атрибутов
            }
            value = value.Substring(value.IndexOf("|") + 3); // Текст

            Set_labelOutEqMsgHarTitleValue_Text(NilsaUtils.StringToText(value));
            for (int i = 0; i < iMsgHarCount; i++)
            {
                lblEQOutHarValues[i].Text = sMsgHar[i, 3];
                toolTipMessage.SetToolTip(lblEQOutHarValues[i], sMsgHar[i, 3]);

                SetMsgHarTrafficLight(i, lblEQInHarValues[i].Text.ToLower(), lblEQOutHarValues[i].Text.ToLower());
            }

            tbSendOutMessage.Enabled = true;
            //TimerSendAnswerCycle = DefaultTimerSendAnswerCycle;
            if (bUnknownGenerated)
            {
                //if (DefaultTimerSendAnswerCycle < DefaultTimerSkipMessageCycle)
                //    TimerSendAnswerCycle = DefaultTimerSkipMessageCycle;
                bUnknownGenerated = false;
            }
            //if (bServiceStart)
            //{
            //    Set_pbSendMessage_Value();
            //    timerWriteMessages.Enabled = true;
            //}
            //timerSkipMessage.Enabled = false;
            //Set_pbSkipMessage_Default();
            timerWriteMessagesOn();

        }

        static Color trafficlight_red_color = Color.FromArgb(255, 255, 0, 0);//Color.Red;
        static Color trafficlight_yellow_color = Color.FromArgb(255, 255, 255, 0);//Color.Yellow;
        static Color trafficlight_green_color = Color.FromArgb(255, 0, 150, 0);//Color.Green;
        public static SolidBrush brush_trafficlight_red_color = new SolidBrush(trafficlight_red_color);
        public static SolidBrush brush_trafficlight_yellow_color = new SolidBrush(trafficlight_yellow_color);
        public static SolidBrush brush_trafficlight_green_color = new SolidBrush(trafficlight_green_color);
        public static SolidBrush brush_trafficlight_off_color = new SolidBrush(SystemColors.Control);

        private void SetMsgHarTrafficLight(int iv, String sInMsgHar, String sOutMsgHar)
        {
            if (iCompareVectorsKoef[iv] == 1111 && sInMsgHar.Length > 0 && sOutMsgHar.Length == 0)
            {
                if (iv == 12 || iv == 13)
                {
                    lblMsgHarCompare[iv].Image = global_Nilsa_Properties_Resources_empty_image; //global::Nilsa.Properties.Resources.trafficlight_green_16;
                    lblMsgHarCompare[iv].BackColor = trafficlight_green_color;
                    toolTipMessage.SetToolTip(lblMsgHarCompare[iv], NilsaUtils.Dictonary_GetText(userInterface, "toolTips_4", this.Name, "Зеленый"));
                }
                else
                {
                    lblMsgHarCompare[iv].Image = global_Nilsa_Properties_Resources_empty_image; //global::Nilsa.Properties.Resources.trafficlight_red_16;
                    lblMsgHarCompare[iv].BackColor = trafficlight_red_color;
                    toolTipMessage.SetToolTip(lblMsgHarCompare[iv], NilsaUtils.Dictonary_GetText(userInterface, "toolTips_5", this.Name, "Красный"));
                }
                return;
            }

            if ("*".Equals(sOutMsgHar))
            {
                lblMsgHarCompare[iv].Image = global_Nilsa_Properties_Resources_empty_image; //global::Nilsa.Properties.Resources.trafficlight_green_16;
                lblMsgHarCompare[iv].BackColor = trafficlight_yellow_color;
                toolTipMessage.SetToolTip(lblMsgHarCompare[iv], "Коэф. совпадения: " + adbrCurrent.CompareStarLevel.ToString() + "%");
                return;
            }

            if (sInMsgHar.Length == 0 && sOutMsgHar.Length == 0)
            {
                lblMsgHarCompare[iv].Image = global_Nilsa_Properties_Resources_empty_image; //global::Nilsa.Properties.Resources.trafficlight_yellow_16;
                lblMsgHarCompare[iv].BackColor = trafficlight_yellow_color;
                toolTipMessage.SetToolTip(lblMsgHarCompare[iv], NilsaUtils.Dictonary_GetText(userInterface, "toolTips_6", this.Name, "Желтый"));
                return;
            }

            if (sInMsgHar.Length == 0 || sOutMsgHar.Length == 0)
            {
                lblMsgHarCompare[iv].Image = global_Nilsa_Properties_Resources_empty_image; //global::Nilsa.Properties.Resources.trafficlight_red_16;
                lblMsgHarCompare[iv].BackColor = trafficlight_red_color;
                toolTipMessage.SetToolTip(lblMsgHarCompare[iv], NilsaUtils.Dictonary_GetText(userInterface, "toolTips_5", this.Name, "Красный"));
                return;
            }

            String sInVal = sInMsgHar;
            if (adbrCurrent.ID >= 0)
            {
                if (adbrCurrent.iMsgHarTypes[iv] == 0)
                {
                    if (adbrCurrentDictPairs[iv].Keys.Contains(sInVal))
                        sInVal = adbrCurrentDictPairs[iv][sInVal].ToLower();
                }
                else if (adbrCurrent.iMsgHarTypes[iv] == 1)
                {
                    if (adbrCurrentDictPairs[iv].Count > 0)
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

            if (compareAttribute(sInVal, sOutMsgHar))
            {
                lblMsgHarCompare[iv].Image = global_Nilsa_Properties_Resources_empty_image; //global::Nilsa.Properties.Resources.trafficlight_green_16;
                lblMsgHarCompare[iv].BackColor = trafficlight_green_color;
                toolTipMessage.SetToolTip(lblMsgHarCompare[iv], NilsaUtils.Dictonary_GetText(userInterface, "toolTips_4", this.Name, "Зеленый"));
                return;
            }

            lblMsgHarCompare[iv].Image = global_Nilsa_Properties_Resources_empty_image; //global::Nilsa.Properties.Resources.trafficlight_red_16;
            lblMsgHarCompare[iv].BackColor = trafficlight_red_color;
            toolTipMessage.SetToolTip(lblMsgHarCompare[iv], NilsaUtils.Dictonary_GetText(userInterface, "toolTips_5", this.Name, "Красный"));
        }

        private String TextSearchFilter(String sSrc)
        {
            String sDst = NilsaUtils.TextToString(sSrc).Replace("<br>", " ");

            // TextSearchFilteredChars - Replace divider chars
            foreach (char ch in TextSearchFilteredChars)
                while (sDst.IndexOf(ch) >= 0)
                    sDst = sDst.Replace(ch, ' ');

            // TextSearchIgnoredWords - Remove ignored words
            sDst = " " + sDst + " ";
            String[] IW = TextSearchIgnoredWords.Split('|');
            foreach (String str in IW)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                while (sDst.IndexOf(" " + str + " ") >= 0)
                    sDst = sDst.Replace(" " + str + " ", " ");
            }

            // TextSearchMinWordsLen
            IW = sDst.Split(' ');
            sDst = "";
            foreach (String str in IW)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                if (str.Length >= TextSearchMinWordsLen)
                    sDst = sDst + " " + str;
            }

            if (sDst.Length > 0)
                return sDst.Substring(1);

            return "";
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

        private int TextSearchNew_WordCount(String sSrc)
        {
            int iCount = 0;

            int i = 0;
            int nLen = sSrc.Length;
            while (i < nLen)
            {
                while (i < nLen && !(Char.IsDigit(sSrc[i]) || Char.IsLetter(sSrc[i])))
                    i++;


                if (i < nLen && (Char.IsDigit(sSrc[i]) || Char.IsLetter(sSrc[i])))
                {
                    iCount++;
                    while (i < nLen && (Char.IsDigit(sSrc[i]) || Char.IsLetter(sSrc[i])))
                        i++;
                }

            }
            return iCount;
        }

        private String TextSearchFilterNew_Filter(String sSrc)
        {
            String sDst = NilsaUtils.TextToString(sSrc).Replace("<br>", "~").Trim();
            bool bNotExpr = true;
            int iPosS = sDst.IndexOf("{");
            int iPosE = sDst.IndexOf("}");
            if (iPosS >= 0 && iPosE > iPosS)
                bNotExpr = false;

            while (sDst.IndexOf("  ") >= 0)
                sDst = sDst.Replace("  ", " ");
            if (bNotExpr)
            {
                while (sDst.IndexOf("~ ") >= 0)
                    sDst = sDst.Replace("~ ", "~");
                while (sDst.IndexOf(" ~") >= 0)
                    sDst = sDst.Replace(" ~", "~");
            }
            int nLen = sDst.Length;
            if (nLen > 0 && sDst[nLen - 1] != '~')
                sDst += "~";
            return sDst;
        }

        private String GetMessageTextWOMarker(String sText)
        {

            if (sText.IndexOf("|!*#0") >= 0)
            {
                return sText.Substring(0, sText.IndexOf("|!*#0"));
            }
            return sText;
        }

        private String GetMessageTextWithMarker(String sMsgText)
        {
            if (sMsgText.IndexOf("|!*#0") >= 0)
            {
                sMsgText = sMsgText.Replace("|!*#0", " (" + NilsaUtils.Dictonary_GetText(userInterface, "textValues_10", this.Name, "Маркер") + " ");
                sMsgText += ")";
            }
            return sMsgText;
        }

        private void clearlblEQInHarValues()
        {
            for (int i = 0; i < iMsgHarCount; i++)
            {
                lblEQInHarValues[i].Text = "";
                toolTipMessage.SetToolTip(lblEQInHarValues[i], "");
            }
        }

        private HashSet<string> variantsInMsg;
        private HashSet<string> variantsInMsgWords;
        private void makeKeywordsSubvariants(String text)
        {
            string pattern = @"{[^{}]*}";
            string result = text;
            Regex newReg = new Regex(pattern);

            MatchCollection matches = newReg.Matches(result);
            if (matches.Count > 0)
            {
                Match mat = matches[0];
                string variant = mat.Value.Substring(1, mat.Value.Length - 2);
                String[] variants = new String[] { variant };
                if (variant.IndexOf("~") >= 0)
                    variants = variant.Split('~');
                string start = result.Substring(0, mat.Index);
                string end = result.Substring(mat.Index + mat.Length);
                foreach (string var in variants)
                    makeKeywordsSubvariants(start + var + end);
            }
            else
                variantsInMsg.Add(text);
        }

        private String[] makeEQInTextVariants(String sText)
        {
            int iPosS = sText.IndexOf("{");
            int iPosE = sText.IndexOf("}");
            if (iPosS >= 0 && iPosE > iPosS)
            {
                variantsInMsg = new HashSet<string>();
                variantsInMsgWords = new HashSet<string>();
                makeKeywordsSubvariants(sText);

                foreach (string str in variantsInMsg)
                {
                    if (str == null)
                        continue;

                    if (str.Length == 0)
                        continue;

                    String[] words = str.Split('~');
                    foreach (string word in words)
                        if (word.Length > 0)
                            variantsInMsgWords.Add(word);
                }
                return variantsInMsgWords.ToArray();
            }
            return sText.Split('~');
        }

        bool compareLexicalNewAlgorithm = false;
        private void SetEQInMessageList(String sRQ)
        {
            String sRQOriginal = sRQ;
            Set_labelInEqMsgHarTitleValue_Text("");
            CompareVetors_RestoreDefaultValues();
            clearlblEQInHarValues();
            listBoxInMsg.SelectedIndex = -1;
            listBoxInMsg.Items.Clear();
            //buttonEditInEqMsgHar.Enabled = false;
            Set_labelOutEqMsgHarTitleValue_Text("");

            //cbEQOutMsg.SelectedIndex = -1;
            clearlblEQOutHarValues();
            listBoxOutMsg.SelectedIndex = -1;
            listBoxOutMsg.Items.Clear();

            tbSendOutMessage.Enabled = false;
            tbSkipMessage.Enabled = true;
            tbDeleteMessage.Enabled = true;

            bUnknownGenerated = false;
            bUnknownMessageGenerated = false;
            //timerWriteMessages.Enabled = false;
            //timerSkipMessage.Enabled = false;
            //Set_pbSkipMessage_Default();
            //timerWriteMessagesOff();

            //TimerSkipMessageCycle = DefaultTimerSkipMessageCycle;
            //if (bServiceStart)
            //{
            //    Set_pbSkipMessage_Value();
            //    timerSkipMessage.Enabled = true;
            //}
            timerWriteMessagesOn();

            lstEQInMessagesList = new List<String>();

            sRQ = compareLexicalNewAlgorithm ? TextSearchFilterNew_Filter(GetMessageTextWOMarker(sRQ).ToLower()) : TextSearchFilter(GetMessageTextWOMarker(sRQ).ToLower());

            if (sRQ.Length > 0)
            {
                double dCompareLexicalLevel = comboBoxCompareLexicalLevel.SelectedIndex/* * 10*/;
                String[] RQV = compareLexicalNewAlgorithm ? null : sRQ.Split(' ');
                int RQVwc = compareLexicalNewAlgorithm ? TextSearchNew_WordCount(sRQ) : RQV.Length;

                String[] sLastMessageVector = getLastMessageVector();
                bool bLastMessageVector = false;
                for (int ilmv = 0; ilmv < sLastMessageVector.Length; ilmv++)
                {
                    if (sLastMessageVector[ilmv].Length > 0)
                    {
                        bLastMessageVector = true;
                        break;
                    }
                }

                foreach (String EQ in lstEQInMessagesDB)
                {
                    String sEQ = compareLexicalNewAlgorithm ? TextSearchFilterNew_Filter(EQ.Substring(EQ.IndexOf("|@!") + 3).ToLower()) : TextSearchFilter(GetMessageTextWOMarker(EQ.Substring(EQ.IndexOf("|@!") + 3)).ToLower());
                    if (sEQ.Length > 0)
                    {
                        double dW = 0;
                        if (compareLexicalNewAlgorithm)
                        {
                            double dWeqv = 0;
                            String[] EQV = makeEQInTextVariants(sEQ);// sEQ.Split('~');

                            foreach (String EQW in EQV)
                            {
                                if (TextSearchFilterNew_FindWord(sRQ, EQW))
                                    dWeqv += TextSearchNew_WordCount(EQW);
                            }
                            dW = Math.Round(dWeqv * 100 / RQVwc);
                        }
                        else
                        {
                            double dWrqv = 0;
                            double dWeqv = 0;
                            String[] EQV = sEQ.Split(' ');
                            int EQVwc = EQV.Length;

                            foreach (String RQW in RQV)
                                foreach (String EQW in EQV)
                                    if (RQW == EQW) { dWrqv += 1; break; }

                            foreach (String EQW in EQV)
                                foreach (String RQW in RQV)
                                    if (RQW == EQW) { dWeqv += 1; break; }

                            dW = Math.Round((dWrqv * dWeqv * 100.0) / (EQVwc * RQVwc));
                        }
                        if (dW > 0)
                        {
                            double dW1 = 100.0;
                            dWtheme = 100;
                            double dWthemeMax = getThemeKoefToLexicalVectorCompare();

                            if (bLastMessageVector) dW1 = Compare2Vector(sLastMessageVector, EQ, false);
                            if (adbrCurrent.UseSetKoefWithLexicalCompare)
                                dW *= dWtheme / 100;
                            String value = ((int)dW).ToString("000").Substring(0, 3);

                            //dW = dW * dW1 / 100.0;
                            //---
                            dW = 2 * dWtheme * dW * dW1 / (100.0 * dWthemeMax);

                            //---
                            if (dW > dCompareLexicalLevel)
                            {
                                String EQText = EQ.Substring(EQ.IndexOf("|") + 1);
                                EQText = EQText.Substring(0, EQText.IndexOf("|@!"));
                                String[] OQV = EQText.ToLower().Split('|');

                                value = ((int)dW).ToString("000").Substring(0, 3) + value + FormatMsgID(OQV[MSG_ID_COLUMN]) + "|" + EQ.Substring(EQ.IndexOf("|") + 1);
                                lstEQInMessagesList.Add(value);
                            }
                        }
                    }
                }
            }
            lstEQInMessagesList = lstEQInMessagesList.OrderByDescending(i => i).ToList();

            foreach (String EQ in lstEQInMessagesList)
            {
                String perc1 = Convert.ToInt32(EQ.Substring(0, 3)).ToString("00");
                String perc2 = Convert.ToInt32(EQ.Substring(3, 3)).ToString("00");
                if (compareLexicalNewAlgorithm)
                    listBoxInMsg.Items.Add(perc1 + "% (" + perc2 + "%) " + highlightKeywordsNew(TextSearchFilterNew_Filter(GetMessageTextWithMarker(EQ.Substring(EQ.IndexOf("|@!") + 3))), sRQ));
                else
                    listBoxInMsg.Items.Add(highlightKeywords(perc1 + "% (" + perc2 + "%) " + GetMessageTextWithMarker(EQ.Substring(EQ.IndexOf("|@!") + 3)), sRQ.Split(' ')));
            }

            if (lstEQInMessagesList.Count > 0)
            {
                listBoxInMsg.SelectedIndex = 0;
            }
            else
            {
                bool bUnknownNeedWorked = true;
                //---
                if (adbrCurrent.ID >= 0 && sCurrentContactLastMessageFieldArray != null && sCurrentContactLastMessageFieldArray.Length > 0)
                {
                    if (!adbrCurrent.bNotGenerateUnknownMessages)
                    {
                        bUnknownNeedWorked = false;
                        String sTmpMsgGenerated = "000000|";
                        for (int i = 0; i < iMsgHarCount; i++)
                        {
                            String s = "UNknown";
                            if (adbrCurrent.bMsgReplaceValue[i])
                            {
                                if (sCurrentContactLastMessageFieldArray[i].Length > 0)
                                    s = sCurrentContactLastMessageFieldArray[i];
                                else
                                {
                                    if (i == 0)
                                        s = adbrCurrent.DefaultAlgorithmTheme;
                                }
                            }
                            else
                            {
                                if (i == 0)
                                    s = adbrCurrent.DefaultAlgorithmTheme;
                            }

                            if (sMsgHar[i, 2].IndexOf("*") > 0)
                            {
                                String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("*") + 1);
                                for (int j = 0; j < iContHarCount; j++)
                                {
                                    if (sContHar[j, 0].Equals(s1))
                                    {
                                        s = GetContactParametersValue(sContHar[j, 0]);
                                        break;
                                    }
                                }
                            }
                            else if (sMsgHar[i, 2].IndexOf("#") > 0)
                            {
                                String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("#") + 1);
                                for (int j = 0; j < iPersHarCount; j++)
                                {
                                    if (sPersHar[j, 0].Equals(s1))
                                    {
                                        s = GetPersoneParametersValue(sPersHar[j, 0]);
                                        break;
                                    }
                                }
                            }

                            sTmpMsgGenerated += s + "|";
                        }
                        sTmpMsgGenerated += "@!" + sRQOriginal;

                        bUnknownGenerated = true;
                        bUnknownMessageGenerated = true;
                        lstEQInMessagesList.Add(sTmpMsgGenerated);
                        listBoxInMsg.Items.Add(Convert.ToInt32(sTmpMsgGenerated.Substring(0, sTmpMsgGenerated.IndexOf("|"))).ToString("00") + "% " + GetMessageTextWithMarker(sTmpMsgGenerated.Substring(sTmpMsgGenerated.IndexOf("|@!") + 3)));
                        listBoxInMsg.SelectedIndex = 0;
                    }
                }
                //---
                if (bUnknownNeedWorked)
                {
                    if (adbrCurrent.IgnoreUnknownMessagesWOUnknownGeneration)
                    {
                        SelectNextReceivedMessage();
                        return;
                    }
                }
            }
        }

        private String highlightKeywords(String text, String[] words)
        {
            String result = " " + text.ToLower();
            String search = " " + text.ToLower() + " ";
            foreach (char ch in TextSearchFilteredChars)
                while (search.IndexOf(ch) >= 0)
                    search = search.Replace(ch, ' ');

            foreach (String str in words)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                int i = 0;
                int si = 0;
                while ((si = search.IndexOf(" " + str + " ", i)) > 0)
                {
                    result = result.Substring(0, si + 1) + search.Substring(si + 1, str.Length).ToUpper() + result.Substring(si + str.Length + 1);
                    i = si + str.Length;
                }
            }

            return result.Substring(1);
        }

        private String highlightKeywordsNew(String text, String srctext)
        {

            String result = "";
            String[] EQV = makeEQInTextVariants(text.ToLower());// text.ToLower().Split('~');

            foreach (String EQW in EQV)
            {
                result += "~";
                if (TextSearchFilterNew_FindWord(srctext, EQW))
                    result += EQW.ToUpper();
                else
                    result += EQW;
            }

            return result.Substring(1);
        }

        private double getThemeKoefToLexicalVectorCompare()
        {
            switch (iCompareVectorsKoef[0])
            {
                case 1111: return 100;
                case 100: return 50;
                case 10: return 10;
                case 1: return 2;

            }
            return 1;
        }

        public static String[] IdealRQV = new String[0];
        private void SetEQMessageIdealParameters(String[] RQV)
        {
            IdealRQV = new String[RQV.Length];
            if (RQV.Length == 0)
                return;
            if (adbrCurrent.iMsgHarTypes.Length != RQV.Length)
                return;
            for (int iv = 0; iv < RQV.Length; iv++)
            {

                if (RQV[iv].Length == 0)
                {
                    IdealRQV[iv] = "";
                    continue;
                }

                String sInVal = RQV[iv];
                if (adbrCurrent.ID >= 0)
                {
                    if (adbrCurrent.iMsgHarTypes[iv] == 0)
                    {
                        if (adbrCurrentDictPairs.Length > iv)
                        {
                            if (adbrCurrentDictPairs[iv].Keys.Contains(sInVal.ToLower()))
                                sInVal = adbrCurrentDictPairs[iv][sInVal.ToLower()];
                        }
                    }
                    else if (adbrCurrent.iMsgHarTypes[iv] == 1)
                    {
                        if (adbrCurrentDictPairs.Length > iv)
                        {
                            if (adbrCurrentDictPairs[iv].Count > 0)
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
                }

                IdealRQV[iv] = sInVal;
            }
        }

        private double getWeight(String EQ8)
        {
            double retval = 1;

            if (EQ8.IndexOf('%') > 0)
            {
                string value = EQ8.Substring(0, EQ8.IndexOf('%'));
                try
                {
                    retval = ((double)Convert.ToInt32(value)) / 100.0;
                }
                catch
                {
                    retval = 1;
                }
            }

            return retval;
        }

        double dWtheme = 1;
        private double Compare2Vector(String[] RQV, String EQ, bool bWithAlgorithm)
        {
            double dW = 0;
            double dWMax = 0;
            dWtheme = 1;
            for (int i = 0; i < iMsgHarCount; i++)
                dWMax += (iCompareVectorsKoef[i] == 1111 ? ((i == 1 || i == 4 || i == 5) ? 0 : 100) : iCompareVectorsKoef[i]);

            if (dWMax > 0)
            {
                bool bSuccess = true;
                String EQText = EQ.Substring(EQ.IndexOf("|") + 1);
                EQText = EQText.Substring(0, EQText.IndexOf("|@!"));
                String[] OQV = EQText.ToLower().Split('|');

                for (int iv = 0; iv < RQV.Length; iv++)
                {
                    //if (iCompareVectorsKoef[iv] == 1111 && RQV[iv].Length > 0 && OQV[iv].Length == 0)
                    //    bSuccess = false;

                    if (iCompareVectorsKoef[iv] == 1111)
                    {
                        if (iv == 1 || iv == 4 || iv == 5)
                        {
                            dW += iCompareVectorsKoef[iv] == 1111 ? 0 : iCompareVectorsKoef[iv];
                            continue;
                        }
                    }

                    if (iCompareVectorsKoef[iv] == 1111 && RQV[iv].Length > 0 && OQV[iv].Length == 0)
                    {
                        if (iv == 12 || iv == 13)
                        {
                            dW += iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv];
                            continue;
                        }
                        else
                            bSuccess = false;
                    }


                    if ("*".Equals(OQV[iv]))
                    {
                        dW += (float)((iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]) * adbrCurrent.CompareStarLevel) / 100.0;
                        continue;
                    }

                    if (RQV[iv].Length == 0 && OQV[iv].Length == 0)
                        dW += 0.1 * (iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]);

                    if (RQV[iv].Length == 0 || OQV[iv].Length == 0)
                        continue;

                    String sInVal = RQV[iv];

                    if (bWithAlgorithm && adbrCurrent.ID >= 0)
                    {
                        if (adbrCurrent.iMsgHarTypes[iv] == 0)
                        {
                            if (adbrCurrentDictPairs[iv].Keys.Contains(sInVal))
                                sInVal = adbrCurrentDictPairs[iv][sInVal].ToLower();
                        }
                        else if (adbrCurrent.iMsgHarTypes[iv] == 1)
                        {
                            if (adbrCurrentDictPairs[iv].Count > 0)
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

                    if (iv == 0)
                    {
                        if (compareAttribute(sInVal, OQV[iv]))
                        {
                            dW += (iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]);
                            dWtheme = getThemeKoefToLexicalVectorCompare();
                        }
                        else
                        {
                            if (iCompareVectorsKoef[iv] == 1111)
                            {
                                if (adbrCurrent.LinkAdditionalThemes && adbrCurrentAllowedAdditionalThemes.ContainsKey(OQV[iv].ToLower()))
                                {
                                    double dwKoef = (double)adbrCurrentAllowedAdditionalThemes[OQV[iv].ToLower()];
                                    dW += (iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]);
                                    dWtheme = getThemeKoefToLexicalVectorCompare() * dwKoef / 100.0;
                                    if (dWtheme == 0)
                                        dWtheme = 1;
                                }
                                else
                                    bSuccess = false;
                            }
                        }
                    }
                    else
                    {
                        if (compareAttribute(sInVal, OQV[iv]))
                        {
                            dW += (iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]);
                        }
                        else
                        {
                            if (iCompareVectorsKoef[iv] == 1111)
                                bSuccess = false;
                        }
                    }
                }
                dW = Math.Round(dW * 100 / dWMax);
                if (!bSuccess)
                {
                    dW = 0;
                }
                else
                {
                    dW *= getWeight(OQV[8]);
                }
            }
            return dW;
        }

        private bool compareAttribute(String srcAttr, String dstAttr)
        {
            if (srcAttr.Equals(dstAttr))
                return true;

            bool bSrcNotLogic = srcAttr.StartsWith("!");
            bool bDstNotLogic = dstAttr.StartsWith("!");

            if (srcAttr.StartsWith("!") || dstAttr.StartsWith("!") || srcAttr.Contains('~') || dstAttr.Contains('~'))
            {
                String[] srcVector = bSrcNotLogic ? srcAttr.Substring(1).Split('~') : srcAttr.Split('~');
                String[] dstVector = bDstNotLogic ? dstAttr.Substring(1).Split('~') : dstAttr.Split('~');

                bool bFound = false;
                foreach (String srcStr in srcVector)
                {
                    String srcValue = srcStr.Trim();
                    if (srcValue.Length == 0)
                        continue;

                    foreach (String dstStr in dstVector)
                    {
                        String dstValue = dstStr.Trim();
                        if (dstValue.Length == 0)
                            continue;

                        if (srcValue.Equals(dstValue))
                        {
                            if (bSrcNotLogic)
                            {
                                if (bDstNotLogic)
                                    return true;
                                else
                                    return false;
                            }
                            else
                            {
                                if (bDstNotLogic)
                                    return false;
                                else
                                    return true;
                            }
                        }
                    }
                }
            }

            if (bSrcNotLogic)
            {
                if (bDstNotLogic)
                    return true;
                else
                    return true;
            }
            else
            {
                if (bDstNotLogic)
                    return true;
                else
                    return false;
            }
        }

        private void SetEQOutMessageList(String sRQ)
        {
            bool bThemeChaged = false;
            if (adbrCurrent.ID != GetContacterAlgorithmID() && adbrCurrent.ChangeThemeAlgorithmINMessageTheme)
                bThemeChaged = true;

            Set_labelOutEqMsgHarTitleValue_Text("");

            clearlblEQOutHarValues();
            listBoxOutMsg.SelectedIndex = -1;
            listBoxOutMsg.Items.Clear();

            if (sRQ.Length > 0)
            {
                lstEQOutMessagesList = new List<String>();
                sRQ = sRQ.Substring(sRQ.IndexOf("|") + 1);
                sRQ = sRQ.Substring(0, sRQ.IndexOf("|@!"));

                String[] RQV = sRQ.ToLower().Split('|');
                SetEQMessageIdealParameters(sRQ.Split('|'));

                double dCompareVectorLevel = comboBoxCompareVectorLevel.SelectedIndex/* * 10*/;
                double dWMax = 0;
                for (int i = 0; i < iMsgHarCount; i++)
                    dWMax += (iCompareVectorsKoef[i] == 1111 ? 100 : iCompareVectorsKoef[i]);

                if (dWMax > 0)
                {
                    foreach (String EQ in lstEQOutMessagesDB)
                    {
                        double dW = 0;
                        bool bSuccess = true;

                        if (!EQ.Contains(("|@!")))
                            continue;

                        String EQText = EQ.Substring(EQ.IndexOf("|") + 1);
                        EQText = EQText.Substring(0, EQText.IndexOf("|@!"));
                        String[] OQV = EQText.ToLower().Split('|');

                        for (int iv = 0; iv < RQV.Length; iv++)
                        {
                            //if (RQV[iv].Length == 0 && OQV[iv].Length == 0)
                            //    dW += (iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]);
                            if (iCompareVectorsKoef[iv] == 1111 && RQV[iv].Length > 0 && OQV[iv].Length == 0)
                            {
                                if (iv == 12 || iv == 13)
                                {
                                    dW += iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv];
                                    continue;
                                }
                                else
                                    bSuccess = false;
                            }

                            if ("*".Equals(OQV[iv]))
                            {
                                dW += (float)((iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]) * adbrCurrent.CompareStarLevel) / 100.0;
                                continue;
                            }

                            if (RQV[iv].Length == 0 && OQV[iv].Length == 0)
                                dW += 0.1 * (iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]);

                            if (RQV[iv].Length == 0 || OQV[iv].Length == 0)
                                continue;

                            String sInVal = RQV[iv];
                            if (adbrCurrent.ID >= 0)
                            {
                                if (adbrCurrent.iMsgHarTypes[iv] == 0)
                                {
                                    if (adbrCurrentDictPairs[iv].Keys.Contains(sInVal))
                                        sInVal = adbrCurrentDictPairs[iv][sInVal].ToLower();
                                }
                                else if (adbrCurrent.iMsgHarTypes[iv] == 1)
                                {
                                    if (adbrCurrentDictPairs[iv].Count > 0)
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

                            if (compareAttribute(sInVal, OQV[iv]))
                                dW += (iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]);
                            else
                            {
                                if (iCompareVectorsKoef[iv] == 1111)
                                    bSuccess = false;
                            }
                        }
                        dW *= getWeight(OQV[8]);
                        dW = Math.Round(dW * 100 / dWMax);
                        if (dW > dCompareVectorLevel && bSuccess)
                        {
                            if (adbrCurrent.bLockDuplicateSend)
                            {
                                String scurmsgtext = GetMessageTextWOMarker(EQ.Substring(EQ.IndexOf("|@!") + 3));
                                if (sCurrentContactLastMessageText.Equals(scurmsgtext) || sCurrentContactPrevLastMessageText.Equals(scurmsgtext))
                                    dW = 0;
                            }
                            String value = ((int)dW).ToString("000000").Substring(0, 6) + FormatMsgID(OQV[MSG_ID_COLUMN]) + "|" + EQ.Substring(EQ.IndexOf("|") + 1);
                            lstEQOutMessagesList.Add(value);
                        }
                    }
                }
                lstEQOutMessagesList = lstEQOutMessagesList.OrderByDescending(i => i).ToList();

                foreach (String EQ in lstEQOutMessagesList)
                {
                    listBoxOutMsg.Items.Add(Convert.ToInt32(EQ.Substring(0, 6)).ToString("00") + "%" + (bThemeChaged ? "? " : " ") + GetMessageTextWithMarker(EQ.Substring(EQ.IndexOf("|@!") + 3)));
                }

                if (lstEQOutMessagesList.Count > 0)
                {
                    listBoxOutMsg.SelectedIndex = 0;
                }
            }
        }

        private String FormatMsgID(String sID)
        {
            if (sID.Length == 0)
                return "000000";

            long _id = 0;
            try
            {
                _id = 1000000 - Convert.ToInt64(sID);
            }
            catch
            {

            }
            return _id.ToString("000000").Substring(0, 6);
        }

        private void SetEQOutMessageParametersDefaultValuesOut()
        {
            if (sCurrentEQOutMessageRecordOut.Length > 0)
            {
                String value = sCurrentEQOutMessageRecordOut;
                // Установка параметров по умолчанию
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    value = value.Substring(value.IndexOf("|") + 1);
                    String s = value.Substring(0, value.IndexOf("|"));
                    //sMsgHar[i, 3] = s; // Значение атрибутов
                    lblEQOutHarValues[i].Text = s;
                    toolTipMessage.SetToolTip(lblEQOutHarValues[i], s);
                }
            }
        }


        private void SetEQOutMessageListOut(String sRQ)
        {
            Set_labelOutEqMsgHarTitleValue_Text("");

            SetEQOutMessageParametersDefaultValuesOut();
            listBoxOutMsg.SelectedIndex = -1;
            listBoxOutMsg.Items.Clear();

            if (sRQ.Length > 0)
            {
                lstEQOutMessagesList = new List<String>();
                sRQ = sRQ.Substring(sRQ.IndexOf("|") + 1);
                sRQ = sRQ.Substring(0, sRQ.IndexOf("|@!"));

                String[] RQV = sRQ.ToLower().Split('|');
                double dCompareVectorLevel = comboBoxCompareVectorLevel.SelectedIndex/* * 10*/;
                double dWMax = 0;
                for (int i = 0; i < iMsgHarCount; i++)
                    dWMax += (iCompareVectorsKoefOut[i] == 1111 ? 100 : iCompareVectorsKoefOut[i]);

                if (dWMax > 0)
                {
                    foreach (String EQ in lstEQOutMessagesDB)
                    {
                        double dW = 0;
                        bool bSuccess = true;
                        String EQText = EQ.Substring(EQ.IndexOf("|") + 1);
                        EQText = EQText.Substring(0, EQText.IndexOf("|@!"));
                        String[] OQV = EQText.ToLower().Split('|');

                        for (int iv = 0; iv < RQV.Length; iv++)
                        {
                            if (iCompareVectorsKoefOut[iv] == 1111 && RQV[iv].Length > 0 && OQV[iv].Length == 0)
                                bSuccess = false;

                            if ("*".Equals(OQV[iv]))
                            {
                                //dW += (iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]);
                                dW += (float)((iCompareVectorsKoef[iv] == 1111 ? 100 : iCompareVectorsKoef[iv]) * adbrCurrent.CompareStarLevel) / 100.0;
                                continue;
                            }

                            if (RQV[iv].Length == 0 && OQV[iv].Length == 0)
                                dW += 0.1 * (iCompareVectorsKoefOut[iv] == 1111 ? 100 : iCompareVectorsKoefOut[iv]);

                            if (RQV[iv].Length == 0 || OQV[iv].Length == 0)
                                continue;

                            String sInVal = RQV[iv];
                            if (adbrCurrent.ID >= 0)
                            {
                                if (adbrCurrent.iMsgHarTypes[iv] == 0)
                                {
                                    if (adbrCurrentDictPairs[iv].Keys.Contains(sInVal))
                                        sInVal = adbrCurrentDictPairs[iv][sInVal].ToLower();
                                }
                                else if (adbrCurrent.iMsgHarTypes[iv] == 1)
                                {
                                    if (adbrCurrentDictPairs[iv].Count > 0)
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

                            if (compareAttribute(sInVal, OQV[iv]))
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
                            if (adbrCurrent.bLockDuplicateSend)
                            {
                                String scurmsgtext = GetMessageTextWOMarker(EQ.Substring(EQ.IndexOf("|@!") + 3));
                                if (sCurrentContactLastMessageText.Equals(scurmsgtext) || sCurrentContactPrevLastMessageText.Equals(scurmsgtext))
                                    dW = 0;
                            }

                            String value = ((int)dW).ToString("000000").Substring(0, 6) + FormatMsgID(OQV[MSG_ID_COLUMN]) + "|" + EQ.Substring(EQ.IndexOf("|") + 1);
                            lstEQOutMessagesList.Add(value);
                        }
                    }
                }
                lstEQOutMessagesList = lstEQOutMessagesList.OrderByDescending(i => i).ToList();

                foreach (String EQ in lstEQOutMessagesList)
                {
                    listBoxOutMsg.Items.Add(Convert.ToInt32(EQ.Substring(0, 6)).ToString("00") + "% " + GetMessageTextWithMarker(EQ.Substring(EQ.IndexOf("|@!") + 3)));
                }

                if (lstEQOutMessagesList.Count > 0)
                {
                    listBoxOutMsg.SelectedIndex = 0;
                }
            }
        }

        private void AddEQInMessageParametersValues(String sText)
        {
            // manual set timers
            StopAnswerTimer();
            FormEditMsgValues fe = new FormEditMsgValues(this);
            fe.Text += " " + "Сообщения Контактера";
            fe.sPersHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sPersHar[i, j] = sMsgHar[i, j];
                fe.sPersHar[i, iMsgHarAttrCount] = sMsgHar[i, 3];//(i > 0 ? "" : sMsgHar[i, 3]);
            }
            fe.sPersHar[MSG_ID_COLUMN, iMsgHarAttrCount] = (iMsgINMaxID + 1).ToString();

            fe.iPersHarAttrCount = iMsgHarAttrCount;
            fe.iPersHarCount = iMsgHarCount;
            fe.textBox1.Text = NilsaUtils.StringToText(sText);
            fe.comboBox2.SelectedIndex = 0;
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

                if (!hashsetEQInMessagesDB.Contains(sMsgNewRec))
                {
                    lstEQInMessagesDB.Add(sMsgNewRec);
                    hashsetEQInMessagesDB.Add(sMsgNewRec);
                    SaveEQInMessageDB();
                    iMsgINMaxID++;
                    NilsaUtils.SaveLongValue(0, iMsgINMaxID);
                }
                UndoMarkerChanges();
                SetEQInMessageList(labelInMsgHarTitleValue_Text);
            }
            StartAnswerTimer();
        }

        private void EditEQInMessageParametersValues(String sMessage, String sMarker)
        {
            // manual set timers
            StopAnswerTimer();
            SetEQInMessageParametersDefaultValues();
            String sMsgNewRec = "";
            if (sMessage.Trim().Length > 0)
            {
                if (listBoxInMsg.SelectedIndex >= 0)
                {
                    sMsgNewRec = "000000|";
                    string _tmp = lstEQInMessagesList[listBoxInMsg.SelectedIndex];
                    for (int i = 0; i < iMsgHarCount; i++)
                    {
                        _tmp = _tmp.Substring(_tmp.IndexOf("|") + 1);
                        String _s = _tmp.Substring(0, _tmp.IndexOf("|"));
                        sMsgNewRec = sMsgNewRec + _s + "|";
                    }
                }
                else
                {
                    sMsgNewRec = "000000|";
                    for (int i = 0; i < iMsgHarCount; i++)
                        sMsgNewRec = sMsgNewRec + /*(i > 0 ? "" : sMsgHar[i, 3])*/sMsgHar[i, 3] + "|";
                }
                sMsgNewRec = sMsgNewRec + "@!" + NilsaUtils.TextToString(sMessage) + (sMarker.Length > 0 ? ("|!*#0" + sMarker) : "");

            }

            FormEditEQMessagesDB fe = new FormEditEQMessagesDB(this, true);
            fe.sMsgHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sMsgHar[i, j] = sMsgHar[i, j];
                fe.sMsgHar[i, iContHarAttrCount] = "";
            }
            fe.iMsgHarCount = iMsgHarCount;
            fe.iMsgHarAttrCount = iMsgHarAttrCount;
            fe.Setup(iPersUserID.ToString() + " (" + userLogin + ", " + userPassword + ")", iPersUserID, sMsgNewRec);

            fe.ShowDialog();
            if (labelInMsgHarTitleValue_Text.Length > 0)
            {
                UndoMarkerChanges();
                SetEQInMessageList(labelInMsgHarTitleValue_Text);
                if (fe.bNeedSetMessage && fe.SelectedMsgRecord.Length > 0)
                {
                    if (!lstEQInMessagesList.Contains(fe.SelectedMsgRecord))
                    {
                        listBoxInMsg.SelectedIndex = -1;
                        lstEQInMessagesList.Insert(0, fe.SelectedMsgRecord);
                        listBoxInMsg.Items.Insert(0, "?% " + GetMessageTextWithMarker(fe.SelectedMsgRecord.Substring(fe.SelectedMsgRecord.IndexOf("|@!") + 3)));
                        listBoxInMsg.SelectedIndex = 0;
                    }
                    else
                    {
                        listBoxInMsg.SelectedIndex = lstEQInMessagesList.IndexOf(fe.SelectedMsgRecord);
                    }
                }
            }
            StartAnswerTimer();
        }

        private void EditEQOutMessageParametersValues(String sMessage, String sMarker)
        {
            // manual set timers
            StopAnswerTimer();
            //SetEQOutMessageParametersDefaultValues();
            SetEQInMessageParametersDefaultValues();
            String sMsgNewRec = "";
            if ((sMessage.Trim().Length == 0) && (labelInEqMsgHarTitleValue_Text.Trim().Length > 0))
                sMessage = NilsaUtils.Dictonary_GetText(userInterface, "textValues_11", this.Name, "Новое исходящее сообщение");
            if (sMessage.Trim().Length > 0)
            {
                sMsgNewRec = "000000|";
                for (int i = 0; i < iMsgHarCount; i++)
                    sMsgNewRec = sMsgNewRec + lblEQOutHarValues[i].Text/*sMsgHar[i, 3]*/ + "|";
                sMsgNewRec = sMsgNewRec + "@!" + NilsaUtils.TextToString(sMessage) + (sMarker.Length > 0 ? ("|!*#0" + sMarker) : "");
            }

            FormEditEQMessagesDB fe = new FormEditEQMessagesDB(this, false);
            fe.sMsgHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sMsgHar[i, j] = sMsgHar[i, j];
                fe.sMsgHar[i, iContHarAttrCount] = "";
            }
            fe.iMsgHarCount = iMsgHarCount;
            fe.iMsgHarAttrCount = iMsgHarAttrCount;
            fe.Setup(iPersUserID.ToString() + " (" + userLogin + ", " + userPassword + ")", iPersUserID, sMsgNewRec);

            fe.ShowDialog();
            SetEQOutMessageList(sCurrentEQInMessageRecord);
            if (fe.bNeedSetMessage && fe.SelectedMsgRecord.Length > 0)
            {
                if (!lstEQOutMessagesList.Contains(fe.SelectedMsgRecord))
                {
                    listBoxOutMsg.SelectedIndex = -1;
                    lstEQOutMessagesList.Insert(0, fe.SelectedMsgRecord);
                    listBoxOutMsg.Items.Insert(0, "?% " + GetMessageTextWithMarker(fe.SelectedMsgRecord.Substring(fe.SelectedMsgRecord.IndexOf("|@!") + 3)));
                    listBoxOutMsg.SelectedIndex = 0;
                }
                else
                {
                    try
                    {
                        listBoxOutMsg.SelectedIndex = lstEQOutMessagesList.IndexOf(fe.SelectedMsgRecord);
                    }
                    catch
                    {
                        listBoxOutMsg.SelectedIndex = -1;
                    }
                }
            }
            StartAnswerTimer();
        }

        private void AddEQOutMessageParametersValues(String sText)
        {
            // manual set timers
            StopAnswerTimer();
            SetEQInMessageParametersDefaultValues();
            FormEditMsgValues fe = new FormEditMsgValues(this);
            fe.Text += " " + "Сообщения Персонажа";
            fe.sPersHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sPersHar[i, j] = sMsgHar[i, j];
                fe.sPersHar[i, iMsgHarAttrCount] = (i == 12 || i == 13) ? "" : (FormMain.IdealRQV.Length > 0 ? FormMain.IdealRQV[i] : sMsgHar[i, 3]);// (i > 0 ? "" : sMsgHar[i, 3]);
            }
            fe.sPersHar[MSG_ID_COLUMN, iMsgHarAttrCount] = (iMsgOUTMaxID + 1).ToString();

            fe.iPersHarAttrCount = iMsgHarAttrCount;
            fe.iPersHarCount = iMsgHarCount;
            fe.textBox1.Text = "";// NilsaUtils.StringToText(sText);
            fe.comboBox2.SelectedIndex = 0;
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

                if (!hashsetEQOutMessagesDB.Contains(sMsgNewRec))
                {
                    lstEQOutMessagesDB.Add(sMsgNewRec);
                    hashsetEQOutMessagesDB.Add(sMsgNewRec);
                    SaveEQOutMessageDB();
                    iMsgOUTMaxID++;
                    NilsaUtils.SaveLongValue(1, iMsgOUTMaxID);
                }
                SetEQOutMessageList(sCurrentEQInMessageRecord);
            }
            StartAnswerTimer();
        }

        private void buttonEditInMsgHar_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            AddEQInMessageParametersValues(labelInMsgHarTitleValue_Text);
        }

        private void buttonEditInEqMsgHar_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            EditEQInMessageParametersValues(labelInEqMsgHarTitleValue_Text, labelInEqMsgHarTitleMarker.Text);
        }

        private void buttonEditOutEqMsgHar_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            EditEQOutMessageParametersValues(labelOutEqMsgHarTitleValueFullText, labelOutEqMsgHarTitleMarker.Text);
        }

        private void listBoxInMsg_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxInMsg.SelectedIndex >= 0)
            {
                //buttonEditInMsgHar.Enabled = true;
                //tbDeleteInEQMessage.Enabled = true;
                if (!bUnknownMessageGenerated)
                {
                    tbMessagesDBEqInDeleteMessage.Enabled = true;
                    tbMessagesDBEqInEditMessage.Enabled = true;
                }
                else
                {
                    tbMessagesDBEqInDeleteMessage.Enabled = false;
                    tbMessagesDBEqInEditMessage.Enabled = false;
                }
                SetEQInMessageParametersValues(lstEQInMessagesList[listBoxInMsg.SelectedIndex]);
            }
            else
            {
                tbMessagesDBEqInDeleteMessage.Enabled = false;
                tbMessagesDBEqInEditMessage.Enabled = false;
            }
        }

        private void listBoxOutMsg_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxOutMsg.SelectedIndex >= 0)
            {
                //buttonNewOutMsgHar.Enabled = true;
                //tbDeleteOutEQMessage.Enabled = true;
                tbMessagesDBEqOutDeleteMessage.Enabled = true;
                tbPersoneCopyMessage.Enabled = true;
                tbMessagesDBEqOutEditMessage.Enabled = true;
                SetEQOutMessageParametersValues(lstEQOutMessagesList[listBoxOutMsg.SelectedIndex]);
            }
            else
            {
                tbMessagesDBEqOutDeleteMessage.Enabled = false;
                tbPersoneCopyMessage.Enabled = false;
                tbMessagesDBEqOutEditMessage.Enabled = false;
            }
        }

        private void tbSkipMessage_Click(object sender, EventArgs e)
        {
            if (lstReceivedMessages.Count > 0)
                lstReceivedMessages.Add(lstReceivedMessages[0]);

            SelectNextReceivedMessage();
            if (!timerAnswerWaiting.Enabled)
                timerAnswerWaitingOn();
        }

        public String SetMessageFields(String message)
        {
            String retval = message;

            retval = retval.Replace("#contacter_id#", iContUserID.ToString());
            retval = retval.Replace("#persone_id#", iPersUserID.ToString());
            retval = retval.Replace("#firstname#", labelCont1Name.Text);
            retval = retval.Replace("#lastname#", labelCont1Family.Text);
            retval = retval.Replace("#name#", (labelCont1Name.Text + " " + labelCont1Family.Text).Trim());

            retval = retval.Replace("#firstname_persone#", labelPers1Name.Text);
            retval = retval.Replace("#lastname_persone#", labelPers1Family.Text);
            retval = retval.Replace("#name_persone#", (labelPers1Name.Text + " " + labelPers1Family.Text).Trim());

            for (int i = 0; i < iPersHarCount; i++)
            {
                retval = retval.Replace("#persone_" + (i + 1).ToString() + "#", makeTextVariants(lblPersHarValues[i].Text));
            }
            for (int i = 0; i < iContHarCount; i++)
            {
                retval = retval.Replace("#contacter_" + (i + 1).ToString() + "#", makeTextVariants(lblContHarValues[i].Text));
            }
            return retval;
        }

        private void lstOutgoingMessages_Insert(String uid, String uname, String text, String channel = "0", long _iGroupAnswerID = -1, long _iGroupAnswerPostID = -1, long _iGroupAnswerCommentID = -1)
        {
            //mFormMain.lstOutgoingMessages.Add("*#|" + comboBox1.SelectedIndex.ToString() + "|" + uid + "|" + uname + "|" + SetMessageFields(text.Trim(), uname));
            if (_iGroupAnswerID < 0)
                lstOutgoingMessages.Insert(0, "*#|" + channel + "|" + uid + "|" + uname + "|" + SetMessageFields(NilsaUtils.TextToString(text.Trim())));
            else
                lstOutgoingMessages.Insert(0, "*#|4|" + Convert.ToString(_iGroupAnswerID) + "/" + Convert.ToString(_iGroupAnswerPostID) + "/" + Convert.ToString(_iGroupAnswerCommentID) + "/" + uid + "|" + uname + "|" + SetMessageFields(NilsaUtils.TextToString(text.Trim())));

        }

        private void lstOutgoingMessages_AddParts(String uid, String uname, String text, String channel = "0", long _iGroupAnswerID = -1, long _iGroupAnswerPostID = -1, long _iGroupAnswerCommentID = -1)
        {
            //mFormMain.lstOutgoingMessages.Add("*#|" + comboBox1.SelectedIndex.ToString() + "|" + uid + "|" + uname + "|" + SetMessageFields(text.Trim(), uname));
            if (_iGroupAnswerID < 0)
                lstOutgoingMessagesParts.Add("*#|" + channel + "|" + uid + "|" + uname + "|" + SetMessageFields(NilsaUtils.TextToString(text.Trim())));
            else
                lstOutgoingMessagesParts.Add("*#|4|" + Convert.ToString(_iGroupAnswerID) + "/" + Convert.ToString(_iGroupAnswerPostID) + "/" + Convert.ToString(_iGroupAnswerCommentID) + "/" + uid + "|" + uname + "|" + SetMessageFields(NilsaUtils.TextToString(text.Trim())));

        }

        private void sendPrevUserMessage()
        {
            if (sCurUserPrevMsgUID.Length > 0)
            {
                if (!iContUserID.ToString().Equals(sCurUserPrevMsgUID) || !iGroupAnswerID.ToString().Equals(sCurUserPrevGroupAnswerID) || !iGroupAnswerPostID.ToString().Equals(sCurUserPrevGroupAnswerPostID) || !iGroupAnswerCommentID.ToString().Equals(sCurUserPrevGroupAnswerCommentID) || !adbrCurrent.bGroupingOutMessages /*|| iGroupAnswerID >= 0*/)
                {
                    lstOutgoingMessages_Insert(sCurUserPrevMsgUID, sCurUserPrevName, sCurUserPrevMsgText + sCurUserGroupAdditinalUsers, "0", sCurUserPrevGroupAnswerID.Length > 0 ? Convert.ToInt64(sCurUserPrevGroupAnswerID) : -1, sCurUserPrevGroupAnswerPostID.Length > 0 ? Convert.ToInt64(sCurUserPrevGroupAnswerPostID) : -1, sCurUserPrevGroupAnswerCommentID.Length > 0 ? Convert.ToInt64(sCurUserPrevGroupAnswerCommentID) : -1);
                    sCurUserPrevMsgUID = "";
                    sCurUserGroupAdditinalUsers = "";
                    sCurUserPrevName = "";
                    sCurUserPrevMsgText = "";
                    sCurUserPrevGroupAnswerID = "-1";
                    sCurUserPrevGroupAnswerPostID = "-1";
                    sCurUserPrevGroupAnswerCommentID = "-1";
                    timerPhysicalSendStart();
                }
            }
        }

        public String ResolveID(String text)
        {
            String shortname = "";

            if (text.ToLower().Equals("myid"))
                return iContUserID.ToString();

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
                        catch (VkNet.Exception.AccessTokenInvalidException)
                        {
                            if (!ReAutorize(userLogin, userPassword))
                                break;
                        }
                        catch (System.Net.WebException)
                        {
                            if (!ReAutorize(userLogin, userPassword))
                                break;
                        }
                        catch (VkNet.Exception.VkApiException vkapexeption)
                        {
                            if (!ReAutorize(userLogin, userPassword))
                                break;
                        }
                        catch (Exception exp)
                        {
                            ExceptionToLogList("FormMain", "ResolveID", exp);
                            break;
                        }
                    }
                    while (true);
                    */
                }
            }
            return "-1";
        }

        private void tbSendOutMessage_Click(object sender, EventArgs e)
        {
            if (labelOutEqMsgHarTitleValue_Text.Trim().Length > 0)
            {
                if (adbrCurrent.PlaySendMsg)
                {
                    Stream str = Properties.Resources._3;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();
                }

                String cmd_line = lblEQOutHarValues[11].Text.ToLower();

                bool bNotDelayed = true;
                if (cmd_line.StartsWith("time_delay:"))
                {
                    String timeDelay = cmd_line.Substring(cmd_line.IndexOf("time_delay:") + 11).Trim();

                    int iDelayType = 1;
                    if (timeDelay.EndsWith("min"))
                    {
                        iDelayType = 0;
                        timeDelay = timeDelay.Substring(0, timeDelay.IndexOf("min")).Trim();
                    }
                    else if (timeDelay.EndsWith("hour"))
                    {
                        iDelayType = 1;
                        timeDelay = timeDelay.Substring(0, timeDelay.IndexOf("hour")).Trim();
                    }
                    else if (timeDelay.EndsWith("day"))
                    {
                        iDelayType = 2;
                        timeDelay = timeDelay.Substring(0, timeDelay.IndexOf("day")).Trim();
                    }

                    int iDelay = -1;
                    try
                    {
                        iDelay = Convert.ToInt32(timeDelay);
                    }
                    catch
                    {
                        iDelay = -1;
                    }

                    if (iDelay > 0)
                    {
                        DateTime dtDelay = DateTime.Now;
                        if (iDelayType == 0)
                            dtDelay = dtDelay.AddMinutes((double)iDelay);
                        else if (iDelayType == 1)
                            dtDelay = dtDelay.AddHours((double)iDelay);
                        else if (iDelayType == 2)
                            dtDelay = dtDelay.AddDays((double)iDelay);

                        // iContUserID+labelOutEqMsgHarTitleValueFullText
                        lstOutgoingMessagesDelayed.Add(dtDelay.ToBinary().ToString() + "|" + iContUserID.ToString() + "|" + sCurrentEQOutMessageRecord.Replace("|" + lblEQOutHarValues[11].Text, "|" + "time_delay: 0 min"));
                        SaveOutgoingDelayedMessagesPull();
                        bNotDelayed = false;
                    }

                }

                if (bNotDelayed)
                {
                    bSessionAnswerSended = true;
                    // 2019-04-13
                    //ReadNewReceivedMessages();

                    ApplyEQOutMessageMarker();
                    SaveLastMessage();

                    if ((cmd_line.StartsWith("like_")) || (cmd_line.StartsWith("join_community")) || (cmd_line.StartsWith("remove_delayed_messages")) || (cmd_line.StartsWith("command1_operator")) || (cmd_line.StartsWith("save_contacter")) || (cmd_line.StartsWith("load_contacter")) || (cmd_line.StartsWith("command2_operator")) || (cmd_line.StartsWith("command3_operator")) || (cmd_line.StartsWith("repost_ava")) || (cmd_line.StartsWith("send_operator")) || (cmd_line.StartsWith("info_operator")) || (cmd_line.StartsWith("resend_operator")) || (cmd_line.StartsWith("leave_community")) || (cmd_line.StartsWith("repost_wall")) || (cmd_line.StartsWith("repost_group")) || (cmd_line.StartsWith("like_wall")) || (cmd_line.StartsWith("like_group")) || (cmd_line.StartsWith("friends_add")) || (cmd_line.StartsWith("friends_delete")))
                    {
                        if (cmd_line.StartsWith("remove_delayed_messages")) // Done!
                        {
                            bool bStatusService = !tbStartService.Enabled;
                            tbStopService_Click(null, null);

                            if (iPersUserID >= 0 && SocialNetwork == 0)
                            {
                                removeDelayedMessages(iContUserID);
                            }

                            if (bStatusService)
                                tbStartService_Click(null, null);
                        }
                        else if (cmd_line.StartsWith("like_ava")) // Done!
                        {
                            bool bStatusService = !tbStartService.Enabled;
                            tbStopService_Click(null, null);

                            if (iPersUserID >= 0 && SocialNetwork == 0)
                            {
                                /*
                                VkNet.Model.User usrAdr = FormMain.api.Users.Get(iContUserID, ProfileFields.PhotoId);
                                if (usrAdr.PhotoId != null)
                                {
                                    if (usrAdr.PhotoId.Length > 0)
                                    {
                                        int iPhotoIDPos = usrAdr.PhotoId.IndexOf('_');
                                        if (iPhotoIDPos > 0)
                                        {
                                            string sPhotoID = usrAdr.PhotoId.Substring(iPhotoIDPos + 1);
                                            long lPhotoID = -1;
                                            try
                                            {
                                                lPhotoID = Convert.ToInt64(sPhotoID);
                                            }
                                            catch { }
                                            if (lPhotoID != -1)
                                            {
                                                try
                                                {
                                                    api.Likes.Add(VkNet.Enums.SafetyEnums.LikeObjectType.Photo, lPhotoID, iContUserID);
                                                }
                                                catch { }
                                                //if (FormWebBrowserEnabled)
                                                //{
                                                //    FormWebBrowser fwb = new FormWebBrowser(this);
                                                //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.ShowContactPage, iContUserID);
                                                //    fwb.ShowDialog();
                                                //}
                                            }
                                        }
                                    }
                                }
                                else if (FormWebBrowserEnabled)
                                {
                                */
                                /*
                                FormWebBrowser fwb = new FormWebBrowser(this);
                                fwb.Setup(userLogin, userPassword, WebBrowserCommand.LikeContacterAva, iContUserID);
                                fwb.ShowDialog();
                                */
                                ShowBrowserCommand();

                                fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.LikeContacterAva, iContUserID);
                                //fwbVKontakte.Show();
                                fwbVKontakte.WaitResult();

                                HideBrowserCommand();
                                //}
                            }

                            if (bStatusService)
                                tbStartService_Click(null, null);
                        }
                        else if (cmd_line.StartsWith("repost_ava")) // Done!
                        {
                            bool bStatusService = !tbStartService.Enabled;
                            tbStopService_Click(null, null);

                            if (iPersUserID >= 0 && SocialNetwork == 0)
                            {
                                /*
                                VkNet.Model.User usrAdr = FormMain.api.Users.Get(iContUserID, ProfileFields.PhotoId);
                                if (usrAdr.PhotoId != null)
                                {
                                    if (usrAdr.PhotoId.Length > 0)
                                    {
                                        int iPhotoIDPos = usrAdr.PhotoId.IndexOf('_');
                                        if (iPhotoIDPos > 0)
                                        {
                                            string sPhotoID = usrAdr.PhotoId.Substring(iPhotoIDPos + 1);
                                            long lPhotoID = -1;
                                            try
                                            {
                                                lPhotoID = Convert.ToInt64(sPhotoID);
                                            }
                                            catch { }
                                            if (lPhotoID != -1)
                                            {
                                                try
                                                {
                                                    api.Wall.Repost("photo" + usrAdr.PhotoId);
                                                    api.Likes.Add(VkNet.Enums.SafetyEnums.LikeObjectType.Photo, lPhotoID, iContUserID);
                                                }
                                                catch { }
                                                //if (FormWebBrowserEnabled)
                                                //{
                                                //    FormWebBrowser fwb = new FormWebBrowser(this);
                                                //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.GoToPersonePage, iPersUserID);
                                                //    fwb.ShowDialog();
                                                //}
                                            }
                                        }
                                    }
                                }
                                */
                            }

                            if (bStatusService)
                                tbStartService_Click(null, null);
                        }
                        // resend_operator
                        else if (cmd_line.StartsWith("resend_operator")) // Done!
                        {
                            bool bStatusService = !tbStartService.Enabled;
                            tbStopService_Click(null, null);

                            if (SocialNetwork == 0 && iContUserID >= 0 && iPersUserID >= 0)
                            {
                                if (CheckOperator(iContUserID))
                                    iResendContUserID = -1;
                                else
                                    iResendContUserID = iContUserID;
                                SaveResendContUserID();

                                if (labelInMsgHarTitleValue_Text.Trim().ToLower().StartsWith("resend_operator ") || labelInMsgHarTitleValue_Text.Trim().ToLower().StartsWith("напиши "))
                                {
                                    string _grpsLst = labelInMsgHarTitleValue_Text.Trim();
                                    if (_grpsLst.Length > 0 && _grpsLst.IndexOf(" ") > 0)
                                    {
                                        _grpsLst = _grpsLst.Substring(_grpsLst.IndexOf(" ") + 1).Trim();
                                        if (_grpsLst.Length > 0)
                                        {
                                            long lgid = -1;
                                            try
                                            {
                                                lgid = Convert.ToInt64(_grpsLst);
                                            }
                                            catch
                                            {
                                                lgid = -1;
                                            }

                                            if (lgid >= 0)
                                            {
                                                iResendContUserID = lgid;
                                                SaveResendContUserID();
                                            }
                                        }
                                    }
                                }

                                if (iResendContUserID >= 0)
                                {
                                    ResendToOperators(true);
                                }
                            }

                            if (bStatusService)
                                tbStartService_Click(null, null);
                        }
                        // send_operator
                        else if (cmd_line.StartsWith("send_operator")) // Done!
                        {
                            String soperators = lblPersHarValues[13].Text.Trim() + ",";
                            string[] sOperIDs = soperators.Split(',');
                            bool bStatusService = !tbStartService.Enabled;
                            tbStopService_Click(null, null);

                            if (iPersUserID >= 0 && SocialNetwork == 0)
                            {
                                ReadAllUserMessages(iContUserID);
                                string msg = "SEND_OPERATOR\n";
                                msg += iContUserID.ToString() + "\n";
                                msg += labelCont1FIO.Text + "\n\n";
                                int iStart = listBoxUserMessages.Items.Count - 25;
                                if (iStart < 0)
                                    iStart = 0;
                                for (int i = iStart; i < listBoxUserMessages.Items.Count; i++)
                                {
                                    msg += listBoxUserMessages.Items[i] + "\n";
                                }
                                foreach (string soperid in sOperIDs)
                                {
                                    string soper = soperid.Trim();
                                    if (soper.Length > 0)
                                    {
                                        long lgid = -1;
                                        try
                                        {
                                            lgid = Convert.ToInt64(soper);
                                        }
                                        catch
                                        {
                                            lgid = -1;
                                        }

                                        if (lgid >= 0)
                                        {
                                            api_Messages_Send(lgid, msg);
                                            //try
                                            //{
                                            //    api.Messages.Send(lgid, false, msg); // посылаем сообщение пользователю
                                            //}
                                            //catch (VkNet.Exception.AccessTokenInvalidException)
                                            //{
                                            //    ReAutorize(userLogin, userPassword);
                                            //}
                                            //catch (System.Net.WebException)
                                            //{
                                            //    ReAutorize(userLogin, userPassword);
                                            //}
                                            //catch (VkNet.Exception.VkApiException vkapexeption)
                                            //{
                                            //    ReAutorize(userLogin, userPassword);
                                            //}
                                            //catch (Exception exp)
                                            //{
                                            //    ExceptionToLogList("Setup", "UNKNOWN", exp);
                                            //}
                                        }
                                    }
                                }
                            }

                            if (bStatusService)
                                tbStartService_Click(null, null);
                        }
                        //--- command operator - Start
                        // command1_operator
                        else if (cmd_line.StartsWith("command1_operator")) // Done!
                        {
                            sendCommandToOperators("KNOBY1~KNOBY2~KNOBY3");
                        }
                        else if (cmd_line.StartsWith("command2_operator")) // Done!
                        {
                            sendCommandToOperators("ACTION1~ACTION2~ACTION3");
                        }
                        else if (cmd_line.StartsWith("command3_operator")) // Done!
                        {
                            sendCommandToOperators("DOITNOW1~DOITNOW2~DOITNOW3");
                        }
                        //--- command operator - End
                        // save_contacter
                        else if (cmd_line.StartsWith("save_contacter")) // Done! 
                        {
                            string haridxs = cmd_line;
                            if (haridxs.IndexOf(" ") > 0)
                            {
                                haridxs = haridxs.Substring(haridxs.IndexOf(" ")).Trim();
                                String[] haridxArray = haridxs.Split(',');
                                foreach (String sharidx in haridxArray)
                                {
                                    String sHarIdxItem = sharidx.Trim();
                                    if (sHarIdxItem.Length == 0)
                                        continue;
                                    int harIdx = -1;
                                    try
                                    {
                                        harIdx = Convert.ToInt32(sHarIdxItem);
                                    }
                                    catch
                                    {
                                        harIdx = -1;
                                    }

                                    if (harIdx >= 1 && harIdx <= 16)
                                    {
                                        putContactHarValue(harIdx, lblContHarValues[harIdx - 1].Text);
                                    }
                                }
                            }
                        }
                        // load_contacter
                        else if (cmd_line.StartsWith("load_contacter")) // Done!
                        {
                            string haridxs = cmd_line;
                            if (haridxs.IndexOf(" ") > 0)
                            {
                                haridxs = haridxs.Substring(haridxs.IndexOf(" ")).Trim();
                                String[] haridxArray = haridxs.Split(',');
                                bool bSetVal = false;
                                String algSet = "";
                                foreach (String sharidx in haridxArray)
                                {
                                    String sHarIdxItem = sharidx.Trim();
                                    if (sHarIdxItem.Length == 0)
                                        continue;
                                    int harIdx = -1;
                                    try
                                    {
                                        harIdx = Convert.ToInt32(sHarIdxItem);
                                    }
                                    catch
                                    {
                                        harIdx = -1;
                                    }

                                    if (harIdx >= 1 && harIdx <= 16)
                                    {
                                        if (lstContHarValues.Count >= harIdx)
                                        {
                                            String harValue = getContactHarValue(harIdx);
                                            if (harValue.Length > 0)
                                            {
                                                if (harValue.Equals("#clear#"))
                                                    harValue = "";

                                                lstContHarValues[harIdx - 1] = (harIdx).ToString() + "|" + harValue;
                                                lblContHarValues[harIdx - 1].Text = harValue;
                                                toolTipMessage.SetToolTip(lblContHarValues[harIdx - 1], sContHar[harIdx - 1, 1] + ": " + harValue);
                                                bSetVal = true;

                                                if (CONTACT_COLUMN_ALGORITHM == harIdx - 1)
                                                {
                                                    algSet = harValue;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (bSetVal)
                                {
                                    SaveContactParamersValues();

                                    if (algSet.Length > 0)
                                    {
                                        int iMsgAlg = SearchAlgorithmsDBList(algSet);
                                        if (iMsgAlg >= 0)
                                        {
                                            SaveAlgorithmSettingsContacter(iMsgAlg);

                                            LoadContactParametersDescription();
                                            LoadAlgorithmSettingsContacter();
                                            UpdateContactParametersValues_Algorithm();
                                        }
                                    }
                                }
                            }
                        }
                        // info_operator
                        else if (cmd_line.StartsWith("info_operator")) // Done!
                        {
                            String soperators = lblPersHarValues[13].Text.Trim() + ",";
                            string[] sOperIDs = soperators.Split(',');
                            bool bStatusService = !tbStartService.Enabled;
                            tbStopService_Click(null, null);

                            if (iPersUserID >= 0 && SocialNetwork == 0)
                            {
                                ReadAllUserMessages(iContUserID);
                                string msg = "INFO_OPERATOR\n";
                                msg += labelCont1FIO.Text + "\n";
                                msg += lblContHarValues[2].Text + "\n\n";
                                int iStart = listBoxUserMessages.Items.Count - 25;
                                if (iStart < 0)
                                    iStart = 0;
                                for (int i = iStart; i < listBoxUserMessages.Items.Count; i++)
                                {
                                    msg += listBoxUserMessages.Items[i] + "\n";
                                }
                                foreach (string soperid in sOperIDs)
                                {
                                    string soper = soperid.Trim();
                                    if (soper.Length > 0)
                                    {
                                        long lgid = -1;
                                        try
                                        {
                                            lgid = Convert.ToInt64(soper);
                                        }
                                        catch
                                        {
                                            lgid = -1;
                                        }

                                        if (lgid >= 0)
                                        {
                                            api_Messages_Send(lgid, msg);
                                            //try
                                            //{
                                            //    api.Messages.Send(lgid, false, msg); // посылаем сообщение пользователю
                                            //}
                                            //catch (VkNet.Exception.AccessTokenInvalidException)
                                            //{
                                            //    ReAutorize(userLogin, userPassword);
                                            //}
                                            //catch (System.Net.WebException)
                                            //{
                                            //    ReAutorize(userLogin, userPassword);
                                            //}
                                            //catch (VkNet.Exception.VkApiException vkapexeption)
                                            //{
                                            //    ReAutorize(userLogin, userPassword);
                                            //}
                                            //catch (Exception exp)
                                            //{
                                            //    ExceptionToLogList("Setup", "UNKNOWN", exp);
                                            //}
                                        }
                                    }
                                }
                            }

                            if (bStatusService)
                                tbStartService_Click(null, null);
                        }
                        //friends_add
                        else if (cmd_line.StartsWith("friends_add")) // Done!
                        {
                            string _grpsLst = labelInMsgHarTitleValue_Text.Trim();
                            if (_grpsLst.Length > 0 && _grpsLst.IndexOf(" ") > 0)
                            {
                                _grpsLst = _grpsLst.Substring(_grpsLst.IndexOf(" ") + 1).Trim();
                                if (_grpsLst.Length > 0)
                                {
                                    bool bStatusService = !tbStartService.Enabled;
                                    tbStopService_Click(null, null);

                                    if (iPersUserID >= 0 && SocialNetwork == 0)
                                    {
                                        /*
                                        string _initGrpsLst = _grpsLst;
                                        if (_grpsLst[_grpsLst.Length - 1] != ',')
                                            _grpsLst += ",";
                                        while (_grpsLst.Length > 0)
                                        {
                                            string sgid = _grpsLst.Substring(0, _grpsLst.IndexOf(',')).Trim();
                                            _grpsLst = _grpsLst.Substring(_grpsLst.IndexOf(',') + 1).Trim();
                                            if (sgid.Length > 0)
                                            {
                                                sgid = ResolveID(sgid);
                                                try
                                                {
                                                    long lgid = Convert.ToInt64(sgid);
                                                    try
                                                    {
                                                        if (lgid != iPersUserID)
                                                            api.Friends.Add(lgid);
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                                catch
                                                {

                                                }

                                            }
                                        }
                                        */

                                        // Просто показ браузера, можно убрать
                                        //if (FormWebBrowserEnabled)
                                        //{
                                        //    FormWebBrowser fwb = new FormWebBrowser(this);
                                        //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.ShowFriendsPage);
                                        //    fwb.ShowDialog();
                                        //}
                                    }

                                    if (bStatusService)
                                        tbStartService_Click(null, null);
                                }
                            }
                        }
                        else if (cmd_line.StartsWith("friends_delete")) // Done!
                        {
                            string _grpsLst = labelInMsgHarTitleValue_Text.Trim();
                            if (_grpsLst.Length > 0 && _grpsLst.IndexOf(" ") > 0)
                            {
                                _grpsLst = _grpsLst.Substring(_grpsLst.IndexOf(" ") + 1).Trim();
                                if (_grpsLst.Length > 0)
                                {
                                    bool bStatusService = !tbStartService.Enabled;
                                    tbStopService_Click(null, null);

                                    if (iPersUserID >= 0 && SocialNetwork == 0)
                                    {
                                        /*
                                        string _initGrpsLst = _grpsLst;
                                        if (_grpsLst[_grpsLst.Length - 1] != ',')
                                            _grpsLst += ",";
                                        while (_grpsLst.Length > 0)
                                        {
                                            string sgid = _grpsLst.Substring(0, _grpsLst.IndexOf(',')).Trim();
                                            _grpsLst = _grpsLst.Substring(_grpsLst.IndexOf(',') + 1).Trim();
                                            if (sgid.Length > 0)
                                            {
                                                sgid = ResolveID(sgid);
                                                try
                                                {
                                                    long lgid = Convert.ToInt64(sgid);
                                                    try
                                                    {
                                                        if (lgid != iPersUserID)
                                                            api.Friends.Delete(lgid);
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                                catch
                                                {

                                                }

                                            }
                                        }
                                        */

                                        // Просто показ браузера, можно убрать
                                        //if (FormWebBrowserEnabled)
                                        //{
                                        //    FormWebBrowser fwb = new FormWebBrowser(this);
                                        //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.ShowFriendsPage);
                                        //    fwb.ShowDialog();
                                        //}
                                    }

                                    if (bStatusService)
                                        tbStartService_Click(null, null);
                                }
                            }
                        }
                        else if (cmd_line.StartsWith("join_community")) // Done!
                        {
                            string _grpsLst = labelInMsgHarTitleValue_Text.Trim();
                            if (_grpsLst.Length > 0 && _grpsLst.IndexOf(" ") > 0)
                            {
                                _grpsLst = _grpsLst.Substring(_grpsLst.IndexOf(" ") + 1).Trim();
                                if (_grpsLst.Length > 0)
                                {
                                    bool bStatusService = !tbStartService.Enabled;
                                    tbStopService_Click(null, null);

                                    if (iPersUserID >= 0 && SocialNetwork == 0)
                                    {
                                        /*
                                        string _initGrpsLst = _grpsLst;
                                        if (_grpsLst[_grpsLst.Length - 1] != ',')
                                            _grpsLst += ",";
                                        while (_grpsLst.Length > 0)
                                        {
                                            string sgid = _grpsLst.Substring(0, _grpsLst.IndexOf(',')).Trim();
                                            _grpsLst = _grpsLst.Substring(_grpsLst.IndexOf(',') + 1).Trim();
                                            if (sgid.Length > 0)
                                            {
                                                sgid = ResolveID(sgid);
                                                try
                                                {
                                                    long lgid = Convert.ToInt64(sgid);
                                                    try
                                                    {
                                                        api.Groups.Join(lgid);
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                                catch
                                                {

                                                }

                                            }
                                        }
                                        */

                                        // Просто показ браузера, можно убрать
                                        //if (FormWebBrowserEnabled)
                                        //{
                                        //    FormWebBrowser fwb = new FormWebBrowser(this);
                                        //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.JoinCommunity, -1, _initGrpsLst);
                                        //    fwb.ShowDialog();
                                        //}
                                    }

                                    if (bStatusService)
                                        tbStartService_Click(null, null);
                                }
                            }
                        }
                        else if (cmd_line.StartsWith("leave_community")) // Done!
                        {
                            string _grpsLst = labelInMsgHarTitleValue_Text.Trim();
                            if (_grpsLst.Length > 0 && _grpsLst.IndexOf(" ") > 0)
                            {
                                _grpsLst = _grpsLst.Substring(_grpsLst.IndexOf(" ") + 1).Trim();
                                if (_grpsLst.Length > 0)
                                {
                                    bool bStatusService = !tbStartService.Enabled;
                                    tbStopService_Click(null, null);

                                    if (iPersUserID >= 0 && SocialNetwork == 0)
                                    {
                                        /*
                                        string _initGrpsLst = _grpsLst;
                                        if (_grpsLst[_grpsLst.Length - 1] != ',')
                                            _grpsLst += ",";
                                        while (_grpsLst.Length > 0)
                                        {
                                            string sgid = _grpsLst.Substring(0, _grpsLst.IndexOf(',')).Trim();
                                            _grpsLst = _grpsLst.Substring(_grpsLst.IndexOf(',') + 1).Trim();
                                            if (sgid.Length > 0)
                                            {
                                                sgid = ResolveID(sgid);
                                                try
                                                {
                                                    long lgid = Convert.ToInt64(sgid);
                                                    try
                                                    {
                                                        api.Groups.Leave(lgid);
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                                catch
                                                {

                                                }

                                            }
                                        }
                                        */

                                        // Просто показ браузера, можно убрать
                                        //if (FormWebBrowserEnabled)
                                        //{
                                        //    FormWebBrowser fwb = new FormWebBrowser(this);
                                        //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.JoinCommunity, -1, _initGrpsLst);
                                        //    fwb.ShowDialog();
                                        //}
                                    }

                                    if (bStatusService)
                                        tbStartService_Click(null, null);
                                }
                            }
                        }
                        else if (cmd_line.StartsWith("repost_wall")) // Done!
                        {
                            string repost_wall_posts = labelInMsgHarTitleValue_Text.Trim();
                            string repost_wall_group = "";
                            if (repost_wall_posts.Length > 0 && repost_wall_posts.IndexOf(" ") > 0)
                            {
                                repost_wall_posts = repost_wall_posts.Substring(repost_wall_posts.IndexOf(" ") + 1).Trim();
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
                            }
                            else
                                repost_wall_posts = "";

                            if (repost_wall_group.Length > 0)
                            {
                                if (repost_wall_group.Trim().ToLower().Equals("mywall"))
                                    repost_wall_group = iContUserID.ToString();

                                long _lGroupID = -1;
                                string _initGrpsLst = repost_wall_group;
                                repost_wall_group = ResolveID(repost_wall_group);
                                try
                                {
                                    _lGroupID = Convert.ToInt64(repost_wall_group);
                                }
                                catch
                                {

                                }

                                bool bStatusService = !tbStartService.Enabled;
                                tbStopService_Click(null, null);

                                if (iPersUserID >= 0 && SocialNetwork == 0 && _lGroupID >= 0)
                                {
                                    /*
                                    int _total = 0;
                                    System.Collections.ObjectModel.ReadOnlyCollection<VkNet.Model.Post> posts = null;
                                    try
                                    {
                                        posts = api.Wall.Get(_lGroupID, out _total, 100);
                                    }
                                    catch { }
                                    string _initPostsLst = repost_wall_posts;

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
                                                if (lgid >= 1 && lgid - 1 < _total)
                                                {
                                                    VkNet.Model.Post post = posts[lgid - 1];
                                                    string str = post.ToString();
                                                    try
                                                    {
                                                        api.Wall.Repost("wall" + post.OwnerId + "_" + post.Id);
                                                        api.Likes.Add(VkNet.Enums.SafetyEnums.LikeObjectType.Post, post.Id, post.OwnerId);
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                    }
                                    */

                                    //if (FormWebBrowserEnabled)
                                    //{
                                    //    FormWebBrowser fwb = new FormWebBrowser(this);
                                    //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.RepostPostFromWallToWall, iPersUserID, _initGrpsLst);
                                    //    fwb.ShowDialog();
                                    //}
                                }

                                if (bStatusService)
                                    tbStartService_Click(null, null);
                            }
                        }
                        else if (cmd_line.StartsWith("like_wall")) // Done!
                        {
                            string repost_wall_posts = labelInMsgHarTitleValue_Text.Trim();
                            string repost_wall_group = "";
                            if (repost_wall_posts.Length > 0 && repost_wall_posts.IndexOf(" ") > 0)
                            {
                                repost_wall_posts = repost_wall_posts.Substring(repost_wall_posts.IndexOf(" ") + 1).Trim();
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
                            }
                            else
                                repost_wall_posts = "";

                            if (repost_wall_group.Length > 0)
                            {
                                if (repost_wall_group.Trim().ToLower().Equals("mywall"))
                                    repost_wall_group = iContUserID.ToString();

                                long _lGroupID = -1;
                                string _initGrpsLst = repost_wall_group;
                                repost_wall_group = ResolveID(repost_wall_group);
                                try
                                {
                                    _lGroupID = Convert.ToInt64(repost_wall_group);
                                }
                                catch
                                {

                                }

                                bool bStatusService = !tbStartService.Enabled;
                                tbStopService_Click(null, null);

                                if (iPersUserID >= 0 && SocialNetwork == 0 && _lGroupID >= 0)
                                {
                                    /*
                                    int _total = 0;
                                    System.Collections.ObjectModel.ReadOnlyCollection<VkNet.Model.Post> posts = null;
                                    try
                                    {
                                        posts = api.Wall.Get(_lGroupID, out _total, 100);
                                    }
                                    catch { }
                                    string _initPostsLst = repost_wall_posts;

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
                                                if (lgid >= 1 && lgid - 1 < _total)
                                                {
                                                    VkNet.Model.Post post = posts[lgid - 1];
                                                    string str = post.ToString();
                                                    try
                                                    {
                                                        api.Likes.Add(VkNet.Enums.SafetyEnums.LikeObjectType.Post, post.Id, post.OwnerId);

                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                    }
                                    */
                                    //if (FormWebBrowserEnabled)
                                    //{
                                    //    FormWebBrowser fwb = new FormWebBrowser(this);
                                    //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.RepostPostFromWallToWall, iPersUserID, _initGrpsLst);
                                    //    fwb.ShowDialog();
                                    //}
                                }

                                if (bStatusService)
                                    tbStartService_Click(null, null);
                            }
                        }
                        else if (cmd_line.StartsWith("repost_group")) // Done!
                        {
                            string repost_wall_posts = labelInMsgHarTitleValue_Text.Trim();
                            string repost_wall_group = "";
                            if (repost_wall_posts.Length > 0 && repost_wall_posts.IndexOf(" ") > 0)
                            {
                                repost_wall_posts = repost_wall_posts.Substring(repost_wall_posts.IndexOf(" ") + 1).Trim();
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
                            }
                            else
                                repost_wall_posts = "";

                            if (repost_wall_group.Length > 0)
                            {
                                long _lGroupID = -1;
                                string _initGrpsLst = repost_wall_group;
                                repost_wall_group = ResolveID(repost_wall_group);
                                try
                                {
                                    _lGroupID = Convert.ToInt64(repost_wall_group);
                                }
                                catch
                                {

                                }

                                bool bStatusService = !tbStartService.Enabled;
                                tbStopService_Click(null, null);

                                if (iPersUserID >= 0 && SocialNetwork == 0 && _lGroupID >= 0)
                                {
                                    /*
                                    int _total = 0;
                                    System.Collections.ObjectModel.ReadOnlyCollection<VkNet.Model.Post> posts = null;
                                    try
                                    {
                                        posts = api.Wall.Get(-_lGroupID, out _total, 100);
                                    }
                                    catch { }
                                    string _initPostsLst = repost_wall_posts;

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
                                                if (lgid >= 1 && lgid - 1 < _total)
                                                {
                                                    VkNet.Model.Post post = posts[lgid - 1];
                                                    string str = post.ToString();
                                                    try
                                                    {
                                                        api.Wall.Repost("wall" + post.OwnerId + "_" + post.Id);
                                                        api.Likes.Add(VkNet.Enums.SafetyEnums.LikeObjectType.Post, post.Id, post.OwnerId);
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                    }
                                    */

                                    //if (FormWebBrowserEnabled)
                                    //{
                                    //    FormWebBrowser fwb = new FormWebBrowser(this);
                                    //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.RepostPostFromGroupToWall, -1, _initGrpsLst, _initPostsLst);
                                    //    fwb.ShowDialog();
                                    //}
                                }

                                if (bStatusService)
                                    tbStartService_Click(null, null);
                            }
                        }//else if (cmd_line.StartsWith("repost_group"))
                        else if (cmd_line.StartsWith("like_group")) // Done!
                        {
                            string repost_wall_posts = labelInMsgHarTitleValue_Text.Trim();
                            string repost_wall_group = "";
                            if (repost_wall_posts.Length > 0 && repost_wall_posts.IndexOf(" ") > 0)
                            {
                                repost_wall_posts = repost_wall_posts.Substring(repost_wall_posts.IndexOf(" ") + 1).Trim();
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
                            }
                            else
                                repost_wall_posts = "";

                            if (repost_wall_group.Length > 0)
                            {
                                long _lGroupID = -1;
                                string _initGrpsLst = repost_wall_group;
                                repost_wall_group = ResolveID(repost_wall_group);
                                try
                                {
                                    _lGroupID = Convert.ToInt64(repost_wall_group);
                                }
                                catch
                                {

                                }

                                bool bStatusService = !tbStartService.Enabled;
                                tbStopService_Click(null, null);

                                if (iPersUserID >= 0 && SocialNetwork == 0 && _lGroupID >= 0)
                                {
                                    /*
                                    int _total = 0;
                                    System.Collections.ObjectModel.ReadOnlyCollection<VkNet.Model.Post> posts = null;
                                    try
                                    {
                                        posts = api.Wall.Get(-_lGroupID, out _total, 100);
                                    }
                                    catch { }

                                    string _initPostsLst = repost_wall_posts;

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
                                                if (lgid >= 1 && lgid - 1 < _total)
                                                {
                                                    VkNet.Model.Post post = posts[lgid - 1];
                                                    string str = post.ToString();
                                                    try
                                                    {
                                                        api.Likes.Add(VkNet.Enums.SafetyEnums.LikeObjectType.Post, post.Id, post.OwnerId);
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                    }
                                    */

                                    //if (FormWebBrowserEnabled)
                                    //{
                                    //    FormWebBrowser fwb = new FormWebBrowser(this);
                                    //    fwb.Setup(userLogin, userPassword, WebBrowserCommand.RepostPostFromGroupToWall, -1, _initGrpsLst, _initPostsLst);
                                    //    fwb.ShowDialog();
                                    //}
                                }

                                if (bStatusService)
                                    tbStartService_Click(null, null);
                            }
                        }//else if (cmd_line.StartsWith("like_group"))

                    }

                    if (moveLastContacterMessageToTop() && (!cmd_line.Equals("wall")) && (!cmd_line.Equals("friends")) && (!cmd_line.StartsWith("attach:")) && (!cmd_line.StartsWith("wallattach:")) && (!cmd_line.StartsWith("time_delay:")))
                    {
                        int perc = 100;
                        if (lblEQOutHarValues[14].Text.IndexOf('%') > 0)
                        {
                            string value = lblEQOutHarValues[14].Text.Substring(0, lblEQOutHarValues[14].Text.IndexOf('%')).Trim();
                            try
                            {
                                perc = Convert.ToInt32(value);

                                if (perc >= 0 && perc <= 100)
                                {
                                    Random rnd = new Random();
                                    int iZero = rnd.Next(0, 100);

                                    if (iZero > 100 - perc)
                                        perc = 0;
                                    else
                                        perc = 100;
                                }
                                else
                                    perc = 100;
                            }
                            catch
                            {
                                perc = 100;
                            }
                        }
                        if (lblEQOutHarValues[14].Text.ToLower().Equals("yes"))
                        {
                            perc = 0;
                        }

                        if (perc > 0)
                        {
                            if (sCurUserPrevMsgText.Length > 0)
                                sCurUserPrevMsgText += " ";
                            sCurUserPrevMsgText += labelOutEqMsgHarTitleValue_Text;
                            sCurUserPrevMsgUID = iContUserID.ToString();
                            sCurUserPrevGroupAnswerID = iGroupAnswerID.ToString();
                            sCurUserPrevGroupAnswerPostID = iGroupAnswerPostID.ToString();
                            sCurUserPrevGroupAnswerCommentID = iGroupAnswerCommentID.ToString();
                            sCurUserPrevName = labelCont1Name.Text + " " + labelCont1Family.Text;
                        }
                        if (sGroupAdditinalUsers.Length > 0)
                            sCurUserGroupAdditinalUsers = sGroupAdditinalUsers;

                    }
                    else
                    {
                        if (sGroupAdditinalUsers.Length > 0)
                            sCurUserGroupAdditinalUsers = sGroupAdditinalUsers;

                        int perc = 100;
                        if (lblEQOutHarValues[14].Text.IndexOf('%') > 0)
                        {
                            string value = lblEQOutHarValues[14].Text.Substring(0, lblEQOutHarValues[14].Text.IndexOf('%')).Trim();
                            try
                            {
                                perc = Convert.ToInt32(value);

                                if (perc >= 0 && perc <= 100)
                                {
                                    Random rnd = new Random();
                                    int iZero = rnd.Next(0, 100);

                                    if (iZero > 100 - perc)
                                        perc = 0;
                                    else
                                        perc = 100;
                                }
                                else
                                    perc = 100;
                            }
                            catch
                            {
                                perc = 100;
                            }
                        }
                        if (lblEQOutHarValues[14].Text.ToLower().Equals("yes"))
                        {
                            perc = 0;
                        }

                        if (perc > 0)
                        {
                            if ((!cmd_line.Equals("wall")) && (!cmd_line.Equals("friends")) && (!cmd_line.StartsWith("attach:")) && (!cmd_line.StartsWith("wallattach:")) && (!cmd_line.StartsWith("time_delay:")))
                            {
                                string str = sCurUserPrevMsgText + " " + labelOutEqMsgHarTitleValue_Text;
                                if (!adbrCurrent.bGroupingOutMessages)
                                {

                                    if (str.IndexOf('^') > 0)
                                    {
                                        string[] sequences = str.Split('^');
                                        foreach (string seq in sequences)
                                        {
                                            lstOutgoingMessages_AddParts(iContUserID.ToString(), labelCont1Name.Text + " " + labelCont1Family.Text, seq + sCurUserGroupAdditinalUsers, "0", iGroupAnswerID, iGroupAnswerPostID, iGroupAnswerCommentID);
                                        }
                                    }
                                    else
                                        lstOutgoingMessages_Insert(iContUserID.ToString(), labelCont1Name.Text + " " + labelCont1Family.Text, str.Replace('^', ' ') + sCurUserGroupAdditinalUsers, "0", iGroupAnswerID, iGroupAnswerPostID, iGroupAnswerCommentID);
                                }
                                else
                                    lstOutgoingMessages_Insert(iContUserID.ToString(), labelCont1Name.Text + " " + labelCont1Family.Text, str.Replace('^', ' ') + sCurUserGroupAdditinalUsers, "0", iGroupAnswerID, iGroupAnswerPostID, iGroupAnswerCommentID);
                                sCurUserPrevMsgUID = "";
                                sCurUserPrevName = "";
                                sCurUserPrevMsgText = "";
                                sCurUserPrevGroupAnswerID = "-1";
                                sCurUserPrevGroupAnswerPostID = "-1";
                                sCurUserPrevGroupAnswerCommentID = "-1";
                                sCurUserGroupAdditinalUsers = "";
                            }
                            else
                            {
                                if (sCurUserPrevMsgText.Length > 0)
                                {
                                    lstOutgoingMessages_Insert(iContUserID.ToString(), labelCont1Name.Text + " " + labelCont1Family.Text, sCurUserPrevMsgText + sCurUserGroupAdditinalUsers, "0", Convert.ToInt64(sCurUserPrevGroupAnswerID), Convert.ToInt64(sCurUserPrevGroupAnswerPostID), Convert.ToInt64(sCurUserPrevGroupAnswerCommentID));
                                    sCurUserPrevMsgUID = "";
                                    sCurUserPrevName = "";
                                    sCurUserPrevMsgText = "";
                                    sCurUserPrevGroupAnswerID = "-1";
                                    sCurUserPrevGroupAnswerPostID = "-1";
                                    sCurUserPrevGroupAnswerCommentID = "-1";
                                    sCurUserGroupAdditinalUsers = "";
                                }

                                lstOutgoingMessages_Insert(iContUserID.ToString(), labelCont1Name.Text + " " + labelCont1Family.Text, labelOutEqMsgHarTitleValue_Text + sGroupAdditinalUsers + (cmd_line.StartsWith("attach:") ? lblEQOutHarValues[11].Text : "") + (cmd_line.StartsWith("wallattach:") ? lblEQOutHarValues[11].Text : "") /*+ (cmd_line.StartsWith("time_delay:") ? lblEQOutHarValues[11].Text : "")*/, (cmd_line.Equals("wall") || cmd_line.StartsWith("wallattach:")) ? "1" : (cmd_line.Equals("friends") ? "2" : "0"), iGroupAnswerID, iGroupAnswerPostID, iGroupAnswerCommentID);
                            }
                        }
                        else
                        {
                            if (cmd_line.Equals("friends"))
                            {
                                if (sCurUserPrevMsgText.Length > 0)
                                {
                                    lstOutgoingMessages_Insert(iContUserID.ToString(), labelCont1Name.Text + " " + labelCont1Family.Text, sCurUserPrevMsgText + sCurUserGroupAdditinalUsers, "0", Convert.ToInt64(sCurUserPrevGroupAnswerID), Convert.ToInt64(sCurUserPrevGroupAnswerPostID), Convert.ToInt64(sCurUserPrevGroupAnswerCommentID));
                                    sCurUserPrevMsgUID = "";
                                    sCurUserPrevName = "";
                                    sCurUserPrevMsgText = "";
                                    sCurUserPrevGroupAnswerID = "-1";
                                    sCurUserPrevGroupAnswerPostID = "-1";
                                    sCurUserPrevGroupAnswerCommentID = "-1";
                                    sCurUserGroupAdditinalUsers = "";
                                }

                                lstOutgoingMessages_Insert(iContUserID.ToString(), labelCont1Name.Text + " " + labelCont1Family.Text, "", "2", iGroupAnswerID, iGroupAnswerPostID, iGroupAnswerCommentID);
                            }
                            else
                                sendPrevUserMessage();
                        }
                    }

                }// not delayed

                if (lstOutgoingMessagesParts.Count > 0)
                {
                    timerPhysicalSendStart();

                    //Random rnd = new Random();
                    //TimerPhysicalSendCycle = rnd.Next(1, DefaultTimerPhysicalSendCycle);
                    //RandDefaultTimerPhysicalSendCycle = TimerPhysicalSendCycle;
                    ///* Timers
                    //pbPhysicalSend.ToolTipText = NilsaUtils.Dictonary_GetText(userInterface, "toolTips_7", this.Name, "Задержка ответа") + " " + Convert.ToString(TimerPhysicalSendCycle) + " " + NilsaUtils.Dictonary_GetText(userInterface, "toolTips_8", this.Name, "сек.");
                    //pbPhysicalSend.Value = (int)(100 * (float)(RandDefaultTimerPhysicalSendCycle - TimerPhysicalSendCycle) / (float)(RandDefaultTimerPhysicalSendCycle));
                    //*/
                    //timerPhysicalSend.Enabled = true;

                    Random rnd = new Random();
                    timerDefaultWriteCycle = rnd.Next(1, timersValues[3]);
                    timerWriteCycle = timerDefaultWriteCycle;
                    progressBarWrite.Value = timerDefaultWriteCycle;
                    progressBarWrite.Invalidate();
                    Application.DoEvents();

                    resetTimerWriteMessagesActivity();
                    timerWriteMessages.Enabled = true;

                }
                else
                {
                    timerPhysicalSendStart();
                    SelectNextReceivedMessage();
                }

                timerAnswerWaitingOn();
            }
        }

        private void api_Messages_Send(long lgid, String msg, String attachlist = "")
        {
            //lstOutgoingMessages_Insert(lgid.ToString(), "-", msg+(attachlist.Length>0 ? ("attach:"+attachlist):""));
            //lstOutgoingMessages_Insert(String uid, String uname, String text, String channel = "0", long _iGroupAnswerID = -1, long _iGroupAnswerPostID = -1, long _iGroupAnswerCommentID = -1)

            String text = NilsaUtils.TextToString(msg.Trim()) + (attachlist.Length > 0 ? ("attach:" + attachlist) : "");
            lstOutgoingMessages.Add("*#|0|" + lgid.ToString() + "|-|" + text.Trim());
        }

        private void sendCommandToOperators(string comvar)
        {
            String soperators = makeTextVariants(lblPersHarValues[12].Text.Trim(), false, false) + ",";
            string[] sOperIDs = soperators.Split(',');

            bool bStatusService = !tbStartService.Enabled;
            tbStopService_Click(null, null);

            if (iPersUserID >= 0 && SocialNetwork == 0)
            {
                string msg = makeTextVariants(comvar, false, false);

                msg += " " + iContUserID.ToString() + " ";
                msg += lblContHarValues[13].Text.Trim();

                foreach (string soperid in sOperIDs)
                {
                    string soper = soperid.Trim();
                    if (soper.Length > 0)
                    {
                        long lgid = -1;
                        try
                        {
                            lgid = Convert.ToInt64(soper);
                        }
                        catch
                        {
                            lgid = -1;
                        }

                        if (lgid >= 0)
                        {
                            api_Messages_Send(lgid, msg);
                            //try
                            //{
                            //    api.Messages.Send(lgid, false, msg); // посылаем сообщение пользователю
                            //}
                            //catch (VkNet.Exception.AccessTokenInvalidException)
                            //{
                            //    ReAutorize(userLogin, userPassword);
                            //}
                            //catch (System.Net.WebException)
                            //{
                            //    ReAutorize(userLogin, userPassword);
                            //}
                            //catch (VkNet.Exception.VkApiException vkapexeption)
                            //{
                            //    ReAutorize(userLogin, userPassword);
                            //}
                            //catch (Exception exp)
                            //{
                            //    ExceptionToLogList("Setup", "UNKNOWN", exp);
                            //}
                        }
                    }
                }
            }

            if (bStatusService)
                tbStartService_Click(null, null);
        }

        List<string> lstOutgoingMessagesParts = new List<string>();

        async Task SleepDelay(int delay)
        {
            await Task.Delay(delay);
        }

        private void Delay(int delay)
        {
            try
            {
                Thread.Sleep(delay);
            }
            catch
            {

            }
        }

        private void tbExit_Click(object sender, EventArgs e)
        {
            onManualButtonClick();
            FormMain_FormClosing_Action = true;

            Close();
        }

        private void tbEditPersoneParametersItemsDesc_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            FormEditPersHar fe = new FormEditPersHar(this);
            fe.sPersHar = new String[iPersHarCount, iPersHarAttrCount];
            for (int i = 0; i < iPersHarCount; i++)
                for (int j = 0; j < iPersHarAttrCount; j++)
                    fe.sPersHar[i, j] = sPersHar[i, j];
            fe.iPersHarAttrCount = iPersHarAttrCount;
            fe.iPersHarCount = iPersHarCount;
            fe.Setup(true, 0);

            if (fe.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < iPersHarCount; i++)
                    for (int j = 0; j < iPersHarAttrCount; j++)
                        sPersHar[i, j] = fe.sPersHar[i, j];
                SavePersoneParametersDescription();
                LoadPersoneParametersDescription();
            }
            StartAnswerTimer();
        }

        private void tbEditContactParametersItemsDesc_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            FormEditPersHar fe = new FormEditPersHar(this);
            fe.sPersHar = new String[iContHarCount, iContHarAttrCount];
            for (int i = 0; i < iContHarCount; i++)
                for (int j = 0; j < iContHarAttrCount; j++)
                    fe.sPersHar[i, j] = sContHar[i, j];
            fe.iPersHarAttrCount = iContHarAttrCount;
            fe.iPersHarCount = iContHarCount;
            fe.Setup(true, 1);

            if (fe.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < iContHarCount; i++)
                    for (int j = 0; j < iContHarAttrCount; j++)
                        sContHar[i, j] = fe.sPersHar[i, j];
                SaveContactParametersDescription();
                LoadContactParametersDescription();
            }
            StartAnswerTimer();
        }

        private void tbEditMessageParametersItemsDesc_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            FormEditPersHar fe = new FormEditPersHar(this);
            fe.sPersHar = new String[iMsgHarCount, iMsgHarAttrCount];
            for (int i = 0; i < iMsgHarCount; i++)
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sPersHar[i, j] = sMsgHar[i, j];

            for (int i = 0; i < iMsgHarCount; i++)
                fe.sPersHar[i, 3] = MsgKoefToString(iMsgHarKoef[i]);

            fe.iPersHarAttrCount = iMsgHarAttrCount;
            fe.iPersHarCount = iMsgHarCount;
            fe.Setup(false, 2);

            if (fe.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < iMsgHarCount; i++)
                    for (int j = 0; j < iMsgHarAttrCount; j++)
                        sMsgHar[i, j] = fe.sPersHar[i, j];
                SaveMessageParametersDescription();
                LoadMessageParametersDescription();
                if (listBoxInMsg.Items.Count > 0)
                    SetEQInMessageParametersValues(sCurrentEQInMessageRecord);
            }
            StartAnswerTimer();
        }

        private void tbEditSettings_ButtonClick(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            tbEditSettings.ShowDropDown();
        }

        bool timerWriteMessagesActivity = true;
        int timerWriteMessagesActivityCount = 6;

        private void resetTimerWriteMessagesActivity()
        {
            timerWriteMessagesActivity = true;
            timerWriteMessagesActivityCount = 6;
        }

        private void timerWriteMessages_Tick(object sender, EventArgs e)
        {
            if (timerWriteMessagesActivity)
            {
                setWriteMessageActivity();
                timerWriteMessagesActivity = false;
            }

            timerWriteMessagesActivityCount--;

            if (timerWriteMessagesActivityCount <= 0)
                resetTimerWriteMessagesActivity();

            timerWriteCycle--;

            int pbvalue = (int)(100 * (float)(timerDefaultWriteCycle - timerWriteCycle) / (float)(timerDefaultWriteCycle));
            if (pbvalue < 0) pbvalue = 0; else if (pbvalue > 100) pbvalue = 100;
            progressBarWrite.Value = pbvalue;
            progressBarWrite.Invalidate();
            Application.DoEvents();

            if (timerWriteCycle <= 0)
            {
                timerWriteMessagesOff();

                if (lstOutgoingMessagesParts.Count > 0)
                {
                    string strpartvalue = lstOutgoingMessagesParts[0];
                    lstOutgoingMessages.Insert(0, strpartvalue);
                    lstOutgoingMessagesParts.RemoveAt(0);
                    timerPhysicalSendStart();

                    if (lstOutgoingMessagesParts.Count == 0)
                        SelectNextReceivedMessage();
                    else
                    {
                        Random rnd = new Random();
                        timerDefaultWriteCycle = rnd.Next(1, timersValues[3]) + (strpartvalue.Trim().Length * 60) / timersValues[4];
                        timerWriteCycle = timerDefaultWriteCycle;
                        progressBarWrite.Value = timerDefaultWriteCycle;
                        progressBarWrite.Invalidate();
                        Application.DoEvents();

                        resetTimerWriteMessagesActivity();
                        timerWriteMessages.Enabled = true;
                    }
                }
                else
                {
                    if (tbSendOutMessage.Enabled)
                        tbSendOutMessage_Click(null, null);
                    else
                        tbSkipMessage_Click(null, null);
                }

            }
        }

        //bool bSendBeforeChange = false;
        private void timerAnswerWaiting_Tick(object sender, EventArgs e)
        {
            //if (timerWriteMessages.Enabled && TimerPersoneChangeCycle <= 1)
            //{
            //    TimerPersoneChangeCycle += TimerSendAnswerCycle + 5;
            //    bSendBeforeChange = true;
            //}

            //if (timerPhysicalSend.Enabled && TimerPersoneChangeCycle <= 1)
            //{
            //    TimerPersoneChangeCycle += TimerPhysicalSendCycle + 5;
            //    bSendBeforeChange = true;
            //}

            //if (bSendBeforeChange && TimerPersoneChangeCycle <= 1)
            //{
            //    TimerPersoneChangeCycle += TimerNewMessagesRereadCycle + DefaultTimerNewMessagesRereadDelayCycle + 5;
            //    bSendBeforeChange = false;
            //}
            timerAnswerWaitingCycle--;

            int pbvalue = (int)(100 * (float)(timerDefaultAnswerWaitingCycle - timerAnswerWaitingCycle) / (float)(timerDefaultAnswerWaitingCycle));
            if (pbvalue < 0) pbvalue = 0; else if (pbvalue > 100) pbvalue = 100;
            progressBarAnswerWaiting.Value = pbvalue;
            progressBarAnswerWaiting.Invalidate();
            Application.DoEvents();
            /* Timers
            pbPersoneChange.ToolTipText = NilsaUtils.Dictonary_GetText(userInterface, "toolTips_9", this.Name, "Смена Персонажа через") + " " + Convert.ToString(TimerPersoneChangeCycle) + " " + NilsaUtils.Dictonary_GetText(userInterface, "toolTips_8", this.Name, "сек.");
            pbPersoneChange.Value = (int)(100 * (float)(DefaultTimerPersoneChangeCycle - TimerPersoneChangeCycle) / (float)(DefaultTimerPersoneChangeCycle));
            */
            if (timerAnswerWaitingCycle <= 0)
            {
                timerAnswerWaitingOff();
                if (lstPersoneChange.Count > 0)
                    bSessionAnswerSended = false;// onChangePersoneByTimer(true, true);
                else
                    timerAnswerWaitingOn();
            }

        }

        private void timerAnswerWaitingOff()
        {
            timerAnswerWaiting.Enabled = false;
            timerAnswerWaitingCycle = 0;
            progressBarAnswerWaiting.Value = 0;
            progressBarAnswerWaiting.Invalidate();
            Application.DoEvents(); 
        }


        private bool savedTimersReadMessages;
        private bool savedTimersWriteMessages;
        private bool savedTimersAnswerWaiting;
        private bool savedTimersCountersStart;
        private bool savedTimersOutgoingPull;
        private bool savedTimersFlashStartButton;
        private bool savedTimersChangePersone;

        private void stopTimers()
        {
            savedTimersReadMessages = timerReadMessages.Enabled;
            timerReadMessages.Enabled = false;

            savedTimersWriteMessages = timerWriteMessages.Enabled;
            timerWriteMessages.Enabled = false;

            savedTimersAnswerWaiting = timerAnswerWaiting.Enabled;
            timerAnswerWaiting.Enabled = false;

            savedTimersCountersStart = timerCountersStart.Enabled;
            timerCountersStart.Enabled = false;

            savedTimersOutgoingPull = timerOutgoingPull.Enabled;
            timerOutgoingPull.Enabled = false;

            savedTimersFlashStartButton = timerFlashStartButton.Enabled;
            timerFlashStartButton.Enabled = false;

            savedTimersChangePersone = timerChangePersone.Enabled;
            timerChangePersone.Enabled = false;
        }

        private void startTimers()
        {
            timerReadMessages.Enabled = savedTimersReadMessages;
            timerWriteMessages.Enabled = savedTimersWriteMessages;
            timerAnswerWaiting.Enabled = savedTimersAnswerWaiting;
            timerCountersStart.Enabled = savedTimersCountersStart;
            timerOutgoingPull.Enabled = savedTimersOutgoingPull;
            timerFlashStartButton.Enabled = savedTimersFlashStartButton;
            timerChangePersone.Enabled = savedTimersChangePersone;

        }

        private void timerAnswerWaitingOn()
        {
            if (!bServiceStart)
                return;

            if (bSessionAnswerSended && !timerWriteMessages.Enabled)
            {
                timerDefaultAnswerWaitingCycle = timersValues[5];
                timerAnswerWaitingCycle = timerDefaultAnswerWaitingCycle;
                progressBarAnswerWaiting.Value = 0;
                progressBarAnswerWaiting.Invalidate();
                Application.DoEvents();

                timerAnswerWaiting.Enabled = true;
            }
        }

        private void setReinitDialogsWhenFreeFlag()
        {
            List<String> lstList = new List<String>();
            lstList.Add("1");
            FileWriteAllLines(Path.Combine(Application.StartupPath, "_reinit_dialogs_when_free_flag.txt"), lstList, Encoding.UTF8);
        }

        private bool checkReinitDialogsWhenFreeFlag()
        {
            return File.Exists(Path.Combine(Application.StartupPath, "_reinit_dialogs_when_free_flag.txt"));
        }

        private void removeReinitDialogsWhenFreeFlag()
        {
            if (File.Exists(Path.Combine(Application.StartupPath, "_reinit_dialogs_when_free_flag.txt")))
                File.Delete(Path.Combine(Application.StartupPath, "_reinit_dialogs_when_free_flag.txt"));
        }

        private void onChangePersoneByTimer(bool bDirection, bool _startservice, bool _bskipReinitDialogs = false)
        {

            setMonitorTime(true);

            if (!_bskipReinitDialogs)
            {
                if (lstPersoneChange.Count > 0 && SocialNetwork == 0)
                {
                    if (ChangeModeWhenFree == 2)
                    {
                        if (!checkChangeModeWhenFreeFlag())
                        {
                            if (iContactsGroupsMode == 1) // Contacts
                            {
                                setChangeModeWhenFreeFlag();
                                //DateTime dt = DateTime.Now;
                                //lstReceivedMessagesContacter.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "INIT_PERSONE_DIALOG");
                                ChangeCommunicationMode(0);
                                Application.DoEvents();
                                Thread.Sleep(500);

                                DateTime dt = DateTime.Now;
                                lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "INIT_PERSONE_DIALOG");
                                tbRefreshPull_Click(null, null);
                                Application.DoEvents();
                                Thread.Sleep(500);
                                tbStartService_Click(null, null);
                                Application.DoEvents();
                                Thread.Sleep(500);


                                //Application.DoEvents();
                                //Thread.Sleep(500);

                                //DateTime dt = DateTime.Now;
                                //lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "INIT_PERSONE_DIALOG");
                                //tbRefreshPull_Click(null, null);

                                //Application.DoEvents();
                                //Thread.Sleep(500);
                                //tbStartService_Click(null, null);
                                //Application.DoEvents();
                                //Thread.Sleep(500);

                                return;
                            }
                            else // Groups
                            {
                                ChangeCommunicationMode(1);
                            }
                        }
                        else
                        {
                            removeChangeModeWhenFreeFlag();
                            if (iContactsGroupsMode == 0) // Contacts
                            {
                                ChangeCommunicationMode(1);
                            }
                        }
                    }
                    else if (ChangeModeWhenFree == 1)
                    {
                        removeChangeModeWhenFreeFlag();
                        if (iContactsGroupsMode == 0) // Contacts
                        {
                            ChangeCommunicationMode(1);
                        }
                    }
                    else if (ChangeModeWhenFree == 0)
                    {
                        removeChangeModeWhenFreeFlag();
                        if (iContactsGroupsMode == 1) // Contacts
                        {
                            ChangeCommunicationMode(0);
                        }
                    }
                }
                //}
            }
            else
            {
                removeReinitDialogsWhenFreeFlag();
                removeChangeModeWhenFreeFlag();
            }
            tbStopService_Click(null, null);

            String sUID = "";
            String sULogin = "";
            String sUPwd = "";

            List<string> lstWork = lstPersoneChange;
            bool randomizeRotatePersonenShuffle = randomizeRotatePersonen == 2;
            if (randomizeRotatePersonenShuffle)
                lstWork = lstPersoneShuffle;

            Random rnd = new Random();
            bool bAutorizeAccept = false;
            while (randomizeRotatePersonenShuffle || lstWork.Count > 1)
            {
                bool bCanPresoneShuffleChange = false;
                if (randomizeRotatePersonenShuffle)
                {
                    foreach (String sPUID in lstPersoneShuffle)
                    {
                        if (sPUID == iPersUserID.ToString())
                            continue;

                        if (checkPersoneWorkTime(sPUID))
                        {
                            bCanPresoneShuffleChange = true;
                            break;
                        }
                    }

                    if (!bCanPresoneShuffleChange)
                    {
                        shufflePersonenLists();

                        foreach (String sPUID in lstWork)
                        {
                            if (sPUID == iPersUserID.ToString())
                                continue;

                            if (checkPersoneWorkTime(sPUID))
                            {
                                bCanPresoneShuffleChange = true;
                                break;
                            }
                        }

                        if (!bCanPresoneShuffleChange)
                        {
                            if (_startservice)
                                tbStartService_Click(null, null);
                            return;
                        }
                    }
                }
                // check can change
                bool bCanPresoneChange = bCanPresoneShuffleChange;
                if (!bCanPresoneChange)
                    foreach (String sPUID in lstWork)
                    {
                        if (sPUID == iPersUserID.ToString())
                            continue;

                        if (checkPersoneWorkTime(sPUID))
                        {
                            bCanPresoneChange = true;
                            break;
                        }
                    }

                if (!bCanPresoneChange)
                {
                    if (_startservice)
                        tbStartService_Click(null, null);
                    return;
                }

                do
                {
                    if (randomizeRotatePersonen == 1)
                    {
                        int uIdx = 0;
                        do
                        {
                            uIdx = rnd.Next(0, lstWork.Count);
                        }
                        while (uIdx >= lstWork.Count);

                        sUID = lstWork[uIdx];
                    }
                    else if (randomizeRotatePersonen == 0)
                    {
                        sUID = lstWork[bDirection ? 0 : lstWork.Count - 1];

                        if (bDirection)
                            lstWork.Add(sUID);
                        else
                            lstWork.Insert(0, sUID);

                        lstWork.RemoveAt(bDirection ? 0 : lstWork.Count - 1);
                    }
                    else if (randomizeRotatePersonen == 2)
                    {
                        sUID = lstWork[0];
                        lstWork.RemoveAt(0);
                    }
                }
                while ((sUID.Equals(iPersUserID.ToString())) || !checkPersoneWorkTime(sUID));

                sULogin = PersonenList_GetUserField(sUID, 2);
                sUPwd = PersonenList_GetUserField(sUID, 3);

                if (sULogin.Length == 0 || sUPwd.Length == 0)
                {
                    if (sUID.Length > 0)
                    {
                        for (int i = 0; i < lstWork.Count; i++)
                            if (sUID.Equals(lstWork[i]))
                            {
                                lstWork.RemoveAt(i);
                                break;
                            }
                    }
                }
                else
                {
                    long curuid = -1;
                    try
                    {
                        curuid=Convert.ToInt64(sUID);
                    }
                    catch
                    {
                        curuid = -1;
                    }

                    bAutorizeAccept = false;
                    if (curuid != -1 && curuid == fwbVKontakte.loggedPersoneID)
                        bAutorizeAccept = true;

                    if (!bAutorizeAccept)
                        bAutorizeAccept = AutorizeVK(sULogin, sUPwd);

                    /*
                    if (SocialNetwork == 0 && FormWebBrowserEnabled && !bAutorizeAccept)
                    {
                        fwbVKontakte.Setup(sULogin, sUPwd, WebBrowserCommand.CheckPersonePage);
                        fwbVKontakte.WaitResult();

                        bAutorizeAccept = AutorizeVK(sULogin, sUPwd);
                    }
                    */

                    List<String> lstContHarCurrent = new List<String>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "pers_" + getSocialNetworkPrefix(SocialNetwork) + sUID + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "pers_" + getSocialNetworkPrefix(SocialNetwork) + sUID + ".txt"));
                            lstContHarCurrent = new List<String>(srcFile);
                            for (int i = 0; i < iContHarCount; i++)
                            {
                                if (i == 14)
                                {
                                    if (SocialNetwork == 0)
                                    {
                                        if (bAutorizeAccept)
                                            lstContHarCurrent[i] = "15|" + "Active";
                                        else
                                        {
                                            string currentStatus = lstContHarCurrent[i].Substring(lstContHarCurrent[i].IndexOf("|") + 1).ToLower();
                                            if (currentStatus.Equals("active"))
                                                lstContHarCurrent[i] = "15|" + "Failed 1";
                                            else if (currentStatus.Equals("blocked"))
                                                lstContHarCurrent[i] = "15|" + "Blocked";
                                            else if (currentStatus.Equals("failed"))
                                                lstContHarCurrent[i] = "15|" + "Failed";
                                            else if (currentStatus.StartsWith("failed "))
                                            {
                                                string currentStatusNumber = currentStatus.Substring(currentStatus.IndexOf(" ") + 1);
                                                int rc = 1;
                                                try
                                                {
                                                    rc = Convert.ToInt32(currentStatusNumber);
                                                    rc++;
                                                }
                                                catch
                                                {
                                                    rc = 11;
                                                }

                                                if (rc >= 11)
                                                {
                                                    copyContactsToMasterPersone(sUID);
                                                    lstContHarCurrent[i] = "15|" + "Failed";
                                                }
                                                else
                                                    lstContHarCurrent[i] = "15|" + "Failed " + rc.ToString();
                                            }
                                            else
                                                lstContHarCurrent[i] = "15|" + "Failed";
                                        }
                                    }
                                    else if (SocialNetwork == 1)
                                        lstContHarCurrent[i] = "15|" + "Active";

                                    break;
                                }
                            }
                            FileWriteAllLines(Path.Combine(FormMain.sDataPath, "pers_" + FormMain.getSocialNetworkPrefix(SocialNetwork) + sUID + ".txt"), lstContHarCurrent, Encoding.UTF8);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        }

                    }

                    if (bAutorizeAccept)
                        break;
                }
            }

            if (!bAutorizeAccept)
                AutorizeVK(userLogin, userPassword);

            setBtnB4(sUID);
            if (sULogin.Length == 0 || sUPwd.Length == 0)
            {
                timerAnswerWaitingOff();
            }
            else
            {
                if (autoUpdateModelFromServer || iLicenseType == LICENSE_TYPE_WORK)
                {
                    autoUpdateModelFTP();
                }

                tbStopService_Click(null, null);
                SaveReceivedMessagesPull();
                SaveOutgoingMessagesPull();
                Setup(sULogin, sUPwd, sUID);
                if (_startservice)
                    tbStartService_Click(null, null);
            }
        }

        private bool checkPersoneWorkTime(String sUD)
        {
            bool defWored = true;
            List<String> lstValues = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + Convert.ToString(sUD) + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + Convert.ToString(sUD) + ".txt"));
                    lstValues = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstValues = new List<String>();
                }
            }
            else
                return defWored;

            String timeDiap = "";
            String activePersoneState = "";
            for (int i = 0; i < lstValues.Count; i++)
            {
                if (!lstValues[i].Contains('|'))
                    continue;

                if (lstValues[i].Substring(0, lstValues[i].IndexOf("|")).Equals("7"))
                {
                    timeDiap = lstValues[i].Substring(lstValues[i].IndexOf("|") + 1);
                }
                else if (lstValues[i].Substring(0, lstValues[i].IndexOf("|")).Equals("15"))
                {
                    activePersoneState = lstValues[i].Substring(lstValues[i].IndexOf("|") + 1).ToLower();
                }
            }

            if (activePersoneState.Equals("failed") || activePersoneState.Equals("blocked"))
                return false;

            if (!timeDiap.Contains('-'))
                return defWored;

            int timeStart = -1;
            int timeEnd = -1;

            try
            {
                timeStart = Convert.ToInt32(timeDiap.Substring(0, timeDiap.IndexOf('-')).Trim());
                timeEnd = Convert.ToInt32(timeDiap.Substring(timeDiap.IndexOf('-') + 1).Trim());

            }
            catch
            {
                timeStart = -1;
                timeEnd = -1;
            }

            if (timeStart < 0 || timeEnd < 0 || timeEnd > 24 || timeStart > 24)
                return defWored;

            bool bExclude = false;
            if (timeStart > timeEnd)
                bExclude = true;

            DateTime dt = DateTime.Now;

            int curHour = dt.Hour;
            if (bExclude)
            {
                if (curHour >= timeEnd && curHour < timeStart)
                    return false;
            }
            else
            {
                if (curHour >= timeStart && curHour < timeEnd)
                    return true;
            }
            return bExclude;
        }


        private void setBtnB4(string str)
        {
            btnB4.Text = "";
            if (lstPersoneChangeOriginal.Count > 0)
            {
                int idx = lstPersoneChangeOriginal.IndexOf(str);
                if (idx >= 0)
                    btnB4.Text = (idx + 1).ToString();
            }
        }

        private void timerReadMessagesOff()
        {
            timerReadMessages.Enabled = false;
            timerReadCycle = 0;
            progressBarRead.Value = 0;
            progressBarRead.Invalidate();
            Application.DoEvents();
        }

        private void timerReadMessagesOn()
        {
            Random rnd = new Random();
            timerDefaultReadCycle = timersValues[6] + rnd.Next(1, timersValues[0]);
            timerReadCycle = timerDefaultReadCycle;
            progressBarRead.Value = 0;
            progressBarRead.Invalidate();
            Application.DoEvents();
            if (doDelayedMessages())
                timerReadMessages.Enabled = true;
        }

        private bool doDelayedMessages()
        {
            if (lstOutgoingMessagesDelayed.Count > 0)
            {
                DateTime dtNow = DateTime.Now;
                int iMsgIdx = 0;
                bool bNeedSave = false;
                bool bPulled = false;
                long _icontid = -1;
                String _message = "";

                while (iMsgIdx < lstOutgoingMessagesDelayed.Count)
                {
                    String srec = lstOutgoingMessagesDelayed[iMsgIdx];
                    int timePos = srec.IndexOf('|');
                    String timeRec = srec.Substring(0, timePos);
                    long timeBinary = -1;
                    try
                    {
                        timeBinary = Convert.ToInt64(timeRec);
                    }
                    catch
                    {
                        timeBinary = -1;
                    }

                    if (timeBinary != -1)
                    {
                        DateTime dt = DateTime.FromBinary(timeBinary);

                        if (dt.CompareTo(dtNow) <= 0)
                        {
                            srec = srec.Substring(timePos + 1);
                            timePos = srec.IndexOf('|');
                            timeRec = srec.Substring(0, timePos);
                            try
                            {
                                _icontid = Convert.ToInt64(timeRec);
                            }
                            catch
                            {
                                _icontid = -1;
                            }

                            if (_icontid != -1)
                                _message = srec.Substring(timePos + 1);

                            lstOutgoingMessagesDelayed.RemoveAt(iMsgIdx);
                            bNeedSave = true;

                            if (_icontid != -1 && _message.Length > 0)
                            {
                                bPulled = true;
                                break;
                            }
                        }
                        else
                            iMsgIdx++;
                    }
                    else
                    {
                        lstOutgoingMessagesDelayed.RemoveAt(iMsgIdx);
                        bNeedSave = true;
                    }
                }

                if (bNeedSave)
                    SaveOutgoingDelayedMessagesPull();

                if (bPulled)
                {
                    //tbStopService_Click(null, null);
                    iContUserID = _icontid;
                    ContactsList_Load();
                    LoadContactParamersValues();
                    SetContactParametersValues();
                    LoadAlgorithmSettingsContacter();

                    OnSelectOtherContacter();

                    tbNewOutMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
                    tbInitContactDialogContacter.Enabled = iPersUserID >= 0 && (iContUserID >= 0 || iContUserID < -1);
                    tbNewInMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
                    tbDeleteContacterMessages.Enabled = iPersUserID >= 0 && iContUserID >= 0;
                    UpdateContactParametersValues_Algorithm();

                    DateTime dt = DateTime.Now;
                    String sCurRec = "0|" + getContUserIDWithGroupID() + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + NilsaUtils.TextToString("Delayed message");
                    iInMsgID = 0;
                    lstReceivedMessages.Add(sCurRec);

                    Set_labelInEqMsgHarTitleValue_Text("Delayed message");
                    Set_labelInMsgHarTitleValue_Text("Delayed message");
                    SetEQOutMessageParametersValues(_message);
                    tbSendOutMessage_Click(null, null);
                    //tbStartService_Click(null, null);

                    //if (!lstEQOutMessagesList.Contains(_message))
                    //{
                    //    listBoxOutMsg.SelectedIndex = -1;
                    //    lstEQOutMessagesList.Insert(0, _message);
                    //    listBoxOutMsg.Items.Insert(0, "?% " + GetMessageTextWithMarker(_message.Substring(_message.IndexOf("|@!") + 3)));
                    //    listBoxOutMsg.SelectedIndex = 0;
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        listBoxOutMsg.SelectedIndex = lstEQOutMessagesList.IndexOf(_message);
                    //    }
                    //    catch
                    //    {
                    //        listBoxOutMsg.SelectedIndex = -1;
                    //    }
                    //}
                    //timerWriteMessagesOn();
                    return false;
                }
            }
            return true;
        }

        private void removeDelayedMessages(long _icontusrid)
        {
            if (lstOutgoingMessagesDelayed.Count > 0)
            {
                int iMsgIdx = 0;
                bool bNeedSave = false;
                long _icontid = -1;

                while (iMsgIdx < lstOutgoingMessagesDelayed.Count)
                {
                    String srec = lstOutgoingMessagesDelayed[iMsgIdx];
                    int timePos = srec.IndexOf('|');
                    String timeRec;
                    long timeBinary = -1;

                    if (timePos > 0)
                    {
                        timeRec = srec.Substring(0, timePos);

                        try
                        {
                            timeBinary = Convert.ToInt64(timeRec);
                        }
                        catch
                        {
                            timeBinary = -1;
                        }
                    }

                    if (timeBinary != -1)
                    {
                        srec = srec.Substring(timePos + 1);
                        timePos = srec.IndexOf('|');
                        if (timePos > 0)
                        {
                            timeRec = srec.Substring(0, timePos);
                            try
                            {
                                _icontid = Convert.ToInt64(timeRec);
                            }
                            catch
                            {
                                _icontid = -1;
                            }

                            if (_icontid == _icontusrid)
                            {
                                lstOutgoingMessagesDelayed.RemoveAt(iMsgIdx);
                                bNeedSave = true;
                            }
                            else
                                iMsgIdx++;
                        }
                        else
                        {
                            lstOutgoingMessagesDelayed.RemoveAt(iMsgIdx);
                            bNeedSave = true;
                        }
                    }
                    else
                    {
                        lstOutgoingMessagesDelayed.RemoveAt(iMsgIdx);
                        bNeedSave = true;
                    }
                }

                if (bNeedSave)
                    SaveOutgoingDelayedMessagesPull();
            }
        }

        private void timerReadMessages_Tick(object sender, EventArgs e)
        {
            timerReadCycle--;

            int pbvalue = (int)(100 * (float)(timerDefaultReadCycle - timerReadCycle) / (float)(timerDefaultReadCycle));
            if (pbvalue < 0) pbvalue = 0; else if (pbvalue > 100) pbvalue = 100;
            progressBarRead.Value = pbvalue;
            progressBarRead.Invalidate();
            Application.DoEvents();

            if (timerReadCycle <= 0)
            {
                timerReadMessagesOff();
                bool bNotChanged = true;
                if (lstReceivedMessages.Count > 0)
                {
                    ReadNewReceivedMessages();
                    if (iInMsgID == -1)
                        SelectNextReceivedMessage(false);
                }
                else
                {
                    tbSkipMessage_Click(null, null);
                    if (!bSessionAnswerSended && (lstPersoneChange.Count > 0) && (lstReceivedMessages.Count == 0) && SocialNetwork == 0 && bServiceStart)
                    {
                        bNotChanged = false;
                        onChangePersoneByTimer(true, true);
                    }
                }


                if (bNotChanged && iContUserID != -1)
                    ReadAllUserMessages(iContUserID);

            }

        }

        private void timerWriteMessagesOff()
        {
            timerWriteMessages.Enabled = false;
            timerWriteCycle = 0;
            if (progressBarWrite != null)
            {
                progressBarWrite.Value = 0;
                progressBarWrite.Invalidate();
                Application.DoEvents();
            }
        }

        private void timerWriteMessagesOn()
        {
            if (!bServiceStart)
                return;

            if (labelInEqMsgHarTitleValue_Text.Trim().Length > 0 || labelOutEqMsgHarTitleValue_Text.Trim().Length > 0)
            {
                if (labelOutEqMsgHarTitleValue_Text.Trim().Length > 0 && tbSendOutMessage.Enabled)
                    timerAnswerWaitingOff();
                Random rnd = new Random();
                timerDefaultWriteCycle = (labelInEqMsgHarTitleValue_Text.Trim().Length * 60) / timersValues[1] + rnd.Next(1, timersValues[3]) + (labelOutEqMsgHarTitleValue_Text.Trim().Length > 0 && tbSendOutMessage.Enabled ? ((labelOutEqMsgHarTitleValue_Text.Trim().Length * 60) / timersValues[4] + timersValues[2]) : timersValues[2]);
                timerWriteCycle = timerDefaultWriteCycle;
                progressBarWrite.Value = 0;
                progressBarWrite.Invalidate();
                Application.DoEvents();

                resetTimerWriteMessagesActivity();
                timerWriteMessages.Enabled = true;
            }
        }

        public void setWriteMessageActivity()
        {
            if (SocialNetwork == 0 && iContUserID > 0)
            {
                try
                {
                    //api.Messages.SetActivity(iContUserID);
                }
                catch
                {

                }
            }
        }

        public void tbStartService_Click(object sender, EventArgs e)
        {
            SaveProgramState();
            setMonitorTime(true);

            timerFlashStartButton.Enabled = false;
            tbStartService.Image = global::Nilsa.Properties.Resources.start_green;

            tbStartService.Enabled = false;
            tbStopService.Enabled = true;

            timerCountersStart.Enabled = true;

            if (timerChangePersoneCycle > 0 && lstPersoneChange.Count > 0 && SocialNetwork == 0) timerChangePersone.Enabled = true;

            bServiceStart = true;
            timerAnswerWaitingOn();
            timerWriteMessagesOn();
            //if (tbSendOutMessage.Enabled)
            //{
            //    TimerSendAnswerCycle = DefaultTimerSendAnswerCycle;
            //    Set_pbSendMessage_Value();
            //    timerWriteMessages.Enabled = true;
            //    timerPhysicalSend.Enabled = false;
            //}
            //else
            //    if (iInMsgID >= 0) timerSkipMessage.Enabled = true;

            if (lstReceivedMessages.Count == 0)
                timerReadMessagesOn();

            timerPhysicalSendStart();

            tbNewOutMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            tbInitContactDialogContacter.Enabled = iPersUserID >= 0 && (iContUserID >= 0 || iContUserID < -1);
            tbNewInMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            tbDeleteContacterMessages.Enabled = iPersUserID >= 0 && iContUserID >= 0;

            if (iInMsgID < 0)
                tbSkipMessage_Click(null, null);
        }

        public void tbStopService_Click(object sender, EventArgs e)
        {
            bServiceStart = false;
            tbStartService.Enabled = true;
            tbStopService.Enabled = false;
            timerChangePersone.Enabled = false;

            //timerWriteMessages.Enabled = false;
            timerReadMessagesOff();
            //timerSkipMessage.Enabled = false;
            //timerAnswerWaiting.Enabled = false;
            timerAnswerWaitingOff();

            timerPhysicalSendStop();
            SaveProgramCountersC1C2C3();
            SaveProgramCountersC4C5C6();
            timerCountersStart.Enabled = false;
            //Set_pbSkipMessage_Default();
            timerWriteMessagesOff();

            /* Timers
            pbPersoneChange.ToolTipText = NilsaUtils.Dictonary_GetText(userInterface, "toolTips_10", this.Name, "Смена Персонажа");
            pbPersoneChange.Value = 0;
            */

            timerFlashStartButton.Enabled = true;
            tbStartService.Image = global::Nilsa.Properties.Resources.start_green;
        }

        //private void timerSkipMessage_Tick(object sender, EventArgs e)
        //{
        //    TimerSkipMessageCycle--;
        //    Set_pbSkipMessage_Value();
        //    if (TimerSkipMessageCycle <= 0)
        //    {
        //        timerSkipMessage.Enabled = false;
        //        Set_pbSkipMessage_Default();
        //        tbSkipMessage_Click(null, null);
        //    }
        //}

        bool FormMain_FormClosing_Action = false;
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormMain_FormClosing_Action)
            {
                if (fwbVKontakte != null)
                {
                    if (fwbVKontakte.Visible)
                    {
                        fwbVKontakte.saveSettings();
                        fwbVKontakte.exitBrowser = true;
                        fwbVKontakte.stopAllTimers(); 
                        fwbVKontakte.Hide();
                    }
                }

                setMonitorTime(false);
                SaveProgramCountersC1C2C3();
                SaveProgramCountersC4C5C6();
                tbStopService_Click(null, null);
                SaveReceivedMessagesPull();
                SaveOutgoingMessagesPull();
                SaveProgramSettings();

                killNILSAMonitor();
            }
        }

        #region "Refresh Notification Area Icons"

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);


        public void RefreshTrayArea()
        {
            IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
            IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            IntPtr notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", null);
            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", null);
                IntPtr notifyIconOverflowWindowHandle = FindWindow("NotifyIconOverflowWindow", null);
                IntPtr overflowNotificationAreaHandle = FindWindowEx(notifyIconOverflowWindowHandle, IntPtr.Zero, "ToolbarWindow32", "Overflow Notification Area");
                RefreshTrayArea(overflowNotificationAreaHandle);
            }
            RefreshTrayArea(notificationAreaHandle);
        }


        private static void RefreshTrayArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            RECT rect;
            GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
                for (var y = 0; y < rect.bottom; y += 5)
                    SendMessage(windowHandle, wmMousemove, 0, (y << 16) + x);
        }
        #endregion
        private void killNILSAMonitor()
        {
            if (isNILSAMonitorRunned())
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "NILSA_Monitor.exe")))
                    Process.Start(Path.Combine(Application.StartupPath, "NILSA_Monitor.exe"), "-exit");

                try
                {
                    Thread.Sleep(500);
                }
                catch
                {

                }
                RefreshTrayArea();
            }
        }

        private void tbDeleteMessage_Click(object sender, EventArgs e)
        {
            onManualButtonClick();
            // manual set timers
            StopAnswerTimer();

            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_16", this.Name, "Удалить текущее сообщение из пула?"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_17", this.Name, "Удаление сообщения"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                SelectNextReceivedMessage();

            StartAnswerTimer();
        }

        private void tbRefreshPull_Click(object sender, EventArgs e)
        {
            if (iContactsGroupsMode == 0)
            {
                timerReadCycle = 1;
                timerReadMessages_Tick(null, null);
            }
            else
            {
                tbRefreshPullGroups_Click(true);
            }
        }

        private void tbInitContactDialog_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            if (iContactsGroupsMode == 0)
                tbEditContactsDB_Click(sender, e);
            else
                tbEditGroupsDB_Click(sender, e);
        }

        public static DialogResult InputBox(Form owner, string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            textBox.TextChanged += (o, ep) => buttonOk.Enabled = textBox.Text.Length > 0;
            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonOk.Enabled = false;
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            form.ShowInTaskbar = true;
            form.Owner = owner;
            form.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void tbChangePersone_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_18", this.Name, "Вы уверены, что хотите сменить Персонаж?"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_19", this.Name, "Смена персонажа"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                tbStopService_Click(null, null);
                SaveReceivedMessagesPull();
                SaveOutgoingMessagesPull();
                Setup();
            }
        }

        private bool INIT_PERSONE_DIALOG = false;
        private void tbEditContactsDB_Click(object sender, EventArgs e)
        {
            // manual set timers
            StopAnswerTimer();

            if (iContUserID != iPersUserID && iContUserID != 330643598 && iContUserID >= 0 && iContactsGroupsMode == 0)
            {
                if (ContactsList_GetUserIdx(iContUserID.ToString()) < 0)
                    ContactsList_AddUser(iContUserID.ToString(), contName/*cbCont1.SelectedItem.ToString()*/);
            }

            bool prevTopmost = false;
            if (fwbVKontakte != null)
            {
                prevTopmost = fwbVKontakte.TopMost;
                fwbVKontakte.TopMost = true;
            }

            FormEditContactsDB fe = new FormEditContactsDB(this);
            fe.sContHar = new String[iContHarCount, iContHarAttrCount + 1];
            for (int i = 0; i < iContHarCount; i++)
            {
                for (int j = 0; j < iContHarAttrCount; j++)
                    fe.sContHar[i, j] = sContHar[i, j];
                fe.sContHar[i, iContHarAttrCount] = "";
            }
            fe.iContHarCount = iContHarCount;
            fe.iContHarAttrCount = iContHarAttrCount;
            fe.Setup(iPersUserID.ToString() + " (" + userLogin + ", " + userPassword + ")", iPersUserID, (iContUserID >= 0 ? iContUserID : 0), INIT_PERSONE_DIALOG);

            try
            {
                fe.ShowDialog();
                Application.DoEvents();

                if (fwbVKontakte != null)
                {
                    fwbVKontakte.TopMost = prevTopmost;
                }
            }
            catch
            {

            }
            UpdateProgramCountersInfoB2B5B6();

            if (fe.bNeedPersoneChange && fe.iContUserID >= 0)
            {
                tbStopService_Click(null, null);
                iContUserID = fe.iContUserID;
                ContactsList_Load();
                LoadContactParamersValues();
                SetContactParametersValues();
                LoadAlgorithmSettingsContacter();
                //LoadContactParametersDescription();

                OnSelectOtherContacter();
            }
            else
            {
                ContactsList_Load();
                LoadContactParamersValues();
                SetContactParametersValues();
                LoadAlgorithmSettingsContacter();
                //LoadContactParametersDescription();
                //comboBoxCompareVectorLevel_SelectedIndexChanged(sender, e);
                comboBoxCompareLexicalLevel_SelectedIndexChanged(sender, e);
                if (!INIT_PERSONE_DIALOG && fe.bInitDialogs)
                    SelectNextReceivedMessage(false);

                INIT_PERSONE_DIALOG = false;
                StartAnswerTimer();
            }
            tbNewOutMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            tbInitContactDialogContacter.Enabled = iPersUserID >= 0 && (iContUserID >= 0 || iContUserID < -1);
            tbNewInMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            tbDeleteContacterMessages.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            UpdateContactParametersValues_Algorithm();
        }

        private void onAfterPersonenListChanged()
        {
            setRandomizeRotatePersonenButtonIcon();
            toolStripMenuItemClearInMsgPullPersonen.Enabled = lstPersoneChange.Count > 0 && SocialNetwork == 0;
            toolStripButtonPersoneForward.Enabled = lstPersoneChange.Count > 0 && SocialNetwork == 0;
            toolStripButtonPersoneRewind.Enabled = lstPersoneChange.Count > 0 && SocialNetwork == 0;
            toolStripButtonPersonePause.Enabled = false;// lstPersoneChange.Count > 0 && SocialNetwork == 0;
            toolStripButtonPersonePlay.Enabled = false;// lstPersoneChange.Count > 0 && SocialNetwork == 0;

            toolStripButtonContacterForward.Enabled = false;// lstPersoneChange.Count > 0 && SocialNetwork == 0;
            toolStripButtonContacterRewind.Enabled = false;// lstPersoneChange.Count > 0 && SocialNetwork == 0;
            toolStripButtonContacterPause.Enabled = false;// lstPersoneChange.Count > 0 && SocialNetwork == 0;
            toolStripButtonContacterPlay.Enabled = false;// lstPersoneChange.Count > 0 && SocialNetwork == 0;

            button2.Text = (lstPersoneChange.Count > 0 ? lstPersoneChange.Count.ToString() : "");
        }

        private void tbEditPresonenDB_Click(object sender, EventArgs e)
        {
            // manual set timers
            StopAnswerTimer();

            bool prevTopmost = false;
            if (fwbVKontakte != null)
            {
                prevTopmost = fwbVKontakte.TopMost;
                fwbVKontakte.TopMost = true;
            }

            FormEditPersonenDB fe = new FormEditPersonenDB(this);
            fe.sContHar = new String[iPersHarCount, iPersHarAttrCount + 1];
            for (int i = 0; i < iPersHarCount; i++)
            {
                for (int j = 0; j < iPersHarAttrCount; j++)
                    fe.sContHar[i, j] = sPersHar[i, j];
                fe.sContHar[i, iPersHarAttrCount] = "";
            }
            fe.iContHarCount = iPersHarCount;
            fe.iContHarAttrCount = iPersHarAttrCount;
            fe.Setup(iPersUserID.ToString() + " (" + userLogin + ", " + userPassword + ")", iPersUserID, true);

            if (lstPersoneChange.Count > 0)
            {
                foreach (String str in lstPersoneChange)
                    for (int i = 0; i < fe.lvList.Items.Count; i++)
                    {
                        if (fe.lvList.Items[i].SubItems[1].Text == str)
                        {
                            fe.lvList.Items[i].Checked = true;
                            break;
                        }
                    }
            }

            fe.ShowDialog();

            try
            {
                if (fwbVKontakte != null)
                    fwbVKontakte.TopMost = prevTopmost;
            }
            catch
            {

            }

            LoadPersoneParametersValues();
            UpdatePersoneParametersValues_Algorithm();
            UpdateProgramCountersInfoB2B5B6();

            initPersonenLists(fe);

            if (fe.bNeedPersoneChange)
            {
                tbStopService_Click(null, null);
                SaveReceivedMessagesPull();
                SaveOutgoingMessagesPull();
                if (fe.suSelSocialNetwork != SocialNetwork)
                {
                    SocialNetwork = fe.suSelSocialNetwork;
                    OnSocialNetworkChanged();
                }
                onAfterPersonenListChanged();
                Setup(fe.suSelLogin, fe.suSelPwd, fe.suSelID);
            }
            else if (fe.bNeedPersoneReread)
            {
                tbStopService_Click(null, null);
                SaveReceivedMessagesPull();
                SaveOutgoingMessagesPull();
                onAfterPersonenListChanged();
                Setup(userLogin, userPassword, userID);
            }
            else
            {
                PersonenList_Load();
                userSelectUserIdx = PersonenList_GetUserIdx(iPersUserID.ToString()) + 1;
                if (userSelectUserIdx > 0)
                {
                    LoadPersoneParametersValues();
                    SetPersoneParametersValues();
                    //LoadPersoneParametersDescription();
                    StartAnswerTimer();
                    onAfterPersonenListChanged();
                }
                else
                {
                    tbStopService_Click(null, null);
                    SaveReceivedMessagesPull();
                    SaveOutgoingMessagesPull();

                    //timerWriteMessages.Enabled = false;
                    //timerSkipMessage.Enabled = false;
                    //Set_pbSkipMessage_Default();
                    timerWriteMessagesOff();
                    iInMsgID = -1;
                    iContUserID = -1;
                    //labelInMsgHarDateValue.Text = "";
                    //labelInMsgHarTimeValue.Text = "";
                    Set_labelInMsgHarTitleValue_Text("");
                    comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                    listBoxUserMessages.Items.Clear();
                    listBoxInMsg.Items.Clear();
                    listBoxOutMsg.Items.Clear();
                    //buttonEditContHarValues.Enabled = false;
                    //buttonEditInMsgHar.Enabled = false;
                    //buttonEditInEqMsgHar.Enabled = false;
                    //buttonEditOutEqMsgHar.Enabled = false;
                    tbSkipMessage.Enabled = false;
                    tbDeleteMessage.Enabled = false;
                    tbSendOutMessage.Enabled = false;
                    //tbDeleteInEQMessage.Enabled = false;
                    //tbDeleteOutEQMessage.Enabled = false;
                    Set_labelInEqMsgHarTitleValue_Text("");
                    CompareVetors_RestoreDefaultValues();
                    clearlblEQInHarValues();
                    Set_labelOutEqMsgHarTitleValue_Text("");

                    clearlblEQOutHarValues();
                    contName = contNameFamily = contNameName = "";
                    labelCont1Family.Text = "";
                    labelCont1Name.Text = "";
                    labelCont1FIO.Text = "";
                    toolTipMessage.SetToolTip(labelCont1FIO, labelCont1FIO.Text);

                    for (int i = 0; i < iContHarCount; i++)
                    {
                        lblContHarValues[i].Text = "";
                        //toolTipMessage.SetToolTip(lblContHarNames[i], sContHar[i, 1]);
                        toolTipMessage.SetToolTip(lblContHarValues[i], sContHar[i, 1]);
                    }
                    //labelPers1.Text = "";
                    labelPers1Name.Text = "";
                    labelPers1Family.Text = "";
                    labelPers1FIO.Text = "";
                    toolTipMessage.SetToolTip(labelPers1FIO, labelPers1FIO.Text);
                    //cbPers1.SelectedIndex = -1;
                    for (int i = 0; i < iContHarCount; i++)
                    {
                        lblPersHarValues[i].Text = "";
                        //toolTipMessage.SetToolTip(lblPersHarNames[i], sPersHar[i, 1]);
                        toolTipMessage.SetToolTip(lblPersHarValues[i], sPersHar[i, 1]);
                    }

                    sCurrentEQInMessageRecord = "";
                    sCurrentEQOutMessageRecord = "";
                    sCurrentEQOutMessageRecordOut = "";
                    //this.Text = "NILSA, v" + Application.ProductVersion + ((bDebugVersion || showSessionStartTime) ? " (" + sessionStartTime + ")" : "");
                    setStandardCaption();
                    onAfterPersonenListChanged();
                    Setup();
                }
            }
        }

        private void buttonEditPersHarValues_Click_1(object sender, EventArgs e)
        {
            onManualButtonClick();

            tbEditPresonenDB_Click(sender, e);
        }

        private void buttonEditContHarValues_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            if (iContactsGroupsMode == 0)
                tbEditContactsDB_Click(sender, e);
            else
                tbEditGroupsDB_Click(sender, e);
        }


        private void comboBoxCompareLexicalLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (labelInMsgHarTitleValue_Text.Length > 0 && comboBoxCompareLexicalLevel.Text != null)
            {
                comboBoxCompareLexicalLevel.SelectionStart = comboBoxCompareLexicalLevel.Text.Length;
                comboBoxCompareLexicalLevel.SelectionLength = 0;
                SetEQInMessageList(labelInMsgHarTitleValue_Text);
            }
        }

        private void comboBoxCompareVectorLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (labelInEqMsgHarTitleValue_Text.Length > 0)
            {
                comboBoxCompareVectorLevel.SelectionStart = comboBoxCompareVectorLevel.Text.Length;
                comboBoxCompareVectorLevel.SelectionLength = 0;
                SetEQOutMessageList(sCurrentEQInMessageRecord);
            }
        }

        private void ChangeVectorKoef(int idx, int value)
        {
            if (labelInEqMsgHarTitleValue_Text.Length > 0)
            {
                iCompareVectorsKoef[idx] = value;
                //lblEQInHarNames[idx].Text = MsgKoefToString(iCompareVectorsKoef[idx]);
                toolTipMessage.SetToolTip(lblEQInHarNames[idx], MsgKoefToString(iCompareVectorsKoef[idx]));
                lblEQInHarNames[idx].OwnerDrawText = MsgKoefToImage(iCompareVectorsKoef[idx]);

                SetEQOutMessageList(sCurrentEQInMessageRecord);
            }
        }

        private void ChangeVectorKoefOut(int idx, int value)
        {
            if (iContUserID >= 0)
            {
                iCompareVectorsKoefOut[idx] = value;
                //lblEQOutHarNames[idx].Text = MsgKoefToString(iCompareVectorsKoefOut[idx]);
                toolTipMessage.SetToolTip(lblEQOutHarNames[idx], MsgKoefToString(iCompareVectorsKoefOut[idx]));
                lblEQOutHarNames[idx].OwnerDrawText = MsgKoefToImage(iCompareVectorsKoefOut[idx]);

                SetEQOutMessageListOut(sCurrentEQOutMessageRecordOut);
            }
        }


        private void contextMenuStripVectorKoef_Click(object sender, EventArgs e)
        {
            ChangeVectorKoef((Int32)(contextMenuStripVectorKoef.Tag), (Int32)(((ToolStripMenuItem)sender).Tag));
        }

        private void contextMenuStripVectorKoefOut_Click(object sender, EventArgs e)
        {
            ChangeVectorKoefOut((Int32)(contextMenuStripVectorKoefOut.Tag), (Int32)(((ToolStripMenuItem)sender).Tag));
        }

        private void неВажноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            ChangeVectorKoef((Int32)(contextMenuStripVectorKoef.Tag), 0);
        }

        private void важноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            ChangeVectorKoef((Int32)(contextMenuStripVectorKoef.Tag), 1);
        }

        private void оченьВажноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onManualButtonClick(); 

            ChangeVectorKoef((Int32)(contextMenuStripVectorKoef.Tag), 100);
        }

        void lblEQInHarValues_MouseClick(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;

            if (lbl != null)
            {
                if (labelInEqMsgHarTitleValue_Text.Length > 0)
                {
                    bool bShow = false;
                    int idx = (Int32)(lbl.Tag);
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + getMessagesDBThemeID(idx, lblEQInHarValues[0].Text) + (idx + 1).ToString() + ".txt")))
                    {
                        List<String> lstValues = new List<String>();
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + getMessagesDBThemeID(idx, lblEQInHarValues[0].Text) + (idx + 1).ToString() + ".txt"));
                            lstValues = new List<String>(srcFile);
                        }
                        catch (Exception exp)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", exp);

                            lstValues = new List<String>();
                        }

                        contextMenuStripEQInMsgValues.Items.Clear();
                        contextMenuStripEQInMsgValues.Tag = (Int32)(lbl.Tag);

                        System.Windows.Forms.ToolStripMenuItem tsmiItem = new System.Windows.Forms.ToolStripMenuItem();
                        tsmiItem.Name = "lblEQInHarValuestsmiItemZero";
                        tsmiItem.Size = new System.Drawing.Size(147, 22);
                        tsmiItem.Text = "";
                        tsmiItem.Click += contextMenuStripEQInMsgValuesItem_Click;

                        contextMenuStripEQInMsgValues.Items.Add(tsmiItem);

                        for (int i = 0; i < lstValues.Count; i++)
                        {
                            tsmiItem = new System.Windows.Forms.ToolStripMenuItem();
                            tsmiItem.Name = "lblEQInHarValuestsmiItem" + i.ToString();
                            tsmiItem.Size = new System.Drawing.Size(147, 22);
                            tsmiItem.Text = lstValues[i];
                            tsmiItem.Click += contextMenuStripEQInMsgValuesItem_Click;

                            contextMenuStripEQInMsgValues.Items.Add(tsmiItem);
                        }
                        bShow = true;
                        contextMenuStripEQInMsgValues.Show(lbl.PointToScreen(new Point(e.X, e.Y)));
                    }
                    if (!bShow)
                        toolTipMessage.Show(NilsaUtils.Dictonary_GetText(userInterface, "toolTips_16", this.Name, "Нет возможных значений характеристик..."), this, lbl.PointToScreen(new Point(e.X, e.Y)), 1000);

                }
            }
        }

        void lblEQOutHarValues_MouseClick(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;

            if (lbl != null)
            {
                if (iContUserID >= 0)
                {
                    bool bShow = false;
                    int idx = (Int32)(lbl.Tag);
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_har_" + getMessagesDBThemeID(idx, lblEQOutHarValues[0].Text) + (idx + 1).ToString() + ".txt")))
                    {
                        List<String> lstValues = new List<String>();
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_" + getMessagesDBThemeID(idx, lblEQOutHarValues[0].Text) + (idx + 1).ToString() + ".txt"));
                            lstValues = new List<String>(srcFile);
                        }
                        catch (Exception exp)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", exp);

                            lstValues = new List<String>();
                        }

                        contextMenuStripEQOutMsgValues.Items.Clear();
                        contextMenuStripEQOutMsgValues.Tag = (Int32)(lbl.Tag);

                        System.Windows.Forms.ToolStripMenuItem tsmiItem = new System.Windows.Forms.ToolStripMenuItem();
                        tsmiItem.Name = "lblEQOutHarValuestsmiItemZero";
                        tsmiItem.Size = new System.Drawing.Size(147, 22);
                        tsmiItem.Text = "";
                        tsmiItem.Click += contextMenuStripEQOutMsgValuesItem_Click;

                        contextMenuStripEQOutMsgValues.Items.Add(tsmiItem);

                        for (int i = 0; i < lstValues.Count; i++)
                        {
                            tsmiItem = new System.Windows.Forms.ToolStripMenuItem();
                            tsmiItem.Name = "lblEQOutHarValuestsmiItem" + i.ToString();
                            tsmiItem.Size = new System.Drawing.Size(147, 22);
                            tsmiItem.Text = lstValues[i];
                            tsmiItem.Click += contextMenuStripEQOutMsgValuesItem_Click;

                            contextMenuStripEQOutMsgValues.Items.Add(tsmiItem);
                        }
                        bShow = true;
                        contextMenuStripEQOutMsgValues.Show(lbl.PointToScreen(new Point(e.X, e.Y)));
                    }
                    if (!bShow)
                        toolTipMessage.Show(NilsaUtils.Dictonary_GetText(userInterface, "toolTips_16", this.Name, "Нет возможных значений характеристик..."), this, lbl.PointToScreen(new Point(e.X, e.Y)), 1000);

                }
            }
        }

        void contextMenuStripEQInMsgValuesItem_Click(object sender, EventArgs e)
        {
            if (labelInEqMsgHarTitleValue_Text.Length > 0)
            {
                int idx = (Int32)(contextMenuStripEQInMsgValues.Tag);
                String text = ((System.Windows.Forms.ToolStripMenuItem)sender).Text;
                lblEQInHarValues[idx].Text = text;
                toolTipMessage.SetToolTip(lblEQInHarValues[idx], text);
                String value = sCurrentEQInMessageRecord;
                sCurrentEQInMessageRecord = value.Substring(0, value.IndexOf("|") + 1);

                for (int i = 0; i < iMsgHarCount; i++)
                {
                    value = value.Substring(value.IndexOf("|") + 1);
                    String s = value.Substring(0, value.IndexOf("|"));
                    if (i == idx)
                    {
                        s = text;
                        if (s.Length == 0)
                        {
                            if (sMsgHar[i, 2].IndexOf("*") > 0)
                            {
                                String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("*") + 1);
                                for (int j = 0; j < iContHarCount; j++)
                                {
                                    if (sContHar[j, 0].Equals(s1))
                                    {
                                        s = GetContactParametersValue(sContHar[j, 0]);
                                        lblEQInHarValues[idx].Text = s;
                                        toolTipMessage.SetToolTip(lblEQInHarValues[idx], s);
                                        break;
                                    }
                                }
                            }
                            else if (sMsgHar[i, 2].IndexOf("#") > 0)
                            {
                                String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("#") + 1);
                                for (int j = 0; j < iPersHarCount; j++)
                                {
                                    if (sPersHar[j, 0].Equals(s1))
                                    {
                                        s = GetPersoneParametersValue(sPersHar[j, 0]);
                                        lblEQInHarValues[idx].Text = s;
                                        toolTipMessage.SetToolTip(lblEQInHarValues[idx], s);
                                        break;
                                    }
                                }
                            }

                        }
                    }
                    sCurrentEQInMessageRecord += s + "|";
                }
                sCurrentEQInMessageRecord += value.Substring(value.IndexOf("|") + 1);

                SetEQOutMessageList(sCurrentEQInMessageRecord);
            }
        }

        void contextMenuStripEQOutMsgValuesItem_Click(object sender, EventArgs e)
        {
            if (iContUserID >= 0)
            {
                int idx = (Int32)(contextMenuStripEQOutMsgValues.Tag);
                String text = ((System.Windows.Forms.ToolStripMenuItem)sender).Text;
                lblEQOutHarValues[idx].Text = text;
                toolTipMessage.SetToolTip(lblEQOutHarValues[idx], text);
                String value = sCurrentEQOutMessageRecordOut;
                if (value.Length == 0)
                {
                    value = "000000|" + strDB0Name + "||||||||||||||||@!-";
                }
                sCurrentEQOutMessageRecordOut = value.Substring(0, value.IndexOf("|") + 1);

                for (int i = 0; i < iMsgHarCount; i++)
                {
                    value = value.Substring(value.IndexOf("|") + 1);
                    String s = value.Substring(0, value.IndexOf("|"));
                    if (i == idx)
                    {
                        s = text;
                        if (s.Length == 0)
                        {
                            if (sMsgHar[i, 2].IndexOf("*") > 0)
                            {
                                String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("*") + 1);
                                for (int j = 0; j < iContHarCount; j++)
                                {
                                    if (sContHar[j, 0].Equals(s1))
                                    {
                                        s = GetContactParametersValue(sContHar[j, 0]);
                                        lblEQOutHarValues[idx].Text = s;
                                        toolTipMessage.SetToolTip(lblEQOutHarValues[idx], s);
                                        break;
                                    }
                                }
                            }
                            else if (sMsgHar[i, 2].IndexOf("#") > 0)
                            {
                                String s1 = sMsgHar[i, 2].Substring(sMsgHar[i, 2].IndexOf("#") + 1);
                                for (int j = 0; j < iPersHarCount; j++)
                                {
                                    if (sPersHar[j, 0].Equals(s1))
                                    {
                                        s = GetPersoneParametersValue(sPersHar[j, 0]);
                                        lblEQOutHarValues[idx].Text = s;
                                        toolTipMessage.SetToolTip(lblEQOutHarValues[idx], s);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    sCurrentEQOutMessageRecordOut += s + "|";
                }
                sCurrentEQOutMessageRecordOut += value.Substring(value.IndexOf("|") + 1);

                SetEQOutMessageListOut(sCurrentEQOutMessageRecordOut);
            }
        }

        private void buttonEditAlgorithms_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            FormEditAlgorithms fe = new FormEditAlgorithms(this);
            fe.Setup(adbrCurrent.ID, true);
            Cursor = Cursors.WaitCursor;
            if (fe.ShowDialog() == DialogResult.OK)
            {
                LoadAlgorithmsDB();
                setlabelAlgorithmNameText(adbrCurrent.Name);
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                CompareVetors_RestoreDefaultValues();

                if (fe.comboBox1.SelectedIndex == 0)
                {
                    iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                    // Для всех контактеров
                    ResetAlgorithmSettingsAllContacters(adbrCurrent.ID);
                }
                else if (fe.comboBox1.SelectedIndex == 1)
                {
                    iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                    // Для текущего контактера
                    SaveAlgorithmSettingsContacter();
                }
                else if (fe.comboBox1.SelectedIndex == 2)
                {
                    iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                    // Для всех контактеров с базовым алгоритмом
                    ResetAlgorithmSettingsAllContactersList(fe.ERROR_ALG_ID, adbrCurrent.ID);
                }
                /*
                else
                {
                    LoadAlgorithmSettingsContacter();

                    setlabelAlgorithmNameText(adbrCurrent.Name);
                    comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                    CompareVetors_RestoreDefaultValues();
                }
                */
                LoadAlgorithmSettingsContacter();

                setlabelAlgorithmNameText(adbrCurrent.Name);
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                CompareVetors_RestoreDefaultValues();

                if (labelInMsgHarTitleValue_Text.Length > 0)
                {
                    UndoMarkerChanges();
                    SetEQInMessageList(labelInMsgHarTitleValue_Text);
                }
                UpdatePersoneParametersValues_Algorithm();
                UpdateContactParametersValues_Algorithm();

            }

            Cursor = Cursors.Arrow;
            StartAnswerTimer();
        }

        private void tableLayoutPanel6_Resize(object sender, EventArgs e)
        {
            //groupBox5.SuspendLayout();
            //groupBox5.Size = new System.Drawing.Size(groupBox3.Width, tableLayoutPanel4.Height + tableLayoutPanel6.Height + groupBox5.Padding.Top + groupBox5.Padding.Bottom + groupBox5.Margin.Top + groupBox5.Margin.Bottom + tableLayoutPanel6.Margin.Top + tableLayoutPanel6.Margin.Bottom);
            //groupBox5.PerformLayout();
            //groupBox5.ResumeLayout();
        }

        protected void ProgressBar_Paint(System.Windows.Forms.PaintEventArgs e, ProgressBar pb, String pbText, Color _BorderColor, Color _BarColor, bool bDashed = false)
        {
            //
            // Calculate matching colors
            //
            Color darkColor = ControlPaint.Dark(_BarColor);
            Color bgColor = ControlPaint.Dark(_BarColor);
            Color sepColor = ControlPaint.LightLight(_BarColor);

            //
            // Fill background
            //
            SolidBrush bgBrush = new SolidBrush(bgColor);
            e.Graphics.FillRectangle(bgBrush, pb.ClientRectangle);
            bgBrush.Dispose();

            // 
            // Check for value
            //
            if (pb.Value == 0)
            {
                // Draw border only and exit;
                ProgressBar_drawBorder(pb, e.Graphics, _BorderColor, pbText, sepColor);
                return;
            }

            //
            // The following is the width of the bar. This will vary with each value.
            //
            int fillWidth = (pb.Width * pb.Value) / (pb.Maximum - pb.Minimum);

            //
            // GDI+ doesn't like rectangles 0px wide or high
            //
            if (fillWidth == 0)
            {
                // Draw border only and exti;
                ProgressBar_drawBorder(pb, e.Graphics, _BorderColor, pbText, sepColor);
                return;
            }

            //
            // Rectangles for upper and lower half of bar
            //
            Rectangle topRect = new Rectangle(0, 0, fillWidth, pb.Height / 2);
            Rectangle buttomRect = new Rectangle(0, pb.Height / 2, fillWidth, pb.Height / 2);

            //
            // The gradient brush
            //
            LinearGradientBrush brush;

            //
            // Paint upper half
            //
            brush = new LinearGradientBrush(new Point(0, 0),
                new Point(0, pb.Height / 2), darkColor, _BarColor);
            e.Graphics.FillRectangle(brush, topRect);
            brush.Dispose();

            //
            // Paint lower half
            // (this.Height/2 - 1 because there would be a dark line in the middle of the bar)
            //
            brush = new LinearGradientBrush(new Point(0, pb.Height / 2 - 1),
                new Point(0, pb.Height), _BarColor, darkColor);
            e.Graphics.FillRectangle(brush, buttomRect);
            brush.Dispose();

            //
            // Calculate separator's setting
            //
            int sepWidth = (int)(pb.Height * .67);
            int sepCount = (int)(fillWidth / sepWidth);

            //
            // Paint separators
            //
            if (bDashed)
            {
                for (int i = 1; i <= sepCount; i++)
                {
                    e.Graphics.DrawLine(new Pen(sepColor, 1),
                        sepWidth * i, 0, sepWidth * i, this.Height);
                }
            }

            //
            // Draw border and exit
            //
            ProgressBar_drawBorder(pb, e.Graphics, _BorderColor, pbText, sepColor);
        }

        //
        // Draw border
        //
        protected void ProgressBar_drawBorder(ProgressBar pb, Graphics g, Color _BorderColor, String pbText, Color _SepColor)
        {
            /*
            Rectangle borderRect = new Rectangle(0, 0,
                pb.ClientRectangle.Width - 1, pb.ClientRectangle.Height - 1);
            g.DrawRectangle(new Pen(_BorderColor, 1), borderRect);
            */
            g.DrawLine(new Pen(_BorderColor, 1), 0, pb.ClientRectangle.Height - 1, pb.ClientRectangle.Width - 1, pb.ClientRectangle.Height - 1);
            if (pbText.Length > 0)
            {
                var flags = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine;
                TextRenderer.DrawText(g, pbText, pb.Font, pb.ClientRectangle, _SepColor, flags);
                flags = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine;
                TextRenderer.DrawText(g, NilsaUtils.Dictonary_GetText(userInterface, "toolTips_8", this.Name, "сек."), pb.Font, pb.ClientRectangle, _SepColor, flags);
            }
        }

        private void progressBarRead_Paint(object sender, PaintEventArgs e)
        {
            ProgressBar_Paint(e, progressBarRead.ProgressBar, Convert.ToString(timerReadCycle), Color.Black, Color.FromArgb(((System.Byte)(255)), ((System.Byte)(0)), ((System.Byte)(0))));

        }

        private void progressBarWrite_Paint(object sender, PaintEventArgs e)
        {
            ProgressBar_Paint(e, progressBarWrite.ProgressBar, Convert.ToString(timerWriteCycle), Color.Black, Color.FromArgb(((System.Byte)(255)), ((System.Byte)(255)), ((System.Byte)(0))));

        }

        private void progressBarAnswerWaiting_Paint(object sender, PaintEventArgs e)
        {
            ProgressBar_Paint(e, progressBarAnswerWaiting.ProgressBar, Convert.ToString(timerAnswerWaitingCycle), Color.Black, Color.FromArgb(((System.Byte)(0)), ((System.Byte)(255)), ((System.Byte)(0))));
        }

        private void progressBarChangePersone_Paint(object sender, PaintEventArgs e)
        {
            ProgressBar_Paint(e, progressBarChangePersone.ProgressBar, Convert.ToString(timerChangePersoneCycle), Color.Black, Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(255))));
        }

        public void timerPhysicalSendStart()
        {
            if (bServiceStart)
            {
                if (lstOutgoingMessages.Count > 0)
                {
                    timerOutgoingPull.Enabled = true;
                }
                else
                {
                    timerPhysicalSendStop();
                }
            }
        }

        private void timerPhysicalSendStop()
        {
            timerOutgoingPull.Enabled = false;
        }

        public void NILSA_SendMessage(long from, long to, String text)
        {
            DateTime dt = DateTime.Now;

            lstNILSA_MessagesDB.Add(to.ToString() + "|1|" + from.ToString() + "|" + dt.Ticks.ToString() + "|" + text);
            NILSA_SaveMessagesDB();
        }

        private void редактироватьБазуВозможныхИсходящихСообщенийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonEditOutEqMsgHar_Click(sender, e);
        }

        private void редактироватьБазуПодобныхВходящихСообщенийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonEditInEqMsgHar_Click(sender, e);
        }

        private void оЛицензииToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_1", this.Name, "Пользователь:") + " " + sLicenseUser + "\n" + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_2", this.Name, "Срок лицензии:") + " " + sLicenseDate, NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_3", this.Name, "Лицензия"));
        }

        private void tableLayoutPanel7_Resize(object sender, EventArgs e)
        {
            //groupBox6.SuspendLayout();
            //groupBox6.Size = new System.Drawing.Size(groupBox3.Width, tableLayoutPanel5.Height + tableLayoutPanel7.Height + groupBox6.Padding.Top + groupBox6.Padding.Bottom + groupBox6.Margin.Top + groupBox6.Margin.Bottom + tableLayoutPanel7.Margin.Top + tableLayoutPanel7.Margin.Bottom);
            //groupBox6.PerformLayout();
            //groupBox6.ResumeLayout();
        }

        private void MessagesDB_RenameInContent(String sFileName, String delimiterLeft, String delimiterRight, String oldValue, String newValue, bool bEqual)
        {
            List<String> lstContent = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, sFileName)))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, sFileName));
                    lstContent = new List<String>(srcFile);
                    bool bNeedSave = false;
                    for (int i = 0; i < lstContent.Count; i++)
                    {
                        String str = lstContent[i];
                        if (str.IndexOf(delimiterLeft + oldValue + delimiterRight) >= 0)
                        {
                            if (bEqual)
                            {
                                if (str.Equals(delimiterLeft + oldValue + delimiterRight))
                                {
                                    str = str.Replace(delimiterLeft + oldValue + delimiterRight, delimiterLeft + newValue + delimiterRight);
                                    lstContent[i] = str;
                                    bNeedSave = true;
                                }
                            }
                            else
                            {
                                str = str.Replace(delimiterLeft + oldValue + delimiterRight, delimiterLeft + newValue + delimiterRight);
                                lstContent[i] = str;
                                bNeedSave = true;
                            }
                        }
                    }
                    if (bNeedSave)
                        FileWriteAllLines(Path.Combine(sDataPath, sFileName), lstContent, Encoding.UTF8);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                }
            }
        }

        private void tbDB0Rename_Click(object sender, EventArgs e)
        {
            string value = "";
            if (FormMain.InputBox(this, NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_41", this.Name, "Переименование основной базы тематики"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_42", this.Name, "Введите имя базы:"), ref value) == DialogResult.OK)
            {
                value = value.Trim();
                if (value.Length > 0)
                {
                    MessagesDB_RenameInContent("_messages_db.txt", "0|", "", strDB0Name, value, false);
                    MessagesDB_RenameInContent("_eqinmsgdb_0.txt", "000000|", "|", strDB0Name, value, false);
                    MessagesDB_RenameInContent("_eqoutmsgdb_0.txt", "000000|", "|", strDB0Name, value, false);
                    MessagesDB_RenameInContent("_msg_har_1.txt", "", "", strDB0Name, value, true);
                    List<String> lstList = new List<String>();
                    lstList.Add(value);
                    FileWriteAllLines(Path.Combine(sDataPath, "_default_db_name.txt"), lstList, Encoding.UTF8);
                    strDB0Name = value;

                    LoadMessagesDBList();
                    SaveMessagesDBList();
                    LoadMessageParametersDescription();
                    MessagesDB_LoadUserDBs();
                    LoadEQInMessageDB();
                    LoadEQOutMessageDB();
                    SaveEQInMessageDB();
                    SaveEQOutMessageDB();

                    if (labelInMsgHarTitleValue_Text.Length > 0)
                        SetEQInMessageList(labelInMsgHarTitleValue_Text);
                }
            }
        }

        public void ReloadDBs()
        {
            LoadMessagesDBList();
            SaveMessagesDBList();
            LoadMessageParametersDescription();
            MessagesDB_LoadUserDBs();
            LoadEQInMessageDB();
            LoadEQOutMessageDB();
            SaveEQInMessageDB();
            SaveEQOutMessageDB();

            if (labelInMsgHarTitleValue_Text.Length > 0)
                SetEQInMessageList(labelInMsgHarTitleValue_Text);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            backgroundWorkerUpdate.RunWorkerAsync(false);
        }

        private void labelCont1Name_Click(object sender, EventArgs e)
        {
            if (iContUserID >= 0 && SocialNetwork == 0)
                System.Diagnostics.Process.Start("http://vk.com/id" + iContUserID.ToString());
        }

        private void labelCont1Family_Click(object sender, EventArgs e)
        {
            if (iContUserID >= 0 && SocialNetwork == 0)
                System.Diagnostics.Process.Start("http://vk.com/id" + iContUserID.ToString());
        }

        private void labelPers1Name_Click(object sender, EventArgs e)
        {
            if (iContUserID >= 0 && SocialNetwork == 0)
                System.Diagnostics.Process.Start("http://vk.com/id" + iPersUserID.ToString());
        }

        private void labelPers1Family_Click(object sender, EventArgs e)
        {
            if (iContUserID >= 0 && SocialNetwork == 0)
                System.Diagnostics.Process.Start("http://vk.com/id" + iPersUserID.ToString());
        }

        private void tbClearOutMessages_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            //timerPhysicalSendStop();
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_20", this.Name, "Очистить пул исходящих сообщений?"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_21", this.Name, "Очистка пула исходящих сообщения"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                lstOutgoingMessages.Clear();
            //timerPhysicalSendStart();

        }

        private void tbClearInMessages_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_22", this.Name, "Очистить пул входящих сообщений?"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_23", this.Name, "Очистка пула входящих сообщения"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                while (lstReceivedMessages.Count > 0)
                {
                    String value = lstReceivedMessages[0];
                    long _iInMsgID = Convert.ToInt64(value.Substring(0, value.IndexOf("|")));
                    if (SocialNetwork == 0)
                    {
                        if (_iInMsgID > 0)
                            api_Messages_MarkAsRead(_iInMsgID);
                    }
                    lstReceivedMessages.RemoveAt(0);
                }
                SelectNextReceivedMessage();
            }
            StartAnswerTimer();
        }

        private bool selectUserVKontakte()
        {
            // manual set timers
            StopAnswerTimer();

            bool prevTopmost = false;
            if (fwbVKontakte != null)
            {
                prevTopmost = fwbVKontakte.TopMost;
                fwbVKontakte.TopMost = true;
            }

            FormEditPersonenDB fe = new FormEditPersonenDB(this);
            fe.sContHar = new String[iPersHarCount, iPersHarAttrCount + 1];
            for (int i = 0; i < iPersHarCount; i++)
            {
                for (int j = 0; j < iPersHarAttrCount; j++)
                    fe.sContHar[i, j] = sPersHar[i, j];
                fe.sContHar[i, iPersHarAttrCount] = "";
            }
            fe.iContHarCount = iPersHarCount;
            fe.iContHarAttrCount = iPersHarAttrCount;
            fe.Setup("Init select user", -1);

            fe.ShowDialog();

            if (fwbVKontakte != null)
                fwbVKontakte.TopMost = prevTopmost;

            if (fe.bNeedPersoneChange)
            {
                timerAnswerWaitingOff();

                initPersonenLists(fe);

                if (fe.suSelSocialNetwork != SocialNetwork)
                {
                    SocialNetwork = fe.suSelSocialNetwork;
                    OnSocialNetworkChanged();
                }

                onAfterPersonenListChanged();

                tbStopService_Click(null, null);
                Setup(fe.suSelLogin, fe.suSelPwd, fe.suSelID);
            }
            return fe.bNeedPersoneChange;
        }

        private void OnSocialNetworkChanged()
        {
            if (SocialNetwork == 1)
            {
                iContactsGroupsMode = 0;
                tbChangeCommunicationMode.Image = Nilsa.Properties.Resources._mode_contacter;
                tbChangeCommunicationModeGroups.Enabled = false;
                tbChangeSocialNetwork.Image = global::Nilsa.Properties.Resources.social_nilsa;
            }
            else if (SocialNetwork == 0)
            {
                tbChangeCommunicationModeGroups.Enabled = true;
                tbChangeSocialNetwork.Image = global::Nilsa.Properties.Resources.social_vkontakte;

                iContactsGroupsMode = (lstPersoneChange.Count > 0 && SocialNetwork == 0) ? (ChangeModeWhenFree < 2 ? ChangeModeWhenFree : 1) : 0;
                tbChangeCommunicationMode.Image = iContactsGroupsMode == 0 ? Nilsa.Properties.Resources._mode_contacter : Nilsa.Properties.Resources._mode_groups;
                toolStripButtonChangeModeWhenFree.Checked = lstPersoneChange.Count > 0 && SocialNetwork == 0;
            }
        }

        private void ChangeSocialNetwork(int newSocialNetwork)
        {
            SocialNetwork = newSocialNetwork;

            if (SocialNetwork == 1)
            {
                iContactsGroupsMode = 0;
                tbChangeCommunicationMode.Image = Nilsa.Properties.Resources._mode_contacter;
                tbChangeCommunicationModeGroups.Enabled = false;
                Setup("nilsa", "nilsa");
                tbChangeSocialNetwork.Image = global::Nilsa.Properties.Resources.social_nilsa;
            }
            else if (SocialNetwork == 0)
            {
                tbChangeCommunicationModeGroups.Enabled = true;
                if (selectUserVKontakte())
                {

                    tbChangeSocialNetwork.Image = global::Nilsa.Properties.Resources.social_vkontakte;
                }
                else
                    ChangeSocialNetwork(1);
            }
            else
            {

            }

        }

        private void enterTextMessageToSend()
        {
            if (iPersUserID >= 0 && iContUserID >= 0)
            {
                FormEditMsgHarDefalutValues fe = new FormEditMsgHarDefalutValues(this);
                fe.Text = NilsaUtils.Dictonary_GetText(userInterface, "textValues_12", this.Name, "Текст сообщения");
                fe.groupBox1.Text = NilsaUtils.Dictonary_GetText(userInterface, "textValues_12", this.Name, "Текст сообщения");
                fe.textBox1.Text = "";

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    if (fe.textBox1.Text.Length > 0)
                    {
                        if (SocialNetwork == 1)
                        {
                            NILSA_SendMessage(iContUserID, iPersUserID, NilsaUtils.TextToString(fe.textBox1.Text));
                        }
                        else
                        {
                            DateTime dt = DateTime.Now;
                            String sCurRec = "0|" + getContUserIDWithGroupID() + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + NilsaUtils.TextToString(fe.textBox1.Text);
                            lstReceivedMessages.Add(sCurRec);
                        }
                        tbRefreshPull_Click(null, null);
                    }
                }
            }
        }

        private void tbNewInMessageEnter_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            enterTextMessageToSend();
            StartAnswerTimer();
        }

        private bool isNILSAMonitorRunned()
        {
            try
            {
                string processName = Path.Combine(Application.StartupPath, "NILSA_Monitor.exe");

                Thread.Sleep(500);
                Process[] processes = Process.GetProcesses();

                foreach (Process process in processes)
                {
                    //string str = process.Modules[0].FileName;
                    try
                    {
                        if (process.Modules[0].FileName.Equals(processName))
                            return true;
                    }
                    catch
                    {

                    }

                }
            }
            catch (Exception exp)
            {
                //MessageBox.Show(exp.Message);
            }
            return false;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            ChangeSocialNetwork(1);
            LoadProgramState();
            if (autoStartNILSAMonitor && !isNILSAMonitorRunned())
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "NILSA_Monitor.exe")))
                    Process.Start(Path.Combine(Application.StartupPath, "NILSA_Monitor.exe"));
            }
            if (bInitStart)
            {
                bInitStart = false;
                //    this.Visible = true;
            }
        }

        private SolidBrush reportsForegroundBrushSelected = new SolidBrush(Color.White);
        private SolidBrush reportsForegroundBrush = new SolidBrush(Color.Black);
        private SolidBrush reportsBackgroundBrushSelected = new SolidBrush(Color.FromKnownColor(KnownColor.Highlight));
        private SolidBrush reportsBackgroundBrush1 = new SolidBrush(Color.White);
        private SolidBrush reportsBackgroundBrush2 = new SolidBrush(Color.Gray);

        private static int HISTORY_PHOTO_SIZE = 50;
        private static int HISTORY_PHOTO_OFFSET_X = 10;
        private static int HISTORY_PHOTO_OFFSET_Y = 5;
        private static int HISTORY_PHOTO_WIDTH = HISTORY_PHOTO_SIZE * 2 + HISTORY_PHOTO_OFFSET_X * 2;

        private Font historyItemDateFont = new System.Drawing.Font("Arial", 6.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        private Font historyItemTimeFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        private static int HISTORY_DATE_OFFSET_Y = 16;
        private static int HISTORY_TIME_OFFSET_Y = 0;


        private void listBoxUserMessages_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            int index = e.Index;
            if (index >= 0 && index < listBoxUserMessages.Items.Count)
            {
                String value = listBoxUserMessages.Items[index].ToString();
                String historyItemFlag = value.Substring(0, value.IndexOf(" "));
                bool bPerson = historyItemFlag[0] == '-';
                value = value.Substring(value.IndexOf(" ") + 1);
                String historyItemDate = value.Substring(0, value.IndexOf(" "));
                value = value.Substring(value.IndexOf(" ") + 1);
                String historyItemTime = value.Substring(0, value.IndexOf(" "));
                value = value.Substring(value.IndexOf(" ") + 3);
                String historyItemText = NilsaUtils.StringToText(value);

                Graphics g = e.Graphics;

                //background:
                SolidBrush backgroundBrush;
                if (selected)
                    backgroundBrush = bPerson ? brushPerson1 : brushContact1;
                else
                    backgroundBrush = bPerson ? brushPerson2 : brushContact2;
                g.FillRectangle(backgroundBrush, new Rectangle(e.Bounds.Left + (bPerson ? (HISTORY_PHOTO_SIZE + HISTORY_PHOTO_OFFSET_X) : 0), e.Bounds.Top, e.Bounds.Width - HISTORY_PHOTO_SIZE - HISTORY_PHOTO_OFFSET_X, e.Bounds.Height));

                TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak;
                TextRenderer.DrawText(e.Graphics, historyItemText, e.Font, new Rectangle(e.Bounds.Left + HISTORY_PHOTO_SIZE + HISTORY_PHOTO_OFFSET_X, e.Bounds.Top, e.Bounds.Width - HISTORY_PHOTO_WIDTH, e.Bounds.Height), ForeColor, flags);

                flags = TextFormatFlags.NoPadding | TextFormatFlags.HorizontalCenter;
                if (bPerson)
                {
                    if (personPicture != null)
                        e.Graphics.DrawImage(personPicture, new Rectangle(e.Bounds.Right - HISTORY_PHOTO_SIZE, e.Bounds.Top, HISTORY_PHOTO_SIZE, HISTORY_PHOTO_SIZE));

                    TextRenderer.DrawText(e.Graphics, historyItemDate, historyItemDateFont, new Rectangle(e.Bounds.Left, e.Bounds.Top + HISTORY_DATE_OFFSET_Y, HISTORY_PHOTO_SIZE + HISTORY_PHOTO_OFFSET_X, e.Bounds.Height), ForeColor, flags);
                    TextRenderer.DrawText(e.Graphics, historyItemTime, historyItemTimeFont, new Rectangle(e.Bounds.Left, e.Bounds.Top + HISTORY_TIME_OFFSET_Y, HISTORY_PHOTO_SIZE + HISTORY_PHOTO_OFFSET_X, e.Bounds.Height), ForeColor, flags);

                }
                else
                {
                    if (contactPicture != null)
                        e.Graphics.DrawImage(contactPicture, new Rectangle(e.Bounds.Left, e.Bounds.Top, HISTORY_PHOTO_SIZE, HISTORY_PHOTO_SIZE));

                    TextRenderer.DrawText(e.Graphics, historyItemDate, historyItemDateFont, new Rectangle(e.Bounds.Right - HISTORY_PHOTO_SIZE - HISTORY_PHOTO_OFFSET_X, e.Bounds.Top + HISTORY_DATE_OFFSET_Y, HISTORY_PHOTO_SIZE + HISTORY_PHOTO_OFFSET_X, e.Bounds.Height), ForeColor, flags);
                    TextRenderer.DrawText(e.Graphics, historyItemTime, historyItemTimeFont, new Rectangle(e.Bounds.Right - HISTORY_PHOTO_SIZE - HISTORY_PHOTO_OFFSET_X, e.Bounds.Top + HISTORY_TIME_OFFSET_Y, HISTORY_PHOTO_SIZE + HISTORY_PHOTO_OFFSET_X, e.Bounds.Height), ForeColor, flags);
                }

            }

            e.DrawFocusRectangle();
        }

        private void listBoxUserMessages_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            //FeedHistoryItem item = (sender as ListBox).Items[e.Index] as FeedHistoryItem;
            int index = e.Index;
            if (index >= 0 && index < listBoxUserMessages.Items.Count)
            {
                String value = listBoxUserMessages.Items[index].ToString();
                String historyItemFlag = value.Substring(0, value.IndexOf(" "));
                value = value.Substring(value.IndexOf(" ") + 1);
                String historyItemDate = value.Substring(0, value.IndexOf(" "));
                value = value.Substring(value.IndexOf(" ") + 1);
                String historyItemTime = value.Substring(0, value.IndexOf(" "));
                value = value.Substring(value.IndexOf(" ") + 3);
                String historyItemText = NilsaUtils.StringToText(value);

                TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak;
                Size proposedSize = new Size(listBoxUserMessages.Width - HISTORY_PHOTO_WIDTH, int.MaxValue);
                SizeF sfText = TextRenderer.MeasureText(e.Graphics, historyItemText, Font, proposedSize, flags);

                e.ItemHeight = (int)Math.Ceiling(sfText.Height);
                if (e.ItemHeight < HISTORY_PHOTO_SIZE + HISTORY_PHOTO_OFFSET_Y)
                    e.ItemHeight = HISTORY_PHOTO_SIZE + HISTORY_PHOTO_OFFSET_Y;
            }
        }

        private void tbSetSocialNetworkNILSA_Click(object sender, EventArgs e)
        {
            //SocialNetwork = 1;
            //toolStripButtonSocialNetworkChange_Click(null, null);
            ChangeSocialNetwork(1);
        }

        private void tbSetSocialNetworkVKontacte_Click(object sender, EventArgs e)
        {
            //SocialNetwork = 0;
            //toolStripButtonSocialNetworkChange_Click(null, null);
            ChangeSocialNetwork(0);
        }

        private void tbSetSocialNetworkFacebook_Click(object sender, EventArgs e)
        {
            //SocialNetwork = 0;
            //toolStripButtonSocialNetworkChange_Click(null, null);
            ChangeSocialNetwork(2);
        }

        private void tbChangeSocialNetwork_ButtonClick(object sender, EventArgs e)
        {
            tbChangeSocialNetwork.ShowDropDown();
        }

        private void tbNewOutMessageEnter_Click(object sender, EventArgs e)
        {
            if (iPersUserID >= 0 && iContUserID >= 0)
            {
                onManualButtonClick();
                // manual set timers
                StopAnswerTimer();
                FormEditMsgHarDefalutValues fe = new FormEditMsgHarDefalutValues(this);
                fe.Text = NilsaUtils.Dictonary_GetText(userInterface, "textValues_12", this.Name, "Текст сообщения");
                fe.groupBox1.Text = NilsaUtils.Dictonary_GetText(userInterface, "textValues_12", this.Name, "Текст сообщения");
                fe.textBox1.Text = "";

                if (fe.ShowDialog() == DialogResult.OK)
                {
                    if (fe.textBox1.Text.Length > 0)
                    {
                        //---
                        if (SocialNetwork == 0)
                            lstOutgoingMessages_Insert(iContUserID.ToString(), labelCont1Name.Text + " " + labelCont1Family.Text, NilsaUtils.TextToString(fe.textBox1.Text), "0", iGroupAnswerID, iGroupAnswerPostID, iGroupAnswerCommentID);
                        else
                        {
                            NILSA_SendMessage(iPersUserID, iContUserID, NilsaUtils.TextToString(fe.textBox1.Text));
                            cntE1++;
                            cntE2++;
                            cntE7++;
                            cntE8++;
                            SaveProgramCountersE1E2E3();
                            UpdateProgramCountersInfoE1E2E3();

                            cntE4++;
                            cntE5++;
                            SaveProgramCountersE4E5E6();
                            UpdateProgramCountersInfoE4E5E6();
                        }
                        timerPhysicalSendStart();
                        //---
                    }
                }
                StartAnswerTimer();
            }
        }

        private void buttonNewOutMsgHar_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            AddEQOutMessageParametersValues(labelOutEqMsgHarTitleValue_Text);
        }

        private void tbInitContactDialogContacter_Click(object sender, EventArgs e)
        {
            onManualButtonClick();
            // manual set timers
            StopAnswerTimer();
            List<String> lstInitDialogContacts = new List<String>();

            String sUID = iContUserID < -1 ? (-iContUserID).ToString() : iContUserID.ToString();
            String sUName = contName;
            lstInitDialogContacts.Add(sUID + "|" + sUName);

            FileWriteAllLines(Path.Combine(sDataPath, "_initdialog_" + getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"), lstInitDialogContacts, Encoding.UTF8);

            int oldAlgorithmID = adbrCurrent.ID;
            if (iContUserID >= 0)
            {
                FormInitContactDialog fe = new FormInitContactDialog(this);
                fe.sContHar = new String[iContHarCount, iContHarAttrCount + 1];
                for (int i = 0; i < iContHarCount; i++)
                {
                    for (int j = 0; j < iContHarAttrCount; j++)
                        fe.sContHar[i, j] = sContHar[i, j];
                    fe.sContHar[i, iContHarAttrCount] = "";
                }
                fe.iContHarCount = iContHarCount;
                fe.iContHarAttrCount = iContHarAttrCount;
                fe.Setup(iPersUserID.ToString() + " (" + userLogin + ", " + userPassword + ")", iPersUserID, false);
                INIT_PERSONE_DIALOG = false;

                DialogResult dr = fe.ShowDialog();
            }
            else if (iContUserID < -1)
            {
                FormInitGroupDialog fe = new FormInitGroupDialog(this);
                fe.sContHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
                for (int i = 0; i < iGroupHarCount; i++)
                {
                    for (int j = 0; j < iGroupHarAttrCount; j++)
                        fe.sContHar[i, j] = sGroupHar[i, j];
                    fe.sContHar[i, iGroupHarAttrCount] = "";
                }
                fe.iContHarCount = iGroupHarCount;
                fe.iContHarAttrCount = iGroupHarAttrCount;
                fe.Setup(iPersUserID.ToString() + " (" + userLogin + ", " + userPassword + ")", iPersUserID, false, "");
                INIT_PERSONE_DIALOG = false;

                DialogResult dr = fe.ShowDialog();
            }
            applyAlgorithm(oldAlgorithmID);
            SelectNextReceivedMessage(false);
            StartAnswerTimer();
        }

        private void labelCont1FIO_Click(object sender, EventArgs e)
        {
            if (iContUserID >= 0 && SocialNetwork == 0)
            {
                onManualButtonClick();

                // manual set timers
                StopAnswerTimer();

                if (FormWebBrowserEnabled)
                {
                    FormWebBrowser fwb = new FormWebBrowser(this);
                    fwb.Setup(userLogin, userPassword, WebBrowserCommand.GoToContactPage, iContUserID);
                    fwb.ShowDialog();
                    //tbUpdateLists_Click(null, null);
                }
                else
                    System.Diagnostics.Process.Start("http://vk.com/id" + iContUserID.ToString());

                StartAnswerTimer();
            }

        }

        private void labelPers1FIO_Click(object sender, EventArgs e)
        {
            if (iPersUserID >= 0 && SocialNetwork == 0)
            {
                //System.Diagnostics.Process.Start("http://vk.com/id" + iPersUserID.ToString());
                //String sPage = "http://vk.com/id" + iPersUserID.ToString();

                onManualButtonClick();
                // manual set timers
                StopAnswerTimer();

                if (FormWebBrowserEnabled)
                {
                    FormWebBrowser fwb = new FormWebBrowser(this);
                    fwb.Setup(userLogin, userPassword, WebBrowserCommand.GoToPersonePage, iPersUserID);
                    fwb.ShowDialog();
                    tbUpdateLists_Click(null, null);
                }
                else
                    System.Diagnostics.Process.Start("http://vk.com/id" + iPersUserID.ToString());

                StartAnswerTimer();
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            comboBoxCompareLexicalLevel.ComboBox.DroppedDown = true;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_24", this.Name, "Вы уверены, что хотите сбросить (обнулить) все счетчики программы?"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_25", this.Name, "Сброс счетчиков программы"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                tbStopService_Click(null, null);

                string[] files = Directory.GetFiles(Application.StartupPath, "_program_counters_*.txt");
                foreach (string f in files)
                    File.Delete(f);

                deleteAllPersoneActivationCounter();
                MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_26", this.Name, "Сброс счетчиков программы завершен. Сейчас программа будет перезапущена для обнуления счетчиков."), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_25", this.Name, "Сброс счетчиков программы"));

                FormMain_FormClosing_Action = false;
                System.Diagnostics.Process.Start(Path.Combine(Application.StartupPath, "Nilsa.exe"));
                Close();
            }

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            comboBoxCompareVectorLevel.ComboBox.DroppedDown = true;
        }

        private void deleteAllContacterMessages(bool fromTop)
        {
            if (iContUserID >= 0 && (lstReceivedMessages.Count > (fromTop ? 0 : 1)))
            {
                int i = fromTop ? 0 : 1;
                while (i < lstReceivedMessages.Count)
                {
                    String value = lstReceivedMessages[i];
                    long _iInMsgID = Convert.ToInt64(value.Substring(0, value.IndexOf("|")));
                    value = value.Substring(value.IndexOf("|") + 1);
                    String strUsrId = value.Substring(0, value.IndexOf("|"));
                    long iCurMsgContUserID = -1;
                    long iCurGroupAnswerID = -1;
                    long iCurGroupAnswerPostID = -1;
                    long iCurGroupAnswerCommentID = -1;
                    if (strUsrId.IndexOf('/') > 0)
                    {
                        //Convert.ToString(_ownerId)+"/"+ Convert.ToString(_postId) + "/"+ Convert.ToString(msg.Id) + "/" + Convert.ToString(msg.FromId) + "|";
                        iCurGroupAnswerID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                        strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                        iCurGroupAnswerPostID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                        strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                        iCurGroupAnswerCommentID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                        strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                        iCurMsgContUserID = Convert.ToInt64(strUsrId);
                    }
                    else
                        iCurMsgContUserID = Convert.ToInt64(strUsrId);

                    if (iCurMsgContUserID == iContUserID/* && iCurGroupAnswerID == iGroupAnswerID && iCurGroupAnswerPostID == iGroupAnswerPostID && iCurGroupAnswerCommentID == iGroupAnswerCommentID*/)
                    {
                        if (SocialNetwork == 0)
                        {
                            if (_iInMsgID > 0)
                                api_Messages_MarkAsRead(_iInMsgID);
                        }
                        lstReceivedMessages.RemoveAt(i);
                    }
                    else i++;
                }

            }
        }
        private void tbDeleteContacterMessages_Click(object sender, EventArgs e)
        {
            onManualButtonClick();
            // manual set timers
            StopAnswerTimer();
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_27", this.Name, "Удалить все сообщения Контактера из пула?"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_28", this.Name, "Удаление сообщений"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                deleteAllContacterMessages(false);
                //if (iInMsgID >= 0)
                SelectNextReceivedMessage(iInMsgID >= 0);
            }
            StartAnswerTimer();
        }

        private bool UndoMarkerChanges()
        {

            if (iContUserID >= 0 && iUndoMarkerChangesContHarValuesContID == iContUserID && iInMsgID >= 0 && lstUndoMarkerChangesContHarValues != null && lstUndoMarkerChangesContHarValues.Count == lstContHarValues.Count && iUndoMarkerChangesAlgorithm >= 0 && lstReceivedMessages.Count > 0)
            {
                lstContHarValues = new List<string>();
                foreach (string _str in lstUndoMarkerChangesContHarValues)
                    lstContHarValues.Add(_str);
                SaveContactParamersValues();
                SaveAlgorithmSettingsContacter(iUndoMarkerChangesAlgorithm);
                LoadContactParametersDescription();
                LoadAlgorithmSettingsContacter();
                UpdateContactParametersValues_Algorithm();
                if (sUndoMarkerCurrentEQInMessageRecord.Length == 0)
                    sUndoMarkerCurrentEQInMessageRecord = sCurrentEQInMessageRecord;
                else
                    sCurrentEQInMessageRecord = sUndoMarkerCurrentEQInMessageRecord;
                return true;
            }
            return false;
        }

        private void tbUndoMarkerChanges_Click(object sender, EventArgs e)
        {
            if (UndoMarkerChanges())
                SelectNextReceivedMessage(false);
        }

        const string updatesiteurl = "http://www.nilsa.ru/";
        bool updateShowMessages = false;
        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            updateShowMessages = (bool)e.Argument;
            string _FileName = Path.Combine(Application.StartupPath, "nilsa_update.zip");
            if (File.Exists(_FileName))
            {
                StartProgramUpdate();
            }
            else
            {
                if (autoUpdateProgram || iLicenseType != LICENSE_TYPE_NILS || updateShowMessages)
                {

                    _FileName = Path.Combine(Application.StartupPath, "version.txt");
                    if (File.Exists(_FileName))
                        File.Delete(_FileName);

                    try
                    {
                        var client = new WebClient();
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadVersion_Completed);
                        client.DownloadFileAsync(new Uri(updatesiteurl + "versionnew.txt"), _FileName);
                    }
                    catch (Exception) { }
                }
            }
        }


        private void downloadVersion_Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string _FileName = Path.Combine(Application.StartupPath, "version.txt");
                if (File.Exists(_FileName))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(_FileName);
                        List<String> lstList = new List<String>(srcFile);
                        File.Delete(_FileName);

                        if (lstList.Count > 0)
                        {
                            System.Version remoteVersion = new Version(lstList[0]);
                            System.Version localVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                            if (localVersion < remoteVersion)
                            {
                                if (updateShowMessages)
                                {
                                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_29", this.Name, "Обнаружено обновление программы до версии") + " " + remoteVersion.ToString() + ".\n\n" + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_30", this.Name, "Загрузить сейчас?"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_31", this.Name, "Загрузка обновления программы"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                                        backgroundWorkerNILSA.RunWorkerAsync(remoteVersion);
                                }
                                else
                                    backgroundWorkerNILSA.RunWorkerAsync(remoteVersion);
                            }
                            else
                            {
                                if (updateShowMessages)
                                    MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_32", this.Name, "Вы используете самую новую версию программы."), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_31", this.Name, "Загрузка обновления программы"));
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", exp);
                    }
                }//if (File.Exists(_FileName))
            }
            else
            {
                if (updateShowMessages)
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_33", this.Name, "Ошибка проверки обновления программы, проверьте наличие подключения к интернету или попробуйте повторить операцию позже..."), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_31", this.Name, "Загрузка обновления программы"));
            }
        }

        private void backgroundWorkerNILSA_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Version versionDownload = (System.Version)e.Argument;

            string _FileName = Path.Combine(Application.StartupPath, "nilsa_update.zip");
            if (File.Exists(_FileName))
                File.Delete(_FileName);
            try
            {
                var client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadNILSA_ProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadNILSA_Completed);
                client.DownloadFileAsync(new Uri(updatesiteurl + "nilsa_update_new_" + versionDownload.ToString().Replace('.', '_') + ".zip"), _FileName);
            }
            catch (Exception) { }

        }

        delegate void StringParameterDelegate(string caption);
        public void UpdateProgress_setFormCaption(string caption)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new StringParameterDelegate(setFormCaption), new object[] { caption });
                return;
            }
            // Must be on the UI thread if we've got this far
        }

        private void setFormCaption(string caption)
        {
            this.Text = caption;
        }

        private void downloadNILSA_Completed(object sender, AsyncCompletedEventArgs e)
        {
            UpdateProgress_setFormCaption("NILSA, v" + Application.ProductVersion + ((bDebugVersion || showSessionStartTime) ? " (" + sessionStartTime + ")" : ""));

            if (e.Error == null)
            {
                StartProgramUpdate();
            }
            else
            {
                string updatePath = Path.Combine(Application.StartupPath, "nilsa_update.zip");
                if (File.Exists(updatePath))
                    File.Delete(updatePath);

                if (updateShowMessages)
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_34", this.Name, "Ошибка загрузки обновления программы, проверьте наличие подключения к интернету или попробуйте повторить операцию позже..."), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_31", this.Name, "Загрузка обновления программы"));
            }

        }

        private void StartProgramUpdate()
        {
            string _FileName = Path.Combine(Application.StartupPath, "nilsa_update.zip");
            if (File.Exists(_FileName))
            {
                if (updateShowMessages)
                {
                    if (MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_35", this.Name, "Загрузка обновления программы завершена.") + "\n\n" + NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_36", this.Name, "Установить сейчас?"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_31", this.Name, "Загрузка обновления программы"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        startNilsaUpdater();
                    }
                }
                else
                {
                    startNilsaUpdater();
                }
            }
        }

        private void startNilsaUpdater()
        {
            killNILSAMonitor();
            try
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "NilsaUpdater.exe")))
                {
                    Process.Start(Path.Combine(Application.StartupPath, "NilsaUpdater.exe"), "");
                    Close();
                }
            }
            catch (Exception) { }
        }

        private void downloadNILSA_ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                UpdateProgress_setFormCaption("NILSA, v" + Application.ProductVersion + " (Download update: " + e.ProgressPercentage + "%)");
            }
            catch (Exception) { }
        }

        private void toolStripMenuItemCheckUpdates_Click(object sender, EventArgs e)
        {
            backgroundWorkerUpdate.RunWorkerAsync(true);
        }

        private void toolStripMenuItemProgramSettings_Click(object sender, EventArgs e)
        {
            FormProgramSettings fps = new FormProgramSettings(this);
            fps.checkBox1.Checked = autoUpdateProgram || iLicenseType != LICENSE_TYPE_NILS;
            fps.checkBox2.Checked = compareLexicalNewAlgorithm;
            fps.checkBox3.Checked = !FormWebBrowserEnabled;
            fps.checkBox4.Checked = autoRestoreProgramState;
            fps.checkBox5.Checked = autoStartNILSAMonitor;
            fps.checkBox6.Checked = showSessionStartTime;
            fps.checkBox7.Checked = autoUpdateModelFromServer || iLicenseType == LICENSE_TYPE_WORK;

            if (iLicenseType == LICENSE_TYPE_WORK)
                fps.checkBox7.Enabled = false;
            else
                fps.checkBox7.Enabled = true;

            if (iLicenseType != LICENSE_TYPE_NILS)
                fps.checkBox1.Enabled = false;
            else
                fps.checkBox1.Enabled = true;

            fps.comboBoxLanguage.SelectedIndex = CurrentLanguage.Equals("ru") ? 0 : (CurrentLanguage.Equals("en") ? 1 : 0);
            string oldCurrentLanguage = CurrentLanguage;

            bool oldcompareLexicalNewAlgorithm = compareLexicalNewAlgorithm;
            if (fps.ShowDialog() == DialogResult.OK)
            {
                autoUpdateProgram = fps.checkBox1.Checked || iLicenseType != LICENSE_TYPE_NILS;
                compareLexicalNewAlgorithm = fps.checkBox2.Checked;
                FormWebBrowserEnabled = !fps.checkBox3.Checked;
                autoRestoreProgramState = fps.checkBox4.Checked;
                autoStartNILSAMonitor = fps.checkBox5.Checked;
                showSessionStartTime = fps.checkBox6.Checked;
                autoUpdateModelFromServer = fps.checkBox7.Checked || iLicenseType == LICENSE_TYPE_WORK;

                //FormWebBrowserEnabled = false; // Force disable

                CurrentLanguage = fps.comboBoxLanguage.SelectedIndex == 0 ? "ru" : (fps.comboBoxLanguage.SelectedIndex == 1 ? "en" : "ru");
                SaveProgramSettings();
                //this.Text = userName + " - NILSA, v" + Application.ProductVersion + ((bDebugVersion || showSessionStartTime) ? " (" + sessionStartTime + ")" : "");
                setStandardCaption();

                if (!CurrentLanguage.Equals(oldCurrentLanguage))
                {
                    FormMain_FormClosing_Action = true;
                    System.Diagnostics.Process.Start(Path.Combine(Application.StartupPath, "Nilsa.exe"));
                    Close();
                }
                else
                {
                    if (oldcompareLexicalNewAlgorithm != compareLexicalNewAlgorithm)
                    {
                        if (tbUndoMarkerChanges.Enabled)
                            tbUndoMarkerChanges_Click(null, null);
                    }
                }
            }
        }

        private void toolStripMenuItemClearInMsgPullPersonen_Click(object sender, EventArgs e)
        {
            // manual set timers
            StopAnswerTimer();
            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_37", this.Name, "Очистить пулы входящих сообщений Контактеров Персонажей в ротации?"), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_38", this.Name, "Очистка пулов входящих сообщений"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DateTime dt;
                foreach (string sUID in lstPersoneChange)
                {
                    if (!sUID.Equals(iPersUserID.ToString()))
                    {
                        dt = DateTime.Now;
                        List<String> lstReceivedMessagesTemp = new List<String>();
                        lstReceivedMessagesTemp.Insert(0, "0|" + sUID + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "CLEAR_PERSONE_DIALOGS");
                        FileWriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix() + sUID + (iContactsGroupsMode == 0 ? "_contacter" : "_groups") + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
                    }
                }

                lstReceivedMessages.Clear();
                dt = DateTime.Now;
                lstReceivedMessages.Insert(0, "0|330643598|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + "CLEAR_PERSONE_DIALOGS");

                SelectNextReceivedMessage(false);
            }
            StartAnswerTimer();
        }

        private void tbEditSubjectsList_Click(object sender, EventArgs e)
        {
            FormEditUserMessagesDBList feum = new FormEditUserMessagesDBList(this);
            feum.Setup();
            feum.ShowDialog();

        }

        private void tbEditGroupParametersItemsDesc_Click(object sender, EventArgs e)
        {
            // manual set timers
            StopAnswerTimer();
            LoadGroupParametersDescription();
            FormEditPersHar fe = new FormEditPersHar(this);
            fe.sPersHar = new String[iGroupHarCount, iGroupHarAttrCount];
            for (int i = 0; i < iGroupHarCount; i++)
                for (int j = 0; j < iGroupHarAttrCount; j++)
                    fe.sPersHar[i, j] = sGroupHar[i, j];
            fe.iPersHarAttrCount = iGroupHarAttrCount;
            fe.iPersHarCount = iGroupHarCount;
            fe.Setup(true, 3);

            if (fe.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < iGroupHarCount; i++)
                    for (int j = 0; j < iGroupHarAttrCount; j++)
                        sGroupHar[i, j] = fe.sPersHar[i, j];
                SaveGroupParametersDescription();
                LoadGroupParametersDescription();
            }
            StartAnswerTimer();

        }

        private void tbEditGroupsDB_Click(object sender, EventArgs e)
        {
            if (SocialNetwork == 1)
                return;

            // manual set timers
            StopAnswerTimer();
            LoadGroupParametersDescription();
            FormEditGroupsDB fe = new FormEditGroupsDB(this);
            fe.sGroupHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
            for (int i = 0; i < iGroupHarCount; i++)
            {
                for (int j = 0; j < iGroupHarAttrCount; j++)
                    fe.sGroupHar[i, j] = sGroupHar[i, j];
                fe.sGroupHar[i, iGroupHarAttrCount] = "";
            }
            fe.iGroupHarCount = iGroupHarCount;
            fe.iGroupHarAttrCount = iGroupHarAttrCount;
            fe.Setup(iPersUserID.ToString() + " (" + userLogin + ", " + userPassword + ")", iPersUserID, sGroupAdditinalUsers, INIT_PERSONE_DIALOG);

            try
            {
                fe.ShowDialog();
                Application.DoEvents();
            }
            catch
            {

            }
            UpdateProgramCountersInfoB2B5B6();

            //GroupsList_Load();
            //if (!INIT_PERSONE_DIALOG && fe.bInitDialogs)
            //    SelectNextReceivedMessage(false);

            //INIT_PERSONE_DIALOG = false;
            //StartAnswerTimer();

            if (fe.bNeedPersoneChange && fe.iSelGroupID >= 0)
            {
                tbStopService_Click(null, null);
                iContUserID = -fe.iSelGroupID;
                //iContUserID = -1;
                iGroupAnswerID = -1;// fe.iSelGroupID;
                iGroupAnswerPostID = -1;
                iGroupAnswerCommentID = -1;
                ContactsList_Load();
                LoadContactParamersValues();
                SetContactParametersValues();
                LoadAlgorithmSettingsContacter();
                //LoadContactParametersDescription();

                OnSelectOtherContacter();
            }
            else
            {
                ContactsList_Load();
                LoadContactParamersValues();
                SetContactParametersValues();
                LoadAlgorithmSettingsContacter();
                //LoadContactParametersDescription();
                //comboBoxCompareVectorLevel_SelectedIndexChanged(sender, e);
                comboBoxCompareLexicalLevel_SelectedIndexChanged(sender, e);
                if (!INIT_PERSONE_DIALOG && fe.bInitDialogs)
                    SelectNextReceivedMessage(false);

                INIT_PERSONE_DIALOG = false;
                StartAnswerTimer();
            }
            tbNewOutMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            tbInitContactDialogContacter.Enabled = iPersUserID >= 0 && (iContUserID >= 0 || iContUserID < -1);
            tbNewInMessageEnter.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            tbDeleteContacterMessages.Enabled = iPersUserID >= 0 && iContUserID >= 0;
            UpdateContactParametersValues_Algorithm();

        }

        private void btnInitGroupsDialog_Click(object sender, EventArgs e)
        {
            tbEditGroupsDB_Click(sender, e);
        }

        private void btnInitContactsDialog_Click(object sender, EventArgs e)
        {
            tbEditContactsDB_Click(sender, e);
        }

        private void setContacterWorkMode()
        {
            tbContacterWorkMode.Text = NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_43", this.Name, "Режим работы с Контактером:") + " ";
            if (adbrCurrent.Name.ToLower().Equals("ignore"))
            {
                tbContacterWorkMode.Image = global::Nilsa.Properties.Resources.contacter_mode_ignore;
                tbContacterWorkMode.Text += "Ignore";
            }
            else if (adbrCurrent.Name.ToLower().Equals("boycott"))
            {
                tbContacterWorkMode.Image = global::Nilsa.Properties.Resources.contacter_mode_boycott;
                tbContacterWorkMode.Text += "Boycott";
            }
            else
            {
                tbContacterWorkMode.Image = global::Nilsa.Properties.Resources.contacter_mode_normal;
                tbContacterWorkMode.Text += "Normal";

            }
        }

        private void tbContacterWorkModeNormal_Click(object sender, EventArgs e)
        {
            if (adbrCurrent.Name.ToLower().Equals("ignore") || adbrCurrent.Name.ToLower().Equals("boycott"))
            {
                int iMsgAlg = SearchAlgorithmsDBList("basic");
                ChangeContacterAlgorithm(iMsgAlg);
                applyAlgorithm(iMsgAlg);
                LoadAlgorithmSettingsContacter();
                UpdateContactParametersValues_Algorithm();
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                CompareVetors_RestoreDefaultValues();

                if (labelInMsgHarTitleValue_Text.Length > 0)
                {
                    SetEQInMessageList(labelInMsgHarTitleValue_Text);
                }
            }

        }

        private void tbContacterWorkModeIgnored_Click(object sender, EventArgs e)
        {
            if (!adbrCurrent.Name.ToLower().Equals("ignore"))
            {
                int iMsgAlg = SearchAlgorithmsDBList("ignore");
                if (iMsgAlg < 0)
                    iMsgAlg = SearchAlgorithmsDBList("basic");
                ChangeContacterAlgorithm(iMsgAlg);
                applyAlgorithm(iMsgAlg);
                LoadAlgorithmSettingsContacter();
                UpdateContactParametersValues_Algorithm();
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                CompareVetors_RestoreDefaultValues();

                if (labelInMsgHarTitleValue_Text.Length > 0)
                {
                    SetEQInMessageList(labelInMsgHarTitleValue_Text);
                }
            }
        }

        private void tbContacterWorkModeBoycott_Click(object sender, EventArgs e)
        {
            if (!adbrCurrent.Name.ToLower().Equals("boycott"))
            {
                int iMsgAlg = SearchAlgorithmsDBList("boycott");
                if (iMsgAlg < 0)
                    iMsgAlg = SearchAlgorithmsDBList("basic");
                ChangeContacterAlgorithm(iMsgAlg);
                applyAlgorithm(iMsgAlg);
                LoadAlgorithmSettingsContacter();
                UpdateContactParametersValues_Algorithm();
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                CompareVetors_RestoreDefaultValues();

                if (labelInMsgHarTitleValue_Text.Length > 0)
                {
                    SelectNextReceivedMessage();
                }


            }
            setContacterWorkMode();
        }

        private void tbRefreshPull_ButtonClick(object sender, EventArgs e)
        {
            tbRefreshPull.ShowDropDown();
        }

        private bool Wall_ReadNewComments(long _ownerId, long _postId, long _lastcommentId, Dictionary<long, Dictionary<long, long>> _dictWallMonitoring, Dictionary<long, Dictionary<long, long>> _dictWallMonitoringRemove)
        {
            bool bAdded = false;
            /*
            try
            {
                int totalCount = 0;
                var msgsReceived = api.Wall.GetComments(-_ownerId, _postId, out totalCount, null, false, 100, null, 0, _lastcommentId + 1);
                long lastcommentId = _lastcommentId;

                List<String> lstMsgToAdd = new List<string>();
                foreach (VkNet.Model.Comment msg in msgsReceived)
                {
                    if (msg.Id > lastcommentId)
                        lastcommentId = msg.Id;
                    else
                        continue;

                    String value = "";
                    if (msg.FromId != iPersUserID)
                    {
                        value = "0|"; // msgId
                        value = value + Convert.ToString(_ownerId) + "/" + Convert.ToString(_postId) + "/" + Convert.ToString(msg.Id) + "/" + Convert.ToString(msg.FromId) + "|";
                        value = value + msg.Date.Value.ToShortDateString() + "|";
                        value = value + msg.Date.Value.ToShortTimeString() + "|";
                        value = value + NilsaUtils.TextToString(msg.Text);
                        if (msg.Attachments.Count > 0)
                        {
                            foreach (VkNet.Model.Attachments.Attachment attach in msg.Attachments)
                            {
                                value = value + "<br>" + attach.Type.ToString().Replace('.', '_');
                            }
                        }

                        if (!lstReceivedMessages.Contains(value))
                        {
                            lstMsgToAdd.Add(msg.Date.Value.ToString("yyyy.MM.dd.HH:mm:ss") + "|" + value);
                        }
                    }
                }

                if (lastcommentId > _lastcommentId)
                {
                    if (!_dictWallMonitoring.ContainsKey(_ownerId))
                        _dictWallMonitoring.Add(_ownerId, new Dictionary<long, long>());

                    if (!_dictWallMonitoring[_ownerId].ContainsKey(_postId))
                        _dictWallMonitoring[_ownerId].Add(_postId, lastcommentId);
                    else
                        _dictWallMonitoring[_ownerId][_postId] = lastcommentId;
                }

                if (lstMsgToAdd.Count > 0)
                {
                    lstMsgToAdd = lstMsgToAdd.OrderBy(i => i).ToList();

                    foreach (String EQ in lstMsgToAdd)
                    {
                        bAdded = true;
                        lstReceivedMessages.Add(EQ.Substring(EQ.IndexOf("|") + 1));
                    }
                }
            }
            catch (VkNet.Exception.AccessTokenInvalidException atiexp)
            {
                ReAutorize(userLogin, userPassword);
            }
            catch (System.Net.WebException wbexp)
            {
                ReAutorize(userLogin, userPassword);
            }
            catch (VkNet.Exception.VkApiException vkapiexp)
            {
                //ReAutorize(userLogin, userPassword);
                if (!_dictWallMonitoringRemove.ContainsKey(_ownerId))
                    _dictWallMonitoringRemove.Add(_ownerId, new Dictionary<long, long>());

                if (!_dictWallMonitoringRemove[_ownerId].ContainsKey(_postId))
                    _dictWallMonitoringRemove[_ownerId].Add(_postId, 1);
                else
                    _dictWallMonitoringRemove[_ownerId][_postId] = 1;

                ExceptionToLogList("Wall_ReadNewComments", userLogin + "/" + userPassword, vkapiexp);
            }
            catch (Exception e)
            {
                ExceptionToLogList("Wall_ReadNewComments", userLogin + "/" + userPassword, e);
            }
            finally { }
            */
            return bAdded;
        }

        private void tbRefreshPullGroups_Click(bool _bSelect)
        {
            if (SocialNetwork == 0)
            {
                Dictionary<long, Dictionary<long, long>> _dictWallMonitoring = new Dictionary<long, Dictionary<long, long>>();
                Dictionary<long, Dictionary<long, long>> _dictWallMonitoringRemove = new Dictionary<long, Dictionary<long, long>>();
                bool bAdded = false;
                foreach (var pair in dictWallMonitoring)
                {
                    long _ownerId = pair.Key;
                    Dictionary<long, long> _postIds = pair.Value;
                    foreach (var _postIdDict in _postIds)
                    {
                        bool bNewAdded = Wall_ReadNewComments(_ownerId, _postIdDict.Key, _postIdDict.Value, _dictWallMonitoring, _dictWallMonitoringRemove);
                        bAdded = bAdded || bNewAdded;
                    }
                }

                bool bNeedSave = false;
                foreach (var pair in _dictWallMonitoring)
                {
                    long _ownerId = pair.Key;
                    Dictionary<long, long> _postIds = pair.Value;
                    foreach (var _postIdDict in _postIds)
                    {
                        bNeedSave = true;
                        dictWallMonitoring[_ownerId][_postIdDict.Key] = _postIdDict.Value;
                    }
                }

                foreach (var pair in _dictWallMonitoringRemove)
                {
                    long _ownerId = pair.Key;
                    Dictionary<long, long> _postIds = pair.Value;
                    foreach (var _postIdDict in _postIds)
                    {
                        bNeedSave = true;
                        if (dictWallMonitoring.ContainsKey(_ownerId))
                        {
                            if (dictWallMonitoring[_ownerId].ContainsKey(_postIdDict.Key))
                                dictWallMonitoring[_ownerId].Remove(_postIdDict.Key);

                        }
                    }

                    if (dictWallMonitoring[_ownerId].Count == 0)
                    {
                        bNeedSave = true;
                        dictWallMonitoring.Remove(_ownerId);
                    }
                }

                if (bNeedSave)
                    Wall_SavePostToMonitoring();

                if (bAdded && _bSelect)
                {
                    if (iInMsgID == -1)
                        SelectNextReceivedMessage(false);
                }
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (FormWebBrowserEnabled)
                    Cef.Shutdown();
            }
            catch
            {

            }
        }

        private void tbContacterWorkMode_ButtonClick(object sender, EventArgs e)
        {
            onManualButtonClick();
            // manual set timers
            StopAnswerTimer();
            tbContacterWorkMode.ShowDropDown();
        }

        public Dictionary<long, Dictionary<long, long>> dictWallMonitoring = new Dictionary<long, Dictionary<long, long>>();
        public void Wall_UserSavePostToMonitoring(long _uid, Dictionary<long, Dictionary<long, long>> _dictWallMonitoring)
        {
            List<String> lstTS = new List<String>();

            foreach (var pair in _dictWallMonitoring)
            {
                long _ownerId = pair.Key;
                Dictionary<long, long> _postIds = pair.Value;
                foreach (var _postIdDict in _postIds)
                {
                    lstTS.Add(_ownerId.ToString() + "|" + _postIdDict.Key.ToString() + "|" + _postIdDict.Value.ToString());
                }
            }
            FileWriteAllLines(Path.Combine(sDataPath, "_wall_posts_" + _uid.ToString() + ".txt"), lstTS, Encoding.UTF8);
        }

        public void Wall_SavePostToMonitoring()
        {
            List<String> lstTS = new List<String>();

            foreach (var pair in dictWallMonitoring)
            {
                long _ownerId = pair.Key;
                Dictionary<long, long> _postIds = pair.Value;
                foreach (var _postIdDict in _postIds)
                {
                    lstTS.Add(_ownerId.ToString() + "|" + _postIdDict.Key.ToString() + "|" + _postIdDict.Value.ToString());
                }
            }
            FileWriteAllLines(Path.Combine(sDataPath, "_wall_posts_" + iPersUserID.ToString() + ".txt"), lstTS, Encoding.UTF8);
        }

        private void Wall_LoadPostToMonitoring()
        {
            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_wall_posts_" + iPersUserID.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_wall_posts_" + iPersUserID.ToString() + ".txt"));
                    lstTS = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstTS = new List<String>();
                }
            }

            dictWallMonitoring = new Dictionary<long, Dictionary<long, long>>();
            foreach (String value in lstTS)
            {
                try
                {
                    string str = value;
                    long _ownerId = Convert.ToInt64(str.Substring(0, str.IndexOf("|")));
                    str = str.Substring(str.IndexOf("|") + 1);
                    long _postId = Convert.ToInt64(str.Substring(0, str.IndexOf("|")));
                    str = str.Substring(str.IndexOf("|") + 1);
                    long _lastcommentId = Convert.ToInt64(str);

                    if (!dictWallMonitoring.ContainsKey(_ownerId))
                        dictWallMonitoring.Add(_ownerId, new Dictionary<long, long>());

                    if (!dictWallMonitoring[_ownerId].ContainsKey(_postId))
                        dictWallMonitoring[_ownerId].Add(_postId, _lastcommentId);
                }
                catch
                {

                }
            }
        }

        private Dictionary<long, Dictionary<long, long>> Wall_UserLoadPostToMonitoring(long _uid)
        {
            List<String> lstTS = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_wall_posts_" + _uid.ToString() + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_wall_posts_" + _uid.ToString() + ".txt"));
                    lstTS = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstTS = new List<String>();
                }
            }

            Dictionary<long, Dictionary<long, long>> _dictWallMonitoring = new Dictionary<long, Dictionary<long, long>>();
            foreach (String value in lstTS)
            {
                try
                {
                    string str = value;
                    long _ownerId = Convert.ToInt64(str.Substring(0, str.IndexOf("|")));
                    str = str.Substring(str.IndexOf("|") + 1);
                    long _postId = Convert.ToInt64(str.Substring(0, str.IndexOf("|")));
                    str = str.Substring(str.IndexOf("|") + 1);
                    long _lastcommentId = Convert.ToInt64(str);

                    if (!_dictWallMonitoring.ContainsKey(_ownerId))
                        _dictWallMonitoring.Add(_ownerId, new Dictionary<long, long>());

                    if (!_dictWallMonitoring[_ownerId].ContainsKey(_postId))
                        _dictWallMonitoring[_ownerId].Add(_postId, _lastcommentId);
                }
                catch
                {

                }
            }

            return _dictWallMonitoring;
        }

        private void Wall_UserAddPostToMonitoring(long _uid, long _ownerId, long _postId, long _lastcommentId = 0)
        {
            Dictionary<long, Dictionary<long, long>> _dictWallMonitoring = Wall_UserLoadPostToMonitoring(_uid);

            if (!_dictWallMonitoring.ContainsKey(_ownerId))
                _dictWallMonitoring.Add(_ownerId, new Dictionary<long, long>());

            if (!_dictWallMonitoring[_ownerId].ContainsKey(_postId))
                _dictWallMonitoring[_ownerId].Add(_postId, _lastcommentId);
            else
                _dictWallMonitoring[_ownerId][_postId] = _lastcommentId;

            Wall_UserSavePostToMonitoring(_uid, _dictWallMonitoring);
        }

        private void Wall_AddPostToMonitoring(long _ownerId, long _postId, long _lastcommentId = 0)
        {
            if (!dictWallMonitoring.ContainsKey(_ownerId))
                dictWallMonitoring.Add(_ownerId, new Dictionary<long, long>());

            if (!dictWallMonitoring[_ownerId].ContainsKey(_postId))
                dictWallMonitoring[_ownerId].Add(_postId, _lastcommentId);
            else
                dictWallMonitoring[_ownerId][_postId] = _lastcommentId;

            Wall_SavePostToMonitoring();
        }

        private void tbChangeCommunicationModeContacter_Click(object sender, EventArgs e)
        {
            onManualButtonClick();
            ChangeCommunicationMode(0);
        }

        private void tbChangeCommunicationModeGroups_Click(object sender, EventArgs e)
        {
            onManualButtonClick();
            ChangeCommunicationMode(1);
        }

        private void ChangeCommunicationMode(int _iMode)
        {
            // manual set timers
            StopAnswerTimer();
            tbChangeCommunicationMode.Image = _iMode == 0 ? Nilsa.Properties.Resources._mode_contacter : Nilsa.Properties.Resources._mode_groups;
            SaveReceivedMessagesPull();
            iContactsGroupsMode = _iMode;
            LoadReceivedMessagesPull();
            iContUserID = -1;
            ContactsList_Load();
            LoadContactParamersValues();
            SetContactParametersValues();
            LoadAlgorithmSettingsContacter();
            //LoadContactParametersDescription();

            OnSelectOtherContacter();
            SelectNextReceivedMessage(false);
        }

        private void labelInEqMsgHarTitleMarker_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(brushContact3, e.ClipRectangle);
            TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine;
            TextRenderer.DrawText(e.Graphics, labelInEqMsgHarTitleMarker.Text, labelInEqMsgHarTitleMarker.Font, e.ClipRectangle, labelInEqMsgHarTitleMarker.ForeColor, flags);
        }

        private void labelOutEqMsgHarTitleMarker_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(brushPerson3, e.ClipRectangle);
            TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine;
            TextRenderer.DrawText(e.Graphics, labelOutEqMsgHarTitleMarker.Text, labelOutEqMsgHarTitleMarker.Font, e.ClipRectangle, labelOutEqMsgHarTitleMarker.ForeColor, flags);
        }

        private void tbEditSetList_Click(object sender, EventArgs e)
        {
            FormEditSetList feum = new FormEditSetList(this);
            feum.Setup();
            feum.ShowDialog();
        }

        private List<DBPersonListItem> getAdditionPersonList(string message)
        {
            List<DBPersonListItem> listPerson = new List<DBPersonListItem>();

            string list = getTagValue(message, "<ADD_PERS_LIST>", "</ADD_PERS_LIST>");
            if (list.Length > 0)
            {
                string item = getTagValue(list, "<AP>", "</AP>");
                while (item.Length > 0)
                {
                    string itemID = getTagValue(item, "<APID>", "</APID>");
                    string itemNAME = getTagValue(item, "<APNAME>", "</APNAME>");

                    if (itemID.Length > 0 && itemNAME.Length > 0)
                    {
                        listPerson.Add(new DBPersonListItem(0, itemID, itemNAME));
                    }

                    list = removeTagValue(list, "<AP>", "</AP>");
                    item = getTagValue(list, "<AP>", "</AP>");
                }

            }
            return listPerson;
        }

        private string putAdditionalPersonenList(List<DBPersonListItem> listBoxPersonen_Items)
        {
            if (listBoxPersonen_Items.Count == 0)
                return "";

            string str = "<ADD_PERS_LIST>";
            foreach (DBPersonListItem pers in listBoxPersonen_Items)
                str += "<AP><APID>" + pers.UserID + "</APID><APNAME>" + pers.UserName + "</APNAME></AP>";
            str += "</ADD_PERS_LIST>";

            return str;
        }


        private string getTagValue(string text, string tagstart, string tagend)
        {
            string retval = "";
            if (text.Contains(tagstart) && text.Contains(tagend))
            {
                retval = text.Substring(text.IndexOf(tagstart) + tagstart.Length);
                retval = retval.Substring(0, retval.IndexOf(tagend));
            }
            return retval;
        }

        private string removeTagValue(string text, string tagstart, string tagend)
        {
            string retval = "";
            if (text.Contains(tagstart) && text.Contains(tagend))
            {
                retval = text.Substring(0, text.IndexOf(tagstart));
                retval += text.Substring(text.IndexOf(tagend) + tagend.Length);
            }
            else
                retval = text;

            return retval;
        }

        private void putAdditionalPersoneMessage(string sUID, string msgTEXT, long _iGroupAnswerID, long _iGroupAnswerPostID, long _iGroupAnswerCommentID, long _UserID, List<DBPersonListItem> listBoxPersonen_Items)
        {
            DateTime dt = DateTime.Now;
            if (sUID == iPersUserID.ToString())
            {
                //0 | 117306898 / 46 / 47 / 28109885 | 29.05.2016 | 9:45 | Привет вованыч
                lstReceivedMessages.Insert(0, "0|" + Convert.ToString(_iGroupAnswerID) + "/" + Convert.ToString(_iGroupAnswerPostID) + "/" + Convert.ToString(_iGroupAnswerCommentID) + "/" + Convert.ToString(_UserID) + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + msgTEXT + putAdditionalPersonenList(listBoxPersonen_Items));
            }
            else
            {
                List<String> lstReceivedMessagesTemp = new List<String>();
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix() + sUID + "_groups" + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix() + sUID + "_groups" + ".txt"));
                        lstReceivedMessagesTemp = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        lstReceivedMessagesTemp = new List<String>();
                    }
                }
                lstReceivedMessagesTemp.Insert(0, "0|" + Convert.ToString(_iGroupAnswerID) + "/" + Convert.ToString(_iGroupAnswerPostID) + "/" + Convert.ToString(_iGroupAnswerCommentID) + "/" + Convert.ToString(_UserID) + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|" + msgTEXT + putAdditionalPersonenList(listBoxPersonen_Items));
                FileWriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_received_pull_" + FormMain.getSocialNetworkPrefix() + sUID + "_groups" + ".txt"), lstReceivedMessagesTemp, Encoding.UTF8);
            }

            if (File.Exists(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + sUID + "_" + Convert.ToString(_UserID) + ".txt")))
                File.Delete(Path.Combine(sDataPath, "_algotithm_settings_" + getSocialNetworkPrefix() + sUID + "_" + Convert.ToString(_UserID) + ".txt"));
        }

        private void apiMessagesSend(long id, bool isChat, string message, string title = "", IEnumerable<MediaAttachment> attachment = null, IEnumerable<long> forwardMessagedIds = null, bool fromChat = false, double? latitude = default(double?), double? longitude = default(double?), string guid = null, long? captchaSid = default(long?), string captchaKey = null, string attachmentslist = null)
        {
            if (iPersUserID >= 0 && SocialNetwork == 0)
            {
                stopTimers();
                /*
                FormWebBrowser fwb = new FormWebBrowser(this);
                fwb.Setup(userLogin, userPassword, WebBrowserCommand.SendMessage, id, message, "", userName);
                fwb.ShowDialog();
                */
                addToHistory(iPersUserID, id, false, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), NilsaUtils.TextToString(message));

                ShowBrowserCommand();

                fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.SendMessage, id, message, "", userName);
                //fwbVKontakte.Show();
                fwbVKontakte.WaitResult();

                HideBrowserCommand();

                if (fwbVKontakte.errorSendMessage)
                    GenerateFaceContacterMessage(id, "ERROR_SEND_MESSAGE ");
                startTimers();
            }
        }

        private void apiFriendsAdd(long id, bool isChat, string message)
        {
            if (iPersUserID >= 0 && SocialNetwork == 0)
            {
                stopTimers();
                if (message.Length>0)
                    addToHistory(iPersUserID, id, false, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), NilsaUtils.TextToString(message));

                ShowBrowserCommand();

                fwbVKontakte.Setup(userLogin, userPassword, WebBrowserCommand.AddToFriends, id, message, "", userName);
                //fwbVKontakte.Show();
                fwbVKontakte.WaitResult();

                HideBrowserCommand();

                if (fwbVKontakte.errorSendMessage)
                    GenerateFaceContacterMessage(id, "ERROR_ADD_FRIENDS ");
                startTimers();
            }
        }

        private void timerOutgoingPull_Tick(object sender, EventArgs e)
        {
            timerPhysicalSendStop();
            if (lstOutgoingMessages.Count > 0)
            {
                bool bErrorParsing = false;
                String srec = lstOutgoingMessages[0];
                lstOutgoingMessages.RemoveAt(0);
                long contid = 0;
                long _iGroupAnswerID = -1;
                long _iGroupAnswerPostID = -1;
                long _iGroupAnswerCommentID = -1;
                String message = "";
                long channel = 0; // 0 - ЛС, 1 - стена
                string name = "";
                try
                {
                    if (srec.StartsWith("*#|"))
                    {
                        message = srec.Substring(srec.IndexOf("|") + 1);
                        channel = Convert.ToInt64(message.Substring(0, message.IndexOf("|")));
                        message = message.Substring(message.IndexOf("|") + 1);
                        if (channel == 4)
                        {
                            string strUsrId = message.Substring(0, message.IndexOf("|"));
                            //Convert.ToString(_ownerId)+"/"+ Convert.ToString(_postId) + "/"+ Convert.ToString(msg.Id) + "/" + Convert.ToString(msg.FromId) + "|";
                            _iGroupAnswerID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                            strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                            _iGroupAnswerPostID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                            strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                            _iGroupAnswerCommentID = Convert.ToInt64(strUsrId.Substring(0, strUsrId.IndexOf('/')));
                            strUsrId = strUsrId.Substring(strUsrId.IndexOf('/') + 1);
                            contid = Convert.ToInt64(strUsrId);

                        }
                        else
                            contid = Convert.ToInt64(message.Substring(0, message.IndexOf("|")));

                        message = message.Substring(message.IndexOf("|") + 1);
                        name = message.Substring(message.IndexOf("|") + 1);
                        message = message.Substring(message.IndexOf("|") + 1);

                        if (contid < 0 && channel == 0)
                        {
                            contid = -contid;
                            channel = 3;
                        }

                    }
                    else
                    {
                        contid = Convert.ToInt64(srec.Substring(0, srec.IndexOf("|")));
                        message = srec.Substring(srec.IndexOf("|") + 1);
                        message = message.Substring(message.IndexOf("|") + 1);
                    }
                }
                catch (Exception exp)
                {
                    bErrorParsing = true;
                    ExceptionToLogList("timerOutgoingPull_Tick Parsing srec error", contid.ToString() + ", " + srec, exp);
                }
                finally { }

                if (!bErrorParsing)
                {
                    /*
                    bool bNotDelayed = true;
                    if (message.IndexOf("time_delay:") >= 0)
                    {
                        bNotDelayed = false;
                        String timeDelay = message.Substring(message.IndexOf("time_delay:") + 11).Trim();

                        int iDelayType = 1;
                        if (timeDelay.EndsWith("min"))
                        {
                            iDelayType = 0;
                            timeDelay = timeDelay.Substring(0, timeDelay.IndexOf("min")).Trim();
                        }
                        else if (timeDelay.EndsWith("hour"))
                        {
                            iDelayType = 1;
                            timeDelay = timeDelay.Substring(0, timeDelay.IndexOf("hour")).Trim();
                        }
                        else if (timeDelay.EndsWith("day"))
                        {
                            iDelayType = 2;
                            timeDelay = timeDelay.Substring(0, timeDelay.IndexOf("day")).Trim();
                        }

                        int iDelay = -1;
                        try
                        {
                            iDelay = Convert.ToInt32(timeDelay);
                        }
                        catch
                        {
                            iDelay = -1;
                        }

                        if (iDelay >= 0)
                        {
                            String text = srec.Substring(0, srec.IndexOf("time_delay:"));
                            DateTime dtDelay = DateTime.Now;
                            if (iDelayType == 0)
                                dtDelay = dtDelay.AddMinutes((double)iDelay);
                            else if (iDelayType == 1)
                                dtDelay = dtDelay.AddHours((double)iDelay);
                            else if (iDelayType == 2)
                                dtDelay = dtDelay.AddDays((double)iDelay);

                            lstOutgoingMessagesDelayed.Add(dtDelay.ToBinary().ToString() + "|" + text);
                            SaveOutgoingDelayedMessagesPull();
                        }
                    }
                    */

                    if (((message.Length > 0) || (channel == 2))/* && bNotDelayed*/)
                    {
                        cntE1++;
                        cntE2++;
                        cntE7++;
                        cntE8++;
                        SaveProgramCountersE1E2E3();
                        UpdateProgramCountersInfoE1E2E3();

                        cntE4++;
                        cntE5++;
                        SaveProgramCountersE4E5E6();
                        UpdateProgramCountersInfoE4E5E6();

                        if (SocialNetwork == 0)
                        {
                            try
                            {
                                if (channel == 0)
                                {
                                    string attachlist = null;
                                    if (message.IndexOf("attach:") >= 0)
                                    {
                                        attachlist = message.Substring(message.IndexOf("attach:") + 7);
                                        if (attachlist.Length == 0)
                                            attachlist = null;
                                    }

                                    if (attachlist == null)
                                    {
                                        apiMessagesSend(contid, false, message);
                                    }
                                    else
                                        apiMessagesSend(contid, false, message.Substring(0, message.IndexOf("attach:")), "", null, null, false, null, null, null, null, null, attachlist);
                                }
                                else if (channel == 1)
                                {
                                    /*
                                    string attachlist = null;
                                    if (message.IndexOf("wallattach:") >= 0)
                                    {
                                        attachlist = message.Substring(message.IndexOf("wallattach:") + 11);
                                        if (attachlist.Length == 0)
                                            attachlist = null;
                                    }

                                    if (attachlist == null)
                                    {
                                        //api.Messages.Send(contid, false, message);
                                        api.Wall.Post(contid, false, false, NilsaUtils.StringToText(message));
                                    }
                                    else
                                        api.Wall.Post(contid, false, false, NilsaUtils.StringToText(message.Substring(0, message.IndexOf("wallattach:"))), null, null, null, false, null, null, null, null, null, null, null, attachlist);
                                        */
                                }
                                else if (channel == 2)
                                {
                                    //api.Friends.Add(contid, message);
                                    apiFriendsAdd(contid, false, message);
                                }
                                else if (channel == 3)
                                {
                                    /*
                                    string attachlist = null;
                                    List<DBPersonListItem> listAdditionalPersonen = getAdditionPersonList(message);
                                    string msgrealtext = removeTagValue(message, "<ADD_PERS_LIST>", "</ADD_PERS_LIST>");
                                    if (msgrealtext.IndexOf("attach:") >= 0)
                                    {
                                        attachlist = msgrealtext.Substring(msgrealtext.IndexOf("attach:") + 7);
                                        if (attachlist.Length == 0)
                                            attachlist = null;
                                    }

                                    long postId = -1;
                                    if (attachlist == null)
                                    {
                                        postId = api.Wall.Post(-contid, false, false, msgrealtext);
                                    }
                                    else
                                        postId = api.Wall.Post(-contid, false, false, msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")), null, attachlist);

                                    if (postId >= 0)
                                    {
                                        Wall_AddPostToMonitoring(contid, postId);

                                        if (listAdditionalPersonen.Count > 0)
                                        {
                                            DBPersonListItem personListItem = listAdditionalPersonen[0];
                                            listAdditionalPersonen.RemoveAt(0);
                                            putAdditionalPersoneMessage(personListItem.UserID, msgrealtext.IndexOf("attach:") >= 0 ? msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")) : msgrealtext, contid, postId, postId, iPersUserID, listAdditionalPersonen);

                                            if (!personListItem.UserID.Equals(iPersUserID.ToString()))
                                                Wall_UserAddPostToMonitoring(Convert.ToInt64(personListItem.UserID), contid, postId);
                                        }
                                    }
                                    */
                                }
                                else if (channel == 4)
                                {
                                    /*
                                    string attachlist = null;
                                    List<DBPersonListItem> listAdditionalPersonen = getAdditionPersonList(message);
                                    string msgrealtext = removeTagValue(message, "<ADD_PERS_LIST>", "</ADD_PERS_LIST>");
                                    if (msgrealtext.IndexOf("attach:") >= 0)
                                    {
                                        attachlist = msgrealtext.Substring(msgrealtext.IndexOf("attach:") + 7);
                                        if (attachlist.Length == 0)
                                            attachlist = null;
                                    }

                                    long postId = -1;
                                    if (attachlist == null)
                                    {
                                        if ((_iGroupAnswerPostID == _iGroupAnswerCommentID))
                                            postId = api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext);
                                        else
                                            postId = api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext, _iGroupAnswerCommentID);
                                    }
                                    else
                                    {
                                        if ((_iGroupAnswerPostID == _iGroupAnswerCommentID))
                                            postId = api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")), null, attachlist);
                                        else
                                            postId = api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")), _iGroupAnswerCommentID, attachlist);
                                    }

                                    if (postId >= 0)
                                    {
                                        if (listAdditionalPersonen.Count > 0)
                                        {
                                            DBPersonListItem personListItem = listAdditionalPersonen[0];
                                            listAdditionalPersonen.RemoveAt(0);
                                            putAdditionalPersoneMessage(personListItem.UserID, msgrealtext.IndexOf("attach:") >= 0 ? msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")) : msgrealtext, _iGroupAnswerID, _iGroupAnswerPostID, postId, iPersUserID, listAdditionalPersonen);

                                            if (!personListItem.UserID.Equals(iPersUserID.ToString()))
                                                Wall_UserAddPostToMonitoring(Convert.ToInt64(personListItem.UserID), _iGroupAnswerID, _iGroupAnswerPostID);
                                        }
                                    }
                                    */
                                }
                            }
                            /*
                            catch (VkNet.Exception.CaptchaNeededException ex)
                            {
                                bool bStatusService = !tbStartService.Enabled;
                                tbStopService_Click(null, null);

                                long? captcha_sid = ex.Sid;
                                string captcha_key = null;
                                Uri imguri = ex.Img;
                                bool bCycle = true;
                                do
                                {
                                    var form = new FormCaptcha(this, imguri, captcha_sid);
                                    DialogResult dr = DialogResult.Cancel;
                                    try
                                    {
                                        dr = form.ShowDialog();
                                    }
                                    catch
                                    {
                                        dr = DialogResult.Cancel;
                                    }
                                    if (dr == DialogResult.OK)
                                    {
                                        captcha_key = form.CaptchaKey.Text;
                                        //---
                                        try
                                        {
                                            if (channel == 0)
                                            {
                                                string attachlist = null;
                                                if (message.IndexOf("attach:") >= 0)
                                                {
                                                    attachlist = message.Substring(message.IndexOf("attach:") + 7);
                                                    if (attachlist.Length == 0)
                                                        attachlist = null;
                                                }

                                                if (attachlist == null)
                                                {
                                                    apiMessagesSend(contid, false, message, "", null, null, false, null, null, null, captcha_sid, captcha_key);
                                                }
                                                else
                                                    apiMessagesSend(contid, false, message.Substring(0, message.IndexOf("attach:")), "", null, null, false, null, null, null, captcha_sid, captcha_key, attachlist);
                                            }
                                            else if (channel == 1)
                                            {
                                                //GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? "VkNet.Exception.CaptchaNeededException" : ""));
                                                //bCycle = false;
                                                string attachlist = null;
                                                if (message.IndexOf("wallattach:") >= 0)
                                                {
                                                    attachlist = message.Substring(message.IndexOf("wallattach:") + 11);
                                                    if (attachlist.Length == 0)
                                                        attachlist = null;
                                                }

                                                if (attachlist == null)
                                                {
                                                    //api.Messages.Send(contid, false, message);
                                                    api.Wall.Post(contid, false, false, NilsaUtils.StringToText(message), null, null, null, false, null, null, null, null, null, captcha_sid, captcha_key);
                                                }
                                                else
                                                    api.Wall.Post(contid, false, false, NilsaUtils.StringToText(message.Substring(0, message.IndexOf("wallattach:"))), null, null, null, false, null, null, null, null, null, captcha_sid, captcha_key, attachlist);
                                            }
                                            else if (channel == 2)
                                            {
                                                api.Friends.Add(contid, message, captcha_sid, captcha_key);
                                            }
                                            else if (channel == 3)
                                            {
                                                string attachlist = null;
                                                List<DBPersonListItem> listAdditionalPersonen = getAdditionPersonList(message);
                                                string msgrealtext = removeTagValue(message, "<ADD_PERS_LIST>", "</ADD_PERS_LIST>");
                                                if (msgrealtext.IndexOf("attach:") >= 0)
                                                {
                                                    attachlist = msgrealtext.Substring(msgrealtext.IndexOf("attach:") + 7);
                                                    if (attachlist.Length == 0)
                                                        attachlist = null;
                                                }

                                                long postId = -1;
                                                if (attachlist == null)
                                                {
                                                    postId = api.Wall.Post(-contid, false, false, msgrealtext, null, null, null, false, null, null, null, null, null, captcha_sid, captcha_key);
                                                }
                                                else
                                                    postId = api.Wall.Post(-contid, false, false, msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")), null, attachlist, null, false, null, null, null, null, null, captcha_sid, captcha_key);

                                                if (postId >= 0)
                                                {
                                                    Wall_AddPostToMonitoring(contid, postId);

                                                    if (listAdditionalPersonen.Count > 0)
                                                    {
                                                        DBPersonListItem personListItem = listAdditionalPersonen[0];
                                                        listAdditionalPersonen.RemoveAt(0);
                                                        putAdditionalPersoneMessage(personListItem.UserID, msgrealtext.IndexOf("attach:") >= 0 ? msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")) : msgrealtext, contid, postId, postId, iPersUserID, listAdditionalPersonen);

                                                        if (!personListItem.UserID.Equals(iPersUserID.ToString()))
                                                            Wall_UserAddPostToMonitoring(Convert.ToInt64(personListItem.UserID), contid, postId);
                                                    }
                                                }

                                            }
                                            else if (channel == 4)
                                            {
                                                string attachlist = null;
                                                List<DBPersonListItem> listAdditionalPersonen = getAdditionPersonList(message);
                                                string msgrealtext = removeTagValue(message, "<ADD_PERS_LIST>", "</ADD_PERS_LIST>");
                                                if (msgrealtext.IndexOf("attach:") >= 0)
                                                {
                                                    attachlist = msgrealtext.Substring(msgrealtext.IndexOf("attach:") + 7);
                                                    if (attachlist.Length == 0)
                                                        attachlist = null;
                                                }

                                                if (attachlist == null)
                                                {
                                                    api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext, _iGroupAnswerCommentID, null, captcha_sid, captcha_key);
                                                }
                                                else
                                                    api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")), _iGroupAnswerCommentID, attachlist, captcha_sid, captcha_key);

                                                long postId = -1;
                                                if (attachlist == null)
                                                {
                                                    if ((_iGroupAnswerPostID == _iGroupAnswerCommentID))
                                                        postId = api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext, null, null, captcha_sid, captcha_key);
                                                    else
                                                        postId = api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext, _iGroupAnswerCommentID, null, captcha_sid, captcha_key);
                                                }
                                                else
                                                {
                                                    if ((_iGroupAnswerPostID == _iGroupAnswerCommentID))
                                                        postId = api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")), null, attachlist, captcha_sid, captcha_key);
                                                    else
                                                        postId = api.Wall.AddComment(-_iGroupAnswerID, _iGroupAnswerPostID, false, msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")), _iGroupAnswerCommentID, attachlist, captcha_sid, captcha_key);
                                                }

                                                if (postId >= 0)
                                                {
                                                    if (listAdditionalPersonen.Count > 0)
                                                    {
                                                        DBPersonListItem personListItem = listAdditionalPersonen[0];
                                                        listAdditionalPersonen.RemoveAt(0);
                                                        putAdditionalPersoneMessage(personListItem.UserID, msgrealtext.IndexOf("attach:") >= 0 ? msgrealtext.Substring(0, msgrealtext.IndexOf("attach:")) : msgrealtext, _iGroupAnswerID, _iGroupAnswerPostID, postId, iPersUserID, listAdditionalPersonen);

                                                        if (!personListItem.UserID.Equals(iPersUserID.ToString()))
                                                            Wall_UserAddPostToMonitoring(Convert.ToInt64(personListItem.UserID), _iGroupAnswerID, _iGroupAnswerPostID);
                                                    }
                                                }

                                            }
                                            bCycle = false;
                                        }
                                        catch (VkNet.Exception.CaptchaNeededException ex1)
                                        {
                                            captcha_sid = ex1.Sid;
                                            imguri = ex1.Img;
                                        }
                                        catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                                        {
                                            ReAutorize(userLogin, userPassword);
                                            lstOutgoingMessages.Add(srec);
                                            bCycle = false;
                                        }
                                        catch (System.Net.WebException)
                                        {
                                            ReAutorize(userLogin, userPassword);
                                            lstOutgoingMessages.Add(srec);
                                            bCycle = false;
                                        }
                                        catch (VkNet.Exception.VkApiException vkexpapi)
                                        {
                                            if (vkexpapi as VkNet.Exception.AccessDeniedException != null)
                                            {
                                                GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? vkexpapi.Message : ""));
                                                ExceptionToLogList("timerOutgoingPull_Tick", contid.ToString() + ", " + message, vkexpapi);
                                            }
                                            else if (vkexpapi as VkNet.Exception.PostLimitException != null)
                                            {
                                                GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? vkexpapi.Message : ""));
                                                ExceptionToLogList("timerOutgoingPull_Tick", contid.ToString() + ", " + message, vkexpapi);
                                            }
                                            else if (vkexpapi.Message != null)
                                            {
                                                ExceptionToLogList("timerOutgoingPull_Tick", contid.ToString() + ", " + message, vkexpapi);
                                                if (vkexpapi.Message.IndexOf("Flood control") < 0)
                                                {
                                                    GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? vkexpapi.Message : ""));
                                                }
                                                if (vkexpapi.Message.IndexOf("Базовое соединение закрыто") > 0)
                                                {
                                                    ReAutorize(userLogin, userPassword);
                                                    lstOutgoingMessages.Add(srec);
                                                }
                                            }
                                            else
                                            {
                                                ReAutorize(userLogin, userPassword);
                                                lstOutgoingMessages.Add(srec);
                                            }
                                            bCycle = false;
                                        }
                                        catch (Exception exp)
                                        {
                                            //GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? exp.Message : ""));
                                            ExceptionToLogList("timerOutgoingPull_Tick", contid.ToString() + ", " + message, exp);
                                        }
                                        //---
                                    }
                                    else
                                    {
                                        GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? "VkNet.Exception.CaptchaNeededException" : ""));
                                        //ExceptionToLogList("timerOutgoingPull_Tick", contid.ToString() + ", " + message, "VkNet.Exception.CaptchaNeededException");
                                        bCycle = false;
                                    }
                                }
                                while (bCycle);

                                if (bStatusService)
                                    tbStartService_Click(null, null);
                            }
                            catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                            {
                                ReAutorize(userLogin, userPassword);
                                lstOutgoingMessages.Add(srec);
                            }
                            catch (System.Net.WebException)
                            {
                                ReAutorize(userLogin, userPassword);
                                lstOutgoingMessages.Add(srec);
                            }
                            catch (VkNet.Exception.VkApiException vkexpapi)
                            {
                                if (vkexpapi as VkNet.Exception.AccessDeniedException != null)
                                {
                                    GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? vkexpapi.Message : ""));
                                    ExceptionToLogList("timerOutgoingPull_Tick", contid.ToString() + ", " + message, vkexpapi);
                                }
                                else if (vkexpapi as VkNet.Exception.PostLimitException != null)
                                {
                                    GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? vkexpapi.Message : ""));
                                    ExceptionToLogList("timerOutgoingPull_Tick", contid.ToString() + ", " + message, vkexpapi);
                                }
                                else if (vkexpapi.Message != null)
                                {
                                    ExceptionToLogList("timerOutgoingPull_Tick", contid.ToString() + ", " + message, vkexpapi);
                                    if (vkexpapi.Message.IndexOf("Flood control") < 0)
                                    {
                                        GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? vkexpapi.Message : ""));
                                    }
                                    if (vkexpapi.Message.IndexOf("Базовое соединение закрыто") > 0)
                                    {
                                        ReAutorize(userLogin, userPassword);
                                        lstOutgoingMessages.Add(srec);
                                    }
                                }
                                else
                                {
                                    ReAutorize(userLogin, userPassword);
                                    lstOutgoingMessages.Add(srec);
                                }

                            }
                            */
                            catch (Exception exp)
                            {
                                //lstOutgoingMessages.Add(srec);
                                GenerateFaceContacterMessage(contid, "ERROR_SEND_MESSAGE " + (adbrCurrent.ShowErrorDetails ? exp.Message : ""));
                                ExceptionToLogList("timerOutgoingPull_Tick", contid.ToString() + ", " + message, exp);
                            }
                            finally { }

                            // 2019-04-13
                            if ((channel == 0 || channel == 4) && contid == iContUserID && iContUserID != -1)
                                ReadAllUserMessages(iContUserID);
                        }
                        else if (SocialNetwork == 1)
                        {
                            NILSA_SendMessage(iPersUserID, contid, message);
                        }
                    }
                }
                //--
            }

            if (!tbStartService.Enabled && lstOutgoingMessages.Count > 0)
            {
                timerPhysicalSendStart();
            }

        }

        private string NILSA_ARCHIVE_Create(string strExternalZipFileName = "", int[] _settings = null)
        {
            string strZipFileName = strExternalZipFileName.Length > 0 ? strExternalZipFileName : "nilsa_" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_") + ".nilsa.zip";
            string strZipFilePath = Path.Combine(Application.StartupPath, strZipFileName);

            if (File.Exists(strZipFilePath))
                File.Delete(strZipFilePath);

            using (ZipArchive newZipArchive = ZipFile.Open(strZipFilePath, ZipArchiveMode.Create))
            {
                if (_settings == null)
                {
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "*.txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "*.columns"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "*.values"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "*.*"), Application.StartupPath);
                }
                else
                {
                    if (_settings[0] == 1)
                    {
                        /*
                            Алгоритмы (Импортировать, Не менять, Очистить):
                            _algorithms_db.txt
                            _algorithms_*.txt
                            _text_search_settings_*.txt
                            _algtimers_settings_*.txt
                        */
                        //NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_algorithms_db.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_algorithms_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_text_search_settings_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_algtimers_settings_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "FormEditPersHarValues_2_*.values"), Application.StartupPath);
                    }
                    if (_settings[1] == 1)
                    {
                        /*
                            Базы сообщений Персонажа и Контактера (Импортировать, Не менять, Очистить):
                            _messages_db.txt
                            _eqinmsgdb_*.txt
                            _eqoutmsgdb_*.txt
                            LongValues0.values
                            LongValues1.values
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_messages_db.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_eqinmsgdb_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_eqoutmsgdb_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "LongValues0.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "LongValues1.values"), Application.StartupPath);
                    }
                    if (_settings[2] == 1)
                    {
                        /*
                            Настройки инициации диалогов (Импортировать, Не менять, Очистить):
                            StringValues2.values
                            LongValues2.values
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "StringValues2.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "StringValues3.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "LongValues2.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "LongValues3.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "LongValues4.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "LongValues117.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "LongValues9.values"), Application.StartupPath);

                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues_3*.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesGroups_3*.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEnterContactersToImportCount_3*.values"), Application.StartupPath);

                    }
                    if (_settings[3] == 1)
                    {
                        /*
                            База Персонажей (Импортировать, Не менять, Очистить):
                            _personen*.txt
                            pers_*.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_personen*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "pers_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_dialogsinitperday_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "_program_state*.txt"), Application.StartupPath);
                    }
                    if (_settings[4] == 1)
                    {
                        /*
                            Пулы входящих и исходящих сообщений Персонажей (Импортировать, Не менять, Очистить):
                            _msg_received_pull_*.txt
                            _msg_outgoing_pull_*.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_msg_received_pull_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_msg_outgoing_pull_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_msg_outdelayed_pull_*.txt"), Application.StartupPath);
                    }
                    if (_settings[5] == 1)
                    {
                        /*
                            Разрешенные алгоритмы Персонажей (Импортировать, Не менять, Очистить):
                            _enabledalgorithms_*.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_enabledalgorithms_*.txt"), Application.StartupPath);
                    }
                    if (_settings[6] == 1)
                    {
                        /*
                            Инициация диалогов Персонажей (Импортировать, Не менять, Очистить):
                            _initdialog_*.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_initdialog_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "resend_operator_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_wall_posts_*.txt"), Application.StartupPath);
                    }
                    if (_settings[7] == 1)
                    {
                        /*
                            Базы Контактеров Персонажей (Импортировать, Не менять, Очистить):
                            _contacts_*.txt
                            cont_*_*.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_contacts_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "cont_*_*.txt"), Application.StartupPath);

                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_groups_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "grp_*_*.txt"), Application.StartupPath);
                    }
                    if (_settings[8] == 1)
                    {
                        /*
                            Привязки алгоритмов к Контактерам и Персонажам (Импортировать, Не менять, Очистить):
                            _algotithm_settings_*.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_algotithm_settings_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_algotithm_settings_*_*.txt"), Application.StartupPath);
                    }
                    if (_settings[9] == 1)
                    {
                        /*
                            Флаги инициации диалога с Контактерами (Импортировать, Не менять, Очистить):
                            _flag_init_dialog_*_*.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_flag_init_dialog_*_*.txt"), Application.StartupPath);
                    }
                    if (_settings[10] == 1)
                    {
                        /*
                            Последние отосланные Контактерам сообщения (Импортировать, Не менять, Очистить):
                            _lastmessage_*_*.txt
                            _prevlastmessage_*_*.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_lastmessage_*_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_prevlastmessage_*_*.txt"), Application.StartupPath);
                    }
                    if (_settings[11] == 1)
                    {
                        /*
                            Настройки характеристик Контактера (Импортировать, Не менять, Очистить):
                            _conthar.txt
                            _cont_har_*.txt
                            FormEditContactsDB.columns
                            FormEditPersHarValues_*_*.values
                            FormEditPersHarValuesStatusUpdate.values
                            FormEditPersHarValuesAlgorithmUpdate.values
                            StringValues0.values
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_conthar.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_cont_har_*.txt"), Application.StartupPath);

                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_grouphar.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_grp_har_*.txt"), Application.StartupPath);

                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditContactsDB.columns"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues_0_*.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues_1_*.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesStatusUpdate.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "StringValues0.values"), Application.StartupPath);
                    }
                    if (_settings[12] == 1)
                    {
                        /*
                            Настройки характеристик Персонажа (Импортировать, Не менять, Очистить):
                            _pershar.txt
                            _pers_har_*.txt
                            FormEditPersonenDB.columns
                            FormEditPersHarValuesPersonen_*_*.values
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_pershar.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_pers_har_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersonenDB.columns"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesPersonen_*_*.values"), Application.StartupPath);
                    }
                    if (_settings[13] == 1)
                    {
                        /*
                            Настройки характеристик сообщений (Импортировать, Не менять, Очистить):
                            _msghar.txt
                            _msg_har_*.txt
                            _msg_har_*_*.txt
                            FormEditEQInMessagesDB.columns
                            FormEditEQOutMessagesDB.columns
                            FormEditPersHarValuesEQIn_*_*.values
                            FormEditPersHarValuesEQOut_*_*.values
                            _default_db_name.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_msghar.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_msg_har_*.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditEQInMessagesDB.columns"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesEQIn_*_*.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditEQOutMessagesDB.columns"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesEQOut_*_*.values"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_default_db_name.txt"), Application.StartupPath);
                    }
                    if (_settings[14] == 1)
                    {
                        /*
                            Счетчики программы (Импортировать, Не менять, Очистить):
                            _program_counters_*.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "_program_counters_*.txt"), Application.StartupPath);
                    }
                    if (_settings[15] == 1)
                    {
                        /*
                            Настройки цветов (Импортировать, Не менять, Очистить):
                            _colors_settings.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "_colors_settings.txt"), Application.StartupPath);
                    }
                    if (_settings[16] == 1)
                    {
                        /*
                            Настройки программы (Импортировать, Не менять, Очистить):
                            _program_settings.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "_program_settings.txt"), Application.StartupPath);
                    }
                    if (_settings[17] == 1)
                    {
                        /*
                            База сообщений и пользователи соц. сети NILSA (Импортировать, Не менять, Очистить):
                            nilsa_messagesdb.txt
                            nilsa_userdb.txt
                        */
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "nilsa_messagesdb.txt"), Application.StartupPath);
                        NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "nilsa_userdb.txt"), Application.StartupPath);
                    }
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(Application.StartupPath, "_importexport_model_1.settings"), Application.StartupPath);
                }
            }

            return strZipFileName;
        }

        private string NILSA_ARCHIVE_CreateDialogsArchive(string strFilePath, string strFileName, long usrID)
        {
            NILSA_ARCHIVE_GenerateDialogsFile(strFilePath, usrID);
            string strZipFileName = strFileName.Replace(".html", ".zip");
            string strZipFilePath = Path.Combine(Application.StartupPath, strZipFileName);

            if (File.Exists(strZipFilePath))
                File.Delete(strZipFilePath);

            using (ZipArchive newZipArchive = ZipFile.Open(strZipFilePath, ZipArchiveMode.Create))
            {
                newZipArchive.CreateEntryFromFile(strFilePath, strFileName);
            }

            if (File.Exists(strFilePath))
                File.Delete(strFilePath);

            return strZipFileName;
        }

        private void NILSA_ARCHIVE_GenerateDialogsFile(string strFilePath, long usrID)
        {
            Cursor = Cursors.WaitCursor;
            ShowFormWait(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_44", this.Name, "Пожалуйста, подождите, идет формирование отчета..."));
            List<String> lstHTMLFileText = new List<string>();

            lstHTMLFileText.Add("<HTML>");
            lstHTMLFileText.Add("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");

            lstHTMLFileText.Add("<BODY>");
            lstHTMLFileText.Add("<p align=right>" + DateTime.Now.ToShortDateString() + "<br>" + DateTime.Now.ToShortTimeString() + "</p>");
            lstHTMLFileText.Add("<p align=right>" + FormMain.sLicenseUser + "</p>");

            // Персонаж
            lstHTMLFileText.Add("<H1>" + userName + "</H1>");
            lstHTMLFileText.Add("<UL>");

            String[] EQV = new String[iPersHarCount];
            if (File.Exists(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + usrID.ToString() + ".txt")))
            {
                List<String> lstCurContHarValues = new List<String>();
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix() + usrID.ToString() + ".txt"));
                    lstCurContHarValues = new List<String>(srcFile);
                }
                catch (Exception e)
                {
                    ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                    lstCurContHarValues = new List<String>();
                    for (int ji = 0; ji < iContHarCount; ji++)
                        lstCurContHarValues.Add((ji + 1).ToString() + "|");
                }

                foreach (String str in lstCurContHarValues)
                {
                    if (str == null)
                        continue;

                    if (str.Length == 0)
                        continue;

                    EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                }
            }

            for (int i = 0; i < iPersHarCount; i++)
            {
                String sHarName = sPersHar[i, 1];
                String sHarValue = EQV[i];
                lstHTMLFileText.Add("<LI><B>" + sHarName + ":<B> " + sHarValue + "</LI>");
            }
            lstHTMLFileText.Add("</UL>");

            for (int i = 0; i < lstContactsList.Count; i++)
            {
                String value = lstContactsList[i];
                String sUID = value.Substring(0, value.IndexOf("|")); // usrID
                value = value.Substring(value.IndexOf("|") + 1); // skip usrID
                String sUName = value;

                EQV = new String[iContHarCount];
                if (File.Exists(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix() + usrID.ToString() + "_" + sUID + ".txt")))
                {
                    List<String> lstCurContHarValues = new List<String>();
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix() + usrID.ToString() + "_" + sUID + ".txt"));
                        lstCurContHarValues = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        lstCurContHarValues = new List<String>();
                        for (int ji = 0; ji < iContHarCount; ji++)
                            lstCurContHarValues.Add((ji + 1).ToString() + "|");

                    }
                    foreach (String str in lstCurContHarValues)
                    {
                        if (str == null)
                            continue;

                        if (str.Length == 0)
                            continue;

                        EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1);
                    }
                }

                // Контактер
                lstHTMLFileText.Add("<H2>" + sUName + "</H2>");
                lstHTMLFileText.Add("<UL>");
                for (int j = 0; j < iContHarCount; j++)
                {
                    String sColumnName = sContHar[j, 1];
                    String sColumnValue = EQV[j];
                    lstHTMLFileText.Add("<LI><B>" + sColumnName + ":<B> " + sColumnValue + "</LI>");

                }
                lstHTMLFileText.Add("</UL>");
                List<String> lstMsgsLst = ReadAllUserMessagesContacter(Convert.ToInt64(sUID), usrID, true);
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
            FileWriteAllLines(strFilePath, lstHTMLFileText, Encoding.UTF8);
            HideFormWait();
            Cursor = Cursors.Arrow;
        }

        private List<String> ReadAllUserMessagesContacter(long lUserID, long usrID, bool htmlformat)
        {
            List<String> lstUserMessages = new List<String>();

            // получаем id пользователей из группы, макс. кол-во записей = 1000
            int totalCount; // общее кол-во участников
            if (lUserID >= 0)
            {
                if (FormMain.SocialNetwork == 0)
                {
                    List<String> lstHistory = new List<string>();

                    if (File.Exists(Path.Combine(sDataPath, "chat_" + getSocialNetworkPrefix() + usrID.ToString() + "_" + Convert.ToString(lUserID) + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "chat_" + getSocialNetworkPrefix() + usrID.ToString() + "_" + Convert.ToString(lUserID) + ".txt"));
                            lstHistory = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);
                        }
                    }

                    clearTrashList(lstHistory);
                    foreach (String str in lstHistory)
                    {
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

                        lstUserMessages.Add((htmlformat ? "<B>" : "") + (inboundMessage ? (htmlformat ? "&lt;&lt; " : "<< ") : (htmlformat ? "&gt;&gt; " : ">> ")) + (htmlformat ? "</B><U>" : "") + dateStr + " " + timeStr + (htmlformat ? "</U>" : "") + " - " + bodyStr + (htmlformat ? "<BR>" : ""));
                    }

                    /*
                    try
                    {
                        var msgsReceived = api_Messages_GetHistory(lUserID, false, out totalCount, null, 200);
                        foreach (VkNet.Model.Message msg in msgsReceived)
                        {
                            String value = "";
                            value = Convert.ToString(msg.Id.HasValue ? msg.Id.Value : 0) + "|";
                            value = value + msg.Date.Value.ToShortDateString() + "|";
                            value = value + msg.Date.Value.ToShortTimeString() + "|";
                            value = value + NilsaUtils.TextToString(msg.Body);
                            //lstUserMessages.Insert(0, (htmlformat ? "<B>" : "") + ((msg.Type.HasValue && msg.Type.Value == VkNet.Enums.MessageType.Received) ? (htmlformat ? "&lt;&lt; " : "<< ") : (htmlformat ? "&gt;&gt; " : ">> ")) + (htmlformat ? "</B><U>" : "") + msg.Date.Value.ToShortDateString() + " " + msg.Date.Value.ToShortTimeString() + (htmlformat ? "</U>" : "") + " - " + msg.Body + (htmlformat ? "<BR>" : ""));
                            lstUserMessages.Add((htmlformat ? "<B>" : "") + ((msg.Type.HasValue && msg.Type.Value == VkNet.Enums.MessageType.Received) ? (htmlformat ? "&lt;&lt; " : "<< ") : (htmlformat ? "&gt;&gt; " : ">> ")) + (htmlformat ? "</B><U>" : "") + msg.Date.Value.ToShortDateString() + " " + msg.Date.Value.ToShortTimeString() + (htmlformat ? "</U>" : "") + " - " + msg.Body + (htmlformat ? "<BR>" : ""));
                        }
                    }
                    catch (VkNet.Exception.AccessTokenInvalidException atiexp)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (System.Net.WebException)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (VkNet.Exception.VkApiException vkapexeption)
                    {
                        ReAutorize(userLogin, userPassword);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("ReadAllUserMessagesContacter", lUserID.ToString(), e);
                    }
                    finally { }
                    */
                }
                else if (FormMain.SocialNetwork == 1)
                {
                    NILSA_LoadMessagesDB();
                    String sTo0 = usrID.ToString() + "|0|" + lUserID.ToString() + "|";
                    String sTo1 = lUserID.ToString() + "|0|" + usrID.ToString() + "|";
                    String sTo2 = usrID.ToString() + "|1|" + lUserID.ToString() + "|";
                    String sTo3 = lUserID.ToString() + "|1|" + usrID.ToString() + "|";

                    List<String> lstMsgToAdd = new List<string>();
                    for (int i = 0; i < lstNILSA_MessagesDB.Count; i++)
                    {
                        String srec = lstNILSA_MessagesDB[i];
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

        bool bFlashStartButton = false;
        private void timerFlashStartButton_Tick(object sender, EventArgs e)
        {
            bFlashStartButton = !bFlashStartButton;
            tbStartService.Image = bFlashStartButton ? global::Nilsa.Properties.Resources.start_green : global::Nilsa.Properties.Resources.start_black;
        }

        private void tbChangeCommunicationMode_Click(object sender, EventArgs e)
        {
            onManualButtonClick();
            // manual set timers
            StopAnswerTimer();
        }

        protected override void OnResize(EventArgs e)
        {
            if (!bInitStart) this.Visible = false;
            base.OnResize(e);
            if (!bInitStart) this.Visible = true;

            this.Update();
        }

        private void tbContacterWorkModeSend_Click(object sender, EventArgs e)
        {

        }

        private void tbContacterWorkModeResend_Click(object sender, EventArgs e)
        {

        }

        private void tbContacterWorkModeResendWithWait_Click(object sender, EventArgs e)
        {

        }

        private void tbContacterWorkModeIResend_Click(object sender, EventArgs e)
        {

        }

        private void tbContacterWorkModeIResendWithWait_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonPersoneForward_Click(object sender, EventArgs e)
        {
            onChangePersoneByTimer(true, false, true);
        }

        private void toolStripButtonPersoneRewind_Click(object sender, EventArgs e)
        {
            onChangePersoneByTimer(false, false, true);
        }

        private void copyContactsToMasterPersone(string currentPersoneID)
        {
            if (String.IsNullOrEmpty(currentPersoneID))
                return;

            // check VKontakte
            //if (SocialNetwork != 0)
            //    return;

            List<String> lstPersonenListVKontakte = new List<String>();
            if (File.Exists(Path.Combine(sDataPath, "_personen" + getSocialNetworkPrefix(0) + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_personen" + getSocialNetworkPrefix(0) + ".txt"));
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
            if (File.Exists(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix(0) + currentPersoneID + ".txt")))
            {
                try
                {
                    var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix(0) + currentPersoneID + ".txt"));
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

                if (File.Exists(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix(0) + sUID + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "pers_" + getSocialNetworkPrefix(0) + sUID + ".txt"));
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
                if (File.Exists(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix(0) + masterPersoneID + ".txt")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix(0) + masterPersoneID + ".txt"));
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
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "cont_" + getSocialNetworkPrefix(0) + currentPersoneID + "_" + sUID + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "cont_" + getSocialNetworkPrefix(0) + currentPersoneID + "_" + sUID + ".txt"));
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
                    FileWriteAllLines(Path.Combine(sDataPath, "cont_" + getSocialNetworkPrefix(0) + masterPersoneID + "_" + sUID + ".txt"), lstContactData, Encoding.UTF8);

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
                    FileWriteAllLines(Path.Combine(sDataPath, "_contacts_" + getSocialNetworkPrefix(0) + masterPersoneID + ".txt"), lstMasterPersoneContactsList, Encoding.UTF8);
            }

        }

        private void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            string sHelpFile = Path.Combine(Application.StartupPath, "nilsa_help.html");
            if (File.Exists(sHelpFile))
                System.Diagnostics.Process.Start(sHelpFile);
        }

        private void onManualButtonClick()
        {
            setMonitorTime(false);

        }

        private void tbStopService_Click_1(object sender, EventArgs e)
        {
            onManualButtonClick();
            // manual set timers
            StopAnswerTimer();
        }

        private void NILSA_ARCHIVE_AddFilesToArchive(ZipArchive archive, string[] files, string basepath)
        {
            int nbasepath = basepath.Length + 1;
            foreach (string f in files)
            {
                archive.CreateEntryFromFile(f, f.Substring(nbasepath));
            }
        }

        private JObject VkNet_UploadFileToURL(string URL, string file_path)    //загрузка файла на сервер
        {
            WebClient myWebClient = new WebClient();
            byte[] responseArray = myWebClient.UploadFile(URL, file_path);
            var json = JObject.Parse(System.Text.Encoding.ASCII.GetString(responseArray));

            return json;
        }

        private void GenerateFaceContacterMessage(long contid, string message, long _iGroupAnswerID = -1, long _iGroupAnswerPostID = -1, long _iGroupAnswerCommentID = -1)
        {

            String value = "";
            value = Convert.ToString(0) + "|";
            if (_iGroupAnswerID >= 0)
                value = value + Convert.ToString(_iGroupAnswerID) + "/" + Convert.ToString(_iGroupAnswerPostID) + "/" + Convert.ToString(_iGroupAnswerCommentID) + "/" + Convert.ToString(contid) + "|";
            else
                value = value + Convert.ToString(contid) + "|";
            value = value + new DateTime().ToShortDateString() + "|";
            value = value + new DateTime().ToShortTimeString() + "|";
            value = value + NilsaUtils.TextToString(message);

            lstReceivedMessages.Add(value);
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

        private void timerCountersStart_Tick(object sender, EventArgs e)
        {
            cntC1++;
            cntC7++;
            if (SocialNetwork == 0)
                cntC2++;
            else if (SocialNetwork == 2)
                cntC3++;

            if ((cntC1 % 60 == 0 && cntC1 > 0) || (cntC2 % 60 == 0 && cntC2 > 0) || (cntC3 % 60 == 0 && cntC3 > 0) || (cntC7 % 60 == 0 && cntC7 > 0))
            {
                UpdateProgramCountersInfoC1C2C3();
                SaveProgramCountersC1C2C3();
            }

            cntC4++;
            if (SocialNetwork == 0)
                cntC5++;
            else if (SocialNetwork == 2)
                cntC6++;

            cntC8++;
            if (iContUserID >= 0)
                cntC9++;

            if ((cntC4 % 60 == 0 && cntC4 > 0) || (cntC5 % 60 == 0 && cntC5 > 0) || (cntC6 % 60 == 0 && cntC6 > 0))
            {
                UpdateProgramCountersInfoC4C5C6();
                SaveProgramCountersC4C5C6();
            }
            //if ((cntC8 % 60 == 0 && cntC8 > 0) || (cntC9 % 60 == 0 && cntC9 > 0))
            UpdateProgramCountersInfoC8C9();
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

        const int OFFSET_MESSAGES_LIST_TRAFFIC = 25;
        const int SPACE_MESSAGES_LIST_TRAFFIC = 5;
        const int SPACE_MESSAGES_LIST_TRAFFIC1 = 2;
        private void listBoxInMsg_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            int index = e.Index;
            if (index >= 0 && index < listBoxInMsg.Items.Count)
            {
                String value = listBoxInMsg.Items[index].ToString();
                String valperc = value.Substring(0, value.IndexOf("%"));
                value = value.Substring(value.IndexOf("%") + 1).Trim();
                int perc = 0;
                try
                {
                    perc = Convert.ToInt32(valperc);
                }
                catch
                {
                    perc = 0;
                }

                Graphics g = e.Graphics;

                Brush brush = brush_trafficlight_off_color;
                if (perc >= 50)
                    brush = brush_trafficlight_green_color;
                else if (perc > 10 && perc < 50)
                    brush = brush_trafficlight_yellow_color;
                else
                    brush = brush_trafficlight_red_color;

                g.FillRectangle(brush, new Rectangle(e.Bounds.Left, e.Bounds.Top, OFFSET_MESSAGES_LIST_TRAFFIC, e.Bounds.Height));

                TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine;
                TextRenderer.DrawText(e.Graphics, valperc + "%", e.Font, new Rectangle(e.Bounds.Left + SPACE_MESSAGES_LIST_TRAFFIC1, e.Bounds.Top, OFFSET_MESSAGES_LIST_TRAFFIC - SPACE_MESSAGES_LIST_TRAFFIC1, e.Bounds.Height), ForeColor, flags);

                //background:
                SolidBrush backgroundBrush;
                bool bPerson = false;
                if (selected)
                    backgroundBrush = bPerson ? brushPerson1 : brushContact1;
                else
                    backgroundBrush = bPerson ? brushPerson2 : brushContact2;

                g.FillRectangle(backgroundBrush, new Rectangle(e.Bounds.Left + OFFSET_MESSAGES_LIST_TRAFFIC, e.Bounds.Top, e.Bounds.Width - OFFSET_MESSAGES_LIST_TRAFFIC, e.Bounds.Height));

                //flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine;
                TextRenderer.DrawText(e.Graphics, value, e.Font, new Rectangle(e.Bounds.Left + OFFSET_MESSAGES_LIST_TRAFFIC + SPACE_MESSAGES_LIST_TRAFFIC, e.Bounds.Top, e.Bounds.Width - OFFSET_MESSAGES_LIST_TRAFFIC - SPACE_MESSAGES_LIST_TRAFFIC, e.Bounds.Height), ForeColor, flags);

            }

            e.DrawFocusRectangle();
        }

        private void toolStripButtonRandomizeRotatePersonen_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            StopAnswerTimer();
            randomizeRotatePersonen++;
            if (randomizeRotatePersonen > 2)
                randomizeRotatePersonen = 0;
            saveRandomizeRotatePersonen();
            loadRandomizeRotatePersonen();
            StartAnswerTimer();

        }

        private void toolStripButtonReinitDialogsWhenFree_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            StopAnswerTimer();
            removeReinitDialogsWhenFreeFlag();
            reinitDialogsWhenFree = !reinitDialogsWhenFree;
            saveReinitDialogsWhenFree();
            loadReinitDialogsWhenFree();
            StartAnswerTimer();
        }

        private void labelAlgorithmName_Click(object sender, EventArgs e)
        {
            buttonEditAlgorithms_Click(null, null);
        }

        private void labelInEqMsgHarTitleMarker_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxCompareLexicalLevel_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonChangeModeWhenFree_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            StopAnswerTimer();
            removeChangeModeWhenFreeFlag();
            ChangeModeWhenFree++;
            if (ChangeModeWhenFree > 2)
                ChangeModeWhenFree = 0;
            saveChangeModeWhenFree();
            loadChangeModeWhenFree();
            StartAnswerTimer();

        }

        private void timerChangePersone_Tick(object sender, EventArgs e)
        {
            timerChangePersoneCycle--;

            int pbvalue = timerDefaultChangePersoneCycle > 0 ? (int)(100 * (float)(timerDefaultChangePersoneCycle - timerChangePersoneCycle) / (float)(timerDefaultChangePersoneCycle)) : 0;
            if (pbvalue < 0) pbvalue = 0; else if (pbvalue > 100) pbvalue = 100;
            progressBarChangePersone.Value = pbvalue;
            progressBarChangePersone.Invalidate();
            Application.DoEvents();
            if (timerChangePersoneCycle <= 0)
            {
                timerChangePersoneOff();
            }

        }

        private void timerChangePersoneOff()
        {
            timerChangePersone.Enabled = false;
            timerChangePersoneCycle = 0;
            progressBarChangePersone.Value = 0;
            progressBarChangePersone.Invalidate();
            Application.DoEvents();
        }

        private void timerChangePersoneOn()
        {
            if (progressBarChangePersone == null)
                return;
            timerDefaultChangePersoneCycle = timersValues[7];
            timerChangePersoneCycle = timerDefaultChangePersoneCycle;
            progressBarChangePersone.Value = 0;
            progressBarChangePersone.Invalidate();
            Application.DoEvents();

            if (timerChangePersoneCycle > 0 && lstPersoneChange.Count > 0 && SocialNetwork == 0)
                timerChangePersone.Enabled = true;
        }

        private void tbMessagesDBEqInEditMessage_Click(object sender, EventArgs e)
        {
            if (listBoxInMsg.SelectedIndex < 0)
                return;

            // manual set timers
            StopAnswerTimer();
            SetEQInMessageParametersDefaultValues();
            String sMessage = labelInEqMsgHarTitleValue_Text;
            String sMarker = labelInEqMsgHarTitleMarker.Text;
            String sMsgSrcRec = "";
            if (sMessage.Trim().Length > 0)
            {
                sMsgSrcRec = "000000|";
                string _tmp = lstEQInMessagesList[listBoxInMsg.SelectedIndex];
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    _tmp = _tmp.Substring(_tmp.IndexOf("|") + 1);
                    String _s = _tmp.Substring(0, _tmp.IndexOf("|"));
                    sMsgHar[i, 3] = _s;
                    sMsgSrcRec = sMsgSrcRec + _s + "|";
                }
                sMsgSrcRec = sMsgSrcRec + "@!" + NilsaUtils.TextToString(sMessage) + (sMarker.Length > 0 ? ("|!*#0" + sMarker) : "");

            }

            FormEditMsgValues fe = new FormEditMsgValues(this);
            fe.Text += " " + "Сообщения Контактера";
            fe.sPersHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sPersHar[i, j] = sMsgHar[i, j];
                fe.sPersHar[i, iMsgHarAttrCount] = sMsgHar[i, 3];//(i > 0 ? "" : sMsgHar[i, 3]);
            }

            fe.iPersHarAttrCount = iMsgHarAttrCount;
            fe.iPersHarCount = iMsgHarCount;
            fe.textBox1.Text = NilsaUtils.StringToText(NilsaUtils.TextToString(sMessage));
            fe.comboBox2.SelectedIndex = sMarker.Length > 0 ? (Convert.ToInt32(sMarker)) : 0;
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

                if (hashsetEQInMessagesDB.Contains(sMsgSrcRec))
                {
                    lstEQInMessagesDB.Remove(sMsgSrcRec);
                    hashsetEQInMessagesDB.Remove(sMsgSrcRec);
                }

                if (!hashsetEQInMessagesDB.Contains(sMsgNewRec))
                {
                    lstEQInMessagesDB.Add(sMsgNewRec);
                    hashsetEQInMessagesDB.Add(sMsgNewRec);
                    SaveEQInMessageDB();
                    iMsgINMaxID++;
                    NilsaUtils.SaveLongValue(0, iMsgINMaxID);
                }
                UndoMarkerChanges();
                SetEQInMessageList(labelInMsgHarTitleValue_Text);
            }
            StartAnswerTimer();
        }

        private void tbMessagesDBEqInDeleteMessage_Click(object sender, EventArgs e)
        {
            if (listBoxInMsg.SelectedIndex < 0)
                return;

            // manual set timers
            StopAnswerTimer();
            SetEQInMessageParametersDefaultValues();
            String sMessage = labelInEqMsgHarTitleValue_Text;
            String sMarker = labelInEqMsgHarTitleMarker.Text;
            String sMsgSrcRec = "";
            if (sMessage.Trim().Length > 0)
            {
                sMsgSrcRec = "000000|";
                string _tmp = lstEQInMessagesList[listBoxInMsg.SelectedIndex];
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    _tmp = _tmp.Substring(_tmp.IndexOf("|") + 1);
                    String _s = _tmp.Substring(0, _tmp.IndexOf("|"));
                    sMsgHar[i, 3] = _s;
                    sMsgSrcRec = sMsgSrcRec + _s + "|";
                }
                sMsgSrcRec = sMsgSrcRec + "@!" + NilsaUtils.TextToString(sMessage) + (sMarker.Length > 0 ? ("|!*#0" + sMarker) : "");

            }
            if (hashsetEQInMessagesDB.Contains(sMsgSrcRec))
            {
                if (MessageBox.Show("Вы действительно хотите удалить это сообщение из Базы сообщений Контактера?", "Удаление сообщения", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    lstEQInMessagesDB.Remove(sMsgSrcRec);
                    hashsetEQInMessagesDB.Remove(sMsgSrcRec);
                    UndoMarkerChanges();
                    SetEQInMessageList(labelInMsgHarTitleValue_Text);
                }
            }
        }

        private void tbMessagesDBEqOutEditMessage_Click(object sender, EventArgs e)
        {
            if (listBoxOutMsg.SelectedIndex < 0)
                return;

            // manual set timers
            StopAnswerTimer();
            SetEQInMessageParametersDefaultValues();
            String sMessage = labelOutEqMsgHarTitleValueFullText;
            String sMarker = labelOutEqMsgHarTitleMarker.Text;
            String sMsgSrcRec = "";
            if (sMessage.Trim().Length > 0)
            {
                sMsgSrcRec = "000000|";
                string _tmp = lstEQOutMessagesList[listBoxOutMsg.SelectedIndex];
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    _tmp = _tmp.Substring(_tmp.IndexOf("|") + 1);
                    String _s = _tmp.Substring(0, _tmp.IndexOf("|"));
                    sMsgHar[i, 3] = _s;
                    sMsgSrcRec = sMsgSrcRec + _s + "|";
                }
                sMsgSrcRec = sMsgSrcRec + "@!" + NilsaUtils.TextToString(sMessage) + (sMarker.Length > 0 ? ("|!*#0" + sMarker) : "");

            }

            FormEditMsgValues fe = new FormEditMsgValues(this);
            fe.Text += " " + "Сообщения Персонажа";
            fe.sPersHar = new String[iMsgHarCount, iMsgHarAttrCount + 1];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                for (int j = 0; j < iMsgHarAttrCount; j++)
                    fe.sPersHar[i, j] = sMsgHar[i, j];
                fe.sPersHar[i, iMsgHarAttrCount] = sMsgHar[i, 3];//(i > 0 ? "" : sMsgHar[i, 3]);
            }

            fe.iPersHarAttrCount = iMsgHarAttrCount;
            fe.iPersHarCount = iMsgHarCount;
            fe.textBox1.Text = NilsaUtils.StringToText(NilsaUtils.TextToString(sMessage));
            fe.comboBox2.SelectedIndex = sMarker.Length > 0 ? (Convert.ToInt32(sMarker)) : 0;
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

                if (hashsetEQOutMessagesDB.Contains(sMsgSrcRec))
                {
                    lstEQOutMessagesDB.Remove(sMsgSrcRec);
                    hashsetEQOutMessagesDB.Remove(sMsgSrcRec);
                }

                if (!hashsetEQOutMessagesDB.Contains(sMsgNewRec))
                {
                    lstEQOutMessagesDB.Add(sMsgNewRec);
                    hashsetEQOutMessagesDB.Add(sMsgNewRec);
                    SaveEQOutMessageDB();
                    iMsgOUTMaxID++;
                    NilsaUtils.SaveLongValue(1, iMsgOUTMaxID);
                }
                SetEQOutMessageList(sCurrentEQInMessageRecord);
            }
            StartAnswerTimer();

        }

        private void buttonEditAlgorithmVector_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            FormEditAlgorithms fe = new FormEditAlgorithms(this);
            fe.Setup(adbrCurrent.ID, true);
            Cursor = Cursors.WaitCursor;
            if (fe.comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)fe.comboBoxAlgorithmsItems.Items[fe.comboBoxAlgorithmsItems.SelectedIndex];
                FormEditAlgorithmsVectors feam = new FormEditAlgorithmsVectors(this, fe);
                feam.Text = dbr.Name;
                feam.Setup();
                feam.ShowDialog();
                fe.LoadAlgorithmsPairs();
                fe.applyCloseButtonClick();
            }

            LoadAlgorithmsDB();
            setlabelAlgorithmNameText(adbrCurrent.Name);
            comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
            CompareVetors_RestoreDefaultValues();

            if (fe.comboBox1.SelectedIndex == 0)
            {
                iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                // Для всех контактеров
                ResetAlgorithmSettingsAllContacters(adbrCurrent.ID);
            }
            else if (fe.comboBox1.SelectedIndex == 1)
            {
                iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                // Для текущего контактера
                SaveAlgorithmSettingsContacter();
            }
            else if (fe.comboBox1.SelectedIndex == 2)
            {
                iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                // Для всех контактеров с базовым алгоритмом
                ResetAlgorithmSettingsAllContactersList(fe.ERROR_ALG_ID, adbrCurrent.ID);
            }
            /*
            else
            {
                LoadAlgorithmSettingsContacter();

                setlabelAlgorithmNameText(adbrCurrent.Name);
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                CompareVetors_RestoreDefaultValues();
            }
            */
            LoadAlgorithmSettingsContacter();

            setlabelAlgorithmNameText(adbrCurrent.Name);
            comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
            CompareVetors_RestoreDefaultValues();

            if (labelInMsgHarTitleValue_Text.Length > 0)
            {
                UndoMarkerChanges();
                SetEQInMessageList(labelInMsgHarTitleValue_Text);
            }
            UpdatePersoneParametersValues_Algorithm();
            UpdateContactParametersValues_Algorithm();


            Cursor = Cursors.Arrow;
            StartAnswerTimer();

        }

        private void buttonEditAlgorithmMarkers_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            // manual set timers
            StopAnswerTimer();
            FormEditAlgorithms fe = new FormEditAlgorithms(this);
            fe.Setup(adbrCurrent.ID, true);
            Cursor = Cursors.WaitCursor;
            if (fe.comboBoxAlgorithmsItems.SelectedIndex >= 0)
            {
                AlgorithmsDBRecord dbr = (AlgorithmsDBRecord)fe.comboBoxAlgorithmsItems.Items[fe.comboBoxAlgorithmsItems.SelectedIndex];
                FormEditAlgorithmsMarkers feam = new FormEditAlgorithmsMarkers(this, fe);
                feam.Text = dbr.Name;
                feam.Setup();
                feam.ShowDialog();
                fe.applyCloseButtonClick();
            }

            LoadAlgorithmsDB();
            setlabelAlgorithmNameText(adbrCurrent.Name);
            comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
            CompareVetors_RestoreDefaultValues();

            if (fe.comboBox1.SelectedIndex == 0)
            {
                iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                // Для всех контактеров
                ResetAlgorithmSettingsAllContacters(adbrCurrent.ID);
            }
            else if (fe.comboBox1.SelectedIndex == 1)
            {
                iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                // Для текущего контактера
                SaveAlgorithmSettingsContacter();
            }
            else if (fe.comboBox1.SelectedIndex == 2)
            {
                iUndoMarkerChangesAlgorithm = adbrCurrent.ID;
                // Для всех контактеров с базовым алгоритмом
                ResetAlgorithmSettingsAllContactersList(fe.ERROR_ALG_ID, adbrCurrent.ID);
            }
            /*
            else
            {
                LoadAlgorithmSettingsContacter();

                setlabelAlgorithmNameText(adbrCurrent.Name);
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                CompareVetors_RestoreDefaultValues();
            }
            */
            LoadAlgorithmSettingsContacter();

            setlabelAlgorithmNameText(adbrCurrent.Name);
            comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
            CompareVetors_RestoreDefaultValues();

            if (labelInMsgHarTitleValue_Text.Length > 0)
            {
                UndoMarkerChanges();
                SetEQInMessageList(labelInMsgHarTitleValue_Text);
            }
            UpdatePersoneParametersValues_Algorithm();
            UpdateContactParametersValues_Algorithm();


            Cursor = Cursors.Arrow;
            StartAnswerTimer();

        }

        private void buttonToOperatorMode_Click(object sender, EventArgs e)
        {
            if (iContUserID == -1)
                return;

            if (!adbrCurrent.Name.ToLower().Equals("operator"))
            {
                int iMsgAlg = SearchAlgorithmsDBList("operator");
                if (iMsgAlg < 0)
                    iMsgAlg = SearchAlgorithmsDBList("error");
                ChangeContacterAlgorithm(iMsgAlg);
                applyAlgorithm(iMsgAlg);
                LoadAlgorithmSettingsContacter();
                UpdateContactParametersValues_Algorithm();
                comboBoxCompareLexicalLevel.SelectedIndex = (adbrCurrent.ID >= 0 ? adbrCurrent.CompareLexicalLevel : CompareLexicalLevel);
                CompareVetors_RestoreDefaultValues();

                //if (labelInMsgHarTitleValue_Text.Length > 0)
                //{
                //    SelectNextReceivedMessage();
                //}
                deleteAllContacterMessages(true);
                DateTime dt = DateTime.Now;
                String sCurRec = "0|" + getContUserIDWithGroupID() + "|" + dt.ToShortDateString() + "|" + dt.ToShortTimeString() + "|RESEND_OPERATOR";
                lstReceivedMessages.Insert(0, sCurRec);
                SelectNextReceivedMessage(false);
                //tbRefreshPull_Click(null, null);


            }
            setContacterWorkMode();

        }

        private void UpdateModelFromFile(string openFileDialog_FileName, bool Silent = false)
        {
            const int OPTIONS_COUNT = 18;
            int[] Settings = new int[OPTIONS_COUNT + 1];
            bool[] InitSettings = new bool[OPTIONS_COUNT];

            for (int i = 0; i < OPTIONS_COUNT + 1; i++)
                Settings[i] = 0;

            if (File.Exists(openFileDialog_FileName))
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings")))
                    File.Delete(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings"));

                using (ZipArchive archive = ZipFile.OpenRead(openFileDialog_FileName))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.Equals("_importexport_model_1.settings"))
                        {
                            entry.ExtractToFile(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings"));
                        }
                    }
                }

                // Read model content flags
                List<String> lstInitList = new List<String>();
                if (File.Exists(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings")))
                {
                    try
                    {
                        var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings"));
                        lstInitList = new List<String>(srcFile);
                    }
                    catch (Exception e)
                    {
                        ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                        lstInitList = new List<String>();
                    }

                    if (lstInitList.Count == OPTIONS_COUNT + 1)
                    {
                        for (int i = 0; i < OPTIONS_COUNT; i++)
                            InitSettings[i] = Convert.ToInt32(lstInitList[i]) == 1;
                    }

                    for (int i = 0; i < OPTIONS_COUNT; i++)
                        Settings[i] = InitSettings[i] ? (i < 2 ? 2 : 0) : 0;


                }

                // --- End

                if (Settings[0] == 2 || Settings[1] == 2)
                {
                    //if (MessageBox.Show("При импорте информационной модели из архивного файла текущая информация в данной копии NILSA будет заменена в соответствии с выбранными опциями.\n\nПродолжить выполнение импорта?", "Импорт информационной модели", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    //{
                    tbStopService_Click(null, null);

                    if (Settings[18] == 1)
                    {
                        String _backcopy = saveFileDialog.FileName = "nilsa_" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_") + ".nilsa.zip";
                        if (File.Exists(Path.Combine(Application.StartupPath, _backcopy)))
                            File.Delete(Path.Combine(Application.StartupPath, _backcopy));
                        NILSA_ARCHIVE_Create(_backcopy);
                    }
                    ShowFormWait(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_45", this.Name, "Пожалуйста, подождите, операция выполняется..."));

                    if (Settings[0] >= 1)
                    {
                        /*
                            Алгоритмы (Импортировать, Не менять, Очистить):
                            _algorithms_db.txt
                            _algorithms_*.txt
                            _text_search_settings_*.txt
                            _algtimers_settings_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algorithms_db.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algorithms_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_text_search_settings_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algtimers_settings_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "FormEditPersHarValues_2_*.values"));
                    }
                    if (Settings[1] >= 1)
                    {
                        /*
                            Базы сообщений Персонажа и Контактера (Импортировать, Не менять, Очистить):
                            _messages_db.txt
                            _eqinmsgdb_*.txt
                            _eqoutmsgdb_*.txt
                            LongValues0.values
                            LongValues1.values
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_messages_db.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_eqinmsgdb_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_eqoutmsgdb_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues0.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues1.values"));
                    }
                    if (Settings[2] >= 1)
                    {
                        /*
                            Настройки инициации диалогов (Импортировать, Не менять, Очистить):
                            StringValues2.values
                            LongValues2.values
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "StringValues2.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues2.values"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues4.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues117.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues3.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues9.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "StringValues3.values"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues_3*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesGroups_3*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEnterContactersToImportCount_3*.values"));
                    }
                    if (Settings[3] >= 1)
                    {
                        /*
                            База Персонажей (Импортировать, Не менять, Очистить):
                            _personen*.txt
                            pers_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_personen*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "pers_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_dialogsinitperday_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "_program_state*.txt"));
                    }
                    if (Settings[4] >= 1)
                    {
                        /*
                            Пулы входящих и исходящих сообщений Персонажей (Импортировать, Не менять, Очистить):
                            _msg_received_pull_*.txt
                            _msg_outgoing_pull_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msg_received_pull_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msg_outgoing_pull_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msg_outdelayed_pull_*.txt"));
                    }
                    if (Settings[5] >= 1)
                    {
                        /*
                            Разрешенные алгоритмы Персонажей (Импортировать, Не менять, Очистить):
                            _enabledalgorithms_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_enabledalgorithms_*.txt"));
                    }
                    if (Settings[6] >= 1)
                    {
                        /*
                            Инициация диалогов Персонажей (Импортировать, Не менять, Очистить):
                            _initdialog_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_initdialog_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "resend_operator_ *.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_wall_posts_*.txt"));
                    }
                    if (Settings[7] >= 1)
                    {
                        /*
                            Базы Контактеров Персонажей (Импортировать, Не менять, Очистить):
                            _contacts_*.txt
                            cont_*_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_contacts_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "cont_*_*.txt"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_groups_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "grp_*_*.txt"));
                    }
                    if (Settings[8] >= 1)
                    {
                        /*
                            Привязки алгоритмов к Контактерам и Персонажам (Импортировать, Не менять, Очистить):
                            _algotithm_settings_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algotithm_settings_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algotithm_settings_*_*.txt"));
                    }
                    if (Settings[9] >= 1)
                    {
                        /*
                            Флаги инициации диалога с Контактерами (Импортировать, Не менять, Очистить):
                            _flag_init_dialog_*_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_flag_init_dialog_*_*.txt"));
                    }
                    if (Settings[10] >= 1)
                    {
                        /*
                            Последние отосланные Контактерам сообщения (Импортировать, Не менять, Очистить):
                            _lastmessage_*_*.txt
                            _prevlastmessage_*_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_lastmessage_*_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_prevlastmessage_*_*.txt"));
                    }
                    if (Settings[11] >= 1)
                    {
                        /*
                            Настройки характеристик Контактера (Импортировать, Не менять, Очистить):
                            _conthar.txt
                            _cont_har_*.txt
                            FormEditContactsDB.columns
                            FormEditPersHarValues_*_*.values
                            FormEditPersHarValuesStatusUpdate.values
                            FormEditPersHarValuesAlgorithmUpdate.values
                            StringValues0.values
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_conthar.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_cont_har_*.txt"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_grouphar.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_grp_har_*.txt"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditContactsDB.columns"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues_0_*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues_1_*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesStatusUpdate.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "StringValues0.values"));
                    }
                    if (Settings[12] >= 1)
                    {
                        /*
                            Настройки характеристик Персонажа (Импортировать, Не менять, Очистить):
                            _pershar.txt
                            _pers_har_*.txt
                            FormEditPersonenDB.columns
                            FormEditPersHarValuesPersonen_*_*.values
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_pershar.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_pers_har_*.txt"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersonenDB.columns"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesPersonen_*_*.values"));
                    }
                    if (Settings[13] >= 1)
                    {
                        /*
                            Настройки характеристик сообщений (Импортировать, Не менять, Очистить):
                            _msghar.txt
                            _msg_har_*.txt
                            _msg_har_*_*.txt
                            FormEditEQInMessagesDB.columns
                            FormEditEQOutMessagesDB.columns
                            FormEditPersHarValuesEQIn_*_*.values
                            FormEditPersHarValuesEQOut_*_*.values
                            _default_db_name.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msghar.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msg_har_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditEQInMessagesDB.columns"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesEQIn_*_*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditEQOutMessagesDB.columns"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesEQOut_*_*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_default_db_name.txt"));
                    }
                    if (Settings[14] >= 1)
                    {
                        /*
                            Счетчики программы (Импортировать, Не менять, Очистить):
                            _program_counters_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "_program_counters_*.txt"));
                    }
                    if (Settings[15] >= 1)
                    {
                        /*
                            Настройки цветов (Импортировать, Не менять, Очистить):
                            _colors_settings.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "_colors_settings.txt"));
                    }
                    if (Settings[16] >= 1)
                    {
                        /*
                            Настройки программы (Импортировать, Не менять, Очистить):
                            _program_settings.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "_program_settings.txt"));
                    }
                    if (Settings[17] >= 1)
                    {
                        /*
                            База сообщений и пользователи соц. сети NILSA (Импортировать, Не менять, Очистить):
                            nilsa_messagesdb.txt
                            nilsa_userdb.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "nilsa_messagesdb.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "nilsa_userdb.txt"));
                    }

                    using (ZipArchive archive = ZipFile.OpenRead(openFileDialog_FileName))
                    {
                        if (Settings[0] == 2)
                        {
                            /*
                                Алгоритмы (Импортировать, Не менять, Очистить):
                                _algorithms_db.txt
                                _algorithms_*.txt
                                _text_search_settings_*.txt
                                _algtimers_settings_*.txt
                            */
                            //NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algorithms_db.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algorithms_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_text_search_settings_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algtimers_settings_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValues_2_*.values");
                        }
                        if (Settings[1] == 2)
                        {
                            /*
                                Базы сообщений Персонажа и Контактера (Импортировать, Не менять, Очистить):
                                _messages_db.txt
                                _eqinmsgdb_*.txt
                                _eqoutmsgdb_*.txt
                                LongValues0.values
                                LongValues1.values
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_messages_db.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_eqinmsgdb_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_eqoutmsgdb_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues0.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues1.values");
                        }
                        if (Settings[2] == 2)
                        {
                            /*
                                Настройки инициации диалогов (Импортировать, Не менять, Очистить):
                                StringValues2.values
                                LongValues2.values
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "StringValues2.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "StringValues3.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues2.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues3.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues4.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues117.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues9.values");

                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValues_3*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesGroups_3*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEnterContactersToImportCount_3*.txt");

                        }
                        if (Settings[3] == 2)
                        {
                            /*
                                База Персонажей (Импортировать, Не менять, Очистить):
                                _personen*.txt
                                pers_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_personen*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "pers_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_dialogsinitperday_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_program_state*.txt");
                        }
                        if (Settings[4] == 2)
                        {
                            /*
                                Пулы входящих и исходящих сообщений Персонажей (Импортировать, Не менять, Очистить):
                                _msg_received_pull_*.txt
                                _msg_outgoing_pull_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msg_received_pull_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msg_outgoing_pull_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msg_outdelayed_pull_*.txt");
                        }
                        if (Settings[5] == 2)
                        {
                            /*
                                Разрешенные алгоритмы Персонажей (Импортировать, Не менять, Очистить):
                                _enabledalgorithms_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_enabledalgorithms_*.txt");
                        }
                        if (Settings[6] == 2)
                        {
                            /*
                                Инициация диалогов Персонажей (Импортировать, Не менять, Очистить):
                                _initdialog_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_initdialog_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "resend_operator_ *.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_wall_posts_*.txt");
                        }
                        if (Settings[7] == 2)
                        {
                            /*
                                Базы Контактеров Персонажей (Импортировать, Не менять, Очистить):
                                _contacts_*.txt
                                cont_*_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_contacts_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "cont_*_*.txt");

                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_groups_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "grp_*_*.txt");
                        }
                        if (Settings[8] == 2)
                        {
                            /*
                                Привязки алгоритмов к Контактерам и Персонажам (Импортировать, Не менять, Очистить):
                                _algotithm_settings_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algotithm_settings_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algotithm_settings_*_*.txt");
                        }
                        if (Settings[9] == 2)
                        {
                            /*
                                Флаги инициации диалога с Контактерами (Импортировать, Не менять, Очистить):
                                _flag_init_dialog_*_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_flag_init_dialog_*_*.txt");
                        }
                        if (Settings[10] == 2)
                        {
                            /*
                                Последние отосланные Контактерам сообщения (Импортировать, Не менять, Очистить):
                                _lastmessage_*_*.txt
                                _prevlastmessage_*_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_lastmessage_*_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_prevlastmessage_*_*.txt");
                        }
                        if (Settings[11] == 2)
                        {
                            /*
                                Настройки характеристик Контактера (Импортировать, Не менять, Очистить):
                                _conthar.txt
                                _cont_har_*.txt
                                FormEditContactsDB.columns
                                FormEditPersHarValues_*_*.values
                                FormEditPersHarValuesStatusUpdate.values
                                FormEditPersHarValuesAlgorithmUpdate.values
                                StringValues0.values
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_conthar.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_cont_har_*.txt");

                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_grouphar.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_grp_har_*.txt");

                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditContactsDB.columns");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValues_0_*.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValues_1_*.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesStatusUpdate.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesAlgorithmUpdate.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "StringValues0.values");
                        }
                        if (Settings[12] == 2)
                        {
                            /*
                                Настройки характеристик Персонажа (Импортировать, Не менять, Очистить):
                                _pershar.txt
                                _pers_har_*.txt
                                FormEditPersonenDB.columns
                                FormEditPersHarValuesPersonen_*_*.values
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_pershar.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_pers_har_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersonenDB.columns");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesPersonen_*_*.values");
                        }
                        if (Settings[13] == 2)
                        {
                            /*
                                Настройки характеристик сообщений (Импортировать, Не менять, Очистить):
                                _msghar.txt
                                _msg_har_*.txt
                                _msg_har_*_*.txt
                                FormEditEQInMessagesDB.columns
                                FormEditEQOutMessagesDB.columns
                                FormEditPersHarValuesEQIn_*_*.values
                                FormEditPersHarValuesEQOut_*_*.values
                                _default_db_name.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msghar.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msg_har_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditEQInMessagesDB.columns");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesEQIn_*_*.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditEQOutMessagesDB.columns");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesEQOut_*_*.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_default_db_name.txt");
                        }
                        if (Settings[14] == 2)
                        {
                            /*
                                Счетчики программы (Импортировать, Не менять, Очистить):
                                _program_counters_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_program_counters_*.txt");
                        }
                        if (Settings[15] == 2)
                        {
                            /*
                                Настройки цветов (Импортировать, Не менять, Очистить):
                                _colors_settings.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_colors_settings.txt");
                        }
                        if (Settings[16] == 2)
                        {
                            /*
                                Настройки программы (Импортировать, Не менять, Очистить):
                                _program_settings.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_program_settings.txt");
                        }
                        if (Settings[17] == 2)
                        {
                            /*
                                База сообщений и пользователи соц. сети NILSA (Импортировать, Не менять, Очистить):
                                nilsa_messagesdb.txt
                                nilsa_userdb.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "nilsa_messagesdb.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "nilsa_userdb.txt");
                        }
                    }

                    iMsgINMaxID = NilsaUtils.LoadLongValue(0, 0);
                    iMsgOUTMaxID = NilsaUtils.LoadLongValue(1, 0);

                    LoadAlgorithmsDB();
                    List<String> enabledAlgList = new List<string>();
                    foreach (AlgorithmsDBRecord dbr in lstAlgorithmsRecordsList)
                        enabledAlgList.Add(dbr.ID.ToString());

                    List<String> lstPersonenList0 = new List<String>();
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix(0) + ".txt")))
                    {
                        try
                        {
                            var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_personen" + FormMain.getSocialNetworkPrefix(0) + ".txt"));
                            lstPersonenList0 = new List<String>(srcFile);
                        }
                        catch (Exception e)
                        {
                            ExceptionToLogList("File.ReadAllLines", "Reading lists", e);

                            lstPersonenList0 = new List<String>();
                        }
                    }

                    foreach (String str in lstPersonenList0)
                    {
                        if (str == null)
                            continue;

                        if (str.Length == 0)
                            continue;

                        String sUID = str.Substring(0, str.IndexOf("|"));
                        FileWriteAllLines(Path.Combine(FormMain.sDataPath, "_enabledalgorithms_" + FormMain.getSocialNetworkPrefix(0) + sUID + ".txt"), enabledAlgList, Encoding.UTF8);
                    }

                    HideFormWait();

                    if (!Silent)
                    {
                        MessageBox.Show("Обновление информационной модели завершено. Сейчас Персонаж будет реактивирован для загрузки обновленных параметров...", "Обновление информационной модели");

                        tbStopService_Click(null, null);
                        SaveReceivedMessagesPull();
                        SaveOutgoingMessagesPull();
                        onAfterPersonenListChanged();
                        Setup(userLogin, userPassword, userID);
                    }

                }
            }

        }

        private void tsmiUpdateModel_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "ZIP-files (*.nilsa.zip)|*.nilsa.zip|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                UpdateModelFromFile(openFileDialog.FileName);
            }
        }

        private void autoUpdateModelFTP()
        {
            string filename = NilsaUtils.LoadStringValue(1000, "");

            if (iLicenseType == LICENSE_TYPE_WORK)
                filename = "NILSA_3.0.nilsa.zip";

            if (filename.Length > 0)
            {
                Cursor = Cursors.WaitCursor;

                if (File.Exists(Path.Combine(Application.StartupPath, filename)))
                    File.Delete(Path.Combine(Application.StartupPath, filename));

                bool bErrors = false;
                try
                {
                    // Get the object used to communicate with the server.
                    Uri serverUri = new Uri(FTP_SERVER_URI + filename);
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUri);
                    request.Credentials = new System.Net.NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                    request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    DateTime lastModifiedDate = response.LastModified;
                    String lastModifiedDateS = NilsaUtils.LoadStringValue(1001, "");

                    if (!lastModifiedDateS.Equals(lastModifiedDate.ToString()))
                    {
                        NilsaUtils.SaveStringValue(1001, lastModifiedDate.ToString());

                        using (WebClient ftpClient = new WebClient())
                        {
                            ftpClient.Credentials = new System.Net.NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                            ftpClient.DownloadFile(FTP_SERVER_URI + filename, Path.Combine(Application.StartupPath, filename));
                        }
                    }

                }
                catch
                {
                    bErrors = true;
                }

                Cursor = Cursors.Arrow;

                if (!bErrors)
                    UpdateModelFromFile(Path.Combine(Application.StartupPath, filename), true);
            }
        }

        private void tsmiUpdateModelFTP_Click(object sender, EventArgs e)
        {
            string filename = getFTPModelFileName(true);

            if (filename.Length > 0)
            {
                Cursor = Cursors.WaitCursor;

                if (File.Exists(Path.Combine(Application.StartupPath, filename)))
                    File.Delete(Path.Combine(Application.StartupPath, filename));

                bool bErrors = false;
                try
                {
                    NilsaUtils.SaveStringValue(1000, filename);

                    // Get the object used to communicate with the server.
                    Uri serverUri = new Uri(FTP_SERVER_URI + filename);
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUri);
                    request.Credentials = new System.Net.NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                    request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    DateTime lastModifiedDate = response.LastModified;
                    NilsaUtils.SaveStringValue(1001, lastModifiedDate.ToString());

                    using (WebClient ftpClient = new WebClient())
                    {
                        ftpClient.Credentials = new System.Net.NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                        ftpClient.DownloadFile(FTP_SERVER_URI + filename, Path.Combine(Application.StartupPath, filename));
                    }

                }
                catch
                {
                    bErrors = true;
                }

                Cursor = Cursors.Arrow;

                if (bErrors)
                    MessageBox.Show("Во время загрузки информационной модели с Сервера произошла ошибка. Пожалуйста, проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", "Обновление информационной модели с Сервера", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    UpdateModelFromFile(Path.Combine(Application.StartupPath, filename));
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            if (SocialNetwork == 1)
            {
                MessageBox.Show("Мастер инициации диалога с Контактерами из тематических групп доступен только для социальной сети ВКонтакте!");
                return;
            }

            StopAnswerTimer();
            LoadGroupParametersDescription();
            FormInitDialogsWithContactersFromGroups fid = new FormInitDialogsWithContactersFromGroups(this);
            fid.sGroupHar = new String[iGroupHarCount, iGroupHarAttrCount + 1];
            for (int i = 0; i < iGroupHarCount; i++)
            {
                for (int j = 0; j < iGroupHarAttrCount; j++)
                    fid.sGroupHar[i, j] = sGroupHar[i, j];
                fid.sGroupHar[i, iGroupHarAttrCount] = "";
            }
            fid.iGroupHarCount = iGroupHarCount;
            fid.iGroupHarAttrCount = iGroupHarAttrCount;

            fid.Setup(iPersUserID);
            if (fid.ShowDialog() == DialogResult.OK)
            {
                if (randomizeRotatePersonen != 2)
                {
                    randomizeRotatePersonen = 2;
                    saveRandomizeRotatePersonen();
                    loadRandomizeRotatePersonen();
                }

                if (!reinitDialogsWhenFree)
                {
                    removeReinitDialogsWhenFreeFlag();
                    reinitDialogsWhenFree = !reinitDialogsWhenFree;
                    saveReinitDialogsWhenFree();
                    loadReinitDialogsWhenFree();
                }

                if (ChangeModeWhenFree == 1)
                {
                    removeChangeModeWhenFreeFlag();
                    ChangeModeWhenFree++;
                    if (ChangeModeWhenFree > 2)
                        ChangeModeWhenFree = 0;
                    saveChangeModeWhenFree();
                    loadChangeModeWhenFree();
                }

                tbStartService_Click(null, null);
            }
        }

        private void tbPersoneCopyMessage_Click(object sender, EventArgs e)
        {
            if (listBoxOutMsg.SelectedIndex < 0)
                return;

            // manual set timers
            StopAnswerTimer();
            String sMsgSrcRec = "000000|";
            string _tmp = lstEQOutMessagesList[listBoxOutMsg.SelectedIndex];
            for (int i = 0; i < iMsgHarCount; i++)
            {
                _tmp = _tmp.Substring(_tmp.IndexOf("|") + 1);
                String _s = _tmp.Substring(0, _tmp.IndexOf("|"));
                if (i == MSG_ID_COLUMN)
                    sMsgSrcRec = sMsgSrcRec + (iMsgOUTMaxID + 1).ToString() + "|";
                else
                    sMsgSrcRec = sMsgSrcRec + _s + "|";
            }
            sMsgSrcRec = sMsgSrcRec + _tmp.Substring(_tmp.IndexOf("|") + 1);

            if (hashsetEQOutMessagesDB.Contains(sMsgSrcRec))
            {
                lstEQOutMessagesDB.Remove(sMsgSrcRec);
                hashsetEQOutMessagesDB.Remove(sMsgSrcRec);
            }

            if (!hashsetEQOutMessagesDB.Contains(sMsgSrcRec))
            {
                lstEQOutMessagesDB.Add(sMsgSrcRec);
                hashsetEQOutMessagesDB.Add(sMsgSrcRec);
                SaveEQOutMessageDB();
                iMsgOUTMaxID++;
                NilsaUtils.SaveLongValue(1, iMsgOUTMaxID);
            }
            SetEQOutMessageList(sCurrentEQInMessageRecord);

            StartAnswerTimer();
        }

        private void resetPersonenActivationCounters_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите сбросить счетчики активаций всех Персонажей?", NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_25", this.Name, "Сброс счетчиков программы"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                tbStopService_Click(null, null);

                deleteAllPersoneActivationCounter();
                MessageBox.Show("Сброс счетчиков активаций всех Персонажей завершен.", NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_25", this.Name, "Сброс счетчиков программы"));
            }

        }

        private bool addInMsgRecordToDB(String sMsgNewRec, bool bNeedSave)
        {
            if (!hashsetEQInMessagesDB.Contains(sMsgNewRec))
            {
                lstEQInMessagesDB.Add(sMsgNewRec);
                hashsetEQInMessagesDB.Add(sMsgNewRec);
                bNeedSave = true;
                iMsgINMaxID++;
            }
            return bNeedSave;
        }

        private bool addOutMsgRecordToDB(String sMsgNewRec, bool bNeedSave)
        {
            if (!hashsetEQOutMessagesDB.Contains(sMsgNewRec))
            {
                lstEQOutMessagesDB.Add(sMsgNewRec);
                hashsetEQOutMessagesDB.Add(sMsgNewRec);
                bNeedSave = true;
                iMsgOUTMaxID++;
            }

            return bNeedSave;
        }

        private void toolStripButtonCreateDialogs_Click(object sender, EventArgs e)
        {
            onManualButtonClick();

            StopAnswerTimer();

            FormMasterCreateDialogs fid = new FormMasterCreateDialogs(this);
            fid.Setup();
            if (fid.ShowDialog() == DialogResult.OK)
            {
                // добавление сообщений Контактёра
                bool bNeedSave = false;
                String projectName = NilsaUtils.TextToString(fid.textBoxProjectName.Text);

                bNeedSave = addInMsgRecordToDB("000000|PROK|ВОПРОС_№1|*|*|QUESTION|QUESTION|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||YES|" + (iMsgINMaxID + 1).ToString() + "|@!" + (fid.textBox15.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox15.Text.Trim()) : "ТИПИЧНЫЙ ВОПРОС №1"), bNeedSave);
                bNeedSave = addInMsgRecordToDB("000000|PROK|ВОПРОС_№2|*|*|QUESTION|QUESTION|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||YES|" + (iMsgINMaxID + 1).ToString() + "|@!" + (fid.textBox17.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox17.Text.Trim()) : "ТИПИЧНЫЙ ВОПРОС №2"), bNeedSave);
                bNeedSave = addInMsgRecordToDB("000000|PROK|ВОПРОС_№3|*|*|QUESTION|QUESTION|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||YES|" + (iMsgINMaxID + 1).ToString() + "|@!" + (fid.textBox19.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox19.Text.Trim()) : "ТИПИЧНЫЙ ВОПРОС №3"), bNeedSave);
                bNeedSave = addInMsgRecordToDB("000000|PROK|СОГЛАСИЕ|*|*|YES|YES|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||YES|" + (iMsgINMaxID + 1).ToString() + "|@!" + (fid.textBox12.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox12.Text.Trim()) : "ТИПИЧНОЕ СОГЛАСИЕ"), bNeedSave);
                bNeedSave = addInMsgRecordToDB("000000|PROK|ОТКАЗ|*|*|NO|NO|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||YES|" + (iMsgINMaxID + 1).ToString() + "|@!" + (fid.textBox13.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox13.Text.Trim()) : "ТИПИЧНЫЙ ОТКАЗ"), bNeedSave);

                if (bNeedSave)
                {
                    SaveEQInMessageDB();
                    NilsaUtils.SaveLongValue(0, iMsgINMaxID);
                }

                // добавление сообщений Персонажа
                bNeedSave = false;

                if (fid.checkBox1.Checked)
                    bNeedSave = addOutMsgRecordToDB("000000|START|ПРИВЕТСТВИЕ|НЕЗНАКОМЕЦ|*|START|START|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ|||||" + (iMsgOUTMaxID + 1).ToString() + "|@!" + (fid.textBox1.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox1.Text.Trim()) : "ТИПИЧНОЕ ПРИВЕТСТВИЕ") + "|!*#01", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|КОНТРОЛЬНЫЙ ВОПРОС|1|1|START|START|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||" + (fid.checkBox2.Checked ? "" : "YES") + "|" + (iMsgOUTMaxID + 1).ToString() + "|@!" + ((fid.checkBox2.Checked && (fid.textBox2.Text.Trim().Length > 0)) ? NilsaUtils.TextToString(fid.textBox2.Text.Trim()) : "САМОПРЕЗЕНТАЦИЯ") + "|!*#04", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ОЧЕРЕДНОЕ_СООБЩЕНИЕ_КОНТАКТЕРУ|1|2|COMMAND|COMMAND|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||" + (fid.checkBox3.Checked ? "" : "YES") + "|" + (iMsgOUTMaxID + 1).ToString() + "|@!" + ((fid.checkBox3.Checked && (fid.textBox3.Text.Trim().Length > 0)) ? NilsaUtils.TextToString(fid.textBox3.Text.Trim()) : "ПРИЧИНА ОБРАЩЕНИЯ") + "|!*#04", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ОЧЕРЕДНОЕ_СООБЩЕНИЕ_КОНТАКТЕРУ|1|3|COMMAND|COMMAND|*|ПРОДАВЕЦ|*|" + projectName + "|*|||||" + (iMsgOUTMaxID + 1).ToString() + "|@!" + (fid.textBox4.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox4.Text.Trim()) : "КОНТРОЛЬНЫЙ ВОПРОС") + "|!*#05", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|*|2|4|*|*|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ|||||" + (iMsgOUTMaxID + 1).ToString() + "|@!" + (fid.textBox5.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox5.Text.Trim()) : "ПРЕДЛОЖЕНИЕ") + "|!*#04", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ОЧЕРЕДНОЕ_СООБЩЕНИЕ_КОНТАКТЕРУ|2|5|COMMAND|COMMAND|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||" + (fid.checkBox4.Checked ? "" : "YES") + "|" + (iMsgOUTMaxID + 1).ToString() + "|@!" + ((fid.checkBox4.Checked && (fid.textBox6.Text.Trim().Length > 0)) ? NilsaUtils.TextToString(fid.textBox6.Text.Trim()) : "ДЕТАЛИЗАЦИЯ ПРЕДЛОЖЕНИЯ") + "|!*#04", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ОЧЕРЕДНОЕ_СООБЩЕНИЕ_КОНТАКТЕРУ|2|6|COMMAND|COMMAND|*|ПРОДАВЕЦ|*|" + projectName + "|*|||||" + (iMsgOUTMaxID + 1).ToString() + "|@!" + (fid.textBox7.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox7.Text.Trim()) : "ВОПРОС") + "|!*#06", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|*|3|7|*|*|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||" + (fid.checkBox5.Checked ? "" : "YES") + "|" + (iMsgOUTMaxID + 1).ToString() + "|@!" + ((fid.checkBox5.Checked && (fid.textBox8.Text.Trim().Length > 0)) ? NilsaUtils.TextToString(fid.textBox8.Text.Trim()) : "УСЛОВИЯ") + "|!*#04", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ОЧЕРЕДНОЕ_СООБЩЕНИЕ_КОНТАКТЕРУ|3|8|COMMAND|COMMAND|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||" + (fid.checkBox6.Checked ? "" : "YES") + "|" + (iMsgOUTMaxID + 1).ToString() + "|@!" + ((fid.checkBox6.Checked && (fid.textBox9.Text.Trim().Length > 0)) ? NilsaUtils.TextToString(fid.textBox9.Text.Trim()) : "БОНУСЫ") + "|!*#04", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ОЧЕРЕДНОЕ_СООБЩЕНИЕ_КОНТАКТЕРУ|3|9|COMMAND|COMMAND|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ|||||" + (iMsgOUTMaxID + 1).ToString() + "|@!" + (fid.textBox10.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox10.Text.Trim()) : "ПРИЗЫВ К ДЕЙСТВИЮ") + "|!*#07", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|*|4|10|*|*|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||" + (fid.checkBox7.Checked ? "" : "YES") + "|" + (iMsgOUTMaxID + 1).ToString() + "|@!" + ((fid.checkBox7.Checked && (fid.textBox11.Text.Trim().Length > 0)) ? NilsaUtils.TextToString(fid.textBox11.Text.Trim()) : "ЗАВЕРШЕНИЕ") + "|!*#08", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ОТКАЗ|3~4|*|NO|NO|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ||||YES|" + (iMsgOUTMaxID + 1).ToString() + "|@!" + (fid.textBox14.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox14.Text.Trim()) : "РЕАКЦИЯ НА ОТКАЗ") + "|!*#08", bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ВОПРОС_№1|*|*|ANSWER|ANSWER|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ|||||" + (iMsgOUTMaxID + 1).ToString() + "|@!" + (fid.textBox16.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox16.Text.Trim()) : "ТИПИЧНЫЙ ОТВЕТ №1"), bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ВОПРОС_№2|*|*|ANSWER|ANSWER|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ|||||" + (iMsgOUTMaxID + 1).ToString() + "|@!" + (fid.textBox18.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox18.Text.Trim()) : "ТИПИЧНЫЙ ОТВЕТ №2"), bNeedSave);
                bNeedSave = addOutMsgRecordToDB("000000|PROK|ВОПРОС_№3|*|*|ANSWER|ANSWER|*|ПРОДАВЕЦ|*|" + projectName + "|ПОКУПАТЕЛЬ|||||" + (iMsgOUTMaxID + 1).ToString() + "|@!" + (fid.textBox20.Text.Trim().Length > 0 ? NilsaUtils.TextToString(fid.textBox20.Text.Trim()) : "ТИПИЧНЫЙ ОТВЕТ №3"), bNeedSave);

                if (bNeedSave)
                {
                    SaveEQOutMessageDB();
                    NilsaUtils.SaveLongValue(1, iMsgOUTMaxID);
                }

                MessageBox.Show("Создание диалогов для проекта '" + projectName + "' успено завершено.", "Мастер создания диалогов");
            }
        }

        private void attachRuCaptchaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEnterCapthaParameters fecp = new FormEnterCapthaParameters();
            fecp.ShowDialog();
        }

        private void WhatsNewMenuItem_Click_1(object sender, EventArgs e)
        {
            string sHelpFile = Path.Combine(Application.StartupPath, "WhatsNew.txt");
            if (File.Exists(sHelpFile))
                System.Diagnostics.Process.Start(sHelpFile);
        }

        private void contextMenuStripEQOutMsgValues_Opening(object sender, CancelEventArgs e)
        {

        }

        private void tbMessagesDBEqOutDeleteMessage_Click(object sender, EventArgs e)
        {
            if (listBoxOutMsg.SelectedIndex < 0)
                return;

            // manual set timers
            StopAnswerTimer();
            SetEQInMessageParametersDefaultValues();
            String sMessage = labelOutEqMsgHarTitleValueFullText;
            String sMarker = labelOutEqMsgHarTitleMarker.Text;
            String sMsgSrcRec = "";
            if (sMessage.Trim().Length > 0)
            {
                sMsgSrcRec = "000000|";
                string _tmp = lstEQOutMessagesList[listBoxOutMsg.SelectedIndex];
                for (int i = 0; i < iMsgHarCount; i++)
                {
                    _tmp = _tmp.Substring(_tmp.IndexOf("|") + 1);
                    String _s = _tmp.Substring(0, _tmp.IndexOf("|"));
                    sMsgHar[i, 3] = _s;
                    sMsgSrcRec = sMsgSrcRec + _s + "|";
                }
                sMsgSrcRec = sMsgSrcRec + "@!" + NilsaUtils.TextToString(sMessage) + (sMarker.Length > 0 ? ("|!*#0" + sMarker) : "");
            }

            if (hashsetEQOutMessagesDB.Contains(sMsgSrcRec))
            {
                if (MessageBox.Show("Вы действительно хотите удалить это сообщение из Базы сообщений Персонажа?", "Удаление сообщения", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    lstEQOutMessagesDB.Remove(sMsgSrcRec);
                    hashsetEQOutMessagesDB.Remove(sMsgSrcRec);
                    SetEQOutMessageList(sCurrentEQInMessageRecord);
                }
            }
        }

        private void listBoxOutMsg_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            int index = e.Index;
            if (index >= 0 && index < listBoxOutMsg.Items.Count)
            {
                String value = listBoxOutMsg.Items[index].ToString();
                String valperc = value.Substring(0, value.IndexOf("%"));
                value = value.Substring(value.IndexOf("%") + 1).Trim();
                int perc = 0;
                try
                {
                    perc = Convert.ToInt32(valperc);
                }
                catch
                {
                    perc = 0;
                }

                Graphics g = e.Graphics;

                Brush brush = brush_trafficlight_off_color;
                if (perc >= 50)
                    brush = brush_trafficlight_green_color;
                else if (perc > 10 && perc < 50)
                    brush = brush_trafficlight_yellow_color;
                else
                    brush = brush_trafficlight_red_color;

                g.FillRectangle(brush, new Rectangle(e.Bounds.Left, e.Bounds.Top, OFFSET_MESSAGES_LIST_TRAFFIC, e.Bounds.Height));

                TextFormatFlags flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine;
                TextRenderer.DrawText(e.Graphics, valperc + "%", e.Font, new Rectangle(e.Bounds.Left + SPACE_MESSAGES_LIST_TRAFFIC1, e.Bounds.Top, OFFSET_MESSAGES_LIST_TRAFFIC - SPACE_MESSAGES_LIST_TRAFFIC1, e.Bounds.Height), ForeColor, flags);

                //background:
                SolidBrush backgroundBrush;
                bool bPerson = true;
                if (selected)
                    backgroundBrush = bPerson ? brushPerson1 : brushContact1;
                else
                    backgroundBrush = bPerson ? brushPerson2 : brushContact2;

                g.FillRectangle(backgroundBrush, new Rectangle(e.Bounds.Left + OFFSET_MESSAGES_LIST_TRAFFIC, e.Bounds.Top, e.Bounds.Width - OFFSET_MESSAGES_LIST_TRAFFIC, e.Bounds.Height));

                //flags = TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl | TextFormatFlags.SingleLine;
                TextRenderer.DrawText(e.Graphics, value, e.Font, new Rectangle(e.Bounds.Left + OFFSET_MESSAGES_LIST_TRAFFIC + SPACE_MESSAGES_LIST_TRAFFIC, e.Bounds.Top, e.Bounds.Width - OFFSET_MESSAGES_LIST_TRAFFIC - SPACE_MESSAGES_LIST_TRAFFIC, e.Bounds.Height), ForeColor, flags);

            }

            e.DrawFocusRectangle();
        }

        private void tsmiExportModelFTP_Click(object sender, EventArgs e)
        {
            FormImportModelSettings mims = new FormImportModelSettings(this);
            mims.Setup(1);

            if (mims.ShowDialog() == DialogResult.OK)
            {
                string value = "nilsa_" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_");
                value = getFTPModelFileName(false, value, 0);
                if (value.Length > 0)
                {
                    Cursor = Cursors.WaitCursor;

                    String _backcopy = "_tmp_nilsa_" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_") + FTP_SERVER_MODEL_NAME_POSTFIX;
                    if (File.Exists(Path.Combine(Application.StartupPath, _backcopy)))
                        File.Delete(Path.Combine(Application.StartupPath, _backcopy));

                    NILSA_ARCHIVE_Create(_backcopy, mims.Settings);

                    bool bErrors = false;
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.Credentials = new NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                            client.UploadFile(FTP_SERVER_URI + value, "STOR", Path.Combine(Application.StartupPath, _backcopy));
                        }
                    }
                    catch
                    {
                        bErrors = true;
                    }

                    if (File.Exists(Path.Combine(Application.StartupPath, _backcopy)))
                        File.Delete(Path.Combine(Application.StartupPath, _backcopy));

                    Cursor = Cursors.Arrow;

                    if (bErrors)
                        MessageBox.Show("Во время экспорта информационной модели на Сервер произошла ошибка. Проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", "Экспорт информационной модели на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Экспорт информационной модели на Сервер успешно выполнен", "Экспорт информационной модели на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


            }
        }

        private void NILSA_ARCHIVE_Personen_Extract(string openFileDialog_FileName)
        {

            if (File.Exists(openFileDialog_FileName))
            {
                using (ZipArchive archive = ZipFile.OpenRead(openFileDialog_FileName))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        entry.ExtractToFile(Path.Combine(Application.StartupPath, entry.FullName), true);
                    }
                }
            }

        }

        private string NILSA_ARCHIVE_Personen_Create(string strExternalZipFileName, List<String> listUIDs)
        {
            string strZipFileName = strExternalZipFileName;
            string strZipFilePath = Path.Combine(Application.StartupPath, strZipFileName);

            if (File.Exists(strZipFilePath))
                File.Delete(strZipFilePath);

            using (ZipArchive newZipArchive = ZipFile.Open(strZipFilePath, ZipArchiveMode.Create))
            {
                /*
                    База Персонажей (Импортировать, Не менять, Очистить):
                    _personen*.txt
                    pers_*.txt
                */
                NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_exported_personen.txt"), Application.StartupPath);
                foreach (string suid in listUIDs)
                {
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "pers_" + suid + ".txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_dialogsinitperday_" + suid + ".txt"), Application.StartupPath);
                }

                /*
                    Пулы входящих и исходящих сообщений Персонажей (Импортировать, Не менять, Очистить):
                    _msg_received_pull_*.txt
                    _msg_outgoing_pull_*.txt
                */
                foreach (string suid in listUIDs)
                {
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_msg_received_pull_" + suid + "_contacter" + ".txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_msg_received_pull_" + suid + "_groups" + ".txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_msg_outgoing_pull_" + suid + ".txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_msg_outdelayed_pull_" + suid + ".txt"), Application.StartupPath);
                }

                /*
                    Разрешенные алгоритмы Персонажей (Импортировать, Не менять, Очистить):
                    _enabledalgorithms_*.txt
                */
                foreach (string suid in listUIDs)
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_enabledalgorithms_" + suid + ".txt"), Application.StartupPath);

                /*
                    Базы Контактеров Персонажей (Импортировать, Не менять, Очистить):
                    _contacts_*.txt
                    cont_*_*.txt
                */
                foreach (string suid in listUIDs)
                {
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_contacts_" + suid + ".txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "cont_" + suid + "_*.txt"), Application.StartupPath);

                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_groups_" + suid + ".txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "grp_" + suid + "_*.txt"), Application.StartupPath);
                }

                /*
                    Привязки алгоритмов к Контактерам и Персонажам (Импортировать, Не менять, Очистить):
                    _algotithm_settings_*.txt
                */
                foreach (string suid in listUIDs)
                {
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_algotithm_settings_" + suid + ".txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_algotithm_settings_" + suid + "_*.txt"), Application.StartupPath);
                }
                /*
                    Флаги инициации диалога с Контактерами (Импортировать, Не менять, Очистить):
                    _flag_init_dialog_*_*.txt
                */
                foreach (string suid in listUIDs)
                {
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_flag_init_dialog_" + suid + "_*.txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_wall_posts_" + suid + ".txt"), Application.StartupPath);
                }

                /*
                    Последние отосланные Контактерам сообщения (Импортировать, Не менять, Очистить):
                    _lastmessage_*_*.txt
                    _prevlastmessage_*_*.txt
                */
                foreach (string suid in listUIDs)
                {
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_lastmessage_" + suid + "_*.txt"), Application.StartupPath);
                    NILSA_ARCHIVE_AddFilesToArchive(newZipArchive, System.IO.Directory.GetFiles(sDataPath, "_prevlastmessage_" + suid + "_*.txt"), Application.StartupPath);
                }

            }

            return strZipFileName;
        }

        public int tsmiExportPersonenFTP(List<String> listUIDs)
        {
            int bErrors = 2;
            string value = "personen" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_");
            value = getFTPModelFileName(false, value, 1);
            if (value.Length > 0)
            {
                Cursor = Cursors.WaitCursor;

                String _backcopy = "_tmp_personen_" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_") + FTP_SERVER_PERSONEN_NAME_POSTFIX;
                if (File.Exists(Path.Combine(Application.StartupPath, _backcopy)))
                    File.Delete(Path.Combine(Application.StartupPath, _backcopy));

                NILSA_ARCHIVE_Personen_Create(_backcopy, listUIDs);

                bErrors = 0;
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                        client.UploadFile(FTP_SERVER_URI + value, "STOR", Path.Combine(Application.StartupPath, _backcopy));
                    }
                }
                catch
                {
                    bErrors = 1;
                }

                if (File.Exists(Path.Combine(Application.StartupPath, _backcopy)))
                    File.Delete(Path.Combine(Application.StartupPath, _backcopy));

                Cursor = Cursors.Arrow;

            }

            return bErrors;
        }

        public int tsmiExportMessagesCSVFTP(int type, string filename)
        {
            int bErrors = 2;
            string value = filename;
            value = getFTPModelFileName(false, value, type);
            if (value.Length > 0)
            {
                Cursor = Cursors.WaitCursor;

                String _backcopy = filename + (type == 2 ? FTP_SERVER_EQOUT_NAME_POSTFIX : (type == 3 ? FTP_SERVER_EQIN_NAME_POSTFIX : FTP_SERVER_CONT_NAME_POSTFIX));

                bErrors = 0;
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                        client.UploadFile(FTP_SERVER_URI + value, "STOR", Path.Combine(Application.StartupPath, _backcopy));
                    }
                }
                catch
                {
                    bErrors = 1;
                }


                Cursor = Cursors.Arrow;

            }

            return bErrors;
        }

        public int tsmiImportPersonenFTP()
        {
            int bErrors = 2;

            string filename = getFTPModelFileName(true, "", 1);

            if (filename.Length > 0)
            {
                Cursor = Cursors.WaitCursor;

                if (File.Exists(Path.Combine(Application.StartupPath, filename)))
                    File.Delete(Path.Combine(Application.StartupPath, filename));

                bErrors = 0;
                try
                {
                    using (WebClient ftpClient = new WebClient())
                    {
                        ftpClient.Credentials = new System.Net.NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                        ftpClient.DownloadFile(FTP_SERVER_URI + filename, Path.Combine(Application.StartupPath, filename));
                    }
                }
                catch
                {
                    bErrors = 1;
                }

                Cursor = Cursors.Arrow;

                if (bErrors == 1)
                    MessageBox.Show("Во время загрузки Персонажей с Сервера произошла ошибка. Пожалуйста, проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", "Загрузка Персонажей с Сервера", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    NILSA_ARCHIVE_Personen_Extract(Path.Combine(Application.StartupPath, filename));
            }
            return bErrors;
        }


        public string tsmiImportMessagesCSVFTP(int type)
        {
            string retfile = "";

            string filename = getFTPModelFileName(true, "", type);

            if (filename.Length > 0)
            {
                Cursor = Cursors.WaitCursor;

                if (File.Exists(Path.Combine(Application.StartupPath, filename)))
                    File.Delete(Path.Combine(Application.StartupPath, filename));

                try
                {
                    using (WebClient ftpClient = new WebClient())
                    {
                        ftpClient.Credentials = new System.Net.NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                        ftpClient.DownloadFile(FTP_SERVER_URI + filename, Path.Combine(Application.StartupPath, filename));
                        retfile = filename;
                    }
                }
                catch
                {
                    retfile = "";
                }

                Cursor = Cursors.Arrow;

                if (retfile.Length == 0)
                    MessageBox.Show("Во время загрузки сообщений с Сервера произошла ошибка. Пожалуйста, проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", "Загрузка сообщений с Сервера", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return retfile;
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        private void tsmiExportModel_Click(object sender, EventArgs e)
        {
            FormImportModelSettings mims = new FormImportModelSettings(this);
            mims.Setup(1);

            if (mims.ShowDialog() == DialogResult.OK)
            {
                saveFileDialog.Filter = "ZIP-files (*.nilsa.zip)|*.nilsa.zip|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 0;
                saveFileDialog.FileName = "nilsa_" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_") + ".nilsa.zip";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    NILSA_ARCHIVE_Create(saveFileDialog.FileName, mims.Settings);
                }
            }
        }

        //public const string FTP_SERVER_DEMOVERSION_FILENAME_PREFIX = "Demoversion ";

        public const string FTP_SERVER_URI = "ftp://ftp.h811285380.nichost.ru/nilsa.ru/docs/";//"ftp://ftp.ourdomens.nichost.ru/";//"ftp://nilsa.ru/";
        public string FTP_SERVER_LOGIN;
        public string FTP_SERVER_PASSWORD;
        public const string FTP_SERVER_MODEL_NAME_POSTFIX = ".nilsa.zip";
        public const string FTP_SERVER_PERSONEN_NAME_POSTFIX = ".pers.zip";
        public const string FTP_SERVER_EQOUT_NAME_POSTFIX = ".eqout.csv";
        public const string FTP_SERVER_EQIN_NAME_POSTFIX = ".eqin.csv";
        public const string FTP_SERVER_CONT_NAME_POSTFIX = ".cont.csv";

        private string getPrefixByLicenceType()
        {
            switch (iLicenseType)
            {
                case LICENSE_TYPE_WORK:
                    return sLicenseUser + "_";
                case LICENSE_TYPE_PRO:
                    return sLicenseUser + "_";
                case LICENSE_TYPE_ADMIN:
                    return "ADMIN_";
                case LICENSE_TYPE_NILS:
                    return "NILS_";
            }

            return "DEMO_";
        }

        private string getFTPModelFileName(bool bImport, string initFileName = "", int modeltype = 0)
        {
            string FTP_SERVER_POSTFIX = FTP_SERVER_MODEL_NAME_POSTFIX;
            if (modeltype == 1)
                FTP_SERVER_POSTFIX = FTP_SERVER_PERSONEN_NAME_POSTFIX;
            else if (modeltype == 2)
                FTP_SERVER_POSTFIX = FTP_SERVER_EQOUT_NAME_POSTFIX;
            else if (modeltype == 3)
                FTP_SERVER_POSTFIX = FTP_SERVER_EQIN_NAME_POSTFIX;
            else if (modeltype == 4)
                FTP_SERVER_POSTFIX = FTP_SERVER_CONT_NAME_POSTFIX;

            string FTP_SERVER_PREFIX = getPrefixByLicenceType();

            List<string> directories = null;
            bool bErrors = false;
            string retval = "";
            //bool bDemoVersionOnly = !sLicenseUser.Trim().StartsWith("379");

            Cursor = Cursors.WaitCursor;
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(FTP_SERVER_URI);
                ftpRequest.Credentials = new NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251));

                directories = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    if (line.EndsWith(FTP_SERVER_POSTFIX))
                    {
                        switch (iLicenseType)
                        {
                            case LICENSE_TYPE_DEMO:
                                if (line.StartsWith(FTP_SERVER_PREFIX))
                                    directories.Add(line.Substring(0, line.Length - FTP_SERVER_POSTFIX.Length));
                                break;
                            case LICENSE_TYPE_WORK:
                                if (line.StartsWith(FTP_SERVER_PREFIX))
                                    directories.Add(line.Substring(0, line.Length - FTP_SERVER_POSTFIX.Length));
                                break;
                            case LICENSE_TYPE_PRO:
                                if (line.StartsWith(FTP_SERVER_PREFIX))
                                    directories.Add(line.Substring(0, line.Length - FTP_SERVER_POSTFIX.Length));
                                break;
                            case LICENSE_TYPE_ADMIN:
                                if (!line.StartsWith("PRO_") && !line.StartsWith("NILS_"))
                                    directories.Add(line.Substring(0, line.Length - FTP_SERVER_POSTFIX.Length));
                                break;
                            case LICENSE_TYPE_NILS:
                                directories.Add(line.Substring(0, line.Length - FTP_SERVER_POSTFIX.Length));
                                break;
                        }
                    }
                    line = streamReader.ReadLine();
                }

                streamReader.Close();
            }
            catch
            {
                bErrors = true;
            }
            Cursor = Cursors.Arrow;

            if (bErrors)
                MessageBox.Show("Во время подключения к Серверу произошла ошибка. Пожалуйста, проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", bImport ? "Импорт с Сервера" : "Экспорт на Сервер", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (directories != null)
                {
                    FormSelectFileName fsfn = new FormSelectFileName(this, modeltype);
                    fsfn.lbFiles.Items.AddRange(directories.ToArray());
                    fsfn.tbFileName.Text = "";
                    if (initFileName.Length > 0)
                    {
                        if (initFileName.StartsWith(FTP_SERVER_PREFIX))
                            fsfn.tbFileName.Text = initFileName;
                        else
                            fsfn.tbFileName.Text = FTP_SERVER_PREFIX + initFileName;

                        //if (bDemoVersionOnly)
                        //{
                        //    if (initFileName.StartsWith(FTP_SERVER_DEMOVERSION_FILENAME_PREFIX))
                        //        fsfn.tbFileName.Text = initFileName;
                        //    else
                        //        fsfn.tbFileName.Text = FTP_SERVER_DEMOVERSION_FILENAME_PREFIX + initFileName;
                        //}
                        //else
                        //    fsfn.tbFileName.Text = initFileName;
                    }
                    fsfn.tbFileName.Enabled = !bImport;
                    //fsfn.bDemoVersionOnly = bDemoVersionOnly;
                    //fsfn.checkBoxDemoVersion.Enabled = !bImport;
                    fsfn.buttonDelete.Enabled = false;

                    if (fsfn.ShowDialog() == DialogResult.OK)
                    {
                        string value = fsfn.tbFileName.Text.Trim();
                        value = CleanFileName(value);
                        if (value.Length > 0)
                            retval = value + FTP_SERVER_POSTFIX;
                    }
                }
            }
            return retval;
        }

        private void tsmiImportModelFTP_Click(object sender, EventArgs e)
        {

            string filename = getFTPModelFileName(true);

            if (filename.Length > 0)
            {
                Cursor = Cursors.WaitCursor;

                if (File.Exists(Path.Combine(Application.StartupPath, filename)))
                    File.Delete(Path.Combine(Application.StartupPath, filename));

                bool bErrors = false;
                try
                {
                    using (WebClient ftpClient = new WebClient())
                    {
                        ftpClient.Credentials = new System.Net.NetworkCredential(FTP_SERVER_LOGIN, FTP_SERVER_PASSWORD);
                        ftpClient.DownloadFile(FTP_SERVER_URI + filename, Path.Combine(Application.StartupPath, filename));
                    }
                }
                catch
                {
                    bErrors = true;
                }

                Cursor = Cursors.Arrow;

                if (bErrors)
                    MessageBox.Show("Во время загрузки информационной модели с Сервера произошла ошибка. Пожалуйста, проверьте наличие подключения к интернету или попробуйте выполнить операцию позже...", "Импорт информационной модели с Сервера", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    ImportModelFromFile(Path.Combine(Application.StartupPath, filename));
            }
        }

        private void tsmiImportModel_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "ZIP-files (*.nilsa.zip)|*.nilsa.zip|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                ImportModelFromFile(openFileDialog.FileName);
            }
        }

        private void ImportModelFromFile(string openFileDialog_FileName)
        {

            if (File.Exists(openFileDialog_FileName))
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings")))
                    File.Delete(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings"));

                using (ZipArchive archive = ZipFile.OpenRead(openFileDialog_FileName))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.Equals("_importexport_model_1.settings"))
                        {
                            entry.ExtractToFile(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings"));
                        }
                    }
                }

                FormImportModelSettings mims = new FormImportModelSettings(this);
                mims.Setup(0);

                if (mims.ShowDialog() == DialogResult.OK)
                {
                    //if (MessageBox.Show("При импорте информационной модели из архивного файла текущая информация в данной копии NILSA будет заменена в соответствии с выбранными опциями.\n\nПродолжить выполнение импорта?", "Импорт информационной модели", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    //{
                    tbStopService_Click(null, null);

                    if (mims.Settings[18] == 1)
                    {
                        String _backcopy = saveFileDialog.FileName = "nilsa_" + FormMain.sLicenseUser.Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_").Replace(".", "_") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", "_").Replace(":", "_") + ".nilsa.zip";
                        if (File.Exists(Path.Combine(Application.StartupPath, _backcopy)))
                            File.Delete(Path.Combine(Application.StartupPath, _backcopy));
                        NILSA_ARCHIVE_Create(_backcopy);
                    }
                    /*
                        string[] files = Directory.GetFiles(Application.StartupPath, "*.txt");
                        foreach (string f in files)
                            File.Delete(f);
                        files = Directory.GetFiles(Application.StartupPath, "*.columns");
                        foreach (string f in files)
                            File.Delete(f);
                        files = Directory.GetFiles(Application.StartupPath, "*.values");
                        foreach (string f in files)
                            File.Delete(f);
                        files = Directory.GetFiles(sDataPath, "*.*");
                        foreach (string f in files)
                            File.Delete(f);
                            */
                    ShowFormWait(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_45", this.Name, "Пожалуйста, подождите, операция выполняется..."));
                    if (mims.Settings[0] >= 1)
                    {
                        /*
                            Алгоритмы (Импортировать, Не менять, Очистить):
                            _algorithms_db.txt
                            _algorithms_*.txt
                            _text_search_settings_*.txt
                            _algtimers_settings_*.txt
                        */
                        //NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algorithms_db.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algorithms_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_text_search_settings_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algtimers_settings_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "FormEditPersHarValues_2_*.values"));
                    }
                    if (mims.Settings[1] >= 1)
                    {
                        /*
                            Базы сообщений Персонажа и Контактера (Импортировать, Не менять, Очистить):
                            _messages_db.txt
                            _eqinmsgdb_*.txt
                            _eqoutmsgdb_*.txt
                            LongValues0.values
                            LongValues1.values
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_messages_db.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_eqinmsgdb_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_eqoutmsgdb_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues0.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues1.values"));
                    }
                    if (mims.Settings[2] >= 1)
                    {
                        /*
                            Настройки инициации диалогов (Импортировать, Не менять, Очистить):
                            StringValues2.values
                            LongValues2.values
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "StringValues2.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues2.values"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues4.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues117.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues3.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "LongValues9.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "StringValues3.values"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues_3*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesGroups_3*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEnterContactersToImportCount_3*.values"));

                    }
                    if (mims.Settings[3] >= 1)
                    {
                        /*
                            База Персонажей (Импортировать, Не менять, Очистить):
                            _personen*.txt
                            pers_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_personen*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "pers_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_dialogsinitperday_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "_program_state*.txt"));
                    }
                    if (mims.Settings[4] >= 1)
                    {
                        /*
                            Пулы входящих и исходящих сообщений Персонажей (Импортировать, Не менять, Очистить):
                            _msg_received_pull_*.txt
                            _msg_outgoing_pull_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msg_received_pull_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msg_outgoing_pull_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msg_outdelayed_pull_*.txt"));
                    }
                    if (mims.Settings[5] >= 1)
                    {
                        /*
                            Разрешенные алгоритмы Персонажей (Импортировать, Не менять, Очистить):
                            _enabledalgorithms_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_enabledalgorithms_*.txt"));
                    }
                    if (mims.Settings[6] >= 1)
                    {
                        /*
                            Инициация диалогов Персонажей (Импортировать, Не менять, Очистить):
                            _initdialog_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_initdialog_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "resend_operator_ *.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_wall_posts_*.txt"));
                    }
                    if (mims.Settings[7] >= 1)
                    {
                        /*
                            Базы Контактеров Персонажей (Импортировать, Не менять, Очистить):
                            _contacts_*.txt
                            cont_*_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_contacts_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "cont_*_*.txt"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_groups_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "grp_*_*.txt"));
                    }
                    if (mims.Settings[8] >= 1)
                    {
                        /*
                            Привязки алгоритмов к Контактерам и Персонажам (Импортировать, Не менять, Очистить):
                            _algotithm_settings_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algotithm_settings_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_algotithm_settings_*_*.txt"));
                    }
                    if (mims.Settings[9] >= 1)
                    {
                        /*
                            Флаги инициации диалога с Контактерами (Импортировать, Не менять, Очистить):
                            _flag_init_dialog_*_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_flag_init_dialog_*_*.txt"));
                    }
                    if (mims.Settings[10] >= 1)
                    {
                        /*
                            Последние отосланные Контактерам сообщения (Импортировать, Не менять, Очистить):
                            _lastmessage_*_*.txt
                            _prevlastmessage_*_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_lastmessage_*_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_prevlastmessage_*_*.txt"));
                    }
                    if (mims.Settings[11] >= 1)
                    {
                        /*
                            Настройки характеристик Контактера (Импортировать, Не менять, Очистить):
                            _conthar.txt
                            _cont_har_*.txt
                            FormEditContactsDB.columns
                            FormEditPersHarValues_*_*.values
                            FormEditPersHarValuesStatusUpdate.values
                            FormEditPersHarValuesAlgorithmUpdate.values
                            StringValues0.values
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_conthar.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_cont_har_*.txt"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_grouphar.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_grp_har_*.txt"));

                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditContactsDB.columns"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues_0_*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValues_1_*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesStatusUpdate.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesAlgorithmUpdate.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "StringValues0.values"));
                    }
                    if (mims.Settings[12] >= 1)
                    {
                        /*
                            Настройки характеристик Персонажа (Импортировать, Не менять, Очистить):
                            _pershar.txt
                            _pers_har_*.txt
                            FormEditPersonenDB.columns
                            FormEditPersHarValuesPersonen_*_*.values
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_pershar.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_pers_har_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersonenDB.columns"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesPersonen_*_*.values"));
                    }
                    if (mims.Settings[13] >= 1)
                    {
                        /*
                            Настройки характеристик сообщений (Импортировать, Не менять, Очистить):
                            _msghar.txt
                            _msg_har_*.txt
                            _msg_har_*_*.txt
                            FormEditEQInMessagesDB.columns
                            FormEditEQOutMessagesDB.columns
                            FormEditPersHarValuesEQIn_*_*.values
                            FormEditPersHarValuesEQOut_*_*.values
                            _default_db_name.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msghar.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_msg_har_*.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditEQInMessagesDB.columns"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesEQIn_*_*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditEQOutMessagesDB.columns"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "FormEditPersHarValuesEQOut_*_*.values"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(sDataPath, "_default_db_name.txt"));
                    }
                    if (mims.Settings[14] >= 1)
                    {
                        /*
                            Счетчики программы (Импортировать, Не менять, Очистить):
                            _program_counters_*.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "_program_counters_*.txt"));
                    }
                    if (mims.Settings[15] >= 1)
                    {
                        /*
                            Настройки цветов (Импортировать, Не менять, Очистить):
                            _colors_settings.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "_colors_settings.txt"));
                    }
                    if (mims.Settings[16] >= 1)
                    {
                        /*
                            Настройки программы (Импортировать, Не менять, Очистить):
                            _program_settings.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "_program_settings.txt"));
                    }
                    if (mims.Settings[17] >= 1)
                    {
                        /*
                            База сообщений и пользователи соц. сети NILSA (Импортировать, Не менять, Очистить):
                            nilsa_messagesdb.txt
                            nilsa_userdb.txt
                        */
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "nilsa_messagesdb.txt"));
                        NILSA_ARCHIVE_DeleteFiles(System.IO.Directory.GetFiles(Application.StartupPath, "nilsa_userdb.txt"));
                    }

                    using (ZipArchive archive = ZipFile.OpenRead(openFileDialog_FileName))
                    {
                        if (mims.Settings[0] == 2)
                        {
                            /*
                                Алгоритмы (Импортировать, Не менять, Очистить):
                                _algorithms_db.txt
                                _algorithms_*.txt
                                _text_search_settings_*.txt
                                _algtimers_settings_*.txt
                            */
                            //NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algorithms_db.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algorithms_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_text_search_settings_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algtimers_settings_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValues_2_*.values");
                        }
                        if (mims.Settings[1] == 2)
                        {
                            /*
                                Базы сообщений Персонажа и Контактера (Импортировать, Не менять, Очистить):
                                _messages_db.txt
                                _eqinmsgdb_*.txt
                                _eqoutmsgdb_*.txt
                                LongValues0.values
                                LongValues1.values
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_messages_db.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_eqinmsgdb_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_eqoutmsgdb_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues0.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues1.values");
                        }
                        if (mims.Settings[2] == 2)
                        {
                            /*
                                Настройки инициации диалогов (Импортировать, Не менять, Очистить):
                                StringValues2.values
                                LongValues2.values
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "StringValues2.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "StringValues3.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues2.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues3.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues4.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues117values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "LongValues9.values");

                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValues_3*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesGroups_3*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEnterContactersToImportCount_3*.txt");
                        }
                        if (mims.Settings[3] == 2)
                        {
                            /*
                                База Персонажей (Импортировать, Не менять, Очистить):
                                _personen*.txt
                                pers_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_personen*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "pers_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_dialogsinitperday_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_program_state*.txt");
                        }
                        if (mims.Settings[4] == 2)
                        {
                            /*
                                Пулы входящих и исходящих сообщений Персонажей (Импортировать, Не менять, Очистить):
                                _msg_received_pull_*.txt
                                _msg_outgoing_pull_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msg_received_pull_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msg_outgoing_pull_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msg_outdelayed_pull_*.txt");
                        }
                        if (mims.Settings[5] == 2)
                        {
                            /*
                                Разрешенные алгоритмы Персонажей (Импортировать, Не менять, Очистить):
                                _enabledalgorithms_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_enabledalgorithms_*.txt");
                        }
                        if (mims.Settings[6] == 2)
                        {
                            /*
                                Инициация диалогов Персонажей (Импортировать, Не менять, Очистить):
                                _initdialog_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_initdialog_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "resend_operator_ *.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_wall_posts_*.txt");
                        }
                        if (mims.Settings[7] == 2)
                        {
                            /*
                                Базы Контактеров Персонажей (Импортировать, Не менять, Очистить):
                                _contacts_*.txt
                                cont_*_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_contacts_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "cont_*_*.txt");

                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_groups_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "grp_*_*.txt");
                        }
                        if (mims.Settings[8] == 2)
                        {
                            /*
                                Привязки алгоритмов к Контактерам и Персонажам (Импортировать, Не менять, Очистить):
                                _algotithm_settings_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algotithm_settings_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_algotithm_settings_*_*.txt");
                        }
                        if (mims.Settings[9] == 2)
                        {
                            /*
                                Флаги инициации диалога с Контактерами (Импортировать, Не менять, Очистить):
                                _flag_init_dialog_*_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_flag_init_dialog_*_*.txt");
                        }
                        if (mims.Settings[10] == 2)
                        {
                            /*
                                Последние отосланные Контактерам сообщения (Импортировать, Не менять, Очистить):
                                _lastmessage_*_*.txt
                                _prevlastmessage_*_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_lastmessage_*_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_prevlastmessage_*_*.txt");
                        }
                        if (mims.Settings[11] == 2)
                        {
                            /*
                                Настройки характеристик Контактера (Импортировать, Не менять, Очистить):
                                _conthar.txt
                                _cont_har_*.txt
                                FormEditContactsDB.columns
                                FormEditPersHarValues_*_*.values
                                FormEditPersHarValuesStatusUpdate.values
                                FormEditPersHarValuesAlgorithmUpdate.values
                                StringValues0.values
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_conthar.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_cont_har_*.txt");

                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_grouphar.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_grp_har_*.txt");

                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditContactsDB.columns");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValues_0_*.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValues_1_*.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesStatusUpdate.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesAlgorithmUpdate.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "StringValues0.values");
                        }
                        if (mims.Settings[12] == 2)
                        {
                            /*
                                Настройки характеристик Персонажа (Импортировать, Не менять, Очистить):
                                _pershar.txt
                                _pers_har_*.txt
                                FormEditPersonenDB.columns
                                FormEditPersHarValuesPersonen_*_*.values
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_pershar.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_pers_har_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersonenDB.columns");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesPersonen_*_*.values");
                        }
                        if (mims.Settings[13] == 2)
                        {
                            /*
                                Настройки характеристик сообщений (Импортировать, Не менять, Очистить):
                                _msghar.txt
                                _msg_har_*.txt
                                _msg_har_*_*.txt
                                FormEditEQInMessagesDB.columns
                                FormEditEQOutMessagesDB.columns
                                FormEditPersHarValuesEQIn_*_*.values
                                FormEditPersHarValuesEQOut_*_*.values
                                _default_db_name.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msghar.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_msg_har_*.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditEQInMessagesDB.columns");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesEQIn_*_*.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditEQOutMessagesDB.columns");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "FormEditPersHarValuesEQOut_*_*.values");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_default_db_name.txt");
                        }
                        if (mims.Settings[14] == 2)
                        {
                            /*
                                Счетчики программы (Импортировать, Не менять, Очистить):
                                _program_counters_*.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_program_counters_*.txt");
                        }
                        if (mims.Settings[15] == 2)
                        {
                            /*
                                Настройки цветов (Импортировать, Не менять, Очистить):
                                _colors_settings.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_colors_settings.txt");
                        }
                        if (mims.Settings[16] == 2)
                        {
                            /*
                                Настройки программы (Импортировать, Не менять, Очистить):
                                _program_settings.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "_program_settings.txt");
                        }
                        if (mims.Settings[17] == 2)
                        {
                            /*
                                База сообщений и пользователи соц. сети NILSA (Импортировать, Не менять, Очистить):
                                nilsa_messagesdb.txt
                                nilsa_userdb.txt
                            */
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "nilsa_messagesdb.txt");
                            NILSA_ARCHIVE_ExtractFilesFromArchive(archive, "nilsa_userdb.txt");
                        }
                    }
                    HideFormWait();
                    MessageBox.Show(NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_39", this.Name, "Импорт информационной модели завершен. Сейчас программа будет перезапущена для применения настроек импортированной модели."), NilsaUtils.Dictonary_GetText(userInterface, "messageboxText_40", this.Name, "Импорт информационной модели"));

                    FormMain_FormClosing_Action = false;
                    System.Diagnostics.Process.Start(Path.Combine(Application.StartupPath, "Nilsa.exe"));
                    Close();
                    //}
                }
            }

        }

        private void NILSA_ARCHIVE_ExtractFilesFromArchive(ZipArchive archive, string _mask)
        {
            Regex mask = new Regex(_mask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (mask.IsMatch(entry.FullName))
                    entry.ExtractToFile(Path.Combine(Application.StartupPath, entry.FullName), true);
            }
        }
        private void NILSA_ARCHIVE_DeleteFiles(string[] files)
        {
            foreach (string f in files)
                File.Delete(f);
        }

        private void tbUpdateLists_Click(object sender, EventArgs e)
        {

            string[] filesToDelete = System.IO.Directory.GetFiles(sDataPath, "_msg_har_*.txt");
            foreach (string f in filesToDelete)
            {
                try
                {
                    File.Delete(f);
                }
                catch
                {

                }
            }
            SaveEQInMessageDB(false, true);
            SaveEQOutMessageDB(true);
        }
    }
}
