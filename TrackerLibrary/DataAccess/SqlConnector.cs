using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        // TODO - Make the CreatePrize method actually save to the database
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information</param>
        /// <returns>The prize information, including the unique indentifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                // DynamicParameters a method from Dapper
                var p = new DynamicParameters();
                // The parameter inside SSMS and value we're passing in
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                // This is unique because it's coming out, rather than being sent in, Therefore:
                // So we're going to pass a value in of 0 and say dbType is an Int32
                // ParameterDirection is an enum, so it's enumerating the options we have available to us
                // By default, it's an input that why this isn't specified in the aforementioned parameters
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Running the stored procedure
                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);
                // <T> Generic example because we've specified the type as "int"
                model.Id = p.Get<int>("@id");

                return model;
            }
        }
    }
}
