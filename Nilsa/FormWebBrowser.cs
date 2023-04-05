//using DotNetBrowser;
//using DotNetBrowser.WinForms;
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
using CefSharp.WinForms;
using CefSharp;
using CefSharp.WinForms.Internals;
using CefSharp.Internals;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using VkNet.Model.Attachments;

namespace Nilsa
{
    public partial class FormWebBrowser : Form, IDisposable
    {
        FormMain mFormMain;

        String mUserLogin;
        String mUserPassword;
        uint mCommand = WebBrowserCommand.None;
        long mContacterID = -1;
        string mPostsToRepost = "";
        string mGroupsList = "";
        string mMessageToSend = "";
        string statusText = "";
        private int _responseFromBrowserReadLines = Convert.ToInt32(File.ReadAllText(Path.Combine(Application.StartupPath, "_response_from_browser_Read_Lines.txt")));
        private int _requestToBrowserReadLines = Convert.ToInt32(File.ReadAllText(Path.Combine(Application.StartupPath, "_requset_to_browser_Read_Lines.txt")));
        private string _actualResponse = ""; 

        bool blogin = false;
        bool blogout = false;

        bool bcontacternavigate = false;
        bool mRepostPosts = false;
        bool bcontacterrepostinit = true;
        private readonly ChromiumWebBrowser browser;

        private int autoclosedelay = 15;
        private int autoclosedelaydefault = 15;
        private int dTimerDalay = 1;
        private bool formLoading = false;

        private void loadTimers()
        {
            //List<String> lstList = new List<String>();
            //if (File.Exists(Path.Combine(Application.StartupPath, "_web_browser_timers.txt")))
            //{
            //    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_web_browser_timers.txt"));
            //    lstList = new List<String>(srcFile);
            //    try
            //    {
            //        if (lstList.Count > 0) autoclosedelaydefault = Convert.ToInt32(lstList[0]);
            //    }
            //    catch
            //    {
            //        autoclosedelaydefault = 15;
            //    }
            //    try
            //    {
            //        if (lstList.Count > 1) timerAutorize.Interval = Convert.ToInt32(lstList[1]);
            //    }
            //    catch
            //    {
            //        timerAutorize.Interval = 3000;
            //    }
            //    try
            //    {
            //        if (lstList.Count > 2) timerDisconnect.Interval = Convert.ToInt32(lstList[2]);
            //    }
            //    catch
            //    {
            //        timerDisconnect.Interval = 3000;
            //    }
            //}
        }

        void IDisposable.Dispose()
        {
            browser.Dispose();
        }

        private void saveTimers()
        {
            //List<String> lstList = new List<String>();
            //lstList.Add(autoclosedelaydefault.ToString());
            //lstList.Add(timerAutorize.Interval.ToString());
            //lstList.Add(timerDisconnect.Interval.ToString());
            //File.WriteAllLines(Path.Combine(Application.StartupPath, "_web_browser_timers.txt"), lstList, Encoding.UTF8);
        }

        private int readSettingValueInt(int idx, List<String> lstList, int defaultValue)
        {
            int retval = defaultValue;
            try
            {
                if (lstList.Count > idx) retval = Convert.ToInt32(lstList[idx]);
            }
            catch
            {
                retval = defaultValue;
            }
            return retval;
        }

        private void loadSettings()
        {
            if (noClose)
            {
                int defaultScaleBrowser = -2;
                int x = Screen.PrimaryScreen.WorkingArea.Size.Width / 2;
                int y = Screen.PrimaryScreen.WorkingArea.Size.Height / 2;
                int w = Screen.PrimaryScreen.WorkingArea.Size.Width / 2;
                int h = Screen.PrimaryScreen.WorkingArea.Size.Height / 2;

                List<String> lstList = new List<String>();
                if (File.Exists(Path.Combine(Application.StartupPath, "_web_browser_settings.txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_web_browser_settings.txt"));
                    lstList = new List<String>(srcFile);
                    x = readSettingValueInt(0, lstList, x);
                    y = readSettingValueInt(1, lstList, y);
                    w = readSettingValueInt(2, lstList, w);
                    h = readSettingValueInt(3, lstList, h);
                    defaultScaleBrowser = readSettingValueInt(4, lstList, defaultScaleBrowser);
                }

                scaleBrowser = defaultScaleBrowser;
                if (x > Screen.PrimaryScreen.WorkingArea.Size.Width || y > Screen.PrimaryScreen.WorkingArea.Size.Height || (x + w) < 0 || (y + h) < 0)
                {
                    x = Screen.PrimaryScreen.WorkingArea.Size.Width / 2;
                    y = Screen.PrimaryScreen.WorkingArea.Size.Height / 2;
                    w = Screen.PrimaryScreen.WorkingArea.Size.Width / 2;
                    h = Screen.PrimaryScreen.WorkingArea.Size.Height / 2;
                }
                this.Location = new Point(x, y);
                this.Size = new Size(w, h);
            }
        }

        public void saveSettings()
        {
            if (noClose)
            {
                List<String> lstList = new List<String>();
                lstList.Add(this.Location.X.ToString());
                lstList.Add(this.Location.Y.ToString());
                lstList.Add(this.Size.Width.ToString());
                lstList.Add(this.Size.Height.ToString());
                lstList.Add(this.scaleBrowser.ToString());
                File.WriteAllLines(Path.Combine(Application.StartupPath, "_web_browser_settings.txt"), lstList, Encoding.UTF8);
            }
        }

        private bool noClose = false;
        int scaleBrowser = -2;

        int[] workTimersCycle = new int[6];
        int[] workTimersInterval = new int[6];
        System.Windows.Forms.Timer[] workTimers = new System.Windows.Forms.Timer[6];

        public FormWebBrowser(FormMain _formmain, bool _noClose = false)
        {
            noClose = _noClose;

            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            workTimers[0] = timer0;
            workTimers[1] = timer1;
            workTimers[2] = timer2;
            workTimers[3] = timer3;
            workTimers[4] = timer4;
            workTimers[5] = timer5;

            if (noClose)
                btnCloseText = "Таймер";
            else
                btnCloseText = buttonExit.Text;

            autoclosedelaydefault = 15;
            timerAutorize.Interval = 3000;
            timerDisconnect.Interval = 3000;
            loadTimers();
            saveTimers();


            blogout = false;
            blogin = false;


            browser = new ChromiumWebBrowser("https://vk.com")
            {
                Dock = DockStyle.Fill,
            };

            groupBox1.Controls.Add(browser);

            if (noClose)
            {
                ControlBox = true;
                MinimizeBox = true;
                MaximizeBox = true;
                SizeGripStyle = SizeGripStyle.Show;
                TopMost = true;
                TopLevel = true;
                //buttonExit.Enabled = false;
                AutoSize = false;
                toolStrip1.Visible = false;

                //scaleBrowser = -2;
                //this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2);
                //this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Size.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2);
                loadSettings();

                setButtonExitMessage("Ожидание...");
                setStatusMessage("Ожидание...");
            }
            else
            {
                this.Location = new Point(0, 0);
                this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            }
        }

        private string btnCloseText = "";
        List<HtmlElement> lstWallPosts = null;
        private bool bGotoNews = false;
        bool processEnd = false;

        async Task WaitForEnd()
        {
            await Task.Delay(1000);
        }

