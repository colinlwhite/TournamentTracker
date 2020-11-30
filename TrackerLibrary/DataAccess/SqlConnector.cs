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
        private const string db = "Tournaments";
        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                // DynamicParameters a method from Dapper
                var p = new DynamicParameters();
                // The parameter inside SSMS and value we're passing in
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellPhoneNumber", model.CellPhoneNumber);
                // This is unique because it's coming out, rather than being sent in, Therefore:
                // So we're going to pass a value in of 0 and say dbType is an Int32
                // ParameterDirection is an enum, so it's enumerating the options we have available to us
                // By default, it's an input that why this isn't specified in the aforementioned parameters
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Running the stored procedure
                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);
                // <T> Generic example because we've specified the type as "int"
                model.Id = p.Get<int>("@id");

                return model;
            }
        }

        // TODO - Make the CreatePrize method actually save to the database
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information</param>
        /// <returns>The prize information, including the unique indentifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
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

        public TeamModel CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                foreach (PersonModel tm in model.TeamMembers)
                {
                    // A new insert for each teamMember
                    p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@personId", tm.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }

                return model;
            }
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                SaveTournament(model, connection);
                SaveTournamentPrizes(model, connection);
                SaveTournamentEntries(model, connection);
                SaveTournamentRounds(model, connection);
            }
        }

        private void SaveTournament(TournamentModel model, IDbConnection connection)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentName", model.TournamentName);
            p.Add("@EntryFee", model.EntryFee);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@id");
        }
        private void SaveTournamentPrizes(TournamentModel model, IDbConnection connection)
        {
            foreach (PrizeModel pz in model.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@PrizeId", pz.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }
        private void SaveTournamentEntries(TournamentModel model, IDbConnection connection)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@TeamId", tm.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);

            }
        }
        private void SaveTournamentRounds(TournamentModel model, IDbConnection connection)
        {
            // Loop through the Rounds
            foreach (List<MatchupModel> round in model.Rounds)
            {
                // Loop through the matchups
                // Because each Round is comprised of a List<MatchupModel>
                foreach (MatchupModel matchup in round)
                {
                    // save the matchups via the Dapper pattern and using the stored procedure
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", model.Id);
                    p.Add("@MatchupRound", matchup.MatchupRound);
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_Insert", p, commandType: CommandType.StoredProcedure);

                    matchup.Id = p.Get<int>("@id");

                    // Looping through each entry to save them
                    foreach (MatchupEntryModel matchupEntry in matchup.Entries)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId", matchup.Id);
                        if (matchupEntry.ParentMatchup == null)
                        {
                            p.Add("@ParentMatchupId", null);
                        }
                        else
                        {
                            p.Add("@ParentMatchupId", matchupEntry.ParentMatchup.Id );
                        }
                        // p.Add("@ParentMatchupId", matchupEntry.ParentMatchup);
                        // check for null value
                        if (matchupEntry.TeamCompeting == null)
                        {
                            p.Add("@TeamCompetingId", null);
                        } 
                        else
                        {
                            p.Add("@TeamCompetingId", matchupEntry.TeamCompeting.Id);
                        }
                        // p.Add("@TeamCompetingId", matchupEntry.TeamCompeting.Id);
                        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
                    }

                }
            }
        }

        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeam_GetAll").ToList();

                foreach (TeamModel team in output)
                {
                    var tm = new DynamicParameters();
                    tm.Add("@TeamId", team.Id);

                    team.TeamMembers = connection.Query<PersonModel>("spTeamMembers_GetByTeam", tm, commandType: CommandType.StoredProcedure).ToList();

                }
            }

            return output;
        }
    }
}
