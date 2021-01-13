using MVCUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace MVCUI.Controllers
{
    public class TournamentsController : Controller
    {
        // GET: Tournaments
        public ActionResult Index()
        {
            return RedirectToAction("Index","Home");
        }

        public ActionResult Details(int id, int roundId = 0)
        {
            List<TournamentModel> tournaments = GlobalConfig.Connection.GetTournament_All();

            try
            {
                TournamentMVCDetailsModel input = new TournamentMVCDetailsModel();
                TournamentModel t = tournaments.Where(x => x.Id == id).First();

                input.TournamentName = t.TournamentName;

                var orderedRounds = t.Rounds.OrderBy(x => x.First().MatchupRound).ToList();
                bool activeRound = false;

                for (int i = 0; i < orderedRounds.Count; i++)
                {
                    RoundStatus status = RoundStatus.Locked;

                    if (!activeRound)
                    {
                        if (orderedRounds[i].TrueForAll(x => x.Winner != null))
                        {
                            status = RoundStatus.Complete;
                        }
                        else
                        {
                            status = RoundStatus.Active;
                            activeRound = true;
                            if (roundId == 0)
                            {
                                // Setting the roundId to be the active round number
                                roundId = i + 1;
                            }
                        } 
                    }

                    // "i + 1" because our loop starts at 0, hence "int i = 0"
                    input.Rounds.Add(
                        new RoundMVCModel 
                        { 
                            RoundName = "Round " + (i + 1), 
                            Status = status, 
                            RoundNumber = i + 1 
                        });
                }

                // Converting a 1 based list to a 0 based list for lookup
                input.Matchups = GetMatchups(orderedRounds[roundId - 1]);

                return View(input);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

        }

        private List<MatchupMVCModel> GetMatchups(List<MatchupModel> roundMatchups)
        {
            List<MatchupMVCModel> output = new List<MatchupMVCModel>();

            foreach (var game in roundMatchups)
            {
                int teamTwoId = 0;
                string teamTwoName = "Bye";
                double teamTwoScore = 0;

                if (game.Entries.Count > 1)
                {
                    teamTwoId = game.Entries[1].Id;
                    teamTwoName = game.Entries[1].TeamCompeting.TeamName;
                    teamTwoScore = game.Entries[1].Score;
                }

                output.Add(new MatchupMVCModel { 
                    MatchupId = game.Id,
                    FirstTeamMatchupEntryId = game.Entries[0].Id,
                    FirstTeamName = game.Entries[0].TeamCompeting.TeamName,
                    FirstTeamScore = game.Entries[0].Score,
                    SecondTeamMatchupEntryId = teamTwoId,
                    SecondTeamName = teamTwoName,
                    SecondTeamScore = teamTwoScore
                });
            }

            return output;
        }

        public ActionResult Create()
        {
            TournamentMVCCreateModel input = new TournamentMVCCreateModel();

            List<TeamModel> allTeams = GlobalConfig.Connection.GetTeam_All();
            List<PrizeModel> allPrizes = GlobalConfig.Connection.GetPrizes_All();

            input.EnteredTeams = allTeams.Select(x => new SelectListItem { Text = x.TeamName, Value = x.Id.ToString() }).ToList();
            input.Prizes = allPrizes.Select(x => new SelectListItem { Text = x.PlaceName, Value = x.Id.ToString() }).ToList();

            return View(input);
        }

        // POST: People/Create
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(TournamentMVCCreateModel model)
        {
            try
            {
                if (ModelState.IsValid && model.SelectedEnteredTeams.Count > 0)
                {
                    TournamentModel t = new TournamentModel();
                    t.TournamentName = model.TournamentName;
                    t.EntryFee = model.EntryFee;
                    t.EnteredTeams = model.SelectedEnteredTeams.Select(x => new TeamModel { Id = int.Parse(x) }).ToList();
                    t.Prizes = model.SelectedPrizes.Select(x => new PrizeModel { Id = int.Parse(x) }).ToList();

                    // Wire up the matchups
                    TournamentLogic.CreateRounds(t);

                    GlobalConfig.Connection.CreateTournament(t);

                    t.AlertUsersToNewRound();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Create");
                }
            }
            catch(Exception ex)
            {
                return View();
            }
        }
    }
}