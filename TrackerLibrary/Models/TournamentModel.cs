﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one tournament, with all of the rounds, matchups, prizes and outcomes. 
    /// </summary>
    public class TournamentModel
    {
        public event EventHandler<DateTime> OnTournamentComplete;
        /// <summary>
        /// Unique indentifier for the tournament. 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name given to this tournament.
        /// </summary>
        public string TournamentName { get; set; }
        /// <summary>
        /// The amount ofmoney each team needs to put up to enter.
        /// </summary>
        public decimal EntryFee { get; set; }
        /// <summary>
        /// The set of teams that have been entered.
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();
        /// <summary>
        /// The list of prizes for the various places.
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        /// <summary>
        /// The matchups per round.
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
        /// <summary>
        /// Fires off the event above - invokes the event
        /// '?' If you have a subscriber, fire the event
        /// </summary>
        public void CompleteTourament()
        {
            OnTournamentComplete?.Invoke(this, DateTime.Now);
        }
    }
}
