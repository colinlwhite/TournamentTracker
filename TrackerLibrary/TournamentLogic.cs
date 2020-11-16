using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {

        // 1. To create an initial Round, randomize all teams

        // 2. Check if we have enough teams to have an even, 2 teams each, bracket
        // 2, 4, 8, 16, 32.
        // If the we don't have (2, 4, 8, 16 or 32) teams, add in a byeweek or byeteam
        // 14 teams we'll need an additional 2 bye weeks
        // We have the correct number of teams if 2, to the N works



        public static void CreateRounds(TournamentModel model)
        {
            // 1. To create an initial Round, randomize all teams
            List<TeamModel> randomizedTeams = randomizeTeamOrder(model.EnteredTeams);

            // 2. Check if we have enough teams to have an even, 2 teams each, bracket
            int tournamentRounds = findNumberofRounds(randomizedTeams.Count);

            // Now, how do we know how many bye weeks we have? 
            int tournamentByeWeeks = findNumberofByeWeeks(tournamentRounds, randomizedTeams.Count);

            // 3. Create a 1st round of matchups since we already have the information
            model.Rounds.Add(CreateFirstRound(tournamentByeWeeks, randomizedTeams));

            CreateOtherRounds(model, tournamentRounds);

        }

        // 4. Create every round after the 1st round. We're dividing by 2 now. 8/2 = 4, 4/2 = 2
        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            // First round was created in its own method
            int currentRound = 2;
            // We have to know the previous round to move forward
            List<MatchupModel> previousRound = model.Rounds[0];

            while (currentRound <= rounds)
            {
                foreach (MatchupModel match in previousRound)
                {

                }
            }

        }


        // 3. Create a 1st round of matchups since it's already from a randomized list of even teams. We already have the information
        // The first round is unique because we already know who the teams are
        private static List<MatchupModel> CreateFirstRound(int byeWeeks, List<TeamModel> teams)
        {
            List<MatchupModel> firstRoundMatchups = new List<MatchupModel>();
            // One MatchupModel 
            MatchupModel currentMatchup = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                // Remember, Entries is a List<>.
                // Therefore a team is only half of a MatchupModel
                // Adding one entry
                currentMatchup.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                // if either is true, we're done with this current matchup
                // a byeweek will be the second team || we already have 2 teams
                if (byeWeeks > 0  || currentMatchup.Entries.Count > 1)
                {
                    // "1" is hardcoded because we're creating the 1st round, hence the method name
                    currentMatchup.MatchupRound = 1;
                    firstRoundMatchups.Add(currentMatchup);
                    // Reusing the variable now that it has been added to our output list
                    currentMatchup = new MatchupModel();

                    // if we still have bye weeks to distribute
                    if (byeWeeks > 0)
                    {
                        // Make sure you subtract the byeWeek we just used. 
                        byeWeeks -= 1;
                    }
                }
            }

            return firstRoundMatchups;
        }

        private static int findNumberofByeWeeks(int rounds, int initialTeamCount)
        {
            int byeWeeks = 0;
            int tournamentTotalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                tournamentTotalTeams *= 2;
            }

            byeWeeks = tournamentTotalTeams - initialTeamCount;

            return byeWeeks;
        }

        private static int findNumberofRounds(int teamCount)
        {
            int rounds = 1;
            int teamPairs = 2;

            while (teamPairs < teamCount)
            {
                rounds += 1;

                teamPairs *= 2;
            }

            return rounds;
        }

        // 1. To create an initial Round, randomize all teams
        private static List<TeamModel> randomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}
