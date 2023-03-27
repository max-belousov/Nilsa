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
    public partial class FormEditUserMessagesDBList : Form
    {
        List<String> lstDBs;
        FormMain mFormMain;

        public FormEditUserMessagesDBList(FormMain formmain)
        {
            mFormMain = formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        public void Setup()
        {
            lstDBs = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_messages_db.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_messages_db.txt"));
                lstDBs = new List<String>(srcFile);
            }
            else
                lstDBs.Add("0|" + FormMain.strDB0Name);

            clbDBs.BeginUpdate();
            clbDBs.Items.Clear();
            foreach (String str in lstDBs)
            {
                if (str == null)
                    continue;

                if (str.Length == 0)
                    continue;

                String sdbID = str.Substring(0, str.IndexOf("|"));
                clbDBs.Items.Add(str.Substring(str.IndexOf("|") + 1));
            }
            clbDBs.EndUpdate();

        }

        private void clbDBs_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = clbDBs.SelectedIndex >= 0;
            buttonRename.Enabled = clbDBs.SelectedIndex >= 0;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i = clbDBs.SelectedIndex;
            String sdbID = lstDBs[i].Substring(0, lstDBs[i].IndexOf("|"));
            String sdbName = lstDBs[i].Substring(lstDBs[i].IndexOf("|") + 1);

            if (MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Вы действительно хотите удалить базу") + " '" + sdbName + "'?", NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Удаление базы"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                clbDBs.Items.RemoveAt(i);
                lstDBs.RemoveAt(i);
                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_messages_db.txt"), lstDBs, Encoding.UTF8);

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_eqinmsgdb_" + sdbID + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_eqinmsgdb_" + sdbID + ".txt"));
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_eqoutmsgdb_" + sdbID + ".txt")))
                    File.Delete(Path.Combine(FormMain.sDataPath, "_eqoutmsgdb_" + sdbID + ".txt"));

                string[] files = System.IO.Directory.GetFiles(FormMain.sDataPath, "_msg_har_" + sdbID + "_*.txt");
                foreach (string f in files)
                    File.Delete(f);

                List<String> lstFile = new List<String>();
                foreach (String str in lstDBs)
                {
                    if (str == null)
                        continue;

                    if (str.Length == 0)
                        continue;

                    String _sdbID = str.Substring(0, str.IndexOf("|"));
                    String _sdbName = str.Substring(str.IndexOf("|") + 1);
                    lstFile.Add(_sdbName);
                }
                File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_msg_har_1.txt"), lstFile, Encoding.UTF8);
                mFormMain.LoadMessagesDBList();
                mFormMain.SaveMessagesDBList();

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void MessagesDB_RenameInContent(String sFileName, String delimiterLeft, String delimiterRight, String oldValue, String newValue, bool bEqual)
        {
            List<String> lstContent = new List<String>();
            if (File.Exists(Path.Combine(FormMain.sDataPath, sFileName)))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, sFileName));
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
                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, sFileName), lstContent, Encoding.UTF8);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int iSelIdx = clbDBs.SelectedIndex;
            String sdbID = lstDBs[iSelIdx].Substring(0, lstDBs[iSelIdx].IndexOf("|"));
            String strDB0Name = lstDBs[iSelIdx].Substring(lstDBs[iSelIdx].IndexOf("|") + 1);

            //if (MessageBox.Show("Вы действительно хотите переименовать базу" + " '" + strDB0Name + "'?", "Переименование базы", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //{
            string value = strDB0Name;
            if (FormMain.InputBox(this, "Переименование базы", "Введите имя базы:", ref value) == DialogResult.OK)
            {
                value = value.Trim();
                //string valuePrefix = strDB0Name;
                //if (FormMain.InputBox("Удаляемый префикс", "Введите префикс для удаления:", ref valuePrefix) == DialogResult.OK)
                //{
                //    valuePrefix = valuePrefix.Trim();
                //}
                if (value.Length > 0)
                {
                    MessagesDB_RenameInContent("_messages_db.txt", sdbID + "|", "", strDB0Name, value, false);
                    MessagesDB_RenameInContent("_algorithms_db.txt", "|", "|", strDB0Name, value, false);
                    MessagesDB_RenameInContent("_algorithms_db.txt", "~", "`", strDB0Name.ToLower(), value.ToLower(), false);

                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_eqinmsgdb_" + sdbID + ".txt")))
                        MessagesDB_RenameInContent("_eqinmsgdb_" + sdbID + ".txt", "000000|", "|", strDB0Name, value, false);
                    if (File.Exists(Path.Combine(FormMain.sDataPath, "_eqoutmsgdb_" + sdbID + ".txt")))
                        MessagesDB_RenameInContent("_eqoutmsgdb_" + sdbID + ".txt", "000000|", "|", strDB0Name, value, false);
                    MessagesDB_RenameInContent("_msg_har_1.txt", "", "", strDB0Name, value, true);
                    lstDBs[iSelIdx] = sdbID + "|" + value;
                    clbDBs.Items[iSelIdx] = value;
                    mFormMain.ReloadDBs();
                }
            }
            //}
        }

        private void buttonDeletePrefix_Click(object sender, EventArgs e)
        {
            string valuePrefix = "";
            if (FormMain.InputBox(this, "Заменяемый префикс", "Введите префикс для замены:", ref valuePrefix) == DialogResult.OK)
            {
                valuePrefix = valuePrefix.Trim();
            }
            if (valuePrefix.Length == 0)
                return;

            if (MessageBox.Show("ВНИМАНИЕ!!! БУДЬТЕ ОЧЕНЬ ОСТОРОЖНЫ!\n\nДанная функция заменит этот префикс (строку) во всех местах, что может привести к некорректности работы модели.\n\nВы действительно хотите заменить префикс " + " '" + valuePrefix + "'?", "Удаление префикса", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string value = valuePrefix;
                if (FormMain.InputBox(this, "Замена префикса", "Введите новый префикс:", ref value) == DialogResult.OK)
                {
                    value = value.Trim();
                    //if (value.Length > 0)
                    //{
                    MessagesDB_RenameInContent("_algorithms_db.txt", "|", "", valuePrefix, value, false);

                    string[] files = Directory.GetFiles(FormMain.sDataPath, "_eqinmsgdb_*.txt");
                    foreach (string f in files)
                        MessagesDB_RenameInContent(Path.GetFileName(f), "|", "", valuePrefix, value, false);

                    files = Directory.GetFiles(FormMain.sDataPath, "_eqoutmsgdb_*.txt");
                    foreach (string f in files)
                        MessagesDB_RenameInContent(Path.GetFileName(f), "|", "", valuePrefix, value, false);

                    MessagesDB_RenameInContent("_msg_har_3.txt", "", "", valuePrefix, value, false);
                    files = Directory.GetFiles(FormMain.sDataPath, "_msg_har_*_3.txt");
                    foreach (string f in files)
                        MessagesDB_RenameInContent(Path.GetFileName(f), "", "", valuePrefix, value, false);

                    mFormMain.ReloadDBs();
                    //}
                }
            }
        }
    }
}
