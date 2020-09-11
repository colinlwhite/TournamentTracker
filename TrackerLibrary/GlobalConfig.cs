using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        /// <summary>
        /// Private Set so only methods inside of this class can change the value of Connections
        /// But everyone can read it.
        /// A list because we might have multiple database to save and pull from
        /// </summary>
        public static List<IDataConnection> Connections { get; private set; } = new List<IDataConnection>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="textFiles"></param>
        public static void InitializeConnections(bool database, bool textFiles)
        {
            // Since the parameter type is a bool you don't have to do a comparison
            if (database)
            {
                // TODO - Set up the SQL Connector properly
                SqlConnector sql = new SqlConnector();
                Connections.Add(sql);
            }
            
            if (textFiles)
            {
                // TODO - Create the Tect Connection
                TextConnector text = new TextConnector();
                Connections.Add(text);
            }
        }
    }
}
