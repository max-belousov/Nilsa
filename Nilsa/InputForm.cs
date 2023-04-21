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
    public partial class InputForm : Form
    {
        // the InputBox
        //private InputForm newInputBox;
        // строка, которая будет возвращена форме запроса
        public  string returnString;

        public InputForm()
        {
            InitializeComponent();
        }
        public string Show(string inputBoxText)
        {
            //newInputBox = new InputForm();
            label1.Text = inputBoxText;
            //ShowDialog();
            return returnString;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            returnString = textBox1.Text;
            Dispose();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            returnString = string.Empty;
            Dispose();
        }
    }
}
