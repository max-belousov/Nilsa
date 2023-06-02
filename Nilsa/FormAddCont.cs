using Nilsa.Data;
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
    public partial class FormAddCont : Form
    { 
        public FormAddCont()
        {
            InitializeComponent();
            Persone = new PersoneAllData();
            firstNameTextBox.DataBindings.Add(new Binding("Text", Persone, "FirstName", false, DataSourceUpdateMode.OnPropertyChanged));
            lastNameTextBox.DataBindings.Add(new Binding("Text", Persone, "LastName", false, DataSourceUpdateMode.OnPropertyChanged));
            ownerTextBox.DataBindings.Add(new Binding("Text", Persone, "Owner", false, DataSourceUpdateMode.OnPropertyChanged));
            cidTextBox.DataBindings.Add(new Binding ("Text", Persone, "Login", false, DataSourceUpdateMode.OnPropertyChanged));
        }
        public PersoneAllData Persone { get; set;  }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
