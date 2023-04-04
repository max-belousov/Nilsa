using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;



namespace Nilsa
{
    internal class VkInterfaceCommands
    {
        private FormMain mFormMain;
        private ResponseFromInterface mResponseFromInterface;
        private string mUserLogin;
        private string mUserPassword;
        private uint operatingMode = NilsaOperatingMode.None;
        private uint mCommand = WebBrowserCommand.None;
        private long mContacterID = -1;
        private string mPostsToRepost = "";
        private string mGroupsList = "";
        private string mMessageToSend = "";
        private string statusText = "";
        private bool blogin = false;
        private bool blogout = false;
        private bool bcontacternavigate = false;
        private bool mRepostPosts = false;
        private bool bcontacterrepostinit = true;
        //private readonly ChromiumWebBrowser browser;
        private int autoclosedelay = 15;
        private int autoclosedelaydefault = 15;
        private int dTimerDalay = 1;
        private bool formLoading = false;
        private string personeName;
        private bool errorSendMessage = false;
        private long loggedPersoneID = -1;
        private string photoURL = "";
        private string lastResponseTime;
        //public Persone personeAtrributes;
        //public Persone contactAtrributes;
        //public Persone personeAtrributesFriends;
        private int iStep = -1;
        private bool bStart = false;
        private string callToBrowser = "";


        public VkInterfaceCommands(FormMain formMain)
        {
            mFormMain = formMain;
        }

