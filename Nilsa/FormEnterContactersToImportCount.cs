using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nilsa
{
    public partial class FormEnterContactersToImportCount : Form
    {
        int iPersonenCount;
        int iMode;

        public FormEnterContactersToImportCount()
        {
            InitializeComponent();


        }

        public void Setup(int _iPersonenCount, int _iMode)
        {
            iMode = _iMode;
            iPersonenCount= _iPersonenCount;
            tbPersonenCount.Text = iPersonenCount.ToString();
            String sObj = "";
            if (iMode == 0)
                sObj = "Контактеров";
            else if (iMode == 1)
                sObj = "Групп";
            else if (iMode == 2)
            {
                sObj = "Участиников групп";
                radioButton2.Checked = true;
                label4.Visible = true;
                numericUpDown1.Visible = true;
                label5.Visible = true;
                numericUpDown2.Visible = true;
                label6.Visible = true;
                numericUpDown3.Visible = true;
                label7.Visible = true;
                numericUpDown4.Visible = true;
            }
            Text = "Импорт "+ sObj;
            groupBox1.Text = "Число "+ sObj + " для импорта";
            label2.Text = "Число " + sObj + " для импорта:";
            label3.Text = "Число " + sObj + " на одного Персонажа:";
            nudContacterCount_ValueChanged(null, null);
        }

        public void nudContacterCount_ValueChanged(object sender, EventArgs e)
        {
            if (iMode == 2)
                tbContacterPerPerson.Text = iPersonenCount > 0 ? ("Уч.: "+((int)nudContacterCount.Value / iPersonenCount).ToString()+ ", Адм.: "+ ((int)numericUpDown1.Value / iPersonenCount).ToString() + ", Авт.: " + ((int)numericUpDown2.Value / iPersonenCount).ToString() + ", Ком.: " + ((int)numericUpDown3.Value / iPersonenCount).ToString()) : "0";
            else
                tbContacterPerPerson.Text = iPersonenCount > 0 ? ((int)nudContacterCount.Value / iPersonenCount).ToString() : "0";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            SelectNextControl(ActiveControl !=null ? ActiveControl: tbContacterPerPerson, true, true, true, true);
            Thread.Sleep(500);
            DialogResult = DialogResult.OK;
        }
    }
}
