using mevoronin.RuCaptchaNETClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nilsa
{
    public partial class FormCaptcha : Form
    {
        string _DownloadFileName = "";
        string _RecognizeFileName = "";
        FormMain mFormMain;
        bool bStatusService=false;

        public FormCaptcha(FormMain _formmain, Uri url, long? sid, bool doNotStart=false)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            bStatusService = !mFormMain.tbStartService.Enabled;
            if (doNotStart)
                bStatusService = false;
            else
                mFormMain.tbStopService_Click(null, null);

            labelInfo.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Загрузка капчи, пожалуйста, ждите...");
            _DownloadFileName = Path.Combine(Application.StartupPath, "captha_"+ sid.ToString() + ".bmp");
            _RecognizeFileName = Path.Combine(Application.StartupPath, "captha_" + sid.ToString() + ".jpg");
            DeleteSIDPictures();

            var client = new WebClient();
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadVersion_Completed);
            client.DownloadFileAsync(url, _DownloadFileName);
            //var image = Image.FromStream(new MemoryStream(client.DownloadData(url)));
            //captchaPictureBox.Image = image;
        }

        private void DeleteSIDPictures()
        {
            try
            {
                if (File.Exists(_RecognizeFileName))
                    File.Delete(_RecognizeFileName);
            }
            catch
            { }
            try
            {
                if (File.Exists(_DownloadFileName))
                    File.Delete(_DownloadFileName);
            }
            catch
            { }
        }

        private void downloadVersion_Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (File.Exists(_DownloadFileName))
                {
                    labelInfo.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Капча загружена, посылаем для распознавания...");
                    var image = Image.FromFile(_DownloadFileName);
                    captchaPictureBox.Image = image;
                    captchaPictureBox.Image.Save(_RecognizeFileName,System.Drawing.Imaging.ImageFormat.Jpeg);
                    Application.DoEvents();
                    Recognize();
                }
            }
            else
            {
                labelInfo.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Автоматическое распознавание не удалось. Пожалуйста, введите код с картинки:");
            }
        }

        bool recognizeDone = false;
        RuCaptchaClient ruCaptchaClient = null;
        int countCaptchaClient = 0;
        string answerCaptchaClient = null;
        string captcha_idCaptchaClient = null;

        private void Recognize()
        {
            ruCaptchaClient = new RuCaptchaClient(NilsaUtils.LoadRuCaptcha((FormMain.iLicenseType == FormMain.LICENSE_TYPE_NILS || FormMain.iLicenseType == FormMain.LICENSE_TYPE_WORK) ? "5fa1d21d96f3f039925cc5f58e024787" : "d29a67832cc05e692ef3b496a36157d5"));
            captcha_idCaptchaClient = null;
            try
            {
                captcha_idCaptchaClient = ruCaptchaClient.UploadCaptchaFile(_RecognizeFileName);
            }
            catch (Exception exp)
            {
                captcha_idCaptchaClient = null;
            }

            if (captcha_idCaptchaClient == null)
            {
                labelInfo.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Автоматическое распознавание не удалось. Пожалуйста, введите код с картинки:");
                return;
            }

            labelInfo.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Капча отослана для распознавания, ждем результат...");
            timerRecognize.Enabled = true;
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeleteSIDPictures();
            timerRecognize.Enabled = false;
            if (bStatusService)
                mFormMain.tbStartService_Click(null, null);
        }

        private void FormCaptcha_Shown(object sender, EventArgs e)
        {
            if (recognizeDone)
            {
                timerRecognize.Enabled = false;
                DeleteSIDPictures();
                if (bStatusService)
                    mFormMain.tbStartService_Click(null, null);
                DialogResult = DialogResult.OK;
                Application.DoEvents();
            }
        }

        private void timerRecognize_Tick(object sender, EventArgs e)
        {
            timerRecognize.Enabled = false;
            countCaptchaClient++;
            if (countCaptchaClient % 5 != 0)
            {
                timerRecognize.Enabled = true;
                labelInfo.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Капча отослана для распознавания, ждем результат")+" " + countCaptchaClient.ToString()+" "+ NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "сек...");
                return;
            }

            timerRecognize.Enabled = true;
            labelInfo.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Капча отослана для распознавания, ждем результат") + " " + countCaptchaClient.ToString() + " " + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "сек...");
            if (string.IsNullOrEmpty(answerCaptchaClient))
            {
                try
                {
                    answerCaptchaClient = ruCaptchaClient.GetCaptcha(captcha_idCaptchaClient);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }

            if (!string.IsNullOrEmpty(answerCaptchaClient))
            {
                recognizeDone = true;
                DeleteSIDPictures();
                CaptchaKey.Text = answerCaptchaClient;
                timerRecognize.Enabled = false;
                if (bStatusService)
                    mFormMain.tbStartService_Click(null, null);
                DialogResult = DialogResult.OK;
                Application.DoEvents();
            }
            else
            {
                if (countCaptchaClient < 100)
                    timerRecognize.Enabled = true;
                else
                    button2_Click(null,null);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteSIDPictures();
            timerRecognize.Enabled = false;
            if (bStatusService)
                mFormMain.tbStartService_Click(null, null);
        }
    }
}
