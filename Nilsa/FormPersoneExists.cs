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
    public partial class FormPersoneExists : Form
    {
        FormMain mFormMain;

        public FormPersoneExists()
        {
            InitializeComponent();
        }

        public void Setup(FormMain _mFormMain, string _persid, string _contid, string _contname, int iFormMode=0) // iFormMode=0 - импорт контактеров, 1 - поиск дубликатов
        {
            mFormMain = _mFormMain;

            labelContact.Text = _contname;
            labelPersone.Text = mFormMain.PersonenList_GetUserField(_persid, 1);

            string sAttrFileName = "cont_" + FormMain.getSocialNetworkPrefix() + _persid + "_" + _contid + ".txt";
            if (File.Exists(Path.Combine(FormMain.sDataPath, sAttrFileName)))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName));
                List<String> lstContHarValues = new List<String>(srcFile);
                ListViewItem lvi;
                for (int i = 0; i < mFormMain.iContHarCount; i++)
                {
                    String sValue = lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1);
                    lvi = new ListViewItem(mFormMain.sContHar[i,1]);
                    lvi.SubItems.Add(sValue);
                    listViewContact.Items.Add(lvi);
                }
            }

            sAttrFileName = "pers_" + FormMain.getSocialNetworkPrefix() + _persid + ".txt";
            if (File.Exists(Path.Combine(FormMain.sDataPath, sAttrFileName)))
            {
                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName));
                List<String> lstContHarValues = new List<String>(srcFile);
                ListViewItem lvi;
                for (int i = 0; i < mFormMain.iPersHarCount; i++)
                {
                    String sValue = lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1);
                    lvi = new ListViewItem(mFormMain.sPersHar[i, 1]);
                    lvi.SubItems.Add(sValue);
                    listViewPersone.Items.Add(lvi);
                }
            }

            if (iFormMode==1)
            {
                label1.Text = "Найден дубликат Контактера";
                this.Text = "Поиск дубликатов";
                buttonChange.Text = "Пометить";
            }
        }
    }
}
