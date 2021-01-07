using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace MVCUI.Controllers
{
    public class PeopleController : Controller
    {
        // GET: People
        public ActionResult Index()
        {
            List<PersonModel> availablePeople = GlobalConfig.Connection.GetPerson_All();

            return View(availablePeople);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                PersonModel p = new PersonModel();

                p.FirstName = collection["FirstName"];
                p.LastName = collection["LastName"];
                p.EmailAddress = collection["EmailAddress"];
                p.CellPhoneNumber = collection["CellPhoneNumber"];

                GlobalConfig.Connection.CreatePerson(p);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
