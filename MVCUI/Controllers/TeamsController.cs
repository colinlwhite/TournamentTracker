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
    public class TeamsController : Controller
    {
        // GET: Teams
        public ActionResult Index()
        {
            List<TeamModel> allTeams = GlobalConfig.Connection.GetTeam_All();
            return View(allTeams);
        }

        // GET: Teams/Create
        public ActionResult Create()
        {
            // Get the people
            List<PersonModel> availablePlayers = GlobalConfig.Connection.GetPerson_All();

            // Create a TeamMVCModel
            TeamMVCModel model = new TeamMVCModel();

            // Populate TeamMVCModel.TeamMembers via LINQ because it's a SelectListItem type
            model.TeamMembers = availablePlayers.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id.ToString() }).ToList();

            return View(model);
        }

        // POST: Teams/Create
        [ValidateAntiForgeryToken()]
        [HttpPost]
        public ActionResult Create(TeamMVCModel model)
        {
            try
            {
                if (ModelState.IsValid && model.SelectedTeamMembers.Count > 0)
                {
                    TeamModel t = new TeamModel();
                    t.TeamName = model.TeamName;
                    t.TeamMembers = model.SelectedTeamMembers.Select(x => new PersonModel { Id = int.Parse(x) }).ToList();

                    GlobalConfig.Connection.CreateTeam(t);

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Create");
                }
            }
            catch
            {
                return View();
            }
        }

    }
}
