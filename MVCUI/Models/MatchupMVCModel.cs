﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCUI.Models
{
    public class MatchupMVCModel
    {

        // List of matchups or games
        // MatchupID to post scores who won
        // MatchupEntryId (2)
        // Associated Team Name of the aforementioned (2)

        public int MatchupId { get; set; }
        public int FirstTeamMatchupEntryId { get; set; }
        public string FirstTeamName { get; set; }
        public double FirstTeamScore { get; set; }
        public int SecondTeamMatchupEntryId { get; set; }
        public string SecondTeamName { get; set; }
        public double SecondTeamScore { get; set; }
    }
}