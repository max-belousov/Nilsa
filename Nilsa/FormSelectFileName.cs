using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nilsa
{
    public partial class FormSelectFileName : Form
    {
        //public bool bDemoVersionOnly = false;
        FormMain mFormMain;
        int modeltype;
        string FTP_SERVER_POSTFIX;

        public FormSelectFileName(FormMain _mFormMain, int _modeltype)
        {
            mFormMain = _mFormMain;
            modeltype = _modeltype;
            InitializeComponent();

            switch (modeltype)
            {
                case 0:
                    FTP_SERVER_POSTFIX = FormMain.FTP_SERVER_MODEL_NAME_POSTFIX;
                    break;
                case 1:
                    FTP_SERVER_POSTFIX = FormMain.FTP_SERVER_PERSONEN_NAME_POSTFIX;
                    break;
                case 2:
                    FTP_SERVER_POSTFIX = FormMain.FTP_SERVER_EQOUT_NAME_POSTFIX;
                    break;
                case 3:
                    FTP_SERVER_POSTFIX = FormMain.FTP_SERVER_EQIN_NAME_POSTFIX;
                    break;
                case 4:
                    FTP_SERVER_POSTFIX = FormMain.FTP_SERVER_CONT_NAME_POSTFIX;
                    break;
            }
        }

        private void tbFileName_TextChanged(object sender, EventArgs e)
        {
            buttonOk.Enabled = tbFileName.Text.Length > 0;
            //if (tbFileName.Text.StartsWith(FormMain.FTP_SERVER_DEMOVERSION_FILENAME_PREFIX))
            //    checkBoxDemoVersion.Checked = true;
            //else
            //    checkBoxDemoVersion.Checked = false;

            if (lbFiles.Items.Contains(tbFileName.Text))
                buttonDelete.Enabled = true; //!bDemoVersionOnly;
            else
                buttonDelete.Enabled = false;
        }

        private void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFiles.SelectedIndex >= 0)
            {
                tbFileName.Text = lbFiles.Items[lbFiles.SelectedIndex].ToString();

                //if (tbFileName.Text.StartsWith(FormMain.FTP_SERVER_DEMOVERSION_FILENAME_PREFIX))
                //    checkBoxDemoVersion.Checked = true;
                //else
                //    checkBoxDemoVersion.Checked = false;

                if (lbFiles.Items.Contains(tbFileName.Text))
                    buttonDelete.Enabled = true; //!bDemoVersionOnly;
                else
                    buttonDelete.Enabled = false;
            }
        }

        private string getPrefixByLicenceType()
        {
            switch (FormMain.iLicenseType)
            {
                case FormMain.LICENSE_TYPE_WORK:
                    return FormMain.sLicenseUser + "_";
                case FormMain.LICENSE_TYPE_PRO:
                    return FormMain.sLicenseUser + "_";
                case FormMain.LICENSE_TYPE_ADMIN:
                    return "ADMIN_";
                case FormMain.LICENSE_TYPE_NILS:
                    return "NILS_";
            }

            return "DEMO_";
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (tbFileName.Enabled)
            {
                string line = tbFileName.Text;
                string FTP_SERVER_PREFIX = getPrefixByLicenceType();

                switch (FormMain.iLicenseType)
                {
                    case FormMain.LICENSE_TYPE_DEMO:
                        if (!line.StartsWith(FTP_SERVER_PREFIX))
                            tbFileName.Text = FTP_SERVER_PREFIX + tbFileName.Text;
                        break;
                    case FormMain.LICENSE_TYPE_WORK:
                        if (!line.StartsWith(FTP_SERVER_PREFIX))
                            tbFileName.Text = FTP_SERVER_PREFIX + tbFileName.Text;
                        break;
                    case FormMain.LICENSE_TYPE_PRO:
                        if (!line.StartsWith(FTP_SERVER_PREFIX))
                            tbFileName.Text = FTP_SERVER_PREFIX + tbFileName.Text;
                        break;
                    case FormMain.LICENSE_TYPE_ADMIN:
                        if (line.StartsWith("PRO_") || line.StartsWith("NILS_"))
                            tbFileName.Text = FTP_SERVER_PREFIX + tbFileName.Text;
                        break;
                    //case FormMain.LICENSE_TYPE_NILS:
                    //    break;
                }

                //if (bDemoVersionOnly && !checkBoxDemoVersion.Checked)
                //{
                //    tbFileName.Text = FormMain.FTP_SERVER_DEMOVERSION_FILENAME_PREFIX + tbFileName.Text;
                //    MessageBox.Show("Вы используете Демоверсию NILSA. К имени файла автоматически добавлен префикс '" + FormMain.FTP_SERVER_DEMOVERSION_FILENAME_PREFIX + "'.", "Коррекция имени файла", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //}

                if (lbFiles.Items.Contains(tbFileName.Text))
                {
                    if (MessageBox.Show("Файл с таким именем уже есть на сервере. Вы можете либо перезаписать его, либо изменить имя.\n\nПерезаписать файл?", "Подтверждение имени файла", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        DialogResult = DialogResult.OK;
                }
                else
                    DialogResult = DialogResult.OK;
            }
            else
                DialogResult = DialogResult.OK;
        }

        //private void checkBox1_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBoxDemoVersion.Checked)
        //    {
        //        if (!tbFileName.Text.StartsWith(FormMain.FTP_SERVER_DEMOVERSION_FILENAME_PREFIX))
        //            tbFileName.Text = FormMain.FTP_SERVER_DEMOVERSION_FILENAME_PREFIX + tbFileName.Text;
        //    }
        //    else
        //    {
        //        if (tbFileName.Text.StartsWith(FormMain.FTP_SERVER_DEMOVERSION_FILENAME_PREFIX))
        //            tbFileName.Text = tbFileName.Text.Substring(FormMain.FTP_SERVER_DEMOVERSION_FILENAME_PREFIX.Length);
        //    }
        //}

        public bool DeleteFileOnFtpServer(string _filename)
        {
            try
            {
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(FormMain.FTP_SERVER_URI + _filename));
                request.Credentials = new NetworkCredential(mFormMain.FTP_SERVER_LOGIN, mFormMain.FTP_SERVER_PASSWORD);
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //Console.WriteLine("Delete status: {0}", response.StatusDescription);
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (lbFiles.Items.Contains(tbFileName.Text))
            {
                if (MessageBox.Show("Вы действительно хотите удалить файл '" + tbFileName.Text + "' с Сервера?", "Удаление файла", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DeleteFileOnFtpServer(tbFileName.Text + FTP_SERVER_POSTFIX))
                    {
                        lbFiles.Items.Remove(tbFileName.Text);
                        MessageBox.Show("Файл '" + tbFileName.Text + "' успешно удален с Сервера", "Удаление файла", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tbFileName.Text = "";
                        tbFileName_TextChanged(null, null);
                    }
                    else
                        MessageBox.Show("При удалении файла '" + tbFileName.Text + "' с Сервера произошла ошибка. Пожалуйста, проверьте Ваше подключение к интернету или повторите операции позже.", "Удаление файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
