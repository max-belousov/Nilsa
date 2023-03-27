using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nilsa
{
    public partial class FormColumnsOrder : Form
    {
        public FormColumnsOrder()
        {
            InitializeComponent();
        }

        public void Setup()
        {
            listBoxColumns_SelectedIndexChanged(null, null);
        }

        private void listBoxColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonUp.Enabled = false;
            buttonDown.Enabled = false;

            if (listBoxColumns.SelectedIndex > 0)
                buttonUp.Enabled = true;

            if (listBoxColumns.SelectedIndex>=0 && (listBoxColumns.SelectedIndex < listBoxColumns.Items.Count - 1))
                buttonDown.Enabled = true;
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            int idx = listBoxColumns.SelectedIndex;
            listBoxColumns.Items.Insert(idx - 1, listBoxColumns.Items[idx]);
            listBoxColumns.Items.RemoveAt(idx+1);
            listBoxColumns.SelectedIndex = idx - 1;
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            int idx = listBoxColumns.SelectedIndex;
            listBoxColumns.Items.Insert(idx + 2, listBoxColumns.Items[idx]);
            listBoxColumns.Items.RemoveAt(idx);
            listBoxColumns.SelectedIndex = idx+1;
        }
    }
}
