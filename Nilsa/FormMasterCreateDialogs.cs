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
    public partial class FormMasterCreateDialogs : Form
    {
        FormMain mFormMain;

        public FormMasterCreateDialogs(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();
        }

        public void Setup()
        {
            button2.Enabled = false;
        }

        private void validateData()
        {
            bool enabled = true;
            enabled = textBoxProjectName.Text.Trim().Length > 0;

            if (enabled)
                enabled=!checkBox1.Checked || textBox1.Text.Trim().Length > 0;

            if (enabled)
                enabled = !checkBox2.Checked || textBox2.Text.Trim().Length > 0;

            if (enabled)
                enabled = !checkBox3.Checked || textBox3.Text.Trim().Length > 0;

            if (enabled)
                enabled = textBox4.Text.Trim().Length > 0;

            if (enabled)
                enabled = textBox5.Text.Trim().Length > 0;

            if (enabled)
                enabled = !checkBox4.Checked || textBox6.Text.Trim().Length > 0;

            if (enabled)
                enabled = textBox7.Text.Trim().Length > 0;

            if (enabled)
                enabled = !checkBox5.Checked || textBox8.Text.Trim().Length > 0;

            if (enabled)
                enabled = !checkBox6.Checked || textBox9.Text.Trim().Length > 0;

            if (enabled)
                enabled = textBox10.Text.Trim().Length > 0;

            if (enabled)
                enabled = !checkBox7.Checked || textBox11.Text.Trim().Length > 0;

            button2.Enabled = enabled;
        }

        private void textBoxProjectName_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox1.Enabled = true;
            else
                textBox1.Enabled = false;

            validateData();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                textBox2.Enabled = true;
            else
                textBox2.Enabled = false;

            validateData();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                textBox3.Enabled = true;
            else
                textBox3.Enabled = false;

            validateData();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
                textBox6.Enabled = true;
            else
                textBox6.Enabled = false;

            validateData();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
                textBox8.Enabled = true;
            else
                textBox8.Enabled = false;

            validateData();
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                textBox9.Enabled = true;
            else
                textBox9.Enabled = false;

            validateData();
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
                textBox11.Enabled = true;
            else
                textBox11.Enabled = false;

            validateData();
        }
    }
}
