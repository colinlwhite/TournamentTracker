using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;
        List<int> rounds = new List<int>();
        public TournamentViewerForm(TournamentModel tournametModel)
        {
            InitializeComponent();

            tournament = tournametModel;

            LoadFormData();
        }

        /// <summary>
        /// This will take the tournament object and populate the information we need for the form
        /// </summary>
        private void LoadFormData()
        {
            // Populating the Tournament Name Example:
            tournamentName.Text = tournament.TournamentName;

        }

        private void LoadRounds()
        {
            // We'll always have one round minimum
            rounds.Add(1);
            int currentRound = 1;
            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound > currentRound)
                {
                    currentRound = matchups.First().MatchupRound;
                    rounds.Add(currentRound);
                }
            }
        }
    }
}
