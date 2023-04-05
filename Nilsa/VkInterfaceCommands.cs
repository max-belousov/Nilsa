using CefSharp;
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
        private string operatingMode = NilsaOperatingMode.None;
        private uint mCommand = WebBrowserCommand.None;
        private string stringJSON = "";
        private string outputJSON = "";
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

        public string Setup(String sLogin, String sPassword, uint _iCommand, string _iOperatingMode, long _contacterID = -1, string _FirstList = "", string _SecondList = "", string _personeName = "")
        {
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
                case WebBrowserCommand.LoginPersone:                    // Done!
                    outputJSON = "{" + $@"'Mode': '{_iOperatingMode}', 'Command': 'LoginPersone', 'User Login': '{mUserLogin}', 'User Password': '{mUserPassword}'" + "}";
                    NilsaWriteToRequestFile(outputJSON);
                    Task.Delay(1000).Wait();
                    NilsaReadFromResponseFile();
                    LogingRequestFile(outputJSON);
                    break;

                case WebBrowserCommand.GetPhotoURL:                     // Done!
                    mContacterID = _contacterID;
                    outputJSON = "{" + $@"'Mode': '{_iOperatingMode}', 'Command': 'GetPhotoURL', 'User ID': '{mContacterID}'" + "}";
                    NilsaWriteToRequestFile(outputJSON);
                    Task.Delay(1000).Wait();
                    NilsaReadFromResponseFile();
                    LogingRequestFile(outputJSON);
                    break;

                case WebBrowserCommand.GetPersoneAttributes:                     // Done!
                    mContacterID = _contacterID;
                    outputJSON = "{" + $@"'Mode': '{_iOperatingMode}', 'Command': 'GetPersoneAttributes', 'User ID': '{mContacterID}'" + "}";
                    NilsaWriteToRequestFile(outputJSON);
                    Task.Delay(1000).Wait();
                    NilsaReadFromResponseFile();
                    LogingRequestFile(outputJSON);
                    break;

                    //Если суть посчитать друзей, то реализовано через атрибуты
                //case WebBrowserCommand.GetPersoneFriendsCount:                     // Done!
                //    mContacterID = _contacterID;
                //    outputJSON = "{" + $@"'Mode': '{_iOperatingMode}', 'Command': 'GetPersoneFriendsCount', 'User ID': '{mContacterID}'" + "}";
                //    NilsaWriteToRequestFile(outputJSON);
                //    Task.Delay(1000).Wait();
                //    NilsaReadFromResponseFile();
                //    LogingRequestFile(outputJSON);
                //    break;

                case WebBrowserCommand.GetContactAttributes:                     // Done!
                    mContacterID = _contacterID;
                    outputJSON = "{" + $@"'Mode': '{_iOperatingMode}', 'Command': 'GetContactAttributes', 'User ID': '{mContacterID}'" + "}";
                    NilsaWriteToRequestFile(outputJSON);
                    Task.Delay(1000).Wait();
                    NilsaReadFromResponseFile();
                    LogingRequestFile(outputJSON);
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

        private void NilsaReadFromResponseFile()
        {
            //раскоментить после реализации записи интерфейса в файл
            //do
            //{
            //    try
            //    {
            //        string responsePath = Path.Combine(mFormMain.AppplicationStarupPath(), "_response_from_browser.txt");

            //        // Read the request from file

            //        stringJSON = File.ReadAllText(responsePath);
            //    }
            //    catch (Exception) { }
            //    mResponseFromInterface = JsonConvert.DeserializeObject<ResponseFromInterface>(stringJSON);
            //} while (mResponseFromInterface.Time == lastResponseTime);
            //lastResponseTime = mResponseFromInterface.Time;
        }

        private void LogingRequestFile(string request)
        {
            try
            {
                string path = Path.Combine(mFormMain.AppplicationStarupPath(), "_loging_requests.txt");
                File.AppendAllText(path, DateTime.Now.ToString() + "|" + request + "\n", Encoding.UTF8);
            }
            catch (Exception) { }
        }
    }
}
