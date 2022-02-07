using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        // TODO: Remove consts below
        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PersonModels.csv";
        private const string TeamsFile = "TeamModels.csv";
        private const string TournamentsFile = "TournamentModel.csv";
        private const string MatchupsFile = "MatchupModel.csv";
        private const string MatchupEntriesFile = "MatchupEntryModel.csv";

        public void CreatePerson(PersonModel model)
        {
            // Load the text fileand convert the text to List<PrizeModel>
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            // Find the last ID
            int currentId = 1;
            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            // Add new record with the new ID (last Id + 1)
            people.Add(model);

            // Convert the people to List<string> and save to the text file
            people.SaveToPersonFile(PeopleFile);

        }

        /// <summary>
        /// Saves a prize to a file.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            // Load the text fileand convert the text to List<PrizeModel>
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Find the last ID
            int currentId = 1;
            if (prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }
            
            model.Id = currentId;

            // Add new record with the new ID (last Id + 1)
            prizes.Add(model);

            // Convert the prizes to List<string> and save to the text file
            prizes.SaveToPrizeFile(PrizesFile);

        }

        /// <summary>
        /// Saves a team to a file.
        /// </summary>
        /// <param name="team"></param>
        /// <returns>The team information, including the unique identifier.</returns>
        public void CreateTeam(TeamModel team)
        {
            // Load the text fileand convert the text to List<TeamModel>
            List<TeamModel> teams = TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

            // Find the last ID
            int currentId = 1;
            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }

            team.Id = currentId;

            // Add new record with the new ID (last Id + 1)
            teams.Add(team);

            // Convert the people to List<string> and save to the text file
            teams.SaveToTeamsFile(TeamsFile);

        }

        public void CreateTournament(TournamentModel tournament)
        {
            List<TournamentModel> tournaments = TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels(TeamsFile, PeopleFile, PrizesFile);
            
            // Find the last ID
            int currentId = 1;
            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }

            tournament.Id = currentId;

            tournament.SaveRoundsToFile(MatchupsFile, MatchupEntriesFile);

            tournaments.Add(tournament);

            tournaments.SaveToTournamentFile(TournamentsFile);

        }

        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            return TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
        }

        public List<TournamentModel> GetTournament_All()
        {
            return TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels(TeamsFile, PeopleFile, PrizesFile);
        }

        public void UpdateMatchup(MatchupModel matchup)
        {
            matchup.UpdateMatchupToFile();
        }
    }
}