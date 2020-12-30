using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one match or game in the tournament.
    /// </summary>
    public class MatchupModel
    {
        /// <summary>
        /// The unique identifier for the matchup.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The set of teams that were involved in this match
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        /// <summary>
        /// The ID from the database that will be used to look up or identify the winner
        /// </summary>
        public int WinnerId { get; set; }
        /// <summary>
        /// The winner of the match. 
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// The specific round that this match is a part of.
        /// </summary>
        public int MatchupRound { get; set; }
        /// <summary>
        /// Read only property with custom getter
        /// </summary>
        public string DisplayName
        {
            // Get the name based off of List<MatchupEntryModel> Entries
            get
            {
                string output = "";
                foreach (MatchupEntryModel me in Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = me.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs. { me.TeamCompeting.TeamName }";
                        }
                    }
                    else
                    {
                        output = "Matchup Not Yet Determined";
                        break;
                    }
                }
                return output;
            }
        }
            

    }
}
