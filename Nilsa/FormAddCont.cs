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
            cidTextBox.DataBindings.Add(new Binding ("Text", Persone, "Login", false, DataSourceUpdateMode.OnPropertyChanged));


            // Добавляем обработчики событий TextChanged
            cidTextBox.TextChanged += CheckFieldsValidity;

            // Проверяем валидность полей при запуске формы
            CheckFieldsValidity(null, EventArgs.Empty);
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

        private void CheckFieldsValidity(object sender, EventArgs e)
        {
            // Проверяем, заполнены ли оба поля
            bool isValid = !string.IsNullOrEmpty(cidTextBox.Text);

            // Устанавливаем свойство Enabled кнопки "OK" в соответствии с валидностью полей
            okButton.Enabled = isValid;
        }
    }
}
