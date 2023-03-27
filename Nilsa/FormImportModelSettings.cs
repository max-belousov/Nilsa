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
    public partial class FormImportModelSettings : Form
    {
        FormMain mFormMain;

        const int OPTIONS_COUNT = 18;
        ComboBox[] comboBox = new ComboBox[OPTIONS_COUNT];
        int Mode;
        public int[] Settings = new int[OPTIONS_COUNT + 1];
        public bool[] ExistsSettings = new bool[OPTIONS_COUNT];
        public bool[] InitSettings = new bool[OPTIONS_COUNT];
        bool SetupDone = false;

        public FormImportModelSettings(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));

            comboBox[0] = comboBox1;
            comboBox[1] = comboBox2;
            comboBox[2] = comboBox3;
            comboBox[3] = comboBox4;
            comboBox[4] = comboBox5;
            comboBox[5] = comboBox6;
            comboBox[6] = comboBox7;
            comboBox[7] = comboBox8;
            comboBox[8] = comboBox9;
            comboBox[9] = comboBox10;
            comboBox[10] = comboBox11;
            comboBox[11] = comboBox12;
            comboBox[12] = comboBox13;
            comboBox[13] = comboBox14;
            comboBox[14] = comboBox15;
            comboBox[15] = comboBox16;
            comboBox[16] = comboBox17;
            comboBox[17] = comboBox18;
        }

        public void Setup(int _mode = 0)
        {
            Mode = _mode;

            for (int i = 0; i < OPTIONS_COUNT; i++)
            {
                ExistsSettings[i] = true;
                InitSettings[i] = true;
            }

            if (Mode == 0)
            {
                List<String> lstInitList = new List<String>();
                if (File.Exists(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings")))
                {
                    var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_importexport_model_contains.settings"));
                    lstInitList = new List<String>(srcFile);
                    if (lstInitList.Count == OPTIONS_COUNT + 1)
                    {
                        for (int i = 0; i < OPTIONS_COUNT; i++)
                            InitSettings[i] = Convert.ToInt32(lstInitList[i]) == 1;
                    }
                }
            }

            if (_mode == 0)
            {
                Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_1", this.Name, "Импорт информационной модели");
                groupBox1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_2", this.Name, "Настройка импорта информационной модели");
                button1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_3", this.Name, "Импортировать");
                for (int i = 0; i < OPTIONS_COUNT; i++)
                {
                    comboBox[i].Items.Clear();
                    comboBox[i].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Не менять"));
                    comboBox[i].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Очистить"));
                    if (i == 0 || i == 1 || i == 3 || i >= 11)
                        if (InitSettings[i])
                            comboBox[i].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "Импортировать"));
                }
            }
            else
            {
                checkBox1.Visible = false;
                Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_7", this.Name, "Экспорт информационной модели");
                groupBox1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_8", this.Name, "Настройка экспорта информационной модели");
                button1.Text = NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_9", this.Name, "Экспортировать");
                for (int i = 0; i < OPTIONS_COUNT; i++)
                {
                    comboBox[i].Items.Clear();
                    comboBox[i].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_10", this.Name, "Не экспортировать"));
                    comboBox[i].Items.Add(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_11", this.Name, "Экспортировать"));
                }

            }

            LoadImportExportSettings();
            SetupDone = true;
            comboBox1_SelectedIndexChanged(null, null);
            comboBox2_SelectedIndexChanged(null, null);
            comboBox4_SelectedIndexChanged(null, null);
            comboBox8_SelectedIndexChanged(null, null);
        }

        private void LoadImportExportSettings()
        {
            for (int i = 0; i < OPTIONS_COUNT + 1; i++)
                Settings[i] = (Mode == 1 ? 1 : (i == OPTIONS_COUNT ? 1 : ((i == 0 || i == 1 || i == 3 || i >= 11) ? (InitSettings[i] ? 2 : 0):0)));

            List<String> lstList = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "_importexport_model_" + Mode.ToString() + ".settings")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "_importexport_model_" + Mode.ToString() + ".settings"));
                lstList = new List<String>(srcFile);
            }

            if (lstList.Count == OPTIONS_COUNT + 1)
            {
                for (int i = 0; i < OPTIONS_COUNT + 1; i++)
                {
                    Settings[i] = Convert.ToInt32(lstList[i]);
                }
            }

            for (int i = 0; i < OPTIONS_COUNT; i++)
                comboBox[i].SelectedIndex = Settings[i];

            checkBox1.Checked = Settings[OPTIONS_COUNT] == 1;

        }

        private void SaveImportExportSettings()
        {
            for (int i = 0; i < OPTIONS_COUNT; i++)
            {
                if (Mode == 1)
                    Settings[i] = comboBox[i].SelectedIndex;
                else
                {
                    if (comboBox[i].Items[comboBox[i].SelectedIndex].Equals(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_4", this.Name, "Не менять")))
                        Settings[i] = 0;
                    else if (comboBox[i].Items[comboBox[i].SelectedIndex].Equals(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_5", this.Name, "Очистить")))
                        Settings[i] = 1;
                    else if (comboBox[i].Items[comboBox[i].SelectedIndex].Equals(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_6", this.Name, "Импортировать")))
                        Settings[i] = 2;
                    else 
                        Settings[i] = -1;
                }
            }
            Settings[OPTIONS_COUNT] = checkBox1.Checked ? 1 : 0;

            if (Mode == 1)
            {
                List<String> lstList = new List<String>();
                for (int i = 0; i < OPTIONS_COUNT + 1; i++)
                    lstList.Add(Settings[i].ToString());

                File.WriteAllLines(Path.Combine(Application.StartupPath, "_importexport_model_" + Mode.ToString() + ".settings"), lstList, Encoding.UTF8);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveImportExportSettings();
            bool NotAction = true;
            for (int i = 0; i < OPTIONS_COUNT; i++)
                if (Settings[i] != 0)
                {
                    NotAction = false;
                    break;
                }
            if (NotAction)
            {
                MessageBox.Show(NilsaUtils.Dictonary_GetText(mFormMain.userInterface, "messageboxText_12", this.Name, "Не выбрана ни одна операция ни для одного пункта настроек. Выполнение импорта/экспорта не будет произведено..."), Text);
                DialogResult = DialogResult.Cancel;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SetupDone && comboBox1.SelectedIndex >= 0)
            {
                if (Mode == 0)
                {
                    ExistsSettings[2] = (/*comboBox1.SelectedIndex == comboBox2.SelectedIndex && */comboBox2.SelectedIndex != 1);
                    ExistsSettings[5] = (/*comboBox1.SelectedIndex == comboBox4.SelectedIndex &&*/comboBox4.SelectedIndex != 1 && comboBox1.SelectedIndex != 1);
                    ExistsSettings[6] = (/*comboBox1.SelectedIndex == comboBox2.SelectedIndex && *//*comboBox1.SelectedIndex == comboBox4.SelectedIndex &&*/comboBox2.SelectedIndex != 1 && comboBox4.SelectedIndex != 1 && comboBox8.SelectedIndex == 0 && comboBox1.SelectedIndex != 1);
                    ExistsSettings[8] = (/*comboBox1.SelectedIndex == comboBox4.SelectedIndex &&*/ comboBox4.SelectedIndex != 1 && comboBox8.SelectedIndex == 0 && comboBox1.SelectedIndex != 1);

                    if (ExistsSettings[2])
                        comboBox[2].Items[0] = comboBox2.Items[comboBox2.SelectedIndex];
                    if (ExistsSettings[5])
                        comboBox[5].Items[0] = comboBox1.Items[comboBox1.SelectedIndex];
                    if (ExistsSettings[6])
                        comboBox[6].Items[0] = comboBox1.Items[comboBox1.SelectedIndex];
                    if (ExistsSettings[8])
                        comboBox[8].Items[0] = comboBox1.Items[comboBox1.SelectedIndex];

                    List<ComboBox> _optcombo = new List<ComboBox>();
                    _optcombo.Add(comboBox3);
                    _optcombo.Add(comboBox6);
                    _optcombo.Add(comboBox7);
                    _optcombo.Add(comboBox9);
                    EnableDisableOptions(_optcombo, 0, 1);
                }
                else
                {
                    ExistsSettings[2] = (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 1);
                    ExistsSettings[5] = (comboBox1.SelectedIndex == 1 && comboBox4.SelectedIndex == 1);
                    ExistsSettings[6] = (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 1 && comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);
                    ExistsSettings[8] = (comboBox1.SelectedIndex == 1 && comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);

                    if (comboBox1.SelectedIndex == 0)
                    {
                        List<ComboBox> _optcombo = new List<ComboBox>();
                        _optcombo.Add(comboBox3);
                        _optcombo.Add(comboBox6);
                        _optcombo.Add(comboBox7);
                        _optcombo.Add(comboBox9);
                        DisableOptions(_optcombo);
                    }
                    else
                    {
                        List<ComboBox> _optcombo = new List<ComboBox>();
                        _optcombo.Add(comboBox3);
                        _optcombo.Add(comboBox6);
                        _optcombo.Add(comboBox7);
                        _optcombo.Add(comboBox9);
                        EnableOptions(_optcombo, 1);
                    }
                }
            }
        }

        private void DisableOptions(List<ComboBox> lstComboBox, int SelInx = 0)
        {
            for (int i = 0; i < lstComboBox.Count; i++)
            {
                if (lstComboBox[i].Enabled)
                {
                    lstComboBox[i].SelectedIndex = SelInx;
                    lstComboBox[i].Enabled = false;
                }
            }
        }

        private void EnableOptions(List<ComboBox> lstComboBox, int SelInx)
        {
            for (int i = 0; i < lstComboBox.Count; i++)
            {
                int iIdx = Convert.ToInt32(lstComboBox[i].Tag);
                if (!lstComboBox[i].Enabled && ExistsSettings[iIdx])
                {
                    lstComboBox[i].Enabled = true;
                    lstComboBox[i].SelectedIndex = SelInx;
                }
            }
        }

        private void EnableDisableOptions(List<ComboBox> lstComboBox, int SelInx, int iDisSelInd)
        {
            for (int i = 0; i < lstComboBox.Count; i++)
            {
                int iIdx = Convert.ToInt32(lstComboBox[i].Tag);
                if (ExistsSettings[iIdx])
                {
                    lstComboBox[i].Enabled = true;
                    lstComboBox[i].SelectedIndex = SelInx;
                }
                else
                {
                    lstComboBox[i].Enabled = false;
                    lstComboBox[i].SelectedIndex = iDisSelInd;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SetupDone && comboBox2.SelectedIndex >= 0)
            {
                if (Mode == 0)
                {
                    ExistsSettings[2] = (/*comboBox1.SelectedIndex == comboBox2.SelectedIndex && */comboBox2.SelectedIndex != 1);
                    ExistsSettings[6] = (/*comboBox1.SelectedIndex == comboBox2.SelectedIndex && *//*comboBox1.SelectedIndex == comboBox4.SelectedIndex &&*/ comboBox2.SelectedIndex != 1 && comboBox4.SelectedIndex != 1 && comboBox8.SelectedIndex == 0 && comboBox1.SelectedIndex != 1);

                    if (ExistsSettings[2])
                        comboBox[2].Items[0] = comboBox2.Items[comboBox2.SelectedIndex];
                    if (ExistsSettings[6])
                        comboBox[6].Items[0] = comboBox1.Items[comboBox1.SelectedIndex];


                    List<ComboBox> _optcombo = new List<ComboBox>();
                    _optcombo.Add(comboBox3);
                    _optcombo.Add(comboBox7);
                    EnableDisableOptions(_optcombo, 0, 1);

                }
                else
                {
                    ExistsSettings[2] = (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 1);
                    ExistsSettings[6] = (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 1 && comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);

                    if (comboBox2.SelectedIndex == 0)
                    {
                        List<ComboBox> _optcombo = new List<ComboBox>();
                        _optcombo.Add(comboBox3);
                        _optcombo.Add(comboBox7);
                        DisableOptions(_optcombo);
                    }
                    else
                    {
                        List<ComboBox> _optcombo = new List<ComboBox>();
                        _optcombo.Add(comboBox3);
                        _optcombo.Add(comboBox7);
                        EnableOptions(_optcombo, 1);
                    }
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SetupDone && comboBox4.SelectedIndex >= 0)
            {
                if (Mode == 0)
                {
                    ExistsSettings[4] = (comboBox4.SelectedIndex != 1);
                    ExistsSettings[5] = (/*comboBox1.SelectedIndex == comboBox4.SelectedIndex &&*/ comboBox4.SelectedIndex!=1 && comboBox1.SelectedIndex != 1);
                    ExistsSettings[6] = (/*comboBox1.SelectedIndex == comboBox2.SelectedIndex && *//*comboBox1.SelectedIndex == comboBox4.SelectedIndex &&*/comboBox2.SelectedIndex != 1 && comboBox4.SelectedIndex != 1 && comboBox8.SelectedIndex == 0 && comboBox1.SelectedIndex != 1);
                    ExistsSettings[7] = (comboBox4.SelectedIndex != 1);
                    ExistsSettings[8] = (/*comboBox1.SelectedIndex == comboBox4.SelectedIndex &&*/ comboBox4.SelectedIndex != 1 && comboBox8.SelectedIndex == 0 && comboBox1.SelectedIndex != 1);
                    ExistsSettings[9] = (comboBox8.SelectedIndex == 0 && comboBox4.SelectedIndex != 1);
                    ExistsSettings[10] = (comboBox8.SelectedIndex == 0 && comboBox4.SelectedIndex != 1);

                    for (int i=4; i<=10;i++)
                        if (ExistsSettings[i])
                            comboBox[i].Items[0] = comboBox4.Items[comboBox4.SelectedIndex];

                    List<ComboBox> _optcombo = new List<ComboBox>();
                    _optcombo.Add(comboBox5);
                    _optcombo.Add(comboBox6);
                    _optcombo.Add(comboBox7);
                    _optcombo.Add(comboBox8);
                    _optcombo.Add(comboBox9);
                    _optcombo.Add(comboBox10);
                    _optcombo.Add(comboBox11);
                    EnableDisableOptions(_optcombo, 0, 1);

                }
                else
                {
                    ExistsSettings[5] = (comboBox1.SelectedIndex == 1 && comboBox4.SelectedIndex == 1);
                    ExistsSettings[6] = (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 1 && comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);
                    ExistsSettings[8] = (comboBox1.SelectedIndex == 1 && comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);
                    ExistsSettings[9] = (comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);
                    ExistsSettings[10] = (comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);

                    if (comboBox4.SelectedIndex == 0)
                    {
                        List<ComboBox> _optcombo = new List<ComboBox>();
                        _optcombo.Add(comboBox5);
                        _optcombo.Add(comboBox6);
                        _optcombo.Add(comboBox7);
                        _optcombo.Add(comboBox8);
                        _optcombo.Add(comboBox9);
                        _optcombo.Add(comboBox10);
                        _optcombo.Add(comboBox11);
                        DisableOptions(_optcombo);
                    }
                    else
                    {
                        List<ComboBox> _optcombo = new List<ComboBox>();
                        _optcombo.Add(comboBox5);
                        _optcombo.Add(comboBox6);
                        _optcombo.Add(comboBox7);
                        _optcombo.Add(comboBox8);
                        _optcombo.Add(comboBox9);
                        _optcombo.Add(comboBox10);
                        _optcombo.Add(comboBox11);
                        EnableOptions(_optcombo, 1);
                    }
                }
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SetupDone && comboBox8.SelectedIndex >= 0)
            {
                if (Mode == 0)
                {
                    ExistsSettings[6] = (/*comboBox1.SelectedIndex == comboBox2.SelectedIndex && *//*comboBox1.SelectedIndex == comboBox4.SelectedIndex &&*/comboBox2.SelectedIndex != 1 && comboBox4.SelectedIndex != 1 && comboBox8.SelectedIndex == 0 && comboBox1.SelectedIndex != 1);
                    ExistsSettings[8] = (/*comboBox1.SelectedIndex == comboBox4.SelectedIndex && */comboBox4.SelectedIndex != 1 && comboBox8.SelectedIndex == 0 && comboBox1.SelectedIndex != 1);
                    ExistsSettings[9] = (comboBox8.SelectedIndex == 0 && comboBox4.SelectedIndex != 1);
                    ExistsSettings[10] = (comboBox8.SelectedIndex == 0 && comboBox4.SelectedIndex != 1);

                    if (ExistsSettings[6])
                        comboBox[6].Items[0] = comboBox8.Items[comboBox8.SelectedIndex];
                    for (int i = 8; i <= 10; i++)
                        if (ExistsSettings[i])
                            comboBox[i].Items[0] = comboBox8.Items[comboBox8.SelectedIndex];

                    List<ComboBox> _optcombo = new List<ComboBox>();
                    _optcombo.Add(comboBox7);
                    _optcombo.Add(comboBox9);
                    _optcombo.Add(comboBox10);
                    _optcombo.Add(comboBox11);
                    EnableDisableOptions(_optcombo, 0, 1);

                }
                else
                {
                    ExistsSettings[6] = (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 1 && comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);
                    ExistsSettings[8] = (comboBox1.SelectedIndex == 1 && comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);
                    ExistsSettings[9] = (comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);
                    ExistsSettings[10] = (comboBox4.SelectedIndex == 1 && comboBox8.SelectedIndex == 1);

                    if (comboBox8.SelectedIndex == 0)
                    {
                        List<ComboBox> _optcombo = new List<ComboBox>();
                        _optcombo.Add(comboBox7);
                        _optcombo.Add(comboBox9);
                        _optcombo.Add(comboBox10);
                        _optcombo.Add(comboBox11);
                        DisableOptions(_optcombo);
                    }
                    else
                    {
                        List<ComboBox> _optcombo = new List<ComboBox>();
                        _optcombo.Add(comboBox7);
                        _optcombo.Add(comboBox9);
                        _optcombo.Add(comboBox10);
                        _optcombo.Add(comboBox11);
                        EnableOptions(_optcombo, 1);
                    }
                }
            }
        }
    }
}
