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
            TeamModel team = (TeamModel)selectTeamDropDown.SelectedItem;

            if (team != null)
            {
                availableTeams.Remove(team);
                selectedTeams.Add(team);

                WireUpLists();
            }
        }

        private void removeSelectedPlayerButton_Click(object sender, EventArgs e)
        {
            TeamModel team = (TeamModel)tournamentTeamsListBox.SelectedItem;

            if (team != null)
            {
                selectedTeams.Remove(team);
                availableTeams.Add(team);

                WireUpLists();
            }
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // Call the createPrizeForm
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();
        }

        public void PrizeComplete(PrizeModel prize)
        {
            // Get back from the form with PrizeModel
            // Take the PrizeModel and put it into our list os selected prizes
            selectedPrizes.Add(prize);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Call the createTeamForm
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        public void TeamComplete(TeamModel team)
        {
            // Get back from the form with TeamModel
            // Take the TeamModel and put it into our list os selected prizes
            selectedTeams.Add(team);
            WireUpLists();
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel prize = (PrizeModel)prizesListBox.SelectedItem;

            if (prize != null)
            {
                selectedPrizes.Remove(prize);

                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            // Validate data
            decimal entryFee = 0;
            bool feeAcceptable = decimal.TryParse(entryFeeText.Text, out entryFee);

            if (!feeAcceptable)
            {
                MessageBox.Show("You need to anter a valid Entry Fee.", 
                    "Invalid Fee", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);

                return;
            }

            // Create ou tournament model
            TournamentModel tournament = new TournamentModel();

            tournament.TournamentName = tournamentNameText.Text;
            tournament.EntryFee = entryFee;
            tournament.Prizes = selectedPrizes;
            tournament.EnteredTeams = selectedTeams;

            // Wire our matchups
            TournamentLogic.CreateRounds(tournament);

            // Create Tournamaent entry
            // Create all of the prizes entries
            // Create all of the teams entries
            GlobalConfig.Connection.CreateTournament(tournament);

        }
    }
}
