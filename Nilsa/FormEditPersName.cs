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
    public partial class FormEditPersName : Form
    { 
        public FormEditPersName(string inputFirstName = "", string inputLastName = "", string inputPhotoUrl = "")
        {
            InitializeComponent();
            currentFirstNameLabel.Text = inputFirstName;
            currentLastNameLabel.Text = inputLastName;
            try
            {
                //подставляем фотку
                //photoPersURL = localphotoPersURL;
                var request = WebRequest.Create(inputPhotoUrl);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var bitmapPicture = Bitmap.FromStream(stream);
                    photoPictureBox.Image = bitmapPicture;
                }
            }
            catch (Exception) { }
            Persone = new PersoneAllData();
            Persone.FirstName = inputFirstName;
            Persone.LastName = inputLastName;
            Persone.PhotoUrl = inputPhotoUrl;
            newFirstNameTextBox.DataBindings.Add(new Binding("Text", Persone, "FirstName", false, DataSourceUpdateMode.OnPropertyChanged));
            newLastNameTextBox.DataBindings.Add(new Binding("Text", Persone, "LastName", false, DataSourceUpdateMode.OnPropertyChanged));
            newPhotoUrlTextBox.DataBindings.Add(new Binding ("Text", Persone, "PhotoUrl", false, DataSourceUpdateMode.OnPropertyChanged));
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