        public string Setup(String sLogin, String sPassword, uint _iCommand, uint _iOperatingMode, long _contacterID = -1, string _FirstList = "", string _SecondList = "", string _personeName = "")
        {
            var stringJSON = "";
            errorSendMessage = false;
            personeName = _personeName;
            autoclosedelaydefault = 45;

            autoclosedelay = autoclosedelaydefault;
            mCommand = _iCommand;
            operatingMode = _iOperatingMode;
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
            iStep = 0;
            blogin = false;
            blogout = false;
            bStart = true;

            switch (mCommand)
            {
                case WebBrowserCommand.Autorize:
                    //enableTimeoutTimer = false;
                    blogin = false;
                    blogout = true;
                    mContacterID = _contacterID;
                    break;
                case WebBrowserCommand.CheckPersonePage:
                    blogin = false;
                    blogout = true;
                    break;

                case WebBrowserCommand.LoginPersone:                    // Done!
                    var outputJSON = "{" + $@"'Model': '{_iOperatingMode}', 'Command': 'LoginPersone', 'User Login': '{mUserLogin}', 'User Password': '{mUserPassword}'" + "}";
                    NilsaWriteToRequestFile(outputJSON);
                    //нужно задержку добавить?
                    //Task.Delay(1000).Wait();
                    do
                    {
                        //Task.Delay(1000).Wait();
                        stringJSON = NilsaReadFromResponseFile();
                        mResponseFromInterface = JsonConvert.DeserializeObject<ResponseFromInterface>(stringJSON);
                    } while (mResponseFromInterface.Time == lastResponseTime);
                    lastResponseTime = mResponseFromInterface.Time;
                    break;

                case WebBrowserCommand.GoToPersonePage:
                    //enableTimeoutTimer = false;
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.GetPhotoURL:                     // Done!
                    photoURL = "";
                    mContacterID = _contacterID;

                    statusText = "Получение адреса фотографии... ";
                    autoclosedelaydefault = 25;

                    bStart = false;

                    //if (mContacterID != 330643598)
                    //{
                    //    if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                    //    {
                    //        iStep = 1;
                    //        doAction();
                    //    }
                    //    else
                    //        startPage = "https://vk.com/id" + mContacterID.ToString();
                    //}
                    //else
                    //{
                    //    contactAtrributes.FirstName = "Internal";
                    //    contactAtrributes.LastName = "Persone";
                    //    iStep = -1;
                    //    autoclosedelay = 1;
                    //}
                    break;

                case WebBrowserCommand.GetPersoneAttributes:                     // Done!
                    mContacterID = _contacterID;
                    //personeAtrributes = new Persone();
                    //personeAtrributes.id = mContacterID;

                    statusText = "Получение хар-к Персонажа... ";
                    autoclosedelaydefault = 25;

                    //workTimersInterval[5] = 7;
                    //workTimersInterval[1] = 6;
                    bStart = false;

                    //if (browserAddress.StartsWith("https://vk.com"))
                    //{
                    //    iStep = 1;
                    //    doAction();
                    //}
                    //if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                    //{
                    //    iStep = 1;
                    //    doAction();
                    //}
                    //else
                    //    startPage = "https://vk.com/id" + mContacterID.ToString();
                    break;


                case WebBrowserCommand.GetPersoneFriendsCount:                     // Done!
                    mContacterID = _contacterID;
                    //personeAtrributesFriends = new Persone();
                    //personeAtrributesFriends.id = mContacterID;

                    statusText = "Получение кол-ва друзей Персонажа... ";
                    autoclosedelaydefault = 25;

                    bStart = false;

                    //if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                    //{
                    //    iStep = 1;
                    //    doAction();
                    //}
                    //else
                    //    startPage = "https://vk.com/id" + mContacterID.ToString();
                    break;

                case WebBrowserCommand.GetContactAttributes:                     // Done!
                    mContacterID = _contacterID;
                    //contactAtrributes = new Persone();
                    //contactAtrributes.id = mContacterID;

                    statusText = "Получение хар-к Контактёра... ";
                    autoclosedelaydefault = 25;

                    //workTimersInterval[5] = 7;
                    //workTimersInterval[1] = 6;
                    bStart = false;

                    //if (mContacterID != 330643598)
                    //{
                    //    if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                    //    {
                    //        iStep = 1;
                    //        doAction();
                    //    }
                    //    else
                    //        startPage = "https://vk.com/id" + mContacterID.ToString();
                    //}
                    //else
                    //{
                    //    contactAtrributes.FirstName = "Internal";
                    //    contactAtrributes.LastName = "Persone";
                    //    iStep = -1;
                    //    autoclosedelay = 1;
                    //}
                    break;

                case WebBrowserCommand.GoToContactPage:
                    //enableTimeoutTimer = false;
                    //startPage = "http://vk.com/id" + mContacterID.ToString();
                    bStart = false;
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.ShowContactPage:
                    //startPage = "http://vk.com/id" + mContacterID.ToString();
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
                    //groupBox1.Text = _personeName;
                    statusText = "Посылка сообщения... ";
                    autoclosedelaydefault = 25;
                    //workTimersInterval[1] = 4;
                    //workTimersInterval[2] = 4;
                    //workTimersInterval[5] = 5;

                    mMessageToSend = _FirstList.Trim();
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.AddToFriends:                    // Done!
                    errorSendMessage = true;
                    //groupBox1.Text = _personeName;
                    statusText = "Добавление в друзья... ";
                    autoclosedelaydefault = 25;
                    //workTimersInterval[1] = 2;
                    //workTimersInterval[2] = 4;
                    //workTimersInterval[5] = 5;

                    mMessageToSend = _FirstList.Trim();
                    mContacterID = _contacterID;
                    break;

                case WebBrowserCommand.ReadMessages:                    // Done!
                    //groupBox1.Text = _personeName;
                    //statusText = "Чтение новых сообщений... ";
                    //autoclosedelaydefault = 25;

                    //currentPeer = null;
                    //peers.Clear();
                    //messages.Clear();

                    bStart = false;

                    //if ("https://vk.com/im?tab=unread".Equals(browserAddress))
                    //{
                    //    iStep = 1;
                    //    doAction();
                    //}
                    //else
                    //{
                    //    startPage = "https://vk.com/im?tab=unread";
                    //}
                    break;

                case WebBrowserCommand.ReadHistory:                    // Done!
                    //groupBox1.Text = _personeName;
                    statusText = "Чтение истории... ";
                    mContacterID = _contacterID;
                    autoclosedelaydefault = 25;

                    //workTimersInterval[5] = 6;
                    //currentPeer = new Peer(mContacterID, 200);
                    //peers.Clear();
                    //messages.Clear();

                    bStart = false;

                    //if (browserAddress.Equals("https://vk.com/im?sel=" + mContacterID.ToString()))
                    //{
                    //    iStep = 1;
                    //    doAction();
                    //}
                    //else
                    //{
                    //    startPage = "https://vk.com/im?sel=" + mContacterID.ToString();
                    //}
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
            return stringJSON;
        }

        //to do
        // в свитче в сетупе убрать все условия, оставить запуск разных методов и возврат строки JSON
        // для каждой команды написать свой метод с логикой, на случай негативного результата
        private void DoAction()
        {

            switch (mCommand)
            {
                case WebBrowserCommand.GetPhotoURL:
                    //if (iStep == 1)
                    //{
                    //    try
                    //    {
                    //        browser.EvaluateScriptAsync("document.getElementsByClassName('page_avatar_img')[0].src;").ContinueWith(x =>
                    //        {
                    //            var response = x.Result;
                    //            //callToBrowser += "document.getElementsByClassName('page_avatar_img')[0].src;\n";
                    //            File.AppendAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), "document.getElementsByClassName('page_avatar_img')[0].src;\n", Encoding.UTF8);
                    //            if (response.Success && response.Result != null)
                    //            {
                    //                var startDate = response.Result;

                    //                if (startDate != null)
                    //                {
                    //                    photoURL = startDate.ToString();
                    //                }
                    //            }

                    //            autoclosedelay = 1;
                    //        });
                    //    }
                    //    catch
                    //    {

                    //    }
                    //}
                    break;

                case WebBrowserCommand.GetContactAttributes:
                    if (iStep == 1)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        //TryToClickElementByClassName("profile_more_info_link", -1, -1);
                        autoclosedelay = autoclosedelaydefault;
                        //Task ts = getContactAttributesTask();
                    }
                    break;

                case WebBrowserCommand.GetPersoneFriendsCount:
                    if (iStep == 1)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        //TryToClickElementByClassName("profile_more_info_link", -1, -1);
                        autoclosedelay = autoclosedelaydefault;
                        //Task ts = getPersoneAttributesFriendsTask();
                    }
                    break;

                case WebBrowserCommand.GetPersoneAttributes:
                    if (iStep == 1)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        //if (!TryToClickElementByID("top_edit_link", -1, -1))
                        //if (!TryToClickElementByID("profile_edit_act", -1, -1))
                        //    iStep = -1;
                    }
                    else if (iStep > 1)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        //Task ts = getPersoneAttributesTask();
                    }
                    break;

