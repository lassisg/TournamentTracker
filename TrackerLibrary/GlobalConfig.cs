using System.Configuration;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{ 
    public static class GlobalConfig
    {
        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PersonModels.csv";
        public const string TeamsFile = "TeamModels.csv";
        public const string TournamentsFile = "TournamentModel.csv";
        public const string MatchupsFile = "MatchupModel.csv";
        public const string MatchupEntriesFile = "MatchupEntryModel.csv";

        public static IDataConnection Connection { get; private set; }

        public static void InitializeConnections(DatabaseType db)
        {
            switch (db)
            {
                case DatabaseType.Sql:
                    // TODO: Set up the SQL Connector properly
                    SqlConnector sql = new SqlConnector();
                    Connection = sql;
                    break;

                case DatabaseType.TextFile:
                    // TODO: Set up the Text Connector properly
                    TextConnector text = new TextConnector();
                    Connection = text;
                    break;

                default:
                    break;
            }
        }

        public static string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    
        public static string AppKeyLookup(string key)
        {
            return $"{ ConfigurationManager.AppSettings[key] }";
        }
    }
}