using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PersonModels.csv";
        public const string TeamFile = "TeamModels.csv";
        public const string TournamentFile = "TournamentModels.csv";
        public const string MatchupFile = "MatchupModels.csv";
        public const string MatchupEntryFile = "MatchupEntryModels.csv";

        /// <summary>
        /// Private Set so only methods inside of this class can change the value of Connections
        /// But everyone can read it.
        /// A list because we might have multiple database to save and pull from
        /// </summary>
        public static IDataConnection Connection { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="textFiles"></param>
        public static void InitializeConnections(DatabaseType db)
        {
            // Since the parameter type is a bool you don't have to do a comparison
            if (db == DatabaseType.Sql)
            {
                // TODO - Set up the SQL Connector properly
                SqlConnector sql = new SqlConnector();
                Connection = sql;
            }
            
            else if (db == DatabaseType.TextFile)
            {
                // TODO - Create the Tect Connection
                TextConnector text = new TextConnector();
                Connection = text;
            }
        }

        public static string CnnString(string name)
        {
            // Going to App.Config to get me the connection string by looking
            // up the name we gave it. It's going to return the ConnectionString attribute vALUE
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static string AppKeyLookup(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