        private void WaitNSeconds(int segundos)
        {
            if (segundos < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(segundos);
            while (DateTime.Now < _desired)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }

        public void WaitResult()
        {
            while (!processEnd)
            {
                WaitNSeconds(1);
                //Console.WriteLine("WaitResult:" + DateTime.Now.ToString("HH:mm:ss"));
            }
        }

        public void Init()
        {
            if (noClose)
            {
                browser.LoadingStateChanged += OnLoadingStateChanged;
                browser.TitleChanged += OnBrowserTitleChanged;
                browser.AddressChanged += OnBrowserAddressChanged;
                //browser.ConsoleMessage += OnBrowserConsoleMessage;
                //browser.StatusMessage += OnBrowserStatusMessage;
            }
        }

        public void stopAllTimers()
        {
            processEnd = true;
            this.InvokeOnUiThreadIfRequired(() =>
            {
                timer0.Enabled = false;
                timer1.Enabled = false;
                timer2.Enabled = false;
                timer3.Enabled = false;
                timer4.Enabled = false;
                timer5.Enabled = false;
                timerAutoClose.Enabled = false;
                timerAutorize.Enabled = false;
                timerDisconnect.Enabled = false;
                timerCondition.Enabled = false;
            });
        }

        string personeName;
        public bool errorSendMessage = false;
        public long loggedPersoneID = -1;
        public string photoURL = "";
        public Persone personeAtrributes;
        public Persone contactAtrributes;
        public Persone personeAtrributesFriends;

        int iStep = -1;
        bool bStart = false;
        private string callToBrowser = "";


        public void Setup(String sLogin, String sPassword, uint _iCommand, long _contacterID = -1, string _FirstList = "", string _SecondList = "", string _personeName = "")
        {
            processEnd = false;
            //TopMost = true;

            errorSendMessage = false;
            personeName = _personeName;
            autoclosedelaydefault = 45;
            timerAutorize.Interval = 3000;
            timerDisconnect.Interval = 3000;
            loadTimers();

            autoclosedelay = autoclosedelaydefault;
            mCommand = _iCommand;
            if (sLogin.StartsWith("7"))
                mUserLogin = "+" + sLogin;
            else
                mUserLogin = sLogin;
            mUserPassword = sPassword;
            mContacterID = -1;
            mRepostPosts = false;
            mPostsToRepost = "";
            mGroupsList = "";
            statusText = "";
            setStatusMessage(statusText);
            Random rnd = new Random();
            setButtonExitMessage(btnCloseText);

            for (int i = 0; i < 6; i++)
                workTimersInterval[i] = 5;
            workTimersInterval[0] = 2;

            if (!noClose)
            {
                browser.LoadingStateChanged += OnLoadingStateChanged;
                browser.TitleChanged += OnBrowserTitleChanged;
                browser.AddressChanged += OnBrowserAddressChanged;
                //browser.ConsoleMessage += OnBrowserConsoleMessage;
                //browser.StatusMessage += OnBrowserStatusMessage;
            }

            bool enableTimeoutTimer = true;
            string startPage = "http://vk.com/";
            iStep = 0;
            blogin = false;
            blogout = false;
            bStart = true;

            switch (mCommand)
            {
                case WebBrowserCommand.Autorize:
                    enableTimeoutTimer = false;
                    blogin = false;
                    blogout = true;
                    mContacterID = _contacterID;
                    break;
                case WebBrowserCommand.CheckPersonePage:
                    blogin = false;
                    blogout = true;
                    break;

                case WebBrowserCommand.LoginPersone:                    // Done!
                    loggedPersoneID = -1;
                    blogin = false;
                    blogout = true;
                    statusText = "Авторизация Персонажа... ";
                    setStatusMessage(statusText + "ждите...");
                    workTimersInterval[1] = 6;
                    workTimersInterval[5] = 6;

                    //bStart = false;
                    //statusText = "Авторизация Персонажа... ";
                    //setStatusMessage(statusText + "ждите...");
                    //workTimersInterval[5] = 3;

                    //if (browserAddress.StartsWith("https://vk.com"))
                    //{
                    //    iStep = 1;
                    //    doAction();
                    //}
                    break;

                case WebBrowserCommand.GoToPersonePage:
                    enableTimeoutTimer = false;
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.GetPhotoURL:                     // Done!
                    photoURL = "";
                    mContacterID = _contacterID;

                    statusText = "Получение адреса фотографии... ";
                    setStatusMessage(statusText + "ждите...");
                    autoclosedelaydefault = 25;

                    bStart = false;

                    if (mContacterID != 330643598)
                    {
                        if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                        {
                            iStep = 1;
                            doAction();
                        }
                        else
                            startPage = "https://vk.com/id" + mContacterID.ToString();
                    }
                    else
                    {
                        contactAtrributes.FirstName = "Internal";
                        contactAtrributes.LastName = "Persone";
                        iStep = -1;
                        autoclosedelay = 1;
                    }
                    break;

                case WebBrowserCommand.GetPersoneAttributes:                     // Done!
                    mContacterID = _contacterID;
                    personeAtrributes = new Persone();
                    personeAtrributes.id = mContacterID;

                    statusText = "Получение хар-к Персонажа... ";
                    setStatusMessage(statusText + "ждите...");
                    autoclosedelaydefault = 25;

                    //workTimersInterval[5] = 7;
                    //workTimersInterval[1] = 6;
                    bStart = false;

                    //if (browserAddress.StartsWith("https://vk.com"))
                    //{
                    //    iStep = 1;
                    //    doAction();
                    //}
                    if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                    {
                        iStep = 1;
                        doAction();
                    }
                    else
                        startPage = "https://vk.com/id" + mContacterID.ToString();
                    break;


                case WebBrowserCommand.GetPersoneFriendsCount:                     // Done!
                    mContacterID = _contacterID;
                    personeAtrributesFriends = new Persone();
                    personeAtrributesFriends.id = mContacterID;

                    statusText = "Получение кол-ва друзей Персонажа... ";
                    setStatusMessage(statusText + "ждите...");
                    autoclosedelaydefault = 25;

                    bStart = false;

                    if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                    {
                        iStep = 1;
                        doAction();
                    }
                    else
                        startPage = "https://vk.com/id" + mContacterID.ToString();
                    break;

                case WebBrowserCommand.GetContactAttributes:                     // Done!
                    mContacterID = _contacterID;
                    contactAtrributes = new Persone();
                    contactAtrributes.id = mContacterID;

                    statusText = "Получение хар-к Контактёра... ";
                    setStatusMessage(statusText + "ждите...");
                    autoclosedelaydefault = 25;

                    //workTimersInterval[5] = 7;
                    //workTimersInterval[1] = 6;
                    bStart = false;

                    if (mContacterID != 330643598)
                    {
                        if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                        {
                            iStep = 1;
                            doAction();
                        }
                        else
                            startPage = "https://vk.com/id" + mContacterID.ToString();
                    }
                    else
                    {
                        contactAtrributes.FirstName = "Internal";
                        contactAtrributes.LastName = "Persone";
                        iStep = -1;
                        autoclosedelay = 1;
                    }
                    break;

                case WebBrowserCommand.GoToContactPage:
                    enableTimeoutTimer = false;
                    startPage = "http://vk.com/id" + mContacterID.ToString();
                    bStart = false;
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.ShowContactPage:
                    startPage = "http://vk.com/id" + mContacterID.ToString();
                    bStart = false;
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.ShowFriendsPage:
                    break;

                //case WebBrowserCommand.RepostSetOfWallPostsToWall:
                //    mContacterID = _contacterID;
                //    mRepostPosts = true;
                //    mPostsToRepost = _FirstList.Trim();
                //    if (mPostsToRepost.Length == 0)
                //        mPostsToRepost = "1,";
                //    if (mPostsToRepost[mPostsToRepost.Length - 1] != ',')
                //        mPostsToRepost += ",";
                //    break;

                case WebBrowserCommand.LikeContacterAva:
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.JoinCommunity:
                    mGroupsList = _FirstList.Trim();
                    break;

                case WebBrowserCommand.SendMessage:                    // Done!
                    errorSendMessage = true;
                    groupBox1.Text = _personeName;
                    statusText = "Посылка сообщения... ";
                    setStatusMessage(statusText + "ждите...");
                    autoclosedelaydefault = 25;
                    workTimersInterval[1] = 4;
                    workTimersInterval[2] = 4;
                    workTimersInterval[5] = 5;

                    mMessageToSend = _FirstList.Trim();
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.AddToFriends:                    // Done!
                    errorSendMessage = true;
                    groupBox1.Text = _personeName;
                    statusText = "Добавление в друзья... ";
                    setStatusMessage(statusText + "ждите...");
                    autoclosedelaydefault = 25;
                    workTimersInterval[1] = 2;
                    workTimersInterval[2] = 4;
                    workTimersInterval[5] = 5;

                    mMessageToSend = _FirstList.Trim();
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.ReadMessages:                    // Done!
                    groupBox1.Text = _personeName;
                    statusText = "Чтение новых сообщений... ";
                    setStatusMessage(statusText + "ждите...");

                    autoclosedelaydefault = 25;

                    currentPeer = null;
                    peers.Clear();
                    messages.Clear();

                    bStart = false;

                    if ("https://vk.com/im?tab=unread".Equals(browserAddress))
                    {
                        iStep = 1;
                        doAction();
                    }
                    else
                    {
                        startPage = "https://vk.com/im?tab=unread";
                    }
                    break;

                case WebBrowserCommand.ReadHistory:                    // Done!
                    groupBox1.Text = _personeName;
                    statusText = "Чтение истории... ";
                    setStatusMessage(statusText + "ждите...");
                    mContacterID = _contacterID;
                    autoclosedelaydefault = 25;

                    //workTimersInterval[5] = 6;
                    currentPeer = new Peer(mContacterID, 200);
                    peers.Clear();
                    messages.Clear();

                    bStart = false;

                    if (browserAddress.Equals("https://vk.com/im?sel=" + mContacterID.ToString()))
                    {
                        iStep = 1;
                        doAction();
                    }
                    else
                    {
                        startPage = "https://vk.com/im?sel=" + mContacterID.ToString();
                    }
                    break;

                case WebBrowserCommand.RepostPostFromWallToWall:
                    mContacterID = _contacterID;
                    mRepostPosts = true;
                    mPostsToRepost = _FirstList.Trim();
                    if (mPostsToRepost.Length == 0)
                        mPostsToRepost = "1,";
                    if (mPostsToRepost[mPostsToRepost.Length - 1] != ',')
                        mPostsToRepost += ",";
                    break;

                case WebBrowserCommand.RepostPostFromGroupToWall:
                    mRepostPosts = true;
                    mGroupsList = _FirstList.Trim();
                    mPostsToRepost = _SecondList.Trim();
                    if (mPostsToRepost.Length == 0)
                        mPostsToRepost = "1,";
                    if (mPostsToRepost[mPostsToRepost.Length - 1] != ',')
                        mPostsToRepost += ",";
                    break;
            }

            if (iStep == 0)
                LoadUrl(startPage);

            if (enableTimeoutTimer)
                enableTimer(timerAutoClose);
        }

        private void doAction()
        {
            
            switch (mCommand)
            {
                case WebBrowserCommand.GetPhotoURL:
                    if (iStep == 1)
                    {
                        try
                        {
                            browser.EvaluateScriptAsync("document.getElementsByClassName('page_avatar_img')[0].src;").ContinueWith(x =>
                            {
                                var response = x.Result;
                                //callToBrowser += "document.getElementsByClassName('page_avatar_img')[0].src;\n";
                                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementsByClassName('page_avatar_img')[0].src;\n", Encoding.UTF8);
                                if (response.Success && response.Result != null)
                                {
                                    var startDate = response.Result;

                                    if (startDate != null)
                                    {
                                        photoURL = startDate.ToString();
                                    }
                                }

                                autoclosedelay = 1;
                            });
                        }
                        catch
                        {

                        }
                    }
                    break;

                case WebBrowserCommand.GetContactAttributes:
                    if (iStep == 1)
                    {
                        setStatusMessage(statusText + "читаем атрибуты Контактёра...");
                        autoclosedelay = autoclosedelaydefault;
                        TryToClickElementByClassName("profile_more_info_link", -1, -1);
                        autoclosedelay = autoclosedelaydefault;
                        Task ts = getContactAttributesTask();
                    }
                    break;

                case WebBrowserCommand.GetPersoneFriendsCount:
                    if (iStep == 1)
                    {
                        setStatusMessage(statusText + "читаем атрибуты Персонажа...");
                        autoclosedelay = autoclosedelaydefault;
                        TryToClickElementByClassName("profile_more_info_link", -1, -1);
                        autoclosedelay = autoclosedelaydefault;
                        Task ts = getPersoneAttributesFriendsTask(); 
                    }
                    break;

                case WebBrowserCommand.GetPersoneAttributes:
                    if (iStep == 1)
                    {
                        setStatusMessage(statusText + "читаем атрибуты Персонажа...");
                        autoclosedelay = autoclosedelaydefault;
                        //if (!TryToClickElementByID("top_edit_link", -1, -1))
                        if (!TryToClickElementByID("profile_edit_act", -1, -1))
                            iStep = -1;
                    }
                    else if (iStep > 1)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        Task ts = getPersoneAttributesTask();
                    }
                    break;

                case WebBrowserCommand.LoginPersone:
                    if (iStep == 1)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        Task ts = logoutTask();
                    }
                    else if (iStep == 2)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        Task ts = loginTask();
                    }
                    else if (iStep == 3)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        Task ts = checkLoginTask();
                    }
                    break;

