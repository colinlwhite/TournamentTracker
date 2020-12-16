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
        List<MatchupModel> selectedMatchups = new List<MatchupModel>();
        public TournamentViewerForm(TournamentModel tournametModel)
        {
            InitializeComponent();

            tournament = tournametModel;

            LoadFormData();

            LoadRounds();
        }

        /// <summary>
        /// This will take the tournament object and populate the information we need for the form
        /// </summary>
        private void LoadFormData()
        {
            // Populating the Tournament Name Example:
            tournamentName.Text = tournament.TournamentName;
        }

        private void WireUpRoundsLists()
        {
            roundDropDown.DataSource = null;
            roundDropDown.DataSource = rounds;
        }

        private void WireUpMatchupsLists()
        {
            matchupListBox.DataSource = null;
            matchupListBox.DataSource = selectedMatchups;
            matchupListBox.DisplayMember = "DisplayName";

        }

        private void LoadRounds()
        {
            // We're initilizing the rounds everytime because if we don't refresh the list we could have duplicate rounds
            rounds = new List<int>();

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

            WireUpRoundsLists();
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups();
        }

        private void LoadMatchups()
        {
            int round = (int)roundDropDown.SelectedItem;

            /*Each round is a list of games so we're just checking one
             games' round information 
             */
            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                // If the random game we're looping through is
                // the game the user selected lets show them 
                // All the games
                if (matchups.First().MatchupRound == round)
                {
                    // Add the random game we're looping though
                    // to the list we're going to show the user
                    selectedMatchups = matchups;
                }
            }

            WireUpMatchupsLists();
        }
    }
}
