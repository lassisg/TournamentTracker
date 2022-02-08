using System.Configuration;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        /// <summary>
        /// Represents the full path for the teaxt file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Full file path, including the file name</returns>
        public static string FullFilePath(this string fileName)
        {
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
        }

        public static List<string> LoadFile (this string file)
        {
            if(!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                PrizeModel prize = new PrizeModel();

                prize.Id = int.Parse(columns[0]);
                prize.PlaceNumber = int.Parse(columns[1]);
                prize.PlaceName = columns[2];
                prize.PrizeAmount = decimal.Parse(columns[3]);
                prize.PrizePercentage = double.Parse(columns[4]);

                output.Add(prize);
            }
            
            return output;
        }

        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                PersonModel person = new PersonModel();

                person.Id = int.Parse(columns[0]);
                person.FirstName = columns[1];
                person.LastName = columns[2];
                person.EmailAddress = columns[3];
                person.CellphoneNumber = columns[4];

                output.Add(person);
            }

            return output;
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines)
        {
            // id, tem name, list of person id separated by pipe
            // 1,Team First Place, 1|3|6
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (var line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel team = new TeamModel();

                team.Id = int.Parse(cols[0]);
                team.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');

                foreach (var id in personIds)
                {
                    team.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }

                output.Add(team);
            }

            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels (this List<string> lines)
        {
            // id, TournamentName, EntryFee, Entered Teams - id|id|id, Prizes - id|id|id, Rounds - id-id-id|id-id-id|id-id-id
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TournamentModel tournament = new TournamentModel();

                tournament.Id = int.Parse(cols[0]);
                tournament.TournamentName = cols[1];
                tournament.EntryFee = decimal.Parse(cols[2]);
                
                string[] teamIds = cols[3].Split('|');
                foreach(string id in teamIds)
                {
                    tournament.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                if (cols[4].Length > 0)
                {
                    string[] prizeIds = cols[4].Split('|');
                    foreach (string id in prizeIds)
                    {
                        tournament.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                    } 
                }

                string[] rounds = cols[5].Split('|');

                foreach(string round in rounds)
                {
                    string[] msText = round.Split('-');
                    List<MatchupModel> ms = new List<MatchupModel>();

                    foreach(string matchupModelTextId in msText)
                    {
                        ms.Add(matchups.Where(x => x.Id == int.Parse(matchupModelTextId)).First());
                    }

                    tournament.Rounds.Add(ms);
                }

                output.Add(tournament);
            }

            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
            }

            File.WriteAllLines(GlobalConfig.PrizesFile.FullFilePath(), lines);
        }

        public static void SaveToPersonFile(this List<PersonModel> models)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel p in models)
            {
                lines.Add($"{ p.Id },{ p.FirstName },{ p.LastName },{ p.EmailAddress },{ p.CellphoneNumber }");
            }

            File.WriteAllLines(GlobalConfig.PeopleFile.FullFilePath(), lines);
        }

        public static void SaveToTeamsFile(this List<TeamModel> models)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in models)
            {
                lines.Add($"{ t.Id },{ t.TeamName },{ ConvertPeopleListToString(t.TeamMembers) }");
            }

            File.WriteAllLines(GlobalConfig.TeamsFile.FullFilePath(), lines);
        }

        public static void SaveToTournamentFile(this List<TournamentModel> models)
        {
            List<string> lines = new List<string>();
            // id, TournamentName, EntryFee, Entered Teams - id|id|id, Prizes - id|id|id, Rounds - id-id-id|id-id-id|id-id-id
            foreach (TournamentModel tm in models)
            {
                lines.Add($"{ tm.Id },{ tm.TournamentName },{ tm.EntryFee },{ ConvertTeamListToString(tm.EnteredTeams) },{ ConvertPrizeListToString(tm.Prizes) },{ ConvertRoundsListToString(tm.Rounds) }");
            }

            File.WriteAllLines(GlobalConfig.TournamentsFile.FullFilePath(), lines);
        }

        public static void SaveRoundsToFile(this TournamentModel model)
        {
            // Loop through each round
            // Loop through each matchup
            // Get the Id for the new matchup and save the record
            // Loop through each Entry , get the id, and save it

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    // Load all of the matchups from file
                    // Get the top id and add 1
                    // Store the id
                    // Save the matchup record

                    matchup.SaveMatchupToFile();

                }
            }
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels (this List<string> lines)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                MatchupEntryModel matchupEntry = new MatchupEntryModel();

                matchupEntry.Id = int.Parse(columns[0]);
                if (columns[1].Length == 0)
                {
                    matchupEntry.TeamCompeting = null;
                }
                else
                {
                    matchupEntry.TeamCompeting = LookupTeamById(int.Parse(columns[1]));
                }
                matchupEntry.Score = double.Parse(columns[2]);

                int parentId = 0;
                if (int.TryParse(columns[3], out parentId))
                {
                    matchupEntry.ParentMatchup = LookupMatchupById(parentId);
                }
                else 
                { 
                    matchupEntry.ParentMatchup = null;
                }

                output.Add(matchupEntry);
            }

            return output;
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            // id=0,entries=1(pipe delimited by id),winner=2,matchupRound=3
            List<MatchupModel> output = new List<MatchupModel>();

            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                MatchupModel matchup = new MatchupModel();

                matchup.Id = int.Parse(columns[0]);
                matchup.Entries = ConvertStringToMactchupEntryModel(columns[1]);
                if(columns[2].Length == 0)
                {
                    matchup.Winner = null;
                }
                else
                {
                    matchup.Winner = LookupTeamById(int.Parse(columns[2]));
                }
                matchup.MatchupRound = int.Parse(columns[3]);

                output.Add(matchup);
            }

            return output;
        }

        private static List<MatchupEntryModel> ConvertStringToMactchupEntryModel(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            foreach (string id in ids)
            {
                foreach (string entry in entries)
                {
                    string[] cols = entry.Split(',');

                    if (cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
            }
            
            output = matchingEntries.ConvertToMatchupEntryModels();

            return output;
        }

        private static TeamModel LookupTeamById (int id)
        {
            List<string> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile();

            foreach (string team in teams)
            {
                string[] cols = team.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);

                    return matchingTeams.ConvertToTeamModels().First();
                }
            }

            return null;
        }

        private static MatchupModel LookupMatchupById (int id)
        {
            List<string> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile();

            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);

                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }

            return null;
        }

        public static void SaveMatchupToFile(this MatchupModel matchup)
        {

            List<MatchupModel> matchups = GlobalConfig.MatchupsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupModels();

            int currentId = 1;

            if (matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }

            matchup.Id = currentId;
            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntriesToFile();
            }

            // save to file
            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchupRound }");
            }

            File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);
        }

        public static void UpdateMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupModels();

            MatchupModel oldMatchup = new MatchupModel();

            foreach (MatchupModel m in matchups)
            {
                if (m.Id == matchup.Id)
                {
                    oldMatchup = m;
                }
            }

            matchups.Remove(oldMatchup);

            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.UpdateEntriesToFile();
            }

            // save to file
            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchupRound }");
            }

            File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);
        }

        public static void SaveEntriesToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupEntryModels();

            int currentId = 1;

            if (entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }

            entry.Id = currentId;
            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.Id.ToString();
                }
                lines.Add($"{ e.Id },{ teamCompeting },{ e.Score },{ parent }");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);
        }

        public static void UpdateEntriesToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupEntryModels();

            MatchupEntryModel oldEntry = new MatchupEntryModel();

            foreach (MatchupEntryModel e in entries)
            {
                if (e.Id == oldEntry.Id)
                {
                    oldEntry = e;
                }
            }

            entries.Remove(oldEntry);

            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.Id.ToString();
                }
                lines.Add($"{ e.Id },{ teamCompeting },{ e.Score },{ parent }");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);
        }

        public static string ConvertRoundsListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";

            if (rounds.Count == 0)
            {
                return output;
            }

            foreach (List<MatchupModel> round in rounds)
            {
                output += $"{ ConvertMatchupListToString(round) }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }
        
        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";

            if (matchups.Count == 0)
            {
                return output;
            }

            foreach (MatchupModel matchup in matchups)
            {
                output += $"{ matchup.Id }-";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
            {
                return output;
            }

            foreach (TeamModel team in teams)
            {
                output += $"{ team.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return output;
            }

            foreach (PrizeModel prize in prizes)
            {
                output += $"{ prize.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";

            if (entries.Count == 0)
            {
                return output;
            }

            foreach (MatchupEntryModel e in entries)
            {
                output += $"{ e.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return output;
            }

            foreach (PersonModel person in people)
            {
                output += $"{ person.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

    }
}
