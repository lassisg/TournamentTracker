using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        public void CreatePerson(PersonModel model)
        {
            // Load the text fileand convert the text to List<PrizeModel>
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

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
            people.SaveToPersonFile();

        }

        /// <summary>
        /// Saves a prize to a file.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            // Load the text fileand convert the text to List<PrizeModel>
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

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
            prizes.SaveToPrizeFile();

        }

        /// <summary>
        /// Saves a team to a file.
        /// </summary>
        /// <param name="team"></param>
        /// <returns>The team information, including the unique identifier.</returns>
        public void CreateTeam(TeamModel team)
        {
            // Load the text fileand convert the text to List<TeamModel>
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();

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
            teams.SaveToTeamsFile();

        }

        public void CreateTournament(TournamentModel tournament)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();
            
            // Find the last ID
            int currentId = 1;
            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }

            tournament.Id = currentId;

            tournament.SaveRoundsToFile();

            tournaments.Add(tournament);

            tournaments.SaveToTournamentFile();

        }

        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
        }

        public List<TournamentModel> GetTournament_All()
        {
            return GlobalConfig.TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();
        }

        public void UpdateMatchup(MatchupModel matchup)
        {
            matchup.UpdateMatchupToFile();
        }
    }
}