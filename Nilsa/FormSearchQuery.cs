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
    public partial class FormSearchQuery : Form
    {
        FormMain mFormMain;
        public String stringResult = "";

        public FormSearchQuery(FormMain _formmain)
        {
            mFormMain = _formmain;
            InitializeComponent();

            NilsaUtils.Dictonary_ApplyAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictonary_AddAllControlsText(mFormMain.userInterface, this, this.Name);
            NilsaUtils.Dictionary_Save(mFormMain.userInterface, Path.Combine(Application.StartupPath, "UserInterface." + mFormMain.CurrentLanguage + ".lng"));
        }

        public void Setup()
        {
            checkedListBox1.SelectedIndex = 0;
            checkedListBox2.SelectedIndex = 0;
            comboBox1.Items.Clear();
            /*
            do
            {
                try
                {
                    comboBox1.Items.Clear();
                    
                    var countries = FormMain.api.Database.GetCountries(true, "", 1000);
                    foreach (VkNet.Model.Country country in countries)
                        comboBox1.Items.Add(country);
                    break;
                }
                catch (Exception e)
                {
                    mFormMain.ExceptionToLogList("FormSearchQuery", "Setup", e);
                    break;
                }
            }
            while (true);
            */
            comboBox2.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBox3.Checked = false;
            if (comboBox1.SelectedIndex >= 0)
            {
                comboBox2.Items.Clear();

                if (comboBox1.SelectedItem == null)
                {
                    comboBox2.Enabled = false;
                    comboBox2.Items.Clear();
                }
                else
                {
                    VkNet.Model.Country country = (VkNet.Model.Country)comboBox1.SelectedItem;
                    if (country == null)
                    {
                        comboBox2.Enabled = false;
                        comboBox2.Items.Clear();
                    }
                    else
                    {
                        checkBox3.Checked = true;

                        if (comboBox2.Items.Count > 1)
                            comboBox2.Enabled = true;
                        else
                            comboBox2.Enabled = false;
                        /*
                    do
                    {
                        try
                        {
                            checkBox3.Checked = true;
                            var cities = FormMain.api.Database.GetCities(country.Id, null, "", true, 1000);
                            foreach (VkNet.Model.City city in cities)
                                comboBox2.Items.Add(city);

                            if (comboBox2.Items.Count > 1)
                                comboBox2.Enabled = true;
                            else
                                comboBox2.Enabled = false;
                            break;
                        }
                        catch (Exception exp)
                        {
                            mFormMain.ExceptionToLogList("FormSearchQuery", "Setup", exp);
                            break;
                        }
                    } while (true);
                    */
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            VkNet.Model.Country country = null;
            VkNet.Model.City city = null;
            if (comboBox1.SelectedItem != null)
            {
                country = (VkNet.Model.Country)comboBox1.SelectedItem;
                if (country != null)
                {
                    if (comboBox2.SelectedItem != null)
                    {
                        city = (VkNet.Model.City)comboBox2.SelectedItem;
                    }
                }
            }

            long lGroupID = -1;
            try
            {
                lGroupID = Convert.ToUInt32(textBox4.Text);
            }
            catch
            {

            }

            stringResult = textBox1.Text + "|" + checkedListBox1.SelectedIndex.ToString() + "|" + (checkBox1.Checked ? "1" : "0") + "|" + (checkBox2.Checked ? "1" : "0") + "|" + (country == null ? "null" : country.Id.ToString()) + "|" + (city == null ? "null" : city.Id.ToString()) + "|" + checkedListBox2.SelectedIndex.ToString() + "|" + textBox2.Text + "|" + textBox3.Text + "|" + lGroupID.ToString() + "|" + textBox5.Text + "|" + textBox6.Text + "|" + textBox7.Text + "|" + textBox8.Text + "|";
            DialogResult = DialogResult.OK;
        }

        private bool bCitiesUpdate = false;
        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            if (bCitiesUpdate)
                return;
            checkBox4.Checked = false;
            bCitiesUpdate = true;
            if (comboBox1.SelectedIndex >= 0)
            {
                if (comboBox1.SelectedItem == null)
                {
                    comboBox2.Enabled = false;
                    comboBox2.Items.Clear();
                }
                else
                {
                    comboBox2.Items.Clear();

                    VkNet.Model.Country country = (VkNet.Model.Country)comboBox1.SelectedItem;
                    if (country == null)
                    {
                        comboBox2.Enabled = false;
                        comboBox2.Items.Clear();
                    }
                    else
                    {
                        comboBox2.Items.Clear();
                        comboBox2.SelectionLength = 0;
                        comboBox2.SelectionStart = comboBox2.Text.Length;
                        if (comboBox2.SelectedItem != null)
                        {
                            VkNet.Model.City city = (VkNet.Model.City)comboBox2.SelectedItem;
                            if (city != null)
                                checkBox4.Checked = true;
                        }

                        /*
                        do
                        {
                            try
                            {
                                comboBox2.Items.Clear();
                                var cities = FormMain.api.Database.GetCities(country.Id, null, comboBox2.Text, true, 1000);
                                foreach (VkNet.Model.City city in cities)
                                    comboBox2.Items.Add(city);
                                comboBox2.SelectionLength = 0;
                                comboBox2.SelectionStart = comboBox2.Text.Length;
                                if (comboBox2.SelectedItem != null)
                                {
                                    VkNet.Model.City city = (VkNet.Model.City)comboBox2.SelectedItem;
                                    if (city != null)
                                        checkBox4.Checked = true;
                                }
                                break;
                            }
                            catch (Exception exp)
                            {
                                mFormMain.ExceptionToLogList("FormSearchQuery", "Setup", exp);
                                break;
                            }
                        } while (true);
                        */
                    }
                }
            }
            bCitiesUpdate = false;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBox4.Checked = false;
            VkNet.Model.Country country = null;
            VkNet.Model.City city = null;
            if (comboBox1.SelectedItem != null)
            {
                country = (VkNet.Model.Country)comboBox1.SelectedItem;
                if (country != null)
                {
                    if (comboBox2.SelectedItem != null)
                    {
                        city = (VkNet.Model.City)comboBox2.SelectedItem;
                        if (city != null)
                            checkBox4.Checked = true;
                    }
                }
            }
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            comboBox2_SelectedIndexChanged(sender, e);
        }
    }
}