                case WebBrowserCommand.ReadMessages:
                    if (iStep == 1)
                    {
                        setStatusMessage(statusText + "обрабатываем диалоги...");
                        autoclosedelay = autoclosedelaydefault;
                        Task ts = getPeers();
                    }
                    else if (iStep == 2)
                    {
                        if (peers.Count > 0)
                        {
                            setStatusMessage(statusText + "читаем новые сообщения из диалога...");

                            autoclosedelay = autoclosedelaydefault;
                            currentPeer = peers[0];
                            peers.RemoveAt(0);
                            Task ts = getMessages();
                        }
                        else
                            iStep = -1;
                    }
                    break;

                case WebBrowserCommand.ReadHistory:
                    if (iStep == 1)
                    {
                        setStatusMessage(statusText + "сохраняем сообщения...");
                        autoclosedelay = autoclosedelaydefault;
                        Task ts = getHistory();
                    }
                    else if (iStep == 2)
                    {
                        setStatusMessage(statusText + "всё...");
                        autoclosedelay = 1;
                    }
                    break;
            }

            if (iStep == -1)
            {
                setStatusMessage(statusText + "ошибка...");
                autoclosedelay = 1;
            }

            //File.WriteAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), callToBrowser, Encoding.UTF8);
        }

        private void doCondition()
        {
            switch (mCommand)
            {
                case WebBrowserCommand.GetPhotoURL:
                    if (iStep == 0)
                    {
                        if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                        {
                            iStep = 1;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    break;

                case WebBrowserCommand.GetContactAttributes:
                    if (iStep == 0)
                    {
                        if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                        {
                            iStep = 1;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    break;

                case WebBrowserCommand.GetPersoneFriendsCount:
                    if (iStep == 0)
                    {
                        if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                        {
                            iStep = 1;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    break;

                case WebBrowserCommand.GetPersoneAttributes:
                    if (iStep == 0)
                    {
                        if (browserAddress.StartsWith("https://vk.com"))
                        {
                            iStep = 1;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    else if (iStep == 1)
                    {
                        if (browserAddress.StartsWith("https://vk.com"))
                        {
                            iStep++;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    else if (iStep > 1)
                    {
                        if (browserAddress.StartsWith("https://vk.com"))
                        {
                            //iStep++;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    break;

                case WebBrowserCommand.LoginPersone:
                    if (iStep == 0)
                    {
                        if (browserAddress.StartsWith("https://vk.com"))
                        {
                            iStep = 2;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    else
                    {
                        if (browserAddress.StartsWith("https://vk.com"))
                            doAction();
                        else
                            iStep = -1;
                    }
                    break;

                case WebBrowserCommand.ReadHistory:
                    if (iStep == 0)
                    {
                        if (browserAddress.Equals("https://vk.com/im?sel=" + mContacterID.ToString()))
                        {
                            iStep = 1;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    else if (iStep == 1)
                    {
                        iStep = 2;
                        doAction();
                    }
                    break;

                case WebBrowserCommand.ReadMessages:
                    if (iStep == 0)
                    {
                        if ("https://vk.com/im?tab=unread".Equals(browserAddress))
                        {
                            iStep = 1;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    else if (iStep == 1)
                    {
                        if (browserAddress.StartsWith("https://vk.com/im?sel="))
                        {
                            iStep = 2;
                            doAction();
                        }
                        else
                            iStep = -1;
                    }
                    break;
            }

            if (iStep == -1)
            {
                setStatusMessage(statusText + "ошибка...");
                autoclosedelay = 1;
            }
        }

        public class Persone
        {
            public long id;
            public string FirstName;
            public string LastName;
            public string Sex;
            public string Relation;

            public string BirthDate;

            public string City;
            public string Country;

            public string CountersFriends;
            public string Online;
            public string LastSeen;

            public Persone()
            {
                id = -1;
                FirstName = "";
                LastName = "";
                Sex = "";
                Relation = "";
                BirthDate = "";
                City = "";
                Country = "";
                CountersFriends = "";
                Online = "Unknown";
                LastSeen = "";
            }

        }

        private async Task getContactAttributesTask()
        {
            try
            {
                string value;
                int idx;

                value = await getBrowserFieldValueByClassName("page_name", "");
                idx = value.IndexOf(' ');
                if (idx > 0)
                {
                    contactAtrributes.FirstName = value.Substring(0, idx);
                    contactAtrributes.LastName = value.Substring(idx + 1);
                }
                else
                    contactAtrributes.FirstName = value;

                contactAtrributes.Sex = "Не указан";
                contactAtrributes.CountersFriends = await getBrowserFieldValueByClassName("count", "");

                value = await getBrowserFieldValueByClassName("profile_online_lv", "");
                if (value.StartsWith("Online"))
                    contactAtrributes.Online = "ON line";
                else
                {
                    if (value.ToLower().StartsWith("заходил "))
                    {
                        contactAtrributes.Sex = "Мужской";
                        contactAtrributes.LastSeen = value;
                    }
                    else if (value.ToLower().StartsWith("заходила "))
                    {
                        contactAtrributes.Sex = "Женский";
                        contactAtrributes.LastSeen = value;
                    }

                    contactAtrributes.Online = "OFF line";
                }

                //value = await getBrowserFieldValueByClassName("profile_time_lv", "");
                //contactAtrributes.LastSeen = value;
                //if (value.Length > 0)
                //{
                //    contactAtrributes.Online = "OFF line";

                //    if (value.ToLower().StartsWith("заходил "))
                //        contactAtrributes.Sex = "Мужской";
                //    else if (value.ToLower().StartsWith("заходила "))
                //        contactAtrributes.Sex = "Женский";
                //}
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

                if (source.Contains("<div class=\"labeled\">"))
                {

                    string pattern = "<div class=.labeled.><a .?href=..search([^\">]*).>(.*)<.a>";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(source);

                    while (match.Success)
                    {
                        if (match.Groups[1].Value != null && match.Groups[1].Value.Length > 0 && match.Groups[2].Value != null && match.Groups[2].Value.Length > 0)
                        {
                            if (match.Groups[1].Value.StartsWith("?c[name]=0&amp;c[section]=people&amp;c[country]="))
                                contactAtrributes.City = match.Groups[2].Value;
                            else if (match.Groups[1].Value.StartsWith("?c[name]=0&amp;c[section]=people&amp;c[status]="))
                            {
                                contactAtrributes.Relation = match.Groups[2].Value;
                            }
                            else if (match.Groups[1].Value.StartsWith("?c[section]=people&amp;c[bday]="))
                            {
                                string res = "";
                                bool bAdd = true;
                                string date = match.Groups[2].Value;
                                int len = date.Length;
                                char prevch = ' ';
                                for (int i = 0; i < len; i++)
                                {
                                    char ch = date[i];
                                    if (ch == '<')
                                        bAdd = false;
                                    else if (ch == '>')
                                        bAdd = true;
                                    else
                                    {
                                        if (bAdd)
                                        {
                                            if (ch != ' ')
                                                res += ch;
                                            else
                                            {
                                                if (prevch != ch)
                                                    res += ch;
                                            }
                                        }
                                        prevch = ch;
                                    }

                                }
                                if (res.IndexOf(' ') > 0)
                                {
                                    string[] dateparts = res.Split(' ');
                                    if (dateparts.Length >= 3)
                                    {
                                        int day = -1;
                                        try
                                        {
                                            day = Convert.ToInt32(dateparts[0]);
                                        }
                                        catch
                                        {
                                            day = -1;
                                        }

                                        if (day > 0)
                                        {
                                            int year = -1;
                                            try
                                            {
                                                year = Convert.ToInt32(dateparts[2]);
                                            }
                                            catch
                                            {
                                                year = -1;
                                            }

                                            if (year > 1900 && year < 3000)
                                            {
                                                int month = -1;
                                                string smonth = dateparts[1].ToLower();
                                                if (smonth.Length >= 2)
                                                {
                                                    if (smonth.StartsWith("янв"))
                                                        month = 1;
                                                    else if (smonth.StartsWith("фев"))
                                                        month = 2;
                                                    else if (smonth.StartsWith("мар"))
                                                        month = 3;
                                                    else if (smonth.StartsWith("апр"))
                                                        month = 4;
                                                    else if (smonth.StartsWith("ма"))
                                                        month = 5;
                                                    else if (smonth.StartsWith("июн"))
                                                        month = 6;
                                                    else if (smonth.StartsWith("июл"))
                                                        month = 7;
                                                    else if (smonth.StartsWith("авг"))
                                                        month = 8;
                                                    else if (smonth.StartsWith("сен"))
                                                        month = 9;
                                                    else if (smonth.StartsWith("окт"))
                                                        month = 10;
                                                    else if (smonth.StartsWith("ноя"))
                                                        month = 11;
                                                    else if (smonth.StartsWith("дек"))
                                                        month = 12;

                                                    if (month > 0)
                                                    {
                                                        contactAtrributes.BirthDate = day.ToString() + "." + month.ToString() + "." + year.ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // Переходим к следующему совпадению
                        match = match.NextMatch();
                    }
                }

            }
            catch (Exception)
            {
            }

            autoclosedelay = 1;
        }

        private async Task getPersoneAttributesFriendsTask()
        {
            try
            {
                string value;
                int idx;

                personeAtrributesFriends.CountersFriends = await getBrowserFieldValueByClassName("count", "");

            }
            catch (Exception)
            {
            }

            autoclosedelay = 1;
        }

        private async Task getPersoneAttributesTask()
        {
            if (iStep == 2)
            {
                try
                {
                    string value;

                    value = await getBrowserFieldValueByID("pedit_first_name", "");
                    personeAtrributes.FirstName = value != null ? value : "";

                    value = await getBrowserFieldValueByID("pedit_last_name", "");
                    personeAtrributes.LastName = value != null ? value : "";

                    value = await getBrowserFieldValueByID("pedit_sex", "0");
                    personeAtrributes.Sex = (value == null) ? "Не указан" : ("2".Equals(value) ? "Мужской" : ("1".Equals(value) ? "Женский" : "Не указан"));

                    value = await getBrowserFieldValueByID("pedit_home_town", "0");
                    personeAtrributes.City = value != null ? value : "";

                    string bday = await getBrowserFieldValueByID("pedit_bday", "");
                    string bmonth = await getBrowserFieldValueByID("pedit_bmonth", "");
                    string byear = await getBrowserFieldValueByID("pedit_byear", "");

                    personeAtrributes.BirthDate = bday + "." + bmonth + "." + byear;
                    if (personeAtrributes.BirthDate.Length < 5)
                        personeAtrributes.BirthDate = "";

                    value = await getBrowserFieldValueByID("pedit_status", "");
                    personeAtrributes.Relation = "Не указано";
                    if ("1".Equals(value))
                        personeAtrributes.Relation = "Не женат (не замужем)";
                    else if ("2".Equals(value))
                        personeAtrributes.Relation = "Встречаюсь";
                    else if ("3".Equals(value))
                        personeAtrributes.Relation = "Помолвлен(-а)";
                    else if ("4".Equals(value))
                        personeAtrributes.Relation = "Женат (замужем)";
                    else if ("5".Equals(value))
                        personeAtrributes.Relation = "Все сложно";
                    else if ("6".Equals(value))
                        personeAtrributes.Relation = "В активном поиске";
                    else if ("7".Equals(value))
                        personeAtrributes.Relation = "Влюблен(-а)";
                    else if ("8".Equals(value))
                        personeAtrributes.Relation = "Есть друг (подруга)";
                }
                catch (Exception)
                {
                }

                iStep = 3;
                //enableTimer(1);
                LoadUrl("https://vk.com/edit?act=contacts");
            }
            else if (iStep == 3)
            {
                try
                {
                    string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

                    if (source.Contains("extend(cur,"))
                    {
                        string pattern = "extend\\(cur,(.*)}\\);";
                        Regex regex = new Regex(pattern);
                        Match match = regex.Match(source);

                        if (match.Success)
                        {
                            if (match.Groups[1] != null && match.Groups[1].Length > 0)
                            {
                                string json = match.Groups[1] + "}";
                                dynamic stuff = JsonConvert.DeserializeObject(json);
                                string value;

                                value = stuff.selectData.country_val[1];
                                personeAtrributes.Country = value != null ? value : "";

                                value = stuff.selectData.city_val[1];
                                personeAtrributes.City = value != null ? value : personeAtrributes.City;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }

                iStep = 4;
                //enableTimer(1);
                LoadUrl("https://vk.com/id" + mContacterID.ToString());
            }
            else if (iStep == 4)
            {
                personeAtrributes.CountersFriends = await getBrowserFieldValueByClassName("count", "");
                personeAtrributes.Online = await getBrowserFieldValueByClassName("profile_online_lv", "");
                if (personeAtrributes.Online.StartsWith("Online"))
                    personeAtrributes.Online = "ON line";
                else
                    personeAtrributes.Online = "OFF line";

                autoclosedelay = 1;
            }
        }

        private async Task logoutTask()
        {
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

                if (source.Contains("top_logout_link"))
                {
                    setStatusMessage(statusText + "logout...");
                    iStep = 2;
                    logoutPersone();
                }
                else
                {
                    iStep = 2;
                    LoadUrl("http://vk.com/");
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task loginTask()
        {
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

                if (source.Contains("quick_login_form"))
                {
                    setStatusMessage(statusText + "login...");
                    iStep = 3;
                    loginPersone();
                    //var flag = NilsaWriteToRequestFile("ndocument.getElementById('quick_email').value=" + '\'' + mUserLogin + '\'' +
                    //"\ndocument.getElementById('quick_pass').value=" + '\'' + mUserPassword + '\'' + "\ndocument.getElementById('quick_login_form').submit()");
                    //while (flag != true)
                    //{
                    //    flag = NilsaWriteToRequestFile("ndocument.getElementById('quick_email').value=" + '\'' + mUserLogin + '\'' +
                    //"\ndocument.getElementById('quick_pass').value=" + '\'' + mUserPassword + '\'' + "\ndocument.getElementById('quick_login_form').submit()");
                    //}
                    //await BrowserReadFromRequestFile();
                    //NilsaReadFromResponseFile();
                }
                //else
                //{
                //    LoadUrl("http://vk.com/");
                //}
            }
            catch (Exception)
            {
            }
        }

        private async Task checkLoginTask()
        {
            disableTimer(timerAutorize);
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

                if (source.Contains("quick_login_form"))
                {
                    setStatusMessage(statusText + "ошибка...");
                    autoclosedelay = 1;
                }
                else if (source.Contains("top_profile_link"))
                {
                    if (browserAddress.Equals("https://vk.com/feed"))
                    {
                        enableTimer(5);
                        //await browser.EvaluateScriptAsync("document.getElementById('top_myprofile_link').href;").ContinueWith(x =>
                        //{
                        //    var response = x.Result;

                        //    long persID = -1;
                        //    if (response.Success && response.Result != null)
                        //    {
                        //        var startDate = response.Result;

                        //        if (startDate != null)
                        //        {
                        //            string href = startDate.ToString();
                        //            int idx = href.IndexOf("id");
                        //            if (idx >= 0)
                        //            {
                        //                href = href.Substring(idx + 2);
                        //                if (href.Length > 0)
                        //                {
                        //                    try
                        //                    {
                        //                        persID = Convert.ToInt64(href);
                        //                    }
                        //                    catch (Exception exp)
                        //                    {

                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //    loggedPersoneID = persID;

                        //    autoclosedelay = 1;
                        //});
                    }
                    else if (browserAddress.Contains("blocked"))
                    {
                        loggedPersoneID = -1;
                        autoclosedelay = 1;
                    }
                    else
                        enableTimer(timerAutorize);
                }
                else
                    enableTimer(timerAutorize);
            }
            catch (Exception)
            {
            }
        }

        class Peer
        {
            public long userid;
            public int unread;

            public Peer(long _userid, int _unread) { userid = _userid; unread = _unread; }
        };

        List<Peer> peers = new List<Peer>();
        Peer currentPeer = null;

        private async Task getPeers()
        {
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();
                if (source.Contains("nim-dialog _im_dialog _im_dialog_"))
                {

                    string pattern = "nim-dialog _im_dialog _im_dialog_([0-9]+) nim-dialog_unread.*?<div class=\"nim-dialog--unread _im_dialog_unread_ct\" aria-hidden=\"true\">([0-9]+)<";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(source);

                    while (match.Success)
                    {
                        if (match.Groups[1].Value != null && match.Groups[1].Value.Length > 0 && match.Groups[2].Value != null && match.Groups[2].Value.Length > 0)
                        {
                            string id = match.Groups[1].Value;
                            long _id = 0;
                            try
                            {
                                _id = Convert.ToInt64(id);
                            }
                            catch
                            {
                                _id = 0;
                            }

                            int _unread = 0;
                            try
                            {
                                _unread = Convert.ToInt32(match.Groups[2].Value);
                            }
                            catch
                            {
                                _unread = 0;
                            }

                            if (_id > 0 && _id < 2000000000 && _unread > 0)
                            {
                                if (source.Contains("nim-dialog _im_dialog _im_dialog_" + id + " nim-dialog_unread"))
                                {
                                    peers.Add(new Peer(_id, _unread));
                                    break;
                                }
                            }
                        }
                        // Переходим к следующему совпадению
                        match = match.NextMatch();
                    }
                }

                if (peers.Count == 0)
                {
                    iStep = -1;
                    setStatusMessage(statusText + "всё...");
                    autoclosedelay = 1;
                }
                else
                {
                    setStatusMessage(statusText + "переходим в диалог...");
                    //TryToClickElementByClassName("nim-dialog _im_dialog _im_dialog_" + peers[0].userid.ToString() + " nim-dialog_unread", 1, -1);
                    if (!TryToClickElementByClassName("nim-dialog _im_dialog _im_dialog_" + peers[0].userid.ToString() + " nim-dialog_unread"))
                    {
                        iStep = -1;
                        setStatusMessage(statusText + "ошибка...");
                        autoclosedelay = 1;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        //private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        //{
        //    this.InvokeOnUiThreadIfRequired(() => consoleStatusLabel.Text = string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        //}

        //private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        //{
        //    this.InvokeOnUiThreadIfRequired(() => statusStatusLabel.Text = args.Value);
        //}

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            browserAddress = args.Address;
            if (browserAddress == null)
                browserAddress = "";

            this.InvokeOnUiThreadIfRequired(() =>
            {
                addressStatusLabelTime.Text = DateTime.Now.ToString("HH:mm:ss:fff");
                urlTextBox.Text = browserAddress;
                addressStatusLabel.Text = browserAddress;
            });
        }

        public class Message
        {
            public bool inout;
            public long msgid;
            public long msgdate;
            public string msgtext;
            public long userid;

            public Message(long _userid, bool _inout, long _msgid, long _msgdate, string _msgtext) { userid = _userid; inout = _inout; msgid = _msgid; msgdate = _msgdate; msgtext = _msgtext; }
        };

        public List<Message> messages = new List<Message>();

        private async Task getMessages()
        {
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();
                List<Message> pagemessages = new List<Message>();

                if (source.Contains("im-mess _im_mess _im_mess_"))
                {

                    string pattern = "im-mess _im_mess _im_mess_.*?data-ts=\"([0-9]+)\".*?data-msgid=\"([0-9]+)\".*?im-mess--text wall_module _im_log_body\">(.*?)<\\/*div";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(source);

                    while (match.Success)
                    {
                        if (match.Groups[1].Value != null && match.Groups[1].Value.Length > 0 && match.Groups[2].Value != null && match.Groups[2].Value.Length > 0 && match.Groups[3].Value != null && match.Groups[3].Value.Length > 0)
                        {
                            long _date = 0;
                            try
                            {
                                _date = Convert.ToInt64(match.Groups[1].Value);
                            }
                            catch
                            {
                                _date = 0;
                            }

                            long _msgid = 0;
                            try
                            {
                                _msgid = Convert.ToInt64(match.Groups[2].Value);
                            }
                            catch
                            {
                                _msgid = 0;
                            }

                            string _msgtext = match.Groups[3].Value;

                            if (_date != 0 && _msgid != 0 && _msgtext != null && _msgtext.Length > 0)
                            {
                                pagemessages.Add(new Message(currentPeer.userid, true, _msgid, _date, _msgtext));
                            }
                        }
                        // Переходим к следующему совпадению
                        match = match.NextMatch();
                    }
                }

                if (pagemessages.Count > 0)
                {
                    int idx = pagemessages.Count - currentPeer.unread;
                    if (idx < 0)
                        idx = 0;

                    while (idx < pagemessages.Count)
                    {
                        messages.Add(pagemessages[idx]);
                        idx++;
                    }
                }

                setStatusMessage(statusText + "ищем диалог с новыми сообщениями...");

                autoclosedelay = autoclosedelaydefault;
                iStep = 0;
                LoadUrl("https://vk.com/im?tab=unread");
            }
            catch (Exception)
            {
            }
        }

        private void setStatusMessage(string text)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                this.Text = text;
                label1.Text = text;
            });
        }

        private void setButtonExitMessage(string text)
        {
            this.InvokeOnUiThreadIfRequired(() => buttonExit.Text = text);
        }

        private Task<object> getBrowserElementByID(String id)
        {
            return browser.EvaluateScriptAsync("document.getElementById('" + id + "')").ContinueWith(x =>
            {
                //callToBrowser += "document.getElementById('" + id + "')\n";
                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementById('" + id + "')\n", Encoding.UTF8);
                var response = x.Result;
                object retval = null;
                if (response.Success && response.Result != null)
                {
                    retval = response.Result;
                }
                return retval;
            });
        }

        private bool logoutPersone()
        {
            try
            {
                browser.EvaluateScriptAsync("document.getElementById('top_logout_link').click()");
                //EvaluateScriptAsync
                //callToBrowser += "document.getElementById('top_logout_link').click()\n";
                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementById('top_logout_link').click()\n", Encoding.UTF8);
                return true;
            }

            catch (Exception e)
            {

                File.AppendAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), e.ToString(), Encoding.UTF8);
            }
            return false;
        }

        private bool loginPersone()
        {
            try
            {
                browser.ExecuteScriptAsync("document.getElementById('quick_email').value=" + '\'' + mUserLogin + '\'');
                browser.ExecuteScriptAsync("document.getElementById('quick_pass').value=" + '\'' + mUserPassword + '\'');
                browser.ExecuteScriptAsync("document.getElementById('quick_login_form').submit()");
                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementById('quick_email').value=" + '\'' + mUserLogin + '\'' + 
                    "\ndocument.getElementById('quick_pass').value=" + '\'' + mUserPassword + '\'' + "\ndocument.getElementById('quick_login_form').submit()", Encoding.UTF8);
                //NilsaWriteToRequestFile("ndocument.getElementById('quick_email').value=" + '\'' + mUserLogin + '\'' +
                //    "\ndocument.getElementById('quick_pass').value=" + '\'' + mUserPassword + '\'' + "\ndocument.getElementById('quick_login_form').submit()");
                return true;
            }
            catch
            {

            }
            //catch (Exception e)
            //{

            //    File.AppendAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), e.ToString(), Encoding.UTF8);
            //}
            return false;

        }

        bool initBrowser = false;

        private void timerAutorize_Tick(object sender, EventArgs e)
        {
            disableTimer(timerAutorize);

            blogin = true;
            LoadUrl("http://vk.com/");
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            browserIsLoading = args.IsLoading;
            browserCanReload = args.CanReload;

            this.InvokeOnUiThreadIfRequired(() =>
            {
                loadingStatusLabel.Text = ((browserIsLoading && !browserCanReload) ? "Loading..." : "Loaded");
                loadingStatusLabelTime.Text = DateTime.Now.ToString("HH:mm:ss:fff");
            });

            if (!args.IsLoading)
            {
                if (!initBrowser)
                {
                    initBrowser = true;
                    if (noClose)
                    {
                        browser.SetZoomLevel(scaleBrowser);
                        this.InvokeOnUiThreadIfRequired(() => { trackBar1.Enabled = true; trackBar1.Value = scaleBrowser; });
                    }
                    else
                    {
                        browser.SetZoomLevel(scaleBrowser);
                        this.InvokeOnUiThreadIfRequired(() => { trackBar1.Enabled = true; trackBar1.Value = 0; });
                    }
                }
                else
                    browser.SetZoomLevel(scaleBrowser);
            }

            if (args.CanReload)
            {
                if (blogout)
                {
                    autoclosedelay = autoclosedelaydefault;
                    blogout = false;
                    if (!logoutPersone())
                        timerAutorize.Interval = 100;
                    enableTimer(timerAutorize);
                }
                else if (blogin || bStart)
                {
                    try
                    {
                        if (blogin)
                        {
                            //var c = "";
                            browser.ExecuteScriptAsync("document.getElementById('quick_email').value=" + '\'' + mUserLogin + '\'') ;
                            //File.AppendAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), "line1703\n" + c, Encoding.UTF8);

                            browser.EvaluateScriptAsync("document.getElementById('quick_pass').value=" + '\'' + mUserPassword + '\'').ContinueWith(x =>
                            {
                                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementById('quick_pass').value=" + '\'' + mUserPassword + '\'' + "\n", Encoding.UTF8);
                                var response = x.Result;
                                string retval = "";
                                if (response.Success && response.Result != null)
                                {
                                    var startDate = response.Result;
                                    retval = startDate.ToString();
                                    File.WriteAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), retval, Encoding.UTF8);
                                }
                            });

                            browser.EvaluateScriptAsync("document.getElementById('quick_login_form').submit()").ContinueWith(x =>
                            {
                                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementById('quick_login_form').submit()", Encoding.UTF8);
                                var response = x.Result;
                                string retval = "";
                                if (response.Success && response.Result != null)
                                {
                                    var startDate = response.Result;
                                    retval = startDate.ToString();
                                    File.WriteAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), retval, Encoding.UTF8);
                                }
                            });
                            File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementById('quick_email').value=" + '\'' + mUserLogin + '\'' +
                                "\ndocument.getElementById('quick_pass').value=" + '\'' + mUserPassword + '\'' + "\ndocument.getElementById('quick_login_form').submit()", Encoding.UTF8);
                        }

                        blogin = false;
                        bStart = false;
                        autoclosedelay = autoclosedelaydefault;
                        if (mContacterID >= 0 || mGroupsList.Length > 0 || mCommand == WebBrowserCommand.ShowFriendsPage || mCommand == WebBrowserCommand.Autorize/* || mCommand == WebBrowserCommand.GetPhotoURL*//* || mCommand == WebBrowserCommand.GetPersoneAttributes*//* || mCommand == WebBrowserCommand.GetContactAttributes*/ || mCommand == WebBrowserCommand.LoginPersone || mCommand == WebBrowserCommand.GoToPersonePage/* || mCommand == WebBrowserCommand.ReadMessages*/)
                            enableTimer(0);
                    }
                    catch (Exception e)
                    {
                        File.AppendAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), e.ToString(), Encoding.UTF8);
                        autoclosedelay = autoclosedelaydefault;
                        blogin = true;
                    }

                }
            }

            if (!args.IsLoading && args.CanReload)
            {
                if (isContitionEnabled())
                    enableTimer(timerCondition);
            }
        }

        private bool isContitionEnabled()
        {
            return (iStep >= 0 && (mCommand == WebBrowserCommand.ReadMessages || mCommand == WebBrowserCommand.ReadHistory/* || mCommand == WebBrowserCommand.LoginPersone*/ || mCommand == WebBrowserCommand.GetPersoneAttributes || mCommand == WebBrowserCommand.GetPersoneFriendsCount || mCommand == WebBrowserCommand.GetContactAttributes || mCommand == WebBrowserCommand.GetPhotoURL));
        }

        private string browserAddress = "";
        private bool browserIsLoading = false;
        private bool browserCanReload = false;
        private string browserTitle = "";

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            browserTitle = args.Title;

            this.InvokeOnUiThreadIfRequired(() =>
            {
                groupBox1.Text = personeName != null ? (personeName + (personeName.Length > 0 ? ": " : "")) : "" + browserTitle;
                titleStatusLabel.Text = browserTitle;
                titleStatusLabelTime.Text = DateTime.Now.ToString("HH:mm:ss:fff");
            });

            /*
            if (args.Title.Equals("Проверка безопасности"))
            {
                browser.ExecuteScriptAsync("document.getElementById('code').value=" + '\'' + mUserLogin.Substring(2, 8) + '\'');
                browser.ExecuteScriptAsync("document.getElementById('validate_btn').click()");

                if (mContacterID >= 0 || mGroupsList.Length > 0 || mCommand == WebBrowserCommand.ShowFriendsPage || mCommand == WebBrowserCommand.Autorize || mCommand == WebBrowserCommand.GetPhotoURL || mCommand == WebBrowserCommand.GetPersoneAttributes || mCommand == WebBrowserCommand.GetContactAttributes || mCommand == WebBrowserCommand.LoginPersone || mCommand == WebBrowserCommand.GoToPersonePage || mCommand == WebBrowserCommand.ReadMessages)
                    enableTimer(0);
            }
        */

        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            if (noClose && sender != null)
                return;

            disableTimer(timerAutoClose);

            if (mCommand == WebBrowserCommand.Autorize || mCommand == WebBrowserCommand.CheckPersonePage)
            {
                logoutPersone();
                enableTimer(timerDisconnect);
            }
            else
            {
                setStatusMessage("");
                if (noClose)
                {
                    setButtonExitMessage("Ожидание...");
                    setStatusMessage("Ожидание...");
                }
                mCommand = WebBrowserCommand.None;
                iStep = -1;
                disableTimer(timerCondition);

                DialogResult = DialogResult.OK;
                processEnd = true;
                //TopMost = false;
            }
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            browser.Reload(true);
        }

        private void timer0_Tick(object sender, EventArgs e)
        {
            workTimersCycle[0]--;
            if (workTimersCycle[0] > 0)
                return;

            disableTimer(0);

            if (mCommand == WebBrowserCommand.ShowFriendsPage)
            {
                LoadUrl("https://vk.com/friends");
            }
            else if (mCommand == WebBrowserCommand.Autorize || mCommand == WebBrowserCommand.GoToPersonePage)
            {
                LoadUrl("http://vk.com/id" + mContacterID.ToString());
            }
            else if (mCommand == WebBrowserCommand.LoginPersone)
            {
                enableTimer(5);
            }
            //else if (mCommand == WebBrowserCommand.GetPhotoURL)
            //{
            //    LoadUrl("http://vk.com/id" + mContacterID.ToString());
            //    enableTimer(5);
            //}
            //else if (mCommand == WebBrowserCommand.GetPersoneAttributes)
            //{
            //    setStatusMessage(statusText + "читаем атрибуты Персонажа...");
            //    autoclosedelay = autoclosedelaydefault;
            //    TryToClickElementByID("top_edit_link", 5, -1);
            //}
            //else if (mCommand == WebBrowserCommand.GetContactAttributes)
            //{
            //    setStatusMessage(statusText + "читаем атрибуты Контакта...");
            //    autoclosedelay = autoclosedelaydefault;
            //    LoadUrl("http://vk.com/id" + mContacterID.ToString());
            //    enableTimer(5);
            //}
            //else if (mCommand == WebBrowserCommand.ReadMessages)
            //{
            //    setStatusMessage(statusText + "ищем беседы c новыми сообщениями...");
            //    LoadUrl("https://vk.com/im?tab=unread");
            //    enableTimer(5);
            //}
            //else if (mCommand == WebBrowserCommand.ReadHistory)
            //{
            //    if (mContacterID >= 0)
            //    {
            //        setStatusMessage(statusText + "ищем диалог...");
            //        LoadUrl("https://vk.com/im?sel=" + mContacterID.ToString());
            //        enableTimer(5);
            //    }
            //    else
            //        autoclosedelay = 2;
            //}
            else
            {
                if (mContacterID >= 0)
                {
                    bcontacternavigate = true;
                    LoadUrl("http://vk.com/id" + mContacterID.ToString());
                    enableTimer(5);
                }
                else if (mGroupsList.Length > 0)
                {
                    if (mCommand == WebBrowserCommand.JoinCommunity)
                    {
                        LoadUrl("https://vk.com/groups");
                    }
                    else
                    {
                        LoadUrl("http://vk.com/" + mGroupsList);
                    }
                    enableTimer(5);
                }
            }
        }

        //private async Task sendMessageTask()
        //{
        //    try
        //    {
        //        string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

        //        if (source.Contains("mail_box_editable"))
        //        {
        //            setStatusMessage(statusText + "посылаем...");
        //            browser.ExecuteScriptAsync("document.getElementById('mail_box_editable').innerHTML=" + '\'' + mMessageToSend + '\'');
        //            File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line1888\ndocument.getElementById('mail_box_editable').innerHTML=" + '\'' + mMessageToSend + '\'', Encoding.UTF8);
        //            enableTimer(2);

        //        }
        //        else
        //        {
        //            setStatusMessage(statusText + "ошибка...");

        //            autoclosedelay = 2;
        //            LoadUrl("https://vk.com/im");
        //        }

        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        private async Task sendMessageTask()
        {
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

                if (source.Contains("im_editable0"))
                {
                    setStatusMessage(statusText + "посылаем...");

                    // Write the request to file
                    //File.WriteAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementById('im_editable0').innerHTML=" + '\'' + mMessageToSend + '\'' + "\ndocument.getElementsByClassName('im-send-btn im-chat-input--send _im_send im-send-btn_send')[0].click()", Encoding.UTF8);
                    //Test message line
                    //File.WriteAllText(Path.Combine(Application.StartupPath, "_request_to_browser.txt"), "document.getElementById('im_editable0').innerHTML=" + '\'' + "Привет, мир" + '\'' + "\ndocument.getElementsByClassName('im-send-btn im-chat-input--send _im_send im-send-btn_audio')[0].click()", Encoding.UTF8);
                    //var flag = NilsaWriteToRequestFile("document.getElementById('im_editable0').innerHTML=" + '\'' + "Привет, мир" + '\'' + "\ndocument.getElementsByClassName('im-send-btn im-chat-input--send _im_send im-send-btn_audio')[0].click()");
                    //while (flag!=true)
                    //{
                    //    flag = NilsaWriteToRequestFile("document.getElementById('im_editable0').innerHTML=" + '\'' + "Привет, мир" + '\'' + "\ndocument.getElementsByClassName('im-send-btn im-chat-input--send _im_send im-send-btn_audio')[0].click()");
                    //}
                    //await BrowserReadFromRequestFile();
                    enableTimer(2);
                    //NilsaReadFromResponseFile();
                }
                else
                {
                    setStatusMessage(statusText + "ошибка...");

                    autoclosedelay = 2;
                    LoadUrl("https://vk.com/im");
                }
            }
            catch (Exception)
            {
            }
        }

        //private bool NilsaWriteToRequestFile(string request)
        //{
        //    try
        //    {
        //        string requestPath = Path.Combine(Application.StartupPath, "_request_to_browser.txt");

        //        // Write the request to file

        //        File.AppendAllText(requestPath, request, Encoding.UTF8);
        //        return true;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //private void NilsaReadFromResponseFile()
        //{
        //    try
        //    {
        //        string responsePath = Path.Combine(Application.StartupPath, "_response_from_browser.txt");

        //        // Read the request from file

        //        using (StreamReader reader = new StreamReader(responsePath))
        //        {
        //            string line;
        //            int lineCount = 0;

        //            // Skip the lines that have already been read
        //            while (lineCount < _responseFromBrowserReadLines && (line = reader.ReadLine()) != null)
        //            {
        //                lineCount++;
        //            }

        //            // Read the new lines and process them
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                lineCount++;
        //            }

        //            // Update the number of lines read
        //            _responseFromBrowserReadLines = lineCount;
        //            _actualResponse = line;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //private async Task BrowserReadFromRequestFile()
        //{
        //    try
        //    {
        //        string requestPath = Path.Combine(Application.StartupPath, "_request_to_browser.txt");
        //        string responsePath = Path.Combine(Application.StartupPath, "_response_from_browser.txt");
        //        JavascriptResponse response = null;

        //        // Read the request from file

        //        using (StreamReader reader = new StreamReader(requestPath))
        //        {
        //            string line;
        //            int lineCount = 0;

        //            // Skip the lines that have already been read
        //            while (lineCount < _requestToBrowserReadLines && (line = reader.ReadLine()) != null)
        //            {
        //                lineCount++;
        //            }

        //            // Read the new lines and process them
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                // Process the new line
        //                response = await browser.EvaluateScriptAsync(line);

        //                lineCount++;
        //            }

        //            // Update the number of lines read
        //            _requestToBrowserReadLines = lineCount;
        //            File.WriteAllText(Path.Combine(Application.StartupPath, "_requset_to_browser_Read_Lines.txt"), _requestToBrowserReadLines.ToString(), Encoding.UTF8);
        //        }
        //        //string request = File.ReadAllText(requestPath, Encoding.UTF8);

        //        // Execute the request in the browser
        //        //var response = await browser.EvaluateScriptAsync(request);

        //        // Write the response to file
        //        File.AppendAllText(responsePath, response.Result.ToString(), Encoding.UTF8);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}


        //private async Task addToFriendsSendMessageTaskEnd()
        //{
        //    try
        //    {
        //        string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

        //        if (source.Contains("mail_box_editable"))
        //        {
        //            setStatusMessage(statusText + "посылаем...");
        //            browser.ExecuteScriptAsync("document.getElementById('preq_input').value=" + '\'' + mMessageToSend + '\'');
        //            enableTimer(2);

        //        }
        //        else
        //        {
        //            setStatusMessage(statusText + "ошибка...");

        //            autoclosedelay = 2;
        //            LoadUrl("https://vk.com/im");
        //        }

        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        private async Task addToFriendsSendMessageTask()
        {
            if (iStep == 1)
            {
                try
                {
                    string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

                    if (source.Contains("Заявка отправлена"))
                    {
                        setStatusMessage(statusText + "меню...");
                        browser.ExecuteScriptAsync("document.getElementsByClassName('flat_button button_wide secondary page_actions_btn')[0].click()");
                        File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementsByClassName('flat_button button_wide secondary page_actions_btn')[0].click()", Encoding.UTF8);
                        iStep = 2;
                        enableTimer(1);
                    }
                    else
                    {
                        setStatusMessage(statusText + "всё...");
                        errorSendMessage = false;
                        autoclosedelay = 2;
                    }

                }
                catch (Exception)
                {
                }
            }
            else if (iStep == 2)
            {
                try
                {
                    setStatusMessage(statusText + "новое сообщение...");
                    browser.ExecuteScriptAsync("document.getElementsByClassName('page_actions_item')[1].click()");
                    File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementsByClassName('page_actions_item')[1].click()", Encoding.UTF8);

                    iStep = 3;
                    enableTimer(1);
                }
                catch (Exception)
                {
                }
            }
            else if (iStep == 3)
            {
                try
                {
                    setStatusMessage(statusText + "вводим текст...");
                    browser.ExecuteScriptAsync("document.getElementById('preq_input').value=" + '\'' + mMessageToSend + '\'');
                    File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementById('preq_input').value=" + '\'' + mMessageToSend + '\'', Encoding.UTF8);

                    enableTimer(2);

                }
                catch (Exception)
                {
                }
            }
        }

        private async Task addToFriendsTask()
        {
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

                if (source.Contains("Добавить в друзья"))
                {
                    setStatusMessage(statusText + "добавляем...");
                    browser.ExecuteScriptAsync("document.getElementsByClassName('flat_button button_wide')[0].click()");
                    File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2003\ndocument.getElementsByClassName('flat_button button_wide')[0].click()", Encoding.UTF8);
                    enableTimer(1);

                }
                else
                {
                    errorSendMessage = false;
                    setStatusMessage(statusText + "всё...");
                    autoclosedelay = 2;
                }

            }
            catch (Exception)
            {
            }
        }

        private Task<string> getBrowserFieldValueByID(String id, String defval)
        {
            return browser.EvaluateScriptAsync("document.getElementById('" + id + "').value;").ContinueWith(x =>
                 {
                     //callToBrowser += "document.getElementById('" + id + "').value;\n";
                     File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2025\ndocument.getElementById('" + id + "').value;\n", Encoding.UTF8);
                     var response = x.Result;
                     string retval = defval;
                     if (response.Success && response.Result != null)
                     {
                         var startDate = response.Result;
                         retval = startDate.ToString();
                         File.WriteAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), retval, Encoding.UTF8);
                     }
                     return retval;
                 });
        }

        private Task<string> getBrowserFieldValueByClassName(String id, String defval)
        {
            return browser.EvaluateScriptAsync("document.getElementsByClassName('" + id + "')[0].innerText;").ContinueWith(x =>
            {
                //callToBrowser += "document.getElementsByClassName('" + id + "')[0].innerText;\n";
                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2043\ndocument.getElementsByClassName('" + id + "')[0].innerText;\n", Encoding.UTF8);
                var response = x.Result;
                string retval = defval;
                if (response.Success && response.Result != null)
                {
                    var startDate = response.Result;
                    retval = startDate.ToString();
                    File.WriteAllText(Path.Combine(Application.StartupPath, "_answer_from_browser.txt"), x.ToString(), Encoding.UTF8);
                }
                return retval;
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            workTimersCycle[1]--;
            if (workTimersCycle[1] > 0)
                return;

            disableTimer(1);

            //if (mCommand == WebBrowserCommand.ReadMessages)
            //{
            //    if (peers.Count > 0)
            //    {
            //        setStatusMessage(statusText + "читаем новые сообщения из беседы...");

            //        autoclosedelay = autoclosedelaydefault;
            //        currentPeer = peers[0];
            //        peers.RemoveAt(0);
            //        enableTimer(2);
            //    }
            //    else
            //    {
            //        setStatusMessage(statusText + "всё...");
            //        autoclosedelay = 2;
            //    }
            //}
            //else 
            //if (mCommand == WebBrowserCommand.GetPersoneAttributes)
            //{
            //    autoclosedelay = autoclosedelaydefault;
            //    Task ts = getPersoneAttributesTask();
            //}
            //else 
            //if (mCommand == WebBrowserCommand.GetContactAttributes)
            //{
            //    autoclosedelay = autoclosedelaydefault;
            //    Task ts = getContactAttributesTask();
            //}
            //else 
            if (mCommand == WebBrowserCommand.SendMessage)
            {
                if (mMessageToSend.Length > 0)
                {
                    autoclosedelay = autoclosedelaydefault;
                    Task ts = sendMessageTask();
                }
                else
                {
                    setStatusMessage(statusText + "всё...");
                    autoclosedelay = 3;
                }
            }
            else if (mCommand == WebBrowserCommand.AddToFriends)
            {
                if (mMessageToSend.Length > 0)
                {
                    autoclosedelay = autoclosedelaydefault;
                    Task ts = addToFriendsSendMessageTask();
                }
                else
                {
                    errorSendMessage = false;
                    setStatusMessage(statusText + "всё...");
                    autoclosedelay = 2;
                }
            }
            else if (mCommand == WebBrowserCommand.LoginPersone)
            {
                setStatusMessage(statusText + "проверяю...");
                Task ts = checkPersoneLoginTaskPhoto();
            }
            else
                TryToClickElementByID("pv_like_wrap", 2, 1);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            workTimersCycle[2]--;
            if (workTimersCycle[2] > 0)
                return;

            disableTimer(2);

            //if (mCommand == WebBrowserCommand.ReadMessages)
            //{
            //    if (currentPeer != null)
            //    {
            //        setStatusMessage(statusText + "сохраняем новые сообщения из беседы...");

            //        autoclosedelay = autoclosedelaydefault;
            //        Task ts = getMessages();
            //    }
            //    else
            //    {
            //        setStatusMessage(statusText + "всё...");
            //        autoclosedelay = 2;
            //    }
            //}
            //else 
            if (mCommand == WebBrowserCommand.SendMessage)
            {
                setStatusMessage(statusText + "послали...");
                browser.ExecuteScriptAsync("document.getElementById('mail_box_send').click()");
                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2158\ndocument.getElementById('mail_box_send').click()", Encoding.UTF8);
                errorSendMessage = false;
                autoclosedelay = 4;
            }
            else if (mCommand == WebBrowserCommand.AddToFriends)
            {
                setStatusMessage(statusText + "послали...");
                browser.ExecuteScriptAsync("document.getElementById('preq_submit').click()");
                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2166\ndocument.getElementById('preq_submit').click()", Encoding.UTF8);
                errorSendMessage = false;
                autoclosedelay = 4;
            }
            else if (TryToClickElementByID("pv_photo", -1, 2))
                autoclosedelay = 3;
        }

        private void timerAutoClose_Tick(object sender, EventArgs e)
        {
            autoclosedelay--;
            setButtonExitMessage(btnCloseText + " " + autoclosedelay.ToString() + " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "сек."));
            if (autoclosedelay <= 0)
            {
                buttonExit_Click(null, null);
            }
        }

        private void enableTimer(System.Windows.Forms.Timer timer)
        {
            this.InvokeOnUiThreadIfRequired(() => { timer.Enabled = true; });
        }

        private void disableTimer(System.Windows.Forms.Timer timer)
        {
            this.InvokeOnUiThreadIfRequired(() => { timer.Enabled = false; });
        }

        private void enableTimer(int timerIdx)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                workTimersCycle[timerIdx] = workTimersInterval[timerIdx];
                workTimers[timerIdx].Enabled = true;
            });
        }

        private void disableTimer(int timerIdx)
        {
            this.InvokeOnUiThreadIfRequired(() => { workTimers[timerIdx].Enabled = false; });
        }

        private bool TryToClickElementByID(string elementID, int timerNextIdx, int timerPrevousIdx)
        {
            bool retval = false;
            try
            {
                browser.ExecuteScriptAsync("document.getElementById('" + elementID + "').click()");
                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2214\ndocument.getElementById('" + elementID + "').click()", Encoding.UTF8);
                if (timerNextIdx >= 0)
                    enableTimer(timerNextIdx);
                retval = true;
            }
            catch
            {
                if (timerPrevousIdx >= 0)
                    enableTimer(timerPrevousIdx);
            }
            return retval;
        }

        private bool TryToClickElementByClassName(string elementID, int timerNextIdx = -1, int timerPrevousIdx = -1)
        {
            bool retval = false;
            try
            {
                browser.ExecuteScriptAsync("document.getElementsByClassName('" + elementID + "')[0].click()");
                //callToBrowser += "document.getElementsByClassName('" + elementID + "')[0].click()\n";
                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2234\ndocument.getElementsByClassName('" + elementID + "')[0].click()\n", Encoding.UTF8);

                if (timerNextIdx >= 0)
                    enableTimer(timerNextIdx);
                retval = true;
            }
            catch
            {
                if (timerPrevousIdx >= 0)
                    enableTimer(timerPrevousIdx);
            }
            return retval;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            workTimersCycle[3]--;
            if (workTimersCycle[3] > 0)
                return;

            disableTimer(3);

            try
            {
                browser.ExecuteScriptAsync("document.getElementById('like_share_send').click()");
                File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2259\ndocument.getElementById('like_share_send').click()", Encoding.UTF8);
                enableTimer(4);
            }
            catch
            {
                enableTimer(3);
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            workTimersCycle[4]--;
            if (workTimersCycle[4] > 0)
                return;

            disableTimer(4);

            if (mPostsToRepost.IndexOf(',') > 0)
            {
                string post = mPostsToRepost.Substring(0, mPostsToRepost.IndexOf(',')).Trim();
                mPostsToRepost = mPostsToRepost.Substring(mPostsToRepost.IndexOf(',') + 1);

                int idxpost = -1;
                try
                {
                    idxpost = Convert.ToInt32(post);
                }
                catch
                {
                    idxpost = -1;
                }

                autoclosedelay += dTimerDalay;
                if (idxpost >= 0)
                {
                    if (idxpost - 1 < lstWallPosts.Count)
                    {

                        lstWallPosts[idxpost - 1].InvokeMember("click");
                        enableTimer(3);
                    }
                    else
                        enableTimer(4);
                }
                else
                    enableTimer(4);
            }
            else
                autoclosedelay = 1;
        }

        private async Task getHistory()
        {
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();
                List<Message> pagemessages = new List<Message>();

                if (source.Contains("im-mess _im_mess")) //
                {

                    string pattern = "im-mess _im_mess.*?data-ts=\"([0-9]+)\".*?data-msgid=\"([0-9]+)\".*?im-mess--text wall_module _im_log_body\">(.*?)<\\/*div";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(source);

                    while (match.Success)
                    {
                        if (match.Groups[1].Value != null && match.Groups[1].Value.Length > 0 && match.Groups[2].Value != null && match.Groups[2].Value.Length > 0 && match.Groups[3].Value != null && match.Groups[3].Value.Length > 0 && match.Groups[4].Value != null && match.Groups[4].Value.Length > 0)
                        {
                            bool _inout = "in".Equals(match.Groups[1].Value);

                            long _date = 0;
                            try
                            {
                                _date = Convert.ToInt64(match.Groups[2].Value);
                            }
                            catch
                            {
                                _date = 0;
                            }

                            long _msgid = 0;
                            try
                            {
                                _msgid = Convert.ToInt64(match.Groups[3].Value);
                            }
                            catch
                            {
                                _msgid = 0;
                            }

                            string _msgtext = match.Groups[4].Value;

                            if (_date != 0 && _msgid != 0 && _msgtext != null && _msgtext.Length > 0)
                            {
                                pagemessages.Add(new Message(currentPeer.userid, _inout, _msgid, _date, _msgtext));
                            }
                        }
                        // Переходим к следующему совпадению
                        match = match.NextMatch();
                    }
                }

                if (pagemessages.Count > 0)
                {
                    int idx = pagemessages.Count - currentPeer.unread;
                    if (idx < 0)
                        idx = 0;

                    while (idx < pagemessages.Count)
                    {
                        messages.Add(pagemessages[idx]);
                        idx++;
                    }
                }

                setStatusMessage(statusText + "уходим из ветки...");
                LoadUrl("https://vk.com/im");

            }
            catch (Exception)
            {
            }
        }

        private async Task checkPersoneLoginTaskPhoto()
        {
            bool done = false;
            try
            {
                //
                //await browser.EvaluateScriptAsync("document.getElementById('profile_photo_link').href;").ContinueWith(x =>
                await browser.EvaluateScriptAsync("document.getElementById('profile_photo_link').href;").ContinueWith(x =>
                {
                    var response = x.Result;
                    //callToBrowser += "document.getElementById('profile_photo_link').href;\n";
                    File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2395\ndocument.getElementById('profile_photo_link').href;\n", Encoding.UTF8);


                    long persID = -1;
                    if (response.Success && response.Result != null)
                    {
                        var startDate = response.Result;

                        if (startDate != null)
                        {
                            string href = startDate.ToString();
                            int idx = href.IndexOf("photo");
                            if (idx >= 0)
                            {
                                href = href.Substring(idx + 5);
                                if (href.Length > 0)
                                {
                                    idx = href.IndexOf("_");
                                    if (idx > 0)
                                        href = href.Substring(0, idx);
                                }
                                if (href.Length > 0)
                                {
                                    try
                                    {
                                        persID = Convert.ToInt64(href);
                                        done = false;
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                            }
                        }
                    }
                    loggedPersoneID = persID;

                    autoclosedelay = 1;
                });
            }
            catch (Exception)
            {
                done = true;
            }

            if (done)
            {
                setStatusMessage(statusText + "ошибка...");
                autoclosedelay = 1;
            }
        }

        private async Task checkPersoneLoginTask()
        {
            bool done = false;
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();

                if (source.Contains("quick_login_form"))
                {
                    setStatusMessage(statusText + "неверный логин или пароль...");
                    autoclosedelay = 1;
                }
                else if (source.Contains("top_profile_link")) // if (source.Contains("top_myprofile_link"))
                {
                    if (browserAddress.Equals("https://vk.com/feed"))
                    {
                        //await browser.EvaluateScriptAsync("document.getElementById('top_myprofile_link').href;").ContinueWith(x =>
                        await browser.EvaluateScriptAsync("document.getElementById('top_profile_link').href;").ContinueWith(x =>
                        {
                            var response = x.Result;
                            //callToBrowser += "document.getElementById('top_profile_link').href;\n";
                            File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "line2469\ndocument.getElementById('top_profile_link').href;\n", Encoding.UTF8);


                            long persID = -1;
                            string address = "";
                            if (response.Success && response.Result != null)
                            {
                                var startDate = response.Result;

                                if (startDate != null)
                                {
                                    string href = startDate.ToString();
                                    address = href;
                                    int idx = href.IndexOf("id");
                                    if (idx >= 0)
                                    {
                                        href = href.Substring(idx + 2);
                                        if (href.Length > 0)
                                        {
                                            try
                                            {
                                                persID = Convert.ToInt64(href);
                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                            loggedPersoneID = persID;

                            if (loggedPersoneID != -1)
                                autoclosedelay = 1;
                            else
                            {
                                if (address.Length > 0)
                                {
                                    LoadUrl(address);
                                    enableTimer(1);
                                }
                            }
                        });
                    }
                    else if (browserAddress.Contains("blocked"))
                    {
                        setStatusMessage(statusText + "блокирован...");
                        loggedPersoneID = -1;
                        autoclosedelay = 1;
                    }
                }
            }
            catch (Exception)
            {
                done = true;
            }

            if (done)
            {
                setStatusMessage(statusText + "ошибка...");
                autoclosedelay = 1;
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            workTimersCycle[5]--;
            if (workTimersCycle[5] > 0)
                return;

            disableTimer(5);

            //if (mCommand == WebBrowserCommand.ReadMessages)
            //{
            //    setStatusMessage(statusText + "обрабатываем беседы...");
            //    autoclosedelay = autoclosedelaydefault;
            //    Task ts = getPeers();
            //}
            //else 
            //if (mCommand == WebBrowserCommand.ReadHistory)
            //{
            //    setStatusMessage(statusText + "читаем историю...");
            //    autoclosedelay = autoclosedelaydefault;
            //    Task ts = getHistory();
            //}

            //else if (mCommand == WebBrowserCommand.GetPhotoURL)
            //{
            //    browser.EvaluateScriptAsync("document.getElementsByClassName('page_avatar_img')[0].src;").ContinueWith(x =>
            //    {
            //        var response = x.Result;

            //        if (response.Success && response.Result != null)
            //        {
            //            var startDate = response.Result;

            //            if (startDate != null)
            //            {
            //                photoURL = startDate.ToString();
            //            }
            //        }

            //        autoclosedelay = 1;
            //    });
            //}
            //else if (mCommand == WebBrowserCommand.GetPersoneAttributes)
            //{
            //    autoclosedelay = autoclosedelaydefault;
            //    Task ts = getPersoneAttributesTask();
            //}
            //else if (mCommand == WebBrowserCommand.GetContactAttributes)
            //{
            //    autoclosedelay = autoclosedelaydefault;
            //    TryToClickElementByClassName("button_link profile_more_info_link", 1, 1);
            //}
            //else 
            if (mCommand == WebBrowserCommand.SendMessage)
            {
                if (mContacterID >= 0 && mMessageToSend.Length > 0)
                {
                    setStatusMessage(statusText + "вводим текст...");
                    autoclosedelay = autoclosedelaydefault;
                    TryToClickElementByClassName("button_link cut_left", 1, -1);
                }
                else
                {
                    setStatusMessage(statusText + "ошибка...");
                    autoclosedelay = 3;
                }

            }
            else if (mCommand == WebBrowserCommand.AddToFriends)
            {
                if (mContacterID >= 0)
                {
                    autoclosedelay = autoclosedelaydefault;
                    iStep = 1;
                    Task ts = addToFriendsTask();
                }
                else
                {
                    setStatusMessage(statusText + "ошибка...");
                    autoclosedelay = 3;
                }
            }
            else if (mCommand == WebBrowserCommand.LoginPersone)
            {
                setStatusMessage(statusText + "проверяю...");
                Task ts = checkPersoneLoginTask();
                //browser.EvaluateScriptAsync("document.getElementById('top_myprofile_link').href;").ContinueWith(x =>
                //{
                //    var response = x.Result;

                //    long persID = -1;
                //    if (response.Success && response.Result != null)
                //    {
                //        var startDate = response.Result;

                //        if (startDate != null)
                //        {
                //            string href = startDate.ToString();
                //            int idx = href.IndexOf("id");
                //            if (idx >= 0)
                //            {
                //                href = href.Substring(idx + 2);
                //                if (href.Length > 0)
                //                {
                //                    try
                //                    {
                //                        persID = Convert.ToInt64(href);
                //                    }
                //                    catch (Exception exp)
                //                    {

                //                    }
                //                }
                //            }
                //        }
                //    }
                //    loggedPersoneID = persID;

                //    autoclosedelay = 1;
                //});
            }
            else if (mContacterID >= 0)
            {
                if (bcontacternavigate)
                {
                    if (mRepostPosts)
                    {
                        if (bcontacterrepostinit)
                        {
                            bcontacterrepostinit = false;
                            if (mPostsToRepost.Length > 0)
                            {
                                autoclosedelay = 2;
                            }

                        }
                    }
                    else
                    {
                        autoclosedelay = autoclosedelaydefault;
                        TryToClickElementByID("profile_photo_link", 1, -1);
                    }
                }
            }
            else if (mGroupsList.Length > 0)
            {
                if (mRepostPosts)
                {
                    if (bcontacterrepostinit)
                    {
                        bcontacterrepostinit = false;
                        if (mPostsToRepost.Length > 0)
                        {
                            autoclosedelay = 2;
                        }

                    }
                }
                else
                {
                    autoclosedelay = 2;
                }
            }
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                browser.Load(url);
            }
        }

        private void FormWebBrowser_FormClosed(object sender, FormClosedEventArgs e)
        {
            browser.Dispose();
        }

        private void timerDisconnect_Tick(object sender, EventArgs e)
        {
            disableTimer(timerDisconnect);
            disableTimer(timerAutoClose);

            setStatusMessage("");
            if (noClose)
            {
                setButtonExitMessage("Ожидание...");
                setStatusMessage("Ожидание...");
            }

            mCommand = WebBrowserCommand.None;
            iStep = -1;
            disableTimer(timerCondition);

            DialogResult = DialogResult.OK;
            processEnd = true;
            //TopMost = false;
        }

        private void tsbGetMusicLink_Click(object sender, EventArgs e)
        {
            Task ts = getSource();
        }

        private async Task getSource()
        {
            try
            {
                string source = await browser.GetBrowser().MainFrame.GetSourceAsync();
                string result = "";
                if (source.Contains("audio_row audio_row_with_cover _audio_row"))
                {

                    string pattern = "audio_row audio_row_with_cover _audio_row?.*data-full-id=\"([_0-9\\-]+)\"?.*data-audio=\"\\[[0-9,-]+,&quot;&quot;,&quot;(.*?&quot;,&quot;)";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(source);

                    while (match.Success)
                    {
                        if (match.Groups[1].Value != null && match.Groups[1].Value.Length > 0 && match.Groups[2].Value != null && match.Groups[2].Value.Length > 0)
                            result += "audio" + match.Groups[1].Value + " - " + (match.Groups[2].Value.Contains("&quot;,&quot;") ? match.Groups[2].Value.Substring(0, match.Groups[2].Value.IndexOf("&quot;,&quot;")) : match.Groups[2].Value) + "\r\n";

                        match = match.NextMatch();
                    }
                }
                else
                    result = "Ссылки на аудио-файлы не найдены";

                string f = Application.StartupPath + "\\audio_links.txt";
                StreamWriter wr = new StreamWriter(f, false, System.Text.Encoding.UTF8);
                wr.Write(result);
                wr.Close();
                System.Diagnostics.Process.Start(f);

            }
            catch (Exception)
            {
            }
        }

        private void urlTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }

        private void tsbGo_Click(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void FormWebBrowser_Shown(object sender, EventArgs e)
        {
            formLoading = true;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (browser.IsBrowserInitialized)
            {
                scaleBrowser = trackBar1.Value;
                browser.SetZoomLevel(scaleBrowser);
                saveSettingsAfterChange();
            }
        }

        public bool exitBrowser = false;
        private void FormWebBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (noClose)
                e.Cancel = !exitBrowser;
        }

        private void timerCondition_Tick(object sender, EventArgs e)
        {
            disableTimer(timerCondition);
            doCondition();
        }

        private void saveSettingsAfterChange()
        {
            try
            {
                if (noClose && formLoading)
                    saveSettings();
            }
            catch
            {

            }
        }

        private void FormWebBrowser_LocationChanged(object sender, EventArgs e)
        {
            saveSettingsAfterChange();
        }

        private void FormWebBrowser_ResizeEnd(object sender, EventArgs e)
        {
            saveSettingsAfterChange();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            //await sendMessageTask();
        }

        private async void toolStripButton2_Click(object sender, EventArgs e)
        {
            //await BrowserReadFromRequestFile();
        }
    }
}