                case WebBrowserCommand.LoginPersone:
                    if (iStep == 1)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        //Task ts = logoutTask();
                    }
                    else if (iStep == 2)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        //Task ts = loginTask();
                    }
                    else if (iStep == 3)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        //Task ts = checkLoginTask();
                    }
                    break;

                case WebBrowserCommand.ReadMessages:
                    if (iStep == 1)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        //Task ts = getPeers();
                    }
                    else if (iStep == 2)
                    {
                        //if (peers.Count > 0)
                        //{
                        //    autoclosedelay = autoclosedelaydefault;
                        //    currentPeer = peers[0];
                        //    peers.RemoveAt(0);
                        //    Task ts = getMessages();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    break;

                case WebBrowserCommand.ReadHistory:
                    if (iStep == 1)
                    {
                        autoclosedelay = autoclosedelaydefault;
                        //Task ts = getHistory();
                    }
                    else if (iStep == 2)
                    {
                        autoclosedelay = 1;
                    }
                    break;
            }

            if (iStep == -1)
            {
                autoclosedelay = 1;
            }

            //File.WriteAllText(Path.Combine(Application.StartupPath, "_call_to_browser.txt"), callToBrowser, Encoding.UTF8);
        }

        private void DoCondition()
        {
            switch (mCommand)
            {
                case WebBrowserCommand.GetPhotoURL:
                    if (iStep == 0)
                    {
                        //if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                        //{
                        //    iStep = 1;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    break;

                case WebBrowserCommand.GetContactAttributes:
                    if (iStep == 0)
                    {
                        //if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                        //{
                        //    iStep = 1;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    break;

                case WebBrowserCommand.GetPersoneFriendsCount:
                    if (iStep == 0)
                    {
                        //if (browserAddress.Equals("https://vk.com/id" + mContacterID.ToString()))
                        //{
                        //    iStep = 1;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    break;

                case WebBrowserCommand.GetPersoneAttributes:
                    if (iStep == 0)
                    {
                        //if (browserAddress.StartsWith("https://vk.com"))
                        //{
                        //    iStep = 1;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    else if (iStep == 1)
                    {
                        //if (browserAddress.StartsWith("https://vk.com"))
                        //{
                        //    iStep++;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    else if (iStep > 1)
                    {
                        //if (browserAddress.StartsWith("https://vk.com"))
                        //{
                        //    //iStep++;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    break;

                case WebBrowserCommand.LoginPersone:
                    if (iStep == 0)
                    {
                        //if (browserAddress.StartsWith("https://vk.com"))
                        //{
                        //    iStep = 2;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    else
                    {
                        //if (browserAddress.StartsWith("https://vk.com"))
                        //    doAction();
                        //else
                        //    iStep = -1;
                    }
                    break;

                case WebBrowserCommand.ReadHistory:
                    if (iStep == 0)
                    {
                        //if (browserAddress.Equals("https://vk.com/im?sel=" + mContacterID.ToString()))
                        //{
                        //    iStep = 1;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    else if (iStep == 1)
                    {
                        iStep = 2;
                        DoAction();
                    }
                    break;

                case WebBrowserCommand.ReadMessages:
                    if (iStep == 0)
                    {
                        //if ("https://vk.com/im?tab=unread".Equals(browserAddress))
                        //{
                        //    iStep = 1;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    else if (iStep == 1)
                    {
                        //if (browserAddress.StartsWith("https://vk.com/im?sel="))
                        //{
                        //    iStep = 2;
                        //    doAction();
                        //}
                        //else
                        //    iStep = -1;
                    }
                    break;
            }

            if (iStep == -1)
            {
                autoclosedelay = 1;
            }
        }

        private void NilsaWriteToRequestFile(string request)
        {
            try
            {
                string requestPath = Path.Combine(mFormMain.AppplicationStarupPath(), "_request_to_browser.txt");

                // Write the request to file

                File.WriteAllText(requestPath, request, Encoding.UTF8);
            }
            catch (Exception) { }
        }

        private string NilsaReadFromResponseFile()
        {
            var response = "Error no response";
            try
            {
                string responsePath = Path.Combine(mFormMain.AppplicationStarupPath(), "_response_from_browser.txt");

                // Read the request from file

                response = File.ReadAllText(responsePath);
            }
            catch (Exception) { }
            return response;
        }
    }
}
