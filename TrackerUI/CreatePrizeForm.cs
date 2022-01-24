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

namespace TrackerUI
{
    public partial class CreatePrizeForm : Form
    {
        public CreatePrizeForm()
        {
            InitializeComponent();
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidadeForm())
            {
                PrizeModel model = new PrizeModel(
                    placeNameText.Text, 
                    placeNumberText.Text,
                    prizeAmountText.Text,
                    prizePercentageText.Text);

                foreach (IDataConnection db in GlobalConfig.Connections) 
                {
                    db.CreatePrize(model);
                }

                placeNameText.Text = "";
                placeNumberText.Text = "";
                prizeAmountText.Text = "0";
                prizePercentageText.Text = "0";
            }
            else
            {
                MessageBox.Show("This Form has invalid information. Please check it and try again.");
            }
        }

        private bool ValidadeForm() 
        {
            bool output = true;
            int placeNumber = 0;
            bool placeNumberValidNumber = int.TryParse(placeNumberText.Text, out placeNumber);

            if (placeNumberValidNumber == false)
            {
                output = false;
            }

            if (placeNumber < 1)
            {
                output = false;
            }

            if (placeNameText.Text.Length == 0)
            {
                output |= false;
            }

            decimal prizeAmount = 0;
            double prizePercentage = 0;

            bool prizeAmountValid = decimal.TryParse(prizeAmountText.Text, out prizeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageText.Text, out prizePercentage);

            if (prizeAmountValid == false || prizePercentageValid == false) 
            {
                output = false;
            }

            if (prizeAmount <= 0 && prizePercentage <= 0)
            { 
                output = false;
            }

            if (prizePercentage < 0 || prizePercentage > 100)
            {
                output = false;
            }

            return output;
        }
    }
}
