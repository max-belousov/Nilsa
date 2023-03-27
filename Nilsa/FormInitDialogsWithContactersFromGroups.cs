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
    public partial class FormInitDialogsWithContactersFromGroups : Form
    {
        bool bDoImportGroups = true;
        FormMain mFormMain;
        public long iPersUserID;
        int iImportMode;

        public String[,] sGroupHar;
        public int iGroupHarCount = 16;
        public int iGroupHarAttrCount = 4;

        private int iMasterStep = 0;
        private int iMasterStepCount = 10;
        bool bExpert = false;

        public FormInitDialogsWithContactersFromGroups(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();
        }

        public void Setup(long _iPersUserID)
        {
            iPersUserID = _iPersUserID;

            labelInfo.Text = "Вас приветствует Мастер инициации диалога с Контактерами из тематических групп! Он проведет Вас по шагам инициации и автоматически запустит в конце процесс общения. \n\nДля запуска процесса инициации диалога с Контактерами нажмите кнопку 'Старт'.\n\n\n\n ВАЖНО:\n\n1. Перед запуском Мастера в форме редактирования базы Персонажей у Вас должны быть выбраны требуемые Персонажи для ротации после автоматического запуска общения по окончанию работы Мастера.\n\n2. Также предварительно у Вас должен быть настроен фильтр Контактеров по характеристикам в форме редактирования базы Контактеров для автоматической инициации диалогов при ротации Персонажей.";
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (bDoImportGroups)
                nextStep();
            else
                this.bwProgress.CancelAsync();
        }

        private void nextStep()
        {
            if (iMasterStep == 0)
            {
                bExpert = checkBoxExprert.Checked;
                checkBoxExprert.Visible = false;
            }

            iMasterStep++;
            labelInfo.Text = "Шаг " + iMasterStep.ToString() + "/" + iMasterStepCount.ToString() + ". ";
            switch (iMasterStep)
            {
                case 1:
                    if (!bExpert)
                        nextStep();
                    else
                    {
                        buttonStart.Text = "Далее";
                        labelInfo.Text += "Инициация настройки импорта групп.\n\n";
                        labelInfo.Text += "По нажатию кнопки 'Далее' будет запущена настройка импорта тематических групп для инициализации диалогов. Вам последовательно будет предложено ввести или выбрать:\n\n";
                        labelInfo.Text += "- поисковый запрос для отбора тематических групп\n";
                        labelInfo.Text += "- требуемое число групп и реакцию на дубликаты групп в списке групп текущего Персонажа (Добавлять или Игнорировать)\n";
                        labelInfo.Text += "- характеристики групп при импорте\n";
                        labelInfo.Text += "- фильтр по характеристикам групп\n";
                        labelInfo.Text += "\nДля начала процесса нажмите кнопку 'Далее'.";
                    }
                    break;

                case 2:
                    labelInfo.Text += "Настройка импорта тематических групп.\n\n";
                    if (!startImportGroups())
                        masterCancelled();
                    else
                        nextStep();
                    break;

                case 3:
                    if (!bExpert)
                        nextStep();
                    else
                    {
                        labelInfo.Text += "Инициация настройки импорта Контактеров.\n\n";
                        labelInfo.Text += "По нажатию кнопки 'Далее' будет запущена настройка импорта Контактеров из тематических групп для инициализации диалогов. Вам последовательно будет предложено ввести или выбрать:\n\n";
                        labelInfo.Text += "- выбрать Персонажей для импорта Контактеров\n";
                        labelInfo.Text += "- требуемое число участников, администраторов, авторов записей и комментаторов записей групп и реакцию на дубликаты Контактеров (Добавлять, Игнорировать или Спрашивать)\n";
                        labelInfo.Text += "- характеристики Контактеров при импорте\n";
                        labelInfo.Text += "- фильтр по характеристикам Контактеров\n";
                        labelInfo.Text += "\nДля начала процесса нажмите кнопку 'Далее'.";
                    }
                    break;

                case 4:
                    labelInfo.Text += "Настройка импорта Контактеров.\n\n";
                    if (!startImportContactersFromGroup())
                        masterCancelled();
                    else
                        nextStep();
                    break;

                case 5:
                    if (!bExpert)
                        nextStep();
                    else
                    {
                        labelInfo.Text += "Инициация настройки Диалогов.\n\n";
                        labelInfo.Text += "По нажатию кнопки 'Далее' будет запущена настройка инициации диалогов с Контактерами. Вам необходимо выбрать:\n\n";
                        labelInfo.Text += "- алгоритм и его параметры\n";
                        labelInfo.Text += "- суточный лимит инициации диалогов Персонажем\n";
                        labelInfo.Text += "- другие требуемые параметры инициации Диалогов\n";
                        labelInfo.Text += "\nДля начала процесса нажмите кнопку 'Далее'.";
                    }
                    break;

                case 6:
                    if (!bExpert)
                        nextStep();
                    else
                    {
                        labelInfo.Text += "Настройка инициации Диалогов.\n\n";
                        if (!startDialogsSettings())
                            masterCancelled();
                        else
                            nextStep();
                    }
                    break;

                case 7:
                    if (!bExpert)
                        nextStep();
                    else
                    {
                        labelInfo.Text += "Запуск импорта Групп, Контактеров и Инициация диалогов.\n\n";
                        buttonStart.Text = "Запуск";
                        labelInfo.Text += "Все настроено.\n\nПо нажатию кнопки 'Запуск' будет запущен автоматический процесс импорта Групп, затем Контактеров из импортированных групп. По окончании процесса автоматически будет запущена инициация диалогов с импортированными Контактерами.\n\n";
                        labelInfo.Text += "Для начала процесса нажмите кнопку 'Запуск'.";
                    }
                    break;

                case 8:
                    pbProgress.Visible = true;
                    buttonStart.Text = "Остановить";
                    labelInfo.Text += "Импорт тематических групп.\n\n";
                    labelData.Text = "Инициация...";
                    labelData.Visible = true;
                    doImportGroups();
                    break;

                case 9:
                    if (hashsetImportedGroupList.Count == 0)
                    {
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        pbProgress.Value = 0;
                        pbProgress.Text = "";
                        buttonStart.Text = "Остановить";
                        labelInfo.Text += "Импорт Контактеров.\n\n";
                        labelData.Text = "Инициация...";
                        labelData.Visible = true;
                        doImportContactersFromGroup();
                    }
                    break;

                case 10:
                    pbProgress.Visible = false;
                    labelData.Visible = false;
                    labelInfo.Text += "Запуск инициации диалогов.\n\n";
                    DialogResult = DialogResult.OK;
                    break;

            }
        }

        int nudContacterCountMembers = 0;
        int nudContacterCountAdministrators = 0;
        int nudContacterCountAuthors = 0;
        int nudContacterCountCommentators = 0;
        int nudNeedContacterMultiplicator = 10;
        String sImportIDsContacterGroups = "";
        String sPersUIDsContacterGroups = "";
        int nudContacterDuplicatesAction = 0;

        private bool startDialogsSettings()
        {
            if (File.Exists(Path.Combine(FormMain.sDataPath, "_initdialog_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt")))
                File.Delete(Path.Combine(FormMain.sDataPath, "_initdialog_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + ".txt"));

            FormInitContactDialog fe = new FormInitContactDialog(mFormMain);
            fe.sContHar = new String[mFormMain.iContHarCount, mFormMain.iContHarAttrCount + 1];
            for (int i = 0; i < mFormMain.iContHarCount; i++)
            {
                for (int j = 0; j < mFormMain.iContHarAttrCount; j++)
                    fe.sContHar[i, j] = mFormMain.sContHar[i, j];
                fe.sContHar[i, mFormMain.iContHarAttrCount] = "";
            }
            fe.iContHarCount = mFormMain.iContHarCount;
            fe.iContHarAttrCount = mFormMain.iContHarAttrCount;
            fe.Setup(iPersUserID.ToString() + " (" + mFormMain.userLogin + ", " + mFormMain.userPassword + ")", iPersUserID, false);

            DialogResult dr = fe.ShowDialog();

            return true;
        }

        private bool startImportContactersFromGroup()
        {
            iImportMode = 8;

            sPersUIDsContacterGroups = SelectPersonen("Выбор Персонажей для импорта Контактеров");
            if (sPersUIDsContacterGroups.Length == 0)
                return false;

            FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
            fecic.Setup(sPersUIDsContacterGroups.Split('|').Length - 1, 2);
            LoadFormEnterContactersToImportCount(fecic, iImportMode, 1);

            if (fecic.ShowDialog() != DialogResult.OK)
                return false;

            SaveFormEnterContactersToImportCount(fecic, iImportMode, 1);

            nudContacterCountMembers = (int)fecic.nudContacterCount.Value;
            nudContacterCountAdministrators = (int)fecic.numericUpDown1.Value;
            nudContacterCountAuthors = (int)fecic.numericUpDown2.Value;
            nudContacterCountCommentators = (int)fecic.numericUpDown3.Value;
            nudNeedContacterMultiplicator = (int)fecic.numericUpDown4.Value;
            nudContacterDuplicatesAction = fecic.radioButton3.Checked ? 0 : (fecic.radioButton1.Checked ? 1 : 2);

            if (!bExpert)
            {
                groupsTargetCount = 0;

                if ((int)Math.Round((double)nudContacterCountMembers / 100.0 + 0.5) > groupsTargetCount)
                    groupsTargetCount = (int)Math.Round((double)nudContacterCountMembers / 100.0 + 0.5);

                if (nudContacterCountAdministrators > groupsTargetCount)
                    groupsTargetCount = nudContacterCountAdministrators;

                if ((int)Math.Round((double)nudContacterCountAuthors / 10.0 + 0.5) > groupsTargetCount)
                    groupsTargetCount = (int)Math.Round((double)nudContacterCountAuthors / 10.0 + 0.5);

                if ((int)Math.Round((double)nudContacterCountCommentators / 10.0 + 0.5) > groupsTargetCount)
                    groupsTargetCount = (int)Math.Round((double)nudContacterCountCommentators / 10.0 + 0.5);
            }

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
            fe.Text = "Редактирование значений характеристик Контактеров";

            if (bExpert)
            {
                if (fe.ShowDialog() != DialogResult.OK)
                    return false;

                SaveFormEditPersHarValuesContacters(fe, iImportMode, 1);
            }

            lstContHarContacterGroups = new List<String>(); 
            for (int i = 0; i < mFormMain.iContHarCount; i++)
                lstContHarContacterGroups.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, mFormMain.iContHarAttrCount]);
            lstFilterHarContacterGroups = GetHarValuesContacters("Фильтр значений характеристик Контактеров", iImportMode);
            if (lstFilterHarContacterGroups != null)
                return true;

            return false;
        }

        private void LoadFormEnterContactersToImportCount(FormEnterContactersToImportCount fecic, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEnterContactersToImportCount_3" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEnterContactersToImportCount_3" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"));
                lstContHar = new List<String>(srcFile);
            }

            if (lstContHar.Count > 0)
            {
                fecic.nudContacterCount.Value = lstContHar.Count > 0 ? Convert.ToInt32(lstContHar[0]) : 0;
                fecic.numericUpDown1.Value = lstContHar.Count > 1 ? Convert.ToInt32(lstContHar[1]) : 0;
                fecic.numericUpDown2.Value = lstContHar.Count > 2 ? Convert.ToInt32(lstContHar[2]) : 0;
                fecic.numericUpDown3.Value = lstContHar.Count > 3 ? Convert.ToInt32(lstContHar[3]) : 0;
                fecic.numericUpDown4.Value = lstContHar.Count > 4 ? Convert.ToInt32(lstContHar[4]) : 10;
                if (lstContHar.Count > 5)
                {
                    if (lstContHar[5].Equals("0"))
                        fecic.radioButton3.Checked = true;
                    else if (lstContHar[5].Equals("1"))
                        fecic.radioButton1.Checked = true;
                    else
                        fecic.radioButton2.Checked = true;
                }
                if (lstContHar.Count > 6)
                {
                    if (lstContHar[6].Equals("0"))
                        fecic.radioButton4.Checked = true;
                    else
                        fecic.radioButton5.Checked = true;
                }

                fecic.nudContacterCount_ValueChanged(null, null);
            }
        }

        private void SaveFormEnterContactersToImportCount(FormEnterContactersToImportCount fecic, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();

            lstContHar.Add(((int)fecic.nudContacterCount.Value).ToString());
            lstContHar.Add(((int)fecic.numericUpDown1.Value).ToString());
            lstContHar.Add(((int)fecic.numericUpDown2.Value).ToString());
            lstContHar.Add(((int)fecic.numericUpDown3.Value).ToString());
            lstContHar.Add(((int)fecic.numericUpDown4.Value).ToString());
            lstContHar.Add(fecic.radioButton3.Checked ? "0" : (fecic.radioButton1.Checked ? "1" : "2"));
            lstContHar.Add(fecic.radioButton4.Checked ? "0" : "1");


            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEnterContactersToImportCount_3" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"), lstContHar, Encoding.UTF8);
        }

        List<String> lstContHarContacterGroups = new List<String>();
        List<String> lstFilterHarContacterGroups = new List<String>();

        private void doImportContactersFromGroup()
        {
            foreach (String sUID in hashsetImportedGroupList)
                sImportIDsContacterGroups += sUID + "|";

            StartImportGroupsThread("GroupAACM=" + sImportIDsContacterGroups, lstContHarContacterGroups, lstFilterHarContacterGroups, sPersUIDsContacterGroups, nudContacterCountMembers, false, null, nudContacterDuplicatesAction);
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

        private void masterCancelled()
        {
            MessageBox.Show("Выполнение мастера инициации диалога с Контактерами из тематических групп прервано по причине отсутствия требуемых данных!");
            DialogResult = DialogResult.Cancel;
        }

        private void SaveFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            for (int i = 0; i < iGroupHarCount; i++)
                lstContHar.Add(fe.sPersHar[i, iGroupHarAttrCount]);

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesGroups_3" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"), lstContHar, Encoding.UTF8);
        }

        private void LoadFormEditPersHarValues(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesGroups_3" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValuesGroups_3" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"));
                lstContHar = new List<String>(srcFile);
            }
            else if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValuesGroups_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
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

        private void SaveFormEditPersHarValuesContacters(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            for (int i = 0; i < mFormMain.iContHarCount; i++)
                lstContHar.Add(fe.sPersHar[i, mFormMain.iContHarAttrCount]);

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValues_3" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"), lstContHar, Encoding.UTF8);
        }

        private void LoadFormEditPersHarValuesContacters(FormEditPersHarValues fe, int importmode, int submode)
        {
            if (importmode < 0)
                return;

            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValues_3" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "FormEditPersHarValues_3" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values"));
                lstContHar = new List<String>(srcFile);
            }
            else if (File.Exists(Path.Combine(Application.StartupPath, "FormEditPersHarValues_" + Convert.ToString(submode) + "_" + Convert.ToString(importmode) + ".values")))
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

            if (bExpert)
            {
                if (fe.ShowDialog() != DialogResult.OK)
                    return null;

                SaveFormEditPersHarValues(fe, importmode, 0);
            }

            List<String> lstContHar = new List<String>();
            for (int i = 0; i < iGroupHarCount; i++)
                lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);
            return lstContHar;

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

            if (bExpert)
            {
                if (fe.ShowDialog() != DialogResult.OK)
                    return null;

                SaveFormEditPersHarValuesContacters(fe, importmode, 0);
            }

            List<String> lstContHar = new List<String>();
            for (int i = 0; i < mFormMain.iContHarCount; i++)
                lstContHar.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, mFormMain.iContHarAttrCount]);
            return lstContHar;

            return null;
        }

        private void DisableAllButtons()
        {
            //buttonStart.Enabled = false;
            buttonCancel.Enabled = false;
        }

        private void EnableAllButtons()
        {
            //buttonStart.Enabled = true;
            buttonCancel.Enabled = true;
        }

        private void StartImportGroupsThread(String sGID, List<String> lstContHar, List<String> lstFilterHar, String sPersUsersIDs, int maxGroupsCount, bool GroupsOrContacts, Dictionary<long, HashSet<long>> usersGroupIDs = null, int _nudDuplicatesAction = 0)
        {
            // This method runs on the main thread.
            this.pbProgress.Visible = true;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Value = 0;
            this.pbProgress.Maximum = 10;
            this.bDoImportGroups = false;

            DisableAllButtons();
            //button9.Enabled = true;

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
                WC.maxContacterCount = nudContacterCountAdministrators + nudContacterCountAuthors + nudContacterCountCommentators + nudContacterCountMembers;
                WC.nudDuplicatesAction = _nudDuplicatesAction;
                WC.groupSearchQuery = groupSearchQuery;
                WC.nudContacterCountMembers = nudContacterCountMembers;
                WC.nudContacterCountAdministrators = nudContacterCountAdministrators;
                WC.nudContacterCountAuthors = nudContacterCountAuthors;
                WC.nudContacterCountCommentators = nudContacterCountCommentators;
                WC.nudNeedContacterMultiplicator = nudNeedContacterMultiplicator;
                // Start the asynchronous operation.
                bwProgress.RunWorkerAsync(WC);
            }
        }

        private string groupSearchQuery = "";
        int groupsTargetCount = 0;
        int groupsTargetDuplicatesAction = 0;

        private bool startImportGroups()
        {
            sPersUIDsGroupsImport = iPersUserID.ToString() + "|";

            String value = "";
            if (FormMain.InputBox(this, "Импорт групп через Поиск", "Тематика групп:", ref value) == DialogResult.OK)
            {
                groupSearchQuery = value;
                iImportMode = 6;

                FormEnterContactersToImportCount fecic = new FormEnterContactersToImportCount();
                fecic.Setup(sPersUIDsGroupsImport.Split('|').Length - 1, 1);
                LoadFormEnterContactersToImportCount(fecic, iImportMode, 1);

                fecic.groupBox2.Visible = false;
                fecic.groupBox3.Visible = true;

                if (bExpert)
                {
                    if (fecic.ShowDialog() != DialogResult.OK)
                        return false;

                    SaveFormEnterContactersToImportCount(fecic, iImportMode, 1);
                }

                groupsTargetCount = (int)fecic.nudContacterCount.Value;
                groupsTargetDuplicatesAction = fecic.radioButton5.Checked ? 0 : 1;

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
                fe.Text = "Редактирование значений характеристик групп";

                if (bExpert)
                {
                    if (fe.ShowDialog() != DialogResult.OK)
                        return false;

                    SaveFormEditPersHarValues(fe, iImportMode, 1);
                }

                lstContHarGroupsImport = new List<String>();
                for (int i = 0; i < iGroupHarCount; i++)
                    lstContHarGroupsImport.Add(fe.sPersHar[i, 0] + "|" + fe.sPersHar[i, iGroupHarAttrCount]);
                lstFilterHarGroupsImport = GetHarValues("Фильтр значений характеристик групп", iImportMode);

                if (lstFilterHarGroupsImport != null)
                    return true;
            }
            return false;
        }

        List<String> lstContHarGroupsImport = new List<String>();
        List<String> lstFilterHarGroupsImport = new List<String>();
        String sPersUIDsGroupsImport = "";
        private void doImportGroups()
        {
            StartImportGroupsThread("Search=" + groupSearchQuery, lstContHarGroupsImport, lstFilterHarGroupsImport, sPersUIDsGroupsImport, groupsTargetCount, true, null, groupsTargetDuplicatesAction);
        }

        List<String> lstExternalContactsList = new List<String>();
        String currentExternalContactsPersonenID = "";
        HashSet<String> hashsetImportedContactsList = new HashSet<String>();

        public void ExternalContactsList_AddUser(String ExternalPersonenID, String sUD, String sUName)
        {
            if (!currentExternalContactsPersonenID.Equals(ExternalPersonenID))
            {
                currentExternalContactsPersonenID = ExternalPersonenID;
                lstExternalContactsList = new List<String>();

                if (File.Exists(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt"));
                    lstExternalContactsList = new List<String>(srcFile);
                }

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

            hashsetImportedContactsList.Add(sUD);

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_contacts_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt"), lstExternalContactsList, Encoding.UTF8);
        }


        List<String> lstExternalGroupList = new List<String>();
        HashSet<String> hashsetImportedGroupList = new HashSet<String>();
        String currentExternalGroupsPersonenID = "";

        public void ExternalGroupsList_Add(String ExternalPersonenID, String sUD, String sUName)
        {
            if (!currentExternalGroupsPersonenID.Equals(ExternalPersonenID))
            {
                currentExternalGroupsPersonenID = ExternalPersonenID;
                lstExternalGroupList = new List<String>();
                if (File.Exists(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt"));
                    lstExternalGroupList = new List<String>(srcFile);
                }
            }

            int iuserIdx = -1;
            for (int i = 0; i < lstExternalGroupList.Count; i++)
            {
                String str = lstExternalGroupList[i];
                if (str.Substring(0, str.IndexOf("|")).Equals(sUD))
                {
                    iuserIdx = i;
                    break;
                }
            }

            String userRec = sUD + "|" + sUName;
            if (iuserIdx >= 0)
                lstExternalGroupList[iuserIdx] = userRec;
            else
                lstExternalGroupList.Add(userRec);

            hashsetImportedGroupList.Add(sUD);

            File.WriteAllLines(Path.Combine(FormMain.sDataPath, "_groups_" + FormMain.getSocialNetworkPrefix() + ExternalPersonenID + ".txt"), lstExternalGroupList, Encoding.UTF8);
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

                if (stateImportedContacts.ImportPhase == 0)
                {
                    pbProgress.Text = stateImportedContacts.sUName;
                    labelData.Text = "Отбор контактеров из групп для импорта по категориям...\n\nОтобрано в категории: " + stateImportedContacts.ContactsImported.ToString() + "/" + stateImportedContacts.ContactsTotal.ToString() + "\n\nОбработка:\n" + pbProgress.Text;

                }
                else
                {
                    String _role = "Неизвестно";
                    int _target = 0;
                    if (stateImportedContacts.ImportPhase == 1)
                    {
                        _role = "Администраторы групп";
                        _target = nudContacterCountAdministrators;
                    }
                    else if (stateImportedContacts.ImportPhase == 2)
                    {
                        _role = "Автор записей";
                        _target = nudContacterCountAuthors;
                    }
                    else if (stateImportedContacts.ImportPhase == 3)
                    {
                        _role = "Комментатор записей";
                        _target = nudContacterCountCommentators;
                    }
                    else if (stateImportedContacts.ImportPhase == 4)
                    {
                        _role = "Участник группы";
                        _target = nudContacterCountMembers;
                    }

                    pbProgress.Text = stateImportedContacts.sUName + (stateImportedContacts.dtLastSeen.Length > 0 ? (" (" + stateImportedContacts.dtLastSeen + ")") : "");
                    labelData.Text = "Импорт контактов категории " + _role + "\n\nИмпортировано: " + stateImportedContacts.ContactsAdded.ToString() + "/" + _target.ToString() + "\n\nОбработано: " + stateImportedContacts.ContactsImported.ToString() + "/" + stateImportedContacts.ContactsTotal.ToString() + "\n\nОбработка:\n" + pbProgress.Text;

                    if (stateImportedContacts.ContactsAdded == 1)
                        buttonStart.Text = "Достаточно";
                }

            }
            else if (e.UserState is ImportGroups.CurrentState)
            {
                stateImported = (ImportGroups.CurrentState)e.UserState;
                if (this.pbProgress.Maximum != stateImported.GroupsTotal)
                    this.pbProgress.Maximum = stateImported.GroupsTotal;

                this.pbProgress.Value = stateImported.GroupsImported;
                this.pbProgress.Text = stateImported.sUName;

                labelData.Text = "Импортировано: " + stateImported.GroupsAdded.ToString() + "/" + groupsTargetCount.ToString() + "\n\nОбработано: " + stateImported.GroupsImported.ToString() + "/" + stateImported.GroupsTotal.ToString() + "\n\nВ обработке:\n" + stateImported.sUName;

                if (stateImported.GroupsAdded == 1)
                    buttonStart.Text = "Достаточно";
            }
        }

        private void bwProgress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.pbProgress.Visible = false;
            this.bDoImportGroups = true;
            this.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_21", this.Name, "Редактирование базы Групп");
            EnableAllButtons();
            nextStep();

            //if (e.Error != null)
            //    MessageBox.Show("Error: " + e.Error.Message, NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Группы"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            //else if (e.Cancelled)
            //{
            //    if (stateImported != null)
            //        MessageBox.Show((stateImported != null ? ("Импортировано " + stateImported.GroupsAdded.ToString() + " групп\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_23", this.Name, "Импорт данных Групп прерван. Если какие-то из Группы не отображаются в списке, смените настройки фильтра базы Групп по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Групп"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    else if (stateImportedContacts != null)
            //        MessageBox.Show((stateImportedContacts != null ? ("Импортировано " + stateImportedContacts.ContactsAdded.ToString() + " контактеров\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_23", this.Name, "Импорт данных Контактеров прерван. Если какие-то из Контактеров не отображаются в списке, смените настройки фильтра базы Контактеров по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            //else
            //{
            //    if (stateImported != null)
            //        MessageBox.Show((stateImported != null ? ("Импортировано " + stateImported.GroupsAdded.ToString() + " групп\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_24", this.Name, "Импорт данных Групп завершен. Если какие-то из Группы не отображаются в списке, смените настройки фильтра базы Групп по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Групп"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    else if (stateImportedContacts != null)
            //        MessageBox.Show((stateImportedContacts != null ? ("Импортировано " + stateImportedContacts.ContactsAdded.ToString() + " контактеров\n\n") : "") + NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_24", this.Name, "Импорт данных Контактеров завершен. Если какие-то из Контактеров не отображаются в списке, смените настройки фильтра базы Контактеров по характеристикам..."), NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_22", this.Name, "Импорт данных Контактеров"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }
    }
}
