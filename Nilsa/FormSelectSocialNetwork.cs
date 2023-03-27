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
    public partial class FormSelectSocialNetwork : Form
    {
        public FormSelectSocialNetwork()
        {
            InitializeComponent();
            cbSocialNetwork.SelectedIndex = 0;
            checkBox1.Visible = false;
            checkBox1.Checked = false;
        }

        public void Setup(bool bIgnoreExistsVisible)
        {
            checkBox1.Visible = bIgnoreExistsVisible;
            checkBox1.Checked = bIgnoreExistsVisible;
        }

        public bool IgnoreExists()
        {
            return checkBox1.Checked;
        }

    }
}
