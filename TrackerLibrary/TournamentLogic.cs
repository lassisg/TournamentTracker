using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        public static void CreateRounds(TournamentModel tournament)
        {
            // Order our list randomly of teams
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(tournament.EnteredTeams);

            // Check if it is big enough - if not, add in byes (2*2*2*2 - 2^4)
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            // Create our first round of matchups
            tournament.Rounds.Add(CreateFirstRound(byes, randomizedTeams));

            // Create every round after that - 8 matchups -> 4 matchups -> 2 matchups -> 1 matchup
            CreateOtherRounds(tournament, rounds);

        }

        private static List<TeamModel> RandomizeTeamOrder (List<TeamModel> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output += 1;
                val *= 2;
            }

            return output;
        }

        private static int NumberOfByes (int rounds, int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }

            output = totalTeams - numberOfTeams;

            return output;
        }

        private static List<MatchupModel> CreateFirstRound (int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel current = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                current.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                if (byes > 0 || current.Entries.Count > 1)
                {
                    current.MatchupRound = 1;
                    output.Add(current);
                    current = new MatchupModel();

                    if (byes > 0)
                    {
                        byes -= 1;
                    }
                }
            }

            return output;

        }

        private static void CreateOtherRounds (TournamentModel tournament, int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = tournament.Rounds[0];
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel();


            while (round <= rounds)
            {
                foreach(MatchupModel match in previousRound)
                {
                    currentMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

                    if (currentMatchup.Entries.Count > 1)
                    {
                        currentMatchup.MatchupRound = round;
                        currentRound.Add(currentMatchup);
                        currentMatchup = new MatchupModel();
                    }
                }

                tournament.Rounds.Add(currentRound);
                previousRound = currentRound;

                currentRound = new List<MatchupModel>();

                round += 1;
            }
        }
    }
}
