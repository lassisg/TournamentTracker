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
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();

        public CreateTournamentForm()
        {
            InitializeComponent();

            WireUpLists();
        }

        private void WireUpLists()
        {

            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;

            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);

                WireUpLists();
            }
        }

        private void removeSelectedPlayerButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentTeamsListBox.SelectedItem;

            if (t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);

                WireUpLists();
            }
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // Call the createPrizeForm
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();
        }

        public void PrizeComplete(PrizeModel model)
        {
            // Get back from the form with PrizeModel
            // Take the PrizeModel and put it into our list os selected prizes
            selectedPrizes.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Call the createTeamForm
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        public void TeamComplete(TeamModel model)
        {
            // Get back from the form with TeamModel
            // Take the TeamModel and put it into our list os selected prizes
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;

            if (p != null)
            {
                selectedPrizes.Remove(p);

                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            // Validate data
            bool feeAcceptable = decimal.TryParse(entryFeeText.Text, out decimal entryFee);

            if (!feeAcceptable)
            {
                MessageBox.Show(
                    "You need to anter a valid Entry Fee.", 
                    "Invalid Fee", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);

                return;
            }

            // Create ou tournament model
            TournamentModel tm = new TournamentModel();

            tm.TournamentName = tournamentNameText.Text;
            tm.EntryFee = entryFee;
            tm.Prizes = selectedPrizes;
            tm.EnteredTeams = selectedTeams;

            // Wire our matchups
            TournamentLogic.CreateRounds(tm);

            // Create Tournamaent entry
            // Create all of the prizes entries
            // Create all of the teams entries
            // FIXME: Address issue of 2nd roud's second team when bye exists
            GlobalConfig.Connection.CreateTournament(tm);

            tm.AlertUsersToNewRound();

            TournamentViewerForm frm = new TournamentViewerForm(tm);
            frm.Show();
            this.Close();
        }
    }
}
