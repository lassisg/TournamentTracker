using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        public CreateTeamForm()
        {
            InitializeComponent();
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel person = new PersonModel();
                person.FirstName =  firstNameText.Text;
                person.LastName = lastNameText.Text;
                person.EmailAddress = emailText.Text;
                person.CellphoneNumber = cellphoneText.Text;

                GlobalConfig.Connection.CreatePerson(person);

                firstNameText.Text = "";
                lastNameText.Text = "";
                emailText.Text = "";
                cellphoneText.Text = "";
            }
            else
            {
                MessageBox.Show("You need to fill in all the fields.");
            }
        }

        private bool ValidateForm()
        {
            // TODO: Add validation to the form
            if (firstNameText.Text.Length == 0)
            {
                return false;
            }

            if (lastNameText.Text.Length == 0)
            {
                return false;
            }

            if (emailText.Text.Length == 0)
            {
                return false;
            }

            if (cellphoneText.Text.Length == 0)
            {
                return false;
            }

            return true;
        }
    }
}
